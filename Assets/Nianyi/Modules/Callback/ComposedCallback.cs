using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Callback/Composed")]
	public class ComposedCallback : Callback {
		public List<Callback> sequence = new List<Callback>();

		public override void InvokeSync() {
			if(asynchronous) {
				CoroutineHelper.Run(InvokeAsync());
				return;
			}
			foreach(var callback in sequence)
				callback.InvokeSync();
		}

		public override IEnumerator InvokeAsync() {
			foreach(var callback in sequence) {
				if(asynchronous)
					yield return callback.InvokeAsync();
				else
					callback.InvokeSync();
			}
		}
	}
}