using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi.UnityToolkit
{
	using Dcel = Dcel<UnityDcel.HalfEdge, UnityDcel.Vertex, UnityDcel.Surface>;

	[System.Serializable]
	public class UnityDcel : Dcel
	{
		public new class HalfEdge : Dcel.HalfEdge { }

		public new class Vertex : Dcel.Vertex
		{
			public Vector3 position;
		}

		public new class Surface : Dcel.Surface
		{
		}

		#region Public interfaces
		public void WriteToMesh(Mesh mesh, int submeshIndex, int vertexIndexOffset = -1)
		{
			if(vertexIndexOffset < 0)
				vertexIndexOffset = mesh.vertexCount;

			List<Vector3> vertexList = new();
			mesh.GetVertices(vertexList);
			List<Vector3> newVertexList = vertices.Select(v => v.position).ToList();

			int i;
			// Replace vertices until reach the end of the original vertex list.
			for(i = 0; i + vertexIndexOffset < vertexList.Count() && i < vertexList.Count(); ++i)
				vertexList[i + vertexIndexOffset] = newVertexList[i];
			newVertexList.RemoveRange(0, i);
			// Add rest vertices.
			vertexList.AddRange(newVertexList);

			mesh.SetVertices(vertexList);
			var triangles = new List<int>();
			foreach(var surface in surfaces)
			{
				foreach(var vertex in surface.Vertices)
					triangles.Add(vertices.IndexOf(vertex) + vertexIndexOffset);
			}
			mesh.SetTriangles(triangles, submeshIndex);
		}
		#endregion
	}
}