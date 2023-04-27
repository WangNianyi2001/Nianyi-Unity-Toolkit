using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(ComposedCallback))]
	public class ComposedCallbackDrawer : PropertyDrawerBase {
		ComposedCallback composed;

		ReorderableList reorderableList;
		List<CallbackDrawer> callbackDrawers = new List<CallbackDrawer>();
		List<GUIContent> labels = new List<GUIContent>();
		GUIContent label;

		void Update(int i) {
			if(i >= labels.Count)
				labels.SetLength(i + 1);
			labels[i] = new GUIContent($"#{i}");
			if(i >= callbackDrawers.Count)
				callbackDrawers.SetLength(i + 1);
			if(callbackDrawers[i] == null)
				callbackDrawers[i] = new CallbackDrawer();
		}
		void UpdateAll() {
			labels.SetLength(reorderableList.count);
			callbackDrawers.SetLength(reorderableList.count);
			for(int i = 0; i < composed.sequence.Count; ++i)
				Update(i);
		}

		protected void DrawNull(SerializedProperty property, GUIContent label) {
			var accessor = new MemberAccessor(property);
			var value = ScriptableObject.CreateInstance<ComposedCallback>();
			accessor.Set(value);
			composed = accessor.Get<ComposedCallback>();
		}

		protected override void Draw(SerializedProperty property, GUIContent label) {
			composed = property.objectReferenceValue as ComposedCallback;
			if(composed == null) {
				DrawNull(property, label);
				return;
			}

			this.label = label;

			composed.asynchronous = Toggle(composed.asynchronous, new GUIContent("Asynchronous"));
			MakeSpacing();

			if(reorderableList == null) {
				reorderableList = new ReorderableList(composed.sequence, typeof(Callback));

				reorderableList.onAddCallback = list => {
					list.list.Add(null);
					Update(composed.sequence.Count - 1);
				};
				reorderableList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) => {
					Update(oldIndex);
					Update(newIndex);
				};
				reorderableList.onRemoveCallback = list => {
					if(list.list.Count == 0)
						return;
					var indices = list.selectedIndices.ToList();
					if(indices.Count == 0)
						indices.Add(list.list.Count - 1);
					indices.Sort();
					indices.Reverse();
					foreach(int index in indices)
						list.list.RemoveAt(index);
					UpdateAll();
				};

				reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, this.label, EditorStyles.label);
				reorderableList.elementHeightCallback = (int i) => {
					Update(i);
					var so = new SerializedObject(composed);
					SerializedProperty sp = so.FindProperty("sequence").GetArrayElementAtIndex(i);
					return callbackDrawers[i].GetPropertyHeight(sp, labels[i]);
				};
				reorderableList.drawElementCallback = (Rect position, int i, bool isActive, bool isFocused) => {
					Update(i);
					var so = new SerializedObject(composed);
					SerializedProperty sp = so.FindProperty("sequence").GetArrayElementAtIndex(i);
					callbackDrawers[i].OnGUI(position, sp, labels[i]);
				};

				UpdateAll();
			}

			MakeHeight(reorderableList.GetHeight());
			if(Rendering)
				reorderableList.DoList(Next);
		}
	}
}
