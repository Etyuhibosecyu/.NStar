namespace NStar.BigCollections;

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
public class CustomBigList<T> : BigList<T, CustomBigList<T>, LimitedBuffer<T>>
{
	public CustomBigList() : base() { }

	public CustomBigList(int subbranchesBitLength, int leafSizeBitLength = -1)
		: base(subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(capacity, subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(collection, subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(MpzT capacity, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(capacity, collection, subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(T[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(values.AsEnumerable(), subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(ReadOnlySpan<T> values, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(values, subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(MpzT capacity, T[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(capacity, values.AsEnumerable(), subbranchesBitLength, leafSizeBitLength) { }

	public CustomBigList(MpzT capacity, ReadOnlySpan<T> values, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(capacity, values, subbranchesBitLength, leafSizeBitLength) { }

	protected override Func<MpzT, CustomBigList<T>> CapacityCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	protected override Func<G.IEnumerable<T>, CustomBigList<T>> CollectionCreator =>
		x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	protected override Func<int, LimitedBuffer<T>> CapacityLowCreator => x => new(x);

	protected override Func<G.IEnumerable<T>, LimitedBuffer<T>> CollectionLowCreator { get; } = x => new(x);
}
