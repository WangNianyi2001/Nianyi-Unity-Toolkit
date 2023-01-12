using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Callback/Legacy")]
	public class LegacyCallback : Callback {
		public UnityEvent unityEvent;

		public override void InvokeSync() {
			unityEvent.Invoke();
		}

		public override IEnumerator InvokeAsync() {
			unityEvent.Invoke();
			return null;
		}
	}
}