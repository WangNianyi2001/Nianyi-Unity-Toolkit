#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public partial class CharacterControl
	{
		protected void OnValidate()
		{
			if(Scene.GetCurrentMode() == Scene.SceneMode.Play)
				return;

			static void ResetArrayRepresentative<T>(ref T value, IList<T> array) where T : class
			{
				if(array.Count == 0)
					value = null;
				else if(value == null)
					value = array[0];
			}

			ResetArrayRepresentative(ref currentShape, shapes);
			ResetArrayRepresentative(ref currentMode, modes);
		}

		protected void OnDrawGizmos()
		{
			if(Shape)
			{
				var bounds = Shape.BoundingBox;

				// Draw label.
				var labelPos = bounds.center;
				labelPos += Vector3.Project(bounds.max - bounds.center, Shape.Up);
				Handles.Label(
					labelPos, name,
					new GUIStyle(GUI.skin.label)
					{
						alignment = TextAnchor.LowerCenter,
					}
				);
			}
		}
	}
}
#endif