using UnityEngine;

namespace Nianyi {
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
	public class ShowIfEnumAttribute : PropertyAttribute {
		public readonly string propertyName;
		public readonly string expected;

		public ShowIfEnumAttribute(string propertyName, string expected) {
			this.propertyName = propertyName;
			this.expected = expected;
		}
	}
}
