using UnityEngine;

namespace Nianyi.UnityToolkit
{
	[CreateAssetMenu(menuName = "Nianyi/Character/Character Controller Profile")]
	public class CharacterControllerProfile : ScriptableObject
	{
		#region Event hooks
		public System.Action OnChanged;

#if UNITY_EDITOR
		protected void OnValidate()
		{
			OnChanged?.Invoke();
		}
#endif
		#endregion

		#region Physics
		[Header("Physics")]

		[Min(0)] public float mass;

		public PhysicMaterial physicMaterial;
		#endregion

		#region Geometry
		[Header("Geometry")]

		[Min(0)] public float radius;

		[Min(0)] public float height;
		#endregion

		#region Motion
		[Header("Motion")]

		#region Movement
		public Movement movement;

		[System.Serializable]
		public struct Movement
		{
			[Min(0)] public float maxVelocity;
			public bool limitAcceleration;

			[ShowWhen("limitAcceleration", true)]
			[Min(0)] public float maxForce;

			[Range(0, 90)] public float maxSlope;
		}
		#endregion

		#region Orientation
		public Orientation orientation;

		[System.Serializable]
		public struct Orientation
		{
			/// <remarks>In degrees per second.</remarks>
			[Tooltip("In degrees per second.")]
			[Min(0)] public float maxAngularVelocity;

			public bool couldPhysicsAffectsOrientation;
		}
		#endregion

		#region Jumping
		public bool useJumping = true;

		[ShowWhen("useJumping", true)]
		public Jumping jumping;

		[System.Serializable]
		public struct Jumping
		{

			[Min(0)] public float jumpHeight;

			public bool useMidAirAttenuation;

			[System.Serializable]
			public struct MidAirAttenuation
			{
				[Range(0f, 1f)] public float movement;
				[Range(0f, 1f)] public float orientation;
			}
			[ShowWhen("useMidAirAttenuation", true)]
			public MidAirAttenuation midAirAttenuation;

			[Min(0)] public int midAirAllowance;

			public bool useCoyoteTime;

			[ShowWhen("useCoyoteTime", true)]
			[Min(0)] public float coyoteTime;
		}
		#endregion
		#endregion
	}
}
