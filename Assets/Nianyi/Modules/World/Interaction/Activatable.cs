using UnityEngine;

namespace Nianyi {
	public class Activatable : MonoBehaviour {
		[SerializeField] ComposedCallback onActivate;
		[SerializeField] ComposedCallback onDeactivate;

		public void Activate() {
			if(!isActiveAndEnabled)
				return;
			onActivate.InvokeSync();
		}
		public void Deactivate() {
			if(!isActiveAndEnabled)
				return;
			onDeactivate.InvokeSync();
		}
	}
}
