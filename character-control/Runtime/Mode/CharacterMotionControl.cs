using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public partial class CharacterMotionControl : CharacterMode
	{
		#region Life cycle
		protected void Start()
		{
			Character.onCollisionEnter += RecordGrounding;
			Character.onCollisionStay += RecordGrounding;
			Character.onCollisionExit += RemoveGrounding;
		}

		protected void OnDestroy()
		{
			Character.onCollisionEnter -= RecordGrounding;
			Character.onCollisionStay -= RecordGrounding;
			Character.onCollisionExit -= RemoveGrounding;
		}

		protected void FixedUpdate()
		{
			float dt = Time.fixedDeltaTime;
			UpdateMovement(dt);
			UpdateOrientation(dt);
		}
		#endregion

		#region Grounding
		protected class GroundingInfo
		{
			public Collider collider;
			public ContactPoint[] contacts;
		}
		protected Dictionary<Collider, GroundingInfo> groundings = new();

		public System.Action OnGrounded;

		// Use static array for buffering to prevent unnecessary frequent memory allocation.
		static ContactPoint[] groundingContacts = new ContactPoint[1];
		protected virtual void RecordGrounding(Collision collision)
		{
			if(groundingContacts.Length < collision.contactCount)
				groundingContacts = new ContactPoint[collision.contactCount];
			collision.GetContacts(groundingContacts);

			IEnumerable<ContactPoint> validContacts =
				// Make sure to take only the beginning elements because we're using a buffer array.
				from contact in groundingContacts.Take(collision.contactCount)
				where Vector3.Angle(contact.normal, Character.Up) <= movement.maxSlope
				select contact;

			bool wasGrounded = IsGrounded;

			GroundingInfo grounding = new()
			{
				collider = collision.collider,
				contacts = validContacts.ToArray(),
			};
			if(grounding.contacts.Length == 0)
				return;
			groundings[collision.collider] = grounding;

			if(!wasGrounded && IsGrounded)
				OnGrounded?.Invoke();
		}

		protected virtual void RemoveGrounding(Collision collision)
		{
			if(!groundings.ContainsKey(collision.collider))
				return;
			groundings.Remove(collision.collider);
		}

		protected virtual void UpdateGrounding()
		{
			CleanExpiredGroundings();
			if(IsGrounded)
			{
				lastGroundedTime = Time.time;
			}
		}

		protected virtual void CleanExpiredGroundings()
		{
			foreach(var collider in groundings.Keys.ToList())
			{
				if(collider != null)
					continue;
				groundings.Remove(collider);
			}
		}

		public bool IsGrounded => groundings.Count > 0;

		float lastGroundedTime = float.NegativeInfinity;
		#endregion

		#region Movement
		public Vector3 InputVelocity { get; set; }

		private void UpdateMovement(float dt)
		{
			// Normalize buffered input velocity.
			float magnitude = InputVelocity.magnitude;
			if(!float.IsNormal(magnitude))
				InputVelocity = Vector3.zero;
			else if(magnitude > movement.maxVelocity)
				InputVelocity *= movement.maxVelocity / magnitude;

			// Apply impulse.
			Vector3 impulse = CalculateMovementImpulse(dt);
			Shape.ProxyRigidbody.AddForce(impulse, ForceMode.Impulse);

			// Perform auto stepping.
			if(movement.useAutoStepping)
			{
				if(IsGrounded && InputVelocity.sqrMagnitude > 0f)
				{
					PerformAutoStepping();
				}
			}

			// Reset buffered inputVelocity.
			InputVelocity = Vector3.zero;
		}

		Vector3 CalculateMovementImpulse(float dt)
		{
			if(dt <= 0.0f)
			{
				Debug.LogError("Delta time must be greater than zero.");
				return Vector3.zero;
			}

			Vector3 dv = Vector3.ProjectOnPlane(InputVelocity - Shape.ProxyRigidbody.velocity, Character.Up);
			Vector3 impulse = dv * Shape.ProxyRigidbody.mass;

			float forceLimit = dv.magnitude * Shape.ProxyRigidbody.mass / dt;
			if(movement.limitAcceleration)
				forceLimit = Mathf.Min(forceLimit, movement.maxForce);
			if(!IsGrounded)
				forceLimit *= movement.ungroundedAttenuation;
			impulse = Vector3.ClampMagnitude(impulse, forceLimit * dt);

			return impulse;
		}

		void PerformAutoStepping()
		{
			// TODO: Cast.
			float radius = 0.3f;
			Vector3 castStart = Character.Position + Character.Up * radius
				+ Vector3.ProjectOnPlane(Shape.ProxyRigidbody.velocity, Character.Up).normalized * movement.autoStepping.detectionRange
				+ Character.Up * movement.autoStepping.height;
			bool hasHit = Physics.SphereCast(
				castStart, radius, -Character.Up,
				out RaycastHit hit,
				movement.autoStepping.height
			);
			if(!hasHit)
				return;

			float dy = Vector3.Dot(hit.point - Character.Position, Character.Up);
			if(dy < 1e-3f)
				return;
			Lift(dy);
		}
		#endregion

		#region Orientation
		public Quaternion BodyOrientation
		{
			get => Shape.Body.rotation;
			set => Shape.Body.rotation = value;
		}

		public Quaternion HeadOrientation
		{
			get => Shape.Head.transform.rotation;
			set => Shape.Head.transform.rotation = value;
		}

		public Vector3 InputAngularVelocity { get; set; }

		private void UpdateOrientation(float dt)
		{
			Vector3 r = Vector3.ClampMagnitude(InputAngularVelocity * dt, orientation.maxAngularVelocity);
			r = Quaternion.Inverse(HeadOrientation) * r;
			BodyOrientation *= Quaternion.Euler(Vector3.Project(r, Shape.Body.up));
			HeadOrientation *= Quaternion.Euler(Vector3.ProjectOnPlane(r, Shape.Body.up));

			InputAngularVelocity = Vector3.zero;
			Shape.ProxyRigidbody.angularVelocity = Vector3.zero;
		}
		#endregion

		#region Jumping
		protected bool CanJump
		{
			get
			{
				if(!useJumping)
					return false;
				if(IsGrounded)
					return true;
				if(!isJumping)
				{
					float coyoteTime = Time.time - lastGroundedTime;
					if(coyoteTime <= jumping.coyoteTime)
						return true;
				}
				else
				{
					if(midAirJumpingAllowance > 0)
						return true;
				}
				return false;
			}
		}
		/// <summary>Whether the character is mid-air due to the most recent active jumping.</summary>
		protected bool isJumping = false, isJumpingBuffered = false;
		protected int midAirJumpingAllowance = 0;

		public virtual void Jump()
		{
			if(!CanJump)
			{
				if(jumping.useBuffer)
					StartCoroutine(nameof(JumpBuffer));
				return;
			}

			if(!isJumping)
				isJumping = true;
			else
				--midAirJumpingAllowance;

			PerformJumping();
		}

		protected void PerformJumping()
		{
			Lift(jumping.height);
		}

		void Lift(float height)
		{
			float vy = Mathf.Sqrt(2f * Physics.gravity.magnitude * height);
			float dv = Mathf.Max(0, vy - Vector3.Dot(Shape.ProxyRigidbody.velocity, Character.Up));
			Shape.ProxyRigidbody.AddForce(Character.Up * dv, ForceMode.VelocityChange);
		}

		IEnumerator JumpBuffer()
		{
			isJumpingBuffered = true;
			yield return new WaitForSeconds(jumping.bufferTime);
			isJumpingBuffered = false;
		}

		void JumpingOnGrounded()
		{
			isJumping = false;
			midAirJumpingAllowance = jumping.midAirAllowance;
			if(isJumpingBuffered)
			{
				StopCoroutine(nameof(JumpBuffer));
				isJumpingBuffered = false;
				Jump();
			}
		}
		#endregion
	}
}
