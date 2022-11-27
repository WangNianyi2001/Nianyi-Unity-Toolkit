using UnityEngine;
using UnityEditor;
using System;

namespace Nianyi.Editor {
	public abstract partial class PropertyDrawerBase : PropertyDrawer {
		#region Core fields
		Rect space, next;
		bool rendering;
		#endregion

		#region Inheritance interface
		protected bool Rendering => rendering;
		protected Rect Next => next;
		protected abstract void Draw(MemberAccessor member, GUIContent label);
		#endregion

		#region Public interface
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			=> GetPropertyHeight(new MemberAccessor(property), label);
		public float GetPropertyHeight(MemberAccessor member, GUIContent label) {
			rendering = false;
			space.x = space.y = space.height = 0;
			Draw(member, label);
			return space.height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			=> OnGUI(position, new MemberAccessor(property), label);
		public void OnGUI(Rect position, MemberAccessor member, GUIContent label) {
			rendering = true;
			space = position;
			var so = new SerializedObject(member.root as UnityEngine.Object);
			so.Update();
			EditorGUI.BeginChangeCheck();
			Draw(member, label);
			if(EditorGUI.EndChangeCheck()) {
				so.ApplyModifiedProperties();
			}
		}
		#endregion
	}
}