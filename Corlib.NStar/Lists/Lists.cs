using System.Diagnostics;
using System.Numerics;

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

	private protected override void Copy(TCertain source, int sourceIndex, TCertain destination, int destinationIndex, int count)
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
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (span == null)
			throw new ArgumentNullException(nameof(span));
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
		else if (collection is G.ICollection<T> list2)
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

	private protected override int LastIndexOfInternal(T item, int index, int count) => Array.LastIndexOf(_items, item, index, count);

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
	public String() : base()
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
				Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
			}
			else
			{
				Marshal.FreeHGlobal((nint)_items);
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

	private protected override void Copy(NList<T> source, int sourceIndex, NList<T> destination, int destinationIndex, int count)
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

	public override void Dispose()
	{
		Marshal.FreeHGlobal((nint)_items);
		_capacity = 0;
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
			Marshal.FreeHGlobal((nint)_items);
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
			Marshal.FreeHGlobal((nint)_items);
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
				Marshal.FreeHGlobal((nint)_items);
				_items = newItems;
				_capacity = newCapacity;
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
				Marshal.FreeHGlobal((nint)_items);
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
				Marshal.FreeHGlobal((nint)_items);
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

	public virtual NList<T> Sort(Func<T, uint> function, int index, int count)
	{
		NSort(_items, function, index, count);
		return this;
	}

	public static implicit operator NList<T>(T x) => new NList<T>().Add(x);
}

internal delegate bool SumWalkPredicate(SumList.Node node);

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public partial class SumList : ListBase<int, SumList>
{
	private Node? root;
	private int version;

	public SumList()
	{
	}

	public SumList(IEnumerable<int> collection) : this()
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is SumList sumList && sumList is not TreeSubSet)
		{
			if (sumList.Length > 0)
			{
				Debug.Assert(sumList.root != null);
				_size = sumList._size;
				root = sumList.root.DeepClone(_size);
			}
			return;
		}
		var elements = collection.ToArray();
		int count = elements.Length;
		if (count > 0)
		{
			root = ConstructRootFromSortedArray(elements, 0, count - 1, null);
			_size = count;
		}
	}

	public SumList(params int[] array) : this((IEnumerable<int>)array)
	{
	}

	public SumList(ReadOnlySpan<int> span) : this((IEnumerable<int>)span.ToArray())
	{
	}

	public override int Capacity
	{
		get => _size;
		set
		{
		}
	}

	private protected override Func<int, SumList> CapacityCreator => x => new();

	private protected override Func<IEnumerable<int>, SumList> CollectionCreator => x => new(x);

	private protected virtual IComparer<int> Comparer => G.Comparer<int>.Default;

	public override int Length
	{
		get
		{
			VersionCheck(updateCount: true);
			return _size;
		}
	}

	public virtual int Max => MaxInternal;

	internal virtual int MaxInternal => _size - 1;

	public virtual int Min => MinInternal;

	internal virtual int MinInternal => 0;

	public override SumList Add(int value) => Insert(_size, value);

	public override Span<int> AsSpan(int index, int count) => throw new NotSupportedException();

	/// <summary>
	/// Does a left-to-right breadth-first tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool BreadthFirstTreeWalk(SumWalkPredicate action)
	{
		if (root == null)
			return true;
		var processQueue = new Queue<Node>();
		processQueue.Enqueue(root);
		Node current;
		while (processQueue.Length != 0)
		{
			current = processQueue.Dequeue();
			if (!action(current))
				return false;
			if (current.Left != null)
				processQueue.Enqueue(current.Left);
			if (current.Right != null)
				processQueue.Enqueue(current.Right);
		}
		return true;
	}

	public override void Clear()
	{
		root = null;
		_size = 0;
		++version;
	}

	private protected override void ClearInternal(int index, int count) => new TreeSubSet(this, index, index + count - 1, true, true).Clear();

	private static Node? ConstructRootFromSortedArray(int[] arr, int startIndex, int endIndex, Node? redNode)
	{
		// You're given a sorted array... say 1 2 3 4 5 6
		// There are 2 cases:
		// -  If there are odd # of elements, pick the middle element (in this case 4), and compute
		//    its left and right branches
		// -  If there are even # of elements, pick the left middle element, save the right middle element
		//    and call the function on the rest
		//    1 2 3 4 5 6 -> pick 3, save 4 and call the fn on 1,2 and 5,6
		//    now add 4 as a red node to the lowest element on the right branch
		//             3                       3
		//         1       5       ->     1        5
		//           2       6             2     4   6
		//    As we're adding to the leftmost of the right branch, nesting will not hurt the red-black properties
		//    Leaf nodes are red if they have no sibling (if there are 2 nodes or if a node trickles
		//    down to the bottom

		// This is done recursively because the iterative way to do this ends up wasting more space than it saves in stack frames
		// Only some base cases are handled below.
		int size = endIndex - startIndex + 1;
		Node root;
		switch (size)
		{
			case 0:
				return null;
			case 1:
				root = new Node(arr[startIndex], NodeColor.Black);
				if (redNode != null)
					root.Left = redNode;
				break;
			case 2:
				root = new Node(arr[startIndex], NodeColor.Black)
				{
					Right = new Node(arr[endIndex], NodeColor.Black)
				};
				root.Right.ColorRed();
				if (redNode != null)
					root.Left = redNode;
				break;
			case 3:
				root = new Node(arr[startIndex + 1], NodeColor.Black)
				{
					Left = new Node(arr[startIndex], NodeColor.Black),
					Right = new Node(arr[endIndex], NodeColor.Black)
				};
				if (redNode != null)
					root.Left.Left = redNode;
				break;
			default:
				int midpt = (startIndex + endIndex) / 2;
				root = new Node(arr[midpt], NodeColor.Black)
				{
					Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode),
					Right = size % 2 == 0 ?
					ConstructRootFromSortedArray(arr, midpt + 2, endIndex, new Node(arr[midpt + 1], NodeColor.Red)) :
					ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null)
				};
				break;
		}
		return root;
	}

	private protected override void Copy(SumList source, int sourceIndex, SumList destination, int destinationIndex, int count)
	{
		if (count == 0)
			return;
		if (count == 1)
		{
			destination.SetInternal(destinationIndex, source.GetInternal(sourceIndex));
			return;
		}
		TreeSubSet subset = new(source as SumList ?? throw new ArgumentException(null, nameof(source)), sourceIndex, sourceIndex + count - 1, true, true);
		var en = subset.GetEnumerator();
		if (destinationIndex < destination._size)
			new TreeSubSet(destination, destinationIndex, Min(destinationIndex + count, destination._size) - 1, true, true).InOrderTreeWalk(node =>
			{
				bool b = en.MoveNext();
				if (b)
					node.Value = en.Current;
				return b;
			});
		while (en.MoveNext())
			destination.Add(en.Current);
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is int[] array2)
			CopyToInternal(0, array2, arrayIndex, _size);
		else
			throw new ArgumentException(null, nameof(array));
	}

	private protected override void CopyToInternal(int index, int[] array, int arrayIndex, int count)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (count > array.Length - index)
			throw new ArgumentException(null);
		count += index; // Make `count` the upper bound.
		int i = 0;
		InOrderTreeWalk(node =>
		{
			if (i >= count)
				return false;
			if (i++ < index)
				return true;
			array[arrayIndex++] = node.Value;
			return true;
		});
	}

	public virtual bool Decrease(int index) => Update(index, GetInternal(index) - 1);

	public override void Dispose()
	{
		root = null;
		_size = 0;
		version = 0;
		GC.SuppressFinalize(this);
	}

	private void FindForRemove(int index2, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch)
	{
		// Search for a node and then find its successor.
		// Then copy the value from the successor to the matching node, and delete the successor.
		// If a node doesn't have a successor, we can replace it with its left child (if not empty),
		// or delete the matching node.
		//
		// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
		// Following code will make sure the node on the path is not a 2-node.

		// Even if we don't actually remove from the list, we may be altering its structure (by doing rotations
		// and such). So update our version to disable any enumerators/subsets working on it.
		version++;
		Node? current = root;
		parent = null;
		grandParent = null;
		match = null;
		parentOfMatch = null;
		bool foundMatch = false;
		while (current != null)
		{
			if (current.Is2Node)
			{
				// Fix up 2-node
				if (parent == null)
					current.ColorRed();
				else if (parent.Left != null && parent.Right != null)
				{
					Node sibling = parent.GetSibling(current);
					if (sibling.IsRed)
					{
						// If parent is a 3-node, flip the orientation of the red link.
						// We can achieve this by a single rotation.
						// This case is converted to one of the other cases below.
						Debug.Assert(parent.IsBlack);
						if (parent.Right == sibling)
							parent.RotateLeft();
						else
							parent.RotateRight();
						parent.ColorRed();
						sibling.ColorBlack(); // The red parent can't have black children.
											  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
						ReplaceChildOrRoot(grandParent, parent, sibling);
						// `sibling` will become the grandparent of `current`.
						grandParent = sibling;
						if (parent == match)
							parentOfMatch = sibling;
						sibling = parent.GetSibling(current);
					}
					Debug.Assert(Node.IsNonNullBlack(sibling));
					if (sibling.Is2Node)
						parent.Merge2Nodes();
					else
					{
						// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
						// We can change the color of `current` to red by some rotation.
						Node newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;
						newGrandParent.Color = parent.Color;
						parent.ColorBlack();
						current.ColorRed();
						ReplaceChildOrRoot(grandParent, parent, newGrandParent);
						if (parent == match)
							parentOfMatch = newGrandParent;
					}
				}
			}
			grandParent = parent;
			parent = current;
			if (foundMatch)
				current = current.Left;
			else if ((current.Left?.LeavesCount ?? 0) == index2)
			{
				// Save the matching node.
				foundMatch = true;
				match = current;
				parentOfMatch = grandParent;
				current = current.Right;
			}
			else if (current.Left == null)
			{
				index2--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount >= index2)
				current = current.Left;
			else
			{
				index2 -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
	}

	internal virtual Node? FindNode(int index)
	{
		Node? current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
				return current;
			else if (current.Left == null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount > index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		return null;
	}

	internal Node? FindRange(int from, int to) => FindRange(from, to, true, true);

	internal Node? FindRange(int from, int to, bool lowerBoundActive, bool upperBoundActive)
	{
		Node? current = root;
		while (current != null)
		{
			if (lowerBoundActive && Comparer.Compare(from, current.Left?.LeavesCount ?? 0) > 0)
				current = current.Right;
			else if (upperBoundActive && Comparer.Compare(to, current.Left?.LeavesCount ?? 0) < 0)
				current = current.Left;
			else
				return current;
		}
		return null;
	}

	public virtual int GetAndRemove(Index index)
	{
		int index2 = index.IsFromEnd ? _size - index.Value : index.Value;
		if (root == null)
		{
			return default!;
		}
		FindForRemove(index2, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch);
		int found = default!;
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			found = match.Left?.LeavesCount ?? 0;
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
		root?.ColorBlack();
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return found;
	}

	public override IEnumerator<int> GetEnumerator() => new Enumerator(this);

	internal override int GetInternal(int index, bool invoke = true)
	{
		Node? current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				var value = current.Value;
				if (invoke)
					Changed();
				return value;
			}
			else if (current.Left == null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount > index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	public virtual long GetLeftValuesSum(int index, out int actualValue)
	{
		Node? current = root;
		long sum = 0;
		while (current != null)
		{
			int order = Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				actualValue = current.Value;
				return sum + (current.Left?.ValuesSum ?? 0);
			}
			else if (order < 0)
			{
				current = current.Left;
			}
			else
			{
				index -= (current.Left?.LeavesCount ?? 0) + 1;
				sum += (current.Left?.ValuesSum ?? 0) + current.Value;
				current = current.Right;
			}
		}
		actualValue = 0;
		return sum;
	}

	public virtual SumList GetViewBetween(int lowerValue, int upperValue)
	{
		if (Comparer.Compare(lowerValue, upperValue) > 0)
			throw new ArgumentException(null, nameof(lowerValue));
		return new TreeSubSet(this, lowerValue, upperValue, true, true);
	}

	public virtual bool Increase(int index) => Update(index, GetInternal(index) + 1);

	private protected override int IndexOfInternal(int value, int index, int count) => throw new NotSupportedException();

	/// <summary>
	/// Does an in-order tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool InOrderTreeWalk(SumWalkPredicate action)
	{
		if (root == null)
			return true;
		// The maximum height of a red-black tree is 2 * log2(n+1).
		// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
		// Note: It's not strictly necessary to provide the stack capacity, but we don't
		// want the stack to unnecessarily allocate arrays as it grows.
		var stack = new Stack<Node>(2 * Log2(Length + 1));
		Node? current = root;
		while (current != null)
		{
			stack.Push(current);
			current = current.Left;
		}
		while (stack.Length != 0)
		{
			current = stack.Pop();
			if (!action(current))
				return false;
			Node? node = current.Right;
			while (node != null)
			{
				stack.Push(node);
				node = node.Left;
			}
		}
		return true;
	}

	public override SumList Insert(int index, int value)
	{
		if (root == null)
		{
			// The tree is empty and this is the first value.
			root = new Node(value, NodeColor.Black);
			_size = 1;
			version++;
			return this;
		}
		// Search for a node at bottom to insert the new node.
		// If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
		// We split 4-nodes along the search path.
		Node? current = root;
		Node? parent = null;
		Node? grandParent = null;
		Node? greatGrandParent = null;
		// Even if we don't actually add to the list, we may be altering its structure (by doing rotations and such).
		// So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
		version++;
		int oldIndex = index;
		int order = 0;
		bool foundMatch = false;
		while (current != null)
		{
			order = foundMatch ? 1 : Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				// We could have changed root node to red during the search process.
				// We need to set it to black before we return.
				root.ColorBlack();
				foundMatch = true;
			}
			// Split a 4-node into two 2-nodes.
			if (current.Is4Node)
			{
				current.Split4Node();
				// We could have introduced two consecutive red nodes after split. Fix that by rotation.
				if (Node.IsNonNullRed(parent))
					InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
				index = oldIndex;
				current = root;
				greatGrandParent = grandParent = parent = null;
				foundMatch = false;
				continue;
			}
			//if (current.Is2Node)
			//{
			//	// Fix up 2-node
			//	if (parent == null)
			//		current.ColorRed();
			//	else if (parent.Left != null && parent.Right != null)
			//	{
			//		Node sibling = parent.GetSibling(current);
			//		if (sibling.IsRed)
			//		{
			//			// If parent is a 3-node, flip the orientation of the red link.
			//			// We can achieve this by a single rotation.
			//			// This case is converted to one of the other cases below.
			//			Debug.Assert(parent.IsBlack);
			//			if (parent.Right == sibling)
			//				parent.RotateLeft();
			//			else
			//				parent.RotateRight();
			//			parent.ColorRed();
			//			sibling.ColorBlack(); // The red parent can't have black children.
			//								  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
			//			ReplaceChildOrRoot(grandParent, parent, sibling);
			//			// `sibling` will become the grandparent of `current`.
			//			grandParent = sibling;
			//			sibling = parent.GetSibling(current);
			//		}
			//		Debug.Assert(Node.IsNonNullBlack(sibling));
			//		if (sibling.Is2Node)
			//			parent.Merge2Nodes();
			//		else
			//		{
			//			// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
			//			// We can change the color of `current` to red by some rotation.
			//			Node newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;
			//			newGrandParent.Color = parent.Color;
			//			parent.ColorBlack();
			//			current.ColorRed();
			//			ReplaceChildOrRoot(grandParent, parent, newGrandParent);
			//		}
			//	}
			//}
			greatGrandParent = grandParent;
			grandParent = parent;
			parent = current;
			if (order <= 0)
				current = current.Left;
			else
			{
				index -= (current.Left?.LeavesCount ?? 0) + 1;
				current = current.Right;
			}
		}
#if DEBUG
		if (index != 0)
			throw new InvalidOperationException();
#endif
		Debug.Assert(parent != null);
		// We're ready to insert the new node.
		Node node = new(value, NodeColor.Red);
		if (order <= 0)
			parent.Left = node;
		else
			parent.Right = node;
		// The new node will be red, so we will need to adjust colors if its parent is also red.
		if (parent.IsRed)
			InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
#if DEBUG
		if (_size + 1 != root.LeavesCount)
			throw new InvalidOperationException();
		foreach (var x in new[] { node, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
		// The root node is always black.
		root.ColorBlack();
		++_size;
		return this;
	}

	// After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
	// It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
	// need to split again in the next node.
	// By the time we need to split again, everything will be correctly set.
	private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
	{
		Debug.Assert(parent != null);
		Debug.Assert(grandParent != null);
		bool parentIsOnRight = grandParent.Right == parent;
		bool currentIsOnRight = parent.Right == current;
		Node newChildOfGreatGrandParent;
		if (parentIsOnRight == currentIsOnRight)
			newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft() : grandParent.RotateRight();
		else
		{
			// Different orientation, double rotation
			newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft();
			// Current node now becomes the child of `greatGrandParent`
			parent = greatGrandParent;
		}
		// `grandParent` will become a child of either `parent` of `current`.
		grandParent.ColorRed();
		newChildOfGreatGrandParent.ColorBlack();
		ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
#if DEBUG
		foreach (var x in new[] { current, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
	}

	// Virtual function for TreeSubSet, which may need to do range checks.
	internal virtual bool IsWithinRange(int index) => true;

	private protected override int LastIndexOfInternal(int value, int index, int count) => throw new NotSupportedException();

	// Used for set checking operations (using enumerables) that rely on counting
	private static int Log2(int value) => BitOperations.Log2((uint)value);

	public override SumList RemoveAt(int index)
	{
		if (root == null)
		{
			return this;
		}
		FindForRemove(index, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch);
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
		root?.ColorBlack();
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return this;
	}

	/// <summary>
	/// Replaces the child of a parent node, or replaces the root if the parent is <c>null</c>.
	/// </summary>
	/// <param name="parent">The (possibly <c>null</c>) parent.</param>
	/// <param name="child">The child node to replace.</param>
	/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
	private void ReplaceChildOrRoot(Node? parent, Node child, Node newChild)
	{
		if (parent != null)
			parent.ReplaceChild(child, newChild);
		else
		{
			root = newChild;
			root?.Isolate();
		}
	}

	/// <summary>
	/// Replaces the matching node with its successor.
	/// </summary>
	private void ReplaceNode(Node match, Node parentOfMatch, Node successor, Node parentOfSuccessor)
	{
		Debug.Assert(match != null);
		if (successor == match)
		{
			// This node has no successor. This can only happen if the right child of the match is null.
			Debug.Assert(match.Right == null);
			successor = match.Left!;
		}
		else
		{
			Debug.Assert(parentOfSuccessor != null);
			Debug.Assert(successor.Left == null);
			Debug.Assert((successor.Right == null && successor.IsRed) || (successor.Right!.IsRed && successor.IsBlack));
			successor.Right?.ColorBlack();
			if (parentOfSuccessor != match)
			{
				// Detach the successor from its parent and set its right child.
				parentOfSuccessor.Left = successor.Right;
				successor.Right = match.Right;
				parentOfSuccessor.FixUp();
			}
			successor.Left = match.Left;
		}
		if (successor != null)
			successor.Color = match.Color;
		ReplaceChildOrRoot(parentOfMatch, match, successor!);
#if DEBUG
		foreach (var x in new[] { match, parentOfMatch, successor, parentOfSuccessor })
			x?.Verify();
#endif
	}

	private protected override SumList ReverseInternal(int index, int count) => throw new NotSupportedException();

	internal override void SetInternal(int index, int value)
	{
		Node? current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				current.Value = value;
				Changed();
				return;
			}
			else if (current.Left == null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount >= index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	// Virtual function for TreeSubSet, which may need the count variable of the parent list.
	internal virtual int TotalCount() => Length;

	public virtual bool Update(int index, int value)
	{
		var node = FindNode(index);
		if (node != null)
		{
			node.Update(value);
#if DEBUG
			foreach (var x in new[] { node, root })
				x?.Verify();
#endif
			return true;
		}
		else
			return false;
	}

	internal void UpdateVersion() => ++version;

	// Virtual function for TreeSubSet, which may need to update its count.
	internal virtual void VersionCheck(bool updateCount = false) { }

#if DEBUG
	/// <summary>
	/// debug status to be checked whenever any operation is called
	/// </summary>
	/// <returns></returns>
	internal virtual bool VersionUpToDate() => true;
#endif

	[DebuggerDisplay("{Value.ToString()}, Left = {Left?.Value.ToString()}, Right = {Right?.Value.ToString()}, Parent = {Parent?.Value.ToString()}")]
	internal sealed class Node
	{
		private Node? _left;
		private Node? _right;
		private Node? Parent { get; set; }
		private int _leavesCount;
		private long _valuesSum;

		public Node(int value, NodeColor color)
		{
			Value = value;
			Color = color;
			_leavesCount = 1;
			_valuesSum = value;
		}

		public int Value { get; set; }

		internal Node? Left
		{
			get => _left;
			set
			{
				if (_left == value)
					return;
				if (_left != null && _left.Parent != value)
					_left.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_left?.LeavesCount ?? 0);
				ValuesSum += (value?.ValuesSum ?? 0) - (_left?.ValuesSum ?? 0);
				_left = value;
				if (_left != null)
					_left.Parent = this;
#if DEBUG
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
#endif
			}
		}

		internal Node? Right
		{
			get => _right;
			set
			{
				if (_right == value)
					return;
				if (_right != null && _right.Parent != value)
					_right.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_right?.LeavesCount ?? 0);
				ValuesSum += (value?.ValuesSum ?? 0) - (_right?.ValuesSum ?? 0);
				_right = value;
				if (_right != null)
					_right.Parent = this;
#if DEBUG
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
#endif
			}
		}

		internal int LeavesCount
		{
			get => _leavesCount;
			set
			{
				if (Parent != null)
					Parent.LeavesCount += value - _leavesCount;
				_leavesCount = value;
				if (Parent != null && Parent.LeavesCount != (Parent._left?.LeavesCount ?? 0) + (Parent._right?.LeavesCount ?? 0) + 1)
					throw new InvalidOperationException();
			}
		}

		internal long ValuesSum
		{
			get => _valuesSum;
			set
			{
				if (Parent != null)
					Parent.ValuesSum += value - _valuesSum;
				_valuesSum = value;
				if (Parent != null && Parent.ValuesSum != (Parent._left?.ValuesSum ?? 0) + (Parent._right?.ValuesSum ?? 0) + Parent.Value)
					throw new InvalidOperationException();
			}
		}

		public NodeColor Color { get; set; }

		public bool IsBlack => Color == NodeColor.Black;

		public bool IsRed => Color == NodeColor.Red;

		public bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

		public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

		public void ColorBlack() => Color = NodeColor.Black;

		public void ColorRed() => Color = NodeColor.Red;

		public Node DeepClone(int count)
		{
#if DEBUG
			Debug.Assert(count == GetCount());
#endif
			Node newRoot = ShallowClone();
			var pendingNodes = new Stack<(Node source, Node target)>(2 * Log2(count) + 2);
			pendingNodes.Push((this, newRoot));
			while (pendingNodes.TryPop(out var next))
			{
				Node clonedNode;
				if (next.source.Left is Node left)
				{
					clonedNode = left.ShallowClone();
					next.target.Left = clonedNode;
					pendingNodes.Push((left, clonedNode));
				}
				if (next.source.Right is Node right)
				{
					clonedNode = right.ShallowClone();
					next.target.Right = clonedNode;
					pendingNodes.Push((right, clonedNode));
				}
			}
			return newRoot;
		}

		public void FixUp()
		{
			if (Left != null)
				Left.Parent = this;
			if (Right != null)
				Right.Parent = this;
		}

		/// <summary>
		/// Gets the rotation this node should undergo during a removal.
		/// </summary>
		public TreeRotation GetRotation(Node current, Node sibling)
		{
			Debug.Assert(IsNonNullRed(sibling.Left) || IsNonNullRed(sibling.Right));
#if DEBUG
			Debug.Assert(HasChildren(current, sibling));
#endif

			bool currentIsLeftChild = Left == current;
			return IsNonNullRed(sibling.Left) ?
				(currentIsLeftChild ? TreeRotation.RightLeft : TreeRotation.Right) :
				(currentIsLeftChild ? TreeRotation.Left : TreeRotation.LeftRight);
		}

		/// <summary>
		/// Gets the sibling of one of this node's children.
		/// </summary>
		public Node GetSibling(Node node)
		{
			Debug.Assert(node != null);
			Debug.Assert(node == Left ^ node == Right);
			return node == Left ? Right! : Left!;
		}

		public static bool IsNonNullBlack(Node? node) => node != null && node.IsBlack;

		public static bool IsNonNullRed(Node? node) => node != null && node.IsRed;

		public static bool IsNullOrBlack(Node? node) => node == null || node.IsBlack;

		public void Isolate()
		{
			if (Parent != null && Parent.Left == this)
				Parent.Left = null;
			if (Parent != null && Parent.Right == this)
				Parent.Right = null;
		}

		/// <summary>
		/// Combines two 2-nodes into a 4-node.
		/// </summary>
		public void Merge2Nodes()
		{
			Debug.Assert(IsRed);
			Debug.Assert(Left!.Is2Node);
			Debug.Assert(Right!.Is2Node);
			// Combine two 2-nodes into a 4-node.
			ColorBlack();
			Left.ColorRed();
			Right.ColorRed();
		}

		/// <summary>
		/// Replaces a child of this node with a new node.
		/// </summary>
		/// <param name="child">The child to replace.</param>
		/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
		public void ReplaceChild(Node child, Node newChild)
		{
			if (Left == child)
				Left = newChild;
			else if (Right == child)
				Right = newChild;
		}

		/// <summary>
		/// Does a rotation on this tree. May change the color of a grandchild from red to black.
		/// </summary>
		public Node? Rotate(TreeRotation rotation)
		{
			Node removeRed;
			switch (rotation)
			{
				case TreeRotation.Right:
					removeRed = Left!.Left!;
					Debug.Assert(removeRed.IsRed);
					removeRed.ColorBlack();
					return RotateRight();
				case TreeRotation.Left:
					removeRed = Right!.Right!;
					Debug.Assert(removeRed.IsRed);
					removeRed.ColorBlack();
					return RotateLeft();
				case TreeRotation.RightLeft:
					Debug.Assert(Right!.Left!.IsRed);
					return RotateRightLeft();
				case TreeRotation.LeftRight:
					Debug.Assert(Left!.Right!.IsRed);
					return RotateLeftRight();
				default:
					Debug.Fail($"{nameof(rotation)}: {rotation} is not a defined {nameof(TreeRotation)} value.");
					return null;
			}
		}

		/// <summary>
		/// Does a left rotation on this tree, making this this the new left child of the current right child.
		/// </summary>
		public Node RotateLeft()
		{
			Node child = Right!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Right = child.Left;
			child.Left = this;
			if (parent != null)
			{
				if (isRight)
					parent.Right = child;
				else
					parent.Left = child;
			}
			return child;
		}

		/// <summary>
		/// Does a left-right rotation on this tree. The left child is rotated left, then this this is rotated right.
		/// </summary>
		public Node RotateLeftRight()
		{
			Node child = Left!;
			Node grandChild = child.Right!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Left = grandChild.Right;
			grandChild.Right = this;
			child.Right = grandChild.Left;
			grandChild.Left = child;
			if (parent != null)
			{
				if (isRight)
					parent.Right = grandChild;
				else
					parent.Left = grandChild;
			}
			return grandChild;
		}

		/// <summary>
		/// Does a right rotation on this tree, making this this the new right child of the current left child.
		/// </summary>
		public Node RotateRight()
		{
			Node child = Left!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Left = child.Right;
			child.Right = this;
			if (parent != null)
			{
				if (isRight)
					parent.Right = child;
				else
					parent.Left = child;
			}
			return child;
		}

		/// <summary>
		/// Does a right-left rotation on this tree. The right child is rotated right, then this this is rotated left.
		/// </summary>
		public Node RotateRightLeft()
		{
			Node child = Right!;
			Node grandChild = child.Left!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Right = grandChild.Left;
			grandChild.Left = this;
			child.Left = grandChild.Right;
			grandChild.Right = child;
			if (parent != null)
			{
				if (isRight)
					parent.Right = grandChild;
				else
					parent.Left = grandChild;
			}
			return grandChild;
		}

		public Node ShallowClone() => new(Value, Color);

		public void Split4Node()
		{
			Debug.Assert(Left != null);
			Debug.Assert(Right != null);
			ColorRed();
			Left.ColorBlack();
			Right.ColorBlack();
		}

		public void Update(int value)
		{
			ValuesSum += value - Value;
			Value = value;
		}

#if DEBUG
		private int GetCount() => 1 + (Left?.GetCount() ?? 0) + (Right?.GetCount() ?? 0);

		private bool HasChild(Node child) => child == Left || child == Right;

		private bool HasChildren(Node child1, Node child2)
		{
			Debug.Assert(child1 != child2);
			return (Left == child1 && Right == child2)
				|| (Left == child2 && Right == child1);
		}

		internal void Verify()
		{
			if (Right != null && Right == Left)
				throw new InvalidOperationException();
			if (LeavesCount != (Left?.LeavesCount ?? 0) + (Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (ValuesSum != (Left?.ValuesSum ?? 0) + (Right?.ValuesSum ?? 0) + Value)
				throw new InvalidOperationException();
			if (Left != null && Left.Parent == null)
				throw new InvalidOperationException();
			if (Right != null && Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
	}

	public new struct Enumerator : IEnumerator<int>
	{
		private readonly SumList _tree;
		private readonly int _version;

		private readonly Stack<Node> _stack;
		private Node? _current;

		private readonly bool _reverse;

		internal Enumerator(SumList list) : this(list, reverse: false)
		{
		}

		internal Enumerator(SumList list, bool reverse)
		{
			_tree = list;
			list.VersionCheck();
			_version = list.version;
			// 2 log(n + 1) is the maximum height.
			_stack = new Stack<Node>(2 * Log2(list.TotalCount() + 1));
			_current = null;
			_reverse = reverse;
			Initialize();
		}

		public int Current
		{
			get
			{
				if (_current != null)
				{
					return _current.Value;
				}
				return default!; // Should only happen when accessing Current is undefined behavior
			}
		}

		object? IEnumerator.Current
		{
			get
			{
				if (_current == null)
				{
					throw new InvalidOperationException();
				}
				return _current.Value;
			}
		}

		internal bool NotStartedOrEnded => _current == null;

		public void Dispose() { }

		private void Initialize()
		{
			_current = null;
			Node? node = _tree.root;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_tree.IsWithinRange(node.Left?.LeavesCount ?? 0))
				{
					_stack.Push(node);
					node = next;
				}
				else if (next == null || !_tree.IsWithinRange(next.Left?.LeavesCount ?? 0))
					node = other;
				else
					node = next;
			}
		}

		public bool MoveNext()
		{
			// Make sure that the underlying subset has not been changed since
			_tree.VersionCheck();
			if (_version != _tree.version)
			{
				throw new InvalidOperationException();
			}
			if (_stack.Length == 0)
			{
				_current = null;
				return false;
			}
			_current = _stack.Pop();
			Node? node = _reverse ? _current.Left : _current.Right;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_tree.IsWithinRange(node.Left?.LeavesCount ?? 0))
				{
					_stack.Push(node);
					node = next;
				}
				else if (other == null || !_tree.IsWithinRange(other.Left?.LeavesCount ?? 0))
					node = next;
				else
					node = other;
			}
			return true;
		}

		internal void Reset()
		{
			if (_version != _tree.version)
			{
				throw new InvalidOperationException();
			}
			_stack.Clear();
			Initialize();
		}

		void IEnumerator.Reset() => Reset();
	}

	internal sealed class TreeSubSet : SumList
	{
		private readonly SumList _underlying;
		private readonly int _min;
		private readonly int _max;
		// keeps track of whether the count variable is up to date
		// up to date -> _countVersion = _underlying.version
		// not up to date -> _countVersion < _underlying.version
		private int _countVersion;
		// these exist for unbounded collections
		// for instance, you could allow this subset to be defined for i > 10. The list will throw if
		// anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
		// in the spec, and are not available, but the framework is there to make them available at some point.
		private readonly bool _lBoundActive, _uBoundActive;
		// used to see if the count is out of date

		public TreeSubSet(SumList Underlying, int Min, int Max, bool lowerBoundActive, bool upperBoundActive) : base()
		{
			_underlying = Underlying;
			_min = Min;
			_max = Max;
			_lBoundActive = lowerBoundActive;
			_uBoundActive = upperBoundActive;
			root = _underlying.FindRange(_min, _max, _lBoundActive, _uBoundActive); // root is first element within range
			_size = 0;
			version = -1;
			_countVersion = -1;
		}

		internal override int MaxInternal
		{
			get
			{
				VersionCheck();
				Node? current = root;
				int result = default;
				while (current != null)
				{
					int comp = _uBoundActive ? Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) : 1;
					if (comp < 0)
						current = current.Left;
					else
					{
						result = current.Left?.LeavesCount ?? 0;
						if (comp == 0)
						{
							break;
						}
						current = current.Right;
					}
				}
				return result!;
			}
		}

		internal override int MinInternal
		{
			get
			{
				VersionCheck();
				Node? current = root;
				int result = default;
				while (current != null)
				{
					int comp = _lBoundActive ? Comparer.Compare(_min, current.Left?.LeavesCount ?? 0) : -1;
					if (comp > 0)
						current = current.Right;
					else
					{
						result = current.Left?.LeavesCount ?? 0;
						if (comp == 0)
						{
							break;
						}
						current = current.Left;
					}
				}
				return result!;
			}
		}

		internal override bool BreadthFirstTreeWalk(SumWalkPredicate action)
		{
			VersionCheck();
			if (root == null)
			{
				return true;
			}
			Queue<Node> processQueue = new();
			processQueue.Enqueue(root);
			Node current;
			while (processQueue.Length != 0)
			{
				current = processQueue.Dequeue();
				if (IsWithinRange(current.Left?.LeavesCount ?? 0) && !action(current))
				{
					return false;
				}
				if (current.Left != null && (!_lBoundActive || Comparer.Compare(_min, current.Left.LeavesCount) < 0))
					processQueue.Enqueue(current.Left);
				if (current.Right != null && (!_uBoundActive || Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) > 0))
					processQueue.Enqueue(current.Right);
			}
			return true;
		}

		public override void Clear()
		{
			if (Length == 0)
			{
				return;
			}
			List<int> toRemove = new();
			BreadthFirstTreeWalk(n => { toRemove.Add(n.Left?.LeavesCount ?? 0); return true; });
			while (toRemove.Length != 0)
			{
				_underlying.RemoveAt(toRemove[^1]);
				toRemove.RemoveAt(toRemove.Length - 1);
			}
			root = null;
			_size = 0;
			version = _underlying.version;
		}

		internal override Node? FindNode(int index)
		{
			if (!IsWithinRange(index))
			{
				return null;
			}
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return base.FindNode(index);
		}

		// This passes functionality down to the underlying tree, clipping edges if necessary
		// There's nothing gained by having a nested subset. May as well draw it from the base
		// Cannot increase the bounds of the subset, can only decrease it
		public override SumList GetViewBetween(int lowerValue, int upperValue)
		{
			if (_lBoundActive && Comparer.Compare(_min, lowerValue) > 0)
			{
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			}
			if (_uBoundActive && Comparer.Compare(_max, upperValue) < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			}
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(SumWalkPredicate action)
		{
			VersionCheck();
			if (root == null)
			{
				return true;
			}
			// The maximum height of a red-black tree is 2*lg(n+1).
			// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
			Stack<Node> stack = new(2 * Log2(_size + 1)); // this is not exactly right if count is out of date, but the stack can grow
			Node? current = root;
			while (current != null)
			{
				if (IsWithinRange(current.Left?.LeavesCount ?? 0))
				{
					stack.Push(current);
					current = current.Left;
				}
				else if (_lBoundActive && Comparer.Compare(_min, current.Left?.LeavesCount ?? 0) > 0)
					current = current.Right;
				else
					current = current.Left;
			}
			while (stack.Length != 0)
			{
				current = stack.Pop();
				if (!action(current))
				{
					return false;
				}
				Node? node = current.Right;
				while (node != null)
				{
					if (IsWithinRange(node.Left?.LeavesCount ?? 0))
					{
						stack.Push(node);
						node = node.Left;
					}
					else if (_lBoundActive && Comparer.Compare(_min, node.Left?.LeavesCount ?? 0) > 0)
						node = node.Right;
					else
						node = node.Left;
				}
			}
			return true;
		}

		public override SumList Insert(int index, int value)
		{
			if (!IsWithinRange(index))
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}
			var ret = _underlying.Insert(index, value);
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return ret;
		}

		internal override bool IsWithinRange(int index)
		{
			int comp = _lBoundActive ? Comparer.Compare(_min, index) : -1;
			if (comp > 0)
			{
				return false;
			}
			comp = _uBoundActive ? Comparer.Compare(_max, index) : 1;
			return comp >= 0;
		}

		/// <summary>
		/// Returns the number of elements <c>count</c> of the parent list.
		/// </summary>
		internal override int TotalCount()
		{
			Debug.Assert(_underlying != null);
			return _underlying.Length;
		}

		/// <summary>
		/// Checks whether this subset is out of date, and updates it if necessary.
		/// </summary>
		/// <param name="updateCount">Updates the count variable if necessary.</param>
		internal override void VersionCheck(bool updateCount = false) => VersionCheckImpl(updateCount);

		private void VersionCheckImpl(bool updateCount)
		{
			Debug.Assert(_underlying != null);
			if (version != _underlying.version)
			{
				root = _underlying.FindRange(_min, _max, _lBoundActive, _uBoundActive);
				version = _underlying.version;
			}
			if (updateCount && _countVersion != _underlying.version)
			{
				_size = 0;
				InOrderTreeWalk(n => { _size++; return true; });
				_countVersion = _underlying.version;
			}
		}

#if DEBUG
		internal override bool VersionUpToDate() => version == _underlying.version;
#endif
	}
}

internal delegate bool BigSumWalkPredicate(BigSumList.Node node);

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public partial class BigSumList : ListBase<mpz_t, BigSumList>
{
	private Node? root;
	private int version;

	public BigSumList()
	{
	}

	public BigSumList(IEnumerable<mpz_t> collection) : this()
	{
		ArgumentNullException.ThrowIfNull(collection);
		// These are explicit type checks in the mold of HashSet. It would have worked better with
		// something like an ISorted interface. (We could make this work for SortedList.Keys, etc.)
		if (collection is BigSumList sumList && sumList is not TreeSubSet)
		{
			if (sumList.Length > 0)
			{
				Debug.Assert(sumList.root != null);
				_size = sumList._size;
				root = sumList.root.DeepClone(_size);
			}
			return;
		}
		var elements = collection.ToArray();
		int count = elements.Length;
		if (count > 0)
		{
			root = ConstructRootFromSortedArray(elements, 0, count - 1, null);
			_size = count;
		}
	}

	public BigSumList(params mpz_t[] array) : this((IEnumerable<mpz_t>)array)
	{
	}

	public BigSumList(ReadOnlySpan<mpz_t> span) : this((IEnumerable<mpz_t>)span.ToArray())
	{
	}

	public override int Capacity
	{
		get => _size;
		set
		{
		}
	}

	private protected override Func<int, BigSumList> CapacityCreator => x => new();

	private protected override Func<IEnumerable<mpz_t>, BigSumList> CollectionCreator => x => new(x);

	private protected virtual IComparer<int> Comparer => G.Comparer<int>.Default;

	public override int Length
	{
		get
		{
			VersionCheck(updateCount: true);
			return _size;
		}
	}

	public virtual int Max => MaxInternal;

	internal virtual int MaxInternal => _size - 1;

	public virtual int Min => MinInternal;

	internal virtual int MinInternal => 0;

	public override BigSumList Add(mpz_t value) => Insert(_size, value);

	public override Span<mpz_t> AsSpan(int index, int count) => throw new NotSupportedException();

	/// <summary>
	/// Does a left-to-right breadth-first tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool BreadthFirstTreeWalk(BigSumWalkPredicate action)
	{
		if (root == null)
			return true;
		var processQueue = new Queue<Node>();
		processQueue.Enqueue(root);
		Node current;
		while (processQueue.Length != 0)
		{
			current = processQueue.Dequeue();
			if (!action(current))
				return false;
			if (current.Left != null)
				processQueue.Enqueue(current.Left);
			if (current.Right != null)
				processQueue.Enqueue(current.Right);
		}
		return true;
	}

	public override void Clear()
	{
		root = null;
		_size = 0;
		++version;
	}

	private protected override void ClearInternal(int index, int count) => new TreeSubSet(this, index, index + count - 1, true, true).Clear();

	private static Node? ConstructRootFromSortedArray(mpz_t[] arr, int startIndex, int endIndex, Node? redNode)
	{
		// You're given a sorted array... say 1 2 3 4 5 6
		// There are 2 cases:
		// -  If there are odd # of elements, pick the middle element (in this case 4), and compute
		//    its left and right branches
		// -  If there are even # of elements, pick the left middle element, save the right middle element
		//    and call the function on the rest
		//    1 2 3 4 5 6 -> pick 3, save 4 and call the fn on 1,2 and 5,6
		//    now add 4 as a red node to the lowest element on the right branch
		//             3                       3
		//         1       5       ->     1        5
		//           2       6             2     4   6
		//    As we're adding to the leftmost of the right branch, nesting will not hurt the red-black properties
		//    Leaf nodes are red if they have no sibling (if there are 2 nodes or if a node trickles
		//    down to the bottom

		// This is done recursively because the iterative way to do this ends up wasting more space than it saves in stack frames
		// Only some base cases are handled below.
		int size = endIndex - startIndex + 1;
		Node root;
		switch (size)
		{
			case 0:
				return null;
			case 1:
				root = new Node(arr[startIndex], NodeColor.Black);
				if (redNode != null)
					root.Left = redNode;
				break;
			case 2:
				root = new Node(arr[startIndex], NodeColor.Black)
				{
					Right = new Node(arr[endIndex], NodeColor.Black)
				};
				root.Right.ColorRed();
				if (redNode != null)
					root.Left = redNode;
				break;
			case 3:
				root = new Node(arr[startIndex + 1], NodeColor.Black)
				{
					Left = new Node(arr[startIndex], NodeColor.Black),
					Right = new Node(arr[endIndex], NodeColor.Black)
				};
				if (redNode != null)
					root.Left.Left = redNode;
				break;
			default:
				int midpt = (startIndex + endIndex) / 2;
				root = new Node(arr[midpt], NodeColor.Black)
				{
					Left = ConstructRootFromSortedArray(arr, startIndex, midpt - 1, redNode),
					Right = size % 2 == 0 ?
					ConstructRootFromSortedArray(arr, midpt + 2, endIndex, new Node(arr[midpt + 1], NodeColor.Red)) :
					ConstructRootFromSortedArray(arr, midpt + 1, endIndex, null)
				};
				break;
		}
		return root;
	}

	private protected override void Copy(BigSumList source, int sourceIndex, BigSumList destination, int destinationIndex, int count)
	{
		if (count == 0)
			return;
		if (count == 1)
		{
			destination.SetInternal(destinationIndex, source.GetInternal(sourceIndex));
			return;
		}
		TreeSubSet subset = new(source as BigSumList ?? throw new ArgumentException(null, nameof(source)), sourceIndex, sourceIndex + count - 1, true, true);
		var en = subset.GetEnumerator();
		if (destinationIndex < destination._size)
			new TreeSubSet(destination, destinationIndex, Min(destinationIndex + count, destination._size) - 1, true, true).InOrderTreeWalk(node =>
			{
				bool b = en.MoveNext();
				if (b)
					node.Value = en.Current;
				return b;
			});
		while (en.MoveNext())
			destination.Add(en.Current);
	}

	private protected override void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is mpz_t[] array2)
			CopyToInternal(0, array2, arrayIndex, _size);
		else
			throw new ArgumentException(null, nameof(array));
	}

	private protected override void CopyToInternal(int index, mpz_t[] array, int arrayIndex, int count)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count));
		if (count > array.Length - index)
			throw new ArgumentException(null);
		count += index; // Make `count` the upper bound.
		int i = 0;
		InOrderTreeWalk(node =>
		{
			if (i >= count)
				return false;
			if (i++ < index)
				return true;
			array[arrayIndex++] = node.Value;
			return true;
		});
	}

	public virtual bool Decrease(int index) => Update(index, GetInternal(index) - 1);

	public override void Dispose()
	{
		root = null;
		_size = 0;
		version = 0;
		GC.SuppressFinalize(this);
	}

	private void FindForRemove(int index2, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch)
	{
		// Search for a node and then find its successor.
		// Then copy the value from the successor to the matching node, and delete the successor.
		// If a node doesn't have a successor, we can replace it with its left child (if not empty),
		// or delete the matching node.
		//
		// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
		// Following code will make sure the node on the path is not a 2-node.

		// Even if we don't actually remove from the list, we may be altering its structure (by doing rotations
		// and such). So update our version to disable any enumerators/subsets working on it.
		version++;
		Node? current = root;
		parent = null;
		grandParent = null;
		match = null;
		parentOfMatch = null;
		bool foundMatch = false;
		while (current != null)
		{
			if (current.Is2Node)
			{
				// Fix up 2-node
				if (parent == null)
					current.ColorRed();
				else if (parent.Left != null && parent.Right != null)
				{
					Node sibling = parent.GetSibling(current);
					if (sibling.IsRed)
					{
						// If parent is a 3-node, flip the orientation of the red link.
						// We can achieve this by a single rotation.
						// This case is converted to one of the other cases below.
						Debug.Assert(parent.IsBlack);
						if (parent.Right == sibling)
							parent.RotateLeft();
						else
							parent.RotateRight();
						parent.ColorRed();
						sibling.ColorBlack(); // The red parent can't have black children.
											  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
						ReplaceChildOrRoot(grandParent, parent, sibling);
						// `sibling` will become the grandparent of `current`.
						grandParent = sibling;
						if (parent == match)
							parentOfMatch = sibling;
						sibling = parent.GetSibling(current);
					}
					Debug.Assert(Node.IsNonNullBlack(sibling));
					if (sibling.Is2Node)
						parent.Merge2Nodes();
					else
					{
						// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
						// We can change the color of `current` to red by some rotation.
						Node newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;
						newGrandParent.Color = parent.Color;
						parent.ColorBlack();
						current.ColorRed();
						ReplaceChildOrRoot(grandParent, parent, newGrandParent);
						if (parent == match)
							parentOfMatch = newGrandParent;
					}
				}
			}
			grandParent = parent;
			parent = current;
			if (foundMatch)
				current = current.Left;
			else if ((current.Left?.LeavesCount ?? 0) == index2)
			{
				// Save the matching node.
				foundMatch = true;
				match = current;
				parentOfMatch = grandParent;
				current = current.Right;
			}
			else if (current.Left == null)
			{
				index2--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount >= index2)
				current = current.Left;
			else
			{
				index2 -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
	}

	internal virtual Node? FindNode(int index)
	{
		Node? current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
				return current;
			else if (current.Left == null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount > index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		return null;
	}

	internal Node? FindRange(int from, int to) => FindRange(from, to, true, true);

	internal Node? FindRange(int from, int to, bool lowerBoundActive, bool upperBoundActive)
	{
		Node? current = root;
		while (current != null)
		{
			if (lowerBoundActive && Comparer.Compare(from, current.Left?.LeavesCount ?? 0) > 0)
				current = current.Right;
			else if (upperBoundActive && Comparer.Compare(to, current.Left?.LeavesCount ?? 0) < 0)
				current = current.Left;
			else
				return current;
		}
		return null;
	}

	public virtual int GetAndRemove(Index index)
	{
		int index2 = index.IsFromEnd ? _size - index.Value : index.Value;
		if (root == null)
		{
			return default!;
		}
		FindForRemove(index2, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch);
		int found = default!;
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			found = match.Left?.LeavesCount ?? 0;
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
		root?.ColorBlack();
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return found;
	}

	public override IEnumerator<mpz_t> GetEnumerator() => new Enumerator(this);

	internal override mpz_t GetInternal(int index, bool invoke = true)
	{
		Node? current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				var value = current.Value;
				if (invoke)
					Changed();
				return value;
			}
			else if (current.Left == null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount > index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	public virtual mpz_t GetLeftValuesSum(int index, out mpz_t actualValue)
	{
		Node? current = root;
		mpz_t sum = 0;
		while (current != null)
		{
			int order = Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				actualValue = current.Value;
				return sum + (current.Left?.ValuesSum ?? 0);
			}
			else if (order < 0)
			{
				current = current.Left;
			}
			else
			{
				index -= (current.Left?.LeavesCount ?? 0) + 1;
				sum += (current.Left?.ValuesSum ?? 0) + current.Value;
				current = current.Right;
			}
		}
		actualValue = 0;
		return sum;
	}

	public virtual BigSumList GetViewBetween(int lowerValue, int upperValue)
	{
		if (Comparer.Compare(lowerValue, upperValue) > 0)
			throw new ArgumentException(null, nameof(lowerValue));
		return new TreeSubSet(this, lowerValue, upperValue, true, true);
	}

	public virtual bool Increase(int index) => Update(index, GetInternal(index) + 1);

	private protected override int IndexOfInternal(mpz_t value, int index, int count) => throw new NotSupportedException();

	/// <summary>
	/// Does an in-order tree walk and calls the delegate for each node.
	/// </summary>
	/// <param name="action">
	/// The delegate to invoke on each node.
	/// If the delegate returns <c>false</c>, the walk is stopped.
	/// </param>
	/// <returns><c>true</c> if the entire tree has been walked; otherwise, <c>false</c>.</returns>
	internal virtual bool InOrderTreeWalk(BigSumWalkPredicate action)
	{
		if (root == null)
			return true;
		// The maximum height of a red-black tree is 2 * log2(n+1).
		// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
		// Note: It's not strictly necessary to provide the stack capacity, but we don't
		// want the stack to unnecessarily allocate arrays as it grows.
		var stack = new Stack<Node>(2 * Log2(Length + 1));
		Node? current = root;
		while (current != null)
		{
			stack.Push(current);
			current = current.Left;
		}
		while (stack.Length != 0)
		{
			current = stack.Pop();
			if (!action(current))
				return false;
			Node? node = current.Right;
			while (node != null)
			{
				stack.Push(node);
				node = node.Left;
			}
		}
		return true;
	}

	public override BigSumList Insert(int index, mpz_t value)
	{
		if (root == null)
		{
			// The tree is empty and this is the first value.
			root = new Node(value, NodeColor.Black);
			_size = 1;
			version++;
			return this;
		}
		// Search for a node at bottom to insert the new node.
		// If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
		// We split 4-nodes along the search path.
		Node? current = root;
		Node? parent = null;
		Node? grandParent = null;
		Node? greatGrandParent = null;
		// Even if we don't actually add to the list, we may be altering its structure (by doing rotations and such).
		// So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
		version++;
		int oldIndex = index;
		int order = 0;
		bool foundMatch = false;
		while (current != null)
		{
			order = foundMatch ? 1 : Comparer.Compare(index, current.Left?.LeavesCount ?? 0);
			if (order == 0)
			{
				// We could have changed root node to red during the search process.
				// We need to set it to black before we return.
				root.ColorBlack();
				foundMatch = true;
			}
			// Split a 4-node into two 2-nodes.
			if (current.Is4Node)
			{
				current.Split4Node();
				// We could have introduced two consecutive red nodes after split. Fix that by rotation.
				if (Node.IsNonNullRed(parent))
					InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
				index = oldIndex;
				current = root;
				greatGrandParent = grandParent = parent = null;
				foundMatch = false;
				continue;
			}
			//if (current.Is2Node)
			//{
			//	// Fix up 2-node
			//	if (parent == null)
			//		current.ColorRed();
			//	else if (parent.Left != null && parent.Right != null)
			//	{
			//		Node sibling = parent.GetSibling(current);
			//		if (sibling.IsRed)
			//		{
			//			// If parent is a 3-node, flip the orientation of the red link.
			//			// We can achieve this by a single rotation.
			//			// This case is converted to one of the other cases below.
			//			Debug.Assert(parent.IsBlack);
			//			if (parent.Right == sibling)
			//				parent.RotateLeft();
			//			else
			//				parent.RotateRight();
			//			parent.ColorRed();
			//			sibling.ColorBlack(); // The red parent can't have black children.
			//								  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
			//			ReplaceChildOrRoot(grandParent, parent, sibling);
			//			// `sibling` will become the grandparent of `current`.
			//			grandParent = sibling;
			//			sibling = parent.GetSibling(current);
			//		}
			//		Debug.Assert(Node.IsNonNullBlack(sibling));
			//		if (sibling.Is2Node)
			//			parent.Merge2Nodes();
			//		else
			//		{
			//			// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
			//			// We can change the color of `current` to red by some rotation.
			//			Node newGrandParent = parent.Rotate(parent.GetRotation(current, sibling))!;
			//			newGrandParent.Color = parent.Color;
			//			parent.ColorBlack();
			//			current.ColorRed();
			//			ReplaceChildOrRoot(grandParent, parent, newGrandParent);
			//		}
			//	}
			//}
			greatGrandParent = grandParent;
			grandParent = parent;
			parent = current;
			if (order <= 0)
				current = current.Left;
			else
			{
				index -= (current.Left?.LeavesCount ?? 0) + 1;
				current = current.Right;
			}
		}
#if DEBUG
		if (index != 0)
			throw new InvalidOperationException();
#endif
		Debug.Assert(parent != null);
		// We're ready to insert the new node.
		Node node = new(value, NodeColor.Red);
		if (order <= 0)
			parent.Left = node;
		else
			parent.Right = node;
		// The new node will be red, so we will need to adjust colors if its parent is also red.
		if (parent.IsRed)
			InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
#if DEBUG
		if (_size + 1 != root.LeavesCount)
			throw new InvalidOperationException();
		foreach (var x in new[] { node, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
		// The root node is always black.
		root.ColorBlack();
		++_size;
		return this;
	}

	// After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
	// It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
	// need to split again in the next node.
	// By the time we need to split again, everything will be correctly set.
	private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
	{
		Debug.Assert(parent != null);
		Debug.Assert(grandParent != null);
		bool parentIsOnRight = grandParent.Right == parent;
		bool currentIsOnRight = parent.Right == current;
		Node newChildOfGreatGrandParent;
		if (parentIsOnRight == currentIsOnRight)
			newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft() : grandParent.RotateRight();
		else
		{
			// Different orientation, double rotation
			newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft();
			// Current node now becomes the child of `greatGrandParent`
			parent = greatGrandParent;
		}
		// `grandParent` will become a child of either `parent` of `current`.
		grandParent.ColorRed();
		newChildOfGreatGrandParent.ColorBlack();
		ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
#if DEBUG
		foreach (var x in new[] { current, parent, grandParent, greatGrandParent })
			x?.Verify();
#endif
	}

	// Virtual function for TreeSubSet, which may need to do range checks.
	internal virtual bool IsWithinRange(int index) => true;

	private protected override int LastIndexOfInternal(mpz_t value, int index, int count) => throw new NotSupportedException();

	// Used for set checking operations (using enumerables) that rely on counting
	private static int Log2(int value) => BitOperations.Log2((uint)value);

	public override BigSumList RemoveAt(int index)
	{
		if (root == null)
		{
			return this;
		}
		FindForRemove(index, out Node? parent, out Node? grandParent, out Node? match, out Node? parentOfMatch);
		// Move successor to the matching node position and replace links.
		if (match != null)
		{
			ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			--_size;
		}
		root?.ColorBlack();
#if DEBUG
		if (_size != (root?.LeavesCount ?? 0))
			throw new InvalidOperationException();
		foreach (var x in new[] { match, parentOfMatch, parent, grandParent })
			x?.Verify();
#endif
		return this;
	}

	/// <summary>
	/// Replaces the child of a parent node, or replaces the root if the parent is <c>null</c>.
	/// </summary>
	/// <param name="parent">The (possibly <c>null</c>) parent.</param>
	/// <param name="child">The child node to replace.</param>
	/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
	private void ReplaceChildOrRoot(Node? parent, Node child, Node newChild)
	{
		if (parent != null)
			parent.ReplaceChild(child, newChild);
		else
		{
			root = newChild;
			root?.Isolate();
		}
	}

	/// <summary>
	/// Replaces the matching node with its successor.
	/// </summary>
	private void ReplaceNode(Node match, Node parentOfMatch, Node successor, Node parentOfSuccessor)
	{
		Debug.Assert(match != null);
		if (successor == match)
		{
			// This node has no successor. This can only happen if the right child of the match is null.
			Debug.Assert(match.Right == null);
			successor = match.Left!;
		}
		else
		{
			Debug.Assert(parentOfSuccessor != null);
			Debug.Assert(successor.Left == null);
			Debug.Assert((successor.Right == null && successor.IsRed) || (successor.Right!.IsRed && successor.IsBlack));
			successor.Right?.ColorBlack();
			if (parentOfSuccessor != match)
			{
				// Detach the successor from its parent and set its right child.
				parentOfSuccessor.Left = successor.Right;
				successor.Right = match.Right;
				parentOfSuccessor.FixUp();
			}
			successor.Left = match.Left;
		}
		if (successor != null)
			successor.Color = match.Color;
		ReplaceChildOrRoot(parentOfMatch, match, successor!);
#if DEBUG
		foreach (var x in new[] { match, parentOfMatch, successor, parentOfSuccessor })
			x?.Verify();
#endif
	}

	private protected override BigSumList ReverseInternal(int index, int count) => throw new NotSupportedException();

	internal override void SetInternal(int index, mpz_t value)
	{
		Node? current = root;
		while (current != null)
		{
			if ((current.Left?.LeavesCount ?? 0) == index)
			{
				current.Value = value;
				Changed();
				return;
			}
			else if (current.Left == null)
			{
				index--;
				current = current.Right;
			}
			else if (current.Left.LeavesCount >= index)
				current = current.Left;
			else
			{
				index -= current.Left.LeavesCount + 1;
				current = current.Right;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	// Virtual function for TreeSubSet, which may need the count variable of the parent list.
	internal virtual int TotalCount() => Length;

	public virtual bool Update(int index, mpz_t value)
	{
		var node = FindNode(index);
		if (node != null)
		{
			node.Update(value);
#if DEBUG
			foreach (var x in new[] { node, root })
				x?.Verify();
#endif
			return true;
		}
		else
			return false;
	}

	internal void UpdateVersion() => ++version;

	// Virtual function for TreeSubSet, which may need to update its count.
	internal virtual void VersionCheck(bool updateCount = false) { }

#if DEBUG
	/// <summary>
	/// debug status to be checked whenever any operation is called
	/// </summary>
	/// <returns></returns>
	internal virtual bool VersionUpToDate() => true;
#endif

	[DebuggerDisplay("{Value.ToString()}, Left = {Left?.Value.ToString()}, Right = {Right?.Value.ToString()}, Parent = {Parent?.Value.ToString()}")]
	internal sealed class Node
	{
		private Node? _left;
		private Node? _right;
		private Node? Parent { get; set; }
		private int _leavesCount;
		private mpz_t _valuesSum;

		public Node(mpz_t value, NodeColor color)
		{
			Value = value;
			Color = color;
			_leavesCount = 1;
			_valuesSum = value;
		}

		public mpz_t Value { get; set; }

		internal Node? Left
		{
			get => _left;
			set
			{
				if (_left == value)
					return;
				if (_left != null && _left.Parent != value)
					_left.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_left?.LeavesCount ?? 0);
				ValuesSum += (value?.ValuesSum ?? 0) - (_left?.ValuesSum ?? 0);
				_left = value;
				if (_left != null)
					_left.Parent = this;
#if DEBUG
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
#endif
			}
		}

		internal Node? Right
		{
			get => _right;
			set
			{
				if (_right == value)
					return;
				if (_right != null && _right.Parent != value)
					_right.Parent = null;
				LeavesCount += (value?.LeavesCount ?? 0) - (_right?.LeavesCount ?? 0);
				ValuesSum += (value?.ValuesSum ?? 0) - (_right?.ValuesSum ?? 0);
				_right = value;
				if (_right != null)
					_right.Parent = this;
#if DEBUG
				foreach (var x in new[] { this, _left, _right, Parent })
					x?.Verify();
#endif
			}
		}

		internal int LeavesCount
		{
			get => _leavesCount;
			set
			{
				if (Parent != null)
					Parent.LeavesCount += value - _leavesCount;
				_leavesCount = value;
				if (Parent != null && Parent.LeavesCount != (Parent._left?.LeavesCount ?? 0) + (Parent._right?.LeavesCount ?? 0) + 1)
					throw new InvalidOperationException();
			}
		}

		internal mpz_t ValuesSum
		{
			get => _valuesSum;
			set
			{
				if (Parent != null)
					Parent.ValuesSum += value - _valuesSum;
				_valuesSum = value;
				if (Parent != null && Parent.ValuesSum != (Parent._left?.ValuesSum ?? 0) + (Parent._right?.ValuesSum ?? 0) + Parent.Value)
					throw new InvalidOperationException();
			}
		}

		public NodeColor Color { get; set; }

		public bool IsBlack => Color == NodeColor.Black;

		public bool IsRed => Color == NodeColor.Red;

		public bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

		public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

		public void ColorBlack() => Color = NodeColor.Black;

		public void ColorRed() => Color = NodeColor.Red;

		public Node DeepClone(int count)
		{
#if DEBUG
			Debug.Assert(count == GetCount());
#endif
			Node newRoot = ShallowClone();
			var pendingNodes = new Stack<(Node source, Node target)>(2 * Log2(count) + 2);
			pendingNodes.Push((this, newRoot));
			while (pendingNodes.TryPop(out var next))
			{
				Node clonedNode;
				if (next.source.Left is Node left)
				{
					clonedNode = left.ShallowClone();
					next.target.Left = clonedNode;
					pendingNodes.Push((left, clonedNode));
				}
				if (next.source.Right is Node right)
				{
					clonedNode = right.ShallowClone();
					next.target.Right = clonedNode;
					pendingNodes.Push((right, clonedNode));
				}
			}
			return newRoot;
		}

		public void FixUp()
		{
			if (Left != null)
				Left.Parent = this;
			if (Right != null)
				Right.Parent = this;
		}

		/// <summary>
		/// Gets the rotation this node should undergo during a removal.
		/// </summary>
		public TreeRotation GetRotation(Node current, Node sibling)
		{
			Debug.Assert(IsNonNullRed(sibling.Left) || IsNonNullRed(sibling.Right));
#if DEBUG
			Debug.Assert(HasChildren(current, sibling));
#endif

			bool currentIsLeftChild = Left == current;
			return IsNonNullRed(sibling.Left) ?
				(currentIsLeftChild ? TreeRotation.RightLeft : TreeRotation.Right) :
				(currentIsLeftChild ? TreeRotation.Left : TreeRotation.LeftRight);
		}

		/// <summary>
		/// Gets the sibling of one of this node's children.
		/// </summary>
		public Node GetSibling(Node node)
		{
			Debug.Assert(node != null);
			Debug.Assert(node == Left ^ node == Right);
			return node == Left ? Right! : Left!;
		}

		public static bool IsNonNullBlack(Node? node) => node != null && node.IsBlack;

		public static bool IsNonNullRed(Node? node) => node != null && node.IsRed;

		public static bool IsNullOrBlack(Node? node) => node == null || node.IsBlack;

		public void Isolate()
		{
			if (Parent != null && Parent.Left == this)
				Parent.Left = null;
			if (Parent != null && Parent.Right == this)
				Parent.Right = null;
		}

		/// <summary>
		/// Combines two 2-nodes into a 4-node.
		/// </summary>
		public void Merge2Nodes()
		{
			Debug.Assert(IsRed);
			Debug.Assert(Left!.Is2Node);
			Debug.Assert(Right!.Is2Node);
			// Combine two 2-nodes into a 4-node.
			ColorBlack();
			Left.ColorRed();
			Right.ColorRed();
		}

		/// <summary>
		/// Replaces a child of this node with a new node.
		/// </summary>
		/// <param name="child">The child to replace.</param>
		/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
		public void ReplaceChild(Node child, Node newChild)
		{
			if (Left == child)
				Left = newChild;
			else if (Right == child)
				Right = newChild;
		}

		/// <summary>
		/// Does a rotation on this tree. May change the color of a grandchild from red to black.
		/// </summary>
		public Node? Rotate(TreeRotation rotation)
		{
			Node removeRed;
			switch (rotation)
			{
				case TreeRotation.Right:
					removeRed = Left!.Left!;
					Debug.Assert(removeRed.IsRed);
					removeRed.ColorBlack();
					return RotateRight();
				case TreeRotation.Left:
					removeRed = Right!.Right!;
					Debug.Assert(removeRed.IsRed);
					removeRed.ColorBlack();
					return RotateLeft();
				case TreeRotation.RightLeft:
					Debug.Assert(Right!.Left!.IsRed);
					return RotateRightLeft();
				case TreeRotation.LeftRight:
					Debug.Assert(Left!.Right!.IsRed);
					return RotateLeftRight();
				default:
					Debug.Fail($"{nameof(rotation)}: {rotation} is not a defined {nameof(TreeRotation)} value.");
					return null;
			}
		}

		/// <summary>
		/// Does a left rotation on this tree, making this this the new left child of the current right child.
		/// </summary>
		public Node RotateLeft()
		{
			Node child = Right!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Right = child.Left;
			child.Left = this;
			if (parent != null)
			{
				if (isRight)
					parent.Right = child;
				else
					parent.Left = child;
			}
			return child;
		}

		/// <summary>
		/// Does a left-right rotation on this tree. The left child is rotated left, then this this is rotated right.
		/// </summary>
		public Node RotateLeftRight()
		{
			Node child = Left!;
			Node grandChild = child.Right!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Left = grandChild.Right;
			grandChild.Right = this;
			child.Right = grandChild.Left;
			grandChild.Left = child;
			if (parent != null)
			{
				if (isRight)
					parent.Right = grandChild;
				else
					parent.Left = grandChild;
			}
			return grandChild;
		}

		/// <summary>
		/// Does a right rotation on this tree, making this this the new right child of the current left child.
		/// </summary>
		public Node RotateRight()
		{
			Node child = Left!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Left = child.Right;
			child.Right = this;
			if (parent != null)
			{
				if (isRight)
					parent.Right = child;
				else
					parent.Left = child;
			}
			return child;
		}

		/// <summary>
		/// Does a right-left rotation on this tree. The right child is rotated right, then this this is rotated left.
		/// </summary>
		public Node RotateRightLeft()
		{
			Node child = Right!;
			Node grandChild = child.Left!;
			var parent = Parent;
			bool isRight = parent != null && (parent.Right == this || (parent.Left == this ? false : throw new InvalidOperationException()));
			Right = grandChild.Left;
			grandChild.Left = this;
			child.Left = grandChild.Right;
			grandChild.Right = child;
			if (parent != null)
			{
				if (isRight)
					parent.Right = grandChild;
				else
					parent.Left = grandChild;
			}
			return grandChild;
		}

		public Node ShallowClone() => new(Value, Color);

		public void Split4Node()
		{
			Debug.Assert(Left != null);
			Debug.Assert(Right != null);
			ColorRed();
			Left.ColorBlack();
			Right.ColorBlack();
		}

		public void Update(mpz_t value)
		{
			ValuesSum += value - Value;
			Value = value;
		}

#if DEBUG
		private int GetCount() => 1 + (Left?.GetCount() ?? 0) + (Right?.GetCount() ?? 0);

		private bool HasChild(Node child) => child == Left || child == Right;

		private bool HasChildren(Node child1, Node child2)
		{
			Debug.Assert(child1 != child2);
			return (Left == child1 && Right == child2)
				|| (Left == child2 && Right == child1);
		}

		internal void Verify()
		{
			if (Right != null && Right == Left)
				throw new InvalidOperationException();
			if (LeavesCount != (Left?.LeavesCount ?? 0) + (Right?.LeavesCount ?? 0) + 1)
				throw new InvalidOperationException();
			if (ValuesSum != (Left?.ValuesSum ?? 0) + (Right?.ValuesSum ?? 0) + Value)
				throw new InvalidOperationException();
			if (Left != null && Left.Parent == null)
				throw new InvalidOperationException();
			if (Right != null && Right.Parent == null)
				throw new InvalidOperationException();
		}
#endif
	}

	public new struct Enumerator : IEnumerator<mpz_t>
	{
		private readonly BigSumList _tree;
		private readonly int _version;

		private readonly Stack<Node> _stack;
		private Node? _current;

		private readonly bool _reverse;

		internal Enumerator(BigSumList list) : this(list, reverse: false)
		{
		}

		internal Enumerator(BigSumList list, bool reverse)
		{
			_tree = list;
			list.VersionCheck();
			_version = list.version;
			// 2 log(n + 1) is the maximum height.
			_stack = new Stack<Node>(2 * Log2(list.TotalCount() + 1));
			_current = null;
			_reverse = reverse;
			Initialize();
		}

		public mpz_t Current
		{
			get
			{
				if (_current != null)
				{
					return _current.Value;
				}
				return default!; // Should only happen when accessing Current is undefined behavior
			}
		}

		object? IEnumerator.Current
		{
			get
			{
				if (_current == null)
				{
					throw new InvalidOperationException();
				}
				return _current.Value;
			}
		}

		internal bool NotStartedOrEnded => _current == null;

		public void Dispose() { }

		private void Initialize()
		{
			_current = null;
			Node? node = _tree.root;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_tree.IsWithinRange(node.Left?.LeavesCount ?? 0))
				{
					_stack.Push(node);
					node = next;
				}
				else if (next == null || !_tree.IsWithinRange(next.Left?.LeavesCount ?? 0))
					node = other;
				else
					node = next;
			}
		}

		public bool MoveNext()
		{
			// Make sure that the underlying subset has not been changed since
			_tree.VersionCheck();
			if (_version != _tree.version)
			{
				throw new InvalidOperationException();
			}
			if (_stack.Length == 0)
			{
				_current = null;
				return false;
			}
			_current = _stack.Pop();
			Node? node = _reverse ? _current.Left : _current.Right;
			Node? next, other;
			while (node != null)
			{
				next = _reverse ? node.Right : node.Left;
				other = _reverse ? node.Left : node.Right;
				if (_tree.IsWithinRange(node.Left?.LeavesCount ?? 0))
				{
					_stack.Push(node);
					node = next;
				}
				else if (other == null || !_tree.IsWithinRange(other.Left?.LeavesCount ?? 0))
					node = next;
				else
					node = other;
			}
			return true;
		}

		internal void Reset()
		{
			if (_version != _tree.version)
			{
				throw new InvalidOperationException();
			}
			_stack.Clear();
			Initialize();
		}

		void IEnumerator.Reset() => Reset();
	}

	internal sealed class TreeSubSet : BigSumList
	{
		private readonly BigSumList _underlying;
		private readonly int _min;
		private readonly int _max;
		// keeps track of whether the count variable is up to date
		// up to date -> _countVersion = _underlying.version
		// not up to date -> _countVersion < _underlying.version
		private int _countVersion;
		// these exist for unbounded collections
		// for instance, you could allow this subset to be defined for i > 10. The list will throw if
		// anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
		// in the spec, and are not available, but the framework is there to make them available at some point.
		private readonly bool _lBoundActive, _uBoundActive;
		// used to see if the count is out of date

		public TreeSubSet(BigSumList Underlying, int Min, int Max, bool lowerBoundActive, bool upperBoundActive) : base()
		{
			_underlying = Underlying;
			_min = Min;
			_max = Max;
			_lBoundActive = lowerBoundActive;
			_uBoundActive = upperBoundActive;
			root = _underlying.FindRange(_min, _max, _lBoundActive, _uBoundActive); // root is first element within range
			_size = 0;
			version = -1;
			_countVersion = -1;
		}

		internal override int MaxInternal
		{
			get
			{
				VersionCheck();
				Node? current = root;
				int result = default;
				while (current != null)
				{
					int comp = _uBoundActive ? Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) : 1;
					if (comp < 0)
						current = current.Left;
					else
					{
						result = current.Left?.LeavesCount ?? 0;
						if (comp == 0)
						{
							break;
						}
						current = current.Right;
					}
				}
				return result!;
			}
		}

		internal override int MinInternal
		{
			get
			{
				VersionCheck();
				Node? current = root;
				int result = default;
				while (current != null)
				{
					int comp = _lBoundActive ? Comparer.Compare(_min, current.Left?.LeavesCount ?? 0) : -1;
					if (comp > 0)
						current = current.Right;
					else
					{
						result = current.Left?.LeavesCount ?? 0;
						if (comp == 0)
						{
							break;
						}
						current = current.Left;
					}
				}
				return result!;
			}
		}

		internal override bool BreadthFirstTreeWalk(BigSumWalkPredicate action)
		{
			VersionCheck();
			if (root == null)
			{
				return true;
			}
			Queue<Node> processQueue = new();
			processQueue.Enqueue(root);
			Node current;
			while (processQueue.Length != 0)
			{
				current = processQueue.Dequeue();
				if (IsWithinRange(current.Left?.LeavesCount ?? 0) && !action(current))
				{
					return false;
				}
				if (current.Left != null && (!_lBoundActive || Comparer.Compare(_min, current.Left.LeavesCount) < 0))
					processQueue.Enqueue(current.Left);
				if (current.Right != null && (!_uBoundActive || Comparer.Compare(_max, current.Left?.LeavesCount ?? 0) > 0))
					processQueue.Enqueue(current.Right);
			}
			return true;
		}

		public override void Clear()
		{
			if (Length == 0)
			{
				return;
			}
			List<int> toRemove = new();
			BreadthFirstTreeWalk(n => { toRemove.Add(n.Left?.LeavesCount ?? 0); return true; });
			while (toRemove.Length != 0)
			{
				_underlying.RemoveAt(toRemove[^1]);
				toRemove.RemoveAt(toRemove.Length - 1);
			}
			root = null;
			_size = 0;
			version = _underlying.version;
		}

		internal override Node? FindNode(int index)
		{
			if (!IsWithinRange(index))
			{
				return null;
			}
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return base.FindNode(index);
		}

		// This passes functionality down to the underlying tree, clipping edges if necessary
		// There's nothing gained by having a nested subset. May as well draw it from the base
		// Cannot increase the bounds of the subset, can only decrease it
		public override BigSumList GetViewBetween(int lowerValue, int upperValue)
		{
			if (_lBoundActive && Comparer.Compare(_min, lowerValue) > 0)
			{
				throw new ArgumentOutOfRangeException(nameof(lowerValue));
			}
			if (_uBoundActive && Comparer.Compare(_max, upperValue) < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(upperValue));
			}
			return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
		}

		internal override bool InOrderTreeWalk(BigSumWalkPredicate action)
		{
			VersionCheck();
			if (root == null)
			{
				return true;
			}
			// The maximum height of a red-black tree is 2*lg(n+1).
			// See page 264 of "Introduction to algorithms" by Thomas H. Cormen
			Stack<Node> stack = new(2 * Log2(_size + 1)); // this is not exactly right if count is out of date, but the stack can grow
			Node? current = root;
			while (current != null)
			{
				if (IsWithinRange(current.Left?.LeavesCount ?? 0))
				{
					stack.Push(current);
					current = current.Left;
				}
				else if (_lBoundActive && Comparer.Compare(_min, current.Left?.LeavesCount ?? 0) > 0)
					current = current.Right;
				else
					current = current.Left;
			}
			while (stack.Length != 0)
			{
				current = stack.Pop();
				if (!action(current))
				{
					return false;
				}
				Node? node = current.Right;
				while (node != null)
				{
					if (IsWithinRange(node.Left?.LeavesCount ?? 0))
					{
						stack.Push(node);
						node = node.Left;
					}
					else if (_lBoundActive && Comparer.Compare(_min, node.Left?.LeavesCount ?? 0) > 0)
						node = node.Right;
					else
						node = node.Left;
				}
			}
			return true;
		}

		public override BigSumList Insert(int index, mpz_t value)
		{
			if (!IsWithinRange(index))
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}
			var ret = _underlying.Insert(index, value);
			VersionCheck();
#if DEBUG
			Debug.Assert(VersionUpToDate() && root == _underlying.FindRange(_min, _max));
#endif
			return ret;
		}

		internal override bool IsWithinRange(int index)
		{
			int comp = _lBoundActive ? Comparer.Compare(_min, index) : -1;
			if (comp > 0)
			{
				return false;
			}
			comp = _uBoundActive ? Comparer.Compare(_max, index) : 1;
			return comp >= 0;
		}

		/// <summary>
		/// Returns the number of elements <c>count</c> of the parent list.
		/// </summary>
		internal override int TotalCount()
		{
			Debug.Assert(_underlying != null);
			return _underlying.Length;
		}

		/// <summary>
		/// Checks whether this subset is out of date, and updates it if necessary.
		/// </summary>
		/// <param name="updateCount">Updates the count variable if necessary.</param>
		internal override void VersionCheck(bool updateCount = false) => VersionCheckImpl(updateCount);

		private void VersionCheckImpl(bool updateCount)
		{
			Debug.Assert(_underlying != null);
			if (version != _underlying.version)
			{
				root = _underlying.FindRange(_min, _max, _lBoundActive, _uBoundActive);
				version = _underlying.version;
			}
			if (updateCount && _countVersion != _underlying.version)
			{
				_size = 0;
				InOrderTreeWalk(n => { _size++; return true; });
				_countVersion = _underlying.version;
			}
		}

#if DEBUG
		internal override bool VersionUpToDate() => version == _underlying.version;
#endif
	}
}
