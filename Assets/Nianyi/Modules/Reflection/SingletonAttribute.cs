using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Nianyi {
	public class SingletonAttribute : Attribute {
		public string memberName;

		public SingletonAttribute(string memberName) {
			this.memberName = memberName;
		}

		public static bool HasOn(Type type) {
			return type.CustomAttributes.Any(
				attribute => attribute.AttributeType == typeof(SingletonAttribute)
			);
		}

		public static object GetOn(Type type) {
			var attribute = type.CustomAttributes.FirstOrDefault(
				attribute => attribute.AttributeType == typeof(SingletonAttribute)
			);
			if(attribute == null)
				throw new CustomAttributeFormatException($"Type {type.Name} is not marked as a singleton.");
			var arguments = attribute.ConstructorArguments;
			var memberName = arguments[0].Value as string;
			var field = type.GetField(memberName);
			if(field != null)
				return field.GetValue(null);
			var property = type.GetProperty(memberName);
			if(property != null)
				return property.GetValue(null);
			throw new MissingReferenceException($"Cannot find singleton member as marked in type {type.Name}.");
		}
	}
}
