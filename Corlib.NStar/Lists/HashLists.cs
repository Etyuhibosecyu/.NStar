using System.Diagnostics;

namespace Corlib.NStar;


[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public abstract class BaseHashList<T, TCertain> : BaseList<T, TCertain> where TCertain : BaseHashList<T, TCertain>, new()
{
	private protected struct Entry
	{
		internal int hashCode;
		internal int next;
		internal T item;
	}

	private protected int[] buckets = default!;
	private protected Entry[] entries = default!;
	private protected readonly FastDelHashSet<T> uniqueElements = new();
	internal const int HashPrime = 101;
	internal const int MaxPrimeArrayLength = 0x7FEFFFFD;
	internal const int HashSearchMultiplier = 32, AnyHashIndexThreshold = HashSearchMultiplier << 1;
	internal static readonly int[] primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

	public BaseHashList() : this(0, (IEqualityComparer<T>?)null) { }

	public BaseHashList(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public BaseHashList(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public BaseHashList(int capacity, IEqualityComparer<T>? comparer)
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

	public BaseHashList(IEnumerable<T> collection) : this(collection, null) { }

	public BaseHashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection.TryGetCountEasily(out int count) ? count : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			Add(item);
	}

	public BaseHashList(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public BaseHashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			Add(item);
	}

	public BaseHashList(params T[] array) : this((IEnumerable<T>)array)
	{
	}

	public BaseHashList(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array)
	{
	}

	public BaseHashList(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray())
	{
	}

	public BaseHashList(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray())
	{
	}

	public override int Capacity
	{
		get => buckets.Length;
		set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			Resize(value, false);
			Changed();
		}
	}

	public virtual IEqualityComparer<T> Comparer { get; private protected set; } = EqualityComparer<T>.Default;

	public override TCertain Add(T item) => Add(item, out _);

	public virtual TCertain Add(T item, out int index) => Insert(item, false, out index);

	public override Span<T> AsSpan(int index, int count) => throw new NotSupportedException();

	private protected override void ClearInternal()
	{
		if (_size > 0)
		{
			for (int i = 0; i < buckets.Length; i++)
			{
				buckets[i] = 0;
				entries[i] = new();
			}
			_size = 0;
		}
	}

	private protected override void ClearInternal(int index, int count)
	{
		int endIndex = index + count;
		for (int i = index; i < endIndex; i++)
			SetNull(i);
		Changed();
	}

	public override bool Contains(T? item, int index, int count) => item != null && IndexOf(item, index, count) >= 0;

	private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int count)
	{
		if (source != destination || sourceIndex >= destinationIndex)
			for (int i = 0; i < count; i++)
				destination.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		else
			for (int i = count - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		destination.Changed();
	}

	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal static int ExpandPrime(int oldSize)
	{
		int newSize = 2 * oldSize;
		if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
			return MaxPrimeArrayLength;
		return GetPrime(newSize);
	}

	public virtual int FirstHashIndexOf(T item) => FirstHashIndexOf(item, _size - 1, _size);

	public virtual int FirstHashIndexOf(T item, int index) => FirstHashIndexOf(item, index, index + 1);

	public virtual int FirstHashIndexOf(T item, int index, int count) => HashIndexesOf(item, index, count).Min();

	public virtual int FirstIndexOf(T item) => FirstIndexOf(item, _size - 1, _size);

	public virtual int FirstIndexOf(T item, int index) => FirstIndexOf(item, index, index + 1);

	public virtual int FirstIndexOf(T item, int index, int count) => IsHashSearchBetter() ? FirstHashIndexOf(item, index, count) : FirstLinearIndexOf(item, index, count);

	public virtual int FirstLinearIndexOf(T item) => FirstLinearIndexOf(item, _size - 1, _size);

	public virtual int FirstLinearIndexOf(T item, int index) => FirstLinearIndexOf(item, index, index + 1);

	public virtual int FirstLinearIndexOf(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		int endIndex = index + count;
		for (int i = index; i < endIndex; i++)
			if (Comparer.Equals(entries[i].item, item))
				return i;
		return -1;
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = entries[index].item;
		if (invoke)
			Changed();
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

	public virtual List<int> HashIndexesOf(T item) => HashIndexesOf(item, _size - 1, _size);

	public virtual List<int> HashIndexesOf(T item, int index) => HashIndexesOf(item, index, index + 1);

	public virtual List<int> HashIndexesOf(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		List<int> result = new();
		if (buckets != null)
		{
			uint collisionCount = 0;
			int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (int i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
			{
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + count)
					result.Add(i);
				collisionCount++;
				if (collisionCount > entries.Length)
					throw new InvalidOperationException();
			}
		}
		return result;
	}

	public virtual int HashIndexOf(T item) => HashIndexOf(item, _size - 1, _size);

	public virtual int HashIndexOf(T item, int index) => HashIndexOf(item, index, index + 1);

	public virtual int HashIndexOf(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (buckets != null)
		{
			uint collisionCount = 0;
			int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (int i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
			{
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + count)
					return i;
				collisionCount++;
				if (collisionCount > entries.Length)
					throw new InvalidOperationException();
			}
		}
		return -1;
	}

	public virtual List<int> IndexesOf(T item) => IndexesOf(item, 0, _size);

	public virtual List<int> IndexesOf(T item, int index) => IndexesOf(item, index, _size - index);

	public virtual List<int> IndexesOf(T item, int index, int count) => IsHashSearchBetter() ? HashIndexesOf(item, index, count) : LinearIndexesOf(item, index, count);

	private protected override int IndexOfInternal(T item, int index, int count) => uniqueElements.Length >= AnyHashIndexThreshold ? HashIndexOf(item, index, count) : LinearIndexOf(item, index, count);

	private protected virtual void Initialize(int capacity, out int[] buckets, out Entry[] entries)
	{
		int size = GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
	}

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity) EnsureCapacity(_size + 1);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (index < _size)
			Copy(this2, index, this2, index + 1, _size - index);
		entries[index].item = item;
		uniqueElements.TryAdd(item);
		return this2;
	}

	private protected abstract TCertain Insert(T? item, bool add, out int index);

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		TCertain list = CollectionCreator(collection);
		int count = list._size;
		if (count > 0)
		{
			EnsureCapacity(_size + count);
			if (index < entries.Length - count)
				Copy(this2, index, this2, index + count, entries.Length - index - count);
			Copy(list, 0, this2, index, count);
		}
		return this2;
	}

	private protected virtual bool IsHashSearchBetter() => (long)_size * HashSearchMultiplier < (long)uniqueElements.Length * uniqueElements.Length;

	protected static bool IsPrime(int candidate)
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

	public virtual int LastHashIndexOf(T item) => LastHashIndexOf(item, _size - 1, _size);

	public virtual int LastHashIndexOf(T item, int index) => LastHashIndexOf(item, index, index + 1);

	public virtual int LastHashIndexOf(T item, int index, int count) => HashIndexesOf(item, index, count).Max();

	private protected override int LastIndexOfInternal(T item, int index, int count) => IsHashSearchBetter() ? LastHashIndexOf(item, index, count) : LastLinearIndexOf(item, index, count);

	public virtual int LastLinearIndexOf(T item) => LastLinearIndexOf(item, _size - 1, _size);

	public virtual int LastLinearIndexOf(T item, int index) => LastLinearIndexOf(item, index, index + 1);

	public virtual int LastLinearIndexOf(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		int endIndex = index + count;
		for (int i = endIndex - 1; i >= index; i--)
			if (Comparer.Equals(entries[i].item, item))
				return i;
		return -1;
	}

	public virtual List<int> LinearIndexesOf(T item) => LinearIndexesOf(item, _size - 1, _size);

	public virtual List<int> LinearIndexesOf(T item, int index) => LinearIndexesOf(item, index, index + 1);

	public virtual List<int> LinearIndexesOf(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		List<int> result = new();
		int endIndex = index + count;
		for (int i = index; i < endIndex; i++)
			if (Comparer.Equals(entries[i].item, item))
				result.Add(i);
		return result;
	}

	public virtual int LinearIndexOf(T item) => LinearIndexOf(item, _size - 1, _size);

	public virtual int LinearIndexOf(T item, int index) => LinearIndexOf(item, index, index + 1);

	public virtual int LinearIndexOf(T item, int index, int count) => FirstLinearIndexOf(item, index, count);

	private protected virtual void Resize() => Resize(ExpandPrime(_size), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		Entry[] newEntries = new Entry[newSize];
		Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (int i = 0; i < _size; i++)
			{
				ref Entry t = ref newEntries[i];
				if (t.hashCode != 0)
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
			}
		buckets = new int[newSize];
		for (int i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				int bucket = ~newEntries[i].hashCode % newSize;
				ref Entry t = ref newEntries[i];
				t.next = buckets[bucket];
				buckets[bucket] = ~i;
			}
		entries = newEntries;
	}

	private protected override TCertain ReverseInternal(int index, int count) => throw new NotImplementedException();

	private protected virtual void SetNull(int index)
	{
		ref Entry t = ref entries[index];
		if (t.hashCode >= 0)
			return;
		int bucket = ~t.hashCode % buckets.Length;
		t.hashCode = 0;
		t.next = 0;
		t.item = default!;
		if (buckets[bucket] == ~index)
			buckets[bucket] = entries[index].next;
	}
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
/// <summary>
/// Внимание! Рекомендуется не использовать в этом хэш-множестве одновременно удаление и индексацию, так как
/// после удаления даже одного элемента обращение по индексу может привести к недействительному элементу
/// (к тому самому удаленному, так как удаление не смещает следующие элементы влево - иначе бы оно было
/// намного медленнее, а главное преимущество именно этого хэш-множества - удаление за Õ(1) ("O(1)
/// в большинстве случаев")), либо же действительный "номер" элемента может существенно отличаться от
/// указанного вами индекса. КРАЙНЕ не рекомендуется применять данный тип в качестве списка где попало,
/// особенно в качестве релизации стандартного интерфейса IList<T> из .NET, так как такая реализация
/// может повести себя непредсказуемым способом. В случае, если вы уже завершили серию удалений и хотите
/// снова перейти к обращениям по индексу, используйте метод FixUpFakeIndexes() для "починки" индексации.
/// </summary>
public abstract class FastDelHashList<T, TCertain> : BaseHashList<T, TCertain> where TCertain : FastDelHashList<T, TCertain>, new()
{
	private protected int freeCount;
	private protected int freeList;

	protected FastDelHashList()
	{
	}

	protected FastDelHashList(int capacity) : base(capacity)
	{
	}

	protected FastDelHashList(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	protected FastDelHashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	protected FastDelHashList(IEnumerable<T> collection) : base(collection)
	{
	}

	protected FastDelHashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer)
	{
	}

	protected FastDelHashList(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	protected FastDelHashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer)
	{
	}

	protected FastDelHashList(params T[] array) : base(array)
	{
	}

	protected FastDelHashList(int capacity, params T[] array) : base(capacity, array)
	{
	}

	protected FastDelHashList(ReadOnlySpan<T> span) : base(span)
	{
	}

	protected FastDelHashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public override T this[Index index, bool invoke = true]
	{
		get
		{
			if (freeCount != 0)
				try
				{
					throw new FakeIndexesException();
				}
				catch
				{
				}
			int index2 = index.IsFromEnd ? entries.Length - index.Value : index.Value;
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set
		{
			if (freeCount != 0)
				try
				{
					throw new FakeIndexesException();
				}
				catch
				{
				}
			int index2 = index.IsFromEnd ? entries.Length - index.Value : index.Value;
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			if (entries[index2].item?.Equals(value) ?? false)
				return;
			SetInternal(index2, value);
		}
	}

	public override int Length => _size - freeCount;

	public virtual int Size => _size;

	private protected override void ClearInternal()
	{
		base.ClearInternal();
		freeCount = 0;
		freeList = 0;
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count)
	{
		int skipped = 0;
		for (int i = 0; i < index; i++)
			if (entries[i].hashCode >= 0)
				skipped++;
		int endIndex = index + count;
		for (int i = index; i < endIndex; i++)
			if (entries[i + skipped].hashCode < 0)
				array[arrayIndex++] = entries[i + skipped].item;
			else
				endIndex++;
	}

	public override void Dispose()
	{
		freeCount = 0;
		freeList = 0;
		base.Dispose();
		GC.SuppressFinalize(this);
	}

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false)
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
			if (index > _size - list.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			for (int i = 0; i < list.Count; i++)
			{
				while (entries[index].hashCode >= 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(list[i]) ?? list[i] is null))
					return false;
			}
			return !toEnd || index == _size;
		}
		else
		{
			if (collection.TryGetCountEasily(out int count) && index > _size - count)
				throw new ArgumentOutOfRangeException(nameof(index));
			foreach (T item in collection)
			{
				while (entries[index].hashCode >= 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item is null))
					return false;
			}
			return !toEnd || index == _size;
		}
	}

	public override TCertain FilterInPlace(Func<T, bool> match)
	{
		foreach (T item in this as TCertain ?? throw new InvalidOperationException())
			if (!match(item))
				RemoveValue(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain FilterInPlace(Func<T, int, bool> match)
	{
		int i = 0;
		foreach (T item in this as TCertain ?? throw new InvalidOperationException())
			if (!match(item, i++))
				RemoveValue(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain FixUpFakeIndexes()
	{
		int newSize = GetPrime(_size - freeCount);
		int[] newBuckets = new int[newSize];
		Entry[] newEntries = new Entry[newSize];
		int skipped = 0;
		for (int i = 0; i < entries.Length; i++)
			if (entries[i].hashCode < 0)
				newEntries[i - skipped] = entries[i];
			else
				skipped++;
		for (int i = 0; i < newSize; i++)
			if (newEntries[i].hashCode < 0)
			{
				int bucket = ~newEntries[i].hashCode % newSize;
				ref Entry t = ref newEntries[i];
				t.next = newBuckets[bucket];
				newBuckets[bucket] = ~i;
			}
		_size = newSize;
		buckets = newBuckets;
		entries = newEntries;
		freeCount = 0;
		freeList = 0;
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	private protected virtual Enumerator GetEnumeratorInternal() => new(this);

	private protected override TCertain Insert(T? item, bool add, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		if (freeCount > 0)
		{
			index = ~freeList;
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
		ref Entry t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		uniqueElements.TryAdd(item);
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain RemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return this as TCertain ?? throw new InvalidOperationException();
		T? item = entries[index].item;
		if (item == null)
			return this as TCertain ?? throw new InvalidOperationException();
		int hashCode = base.Comparer.GetHashCode(item ?? throw new ArgumentException(null)) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		if (bucket != index)
		{
			ref Entry t = ref entries[bucket];
			t.next = entries[index].next;
		}
		ref Entry t2 = ref entries[index];
		t2.hashCode = 0;
		t2.next = freeList;
		t2.item = default!;
		freeList = ~index;
		freeCount++;
		if (!Contains(item))
			uniqueElements.RemoveValue(item);
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		uint collisionCount = 0;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		int last = -1;
		for (int i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref Entry t = ref entries[last];
					t.next = entries[i].next;
				}
				ref Entry t2 = ref entries[i];
				t2.hashCode = 0;
				t2.next = freeList;
				t2.item = default!;
				freeList = ~i;
				freeCount++;
				if (!Contains(item))
					uniqueElements.RemoveValue(item);
				Changed();
				return true;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		return false;
	}

	internal override void SetInternal(int index, T item)
	{
		if (entries[index].hashCode < 0)
			RemoveAt(index);
		if (item == null)
			return;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		if (freeCount > 0 && freeList == ~index)
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
		ref Entry t2 = ref entries[index];
		T? oldItem = t2.item;
		t2.hashCode = ~hashCode;
		t2.next = buckets[targetBucket];
		t2.item = item;
		buckets[targetBucket] = ~index;
		if (!Contains(item))
			uniqueElements.RemoveValue(oldItem);
		uniqueElements.TryAdd(item);
		Changed();
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly FastDelHashList<T, TCertain> dictionary;
		private int index;

		internal Enumerator(FastDelHashList<T, TCertain> dictionary)
		{
			this.dictionary = dictionary;
			index = 0;
			Current = default!;
		}

		public bool MoveNext()
		{
			while ((uint)index < (uint)dictionary.entries.Length)
			{
				if (dictionary.entries[index].hashCode < 0)
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

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
/// <summary>
/// Внимание! Рекомендуется не использовать в этом хэш-множестве одновременно удаление и индексацию, так как
/// после удаления даже одного элемента обращение по индексу может привести к недействительному элементу
/// (к тому самому удаленному, так как удаление не смещает следующие элементы влево - иначе бы оно было
/// намного медленнее, а главное преимущество именно этого хэш-множества - удаление за Õ(1) ("O(1)
/// в большинстве случаев")), либо же действительный "номер" элемента может существенно отличаться от
/// указанного вами индекса. КРАЙНЕ не рекомендуется применять данный тип в качестве списка где попало,
/// особенно в качестве релизации стандартного интерфейса IList<T> из .NET, так как такая реализация
/// может повести себя непредсказуемым способом.
/// </summary>
public class FastDelHashList<T> : FastDelHashList<T, FastDelHashList<T>>
{
	public FastDelHashList() : base()
	{
	}

	public FastDelHashList(int capacity) : base(capacity)
	{
	}

	public FastDelHashList(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	public FastDelHashList(IEnumerable<T> set) : base(set)
	{
	}

	public FastDelHashList(params T[] array) : base(array)
	{
	}

	public FastDelHashList(ReadOnlySpan<T> span) : base(span)
	{
	}

	public FastDelHashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public FastDelHashList(IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(set, comparer)
	{
	}

	public FastDelHashList(int capacity, IEnumerable<T> set) : base(capacity, set)
	{
	}

	public FastDelHashList(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public FastDelHashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public FastDelHashList(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(capacity, set, comparer)
	{
	}

	private protected override Func<int, FastDelHashList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, FastDelHashList<T>> CollectionCreator => x => new(x);
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
/// <summary>
/// Внимание! Рекомендуется не использовать в этом хэш-множестве удаление в цикле, так как такое действие
/// имеет асимптотику O(n²), и при большом размере хэш-множества программа может зависнуть. Дело в том,
/// что здесь, в отличие от класса FakeIndAftDelHashSet<T>, индексация гарантированно "правильная", но за это
/// приходится платить тем, что после каждого удаления нужно сдвинуть все следующие элементы влево, а это
/// имеет сложность по времени O(n), соответственно, цикл таких действий - O(n²). Если вам нужно произвести
/// серию удалений, используйте FakeIndAftDelHashSet<T>, а по завершению серии вызовите FixUpFakeIndexes().
/// </summary>
public abstract class HashList<T, TCertain> : BaseHashList<T, TCertain> where TCertain : HashList<T, TCertain>, new()
{
	protected HashList()
	{
	}

	protected HashList(int capacity) : base(capacity)
	{
	}

	protected HashList(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	protected HashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	protected HashList(IEnumerable<T> collection) : base(collection)
	{
	}

	protected HashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer)
	{
	}

	protected HashList(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	protected HashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer)
	{
	}

	protected HashList(params T[] array) : base(array)
	{
	}

	protected HashList(int capacity, params T[] array) : base(capacity, array)
	{
	}

	protected HashList(ReadOnlySpan<T> span) : base(span)
	{
	}

	protected HashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count)
	{
		for (int i = 0; i < count; i++)
			array[arrayIndex++] = entries[index++].item;
	}

	private protected override TCertain Insert(T? item, bool add, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		if (_size == entries.Length)
		{
			Resize();
			targetBucket = hashCode % buckets.Length;
		}
		index = _size;
		_size++;
		ref Entry t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		uniqueElements.TryAdd(item);
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		T? item = entries[index].item;
		_size--;
		if (index < _size)
			Copy(this2, index + 1, this2, index, _size - index);
		SetNull(_size);
		if (!Contains(item))
			uniqueElements.RemoveValue(item);
		Changed();
		return this2;
	}

	internal override void SetInternal(int index, T item)
	{
		int hashCode = item == null ? 0 : Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		ref Entry t = ref entries[index];
		uint collisionCount = 0;
		int oldBucket = ~t.hashCode % buckets.Length;
		int last = -1;
		for (int i = ~buckets[oldBucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (i == index)
			{
				if (last < 0)
					buckets[oldBucket] = entries[i].next;
				else
				{
					ref Entry t2 = ref entries[last];
					t2.next = entries[i].next;
				}
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		T? oldItem = t.item;
		t.hashCode = ~hashCode;
		t.next = buckets[bucket];
		t.item = item;
		buckets[bucket] = ~index;
		if (!Contains(item))
			uniqueElements.RemoveValue(oldItem);
		uniqueElements.TryAdd(item);
		Changed();
	}
}

public class HashList<T> : HashList<T, HashList<T>>
{
	public HashList() : base()
	{
	}

	public HashList(int capacity) : base(capacity)
	{
	}

	public HashList(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	public HashList(IEnumerable<T> collection) : base(collection)
	{
	}

	public HashList(params T[] array) : base(array)
	{
	}

	public HashList(ReadOnlySpan<T> span) : base(span)
	{
	}

	public HashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public HashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer)
	{
	}

	public HashList(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	public HashList(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public HashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public HashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer)
	{
	}

	private protected override Func<int, HashList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, HashList<T>> CollectionCreator => x => new(x);
}
