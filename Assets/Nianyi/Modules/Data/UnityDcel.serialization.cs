using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi.Data {
	public partial class UnityDcel : ISerializationCallbackReceiver {
		[Serializable]
		private class SerializableVertex : Vertex {
			public SerializableVertex(Vertex vertex) {
				position = vertex.position;
				normal = vertex.normal;
			}
		}

		[Serializable]
		private class SerializableSurface : Surface {
			public int[] indices;
			public SerializableSurface(UnityDcel dcel, Surface surface) {
				center = surface.center;
				normal = surface.normal;
				indices = surface.Vertices.Select(v => dcel.vertices.IndexOf(v)).ToArray();
			}
		}

		[SerializeField] private SerializableVertex[] serializedVertices;
		[SerializeField] private SerializableSurface[] serializedSurfaces;

		public void OnBeforeSerialize() {
			serializedVertices = vertices.Select(v => new SerializableVertex(v)).ToArray();
			serializedSurfaces = surfaces.Select(s => new SerializableSurface(this, s)).ToArray();
		}

		public void OnAfterDeserialize() {
			// Add vertices
			vertices = new List<Vertex>();
			for(int i = 0; i < serializedVertices.Length; ++i) {
				var sv = serializedVertices[i];
				var v = AddVertex();
				v.position = sv.position;
				v.normal = sv.normal;
			}

			// Add surfaces
			surfaces = new List<Surface>();
			for(int i = 0; i < serializedSurfaces.Length; ++i) {
				var sf = serializedSurfaces[i];
				if(sf.indices.Length != 3) {
					Debug.LogWarning("Non-triangle surface encountered while deserializing mesh data");
					continue;
				}
				Vertex
					a = vertices[sf.indices[0]],
					b = vertices[sf.indices[1]],
					c = vertices[sf.indices[2]];
				var surface = CreateSurface(a, b, c);
				surface.center = sf.center;
				surface.normal = sf.normal;
			}
		}
	}
}