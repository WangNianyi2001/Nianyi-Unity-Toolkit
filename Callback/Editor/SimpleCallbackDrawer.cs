using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Nianyi {
	[CustomPropertyDrawer(typeof(SimpleCallback), true)]
	public class SimpleCallbackDrawer : PropertyDrawerBase {
		readonly List<SerializableParameterDrawer> parameterDrawers = new();
		bool parametersExpanded = true;
		SimpleCallback simple;
		GenericMenu menu;

		void AddMethodsToMenu(
			UnityEngine.Object target,
			IEnumerable<MethodInfo> methods,
			SimpleCallback.InvocationType invocationType,
			string path = ""
		) {
			foreach(var method in methods) {
				menu.AddItem(
					new GUIContent(path + Reflection.MethodSignature(method)),
					simple.Method == method,
					() => {
						simple.target = target;
						simple.Method = method;
						simple.invocationType = invocationType;
						EditorUtility.SetDirty(simple);
					}
				);
			}
		}

		protected override void Draw(SerializedProperty property, GUIContent label) {
			simple = property.objectReferenceValue as SimpleCallback;
			if(simple == null)
				new MemberAccessor(property).Set(simple = ScriptableObject.CreateInstance<SimpleCallback>());
			
			// Target slot
			simple.target = ObjectField(simple.target, new GUIContent("Target"), typeof(UnityEngine.Object), true);
			MakeSpacing();

			// Asynchronicity toggle
			if(!simple.Asynchronousable)
				simple.asynchronous = false;
			else {
				simple.asynchronous = Toggle(simple.asynchronous, new GUIContent("Asynchronous"));
				MakeSpacing();
			}

			// Method choice button
			if(simple.target == null) {
				simple.Method = null;
			}
			else {
				string buttonText = simple.Method == null ? "(None)" : simple.Method.Name;
				if(DropdownButton(new GUIContent(buttonText), new GUIContent("Method"))) {
					var target = simple.target;
					if(target is Component)
						target = (target as Component).gameObject;
					menu = new GenericMenu();
					switch(target) {
						case MonoScript script:
							var type = script.GetClass();
							if(!SingletonAttribute.HasOn(type)) {
								AddMethodsToMenu(
									script,
									Reflection.GetInspectableStaticMethods(type),
									SimpleCallback.InvocationType.Static
								);
							}
							else {
								AddMethodsToMenu(
									script,
									Reflection.GetInspectableStaticMethods(type),
									SimpleCallback.InvocationType.Static,
									"MonoScript Static/"
								);
								AddMethodsToMenu(
									script,
									Reflection.GetInspectableInstanceMethods(type),
									SimpleCallback.InvocationType.Singleton,
									"Singleton/"
								);
							}
							break;
						case GameObject gameObject:
							AddMethodsToMenu(
								gameObject,
								Reflection.GetInspectableInstanceMethods(typeof(GameObject)),
								SimpleCallback.InvocationType.Instance,
								"GameObject/"
							);
							foreach(var component in gameObject.GetComponents<Component>()) {
								Type componentType = component.GetType();
								AddMethodsToMenu(
									component,
									Reflection.GetInspectableInstanceMethods(componentType),
									SimpleCallback.InvocationType.Instance,
									$"{componentType.Name}/"
								);
							}
							break;
						default:
							AddMethodsToMenu(
								target,
								Reflection.GetInspectableInstanceMethods(target.GetType()),
								SimpleCallback.InvocationType.Instance
							);
							break;
					}
					menu.ShowAsContext();
				}
				MakeSpacing();
			}

			// Parameter list
			simple.parameters ??= new List<SerializableParameter>();
			if(simple.Method != null) {
				ParameterInfo[] parameterInfos = simple.Method.GetParameters();
				var parameters = simple.parameters;
				parameters.SetLength(parameterInfos.Length);
				parameterDrawers.SetLength(parameterInfos.Length);
				for(int i = 0; i < parameterInfos.Length; ++i) {
					ParameterInfo parameterInfo = parameterInfos[i];
					if(parameters[i] == null || !parameterInfo.ParameterType.IsAssignableFrom(parameters[i].Type)) {
						var parameter = parameters[i] = ScriptableObject.CreateInstance<SerializableParameter>();
						parameter.Type = parameterInfo.ParameterType;
					}
					if(parameterDrawers[i] == null)
						parameterDrawers[i] = new SerializableParameterDrawer();
				}
				if(simple.Method.GetParameters().Length > 0) {
					parametersExpanded = Foldout(parametersExpanded, new GUIContent("Parameters"));
					if(parametersExpanded) {
						++EditorGUI.indentLevel;
						for(int i = 0; i < parameterInfos.Length; ++i) {
							DrawWith(
								parameterDrawers[i],
								new SerializedObject(simple).FindProperty("parameters").GetArrayElementAtIndex(i),
								new GUIContent(parameterInfos[i].Name)
							);
							MakeSpacing();
						}
						--EditorGUI.indentLevel;
					}
				}
			}
		}
	}
}
