using UnityEngine;
using UnityEditor;

namespace Nianyi.Editor {
	public abstract partial class PropertyDrawerBase : PropertyDrawer {
		#region Core fields
		Rect space, next;
		bool rendering;
		Object targetObject;
		#endregion

		#region Inheritance interface
		protected bool Rendering => rendering;
		protected Rect Next => next;
		protected abstract void Draw(SerializedProperty property, GUIContent label);
		protected virtual void DrawNull(SerializedProperty property, GUIContent label) {
			LabelField(new GUIContent("This object is null"));
		}
		#endregion

		#region Public interface
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			rendering = false;
			space.x = space.y = space.height = 0;
			targetObject = property.objectReferenceValue;
			if(targetObject != null)
				Draw(property, label);
			else
				DrawNull(property, label);
			return space.height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			rendering = true;
			space = position;
			EditorGUI.BeginChangeCheck();
			if(targetObject)
				Draw(property, label);
			else
				DrawNull(property, label);
			if(EditorGUI.EndChangeCheck())
				property.serializedObject.ApplyModifiedProperties();
		}
		#endregion
	}
}