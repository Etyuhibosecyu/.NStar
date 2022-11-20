using Corlib.NStar;
using System.Diagnostics;

namespace Corlib.NStar;

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public abstract class ListBase<T, TCertain> : IList<T>, IList, IReadOnlyList<T>, IDisposable/*, IComparable<ListBase<T, TCertain>>*/, IEquatable<ListBase<T, TCertain>> where TCertain : ListBase<T, TCertain>, new()
{
	private protected int _size;
	[NonSerialized]
	private protected object _syncRoot = new();

	public abstract int Capacity { get; set; }

	private protected abstract Func<int, TCertain> CapacityCreator { get; }

	private protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected virtual int DefaultCapacity => 4;

	bool System.Collections.IList.IsFixedSize => false;

	bool G.ICollection<T>.IsReadOnly => false;

	bool System.Collections.IList.IsReadOnly => false;

	bool System.Collections.ICollection.IsSynchronized => false;

	public virtual int Length => _size;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	public virtual T this[Index index, bool invoke = true]
	{
		get
		{
			int index2 = index.IsFromEnd ? _size - index.Value : index.Value;
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			return GetInternal(index2, invoke);
		}
		set
		{
			int index2 = index.IsFromEnd ? _size - index.Value : index.Value;
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			SetInternal(index2, value);
		}
	}

	public virtual TCertain this[Range range] => GetRange(range);

	T G.IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

	T G.IReadOnlyList<T>.this[int index] => this[index];

	object? System.Collections.IList.this[int index]
	{
		get => this[index];
		set
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			try
			{
				this[index] = (T)value;
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(null, nameof(value));
			}
		}
	}

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public virtual TCertain Add(T item)
	{
		if (_size == Capacity) EnsureCapacity(_size + 1);
		SetInternal(_size++, item);
		return this as TCertain ?? throw new InvalidOperationException();
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
			throw new ArgumentException(null, nameof(item));
		}
		return _size - 1;
	}

	public virtual TCertain AddRange(IEnumerable<T> collection) => Insert(_size, collection);

	public virtual TCertain Append(T item) => CollectionCreator(this).Add(item);

	public virtual Span<T> AsSpan() => AsSpan(0, _size);

	public virtual Span<T> AsSpan(Index index) => AsSpan()[index..];

	public virtual Span<T> AsSpan(int index) => AsSpan(index, _size - index);

	public abstract Span<T> AsSpan(int index, int count);

	public virtual Span<T> AsSpan(Range range) => AsSpan()[range];

	private protected void Changed() => ListChanged?.Invoke(this as TCertain ?? throw new InvalidOperationException());

	public virtual void Clear()
	{
		if (_size > 0)
		{
			ClearInternal();
			_size = 0;
		}
	}

	public virtual void Clear(int index, int count)
	{
		if (index > _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0 || index > _size - count)
			throw new ArgumentOutOfRangeException(nameof(count));
		ClearInternal(index, count);
	}

	private protected virtual void ClearInternal() => ClearInternal(0, _size);

	private protected abstract void ClearInternal(int index, int count);

	public virtual TCertain Concat(TCertain collection) => CollectionCreator(this).AddRange(collection);

	//public virtual int CompareTo(ListBase<T, TCertain>? other)
	//{
	//	if (other == null || other is not TCertain m)
	//		return 1;
	//	else
	//		return CompareToInternal(m);
	//}

	//private protected virtual int CompareToInternal(TCertain other)
	//{
	//	int c;
	//	for (int i = 0; i < _size && i < other._size; i++)
	//		if ((c = ((IComparable<T>?)GetInternal(i) ?? throw new InvalidOperationException()).CompareTo(other.GetInternal(i))) != 0)
	//			return c;
	//	return _size.CompareTo(other._size);
	//}

	public virtual bool Contains(T? item) => Contains(item, 0, _size);

	public virtual bool Contains(T? item, int index) => Contains(item, index, _size - index);

	public virtual bool Contains(T? item, int index, int count)
	{
		if (item == null)
		{
			for (int i = 0; i < count; i++)
				if (this[index + i] == null)
					return true;
			return false;
		}
		else
		{
			EqualityComparer<T> c = EqualityComparer<T>.Default;
			for (int i = 0; i < count; i++)
				if (c.Equals(this[index + i], item))
					return true;
			return false;
		}
	}

	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, _size);

	public virtual bool Contains(IEnumerable<T> collection, int index) => Contains(collection, index, _size - index);

	public virtual bool Contains(IEnumerable<T> collection, int index, int count)
	{
		if (count == 0 || !collection.Any())
		{
			return false;
		}
		if (collection is not G.IList<T> list)
			return Contains(CollectionCreator(collection));
		int j = 0;
		for (int i = 0; i - j <= count - list.Count; i++)
		{
			if (this[index + i]?.Equals(list[j]) ?? list[j] is null)
			{
				j++;
				if (j >= list.Count)
				{
					return true;
				}
			}
			else if (j != 0)
			{
				i -= j;
				j = 0;
			}
		}
		return false;
	}

	public virtual bool Contains(TCertain list) => Contains((IEnumerable<T>)list, 0, _size);

	public virtual bool Contains(TCertain list, int index) => Contains((IEnumerable<T>)list, index, _size - index);

	public virtual bool Contains(TCertain list, int index, int count) => Contains((IEnumerable<T>)list, index, count);

	bool System.Collections.IList.Contains(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return Contains((T)item);
		return false;
	}

	public virtual bool ContainsAny(IEnumerable<T> collection) => ContainsAny(collection, 0, _size);

	public virtual bool ContainsAny(IEnumerable<T> collection, int index) => ContainsAny(collection, index, _size - index);

	public virtual bool ContainsAny(IEnumerable<T> collection, int index, int count)
	{
		FakeIndAftDelHashSet<T> hs = collection.ToHashSet();
		for (int i = 0; i < count; i++)
			if (hs.Contains(GetInternal(index + i)))
				return true;
		return false;
	}

	public virtual bool ContainsAny(TCertain list) => ContainsAny((IEnumerable<T>)list, 0, _size);

	public virtual bool ContainsAny(TCertain list, int index) => ContainsAny((IEnumerable<T>)list, index, _size - index);

	public virtual bool ContainsAny(TCertain list, int index, int count) => ContainsAny((IEnumerable<T>)list, index, count);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, _size);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, int index) => ContainsAnyExcluding(collection, index, _size - index);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, int index, int count)
	{
		FakeIndAftDelHashSet<T> hs = collection.ToHashSet();
		for (int i = 0; i < count; i++)
			if (!hs.Contains(GetInternal(index + i)))
				return true;
		return false;
	}

	public virtual bool ContainsAnyExcluding(TCertain list) => ContainsAnyExcluding((IEnumerable<T>)list, 0, _size);

	public virtual bool ContainsAnyExcluding(TCertain list, int index) => ContainsAnyExcluding((IEnumerable<T>)list, index, _size - index);

	public virtual bool ContainsAnyExcluding(TCertain list, int index, int count) => ContainsAnyExcluding((IEnumerable<T>)list, index, count);

	public virtual TCertainOutput Convert<TOutput, TCertainOutput>(Func<T, TOutput> converter) where TCertainOutput : ListBase<TOutput, TCertainOutput>, new()
	{
		if (converter == null)
			throw new ArgumentNullException(nameof(converter));
		TCertainOutput list = Activator.CreateInstance(typeof(TCertainOutput), _size) as TCertainOutput ?? throw new InvalidOperationException();
		for (int i = 0; i < _size; i++)
			list.SetInternal(i, converter(GetInternal(i)));
		list._size = _size;
		return list;
	}

	public virtual TCertainOutput Convert<TOutput, TCertainOutput>(Func<T, int, TOutput> converter) where TCertainOutput : ListBase<TOutput, TCertainOutput>, new()
	{
		if (converter == null)
			throw new ArgumentNullException(nameof(converter));
		TCertainOutput list = Activator.CreateInstance(typeof(TCertainOutput), _size) as TCertainOutput ?? throw new InvalidOperationException();
		for (int i = 0; i < _size; i++)
			list.SetInternal(i, converter(GetInternal(i), i));
		list._size = _size;
		return list;
	}

	public static void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int count) => source.Copy(source, sourceIndex, destination, destinationIndex, count);

	private protected abstract void Copy(ListBase<T, TCertain> source, int sourceIndex, ListBase<T, TCertain> destination, int destinationIndex, int count);

	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	public virtual void CopyTo(T[] array, int arrayIndex) => CopyTo(0, array, arrayIndex, _size);

	public virtual void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		if (index + count > _size)
			throw new ArgumentException(null);
		CopyToInternal(index, array, arrayIndex, count);
	}

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		if ((array != null) && (array.Rank != 1))
			throw new ArgumentException(null);
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		try
		{
			CopyToInternal(array, arrayIndex);
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException(null);
		}
	}

	private protected abstract void CopyToInternal(Array array, int arrayIndex);

	private protected abstract void CopyToInternal(int index, T[] array, int arrayIndex, int count);

	public abstract void Dispose();

	public virtual bool EndsWith(IEnumerable<T> collection) => EqualsInternal(collection, _size - collection.Count());

	public virtual bool EndsWith(TCertain list) => EndsWith((IEnumerable<T>)list);

	private protected virtual void EnsureCapacity(int min)
	{
		if (Capacity < min)
		{
			int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
	}

	public override bool Equals(object? obj)
	{
		if (obj == null || obj is not G.ICollection<T> m)
			return false;
		else if (_size != m.Count)
			return false;
		else
			return Equals(m);
	}


	public virtual bool Equals(IEnumerable<T>? collection, int index = 0, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	public virtual bool Equals(ListBase<T, TCertain>? list) => EqualsInternal(list, 0, true);

	public virtual bool Equals(ListBase<T, TCertain>? list, int index, bool toEnd = false) => EqualsInternal(list, index, toEnd);

	private protected virtual bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.IList<T> list)
		{
			if (index > _size - list.Count)
				return false;
			if (toEnd && index < _size - list.Count)
				return false;
			for (int i = 0; i < list.Count; i++)
				if (!(GetInternal(index++)?.Equals(list[i]) ?? list[i] is null))
					return false;
			return true;
		}
		else
		{
			if (collection.TryGetCountEasily(out int count))
			{
				if (index > _size - count)
					return false;
				if (toEnd && index < _size - count)
					return false;
			}
			foreach (T item in collection)
				if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item is null))
					return false;
			return !toEnd || index == _size;
		}
	}

	public virtual bool Exists(Predicate<T> match) => FindIndex(match) != -1;

	public virtual TCertain Filter(Func<T, bool> match)
	{
		TCertain result = CapacityCreator(_size);
		for (int i = 0; i < _size; i++)
		{
			T item = GetInternal(i);
			if (match(item))
				result.Add(item);
		}
		if (result._size < _size * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual TCertain Filter(Func<T, int, bool> match)
	{
		TCertain result = CapacityCreator(_size);
		for (int i = 0; i < _size; i++)
		{
			T item = GetInternal(i);
			if (match(item, i))
				result.Add(item);
		}
		if (result._size < _size * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual TCertain FilterInPlace(Func<T, bool> match)
	{
		int targetIndex = 0;
		for (int i = 0; i < _size; i++)
		{
			T item = GetInternal(i);
			if (match(item))
				SetInternal(targetIndex++, item);
		}
		_size = targetIndex;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain FilterInPlace(Func<T, int, bool> match)
	{
		int targetIndex = 0;
		for (int i = 0; i < _size; i++)
		{
			T item = GetInternal(i);
			if (match(item, i))
				SetInternal(targetIndex++, item);
		}
		_size = targetIndex;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual T? Find(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		for (int i = 0; i < _size; i++)
			if (match(this[i]))
				return this[i];
		return default;
	}

	public virtual TCertain FindAll(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		TCertain list = new();
		for (int i = 0; i < _size; i++)
			if (match(this[i]))
				list.Add(this[i]);
		return list;
	}

	public virtual int FindIndex(Predicate<T> match) => FindIndex(0, _size, match);

	public virtual int FindIndex(int startIndex, Predicate<T> match) => FindIndex(startIndex, _size - startIndex, match);

	public virtual int FindIndex(int startIndex, int count, Predicate<T> match)
	{
		if ((uint)startIndex > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(startIndex));
		if (count < 0 || startIndex > _size - count)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		int endIndex = startIndex + count;
		for (int i = startIndex; i < endIndex; i++)
			if (match(this[i]))
				return i;
		return -1;
	}

	public virtual T? FindLast(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		for (int i = _size - 1; i >= 0; i--)
			if (match(this[i]))
				return this[i];
		return default;
	}

	public virtual int FindLastIndex(Predicate<T> match) => FindLastIndex(_size - 1, _size, match);

	public virtual int FindLastIndex(int startIndex, Predicate<T> match) => FindLastIndex(startIndex, startIndex + 1, match);

	public virtual int FindLastIndex(int startIndex, int count, Predicate<T> match)
	{
		if ((uint)startIndex >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(startIndex));
		if (count < 0 || startIndex - count + 1 < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		if (_size == 0)
			if (startIndex != -1)
				throw new ArgumentOutOfRangeException(nameof(startIndex));
		int endIndex = startIndex - count;
		for (int i = startIndex; i > endIndex; i--)
			if (match(this[i]))
				return i;
		return -1;
	}

	public virtual TCertain ForEach(Action<T> action)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));
		for (int i = 0; i < _size; i++)
			action(this[i]);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain GetAfter(IEnumerable<T> collection) => GetAfter(collection, 0, _size);

	public virtual TCertain GetAfter(IEnumerable<T> collection, int index) => GetAfter(collection, index, _size - index);

	public virtual TCertain GetAfter(IEnumerable<T> collection, int index, int count)
	{
		int foundIndex = IndexOf(collection, index, count, out int otherCount);
		return index == -1 ? new() : GetRange(foundIndex + otherCount);
	}

	public virtual TCertain GetAfter(TCertain list) => GetAfter((IEnumerable<T>)list, 0, _size);

	public virtual TCertain GetAfter(TCertain list, int index) => GetAfter((IEnumerable<T>)list, index, _size - index);

	public virtual TCertain GetAfter(TCertain list, int index, int count) => GetAfter((IEnumerable<T>)list, index, count);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection) => GetAfterLast(collection, _size - 1, _size);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index) => GetAfterLast(collection, index, index + 1);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index, int count)
	{
		int foundIndex = LastIndexOf(collection, index, count, out int otherCount);
		return index == -1 ? new() : GetRange(foundIndex + otherCount);
	}

	public virtual TCertain GetAfterLast(TCertain list) => GetAfterLast((IEnumerable<T>)list, _size - 1, _size);

	public virtual TCertain GetAfterLast(TCertain list, int index) => GetAfterLast((IEnumerable<T>)list, index, index + 1);

	public virtual TCertain GetAfterLast(TCertain list, int index, int count) => GetAfterLast((IEnumerable<T>)list, index, count);

	public virtual TCertain GetBefore(IEnumerable<T> collection) => GetBefore(collection, 0, _size);

	public virtual TCertain GetBefore(IEnumerable<T> collection, int index) => GetBefore(collection, index, _size - index);

	public virtual TCertain GetBefore(IEnumerable<T> collection, int index, int count)
	{
		int foundIndex = IndexOf(collection, index, count);
		return index == -1 ? this as TCertain ?? throw new InvalidOperationException() : GetRange(0, foundIndex);
	}

	public virtual TCertain GetBefore(TCertain list) => GetBefore((IEnumerable<T>)list, 0, _size);

	public virtual TCertain GetBefore(TCertain list, int index) => GetBefore((IEnumerable<T>)list, index, _size - index);

	public virtual TCertain GetBefore(TCertain list, int index, int count) => GetBefore((IEnumerable<T>)list, index, count);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection) => GetBeforeLast(collection, _size - 1, _size);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index) => GetBeforeLast(collection, index, index + 1);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index, int count)
	{
		int foundIndex = LastIndexOf(collection, index, count);
		return index == -1 ? this as TCertain ?? throw new InvalidOperationException() : GetRange(0, foundIndex);
	}

	public virtual TCertain GetBeforeLast(TCertain list) => GetBeforeLast((IEnumerable<T>)list, _size - 1, _size);

	public virtual TCertain GetBeforeLast(TCertain list, int index) => GetBeforeLast((IEnumerable<T>)list, index, index + 1);

	public virtual TCertain GetBeforeLast(TCertain list, int index, int count) => GetBeforeLast((IEnumerable<T>)list, index, count);

	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection) => GetBeforeSetAfter(collection, 0, _size);

	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection, int index) => GetBeforeSetAfter(collection, index, _size - index);

	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection, int index, int count)
	{
		int foundIndex = IndexOf(collection, index, count, out int otherCount);
		if (index == -1)
		{
			TCertain toReturn = CollectionCreator(this);
			Clear();
			return toReturn;
		}
		else
		{
			TCertain toReturn = GetRange(0, foundIndex);
			Remove(0, foundIndex + otherCount);
			return toReturn;
		}
	}

	public virtual TCertain GetBeforeSetAfter(TCertain list) => GetBeforeSetAfter((IEnumerable<T>)list, 0, _size);

	public virtual TCertain GetBeforeSetAfter(TCertain list, int index) => GetBeforeSetAfter((IEnumerable<T>)list, index, _size - index);

	public virtual TCertain GetBeforeSetAfter(TCertain list, int index, int count) => GetBeforeSetAfter((IEnumerable<T>)list, index, count);

	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection) => GetBeforeSetAfterLast(collection, _size - 1, _size);

	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection, int index) => GetBeforeSetAfterLast(collection, index, index + 1);

	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection, int index, int count)
	{
		int foundIndex = LastIndexOf(collection, index, count, out int otherCount);
		if (index == -1)
		{
			TCertain toReturn = CollectionCreator(this);
			Clear();
			return toReturn;
		}
		else
		{
			TCertain toReturn = GetRange(0, foundIndex);
			Remove(0, foundIndex + otherCount);
			return toReturn;
		}
	}

	public virtual TCertain GetBeforeSetAfterLast(TCertain list) => GetBeforeSetAfterLast((IEnumerable<T>)list, _size - 1, _size);

	public virtual TCertain GetBeforeSetAfterLast(TCertain list, int index) => GetBeforeSetAfterLast((IEnumerable<T>)list, index, index + 1);

	public virtual TCertain GetBeforeSetAfterLast(TCertain list, int index, int count) => GetBeforeSetAfterLast((IEnumerable<T>)list, index, count);

	public virtual IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

	private Enumerator GetEnumeratorInternal() => new(this);

	public override int GetHashCode() => _size < 3 ? 1234567890 : ((GetInternal(0)?.GetHashCode() ?? 0) << 9 ^ (GetInternal(1)?.GetHashCode() ?? 0)) << 9 ^ (GetInternal(_size - 1)?.GetHashCode() ?? 0);

	internal abstract T GetInternal(int index, bool invoke = true);

	public virtual TCertain GetRange(int index) => GetRange(index, _size - index);

	public virtual TCertain GetRange(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		else if (index == 0 && count == _size && this is TCertain thisList)
			return thisList;
		TCertain list = CapacityCreator(count);
		Copy(this, index, list, 0, count);
		list._size = count;
		return list;
	}

	public virtual TCertain GetRange(Range range)
	{
		Index start = range.Start, end = range.End;
		if (start.IsFromEnd)
		{
			if (end.IsFromEnd)
				return GetRange(_size - start.Value, start.Value - end.Value);
			else
				return GetRange(_size - start.Value, end.Value - _size + start.Value);
		}
		else
		{
			if (end.IsFromEnd)
				return GetRange(start.Value, _size - end.Value - start.Value);
			else
				return GetRange(start.Value, end.Value - start.Value);
		}
	}

	public virtual int IndexOf(T item) => IndexOf(item, 0, _size);

	public virtual int IndexOf(T item, int index) => IndexOf(item, index, _size - index);

	public virtual int IndexOf(T item, int index, int count)
	{
		if (index > _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0 || index > _size - count)
			throw new ArgumentOutOfRangeException(nameof(count));
		return IndexOfInternal(item, index, count);
	}

	public virtual int IndexOf(IEnumerable<T> collection) => IndexOf(collection, 0, _size);

	public virtual int IndexOf(IEnumerable<T> collection, int index) => IndexOf(collection, index, _size - index);

	public virtual int IndexOf(IEnumerable<T> collection, int index, int count) => IndexOf(collection, index, count, out _);

	public virtual int IndexOf(IEnumerable<T> collection, int index, int count, out int otherCount)
	{
		if (index > _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0 || index > _size - count)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (_size == 0 || count == 0 || !collection.Any())
		{
			otherCount = 0;
			return -1;
		}
		if (collection is not G.ICollection<T> c)
			return IndexOf(CollectionCreator(collection), index, count, out otherCount);
		otherCount = c.Count;
		for (int i = 0; i <= count - otherCount; i++)
			if (EqualsInternal(collection, index + i))
				return index + i;
		return -1;
	}

	public virtual int IndexOf(TCertain list) => IndexOf((IEnumerable<T>)list, 0, _size);

	public virtual int IndexOf(TCertain list, int index) => IndexOf((IEnumerable<T>)list, index, _size - index);

	public virtual int IndexOf(TCertain list, int index, int count) => IndexOf((IEnumerable<T>)list, index, count);

	public virtual int IndexOf(TCertain list, int index, int count, out int otherCount) => IndexOf((IEnumerable<T>)list, index, count, out otherCount);

	int System.Collections.IList.IndexOf(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return IndexOf((T)item);
		return -1;
	}

	public virtual int IndexOfAny(IEnumerable<T> collection) => IndexOfAny(collection, 0, _size);

	public virtual int IndexOfAny(IEnumerable<T> collection, int index) => IndexOfAny(collection, index, _size - index);

	public virtual int IndexOfAny(IEnumerable<T> collection, int index, int count)
	{
		FakeIndAftDelHashSet<T> hs = collection.ToHashSet();
		for (int i = 0; i < count; i++)
			if (hs.Contains(GetInternal(index + i)))
				return index + i;
		return -1;
	}

	public virtual int IndexOfAny(TCertain list) => IndexOfAny((IEnumerable<T>)list, 0, _size);

	public virtual int IndexOfAny(TCertain list, int index) => IndexOfAny((IEnumerable<T>)list, index, _size - index);

	public virtual int IndexOfAny(TCertain list, int index, int count) => IndexOfAny((IEnumerable<T>)list, index, count);

	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection) => IndexOfAnyExcluding(collection, 0, _size);

	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection, int index) => IndexOfAnyExcluding(collection, index, _size - index);

	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection, int index, int count)
	{
		FakeIndAftDelHashSet<T> hs = collection.ToHashSet();
		for (int i = 0; i < count; i++)
			if (!hs.Contains(GetInternal(index + i)))
				return index + i;
		return -1;
	}

	public virtual int IndexOfAnyExcluding(TCertain list) => IndexOfAnyExcluding((IEnumerable<T>)list, 0, _size);

	public virtual int IndexOfAnyExcluding(TCertain list, int index) => IndexOfAnyExcluding((IEnumerable<T>)list, index, _size - index);

	public virtual int IndexOfAnyExcluding(TCertain list, int index, int count) => IndexOfAnyExcluding((IEnumerable<T>)list, index, count);

	private protected abstract int IndexOfInternal(T item, int index, int count);

	public virtual TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity) EnsureCapacity(_size + 1);
		if (index < _size)
			Copy(this, index, this, index + 1, _size - index);
		SetInternal(index, item);
		_size++;
		return this as TCertain ?? throw new InvalidOperationException();
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
			throw new ArgumentException(null, nameof(item));
		}
	}

	public virtual TCertain Insert(int index, IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		return InsertInternal(index, collection);
	}

	private protected virtual TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is not TCertain list)
			return InsertInternal(index, CollectionCreator(collection));
		int count = list._size;
		if (count > 0)
		{
			EnsureCapacity(_size + count);
			if (index < _size)
				Copy(this, index, this, index + count, _size - index);
			if (this == list)
			{
				Copy(this, 0, this, index, index);
				Copy(this, index + count, this, index * 2, _size - index);
			}
			else
				Copy(list, 0, this, index, count);
			_size += count;
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	protected static bool IsCompatibleObject(object? value) => (value is T) || (value == null && default(T) == null);

	public virtual int LastIndexOf(T item) => LastIndexOf(item, _size - 1, _size);

	public virtual int LastIndexOf(T item, int index) => LastIndexOf(item, index, index + 1);

	public virtual int LastIndexOf(T item, int index, int count)
	{
		if ((_size != 0) && (index < 0))
			throw new ArgumentOutOfRangeException(nameof(index));
		if ((_size != 0) && (count < 0))
			throw new ArgumentOutOfRangeException(nameof(count));
		if (_size == 0)
			return -1;
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count > index + 1)
			throw new ArgumentOutOfRangeException(nameof(count));
		return LastIndexOfInternal(item, index, count);
	}

	public virtual int LastIndexOf(IEnumerable<T> collection) => LastIndexOf(collection, _size - 1, _size);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index) => LastIndexOf(collection, index, index + 1);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int count) => LastIndexOf(collection, index, count, out _);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int count, out int otherCount)
	{
		if ((_size != 0) && (index < 0))
			throw new ArgumentOutOfRangeException(nameof(index));
		if ((_size != 0) && (count < 0))
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count > index + 1)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (_size == 0 || count == 0 || !collection.Any())
		{
			otherCount = 0;
			return -1;
		}
		if (collection is not G.ICollection<T> c)
			return LastIndexOf(CollectionCreator(collection), index, count, out otherCount);
		otherCount = c.Count;
		int startIndex = index + 1 - count;
		for (int i = count - otherCount; i >= 0; i--)
			if (EqualsInternal(c, startIndex + i))
				return startIndex + i;
		return -1;
	}

	public virtual int LastIndexOf(TCertain list) => LastIndexOf((IEnumerable<T>)list, _size - 1, _size);

	public virtual int LastIndexOf(TCertain list, int index) => LastIndexOf((IEnumerable<T>)list, index, index + 1);

	public virtual int LastIndexOf(TCertain list, int index, int count) => LastIndexOf((IEnumerable<T>)list, index, count);

	public virtual int LastIndexOf(TCertain list, int index, int count, out int otherCount) => LastIndexOf((IEnumerable<T>)list, index, count, out otherCount);

	public virtual int LastIndexOfAny(IEnumerable<T> collection) => LastIndexOfAny(collection, _size - 1, _size);

	public virtual int LastIndexOfAny(IEnumerable<T> collection, int index) => LastIndexOfAny(collection, index, index + 1);

	public virtual int LastIndexOfAny(IEnumerable<T> collection, int index, int count)
	{
		FakeIndAftDelHashSet<T> hs = collection.ToHashSet();
		for (int i = count - 1; i >= 0; i--)
			if (hs.Contains(GetInternal(index + i)))
				return index + i;
		return -1;
	}

	public virtual int LastIndexOfAny(TCertain list) => LastIndexOfAny((IEnumerable<T>)list, _size - 1, _size);

	public virtual int LastIndexOfAny(TCertain list, int index) => LastIndexOfAny((IEnumerable<T>)list, index, index + 1);

	public virtual int LastIndexOfAny(TCertain list, int index, int count) => LastIndexOfAny((IEnumerable<T>)list, index, count);

	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection) => LastIndexOfAnyExcluding(collection, _size - 1, _size);

	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection, int index) => LastIndexOfAnyExcluding(collection, index, index + 1);

	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection, int index, int count)
	{
		FakeIndAftDelHashSet<T> hs = collection.ToHashSet();
		for (int i = count - 1; i >= 0; i--)
			if (!hs.Contains(GetInternal(index + i)))
				return index + i;
		return -1;
	}

	public virtual int LastIndexOfAnyExcluding(TCertain list) => LastIndexOfAnyExcluding((IEnumerable<T>)list, _size - 1, _size);

	public virtual int LastIndexOfAnyExcluding(TCertain list, int index) => LastIndexOfAnyExcluding((IEnumerable<T>)list, index, index + 1);

	public virtual int LastIndexOfAnyExcluding(TCertain list, int index, int count) => LastIndexOfAnyExcluding((IEnumerable<T>)list, index, count);

	private protected abstract int LastIndexOfInternal(T item, int index, int count);

	public virtual TCertain Remove(int index) => Remove(index, _size - index);

	public virtual TCertain Remove(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count > 0)
		{
			_size -= count;
			if (index < _size)
				Copy(this, index + count, this, index, _size - index);
			ClearInternal(_size, count);
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	void System.Collections.IList.Remove(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				RemoveValue((T)item);
	}

	public virtual int RemoveAll(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		int freeIndex = 0;
		while (freeIndex < _size && !match(GetInternal(freeIndex))) freeIndex++;
		if (freeIndex >= _size) return 0;
		int current = freeIndex + 1;
		while (current < _size)
		{
			while (current < _size && match(GetInternal(current))) current++;
			if (current < _size)
				SetInternal(freeIndex++, GetInternal(current++));
		}
		ClearInternal(freeIndex, _size - freeIndex);
		int result = _size - freeIndex;
		_size = freeIndex;
		return result;
	}

	public virtual TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		_size--;
		if (index < _size)
			Copy(this, index + 1, this, index, _size - index);
		SetInternal(_size, default!);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	void System.Collections.IList.RemoveAt(int index) => RemoveAt(index);

	void G.IList<T>.RemoveAt(int index) => RemoveAt(index);

	internal static TCertain RemoveIndexes(TCertain originalList, Queue<int> toRemove)
	{
		List<int> toRemove2 = toRemove.ToList().Sort();
		TCertain result = originalList.CapacityCreator(originalList._size - toRemove2._size);
		int pos = 0;
		for (int i = 0; i < toRemove2._size; i++)
		{
			result.Copy(originalList, pos, result, pos - i, toRemove2[i] - pos);
			pos = toRemove2[i] + 1;
		}
		result._size = originalList._size - toRemove2._size;
		return result;
	}

	public virtual bool RemoveValue(T item)
	{
		int index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual TCertain Replace(IEnumerable<T> collection) => ReplaceRangeInternal(0, _size, collection);

	public virtual TCertain ReplaceRange(int index, int count, IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		return ReplaceRangeInternal(index, count, collection);
	}

	internal virtual TCertain ReplaceRangeInternal(int index, int count, IEnumerable<T> collection)
	{
		if (collection is TCertain list)
		{
			if (list._size > 0)
			{
				EnsureCapacity(_size + list._size - count);
				if (index + count < _size)
					Copy(this, index + count, this, index + list._size, _size - index - count);
				Copy(list, 0, this, index, list._size);
				_size += list._size - count;
			}
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return ReplaceRangeInternal(index, count, CollectionCreator(collection));
	}

	public virtual TCertain Reverse() => Reverse(0, _size);

	public virtual TCertain Reverse(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		return ReverseInternal(index, count);
	}

	private protected abstract TCertain ReverseInternal(int index, int count);

	internal abstract void SetInternal(int index, T value);

	public virtual TCertain SetRange(int index, IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is TCertain list)
		{
			int count = list._size;
			if (index + count > _size)
				throw new ArgumentException(null);
			EnsureCapacity(index + count);
			return SetRangeInternal(index, count, list);
		}
		else
			return SetRange(index, CollectionCreator(collection));
	}

	internal virtual TCertain SetRangeAndSizeInternal(int index, int count, TCertain list)
	{
		SetRangeInternal(index, count, list);
		_size = Max(_size, index + count);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	internal virtual TCertain SetRangeInternal(int index, int count, TCertain list)
	{
		if (count > 0)
			Copy(list, 0, this, index, count);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain Shuffle()
	{
		Random random = new();
		for (int i = _size; i > 0; i--)
		{
			int swapIndex = random.Next(i);
			(this[swapIndex], this[i - 1]) = (this[i - 1], this[swapIndex]);
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain Skip(int count) => GetRange(Min(count, _size), Max(0, _size - count));

	public virtual TCertain SkipLast(int count) => GetRange(0, Max(0, _size - count));

	public virtual TCertain SkipWhile(Func<T, bool> function)
	{
		int i = 0;
		for (; i < _size && function(GetInternal(i)); i++) ;
		return GetRange(i, _size - i);
	}

	public virtual TCertain SkipWhile(Func<T, int, bool> function)
	{
		int i = 0;
		for (; i < _size && function(GetInternal(i), i); i++) ;
		return GetRange(i, _size - i);
	}

	public virtual bool StartsWith(IEnumerable<T> collection) => EqualsInternal(collection, 0);

	public virtual bool StartsWith(TCertain list) => StartsWith((IEnumerable<T>)list);

	public virtual TCertain Take(int count) => GetRange(0, Min(count, _size));

	public virtual TCertain TakeLast(int count) => GetRange(Max(0, _size - count), Min(count, _size));

	public virtual TCertain TakeWhile(Func<T, bool> function)
	{
		int i = 0;
		for (; i < _size && function(GetInternal(i)); i++) ;
		return GetRange(0, i);
	}

	public virtual TCertain TakeWhile(Func<T, int, bool> function)
	{
		int i = 0;
		for (; i < _size && function(GetInternal(i), i); i++) ;
		return GetRange(0, i);
	}

	public virtual T[] ToArray()
	{
		T[] array = new T[Length];
		CopyToInternal(0, array, 0, Length);
		return array;
	}

	public static List<TCertain> Transpose(List<TCertain> list, bool widen = false)
	{
		if (list._size == 0)
			throw new ArgumentException(null, nameof(list));
		int yCount = widen ? list.Max(x => x._size) : list.Min(x => x._size);
		List<TCertain> new_list = new();
		for (int i = 0; i < yCount; i++)
		{
			new_list.Add(list[0].CapacityCreator(list._size));
			for (int j = 0; j < list._size; j++)
			{
				TCertain temp = list[j];
				new_list[i].Add(temp._size <= i ? default! : temp[i]);
			}
		}
		return new_list;
	}

	public virtual TCertain TrimExcess()
	{
		int threshold = (int)(Capacity * 0.9);
		if (_size < threshold)
			Capacity = _size;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual bool TrueForAll(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		for (int i = 0; i < _size; i++)
			if (!match(this[i]))
				return false;
		return true;
	}

	public static implicit operator ListBase<T, TCertain>(T x) => new TCertain().Add(x);

	[Serializable]
	public struct Enumerator : IEnumerator<T>, IEnumerator
	{
		private readonly ListBase<T, TCertain> list;
		private int index;
		private T current;

		internal Enumerator(ListBase<T, TCertain> list)
		{
			this.list = list;
			index = 0;
			current = default!;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			ListBase<T, TCertain> localList = list;
			if ((uint)index < (uint)localList._size)
			{
				current = localList[index++];
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = list._size + 1;
			current = default!;
			return false;
		}

		public T Current => current;

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list._size + 1)
					throw new InvalidOperationException();
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

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public abstract class BigListBase<T, TCertain, TLow> : IBigList<T> where TCertain : BigListBase<T, TCertain, TLow>, new() where TLow : ListBase<T, TLow>, new()
{
	private protected TLow? low;
	private protected List<TCertain>? high;
	private protected Queue<int> deletedIndexes = new();
	private protected BitList indexDeleted = new();
	private protected mpz_t _size = 0;
	private protected mpz_t deletedCount = 0;
	private protected mpz_t _capacity = 0;
	private protected mpz_t fragment = 1;
	private protected bool isHigh;

	public virtual mpz_t Capacity
	{
		get => _capacity;
		set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value == _capacity)
				return;
			if (value <= 0)
			{
				low = new();
				high = null;
				isHigh = false;
			}
			else if (value <= CapacityFirstStep)
			{
				try
				{
					throw new ExperimentalException();
				}
				catch
				{
				}
				(low, indexDeleted) = GetFirstLists();
				int value2 = (int)value;
				low.Capacity = value2;
				indexDeleted.Capacity = value2;
				high = null;
				isHigh = false;
			}
			else if (!isHigh && low != null)
			{
				fragment = (mpz_t)1 << ((((value - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
				high = new((int)((value + (fragment - 1)) / fragment));
				for (mpz_t i = 0; i < value / fragment; i++)
					high.Add(CapacityCreator(fragment));
				if (value % fragment != 0)
					high.Add(CapacityCreator(value % fragment));
				high[0].AddRange(low);
				low = null;
			}
			else if (high != null)
			{
				high.Capacity = (int)((value + fragment - 1) / fragment);
				high[^1].Capacity = (high.Length < high.Capacity || value % fragment == 0) ? fragment : value % fragment;
				for (int i = high.Length; i < high.Capacity - 1; i++)
					high.Add(CapacityCreator(fragment));
				if (high.Length < high.Capacity)
					high.Add(CapacityCreator(value % fragment == 0 ? fragment : value % fragment));
			}
			_capacity = value;
		}
	}

	private protected abstract Func<mpz_t, TCertain> CapacityCreator { get; }

	private protected virtual int CapacityStepBitLength => 16;

	private protected virtual int CapacityFirstStepBitLength => 16;

	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	private protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected abstract Func<IEnumerable<T>, TLow> CollectionLowCreator { get; }

	private protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	public virtual mpz_t Length => _size - deletedCount;

	public virtual mpz_t Size => _size;

	public virtual T this[mpz_t index]
	{
		get
		{
			if (index >= _size)
				throw new IndexOutOfRangeException();
			return GetInternal(index);
		}
		set
		{
			if (index >= _size)
				throw new IndexOutOfRangeException();
			SetInternal(index, value);
		}
	}

	public virtual void Add(T item)
	{
		if (_size == Capacity && deletedCount == 0) EnsureCapacity(_size + 1);
		if (deletedCount != 0)
		{
			mpz_t index = GetDeletedIndex();
			SetInternal(index, item);
		}
		else
			AddToEnd(item);
	}

	public virtual void AddRange(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is TCertain bigList)
		{
			mpz_t count = bigList.Length;
			if (count == 0)
				return;
			mpz_t offset = new(count < deletedCount ? count : deletedCount);
			for (mpz_t i = 0; i < offset; i++)
			{
				mpz_t index = GetDeletedIndex();
				SetInternal(index, bigList[i]);
			}
			mpz_t count2 = count - offset;
			EnsureCapacity(_size + count2);
			SetRangeInternal(_size, bigList.GetRange(offset, count2));
			_size += count2;
		}
		else
			AddRange(CollectionCreator(collection));
	}

	private protected virtual void AddRangeToEnd(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is TCertain bigList)
		{
			mpz_t count = bigList.Length;
			if (count > 0)
			{
				SetRangeInternal(_size, bigList);
				_size += count;
			}
		}
		else
			AddRangeToEnd(CollectionCreator(collection));
	}

	private protected virtual void AddToEnd(T item)
	{
		if (!isHigh && low != null)
		{
			low.Add(item);
			indexDeleted.Add(false);
		}
		else if (high != null)
			high[(int)(_size / fragment)].AddToEnd(item);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		_size++;
	}

	public virtual void Clear()
	{
		if (!isHigh && low != null)
		{
			low.Clear();
			indexDeleted.Clear();
		}
		else
			high?.Clear();
		deletedIndexes.Clear();
		deletedCount = 0;
	}

	public virtual void Clear(mpz_t index, mpz_t count)
	{
		if (!isHigh && low != null)
			low.Clear((int)index, (int)count);
		else if (high != null)
		{
			int quotient = (int)index.Divide(fragment, out mpz_t remainder);
			int quotient2 = (int)(index + count).Divide(fragment, out mpz_t remainder2);
			if (quotient == quotient2)
			{
				high[quotient].Clear(remainder, remainder2 - remainder);
				return;
			}
			high[quotient].Clear(remainder, fragment - remainder);
			for (int i = quotient + 1; i < quotient2; i++)
				high[i].Clear(0, fragment);
			high[quotient2].Clear(0, remainder2);
		}
	}

	public virtual bool Contains(T item)
	{
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
			if (!isHigh && low != null)
				return low.Contains(item);
			else if (high != null)
				return high.Any(x => x.Contains(item));
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		}
	}

	public virtual void CopyTo(T[] array, int index)
	{
		if (!isHigh && low != null)
			low.CopyTo(array, index);
		else
			throw new InvalidOperationException("Слишком большой список для копирования в массив!");
	}

	private protected virtual void EnsureCapacity(mpz_t min)
	{
		if (_size < min)
		{
			mpz_t newCapacity = _size == 0 ? DefaultCapacity : _size * 2;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
	}

	private protected virtual T GetInternal(mpz_t index)
	{
		if (!isHigh && low != null)
		{
			try
			{
				throw new ExperimentalException();
			}
			catch
			{
			}
			return low.GetInternal((int)index) ?? throw new InvalidOperationException();
		}
		else if (high != null)
			return high.GetInternal((int)(index / fragment)).GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected virtual mpz_t GetDeletedIndex()
	{
		if (!isHigh)
			return deletedIndexes.Dequeue();
		else if (high != null)
		{
			int index = deletedIndexes.Dequeue();
			return fragment * index + high[index].GetDeletedIndex();
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private protected virtual (TLow, BitList) GetFirstLists()
	{
		if (!isHigh && low != null)
			return (low, indexDeleted);
		else if (high != null)
			return high[0].GetFirstLists();
		else
			return new();
	}

	public virtual TCertain GetRange(mpz_t index, mpz_t count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		else if (index == 0 && count == _size && this is TCertain thisList)
			return thisList;
		else if (!isHigh && low != null)
			return CollectionCreator(ListBase<T, TLow>.RemoveIndexes(low.GetRange((int)index, (int)count), deletedIndexes));
		else if (high != null)
		{
			TCertain list = CapacityCreator(count);
			if (index / fragment == (index + count - 1) / fragment)
				list.AddRangeToEnd(high[(int)(index / fragment)].GetRange(index % fragment, count));
			else
			{
				mpz_t offset = index % fragment == 0 ? 0 : fragment - index % fragment;
				if (offset == 0 && count == fragment)
					return high[(int)(index / fragment)];
				if ((int)(index % fragment) != 0)
					list.AddRangeToEnd(high[(int)(index / fragment)].GetRange(index % fragment, offset));
				for (int i = (int)((index + fragment - 1) / fragment); i < (index + count) / fragment && offset <= count - fragment; i++, offset += fragment)
					list.AddRangeToEnd(high[i]);
				if ((index + count) % fragment != 0)
					list.AddRangeToEnd(high[(int)((index + count) / fragment)].GetRange(0, (index + count) % fragment));
			}
			return list;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual mpz_t IndexOf(T item) => IndexOf(item, 0, _size);

	public virtual mpz_t IndexOf(T item, mpz_t index) => IndexOf(item, index, _size - index);

	public virtual mpz_t IndexOf(T item, mpz_t index, mpz_t count)
	{
		if (index > _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0 || index > _size - count)
			throw new ArgumentOutOfRangeException(nameof(count));
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
			for (mpz_t i = index; i < index + count; i++)
				if (GetInternal(i)?.Equals(item) ?? false)
					return i;
			return -1;
		}
	}

	void IBigList<T>.Insert(mpz_t index, T item) => Add(item);

	public virtual bool Remove(T item)
	{
		mpz_t index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual void RemoveAt(mpz_t index)
	{
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index == _size - 1)
		{
			_size--;
			if (!isHigh && low != null)
			{
				low.RemoveAt((int)index);
				indexDeleted.RemoveAt((int)index);
			}
			else
				high?.GetInternal((int)(index / fragment)).RemoveAt(index % fragment);
		}
		else
			RemoveNotFromEnd(index);
	}

	private protected virtual void RemoveNotFromEnd(mpz_t index)
	{
		if (!isHigh && low != null)
		{
			int index2 = (int)index;
			low.SetInternal(index2, default!);
			indexDeleted.SetInternal(index2, true);
			deletedIndexes.Enqueue(index2);
		}
		else if (high != null)
		{
			int highIndex = (int)(index / fragment);
			high.GetInternal(highIndex).RemoveNotFromEnd(index % fragment);
			deletedIndexes.Enqueue(highIndex);
		}
		deletedCount++;
	}

	public virtual void RemoveRange(mpz_t index, mpz_t count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		for (mpz_t i = index + count - 1; i >= index; i--)
			RemoveAt(i);
	}

	private protected virtual void SetInternal(mpz_t index, T value)
	{
		if (!isHigh && low != null)
		{
			try
			{
				throw new ExperimentalException();
			}
			catch
			{
			}
			low.SetInternal((int)index, value);
			indexDeleted.SetInternal((int)index, false);
		}
		else if (high != null)
			high.GetInternal((int)(index / fragment)).SetInternal(index % fragment, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual void SetRange(mpz_t index, IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (index > _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is TCertain bigList)
		{
			if (index + bigList.Length > _size)
				throw new ArgumentException(null);
			EnsureCapacity(index + bigList.Length);
			SetRangeInternal(index, bigList);
		}
		else
			SetRange(index, CollectionLowCreator(collection));
	}

	internal virtual void SetRangeAndSizeInternal(mpz_t index, TCertain list)
	{
		SetRangeInternal(index, list);
		_size = _size >= index + list.Length ? _size : index + list.Length;
	}

	private protected virtual void SetRangeInternal(mpz_t index, TCertain bigList)
	{
		mpz_t count = bigList.Length;
		if (count == 0)
			return;
		if (!isHigh && low != null)
		{
			TLow lowList = CollectionLowCreator(bigList);
			if (index == 0 && count == fragment && lowList.Length == fragment)
				low = lowList;
			else
				low.SetRangeAndSizeInternal((int)index, lowList.Length, lowList);
			indexDeleted.SetRangeAndSizeInternal((int)index, lowList.Length, new((int)count, false));
		}
		else if (high != null)
		{
			if (index % fragment == 0 && count == fragment)
				high[(int)(index / fragment)] = bigList;
			else if (index / fragment == (index + count - 1) / fragment)
				high[(int)(index / fragment)].SetRangeAndSizeInternal(index % fragment, bigList);
			else
			{
				mpz_t offset = index % fragment == 0 ? 0 : fragment - index % fragment;
				if ((int)(index % fragment) != 0)
					high[(int)(index / fragment)].SetRangeAndSizeInternal(index % fragment, bigList.GetRange(0, offset));
				for (int i = (int)((index + fragment - 1) / fragment); i < (index + count) / fragment && offset <= count - fragment; i++, offset += fragment)
					high[i].SetRangeAndSizeInternal(0, bigList.GetRange(offset, fragment));
				if ((index + count) % fragment != 0)
					high[(int)((index + count) / fragment)].SetRangeAndSizeInternal(0, bigList.GetRange(offset, count - offset));
			}
		}
	}

	public virtual T[] ToArray()
	{
		if (!isHigh && low != null)
			return low.ToArray();
		else
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
	}

	public virtual void TrimExcess()
	{
		if (_size <= CapacityFirstStep)
		{
			(low, indexDeleted) = GetFirstLists();
			low.TrimExcess();
		}
		else if (high != null)
		{
			high.TrimExcess();
			high[^1].TrimExcess();
		}
	}

	public virtual bool TryGet(mpz_t index, out T value)
	{
		if (index >= _capacity)
			throw new FormatException();
		if (!isHigh && low != null)
		{
			bool result = !indexDeleted.GetInternal((int)index);
			value = result ? low.GetInternal((int)index) : default!;
			return result;
		}
		else if (high != null)
			return high.GetInternal((int)(index / fragment)).TryGet(index % fragment, out value);
		else
		{
			value = default!;
			return false;
		}
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>, IEnumerator
	{
		private readonly BigListBase<T, TCertain, TLow> list;
		private mpz_t index;
		private T current;

		internal Enumerator(BigListBase<T, TCertain, TLow> list)
		{
			this.list = list;
			index = 0;
			current = default!;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (index >= list._size)
				return MoveNextRare();
			try
			{
				while (!list.TryGet(index, out current))
					index++;
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
			index = list._size + 1;
			current = default!;
			return false;
		}

		public T Current => current;

		object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list._size + 1)
					throw new InvalidOperationException();
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

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public abstract class SetBase<T, TCertain> : ListBase<T, TCertain>, ISet<T>, ICollection where TCertain : SetBase<T, TCertain>, new()
{
	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

	public override TCertain Add(T item)
	{
		TryAdd(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	bool ISet<T>.Add(T item) => TryAdd(item);

	public override Span<T> AsSpan(int index, int count) => throw new NotSupportedException();

	private protected override void ClearInternal(int index, int count)
	{
		for (int i = 0; i < count; i++)
			SetInternal(index + i, default!);
	}

	public override bool Contains(T? item, int index, int count) => item != null && IndexOf(item, index, count) >= 0;

	public virtual void ExceptWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = new SlowDeletionHashSet<T>(other);
		FilterInPlace(x => !set.Contains(x));
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
			set = new SlowDeletionHashSet<T>(other);
		FilterInPlace(set.Contains);
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
		TCertain temp = CollectionCreator(other);
		temp.ExceptWith(this);
		ExceptWith(other);
		AddRange(temp);
	}

	public abstract bool TryAdd(T item);

	public virtual void UnionWith(IEnumerable<T> other)
	{
		foreach (T item in other)
			TryAdd(item);
	}
}
