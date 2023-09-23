using System.Numerics;

namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract partial class Buffer<T, TCertain> : BaseList<T, TCertain> where TCertain : Buffer<T, TCertain>, new()
{
	private protected T[] _items;
	private protected int _start;

	public Buffer(int capacity)
	{
		if (capacity <= 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		_items = new T[capacity];
		_start = 0;
	}

	public Buffer(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				throw new ArgumentException(null, nameof(collection));
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
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		_items = array.ToArray();
		_start = 0;
	}

	public Buffer(int capacity, params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = Min(capacity, array.Length);
		_items = new T[capacity];
		Array.Copy(array, Max(0, array.Length - _size), _items, 0, _size);
		_start = 0;
	}

	public Buffer(ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_size = span.Length;
		_items = span.ToArray();
		_start = 0;
	}

	public Buffer(int capacity, ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
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
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value == _items.Length)
				return;
			if (value == 0)
				throw new ArgumentOutOfRangeException(nameof(value));
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

	public override TCertain Add(T item)
	{
		if (_size == Capacity)
		{
			SetInternal(_size, item);
			_start = (_start + 1) % Capacity;
		}
		else
			SetInternal(_size++, item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override Span<T> AsSpan(int index, int length) => throw new NotSupportedException();

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

	public virtual Buffer<TOutput> Convert<TOutput>(Func<T, TOutput> converter) => base.Convert<TOutput, Buffer<TOutput>>(converter);

	public virtual Buffer<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) => base.Convert<TOutput, Buffer<TOutput>>(converter);

	private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (source != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
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

	private protected override void EnsureCapacity(int min) => throw new NotSupportedException();

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
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (index == 0)
		{
			if (_size == Capacity)
				return this2;
			else
				_start = (_start + Capacity - 1) % Capacity;
		}
		if (index > 0 && index < _size)
			Copy(this2, index, this2, index + 1, _size - index);
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
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length == 0)
			return this2;
		var toSkip = Max(0, length + _size - Capacity - index);
		var index2 = _size + length - toSkip >= Capacity + index ? 0 : index;
		if (index2 > 0 && index2 < _size)
			Copy(this2, index2, this2, index2 + length - toSkip, _size - index2);
		if (index2 == 0)
			_start = (_start + Max(_size, Capacity - length + toSkip)) % Capacity;
		var i = 0;
		var en = collection.GetEnumerator();
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
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		_size--;
		if (index == 0)
		{
			SetInternal(0, default!);
			_start = (_start + 1) % Capacity;
			return this2;
		}
		else if (index < _size)
			Copy(this2, index + 1, this2, index, _size - index);
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
		return this as TCertain ?? throw new InvalidOperationException();
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

	public Buffer(params T[] array) : base(array) { }

	public Buffer(int capacity, params T[] array) : base(capacity, array) { }

	public Buffer(ReadOnlySpan<T> span) : base(span) { }

	public Buffer(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	private protected override Func<int, Buffer<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, Buffer<T>> CollectionCreator => x => new(x);

	public static implicit operator Buffer<T>(T x) => new(x);

	public static implicit operator Buffer<T>(T[] x) => new(x);

	public static explicit operator Buffer<T>((T, T) x) => new(x.Item1, x.Item2);

	public static explicit operator Buffer<T>((T, T, T) x) => new(x.Item1, x.Item2, x.Item3);

	public static explicit operator Buffer<T>((T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4);

	public static explicit operator Buffer<T>((T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5);

	public static explicit operator Buffer<T>((T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15);

	public static explicit operator Buffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16);

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

	private static readonly T[] _emptyArray = Array.Empty<T>();

	public List() => _items = _emptyArray;

	public List(int capacity)
	{
		if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
		_items = capacity == 0 ? _emptyArray : (new T[capacity]);
	}

	public List(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		_items = array.ToArray();
	}

	public List(int capacity, params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		if (array.Length > capacity)
			_items = array.ToArray();
		else
		{
			_items = new T[capacity];
			Array.Copy(array, _items, array.Length);
		}
	}

	public List(ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_size = span.Length;
		_items = span.ToArray();
	}

	public List(int capacity, ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
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
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return new();
		return MemoryExtensions.AsSpan(_items, index, length);
	}

	public virtual int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	public virtual int BinarySearch(int index, int length, T item, IComparer<T> comparer)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		return Array.BinarySearch(_items, index, length, item, comparer);
	}

	private protected override void ClearInternal(int index, int length)
	{
		Array.Clear(_items, index, length);
		Changed();
	}

	public virtual List<TOutput> Convert<TOutput>(Func<T, TOutput> converter) => base.Convert<TOutput, List<TOutput>>(converter);

	public virtual List<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) => base.Convert<TOutput, List<TOutput>>(converter);

	private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		Array.Copy(source._items, sourceIndex, destination._items, destinationIndex, length);
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
			var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
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
				Copy(this as TCertain ?? throw new InvalidOperationException(), index, this as TCertain ?? throw new InvalidOperationException(), index + 1, _size - index);
			else
				_size++;
			_items[index] = item;
		}
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain Insert(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		var length = span.Length;
		if (length == 0)
			return this as TCertain ?? throw new InvalidOperationException();
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
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
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is List<T, TCertain> list)
		{
			var length = list._size;
			if (length == 0)
				return this as TCertain ?? throw new InvalidOperationException();
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				var newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + length, _size - index);
				if (this == list)
				{
					Array.Copy(_items, 0, newItems, index, index);
					Array.Copy(_items, index + length, newItems, index * 2, _size - index);
				}
				else
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
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else if (collection is T[] array)
		{
			var length = array.Length;
			if (length == 0)
				return this as TCertain ?? throw new InvalidOperationException();
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
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
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else if (collection is G.ICollection<T> list2)
		{
			var length = list2.Count;
			if (length == 0)
				return this as TCertain ?? throw new InvalidOperationException();
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
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
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return InsertInternal(index, CollectionCreator(collection));
	}

	private protected override int LastIndexOfInternal(T item, int index, int length) => Array.LastIndexOf(_items, item, index, length);

	public virtual TCertain NSort() => NSort(0, _size);

	public unsafe virtual TCertain NSort(int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (this is List<uint> uintList)
		{
			uintList._items.NSort(index, length);
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return Sort(index, length, G.Comparer<T>.Default);
	}

	public virtual TCertain NSort(Func<T, uint> function) => NSort(function, 0, _size);

	public unsafe virtual TCertain NSort(Func<T, uint> function, int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		_items.NSort(function, index, length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public static List<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) => collection is List<TList> list ? list : new(collection);

	private protected override TCertain ReverseInternal(int index, int length)
	{
		Array.Reverse(_items, index, length);
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		Array.Sort(_items, index, length, comparer);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain Sort<TValue>(Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(0, _size, function, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(index, length, function, G.Comparer<TValue>.Default, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, IComparer<TValue> comparer, bool fasterButMoreMemory = true)
	{
		if (fasterButMoreMemory)
		{
			Convert(function).Sort(this, index, length, comparer);
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return Sort(index, length, new Comparer<T>((x, y) => comparer.Compare(function(x), function(y))));
	}

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, comparer);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, int index, int length, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new()
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (index + length > values._size)
			throw new ArgumentException(null);
		Array.Sort(_items, values._items, index, length, comparer);
		return this as TCertain ?? throw new InvalidOperationException();
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class List<T> : List<T, List<T>>
{
	public List() { }

	public List(int capacity) : base(capacity) { }

	public List(IEnumerable<T> collection) : base(collection) { }

	public List(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public List(params T[] array) : base(array) { }

	public List(int capacity, params T[] array) : base(capacity, array) { }

	public List(ReadOnlySpan<T> span) : base(span) { }

	public List(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	private protected override Func<int, List<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, List<T>> CollectionCreator => x => new(x);

	public static implicit operator List<T>(T x) => new(x);

	public static implicit operator List<T>(T[] x) => new(x);

	public static explicit operator List<T>((T, T) x) => new(x.Item1, x.Item2);

	public static explicit operator List<T>((T, T, T) x) => new(x.Item1, x.Item2, x.Item3);

	public static explicit operator List<T>((T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4);

	public static explicit operator List<T>((T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5);

	public static explicit operator List<T>((T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6);

	public static explicit operator List<T>((T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15);

	public static explicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16);

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

[DebuggerDisplay("{ToString()}")]
[ComVisible(true)]
[Serializable]
public class String : List<char, String>
{
	public String() : base() { }

	public String(int capacity) : base(capacity) { }

	public String(IEnumerable<char> collection) : base(collection) { }

	public String(params char[] array) : base(array) { }

	public String(ReadOnlySpan<char> span) : base(span) { }

	public String(int capacity, IEnumerable<char> collection) : base(capacity, collection) { }

	public String(int capacity, params char[] array) : base(capacity, array) { }

	public String(int capacity, ReadOnlySpan<char> span) : base(capacity, span) { }

	private protected override Func<int, String> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<char>, String> CollectionCreator => x => new(x);

	public override bool Equals(object? obj) => base.Equals(obj);

	public override int GetHashCode() => base.GetHashCode();

	public override String Pad(int length) => Pad(length, ' ');

	public override String PadInPlace(int length) => PadInPlace(length, ' ');

	public override String PadLeft(int length) => PadLeft(length, ' ');

	public override String PadLeftInPlace(int length) => PadLeftInPlace(length, ' ');

	public override String PadRight(int length) => PadRight(length, ' ');

	public override String PadRightInPlace(int length) => PadRightInPlace(length, ' ');

	public override string ToString() => new(AsSpan());

	public static bool operator ==(String? x, String? y) => x?.Equals(y) ?? y == null;

	public static bool operator !=(String? x, String? y) => !(x == y);

	public static implicit operator String(char x) => new(x);

	public static implicit operator String(char[] x) => new(x);

	public static implicit operator String(string x) => new((ReadOnlySpan<char>)x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigList<T> : BigList<T, BigList<T>, List<T>>
{
	public BigList() : this(-1) { }

	public BigList(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigList(MpzT capacity, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacity, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigList(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(collection, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigList(MpzT capacity, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacity, collection, capacityStepBitLength, capacityFirstStepBitLength) { }

	private protected override Func<MpzT, BigList<T>> CapacityCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<IEnumerable<T>, BigList<T>> CollectionCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<int, List<T>> CapacityLowCreator => x => new(x);

	private protected override Func<IEnumerable<T>, List<T>> CollectionLowCreator => x => new(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public unsafe partial class NList<T> : BaseList<T, NList<T>> where T : unmanaged
{
	private T* _items;
	private int _capacity;

	private static readonly T* _emptyArray = null;

	public NList() => _items = _emptyArray;

	public NList(int capacity)
	{
		if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
		_items = capacity == 0 ? _emptyArray : (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = capacity));
	}

	public NList(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				return;
			if (length > capacity)
			{
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
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_capacity = _size = array.Length;
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = array)
			CopyMemory(ptr, _items, _size);
	}

	public NList(int capacity, params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
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
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_capacity = _size = span.Length;
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = span)
			CopyMemory(ptr, _items, _size);
	}

	public NList(int capacity, ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_capacity = Max(capacity, _size = span.Length);
		_items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
		fixed (T* ptr = span)
			CopyMemory(ptr, _items, _size);
	}

	public override int Capacity
	{
		get => _capacity;
		set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value == _capacity)
				return;
			if (value > 0)
			{
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * value);
				if (_size > 0)
					CopyMemory(_items, newItems, _size);
				Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
			}
			else
			{
				Marshal.FreeHGlobal((nint)_items);
				_items = _emptyArray;
			}
			_capacity = value;
			Changed();
		}
	}

	private protected override Func<int, NList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, NList<T>> CollectionCreator => x => new(x);

	public virtual NList<T> AddRange(ReadOnlySpan<T> span) => Insert(_size, span);

	public override Span<T> AsSpan(int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return new();
		return new(_items + index, length);
	}

	private protected override void ClearInternal(int index, int length)
	{
		FillMemory(_items + index, length, 0);
		Changed();
	}

	private protected override int CompareInternal(int index, NList<T> other, int otherIndex, int length) => CompareMemory(_items + index, other._items + otherIndex, length);

	public virtual NList<TOutput> Convert<TOutput>(Func<T, TOutput> converter) where TOutput : unmanaged => base.Convert<TOutput, NList<TOutput>>(converter);

	public virtual NList<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) where TOutput : unmanaged => base.Convert<TOutput, NList<TOutput>>(converter);

	private protected override void Copy(NList<T> source, int sourceIndex, NList<T> destination, int destinationIndex, int length)
	{
		CopyMemory(source._items, sourceIndex, destination._items, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		Changed();
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		fixed (T* ptr = array)
			CopyMemory(_items, index, ptr, arrayIndex, length);
	}

	public override void Dispose()
	{
		Marshal.FreeHGlobal((nint)_items);
		_capacity = 0;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is not G.IList<T>)
			return base.EqualsInternal(collection, index, toEnd);
		if (collection is NList<T> nList)
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

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = _items[index];
		if (invoke)
			Changed();
		return item;
	}

	public override NList<T> GetRange(int index, int length, bool alwaysCopy = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return new();
		else if (!alwaysCopy && index == 0 && length == _size)
			return this;
		if (!alwaysCopy)
			return new(length) { _items = _items + index, _size = length };
		else
		{
			NList<T> list = new(length) { _size = length };
			CopyMemory(_items + index, list._items, length);
			return list;
		}
	}

	private protected override int IndexOfInternal(T item, int index, int length)
	{
		var ptr = _items + index;
		for (var i = 0; i < length; i++)
			if (ptr[i].Equals(item))
				return index + i;
		return -1;
	}

	public override NList<T> Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
		{
			var min = _size + 1;
			var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + 1, _size - index);
			newItems[index] = item;
			Marshal.FreeHGlobal((nint)_items);
			_items = newItems;
			_capacity = newCapacity;
			_size++;
		}
		else
		{
			if (index < _size)
				Copy(this, index, this, index + 1, _size - index);
			_items[index] = item;
		}
		Changed();
		return this;
	}

	public virtual NList<T> Insert(int index, ReadOnlySpan<T> span)
	{
		var length = span.Length;
		if (length == 0)
			return this;
		if (Capacity < _size + length)
		{
			var min = _size + length;
			var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + length, _size - index);
			span.CopyTo(new(newItems + index, newCapacity - index));
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
		return this;
	}

	private protected override NList<T> InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is NList<T> list)
		{
			var length = list._size;
			if (length == 0)
				return this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
				if (this == list)
				{
					CopyMemory(_items, 0, newItems, index, index);
					CopyMemory(_items, index + length, newItems, index * 2, _size - index);
				}
				else
					CopyMemory(list._items, 0, newItems, index, length);
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
			return this;
		}
		else if (collection is T[] array)
		{
			var length = array.Length;
			if (length == 0)
				return this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
				fixed (T* ptr = array)
					CopyMemory(ptr, 0, newItems, index, length);
				Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + length, _size - index);
				fixed (T* ptr = array)
					CopyMemory(ptr, 0, _items, index, length);
			}
			_size += length;
			return this;
		}
		else if (collection is G.ICollection<T> list2)
		{
			var length = list2.Count;
			if (length == 0)
				return this;
			if (Capacity < _size + length)
			{
				var min = _size + length;
				var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				var newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + length, _size - index);
				fixed (T* ptr = list2.AsSpan())
					CopyMemory(ptr, 0, newItems, index, list2.Count);
				Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
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
			return this;
		}
		else
			return InsertInternal(index, new NList<T>(collection));
	}

	private protected override int LastIndexOfInternal(T item, int index, int length)
	{
		var endIndex = index - length + 1;
		for (var i = index; i >= endIndex; i--)
			if (_items[i].Equals(item))
				return i;
		return -1;
	}

	public static NList<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) where TList : unmanaged => collection is NList<TList> list ? list : new(collection);

	private protected override NList<T> ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
			(_items[index + i], _items[index + length - 1 - i]) = (_items[index + length - 1 - i], _items[index + i]);
		Changed();
		return this;
	}

	internal override void SetInternal(int index, T value)
	{
		_items[index] = value;
		Changed();
	}

	public virtual NList<T> Sort() => Sort(0, _size);

	public virtual NList<T> Sort(int index, int length)
	{
		var shiftedItems = _items + index;
		RadixSort(shiftedItems, length);
		return this;
	}

	public virtual NList<T> Sort(Func<T, uint> function) => Sort(function, 0, _size);

	public virtual NList<T> Sort(Func<T, uint> function, int index, int length)
	{
		NSort(_items, function, index, length);
		return this;
	}

	public static implicit operator NList<T>(T x) => new NList<T>().Add(x);

	public static explicit operator NList<T>((T, T) x) => new(x.Item1, x.Item2);

	public static explicit operator NList<T>((T, T, T) x) => new(x.Item1, x.Item2, x.Item3);

	public static explicit operator NList<T>((T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4);

	public static explicit operator NList<T>((T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5);

	public static explicit operator NList<T>((T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6);

	public static explicit operator NList<T>((T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15);

	public static explicit operator NList<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => new(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16);

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
