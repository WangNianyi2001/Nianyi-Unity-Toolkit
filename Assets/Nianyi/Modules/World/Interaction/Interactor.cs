using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	public abstract class Interactor : MonoBehaviour {
		#region Core fields
		IEnumerable<Interactive> previousTargets = new List<Interactive>();
		bool interacting = false;
		#endregion

		#region Public interfaces
		public abstract IEnumerable<Interactive> Targets {
			get;
		}

		public bool Interacting {
			get => isActiveAndEnabled && interacting;
			set {
				if(!isActiveAndEnabled)
					value = false;
				if(value == interacting)
					return;
				foreach(var target in Targets)
					target.Interacting = value;
			}
		}
		#endregion

		#region Life cycle
		protected void LateUpdate() {
			var currentTargets = Targets;
			foreach(var previousTarget in previousTargets) {
				if(!currentTargets.Contains(previousTarget))
					previousTarget.Focused = false;
			}
			foreach(var currentTarget in currentTargets) {
				if(!previousTargets.Contains(currentTarget)) {
					currentTarget.Focused = true;
				}
			}
			previousTargets = currentTargets.ToArray();
		}
		#endregion
	}
}