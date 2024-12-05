using UnityEngine;
using System.Collections.Generic;

namespace Nianyi.UnityToolkit
{
	public class Array : ProceduralGenerator
	{
		#region Serialized fields
		[System.Serializable]
		public class GenerationOptions
		{
			public GameObject template;
			public Matrix4x4 basis = Matrix4x4.identity;
			public Vector3Int count = Vector3Int.one;
		}
		public GenerationOptions generation;
		#endregion

		#region Interfaces
		[ContextMenu("Regenerate")]
		public override void Regenerate()
		{
#if UNITY_EDITOR
			if(!Asset.IsAsset(generation.template))
				return;
#endif
			if(generation.template == null)
				return;
			if(Scene.IsAncestorOf(generation.template.transform, gameObject.transform))
			{
				Debug.LogError("Array generation aborted, as the template GameObject is an ancestor of the generator.", this);
				return;
			}

			DestroyPreviousGeneration();
			GenerateNew();
		}
		#endregion

		#region Functions
		private void DestroyPreviousGeneration()
		{
			foreach(var child in Scene.GetDirectChildren(transform))
			{
				Scene.Destroy(child.gameObject);
			}
		}

		private void GenerateNew()
		{
			List<GameObject> instances = new();
			foreach(var index in IterateThroughIntBox(generation.count))
			{
				Vector4 pos = (Vector3)index;
				pos.w = 1.0f;
				Vector3 localPosition = generation.basis * pos;

				var instance = Scene.Instantiate(generation.template, transform);
				instance.name = $"{generation.template.name}@{index}";
				instance.transform.localPosition = localPosition;
				instances.Add(instance);
			}
		}

		private IEnumerable<Vector3Int> IterateThroughIntBox(Vector3Int counts)
		{
			for(int ix = 0; ix < counts.x; ++ix)
			{
				for(int iy = 0; iy < counts.y; ++iy)
				{
					for(int iz = 0; iz < counts.z; ++iz)
					{
						yield return new(ix, iy, iz);
					}
				}
			}
		}
		#endregion
	}
}
