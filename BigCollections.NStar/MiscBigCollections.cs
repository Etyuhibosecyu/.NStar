namespace BigCollections.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigQueue<T> : G.IEnumerable<T>, ICloneable
{
	private protected Queue<T>? low;
	private protected Queue<BigQueue<T>>? high;
	private protected MpzT _size;
	private protected MpzT fragment;
	private protected bool isHigh;
	private protected const int SubbranchesBitLength = 16, LeafSizeBitLength = 16;
	private protected const int Subbranches = 1 << SubbranchesBitLength, LeafSize = 1 << LeafSizeBitLength;

	public BigQueue() : this(32) { }

	public BigQueue(MpzT capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity <= LeafSize)
		{
			low = new((int)capacity);
			high = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - LeafSizeBitLength, SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			high = new((int)GetArrayLength(capacity, fragment));
			for (MpzT i = 0; i < capacity / fragment; i++)
				high.Enqueue(new(fragment));
			high.Enqueue(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
	}

	public BigQueue(G.IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : col.TryGetLengthEasily(out var length) ? length : 32)
	{
		using var en = col.GetEnumerator();
		while (en.MoveNext())
			Enqueue(en.Current);
	}

	public virtual MpzT Length => _size;

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
		if (_size == LeafSize && !isHigh && low != null)
		{
			high = new(4);
			high.Enqueue(new(Subbranches) { low = low, _size = low.Length });
			BigQueue<T> tempQueue = new(Subbranches);
			high.Enqueue(tempQueue);
			tempQueue.Enqueue(obj);
			low = null;
			fragment = LeafSize;
			isHigh = true;
		}
		else if (!isHigh && low != null)
			low.Enqueue(obj);
		else if (high != null)
		{
			var index = (int)(_size / fragment);
			if (index == Subbranches)
			{
				var temp = high;
				high = new(4);
				high.Enqueue(new(_size) { high = temp, _size = temp.Length });
				BigQueue<T> tempQueue = new(_size);
				high.Enqueue(tempQueue);
				tempQueue.Enqueue(obj);
				fragment *= Subbranches;
			}
			else if (E.ElementAt(high, index)._size == fragment)
			{
				BigQueue<T> tempQueue = new(Subbranches);
				high.Enqueue(tempQueue);
				tempQueue.Enqueue(obj);
			}
			else
				E.ElementAt(high, index).Enqueue(obj);
		}
		_size++;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<T> G.IEnumerable<T>.GetEnumerator() => GetEnumerator();

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
			var removed = high.Peek().Dequeue();
			if (high.Peek()._size == 0)
				high.Dequeue();
			return removed;
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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
			foreach (var subQueue in high)
				if (subQueue.Contains(obj))
					return true;
			return false;
		}
		else
			return false;
	}

	internal T GetElement(MpzT i)
	{
		if (!isHigh && low != null)
			return E.ElementAt(low, (int)(i % Subbranches));
		else if (high != null)
			return E.ElementAt(high, (int)(i / fragment)).GetElement(i % fragment);
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public virtual T[] ToArray()
	{
		if (!isHigh && low != null)
			return [.. low];
		else
			throw new InvalidOperationException("Слишком большая очередь для преобразования в массив!");
	}

	public virtual void TrimExcess()
	{
		if (_size <= LeafSize)
		{
			low = PeekQueue();
			low.TrimExcess();
		}
		else if (high != null)
		{
			high.TrimExcess();
			E.ElementAt(high, high.Length - 1).TrimExcess();
		}
	}

	public virtual bool TryDequeue(out T value)
	{
		if (_size == 0)
		{
			value = default!;
			return false;
		}
		else
		{
			value = Dequeue();
			return true;
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

	[Serializable]
	public struct Enumerator : G.IEnumerator<T>
	{
		private readonly BigQueue<T> queue;
		private MpzT index;
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

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
[Obsolete("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа. Теперь он удален окончательно."
		+ " Большие списки делают все то же самое и многое другое, и они уже работают.", true)]
public abstract class BigArray<T, TCertain, TLow> where TCertain : BigArray<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected TCertain[]? high;
	private protected MpzT fragment = 1;

	public BigArray() : this(-1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigArray(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigArray(MpzT length, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual MpzT Capacity { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

	public virtual MpzT Length {
		get =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
			+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
		private protected set =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
			+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
	}

	public virtual TCertain Add(T item) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual TCertain AddRange(G.IEnumerable<T> collection) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual bool Contains(T item, MpzT index, MpzT length) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual void Dispose() =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
}

/// <summary>
/// Представляет строго типизированный массив элементов, упорядоченных по индексу.
/// В отличие от стандартного T[], имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций с массивом находятся в разработке, на текущий момент
/// поддерживаются только установка элемента по индексу, копирование диапазона и копирование в обычный массив.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
[Obsolete("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа. Теперь он удален окончательно."
		+ " Большие списки делают все то же самое и многое другое, и они уже работают.", true)]
public class BigArray<T> : BigArray<T, BigArray<T>, List<T>>
{
	public BigArray() { }

	public BigArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(length, subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(collection, subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(MpzT length, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(length, collection, subbranchesBitLength, leafSizeBitLength) { }

	protected virtual Func<MpzT, BigArray<T>> CapacityCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<int, List<T>> CapacityLowCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<G.IEnumerable<T>, BigArray<T>> CollectionCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<G.IEnumerable<T>, List<T>> CollectionLowCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");}

/// <summary>
/// Представляет компактный строго типизированный массив бит (true или false), упорядоченных по индексу.
/// В отличие от стандартного <see cref="BitArray"/>, имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций с массивом находятся в разработке, на текущий момент
/// поддерживаются только установка элемента по индексу, копирование диапазона и копирование в обычный массив.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
[Obsolete("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа. Теперь он удален окончательно."
		+ " Большие списки делают все то же самое и многое другое, и они уже работают.", true)]
public class BigBitArray : BigArray<bool, BigBitArray, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private protected const int BitsPerInt = sizeof(int) * BitsPerByte;
	private protected const int BytesPerInt = sizeof(int);
	private protected const int BitsPerByte = 8;

	public BigBitArray() =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigBitArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(subbranchesBitLength, leafSizeBitLength) { }

	public BigBitArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(length, subbranchesBitLength, leafSizeBitLength) { }

	public BigBitArray(MpzT length, bool defaultValue, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigBitArray(BitArray bitArray, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigBitArray(G.IEnumerable<byte> bytes, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigBitArray(G.IEnumerable<bool> bools, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigBitArray(G.IEnumerable<int> ints, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public BigBitArray(G.IEnumerable<uint> uints, int subbranchesBitLength = -1, int leafSizeBitLength = -1) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<MpzT, BigBitArray> CapacityCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<int, BitList> CapacityLowCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<G.IEnumerable<bool>, BigBitArray> CollectionCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	protected virtual Func<G.IEnumerable<bool>, BitList> CollectionLowCreator =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual BigBitArray And(BigBitArray value) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual uint GetSmallRange(MpzT index, int length) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual BigBitArray Not() =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual BigBitArray Or(BigBitArray value) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual void SetAll(bool value) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual BigArray<uint> ToUIntBigList() =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");

	public virtual BigBitArray Xor(BigBitArray value) =>
		throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");}
