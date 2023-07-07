using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi.Data {
	[CreateAssetMenu(menuName = "Nianyi/Physics/Mesh Data")]
	public partial class Mesh : ScriptableObject {
		#region Internal fields
		private UnityEngine.Mesh importedMesh;
		private MinMaxRange<Vector3> range;
		private Grid3d<UnityDcel.Vertex> vertexGrid;
		private Matrix4x4 gridToLocalMatrix;
		#endregion

		#region Serialized fields
		public UnityEngine.Mesh sourceMesh;
		[HideInInspector] public List<UnityDcel> submeshData = new List<UnityDcel>();

		[Serializable]
		public struct ImportOptions {
			[Tooltip("Auto re-import mesh data when source mesh changes")]
			public bool autoReimport;   // TODO: auto detect mesh change

			public bool limitVertexCount;
			[ShowIfBool("limitVertexCount")]
			[Min(0)] public int maxVertexCount;

			public bool useGridOptimization;
			[ShowIfBool("useGridOptimization")]
			public bool controlGridSize;
			[ShowIfBool("useGridOptimization")]
			[ShowIfBool("controlGridSize")]
			[Tooltip("Desired vertex count per grid")]
			[Min(2)] public int desiredGridSize;
		}
		public ImportOptions importOptions;
		#endregion

		#region Public interfaces
		public Vector3 Size => new Vector3(
			Mathf.Abs(range.max[0] - range.min[0]),
			Mathf.Abs(range.max[1] - range.min[1]),
			Mathf.Abs(range.max[2] - range.min[2])
		);

		public int VertexCount {
			get {
				int count = 0;
				foreach(var submesh in submeshData)
					count += submesh.vertices.Count;
				return count;
			}
		}
		public IEnumerable<UnityDcel.Vertex> Vertices {
			get {
				foreach(var submesh in submeshData) {
					foreach(var vertex in submesh.vertices)
						yield return vertex;
				}
			}
		}

		public int SurfaceCount {
			get {
				int count = 0;
				foreach(var submesh in submeshData)
					count += submesh.surfaces.Count;
				return count;
			}
		}

		public UnityEngine.Mesh ImportedMesh {
			get {
				if(submeshData == null)
					return importedMesh = null;
				if(importedMesh == null) {
					importedMesh = new UnityEngine.Mesh();
					for(int i = 0; i < submeshData.Count; ++i)
						submeshData[i].WriteToMesh(importedMesh, i);
				}
				return importedMesh;
			}
		}

		public Grid3d<UnityDcel.Vertex> VertexGrid {
			get {
				if(!importOptions.useGridOptimization)
					return null;
				if(vertexGrid == null)
					vertexGrid = GenerateVertexGrid(this);
				return vertexGrid;
			}
		}

		public void Reset() {
			submeshData.Clear();
			importedMesh = null;
			range = new MinMaxRange<Vector3>(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
			vertexGrid = null;
		}

		public void ReimportMeshData() {
			Reset();
			if(sourceMesh == null || !sourceMesh.isReadable)
				return;

			for(int i = 0; i < sourceMesh.subMeshCount; ++i)
				submeshData.Add(ConstructSubmeshDataFromMesh(sourceMesh, i));
			range = CalculateVertexRange(submeshData);
			vertexGrid = GenerateVertexGrid(this);
		}
		#endregion
	}
}