using Corlib.NStar;
using System.Linq;

namespace Corlib.NStar;

public abstract class SetBase<T, TCertain> : ListBase<T, TCertain>, ISet<T>, ICollection where TCertain : SetBase<T, TCertain>, new()
{
	public virtual IEqualityComparer<T> Comparer { get; protected set; } = EqualityComparer<T>.Default;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	public override TCertain Add(T item)
	{
		TryAdd(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	bool ISet<T>.Add(T item) => TryAdd(item);

	public override Span<T> AsSpan(int index, int count) => throw new NotSupportedException();

	public override bool Contains(T? item, int index, int count) => item != null && IndexOf(item, index, count) >= 0;

	public virtual void ExceptWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			RemoveValue(item);
	}

	public override TCertain Insert(int index, T item)
	{
		if (!Contains(item))
		{
			base.Insert(index, item);
			_size--;
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual void IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = CollectionCreator(other);
		foreach (T item in this)
			if (!set.Contains(item))
				RemoveValue(item);
	}

	public virtual bool IsProperSubsetOf(IEnumerable<T> other) => !SetEquals(other is ISet<T> set ? set : set = CollectionCreator(other)) && IsSubsetOf(set);

	public virtual bool IsProperSupersetOf(IEnumerable<T> other) => !SetEquals(other) && IsSupersetOf(other);

	public virtual bool IsSubsetOf(IEnumerable<T> other) => other is ISet<T> set ? set.IsSupersetOf(this) : IsSubsetOf(CollectionCreator(other));

	public virtual bool IsSupersetOf(IEnumerable<T> other)
	{
		foreach (T item in other)
			if (!Contains(item))
				return false;
		return true;
	}

	private protected override int LastIndexOfInternal(T item, int index, int count) => throw new NotSupportedException();

	public virtual bool Overlaps(IEnumerable<T> other)
	{
		foreach (T item in other)
			if (Contains(item))
				return true;
		return false;
	}

	private protected override TCertain ReverseInternal(int index, int count) => throw new NotSupportedException();

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

	public abstract bool TryAdd(T item);

	public virtual void UnionWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			TryAdd(item);
	}
}

public abstract class HashSet<T, TCertain> : SetBase<T, TCertain> where TCertain : HashSet<T, TCertain>, new()
{
	private protected struct Entry
	{
		internal int hashCode;
		internal int next;
		internal T item;
	}

	private protected List<int> buckets = default!;
	private protected Entry[] entries = default!;
	private protected int freeCount;
	private protected int freeList;
	internal const int HashPrime = 101;
	internal const int MaxPrimeArrayLength = 0x7FEFFFFD;
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

	public HashSet(IEnumerable<T> collection) : this(collection, null) { }

	public HashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetCountEasily(out int count) ? (int)(Sqrt(count) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			TryAdd(item);
	}

	public HashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public HashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (T item in collection)
			TryAdd(item);
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
			Changed();
		}
	}

	public override int Length => _size - freeCount;

	public virtual int Size => _size;

	private protected override void ClearInternal()
	{
		if (_size > 0)
		{
			for (int i = 0; i < buckets.Length; i++)
			{
				buckets[i] = 0;
				entries[i] = new();
			}
			freeList = 0;
			_size = 0;
			freeCount = 0;
		}
	}

	private protected override void ClearInternal(int index, int count)
	{
		for (int i = 0; i < count; i++)
			RemoveValue(GetInternal(index + i));
		Changed();
	}

	private protected override void Copy(ListBase<T, TCertain> source, int sourceIndex, ListBase<T, TCertain> destination, int destinationIndex, int count)
	{
		if (destination is not TCertain destination2)
			throw new InvalidOperationException();
		if (source != destination || sourceIndex >= destinationIndex)
			for (int i = 0; i < count; i++)
				destination2.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		else
			for (int i = count - 1; i >= 0; i--)
				destination2.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		destination2.Changed();
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
		Changed();
	}

	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		freeCount = 0;
		freeList = 0;
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

	internal static int ExpandPrime(int oldSize)
	{
		int newSize = 2 * oldSize;
		if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
			return MaxPrimeArrayLength;
		return GetPrime(newSize);
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

	public override IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	private protected virtual Enumerator GetEnumeratorInternal() => new(this);

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
		freeList = 0;
	}

	private protected virtual TCertain Insert(T? item, bool add)
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
				return this as TCertain ?? throw new InvalidOperationException();
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
		//entries[index] = t;
		buckets[targetBucket] = ~index;
		return this as TCertain ?? throw new InvalidOperationException();
	}

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
			if (this == set)
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
			//entries[bucket] = t;
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
					//entries[last] = t;
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
				{
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
					//newEntries[i] = t;
				}
			}
		for (int i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				int bucket = ~newEntries[i].hashCode % newSize;
				ref Entry t = ref newEntries[i];
				t.next = newBuckets[bucket];
				//newEntries[i] = t;
				newBuckets[bucket] = ~i;
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

	public override bool TryAdd(T item)
	{
		if (Contains(item))
			return false;
		Insert(item, true);
		return true;
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly HashSet<T, TCertain> dictionary;
		private int index;

		internal Enumerator(HashSet<T, TCertain> dictionary)
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

public class HashSet<T> : HashSet<T, HashSet<T>>
{
	public HashSet()
	{
	}

	public HashSet(int capacity) : base(capacity)
	{
	}

	public HashSet(IEqualityComparer<T>? comparer) : base(comparer)
	{
	}

	public HashSet(IEnumerable<T> set) : base(set)
	{
	}

	public HashSet(params T[] array) : base(array)
	{
	}

	public HashSet(ReadOnlySpan<T> span) : base(span)
	{
	}

	public HashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer)
	{
	}

	public HashSet(IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(set, comparer)
	{
	}

	public HashSet(int capacity, IEnumerable<T> set) : base(capacity, set)
	{
	}

	public HashSet(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public HashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	public HashSet(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(capacity, set, comparer)
	{
	}

	private protected override Func<int, HashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, HashSet<T>> CollectionCreator => x => new(x);
}

public class ParallelHashSet<T> : HashSet<T, ParallelHashSet<T>>
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
		Parallel.ForEach(collection, item => TryAdd(item));
	}

	public ParallelHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public ParallelHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		Parallel.ForEach(collection, item => TryAdd(item));
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

	private protected virtual bool BaseContains(T? item) => BaseContains(item, 0, _size);

	private protected virtual bool BaseContains(T? item, int index, int count) => item != null && BaseIndexOf(item, index, count) >= 0;

	private protected virtual int BaseIndexOf(T item, int index, int count)
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

	private protected virtual ParallelHashSet<T> BaseInsert(T? item, bool add)
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
				BaseResize();
				targetBucket = hashCode % buckets.Length;
			}
			index = _size;
			_size++;
		}
		ref Entry t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		//entries[index] = t;
		buckets[targetBucket] = ~index;
		return this;
	}

	private protected virtual ParallelHashSet<T> BaseRemoveAt(int index)
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
			//entries[bucket] = t;
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

	private protected virtual bool BaseRemoveValue(T? item)
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
					//entries[last] = t;
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

	private protected virtual void BaseResize() => BaseResize(ExpandPrime(_size), false);

	private protected virtual void BaseResize(int newSize, bool forceNewHashCodes)
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
					//newEntries[i] = t;
				}
			}
		for (int i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				int bucket = ~newEntries[i].hashCode % newSize;
				ref Entry t = ref newEntries[i];
				t.next = newBuckets[bucket];
				//newEntries[i] = t;
				newBuckets[bucket] = ~i;
			}
		buckets = newBuckets;
		entries = newEntries;
	}

	private protected virtual bool BaseTryAdd(T item)
	{
		if (BaseContains(item))
			return false;
		BaseInsert(item, true);
		return true;
	}

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

	public override bool Contains(T? item, int index, int count) => BaseContains(item, index, count) || Lock(lockObj, BaseContains, item, index, count);

	public override bool Contains(IEnumerable<T> collection, int index, int count) => Lock(lockObj, base.Contains, collection, index, count);

	private protected override void Copy(ListBase<T, ParallelHashSet<T>> source, int sourceIndex, ListBase<T, ParallelHashSet<T>> destination, int destinationIndex, int count) => Lock(lockObj, base.Copy, source, sourceIndex, destination, destinationIndex, count);

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false) => Lock(lockObj, base.EqualsInternal, collection, index, toEnd);

	public override void ExceptWith(IEnumerable<T> other) => Parallel.ForEach(other, item => RemoveValue(item));

	public override int IndexOf(IEnumerable<T> collection, int index, int count, out int otherCount)
	{
		lock (lockObj)
			return base.IndexOf(collection, index, count, out otherCount);
	}

	private protected override int IndexOfInternal(T item, int index, int count)
	{
		int foundIndex = BaseIndexOf(item, index, count);
		return foundIndex < 0 ? foundIndex : Lock(lockObj, BaseIndexOf, item, index, count);
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

	private protected override ParallelHashSet<T> Insert(T? item, bool add)
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
		lock (lockObj)
		{
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
					BaseResize();
					targetBucket = hashCode % buckets.Length;
				}
				index = _size;
				_size++;
			}
			ref Entry t = ref entries[index];
			t.hashCode = ~hashCode;
			t.next = buckets[targetBucket];
			t.item = item;
			//entries[index] = t;
			buckets[targetBucket] = ~index;
			return this;
		}
	}

	public override void IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = CollectionCreator(other);
		Parallel.ForEach(this, item =>
		{
			if (!set.Contains(item))
				RemoveValue(item);
		});
	}

	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		bool result = true;
		Parallel.ForEach(other, (item, pls) =>
		{
			if (!Contains(item))
			{
				result = false;
				pls.Stop();
			}
		});
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
		Parallel.ForEach(other, (item, pls) =>
		{
			if (Contains(item))
			{
				result = true;
				pls.Stop();
			}
		});
		return result;
	}

	public override ParallelHashSet<T> RemoveAt(int index) => Lock(lockObj, BaseRemoveAt, index);

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
				return Lock(lockObj, BaseRemoveValue, item);
		return false;
	}

	private protected override void Resize(int newSize, bool forceNewHashCodes) => Lock(lockObj, BaseResize, newSize, forceNewHashCodes);

	public override bool SetEquals(IEnumerable<T> other)
	{
		bool result = true;
		if (other.TryGetCountEasily(out int count))
		{
			if (Length != count)
				return false;
			Parallel.ForEach(other, (item, pls) =>
			{
				if (!Contains(item))
				{
					result = false;
					pls.Stop();
				}
			});
			return result;
		}
		else
		{
			ParallelHashSet<T> set = new(other);
			if (Length != set.Length)
				return false;
			Parallel.ForEach(set, (item, pls) =>
			{
				if (!Contains(item))
				{
					result = false;
					pls.Stop();
				}
			});
			return result;
		}
	}

	public override bool TryAdd(T item) => !BaseContains(item) && Lock(lockObj, BaseTryAdd, item);

	public override void UnionWith(IEnumerable<T> other) => Parallel.ForEach(other, item => TryAdd(item));
}
