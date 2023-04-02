using UnityEngine;
using UnityEngine.UI;

namespace Nianyi {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Text))]
	public class FontFilter : MonoBehaviour {
		#region Inspector fields
		public FontMap fontMap;
		public Text target;
		public FontMap.FontSet fontSet;
		#endregion

		#region Auxiliary methods
		bool TryFetchTarget() {
			if(target)
				return true;
			if(target = GetComponent<Text>())
				return true;
			return false;
		}

		void TrySetFont() {
			if(!target || fontSet == null)
				return;
			target.font = fontSet.font;
			target.fontStyle = (FontStyle)(
				(fontSet.bold ? 1 : 0)
				| ((fontSet.italic ? 1 : 0) << 1)
			);
		}
		#endregion

		#region Life cycle
		protected void Start() {
			TrySetFont();
		}

		protected void Update() {
			if(!Application.isPlaying) {
				if(fontMap) {
					if(fontSet == null) {
						if(fontMap.fontsets.Count != 0)
							fontSet = fontMap.fontsets[0];
					}
				}
				if(!target)
					TryFetchTarget();
				if(fontMap && target)
					TrySetFont();
				return;
			}
		}
		#endregion
	}
}