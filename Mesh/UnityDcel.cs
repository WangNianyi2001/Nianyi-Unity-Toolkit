using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	using Dcel = Dcel<UnityDcel.HalfEdge, UnityDcel.Vertex, UnityDcel.Surface>;

	[Serializable]
	public partial class UnityDcel : Dcel {
		public new class HalfEdge : Dcel.HalfEdge { }

		public new class Vertex : Dcel.Vertex {
			public Vector3 position;
			public Vector3 normal;
		}

		public new class Surface : Dcel.Surface {
			public Vector3 center;
			public Vector3 normal;
		}

		public struct SurfacePoint {
			public readonly static SurfacePoint Invalid = new() {
				surface = null,
				halfEdge = null,
				vertex = null,
				point = Vector3.zero,
			};

			public Surface surface;
			public HalfEdge halfEdge;
			public Vertex vertex;
			public Vector3 point;

			public readonly bool IsInvalid => surface == null && halfEdge == null && vertex == null;
		}

		#region Internal functions
		private bool IsInsideTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c, out Vector3 land) {
			Vector3 i = b - a, j = c - a, k = Vector3.Cross(i, j);
			k.Normalize();
			Matrix4x4 toTriangle = new(i, j, k, new Vector4(0, 0, 0, 1));
			Vector3 t = toTriangle.inverse * (point - a);
			t.z = 0;
			land = (Vector3)(toTriangle * t) + a;
			return t.x >= 0 && t.y >= 0 && t.x + t.y <= 1;
		}

		private Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 a, Vector3 b) {
			Vector3 i = b - a, d = point - a;
			float x = Vector3.Dot(d, i.normalized) / Vector3.Distance(b, a);
			x = Mathf.Clamp01(x);
			return a + i * x;
		}

		private IEnumerable<SurfacePoint> ClosestPointCandidates(Vector3 point, Matrix4x4 under, Surface surface) {
			var vertices = surface.Vertices.ToArray();
			if(vertices.Length != 3)
				yield break;
			Vector3
				a = under * vertices[0].position,
				b = under * vertices[1].position,
				c = under * vertices[2].position;

			if(IsInsideTriangle(point, a, b, c, out Vector3 land)) {
				yield return new SurfacePoint {
					surface = surface,
					point = land,
				};
				yield break;
			}
			yield return new SurfacePoint {
				vertex = vertices[0],
				point = a
			};
			yield return new SurfacePoint {
				vertex = vertices[1],
				point = b
			};
			yield return new SurfacePoint {
				vertex = vertices[2],
				point = c
			};
			yield return new SurfacePoint {
				halfEdge = vertices[0].HalfEdgeOf(surface),
				point = ClosestPointOnLineSegment(point, a, b)
			};
			yield return new SurfacePoint {
				halfEdge = vertices[1].HalfEdgeOf(surface),
				point = ClosestPointOnLineSegment(point, b, c)
			};
			yield return new SurfacePoint {
				halfEdge = vertices[2].HalfEdgeOf(surface),
				point = ClosestPointOnLineSegment(point, c, a)
			};
		}
		#endregion

		#region Public interfaces
		public void ClosestPointOnSurface(Vector3 world, Matrix4x4 under, Surface surface, out SurfacePoint info) {
			info = SurfacePoint.Invalid;
			var candidates = ClosestPointCandidates(world, under, surface);
			float bestDistance = Mathf.Infinity;
			foreach(var candidate in candidates) {
				float distance = Vector3.Distance(candidate.point, world);
				if(distance >= bestDistance)
					continue;
				info = candidate;
				bestDistance = distance;
			}
		}

		public void WriteToMesh(UnityEngine.Mesh mesh, int submeshIndex, int vertexIndexOffset = -1) {
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
			foreach(var surface in surfaces) {
				foreach(var vertex in surface.Vertices)
					triangles.Add(vertices.IndexOf(vertex) + vertexIndexOffset);
			}
			mesh.SetTriangles(triangles, submeshIndex);
		}
		#endregion
	}
}