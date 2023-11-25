﻿
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Chain : IReadOnlyCollection<int>
{
	private readonly int start;

	public Chain(int length) : this(0, length) { }

	public Chain(int start, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		this.start = start;
		Length = length;
	}

	public virtual int Length { get; }

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual List<int> ToList()
	{
		List<int> list = new(Length);
		for (var i = 0; i < Length; i++)
			list.Add(start + i);
		return list;
	}

	public struct Enumerator(Chain chain) : IEnumerator<int>
	{
		private readonly Chain chain = chain;
		private int index = 0;

		public int Current { get; private set; } = chain.start;

		readonly object IEnumerator.Current => Current;

		public void Dispose() => index = chain.Length;

		public bool MoveNext()
		{
			if (index < chain.Length)
			{
				Current = chain.start + index++;
				return true;
			}
			else
			{
				Current = chain.start + chain.Length;
				return false;
			}
		}

		public void Reset() => index = 0;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Group<T, TKey> : List<T>
{
	public virtual TKey Key { get; private set; }

	public Group(int capacity, TKey key) : base(capacity) => Key = key;

	public Group(IEnumerable<T> collection, TKey key) : base(collection) => Key = key;
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Queue<T> : IEnumerable<T>, ICollection, IReadOnlyCollection<T>, ICloneable
{
	private T[] _array;
	private int _head;
	private int _tail;
	private int _size;
	private const int _MinimumGrow = 4;
	[NonSerialized]
	private readonly object _syncRoot = new();

	internal virtual int Capacity => _array.Length;
	public virtual int Length => _size;

	int System.Collections.ICollection.Count => Length;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	public Queue() : this(32) { }

	public Queue(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_array = new T[capacity];
		_head = 0;
		_tail = 0;
		_size = 0;
	}

	public Queue(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetLengthEasilyEnumerable(col, out var length) ? length : 32)
	{
		var en = col.GetEnumerator();
		while (en.MoveNext())
			Enqueue(en.Current);
	}

	public virtual object Clone()
	{
		Queue<T> q = new(_size) { _size = _size };
		var numToCopy = _size;
		var firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
		Array.Copy(_array, _head, q._array, 0, firstPart);
		numToCopy -= firstPart;
		if (numToCopy > 0)
			Array.Copy(_array, 0, q._array, _array.Length - _head, numToCopy);
		return q;
	}

	public virtual void Clear()
	{
		if (_head < _tail)
			Array.Clear(_array, _head, _size);
		else
		{
			Array.Clear(_array, _head, _array.Length - _head);
			Array.Clear(_array, 0, _tail);
		}
		_head = 0;
		_tail = 0;
		_size = 0;
	}

	public virtual void CopyTo(Array array, int index)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException();
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		var arrayLen = array.Length;
		if (arrayLen - index < _size)
			throw new ArgumentException(null);
		var numToCopy = _size;
		if (numToCopy == 0)
			return;
		var firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
		Array.Copy(_array, _head, array, index, firstPart);
		numToCopy -= firstPart;
		if (numToCopy > 0)
			Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
	}

	public virtual void Enqueue(T obj)
	{
		if (_size == _array.Length)
		{
			var newCapacity = _array.Length * 2;
			if (newCapacity < _array.Length + _MinimumGrow)
				newCapacity = _array.Length + _MinimumGrow;
			SetCapacity(newCapacity);
		}
		_array[_tail] = obj;
		_tail = (_tail + 1) % _array.Length;
		_size++;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual T Dequeue()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		var removed = _array[_head];
		_array[_head] = default!;
		_head = (_head + 1) % _array.Length;
		_size--;
		return removed;
	}

	public virtual T Peek()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		return _array[_head];
	}

	public virtual bool Contains(T? obj)
	{
		var index = _head;
		var length = _size;
		while (length-- > 0)
		{
			if (obj == null && _array[index] == null)
				return true;
			else if (_array[index] != null && (_array[index]?.Equals(obj) ?? false))
				return true;
			index = (index + 1) % _array.Length;
		}
		return false;
	}

	internal T GetElement(int i) => _array[(_head + i) % _array.Length];

	public virtual T[] ToArray()
	{
		var arr = new T[_size];
		if (_size == 0)
			return arr;
		if (_head < _tail)
			Array.Copy(_array, _head, arr, 0, _size);
		else
		{
			Array.Copy(_array, _head, arr, 0, _array.Length - _head);
			Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
		}
		return arr;
	}

	private protected void SetCapacity(int capacity)
	{
		if (Capacity == capacity)
			return;
		var newArray = new T[capacity];
		if (_size > 0)
		{
			if (_head < _tail)
				Array.Copy(_array, _head, newArray, 0, _size);
			else
			{
				Array.Copy(_array, _head, newArray, 0, _array.Length - _head);
				Array.Copy(_array, 0, newArray, _array.Length - _head, _tail);
			}
		}
		_array = newArray;
		_head = 0;
		_tail = (_size == capacity) ? 0 : _size;
	}

	public virtual void TrimExcess() => SetCapacity(_size);

	[Serializable]
	public struct Enumerator : IEnumerator<T>
	{
		private readonly Queue<T> queue;
		private int index;
		private T current;

		public readonly T Current
		{
			get
			{
				if (index == 0 || index == queue._size + 1)
					throw new InvalidOperationException();
				return current;
			}
		}

		internal Enumerator(Queue<T> queue)
		{
			this.queue = queue;
			index = 0;
			current = default!;
		}

		public readonly void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (index < queue._size)
			{
				current = queue.GetElement(index++)!;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = queue._size + 1;
			current = default!;
			return false;
		}

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == queue._size + 1)
					throw new InvalidOperationException();
				return Current!;
			}
		}

		void IEnumerator.Reset()
		{
			index = 0;
			current = default!;
		}
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class LimitedQueue<T> : Queue<T>
{
	public LimitedQueue(int capacity) : base(capacity) { }

	public LimitedQueue(IEnumerable<T> col) : base(col) => SetCapacity(Length);

	public virtual bool IsFull => Length == Capacity;

	public override void Enqueue(T obj)
	{
		if (IsFull)
			base.Dequeue();
		base.Enqueue(obj);
	}

	public virtual void Enqueue(T obj, G.ICollection<T> receiver)
	{
		if (IsFull)
			receiver.Add(base.Dequeue());
		base.Enqueue(obj);
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Slice<T> : BaseIndexable<T, Slice<T>>
{
	private readonly G.IList<T> _base;
	private readonly int _start;

	public Slice() : this(Array.Empty<T>()) { }

	public Slice(G.IList<T> @base) : this(@base, 0, @base.Count) { }

	public Slice(G.IList<T> @base, Index start) : this(@base, start.GetOffset(@base.Count)) { }

	public Slice(G.IList<T> @base, int start) : this(@base, start, @base.Count - start) { }

	public Slice(G.IList<T> @base, int start, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(start);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (start + length > @base.Count)
			throw new ArgumentException(null);
		ArgumentNullException.ThrowIfNull(@base);
		if (@base is Slice<T> slice)
		{
			_base = slice._base;
			_start = slice._start + start;
			_size = length;
		}
		else
		{
			_base = @base;
			_start = start;
			_size = length;
		}
	}

	public Slice(G.IList<T> @base, Range range) : this(range.End.GetOffset(@base.Count) > @base.Count ? throw new ArgumentException(null) : @base, CreateVar(range.GetOffsetAndLength(@base.Count), out var startAndLength).Offset, startAndLength.Length) { }

	public Slice(Slice<T> slice) : this(slice._base, slice._start, slice._size) { }

	public Slice(Slice<T> slice, Index start) : this(slice, start.GetOffset(slice._size)) { }

	public Slice(Slice<T> slice, int start) : this(slice, start, slice._size - start) { }

	public Slice(Slice<T> slice, int start, int length) : this(slice._base, slice._start + start, length) { }

	public Slice(Slice<T> slice, Range range) : this(range.End.GetOffset(slice._size) > slice._size ? throw new ArgumentException(null) : slice._base, slice._start + CreateVar(range.GetOffsetAndLength(slice._size), out var startAndLength).Offset, startAndLength.Length) { }

	public override int Length => _size;

	public override Span<T> AsSpan(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException(null);
		return _base.AsSpan(_start + index, length);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		if (_base is BaseIndexable<T> collection)
			collection.CopyTo(_start + index, array, arrayIndex, length);
		else if (_base is T[] array2)
			Array.Copy(array2, _start + index, array, arrayIndex, length);
		else
			ForEach(x => array[arrayIndex++] = x, index, length);
	}

	public override void Dispose() => GC.SuppressFinalize(this);

	internal override T GetInternal(int index, bool invoke = true) => _base is BaseIndexable<T> collection ? collection.GetInternal(_start + index) : _base[_start + index];

	private protected override Slice<T> GetRangeInternal(int index, int length) => GetSliceInternal(index, length);

	private protected override Slice<T> GetSliceInternal(int index, int length) => new(_base, _start + index, length);

	private protected override int IndexOfInternal(T item, int index, int length)
	{
		if (_base is BaseIndexable<T> collection)
			return CreateVar(collection.IndexOf(item, _start + index, length), out var foundIndex) >= 0 ? foundIndex - _start : foundIndex;
		else if (_base is T[] array)
			return CreateVar(Array.IndexOf(array, item, _start + index, length), out var foundIndex) >= 0 ? foundIndex - _start : foundIndex;
		else
		{
			for (var i = _start + index; i < _start + index + length; i++)
				if (_base[i]?.Equals(item) ?? item == null)
					return i - _start;
			return -1;
		}
	}

	private protected override int LastIndexOfInternal(T item, int index, int length)
	{
		if (_base is BaseIndexable<T> collection)
			return CreateVar(collection.LastIndexOf(item, _start + index, length), out var foundIndex) >= 0 ? foundIndex - _start : foundIndex;
		else if (_base is T[] array)
			return CreateVar(Array.LastIndexOf(array, item, _start + index, length), out var foundIndex) >= 0 ? foundIndex - _start : foundIndex;
		else
		{
			var endIndex = _start + index - length + 1;
			for (var i = _start + index; i >= endIndex; i--)
				if (_base[i]?.Equals(item) ?? item == null)
					return i - _start;
			return -1;
		}
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Stack<T> : IEnumerable<T>, ICollection, IReadOnlyCollection<T>
{
	private T[] _array;     // Storage for stack elements
	private int _size;           // Number of items in the stack.
	[NonSerialized]
	private object _syncRoot = new();

	private const int _defaultCapacity = 4;
	private static readonly T[] _emptyArray = Array.Empty<T>();

	public Stack()
	{
		_array = _emptyArray;
		_size = 0;
	}

	public Stack(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_array = new T[capacity];
		_size = 0;
	}

	//
	public Stack(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			_array = new T[length];
			c.CopyTo(_array, 0);
			_size = length;
		}
		else
		{
			_size = 0;
			_array = new T[_defaultCapacity];
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Push(en.Current);
		}
	}

	public Stack(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		_array = [.. array];
	}

	public Stack(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		if (array.Length > capacity)
			_array = [.. array];
		else
		{
			_array = new T[capacity];
			Array.Copy(array, _array, array.Length);
		}
	}

	public virtual int Length => _size;

	int System.Collections.ICollection.Count => Length;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot
	{
		get
		{
			if (_syncRoot == null)
				Interlocked.CompareExchange<object>(ref _syncRoot!, new(), new());
			return _syncRoot;
		}
	}

	public virtual void Clear()
	{
		Array.Clear(_array, 0, _size); // Don't2 need to doc this but we clear the elements so that the gc can reclaim the references.
		_size = 0;
	}

	public virtual bool Contains(T? item)
	{
		var length = _size;
		var c = EqualityComparer<T>.Default;
		while (length-- > 0)
			if (item == null)
			{
				if (_array[length] == null)
					return true;
			}
			else if (_array[length] != null && c.Equals(_array[length], item))
				return true;
		return false;
	}

	public virtual void CopyTo(T[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < _size)
			throw new ArgumentException(null);
		Array.Copy(_array, 0, array, arrayIndex, _size);
		Array.Reverse(array, arrayIndex, _size);
	}

	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new ArgumentException(null);
		if (array.GetLowerBound(0) != 0)
			throw new ArgumentException(null);
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < _size)
			throw new ArgumentException(null);
		try
		{
			Array.Copy(_array, 0, array, arrayIndex, _size);
			Array.Reverse(array, arrayIndex, _size);
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException(null);
		}
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual T Peek()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		return _array[_size - 1];
	}

	public virtual T Pop()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		var item = _array[--_size];
		_array[_size] = default!;     // Free memory quicker.
		return item;
	}

	public virtual void Push(T item)
	{
		if (_size == _array.Length)
		{
			var newArray = new T[(_array.Length == 0) ? _defaultCapacity : 2 * _array.Length];
			Array.Copy(_array, 0, newArray, 0, _size);
			_array = newArray;
		}
		_array[_size++] = item;
	}

	public virtual T[] ToArray()
	{
		var array = new T[_size];
		for (var i = 0; i < _size; i++)
			array[i] = _array[i];
		return array;
	}

	public virtual List<T> ToList()
	{
		List<T> list = new(_size);
		for (var i = 0; i < _size; i++)
			list.Add(_array[i]);
		return list;
	}

	public virtual void TrimExcess()
	{
		var threshold = (int)(_array.Length * 0.9);
		if (_size < threshold)
		{
			var newarray = new T[_size];
			Array.Copy(_array, 0, newarray, 0, _size);
			_array = newarray;
		}
	}

	public virtual bool TryPeek(out T value)
	{
		if (_size == 0)
		{
			value = default!;
			return false;
		}
		else
		{
			value = Peek();
			return true;
		}
	}

	public virtual bool TryPop(out T value)
	{
		if (_size == 0)
		{
			value = default!;
			return false;
		}
		else
		{
			value = Pop();
			return true;
		}
	}

	[Serializable()]
	public struct Enumerator : IEnumerator<T>,
		IEnumerator
	{
		private readonly Stack<T> stack;
		private int index;
		private T current;

		internal Enumerator(Stack<T> stack)
		{
			this.stack = stack;
			index = 0;
			current = default!;
		}

		public void Dispose() => index = stack._size + 1;

		public bool MoveNext()
		{
			if (index < stack._size)
			{
				current = stack._array[index++];
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = stack._size + 1;
			current = default!;
			return false;
		}

		public readonly T Current => current;

		readonly object IEnumerator.Current => Current!;

		void IEnumerator.Reset()
		{
			index = -2;
			current = default!;
		}
	}
}
