﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi.Data {
	[CreateAssetMenu(menuName = "Nianyi/Physics/Mesh Data")]
	public partial class Mesh : ScriptableObject {
		#region Internal fields
		private UnityEngine.Mesh importedMesh;
		private MinMaxRange<Vector3> range;
		private Grid3d<UnityDcel.Vertex> vertexGrid;
		#endregion

		#region Serialized fields
		public UnityEngine.Mesh sourceMesh;
		[HideInInspector] public UnityDcel data;

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

		public UnityEngine.Mesh ImportedMesh {
			get {
				if(data == null)
					return importedMesh = null;
				if(importedMesh == null)
					importedMesh = data.MakeMesh();
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
			data = null;
			importedMesh = null;
			range = new MinMaxRange<Vector3>(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
			vertexGrid = null;
		}

		public void ReimportMeshData() {
			Reset();
			if(sourceMesh == null || !sourceMesh.isReadable)
				return;

			data = ConstructDataFromMesh(sourceMesh);
			range = CalculateVertexRange(data);
			vertexGrid = GenerateVertexGrid(this);
		}
		#endregion
	}
}