using UnityEngine;
using UnityEditor;
using System;

namespace Nianyi.Editor {
	public abstract partial class PropertyDrawerBase : PropertyDrawer {
		#region Layout
		protected void MakeHeight(float height) {
			if(rendering) {
				next = new Rect(space) { height = height };
				space.yMin += height;
			}
			else {
				space.height += height;
			}
		}
		protected void MakeSpacing() => MakeHeight(EditorGUIUtility.standardVerticalSpacing);
		protected void MakeLine(int count = 1) {
			if(count < 1)
				return;
			MakeHeight(EditorGUIUtility.singleLineHeight);
			for(int i = 1; i < count; ++i) {
				MakeSpacing();
				MakeHeight(EditorGUIUtility.singleLineHeight);
			}
		}
		#endregion

		#region Using other drawers
		protected void DrawWith(PropertyDrawer drawer, SerializedProperty property, GUIContent label) {
			MakeHeight(drawer.GetPropertyHeight(property, label));
			if(Rendering) {
				EditorGUI.BeginChangeCheck();
				drawer.OnGUI(Next, property, label);
				if(EditorGUI.EndChangeCheck())
					property.serializedObject.ApplyModifiedProperties();
			}
		}
		#endregion

		#region Input fields
		protected delegate T StandardDrawer<T>(Rect position, GUIContent label, T value);
		protected T StandardDraw<T>(StandardDrawer<T> drawer, T value, GUIContent label, int lineCount = 1) {
			MakeLine(lineCount);
			if(Rendering)
				return drawer(Next, label, value);
			return value;
		}

		protected Bounds BoundsField(Bounds value, GUIContent label) => StandardDraw(EditorGUI.BoundsField, value, label);
		protected BoundsInt BoundsIntField(BoundsInt value, GUIContent label) => StandardDraw(EditorGUI.BoundsIntField, value, label);
		protected Color ColorField(Color value, GUIContent label) {
			MakeLine();
			if(Rendering)
				return EditorGUI.ColorField(Next, label, value, true, true, true);
			else return value;
		}
		protected AnimationCurve CurveField(AnimationCurve value, GUIContent label) => StandardDraw(EditorGUI.CurveField, value, label);
		protected Enum EnumPopUp(Enum value, GUIContent label) => StandardDraw(EditorGUI.EnumPopup, value, label);
		protected Enum EnumFlagsField(Enum value, GUIContent label) => StandardDraw(EditorGUI.EnumFlagsField, value, label);
		protected float FloatField(float value, GUIContent label) => StandardDraw(EditorGUI.FloatField, value, label);
		protected Gradient GradientField(Gradient value, GUIContent label) => StandardDraw(EditorGUI.GradientField, value, label);
		protected int IntField(int value, GUIContent label) => StandardDraw(EditorGUI.IntField, value, label);
		protected int LayerField(int value, GUIContent label) => StandardDraw(EditorGUI.LayerField, value, label);
		protected UnityEngine.Object ObjectField(UnityEngine.Object value, GUIContent label, Type objType, bool allowSceneObjects) {
			MakeLine();
			if(Rendering)
				return EditorGUI.ObjectField(Next, label, value, objType, allowSceneObjects);
			else return value;
		}
		protected Rect RectField(Rect value, GUIContent label) => StandardDraw(EditorGUI.RectField, value, label);
		protected RectInt RectIntField(RectInt value, GUIContent label) => StandardDraw(EditorGUI.RectIntField, value, label);
		protected string TagField(string value, GUIContent label) => StandardDraw(EditorGUI.TagField, value, label);
		protected string TextField(string value, GUIContent label) => StandardDraw(EditorGUI.TextField, value, label);
		protected bool Toggle(bool value, GUIContent label) => StandardDraw(EditorGUI.Toggle, value, label);
		protected Vector2 Vector2Field(Vector2 value, GUIContent label) => StandardDraw(EditorGUI.Vector2Field, value, label);
		protected Vector2Int Vector2IntField(Vector2Int value, GUIContent label) => StandardDraw(EditorGUI.Vector2IntField, value, label);
		protected Vector3 Vector3Field(Vector3 value, GUIContent label) => StandardDraw(EditorGUI.Vector3Field, value, label);
		protected Vector3Int Vector3IntField(Vector3Int value, GUIContent label) => StandardDraw(EditorGUI.Vector3IntField, value, label);
		protected Vector4 Vector4Field(Vector4 value, GUIContent label) => StandardDraw(EditorGUI.Vector4Field, value, label);
		#endregion

		protected void LabelField(GUIContent label) {
			MakeLine();
			if(Rendering)
				EditorGUI.LabelField(Next, label, EditorStyles.label);
		}
		protected bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren = false) {
			MakeHeight(EditorGUI.GetPropertyHeight(property, label, includeChildren));
			return Rendering ? EditorGUI.PropertyField(Next, property, label, includeChildren) : false;
		}

		#region Interactable controls
		protected bool DropdownButton(GUIContent content, GUIContent label = null) {
			GUIStyle style = GuiUtility.guiStyles.DropdownButton;
			MakeHeight(style.CalcHeight(content, space.width));
			if(!Rendering)
				return false;
			bool labelEmpty = label == null || label.Equals(GUIContent.none);
			Rect position = labelEmpty ? Next : EditorGUI.PrefixLabel(Next, label);
			return EditorGUI.DropdownButton(position, content, FocusType.Keyboard, style);
		}
		protected bool Button(GUIContent content, GUIContent label = null) {
			GUIStyle style = GuiUtility.guiStyles.Button;
			MakeHeight(style.CalcHeight(content, space.width));
			if(!Rendering)
				return false;
			bool labelEmpty = label == null || label.Equals(GUIContent.none);
			Rect position = labelEmpty ? Next : EditorGUI.PrefixLabel(Next, label);
			return GUI.Button(position, content, style);
		}
		protected bool Foldout(bool value, GUIContent content) {
			MakeLine();
			return Rendering ? EditorGUI.Foldout(Next, value, content) : value;
		}
		protected bool Alert(string title, string message, string ok, string cancel = "")
			=> EditorUtility.DisplayDialog(title, message, ok, cancel);
		#endregion
	}
}