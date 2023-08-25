using System.Diagnostics;

namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseHashSet<T, TCertain> : BaseSet<T, TCertain> where TCertain : BaseHashSet<T, TCertain>, new()
{
	private protected struct Entry
	{
		internal int hashCode;
		internal int next;
		internal T item;
	}

	private protected int[] buckets = default!;
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
			for (var i = 0; i < buckets.Length; i++)
			{
				buckets[i] = 0;
				entries[i] = new();
			}
			_size = 0;
		}
	}

	private protected override void ClearInternal(int index, int length)
	{
		for (var i = 0; i < length; i++)
			SetNull(index + i);
		Changed();
	}

	//private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int length)
	//{
	//	if (source != destination || sourceIndex >= destinationIndex)
	//		for (var i = 0; i < length; i++)
	//			CopyOne(sourceIndex + i, destinationIndex + i, source, destination);
	//	else
	//		for (var i = length - 1; i >= 0; i--)
	//			CopyOne(sourceIndex + i, destinationIndex + i, source, destination);
	//	destination.Changed();
	//}

	//private static void CopyOne(int sourceIndex, int destinationIndex, TCertain source2, TCertain destination)
	//{
	//	var hashCode = ~source2.entries[sourceIndex].hashCode;
	//	if (hashCode < 0)
	//	{
	//		destination.SetNull(destinationIndex);
	//		return;
	//	}
	//	var bucket = hashCode % destination.buckets.Length;
	//	ref Entry t = ref destination.entries[destinationIndex];
	//	uint collisionCount = 0;
	//	var oldBucket = ~t.hashCode % destination.buckets.Length;
	//	var last = -1;
	//	for (var i = ~destination.buckets[oldBucket]; i >= 0; last = i, i = ~destination.entries[i].next)
	//	{
	//		if (i == destinationIndex)
	//		{
	//			if (last < 0)
	//				destination.buckets[oldBucket] = destination.entries[i].next;
	//			else
	//			{
	//				ref Entry t2 = ref destination.entries[last];
	//				t2.next = destination.entries[i].next;
	//			}
	//			break;
	//		}
	//		collisionCount++;
	//		if (collisionCount > destination.entries.Length)
	//			throw new InvalidOperationException();
	//	}
	//	t = source2.entries[sourceIndex];
	//	t.next = destination.buckets[bucket];
	//	destination.buckets[bucket] = ~destinationIndex;
	//}

	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = entries[index].item;
		if (invoke)
			Changed();
		return item;
	}

	private protected override int IndexOfInternal(T item, int index, int length) => IndexOfInternal(item, index, length, -1);

	private protected virtual int IndexOfInternal(T item, int index, int length, int hashCode = -1)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets != null)
		{
			uint collisionCount = 0;
			if (hashCode < 0)
				hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (var i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
			{
				if (~entries[i].next == i)
					throw new InvalidOperationException();
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + length)
					return i;
				collisionCount++;
				if (collisionCount > entries.Length)
					throw new InvalidOperationException();
			}
		}
		return -1;
	}

	private protected virtual void Initialize(int capacity, out int[] buckets, out Entry[] entries)
	{
		var size = HashHelpers.GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
	}

	private protected abstract TCertain Insert(T? item, bool add, out int index, int hashCode);

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		var set = CollectionCreator(collection);
		set.ExceptWith(this);
		var length = set._size;
		if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < _size)
				Copy(this2, index, this2, index + length, _size - index);
			if (this == collection)
				return this as TCertain ?? throw new InvalidOperationException();
			else
				Copy(set, 0, this2, index, length);
		}
		return this2;
	}

	private protected virtual void Resize() => Resize(HashHelpers.ExpandPrime(_size), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		var newBuckets = new int[newSize];
		var newEntries = new Entry[newSize];
		Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (var i = 0; i < _size; i++)
			{
				ref var t = ref newEntries[i];
				if (t.hashCode != 0)
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
			}
		for (var i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				var bucket = ~newEntries[i].hashCode % newSize;
				ref var t = ref newEntries[i];
				t.next = newBuckets[bucket];
				newBuckets[bucket] = ~i;
			}
		buckets = newBuckets;
		entries = newEntries;
	}

	private protected virtual void SetNull(int index)
	{
		ref var t = ref entries[index];
		if (t.hashCode >= 0)
			return;
		uint collisionCount = 0;
		var bucket = ~t.hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref var t2 = ref entries[last];
					t2.next = entries[i].next;
				}
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		t.hashCode = 0;
		t.next = 0;
		t.item = default!;
	}

	public override bool TryAdd(T item, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		if (TryGetIndexOf(item, out index, hashCode))
			return false;
		Insert(item, true, out index, hashCode);
		return true;
	}

	public override bool TryGetIndexOf(T item, out int index) => TryGetIndexOf(item, out index, -1);

	private protected virtual bool TryGetIndexOf(T item, out int index, int hashCode = -1) => (index = IndexOfInternal(item, 0, _size, hashCode)) >= 0;
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
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
public abstract class FastDelHashSet<T, TCertain> : BaseHashSet<T, TCertain> where TCertain : FastDelHashSet<T, TCertain>, new()
{
	private protected int freeCount;
	private protected int freeList;

	public FastDelHashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public FastDelHashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public FastDelHashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public FastDelHashSet(int capacity, IEqualityComparer<T>? comparer)
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

	public FastDelHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public FastDelHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (var item in collection)
			TryAdd(item);
	}

	public FastDelHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public FastDelHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (var item in collection)
			TryAdd(item);
	}

	public FastDelHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public FastDelHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public FastDelHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public FastDelHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override T this[Index index, bool invoke = true] { get => this[index, invoke, false]; set => this[index, invoke, false] = value; }

	public virtual T this[Index index, bool invoke = true, bool suppressException = false]
	{
		get
		{
			if (!suppressException && freeCount != 0)
				try
				{
					throw new FakeIndexesException();
				}
				catch
				{
				}
			var index2 = index.GetOffset(entries.Length);
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set
		{
			if (!suppressException && freeCount != 0)
				try
				{
					throw new FakeIndexesException();
				}
				catch
				{
				}
			var index2 = index.GetOffset(entries.Length);
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			if (entries[index2].hashCode < 0 && (entries[index2].item?.Equals(value) ?? value == null))
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

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		var skipped = 0;
		for (var i = 0; i < index; i++)
			if (entries[i].hashCode >= 0)
				skipped++;
		for (var i = 0; i < length; i++)
			if (entries[i].hashCode < 0)
				array[arrayIndex++] = entries[index + i + skipped].item;
			else
				length++;
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
			for (var i = 0; i < list.Count; i++)
			{
				while (entries[index].hashCode >= 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(list[i]) ?? list[i] == null))
					return false;
			}
			return !toEnd || index == _size;
		}
		else
		{
			if (collection.TryGetLengthEasily(out var length) && index > _size - length)
				throw new ArgumentOutOfRangeException(nameof(index));
			foreach (var item in collection)
			{
				while (entries[index].hashCode >= 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item == null))
					return false;
			}
			return !toEnd || index == _size;
		}
	}

	public override TCertain FilterInPlace(Func<T, bool> match)
	{
		foreach (var item in this as TCertain ?? throw new InvalidOperationException())
			if (!match(item))
				RemoveValue(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain FilterInPlace(Func<T, int, bool> match)
	{
		var i = 0;
		foreach (var item in this as TCertain ?? throw new InvalidOperationException())
			if (!match(item, i++))
				RemoveValue(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain FixUpFakeIndexes()
	{
		var newSize = HashHelpers.GetPrime(_size - freeCount);
		var newBuckets = new int[newSize];
		var newEntries = new Entry[newSize];
		var skipped = 0;
		for (var i = 0; i < entries.Length; i++)
			if (entries[i].hashCode < 0)
				newEntries[i - skipped] = entries[i];
			else
				skipped++;
		for (var i = 0; i < newSize; i++)
			if (newEntries[i].hashCode < 0)
			{
				var bucket = ~newEntries[i].hashCode % newSize;
				ref var t = ref newEntries[i];
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

	private protected override TCertain Insert(T? item, bool add, out int index, int hashCode)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		var targetBucket = hashCode % buckets.Length;
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
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual bool IsValidIndex(int index) => entries[index].hashCode < 0;

	public override TCertain RemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return this as TCertain ?? throw new InvalidOperationException();
		if (entries[index].hashCode >= 0)
			return this as TCertain ?? throw new InvalidOperationException();
		ref var t = ref entries[index];
		uint collisionCount = 0;
		var bucket = ~t.hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref var t2 = ref entries[last];
					t2.next = entries[i].next;
				}
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		t.hashCode = 0;
		t.next = freeList;
		t.item = default!;
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
		uint collisionCount = 0;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref var t = ref entries[last];
					t.next = entries[i].next;
				}
				ref var t2 = ref entries[i];
				t2.hashCode = 0;
				t2.next = freeList;
				t2.item = default!;
				freeList = ~i;
				freeCount++;
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
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var targetBucket = hashCode % buckets.Length;
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
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		Changed();
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly FastDelHashSet<T, TCertain> hashSet;
		private int index;

		internal Enumerator(FastDelHashSet<T, TCertain> hashSet)
		{
			this.hashSet = hashSet;
			index = 0;
			Current = default!;
		}

		public bool MoveNext()
		{
			while ((uint)index < (uint)hashSet.Size)
			{
				if (hashSet.entries[index].hashCode < 0)
				{
					Current = hashSet.entries[index].item;
					index++;
					return true;
				}
				index++;
			}
			index = hashSet._size + 1;
			Current = default!;
			return false;
		}

		public T Current { get; private set; }

		readonly object? IEnumerator.Current => Current;

		public readonly void Dispose()
		{
		}

		void IEnumerator.Reset()
		{
			index = 0;
			Current = default!;
		}
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
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
public class FastDelHashSet<T> : FastDelHashSet<T, FastDelHashSet<T>>
{
	public FastDelHashSet() : base() { }

	public FastDelHashSet(int capacity) : base(capacity) { }

	public FastDelHashSet(IEqualityComparer<T>? comparer) : base(comparer) { }

	public FastDelHashSet(IEnumerable<T> set) : base(set) { }

	public FastDelHashSet(params T[] array) : base(array) { }

	public FastDelHashSet(ReadOnlySpan<T> span) : base(span) { }

	public FastDelHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public FastDelHashSet(IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(set, comparer) { }

	public FastDelHashSet(int capacity, IEnumerable<T> set) : base(capacity, set) { }

	public FastDelHashSet(int capacity, params T[] array) : base(capacity, array) { }

	public FastDelHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public FastDelHashSet(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(capacity, set, comparer) { }

	private protected override Func<int, FastDelHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, FastDelHashSet<T>> CollectionCreator => x => new(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
/// <summary>
/// Внимание! Рекомендуется не использовать в этом хэш-множестве удаление в цикле, так как такое действие
/// имеет асимптотику O(n²), и при большом размере хэш-множества программа может зависнуть. Дело в том,
/// что здесь, в отличие от класса FastDelHashSet<T>, индексация гарантированно "правильная", но за это
/// приходится платить тем, что после каждого удаления нужно сдвинуть все следующие элементы влево, а это
/// имеет сложность по времени O(n), соответственно, цикл таких действий - O(n²). Если вам нужно произвести
/// серию удалений, используйте FastDelHashSet<T>, а по завершению серии вызовите FixUpFakeIndexes().
/// </summary>
public abstract class ListHashSet<T, TCertain> : BaseHashSet<T, TCertain> where TCertain : ListHashSet<T, TCertain>, new()
{
	public ListHashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public ListHashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public ListHashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public ListHashSet(int capacity, IEqualityComparer<T>? comparer)
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

	public ListHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public ListHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (var item in collection)
			TryAdd(item);
	}

	public ListHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public ListHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (var item in collection)
			TryAdd(item);
	}

	public ListHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public ListHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public ListHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public ListHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override T this[Index index, bool invoke = true]
	{
		get => base[index, invoke];
		set
		{
			var index2 = index.GetOffset(entries.Length);
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			if (entries[index2].item?.Equals(value) ?? value == null)
				return;
			if (Contains(value))
				throw new ArgumentException(null, nameof(value));
			SetInternal(index2, value);
		}
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex++] = entries[index++].item;
	}

	private protected override TCertain Insert(T? item, bool add, out int index, int hashCode)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		var targetBucket = hashCode % buckets.Length;
		if (_size == entries.Length)
		{
			Resize();
			targetBucket = hashCode % buckets.Length;
		}
		index = _size;
		_size++;
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		_size--;
		if (index < _size)
			Copy(this2, index + 1, this2, index, _size - index);
		SetNull(_size);
		Changed();
		return this2;
	}

	internal override void SetInternal(int index, T item)
	{
		var hashCode = item == null ? 0 : Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		ref var t = ref entries[index];
		uint collisionCount = 0;
		var oldBucket = ~t.hashCode % buckets.Length;
		var last = -1;
		for (var i = oldBucket >= 0 ? ~buckets[oldBucket] : -1; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					buckets[oldBucket] = entries[i].next;
				else
				{
					ref var t2 = ref entries[last];
					t2.next = entries[i].next;
				}
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		t.hashCode = ~hashCode;
		t.next = buckets[bucket];
		t.item = item;
		buckets[bucket] = ~index;
		Changed();
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
/// <summary>
/// Внимание! Рекомендуется не использовать в этом хэш-множестве удаление в цикле, так как такое действие
/// имеет асимптотику O(n²), и при большом размере хэш-множества программа может зависнуть. Дело в том,
/// что здесь, в отличие от класса FastDelHashSet<T>, индексация гарантированно "правильная", но за это
/// приходится платить тем, что после каждого удаления нужно сдвинуть все следующие элементы влево, а это
/// имеет сложность по времени O(n), соответственно, цикл таких действий - O(n²). Если вам нужно произвести
/// серию удалений, используйте FastDelHashSet<T>, а по завершению серии вызовите FixUpFakeIndexes().
/// </summary>
public class ListHashSet<T> : ListHashSet<T, ListHashSet<T>>
{
	public ListHashSet() : base() { }

	public ListHashSet(int capacity) : base(capacity) { }

	public ListHashSet(IEqualityComparer<T>? comparer) : base(comparer) { }

	public ListHashSet(IEnumerable<T> collection) : base(collection) { }

	public ListHashSet(params T[] array) : base(array) { }

	public ListHashSet(ReadOnlySpan<T> span) : base(span) { }

	public ListHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public ListHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	public ListHashSet(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public ListHashSet(int capacity, params T[] array) : base(capacity, array) { }

	public ListHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public ListHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	private protected override Func<int, ListHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, ListHashSet<T>> CollectionCreator => x => new(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class ParallelHashSet<T> : FastDelHashSet<T, ParallelHashSet<T>>
{
	private protected readonly object lockObj = new();

	public ParallelHashSet() : base() { }

	public ParallelHashSet(int capacity) : base(capacity) { }

	public ParallelHashSet(IEqualityComparer<T>? comparer) : base(comparer) { }

	public ParallelHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public ParallelHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public ParallelHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (var item in collection)
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
			foreach (var item in collection)
				TryAdd(item);
	}

	public ParallelHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public ParallelHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public ParallelHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public ParallelHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override T this[Index index, bool invoke = true, bool suppressException = false]
	{
		get => base[index, invoke, suppressException];
		set
		{
			lock (lockObj)
				base[index, invoke, suppressException] = value;
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

	private protected override void ClearInternal(int index, int length)
	{
		Parallel.For(0, length, i => RemoveValue(GetInternal(index + i)));
		Changed();
	}

	public override bool Contains(T? item, int index, int length) => UnsafeContains(item, index, length) || Lock(lockObj, UnsafeContains, item, index, length);

	public override bool Contains(IEnumerable<T> collection, int index, int length) => Lock(lockObj, base.Contains, collection, index, length);

	private protected override void Copy(ParallelHashSet<T> source, int sourceIndex, ParallelHashSet<T> destination, int destinationIndex, int length) => Lock(lockObj, base.Copy, source, sourceIndex, destination, destinationIndex, length);

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false) => Lock(lockObj, base.EqualsInternal, collection, index, toEnd);

	public override ParallelHashSet<T> ExceptWith(IEnumerable<T> other)
	{
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, i => RemoveValue(list[i]));
		else
			foreach (var item in other)
				RemoveValue(item);
		return this;
	}

	public override ParallelHashSet<T> FixUpFakeIndexes() => Lock(lockObj, base.FixUpFakeIndexes);

	public override int IndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		lock (lockObj)
			return base.IndexOf(collection, index, length, out collectionLength);
	}

	private protected override int IndexOfInternal(T item, int index, int length, int hashCode = -1)
	{
		var foundIndex = UnsafeIndexOf(item, index, length);
		return foundIndex < 0 ? foundIndex : Lock(lockObj, UnsafeIndexOf, item, index, length, hashCode);
	}

	private protected override void Initialize(int capacity, out int[] buckets, out Entry[] entries)
	{
		var size = HashHelpers.GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
		freeList = 0;
	}

	public override ParallelHashSet<T> Insert(int index, T item) => Lock(lockObj, base.Insert, index, item);

	public override ParallelHashSet<T> Insert(int index, IEnumerable<T> collection) => Lock(lockObj, base.Insert, index, collection);

	private protected override ParallelHashSet<T> Insert(T? item, bool add, out int index, int hashCode)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		uint collisionCount = 0;
		var targetBucket = hashCode % buckets.Length;
		for (var i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
		{
			if (~entries[i].next == i)
				throw new InvalidOperationException();
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				index = i;
				return this;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		lock (lockObj)
		{
			collisionCount = 0;
			for (var i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
			{
				if (~entries[i].next == i)
					throw new InvalidOperationException();
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
				{
					if (add) throw new ArgumentException(null);
					index = i;
					return this;
				}
				collisionCount++;
				if (collisionCount > entries.Length)
					throw new InvalidOperationException();
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
			ref var t = ref entries[index];
			t.hashCode = ~hashCode;
			t.next = buckets[targetBucket];
			t.item = item;
			buckets[targetBucket] = ~index;
			return this;
		}
	}

	public override ParallelHashSet<T> IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = new ParallelHashSet<T>(other);
		Parallel.For(0, Length, i =>
		{
			var item = this[i];
			if (!set.Contains(item))
				RemoveValue(item);
		});
		return this;
	}

	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		var result = true;
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
			foreach (var item in other)
				if (!Contains(item))
					return false;
		return result;
	}

	public override int LastIndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		lock (lockObj)
			return base.LastIndexOf(collection, index, length, out collectionLength);
	}

	public override bool Overlaps(IEnumerable<T> other)
	{
		var result = false;
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
			foreach (var item in other)
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
		uint collisionCount = 0;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
				return Lock(lockObj, UnsafeRemoveValue, item);
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		return false;
	}

	private protected override void Resize(int newSize, bool forceNewHashCodes) => Lock(lockObj, UnsafeResize, newSize, forceNewHashCodes);

	public override bool SetEquals(IEnumerable<T> other)
	{
		var result = true;
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
		else if (other.TryGetLengthEasily(out var length))
		{
			if (Length != length)
				return false;
			foreach (var item in other)
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

	public override bool TryAdd(T item, out int index)
	{
		index = UnsafeIndexOf(item);
		if (index >= 0)
			return false;
		lock (lockObj)
			return UnsafeTryAdd(item, out index);
	}

	public override ParallelHashSet<T> UnionWith(IEnumerable<T> other)
	{
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (var item in other)
				TryAdd(item);
		return this;
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
	public virtual bool UnsafeContains(T? item, int index, int length) => item != null && UnsafeIndexOf(item, index, length) >= 0;

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
	public virtual int UnsafeIndexOf(T item, int index, int length, int hashCode = -1)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets != null)
		{
			uint collisionCount = 0;
			if (hashCode < 0)
				hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (var i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
			{
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + length)
					return i;
				if (~entries[i].next == i)
					throw new InvalidOperationException();
				collisionCount++;
				if (collisionCount > entries.Length)
					throw new InvalidOperationException();
			}
		}
		return -1;
	}

	private protected virtual ParallelHashSet<T> UnsafeInsert(T? item, bool add, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		uint collisionCount = 0;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var targetBucket = hashCode % buckets.Length;
		for (var i = ~buckets[targetBucket]; i >= 0; i = ~entries[i].next)
		{
			if (~entries[i].next == i)
				throw new InvalidOperationException();
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (add)
					throw new ArgumentException(null);
				index = i;
				return this;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
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
		ref var t = ref entries[index];
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
		if (entries[index].hashCode >= 0)
			return this;
		var hashCode = Comparer.GetHashCode(entries[index].item ?? throw new ArgumentException(null)) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		if (bucket != index)
		{
			ref var t = ref entries[bucket];
			t.next = entries[index].next;
		}
		ref var t2 = ref entries[index];
		t2.hashCode = 0;
		t2.next = freeList;
		t2.item = default!;
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
		uint collisionCount = 0;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref var t = ref entries[last];
					t.next = entries[i].next;
				}
				ref var t2 = ref entries[i];
				t2.hashCode = 0;
				t2.next = freeList;
				t2.item = default!;
				freeList = ~i;
				freeCount++;
				return true;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		return false;
	}

	private protected virtual void UnsafeResize() => UnsafeResize(HashHelpers.ExpandPrime(_size), false);

	private protected virtual void UnsafeResize(int newSize, bool forceNewHashCodes)
	{
		var newBuckets = new int[newSize];
		var newEntries = new Entry[newSize];
		Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (var i = 0; i < _size; i++)
			{
				ref var t = ref newEntries[i];
				if (t.hashCode != 0)
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
			}
		for (var i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				var bucket = ~newEntries[i].hashCode % newSize;
				ref var t = ref newEntries[i];
				t.next = newBuckets[bucket];
				newBuckets[bucket] = ~i;
			}
		buckets = newBuckets;
		entries = newEntries;
	}

	private protected virtual bool UnsafeTryAdd(T item, out int index)
	{
		index = UnsafeIndexOf(item);
		if (index >= 0)
			return false;
		UnsafeInsert(item, true, out index);
		return true;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class TreeHashSet<T, TCertain> : BaseHashSet<T, TCertain> where TCertain : TreeHashSet<T, TCertain>, new()
{
	private protected readonly TreeSet<int> deleted = new();
	private protected byte fixes = 0;

	public TreeHashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public TreeHashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public TreeHashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public TreeHashSet(int capacity, IEqualityComparer<T>? comparer)
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

	public TreeHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public TreeHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (var item in collection)
			TryAdd(item);
	}

	public TreeHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public TreeHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		foreach (var item in collection)
			TryAdd(item);
	}

	public TreeHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public TreeHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public TreeHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public TreeHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override T this[Index index, bool invoke = true]
	{
		get
		{
			var index2 = index.GetOffset(entries.Length);
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set
		{
			var index2 = index.GetOffset(entries.Length);
			if ((uint)index2 >= (uint)entries.Length)
				throw new IndexOutOfRangeException();
			if (entries[IndexGetDirect(index2)].item?.Equals(value) ?? value == null)
				return;
			if (Contains(value))
				throw new ArgumentException(null, nameof(value));
			SetInternal(index2, value);
		}
	}

	public override int Length => _size - deleted.Length;

	public virtual int Size => _size;

	private protected override void ClearInternal()
	{
		base.ClearInternal();
		deleted.Clear();
	}

	private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		int sourceIndex2 = IndexGetActual(sourceIndex), destinationIndex2 = IndexGetActual(destinationIndex);
		if (source != destination || sourceIndex2 >= destinationIndex2)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex2 + i, source.GetInternal(sourceIndex2 + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex2 + i, source.GetInternal(sourceIndex2 + i));
		if (destination._size < destinationIndex2 + length + deleted.Length)
			destination._size = destinationIndex2 + length + deleted.Length;
		destination.Changed();
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		var skipped = 0;
		for (var i = 0; i < index; i++)
			if (entries[i].hashCode >= 0)
				skipped++;
		for (var i = 0; i < length; i++)
			if (entries[i].hashCode < 0)
				array[arrayIndex++] = entries[index + i + skipped].item;
			else
				length++;
	}

	public override void Dispose()
	{
		deleted.Dispose();
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
			for (var i = 0; i < list.Count; i++)
			{
				while (entries[index].hashCode >= 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(list[i]) ?? list[i] == null))
					return false;
			}
			return !toEnd || index == _size;
		}
		else
		{
			if (collection.TryGetLengthEasily(out var length) && index > _size - length)
				throw new ArgumentOutOfRangeException(nameof(index));
			foreach (var item in collection)
			{
				while (entries[index].hashCode >= 0)
					index++;
				if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item == null))
					return false;
			}
			return !toEnd || index == _size;
		}
	}

	public override TCertain FilterInPlace(Func<T, bool> match)
	{
		for (var i = 0; i < Length; i++)
			if (!match(GetInternal(i)))
				RemoveAt(i--);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain FilterInPlace(Func<T, int, bool> match)
	{
		for (var i = 0; i < Length; i++)
			if (!match(GetInternal(i), i))
				RemoveAt(i--);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain FixUpDeleted()
	{
		fixes++;
		var newSize = HashHelpers.GetPrime(_size - deleted.Length);
		var newBuckets = new int[newSize];
		var newEntries = new Entry[newSize];
		var skipped = 0;
		for (var i = 0; i < entries.Length; i++)
			if (entries[i].hashCode < 0)
				newEntries[i - skipped] = entries[i];
			else
				skipped++;
		for (var i = 0; i < newSize; i++)
			if (newEntries[i].hashCode < 0)
			{
				var bucket = ~newEntries[i].hashCode % newSize;
				ref var t = ref newEntries[i];
				t.next = newBuckets[bucket];
				newBuckets[bucket] = ~i;
			}
		_size -= deleted.Length;
		buckets = newBuckets;
		entries = newEntries;
		deleted.Clear();
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	private protected virtual Enumerator GetEnumeratorInternal() => new(this);

	internal override T GetInternal(int index, bool invoke = true) => GetDirect(IndexGetDirect(index));

	private protected virtual T GetDirect(int index) => base.GetInternal(index);

	private protected virtual int IndexGetActual(int direct) => direct - deleted.IndexOfNotLess(direct);

	private protected virtual int IndexGetDirect(int actual) => deleted.NthAbsent(actual);

	private protected override int IndexOfInternal(T item, int index, int length, int hashCode = -1) => IndexGetActual(IndexOfDirect(item, CreateVar(IndexGetDirect(index), out var index2), IndexGetDirect(index + length) - index2, hashCode));

	private protected virtual int IndexOfDirect(T item, int index, int length, int hashCode = -1) => base.IndexOfInternal(item, index, length, hashCode);

	private protected override TCertain Insert(T? item, bool add, out int index, int hashCode)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException();
		var targetBucket = hashCode % buckets.Length;
		if (deleted.Length > 0)
			index = deleted.GetAndRemove(^1);
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
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override TCertain RemoveAt(int index) => RemoveAtDirect(IndexGetDirect(index));

	private protected virtual TCertain RemoveAtDirect(int index)
	{
		if (buckets == null || entries == null)
			return this as TCertain ?? throw new InvalidOperationException();
		if (entries[index].hashCode >= 0)
			return this as TCertain ?? throw new InvalidOperationException();
		var hashCode = Comparer.GetHashCode(entries[index].item ?? throw new ArgumentException(null)) & 0x7FFFFFFF;
		uint collisionCount = 0;
		var bucket = hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref var t = ref entries[last];
					t.next = entries[i].next;
				}
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		ref var t2 = ref entries[index];
		t2.hashCode = 0;
		t2.next = 0;
		t2.item = default!;
		deleted.TryAdd(index);
		if (deleted.Length >= Length && deleted.Length >= DefaultCapacity)
			FixUpDeleted();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		uint collisionCount = 0;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
				{
					ref var t = ref entries[last];
					t.next = entries[i].next;
				}
				ref var t2 = ref entries[i];
				t2.hashCode = 0;
				t2.next = 0;
				t2.item = default!;
				deleted.TryAdd(i);
				if (deleted.Length >= Length && deleted.Length >= DefaultCapacity)
					FixUpDeleted();
				return true;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		return false;
	}

	internal override void SetInternal(int index, T item) => SetDirect(IndexGetDirect(index), item, index);

	internal virtual void SetDirect(int index, T item, int actualIndex)
	{
		var deletedLength = deleted.Length;
		if (entries[index].hashCode < 0)
		{
			RemoveAtDirect(index);
			deletedLength++;
		}
		if (deletedLength != deleted.Length)
		{
			Insert(actualIndex, item);
			return;
		}
		if (item == null)
			return;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var bucket = hashCode % buckets.Length;
		if (deleted.Length > 0 && deleted.RemoveValue(index))
		{
		}
		else
		{
			if (_size == entries.Length)
			{
				Resize();
				bucket = hashCode % buckets.Length;
			}
			_size++;
		}
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[bucket];
		t.item = item;
		buckets[bucket] = ~index;
		Changed();
	}

	public new struct Enumerator : IEnumerator<T>
	{
		private readonly TreeHashSet<T, TCertain> hashSet;
		private int index;
		private readonly byte fixes;

		internal Enumerator(TreeHashSet<T, TCertain> hashSet)
		{
			this.hashSet = hashSet;
			index = 0;
			fixes = hashSet.fixes;
			Current = default!;
		}

		public bool MoveNext()
		{
			if (fixes != hashSet.fixes)
				throw new InvalidOperationException();
			while ((uint)index < (uint)hashSet.entries.Length)
			{
				if (hashSet.entries[index].hashCode < 0)
				{
					Current = hashSet.entries[index].item;
					index++;
					return true;
				}
				index++;
			}
			index = hashSet._size + 1;
			Current = default!;
			return false;
		}

		public T Current { get; private set; }

		readonly object? IEnumerator.Current => Current;

		public readonly void Dispose()
		{
		}

		void IEnumerator.Reset()
		{
			index = 0;
			Current = default!;
		}
	}
}

public class TreeHashSet<T> : TreeHashSet<T, TreeHashSet<T>>
{
	public TreeHashSet() : base() { }

	public TreeHashSet(int capacity) : base(capacity) { }

	public TreeHashSet(IEqualityComparer<T>? comparer) : base(comparer) { }

	public TreeHashSet(IEnumerable<T> collection) : base(collection) { }

	public TreeHashSet(params T[] array) : base(array) { }

	public TreeHashSet(ReadOnlySpan<T> span) : base(span) { }

	public TreeHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public TreeHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	public TreeHashSet(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public TreeHashSet(int capacity, params T[] array) : base(capacity, array) { }

	public TreeHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public TreeHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	private protected override Func<int, TreeHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, TreeHashSet<T>> CollectionCreator => x => new(x);
}
