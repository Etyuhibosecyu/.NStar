namespace NStar.Core;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseHashSet<T, TCertain> : BaseSet<T, TCertain> where TCertain : BaseHashSet<T, TCertain>, new()
{
	protected struct Entry
	{
		public int hashCode;
		public int next;
		public T item;
	}

	protected int[] buckets = default!;
	protected Entry[] entries = default!;

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
	public virtual IEqualityComparer<T> Comparer { get; protected set; } = EqualityComparer<T>.Default;

	protected override void ClearInternal()
	{
		if (_size > 0)
		{
			for (var i = 0; i < buckets.Length; i++)
			{
				buckets[i] = 0;
				entries[i] = new();
			}
			_size = 0;
			Changed();
		}
	}

	protected override void ClearInternal(int index, int length)
	{
		for (var i = 0; i < length; i++)
			SetNull(index + i);
		Changed();
	}

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (this != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, GetInternal(sourceIndex + i));
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		destination.Changed();
	}

	protected virtual void CopyOne(int sourceIndex, TCertain destination, int destinationIndex)
	{
		var hashCode = entries[sourceIndex].hashCode;
		if (hashCode < 0)
		{
			destination.SetInternal(destinationIndex, entries[sourceIndex].item);
			if (destination is ListHashSet<T> && destinationIndex == destination._size)
				destination._size++;
		}
		else if (destination.entries[destinationIndex].hashCode < 0)
			destination.SetNull(destinationIndex);
		else if (destinationIndex == destination._size)
		{
			destination.SetInternal(destinationIndex, default!);
			destination.SetNull(destinationIndex);
		}
	}

	protected virtual void CopyToCommon(int index, T[] array, int arrayIndex, int length)
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
		buckets = default!;
		entries = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	protected override T GetInternal(int index) => entries[index].item;

	protected override int IndexOfInternal(T item, int index, int length) => item != null ? IndexOfInternal(item, index, length, Comparer.GetHashCode(item) & 0x7FFFFFFF) : throw new ArgumentNullException(nameof(item));

	protected virtual int IndexOfInternal(T item, int index, int length, int hashCode)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			return -1;
		uint collisionCount = 0;
		Debug.Assert(hashCode >= 0);
		for (var i = ~buckets[hashCode % buckets.Length]; i >= 0; i = ~entries[i].next)
		{
			if (~entries[i].next == i)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item) && i >= index && i < index + length)
				return i;
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		return -1;
	}

	protected virtual void Initialize(int capacity, out int[] buckets, out Entry[] entries)
	{
		var size = HashHelpers.GetPrime(capacity);
		buckets = new int[size];
		entries = new Entry[size];
	}

	protected abstract TCertain Insert(T? item, out int index, int hashCode);

	protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		var this2 = (TCertain)this;
		var set = CollectionCreator(collection).ExceptWith(this);
		if (CreateVar(set.GetType(), out var type).Name.Contains("FastDelHashSet") || type.Name.Contains("ParallelHashSet"))
			(type.GetMethod("FixUpFakeIndexes")
				?? throw new MissingMethodException("Не удалось загрузить метод \"починки\" фейковых индексов." +
				" Обратитесь к разработчикам .NStar.")).Invoke(set, null);
		var length = set.Length;
		if (length > 0)
		{
			if (this == collection)
				return this2;
			EnsureCapacity(_size + length);
			if (index < _size)
				CopyToInternal(index, this2, index + length, _size - index);
			set.CopyToInternal(0, this2, index, length);
		}
		return this2;
	}

	protected override TCertain InsertInternal(int index, ReadOnlySpan<T> span)
	{
		var this2 = (TCertain)this;
		var set = SpanCreator(span).ExceptWith(this);
		if (CreateVar(set.GetType(), out var type).Name.Contains("FastDelHashSet") || type.Name.Contains("ParallelHashSet"))
			(type.GetMethod("FixUpFakeIndexes")
				?? throw new MissingMethodException("Не удалось загрузить метод \"починки\" фейковых индексов." +
				" Обратитесь к разработчикам .NStar.")).Invoke(set, null);
		var length = set.Length;
		if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < _size)
				CopyToInternal(index, this2, index + length, _size - index);
			set.CopyToInternal(0, this2, index, length);
		}
		return this2;
	}

	protected virtual void RemoveAtCommon(int index, ref Entry t)
	{
		uint collisionCount = 0;
		var bucket = ~t.hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
			if (i == index)
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
					entries[last].next = entries[i].next;
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		t.hashCode = 0;
		t.item = default!;
	}

	protected virtual bool RemoveValueCommon(T? item, int hashCode, RemoveValueAction action)
	{
		uint collisionCount = 0;
		var bucket = hashCode % buckets.Length;
		var last = -1;
		for (var i = ~buckets[bucket]; i >= 0; last = i, i = ~entries[i].next)
		{
			if (~entries[i].next == i || ~entries[i].next == last && last != -1)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
			if (entries[i].hashCode == ~hashCode && Comparer.Equals(entries[i].item, item))
			{
				if (last < 0)
					buckets[bucket] = entries[i].next;
				else
					entries[last].next = entries[i].next;
				ref var t = ref entries[i];
				t.hashCode = 0;
				t.item = default!;
				action(ref t, i);
				return true;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		}
		return false;
	}

	protected delegate void RemoveValueAction(ref Entry t, int i);

	protected virtual void Resize() => Resize(HashHelpers.ExpandPrime(_size), false);

	protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		var newBuckets = new int[newSize];
		var newEntries = new Entry[newSize];
		if (entries != null)
			Array.Copy(entries, 0, newEntries, 0, Min(entries.Length, newSize));
		if (forceNewHashCodes)
			for (var i = 0; i < _size; i++)
			{
				ref var t = ref newEntries[i];
				if (t.hashCode == 0)
					continue;
				t.hashCode = ~Comparer.GetHashCode(t.item
					?? throw new InvalidOperationException("Произошла внутренняя ошибка." +
					" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
					" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")) & 0x7FFFFFFF;
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

	protected virtual void SetNull(int index)
	{
		if (this is not ListHashSet<T>)
		{
			RemoveAt(index);
			return;
		}
		ref var t = ref entries[index];
		if (t.hashCode >= 0)
			return;
		RemoveAtCommon(index, ref t);
		t.next = 0;
		Debug.Assert(entries[index].hashCode >= 0);
	}

	public override bool TryAdd(T item, out int index)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		if (TryGetIndexOf(item, out index, hashCode))
			return false;
		Insert(item, out index, hashCode);
		return true;
	}

	public override bool TryGetIndexOf(T item, out int index) => item != null ? TryGetIndexOf(item, out index, Comparer.GetHashCode(item) & 0x7FFFFFFF) : throw new ArgumentNullException(nameof(item));

	protected virtual bool TryGetIndexOf(T item, out int index, int hashCode) => (index = IndexOfInternal(item, 0, _size, hashCode)) >= 0;
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

	public ListHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public ListHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count : typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			TryAdd(item);
	}

	public ListHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public ListHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		foreach (var item in collection)
			TryAdd(item);
	}

	public ListHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public ListHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public ListHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	public ListHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public override T this[Index index, bool invoke = false]
	{
		get => base[index, invoke];
		set
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			if (entries[index2].item?.Equals(value) ?? value == null)
				return;
			if (Contains(value))
				throw new ArgumentException("Ошибка, такой элемент уже был добавлен.", nameof(value));
			SetInternal(index2, value);
		}
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex++] = entries[index++].item;
	}

	protected override TCertain Insert(T? item, out int index, int hashCode)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));
		if (buckets == null)
			Initialize(0, out buckets, out entries);
		if (buckets == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка." +
				" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
				" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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
		Changed();
		return (TCertain)this;
	}

	public override TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		_size--;
		if (index < _size)
			CopyToInternal(index + 1, this2, index, _size - index);
		SetNull(_size);
		Changed();
		return this2;
	}

	protected override void SetInternal(int index, T item)
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
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
				" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
				" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
			if (i == index)
			{
				if (last < 0)
					buckets[oldBucket] = entries[i].next;
				else
					entries[last].next = entries[i].next;
				break;
			}
			collisionCount++;
			if (collisionCount > entries.Length)
				throw new InvalidOperationException("Произошла внутренняя ошибка." +
				" Возможно, вы пытаетесь писать в одно множество в несколько потоков?" +
				" Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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

	public ListHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer) { }

	public ListHashSet(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public ListHashSet(int capacity, params T[] array) : base(capacity, array) { }

	public ListHashSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public ListHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(capacity, collection, comparer) { }

	public ListHashSet(int capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }

	public ListHashSet(params T[] array) : base(array) { }

	public ListHashSet(ReadOnlySpan<T> span) : base(span) { }

	protected override Func<int, ListHashSet<T>> CapacityCreator { get; } = x => new(x);

	protected override Func<IEnumerable<T>, ListHashSet<T>> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<T>, ListHashSet<T>> SpanCreator { get; } = x => new(x);
}
