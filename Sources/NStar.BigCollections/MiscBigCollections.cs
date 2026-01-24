namespace NStar.BigCollections;

public interface IBigCollection<T> : G.IEnumerable<T>
{
	bool IsReadOnly { get; }
	MpzT Length { get; }

	void Add(T item);
	void Clear();
	bool Contains(T item);
	void CopyTo(T[] array, int arrayIndex);
	void CopyTo(IBigList<T> list, MpzT listIndex);
	bool RemoveValue(T item);
}

public interface IBigList<T> : IBigCollection<T>
{
	T this[MpzT index] { get; set; }

	MpzT IndexOf(T item);
	void Insert(MpzT index, T item);
	void RemoveAt(MpzT index);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigQueue<T> : G.IEnumerable<T>, ICloneable, IDisposable
{
	private protected Queue<T>? low;
	private protected LimitedBuffer<BigQueue<T>>? high;
	private protected MpzT fragment = 1;

	public BigQueue() : this(32) { }

	public BigQueue(int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength is >= 2 and <= 30)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength is >= 2 and <= 30)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength is >= 2 and <= 30)
			LeafSizeBitLength = subbranchesBitLength;
		low = new();
	}

	public BigQueue(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength is >= 2 and <= 30)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength is >= 2 and <= 30)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength is >= 2 and <= 30)
			LeafSizeBitLength = subbranchesBitLength;
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity <= LeafSize)
		{
			low = new(LeafSize);
		}
		else
		{
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - LeafSizeBitLength,
				SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			high = new((int)GetArrayLength(capacity, fragment));
			for (MpzT i = 0; i < high.Capacity; i++)
				high.Add(new(fragment, SubbranchesBitLength, LeafSizeBitLength));
		}
	}

	public BigQueue(G.IEnumerable<T> col, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this((col == null) ? throw new ArgumentNullException(nameof(col))
		: col.TryGetLengthEasily(out var length) ? length : 32, subbranchesBitLength, leafSizeBitLength)
	{
		using var en = col.GetEnumerator();
		while (en.MoveNext())
			Enqueue(en.Current);
	}

	protected virtual bool IsFull
	{
		get
		{
			var last = this;
			while (last.high != null)
			{
				if (last.high.Length != Subbranches
					|| last.fragment != (last.high[^1].low != null ? LeafSize : last.high[^1].fragment << SubbranchesBitLength))
					return false;
				last = last.high[^1];
			}
			Debug.Assert(last.low != null);
			return last.low.Length == LeafSize;
		}
	}

	protected virtual int LeafSizeBitLength { get; init; } = 16;

	protected virtual int LeafSize => 1 << LeafSizeBitLength;

	public virtual MpzT Length { get; protected set; } = 0;

	protected virtual int SubbranchesBitLength { get; init; } = 16;

	protected virtual int Subbranches => 1 << SubbranchesBitLength;

	public virtual void Clear()
	{
		if (low != null)
			low.Clear();
		else if (high != null)
		{
			foreach (var x in high)
				x.Clear();
			high.RemoveEnd(1);
		}
		else
			throw new InvalidOperationException("Невозможно очистить очередь. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		Length = 0;
	}

	public virtual object Clone()
	{
		BigQueue<T> q = new(SubbranchesBitLength, LeafSizeBitLength) { Length = Length };
		if (low != null)
		{
			q.low = (Queue<T>)low.Clone();
			q.high = null;
		}
		else if (high != null)
		{
			q.low = null;
			q.high = new(high.Convert(x => (BigQueue<T>)x.Clone()));
		}
		else
			throw new InvalidOperationException("Невозможно клонировать очередь. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		q.fragment = fragment;
#if VERIFY
		Verify();
		q.Verify();
#endif
		return q;
	}

	public virtual bool Contains(T? obj)
	{
		if (low != null)
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

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex + Length, array.Length);
		if (array.Rank != 1)
			throw new RankException("Массив должен иметь одно измерение.");
		try
		{
			if (low != null)
				low.CopyTo(array, arrayIndex);
			else if (high != null)
			{
				for (var i = 0; i < high.Length; i++)
				{
					high[i].CopyTo(array, arrayIndex);
					arrayIndex += (int)high[i].Length;
				}
			}
			else
				throw new InvalidOperationException("Слишком большая очередь для копирования в массив!");
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		}
	}

	public virtual T Dequeue()
	{
		if (Length == 0)
			throw new InvalidOperationException("Невозможно удалить элемент из очереди, так как она пуста.");
		Length--;
		if (low != null)
			return low.Dequeue();
		else if (high != null)
		{
			var removed = high[0].Dequeue();
			if (high[0].Length == 0 && high.Length != 1)
			{
				high[0].Dispose();
				high.RemoveAt(0);
			}
#if VERIFY
			Verify();
#endif
			return removed;
		}
		else
			throw new InvalidOperationException("Невозможно удалить элемент из очереди. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	public virtual void Dispose()
	{
		low?.Dispose();
		low = null;
		if (high != null)
		{
			foreach (var x in high)
				x.Dispose();
			high.Dispose();
		}
		high = null;
		fragment = 1;
		Length = 0;
		GC.SuppressFinalize(this);
	}

	public virtual void Enqueue(T item)
	{
		if (Length == LeafSize && low != null)
		{
			high = new(4)
			{
				new(LeafSize, SubbranchesBitLength, LeafSizeBitLength) { low = low, Length = low.Length }
			};
			BigQueue<T> tempQueue = new(LeafSize, SubbranchesBitLength, LeafSizeBitLength);
			high.Add(tempQueue);
			tempQueue.Enqueue(item);
			low = null;
			fragment = LeafSize;
		}
		else if (low != null)
			low.Enqueue(item);
		else if (high != null)
		{
			var index = high.Length == 1 || high[1].Length == 0 || Length < high[0].Length
				? 0 : (int)((Length - high[0].Length) / fragment) + 1;
			if (index < high.Length
				&& (index == high.Length - 1
				? fragment == (high[^1].low != null ? LeafSize : high[^1].fragment << SubbranchesBitLength)
				&& high[index].IsFull : high[index].Length == fragment))
				index++;
			if (index == Subbranches)
			{
				var temp = high;
				high = new(4);
				high.Add(new(SubbranchesBitLength, LeafSizeBitLength)
				{
					low = null,
					high = temp,
					fragment = fragment,
					Length = Length
				});
				fragment <<= SubbranchesBitLength;
				BigQueue<T> tempQueue = new(fragment, SubbranchesBitLength, LeafSizeBitLength);
				high.Add(tempQueue);
				tempQueue.Enqueue(item);
			}
			else if (index == high.Length)
			{
				if (high.IsFull)
					high.Capacity = Min(high.Capacity << 1, Subbranches);
				BigQueue<T> tempQueue = new(fragment, SubbranchesBitLength, LeafSizeBitLength);
				high.Add(tempQueue);
				tempQueue.Enqueue(item);
			}
			else
				high[index].Enqueue(item);
		}
		else
			throw new InvalidOperationException("Невозможно добавить элемент в очередь. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		Length += 1;
#if VERIFY
		Verify();
#endif
	}

	internal T GetElement(MpzT i)
	{
		if (low != null)
			return E.ElementAt(low, (int)(i % LeafSize));
		else if (high != null)
		{
			if (high[0].Length == 0 || i < high[0].Length)
				return high[0].GetElement(i);
			var indexExceptFirst = i - high[0].Length;
			return high[(int)(indexExceptFirst / fragment) + 1].GetElement(indexExceptFirst % fragment);
		}
		else
			throw new InvalidOperationException("Невозможно получить элемент очереди. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<T> G.IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual T Peek()
	{
		if (Length == 0)
			throw new InvalidOperationException("Невозможно получить ближайший элемент в очереди, так как она пуста.");
		if (low != null)
			return low.Peek();
		else if (high != null)
			return high[0].Peek();
		else
			throw new InvalidOperationException("Невозможно получить ближайший элемент в очереди. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	public virtual T[] ToArray()
	{
		if (low != null)
			return [.. low];
		else
			throw new InvalidOperationException("Слишком большая очередь для преобразования в массив!");
	}

	public virtual void TrimExcess()
	{
		if (Length <= LeafSize)
		{
			if (low != null)
				return;
			var first = this;
			while (high != null)
			{
				var index = high.Length == 1 || high[1].Length == 0 || Length < high[0].Length
					? 1 : (int)((Length - high[0].Length) / fragment) + 1;
				if (index < high.Length && high[index].Length != 0)
					index++;
				for (var i = index; i < high.Length; i++)
					high[i].Dispose();
				if (index > 1)
				{
					high.RemoveEnd(index);
#if VERIFY
					Verify();
#endif
					return;
				}
				fragment = high[0].fragment;
				var oldHigh = high;
				low = high[0].low;
				high = high[0].high;
				oldHigh.Clear();
				oldHigh.Dispose();
			}
			Debug.Assert(low != null);
			high = null;
			fragment = 1;
		}
		else if (high != null)
		{
			int index;
			while (Length <= fragment)
			{
				Debug.Assert(high != null);
				index = high.Length == 1 || high[1].Length == 0 || Length < high[0].Length
					? 1 : (int)((Length - high[0].Length) / fragment) + 1;
				if (index < high.Length && high[index].Length != 0)
					index++;
				for (var i = index; i < high.Length; i++)
					high[i].Dispose();
				if (index > 1)
				{
#if VERIFY
					Verify();
#endif
					return;
				}
				var oldHigh = high;
				high = high[0].high;
				oldHigh.RemoveAt(0);
				oldHigh.Dispose();
				fragment >>= SubbranchesBitLength;
			}
			Debug.Assert(high != null);
			index = high.Length == 1 || high[1].Length == 0 || Length < high[0].Length
				? 1 : (int)((Length - high[0].Length) / fragment) + 1;
			if (index < high.Length && high[index].Length != 0)
				index++;
			high.RemoveEnd(index);
			high.TrimExcess();
			high[^1].TrimExcess();
		}
		else
			throw new InvalidOperationException("Невозможно освободить избыток памяти в очереди. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
#if VERIFY
		Verify();
#endif
	}

	public virtual bool TryDequeue(out T value)
	{
		if (Length == 0)
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
		if (Length == 0)
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
#if VERIFY

	protected virtual void Verify() => Verify(this);

	protected virtual void Verify(BigQueue<T> item)
	{
		item.VerifySingle();
		if (item.high == null)
			return;
		for (var i = 0; i < item.high.Length; i++)
		{
			var x = item.high[i];
			Verify(x);
		}
	}

	protected virtual void VerifySingle()
	{
		Debug.Assert(low != null ^ high != null);
		if (low != null)
		{
			Debug.Assert(Length == low.Length);
			Debug.Assert(Length <= LeafSize);
		}
		else if (high != null)
		{
			Debug.Assert(high.Length != 0);
			Debug.Assert(Length == 0 || high[0].Length != 0);
			Debug.Assert(Length == high.Sum(x => x.Length));
			Debug.Assert(Length <= fragment << SubbranchesBitLength);
			for (var i = 0; i < high.Length - 1; i++)
				Debug.Assert(fragment == (high[i].low != null ? LeafSize : high[i].fragment << SubbranchesBitLength));
			Debug.Assert(high.Capacity <= Subbranches);
		}
		else
			throw new InvalidOperationException("Обнаружен недействительный список. Возможные причины:\r\n"
				+ "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
				+ "2. Нарушение целостности структуры очереди (ошибка в логике -"
				+ " очередь все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
				+ "3. Системная ошибка (память, диск и т. д.).\r\n"
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length?? 0},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}
#endif

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
				if (index == 0 || index == queue.Length + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
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
			if (index < queue.Length)
			{
				current = queue.GetElement(index)!;
				index++;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = queue.Length + 1;
			current = default!;
			return false;
		}

		readonly object IEnumerator.Current => Current!;

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
public abstract class BigArray<T, TCertain, TLow>
	where TCertain : BigArray<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
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

	public virtual MpzT Capacity
	{
		get =>
			throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
		set => throw new NotSupportedException("Этот класс никогда не был корректно работающим, хотя бы на уровне прототипа."
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
	}

	public virtual MpzT Length
	{
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

	public BigArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(length, subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(collection, subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(MpzT length, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(length, collection, subbranchesBitLength, leafSizeBitLength) { }

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
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
}

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

	public BigBitArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(subbranchesBitLength, leafSizeBitLength) { }

	public BigBitArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: base(length, subbranchesBitLength, leafSizeBitLength) { }

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
		+ " Теперь он удален окончательно. Большие списки делают все то же самое и многое другое, и они уже работают.");
}
