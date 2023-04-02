using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(SimpleCallback), true)]
	public class SimpleCallbackDrawer : PropertyDrawerBase {
		List<SerializableParameterDrawer> parameterDrawers = new List<SerializableParameterDrawer>();
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
					new GUIContent(path + ReflectionUtility.MethodSignature(method)),
					simple.method == method,
					() => {
						simple.target = target;
						simple.method = method;
						simple.invocationType = invocationType;
						EditorUtility.SetDirty(simple);
					}
				);
			}
		}

		protected override void Draw(SerializedProperty property, GUIContent label) {
			simple = property.objectReferenceValue as SimpleCallback;
			if(simple == null)
				new MemberAccessor(property).Set(simple = new SimpleCallback());
			
			// Target slot
			simple.target = ObjectField(simple.target, new GUIContent("Target"), typeof(UnityEngine.Object), true);
			MakeSpacing();

			// Asynchronicity toggle
			if(!simple.CanBeAsynchronous)
				simple.asynchronous = false;
			else {
				simple.asynchronous = Toggle(simple.asynchronous, new GUIContent("Asynchronous"));
				MakeSpacing();
			}

			// Method choice button
			if(simple.target == null) {
				simple.method = null;
			}
			else {
				string buttonText = simple.method == null ? "(None)" : simple.method.Name;
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
									ReflectionUtility.GetInspectableStaticMethods(type),
									SimpleCallback.InvocationType.Static
								);
							}
							else {
								AddMethodsToMenu(
									script,
									ReflectionUtility.GetInspectableStaticMethods(type),
									SimpleCallback.InvocationType.Static,
									"MonoScript Static/"
								);
								AddMethodsToMenu(
									script,
									ReflectionUtility.GetInspectableInstanceMethods(type),
									SimpleCallback.InvocationType.Singleton,
									"Singleton/"
								);
							}
							break;
						case GameObject gameObject:
							AddMethodsToMenu(
								gameObject,
								ReflectionUtility.GetInspectableInstanceMethods(typeof(GameObject)),
								SimpleCallback.InvocationType.Instance,
								"GameObject/"
							);
							foreach(var component in gameObject.GetComponents<Component>()) {
								Type componentType = component.GetType();
								AddMethodsToMenu(
									component,
									ReflectionUtility.GetInspectableInstanceMethods(componentType),
									SimpleCallback.InvocationType.Instance,
									$"{componentType.Name}/"
								);
							}
							break;
						default:
							AddMethodsToMenu(
								target,
								ReflectionUtility.GetInspectableInstanceMethods(target.GetType()),
								SimpleCallback.InvocationType.Instance
							);
							break;
					}
					menu.ShowAsContext();
				}
				MakeSpacing();
			}

			// Parameter list
			if(simple.parameters == null)
				simple.parameters = new List<SerializableParameter>();
			if(simple.method != null) {
				ParameterInfo[] parameterInfos = simple.method.GetParameters();
				var parameters = simple.parameters;
				parameters.SetLength(parameterInfos.Length);
				parameterDrawers.SetLength(parameterInfos.Length);
				for(int i = 0; i < parameterInfos.Length; ++i) {
					ParameterInfo parameterInfo = parameterInfos[i];
					if(parameters[i] == null || !parameterInfo.ParameterType.IsAssignableFrom(parameters[i].type)) {
						var parameter = parameters[i] = ScriptableObject.CreateInstance<SerializableParameter>();
						parameter.type = parameterInfo.ParameterType;
					}
					if(parameterDrawers[i] == null)
						parameterDrawers[i] = new SerializableParameterDrawer();
				}
				if(simple.method.GetParameters().Length > 0) {
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
