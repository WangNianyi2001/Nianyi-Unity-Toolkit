using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public abstract class BaseCharacterController : MonoBehaviour
	{
		#region Geometry
		/// <summary>How high the character is in shape.</summary>
		public abstract float Height { get; set; }
		/// <summary>How fat the character is in shape.</summary>
		public abstract float Radius { get; set; }
		#endregion

		#region Physics
		/// <summary>How heavy the character is.</summary>
		public abstract float Mass { get; set; }
		#endregion

		#region Motion
		/// <summary>An abstract value representing the "pivot position" of the character.</summary>
		public abstract Vector3 Position { get; }
		/// <summary>The actual moving velocity in the world space.</summary>
		public abstract Vector3 Velocity { get; }
		/// <summary>The velocity that will be processed in the next physics frame.</summary>
		public abstract Vector3 InputVelocity { get; set; }

		/// <summary>An abstract value representing the "body orientation" of the character.</summary>
		public abstract Quaternion Orientation { get; }
		/// <remarks>
		/// This could differ with the main orientation when head movement is not in sync with body movement,
		/// e.g. in a third-person control scenario.
		/// </remarks>
		public abstract Quaternion FacingDirection { get; }
		#endregion
	}
}
