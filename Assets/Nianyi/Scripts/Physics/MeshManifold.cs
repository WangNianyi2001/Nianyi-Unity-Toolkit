using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi {
	[ExecuteAlways]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public partial class MeshManifold : BehaviourBase {
		#region Internal fields
		[NonSerialized] public Data.DcelUnity data;
		private MeshFilter filter;
		private new MeshRenderer renderer;
		#endregion

		#region Serialized fields
		[SerializeField] private Mesh mesh;
		[Serializable]
		public struct ImportOptions {
			public bool limitVertexCount;
			[ShowIfBool("limitVertexCount")][Min(0)] public int maxVertexCount;
			public bool weldVertices;
			[ShowIfBool("weldVertices")][Min(0)] public float maxWeldingDistance;
		}
		public ImportOptions importOptions;
		#endregion

		#region Internal functions
		protected bool IsMeshHandlable() {
			if(Mesh == null)
				return false;
			if(!Mesh.isReadable)
				return false;
			if(importOptions.limitVertexCount) {
				if(Mesh.vertexCount > importOptions.maxVertexCount)
					return false;
			}
			return true;
		}

		protected void RegenerateMeshData() {
			data = null;
			if(mesh == null || !mesh.isReadable)
				return;
			data = new Data.DcelUnity();
			data.transform = transform;

			// Add vertices
			var vertexPositions = new List<Vector3>();
			mesh.GetVertices(vertexPositions);
			var vertexNormals = new List<Vector3>();
			mesh.GetNormals(vertexNormals);
			for(int i = 0; i < mesh.vertexCount; ++i) {
				Data.DcelUnity.Vertex vertex = data.AddVertex();
				vertex.position = vertexPositions[i];
				vertex.normal = vertexNormals[i];
			}

			// Add surfaces
			for(int submeshI = 0; submeshI < mesh.subMeshCount; ++submeshI) {
				int[] surfaceIndices = mesh.GetTriangles(submeshI);
				for(int i = 0; i < surfaceIndices.Length; i += 3) {
					var a = data.vertices[surfaceIndices[i + 0]];
					var b = data.vertices[surfaceIndices[i + 1]];
					var c = data.vertices[surfaceIndices[i + 2]];
					Data.DcelUnity.Surface surface = data.CreateSurface(a, b, c);
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

			// Post-import process
			if(importOptions.weldVertices)
				data.WeldCloseVertices(importOptions.maxWeldingDistance);
		}
		#endregion

		#region Public interfaces
		public Mesh Mesh {
			get => mesh;
			set {
				mesh = value;
				if(filter) {
					filter.mesh = value;
				}
			}
		}

		public Material Material {
			get => renderer?.sharedMaterial;
			set {
				if(renderer) {
					renderer.sharedMaterial = value;
				}
			}
		}
		#endregion

		#region Life cycle
		#endregion
	}
}