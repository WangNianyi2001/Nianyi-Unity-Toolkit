using System.Collections.Generic;

namespace Nianyi {
	public static class DataUtility {
		public static void SetLength<T>(this List<T> list, int count) {
			if(list.Count != count) {
				if(list.Count > count)
					list.RemoveRange(count, list.Count - count);
				else for(int i = list.Count; i < count; ++i)
						list.Add(default);
			}
		}
	}
}