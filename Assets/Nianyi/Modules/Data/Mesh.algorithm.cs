﻿using UnityEngine;
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

		public static Grid3d<UnityDcel.Vertex> GenerateVertexGrid(Mesh mesh) {
			int gridSize = mesh.importOptions.controlGridSize
				? mesh.importOptions.desiredGridSize
				: CalculateReasonableGridSize(mesh);
			return GenerateVertexGrid(mesh, gridSize);
		}

		public static Grid3d<UnityDcel.Vertex> GenerateVertexGrid(Mesh mesh, int gridSize) {
			var size = mesh.Size;
			float volume = size.x * size.y * size.z;
			if(volume <= 0)
				return null;
			int vertexCount = mesh.data.vertices.Count;
			float desiredGridVolume = volume / vertexCount * gridSize;
			float desiredGridSideLength = Mathf.Pow(desiredGridVolume, 1f / 3);
			var gridDimensions = new Vector3Int(
				Mathf.FloorToInt(size[0] / desiredGridSideLength),
				Mathf.FloorToInt(size[1] / desiredGridSideLength),
				Mathf.FloorToInt(size[2] / desiredGridSideLength)
			);
			gridDimensions += Vector3Int.one;

			var grid = new Grid3d<UnityDcel.Vertex>(gridDimensions);

			var sizeReciprocal = size.Reciprocal();
			foreach(var vertex in mesh.data.vertices) {
				var gridCoordinate = Vector3.Scale(vertex.position - mesh.range.min, sizeReciprocal);
				grid.AddPoint(vertex, gridCoordinate);
			}

			return grid;
		}

		public static IEnumerable<Vector3Int> GridIndices(Mesh mesh) {
			if(mesh.VertexGrid == null)
				yield break;
			var dimensions = mesh.vertexGrid.dimensions;
			for(int i = 0; i < dimensions[0]; ++i) {
				for(int j = 0; j < dimensions[1]; ++j) {
					for(int k = 0; k < dimensions[2]; ++k)
						yield return new Vector3Int(i, j, k);
				}
			}
		}

		public static IEnumerable<Vector3> GridVertices(Mesh mesh, Vector3Int gridIndex, Matrix4x4 under) {
			if(mesh.VertexGrid == null)
				yield break;
			Vector3 diagonal = mesh.range.max - mesh.range.min;
			Vector3
				i = Vector3.right * diagonal[0] / mesh.VertexGrid.dimensions[0],
				j = Vector3.up * diagonal[1] / mesh.VertexGrid.dimensions[1],
				k = Vector3.forward * diagonal[2] / mesh.VertexGrid.dimensions[2];
			Vector3 o = i * gridIndex[0] + j * gridIndex[1] + k * gridIndex[2];
			o += mesh.range.min;
			o = under.MultiplyPoint(o);
			i = under * i;
			j = under * j;
			k = under * k;

			yield return o;
			yield return o + i;
			yield return o + j;
			yield return o + k;
			o += i + j + k;
			yield return o - i;
			yield return o - j;
			yield return o - k;
			yield return o;
		}
	}
}
