
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract partial class Buffer<T, TCertain> : BaseList<T, TCertain> where TCertain : Buffer<T, TCertain>, new()
{
	private protected T[] _items;
	private protected int _start;

	public Buffer(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
		_items = new T[capacity];
		_start = 0;
	}

	public Buffer(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				throw new ArgumentException("Коллекция нулевой длины недопустима в этом контексте.", nameof(collection));
			else
			{
				_items = new T[length];
				c.CopyTo(_items, 0);
				_size = length;
			}
		}
		else
		{
			_size = 0;
			_items = new T[collection.Length()];
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
		_start = 0;
	}

	public Buffer(int capacity, IEnumerable<T> collection) : this(capacity)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
		{
			var length = list.Count;
			if (length == 0)
				return;
			var start = Max(0, length - capacity);
			for (var i = start; i < length; i++)
				_items[i - start] = list[i];
			_size = length;
		}
		else
		{
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public Buffer(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		_items = [.. array];
		_start = 0;
	}

	public Buffer(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = Min(capacity, array.Length);
		_items = new T[capacity];
		Array.Copy(array, Max(0, array.Length - _size), _items, 0, _size);
		_start = 0;
	}

	public Buffer(ReadOnlySpan<T> span)
	{
		_size = span.Length;
		_items = span.ToArray();
		_start = 0;
	}

	public Buffer(int capacity, ReadOnlySpan<T> span)
	{
		_size = Min(capacity, span.Length);
		_items = new T[capacity];
		span.Slice(Max(0, span.Length - _size), _size).CopyTo(_items);
		_start = 0;
	}

	public override int Capacity
	{
		get => _items.Length;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			if (value == _items.Length)
				return;
			ArgumentOutOfRangeException.ThrowIfZero(value);
			var newItems = new T[value];
			if (_size > 0)
			{
				if (_start + _size < Capacity)
					Array.Copy(_items, _start, newItems, 0, _size);
				else
				{
					Array.Copy(_items, _start, newItems, 0, Capacity - _start);
					Array.Copy(_items, 0, newItems, Capacity - _start, _size - (Capacity - _start));
				}
			}
			_items = newItems;
			_start = 0;
			Changed();
		}
	}

	public virtual bool IsFull => _size == Capacity;

	public override TCertain Add(T item)
	{
		if (_size == Capacity)
		{
			SetInternal(_size, item);
			_start = (_start + 1) % Capacity;
		}
		else
			SetInternal(_size++, item);
		return (TCertain)this;
	}

	public override Span<T> AsSpan(int index, int length) => RedStarLinq.ToArray(GetSlice(index, length)).AsSpan();

	private protected override void ClearInternal(int index, int length)
	{
		if (_start + index + length < Capacity)
			Array.Clear(_items, _start + index, length);
		else if (_start + index < Capacity)
		{
			Array.Clear(_items, _start + index, Capacity - _start - index);
			Array.Clear(_items, 0, length - (Capacity - _start - index));
		}
		else
			Array.Clear(_items, (_start + index) % Capacity, length);
		Changed();
	}

	private protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (this != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		destination.Changed();
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (_start + _size < Capacity)
			Array.Copy(_items, _start, array, arrayIndex, _size);
		else
		{
			Array.Copy(_items, _start, array, arrayIndex, Capacity - _start);
			Array.Copy(_items, 0, array, arrayIndex + Capacity - _start, _size - (Capacity - _start));
		}
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		if (_start + index + length < Capacity)
			Array.Copy(_items, _start + index, array, arrayIndex, length);
		else if (_start + index < Capacity)
		{
			Array.Copy(_items, _start + index, array, arrayIndex, Capacity - _start - index);
			Array.Copy(_items, 0, array, arrayIndex + Capacity - _start - index, length - (Capacity - _start - index));
		}
		else
			Array.Copy(_items, (_start + index) % Capacity, array, arrayIndex, length);
	}

	public override void Dispose()
	{
		_items = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	private protected override void EnsureCapacity(int min) =>
		throw new NotSupportedException("Вы докопались до этого метода? Поздравляем, ваши знания C# выше уровня Hello world!"
		+ " Вот только для данной коллекции он неуместен, так как при исчерпании емкости она удаляет элементы,"
		+ " а не увеличивает емкость. Если вам нужно изменить емкость, такие извращения не нужны,"
		+ " достаточно Capacity = value.");

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = _items[(_start + index) % Capacity];
		if (invoke)
			Changed();
		return item;
	}

	private protected override int IndexOfInternal(T item, int index, int length)
	{
		for (var i = index; i < index + length; i++)
			if (GetInternal(i)?.Equals(item) ?? item == null)
				return i;
		return -1;
	}

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		if (index == 0)
		{
			if (_size == Capacity)
				return this2;
			else
				_start = (_start + Capacity - 1) % Capacity;
		}
		if (index > 0 && index < _size)
			CopyToInternal(index, this2, index + 1, _size - index);
		SetInternal(index, item);
		if (_size < Capacity)
			_size++;
		else
			_start = (_start + 1) % Capacity;
		Changed();
		return this2;
	}

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = collection.Length();
		var this2 = (TCertain)this;
		if (length == 0)
			return this2;
		var toSkip = Max(0, length + _size - Capacity - index);
		var index2 = _size + length - toSkip >= Capacity + index ? 0 : index;
		if (index2 > 0 && index2 < _size)
			CopyToInternal(index2, this2, index2 + length - toSkip, _size - index2);
		if (index2 == 0)
			_start = (_start + Max(_size, Capacity - length + toSkip)) % Capacity;
		var i = 0;
		using var en = collection.GetEnumerator();
		while (i < toSkip && en.MoveNext())
			i++;
		i = index2;
		while (en.MoveNext())
			SetInternal(i++, en.Current);
		if (index2 != 0)
			_start = (_start + Max(0, _size + length - Capacity - toSkip)) % Capacity;
		_size = Min(_size + length - toSkip, Capacity);
		return this2;
	}

	private protected override int LastIndexOfInternal(T item, int index, int length)
	{
		var endIndex = index - length + 1;
		for (var i = index; i >= endIndex; i--)
			if (GetInternal(i)?.Equals(item) ?? item == null)
				return i;
		return -1;
	}

	public override TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		_size--;
		if (index == 0)
		{
			SetInternal(0, default!);
			_start = (_start + 1) % Capacity;
			return this2;
		}
		else if (index < _size)
			CopyToInternal(index + 1, this2, index, _size - index);
		SetInternal(_size, default!);
		return this2;
	}

	private protected override TCertain ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
		{
			var temp = GetInternal(index + i);
			SetInternal(index + i, GetInternal(index + length - i - 1));
			SetInternal(index + length - i - 1, temp);
		}
		return (TCertain)this;
	}

	internal override void SetInternal(int index, T value)
	{
		_items[(_start + index) % Capacity] = value;
		Changed();
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Buffer<T> : Buffer<T, Buffer<T>>
{
	public Buffer() { }

	public Buffer(int capacity) : base(capacity) { }

	public Buffer(IEnumerable<T> collection) : base(collection) { }

	public Buffer(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public Buffer(int capacity, params T[] array) : base(capacity, array) { }

	public Buffer(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public Buffer(params T[] array) : base(array) { }

	public Buffer(ReadOnlySpan<T> span) : base(span) { }

	private protected override Func<int, Buffer<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, Buffer<T>> CollectionCreator => x => new(x);

	public static implicit operator Buffer<T>(T x) => new(x);

	public static implicit operator Buffer<T>(T[] x) => new(x);

	public static explicit operator Buffer<T>((T, T) x) => [x.Item1, x.Item2];

	public static explicit operator Buffer<T>((T, T, T) x) => [x.Item1, x.Item2, x.Item3];

	public static explicit operator Buffer<T>((T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static explicit operator Buffer<T>((T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static explicit operator Buffer<T>((T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (T, T)(Buffer<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T)(Buffer<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T)(Buffer<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T)(Buffer<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T)(Buffer<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(Buffer<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException();
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract partial class List<T, TCertain> : BaseList<T, TCertain> where TCertain : List<T, TCertain>, new()
{
	private protected T[] _items;

	private protected static readonly T[] _emptyArray = [];

	public List() => _items = _emptyArray;

	public List(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_items = capacity == 0 ? _emptyArray : (new T[capacity]);
	}

	public List(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				_items = _emptyArray;
			else
			{
				_items = new T[length];
				c.CopyTo(_items, 0);
				_size = length;
			}
		}
		else
		{
			_size = 0;
			_items = collection.TryGetLengthEasily(out var length) ? new T[length] : _emptyArray;
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public List(int capacity, IEnumerable<T> collection) : this(capacity)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				return;
			if (length > capacity)
				_items = new T[length];
			c.CopyTo(_items, 0);
			_size = length;
		}
		else
		{
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public List(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		_items = [.. array];
	}

	public List(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		if (array.Length > capacity)
			_items = [.. array];
		else
		{
			_items = new T[capacity];
			Array.Copy(array, _items, array.Length);
		}
	}

	public List(ReadOnlySpan<T> span)
	{
		_size = span.Length;
		_items = span.ToArray();
	}

	public List(int capacity, ReadOnlySpan<T> span)
	{
		_size = span.Length;
		if (span.Length > capacity)
			_items = span.ToArray();
		else
		{
			_items = new T[capacity];
			span.CopyTo(_items);
		}
	}

	public override int Capacity
	{
		get => _items.Length;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			if (value == _items.Length)
				return;
			if (value > 0)
			{
				var newItems = new T[value];
				if (_size > 0)
					Array.Copy(_items, 0, newItems, 0, _size);
				_items = newItems;
			}
			else
				_items = _emptyArray;
			Changed();
		}
	}

	public virtual TCertain AddRange(ReadOnlySpan<T> span) => Insert(_size, span);

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
		return Array.BinarySearch(_items, index, length, item, comparer);
	}

	public virtual int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	private protected override void ClearInternal(int index, int length)
	{
		Array.Clear(_items, index, length);
		Changed();
	}

	private protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		Array.Copy(_items, sourceIndex, destination._items, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		Changed();
	}

	private protected override void CopyToInternal(Array array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, _size);

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => Array.Copy(_items, index, array, arrayIndex, length);

	public override void Dispose()
	{
		_items = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = _items[index];
		if (invoke)
			Changed();
		return item;
	}

	private protected override int IndexOfInternal(T item, int index, int length) => Array.IndexOf(_items, item, index, length);

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
		{
			var min = _size + 1;
			var newCapacity = Math.Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = new T[newCapacity];
			if (index > 0)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size)
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
			_items[index] = item;
		}
		Changed();
		return (TCertain)this;
	}

	public virtual TCertain Insert(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = span.Length;
		if (length == 0)
			return (TCertain)this;
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Math.Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			var newItems = new T[newCapacity];
			if (index > 0)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size)
				Array.Copy(_items, index, newItems, index + length, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(newItems, index));
			_items = newItems;
		}
		else
		{
			if (index < _size)
				Array.Copy(_items, index, _items, index + length, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(_items, index));
		}
		_size += length;
		Changed();
		return (TCertain)this;
	}

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is List<T, TCertain> list)
		{
			var length = list._size;
			if (length == 0)
				return (TCertain)this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Math.Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				Array.Copy(list._items, 0, newItems, index, length);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					Array.Copy(_items, index, _items, index + length, _size - index);
				if (this == list)
				{
					Array.Copy(_items, 0, _items, index, index);
					Array.Copy(_items, index + length, _items, index * 2, _size - index);
				}
				else
					Array.Copy(list._items, 0, _items, index, length);
			}
			_size += length;
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
				var newCapacity = Math.Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				Array.Copy(array, 0, newItems, index, length);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					Array.Copy(_items, index, _items, index + length, _size - index);
				Array.Copy(array, 0, _items, index, length);
			}
			_size += length;
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
				var newCapacity = Math.Max(DefaultCapacity, Capacity * 2);
				if ((uint)newCapacity > int.MaxValue)
					newCapacity = int.MaxValue;
				if (newCapacity < min)
					newCapacity = min;
				var newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				list2.CopyTo(newItems, index);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					Array.Copy(_items, index, _items, index + length, _size - index);
				list2.CopyTo(_items, index);
			}
			_size += length;
			Changed();
			return (TCertain)this;
		}
		else
			return InsertInternal(index, CollectionCreator(collection));
	}

	private protected override int LastIndexOfInternal(T item, int index, int length) => Array.LastIndexOf(_items, item, index, length);

	public virtual TCertain NSort() => NSort(0, _size);

	public unsafe virtual TCertain NSort(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (this is List<byte> byteList)
		{
			byteList._items.NSort(index, length);
			return (TCertain)this;
		}
		else if (this is List<ushort> ushortList)
		{
			ushortList._items.NSort(index, length);
			return (TCertain)this;
		}
		else if (this is List<uint> uintList)
		{
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
		_items.NSort(function, index, length);
		return (TCertain)this;
	}

	public static List<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) => collection is List<TList> list ? list : new(collection);

	private protected override TCertain ReverseInternal(int index, int length)
	{
		Array.Reverse(_items, index, length);
		Changed();
		return (TCertain)this;
	}

	internal override void SetInternal(int index, T value)
	{
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
		Array.Sort(_items, index, length, comparer);
		return (TCertain)this;
	}

	public virtual TCertain Sort<TValue>(Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(0, _size, function, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(index, length, function, G.Comparer<TValue>.Default, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, IComparer<TValue> comparer, bool fasterButMoreMemory = true)
	{
		if (fasterButMoreMemory)
		{
			ToListEnumerable(this, function).Sort(this, index, length, comparer);
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
		Array.Sort(_items, values._items, index, length, comparer);
		return (TCertain)this;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class List<T> : List<T, List<T>>
{
	public List() { }

	public List(int capacity) : base(capacity) { }

	public List(IEnumerable<T> collection) : base(collection) { }

	public List(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public List(int capacity, params T[] array) : base(capacity, array) { }

	public List(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public List(params T[] array) : base(array) { }

	public List(ReadOnlySpan<T> span) : base(span) { }

	private protected override Func<int, List<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, List<T>> CollectionCreator => x => new(x);

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

	public static explicit operator (T, T)(List<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T)(List<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T)(List<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T)(List<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T)(List<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T)(List<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T)(List<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException();

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException();
}
