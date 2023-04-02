using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	public abstract class Interactor : MonoBehaviour {
		#region Core fields
		IEnumerable<Interactive> previousTargets = new List<Interactive>();
		#endregion

		#region Public interfaces
		public abstract IEnumerable<Interactive> Targets {
			get;
		}

		public void Interact() {
			foreach(var target in Targets)
				target.SendMessage("OnInteract");
		}
		#endregion

		#region Life cycle
		protected void LateUpdate() {
			var currentTargets = Targets;
			foreach(var previousTarget in previousTargets) {
				if(!currentTargets.Contains(previousTarget))
					previousTarget.SendMessage("OnBlur");
			}
			foreach(var currentTarget in currentTargets) {
				if(!previousTargets.Contains(currentTarget))
					currentTarget.SendMessage("OnFocus");
			}
			previousTargets = currentTargets;
		}

		protected void OnDrawGizmos() {
			if(!Application.isPlaying)
				return;

			Gizmos.color = new Color(1, 0, 0, .2f);
			foreach(var target in Targets) {
				foreach(var mesh in target.GetComponentsInChildren<MeshFilter>()) {
					var tr = mesh.transform;
					Gizmos.DrawMesh(mesh.sharedMesh, tr.position, tr.rotation, tr.lossyScale);
				}
			}
		}
		#endregion
	}
}