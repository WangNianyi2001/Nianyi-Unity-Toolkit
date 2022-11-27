using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(ComposedCallback))]
	public class ComposedCallbackDrawer : PropertyDrawerBase {
		ComposedCallback composed;

		ReorderableList reorderableList;
		List<CallbackDrawer> callbackDrawers = new List<CallbackDrawer>();
		List<GUIContent> labels = new List<GUIContent>();
		GUIContent label;

		protected override void Draw(MemberAccessor member, GUIContent label) {
			composed = member.Get<ComposedCallback>();
			if(composed == null)
				member.Set(composed = new ComposedCallback());
			this.label = label;

			composed.asynchrnous = Toggle(composed.asynchrnous, new GUIContent("Asynchronous"));

			if(reorderableList == null) {
				reorderableList = new ReorderableList(composed.sequence, typeof(Callback));
				reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, this.label);
				reorderableList.onAddCallback = (ReorderableList list) => composed.sequence.Add(null);
				reorderableList.elementHeightCallback = (int index) => {
					callbackDrawers.SetLength(composed.sequence.Count);
					labels.SetLength(composed.sequence.Count);
					for(int i = 0; i < callbackDrawers.Count; ++i) {
						if(callbackDrawers[i] == null)
							callbackDrawers[i] = new CallbackDrawer();
					}
					var so = new SerializedObject(composed);
					SerializedProperty sp = so.FindProperty("sequence").GetArrayElementAtIndex(index);
					labels[index] = new GUIContent($"Sub-callback {index}");
					return callbackDrawers[index].GetPropertyHeight(sp, labels[index]);
				};
				reorderableList.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) => {
					var so = new SerializedObject(composed);
					SerializedProperty sp = so.FindProperty("sequence").GetArrayElementAtIndex(index);
					callbackDrawers[index].OnGUI(position, sp, labels[index]);
				};
			}

			MakeHeight(reorderableList.GetHeight());
			if(Rendering)
				reorderableList.DoList(Next);
		}
	}
}
