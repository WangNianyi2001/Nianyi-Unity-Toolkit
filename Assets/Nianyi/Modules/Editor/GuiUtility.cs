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
			Object rootObject = property.serializedObject.targetObject;
			string path = property.propertyPath;
			// TODO: Fully resolve property path
			System.Type type = rootObject.GetType();
			FieldInfo accessor = type.GetField(path, ReflectionUtility.bindingFlagsDontCare);
			if(accessor == null)
				throw new TargetException($"Cannot find {path} in type {type}");
			return (T)accessor.GetValue(rootObject);
		}
		public static void SetUnderlyingMember<T>(this SerializedProperty property, T value) {
			Object rootObject = property.serializedObject.targetObject;
			string path = property.propertyPath;
			// TODO: Fully resolve property path
			System.Type type = rootObject.GetType();
			FieldInfo accessor = type.GetField(path, ReflectionUtility.bindingFlagsDontCare);
			if(accessor == null)
				throw new TargetException($"Cannot find {path} in type {type}");
			accessor.SetValue(rootObject, value);
		}
		#endregion
	}
}
