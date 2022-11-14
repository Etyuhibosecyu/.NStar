namespace Corlib.NStar;

public abstract class SetBase<T, TCertain> : ListBase<T, TCertain>, ISet<T>, ICollection where TCertain : SetBase<T, TCertain>, new()
{
	public virtual IEqualityComparer<T> Comparer { get; protected set; } = EqualityComparer<T>.Default;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	bool ISet<T>.Add(T item) => TryAdd(item);

	[DllExport("AsSpan", CallingConvention.Cdecl)]
	public override Span<T> AsSpan(int index, int count) => throw new NotSupportedException();

	[DllExport("Contains", CallingConvention.Cdecl)]
	public override bool Contains(T? item) => item != null && IndexOf(item) >= 0;

	[DllExport("ExceptWith", CallingConvention.Cdecl)]
	public virtual void ExceptWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			RemoveValue(item);
	}

	[DllExport("Insert", CallingConvention.Cdecl)]
	public override TCertain Insert(int index, T item)
	{
		if (!Contains(item))
		{
			base.Insert(index, item);
			_size--;
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	[DllExport("IntersectWith", CallingConvention.Cdecl)]
	public virtual void IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = CollectionCreator(other);
		foreach (T item in this)
			if (!set.Contains(item))
				RemoveValue(item);
	}

	[DllExport("IsProperSubsetOf", CallingConvention.Cdecl)]
	public virtual bool IsProperSubsetOf(IEnumerable<T> other) => !SetEquals(other is ISet<T> set ? set : set = CollectionCreator(other)) && IsSubsetOf(set);

	[DllExport("IsProperSupersetOf", CallingConvention.Cdecl)]
	public virtual bool IsProperSupersetOf(IEnumerable<T> other) => !SetEquals(other) && IsSupersetOf(other);

	[DllExport("IsSubsetOf", CallingConvention.Cdecl)]
	public virtual bool IsSubsetOf(IEnumerable<T> other) => other is ISet<T> set ? set.IsSupersetOf(this) : IsSubsetOf(CollectionCreator(other));

	[DllExport("IsSupersetOf", CallingConvention.Cdecl)]
	public virtual bool IsSupersetOf(IEnumerable<T> other)
	{
		foreach (T item in other)
			if (!Contains(item))
				return false;
		return true;
	}

	private protected override int LastIndexOfInternal(T item, int index, int count) => throw new NotSupportedException();

	[DllExport("Overlaps", CallingConvention.Cdecl)]
	public virtual bool Overlaps(IEnumerable<T> other)
	{
		foreach (T item in other)
			if (Contains(item))
				return true;
		return false;
	}

	private protected override TCertain ReverseInternal(int index, int count) => throw new NotSupportedException();

	[DllExport("SetEquals", CallingConvention.Cdecl)]
	public virtual bool SetEquals(IEnumerable<T> other)
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
			TCertain set = CollectionCreator(other);
			if (Length != set.Length)
				return false;
			foreach (T item in set)
				if (!Contains(item))
					return false;
			return true;
		}
	}

	[DllExport("SymmetricExceptWith", CallingConvention.Cdecl)]
	public virtual void SymmetricExceptWith(IEnumerable<T> other)
	{
		TCertain added = new(), removed = new();
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

	[DllExport("TryAdd", CallingConvention.Cdecl)]
	public abstract bool TryAdd(T item);

	[DllExport("UnionWith", CallingConvention.Cdecl)]
	public virtual void UnionWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			TryAdd(item);
	}
}

public class HashSet<T> : SetBase<T, HashSet<T>>
{
	private protected struct Entry
	{
		public int hashCode = -1;
		public int next = -1;
		public T item = default!;

		public Entry()
		{
		}
	}

	private protected List<int> buckets;
	private protected List<Entry> entries;
	private protected int freeList;
	private protected int freeCount;

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

	public override int Capacity
	{
		get => buckets.Length;
		set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			Resize(value, false);
			ListChanged?.Invoke(this);
		}
	}

	private protected override Func<int, HashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, HashSet<T>> CollectionCreator => x => new(x);

	public virtual int FullLength => _size;

	public override int Length => _size - freeCount;

	public event ListChangedHandler? ListChanged;

	[DllExport("Add", CallingConvention.Cdecl)]
	public override HashSet<T> Add(T item)
	{
		if (!Contains(item))
			return Insert(item, true);
		return this;
	}

	private protected override void ClearInternal()
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

	private protected override void ClearInternal(int index, int count)
	{
		for (int i = 0; i < count; i++)
			RemoveValue(GetInternal(index + i));
		ListChanged?.Invoke(this);
	}

	private protected override void Copy(ListBase<T, HashSet<T>> source, int sourceIndex, ListBase<T, HashSet<T>> destination, int destinationIndex, int count)
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
			if (entries[i].hashCode < 0)
				skipped++;
		for (int i = 0; i < count; i++)
			if (entries[i].hashCode >= 0)
				array[arrayIndex++] = entries[index + i + skipped].item;
			else
				count++;
		ListChanged?.Invoke(this);
	}

	[DllExport("Dispose", CallingConvention.Cdecl)]
	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		freeCount = 0;
		freeList = -1;
		_size = 0;
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
				while (entries[index].hashCode < 0)
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
				while (entries[index].hashCode < 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item is null))
					return false;
			}
			return !toEnd || index == _size;
		}
	}

	internal static int ExpandPrime(int oldSize)
	{
		int newSize = 2 * oldSize;
		if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
			return MaxPrimeArrayLength;
		return GetPrime(newSize);
	}

	[DllExport("FilterInPlace", CallingConvention.Cdecl)]
	public override HashSet<T> FilterInPlace(Func<T, bool> match)
	{
		foreach (T item in this)
			if (!match(item))
				RemoveValue(item);
		return this;
	}

	[DllExport("FilterInPlace", CallingConvention.Cdecl)]
	public override HashSet<T> FilterInPlace(Func<T, int, bool> match)
	{
		int i = 0;
		foreach (T item in this)
			if (!match(item, i++))
				RemoveValue(item);
		return this;
	}

	[DllExport("GetEnumerator", CallingConvention.Cdecl)]
	public override IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	private protected virtual Enumerator GetEnumeratorInternal() => new(this);

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

	private protected override int IndexOfInternal(T item, int index, int count)
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

	private protected virtual void Initialize(int capacity, out List<int> buckets, out List<Entry> entries)
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

	private protected virtual HashSet<T> Insert(T? item, bool add)
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

	private protected override HashSet<T> InsertInternal(int index, IEnumerable<T> collection)
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

	[DllExport("RemoveAt", CallingConvention.Cdecl)]
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

	[DllExport("RemoveValue", CallingConvention.Cdecl)]
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

	private protected virtual void Resize() => Resize(ExpandPrime(_size), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
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

	[DllExport("TryAdd", CallingConvention.Cdecl)]
	public override bool TryAdd(T item)
	{
		if (Contains(item))
			return false;
		Insert(item, true);
		return true;
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
