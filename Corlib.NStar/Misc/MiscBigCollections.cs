
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigQueue<T> : IEnumerable<T>, ICloneable
{
	private Queue<T>? low;
	private Queue<BigQueue<T>>? high;
	private MpzT _size;
	private MpzT fragment;
	private bool isHigh;
	private const int CapacityStepBitLength = 16, CapacityFirstStepBitLength = 16;
	private const int CapacityStep = 1 << CapacityStepBitLength, CapacityFirstStep = 1 << CapacityFirstStepBitLength;

	public BigQueue() : this(32) { }

	public BigQueue(MpzT capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException(nameof(capacity));
		if (capacity <= CapacityFirstStep)
		{
			low = new((int)capacity);
			high = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			high = new((int)GetArrayLength(capacity, fragment));
			for (MpzT i = 0; i < capacity / fragment; i++)
				high.Enqueue(new(fragment));
			high.Enqueue(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
	}

	public BigQueue(IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : List<T>.TryGetLengthEasilyEnumerable(col, out var length) ? length : 32)
	{
		var en = col.GetEnumerator();
		while (en.MoveNext())
			Enqueue(en.Current);
	}

	public virtual MpzT Length => _size;

	public virtual object Clone()
	{
		BigQueue<T> q = new(_size) { _size = _size };
		if (!isHigh && low != null)
		{
			q.low = (Queue<T>)low.Clone();
			q.high = null;
		}
		else if (high != null)
		{
			q.low = null;
			q.high = (Queue<BigQueue<T>>)high.Clone();
		}
		q.isHigh = isHigh;
		return q;
	}

	public virtual void Clear()
	{
		if (!isHigh && low != null)
			low.Clear();
		else high?.Clear();
	}

	public virtual void CopyTo(Array array, int index)
	{
		if (!isHigh && low != null)
			low.CopyTo(array, index);
		else
			throw new InvalidOperationException("Слишком большая очередь для копирования в массив!");
	}

	public virtual void Enqueue(T obj)
	{
		if (_size == CapacityFirstStep && !isHigh && low != null)
		{
			high = new(4);
			high.Enqueue(new(CapacityStep) { low = low, _size = low.Length });
			high.Enqueue(new(CapacityStep));
			high.GetElement(1).Enqueue(obj);
			low = null;
			fragment = CapacityFirstStep;
			isHigh = true;
		}
		else if (!isHigh && low != null)
			low.Enqueue(obj);
		else if (high != null)
		{
			var index = (int)(_size / fragment);
			if (index == CapacityStep)
			{
				var temp = high;
				high = new(4);
				high.Enqueue(new(_size) { high = temp, _size = temp.Length });
				high.Enqueue(new(_size));
				high.GetElement(high.Length - 1).Enqueue(obj);
				fragment *= CapacityStep;
			}
			else if (high.GetElement(index)._size == fragment)
			{
				high.Enqueue(new(CapacityStep));
				high.GetElement(index).Enqueue(obj);
			}
			else
				high.GetElement(index).Enqueue(obj);
		}
		_size++;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual T Dequeue()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		_size--;
		if (!isHigh && low != null)
			return low.Dequeue();
		else if (high != null)
		{
			var removed = high.Peek().Dequeue();
			if (high.Peek()._size == 0)
				high.Dequeue();
			return removed;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual T Peek()
	{
		if (_size == 0)
			throw new InvalidOperationException();
		if (!isHigh && low != null)
			return low.Peek();
		else if (high != null)
			return high.Peek().Peek();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected Queue<T> PeekQueue()
	{
		if (!isHigh && low != null)
			return low;
		else if (high != null)
			return high.Peek().PeekQueue();
		else
			return new();
	}

	public virtual bool Contains(T? obj)
	{
		if (!isHigh && low != null)
			return low.Contains(obj);
		else if (high != null)
		{
			for (var i = 0; i < high.Length; i++)
				if (high.GetElement(i).Contains(obj))
					return true;
			return false;
		}
		else
			return false;
	}

	internal T GetElement(MpzT i)
	{
		if (!isHigh && low != null)
			return low.GetElement((int)(i % CapacityStep));
		else if (high != null)
			return high.GetElement((int)(i / fragment)).GetElement(i % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual T[] ToArray()
	{
		if (!isHigh && low != null)
			return low.ToArray();
		else
			throw new InvalidOperationException("Слишком большая очередь для преобразования в массив!");
	}

	public virtual void TrimExcess()
	{
		if (_size <= CapacityFirstStep)
		{
			low = PeekQueue();
			low.TrimExcess();
		}
		else if (high != null)
		{
			high.TrimExcess();
			high.GetElement(high.Length - 1).TrimExcess();
		}
	}

	[Serializable]
	public struct Enumerator : IEnumerator<T>
	{
		private readonly BigQueue<T> queue;
		private MpzT index;
		private T current;

		public readonly T Current
		{
			get
			{
				if (index == 0 || index == queue._size + 1)
					throw new InvalidOperationException();
				return current;
			}
		}

		internal Enumerator(BigQueue<T> queue)
		{
			this.queue = queue;
			index = 0;
			current = default!;
		}

		public readonly void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (index < queue._size)
			{
				current = queue.GetElement(index)!;
				index++;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = queue._size + 1;
			current = default!;
			return false;
		}

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == queue._size + 1)
					throw new InvalidOperationException();
				return Current!;
			}
		}

		void IEnumerator.Reset()
		{
			index = 0;
			current = default!;
		}
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BigArray<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow> where TCertain : BigArray<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected TCertain[]? high;
	private protected MpzT fragment = 1;
	private protected bool isHigh;

	public BigArray() : this(-1) { }

	public BigArray(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		low = new();
		high = null;
		fragment = 1;
		isHigh = false;
		Size2 = 0;
	}

	public BigArray(MpzT length, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
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
			low = CapacityLowCreator((int)length);
			high = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = default;
			fragment = (MpzT)1 << (GetArrayLength((length - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			var quotient = (int)length.Divide(fragment, out var remainder);
			var highCount = (int)GetArrayLength(length, fragment);
			high = new TCertain[highCount];
			for (var i = 0; i < quotient; i++)
				high[i] = CapacityCreator(fragment);
			if (remainder != 0)
				high[^1] = CapacityCreator(remainder);
			isHigh = true;
		}
		Size2 = length;
	}

	public BigArray(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(collection == null ? throw new ArgumentNullException(nameof(collection)) : collection.Length(), capacityFirstStepBitLength, capacityStepBitLength)
	{
		var en = collection.GetEnumerator();
		MpzT i = 0;
		while (en.MoveNext())
		{
			SetInternal(i, en.Current);
			i++;
		}
	}

	public BigArray(MpzT length, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(length, capacityFirstStepBitLength, capacityStepBitLength)
	{
		var en = collection.GetEnumerator();
		MpzT i = 0;
		while (en.MoveNext())
		{
			SetInternal(i, en.Current);
			i++;
		}
	}

	public override MpzT Capacity
	{
		get => Size;
		set
		{
		}
	}

	private protected virtual int CapacityFirstStepBitLength { get; init; } = 16;

	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	private protected virtual int CapacityStepBitLength { get; init; } = 16;

	private protected virtual int CapacityStep => 1 << CapacityStepBitLength;

	private protected override MpzT Size { get => Size2; set => throw new NotSupportedException(); }

	private protected virtual MpzT Size2 { get; init; } = 0;

	public override TCertain Add(T item) => throw new NotSupportedException();

	private protected override void ClearInternal()
	{
		if (!isHigh && low != null)
			low.Clear();
		else if (high != null)
			Array.Clear(high);
		Changed();
	}

	private protected override void ClearInternal(MpzT index, MpzT length)
	{
		if (!isHigh && low != null)
			low.Clear((int)index, (int)length);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)((index + length - 1) / fragment);
			if (quotient == quotient2)
			{
				high[quotient].ClearInternal(remainder, length);
				return;
			}
			var previousPart = fragment - remainder;
			high[quotient].ClearInternal(remainder, previousPart);
			for (var i = quotient + 1; i < quotient2; i++)
			{
				high[i].ClearInternal(0, fragment);
				previousPart += fragment;
			}
			high[quotient2].ClearInternal(0, length - previousPart);
		}
		Changed();
	}

	public override bool Contains(T item, MpzT index, MpzT length)
	{
		if (index > Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		if (!isHigh && low != null)
			return low.Contains(item, (int)index, (int)length);
		else if (high != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			var quotient2 = (int)((index + length - 1) / fragment);
			if (quotient == quotient2)
				return high[quotient].Contains(item, remainder, length);
			var previousPart = fragment - remainder;
			if (high[quotient].Contains(item, remainder, previousPart))
				return true;
			for (var i = quotient + 1; i < quotient2; i++)
			{
				if (high[i].Contains(item))
					return true;
				previousPart += fragment;
			}
			return high[quotient2].Contains(item, 0, length - previousPart);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void Copy(TCertain sourceBits, MpzT sourceIndex, TCertain destinationBits, MpzT destinationIndex, MpzT length)
	{
		CheckParams(sourceBits, sourceIndex, destinationBits, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (sourceBits == destinationBits && sourceIndex == destinationIndex)
			return;
		if (!sourceBits.isHigh && sourceBits.low != null && !destinationBits.isHigh && destinationBits.low != null)
		{
			destinationBits.low.SetRangeInternal((int)destinationIndex, (int)length, sourceBits.low.GetRange((int)sourceIndex, (int)length));
			destinationBits.Changed();
			return;
		}
		if (sourceBits.fragment > destinationBits.fragment && sourceBits.isHigh && sourceBits.high != null)
		{
			int index = (int)sourceIndex.Divide(sourceBits.fragment, out var remainder), index2 = (int)((sourceIndex + length - 1) / sourceBits.fragment);
			if (index == index2)
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, length);
			else
			{
				var firstPart = sourceBits.fragment - remainder;
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, firstPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits, 0, destinationBits, sourceIndex + firstPart, sourceBits.fragment);
					firstPart += sourceBits.fragment;
				}
				Copy(sourceBits.high[index2], 0, destinationBits, destinationIndex + firstPart, length - firstPart);
			}
			destinationBits.Changed();
			return;
		}
		if (destinationBits.fragment > sourceBits.fragment && destinationBits.isHigh && destinationBits.high != null)
		{
			int index = (int)destinationIndex.Divide(destinationBits.fragment, out var remainder), index2 = (int)((destinationIndex + length - 1) / destinationBits.fragment);
			if (index == index2)
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, length);
			else
			{
				var firstPart = destinationBits.fragment - remainder;
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, firstPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits, sourceIndex + firstPart, destinationBits, 0, destinationBits.fragment);
					firstPart += destinationBits.fragment;
				}
				Copy(sourceBits, sourceIndex + firstPart, destinationBits.high[index2], 0, length - firstPart);
			}
			destinationBits.Changed();
			return;
		}
		if (!(sourceBits.isHigh && sourceBits.high != null && destinationBits.isHigh && destinationBits.high != null && sourceBits.fragment == destinationBits.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		TCertain[] sourceBits2 = sourceBits.high, destinationBits2 = destinationBits.high;
		var fragment = sourceBits.fragment;
		var sourceIntIndex = (int)sourceIndex.Divide(fragment, out var sourceBitsIndex);               // Целый индекс в исходном массиве.
		var destinationIntIndex = (int)destinationIndex.Divide(fragment, out var destinationBitsIndex);     // Целый индекс в целевом массиве.
		var intOffset = destinationIntIndex - sourceIntIndex;       // Целое смещение.
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		var sourceEndIntIndex = (int)sourceEndIndex.Divide(fragment, out var sourceEndBitsIndex);  // Индекс инта последнего бита.
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		var destinationEndIntIndex = (int)destinationEndIndex.Divide(fragment, out var destinationEndBitsIndex);  // Индекс инта последнего бита.
		if (destinationEndIntIndex == destinationIntIndex)
		{
			if (sourceEndIntIndex == sourceIntIndex)
				Copy(sourceBits2[sourceIntIndex], sourceBitsIndex, destinationBits2[destinationIntIndex], destinationBitsIndex, length);
			else
			{
				var firstPart = fragment - sourceBitsIndex;
				Copy(sourceBits2[sourceIntIndex], sourceBitsIndex, destinationBits2[destinationIntIndex], destinationBitsIndex, firstPart);
				Copy(sourceBits2[sourceEndIntIndex], 0, destinationBits2[destinationIntIndex], destinationBitsIndex + firstPart, length - firstPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
		{
			var buff = CapacityCreator(fragment << 1);
			Copy(destinationBits2[destinationIntIndex], 0, buff, 0, destinationBitsIndex);
			Copy(sourceBits2[sourceIntIndex], sourceBitsIndex, buff, destinationBitsIndex, fragment - sourceBitsIndex);
			var buffBitsIndex = destinationBitsIndex + (fragment - sourceBitsIndex);
			var secondPart = buffBitsIndex >= fragment;
			if (secondPart)
				buffBitsIndex -= fragment;
			for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; sourceCurrentIntIndex < sourceEndIntIndex + 1 || destinationCurrentIntIndex < destinationEndIntIndex;)
			{
				if (!secondPart)
				{
					Copy(sourceBits2[sourceCurrentIntIndex], 0, buff, buffBitsIndex, sourceCurrentIntIndex++ == sourceEndIntIndex ? sourceEndBitsIndex + 1 : fragment);
					secondPart = true;
				}
				if (secondPart && destinationCurrentIntIndex < destinationEndIntIndex && buff.high != null)
				{
					destinationBits2[destinationCurrentIntIndex++] = buff.high[0];
					(buff.high[0], buff.high[1]) = (buff.high[1], CapacityCreator(fragment));
					secondPart = false;
				}
			}
			Copy(buff, 0, destinationBits2[destinationEndIntIndex], 0, destinationEndBitsIndex + 1);
		}
		else
		{
			var buff = CapacityCreator(fragment << 1);
			Copy(sourceBits2[sourceEndIntIndex], 0, buff, 0, sourceEndBitsIndex + 1);
			Copy(destinationBits2[destinationEndIntIndex], destinationEndBitsIndex + 1, buff, sourceEndBitsIndex + 1, RedStarLinq.Min(fragment, destinationBits2[destinationEndIntIndex].Length) - (destinationEndBitsIndex + 1));
			var buffBitsIndex = sourceEndBitsIndex + (fragment - destinationEndBitsIndex);
			var secondPart = buffBitsIndex >= fragment;
			if (buffBitsIndex >= fragment)
				buffBitsIndex -= fragment;
			for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; sourceCurrentIntIndex > sourceIntIndex - 1 || destinationCurrentIntIndex > destinationIntIndex;)
			{
				if (!secondPart && sourceCurrentIntIndex >= 0 && buff.high != null)
				{
					(buff.high[1], buff.high[0]) = (buff.high[0], sourceBits2[sourceCurrentIntIndex--]);
					secondPart = true;
				}
				//if (sourceCurrentIntIndex <= sourceIntIndex - 1)
				//	break;
				if (secondPart && destinationCurrentIntIndex > destinationIntIndex)
				{
					Copy(buff, buffBitsIndex, destinationBits2[destinationCurrentIntIndex], 0, RedStarLinq.Min(fragment, destinationBits2[destinationCurrentIntIndex--].Length));
					secondPart = false;
				}
			}
			Copy(buff, 0, destinationBits2[destinationIntIndex], destinationBitsIndex, fragment - destinationBitsIndex);
		}
		destinationBits.Changed();
	}

	private static void CheckParams(TCertain sourceBits, MpzT sourceIndex, TCertain destinationBits, MpzT destinationIndex, MpzT length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Size == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Size == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceBits.Size)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationBits.Size)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	private protected override void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length)
	{
		if (length == 0)
			return;
		if (!isHigh && low != null)
		{
			var index2 = (int)index;
			for (var i = 0; i < length; i++)
				array[arrayIndex + i] = low[index2 + i];
		}
		else if (high != null)
		{
			var intIndex = (int)index.Divide(fragment, out var bitsIndex);
			var endIntIndex = (int)(index + length - 1).Divide(fragment, out var endBitsIndex);
			if (endIntIndex == intIndex)
			{
				high[intIndex].CopyToInternal(bitsIndex, array, arrayIndex, length);
				return;
			}
			high[intIndex].CopyToInternal(bitsIndex, array, arrayIndex, (int)(fragment - bitsIndex));
			var destIndex = arrayIndex + fragment - bitsIndex;
			for (var i = intIndex + 1; i < endIntIndex; i++, destIndex += fragment)
				high[i].CopyToInternal(0, array, (int)destIndex, (int)fragment);
			high[endIntIndex].CopyToInternal(0, array, (int)destIndex, (int)(endBitsIndex + 1));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void CopyToInternal(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length)
	{
		if (length == 0)
			return;
		if (!isHigh && low != null)
		{
			int index2 = (int)index, count2 = (int)length;
			for (var i = 0; i < count2; i++)
				list[listIndex + i] = low[index2 + i];
		}
		else if (high != null)
		{
			var intIndex = (int)index.Divide(fragment, out var bitsIndex);
			var endIntIndex = (int)(index + length - 1).Divide(fragment, out var endBitsIndex);
			if (endIntIndex == intIndex)
			{
				high[intIndex].CopyToInternal(bitsIndex, list, listIndex, length);
				return;
			}
			high[intIndex].CopyToInternal(bitsIndex, list, listIndex, fragment - bitsIndex);
			var destIndex = listIndex + fragment - bitsIndex;
			for (var i = intIndex + 1; i < endIntIndex; i++, destIndex += fragment)
				high[i].CopyToInternal(0, list, destIndex, fragment);
			high[endIntIndex].CopyToInternal(0, list, destIndex, endBitsIndex + 1);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override T GetInternal(MpzT index, bool invoke = true)
	{
		T item;
		if (!isHigh && low != null)
		{
			//try
			//{
			//	throw new ExperimentalException();
			//}
			//catch
			//{
			//}
			item = low.GetInternal((int)index);
		}
		else if (high != null)
			item = high[(int)(index / fragment)].GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		if (invoke)
			Changed();
		return item;
	}

	public override TCertain GetRange(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));
		if (index + length > Size)
			throw new ArgumentException(null);
		if (length == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && length == Size && this is TCertain thisList)
			return thisList;
		var list = CapacityCreator(length);
		Copy(this as TCertain ?? throw new InvalidOperationException(), index, list, 0, length);
		return list;
	}

	private protected override void SetInternal(MpzT index, T value)
	{
		if (!isHigh && low != null)
		{
			//try
			//{
			//	throw new ExperimentalException();
			//}
			//catch
			//{
			//}
			low.SetInternal((int)index, value);
		}
		else if (high != null)
			high[(int)(index / fragment)].SetInternal(index % fragment, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Changed();
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigArray<T> : BigArray<T, BigArray<T>, List<T>>
{
	public BigArray() { }

	public BigArray(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigArray(MpzT length, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(length, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigArray(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(collection, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigArray(MpzT length, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(length, collection, capacityStepBitLength, capacityFirstStepBitLength) { }

	private protected override Func<MpzT, BigArray<T>> CapacityCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<int, List<T>> CapacityLowCreator => RedStarLinq.EmptyList<T>;

	private protected override Func<IEnumerable<T>, BigArray<T>> CollectionCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<IEnumerable<T>, List<T>> CollectionLowCreator => x => new(x);
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigBitArray : BigArray<bool, BigBitArray, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private const int BitsPerInt = sizeof(int) * BitsPerByte;
	private const int BytesPerInt = sizeof(int);
	private const int BitsPerByte = 8;

	public BigBitArray() { }

	public BigBitArray(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigBitArray(MpzT length, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : base(length, capacityStepBitLength, capacityFirstStepBitLength) { }

	public BigBitArray(MpzT length, bool defaultValue, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
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
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((length - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			high = new BigBitArray[(int)GetArrayLength(length, fragment)];
			for (var i = 0; i < length / fragment; i++)
				high[i] = new(fragment, defaultValue, CapacityStepBitLength, CapacityFirstStepBitLength);
			if (length % fragment != 0)
				high[^1] = new(length % fragment, defaultValue, CapacityStepBitLength, CapacityFirstStepBitLength);
			isHigh = true;
		}
		Size2 = length;
	}

	public BigBitArray(IEnumerable bits, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 5)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		if (bits == null)
			throw new ArgumentNullException(nameof(bits));
		else if (bits is BigBitArray bigBitArray)
		{
			if (!bigBitArray.isHigh && bigBitArray.low != null)
			{
				low = new(bigBitArray.low);
				high = null;
				Size2 = bigBitArray.Size;
				fragment = bigBitArray.fragment;
				isHigh = false;
			}
			else if (bigBitArray.high != null)
			{
				BigBitArray array = new(bigBitArray.Size, CapacityStepBitLength, CapacityFirstStepBitLength);
				Copy(bigBitArray, 0, array, 0, bigBitArray.Size);
				low = array.low;
				high = array.high;
				Size2 = array.Size;
				fragment = array.fragment;
				isHigh = array.isHigh;
			}
		}
		else if (bits is BitList bitList)
		{
			if (bitList.Length <= CapacityFirstStep)
			{
				low = new(bitList);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << ((((MpzT)bitList.Length - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var fragment2 = (int)fragment;
				high = new BigBitArray[GetArrayLength(bitList.Length, fragment2)];
				int index = 0, i = 0;
				for (; index <= bitList.Length - fragment2; index += fragment2)
					high[i++] = new(bitList.GetRange(index, fragment2), CapacityStepBitLength, CapacityFirstStepBitLength);
				if (bitList.Length % fragment2 != 0)
				{
					high[i] = new(bitList.Length - index, CapacityStepBitLength, CapacityFirstStepBitLength);
					high[i].SetRangeInternal(0, CollectionCreator(bitList.GetRange(index)));
				}
				isHigh = true;
			}
			Size2 = bitList.Length;
		}
		else if (bits is BigArray<uint> bigUIntArray)
		{
			var length = bigUIntArray.Length;
			if (length <= GetArrayLength(CapacityFirstStep, BitsPerInt))
			{
				low = new(bigUIntArray);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << (((length - 1).BitLength + ((MpzT)BitsPerInt - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var uintsFragment = fragment / BitsPerInt;
				high = new BigBitArray[(int)((length + uintsFragment - 1) / uintsFragment)];
				MpzT index = 0;
				var i = 0;
				for (; index <= length - uintsFragment; index += uintsFragment)
					high[i++] = new(bigUIntArray.GetRange(index, uintsFragment), CapacityStepBitLength, CapacityFirstStepBitLength);
				if (index != length)
					high[i] = new(bigUIntArray.GetRange(index, length - index), CapacityStepBitLength, CapacityFirstStepBitLength);
				isHigh = true;
			}
			Size2 = length * BitsPerInt;
		}
		else if (bits is BigList<uint> bigUIntList)
		{
			var length = bigUIntList.Length;
			if (length <= CapacityFirstStep / BitsPerInt)
			{
				low = new(bigUIntList);
				high = null;
				fragment = 1;
				isHigh = false;
			}
			else
			{
				low = null;
				fragment = 1 << (((length - 1).BitLength + ((MpzT)BitsPerInt - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				var uintsFragment = fragment / BitsPerInt;
				high = new BigBitArray[(int)((length + uintsFragment - 1) / uintsFragment)];
				MpzT index = 0;
				var i = 0;
				for (; index <= length - uintsFragment; index += uintsFragment)
					high[i++] = new(bigUIntList.GetRange(index, uintsFragment), CapacityStepBitLength, CapacityFirstStepBitLength);
				if (index != length)
					high[i] = new(bigUIntList.GetRange(index, length - index), CapacityStepBitLength, CapacityFirstStepBitLength);
				isHigh = true;
			}
			Size2 = length * BitsPerInt;
		}
		else if (bits is IEnumerable<uint> uints)
		{
			BigBitArray array = new(new BigArray<uint>(uints), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = array.low;
			high = array.high;
			Size2 = array.Size;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is IEnumerable<int> ints)
		{
			BigBitArray array = new(new BigArray<uint>(ints.Select(x => (uint)x)), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = array.low;
			high = array.high;
			Size2 = array.Size;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is IEnumerable<byte> bytes)
		{
			var b = true;
			var en = bytes.GetEnumerator();
			BigArray<uint> values = new(length: GetArrayLength(bytes.Length(), 4));
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
					values[(n - 1) / 4] = value;
			}
			BigBitArray bigBitArray2 = new(values, CapacityStepBitLength, CapacityFirstStepBitLength);
			BigBitArray array = new(n * BitsPerByte, CapacityStepBitLength, CapacityFirstStepBitLength);
			Copy(bigBitArray2, 0, array, 0, n * BitsPerByte);
			low = array.low;
			high = array.high;
			Size2 = n * BitsPerByte;
			fragment = array.fragment;
			isHigh = array.isHigh;
		}
		else if (bits is BitArray or byte[] or bool[])
		{
			BigBitArray array = new(new BitList(bits), CapacityStepBitLength, CapacityFirstStepBitLength);
			low = array.low;
			high = array.high;
			Size2 = array.Size;
			fragment = array.fragment;
			isHigh = array.isHigh;
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

	private protected override Func<MpzT, BigBitArray> CapacityCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<int, BitList> CapacityLowCreator => x => new(x, false);

	private protected override Func<IEnumerable<bool>, BigBitArray> CollectionCreator => x => new(x, CapacityStepBitLength, CapacityFirstStepBitLength);

	private protected override Func<IEnumerable<bool>, BitList> CollectionLowCreator => x => new(x);

	public virtual BigBitArray And(BigBitArray value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.And(value.low);
		else if (high != null && value.high != null)
			high = high.Combine(value.high, (x, y) => x.And(y)).ToArray();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual uint GetSmallRange(MpzT index, int length)
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
				result |= high[quotient + 1].GetSmallRange(0, (int)(remainder2 + 1)) << (int)(fragment - remainder);
			}
			return result;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitArray Not()
	{
		if (!isHigh && low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual BigBitArray Or(BigBitArray value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Or(value.low);
		else if (high != null && value.high != null)
			high = high.Combine(value.high, (x, y) => x.Or(y)).ToArray();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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

	public virtual BigArray<uint> ToUIntBigList()
	{
		if (!isHigh && low != null)
			return new(low.ToUIntList());
		else if (high != null)
			return new(high.SelectMany(x => x.ToUIntBigList()));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitArray Xor(BigBitArray value)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (Size != value.Size)
			throw new ArgumentException(null, nameof(value));
		if (!isHigh && low != null && !value.isHigh && value.low != null)
			low.Xor(value.low);
		else if (high != null && value.high != null)
			high = high.Combine(value.high, (x, y) => x.Xor(y)).ToArray();
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}
}
