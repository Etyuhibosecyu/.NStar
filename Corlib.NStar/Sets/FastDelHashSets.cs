
namespace Corlib.NStar;

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
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
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

	public FastDelHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			TryAdd(item);
	}

	public FastDelHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public FastDelHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
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
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
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
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			if (entries[index2].hashCode < 0 && (entries[index2].item?.Equals(value) ?? value == null))
				return;
			if (Contains(value))
				throw new ArgumentException("Ошибка, такой элемент уже был добавлен.", nameof(value));
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

	private protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (this != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
		else
		{
			for (var i = length - destinationIndex + sourceIndex; i < length; i++)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
			for (var i = length - 1; i >= 0; i--)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
		}
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => CopyToCommon(index, array, arrayIndex, length);

	public override void Dispose()
	{
		freeCount = 0;
		freeList = 0;
		base.Dispose();
		GC.SuppressFinalize(this);
	}

	private protected override bool EqualsToList(G.IList<T> list, int index, bool toEnd)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _size - list.Count);
		for (var i = 0; i < list.Count; i++)
		{
			while (index < _size && entries[index].hashCode >= 0)
				index++;
			if (index >= _size || !(GetInternal(index++)?.Equals(list[i]) ?? list[i] == null))
				return false;
		}
		return !toEnd || index == _size;
	}

	private protected override bool EqualsToNonList(IEnumerable<T> collection, int index, bool toEnd)
	{
		if (collection.TryGetLengthEasily(out var length) && index > _size - length)
			throw new ArgumentOutOfRangeException(nameof(index));
		foreach (var item in collection)
		{
			while (index < _size && entries[index].hashCode >= 0)
				index++;
			if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item == null))
				return false;
		}
		return !toEnd || index == _size;
	}

	public override TCertain FilterInPlace(Func<T, bool> match)
	{
		foreach (var item in (TCertain)this)
			if (!match(item))
				RemoveValue(item);
		return (TCertain)this;
	}

	public override TCertain FilterInPlace(Func<T, int, bool> match)
	{
		var i = 0;
		foreach (var item in (TCertain)this)
			if (!match(item, i++))
				RemoveValue(item);
		return (TCertain)this;
	}

	public virtual TCertain FixUpFakeIndexes()
	{
		if (freeCount == 0)
			return (TCertain)this;
		var newSize = _size - freeCount;
		var newSizeExt = HashHelpers.GetPrime(newSize);
		var newBuckets = new int[newSizeExt];
		var newEntries = new Entry[newSizeExt];
		var skipped = 0;
		for (var i = 0; i < _size; i++)
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
		return (TCertain)this;
	}

	public override IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	private protected virtual Enumerator GetEnumeratorInternal() => new(this);

	private protected override Slice<T> GetSliceInternal(int index, int length) => new((G.IList<T>)GetRangeInternal(index, length).FixUpFakeIndexes());

	private protected override TCertain Insert(T? item, out int index, int hashCode)
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
		return (TCertain)this;
	}

	public virtual bool IsValidIndex(int index) => entries[index].hashCode < 0;

	public override TCertain RemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return (TCertain)this;
		if (entries[index].hashCode >= 0)
			return (TCertain)this;
		ref var t = ref entries[index];
		RemoveAtCommon(index, ref t);
		t.next = freeList;
		freeList = ~index;
		freeCount++;
		return (TCertain)this;
	}

	private protected override TCertain RemoveInternal(int index, int length)
	{
		for (var i = index; i < index + length; i++)
			RemoveAt(i);
		return (TCertain)this;
	}

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		return RemoveValueCommon(item, hashCode, (ref Entry t, int i) =>
		{
			t.next = freeList;
			freeList = ~i;
			freeCount++;
		});
	}

	internal override void SetInternal(int index, T item)
	{
		if (entries[index].hashCode < 0)
			RemoveAt(index);
		if (item == null)
			return;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var targetBucket = hashCode % buckets.Length;
		uint collisionCount = 0;
		var last = -1;
		for (var i = ~freeList; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					freeList = entries[i].next;
				else
					entries[last].next = entries[i].next;
				freeCount--;
				goto l1;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		if (_size == entries.Length)
		{
			Resize();
			targetBucket = hashCode % buckets.Length;
		}
		_size++;
	l1:
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		Changed();
		Debug.Assert(IsValidIndex(index) && (entries[index].item?.Equals(item) ?? item == null));
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

	public FastDelHashSet(int capacity, IEnumerable<T> set) : base(capacity, set) { }

	public FastDelHashSet(int capacity, params T[] array) : base(capacity, array) { }

	public FastDelHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public FastDelHashSet(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(capacity, set, comparer) { }

	public FastDelHashSet(params T[] array) : base(array) { }

	public FastDelHashSet(ReadOnlySpan<T> span) : base(span) { }

	public FastDelHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public FastDelHashSet(IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(set, comparer) { }

	private protected override Func<int, FastDelHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, FastDelHashSet<T>> CollectionCreator => x => new(x);

	private protected override Func<ReadOnlySpan<T>, FastDelHashSet<T>> SpanCreator => x => new(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class ParallelHashSet<T> : FastDelHashSet<T, ParallelHashSet<T>>
{
	private protected readonly object lockObj = new();

	public ParallelHashSet() : base() { }

	public ParallelHashSet(int capacity) : base(capacity) { }

	public ParallelHashSet(IEqualityComparer<T>? comparer) : base(comparer) { }

	public ParallelHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public ParallelHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (var item in collection)
				TryAdd(item);
	}

	public ParallelHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public ParallelHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (var item in collection)
				TryAdd(item);
	}

	public ParallelHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public ParallelHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public ParallelHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public ParallelHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public ParallelHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

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

	private protected override Func<ReadOnlySpan<T>, ParallelHashSet<T>> SpanCreator => x => new(x);

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

	private protected override void CopyToInternal(int sourceIndex, ParallelHashSet<T> destination, int destinationIndex, int length) => Lock(lockObj, base.CopyToInternal, sourceIndex, destination, destinationIndex, length);

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false) => Lock(lockObj, base.EqualsInternal, collection, index, toEnd);

	public override ParallelHashSet<T> ExceptWith(IEnumerable<T> other)
	{
		if (other is FastDelHashSet<T> fhs)
			Parallel.For(0, fhs.Size, i => _ = fhs.IsValidIndex(i) && RemoveValue(fhs.GetInternal(i)));
		else if (other is ParallelHashSet<T> phs)
			Parallel.For(0, phs.Size, i => _ = phs.IsValidIndex(i) && RemoveValue(phs.GetInternal(i)));
		else if (other is G.IList<T> list)
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

	private protected override int IndexOfInternal(T item, int index, int length, int hashCode)
	{
		var foundIndex = UnsafeIndexOf(item, index, length);
		return foundIndex < 0 ? foundIndex : Lock(lockObj, UnsafeIndexOf, item, index, length, hashCode);
	}

	private protected override void Initialize(int capacity, out int[] buckets, out Entry[] entries)
	{
		lock (lockObj)
		{
			if (this.buckets == null)
				base.Initialize(capacity, out buckets, out entries);
			else
			{
				buckets = this.buckets;
				entries = this.entries;
			}
		}
	}

	public override ParallelHashSet<T> Insert(int index, T item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (Contains(item))
			return this;
		lock (lockObj)
		{
			base.Insert(index, item);
			Debug.Assert(Contains(item));
		}
		return this;
	}

	public override ParallelHashSet<T> Insert(int index, IEnumerable<T> collection) => Lock(lockObj, base.Insert, index, collection);

	public override ParallelHashSet<T> Insert(int index, ReadOnlySpan<T> span)
	{
		lock(lockObj)
			base.Insert(index, span);
		return this;
	}

	private protected override ParallelHashSet<T> Insert(T? item, out int index, int hashCode)
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
				index = i;
				return this;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException();
		}
		lock (lockObj)
			return UnsafeInsert(item, out index, hashCode);
	}

	public override ParallelHashSet<T> IntersectWith(IEnumerable<T> other)
	{
		if (other is ISet<T> set)
			return Do(set);
		return Do(new ParallelHashSet<T>(other));
		ParallelHashSet<T> Do(ISet<T> set)
		{
			Parallel.For(0, _size, i =>
			{
				var item = GetInternal(i);
				if (!set.Contains(item))
					RemoveValue(item);
			});
			return this;
		}
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

	public override ParallelHashSet<T> RemoveAt(int index)
	{
		if (entries[index].hashCode >= 0)
			return this;
		lock (lockObj)
		{
			UnsafeRemoveAt(index);
			Debug.Assert(entries[index].hashCode >= 0);
		}
		return this;
	}

	private protected override ParallelHashSet<T> RemoveInternal(int index, int length)
	{
		Parallel.For(index, index + length, i => RemoveAt(i));
		return this;
	}

	public override bool RemoveValue(T? item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return false;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var index = UnsafeIndexOf(item, 0, _size, hashCode);
		if (index < 0)
			return false;
		lock (lockObj)
		{
			RemoveValueCommon(item, hashCode, (ref Entry t, int i) =>
			{
				t.next = freeList;
				freeList = ~i;
				freeCount++;
			});
			Debug.Assert(!Contains(item));
		}
		return true;
	}

	private protected override void Resize(int newSize, bool forceNewHashCodes) => Lock(lockObj, base.Resize, newSize, forceNewHashCodes);

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

	private protected override ParallelHashSet<T> SymmetricExceptInternal(IEnumerable<T> other) => Lock(lockObj, base.SymmetricExceptInternal, other);

	public override bool TryAdd(T item, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		index = UnsafeIndexOf(item, 0, _size, hashCode);
		if (index >= 0)
			return false;
		lock (lockObj)
		{
			UnsafeTryAdd(item, out index, hashCode);
			Debug.Assert(Contains(item));
		}
		return true;
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
	public virtual int UnsafeIndexOf(T item, int index, int length) => item != null ? UnsafeIndexOf(item, index, length, Comparer.GetHashCode(item) & 0x7FFFFFFF) : throw new ArgumentNullException(nameof(item));

	private protected virtual int UnsafeIndexOf(T item, int index, int length, int hashCode)
	{
		var buckets = this.buckets;
		if (buckets != null)
		{
			uint collisionCount = 0;
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

	private protected virtual ParallelHashSet<T> UnsafeInsert(T? item, out int index, int hashCode)
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
		ref var t = ref entries[index];
		RemoveAtCommon(index, ref t);
		t.next = freeList;
		freeList = ~index;
		freeCount++;
		return this;
	}

	private protected virtual void UnsafeResize() => base.Resize(HashHelpers.ExpandPrime(_size), false);

	private protected virtual bool UnsafeTryAdd(T item, out int index, int hashCode)
	{
		index = UnsafeIndexOf(item);
		if (index >= 0)
			return false;
		UnsafeInsert(item, out index, hashCode);
		return true;
	}
}
