using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nianyi.UnityToolkit
{
	public abstract class ProceduralGenerator : MonoBehaviour
	{
		#region Serialized fields
		[System.Serializable]
		public struct UpdateOptions
		{
			public bool updateWhenEditing;
			[ShowWhen("updateWhenEditing", true)]
			public bool updateInPrefabMode;
		}
		public UpdateOptions updateOptions;
		#endregion

		#region Interfaces
		public abstract void Regenerate();
		#endregion

		#region Life cycle
		protected void OnChanged()
		{
			if(ShouldAutomaticallyRegenerate())
				Regenerate();
		}
		#endregion

		#region Unity messages
#if UNITY_EDITOR
		/// <remarks>
		/// Should not trigger main-thread operations here.
		/// Add a listener on <c>EditorApplication.Update</c>.
		/// </remarks>
		/// <see href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html" />
		protected void OnValidate()
		{
			void TriggerOnChangeOnNextEditorUpdate()
			{
				EditorApplication.update -= TriggerOnChangeOnNextEditorUpdate;
				OnChanged();
			}
			EditorApplication.update += TriggerOnChangeOnNextEditorUpdate;
		}
#endif

		public bool ShouldAutomaticallyRegenerate()
		{
			return Scene.GetCurrentMode() switch
			{
				Scene.SceneMode.Play => true,
				Scene.SceneMode.Edit => updateOptions.updateWhenEditing,
				Scene.SceneMode.Prefab => updateOptions.updateWhenEditing && updateOptions.updateInPrefabMode,
				_ => false,
			};
		}
		#endregion
	}
}
