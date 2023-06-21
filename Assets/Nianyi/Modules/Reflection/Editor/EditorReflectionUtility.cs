using System;
using System.Reflection;

namespace Nianyi {
	public static class EditorReflectionUtility {
		public static Type FindTypeOfName(string name) {
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach(var type in assembly.GetTypes()) {
					if(type.Name == name)
						return type;
				}
			}
			return null;
		}

		static MethodInfo GetDrawerTypeForType;
		public static Type DrawerTypeOf(Type type) {
			if(GetDrawerTypeForType == null) {
				GetDrawerTypeForType = FindTypeOfName("ScriptAttributeUtility")
					.GetMethod("GetDrawerTypeForType", BindingFlags.Static | BindingFlags.NonPublic);
			}
			return GetDrawerTypeForType.Invoke(null, new object[] { type }) as Type;
		}
	}
}