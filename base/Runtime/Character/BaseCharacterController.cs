using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public abstract class BaseCharacterController : MonoBehaviour
	{
		public abstract CharacterControllerProfile Profile { get; }

		#region Geometry
		/// <summary>How high the character is in shape.</summary>
		public abstract float Height { get; set; }
		/// <summary>How fat the character is in shape.</summary>
		public abstract float Radius { get; set; }
		#endregion

		#region Anatomy
		public abstract Transform Body { get; }
		public abstract Transform Head { get; }
		#endregion

		#region Physics
		/// <summary>How heavy the character is.</summary>
		public abstract float Mass { get; set; }
		#endregion

		#region Motion
		#region Movement
		/// <summary>An abstract value representing the "pivot position" of the character.</summary>
		public abstract Vector3 Position { get; set; }
		/// <summary>The actual moving velocity in the world space.</summary>
		public abstract Vector3 Velocity { get; }
		/// <summary>The velocity that will be processed in the next physics frame.</summary>
		public abstract Vector3 InputVelocity { get; set; }
		#endregion

		#region Orientation
		/// <summary>An abstract value representing the orientation of the entire character.</summary>
		public abstract Quaternion BodyOrientation { get; set; }

		public abstract Quaternion HeadOrientation { get; set; }

		public abstract Vector3 AngularVelocity { get; }

		public abstract Vector3 InputAngularVelocity { get; set; }
		#endregion
		#endregion
	}
}
