using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public partial class Character : MonoBehaviour
	{
		#region Configs
		[SerializeField] private List<CharacterShape> shapes;
		public IList<CharacterShape> Shapes => shapes;
		[SerializeField] private CharacterShape currentShape;
		public CharacterShape Shape => currentShape;

		[SerializeField] private List<CharacterMode> modes;
		public IList<CharacterMode> Modes => modes;
		[SerializeField] private CharacterMode currentMode;
		public CharacterMode Mode => currentMode;
		#endregion

#if UNITY_EDITOR
		#region Editor
		protected void OnDrawGizmos()
		{
			var bounds = Shape.BoundingBox;

			// Draw label.
			var labelPos = bounds.center;
			labelPos += Vector3.Project(bounds.max - bounds.center, Shape.Up);
			UnityEditor.Handles.Label(
				labelPos, name,
				new GUIStyle(GUI.skin.label)
				{
					alignment = TextAnchor.LowerCenter,
				}
			);
		}
		#endregion
#endif
	}
}
