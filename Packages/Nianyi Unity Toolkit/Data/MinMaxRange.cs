namespace Nianyi.Data {
	public struct MinMaxRange<T> {
		public T min, max;

		public MinMaxRange(T min, T max) {
			this.min = min;
			this.max = max;
		}

		public MinMaxRange(MinMaxRange<T> range) : this(range.min, range.max) { }
	}
}
