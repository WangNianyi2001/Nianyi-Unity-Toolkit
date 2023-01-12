using UnityEngine;

namespace Nianyi {
	[RequireComponent(typeof(Camera))]
	public class CameraSelector : Selector {
		public enum AimPosition {
			CenterOfScreen,
			MousePosition,
		}

		#region Inspector fields
		[SerializeField] new Camera camera = null;
		public AimPosition aimPosition;
		public bool finiteSelectDistance = true;
		[ShowIfBool("finiteSelectDistance")] public float maxSelectDistance = 10;
		#endregion

		#region Core fields
		InputFilter input;
		#endregion

		#region Life cycle
		void Start() {
			camera = GetComponent<Camera>();
			input = GetComponent<InputFilter>();
		}

		void Update() {
			Vector3 point;
			switch(aimPosition) {
				case AimPosition.CenterOfScreen:
					point = new Vector3(Screen.width, Screen.height, 0) * .5f;
					break;
				case AimPosition.MousePosition:
					point = Input.mousePosition;
					break;
				default:
					point = Vector3.zero;
					break;
			}
			Ray ray = camera.ScreenPointToRay(point);
			float distance = finiteSelectDistance ? maxSelectDistance : Mathf.Infinity;
			Physics.Raycast(ray, out RaycastHit hit, distance);
			Target = hit.transform?.GetComponent<SelectTarget>();
		}
		#endregion
	}
}