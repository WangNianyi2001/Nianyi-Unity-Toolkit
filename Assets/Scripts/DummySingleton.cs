using UnityEngine;

namespace Nianyi.Demo {
	[Singleton("instance")]
	public class DummySingleton : MonoBehaviour {
		public static DummySingleton instance;
		public DummySingleton() => instance = this;

		public static void LogFromStatic() {
            UnityEngine.Debug.Log("Log from dummy singleton static!");
		}
		public void LogFromInstance() {
            UnityEngine.Debug.Log("Log from dummy singleton instance!");
		}
	}
}