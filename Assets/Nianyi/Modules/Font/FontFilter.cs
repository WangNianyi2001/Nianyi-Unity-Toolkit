using UnityEngine;
using UnityEngine.UI;

namespace Nianyi {
	[ExecuteAlways]
	public class FontFilter : BehaviourBase {
		#region Serialized fields
		public FontMap fontMap;
		public Text target;
		public FontMap.FontSet fontSet;
		#endregion

		#region Internal functions
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

		#region Message handlers
		protected void Start() {
			TrySetFont();
		}

		protected void OnEditUpdate() {
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
		}
		#endregion
	}
}