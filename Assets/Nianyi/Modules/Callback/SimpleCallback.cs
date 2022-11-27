using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	public class SimpleCallback : Callback {
		public UnityEngine.Object target = null;

		public bool isStatic = false;
		[SerializeField] string typeName = null;
		[SerializeField] string methodName = null;
		[SerializeField] string[] parameterTypes = new string[0];
		public List<SerializableParameter> parameters = new List<SerializableParameter>();

		public MethodInfo method {
			get {
				if(typeName == null || methodName == null)
					return null;
				Type type = ReflectionUtility.GetTypeByName(typeName);
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
					return method;
				}
				return null;
			}
			set {
				typeName = value?.DeclaringType.FullName;
				methodName = value?.Name;
				parameterTypes = value?.GetParameters().Select(p => p.ParameterType.Name).ToArray() ?? new string[0];
				isStatic = value?.IsStatic ?? false;
			}
		}
		static Type[] asynchronousTypes = new Type[] {
			typeof(IEnumerator),
			typeof(Coroutine),
			typeof(System.Threading.Tasks.Task),
			typeof(YieldInstruction),
		};
		public bool CanBeAsynchronous {
			get {
				if(typeName == null)
					return false;
				return asynchronousTypes.Contains(method.ReturnType);
			}
		}

		public override IEnumerator Invoke() {
			if(target == null || methodName == null)
				return null;
			var result = method.Invoke(
				isStatic ? null : target,
				parameters.Select(parameter => parameter.value).ToArray()
			);
			return asynchronous ? CoroutineHelper.Make(result) : null;
		}
	}
}