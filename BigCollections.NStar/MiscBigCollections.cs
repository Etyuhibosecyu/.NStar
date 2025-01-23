
namespace BigCollections.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigQueue<T> : G.IEnumerable<T>, ICloneable
{
	private protected Queue<T>? low;
	private protected Queue<BigQueue<T>>? high;
	private protected MpzT _size;
	private protected MpzT fragment;
	private protected bool isHigh;
	private protected const int SubbranchesBitLength = 16, LeafSizeBitLength = 16;
	private protected const int Subbranches = 1 << SubbranchesBitLength, LeafSize = 1 << LeafSizeBitLength;

	public BigQueue() : this(32) { }

	public BigQueue(MpzT capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity <= LeafSize)
		{
			low = new((int)capacity);
			high = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - LeafSizeBitLength, SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			high = new((int)GetArrayLength(capacity, fragment));
			for (MpzT i = 0; i < capacity / fragment; i++)
				high.Enqueue(new(fragment));
			high.Enqueue(new(capacity % fragment));
			isHigh = true;
		}
		_size = 0;
	}

	public BigQueue(G.IEnumerable<T> col) : this((col == null) ? throw new ArgumentNullException(nameof(col)) : col.TryGetLengthEasily(out var length) ? length : 32)
	{
		using var en = col.GetEnumerator();
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
		if (_size == LeafSize && !isHigh && low != null)
		{
			high = new(4);
			high.Enqueue(new(Subbranches) { low = low, _size = low.Length });
			BigQueue<T> tempQueue = new(Subbranches);
			high.Enqueue(tempQueue);
			tempQueue.Enqueue(obj);
			low = null;
			fragment = LeafSize;
			isHigh = true;
		}
		else if (!isHigh && low != null)
			low.Enqueue(obj);
		else if (high != null)
		{
			var index = (int)(_size / fragment);
			if (index == Subbranches)
			{
				var temp = high;
				high = new(4);
				high.Enqueue(new(_size) { high = temp, _size = temp.Length });
				BigQueue<T> tempQueue = new(_size);
				high.Enqueue(tempQueue);
				tempQueue.Enqueue(obj);
				fragment *= Subbranches;
			}
			else if (E.ElementAt(high, index)._size == fragment)
			{
				BigQueue<T> tempQueue = new(Subbranches);
				high.Enqueue(tempQueue);
				tempQueue.Enqueue(obj);
			}
			else
				E.ElementAt(high, index).Enqueue(obj);
		}
		_size++;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<T> G.IEnumerable<T>.GetEnumerator() => GetEnumerator();

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
			foreach (var subQueue in high)
				if (subQueue.Contains(obj))
					return true;
			return false;
		}
		else
			return false;
	}

	internal T GetElement(MpzT i)
	{
		if (!isHigh && low != null)
			return E.ElementAt(low, (int)(i % Subbranches));
		else if (high != null)
			return E.ElementAt(high, (int)(i / fragment)).GetElement(i % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual T[] ToArray()
	{
		if (!isHigh && low != null)
			return [.. low];
		else
			throw new InvalidOperationException("Слишком большая очередь для преобразования в массив!");
	}

	public virtual void TrimExcess()
	{
		if (_size <= LeafSize)
		{
			low = PeekQueue();
			low.TrimExcess();
		}
		else if (high != null)
		{
			high.TrimExcess();
			E.ElementAt(high, high.Length - 1).TrimExcess();
		}
	}

	[Serializable]
	public struct Enumerator : G.IEnumerator<T>
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

	public BigArray() : this(-1) { }

	public BigArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 2)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		low = new();
		high = null;
		fragment = 1;
		ArraySize = 0;
#if VERIFY
		Verify();
#endif
	}

	public BigArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 2)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (length <= LeafSize)
		{
			low = CapacityLowCreator((int)length);
			high = null;
			fragment = 1;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((length - 1).BitLength - LeafSizeBitLength, SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			var quotient = (int)length.Divide(fragment, out var remainder);
			var highCount = (int)GetArrayLength(length, fragment);
			high = new TCertain[highCount];
			for (var i = 0; i < quotient; i++)
				high[i] = CapacityCreator(fragment);
			if (remainder != 0)
				high[^1] = CapacityCreator(remainder);
		}
		ArraySize = length;
#if VERIFY
		Verify();
#endif
	}

	public BigArray(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(collection == null ? throw new ArgumentNullException(nameof(collection)) : collection.Length(), leafSizeBitLength, subbranchesBitLength)
	{
		using var en = collection.GetEnumerator();
		MpzT i = 0;
		while (en.MoveNext())
		{
			SetInternal(i, en.Current);
			i++;
		}
#if VERIFY
		Verify();
#endif
	}

	public BigArray(MpzT length, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(length, leafSizeBitLength, subbranchesBitLength)
	{
		using var en = collection.GetEnumerator();
		MpzT i = 0;
		while (en.MoveNext())
		{
			SetInternal(i, en.Current);
			i++;
		}
#if VERIFY
		Verify();
#endif
	}

	private protected virtual MpzT ArraySize { get; init; } = 0;

	public override MpzT Capacity { get => Length; set { } }

	private protected virtual int LeafSizeBitLength { get; init; } = 16;

	private protected virtual int LeafSize => 1 << LeafSizeBitLength;

	private protected virtual int SubbranchesBitLength { get; init; } = 16;

	private protected virtual int Subbranches => 1 << SubbranchesBitLength;

	public override MpzT Length {
		get => ArraySize;
		private protected set => throw new NotSupportedException("Это действие не поддерживается в этой коллекции."
			+ " Если оно нужно вам, используйте один из видов списков или множеств, а не массивов.");
	}

	public override TCertain Add(T item) => throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или множеств, а не массивов.");

	public override TCertain AddRange(G.IEnumerable<T> collection) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или множеств, а не массивов.");

	private protected override void ClearInternal(bool verify = true)
	{
		if (low != null)
			low.Clear(false);
		else if (high != null)
			Array.Clear(high);
		Changed();
#if VERIFY
		if (verify)
			Verify();
#endif
	}

	private protected override void ClearInternal(MpzT index, MpzT length)
	{
		if (low != null)
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
#if VERIFY
		Verify();
#endif
	}

	public override bool Contains(T item, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Length);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		if (low != null)
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

	private protected override void CopyToInternal(MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length)
	{
		CheckParams(sourceIndex, destination, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (this == destination && sourceIndex == destinationIndex)
			return;
		if (low != null && destination.low != null)
		{
			if (destination.low.Length < destinationIndex + length)
				destination.low.Resize((int)(destinationIndex + length));
			destination.low.ReplaceRange((int)destinationIndex, (int)length, low.GetRange((int)sourceIndex, (int)length));
			destination.Changed();
#if VERIFY
			source.Verify();
			destination.Verify();
#endif
			return;
		}
		if (this.fragment > destination.fragment && high != null)
		{
			int index = (int)sourceIndex.Divide(this.fragment, out var remainder), index2 = (int)((sourceIndex + length - 1) / this.fragment);
			if (index == index2)
				high[index].CopyToInternal(remainder, destination, destinationIndex, length);
			else
			{
				var firstPart = this.fragment - remainder;
				high[index].CopyToInternal(remainder, destination, destinationIndex, firstPart);
				for (var i = index + 1; i < index2; i++)
				{
					CopyToInternal(0, destination, sourceIndex + firstPart, this.fragment);
					firstPart += this.fragment;
				}
				high[index2].CopyToInternal(0, destination, destinationIndex + firstPart, length - firstPart);
			}
			destination.Changed();
#if VERIFY
			source.Verify();
			destination.Verify();
#endif
			return;
		}
		if (destination.fragment > this.fragment && destination.high != null)
		{
			int index = (int)destinationIndex.Divide(destination.fragment, out var remainder), index2 = (int)((destinationIndex + length - 1) / destination.fragment);
			if (index == index2)
				CopyToInternal(sourceIndex, destination.high[index], remainder, length);
			else
			{
				var firstPart = destination.fragment - remainder;
				CopyToInternal(sourceIndex, destination.high[index], remainder, firstPart);
				for (var i = index + 1; i < index2; i++)
				{
					CopyToInternal(sourceIndex + firstPart, destination, 0, destination.fragment);
					firstPart += destination.fragment;
				}
				CopyToInternal(sourceIndex + firstPart, destination.high[index2], 0, length - firstPart);
			}
			destination.Changed();
#if VERIFY
			source.Verify();
			destination.Verify();
#endif
			return;
		}
		if (!(high != null && destination.high != null && this.fragment == destination.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		TCertain[] sourceBits2 = high, destinationBits2 = destination.high;
		var fragment = this.fragment;
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
				sourceBits2[sourceIntIndex].CopyToInternal(sourceBitsIndex, destinationBits2[destinationIntIndex], destinationBitsIndex, length);
			else
			{
				var firstPart = fragment - sourceBitsIndex;
				sourceBits2[sourceIntIndex].CopyToInternal(sourceBitsIndex, destinationBits2[destinationIntIndex], destinationBitsIndex, firstPart);
				sourceBits2[sourceEndIntIndex].CopyToInternal(0, destinationBits2[destinationIntIndex], destinationBitsIndex + firstPart, length - firstPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
		{
			using var buff = CapacityCreator(fragment << 1);
			destinationBits2[destinationIntIndex].CopyToInternal(0, buff, 0, destinationBitsIndex);
			sourceBits2[sourceIntIndex].CopyToInternal(sourceBitsIndex, buff, destinationBitsIndex, fragment - sourceBitsIndex);
			var buffBitsIndex = destinationBitsIndex + (fragment - sourceBitsIndex);
			var secondPart = buffBitsIndex >= fragment;
			if (secondPart)
				buffBitsIndex -= fragment;
			for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; sourceCurrentIntIndex < sourceEndIntIndex + 1 || destinationCurrentIntIndex < destinationEndIntIndex;)
			{
				if (!secondPart)
				{
					sourceBits2[sourceCurrentIntIndex].CopyToInternal(0, buff, buffBitsIndex, sourceCurrentIntIndex++ == sourceEndIntIndex ? sourceEndBitsIndex + 1 : fragment);
					secondPart = true;
				}
				if (secondPart && destinationCurrentIntIndex < destinationEndIntIndex && buff.high != null)
				{
					destinationBits2[destinationCurrentIntIndex++] = buff.high[0];
					(buff.high[0], buff.high[1]) = (buff.high[1], CapacityCreator(fragment));
					secondPart = false;
				}
			}
			buff.CopyToInternal(0, destinationBits2[destinationEndIntIndex], 0, destinationEndBitsIndex + 1);
		}
		else
		{
			using var buff = CapacityCreator(fragment << 1);
			sourceBits2[sourceEndIntIndex].CopyToInternal(0, buff, 0, sourceEndBitsIndex + 1);
			destinationBits2[destinationEndIntIndex].CopyToInternal(destinationEndBitsIndex + 1, buff, sourceEndBitsIndex + 1, RedStarLinq.Min(fragment, destinationBits2[destinationEndIntIndex].Length) - (destinationEndBitsIndex + 1));
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
				if (secondPart && destinationCurrentIntIndex > destinationIntIndex)
				{
					buff.CopyToInternal(buffBitsIndex, destinationBits2[destinationCurrentIntIndex], 0, RedStarLinq.Min(fragment, destinationBits2[destinationCurrentIntIndex--].Length));
					secondPart = false;
				}
			}
			buff.CopyToInternal(0, destinationBits2[destinationIntIndex], destinationBitsIndex, fragment - destinationBitsIndex);
			buff.high = null;
		}
		destination.Changed();
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
	}

	private protected void CheckParams(MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length)
	{
		if (Length == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.");
		if (destination == null)
			throw new ArgumentNullException(nameof(destination), "Целевой массив не может быть нулевым.");
		if (destination.Length == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destination));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destination.Length)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	private protected override void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length)
	{
		if (length == 0)
			return;
		if (low != null)
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
		if (low != null)
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

	public override void Dispose()
	{
		low?.Dispose();
		high?.ForEach(x => x.Dispose());
		fragment = 1;
		GC.SuppressFinalize(this);
	}

	private protected override T GetInternal(MpzT index, bool invoke = true)
	{
		T item;
		if (low != null)
		{
			item = low[(int)index];
		}
		else if (high != null)
			item = high[(int)(index / fragment)].GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		if (invoke)
			Changed();
		return item;
	}

	private protected override TCertain GetRangeInternal(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		if (length == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && length == Length && this is TCertain thisList)
			return thisList;
		var list = CapacityCreator(length);
		CopyToInternal(index, list, 0, length);
#if VERIFY
		Verify();
		list.Verify();
#endif
		return list;
	}

	private protected override void RemoveInternal(MpzT index, MpzT length) => throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или множеств, а не массивов.");

	private protected override void RemoveEndInternal(MpzT index) => throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или множеств, а не массивов.");

	private protected override void SetInternal(MpzT index, T value)
	{
		if (low != null)
		{
			low[(int)index] = value;
		}
		else if (high != null)
			high[(int)(index / fragment)].SetInternal(index % fragment, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Changed();
#if VERIFY
		Verify();
#endif
	}
#if VERIFY

	private protected override void Verify()
	{
		VerifySingle();
		if (high == null)
			return;
		for (var i = 0; i < high.Length; i++)
		{
			var x = high[i];
			x.Verify();
		}
	}

	private protected override void VerifySingle() => Debug.Assert(Length == (low?.Length ?? high?.Sum(x => x.Length)
		?? throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")));
#endif
}

/// <summary>
/// Представляет строго типизированный массив элементов, упорядоченных по индексу.
/// В отличие от стандартного T[], имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций с массивом находятся в разработке, на текущий момент
/// поддерживаются только установка элемента по индексу, копирование диапазона и копирование в обычный массив.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigArray<T> : BigArray<T, BigArray<T>, List<T>>
{
	public BigArray() { }

	public BigArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(length, subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(collection, subbranchesBitLength, leafSizeBitLength) { }

	public BigArray(MpzT length, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(length, collection, subbranchesBitLength, leafSizeBitLength) { }

	private protected override Func<MpzT, BigArray<T>> CapacityCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	private protected override Func<int, List<T>> CapacityLowCreator => RedStarLinq.EmptyList<T>;

	private protected override Func<G.IEnumerable<T>, BigArray<T>> CollectionCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	private protected override Func<G.IEnumerable<T>, List<T>> CollectionLowCreator => x => new(x);
}

/// <summary>
/// Представляет компактный строго типизированный массив бит (true или false), упорядоченных по индексу.
/// В отличие от стандартного <see cref="BitArray"/>, имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций с массивом находятся в разработке, на текущий момент
/// поддерживаются только установка элемента по индексу, копирование диапазона и копирование в обычный массив.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigBitArray : BigArray<bool, BigBitArray, BitList>
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	private protected const int BitsPerInt = sizeof(int) * BitsPerByte;
	private protected const int BytesPerInt = sizeof(int);
	private protected const int BitsPerByte = 8;

	public BigBitArray() { }

	public BigBitArray(int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(subbranchesBitLength, leafSizeBitLength) { }

	public BigBitArray(MpzT length, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(length, subbranchesBitLength, leafSizeBitLength) { }

	public BigBitArray(MpzT length, bool defaultValue, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 2)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (length <= LeafSize)
		{
			low = new((int)length, defaultValue);
			high = null;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((length - 1).BitLength - LeafSizeBitLength, SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			high = new BigBitArray[(int)GetArrayLength(length, fragment)];
			for (var i = 0; i < length / fragment; i++)
				high[i] = new(fragment, defaultValue, SubbranchesBitLength, LeafSizeBitLength);
			if (length % fragment != 0)
				high[^1] = new(length % fragment, defaultValue, SubbranchesBitLength, LeafSizeBitLength);
		}
		ArraySize = length;
	}

	public BigBitArray(BitArray bitArray, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 5)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		if (bitArray == null)
			throw new ArgumentNullException(nameof(bitArray));
		else
		{
			using BitList bitList = new(bitArray);
			BigBitArray array = new(bitList, base.SubbranchesBitLength, base.LeafSizeBitLength);
			low = array.low;
			high = array.high;
			ArraySize = array.Length;
			fragment = array.fragment;
		}
	}

	public BigBitArray(G.IEnumerable<byte> bytes, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 5)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		if (bytes == null)
			throw new ArgumentNullException(nameof(bytes));
		else if (bytes is byte[] byteArray && byteArray.Length <= int.MaxValue / BitsPerByte)
		{
			using BitList bitList = new(byteArray);
			BigBitArray array = new(bitList, base.SubbranchesBitLength, base.LeafSizeBitLength);
			low = array.low;
			high = array.high;
			ArraySize = array.Length;
			fragment = array.fragment;
		}
		else
		{
			var b = true;
			using var en = bytes.GetEnumerator();
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
			BigBitArray bigBitArray2 = new(values, SubbranchesBitLength, LeafSizeBitLength);
			BigBitArray array = new(n * BitsPerByte, SubbranchesBitLength, LeafSizeBitLength);
			bigBitArray2.CopyToInternal(0, array, 0, n * BitsPerByte);
			low = array.low;
			high = array.high;
			ArraySize = n * BitsPerByte;
			fragment = array.fragment;
		}
	}

	public BigBitArray(G.IEnumerable<bool> bools, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 5)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		if (bools == null)
			throw new ArgumentNullException(nameof(bools));
		else if (bools is BigBitArray bigBitArray)
		{
			if (bigBitArray.low != null)
			{
				low = new(bigBitArray.low);
				high = null;
				ArraySize = bigBitArray.Length;
				fragment = bigBitArray.fragment;
			}
			else if (bigBitArray.high != null)
			{
				BigBitArray array = new(bigBitArray.Length, SubbranchesBitLength, LeafSizeBitLength);
				bigBitArray.CopyToInternal(0, array, 0, bigBitArray.Length);
				low = array.low;
				high = array.high;
				ArraySize = array.Length;
				fragment = array.fragment;
			}
		}
		else if (bools is BitList bitList)
		{
			if (bitList.Length <= LeafSize)
			{
				low = new(bitList);
				high = null;
				fragment = 1;
			}
			else
			{
				low = null;
				fragment = 1 << ((((MpzT)bitList.Length - 1).BitLength + SubbranchesBitLength - 1 - LeafSizeBitLength) / SubbranchesBitLength - 1) * SubbranchesBitLength + LeafSizeBitLength;
				var fragment2 = (int)fragment;
				high = new BigBitArray[GetArrayLength(bitList.Length, fragment2)];
				int index = 0, i = 0;
				for (; index <= bitList.Length - fragment2; index += fragment2)
					high[i++] = new(bitList.GetRange(index, fragment2), SubbranchesBitLength, LeafSizeBitLength);
				if (bitList.Length % fragment2 != 0)
				{
					high[i] = new(bitList.Length - index, SubbranchesBitLength, LeafSizeBitLength);
					high[i].SetRangeInternal(0, CollectionCreator(bitList.GetRange(index)));
				}
			}
			ArraySize = bitList.Length;
		}
		else if (bools is bool[] boolArray)
		{
			BigBitArray array = new(new BitList(boolArray), SubbranchesBitLength, LeafSizeBitLength);
			low = array.low;
			high = array.high;
			ArraySize = array.Length;
			fragment = array.fragment;
		}
		else
		{
			using var en = bools.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	public BigBitArray(G.IEnumerable<int> ints, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 5)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		if (ints == null)
			throw new ArgumentNullException(nameof(ints));
		else
		{
			BigBitArray array = new(new BigArray<uint>(E.Select(ints, x => (uint)x)), SubbranchesBitLength, LeafSizeBitLength);
			low = array.low;
			high = array.high;
			ArraySize = array.Length;
			fragment = array.fragment;
		}
	}

	public BigBitArray(G.IEnumerable<uint> uints, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 5)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		if (uints == null)
			throw new ArgumentNullException(nameof(uints));
		else if (uints is BigArray<uint> bigUIntArray)
		{
			var length = bigUIntArray.Length;
			if (length <= GetArrayLength(LeafSize, BitsPerInt))
			{
				low = new(bigUIntArray);
				high = null;
				fragment = 1;
			}
			else
			{
				low = null;
				fragment = 1 << (((length - 1).BitLength + ((MpzT)BitsPerInt - 1).BitLength + SubbranchesBitLength - 1 - LeafSizeBitLength) / SubbranchesBitLength - 1) * SubbranchesBitLength + LeafSizeBitLength;
				var uintsFragment = fragment / BitsPerInt;
				high = new BigBitArray[(int)((length + uintsFragment - 1) / uintsFragment)];
				MpzT index = 0;
				var i = 0;
				for (; index <= length - uintsFragment; index += uintsFragment)
					high[i++] = new(bigUIntArray.GetRange(index, uintsFragment), SubbranchesBitLength, LeafSizeBitLength);
				if (index != length)
					high[i] = new(bigUIntArray.GetRange(index, length - index), SubbranchesBitLength, LeafSizeBitLength);
			}
			ArraySize = length * BitsPerInt;
		}
		else if (uints is BigList<uint> bigUIntList)
		{
			var length = bigUIntList.Length;
			if (length <= LeafSize / BitsPerInt)
			{
				low = new(bigUIntList);
				high = null;
				fragment = 1;
			}
			else
			{
				low = null;
				fragment = 1 << (((length - 1).BitLength + ((MpzT)BitsPerInt - 1).BitLength + SubbranchesBitLength - 1 - LeafSizeBitLength) / SubbranchesBitLength - 1) * SubbranchesBitLength + LeafSizeBitLength;
				var uintsFragment = fragment / BitsPerInt;
				high = new BigBitArray[(int)((length + uintsFragment - 1) / uintsFragment)];
				MpzT index = 0;
				var i = 0;
				for (; index <= length - uintsFragment; index += uintsFragment)
					high[i++] = new(bigUIntList.GetRange(index, uintsFragment), SubbranchesBitLength, LeafSizeBitLength);
				if (index != length)
					high[i] = new(bigUIntList.GetRange(index, length - index), SubbranchesBitLength, LeafSizeBitLength);
			}
			ArraySize = length * BitsPerInt;
		}
		else
		{
			BigBitArray array = new(new BigArray<uint>(uints), SubbranchesBitLength, LeafSizeBitLength);
			low = array.low;
			high = array.high;
			ArraySize = array.Length;
			fragment = array.fragment;
		}
	}

	private protected override Func<MpzT, BigBitArray> CapacityCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	private protected override Func<int, BitList> CapacityLowCreator => x => new(x, false);

	private protected override Func<G.IEnumerable<bool>, BigBitArray> CollectionCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	private protected override Func<G.IEnumerable<bool>, BitList> CollectionLowCreator => x => new(x);

	public virtual BigBitArray And(BigBitArray value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.And(value.low);
		else if (high != null && value.high != null)
			high = [.. high.Combine(value.high, (x, y) => x.And(y))];
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
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
		if (low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}

	public virtual BigBitArray Or(BigBitArray value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.Or(value.low);
		else if (high != null && value.high != null)
			high = [.. high.Combine(value.high, (x, y) => x.Or(y))];
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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

	public virtual BigArray<uint> ToUIntBigList()
	{
		if (low != null)
			return new(low.ToUIntList());
		else if (high != null)
			return new(E.SelectMany(high, x => x.ToUIntBigList()));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	public virtual BigBitArray Xor(BigBitArray value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.Xor(value.low);
		else if (high != null && value.high != null)
			high = [.. high.Combine(value.high, (x, y) => x.Xor(y))];
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		return this;
	}
}
