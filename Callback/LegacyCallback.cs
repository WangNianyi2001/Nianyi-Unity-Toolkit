using UnityEngine;
using UnityEngine.Events;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Callback/Legacy")]
	public class LegacyCallback : Callback {
		public UnityEvent unityEvent;

		public override bool Asynchronousable => false;

		public override void InvokeSync() {
			unityEvent.Invoke();
		}

		public override Coroutine InvokeAsync() {
			unityEvent.Invoke();
			return null;
		}
	}
}