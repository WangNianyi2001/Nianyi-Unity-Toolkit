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
			impulse = Vector3.ClampMagnitude(impulse, forceLimit * dt);

			return impulse;
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
			Lift(Profile.jumping.jumpHeight);
		}

		private void Lift(float height)
		{
			float dv = Mathf.Sqrt(2f * Physics.gravity.magnitude * height);
			Rigidbody.AddForce(Up * dv, ForceMode.VelocityChange);
		}
		#endregion
	}
}
