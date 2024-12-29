using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public partial class DynamicMesh
	{
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

		public readonly List<UnityDcel> submeshes = new();

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
				foreach(var submesh in submeshes)
					count += submesh.VertexCount;
				return count;
			}
		}
		public IEnumerable<UnityDcel.Vertex> Vertices
		{
			get
			{
				foreach(var submesh in submeshes)
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
				foreach(var submesh in submeshes)
					count += submesh.SurfaceCount;
				return count;
			}
		}

		public Mesh ToMesh(string name)
		{
			if(submeshes == null)
				return null;

			Mesh mesh = new() { name = name };
			for(int i = 0; i < submeshes.Count; ++i)
				submeshes[i].WriteToMesh(mesh, i);

			return mesh;
		}
	}
}