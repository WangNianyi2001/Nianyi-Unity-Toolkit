﻿using System.Collections;
using System.Collections.Generic;

namespace Nianyi {
	public class ComposedCallback : Callback {
		public List<Callback> sequence = new List<Callback>();

		public override IEnumerator Invoke() {
			if(!asynchrnous) {
				foreach(var callback in sequence)
					callback.Invoke();
				yield break;
			}
			foreach(var callback in sequence)
				yield return callback.Invoke();
		}
	}
}