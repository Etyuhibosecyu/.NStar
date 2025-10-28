namespace NStar.Core;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract unsafe partial class NList<T, TCertain> : BaseList<T, TCertain> where T : unmanaged where TCertain : NList<T, TCertain>, new()
{
	private protected T* _items;
	private protected int _capacity;
	private protected bool _isRange;

	public NList() => _items = null;

	public NList(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_items = capacity == 0 ? null : (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = capacity));
	}

	public NList(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				_items = null;
			else
			{
				_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = length));
				fixed (T* ptr = c.AsSpan())
					CopyMemory(ptr, _items, c.Count);
				_size = length;
			}
		}
		else
		{
			_size = 0;
			_items = collection.TryGetLengthEasily(out var length) ? (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = length)) : null;
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public NList(int capacity, IEnumerable<T> collection) : this(capacity)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				return;
			if (length > capacity)
			{
				if (capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = length));
			}
			fixed (T* ptr = c.AsSpan())
				CopyMemory(ptr, _items, c.Count);
			_size = length;
		}
		else
		{
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public NList(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_capacity = _size = array.Length;
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = array)
			CopyMemory(ptr, _items, _size);
	}

	public NList(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_capacity = Max(capacity, _size = array.Length);
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = array)
			CopyMemory(ptr, _items, _size);
	}

	public NList(int capacity, T* ptr)
	{
		if (ptr == null)
			throw new ArgumentNullException(nameof(ptr));
		_size = capacity;
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = capacity));
		CopyMemory(ptr, _items, _capacity);
	}

	public NList(ReadOnlySpan<T> span)
	{
		_capacity = _size = span.Length;
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = span)
			CopyMemory(ptr, _items, _size);
	}

	public NList(int capacity, ReadOnlySpan<T> span)
	{
		_capacity = Max(capacity, _size = span.Length);
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = span)
			CopyMemory(ptr, _items, _size);
	}

	public NList(T item)
	{
		_capacity = _size = 1;
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		_items[0] = item;
	}

	public NList(int capacity, T item)
	{
		_capacity = Max(capacity, _size = 1);
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		_items[0] = item;
	}

	public override int Capacity
	{
		get => _capacity;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			if (value == _capacity)
				return;
			if (_isRange)
				throw new InvalidOperationException("Изменение нативного списка, являющегося диапазоном другого списка,"
					+ " запрещено. Если вы хотите создать список из диапазона другого списка, а затем изменять его,"
					+ " в методе GetRange() установите параметр alwaysCopy в true.");
			if (value > 0)
			{
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * value);
				if (_size > 0)
					CopyMemory(_items, newItems, _size);
				if (_capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
			}
			else
			{
				if (_capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = null;
			}
			_capacity = value;
			Changed();
		}
	}

	public override Span<T> AsSpan(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		return new(_items + index, length);
	}

	public virtual int BinarySearch(int index, int length, T item) => BinarySearch(index, length, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(int index, int length, T item, IComparer<T> comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		return new Span<T>(_items + index, length).BinarySearch(item, comparer);
	}

	public virtual int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	protected override void ClearInternal(int index, int length)
	{
		FillMemory(_items + index, length, 0);
		Changed();
	}

	protected override int CompareInternal(int index, TCertain other, int otherIndex, int length) => CompareMemory(_items + index, other._items + otherIndex, length);

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		CopyMemory(_items, sourceIndex, destination._items, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		Changed();
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		fixed (T* ptr = array)
			CopyMemory(_items, index, ptr, arrayIndex, length);
	}

	public override void Dispose()
	{
		if (_isRange)
			throw new InvalidOperationException("Уничтожение нативного списка, являющегося диапазоном другого списка,"
				+ " запрещено. Если вы хотите создать список из диапазона другого списка, а затем уничтожить его,"
				+ " в методе GetRange() установите параметр alwaysCopy в true.");
		if (_capacity != 0)
			Marshal.FreeHGlobal((nint)_items);
		_capacity = 0;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal static NList<T> EmptyList(int length) => new(length) { _size = length };

	protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (collection == null)
			return false;
		if (collection is not G.IList<T>)
			return base.EqualsInternal(collection, index, toEnd);
		if (collection is TCertain nList)
		{
			if (index > _size - nList.Length)
				return false;
			if (toEnd && index < _size - nList.Length)
				return false;
			T* leftptr = _items + index, rightptr = nList._items;
			return EqualMemory(leftptr, rightptr, nList.Length);
		}
		if (collection is T[] array)
		{
			if (index > _size - array.Length)
				return false;
			if (toEnd && index < _size - array.Length)
				return false;
			var leftptr = _items + index;
			fixed (T* rightptr = array)
				return EqualMemory(leftptr, rightptr, array.Length);
		}
		if (collection is List<T> list)
		{
			if (index > _size - list.Length)
				return false;
			if (toEnd && index < _size - list.Length)
				return false;
			var leftptr = _items + index;
			fixed (T* rightptr = list.AsSpan())
				return EqualMemory(leftptr, rightptr, list.Length);
		}
		return base.EqualsInternal(collection, index, toEnd);
	}

	protected override T GetInternal(int index, bool invoke = true)
	{
		var item = _items[index];
		if (invoke)
			Changed();
		return item;
	}

	public override TCertain GetRange(int index, int length, bool alwaysCopy = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return [];
		else if (!alwaysCopy && index == 0 && length == _size)
			return (TCertain)this;
		if (!alwaysCopy)
		{
			var list = CapacityCreator(0);
			list._capacity = length;
			list._items = _items + index;
			list._size = length;
			list._isRange = true;
			return list;
		}
		else
		{
			var list = CapacityCreator(length);
			list._size = length;
			CopyMemory(_items + index, list._items, length);
			return list;
		}
	}

	protected override int IndexOfInternal(T item, int index, int length)
	{
		var ptr = _items + index;
		for (var i = 0; i < length; i++)
			if (ptr[i].Equals(item))
				return index + i;
		return -1;
	}

	public override TCertain Insert(int index, T item)
	{
		if (_isRange)
			throw new InvalidOperationException("Изменение нативного списка, являющегося диапазоном другого списка,"
				+ " запрещено. Если вы хотите создать список из диапазона другого списка, а затем изменять его,"
				+ " в методе GetRange() установите параметр alwaysCopy в true.");
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		if (_size == Capacity)
		{
			var min = _size + 1;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + 1, _size - index);
			newItems[index] = item;
			if (_capacity != 0)
				Marshal.FreeHGlobal((nint)_items);
			_items = newItems;
			_capacity = newCapacity;
			_size++;
		}
		else
		{
			if (index < _size)
				CopyToInternal(index, this2, index + 1, _size - index);
			else
				_size++;
			_items[index] = item;
		}
		Changed();
		return this2;
	}

	protected override TCertain InsertInternal(int index, ReadOnlySpan<T> span)
	{
		if (_isRange)
			throw new InvalidOperationException("Изменение нативного списка, являющегося диапазоном другого списка,"
				+ " запрещено. Если вы хотите создать список из диапазона другого списка, а затем изменять его,"
				+ " в методе GetRange() установите параметр alwaysCopy в true.");
		var length = span.Length;
		var this2 = (TCertain)this;
		if (length == 0)
			return this2;
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + length, _size - index);
			span.CopyTo(new(newItems + index, newCapacity - index));
			if (_capacity != 0)
				Marshal.FreeHGlobal((nint)_items);
			_items = newItems;
			_capacity = newCapacity;
		}
		else
		{
			if (index < _size)
				CopyMemory(_items, index, _items, index + length, _size - index);
			span.CopyTo(new(_items + index, Capacity - index));
		}
		_size += length;
		Changed();
		return this2;
	}

	protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (_isRange)
			throw new InvalidOperationException("Изменение нативного списка, являющегося диапазоном другого списка,"
				+ " запрещено. Если вы хотите создать список из диапазона другого списка, а затем изменять его,"
				+ " в методе GetRange() установите параметр alwaysCopy в true.");
		var this2 = (TCertain)this;
		if (collection is TCertain list)
		{
			var length = list._size;
			if (length == 0)
				return this2;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
				if (this == list)
				{
					CopyMemory(newItems, 0, newItems, index, index);
					CopyMemory(newItems, index + length, newItems, index * 2, _size - index);
				}
				else
					CopyMemory(list._items, 0, newItems, index, length);
				if (_capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
				_capacity = newCapacity;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + length, _size - index);
				if (this == list)
				{
					CopyMemory(_items, 0, _items, index, index);
					CopyMemory(_items, index + length, _items, index * 2, _size - index);
				}
				else
					CopyMemory(list._items, 0, _items, index, length);
			}
			_size += length;
			Changed();
			return this2;
		}
		else if (collection is T[] array)
		{
			var length = array.Length;
			if (length == 0)
				return this2;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
				fixed (T* ptr = array)
					CopyMemory(ptr, 0, newItems, index, length);
				if (_capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
				_capacity = newCapacity;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + length, _size - index);
				fixed (T* ptr = array)
					CopyMemory(ptr, 0, _items, index, length);
			}
			_size += length;
			Changed();
			return this2;
		}
		else if (collection is G.ICollection<T> list2)
		{
			var length = list2.Count;
			if (length == 0)
				return this2;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
				fixed (T* ptr = list2.AsSpan())
					CopyMemory(ptr, 0, newItems, index, list2.Count);
				if (_capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
				_capacity = newCapacity;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + length, _size - index);
				fixed (T* ptr = list2.AsSpan())
					CopyMemory(ptr, 0, _items, index, list2.Count);
			}
			_size += length;
			Changed();
			return this2;
		}
		else
			return InsertInternal(index, CollectionCreator(collection));
	}

	protected override int LastIndexOfInternal(T item, int index, int length)
	{
		var endIndex = index - length + 1;
		for (var i = index; i >= endIndex; i--)
			if (_items[i].Equals(item))
				return i;
		return -1;
	}

	public static NList<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) where TList : unmanaged => collection is NList<TList> list ? list : [.. collection];

	protected override void ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
			(_items[index + i], _items[index + length - 1 - i]) = (_items[index + length - 1 - i], _items[index + i]);
		Changed();
	}

	protected override void SetInternal(int index, T value)
	{
		_items[index] = value;
		Changed();
	}

	public virtual TCertain Sort() => Sort(0, _size);

	public virtual TCertain Sort(int index, int length)
	{
		var shiftedItems = _items + index;
		RadixSort(shiftedItems, length);
		return (TCertain)this;
	}

	public virtual TCertain Sort(Func<T, byte> function) => Sort(function, 0, _size);

	public virtual TCertain Sort(Func<T, byte> function, int index, int length)
	{
		NSort(_items, function, index, length);
		return (TCertain)this;
	}

	public virtual TCertain Sort(Func<T, uint> function) => Sort(function, 0, _size);

	public virtual TCertain Sort(Func<T, uint> function, int index, int length)
	{
		NSort(_items, function, index, length);
		return (TCertain)this;
	}

	public virtual TCertain Sort(Func<T, ushort> function) => Sort(function, 0, _size);

	public virtual TCertain Sort(Func<T, ushort> function, int index, int length)
	{
		NSort(_items, function, index, length);
		return (TCertain)this;
	}
}

public unsafe class NList<T> : NList<T, NList<T>> where T : unmanaged
{
	public NList()
	{
	}

	public NList(int capacity) : base(capacity)
	{
	}

	public NList(IEnumerable<T> collection) : base(collection)
	{
	}

	public NList(params T[] array) : base(array)
	{
	}

	public NList(ReadOnlySpan<T> span) : base(span)
	{
	}

	public NList(T item) : base(item)
	{
	}

	public NList(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	public NList(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public NList(int capacity, T* ptr) : base(capacity, ptr)
	{
	}

	public NList(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public NList(int capacity, T item) : base(capacity, item)
	{
	}

	protected override Func<int, NList<T>> CapacityCreator { get; } = x => new(x);

	protected override Func<IEnumerable<T>, NList<T>> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<T>, NList<T>> SpanCreator { get; } = x => new(x);

	public static implicit operator NList<T>(T x) => new(x);

	public static implicit operator NList<T>((T, T) x) => [x.Item1, x.Item2];

	public static implicit operator NList<T>((T, T, T) x) => [x.Item1, x.Item2, x.Item3];

	public static implicit operator NList<T>((T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static implicit operator NList<T>((T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static implicit operator NList<T>((T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static implicit operator NList<T>((T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static implicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (T, T)(NList<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException("Список должен иметь 2 элемента.");

	public static explicit operator (T, T, T)(NList<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException("Список должен иметь 3 элемента.");

	public static explicit operator (T, T, T, T)(NList<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException("Список должен иметь 4 элемента.");

	public static explicit operator (T, T, T, T, T)(NList<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException("Список должен иметь 5 элементов.");

	public static explicit operator (T, T, T, T, T, T)(NList<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException("Список должен иметь 6 элементов.");

	public static explicit operator (T, T, T, T, T, T, T)(NList<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException("Список должен иметь 7 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException("Список должен иметь 8 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException("Список должен иметь 9 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException("Список должен иметь 10 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException("Список должен иметь 11 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException("Список должен иметь 12 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException("Список должен иметь 13 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException("Список должен иметь 14 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException("Список должен иметь 15 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException("Список должен иметь 16 элементов.");
}
