using UnityEngine;

namespace Nianyi {
	[RequireComponent(typeof(InputFilter))]
	public class Interactor : MonoBehaviour {
		#region Core fields
		#endregion

		#region Public interfaces
		public Interactive Target {
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
		Interactive target = null;
		#endregion

		#region Life cycle
		protected void Update() {
			if(!target?.isActiveAndEnabled ?? false)
				target = null;
		}
		#endregion
	}
}