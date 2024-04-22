#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace Nianyi {
	public partial class Grid3d<Point> where Point : class {
		public void DrawGizmos(in Matrix4x4 under) {
			foreach(var grid in Grids)
				grid.DrawGizmos(under);
		}
	}

	public partial class Grid3dTile<Point> : IEnumerable<Point> where Point : class {
		public void DrawGizmos(in Matrix4x4 under) {
			Vector4
				o = Origin,
				i = o + Right,
				j = o + Up,
				k = o + Forward,
				ij = o + Right + Up,
				kj = o + Up + Forward,
				ki = o + Forward + Right,
				kij = o + Right + Up + Forward;

			var oldMatrix = Gizmos.matrix;
			Gizmos.matrix *= under;

			Gizmos.DrawLine(o, i);
			Gizmos.DrawLine(i, ij);
			Gizmos.DrawLine(ij, j);
			Gizmos.DrawLine(j, o);

			Gizmos.DrawLine(k, ki);
			Gizmos.DrawLine(ki, kij);
			Gizmos.DrawLine(kij, kj);
			Gizmos.DrawLine(kj, k);

			Gizmos.DrawLine(o, k);
			Gizmos.DrawLine(i, ki);
			Gizmos.DrawLine(j, kj);
			Gizmos.DrawLine(ij, kij);

			Gizmos.matrix = oldMatrix;
		}
	}
}
#endif