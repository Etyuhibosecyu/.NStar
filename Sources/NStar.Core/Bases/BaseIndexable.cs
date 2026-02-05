namespace NStar.Core;

/// <summary>
/// Представляет базовый абстрактный класс для всех коллекций в фреймворке .NStar, которые могут быть проиндексированы.
/// Сюда относятся: разнообразные списки, буферы, срез, строка и прочие.
/// Тем не менее, в некоторых коллекциях, технически относимых сюда (например, ParallelHashSet&lt;T&gt;),
/// на индексацию лучше забить, так как она не имеет практического смысла.
/// Сюда не относятся коллекции, стоящие в принципе отдельно от индексируемых,
/// для которых не имеют смысла практически никакие из присутствующих в этом классе методов,
/// например, <see cref="Stack{T}"/>, <see cref="Queue{T}"/>, <see cref="Chain"/> и т. д.
/// </summary>
/// <typeparam name="T">Тип элемента данной коллекции.</typeparam>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseIndexable<T> : IReadOnlyList<T>, IDisposable
{
	protected int _size;
	[NonSerialized]
	private protected object _syncRoot = new();

	/// <inheritdoc cref="G.IReadOnlyList{T}.this[int]"/>
	public virtual T this[Index index, bool invoke = false]
	{
		get
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			return GetInternal(index2);
		}
		set => throw new NotSupportedException("Это действие не поддерживается в этой коллекции."
			+ " Если оно нужно вам, используйте один из видов коллекций, наследующихся от BaseList.");
	}

	T G.IReadOnlyList<T>.this[int index] => this[index];

	/// <summary>
	/// Не нуждается в описании.
	/// </summary>
	public virtual int Length => _size;

	protected static bool IsCompatibleObject(object? value) => value is T || value is null && default(T) is null;

	/// <summary>
	/// Преобразует данную коллекцию в структуру <see cref="Memory{T}"/>.
	/// </summary>
	/// <returns><see cref="Memory{T}"/></returns>
	public virtual Memory<T> AsMemory() => AsMemory(0, _size);

	/// <summary>
	/// Преобразует диапазон данной коллекции от указанного индекса и до конца в структуру <see cref="Memory{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона в виде структуры <see cref="Index"/>.</param>
	/// <returns><see cref="Memory{T}"/></returns>
	public virtual Memory<T> AsMemory(Index index) => AsMemory(index.GetOffset(_size));

	/// <summary>
	/// Преобразует диапазон данной коллекции от указанного индекса и до конца в структуру <see cref="Memory{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns><see cref="Memory{T}"/></returns>
	public virtual Memory<T> AsMemory(int index) => AsMemory(index, _size - index);

	/// <summary>
	/// Преобразует диапазон данной коллекции в структуру <see cref="Memory{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <returns><see cref="Memory{T}"/></returns>
	public abstract Memory<T> AsMemory(int index, int length);

	/// <summary>
	/// Преобразует диапазон данной коллекции в структуру <see cref="Memory{T}"/>.
	/// </summary>
	/// <param name="range">Преобразуемый диапазон в виде структуры <see cref="Range"/>.</param>
	/// <returns><see cref="Memory{T}"/></returns>
	public virtual Memory<T> AsMemory(Range range) => AsMemory()[range];

	/// <summary>
	/// Преобразует данную коллекцию в структуру <see cref="Span{T}"/>.
	/// </summary>
	/// <returns><see cref="Span{T}"/></returns>
	public virtual Span<T> AsSpan() => AsSpan(0, _size);

	/// <summary>
	/// Преобразует диапазон данной коллекции от указанного индекса и до конца в структуру <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона в виде структуры <see cref="Index"/>.</param>
	/// <returns><see cref="Span{T}"/></returns>
	public virtual Span<T> AsSpan(Index index) => AsSpan(index.GetOffset(_size));

	/// <summary>
	/// Преобразует диапазон данной коллекции от указанного индекса и до конца в структуру <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns><see cref="Span{T}"/></returns>
	public virtual Span<T> AsSpan(int index) => AsSpan(index, _size - index);

	/// <summary>
	/// Преобразует диапазон данной коллекции в структуру <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <returns><see cref="Span{T}"/></returns>
	public abstract Span<T> AsSpan(int index, int length);

	/// <summary>
	/// Преобразует диапазон данной коллекции в структуру <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="range">Преобразуемый диапазон в виде структуры <see cref="Range"/>.</param>
	/// <returns><see cref="Span{T}"/></returns>
	public virtual Span<T> AsSpan(Range range) => AsSpan()[range];

	/// <summary>
	/// Проверяет, включает ли данная коллекция указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <returns><see langword="true"/>, если последовательность была найдена, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, _size);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции от указанного индекса и до конца указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns><see langword="true"/>, если последовательность была найдена, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(IEnumerable<T> collection, int index) => Contains(collection, index, _size - index);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns><see langword="true"/>, если последовательность была найдена, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(IEnumerable<T> collection, int index, int length) =>
		Contains(collection, index, length, EqualityComparer<T>.Default);

	/// <summary>
	/// Проверяет, включает ли данная коллекция указанную последовательность
	/// элементов, используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <returns><see langword="true"/>, если последовательность была найдена, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(IEnumerable<T> collection, IEqualityComparer<T> comparer) =>
		Contains(collection, 0, _size, comparer);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции от указанного индекса и до конца указанную последовательность
	/// элементов, используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <returns><see langword="true"/>, если последовательность была найдена, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(IEnumerable<T> collection, int index, IEqualityComparer<T> comparer) =>
		Contains(collection, index, _size - index, comparer);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции указанную последовательность
	/// элементов, используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <returns><see langword="true"/>, если последовательность была найдена, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(IEnumerable<T> collection, int index, int length, IEqualityComparer<T> comparer) =>
		IndexOf(collection, index, length, comparer) >= 0;

	/// <summary>
	/// Проверяет, включает ли данная коллекция указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <returns><see langword="true"/>, если элемент был найден, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(T? item) => Contains(item, 0, _size);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции от указанного индекса и до конца указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns><see langword="true"/>, если элемент был найден, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(T? item, int index) => Contains(item, index, _size - index);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns><see langword="true"/>, если элемент был найден, иначе <see langword="false"/>.</returns>
	public virtual bool Contains(T? item, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		if (item is null)
		{
			for (var i = index; i < index + length; i++)
				if (GetInternal(i) is null)
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

	/// <summary>
	/// Проверяет, включает ли данная коллекция любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <returns><see langword="true"/>, если какой-либо из элементов был найден, иначе <see langword="false"/>.</returns>
	public virtual bool ContainsAny(IEnumerable<T> collection) => ContainsAny(collection, 0, _size);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции от указанного индекса и до конца любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns><see langword="true"/>, если какой-либо из элементов был найден, иначе <see langword="false"/>.</returns>
	public virtual bool ContainsAny(IEnumerable<T> collection, int index) => ContainsAny(collection, index, _size - index);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns><see langword="true"/>, если какой-либо из элементов был найден, иначе <see langword="false"/>.</returns>
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

	/// <summary>
	/// Проверяет, включает ли данная коллекция любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <returns><see langword="true"/>, если какой-либо отличающийся элемент был найден,
	/// иначе <see langword="false"/>.</returns>
	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, _size);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции от указанного индекса и до конца любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns><see langword="true"/>, если какой-либо отличающийся элемент был найден,
	/// иначе <see langword="false"/>.</returns>
	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, int index) => ContainsAnyExcluding(collection, index, _size - index);

	/// <summary>
	/// Проверяет, включает ли диапазон данной коллекции любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns><see langword="true"/>, если какой-либо отличающийся элемент был найден,
	/// иначе <see langword="false"/>.</returns>
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

	/// <summary>
	/// Копирует элементы данной коллекции в массив.
	/// </summary>
	/// <param name="array">Целевой массив.</param>
	/// <param name="arrayIndex">Индекс в массиве, начиная с которого нужно вставлять данную коллекцию.</param>
	/// <exception cref="RankException">Массив должен иметь одно измерение.</exception>
	/// <exception cref="ArgumentException">Может появляться в разных случаях, подробнее см. текст исключения.</exception>
	public virtual void CopyTo(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException("Массив должен иметь одно измерение.");
		CopyToInternal(array, arrayIndex);
	}

	/// <summary>
	/// Копирует диапазон элементов данной коллекции в массив.
	/// </summary>
	/// <param name="index">Индекс в данной коллекции, начиная с которого нужно брать диапазон.</param>
	/// <param name="array">Целевой массив.</param>
	/// <param name="arrayIndex">Индекс в массиве, начиная с которого нужно вставлять часть данной коллекции.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <exception cref="ArgumentException">Может появляться в разных случаях, подробнее см. текст исключения.</exception>
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

	/// <summary>
	/// Копирует элементы данной коллекции в массив.
	/// </summary>
	/// <param name="array">Целевой массив.</param>
	/// <exception cref="ArgumentException">Может появляться в разных случаях, подробнее см. текст исключения.</exception>
	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	/// <summary>
	/// Копирует элементы данной коллекции в массив.
	/// </summary>
	/// <param name="array">Целевой массив.</param>
	/// <param name="arrayIndex">Индекс в массиве, начиная с которого нужно вставлять данную коллекцию.</param>
	/// <exception cref="ArgumentException">Может появляться в разных случаях, подробнее см. текст исключения.</exception>
	public virtual void CopyTo(T[] array, int arrayIndex) => CopyTo(0, array, arrayIndex, Length);

	protected virtual void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		CopyToInternal(0, array2, arrayIndex, _size);
	}

	protected abstract void CopyToInternal(int index, T[] array, int arrayIndex, int length);

	/// <summary>
	/// Уничтожает данную коллекцию, позволяя ОС освободить ресурсы.
	/// </summary>
	public abstract void Dispose();

	/// <summary>
	/// Проверяет, является ли указанный элемент последним в данной коллекции.
	/// </summary>
	/// <param name="item">Элемент для проверки.</param>
	/// <returns>Булева константа (<see langword="true"/> или <see langword="false"/>).</returns>
	public virtual bool EndsWith(T? item) => _size > 0 && (GetInternal(_size - 1)?.Equals(item) ?? item is null);

	/// <summary>
	/// Проверяет, заканчивается ли данная коллекция указанной последовательностью элементов.
	/// </summary>
	/// <param name="collection">Последовательность элементов, которой должна заканчиваться коллекция,
	/// для которой данный метод вызывается.</param>
	/// <returns>Булева константа (<see langword="true"/> или <see langword="false"/>).</returns>
	public virtual bool EndsWith(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		return _size >= CreateVar(collection.Length(), out var length) && EqualsInternal(collection, _size - length);
	}

	/// <summary>
	/// Проверяет, равна ли поэлементно данная коллекция указанной последовательности.
	/// </summary>
	/// <param name="collection">Последовательность, которой должна быть равна коллекция,
	/// для которой данный метод вызывается.</param>
	/// <returns>Булева константа (<see langword="true"/> или <see langword="false"/>).</returns>
	public virtual bool Equals(IEnumerable<T>? collection) => EqualsInternal(collection, 0, true);

	/// <summary>
	/// Проверяет, равен ли поэлементно диапазон данной коллекции указанной последовательности.
	/// </summary>
	/// <param name="collection">Последовательность, которой должен быть равен диапазон коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="toEnd">Специальный флаг, определяющий, что возвращать, если достигнут конец <paramref name="collection"/>,
	/// а в данной коллекции еще есть элементы. Если в такой ситуации этот флаг равен <see langword="true"/>,
	/// возвращает <see langword="false"/> и наоборот.</param>
	/// <returns>Булева константа (<see langword="true"/> или <see langword="false"/>).</returns>
	public virtual bool Equals(IEnumerable<T>? collection, int index, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	/// <inheritdoc/>
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
		if (collection is G.IList<T> list && !(CreateVar(list.GetType(), out var type)
			.Name.Contains("FastDelHashSet") || type.Name.Contains("ParallelHashSet")))
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
			if (!(GetInternal(index++)?.Equals(list[i]) ?? list[i] is null))
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
			if (index >= _size || !(GetInternal(index++)?.Equals(item) ?? item is null))
				return false;
		return !toEnd || index == _size;
	}

	/// <summary>
	/// Проверяет, есть ли в данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns><see langword="true"/>, если элемент был найден, иначе <see langword="false"/>.</returns>
	public virtual bool Exists(Predicate<T> match) => FindIndex(match) != -1;

	/// <summary>
	/// Ищет, есть ли в данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Первый элемент, удовлетворяющий критериям <paramref name="match"/>,
	/// или значение по умолчанию (<see langword="null"/>, ноль и т. д.), если такой элемент не найден.</returns>
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

	/// <summary>
	/// Ищет, есть ли в данной коллекции элементы, удовлетворяющие критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Список элементов, удовлетворяющих критериям <paramref name="match"/>.</returns>
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

	/// <summary>
	/// Ищет, есть ли в диапазоне данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="startIndex">Индекс начала.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Индекс первого элемента, удовлетворяющего критериям <paramref name="match"/>,
	/// или -1, если такой элемент не найден.</returns>
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

	/// <summary>
	/// Ищет, есть ли в диапазоне данной коллекции от указанного индекса и до конца элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="startIndex">Индекс начала.</param>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Индекс первого элемента, удовлетворяющего критериям <paramref name="match"/>,
	/// или -1, если такой элемент не найден.</returns>
	public virtual int FindIndex(int startIndex, Predicate<T> match) => FindIndex(startIndex, _size - startIndex, match);

	/// <summary>
	/// Ищет, есть ли в данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Индекс первого элемента, удовлетворяющего критериям <paramref name="match"/>,
	/// или -1, если такой элемент не найден.</returns>
	public virtual int FindIndex(Predicate<T> match) => FindIndex(0, _size, match);

	/// <summary>
	/// Ищет с конца, есть ли в данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Последний элемент, удовлетворяющий критериям <paramref name="match"/>,
	/// или значение по умолчанию (<see langword="null"/>, ноль и т. д.), если такой элемент не найден.</returns>
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

	/// <summary>
	/// Ищет с конца, есть ли в диапазоне данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="startIndex">Индекс начала.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Индекс последнего элемента, удовлетворяющего критериям <paramref name="match"/>,
	/// или -1, если такой элемент не найден.</returns>
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

	/// <summary>
	/// Ищет с конца, есть ли в диапазоне данной коллекции от указанного индекса и до конца элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="startIndex">Индекс начала.</param>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Индекс последнего элемента, удовлетворяющего критериям <paramref name="match"/>,
	/// или -1, если такой элемент не найден.</returns>
	public virtual int FindLastIndex(int startIndex, Predicate<T> match) => FindLastIndex(startIndex, startIndex + 1, match);

	/// <summary>
	/// Ищет с конца, есть ли в данной коллекции элемент, удовлетворяющий критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Индекс последнего элемента, удовлетворяющего критериям <paramref name="match"/>,
	/// или -1, если такой элемент не найден.</returns>
	public virtual int FindLastIndex(Predicate<T> match) => FindLastIndex(_size - 1, _size, match);

	/// <summary>
	/// Выполняет действие с каждым элементом данной коллекции.
	/// </summary>
	/// <param name="action">Действие (метод или лямбда-выражение).</param>
	public virtual void ForEach(Action<T> action) => ForEach(action, 0, _size);

	/// <summary>
	/// Выполняет действие с каждым элементом диапазона данной коллекции от указанного индекса и до конца.
	/// </summary>
	/// <param name="action">Действие (метод или лямбда-выражение).</param>
	/// <param name="index">Индекс начала диапазона.</param>
	public virtual void ForEach(Action<T> action, int index) => ForEach(action, index, _size - index);

	/// <summary>
	/// Выполняет действие с каждым элементом диапазона данной коллекции.
	/// </summary>
	/// <param name="action">Действие (метод или лямбда-выражение).</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
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

	/// <summary>
	/// Выполняет действие с каждым элементом данной коллекции.
	/// </summary>
	/// <param name="action">Действие (метод или лямбда-выражение), принимающее на вход не только элемент,
	/// но и его индекс (типа <see langword="int"/>).</param>
	public virtual void ForEach(Action<T, int> action) => ForEach(action, 0, _size);

	/// <summary>
	/// Выполняет действие с каждым элементом диапазона данной коллекции от указанного индекса и до конца.
	/// </summary>
	/// <param name="action">Действие (метод или лямбда-выражение), принимающее на вход не только элемент,
	/// но и его индекс (типа <see langword="int"/>).</param>
	/// <param name="index">Индекс начала диапазона.</param>
	public virtual void ForEach(Action<T, int> action, int index) => ForEach(action, index, _size - index);

	/// <summary>
	/// Выполняет действие с каждым элементом диапазона данной коллекции.
	/// </summary>
	/// <param name="action">Действие (метод или лямбда-выражение), принимающее на вход не только элемент,
	/// но и его индекс (типа <see langword="int"/>).</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
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

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public virtual IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

	private Enumerator GetEnumeratorInternal() => new(this);

	/// <inheritdoc/>
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

	protected abstract T GetInternal(int index);

	/// <summary>
	/// Получает срез данной коллекции (подробнее см. описание коллекции <see cref="Slice{T}"/>).
	/// </summary>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> GetSlice() => GetSlice(0, _size);

	/// <summary>
	/// Получает срез данной коллекции от указанного индекса и до конца
	/// (подробнее см. описание коллекции <see cref="Slice{T}"/>).
	/// </summary>
	/// <param name="index">Индекс начала диапазона в виде структуры <see cref="Index"/>.</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> GetSlice(Index index) => GetSlice(index.GetOffset(_size));

	/// <summary>
	/// Получает срез данной коллекции от указанного индекса и до конца
	/// (подробнее см. описание коллекции <see cref="Slice{T}"/>).
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> GetSlice(int index) => GetSlice(index, _size - index);

	/// <summary>
	/// Получает срез данной коллекции (подробнее см. описание коллекции <see cref="Slice{T}"/>).
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
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

	/// <summary>
	/// Получает срез данной коллекции (подробнее см. описание коллекции <see cref="Slice{T}"/>).
	/// </summary>
	/// <param name="range">Диапазон в виде структуры <see cref="Range"/>.</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> GetSlice(Range range)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		var (start, length) = range.GetOffsetAndLength(_size);
		return GetSlice(start, length);
	}

	protected abstract Slice<T> GetSliceInternal(int index, int length);

	/// <summary>
	/// Ищет в данной коллекции указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection) => IndexOf(collection, 0, _size);

	/// <summary>
	/// Ищет в диапазоне данной коллекции от указанного индекса и до конца указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, int index) => IndexOf(collection, index, _size - index);

	/// <summary>
	/// Ищет в диапазоне данной коллекции указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>)
	/// и возвращает в виде <see langword="out"/>-параметра длину этой указанной последовательности, чтобы не считать ее
	/// лишний раз (на случай, если последовательность не наследуется от интерфейсов, позволяющих легко получить длину).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <param name="collectionLength">Посчитанная длина <paramref name="collection"/>.</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength) =>
		IndexOf(collection, index, length, EqualityComparer<T>.Default, out collectionLength);

	/// <summary>
	/// Ищет в диапазоне данной коллекции указанную последовательность элементов
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, int index, int length) =>
		IndexOf(collection, index, length, EqualityComparer<T>.Default, out _);

	/// <summary>
	/// Ищет в данной коллекции указанную последовательность элементов,
	/// используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, IEqualityComparer<T> comparer) =>
		IndexOf(collection, 0, _size, comparer);

	/// <summary>
	/// Ищет в диапазоне данной коллекции от указанного индекса и до конца указанную последовательность элементов,
	/// используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, int index, IEqualityComparer<T> comparer) =>
		IndexOf(collection, index, _size - index, comparer);

	/// <summary>
	/// Ищет в диапазоне данной коллекции указанную последовательность элементов,
	/// используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, int index, int length, IEqualityComparer<T> comparer) =>
		IndexOf(collection, index, length, comparer, out _);

	/// <summary>
	/// Ищет в диапазоне данной коллекции указанную последовательность элементов,
	/// используя пользовательский компаратор равенства
	/// (с помощью алгоритма Бойера-Мура - подробнее <a href="https://ru.wikipedia.org/wiki/Алгоритм_Бойера_—_Мура">здесь</a>)
	/// и возвращает в виде <see langword="out"/>-параметра длину этой указанной последовательности, чтобы не считать ее
	/// лишний раз (на случай, если последовательность не наследуется от интерфейсов, позволяющих легко получить длину).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <param name="comparer">Компаратор на равенство, содержащий методы Equals() и GetHashCode(),
	/// (подробнее см. в описании интерфейса <see cref="IEqualityComparer{T}"/>).</param>
	/// <param name="collectionLength">Посчитанная длина <paramref name="collection"/>.</param>
	/// <returns>Индекс начала первого вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int IndexOf(IEnumerable<T> collection, int index, int length,
		IEqualityComparer<T> comparer, out int collectionLength)
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
		if (collection is G.IReadOnlyList<T> list) { }
		else if (collection is G.IList<T> list2)
			list = list2.GetSlice();
		else
			list = RedStarLinq.ToList(collection);
		var m = collectionLength = list.Count;
		var suffshift = RedStarLinq.FillArray(m, m + 1);
		var z = RedStarLinq.FillArray(0, m);
		for (int j = 1, maxZidx = 0, maxZ = 0; j < m; ++j)
		{
			if (j <= maxZ)
				z[j] = Min(maxZ - j + 1, z[j - maxZidx]);
			while (j + z[j] < m && comparer.Equals(list[m - 1 - z[j]], list[m - 1 - (j + z[j])]))
				z[j]++;
			if (j + z[j] - 1 > maxZ)
			{
				maxZidx = j;
				maxZ = j + z[j] - 1;
			}
		}
		for (var j = m - 1; j > 0; j--)
			suffshift[m - z[j]] = j;
		for (int j = 1, r = 0; j <= m - 1; j++)
		{
			if (j + z[j] != m)
				continue;
			for (; r <= j; r++)
				if (suffshift[r] == m)
					suffshift[r] = j;
		}
		for (int i = index, j = 0; i <= index + length - m && j >= 0; i += suffshift[j + 1])
		{
			for (j = m - 1; j >= 0 && comparer.Equals(list[j], GetInternal(i + j)); j--)
				;
			if (j < 0)
				return i;
		}
		return -1;
	}

	/// <summary>
	/// Ищет в данной коллекции указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <returns>Индекс первого вхождения элемента, или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOf(T item) => IndexOf(item, 0, _size);

	/// <summary>
	/// Ищет в диапазоне данной коллекции от указанного индекса и до конца указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс первого вхождения элемента, или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOf(T item, int index) => IndexOf(item, index, _size - index);

	/// <summary>
	/// Ищет в диапазоне данной коллекции указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс первого вхождения элемента, или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOf(T item, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		return IndexOfInternal(item, index, length);
	}

	/// <summary>
	/// Ищет в данной коллекции любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <returns>Индекс первого вхождения какого-либо из элементов, или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOfAny(IEnumerable<T> collection) => IndexOfAny(collection, 0, _size);

	/// <summary>
	/// Ищет в диапазоне данной коллекции от указанного индекса и до конца любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс первого вхождения какого-либо из элементов, или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOfAny(IEnumerable<T> collection, int index) => IndexOfAny(collection, index, _size - index);

	/// <summary>
	/// Ищет в диапазоне данной коллекции любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс первого вхождения какого-либо из элементов, или -1, если такой элемент не был найден.</returns>
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

	/// <summary>
	/// Ищет в данной коллекции любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <returns>Индекс первого вхождения какого-либо из отличающихся элементов,
	/// или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection) => IndexOfAnyExcluding(collection, 0, _size);

	/// <summary>
	/// Ищет в диапазоне данной коллекции от указанного индекса и до конца любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс первого вхождения какого-либо из отличающихся элементов,
	/// или -1, если такой элемент не был найден.</returns>
	public virtual int IndexOfAnyExcluding(IEnumerable<T> collection, int index) => IndexOfAnyExcluding(collection, index, _size - index);

	/// <summary>
	/// Ищет в диапазоне данной коллекции любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс первого вхождения какого-либо из отличающихся элементов,
	/// или -1, если такой элемент не был найден.</returns>
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

	/// <summary>
	/// Ищет с конца в данной коллекции указанную последовательность элементов
	/// (к сожалению, пока что "наивным" способом, алгоритм Бойера-Мура с конца пока не реализован).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <returns>Индекс начала последнего вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int LastIndexOf(IEnumerable<T> collection) => LastIndexOf(collection, _size - 1, _size);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции от указанного индекса и до конца указанную последовательность элементов
	/// (к сожалению, пока что "наивным" способом, алгоритм Бойера-Мура с конца пока не реализован).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс начала последнего вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int LastIndexOf(IEnumerable<T> collection, int index) => LastIndexOf(collection, index, index + 1);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции указанную последовательность элементов
	/// (к сожалению, пока что "наивным" способом, алгоритм Бойера-Мура с конца пока не реализован).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс начала последнего вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
	public virtual int LastIndexOf(IEnumerable<T> collection, int index, int length) => LastIndexOf(collection, index, length, out _);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции указанную последовательность элементов,
	/// (к сожалению, пока что "наивным" способом, алгоритм Бойера-Мура с конца пока не реализован)
	/// и возвращает в виде <see langword="out"/>-параметра длину этой указанной последовательности, чтобы не считать ее
	/// лишний раз (на случай, если последовательность не наследуется от интерфейсов, позволяющих легко получить длину).
	/// </summary>
	/// <param name="collection">Последовательность для поиска в коллекции, для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <param name="collectionLength">Посчитанная длина <paramref name="collection"/>.</param>
	/// <returns>Индекс начала последнего вхождения <paramref name="collection"/>,
	/// или -1, если последовательность не была найдена.</returns>
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

	/// <summary>
	/// Ищет с конца в данной коллекции указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <returns>Индекс последнего вхождения элемента, или -1, если такой элемент не был найден.</returns>
	public virtual int LastIndexOf(T item) => LastIndexOf(item, _size - 1, _size);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции от указанного индекса и до конца указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс последнего вхождения элемента, или -1, если такой элемент не был найден.</returns>
	public virtual int LastIndexOf(T item, int index) => LastIndexOf(item, index, index + 1);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для поиска в данной коллекции.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс последнего вхождения элемента, или -1, если такой элемент не был найден.</returns>
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

	/// <summary>
	/// Ищет с конца в данной коллекции любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <returns>Индекс последнего вхождения какого-либо из элементов, или -1, если такой элемент не был найден.</returns>
	public virtual int LastIndexOfAny(IEnumerable<T> collection) => LastIndexOfAny(collection, _size - 1, _size);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции от указанного индекса и до конца любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс последнего вхождения какого-либо из элементов, или -1, если такой элемент не был найден.</returns>

	public virtual int LastIndexOfAny(IEnumerable<T> collection, int index) => LastIndexOfAny(collection, index, index + 1);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции любой из указанных элементов.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого из них в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс последнего вхождения какого-либо из элементов, или -1, если такой элемент не был найден.</returns>
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

	/// <summary>
	/// Ищет с конца в данной коллекции любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <returns>Индекс последнего вхождения какого-либо из отличающихся элементов,
	/// или -1, если такой элемент не был найден.</returns>
	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection) => LastIndexOfAnyExcluding(collection, _size - 1, _size);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции от указанного индекса и до конца любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Индекс последнего вхождения какого-либо из отличающихся элементов,
	/// или -1, если такой элемент не был найден.</returns>
	public virtual int LastIndexOfAnyExcluding(IEnumerable<T> collection, int index) => LastIndexOfAnyExcluding(collection, index, index + 1);

	/// <summary>
	/// Ищет с конца в диапазоне данной коллекции любой из элементов, кроме указанных.
	/// </summary>
	/// <param name="collection">Коллекция элементов для поиска любого, кроме них, в коллекции,
	/// для которой данный метод вызывается.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона</param>
	/// <returns>Индекс последнего вхождения какого-либо из отличающихся элементов,
	/// или -1, если такой элемент не был найден.</returns>
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

	/// <summary>
	/// Получает случайный элемент коллекции, используя внутренний рандомизатор фреймворка.
	/// </summary>
	/// <returns>Случайный элемент коллекции.</returns>
	public virtual T Random() => Random(random);

	/// <summary>
	/// Получает случайный элемент коллекции, используя указанный рандомизатор.
	/// </summary>
	/// <param name="randomObj">Рандомизатор, используемый для получения номера элемента.</param>
	/// <returns>Случайный элемент коллекции.</returns>
	public virtual T Random(Random randomObj)
	{
		if (_size == 0)
			throw new InvalidOperationException("Невозможно получить случайный элемент пустой коллекции.");
		return GetInternal(randomObj.Next(_size));
	}

	/// <inheritdoc cref="Enumerable.Skip{T}(IEnumerable{T}, int)"/>
	/// <remarks>Возвращает коллекцию типа <see cref="Slice{T}"/>.</remarks>
	public virtual Slice<T> Skip(int length) => GetSlice(Clamp(length, 0, _size));

	/// <inheritdoc cref="Enumerable.SkipLast{T}(IEnumerable{T}, int)"/>
	/// <remarks>Возвращает коллекцию типа <see cref="Slice{T}"/>.</remarks>
	public virtual Slice<T> SkipLast(int length) => GetSlice(0, Max(0, _size - Max(length, 0)));

	/// <summary>
	/// Пропускает стоящие в начале элементы коллекции, удовлетворяющие критериям, после чего возвращает остаток коллекции.
	/// </summary>
	/// <param name="function">Критерии для проверки (функция или лямбда-выражение от элемента,
	/// возвращающие <see langword="bool"/>).</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> SkipWhile(Func<T, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i)); i++) ;
		return GetSlice(i);
	}

	/// <summary>
	/// Пропускает стоящие в начале элементы коллекции, удовлетворяющие критериям, после чего возвращает остаток коллекции.
	/// </summary>
	/// <param name="function">Критерии для проверки (функция или лямбда-выражение как от элемента, так и от его индекса,
	/// возвращающие <see langword="bool"/>).</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> SkipWhile(Func<T, int, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i), i); i++) ;
		return GetSlice(i);
	}

	/// <summary>
	/// Проверяет, является ли указанный элемент первым в данной коллекции.
	/// </summary>
	/// <param name="item">Элемент для проверки.</param>
	/// <returns>Булева константа (<see langword="true"/> или <see langword="false"/>).</returns>
	public virtual bool StartsWith(T? item) => _size > 0 && (GetInternal(0)?.Equals(item) ?? item is null);

	/// <summary>
	/// Проверяет, начинается ли данная коллекция указанной последовательностью элементов.
	/// </summary>
	/// <param name="collection">Последовательность элементов, которой должна начинаться коллекция,
	/// для которой данный метод вызывается.</param>
	/// <returns>Булева константа (<see langword="true"/> или <see langword="false"/>).</returns>
	public virtual bool StartsWith(IEnumerable<T> collection) => EqualsInternal(collection, 0, false);

	/// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
	/// <remarks>Возвращает коллекцию типа <see cref="Slice{T}"/>.</remarks>
	public virtual Slice<T> Take(int length) => GetSlice(0, Clamp(length, 0, _size));

	/// <inheritdoc cref="Enumerable.TakeLast{T}(IEnumerable{T}, int)"/>
	/// <remarks>Возвращает коллекцию типа <see cref="Slice{T}"/>.</remarks>
	public virtual Slice<T> TakeLast(int length) => GetSlice(Max(0, _size - Max(length, 0)));

	/// <summary>
	/// Возвращает стоящие в начале элементы коллекции, удовлетворяющие критериям, отбрасывая остаток коллекции.
	/// </summary>
	/// <param name="function">Критерии для проверки (функция или лямбда-выражение от элемента,
	/// возвращающие <see langword="bool"/>).</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> TakeWhile(Func<T, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i)); i++) ;
		return GetSlice(0, i);
	}

	/// <summary>
	/// Возвращает стоящие в начале элементы коллекции, удовлетворяющие критериям, отбрасывая остаток коллекции.
	/// </summary>
	/// <param name="function">Критерии для проверки (функция или лямбда-выражение как от элемента, так и от его индекса,
	/// возвращающие <see langword="bool"/>).</param>
	/// <returns>Коллекция <see cref="Slice{T}"/>.</returns>
	public virtual Slice<T> TakeWhile(Func<T, int, bool> function)
	{
		var i = 0;
		for (; i < _size && function(GetInternal(i), i); i++) ;
		return GetSlice(0, i);
	}

	/// <summary>
	/// Конвертирует коллекцию в массив.
	/// </summary>
	/// <returns>Массив, содержащий все элементы данной коллекции.</returns>
	public virtual T[] ToArray()
	{
		var array = GC.AllocateUninitializedArray<T>(Length);
		CopyToInternal(0, array, 0, Length);
		return array;
	}

	/// <summary>
	/// Проверяет, что все элементы данной коллекции удовлетворяют заданным критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns><see langword="true"/>, если все элементы удовлетворяют критериям <paramref name="match"/>,
	/// иначе <see langword="false"/>.</returns>
	public virtual bool TrueForAll(Predicate<T> match)
	{
		ArgumentNullException.ThrowIfNull(match);
		for (var i = 0; i < _size; i++)
			if (!match(GetInternal(i)))
				return false;
		return true;
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>
	{
		private readonly BaseIndexable<T> collection;
		private int index;

		internal Enumerator(BaseIndexable<T> collection)
		{
			this.collection = collection;
			index = 0;
			Current = default!;
		}

		public readonly void Dispose()
		{
		}

		public bool MoveNext()
		{
			var localCollection = collection;
			if ((uint)index < (uint)localCollection._size)
			{
				Current = localCollection.GetInternal(index++);
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = collection._size + 1;
			Current = default!;
			return false;
		}

		public T Current { get; private set; }

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
			Current = default!;
		}
	}
}

/// <inheritdoc/>
/// <typeparam name="TCertain">Ссылка-замыкание данной коллекции на себя.
/// Без этого параметра все методы, где нужно вернуть коллекцию того же типа,
/// что и коллекция, для которой метод вызывается (например, <see cref="GetRange(int, int, bool)"/>),
/// возвращали бы тип, в котором объявлены, вне зависимости от того, что он, например, абстрактный (как, например, данный тип),
/// и при дальнейших операциях, которые есть в конкретном типе данной коллекции,
/// но которых нет в базовом типе, пришлось бы добавлять явное приведение, что явно является неудобным и лишним действием.
/// А так, если от конкретного типа без этой ссылки-замыкания уже не наследовать другие типы,
/// явное приведение не нужно - меньше не определяющего полезную функциональность программы "boilerplate"-кода!</typeparam>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseIndexable<T, TCertain> : BaseIndexable<T>, IEquatable<TCertain> where TCertain : BaseIndexable<T, TCertain>, new()
{
	/// <summary>
	/// Краткая запись <see cref="GetRange(Range, bool)"/>.
	/// </summary>
	/// <param name="range">Диапазон в виде структуры <see cref="Range"/>.</param>
	/// <param name="alwaysCopy">Флаг, заставляющий метод принудительно возвращать копию диапазона
	/// (в некоторых производных коллекциях ничего не делает,
	/// в других, если он равен false, есть возможность вернуть только ссылку.</param>
	/// <returns>A shallow copy of a range of elements in the source collection.</returns>
	public virtual TCertain this[Range range, bool alwaysCopy = false] => GetRange(range, alwaysCopy);

	/// <summary>
	/// Сравнивает диапазоны данной коллекции и указанной коллекции от указанных индексов и до конца меньшего из диапазонов
	/// элемент за элементом, возвращая длину общего префикса.
	/// </summary>
	/// <param name="index">Индекс начала в данной коллекции.</param>
	/// <param name="other">Коллекция для сравнения диапазонов.</param>
	/// <param name="otherIndex">Индекс начала в <paramref name="other"/>.</param>
	/// <returns>Длина общего префикса указанных диапазонов (0, если первая же пара элементов различается,
	/// индекс первого отличающегося элемента (относительно начала диапазона, а НЕ какой-либо из коллекций),
	/// либо длина меньшего из диапазонов (от указанного для какой-либо из коллекций индекса до конца этой коллекции),
	/// если все элементы до его конца совпадают).</returns>
	public virtual int Compare(int index, TCertain other, int otherIndex) => Compare(index, other, otherIndex, Min(_size - index, other._size - otherIndex));

	/// <summary>
	/// Сравнивает диапазоны данной коллекции и указанной коллекции элемент за элементом, возвращая длину общего префикса.
	/// </summary>
	/// <param name="index">Индекс начала в данной коллекции.</param>
	/// <param name="other">Коллекция для сравнения диапазонов.</param>
	/// <param name="otherIndex">Индекс начала в <paramref name="other"/>.</param>
	/// <param name="length">Длина диапазонов.</param>
	/// <returns>Длина общего префикса указанных диапазонов (0, если первая же пара элементов различается,
	/// индекс первого отличающегося элемента (относительно начала диапазона, а НЕ какой-либо из коллекций),
	/// либо <paramref name="length"/>, если все элементы до конца диапазонов совпадают).</returns>
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

	/// <summary>
	/// Сравнивает данную коллекцию и указанную коллекцию
	/// элемент за элементом, возвращая длину общего префикса.
	/// </summary>
	/// <param name="other">Коллекция для сравнения.</param>
	/// <returns>Длина общего префикса указанных коллекций (0, если первая же пара элементов различается,
	/// индекс первого отличающегося элемента, либо длина меньшей из коллекций,
	/// если все элементы до его конца совпадают).</returns>
	public virtual int Compare(TCertain other) => Compare(0, other, 0, Min(_size, other._size));

	/// <summary>
	/// Сравнивает данную коллекцию и указанную коллекцию
	/// элемент за элементом, возвращая длину общего префикса, но не более <paramref name="length"/>.
	/// </summary>
	/// <param name="other">Коллекция для сравнения.</param>
	/// <param name="length">Максимальная длина сравнения.</param>
	/// <returns>Длина общего префикса указанных коллекций (0, если первая же пара элементов различается,
	/// индекс первого отличающегося элемента не далее <paramref name="length"/>, либо <paramref name="length"/>,
	/// если все элементы до этого значения совпадают).</returns>
	public virtual int Compare(TCertain other, int length) => Compare(0, other, 0, length);

	protected virtual int CompareInternal(int index, TCertain other, int otherIndex, int length)
	{
		for (var i = 0; i < length; i++)
			if (!(GetInternal(index + i)?.Equals(other.GetInternal(otherIndex + i)) ?? other.GetInternal(otherIndex + i) is null))
				return i;
		return length;
	}

	/// <inheritdoc cref="BaseIndexable{T}.Contains(IEnumerable{T})"/>
	public virtual bool Contains(TCertain collection) => Contains((IEnumerable<T>)collection, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.Contains(IEnumerable{T}, int)"/>
	public virtual bool Contains(TCertain collection, int index) => Contains((IEnumerable<T>)collection, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.Contains(IEnumerable{T}, int, int)"/>
	public virtual bool Contains(TCertain collection, int index, int length) => Contains((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.ContainsAny(IEnumerable{T})"/>
	public virtual bool ContainsAny(TCertain collection) => ContainsAny((IEnumerable<T>)collection, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.ContainsAny(IEnumerable{T}, int)"/>
	public virtual bool ContainsAny(TCertain collection, int index) => ContainsAny((IEnumerable<T>)collection, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.ContainsAny(IEnumerable{T}, int, int)"/>
	public virtual bool ContainsAny(TCertain collection, int index, int length) => ContainsAny((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.ContainsAnyExcluding(IEnumerable{T})"/>
	public virtual bool ContainsAnyExcluding(TCertain collection) => ContainsAnyExcluding((IEnumerable<T>)collection, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.ContainsAnyExcluding(IEnumerable{T}, int)"/>
	public virtual bool ContainsAnyExcluding(TCertain collection, int index) => ContainsAnyExcluding((IEnumerable<T>)collection, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.ContainsAnyExcluding(IEnumerable{T}, int, int)"/>
	public virtual bool ContainsAnyExcluding(TCertain collection, int index, int length) => ContainsAnyExcluding((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.EndsWith(IEnumerable{T})"/>
	public virtual bool EndsWith(TCertain collection) => EndsWith((IEnumerable<T>)collection);

	/// <inheritdoc cref="BaseIndexable{T}.Equals(IEnumerable{T})"/>
	public virtual bool Equals(TCertain? other) => EqualsInternal(other, 0, true);

	/// <inheritdoc cref="BaseIndexable{T}.Equals(IEnumerable{T}, int, bool)"/>
	public virtual bool Equals(TCertain? collection, int index, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	/// <inheritdoc cref="BaseIndexable{T}.ForEach(Action{T})"/>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual new TCertain ForEach(Action<T> action) => ForEach(action, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.ForEach(Action{T}, int)"/>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual new TCertain ForEach(Action<T> action, int index) => ForEach(action, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.ForEach(Action{T}, int, int)"/>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual new TCertain ForEach(Action<T> action, int index, int length)
	{
		base.ForEach(action, index, length);
		return (TCertain)this;
	}

	/// <inheritdoc cref="BaseIndexable{T}.ForEach(Action{T, int})"/>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual new TCertain ForEach(Action<T, int> action) => ForEach(action, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.ForEach(Action{T, int}, int)"/>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual new TCertain ForEach(Action<T, int> action, int index) => ForEach(action, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.ForEach(Action{T, int}, int, int)"/>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual new TCertain ForEach(Action<T, int> action, int index, int length)
	{
		base.ForEach(action, index, length);
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfter(IEnumerable<T> collection) => GetAfter(collection, 0, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfter(IEnumerable<T> collection, int index) => GetAfter(collection, index, _size - index);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfter(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length, out var collectionLength);
		return foundIndex == -1 ? GetRange(0, 0) : GetRange(foundIndex + collectionLength);
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfter(TCertain collection) => GetAfter((IEnumerable<T>)collection, 0, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfter(TCertain collection, int index) => GetAfter((IEnumerable<T>)collection, index, _size - index);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfter(TCertain collection, int index, int length) => GetAfter((IEnumerable<T>)collection, index, length);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfterLast(IEnumerable<T> collection) => GetAfterLast(collection, _size - 1, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index) => GetAfterLast(collection, index, index + 1);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfterLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length, out var collectionLength);
		return foundIndex == -1 ? (TCertain)this : GetRange(foundIndex + collectionLength);
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfterLast(TCertain collection) => GetAfterLast((IEnumerable<T>)collection, _size - 1, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfterLast(TCertain collection, int index) => GetAfterLast((IEnumerable<T>)collection, index, index + 1);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetAfterLast(TCertain collection, int index, int length) => GetAfterLast((IEnumerable<T>)collection, index, length);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBefore(IEnumerable<T> collection) => GetBefore(collection, 0, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBefore(IEnumerable<T> collection, int index) => GetBefore(collection, index, _size - index);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBefore(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = IndexOf(collection, index, length);
		return foundIndex == -1 ? (TCertain)this : GetRange(0, foundIndex);
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBefore(TCertain collection) => GetBefore((IEnumerable<T>)collection, 0, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBefore(TCertain collection, int index) => GetBefore((IEnumerable<T>)collection, index, _size - index);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBefore(TCertain collection, int index, int length) => GetBefore((IEnumerable<T>)collection, index, length);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeLast(IEnumerable<T> collection) => GetBeforeLast(collection, _size - 1, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index) => GetBeforeLast(collection, index, index + 1);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeLast(IEnumerable<T> collection, int index, int length)
	{
		var foundIndex = LastIndexOf(collection, index, length);
		return foundIndex == -1 ? GetRange(0, 0) : GetRange(0, foundIndex);
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeLast(TCertain collection) => GetBeforeLast((IEnumerable<T>)collection, _size - 1, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeLast(TCertain collection, int index) => GetBeforeLast((IEnumerable<T>)collection, index, index + 1);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeLast(TCertain collection, int index, int length) => GetBeforeLast((IEnumerable<T>)collection, index, length);

	/// <summary>
	/// Возвращает неглубокую копию диапазона данной коллекции от указанного индекса и до конца
	/// (см. также <paramref name="alwaysCopy"/>).
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="alwaysCopy">Флаг, заставляющий метод принудительно возвращать копию диапазона
	/// (в некоторых производных коллекциях ничего не делает,
	/// в других, если он равен false, есть возможность вернуть только ссылку.</param>
	/// <returns>A shallow copy of a range of elements in the source collection.</returns>
	public virtual TCertain GetRange(int index, bool alwaysCopy = false) => GetRange(index, _size - index, alwaysCopy);

	/// <summary>
	/// Возвращает неглубокую копию диапазона данной коллекции
	/// (см. также <paramref name="alwaysCopy"/>).
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <param name="alwaysCopy">Флаг, заставляющий метод принудительно возвращать копию диапазона
	/// (в некоторых производных коллекциях ничего не делает,
	/// в других, если он равен false, есть возможность вернуть только ссылку.</param>
	public virtual TCertain GetRange(int index, int length, bool alwaysCopy = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		else if (!alwaysCopy && index == 0 && length == _size)
			return (TCertain)this;
		return GetRangeInternal(index, length);
	}

	/// <summary>
	/// Аналогично <see cref="GetRange(int, int, bool)"/>, но с использованием структуры <see cref="Range"/>.
	/// </summary>
	/// <param name="range">Диапазон в виде структуры <see cref="Range"/>.</param>
	/// <param name="alwaysCopy">Флаг, заставляющий метод принудительно возвращать копию диапазона
	/// (в некоторых производных коллекциях ничего не делает,
	/// в других, если он равен false, есть возможность вернуть только ссылку.</param>
	/// <returns>A shallow copy of a range of elements in the source collection.</returns>
	public virtual TCertain GetRange(Range range, bool alwaysCopy = false)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		var (start, length) = range.GetOffsetAndLength(_size);
		return GetRange(start, length, alwaysCopy);
	}

	protected abstract TCertain GetRangeInternal(int index, int length);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T})"/>
	public virtual int IndexOf(TCertain collection) => IndexOf((IEnumerable<T>)collection, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, int)"/>
	public virtual int IndexOf(TCertain collection, int index) => IndexOf((IEnumerable<T>)collection, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, int, int)"/>
	public virtual int IndexOf(TCertain collection, int index, int length) => IndexOf((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, int, int, out int)"/>
	public virtual int IndexOf(TCertain collection, int index, int length, out int collectionLength) => IndexOf((IEnumerable<T>)collection, index, length, out collectionLength);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, IEqualityComparer{T})"/>
	public virtual int IndexOf(TCertain collection, IEqualityComparer<T> comparer) =>
		IndexOf((IEnumerable<T>)collection, 0, _size, comparer);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, int, IEqualityComparer{T})"/>
	public virtual int IndexOf(TCertain collection, int index, IEqualityComparer<T> comparer) =>
		IndexOf((IEnumerable<T>)collection, index, _size - index, comparer);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, int, int, IEqualityComparer{T})"/>
	public virtual int IndexOf(TCertain collection, int index, int length, IEqualityComparer<T> comparer) =>
		IndexOf((IEnumerable<T>)collection, index, length, comparer);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOf(IEnumerable{T}, int, int, IEqualityComparer{T}, out int)"/>
	public virtual int IndexOf(TCertain collection, int index, int length,
		IEqualityComparer<T> comparer, out int collectionLength) =>
		IndexOf((IEnumerable<T>)collection, index, length, comparer, out collectionLength);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOfAny(IEnumerable{T})"/>
	public virtual int IndexOfAny(TCertain collection) => IndexOfAny((IEnumerable<T>)collection, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOfAny(IEnumerable{T}, int)"/>
	public virtual int IndexOfAny(TCertain collection, int index) => IndexOfAny((IEnumerable<T>)collection, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOfAny(IEnumerable{T}, int, int)"/>
	public virtual int IndexOfAny(TCertain collection, int index, int length) => IndexOfAny((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOfAnyExcluding(IEnumerable{T})"/>
	public virtual int IndexOfAnyExcluding(TCertain collection) => IndexOfAnyExcluding((IEnumerable<T>)collection, 0, _size);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOfAnyExcluding(IEnumerable{T}, int)"/>
	public virtual int IndexOfAnyExcluding(TCertain collection, int index) => IndexOfAnyExcluding((IEnumerable<T>)collection, index, _size - index);

	/// <inheritdoc cref="BaseIndexable{T}.IndexOfAnyExcluding(IEnumerable{T}, int, int)"/>
	public virtual int IndexOfAnyExcluding(TCertain collection, int index, int length) => IndexOfAnyExcluding((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOf(IEnumerable{T})"/>
	public virtual int LastIndexOf(TCertain collection) => LastIndexOf((IEnumerable<T>)collection, _size - 1, _size);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOf(IEnumerable{T}, int)"/>
	public virtual int LastIndexOf(TCertain collection, int index) => LastIndexOf((IEnumerable<T>)collection, index, index + 1);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOf(IEnumerable{T}, int, int)"/>
	public virtual int LastIndexOf(TCertain collection, int index, int length) => LastIndexOf((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOf(IEnumerable{T}, int, int, out int)"/>
	public virtual int LastIndexOf(TCertain collection, int index, int length, out int collectionLength) => LastIndexOf((IEnumerable<T>)collection, index, length, out collectionLength);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOfAny(IEnumerable{T})"/>
	public virtual int LastIndexOfAny(TCertain collection) => LastIndexOfAny((IEnumerable<T>)collection, _size - 1, _size);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOfAny(IEnumerable{T}, int)"/>
	public virtual int LastIndexOfAny(TCertain collection, int index) => LastIndexOfAny((IEnumerable<T>)collection, index, index + 1);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOfAny(IEnumerable{T}, int, int)"/>
	public virtual int LastIndexOfAny(TCertain collection, int index, int length) => LastIndexOfAny((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOfAnyExcluding(IEnumerable{T})"/>
	public virtual int LastIndexOfAnyExcluding(TCertain collection) => LastIndexOfAnyExcluding((IEnumerable<T>)collection, _size - 1, _size);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOfAnyExcluding(IEnumerable{T}, int)"/>
	public virtual int LastIndexOfAnyExcluding(TCertain collection, int index) => LastIndexOfAnyExcluding((IEnumerable<T>)collection, index, index + 1);

	/// <inheritdoc cref="BaseIndexable{T}.LastIndexOfAnyExcluding(IEnumerable{T}, int, int)"/>
	public virtual int LastIndexOfAnyExcluding(TCertain collection, int index, int length) => LastIndexOfAnyExcluding((IEnumerable<T>)collection, index, length);

	/// <inheritdoc cref="BaseIndexable{T}.StartsWith(IEnumerable{T})"/>
	public virtual bool StartsWith(TCertain collection) => StartsWith((IEnumerable<T>)collection);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseMutableIndexable<T, TCertain> : BaseIndexable<T, TCertain> where TCertain : BaseMutableIndexable<T, TCertain>, new()
{
	public override T this[Index index, bool invoke = false]
	{
		get
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			var item = GetInternal(index2);
			if (invoke)
				Changed();
			return item;
		}
		set
		{
			var index2 = index.GetOffset(_size);
			if ((uint)index2 >= (uint)_size)
				throw new IndexOutOfRangeException();
			SetInternal(index2, value);
		Changed();
		}
	}

	public delegate void ListChangedHandler(TCertain newList);

	/// <summary>
	/// Событие, вызываемое при изменении данной коллекции.
	/// Если вам нужно уведомление, когда коллекция изменена, нет смысла проверять в цикле постоянно -
	/// добавьте уведомление в это событие.
	/// </summary>
	public event ListChangedHandler? ListChanged;

	protected virtual void Changed() => ListChanged?.Invoke((TCertain)this);

	/// <summary>
	/// Заменяет все элементы данной коллекции ссылками на указанный элемент.
	/// </summary>
	/// <param name="value">Элемент для замены элементов на него.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetAll(T value) => SetAll(value, 0, _size);

	/// <summary>
	/// Заменяет все элементы диапазона данной коллекции от указанного индекса и до конца ссылками на указанный элемент.
	/// </summary>
	/// <param name="value">Элемент для замены элементов на него.</param>
	/// <param name="index">Индекс начала диапазона в виде структуры <see cref="Index"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetAll(T value, Index index) => SetAll(value, index.GetOffset(_size));

	/// <summary>
	/// Заменяет все элементы диапазона данной коллекции от указанного индекса и до конца ссылками на указанный элемент.
	/// </summary>
	/// <param name="value">Элемент для замены элементов на него.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetAll(T value, int index) => SetAll(value, index, _size - index);

	/// <summary>
	/// Заменяет все элементы диапазона данной коллекции ссылками на указанный элемент.
	/// </summary>
	/// <param name="value">Элемент для замены элементов на него.</param>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetAll(T value, int index, int length)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Устанавливаемый диапазон выходит за текущий размер коллекции.");
		SetAllInternal(value, index, length);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Заменяет все элементы диапазона данной коллекции ссылками на указанный элемент.
	/// </summary>
	/// <param name="value">Элемент для замены элементов на него.</param>
	/// <param name="range">Заменяемый диапазон в виде структуры <see cref="Range"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetAll(T value, Range range) => SetAll(value, CreateVar(range.GetOffsetAndLength(_size), out var range2).Offset, range2.Length);

	protected virtual void SetAllInternal(T value, int index, int length)
	{
		var endIndex = index + length - 1;
		for (var i = index; i <= endIndex; i++)
			SetInternal(i, value);
	}

	protected abstract void SetInternal(int index, T value);
}
