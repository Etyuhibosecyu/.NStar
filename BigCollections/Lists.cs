using Mpir.NET;
#if !RELEASE
using NativeFunctions;
#endif
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using G = System.Collections.Generic;
using static BigCollections.Extents;
using static System.Math;

namespace BigCollections;

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public abstract class ListBase<T, TCertain> : IList<T>, IList, IReadOnlyList<T>, IDisposable/*, IComparable<ListBase<T, TCertain>>, IEquatable<ListBase<T, TCertain>>*/ where TCertain : ListBase<T, TCertain>, new()
{
	private protected int _size;
	[NonSerialized]
	private protected object _syncRoot = new();

	public abstract int Capacity { get; set; }

	protected abstract Func<int, TCertain> CapacityCreator { get; }

	protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	public virtual int Count => _size;

	protected virtual int DefaultCapacity => 4;

	bool IList.IsFixedSize => false;

	bool ICollection<T>.IsReadOnly => false;

	bool IList.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => _syncRoot;

	public virtual T this[Index index, bool invoke = true]
	{
		get
		{
			int index2 = index.IsFromEnd ? _size - index.Value : index.Value;
			if ((uint)index2 >= (uint)_size)
				throw new ArgumentOutOfRangeException(nameof(index));
			return GetInternal(index2, invoke);
		}
		set
		{
			int index2 = index.IsFromEnd ? _size - index.Value : index.Value;
			if ((uint)index2 >= (uint)_size)
				throw new ArgumentOutOfRangeException(nameof(index));
			SetInternal(index2, value);
		}
	}

	public virtual TCertain this[Range range] => GetRange(range);

	T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

	T IReadOnlyList<T>.this[int index] => this[index];

	object? IList.this[int index]
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

	public virtual TCertain Add(T item)
	{
		if (_size == Capacity) EnsureCapacity(_size + 1);
		SetInternal(_size++, item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	void ICollection<T>.Add(T item) => Add(item);

	int IList.Add(object? item)
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

	public virtual TCertain AddRange(IEnumerable<T> collection) => InsertRange(_size, collection);

	public virtual TCertain Append(T item) => Create(this, CollectionCreator).Add(item);

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

	protected virtual void ClearInternal() => ClearInternal(0, _size);

	protected abstract void ClearInternal(int index, int count);

	public virtual TCertain Concat(TCertain collection) => Create(this, CollectionCreator).AddRange(collection);

	//public virtual int CompareTo(ListBase<T, TCertain>? other)
	//{
	//	if (other == null || other is not TCertain m)
	//		return 1;
	//	else
	//		return CompareToInternal(m);
	//}

	//protected virtual int CompareToInternal(TCertain other)
	//{
	//	int c;
	//	for (int i = 0; i < _size && i < other._size; i++)
	//		if ((c = ((IComparable<T>?)GetInternal(i) ?? throw new InvalidOperationException()).CompareTo(other.GetInternal(i))) != 0)
	//			return c;
	//	return _size.CompareTo(other._size);
	//}

	public virtual bool Contains(T? item)
	{
		if (item == null)
		{
			for (int i = 0; i < _size; i++)
				if (this[i] == null)
					return true;
			return false;
		}
		else
		{
			EqualityComparer<T> c = EqualityComparer<T>.Default;
			for (int i = 0; i < _size; i++)
				if (c.Equals(this[i], item))
					return true;
			return false;
		}
	}

	bool IList.Contains(object? item)
	{
		if (IsCompatibleObject(item))
			return Contains((T?)item);
		return false;
	}

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

	protected abstract void Copy(ListBase<T, TCertain> source, int sourceIndex, ListBase<T, TCertain> destination, int destinationIndex, int count);

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

	protected abstract void CopyToInternal(Array array, int arrayIndex);

	protected abstract void CopyToInternal(int index, T[] array, int arrayIndex, int count);

	public abstract void Dispose();

	protected virtual void EnsureCapacity(int min)
	{
		if (Capacity < min)
		{
			int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
	}

	//public override bool Equals(object? obj)
	//{
	//	if (obj == null || obj is not TCertain m)
	//		return false;
	//	else if (_size != m._size)
	//		return false;
	//	else
	//		return EqualsInternal(m);
	//}

	//bool IEquatable<ListBase<T, TCertain>>.Equals(ListBase<T, TCertain>? other) => Equals(other);

	//protected virtual bool EqualsInternal(TCertain other)
	//{
	//	for (int i = 0; i < _size; i++)
	//		if (!(GetInternal(i)?.Equals(other.GetInternal(i)) ?? false))
	//			return false;
	//	return true;
	//}

	public virtual bool Exists(Predicate<T> match) => FindIndex(match) != -1;

	public virtual TCertain Filter(Func<T, bool> match)
	{
		TCertain result = Create(_size, CapacityCreator);
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
		TCertain result = Create(_size, CapacityCreator);
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

	public virtual void ForEach(Action<T> action)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));
		for (int i = 0; i < _size; i++)
			action(this[i]);
	}

	internal abstract T GetInternal(int index, bool invoke = true);

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	//public override int GetHashCode()
	//{
	//	return _size < 3 ? 1234567890 : (GetInternal(0)?.GetHashCode() ?? 0) ^ (GetInternal(1)?.GetHashCode() ?? 0) ^ (GetInternal(_size - 1)?.GetHashCode() ?? 0);
	//}

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
		TCertain list = Create(count, CapacityCreator);
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

	int IList.IndexOf(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return IndexOf((T)item);
		return -1;
	}

	protected abstract int IndexOfInternal(T item, int index, int count);

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

	void IList<T>.Insert(int index, T item) => Insert(index, item);

	void IList.Insert(int index, object? item)
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

	public virtual TCertain InsertRange(int index, IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		return InsertRangeInternal(index, collection);
	}

	protected virtual TCertain InsertRangeInternal(int index, IEnumerable<T> collection)
	{
		if (collection is TCertain list)
		{
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
		else
			return InsertRangeInternal(index, Create(collection, CollectionCreator));
	}

	protected static bool IsCompatibleObject(object? value) => (value is T) || (value == null && default(T) == null);

	public virtual int LastIndexOf(T item)
	{
		if (_size == 0)
			return -1;
		else
			return LastIndexOf(item, _size - 1, _size);
	}

	public virtual int LastIndexOf(T item, int index)
	{
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		return LastIndexOf(item, index, index + 1);
	}

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

	protected abstract int LastIndexOfInternal(T item, int index, int count);

	public virtual bool Remove(T item)
	{
		int index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	void IList.Remove(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				Remove((T)item);
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

	void IList.RemoveAt(int index) => RemoveAt(index);

	void IList<T>.RemoveAt(int index) => RemoveAt(index);

	internal static TCertain RemoveIndexes(TCertain originalList, Queue<int> toRemove)
	{
		List<int> toRemove2 = toRemove.ToList().Sort();
		TCertain result = Create(originalList._size - toRemove2._size, originalList.CapacityCreator);
		int pos = 0;
		for (int i = 0; i < toRemove2._size; i++)
		{
			result.Copy(originalList, pos, result, pos - i, toRemove2[i] - pos);
			pos = toRemove2[i] + 1;
		}
		result._size = originalList._size - toRemove2._size;
		return result;
	}

	public virtual TCertain RemoveRange(int index, int count)
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
			return ReplaceRange(index, count, Create(collection, CollectionCreator));
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

	protected abstract TCertain ReverseInternal(int index, int count);

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
			return SetRange(index, Create(collection, CollectionCreator));
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
		T[] array = new T[_size];
		CopyToInternal(0, array, 0, _size);
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
			new_list.Add(Create(list._size, list[0].CapacityCreator));
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

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public abstract class BigListBase<T, TCertain, TLow> : IBigList<T> where TCertain : BigListBase<T, TCertain, TLow>, new() where TLow : ListBase<T, TLow>, new()
{
	private protected TLow? low;
	private protected List<TCertain>? high;
	private protected Queue<int> deletedIndexes = new();
	private protected BitList2 indexDeleted = new();
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
					high.Add(Create(fragment, CapacityCreator));
				if (value % fragment != 0)
					high.Add(Create(value % fragment, CapacityCreator));
				high[0].AddRange(low);
				low = null;
			}
			else if (high != null)
			{
				high.Capacity = (int)((value + fragment - 1) / fragment);
				high[^1].Capacity = (high.Count < high.Capacity || value % fragment == 0) ? fragment : value % fragment;
				for (int i = high.Count; i < high.Capacity - 1; i++)
					high.Add(Create(fragment, CapacityCreator));
				if (high.Count < high.Capacity)
					high.Add(Create(value % fragment == 0 ? fragment : value % fragment, CapacityCreator));
			}
			_capacity = value;
		}
	}

	public virtual mpz_t Count => _size - deletedCount;

	protected abstract Func<mpz_t, TCertain> CapacityCreator { get; }

	protected virtual int CapacityStepBitLength => 16;

	protected virtual int CapacityFirstStepBitLength => 16;

	protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	protected abstract Func<IEnumerable<T>, TLow> CollectionLowCreator { get; }

	protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	public virtual mpz_t Size => _size;

	public virtual T this[mpz_t index]
	{
		get
		{
			if (index >= _size)
				throw new ArgumentOutOfRangeException(nameof(index));
			return GetInternal(index);
		}
		set
		{
			if (index >= _size)
				throw new ArgumentOutOfRangeException(nameof(index));
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
			mpz_t count = bigList.Count;
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
			AddRange(Create(collection, CollectionCreator));
	}

	protected virtual void AddRangeToEnd(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is TCertain bigList)
		{
			mpz_t count = bigList.Count;
			if (count > 0)
			{
				SetRangeInternal(_size, bigList);
				_size += count;
			}
		}
		else
			AddRangeToEnd(Create(collection, CollectionCreator));
	}

	protected virtual void AddToEnd(T item)
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
		else if (high != null)
			high.Clear();
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

	protected virtual void EnsureCapacity(mpz_t min)
	{
		if (_size < min)
		{
			mpz_t newCapacity = _size == 0 ? DefaultCapacity : _size * 2;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
	}

	protected virtual T GetInternal(mpz_t index)
	{
		if (!isHigh && low != null)
			return low.GetInternal((int)index) ?? throw new InvalidOperationException();
		else if (high != null)
			return high.GetInternal((int)(index / fragment)).GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	protected virtual mpz_t GetDeletedIndex()
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

	private protected virtual (TLow, BitList2) GetFirstLists()
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
			return Create(ListBase<T, TLow>.RemoveIndexes(low.GetRange((int)index, (int)count), deletedIndexes), CollectionCreator);
		else if (high != null)
		{
			TCertain list = Create(count, CapacityCreator);
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
			else if (high != null)
				high.GetInternal((int)(index / fragment)).RemoveAt(index % fragment);
		}
		else
			RemoveNotFromEnd(index);
	}

	protected virtual void RemoveNotFromEnd(mpz_t index)
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

	protected virtual void SetInternal(mpz_t index, T value)
	{
		if (!isHigh && low != null)
		{
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
			if (index + bigList.Count > _size)
				throw new ArgumentException(null);
			EnsureCapacity(index + bigList.Count);
			SetRangeInternal(index, bigList);
		}
		else
			SetRange(index, Create(collection, CollectionLowCreator));
	}

	internal virtual void SetRangeAndSizeInternal(mpz_t index, TCertain list)
	{
		SetRangeInternal(index, list);
		_size = _size >= index + list.Count ? _size : index + list.Count;
	}

	protected virtual void SetRangeInternal(mpz_t index, TCertain bigList)
	{
		mpz_t count = bigList.Count;
		if (count == 0)
			return;
		if (!isHigh && low != null)
		{
			TLow lowList = Create(bigList, CollectionLowCreator);
			if (index == 0 && count == fragment && lowList.Count == fragment)
				low = lowList;
			else
				low.SetRangeAndSizeInternal((int)index, lowList.Count, lowList);
			indexDeleted.SetRangeAndSizeInternal((int)index, lowList.Count, new((int)count, false));
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
			if (index < list._size)
			{
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
#if RELEASE

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public class BitList : ListBase<bool, BitList>, ICloneable
{
	private uint[] _items;

	private const int _shrinkThreshold = 256;

	// XPerY=n means that n Xs can be stored in 1 Y. 
	private const int BitsPerInt = sizeof(int) * BitsPerByte;
	private const int BytesPerInt = sizeof(int);
	private const int BitsPerByte = 8;

	private static readonly uint[] _emptyArray = Array.Empty<uint>();

	public BitList()
	{
		_items = _emptyArray;
		_size = 0;
	}

	public BitList(int capacity)
	{
		if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity == 0)
			_items = _emptyArray;
		else
			_items = new uint[GetArrayLength(capacity, BitsPerInt)];
	}

	public BitList(int length, bool defaultValue)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		_items = new uint[GetArrayLength(length, BitsPerInt)];
		_size = length;
		uint fillValue = defaultValue ? 0xffffffff : 0;
		for (int i = 0; i < _items.Length; i++)
			_items[i] = fillValue;
	}

	public BitList(uint[] values)
	{
		if (values == null)
			throw new ArgumentNullException(nameof(values));
		// this value is chosen to prevent overflow when computing m_length
		if (values.Length > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = new uint[values.Length];
		_size = values.Length * BitsPerInt;
		Array.Copy(values, _items, values.Length);
	}

	public BitList(IEnumerable bits)
	{
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BitList bitList)
		{
			int arrayLength = GetArrayLength(bitList._size, BitsPerInt);
			_items = new uint[arrayLength];
			_size = bitList._size;
			Array.Copy(bitList._items, _items, arrayLength);
		}
		else if (bits is BitArray bitArray)
		{
			int arrayLength = GetArrayLength(bitArray.Length, BitsPerInt);
			_items = new uint[arrayLength];
			_size = bitArray.Length;
			bitArray.CopyTo(_items, 0);
		}
		else if (bits is byte[] byteArray)
		{
			if (byteArray.Length > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bits));
			_items = new uint[GetArrayLength(byteArray.Length, BytesPerInt)];
			_size = byteArray.Length * BitsPerByte;
			int i = 0;
			int j = 0;
			while (byteArray.Length - j >= 4)
			{
				_items[i++] = (uint)((byteArray[j] & 0xff) |
							  ((byteArray[j + 1] & 0xff) << 8) |
							  ((byteArray[j + 2] & 0xff) << 16) |
							  ((byteArray[j + 3] & 0xff) << 24));
				j += 4;
			}
			switch (byteArray.Length - j)
			{
				case 3:
					_items[i] = (uint)(byteArray[j + 2] & 0xff) << 16;
					goto case 2;
				case 2:
					_items[i] |= (uint)(byteArray[j + 1] & 0xff) << 8;
					goto case 1;
				case 1:
					_items[i] |= (uint)(byteArray[j] & 0xff);
					break;
			}
		}
		else if (bits is bool[] boolArray)
		{
			_items = new uint[GetArrayLength(boolArray.Length, BitsPerInt)];
			_size = boolArray.Length;
			for (int i = 0; i < boolArray.Length; i++)
				if (boolArray[i])
					_items[i / BitsPerInt] |= (uint)1 << (i % BitsPerInt);
		}
		else if (bits is IEnumerable<int> ints)
		{
			_items = ints.Select(x => (uint)x).ToArray();
			_size = _items.Length * BitsPerInt;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			_items = uints.ToArray();
			_size = _items.Length * BitsPerInt;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			if (!bytes.TryGetNonEnumeratedCount(out int count))
				count = bytes.Count();
			if (count > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bits));
			int arrayLength = GetArrayLength(count, BytesPerInt);
			_items = new uint[arrayLength];
			int i = 0;
			using IEnumerator<byte> en = bytes.GetEnumerator();
			while (en.MoveNext())
			{
				(int index, int remainder) = DivRem(i++, BytesPerInt);
				_items[index] |= (uint)en.Current << remainder * BitsPerByte;
			}
			_size = count * BitsPerByte;
		}
		else if (bits is IEnumerable<bool> bools)
		{
			if (!bools.TryGetNonEnumeratedCount(out int count))
				count = bools.Count();
			int arrayLength = GetArrayLength(count, BitsPerInt);
			_items = new uint[arrayLength];
			int i = 0;
			using IEnumerator<bool> en = bools.GetEnumerator();
			while (en.MoveNext())
			{
				(int index, int remainder) = DivRem(i++, BitsPerInt);
				if (en.Current)
					_items[index] |= (uint)1 << remainder;
			}
			_size = count;
		}
		else
			throw new ArgumentException(null, nameof(bits));
	}

	public override int Capacity
	{
		get => _items.Length * BitsPerInt;
		set
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value));
			int newints = GetArrayLength(value, BitsPerInt);
			if (newints > _items.Length || newints + _shrinkThreshold < _items.Length)
			{
				uint[] newarray = new uint[newints];
				Array.Copy(_items, newarray, newints > _items.Length ? _items.Length : newints);
				_items = newarray;
			}
			if (value > _size)
			{
				int last = GetArrayLength(_size, BitsPerInt) - 1;
				int bits = _size % BitsPerInt;
				if (bits > 0)
					_items[last] &= ((uint)1 << bits) - 1;
				Array.Clear(_items, last + 1, newints - last - 1);
			}
			ListChanged?.Invoke(this);
		}
	}

	protected override Func<int, BitList> CapacityCreator => CapacityCreatorStatic;

	private static Func<int, BitList> CapacityCreatorStatic => capacity => new(capacity);

	protected override Func<IEnumerable<bool>, BitList> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<bool>, BitList> CollectionCreatorStatic => collection => new(collection);

	protected override int DefaultCapacity => 64;

	public event ListChangedHandler? ListChanged;

	internal override bool GetInternal(int index, bool invoke = true)
	{
		bool item = (_items[index / BitsPerInt] & (1 << (index % BitsPerInt))) != 0;
		if (invoke)
			ListChanged?.Invoke(this);
		return item;
	}

	internal override void SetInternal(int index, bool value)
	{
		if (value)
			_items[index / BitsPerInt] |= (uint)1 << (index % BitsPerInt);
		else
			_items[index / BitsPerInt] &= ~((uint)1 << (index % BitsPerInt));
		ListChanged?.Invoke(this);
	}

	public void SetAll(bool value)
	{
		uint fillValue = value ? 0xffffffff : 0;
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] = fillValue;
	}

	public BitList And(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] &= value._items[i];
		return this;
	}

	public BitList Or(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] |= value._items[i];
		return this;
	}

	public BitList Xor(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] ^= value._items[i];
		return this;
	}

	public BitList Not()
	{
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] = ~_items[i];
		return this;
	}

	protected override void ClearInternal(int index, int count)
	{
		(int startIndex, int startRemainder) = DivRem(index, BitsPerInt);
		uint startMask = ((uint)1 << startRemainder) - 1;
		(int endIndex, int endRemainder) = DivRem(index + count, BitsPerInt);
		uint endMask = ~0u << endRemainder;
		if (startIndex == endIndex)
			_items[startIndex] &= startMask | endMask;
		else
		{
			_items[startIndex] &= startMask;
			for (int i = startIndex + 1; i < endIndex; i++)
				_items[i] = 0;
			_items[endIndex] &= endMask;
		}
		ListChanged?.Invoke(this);
	}

	//protected override int CompareToInternal(BitList other)
	//{
	//	int count = Min(_size, other._size), c;
	//	(int intCount, int bitsCount) = DivRem(count, BitsPerInt);
	//	for (int i = 0; i < intCount; i++)
	//		if ((c = _items[i].CompareTo(other._items[i])) != 0)
	//			return c;
	//	if (bitsCount != 0)
	//	{
	//		uint mask = ((uint)1 << bitsCount) - 1;
	//		if ((c = (_items[intCount] & mask).CompareTo(other._items[intCount] & mask)) != 0)
	//			return c;
	//	}
	//	return _size.CompareTo(other._size);
	//}

	public override bool Contains(bool item)
	{
		int emptyValue = item ? 0 : unchecked((int)0xffffffff);
		for (int i = 0; i < _size / BitsPerInt; i++)
			if (_items[i] != emptyValue)
				return true;
		int rest = _size / BitsPerInt;
		if (rest != 0)
			if ((_items[^1] & (1 << rest)) != (item ? 0 : (1 << rest) - 1))
				return true;
		return false;
	}

	public static void CopyBits(IList<uint> sourceBits, int sourceIndex, IList<uint> destinationBits, int destinationIndex, int length)
	{
		CheckParams(sourceBits, sourceIndex, destinationBits, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		(int sourceIntIndex, int sourceBitsIndex) = DivRem(sourceIndex, BitsPerInt);               // Целый индех в исходном массиве.
		(int destinationIntIndex, int destinationBitsIndex) = DivRem(destinationIndex, BitsPerInt);     // Целый индекс в целевом массиве.
		int bitsOffset = destinationBitsIndex - sourceBitsIndex;    // Битовое смещение.
		int intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		uint sourceStartMask = ~0u << sourceBitsIndex; // Маска "головы" источника
		int destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		(int destinationEndIntIndex, int destinationEndBitsIndex) = DivRem(destinationEndIndex, BitsPerInt);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			uint buff = sourceBits[sourceIntIndex] & sourceStartMask;
			uint destinationMask = (~(~0u << length)) << destinationBitsIndex;
			if (bitsOffset >= 0)
				buff <<= bitsOffset;
			else
			{
				buff >>= -bitsOffset;
				if (length + sourceBitsIndex > BitsPerInt)
					buff |= sourceBits[sourceIntIndex + 1] << (BitsPerInt + bitsOffset);
			}
			buff &= destinationMask;
			destinationBits[destinationIntIndex] &= ~destinationMask;
			destinationBits[destinationIntIndex] |= buff;
		}
		else if (sourceIndex >= destinationIndex)
		{
			if (bitsOffset < 0)
			{
				bitsOffset = -bitsOffset;
				ulong buff = destinationBits[destinationIntIndex];
				buff &= ((ulong)1 << destinationBitsIndex) - 1;
				buff |= (ulong)(sourceBits[sourceIntIndex] & sourceStartMask) >> bitsOffset;
				int sourceEndIntIndex = (sourceIndex + length - 1) / BitsPerInt; // Индекс инта "хвоста".
				for (int sourceCurrentIntIndex = sourceIntIndex + 1; sourceCurrentIntIndex <= sourceEndIntIndex; sourceCurrentIntIndex++)
				{
					buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << (BitsPerInt - bitsOffset);
					destinationBits[sourceCurrentIntIndex + intOffset - 1] = (uint)buff;
					buff >>= BitsPerInt;
				}
				if (sourceEndIntIndex + intOffset < destinationBits.Count)
				{
					ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
					buff &= destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] &= (uint)~destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] |= (uint)buff;
				}
			}
			else
			{
				ulong buff = destinationBits[destinationIntIndex];
				buff &= ((ulong)1 << destinationBitsIndex) - 1;
				buff |= ((ulong)(sourceBits[sourceIntIndex] & sourceStartMask)) << bitsOffset;
				int sourceEndIntIndex = (sourceIndex + length - 1) / BitsPerInt; // Индекс инта "хвоста".
				for (int sourceCurrentIntIndex = sourceIntIndex; sourceCurrentIntIndex < sourceEndIntIndex; sourceCurrentIntIndex++)
				{
					destinationBits[sourceCurrentIntIndex + intOffset] = (uint)buff;
					buff >>= BitsPerInt;
					if (sourceCurrentIntIndex + 1 < sourceBits.Count) buff |= ((ulong)sourceBits[sourceCurrentIntIndex + 1]) << bitsOffset;
				}
				if (sourceEndIntIndex + intOffset < destinationBits.Count)
				{
					ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
					buff &= destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] &= (uint)~destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] |= (uint)buff;
				}
			}
		}
		else
		{
			var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
			var sourceEndBitsIndex = sourceEndIndex % BitsPerInt; // Индекс последнего бита в инт.
			var sourceEndIntIndex = sourceEndIndex / BitsPerInt;  // Индекс инта последнего бита.
			uint sourceEndMask = ~0u >> (BitsPerInt - sourceEndBitsIndex - 1); // Маска "хвоста" источника
			if (bitsOffset < 0)
			{
				bitsOffset = -bitsOffset;
				ulong buff = destinationBits[destinationEndIntIndex];
				buff &= ~0ul << (destinationEndBitsIndex + 1);
				buff <<= BitsPerInt;
				buff |= ((ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask)) << (BitsPerInt - bitsOffset);
				for (int sourceCurrentIntIndex = sourceEndIntIndex; sourceCurrentIntIndex > sourceIntIndex; sourceCurrentIntIndex--)
				{
					destinationBits[sourceCurrentIntIndex + intOffset] = (uint)(buff >> BitsPerInt);
					buff <<= BitsPerInt;
					buff |= ((ulong)sourceBits[sourceCurrentIntIndex - 1]) << (BitsPerInt - bitsOffset);
				}
				ulong destinationMask = ~0ul << (BitsPerInt + destinationBitsIndex);
				buff &= destinationMask;
				destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> BitsPerInt);
				destinationBits[destinationIntIndex] |= (uint)(buff >> BitsPerInt);
			}
			else
			{
				ulong buff = destinationBits[destinationEndIntIndex];
				buff &= ~0ul << (destinationEndBitsIndex + 1);
				buff <<= BitsPerInt;
				buff |= (ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask) << (BitsPerInt + bitsOffset);
				for (int sourceCurrentIntIndex = sourceEndIntIndex - 1; sourceCurrentIntIndex >= sourceIntIndex; sourceCurrentIntIndex--)
				{
					buff |= (ulong)sourceBits[sourceCurrentIntIndex] << bitsOffset;
					destinationBits[sourceCurrentIntIndex + intOffset + 1] = (uint)(buff >> BitsPerInt);
					buff <<= BitsPerInt;
				}
				ulong destinationMask = ~0ul << (BitsPerInt + destinationBitsIndex);
				buff &= destinationMask;
				destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> BitsPerInt);
				destinationBits[destinationIntIndex] |= (uint)(buff >> BitsPerInt);
			}
		}
	}

	private static void CheckParams(IList<uint> sourceBits, int sourceIndex, IList<uint> destinationBits, int destinationIndex, int length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Count == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Count == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		int sourceLengthBits = sourceBits.Count * BitsPerInt; // Длина массивов в битах.
		int destinationLengthBits = destinationBits.Count * BitsPerInt; // Длина массивов в битах.
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceLengthBits)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationLengthBits)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	public object Clone()
	{
		BitList bitList = new(_items) { _size = _size };
		return bitList;
	}

	protected override void Copy(ListBase<bool, BitList> source, int sourceIndex, ListBase<bool, BitList> destination, int destinationIndex, int count)
	{
		BitList destination2 = destination as BitList ?? throw new ArgumentException(null, nameof(destination));
		CopyBits((source as BitList ?? throw new ArgumentException(null, nameof(source)))._items, sourceIndex, destination2._items, destinationIndex, count);
		destination2.ListChanged?.Invoke(this);
	}

	protected override void CopyToInternal(Array array, int index)
	{
		if (array.Rank != 1)
			throw new RankException();
		if (array is int[])
			Array.Copy(_items, 0, array, index, GetArrayLength(_size, BitsPerInt));
		else if (array is byte[] byteArray)
		{
			int arrayLength = GetArrayLength(_size, BitsPerByte);
			if (array.Length - index < arrayLength)
				throw new ArgumentException(null);
			byte[] b = byteArray;
			int mask = unchecked((1 << BitsPerByte) - 1);
			for (int i = 0; i < arrayLength; i++)
				b[index + i] = (byte)((_items[i / BytesPerInt] >> (i % BytesPerInt * BitsPerByte)) & mask); // Shift to bring the required byte to LSB, then mask
		}
		else if (array is bool[] boolArray)
			CopyToInternal(boolArray, index);
		else
			throw new ArgumentException(null, nameof(array));
	}

	private void CopyToInternal(bool[] array, int index)
	{
		if (array.Length - index < _size)
			throw new ArgumentException(null);
		CopyToInternal(index, array, 0, array.Length);
	}

	protected override void CopyToInternal(int index, bool[] array, int arrayIndex, int count)
	{
		for (int i = 0; i < count; i++)
			array[arrayIndex + i] = ((_items[(index + i) / BitsPerInt] >> ((index + i) % BitsPerInt)) & 0x00000001) != 0;
	}

	public override void Dispose() => GC.SuppressFinalize(this);

	//protected override bool EqualsInternal(BitList other)
	//{
	//	int count = Min(_size, other._size);
	//	(int intCount, int bitsCount) = DivRem(count, BitsPerInt);
	//	for (int i = 0; i < intCount; i++)
	//		if (_items[i] != other._items[i])
	//			return false;
	//	if (bitsCount != 0)
	//	{
	//		uint mask = ((uint)1 << bitsCount) - 1;
	//		if ((_items[intCount] & mask) != (other._items[intCount] & mask))
	//			return false;
	//	}
	//	return true;
	//}

	//public override int GetHashCode()
	//{
	//	return _items.Length < 3 ? 1234567890 : _items[0].GetHashCode() ^ _items[1].GetHashCode() ^ _items[^1].GetHashCode();
	//}

	public virtual uint GetSmallRange(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		if (count > BitsPerInt)
			throw new ArgumentException(null, nameof(count));
		(int quotient, int remainder) = DivRem(index, BitsPerInt);
		(int quotient2, int remainder2) = DivRem(index + count, BitsPerInt);
		uint result;
		if (quotient == quotient2)
			result = _items[quotient] & (~(~0u << count) << remainder);
		else
		{
			result = _items[quotient] & (~0u << remainder);
			result |= (_items[quotient + 1] & (((uint)1 << remainder2) - 1)) << (BitsPerInt - remainder2);
		}
		return result;
	}

	protected override int IndexOfInternal(bool item, int index, int count)
	{
		int fillValue = item ? 0 : unchecked((int)0xffffffff);
		int startIndex = GetArrayLength(index, BitsPerInt), endIndex = (index + count) / BitsPerInt;
		int startRemainder = index % BitsPerInt;
		int endRemainder = (index + count) % BitsPerInt;
		if (startIndex == endIndex && startRemainder != 0)
			for (int i = startRemainder; i < endRemainder; i++)
				if ((_items[startIndex] & (1 << i)) != 0)
					return startIndex * BitsPerInt + i;
		if (startRemainder != 0)
		{
			int invRemainder = BitsPerInt - startRemainder;
			int mask = (1 << invRemainder) - 1;
			uint first = _items[startIndex - 1] >> startRemainder;
			if (first != (item ? 0 : mask))
				for (int i = 0; i < invRemainder; i++)
					if ((first & (1 << i)) != 0)
						return index + i;
		}
		for (int i = startIndex; i < endIndex; i++)
			if (_items[i] != fillValue)
				for (int j = 0; j < BitsPerInt; j++)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		if (endRemainder != 0)
			for (int i = 0; i < endRemainder; i++)
				if ((_items[endIndex] & (1 << i)) != 0)
					return endIndex * BitsPerInt + i;
		return -1;
	}

	public void InsertRange(int index, IEnumerable collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is BitList bitList)
		{
			int count = bitList._size;
			if (count == 0)
				return;
			EnsureCapacity(_size + count);
			CopyBits(_items, index, _items, index + count, _size - index);
			CopyBits(bitList._items, 0, _items, index, count);
			_size += count;
		}
		else if (collection is uint[] intArray)
		{
			int count = intArray.Length * BitsPerInt;
			if (count == 0)
				return;
			EnsureCapacity(_size + count);
			CopyBits(_items, index, _items, index + count, _size - index);
			CopyBits(intArray, 0, _items, index, count);
			_size += count;
		}
		else
			InsertRange(index, new BitList(collection));
	}

	protected override int LastIndexOfInternal(bool item, int index, int count)
	{
		int fillValue = item ? 0 : unchecked((int)0xffffffff);
		int startIndex = GetArrayLength(index, BitsPerInt), endIndex = (index + count) / BitsPerInt;
		int startRemainder = index % BitsPerInt;
		int endRemainder = (index + count) % BitsPerInt;
		if (startIndex == endIndex && startRemainder != 0)
			for (int i = endRemainder - 1; i >= startRemainder; i--)
				if ((_items[startIndex] & (1 << i)) != 0)
					return startIndex * BitsPerInt + i;
		if (endRemainder != 0)
			for (int i = endRemainder - 1; i >= 0; i--)
				if ((_items[endIndex] & (1 << i)) != 0)
					return endIndex * BitsPerInt + i;
		for (int i = endIndex - 1; i >= startIndex; i--)
			if (_items[i] != fillValue)
				for (int j = BitsPerInt; j >= 0; j--)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		if (startRemainder != 0)
		{
			int invRemainder = BitsPerInt - startRemainder;
			int mask = (1 << invRemainder) - 1;
			uint first = _items[startIndex - 1] >> startRemainder;
			if (first != (item ? 0 : mask))
				for (int i = invRemainder - 1; i >= 0; i--)
					if ((first & (1 << i)) != 0)
						return index + i;
		}
		return -1;
	}

	protected override BitList ReverseInternal(int index, int count)
	{
		for (int i = 0; i < count / 2; i++)
		{
			(this[index + i], this[index + count - i - 1]) = (this[index + count - i - 1], this[index + i]);
		}
		ListChanged?.Invoke(this);
		return this;
	}

	public void SetRange(int index, IEnumerable collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is BitList bitList)
		{
			int count = bitList._size;
			if (index + count > _size)
				throw new ArgumentException(null);
			if (count > 0)
				CopyBits(bitList._items, 0, _items, index, count);
		}
		else
			SetRange(index, new BitList(collection));
	}

	public List<uint> ToUIntList() => _items.Take(GetArrayLength(_size, BitsPerInt));

	public object SyncRoot
	{
		get
		{
			if (_syncRoot == null)
				Interlocked.CompareExchange<object?>(ref _syncRoot, new(), null);
			return _syncRoot;
		}
	}

	public bool IsReadOnly => false;

	public bool IsSynchronized => false;
}

internal class BitList2 : BitList
{
	public BitList2() : base()
	{
	}

	public BitList2(int capacity) : base(capacity)
	{
	}

	public BitList2(int length, bool defaultValue) : base(length, defaultValue)
	{
	}

	public BitList2(IEnumerable bits) : base(bits)
	{
	}
}
#else

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public unsafe class BitList : ListBase<bool, BitList>, ICloneable
{
	private uint* _items;
	private int _capacity = 0;

	private const int _shrinkThreshold = 256;

	// XPerY=n means that n Xs can be stored in 1 Y. 
	private const int BitsPerInt = sizeof(int) * BitsPerByte;
	private const int BytesPerInt = sizeof(int);
	private const int BitsPerByte = 8;

	private static readonly uint* _emptyPointer = null;

	public BitList()
	{
		_items = _emptyPointer;
		_size = 0;
	}

	public BitList(int capacity)
	{
		if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity == 0)
			_items = _emptyPointer;
		else
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(capacity, BitsPerInt)));
	}

	public BitList(int length, bool defaultValue)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(length, BitsPerInt)));
		_size = length;
		uint fillValue = defaultValue ? 0xffffffff : 0;
		for (int i = 0; i < _capacity; i++)
			_items[i] = fillValue;
	}

	public BitList(int length, uint* ptr)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(length, BitsPerInt)));
		_size = length;
		CopyMemory(ptr, _items, length);
	}

	public BitList(uint[] values)
	{
		if (values == null)
			throw new ArgumentNullException(nameof(values));
		// this value is chosen to prevent overflow when computing m_length
		if (values.Length > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = values.Length));
		_size = values.Length * BitsPerInt;
		fixed (uint* values2 = values)
			CopyMemory(values2, _items, values.Length);
	}

	public BitList(IEnumerable bits)
	{
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BitList bitList)
		{
			int arrayLength = _capacity = GetArrayLength(bitList._size, BitsPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			_size = bitList._size;
			CopyMemory(bitList._items, _items, arrayLength);
		}
		else if (bits is BitArray bitArray)
		{
			int arrayLength = _capacity = GetArrayLength(bitArray.Length, BitsPerInt);
			int[] array = new int[arrayLength];
			bitArray.CopyTo(array, 0);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			fixed (int* array2 = array)
				CopyMemory((uint*)array2, _items, arrayLength);
			_size = bitArray.Length;
		}
		else if (bits is byte[] byteArray)
		{
			if (byteArray.Length > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bits));
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(byteArray.Length, BytesPerInt)));
			_size = byteArray.Length * BitsPerByte;
			int i = 0;
			int j = 0;
			while (byteArray.Length - j >= 4)
			{
				_items[i++] = (uint)((byteArray[j] & 0xff) |
							  ((byteArray[j + 1] & 0xff) << 8) |
							  ((byteArray[j + 2] & 0xff) << 16) |
							  ((byteArray[j + 3] & 0xff) << 24));
				j += 4;
			}
			switch (byteArray.Length - j)
			{
				case 3:
					_items[i] = (uint)(byteArray[j + 2] & 0xff) << 16;
					goto case 2;
				case 2:
					_items[i] |= (uint)(byteArray[j + 1] & 0xff) << 8;
					goto case 1;
				case 1:
					_items[i] |= (uint)(byteArray[j] & 0xff);
					break;
			}
		}
		else if (bits is bool[] boolArray)
		{
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(boolArray.Length, BitsPerInt)));
			_size = boolArray.Length;
			for (int i = 0; i < boolArray.Length; i++)
				if (boolArray[i])
					_items[i / BitsPerInt] |= (uint)1 << (i % BitsPerInt);
		}
		else if (bits is IEnumerable<int> ints)
		{
			uint[] uintArray = List<uint>.ToArrayEnumerable(ints, x => (uint)x);
			fixed (uint* ptr = uintArray)
				_items = ptr;
			_size = (_capacity = uintArray.Length) * BitsPerInt;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			uint[] uintArray = List<uint>.ToArrayEnumerable(uints);
			fixed (uint* ptr = uintArray)
				_items = ptr;
			_size = (_capacity = uintArray.Length) * BitsPerInt;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			if (!List<byte>.TryGetCountEasilyEnumerable(bytes, out int count))
				count = bytes.Count();
			if (count > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bits));
			int arrayLength = _capacity = GetArrayLength(count, BytesPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			int i = 0;
			using IEnumerator<byte> en = bytes.GetEnumerator();
			while (en.MoveNext())
			{
				(int index, int remainder) = DivRem(i++, BytesPerInt);
				_items[index] |= (uint)en.Current << remainder * BitsPerByte;
			}
			_size = count * BitsPerByte;
		}
		else if (bits is IEnumerable<bool> bools)
		{
			if (!List<bool>.TryGetCountEasilyEnumerable(bools, out int count))
				count = bools.Count();
			int arrayLength = _capacity = GetArrayLength(count, BitsPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			int i = 0;
			using IEnumerator<bool> en = bools.GetEnumerator();
			while (en.MoveNext())
			{
				(int index, int remainder) = DivRem(i++, BitsPerInt);
				if (en.Current)
					_items[index] |= (uint)1 << remainder;
			}
			_size = count;
		}
		else
			throw new ArgumentException(null, nameof(bits));
	}

	public override int Capacity
	{
		get => _capacity * BitsPerInt;
		set
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value));
			int newints = GetArrayLength(value, BitsPerInt);
			if (newints > _capacity || newints + _shrinkThreshold < _capacity)
			{
				uint* newarray = (uint*)Marshal.AllocHGlobal(sizeof(uint) * newints);
				CopyMemory(_items, newarray, newints > _capacity ? _capacity : newints);
				Marshal.FreeHGlobal((IntPtr)_items);
				_items = newarray;
				_capacity = newints;
			}
			if (value > _size)
			{
				int last = GetArrayLength(_size, BitsPerInt) - 1;
				int bits = _size % BitsPerInt;
				if (bits > 0)
					_items[last] &= ((uint)1 << bits) - 1;
				FillMemory(_items + (last + 1), newints - last - 1, 0);
			}
			ListChanged?.Invoke(this);
		}
	}

	protected override Func<int, BitList> CapacityCreator => CapacityCreatorStatic;

	private static Func<int, BitList> CapacityCreatorStatic => capacity => new(capacity);

	protected override Func<IEnumerable<bool>, BitList> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<bool>, BitList> CollectionCreatorStatic => collection => new(collection);

	protected override int DefaultCapacity => 64;

	public event ListChangedHandler? ListChanged;

	internal override bool GetInternal(int index, bool invoke = true)
	{
		bool item = (_items[index / BitsPerInt] & (1 << (index % BitsPerInt))) != 0;
		if (invoke)
			ListChanged?.Invoke(this);
		return item;
	}

internal override void SetInternal(int index, bool value)
	{
		if (value)
			_items[index / BitsPerInt] |= (uint)1 << (index % BitsPerInt);
		else
			_items[index / BitsPerInt] &= ~((uint)1 << (index % BitsPerInt));
		ListChanged?.Invoke(this);
	}

	public void SetAll(bool value)
	{
		uint fillValue = value ? 0xffffffff : 0;
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] = fillValue;
	}

	public BitList And(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] &= value._items[i];
		return this;
	}

	public BitList Or(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] |= value._items[i];
		return this;
	}

	public BitList Xor(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] ^= value._items[i];
		return this;
	}

	public BitList Not()
	{
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] = ~_items[i];
		return this;
	}

	protected override void ClearInternal(int index, int count)
	{
		(int startIndex, int startRemainder) = DivRem(index, BitsPerInt);
		uint startMask = ((uint)1 << startRemainder) - 1;
		(int endIndex, int endRemainder) = DivRem(index + count, BitsPerInt);
		uint endMask = ~0u << endRemainder;
		if (startIndex == endIndex)
			_items[startIndex] &= startMask | endMask;
		else
		{
			_items[startIndex] &= startMask;
			for (int i = startIndex + 1; i < endIndex; i++)
				_items[i] = 0;
			_items[endIndex] &= endMask;
		}
		ListChanged?.Invoke(this);
	}

	//protected override int CompareToInternal(BitList other)
	//{
	//	int count = Min(_size, other._size), c;
	//	(int intCount, int bitsCount) = DivRem(count, BitsPerInt);
	//	for (int i = 0; i < intCount; i++)
	//		if ((c = _items[i].CompareTo(other._items[i])) != 0)
	//			return c;
	//	if (bitsCount != 0)
	//	{
	//		uint mask = ((uint)1 << bitsCount) - 1;
	//		if ((c = (_items[intCount] & mask).CompareTo(other._items[intCount] & mask)) != 0)
	//			return c;
	//	}
	//	return _size.CompareTo(other._size);
	//}

	public override bool Contains(bool item)
	{
		int emptyValue = item ? 0 : unchecked((int)0xffffffff);
		for (int i = 0; i < _size / BitsPerInt; i++)
			if (_items[i] != emptyValue)
				return true;
		(int last, int remainder) = DivRem(_size, BitsPerInt);
		if (last != 0)
			if ((_items[last] & (1 << remainder)) != (item ? 0 : (1 << remainder) - 1))
				return true;
		return false;
	}

	public static void CopyBits(IList<uint> sourceBits, int sourceIndex, IList<uint> destinationBits, int destinationIndex, int length)
	{
		fixed (uint* sourcePtr = sourceBits.AsSpan(), destinationPtr = destinationBits.AsSpan())
			CopyBits(sourcePtr, sourceBits.Count, sourceIndex, destinationPtr, destinationBits.Count, destinationIndex, length);
	}

	public static void CopyBits(uint* sourceBits, int sourceBound, int sourceIndex, uint* destinationBits, int destinationBound, int destinationIndex, int length)
	{
		CheckParams(sourceBits, sourceBound, sourceIndex, destinationBits, destinationBound, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		(int sourceIntIndex, int sourceBitsIndex) = DivRem(sourceIndex, BitsPerInt);               // Целый индех в исходном массиве.
		(int destinationIntIndex, int destinationBitsIndex) = DivRem(destinationIndex, BitsPerInt);     // Целый индекс в целевом массиве.
		int bitsOffset = destinationBitsIndex - sourceBitsIndex;    // Битовое смещение.
		int intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		uint sourceStartMask = ~0u << sourceBitsIndex; // Маска "головы" источника
		int destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		(int destinationEndIntIndex, int destinationEndBitsIndex) = DivRem(destinationEndIndex, BitsPerInt);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			uint buff = sourceBits[sourceIntIndex] & sourceStartMask;
			uint destinationMask = (~(~0u << length)) << destinationBitsIndex;
			if (bitsOffset >= 0)
				buff <<= bitsOffset;
			else
			{
				buff >>= -bitsOffset;
				if (length + sourceBitsIndex > BitsPerInt)
					buff |= sourceBits[sourceIntIndex + 1] << (BitsPerInt + bitsOffset);
			}
			buff &= destinationMask;
			destinationBits[destinationIntIndex] &= ~destinationMask;
			destinationBits[destinationIntIndex] |= buff;
		}
		else if (sourceIndex >= destinationIndex)
		{
			if (bitsOffset < 0)
			{
				bitsOffset = -bitsOffset;
				ulong buff = destinationBits[destinationIntIndex];
				buff &= ((ulong)1 << destinationBitsIndex) - 1;
				buff |= (ulong)(sourceBits[sourceIntIndex] & sourceStartMask) >> bitsOffset;
				int sourceEndIntIndex = (sourceIndex + length - 1) / BitsPerInt; // Индекс инта "хвоста".
				for (int sourceCurrentIntIndex = sourceIntIndex + 1; sourceCurrentIntIndex <= sourceEndIntIndex; sourceCurrentIntIndex++)
				{
					buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << (BitsPerInt - bitsOffset);
					destinationBits[sourceCurrentIntIndex + intOffset - 1] = (uint)buff;
					buff >>= BitsPerInt;
				}
				if (sourceEndIntIndex + intOffset < destinationBound)
				{
					ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
					buff &= destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] &= (uint)~destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] |= (uint)buff;
				}
			}
			else
			{
				ulong buff = destinationBits[destinationIntIndex];
				buff &= ((ulong)1 << destinationBitsIndex) - 1;
				buff |= ((ulong)(sourceBits[sourceIntIndex] & sourceStartMask)) << bitsOffset;
				int sourceEndIntIndex = (sourceIndex + length - 1) / BitsPerInt; // Индекс инта "хвоста".
				for (int sourceCurrentIntIndex = sourceIntIndex; sourceCurrentIntIndex < sourceEndIntIndex; sourceCurrentIntIndex++)
				{
					destinationBits[sourceCurrentIntIndex + intOffset] = (uint)buff;
					buff >>= BitsPerInt;
					if (sourceCurrentIntIndex + 1 < sourceBound) buff |= ((ulong)sourceBits[sourceCurrentIntIndex + 1]) << bitsOffset;
				}
				if (sourceEndIntIndex + intOffset < destinationBound)
				{
					ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
					buff &= destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] &= (uint)~destinationMask;
					destinationBits[sourceEndIntIndex + intOffset] |= (uint)buff;
				}
			}
		}
		else
		{
			var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
			var sourceEndBitsIndex = sourceEndIndex % BitsPerInt; // Индекс последнего бита в инт.
			var sourceEndIntIndex = sourceEndIndex / BitsPerInt;  // Индекс инта последнего бита.
			uint sourceEndMask = ~0u >> (BitsPerInt - sourceEndBitsIndex - 1); // Маска "хвоста" источника
			if (bitsOffset < 0)
			{
				bitsOffset = -bitsOffset;
				ulong buff = destinationBits[destinationEndIntIndex];
				buff &= ~0ul << (destinationEndBitsIndex + 1);
				buff <<= BitsPerInt;
				buff |= ((ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask)) << (BitsPerInt - bitsOffset);
				for (int sourceCurrentIntIndex = sourceEndIntIndex; sourceCurrentIntIndex > sourceIntIndex; sourceCurrentIntIndex--)
				{
					destinationBits[sourceCurrentIntIndex + intOffset] = (uint)(buff >> BitsPerInt);
					buff <<= BitsPerInt;
					buff |= ((ulong)sourceBits[sourceCurrentIntIndex - 1]) << (BitsPerInt - bitsOffset);
				}
				ulong destinationMask = ~0ul << (BitsPerInt + destinationBitsIndex);
				buff &= destinationMask;
				destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> BitsPerInt);
				destinationBits[destinationIntIndex] |= (uint)(buff >> BitsPerInt);
			}
			else
			{
				ulong buff = destinationBits[destinationEndIntIndex];
				buff &= ~0ul << (destinationEndBitsIndex + 1);
				buff <<= BitsPerInt;
				buff |= (ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask) << (BitsPerInt + bitsOffset);
				for (int sourceCurrentIntIndex = sourceEndIntIndex - 1; sourceCurrentIntIndex >= sourceIntIndex; sourceCurrentIntIndex--)
				{
					buff |= (ulong)sourceBits[sourceCurrentIntIndex] << bitsOffset;
					destinationBits[sourceCurrentIntIndex + intOffset + 1] = (uint)(buff >> BitsPerInt);
					buff <<= BitsPerInt;
				}
				ulong destinationMask = ~0ul << (BitsPerInt + destinationBitsIndex);
				buff &= destinationMask;
				destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> BitsPerInt);
				destinationBits[destinationIntIndex] |= (uint)(buff >> BitsPerInt);
			}
		}
	}

	private static void CheckParams(IList<uint> sourceBits, int sourceIndex, IList<uint> destinationBits, int destinationIndex, int length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Count == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Count == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		int sourceLengthBits = sourceBits.Count * BitsPerInt; // Длина массивов в битах.
		int destinationLengthBits = destinationBits.Count * BitsPerInt; // Длина массивов в битах.
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceLengthBits)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationLengthBits)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	private static void CheckParams(uint* sourceBits, int sourceBound, int sourceIndex, uint* destinationBits, int destinationBound, int destinationIndex, int length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBound == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBound == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		int sourceLengthBits = sourceBound * BitsPerInt; // Длина массивов в битах.
		int destinationLengthBits = destinationBound * BitsPerInt; // Длина массивов в битах.
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceLengthBits)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationLengthBits)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	public object Clone()
	{
		BitList bitList = new(_size, _items);
		return bitList;
	}

	protected override void Copy(ListBase<bool, BitList> source, int sourceIndex, ListBase<bool, BitList> destination, int destinationIndex, int count)
	{
		BitList source2 = source as BitList ?? throw new ArgumentException(null, nameof(source));
		BitList destination2 = destination as BitList ?? throw new ArgumentException(null, nameof(destination));
		CopyBits(source2._items, source2._capacity, sourceIndex, destination2._items, destination2._capacity, destinationIndex, count);
		destination2.ListChanged?.Invoke(this);
	}

	protected override void CopyToInternal(Array array, int index)
	{
		if (array.Rank != 1)
			throw new RankException();
		if (array is int[] intArray)
			fixed (int* ptr = intArray)
				CopyMemory(_items, (uint*)(ptr + index), GetArrayLength(_size, BitsPerInt));
		else if (array is byte[] byteArray)
		{
			int arrayLength = GetArrayLength(_size, BitsPerByte);
			if (array.Length - index < arrayLength)
				throw new ArgumentException(null);
			byte[] b = byteArray;
			int mask = unchecked((1 << BitsPerByte) - 1);
			for (int i = 0; i < arrayLength; i++)
				b[index + i] = (byte)((_items[i / BytesPerInt] >> (i % BytesPerInt * BitsPerByte)) & mask); // Shift to bring the required byte to LSB, then mask
		}
		else if (array is bool[] boolArray)
			CopyToInternal(boolArray, index);
		else
			throw new ArgumentException(null, nameof(array));
	}

	private void CopyToInternal(bool[] array, int index)
	{
		if (array.Length - index < _size)
			throw new ArgumentException(null);
		CopyToInternal(index, array, 0, array.Length);
	}

	protected override void CopyToInternal(int index, bool[] array, int arrayIndex, int count)
	{
		for (int i = 0; i < count; i++)
			array[arrayIndex + i] = ((_items[(index + i) / BitsPerInt] >> ((index + i) % BitsPerInt)) & 0x00000001) != 0;
	}

	public override void Dispose()
	{
		Marshal.FreeHGlobal((IntPtr)_items);
		GC.SuppressFinalize(this);
	}

	//protected override bool EqualsInternal(BitList other)
	//{
	//	int count = Min(_size, other._size);
	//	(int intCount, int bitsCount) = DivRem(count, BitsPerInt);
	//	for (int i = 0; i < intCount; i++)
	//		if (_items[i] != other._items[i])
	//			return false;
	//	if (bitsCount != 0)
	//	{
	//		uint mask = ((uint)1 << bitsCount) - 1;
	//		if ((_items[intCount] & mask) != (other._items[intCount] & mask))
	//			return false;
	//	}
	//	return true;
	//}

	//public override int GetHashCode()
	//{
	//	return _capacity < 3 ? 1234567890 : _items[0].GetHashCode() ^ _items[1].GetHashCode() ^ _items[^1].GetHashCode();
	//}

	public virtual uint GetSmallRange(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		if (count > BitsPerInt)
			throw new ArgumentException(null, nameof(count));
		(int quotient, int remainder) = DivRem(index, BitsPerInt);
		(int quotient2, int remainder2) = DivRem(index + count, BitsPerInt);
		uint result;
		if (quotient == quotient2)
			result = _items[quotient] & (~(~0u << count) << remainder);
		else
		{
			result = _items[quotient] & (~0u << remainder);
			result |= (_items[quotient + 1] & (((uint)1 << remainder2) - 1)) << (BitsPerInt - remainder2);
		}
		return result;
	}

	protected override int IndexOfInternal(bool item, int index, int count)
	{
		int fillValue = item ? 0 : unchecked((int)0xffffffff);
		int startIndex = GetArrayLength(index, BitsPerInt), endIndex = (index + count) / BitsPerInt;
		int startRemainder = index % BitsPerInt;
		int endRemainder = (index + count) % BitsPerInt;
		if (startIndex == endIndex && startRemainder != 0)
			for (int i = startRemainder; i < endRemainder; i++)
				if ((_items[startIndex] & (1 << i)) != 0)
					return startIndex * BitsPerInt + i;
		if (startRemainder != 0)
		{
			int invRemainder = BitsPerInt - startRemainder;
			int mask = (1 << invRemainder) - 1;
			uint first = _items[startIndex - 1] >> startRemainder;
			if (first != (item ? 0 : mask))
				for (int i = 0; i < invRemainder; i++)
					if ((first & (1 << i)) != 0)
						return index + i;
		}
		for (int i = startIndex; i < endIndex; i++)
			if (_items[i] != fillValue)
				for (int j = 0; j < BitsPerInt; j++)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		if (endRemainder != 0)
			for (int i = 0; i < endRemainder; i++)
				if ((_items[endIndex] & (1 << i)) != 0)
					return endIndex * BitsPerInt + i;
		return -1;
	}

	public void InsertRange(int index, IEnumerable collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is BitList bitList)
		{
			int count = bitList._size;
			if (count == 0)
				return;
			EnsureCapacity(_size + count);
			CopyBits(_items, _capacity, index, _items, _capacity, index + count, _size - index);
			CopyBits(bitList._items, bitList._capacity, 0, _items, _capacity, index, count);
			_size += count;
		}
		else if (collection is uint[] uintArray)
		{
			int count = uintArray.Length * BitsPerInt;
			if (count == 0)
				return;
			EnsureCapacity(_size + count);
			CopyBits(_items, _capacity, index, _items, _capacity, index + count, _size - index);
			fixed (uint* uintPtr = uintArray)
				CopyBits(uintPtr, uintArray.Length, 0, _items, _capacity, index, count);
			_size += count;
		}
		else
			InsertRange(index, new BitList(collection));
	}

	protected override int LastIndexOfInternal(bool item, int index, int count)
	{
		int fillValue = item ? 0 : unchecked((int)0xffffffff);
		int startIndex = GetArrayLength(index, BitsPerInt), endIndex = (index + count) / BitsPerInt;
		int startRemainder = index % BitsPerInt;
		int endRemainder = (index + count) % BitsPerInt;
		if (startIndex == endIndex && startRemainder != 0)
			for (int i = endRemainder - 1; i >= startRemainder; i--)
				if ((_items[startIndex] & (1 << i)) != 0)
					return startIndex * BitsPerInt + i;
		if (endRemainder != 0)
			for (int i = endRemainder - 1; i >= 0; i--)
				if ((_items[endIndex] & (1 << i)) != 0)
					return endIndex * BitsPerInt + i;
		for (int i = endIndex - 1; i >= startIndex; i--)
			if (_items[i] != fillValue)
				for (int j = BitsPerInt; j >= 0; j--)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		if (startRemainder != 0)
		{
			int invRemainder = BitsPerInt - startRemainder;
			int mask = (1 << invRemainder) - 1;
			uint first = _items[startIndex - 1] >> startRemainder;
			if (first != (item ? 0 : mask))
				for (int i = invRemainder - 1; i >= 0; i--)
					if ((first & (1 << i)) != 0)
						return index + i;
		}
		return -1;
	}

	protected override BitList ReverseInternal(int index, int count)
	{
		for (int i = 0; i < count / 2; i++)
		{
			(this[index + i], this[index + count - i - 1]) = (this[index + count - i - 1], this[index + i]);
		}
		ListChanged?.Invoke(this);
		return this;
	}

	public void SetRange(int index, IEnumerable collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is BitList bitList)
		{
			int count = bitList._size;
			if (index + count > _size)
				throw new ArgumentException(null);
			if (count > 0)
				CopyBits(bitList._items, bitList._capacity, 0, _items, _capacity, index, count);
		}
		else
			SetRange(index, new BitList(collection));
	}

	public List<uint> ToUIntList()
	{
		int length = GetArrayLength(_size, BitsPerInt);
		List<uint> result = new(length);
		for (int i = 0; i < length; i++)
			result.Add(_items[i]);
		return result;
	}

	public object SyncRoot
	{
		get
		{
			if (_syncRoot == null)
				Interlocked.CompareExchange<object?>(ref _syncRoot, new(), null);
			return _syncRoot;
		}
	}

	public bool IsReadOnly => false;

	public bool IsSynchronized => false;
}

internal class BitList2 : BitList
{
	public BitList2() : base()
	{
	}

	public BitList2(int capacity) : base(capacity)
	{
	}

	public BitList2(int length, bool defaultValue) : base(length, defaultValue)
	{
	}

	public BitList2(IEnumerable bits) : base(bits)
	{
	}
}
#endif

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public class BigBitList : BigListBase<bool, BigBitList, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private const int BitsPerInt = sizeof(int) * BitsPerByte;
	private const int BytesPerInt = sizeof(int);
	private const int BitsPerByte = 8;

	public BigBitList()
	{
		low = new();
		high = null;
		isHigh = false;
	}

	public BigBitList(mpz_t capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity <= CapacityFirstStep)
		{
			low = new((int)capacity);
			high = null;
			indexDeleted = new((int)capacity);
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << ((((capacity - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
			high = new((int)((capacity + (fragment - 1)) / fragment));
			for (mpz_t i = 0; i < capacity / fragment; i++)
				high.Add(new(fragment));
			if (capacity % fragment != 0)
				high.Add(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
		_capacity = capacity;
	}

	public BigBitList(mpz_t length, bool defaultValue)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (length <= CapacityFirstStep)
		{
			low = new((int)length, defaultValue);
			high = null;
			indexDeleted = new((int)length, false);
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << ((((length - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
			high = new((int)((length + (fragment - 1)) / fragment));
			for (mpz_t i = 0; i < length / fragment; i++)
				high.Add(new(fragment, defaultValue));
			if (length % fragment != 0)
				high.Add(new(length % fragment, defaultValue));
			isHigh = true;
		}
		_size = length;
		_capacity = _size;
	}

	public BigBitList(IEnumerable bits)
	{
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BigBitList bigBitList)
		{
			if (!bigBitList.isHigh && bigBitList.low != null)
			{
				low = BitList.RemoveIndexes(bigBitList.low, bigBitList.deletedIndexes);
				high = null;
				_size = bigBitList._size;
				_capacity = bigBitList._capacity;
				fragment = bigBitList.fragment;
				isHigh = false;
			}
			else if (bigBitList.high != null)
			{
				BigBitList list = new(bigBitList.Count);
				for (int i = 0; i < bigBitList.high.Count; i++)
					list.AddRange(bigBitList.high[i]);
				low = list.low;
				high = list.high;
				_size = list._size;
				_capacity = list._capacity;
				fragment = list.fragment;
				isHigh = list.isHigh;
			}
		}
		else if (bits is BitList bitList)
		{
			if (bitList.Count <= CapacityFirstStep)
			{
				low = new(bitList);
				indexDeleted = new(bitList.Count, false);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				int fragment2 = 1 << (((((mpz_t)bitList.Count - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
				fragment = fragment2;
				high = new(GetArrayLength(bitList.Count, fragment2));
				int index = 0;
				for (; index <= bitList.Count - fragment2; index += fragment2)
					high.Add(new(bitList.GetRange(index, fragment2)));
				if (bitList.Count % fragment2 != 0)
					high.Add(new(bitList.GetRange(index, bitList.Count - index)));
				isHigh = true;
			}
			_size = bitList.Count;
			_capacity = _size;
		}
		else if (bits is BigList<uint> bigUIntList)
		{
			mpz_t count = bigUIntList.Count;
			if (count <= CapacityFirstStep / BitsPerInt)
			{
				low = new(bigUIntList);
				indexDeleted = new((int)count * BitsPerInt, false);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				int fragment2 = 1 << ((((count - 1).BitLength + ((mpz_t)BitsPerInt - 1).BitLength + base.CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / base.CapacityStepBitLength - 1) * base.CapacityStepBitLength + CapacityFirstStepBitLength);
				int uintsFragment = fragment2 / BitsPerInt;
				fragment = fragment2;
				high = new((int)((count + uintsFragment - 1) / uintsFragment));
				int index = 0;
				for (; index <= count - uintsFragment; index += uintsFragment)
					high.Add(new(bigUIntList.GetRange(index, uintsFragment)));
				if (index != count)
					high.Add(new(bigUIntList.GetRange(index, count - index)));
				isHigh = true;
			}
			_size = count * BitsPerInt;
			_capacity = _size;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			BigBitList list = new(new BigList<uint>(uints));
			low = list.low;
			high = list.high;
			_size = list._size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else if (bits is IEnumerable<int> ints)
		{
			BigBitList list = new(new BigList<uint>(ints.Select(x => (uint)x)));
			low = list.low;
			high = list.high;
			_size = list._size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			bool b = true;
			IEnumerator<byte> en = bytes.GetEnumerator();
			BigList<uint> values = new();
			while (b)
			{
				int i = 0;
				uint value = 0;
				for (; i < BytesPerInt; i++)
				{
					if (b = en.MoveNext())
						value |= (uint)en.Current << (BitsPerByte * i);
					else
						break;
				}
				if (i != 0)
					values.Add(value);
			}
			BigBitList list = new(values);
			low = list.low;
			high = list.high;
			_size = list._size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else if (bits is BitArray or byte[] or bool[] or IEnumerable<int> or IEnumerable<uint> or IEnumerable<byte> or IEnumerable<bool>)
		{
			BigBitList list = new(new BitList(bits));
			low = list.low;
			high = list.high;
			_size = list._size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else
			throw new ArgumentException(null, nameof(bits));
	}

	protected override Func<mpz_t, BigBitList> CapacityCreator => CapacityCreatorStatic;

	private static Func<mpz_t, BigBitList> CapacityCreatorStatic => capacity => new(capacity);

	protected override int CapacityFirstStepBitLength => 24;

	protected override Func<IEnumerable<bool>, BigBitList> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<bool>, BigBitList> CollectionCreatorStatic => collection => new(collection);

	protected override Func<IEnumerable<bool>, BitList> CollectionLowCreator => CollectionLowCreatorStatic;

	private static Func<IEnumerable<bool>, BitList> CollectionLowCreatorStatic => collection => new(collection);

	protected override int DefaultCapacity => 256;

	public void SetAll(bool value)
	{
		if (!isHigh && low != null)
		{
			low.SetAll(value);
			indexDeleted.SetAll(false);
		}
		else if (high != null)
			high.ForEach(x => x.SetAll(value));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public BigBitList And(BigBitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.And(value.low);
		else if (high != null && value.high != null)
			high = new(high.Zip(value.high, (x, y) => x.And(y)));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public BigBitList Or(BigBitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Or(value.low);
		else if (high != null && value.high != null)
			high = new(high.Zip(value.high, (x, y) => x.Or(y)));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public BigBitList Xor(BigBitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Xor(value.low);
		else if (high != null && value.high != null)
			high = new(high.Zip(value.high, (x, y) => x.Xor(y)));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public BigBitList Not()
	{
		if (!isHigh && low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public void AddRange(IEnumerable bits)
	{
		if (bits is IEnumerable<bool> bools)
			base.AddRange(bools);
		else
			AddRange(new BigBitList(bits));
	}

	protected void AddRangeToEnd(IEnumerable bits)
	{
		if (bits is IEnumerable<bool> bools)
			base.AddRangeToEnd(bools);
		else
			AddRangeToEnd(new BigBitList(bits));
	}

	public BigList<uint> ToUIntBigList()
	{
		if (!isHigh && low != null)
			return new(BitList.RemoveIndexes(low, deletedIndexes).ToUIntList());
		else if (high != null)
			return new(high.SelectMany(x => x.ToUIntBigList()));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
}

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public partial class List<T> : ListBase<T, List<T>>
{
	private T[] _items;

	private static readonly T[] _emptyArray = Array.Empty<T>();

	public List() => _items = _emptyArray;

	public List(int capacity)
	{
		if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity == 0)
			_items = _emptyArray;
		else
			_items = new T[capacity];
	}

	public List(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is ICollection<T> c)
		{
			int count = c.Count;
			if (count == 0)
				_items = _emptyArray;
			else
			{
				_items = new T[count];
				c.CopyTo(_items, 0);
				_size = count;
			}
		}
		else
		{
			_size = 0;
			_items = _emptyArray;
			using IEnumerator<T> en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public List(int capacity, IEnumerable<T> collection) : this(capacity)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is ICollection<T> c)
		{
			int count = c.Count;
			if (count == 0)
				return;
			if (count > capacity)
				_items = new T[count];
			c.CopyTo(_items, 0);
			_size = count;
		}
		else
		{
			using IEnumerator<T> en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public List(T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		_items = array.ToArray();
	}

	public List(int capacity, T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		if (array.Length > capacity)
			_items = array;
		else
		{
			_items = new T[capacity];
			Array.Copy(array, _items, array.Length);
		}
	}

	public List(ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_size = span.Length;
		_items = span.ToArray();
	}

	public List(int capacity, ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_size = span.Length;
		if (span.Length > capacity)
			_items = span.ToArray();
		else
		{
			_items = new T[capacity];
			span.CopyTo(_items);
		}
	}

	public override int Capacity
	{
		get => _items.Length;
		set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value != _items.Length)
			{
				if (value > 0)
				{
					T[] newItems = new T[value];
					if (_size > 0)
						Array.Copy(_items, 0, newItems, 0, _size);
					_items = newItems;
				}
				else
					_items = _emptyArray;
				ListChanged?.Invoke(this);
			}
		}
	}

	protected override Func<int, List<T>> CapacityCreator => CapacityCreatorStatic;

	private static Func<int, List<T>> CapacityCreatorStatic => capacity => new(capacity);

	protected override Func<IEnumerable<T>, List<T>> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<T>, List<T>> CollectionCreatorStatic => collection => new(collection);

	public event ListChangedHandler? ListChanged;

	public List<T> AddRange(ReadOnlySpan<T> span) => InsertRange(_size, span);

	public Span<T> AsSpan() => AsSpan(0, _size);

	public Span<T> AsSpan(int index) => AsSpan(index, _size - index);

	public Span<T> AsSpan(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		return MemoryExtensions.AsSpan(_items, index, count);
	}

	public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		return Array.BinarySearch(_items, index, count, item, comparer);
	}

	public int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	protected override void ClearInternal(int index, int count)
	{
		Array.Clear(_items, index, count);
		ListChanged?.Invoke(this);
	}

	public List<TOutput> Convert<TOutput>(Func<T, TOutput> converter) => base.Convert<TOutput, List<TOutput>>(converter);

	public List<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) => base.Convert<TOutput, List<TOutput>>(converter);

	protected override void Copy(ListBase<T, List<T>> source, int sourceIndex, ListBase<T, List<T>> destination, int destinationIndex, int count)
	{
		Array.Copy((source as List<T> ?? throw new ArgumentException(null, nameof(source)))._items, sourceIndex, (destination as List<T> ?? throw new ArgumentException(null, nameof(destination)))._items, destinationIndex, count);
		ListChanged?.Invoke(this);
	}

	protected override void CopyToInternal(Array array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, _size);

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count) => Array.Copy(_items, index, array, arrayIndex, count);

	public override void Dispose() => GC.SuppressFinalize(this);

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = _items[index];
		if (invoke)
			ListChanged?.Invoke(this);
		return item;
	}

	protected override int IndexOfInternal(T item, int index, int count) => Array.IndexOf(_items, item, index, count);

	public override List<T> Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
		{
			int min = _size + 1;
			int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			T[] newItems = new T[newCapacity];
			if (index > 0)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size)
				Array.Copy(_items, index, newItems, index + 1, _size - index);
			newItems[index] = item;
			_items = newItems;
		}
		else
		{
			if (index < _size)
				Copy(this, index, this, index + 1, _size - index);
			_items[index] = item;
		}
		_size++;
		ListChanged?.Invoke(this);
		return this;
	}

	public List<T> InsertRange(int index, ReadOnlySpan<T> span)
	{
		int count = span.Length;
		if (count == 0)
			return this;
		if (Capacity < _size + count)
		{
			int min = _size + count;
			int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			T[] newItems = new T[newCapacity];
			if (index > 0)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size)
				Array.Copy(_items, index, newItems, index + count, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(newItems, index));
			_items = newItems;
		}
		else
		{
			if (index < _size)
				Array.Copy(_items, index, _items, index + count, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(_items, index));
		}
		_size += count;
		ListChanged?.Invoke(this);
		return this;
	}

	protected override List<T> InsertRangeInternal(int index, IEnumerable<T> collection)
	{
		if (collection is List<T> list)
		{
			int count = list._size;
			if (count == 0)
				return this;
			if (Capacity < _size + count)
			{
				int min = _size + count;
				int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				T[] newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + count, _size - index);
				if (this == list)
				{
					Array.Copy(_items, 0, newItems, index, index);
					Array.Copy(_items, index + count, newItems, index * 2, _size - index);
				}
				else
					Array.Copy(list._items, 0, newItems, index, count);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					Array.Copy(_items, index, _items, index + count, _size - index);
				if (this == list)
				{
					Array.Copy(_items, 0, _items, index, index);
					Array.Copy(_items, index + count, _items, index * 2, _size - index);
				}
				else
					Array.Copy(list._items, 0, _items, index, count);
			}
			_size += count;
			return this;
		}
		else if (collection is T[] array)
		{
			int count = array.Length;
			if (count == 0)
				return this;
			if (Capacity < _size + count)
			{
				int min = _size + count;
				int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				T[] newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + count, _size - index);
				Array.Copy(array, 0, newItems, index, count);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					Array.Copy(_items, index, _items, index + count, _size - index);
				Array.Copy(array, 0, _items, index, count);
			}
			_size += count;
			return this;
		}
		else if (collection is ICollection<T> list2)
		{
			int count = list2.Count;
			if (count == 0)
				return this;
			if (Capacity < _size + count)
			{
				int min = _size + count;
				int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				T[] newItems = new T[newCapacity];
				if (index > 0)
					Array.Copy(_items, 0, newItems, 0, index);
				if (index < _size)
					Array.Copy(_items, index, newItems, index + count, _size - index);
				list2.CopyTo(newItems, index);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					Array.Copy(_items, index, _items, index + count, _size - index);
				list2.CopyTo(_items, index);
			}
			_size += count;
			ListChanged?.Invoke(this);
			return this;
		}
		else
			return InsertRangeInternal(index, new List<T>(collection));
	}

	protected override int LastIndexOfInternal(T item, int index, int count) => Array.LastIndexOf(_items, index, count);
#if !RELEASE

	public List<T> NSort() => NSort(0, _size);

	public List<T> NSort(int index, int count)
	{
		if (this is List<uint> uintList)
		{
			Radix.Sort(uintList._items, index, count);
			return this;
		}
		else
			return Sort(index, count, G.Comparer<T>.Default);
	}

	public List<T> NSort(Func<T, uint> function) => NSort(function, 0, _size);

	public List<T> NSort(Func<T, uint> function, int index, int count)
	{
		Radix.Sort(_items, function, index, count);
		return this;
	}
#endif

	public static List<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) => collection is List<TList> list ? list : new(collection);

	protected override List<T> ReverseInternal(int index, int count)
	{
		Array.Reverse(_items, index, count);
		ListChanged?.Invoke(this);
		return this;
	}

	internal override void SetInternal(int index, T value)
	{
		_items[index] = value;
		ListChanged?.Invoke(this);
	}

	public List<T> Sort() => Sort(0, _size, G.Comparer<T>.Default);

	public List<T> Sort(IComparer<T> comparer) => Sort(0, _size, comparer);

	public List<T> Sort(int index, int count, IComparer<T> comparer)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		Array.Sort(_items, index, count, comparer);
		return this;
	}

	public List<T> Sort<TValue>(List<TValue> values) => Sort(values, 0, _size, G.Comparer<T>.Default);

	public List<T> Sort<TValue>(List<TValue> values, IComparer<T>? comparer) => Sort(values, 0, _size, comparer);

	public List<T> Sort<TValue>(List<TValue> values, int index, int count, IComparer<T>? comparer)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (index + count > values._size)
			throw new ArgumentException(null);
		Array.Sort(_items, values._items, index, count, comparer);
		return this;
	}

	public static implicit operator List<T>(T x) => new List<T>().Add(x);

	public static implicit operator List<T>(T[] x) => new(x);
}

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public class BigList<T> : BigListBase<T, BigList<T>, List<T>>
{
	public BigList()
	{
		low = new();
		high = null;
		fragment = 1;
		isHigh = false;
		_size = 0;
		_capacity = 0;
	}

	public BigList(mpz_t capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity <= CapacityFirstStep)
		{
			low = new((int)capacity);
			high = null;
			fragment = 1;
			indexDeleted = new((int)capacity);
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << ((((capacity - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength);
			high = new((int)((capacity + (fragment - 1)) / fragment));
			for (mpz_t i = 0; i < capacity / fragment; i++)
				high.Add(new(fragment));
			if (capacity % fragment != 0)
				high.Add(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
		_capacity = capacity;
	}

	public BigList(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetCountEasilyEnumerable(col, out int count) ? count : 32)
	{
		IEnumerator<T> en = col.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	protected override Func<mpz_t, BigList<T>> CapacityCreator => CapacityCreatorStatic;

	private static Func<mpz_t, BigList<T>> CapacityCreatorStatic => capacity => new(capacity);

	protected override Func<IEnumerable<T>, BigList<T>> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<T>, BigList<T>> CollectionCreatorStatic => collection => new(collection);

	protected override Func<IEnumerable<T>, List<T>> CollectionLowCreator => CollectionLowCreatorStatic;

	private static Func<IEnumerable<T>, List<T>> CollectionLowCreatorStatic => collection => new(collection);

	public static void CopyBits(BigList<uint> sourceBits, mpz_t sourceIndex, BigList<uint> destinationBits, mpz_t destinationIndex, mpz_t length)
	{
		CheckParams(sourceBits, sourceIndex, destinationBits, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		if (!sourceBits.isHigh && sourceBits.low != null && !destinationBits.isHigh && destinationBits.low != null)
		{
			BitList.CopyBits(sourceBits.low, (int)sourceIndex, destinationBits.low, (int)destinationIndex, (int)length);
			return;
		}
		if (sourceBits.fragment > destinationBits.fragment && sourceBits.isHigh && sourceBits.high != null)
		{
			int index = (int)(sourceIndex / sourceBits.fragment), index2 = (int)((sourceIndex + length) / sourceBits.fragment);
			mpz_t remainder = sourceIndex % sourceBits.fragment;
			if (index == index2)
				CopyBits(sourceBits.high[index], remainder, destinationBits, destinationIndex, length);
			else
			{
				mpz_t firstPart = sourceBits.fragment - remainder;
				CopyBits(sourceBits.high[index], remainder, destinationBits, destinationIndex, firstPart);
				CopyBits(sourceBits.high[index2], 0, destinationBits, destinationIndex + firstPart, length - firstPart);
			}
			return;
		}
		if (destinationBits.fragment > sourceBits.fragment && destinationBits.isHigh && destinationBits.high != null)
		{
			int index = (int)(destinationIndex / destinationBits.fragment), index2 = (int)((destinationIndex + length) / destinationBits.fragment);
			mpz_t remainder = destinationIndex % destinationBits.fragment;
			if (index == index2)
				CopyBits(sourceBits, sourceIndex, destinationBits.high[index], remainder, length);
			else
			{
				mpz_t firstPart = destinationBits.fragment - remainder;
				CopyBits(sourceBits, sourceIndex, destinationBits.high[index], remainder, firstPart);
				CopyBits(sourceBits, sourceIndex + firstPart, destinationBits.high[index2], 0, length - firstPart);
			}
			return;
		}
		if (!(sourceBits.isHigh && sourceBits.high != null && destinationBits.isHigh && destinationBits.high != null && sourceBits.fragment == destinationBits.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		mpz_t fragment = sourceBits.fragment;
		int sourceIntIndex = (int)sourceIndex.Divide(fragment, out mpz_t sourceBitsIndex);               // Целый индех в исходном массиве.
		int destinationIntIndex = (int)destinationIndex.Divide(fragment, out mpz_t destinationBitsIndex);     // Целый индекс в целевом массиве.
		mpz_t bitsOffset = destinationBitsIndex - sourceBitsIndex;    // Битовое смещение.
		int intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		mpz_t destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		int destinationEndIntIndex = (int)destinationEndIndex.Divide(fragment, out mpz_t destinationEndBitsIndex);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			if (bitsOffset >= 0)
				CopyBits(sourceBits.high[sourceIntIndex], sourceBitsIndex, destinationBits.high[destinationIntIndex], destinationBitsIndex, length);
			else
			{
				mpz_t firstPart = fragment - sourceBitsIndex;
				CopyBits(sourceBits.high[sourceIntIndex], sourceBitsIndex, destinationBits.high[destinationIntIndex], destinationBitsIndex, firstPart);
				CopyBits(sourceBits.high[sourceIntIndex + 1], 0, destinationBits.high[destinationIntIndex], destinationBitsIndex + firstPart, length - firstPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
		{
			if (bitsOffset < 0)
			{
				BigList<uint> buff = new(fragment * 2);
				if (!(buff.isHigh && buff.high != null))
					throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
				buff.AddRangeToEnd(destinationBits.high[destinationIntIndex].GetRange(0, destinationBitsIndex));
				int sourceEndIntIndex = (int)((sourceIndex + length - 1) / fragment); // Индекс инта "хвоста".
				for (int sourceCurrentIntIndex = sourceIntIndex + 1; sourceCurrentIntIndex <= sourceEndIntIndex; sourceCurrentIntIndex++)
				{
					buff.AddRangeToEnd(sourceBits.high[sourceCurrentIntIndex]);
					destinationBits.high[sourceCurrentIntIndex + intOffset - 1] = buff.high[0];
					(buff.high[0], buff.high[1]) = (buff.high[1], new(fragment));
					buff._size -= fragment;
				}
				if (sourceEndIntIndex + intOffset < destinationBits.Count && buff.isHigh && buff.high != null)
					destinationBits.high[sourceEndIntIndex + intOffset].SetRange(0, buff.high[0].GetRange(0, destinationEndBitsIndex + 1));
			}
			//else
			//{
			//	ulong buff = destinationBits[destinationIntIndex];
			//	buff &= ((ulong)1 << destinationBitsIndex) - 1;
			//	buff |= ((ulong)(sourceBits[sourceIntIndex] & sourceStartMask)) << bitsOffset;
			//	int sourceEndIntIndex = (sourceIndex + length - 1) / fragment; // Индекс инта "хвоста".
			//	for (int sourceCurrentIntIndex = sourceIntIndex; sourceCurrentIntIndex < sourceEndIntIndex; sourceCurrentIntIndex++)
			//	{
			//		destinationBits[sourceCurrentIntIndex + intOffset] = (uint)buff;
			//		buff >>= fragment;
			//		if (sourceCurrentIntIndex + 1 < sourceBits.Count) buff |= ((ulong)sourceBits[sourceCurrentIntIndex + 1]) << bitsOffset;
			//	}
			//	if (sourceEndIntIndex + intOffset < destinationBits.Count)
			//	{
			//		ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
			//		buff &= destinationMask;
			//		destinationBits[sourceEndIntIndex + intOffset] &= (uint)~destinationMask;
			//		destinationBits[sourceEndIntIndex + intOffset] |= (uint)buff;
			//	}
			//}
		}
		//else
		//{
		//	var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		//	var sourceEndBitsIndex = sourceEndIndex % fragment; // Индекс последнего бита в инт.
		//	var sourceEndIntIndex = sourceEndIndex / fragment;  // Индекс инта последнего бита.
		//	uint sourceEndMask = ~0u >> (fragment - sourceEndBitsIndex - 1); // Маска "хвоста" источника
		//	if (bitsOffset < 0)
		//	{
		//		bitsOffset = -bitsOffset;
		//		ulong buff = destinationBits[destinationEndIntIndex];
		//		buff &= ~0ul << (destinationEndBitsIndex + 1);
		//		buff <<= fragment;
		//		buff |= ((ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask)) << (fragment - bitsOffset);
		//		for (int sourceCurrentIntIndex = sourceEndIntIndex; sourceCurrentIntIndex > sourceIntIndex; sourceCurrentIntIndex--)
		//		{
		//			destinationBits[sourceCurrentIntIndex + intOffset] = (uint)(buff >> fragment);
		//			buff <<= fragment;
		//			buff |= ((ulong)sourceBits[sourceCurrentIntIndex - 1]) << (fragment - bitsOffset);
		//		}
		//		ulong destinationMask = ~0ul << (fragment + destinationBitsIndex);
		//		buff &= destinationMask;
		//		destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> fragment);
		//		destinationBits[destinationIntIndex] |= (uint)(buff >> fragment);
		//	}
		//	else
		//	{
		//		ulong buff = destinationBits[destinationEndIntIndex];
		//		buff &= ~0ul << (destinationEndBitsIndex + 1);
		//		buff <<= fragment;
		//		buff |= (ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask) << (fragment + bitsOffset);
		//		for (int sourceCurrentIntIndex = sourceEndIntIndex - 1; sourceCurrentIntIndex >= sourceIntIndex; sourceCurrentIntIndex--)
		//		{
		//			buff |= (ulong)sourceBits[sourceCurrentIntIndex] << bitsOffset;
		//			destinationBits[sourceCurrentIntIndex + intOffset + 1] = (uint)(buff >> fragment);
		//			buff <<= fragment;
		//		}
		//		ulong destinationMask = ~0ul << (fragment + destinationBitsIndex);
		//		buff &= destinationMask;
		//		destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> fragment);
		//		destinationBits[destinationIntIndex] |= (uint)(buff >> fragment);
		//	}
		//}
	}

	private static void CheckParams(BigList<uint> sourceBits, mpz_t sourceIndex, BigList<uint> destinationBits, mpz_t destinationIndex, mpz_t length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Count == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Count == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceBits.Count)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationBits.Count)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}
}

[DebuggerDisplay("Count = {Count}")]
[ComVisible(true)]
[Serializable]
public unsafe partial class NList<T> : ListBase<T, NList<T>> where T : unmanaged
{
	private T* _items;
	private int _capacity;

	private static readonly T* _emptyArray = null;

	public NList() => _items = _emptyArray;

	public NList(int capacity)
	{
		if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity == 0)
			_items = _emptyArray;
		else
			_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = capacity));
	}

	public NList(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is ICollection<T> c)
		{
			int count = c.Count;
			if (count == 0)
				_items = _emptyArray;
			else
			{
				_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = count));
				fixed (T* ptr = c.AsSpan())
					CopyMemory(ptr, _items, c.Count);
				_size = count;
			}
		}
		else
		{
			_size = 0;
			_items = _emptyArray;
			using IEnumerator<T> en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public NList(int capacity, IEnumerable<T> collection) : this(capacity)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is ICollection<T> c)
		{
			int count = c.Count;
			if (count == 0)
				return;
			if (count > capacity)
				_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = count));
			fixed (T* ptr = c.AsSpan())
				CopyMemory(ptr, _items, c.Count);
			_size = count;
		}
		else
		{
			using IEnumerator<T> en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public NList(T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = _capacity = array.Length;
		fixed (T* ptr = array.ToArray())
			_items = ptr;
	}

	public NList(int capacity, T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		if (array.Length > (_capacity = capacity))
			fixed (T* ptr = array)
				_items = ptr;
		else
		{
			_items = (T*)Marshal.AllocHGlobal(sizeof(T) * (_capacity = capacity));
			fixed (T* ptr = array)
				CopyMemory(ptr, _items, array.Length);
		}
	}

	public NList(ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_size = _capacity = span.Length;
		fixed (T* ptr = span.ToArray())
			_items = ptr;
	}

	public NList(int capacity, ReadOnlySpan<T> span)
	{
		if (span == null)
			throw new ArgumentNullException(nameof(span));
		_size = span.Length;
		if (span.Length > capacity)
			fixed (T* ptr = span.ToArray())
				_items = ptr;
		else
		{
			_items = (T*)Marshal.AllocHGlobal(sizeof(T) * capacity);
			fixed (T* ptr = span)
				CopyMemory(ptr, _items, span.Length);
		}
	}

	public override int Capacity
	{
		get => _capacity;
		set
		{
			if (value < _size)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value != _capacity)
			{
				if (value > 0)
				{
					T* newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * value);
					if (_size > 0)
						CopyMemory(_items, newItems, _size);
					Marshal.FreeHGlobal((IntPtr)_items);
					_items = newItems;
				}
				else
				{
					Marshal.FreeHGlobal((IntPtr)_items);
					_items = _emptyArray;
				}
				_capacity = value;
				ListChanged?.Invoke(this);
			}
		}
	}

	protected override Func<int, NList<T>> CapacityCreator => CapacityCreatorStatic;

	private static Func<int, NList<T>> CapacityCreatorStatic => capacity => new(capacity);

	protected override Func<IEnumerable<T>, NList<T>> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<T>, NList<T>> CollectionCreatorStatic => collection => new(collection);

	public event ListChangedHandler? ListChanged;

	public NList<T> AddRange(ReadOnlySpan<T> span) => InsertRange(_size, span);

	public Span<T> AsSpan() => AsSpan(0, _size);

	public Span<T> AsSpan(int index) => AsSpan(index, _size - index);

	public Span<T> AsSpan(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		return new(_items + index, count);
	}

	protected override void ClearInternal(int index, int count)
	{
		FillMemory(_items + index, count, 0);
		ListChanged?.Invoke(this);
	}

	public NList<TOutput> Convert<TOutput>(Func<T, TOutput> converter) where TOutput : unmanaged => base.Convert<TOutput, NList<TOutput>>(converter);

	public NList<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) where TOutput : unmanaged => base.Convert<TOutput, NList<TOutput>>(converter);

	protected override void Copy(ListBase<T, NList<T>> source, int sourceIndex, ListBase<T, NList<T>> destination, int destinationIndex, int count)
	{
		CopyMemory((source as NList<T> ?? throw new ArgumentException(null, nameof(source)))._items, sourceIndex, (destination as NList<T> ?? throw new ArgumentException(null, nameof(destination)))._items, destinationIndex, count);
		ListChanged?.Invoke(this);
	}

	protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		fixed (T* ptr = array2)
			CopyMemory(_items, 0, ptr, arrayIndex, _size);
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count)
	{
		fixed (T* ptr = array)
			CopyMemory(_items, index, ptr, arrayIndex, count);
	}

	public override void Dispose() => GC.SuppressFinalize(this);

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = _items[index];
		if (invoke)
			ListChanged?.Invoke(this);
		return item;
	}

	protected override int IndexOfInternal(T item, int index, int count)
	{
		T* ptr = _items + index;
		for (int i = 0; i < count; i++)
			if (ptr[i].Equals(item))
				return index + i;
		return -1;
	}

	public override NList<T> Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
		{
			int min = _size + 1;
			int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			T* newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + 1, _size - index);
			newItems[index] = item;
			Marshal.FreeHGlobal((IntPtr)_items);
			_items = newItems;
		}
		else
		{
			if (index < _size)
				Copy(this, index, this, index + 1, _size - index);
			_items[index] = item;
		}
		_size++;
		ListChanged?.Invoke(this);
		return this;
	}

	public NList<T> InsertRange(int index, ReadOnlySpan<T> span)
	{
		int count = span.Length;
		if (count == 0)
			return this;
		if (Capacity < _size + count)
		{
			int min = _size + count;
			int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			T* newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
			if (index > 0)
				CopyMemory(_items, 0, newItems, 0, index);
			if (index < _size)
				CopyMemory(_items, index, newItems, index + count, _size - index);
			span.CopyTo(new(newItems + index, newCapacity - index));
			Marshal.FreeHGlobal((IntPtr)_items);
			_items = newItems;
		}
		else
		{
			if (index < _size)
				CopyMemory(_items, index, _items, index + count, _size - index);
			span.CopyTo(new(_items + index, Capacity - index));
		}
		_size += count;
		ListChanged?.Invoke(this);
		return this;
	}

	protected override NList<T> InsertRangeInternal(int index, IEnumerable<T> collection)
	{
		if (collection is NList<T> list)
		{
			int count = list._size;
			if (count == 0)
				return this;
			if (Capacity < _size + count)
			{
				int min = _size + count;
				int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				T* newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + count, _size - index);
				if (this == list)
				{
					CopyMemory(_items, 0, newItems, index, index);
					CopyMemory(_items, index + count, newItems, index * 2, _size - index);
				}
				else
					CopyMemory(list._items, 0, newItems, index, count);
				Marshal.FreeHGlobal((IntPtr)_items);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + count, _size - index);
				if (this == list)
				{
					CopyMemory(_items, 0, _items, index, index);
					CopyMemory(_items, index + count, _items, index * 2, _size - index);
				}
				else
					CopyMemory(list._items, 0, _items, index, count);
			}
			_size += count;
			return this;
		}
		else if (collection is T[] array)
		{
			int count = array.Length;
			if (count == 0)
				return this;
			if (Capacity < _size + count)
			{
				int min = _size + count;
				int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				T* newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + count, _size - index);
				fixed (T* ptr = array)
					CopyMemory(ptr, 0, newItems, index, count);
				Marshal.FreeHGlobal((IntPtr)_items);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + count, _size - index);
				fixed (T* ptr = array)
					CopyMemory(ptr, 0, _items, index, count);
			}
			_size += count;
			return this;
		}
		else if (collection is ICollection<T> list2)
		{
			int count = list2.Count;
			if (count == 0)
				return this;
			if (Capacity < _size + count)
			{
				int min = _size + count;
				int newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				if (newCapacity < min) newCapacity = min;
				T* newItems = (T*)Marshal.AllocHGlobal(sizeof(T) * newCapacity);
				if (index > 0)
					CopyMemory(_items, 0, newItems, 0, index);
				if (index < _size)
					CopyMemory(_items, index, newItems, index + count, _size - index);
				fixed (T* ptr = list2.AsSpan())
					CopyMemory(ptr, 0, newItems, index, list2.Count);
				Marshal.FreeHGlobal((IntPtr)_items);
				_items = newItems;
			}
			else
			{
				if (index < _size)
					CopyMemory(_items, index, _items, index + count, _size - index);
				fixed (T* ptr = list2.AsSpan())
					CopyMemory(ptr, 0, _items, index, list2.Count);
			}
			_size += count;
			ListChanged?.Invoke(this);
			return this;
		}
		else
			return InsertRangeInternal(index, new NList<T>(collection));
	}

	protected override int LastIndexOfInternal(T item, int index, int count)
	{
		int endIndex = index - count + 1;
		for (int i = index; i >= endIndex; i--)
			if (_items[i].Equals(item))
				return i;
		return -1;
	}

	public static NList<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) where TList : unmanaged => collection is NList<TList> list ? list : new(collection);

	protected override NList<T> ReverseInternal(int index, int count)
	{
		for (int i = 0; i < _size / 2; i++)
			(_items[i], _items[_size - 1 - i]) = (_items[_size - 1 - i], _items[i]);
		ListChanged?.Invoke(this);
		return this;
	}

	internal override void SetInternal(int index, T value)
	{
		_items[index] = value;
		ListChanged?.Invoke(this);
	}

	public NList<T> Sort() => Sort(0, _size);

	public NList<T> Sort(int index, int count)
	{
		if (this is NList<uint> uintList)
		{
#if !RELEASE
			Radix.Sort(uintList._items + index, count);
#endif
			return this;
		}
		else
			throw new NotSupportedException();
	}

	public NList<T> Sort(Func<T, uint> function) => Sort(function, 0, _size);

	public NList<T> Sort(Func<T, uint> function, int index, int count) =>
#if !RELEASE
		//Radix.Sort(_items + index, function, count);
#endif
		this;

	public static implicit operator NList<T>(T x) => new NList<T>().Add(x);
}

[DebuggerDisplay("Count = {Count}")]
public class Compact2dList<T> : IList<List<T>>
{
	private readonly List<T> main;
	private readonly List<int> starts, lengths;

	public Compact2dList()
	{
		main = new(0);
		starts = new();
		lengths = new();
	}

	public Compact2dList(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		main = new(capacity);
		starts = new();
		lengths = new();
	}

	public Compact2dList(int capacity, int lengthsCapacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		main = new(capacity);
		starts = new(lengthsCapacity);
		lengths = new(lengthsCapacity);
	}

	public Compact2dList(IEnumerable<List<T>> collection)
	{
		main = new(0);
		starts = new();
		lengths = new();
		using IEnumerator<List<T>> en = collection.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	public virtual int Capacity
	{
		get => main.Capacity;
		set
		{
			if (value < main.Count)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value != main.Capacity)
				main.Capacity = value;
			ListChanged?.Invoke(this);
		}
	}

	public virtual int Count => lengths.Count;
	public bool IsReadOnly => false;

	public virtual List<T> this[int index, bool invoke = true]
	{
		get
		{
			List<T> list = new((index == starts.Count - 1 ? main.Count : starts[index + 1]) - starts[index], main.GetRange(starts[index], lengths[index]));
			if (invoke)
				ListChanged?.Invoke(this);
			list.ListChanged += l => this[index] = l;
			return list;
		}
		set
		{
			ShiftCapacities(index, value.Capacity);
			main.ReplaceRange(starts[index], Min(value.Count, (index == starts.Count - 1 ? main.Count : starts[index + 1]) - starts[index]), value);
			lengths[index] = value.Count;
			ListChanged?.Invoke(this);
		}
	}

	List<T> IList<List<T>>.this[int index]
	{
		get => this[index];
		set => this[index] = value;
	}

	public virtual T this[int index, int innerIndex]
	{
		get => innerIndex < 0 || innerIndex >= (index == starts.Count - 1 ? main.Count : starts[index + 1]) - starts[index] ? throw new ArgumentOutOfRangeException(nameof(innerIndex)) : main[starts[index] + innerIndex];
		set
		{
			if (innerIndex < 0 || innerIndex >= (index == starts.Count - 1 ? main.Count : starts[index + 1]) - starts[index])
				throw new ArgumentOutOfRangeException(nameof(innerIndex));
			main[starts[index] + innerIndex] = value;
		}
	}

	public virtual T this[Index index, Index innerIndex]
	{
		get
		{
			int index2 = index.IsFromEnd ? lengths.Count - index.Value : index.Value;
			int capacity = (index2 == starts.Count - 1 ? main.Count : starts[index2 + 1]) - starts[index2];
			int innerIndex2 = innerIndex.IsFromEnd ? capacity - innerIndex.Value : innerIndex.Value;
			if (innerIndex2 < 0 || innerIndex2 >= capacity)
				throw new ArgumentOutOfRangeException(nameof(innerIndex));
			return main[starts[index2] + innerIndex2];
		}
		set
		{
			int index2 = index.IsFromEnd ? lengths.Count - index.Value : index.Value;
			int capacity = (index2 == starts.Count - 1 ? main.Count : starts[index2 + 1]) - starts[index2];
			int innerIndex2 = innerIndex.IsFromEnd ? capacity - innerIndex.Value : innerIndex.Value;
			if (innerIndex2 < 0 || innerIndex2 >= capacity)
				throw new ArgumentOutOfRangeException(nameof(innerIndex));
			main[starts[index2] + innerIndex2] = value;
		}
	}

	public delegate void ListChangedHandler(Compact2dList<T> newList);
	public event ListChangedHandler? ListChanged;

	public virtual void Add(List<T> item)
	{
		starts.Add(main.Count);
		lengths.Add(item.Count);
		T[] toAdd = new T[item.Capacity];
		item.CopyTo(toAdd);
		main.AddRange(toAdd);
		ListChanged?.Invoke(this);
	}

	public virtual void Add(int index, T item)
	{
		if ((index == starts.Count - 1 ? main.Count : starts[index + 1]) - starts[index] == lengths[index])
			ShiftCapacities(index, Max(4, lengths[index] * 2));
		main[starts[index] + lengths[index]++] = item;
	}

	public virtual void AddRange(IEnumerable<List<T>> collection) => InsertRange(lengths.Count, collection);

	public virtual void Clear()
	{
		main.Clear();
		starts.Clear();
		lengths.Clear();
		ListChanged?.Invoke(this);
	}

	public virtual bool Contains(List<T> item)
	{
		using (IEnumerator<List<T>> en = GetEnumerator())
			while (en.MoveNext())
				if (OptimizedLinq.Equals(en.Current, item))
					return true;
		return false;
	}

	public virtual void CopyTo(List<T>[] array) => CopyTo(array, 0);

	public virtual void CopyTo(int index, List<T>[] array, int arrayIndex, int count)
	{
		for (int i = 0; i < count; i++)
			array[arrayIndex + i] = this[index + i, false];
	}

	public virtual void CopyTo(List<T>[] array, int arrayIndex) => CopyTo(0, array, arrayIndex, lengths.Count);

	public virtual Compact2dList<T> Filter(Func<List<T>, bool> match)
	{
		Compact2dList<T> result = new();
		for (int i = 0; i < lengths.Count; i++)
		{
			List<T> item = this[i];
			if (match(item))
				result.Add(item);
		}
		if (result.lengths.Count < lengths.Count * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual Compact2dList<T> Filter(Func<List<T>, int, bool> match)
	{
		Compact2dList<T> result = new();
		for (int i = 0; i < lengths.Count; i++)
		{
			List<T> item = this[i];
			if (match(item, i))
				result.Add(item);
		}
		if (result.lengths.Count < lengths.Count * 0.8)
			result.TrimExcess();
		return result;
	}

	public virtual Compact2dList<T> FilterInPlace(Func<List<T>, bool> match)
	{
		int targetIndex = 0;
		for (int i = 0; i < lengths.Count; i++)
		{
			List<T> item = this[i];
			if (match(item))
				this[targetIndex++] = item;
		}
		starts.RemoveRange(targetIndex, starts.Count - targetIndex);
		lengths.RemoveRange(targetIndex, lengths.Count - targetIndex);
		return this;
	}

	public virtual Compact2dList<T> FilterInPlace(Func<List<T>, int, bool> match)
	{
		int targetIndex = 0;
		for (int i = 0; i < lengths.Count; i++)
		{
			List<T> item = this[i];
			if (match(item, i))
				this[targetIndex++] = item;
		}
		starts.RemoveRange(targetIndex, starts.Count - targetIndex);
		lengths.RemoveRange(targetIndex, lengths.Count - targetIndex);
		return this;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<List<T>> IEnumerable<List<T>>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual Compact2dList<T> GetRange(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > lengths.Count)
			throw new ArgumentException(null);
		Compact2dList<T> list = new();
		for (int i = index; i < index + count; i++)
			list.Add(this[i]);
		return list;
	}

	public virtual Compact2dList<T> GetRange(Range range)
	{
		Index start = range.Start, end = range.End;
		if (start.IsFromEnd)
		{
			if (end.IsFromEnd)
				return GetRange(lengths.Count - start.Value, start.Value - end.Value);
			else
				return GetRange(lengths.Count - start.Value, end.Value - lengths.Count + start.Value);
		}
		else
		{
			if (end.IsFromEnd)
				return GetRange(start.Value, lengths.Count - end.Value - start.Value);
			else
				return GetRange(start.Value, end.Value - start.Value);
		}
	}

	public virtual int IndexOf(List<T> item)
	{
		using (IEnumerator<List<T>> en = GetEnumerator())
		{
			int i = 0;
			while (en.MoveNext())
			{
				if (OptimizedLinq.Equals(en.Current, item))
					return i;
				i++;
			}
		}
		return -1;
	}

	public virtual int IndexOf(List<T> item, int index)
	{
		if (index > lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		for (int i = Max(index, 0); i < lengths.Count; i++)
			if (OptimizedLinq.Equals(this[i], item))
				return i;
		return -1;
	}

	public virtual int IndexOf(List<T> item, int index, int count)
	{
		if (index > lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0 || index > lengths.Count - count)
			throw new ArgumentOutOfRangeException(nameof(count));
		for (int i = index; i < index + count; i++)
			if (OptimizedLinq.Equals(this[i], item))
				return i;
		return -1;
	}

	public virtual void Insert(int index, List<T> item)
	{
		if ((uint)index > (uint)lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		starts.Add((starts.Count == 0 ? 0 : starts[^1]) + item.Capacity);
		for (int i = starts.Count - 2; i >= index + 1; i--)
			starts[i] = starts[i - 1] + item.Capacity;
		lengths.Insert(index, item.Count);
		T[] toAdd = new T[item.Capacity];
		item.CopyTo(toAdd);
		main.InsertRange(starts[index], toAdd);
		ListChanged?.Invoke(this);
	}

	public virtual void InsertRange(int index, IEnumerable<List<T>> collection)
	{
		if (collection is null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is Compact2dList<T> dfl)
		{
			int indexStart = index == starts.Count ? main.Count : starts[index];
			main.InsertRange(indexStart, dfl.main);
			int sum = dfl.starts.Sum();
			for (int i = index; i < starts.Count; i++)
				starts[i] += sum;
			starts.InsertRange(index, dfl.starts.Convert(x => x + indexStart));
			lengths.InsertRange(index, dfl.lengths);
			ListChanged?.Invoke(this);
		}
		else
			InsertRange(index, new Compact2dList<T>(collection));
	}

	public virtual int LastIndexOf(List<T> item)
	{
		if (lengths.Count == 0)
			return -1;
		else
			return LastIndexOf(item, lengths.Count - 1, lengths.Count);
	}

	public virtual int LastIndexOf(List<T> item, int index) => LastIndexOf(item, index, index + 1);

	public virtual int LastIndexOf(List<T> item, int index, int count) => throw new NotImplementedException();

	public virtual bool Remove(List<T> item)
	{
		int index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual void RemoveAt(int index)
	{
		if ((uint)index > (uint)lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		int capacity = this[index].Capacity;
		for (int i = index; i < starts.Count; i++)
			starts[i] -= capacity;
		starts.RemoveAt(starts.Count - 1);
		lengths.RemoveAt(index);
		Shift(starts[index] + capacity, -capacity);
		ListChanged?.Invoke(this);
	}

	public virtual void RemoveRange(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > lengths.Count)
			throw new ArgumentException(null);
		if (index + count < starts.Count)
		{
			int diff = starts[index + count] - starts[index];
			for (int i = index; i < starts.Count - count; i++)
				starts[i] -= diff;
		}
		starts.RemoveRange(starts.Count - count, count);
		lengths.RemoveRange(index, count);
		int newPos = starts[index], oldPos = starts[index + count];
		Shift(newPos, newPos - oldPos);
		ListChanged?.Invoke(this);
	}

	public virtual Compact2dList<T> Replace(IEnumerable<List<T>> collection) => ReplaceRangeInternal(0, lengths.Count, collection);

	public virtual Compact2dList<T> ReplaceRange(int index, int count, IEnumerable<List<T>> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > lengths.Count)
			throw new ArgumentException(null);
		return ReplaceRangeInternal(index, count, collection);
	}

	internal virtual Compact2dList<T> ReplaceRangeInternal(int index, int count, IEnumerable<List<T>> collection)
	{
		if (collection is Compact2dList<T> list)
		{
			if (list.lengths.Count > 0)
			{
				int leftMainIndex = index >= starts.Count ? main.Count : starts[index];
				main.ReplaceRangeInternal(leftMainIndex, (index + count >= starts.Count ? main.Count : starts[index + count]) - leftMainIndex, list.main);
				starts.ReplaceRangeInternal(index, count, list.starts);
				lengths.ReplaceRangeInternal(index, count, list.lengths);
			}
			return this;
		}
		else
			return ReplaceRange(index, count, new Compact2dList<T>(collection));
	}

	public static Compact2dList<T> ReturnOrConstruct(IEnumerable<List<T>> collection) => collection is Compact2dList<T> list ? list : new(collection);

	public virtual void Reverse() => Reverse(0, lengths.Count);

	public virtual void Reverse(int index, int count) => throw new NotImplementedException();

	public virtual Compact2dList<T> SetRange(int index, IEnumerable<List<T>> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)lengths.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is Compact2dList<T> list)
		{
			if (index + list.lengths.Count > lengths.Count)
				throw new ArgumentException(null);
			return SetRangeInternal(index, list);
		}
		else
			return SetRange(index, new Compact2dList<T>(collection));
	}

	internal virtual Compact2dList<T> SetRangeAndSizeInternal(int index, Compact2dList<T> list)
	{
		SetRangeInternal(index, list);
		return this;
	}

	internal virtual Compact2dList<T> SetRangeInternal(int index, Compact2dList<T> list)
	{
		int count = list.lengths.Count;
		int leftPos = starts[index];
		if (index + count < lengths.Count)
		{
			int rightPos = index + count == starts.Count ? main.Count : starts[index + count];
			Shift(rightPos, leftPos + list.main.Count - rightPos);
		}
		if (count > 0)
		{
			starts.SetRange(index, list.starts);
			lengths.SetRange(index, list.lengths);
			main.SetRange(leftPos, list.main);
		}
		return this;
	}

	private void Shift(int startPos, int amount)
	{
		if (startPos < 0 || startPos > main.Count)
			throw new ArgumentOutOfRangeException(nameof(startPos));
		if ((uint)(main.Count + amount) > int.MaxValue)
			throw new ArgumentException(null);
		if (startPos + amount < 0)
			throw new ArgumentException(null);
		if (startPos == main.Count)
			return;
		main.SetRange(startPos + amount, main.GetRange(startPos, main.Count - startPos));
	}

	private void ShiftCapacities(int index, int targetCapacity)
	{
		if (index >= starts.Count - 1)
			return;
		int count = 0, count2 = 0, targetCount = targetCapacity - (starts[index + 1] - starts[index]);
		if (targetCount <= 0)
			return;
		int i;
		for (i = index + 1; i < starts.Count && count < targetCount; i++)
		{
			count2 = count;
			count += (i == starts.Count - 1 ? main.Count : starts[i + 1]) - starts[i] - lengths[i];
		}
		if (count < targetCount)
		{
			SetRange(index, GetRange(index..).Convert((x, index2) => new List<T>(Max(x.Capacity, index2 == 0 ? targetCapacity : Max(4, x.Count * 2)), x)));
			return;
		}
		i--;
		main.SetRange(starts[i] + targetCount - count2, main.GetRange(starts[i], lengths[i]));
		starts[i] += targetCount - count2;
		for (i--; i >= index + 1; i--)
		{
			main.SetRange(starts[i + 1] - lengths[i], main.GetRange(starts[i], lengths[i]));
			starts[i] = starts[i + 1] - lengths[i];
		}
	}

	public virtual Compact2dList<T> Shuffle()
	{
		Random random = new();
		for (int i = lengths.Count; i > 0; i--)
		{
			int swapIndex = random.Next(i);
			(this[swapIndex], this[i - 1]) = (this[i - 1], this[swapIndex]);
		}
		return this as Compact2dList<T> ?? throw new InvalidOperationException();
	}

	public virtual Compact2dList<T> Skip(int count) => GetRange(Min(count, lengths.Count), Max(0, lengths.Count - count));

	public virtual Compact2dList<T> SkipLast(int count) => GetRange(0, Max(0, lengths.Count - count));

	public virtual Compact2dList<T> SkipWhile(Func<List<T>, bool> function)
	{
		int i = 0;
		for (; i < lengths.Count && function(this[i]); i++) ;
		return GetRange(i, lengths.Count - i);
	}

	public virtual Compact2dList<T> SkipWhile(Func<List<T>, int, bool> function)
	{
		int i = 0;
		for (; i < lengths.Count && function(this[i], i); i++) ;
		return GetRange(i, lengths.Count - i);
	}

	public virtual Compact2dList<T> Take(int count) => GetRange(0, Min(count, lengths.Count));

	public virtual Compact2dList<T> TakeLast(int count) => GetRange(Max(0, lengths.Count - count), Min(count, lengths.Count));

	public virtual Compact2dList<T> TakeWhile(Func<List<T>, bool> function)
	{
		int i = 0;
		for (; i < lengths.Count && function(this[i]); i++) ;
		return GetRange(0, i);
	}

	public virtual Compact2dList<T> TakeWhile(Func<List<T>, int, bool> function)
	{
		int i = 0;
		for (; i < lengths.Count && function(this[i], i); i++) ;
		return GetRange(0, i);
	}

	public virtual List<T>[] ToArray()
	{
		List<T>[] array = new List<T>[lengths.Count];
		for (int i = 0; i < lengths.Count; i++)
			array[i] = this[i];
		return array;
	}

	public virtual void TrimExcess()
	{
		int threshold = (int)(main.Capacity * 0.9);
		if (main.Count < threshold)
			Capacity = main.Count;
		starts.TrimExcess();
		lengths.TrimExcess();
	}

	public struct Enumerator : IEnumerator<List<T>>
	{
		private readonly Compact2dList<T> list;
		private int index, start, length;
		private readonly IEnumerator<int> startsEnumerator, lengthsEnumerator;

		public List<T> Current { get; private set; }
		object IEnumerator.Current => Current;

		internal Enumerator(Compact2dList<T> list)
		{
			this.list = list;
			index = 0;
			start = 0;
			length = 0;
			startsEnumerator = list.starts.GetEnumerator();
			lengthsEnumerator = list.lengths.GetEnumerator();
			Current = default!;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (!startsEnumerator.MoveNext() || !lengthsEnumerator.MoveNext())
				return MoveNextRare();
			if ((uint)index < (uint)list.lengths.Count)
			{
				start = startsEnumerator.Current;
				length = lengthsEnumerator.Current;
				Current = new((index == list.starts.Count - 1 ? list.main.Count : list.starts[index + 1]) - startsEnumerator.Current, list.main.GetRange(start, length));
				var list_ = list;
				var index_ = index;
				Current.ListChanged += l => list_[index_] = l;
				index++;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = list.lengths.Count + 1;
			length = 0;
			Current = default!;
			return false;
		}

		void IEnumerator.Reset()
		{
			index = 0;
			start = 0;
			length = 0;
			startsEnumerator.Reset();
			lengthsEnumerator.Reset();
			Current = default!;
		}
	}
}
