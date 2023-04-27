using UnityEditor;
using UnityEngine;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
	public class ShowIfEnumAttributeDrawer : PropertyDrawerBase {
		bool show = false;

		protected override void Draw(SerializedProperty member, GUIContent label) {
			if(!Rendering) {
				var showIf = attribute as ShowIfEnumAttribute;
				SerializedProperty showProperty = member.serializedObject.FindProperty(showIf.propertyName);
				show = showProperty != null ? showProperty.enumNames[showProperty.enumValueIndex] == showIf.expected : false;
			}
			if(show)
				PropertyField(member, label);
		}
	}
}