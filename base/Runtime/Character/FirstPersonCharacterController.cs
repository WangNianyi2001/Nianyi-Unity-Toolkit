using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public class FirstPersonCharacterController : BaseCharacterController
	{
		#region Profile
		[SerializeField] private CharacterControllerProfile profile;
		private bool profileInstantiated = false;
		public CharacterControllerProfile Profile
		{
			get
			{
				if(profile == null)
					profile = Resources.Load<CharacterControllerProfile>("Fallback Character Controller Profile");
				if(!profileInstantiated)
				{
					profile = Instantiate(profile);
					profileInstantiated = true;
				}
				return profile;
			}
		}
		#endregion

		#region Life cycle
		protected void Start()
		{
			CapsuleInitialize();
		}

		protected void FixedUpdate()
		{
			MotionFixedUpdate();
		}
		#endregion

		#region Geometry
		private CapsuleCollider capsule;
		private CapsuleCollider Capsule
		{
			get
			{
				if(capsule == null)
					CapsuleInitialize();
				return capsule;
			}
		}
		private void CapsuleInitialize()
		{
			if(!TryGetComponent(out capsule))
				capsule = gameObject.AddComponent<CapsuleCollider>();

			Height = Profile.height;
			Radius = Profile.radius;
		}

		public override float Height
		{
			get => Capsule.height;
			set
			{
				Capsule.height = value;
				Capsule.center = Vector3.up * (value * .5f);
			}
		}

		public override float Radius
		{
			get => Capsule.radius;
			set => Capsule.radius = value;
		}
		#endregion

		#region Physics
#if DEBUG
		new
#endif
		private Rigidbody rigidbody;
		private Rigidbody Rigidbody
		{
			get
			{
				if(rigidbody == null)
					RigidbodyInitialize();
				return rigidbody;
			}
		}
		private void RigidbodyInitialize()
		{
			if(!TryGetComponent(out rigidbody))
				rigidbody = gameObject.AddComponent<Rigidbody>();

			Mass = Profile.mass;

			// TODO: Deprecate this.
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}

		public override float Mass
		{
			get => Rigidbody.mass;
			set => Rigidbody.mass = value;
		}
		#endregion

		#region Motion
		public override Vector3 Position => transform.position;
		public override Vector3 Velocity => Rigidbody.velocity;

		private Vector3 bufferedInputVelocity = Vector3.zero;
		public override Vector3 InputVelocity
		{
			get => Vector3.zero;
			set => bufferedInputVelocity = value;
		}

		public override Quaternion Orientation => transform.rotation;
		public override Quaternion FacingDirection => transform.rotation;

		private void MotionFixedUpdate()
		{
			NormalizeBufferedInputVelocity();
			Vector3 Impulse = CalculateImpulse(Time.fixedDeltaTime);
			Debug.Log(Impulse);
			Rigidbody.AddForce(Impulse, ForceMode.Impulse);
			// Reset buffered inputVelocity.
			bufferedInputVelocity = Vector3.zero;
		}

		void NormalizeBufferedInputVelocity()
		{
			float magnitude = bufferedInputVelocity.magnitude;
			if(!float.IsNormal(magnitude))
				bufferedInputVelocity = Vector3.zero;
			else if(magnitude > Profile.maxVelocity)
				bufferedInputVelocity *= Profile.maxVelocity / magnitude;
		}

		Vector3 CalculateImpulse(float dt)
		{
			if(dt <= 0.0f)
			{
				Debug.LogError("Delta time must be greater than zero.");
				return Vector3.zero;
			}

			Vector3 dv = bufferedInputVelocity - Velocity;
			Vector3 force = dv / dt * Mass;

			float forceLimit = bufferedInputVelocity.magnitude * Mass / dt;
			if(Profile.limitAcceleration)
				forceLimit = Mathf.Min(forceLimit, Profile.maxForce);
			float magnitude = force.magnitude;
			if(magnitude > forceLimit)
				force *= forceLimit / magnitude;

			return force / Mass;
		}
		#endregion
	}
}
