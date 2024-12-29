using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public partial class DynamicMesh : ScriptableObject
	{
		#region Internal fields
		private Mesh importedMesh;
		private struct MinMaxRange<T>
		{
			public T min, max;

			public MinMaxRange(T min, T max)
			{
				this.min = min;
				this.max = max;
			}

			public MinMaxRange(MinMaxRange<T> range) : this(range.min, range.max) { }
		}
		private MinMaxRange<Vector3> range;
		#endregion

		#region Serialized fields
		public Mesh sourceMesh;
		[HideInInspector] public readonly List<UnityDcel> submeshData = new();

		[Serializable]
		public struct ImportOptions
		{
			[Tooltip("Auto re-import mesh data when source mesh changes")]
			public bool autoReimport;   // TODO: auto detect mesh change

			public bool limitVertexCount;
			[ShowWhen("limitVertexCount", true)]
			[Min(0)] public int maxVertexCount;

			public bool useGridOptimization;
			[ShowWhen("useGridOptimization", true)]
			public bool controlGridSize;
			[ShowWhen("useGridOptimization", true)]
			[ShowWhen("controlGridSize", true)]
			[Tooltip("Desired vertex count per grid")]
			[Min(2)] public int desiredGridSize;
		}
		public ImportOptions importOptions;
		#endregion

		#region Public interfaces
		public Vector3 Size => new(
			Mathf.Abs(range.max[0] - range.min[0]),
			Mathf.Abs(range.max[1] - range.min[1]),
			Mathf.Abs(range.max[2] - range.min[2])
		);

		public int VertexCount
		{
			get
			{
				int count = 0;
				foreach(var submesh in submeshData)
					count += submesh.VertexCount;
				return count;
			}
		}
		public IEnumerable<UnityDcel.Vertex> Vertices
		{
			get
			{
				foreach(var submesh in submeshData)
				{
					foreach(var vertex in submesh.vertices)
						yield return vertex;
				}
			}
		}

		public int SurfaceCount
		{
			get
			{
				int count = 0;
				foreach(var submesh in submeshData)
					count += submesh.SurfaceCount;
				return count;
			}
		}

		public Mesh ImportedMesh
		{
			get
			{
				if(submeshData == null)
					return importedMesh = null;
				if(importedMesh == null)
				{
					importedMesh = new Mesh();
					for(int i = 0; i < submeshData.Count; ++i)
						submeshData[i].WriteToMesh(importedMesh, i);
				}
				return importedMesh;
			}
		}

		public void Reset()
		{
			submeshData.Clear();
			importedMesh = null;
			range = new MinMaxRange<Vector3>(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
		}

		public void ReimportMeshData()
		{
			Reset();
			if(sourceMesh == null || !sourceMesh.isReadable)
				return;

			for(int i = 0; i < sourceMesh.subMeshCount; ++i)
				submeshData.Add(ConstructSubmeshDataFromMesh(sourceMesh, i));
			range = CalculateVertexRange(submeshData);
		}
		#endregion
	}
}