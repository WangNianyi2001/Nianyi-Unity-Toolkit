using UnityEditor;
using UnityEngine;
using System.IO;

namespace Nianyi.Data.Editor {
	[CustomEditor(typeof(Mesh))]
	public class MeshEditor : UnityEditor.Editor {
		private Mesh mesh;

		private void Warning(string text) {
			EditorGUILayout.HelpBox(new GUIContent(text));
		}

		private void FixReadAccess() {
			var path = AssetDatabase.GetAssetPath(mesh.sourceMesh.GetInstanceID());
			path += ".meta";
			path = Path.Combine(Directory.GetCurrentDirectory(), path);
			string content = File.ReadAllText(path);
			content = content.Replace("m_IsReadable: 0", "m_IsReadable: 1");
			content = content.Replace("isReadable: 0", "isReadable: 1");
			File.WriteAllText(path, content);
			AssetDatabase.Refresh();
		}

		private void DrawSourceMeshSection() {
			SerializedProperty it = serializedObject.FindProperty("sourceMesh");
			EditorGUILayout.PropertyField(it);
			if(mesh.sourceMesh == null) {
				Warning("No mesh is assigned to the manifold");
			}
			else {
				var mesh = this.mesh.sourceMesh;
				if(!mesh.isReadable) {
					Warning("The read/write access for this mesh is not enabled");
					if(GUILayout.Button("Enable read/write access"))
						FixReadAccess();
				}
				else if(this.mesh.importOptions.limitVertexCount) {
					if(mesh.vertexCount > this.mesh.importOptions.maxVertexCount)
						Warning("Vertices of the mesh exceeds the max count specified in the import options");
				}
			}
			if(mesh.sourceMesh != null) {
				int triangleCount = 0;
				for(int i = 0; i < mesh.sourceMesh.subMeshCount; ++i)
					triangleCount += mesh.sourceMesh.GetTriangles(i).Length;
				GUILayout.Label($"Source mesh: {mesh.sourceMesh.vertexCount} vertices, {triangleCount} triangles");
			}
		}

		private void DrawImportSection() {
			var it = serializedObject.FindProperty("importOptions");
			EditorGUILayout.PropertyField(it);

			if(GUILayout.Button("Re-import Mesh Data")) {
				mesh.ReimportMeshData();
			}

			if(mesh.data != null) {
				GUILayout.Label($"Imported data: {mesh.data.vertices.Count} vertices, {mesh.data.surfaces.Count} triangles");
			}
			else {
				GUILayout.Label("Imported data: (not imported)");
			}
		}

		protected void OnEnable() {
			mesh = target as Mesh;
		}

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();

			DrawSourceMeshSection();
			DrawImportSection();

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
	}
}