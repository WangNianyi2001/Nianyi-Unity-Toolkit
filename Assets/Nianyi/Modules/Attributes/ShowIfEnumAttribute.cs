using UnityEngine;
using System.Linq;

namespace Nianyi {
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
	public class ShowIfEnumAttribute : PropertyAttribute {
		public readonly string propertyName;
		public readonly System.Enum[] expectedEnumValues;

		public ShowIfEnumAttribute(string propertyName, params object[] expectedEnumValues) {
			this.propertyName = propertyName;
			this.expectedEnumValues = expectedEnumValues
				.Cast<System.Enum>()
				.Where(value => value != null)
				.ToArray();
		}
	}
}
