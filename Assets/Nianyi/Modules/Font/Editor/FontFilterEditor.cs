using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi.Editor {
	[CustomEditor(typeof(FontFilter))]
	public class FontFilterEditor : UnityEditor.Editor {
		static string[] customProperties = new string[] { "fontSet" };

		public override void OnInspectorGUI() {
			var defaultProperties = new List<SerializedProperty>();
			var it = serializedObject.GetIterator();
			for(it.Next(true); it.NextVisible(false);)
				defaultProperties.Add(it.Copy());
			defaultProperties = defaultProperties.Where(property => !customProperties.Contains(property.name)).ToList();

			EditorGUI.BeginChangeCheck();

			foreach(var property in defaultProperties)
				EditorGUILayout.PropertyField(property);

			var fontFilter = target as FontFilter;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Font Set");
			if(fontFilter.fontMap != null) {
				if(EditorGUILayout.DropdownButton(
					new GUIContent(fontFilter.fontSet != null ? fontFilter.fontSet.name : "None"),
					FocusType.Keyboard
					)) {
					var menu = new GenericMenu();
					foreach(var fontSet in fontFilter.fontMap.fontsets) {
						menu.AddItem(
							new GUIContent(fontSet.name),
							fontSet == fontFilter.fontSet,
							() => {
								fontFilter.fontSet = fontSet;
								UnityEditor.EditorUtility.SetDirty(fontFilter);
								EditorApplication.update();
							}
						);
					}
					menu.ShowAsContext();
				}
			}
			else {
				EditorGUILayout.LabelField("No font map selected");
			}
			EditorGUILayout.EndHorizontal();

			if(EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
				EditorApplication.update();
			}
		}
	}
}