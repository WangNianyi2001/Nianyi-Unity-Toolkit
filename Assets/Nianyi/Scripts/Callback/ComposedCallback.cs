using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Callback/Composed")]
	public class ComposedCallback : Callback {
		public List<Callback> sequence = new List<Callback>();

		public override bool Asynchronousable => sequence.Any(callback => callback.Asynchronousable);

		public override void InvokeSync() {
			foreach(var callback in sequence)
				callback.Invoke();
		}

		public override IEnumerator InvokeAsync() {
			if(asynchronous) {
				foreach(var callback in sequence)
					yield return callback.InvokeAsync();
			}
			else
				InvokeSync();
		}
	}
}