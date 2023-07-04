using UnityEngine;
using System.Security.Cryptography;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Nianyi {
	public abstract class BehaviourBase : MonoBehaviour {
		#region Internal functions
		private const string
			gameModeMessagePrefix = "OnGame",
			editModeMessagePrefix = "OnEdit",
			sceneModeMessagePrefix = "OnScene",
			prefabModeMessagePrefix = "OnPrefab";
#if UNITY_EDITOR
		private void EditorCallByMode(string stem, params object[] parameters) {
			this.Call(editModeMessagePrefix + stem, parameters);
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if(prefabStage == null)
				this.Call(sceneModeMessagePrefix + stem, parameters);
			else
				this.Call(prefabModeMessagePrefix + stem, parameters);
		}
#endif
		private void CallByMode(string stem, bool editorDelayed, params object[] parameters) {
			if(Application.isPlaying) {
				this.Call(gameModeMessagePrefix + stem, parameters);
				return;
			}
#if UNITY_EDITOR
			if(!editorDelayed) {
				EditorCallByMode(stem, parameters);
			}
			else {
				EditorApplication.delayCall += () => {
					if(Application.isPlaying)
						return;
					EditorCallByMode(stem, parameters);
					EditorApplication.QueuePlayerLoopUpdate();
				};
			}
#endif
		}
		#endregion

		#region Public functions
		public T EnsureComponent<T>(T result = null) where T : Component => gameObject.EnsureComponent<T>(result);
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
		private void OnEnable() => CallByMode("Enable", false);
		// Editor
		private void OnValidate() => CallByMode("Validate", true);
		// Initialization
		private void Start() => CallByMode("Start", false);
		// Physics
		private void FixedUpdate() => CallByMode("FixedUpdate", false);
		// Game logic
		private void Update() => CallByMode("Update", false);
		private void LateUpdate() => CallByMode("LateUpdate", false);
		// Scene rendering
		// Gizmos rendering
		private void OnDrawGizmos() => CallByMode("DrawGizmos", false);
		// GUI rendering
		private void OnGUI() => CallByMode("GUI", false);
		// Decommissioning
		private void OnDisable() => CallByMode("Disable", false);
		#endregion
	}
}