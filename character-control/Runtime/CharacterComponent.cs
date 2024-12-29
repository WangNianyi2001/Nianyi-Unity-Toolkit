using UnityEngine;

namespace Nianyi.UnityToolkit
{
	public abstract class CharacterComponent : MonoBehaviour
	{
		private CharacterControl character;
		public CharacterControl Character
		{
			get
			{
				if(character == null)
					character = GetComponentInParent<CharacterControl>();
				return character;
			}
		}

		protected CharacterShape Shape => Character.Shape;
		protected CharacterMode Mode => Character.Mode;
	}
}
