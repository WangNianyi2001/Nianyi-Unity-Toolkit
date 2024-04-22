using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Callback/Simple")]
	public class SimpleCallback : Callback {
		public enum InvocationType {
			Instance, Static, Singleton
		}

		public UnityEngine.Object target = null;
		public InvocationType invocationType = InvocationType.Instance;
		[SerializeField] string typeName = null;
		[SerializeField] string methodName = null;
		[SerializeField] string[] parameterTypes = new string[0];
		public List<SerializableParameter> parameters = new();

		MethodInfo method;
		public MethodInfo Method {
			get {
				if(typeName == null || methodName == null)
					return null;
				if(method != null) {
					if(method.Name == methodName)
						return method;
					method = null;
				}
				Type type = Reflection.GetTypeByName(typeName);
				if(type == null)
					return null;
				foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
					if(method.Name != methodName)
						continue;
					var parameters = method.GetParameters();
					if(parameters.Length != parameterTypes.Length)
						continue;
					bool badMatch = false;
					for(int i = 0; i < parameters.Length; ++i) {
						if(parameters[i].ParameterType.Name != parameterTypes[i]) {
							badMatch = true;
							break;
						}
					}
					if(badMatch)
						continue;
					return this.method = method;
				}
				return null;
			}
			set {
				typeName = value?.DeclaringType.FullName;
				methodName = value?.Name;
				parameterTypes = value?.GetParameters().Select(p => p.ParameterType.Name).ToArray() ?? new string[0];
				invocationType = InvocationType.Instance;
				if(value != null) {
					if(value.IsStatic)
						invocationType = InvocationType.Static;
					else
						invocationType = target ? InvocationType.Instance : InvocationType.Singleton;
				}
			}
		}
		static readonly Type[] asynchronousTypes = new Type[] {
			typeof(IEnumerator),
			typeof(Coroutine),
			typeof(System.Threading.Tasks.Task),
			typeof(YieldInstruction),
		};
		public object Target {
			get {
				return invocationType switch {
					InvocationType.Instance => target,
					InvocationType.Static => null,
					InvocationType.Singleton => SingletonAttribute.GetOn(Method.DeclaringType),
					_ => target,
				};
			}
		}

		public override bool Asynchronousable => typeName != null && asynchronousTypes.Contains(Method?.ReturnType);

		public override void InvokeSync() {
			if(target == null || methodName == null)
				return;
			Method.Invoke(
				Target,
				parameters.Select(parameter => parameter.Value).ToArray()
			);
		}

		public override Coroutine InvokeAsync() {
			if(target == null || methodName == null)
				return null;
			var result = Method.Invoke(
				Target,
				parameters.Select(parameter => parameter.Value).ToArray()
			);
			return asynchronous ? CoroutineHelper.Run(CoroutineHelper.Make(result)) : null;
		}
	}
}