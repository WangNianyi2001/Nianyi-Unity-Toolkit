using UnityEngine;
using UnityEditor;

namespace Nianyi.Editor {
	[CustomEditor(typeof(CallbackFilter))]
	public class CallbackFilterEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			var filter = target as CallbackFilter;
			if(Application.isPlaying && filter.callback != null) {
				if(GUILayout.Button("Invoke"))
					filter.Invoke();
			}
			base.OnInspectorGUI();
		}
	}
}
