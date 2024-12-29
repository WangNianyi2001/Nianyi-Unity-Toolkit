using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public abstract class CharacterShape : CharacterComponent
	{
		#region Anatomy
		public abstract Transform Body { get; }
		public abstract Transform Head { get; }

		public abstract Vector3 Up { get; }
		public abstract Vector3 Forward { get; }
		public abstract float Azimuth { get; set; }
		public abstract float Zenith { get; set; }
		#endregion

		#region Geometry
		public abstract Bounds BoundingBox { get; }

		public abstract bool SweepCast(Vector3 direction,
			out RaycastHit hitInfo,
			float maxDistance = Mathf.Infinity,
			Vector3 offset = default,
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore);
		#endregion

		#region Physics
		public abstract float Mass { get; set; }

		public abstract Vector3 Velocity { get; set; }
		public abstract void ApplyForce(Vector3 force);
		public abstract void ApplyImpulse(Vector3 impulse);

		public abstract Vector3 AngularVelocity { get; set; }
		#endregion

		#region Event hooks
		#region Collision
		public System.Action<Collision>
			onCollisionEnter,
			onCollisionStay,
			onCollisionExit;
		#endregion
		#endregion
	}
}
