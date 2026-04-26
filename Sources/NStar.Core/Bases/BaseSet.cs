namespace NStar.Core;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseSet<T, TCertain> : BaseList<T, TCertain>, ISet<T> where TCertain : BaseSet<T, TCertain>, new()
{
	/// <summary>
	/// Добавляет в конец данного множества указанный элемент, если его там еще нет.
	/// </summary>
	/// <param name="item">Элемент для добавления.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public override TCertain Add(T item)
	{
		TryAdd(item);
		return (TCertain)this;
	}

	bool ISet<T>.Add(T item) => TryAdd(item);

	/// <summary>
	/// Для множеств является псевдонимом <see cref="UnionWith(IEnumerable{T})"/>.
	/// </summary>
	/// <param name="collection">Последовательность для добавления.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public override TCertain AddRange(IEnumerable<T> collection) => UnionWith(collection);

	/// <summary>
	/// Для множеств является псевдонимом <see cref="UnionWith(IEnumerable{T})"/>.
	/// </summary>
	/// <param name="array">Последовательность для добавления в виде массива.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public override TCertain AddRange(T[] array) => UnionWith(array);

	protected override void AddSeriesInternal(T item, int length)
	{
		if (length != 0)
			Add(item);
	}

	/// <inheritdoc/>
	public override Memory<T> AsMemory(int index, int length) => List<T>.ReturnOrConstruct(this).AsMemory(index, length);

	/// <inheritdoc/>
	public override Span<T> AsSpan(int index, int length) => List<T>.ReturnOrConstruct(this).AsSpan(index, length);

	/// <inheritdoc/>
	public override void Clear(bool deep) => base.Clear(true);

	protected override void ClearInternal(int index, int length)
	{
		for (var i = index; i < index + length; i++)
			SetInternal(i, default!);
	}

	/// <inheritdoc/>
	public override bool Contains(T? item, int index, int length) => item is not null && IndexOf(item, index, length) >= 0;

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
	}

	/// <summary>
	/// Удаляет из данного множества все элементы, <b>при</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain ExceptWith(IEnumerable<T> other)
	{
		if (other is ISet<T> set)
			return FilterInPlace(x => !set.Contains(x));
		var set2 = other.ToHashSet();
		return FilterInPlace(x => !set2.Contains(x));
	}

	void ISet<T>.ExceptWith(IEnumerable<T> other) => ExceptWith(other);

	/// <summary>
	/// Заменяет содержимое данного множества на пустое множество или множество из одного указанного элемента.
	/// (Выдает ошибку, если <paramref name="length"/> больше 1.)
	/// </summary>
	/// <param name="item">Элемент для добавления (см. <see cref="RedStarLinq.Fill{T}(T, int)"/>).</param>
	/// <param name="length">0 или 1 (иначе метод выдаст ошибку).</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public override TCertain FillInPlace(T item, int length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(length, 1);
		return base.FillInPlace(item, length);
	}

	/// <summary>
	/// Если указанного элемента нет в данной коллекции, вставляет внутрь нее указанный элемент,
	/// так что он оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// В противном случае ничего не делает.
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается элемент.</param>
	/// <param name="item">Элемент для вставки.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public override TCertain Insert(int index, T item)
	{
		if (!Contains(item))
			base.Insert(index, item);
		return (TCertain)this;
	}

	/// <summary>
	/// Удаляет из данного множества все элементы, <b>от</b>сутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для удаления <b>от</b>сутствующих в ней элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain IntersectWith(IEnumerable<T> other)
	{
		if (other is not ISet<T> set)
			set = other.ToHashSet();
		return FilterInPlace(set.Contains);
	}

	void ISet<T>.IntersectWith(IEnumerable<T> other) => IntersectWith(other);

	/// <summary>
	/// Проверяет, содержит ли указанная последовательность все элементы данного множества
	/// и хотя бы один элемент, <b>от</b>сутствующий в данном множестве (дубликаты не считаются).
	/// </summary>
	/// <param name="other">Последовательность для проверки, содержатся ли в ней элементы данного множества.</param>
	/// <returns>Результат проверки - <see langword="true"/> или <see langword="false"/>.</returns>
	public virtual bool IsProperSubsetOf(IEnumerable<T> other) => !SetEquals(other is ISet<T> set ? set : set = CollectionCreator(other)) && IsSubsetOf(set);

	/// <summary>
	/// Проверяет, содержит ли данное множество все элементы указанной последовательности
	/// и хотя бы один элемент, <b>от</b>сутствующий в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для проверки, содержатся ли ее элементы в данном множестве.</param>
	/// <returns>Результат проверки - <see langword="true"/> или <see langword="false"/>.</returns>
	public virtual bool IsProperSupersetOf(IEnumerable<T> other) => !SetEquals(other) && IsSupersetOf(other);

	/// <summary>
	/// Проверяет, содержит ли указанная последовательность все элементы данного множества и, возможно, какие-то еще.
	/// </summary>
	/// <param name="other">Последовательность для проверки, содержатся ли в ней элементы данного множества.</param>
	/// <returns>Результат проверки - <see langword="true"/> или <see langword="false"/>.</returns>
	public virtual bool IsSubsetOf(IEnumerable<T> other) => (other is ISet<T> set ? set : CollectionCreator(other)).IsSupersetOf(this);

	/// <summary>
	/// Проверяет, содержит ли данное множество все элементы указанной последовательности и, возможно, какие-то еще.
	/// </summary>
	/// <param name="other">Последовательность для проверки, содержатся ли ее элементы в данном множестве.</param>
	/// <returns>Результат проверки - <see langword="true"/> или <see langword="false"/>.</returns>
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

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain Pad(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain PadInPlace(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain PadLeft(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain PadLeftInPlace(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain PadRight(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain PadRightInPlace(int length, T value) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков, а не множеств.");

	/// <summary>
	/// Этот метод не поддерживается в этой коллекции.
	/// Если он нужен вам, используйте один из видов списков, а не множеств.
	/// (Выбрасывает исключение NotSupportedException.)
	/// </summary>
	public override TCertain Repeat(int length) => length switch
	{
		0 => new(),
		1 => Copy(),
		_ => throw new ArgumentOutOfRangeException(nameof(length)),
	};

	/// <inheritdoc/>
	public override TCertain Replace(IEnumerable<T> collection)
	{
		if (ReferenceEquals(this, collection))
			return (TCertain)this;
		ClearInternal();
		return AddRange(collection);
	}

	/// <inheritdoc/>
	public override TCertain Replace(ReadOnlySpan<T> span)
	{
		ClearInternal();
		return AddRange(span);
	}

	protected override void ReplaceRangeInternal(int index, int length, IEnumerable<T> collection)
	{
		if (collection is not TCertain list)
			list = CollectionCreator(collection).ExceptWith(GetSlice(0, index)).ExceptWith(GetSlice(index + length));
		base.ReplaceRangeInternal(index, length, list);
	}

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

	protected override void SetRangeInternal(int index, int length, TCertain list) => base.SetRangeInternal(index, CreateVar(CollectionCreator(list).ExceptWith(GetSlice(0, index)).ExceptWith(GetSlice(index + length)), out var list2).Length, list2);

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

	/// <summary>
	/// Добавляет в конец данного множества указанный элемент, если его там еще нет.
	/// </summary>
	/// <param name="item">Элемент для добавления.</param>
	/// <returns>В отличие от <see cref="Add(T)"/>, возвращает НЕ данную коллекцию, а флаг, удалось ли добавить элемент:
	/// <see langword="true"/>, если он был добавлен, и <see langword="false"/>, если он уже был во множестве.</returns>
	public virtual bool TryAdd(T item) => TryAdd(item, out _);

	/// <summary>
	/// Добавляет в конец данного множества указанный элемент, если его там еще нет, и позволяет получить его индекс.
	/// </summary>
	/// <param name="item">Элемент для добавления.</param>
	/// <param name="index">Индекс элемента, существовавшего во множестве или добавленного.</param>
	/// <returns>Флаг, удалось ли добавить элемент:
	/// <see langword="true"/>, если он был добавлен, и <see langword="false"/>, если он уже был во множестве.</returns>
	public abstract bool TryAdd(T item, out int index);

	public virtual bool TryGetIndexOf(T item, out int index) => (index = IndexOf(item)) >= 0;

	/// <summary>
	/// Добавляет в конец данного множества все элементы, отсутствующие в нем, но присутствующие в указанной последовательности.
	/// </summary>
	/// <param name="other">Последовательность для добавления элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain UnionWith(IEnumerable<T> other)
	{
		foreach (var item in other)
			TryAdd(item);
		return (TCertain)this;
	}

	void ISet<T>.UnionWith(IEnumerable<T> other) => UnionWith(other);
}
