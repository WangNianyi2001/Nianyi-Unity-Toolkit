using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;

namespace Nianyi {
	public class SerializableParameter : ScriptableObject {
		public enum LegacyDrawerType {
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
		public static readonly Dictionary<Type, LegacyDrawerType> drawerTypeMap = new() {
			{ typeof(int), LegacyDrawerType.Integer },
			{ typeof(long), LegacyDrawerType.Integer },
			{ typeof(float), LegacyDrawerType.Float },
			{ typeof(double), LegacyDrawerType.Float },
			{ typeof(string), LegacyDrawerType.String },
			{ typeof(bool), LegacyDrawerType.Boolean },
			{ typeof(Color), LegacyDrawerType.Color },
			{ typeof(UnityEngine.Object), LegacyDrawerType.UnityObject },
			{ typeof(LayerMask), LegacyDrawerType.LayerMask },
			{ typeof(Enum), LegacyDrawerType.Enum },
			{ typeof(Vector2), LegacyDrawerType.Vector2 },
			{ typeof(Vector3), LegacyDrawerType.Vector3 },
			{ typeof(Vector4), LegacyDrawerType.Vector4 },
			{ typeof(Rect), LegacyDrawerType.Rect },
			{ typeof(char), LegacyDrawerType.Character },
			{ typeof(AnimationCurve), LegacyDrawerType.AnimationCurve },
			{ typeof(Bounds), LegacyDrawerType.Bounds },
			{ typeof(Gradient), LegacyDrawerType.Gradient },
			{ typeof(Quaternion), LegacyDrawerType.Quaternion },
			{ typeof(Vector2Int), LegacyDrawerType.Vector2Int },
			{ typeof(Vector3Int), LegacyDrawerType.Vector3Int },
			{ typeof(RectInt), LegacyDrawerType.Rect },
			{ typeof(BoundsInt), LegacyDrawerType.BoundsInt },
		};
		public static LegacyDrawerType GetDrawerTypeOfType(Type type) {
			foreach(var key in drawerTypeMap.Keys) {
				if(key.IsAssignableFrom(type))
					return drawerTypeMap[key];
			}
			return LegacyDrawerType.Generic;
		}
		static readonly Dictionary<Type, FieldInfo> valueAccessorMap = new(
			new Dictionary<Type, string> {
				{ typeof(string), "serializedString" },
				{ typeof(UnityEngine.Object), "serializedUnityObject" },
				{ typeof(AnimationCurve), "serializedAnimationCurve" },
				{ typeof(Gradient), "serializedGradient" },
			}.Select(pair => new KeyValuePair<Type, FieldInfo>(
				pair.Key,
				typeof(SerializableParameter).GetField(pair.Value, BindingFlags.Instance | BindingFlags.NonPublic)
			))
		);

		/* Core fields */
		[SerializeField] string typeName;
		Type _type;
		[SerializeField] LegacyDrawerType drawerType;
		[SerializeField] byte[] serializedBytes;
		[SerializeField] string serializedString;
		[SerializeField] UnityEngine.Object serializedUnityObject;
		[SerializeField] AnimationCurve serializedAnimationCurve;
		[SerializeField] Gradient serializedGradient;

		/* Properties */
		FieldInfo valueAccessor;
		public object Value {
			get {
				if(Type.IsValueType)
					return Reflection.BytesToStruct(Type, serializedBytes);
				if(valueAccessor == null)
					Type = _type;
				return valueAccessor.GetValue(this);
			}
			set {
				if(Type.IsValueType) {
					serializedBytes = Reflection.StructToBytes(Type, value);
					return;
				}
				if(valueAccessor == null)
					Type = _type;
				valueAccessor.SetValue(this, value);
			}
		}
		public LegacyDrawerType DrawerType => drawerType;
		public Type Type {
			set {
				_type = value;
				typeName = Type.AssemblyQualifiedName;
				drawerType = GetDrawerTypeOfType(_type);
				valueAccessor = null;
				foreach(var key in valueAccessorMap.Keys) {
					if(key.IsAssignableFrom(_type)) {
						valueAccessor = valueAccessorMap[key];
						break;
					}
				}
			}
			get {
				if(_type == null && typeName != null)
					_type = Type.GetType(typeName);
				return _type;
			}
		}
	}
}