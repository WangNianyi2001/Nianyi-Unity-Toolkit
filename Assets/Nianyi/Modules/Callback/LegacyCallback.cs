using UnityEngine;
using UnityEngine.Events;

namespace Nianyi {
	public class LegacyCallback : Callback {
		public UnityEvent unityEvent;

		public override Coroutine Invoke() {
			unityEvent.Invoke();
			return null;
		}
	}
}