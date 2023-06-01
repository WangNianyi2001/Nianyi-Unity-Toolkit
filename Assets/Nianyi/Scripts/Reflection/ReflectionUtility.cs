using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Nianyi {
	public static class ReflectionUtility {
		public const BindingFlags
			bindingFlagsDontCare = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance,
			bindingFlagsInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		static Type[] specialUnserializableTypes = {
			typeof(Type), typeof(object)
		};
		public static bool Serializable(Type type) {
			if(type.IsSubclassOf(typeof(UnityEngine.Object)) || type == typeof(UnityEngine.Object))
				return true;
			if(type.IsEnum)
				return true;
			if(type.GetCustomAttributes<SerializableAttribute>().Count() != 0) {
				if(specialUnserializableTypes.Contains(type))
					return false;
				return true;
			}
			if(type == typeof(Vector2) || type == typeof(Vector3))
				return true;
			return false;
		}
		public static bool FilterNonSerializableMethod(MethodInfo method) {
			// Reject generic methods
			if(method.ContainsGenericParameters)
				return false;
			ParameterInfo[] parameterInfos = method.GetParameters();
			// Reject if any parameter is non-serializable
			if(!parameterInfos.All(parameterInfo => Serializable(parameterInfo.ParameterType)))
				return false;
			return true;
		}

		public static object DefaultValue(Type type)
			=> type.IsValueType ? Activator.CreateInstance(type) : null;

		public static IEnumerable<MethodInfo> GetInspectableInstanceMethods(Type type)
			=> type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(FilterNonSerializableMethod);
		public static IEnumerable<MethodInfo> GetInspectableStaticMethods(Type type)
			=> type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(FilterNonSerializableMethod);

		public static string MethodSignature(MethodInfo method) {
			var methodNames = method.GetParameters().Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}");
			return $"{method.Name}({string.Join(", ", methodNames)})";
		}

		public static Type GetTypeByName(string name) {
			if(string.IsNullOrEmpty(name))
				return null;
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse()) {
				var type = assembly.GetType(name);
				if(type != null)
					return type;
			}
			return null;
		}

		public static byte[] StructToBytes<T>(T value) where T : struct
			=> StructToBytes(typeof(T), value);
		public static byte[] StructToBytes(Type type, object value) {
			if(value == null)
				return new byte[0];
			if(type.IsEnum)
				type = typeof(int);
			int size = Marshal.SizeOf(type);
			byte[] bytes = new byte[size];
			IntPtr ptr = IntPtr.Zero;
			try {
				ptr = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr(value, ptr, true);
				Marshal.Copy(ptr, bytes, 0, size);
			}
			finally {
				Marshal.FreeHGlobal(ptr);
			}
			return bytes;
		}
		public static T BytesToStruct<T>(byte[] bytes) where T : struct
			=> (T)BytesToStruct(typeof(T), bytes);
		public static object BytesToStruct(Type type, byte[] bytes) {
			if(bytes == null)
				return DefaultValue(type);
			if(type.IsEnum)
				type = typeof(int);
			object obj = null;
			int size = Marshal.SizeOf(type);
			if(size > bytes.Length)
				return DefaultValue(type);
			IntPtr ptr = IntPtr.Zero;
			try {
				ptr = Marshal.AllocHGlobal(size);
				Marshal.Copy(bytes, 0, ptr, size);
				obj = Marshal.PtrToStructure(ptr, type);
			}
			finally {
				Marshal.FreeHGlobal(ptr);
			}
			return obj;
		}

		public static void Call(this object target, string name, params object[] parameters) {
			if(target == null)
				throw new NullReferenceException($"Cannot call function \"{name}\" on null");
			var type = target.GetType();
			var method = type.GetMethod(name, bindingFlagsInstance);
			if(method == null)
				throw new MissingMethodException($"Object has no method named \"{name}\"");
			method.Invoke(target, parameters);
		}
	}
}
