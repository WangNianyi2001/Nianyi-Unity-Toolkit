using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public partial class CharacterControl : MonoBehaviour
	{
		#region Life cycle
		protected void Start()
		{
			foreach(var shape in shapes)
				shape.enabled = shape == Shape;
			foreach(var mode in modes)
				mode.gameObject.SetActive(mode == Mode);
		}
		#endregion

		#region Shape
		[SerializeField] private List<CharacterShape> shapes;
		public IList<CharacterShape> Shapes => shapes;
		[SerializeField] private CharacterShape currentShape;
		public CharacterShape Shape
		{
			get => currentShape;
			set
			{
				if(currentShape != null)
					currentShape.enabled = false;
				currentShape = value;
				currentShape.enabled = true;
			}
		}
		public void SwitchShape(string name)
		{
			var shape = shapes.Find(s => s.name == name);
			if(shape == null)
			{
				Debug.LogError($"Cannot find shape with name \"{name}\".", this);
				return;
			}
			Shape = shape;
		}
		#endregion

		#region Mode
		[SerializeField] private List<CharacterMode> modes;
		public IList<CharacterMode> Modes => modes;
		[SerializeField] private CharacterMode currentMode;
		public CharacterMode Mode
		{
			get => currentMode;
			set
			{
				if(currentMode != null)
					currentMode.gameObject.SetActive(false);
				currentMode = value;
				currentMode.gameObject.SetActive(true);
			}
		}

		public void SwitchMode(string name)
		{
			var mode = modes.Find(m => m.name == name);
			if(mode == null)
			{
				Debug.LogError($"Cannot find mode with name \"{name}\".", this);
				return;
			}
			Mode = mode;
		}
		#endregion
	}
}
