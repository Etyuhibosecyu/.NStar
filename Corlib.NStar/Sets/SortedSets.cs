namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseSortedSet<T, TCertain> : BaseSet<T, TCertain> where TCertain : BaseSortedSet<T, TCertain>, new()
{
	public override T this[Index index, bool invoke = true]
	{
		get => base[index, invoke];
		set => throw new NotSupportedException();
	}

	public abstract IComparer<T> Comparer { get; }

	public override TCertain Add(T item)
	{
		TryAdd(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected override int IndexOfInternal(T item, int index, int length)
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

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection) => throw new NotSupportedException();

	public override TCertain Insert(int index, T item) => throw new NotSupportedException();

	internal abstract void InsertInternal(int index, T item);

	internal override TCertain ReplaceRangeInternal(int index, int length, IEnumerable<T> collection) => throw new NotSupportedException();

	public abstract int Search(T item);

	internal override TCertain SetRangeInternal(int index, int length, TCertain list) => throw new NotSupportedException();

	public override TCertain Shuffle() => throw new NotSupportedException();

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

	public SortedSet(IEnumerable<T> collection, IComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
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

	private protected override void ClearInternal(int index, int length) => items.Clear(index, length);

	private protected override void CopyToInternal(Array array, int arrayIndex) => items.CopyTo(array, arrayIndex);

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => items.CopyTo(index, array, arrayIndex, length);

	public override void Dispose()
	{
		items.Dispose();
		GC.SuppressFinalize(this);
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = items.GetInternal(index, invoke);
		if (invoke)
			Changed();
		return item;
	}

	internal override void InsertInternal(int index, T item) => items.Insert(index, item);

	public override int Search(T item) => items.BinarySearch(item, Comparer);

	internal override void SetInternal(int index, T value)
	{
		items.SetInternal(index, value);
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

	private protected override Func<int, SortedSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, SortedSet<T>> CollectionCreator => x => new(x);
}

internal delegate bool TreeWalkPredicate<T>(TreeSet<T>.Node node);

internal enum NodeColor : byte
{
	Black,
	Red
}

internal enum TreeRotation
{
	Left = 1,
	Right = 2,
	RightLeft = 3,
	LeftRight = 4,
}

internal ref struct BitHelper
{
	private const int IntSize = sizeof(int) * 8;
	private readonly Span<int> _span;

	internal BitHelper(Span<int> span, bool clear)
	{
		if (clear)
			span.Clear();
		_span = span;
	}

	internal readonly bool IsMarked(int bitPosition)
	{
		Debug.Assert(bitPosition >= 0);
		var bitArrayIndex = (uint)bitPosition / IntSize;
		// Workaround for https://github.com/dotnet/runtime/issues/72004
		var span = _span;
		return bitArrayIndex < (uint)span.Length && (span[(int)bitArrayIndex] & (1 << ((int)((uint)bitPosition % IntSize)))) != 0;
	}

	internal readonly void MarkBit(int bitPosition)
	{
		Debug.Assert(bitPosition >= 0);
		var bitArrayIndex = (uint)bitPosition / IntSize;
		// Workaround for https://github.com/dotnet/runtime/issues/72004
		var span = _span;
		if (bitArrayIndex < (uint)span.Length)
			span[(int)bitArrayIndex] |= 1 << (int)((uint)bitPosition % IntSize);
	}

	/// <summary>How many ints must be allocated to represent n bits. Returns (n+31)/32, but avoids overflow.</summary>
	internal static int ToIntArrayLength(int n) => n > 0 ? ((n - 1) / IntSize + 1) : 0;
}
