#if UNITY_EDITOR
using UnityEngine;

namespace Nianyi {
	public partial class InteractionSystem {
		protected void OnEditStart() {
			previouslySelectedInteractions = selectedInteractions;
		}

		protected void OnEditUpdate() {
			// Newly-created null references would be removed by OnGameUpdate,
			// they must be restored in order for developers to assign them.
			var selected = selectedInteractions;
			OnGameUpdate();
			selectedInteractions = selected;
		}
	}
}
#endif