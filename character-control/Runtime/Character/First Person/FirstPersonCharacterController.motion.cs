using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public partial class FirstPersonCharacterController
	{
		#region Life cycle
		private void MotionFixedUpdate(float dt)
		{
			UpdateMovement(dt);
			UpdateOrientation(dt);
		}
		#endregion

		#region Movement
		public override Vector3 Position
		{
			get => Rigidbody.position;
			set => Rigidbody.position = value;
		}
		public override Vector3 Velocity => Rigidbody.velocity;

		public override Vector3 InputVelocity { get; set; }

		private void UpdateMovement(float dt)
		{
			// Normalize buffered input velocity.
			float magnitude = InputVelocity.magnitude;
			if(!float.IsNormal(magnitude))
				InputVelocity = Vector3.zero;
			else if(magnitude > Profile.movement.maxVelocity)
				InputVelocity *= Profile.movement.maxVelocity / magnitude;

			// Apply impulse.
			Vector3 impulse = CalculateMovementImpulse(dt);
			Rigidbody.AddForce(impulse, ForceMode.Impulse);

			// Perform auto stepping.
			if(Profile.movement.useAutoStepping)
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

			Vector3 dv = Vector3.ProjectOnPlane(InputVelocity - Velocity, Up);
			Vector3 impulse = dv * Mass;

			float forceLimit = dv.magnitude * Mass / dt;
			if(Profile.movement.limitAcceleration)
				forceLimit = Mathf.Min(forceLimit, Profile.movement.maxForce);
			if(!IsGrounded)
				forceLimit *= Profile.movement.ungroundedAttenuation;
			impulse = Vector3.ClampMagnitude(impulse, forceLimit * dt);

			return impulse;
		}

		void PerformAutoStepping()
		{
			Vector3 castStart = Position + Up * Radius
				+ Vector3.ProjectOnPlane(Velocity, Up).normalized * Profile.movement.autoStepping.detectionRange
				+ Up * Profile.movement.autoStepping.height;
			bool hasHit = Physics.SphereCast(
				castStart, Radius, -Up,
				out RaycastHit hit,
				Profile.movement.autoStepping.height,
				collisionLayerMask, QueryTriggerInteraction.Ignore
			);
			if(!hasHit)
				return;

			float dy = Vector3.Dot(hit.point - Position, Up);
			if(dy < 1e-3f)
				return;
			Lift(dy);
		}
		#endregion

		#region Orientation
		public override Quaternion BodyOrientation
		{
			get => Rigidbody.rotation;
			set => Rigidbody.rotation = value;
		}

		public override Quaternion HeadOrientation
		{
			get => head.transform.rotation;
			set => head.transform.rotation = value;
		}

		public override Vector3 AngularVelocity => Rigidbody.angularVelocity;

		public override Vector3 InputAngularVelocity { get; set; }

		private void UpdateOrientation(float dt)
		{
			Vector3 r = Vector3.ClampMagnitude(InputAngularVelocity * dt, Profile.orientation.maxAngularVelocity);
			r = Quaternion.Inverse(HeadOrientation) * r;
			BodyOrientation *= Quaternion.Euler(Vector3.Project(r, Body.up));
			HeadOrientation *= Quaternion.Euler(Vector3.ProjectOnPlane(r, Body.up));

			InputAngularVelocity = Vector3.zero;
			Rigidbody.angularVelocity = Vector3.zero;
		}
		#endregion

		#region Jumping
		protected override void PerformJumping()
		{
			Lift(Profile.jumping.height);
		}

		void Lift(float height)
		{
			float vy = Mathf.Sqrt(2f * Physics.gravity.magnitude * height);
			float dv = Mathf.Max(0, vy - Vector3.Dot(Velocity, Up));
			Rigidbody.AddForce(Up * dv, ForceMode.VelocityChange);
		}
		#endregion
	}
}
