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
			public Vector3 normal = Vector3.forward;
			public Vector2 uv;

			public Vertex() { }
			public Vertex(Vertex v)
			{
				position = v.position;
				normal = v.normal;
				uv = v.uv;
			}
		}

		public new class Surface : Dcel.Surface
		{
		}

		#region Public interfaces
		public void WriteToMesh(in Mesh mesh, int submeshIndex)
		{
			int originalVertexCount = mesh.vertexCount;
			int newCount = originalVertexCount + vertices.Count;

			List<Vector3> positions = new(newCount);
			mesh.GetVertices(positions);
			List<Vector3> normals = new(newCount);
			mesh.GetNormals(normals);
			List<Vector2> uvs = new(newCount);
			mesh.GetUVs(0, uvs);

			for(int i = 0; i < vertices.Count; ++i)
			{
				positions.Add(vertices[i].position);
				normals.Add(vertices[i].normal);
				uvs.Add(vertices[i].uv);
			}

			mesh.SetVertices(positions);
			mesh.SetNormals(normals);
			mesh.SetUVs(0, uvs);

			// Triangles.
			var triangles = new List<int>();
			foreach(var surface in surfaces)
			{
				foreach(var vertex in surface.Vertices)
					triangles.Add(vertices.IndexOf(vertex) + originalVertexCount);
			}
			mesh.SetTriangles(triangles, submeshIndex);
		}

		public void CalculateNormals()
		{
			foreach(Vertex v in vertices)
			{
				foreach(HalfEdge he in v.outGoingHalfEdges)
				{
					Vertex a = he.To, b = he.next.To;
					v.normal = Vector3.Cross(v.position - a.position, v.position - b.position).normalized;
				}
			}
		}

		public void SplitSurfaces()
		{
			List<Surface> oldSurfaces = surfaces, newSurfaces = new();
			surfaces = newSurfaces;
			halfEdges = new List<HalfEdge>();
			vertices = new List<Vertex>();

			foreach(var surface in oldSurfaces)
			{
				var copy = surface.Vertices.Select(v => new Vertex(v)).ToArray();
				vertices.AddRange(copy);
				CreateSurface(copy[0], copy[1], copy[2]);
			}
		}
		#endregion
	}
}