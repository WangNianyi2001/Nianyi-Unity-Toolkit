using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Linq;

namespace Nianyi.UnityToolkit
{
	/// <summary>In-world hierarchical actions.</summary>
	public static class Hierarchy
	{
		#region Existence
		/// <remarks>Will create a prefab instance if <c>template</c> is a prefab asset and in edit mode.</remarks>
		public static GameObject Instantiate(GameObject template, Transform under = null) {
#if UNITY_EDITOR
			if(!Application.isPlaying) {
				if(PrefabUtility.IsPartOfPrefabAsset(template))
					return PrefabUtility.InstantiatePrefab(template, under) as GameObject;
			}
#endif
			return Object.Instantiate(template, under);
		}

		/// <remarks>Cannot delete assets.</remarks>
		public static void Destroy(this Object target) {
#if UNITY_EDITOR
			if(!Application.isPlaying)
				Object.DestroyImmediate(target);
#endif
			Object.Destroy(target);
		}
		#endregion

		#region Genealogy
		public static bool IsAncestorOf(this Transform target, Transform reference)
		{
			if(target == null)
				return true;
			if(reference == null)
				return false;
			return reference.IsChildOf(target);
		}

		public static bool IsDescendantOf(this Transform target, Transform reference) {
			if(target == null)
				return false;
			if(reference == null)
				return true;
			return target.IsChildOf(reference);
		}

		public static Transform[] GetAncestorChain(this Transform target, bool bottomToTop = true) {
			List<Transform> chain = new();
			for(; target != null; target = target.parent)
				chain.Add(target);
			if(!bottomToTop)
				chain.Reverse();
			return chain.ToArray();
		}

		public static Transform[] GetDirectChildren(this Transform parent) {
			Transform[] children = new Transform[parent.childCount];
			for(int i = 0; i < parent.childCount; ++i)
				children[i] = parent.GetChild(i);
			return children;
		}
		#endregion

		#region Finding
		public static IEnumerable<T> FindObjectsByName<T>(string name, bool includeInactive = false) where T : Component {
			FindObjectsInactive includingFlag = includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;
			var instances = Object.FindObjectsByType<T>(includingFlag, FindObjectsSortMode.None);
			return instances.Where(i => i.name == name);
		}

		public static T FindObjectByName<T>(string name, bool includeInactive = false) where T : Component {
			return FindObjectsByName<T>(name, includeInactive).FirstOrDefault();
		}
		#endregion

		#region Component
		public static T EnsureComponent<T>(this GameObject gameObject) where T : Component {
			if(gameObject.TryGetComponent<T>(out var existing))
				return existing;
			return gameObject.AddComponent<T>();
		}

		public static T EnsureComponent<T>(this Component target) where T : Component
			=> target.gameObject.EnsureComponent<T>();
		#endregion

	}
}
