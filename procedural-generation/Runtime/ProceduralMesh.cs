using UnityEngine;

namespace Nianyi.UnityToolkit
{
	[RequireComponent(typeof(MeshFilter))]
	public abstract class ProceduralMesh : ProceduralGenerator
	{
		#region Component
		private MeshFilter meshFilter;
		private MeshFilter MeshFilter
		{
			get
			{
				if(meshFilter == null)
					meshFilter = GetComponent<MeshFilter>();
				if(meshFilter == null)
					meshFilter = gameObject.AddComponent<MeshFilter>();
				return meshFilter;
			}
		}
		#endregion

		#region Mesh
		private Mesh mesh;
		protected Mesh Mesh
		{
			get => mesh;
			set
			{
				if(mesh != null)
					DestroyMesh();

				mesh = value;
				MeshFilter.sharedMesh = mesh;
			}
		}

		protected void DestroyMesh()
		{
			Asset.Destroy(mesh);
			mesh = null;
		}
		#endregion
	}
}
