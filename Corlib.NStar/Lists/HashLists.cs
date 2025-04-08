
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
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
	private protected readonly FastDelHashSet<T> uniqueElements = [];
	internal const int HashSearchMultiplier = 32, AnyHashIndexThreshold = HashSearchMultiplier << 1;

	public BaseHashList() : this(0, (IEqualityComparer<T>?)null) { }

	public BaseHashList(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public BaseHashList(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public BaseHashList(int capacity, IEqualityComparer<T>? comparer)
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

	public BaseHashList(IEnumerable<T> collection) : this(collection, null) { }

	public BaseHashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection.TryGetLengthEasily(out var length) ? length : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			Add(item);
	}

	public BaseHashList(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public BaseHashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			Add(item);
	}

	public BaseHashList(params T[] array) : this((IEnumerable<T>)array) { }

	public BaseHashList(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public BaseHashList(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public BaseHashList(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override int Capacity
	{
		get => buckets?.Length ?? 0;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			Resize(value, false);
			Changed();
		}
	}

	public virtual IEqualityComparer<T> Comparer { get; private protected set; } = EqualityComparer<T>.Default;

	public override TCertain Add(T item) => Add(item, out _);

	public virtual TCertain Add(T item, out int index) => Insert(item, false, out index);

	public override Span<T> AsSpan(int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
		+ " Используйте GetSlice() вместо него.");

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
		var endIndex = index + length;
		for (var i = index; i < endIndex; i++)
			SetNull(i);
		Changed();
	}

	public override bool Contains(T? item, int index, int length) => item != null && IndexOf(item, index, length) >= 0;

	private protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (this != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		destination.Changed();
	}

	public override void Dispose()
	{
		buckets = default!;
		entries = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	public virtual int FirstHashIndexOf(T item) => FirstHashIndexOf(item, _size - 1, _size);

	public virtual int FirstHashIndexOf(T item, int index) => FirstHashIndexOf(item, index, index + 1);

	public virtual int FirstHashIndexOf(T item, int index, int length) => HashIndexesOf(item, index, length).Min();

	public virtual int FirstIndexOf(T item) => FirstIndexOf(item, _size - 1, _size);

	public virtual int FirstIndexOf(T item, int index) => FirstIndexOf(item, index, index + 1);

	public virtual int FirstIndexOf(T item, int index, int length) => IsHashSearchBetter() ? FirstHashIndexOf(item, index, length) : FirstLinearIndexOf(item, index, length);

	public virtual int FirstLinearIndexOf(T item) => FirstLinearIndexOf(item, _size - 1, _size);

	public virtual int FirstLinearIndexOf(T item, int index) => FirstLinearIndexOf(item, index, index + 1);

	public virtual int FirstLinearIndexOf(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		var endIndex = index + length;
		for (var i = index; i < endIndex; i++)
			if (Comparer.Equals(entries[i].item, item))
				return i;
		return -1;
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = entries[index].item;
		if (invoke)
			Changed();
		return item;
	}

	public virtual NList<int> HashIndexesOf(T item) => HashIndexesOf(item, _size - 1, _size);

	public virtual NList<int> HashIndexesOf(T item, int index) => HashIndexesOf(item, index, index + 1);

	public virtual NList<int> HashIndexesOf(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		NList<int> result = [];
		if (buckets != null)
		{
			uint collisionCount = 0;
			var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
			for (var i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
			{
				if (~entries[i].next == i)
					throw new InvalidOperationException();
				if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + length)
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

	public virtual int HashIndexOf(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		if (buckets != null)
		{
			uint collisionCount = 0;
			var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
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

	public virtual NList<int> IndexesOf(T item) => IndexesOf(item, 0, _size);

	public virtual NList<int> IndexesOf(T item, int index) => IndexesOf(item, index, _size - index);

	public virtual NList<int> IndexesOf(T item, int index, int length) => IsHashSearchBetter() ? HashIndexesOf(item, index, length) : LinearIndexesOf(item, index, length);

	private protected override int IndexOfInternal(T item, int index, int length) => uniqueElements.Length >= AnyHashIndexThreshold ? HashIndexOf(item, index, length) : LinearIndexOf(item, index, length);

	private protected virtual void Initialize(int capacity, out int[] buckets, out Entry[] entries)
	{
		var size = HashHelpers.GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
	}

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
			EnsureCapacity(_size + 1);
		var this2 = (TCertain)this;
		if (index < _size)
			CopyToInternal(index, this2, index + 1, _size - index);
		entries[index].item = item;
		uniqueElements.TryAdd(item);
		return this2;
	}

	private protected abstract TCertain Insert(T? item, bool add, out int index);

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		var this2 = (TCertain)this;
		var list = CollectionCreator(collection);
		var length = list._size;
		if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < entries.Length - length)
				CopyToInternal(index, this2, index + length, entries.Length - index - length);
			list.CopyToInternal(0, this2, index, length);
		}
		return this2;
	}

	private protected virtual bool IsHashSearchBetter() => (long)_size * HashSearchMultiplier < (long)uniqueElements.Length * uniqueElements.Length;

	public virtual int LastHashIndexOf(T item) => LastHashIndexOf(item, _size - 1, _size);

	public virtual int LastHashIndexOf(T item, int index) => LastHashIndexOf(item, index, index + 1);

	public virtual int LastHashIndexOf(T item, int index, int length) => HashIndexesOf(item, index, length).Max();

	private protected override int LastIndexOfInternal(T item, int index, int length) => IsHashSearchBetter() ? LastHashIndexOf(item, index, length) : LastLinearIndexOf(item, index, length);

	public virtual int LastLinearIndexOf(T item) => LastLinearIndexOf(item, _size - 1, _size);

	public virtual int LastLinearIndexOf(T item, int index) => LastLinearIndexOf(item, index, index + 1);

	public virtual int LastLinearIndexOf(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (_size != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (_size == 0)
			return -1;
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _size);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(length, index + 1);
		var endIndex = index - length + 1;
		for (var i = index; i >= endIndex; i--)
			if (Comparer.Equals(entries[i].item, item))
				return i;
		return -1;
	}

	public virtual NList<int> LinearIndexesOf(T item) => LinearIndexesOf(item, _size - 1, _size);

	public virtual NList<int> LinearIndexesOf(T item, int index) => LinearIndexesOf(item, index, index + 1);

	public virtual NList<int> LinearIndexesOf(T item, int index, int length)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		NList<int> result = [];
		var endIndex = index + length;
		for (var i = index; i < endIndex; i++)
			if (Comparer.Equals(entries[i].item, item))
				result.Add(i);
		return result;
	}

	public virtual int LinearIndexOf(T item) => LinearIndexOf(item, _size - 1, _size);

	public virtual int LinearIndexOf(T item, int index) => LinearIndexOf(item, index, index + 1);

	public virtual int LinearIndexOf(T item, int index, int length) => FirstLinearIndexOf(item, index, length);

	private protected virtual void Resize() => Resize(HashHelpers.ExpandPrime(_size), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		var newEntries = new Entry[newSize];
		Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (var i = 0; i < _size; i++)
			{
				ref var t = ref newEntries[i];
				if (t.hashCode != 0)
					t.hashCode = ~Comparer.GetHashCode(t.item ?? throw new InvalidOperationException()) & 0x7FFFFFFF;
			}
		buckets = new int[newSize];
		for (var i = 0; i < _size; i++)
			if (newEntries[i].hashCode < 0)
			{
				var bucket = ~newEntries[i].hashCode % newSize;
				ref var t = ref newEntries[i];
				t.next = buckets[bucket];
				buckets[bucket] = ~i;
			}
		entries = newEntries;
	}

	private protected override TCertain ReverseInternal(int index, int length) => throw new NotImplementedException();

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
public abstract class FastDelHashList<T, TCertain> : BaseHashList<T, TCertain> where TCertain : FastDelHashList<T, TCertain>, new()
{
	private protected int freeCount;
	private protected int freeList;

	protected FastDelHashList() { }

	protected FastDelHashList(int capacity) : base(capacity) { }

	protected FastDelHashList(IEqualityComparer<T>? comparer) : base(comparer) { }

	protected FastDelHashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	protected FastDelHashList(IEnumerable<T> collection) : base(collection) { }

	protected FastDelHashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	protected FastDelHashList(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	protected FastDelHashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	protected FastDelHashList(params T[] array) : base(array) { }

	protected FastDelHashList(int capacity, params T[] array) : base(capacity, array) { }

	protected FastDelHashList(ReadOnlySpan<T> span) : base(span) { }

	protected FastDelHashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

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
			var index2 = index.GetOffset(entries.Length);
			return (uint)index2 >= (uint)entries.Length ? throw new IndexOutOfRangeException() : GetInternal(index2, invoke);
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
			var index2 = index.GetOffset(entries.Length);
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

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		var skipped = 0;
		for (var i = 0; i < index; i++)
			if (entries[i].hashCode >= 0)
				skipped++;
		var endIndex = index + length;
		for (var i = index; i < endIndex; i++)
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
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _size - list.Count);
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
		return (TCertain)this;
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
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
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
		uniqueElements.TryAdd(item);
		Changed();
		return (TCertain)this;
	}

	public override TCertain RemoveAt(int index)
	{
		if (buckets == null || entries == null)
			return (TCertain)this;
		var item = entries[index].item;
		if (item == null)
			return (TCertain)this;
		var hashCode = base.Comparer.GetHashCode(item) & 0x7FFFFFFF;
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
		if (!Contains(item))
			uniqueElements.RemoveValue(item);
		Changed();
		return (TCertain)this;
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
		ref var t2 = ref entries[index];
		var oldItem = t2.item;
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
		private readonly FastDelHashList<T, TCertain> hashList;
		private int index;

		internal Enumerator(FastDelHashList<T, TCertain> hashList)
		{
			this.hashList = hashList;
			index = 0;
			Current = default!;
		}

		public bool MoveNext()
		{
			while ((uint)index < (uint)hashList.Size)
			{
				if (hashList.entries[index].hashCode < 0)
				{
					Current = hashList.entries[index].item;
					index++;
					return true;
				}
				index++;
			}
			index = hashList._size + 1;
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
public class FastDelHashList<T> : FastDelHashList<T, FastDelHashList<T>>
{
	public FastDelHashList() : base() { }

	public FastDelHashList(int capacity) : base(capacity) { }

	public FastDelHashList(IEqualityComparer<T>? comparer) : base(comparer) { }

	public FastDelHashList(IEnumerable<T> set) : base(set) { }

	public FastDelHashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public FastDelHashList(int capacity, IEnumerable<T> set) : base(capacity, set) { }

	public FastDelHashList(int capacity, params T[] array) : base(capacity, array) { }

	public FastDelHashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public FastDelHashList(int capacity, IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(capacity, set, comparer) { }

	public FastDelHashList(List<T> list) : this((IEnumerable<T>)list) { }

	public FastDelHashList(params T[] array) : base(array) { }

	public FastDelHashList(ReadOnlySpan<T> span) : base(span) { }

	public FastDelHashList(IEnumerable<T> set, IEqualityComparer<T>? comparer) : base(set, comparer) { }

	private protected override Func<int, FastDelHashList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, FastDelHashList<T>> CollectionCreator => x => new(x);

	private protected override Func<ReadOnlySpan<T>, FastDelHashList<T>> SpanCreator => x => new(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
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
	protected HashList() { }

	protected HashList(int capacity) : base(capacity) { }

	protected HashList(IEqualityComparer<T>? comparer) : base(comparer) { }

	protected HashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	protected HashList(IEnumerable<T> collection) : base(collection) { }

	protected HashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	protected HashList(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	protected HashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	protected HashList(params T[] array) : base(array) { }

	protected HashList(int capacity, params T[] array) : base(capacity, array) { }

	protected HashList(ReadOnlySpan<T> span) : base(span) { }

	protected HashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
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
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		if (_size == entries.Length)
			Resize();
		var targetBucket = hashCode % buckets.Length;
		index = _size;
		_size++;
		ref var t = ref entries[index];
		t.hashCode = ~hashCode;
		t.next = buckets[targetBucket];
		t.item = item;
		buckets[targetBucket] = ~index;
		uniqueElements.TryAdd(item);
		Changed();
		return (TCertain)this;
	}

	public override TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		var item = entries[index].item;
		_size--;
		if (index < _size)
			CopyToInternal(index + 1, this2, index, _size - index);
		SetNull(_size);
		if (!Contains(item))
			uniqueElements.RemoveValue(item);
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
		for (var i = ~buckets[oldBucket]; i >= 0; last = i, i = ~entries[i].next)
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
		var oldItem = t.item;
		t.hashCode = ~hashCode;
		t.next = buckets[bucket];
		t.item = item;
		buckets[bucket] = ~index;
		if (!Contains(oldItem))
			uniqueElements.RemoveValue(oldItem);
		uniqueElements.TryAdd(item);
		Changed();
	}
}

public class HashList<T> : HashList<T, HashList<T>>
{
	public HashList() : base() { }

	public HashList(int capacity) : base(capacity) { }

	public HashList(IEqualityComparer<T>? comparer) : base(comparer) { }

	public HashList(IEnumerable<T> collection) : base(collection) { }

	public HashList(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public HashList(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public HashList(int capacity, params T[] array) : base(capacity, array) { }

	public HashList(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public HashList(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	public HashList(params T[] array) : base(array) { }

	public HashList(ReadOnlySpan<T> span) : base(span) { }

	public HashList(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	private protected override Func<int, HashList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, HashList<T>> CollectionCreator => x => new(x);

	private protected override Func<ReadOnlySpan<T>, HashList<T>> SpanCreator => x => new(x);
}
