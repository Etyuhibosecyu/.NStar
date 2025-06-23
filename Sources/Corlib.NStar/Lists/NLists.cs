using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract unsafe partial class NList<T, TCertain> : BaseList<T, TCertain> where T : unmanaged where TCertain : NList<T, TCertain>, new()
{
	private protected T* _items;
	private protected int _capacity;
	private protected bool _isRange;

	private protected static readonly T* _emptyArray = null;

	public NList() => _items = _emptyArray;

	public NList(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_items = capacity == 0 ? _emptyArray : (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = capacity));
	}

	public NList(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				_items = _emptyArray;
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
			_items = collection.TryGetLengthEasily(out var length) ? (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = length)) : _emptyArray;
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
				_items = _emptyArray;
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

	public virtual TCertain Sort(Func<T, uint> function) => Sort(function, 0, _size);

	public virtual TCertain Sort(Func<T, uint> function, int index, int length)
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

	protected override Func<int, NList<T>> CapacityCreator => x => new(x);

	protected override Func<IEnumerable<T>, NList<T>> CollectionCreator => x => new(x);

	protected override Func<ReadOnlySpan<T>, NList<T>> SpanCreator => x => new(x);

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

	public static explicit operator (T, T)(NList<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T)(NList<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T)(NList<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T)(NList<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T)(NList<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T)(NList<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(NList<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException();
}

[DebuggerDisplay("{ToString()}")]
[ComVisible(true)]
[Serializable]
public unsafe class String : NList<char, String>, IComparable, IComparable<char[]>, IComparable<IEnumerable<char>>, IComparable<string>, IComparable<String>
{
	private protected static readonly CompareInfo CurrentCompareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
	private protected static readonly CompareInfo DefaultCompareInfo = CompareInfo.GetCompareInfo(CultureInfo.InvariantCulture.LCID);
	private const string CompareMessage = "Этот метод не работает в .NStar и всегда выбрасывает исключение."
			+ " Используйте strA.CompareTo(strB, ...).";
	private const string CompareRangeMessage = "Этот метод не работает в .NStar и всегда выбрасывает исключение. Используйте"
		+ " strA.GetRange(indexA, length).CompareTo(strB.GetRange(indexB, length), ...) - в нативных коллекциях, какой является"
		+ " Corlib.NStar.String, метод GetRange() использует арифметику указателей и работает очень быстро.";
	private const string CompareTrivialMessage = "Этот метод не работает в .NStar и всегда выбрасывает исключение."
		+ " Используйте strA.CompareTo(strB).";

	public String() : base() { }

	public String(IEnumerable<char> collection) : base(collection) { }

	public String(int capacity) : base(capacity) { }

	public String(int capacity, IEnumerable<char> collection) : base(capacity, collection) { }

	public String(int capacity, string s) : base(capacity, [.. s]) { }

	public String(int capacity, params char[] array) : base(capacity, array) { }

	public String(int capacity, ReadOnlySpan<char> span) : base(capacity, span) { }

	public String(string s) : base([.. s]) { }

	public String(params char[] array) : base(array) { }

	public String(ReadOnlySpan<char> span) : base(span) { }

	public String(char c, int length) : base(length)
	{
		for (var i = 0; i < length; i++)
			SetInternal(i, c);
		_size = length;
	}

	protected override Func<int, String> CapacityCreator => x => new(x);

	protected override Func<IEnumerable<char>, String> CollectionCreator => x => new(x);

	protected override Func<ReadOnlySpan<char>, String> SpanCreator => x => new(x);

	public virtual String AddRange(string s) => Insert(_size, s);

	[Obsolete(CompareTrivialMessage, true)]
	public static int Compare(String? strA, String? strB) =>
		throw new NotSupportedException(CompareTrivialMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, bool ignoreCase) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, bool ignoreCase, CultureInfo culture) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, CultureInfo culture) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, CultureInfo culture, CompareOptions options) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareMessage, true)]
	public static int Compare(String? strA, String? strB, StringComparison comparisonType) =>
		throw new NotSupportedException(CompareMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length) =>
		throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, bool ignoreCase) =>
		throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, bool ignoreCase,
		CultureInfo culture) => throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, CultureInfo culture) =>
		throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length, CultureInfo culture,
		CompareOptions options) => throw new NotSupportedException(CompareRangeMessage);

	[Obsolete(CompareRangeMessage, true)]
	public static int Compare(String? strA, int indexA, String? strB, int indexB, int length,
		StringComparison comparisonType) =>
		throw new NotSupportedException(CompareRangeMessage);

	public virtual int CompareTo(object? other) => other switch
	{
		null => 1,
		IEnumerable<char> enumerable => CompareTo(enumerable),
		IComparable<String> icns => -icns.CompareTo(this),
		IComparable<char[]> icarr => -icarr.CompareTo(ToArray()),
		IComparable<string> ics => -ics.CompareTo(ToString()),
		_ => throw new ArgumentException("Невозможно сравнить строку с этим объектом!", nameof(other)),
	};

	public virtual int CompareTo(char[]? other) => DefaultCompareInfo.Compare(AsSpan(), other ?? []);

	public virtual int CompareTo(IEnumerable<char>? other) => other switch
	{
		null => 1,
		char[] chars => CompareTo(chars.AsSpan()),
		List<char> list => CompareTo(list.AsSpan()),
		NList<char> nList => CompareTo(nList.AsSpan()),
		string s => CompareToNotNull(s),
		String ns => CompareTo(ns.AsSpan()),
		IComparable<String> icns => -icns.CompareTo(this),
		IComparable<char[]> icarr => -icarr.CompareTo(ToArray()),
		IComparable<string> ics => -ics.CompareTo(ToString()),
		_ => CompareToNotNull([.. other]),
	};

	public virtual int CompareTo(ReadOnlySpan<char> other) => DefaultCompareInfo.Compare(AsSpan(), other);

	public virtual int CompareTo(string? other) => DefaultCompareInfo.Compare(AsSpan(), other ?? "");

	public virtual int CompareTo(String? other) => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan());

	public virtual int CompareTo(String? other, bool ignoreCase) => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
		ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

	public virtual int CompareTo(String? other, bool ignoreCase, CultureInfo culture) =>
		CompareInfo.GetCompareInfo(culture.LCID).Compare(AsSpan(), (other ?? []).AsSpan(),
			ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

	public virtual int CompareTo(String? other, CultureInfo culture) =>
		CompareInfo.GetCompareInfo(culture.LCID).Compare(AsSpan(), (other ?? []).AsSpan());

	[Experimental("CS9216")]
	public virtual int CompareTo(String? other, CultureInfo culture, CompareOptions options) =>
		CompareInfo.GetCompareInfo(culture.LCID).Compare(AsSpan(), (other ?? []).AsSpan(), options);

	/// <summary>
	/// WARNING!!! This methods works wrong with StringComparison.Ordinal!
	/// (But probably works right with StringComparison.OrdinalIgnoreCase.)
	/// </summary>
	/// <param name="comparisonType">NOT StringComparison.Ordinal!!!</param>
	public virtual int CompareTo(String? other, StringComparison comparisonType) => comparisonType switch
	{
		StringComparison.CurrentCulture => CurrentCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan()),
		StringComparison.CurrentCultureIgnoreCase => CurrentCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
			CompareOptions.IgnoreCase),
		StringComparison.InvariantCulture => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan()),
		StringComparison.InvariantCultureIgnoreCase => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
			CompareOptions.IgnoreCase),
		StringComparison.Ordinal => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(), CompareOptions.Ordinal),
		StringComparison.OrdinalIgnoreCase => DefaultCompareInfo.Compare(AsSpan(), (other ?? []).AsSpan(),
			CompareOptions.OrdinalIgnoreCase),
		_ => throw new ArgumentException("Такой способ сравнения строк не существует!", nameof(comparisonType)),
	};

	protected virtual int CompareToNotNull([NotNull] char[] other) => DefaultCompareInfo.Compare(AsSpan(), other);

	protected virtual int CompareToNotNull([NotNull] string other) => DefaultCompareInfo.Compare(AsSpan(), other);

	public virtual bool Contains(char value, bool ignoreCase) => Contains(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool Contains(char value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).Contains([value], comparisonType);

	public virtual bool Contains(ReadOnlySpan<char> value, bool ignoreCase) => Contains(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool Contains(ReadOnlySpan<char> value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).Contains(value, comparisonType);

	public virtual bool Contains(String value, bool ignoreCase) => Contains(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool Contains(String value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).Contains(value.AsSpan(), comparisonType);

	public virtual bool EndsWith(char value, bool ignoreCase) => EndsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool EndsWith(char value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).EndsWith([value], comparisonType);

	public virtual bool EndsWith(ReadOnlySpan<char> value, bool ignoreCase) => EndsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).EndsWith(value, comparisonType);

	public virtual bool EndsWith(String value, bool ignoreCase) => EndsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool EndsWith(String value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).EndsWith(value.AsSpan(), comparisonType);

	public override bool Equals(object? obj) => base.Equals(obj);

	public override int GetHashCode() => base.GetHashCode();

	public virtual int IndexOf(char value, bool ignoreCase) => IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual int IndexOf(char value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).IndexOf([value], comparisonType);

	public virtual int IndexOf(ReadOnlySpan<char> value, bool ignoreCase) => IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).IndexOf(value, comparisonType);

	public virtual int IndexOf(String value, bool ignoreCase) => IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual int IndexOf(String value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).IndexOf(value.AsSpan(), comparisonType);

	public virtual String Insert(int index, string s)
	{
		if (_isRange)
			throw new InvalidOperationException("Изменение нативного списка, являющегося диапазоном другого списка,"
				+ " запрещено. Если вы хотите создать список из диапазона другого списка, а затем изменять его,"
				+ " в методе GetRange() установите параметр alwaysCopy в true.");
		var length = s.Length;
		if (length == 0)
			return this;
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = (char*)Marshal.AllocHGlobal(sizeof(char) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + length, _size - index);
			fixed (char* ptr = s)
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
			fixed (char* ptr = s)
				CopyMemory(ptr, 0, _items, index, length);
		}
		_size += length;
		Changed();
		return this;
	}

	public static String Join(char separator, IEnumerable<string> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.ToNString();
		while (en.MoveNext())
		{
			result.Add(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(String separator, IEnumerable<string> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.ToNString();
		while (en.MoveNext())
		{
			result.AddRange(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(char separator, IEnumerable<String> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.Copy();
		while (en.MoveNext())
		{
			result.Add(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(String separator, IEnumerable<String> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		var en = collection.GetEnumerator();
		if (!en.MoveNext())
			return [];
		var result = en.Current.Copy();
		while (en.MoveNext())
		{
			result.AddRange(separator);
			result.AddRange(en.Current);
		}
		return result;
	}

	public static String Join(char separator, params string[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].ToNString();
		for (var i = 1; i < array.Length; i++)
		{
			result.Add(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public static String Join(String separator, params string[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].ToNString();
		for (var i = 1; i < array.Length; i++)
		{
			result.AddRange(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public static String Join(char separator, params String[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].Copy();
		for (var i = 1; i < array.Length; i++)
		{
			result.Add(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public static String Join(String separator, params String[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length == 0)
			return [];
		else if (array.Length == 1)
			return array[0];
		var result = array[0].Copy();
		for (var i = 1; i < array.Length; i++)
		{
			result.AddRange(separator);
			result.AddRange(array[i]);
		}
		return result;
	}

	public override String Pad(int length) => Pad(length, ' ');

	public override String PadInPlace(int length) => PadInPlace(length, ' ');

	public override String PadLeft(int length) => PadLeft(length, ' ');

	public override String PadLeftInPlace(int length) => PadLeftInPlace(length, ' ');

	public override String PadRight(int length) => PadRight(length, ' ');

	public override String PadRightInPlace(int length) => PadRightInPlace(length, ' ');

	public virtual String Replace(string s) => Replace(s.AsSpan());

	// TODO: этот метод разбиения игнорирует флаг TrimEntries в опциях. Правильное поведение этого флага в разработке.
	public virtual List<String> Split(char separator, StringSplitOptions options = StringSplitOptions.None)
	{
		if (_size == 0)
			return [];
		else if (_size == 1)
		{
			if (GetInternal(0) == separator)
				return options.HasFlag(StringSplitOptions.RemoveEmptyEntries) ? [] : [[], []];
			else
				return [GetInternal(0)];
		}
		var prevPos = 0;
		List<String> result = [];
		for (var i = 0; i < _size; i++)
			if (GetInternal(i) == separator)
			{
				if (!(prevPos == i && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
					result.Add(GetRange(prevPos..i));
				prevPos = i + 1;
			}
		if (!(prevPos == _size && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
			result.Add(GetRange(prevPos.._size));
		return result;
	}

	// TODO: этот метод разбиения игнорирует флаг TrimEntries в опциях. Правильное поведение этого флага в разработке.
	public virtual List<String> Split(String separator, StringSplitOptions options = StringSplitOptions.None)
	{
		if (_size == 0)
			return [];
		else if (_size < separator.Length)
			return [this];
		var prevPos = 0;
		LimitedQueue<char> queue = new(separator.Length);
		List<String> result = [];
		for (var i = 0; i < _size; i++)
		{
			queue.Enqueue(GetInternal(i));
			if (separator.Equals(queue))
			{
				if (!(prevPos >= i + 1 - queue.Length && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
					result.Add(GetRange(prevPos..(i + 1 - queue.Length)));
				prevPos = i + 1;
				queue.Clear();
			}
		}
		if (!(prevPos == _size && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)))
			result.Add(GetRange(prevPos.._size));
		return result;
	}

	public virtual bool StartsWith(char value, bool ignoreCase) => StartsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool StartsWith(char value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).StartsWith([value], comparisonType);

	public virtual bool StartsWith(ReadOnlySpan<char> value, bool ignoreCase) => StartsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool StartsWith(ReadOnlySpan<char> value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).StartsWith(value, comparisonType);

	public virtual bool StartsWith(String value, bool ignoreCase) => StartsWith(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

	public virtual bool StartsWith(String value, StringComparison comparisonType) => ((ReadOnlySpan<char>)AsSpan()).StartsWith(value.AsSpan(), comparisonType);

	public virtual String ToLower()
	{
		for (var i = 0; i < _size; i++)
			SetInternal(i, char.ToLower(GetInternal(i)));
		return this;
	}

	public override string ToString() => new(AsSpan());

	public virtual String ToUpper()
	{
		for (var i = 0; i < _size; i++)
			SetInternal(i, char.ToUpper(GetInternal(i)));
		return this;
	}

	public virtual String Trim() => TrimEnd().TrimStart();

	public virtual String Trim(char c) => TrimEnd(c).TrimStart(c);

	public virtual String Trim(IEnumerable<char> chars) => TrimEnd(chars).TrimStart(chars);

	public virtual String Trim(params char[] chars) => Trim((IEnumerable<char>)chars);

	public virtual String TrimEnd()
	{
		for (var i = _size - 1; i >= 0; i--)
			if (!char.IsWhiteSpace(GetInternal(i)))
			{
				RemoveEnd(i + 1);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimEnd(char c)
	{
		for (var i = _size - 1; i >= 0; i--)
			if (GetInternal(i) != c)
			{
				RemoveEnd(i + 1);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimEnd(IEnumerable<char> chars)
	{
		ArgumentNullException.ThrowIfNull(chars);
		if (chars is not ISet<char> set)
			set = chars.ToHashSet();
		for (var i = _size - 1; i >= 0; i--)
			if (!set.Contains(GetInternal(i)))
			{
				RemoveEnd(i + 1);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimEnd(params char[] chars) => TrimEnd((IEnumerable<char>)chars);

	public virtual String TrimStart()
	{
		for (var i = 0; i < _size; i++)
			if (!char.IsWhiteSpace(GetInternal(i)))
			{
				Remove(0, i);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimStart(char c)
	{
		for (var i = 0; i < _size; i++)
			if (GetInternal(i) != c)
			{
				Remove(0, i);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimStart(IEnumerable<char> chars)
	{
		ArgumentNullException.ThrowIfNull(chars);
		if (chars is not ISet<char> set)
			set = chars.ToHashSet();
		for (var i = 0; i < _size; i++)
			if (!set.Contains(GetInternal(i)))
			{
				Remove(0, i);
				return this;
			}
		Clear();
		return this;
	}

	public virtual String TrimStart(params char[] chars) => TrimStart((IEnumerable<char>)chars);

	public static bool operator ==(String? x, String? y) => x?.Equals(y) ?? y is null;

	public static bool operator ==(String? x, string? y) => x is null && y is null || x is not null && y is not null && x.Equals((String)y);

	public static bool operator ==(string? x, String? y) => x is null && y is null || x is not null && y is not null && ((String)x).Equals(y);

	public static bool operator !=(String? x, String? y) => !(x == y);

	public static bool operator !=(String? x, string? y) => !(x == y);

	public static bool operator !=(string? x, String? y) => !(x == y);

	public static implicit operator String(char x) => new(32, x);

	public static implicit operator String(char[]? x) => x == null ? [] : new(32, x);

	public static implicit operator String(string? x) => x == null ? [] : new(32, (ReadOnlySpan<char>)x);

	public static explicit operator String((char, char) x) => [x.Item1, x.Item2];

	public static explicit operator String((char, char, char) x) => [x.Item1, x.Item2, x.Item3];

	public static explicit operator String((char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static explicit operator String((char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static explicit operator String((char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static explicit operator String((char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static explicit operator String((char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static explicit operator String((char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static explicit operator String((char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (char, char)(String x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char)(String x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char)(String x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char)(String x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char)(String x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char)(String x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char)(String x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char)(String x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException();

	public static explicit operator (char, char, char, char, char, char, char, char, char, char, char, char, char, char, char, char)(String x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException();
}
