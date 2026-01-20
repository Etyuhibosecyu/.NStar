namespace NStar.BigCollections;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseBigList<T, TCertain, TLow> : IBigList<T>, ICloneable, IDisposable
	where TCertain : BaseBigList<T, TCertain, TLow>, new() where TLow : G.IList<T>, new()
{
	public virtual T this[MpzT index]
	{
		get
		{
			if (index >= Length)
				throw new IndexOutOfRangeException();
			return GetInternal(index);
		}
		set
		{
			if (index >= Length)
				throw new IndexOutOfRangeException();
			SetInternal(index, value);
		}
	}

	public abstract MpzT Capacity { get; set; }

	public virtual MpzT Length { get; private protected set; } = 0;

	protected abstract Func<MpzT, TCertain> CapacityCreator { get; }

	protected abstract Func<int, TLow> CapacityLowCreator { get; }

	protected abstract Func<G.IEnumerable<T>, TCertain> CollectionCreator { get; }

	protected abstract Func<G.IEnumerable<T>, TLow> CollectionLowCreator { get; }

	protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public abstract TCertain Add(T item);

	void IBigCollection<T>.Add(T item) => Add(item);

	public virtual TCertain AddRange(G.IEnumerable<T> collection) => Insert(Length, collection);

	private protected void Changed() => ListChanged?.Invoke((TCertain)this);

	public virtual void Clear()
	{
		if (Length > 0)
			ClearInternal(true);
	}

	public virtual void Clear(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Очищаемый диапазон выходит за текущий размер коллекции.");
		ClearInternal(index, length);
	}

	protected abstract void ClearInternal(bool verify);

	protected abstract void ClearInternal(MpzT index, MpzT length);

	public virtual bool Contains(T item) => Contains(item, 0, Length);

	public virtual bool Contains(T item, MpzT index) => Contains(item, index, Length - index);

	public virtual bool Contains(T item, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
#if !VERIFY
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
#endif
		return IndexOfInternal(item, index, length, false) >= 0;
	}

	public virtual bool Contains(G.IEnumerable<T> collection) => Contains(collection, 0, Length);

	public virtual bool Contains(G.IEnumerable<T> collection, MpzT index) => Contains(collection, index, Length - index);

	public virtual bool Contains(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		return ContainsInternal(collection, index, length);
	}

	public virtual bool Contains(TCertain list) => Contains((G.IEnumerable<T>)list, 0, Length);

	public virtual bool Contains(TCertain list, MpzT index) => Contains((G.IEnumerable<T>)list, index, Length - index);

	public virtual bool Contains(TCertain list, MpzT index, MpzT length) => Contains((G.IEnumerable<T>)list, index, length);

	public virtual bool ContainsAny(G.IEnumerable<T> collection) => ContainsAny(collection, 0, Length);

	public virtual bool ContainsAny(G.IEnumerable<T> collection, MpzT index) => ContainsAny(collection, index, Length - index);

	public virtual bool ContainsAny(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		var hs = collection.ToHashSet();
		for (MpzT i = new(index); i < index + length; i++)
			if (hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAny(TCertain list) => ContainsAny((G.IEnumerable<T>)list, 0, Length);

	public virtual bool ContainsAny(TCertain list, MpzT index) => ContainsAny((G.IEnumerable<T>)list, index, Length - index);

	public virtual bool ContainsAny(TCertain list, MpzT index, MpzT length) =>
		ContainsAny((G.IEnumerable<T>)list, index, length);

	public virtual bool ContainsAnyExcluding(G.IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, Length);

	public virtual bool ContainsAnyExcluding(G.IEnumerable<T> collection, MpzT index) =>
		ContainsAnyExcluding(collection, index, Length - index);

	public virtual bool ContainsAnyExcluding(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		var hs = collection.ToHashSet();
		for (MpzT i = new(index); i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAnyExcluding(TCertain list) => ContainsAnyExcluding((G.IEnumerable<T>)list, 0, Length);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index) =>
		ContainsAnyExcluding((G.IEnumerable<T>)list, index, Length - index);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index, MpzT length) =>
		ContainsAnyExcluding((G.IEnumerable<T>)list, index, length);

	protected virtual bool ContainsInternal(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		if (length == 0 || !collection.Any())
			return false;
		if (collection is not IBigList<T> list)
			list = CollectionCreator(collection);
		MpzT j = 0;
		for (MpzT i = 0; i - j <= length - list.Length; i++)
		{
			if (GetInternal(index + i)?.Equals(list[j]) ?? list[j] == null)
			{
				j++;
				if (j >= list.Length)
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

	public virtual object Clone() => Copy();

	/// <summary>
	/// Клонирует данный список. В отличие от метода Clone(), возвращает сразу нужный тип, а не <see cref="object"/>.
	/// </summary>
	/// <returns>Клон данного списка (неглубокий).</returns>
	public virtual TCertain Copy() => CollectionCreator(this);

	protected abstract void CopyToInternal(MpzT sourceIndex, TCertain destination,
		MpzT destinationIndex, MpzT length);

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

	public virtual void CopyTo(IBigList<T> list) => CopyTo(list, 0);

	public virtual void CopyTo(IBigList<T> list, MpzT listIndex) => CopyTo(0, list, listIndex, Length);

	/// <summary>
	/// Копирует диапазон из этого большого списка в другой такого же типа. Этот метод быстрее, чем SetRange()
	/// и ReplaceRange(), но медленнее, чем CopyTo() со вторым параметров того же типа, что и данный список.
	/// </summary>
	/// <param name="index">Индекс начала диапазона в данном списке.</param>
	/// <param name="list">Целевой список для копирования диапазона.</param>
	/// <param name="listIndex">Индекс начала диапазона в целевом списке.</param>
	/// <param name="length">Длина копируемого диапазона.</param>
	/// <exception cref="ArgumentException"></exception>
	public virtual void CopyTo(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		if (listIndex + length > list.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевой коллекции.");
		CopyToInternal(index, list, listIndex, length);
	}

	/// <summary>
	/// Копирует диапазон из этого большого списка в другой такого же типа. Этот метод быстрее, чем SetRange(),
	/// ReplaceRange() и CopyTo() со вторым параметров типа <see cref="IBigList{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона в данном списке.</param>
	/// <param name="list">Целевой список для копирования диапазона.</param>
	/// <param name="listIndex">Индекс начала диапазона в целевом списке.</param>
	/// <param name="length">Длина копируемого диапазона.</param>
	/// <exception cref="ArgumentException"></exception>
	public virtual void CopyTo(MpzT index, TCertain list, MpzT listIndex, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		if (listIndex + length > list.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевой коллекции.");
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(MpzT index, T[] array, int arrayIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		if (arrayIndex + length > array.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		CopyToInternal(index, array, arrayIndex, length);
	}

	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	public virtual void CopyTo(T[] array, int arrayIndex)
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		CopyTo(0, array, arrayIndex, (int)Length);
	}

	protected virtual void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		CopyToInternal(0, array2, arrayIndex, (int)Length);
	}

	protected abstract void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length);

	protected abstract void CopyToInternal(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length);

	public abstract void Dispose();

	protected virtual void EnsureCapacity(MpzT min)
	{
		if (Capacity < min)
		{
			var newCapacity = Length == 0 ? DefaultCapacity : Length * 2;
			if (newCapacity < min)
				newCapacity = min;
			Capacity = newCapacity;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EqualItems(MpzT index, T listItem) => GetInternal(index)?.Equals(listItem) ?? listItem == null;

	public virtual bool Equals(G.IEnumerable<T>? collection) =>
		ReferenceEquals(this, collection) || EqualsInternal(collection, 0, true);

	public virtual bool Equals(G.IEnumerable<T>? collection, MpzT index, bool toEnd = false) =>
		EqualsInternal(collection, index, toEnd);

	public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj switch
	{
		null => false,
		G.IEnumerable<T> enumerable => Equals(enumerable),
		IEquatable<G.IEnumerable<T>> iqen => iqen.Equals(this),
		_ => false,
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EqualsInternal(G.IEnumerable<T>? collection, MpzT index, bool toEnd = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is TCertain bigList)
			return EqualsToBigList(bigList, index, toEnd);
		else if (collection is G.IReadOnlyList<T> list && !(CreateVar(list.GetType(),
			out var type).Name.Contains("FastDelHashSet") || type.Name.Contains("ParallelHashSet")))
			return EqualsToList(list, index, toEnd);
		else if (collection is G.IList<T> list2)
			return EqualsToList(list2.GetSlice(), index, toEnd);
		else
			return EqualsToNonList(collection, index, toEnd);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EqualsToBigList(TCertain list, MpzT index, bool toEnd = false)
	{
		if (index > Length - list.Length)
			return false;
		if (toEnd && index < Length - list.Length)
			return false;
		for (var i = 0; i < list.Length; i++)
			if (!EqualItems(index + i, list.GetInternal(i)))
				return false;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EqualsToList(G.IReadOnlyList<T> list, MpzT index, bool toEnd = false)
	{
		if (index > Length - list.Count)
			return false;
		if (toEnd && index < Length - list.Count)
			return false;
		for (var i = 0; i < list.Count; i++)
			if (!EqualItems(index + i, list[i]))
				return false;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EqualsToNonList(G.IEnumerable<T> collection, MpzT index, bool toEnd = false)
	{
		if (collection.TryGetLengthEasily(out var length))
		{
			if (index > Length - length)
				return false;
			if (toEnd && index < Length - length)
				return false;
		}
		foreach (var item in collection)
		{
			if (index >= Length || !EqualItems(index, item))
				return false;
			index++;
		}
		return !toEnd || index == Length;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<T> G.IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public override int GetHashCode()
	{
		var hash = 486187739;
		foreach (var item in this)
			hash = (hash * 16777619) ^ (item?.GetHashCode() ?? 0);
		return hash;
	}

	protected abstract T GetInternal(MpzT index, bool invoke = true);

	public virtual TCertain GetRange(MpzT index, bool alwaysCopy = false) => GetRange(index, Length - index, alwaysCopy);

	public virtual TCertain GetRange(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		return GetRangeInternal(index, length, alwaysCopy);
	}

	protected virtual void GetRangeCopyTo(MpzT index, MpzT length, TCertain list) => CopyToInternal(index, list, 0, length);

	protected virtual TCertain GetRangeInternal(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		if (length == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && length == Length && this is TCertain thisList)
			return thisList;
		var list = CapacityCreator(length);
		GetRangeCopyTo(index, length, list);
		list.Length = length;
#if VERIFY
		Verify();
		list.Verify();
#endif
		return list;
	}

	public virtual MpzT IndexOf(T item) => IndexOf(item, 0, Length);

	public virtual MpzT IndexOf(T item, MpzT index) => IndexOf(item, index, Length - index);

	public virtual MpzT IndexOf(T item, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
#if !VERIFY
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
#endif
		return IndexOfInternal(item, index, length, false);
	}

	protected virtual MpzT IndexOfInternal(T item, MpzT index, MpzT length, bool fromEnd)
	{
		if (fromEnd)
		{
			var endIndex = index - length + 1;
			for (MpzT i = new(index); i >= endIndex; i--)
				if (GetInternal(i)?.Equals(item) ?? false)
					return i;
		}
		else
			for (MpzT i = new(index); i < index + length; i++)
				if (GetInternal(i)?.Equals(item) ?? false)
					return i;
		return -1;
	}

	public virtual TCertain Insert(Index index, T item) => Insert(index.GetOffset(Length), item);

	public virtual TCertain Insert(int index, T item) => Insert((MpzT)index, item);

	public virtual TCertain Insert(MpzT index, T item)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Length);
		InsertInternal(index, item);
		return (TCertain)this;
	}

	void IBigList<T>.Insert(MpzT index, T item) => Insert(index, item);

	/// <summary>
	/// Этот метод называется именно так, а не InsertRange().
	/// </summary>
	public virtual TCertain Insert(Index index, G.IEnumerable<T> collection) => Insert(index.GetOffset(Length), collection);

	/// <summary>
	/// Этот метод называется именно так, а не InsertRange().
	/// </summary>
	public virtual TCertain Insert(int index, G.IEnumerable<T> collection) => Insert((MpzT)index, collection);

	/// <summary>
	/// Этот метод называется именно так, а не InsertRange().
	/// </summary>
	public virtual TCertain Insert(MpzT index, G.IEnumerable<T> collection)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Length);
#if VERIFY
		MpzT oldLength = new(Length);
		MpzT oldBigListLength;
#endif
		if (collection is TCertain bigList)
		{
#if VERIFY
			oldBigListLength = bigList.Length;
#endif
			InsertInternal(index, bigList, true);
		}
		else
		{
			bigList = CollectionCreator(collection);
#if VERIFY
			oldBigListLength = new(bigList.Length);
#endif
			InsertInternal(index, bigList, false);
		}
#if VERIFY
		Debug.Assert(Length == oldLength + oldBigListLength);
		Verify();
#endif
		return (TCertain)this;
	}

	protected abstract void InsertInternal(MpzT index, T item);

	protected abstract void InsertInternal(MpzT index, TCertain bigList, bool saveOriginal);

	public virtual MpzT LastIndexOf(T item) => LastIndexOf(item, Length - 1, Length);

	public virtual MpzT LastIndexOf(T item, MpzT index) => LastIndexOf(item, index, index + 1);

	public virtual MpzT LastIndexOf(T item, MpzT index, MpzT length)
	{
		if (Length != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (Length != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (Length == 0)
			return -1;
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(length, index + 1);
#if !VERIFY
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
#endif
		return IndexOfInternal(item, index - length + 1, length, true);
	}

	[Obsolete("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо использовать Remove()"
		+ " с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().", true)]
	public virtual TCertain Remove(MpzT index) =>
		throw new NotSupportedException("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо"
			+ " использовать Remove() с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().");

	public virtual TCertain Remove(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveInternal(index, length);
		return (TCertain)this;
	}

	protected abstract void RemoveInternal(MpzT index, MpzT length);

	public virtual TCertain RemoveAt(MpzT index)
	{
		if ((uint)index >= (uint)Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		RemoveAtInternal(index);
		return (TCertain)this;
	}

	void IBigList<T>.RemoveAt(MpzT index) => RemoveAt(index);

	protected virtual void RemoveAtInternal(MpzT index)
	{
		var this2 = (TCertain)this;
		Length -= 1;
		if (index < Length)
			CopyToInternal(index + 1, this2, index, Length - index);
		SetInternal(Length, default!);
#if VERIFY
		Verify();
#endif
	}

	/// <summary>
	/// Удаляет все элементы начиная с <paramref name="index"/> и до конца списка.
	/// </summary>
	/// <param name="index">Индекс, с которого начинается удаление.</param>
	/// <returns>Ссылка на себя.</returns>
	/// <exception cref="ArgumentException"></exception>
	public virtual TCertain RemoveEnd(MpzT index)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (index > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveEndInternal(index);
		return (TCertain)this;
	}

	protected abstract void RemoveEndInternal(MpzT index);

	/// <summary>
	/// Удаляет элемент по его значению, а не по индексу. Возвращает флаг, был ли такой элемент найден и удален.
	/// </summary>
	/// <param name="item">Элемент для удаления (ЗНАЧЕНИЕ, а не индекс!).</param>
	/// <returns>Был ли элемент найден и удален.</returns>
	public virtual bool RemoveValue(T item)
	{
		var index = IndexOfInternal(item, 0, Length, false);
		if (index >= 0)
		{
			RemoveAtInternal(index);
			return true;
		}
#if VERIFY
		Verify();
#endif
		return false;
	}

	/// <summary>
	/// Целиком заменяет содержимое списка на копию указанной коллекции (неглубокое копирование).
	/// </summary>
	/// <param name="collection">Коллекция для копирования.</param>
	/// <returns>Ссылка на себя.</returns>
	public virtual TCertain Replace(G.IEnumerable<T> collection)
	{
		if (collection is IBigCollection<T> col)
		{
			col.CopyTo(this, 0);
			Length = col.Length;
		}
		else
		{
			MpzT i = 0;
			foreach (var item in collection)
			{
				SetInternal(i, item);
				i++;
			}
			Length = i;
		}
#if VERIFY
		Verify();
#endif
		return (TCertain)this;
	}

	public virtual TCertain Resize(MpzT newSize)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(newSize);
		if (newSize == Length)
			return (TCertain)this;
		ResizeInternal(newSize);
		return (TCertain)this;
	}

	protected abstract void ResizeInternal(MpzT newSize);

	public virtual TCertain ResizeLeft(MpzT newSize)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(newSize);
		if (newSize == Length)
			return (TCertain)this;
		ResizeLeftInternal(newSize);
		return (TCertain)this;
	}

	protected abstract void ResizeLeftInternal(MpzT newSize);

	public abstract TCertain Reverse();

	public virtual TCertain Reverse(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Переворачиваемый диапазон выходит за текущий размер коллекции.");
		ReverseInternal(index, length);
		return (TCertain)this;
	}

	protected abstract void ReverseInternal(MpzT index, MpzT length);

	protected abstract void SetInternal(MpzT index, T value);

	public virtual TCertain SetOrAdd(MpzT index, T value)
	{
		if ((uint)index > (uint)Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		SetOrAddInternal(index, value);
		return (TCertain)this;
	}

	protected abstract void SetOrAddInternal(MpzT index, T value);

	/// <summary>
	/// Заменяет диапазон, начиная с указанного индекса, на копию указанной коллекции (неглубокое копирование).
	/// </summary>
	/// <param name="index">Индекс, с которого начинается заменяемый диапазон.</param>
	/// <param name="collection">Диапазон начиная с <paramref name="index"/> становится копией этой коллекции.</param>
	/// <returns>Ссылка на себя.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="ArgumentException"></exception>
	public virtual TCertain SetRange(MpzT index, G.IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (index < 0 || index > Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		return SetRangeInternal(index, bigList);
	}

	protected virtual TCertain SetRangeInternal(MpzT index, TCertain bigList)
	{
		var length = bigList.Length;
		if (length == 0)
			return (TCertain)this;
		EnsureCapacity(index + length);
		var this2 = (TCertain)this;
		if (length > 0)
			bigList.CopyToInternal(0, this2, index, length);
#if VERIFY
		Verify();
#endif
		return this2;
	}

	public virtual T[] ToArray()
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		var length = (int)Length;
		var array = GC.AllocateUninitializedArray<T>(length);
		CopyToInternal(0, array, 0, length);
		return array;
	}

	public virtual TCertain TrimExcess()
	{
		var threshold = Capacity * 9 / 10;
		if (Length < threshold)
			Capacity = Length;
#if VERIFY
		Verify();
#endif
		return (TCertain)this;
	}
#if VERIFY

	protected abstract void Verify();

	protected abstract void VerifySingle();
#endif

	[Serializable]
	public struct Enumerator : G.IEnumerator<T>, IEnumerator
	{
		private readonly BaseBigList<T, TCertain, TLow> list;
		private MpzT index;
		private T current;

		internal Enumerator(BaseBigList<T, TCertain, TLow> list)
		{
			this.list = list;
			index = 0;
			current = default!;
		}

		public readonly void Dispose() => GC.SuppressFinalize(this);

		public bool MoveNext()
		{
			if (index >= list.Length)
				return MoveNextRare();
			try
			{
				current = list.GetInternal(index);
				index++;
				return true;
			}
			catch
			{
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = list.Length + 1;
			current = default!;
			return false;
		}

		public readonly T Current => current;

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list.Length + 1)
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
