using UnityEngine;

namespace Nianyi.UnityToolkit
{
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class CharacterCapsule : CharacterShape
	{
		#region Anatomy
		[SerializeField] private Transform head;
		public override Transform Head => head;
		public override Transform Body => transform;

		public override Vector3 Up => Body.up;
		public override Vector3 Forward => Body.forward;
		#endregion

		#region Physics
		public override Rigidbody ProxyRigidbody => Rigidbody;
		#endregion

		#region Components
		private CapsuleCollider capsule;
		public CapsuleCollider Capsule
		{
			get
			{
				if(capsule == null)
					capsule = GetComponent<CapsuleCollider>();
				return capsule;
			}
		}

#if UNITY_EDITOR
		new
#endif
		private Rigidbody rigidbody;
		public Rigidbody Rigidbody
		{
			get
			{
				if(rigidbody == null)
					rigidbody = GetComponent<Rigidbody>();
				return rigidbody;
			}
		}
		#endregion

		#region Property
		public float Height
		{
			get => Capsule.height;
			set
			{
				Capsule.height = value;
				Capsule.center = Vector3.up * (value * .5f);
			}
		}

		public float Radius
		{
			get => Capsule.radius;
			set => Capsule.radius = value;
		}
		#endregion

		#region Life cycle
		protected void OnCollisionEnter(Collision collision)
		{
			Character.onCollisionEnter?.Invoke(collision);
		}

		protected void OnCollisionStay(Collision collision)
		{
			Character.onCollisionStay?.Invoke(collision);
		}

		protected void OnCollisionExit(Collision collision)
		{
			Character.onCollisionExit?.Invoke(collision);
		}
		#endregion
	}
}
