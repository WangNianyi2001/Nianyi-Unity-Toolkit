using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Nianyi {
	public class ComposedCallback : Callback {
		public List<Callback> sequence = new List<Callback>();

		IEnumerator InvokeCoroutine() {
			foreach(var callback in sequence)
				yield return callback.Invoke();
		}

		public override Coroutine Invoke() {
			if(asynchrnous)
				return StartCoroutine(InvokeCoroutine());
			foreach(var callback in sequence)
				callback.Invoke();
			return null;
		}
	}
}