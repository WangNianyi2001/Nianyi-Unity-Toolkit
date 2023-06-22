using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi.Data {
	[CreateAssetMenu(menuName = "Nianyi/Physics/Mesh Data")]
	public partial class Mesh : ScriptableObject {
		#region Internal fields
		private UnityEngine.Mesh importedMesh;
		#endregion

		#region Serialized fields
		public UnityEngine.Mesh sourceMesh;
		[HideInInspector] public UnityDcel data;

		[Serializable]
		public struct ImportOptions {
			[Tooltip("Auto re-import mesh data when source mesh changes")]
			public bool autoReimport;	// TODO: auto detect mesh change

			public bool limitVertexCount;
			[ShowIfBool("limitVertexCount")]
			[Min(0)] public int maxVertexCount;

			public bool useGridOptimization;
			[ShowIfBool("useGridOptimization")]
			public bool controlGridSize;
			[ShowIfBool("useGridOptimization")]
			[ShowIfBool("controlGridSize")]
			[Min(2)] public int desiredVerticeCountPerGrid;
		}
		public ImportOptions importOptions;
		#endregion

		#region Public interfaces
		public UnityEngine.Mesh ImportedMesh {
			get {
				if(data == null)
					return importedMesh = null;
				if(importedMesh == null)
					importedMesh = data.MakeMesh();
				return importedMesh;
			}
		}
		#endregion
	}
}