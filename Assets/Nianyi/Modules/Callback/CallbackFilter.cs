using UnityEngine;

namespace Nianyi {
	public class CallbackFilter : MonoBehaviour {
		public Callback callback;

		[ContextMenu("Invoke")]
		public Coroutine Invoke()
			=> callback?.Invoke();
	}
}