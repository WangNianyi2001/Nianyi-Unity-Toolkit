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

		SimpleCallbackDrawer simpleDrawer;
		UnityEventDrawer legacyDrawer;

		protected override void Draw(MemberAccessor member, GUIContent label) {
			Callback callback = member.Get<Callback>();
			LabelField(label);
			if(callback == null) {
				if(DropdownButton(new GUIContent("Create Callback"))) {
					GenericMenu menu = new GenericMenu();
					foreach(Type type in types) {
						menu.AddItem(
							new GUIContent(type.Name),
							false,
							() => member.Set(ScriptableObject.CreateInstance(type))
						);
					}
					menu.ShowAsContext();
				}
				return;
			}
			else {
				if(Button(new GUIContent("Clear"))) {
					if(Alert("Clearing a callback", "This operation is inreversible, proceed?", "Proceed", "Cancel")) {
						member.Set<Callback>(null);
						return;
					}
				}
			}
			if(Button(new GUIContent("Invoke Manually"))) {
				callback.Invoke();
			}
			switch(callback) {
				case LegacyCallback _:
					if(legacyDrawer == null)
						legacyDrawer = new UnityEventDrawer();
					DrawWith(legacyDrawer, member.Navigate("unityEvent").Simplify(), new GUIContent("Legacy Callback"));
					return;
				case SimpleCallback _:
					if(simpleDrawer == null)
						simpleDrawer = new SimpleCallbackDrawer();
					DrawWith(simpleDrawer, member, new GUIContent("Simple Callback"));
					return;
				case ComposedCallback _:
					return;
			}
		}
	}
}