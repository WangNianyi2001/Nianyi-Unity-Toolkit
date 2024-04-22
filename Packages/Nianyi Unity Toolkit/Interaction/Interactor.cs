using UnityEngine;

namespace Nianyi {
	/// <summary>
	/// Component that controls an interaction system.
	/// It tells the system what should be selected and when to interact.
	/// </summary>
	public abstract class Interactor : MonoBehaviour {
		public InteractionSystem system;

		public void Interact() {
			if(system != null)
				system.SendMessage("OnInteract");
		}
	}
}