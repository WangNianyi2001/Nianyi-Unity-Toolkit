using UnityEngine;

namespace Nianyi.UnityToolkit
{
	[CreateAssetMenu(menuName = "Nianyi/Character/Character Controller Profile")]
	public class CharacterControllerProfile : ScriptableObject
	{
		#region Physics
		[Header("Physics")]
		[Min(0)] public float mass;
		#endregion

		#region Geometry
		[Header("Geometry")]
		[Min(0)] public float radius;
		[Min(0)] public float height;
		#endregion

		#region Motion
		#region Movement
		[Header("Movement")]
		[Min(0)] public float maxVelocity;
		public bool limitAcceleration;
		[ShowWhen("limitAcceleration", true)]
		[Min(0)] public float maxForce;
		#endregion

		#region Orientation
		[Header("Orientation")]
		/// <remarks>In degrees per second.</remarks>
		[Tooltip("In degrees per second.")]
		[Min(0)] public float maxAngularVelocity;
		#endregion
		#endregion
	}
}
