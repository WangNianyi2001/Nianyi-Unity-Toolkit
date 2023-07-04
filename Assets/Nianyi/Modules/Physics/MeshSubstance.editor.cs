#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Nianyi {
	[ExecuteAlways]
	public partial class MeshSubstance {
		#region Internal fields
		private GameObject gizmosObject;
		private MeshFilter gizmosFilter;
		private MeshRenderer gizmosRenderer;
		#endregion

		#region Serialized fields
		public bool drawMeshGizmos;
		[ShowIfBool("drawMeshGizmos")] public Data.Mesh.GizmosOptions meshGizmosOptions;
		#endregion

		#region Internal functions
		private GameObject GizmosObject {
			get {
				if(gizmosObject != null)
					return gizmosObject;

				gizmosObject = new GameObject($"{gameObject.name} (Gizmos)");
				gizmosObject.hideFlags = HideFlags.DontSave;

				return gizmosObject;
			}
		}
		private Transform GizmosTransform => GizmosObject.transform;
		private MeshFilter GizmosFilter => gizmosFilter = GizmosObject.EnsureComponent(gizmosFilter);
		private MeshRenderer GizmosRenderer => gizmosRenderer = GizmosObject.EnsureComponent(gizmosRenderer);
		#endregion

		#region Life cycle
		protected void OnEditEnable() {
			GizmosObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

			GizmosTransform.parent = transform;
			GizmosTransform.localPosition = Vector3.zero;
			GizmosTransform.localRotation = Quaternion.identity;
			GizmosTransform.localScale = Vector3.one;
		}

		protected void OnEditUpdate() {
			GizmosFilter.sharedMesh = mesh?.sourceMesh;
			GizmosRenderer.sharedMaterials = materials;
		}

		protected void OnEditValidate() {
			GizmosObject.SetActive(enabled);
		}

		protected void OnDrawGizmos() {
			if(!isActiveAndEnabled || mesh == null)
				return;
			if(drawMeshGizmos)
				mesh.DrawGizmos(transform, in meshGizmosOptions);
		}
		#endregion
	}
}
#endif