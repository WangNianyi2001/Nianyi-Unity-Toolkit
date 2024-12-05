using UnityEngine;
using UnityEditor;

namespace Nianyi.UnityToolkit
{
	[CustomEditor(typeof(Array))]
	public class ArrayEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var array = target as Array;
			if(array.generation.template != null)
			{
				if(!Asset.IsAsset(array.generation.template))
				{
					EditorGUILayout.HelpBox(
						"Template GameObject is not a prefab. Will not regenerate when editing.",
						MessageType.Warning,
						true
					);
				}
			}

			base.OnInspectorGUI();
		}
	}
}
