using UnityEditor;
using UnityEngine;
using System;
using DT = Nianyi.SerializableParameter.DrawerType;

namespace Nianyi.Editor {
	[CustomPropertyDrawer(typeof(SerializableParameter))]
	public class SerializableParameterDrawer : PropertyDrawerBase {
		SerializableParameter parameter;

		protected override void Draw(SerializedProperty property, GUIContent label) {
			parameter = property.objectReferenceValue as SerializableParameter;
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
				case DT.Color:
					parameter.value = ColorField((Color)parameter.value, label);
					break;
				case DT.UnityObject:
					parameter.value = ObjectField(parameter.value as UnityEngine.Object, label, parameter.type, true);
					break;
				case DT.LayerMask:
					parameter.value = LayerField((int)parameter.value, label);
					break;
				case DT.Enum:
					Enum enumValue = (Enum)Enum.Parse(parameter.type, Enum.GetName(parameter.type, parameter.value));
					parameter.value = EnumPopUp(enumValue, label);
					break;
				case DT.Vector2:
					parameter.value = Vector2Field((Vector2)parameter.value, label);
					break;
				case DT.Vector3:
					parameter.value = Vector3Field((Vector3)parameter.value, label);
					break;
				case DT.Vector4:
					parameter.value = Vector4Field((Vector4)parameter.value, label);
					break;
				case DT.Rect:
					parameter.value = RectField((Rect)parameter.value, label);
					break;
				case DT.Character:
					// TODO
					break;
				case DT.AnimationCurve:
					parameter.value = CurveField((AnimationCurve)parameter.value, label);
					break;
				case DT.Bounds:
					parameter.value = BoundsField((Bounds)parameter.value, label);
					break;
				case DT.Gradient:
					parameter.value = GradientField((Gradient)parameter.value, label);
					break;
				case DT.Quaternion:
					var euler = Vector3Field(((Quaternion)parameter.value).eulerAngles, label);
					parameter.value = Quaternion.Euler(euler);
					break;
				case DT.Vector2Int:
					parameter.value = Vector2IntField((Vector2Int)parameter.value, label);
					break;
				case DT.Vector3Int:
					parameter.value = Vector3IntField((Vector3Int)parameter.value, label);
					break;
				case DT.RectInt:
					parameter.value = RectIntField((RectInt)parameter.value, label);
					break;
				case DT.BoundsInt:
					parameter.value = BoundsIntField((BoundsInt)parameter.value, label);
					break;
				case DT.Generic:
					LabelField(new GUIContent($"{label} ({parameter.type.Name}) cannot be serialized."));
					// TODO
					break;
			}
		}
	}
}
