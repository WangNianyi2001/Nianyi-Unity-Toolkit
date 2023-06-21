using UnityEditor;
using UnityEngine;
using System.IO;

namespace Nianyi.Editor {
	[CustomEditor(typeof(MeshManifold))]
	public class MeshManifoldEditor : UnityEditor.Editor {
		private MeshManifold meshManifold;

		private void Warning(string text) {
			EditorGUILayout.HelpBox(new GUIContent(text));
		}

		private void FixReadAccess() {
			var path = AssetDatabase.GetAssetPath(meshManifold.Mesh.GetInstanceID());
			path += ".meta";
			path = Path.Combine(Directory.GetCurrentDirectory(), path);
			string content = File.ReadAllText(path);
			content = content.Replace("m_IsReadable: 0", "m_IsReadable: 1");
			content = content.Replace("isReadable: 0", "isReadable: 1");
			File.WriteAllText(path, content);
			AssetDatabase.Refresh();
		}

		protected void OnEnable() {
			meshManifold = target as MeshManifold;
		}

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();

			SerializedProperty it = serializedObject.FindProperty("mesh");
			EditorGUILayout.PropertyField(it);
			if(meshManifold.Mesh == null) {
				Warning("No mesh is assigned to the manifold");
			}
			else {
				var mesh = meshManifold.Mesh;
				if(!mesh.isReadable) {
					Warning("The read/write access for this mesh is not enabled");
					if(GUILayout.Button("Enable read/write access"))
						FixReadAccess();
				}
				else if(meshManifold.importOptions.limitVertexCount) {
					if(mesh.vertexCount > meshManifold.importOptions.maxVertexCount)
						Warning("Vertices of the mesh exceeds the max count specified in the import options");
				}
			}
			for(; it.NextVisible(false);) {
				EditorGUILayout.PropertyField(it);
			}

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
	}
}