using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nianyi {
	public class ComposedCallback : Callback {
		[Serializable]
		public struct Step {
			public Callback callback;
			public bool asynchronous;
		}

		public List<Step> stepSequence;

		public override Coroutine Invoke() {
			// TODO
			return null;
		}
	}
}