using UnityEngine;

namespace Nianyi {
	public class CameraInteractor : Interactor {
		#region Serialized fields
		public enum InteractionModeEnum {
			CenterOfScreen,
			CenterOfFrustum,
			MousePosition,
		}
		public InteractionModeEnum interactionMode;
		#endregion

		#region Internal functions
		#endregion

		#region Life cycle
		protected void OnGameUpdate() {
			//
		}
		#endregion
	}
}