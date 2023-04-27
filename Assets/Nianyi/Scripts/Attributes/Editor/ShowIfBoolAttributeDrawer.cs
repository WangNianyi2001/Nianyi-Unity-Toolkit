using UnityEditor;
using UnityEngine;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
	public class ShowIfBoolAttributeDrawer : PropertyDrawer {
		bool show = false;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var showIf = attribute as ShowIfBoolAttribute;
			SerializedProperty showProperty = property.serializedObject.FindProperty(showIf.propertyName);
			show = showProperty != null ? showProperty.boolValue == showIf.expected : false;
			return show ? EditorGUI.GetPropertyHeight(property, label) : 0;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(!show)
				return;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property, label);
			if(EditorGUI.EndChangeCheck())
				property.serializedObject.ApplyModifiedProperties();
		}
	}
}