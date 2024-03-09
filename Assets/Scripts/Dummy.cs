using System.Collections;
using UnityEngine;

namespace Nianyi.Demo {
	public class Dummy : MonoBehaviour {
		public void JustDoSomething() { }

		public void DoSomethingWithComplexArguments(Vector3 vector, string text, float number) { }

		public IEnumerator DoSomethingAsync() {
			yield return null;
		}
	}
}