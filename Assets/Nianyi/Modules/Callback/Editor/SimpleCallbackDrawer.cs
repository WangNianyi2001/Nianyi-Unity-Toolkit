using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(SimpleCallback), true)]
	public class SimpleCallbackDrawer : PropertyDrawerBase {
		List<SerializableParameterDrawer> parameterDrawers = new List<SerializableParameterDrawer>();

		static void SetLength<T>(List<T> list, int count) {
			if(list.Count != count) {
				if(list.Count > count)
					list.RemoveRange(count, list.Count - count);
				else for(int i = list.Count; i < count; ++i)
						list.Add(default);
			}
		}

		protected override void Draw(MemberAccessor member, GUIContent label) {
			var simple = member.Get<SimpleCallback>();
			if(simple == null)
				member.Set(simple = new SimpleCallback());

			// Target slot
			simple.target = ObjectField(simple.target, new GUIContent("Target"), typeof(UnityEngine.Object), true);

			// Asynchronicity toggle
			simple.asynchrnous = Toggle(simple.asynchrnous, new GUIContent("Asynchronous"));

			// Method choice button
			if(simple.target == null) {
				simple.Method = null;
			}
			else {
				LabelField(new GUIContent("Method"));
				// Reset method if target has changed
				if(simple.Method != null) {
					if(!simple.Method.DeclaringType.IsAssignableFrom(simple.target.GetType())) {
						simple.Method = null;
						EditorUtility.SetDirty(simple);
					}
				}
				string buttonText = simple.Method == null ? "(None)" : simple.Method.Name;
				if(DropdownButton(new GUIContent(buttonText))) {
					var target = simple.target;
					if(target is Component)
						target = (target as Component).gameObject;
					var menu = new GenericMenu();
					switch(target) {
						case MonoScript script:
							var underlyingClass = script.GetClass();
							foreach(var method in ReflectionUtility.GetInspectableStaticMethods(underlyingClass)) {
								menu.AddItem(
									new GUIContent(ReflectionUtility.MethodSignature(method)),
									method == simple.Method,
									() => {
										simple.Method = method;
										EditorUtility.SetDirty(simple);
									}
								);
							}
							break;
						case GameObject gameObject:
							foreach(var method in ReflectionUtility.GetInspectableInstanceMethods(typeof(GameObject))) {
								menu.AddItem(
									new GUIContent($"GameObject/{ReflectionUtility.MethodSignature(method)}"),
									method == simple.Method,
									() => {
										simple.target = gameObject;
										simple.Method = method;
										EditorUtility.SetDirty(simple);
									}
								);
							}
							foreach(var component in gameObject.GetComponents<Component>()) {
								Type componentType = component.GetType();
								foreach(var method in ReflectionUtility.GetInspectableInstanceMethods(componentType)) {
									menu.AddItem(
										new GUIContent($"{componentType.Name}/{ReflectionUtility.MethodSignature(method)}"),
										method == simple.Method,
										() => {
											simple.target = component;
											simple.Method = method;
											EditorUtility.SetDirty(simple);
										}
									);
								}
							}
							break;
						default:
							foreach(var method in ReflectionUtility.GetInspectableInstanceMethods(simple.target.GetType())) {
								menu.AddItem(
									new GUIContent(ReflectionUtility.MethodSignature(method)),
									method == simple.Method,
									() => {
										simple.Method = method;
										EditorUtility.SetDirty(simple);
									}
								);
							}
							break;
					}
					menu.ShowAsContext();
				}
			}

			// Parameter list
			if(simple.parameters == null)
				simple.parameters = new List<SerializableParameter>();
			if(simple.Method != null) {
				ParameterInfo[] parameterInfos = simple.Method.GetParameters();
				var parameters = simple.parameters;
				SetLength(parameters, parameterInfos.Length);
				SetLength(parameterDrawers, parameterInfos.Length);
				for(int i = 0; i < parameterInfos.Length; ++i) {
					ParameterInfo parameterInfo = parameterInfos[i];
					if(parameters[i] == null || !parameterInfo.ParameterType.IsAssignableFrom(parameters[i].type)) {
						var parameter = parameters[i] = ScriptableObject.CreateInstance<SerializableParameter>();
						parameter.type = parameterInfo.ParameterType;
					}
					if(parameterDrawers[i] == null)
						parameterDrawers[i] = new SerializableParameterDrawer();
				}
				if(simple.Method.GetParameters().Length > 0) {
					LabelField(new GUIContent("Parameters"));
					for(int i = 0; i < parameterInfos.Length; ++i) {
						DrawWith(
							parameterDrawers[i],
							new SerializedObject(simple).FindProperty("parameters").GetArrayElementAtIndex(i),
							new GUIContent(parameterInfos[i].Name)
						);
					}
				}
			}
		}
	}
}
