using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public class FirstPersonCharacterController : BaseCharacterController
	{
		#region Profile
		[SerializeField] private CharacterControllerProfile profile;
		private bool profileInstantiated = false;
		public override CharacterControllerProfile Profile
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
			float dt = Time.fixedDeltaTime;
			MotionFixedUpdate(dt);
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

		#region Anatomy
		public override Transform Body => transform;

		[SerializeField] private Transform head;
		public override Transform Head => head;
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
		private void MotionFixedUpdate(float dt)
		{
			MovementUpdate(dt);
			OrientationUpdate(dt);
		}

		#region Movement
		public override Vector3 Position
		{
			get => Rigidbody.position;
			set => Rigidbody.position = value;
		}
		public override Vector3 Velocity => Rigidbody.velocity;

		public override Vector3 InputVelocity { get; set; }

		private void MovementUpdate(float dt)
		{
			NormalizeBufferedInputVelocity();
			Vector3 Impulse = CalculateImpulse(dt);
			Rigidbody.AddForce(Impulse, ForceMode.Impulse);

			// Reset buffered inputVelocity.
			InputVelocity = Vector3.zero;
		}

		void NormalizeBufferedInputVelocity()
		{
			float magnitude = InputVelocity.magnitude;
			if(!float.IsNormal(magnitude))
				InputVelocity = Vector3.zero;
			else if(magnitude > Profile.maxVelocity)
				InputVelocity *= Profile.maxVelocity / magnitude;
		}

		Vector3 CalculateImpulse(float dt)
		{
			if(dt <= 0.0f)
			{
				Debug.LogError("Delta time must be greater than zero.");
				return Vector3.zero;
			}

			Vector3 dv = InputVelocity - Velocity;
			Vector3 impulse = dv * Mass;

			float forceLimit = dv.magnitude * Mass / dt;
			if(Profile.limitAcceleration)
				forceLimit = Mathf.Min(forceLimit, Profile.maxForce);
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

		private void OrientationUpdate(float dt)
		{
			Vector3 r = Vector3.ClampMagnitude(InputAngularVelocity * dt, Profile.maxAngularVelocity);
			r = Quaternion.Inverse(HeadOrientation) * r;
			BodyOrientation *= Quaternion.Euler(Vector3.Project(r, Body.up));
			HeadOrientation *= Quaternion.Euler(Vector3.ProjectOnPlane(r, Body.up));

			InputAngularVelocity = Vector3.zero;
			Rigidbody.angularVelocity = Vector3.zero;
		}
		#endregion
		#endregion
	}
}
