using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi.UnityToolkit
{
	public partial class CharacterMotionControl : CharacterMode
	{
		#region Life cycle
		protected void Start()
		{
			Shape.onCollisionEnter += RecordGrounding;
			Shape.onCollisionStay += RecordGrounding;
			Shape.onCollisionExit += RemoveGrounding;
			onGrounded += JumpingOnGrounded;
		}

		protected void OnDestroy()
		{
			Shape.onCollisionEnter -= RecordGrounding;
			Shape.onCollisionStay -= RecordGrounding;
			Shape.onCollisionExit -= RemoveGrounding;
		}

		protected void FixedUpdate()
		{
			UpdateGrounding();

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

		public System.Action onGrounded;

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
				where Vector3.Angle(contact.normal, Shape.Up) <= movement.maxSlope
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
				onGrounded?.Invoke();
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
			Shape.ApplyImpulse(impulse);

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

			Vector3 dv = Vector3.ProjectOnPlane(InputVelocity - Shape.Velocity, Shape.Up);
			Vector3 impulse = dv * Shape.Mass;

			float forceLimit = dv.magnitude * Shape.Mass / dt;
			if(movement.limitAcceleration)
				forceLimit = Mathf.Min(forceLimit, movement.maxForce);
			if(!IsGrounded)
				forceLimit *= movement.ungroundedAttenuation;
			impulse = Vector3.ClampMagnitude(impulse, forceLimit * dt);

			return impulse;
		}

		private void PerformAutoStepping()
		{
			Vector3 offset =
				Vector3.ProjectOnPlane(Shape.Velocity, Shape.Up).normalized * movement.autoStepping.detectionRange
				+ Shape.Up * movement.autoStepping.height;
			if(!Shape.SweepCast(-Shape.Up, out RaycastHit hit, movement.autoStepping.height, offset))
				return;

			float dy = Vector3.Dot(hit.point - Shape.Body.position, Shape.Up);
			if(dy < movement.autoStepping.height * 0.05f)
				return;
			Lift(dy);
		}
		#endregion

		#region Orientation
		public Vector3 InputAngularVelocity
		{
			get => Shape.Head.rotation * InputLocalAngularVelocity;
			set => InputLocalAngularVelocity = Quaternion.Inverse(Shape.Head.rotation) * value;
		}
		public Vector3 InputLocalAngularVelocity { get; set; }

		private void UpdateOrientation(float dt)
		{
			Vector3 dEuler = InputLocalAngularVelocity * dt;
			if(orientation.limitAngularVelocity)
				dEuler = Vector3.ClampMagnitude(dEuler, orientation.maxAngularVelocity);

			// Limit zenith.
			{
				float afterZenith = Shape.HeadLocalEuler.x + dEuler.x;
				float dZenith = Mathf.Clamp(afterZenith, orientation.zenithRange.x, orientation.zenithRange.y) - afterZenith;
				dEuler.x += dZenith;
			}

			Shape.HeadLocalEuler += dEuler;

			// Limit azimuth.
			{
				float azimuth = Shape.HeadLocalEuler.y;
				float dAzimuth = azimuth - Mathf.Clamp(azimuth, -orientation.headAzimuthTolerance, orientation.headAzimuthTolerance);
				Vector3 dAzimuthEuler = new(0f, dAzimuth, 0f);
				Shape.HeadLocalEuler -= dAzimuthEuler;
				Shape.Body.rotation *= Quaternion.Euler(dAzimuthEuler);
			}

			InputAngularVelocity = Vector3.zero;
			Shape.AngularVelocity = Vector3.zero;
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

		private void Lift(float height)
		{
			float vy = Mathf.Sqrt(2f * Physics.gravity.magnitude * height);
			float dv = Mathf.Max(0, vy - Vector3.Dot(Shape.Velocity, Shape.Up));
			Shape.Velocity += Shape.Up * dv;
		}

		private IEnumerator JumpBuffer()
		{
			isJumpingBuffered = true;
			yield return new WaitForSeconds(jumping.bufferTime);
			isJumpingBuffered = false;
		}

		private void JumpingOnGrounded()
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
