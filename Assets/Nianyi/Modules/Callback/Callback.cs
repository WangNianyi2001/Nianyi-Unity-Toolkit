using UnityEngine;
using System.Collections;

namespace Nianyi {
	public abstract class Callback : ScriptableObject {
		public bool asynchronous;

		public abstract void InvokeSync();
		public abstract IEnumerator InvokeAsync();
	}
}