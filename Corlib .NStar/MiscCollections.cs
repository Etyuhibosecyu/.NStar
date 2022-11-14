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

	public virtual mpz_t Length => _size;

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
