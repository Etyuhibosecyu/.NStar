using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Corlib.NStar;

[ComVisible(false)]
[DebuggerDisplay("Length = {Length}")]
[Serializable()]
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
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		_array = new T[capacity];
		_size = 0;
	}

	//
	public Stack(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is ICollection<T> c)
		{
			int count = c.Count;
			_array = new T[count];
			c.CopyTo(_array, 0);
			_size = count;
		}
		else
		{
			_size = 0;
			_array = new T[_defaultCapacity];
			using IEnumerator<T> en = collection.GetEnumerator();
			while (en.MoveNext())
				Push(en.Current);
		}
	}

	public Stack(params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		_array = array.ToArray();
	}

	public Stack(int capacity, params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		if (array.Length > capacity)
			_array = array.ToArray();
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

	public void Clear()
	{
		Array.Clear(_array, 0, _size); // Don't2 need to doc this but we clear the elements so that the gc can reclaim the references.
		_size = 0;
	}

	public bool Contains(T? item)
	{
		int count = _size;
		EqualityComparer<T> c = EqualityComparer<T>.Default;
		while (count-- > 0)
			if (item == null)
			{
				if (_array[count] == null)
					return true;
			}
			else if (_array[count] != null && c.Equals(_array[count], item))
				return true;
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (arrayIndex < 0 || arrayIndex > array.Length)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < _size)
			throw new ArgumentException(null);
		Array.Copy(_array, 0, array, arrayIndex, _size);
		Array.Reverse(array, arrayIndex, _size);
	}

	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
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

	public Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public T Peek()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		return _array[_size - 1];
	}

	public T Pop()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		T item = _array[--_size];
		_array[_size] = default!;     // Free memory quicker.
		return item;
	}

	public void Push(T item)
	{
		if (_size == _array.Length)
		{
			T[] newArray = new T[(_array.Length == 0) ? _defaultCapacity : 2 * _array.Length];
			Array.Copy(_array, 0, newArray, 0, _size);
			_array = newArray;
		}
		_array[_size++] = item;
	}

	public T[] ToArray()
	{
		T[] array = new T[_size];
		for (int i = 0; i < _size; i++)
			array[i] = _array[i];
		return array;
	}

	public List<T> ToList()
	{
		List<T> list = new(_size);
		for (int i = 0; i < _size; i++)
			list.Add(_array[i]);
		return list;
	}

	public void TrimExcess()
	{
		int threshold = (int)(_array.Length * 0.9);
		if (_size < threshold)
		{
			T[] newarray = new T[_size];
			Array.Copy(_array, 0, newarray, 0, _size);
			_array = newarray;
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

		public T Current => current;

		object IEnumerator.Current => Current!;

		void IEnumerator.Reset()
		{
			index = -2;
			current = default!;
		}
	}
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
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

	public Queue() : this(32)
	{
	}

	public Queue(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		_array = new T[capacity];
		_head = 0;
		_tail = 0;
		_size = 0;
	}

	public Queue(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetCountEasilyEnumerable(col, out int count) ? count : 32)
	{
		IEnumerator<T> en = col.GetEnumerator();
		while (en.MoveNext())
			Enqueue(en.Current);
	}

	public virtual object Clone()
	{
		Queue<T> q = new(_size) { _size = _size };
		int numToCopy = _size;
		int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
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
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (array.Rank != 1)
			throw new RankException();
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		int arrayLen = array.Length;
		if (arrayLen - index < _size)
			throw new ArgumentException(null);
		int numToCopy = _size;
		if (numToCopy == 0)
			return;
		int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
		Array.Copy(_array, _head, array, index, firstPart);
		numToCopy -= firstPart;
		if (numToCopy > 0)
			Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
	}

	public virtual void Enqueue(T obj)
	{
		if (_size == _array.Length)
		{
			int newCapacity = _array.Length * 2;
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
		T removed = _array[_head];
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
		int index = _head;
		int count = _size;
		while (count-- > 0)
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
		T[] arr = new T[_size];
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

	private void SetCapacity(int capacity)
	{
		T[] newArray = new T[capacity];
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

		public T Current
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

		public void Dispose()
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

		object IEnumerator.Current
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

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public class BigQueue<T> : IEnumerable<T>, ICloneable
{
	private Queue<T>? low;
	private Queue<BigQueue<T>>? high;
	private mpz_t _size;
	private mpz_t fragment;
	private bool isHigh;
	public virtual mpz_t Count => _size;
	private const int CapacityStepBitLength = 16, CapacityFirstStepBitLength = 16;
	private const int CapacityStep = 1 << CapacityStepBitLength, CapacityFirstStep = 1 << CapacityFirstStepBitLength;

	public BigQueue() : this(32)
	{
	}

	public BigQueue(mpz_t capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity <= CapacityFirstStep)
		{
			low = new((int)capacity);
			high = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << ((((capacity - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
			high = new((int)((capacity + (fragment - 1)) / fragment));
			for (mpz_t i = 0; i < capacity / fragment; i++)
				high.Enqueue(new(fragment));
			high.Enqueue(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
	}

	public BigQueue(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetCountEasilyEnumerable(col, out int count) ? count : 32)
	{
		IEnumerator<T> en = col.GetEnumerator();
		while (en.MoveNext())
			Enqueue(en.Current);
	}

	public virtual object Clone()
	{
		BigQueue<T> q = new(_size) { _size = _size };
		if (!isHigh && low != null)
		{
			q.low = (Queue<T>)low.Clone();
			q.high = null;
		}
		else if (high != null)
		{
			q.low = null;
			q.high = (Queue<BigQueue<T>>)high.Clone();
		}
		q.isHigh = isHigh;
		return q;
	}

	public virtual void Clear()
	{
		if (!isHigh && low != null)
			low.Clear();
		else high?.Clear();
	}

	public virtual void CopyTo(Array array, int index)
	{
		if (!isHigh && low != null)
			low.CopyTo(array, index);
		else
			throw new InvalidOperationException("Слишком большая очередь для копирования в массив!");
	}

	public virtual void Enqueue(T obj)
	{
		if (_size == CapacityFirstStep && !isHigh && low != null)
		{
			high = new(4);
			high.Enqueue(new(CapacityStep) { low = low, _size = low.Length });
			high.Enqueue(new(CapacityStep));
			high.GetElement(1).Enqueue(obj);
			low = null;
			fragment = CapacityFirstStep;
			isHigh = true;
		}
		else if (!isHigh && low != null)
			low.Enqueue(obj);
		else if (high != null)
		{
			int index = (int)(_size / fragment);
			if (index == CapacityStep)
			{
				Queue<BigQueue<T>> temp = high;
				high = new(4);
				high.Enqueue(new(_size) { high = temp, _size = temp.Length });
				high.Enqueue(new(_size));
				high.GetElement(high.Length - 1).Enqueue(obj);
				fragment *= CapacityStep;
			}
			else if (high.GetElement(index)._size == fragment)
			{
				high.Enqueue(new(CapacityStep));
				high.GetElement(index).Enqueue(obj);
			}
			else
				high.GetElement(index).Enqueue(obj);
		}
		_size++;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual T Dequeue()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		_size--;
		if (!isHigh && low != null)
			return low.Dequeue();
		else if (high != null)
		{
			T removed = high.Peek().Dequeue();
			if (high.Peek()._size == 0)
				high.Dequeue();
			return removed;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual T Peek()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		if (!isHigh && low != null)
			return low.Peek();
		else if (high != null)
			return high.Peek().Peek();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private Queue<T> PeekQueue()
	{
		if (!isHigh && low != null)
			return low;
		else if (high != null)
			return high.Peek().PeekQueue();
		else
			return new();
	}

	public virtual bool Contains(T? obj)
	{
		if (!isHigh && low != null)
			return low.Contains(obj);
		else if (high != null)
		{
			for (int i = 0; i < high.Length; i++)
				if (high.GetElement(i).Contains(obj))
					return true;
			return false;
		}
		else
			return false;
	}

	internal T GetElement(mpz_t i)
	{
		if (!isHigh && low != null)
			return low.GetElement((int)(i % CapacityStep));
		else if (high != null)
			return high.GetElement((int)(i / fragment)).GetElement(i % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual T[] ToArray()
	{
		if (!isHigh && low != null)
			return low.ToArray();
		else
			throw new InvalidOperationException("Слишком большая очередь для преобразования в массив!");
	}

	public virtual void TrimExcess()
	{
		if (_size <= CapacityFirstStep)
		{
			low = PeekQueue();
			low.TrimExcess();
		}
		else if (high != null)
		{
			high.TrimExcess();
			high.GetElement(high.Length - 1).TrimExcess();
		}
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>
	{
		private readonly BigQueue<T> queue;
		private mpz_t index;
		private T current;

		public T Current
		{
			get
			{
				if (index == 0 || index == queue._size + 1)
					throw new InvalidOperationException();
				return current;
			}
		}

		internal Enumerator(BigQueue<T> queue)
		{
			this.queue = queue;
			index = 0;
			current = default!;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (index < queue._size)
			{
				current = queue.GetElement(index)!;
				index++;
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

		object IEnumerator.Current
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

public class LimitedQueue<T> : Queue<T>
{
	public LimitedQueue(int capacity) : base(capacity)
	{
	}

	public LimitedQueue(IEnumerable<T> col) : base(col)
	{
	}

	public override void Enqueue(T obj)
	{
		if (Length == Capacity)
			base.Dequeue();
		base.Enqueue(obj);
	}
}

public class HashSet<T> : ListBase<T, HashSet<T>>, ISet<T>, ICollection
{
	private struct Entry
	{
		public int hashCode = -1;
		public int next = -1;
		public T item = default!;

		public Entry()
		{
		}
	}

	private List<int> buckets;
	private List<Entry> entries;
	private int freeList;
	private int freeCount;

	internal const int MaxPrimeArrayLength = 0x7FEFFFFD;
	internal const int HashPrime = 101;
	internal static readonly int[] primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

	public HashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public HashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public HashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public HashSet(int capacity, IEqualityComparer<T>? comparer)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity > 0)
			Initialize(capacity, out buckets, out entries);
		else
		{
			buckets = default!;
			entries = default!;
		}
		Comparer = comparer ?? EqualityComparer<T>.Default;
	}

	public HashSet(IEnumerable<T> set) : this(set, null) { }

	public HashSet(IEnumerable<T> set, IEqualityComparer<T>? comparer) : this(set != null && set.TryGetCountEasily(out int count) ? count : 0, comparer)
	{
		if (set == null)
			throw new ArgumentNullException(nameof(set));
		foreach (T pair in set)
			TryAdd(pair);
	}

	public HashSet(int capacity, IEnumerable<T> set) : this(capacity, set, null) { }

	public HashSet(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (set == null)
			throw new ArgumentNullException(nameof(set));
		foreach (T pair in set)
			TryAdd(pair);
	}

	public HashSet(params T[] array) : this((IEnumerable<T>)array)
	{
	}

	public HashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array)
	{
	}

	public HashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray())
	{
	}

	public HashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray())
	{
	}

	public override int Capacity
	{
		get => buckets.Length; set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			Resize(value, false);
			ListChanged?.Invoke(this);
		}
	}

	protected override Func<int, HashSet<T>> CapacityCreator => x => new(x);

	protected override Func<IEnumerable<T>, HashSet<T>> CollectionCreator => x => new(x);

	public IEqualityComparer<T> Comparer { get; private set; }

	public virtual int FullLength => _size;

	public override int Length => _size - freeCount;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	public override T this[Index index, bool invoke = true]
	{
		get
		{
			int index2 = index.IsFromEnd ? entries.Length - index.Value : index.Value;
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set
		{
			int index2 = index.IsFromEnd ? entries.Length - index.Value : index.Value;
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			if (entries[index2].item?.Equals(value) ?? false)
				return;
			if (Contains(value))
				throw new ArgumentException(null, nameof(value));
			SetInternal(index2, value);
		}
	}

	public event ListChangedHandler? ListChanged;

	public override HashSet<T> Add(T item)
	{
		if (!Contains(item))
			return Insert(item, true);
		return this;
	}

	bool ISet<T>.Add(T item) => TryAdd(item);

	public override Span<T> AsSpan(int index, int count) => throw new NotSupportedException();

	protected override void ClearInternal()
	{
		if (_size > 0)
		{
			for (int i = 0; i < buckets.Length; i++)
			{
				buckets[i] = -1;
				entries[i] = new();
			}
			freeList = -1;
			_size = 0;
			freeCount = 0;
		}
	}

	protected override void ClearInternal(int index, int count)
	{
		for (int i = 0; i < count; i++)
			RemoveValue(GetInternal(index + i));
		ListChanged?.Invoke(this);
	}

	public override bool Contains(T? item) => item != null && IndexOf(item) >= 0;

	protected override void Copy(ListBase<T, HashSet<T>> source, int sourceIndex, ListBase<T, HashSet<T>> destination, int destinationIndex, int count)
	{
		if (destination is not HashSet<T> destination2)
			throw new InvalidOperationException();
		if (source != destination || sourceIndex >= destinationIndex)
			for (int i = 0; i < count; i++)
				destination2.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		else
			for (int i = count - 1; i >= 0; i--)
				destination2.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		destination2.ListChanged?.Invoke(this);
	}

	protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count)
	{
		int skipped = 0;
		for (int i = 0; i < index; i++)
			if (entries[i].hashCode < 0)
				skipped++;
		for (int i = 0; i < count; i++)
			if (entries[i].hashCode >= 0)
				array[arrayIndex++] = entries[index + i + skipped].item;
			else
				count++;
		ListChanged?.Invoke(this);
	}

	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		freeCount = 0;
		freeList = -1;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	protected override bool EqualsInternal(IEnumerable<T>? collection, int index)
	{
		try
		{
			throw new ExperimentalException();
		}
		catch
		{
		}
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				while (entries[index].hashCode < 0)
					index++;
				if (!(GetInternal(index++)?.Equals(list[i]) ?? list[i] is null))
					return false;
			}
			return true;
		}
		else
		{
			foreach (T item in collection)
			{
				while (entries[index].hashCode < 0)
					index++;
				if (!(GetInternal(index++)?.Equals(item) ?? item is null))
					return false;
			}
			return true;
		}
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			RemoveValue(item);
	}

	internal static int ExpandPrime(int oldSize)
	{
		int newSize = 2 * oldSize;
		if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
			return MaxPrimeArrayLength;
		return GetPrime(newSize);
	}

	public override HashSet<T> FilterInPlace(Func<T, bool> match)
	{
		foreach (T item in this)
			if (!match(item))
				RemoveValue(item);
		return this;
	}

	public override HashSet<T> FilterInPlace(Func<T, int, bool> match)
	{
		int i = 0;
		foreach (T item in this)
			if (!match(item, i++))
				RemoveValue(item);
		return this;
	}

	public override IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	private Enumerator GetEnumeratorInternal() => new(this);

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = entries[index].item;
		if (invoke)
			ListChanged?.Invoke(this);
		return item;
	}

	internal static int GetPrime(int min)
	{
		if (min < 0)
			throw new ArgumentException(null);
		for (int i = 0; i < primes.Length; i++)
		{
			int prime = primes[i];
			if (prime >= min) return prime;
		}
		for (int i = min | 1; i < int.MaxValue; i += 2)
			if (IsPrime(i) && ((i - 1) % HashPrime != 0))
				return i;
		return min;
	}

	protected override int IndexOfInternal(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets != null)
		{
			int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
				if (entries[i].hashCode == hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + count)
					return i;
		}
		return -1;
	}

	private void Initialize(int capacity, out List<int> buckets, out List<Entry> entries)
	{
		int size = GetPrime(capacity);
		buckets = new int[size];
		for (int i = 0; i < buckets.Length; i++)
			buckets[i] = -1;
		entries = new Entry[size];
		for (int i = 0; i < size; i++)
			entries[i] = new();
		freeList = -1;
	}

	public override HashSet<T> Insert(int index, T item)
	{
		if (!Contains(item))
		{
			base.Insert(index, item);
			_size--;
		}
		return this;
	}

	private HashSet<T> Insert(T? item, bool add)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
			if (entries[i].hashCode == hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				return this;
			}
		int index;
		if (freeCount > 0)
		{
			index = freeList;
			freeList = entries[index].next;
			freeCount--;
		}
		else
		{
			if (_size == entries.Length)
			{
				Resize();
				targetBucket = hashCode % buckets.Length;
			}
			index = _size;
			_size++;
		}
		Entry t = entries[index];
		t.hashCode = hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		entries[index] = t;
		buckets[targetBucket] = index;
		return this;
	}

	protected override HashSet<T> InsertInternal(int index, IEnumerable<T> collection)
	{
		HashSet<T> set = new(collection);
		set.ExceptWith(this);
		int count = set._size;
		if (count > 0)
		{
			EnsureCapacity(_size + count);
			if (index < entries.Length - count)
				Copy(this, index, this, index + count, entries.Length - index - count);
			if (this == set)
				return this as HashSet<T> ?? throw new InvalidOperationException();
			else
				Copy(set, 0, this, index, count);
		}
		return this as HashSet<T> ?? throw new InvalidOperationException();
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		if (other is not HashSet<T> set)
			set = new(other);
		foreach (T item in this)
			if (!set.Contains(item))
				RemoveValue(item);
	}

	public static bool IsPrime(int candidate)
	{
		if ((candidate & 1) != 0)
		{
			int limit = (int)Sqrt(candidate);
			for (int i = 0, divisor; i < primesList.Length && (divisor = primesList[i]) <= limit; i++)
				if ((candidate % divisor) == 0)
					return false;
			return true;
		}
		return candidate == 2;
	}

	public bool IsProperSubsetOf(IEnumerable<T> other) => !SetEquals(other is ISet<T> set ? set : set = new HashSet<T>(other)) && IsSubsetOf(set);

	public bool IsProperSupersetOf(IEnumerable<T> other) => !SetEquals(other) && IsSupersetOf(other);

	public bool IsSubsetOf(IEnumerable<T> other) => other is ISet<T> set ? set.IsSupersetOf(this) : IsSubsetOf(new HashSet<T>(other));

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		foreach (T item in other)
			if (!Contains(item))
				return false;
		return true;
	}

	protected override int LastIndexOfInternal(T item, int index, int count) => throw new NotSupportedException();

	public bool Overlaps(IEnumerable<T> other)
	{
		foreach (T item in other)
			if (Contains(item))
				return true;
		return false;
	}

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		int last = -1;
		for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
			if (entries[i].hashCode == hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					Entry t = entries[last];
					t.next = entries[i].next;
					entries[last] = t;
				}
				Entry t2 = entries[i];
				t2.hashCode = -1;
				t2.next = freeList;
				t2.item = default!;
				entries[i] = t2;
				freeList = i;
				freeCount++;
				return true;
			}
		return false;
	}

	public override HashSet<T> RemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return this;
		if (entries[index].item == null)
			return this;
		int hashCode = Comparer.GetHashCode(entries[index].item ?? throw new ArgumentException(null)) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		if (bucket != index)
		{
			Entry t = entries[bucket];
			t.next = entries[index].next;
			entries[bucket] = t;
		}
		Entry t2 = entries[index];
		t2.hashCode = -1;
		t2.next = freeList;
		t2.item = default!;
		entries[index] = t2;
		freeList = index;
		freeCount++;
		return this;
	}

	private void Resize() => Resize(ExpandPrime(_size), false);

	private void Resize(int newSize, bool forceNewHashCodes)
	{
		int[] newBuckets = new int[newSize];
		for (int i = 0; i < newBuckets.Length; i++)
			newBuckets[i] = -1;
		List<Entry> newEntries = new Entry[newSize];
		List<Entry>.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		for (int i = entries.Length; i < newEntries.Length; i++)
			newEntries[i] = new();
		if (forceNewHashCodes)
			for (int i = 0; i < _size; i++)
			{
				Entry t = newEntries[i];
				if (t.hashCode != -1)
				{
					t.hashCode = Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
					newEntries[i] = t;
				}
			}
		for (int i = 0; i < _size; i++)
			if (newEntries[i].hashCode >= 0)
			{
				int bucket = newEntries[i].hashCode % newSize;
				Entry t = newEntries[i];
				t.next = newBuckets[bucket];
				newEntries[i] = t;
				newBuckets[bucket] = i;
			}
		buckets = newBuckets;
		entries = newEntries;
	}

	protected override HashSet<T> ReverseInternal(int index, int count) => throw new NotSupportedException();

	public bool SetEquals(IEnumerable<T> other)
	{
		if (other.TryGetCountEasily(out int count))
		{
			if (Length != count)
				return false;
			foreach (T item in other)
				if (!Contains(item))
					return false;
			return true;
		}
		else
		{
			HashSet<T> set = new(other);
			if (Length != set.Length)
				return false;
			foreach (T item in set)
				if (!Contains(item))
					return false;
			return true;
		}
	}

	internal override void SetInternal(int index, T item)
	{
		if (entries[index].item != null)
			RemoveAt(index);
		if (item == null)
			return;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		if (freeCount > 0 && freeList == index)
		{
			freeList = entries[index].next;
			freeCount--;
		}
		else
		{
			if (_size == entries.Length)
			{
				Resize();
				targetBucket = hashCode % buckets.Length;
			}
			_size++;
		}
		Entry t2 = entries[index];
		t2.hashCode = hashCode;
		t2.next = buckets[targetBucket];
		t2.item = item;
		entries[index] = t2;
		buckets[targetBucket] = index;
		ListChanged?.Invoke(this);
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		HashSet<T> added = new(), removed = new();
		foreach (T item in other)
			if (!added.Contains(item) && !removed.Contains(item))
				if (Contains(item))
				{
					RemoveValue(item);
					removed.TryAdd(item);
				}
				else
				{
					TryAdd(item);
					added.TryAdd(item);
				}
	}

	public bool TryAdd(T item)
	{
		if (Contains(item))
			return false;
		Insert(item, true);
		return true;
	}

	public void UnionWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			TryAdd(item);
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly HashSet<T> dictionary;
		private int index;

		internal Enumerator(HashSet<T> dictionary)
		{
			this.dictionary = dictionary;
			index = 0;
			Current = default!;
		}

		public bool MoveNext()
		{
			while ((uint)index < (uint)dictionary.entries.Length)
			{
				if (dictionary.entries[index].hashCode >= 0)
				{
					Current = dictionary.entries[index].item;
					index++;
					return true;
				}
				index++;
			}
			index = dictionary._size + 1;
			Current = default!;
			return false;
		}

		public T Current { get; private set; }

		object? IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		void IEnumerator.Reset()
		{
			index = 0;
			Current = default!;
		}
	}
}

public class Chain : IReadOnlyCollection<int>
{
	private readonly int start;

	public Chain(int count) : this(0, count)
	{
	}

	public Chain(int start, int count)
	{
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		this.start = start;
		Length = count;
	}

	public virtual int Length { get; }

	public Enumerator GetEnumerator() => new(this);

	IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public List<int> ToList()
	{
		List<int> list = new();
		for (int i = 0; i < Length; i++)
			list.Add(start + i);
		return list;
	}

	public struct Enumerator : IEnumerator<int>
	{
		private readonly Chain chain;
		private int index;

		public Enumerator(Chain chain)
		{
			this.chain = chain;
			index = 0;
			Current = chain.start;
		}

		public int Current { get; private set; }

		object IEnumerator.Current => Current;

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

public class Group<T, TKey> : List<T>
{
	public TKey Key { get; private set; }

	public Group(int capacity, TKey key) : base(capacity) => Key = key;

	public Group(IEnumerable<T> collection, TKey key) : base(collection) => Key = key;
}
