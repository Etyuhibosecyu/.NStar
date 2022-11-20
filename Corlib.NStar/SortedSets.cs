using System.Diagnostics;

namespace Corlib.NStar;

[ComVisible(false)]
[DebuggerDisplay("Length = {Length}")]
[Serializable]
public abstract class SortedSetBase<T, TCertain> : SetBase<T, TCertain> where TCertain : SortedSetBase<T, TCertain>, new()
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

	public override TCertain Append(T item) => throw new NotSupportedException();

	private protected override int IndexOfInternal(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		int ret = Search(item);
		return ret >= index && ret < index + count ? ret : -1;
	}

	internal virtual int IndexOfNotLess(T item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		int ret = Search(item);
		return ret >= 0 ? ret : ~ret;
	}

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection) => throw new NotSupportedException();

	public override TCertain Insert(int index, T item) => throw new NotSupportedException();

	private protected abstract void InsertInternal(int index, T item);

	internal static int NthAbsent<TCertain2>(SortedSetBase<int, TCertain2> set, int n) where TCertain2 : SortedSetBase<int, TCertain2>, new()
	{
		if (set == null)
			throw new ArgumentNullException(nameof(set));
		if (set.Comparer != G.Comparer<int>.Default)
			throw new ArgumentException("Множество должно иметь стандартный для int компаратор.", nameof(set));
		if (set[0] < 0)
			throw new ArgumentException("Не допускается множество, содержащее отрицательные значения.", nameof(set));
		int lo = 0;
		int hi = set.Length - 1;
		var comparer = G.Comparer<int>.Default;
		while (lo <= hi)
		{
			// i might overflow if lo and hi are both large positive numbers. 
			int i = lo + ((hi - lo) >> 1);
			int c;
			try
			{
				c = comparer.Compare(set[i] - i, n);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(null, ex);
			}
			if (c == 0) return n + i + 1;
			if (c < 0)
			{
				lo = i + 1;
			}
			else
			{
				hi = i - 1;
			}
		}
		return n + lo;
	}

	internal override TCertain ReplaceRangeInternal(int index, int count, IEnumerable<T> collection) => throw new NotSupportedException();

	public abstract int Search(T item);

	internal override TCertain SetRangeInternal(int index, int count, TCertain list) => throw new NotSupportedException();

	public override TCertain Shuffle() => throw new NotSupportedException();
}

[DebuggerDisplay("Length = {Length}")]
[Serializable()]
[ComVisible(false)]
public abstract class SortedSet<T, TCertain> : SortedSetBase<T, TCertain> where TCertain : SortedSet<T, TCertain>, new()
{
	private protected readonly List<T> items;

	public SortedSet()
	{
		items = new();
		Comparer = G.Comparer<T>.Default;
	}

	public SortedSet(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		items = new(capacity);
		Comparer = G.Comparer<T>.Default;
	}

	public SortedSet(IComparer<T>? comparer) : this()
	{
		if (comparer != null)
			Comparer = comparer;
	}

	public SortedSet(Func<T, T, int> compareFunction) : this(new Comparer<T>(compareFunction))
	{
	}

	public SortedSet(int capacity, IComparer<T>? comparer) : this(comparer) => Capacity = capacity;

	public SortedSet(int capacity, Func<T, T, int> compareFunction) : this(capacity, new Comparer<T>(compareFunction))
	{
	}

	public SortedSet(IEnumerable<T> collection) : this(collection, null) { }

	public SortedSet(IEnumerable<T> collection, IComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetCountEasily(out int count) ? (int)(Sqrt(count) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		items.AddRange(collection).Sort(Comparer);
	}

	public SortedSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public SortedSet(int capacity, IEnumerable<T> collection, IComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		items.AddRange(collection).Sort(Comparer);
	}

	public SortedSet(params T[] array) : this((IEnumerable<T>)array)
	{
	}

	public SortedSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array)
	{
	}

	public SortedSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray())
	{
	}

	public SortedSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray())
	{
	}

	public override int Capacity { get => items.Capacity; set => items.Capacity = value; }

	public override IComparer<T> Comparer { get; }

	public override int Length => items.Length;

	private protected override void ClearInternal(int index, int count) => items.Clear(index, count);

	private protected override void Copy(ListBase<T, TCertain> source, int sourceIndex, ListBase<T, TCertain> destination, int destinationIndex, int length)
	{
		if (destination is not TCertain destinationSet)
			throw new InvalidOperationException();
		for (int i = 0; i < length; i++)
			destinationSet.items.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
	}

	private protected override void CopyToInternal(Array array, int arrayIndex) => items.CopyTo(array, arrayIndex);

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count) => items.CopyTo(index, array, arrayIndex, count);

	public override void Dispose()
	{
		items.Dispose();
		GC.SuppressFinalize(this);
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = items.GetInternal(index, invoke);
		if (invoke)
			Changed();
		return item;
	}

	private protected override void InsertInternal(int index, T item) => items.Insert(index, item);

	public override int Search(T item) => items.BinarySearch(item, Comparer);

	internal override void SetInternal(int index, T value)
	{
		items.SetInternal(index, value);
		Changed();
	}

	public override bool TryAdd(T item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		int i = Search(item);
		if (i >= 0)
			return false;
		InsertInternal(~i, item);
		return true;
	}
}

public class SortedSet<T> : SortedSet<T, SortedSet<T>>
{
	public SortedSet()
	{
	}

	public SortedSet(int capacity) : base(capacity)
	{
	}

	public SortedSet(IComparer<T>? comparer) : base(comparer)
	{
	}

	public SortedSet(Func<T, T, int> compareFunction) : base(compareFunction)
	{
	}

	public SortedSet(IEnumerable<T> collection) : base(collection)
	{
	}

	public SortedSet(params T[] array) : base(array)
	{
	}

	public SortedSet(ReadOnlySpan<T> span) : base(span)
	{
	}

	public SortedSet(int capacity, IComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public SortedSet(int capacity, Func<T, T, int> compareFunction) : base(capacity, compareFunction)
	{
	}

	public SortedSet(IEnumerable<T> collection, IComparer<T>? comparer) : base(collection, comparer)
	{
	}

	public SortedSet(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	public SortedSet(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public SortedSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public SortedSet(int capacity, IEnumerable<T> collection, IComparer<T>? comparer) : base(capacity, collection, comparer)
	{
	}

	private protected override Func<int, SortedSet<T>> CapacityCreator => capacity => new(capacity);

	private protected override Func<IEnumerable<T>, SortedSet<T>> CollectionCreator => collection => new(collection);
}