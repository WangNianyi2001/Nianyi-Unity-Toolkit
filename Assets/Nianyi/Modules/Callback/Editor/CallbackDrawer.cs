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

		protected override void Draw(MemberAccessor member, GUIContent label) {
			Callback callback = member.Get<Callback>();
			if(callback == null) {
				if(DropdownButton(new GUIContent("Create Callback"), label)) {
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
				if(Button(new GUIContent("Clear"), label)) {
					if(Alert("Clearing a callback", "This operation is inreversible, proceed?", "Proceed", "Cancel")) {
						member.Set<Callback>(null);
						return;
					}
				}
			}

			switch(callback) {
				case LegacyCallback _:
					if(legacyDrawer == null)
						legacyDrawer = new UnityEventDrawer();
					DrawWith(legacyDrawer, member.Navigate(".unityEvent").Simplify(), new GUIContent("Legacy Callback"));
					return;
				case SimpleCallback _:
					if(simpleDrawer == null)
						simpleDrawer = new SimpleCallbackDrawer();
					DrawWith(simpleDrawer, member, new GUIContent("Simple Callback"));
					return;
				case ComposedCallback _:
					if(composedDrawer == null)
						composedDrawer = new ComposedCallbackDrawer();
					DrawWith(composedDrawer, member, new GUIContent("Composed Callback"));
					return;
			}
		}
	}
}