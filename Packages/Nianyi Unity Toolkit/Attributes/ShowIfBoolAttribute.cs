using UnityEngine;

namespace Nianyi {
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
	public class ShowIfBoolAttribute : PropertyAttribute {
		public readonly string propertyName;
		public readonly bool expected;

		public ShowIfBoolAttribute(string propertyName, bool expected = true) {
			this.propertyName = propertyName;
			this.expected = expected;
		}
	}
}