
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public unsafe class BitList : BaseList<bool, BitList>, ICloneable
{
	private protected uint* _items;
	private protected int _capacity;

	private protected const int _shrinkThreshold = 256;

	private protected static readonly uint* _emptyPointer = null;

	public BitList()
	{
		_items = _emptyPointer;
		_size = 0;
	}

	public BitList(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_items = capacity == 0 ? _emptyPointer : (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(capacity, BitsPerInt)));
	}

	public BitList(int length, bool defaultValue)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(length, BitsPerInt)));
		_size = length;
		var fillValue = defaultValue ? 0xffffffff : 0;
		for (var i = 0; i < _capacity; i++)
			_items[i] = fillValue;
	}

	public BitList(int length, uint* ptr)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = GetArrayLength(length, BitsPerInt);
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = uints));
		_size = length;
		CopyMemory(ptr, _items, uints);
	}

	public BitList(uint[] values) : this(values.AsSpan()) { }

	public BitList(int length, params uint[] values)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = Max(GetArrayLength(length, BitsPerInt), values.Length);
		// this value is chosen to prevent overflow when computing m_length
		if (uints > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = uints));
		if (uints > values.Length)
			FillMemory(_items + values.Length, uints - values.Length, 0);
		_size = length;
		fixed (uint* ptr = values)
			CopyMemory(ptr, _items, values.Length);
	}

	public BitList(ReadOnlySpan<uint> values)
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

	public BitList(int length, ReadOnlySpan<uint> values)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = Max(GetArrayLength(length, BitsPerInt), values.Length);
		// this value is chosen to prevent overflow when computing m_length
		if (uints > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = uints));
		if (uints > values.Length)
			FillMemory(_items + values.Length, uints - values.Length, 0);
		_size = length;
		fixed (uint* ptr = values)
			CopyMemory(ptr, _items, values.Length);
	}

	public BitList(BitArray bitArray)
	{
		if (bitArray == null)
			throw new ArgumentNullException(nameof(bitArray));
		else
		{
			var arrayLength = _capacity = GetArrayLength(bitArray.Length, BitsPerInt);
			var array = new int[arrayLength];
			bitArray.CopyTo(array, 0);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			fixed (int* array2 = array)
				CopyMemory((uint*)array2, _items, arrayLength);
			_size = bitArray.Length;
		}
	}

	public BitList(IEnumerable<byte> bytes)
	{
		if (bytes == null)
			throw new ArgumentNullException(nameof(bytes));
		else if (bytes is byte[] byteArray)
		{
			if (byteArray.Length > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bytes));
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(byteArray.Length, BytesPerInt)));
			_size = byteArray.Length * BitsPerByte;
			var i = 0;
			var j = 0;
			while (byteArray.Length - j >= 4)
			{
				_items[i++] = (uint)((byteArray[j] & 0xff) |
					((byteArray[j + 1] & 0xff) << BitsPerByte) |
					((byteArray[j + 2] & 0xff) << BitsPerByte * 2) |
					((byteArray[j + 3] & 0xff) << BitsPerByte * 3));
				j += 4;
			}
			if (byteArray.Length - j != 0)
				_items[i] = 0;
			switch (byteArray.Length - j)
			{
				case 3:
				_items[i] |= (uint)(byteArray[j + 2] & 0xff) << BitsPerByte * 2;
				goto case 2;
				case 2:
				_items[i] |= (uint)(byteArray[j + 1] & 0xff) << BitsPerByte;
				goto case 1;
				case 1:
				_items[i] |= (uint)(byteArray[j] & 0xff);
				break;
			}
		}
		else
		{
			var length = bytes.Length();
			if (length > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bytes));
			var arrayLength = _capacity = GetArrayLength(length, BytesPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			FillMemory(_items, arrayLength, 0);
			var i = 0;
			using var en = bytes.GetEnumerator();
			while (en.MoveNext())
			{
				(var index, var remainder) = DivRem(i++, BytesPerInt);
				_items[index] |= (uint)en.Current << remainder * BitsPerByte;
			}
			_size = length * BitsPerByte;
		}
	}

	public BitList(IEnumerable<bool> bools)
	{
		if (bools == null)
			throw new ArgumentNullException(nameof(bools));
		else if (bools is BitList bitList)
		{
			var arrayLength = _capacity = GetArrayLength(bitList._size, BitsPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			_size = bitList._size;
			CopyMemory(bitList._items, _items, arrayLength);
		}
		else if (bools is bool[] boolArray)
		{
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(boolArray.Length, BitsPerInt)));
			_size = boolArray.Length;
			FillMemory(_items, _capacity, 0);
			for (var i = 0; i < boolArray.Length; i++)
				if (boolArray[i])
					_items[i / BitsPerInt] |= 1u << i % BitsPerInt;
		}
		else
		{
			var length = bools.Length();
			var arrayLength = _capacity = GetArrayLength(length, BitsPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			FillMemory(_items, arrayLength, 0);
			var i = 0;
			using var en = bools.GetEnumerator();
			while (en.MoveNext())
			{
				(var index, var remainder) = DivRem(i++, BitsPerInt);
				if (en.Current)
					_items[index] |= 1u << remainder;
			}
			_size = length;
		}
	}

	public BitList(IEnumerable<int> ints)
	{
		if (ints == null)
			throw new ArgumentNullException(nameof(ints));
		else if (ints is int[] intArray)
		{
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * intArray.Length);
			fixed (int* ptr = intArray)
				CopyMemory((uint*)ptr, _items, intArray.Length);
			_size = (_capacity = intArray.Length) * BitsPerInt;
		}
		else
		{
			var toArray = List<int>.ToArrayEnumerable(ints);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * toArray.Length);
			fixed (int* ptr = toArray)
				CopyMemory((uint*)ptr, _items, toArray.Length);
			_size = (_capacity = toArray.Length) * BitsPerInt;
		}
	}

	public BitList(IEnumerable<uint> uints)
	{
		if (uints == null)
			throw new ArgumentNullException(nameof(uints));
		else if (uints is uint[] uintArray)
		{
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * uintArray.Length);
			fixed (uint* ptr = uintArray)
				CopyMemory(ptr, _items, uintArray.Length);
			_size = (_capacity = uintArray.Length) * BitsPerInt;
		}
		else
		{
			var toArray = List<uint>.ToArrayEnumerable(uints);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * toArray.Length);
			fixed (uint* ptr = toArray)
				CopyMemory(ptr, _items, toArray.Length);
			_size = (_capacity = toArray.Length) * BitsPerInt;
		}
	}

	public override int Capacity
	{
		get => _capacity * BitsPerInt;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			var newints = GetArrayLength(value, BitsPerInt);
			if (newints == _capacity)
				return;
			if (newints > _capacity || newints + _shrinkThreshold < _capacity)
			{
				var newarray = (uint*)Marshal.AllocHGlobal(sizeof(uint) * newints);
				CopyMemory(_items, newarray, Min(_capacity, newints));
				if (_capacity != 0)
					Marshal.FreeHGlobal((nint)_items);
				_items = newarray;
				_capacity = newints;
			}
			if (value > _size)
			{
				(var last, var remainder) = DivRem(_size, BitsPerInt);
				_items[last] &= (1u << remainder) - 1;
				FillMemory(_items + (last + 1), newints - last - 1, 0);
			}
			Changed();
		}
	}

	public virtual bool IsReadOnly => false;

	public virtual bool IsSynchronized => false;

	public virtual object SyncRoot
	{
		get
		{
			if (_syncRoot == null)
				Interlocked.CompareExchange<object?>(ref _syncRoot, new(), null);
			return _syncRoot;
		}
	}

	private protected override Func<int, BitList> CapacityCreator => CapacityCreatorStatic;

	private protected static Func<int, BitList> CapacityCreatorStatic => x => new(x);

	private protected override Func<IEnumerable<bool>, BitList> CollectionCreator => CollectionCreatorStatic;

	private protected static Func<IEnumerable<bool>, BitList> CollectionCreatorStatic => x => new(x);

	private protected override int DefaultCapacity => 256;

	public virtual BitList AddRange(BitArray bitArray) => Insert(_size, bitArray);

	public virtual BitList AddRange(IEnumerable<byte> bytes) => Insert(_size, bytes);

	public virtual BitList AddRange(IEnumerable<int> ints) => Insert(_size, ints);

	public virtual BitList AddRange(IEnumerable<uint> uints) => Insert(_size, uints);

	public virtual BitList And(BitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (_size != value._size)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] &= value._items[i];
		return this;
	}

	public override Span<bool> AsSpan(int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
		+ " Используйте GetSlice() вместо него.");

	private protected override void ClearInternal(int index, int length)
	{
		(var startIndex, var startRemainder) = DivRem(index, BitsPerInt);
		var startMask = (1u << startRemainder) - 1;
		(var endIndex, var endRemainder) = DivRem(index + length, BitsPerInt);
		var endMask = ~0u << endRemainder;
		if (startIndex == endIndex)
			_items[startIndex] &= startMask | endMask;
		else
		{
			_items[startIndex] &= startMask;
			for (var i = startIndex + 1; i < endIndex; i++)
				_items[i] = 0;
			_items[endIndex] &= endMask;
		}
		Changed();
	}

	public override bool Contains(bool item, int index, int length) => IndexOf(item, index, length) != -1;

	public static void CopyBits(G.IList<uint> sourceBits, int sourceIndex, G.IList<uint> destinationBits, int destinationIndex, int length)
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
		CopyBitsSide source = (sourceIndex, sourceIndex + length - 1);
		CopyBitsSide destination = (destinationIndex, destinationIndex + length - 1);
		CopyBitsIndex offset = new(destination.Start.IntIndex - source.Start.IntIndex, destination.Start.BitsIndex - source.Start.BitsIndex);
		var sourceStartMask = ~0u << source.Start.BitsIndex; // Маска "головы" источника
		if (destination.End.IntIndex == destination.Start.IntIndex)
			CopyBitsOneInt(sourceBits, destinationBits, length, source, destination, offset, sourceStartMask);
		else if (sourceIndex >= destinationIndex)
			CopyBitsForward(sourceBits, sourceBound, destinationBits, source, destination, offset, sourceStartMask);
		else
			CopyBitsBackward(sourceBits, destinationBits, source, destination, offset);
	}

	private protected static void CopyBitsBackward(uint* sourceBits, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset)
	{
		var sourceEndMask = (uint)(((ulong)1 << source.End.BitsIndex + 1) - 1); // Маска "хвоста" источника
		if (offset.BitsIndex < 0)
			CopyBitsBackwardLeft(sourceBits, destinationBits, source, destination, offset, sourceEndMask);
		else
			CopyBitsBackwardRight(sourceBits, destinationBits, source, destination, offset, sourceEndMask);
	}

	private protected static void CopyBitsBackwardLeft(uint* sourceBits, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset, uint sourceEndMask)
	{
		offset.BitsIndex = -offset.BitsIndex;
		ulong buff = destinationBits[destination.End.IntIndex];
		buff &= ~0ul << destination.End.BitsIndex + 1;
		buff <<= BitsPerInt;
		buff |= (ulong)(sourceBits[source.End.IntIndex] & sourceEndMask) << (source.End.IntIndex + offset.IntIndex != destination.End.IntIndex ? BitsPerInt * 2 : BitsPerInt) - offset.BitsIndex;
		for (int sourceCurrentIntIndex = source.End.IntIndex - 1, destinationCurrentIntIndex = destination.End.IntIndex; destinationCurrentIntIndex > destination.Start.IntIndex; sourceCurrentIntIndex--, destinationCurrentIntIndex--)
		{
			if (source.End.IntIndex + offset.IntIndex != destination.End.IntIndex)
				buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << BitsPerInt - offset.BitsIndex;
			destinationBits[destinationCurrentIntIndex] = (uint)(buff >> BitsPerInt);
			buff <<= BitsPerInt;
			if (source.End.IntIndex + offset.IntIndex == destination.End.IntIndex)
				buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << BitsPerInt - offset.BitsIndex;
		}
		if (source.End.IntIndex + offset.IntIndex != destination.End.IntIndex)
			buff |= (ulong)sourceBits[source.Start.IntIndex] << BitsPerInt - offset.BitsIndex;
		var destinationMask = ~0ul << BitsPerInt + destination.Start.BitsIndex;
		buff &= destinationMask;
		destinationBits[destination.Start.IntIndex] &= (uint)(~destinationMask >> BitsPerInt);
		destinationBits[destination.Start.IntIndex] |= (uint)(buff >> BitsPerInt);
	}

	private protected static void CopyBitsBackwardRight(uint* sourceBits, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset, uint sourceEndMask)
	{
		ulong buff = destinationBits[destination.End.IntIndex];
		buff &= ~0ul << destination.End.BitsIndex + 1;
		buff <<= BitsPerInt;
		buff |= (ulong)(sourceBits[source.End.IntIndex] & sourceEndMask) << (source.End.IntIndex + offset.IntIndex != destination.End.IntIndex ? 0 : BitsPerInt) + offset.BitsIndex;
		for (int sourceCurrentIntIndex = source.End.IntIndex - 1, destinationCurrentIntIndex = destination.End.IntIndex; destinationCurrentIntIndex > destination.Start.IntIndex; sourceCurrentIntIndex--, destinationCurrentIntIndex--)
		{
			if (source.End.IntIndex + offset.IntIndex == destination.End.IntIndex)
				buff |= (ulong)sourceBits[sourceCurrentIntIndex] << offset.BitsIndex;
			destinationBits[destinationCurrentIntIndex] = (uint)(buff >> BitsPerInt);
			buff <<= BitsPerInt;
			if (source.End.IntIndex + offset.IntIndex != destination.End.IntIndex && sourceCurrentIntIndex >= 0)
				buff |= (ulong)sourceBits[sourceCurrentIntIndex] << offset.BitsIndex;
		}
		var destinationMask = ~0ul << BitsPerInt + destination.Start.BitsIndex;
		buff &= destinationMask;
		destinationBits[destination.Start.IntIndex] &= (uint)(~destinationMask >> BitsPerInt);
		destinationBits[destination.Start.IntIndex] |= (uint)(buff >> BitsPerInt);
	}

	private protected static void CopyBitsForward(uint* sourceBits, int sourceBound, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset, uint sourceStartMask)
	{
		if (offset.BitsIndex < 0)
			CopyBitsForwardLeft(sourceBits, destinationBits, source, destination, offset, sourceStartMask);
		else
			CopyBitsForwardRight(sourceBits, sourceBound, destinationBits, source, destination, offset, sourceStartMask);
	}

	private protected static void CopyBitsForwardLeft(uint* sourceBits, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset, uint sourceStartMask)
	{
		offset.BitsIndex = -offset.BitsIndex;
		ulong buff = destinationBits[destination.Start.IntIndex];
		buff &= ((ulong)1 << destination.Start.BitsIndex) - 1;
		buff |= (ulong)(sourceBits[source.Start.IntIndex] & sourceStartMask) >> offset.BitsIndex;
		for (int sourceCurrentIntIndex = source.Start.IntIndex + 1, destinationCurrentIntIndex = destination.Start.IntIndex; destinationCurrentIntIndex < destination.End.IntIndex; sourceCurrentIntIndex++, destinationCurrentIntIndex++)
		{
			buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << BitsPerInt - offset.BitsIndex;
			destinationBits[destinationCurrentIntIndex] = (uint)buff;
			buff >>= BitsPerInt;
		}
		if (source.End.IntIndex + offset.IntIndex != destination.End.IntIndex)
			buff |= (ulong)sourceBits[source.End.IntIndex] << BitsPerInt - offset.BitsIndex;
		var destinationMask = ((ulong)1 << destination.End.BitsIndex + 1) - 1;
		buff &= destinationMask;
		destinationBits[destination.End.IntIndex] &= (uint)~destinationMask;
		destinationBits[destination.End.IntIndex] |= (uint)buff;
	}

	private protected static void CopyBitsForwardRight(uint* sourceBits, int sourceBound, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset, uint sourceStartMask)
	{
		ulong buff = destinationBits[destination.Start.IntIndex];
		buff &= ((ulong)1 << destination.Start.BitsIndex) - 1;
		buff |= (ulong)(sourceBits[source.Start.IntIndex] & sourceStartMask) << offset.BitsIndex;
		for (int sourceCurrentIntIndex = source.Start.IntIndex + 1, destinationCurrentIntIndex = destination.Start.IntIndex; destinationCurrentIntIndex < destination.End.IntIndex; sourceCurrentIntIndex++, destinationCurrentIntIndex++)
		{
			destinationBits[destinationCurrentIntIndex] = (uint)buff;
			buff >>= BitsPerInt;
			if (sourceCurrentIntIndex < sourceBound)
				buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << offset.BitsIndex;
		}
		var destinationMask = ((ulong)1 << destination.End.BitsIndex + 1) - 1;
		buff &= destinationMask;
		destinationBits[destination.End.IntIndex] &= (uint)~destinationMask;
		destinationBits[destination.End.IntIndex] |= (uint)buff;
	}

	private protected static void CopyBitsOneInt(uint* sourceBits, uint* destinationBits, int length, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset, uint sourceStartMask)
	{
		var buff = sourceBits[source.Start.IntIndex] & sourceStartMask;
		var destinationMask = ~(length == BitsPerInt ? 0 : ~0u << length) << destination.Start.BitsIndex;
		if (offset.BitsIndex >= 0)
			buff <<= offset.BitsIndex;
		else
		{
			buff >>= -offset.BitsIndex;
			if (length + source.Start.BitsIndex > BitsPerInt)
				buff |= sourceBits[source.Start.IntIndex + 1] << BitsPerInt + offset.BitsIndex;
		}
		buff &= destinationMask;
		destinationBits[destination.Start.IntIndex] &= ~destinationMask;
		destinationBits[destination.Start.IntIndex] |= buff;
	}

	private protected static void CheckParams(uint* sourceBits, int sourceBound, int sourceIndex, uint* destinationBits, int destinationBound, int destinationIndex, int length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBound == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBound == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		var sourceLengthBits = sourceBound * BitsPerInt; // Длина массивов в битах.
		var destinationLengthBits = destinationBound * BitsPerInt; // Длина массивов в битах.
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

	internal override void CopyToInternal(int sourceIndex, BitList destination, int destinationIndex, int length)
	{
		CopyBits(_items, _capacity, sourceIndex, destination._items, destination._capacity, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
		destination.Changed();
	}

	private protected override void CopyToInternal(Array array, int index)
	{
		if (array.Rank != 1)
			throw new RankException();
		if (array is int[] intArray)
			fixed (int* ptr = intArray)
				CopyMemory(_items, (uint*)(ptr + index), GetArrayLength(_size, BitsPerInt));
		else if (array is uint[] uintArray)
			fixed (uint* ptr = uintArray)
				CopyMemory(_items, ptr + index, GetArrayLength(_size, BitsPerInt));
		else if (array is byte[] byteArray)
			fixed (byte* ptr = byteArray)
				CopyMemory((byte*)_items, ptr + index, GetArrayLength(_size, BitsPerByte));
		else if (array is bool[] boolArray)
			CopyToInternal(boolArray, index);
		else
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
	}

	private protected void CopyToInternal(bool[] array, int index)
	{
		if (array.Length - index < _size)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		CopyToInternal(index, array, 0, array.Length);
	}

	private protected override void CopyToInternal(int index, bool[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex + i] = ((_items[(index + i) / BitsPerInt] >> ((index + i) % BitsPerInt)) & 0x00000001) != 0;
	}

	public override void Dispose()
	{
		if (_capacity != 0)
			Marshal.FreeHGlobal((nint)_items);
		_capacity = 0;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	private protected override bool EqualsInternal(IEnumerable<bool>? collection, int index, bool toEnd = false)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is BitList bitList)
			return EqualsToBitList(bitList, index, toEnd);
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
				if (index >= _size || !GetInternal(index++).Equals(item))
					return false;
			return !toEnd || index == _size;
		}
	}

	private protected virtual bool EqualsToBitList(BitList bitList, int index, bool toEnd = false)
	{
		if (index > _size - bitList._size)
			return false;
		if (toEnd && index < _size - bitList._size)
			return false;
		if (bitList._size == 0)
			return !toEnd || index == _size;
		CopyBitsIndex start = index;
		var startMask = ~0u << start.BitsIndex;
		CopyBitsIndex destinationEnd = bitList._size - 1;
		if (destinationEnd.IntIndex == 0)
			return EqualsToBitListOneInt(bitList, start, startMask);
		else
			return EqualsToBitListSeveralInts(bitList, index, start, startMask, destinationEnd);
	}

	private protected virtual bool EqualsToBitListOneInt(BitList bitList, CopyBitsIndex start, uint startMask)
	{
		var buff = _items[start.IntIndex] & startMask;
		var destinationMask = ~(bitList._size == BitsPerInt ? 0 : ~0u << bitList._size);
		buff >>= start.BitsIndex;
		if (bitList._size + start.BitsIndex > BitsPerInt)
			buff |= _items[start.IntIndex + 1] << BitsPerInt - start.BitsIndex;
		buff &= destinationMask;
		return (bitList._items[0] & destinationMask) == buff;
	}

	private protected virtual bool EqualsToBitListSeveralInts(BitList bitList, int index, CopyBitsIndex start, uint startMask, CopyBitsIndex destinationEnd)
	{
		var buff = (ulong)(_items[start.IntIndex] & startMask) >> start.BitsIndex;
		var sourceEndIntIndex = (index + bitList._size - 1) / BitsPerInt;
		for (int sourceCurrentIntIndex = start.IntIndex + 1, destinationCurrentIntIndex = 0; destinationCurrentIntIndex < destinationEnd.IntIndex; sourceCurrentIntIndex++, destinationCurrentIntIndex++)
		{
			buff |= ((ulong)_items[sourceCurrentIntIndex]) << BitsPerInt - start.BitsIndex;
			if (bitList._items[destinationCurrentIntIndex] != (uint)buff)
				return false;
			buff >>= BitsPerInt;
		}
		if (sourceEndIntIndex - start.IntIndex != destinationEnd.IntIndex)
			buff |= (ulong)_items[sourceEndIntIndex] << BitsPerInt - start.BitsIndex;
		var destinationMask = ((ulong)1 << destinationEnd.BitsIndex + 1) - 1;
		buff &= destinationMask;
		return (bitList._items[destinationEnd.IntIndex] & destinationMask) == buff;
	}

	public override int GetHashCode() => _capacity < 3 ? 1234567890 : _items[0].GetHashCode() ^ _items[1].GetHashCode() ^ _items[_capacity - 1].GetHashCode();

	internal override bool GetInternal(int index, bool invoke = true)
	{
		var item = (_items[index / BitsPerInt] & (1 << index % BitsPerInt)) != 0;
		if (invoke)
			Changed();
		return item;
	}

	public virtual uint GetSmallRange(int index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return new();
		if (length > BitsPerInt)
			throw new ArgumentException($"Метод GetSmallRange() возвращает одно число типа uint, поэтому необходим диапазон длиной не более {BitsPerInt} бит.", nameof(length));
		(var quotient, var remainder) = DivRem(index, BitsPerInt);
		(var quotient2, var remainder2) = DivRem(index + length - 1, BitsPerInt);
		uint result;
		if (quotient == quotient2)
			result = (_items[quotient] >> remainder) & ~(length == BitsPerInt ? 0 : ~0u << length);
		else
		{
			result = _items[quotient] >> remainder;
			result |= (_items[quotient + 1] & ((1u << remainder2 + 1) - 1)) << BitsPerInt - remainder;
		}
		return result;
	}

	private protected override int IndexOfInternal(bool item, int index, int length)
	{
		var fillValue = item ? 0 : unchecked((int)0xffffffff);
		(var startIndex, var startRemainder) = DivRem(index, BitsPerInt);
		(var endIndex, var endRemainder) = DivRem(index + length, BitsPerInt);
		if (startIndex == endIndex)
		{
			for (var i = startRemainder; i < endRemainder; i++)
				if ((_items[startIndex] & (1 << i)) == 0 ^ item)
					return startIndex * BitsPerInt + i;
			return -1;
		}
		var invRemainder = BitsPerInt - startRemainder;
		var mask = (1 << invRemainder) - 1;
		var first = _items[startIndex] >> startRemainder;
		if (first != (item ? 0 : mask))
			for (var i = 0; i < invRemainder; i++)
				if ((first & (1 << i)) == 0 ^ item)
					return index + i;
		for (var i = startIndex + 1; i < endIndex; i++)
			if (_items[i] != fillValue)
				if (IndexOfMainCycle(item, i, out var result))
					return result;
		if (endRemainder != 0)
			for (var i = 0; i < endRemainder; i++)
				if ((_items[endIndex] & (1 << i)) == 0 ^ item)
					return endIndex * BitsPerInt + i;
		return -1;
	}

	private protected virtual bool IndexOfMainCycle(bool item, int i, out int index)
	{
		for (var j = 0; j < BitsPerInt; j++)
			if ((_items[i] & (1 << j)) == 0 ^ item)
			{
				index = i * BitsPerInt + j;
				return true;
			}
		index = -1;
		return false;
	}

	public virtual BitList Insert(int index, BitArray bitArray) => Insert(index, new BitList(bitArray));

	public virtual BitList Insert(int index, IEnumerable<byte> bytes) => Insert(index, new BitList(bytes));

	public virtual BitList Insert(int index, IEnumerable<int> ints)
	{
		ArgumentNullException.ThrowIfNull(ints);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (ints is NList<int> intNList)
		{
			var length = intNList.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			fixed (int* intPtr = intNList.AsSpan())
				CopyBits((uint*)intPtr, intNList.Length, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else if (ints is List<int> intList)
		{
			var length = intList.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			fixed (int* intPtr = intList.AsSpan())
				CopyBits((uint*)intPtr, intList.Length, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else if (ints is int[] intArray)
		{
			var length = intArray.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			fixed (int* intPtr = intArray)
				CopyBits((uint*)intPtr, intArray.Length, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else
			return Insert(index, new BitList(ints));
	}

	public virtual BitList Insert(int index, IEnumerable<uint> uints)
	{
		ArgumentNullException.ThrowIfNull(uints);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (uints is NList<uint> uintNList)
		{
			var length = uintNList.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			fixed (uint* uintPtr = uintNList.AsSpan())
				CopyBits(uintPtr, uintNList.Length, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else if (uints is List<uint> uintList)
		{
			var length = uintList.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			fixed (uint* uintPtr = uintList.AsSpan())
				CopyBits(uintPtr, uintList.Length, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else if (uints is uint[] uintArray)
		{
			var length = uintArray.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			fixed (uint* uintPtr = uintArray)
				CopyBits(uintPtr, uintArray.Length, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else
			return Insert(index, new BitList(uints));
	}

	private protected override int LastIndexOfInternal(bool item, int index, int length)
	{
		var fillValue = item ? 0 : unchecked((int)0xffffffff);
		(var startIndex, var startRemainder) = DivRem(index + 1 - length, BitsPerInt);
		(var endIndex, var endRemainder) = DivRem(index + 1, BitsPerInt);
		if (startIndex == endIndex)
		{
			for (var i = endRemainder - 1; i >= startRemainder; i--)
				if ((_items[startIndex] & (1 << i)) == 0 ^ item)
					return startIndex * BitsPerInt + i;
			return -1;
		}
		if (endRemainder != 0)
			for (var i = endRemainder - 1; i >= 0; i--)
				if ((_items[endIndex] & (1 << i)) == 0 ^ item)
					return endIndex * BitsPerInt + i;
		for (var i = endIndex - 1; i >= startIndex + 1; i--)
			if (_items[i] != fillValue)
				if (LastIndexOfMainCycle(item, i, out var result))
					return result;
		var invRemainder = BitsPerInt - startRemainder;
		var mask = invRemainder == BitsPerInt ? 0xffffffff : (1u << invRemainder) - 1;
		var first = _items[startIndex] >> startRemainder;
		if (first != (item ? 0 : mask))
			for (var i = invRemainder - 1; i >= 0; i--)
				if ((first & (1 << i)) == 0 ^ item)
					return i;
		return -1;
	}

	private protected virtual bool LastIndexOfMainCycle(bool item, int i, out int index)
	{
		for (var j = BitsPerInt - 1; j >= 0; j--)
			if ((_items[i] & (1 << j)) == 0 ^ item)
			{
				index = i * BitsPerInt + j;
				return true;
			}
		index = -1;
		return false;
	}

	public virtual BitList Not()
	{
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] = ~_items[i];
		return this;
	}

	public virtual BitList Or(BitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (_size != value._size)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] |= value._items[i];
		return this;
	}

	private protected override BitList ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
			(this[index + i], this[index + length - i - 1]) = (GetInternal(index + length - i - 1), GetInternal(index + i));
		Changed();
		return this;
	}

	public override BitList SetAll(bool value, int index, int length)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > _size)
			throw new ArgumentException("Устанавливаемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return this;
		SetAllInternal(value, index, length);
		return this;
	}

	private protected override void SetAllInternal(bool value, int index, int length)
	{
		if (length == 0)
			return;
		(var intIndex, var bitsIndex) = DivRem(index, BitsPerInt);
		(var endIntIndex, var endBitsIndex) = DivRem(index + length - 1, BitsPerInt);
		if (intIndex == endIntIndex)
		{
			var mask = ~(length == BitsPerInt ? 0 : ~0u << length) << bitsIndex;
			_items[intIndex] = value ? _items[intIndex] | mask : _items[intIndex] & ~mask;
		}
		else
		{
			var startMask = ~0u << bitsIndex;
			_items[intIndex] = value ? _items[intIndex] | startMask : _items[intIndex] & ~startMask;
			var fillValue = value ? 0xffffffff : 0;
			for (var i = intIndex + 1; i < endIntIndex; i++)
				_items[i] = fillValue;
			var endMask = endBitsIndex == BitsPerInt - 1 ? 0xffffffff : (1u << endBitsIndex + 1) - 1;
			_items[endIntIndex] = value ? _items[endIntIndex] | endMask : _items[endIntIndex] & ~endMask;
		}
	}

	internal override void SetInternal(int index, bool value)
	{
		if (value)
			_items[index / BitsPerInt] |= 1u << index % BitsPerInt;
		else
			_items[index / BitsPerInt] &= ~(1u << index % BitsPerInt);
		Changed();
	}

	public virtual BitList SetRange(int index, BitArray bitArray) => SetRange(index, new BitList(bitArray));

	public virtual BitList SetRange(int index, IEnumerable<byte> bytes) => SetRange(index, new BitList(bytes));

	public virtual BitList SetRange(int index, IEnumerable<int> ints)
	{
		ArgumentNullException.ThrowIfNull(ints);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (ints is NList<int> intNList)
		{
			if (index + intNList.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (intNList.Length <= 0)
				return this;
			fixed (int* intPtr = intNList.AsSpan())
				CopyBits((uint*)intPtr, intNList.Length, 0, _items, _capacity, index, intNList.Length);
		}
		else if (ints is List<int> intList)
		{
			if (index + intList.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (intList.Length <= 0)
				return this;
			fixed (int* intPtr = intList.AsSpan())
				CopyBits((uint*)intPtr, intList.Length, 0, _items, _capacity, index, intList.Length);
		}
		else if (ints is int[] intArray)
		{
			if (index + intArray.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (intArray.Length <= 0)
				return this;
			fixed (int* intPtr = intArray)
				CopyBits((uint*)intPtr, intArray.Length, 0, _items, _capacity, index, intArray.Length);
		}
		else
			SetRange(index, new BitList(ints));
		return this;
	}

	public virtual BitList SetRange(int index, IEnumerable<uint> uints)
	{
		ArgumentNullException.ThrowIfNull(uints);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (uints is NList<uint> uintNList)
		{
			if (index + uintNList.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (uintNList.Length <= 0)
				return this;
			fixed (uint* uintPtr = uintNList.AsSpan())
				CopyBits(uintPtr, uintNList.Length, 0, _items, _capacity, index, uintNList.Length);
		}
		else if (uints is List<uint> uintList)
		{
			if (index + uintList.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (uintList.Length <= 0)
				return this;
			fixed (uint* uintPtr = uintList.AsSpan())
				CopyBits(uintPtr, uintList.Length, 0, _items, _capacity, index, uintList.Length);
		}
		else if (uints is uint[] uintArray)
		{
			if (index + uintArray.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (uintArray.Length <= 0)
				return this;
			fixed (uint* uintPtr = uintArray)
				CopyBits(uintPtr, uintArray.Length, 0, _items, _capacity, index, uintArray.Length);
		}
		else
			SetRange(index, new BitList(uints));
		return this;
	}

	public NList<byte> ToByteList()
	{
		using var uints = ToUIntList();
		fixed (uint* ptr = uints.AsSpan())
			return new(GetArrayLength(_size, BitsPerByte), (byte*)ptr);
	}

	public virtual NList<uint> ToUIntList()
	{
		var length = GetArrayLength(_size, BitsPerInt);
		NList<uint> result = new(length);
		for (var i = 0; i < length; i++)
			result.Add(_items[i]);
		return result;
	}

	public virtual BitList Xor(BitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (_size != value._size)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] ^= value._items[i];
		return this;
	}

	private protected record struct CopyBitsIndex(int IntIndex, int BitsIndex)
	{
		public static implicit operator CopyBitsIndex(int index) => new(index / BitsPerInt, index % BitsPerInt);
	}

	private protected record struct CopyBitsSide(CopyBitsIndex Start, CopyBitsIndex End)
	{
		public static implicit operator CopyBitsSide((CopyBitsIndex Start, CopyBitsIndex End) x) => new(x.Start, x.End);
	}
}

/// <summary>
/// Представляет компактный строго типизированный список бит (true или false), упорядоченных по индексу.
/// В отличие от <see cref="BitList"/> и стандартного <see cref="BitArray"/>, имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций со списком находятся в разработке, на текущий момент
/// поддерживаются только добавление в конец, установка элемента по индексу и частично удаление.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigBitList : BigList<bool, BigBitList, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private protected const int BitsPerInt = sizeof(int) * BitsPerByte;
	private protected const int BytesPerInt = sizeof(int);
	private protected const int BitsPerByte = 8;

	public BigBitList() : this(-1) { }

	public BigBitList(int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(subbranchesBitLength, leafSizeBitLength) { }

	public BigBitList(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(capacity, subbranchesBitLength, leafSizeBitLength) { }

	public BigBitList(MpzT length, bool defaultValue, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (length <= LeafSize)
		{
			low = new((int)length, defaultValue);
			high = null;
			highLength = null;
			AddCapacity(low.Length);
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((length - 1).BitLength - LeafSizeBitLength, SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			high = new((int)GetArrayLength(length, fragment));
			highLength = [];
			for (MpzT i = 0; i < high.Capacity - 1; i++)
			{
				high.Add(new(fragment, defaultValue, SubbranchesBitLength, LeafSizeBitLength));
				high[^1].parent = this;
				AddCapacity(fragment);
				highLength.Add(fragment);
			}
			var rest = length % fragment == 0 ? fragment : length % fragment;
			high.Add(new(rest, defaultValue, SubbranchesBitLength, LeafSizeBitLength));
			high[^1].parent = this;
			AddCapacity(rest);
			highLength.Add(rest);
		}
		Length = length;
		AddCapacity(Length - _capacity);
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(BitArray bitArray, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		if (bitArray == null)
			throw new ArgumentNullException(nameof(bitArray));
		else
		{
			using BitList bitList = new(bitArray);
			ConstructFromBitList(bitList);
		}
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(IEnumerable<byte> bytes, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		if (bytes == null)
			throw new ArgumentNullException(nameof(bytes));
		else if (bytes is byte[] byteArray && byteArray.Length <= int.MaxValue / BitsPerByte)
		{
			using BitList bitList = new(byteArray);
			ConstructFromBitList(bitList);
		}
		else
		{
			var b = true;
			using var en = bytes.GetEnumerator();
			using BigList<uint> values = [];
			var n = 0;
			while (b)
			{
				var i = 0;
				uint value = 0;
				for (; i < BytesPerInt; i++, n++)
				{
					if (!(b = en.MoveNext())) break;
					value |= (uint)en.Current << BitsPerByte * i;
				}
				if (i != 0)
					values.Add(value);
			}
			ConstructFromUIntList(values, n % BytesPerInt * BitsPerByte);
		}
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(IEnumerable<bool> bools, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		if (bools == null)
			throw new ArgumentNullException(nameof(bools));
		else if (bools is BigBitList bigBitList)
		{
			ConstructFromCapacity(bigBitList.Length);
			if (bigBitList.low != null)
				ConstructFromBitList(bigBitList.low);
			else if (bigBitList.high != null && bigBitList.highLength != null)
				bigBitList.CopyToInternal(0, this, 0, bigBitList.Length);
		}
		else if (bools is BitList bitList)
		{
			ConstructFromBitList(bitList);
		}
		else if (bools is bool[] boolArray)
		{
			using BitList bitList2 = new(boolArray);
			ConstructFromBitList(bitList2);
		}
		else
		{
			using var en = bools.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(IEnumerable<int> ints, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		if (ints == null)
			throw new ArgumentNullException(nameof(ints));
		else if (ints is int[] intArray && intArray.Length <= int.MaxValue / BitsPerInt)
		{
			using BitList bitList = new(intArray);
			ConstructFromBitList(bitList);
		}
		else
		{
			using BigList<uint> list = new(ints.Select(x => (uint)x));
			ConstructFromUIntList(list);
		}
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(IEnumerable<uint> uints, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		if (uints == null)
			throw new ArgumentNullException(nameof(uints));
		else if (uints is BigList<uint> bigUIntList)
			ConstructFromUIntList(bigUIntList);
		else if (uints is uint[] uintArray && uintArray.Length <= int.MaxValue / BitsPerInt)
		{
			ConstructFromBitList(new(uintArray));
		}
		else
		{
		{
			using BigList<uint> list = new(uints);
			ConstructFromUIntList(list);
		}
		}
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(uint[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(values.AsEnumerable(), subbranchesBitLength, leafSizeBitLength) { }

	public BigBitList(ReadOnlySpan<uint> values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(values.Length * BitsPerInt, subbranchesBitLength, leafSizeBitLength)
	{
		using BigList<uint> list = new(values);
		ConstructFromUIntList(list);
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigBitList(MpzT capacity, uint[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(RedStarLinq.Max(capacity, values.Length * BitsPerInt), values.AsSpan(),
		subbranchesBitLength, leafSizeBitLength) { }

	public BigBitList(MpzT capacity, ReadOnlySpan<uint> values, int subbranchesBitLength = -1,
		int leafSizeBitLength = -1) : this(RedStarLinq.Max(capacity, values.Length * BitsPerInt),
		subbranchesBitLength, leafSizeBitLength)
	{
		using BigList<uint> list = new(values);
		ConstructFromUIntList(list);
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	private protected override Func<MpzT, BigBitList> CapacityCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	private protected override int LeafSizeBitLength { get; init; } = 20;

	private protected override Func<int, BitList> CapacityLowCreator => x => new(x);

	private protected override Func<IEnumerable<bool>, BitList> CollectionLowCreator => x => new(x);

	private protected override Func<IEnumerable<bool>, BigBitList> CollectionCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	private protected override int DefaultCapacity => 256;

	public virtual BigBitList AddRange(BitArray bitArray)
	{
		using BigBitList bitList = new(bitArray, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList AddRange(IEnumerable<byte> bytes)
	{
		using BigBitList bitList = new(bytes, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList AddRange(IEnumerable<int> ints)
	{
		using BigBitList bitList = new(ints, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList AddRange(IEnumerable<uint> uints)
	{
		using BigBitList bitList = new(uints, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList And(BigBitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.And(value.low);
		else high = high != null && value.high != null
			? [.. high.Combine(value.high, (x, y) => x.And(y))]
			: throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	private protected virtual void ConstructFromBitList(BitList bitList)
	{
		if ((low == null || low.Capacity == 0) && high == null && highLength == null && fragment == 1 && _capacity == 0)
		{
			ConstructFromBitListFromScratch(bitList);
			return;
		}
		Debug.Assert(bitList.Length <= _capacity);
		if (bitList.Length <= LeafSize && low != null && high == null && highLength == null && fragment == 1)
		{
			low.AddRange(bitList);
			Length = bitList.Length;
		}
		else
		{
			Debug.Assert(low == null && high != null && highLength != null && fragment != 1);
			var fragment2 = (int)fragment;
			var i = 0;
			var index = 0;
			for (; index <= bitList.Length - fragment2; index += fragment2)
			{
				high[i++].ConstructFromBitList(bitList.GetRange(index, fragment2));
				highLength.Add(fragment);
			}
			var rest = bitList.Length - index;
			Debug.Assert(rest < fragment);
			if (rest != 0)
			{
				high[i].ConstructFromBitList(bitList.GetRange(index));
				highLength.Add(rest);
			}
		}
	}

	private protected virtual void ConstructFromBitListFromScratch(BitList bitList)
	{
		Debug.Assert((low == null || low.Capacity == 0) && high == null && highLength == null && fragment == 1 && _capacity == 0);
		if (bitList.Length <= LeafSize && high == null && highLength == null && fragment == 1)
		{
			if (low == null)
				low = new(bitList);
			else
				low.AddRange(bitList);
			high = null;
			highLength = null;
			fragment = 1;
			AddCapacity(bitList.Length);
			Length = bitList.Length;
		}
		else
		{
			low = null;
			fragment = 1 << ((((MpzT)bitList.Length - 1).BitLength + SubbranchesBitLength - 1 - LeafSizeBitLength) / SubbranchesBitLength - 1) * SubbranchesBitLength + LeafSizeBitLength;
			var fragment2 = (int)fragment;
			high = new(GetArrayLength(bitList.Length, fragment2));
			highLength = [];
			var index = 0;
			for (; index <= bitList.Length - fragment2; index += fragment2)
			{
				high.Add(new(SubbranchesBitLength, LeafSizeBitLength));
				high[^1].parent = this;
				high[^1].ConstructFromBitListFromScratch(bitList.GetRange(index, fragment2));
				highLength.Add(fragment);
			}
			if (bitList.Length % fragment2 != 0)
			{
				var rest = bitList.Length - index;
				high.Add(new(SubbranchesBitLength, LeafSizeBitLength));
				high[^1].parent = this;
				high[^1].ConstructFromBitListFromScratch(bitList.GetRange(index));
				highLength.Add(rest);
			}
		}
	}

	private protected virtual void ConstructFromUIntList(BigList<uint> bigUIntList, int overrideLength = 0)
	{
		if ((low == null || low.Capacity == 0) && high == null && highLength == null && fragment == 1 && _capacity == 0)
		{
			ConstructFromUIntListFromScratch(bigUIntList, overrideLength);
			return;
		}
		ArgumentOutOfRangeException.ThrowIfNegative(overrideLength);
		var length = bigUIntList.Length;
		var bitLength = length == 0 || overrideLength % BitsPerInt == 0 ? length * BitsPerInt : (length - 1) * BitsPerInt + overrideLength % BitsPerInt;
		Debug.Assert(bitLength <= _capacity);
		if (length <= LeafSize / BitsPerInt && low != null && high == null && highLength == null && fragment == 1)
		{
			low.AddRange(bigUIntList);
			low.RemoveEnd((int)bitLength);
			Length = bitLength;
		}
		else
		{
			Debug.Assert(low == null && high != null && highLength != null && fragment != 1);
			var uintsFragment = fragment / BitsPerInt;
			var i = 0;
			MpzT index = 0;
			for (; index < length - uintsFragment; index += uintsFragment)
			{
				high[i++].ConstructFromUIntList(bigUIntList.GetRange(index, uintsFragment));
				highLength.Add(fragment);
			}
			if (index != length)
			{
				var rest = bitLength % fragment;
				if (rest == 0)
					rest = fragment;
				high[i].ConstructFromUIntList(bigUIntList.GetRange(index), overrideLength);
				high[i].RemoveEndInternal(rest);
				highLength.Add(rest);
			}
		}
	}

	private protected virtual void ConstructFromUIntListFromScratch(BigList<uint> bigUIntList, int overrideLength = 0)
	{
		Debug.Assert((low == null || low.Capacity == 0) && high == null && highLength == null && fragment == 1 && _capacity == 0);
		ArgumentOutOfRangeException.ThrowIfNegative(overrideLength);
		var length = bigUIntList.Length;
		var bitLength = length == 0 || overrideLength % BitsPerInt == 0 ? length * BitsPerInt : (length - 1) * BitsPerInt + overrideLength % BitsPerInt;
		if (bitLength <= LeafSize && high == null && highLength == null && fragment == 1)
		{
			if (low == null)
				low = new(bigUIntList);
			else
				low.AddRange(bigUIntList);
			low.RemoveEnd((int)bitLength);
			high = null;
			highLength = null;
			fragment = 1;
			AddCapacity(bitLength);
			Length = bitLength;
		}
		else
		{
			low = null;
			fragment = 1 << (((length - 1).BitLength + ((MpzT)BitsPerInt - 1).BitLength + SubbranchesBitLength - 1 - LeafSizeBitLength) / SubbranchesBitLength - 1) * SubbranchesBitLength + LeafSizeBitLength;
			var uintsFragment = fragment / BitsPerInt;
			high = new((int)GetArrayLength(length, uintsFragment));
			highLength = [];
			MpzT index = 0;
			for (; index < length - uintsFragment; index += uintsFragment)
			{
				high.Add(new(SubbranchesBitLength, LeafSizeBitLength));
				high[^1].parent = this;
				high[^1].ConstructFromUIntListFromScratch(bigUIntList.GetRange(index, uintsFragment));
				highLength.Add(fragment);
			}
			if (index != length)
			{
				var rest = bitLength % fragment;
				if (rest == 0)
					rest = fragment;
				high.Add(new(SubbranchesBitLength, LeafSizeBitLength));
				high[^1].parent = this;
				high[^1].ConstructFromUIntListFromScratch(bigUIntList.GetRange(index), overrideLength);
				high[^1].RemoveEndInternal(rest);
				highLength.Add(rest);
			}
		}
	}

	public virtual uint GetSmallRange(MpzT index, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		if (length == 0)
			return 0;
		if (length > BitsPerInt)
			throw new ArgumentException($"Метод GetSmallRange() возвращает одно число типа uint, поэтому необходим диапазон длиной не более {BitsPerInt} бит.", nameof(length));
		if (low != null)
			return low.GetSmallRange((int)index, length);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)(index + length - 1).Divide(fragment, out var remainder2);
			uint result;
			if (quotient == quotient2)
				result = high.GetInternal(quotient).GetSmallRange(remainder, length);
			else
			{
				result = high.GetInternal(quotient).GetSmallRange(remainder, (int)(fragment - remainder));
				result |= high.GetInternal(quotient + 1).GetSmallRange(0, (int)remainder2) << (int)(BitsPerInt - remainder);
			}
			return result;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitList Not()
	{
		if (low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual BigBitList Or(BigBitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.Or(value.low);
		else high = high != null && value.high != null
			? [.. high.Combine(value.high, (x, y) => x.Or(y))]
			: throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual void SetAll(bool value)
	{
		if (low != null)
			low.SetAll(value);
		else if (high != null)
			high.ForEach(x => x.SetAll(value));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigList<uint> ToUIntBigList()
	{
		if (low != null)
			return new(low.ToUIntList());
		else if (high != null)
			return new(high.SelectMany(x => x.ToUIntBigList()));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitList Xor(BigBitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.Xor(value.low);
		else high = high != null && value.high != null
			? [.. high.Combine(value.high, (x, y) => x.Xor(y))]
			: throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}
}
