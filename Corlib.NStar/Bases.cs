using Corlib.NStar;
using System.Diagnostics;

namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseIndexable<T> : IReadOnlyList<T>, IDisposable
{
	private protected int _size;
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
		set => throw new NotSupportedException();
	}

	T G.IReadOnlyList<T>.this[int index] => this[index];

	public virtual int Length => _size;

	protected static bool IsCompatibleObject(object? value) => value is T || value == null && default(T) == null;

	public virtual Span<T> AsSpan() => AsSpan(0, _size);

	public virtual Span<T> AsSpan(Index index) => AsSpan(index.GetOffset(_size));

	public virtual Span<T> AsSpan(int index) => AsSpan(index, _size - index);

	public abstract Span<T> AsSpan(int index, int length);

	public virtual Span<T> AsSpan(Range range) => AsSpan()[range];

	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, _size);

	public virtual bool Contains(IEnumerable<T> collection, int index) => Contains(collection, index, _size - index);

	public virtual bool Contains(IEnumerable<T> collection, int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (length == 0 || !collection.Any())
			return false;
		if (collection is not G.IList<T> list)
			list = collection.ToList();
		var j = 0;
		for (var i = 0; i - j <= length - list.Count; i++)
		{
			if (this[index + i]?.Equals(list[j]) ?? list[j] == null)
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

	public virtual bool Contains(T? item) => Contains(item, 0, _size);

	public virtual bool Contains(T? item, int index) => Contains(item, index, _size - index);

	public virtual bool Contains(T? item, int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		var hs = collection.ToHashSet();
		for (var i = index; i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		if (array != null && array.Rank != 1)
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

	public virtual void CopyTo(int index, T[] array, int arrayIndex, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		if (arrayIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (arrayIndex + length > array.Length)
			throw new ArgumentException(null);
		CopyToInternal(index, array, arrayIndex, length);
	}

	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	public virtual void CopyTo(T[] array, int arrayIndex) => CopyTo(0, array, arrayIndex, Length);

	private protected virtual void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, Length);
	}

	private protected abstract void CopyToInternal(int index, T[] array, int arrayIndex, int length);

	public abstract void Dispose();

	public virtual bool EndsWith(T? item) => _size > 0 && (GetInternal(_size - 1)?.Equals(item) ?? item == null);

	public virtual bool EndsWith(IEnumerable<T> collection) => _size >= CreateVar(collection.Length(), out var length) && EqualsInternal(collection, _size - length);

	public virtual bool Equals(IEnumerable<T>? collection) => EqualsInternal(collection, 0, true);

	public virtual bool Equals(IEnumerable<T>? collection, int index, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	public override bool Equals(object? obj)
	{
		if (obj == null || obj is not G.ICollection<T> m)
			return false;
		else if (_size != m.Count)
			return false;
		else
			return Equals(m);
	}

	private protected virtual bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is G.IList<T> list)
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
		else
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
	}

	public virtual bool Exists(Predicate<T> match) => FindIndex(match) != -1;

	public virtual T? Find(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		for (var i = 0; i < _size; i++)
			if (match(this[i]))
				return this[i];
		return default;
	}

	public virtual List<T> FindAll(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		List<T> list = new();
		for (var i = 0; i < _size; i++)
			if (match(this[i]))
				list.Add(this[i]);
		return list;
	}

	public virtual int FindIndex(int startIndex, int length, Predicate<T> match)
	{
		if ((uint)startIndex > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(startIndex));
		if (length < 0 || startIndex > _size - length)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		var endIndex = startIndex + length;
		for (var i = startIndex; i < endIndex; i++)
			if (match(this[i]))
				return i;
		return -1;
	}

	public virtual int FindIndex(int startIndex, Predicate<T> match) => FindIndex(startIndex, _size - startIndex, match);

	public virtual int FindIndex(Predicate<T> match) => FindIndex(0, _size, match);

	public virtual T? FindLast(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		for (var i = _size - 1; i >= 0; i--)
			if (match(this[i]))
				return this[i];
		return default;
	}

	public virtual int FindLastIndex(int startIndex, int length, Predicate<T> match)
	{
		if ((uint)startIndex >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(startIndex));
		if (length < 0 || startIndex - length + 1 < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		if (_size == 0)
			if (startIndex != -1)
				throw new ArgumentOutOfRangeException(nameof(startIndex));
		var endIndex = startIndex - length;
		for (var i = startIndex; i > endIndex; i--)
			if (match(this[i]))
				return i;
		return -1;
	}

	public virtual int FindLastIndex(int startIndex, Predicate<T> match) => FindLastIndex(startIndex, startIndex + 1, match);

	public virtual int FindLastIndex(Predicate<T> match) => FindLastIndex(_size - 1, _size, match);

	public virtual void ForEach(Action<T> action) => ForEach(action, 0, _size);

	public virtual void ForEach(Action<T> action, int index) => ForEach(action, index, _size - index);

	public virtual void ForEach(Action<T> action, int index, int count)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));
		for (var i = index; i < index + count; i++)
			action(this[i]);
	}

	public virtual void ForEach(Action<T, int> action) => ForEach(action, 0, _size);

	public virtual void ForEach(Action<T, int> action, int index) => ForEach(action, index, _size - index);

	public virtual void ForEach(Action<T, int> action, int index, int count)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));
		for (var i = index; i < index + count; i++)
			action(this[i], i);
	}

	public virtual IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

	private Enumerator GetEnumeratorInternal() => new(this);

	public override int GetHashCode() => _size < 3 ? 1234567890 : ((GetInternal(0)?.GetHashCode() ?? 0) << 9 ^ (GetInternal(1)?.GetHashCode() ?? 0)) << 9 ^ (GetInternal(_size - 1)?.GetHashCode() ?? 0);

	internal abstract T GetInternal(int index, bool invoke = true);

	public virtual Slice<T> GetSlice() => GetSlice(0, _size);

	public virtual Slice<T> GetSlice(Index index) => GetSlice(index.GetOffset(_size));

	public virtual Slice<T> GetSlice(int index) => GetSlice(index, _size - index);

	public virtual Slice<T> GetSlice(int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return new();
		return GetSliceInternal(index, length);
	}

	public virtual Slice<T> GetSlice(Range range)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException(null);
		var (start, length) = range.GetOffsetAndLength(_size);
		return GetSlice(start, length);
	}

	private protected abstract Slice<T> GetSliceInternal(int index, int length);

	public virtual int IndexOf(IEnumerable<T> collection) => IndexOf(collection, 0, _size);

	public virtual int IndexOf(IEnumerable<T> collection, int index) => IndexOf(collection, index, _size - index);

	public virtual int IndexOf(IEnumerable<T> collection, int index, int length) => IndexOf(collection, index, length, out _);

	public virtual int IndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (_size == 0 || length == 0 || !collection.Any())
		{
			collectionLength = 0;
			return -1;
		}
		if (collection is not G.ICollection<T> c)
			c = collection.ToList();
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		return IndexOfInternal(item, index, length);
	}

	public virtual int IndexOfAny(IEnumerable<T> collection) => IndexOfAny(collection, 0, _size);

	public virtual int IndexOfAny(IEnumerable<T> collection, int index) => IndexOfAny(collection, index, _size - index);

	public virtual int IndexOfAny(IEnumerable<T> collection, int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		var hs = collection.ToHashSet();
		for (var i = index; i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return i;
		return -1;
	}

	private protected abstract int IndexOfInternal(T item, int index, int length);

	public virtual int LastIndexOf(IEnumerable<T> collection) => LastIndexOf(collection, _size - 1, _size);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index) => LastIndexOf(collection, index, index + 1);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int length) => LastIndexOf(collection, index, length, out _);

	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		if (_size != 0 && index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size != 0 && length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length > index + 1)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (_size == 0 || length == 0 || !collection.Any())
		{
			collectionLength = 0;
			return -1;
		}
		if (collection is not G.ICollection<T> c)
			c = collection.ToList();
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
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length > index + 1)
			throw new ArgumentOutOfRangeException(nameof(length));
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
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length > index + 1)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		if (index >= _size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length > index + 1)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		var hs = collection.ToHashSet();
		var startIndex = index + 1 - length;
		for (var i = index; i >= startIndex; i--)
			if (!hs.Contains(GetInternal(i)))
				return i;
		return -1;
	}

	private protected abstract int LastIndexOfInternal(T item, int index, int length);

	public virtual T Random() => this[random.Next(_size)];

	public virtual T Random(Random randomObj) => this[randomObj.Next(_size)];

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

	public virtual bool StartsWith(IEnumerable<T> collection) => EqualsInternal(collection, 0);

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
		var array = new T[Length];
		CopyToInternal(0, array, 0, Length);
		return array;
	}

	public virtual bool TrueForAll(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException(nameof(match));
		for (var i = 0; i < _size; i++)
			if (!match(this[i]))
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
				current = localCollection[index++];
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

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseIndexable<T, TCertain> : BaseIndexable<T>, IEquatable<TCertain> where TCertain : BaseIndexable<T, TCertain>, new()
{
	public virtual TCertain this[Range range] => GetRange(range);

	public virtual int Compare(int index, TCertain other, int otherIndex) => Compare(index, other, otherIndex, Min(_size - index, other._size - otherIndex));

	public virtual int Compare(int index, TCertain other, int otherIndex, int length)
	{
		if (other == null)
			throw new ArgumentNullException(nameof(other));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (otherIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(otherIndex));
		if (otherIndex + length > other._size)
			throw new ArgumentException(null);
		return CompareInternal(index, other, otherIndex, length);
	}

	public virtual int Compare(TCertain other) => Compare(0, other, 0, Min(_size, other._size));

	public virtual int Compare(TCertain other, int length) => Compare(0, other, 0, length);

	private protected virtual int CompareInternal(int index, TCertain other, int otherIndex, int length)
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
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual new TCertain ForEach(Action<T, int> action) => ForEach(action, 0, _size);

	public virtual new TCertain ForEach(Action<T, int> action, int index) => ForEach(action, index, _size - index);

	public virtual new TCertain ForEach(Action<T, int> action, int index, int length)
	{
		base.ForEach(action, index, length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain GetAfter(IEnumerable<T> collection) => GetAfter(collection, 0, _size);

	public virtual TCertain GetAfter(IEnumerable<T> collection, int index) => GetAfter(collection, index, _size - index);

	public virtual TCertain GetAfter(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length, out var collectionLength);
		return foundIndex == -1 ? new() : GetRange(foundIndex + collectionLength);
	}

	public virtual TCertain GetAfter(TCertain collection) => GetAfter((IEnumerable<T>)collection, 0, _size);

	public virtual TCertain GetAfter(TCertain collection, int index) => GetAfter((IEnumerable<T>)collection, index, _size - index);

	public virtual TCertain GetAfter(TCertain collection, int index, int length) => GetAfter((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection) => GetAfterLast(collection, _size - 1, _size);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index) => GetAfterLast(collection, index, index + 1);

	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length, out var collectionLength);
		return foundIndex == -1 ? new() : GetRange(foundIndex + collectionLength);
	}

	public virtual TCertain GetAfterLast(TCertain collection) => GetAfterLast((IEnumerable<T>)collection, _size - 1, _size);

	public virtual TCertain GetAfterLast(TCertain collection, int index) => GetAfterLast((IEnumerable<T>)collection, index, index + 1);

	public virtual TCertain GetAfterLast(TCertain collection, int index, int length) => GetAfterLast((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetBefore(IEnumerable<T> collection) => GetBefore(collection, 0, _size);

	public virtual TCertain GetBefore(IEnumerable<T> collection, int index) => GetBefore(collection, index, _size - index);

	public virtual TCertain GetBefore(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length);
		return foundIndex == -1 ? this as TCertain ?? throw new InvalidOperationException() : GetRange(0, foundIndex);
	}

	public virtual TCertain GetBefore(TCertain collection) => GetBefore((IEnumerable<T>)collection, 0, _size);

	public virtual TCertain GetBefore(TCertain collection, int index) => GetBefore((IEnumerable<T>)collection, index, _size - index);

	public virtual TCertain GetBefore(TCertain collection, int index, int length) => GetBefore((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection) => GetBeforeLast(collection, _size - 1, _size);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index) => GetBeforeLast(collection, index, index + 1);

	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length);
		return foundIndex == -1 ? this as TCertain ?? throw new InvalidOperationException() : GetRange(0, foundIndex);
	}

	public virtual TCertain GetBeforeLast(TCertain collection) => GetBeforeLast((IEnumerable<T>)collection, _size - 1, _size);

	public virtual TCertain GetBeforeLast(TCertain collection, int index) => GetBeforeLast((IEnumerable<T>)collection, index, index + 1);

	public virtual TCertain GetBeforeLast(TCertain collection, int index, int length) => GetBeforeLast((IEnumerable<T>)collection, index, length);

	public virtual TCertain GetRange(int index, bool alwaysCopy = false) => GetRange(index, _size - index, alwaysCopy);

	public virtual TCertain GetRange(int index, int length, bool alwaysCopy = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return new();
		else if (!alwaysCopy && index == 0 && length == _size && this is TCertain thisList)
			return thisList;
		return GetRangeInternal(index, length);
	}

	public virtual TCertain GetRange(Range range, bool alwaysCopy = false)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException(null);
		var (start, length) = range.GetOffsetAndLength(_size);
		return GetRange(start, length, alwaysCopy);
	}

	private protected abstract TCertain GetRangeInternal(int index, int length);

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
public abstract class BaseList<T, TCertain> : BaseIndexable<T, TCertain>, IList<T>, IList where TCertain : BaseList<T, TCertain>, new()
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

	public abstract int Capacity { get; set; }

	private protected abstract Func<int, TCertain> CapacityCreator { get; }

	private protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected virtual int DefaultCapacity => 32;

	bool System.Collections.IList.IsFixedSize => false;

	bool G.ICollection<T>.IsReadOnly => false;

	bool System.Collections.IList.IsReadOnly => false;

	bool System.Collections.ICollection.IsSynchronized => false;

	object System.Collections.ICollection.SyncRoot => _syncRoot;

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
		return this as TCertain ?? throw new InvalidOperationException();
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
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected void Changed() => ListChanged?.Invoke(this as TCertain ?? throw new InvalidOperationException());

	public virtual void Clear()
	{
		if (_size > 0)
		{
			ClearInternal();
			_size = 0;
		}
	}

	public virtual void Clear(int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		ClearInternal(index, length);
	}

	private protected virtual void ClearInternal() => ClearInternal(0, _size);

	private protected abstract void ClearInternal(int index, int length);

	public virtual object Clone() => Copy();

	public virtual TCertain Concat(TCertain collection) => CollectionCreator(this).AddRange(collection);

	bool System.Collections.IList.Contains(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return Contains((T)item);
		return false;
	}

	public virtual TCertainOutput Convert<TOutput, TCertainOutput>(Func<T, TOutput> converter) where TCertainOutput : BaseList<TOutput, TCertainOutput>, new()
	{
		if (converter == null)
			throw new ArgumentNullException(nameof(converter));
		var list = Activator.CreateInstance(typeof(TCertainOutput), _size) as TCertainOutput ?? throw new InvalidOperationException();
		for (var i = 0; i < _size; i++)
			list.SetInternal(i, converter(GetInternal(i)));
		list._size = _size;
		return list;
	}

	public virtual TCertainOutput Convert<TOutput, TCertainOutput>(Func<T, int, TOutput> converter) where TCertainOutput : BaseList<TOutput, TCertainOutput>, new()
	{
		if (converter == null)
			throw new ArgumentNullException(nameof(converter));
		var list = Activator.CreateInstance(typeof(TCertainOutput), _size) as TCertainOutput ?? throw new InvalidOperationException();
		for (var i = 0; i < _size; i++)
			list.SetInternal(i, converter(GetInternal(i), i));
		list._size = _size;
		return list;
	}

	public virtual TCertain Copy() => CollectionCreator(this);

	private protected abstract void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int length);

	private protected virtual void EnsureCapacity(int min)
	{
		if (Capacity < min)
		{
			var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
			if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
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
		return this as TCertain ?? throw new InvalidOperationException();
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
		return this as TCertain ?? throw new InvalidOperationException();
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

	private protected override TCertain GetRangeInternal(int index, int length)
	{
		var list = CapacityCreator(length);
		Copy(this as TCertain ?? throw new InvalidOperationException(), index, list, 0, length);
		list._size = length;
		return list;
	}

	private protected override Slice<T> GetSliceInternal(int index, int length) => new(this, index, length);

	int System.Collections.IList.IndexOf(object? item)
	{
		if (IsCompatibleObject(item))
			if (item != null)
				return IndexOf((T)item);
		return -1;
	}

	public virtual TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity) EnsureCapacity(_size + 1);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (index < _size)
			Copy(this2, index, this2, index + 1, _size - index);
		SetInternal(index, item);
		_size++;
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
			throw new ArgumentException(null, nameof(item));
		}
	}

	public virtual TCertain Insert(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		return InsertInternal(index, collection);
	}

	private protected virtual TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		var length = list._size;
		if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < _size)
				Copy(this2, index, this2, index + length, _size - index);
			if (this == list)
			{
				Copy(this2, 0, this2, index, index);
				Copy(this2, index + length, this2, index * 2, _size - index);
			}
			else
				Copy(list, 0, this2, index, length);
			_size += length;
		}
		return this2;
	}

	public virtual TCertain Pad(int length) => Pad(length, default!);

	public virtual TCertain Pad(int length, T value)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
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
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
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
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
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
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
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
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
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
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length <= _size)
			return this2;
		while (_size < length)
			Add(value);
		return this2;
	}

	public virtual TCertain Remove(Index index) => Remove(index.GetOffset(_size));

	public virtual TCertain Remove(int index) => Remove(index, _size - index);

	public virtual TCertain Remove(int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length > 0)
		{
			_size -= length;
			if (index < _size)
				Copy(this2, index + length, this2, index, _size - index);
			ClearInternal(_size, length);
		}
		return this2;
	}

	public virtual TCertain Remove(Range range)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException(null);
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
		if (match == null)
			throw new ArgumentNullException(nameof(match));
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
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		_size--;
		if (index < _size)
			Copy(this2, index + 1, this2, index, _size - index);
		SetInternal(_size, default!);
		return this2;
	}

	void System.Collections.IList.RemoveAt(int index) => RemoveAt(index);

	void G.IList<T>.RemoveAt(int index) => RemoveAt(index);

	internal static TCertain RemoveIndexes(TCertain originalList, Queue<int> toRemove)
	{
		var toRemove2 = toRemove.ToList().Sort();
		var result = originalList.CapacityCreator(originalList._size - toRemove2._size);
		var pos = 0;
		for (var i = 0; i < toRemove2._size; i++)
		{
			result.Copy(originalList, pos, result, pos - i, toRemove2[i] - pos);
			pos = toRemove2[i] + 1;
		}
		result._size = originalList._size - toRemove2._size;
		return result;
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

	public virtual TCertain Replace(IEnumerable<T> collection) => ReplaceRangeInternal(0, _size, collection);

	public virtual TCertain Replace(T oldItem, T newItem)
	{
		var result = CollectionCreator(this);
		for (var index = IndexOf(oldItem); index >= 0; index = IndexOf(oldItem, index + 1))
			result.SetInternal(index, newItem);
		return result;
	}

	public virtual TCertain Replace(IEnumerable<T> oldCollection, IEnumerable<T> newCollection)
	{
		var length = oldCollection.Length();
		if (length == 0 || length > _size)
			return CollectionCreator(this);
		var result = CapacityCreator(_size);
		LimitedQueue<T> queue = new(length);
		for (var i = 0; i < length - 1; i++)
			queue.Enqueue(GetInternal(i));
		for (var i = length - 1; i < _size; i++)
		{
			queue.Enqueue(GetInternal(i));
			if (RedStarLinq.Equals(queue, oldCollection))
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
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain ReplaceInPlace(IEnumerable<T> oldCollection, IEnumerable<T> newCollection) => Replace(Replace(oldCollection, newCollection));

	public virtual TCertain ReplaceInPlace(TCertain oldList, TCertain newList) => ReplaceInPlace((IEnumerable<T>)oldList, newList);

	public virtual TCertain ReplaceRange(int index, int length, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		return ReplaceRangeInternal(index, length, collection);
	}

	public virtual TCertain ReplaceRange(int index, int length, TCertain list) => ReplaceRange(index, length, (IEnumerable<T>)list);

	internal virtual TCertain ReplaceRangeInternal(int index, int length, IEnumerable<T> collection)
	{
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (list._size > 0)
		{
			EnsureCapacity(_size + list._size - length);
			if (index + length < _size)
				Copy(this2, index + length, this2, index + list._size, _size - index - length);
			Copy(list, 0, this2, index, list._size);
			_size += list._size - length;
		}
		return this2;
	}

	public virtual TCertain Reverse() => Reverse(0, _size);

	public virtual TCertain Reverse(int index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		return ReverseInternal(index, length);
	}

	private protected abstract TCertain ReverseInternal(int index, int length);

	public virtual TCertain SetAll(T value) => SetAll(value, 0, _size);

	public virtual TCertain SetAll(T value, Index index) => SetAll(value, index.GetOffset(_size));

	public virtual TCertain SetAll(T value, int index) => SetAll(value, index, _size - index);

	public virtual TCertain SetAll(T value, int index, int length)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		var endIndex = index + length;
		for (var i = index; i < endIndex; i++)
			SetInternal(i, value);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain SetAll(T value, Range range) => SetAll(value, CreateVar(range.GetOffsetAndLength(_size), out var range2).Offset, range2.Length);

	internal abstract void SetInternal(int index, T value);

	public virtual TCertain SetOrAdd(int index, T value)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		return SetOrAddInternal(index, value);
	}

	private protected virtual TCertain SetOrAddInternal(int index, T value)
	{
		if (index == _size)
			return Add(value);
		SetInternal(index, value);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain SetRange(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var length = list._size;
		return index + length > _size ? throw new ArgumentException(null) : SetRangeInternal(index, length, list);
	}

	internal virtual TCertain SetRangeAndSizeInternal(int index, int length, TCertain list)
	{
		SetRangeInternal(index, length, list);
		_size = Max(_size, index + length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	internal virtual TCertain SetRangeInternal(int index, int length, TCertain list)
	{
		EnsureCapacity(index + length);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length > 0)
			Copy(list, 0, this2, index, length);
		return this2;
	}

	public virtual TCertain Shuffle()
	{
		Random random = new();
		for (var i = _size; i > 0; i--)
		{
			var swapIndex = random.Next(i);
			(this[swapIndex], this[i - 1]) = (this[i - 1], this[swapIndex]);
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public static List<TCertain> Transpose(List<TCertain> list, bool widen = false)
	{
		if (list._size == 0)
			throw new ArgumentException(null, nameof(list));
		var yCount = widen ? list.Max(x => x._size) : list.Min(x => x._size);
		List<TCertain> new_list = new();
		for (var i = 0; i < yCount; i++)
		{
			new_list.Add(list[0].CapacityCreator(list._size));
			for (var j = 0; j < list._size; j++)
			{
				var temp = list[j];
				new_list[i].Add(temp._size <= i ? default! : temp[i]);
			}
		}
		return new_list;
	}

	public virtual TCertain TrimExcess()
	{
		var threshold = (int)(Capacity * 0.9);
		if (_size < threshold)
			Capacity = _size;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public static implicit operator BaseList<T, TCertain>(T x) => new TCertain().Add(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseBigList<T, TCertain, TLow> : IBigList<T> where TCertain : BaseBigList<T, TCertain, TLow>, new() where TLow : G.IList<T>, new()
{
	public virtual T this[mpz_t index]
	{
		get
		{
			if (index >= Size)
				throw new IndexOutOfRangeException();
			return GetInternal(index);
		}
		set
		{
			if (index >= Size)
				throw new IndexOutOfRangeException();
			SetInternal(index, value);
		}
	}

	public abstract mpz_t Capacity { get; set; }

	public virtual mpz_t Length => Size;

	private protected abstract Func<mpz_t, TCertain> CapacityCreator { get; }

	private protected abstract Func<int, TLow> CapacityLowCreator { get; }

	private protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected abstract Func<IEnumerable<T>, TLow> CollectionLowCreator { get; }

	private protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	private protected virtual mpz_t Size { get; set; } = 0;

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public abstract TCertain Add(T item);

	void IBigCollection<T>.Add(T item) => Add(item);

	public virtual TCertain AddRange(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		var length = bigList.Length;
		if (length == 0)
			return this as TCertain ?? throw new InvalidOperationException();
		return SetRangeAndSizeInternal(Size, Size + length, bigList);
	}

	private protected void Changed() => ListChanged?.Invoke(this as TCertain ?? throw new InvalidOperationException());

	public virtual void Clear()
	{
		if (Size > 0)
		{
			ClearInternal();
			Size = 0;
		}
	}

	public virtual void Clear(mpz_t index, mpz_t length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		ClearInternal(index, length);
	}

	private protected abstract void ClearInternal();

	private protected abstract void ClearInternal(mpz_t index, mpz_t length);

	public virtual bool Contains(T item) => Contains(item, 0, Size);

	public virtual bool Contains(T item, mpz_t index) => Contains(item, index, Size - index);

	public virtual bool Contains(T item, mpz_t index, mpz_t length)
	{
		if (index > Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		for (mpz_t i = new(index); i < index + length; i++)
			if (GetInternal(i)?.Equals(item) ?? item == null)
				return true;
		return false;
	}

	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, Size);

	public virtual bool Contains(IEnumerable<T> collection, mpz_t index) => Contains(collection, index, Size - index);

	public virtual bool Contains(IEnumerable<T> collection, mpz_t index, mpz_t length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
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
		mpz_t j = 0;
		for (mpz_t i = 0; i - j <= length - list.Length; i++)
		{
			if (this[index + i]?.Equals(list[j]) ?? list[j] == null)
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

	public virtual bool Contains(TCertain list) => Contains((IEnumerable<T>)list, 0, Size);

	public virtual bool Contains(TCertain list, mpz_t index) => Contains((IEnumerable<T>)list, index, Size - index);

	public virtual bool Contains(TCertain list, mpz_t index, mpz_t length) => Contains((IEnumerable<T>)list, index, length);

	public virtual bool ContainsAny(IEnumerable<T> collection) => ContainsAny(collection, 0, Size);

	public virtual bool ContainsAny(IEnumerable<T> collection, mpz_t index) => ContainsAny(collection, index, Size - index);

	public virtual bool ContainsAny(IEnumerable<T> collection, mpz_t index, mpz_t length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		var hs = collection.ToHashSet();
		for (mpz_t i = new(index); i < index + length; i++)
			if (hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAny(TCertain list) => ContainsAny((IEnumerable<T>)list, 0, Size);

	public virtual bool ContainsAny(TCertain list, mpz_t index) => ContainsAny((IEnumerable<T>)list, index, Size - index);

	public virtual bool ContainsAny(TCertain list, mpz_t index, mpz_t length) => ContainsAny((IEnumerable<T>)list, index, length);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, Size);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, mpz_t index) => ContainsAnyExcluding(collection, index, Size - index);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, mpz_t index, mpz_t length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		var hs = collection.ToHashSet();
		for (mpz_t i = new(index); i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAnyExcluding(TCertain list) => ContainsAnyExcluding((IEnumerable<T>)list, 0, Size);

	public virtual bool ContainsAnyExcluding(TCertain list, mpz_t index) => ContainsAnyExcluding((IEnumerable<T>)list, index, Size - index);

	public virtual bool ContainsAnyExcluding(TCertain list, mpz_t index, mpz_t length) => ContainsAnyExcluding((IEnumerable<T>)list, index, length);

	private protected abstract void Copy(TCertain sourceBits, mpz_t sourceIndex, TCertain destinationBits, mpz_t destinationIndex, mpz_t length);

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		if (array != null && array.Rank != 1)
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

	public virtual void CopyTo(IBigList<T> list) => CopyTo(list, 0);

	public virtual void CopyTo(IBigList<T> list, mpz_t listIndex) => CopyTo(0, list, listIndex, Length);

	public virtual void CopyTo(mpz_t index, IBigList<T> list, mpz_t listIndex, mpz_t length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(mpz_t index, T[] array, int arrayIndex, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		CopyToInternal(index, array, arrayIndex, length);
	}

	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	public virtual void CopyTo(T[] array, int arrayIndex)
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		CopyTo(0, array, arrayIndex, (int)Length);
	}

	private protected virtual void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, (int)Length);
	}

	private protected abstract void CopyToInternal(mpz_t index, T[] array, int arrayIndex, int length);

	private protected abstract void CopyToInternal(mpz_t index, IBigList<T> list, mpz_t listIndex, mpz_t length);

	private protected virtual void EnsureCapacity(mpz_t min)
	{
		if (Capacity < min)
		{
			var newCapacity = Size == 0 ? DefaultCapacity : Size * 2;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private protected abstract T GetInternal(mpz_t index, bool invoke = true);

	public virtual TCertain GetRange(mpz_t index, bool alwaysCopy = false) => GetRange(index, Size - index, alwaysCopy);

	public virtual TCertain GetRange(mpz_t index, mpz_t length, bool alwaysCopy = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		if (length == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && length == Size && this is TCertain thisList)
			return thisList;
		var list = CapacityCreator(length);
		Copy(this as TCertain ?? throw new InvalidOperationException(), index, list, 0, length);
		list.Size = length;
		return list;
	}

	public virtual mpz_t IndexOf(T item) => IndexOf(item, 0, Size);

	public virtual mpz_t IndexOf(T item, mpz_t index) => IndexOf(item, index, Size - index);

	public virtual mpz_t IndexOf(T item, mpz_t index, mpz_t length)
	{
		if (index > Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0 || index > Size - length)
			throw new ArgumentOutOfRangeException(nameof(length));
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
			for (mpz_t i = new(index); i < index + length; i++)
				if (GetInternal(i)?.Equals(item) ?? false)
					return i;
			return -1;
		}
	}

	void IBigList<T>.Insert(mpz_t index, T item) => Add(item);

	public virtual TCertain Remove(mpz_t index) => Remove(index, Size - index);

	public virtual TCertain Remove(mpz_t index, mpz_t length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length > 0)
		{
			Size -= length;
			if (index < Size)
				Copy(this2, index + length, this2, index, Size - index);
			ClearInternal(Size, length);
		}
		return this2;
	}

	public virtual TCertain RemoveAt(mpz_t index)
	{
		if ((uint)index >= (uint)Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		Size--;
		if (index < Size)
			Copy(this2, index + 1, this2, index, Size - index);
		SetInternal(Size, default!);
		return this2;
	}

	void IBigList<T>.RemoveAt(mpz_t index) => RemoveAt(index);

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

	public virtual TCertain Replace(IEnumerable<T> collection)
	{
		if (collection is IBigCollection<T> col)
		{
			col.CopyTo(this, 0);
			Size = col.Length;
		}
		else
		{
			mpz_t i = 0;
			foreach (var item in collection)
			{
				SetInternal(i, item);
				i++;
			}
			Size = i;
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected abstract void SetInternal(mpz_t index, T value);

	public virtual TCertain SetRange(mpz_t index, IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (index < 0 || index > Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		if (index + bigList.Length > Size)
			throw new ArgumentException(null);
		return SetRangeInternal(index, bigList);
	}

	internal virtual TCertain SetRangeAndSizeInternal(mpz_t index, mpz_t length, TCertain list)
	{
		SetRangeInternal(index, list);
		Size = RedStarLinq.Max(Size, length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	internal virtual TCertain SetRangeAndSizeInternal(mpz_t index, TCertain list)
	{
		SetRangeInternal(index, list);
		Size = RedStarLinq.Max(Size, index + list.Length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected virtual TCertain SetRangeInternal(mpz_t index, TCertain bigList)
	{
		var length = bigList.Length;
		if (length == 0)
			return this as TCertain ?? throw new InvalidOperationException();
		EnsureCapacity(index + length);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length > 0)
			Copy(bigList, 0, this2, index, length);
		return this2;
	}

	public virtual T[] ToArray()
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		var length = (int)Length;
		var array = new T[length];
		CopyToInternal(0, array, 0, length);
		return array;
	}

	public virtual TCertain TrimExcess()
	{
		var threshold = (int)(Capacity * 0.9);
		if (Size < threshold)
			Capacity = Size;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>, IEnumerator
	{
		private readonly BaseBigList<T, TCertain, TLow> list;
		private mpz_t index;
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
			if (index >= list.Size)
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
			index = list.Size + 1;
			current = default!;
			return false;
		}

		public readonly T Current => current;

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list.Size + 1)
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

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BigList<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow> where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected List<TCertain>? high;
	private protected BigSumList? highCapacity;
	private protected mpz_t _capacity = 0;
	private protected mpz_t fragment = 1;
	private protected bool isHigh;

	public BigList() : this(-1) { }

	public BigList(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		low = new();
		high = null;
		highCapacity = null;
		fragment = 1;
		isHigh = false;
		Size = 0;
		_capacity = 0;
	}

	public BigList(mpz_t capacity, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity <= CapacityFirstStep)
		{
			low = CapacityLowCreator((int)capacity);
			high = null;
			highCapacity = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << (GetArrayLength((capacity - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			while (ProperFragment * (CapacityStep * 3 >> 2) < capacity)
				fragment <<= CapacityStepBitLength;
			var quotient = capacity.Divide(ProperFragment, out var remainder);
			var highCount = (int)GetArrayLength(capacity, ProperFragment);
			high = new(highCount);
			highCapacity = new();
			for (mpz_t i = 0; i < quotient; i++)
			{
				high.Add(CapacityCreator(ProperFragment));
				highCapacity.Add(ProperFragment);
			}
			if (remainder != 0)
			{
				high.Add(CapacityCreator(remainder));
				highCapacity.Add(remainder);
			}
			isHigh = true;
		}
		Size = 0;
		_capacity = capacity;
	}

	public BigList(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(collection == null ? throw new ArgumentNullException(nameof(collection)) : List<T>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : 0, capacityFirstStepBitLength, capacityStepBitLength)
	{
		var en = collection.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	public BigList(mpz_t capacity, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(capacity, capacityFirstStepBitLength, capacityStepBitLength)
	{
		var en = collection.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	public override mpz_t Capacity
	{
		get => _capacity;
		set
		{
			if (value < Size)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (value == _capacity)
				return;
			if (value <= 0)
			{
				low = new();
				high = null;
				highCapacity = null;
				isHigh = false;
			}
			else if (value <= CapacityFirstStep)
			{
				//try
				//{
				//	throw new ExperimentalException();
				//}
				//catch
				//{
				//}
				low = GetFirstLists();
				var value2 = (int)value;
				low.Capacity = value2;
				high = null;
				highCapacity = null;
				isHigh = false;
			}
			else if (!isHigh && low != null)
			{
				fragment = (mpz_t)1 << (GetArrayLength((value - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				while (ProperFragment * (CapacityStep * 3 >> 2) < value)
					fragment <<= CapacityStepBitLength;
				var highCount = (int)GetArrayLength(value, ProperFragment);
				high = new(highCount);
				highCapacity = new();
				for (mpz_t i = 0; i < value / ProperFragment; i++)
				{
					high.Add(CapacityCreator(ProperFragment));
					highCapacity.Add(ProperFragment);
				}
				if (value % ProperFragment != 0)
				{
					high.Add(CapacityCreator(value % ProperFragment));
					highCapacity.Add(value % ProperFragment);
				}
				high[0].AddRange(low);
				low = null;
				isHigh = true;
			}
			else if (high != null && highCapacity != null)
			{
				var oldFragment = fragment;
				fragment = (mpz_t)1 << (GetArrayLength((value - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				while (ProperFragment * (CapacityStep * 3 >> 2) < value)
					fragment <<= CapacityStepBitLength;
				if (fragment > oldFragment)
					goto l0;
				if (fragment < oldFragment)
					goto l1;
				high.Capacity = (int)GetArrayLength(value, ProperFragment);
				high[^1].Capacity = (high.Length < high.Capacity || value % ProperFragment == 0) ? ProperFragment : value % ProperFragment;
				for (var i = high.Length; i < high.Capacity - 1; i++)
				{
					high.Add(CapacityCreator(ProperFragment));
					highCapacity.Add(ProperFragment);
				}
				if (high.Length < high.Capacity)
				{
					var remainder = value % ProperFragment == 0 ? ProperFragment : value % ProperFragment;
					high.Add(CapacityCreator(remainder));
					highCapacity.Add(remainder);
				}
				return;
			l0:
				do
				{
					oldFragment <<= CapacityStepBitLength;
					var highCount = (int)RedStarLinq.Min(GetArrayLength(value, oldFragment), CapacityStep);
					high = new(highCount) { this as TCertain ?? throw new InvalidOperationException() };
					new Chain(1, high.Capacity - 2).ForEach(_ => high.Add(CapacityCreator(oldFragment)));
					highCapacity = new(RedStarLinq.FillArray(high.Capacity - 1, _ => new mpz_t(oldFragment)));
					var remainder = (oldFragment < fragment || value % oldFragment == 0) ? oldFragment : value % oldFragment;
					high.Add(CapacityCreator(remainder));
					highCapacity.Add(remainder);
				} while (oldFragment < fragment);
				return;
			l1:
				do
				{
					oldFragment >>= CapacityStepBitLength;
					high = high[0].high!;
				} while (oldFragment > fragment);
				high[0].Capacity = value;
			}
			_capacity = value;
		}
	}

	private protected virtual int CapacityFirstStepBitLength { get; init; } = 16;

	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	private protected virtual int CapacityStepBitLength { get; init; } = 16;

	private protected virtual int CapacityStep => 1 << CapacityStepBitLength;

	private protected virtual mpz_t ProperFragment
	{
		get
		{
			if (fragment == 1)
				return 1;
			var steps = ((fragment - 1).BitLength - CapacityFirstStepBitLength) / CapacityStepBitLength;
			return ((mpz_t)1 << steps * (CapacityStepBitLength - 2) + (CapacityFirstStepBitLength - 2)) * mpz_t.Power(3, steps + 1);
		}
	}

	public override TCertain Add(T item)
	{
		EnsureCapacity(Size + 1);
		if (!isHigh && low != null)
			low.Add(item);
		else if (high != null)
			high[(int)(Size / fragment)].Add(item);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Size++;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected override void ClearInternal()
	{
		if (!isHigh && low != null)
			low.Clear();
		else
		{
			high?.Clear();
			highCapacity?.Clear();
		}
	}

	private protected override void ClearInternal(mpz_t index, mpz_t length)
	{
		if (!isHigh && low != null)
			low.Clear((int)index, (int)length);
		else if (high != null && highCapacity != null)
		{
			var quotient = highCapacity.IndexOfNotGreaterSum(index, out var remainder);
			var quotient2 = highCapacity.IndexOfNotGreaterSum(index + length - 1);
			if (quotient == quotient2)
			{
				high[quotient].ClearInternal(remainder, length);
				return;
			}
			var previousPart = highCapacity[quotient] - remainder;
			high[quotient].ClearInternal(remainder, previousPart);
			for (var i = quotient + 1; i < quotient2; i++)
			{
				high[i].ClearInternal(0, highCapacity[i]);
				previousPart += highCapacity[i];
			}
			high[quotient2].ClearInternal(0, length - previousPart);
		}
	}

	private protected override void Copy(TCertain sourceBits, mpz_t sourceIndex, TCertain destinationBits, mpz_t destinationIndex, mpz_t length)
	{
		CheckParams(sourceBits, sourceIndex, destinationBits, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		if (!sourceBits.isHigh && sourceBits.low != null && !destinationBits.isHigh && destinationBits.low != null)
		{
			destinationBits.low.SetRangeAndSizeInternal((int)destinationIndex, (int)length, sourceBits.low.GetRange((int)sourceIndex, (int)length));
			destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
			return;
		}
		if (sourceBits.fragment > destinationBits.fragment && sourceBits.isHigh && sourceBits.high != null && sourceBits.highCapacity != null)
		{
			int index = sourceBits.highCapacity.IndexOfNotGreaterSum(sourceIndex, out var remainder), index2 = sourceBits.highCapacity.IndexOfNotGreaterSum(sourceIndex + length - 1);
			if (index == index2)
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, length);
			else
			{
				var previousPart = sourceBits.highCapacity[index] - remainder;
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits.high[i], 0, destinationBits, destinationIndex + previousPart, sourceBits.highCapacity[i]);
					previousPart += sourceBits.highCapacity[i];
				}
				Copy(sourceBits.high[index2], 0, destinationBits, destinationIndex + previousPart, length - previousPart);
			}
			destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
			return;
		}
		if (destinationBits.fragment > sourceBits.fragment && destinationBits.isHigh && destinationBits.high != null && destinationBits.highCapacity != null)
		{
			int index = destinationBits.highCapacity.IndexOfNotGreaterSum(destinationIndex, out var remainder), index2 = destinationBits.highCapacity.IndexOfNotGreaterSum(destinationIndex + length - 1);
			if (index == index2)
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, length);
			else
			{
				var previousPart = destinationBits.highCapacity[index] - remainder;
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits, sourceIndex + previousPart, destinationBits.high[i], 0, destinationBits.highCapacity[i]);
					previousPart += destinationBits.highCapacity[i];
				}
				Copy(sourceBits, sourceIndex + previousPart, destinationBits.high[index2], 0, length - previousPart);
			}
			destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
			return;
		}
		if (!(sourceBits.isHigh && sourceBits.high != null && sourceBits.highCapacity != null && destinationBits.isHigh && destinationBits.high != null && destinationBits.highCapacity != null && sourceBits.fragment == destinationBits.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Copy(sourceBits.high, sourceBits.highCapacity, sourceIndex, destinationBits.high, destinationBits.highCapacity, destinationIndex, length, sourceBits.fragment);
		destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
	}

	private protected virtual void Copy(List<TCertain> sourceBits, BigSumList sourceCount, mpz_t sourceIndex, List<TCertain> destinationBits, BigSumList destinationCount, mpz_t destinationIndex, mpz_t length, mpz_t fragment)
	{
		var sourceIntIndex = sourceCount.IndexOfNotGreaterSum(sourceIndex, out var sourceBitsIndex);               // Целый индекс в исходном массиве.
		var destinationIntIndex = destinationCount.IndexOfNotGreaterSum(destinationIndex, out var destinationBitsIndex);     // Целый индекс в целевом массиве.
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		var sourceEndIntIndex = sourceCount.IndexOfNotGreaterSum(sourceEndIndex, out var sourceEndBitsIndex);  // Индекс инта последнего бита.
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		var destinationEndIntIndex = destinationCount.IndexOfNotGreaterSum(destinationEndIndex, out var destinationEndBitsIndex);  // Индекс инта последнего бита.
		if (sourceEndIntIndex == sourceIntIndex)
		{
			int index = destinationCount.IndexOfNotGreaterSum(destinationIndex, out var remainder), index2 = destinationCount.IndexOfNotGreaterSum(destinationIndex + length - 1);
			if (index == index2)
				Copy(sourceBits[sourceIntIndex], sourceIndex, destinationBits[index], remainder, length);
			else
			{
				var previousPart = destinationCount[index] - remainder;
				Copy(sourceBits[sourceIntIndex], sourceIndex, destinationBits[index], remainder, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits[sourceIntIndex], sourceIndex + previousPart, destinationBits[i], 0, destinationCount[i]);
					previousPart += destinationCount[i];
				}
				Copy(sourceBits[sourceIntIndex], sourceIndex + previousPart, destinationBits[index2], 0, length - previousPart);
			}
		}
		else if (destinationEndIntIndex == destinationIntIndex)
		{
			int index = sourceCount.IndexOfNotGreaterSum(sourceIndex, out var remainder), index2 = sourceCount.IndexOfNotGreaterSum(sourceIndex + length - 1);
			if (index == index2)
				Copy(sourceBits[index], remainder, destinationBits[destinationIntIndex], destinationIndex, length);
			else
			{
				var previousPart = sourceCount[index] - remainder;
				Copy(sourceBits[index], remainder, destinationBits[destinationIntIndex], destinationIndex, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits[i], 0, destinationBits[destinationIntIndex], destinationIndex + previousPart, sourceCount[i]);
					previousPart += sourceCount[i];
				}
				Copy(sourceBits[index2], 0, destinationBits[destinationIntIndex], destinationIndex + previousPart, length - previousPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
		{
			var buff = destinationBits[destinationIntIndex].GetRange(0, destinationBitsIndex);
			buff.AddRange(sourceBits[sourceIntIndex].GetRange(sourceBitsIndex));
			for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; sourceCurrentIntIndex < sourceEndIntIndex + 1 || destinationCurrentIntIndex < destinationEndIntIndex;)
			{
				var currentLength = destinationCount[destinationCurrentIntIndex];
				if (buff.Length < currentLength)
				{
					var sourceElem = sourceBits[sourceCurrentIntIndex++];
					buff.AddRange(sourceCurrentIntIndex == sourceEndIntIndex ? sourceElem.GetRange(0, destinationEndBitsIndex + 1) : sourceElem);
				}
				if (buff.Length >= currentLength && destinationCurrentIntIndex < destinationEndIntIndex)
				{
					destinationBits[destinationCurrentIntIndex++] = buff.GetRange(0, currentLength, true);
					buff.Remove(0, currentLength);
				}
			}
			destinationBits[destinationEndIntIndex].SetRangeAndSizeInternal(0, buff.GetRange(0, destinationEndBitsIndex + 1));
		}
		else
		{
			var buff = sourceBits[sourceEndIntIndex].GetRange(0, sourceEndBitsIndex + 1);
			buff.AddRange(destinationBits[destinationEndIntIndex].GetRange(destinationEndBitsIndex + 1));
			buff.Capacity = fragment << 1;
			for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; sourceCurrentIntIndex > sourceIntIndex - 1 || destinationCurrentIntIndex > destinationIntIndex;)
			{
				var currentLength = destinationCount[destinationCurrentIntIndex];
				if (buff.Length < currentLength)
				{
					var sourceElem = sourceBits[sourceCurrentIntIndex--];
					buff.high?.Insert(0, sourceElem);
				}
				if (buff.Length >= currentLength && destinationCurrentIntIndex > destinationIntIndex)
				{
					destinationBits[destinationCurrentIntIndex--] = buff.GetRange(buff.Size - fragment, true);
					buff.Remove(buff.Size - fragment);
				}
			}
			destinationBits[destinationIntIndex].SetRangeAndSizeInternal(destinationBitsIndex, buff.GetRange(buff.Size - destinationBitsIndex));
		}
	}

	private static void CheckParams(TCertain sourceBits, mpz_t sourceIndex, TCertain destinationBits, mpz_t destinationIndex, mpz_t length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Capacity == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Capacity == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceBits.Capacity)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationBits.Capacity)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	private protected override void CopyToInternal(mpz_t index, IBigList<T> list, mpz_t listIndex, mpz_t length)
	{
		if (length == 0)
			return;
		if (!isHigh && low != null)
		{
			int index2 = (int)index, count2 = (int)length;
			for (var i = 0; i < count2; i++)
				list[listIndex + i] = low[index2 + i];
		}
		else if (high != null)
		{
			var intIndex = (int)index.Divide(fragment, out var bitsIndex);
			var endIntIndex = (int)(index + length - 1).Divide(fragment, out var endBitsIndex);
			if (endIntIndex == intIndex)
			{
				high[intIndex].CopyToInternal(bitsIndex, list, listIndex, length);
				return;
			}
			high[intIndex].CopyToInternal(bitsIndex, list, listIndex, fragment - bitsIndex);
			var destIndex = listIndex + fragment - bitsIndex;
			for (var i = intIndex + 1; i < endIntIndex; i++, destIndex += fragment)
				high[i].CopyToInternal(0, list, destIndex, fragment);
			high[endIntIndex].CopyToInternal(0, list, destIndex, endBitsIndex + 1);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void CopyToInternal(mpz_t index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex + i] = GetInternal(index + i);
	}

	private protected virtual TLow GetFirstLists()
	{
		if (!isHigh && low != null)
			return low;
		else if (high != null)
			return high[0].GetFirstLists();
		else
			return new();
	}

	private protected override T GetInternal(mpz_t index, bool invoke = true)
	{
		if (!isHigh && low != null)
			return low.GetInternal((int)index);
		else if (high != null)
			return high.GetInternal((int)(index / fragment)).GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void SetInternal(mpz_t index, T value)
	{
		if (!isHigh && low != null)
			low.SetInternal((int)index, value);
		else if (high != null)
			high.GetInternal((int)(index / fragment)).SetInternal(index % fragment, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseSet<T, TCertain> : BaseList<T, TCertain>, ISet<T> where TCertain : BaseSet<T, TCertain>, new()
{
	public override TCertain Add(T item)
	{
		TryAdd(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	bool ISet<T>.Add(T item) => TryAdd(item);

	public override Span<T> AsSpan(int index, int length) => List<T>.ReturnOrConstruct(this).AsSpan(index, length);

	private protected override void ClearInternal(int index, int length)
	{
		for (var i = index; i < index + length; i++)
			SetInternal(i, default!);
	}

	public override bool Contains(T? item, int index, int length) => item != null && IndexOf(item, index, length) >= 0;

	private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (source != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				destination.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		else
			for (var i = length - 1; i >= 0; i--)
				destination.SetInternal(destinationIndex + i, source.GetInternal(sourceIndex + i));
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		destination.Changed();
	}

	public virtual TCertain ExceptWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = other.ToHashSet();
		return FilterInPlace(x => !set.Contains(x));
	}

	void ISet<T>.ExceptWith(IEnumerable<T> other) => ExceptWith(other);

	public override TCertain Insert(int index, T item)
	{
		if (!Contains(item))
		{
			base.Insert(index, item);
			_size--;
		}
		return this as TCertain ?? throw new InvalidOperationException();
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

	private protected override int LastIndexOfInternal(T item, int index, int length) => throw new NotSupportedException();

	public virtual bool Overlaps(IEnumerable<T> other)
	{
		foreach (var item in other)
			if (Contains(item))
				return true;
		return false;
	}

	public override TCertain Repeat(int length) => length switch
	{
		0 => new(),
		1 => Copy(),
		_ => throw new ArgumentOutOfRangeException(nameof(length)),
	};

	private protected override TCertain ReverseInternal(int index, int length) => throw new NotSupportedException();

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

	public virtual TCertain SymmetricExceptWith(IEnumerable<T> other)
	{
		var temp = CollectionCreator(other);
		temp.ExceptWith(this);
		ExceptWith(other);
		return AddRange(temp);
	}

	void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => SymmetricExceptWith(other);

	public virtual bool TryAdd(T item) => TryAdd(item, out _);

	public abstract bool TryAdd(T item, out int index);

	public virtual bool TryGetIndexOf(T item, out int index) => (index = IndexOf(item)) >= 0;

	public virtual TCertain UnionWith(IEnumerable<T> other)
	{
		foreach (var item in other)
			TryAdd(item);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	void ISet<T>.UnionWith(IEnumerable<T> other) => UnionWith(other);
}
