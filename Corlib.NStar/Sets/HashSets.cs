using Corlib.NStar;
using System.Diagnostics;

namespace Corlib.NStar;

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public abstract class HashSetBase<T, TCertain> : SetBase<T, TCertain> where TCertain : HashSetBase<T, TCertain>, new()
{
	private protected struct Entry
	{
		internal int hashCode;
		internal int next;
		internal T item;
	}

	private protected List<int> buckets = default!;
	private protected Entry[] entries = default!;
	internal const int HashPrime = 101;
	internal const int MaxPrimeArrayLength = 0x7FEFFFFD;
	internal static readonly int[] primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};


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
		for (int i = 0; i < count; i++)
			RemoveValue(GetInternal(index + i));
		Changed();
	}
	internal static int ExpandPrime(int oldSize)
	{
		int newSize = 2 * oldSize;
		if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
			return MaxPrimeArrayLength;
		return GetPrime(newSize);
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

	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = entries[index].item;
		if (invoke)
			Changed();
		return item;
	}

	private protected override int IndexOfInternal(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets != null)
		{
			int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (int i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + count)
					return i;
		}
		return -1;
	}

	private protected virtual void Initialize(int capacity, out List<int> buckets, out Entry[] entries)
	{
		int size = GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
	}

	private protected abstract TCertain Insert(T? item, bool add, out int index);

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		TCertain set = CollectionCreator(collection);
		set.ExceptWith(this);
		int count = set._size;
		if (count > 0)
		{
			EnsureCapacity(_size + count);
			if (index < entries.Length - count)
				Copy(this, index, this, index + count, entries.Length - index - count);
			if (this == collection)
				return this as TCertain ?? throw new InvalidOperationException();
			else
				Copy(set, 0, this, index, count);
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

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

	private protected virtual void Resize() => Resize(ExpandPrime(_size), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		int[] newBuckets = new int[newSize];
		Entry[] newEntries = new Entry[newSize];
		Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (int i = 0; i < _size; i++)
			{
				ref Entry t = ref newEntries[i];
				if (t.hashCode != 0)
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
			}
		for (int i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				int bucket = ~newEntries[i].hashCode % newSize;
				ref Entry t = ref newEntries[i];
				t.next = newBuckets[bucket];
				newBuckets[bucket] = ~i;
			}
		buckets = newBuckets;
		entries = newEntries;
	}

	public override bool TryAdd(T item, out int index)
	{
		index = IndexOf(item);
		if (index < 0)
			return false;
		Insert(item, true, out index);
		return true;
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
public abstract class FakeIndAftDelHashSet<T, TCertain> : HashSetBase<T, TCertain> where TCertain : FakeIndAftDelHashSet<T, TCertain>, new()
{
	private protected int freeCount;
	private protected int freeList;

	public FakeIndAftDelHashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public FakeIndAftDelHashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public FakeIndAftDelHashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public FakeIndAftDelHashSet(int capacity, IEqualityComparer<T>? comparer)
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

	public FakeIndAftDelHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public FakeIndAftDelHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetCountEasily(out int count) ? (int)(Sqrt(count) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			TryAdd(item);
	}

	public FakeIndAftDelHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public FakeIndAftDelHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			TryAdd(item);
	}

	public FakeIndAftDelHashSet(params T[] array) : this((IEnumerable<T>)array)
	{
	}

	public FakeIndAftDelHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array)
	{
	}

	public FakeIndAftDelHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray())
	{
	}

	public FakeIndAftDelHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray())
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
			if (Contains(value))
				throw new ArgumentException(null, nameof(value));
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
		for (int i = 0; i < count; i++)
			if (entries[i].hashCode < 0)
				array[arrayIndex++] = entries[index + i + skipped].item;
			else
				count++;
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
		for (int i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				index = i;
				return this as TCertain ?? throw new InvalidOperationException();
			}
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
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain RemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return this as TCertain ?? throw new InvalidOperationException();
		if (entries[index].item == null)
			return this as TCertain ?? throw new InvalidOperationException();
		int hashCode = Comparer.GetHashCode(entries[index].item ?? throw new ArgumentException(null)) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		if (bucket != index)
		{
			ref Entry t = ref entries[bucket];
			t.next = entries[index].next;
		}
		Entry t2 = entries[index];
		t2.hashCode = 0;
		t2.next = freeList;
		t2.item = default!;
		entries[index] = t2;
		freeList = ~index;
		freeCount++;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		int last = 0;
		for (int i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref Entry t = ref entries[last];
					t.next = entries[i].next;
				}
				Entry t2 = entries[i];
				t2.hashCode = 0;
				t2.next = freeList;
				t2.item = default!;
				entries[i] = t2;
				freeList = ~i;
				freeCount++;
				return true;
			}
		return false;
	}

	internal override void SetInternal(int index, T item)
	{
		if (entries[index].item != null)
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
		Entry t2 = entries[index];
		t2.hashCode = ~hashCode;
		t2.next = buckets[targetBucket];
		t2.item = item;
		entries[index] = t2;
		buckets[targetBucket] = ~index;
		Changed();
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly FakeIndAftDelHashSet<T, TCertain> dictionary;
		private int index;

		internal Enumerator(FakeIndAftDelHashSet<T, TCertain> dictionary)
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
public class FakeIndAftDelHashSet<T> : FakeIndAftDelHashSet<T, FakeIndAftDelHashSet<T>>
{
	public FakeIndAftDelHashSet()
	{
	}

	public FakeIndAftDelHashSet(int capacity) : base(capacity)
	{
	}

	public FakeIndAftDelHashSet(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	public FakeIndAftDelHashSet(IEnumerable<T> set) : base(set)
	{
	}

	public FakeIndAftDelHashSet(params T[] array) : base(array)
	{
	}

	public FakeIndAftDelHashSet(ReadOnlySpan<T> span) : base(span)
	{
	}

	public FakeIndAftDelHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public FakeIndAftDelHashSet(IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(set, comparer)
	{
	}

	public FakeIndAftDelHashSet(int capacity, IEnumerable<T> set) : base(capacity, set)
	{
	}

	public FakeIndAftDelHashSet(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public FakeIndAftDelHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public FakeIndAftDelHashSet(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(capacity, set, comparer)
	{
	}

	private protected override Func<int, FakeIndAftDelHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, FakeIndAftDelHashSet<T>> CollectionCreator => x => new(x);
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public class ParallelHashSet<T> : FakeIndAftDelHashSet<T, ParallelHashSet<T>>
{
	private protected readonly object lockObj = new();

	public ParallelHashSet()
	{
	}

	public ParallelHashSet(int capacity) : base(capacity)
	{
	}

	public ParallelHashSet(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	public ParallelHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public ParallelHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public ParallelHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetCountEasily(out int count) ? (int)(Sqrt(count) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (T item in collection)
				TryAdd(item);
	}

	public ParallelHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public ParallelHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (T item in collection)
				TryAdd(item);
	}

	public ParallelHashSet(params T[] array) : this((IEnumerable<T>)array)
	{
	}

	public ParallelHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array)
	{
	}

	public ParallelHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray())
	{
	}

	public ParallelHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray())
	{
	}

	public override T this[Index index, bool invoke = true]
	{
		get => base[index, invoke];
		set
		{
			lock (lockObj)
				base[index, invoke] = value;
		}
	}

	private protected override Func<int, ParallelHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, ParallelHashSet<T>> CollectionCreator => x => new(x);

	private protected override void ClearInternal()
	{
		if (_size > 0)
		{
			Parallel.For(0, buckets.Length, i =>
			{
				buckets[i] = 0;
				entries[i] = new();
			});
			freeList = 0;
			_size = 0;
			freeCount = 0;
		}
	}

	private protected override void ClearInternal(int index, int count)
	{
		Parallel.For(0, count, i => RemoveValue(GetInternal(index + i)));
		Changed();
	}

	public override bool Contains(T? item, int index, int count) => UnsafeContains(item, index, count) || Lock(lockObj, UnsafeContains, item, index, count);

	public override bool Contains(IEnumerable<T> collection, int index, int count) => Lock(lockObj, base.Contains, collection, index, count);

	private protected override void Copy(ListBase<T, ParallelHashSet<T>> source, int sourceIndex, ListBase<T, ParallelHashSet<T>> destination, int destinationIndex, int count) => Lock(lockObj, base.Copy, source, sourceIndex, destination, destinationIndex, count);

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false) => Lock(lockObj, base.EqualsInternal, collection, index, toEnd);

	public override void ExceptWith(IEnumerable<T> other)
	{
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, i => RemoveValue(list[i]));
		else
			foreach (T item in other)
				RemoveValue(item);
	}

	public override ParallelHashSet<T> FixUpFakeIndexes() => Lock(lockObj, base.FixUpFakeIndexes);

	public override int IndexOf(IEnumerable<T> collection, int index, int count, out int otherCount)
	{
		lock (lockObj)
			return base.IndexOf(collection, index, count, out otherCount);
	}

	private protected override int IndexOfInternal(T item, int index, int count)
	{
		int foundIndex = UnsafeIndexOf(item, index, count);
		return foundIndex < 0 ? foundIndex : Lock(lockObj, UnsafeIndexOf, item, index, count);
	}

	private protected override void Initialize(int capacity, out List<int> buckets, out Entry[] entries)
	{
		int size = GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
		freeList = 0;
	}

	public override ParallelHashSet<T> Insert(int index, T item) => Lock(lockObj, base.Insert, index, item);

	public override ParallelHashSet<T> Insert(int index, IEnumerable<T> collection) => Lock(lockObj, base.Insert, index, collection);

	private protected override ParallelHashSet<T> Insert(T? item, bool add, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		for (int i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				index = i;
				return this;
			}
		lock (lockObj)
		{
			for (int i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
				{
					if (add)
						throw new ArgumentException(null);
					index = i;
					return this;
				}
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
					UnsafeResize();
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
			return this;
		}
	}

	public override void IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = new ParallelHashSet<T>(other);
		Parallel.For(0, Length, i =>
		{
			T item = this[i];
			if (!set.Contains(item))
				RemoveValue(item);
		});
	}

	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		bool result = true;
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, (i, pls) =>
			{
				if (!Contains(list[i]))
				{
					result = false;
					pls.Stop();
				}
			});
		else
			foreach (T item in other)
				if (!Contains(item))
					return false;
		return result;
	}

	public override int LastIndexOf(IEnumerable<T> collection, int index, int count, out int otherCount)
	{
		lock (lockObj)
			return base.LastIndexOf(collection, index, count, out otherCount);
	}

	public override bool Overlaps(IEnumerable<T> other)
	{
		bool result = false;
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, (i, pls) =>
			{
				if (Contains(list[i]))
				{
					result = true;
					pls.Stop();
				}
			});
		else
			foreach (T item in other)
				if (Contains(item))
					return true;
		return result;
	}

	public override ParallelHashSet<T> RemoveAt(int index) => Lock(lockObj, UnsafeRemoveAt, index);

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		int last = 0;
		for (int i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
				return Lock(lockObj, UnsafeRemoveValue, item);
		return false;
	}

	private protected override void Resize(int newSize, bool forceNewHashCodes) => Lock(lockObj, UnsafeResize, newSize, forceNewHashCodes);

	public override bool SetEquals(IEnumerable<T> other)
	{
		bool result = true;
		if (other is G.IList<T> list)
		{
			if (Length != list.Count)
				return false;
			Parallel.For(0, list.Count, (i, pls) =>
			{
				if (!Contains(list[i]))
				{
					result = false;
					pls.Stop();
				}
			});
			return result;
		}
		else if (other.TryGetCountEasily(out int count))
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
			ParallelHashSet<T> set = new(other);
			if (Length != set.Length)
				return false;
			Parallel.For(0, set.Length, (i, pls) =>
			{
				if (!Contains(set[i]))
				{
					result = false;
					pls.Stop();
				}
			});
			return result;
		}
	}

	public override bool TryAdd(T item) => !UnsafeContains(item) && Lock(lockObj, UnsafeTryAdd, item);

	public override void UnionWith(IEnumerable<T> other)
	{
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (T item in other)
				TryAdd(item);
	}

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual bool UnsafeContains(T? item) => UnsafeContains(item, 0, _size);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual bool UnsafeContains(T? item, int index) => UnsafeContains(item, index, _size - index);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual bool UnsafeContains(T? item, int index, int count) => item != null && UnsafeIndexOf(item, index, count) >= 0;

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual int UnsafeIndexOf(T item) => UnsafeIndexOf(item, 0, _size);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual int UnsafeIndexOf(T item, int index) => UnsafeIndexOf(item, index, _size - index);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual int UnsafeIndexOf(T item, int index, int count)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets != null)
		{
			int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (int i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + count)
					return i;
		}
		return -1;
	}

	private protected virtual ParallelHashSet<T> UnsafeInsert(T? item, bool add)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		for (int i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				return this;
			}
		int index;
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
				UnsafeResize();
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
		return this;
	}

	private protected virtual ParallelHashSet<T> UnsafeRemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return this;
		if (entries[index].item == null)
			return this;
		int hashCode = Comparer.GetHashCode(entries[index].item ?? throw new ArgumentException(null)) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		if (bucket != index)
		{
			ref Entry t = ref entries[bucket];
			t.next = entries[index].next;
		}
		Entry t2 = entries[index];
		t2.hashCode = 0;
		t2.next = freeList;
		t2.item = default!;
		entries[index] = t2;
		freeList = ~index;
		freeCount++;
		return this;
	}

	private protected virtual bool UnsafeRemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int bucket = hashCode % buckets.Length;
		int last = 0;
		for (int i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref Entry t = ref entries[last];
					t.next = entries[i].next;
				}
				Entry t2 = entries[i];
				t2.hashCode = 0;
				t2.next = freeList;
				t2.item = default!;
				entries[i] = t2;
				freeList = ~i;
				freeCount++;
				return true;
			}
		return false;
	}

	private protected virtual void UnsafeResize() => UnsafeResize(ExpandPrime(_size), false);

	private protected virtual void UnsafeResize(int newSize, bool forceNewHashCodes)
	{
		int[] newBuckets = new int[newSize];
		Entry[] newEntries = new Entry[newSize];
		Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (int i = 0; i < _size; i++)
			{
				ref Entry t = ref newEntries[i];
				if (t.hashCode != 0)
				{
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
				}
			}
		for (int i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				int bucket = ~newEntries[i].hashCode % newSize;
				ref Entry t = ref newEntries[i];
				t.next = newBuckets[bucket];
				newBuckets[bucket] = ~i;
			}
		buckets = newBuckets;
		entries = newEntries;
	}

	private protected virtual bool UnsafeTryAdd(T item)
	{
		if (UnsafeContains(item))
			return false;
		UnsafeInsert(item, true);
		return true;
	}
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
public abstract class SlowDeletionHashSet<T, TCertain> : HashSetBase<T, TCertain> where TCertain : SlowDeletionHashSet<T, TCertain>, new()
{
	public SlowDeletionHashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public SlowDeletionHashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public SlowDeletionHashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public SlowDeletionHashSet(int capacity, IEqualityComparer<T>? comparer)
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

	public SlowDeletionHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public SlowDeletionHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetCountEasily(out int count) ? (int)(Sqrt(count) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			TryAdd(item);
	}

	public SlowDeletionHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public SlowDeletionHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			TryAdd(item);
	}

	public SlowDeletionHashSet(params T[] array) : this((IEnumerable<T>)array)
	{
	}

	public SlowDeletionHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array)
	{
	}

	public SlowDeletionHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray())
	{
	}

	public SlowDeletionHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray())
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
		for (int i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				index = i;
				return this as TCertain ?? throw new InvalidOperationException();
			}
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
		return this as TCertain ?? throw new InvalidOperationException();
	}

	internal override void SetInternal(int index, T item)
	{
		if (entries[index].item != null)
			RemoveAt(index);
		if (item == null)
			return;
		int hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		int targetBucket = hashCode % buckets.Length;
		if (_size == entries.Length)
		{
			Resize();
			targetBucket = hashCode % buckets.Length;
		}
		_size++;
		Entry t2 = entries[index];
		t2.hashCode = ~hashCode;
		t2.next = buckets[targetBucket];
		t2.item = item;
		entries[index] = t2;
		buckets[targetBucket] = ~index;
		Changed();
	}
}

public class SlowDeletionHashSet<T> : SlowDeletionHashSet<T, SlowDeletionHashSet<T>>
{
	public SlowDeletionHashSet()
	{
	}

	public SlowDeletionHashSet(int capacity) : base(capacity)
	{
	}

	public SlowDeletionHashSet(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	public SlowDeletionHashSet(IEnumerable<T> collection) : base(collection)
	{
	}

	public SlowDeletionHashSet(params T[] array) : base(array)
	{
	}

	public SlowDeletionHashSet(ReadOnlySpan<T> span) : base(span)
	{
	}

	public SlowDeletionHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public SlowDeletionHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer)
	{
	}

	public SlowDeletionHashSet(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	public SlowDeletionHashSet(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public SlowDeletionHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public SlowDeletionHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer)
	{
	}

	private protected override Func<int, SlowDeletionHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, SlowDeletionHashSet<T>> CollectionCreator => x => new(x);
}
