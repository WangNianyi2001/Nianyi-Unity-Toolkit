#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Nianyi {
	public static partial class TransformUtility {
		public static Camera SceneCamera => SceneView.currentDrawingSceneView.camera;

		public static bool IsFacingCamera(Vector3 position, Vector3 direction)
			=> Vector3.Dot(SceneCamera.transform.position - position, direction) > 0;
		public static bool IsFacingCameraLocal(this Transform transform, Vector3 position, Vector3 direction)
			=> IsFacingCamera(transform.localToWorldMatrix.MultiplyPoint(position), transform.localToWorldMatrix * direction);
	}
}
#endif