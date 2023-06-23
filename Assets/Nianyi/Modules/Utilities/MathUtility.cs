using UnityEngine;

namespace Nianyi {
	public static class MathUtility {
		public static Vector3 Reciprocal(this Vector3 v) {
			return new Vector3(
				1 / v[0],
				1 / v[1],
				1 / v[2]
			);
		}

		public static Vector3 AsVector3(this Vector3Int v) {
			return new Vector3(v[0], v[1], v[2]);
		}
	}
}
