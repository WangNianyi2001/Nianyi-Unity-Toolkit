#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nianyi.Data {
	public partial class Mesh {
		[Serializable]
		public struct GizmosOptions {
			public bool backFaceCulling;
			public bool offsetVertices;
		}

		protected void DrawEdges(Transform transform, in GizmosOptions meshGizmosOptions) {
			Color edgeColor = Color.green, brinkColor = Color.red;
			edgeColor.a = .2f;
			foreach(var submesh in submeshData) {
				foreach(var halfEdge in submesh.halfEdges) {
					if(meshGizmosOptions.backFaceCulling) {
						if(!transform.IsFacingCameraLocal(halfEdge.from.position, halfEdge.surface.normal))
							continue;
					}
					var from = transform.localToWorldMatrix.MultiplyPoint(halfEdge.from.position);
					var to = transform.localToWorldMatrix.MultiplyPoint(halfEdge.To.position);
					// If edge is on brink, draw with red; otherwise green
					Gizmos.color = halfEdge.twins.Count != 0 ? edgeColor : brinkColor;
					Gizmos.DrawLine(from, to);
				}
			}
		}

		protected void DrawVertices(Transform transform, in GizmosOptions meshGizmosOptions) {
			float vertexSize = .005f;
			Color vertexColor = Color.red;
			vertexColor.a = .7f;
			float vertexNormalLength = .02f;
			Color vertexNormalColor = Color.yellow;
			vertexNormalColor.a = .3f;
			foreach(var submesh in submeshData) {
				foreach(var vertex in submesh.vertices) {
					if(!vertex.outGoingHalfEdges.Any(halfEdge => transform.IsFacingCameraLocal(vertex.position, halfEdge.surface.normal)))
						continue;
					Gizmos.color = vertexColor;
					Vector3 vertexPosition = vertex.position;
					if(meshGizmosOptions.offsetVertices) {
						int index = submesh.vertices.IndexOf(vertex);
						Vector3 offset = new(
							Mathf.Sin(index),
							0,
							Mathf.Cos(index)
						);
						vertexPosition += offset * .003f;
					}
					vertexPosition = transform.localToWorldMatrix.MultiplyPoint(vertexPosition);
					Gizmos.DrawSphere(vertexPosition, vertexSize);
					Gizmos.color = vertexNormalColor;
					Gizmos.DrawLine(vertexPosition, vertexPosition + transform.localToWorldMatrix.MultiplyVector(vertex.normal) * vertexNormalLength);
				}
			}
		}

		protected void DrawSurfaceNormals(Transform transform, in GizmosOptions meshGizmosOptions) {
			float surfaceNormalLength = .02f;
			Color surfaceNormalColor = Color.cyan;
			surfaceNormalColor.a = .7f;
			Gizmos.color = surfaceNormalColor;
			foreach(var submesh in submeshData) {
				foreach(var surface in submesh.surfaces) {
					if(!transform.IsFacingCameraLocal(surface.center, surface.normal))
						continue;
					Vector3 center = transform.localToWorldMatrix.MultiplyPoint(surface.center);
					Gizmos.DrawLine(center, center + transform.localToWorldMatrix.MultiplyVector(surface.normal) * surfaceNormalLength);
				}
			}
		}

		protected void DrawGrids(Transform transform, in GizmosOptions meshGizmosOptions) {
			var color = Color.blue;
			color.a = .3f;
			Gizmos.color = color;
			vertexGrid.DrawGizmos(transform.localToWorldMatrix * gridToLocalMatrix);
		}

		public void DrawGizmos(Transform transform, in GizmosOptions meshGizmosOptions) {
			Gizmos.color = Color.white;
			DrawEdges(transform, in meshGizmosOptions);
			DrawVertices(transform, in meshGizmosOptions);
			DrawSurfaceNormals(transform, in meshGizmosOptions);
			if(VertexGrid != null) {
				DrawGrids(transform, in meshGizmosOptions);
			}
		}
	}
}
#endif