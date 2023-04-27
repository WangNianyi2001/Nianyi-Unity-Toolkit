using UnityEngine;
using UnityEditor;

namespace Nianyi.Editor {
	public static class GuiUtility {
		public struct GUIStyles {
			public GUIStyle PlainText, Label;
			public GUIStyle DropdownButton, Button;
		}
		public static GUIStyles guiStyles = new GUIStyles {
			PlainText = new GUIStyle(EditorStyles.helpBox) {
				alignment = TextAnchor.UpperCenter,
			},
			Label = EditorStyles.whiteLabel,
			DropdownButton = new GUIStyle("dropdownbutton") {
				alignment = TextAnchor.MiddleLeft,
				padding = new RectOffset() {
					left = 4,
					right = 4,
				}
			},
			Button = new GUIStyle("button") {
				alignment = TextAnchor.MiddleCenter,
			},
		};
	}
}
