using UnityEngine;

namespace Nianyi {
	[RequireComponent(typeof(Collider))]
	public class Interactive : MonoBehaviour {
		#region Inspector fields
		public Callback onFocus;
		public Callback onBlur;
		public Callback onInteract;
		public Callback onRelease;
		#endregion

		#region Core fields
		bool focused = false;
		bool interacting = false;
		#endregion

		#region Public interfaces
		public bool Focused {
			get => isActiveAndEnabled && focused;
			set {
				if(!isActiveAndEnabled)
					value = false;
				if(focused == value)
					return;
				if(focused = value)
					onFocus?.InvokeSync();
				else {
					if(Interacting)
						Interacting = false;
					onBlur?.InvokeSync();
				}
			}
		}
		public bool Interacting {
			get => isActiveAndEnabled && interacting;
			set {
				if(!isActiveAndEnabled)
					value = false;
				if(interacting == value)
					return;
				if(interacting = value) {
					if(!Focused)
						Focused = true;
					onInteract?.InvokeSync();
				}
				else
					onRelease?.InvokeSync();
			}
		}
		#endregion

		#region Life cycle
		protected void OnDisable() {
			Focused = false;
		}

		protected void OnDrawGizmos() {
			if(!Application.isPlaying)
				return;
			
			if(!Focused)
				return;

			Color color = interacting ? Color.red : Color.yellow;
			color.a = .3f;
			Gizmos.color = color;

			foreach(var mesh in GetComponentsInChildren<MeshFilter>()) {
				var tr = mesh.transform;
				Gizmos.DrawMesh(mesh.sharedMesh, tr.position, tr.rotation, tr.lossyScale);
			}
		}
		#endregion
	}
}