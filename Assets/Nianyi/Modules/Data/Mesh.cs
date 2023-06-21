using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi.Data {
	[CreateAssetMenu(menuName = "Nianyi/Physics/Mesh Data")]
	public class Mesh : ScriptableObject {
		#region Serialized fields
		public UnityEngine.Mesh sourceMesh;
		// TODO: Custom serialization of UnityDcel
		[NonSerialized] [HideInInspector] public UnityDcel data;

		[Serializable]
		public struct ImportOptions {
			[Tooltip("Auto re-import mesh data when source mesh changes")]
			public bool autoReimport;

			public bool limitVertexCount;
			[ShowIfBool("limitVertexCount")][Min(0)] public int maxVertexCount;
		}
		public ImportOptions importOptions;
		#endregion

		#region Public interfaces
		public void ReimportMeshData() {
			if(sourceMesh == null || !sourceMesh.isReadable)
				return;
			data = new UnityDcel();

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
		}
		#endregion
	}
}