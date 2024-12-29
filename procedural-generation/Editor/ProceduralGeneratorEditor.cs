using UnityEngine;
using UnityEditor;

namespace Nianyi.UnityToolkit
{
	[CustomEditor(typeof(ProceduralGenerator), true)]
	public class ProceduralGeneratorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var generator = target as ProceduralGenerator;
			if(!generator.CouldGenerate(out string message))
				EditorGUILayout.HelpBox(message, MessageType.Warning, true);
			else
			{
				if(!generator.ShouldAutomaticallyRegenerate())
				{
					bool regenerate = GUILayout.Button(new GUIContent("Regenerate"));
					if(regenerate)
						generator.Regenerate();
				}
			}

			base.OnInspectorGUI();
		}
	}
}
