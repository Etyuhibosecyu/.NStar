namespace NStar.Core;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract partial class List<T, TCertain> : BaseList<T, TCertain> where TCertain : List<T, TCertain>, new()
{
	private protected T[]? _items;

	private protected static readonly SortedDictionary<int, G.List<T[]>> arrayPool = [];
	private protected static readonly object globalLockObj = new();

	public List() => _items = null;

	public List(int capacity) : this(capacity, false) { }

	public List(int capacity, bool exact)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity == 0)
		{
			_items = null;
			return;
		}
		lock (globalLockObj)
			_items = arrayPool.GetAndRemove(capacity, exact);
	}

	public List(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		int length;
		if (collection is not G.ICollection<T> c)
		{
			_size = 0;
			_items = collection.TryGetLengthEasily(out length) ? Lock(globalLockObj, () => arrayPool.Count == 0
				? GC.AllocateUninitializedArray<T>(length) : arrayPool.GetAndRemove(length)) : null;
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
			return;
		}
		length = c.Count;
		if (length == 0)
			_items = null;
		else
		{
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(length);
			c.CopyTo(_items, 0);
			_size = length;
		}
	}

	public List(int capacity, IEnumerable<T> collection) : this(capacity, collection, false) { }

	public List(int capacity, IEnumerable<T> collection, bool exact) : this(capacity, exact)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is not G.ICollection<T> c)
		{
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
			return;
		}
		var length = c.Count;
		if (length == 0)
			return;
		if (length > capacity)
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(length, exact);
		Debug.Assert(_items != null);
		c.CopyTo(_items, 0);
		_size = length;
	}

	public List(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		lock (globalLockObj)
		{
			_items = arrayPool.GetAndRemove(array.Length);
			array.CopyTo(_items, 0);
		}
	}

	public List(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		if (array.Length > capacity)
			lock (globalLockObj)
			{
				_items = arrayPool.GetAndRemove(array.Length);
				array.CopyTo(_items, 0);
			}
		else
		{
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(capacity);
			Array.Copy(array, _items, array.Length);
		}
	}

	public List(ReadOnlySpan<T> span)
	{
		_size = span.Length;
		lock (globalLockObj)
		{
			_items = arrayPool.GetAndRemove(span.Length);
			span.CopyTo(_items);
		}
	}

	public List(int capacity, ReadOnlySpan<T> span) : this(capacity, span, false) { }

	public List(int capacity, ReadOnlySpan<T> span, bool exact)
	{
		_size = span.Length;
		if (span.Length > capacity)
			lock (globalLockObj)
			{
				_items = arrayPool.GetAndRemove(span.Length, exact);
				span.CopyTo(_items);
			}
		else
		{
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(capacity, exact);
			span.CopyTo(_items);
		}
	}

	public override int Capacity
	{
		get => _items?.Length ?? 0;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			if (value == (_items?.Length ?? 0))
				return;
			if (value > 0)
			{
				T[] newItems;
				lock (globalLockObj)
					newItems = arrayPool.GetAndRemove(value, true);
				if (_size > 0 && _items != null)
					Array.Copy(_items, 0, newItems, 0, _size);
				_items = newItems;
			}
			else
				_items = null;
			Changed();
		}
	}

	public override Memory<T> AsMemory(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		return MemoryExtensions.AsMemory(_items, index, length);
	}

	public override Span<T> AsSpan(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		return MemoryExtensions.AsSpan(_items, index, length);
	}

	public virtual int BinarySearch(int index, int length, T item) => BinarySearch(index, length, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(int index, int length, T item, IComparer<T> comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (_items == null)
			return -1;
		return Array.BinarySearch(_items, index, length, item, comparer);
	}

	public virtual int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	protected override void ClearInternal(int index, int length)
	{
		if (_items == null)
			return;
		Array.Clear(_items, index, length);
		Changed();
	}

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (_items == null || destination._items == null)
			return;
		Array.Copy(_items, sourceIndex, destination._items, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		Changed();
	}

	protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (_items == null)
			return;
		Array.Copy(_items, 0, array, arrayIndex, _size);
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		if (_items == null)
			return;
		Array.Copy(_items, index, array, arrayIndex, length);
	}

	public override void Dispose()
	{
		if (_items == null)
			return;
		lock (globalLockObj)
		{
			if (arrayPool.TryGetValue(_items.Length, out var list))
				list.Add(_items);
			else
				arrayPool.Add(_items.Length, new(32) { _items });
		}
		_items = null;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal static List<T> EmptyList(int length) => new(length) { _size = length };

	public static unsafe TCertain FromPointer(int capacity, void* ptr)
	{
		if (ptr == null)
			throw new ArgumentNullException(nameof(ptr));
		TCertain list = [];
		list._size = capacity;
		list._items = GC.AllocateUninitializedArray<T>(capacity);
		new Span<T>(ptr, list._size).CopyTo(list._items);
		return list;
	}

	protected override T GetInternal(int index, bool invoke = true)
	{
		Debug.Assert(_items != null);
		var item = _items[index];
		if (invoke)
			Changed();
		return item;
	}

	protected override int IndexOfInternal(T item, int index, int length)
	{
		if (_items == null)
			return -1;
		return Array.IndexOf(_items, item, index, length);
	}

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
		{
			var min = _size + 1;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = GC.AllocateUninitializedArray<T>(newCapacity);
			if (index > 0 && _items != null)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size && _items != null)
				Array.Copy(_items, index, newItems, index + 1, _size - index);
			_size++;
			newItems[index] = item;
			_items = newItems;
		}
		else
		{
			if (index < _size)
				CopyToInternal(index, (TCertain)this, index + 1, _size - index);
			else
				_size++;
			Debug.Assert(_items != null);
			_items[index] = item;
		}
		Changed();
		return (TCertain)this;
	}

	protected override TCertain InsertInternal(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = span.Length;
		if (length == 0)
			return (TCertain)this;
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = GC.AllocateUninitializedArray<T>(newCapacity);
			if (index > 0 && _items != null)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size && _items != null)
				Array.Copy(_items, index, newItems, index + length, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(newItems, index));
			_items = newItems;
		}
		else
		{
			if (index < _size && _items != null)
				Array.Copy(_items, index, _items, index + length, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(_items, index));
		}
		_size += length;
		Changed();
		return (TCertain)this;
	}

	protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is List<T, TCertain> list)
		{
			var length = list._size;
			if (length == 0)
				return (TCertain)this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = GC.AllocateUninitializedArray<T>(newCapacity);
				if (index > 0 && _items != null)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size && _items != null)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				if (list._items != null)
					Array.Copy(list._items, 0, newItems, index, length);
				_items = newItems;
			}
			else
			{
				if (index < _size && _items != null)
					Array.Copy(_items, index, _items, index + length, _size - index);
				if (this == list && _items != null)
				{
					Array.Copy(_items, 0, _items, index, index);
					Array.Copy(_items, index + length, _items, index * 2, _size - index);
				}
				else if (list._items != null)
				{
					Debug.Assert(_items != null);
					Array.Copy(list._items, 0, _items, index, length);
				}
			}
			_size += length;
			Changed();
			return (TCertain)this;
		}
		else if (collection is T[] array)
		{
			var length = array.Length;
			if (length == 0)
				return (TCertain)this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = GC.AllocateUninitializedArray<T>(newCapacity);
				if (index > 0 && _items != null)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size && _items != null)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				Array.Copy(array, 0, newItems, index, length);
				_items = newItems;
			}
			else
			{
				if (index < _size && _items != null)
					Array.Copy(_items, index, _items, index + length, _size - index);
				Debug.Assert(_items != null);
				Array.Copy(array, 0, _items, index, length);
			}
			_size += length;
			Changed();
			return (TCertain)this;
		}
		else if (collection is G.ICollection<T> list2)
		{
			var length = list2.Count;
			if (length == 0)
				return (TCertain)this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = GC.AllocateUninitializedArray<T>(newCapacity);
				if (index > 0 && _items != null)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size && _items != null)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				list2.CopyTo(newItems, index);
				_items = newItems;
			}
			else
			{
				if (index < _size && _items != null)
					Array.Copy(_items, index, _items, index + length, _size - index);
				Debug.Assert(_items != null);
				list2.CopyTo(_items, index);
			}
			_size += length;
			Changed();
			return (TCertain)this;
		}
		else
			return InsertInternal(index, CollectionCreator(collection));
	}

	protected override int LastIndexOfInternal(T item, int index, int length)
	{
		if (_items == null)
			return -1;
		return Array.LastIndexOf(_items, item, index, length);
	}

	public virtual TCertain NSort() => NSort(0, _size);

	public unsafe virtual TCertain NSort(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return (TCertain)this;
		if (this is List<byte> byteList)
		{
			Debug.Assert(byteList._items != null);
			byteList._items.NSort(index, length);
			return (TCertain)this;
		}
		else if (this is List<ushort> ushortList)
		{
			Debug.Assert(ushortList._items != null);
			ushortList._items.NSort(index, length);
			return (TCertain)this;
		}
		else if (this is List<uint> uintList)
		{
			Debug.Assert(uintList._items != null);
			uintList._items.NSort(index, length);
			return (TCertain)this;
		}
		else
			return Sort(index, length, G.Comparer<T>.Default);
	}

	public virtual TCertain NSort(Func<T, uint> function) => NSort(function, 0, _size);

	public unsafe virtual TCertain NSort(Func<T, uint> function, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (_items == null)
			return (TCertain)this;
		_items.NSort(function, index, length);
		return (TCertain)this;
	}

	public static List<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) => collection is List<TList> list ? list : [.. collection];

	protected override void ReverseInternal(int index, int length)
	{
		if (_items == null)
			return;
		Array.Reverse(_items, index, length);
		Changed();
	}

	protected override void SetInternal(int index, T value)
	{
		if (_items == null)
			return;
		_items[index] = value;
		Changed();
	}

	public virtual TCertain Sort() => Sort(0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort(IComparer<T> comparer) => Sort(0, _size, comparer);

	public virtual TCertain Sort(int index, int length, IComparer<T> comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (_items == null)
			return (TCertain)this;
		Array.Sort(_items, index, length, comparer);
		return (TCertain)this;
	}

	public virtual TCertain Sort<TValue>(Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(0, _size, function, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(index, length, function, G.Comparer<TValue>.Default, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, IComparer<TValue> comparer, bool fasterButMoreMemory = true)
	{
		if (fasterButMoreMemory)
		{
			RedStarLinq.ToList(this, function).Sort(this, index, length, comparer);
			return (TCertain)this;
		}
		else
			return Sort(index, length, new Comparer<T>((x, y) => comparer.Compare(function(x), function(y))));
	}

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, comparer);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, int index, int length, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new()
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (index + length > values._size)
			throw new ArgumentException("Сортируемый диапазон выходит за размер экстра-коллекции.");
		if (_items == null)
			return (TCertain)this;
		Array.Sort(_items, values._items, index, length, comparer);
		return (TCertain)this;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class List<T> : List<T, List<T>>
{
	public List() { }

	public List(int capacity) : base(capacity) { }

	public List(int capacity, bool exact) : base(capacity, exact) { }

	public List(IEnumerable<T> collection) : base(collection) { }

	public List(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public List(int capacity, IEnumerable<T> collection, bool exact) : base(capacity, collection, exact) { }

	public List(int capacity, params T[] array) : base(capacity, array) { }

	public List(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public List(int capacity, ReadOnlySpan<T> span, bool exact) : base(capacity, span, exact) { }

	public List(params T[] array) : base(array) { }

	public List(ReadOnlySpan<T> span) : base(span) { }

	protected override Func<int, List<T>> CapacityCreator { get; } = x => new(x);

	protected override Func<IEnumerable<T>, List<T>> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<T>, List<T>> SpanCreator { get; } = x => new(x);

	public static implicit operator List<T>(T x) => new(x);

	public static implicit operator List<T>(T[] x) => new(x);

	public static implicit operator List<T>((T, T) x) => [x.Item1, x.Item2];

	public static implicit operator List<T>((T, T, T) x) => [x.Item1, x.Item2, x.Item3];

	public static implicit operator List<T>((T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static implicit operator List<T>((T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static implicit operator List<T>((T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static implicit operator List<T>((T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (T, T)(List<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException("Список должен иметь 2 элемента.");

	public static explicit operator (T, T, T)(List<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException("Список должен иметь 3 элемента.");

	public static explicit operator (T, T, T, T)(List<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException("Список должен иметь 4 элемента.");

	public static explicit operator (T, T, T, T, T)(List<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException("Список должен иметь 5 элементов.");

	public static explicit operator (T, T, T, T, T, T)(List<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException("Список должен иметь 6 элементов.");

	public static explicit operator (T, T, T, T, T, T, T)(List<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException("Список должен иметь 7 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T)(List<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException("Список должен иметь 8 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException("Список должен иметь 9 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException("Список должен иметь 10 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException("Список должен иметь 11 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException("Список должен иметь 12 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException("Список должен иметь 13 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException("Список должен иметь 14 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException("Список должен иметь 15 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException("Список должен иметь 16 элементов.");
}
