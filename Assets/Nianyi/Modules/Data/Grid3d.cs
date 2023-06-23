using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Nianyi.Data {
	public partial class Grid3d<Point> where Point : class {
		#region Internal fields
		public readonly Vector3Int dimensions;
		public readonly Vector3 dimensionsReciprocal;
		protected readonly Grid3dTile<Point>[] tiles;
		#endregion

		#region Internal functions
		protected int LinearizeIndex(Vector3Int index) {
			int result = index[0];
			for(int i = 1; i < 3; ++i)
				result += index[i] * (dimensions[i - 1] - 1);
			return result;
		}
		#endregion

		#region Public interfaces
		public Grid3d(Vector3Int dimensions) {
			this.dimensions = dimensions;
			dimensionsReciprocal = dimensions.AsVector3().Reciprocal();

			tiles = new Grid3dTile<Point>[Volume];
			int i = 0;
			foreach(var index in Indices) {
				tiles[i] = new Grid3dTile<Point>(this, index);
				++i;
			}
		}

		public int Volume => dimensions[0] * dimensions[1] * dimensions[2];
		public IEnumerable<Vector3Int> Indices {
			get {
				for(int i = 0; i < dimensions[0]; ++i) {
					for(int j = 0; j < dimensions[1]; ++j) {
						for(int k = 0; k < dimensions[2]; ++k)
							yield return new Vector3Int(i, j, k);
					}
				}
			}
		}

		public IEnumerable<Grid3dTile<Point>> Grids {
			get {
				foreach(var grid in tiles)
					yield return grid;
			}
		}
		public IEnumerable<Point> Points {
			get {
				foreach(var grid in tiles) {
					foreach(var point in grid)
						yield return point;
				}
			}
		}

		public Grid3dTile<Point> TileAt(Vector3Int index) {
			index.Clamp(Vector3Int.zero, dimensions - Vector3Int.one);
			return tiles[LinearizeIndex(index)];
		}
		public Grid3dTile<Point> TileAt(Vector3 index) {
			Vector3Int i = new Vector3Int(
				Mathf.FloorToInt(index[0]),
				Mathf.FloorToInt(index[1]),
				Mathf.FloorToInt(index[2])
			);
			return TileAt(i);
		}

		public void Add(Point point, Vector3 index) => TileAt(index).Add(point);
		#endregion
	}

	public partial class Grid3dTile<Point> : IEnumerable<Point> where Point : class {
		private readonly Grid3d<Point> grid;
		private readonly Vector3Int index;
		public readonly Matrix4x4 localToGridMatrix;

		public readonly List<Point> points = new List<Point>();

		public static Matrix4x4 CalculateLocalToGridMatrix(in Grid3d<Point> grid, in Vector3Int index) {
			return Matrix4x4.TRS(
				Vector3.Scale(grid.dimensionsReciprocal, index),
				Quaternion.identity,
				grid.dimensionsReciprocal
			);
		}

		public Grid3dTile(Grid3d<Point> grid, Vector3Int index) {
			this.grid = grid;
			this.index = index;
			localToGridMatrix = CalculateLocalToGridMatrix(this.grid, this.index);
		}

		public IEnumerator<Point> GetEnumerator() => points.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(Point point) {
			if(!points.Contains(point))
				points.Add(point);
		}

		public Vector4 Origin => localToGridMatrix * new Vector4(0, 0, 0, 1);
		public Vector4 Right => localToGridMatrix * Vector3.right;
		public Vector4 Up => localToGridMatrix * Vector3.up;
		public Vector4 Forward => localToGridMatrix * Vector3.forward;
	}
}
