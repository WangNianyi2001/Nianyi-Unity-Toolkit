using UnityEditor;
using UnityEngine;
using System;
using DT = Nianyi.SerializableParameter.LegacyDrawerType;

namespace Nianyi {
	[CustomPropertyDrawer(typeof(SerializableParameter))]
	public class SerializableParameterDrawer : PropertyDrawerBase {
		SerializableParameter parameter;

		protected override void Draw(SerializedProperty property, GUIContent label) {
			parameter = property.objectReferenceValue as SerializableParameter;
			switch(parameter.DrawerType) {
				case DT.Integer:
					parameter.Value = IntField((int)parameter.Value, label);
					break;
				case DT.Float:
					parameter.Value = FloatField((float)parameter.Value, label);
					break;
				case DT.String:
					parameter.Value = TextField((string)parameter.Value, label);
					break;
				case DT.Boolean:
					parameter.Value = Toggle((bool)parameter.Value, label);
					break;
				case DT.Color:
					parameter.Value = ColorField((Color)parameter.Value, label);
					break;
				case DT.UnityObject:
					parameter.Value = ObjectField(parameter.Value as UnityEngine.Object, label, parameter.Type, true);
					break;
				case DT.LayerMask:
					parameter.Value = LayerField((int)parameter.Value, label);
					break;
				case DT.Enum:
					Enum enumValue = (Enum)Enum.Parse(parameter.Type, Enum.GetName(parameter.Type, parameter.Value));
					parameter.Value = EnumPopUp(enumValue, label);
					break;
				case DT.Vector2:
					parameter.Value = Vector2Field((Vector2)parameter.Value, label);
					break;
				case DT.Vector3:
					parameter.Value = Vector3Field((Vector3)parameter.Value, label);
					break;
				case DT.Vector4:
					parameter.Value = Vector4Field((Vector4)parameter.Value, label);
					break;
				case DT.Rect:
					parameter.Value = RectField((Rect)parameter.Value, label);
					break;
				case DT.Character:
					// TODO
					break;
				case DT.AnimationCurve:
					parameter.Value = CurveField((AnimationCurve)parameter.Value, label);
					break;
				case DT.Bounds:
					parameter.Value = BoundsField((Bounds)parameter.Value, label);
					break;
				case DT.Gradient:
					parameter.Value = GradientField((Gradient)parameter.Value, label);
					break;
				case DT.Quaternion:
					var euler = Vector3Field(((Quaternion)parameter.Value).eulerAngles, label);
					parameter.Value = Quaternion.Euler(euler);
					break;
				case DT.Vector2Int:
					parameter.Value = Vector2IntField((Vector2Int)parameter.Value, label);
					break;
				case DT.Vector3Int:
					parameter.Value = Vector3IntField((Vector3Int)parameter.Value, label);
					break;
				case DT.RectInt:
					parameter.Value = RectIntField((RectInt)parameter.Value, label);
					break;
				case DT.BoundsInt:
					parameter.Value = BoundsIntField((BoundsInt)parameter.Value, label);
					break;
				case DT.Generic:
					LabelField(new GUIContent($"{label} ({parameter.Type.Name}) cannot be serialized."));
					// TODO
					break;
			}
		}
	}
}
