namespace NStar.BigCollections.LowMemory
{
	/// <summary>
	/// Представляет строго типизированный список элементов, упорядоченных по индексу.
	/// В отличие от <see cref="List{T}"/> и стандартного <see cref="G.List{T}"/>, имеет индекс типа <see cref="MpzT"/>, а не
	/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
	/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
	/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
	/// коллекции). Методы для поиска, сортировки и других манипуляций со списком находятся в разработке, на текущий момент
	/// поддерживаются только добавление в конец, установка элемента по индексу и частично удаление.
	/// </summary>
	[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
	public class BigList<T> : BigList<T, BigList<T>, LimitedBuffer<T>>
	{
		public BigList() : base(20, 30) { }

		public BigList(MpzT capacity)
			: base(capacity, 20, 30) { }

		public BigList(G.IEnumerable<T> collection)
			: base(collection, 20, 30) { }

		public BigList(MpzT capacity, G.IEnumerable<T> collection)
			: base(capacity, collection, 20, 30) { }

		public BigList(T[] values)
			: base(values.AsEnumerable(), 20, 30) { }

		public BigList(ReadOnlySpan<T> values)
			: base(values, 20, 30) { }

		public BigList(MpzT capacity, T[] values)
			: base(capacity, values.AsEnumerable(), 20, 30) { }

		public BigList(MpzT capacity, ReadOnlySpan<T> values)
			: base(capacity, values, 20, 30) { }

		protected override Func<MpzT, BigList<T>> CapacityCreator => x => new(x);

		protected override Func<G.IEnumerable<T>, BigList<T>> CollectionCreator => x => new(x);

		protected override Func<int, LimitedBuffer<T>> CapacityLowCreator => x => new(x);

		protected override Func<G.IEnumerable<T>, LimitedBuffer<T>> CollectionLowCreator { get; } = x => new(x);
	}
}
