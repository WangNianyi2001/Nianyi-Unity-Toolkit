using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Nianyi.Data {
	public class DisjointSet<T> : IEnumerable<T> {
		#region Internal fields
		public class Set<U> : List<U> { }
		public class Collection<U> : List<U> { }

		protected Collection<Set<T>> sets = new Collection<Set<T>>();
		#endregion

		#region Internal functions
		protected Set<T> FindSet(T representative) {
			foreach(var set in sets) {
				if(set.Contains(representative))
					return set;
			}
			return null;
		}
		#endregion

		#region Interface implementations
		public IEnumerator<T> GetEnumerator() {
			foreach(var set in sets) {
				foreach(var element in set)
					yield return element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion

		#region Public interfaces
		public IEnumerable<Set<T>> Sets => sets;

		public void Add(T element) {
			if(sets.Any(set => set.Contains(element)))
				return;
			var set = new Set<T> { element };
			sets.Add(set);
		}

		public void AddTo(T element, T representative) {
			var set = FindSet(representative);
			if(set != null) {
				if(!set.Contains(element))
					set.Add(element);
			}
			else
				Add(element);
		}

		public bool MergeSet(T a, T b) {
			Set<T> setA = FindSet(a), setB = FindSet(b);
			if(setA == null || setB == null)
				return false;
			setA.AddRange(setB);
			sets.Remove(setB);
			return true;
		}
		#endregion
	}
}