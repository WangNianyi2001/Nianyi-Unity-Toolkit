using System.Collections.Generic;

namespace Nianyi.Data {
	public class Dcel<HE, V, S>
		where HE : Dcel<HE, V, S>.HalfEdge, new()
		where V : Dcel<HE, V, S>.Vertex, new()
		where S : Dcel<HE, V, S>.Surface, new() {
		#region Fields
		public class HalfEdge {
			public readonly List<HE> twins = new();
			public HE next;
			public S surface;
			public V from;
			public V To {
				get => next.from;
				set => next.from = value;
			}
		}

		public class Vertex {
			public readonly List<HE> outGoingHalfEdges = new();
			public HE HalfEdgeOf(S surface) {
				foreach(var halfEdge in outGoingHalfEdges) {
					if(halfEdge.surface == surface)
						return halfEdge;
				}
				return null;
			}
		}

		public class Surface {
			public HE oneHalfEdge;
			public IEnumerable<HE> HalfEdges {
				get {
					var halfEdge = oneHalfEdge;
					do {
						yield return halfEdge;
						halfEdge = halfEdge.next;
					} while(halfEdge != oneHalfEdge);
				}
			}
			public IEnumerable<V> Vertices {
				get {
					var halfEdge = oneHalfEdge;
					do {
						yield return halfEdge.from;
						halfEdge = halfEdge.next;
					} while(halfEdge != oneHalfEdge);
				}
			}
		}

		public List<HE> halfEdges = new();
		public List<V> vertices = new();
		public List<S> surfaces = new();
		#endregion

		#region Internal functions
		private HE CreateHalfEdgeOnSurface(V from, V to, S surface) {
			var halfEdge = new HE();
			halfEdges.Add(halfEdge);
			halfEdge.from = from;
			from.outGoingHalfEdges.Add(halfEdge);
			halfEdge.surface = surface;
			foreach(var twin in FindHalfEdges(to, from)) {
				halfEdge.twins.Add(twin);
				twin.twins.Add(halfEdge);
			}
			return halfEdge;
		}
		#endregion

		#region Public interfaces
		public int VertexCount => vertices.Count;
		public int HalfEdgeCount => halfEdges.Count;
		public int SurfaceCount => surfaces.Count;

		public V AddVertex() {
			V vertex = new();
			vertices.Add(vertex);
			return vertex;
		}

		public IEnumerable<HE> FindHalfEdges(V from, V to) {
			foreach(var halfEdge in halfEdges) {
				if(halfEdge.next == null)
					continue;
				if(halfEdge.from == from && halfEdge.To == to)
					yield return halfEdge;
			}
		}

		public IEnumerable<S> FindSurfaces(V a, V b, V c) {
			foreach(var ab in FindHalfEdges(a, b)) {
				if(ab.next.To != c)
					continue;
				yield return ab.surface;
			}
		}

		public S CreateSurface(V a, V b, V c) {
			S surface = new();
			surfaces.Add(surface);
			HE
				ab = CreateHalfEdgeOnSurface(a, b, surface),
				bc = CreateHalfEdgeOnSurface(b, c, surface),
				ca = CreateHalfEdgeOnSurface(c, a, surface);
			ab.next = bc;
			bc.next = ca;
			ca.next = ab;
			surface.oneHalfEdge = ab;
			return surface;
		}

		public void RemoveSurface(S surface) {
			foreach(var halfEdge in surface.HalfEdges) {
				foreach(var twin in halfEdge.twins)
					twin.twins.Remove(halfEdge);
				halfEdges.Remove(halfEdge);
			}
			surfaces.Remove(surface);
		}
		#endregion
	}
}