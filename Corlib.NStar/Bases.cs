﻿namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseList<T, TCertain> : BaseIndexable<T, TCertain>, ICloneable, IList<T>, IList where TCertain : BaseList<T, TCertain>, new()
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

	T G.IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

	object? System.Collections.IList.this[int index]
	{
		get => this[index];
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			try
			{
				this[index] = (T)value;
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException("Ошибка, такой элемент не подходит для этой коллекции.", nameof(value));
			}
		}
	}

	public abstract int Capacity { get; set; }

	protected abstract Func<int, TCertain> CapacityCreator { get; }

	protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	protected virtual int DefaultCapacity => 32;

	bool System.Collections.IList.IsFixedSize => false;

	bool G.ICollection<T>.IsReadOnly => false;

	bool System.Collections.IList.IsReadOnly => false;

	bool System.Collections.ICollection.IsSynchronized => false;

	protected abstract Func<ReadOnlySpan<T>, TCertain> SpanCreator { get; }

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public virtual TCertain Add(T item)
	{
		if (_size == Capacity)
			EnsureCapacity(_size + 1);
		SetInternal(_size++, item);
		return (TCertain)this;
	}

	void G.ICollection<T>.Add(T item) => Add(item);

	int System.Collections.IList.Add(object? item)
	{
		try
		{
			if (item != null)
				Add((T)item);
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException("Ошибка, такой элемент не подходит для этой коллекции.", nameof(item));
		}
		return _size - 1;
	}

	public virtual TCertain AddRange(IEnumerable<T> collection) => Insert(_size, collection);

	public virtual TCertain AddRange(ReadOnlySpan<T> span) => Insert(_size, span);

	public virtual TCertain AddRange(T[] array) => AddRange(array.AsSpan());

	public virtual TCertain AddRange(List<T> list) => Insert(_size, (IEnumerable<T>)list);

	public virtual TCertain AddSeries(Func<int, T> function, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(length));
		for (var i = 0; i < length; i++)
			Add(function(i));
		return (TCertain)this;
	}

	public virtual TCertain AddSeries(int length, Func<int, T> function) => AddSeries(function, length);

	public virtual TCertain AddSeries(T item, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(length));
		AddSeriesInternal(item, length);
		return (TCertain)this;
	}

	protected virtual void AddSeriesInternal(T item, int length)
	{
		EnsureCapacity(_size + length);
		SetAllInternal(item, _size, length);
		_size += length;
	}

	public virtual TCertain Append(T item) => CollectionCreator(this).Add(item);

	public virtual (TCertain, TCertain) BreakFilter(Func<T, bool> match) => (BreakFilter(match, out var result2), result2);

	public virtual TCertain BreakFilter(Func<T, bool> match, out TCertain result2)
	{
		var result = CapacityCreator(_size / 2);
		result2 = CapacityCreator(_size / 2);
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item))
				result.Add(item);
			else
				result2.Add(item);
		}
		if (result._size < _size * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual (TCertain, TCertain) BreakFilter(Func<T, int, bool> match) => (BreakFilter(match, out var result2), result2);

	public virtual TCertain BreakFilter(Func<T, int, bool> match, out TCertain result2)
	{
		var result = CapacityCreator(_size / 2);
		result2 = CapacityCreator(_size / 2);
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item, i))
				result.Add(item);
			else
				result2.Add(item);
		}
		if (result._size < _size * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual (TCertain, TCertain) BreakFilterInPlace(Func<T, bool> match) => (BreakFilterInPlace(match, out var result2), result2);

	public virtual TCertain BreakFilterInPlace(Func<T, bool> match, out TCertain result2)
	{
		result2 = CapacityCreator(_size / 2);
		var targetIndex = 0;
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item))
			{
				if (i != targetIndex)
					SetInternal(targetIndex, item);
				targetIndex++;
			}
			else
				result2.Add(item);
		}
		_size = targetIndex;
		return (TCertain)this;
	}

	public virtual (TCertain, TCertain) BreakFilterInPlace(Func<T, int, bool> match) => (BreakFilterInPlace(match, out var result2), result2);

	public virtual TCertain BreakFilterInPlace(Func<T, int, bool> match, out TCertain result2)
	{
		result2 = CapacityCreator(_size / 2);
		var targetIndex = 0;
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item, i))
			{
				if (i != targetIndex)
					SetInternal(targetIndex, item);
				targetIndex++;
			}
			else
				result2.Add(item);
		}
		_size = targetIndex;
		return (TCertain)this;
	}

	protected void Changed() => ListChanged?.Invoke((TCertain)this);

	public virtual void Clear() => Clear(true);

	public virtual void Clear(bool full)
	{
		if (full && _size > 0)
			ClearInternal();
		_size = 0;
	}

	public virtual void Clear(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Очищаемый диапазон выходит за текущий размер коллекции.");
		ClearInternal(index, length);
	}

	protected virtual void ClearInternal() => ClearInternal(0, _size);

	protected abstract void ClearInternal(int index, int length);

	public virtual object Clone() => Copy();

	public virtual TCertain Concat(IEnumerable<T> collection) => CollectionCreator(this).AddRange(collection);

	bool System.Collections.IList.Contains(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return Contains((T)item);
		return false;
	}

	public virtual TCertain Copy() => CollectionCreator(this);

	public virtual void CopyRangeTo(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(sourceIndex);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (sourceIndex + length > _size)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(destination);
		ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);
		if (destinationIndex + length > destination.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевой коллекции.");
		if (this == destination && sourceIndex == destinationIndex)
			return;
		CopyToInternal(sourceIndex, destination, destinationIndex, length);
	}

	protected abstract void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length);

	protected virtual void EnsureCapacity(int min)
	{
		if (Capacity < min)
		{
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			Capacity = newCapacity;
		}
	}

	public virtual TCertain FillInPlace(Func<int, T> function, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(length));
		EnsureCapacity(length);
		for (var i = 0; i < length; i++)
			SetInternal(i, function(i));
		if (_size > length)
			ClearInternal(length, _size - length);
		_size = length;
		return (TCertain)this;
	}

	public virtual TCertain FillInPlace(int length, Func<int, T> function) => FillInPlace(function, length);

	public virtual TCertain FillInPlace(T item, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(length));
		EnsureCapacity(length);
		SetAllInternal(item, 0, length);
		if (_size > length)
			ClearInternal(length, _size - length);
		_size = length;
		return (TCertain)this;
	}

	public virtual TCertain Filter(Func<T, bool> match)
	{
		var result = CapacityCreator(_size / 2);
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item))
				result.Add(item);
		}
		if (result._size < _size * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual TCertain Filter(Func<T, int, bool> match)
	{
		var result = CapacityCreator(_size / 2);
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item, i))
				result.Add(item);
		}
		if (result._size < _size * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual TCertain FilterInPlace(Func<T, bool> match)
	{
		var targetIndex = 0;
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item) && i != targetIndex++)
				SetInternal(targetIndex - 1, item);
		}
		Clear(targetIndex, _size - targetIndex);
		_size = targetIndex;
		return (TCertain)this;
	}

	public virtual TCertain FilterInPlace(Func<T, int, bool> match)
	{
		var targetIndex = 0;
		for (var i = 0; i < _size; i++)
		{
			var item = GetInternal(i);
			if (match(item, i) && i != targetIndex++)
				SetInternal(targetIndex - 1, item);
		}
		Clear(targetIndex, _size - targetIndex);
		_size = targetIndex;
		return (TCertain)this;
	}

	public virtual T GetAndRemove(Index index)
	{
		var value = this[index];
		RemoveAt(index);
		return value;
	}

	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection) => GetBeforeSetAfter(collection, 0, _size);

	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection, int index) => GetBeforeSetAfter(collection, index, _size - index);

	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length, out var collectionLength);
		if (foundIndex == -1)
		{
			var toReturn = CollectionCreator(this);
			Clear();
			return toReturn;
		}
		else
		{
			var toReturn = GetRange(0, foundIndex, true);
			Remove(0, foundIndex + collectionLength);
			return toReturn;
		}
	}

	public virtual TCertain GetBeforeSetAfter(TCertain list) => GetBeforeSetAfter((IEnumerable<T>)list, 0, _size);

	public virtual TCertain GetBeforeSetAfter(TCertain list, int index) => GetBeforeSetAfter((IEnumerable<T>)list, index, _size - index);

	public virtual TCertain GetBeforeSetAfter(TCertain list, int index, int length) => GetBeforeSetAfter((IEnumerable<T>)list, index, length);

	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection) => GetBeforeSetAfterLast(collection, _size - 1, _size);

	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection, int index) => GetBeforeSetAfterLast(collection, index, index + 1);

	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length, out var collectionLength);
		if (foundIndex == -1)
		{
			var toReturn = CollectionCreator(this);
			Clear();
			return toReturn;
		}
		else
		{
			var toReturn = GetRange(0, foundIndex, true);
			Remove(0, foundIndex + collectionLength);
			return toReturn;
		}
	}

	public virtual TCertain GetBeforeSetAfterLast(TCertain list) => GetBeforeSetAfterLast((IEnumerable<T>)list, _size - 1, _size);

	public virtual TCertain GetBeforeSetAfterLast(TCertain list, int index) => GetBeforeSetAfterLast((IEnumerable<T>)list, index, index + 1);

	public virtual TCertain GetBeforeSetAfterLast(TCertain list, int index, int length) => GetBeforeSetAfterLast((IEnumerable<T>)list, index, length);

	public virtual T GetOrAdd(int index, T value)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		return GetOrAddInternal(index, value);
	}

	protected virtual T GetOrAddInternal(int index, T value) => index == _size ? Add(value).GetInternal(index) : GetInternal(index);

	protected override TCertain GetRangeInternal(int index, int length)
	{
		var list = CapacityCreator(length);
		CopyToInternal(index, list, 0, length);
		return list;
	}

	protected override Slice<T> GetSliceInternal(int index, int length) => new((G.IList<T>)this, index, length);

	int System.Collections.IList.IndexOf(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return IndexOf((T)item);
		return -1;
	}

	public virtual TCertain Insert(Index index, T item) => Insert(index.GetOffset(_size), item);

	public virtual TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
			EnsureCapacity(_size + 1);
		var this2 = (TCertain)this;
		if (index < _size)
			CopyToInternal(index, this2, index + 1, _size - index);
		else
			_size++;
		SetInternal(index, item);
		return this2;
	}

	void G.IList<T>.Insert(int index, T item) => Insert(index, item);

	void System.Collections.IList.Insert(int index, object? item)
	{
		try
		{
			if (item != null)
				Insert(index, (T)item);
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException("Ошибка, такой элемент не подходит для этой коллекции.", nameof(item));
		}
	}

	public virtual TCertain Insert(Index index, IEnumerable<T> collection) => Insert(index.GetOffset(_size), collection);

	public virtual TCertain Insert(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentNullException.ThrowIfNull(collection);
		return InsertInternal(index, collection);
	}

	public virtual TCertain Insert(Index index, ReadOnlySpan<T> span) => Insert(index.GetOffset(_size), span);

	public virtual TCertain Insert(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		return InsertInternal(index, span);
	}

	public virtual TCertain Insert(Index index, T[] array) => Insert(index.GetOffset(_size), array.AsSpan());

	public virtual TCertain Insert(int index, T[] array) => Insert(index, array.AsSpan());

	public virtual TCertain Insert(Index index, List<T> list) => Insert(index.GetOffset(_size), (IEnumerable<T>)list);

	public virtual TCertain Insert(int index, List<T> list) => Insert(index, (IEnumerable<T>)list);

	protected virtual TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var this2 = (TCertain)this;
		var length = list._size;
		if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < _size)
				CopyToInternal(index, this2, index + length, _size - index);
			if (this == list)
			{
				CopyToInternal(0, this2, index, index);
				CopyToInternal(index + length, this2, index * 2, _size - index);
			}
			else
				list.CopyToInternal(0, this2, index, length);
		}
		return this2;
	}

	protected virtual TCertain InsertInternal(int index, ReadOnlySpan<T> span)
	{
		var this2 = (TCertain)this;
		var length = span.Length;
		if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < _size)
				CopyToInternal(index, this2, index + length, _size - index);
			else
				_size += span.Length;
			for (var i = 0; i < length; i++)
				SetInternal(index + i, span[i]);
		}
		return this2;
	}

	public virtual TCertain Pad(int length) => Pad(length, default!);

	public virtual TCertain Pad(int length, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var this2 = (TCertain)this;
		if (length <= _size)
			return this2;
		var result = CapacityCreator(length);
		var left = (length - _size) >> 1;
		for (var i = 0; i < left; i++)
			result.Add(value);
		result.AddRange(this2);
		while (result.Length < length)
			result.Add(value);
		return result;
	}

	public virtual TCertain PadInPlace(int length) => PadInPlace(length, default!);

	public virtual TCertain PadInPlace(int length, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var this2 = (TCertain)this;
		if (length <= _size)
			return this2;
		var left = (length - _size) >> 1;
		var toPadLeft = CapacityCreator(left);
		for (var i = 0; i < left; i++)
			toPadLeft.Add(value);
		Insert(0, toPadLeft);
		while (_size < length)
			Add(value);
		return this2;
	}

	public virtual TCertain PadLeft(int length) => PadLeft(length, default!);

	public virtual TCertain PadLeft(int length, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var this2 = (TCertain)this;
		if (length <= _size)
			return this2;
		var result = CapacityCreator(length);
		var left = length - _size;
		for (var i = 0; i < left; i++)
			result.Add(value);
		result.AddRange(this2);
		return result;
	}

	public virtual TCertain PadLeftInPlace(int length) => PadLeftInPlace(length, default!);

	public virtual TCertain PadLeftInPlace(int length, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var this2 = (TCertain)this;
		if (length <= _size)
			return this2;
		var left = length - _size;
		var toPadLeft = CapacityCreator(left);
		for (var i = 0; i < left; i++)
			toPadLeft.Add(value);
		Insert(0, toPadLeft);
		return this2;
	}

	public virtual TCertain PadRight(int length) => PadRight(length, default!);

	public virtual TCertain PadRight(int length, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var this2 = (TCertain)this;
		if (length <= _size)
			return this2;
		var result = CapacityCreator(length);
		result.AddRange(this2);
		while (result.Length < length)
			result.Add(value);
		return result;
	}

	public virtual TCertain PadRightInPlace(int length) => PadRightInPlace(length, default!);

	public virtual TCertain PadRightInPlace(int length, T value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var this2 = (TCertain)this;
		if (length <= _size)
			return this2;
		while (_size < length)
			Add(value);
		return this2;
	}

	[Obsolete("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо использовать Remove()"
		+ " с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().", true)]
	public virtual TCertain Remove(Index index) =>
		throw new NotSupportedException("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо"
			+ " использовать Remove() с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().");

	[Obsolete("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо использовать Remove()"
		+ " с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().", true)]
	public virtual TCertain Remove(int index) =>
		throw new NotSupportedException("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо"
			+ " использовать Remove() с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().");

	public virtual TCertain Remove(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		return RemoveInternal(index, length);
	}

	public virtual TCertain Remove(Range range)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		var (start, length) = range.GetOffsetAndLength(_size);
		return Remove(start, length);
	}

	void System.Collections.IList.Remove(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				RemoveValue((T)item);
	}

	public virtual int RemoveAll(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		var freeIndex = 0;
		while (freeIndex < _size && !match(GetInternal(freeIndex))) freeIndex++;
		if (freeIndex >= _size) return 0;
		var current = freeIndex + 1;
		while (current < _size)
		{
			while (current < _size && match(GetInternal(current))) current++;
			if (current < _size)
				SetInternal(freeIndex++, GetInternal(current++));
		}
		ClearInternal(freeIndex, _size - freeIndex);
		var result = _size - freeIndex;
		_size = freeIndex;
		return result;
	}

	public virtual TCertain RemoveAt(Index index) => RemoveAt(index.GetOffset(_size));

	public virtual TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		_size--;
		if (index < _size)
			CopyToInternal(index + 1, this2, index, _size - index);
		SetInternal(_size, default!);
		return this2;
	}

	void System.Collections.IList.RemoveAt(int index) => RemoveAt(index);

	void G.IList<T>.RemoveAt(int index) => RemoveAt(index);

	public virtual TCertain RemoveEnd(Index index) => RemoveEnd(index.GetOffset(_size));

	public virtual TCertain RemoveEnd(int index) => Remove(index, _size - index);

	protected virtual TCertain RemoveInternal(int index, int length)
	{
		var this2 = (TCertain)this;
		if (length > 0)
		{
			_size -= length;
			if (index < _size)
				CopyToInternal(index + length, this2, index, _size - index);
			ClearInternal(_size, length);
		}
		return this2;
	}

	public virtual bool RemoveValue(T item)
	{
		var index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual TCertain Repeat(int length)
	{
		var result = CapacityCreator(Length * length);
		for (var i = 0; i < length; i++)
			result.AddRange(this);
		return result;
	}

	public virtual TCertain Replace(IEnumerable<T> collection)
	{
		Clear();
		return AddRange(collection);
	}

	public virtual TCertain Replace(ReadOnlySpan<T> span)
	{
		Clear();
		return AddRange(span);
	}

	public virtual TCertain Replace(T[] array) => Replace(array.AsSpan());

	public virtual TCertain Replace(T oldItem, T newItem)
	{
		var result = CollectionCreator(this);
		for (var index = IndexOf(oldItem); index >= 0; index = IndexOf(oldItem, index + 1))
			result.SetInternal(index, newItem);
		return result;
	}
#pragma warning disable CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	public virtual TCertain Replace(Dictionary<T, IEnumerable<T>>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		if (_size < 2)
			return CollectionCreator(this);
		var result = CapacityCreator(_size + GetArrayLength(_size, 10));
		for (var i = 0; i < _size; i++)
		{
			var element = GetInternal(i);
			if (dic.TryGetValue(element, out var newCollection))
				result.AddRange(newCollection);
			else
				result.Add(element);
		}
		return result;
	}

	public virtual TCertain Replace(Dictionary<T, T>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		var result = CollectionCreator(this);
		for (var i = 0; i < _size; i++)
			if (dic.TryGetValue(result.GetInternal(i), out var newItem))
				result.SetInternal(i, newItem);
		return result;
	}

	public virtual TCertain Replace(Dictionary<T, TCertain>? dic) => Replace(dic?.ToDictionary(x => x.Key, x => (IEnumerable<T>)x.Value)!);
#pragma warning restore CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	public virtual TCertain Replace(Dictionary<(T, T), IEnumerable<T>>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		if (_size < 2)
			return CollectionCreator(this);
		var result = CapacityCreator(_size + GetArrayLength(_size, 10));
		using Buffer<T> buffer = new(2) { GetInternal(0) };
		for (var i = 1; i < _size; i++)
		{
			buffer.Add(GetInternal(i));
			if (!buffer.IsFull)
				continue;
			if (dic.TryGetValue((buffer.GetInternal(0), buffer.GetInternal(1)), out var newCollection))
			{
				result.AddRange(newCollection);
				buffer.Clear(false);
			}
			else
				result.Add(buffer.GetAndRemove(0));
		}
		return result.AddRange(buffer);
	}

	public virtual TCertain Replace(Dictionary<(T, T), TCertain>? dic) => Replace(dic?.ToDictionary(x => x.Key, x => (IEnumerable<T>)x.Value)!);

	public virtual TCertain Replace(Dictionary<(T, T, T), IEnumerable<T>>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		if (_size < 3)
			return CollectionCreator(this);
		var result = CapacityCreator(_size + GetArrayLength(_size, 10));
		using Buffer<T> buffer = new(3) { GetInternal(0), GetInternal(1) };
		for (var i = 2; i < _size; i++)
		{
			buffer.Add(GetInternal(i));
			if (!buffer.IsFull)
				continue;
			if (dic.TryGetValue((buffer.GetInternal(0), buffer.GetInternal(1), buffer.GetInternal(2)), out var newCollection))
			{
				result.AddRange(newCollection);
				buffer.Clear(false);
			}
			else
				result.Add(buffer.GetAndRemove(0));
		}
		return result.AddRange(buffer);
	}

	public virtual TCertain Replace(Dictionary<(T, T, T), TCertain>? dic) => Replace(dic?.ToDictionary(x => x.Key, x => (IEnumerable<T>)x.Value)!);

	public virtual TCertain Replace(IEnumerable<T> oldCollection, IEnumerable<T> newCollection)
	{
		ArgumentNullException.ThrowIfNull(oldCollection);
		ArgumentNullException.ThrowIfNull(newCollection);
		var length = oldCollection.Length();
		if (length == 0 || length > _size)
			return CollectionCreator(this);
		var result = CapacityCreator(_size);
		using LimitedQueue<T> queue = new(length);
		for (var i = 0; i < length - 1; i++)
			queue.Enqueue(GetInternal(i));
		for (var i = length - 1; i < _size; i++)
		{
			queue.Enqueue(GetInternal(i));
			if (Enumerable.SequenceEqual(queue, oldCollection))
			{
				result.AddRange(newCollection);
				queue.Clear();
			}
			else if (queue.IsFull)
				result.Add(queue.Dequeue());
		}
		return result.AddRange(queue);
	}

	public virtual TCertain Replace(TCertain oldList, TCertain newList) => Replace((IEnumerable<T>)oldList, newList);

	public virtual TCertain ReplaceInPlace(T oldItem, T newItem)
	{
		for (var index = IndexOf(oldItem); index >= 0; index = IndexOf(oldItem, index + 1))
			SetInternal(index, newItem);
		return (TCertain)this;
	}
#pragma warning disable CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	public virtual TCertain ReplaceInPlace(Dictionary<T, IEnumerable<T>>? dic) => Replace(Replace(dic));

	public virtual TCertain ReplaceInPlace(Dictionary<T, T>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		for (var i = 0; i < _size; i++)
			if (dic.TryGetValue(GetInternal(i), out var newItem))
				SetInternal(i, newItem);
		return (TCertain)this;
	}

	public virtual TCertain ReplaceInPlace(Dictionary<T, TCertain>? dic) => Replace(Replace(dic));
#pragma warning restore CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	public virtual TCertain ReplaceInPlace(Dictionary<(T, T), IEnumerable<T>>? dic) => Replace(Replace(dic));

	public virtual TCertain ReplaceInPlace(Dictionary<(T, T), TCertain>? dic) => Replace(Replace(dic));

	public virtual TCertain ReplaceInPlace(Dictionary<(T, T, T), IEnumerable<T>>? dic) => Replace(Replace(dic));

	public virtual TCertain ReplaceInPlace(Dictionary<(T, T, T), TCertain>? dic) => Replace(Replace(dic));

	public virtual TCertain ReplaceInPlace(IEnumerable<T> oldCollection, IEnumerable<T> newCollection) => Replace(Replace(oldCollection, newCollection));

	public virtual TCertain ReplaceInPlace(TCertain oldList, TCertain newList) => ReplaceInPlace((IEnumerable<T>)oldList, newList);

	public virtual TCertain ReplaceRange(int index, int length, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Заменяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		return ReplaceRangeInternal(index, length, collection);
	}

	public virtual TCertain ReplaceRange(int index, int length, TCertain list) => ReplaceRange(index, length, (IEnumerable<T>)list);

	internal virtual TCertain ReplaceRangeInternal(int index, int length, IEnumerable<T> collection)
	{
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var this2 = (TCertain)this;
		if (list._size > 0)
		{
			EnsureCapacity(_size + list._size - length);
			if (index + length < _size)
				CopyToInternal(index + length, this2, index + list._size, _size - index - length);
			list.CopyToInternal(0, this2, index, list._size);
			_size += index + length < _size ? 0 : list._size - length;
		}
		return this2;
	}

	public virtual TCertain Resize(int newSize)
	{
		if (newSize == _size)
			return (TCertain)this;
		EnsureCapacity(newSize);
		if (newSize > _size)
		{
			_size = newSize;
			return (TCertain)this;
		}
		else
			return RemoveEnd(newSize);
	}

	public virtual TCertain Reverse() => Reverse(0, _size);

	public virtual TCertain Reverse(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		ReverseInternal(index, length);
		return (TCertain)this;
	}

	protected abstract void ReverseInternal(int index, int length);

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

	public virtual TCertain SetOrAdd(int index, T value)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		return SetOrAddInternal(index, value);
	}

	protected virtual TCertain SetOrAddInternal(int index, T value)
	{
		if (index == _size)
			return Add(value);
		SetInternal(index, value);
		return (TCertain)this;
	}

	public virtual TCertain SetRange(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var length = list._size;
		if (index + length > _size)
			throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
		return SetRangeInternal(index, length, list);
	}

	internal virtual TCertain SetRangeAndSizeInternal(int index, int length, TCertain list)
	{
		SetRangeInternal(index, length, list);
		_size = Max(_size, index + length);
		return (TCertain)this;
	}

	internal virtual TCertain SetRangeInternal(int index, int length, TCertain list)
	{
		EnsureCapacity(index + length);
		var this2 = (TCertain)this;
		if (length > 0)
			list.CopyToInternal(0, this2, index, length);
		return this2;
	}

	public virtual TCertain Shuffle()
	{
		Random random = new();
		for (var i = _size; i > 0; i--)
		{
			var swapIndex = random.Next(i);
			var temp = GetInternal(swapIndex);
			SetInternal(swapIndex, GetInternal(i - 1));
			SetInternal(i - 1, temp);
		}
		return (TCertain)this;
	}

	public static List<TCertain> Transpose(List<TCertain> list, bool widen = false)
	{
		if (list._size == 0)
			throw new ArgumentException("Невозможно транспонировать коллекцию нулевой длины.", nameof(list));
		var yCount = widen ? list.Max(x => x._size) : list.Min(x => x._size);
		List<TCertain> new_list = [];
		for (var i = 0; i < yCount; i++)
		{
			new_list.Add(list.GetInternal(0).CapacityCreator(list._size));
			for (var j = 0; j < list._size; j++)
			{
				var temp = list.GetInternal(j);
				new_list.GetInternal(i).Add(temp._size <= i ? default! : temp.GetInternal(i));
			}
		}
		return new_list;
	}

	public virtual TCertain TrimExcess()
	{
		var threshold = (int)(Capacity * 0.9);
		if (_size < threshold)
			Capacity = _size;
		return (TCertain)this;
	}

	public static implicit operator BaseList<T, TCertain>(T x) => new TCertain().Add(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseSet<T, TCertain> : BaseList<T, TCertain>, ISet<T> where TCertain : BaseSet<T, TCertain>, new()
{
	public override TCertain Add(T item)
	{
		TryAdd(item);
		return (TCertain)this;
	}

	bool ISet<T>.Add(T item) => TryAdd(item);

	public override TCertain AddRange(IEnumerable<T> collection) => UnionWith(collection);

	protected override void AddSeriesInternal(T item, int length)
	{
		if (length != 0)
			Add(item);
	}

	public override Span<T> AsSpan(int index, int length) => List<T>.ReturnOrConstruct(this).AsSpan(index, length);

	protected override void ClearInternal(int index, int length)
	{
		for (var i = index; i < index + length; i++)
			SetInternal(i, default!);
	}

	public override bool Contains(T? item, int index, int length) => item != null && IndexOf(item, index, length) >= 0;

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
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

	public virtual TCertain ExceptWith(IEnumerable<T> other)
	{
		if (other is ISet<T> set)
			return FilterInPlace(x => !set.Contains(x));
		var set2 = other.ToHashSet();
		return FilterInPlace(x => !set2.Contains(x));
	}

	void ISet<T>.ExceptWith(IEnumerable<T> other) => ExceptWith(other);

	public override TCertain FillInPlace(Func<int, T> function, int length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(length, 1, nameof(length));
		return base.FillInPlace(function, length);
	}

	public override TCertain FillInPlace(T item, int length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(length, 1, nameof(length));
		return base.FillInPlace(item, length);
	}

	public override TCertain Insert(int index, T item)
	{
		if (!Contains(item))
			base.Insert(index, item);
		return (TCertain)this;
	}

	public virtual TCertain IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = other.ToHashSet();
		return FilterInPlace(set.Contains);
	}

	void ISet<T>.IntersectWith(IEnumerable<T> other) => IntersectWith(other);

	public virtual bool IsProperSubsetOf(IEnumerable<T> other) => !SetEquals(other is ISet<T> set ? set : set = CollectionCreator(other)) && IsSubsetOf(set);

	public virtual bool IsProperSupersetOf(IEnumerable<T> other) => !SetEquals(other) && IsSupersetOf(other);

	public virtual bool IsSubsetOf(IEnumerable<T> other) => (other is ISet<T> set ? set : CollectionCreator(other)).IsSupersetOf(this);

	public virtual bool IsSupersetOf(IEnumerable<T> other)
	{
		foreach (var item in other)
			if (!Contains(item))
				return false;
		return true;
	}

	public override int LastIndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции. Используйте IndexOf() вместо него.");

	public override int LastIndexOfAny(IEnumerable<T> collection, int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции. Используйте IndexOfAny() вместо него.");

	public override int LastIndexOfAnyExcluding(IEnumerable<T> collection, int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
		+ " Используйте IndexOfAnyExcluding() вместо него.");

	protected override int LastIndexOfInternal(T item, int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции. Используйте IndexOf() вместо него.");

	public virtual bool Overlaps(IEnumerable<T> other)
	{
		foreach (var item in other)
			if (Contains(item))
				return true;
		return false;
	}

	public override TCertain Pad(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public override TCertain PadInPlace(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public override TCertain PadLeft(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public override TCertain PadLeftInPlace(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public override TCertain PadRight(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public override TCertain PadRightInPlace(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public override TCertain Repeat(int length) => length switch
	{
		0 => new(),
		1 => Copy(),
		_ => throw new ArgumentOutOfRangeException(nameof(length)),
	};

	internal override TCertain ReplaceRangeInternal(int index, int length, IEnumerable<T> collection) => base.ReplaceRangeInternal(index, length, collection is TCertain list ? list : CollectionCreator(collection).ExceptWith(GetSlice(0, index)).ExceptWith(GetSlice(index + length)));

	protected override void ReverseInternal(int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	public virtual bool SetEquals(IEnumerable<T> other)
	{
		if (other.TryGetLengthEasily(out var length))
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
			var set = CollectionCreator(other);
			if (Length != set.Length)
				return false;
			foreach (var item in set)
				if (!Contains(item))
					return false;
			return true;
		}
	}

	internal override TCertain SetRangeInternal(int index, int length, TCertain list) => base.SetRangeInternal(index, CreateVar(CollectionCreator(list).ExceptWith(GetSlice(0, index)).ExceptWith(GetSlice(index + length)), out var list2).Length, list2);

	public virtual TCertain SymmetricExceptWith(IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		var this2 = (TCertain)this;
		if (Length == 0)
		{
			UnionWith(other);
			return this2;
		}
		if (other == this)
		{
			Clear();
			return this2;
		}
		return SymmetricExceptInternal(other);
	}

	protected virtual TCertain SymmetricExceptInternal(IEnumerable<T> other)
	{
		foreach (var item in other is ISet<T> set ? set : other.ToHashSet())
		{
			var result = Contains(item) ? RemoveValue(item) : TryAdd(item);
			Debug.Assert(result);
		}
		return (TCertain)this;
	}

	void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => SymmetricExceptWith(other);

	public virtual bool TryAdd(T item) => TryAdd(item, out _);

	public abstract bool TryAdd(T item, out int index);

	public virtual bool TryGetIndexOf(T item, out int index) => (index = IndexOf(item)) >= 0;

	public virtual TCertain UnionWith(IEnumerable<T> other)
	{
		foreach (var item in other)
			TryAdd(item);
		return (TCertain)this;
	}

	void ISet<T>.UnionWith(IEnumerable<T> other) => UnionWith(other);
}
