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
		// TODO: Make compatible for any `Up`.
		public override float Azimuth
		{
			get
			{
				return Body.rotation.eulerAngles.y;
			}
			set
			{
				var euler = Body.rotation.eulerAngles;
				euler.y = value;
				Body.rotation = Quaternion.Euler(euler);
			}
		}
		public override float Zenith
		{
			get
			{
				float value = Head.localRotation.eulerAngles.x;
				if(value > 180f)
					value -= 360f;
				return value;
			}
			set
			{
				var euler = Head.localRotation.eulerAngles;
				euler.x = value;
				Head.localRotation = Quaternion.Euler(euler);
			}
		}
		#endregion

		#region Geometry
		public override Bounds BoundingBox => Capsule.bounds;

		public override bool SweepCast(
			Vector3 direction, out RaycastHit hitInfo,
			float maxDistance = float.PositiveInfinity,
			Vector3 offset = default,
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
		)
		{
			Vector3 oldPos = Rigidbody.position;

			Rigidbody.position += offset;
			bool hasHit = Rigidbody.SweepTest(direction, out hitInfo, maxDistance, queryTriggerInteraction);

			Rigidbody.position = oldPos;

			return hasHit;
		}
		#endregion

		#region Physics
		public override float Mass
		{
			get => Rigidbody.mass;
			set => Rigidbody.mass = value;
		}

		public override Vector3 Velocity
		{
			get => Rigidbody.velocity;
			set => Rigidbody.velocity = value;
		}
		public override void ApplyForce(Vector3 force)
		{
			Rigidbody.AddForce(force, ForceMode.Force);
		}
		public override void ApplyImpulse(Vector3 impulse)
		{
			Rigidbody.AddForce(impulse, ForceMode.Impulse);
		}

		public override Vector3 AngularVelocity
		{
			get => Rigidbody.angularVelocity;
			set => Rigidbody.angularVelocity = value;
		}
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

		#region Geometry
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
			Shape.onCollisionEnter?.Invoke(collision);
		}

		protected void OnCollisionStay(Collision collision)
		{
			Shape.onCollisionStay?.Invoke(collision);
		}

		protected void OnCollisionExit(Collision collision)
		{
			Shape.onCollisionExit?.Invoke(collision);
		}
		#endregion
	}
}
