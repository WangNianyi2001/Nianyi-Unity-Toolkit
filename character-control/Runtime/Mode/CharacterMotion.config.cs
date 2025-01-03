using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public partial class CharacterMotion
	{
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
			[Range(0f, 1f)] public float ungroundedAttenuation;

			public bool useAutoStepping;

			[ShowWhen("useAutoStepping", true)]
			public AutoStepping autoStepping;

			[System.Serializable]
			public struct AutoStepping
			{
				[Min(0)] public float height;
				[Min(0)] public float detectionRange;
			}
		}
		#endregion

		#region Orientation
		public Orientation orientation;

		[System.Serializable]
		public struct Orientation
		{
			public Vector2 zenithRange;
			[Min(0)] public float headAzimuthTolerance;

			public bool limitAngularVelocity;
			/// <remarks>In degrees per second.</remarks>
			[ShowWhen("limitAngularVelocity", true)]
			[Tooltip("In degrees per second.")]
			[Min(0)] public float maxAngularVelocity;

			public bool couldBeAffectedByPhysics;
		}
		#endregion

		#region Jumping
		public bool useJumping = true;

		[ShowWhen("useJumping", true)]
		public Jumping jumping;

		[System.Serializable]
		public struct Jumping
		{

			[Min(0)] public float height;

			[Min(0)] public int midAirAllowance;

			public bool useBuffer;

			[ShowWhen("useBuffer", true)]
			[Min(0)] public float bufferTime;

			public bool useCoyoteTime;

			[ShowWhen("useCoyoteTime", true)]
			[Min(0)] public float coyoteTime;
		}
		#endregion
	}
}
