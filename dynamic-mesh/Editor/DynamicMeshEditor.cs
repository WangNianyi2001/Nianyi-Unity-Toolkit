using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	[CustomEditor(typeof(DynamicMesh))]
	public class DynamicMeshEditor : Editor
	{
		private DynamicMesh mesh;

		private bool sourceMeshInfoExpanded = false, importedMeshInfoExpanded = false;

		private void Warning(string text)
		{
			EditorGUILayout.HelpBox(new GUIContent(text));
		}

		private void FixReadAccess()
		{
			var path = AssetDatabase.GetAssetPath(mesh.sourceMesh.GetInstanceID());
			path += ".meta";
			path = Path.Combine(Directory.GetCurrentDirectory(), path);
			string content = File.ReadAllText(path);
			content = content.Replace("m_IsReadable: 0", "m_IsReadable: 1");
			content = content.Replace("isReadable: 0", "isReadable: 1");
			File.WriteAllText(path, content);
			AssetDatabase.Refresh();
		}

		private void DrawSourceMeshSection()
		{
			SerializedProperty it = serializedObject.FindProperty("sourceMesh");
			EditorGUILayout.PropertyField(it);
			if(mesh.sourceMesh == null)
			{
				Warning("No mesh is assigned to the manifold");
			}
			else
			{
				var mesh = this.mesh.sourceMesh;
				if(!mesh.isReadable)
				{
					Warning("The read/write access for this mesh is not enabled");
					if(GUILayout.Button("Enable read/write access"))
						FixReadAccess();
				}
				else if(this.mesh.importOptions.limitVertexCount)
				{
					if(mesh.vertexCount > this.mesh.importOptions.maxVertexCount)
						Warning("Vertices of the mesh exceeds the max count specified in the import options");
				}
			}
		}

		private void DrawMeshInfoSection(Mesh mesh, string prefix, ref bool expanded)
		{
			if(mesh == null)
			{
				GUILayout.Label($"{prefix}: (not imported)");
				return;
			}

			List<int> triangleCounts = new List<int>();
			int triangleCount = 0;
			for(int i = 0; i < mesh.subMeshCount; ++i)
			{
				int count = mesh.GetTriangles(i).Length;
				triangleCount += count;
				triangleCounts.Add(count);
			}

			expanded = EditorGUILayout.Foldout(expanded, $"{prefix} (total): {mesh.vertexCount} vertices, {triangleCount} triangles");
			if(expanded)
			{
				++EditorGUI.indentLevel;
				for(int i = 0; i < mesh.subMeshCount; ++i)
				{
					EditorGUILayout.LabelField($"Submesh {i}: {triangleCounts[i]} triangles");
				}
				--EditorGUI.indentLevel;
			}
		}

		private void DrawImportSection()
		{
			var it = serializedObject.FindProperty("importOptions");
			EditorGUILayout.PropertyField(it);

			if(GUILayout.Button("Re-import Mesh Data"))
			{
				mesh.ReimportMeshData();
				return;
			}
		}

		protected void OnEnable()
		{
			mesh = target as DynamicMesh;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			DrawSourceMeshSection();
			DrawMeshInfoSection(mesh.sourceMesh, "Source mesh", ref sourceMeshInfoExpanded);
			DrawImportSection();
			DrawMeshInfoSection(mesh.ImportedMesh, "Imported mesh", ref importedMeshInfoExpanded);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
	}
}