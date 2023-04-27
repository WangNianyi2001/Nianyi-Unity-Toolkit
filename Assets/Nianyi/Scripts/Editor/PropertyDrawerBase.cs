using UnityEngine;
using UnityEditor;

namespace Nianyi.Editor {
	public abstract partial class PropertyDrawerBase : PropertyDrawer {
		#region Core fields
		Rect space, next;
		bool rendering;
		#endregion

		#region Inheritance interface
		protected bool Rendering => rendering;
		protected Rect Next => next;
		protected abstract void Draw(SerializedProperty property, GUIContent label);
		#endregion

		#region Public interface
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			rendering = false;
			space.x = space.y = space.height = 0;
			Draw(property, label);
			return space.height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			rendering = true;
			space = position;
			EditorGUI.BeginChangeCheck();
			Draw(property, label);
			if(EditorGUI.EndChangeCheck())
				property.serializedObject.ApplyModifiedProperties();
		}
		#endregion
	}
}