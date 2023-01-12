using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Nianyi {
	public class InputFilter : MonoBehaviour {
		public enum Type {
#if ENABLE_LEGACY_INPUT_MANAGER
			LegacyAxis = 0x01,
			LegacyKeyCode = 0x02,
#endif
#if ENABLE_INPUT_SYSTEM
			InputSystemAction = 0x11,
#endif
		}
		public Type type;

#if ENABLE_LEGACY_INPUT_MANAGER
		[ShowIfEnum("type", "LegacyAxis")] public string axis = "Fire1";
		[ShowIfEnum("type", "LegacyKeyCode")] public KeyCode keyCode = KeyCode.Mouse0;
#endif
#if ENABLE_INPUT_SYSTEM
		[ShowIfEnum("type", "InputSystemAction")] public InputAction inputAction;
#endif
	}
}
