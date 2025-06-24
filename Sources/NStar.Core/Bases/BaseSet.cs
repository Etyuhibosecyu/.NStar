namespace NStar.Core;

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

	protected override TCertain ReplaceRangeInternal(int index, int length, IEnumerable<T> collection) => base.ReplaceRangeInternal(index, length, collection is TCertain list ? list : CollectionCreator(collection).ExceptWith(GetSlice(0, index)).ExceptWith(GetSlice(index + length)));

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

	protected override TCertain SetRangeInternal(int index, int length, TCertain list) => base.SetRangeInternal(index, CreateVar(CollectionCreator(list).ExceptWith(GetSlice(0, index)).ExceptWith(GetSlice(index + length)), out var list2).Length, list2);

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
