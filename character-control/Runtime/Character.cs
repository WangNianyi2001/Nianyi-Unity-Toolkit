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
		public CharacterShape CurrentShape => currentShape;

		[SerializeField] private List<CharacterMode> modes;
		public IList<CharacterMode> Modes => modes;
		[SerializeField] private CharacterMode currentMode;
		public CharacterMode CurrentMode => currentMode;
		#endregion

		#region Life cycle
		#endregion

		#region Event hooks
		#region Collision
		public System.Action<Collision>
			onCollisionEnter,
			onCollisionStay,
			onCollisionExit;
		#endregion
		#endregion

		#region Generic interfaces
		public Vector3 Position => CurrentShape.Body.position;

		public Vector3 Up => CurrentShape.Up;
		public Vector3 Forward => CurrentShape.Forward;
		public Vector3 LookingDirection => CurrentShape.Head.forward;
		#endregion
	}
}
