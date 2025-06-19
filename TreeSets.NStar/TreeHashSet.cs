namespace TreeSets.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class TreeHashSet<T, TCertain> : BaseHashSet<T, TCertain> where TCertain : TreeHashSet<T, TCertain>, new()
{
	private protected readonly TreeSet<int> deleted = [];
	private protected byte fixes = 0;

	public TreeHashSet() : this(0, (G.IEqualityComparer<T>?)null) { }

	public TreeHashSet(int capacity) : this(capacity, (G.IEqualityComparer<T>?)null) { }

	public TreeHashSet(G.IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public TreeHashSet(int capacity, G.IEqualityComparer<T>? comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity > 0)
			Initialize(capacity, out buckets, out entries);
		else
		{
			buckets = default!;
			entries = default!;
		}
		Comparer = comparer ?? G.EqualityComparer<T>.Default;
	}

	public TreeHashSet(G.IEnumerable<T> collection) : this(collection, null) { }

	public TreeHashSet(G.IEnumerable<T> collection, G.IEqualityComparer<T>? comparer) : this(collection is G.ISet<T> set ? set.Count : typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			TryAdd(item);
	}

	public TreeHashSet(int capacity, G.IEnumerable<T> collection) : this(capacity, collection, null) { }

	public TreeHashSet(int capacity, G.IEnumerable<T> collection, G.IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			TryAdd(item);
	}

	public TreeHashSet(params T[] array) : this((G.IEnumerable<T>)array) { }

	public TreeHashSet(int capacity, params T[] array) : this(capacity, (G.IEnumerable<T>)array) { }

	public TreeHashSet(ReadOnlySpan<T> span) : this((G.IEnumerable<T>)span.ToArray()) { }

	public TreeHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (G.IEnumerable<T>)span.ToArray()) { }

	public override T this[Index index, bool invoke = true]
	{
		get
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			if (entries[IndexGetDirect(index2)].item?.Equals(value) ?? value == null)
				return;
			if (Contains(value))
				throw new ArgumentException("Ошибка, такой элемент уже был добавлен.", nameof(value));
			SetInternal(index2, value);
		}
	}

	public override int Length => _size - deleted.Length;

	public virtual int Size => _size;

	protected override void ClearInternal()
	{
		base.ClearInternal();
		deleted.Clear();
	}

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		int sourceIndex2 = IndexGetActual(sourceIndex), destinationIndex2 = IndexGetActual(destinationIndex);
		if (this != destination || sourceIndex2 >= destinationIndex2)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex2 + i, GetInternal(sourceIndex2 + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex2 + i, GetInternal(sourceIndex2 + i));
		if (destination._size < destinationIndex2 + length + deleted.Length)
			destination._size = destinationIndex2 + length + deleted.Length;
		destination.Changed();
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => CopyToCommon(IndexGetDirect(index), array, arrayIndex, length);

	public override void Dispose()
	{
		deleted.Dispose();
		base.Dispose();
		GC.SuppressFinalize(this);
	}

	public override TCertain FilterInPlace(Func<T, bool> match)
	{
		for (var i = 0; i < Length; i++)
			if (!match(GetInternal(i)))
				RemoveAt(i--);
		return (TCertain)this;
	}

	public override TCertain FilterInPlace(Func<T, int, bool> match)
	{
		for (var i = 0; i < Length; i++)
			if (!match(GetInternal(i), i))
				RemoveAt(i--);
		return (TCertain)this;
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
		return (TCertain)this;
	}

	public override G.IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	protected virtual Enumerator GetEnumeratorInternal() => new(this);

	protected override T GetInternal(int index, bool invoke = true) => GetDirect(IndexGetDirect(index));

	protected virtual T GetDirect(int index) => base.GetInternal(index);

	protected virtual int IndexGetActual(int direct) => direct - deleted.IndexOfNotLess(direct);

	protected virtual int IndexGetDirect(int actual) => deleted.NthAbsent(actual);

	protected override int IndexOfInternal(T item, int index, int length, int hashCode) => IndexGetActual(IndexOfDirect(item, CreateVar(IndexGetDirect(index), out var index2), IndexGetDirect(index + length) - index2, hashCode));

	protected virtual int IndexOfDirect(T item, int index, int length, int hashCode) => base.IndexOfInternal(item, index, length, hashCode);

	protected override TCertain Insert(T? item, out int index, int hashCode)
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
		Changed();
		return (TCertain)this;
	}

	public override TCertain RemoveAt(int index) => RemoveAtDirect(IndexGetDirect(index));

	protected virtual TCertain RemoveAtDirect(int index)
	{
		if (buckets == null || entries == null)
			return (TCertain)this;
		if (entries[index].hashCode >= 0)
			return (TCertain)this;
		ref var t = ref entries[index];
		RemoveAtCommon(index, ref t);
		t.next = 0;
		deleted.TryAdd(index);
		if (deleted.Length >= Length && deleted.Length >= DefaultCapacity)
			FixUpDeleted();
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
			t.next = 0;
			deleted.TryAdd(i);
			if (deleted.Length >= Length && deleted.Length >= DefaultCapacity)
				FixUpDeleted();
		});
	}

	protected override void SetInternal(int index, T item) => SetDirect(IndexGetDirect(index), item, index);

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

	public new struct Enumerator : G.IEnumerator<T>
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
			while ((uint)index < (uint)hashSet._size)
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

	public TreeHashSet(G.IEqualityComparer<T>? comparer) : base(comparer) { }

	public TreeHashSet(G.IEnumerable<T> collection) : base(collection) { }

	public TreeHashSet(G.IEnumerable<T> collection, G.IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	public TreeHashSet(int capacity, G.IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public TreeHashSet(int capacity, G.IEnumerable<T> collection) : base(capacity, collection) { }

	public TreeHashSet(int capacity, params T[] array) : base(capacity, array) { }

	public TreeHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public TreeHashSet(int capacity, G.IEnumerable<T> collection, G.IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	public TreeHashSet(params T[] array) : base(array) { }

	public TreeHashSet(ReadOnlySpan<T> span) : base(span) { }

	protected override Func<int, TreeHashSet<T>> CapacityCreator => x => new(x);

	protected override Func<G.IEnumerable<T>, TreeHashSet<T>> CollectionCreator => x => new(x);

	protected override Func<ReadOnlySpan<T>, TreeHashSet<T>> SpanCreator => x => new(x);
}
