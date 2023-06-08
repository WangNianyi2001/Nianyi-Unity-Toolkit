#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Nianyi {
	public partial class MeshManifold {
		#region Internal functions
		private bool IsFacingCamera(Vector3 position, Vector3 direction) {
			Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
			Vector3 cameraPosition = sceneCamera.transform.position;
			cameraPosition = transform.worldToLocalMatrix.MultiplyPoint(cameraPosition);
			return Vector3.Dot(cameraPosition - position, direction) > 0;
		}
		#endregion

		#region Life cycle
		protected void OnEditUpdate() {
			filter = GetComponent<MeshFilter>();
			renderer = GetComponent<MeshRenderer>();

			Mesh = Mesh;
			if(Mesh == null)
				data = null;
			else {
				if(IsMeshHandlable())
					RegenerateMeshData();
				else
					data = null;
			}
		}

		protected void OnDrawGizmos() {
			if(!isActiveAndEnabled)
				return;
			Matrix4x4 toWorld = transform.localToWorldMatrix;
			if(data != null) {
				Gizmos.color = Color.white;
				// Draw edges
				Color edgeColor = Color.green, brinkColor = Color.red;
				edgeColor.a = .2f;
				foreach(var halfEdge in data.halfEdges) {
					if(!IsFacingCamera(halfEdge.from.position, halfEdge.surface.normal))
						continue;
					var from = toWorld.MultiplyPoint(halfEdge.from.position);
					var to = toWorld.MultiplyPoint(halfEdge.To.position);
					// If edge is on brink, draw with red; otherwise green
					Gizmos.color = halfEdge.twin != null ? edgeColor : brinkColor;
					Gizmos.DrawLine(from, to);
				}
				// Draw vertices
				float vertexSize = .005f;
				Color vertexColor = Color.red;
				vertexColor.a = .7f;
				float vertexNormalLength = .02f;
				Color vertexNormalColor = Color.yellow;
				vertexNormalColor.a = .3f;
				foreach(var vertex in data.vertices) {
					if(!vertex.HalfEdges.Any(halfEdge => IsFacingCamera(vertex.position, halfEdge.surface.normal)))
						continue;
					Gizmos.color = vertexColor;
					Vector3 vertexPosition = toWorld.MultiplyPoint(vertex.position);
					Gizmos.DrawSphere(vertexPosition, vertexSize);
					Gizmos.color = vertexNormalColor;
					Gizmos.DrawLine(vertexPosition, vertexPosition + toWorld.MultiplyVector(vertex.normal) * vertexNormalLength);
				}
				// Draw surface normals
				float surfaceNormalLength = vertexNormalLength;
				Color surfaceNormalColor = Color.cyan;
				surfaceNormalColor.a = .7f;
				Gizmos.color = surfaceNormalColor;
				foreach(var surface in data.surfaces) {
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