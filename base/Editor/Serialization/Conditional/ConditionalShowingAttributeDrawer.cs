using UnityEngine;
using UnityEditor;

namespace Nianyi.UnityToolkit
{
	[CustomPropertyDrawer(typeof(ConditionalShowingAttribute))]
	public class ConditionalShowingAttributeDrawer : PropertyDrawer
	{
		private bool ShouldShow(SerializedProperty property)
		{
			bool showing = true;
			MemberAccessor baseProperty = new(property);

			foreach(var attribute in fieldInfo.GetCustomAttributes(false))
			{
				switch(attribute as ConditionalShowingAttribute)
				{
					default:
						continue;
					case ShowWhenAttribute showWhen:
						var targetProperty = baseProperty.Navigate(1, '.' + showWhen.propertyName);
						showing = showing && Equals(targetProperty.Get<object>(), showWhen.value);
						break;
				}
			}
			return showing;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(!ShouldShow(property))
				return 0f;
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!ShouldShow(property))
				return;
			EditorGUI.PropertyField(position, property, label, true);
		}
	}
}
