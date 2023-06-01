using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Nianyi {
	public abstract class BehaviourBase : MonoBehaviour {
		#region Internal functions
		private static string
			gameModeMessagePrefix = "OnGame",
			editModeMessagePrefix = "OnEdit",
			sceneModeMessagePrefix = "OnScene",
			prefabModeMessagePrefix = "OnPrefab";
		private void SendPrivateMessageByMode(string stem, params object[] parameters) {
			if(Application.isPlaying) {
				SendPrivateMessage(gameModeMessagePrefix + stem, parameters);
				return;
			}
#if UNITY_EDITOR
			SendPrivateMessage(editModeMessagePrefix + stem, parameters);
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if(prefabStage == null)
				SendPrivateMessage(sceneModeMessagePrefix + stem, parameters);
			else
				SendPrivateMessage(prefabModeMessagePrefix + stem, parameters);
#endif
		}
		#endregion

		#region Public functions
		/// <summary>
		/// Send a message to this component only.
		/// </summary>
		public void SendPrivateMessage(string name, params object[] parameters) {
			if(!isActiveAndEnabled)
				return;
			try {
				this.Call(name, parameters);
			}
			catch { }
		}
		#endregion

		#region Message handlers
		/**
		 * Sometimes you'll need behaviours to be labelled with `ExecuteAlways`
		 * or `ExecuteInEditMode` to give them special abilities while in edit mode.
		 * Cons of this is that in-game logics & editor logics could easily get
		 * messed up; or you gotta use `Application.isPlaying` to manually check if
		 * the game is running. This part of the job is done automatically here.
		 * 
		 * It is adviced best to put editor functions as a partial class in
		 * separate files named like `*.editor.cs` and wrap them around with
		 * `#if UNITY_EDITOR` and `#endif`. This way editor logics will not be
		 * built into the game.
		 * 
		 * This is *not* a full list of all possible messages that can be fired
		 * on a `MonoBehaviour`. You should extend/trim this base class in your
		 * project based on your custom needs.
		 */

		// See https://docs.unity3d.com/Manual/ExecutionOrder.html
		// Initialization
		private void Awake() => SendPrivateMessageByMode("Awake");
		private void OnEnable() => SendPrivateMessageByMode("Enable");
		// Editor
		// Initialization
		private void Start() => SendPrivateMessageByMode("Start");
		// Physics
		private void FixedUpdate() => SendPrivateMessageByMode("FixedUpdate");
		// Game logic
		private void Update() => SendPrivateMessageByMode("Update");
		private void LateUpdate() => SendPrivateMessageByMode("LateUpdate");
		// Scene rendering
		// Gizmos rendering
		private void OnDrawGizmos() => SendPrivateMessageByMode("DrawGizmos");
		// GUI rendering
		private void OnGUI() => SendPrivateMessageByMode("GUI");
		// Decommissioning
		private void OnDisable() => SendPrivateMessageByMode("Disable");
		#endregion
	}
}