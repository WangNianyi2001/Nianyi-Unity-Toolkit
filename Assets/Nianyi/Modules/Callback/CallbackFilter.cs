using UnityEngine;

namespace Nianyi {
	[ExecuteInEditMode]
	public class CallbackFilter : MonoBehaviour {
		public Callback callback;

		public void Invoke()
			=> CoroutineHelper.Run(callback?.Invoke());
	}
}