using System;
using System.Collections;
using System.Collections.Generic;

namespace Nianyi {
	public class ComposedCallback : Callback {
		[Serializable]
		public struct Step {
			public Callback callback;
			public bool asynchronous;
		}

		public List<Step> stepSequence;

		public override IEnumerator Invoke() {
			// TODO
			return null;
		}
	}
}