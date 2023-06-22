using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.Data {
	public partial class Mesh {
		public static UnityDcel ConstructDataFromMesh(UnityEngine.Mesh sourceMesh) {
			var data = new UnityDcel();

			// Add vertices
			var vertexPositions = new List<Vector3>();
			sourceMesh.GetVertices(vertexPositions);
			var vertexNormals = new List<Vector3>();
			sourceMesh.GetNormals(vertexNormals);
			for(int i = 0; i < sourceMesh.vertexCount; ++i) {
				UnityDcel.Vertex vertex = data.AddVertex();
				vertex.position = vertexPositions[i];
				vertex.normal = vertexNormals[i];
			}

			// Add surfaces
			for(int submeshI = 0; submeshI < sourceMesh.subMeshCount; ++submeshI) {
				int[] surfaceIndices = sourceMesh.GetTriangles(submeshI);
				for(int i = 0; i < surfaceIndices.Length; i += 3) {
					var a = data.vertices[surfaceIndices[i + 0]];
					var b = data.vertices[surfaceIndices[i + 1]];
					var c = data.vertices[surfaceIndices[i + 2]];
					UnityDcel.Surface surface = data.CreateSurface(a, b, c);
					surface.normal = Vector3.Cross(
						a.position - b.position,
						b.position - c.position
					).normalized;
					surface.center = (
						a.position +
						b.position +
						c.position
					) / 3;
				}
			}

			return data;
		}

		public static MinMaxRange<Vector3> CalculateVertexRange(UnityDcel data) {
			var range = new MinMaxRange<Vector3>(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
			if(data == null)
				return range;
			foreach(var vertex in data.vertices) {
				range.min = Vector3.Min(range.min, vertex.position);
				range.max = Vector3.Max(range.max, vertex.position);
			}
			return range;
		}

		public static int CalculateReasonableGridSize(Mesh mesh) {
			Vector3 size = mesh.Size;
			int vertexCount = mesh.data.vertices.Count;
			// TODO
			return 2;
		}

		public static Grid3d<UnityDcel.Vertex> GenerateVertexGrid(Mesh mesh, int gridSize) {
			var size = mesh.Size;
			float volume = size.x * size.y * size.z;
			int vertexCount = mesh.data.vertices.Count;
			float desiredGridVolume = volume / vertexCount * gridSize;
			float desiredGridSideLength = Mathf.Pow(desiredGridVolume, 1 / 3);
			var gridDimensions = new Vector3Int(
				Mathf.FloorToInt(size[0] / desiredGridSideLength),
				Mathf.FloorToInt(size[1] / desiredGridSideLength),
				Mathf.FloorToInt(size[2] / desiredGridSideLength)
			);
			gridDimensions += Vector3Int.one;

			var grid = new Grid3d<UnityDcel.Vertex>(gridDimensions);

			var sizeReciprocal = size.Reciprocal();
			foreach(var vertex in mesh.data.vertices) {
				var gridCoordinate = Vector3.Scale(vertex.position - mesh.range.min, sizeReciprocal) + mesh.range.min;
				grid.AddPoint(vertex, gridCoordinate);
			}

			return grid;
		}
	}
}
