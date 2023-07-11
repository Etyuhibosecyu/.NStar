using System.Diagnostics;

namespace Corlib.NStar;

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public unsafe class BitList : BaseList<bool, BitList>, ICloneable
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
		_items = capacity == 0 ? _emptyPointer : (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(capacity, BitsPerInt)));
	}

	public BitList(int length, bool defaultValue)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = GetArrayLength(length, BitsPerInt)));
		_size = length;
		var fillValue = defaultValue ? 0xffffffff : 0;
		for (var i = 0; i < _capacity; i++)
			_items[i] = fillValue;
	}

	public BitList(int length, uint* ptr)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var uints = GetArrayLength(length, BitsPerInt);
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = uints));
		_size = length;
		CopyMemory(ptr, _items, uints);
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

	public BitList(int length, params uint[] values)
	{
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var uints = GetArrayLength(length, BitsPerInt);
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = uints));
		_size = length;
		fixed (uint* ptr = values)
			CopyMemory(ptr, _items, uints);
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
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		var uints = GetArrayLength(length, BitsPerInt);
		_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * (_capacity = uints));
		_size = length;
		fixed (uint* ptr = values)
			CopyMemory(ptr, _items, uints);
	}

	public BitList(IEnumerable bits)
	{
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BitList bitList)
		{
			var arrayLength = _capacity = GetArrayLength(bitList._size, BitsPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			_size = bitList._size;
			CopyMemory(bitList._items, _items, arrayLength);
		}
		else if (bits is BitArray bitArray)
		{
			var arrayLength = _capacity = GetArrayLength(bitArray.Length, BitsPerInt);
			var array = new int[arrayLength];
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
			var i = 0;
			var j = 0;
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
			for (var i = 0; i < boolArray.Length; i++)
				if (boolArray[i])
					_items[i / BitsPerInt] |= (uint)1 << i % BitsPerInt;
		}
		else if (bits is IEnumerable<int> ints)
		{
			var uintArray = List<uint>.ToArrayEnumerable(ints, x => (uint)x);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * uintArray.Length);
			fixed (uint* ptr = uintArray)
				CopyMemory(ptr, _items, uintArray.Length);
			_size = (_capacity = uintArray.Length) * BitsPerInt;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			var uintArray = List<uint>.ToArrayEnumerable(uints);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * uintArray.Length);
			fixed (uint* ptr = uintArray)
				CopyMemory(ptr, _items, uintArray.Length);
			_size = (_capacity = uintArray.Length) * BitsPerInt;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			if (!List<byte>.TryGetLengthEasilyEnumerable(bytes, out var length))
				length = bytes.Length();
			if (length > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bits));
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
		else if (bits is IEnumerable<bool> bools)
		{
			if (!List<bool>.TryGetLengthEasilyEnumerable(bools, out var length))
				length = bools.Length();
			var arrayLength = _capacity = GetArrayLength(length, BitsPerInt);
			_items = (uint*)Marshal.AllocHGlobal(sizeof(uint) * arrayLength);
			FillMemory(_items, arrayLength, 0);
			var i = 0;
			using var en = bools.GetEnumerator();
			while (en.MoveNext())
			{
				(var index, var remainder) = DivRem(i++, BitsPerInt);
				if (en.Current)
					_items[index] |= (uint)1 << remainder;
			}
			_size = length;
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
			var newints = GetArrayLength(value, BitsPerInt);
			if (newints == _capacity)
				return;
			if (newints > _capacity || newints + _shrinkThreshold < _capacity)
			{
				var newarray = (uint*)Marshal.AllocHGlobal(sizeof(uint) * newints);
				CopyMemory(_items, newarray, Min(_capacity, newints));
				Marshal.FreeHGlobal((nint)_items);
				_items = newarray;
				_capacity = newints;
			}
			if (value > _size)
			{
				(var last, var remainder) = DivRem(_size, BitsPerInt);
				_items[last] &= ((uint)1 << remainder) - 1;
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

	private static Func<int, BitList> CapacityCreatorStatic => x => new(x);

	private protected override Func<IEnumerable<bool>, BitList> CollectionCreator => CollectionCreatorStatic;

	private static Func<IEnumerable<bool>, BitList> CollectionCreatorStatic => x => new(x);

	private protected override int DefaultCapacity => 256;

	public virtual BitList And(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] &= value._items[i];
		return this;
	}

	public override Span<bool> AsSpan(int index, int length) => throw new NotSupportedException();

	private protected override void ClearInternal(int index, int length)
	{
		(var startIndex, var startRemainder) = DivRem(index, BitsPerInt);
		var startMask = ((uint)1 << startRemainder) - 1;
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
		(var sourceIntIndex, var sourceBitsIndex) = DivRem(sourceIndex, BitsPerInt);               // Целый индекс в исходном массиве.
		(var destinationIntIndex, var destinationBitsIndex) = DivRem(destinationIndex, BitsPerInt);     // Целый индекс в целевом массиве.
		var bitsOffset = destinationBitsIndex - sourceBitsIndex;    // Битовое смещение.
		var intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		var sourceStartMask = ~0u << sourceBitsIndex; // Маска "головы" источника
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		(var sourceEndIntIndex, var sourceEndBitsIndex) = DivRem(sourceEndIndex, BitsPerInt);  // Индекс инта последнего бита.
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		(var destinationEndIntIndex, var destinationEndBitsIndex) = DivRem(destinationEndIndex, BitsPerInt);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			var buff = sourceBits[sourceIntIndex] & sourceStartMask;
			var destinationMask = ~(length == BitsPerInt ? 0 : ~0u << length) << destinationBitsIndex;
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
				var destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
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
				var destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
				buff &= destinationMask;
				destinationBits[destinationEndIntIndex] &= (uint)~destinationMask;
				destinationBits[destinationEndIntIndex] |= (uint)buff;
			}
		}
		else
		{
			var sourceEndMask = (uint)(((ulong)1 << sourceEndBitsIndex + 1) - 1); // Маска "хвоста" источника
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
				var destinationMask = ~0ul << BitsPerInt + destinationBitsIndex;
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
				var destinationMask = ~0ul << BitsPerInt + destinationBitsIndex;
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

	private protected override void Copy(BitList source, int sourceIndex, BitList destination, int destinationIndex, int length)
	{
		CopyBits(source._items, source._capacity, sourceIndex, destination._items, destination._capacity, destinationIndex, length);
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
			throw new ArgumentException(null, nameof(array));
	}

	private void CopyToInternal(bool[] array, int index)
	{
		if (array.Length - index < _size)
			throw new ArgumentException(null);
		CopyToInternal(index, array, 0, array.Length);
	}

	private protected override void CopyToInternal(int index, bool[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
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
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if (collection is BitList bitList)
		{
			if (index > _size - bitList._size)
				return false;
			if (toEnd && index < _size - bitList._size)
				return false;
			if (bitList._size == 0)
				return !toEnd || index == _size;
			(var intIndex, var bitsIndex) = DivRem(index, BitsPerInt);
			var startMask = ~0u << bitsIndex;
			var destinationEndIndex = bitList._size - 1;
			(var destinationEndIntIndex, var destinationEndBitsIndex) = DivRem(destinationEndIndex, BitsPerInt);
			if (destinationEndIntIndex == 0)
			{
				var buff = _items[intIndex] & startMask;
				var destinationMask = ~(bitList._size == BitsPerInt ? 0 : ~0u << bitList._size);
				buff >>= bitsIndex;
				if (bitList._size + bitsIndex > BitsPerInt)
					buff |= _items[intIndex + 1] << BitsPerInt - bitsIndex;
				buff &= destinationMask;
				return (bitList._items[0] & destinationMask) == buff;
			}
			else
			{
				var buff = (ulong)(_items[intIndex] & startMask) >> bitsIndex;
				var sourceEndIntIndex = (index + bitList._size - 1) / BitsPerInt;
				for (int sourceCurrentIntIndex = intIndex + 1, destinationCurrentIntIndex = 0; destinationCurrentIntIndex < destinationEndIntIndex; sourceCurrentIntIndex++, destinationCurrentIntIndex++)
				{
					buff |= ((ulong)_items[sourceCurrentIntIndex]) << BitsPerInt - bitsIndex;
					if (bitList._items[destinationCurrentIntIndex] != (uint)buff) return false;
					buff >>= BitsPerInt;
				}
				if (sourceEndIntIndex - intIndex != destinationEndIntIndex)
					buff |= (ulong)_items[sourceEndIntIndex] << BitsPerInt - bitsIndex;
				var destinationMask = ((ulong)1 << destinationEndBitsIndex + 1) - 1;
				buff &= destinationMask;
				return (bitList._items[destinationEndIntIndex] & destinationMask) == buff;
			}
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
				if (index >= _size || !GetInternal(index++).Equals(item))
					return false;
			return !toEnd || index == _size;
		}
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
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return new();
		if (length > BitsPerInt)
			throw new ArgumentException(null, nameof(length));
		(var quotient, var remainder) = DivRem(index, BitsPerInt);
		(var quotient2, var remainder2) = DivRem(index + length - 1, BitsPerInt);
		uint result;
		if (quotient == quotient2)
			result = (_items[quotient] >> remainder) & ~(length == BitsPerInt ? 0 : ~0u << length);
		else
		{
			result = _items[quotient] >> remainder;
			result |= (_items[quotient + 1] & (((uint)1 << remainder2 + 1) - 1)) << BitsPerInt - remainder;
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
				for (var j = 0; j < BitsPerInt; j++)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		if (endRemainder != 0)
			for (var i = 0; i < endRemainder; i++)
				if ((_items[endIndex] & (1 << i)) == 0 ^ item)
					return endIndex * BitsPerInt + i;
		return -1;
	}

	public virtual BitList Insert(int index, IEnumerable collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is BitList bitList)
		{
			var length = bitList._size;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			CopyBits(_items, _capacity, index, _items, _capacity, index + length, _size - index);
			CopyBits(bitList._items, bitList._capacity, 0, _items, _capacity, index, length);
			_size += length;
			return this;
		}
		else if (collection is IEnumerable<bool> bools)
			return InsertInternal(index, bools);
		else if (collection is uint[] uintArray)
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
			return Insert(index, new BitList(collection));
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
				for (var j = BitsPerInt; j >= 0; j--)
					if ((_items[i] & (1 << j)) != 0)
						return i * BitsPerInt + j;
		var invRemainder = BitsPerInt - startRemainder;
		var mask = (1 << invRemainder) - 1;
		var first = _items[startIndex] >> startRemainder;
		if (first != (item ? 0 : mask))
			for (var i = invRemainder - 1; i >= 0; i--)
				if ((first & (1 << i)) == 0 ^ item)
					return index + i;
		return -1;
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
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] |= value._items[i];
		return this;
	}

	private protected override BitList ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
			(this[index + i], this[index + length - i - 1]) = (this[index + length - i - 1], this[index + i]);
		Changed();
		return this;
	}

	public override BitList SetAll(bool value, int index, int length)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > _size)
			throw new ArgumentException(null);
		if (length == 0)
			return this;
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
			var endMask = endBitsIndex == BitsPerInt - 1 ? 0xffffffff : ((uint)1 << endBitsIndex + 1) - 1;
			_items[endIntIndex] = value ? _items[endIntIndex] | endMask : _items[endIntIndex] & ~endMask;
		}
		return this;
	}

	internal override void SetInternal(int index, bool value)
	{
		if (value)
			_items[index / BitsPerInt] |= (uint)1 << index % BitsPerInt;
		else
			_items[index / BitsPerInt] &= ~((uint)1 << index % BitsPerInt);
		Changed();
	}

	public virtual BitList SetRange(int index, IEnumerable collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is BitList bitList)
		{
			var length = bitList._size;
			if (index + length > _size)
				throw new ArgumentException(null);
			if (length > 0)
				CopyBits(bitList._items, bitList._capacity, 0, _items, _capacity, index, length);
		}
		else
			SetRange(index, new BitList(collection));
		return this;
	}

	public virtual List<uint> ToUIntList()
	{
		var length = GetArrayLength(_size, BitsPerInt);
		List<uint> result = new(length);
		for (var i = 0; i < length; i++)
			result.Add(_items[i]);
		return result;
	}

	public virtual BitList Xor(BitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (_size != value._size)
			throw new ArgumentException(null, nameof(value));
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] ^= value._items[i];
		return this;
	}
}

[DebuggerDisplay("Length = {Length}")]
[ComVisible(true)]
[Serializable]
public class BigBitList : BigList<bool, BigBitList, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private const int BitsPerInt = sizeof(int) * BitsPerByte;
	private const int BytesPerInt = sizeof(int);
	private const int BitsPerByte = 8;

	public BigBitList() : this(-1) { }

	public BigBitList(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigBitList(mpz_t capacity, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacity, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigBitList(mpz_t length, bool defaultValue, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (length <= CapacityFirstStep)
		{
			low = new((int)length, defaultValue);
			high = null;
			highCapacity = null;
			isHigh = false;

		}
		else
		{
			low = null;
			fragment = (mpz_t)1 << (GetArrayLength((length - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			high = new((int)GetArrayLength(length, fragment));
			highCapacity = new();
			for (mpz_t i = 0; i < length / fragment; i++)
			{
				high.Add(new(fragment, defaultValue, CapacityStepBitLength, CapacityFirstStepBitLength));
				highCapacity.Add(fragment);
			}
			if (length % fragment != 0)
			{
				high.Add(new(length % fragment, defaultValue, CapacityStepBitLength, CapacityFirstStepBitLength));
				highCapacity.Add(length % fragment);
			}
			isHigh = true;
		}
		Size = length;
		_capacity = Size;
	}

	public BigBitList(IEnumerable bits, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BigBitList bigBitList)
		{
			if (!bigBitList.isHigh && bigBitList.low != null)
			{
				low = new(bigBitList.low);
				high = null;
				highCapacity = null;
				Size = bigBitList.Size;
				_capacity = bigBitList._capacity;
				fragment = bigBitList.fragment;
				isHigh = false;
			}
			else if (bigBitList.high != null)
			{
				BigBitList list = new(bigBitList.Size, CapacityStepBitLength, CapacityFirstStepBitLength);
				Copy(bigBitList, 0, list, 0, bigBitList.Size);
				low = list.low;
				high = list.high;
				highCapacity = list.highCapacity;
				Size = list.Size;
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
				high = null;
				highCapacity = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << ((((mpz_t)bitList.Length - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var fragment2 = (int)ProperFragment;
				high = new(GetArrayLength(bitList.Length, fragment2));
				highCapacity = new();
				var index = 0;
				for (; index <= bitList.Length - fragment2; index += fragment2)
				{
					high.Add(new(bitList.GetRange(index, fragment2), CapacityStepBitLength, CapacityFirstStepBitLength));
					highCapacity.Add(fragment2);
				}
				if (bitList.Length % fragment2 != 0)
				{
					high.Add(new(bitList.GetRange(index), CapacityStepBitLength, CapacityFirstStepBitLength));
					highCapacity.Add(bitList.Length - index);
				}
				isHigh = true;
			}
			Size = bitList.Length;
			_capacity = Size;
		}
		else if (bits is BigList<uint> bigUIntList)
		{
			var length = bigUIntList.Length;
			if (length <= CapacityFirstStep / BitsPerInt)
			{
				low = new(bigUIntList);
				high = null;
				highCapacity = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << (((length - 1).BitLength + ((mpz_t)BitsPerInt - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var fragment2 = (int)ProperFragment;
				var uintsFragment = fragment2 / BitsPerInt;
				high = new((int)((length + uintsFragment - 1) / uintsFragment));
				highCapacity = new();
				var index = 0;
				for (; index <= length - uintsFragment; index += uintsFragment)
				{
					high.Add(new(bigUIntList.GetRange(index, uintsFragment), CapacityStepBitLength, CapacityFirstStepBitLength));
					highCapacity.Add(fragment2);
				}
				if (index != length)
				{
					high.Add(new(bigUIntList.GetRange(index, length - index), CapacityStepBitLength, CapacityFirstStepBitLength));
					highCapacity.Add((length - index) * BitsPerInt);
				}
				isHigh = true;
			}
			Size = length * BitsPerInt;
			_capacity = Size;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			BigBitList list = new(new BigList<uint>(uints), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = list.low;
			high = list.high;
			highCapacity = list.highCapacity;
			Size = list.Size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else if (bits is IEnumerable<int> ints)
		{
			BigBitList list = new(new BigList<uint>(ints.Select(x => (uint)x)), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = list.low;
			high = list.high;
			highCapacity = list.highCapacity;
			Size = list.Size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			var b = true;
			var en = bytes.GetEnumerator();
			BigList<uint> values = new();
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
			BigBitList list = new(values, CapacityStepBitLength, CapacityFirstStepBitLength);
			low = list.low;
			high = list.high;
			highCapacity = list.highCapacity;
			Size = n * 8;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
			for (var target = this; target.high != null && target.highCapacity != null;)
			{
				var newSize = n * BitsPerByte % target.fragment;
				target.highCapacity[^1] = newSize;
				target = target.high[^1];
				target.Size = newSize;
			}
		}
		else if (bits is BitArray or byte[] or bool[])
		{
			BigBitList list = new(new BitList(bits), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = list.low;
			high = list.high;
			highCapacity = list.highCapacity;
			Size = list.Size;
			_capacity = list._capacity;
			fragment = list.fragment;
			isHigh = list.isHigh;
		}
		else if (bits is IEnumerable<bool> bools)
		{
			var en = bools.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
		else
			throw new ArgumentException(null, nameof(bits));
	}

	private protected override Func<mpz_t, BigBitList> CapacityCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override int CapacityFirstStepBitLength { get; init; } = 20;

	private protected override Func<int, BitList> CapacityLowCreator => x => new(x);

	private protected override Func<IEnumerable<bool>, BitList> CollectionLowCreator => x => new(x);

	private protected override Func<IEnumerable<bool>, BigBitList> CollectionCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override int DefaultCapacity => 256;

	public virtual void AddRange(IEnumerable bits)
	{
		if (bits is IEnumerable<bool> bools)
			base.AddRange(bools);
		else
			AddRange(new BigBitList(bits, CapacityStepBitLength, CapacityFirstStepBitLength));
	}

	public virtual BigBitList And(BigBitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.And(value.low);
		else high = high != null && value.high != null
			? high.Combine(value.high, (x, y) => x.And(y))
			:            throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual uint GetSmallRange(mpz_t index, int length)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		if (length == 0)
			return 0;
		if (length > BitsPerInt)
			throw new ArgumentException(null, nameof(length));
		if (!isHigh && low != null)
			return low.GetSmallRange((int)index, length);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)(index + length - 1).Divide(fragment, out var remainder2);
			uint result;
			if (quotient == quotient2)
				result = high[quotient].GetSmallRange(remainder, length);
			else
			{
				result = high[quotient].GetSmallRange(remainder, (int)(fragment - remainder));
				result |= high[quotient + 1].GetSmallRange(0, (int)remainder2) << (int)(BitsPerInt - remainder);
			}
			return result;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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

	public virtual BigBitList Or(BigBitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Or(value.low);
		else high = high != null && value.high != null
			? high.Combine(value.high, (x, y) => x.Or(y))
			:         throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual void SetAll(bool value)
	{
		if (!isHigh && low != null)
			low.SetAll(value);
		else if (high != null)
			high.ForEach(x => x.SetAll(value));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigList<uint> ToUIntBigList() => !isHigh && low != null
			? new(low.ToUIntList())
			: high != null
			? new(high.SelectMany(x => x.ToUIntBigList()))
			: throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");

	public virtual BigBitList Xor(BigBitList value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Xor(value.low);
		else high = high != null && value.high != null
			? high.Combine(value.high, (x, y) => x.Xor(y))
			:            throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}
}
