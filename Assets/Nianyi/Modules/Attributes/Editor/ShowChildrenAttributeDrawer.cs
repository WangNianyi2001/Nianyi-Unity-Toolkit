using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(ShowChildrenAttribute))]
	public class ShowChildrenAttributeDrawer : PropertyDrawer {
		List<float> heights;
		List<GUIContent> labels;
		SerializedObject so;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			heights = new List<float>();
			labels = new List<GUIContent>();
			UnityEngine.Object targetObject = property.objectReferenceValue;
			if(targetObject == null) {
				so = null;
				return EditorGUIUtility.singleLineHeight;
			}
			so = new SerializedObject(targetObject);

			float height = 0;
			var child = so.GetIterator();
			child.Next(true);
			for(; child.NextVisible(true);) {
				var childLabel = new GUIContent(child.displayName);
				labels.Add(childLabel);
				float childHeight = EditorGUI.GetPropertyHeight(child, childLabel);
				heights.Add(childHeight);
				height += childHeight;
				height += EditorGUIUtility.standardVerticalSpacing;
			}
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(so == null) {
				EditorGUI.LabelField(position, label, new GUIContent("Object is null"));
				return;
			}
			float top = 0;
			var child = so.GetIterator();
			child.Next(true);
			for(int i = 0; child.NextVisible(true); ++i) {
				float childHeight = heights[i];
				GUIContent childLabel = labels[i];
				Rect childPosition = new Rect(position) {
					yMin = top,
					height = childHeight
				};
				EditorGUI.PropertyField(childPosition, child, childLabel);
				top += childHeight;
				top += EditorGUIUtility.standardVerticalSpacing;
			}
		}
	}
}