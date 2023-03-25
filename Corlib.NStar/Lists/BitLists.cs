using System.Diagnostics;

namespace Corlib.NStar;

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public unsafe class BitList : ListBase<bool, BitList>, ICloneable
{
	private uint* _items;
	private int _capacity;

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
					_items[i / BitsPerInt] |= (uint)1 << i % BitsPerInt;
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
				Marshal.FreeHGlobal((nint)_items);
				_items = newarray;
				_capacity = newints;
			}
			if (value > _size)
			{
				(int last, int remainder) = DivRem(_size, BitsPerInt);
				_items[last] &= ((uint)1 << remainder) - 1;
				FillMemory(_items + (last + 1), newints - last - 1, 0);
			}
			Changed();
		}
	}

	private protected override Func<int, BitList> CapacityCreator => CapacityCreatorStatic;

	private static Func<int, BitList> CapacityCreatorStatic => x => new(x);

	private protected override Func<IEnumerable<bool>, BitList> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<bool>, BitList> CollectionCreatorStatic => x => new(x);

	private protected override int DefaultCapacity => 256;

	internal override bool GetInternal(int index, bool invoke = true)
	{
		bool item = (_items[index / BitsPerInt] & (1 << index % BitsPerInt)) != 0;
		if (invoke)
			Changed();
		return item;
	}

	internal override void SetInternal(int index, bool value)
	{
		if (value)
			_items[index / BitsPerInt] |= (uint)1 << index % BitsPerInt;
		else
			_items[index / BitsPerInt] &= ~((uint)1 << index % BitsPerInt);
		Changed();
	}

	public virtual void SetAll(bool value)
	{
		uint fillValue = value ? 0xffffffff : 0;
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] = fillValue;
	}

	public virtual BitList And(BitList value)
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

	public virtual BitList Or(BitList value)
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

	public virtual BitList Xor(BitList value)
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

	public virtual BitList Not()
	{
		int ints = GetArrayLength(_size, BitsPerInt);
		for (int i = 0; i < ints; i++)
			_items[i] = ~_items[i];
		return this;
	}

	public override Span<bool> AsSpan(int index, int count) => throw new NotSupportedException();

	private protected override void ClearInternal(int index, int count)
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
		Changed();
	}

	public override bool Contains(bool item, int index, int count) => IndexOf(item, index, count) != -1;

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
		(int sourceIntIndex, int sourceBitsIndex) = DivRem(sourceIndex, BitsPerInt);               // Целый индекс в исходном массиве.
		(int destinationIntIndex, int destinationBitsIndex) = DivRem(destinationIndex, BitsPerInt);     // Целый индекс в целевом массиве.
		int bitsOffset = destinationBitsIndex - sourceBitsIndex;    // Битовое смещение.
		int intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		uint sourceStartMask = ~0u << sourceBitsIndex; // Маска "головы" источника
		int sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		(int sourceEndIntIndex, int sourceEndBitsIndex) = DivRem(sourceEndIndex, BitsPerInt);  // Индекс инта последнего бита.
		int destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		(int destinationEndIntIndex, int destinationEndBitsIndex) = DivRem(destinationEndIndex, BitsPerInt);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			uint buff = sourceBits[sourceIntIndex] & sourceStartMask;
			uint destinationMask = ~(~0u << length) << destinationBitsIndex;
			if (bitsOffset >= 0)
				buff <<= bitsOffset;
			else
			{
				buff >>= -bitsOffset;
				if (length + sourceBitsIndex > BitsPerInt)
					buff |= sourceBits[sourceIntIndex + 1] << BitsPerInt + bitsOffset;
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
				for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; destinationCurrentIntIndex < destinationEndIntIndex; sourceCurrentIntIndex++, destinationCurrentIntIndex++)
				{
					buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << BitsPerInt - bitsOffset;
					destinationBits[destinationCurrentIntIndex] = (uint)buff;
					buff >>= BitsPerInt;
				}
				if (sourceEndIntIndex + intOffset != destinationEndIntIndex)
					buff |= (ulong)sourceBits[sourceEndIntIndex] << BitsPerInt - bitsOffset;
				ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
				buff &= destinationMask;
				destinationBits[destinationEndIntIndex] &= (uint)~destinationMask;
				destinationBits[destinationEndIntIndex] |= (uint)buff;
			}
			else
			{
				ulong buff = destinationBits[destinationIntIndex];
				buff &= ((ulong)1 << destinationBitsIndex) - 1;
				buff |= (ulong)(sourceBits[sourceIntIndex] & sourceStartMask) << bitsOffset;
				for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; destinationCurrentIntIndex < destinationEndIntIndex; sourceCurrentIntIndex++, destinationCurrentIntIndex++)
				{
					destinationBits[destinationCurrentIntIndex] = (uint)buff;
					buff >>= BitsPerInt;
					if (sourceCurrentIntIndex < sourceBound) buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << bitsOffset;
				}
				ulong destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
				buff &= destinationMask;
				destinationBits[destinationEndIntIndex] &= (uint)~destinationMask;
				destinationBits[destinationEndIntIndex] |= (uint)buff;
			}
		}
		else
		{
			uint sourceEndMask = (uint)(((ulong)1 << sourceEndBitsIndex + 1) - 1); // Маска "хвоста" источника
			if (bitsOffset < 0)
			{
				bitsOffset = -bitsOffset;
				ulong buff = destinationBits[destinationEndIntIndex];
				buff &= ~0ul << destinationEndBitsIndex + 1;
				buff <<= BitsPerInt;
				buff |= (ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask) << (sourceEndIntIndex + intOffset != destinationEndIntIndex ? BitsPerInt * 2 : BitsPerInt) - bitsOffset;
				for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; destinationCurrentIntIndex > destinationIntIndex; sourceCurrentIntIndex--, destinationCurrentIntIndex--)
				{
					if (sourceEndIntIndex + intOffset != destinationEndIntIndex) buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << BitsPerInt - bitsOffset;
					destinationBits[destinationCurrentIntIndex] = (uint)(buff >> BitsPerInt);
					buff <<= BitsPerInt;
					if (sourceEndIntIndex + intOffset == destinationEndIntIndex) buff |= ((ulong)sourceBits[sourceCurrentIntIndex]) << BitsPerInt - bitsOffset;
				}
				if (sourceEndIntIndex + intOffset != destinationEndIntIndex)
					buff |= (ulong)sourceBits[sourceIntIndex] << BitsPerInt - bitsOffset;
				ulong destinationMask = ~0ul << BitsPerInt + destinationBitsIndex;
				buff &= destinationMask;
				destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> BitsPerInt);
				destinationBits[destinationIntIndex] |= (uint)(buff >> BitsPerInt);
			}
			else
			{
				ulong buff = destinationBits[destinationEndIntIndex];
				buff &= ~0ul << destinationEndBitsIndex + 1;
				buff <<= BitsPerInt;
				buff |= (ulong)(sourceBits[sourceEndIntIndex] & sourceEndMask) << (sourceEndIntIndex + intOffset != destinationEndIntIndex ? 0 : BitsPerInt) + bitsOffset;
				for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; destinationCurrentIntIndex > destinationIntIndex; sourceCurrentIntIndex--, destinationCurrentIntIndex--)
				{
					if (sourceEndIntIndex + intOffset == destinationEndIntIndex) buff |= (ulong)sourceBits[sourceCurrentIntIndex] << bitsOffset;
					destinationBits[destinationCurrentIntIndex] = (uint)(buff >> BitsPerInt);
					buff <<= BitsPerInt;
					if (sourceEndIntIndex + intOffset != destinationEndIntIndex && sourceCurrentIntIndex >= 0) buff |= (ulong)sourceBits[sourceCurrentIntIndex] << bitsOffset;
				}
				ulong destinationMask = ~0ul << BitsPerInt + destinationBitsIndex;
				buff &= destinationMask;
				destinationBits[destinationIntIndex] &= (uint)(~destinationMask >> BitsPerInt);
				destinationBits[destinationIntIndex] |= (uint)(buff >> BitsPerInt);
			}
		}
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

	public virtual object Clone()
	{
		BitList bitList = new(_size, _items);
		return bitList;
	}

	private protected override void Copy(ListBase<bool, BitList> source, int sourceIndex, ListBase<bool, BitList> destination, int destinationIndex, int count)
	{
		BitList source2 = source as BitList ?? throw new ArgumentException(null, nameof(source));
		BitList destination2 = destination as BitList ?? throw new ArgumentException(null, nameof(destination));
		CopyBits(source2._items, source2._capacity, sourceIndex, destination2._items, destination2._capacity, destinationIndex, count);
		destination2.Changed();
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
			throw new ArgumentException(null, nameof(array));
	}

	private void CopyToInternal(bool[] array, int index)
	{
		if (array.Length - index < _size)
			throw new ArgumentException(null);
		CopyToInternal(index, array, 0, array.Length);
	}

	private protected override void CopyToInternal(int index, bool[] array, int arrayIndex, int count)
	{
		for (int i = 0; i < count; i++)
			array[arrayIndex + i] = ((_items[(index + i) / BitsPerInt] >> ((index + i) % BitsPerInt)) & 0x00000001) != 0;
	}

	public override void Dispose()
	{
		Marshal.FreeHGlobal((nint)_items);
		_capacity = 0;
		_size = 0;
		GC.SuppressFinalize(this);
	}

	private protected override bool EqualsInternal(IEnumerable<bool>? collection, int index, bool toEnd = false)
	{
		//try
		//{
		//	throw new ExperimentalException();
		//}
		//catch
		//{
		//}
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is BitList bitList)
		{
			if (index > _size - bitList._size)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (toEnd && index < _size - bitList._size)
				return false;
			if (bitList._size == 0)
				return !toEnd || index == _size;
			(int intIndex, int bitsIndex) = DivRem(index, BitsPerInt);
			uint startMask = ~0u << bitsIndex;
			int endIndex = bitList._size - 1;
			(int endIntIndex, int endBitsIndex) = DivRem(endIndex, BitsPerInt);
			if (endIntIndex == 0)
			{
				uint buff = _items[intIndex] & startMask;
				uint destinationMask = ~(~0u << bitList._size);
				buff >>= bitsIndex;
				if (bitList._size + bitsIndex > BitsPerInt)
					buff |= _items[intIndex + 1] << BitsPerInt - bitsIndex;
				buff &= destinationMask;
				return (bitList._items[0] & destinationMask) == buff;
			}
			else
			{
				ulong buff = bitList._items[0];
				buff |= (ulong)(_items[intIndex] & startMask) >> bitsIndex;
				int sourceEndIntIndex = (index + bitList._size - 1) / BitsPerInt;
				for (int currentIntIndex = intIndex + 1; currentIntIndex <= sourceEndIntIndex; currentIntIndex++)
				{
					buff |= ((ulong)_items[currentIntIndex]) << BitsPerInt - bitsIndex;
					if (bitList._items[currentIntIndex - intIndex - 1] != (uint)buff) return false;
					buff >>= BitsPerInt;
				}
				if (sourceEndIntIndex - intIndex < bitList._capacity)
				{
					ulong destinationMask = ((ulong)1 << endBitsIndex + 1) - 1;
					buff &= destinationMask;
					return (bitList._items[sourceEndIntIndex - intIndex] & (uint)destinationMask) == (uint)buff;
				}
				return true;
			}
		}
		else
		{
			if (collection.TryGetCountEasily(out int count))
			{
				if (index > _size - count)
					throw new ArgumentOutOfRangeException(nameof(index));
				if (toEnd && index < _size - count)
					return false;
			}
			foreach (bool item in collection)
				if (index >= _size || !GetInternal(index++).Equals(item))
					return false;
			return !toEnd || index == _size;
		}
	}

	public override int GetHashCode() => _capacity < 3 ? 1234567890 : _items[0].GetHashCode() ^ _items[1].GetHashCode() ^ _items[_capacity - 1].GetHashCode();

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
		(int quotient2, int remainder2) = DivRem(index + count - 1, BitsPerInt);
		uint result;
		if (quotient == quotient2)
			result = (_items[quotient] >> remainder) & (((uint)1 << count) - 1);
		else
		{
			result = _items[quotient] >> remainder;
			result |= (_items[quotient + 1] & (((uint)1 << remainder2 + 1) - 1)) << BitsPerInt - remainder;
		}
		return result;
	}

	private protected override int IndexOfInternal(bool item, int index, int count)
	{
		int fillValue = item ? 0 : unchecked((int)0xffffffff);
		(int startIndex, int startRemainder) = DivRem(index, BitsPerInt);
		(int endIndex, int endRemainder) = DivRem(index + count, BitsPerInt);
		if (startIndex == endIndex)
		{
			for (int i = startRemainder; i < endRemainder; i++)
				if ((_items[startIndex] & (1 << i)) != 0)
					return startIndex * BitsPerInt + i;
			return -1;
		}
		int invRemainder = BitsPerInt - startRemainder;
		int mask = (1 << invRemainder) - 1;
		uint first = _items[startIndex] >> startRemainder;
		if (first != (item ? 0 : mask))
			for (int i = 0; i < invRemainder; i++)
				if ((first & (1 << i)) != 0)
					return index + i;
		for (int i = startIndex + 1; i < endIndex; i++)
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

	public virtual void InsertRange(int index, IEnumerable collection)
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

	private protected override int LastIndexOfInternal(bool item, int index, int count)
	{
		int fillValue = item ? 0 : unchecked((int)0xffffffff);
		(int startIndex, int startRemainder) = DivRem(index + 1 - count, BitsPerInt);
		(int endIndex, int endRemainder) = DivRem(index + 1, BitsPerInt);
		if (startIndex == endIndex)
		{
			for (int i = endRemainder - 1; i >= startRemainder; i--)
				if ((_items[startIndex] & (1 << i)) != 0)
					return startIndex * BitsPerInt + i;
			return -1;
		}
		if (endRemainder != 0)
			for (int i = endRemainder - 1; i >= 0; i--)
				if ((_items[endIndex] & (1 << i)) != 0)
					return endIndex * BitsPerInt + i;
		for (int i = endIndex - 1; i >= startIndex + 1; i--)
			if (_items[i] != fillValue)
				for (int j = BitsPerInt; j >= 0; j--)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		int invRemainder = BitsPerInt - startRemainder;
		int mask = (1 << invRemainder) - 1;
		uint first = _items[startIndex] >> startRemainder;
		if (first != (item ? 0 : mask))
			for (int i = invRemainder - 1; i >= 0; i--)
				if ((first & (1 << i)) != 0)
					return index + i;
		return -1;
	}

	private protected override BitList ReverseInternal(int index, int count)
	{
		for (int i = 0; i < count / 2; i++)
		{
			(this[index + i], this[index + count - i - 1]) = (this[index + count - i - 1], this[index + i]);
		}
		Changed();
		return this;
	}

	public virtual void SetRange(int index, IEnumerable collection)
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

	public virtual List<uint> ToUIntList()
	{
		int length = GetArrayLength(_size, BitsPerInt);
		List<uint> result = new(length);
		for (int i = 0; i < length; i++)
			result.Add(_items[i]);
		return result;
	}

	public virtual object SyncRoot
	{
		get
		{
			if (_syncRoot == null)
				Interlocked.CompareExchange<object?>(ref _syncRoot, new(), null);
			return _syncRoot;
		}
	}

	public virtual bool IsReadOnly => false;

	public virtual bool IsSynchronized => false;
}

[DebuggerDisplay("Length = {Length}")]
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
			fragment = (mpz_t)1 << (((capacity - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
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
			fragment = (mpz_t)1 << (((length - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
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
				BigBitList list = new(bigBitList._size);
				for (int i = 0; i < bigBitList.high.Length; i++)
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
			if (bitList.Length <= CapacityFirstStep)
			{
				low = new(bitList);
				indexDeleted = new(bitList.Length, false);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				int fragment2 = 1 << ((((mpz_t)bitList.Length - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				fragment = fragment2;
				high = new(GetArrayLength(bitList.Length, fragment2));
				int index = 0;
				for (; index <= bitList.Length - fragment2; index += fragment2)
					high.Add(new(bitList.GetRange(index, fragment2)));
				if (bitList.Length % fragment2 != 0)
					high.Add(new(bitList.GetRange(index, bitList.Length - index)));
				isHigh = true;
			}
			_size = bitList.Length;
			_capacity = _size;
		}
		else if (bits is BigList<uint> bigUIntList)
		{
			mpz_t count = bigUIntList.Length;
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
				int fragment2 = 1 << (((count - 1).BitLength + ((mpz_t)BitsPerInt - 1).BitLength + base.CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / base.CapacityStepBitLength - 1) * base.CapacityStepBitLength + CapacityFirstStepBitLength;
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
					if (!(b = en.MoveNext())) break;
					value |= (uint)en.Current << BitsPerByte * i;
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

	private protected override Func<mpz_t, BigBitList> CapacityCreator => x => new(x);

	private protected override int CapacityFirstStepBitLength => 24;

	private protected override Func<int, BitList> CapacityLowCreator => x => new(x);

	private protected override Func<IEnumerable<bool>, BigBitList> CollectionCreator => x => new(x);

	private protected override Func<IEnumerable<bool>, BitList> CollectionLowCreator => x => new(x);

	private protected override int DefaultCapacity => 256;

	public virtual void SetAll(bool value)
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

	public virtual BigBitList And(BigBitList value)
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

	public virtual BigBitList Or(BigBitList value)
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

	public virtual BigBitList Xor(BigBitList value)
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

	public virtual BigBitList Not()
	{
		if (!isHigh && low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual void AddRange(IEnumerable bits)
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

	public virtual BigList<uint> ToUIntBigList()
	{
		if (!isHigh && low != null)
			return new(BitList.RemoveIndexes(low, deletedIndexes).ToUIntList());
		else if (high != null)
			return new(high.SelectMany(x => x.ToUIntBigList()));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
}
