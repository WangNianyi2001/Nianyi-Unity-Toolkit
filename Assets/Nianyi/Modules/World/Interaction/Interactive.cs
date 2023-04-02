using UnityEngine;

namespace Nianyi {
	[RequireComponent(typeof(Collider))]
	public class Interactive : MonoBehaviour {
		#region Inspector fields
		[SerializeField] ComposedCallback onSelect;
		[SerializeField] ComposedCallback onDeselect;
		[SerializeField] ComposedCallback onUse;
		#endregion

		#region Core fields
		bool selected = false;
		#endregion

		#region Auxiliary
		public void SetSelectingState(bool value) {
			if(selected == value)
				return;
			if(selected = value)
				onSelect.InvokeSync();
			else
				onDeselect.InvokeSync();
		}
		#endregion

		#region Public interfaces
		public bool Selected {
			get => isActiveAndEnabled && selected;
			set {
				if(!isActiveAndEnabled)
					return;
				SetSelectingState(value);
			}
		}

		public void Use() {
			if(!isActiveAndEnabled)
				return;
			onUse.InvokeSync();
		}
		#endregion

		#region Life cycle
		protected void OnDisable() {
			SetSelectingState(false);
		}
		#endregion
	}
}