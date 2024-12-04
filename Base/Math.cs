using UnityEngine;

namespace Nianyi {
	public static class Math {
		public static Vector3 Reciprocal(this Vector3 v) {
			return new(
				1 / v[0],
				1 / v[1],
				1 / v[2]
			);
		}

		public static Vector3 AsVector3(this Vector3Int v) {
			return new(v[0], v[1], v[2]);
		}

		public static bool IsFacingCamera(Vector3 position, Vector3 direction)
			=> Vector3.Dot(Hierarchy.EditorSceneCamera.transform.position - position, direction) > 0;
		public static bool IsFacingCameraLocal(this Transform transform, Vector3 position, Vector3 direction)
			=> IsFacingCamera(transform.localToWorldMatrix.MultiplyPoint(position), transform.localToWorldMatrix * direction);
	}
}
