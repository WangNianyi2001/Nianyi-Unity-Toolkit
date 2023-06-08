using System.Collections.Generic;

namespace Nianyi.Data.Dcel {
	public class HalfEdge<HE, V, S>
		where HE : HalfEdge<HE, V, S>
		where V : Vertex<HE, V, S>
		where S : Surface<HE, V, S> {
		public HE twin;
		public HE next;
		public S surface;
		public V from;
		public V To => next?.from;
	}

	public class Vertex<HE, V, S>
		where HE : HalfEdge<HE, V, S>
		where V : Vertex<HE, V, S>
		where S : Surface<HE, V, S> {
		public HE oneOutGoingHalfEdge;
		public IEnumerable<HE> HalfEdges {
			get {
				var halfEdge = oneOutGoingHalfEdge;
				do {
					yield return halfEdge;
					halfEdge = halfEdge.twin?.next;
				} while(halfEdge != oneOutGoingHalfEdge && halfEdge != null);
			}
		}
		public HE HalfEdgeOf(S surface) {
			foreach(var halfEdge in HalfEdges) {
				if(halfEdge.surface == surface)
					return halfEdge;
			}
			return null;
		}
	}

	public class Surface<HE, V, S>
		where HE : HalfEdge<HE, V, S>
		where V : Vertex<HE, V, S>
		where S : Surface<HE, V, S> {
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

	public class Mesh<HE, V, S>
		where HE : HalfEdge<HE, V, S>, new()
		where V : Vertex<HE, V, S>, new()
		where S : Surface<HE, V, S>, new() {

		public List<HE> halfEdges;
		public List<V> vertices;
		public List<S> surfaces;

		public Mesh() {
			halfEdges = new List<HE>();
			vertices = new List<V>();
			surfaces = new List<S>();
		}

		public V AddVertex() {
			V vertex = new V();
			vertices.Add(vertex);
			return vertex;
		}

		public HE FindHalfEdge(V from, V to) {
			foreach(var halfEdge in halfEdges) {
				if(halfEdge.from == from && halfEdge.To == to)
					return halfEdge;
			}
			return null;
		}

		public S FindSurface(V a, V b, V c) {
			var ab = FindHalfEdge(a, b);
			if(ab == null)
				return null;
			if(ab.next.To != c)
				return null;
			return ab.surface;
		}

		public HE AddHalfEdge(V a, V b, bool noCheck = false) {
			HE halfEdge = null;
			if(!noCheck)
				halfEdge = FindHalfEdge(a, b);
			if(halfEdge == null) {
				halfEdge = new HE();
				halfEdge.from = a;
				halfEdges.Add(halfEdge);
			}
			var twin = FindHalfEdge(b, a);
			if(twin != null) {
				twin.from = b;
				halfEdge.twin = twin;
				twin.twin = halfEdge;
			}
			if(a.oneOutGoingHalfEdge == null || twin == null)
				a.oneOutGoingHalfEdge = halfEdge;
			return halfEdge;
		}

		public S AddSurface(V a, V b, V c, bool noCheck = false) {
			S surface;
			if(!noCheck) {
				surface = FindSurface(a, b, c);
				if(surface != null)
					return surface;
			}
			surface = new S();
			HE
				ab = AddHalfEdge(a, b, noCheck),
				bc = AddHalfEdge(b, c, noCheck),
				ca = AddHalfEdge(c, a, noCheck);
			ab.next = bc;
			bc.next = ca;
			ca.next = ab;
			surface.oneHalfEdge = ab;
			ab.surface = bc.surface = ca.surface = surface;
			surfaces.Add(surface);
			return surface;
		}
	}
}