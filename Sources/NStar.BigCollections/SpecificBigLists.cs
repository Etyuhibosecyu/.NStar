#pragma warning disable CS9216 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.

namespace NStar.BigCollections;

/// <summary>
/// Представляет компактный список бит в виде логических значений (true или false), упорядоченных по индексу.
/// В отличие от <see cref="BitList"/> и стандартного <see cref="BitArray"/>, имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций со списком находятся в разработке, на текущий момент
/// поддерживаются только добавление в конец, установка элемента по индексу и частично удаление.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class BigBitList : OldBigList<bool, BigBitList, BitList>
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
			high = new((int)GetArrayLength(length, fragment), true);
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
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
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
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigBitList(G.IEnumerable<byte> bytes, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
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
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigBitList(G.IEnumerable<bool> bools, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
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
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigBitList(G.IEnumerable<int> ints, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
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
			using BigList<uint> list = new(E.Select(ints, x => (uint)x));
			ConstructFromUIntList(list);
		}
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigBitList(G.IEnumerable<uint> uints, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
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
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigBitList(uint[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(values.AsEnumerable(), subbranchesBitLength, leafSizeBitLength) { }

	public BigBitList(ReadOnlySpan<uint> values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(values.Length * BitsPerInt, subbranchesBitLength, leafSizeBitLength)
	{
		using BigList<uint> list = new(values);
		ConstructFromUIntList(list);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigBitList(MpzT capacity, uint[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(RedStarLinqMath.Max(capacity, values.Length * BitsPerInt), values.AsSpan(),
		subbranchesBitLength, leafSizeBitLength) { }

	public BigBitList(MpzT capacity, ReadOnlySpan<uint> values, int subbranchesBitLength = -1,
		int leafSizeBitLength = -1) : this(RedStarLinqMath.Max(capacity, values.Length * BitsPerInt),
		subbranchesBitLength, leafSizeBitLength)
	{
		using BigList<uint> list = new(values);
		ConstructFromUIntList(list);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override Func<MpzT, BigBitList> CapacityCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	protected override int LeafSizeBitLength { get; init; } = 20;

	protected override Func<int, BitList> CapacityLowCreator { get; } = x => new(x);

	protected override Func<G.IEnumerable<bool>, BitList> CollectionLowCreator { get; } = x => new(x);

	protected override Func<G.IEnumerable<bool>, BigBitList> CollectionCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	protected override int DefaultCapacity => 256;

	public virtual BigBitList AddRange(BitArray bitArray)
	{
		using BigBitList bitList = new(bitArray, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList AddRange(G.IEnumerable<byte> bytes)
	{
		using BigBitList bitList = new(bytes, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList AddRange(G.IEnumerable<int> ints)
	{
		using BigBitList bitList = new(ints, SubbranchesBitLength, LeafSizeBitLength);
		return base.AddRange(bitList);
	}

	public virtual BigBitList AddRange(G.IEnumerable<uint> uints)
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
		else if (high != null && value.high != null)
			high = [.. high.Combine(value.high, (x, y) => x.And(y))];
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		return this;
	}

	protected virtual void ConstructFromBitList(BitList bitList)
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

	protected virtual void ConstructFromBitListFromScratch(BitList bitList)
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
			high = new(GetArrayLength(bitList.Length, fragment2), true);
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

	protected virtual void ConstructFromUIntList(BigList<uint> bigUIntList, int overrideLength = 0)
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

	protected virtual void ConstructFromUIntListFromScratch(BigList<uint> bigUIntList, int overrideLength = 0)
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
			high = new((int)GetArrayLength(length, uintsFragment), true);
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
				result = high[quotient].GetSmallRange(remainder, length);
			else
			{
				result = high[quotient].GetSmallRange(remainder, (int)(fragment - remainder));
				result |= high[quotient + 1].GetSmallRange(0, (int)remainder2) << (int)(BitsPerInt - remainder);
			}
			return result;
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public virtual BigBitList Not()
	{
		if (low != null)
			low.Not();
		else if (high != null)
			high.ForEach(x => x.Not());
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		return this;
	}

	public virtual BigBitList Or(BigBitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.Or(value.low);
		else if (high != null && value.high != null)
			high = [.. high.Combine(value.high, (x, y) => x.Or(y))];
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		return this;
	}

	public virtual void SetAll(bool value)
	{
		if (low != null)
			low.SetAll(value);
		else if (high != null)
			high.ForEach(x => x.SetAll(value));
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public virtual BigList<uint> ToUIntBigList()
	{
		if (low != null)
			return new(low.ToUIntList());
		else if (high != null)
			return new(E.SelectMany(high, x => x.ToUIntBigList()));
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	public virtual BigBitList Xor(BigBitList value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (Length != value.Length)
			throw new ArgumentException("Для побитовых операций текущий и второй списки бит должны иметь одинаковую длину.", nameof(value));
		if (low != null && value.low != null)
			low.Xor(value.low);
		else if (high != null && value.high != null)
			high = [.. high.Combine(value.high, (x, y) => x.Xor(y))];
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		return this;
	}
}

/// <summary>
/// Представляет строго типизированный список элементов, упорядоченных по индексу.
/// В отличие от <see cref="List{T}"/> и стандартного <see cref="G.List{T}"/>, имеет индекс типа <see cref="MpzT"/>, а не
/// <see langword="int"/>, что позволяет хранить больше элементов, чем <see cref="int.MaxValue"/>
/// (теоретически - предел типа <see cref="MpzT"/> равен 2 ^ <see cref="int.MaxValue"/> - 1, практически же даже самый мощный
/// суперкомпьютер имеет несравнимо меньшее количество памяти, но это уже проблемы этого суперкомпьютера, а не моей
/// коллекции). Методы для поиска, сортировки и других манипуляций со списком находятся в разработке, на текущий момент
/// поддерживаются только добавление в конец, установка элемента по индексу и частично удаление.
/// </summary>
[ComVisible(true), DebuggerDisplay("Length = {Length}"), Experimental("CS9216"), Serializable]
public class BigList<T> : BigList<T, BigList<T>, LimitedBuffer<T>>
{
	public BigList() : this(-1) { }

	public BigList(int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(subbranchesBitLength, leafSizeBitLength) { }

	public BigList(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(capacity, subbranchesBitLength, leafSizeBitLength) { }

	public BigList(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(collection, subbranchesBitLength, leafSizeBitLength) { }

	public BigList(MpzT capacity, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(capacity, collection, subbranchesBitLength, leafSizeBitLength) { }

	public BigList(T[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(values.AsEnumerable(), subbranchesBitLength, leafSizeBitLength) { }

	public BigList(ReadOnlySpan<T> values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(values, subbranchesBitLength, leafSizeBitLength) { }

	public BigList(MpzT capacity, T[] values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(capacity, values.AsEnumerable(), subbranchesBitLength, leafSizeBitLength) { }

	public BigList(MpzT capacity, ReadOnlySpan<T> values, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : base(capacity, values, subbranchesBitLength, leafSizeBitLength) { }

	protected override Func<MpzT, BigList<T>> CapacityCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	protected override Func<G.IEnumerable<T>, BigList<T>> CollectionCreator => x => new(x, SubbranchesBitLength, LeafSizeBitLength);

	protected override Func<int, LimitedBuffer<T>> CapacityLowCreator => x => new(x);

	protected override Func<G.IEnumerable<T>, LimitedBuffer<T>> CollectionLowCreator { get; } = x => new(x);
}
