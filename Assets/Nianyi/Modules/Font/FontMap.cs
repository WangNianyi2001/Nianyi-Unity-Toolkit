using UnityEngine;
using System;
using System.Collections.Generic;

namespace Nianyi {
	[CreateAssetMenu(menuName = "Nianyi/Font Map")]
	public class FontMap : ScriptableObject {
		[Serializable]
		public class FontSet {
			public string name;
			public Font font;
			public bool bold, italic;
		}
		public List<FontSet> fontsets;
	}
}