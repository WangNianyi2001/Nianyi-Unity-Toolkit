using UnityEngine;
using System.Collections;

namespace Nianyi {
	public abstract class Callback : ScriptableObject {
		public abstract IEnumerator Invoke();
		public bool asynchrnous;
	}
}