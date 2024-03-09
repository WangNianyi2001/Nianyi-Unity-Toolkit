using UnityEngine;
using System.Collections;

namespace Nianyi {
	public abstract class Callback : ScriptableObject {
		public bool asynchronous;
		public abstract bool Asynchronousable {
			get;
		}

		public abstract void InvokeSync();
		public abstract Coroutine InvokeAsync();

		private IEnumerator InvokeCoroutine() {
			if(Asynchronousable)
				return CoroutineHelper.Make(InvokeAsync());
			InvokeSync();
			return null;
		}

		public Coroutine Invoke() {
			return CoroutineHelper.Run(InvokeCoroutine());
		}
	}
}