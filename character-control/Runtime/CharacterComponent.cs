using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public abstract class CharacterComponent : MonoBehaviour
	{
		private Character character;
		public Character Character
		{
			get
			{
				if(character == null)
					character = GetComponentInParent<Character>();
				return character;
			}
		}

		protected CharacterShape Shape => Character.CurrentShape;
		protected CharacterMode Mode => Character.CurrentMode;
	}
}
