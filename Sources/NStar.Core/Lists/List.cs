namespace NStar.Core;

/// <summary>
/// Представляет общий базовый класс для <see cref="List{T}"/> и <see cref="String"/>
/// - списков последовательных элементов переменного размера, упорядоченных по индексу.
/// Список бит (<see cref="BitList"/>) также очень похож на них, но технически сюда не относится из-за низкоуровневых причин,
/// плюс он не поддерживает сортировку из-за бессмысленности сортировки битов
/// (можно просто посчитать количество <see langword="true"/> и разместить столько-то их и столько-то <see langword="false"/>).
/// </summary>
/// <typeparam name="T">Тип элемента данной коллекции.</typeparam>
/// <typeparam name="TCertain">См. описание TCertain в <see cref="BaseIndexable{T, TCertain}"/>.</typeparam>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract partial class List<T, TCertain> : BaseList<T, TCertain> where TCertain : List<T, TCertain>, new()
{
	private protected T[]? _items;

	private protected static readonly Dictionary<int, G.List<T[]>> arrayPool = [];
	private protected static readonly object globalLockObj = new();

	protected List() => _items = null;

	protected List(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity == 0)
		{
			_items = null;
			return;
		}
		_items = ArrayPool<T>.Shared.Rent(capacity);
	}

	protected List(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		int length;
		if (collection is G.ICollection<T> c)
		{
			length = c.Count;
			if (length == 0)
			{
				_items = null;
				return;
			}
			_items = ArrayPool<T>.Shared.Rent(length);
			if (c is T[] array && length >= 2048)
				Parallel.For(0, length, i => _items[i] = array[i]);
			else
				c.CopyTo(_items, 0);
			_size = length;
			return;
		}
		_size = 0;
		if (collection.TryGetLengthEasily(out length))
			_items = ArrayPool<T>.Shared.Rent(length);
		else
			_items = null;
		using var en = collection.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	protected List(int capacity, IEnumerable<T> collection)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.ICollection<T> c)
		{
			var length = c.Count;
			if (length == 0)
				return;
			var arrayLength = Max(capacity, length);
			_items = ArrayPool<T>.Shared.Rent(arrayLength);
			Debug.Assert(_items is not null);
			if (c is T[] array && length >= 2048)
				Parallel.For(0, length, i => _items[i] = array[i]);
			else
				c.CopyTo(_items, 0);
			_size = length;
		}
		else
		{
			_items = ArrayPool<T>.Shared.Rent(capacity == 0 ? DefaultCapacity : capacity);
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
			return;
		}
	}

	protected List(params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		_items = ArrayPool<T>.Shared.Rent(array.Length);
		Parallel.For(0, array.Length, i => _items[i] = array[i]);
	}

	protected List(int capacity, params T[] array)
	{
		ArgumentNullException.ThrowIfNull(array);
		_size = array.Length;
		var arrayLength = Max(capacity, array.Length);
		_items = ArrayPool<T>.Shared.Rent(arrayLength);
		Parallel.For(0, array.Length, i => _items[i] = array[i]);
	}

	protected List(ReadOnlySpan<T> span)
	{
		_size = span.Length;
		_items = ArrayPool<T>.Shared.Rent(span.Length);
		span.CopyTo(_items);
	}

	protected List(int capacity, ReadOnlySpan<T> span)
	{
		_size = span.Length;
		if (span.Length > capacity)
		{
			_items = ArrayPool<T>.Shared.Rent(span.Length);
			span.CopyTo(_items);
		}
		else
		{
			_items = ArrayPool<T>.Shared.Rent(capacity);
			span.CopyTo(_items);
		}
	}

	~List() => Dispose();

	/// <summary>
	/// Представляет максимальное количество элементов, под которое в данный момент выделена память.
	/// Это НЕ длина списка и НЕ максимальное количество элементов, которое вообще в принципе можно добавить
	/// (которое постоянно и равно количеству элементов, умещающемуся в 2 ГБ минус 1 байт)
	/// - емкость автоматически увеличивается при попытке добавить/вставить элемент, когда она равна длине,
	/// но это происходит редко, также можно в любой момент вручную изменить ее на любое число не меньше длины.
	/// </summary>
	public override int Capacity
	{
		get => _items?.Length ?? 0;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			if (value == (_items?.Length ?? 0))
				return;
			if (value > 0)
			{
				T[] newItems;
				newItems = ArrayPool<T>.Shared.Rent(value);
				if (_size > 0 && _items is not null)
					Array.Copy(_items, 0, newItems, 0, _size);
				_items = newItems;
			}
			else
				_items = null;
			Changed();
		}
	}

	public override Memory<T> AsMemory(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		return MemoryExtensions.AsMemory(_items, index, length);
	}

	public override Span<T> AsSpan(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		return MemoryExtensions.AsSpan(_items, index, length);
	}

	public virtual int BinarySearch(int index, int length, T item) => BinarySearch(index, length, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(int index, int length, T item, IComparer<T> comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Диапазон выходит за текущий размер коллекции.");
		if (_items is null)
			return -1;
		return Array.BinarySearch(_items, index, length, item, comparer);
	}

	public virtual int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	protected override void ClearInternal(int index, int length)
	{
		if (_items is null)
			return;
		Array.Clear(_items, index, length);
	}

	protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (_items is null || destination._items is null)
			return;
		Array.Copy(_items, sourceIndex, destination._items, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
	}

	protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (_items is null)
			return;
		Array.Copy(_items, 0, array, arrayIndex, _size);
	}

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
	{
		if (_items is null)
			return;
		Parallel.For(0, length, i => array[arrayIndex + i] = _items[index + i]);
	}

	public override void Dispose()
	{
		if (_items is null)
			return;
		ArrayPool<T>.Shared.Return(_items);
		_items = null;
		_size = 0;
		Changed();
		GC.SuppressFinalize(this);
	}

	internal static List<T> EmptyList(int length) => new(length) { _size = length };

	public static unsafe TCertain FromPointer(int capacity, void* ptr)
	{
		if (ptr is null)
			throw new ArgumentNullException(nameof(ptr));
		TCertain list = [];
		list._size = capacity;
		list._items = ArrayPool<T>.Shared.Rent(capacity);
		new Span<T>(ptr, list._size).CopyTo(list._items);
		return list;
	}

	protected override T GetInternal(int index)
	{
		Debug.Assert(_items is not null);
		return _items[index];
	}

	protected override int IndexOfInternal(T item, int index, int length)
	{
		if (_items is null)
			return -1;
		return Array.IndexOf(_items, item, index, length);
	}

	public override TCertain Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (_size == Capacity)
		{
			var min = _size + 1;
			var newCapacity = Max(DefaultCapacity, Capacity * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < min)
				newCapacity = min;
			T[] newItems;
			newItems = ArrayPool<T>.Shared.Rent(newCapacity);
			if (index > 0 && _items is not null)
				Array.Copy(_items, 0, newItems, 0, index);
			if (index < _size && _items is not null)
				Array.Copy(_items, index, newItems, index + 1, _size - index);
			_size++;
			newItems[index] = item;
			_items = newItems;
		}
		else
		{
			if (index < _size)
				CopyToInternal(index, (TCertain)this, index + 1, _size - index);
			else
				_size++;
			Debug.Assert(_items is not null);
			_items[index] = item;
		}
		Changed();
		return (TCertain)this;
	}

	protected override void InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is List<T, TCertain> list)
		{
			var length = list._size;
			if (length == 0)
				return;
			if (Capacity < _size + length)
			{
				var newItems = InsertInternalGetNewItems(index, length);
				if (list._items is not null)
					Parallel.For(0, length, i => newItems[index + i] = list._items[i]);
				_items = newItems;
			}
			else
			{
				if (index < _size && _items is not null)
					Array.Copy(_items, index, _items, index + length, _size - index);
				if (this == list && _items is not null)
				{
					Array.Copy(_items, 0, _items, index, index);
					Array.Copy(_items, index + length, _items, index * 2, _size - index);
				}
				else if (list._items is not null)
				{
					Debug.Assert(_items is not null);
					Parallel.For(0, length, i => _items[index + i] = list._items[i]);
				}
			}
			_size += length;
		}
		else if (collection is T[] array)
		{
			var length = array.Length;
			if (length == 0)
				return;
			if (Capacity < _size + length)
			{
				var newItems = InsertInternalGetNewItems(index, length);
				Parallel.For(0, length, i => newItems[index + i] = array[i]);
				_items = newItems;
			}
			else
			{
				if (index < _size && _items is not null)
					Array.Copy(_items, index, _items, index + length, _size - index);
				Debug.Assert(_items is not null);
				Parallel.For(0, length, i => _items[index + i] = array[i]);
			}
			_size += length;
		}
		else if (collection is G.ICollection<T> list2)
		{
			var length = list2.Count;
			if (length == 0)
				return;
			if (Capacity < _size + length)
			{
				var newItems = InsertInternalGetNewItems(index, length);
				list2.CopyTo(newItems, index);
				_items = newItems;
			}
			else
			{
				if (index < _size && _items is not null)
					Array.Copy(_items, index, _items, index + length, _size - index);
				Debug.Assert(_items is not null);
				list2.CopyTo(_items, index);
			}
			_size += length;
		}
		else
			InsertInternal(index, CollectionCreator(collection));
	}

	protected override void InsertInternal(int index, ReadOnlySpan<T> span)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = span.Length;
		if (length == 0)
			return;
		if (Capacity < _size + length)
		{
			var newItems = InsertInternalGetNewItems(index, length);
			span.CopyTo(MemoryExtensions.AsSpan(newItems, index));
			_items = newItems;
		}
		else
		{
			if (index < _size && _items is not null)
				Array.Copy(_items, index, _items, index + length, _size - index);
			span.CopyTo(MemoryExtensions.AsSpan(_items, index));
		}
		_size += length;
	}
	private T[] InsertInternalGetNewItems(int index, int length)
	{
		var min = _size + length;
		var newCapacity = Max(DefaultCapacity, Capacity * 2);
		if ((uint)newCapacity > int.MaxValue)
			newCapacity = int.MaxValue;
		if (newCapacity < min)
			newCapacity = min;
		T[] newItems;
		newItems = ArrayPool<T>.Shared.Rent(newCapacity);
		if (index > 0 && _items is not null)
			Parallel.For(0, index, i => newItems[i] = _items[i]);
		if (index < _size && _items is not null)
			Parallel.For(index, _size, i => newItems[i + length] = _items[i]);
		return newItems;
	}

	protected override int LastIndexOfInternal(T item, int index, int length)
	{
		if (_items is null)
			return -1;
		return Array.LastIndexOf(_items, item, index, length);
	}

	public virtual TCertain NSort() => NSort(0, _size);

	public virtual TCertain NSort(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return (TCertain)this;
		if (this is List<byte> byteList)
		{
			Debug.Assert(byteList._items is not null);
			byteList._items.NSort(index, length);
		}
		else if (this is List<ushort> ushortList)
		{
			Debug.Assert(ushortList._items is not null);
			ushortList._items.NSort(index, length);
		}
		else if (this is List<uint> uintList)
		{
			Debug.Assert(uintList._items is not null);
			uintList._items.NSort(index, length);
		}
		else
			return Sort(index, length, G.Comparer<T>.Default);
		Changed();
		return (TCertain)this;
	}

	public virtual TCertain NSort(Func<T, uint> function) => NSort(function, 0, _size);

	public virtual TCertain NSort(Func<T, uint> function, int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (_items is null)
			return (TCertain)this;
		_items.NSort(function, index, length);
		Changed();
		return (TCertain)this;
	}

	public static List<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) => collection is List<TList> list ? list : [.. collection];

	protected override void ReverseInternal(int index, int length)
	{
		if (_items is null)
			return;
		Array.Reverse(_items, index, length);
	}

	protected override void SetInternal(int index, T value)
	{
		if (_items is null)
			return;
		_items[index] = value;
	}

	public virtual TCertain Sort() => Sort(0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort(IComparer<T> comparer) => Sort(0, _size, comparer);

	public virtual TCertain Sort(int index, int length, IComparer<T> comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (_items is null)
			return (TCertain)this;
		Array.Sort(_items, index, length, comparer);
		Changed();
		return (TCertain)this;
	}

	public virtual TCertain Sort<TValue>(Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(0, _size, function, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(index, length, function, G.Comparer<TValue>.Default, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int length, Func<T, TValue> function, IComparer<TValue> comparer, bool fasterButMoreMemory = true)
	{
		if (fasterButMoreMemory)
		{
			RedStarLinq.ToList(this, function).Sort(this, index, length, comparer);
			Changed();
			return (TCertain)this;
		}
		else
			return Sort(index, length, new Comparer<T>((x, y) => comparer.Compare(function(x), function(y))));
	}

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, comparer);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, int index, int length, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new()
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Сортируемый диапазон выходит за текущий размер коллекции.");
		if (index + length > values._size)
			throw new ArgumentException("Сортируемый диапазон выходит за размер экстра-коллекции.");
		if (_items is null)
			return (TCertain)this;
		Array.Sort(_items, values._items, index, length, comparer);
		Changed();
		return (TCertain)this;
	}

	/// <summary>
	/// Описание этого метода в разработке.
	/// </summary>
	public static List<List<T>> Transpose(List<Slice<T>> list, bool widen = false)
	{
		if (list._size == 0)
			throw new ArgumentException("Невозможно транспонировать коллекцию нулевой длины.", nameof(list));
		var yCount = widen ? list.Max(x => x.Length) : list.Min(x => x.Length);
		List<List<T>> newList = [];
		for (var i = 0; i < yCount; i++)
		{
			newList.Add(new List<T>(list._size));
			for (var j = 0; j < list._size; j++)
			{
				var temp = list.GetInternal(j);
				newList.GetInternal(i).Add(temp.Length <= i ? default! : temp[i]);
			}
		}
		return newList;
	}
}

/// <summary>
/// Представляет список последовательных элементов переменного размера, упорядоченных по индексу.
/// Доступны методы добавления, удаления, вставки, поиска, сортировки, реверса и другие методы для манипулирования списком.
/// Любой элемент, в том числе одна и та же ссылка, может встречаться несколько раз (в отличие от множеств).
/// Содержит такое свойство, как емкость (<see cref="List{T, TCertain}.Capacity">Capacity</see>),
/// представляющее собой максимальное количество элементов, под которое в данный момент выделена память
/// (подробнее см. по ссылке выше).
/// </summary>
/// <typeparam name="T">Тип элемента данной коллекции.</typeparam>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class List<T> : List<T, List<T>>
{
	public List() { }

	public List(int capacity) : base(capacity) { }

	public List(IEnumerable<T> collection) : base(collection) { }

	public List(int capacity, IEnumerable<T> collection) : base(capacity, collection) { }

	public List(int capacity, params T[] array) : base(capacity, array) { }

	public List(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public List(params T[] array) : base(array) { }

	public List(ReadOnlySpan<T> span) : base(span) { }

	protected override Func<int, List<T>> CapacityCreator { get; } = x => new(x);

	protected override Func<IEnumerable<T>, List<T>> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<T>, List<T>> SpanCreator { get; } = x => new(x);

	public static implicit operator List<T>(T x) => new(x);

	public static implicit operator List<T>(T[] x) => new(x);

	public static implicit operator List<T>((T, T) x) => [x.Item1, x.Item2];

	public static implicit operator List<T>((T, T, T) x) => [x.Item1, x.Item2, x.Item3];

	public static implicit operator List<T>((T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4];

	public static implicit operator List<T>((T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5];

	public static implicit operator List<T>((T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6];

	public static implicit operator List<T>((T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15];

	public static implicit operator List<T>((T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) x) => [x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8, x.Item9, x.Item10, x.Item11, x.Item12, x.Item13, x.Item14, x.Item15, x.Item16];

	public static explicit operator (T, T)(List<T> x) => x._size == 2 ? (x.GetInternal(0), x.GetInternal(1)) : throw new InvalidOperationException("Список должен иметь 2 элемента.");

	public static explicit operator (T, T, T)(List<T> x) => x._size == 3 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2)) : throw new InvalidOperationException("Список должен иметь 3 элемента.");

	public static explicit operator (T, T, T, T)(List<T> x) => x._size == 4 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3)) : throw new InvalidOperationException("Список должен иметь 4 элемента.");

	public static explicit operator (T, T, T, T, T)(List<T> x) => x._size == 5 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4)) : throw new InvalidOperationException("Список должен иметь 5 элементов.");

	public static explicit operator (T, T, T, T, T, T)(List<T> x) => x._size == 6 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5)) : throw new InvalidOperationException("Список должен иметь 6 элементов.");

	public static explicit operator (T, T, T, T, T, T, T)(List<T> x) => x._size == 7 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6)) : throw new InvalidOperationException("Список должен иметь 7 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T)(List<T> x) => x._size == 8 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7)) : throw new InvalidOperationException("Список должен иметь 8 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 9 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8)) : throw new InvalidOperationException("Список должен иметь 9 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 10 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9)) : throw new InvalidOperationException("Список должен иметь 10 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 11 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10)) : throw new InvalidOperationException("Список должен иметь 11 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 12 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11)) : throw new InvalidOperationException("Список должен иметь 12 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 13 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12)) : throw new InvalidOperationException("Список должен иметь 13 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 14 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13)) : throw new InvalidOperationException("Список должен иметь 14 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 15 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14)) : throw new InvalidOperationException("Список должен иметь 15 элементов.");

	public static explicit operator (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)(List<T> x) => x._size == 16 ? (x.GetInternal(0), x.GetInternal(1), x.GetInternal(2), x.GetInternal(3), x.GetInternal(4), x.GetInternal(5), x.GetInternal(6), x.GetInternal(7), x.GetInternal(8), x.GetInternal(9), x.GetInternal(10), x.GetInternal(11), x.GetInternal(12), x.GetInternal(13), x.GetInternal(14), x.GetInternal(15)) : throw new InvalidOperationException("Список должен иметь 16 элементов.");
}
