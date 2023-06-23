using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi.Data {
	public class Grid3d<Point> where Point : class {
		#region Internal fields
		public readonly Vector3Int dimensions;
		protected readonly List<Point>[] grids;
		#endregion

		#region Internal functions
		protected int CoordinateToGridIndex(Vector3Int coordinates) {
			int index = coordinates[0];
			for(int i = 1; i < 3; ++i)
				index += coordinates[i] * (dimensions[i - 1] - 1);
			return index;
		}
		#endregion

		#region Public interfaces
		public Grid3d(Vector3Int dimensions) {
			this.dimensions = dimensions;

			grids = new List<Point>[GridCount];
			for(int i = 0; i < GridCount; ++i)
				grids[i] = new List<Point>();
		}

		public int GridCount => dimensions[0] * dimensions[1] * dimensions[2];

		public IEnumerable<List<Point>> Grids() {
			foreach(var grid in grids)
				yield return grid;
		}
		public IEnumerable<Point> Points() {
			foreach(var grid in grids) {
				foreach(var point in grid)
					yield return point;
			}
		}

		public List<Point> GridAt(Vector3Int coordinates) {
			coordinates.Clamp(Vector3Int.zero, dimensions - Vector3Int.one);
			int index = CoordinateToGridIndex(coordinates);
			return grids[index];
		}
		public List<Point> GridAt(Vector3 coordinates) {
			Vector3Int i = new Vector3Int(
				Mathf.FloorToInt(coordinates[0]),
				Mathf.FloorToInt(coordinates[1]),
				Mathf.FloorToInt(coordinates[2])
			);
			return GridAt(i);
		}

		public void AddPoint(Point point, Vector3 coordinates) {
			var grid = GridAt(coordinates);
			if(!grid.Contains(point))
				grid.Add(point);
		}
		#endregion
	}
}
