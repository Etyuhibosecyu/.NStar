namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseSortedSet<T, TCertain> : BaseSet<T, TCertain> where TCertain : BaseSortedSet<T, TCertain>, new()
{
	public override T this[Index index, bool invoke = true]
	{
		get => base[index, invoke];
		set => throw new NotSupportedException("Это действие не поддерживается в этой коллекции."
			+ " Если оно нужно вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");
	}

	public abstract IComparer<T> Comparer { get; }

	public override TCertain Add(T item)
	{
		TryAdd(item);
		return (TCertain)this;
	}

	protected override int IndexOfInternal(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var ret = Search(item);
		return ret >= index && ret < index + length ? ret : -1;
	}

	public virtual int IndexOfNotLess(T item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var ret = Search(item);
		return ret >= 0 ? ret : ~ret;
	}

	protected override TCertain InsertInternal(int index, IEnumerable<T> collection) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public override TCertain Insert(int index, T item) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	protected internal abstract void InsertInternal(int index, T item);

	internal override TCertain ReplaceRangeInternal(int index, int length, IEnumerable<T> collection) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public abstract int Search(T item);

	internal override TCertain SetRangeInternal(int index, int length, TCertain list) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public override TCertain Shuffle() =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public override bool TryAdd(T item, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		index = Search(item);
		if (index >= 0)
			return false;
		InsertInternal(index = ~index, item);
		return true;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class SortedSet<T, TCertain> : BaseSortedSet<T, TCertain> where TCertain : SortedSet<T, TCertain>, new()
{
	private protected readonly List<T> items;

	public SortedSet() : this(G.Comparer<T>.Default) { }

	public SortedSet(int capacity) : this(capacity, G.Comparer<T>.Default) { }

	public SortedSet(IComparer<T>? comparer)
	{
		items = [];
		Comparer = comparer ?? G.Comparer<T>.Default;
	}

	public SortedSet(Func<T, T, int> compareFunction) : this(new Comparer<T>(compareFunction)) { }

	public SortedSet(int capacity, IComparer<T>? comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		items = new(capacity);
		Comparer = comparer ?? G.Comparer<T>.Default;
	}

	public SortedSet(int capacity, Func<T, T, int> compareFunction) : this(capacity, new Comparer<T>(compareFunction)) { }

	public SortedSet(IEnumerable<T> collection) : this(collection, null) { }

	public SortedSet(IEnumerable<T> collection, IComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		items.AddRange(collection).Sort(Comparer);
	}

	public SortedSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public SortedSet(int capacity, IEnumerable<T> collection, IComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		items.AddRange(collection).Sort(Comparer);
	}

	public SortedSet(params T[] array) : this((IEnumerable<T>)array) { }

	public SortedSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public SortedSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public SortedSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override int Capacity { get => items.Capacity; set => items.Capacity = value; }

	public override IComparer<T> Comparer { get; }

	public override int Length => items.Length;

	protected override void ClearInternal(int index, int length) => items.Clear(index, length);

	protected override void CopyToInternal(Array array, int arrayIndex) => items.CopyTo(array, arrayIndex);

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => items.CopyTo(index, array, arrayIndex, length);

	public override void Dispose()
	{
		items.Dispose();
		GC.SuppressFinalize(this);
	}

	protected override T GetInternal(int index, bool invoke = true)
	{
		var item = items[index, invoke];
		if (invoke)
			Changed();
		return item;
	}

	protected internal override void InsertInternal(int index, T item) => items.Insert(index, item);

	public override int Search(T item) => items.BinarySearch(item, Comparer);

	protected override void SetInternal(int index, T value)
	{
		items[index] = value;
		Changed();
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class SortedSet<T> : SortedSet<T, SortedSet<T>>
{
	public SortedSet() { }

	public SortedSet(int capacity) : base(capacity) { }

	public SortedSet(IComparer<T>? comparer) : base(comparer) { }

	public SortedSet(Func<T, T, int> compareFunction) : base(compareFunction) { }

	public SortedSet(IEnumerable<T> collection) : base(collection) { }

	public SortedSet(params T[] array) : base(array) { }

	public SortedSet(ReadOnlySpan<T> span) : base(span) { }

	public SortedSet(int capacity, IComparer<T>? comparer) : base(capacity, comparer) { }

	public SortedSet(int capacity, Func<T, T, int> compareFunction) : base(capacity, compareFunction) { }

	public SortedSet(IEnumerable<T> collection, IComparer<T>? comparer) : base(collection, comparer) { }

	public SortedSet(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public SortedSet(int capacity, params T[] array) : base(capacity, array) { }

	public SortedSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public SortedSet(int capacity, IEnumerable<T> collection, IComparer<T>? comparer) : base(capacity, collection, comparer) { }

	protected override Func<int, SortedSet<T>> CapacityCreator => x => new(x);

	protected override Func<IEnumerable<T>, SortedSet<T>> CollectionCreator => x => new(x);

	protected override Func<ReadOnlySpan<T>, SortedSet<T>> SpanCreator => x => new(x);
}

internal delegate bool TreeWalkPredicate<T>(TreeSet<T>.Node node);
