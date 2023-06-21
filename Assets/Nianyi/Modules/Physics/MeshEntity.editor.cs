#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace Nianyi {
	[ExecuteAlways]
	public partial class MeshEntity {
		#region Internal fields
		private GameObject gizmosObject;
		private MeshFilter gizmosFilter;
		private MeshRenderer gizmosRenderer;
		#endregion

		#region Serialized fields
		[Serializable]
		public struct GizmosOptions {
			public bool backFaceCulling;
			public bool offsetVertices;
		}
		public GizmosOptions gizmosOptions;
		#endregion

		#region Internal functions
		private GameObject GizmosObject {
			get {
				if(gizmosObject != null)
					return gizmosObject;

				gizmosObject = new GameObject($"${gameObject.name} (Gizmos)");
				gizmosObject.hideFlags = HideFlags.HideAndDontSave;

				gizmosRenderer = gizmosObject.AddComponent<MeshRenderer>();
				gizmosFilter = gizmosObject.AddComponent<MeshFilter>();

				return gizmosObject;
			}
		}
		private Transform GizmosTransform => GizmosObject.transform;
		private MeshFilter GizmosFilter {
			get {
				if(gizmosFilter && gizmosFilter.gameObject == GizmosObject)
					return gizmosFilter;
				gizmosFilter = GizmosObject.GetComponent<MeshFilter>();
				if(!gizmosFilter)
					gizmosFilter = GizmosObject.AddComponent<MeshFilter>();
				return gizmosFilter;
			}
		}
		private MeshRenderer GizmosRenderer {
			get {
				if(gizmosRenderer && gizmosRenderer.gameObject == GizmosObject)
					return gizmosRenderer;
				gizmosRenderer = GizmosObject.GetComponent<MeshRenderer>();
				if(!gizmosRenderer)
					gizmosRenderer = GizmosObject.AddComponent<MeshRenderer>();
				return gizmosRenderer;
			}
		}

		private bool IsFacingCamera(Vector3 position, Vector3 direction) {
			Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
			Vector3 cameraPosition = sceneCamera.transform.position;
			cameraPosition = transform.worldToLocalMatrix.MultiplyPoint(cameraPosition);
			return Vector3.Dot(cameraPosition - position, direction) > 0;
		}
		#endregion

		#region Life cycle
		protected void OnEditUpdate() {
			GizmosTransform.parent = transform;
			GizmosTransform.localPosition = Vector3.zero;
			GizmosTransform.localRotation = Quaternion.identity;
			GizmosTransform.localScale = Vector3.one;

			GizmosFilter.sharedMesh = mesh.sourceMesh;
			GizmosRenderer.sharedMaterials = materials;
		}

		protected void OnDrawGizmos() {
			if(!isActiveAndEnabled)
				return;
			Matrix4x4 toWorld = transform.localToWorldMatrix;
			if(mesh != null) {
				Gizmos.color = Color.white;
				// Draw edges
				Color edgeColor = Color.green, brinkColor = Color.red;
				edgeColor.a = .2f;
				foreach(var halfEdge in mesh.data.halfEdges) {
					if(gizmosOptions.backFaceCulling) {
						if(!IsFacingCamera(halfEdge.from.position, halfEdge.surface.normal))
							continue;
					}
					var from = toWorld.MultiplyPoint(halfEdge.from.position);
					var to = toWorld.MultiplyPoint(halfEdge.To.position);
					// If edge is on brink, draw with red; otherwise green
					Gizmos.color = halfEdge.twins.Count != 0 ? edgeColor : brinkColor;
					Gizmos.DrawLine(from, to);
				}
				// Draw vertices
				float vertexSize = .005f;
				Color vertexColor = Color.red;
				vertexColor.a = .7f;
				float vertexNormalLength = .02f;
				Color vertexNormalColor = Color.yellow;
				vertexNormalColor.a = .3f;
				foreach(var vertex in mesh.data.vertices) {
					if(!vertex.outGoingHalfEdges.Any(halfEdge => IsFacingCamera(vertex.position, halfEdge.surface.normal)))
						continue;
					Gizmos.color = vertexColor;
					Vector3 vertexPosition = vertex.position;
					if(gizmosOptions.offsetVertices) {
						int index = mesh.data.vertices.IndexOf(vertex);
						Vector3 offset = new Vector3(
							Mathf.Sin(index),
							0,
							Mathf.Cos(index)
						);
						vertexPosition += offset * .003f;
					}
					vertexPosition = toWorld.MultiplyPoint(vertexPosition);
					Gizmos.DrawSphere(vertexPosition, vertexSize);
					Gizmos.color = vertexNormalColor;
					Gizmos.DrawLine(vertexPosition, vertexPosition + toWorld.MultiplyVector(vertex.normal) * vertexNormalLength);
				}
				// Draw surface normals
				float surfaceNormalLength = vertexNormalLength;
				Color surfaceNormalColor = Color.cyan;
				surfaceNormalColor.a = .7f;
				Gizmos.color = surfaceNormalColor;
				foreach(var surface in mesh.data.surfaces) {
					if(!IsFacingCamera(surface.center, surface.normal))
						continue;
					Vector3 center = toWorld.MultiplyPoint(surface.center);
					Gizmos.DrawLine(center, center + toWorld.MultiplyVector(surface.normal) * surfaceNormalLength);
				}
			}
		}
		#endregion
	}
}
#endif