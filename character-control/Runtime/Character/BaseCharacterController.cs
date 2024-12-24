using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public abstract partial class BaseCharacterController : MonoBehaviour
	{
		#region Profile
		[SerializeField] private CharacterControllerProfile profile;
		public virtual CharacterControllerProfile Profile
		{
			get
			{
				if(profile == null)
					profile = Resources.Load<CharacterControllerProfile>("Character/Fallback Character Controller Profile");
				return profile;
			}
			set
			{
				if(profile != null)
					DetachProfile();

				profile = value;
				profile = Profile;  // Make sure that `null` would be converted to the fallback profile.

				ApplyProfile();
				profile.OnChanged += ApplyProfile;
			}
		}

		protected abstract void ApplyProfile();

		protected virtual void DetachProfile()
		{
			if(profile == null)
				return;
			profile.OnChanged -= ApplyProfile;
		}
		#endregion

		#region Life cycle
		protected virtual void Start()
		{
			Profile = Profile;
			OnGrounded += JumpingOnGrounded;
		}

		protected virtual void OnDestroy()
		{
			DetachProfile();
		}

		protected virtual void FixedUpdate()
		{
			UpdateGrounding();
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			RecordGrounding(collision);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			RecordGrounding(collision);
		}

		protected virtual void OnCollisionExit(Collision collision)
		{
			RemoveGrounding(collision);
		}
		#endregion

		#region Geometry
		/// <summary>How high the character is in shape.</summary>
		public abstract float Height { get; set; }
		/// <summary>How fat the character is in shape.</summary>
		public abstract float Radius { get; set; }
		#endregion

		#region Anatomy
		public abstract Transform Body { get; }
		public abstract Transform Head { get; }
		public abstract Vector3 Up { get; }
		#endregion

		#region Physics
		/// <summary>How heavy the character is.</summary>
		public abstract float Mass { get; set; }
		#endregion
	}
}
