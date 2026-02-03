namespace NStar.Core;

/// <summary>
/// Представляет базовый абстрактный класс для всех списков в фреймворке .NStar.
/// Технически сюда относятся не только списки, но и множества, у некоторых из которых (например, <see cref="ListHashSet{T}"/>)
/// наличие индексов действительно является значимым преимуществом перед
/// <see cref="HashSet{T}">хэш-множеством от Microsoft</see>, а у некоторых (например, ParallelHashSet&lt;T&gt;)
/// на индексацию лучше забить, так как она не имеет практического смысла.
/// <see cref="Slice{T}">Срез</see> не наследуется от данного абстрактного класса.
/// </summary>
/// <typeparam name="T">Тип элемента данной коллекции.</typeparam>
/// <typeparam name="TCertain">См. описание TCertain в <see cref="BaseIndexable{T, TCertain}"/>.</typeparam>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseList<T, TCertain> : BaseMutableIndexable<T, TCertain>, ICloneable, IList<T>, IList
	where TCertain : BaseList<T, TCertain>, new()
{
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

	/// <summary>
	/// Добавляет в конец данной коллекции указанный элемент.
	/// </summary>
	/// <param name="item">Элемент для добавления.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Add(T item)
	{
		if (_size == Capacity)
			EnsureCapacity(_size + 1);
		SetInternal(_size++, item);
		Changed();
		return (TCertain)this;
	}

	void G.ICollection<T>.Add(T item) => Add(item);

	int System.Collections.IList.Add(object? value)
	{
		try
		{
			if (value is not null)
				Add((T)value);
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException("Ошибка, такой элемент не подходит для этой коллекции.", nameof(value));
		}
		return _size - 1;
	}

	/// <summary>
	/// Добавляет в конец данной коллекции указанную последовательность.
	/// </summary>
	/// <param name="collection">Последовательность для добавления.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddRange(IEnumerable<T> collection) => Insert(_size, collection);

	/// <summary>
	/// Добавляет в конец данной коллекции указанную последовательность.
	/// </summary>
	/// <param name="span">Последовательность для добавления в виде структуры <see cref="Span{T}"/>
	/// или <see cref="ReadOnlySpan{T}"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddRange(ReadOnlySpan<T> span) => Insert(_size, span);

	/// <summary>
	/// Добавляет в конец данной коллекции указанную последовательность.
	/// </summary>
	/// <param name="array">Последовательность для добавления в виде массива.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddRange(T[] array) => AddRange(array.AsSpan());

	/// <summary>
	/// Добавляет в конец данной коллекции указанную последовательность.
	/// </summary>
	/// <param name="list">Последовательность для добавления в виде списка.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddRange(List<T> list) => Insert(_size, (IEnumerable<T>)list);

	/// <summary>
	/// Добавляет в конец данной коллекции последовательность шаблонных элементов
	/// (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).
	/// </summary>
	/// <param name="function">Шаблон (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).</param>
	/// <param name="length">Количество добавляемых элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddSeries(Func<int, T> function, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		for (var i = 0; i < length; i++)
			Add(function(i));
		return (TCertain)this;
	}

	/// <summary>
	/// Добавляет в конец данной коллекции последовательность шаблонных элементов
	/// (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).
	/// </summary>
	/// <param name="function">Шаблон (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).</param>
	/// <param name="length">Количество добавляемых элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddSeries(int length, Func<int, T> function) => AddSeries(function, length);

	/// <summary>
	/// Добавляет в конец данной коллекции последовательность одинаковых элементов
	/// (подробнее см. <see cref="RedStarLinq.Fill{T}(T, int)"/>).
	/// </summary>
	/// <param name="item">Элемент для добавления (см. <see cref="RedStarLinq.Fill{T}(T, int)"/>).</param>
	/// <param name="length">Количество добавляемых элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain AddSeries(T item, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		AddSeriesInternal(item, length);
		return (TCertain)this;
	}

	protected virtual void AddSeriesInternal(T item, int length)
	{
		EnsureCapacity(_size + length);
		SetAllInternal(item, _size, length);
		_size += length;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain Append(T item) => CollectionCreator(this).Add(item);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual (TCertain, TCertain) BreakFilter(Func<T, bool> match) => (BreakFilter(match, out var result2), result2);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual (TCertain, TCertain) BreakFilter(Func<T, int, bool> match) => (BreakFilter(match, out var result2), result2);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual (TCertain, TCertain) BreakFilterInPlace(Func<T, bool> match) => (BreakFilterInPlace(match, out var result2), result2);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual (TCertain, TCertain) BreakFilterInPlace(Func<T, int, bool> match) => (BreakFilterInPlace(match, out var result2), result2);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Опустошает данную коллекцию.
	/// Тем не менее, этот метод не освобождает занятую коллекцией память - для этого после данного метода
	/// вызовите также метод <see cref="TrimExcess">TrimExcess()</see>.
	/// </summary>
	public virtual void Clear() => Clear(false);

	/// <summary>
	/// Опустошает данную коллекцию (подробнее см. <see cref="Clear()"> описание основной перегрузки</see>).
	/// </summary>
	/// <param name="deep">Позволяет выполнить "глубокую" очистку - с принудительным заполнением занятой памяти нулями.
	/// Для множеств это обязательно, так как иначе останутся "маркеры" сортировки, хэшей или других конструкций,
	/// используемых множеством, отчего оно может сломаться, для остальных коллекций опционально.</param>
	public virtual void Clear(bool deep)
	{
		if (deep && _size > 0)
			ClearInternal();
		_size = 0;
		Changed();
	}

	public virtual void Clear(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Очищаемый диапазон выходит за текущий размер коллекции.");
		ClearInternal(index, length);
		Changed();
	}

	protected virtual void ClearInternal() => ClearInternal(0, _size);

	protected abstract void ClearInternal(int index, int length);

	public virtual object Clone() => Copy();

	/// <summary>
	/// Создает новую коллекцию, являющуюся <a href="https://ru.wikipedia.org/wiki/Конкатенация">конкатенацией</a>
	/// данной коллекции и указанной последовательности.
	/// </summary>
	/// <param name="collection">Последовательность для конкатенации с данной коллекцией.</param>
	/// <returns>Созданная коллекция.</returns>
	public virtual TCertain Concat(IEnumerable<T> collection) => CollectionCreator(this).AddRange(collection);

	bool System.Collections.IList.Contains(object? value)
	{
		if (IsCompatibleObject(value) && value is not null)
			return Contains((T)value);
		return false;
	}

	/// <summary>
	/// Создает копию данной коллекции того же типа и возвращает ее (не глубокое копирование - подробнее см.
	/// <a href="https://doka-guide.vercel.app/js/shallow-or-deep-clone/">здесь</a> - да, там про Javascript,
	/// но что поделать, если это самый популярный язык программирования в мире, а для нашего языка еще никто не написал
	/// подобное объяснение, при этом представленное вполне рабочее).
	/// </summary>
	/// <returns>Созданная копия.</returns>
	public virtual TCertain Copy() => CollectionCreator(this);

	/// <summary>
	/// Копирует диапазон данной коллекции, начиная с <paramref name="sourceIndex"/> и длиной <paramref name="length"/>,
	/// в указанную в параметре <paramref name="destination"/> коллекцию, начиная с <paramref name="destinationIndex"/>.
	/// Ничего не возвращает.
	/// </summary>
	/// <param name="sourceIndex">Индекс начала исходного диапазона.</param>
	/// <param name="destination">Целевая коллекция.</param>
	/// <param name="destinationIndex">Индекс начала диапазона в целевой коллекции.</param>
	/// <param name="length">Длина копируемого диапазона.</param>
	public virtual void CopyRangeTo(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(sourceIndex);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (sourceIndex + length > _size)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(destination);
		ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);
		if (destinationIndex > destination.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевой коллекции.");
		if (this == destination && sourceIndex == destinationIndex)
			return;
		CopyToInternal(sourceIndex, destination, destinationIndex, length);
		destination.Changed();
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

	/// <summary>
	/// Заполняет данную коллекцию шаблонными элементами
	/// (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).
	/// </summary>
	/// <param name="function">Шаблон (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).</param>
	/// <param name="length">Желаемое количество элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain FillInPlace(Func<int, T> function, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		EnsureCapacity(length);
		for (var i = 0; i < length; i++)
			SetInternal(i, function(i));
		if (_size > length)
			ClearInternal(length, _size - length);
		_size = length;
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Заполняет данную коллекцию шаблонными элементами
	/// (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).
	/// </summary>
	/// <param name="function">Шаблон (см. <see cref="RedStarLinq.Fill{T}(Func{int, T}, int)"/>).</param>
	/// <param name="length">Желаемое количество элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain FillInPlace(int length, Func<int, T> function) => FillInPlace(function, length);

	/// <summary>
	/// Заполняет данную коллекцию одинаковыми элементами
	/// (см. <see cref="RedStarLinq.Fill{T}(T, int)"/>).
	/// </summary>
	/// <param name="item">Элемент для добавления (см. <see cref="RedStarLinq.Fill{T}(T, int)"/>).</param>
	/// <param name="length">Желаемое количество элементов.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain FillInPlace(T item, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		EnsureCapacity(length);
		SetAllInternal(item, 0, length);
		if (_size > length)
			ClearInternal(length, _size - length);
		_size = length;
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual T GetAndRemove(Index index)
	{
		var value = this[index];
		RemoveAt(index);
		return value;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection) => GetBeforeSetAfter(collection, 0, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfter(IEnumerable<T> collection, int index) => GetBeforeSetAfter(collection, index, _size - index);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfter(TCertain list) => GetBeforeSetAfter((IEnumerable<T>)list, 0, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfter(TCertain list, int index) => GetBeforeSetAfter((IEnumerable<T>)list, index, _size - index);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfter(TCertain list, int index, int length) => GetBeforeSetAfter((IEnumerable<T>)list, index, length);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection) => GetBeforeSetAfterLast(collection, _size - 1, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfterLast(IEnumerable<T> collection, int index) => GetBeforeSetAfterLast(collection, index, index + 1);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfterLast(TCertain list) => GetBeforeSetAfterLast((IEnumerable<T>)list, _size - 1, _size);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfterLast(TCertain list, int index) => GetBeforeSetAfterLast((IEnumerable<T>)list, index, index + 1);

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain GetBeforeSetAfterLast(TCertain list, int index, int length) => GetBeforeSetAfterLast((IEnumerable<T>)list, index, length);

	protected override TCertain GetRangeInternal(int index, int length)
	{
		var list = CapacityCreator(length);
		CopyToInternal(index, list, 0, length);
		return list;
	}

	protected override Slice<T> GetSliceInternal(int index, int length) => new((G.IList<T>)this, index, length);

	int System.Collections.IList.IndexOf(object? value)
	{
		if (IsCompatibleObject(value) && value is not null)
			return IndexOf((T)value);
		return -1;
	}

	void G.IList<T>.Insert(int index, T item) => Insert(index, item);

	void System.Collections.IList.Insert(int index, object? value)
	{
		try
		{
			if (value is not null)
				Insert(index, (T)value);
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException("Ошибка, такой элемент не подходит для этой коллекции.", nameof(value));
		}
	}

	/// <summary>
	/// Вставляет внутрь данной коллекции указанный элемент, так что он оказывается под указанным индексом
	/// (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается элемент, в виде структуры <see cref="Index"/>.</param>
	/// <param name="item">Элемент для вставки.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(Index index, T item) => Insert(index.GetOffset(_size), item);

	/// <summary>
	/// Вставляет внутрь данной коллекции указанный элемент, так что он оказывается под указанным индексом
	/// (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается элемент.</param>
	/// <param name="item">Элемент для вставки.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
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
		Changed();
		return this2;
	}

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="collection"/>,
	/// в виде структуры <see cref="Index"/>.</param>
	/// <param name="collection">Последовательность для вставки.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(Index index, IEnumerable<T> collection) => Insert(index.GetOffset(_size), collection);

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="collection"/>.</param>
	/// <param name="collection">Последовательность для вставки.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentNullException.ThrowIfNull(collection);
		InsertInternal(index, collection);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="span"/>,
	/// в виде структуры <see cref="Index"/>.</param>
	/// <param name="span">Последовательность для вставки в виде структуры <see cref="Span{T}"/>
	/// или <see cref="ReadOnlySpan{T}"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(Index index, ReadOnlySpan<T> span) => Insert(index.GetOffset(_size), span);

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="span"/>.</param>
	/// <param name="span">Последовательность для вставки в виде структуры <see cref="Span{T}"/>
	/// или <see cref="ReadOnlySpan{T}"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		InsertInternal(index, span);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="array"/>,
	/// в виде структуры <see cref="Index"/>.</param>
	/// <param name="array">Последовательность для вставки в виде массива.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(Index index, T[] array) => Insert(index.GetOffset(_size), array.AsSpan());

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="array"/>.</param>
	/// <param name="array">Последовательность для вставки в виде массива.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(int index, T[] array) => Insert(index, array.AsSpan());

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="list"/>,
	/// в виде структуры <see cref="Index"/>.</param>
	/// <param name="list">Последовательность для вставки в виде списка.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(Index index, List<T> list) => Insert(index.GetOffset(_size), (IEnumerable<T>)list);

	/// <summary>
	/// Вставляет внутрь данной коллекции копию указанной последовательности,
	/// так что первый ее элемент оказывается под указанным индексом (все следующие элементы сдвигаются вправо).
	/// </summary>
	/// <param name="index">Индекс, под которым оказывается первый элемент <paramref name="list"/>.</param>
	/// <param name="list">Последовательность для вставки в виде списка.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Insert(int index, List<T> list) => Insert(index, (IEnumerable<T>)list);

	protected virtual void InsertInternal(int index, IEnumerable<T> collection)
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
	}

	protected virtual void InsertInternal(int index, ReadOnlySpan<T> span)
	{
		var this2 = (TCertain)this;
		var length = span.Length;
		if (_size == 0)
			for (var i = 0; i < length; i++)
				Add(span[i]);
		else if (length > 0)
		{
			EnsureCapacity(_size + length);
			if (index < _size)
				CopyToInternal(index, this2, index + length, _size - index);
			else
				_size += span.Length;
			for (var i = 0; i < length; i++)
				SetInternal(index + i, span[i]);
		}
	}

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя элементами "по умолчанию" (null, ноль и т. д.),
	/// поровну слева и справа (или на один больше справа, если дополняемая длина нечетная).
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <returns>Дополненная копия коллекции. Этот метод НЕ изменяет данную коллекцию -
	/// для этого используйте <see cref="PadInPlace(int)"/>.</returns>
	public virtual TCertain Pad(int length) => Pad(length, default!);

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя указанными элементами,
	/// поровну слева и справа (или на один больше справа, если дополняемая длина нечетная).
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <param name="value">Элемент для заполнения.</param>
	/// <returns>Дополненная копия коллекции. Этот метод НЕ изменяет данную коллекцию -
	/// для этого используйте <see cref="PadInPlace(int, T)"/>.</returns>
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

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя элементами "по умолчанию" (null, ноль и т. д.),
	/// поровну слева и справа (или на один больше справа, если дополняемая длина нечетная).
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain PadInPlace(int length) => PadInPlace(length, default!);

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя указанными элементами,
	/// поровну слева и справа (или на один больше справа, если дополняемая длина нечетная).
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <param name="value">Элемент для заполнения.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
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

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя элементами "по умолчанию" (null, ноль и т. д.),
	/// добавляя их слева.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <returns>Дополненная копия коллекции. Этот метод НЕ изменяет данную коллекцию -
	/// для этого используйте <see cref="PadInPlace(int)"/>.</returns>
	public virtual TCertain PadLeft(int length) => PadLeft(length, default!);

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя указанными элементами,
	/// добавляя их слева.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <param name="value">Элемент для заполнения.</param>
	/// <returns>Дополненная копия коллекции. Этот метод НЕ изменяет данную коллекцию -
	/// для этого используйте <see cref="PadInPlace(int, T)"/>.</returns>
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

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя элементами "по умолчанию" (null, ноль и т. д.),
	/// добавляя их слева.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain PadLeftInPlace(int length) => PadLeftInPlace(length, default!);

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя указанными элементами,
	/// добавляя их слева.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <param name="value">Элемент для заполнения.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
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

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя элементами "по умолчанию" (null, ноль и т. д.),
	/// добавляя их справа.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <returns>Дополненная копия коллекции. Этот метод НЕ изменяет данную коллекцию -
	/// для этого используйте <see cref="PadInPlace(int)"/>.</returns>
	public virtual TCertain PadRight(int length) => PadRight(length, default!);

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя указанными элементами,
	/// добавляя их справа.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <param name="value">Элемент для заполнения.</param>
	/// <returns>Дополненная копия коллекции. Этот метод НЕ изменяет данную коллекцию -
	/// для этого используйте <see cref="PadInPlace(int, T)"/>.</returns>
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

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя элементами "по умолчанию" (null, ноль и т. д.),
	/// добавляя их справа.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain PadRightInPlace(int length) => PadRightInPlace(length, default!);

	/// <summary>
	/// Дополняет длину данной коллекции до <paramref name="length"/>, заполняя указанными элементами,
	/// добавляя их справа.
	/// </summary>
	/// <param name="length">Желаемая длина коллекции.</param>
	/// <param name="value">Элемент для заполнения.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
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

	/// <summary>
	/// Удаляет диапазон элементов данной коллекции, сдвигая все следующие элементы влево.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Remove(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveInternal(index, length);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Удаляет диапазон элементов данной коллекции, сдвигая все следующие элементы влево.
	/// </summary>
	/// <param name="range">Удаляемый диапазон в виде структуры <see cref="Range"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Remove(Range range)
	{
		if (range.End.GetOffset(_size) > _size)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		var (start, length) = range.GetOffsetAndLength(_size);
		return Remove(start, length);
	}

	void System.Collections.IList.Remove(object? value)
	{
		if (IsCompatibleObject(value) && value is not null)
			RemoveValue((T)value);
	}

	/// <summary>
	/// Удаляет из данной коллекции все элементы, удовлетворяющие критериям.
	/// </summary>
	/// <param name="match">Предикат, задающий критерии (подробнее см. в описании <see cref="Predicate{T}"/>).</param>
	/// <returns>Количество удаленных элементов (НЕ данная коллекция - для этого используйте
	/// <see cref="FilterInPlace(Func{T, bool})"/> с противоположным условием).</returns>
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

	/// <summary>
	/// Удаляет элемент из данной коллекции, сдвигая все следующие элементы влево.
	/// </summary>
	/// <param name="index">Индекс удаляемого элемента в виде структуры <see cref="Index"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain RemoveAt(Index index) => RemoveAt(index.GetOffset(_size));

	/// <summary>
	/// Удаляет элемент из данной коллекции, сдвигая все следующие элементы влево.
	/// </summary>
	/// <param name="index">Индекс удаляемого элемента.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		if (index < _size)
			CopyToInternal(index + 1, this2, index, _size - 1 - index);
		_size--;
		Changed();
		SetInternal(_size, default!);
		Changed();
		return this2;
	}

	void System.Collections.IList.RemoveAt(int index) => RemoveAt(index);

	void G.IList<T>.RemoveAt(int index) => RemoveAt(index);

	/// <summary>
	/// Удаляет диапазон элементов данной коллекции от указанного индекса и до конца.
	/// </summary>
	/// <param name="index">Индекс начала диапазона в виде структуры <see cref="Index"/>.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain RemoveEnd(Index index) => RemoveEnd(index.GetOffset(_size));

	/// <summary>
	/// Удаляет диапазон элементов данной коллекции от указанного индекса и до конца.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain RemoveEnd(int index) => Remove(index, _size - index);

	protected virtual void RemoveInternal(int index, int length)
	{
		var this2 = (TCertain)this;
		if (length > 0)
		{
			_size -= length;
			if (index < _size)
				CopyToInternal(index + length, this2, index, _size - index);
			ClearInternal(_size, length);
		}
	}

	/// <summary>
	/// Удаляет элемент из данной коллекции, сдвигая все следующие элементы влево.
	/// Этот метод принимает на вход ЗНАЧЕНИЕ элемента, а не индекс!
	/// </summary>
	/// <param name="item">Удаляемый элемент.</param>
	/// <returns><see langword="true"/>, если элемент был найден и успешно удален, иначе <see langword="false"/>
	/// (НЕ данная коллекция).</returns>
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

	/// <summary>
	/// Создает коллекцию, состоящую из N повторений данной коллекции.
	/// </summary>
	/// <param name="length">Количество повторений.</param>
	/// <returns>Созданная коллекция.</returns>
	public virtual TCertain Repeat(int length)
	{
		var result = CapacityCreator(Length * length);
		for (var i = 0; i < length; i++)
			result.AddRange(this);
		return result;
	}

	/// <summary>
	/// Заменяет содержимое данной коллекции на копию указанной последовательности (не глубокое копирование - подробнее см.
	/// <a href="https://doka-guide.vercel.app/js/shallow-or-deep-clone/">здесь</a> - да, там про Javascript,
	/// но что поделать, если это самый популярный язык программирования в мире, а для нашего языка еще никто не написал
	/// подобное объяснение, при этом представленное вполне рабочее).
	/// </summary>
	/// <param name="collection">Последовательность для копирования.</param>
	/// <returns>Данная коллекция, ставшая копией <paramref name="collection"/>
	/// (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Replace(IEnumerable<T> collection)
	{
		if (ReferenceEquals(this, collection))
			return (TCertain)this;
		_size = 0;
		return AddRange(collection);
	}

	/// <summary>
	/// Заменяет содержимое данной коллекции на копию указанной последовательности (не глубокое копирование - подробнее см.
	/// <a href="https://doka-guide.vercel.app/js/shallow-or-deep-clone/">здесь</a> - да, там про Javascript,
	/// но что поделать, если это самый популярный язык программирования в мире, а для нашего языка еще никто не написал
	/// подобное объяснение, при этом представленное вполне рабочее).
	/// </summary>
	/// <param name="span">Последовательность для копирования в виде структуры <see cref="Span{T}"/>
	/// или <see cref="ReadOnlySpan{T}"/>.</param>
	/// <returns>Данная коллекция, ставшая копией <paramref name="collection"/>
	/// (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Replace(ReadOnlySpan<T> span)
	{
		_size = 0;
		return AddRange(span);
	}

	/// <summary>
	/// Заменяет содержимое данной коллекции на копию указанной последовательности (не глубокое копирование - подробнее см.
	/// <a href="https://doka-guide.vercel.app/js/shallow-or-deep-clone/">здесь</a> - да, там про Javascript,
	/// но что поделать, если это самый популярный язык программирования в мире, а для нашего языка еще никто не написал
	/// подобное объяснение, при этом представленное вполне рабочее).
	/// </summary>
	/// <param name="array">Последовательность для копирования в виде массива.</param>
	/// <returns>Данная коллекция, ставшая копией <paramref name="collection"/>
	/// (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Replace(T[] array) => Replace(array.AsSpan());

	/// <summary>
	/// Заменяет в данной коллекции все вхождения одного элемента на другой элемент.
	/// </summary>
	/// <param name="oldItem">Элемент для поиска.</param>
	/// <param name="newItem">Элемент для замены.</param>
	/// <returns>Коллекция с замененными элементами (НЕ та, для которой данный метод вызывается -
	/// для этого есть <see cref="ReplaceInPlace(T, T)"/>).</returns>
	public virtual TCertain Replace(T oldItem, T newItem)
	{
		var result = CollectionCreator(this);
		for (var index = IndexOf(oldItem); index >= 0; index = IndexOf(oldItem, index + 1))
			result.SetInternal(index, newItem);
		return result;
	}
#pragma warning disable CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain Replace(Dictionary<T, T>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		var result = CollectionCreator(this);
		for (var i = 0; i < _size; i++)
			if (dic.TryGetValue(result.GetInternal(i), out var newItem))
				result.SetInternal(i, newItem);
		return result;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain Replace(Dictionary<T, TCertain>? dic) => Replace(dic?.ToDictionary(x => x.Key, x => (IEnumerable<T>)x.Value)!);

	/// <summary>
	/// Заменяет в данной коллекции все вхождения одной последовательности на другую последовательность.
	/// </summary>
	/// <param name="oldCollection">Последовательность для поиска.</param>
	/// <param name="newCollection">Последовательность для замены.</param>
	/// <returns>Коллекция с замененными элементами (НЕ та, для которой данный метод вызывается).</returns>
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

	/// <inheritdoc cref="Replace(IEnumerable{T}, IEnumerable{T})"/>
	public virtual TCertain Replace(TCertain oldList, TCertain newList) => Replace((IEnumerable<T>)oldList, newList);

	/// <summary>
	/// Заменяет в данной коллекции все вхождения одного элемента на другой элемент.
	/// </summary>
	/// <param name="oldItem">Элемент для поиска.</param>
	/// <param name="newItem">Элемент для замены.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain ReplaceInPlace(T oldItem, T newItem)
	{
		for (var index = IndexOf(oldItem); index >= 0; index = IndexOf(oldItem, index + 1))
			SetInternal(index, newItem);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain ReplaceInPlace(Dictionary<T, IEnumerable<T>>? dic) => Replace(Replace(dic));

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain ReplaceInPlace(Dictionary<T, T>? dic)
	{
		ArgumentNullException.ThrowIfNull(dic);
		for (var i = 0; i < _size; i++)
			if (dic.TryGetValue(GetInternal(i), out var newItem))
				SetInternal(i, newItem);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public virtual TCertain ReplaceInPlace(Dictionary<T, TCertain>? dic) => Replace(Replace(dic));
#pragma warning restore CS8714 // Тип не может быть использован как параметр типа в универсальном типе или методе. Допустимость значения NULL для аргумента типа не соответствует ограничению "notnull".

	/// <summary>
	/// Заменяет в данной коллекции все вхождения одной последовательности на другую последовательность.
	/// </summary>
	/// <param name="oldCollection">Последовательность для поиска.</param>
	/// <param name="newCollection">Последовательность для замены.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>)
	/// (по факту сначала создает новую коллекцию, в которой элементы заменены, а затем заменяет содержимое данной коллекции
	/// на нее).</returns>
	public virtual TCertain ReplaceInPlace(IEnumerable<T> oldCollection, IEnumerable<T> newCollection) =>
		Replace(Replace(oldCollection, newCollection));

	/// <inheritdoc cref="ReplaceInPlace(IEnumerable{T}, IEnumerable{T})"/>
	public virtual TCertain ReplaceInPlace(TCertain oldList, TCertain newList) => ReplaceInPlace((IEnumerable<T>)oldList, newList);

	/// <summary>
	/// Заменяет диапазон данной коллекции на указанную последовательность, при необходимости сдвигая следующие элементы
	/// вправо или влево.
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="length">Длина диапазона.</param>
	/// <param name="collection">Последовательность для замены.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain ReplaceRange(int index, int length, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Заменяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		ReplaceRangeInternal(index, length, collection);
		Changed();
		return (TCertain)this;
	}

	/// <inheritdoc cref="ReplaceRange(int, int, IEnumerable{T})"/>
	public virtual TCertain ReplaceRange(int index, int length, TCertain list) => ReplaceRange(index, length, (IEnumerable<T>)list);

	protected virtual void ReplaceRangeInternal(int index, int length, IEnumerable<T> collection)
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
			_size -= Max(length - list._size, 0);
		}
	}

	/// <summary>
	/// Изменяет размер данной коллекции. Метод не заполняет добавляемые элементы конкретными значениями,
	/// поэтому коллекция будет содержать "мусор" (псевдослучайные данные, не несущие полезной нагрузки).
	/// </summary>
	/// <param name="newSize">Новый размер коллекции.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Resize(int newSize)
	{
		if (newSize == _size)
			return (TCertain)this;
		EnsureCapacity(newSize);
		if (newSize > _size)
		{
			_size = newSize;
			Changed();
			return (TCertain)this;
		}
		else
			return RemoveEnd(newSize);
	}

	/// <summary>
	/// Изменяет размер данной коллекции, добавляя элементы слева.
	/// Метод не заполняет добавляемые элементы конкретными значениями,
	/// поэтому коллекция будет содержать "мусор" (псевдослучайные данные, не несущие полезной нагрузки).
	/// </summary>
	/// <param name="newSize">Новый размер коллекции.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain ResizeLeft(int newSize)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(newSize);
		if (newSize < _size)
			return Remove(0, _size - newSize);
		var oldSize = _size;
		EnsureCapacity(newSize);
		_size = newSize;
		CopyRangeTo(0, (TCertain)this, newSize - oldSize, oldSize);
		return (TCertain)this;
	}

	/// <summary>
	/// Переставляет все элементы данной коллекции в обратном порядке.
	/// </summary>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Reverse() => Reverse(0, _size);

	/// <summary>
	/// Переставляет диапазон данной коллекции в обратном порядке.
	/// </summary>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain Reverse(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		ReverseInternal(index, length);
		Changed();
		return (TCertain)this;
	}

	protected abstract void ReverseInternal(int index, int length);

	/// <summary>
	/// Устанавливает элемент по указанному индексу в указанное значение, если индекс меньше длины коллекции,
	/// или добавляет это значение в конец, если индекс равен длине коллекции.
	/// Выбрасывает исключение <see cref="ArgumentOutOfRangeException"/>, если индекс больше длины коллекции.
	/// </summary>
	/// <param name="index">Индекс для установки или добавления элемента.</param>
	/// <param name="value">Элемент для установки или добавления.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetOrAdd(int index, T value)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (index == _size)
			return Add(value);
		SetInternal(index, value);
		Changed();
		return (TCertain)this;
	}

	protected virtual void SetOrAddInternal(int index, T value)
	{
		if (index == _size)
			Add(value);
		SetInternal(index, value);
	}

	/// <summary>
	/// Заменяет диапазон данной коллекции, начиная с указанного индекса, на указанную последовательность,
	/// продолжая, пока в указанной последовательности есть элементы (этот метод НЕ сдвигает элементы вправо или влево).
	/// Этот метод может изменить длину данной коллекции только в сторону увеличения
	/// (или не изменить совсем, если в указанной последовательности меньше элементов).
	/// </summary>
	/// <param name="index">Индекс начала диапазона.</param>
	/// <param name="collection">Последовательность для замены.</param>
	/// <returns>Данная коллекция (подробнее см. в описании TCertain в <see cref="BaseIndexable{T, TCertain}"/>).</returns>
	public virtual TCertain SetRange(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is not TCertain list)
			list = CollectionCreator(collection);
		var length = list._size;
		SetRangeInternal(index, length, list);
		Changed();
		return (TCertain)this;
	}

	protected virtual void SetRangeInternal(int index, int length, TCertain list)
	{
		EnsureCapacity(index + length);
		var this2 = (TCertain)this;
		if (length > 0)
			list.CopyToInternal(0, this2, index, length);
	}

	/// <summary>
	/// Перемешивает элементы данной последовательности в (псевдо)случайном порядке.
	/// </summary>
	/// <returns></returns>
	public virtual TCertain Shuffle()
	{
		for (var i = _size; i > 0; i--)
		{
			var swapIndex = random.Next(i);
			var temp = GetInternal(swapIndex);
			SetInternal(swapIndex, GetInternal(i - 1));
			SetInternal(i - 1, temp);
		}
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
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

	/// <summary>
	/// Уменьшает количество памяти, занимаемое данной коллекцией, до фактической длины
	/// (по факту потребление памяти сначала немного увеличивается, так как выделяется новая область для элементов
	/// (массив, указатель и т. д.), а только когда менеджер памяти в ОС "доберется" до старой области и освободит ее,
	/// потребление памяти уменьшится).
	/// </summary>
	/// <returns></returns>
	public virtual TCertain TrimExcess()
	{
		var threshold = (int)(Capacity * 0.9);
		if (_size < threshold)
			Capacity = _size;
		return (TCertain)this;
	}

	/// <summary>
	/// Создает список из одного элемента.
	/// </summary>
	/// <param name="x">Единственный элемент для добавления в список.</param>
	public static implicit operator BaseList<T, TCertain>(T x) => new TCertain().Add(x);
}
