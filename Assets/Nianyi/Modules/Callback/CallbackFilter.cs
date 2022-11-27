using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nianyi {
	[ExecuteInEditMode]
	public class CallbackFilter : MonoBehaviour {
		public Callback callback;

		public IEnumerator Invoke() {
			if(!isActiveAndEnabled)
				yield break;
			yield return CoroutineHelper.Run(callback?.Invoke());
		}

		[ContextMenu("Invoke")]
		public void InvokeFromInspector() {
#if UNITY_EDITOR
			if(callback.asynchrnous && !Application.isPlaying) {
				EditorUtility.DisplayDialog(
					"Warning",
					"Unity application is now in edit mode, cannot invoke asynchronous callback.",
					"OK"
				);
				return;
			}
#endif
			CoroutineHelper.Run(Invoke());
		}
	}
}