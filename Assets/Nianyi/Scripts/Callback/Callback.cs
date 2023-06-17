using UnityEngine;
using System.Collections;

namespace Nianyi {
	public abstract class Callback : ScriptableObject {
		public bool asynchronous;
		public abstract bool Asynchronousable {
			get;
		}

		public abstract void InvokeSync();
		public abstract IEnumerator InvokeAsync();

		private IEnumerator InvokeCoroutine() {
			if(Asynchronousable)
				return InvokeAsync();
			InvokeSync();
			return null;
		}

		public Coroutine Invoke() {
			return CoroutineHelper.Run(InvokeCoroutine());
		}
	}
}