using System.Reflection;

namespace NStar.BufferLib;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract partial class LimitedBuffer<T, TCertain> : BaseList<T, TCertain> where TCertain : LimitedBuffer<T, TCertain>, new()
{
	private const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
	private protected T[]? _items;
	private protected int _start;

	private protected static readonly G.SortedDictionary<int, G.List<T[]>> arrayPool = [];
	private protected static readonly object globalLockObj = new();
	private static readonly FieldInfo? sliceStartField = typeof(Slice<T>).GetField("_start", BindingAttr);
	private static readonly FieldInfo? sliceBaseField = typeof(Slice<T>).GetField("_base", BindingAttr);
	private static readonly FieldInfo? sliceBase2Field = typeof(Slice<T>).GetField("_base2", BindingAttr);

	public LimitedBuffer(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
		_items = arrayPool.GetAndRemove(capacity);
		_start = 0;
	}

	public LimitedBuffer(G.IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(length);
			c.CopyTo(_items, 0);
			_size = length;
		}
		else if (collection is Slice<T> slice)
		{
			var length = slice.Length;
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(length);
			if (sliceStartField?.GetValue(slice) is not int start)
				start = 0;
			if (sliceBaseField?.GetValue(slice) is G.IList<T> list)
				for (var i = 0; i < length; i++)
					_items[i] = list[start + i];
			else if (sliceBase2Field?.GetValue(slice) is G.IReadOnlyList<T> list2)
				for (var i = 0; i < length; i++)
					_items[i] = list2[start + i];
			_size = length;
		}
		else if (collection is G.IReadOnlyList<T> rol)
		{
			var length = rol.Count;
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(length);
			for (var i = 0; i < length; i++)
				_items[i] = rol[i];
			_size = length;
		}
		else
		{
			_size = 0;
			lock (globalLockObj)
				_items = arrayPool.GetAndRemove(collection.Length());
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public LimitedBuffer(int capacity, G.IEnumerable<T> collection) : this(capacity)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
		{
			var length = list.Count;
			if (length == 0)
				return;
			if (length > capacity)
				throw new FullBufferException();
			_size = Min(capacity, length);
			var start = length - _size;
			for (var i = start; i < length; i++)
				_items?[i - start] = list[i];
		}
		else
		{
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public LimitedBuffer(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		_items = [.. array];
	}

	public LimitedBuffer(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Length > capacity)
			throw new FullBufferException();
		_size = Min(capacity, array.Length);
		lock (globalLockObj)
			_items = arrayPool.GetAndRemove(capacity);
		Array.Copy(array, array.Length - _size, _items, 0, _size);
	}

	public LimitedBuffer(ReadOnlySpan<T> span)
	{
		_size = span.Length;
		_items = span.ToArray();
	}

	public LimitedBuffer(int capacity, ReadOnlySpan<T> span)
	{
		if (span.Length > capacity)
			throw new FullBufferException();
		_size = Min(capacity, span.Length);
		lock (globalLockObj)
			_items = arrayPool.GetAndRemove(capacity);
		span.Slice(span.Length - _size, _size).CopyTo(_items);
	}

	public override int Capacity
	{
		get => _items?.Length ?? 0;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			if (value == (_items?.Length ?? 0))
				return;
			if (value == 0)
			{
				_items = null;
				return;
			}
			ArgumentOutOfRangeException.ThrowIfZero(value);
			T[] newItems;
			lock (globalLockObj)
				newItems = arrayPool.GetAndRemove(value);
			if (_size > 0)
			{
				Debug.Assert(_items is not null);
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
			throw new FullBufferException();
		else
			SetInternal(_size++, item);
		Changed();
		return (TCertain)this;
	}

	public override Memory<T> AsMemory(int index, int length) => RedStarLinq.ToArray(GetSlice(index, length)).AsMemory();

	public override Span<T> AsSpan(int index, int length) => RedStarLinq.ToArray(GetSlice(index, length)).AsSpan();

	protected override void ClearInternal(int index, int length)
	{
		Debug.Assert(_items is not null);
		if (_start + index + length < Capacity)
			Array.Clear(_items, _start + index, length);
		else if (_start + index < Capacity)
		{
			Array.Clear(_items, _start + index, Capacity - _start - index);
			Array.Clear(_items, 0, length - (Capacity - _start - index));
		}
		else
			Array.Clear(_items, (_start + index) % Capacity, length);
	}

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (destinationIndex + length > destination.Capacity)
			throw new FullBufferException();
		if (this != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
	}

	protected override void CopyToInternal(Array array, int arrayIndex)
	{
		Debug.Assert(_items is not null);
		if (_start + _size < Capacity)
			Array.Copy(_items, _start, array, arrayIndex, _size);
		else
		{
			Array.Copy(_items, _start, array, arrayIndex, Capacity - _start);
			Array.Copy(_items, 0, array, arrayIndex + Capacity - _start, _size - (Capacity - _start));
		}
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		Debug.Assert(_items is not null);
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
		Changed();
		GC.SuppressFinalize(this);
	}

	protected override void EnsureCapacity(int min) =>
		throw new NotSupportedException("Вы докопались до этого метода? Поздравляем, ваши знания C# выше уровня Hello world!"
		+ " Вот только для данной коллекции он неуместен, так как при исчерпании емкости она удаляет элементы,"
		+ " а не увеличивает емкость. Если вам нужно изменить емкость, такие извращения не нужны,"
		+ " достаточно Capacity = value.");

	protected override T GetInternal(int index)
	{
		Debug.Assert(_items is not null);
		return _items[(_start + index) % Capacity];
	}

	protected override int IndexOfInternal(T item, int index, int length)
	{
		for (var i = index; i < index + length; i++)
			if (GetInternal(i)?.Equals(item) ?? item is null)
				return i;
		return -1;
	}

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
			throw new FullBufferException();
		var this2 = (TCertain)this;
		if (index < _size >> 1)
		{
			_start = (_start + Capacity - 1) % Capacity;
			if (index != 0)
				CopyToInternal(1, this2, 0, index);
			_size++;
		}
		else if (index < _size)
			CopyToInternal(index, this2, index + 1, _size - index);
		else
			_size++;
		SetInternal(index, item);
		Changed();
		return this2;
	}

	protected override void InsertInternal(int index, G.IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = collection.Length();
		if (_size + length > Capacity)
			throw new FullBufferException();
		var this2 = (TCertain)this;
		if (length == 0)
			return;
		if (index < _size >> 1)
		{
			_start = (_start + Capacity - length) % Capacity;
			if (index != 0)
				CopyToInternal(length, this2, 0, index);
			_size += length;
		}
		else if (index < _size)
			CopyToInternal(index, this2, index + length, _size - index);
		else
			_size += length;
		using var en = collection.GetEnumerator();
		while (en.MoveNext())
			SetInternal(index++, en.Current);
	}

	protected override void InsertInternal(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = span.Length;
		if (_size + length > Capacity)
			throw new FullBufferException();
		var this2 = (TCertain)this;
		if (length == 0)
			return;
		if (index < _size >> 1)
		{
			_start = (_start + Capacity - length) % Capacity;
			if (index != 0)
				CopyToInternal(length, this2, 0, index);
			_size += length;
		}
		else if (index < _size)
			CopyToInternal(index, this2, index + length, _size - index);
		else
			_size += length;
		for (var i = 0; i < length; i++)
			SetInternal(index + i, span[i]);
	}

	protected override int LastIndexOfInternal(T item, int index, int length)
	{
		var endIndex = index - length + 1;
		for (var i = index; i >= endIndex; i--)
			if (GetInternal(i)?.Equals(item) ?? item is null)
				return i;
		return -1;
	}

	public override TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		_size--;
		if (index < _size >> 1)
		{
			if (index != 0)
				CopyToInternal(0, this2, 1, index);
			_start = (_start + 1) % Capacity;
		}
		else if (index < _size)
			CopyToInternal(index + 1, this2, index, _size - index);
		Changed();
		return this2;
	}

	protected override void RemoveInternal(int index, int length)
	{
		var this2 = (TCertain)this;
		if (length <= 0)
			return;
		_size -= length;
		if (index * 2 + length < _size)
		{
			if (index != 0)
				CopyToInternal(0, this2, length, index);
			_start = (_start + length) % Capacity;
		}
		else if (index < _size)
			CopyToInternal(index + length, this2, index, _size - index);
	}

	public override TCertain Resize(int newSize)
	{
		if (newSize == _size)
			return (TCertain)this;
		if (newSize > Capacity)
			throw new FullBufferException();
		_size = newSize;
		Changed();
		return (TCertain)this;
	}

	public override TCertain ResizeLeft(int newSize)
	{
		if (newSize == _size)
			return (TCertain)this;
		if (newSize > Capacity)
			throw new FullBufferException();
		_start = (_start + Capacity + _size - newSize) % Capacity;
		_size = newSize;
		Changed();
		return (TCertain)this;
	}

	protected override void ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
		{
			var temp = GetInternal(index + i);
			SetInternal(index + i, GetInternal(index + length - i - 1));
			SetInternal(index + length - i - 1, temp);
		}
	}

	protected override void SetInternal(int index, T value)
	{
		Debug.Assert(_items is not null);
		_items[(_start + index) % Capacity] = value;
	}

	protected override void SetRangeInternal(int index, int length, TCertain list)
	{
		if (index + length > Capacity)
			throw new FullBufferException();
		var this2 = (TCertain)this;
		if (length > 0)
			list.CopyToInternal(0, this2, index, length);
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class LimitedBuffer<T> : LimitedBuffer<T, LimitedBuffer<T>>
{
	public LimitedBuffer() { }

	public LimitedBuffer(int capacity) : base(capacity) { }

	public LimitedBuffer(G.IEnumerable<T> collection) : base(collection) { }

	public LimitedBuffer(int capacity, G.IEnumerable<T> collection) : base(capacity, collection) { }

	public LimitedBuffer(int capacity, params T[] array) : base(capacity, array) { }

	public LimitedBuffer(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public LimitedBuffer(params T[] array) : base(array) { }

	public LimitedBuffer(ReadOnlySpan<T> span) : base(span) { }

	protected override Func<int, LimitedBuffer<T>> CapacityCreator { get; } = x => new(x);

	protected override Func<G.IEnumerable<T>, LimitedBuffer<T>> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<T>, LimitedBuffer<T>> SpanCreator { get; } = x => new(x);

	public static implicit operator LimitedBuffer<T>(T x) => new(x);

	public static implicit operator LimitedBuffer<T>(T[] x) => new(x);

	public static explicit operator LimitedBuffer<T>((T, T) x) => [x.Item1, x.Item2];

	public static explicit operator LimitedBuffer<T>((T, T, T) x) => [x.Item1, x.Item2, x.Item3];

	public static explicit operator LimitedBuffer<T>((T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static explicit operator LimitedBuffer<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (T, T)(LimitedBuffer<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException("Список должен иметь 2 элемента.");

	public static explicit operator (T, T, T)(LimitedBuffer<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException("Список должен иметь 3 элемента.");

	public static explicit operator (T, T, T, T)(LimitedBuffer<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException("Список должен иметь 4 элемента.");

	public static explicit operator (T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException("Список должен иметь 5 элементов.");

	public static explicit operator (T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException("Список должен иметь 6 элементов.");

	public static explicit operator (T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException("Список должен иметь 7 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException("Список должен иметь 8 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException("Список должен иметь 9 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException("Список должен иметь 10 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException("Список должен иметь 11 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException("Список должен иметь 12 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException("Список должен иметь 13 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException("Список должен иметь 14 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException("Список должен иметь 15 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(LimitedBuffer<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException("Список должен иметь 16 элементов.");
}

public class FullBufferException : Exception
{
	public FullBufferException() : base("Ограниченный буфер не может вместить столько элементов."
		+ " Используйте класс Buffer<T>, если вам нужно при переполнении автоматически удалять элементы из начала,"
		+ " или List<T>, если нужно автоматически увеличивать емкость.")
	{
	}

	public FullBufferException(string? message) : base(message)
	{
	}

	public FullBufferException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
