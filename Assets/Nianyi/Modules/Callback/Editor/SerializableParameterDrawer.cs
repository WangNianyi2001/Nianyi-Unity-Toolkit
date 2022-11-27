using UnityEditor;
using UnityEngine;
using DT = Nianyi.SerializableParameter.DrawerType;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(SerializableParameter))]
	public class SerializableParameterDrawer : PropertyDrawerBase {
		SerializableParameter parameter;
		protected override void Draw(MemberAccessor member, GUIContent label) {
			parameter = member.Get<SerializableParameter>();
			switch(parameter.drawerType) {
				case DT.Integer:
					parameter.value = IntField((int)parameter.value, label);
					break;
				case DT.Float:
					parameter.value = FloatField((float)parameter.value, label);
					break;
				case DT.String:
					parameter.value = TextField((string)parameter.value, label);
					break;
				case DT.Boolean:
					parameter.value = Toggle((bool)parameter.value, label);
					break;
				case DT.UnityObject:
					parameter.value = ObjectField(parameter.value as Object, label, parameter.type, true);
					break;
				// TODO
				default:
					LabelField(new GUIContent($"{label} ({parameter.type.Name}) ({parameter.drawerType})"));
					break;
			}
		}
	}
}
