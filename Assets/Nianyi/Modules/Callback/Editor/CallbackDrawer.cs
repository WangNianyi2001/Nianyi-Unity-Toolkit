using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(Callback), true)]
	public class CallbackDrawer : PropertyDrawerBase {
		static Type[] types = new Type[] {
			typeof(LegacyCallback),
			typeof(SimpleCallback),
			typeof(ComposedCallback),
		};

		UnityEventDrawer legacyDrawer;
		SimpleCallbackDrawer simpleDrawer;
		ComposedCallbackDrawer composedDrawer;

		protected void DrawNull(SerializedProperty property, GUIContent label) {
			var accessor = new MemberAccessor(property);
			if(DropdownButton(new GUIContent("Create Callback"), label)) {
				GenericMenu menu = new GenericMenu();
				foreach(Type type in types) {
					menu.AddItem(
						new GUIContent(type.Name),
						false,
						() => accessor.Set(ScriptableObject.CreateInstance(type))
					);
				}
				menu.ShowAsContext();
			}
			var asset = ObjectField(null, new GUIContent("Or choose an existing asset"), typeof(Callback), false) as Callback;
			if(asset != null)
				accessor.Set(asset);
			return;
		}

		protected override void Draw(SerializedProperty property, GUIContent label) {
			Callback callback = property.objectReferenceValue as Callback;
			if(callback == null) {
				DrawNull(property, label);
				return;
			}

			var accessor = new MemberAccessor(property);

			if(label == null) {
				label = new GUIContent(callback?.GetType().Name ?? "null");
			}
			else {
				label = new GUIContent(label);
				label.text += $" ({callback?.GetType().Name ?? "null"})";
			}

			if(Button(new GUIContent("Clear"), label)) {
				if(Alert("Clearing a callback", "This operation is irreversible, proceed?", "Proceed", "Cancel")) {
					accessor.Set<Callback>(null);
					return;
				}
			}
			MakeSpacing();

			switch(callback) {
				case LegacyCallback _:
					if(legacyDrawer == null)
						legacyDrawer = new UnityEventDrawer();
					DrawWith(
						legacyDrawer,
						accessor.Navigate(".unityEvent").Simplify(),
						new GUIContent("Legacy Callback")
					);
					return;
				case SimpleCallback _:
					if(simpleDrawer == null)
						simpleDrawer = new SimpleCallbackDrawer();
					DrawWith(simpleDrawer, property, new GUIContent("Simple Callback"));
					return;
				case ComposedCallback _:
					if(composedDrawer == null)
						composedDrawer = new ComposedCallbackDrawer();
					DrawWith(composedDrawer, property, new GUIContent("Composed Callback"));
					return;
			}
		}
	}
}