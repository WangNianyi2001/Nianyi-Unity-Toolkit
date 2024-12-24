using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static Nianyi.UnityToolkit.CharacterControllerProfile;

namespace Nianyi.UnityToolkit
{
	public abstract partial class BaseCharacterController
	{
		#region Grounding
		protected class GroundingInfo
		{
			public Collider collider;
			public ContactPoint[] contacts;
		}
		protected Dictionary<Collider, GroundingInfo> groundings = new();

		static ContactPoint[] groundingContacts = new ContactPoint[1];
		protected virtual void AddGrounding(Collision collision)
		{
			if(groundingContacts.Length < collision.contactCount)
				groundingContacts = new ContactPoint[collision.contactCount];
			collision.GetContacts(groundingContacts);

			IEnumerable<ContactPoint> validContacts =
				from contact in groundingContacts
				where Vector3.Angle(contact.normal, Up) <= Profile.movement.maxSlope
				select contact;

			GroundingInfo grounding = new()
			{
				collider = collision.collider,
				contacts = validContacts.ToArray(),
			};
			if(grounding.contacts.Length == 0)
				return;
			groundings[collision.collider] = grounding;

			if(IsGrounded)
			{
				isJumping = false;
				midAirJumpingAllowance = Profile.jumping.midAirAllowance;
			}
		}

		protected virtual void RemoveGrounding(Collision collision)
		{
			if(!groundings.ContainsKey(collision.collider))
				return;
			groundings.Remove(collision.collider);
		}

		protected virtual void UpdateGrounding()
		{
			CleanExpiredGroundings();
			if(IsGrounded)
			{
				lastGroundedTime = Time.time;
			}
		}

		protected virtual void CleanExpiredGroundings()
		{
			foreach(var collider in groundings.Keys.ToList())
			{
				if(collider != null)
					continue;
				groundings.Remove(collider);
			}
		}

		public bool IsGrounded => groundings.Count > 0;

		float lastGroundedTime = float.NegativeInfinity;
		#endregion

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

		#region Jumping
		protected bool CanJump
		{
			get
			{
				if(!Profile.useJumping)
					return false;
				if(IsGrounded)
					return true;
				if(!isJumping)
				{
					float coyoteTime = Time.time - lastGroundedTime;
					if(coyoteTime <= Profile.jumping.coyoteTime)
						return true;
				}
				else
				{
					if(midAirJumpingAllowance > 0)
						return true;
				}
				return false;
			}
		}
		/// <summary>Whether the character is mid-air due to the most recent active jumping.</summary>
		protected bool isJumping = false;
		protected int midAirJumpingAllowance = 0;

		public virtual void Jump()
		{
			if(!CanJump)
				return;

			if(!isJumping)
				isJumping = true;
			else
				--midAirJumpingAllowance;

			PerformJumping();
		}

		protected abstract void PerformJumping();
		#endregion
	}
}