using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nianyi.UnityToolkit
{
	public static class Asset
	{
		/// <remarks>Only works in editor.</remarks>
		public static bool IsAsset(this Object target)
		{
			if(target == null)
				return false;
#if !UNITY_EDITOR
			Debug.LogWarning("Shouldn't check if an Unity object is an asset in non-editor environment.");
			return false;
#else
			return AssetDatabase.Contains(target);
#endif
		}
	}
}
