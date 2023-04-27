using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
		public List<SerializableParameter> parameters = new List<SerializableParameter>();

		MethodInfo _method;
		public MethodInfo method {
			get {
				if(typeName == null || methodName == null)
					return null;
				if(_method != null) {
					if(_method.Name == methodName)
						return _method;
					_method = null;
				}
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
					return _method = method;
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
				return asynchronousTypes.Contains(method?.ReturnType);
			}
		}
		public object Target {
			get {
				switch(invocationType) {
					case InvocationType.Instance:
						return target;
					case InvocationType.Static:
						return null;
					case InvocationType.Singleton:
						return SingletonAttribute.GetOn(method.DeclaringType);
				}
				return target;
			}
		}

		public override void InvokeSync() {
			if(target == null || methodName == null)
				return;
			method.Invoke(
				Target,
				parameters.Select(parameter => parameter.value).ToArray()
			);
		}

		public override IEnumerator InvokeAsync() {
			if(target == null || methodName == null)
				return null;
			var result = method.Invoke(
				Target,
				parameters.Select(parameter => parameter.value).ToArray()
			);
			return asynchronous ? CoroutineHelper.Make(result) : null;
		}
	}
}