using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public partial class FirstPersonCharacterController : BaseCharacterController
	{
		#region Profile
		protected override void ApplyProfile()
		{
			/* Capsule */
			CreateCapsule();
			Height = Profile.height;
			Radius = Profile.radius;
			Capsule.sharedMaterial = Profile.physicMaterial;

			/* Rigidbody */
			CreateRigidbody();
			Mass = Profile.mass;
			rigidbody.constraints =
				RigidbodyConstraints.FreezeRotationX |
				RigidbodyConstraints.FreezeRotationZ |
				(Profile.orientation.couldBeAffectedByPhysics ? 0 : RigidbodyConstraints.FreezeRotationY);

			/* Collisionlayer mask */
			collisionLayerMask = 0;
			for(int layer = 0; layer < 32; ++layer)
			{
				bool interactive = !Physics.GetIgnoreLayerCollision(gameObject.layer, layer);
				collisionLayerMask |= (interactive ? ~0 : 0) & (1 << layer);
			}
		}
		#endregion

		#region Life cycle
		protected override void FixedUpdate()
		{
			base.FixedUpdate();

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
					CreateCapsule();
				return capsule;
			}
		}
		void CreateCapsule()
		{
			if(!TryGetComponent(out capsule))
				capsule = gameObject.AddComponent<CapsuleCollider>();
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

		public override Vector3 Up => Body.up;
		#endregion

		#region Physics
		protected int collisionLayerMask = 0;

#if DEBUG
		new
#endif
		private Rigidbody rigidbody;
		private Rigidbody Rigidbody
		{
			get
			{
				if(rigidbody == null)
					CreateRigidbody();
				return rigidbody;
			}
		}
		private void CreateRigidbody()
		{
			if(!TryGetComponent(out rigidbody))
				rigidbody = gameObject.AddComponent<Rigidbody>();
		}

		public override float Mass
		{
			get => Rigidbody.mass;
			set => Rigidbody.mass = value;
		}
		#endregion
	}
}
