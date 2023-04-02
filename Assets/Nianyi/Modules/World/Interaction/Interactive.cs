using UnityEngine;

namespace Nianyi {
	[RequireComponent(typeof(Collider))]
	public class Interactive : MonoBehaviour {
		#region Inspector fields
		[SerializeField] ComposedCallback onFocus;
		[SerializeField] ComposedCallback onBlur;
		[SerializeField] ComposedCallback onInteract;
		#endregion

		#region Core fields
		bool focused = false;
		#endregion

		#region Public interfaces
		public bool Focused {
			get => isActiveAndEnabled && focused;
			set {
				if(!isActiveAndEnabled)
					value = false;
				if(focused == value)
					return;
				if(focused = value)
					onFocus.InvokeSync();
				else
					onBlur.InvokeSync();
			}
		}

		public void OnFocus() => onInteract.InvokeSync();
		public void OnBlur() => onInteract.InvokeSync();
		public void OnInteract() => onInteract.InvokeSync();
		#endregion

		#region Life cycle
		protected void OnDisable() {
			Focused = false;
		}
		#endregion
	}
}