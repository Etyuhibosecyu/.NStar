using System.Diagnostics;

namespace Corlib.NStar;

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public abstract partial class List<T, TCertain> : ListBase<T, TCertain> where TCertain : List<T, TCertain>, new()
{
	private protected T[] _items;

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

	public List(params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		_items = array.ToArray();
	}

	public List(int capacity, params T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));
		_size = array.Length;
		if (array.Length > capacity)
			_items = array.ToArray();
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
			if (value == _items.Length)
				return;
			if (value > 0)
			{
				T[] newItems = new T[value];
				if (_size > 0)
					Array.Copy(_items, 0, newItems, 0, _size);
				_items = newItems;
			}
			else
				_items = _emptyArray;
			Changed();
		}
	}

	public virtual TCertain AddRange(ReadOnlySpan<T> span) => Insert(_size, span);

	public override Span<T> AsSpan(int index, int count)
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

	public virtual int BinarySearch(T item) => BinarySearch(0, _size, item, G.Comparer<T>.Default);

	public virtual int BinarySearch(T item, IComparer<T> comparer) => BinarySearch(0, _size, item, comparer);

	public virtual int BinarySearch(int index, int count, T item, IComparer<T> comparer)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		return Array.BinarySearch(_items, index, count, item, comparer);
	}

	private protected override void ClearInternal(int index, int count)
	{
		Array.Clear(_items, index, count);
		Changed();
	}

	public virtual List<TOutput> Convert<TOutput>(Func<T, TOutput> converter) => base.Convert<TOutput, List<TOutput>>(converter);

	public virtual List<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) => base.Convert<TOutput, List<TOutput>>(converter);

	private protected override void Copy(ListBase<T, TCertain> source, int sourceIndex, ListBase<T, TCertain> destination, int destinationIndex, int count)
	{
		Array.Copy((source as TCertain ?? throw new ArgumentException(null, nameof(source)))._items, sourceIndex, (destination as TCertain ?? throw new ArgumentException(null, nameof(destination)))._items, destinationIndex, count);
		Changed();
	}

	private protected override void CopyToInternal(Array array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, _size);

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count) => Array.Copy(_items, index, array, arrayIndex, count);

	public override void Dispose()
	{
		_items = default!;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = _items[index];
		if (invoke)
			Changed();
		return item;
	}

	private protected override int IndexOfInternal(T item, int index, int count) => Array.IndexOf(_items, item, index, count);

	public override TCertain Insert(int index, T item)
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
				Copy(this as TCertain ?? throw new InvalidOperationException(), index, this as TCertain ?? throw new InvalidOperationException(), index + 1, _size - index);
			_items[index] = item;
		}
		_size++;
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain Insert(int index, ReadOnlySpan<T> span)
	{
		int count = span.Length;
		if (count == 0)
			return this as TCertain ?? throw new InvalidOperationException();
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
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		if (collection is List<T, TCertain> list)
		{
			int count = list._size;
			if (count == 0)
				return this as TCertain ?? throw new InvalidOperationException();
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
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else if (collection is T[] array)
		{
			int count = array.Length;
			if (count == 0)
				return this as TCertain ?? throw new InvalidOperationException();
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
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else if (collection is ICollection<T> list2)
		{
			int count = list2.Count;
			if (count == 0)
				return this as TCertain ?? throw new InvalidOperationException();
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
			Changed();
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return InsertInternal(index, CollectionCreator(collection));
	}

	private protected override int LastIndexOfInternal(T item, int index, int count) => Array.LastIndexOf(_items, index, count);

	public virtual TCertain NSort() => NSort(0, _size);

	public unsafe virtual TCertain NSort(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (this is List<uint> uintList)
		{
			uintList._items.NSort(index, count);
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return Sort(index, count, G.Comparer<T>.Default);
	}

	public virtual TCertain NSort(Func<T, uint> function) => NSort(function, 0, _size);

	public unsafe virtual TCertain NSort(Func<T, uint> function, int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		_items.NSort(function, index, count);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public static List<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) => collection is List<TList> list ? list : new(collection);

	private protected override TCertain ReverseInternal(int index, int count)
	{
		Array.Reverse(_items, index, count);
		Changed();
		return this as TCertain ?? throw new InvalidOperationException();
	}

	internal override void SetInternal(int index, T value)
	{
		_items[index] = value;
		Changed();
	}

	public virtual TCertain Sort() => Sort(0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort(IComparer<T> comparer) => Sort(0, _size, comparer);

	public virtual TCertain Sort(int index, int count, IComparer<T> comparer)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		Array.Sort(_items, index, count, comparer);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	public virtual TCertain Sort<TValue>(Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(0, _size, function, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int count, Func<T, TValue> function, bool fasterButMoreMemory = true) => Sort(index, count, function, G.Comparer<TValue>.Default, fasterButMoreMemory);

	public virtual TCertain Sort<TValue>(int index, int count, Func<T, TValue> function, IComparer<TValue> comparer, bool fasterButMoreMemory = true)
	{
		if (fasterButMoreMemory)
		{
			Convert(function).Sort(this, index, count, comparer);
			return this as TCertain ?? throw new InvalidOperationException();
		}
		else
			return Sort(index, count, new Comparer<T>((x, y) => comparer.Compare(function(x), function(y))));
	}

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, G.Comparer<T>.Default);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new() => Sort(values, 0, _size, comparer);

	public virtual TCertain Sort<TValue, TValueCertain>(List<TValue, TValueCertain> values, int index, int count, IComparer<T>? comparer) where TValueCertain : List<TValue, TValueCertain>, new()
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
		return this as TCertain ?? throw new InvalidOperationException();
	}
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public class List<T> : List<T, List<T>>
{
	public List()
	{
	}

	public List(int capacity) : base(capacity)
	{
	}

	public List(IEnumerable<T> collection) : base(collection)
	{
	}

	public List(int capacity, IEnumerable<T> collection) : base(capacity, collection)
	{
	}

	public List(params T[] array) : base(array)
	{
	}

	public List(int capacity, params T[] array) : base(capacity, array)
	{
	}

	public List(ReadOnlySpan<T> span) : base(span)
	{
	}

	public List(int capacity, ReadOnlySpan<T> span) : base(capacity, span)
	{
	}

	private protected override Func<int, List<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, List<T>> CollectionCreator => x => new(x);

	public static implicit operator List<T>(T x) => new(x);

	public static implicit operator List<T>(T[] x) => new(x);
}

[DebuggerDisplay("{ToString()}")]
[ComVisible(true)]
[Serializable]
public class String : List<char, String>
{
	public String()
	{
	}

	public String(int capacity) : base(capacity)
	{
	}

	public String(IEnumerable<char> collection) : base(collection)
	{
	}

	public String(params char[] array) : base(array)
	{
	}

	public String(ReadOnlySpan<char> span) : base(span)
	{
	}

	public String(int capacity, IEnumerable<char> collection) : base(capacity, collection)
	{
	}

	public String(int capacity, params char[] array) : base(capacity, array)
	{
	}

	public String(int capacity, ReadOnlySpan<char> span) : base(capacity, span)
	{
	}

	private protected override Func<int, String> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<char>, String> CollectionCreator => x => new(x);

	public override string ToString() => new(AsSpan());

	public static implicit operator String(char x) => new(x);

	public static implicit operator String(char[] x) => new(x);

	public static implicit operator String(string x) => new((ReadOnlySpan<char>)x);
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public class BigList<T> : BigListBase<T, BigList<T>, List<T>>
{
	public BigList()
	{
	}

	public BigList(mpz_t capacity) : base(capacity)
	{
	}

	public BigList(IEnumerable<T> col) : base(col)
	{
	}

	private protected override Func<mpz_t, BigList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, BigList<T>> CollectionCreator => x => new(x);

	private protected override Func<int, List<T>> CapacityLowCreator => x => new(x);

	private protected override Func<IEnumerable<T>, List<T>> CollectionLowCreator => x => new(x);

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
				if (sourceEndIntIndex + intOffset < destinationBits.Length && buff.isHigh && buff.high != null)
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
			//		if (sourceCurrentIntIndex + 1 < sourceBits.Length) buff |= ((ulong)sourceBits[sourceCurrentIntIndex + 1]) << bitsOffset;
			//	}
			//	if (sourceEndIntIndex + intOffset < destinationBits.Length)
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
		if (sourceBits.Length == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Length == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceBits.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationBits.Length)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}
}

[DebuggerDisplay("Length = {Length}")]
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
			if (value == _capacity)
				return;
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
			Changed();
		}
	}

	private protected override Func<int, NList<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, NList<T>> CollectionCreator => x => new(x);

	public virtual NList<T> AddRange(ReadOnlySpan<T> span) => InsertRange(_size, span);

	public override Span<T> AsSpan(int index, int count)
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

	private protected override void ClearInternal(int index, int count)
	{
		FillMemory(_items + index, count, 0);
		Changed();
	}

	public virtual NList<TOutput> Convert<TOutput>(Func<T, TOutput> converter) where TOutput : unmanaged => base.Convert<TOutput, NList<TOutput>>(converter);

	public virtual NList<TOutput> Convert<TOutput>(Func<T, int, TOutput> converter) where TOutput : unmanaged => base.Convert<TOutput, NList<TOutput>>(converter);

	private protected override void Copy(ListBase<T, NList<T>> source, int sourceIndex, ListBase<T, NList<T>> destination, int destinationIndex, int count)
	{
		CopyMemory((source as NList<T> ?? throw new ArgumentException(null, nameof(source)))._items, sourceIndex, (destination as NList<T> ?? throw new ArgumentException(null, nameof(destination)))._items, destinationIndex, count);
		Changed();
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException(null, nameof(array));
		fixed (T* ptr = array2)
			CopyMemory(_items, 0, ptr, arrayIndex, _size);
	}

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int count)
	{
		fixed (T* ptr = array)
			CopyMemory(_items, index, ptr, arrayIndex, count);
	}

	public override void Dispose() => GC.SuppressFinalize(this);

	internal override T GetInternal(int index, bool invoke = true)
	{
		T item = _items[index];
		if (invoke)
			Changed();
		return item;
	}

	public override NList<T> GetRange(int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (index + count > _size)
			throw new ArgumentException(null);
		if (count == 0)
			return new();
		else if (index == 0 && count == _size)
			return this;
		NList<T> list = new(count) { _items = _items + index, _size = count };
		return list;
	}

	private protected override int IndexOfInternal(T item, int index, int count)
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
		Changed();
		return this;
	}

	public virtual NList<T> InsertRange(int index, ReadOnlySpan<T> span)
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
		Changed();
		return this;
	}

	private protected override NList<T> InsertInternal(int index, IEnumerable<T> collection)
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
			Changed();
			return this;
		}
		else
			return InsertInternal(index, new NList<T>(collection));
	}

	private protected override int LastIndexOfInternal(T item, int index, int count)
	{
		int endIndex = index - count + 1;
		for (int i = index; i >= endIndex; i--)
			if (_items[i].Equals(item))
				return i;
		return -1;
	}

	public static NList<TList> ReturnOrConstruct<TList>(IEnumerable<TList> collection) where TList : unmanaged => collection is NList<TList> list ? list : new(collection);

	private protected override NList<T> ReverseInternal(int index, int count)
	{
		for (int i = 0; i < _size / 2; i++)
			(_items[i], _items[_size - 1 - i]) = (_items[_size - 1 - i], _items[i]);
		Changed();
		return this;
	}

	internal override void SetInternal(int index, T value)
	{
		_items[index] = value;
		Changed();
	}

	public virtual NList<T> Sort() => Sort(0, _size);

	public virtual NList<T> Sort(int index, int count)
	{
		if (this is NList<uint> uintList)
		{
			uint* shiftedItems = uintList._items + index;
			RadixSort(&shiftedItems, count);
			return this;
		}
		else
			throw new NotSupportedException();
	}

	public virtual NList<T> Sort(Func<T, uint> function) => Sort(function, 0, _size);

	public virtual NList<T> Sort(Func<T, uint> function, int index, int count) =>
		//Radix.Sort(_items + index, function, count);
		this;

	public static implicit operator NList<T>(T x) => new NList<T>().Add(x);
}
