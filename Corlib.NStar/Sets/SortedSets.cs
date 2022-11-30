using System.Collections.Generic;
using System.Diagnostics;

namespace Corlib.NStar;

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
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

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
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

	private protected override Func<int, SortedSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, SortedSet<T>> CollectionCreator => x => new(x);
}

//public class BalancedSet<T, TCertain> : SortedSetBase<T, TCertain> where TCertain : BalancedSet<T, TCertain>, new()
//{
//	private INode root;
//	private protected int _capacity;
//	private protected int fragment = 1;

//	public override int Capacity
//	{
//		get => _capacity;
//		set
//		{
//			if (value < _size)
//				throw new ArgumentOutOfRangeException(nameof(value));
//			if (value == _capacity)
//				return;
//			if (value <= 0)
//			{
//				root = (INode<T>)new SortedSet<T>();
//			}
//			else if (value <= CapacityFirstStep && root is Leaf<T> leaf)
//			{
//				try
//				{
//					throw new ExperimentalException();
//				}
//				catch
//				{
//				}
//				root = (INode<T>)new SortedSet<T>(value, leaf.items);
//			}
//			else if (root is Leaf<T> leaf2)
//			{
//				fragment = (int)1 << ((((value - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
//				SortedSet<BalancedSet<T, TCertain>.INode> set = new((int)((value + (fragment - 1)) / fragment));
//				for (int i = 0; i < value / fragment; i++)
//					set.Add((INode)new SortedSet<T>(fragment));
//				if (value % fragment != 0)
//					set.Add((INode)new SortedSet<T>(value % fragment));
//				set[0].AddRange(low);
//				low = null;
//			}
//			else if (high != null)
//			{
//				high.Capacity = (int)((value + fragment - 1) / fragment);
//				high[^1].Capacity = (high.Length < high.Capacity || value % fragment == 0) ? fragment : value % fragment;
//				for (int i = high.Length; i < high.Capacity - 1; i++)
//					high.Add(CapacityCreator(fragment));
//				if (high.Length < high.Capacity)
//					high.Add(CapacityCreator(value % fragment == 0 ? fragment : value % fragment));
//			}
//			_capacity = value;
//		}
//	}

//	private protected virtual int CapacityStepBitLength => 5;

//	private protected virtual int CapacityFirstStepBitLength => 5;

//	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;
//}

//file interface INode<T> : IEnumerable<T>
//{
//	int Length { get; }

//	void Add(T item);

//	void AddRange(IEnumerable<T> collection);

//	void Clear();

//	SortedSet<T> GetRange(int index, int count);

//	void Insert(int index, T item);

//	void Remove(int index);

//	void Remove(int index, int count);

//	void RemoveAt(int index);

//	void RemoveValue(T item);
//}

//file struct Leaf<T> : INode<T>
//{
//	public SortedSet<T> items;

//	public Leaf(SortedSet<T> items) => this.items = items;

//	public int Length => items.Length;

//	public void Add(T item) => items.Add(item);

//	public void AddRange(IEnumerable<T> collection) => items.AddRange(collection);

//	public void Clear() => items.Clear();

//	public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

//	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

//	public SortedSet<T> GetRange(int index, int count) => items.GetRange(index, count);

//	public void Insert(int index, T item) => items.Insert(index, item);

//	public void Remove(int index) => items.Remove(index);

//	public void Remove(int index, int count) => items.Remove(index, count);

//	public void RemoveAt(int index) => items.RemoveAt(index);

//	public void RemoveValue(T item) => items.RemoveValue(item);

//	public static implicit operator Leaf<T>(SortedSet<T> items) => new(items);
//}

//file struct Branch<T> : INode<T>
//{
//	public SortedSet<INode<T>> nodes;

//	public Branch(SortedSet<INode<T>> nodes) => this.nodes = nodes;

//	public static implicit operator Branch<T>(SortedSet<INode<T>> nodes) => new(nodes);
//}
