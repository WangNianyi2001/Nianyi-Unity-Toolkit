using UnityEngine;

namespace Nianyi {
	[RequireComponent(typeof(InputFilter))]
	public class Selector : MonoBehaviour {
		#region Core fields
		#endregion

		#region Public interfaces
		public SelectTarget Target {
			get => target;
			set {
				if(target == value)
					return;
				if(target)
					target.Selected = false;
				target = value;
				if(target)
					target.Selected = true;
			}
		}
#endregion

#region Core fields
		SelectTarget target = null;
#endregion

#region Life cycle
		void Update() {
			if(!target.isActiveAndEnabled)
				target = null;
		}
#endregion
	}
}