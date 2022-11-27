using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

namespace Nianyi {
	public abstract class Callback : ScriptableObject {
		public abstract Coroutine Invoke();
		public bool asynchrnous;

		public static Coroutine StartCoroutine(IEnumerator coroutine) {
			var gameObject = FindObjectOfType<MonoBehaviour>();
			return gameObject.StartCoroutine(coroutine);
		}

		public static Coroutine MakeCoroutine(object returnValue) {
			switch(returnValue) {
				default:
					return null;
				case IEnumerator enumerator:
					return StartCoroutine(enumerator);
				case Task task:
					return StartCoroutine(new WaitUntil(() => task.IsCompleted));
			}
		}

		public static IEnumerable WaitForSeconds(float seconds) {
			yield return new WaitForSeconds(seconds);
		}
	}
}