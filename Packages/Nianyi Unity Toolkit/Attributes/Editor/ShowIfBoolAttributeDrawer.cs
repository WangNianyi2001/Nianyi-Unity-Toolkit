using UnityEditor;
using UnityEngine;

namespace Nianyi {
	[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
	public class ShowIfBoolAttributeDrawer : PropertyDrawer {
		bool show = false;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var showIf = attribute as ShowIfBoolAttribute;
			try {
				var accessor = new MemberAccessor(property).Navigate(1, "." + showIf.propertyName);
				show = accessor.Get<bool>();
			}
			catch {
				show = false;
			}
			return show ? EditorGUI.GetPropertyHeight(property, label) : 0;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(!show)
				return;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property, label, true);
			if(EditorGUI.EndChangeCheck())
				property.serializedObject.ApplyModifiedProperties();
		}
	}
}