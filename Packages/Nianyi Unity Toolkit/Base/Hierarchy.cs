using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nianyi {
	public static class Hierarchy {
		public static Camera EditorSceneCamera {
			get {
#if UNITY_EDITOR
				if(SceneView.currentDrawingSceneView != null)
					return SceneView.currentDrawingSceneView.camera;
#endif
				return null;
			}
		}

		public static T EnsureComponent<T>(this GameObject obj, T component = null) where T : Component {
			if(!component || component.gameObject != obj)
				component = obj.GetComponent<T>();
			if(component == null)
				component = obj.AddComponent<T>();
			return component;
		}
	}
}
