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
		#endregion

		#region Physics
		public abstract Rigidbody ProxyRigidbody { get; }
		#endregion
	}
}
