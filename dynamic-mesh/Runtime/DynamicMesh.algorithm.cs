using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public partial class DynamicMesh
	{
		public static UnityDcel ConstructSubmeshDataFromMesh(Mesh sourceMesh, int submeshIndex)
		{
			var data = new UnityDcel();

			// Add vertices
			var vertexPositions = new List<Vector3>();
			sourceMesh.GetVertices(vertexPositions);
			var vertexNormals = new List<Vector3>();
			sourceMesh.GetNormals(vertexNormals);
			for(int i = 0; i < sourceMesh.vertexCount; ++i)
			{
				UnityDcel.Vertex vertex = data.AddVertex();
				vertex.position = vertexPositions[i];
				vertex.normal = vertexNormals[i];
			}

			// Add surfaces
			int[] surfaceIndices = sourceMesh.GetTriangles(submeshIndex);
			for(int i = 0; i < surfaceIndices.Length; i += 3)
			{
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

			return data;
		}

		private static MinMaxRange<Vector3> CalculateVertexRange(IEnumerable<UnityDcel> submeshData)
		{
			var range = new MinMaxRange<Vector3>(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
			if(submeshData == null)
				return range;
			foreach(var submesh in submeshData)
			{
				foreach(var vertex in submesh.vertices)
				{
					range.min = Vector3.Min(range.min, vertex.position);
					range.max = Vector3.Max(range.max, vertex.position);
				}
			}
			return range;
		}

		private static bool IsFacingCamera(Vector3 position, Vector3 direction)
			=> Vector3.Dot(Scene.EditorSceneCamera.transform.position - position, direction) > 0;
		private static bool IsFacingCameraLocal(Transform transform, Vector3 position, Vector3 direction)
			=> IsFacingCamera(transform.localToWorldMatrix.MultiplyPoint(position), transform.localToWorldMatrix * direction);
	}
}
