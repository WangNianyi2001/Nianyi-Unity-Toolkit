using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
	public class ShowIfEnumAttributeDrawer : PropertyDrawerBase {
		bool show = false;

		protected static bool DetermineVisibility(ShowIfEnumAttribute attribute, SerializedProperty member) {
			if(attribute == null)
				return false;
			if(attribute.expectedEnumValues.Length <= 0)
				return true;

			SerializedProperty showProperty = member.serializedObject.FindProperty(attribute.propertyName);
			if(showProperty == null)
				return false;

			var enumType = attribute.expectedEnumValues[0].GetType();
			var enumValues = enumType.GetEnumValues().Cast<System.Enum>();
			var actualIndex = showProperty.enumValueIndex;

			return attribute.expectedEnumValues.Any(expected => System.Convert.ToInt32(expected) == actualIndex);
		}

		protected override void Draw(SerializedProperty member, GUIContent label) {
			if(!Rendering)
				show = DetermineVisibility(attribute as ShowIfEnumAttribute, member);
			if(show)
				PropertyField(member, label, true);
		}
	}
}