using System.Reflection;
using System.Runtime.CompilerServices;

namespace NStar.Core;

/// <summary>
/// Представляет компактный список значений битов, которые представлены в виде логических значений, где
/// true указывает, что бит включен (1), а false указывает, что бит выключен (0).
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public unsafe class BitList : BaseList<bool, BitList>
{
	private protected uint[]? _items;

	private protected const int _shrinkThreshold = 256;

	private protected static readonly uint[]? _emptyPointer = null;

	public BitList()
	{
		_items = _emptyPointer;
		_size = 0;
	}

	public BitList(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		_items = capacity == 0 ? _emptyPointer : GC.AllocateUninitializedArray<uint>(GetArrayLength(capacity, BitsPerInt));
	}

	public BitList(int length, bool defaultValue)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		_items = GC.AllocateUninitializedArray<uint>(GetArrayLength(length, BitsPerInt));
		_size = length;
		var fillValue = defaultValue ? 0xffffffff : 0;
		for (var i = 0; i < _items.Length; i++)
			_items[i] = fillValue;
	}

	public BitList(int length, uint* ptr)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = GetArrayLength(length, BitsPerInt);
		_items = GC.AllocateUninitializedArray<uint>(uints);
		_size = length;
		fixed (uint* itemsPtr = _items)
			CopyMemory(ptr, itemsPtr, uints);
	}

	public BitList(int[] values) : this(values.AsSpan()) { }

	public BitList(int length, params int[] values) : this(length, values.AsSpan()) { }

	public BitList(uint[] values) : this(values.AsSpan()) { }

	public BitList(int length, params uint[] values) : this(length, values.AsSpan()) { }

	public BitList(bool[] values) : this(values.AsSpan()) { }

	public BitList(int length, params bool[] values) : this(length, values.AsSpan()) { }

	public BitList(ReadOnlySpan<int> values)
	{
		// this value is chosen to prevent overflow when computing m_length
		if (values.Length > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = GC.AllocateUninitializedArray<uint>(values.Length);
		_size = values.Length * BitsPerInt;
		fixed (int* ptr = values)
		fixed (uint* itemsPtr = _items)
			CopyMemory((uint*)ptr, itemsPtr, values.Length);
	}

	public BitList(int length, ReadOnlySpan<int> values)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = Max(GetArrayLength(length, BitsPerInt), values.Length);
		// this value is chosen to prevent overflow when computing m_length
		if (uints > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = GC.AllocateUninitializedArray<uint>(uints);
		if (uints > values.Length)
			fixed (uint* itemsPtr = _items)
				FillMemory(itemsPtr + values.Length, uints - values.Length, 0);
		_size = length;
		fixed (int* ptr = values)
		fixed (uint* itemsPtr = _items)
			CopyMemory((uint*)ptr, itemsPtr, values.Length);
	}

	public BitList(ReadOnlySpan<uint> values)
	{
		// this value is chosen to prevent overflow when computing m_length
		if (values.Length > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = GC.AllocateUninitializedArray<uint>(values.Length);
		_size = values.Length * BitsPerInt;
		fixed (uint* ptr = values)
		fixed (uint* itemsPtr = _items)
			CopyMemory(ptr, itemsPtr, values.Length);
	}

	public BitList(int length, ReadOnlySpan<uint> values)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = Max(GetArrayLength(length, BitsPerInt), values.Length);
		// this value is chosen to prevent overflow when computing m_length
		if (uints > int.MaxValue / BitsPerInt)
			throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(values));
		_items = GC.AllocateUninitializedArray<uint>(uints);
		if (uints > values.Length)
			fixed (uint* itemsPtr = _items)
				FillMemory(itemsPtr + values.Length, uints - values.Length, 0);
		_size = length;
		fixed (uint* ptr = values)
		fixed (uint* itemsPtr = _items)
			CopyMemory(ptr, itemsPtr, values.Length);
	}

	public BitList(ReadOnlySpan<bool> values)
	{
		_size = values.Length;
		var uints = GetArrayLength(_size, BitsPerInt);
		// this value is chosen to prevent overflow when computing m_length
		_items = GC.AllocateUninitializedArray<uint>(uints);
		fixed (uint* itemsPtr = _items)
			FillMemory(itemsPtr, uints, 0);
		var (quotient, remainder) = DivRem(_size, BitsPerInt);
		var index = 0;
		for (var i = 0; i < quotient; i++)
			for (var j = 0; j < BitsPerInt; j++)
				if (values[index++])
					_items[i] |= 1u << j;
		for (var i = 0; i < remainder; i++)
			if (values[index++])
				_items[quotient] |= 1u << i;
	}

	public BitList(int length, ReadOnlySpan<bool> values)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var uints = GetArrayLength(Max(length, values.Length), BitsPerInt);
		// this value is chosen to prevent overflow when computing m_length
		_items = GC.AllocateUninitializedArray<uint>(uints);
		fixed (uint* itemsPtr = _items)
			FillMemory(itemsPtr, uints, 0);
		var (quotient, remainder) = DivRem(values.Length, BitsPerInt);
		var index = 0;
		for (var i = 0; i < quotient; i++)
			for (var j = 0; j < BitsPerInt; j++)
				if (values[index++])
					_items[i] |= 1u << j;
		for (var i = 0; i < remainder; i++)
			if (values[index++])
				_items[quotient] |= 1u << i;
		_size = length;
	}

	public BitList(BitArray bitArray)
	{
		if (bitArray is null)
			throw new ArgumentNullException(nameof(bitArray));
		else
		{
			var arrayLength = GetArrayLength(bitArray.Length, BitsPerInt);
			var array = new int[arrayLength];
			bitArray.CopyTo(array, 0);
			_items = GC.AllocateUninitializedArray<uint>(arrayLength);
			fixed (int* array2 = array)
			fixed (uint* itemsPtr = _items)
				CopyMemory((uint*)array2, itemsPtr, arrayLength);
			_size = bitArray.Length;
		}
	}

	public BitList(IEnumerable<byte> bytes)
	{
		if (bytes is null)
			throw new ArgumentNullException(nameof(bytes));
		else if (bytes is byte[] byteArray)
		{
			if (byteArray.Length > int.MaxValue / BitsPerByte)
				throw new ArgumentException("Длина коллекции превышает диапазон допустимых значений.", nameof(bytes));
			_items = GC.AllocateUninitializedArray<uint>(GetArrayLength(byteArray.Length, BytesPerInt));
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
			var arrayLength = GetArrayLength(length, BytesPerInt);
			_items = GC.AllocateUninitializedArray<uint>(arrayLength);
			fixed (uint* itemsPtr = _items)
				FillMemory(itemsPtr, arrayLength, 0);
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
		if (bools is null)
			throw new ArgumentNullException(nameof(bools));
		else if (bools is BitList bitList)
		{
			var arrayLength = GetArrayLength(bitList._size, BitsPerInt);
			_items = GC.AllocateUninitializedArray<uint>(arrayLength);
			_size = bitList._size;
			if (bitList._items is not null)
				Array.Copy(bitList._items, _items, arrayLength);
		}
		else if (bools is bool[] boolArray)
		{
			_items = GC.AllocateUninitializedArray<uint>(GetArrayLength(boolArray.Length, BitsPerInt));
			_size = boolArray.Length;
			fixed (uint* itemsPtr = _items)
				FillMemory(itemsPtr, _items.Length, 0);
			for (var i = 0; i < boolArray.Length; i++)
				if (boolArray[i])
					_items[i / BitsPerInt] |= 1u << i % BitsPerInt;
		}
		else
		{
			var length = bools.Length();
			var arrayLength = GetArrayLength(length, BitsPerInt);
			_items = GC.AllocateUninitializedArray<uint>(arrayLength);
			fixed (uint* itemsPtr = _items)
				FillMemory(itemsPtr, arrayLength, 0);
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
		if (ints is null)
			throw new ArgumentNullException(nameof(ints));
		else if (ints is int[] intArray)
		{
			_items = GC.AllocateUninitializedArray<uint>(intArray.Length);
			fixed (int* ptr = intArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory((uint*)ptr, itemsPtr, intArray.Length);
			_size = intArray.Length * BitsPerInt;
		}
		else
		{
			var toArray = RedStarLinq.ToArray(ints);
			_items = GC.AllocateUninitializedArray<uint>(toArray.Length);
			fixed (int* ptr = toArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory((uint*)ptr, itemsPtr, toArray.Length);
			_size = toArray.Length * BitsPerInt;
		}
	}

	public BitList(IEnumerable<uint> uints)
	{
		if (uints is null)
			throw new ArgumentNullException(nameof(uints));
		else if (uints is uint[] uintArray)
		{
			_items = GC.AllocateUninitializedArray<uint>(uintArray.Length);
			fixed (uint* ptr = uintArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory(ptr, itemsPtr, uintArray.Length);
			_size = uintArray.Length * BitsPerInt;
		}
		else
		{
			var toArray = RedStarLinq.ToArray(uints);
			_items = GC.AllocateUninitializedArray<uint>(toArray.Length);
			fixed (uint* ptr = toArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory(ptr, itemsPtr, toArray.Length);
			_size = toArray.Length * BitsPerInt;
		}
	}

	public override int Capacity
	{
		get => (_items?.Length ?? 0) * BitsPerInt;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			var oldLength = _items?.Length ?? 0;
			var newints = GetArrayLength(value, BitsPerInt);
			if (newints == oldLength)
				return;
			if (newints > oldLength || newints + _shrinkThreshold < oldLength)
			{
				var newarray = GC.AllocateUninitializedArray<uint>(newints);
				if (_items is not null)
					Array.Copy(_items, newarray, Min(oldLength, newints));
				_items = newarray;
			}
			if (value > _size)
			{
				(var last, var remainder) = DivRem(_size, BitsPerInt);
				if (_items is not null)
					_items[last] &= (1u << remainder) - 1;
				fixed (uint* itemsPtr = _items)
					FillMemory(itemsPtr + (last + 1), newints - last - 1, 0);
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
			if (_syncRoot is null)
				Interlocked.CompareExchange<object?>(ref _syncRoot, new(), null);
			return _syncRoot;
		}
	}

	protected override Func<int, BitList> CapacityCreator { get; } = x => new(x);

	protected override Func<IEnumerable<bool>, BitList> CollectionCreator { get; } = x => new(x);

	protected override int DefaultCapacity => 256;

	protected override Func<ReadOnlySpan<bool>, BitList> SpanCreator { get; } = x => new(x);

	public virtual BitList AddRange(BitArray bitArray) => Insert(_size, bitArray);

	public virtual BitList AddRange(IEnumerable<byte> bytes) => Insert(_size, bytes);

	public virtual BitList AddRange(IEnumerable<int> ints) => Insert(_size, ints);

	public virtual BitList AddRange(IEnumerable<uint> uints) => Insert(_size, uints);

	public virtual BitList AddRange(ReadOnlySpan<int> ints) => Insert(_size, ints);

	public virtual BitList AddRange(ReadOnlySpan<uint> uints) => Insert(_size, uints);

	public virtual BitList And(BitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (_size != value._size)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (_items is null || value._items is null)
			return this;
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] &= value._items[i];
		Changed();
		return this;
	}

	public override Memory<bool> AsMemory(int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
		+ " Используйте GetSlice() вместо него.");

	public override Span<bool> AsSpan(int index, int length) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
		+ " Используйте GetSlice() вместо него.");

	protected override void ClearInternal(int index, int length)
	{
		if (_items is null)
			return;
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
			if (endIndex != _items.Length)
				_items[endIndex] &= endMask;
		}
	}

	public override bool Contains(bool item, int index, int length) => IndexOf(item, index, length) != -1;

	protected static void CopyBits(uint* sourceBits, int sourceBound, int sourceIndex, uint* destinationBits, int destinationBound, int destinationIndex, int length)
	{
		CheckParams(sourceBits, sourceBound, sourceIndex, destinationBits, destinationBound, destinationIndex, length);
		if (length == 0)
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		CopyBitsSide source = (sourceIndex, sourceIndex + length - 1);
		CopyBitsSide destination = (destinationIndex, destinationIndex + length - 1);
		CopyBitsIndex offset = new(destination.Start.IntIndex - source.Start.IntIndex, destination.Start.BitsIndex - source.Start.BitsIndex);
		var sourceStartMask = ~0u << source.Start.BitsIndex;
		if (destination.End.IntIndex == destination.Start.IntIndex)
			CopyBitsOneInt(sourceBits, destinationBits, length, source, destination, offset, sourceStartMask);
		else if (sourceIndex >= destinationIndex)
			CopyBitsForward(sourceBits, sourceBound, destinationBits, source, destination, offset, sourceStartMask);
		else
			CopyBitsBackward(sourceBits, destinationBits, source, destination, offset);
	}

	private protected static void CopyBitsBackward(uint* sourceBits, uint* destinationBits, CopyBitsSide source, CopyBitsSide destination, CopyBitsIndex offset)
	{
		var sourceEndMask = (uint)(((ulong)1 << source.End.BitsIndex + 1) - 1);
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
		if (sourceBits is null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBound == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits is null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBound == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		// Длина массивов в битах.
		var sourceLengthBits = sourceBound * BitsPerInt;
		var destinationLengthBits = destinationBound * BitsPerInt;
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

	protected override void CopyToInternal(int sourceIndex, BitList destination, int destinationIndex, int length)
	{
		Debug.Assert(_items is not null);
		Debug.Assert(destination._items is not null);
		fixed (uint* itemsPtr = _items)
		fixed (uint* destinationItemsPtr = destination._items)
			CopyBits(itemsPtr, _items.Length, sourceIndex, destinationItemsPtr, destination._items.Length, destinationIndex, length);
		if (destination._size < destinationIndex + length)
			destination._size = destinationIndex + length;
	}

	protected override void CopyToInternal(Array array, int index)
	{
		if (array.Rank != 1)
			throw new RankException("Массив должен иметь одно измерение.");
		if (array is int[] intArray)
			fixed (int* ptr = intArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory(itemsPtr, (uint*)(ptr + index), GetArrayLength(_size, BitsPerInt));
		else if (array is uint[] uintArray)
			fixed (uint* ptr = uintArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory(itemsPtr, ptr + index, GetArrayLength(_size, BitsPerInt));
		else if (array is byte[] byteArray)
			fixed (byte* ptr = byteArray)
			fixed (uint* itemsPtr = _items)
				CopyMemory((byte*)itemsPtr, ptr + index, GetArrayLength(_size, BitsPerByte));
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

	protected override void CopyToInternal(int index, bool[] array, int arrayIndex, int length)
	{
		if (_items is null)
			return;
		for (var i = 0; i < length; i++)
			array[arrayIndex + i] = ((_items[(index + i) / BitsPerInt] >> ((index + i) % BitsPerInt)) & 0x00000001) != 0;
	}

	public override void Dispose()
	{
		_items = null;
		_size = 0;
		Changed();
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is null)
			return false;
		else if (obj is BitArray bitArray)
		{
			if (typeof(BitArray).GetField("_array", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(bitArray)
				is not byte[] array)
				throw new InvalidOperationException("Не удалось сравнить этот список бит со стандартным массивом бит"
					+ " из-за ошибки в имени или типе поля. Обратитесь к разработчикам фреймворка .NStar.");
			if (_size != bitArray.Length)
				return false;
			var (quotient, remainder) = DivRem(_size, BitsPerByte);
			fixed (uint* itemsPtr = _items)
			{
				if (new Span<byte>(itemsPtr, quotient).CommonPrefixLength(array) != quotient)
					return false;
				if (remainder == 0)
					return true;
				return array.Length > quotient && (((byte*)itemsPtr)[quotient] & CreateVar((1 << remainder) - 1, out var mask))
					== (array[quotient] & mask);
			}
		}
		else if (obj is IEnumerable<bool> bools)
			return Equals(bools);
		else if (obj is IEnumerable<byte> bytes)
			return EqualsToBytes(bytes);
		else if (obj is IEnumerable<int> ints)
		{
			if (_items is null)
				return !ints.Any();
			var (quotient, remainder) = DivRem(_size, BitsPerInt);
			var en = ints.GetEnumerator();
			for (var i = 0; i < quotient; i++)
				if (!en.MoveNext() || _items[i] != (uint)en.Current)
					return false;
			if (remainder == 0)
				return true;
			return en.MoveNext() && (_items[quotient] & CreateVar((1 << remainder) - 1, out var mask)) == (en.Current & mask);
		}
		else if (obj is IEnumerable<uint> uints)
		{
			if (_items is null)
				return !uints.Any();
			var (quotient, remainder) = DivRem(_size, BitsPerInt);
			var en = uints.GetEnumerator();
			for (var i = 0; i < quotient; i++)
				if (!en.MoveNext() || _items[i] != en.Current)
					return false;
			if (remainder == 0)
				return true;
			return en.MoveNext() && (_items[quotient] & CreateVar((1 << remainder) - 1, out var mask)) == (en.Current & mask);
		}
		else if (obj is IEquatable<BitArray> iqba)
		{
			if (_items is null)
				return iqba.Equals(new(0));
			var (quotient, remainder) = DivRem(_size, BitsPerInt);
			if (remainder != 0)
				return false;
			return iqba.Equals(new BitArray(Unsafe.As<int[]>(_items)[..quotient]));
		}
		else if (obj is IEquatable<IEnumerable<bool>> iqbe)
			return iqbe.Equals(this);
		else if (obj is IEquatable<IEnumerable<byte>> iqye)
		{
			if (_items is null)
				return iqye.Equals([]);
			var (quotient, remainder) = DivRem(_size, BitsPerInt);
			if (remainder != 0)
				return false;
			fixed (uint* itemsPtr = _items)
				return iqye.Equals(new Span<byte>(itemsPtr, quotient).ToArray());
		}
		else if (obj is IEquatable<IEnumerable<int>> iqie)
		{
			if (_items is null)
				return iqie.Equals([]);
			var (quotient, remainder) = DivRem(_size, BitsPerInt);
			if (remainder != 0)
				return false;
			return iqie.Equals(Unsafe.As<int[]>(_items).GetSlice(0, quotient));
		}
		else if (obj is IEquatable<IEnumerable<uint>> iquie)
		{
			if (_items is null)
				return iquie.Equals([]);
			var (quotient, remainder) = DivRem(_size, BitsPerInt);
			if (remainder != 0)
				return false;
			return iquie.Equals(_items.GetSlice(0, quotient));
		}
		else
			return false;
	}

	protected override bool EqualsInternal(IEnumerable<bool>? collection, int index, bool toEnd = false)
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

	protected virtual bool EqualsToBitList(BitList bitList, int index, bool toEnd = false)
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

	protected virtual bool EqualsToBitListOneInt(BitList bitList, CopyBitsIndex start, uint startMask)
	{
		Debug.Assert(_items is not null);
		Debug.Assert(bitList._items is not null);
		var buff = _items[start.IntIndex] & startMask;
		var destinationMask = ~(bitList._size == BitsPerInt ? 0 : ~0u << bitList._size);
		buff >>= start.BitsIndex;
		if (bitList._size + start.BitsIndex > BitsPerInt)
			buff |= _items[start.IntIndex + 1] << BitsPerInt - start.BitsIndex;
		buff &= destinationMask;
		return (bitList._items[0] & destinationMask) == buff;
	}

	protected virtual bool EqualsToBitListSeveralInts(BitList bitList, int index, CopyBitsIndex start, uint startMask, CopyBitsIndex destinationEnd)
	{
		Debug.Assert(_items is not null);
		Debug.Assert(bitList._items is not null);
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

	protected virtual bool EqualsToBytes(IEnumerable<byte> ints)
	{
		if (_items is null)
			return !ints.Any();
		var (quotient, remainder) = DivRem(_size, BitsPerByte);
		if (remainder != 0)
			return false;
		var length = 0;
		fixed (uint* itemsPtr = _items)
		{
			var bytePtr = (byte*)itemsPtr;
			var en = ints.GetEnumerator();
			for (var i = 0; i < quotient; i++)
			{
				if (!en.MoveNext() || bytePtr[i] != en.Current)
					return false;
				length++;
			}
		}
		return _size == length * BitsPerByte;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		if (_items is null || _items.Length < 3)
			return 1234567890;
		else
			return _items[0].GetHashCode() ^ _items[1].GetHashCode() ^ _items[_items.Length - 1].GetHashCode();
	}

	protected override bool GetInternal(int index)
	{
		Debug.Assert(_items is not null);
		return (_items[index / BitsPerInt] & (1 << index % BitsPerInt)) != 0;
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
		Debug.Assert(_items is not null);
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

	protected override int IndexOfInternal(bool item, int index, int length)
	{
		Debug.Assert(_items is not null);
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
		var mask = invRemainder == BitsPerInt ? 0xffffffff : (1u << invRemainder) - 1;
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

	protected virtual bool IndexOfMainCycle(bool item, int i, out int index)
	{
		Debug.Assert(_items is not null);
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
		if (ints is List<int> intList)
		{
			var length = intList.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			fixed (uint* itemsPtr = _items)
			{
				CopyBits(itemsPtr, _items?.Length ?? 0, index, itemsPtr, _items?.Length ?? 0, index + length, _size - index);
				fixed (int* intPtr = intList.AsSpan())
					CopyBits((uint*)intPtr, intList.Length, 0, itemsPtr, _items?.Length ?? 0, index, length);
			}
			_size += length;
			Changed();
			return this;
		}
		else if (ints is int[] intArray)
		{
			var length = intArray.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			fixed (uint* itemsPtr = _items)
			{
				CopyBits(itemsPtr, _items?.Length ?? 0, index, itemsPtr, _items?.Length ?? 0, index + length, _size - index);
				fixed (int* intPtr = intArray)
					CopyBits((uint*)intPtr, intArray.Length, 0, itemsPtr, _items?.Length ?? 0, index, length);
			}
			_size += length;
			Changed();
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
		if (uints is List<uint> uintList)
		{
			var length = uintList.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			fixed (uint* itemsPtr = _items)
			{
			CopyBits(itemsPtr, _items?.Length ?? 0, index, itemsPtr, _items?.Length ?? 0, index + length, _size - index);
			fixed (uint* uintPtr = uintList.AsSpan())
				CopyBits(uintPtr, uintList.Length, 0, itemsPtr, _items?.Length ?? 0, index, length);
			}
			_size += length;
			Changed();
			return this;
		}
		else if (uints is uint[] uintArray)
		{
			var length = uintArray.Length * BitsPerInt;
			if (length == 0)
				return this;
			EnsureCapacity(_size + length);
			fixed (uint* itemsPtr = _items)
			{
			CopyBits(itemsPtr, _items?.Length ?? 0, index, itemsPtr, _items?.Length ?? 0, index + length, _size - index);
			fixed (uint* uintPtr = uintArray)
				CopyBits(uintPtr, uintArray.Length, 0, itemsPtr, _items?.Length ?? 0, index, length);
			}
			_size += length;
			Changed();
			return this;
		}
		else
			return Insert(index, new BitList(uints));
	}

	public virtual BitList Insert(int index, ReadOnlySpan<int> ints)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = ints.Length * BitsPerInt;
		if (length == 0)
			return this;
		EnsureCapacity(_size + length);
			fixed (uint* itemsPtr = _items)
			{
		CopyBits(itemsPtr, _items?.Length ?? 0, index, itemsPtr, _items?.Length ?? 0, index + length, _size - index);
		fixed (int* intPtr = ints)
			CopyBits((uint*)intPtr, ints.Length, 0, itemsPtr, _items?.Length ?? 0, index, length);
			}
		_size += length;
		Changed();
		return this;
	}

	public virtual BitList Insert(int index, ReadOnlySpan<uint> uints)
	{
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var length = uints.Length * BitsPerInt;
		if (length == 0)
			return this;
		EnsureCapacity(_size + length);
			fixed (uint* itemsPtr = _items)
			{
		CopyBits(itemsPtr, _items?.Length ?? 0, index, itemsPtr, _items?.Length ?? 0, index + length, _size - index);
		fixed (uint* uintPtr = uints)
			CopyBits(uintPtr, uints.Length, 0, itemsPtr, _items?.Length ?? 0, index, length);
			}
		_size += length;
		Changed();
		return this;
	}

	protected override int LastIndexOfInternal(bool item, int index, int length)
	{
		Debug.Assert(_items is not null);
		var fillValue = item ? 0 : unchecked((int)0xffffffff);
		var endIndex = index + 1 - length;
		(var intIndex, var bitsIndex) = DivRem(endIndex, BitsPerInt);
		(var endIntIndex, var endBitsIndex) = DivRem(index + 1, BitsPerInt);
		if (intIndex == endIntIndex)
		{
			for (var i = endBitsIndex - 1; i >= bitsIndex; i--)
				if ((_items[intIndex] & (1 << i)) == 0 ^ item)
					return intIndex * BitsPerInt + i;
			return -1;
		}
		if (endBitsIndex != 0)
			for (var i = endBitsIndex - 1; i >= 0; i--)
				if ((_items[endIntIndex] & (1 << i)) == 0 ^ item)
					return endIntIndex * BitsPerInt + i;
		for (var i = endIntIndex - 1; i >= intIndex + 1; i--)
			if (_items[i] != fillValue)
				if (LastIndexOfMainCycle(item, i, out var result))
					return result;
		var invRemainder = BitsPerInt - bitsIndex;
		var mask = invRemainder == BitsPerInt ? 0xffffffff : (1u << invRemainder) - 1;
		var first = _items[intIndex] >> bitsIndex;
		if (first != (item ? 0 : mask))
			for (var i = invRemainder - 1; i >= 0; i--)
				if ((first & (1 << i)) == 0 ^ item)
					return endIndex + i;
		return -1;
	}

	protected virtual bool LastIndexOfMainCycle(bool item, int i, out int index)
	{
		Debug.Assert(_items is not null);
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
		if (_items is null)
			return this;
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] = ~_items[i];
		Changed();
		return this;
	}

	public virtual BitList Or(BitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (_size != value._size)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (_items is null || value._items is null)
			return this;
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] |= value._items[i];
		Changed();
		return this;
	}

	protected override void ReverseInternal(int index, int length)
	{
		for (var i = 0; i < length / 2; i++)
			(this[index + i], this[index + length - i - 1]) = (GetInternal(index + length - i - 1), GetInternal(index + i));
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
		Changed();
		return this;
	}

	protected override void SetAllInternal(bool value, int index, int length)
	{
		if (length == 0)
			return;
		Debug.Assert(_items is not null);
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

	protected override void SetInternal(int index, bool value)
	{
		Debug.Assert(_items is not null);
		if (value)
			_items[index / BitsPerInt] |= 1u << index % BitsPerInt;
		else
			_items[index / BitsPerInt] &= ~(1u << index % BitsPerInt);
	}

	public virtual BitList SetRange(int index, BitArray bitArray) => SetRange(index, new BitList(bitArray));

	public virtual BitList SetRange(int index, IEnumerable<byte> bytes) => SetRange(index, new BitList(bytes));

	public virtual BitList SetRange(int index, IEnumerable<int> ints)
	{
		ArgumentNullException.ThrowIfNull(ints);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (ints is List<int> intList)
		{
			if (index + intList.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (intList.Length <= 0)
				return this;
			fixed (int* intPtr = intList.AsSpan())
			fixed (uint* itemsPtr = _items)
				CopyBits((uint*)intPtr, intList.Length, 0, itemsPtr, _items?.Length ?? 0, index, intList.Length);
			Changed();
		}
		else if (ints is int[] intArray)
		{
			if (index + intArray.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (intArray.Length <= 0)
				return this;
			fixed (int* intPtr = intArray)
			fixed (uint* itemsPtr = _items)
				CopyBits((uint*)intPtr, intArray.Length, 0, itemsPtr, _items?.Length ?? 0, index, intArray.Length);
			Changed();
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
		if (uints is List<uint> uintList)
		{
			if (index + uintList.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (uintList.Length <= 0)
				return this;
			fixed (uint* uintPtr = uintList.AsSpan())
			fixed (uint* itemsPtr = _items)
				CopyBits(uintPtr, uintList.Length, 0, itemsPtr, _items?.Length ?? 0, index, uintList.Length);
			Changed();
		}
		else if (uints is uint[] uintArray)
		{
			if (index + uintArray.Length > _size)
				throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
			if (uintArray.Length <= 0)
				return this;
			fixed (uint* uintPtr = uintArray)
			fixed (uint* itemsPtr = _items)
				CopyBits(uintPtr, uintArray.Length, 0, itemsPtr, _items?.Length ?? 0, index, uintArray.Length);
			Changed();
		}
		else
			SetRange(index, new BitList(uints));
		Changed();
		return this;
	}

	public List<byte> ToByteList()
	{
		using var uints = ToUIntList();
		fixed (uint* ptr = uints.AsSpan())
			return List<byte>.FromPointer(GetArrayLength(_size, BitsPerByte), ptr);
	}

	public virtual List<uint> ToUIntList()
	{
		if (_items is null)
			return [];
		var length = GetArrayLength(_size, BitsPerInt);
		List<uint> result = new(length);
		for (var i = 0; i < length; i++)
			result.Add(_items[i]);
		return result;
	}

	public virtual BitList Xor(BitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (_size != value._size)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (_items is null || value._items is null)
			return this;
		var ints = GetArrayLength(_size, BitsPerInt);
		for (var i = 0; i < ints; i++)
			_items[i] ^= value._items[i];
		Changed();
		return this;
	}

	protected record struct CopyBitsIndex(int IntIndex, int BitsIndex)
	{
		public static implicit operator CopyBitsIndex(int index) => new(index / BitsPerInt, index % BitsPerInt);
	}

	protected record struct CopyBitsSide(CopyBitsIndex Start, CopyBitsIndex End)
	{
		public static implicit operator CopyBitsSide((CopyBitsIndex Start, CopyBitsIndex End) x) => new(x.Start, x.End);
	}
}
