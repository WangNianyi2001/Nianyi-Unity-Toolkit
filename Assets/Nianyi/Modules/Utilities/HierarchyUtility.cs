using UnityEngine;

namespace Nianyi {
	public static class HierarchyUtility {
		public static T EnsureComponent<T>(this GameObject obj, T component = null) where T : Component {
			if(!component || component.gameObject != obj)
				component = obj.GetComponent<T>();
			if(component == null)
				component = obj.AddComponent<T>();
			return component;
		}
	}
}
