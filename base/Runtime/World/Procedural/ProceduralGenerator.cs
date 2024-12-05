using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nianyi.UnityToolkit
{
	[ExecuteAlways]
	public abstract class ProceduralGenerator : MonoBehaviour
	{
		#region Serialized fields
		[System.Serializable]
		private struct UpdateOptions
		{
			public bool updateWhenEditing;
			[ShowWhen("updateWhenEditing", true)]
			public bool updateInPrefabMode;
		}
		[SerializeField] private UpdateOptions updateOptions;
		#endregion

		#region Interfaces
		public abstract void Regenerate();
		#endregion

		#region Life cycle
		protected void OnChanged()
		{
			switch(Scene.GetCurrentMode())
			{
				case Scene.SceneMode.Play:
					OnChangedInPlayMode();
					break;
				case Scene.SceneMode.Edit:
					OnChangedInEditMode();
					break;
				case Scene.SceneMode.Prefab:
					OnChangedInPrefabMode();
					break;
			}
		}

		protected virtual void OnChangedInPlayMode()
		{
			Regenerate();
		}

		protected virtual void OnChangedInEditMode()
		{
			if(updateOptions.updateWhenEditing)
				Regenerate();
		}

		protected virtual void OnChangedInPrefabMode()
		{
			if(updateOptions.updateWhenEditing && updateOptions.updateInPrefabMode)
				Regenerate();
		}
		#endregion

		#region Unity messages
		protected void Update()
		{
			// Filter out normal in-play updates.
			if(Application.isPlaying)
				return;

			OnChanged();
		}

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
		#endregion
	}
}
