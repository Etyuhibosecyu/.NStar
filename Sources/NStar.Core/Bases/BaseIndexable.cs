namespace NStar.Core;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseIndexable<T> : IReadOnlyList<T>, IDisposable
{
	protected int _size;
	[NonSerialized]
	private protected object _syncRoot = new();

	public virtual T this[Index index, bool invoke = true]
	{
		get
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set => throw new NotSupportedException("Это действие не поддерживается в этой коллекции."
			+ " Если оно нужно вам, используйте один из видов коллекций, наследующихся от BaseList.");
	}

	T G.IReadOnlyList<T>.this[int index] => this[index];

	public virtual int Length => _size;

	protected static bool IsCompatibleObject(object? value) => value is T || value == null && default(T) == null;

	public virtual Memory<T> AsMemory() => AsMemory(0, _size);

	public virtual Memory<T> AsMemory(Index index) => AsMemory(index.GetOffset(_size));

	public virtual Memory<T> AsMemory(int index) => AsMemory(index, _size - index);

	public abstract Memory<T> AsMemory(int index, int length);

	public virtual Memory<T> AsMemory(Range range) => AsMemory()[range];

	public virtual Span<T> AsSpan() => AsSpan(0, _size);

	public virtual Span<T> AsSpan(Index index) => AsSpan(index.GetOffset(_size));

	public virtual Span<T> AsSpan(int index) => AsSpan(index, _size - index);

	public abstract Span<T> AsSpan(int index, int length);

	public virtual Span<T> AsSpan(Range range) => AsSpan()[range];

	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, _size);

	public virtual bool Contains(IEnumerable<T> collection, int index) => Contains(collection, index, _size - index);

	public virtual bool Contains(IEnumerable<T> collection, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		if (length == 0)
			return false;
		if (!collection.Any())
			return true;
		if (collection is not G.IReadOnlyList<T> list)
		{
			if (collection is G.IList<T> list2)
				list = list2.GetSlice();
			else
				list = new List<T>(collection);
		}
		return ContainsInternal(list, index, length);
	}

	public virtual bool Contains(T? item) => Contains(item, 0, _size);

	public virtual bool Contains(T? item, int index) => Contains(item, index, _size - index);

	public virtual bool Contains(T? item, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		if (item == null)
		{
			for (var i = index; i < index + length; i++)
				if (GetInternal(i) == null)
					return true;
			return false;
		}
		else
		{
			var c = EqualityComparer<T>.Default;
			for (var i = index; i < index + length; i++)
				if (c.Equals(GetInternal(i), item))
					return true;
			return false;
		}
	}

	public virtual bool ContainsAny(IEnumerable<T> collection) => ContainsAny(collection, 0, _size);

	public virtual bool ContainsAny(IEnumerable<T> collection, int index) => ContainsAny(collection, index, _size - index);

	public virtual bool ContainsAny(IEnumerable<T> collection, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		var hs = collection.ToHashSet();
		for (var i = index; i < index + length; i++)
			if (hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, _size);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, int index) => ContainsAnyExcluding(collection, index, _size - index);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		var hs = collection.ToHashSet();
		for (var i = index; i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	protected virtual bool ContainsInternal(G.IReadOnlyList<T> list, int index, int length)
	{
		var j = 0;
		for (var i = 0; i - j <= length - list.Count; i++)
		{
			if (GetInternal(index + i)?.Equals(list[j]) ?? list[j] == null)
			{
				j++;
				if (j >= list.Count)
					return true;
			}
			else if (j != 0)
			{
				i -= j;
				j = 0;
			}
		}
		return false;
	}

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException("Массив должен иметь одно измерение.");
		try
		{
			CopyToInternal(array, arrayIndex);
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		}
	}

	public virtual void CopyTo(int index, T[] array, int arrayIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(array);
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
		if (arrayIndex + length > array.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		CopyToInternal(index, array, arrayIndex, length);
	}

	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	public virtual void CopyTo(T[] array, int arrayIndex) => CopyTo(0, array, arrayIndex, Length);

	protected virtual void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	protected abstract void CopyToInternal(int index, T[] array, int arrayIndex, int length);

	public abstract void Dispose();

	public virtual bool EndsWith(T? item) => _size > 0 && (GetInternal(_size - 1)?.Equals(item) ?? item == null);

	public virtual bool EndsWith(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		return _size >= CreateVar(collection.Length(), out var length) && EqualsInternal(collection, _size - length);
	}

	public virtual bool Equals(IEnumerable<T>? collection) => EqualsInternal(collection, 0, true);

	public virtual bool Equals(IEnumerable<T>? collection, int index, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	public override bool Equals(object? obj) => obj switch
	{
		null => false,
		IEnumerable<T> enumerable => Equals(enumerable),
		IEquatable<IEnumerable<T>> iqen => iqen.Equals(this),
		_ => false,
	};

	protected virtual bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (collection is null)
			return false;
		if (collection is G.IList<T> list && !(CreateVar(list.GetType(),
			out var type).Name.Contains("FastDelHashSet") || type.Name.Contains("ParallelHashSet")))
			return EqualsToList(list, index, toEnd);
		else
			return EqualsToNonList(collection, index, toEnd);
	}

	protected virtual bool EqualsToList(G.IList<T> list, int index, bool toEnd = false)
	{
		if (index > _size - list.Count)
			return false;
		if (toEnd && index < _size - list.Count)
			return false;
		for (var i = 0; i < list.Count; i++)
			if (!(GetInternal(index++)?.Equals(list[i]) ?? list[i] == null))
				return false;
		return true;
	}

	protected virtual bool EqualsToNonList(IEnumerable<T> collection, int index, bool toEnd = false)
	{
		if (collection.TryGetLengthEasily(out var length))
		{
			if (index > _size - length)
				return false;
			if (toEnd && index < _size - length)
				return false;
		}
		foreach (var item in collection)
			if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item == null))
				return false;
		return !toEnd || index == _size;
	}

	public virtual bool Exists(Predicate<T> match) => FindIndex(match) != -1;

	public virtual T? Find(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item))
				return item;
		}
		return default;
	}

	public virtual List<T> FindAll(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		List<T> list = [];
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item))
				list.Add(item);
		}
		return list;
	}

	public virtual int FindIndex(int startIndex, int length, Predicate<T> match)
	{
		if ((uint)startIndex > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(startIndex));
		if (length < 0 || startIndex > _size - length)
			throw new ArgumentOutOfRangeException(nameof(length));
		ArgumentNullException.ThrowIfNull(match);
		var endIndex = startIndex + length;
		for (var i = startIndex; i < endIndex; i++)
			if (match(GetInternal(i)))
				return i;
		return -1;
	}

	public virtual int FindIndex(int startIndex, Predicate<T> match) => FindIndex(startIndex, _size - startIndex, match);

	public virtual int FindIndex(Predicate<T> match) => FindIndex(0, _size, match);

	public virtual T? FindLast(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		for (var i = _size - 1; i >= 0; i--)
		{
			var item = GetInternal(i);
			if (match(item))
				return item;
		}
		return default;
	}

	public virtual int FindLastIndex(int startIndex, int length, Predicate<T> match)
	{
		if (length < 0 || startIndex - length + 1 < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		ArgumentNullException.ThrowIfNull(match);
		if (_size == 0)
			ArgumentOutOfRangeException.ThrowIfNotEqual(startIndex, -1);
		var endIndex = startIndex - length;
		for (var i = startIndex; i > endIndex; i--)
			if (match(GetInternal(i)))
				return i;
		return -1;
	}

	public virtual int FindLastIndex(int startIndex, Predicate<T> match) => FindLastIndex(startIndex, startIndex + 1, match);

	public virtual int FindLastIndex(Predicate<T> match) => FindLastIndex(_size - 1, _size, match);

	public virtual void ForEach(Action<T> action) => ForEach(action, 0, _size);

	public virtual void ForEach(Action<T> action, int index) => ForEach(action, index, _size - index);

	public virtual void ForEach(Action<T> action, int index, int length)
	{
		ArgumentNullException.ThrowIfNull(action);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		for (var i = index; i < index + length; i++)
			action(GetInternal(i));
	}

	public virtual void ForEach(Action<T, int> action) => ForEach(action, 0, _size);

	public virtual void ForEach(Action<T, int> action, int index) => ForEach(action, index, _size - index);

	public virtual void ForEach(Action<T, int> action, int index, int length)
	{
		ArgumentNullException.ThrowIfNull(action);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		for (var i = index; i < index + length; i++)
			action(GetInternal(i), i);
	}

	public virtual IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

	private Enumerator GetEnumeratorInternal() => new(this);

	public override int GetHashCode()
	{
		var hash = 486187739;
		var en = GetEnumerator();
		if (en.MoveNext())
		{
			hash = (hash * 16777619) ^ (en.Current?.GetHashCode() ?? 0);
			if (en.MoveNext())
			{
				hash = (hash * 16777619) ^ (en.Current?.GetHashCode() ?? 0);
				hash = (hash * 16777619) ^ (GetInternal(_size - 1)?.GetHashCode() ?? 0);
			}
		}
		return hash;
	}

	protected abstract T GetInternal(int index, bool invoke = true);

	public virtual Slice<T> GetSlice() => GetSlice(0, _size);

	public virtual Slice<T> GetSlice(Index index) => GetSlice(index.GetOffset(_size));

	public virtual Slice<T> GetSlice(int index) => GetSlice(index, _size - index);

	public virtual Slice<T> GetSlice(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		return GetSliceInternal(index, length);
	}

	public virtual Slice<T> GetSlice(Range range)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		var (start, length) = range.GetOffsetAndLength(_size);
		return GetSlice(start, length);
	}

	protected abstract Slice<T> GetSliceInternal(int index, int length);

	public virtual int IndexOf(IEnumerable<T> collection) => IndexOf(collection, 0, _size);

	public virtual int IndexOf(IEnumerable<T> collection, int index) => IndexOf(collection, index, _size - index);

	public virtual int IndexOf(IEnumerable<T> collection, int index, int length) => IndexOf(collection, index, length, out _);

	public virtual int IndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		if (!collection.Any())
		{
			collectionLength = 0;
			return 0;
		}
		if (_size == 0 || length == 0)
		{
			collectionLength = 0;
			return -1;
		}

		if (collection is not G.ICollection<T> c)
			c = new List<T>(collection);
		collectionLength = c.Count;
		for (var i = index; i <= index + length - collectionLength; i++)
			if (EqualsInternal(collection, i))
				return i;
		return -1;
	}

	public virtual int IndexOf(T item) => IndexOf(item, 0, _size);

	public virtual int IndexOf(T item, int index) => IndexOf(item, index, _size - index);

	public virtual int IndexOf(T item, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		return IndexOfInternal(item, index, length);
	}

	public virtual int IndexOfAny(IEnumerable<T> collection) => IndexOfAny(collection, 0, _size);

	public virtual int IndexOfAny(IEnumerable<T> collection, int index) => IndexOfAny(collection, index, _size - index);

	public virtual int IndexOfAny(IEnumerable<T> collection, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		var hs = collection.ToHashSet();
		for (var i = index; i < index + length; i++)
			if (hs.Contains(GetInternal(i)))
				return i;
		return -1;
	}

	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection) => IndexOfAnyExcluding(collection, 0, _size);

	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection, int index) => IndexOfAnyExcluding(collection, index, _size - index);

	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		var hs = collection.ToHashSet();
		for (var i = index; i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return i;
		return -1;
	}

	protected abstract int IndexOfInternal(T item, int index, int length);

	public virtual int LastIndexOf(IEnumerable<T> collection) => LastIndexOf(collection, _size - 1, _size);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index) => LastIndexOf(collection, index, index + 1);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int length) => LastIndexOf(collection, index, length, out _);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		if (_size != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _size);
		if (length > index + 1)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		if (_size == 0 || length == 0 || !collection.Any())
		{
			collectionLength = 0;
			return -1;
		}
		if (collection is not G.ICollection<T> c)
			c = new List<T>(collection);
		collectionLength = c.Count;
		var startIndex = index + 1 - length;
		for (var i = length - collectionLength; i >= 0; i--)
			if (EqualsInternal(c, startIndex + i))
				return startIndex + i;
		return -1;
	}

	public virtual int LastIndexOf(T item) => LastIndexOf(item, _size - 1, _size);

	public virtual int LastIndexOf(T item, int index) => LastIndexOf(item, index, index + 1);

	public virtual int LastIndexOf(T item, int index, int length)
	{
		if (_size != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (_size == 0)
			return -1;
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _size);
		if (length > index + 1)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		return LastIndexOfInternal(item, index, length);
	}

	public virtual int LastIndexOfAny(IEnumerable<T> collection) => LastIndexOfAny(collection, _size - 1, _size);

	public virtual int LastIndexOfAny(IEnumerable<T> collection, int index) => LastIndexOfAny(collection, index, index + 1);

	public virtual int LastIndexOfAny(IEnumerable<T> collection, int index, int length)
	{
		if (_size != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _size);
		if (length > index + 1)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		var hs = collection.ToHashSet();
		var startIndex = index + 1 - length;
		for (var i = index; i >= startIndex; i--)
			if (hs.Contains(GetInternal(i)))
				return i;
		return -1;
	}

	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection) => LastIndexOfAnyExcluding(collection, _size - 1, _size);

	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection, int index) => LastIndexOfAnyExcluding(collection, index, index + 1);

	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection, int index, int length)
	{
		if (_size != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _size);
		if (length > index + 1)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		var hs = collection.ToHashSet();
		var startIndex = index + 1 - length;
		for (var i = index; i >= startIndex; i--)
			if (!hs.Contains(GetInternal(i)))
				return i;
		return -1;
	}

	protected abstract int LastIndexOfInternal(T item, int index, int length);

	public virtual T Random() => GetInternal(random.Next(_size));

	public virtual T Random(Random randomObj) => GetInternal(randomObj.Next(_size));

	public virtual Slice<T> Skip(int length) => GetSlice(Clamp(length, 0, _size));

	public virtual Slice<T> SkipLast(int length) => GetSlice(0, Max(0, _size - Max(length, 0)));

	public virtual Slice<T> SkipWhile(Func<T, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i)); i++) ;
		return GetSlice(i);
	}

	public virtual Slice<T> SkipWhile(Func<T, int, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i), i); i++) ;
		return GetSlice(i);
	}

	public virtual bool StartsWith(T? item) => _size > 0 && (GetInternal(0)?.Equals(item) ?? item == null);

	public virtual bool StartsWith(IEnumerable<T> collection) => EqualsInternal(collection, 0, false);

	public virtual Slice<T> Take(int length) => GetSlice(0, Clamp(length, 0, _size));

	public virtual Slice<T> TakeLast(int length) => GetSlice(Max(0, _size - Max(length, 0)));

	public virtual Slice<T> TakeWhile(Func<T, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i)); i++) ;
		return GetSlice(0, i);
	}

	public virtual Slice<T> TakeWhile(Func<T, int, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i), i); i++) ;
		return GetSlice(0, i);
	}

	public virtual T[] ToArray()
	{
		var array = GC.AllocateUninitializedArray<T>(Length);
		CopyToInternal(0, array, 0, Length);
		return array;
	}

	public virtual bool TrueForAll(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		for (var i = 0; i < _size; i++)
			if (!match(GetInternal(i)))
				return false;
		return true;
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>, IEnumerator
	{
		private readonly BaseIndexable<T> collection;
		private int index;
		private T current;

		internal Enumerator(BaseIndexable<T> collection)
		{
			this.collection = collection;
			index = 0;
			current = default!;
		}

		public readonly void Dispose()
		{
		}

		public bool MoveNext()
		{
			var localCollection = collection;
			if ((uint)index < (uint)localCollection._size)
			{
				current = localCollection.GetInternal(index++);
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = collection._size + 1;
			current = default!;
			return false;
		}

		public readonly T Current => current;

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == collection._size + 1)
					throw new InvalidOperationException("Указатель находится за границей коллекции.");
				return Current!;
			}
		}

		void IEnumerator.Reset()
		{
			index = 0;
			current = default!;
		}
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseIndexable<T, TCertain> : BaseIndexable<T>, IEquatable<TCertain> where TCertain : BaseIndexable<T, TCertain>, new()
{
	public virtual TCertain this[Range range] => GetRange(range);

	public virtual int Compare(int index, TCertain other, int otherIndex) => Compare(index, other, otherIndex, Min(_size - index, other._size - otherIndex));

	public virtual int Compare(int index, TCertain other, int otherIndex, int length)
	{
		ArgumentNullException.ThrowIfNull(other);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сравниваемый диапазон выходит за текущий размер коллекции.");
		ArgumentOutOfRangeException.ThrowIfNegative(otherIndex);
		if (otherIndex + length > other._size)
			throw new ArgumentException("Сравниваемый диапазон выходит за размер экстра-коллекции.");
		return CompareInternal(index, other, otherIndex, length);
	}

	public virtual int Compare(TCertain other) => Compare(0, other, 0, Min(_size, other._size));

	public virtual int Compare(TCertain other, int length) => Compare(0, other, 0, length);

	protected virtual int CompareInternal(int index, TCertain other, int otherIndex, int length)
	{
		for (var i = 0; i < length; i++)
			if (!(GetInternal(index + i)?.Equals(other.GetInternal(otherIndex + i)) ?? other.GetInternal(otherIndex + i) == null))
				return i;
		return length;
	}

	public virtual bool Contains(TCertain collection) => Contains((IEnumerable<T>)collection, 0, _size);

	public virtual bool Contains(TCertain collection, int index) => Contains((IEnumerable<T>)collection, index, _size - index);

	public virtual bool Contains(TCertain collection, int index, int length) => Contains((IEnumerable<T>)collection, index, length);

	public virtual bool ContainsAny(TCertain collection) => ContainsAny((IEnumerable<T>)collection, 0, _size);

	public virtual bool ContainsAny(TCertain collection, int index) => ContainsAny((IEnumerable<T>)collection, index, _size - index);

	public virtual bool ContainsAny(TCertain collection, int index, int length) => ContainsAny((IEnumerable<T>)collection, index, length);

	public virtual bool ContainsAnyExcluding(TCertain collection) => ContainsAnyExcluding((IEnumerable<T>)collection, 0, _size);

	public virtual bool ContainsAnyExcluding(TCertain collection, int index) => ContainsAnyExcluding((IEnumerable<T>)collection, index, _size - index);

	public virtual bool ContainsAnyExcluding(TCertain collection, int index, int length) => ContainsAnyExcluding((IEnumerable<T>)collection, index, length);

	public virtual bool EndsWith(TCertain collection) => EndsWith((IEnumerable<T>)collection);

	public virtual bool Equals(TCertain? collection) => EqualsInternal(collection, 0, true);

	public virtual bool Equals(TCertain? collection, int index, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	public virtual new TCertain ForEach(Action<T> action) => ForEach(action, 0, _size);

	public virtual new TCertain ForEach(Action<T> action, int index) => ForEach(action, index, _size - index);

	public virtual new TCertain ForEach(Action<T> action, int index, int length)
	{
		base.ForEach(action, index, length);
		return (TCertain)this;
	}

	public virtual new TCertain ForEach(Action<T, int> action) => ForEach(action, 0, _size);

	public virtual new TCertain ForEach(Action<T, int> action, int index) => ForEach(action, index, _size - index);

	public virtual new TCertain ForEach(Action<T, int> action, int index, int length)
	{
		base.ForEach(action, index, length);
		return (TCertain)this;
	}

	public virtual TCertain GetAfter(IEnumerable<T> collection) => GetAfter(collection, 0, _size);

	public virtual TCertain GetAfter(IEnumerable<T> collection, int index) => GetAfter(collection, index, _size - index);

	public virtual TCertain GetAfter(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length, out var collectionLength);
		return foundIndex == -1 ? GetRange(0, 0) : GetRange(foundIndex + collectionLength);
	}

	public virtual TCertain GetAfter(TCertain collection) => GetAfter((IEnumerable<T>)collection, 0, _size);

	public virtual TCertain GetAfter(TCertain collection, int index) => GetAfter((IEnumerable<T>)collection, index, _size - index);

	public virtual TCertain GetAfter(TCertain collection, int index, int length) => GetAfter((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection) => GetAfterLast(collection, _size - 1, _size);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index) => GetAfterLast(collection, index, index + 1);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length, out var collectionLength);
		return foundIndex == -1 ? (TCertain)this : GetRange(foundIndex + collectionLength);
	}

	public virtual TCertain GetAfterLast(TCertain collection) => GetAfterLast((IEnumerable<T>)collection, _size - 1, _size);

	public virtual TCertain GetAfterLast(TCertain collection, int index) => GetAfterLast((IEnumerable<T>)collection, index, index + 1);

	public virtual TCertain GetAfterLast(TCertain collection, int index, int length) => GetAfterLast((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetBefore(IEnumerable<T> collection) => GetBefore(collection, 0, _size);

	public virtual TCertain GetBefore(IEnumerable<T> collection, int index) => GetBefore(collection, index, _size - index);

	public virtual TCertain GetBefore(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length);
		return foundIndex == -1 ? (TCertain)this : GetRange(0, foundIndex);
	}

	public virtual TCertain GetBefore(TCertain collection) => GetBefore((IEnumerable<T>)collection, 0, _size);

	public virtual TCertain GetBefore(TCertain collection, int index) => GetBefore((IEnumerable<T>)collection, index, _size - index);

	public virtual TCertain GetBefore(TCertain collection, int index, int length) => GetBefore((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection) => GetBeforeLast(collection, _size - 1, _size);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index) => GetBeforeLast(collection, index, index + 1);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length);
		return foundIndex == -1 ? GetRange(0, 0) : GetRange(0, foundIndex);
	}

	public virtual TCertain GetBeforeLast(TCertain collection) => GetBeforeLast((IEnumerable<T>)collection, _size - 1, _size);

	public virtual TCertain GetBeforeLast(TCertain collection, int index) => GetBeforeLast((IEnumerable<T>)collection, index, index + 1);

	public virtual TCertain GetBeforeLast(TCertain collection, int index, int length) => GetBeforeLast((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetRange(int index, bool alwaysCopy = false) => GetRange(index, _size - index, alwaysCopy);

	public virtual TCertain GetRange(int index, int length, bool alwaysCopy = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		else if (!alwaysCopy && index == 0 && length == _size && this is TCertain thisList)
			return thisList;
		return GetRangeInternal(index, length);
	}

	public virtual TCertain GetRange(Range range, bool alwaysCopy = false)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		var (start, length) = range.GetOffsetAndLength(_size);
		return GetRange(start, length, alwaysCopy);
	}

	protected abstract TCertain GetRangeInternal(int index, int length);

	public virtual int IndexOf(TCertain collection) => IndexOf((IEnumerable<T>)collection, 0, _size);

	public virtual int IndexOf(TCertain collection, int index) => IndexOf((IEnumerable<T>)collection, index, _size - index);

	public virtual int IndexOf(TCertain collection, int index, int length) => IndexOf((IEnumerable<T>)collection, index, length);

	public virtual int IndexOf(TCertain collection, int index, int length, out int collectionLength) => IndexOf((IEnumerable<T>)collection, index, length, out collectionLength);

	public virtual int IndexOfAny(TCertain collection) => IndexOfAny((IEnumerable<T>)collection, 0, _size);

	public virtual int IndexOfAny(TCertain collection, int index) => IndexOfAny((IEnumerable<T>)collection, index, _size - index);

	public virtual int IndexOfAny(TCertain collection, int index, int length) => IndexOfAny((IEnumerable<T>)collection, index, length);

	public virtual int IndexOfAnyExcluding(TCertain collection) => IndexOfAnyExcluding((IEnumerable<T>)collection, 0, _size);

	public virtual int IndexOfAnyExcluding(TCertain collection, int index) => IndexOfAnyExcluding((IEnumerable<T>)collection, index, _size - index);

	public virtual int IndexOfAnyExcluding(TCertain collection, int index, int length) => IndexOfAnyExcluding((IEnumerable<T>)collection, index, length);

	public virtual int LastIndexOf(TCertain collection) => LastIndexOf((IEnumerable<T>)collection, _size - 1, _size);

	public virtual int LastIndexOf(TCertain collection, int index) => LastIndexOf((IEnumerable<T>)collection, index, index + 1);

	public virtual int LastIndexOf(TCertain collection, int index, int length) => LastIndexOf((IEnumerable<T>)collection, index, length);

	public virtual int LastIndexOf(TCertain collection, int index, int length, out int collectionLength) => LastIndexOf((IEnumerable<T>)collection, index, length, out collectionLength);

	public virtual int LastIndexOfAny(TCertain collection) => LastIndexOfAny((IEnumerable<T>)collection, _size - 1, _size);

	public virtual int LastIndexOfAny(TCertain collection, int index) => LastIndexOfAny((IEnumerable<T>)collection, index, index + 1);

	public virtual int LastIndexOfAny(TCertain collection, int index, int length) => LastIndexOfAny((IEnumerable<T>)collection, index, length);

	public virtual int LastIndexOfAnyExcluding(TCertain collection) => LastIndexOfAnyExcluding((IEnumerable<T>)collection, _size - 1, _size);

	public virtual int LastIndexOfAnyExcluding(TCertain collection, int index) => LastIndexOfAnyExcluding((IEnumerable<T>)collection, index, index + 1);

	public virtual int LastIndexOfAnyExcluding(TCertain collection, int index, int length) => LastIndexOfAnyExcluding((IEnumerable<T>)collection, index, length);

	public virtual bool StartsWith(TCertain collection) => StartsWith((IEnumerable<T>)collection);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseMutableIndexable<T, TCertain> : BaseIndexable<T, TCertain> where TCertain : BaseMutableIndexable<T, TCertain>, new()
{
	public override T this[Index index, bool invoke = true]
	{
		get => base[index, invoke];
		set
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			SetInternal(index2, value);
		}
	}

	public virtual TCertain SetAll(T value) => SetAll(value, 0, _size);

	public virtual TCertain SetAll(T value, Index index) => SetAll(value, index.GetOffset(_size));

	public virtual TCertain SetAll(T value, int index) => SetAll(value, index, _size - index);

	public virtual TCertain SetAll(T value, int index, int length)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Устанавливаемый диапазон выходит за текущий размер коллекции.");
		SetAllInternal(value, index, length);
		return (TCertain)this;
	}

	protected virtual void SetAllInternal(T value, int index, int length)
	{
		var endIndex = index + length - 1;
		for (var i = index; i <= endIndex; i++)
			SetInternal(i, value);
	}

	public virtual TCertain SetAll(T value, Range range) => SetAll(value, CreateVar(range.GetOffsetAndLength(_size), out var range2).Offset, range2.Length);

	protected abstract void SetInternal(int index, T value);
}
