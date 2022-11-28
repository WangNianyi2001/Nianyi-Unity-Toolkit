using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Callback/Legacy")]
	public class LegacyCallback : Callback {
		public UnityEvent unityEvent;

		public override IEnumerator Invoke() {
			unityEvent.Invoke();
			return null;
		}
	}
}