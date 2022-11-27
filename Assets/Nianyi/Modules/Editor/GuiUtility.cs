using UnityEngine;
using UnityEditor;
using System.Reflection;

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

		#region SerializedProperty
		public static T GetUnderlyingMember<T>(this SerializedProperty property) {
			UnityEngine.Object rootObject = property.serializedObject.targetObject;
			string path = property.propertyPath;
			// TODO: Fully resolve property path
			FieldInfo accessor = rootObject.GetType().GetField(path);
			if(accessor == null)
				return default;
			return (T)accessor.GetValue(rootObject);
		}
		public static void SetUnderlyingMember<T>(this SerializedProperty property, T value) {
			UnityEngine.Object rootObject = property.serializedObject.targetObject;
			string path = property.propertyPath;
			// TODO: Fully resolve property path
			FieldInfo accessor = rootObject.GetType().GetField(path);
			accessor.SetValue(rootObject, value);
		}
		#endregion
	}
}
