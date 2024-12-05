using UnityEngine;

namespace Nianyi.UnityToolkit
{
	[CreateAssetMenu(menuName = "Nianyi/Character/Character Controller Profile")]
	public class CharacterControllerProfile : ScriptableObject
	{
		#region Physics
		[Min(0)] public float mass;
		#endregion

		#region Geometry
		[Min(0)] public float radius;
		[Min(0)] public float height;
		#endregion

		#region Motion
		[Min(0)] public float maxVelocity;
		public bool limitAcceleration;
		[ShowWhen("limitAcceleration", true)]
		[Min(0)] public float maxForce;
		#endregion
	}
}
