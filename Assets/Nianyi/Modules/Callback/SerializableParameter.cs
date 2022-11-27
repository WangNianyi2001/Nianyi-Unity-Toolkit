using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi {
	public class SerializableParameter : ScriptableObject {
		public enum DrawerType {
			Integer,
			Boolean,
			Float,
			String,
			Color,
			UnityObject,
			LayerMask,
			Enum,
			Vector2,
			Vector3,
			Vector4,
			Rect,
			Character,
			AnimationCurve,
			Bounds,
			Gradient,
			Quaternion,
			Vector2Int,
			Vector3Int,
			RectInt,
			BoundsInt,
			Generic,
		}
		public static Dictionary<Type, DrawerType> drawerTypeMap = new Dictionary<Type, DrawerType> {
			{ typeof(int), DrawerType.Integer },
			{ typeof(long), DrawerType.Integer },
			{ typeof(float), DrawerType.Float },
			{ typeof(double), DrawerType.Float },
			{ typeof(string), DrawerType.String },
			{ typeof(bool), DrawerType.Boolean },
			{ typeof(Color), DrawerType.Color },
			{ typeof(UnityEngine.Object), DrawerType.UnityObject },
			{ typeof(LayerMask), DrawerType.LayerMask },
			{ typeof(Enum), DrawerType.Enum },
			{ typeof(Vector2), DrawerType.Vector2 },
			{ typeof(Vector3), DrawerType.Vector3 },
			{ typeof(Vector4), DrawerType.Vector4 },
			{ typeof(Rect), DrawerType.Rect },
			{ typeof(char), DrawerType.Character },
			{ typeof(AnimationCurve), DrawerType.AnimationCurve },
			{ typeof(Bounds), DrawerType.Bounds },
			{ typeof(Gradient), DrawerType.Gradient },
			{ typeof(Quaternion), DrawerType.Quaternion },
			{ typeof(Vector2Int), DrawerType.Vector2Int },
			{ typeof(Vector3Int), DrawerType.Vector3Int },
			{ typeof(RectInt), DrawerType.Rect },
			{ typeof(BoundsInt), DrawerType.BoundsInt },
		};
		public static DrawerType GetDrawerTypeOfType(Type type) {
			foreach(var key in drawerTypeMap.Keys) {
				if(key.IsAssignableFrom(type))
					return drawerTypeMap[key];
			}
			return DrawerType.Generic;
		}

		/* Core fields */
		[SerializeField] string typeName;
		Type _type;
		[SerializeField] DrawerType _drawerType;
		[SerializeField] byte[] bytes;
		[SerializeField] string stringValue;

		/* Properties */
		public object value {
			get {
				if(type.IsValueType) {
					return ReflectionUtility.BytesToStruct(type, bytes);
				}
				if(type == typeof(string)) {
					return stringValue;
				}
				// TODO
				return null;
			}
			set {
				if(type.IsValueType) {
					bytes = ReflectionUtility.StructToBytes(type, value);
					return;
				}
				if(type == typeof(string)) {
					stringValue = value as string;
					return;
				}
				// TODO
			}
		}
		public DrawerType drawerType => _drawerType;
		public Type type {
			set {
				_type = value;
				typeName = type.AssemblyQualifiedName;
				_drawerType = GetDrawerTypeOfType(_type);
			}
			get {
				if(_type == null && typeName != null)
					_type = Type.GetType(typeName);
				return _type;
			}
		}
	}
}