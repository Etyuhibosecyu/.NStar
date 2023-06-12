using System.Diagnostics;

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
			var count = c.Count;
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

	public virtual void Clear()
	{
		Array.Clear(_array, 0, _size); // Don't2 need to doc this but we clear the elements so that the gc can reclaim the references.
		_size = 0;
	}

	public virtual bool Contains(T? item)
	{
		var count = _size;
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

	public virtual void CopyTo(T[] array, int arrayIndex)
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
		T item = _array[--_size];
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

	public Queue() : this(32) { }

	public Queue(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		_array = new T[capacity];
		_head = 0;
		_tail = 0;
		_size = 0;
	}

	public Queue(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetCountEasilyEnumerable(col, out var count) ? count : 32)
	{
		IEnumerator<T> en = col.GetEnumerator();
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
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (array.Rank != 1)
			throw new RankException();
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
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
		var index = _head;
		var count = _size;
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

	public BigQueue() : this(32) { }

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
			fragment = (mpz_t)1 << (GetArrayLength((capacity - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			high = new((int)GetArrayLength(capacity, fragment));
			for (mpz_t i = 0; i < capacity / fragment; i++)
				high.Enqueue(new(fragment));
			high.Enqueue(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
	}

	public BigQueue(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetCountEasilyEnumerable(col, out var count) ? count : 32)
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
			var index = (int)(_size / fragment);
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

	private protected Queue<T> PeekQueue()
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
			for (var i = 0; i < high.Length; i++)
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

		public readonly T Current
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

		public readonly void Dispose()
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

public abstract class BigArray<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow> where TCertain : BigArray<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected TCertain[]? high;
	private protected mpz_t fragment = 1;
	private protected bool isHigh;

	public BigArray() : this(-1) { }

	public BigArray(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		low = new();
		high = null;
		fragment = 1;
		isHigh = false;
		Size2 = 0;
	}

	public BigArray(mpz_t length, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (length <= CapacityFirstStep)
		{
			low = CapacityLowCreator((int)length);
			high = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = default;
			fragment = (mpz_t)1 << (GetArrayLength((length - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			var quotient = (int)length.Divide(fragment, out var remainder);
			var highCount = (int)GetArrayLength(length, fragment);
			high = new TCertain[highCount];
			for (var i = 0; i < quotient; i++)
				high[i] = CapacityCreator(fragment);
			if (remainder != 0)
				high[^1] = CapacityCreator(remainder);
			isHigh = true;
		}
		Size2 = length;
	}

	public BigArray(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(collection == null ? throw new ArgumentNullException(nameof(collection)) : collection.Count(), capacityFirstStepBitLength, capacityStepBitLength)
	{
		IEnumerator<T> en = collection.GetEnumerator();
		mpz_t i = 0;
		while (en.MoveNext())
		{
			SetInternal(i, en.Current);
			i++;
		}
	}

	public BigArray(mpz_t length, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(length, capacityFirstStepBitLength, capacityStepBitLength)
	{
		IEnumerator<T> en = collection.GetEnumerator();
		mpz_t i = 0;
		while (en.MoveNext())
		{
			SetInternal(i, en.Current);
			i++;
		}
	}

	public override mpz_t Capacity
	{
		get => Size;
		set
		{
		}
	}

	private protected virtual int CapacityFirstStepBitLength { get; init; } = 16;

	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	private protected virtual int CapacityStepBitLength { get; init; } = 16;

	private protected virtual int CapacityStep => 1 << CapacityStepBitLength;

	private protected override mpz_t Size { get => Size2; set => throw new NotSupportedException(); }

	private protected virtual mpz_t Size2 { get; init; } = 0;

	public override TCertain Add(T item) => throw new NotSupportedException();

	private protected override void ClearInternal()
	{
		if (!isHigh && low != null)
			low.Clear();
		else if (high != null)
			Array.Clear(high);
		Changed();
	}

	private protected override void ClearInternal(mpz_t index, mpz_t count)
	{
		if (!isHigh && low != null)
			low.Clear((int)index, (int)count);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)((index + count - 1) / fragment);
			if (quotient == quotient2)
			{
				high[quotient].ClearInternal(remainder, count);
				return;
			}
			var previousPart = fragment - remainder;
			high[quotient].ClearInternal(remainder, previousPart);
			for (var i = quotient + 1; i < quotient2; i++)
			{
				high[i].ClearInternal(0, fragment);
				previousPart += fragment;
			}
			high[quotient2].ClearInternal(0, count - previousPart);
		}
		Changed();
	}

	public override bool Contains(T item, mpz_t index, mpz_t count)
	{
		if (index > Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > Size)
			throw new ArgumentException(null);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		if (!isHigh && low != null)
			return low.Contains(item, (int)index, (int)count);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)((index + count - 1) / fragment);
			if (quotient == quotient2)
				return high[quotient].Contains(item, remainder, count);
			var previousPart = fragment - remainder;
			if (high[quotient].Contains(item, remainder, previousPart))
				return true;
			for (var i = quotient + 1; i < quotient2; i++)
			{
				if (high[i].Contains(item))
					return true;
				previousPart += fragment;
			}
			return high[quotient2].Contains(item, 0, count - previousPart);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void Copy(TCertain sourceBits, mpz_t sourceIndex, TCertain destinationBits, mpz_t destinationIndex, mpz_t length)
	{
		CheckParams(sourceBits, sourceIndex, destinationBits, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		if (!sourceBits.isHigh && sourceBits.low != null && !destinationBits.isHigh && destinationBits.low != null)
		{
			destinationBits.low.SetRangeInternal((int)destinationIndex, (int)length, sourceBits.low.GetRange((int)sourceIndex, (int)length));
			destinationBits.Changed();
			return;
		}
		if (sourceBits.fragment > destinationBits.fragment && sourceBits.isHigh && sourceBits.high != null)
		{
			int index = (int)sourceIndex.Divide(sourceBits.fragment, out var remainder), index2 = (int)((sourceIndex + length - 1) / sourceBits.fragment);
			if (index == index2)
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, length);
			else
			{
				var firstPart = sourceBits.fragment - remainder;
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, firstPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits, 0, destinationBits, sourceIndex + firstPart, sourceBits.fragment);
					firstPart += sourceBits.fragment;
				}
				Copy(sourceBits.high[index2], 0, destinationBits, destinationIndex + firstPart, length - firstPart);
			}
			destinationBits.Changed();
			return;
		}
		if (destinationBits.fragment > sourceBits.fragment && destinationBits.isHigh && destinationBits.high != null)
		{
			int index = (int)destinationIndex.Divide(destinationBits.fragment, out var remainder), index2 = (int)((destinationIndex + length - 1) / destinationBits.fragment);
			if (index == index2)
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, length);
			else
			{
				var firstPart = destinationBits.fragment - remainder;
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, firstPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits, sourceIndex + firstPart, destinationBits, 0, destinationBits.fragment);
					firstPart += destinationBits.fragment;
				}
				Copy(sourceBits, sourceIndex + firstPart, destinationBits.high[index2], 0, length - firstPart);
			}
			destinationBits.Changed();
			return;
		}
		if (!(sourceBits.isHigh && sourceBits.high != null && destinationBits.isHigh && destinationBits.high != null && sourceBits.fragment == destinationBits.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		TCertain[] sourceBits2 = sourceBits.high, destinationBits2 = destinationBits.high;
		mpz_t fragment = sourceBits.fragment;
		var sourceIntIndex = (int)sourceIndex.Divide(fragment, out var sourceBitsIndex);               // Целый индекс в исходном массиве.
		var destinationIntIndex = (int)destinationIndex.Divide(fragment, out var destinationBitsIndex);     // Целый индекс в целевом массиве.
		var intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		var sourceEndIntIndex = (int)sourceEndIndex.Divide(fragment, out var sourceEndBitsIndex);  // Индекс инта последнего бита.
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		var destinationEndIntIndex = (int)destinationEndIndex.Divide(fragment, out var destinationEndBitsIndex);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			if (sourceEndIntIndex == sourceIntIndex)
				Copy(sourceBits2[sourceIntIndex], sourceBitsIndex, destinationBits2[destinationIntIndex], destinationBitsIndex, length);
			else
			{
				var firstPart = fragment - sourceBitsIndex;
				Copy(sourceBits2[sourceIntIndex], sourceBitsIndex, destinationBits2[destinationIntIndex], destinationBitsIndex, firstPart);
				Copy(sourceBits2[sourceEndIntIndex], 0, destinationBits2[destinationIntIndex], destinationBitsIndex + firstPart, length - firstPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
		{
			TCertain buff = CapacityCreator(fragment << 1);
			Copy(destinationBits2[destinationIntIndex], 0, buff, 0, destinationBitsIndex);
			Copy(sourceBits2[sourceIntIndex], sourceBitsIndex, buff, destinationBitsIndex, fragment - sourceBitsIndex);
			var buffBitsIndex = destinationBitsIndex + (fragment - sourceBitsIndex);
			var secondPart = buffBitsIndex >= fragment;
			if (secondPart)
				buffBitsIndex -= fragment;
			for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; sourceCurrentIntIndex < sourceEndIntIndex + 1 || destinationCurrentIntIndex < destinationEndIntIndex;)
			{
				if (!secondPart)
				{
					Copy(sourceBits2[sourceCurrentIntIndex], 0, buff, buffBitsIndex, sourceCurrentIntIndex++ == sourceEndIntIndex ? sourceEndBitsIndex + 1 : fragment);
					secondPart = true;
				}
				if (secondPart && destinationCurrentIntIndex < destinationEndIntIndex && buff.high != null)
				{
					destinationBits2[destinationCurrentIntIndex++] = buff.high[0];
					(buff.high[0], buff.high[1]) = (buff.high[1], CapacityCreator(fragment));
					secondPart = false;
				}
			}
			Copy(buff, 0, destinationBits2[destinationEndIntIndex], 0, destinationEndBitsIndex + 1);
		}
		else
		{
			TCertain buff = CapacityCreator(fragment << 1);
			Copy(sourceBits2[sourceEndIntIndex], 0, buff, 0, sourceEndBitsIndex + 1);
			Copy(destinationBits2[destinationEndIntIndex], destinationEndBitsIndex + 1, buff, sourceEndBitsIndex + 1, RedStarLinq.Min(fragment, destinationBits2[destinationEndIntIndex].Length) - (destinationEndBitsIndex + 1));
			var buffBitsIndex = sourceEndBitsIndex + (fragment - destinationEndBitsIndex);
			var secondPart = buffBitsIndex >= fragment;
			if (buffBitsIndex >= fragment)
				buffBitsIndex -= fragment;
			for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; sourceCurrentIntIndex > sourceIntIndex - 1 || destinationCurrentIntIndex > destinationIntIndex;)
			{
				if (!secondPart && sourceCurrentIntIndex >= 0 && buff.high != null)
				{
					(buff.high[1], buff.high[0]) = (buff.high[0], sourceBits2[sourceCurrentIntIndex--]);
					secondPart = true;
				}
				//if (sourceCurrentIntIndex <= sourceIntIndex - 1)
				//	break;
				if (secondPart && destinationCurrentIntIndex > destinationIntIndex)
				{
					Copy(buff, buffBitsIndex, destinationBits2[destinationCurrentIntIndex], 0, RedStarLinq.Min(fragment, destinationBits2[destinationCurrentIntIndex--].Length));
					secondPart = false;
				}
			}
			Copy(buff, 0, destinationBits2[destinationIntIndex], destinationBitsIndex, fragment - destinationBitsIndex);
		}
		destinationBits.Changed();
	}

	private static void CheckParams(TCertain sourceBits, mpz_t sourceIndex, TCertain destinationBits, mpz_t destinationIndex, mpz_t length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Size == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Size == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceBits.Size)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationBits.Size)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	private protected override void CopyToInternal(mpz_t index, T[] array, int arrayIndex, int count)
	{
		if (count == 0)
			return;
		if (!isHigh && low != null)
		{
			var index2 = (int)index;
			for (var i = 0; i < count; i++)
				array[arrayIndex + i] = low[index2 + i];
		}
		else if (high != null)
		{
			var intIndex = (int)index.Divide(fragment, out var bitsIndex);
			var endIntIndex = (int)(index + count - 1).Divide(fragment, out var endBitsIndex);
			if (endIntIndex == intIndex)
			{
				high[intIndex].CopyToInternal(bitsIndex, array, arrayIndex, count);
				return;
			}
			high[intIndex].CopyToInternal(bitsIndex, array, arrayIndex, (int)(fragment - bitsIndex));
			var destIndex = arrayIndex + fragment - bitsIndex;
			for (var i = intIndex + 1; i < endIntIndex; i++, destIndex += fragment)
				high[i].CopyToInternal(0, array, (int)destIndex, (int)fragment);
			high[endIntIndex].CopyToInternal(0, array, (int)destIndex, (int)(endBitsIndex + 1));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void CopyToInternal(mpz_t index, IBigList<T> list, mpz_t listIndex, mpz_t count)
	{
		if (count == 0)
			return;
		if (!isHigh && low != null)
		{
			int index2 = (int)index, count2 = (int)count;
			for (var i = 0; i < count2; i++)
				list[listIndex + i] = low[index2 + i];
		}
		else if (high != null)
		{
			var intIndex = (int)index.Divide(fragment, out var bitsIndex);
			var endIntIndex = (int)(index + count - 1).Divide(fragment, out var endBitsIndex);
			if (endIntIndex == intIndex)
			{
				high[intIndex].CopyToInternal(bitsIndex, list, listIndex, count);
				return;
			}
			high[intIndex].CopyToInternal(bitsIndex, list, listIndex, fragment - bitsIndex);
			var destIndex = listIndex + fragment - bitsIndex;
			for (var i = intIndex + 1; i < endIntIndex; i++, destIndex += fragment)
				high[i].CopyToInternal(0, list, destIndex, fragment);
			high[endIntIndex].CopyToInternal(0, list, destIndex, endBitsIndex + 1);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override T GetInternal(mpz_t index, bool invoke = true)
	{
		T item;
		if (!isHigh && low != null)
		{
			//try
			//{
			//	throw new ExperimentalException();
			//}
			//catch
			//{
			//}
			item = low.GetInternal((int)index);
		}
		else if (high != null)
			item = high[(int)(index / fragment)].GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		if (invoke)
			Changed();
		return item;
	}

	public override TCertain GetRange(mpz_t index, mpz_t count, bool alwaysCopy = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > Size)
			throw new ArgumentException(null);
		if (count == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && count == Size && this is TCertain thisList)
			return thisList;
		TCertain list = CapacityCreator(count);
		Copy(this as TCertain ?? throw new InvalidOperationException(), index, list, 0, count);
		return list;
	}

	private protected override void SetInternal(mpz_t index, T value)
	{
		if (!isHigh && low != null)
		{
			//try
			//{
			//	throw new ExperimentalException();
			//}
			//catch
			//{
			//}
			low.SetInternal((int)index, value);
		}
		else if (high != null)
			high[(int)(index / fragment)].SetInternal(index % fragment, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Changed();
	}
}

public class BigArray<T> : BigArray<T, BigArray<T>, List<T>>
{
	public BigArray() { }

	public BigArray(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigArray(mpz_t length, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(length, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigArray(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(collection, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigArray(mpz_t length, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(length, collection, capacityStepBitLength, capacityFirstStepBitLength) { }

	private protected override Func<mpz_t, BigArray<T>> CapacityCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<int, List<T>> CapacityLowCreator => RedStarLinq.EmptyList<T>;

	private protected override Func<IEnumerable<T>, BigArray<T>> CollectionCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<IEnumerable<T>, List<T>> CollectionLowCreator => x => new(x);
}

public class BigBitArray : BigArray<bool, BigBitArray, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private const int BitsPerInt = sizeof(int) * BitsPerByte;
	private const int BytesPerInt = sizeof(int);
	private const int BitsPerByte = 8;

	public BigBitArray() { }

	public BigBitArray(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigBitArray(mpz_t length, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(length, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigBitArray(mpz_t length, bool defaultValue, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (length <= CapacityFirstStep)
		{
			low = new((int)length, defaultValue);
			high = null;
			isHigh = false;

		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << (GetArrayLength((length - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			high = new BigBitArray[(int)GetArrayLength(length, fragment)];
			for (var i = 0; i < length / fragment; i++)
				high[i] = new(fragment, defaultValue, CapacityStepBitLength, CapacityFirstStepBitLength);
			if (length % fragment != 0)
				high[^1] = new(length % fragment, defaultValue, CapacityStepBitLength, CapacityFirstStepBitLength);
			isHigh = true;
		}
		Size2 = length;
	}

	public BigBitArray(IEnumerable bits, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 5)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BigBitArray bigBitArray)
		{
			if (!bigBitArray.isHigh && bigBitArray.low != null)
			{
				low = new(bigBitArray.low);
				high = null;
				Size2 = bigBitArray.Size;
				fragment = bigBitArray.fragment;
				isHigh = false;
			}
			else if (bigBitArray.high != null)
			{
				BigBitArray array = new(bigBitArray.Size, CapacityStepBitLength, CapacityFirstStepBitLength);
				Copy(bigBitArray, 0, array, 0, bigBitArray.Size);
				low = array.low;
				high = array.high;
				Size2 = array.Size;
				fragment = array.fragment;
				isHigh = array.isHigh;
			}
		}
		else if (bits is BitList bitList)
		{
			if (bitList.Length <= CapacityFirstStep)
			{
				low = new(bitList);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << ((((mpz_t)bitList.Length - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var fragment2 = (int)fragment;
				high = new BigBitArray[GetArrayLength(bitList.Length, fragment2)];
				int index = 0, i = 0;
				for (; index <= bitList.Length - fragment2; index += fragment2)
					high[i++] = new(bitList.GetRange(index, fragment2), CapacityStepBitLength, CapacityFirstStepBitLength);
				if (bitList.Length % fragment2 != 0)
				{
					high[i] = new(bitList.Length - index, CapacityStepBitLength, CapacityFirstStepBitLength);
					high[i].SetRangeInternal(0, CollectionCreator(bitList.GetRange(index)));
				}
				isHigh = true;
			}
			Size2 = bitList.Length;
		}
		else if (bits is BigArray<uint> bigUIntArray)
		{
			var count = bigUIntArray.Length;
			if (count <= GetArrayLength(CapacityFirstStep, BitsPerInt))
			{
				low = new(bigUIntArray);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << (((count - 1).BitLength + ((mpz_t)BitsPerInt - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var uintsFragment = fragment / BitsPerInt;
				high = new BigBitArray[(int)((count + uintsFragment - 1) / uintsFragment)];
				mpz_t index = 0;
				var i = 0;
				for (; index <= count - uintsFragment; index += uintsFragment)
					high[i++] = new(bigUIntArray.GetRange(index, uintsFragment), CapacityStepBitLength, CapacityFirstStepBitLength);
				if (index != count)
					high[i] = new(bigUIntArray.GetRange(index, count - index), CapacityStepBitLength, CapacityFirstStepBitLength);
				isHigh = true;
			}
			Size2 = count * BitsPerInt;
		}
		else if (bits is BigList<uint> bigUIntList)
		{
			var count = bigUIntList.Length;
			if (count <= CapacityFirstStep / BitsPerInt)
			{
				low = new(bigUIntList);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << (((count - 1).BitLength + ((mpz_t)BitsPerInt - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var uintsFragment = fragment / BitsPerInt;
				high = new BigBitArray[(int)((count + uintsFragment - 1) / uintsFragment)];
				mpz_t index = 0;
				var i = 0;
				for (; index <= count - uintsFragment; index += uintsFragment)
					high[i++] = new(bigUIntList.GetRange(index, uintsFragment), CapacityStepBitLength, CapacityFirstStepBitLength);
				if (index != count)
					high[i] = new(bigUIntList.GetRange(index, count - index), CapacityStepBitLength, CapacityFirstStepBitLength);
				isHigh = true;
			}
			Size2 = count * BitsPerInt;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			BigBitArray array = new(new BigArray<uint>(uints), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = array.low;
			high = array.high;
			Size2 = array.Size;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is IEnumerable<int> ints)
		{
			BigBitArray array = new(new BigArray<uint>(ints.Select(x => (uint)x)), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = array.low;
			high = array.high;
			Size2 = array.Size;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			var b = true;
			IEnumerator<byte> en = bytes.GetEnumerator();
			BigArray<uint> values = new(length: GetArrayLength(bytes.Count(), 4));
			var n = 0;
			while (b)
			{
				var i = 0;
				uint value = 0;
				for (; i < BytesPerInt; i++, n++)
				{
					if (!(b = en.MoveNext())) break;
					value |= (uint)en.Current << BitsPerByte * i;
				}
				if (i != 0)
					values[(n - 1) / 4] = value;
			}
			BigBitArray bigBitArray2 = new(values, CapacityStepBitLength, CapacityFirstStepBitLength);
			BigBitArray array = new(n * BitsPerByte, CapacityStepBitLength, CapacityFirstStepBitLength);
			Copy(bigBitArray2, 0, array, 0, n * BitsPerByte);
			low = array.low;
			high = array.high;
			Size2 = n * BitsPerByte;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is BitArray or byte[] or bool[])
		{
			BigBitArray array = new(new BitList(bits), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = array.low;
			high = array.high;
			Size2 = array.Size;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is IEnumerable<bool> bools)
		{
			var en = bools.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
		else
			throw new ArgumentException(null, nameof(bits));
	}

	private protected override Func<mpz_t, BigBitArray> CapacityCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<int, BitList> CapacityLowCreator => x => new(x, false);

	private protected override Func<IEnumerable<bool>, BigBitArray> CollectionCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<IEnumerable<bool>, BitList> CollectionLowCreator => x => new(x);

	public virtual BigBitArray And(BigBitArray value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.And(value.low);
		else if (high != null && value.high != null)
			high = high.Combine(value.high, (x, y) => x.And(y)).ToArray();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual uint GetSmallRange(mpz_t index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > Size)
			throw new ArgumentException(null);
		if (count == 0)
			return 0;
		if (count > BitsPerInt)
			throw new ArgumentException(null, nameof(count));
		if (!isHigh && low != null)
			return low.GetSmallRange((int)index, count);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)(index + count - 1).Divide(fragment, out var remainder2);
			uint result;
			if (quotient == quotient2)
				result = high[quotient].GetSmallRange(remainder, count);
			else
			{
				result = high[quotient].GetSmallRange(remainder, (int)(fragment - remainder));
				result |= high[quotient + 1].GetSmallRange(0, (int)(remainder2 + 1)) << (int)(fragment - remainder);
			}
			return result;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitArray Not()
	{
		if (!isHigh && low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual BigBitArray Or(BigBitArray value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Or(value.low);
		else if (high != null && value.high != null)
			high = high.Combine(value.high, (x, y) => x.Or(y)).ToArray();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual void SetAll(bool value)
	{
		if (!isHigh && low != null)
			low.SetAll(value);
		else if (high != null)
			high.ForEach(x => x.SetAll(value));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigArray<uint> ToUIntBigList()
	{
		if (!isHigh && low != null)
			return new(low.ToUIntList());
		else if (high != null)
			return new(high.SelectMany(x => x.ToUIntBigList()));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitArray Xor(BigBitArray value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Xor(value.low);
		else if (high != null && value.high != null)
			high = high.Combine(value.high, (x, y) => x.Xor(y)).ToArray();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}
}

public class Chain : IReadOnlyCollection<int>
{
	private readonly int start;

	public Chain(int count) : this(0, count) { }

	public Chain(int start, int count)
	{
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		this.start = start;
		Length = count;
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

public class Group<T, TKey> : List<T>
{
	public virtual TKey Key { get; private set; }

	public Group(int capacity, TKey key) : base(capacity) => Key = key;

	public Group(IEnumerable<T> collection, TKey key) : base(collection) => Key = key;
}
