using UnityEngine;

namespace Nianyi {
	[ExecuteAlways]
	public class Interaction : BehaviourBase {
		#region Message handlers
		protected void OnSelect() {
			Debug.Log($"Selected {this}", this);
		}

		protected void OnDeselect() {
			Debug.Log($"Deselected {this}", this);
		}
		#endregion
	}
}