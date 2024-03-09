using UnityEngine;

namespace Nianyi.Demo {
	[Singleton("instance")]
	public class DummySingleton : MonoBehaviour {
		public static DummySingleton instance;
		public DummySingleton() => instance = this;

		public static void LogFromStatic() {
			Debug.Log("Log from dummy singleton static!");
		}
		public void LogFromInstance() {
			Debug.Log("Log from dummy singleton instance!");
		}
	}
}