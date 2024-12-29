using UnityEngine;
using UnityEngine.Events;

namespace Nianyi.UnityToolkit
{
	public abstract class CharacterComponent : MonoBehaviour
	{
		#region Accessors
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
		#endregion

		#region Life cycle
		[SerializeField] private UnityEvent onEnable;
		protected virtual void OnEnable()
		{
			onEnable.Invoke();
		}
		[SerializeField] private UnityEvent onDisable;
		protected virtual void OnDisable()
		{
			onDisable.Invoke();
		}
		#endregion
	}
}
