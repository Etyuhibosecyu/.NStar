global using Corlib.NStar;
global using LINQ.NStar;
global using MathLib.NStar;
global using Mpir.NET;
global using SumCollections.NStar;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static System.Math;
global using E = System.Linq.Enumerable;

namespace BigCollections.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BigList<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow>
	where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected List<TCertain>? high;
	private protected BigSumList? highLength;
	private protected TCertain? parent;
	private protected MpzT _capacity = 0;
	private protected MpzT fragment = 1;
	private protected bool bReversed;

	public BigList() : this(-1) { }

	public BigList(int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength >= 2)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength >= 2)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength >= 2)
			LeafSizeBitLength = subbranchesBitLength;
		low = new();
		high = null;
		highLength = null;
		fragment = 1;
		Length = 0;
		_capacity = 0;
#if VERIFY
		Verify();
#endif
	}

	public BigList(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(subbranchesBitLength, leafSizeBitLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		ConstructFromCapacity(capacity);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(collection == null ? throw new ArgumentNullException(nameof(collection))
			  : collection.TryGetLengthEasily(out var length) ? length : 0,
			  leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(MpzT capacity, G.IEnumerable<T> collection, int subbranchesBitLength = -1,
		int leafSizeBitLength = -1) : this(capacity, leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(collection.Length, leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(MpzT capacity, ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(RedStarLinqMath.Max(capacity, collection.Length), leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public override MpzT Capacity
	{
		get => _capacity;
		set
		{
			var this2 = (TCertain)this;
			ArgumentOutOfRangeException.ThrowIfLessThan(value, Length);
			if (value == _capacity)
				return;
			if (parent != null && value > parent.fragment)
				value = parent.fragment;
			if (value == 0)
			{
				if (low != null)
					low.Capacity = 0;
				else
				{
					low = new();
					high?.ForEach(x => x?.Dispose());
					high?.Dispose();
					highLength?.Dispose();
				}
				high = null;
				highLength = null;
			}
			else if (value <= LeafSize) // Список сокращается до одного листа
			{
				if (high != null && highLength != null)
					ReduceCapacityExponential(LeafSize);
				if (high != null && highLength != null) // В этом случае в списке очень много пустоты
					Compactify();
				var first = this;
				// Ищем первый лист и разрушаем "лишние" ветки
				for (; first.high != null; first = first.high[0], first.parent?.high?.Dispose())
				{
					Debug.Assert(first.highLength != null);
					first.high.Skip(1).ForEach(x => x.Dispose());
					first.highLength.Dispose();
				}
				Debug.Assert(first.low != null);
				low = first.low;
				// Емкость листа должна быть типа int
				var value2 = (int)value;
				low.Capacity = value2;
				high = null;
				highLength = null;
			}
			else if (low != null) // Список расширяется с одного листа до многих
			{
				// Разберем эту строку по частям:
				// (value - 1).BitLength - какого порядка (по основанию 2) новая длина.
				// value.BitLength не подходит, потому что если новая емкость - степень двух,
				// это выражение вернет на один бо́льшую степень двух (например, 17 для 65536).
				// Минус LeafSizeBitLength - сколько порядков занимают именно ветки, а не элементы внутри листа.
				// GetArrayLength на SubbranchesBitLength - глубина дерева (количество уровней от корня до листьев).
				// Минус 1 - глубина каждой подветки корня.
				// Остальная часть выражения - вычисляет количество элементов в каждой подветке корня.
				fragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - LeafSizeBitLength,
					SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
				while (fragment << SubbranchesBitLength < value)
					fragment <<= SubbranchesBitLength;
				// Количество подветок корня
				var highCount = (int)GetArrayLength(value, fragment);
				high = new(highCount);
				// Создаем подветки
				for (MpzT i = 0; i < value / fragment; i++)
				{
					high.Add(CapacityCreator(fragment));
					high[^1].parent = this2;
				}
				// Если нужно, создаем последнюю неполную подветку
				if (value % fragment != 0)
				{
					high.Add(CapacityCreator(value % fragment));
					high[^1].parent = this2;
				}
				using var preservedLow = low;
				low = null;
				Length = 0;
				var first = this;
				// Устанавливаем highLength на всех уровнях
				for (; first.high != null; first = first.high[0])
					first.highLength = [preservedLow.Length];
				ArgumentNullException.ThrowIfNull(first.low);
				// Вставляем первый лист
				first.low.AddRange(preservedLow);
				if (preservedLow.Length != 0)
					first.Length = preservedLow.Length;
			}
			else if (high != null && highLength != null) // Как старая, так и новая емкость определяет много листьев
			{
				// См. подробный разбор аналогичной строки в предыдущем разделе
				var newFragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - LeafSizeBitLength,
					SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
				while (newFragment << SubbranchesBitLength < value)
					newFragment <<= SubbranchesBitLength;
				if (newFragment > fragment) // Новое дерево существенно больше старого (до уровня разной глубины)
					IncreaseCapacityExponential(value, newFragment);
				else if (newFragment < fragment) // Старое дерево существенно больше
				{
					ReduceCapacityExponential(newFragment);
					AddCapacity((fragment << SubbranchesBitLength) - _capacity);
					ReduceCapacityLinear(value);
				}
				else if (value > _capacity) // Новое дерево на несколько веток больше старого
					IncreaseCapacityLinear(value);
				else // Старое дерево на несколько веток больше
					ReduceCapacityLinear(value);
			}
			AddCapacity(value - _capacity);
#if VERIFY
			if (high != null && highLength != null)
				Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Verify();
#endif
		}
	}

	protected virtual bool IsReversed => bReversed ^ (parent?.IsReversed ?? false);

	protected virtual int LeafSizeBitLength { get; init; } = 16;

	protected virtual int LeafSize => 1 << LeafSizeBitLength;

	protected virtual int SubbranchesBitLength { get; init; } = 16;

	protected virtual int Subbranches => 1 << SubbranchesBitLength;

	public override MpzT Length
	{
		get => base.Length;
		private protected set
		{
			if (parent != null)
				parent.Length += value - base.Length;
			base.Length = value;
		}
	}

	protected virtual TCertain Root => parent == null ? (TCertain)this : parent.Root;

	public override TCertain Add(T item)
	{
		var this2 = (TCertain)this;
		EnsureCapacity(Length + 1);
	start:
		if (low != null)
		{
			low.Insert(bReversed ? 0 : ^0, item);
			Length += 1;
		}
		else if (high != null && highLength != null)
		{
			var bReversedOld = bReversed;
			if (bReversedOld)
			{
				InsertInternal(Length, item);
				return this2;
			}
			var intIndex = highLength.IndexOfNotGreaterSum(Length, out var bitsIndex);
			if (intIndex != 0 && high[intIndex - 1].Capacity != high[intIndex - 1].Length)
				intIndex--;
			if (high.Length == intIndex)
			{
				if (high.Length < Subbranches && high[^1].Capacity == fragment)
				{
					high.Capacity = Min(high.Capacity << 1, Subbranches);
					high.Add(CapacityCreator(fragment));
					high[^1].parent = this2;
					AddCapacity(fragment);
				}
				else
				{
					Compactify();
					goto start;
				}
			}
			high[intIndex].Add(item);
			if (highLength.Length == intIndex)
				highLength.Add(1);
			else
				highLength[intIndex]++;
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
		return this2;
	}

	protected virtual void AddCapacity(MpzT increment)
	{
		if (increment == 0)
			return;
		for (var list = this; list != null; list = list.parent)
			list._capacity += increment;
	}

	protected override void ClearInternal(bool verify = true)
	{
		if (low != null)
		{
			low.Clear(false);
			Length = 0;
		}
		else
		{
			high?.ForEach(x => x?.Clear());
			highLength?.Clear();
		}
#if VERIFY
		if (verify)
			Verify();
#endif
	}

	protected override void ClearInternal(MpzT index, MpzT length)
	{
		if (length == 0)
			return;
		if (low != null)
			low.Clear((int)(bReversed ? Length - length - index : index), (int)length);
		else if (high != null && highLength != null)
		{
			var endIndex = index + length - 1;
			if (bReversed)
				(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
			var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
			var endIntIndex = highLength.IndexOfNotGreaterSum(endIndex);
			if (intIndex == endIntIndex)
			{
				high[intIndex].ClearInternal(bitsIndex, length);
				return;
			}
			var previousPart = highLength[intIndex] - bitsIndex;
			high[intIndex].ClearInternal(bitsIndex, previousPart);
			for (var i = intIndex + 1; i < endIntIndex; i++)
			{
				high[i].ClearInternal(0, highLength[i]);
				previousPart += highLength[i];
			}
			high[endIntIndex].ClearInternal(0, length - previousPart);
		}
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void Compactify()
	{
		Debug.Assert(high != null && highLength != null);
		for (var i = 1; i < highLength.Length; i++)
		{
			if (highLength[i - 1] == fragment)
				continue;
			var amount = fragment - highLength[i - 1];
			var highElementI = high[i];
			var highLengthElementI = highLength[i];
			var highElementBeforeI = high[i - 1];
			if (highElementBeforeI.high != null && highElementBeforeI.highLength != null
				&& highElementBeforeI.highLength.ValuesSum + highElementBeforeI.fragment - highElementBeforeI.highLength[^1]
				!= highElementBeforeI.highLength.Length * highElementBeforeI.fragment)
				highElementBeforeI.Compactify();
			if (highLengthElementI > amount)
			{
				highElementI.CopyToInternal(0, highElementBeforeI, highLength[i - 1], amount);
				highElementI.RemoveInternal(0, amount);
				highLength[i - 1] = fragment;
				highLength[i] -= amount;
			}
			else
			{
				highElementI.CopyToInternal(0, highElementBeforeI,
					highLength[i - 1], highLengthElementI);
				var temp = highElementI;
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				if (i != highLength.Length - 1)
				{
					high.CopyRangeTo(i + 1, high, i, high.Length - i - offsetFromEnd - 1);
					high[^(offsetFromEnd + 1)] = temp;
				}
				temp.RemoveEndInternal(RedStarLinqMath.Min(high[^1].Length, temp.Length));
				high[^1].CopyToInternal(0, temp, 0, high[^1].Length);
				high[^1].ClearInternal(false);
				highLength[i - 1] += highLengthElementI;
				highLength.RemoveAt(i);
				i--;
			}
		}
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void ConstructFromCapacity(MpzT capacity)
	{
		if (_capacity >= capacity)
			return;
		AddCapacity(-_capacity);
		var this2 = (TCertain)this;
		if (capacity <= LeafSize)
		{
			low = CapacityLowCreator((int)capacity);
			high = null;
			highLength = null;
			fragment = 1;
			AddCapacity(capacity - _capacity);
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - LeafSizeBitLength,
				SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
			while (fragment << SubbranchesBitLength < capacity)
				fragment <<= SubbranchesBitLength;
			var intIndex = capacity.Divide(fragment, out var bitsIndex);
			var highCount = (int)GetArrayLength(capacity, fragment);
			high = new(highCount);
			highLength = [];
			for (MpzT i = 0; i < intIndex; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			if (bitsIndex != 0)
			{
				high.Add(CapacityCreator(bitsIndex));
				high[^1].parent = this2;
				AddCapacity(bitsIndex);
			}
		}
		Length = 0;
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void ConstructFromEnumerable(G.IEnumerable<T> collection)
	{
		if (collection is G.IReadOnlyList<T> list)
			ConstructFromList(list);
		else if (collection is G.IList<T> list2)
			ConstructFromList(list2.GetSlice());
		else if (collection is BigList<T, TCertain, TLow> bigList)
		{
			if (bigList.low != null)
				ConstructFromList(bigList.low);
			else if (bigList.high != null && bigList.highLength != null)
			{
				ConstructFromCapacity(bigList.Length);
				bigList.CopyToInternal(0, (TCertain)this, 0, bigList.Length);
			}
		}
		else if (collection.TryGetLengthEasily(out _))
			ConstructFromList(RedStarLinq.ToList(collection));
		else
		{
			using var en = collection.GetEnumerator();
			while (en.MoveNext())
				Add(en.Current);
		}
	}

	protected virtual void ConstructFromList(G.IReadOnlyList<T> list)
	{
		if ((low == null || low.Capacity == 0) && high == null && highLength == null && fragment == 1 && _capacity == 0)
		{
			ConstructFromListFromScratch(list);
			return;
		}
		if (list.Count <= LeafSize && low != null && high == null && highLength == null && fragment == 1)
		{
			if (low == null)
				low = CollectionLowCreator(list);
			else
				low.AddRange(CollectionLowCreator(list));
			Length = list.Count;
		}
		else
		{
			Debug.Assert(low == null && high != null && highLength != null && fragment != 1);
			var fragment2 = (int)fragment;
			var i = 0;
			var index = 0;
			for (; index <= list.Count - fragment2; index += fragment2)
			{
				high[i++].ConstructFromList(list.GetROLSlice(index, fragment2));
				highLength.Add(fragment);
			}
			var rest = list.Count - index;
			Debug.Assert(rest < fragment);
			if (rest != 0)
			{
				high[i].ConstructFromList(list.GetROLSlice(index));
				highLength.Add(rest);
			}
		}
		AddCapacity(Length - _capacity);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void ConstructFromListFromScratch(G.IReadOnlyList<T> list)
	{
		Debug.Assert((low == null || low.Capacity == 0) && high == null && highLength == null && fragment == 1 && _capacity == 0);
		if (list.Count <= LeafSize && high == null && highLength == null && fragment == 1)
		{
			low = CollectionLowCreator(list);
			high = null;
			highLength = null;
			fragment = 1;
			AddCapacity(list.Count);
			Length = list.Count;
		}
		else
		{
			low = null;
			fragment = 1 << ((((MpzT)list.Count - 1).BitLength + SubbranchesBitLength - 1 - LeafSizeBitLength)
				/ SubbranchesBitLength - 1) * SubbranchesBitLength + LeafSizeBitLength;
			var fragment2 = (int)fragment;
			high = new(GetArrayLength(list.Count, fragment2));
			highLength = [];
			var index = 0;
			for (; index <= list.Count - fragment2; index += fragment2)
			{
				high.Add(CapacityCreator(0));
				high[^1].parent = (TCertain)this;
				high[^1].ConstructFromListFromScratch(list.GetROLSlice(index, fragment2));
				highLength.Add(fragment);
			}
			if (list.Count % fragment2 != 0)
			{
				var rest = list.Count - index;
				high.Add(CapacityCreator(0));
				high[^1].parent = (TCertain)this;
				high[^1].ConstructFromListFromScratch(list.GetROLSlice(index));
				highLength.Add(rest);
			}
		}
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected static void CheckParams(CopyRangeContext context)
	{
		if (context.Source.Capacity == 0 && !(context.SourceIndex == 0 && context.Length == 0))
			throw new ArgumentException("Исходный массив не может быть пустым.");
		if (context.Destination == null)
			throw new ArgumentNullException(nameof(context.Destination), "Целевой массив не может быть нулевым.");
		if (context.Destination.Capacity == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(context.Destination));
		if (context.SourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context.SourceIndex), "Индекс не может быть отрицательным.");
		if (context.DestinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context.DestinationIndex), "Индекс не может быть отрицательным.");
		if (context.Length < 0)
			throw new ArgumentOutOfRangeException(nameof(context.Length), "Длина не может быть отрицательной.");
		if (context.SourceIndex + context.Length > context.Source.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (context.DestinationIndex + context.Length > context.Destination.Capacity)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	protected static void CopyRange(TCertain source, MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length, bool ignoreReversed = false)
	{
		List<CopyRangeContext> stack
			= [(source, sourceIndex, destination, destinationIndex, length, ignoreReversed)];
		while (stack.Length > 0)
			CopyRangeIteration(stack);
	}

	protected static void CopyRangeIteration(List<CopyRangeContext> stack)
	{
		var context = stack.GetAndRemove(^1);
		CheckParams(context);
		if (context.Length == 0)
			return;
		var stackIndex = stack.Length;
		var fragment = context.Source.fragment;
		// Тривиальный случай - оба списка являются листьями дерева
		if (context.Source.low != null && context.Destination.low != null)
		{
			CopyRangeTrivial(context.Source, context.SourceIndex, context.Destination, context.DestinationIndex, context.Length, context.IgnoreReversed);
			return;
		}
		// Только целевой список является листом дерева
		else if (context.Destination.low != null && context.Source.high != null && context.Source.highLength != null)
		{
			CopyRangeToLeaf(stack, (context.Source, context.SourceIndex, context.Destination, context.DestinationIndex, context.Length, context.IgnoreReversed), stackIndex, fragment);
			return;
		}
		// Исходный список является более мелкой веткой (возможно, даже листом)
		else if ((context.Source.low != null || context.Destination.fragment > fragment) && context.Destination.high != null && context.Destination.highLength != null)
		{
			CopyRangeToLarger(stack, (context.Source, context.SourceIndex, context.Destination, context.DestinationIndex, context.Length, context.IgnoreReversed), stackIndex);
			return;
		}
		// Самый сложный случай: исходный список является соизмеримой или более крупной веткой,
		// а целевой также не является листом
		CopyRangeToSimilar(stack, (context.Source, context.SourceIndex, context.Destination, context.DestinationIndex, context.Length, context.IgnoreReversed), stackIndex);
	}

	protected static void CopyRangeToLarger(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment) && context.Destination.high != null && context.Destination.highLength != null);
		var fragment = context.Destination.fragment;
	destinationFragmentBigger:
		MpzT destinationIndexOld = new(context.DestinationIndex); // Сохраняем значение как изменяющееся после реверса
		var endIndex = context.DestinationIndex + context.Length - 1;
		// Если один или оба списка перевернуты, корректируем индексы
		if (context.Source.bReversed && !context.IgnoreReversed)
			context.SourceIndex = context.Source.Length - context.Length - context.SourceIndex;
		// Если целевой список перевернут и в нем не хватает места, расширяем
		if (context.Destination.bReversed && !context.IgnoreReversed && context.DestinationIndex + context.Length > context.Destination.Length)
		{
			var dummy = context.Source.CapacityCreator(RedStarLinqMath.Max(context.DestinationIndex + context.Length - (context.Destination.Length << 1), 0));
			dummy.Length = dummy.Capacity;
			context.Destination.InsertInternal(0, dummy);
			stack.Insert(stackIndex, (context.Destination, 0, context.Destination, context.DestinationIndex + context.Length - context.Destination.Length, context.Destination.Length, true));
		}
		if (context.Destination.bReversed && !context.IgnoreReversed)
			(context.DestinationIndex, endIndex) = (context.Destination.Length - 1 - endIndex,
				context.Destination.Length - 1 - context.DestinationIndex);
		CopyRangeIndex start = new(context.Destination.highLength, context.DestinationIndex);
		CopyRangeIndex end = new(context.Destination.highLength, endIndex);
		MpzT index2Old = new(end.IntIndex);
		// Диапазон дописывается после конца списка
		if (start.IntIndex != 0 && start.IntIndex == context.Destination.highLength.Length && start.RestIndex == 0 && context.Destination.highLength[^1] != fragment)
			start = (start.Index, start.IntIndex - 1, context.Destination.highLength[^1]);
		// Диапазон частично расположен за концом списка
		if (end.IntIndex != 0 && end.IntIndex == context.Destination.highLength.Length)
		{
			if (end.RestIndex >= fragment - context.Destination.highLength[^1])
				end = (end.Index, end.IntIndex, end.RestIndex - (fragment - context.Destination.highLength[^1]));
			else
				end = (end.Index, end.IntIndex - 1, end.RestIndex + context.Destination.highLength[^1]);
		}
		while (end.RestIndex >= fragment)
			end = (end.Index, end.IntIndex + 1, end.RestIndex - fragment);
		if (end.IntIndex >= context.Destination.high.Length || context.Destination.highLength.Length != 0
			&& context.Destination.highLength.ValuesSum + fragment - context.Destination.highLength[^1]
			+ (context.Destination.high.Length - context.Destination.highLength.Length - 1) * fragment
			+ context.Destination.high[^1].Capacity < context.DestinationIndex + context.Length)
		{
			context.Destination.Compactify();
			goto destinationFragmentBigger;
		}
		// Весь диапазон помещается в одну ветку
		if (start.IntIndex == end.IntIndex)
		{
			stack.Insert(stackIndex, (context.Source, context.SourceIndex, context.Destination.high[start.IntIndex], start.RestIndex, context.Length, true));
			context.Destination.highLength.SetOrAdd(start.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length
				> start.IntIndex ? context.Destination.highLength[start.IntIndex] : 0, start.RestIndex + context.Length));
		}
		else if (context.Source.IsReversed != context.Destination.IsReversed)
			CopyRangeToLargerDiagonal(stack, context, stackIndex, fragment, (start, end));
		else if (context.SourceIndex >= context.DestinationIndex || context.Source != context.Destination)
			CopyRangeToLargerLTR(stack, context, stackIndex, fragment, (start, end));
		else
			CopyRangeToLargerRTL(stack, context, stackIndex, fragment, (start, end), index2Old);
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToLargerDiagonal(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment) && context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var previousPart = RedStarLinqMath.Min((indexes.Start.IntIndex < context.Destination.highLength.Length - 1
			? context.Destination.highLength[indexes.Start.IntIndex] : fragment) - indexes.Start.RestIndex, context.Length);
		stack.Insert(stackIndex, (context.Source, context.SourceIndex + context.Length - previousPart, context.Destination.high[indexes.Start.IntIndex], indexes.Start.RestIndex, previousPart, true));
		context.Destination.highLength.SetOrAdd(indexes.Start.IntIndex, RedStarLinqMath.Max(indexes.Start.IntIndex < context.Destination.highLength.Length - 1
			? context.Destination.highLength[indexes.Start.IntIndex] : 0, indexes.Start.RestIndex + previousPart));
		for (var i = indexes.Start.IntIndex + 1; i < indexes.End.IntIndex; i++)
		{
			var branchLength = i < context.Destination.highLength.Length - 1 ? context.Destination.highLength[i] : fragment;
			previousPart += branchLength;
			stack.Insert(stackIndex, (context.Source, context.SourceIndex + context.Length - previousPart, context.Destination.high[i], 0, branchLength, true));
			context.Destination.highLength.SetOrAdd(i, branchLength);
		}
		stack.Insert(stackIndex, (context.Source, context.SourceIndex, context.Destination.high[indexes.End.IntIndex], 0, context.Length - previousPart, true));
		if (context.Length != previousPart)
			context.Destination.highLength.SetOrAdd(indexes.End.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length > indexes.End.IntIndex ? context.Destination.highLength[indexes.End.IntIndex] : 0, context.Length - previousPart));
	}

	protected static void CopyRangeToLargerLTR(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment) && context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон копируется слева направо
		var previousPart = RedStarLinqMath.Min((indexes.Start.IntIndex < context.Destination.highLength.Length - 1
			? context.Destination.highLength[indexes.Start.IntIndex] : fragment) - indexes.Start.RestIndex, context.Length);
		stack.Insert(stackIndex, (context.Source, context.SourceIndex, context.Destination.high[indexes.Start.IntIndex], indexes.Start.RestIndex, previousPart, true));
		context.Destination.highLength.SetOrAdd(indexes.Start.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length
			> indexes.Start.IntIndex ? context.Destination.highLength[indexes.Start.IntIndex] : 0, indexes.Start.RestIndex + previousPart));
		for (var i = indexes.Start.IntIndex + 1; i < indexes.End.IntIndex; i++)
		{
			var branchLength = i < context.Destination.highLength.Length - 1 ? context.Destination.highLength[i] : fragment;
			stack.Insert(stackIndex, (context.Source, context.SourceIndex + previousPart, context.Destination.high[i], 0, branchLength, true));
			context.Destination.highLength.SetOrAdd(i, branchLength);
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (context.Source, context.SourceIndex + previousPart, context.Destination.high[indexes.End.IntIndex], 0, context.Length - previousPart, true));
		if (context.Length != previousPart)
			context.Destination.highLength.SetOrAdd(indexes.End.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length > indexes.End.IntIndex ? context.Destination.highLength[indexes.End.IntIndex] : 0, context.Length - previousPart));
	}

	protected static void CopyRangeToLargerRTL(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide indexes, MpzT index2Old)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment) && context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон копируется справа налево
		for (var i = context.Destination.highLength.Length; i < indexes.End.IntIndex; i++)
			context.Destination.highLength.Add(1);
		var previousPart = RedStarLinqMath.Min(fragment - indexes.Start.RestIndex, context.Length) + (indexes.End.IntIndex - indexes.Start.IntIndex - 1) * fragment;
		stack.Insert(stackIndex, (context.Source, context.SourceIndex + previousPart, context.Destination.high[indexes.End.IntIndex], 0, context.Length - previousPart, true));
		if (context.Length != previousPart)
			context.Destination.highLength.SetOrAdd(indexes.End.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length > indexes.End.IntIndex ? context.Destination.highLength[indexes.End.IntIndex] : 0, context.Length - previousPart));
		for (var i = indexes.End.IntIndex - 1; i > indexes.Start.IntIndex; i--)
		{
			var branchLength = i < index2Old ? context.Destination.highLength[i] : fragment;
			previousPart -= branchLength;
			stack.Insert(stackIndex, (context.Source, context.SourceIndex + previousPart, context.Destination.high[i], 0, branchLength, true));
			context.Destination.highLength[i] = branchLength;
		}
		stack.Insert(stackIndex, (context.Source, context.SourceIndex, context.Destination.high[indexes.Start.IntIndex], indexes.Start.RestIndex, previousPart, true));
		context.Destination.highLength[indexes.Start.IntIndex] = RedStarLinqMath.Max(context.Destination.highLength.Length > indexes.Start.IntIndex ? context.Destination.highLength[indexes.Start.IntIndex] : 0, indexes.Start.RestIndex + previousPart);
	}

	protected static void CopyRangeToLeaf(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		MpzT destinationIndexOld = new(context.DestinationIndex); // Сохраняем значение как изменяющееся после реверса
		var endIndex = context.SourceIndex + context.Length - 1;
		// Если один или оба списка перевернуты, корректируем индексы
		if (context.Source.bReversed && !context.IgnoreReversed)
			(context.SourceIndex, endIndex) = (context.Source.Length - 1 - endIndex, context.Source.Length - 1 - context.SourceIndex);
		if (context.Destination.bReversed && !context.IgnoreReversed)
			context.DestinationIndex = context.Destination.Length - context.Length - context.DestinationIndex;
		CopyRangeIndex start = new(context.Source.highLength, context.SourceIndex);
		CopyRangeIndex end = new(context.Source.highLength, endIndex);
		// Диапазон дописывается после конца списка
		if (start.IntIndex != 0 && start.IntIndex == context.Source.highLength.Length && start.RestIndex == 0 && context.Source.highLength[^1] != fragment)
			start = (start.Index, start.IntIndex - 1, context.Source.highLength[^1]);
		// Диапазон частично расположен за концом списка
		if (start.IntIndex != 0 && start.IntIndex == context.Source.highLength.Length)
		{
			if (end.RestIndex >= fragment - context.Source.highLength[^1])
				end = (end.Index, end.IntIndex, end.RestIndex - (fragment - context.Source.highLength[^1]));
			else
				end = (end.Index, end.IntIndex - 1, end.RestIndex + context.Source.highLength[^1]);
		}
		// Весь диапазон помещается в одну ветку
		if (start.IntIndex == end.IntIndex)
			stack.Insert(stackIndex, (context.Source.high[start.IntIndex], start.RestIndex, context.Destination, context.DestinationIndex, context.Length, true));
		else if (context.Source.IsReversed != context.Destination.IsReversed)
			CopyRangeToLeafDiagonal(stack, context, stackIndex, fragment, (start, end));
		else if (context.SourceIndex >= context.DestinationIndex || context.Source != context.Destination)
			CopyRangeToLeafLTR(stack, context, stackIndex, fragment, (start, end));
		else
			CopyRangeToLeafRTL(stack, context, stackIndex, fragment, (start, end));
		context.Destination.highLength?.SetOrAdd(start.IntIndex, context.Destination.highLength.Length
			> start.IntIndex ? context.Destination.highLength[start.IntIndex] : context.DestinationIndex + context.Length);
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToLeafDiagonal(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var (start, end) = indexes;
		var previousPart = end.RestIndex + 1;
		stack.Insert(stackIndex, (context.Source.high[end.IntIndex], 0, context.Destination, context.DestinationIndex, previousPart, true));
		for (var i = end.IntIndex - 1; i > start.IntIndex; i--)
		{
			var branchLength = i < context.Source.highLength.Length - 1 ? context.Source.highLength[i] : fragment;
			stack.Insert(stackIndex, (context.Source.high[i], 0, context.Destination, context.DestinationIndex + previousPart, branchLength, true));
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (context.Source.high[start.IntIndex], start.RestIndex, context.Destination, context.DestinationIndex + previousPart, context.Length - previousPart, true));
	}

	protected static void CopyRangeToLeafLTR(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		// Диапазон копируется слева направо
		var (start, end) = indexes;
		var previousPart = (start.IntIndex < context.Source.highLength.Length - 1 ? context.Source.highLength[start.IntIndex] : fragment) - start.RestIndex;
		stack.Insert(stackIndex, (context.Source.high[start.IntIndex], start.RestIndex, context.Destination, context.DestinationIndex, previousPart, true));
		for (var i = start.IntIndex + 1; i < end.IntIndex; i++)
		{
			var branchLength = i < context.Source.highLength.Length - 1 ? context.Source.highLength[i] : fragment;
			stack.Insert(stackIndex, (context.Source.high[i], 0, context.Destination, context.DestinationIndex + previousPart, branchLength, true));
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (context.Source.high[end.IntIndex], 0, context.Destination, context.DestinationIndex + previousPart, context.Length - previousPart, true));
	}

	protected static void CopyRangeToLeafRTL(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		// Диапазон копируется справа налево
		var (start, end) = indexes;
		var previousPart = (end.IntIndex < context.Source.highLength.Length - 1 ? context.Source.highLength[start.IntIndex] : fragment) - start.RestIndex;
		for (var i = start.IntIndex + 1; i < end.IntIndex; i++)
			previousPart += context.Source.highLength[i];
		stack.Insert(stackIndex, (context.Source.high[end.IntIndex], 0, context.Destination, context.DestinationIndex + previousPart, context.Length - previousPart, true));
		for (var i = end.IntIndex - 1; i > start.IntIndex; i--)
		{
			var branchLength = i < context.Source.highLength.Length - 1 ? context.Source.highLength[i] : fragment;
			previousPart -= branchLength;
			stack.Insert(stackIndex, (context.Source.high[i], 0, context.Destination, context.DestinationIndex + previousPart, branchLength, true));
		}
		stack.Insert(stackIndex, (context.Source.high[start.IntIndex], start.RestIndex, context.Destination, context.DestinationIndex, previousPart, true));
	}

	protected static void CopyRangeToSimilar(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex)
	{
		if (!(context.Source.high != null && context.Source.highLength != null && context.Destination.high != null && context.Destination.highLength != null))
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		if (context.DestinationIndex > context.Destination.Length)
			throw new ArgumentOutOfRangeException(nameof(context),
				"Индекс не может быть больше длины содержимого цели.");
		var fragment = context.Destination.fragment;
		var compactified = false;
	sameFragment:
		if (context.Length == 0)
			return;
		var sourceEndIndex = context.SourceIndex + context.Length - 1;
		var destinationEndIndex = context.DestinationIndex + context.Length - 1;
		// Если один или оба списка перевернуты, корректируем индексы
		if (context.Source.bReversed && !context.IgnoreReversed)
			(context.SourceIndex, sourceEndIndex) = (context.Source.Length - 1 - sourceEndIndex, context.Source.Length - 1 - context.SourceIndex);
		if (context.Destination.bReversed && !context.IgnoreReversed)
			(context.DestinationIndex, destinationEndIndex) = (context.Destination.Length - 1 - destinationEndIndex, context.Destination.Length - 1 - context.DestinationIndex);
		CopyRangeIndex sourceStart = new(context.Source.highLength, context.SourceIndex);
		CopyRangeIndex sourceEnd = new(context.Source.highLength, sourceEndIndex);
		CopyRangeIndex destinationStart = new(context.Destination.highLength, context.DestinationIndex);
		CopyRangeIndex destinationEnd = new(context.Destination.highLength, destinationEndIndex);
		// Сохраняем значение как изменяющееся после реверса
		while (sourceEnd.RestIndex >= context.Source.fragment)
			sourceEnd = (sourceEnd.Index, sourceEnd.IntIndex + 1, sourceEnd.RestIndex - context.Source.fragment);
		if (context.Destination.highLength.Length != 0 && destinationEnd.IntIndex == context.Destination.highLength.Length)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex, destinationEnd.RestIndex - (fragment - context.Destination.highLength[^1]));
		while (destinationEnd.RestIndex >= fragment)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex + 1, destinationEnd.RestIndex - fragment);
		// Диапазон дописывается после конца списка
		if (destinationStart.IntIndex != 0 && destinationStart.IntIndex == context.Destination.highLength.Length && destinationStart.RestIndex == 0 && context.Destination.highLength[destinationStart.IntIndex - 1] != fragment)
			destinationStart = (destinationStart.Index, destinationStart.IntIndex - 1, context.Destination.highLength[^1]);
		if (destinationEnd.IntIndex != 0 && destinationEnd.RestIndex < 0)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex - 1, destinationEnd.RestIndex + fragment);
		// Если список после серии удалений стал слишком "рыхлым", уплотняем
		if (destinationEnd.IntIndex >= context.Destination.high.Length || destinationEnd.RestIndex < 0
			|| context.Destination.highLength.Length != 0 && context.Destination.highLength.ValuesSum + fragment - context.Destination.highLength[^1]
			+ (context.Destination.high.Length - context.Destination.highLength.Length - 1) * fragment
			+ context.Destination.high[^1].Capacity < context.DestinationIndex + context.Length)
		{
			Debug.Assert(!compactified);
			context.Destination.Compactify();
			compactified = true;
			goto sameFragment;
		}
		else
		{
			Debug.Assert(sourceStart.IntIndex >= 0 && sourceStart.IntIndex < context.Source.highLength.Length
				&& sourceEnd.IntIndex >= 0 && sourceEnd.IntIndex < context.Source.highLength.Length
				&& destinationStart.IntIndex >= 0 && destinationStart.IntIndex < context.Destination.high.Length
				&& destinationEnd.IntIndex >= 0 && destinationEnd.IntIndex < context.Destination.high.Length
				&& sourceStart.RestIndex >= 0 && sourceEnd.RestIndex >= 0 && destinationStart.RestIndex >= 0 && destinationEnd.RestIndex >= 0);
			if (context.Source.IsReversed != context.Destination.IsReversed)
				CopyRangeToSimilarDiagonal(stack, context, stackIndex, fragment, (sourceStart, sourceEnd), (destinationStart, destinationEnd));
			else if (context.SourceIndex >= context.DestinationIndex || context.DestinationIndex >= context.SourceIndex + context.Length || context.Source != context.Destination)
				CopyRangeToSimilarLTR(stack, context, stackIndex, fragment, (sourceStart, sourceEnd), (destinationStart, destinationEnd));
			else
				CopyRangeToSimilarRTL(stack, context, stackIndex, fragment, (sourceStart, sourceEnd), (destinationStart, destinationEnd));
		}
	}

	protected static void CopyRangeToSimilarDiagonal(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		Debug.Assert(context.Source.high != null && context.Source.highLength != null && context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var sourceCurrentRestIndex = sourceIndexes.End.RestIndex + 1;
		var destinationCurrentRestIndex = destinationIndexes.Start.RestIndex;
		var sourceCurrentIntIndex = sourceIndexes.End.IntIndex;
		var destinationCurrentIntIndex = destinationIndexes.Start.IntIndex;
		var startRestIndexDiff = (sourceIndexes.Start.RestIndex - destinationIndexes.Start.RestIndex).Abs();
		startRestIndexDiff = RedStarLinqMath.Max(startRestIndexDiff, fragment - startRestIndexDiff);
		MpzT step = 0;
		var leftLength = context.Length;
		while (leftLength > 0)
		{
			var destinationMax = context.Destination.highLength.Length > destinationCurrentIntIndex + 1
				? context.Destination.highLength[destinationCurrentIntIndex] : fragment;
			step = RedStarLinqMath.Min(context.Source.highLength[sourceCurrentIntIndex], startRestIndexDiff,
				(destinationCurrentIntIndex == destinationIndexes.End.IntIndex ? destinationIndexes.End.RestIndex + 1 : fragment) - step,
				sourceCurrentRestIndex, destinationMax - destinationCurrentRestIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			var sourceThresholdReached = (sourceCurrentRestIndex -= step) == 0;
			stack.Insert(stackIndex, (context.Source.high[sourceCurrentIntIndex], sourceCurrentRestIndex,
				context.Destination.high[destinationCurrentIntIndex], destinationCurrentRestIndex, step, true));
			var destinationThresholdReached = (destinationCurrentRestIndex += step) == destinationMax;
			leftLength -= step;
			if (sourceThresholdReached)
			{
				sourceCurrentIntIndex--;
				sourceCurrentRestIndex = sourceCurrentIntIndex >= sourceIndexes.Start.IntIndex ? context.Source.highLength[sourceCurrentIntIndex] : fragment;
			}
			if (destinationThresholdReached)
			{
				context.Destination.highLength.SetOrAdd(destinationCurrentIntIndex, destinationCurrentRestIndex);
				destinationCurrentIntIndex++;
				destinationCurrentRestIndex = 0;
			}
		}
		context.Destination.highLength.SetOrAdd(destinationIndexes.End.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length
			> destinationIndexes.End.IntIndex ? context.Destination.highLength[destinationIndexes.End.IntIndex] : 0, destinationIndexes.End.RestIndex + 1));
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToSimilarLTR(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		Debug.Assert(context.Source.high != null && context.Source.highLength != null && context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон копируется слева направо
		var sourceCurrentRestIndex = sourceIndexes.Start.RestIndex;
		var destinationCurrentRestIndex = destinationIndexes.Start.RestIndex;
		var sourceCurrentIntIndex = sourceIndexes.Start.IntIndex;
		var destinationCurrentIntIndex = destinationIndexes.Start.IntIndex;
		var startRestIndexDiff = (sourceIndexes.Start.RestIndex - destinationIndexes.Start.RestIndex).Abs();
		startRestIndexDiff = RedStarLinqMath.Max(startRestIndexDiff, fragment - startRestIndexDiff);
		MpzT step = 0;
		var leftLength = context.Length;
		while (leftLength > 0)
		{
			var destinationMax = context.Destination.highLength.Length > destinationCurrentIntIndex + 1
				? context.Destination.highLength[destinationCurrentIntIndex] : fragment;
			step = RedStarLinqMath.Min(context.Source.highLength[sourceCurrentIntIndex] - step, startRestIndexDiff,
				(destinationCurrentIntIndex == destinationIndexes.End.IntIndex ? destinationIndexes.End.RestIndex + 1 : fragment) - step,
				context.Source.highLength[sourceCurrentIntIndex] - sourceCurrentRestIndex, destinationMax - destinationCurrentRestIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			stack.Insert(stackIndex, (context.Source.high[sourceCurrentIntIndex], sourceCurrentRestIndex,
				context.Destination.high[destinationCurrentIntIndex], destinationCurrentRestIndex, step, true));
			var sourceThresholdReached = (sourceCurrentRestIndex += step) == context.Source.highLength[sourceCurrentIntIndex];
			var destinationThresholdReached = (destinationCurrentRestIndex += step) == destinationMax;
			leftLength -= step;
			if (sourceThresholdReached)
			{
				sourceCurrentIntIndex++;
				sourceCurrentRestIndex = 0;
			}
			if (destinationThresholdReached)
			{
				context.Destination.highLength.SetOrAdd(destinationCurrentIntIndex, destinationCurrentRestIndex);
				destinationCurrentIntIndex++;
				destinationCurrentRestIndex = 0;
			}
		}
		context.Destination.highLength.SetOrAdd(destinationIndexes.End.IntIndex, RedStarLinqMath.Max(context.Destination.highLength.Length
			> destinationIndexes.End.IntIndex ? context.Destination.highLength[destinationIndexes.End.IntIndex] : 0, destinationIndexes.End.RestIndex + 1));
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToSimilarRTL(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		Debug.Assert(context.Source.high != null && context.Source.highLength != null && context.Destination.high != null && context.Destination.highLength != null);
		MpzT destinationHighLengthOld = new(context.Destination.highLength.Length);
		for (var i = context.Destination.highLength.Length; i < destinationIndexes.End.IntIndex + 1; i++)
			context.Destination.highLength.Add(1);
		// Диапазон копируется справа налево
		var sourceCurrentRestIndex = sourceIndexes.End.RestIndex + 1;
		var destinationCurrentRestIndex = destinationIndexes.End.RestIndex + 1;
		var sourceCurrentIntIndex = sourceIndexes.End.IntIndex;
		var destinationCurrentIntIndex = destinationIndexes.End.IntIndex;
		var startRestIndexDiff = (sourceIndexes.Start.RestIndex - destinationIndexes.Start.RestIndex).Abs();
		startRestIndexDiff = RedStarLinqMath.Max(startRestIndexDiff, fragment - startRestIndexDiff);
		MpzT step = 0;
		context.Destination.highLength[destinationIndexes.End.IntIndex] = RedStarLinqMath.Max(context.Destination.highLength.Length > destinationIndexes.End.IntIndex
			? context.Destination.highLength[destinationIndexes.End.IntIndex] : 0, destinationIndexes.End.RestIndex + 1);
		var leftLength = context.Length;
		while (leftLength > 0)
		{
			step = RedStarLinqMath.Min(context.Source.highLength[sourceCurrentIntIndex] - step, startRestIndexDiff,
				(destinationCurrentIntIndex == destinationIndexes.End.IntIndex ? destinationIndexes.End.RestIndex + 1 : destinationCurrentIntIndex
					< destinationHighLengthOld ? context.Destination.highLength[destinationCurrentIntIndex] : fragment)
				- step, sourceCurrentRestIndex, destinationCurrentRestIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			var sourceThresholdReached = (sourceCurrentRestIndex -= step) == 0;
			var destinationThresholdReached = (destinationCurrentRestIndex -= step) == 0;
			// Если вставляемый блок вне допустимого для ветки диапазона, расширяем ветку
			if (destinationCurrentRestIndex > context.Destination.high[destinationCurrentIntIndex].Length)
				stack.Insert(stackIndex, (context.Source, sourceIndexes.End.Index + 1 + context.Destination.high[destinationCurrentIntIndex].Length
					- destinationCurrentRestIndex, context.Destination.high[destinationCurrentIntIndex],
					context.Destination.high[destinationCurrentIntIndex].Length,
					destinationCurrentRestIndex - context.Destination.high[destinationCurrentIntIndex].Length, true));
			stack.Insert(stackIndex, (context.Source.high[sourceCurrentIntIndex], sourceCurrentRestIndex,
				context.Destination.high[destinationCurrentIntIndex], destinationCurrentRestIndex, step, true));
			leftLength -= step;
			if (sourceThresholdReached)
			{
				sourceCurrentIntIndex--;
				sourceCurrentRestIndex = sourceCurrentIntIndex >= sourceIndexes.Start.IntIndex ? context.Source.highLength[sourceCurrentIntIndex] : fragment;
			}
			if (destinationThresholdReached)
			{
				destinationCurrentIntIndex--;
				context.Destination.highLength[destinationCurrentIntIndex] = destinationCurrentRestIndex = destinationCurrentIntIndex
					< destinationHighLengthOld - 1 ? context.Destination.highLength[destinationCurrentIntIndex] : fragment;
			}
		}
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeTrivial(TCertain source, MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length, bool ignoreReversed)
	{
		Debug.Assert(source.low != null && destination.low != null);
#if VERIFY
		MpzT oldLength = new(destination.Length);
#endif
		var bReversedOld = source.bReversed && !ignoreReversed; // Сохраняем значения переменных, потому что после первого
		var destinationReversedOld = destination.bReversed && !ignoreReversed; // реверса они изменяются
		if (bReversedOld)
		{
			source.Reverse();
			sourceIndex = source.Length - length - sourceIndex;
		}
		// Если копируемый диапазон не помещается в целевой список, расширяем его
		if (destination.low.Length < destinationIndex + length)
			if (destinationReversedOld)
			{
				// Если список был перевернут, расширяем слева
				using var toInsert = RedStarLinq.EmptyList<T>((int)(destinationIndex + length - destination.Length));
				destination.low.Insert(0, toInsert);
			}
			else // Иначе справа
				destination.low.Resize((int)(destinationIndex + length));
		MpzT destinationIndexOld = new(destinationIndex); // Сохраняем также и целевой индекс
		if (destinationReversedOld)
		{
			destination.Reverse();
			destinationIndex = destination.Length - length - destinationIndex;
		}
		// Если оба списка направлены одинаково, копируем напрямую
		if (source.IsReversed == destination.IsReversed ^ bReversedOld ^ destinationReversedOld)
			source.low.CopyRangeTo((int)sourceIndex, destination.low, (int)destinationIndex, (int)length);
		else // Иначе используем срезы для предотвращения лишних проходов
			source.CollectionLowCreator(source.low.Skip((int)sourceIndex).Take((int)length).Reverse())
				.CopyRangeTo(0, destination.low, (int)destinationIndex, (int)length);
		// Если список был расширен, фиксируем это
		destination.Length = RedStarLinqMath.Max(destination.Length, destinationIndexOld + length);
		// И, наконец, возвращаем направление следования списков к тому, что было до этого вызова
		if (bReversedOld)
			source.Reverse();
		if (destinationReversedOld)
			destination.Reverse();
#if VERIFY
		Debug.Assert(destination.Length == RedStarLinqMath.Max(oldLength, destinationIndexOld + length));
		source.Verify();
		destination.Verify();
#endif
	}

	protected override void CopyToInternal(MpzT sourceIndex, TCertain destination,
		MpzT destinationIndex, MpzT length, bool ignoreReversed = false) =>
		CopyRange((TCertain)this, sourceIndex, destination, destinationIndex, length, ignoreReversed);

	protected override void CopyToInternal(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length)
	{
		if (length == 0)
			return;
		if (low != null)
		{
			int index2 = (int)index, count2 = (int)length;
			for (var i = 0; i < count2; i++)
				list[listIndex + i] = low[index2 + i];
		}
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(IsReversed ? Length - 1 - index : index, out var bitsIndex);
			var endIntIndex = highLength.IndexOfNotGreaterSum(index + length - 1, out var endBitsIndex);
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
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	protected override void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex + i] = GetInternal(index + i);
	}

	public override void Dispose()
	{
		low?.Dispose();
		high?.ForEach(x => x.Dispose());
		high?.Dispose();
		highLength?.Dispose();
		parent = null;
		_capacity = 0;
		fragment = 1;
		bReversed = false;
		GC.SuppressFinalize(this);
	}

	protected override T GetInternal(MpzT index, bool invoke = true)
	{
		if (low != null)
			return low[(int)(bReversed ? Length - 1 - index : index)];
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var bitsIndex);
			return high[intIndex].GetInternal(bitsIndex, invoke);
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	protected virtual void IncreaseCapacityExponential(MpzT value, MpzT targetFragment)
	{
		var this2 = (TCertain)this;
		Debug.Assert(high != null && highLength != null);
		do
		{
			var newFragment = fragment << SubbranchesBitLength;
			if (_capacity < newFragment)
				IncreaseCapacityLinear(newFragment);
			var highCount = (int)RedStarLinqMath.Min(GetArrayLength(value, newFragment), Subbranches);
			var oldHigh = high;
			var oldHighLength = highLength;
			var oldLength = Length;
			high = new(highCount) { CapacityCreator(0) };
			var first = high[0];
			first.fragment = fragment;
			fragment = newFragment;
			first.low = null;
			first.high = oldHigh;
			first.highLength = oldHighLength;
			first.Length = oldHighLength.ValuesSum;
			first._capacity = _capacity;
			Debug.Assert(first.high != null && first.highLength != null);
			first.parent = this2;
			oldHigh.ForEach(x => x.parent = first);
			for (var i = 1; i < high.Capacity - 1; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			var leftCapacity = value % fragment;
			if (leftCapacity == 0)
				leftCapacity = fragment;
			high.Add(CapacityCreator(leftCapacity));
			high[^1].parent = this2;
			AddCapacity(leftCapacity);
			highLength = [Length];
		} while (fragment < targetFragment);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void IncreaseCapacityLinear(MpzT value)
	{
		var this2 = (TCertain)this;
		Debug.Assert(high != null && highLength != null);
		if (GetArrayLength(value, fragment) == GetArrayLength(_capacity, fragment))
		{
			high[^1].Capacity = value % fragment == 0 ? fragment : value % fragment;
			return;
		}
		if (_capacity < fragment * high.Length)
		{
			high[^1].Capacity = fragment;
			if (_capacity == value)
				return;
		}
		var highCount = (int)GetArrayLength(value - _capacity, fragment);
		high.Capacity = Max(high.Capacity, high.Length + highCount);
		for (var i = 0; i < highCount - 1; i++)
		{
			high.Add(CapacityCreator(fragment));
			high[^1].parent = this2;
			AddCapacity(fragment);
		}
		var leftCapacity = (value - _capacity) % fragment;
		if (leftCapacity == 0)
			leftCapacity = fragment;
		high.Add(CapacityCreator(leftCapacity));
		high[^1].parent = this2;
		AddCapacity(leftCapacity);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override MpzT IndexOfInternal(T item, MpzT index, MpzT length, bool fromEnd)
	{
		if (length == 0)
			return -1;
		if (low != null)
			return IndexOfTrivial(item, index, length, fromEnd);
		else if (high == null || highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		var endIndex = index + length - 1;
		var bReversedOld = bReversed;
		if (bReversedOld)
		{
			Reverse();
			(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
			fromEnd = !fromEnd;
		}
		try
		{
			var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
			var endIntIndex = highLength.IndexOfNotGreaterSum(endIndex, out var endBitsIndex);
			while (endBitsIndex >= fragment)
			{
				endIntIndex++;
				endBitsIndex -= fragment;
			}
			if (intIndex != 0 && intIndex == highLength.Length && bitsIndex == 0 && highLength[intIndex - 1] != fragment)
			{
				intIndex--;
				bitsIndex = highLength[^1];
			}
			MpzT foundIndex;
			if (intIndex == endIntIndex)
			{
				foundIndex = high[intIndex].IndexOfInternal(item, bitsIndex, length, fromEnd);
				if (foundIndex >= 0)
				{
					foundIndex += index - bitsIndex;
					return bReversedOld ? Length - 1 - foundIndex : foundIndex;
				}
				return foundIndex;
			}
			MpzT offset;
			if (fromEnd)
			{
				offset = endIndex - endBitsIndex;
				foundIndex = high[endIntIndex].IndexOfInternal(item, 0, endBitsIndex + 1, true);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
				offset -= high[endIntIndex - 1].Length;
			}
			else
			{
				offset = index - bitsIndex;
				foundIndex = high[intIndex].IndexOfInternal(item, bitsIndex, high[intIndex].Length - bitsIndex, false);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
				offset += high[intIndex].Length;
			}
			for (var i = fromEnd ? endIntIndex - 1 : intIndex + 1;
				fromEnd ? i > intIndex : i < endIntIndex; i += fromEnd ? -1 : 1)
			{
				foundIndex = high[i].IndexOfInternal(item, 0, high[i].Length, fromEnd);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
				offset += fromEnd ? -high[i - 1].Length : high[i].Length;
			}
			if (fromEnd)
			{
				foundIndex = high[intIndex].IndexOfInternal(item, bitsIndex, high[intIndex].Length - bitsIndex, true);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
			}
			else
			{
				foundIndex = high[endIntIndex].IndexOfInternal(item, 0, endBitsIndex + 1, false);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
			}
			return -1;
		}
		finally
		{
			if (bReversedOld)
				Reverse();
		}
	}

	protected virtual MpzT IndexOfTrivial(T item, MpzT index, MpzT length, bool fromEnd)
	{
		Debug.Assert(low != null);
		Debug.Assert(index < int.MaxValue - 1);
		Func<T, int, int, int> func = bReversed ^ fromEnd ? low.LastIndexOf : low.IndexOf;
		if (fromEnd)
		{
			index += length - 1;
		}
		if (bReversed)
		{
			var foundIndex = func(item, (int)(Length - 1 - index), (int)length);
			return foundIndex >= 0 ? Length - 1 - foundIndex : foundIndex;
		}
		else
			return func(item, (int)index, (int)length);
	}

	protected override void InsertInternal(MpzT index, T item)
	{
		var this2 = (TCertain)this;
		EnsureCapacity(Length + 1);
		var bReversedOld = bReversed;
	start:
		if (low != null)
		{
			Debug.Assert(index < int.MaxValue - 1);
			low.Insert((int)(bReversed ? Length - index : index), item);
			Length += 1;
#if VERIFY
			Verify();
#endif
			return;
		}
		if (high == null || highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		if (bReversed)
		{
			Reverse();
			index = Length - index;
		}
		var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
		if (intIndex != 0 && (intIndex == highLength.Length || bitsIndex == highLength[intIndex])
			&& high[intIndex - 1].Capacity != high[intIndex - 1].Length)
			bitsIndex = high[--intIndex].Length;
		if (high.Length == intIndex)
		{
			if (high.Length < Subbranches && high[^1].Capacity == fragment)
			{
				high.Capacity = Min(high.Capacity << 1, Subbranches);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			else
			{
				Compactify();
				goto start;
			}
		}
		if (Length > fragment * (Subbranches - 1))
		{
			Debug.Assert(parent == null);
			Capacity <<= 1;
			goto start;
		}
		if (intIndex < highLength.Length && (high[intIndex].low != null && high[intIndex].Length == LeafSize
			|| high[intIndex].high != null && high[intIndex].highLength != null
			&& high[intIndex].Length > high[intIndex].fragment * (high[intIndex].Subbranches - 1)))
		{
			if (highLength.Length == Subbranches)
			{
				Compactify();
				goto start;
			}
			if (intIndex == high.Length - 1)
				high[intIndex].Capacity = fragment;
			var temp = high[^1];
			if (temp.Length != 0)
			{
				high.Capacity = Max(high.Capacity, Subbranches);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
				temp = high[^1];
			}
			temp.Capacity = fragment;
			high.CopyRangeTo(intIndex + 1, high, intIndex + 2, high.Length - intIndex - 2);
			high[intIndex + 1] = temp;
			high[intIndex].CopyToInternal(bitsIndex, temp, 0, high[intIndex].Length - bitsIndex);
			temp.parent = this2;
			highLength.Insert(intIndex + 1, high[intIndex].Length - bitsIndex);
			high[intIndex].SetOrAddInternal(bitsIndex, item);
			high[intIndex].RemoveEnd(bitsIndex + 1);
			highLength[intIndex] = bitsIndex + 1;
		}
		else
		{
			high[intIndex].InsertInternal(bitsIndex, item);
			if (highLength.Length == intIndex)
				highLength.Add(1);
			else
				highLength[intIndex]++;
		}
		if (bReversedOld)
			Reverse();
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override void InsertInternal(MpzT index, TCertain bigList)
	{
		var this2 = (TCertain)this;
		var length = bigList.Length;
		EnsureCapacity(Length + length);
		if (length == 0)
			return;
		if (low != null)
		{
			Debug.Assert(index < int.MaxValue - 1);
			low.Insert((int)(bReversed ? Length - index : index), bigList.Wrap(x => bReversed ? x.Reverse<T>() : x));
			Length += length;
#if VERIFY
			Verify();
#endif
			return;
		}
		else if (high == null || highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		var endIndex = index + length - 1;
		var bReversedOld = bReversed;
		if (bReversedOld)
		{
			Reverse();
			bigList.Reverse();
			(index, endIndex) = (Length + length - 1 - endIndex, Length + length - 1 - index);
		}
	start:
		var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
		var endIntIndex = highLength.IndexOfNotGreaterSum(endIndex, out var endBitsIndex);
		while (endBitsIndex >= fragment)
		{
			endIntIndex++;
			endBitsIndex -= fragment;
		}
		if (intIndex != 0 && intIndex == highLength.Length && bitsIndex == 0 && highLength[intIndex - 1] != fragment)
		{
			intIndex--;
			bitsIndex = highLength[^1];
		}
		if (high.Length == intIndex)
		{
			if (high.Length < Subbranches && high[^1].Capacity == fragment)
			{
				high.Capacity = Min(high.Capacity << 1, Subbranches);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			else
			{
				Compactify();
				goto start;
			}
		}
		if (intIndex == endIntIndex && high[intIndex].Length + length <= fragment)
		{
			high[intIndex].InsertInternal(bitsIndex, bigList);
			if (highLength.Length == intIndex)
				highLength.Add(length);
			else
				highLength[intIndex] += length;
		}
		else if (index == Length)
			bigList.CopyToInternal(0, this2, Length, length, true);
		else if (index + length <= Length)
		{
			CopyToInternal(index, this2, index + length, Length - index, true);
			bigList.CopyToInternal(0, this2, index, length, true);
		}
		else
		{
			var temp = CapacityCreator(Length - index);
			CopyToInternal(index, temp, 0, Length - index, true);
			bigList.CopyToInternal(0, this2, index, length, true);
			temp.CopyToInternal(0, this2, index + length, temp.Length, true);
		}
		if (bReversedOld)
		{
			Reverse();
			bigList.Reverse();
			(index, endIndex) = (Length + length - 1 - endIndex, Length + length - 1 - index);
		}
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void ReduceCapacityExponential(MpzT newFragment)
	{
		var this2 = (TCertain)this;
		do
		{
			Debug.Assert(high != null && highLength != null);
			if (high.Length > 1 && high[1].Length != 0)
				Compactify();
			fragment >>= SubbranchesBitLength;
			var oldHigh = high;
			var oldHighLength = highLength;
			oldHigh.Skip(1).ForEach(x => x.Dispose());
			highLength.Dispose();
			low = oldHigh[0].low;
			high = oldHigh[0].high;
			highLength = oldHigh[0].highLength;
			AddCapacity(oldHigh[0].Capacity - _capacity);
			oldHigh.Dispose();
			oldHighLength.Dispose();
			if (high == null || highLength == null)
			{
				Debug.Assert(low != null);
				return;
			}
			high.ForEach(x => x.parent = this2);
		} while (fragment > newFragment);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void ReduceCapacityLinear(MpzT value)
	{
		Debug.Assert(high != null && highLength != null);
		var highCount = (int)GetArrayLength(value, fragment);
		if (highCount == high.Length || high[highCount].Length != 0)
			Compactify();
		for (var i = high.Length - 1; i >= highCount; i--)
		{
			AddCapacity(-high[i].Capacity);
			high[i].Dispose();
		}
		high.Skip(highCount).ForEach(x => x?.Dispose());
		high.RemoveEnd(highCount);
		var leftCapacity = value % fragment;
		if (leftCapacity == 0)
			leftCapacity = fragment;
		if (high[^1].Length > leftCapacity)
			Compactify();
		high[^1].Capacity = leftCapacity == 0 ? fragment : leftCapacity;
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public override TCertain Remove(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveInternal(index, length);
		return (TCertain)this;
	}

	protected override void RemoveAtInternal(MpzT index)
	{
		if (low != null)
		{
			low.RemoveAt((int)(bReversed ? Length - 1 - index : index));
			Length -= 1;
		}
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var bitsIndex);
			if (intIndex != highLength.Length - 1 && highLength[intIndex] == 1)
			{
				var temp = high[intIndex];
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(intIndex + 1, high, intIndex, high.Length - intIndex - offsetFromEnd - 1);
				high[^(offsetFromEnd + 1)] = temp;
				temp.RemoveEndInternal(RedStarLinqMath.Min(high[^1].Length, temp.Length));
				high[^1].CopyToInternal(0, temp, 0, high[^1].Length);
				high[^1].ClearInternal(false);
				highLength.RemoveAt(intIndex);
			}
			else
			{
				highLength.Decrease(intIndex);
				high[intIndex].RemoveAtInternal(bitsIndex);
			}
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override void RemoveEndInternal(MpzT index)
	{
		if (index == Length)
			return;
		if (bReversed)
		{
			RemoveInternal(index, Length - index);
			return;
		}
		if (low != null)
		{
			low.RemoveEnd((int)index);
			Length = index;
		}
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var bitsIndex);
			for (var i = high.Length - 1; i > intIndex; i--)
				high[i].Clear();
			high[intIndex].RemoveEndInternal(bitsIndex);
			if (bitsIndex == 0)
				highLength.RemoveEnd(intIndex);
			else
			{
				highLength.RemoveEnd(intIndex + 1);
				highLength[intIndex] = bitsIndex;
			}
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override void RemoveInternal(MpzT index, MpzT length)
	{
		if (length == 0)
			return;
		if (low != null)
		{
			low.Remove((int)(bReversed ? Length - length - index : index), (int)length);
			Length -= length;
#if VERIFY
			Verify();
#endif
			return;
		}
		else if (high == null || highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		var endIndex = index + length - 1;
		if (bReversed)
			(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
		var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
		var endIntIndex = highLength.IndexOfNotGreaterSum(endIndex, out var endBitsIndex);
		if (intIndex == endIntIndex)
		{
			if (intIndex != highLength.Length - 1 && bitsIndex == 0 && endBitsIndex == highLength[intIndex] - 1)
			{
				var temp = high[intIndex];
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(intIndex + 1, high, intIndex, high.Length - intIndex - offsetFromEnd - 1);
				high[^(offsetFromEnd + 1)] = temp;
				temp.RemoveEndInternal(RedStarLinqMath.Min(high[^1].Length, temp.Length));
				high[^1].CopyToInternal(0, temp, 0, high[^1].Length);
				high[^1].ClearInternal(false);
				if (offsetFromEnd != 0)
					highLength[intIndex] = highLength[intIndex + 1];
				highLength.RemoveAt(intIndex + offsetFromEnd);
			}
			else
			{
				high[intIndex].RemoveInternal(bitsIndex, length);
				highLength[intIndex] -= length;
			}
		}
		else
		{
			var startOffset = bitsIndex == 0 ? 0 : 1;
			var endOffset = endBitsIndex + 1 == highLength[endIntIndex] ? 1 : 0;
			if (startOffset == 1)
			{
				high[intIndex].RemoveEndInternal(bitsIndex);
				highLength[intIndex] = bitsIndex;
			}
			var tempRange = high.GetRange(intIndex + startOffset, endIntIndex + endOffset - (intIndex + startOffset));
			var tempOffset = Capacity == high.Length * fragment ? 0 : 1;
			var copyLength = high.Length - (endIntIndex + endOffset) - tempOffset;
			if (copyLength > 0)
			{
				high.CopyRangeTo(endIntIndex + endOffset, high, intIndex + startOffset, copyLength);
				tempRange.CopyRangeTo(0, high, high.Length + intIndex + startOffset - (endIntIndex + endOffset) - tempOffset, tempRange.Length);
			}
			if (tempRange.Length != 0)
			{
				tempRange[0].RemoveEndInternal(RedStarLinqMath.Min(high[^1].Length, tempRange[0].Length));
				high[^1].CopyToInternal(0, tempRange[0], 0, high[^1].Length);
				high[^1].ClearInternal(false);
			}
			highLength.Remove((intIndex + startOffset)..(endIntIndex + endOffset));
			for (var i = high.Length - tempRange.Length; i < high.Length; i++)
				high[i].ClearInternal(false);
			Length = highLength.ValuesSum;
			if (endOffset == 0)
			{
				high[intIndex + startOffset].RemoveInternal(0, endBitsIndex + 1);
				if (highLength.Length > intIndex + startOffset)
					highLength[intIndex + startOffset] -= endBitsIndex + 1;
			}
		}
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	/// <summary>
	/// Этот метод выполняется моментально, всего лишь меняя состояние внутреннего флага "список перевернут"
	/// на противоположное. После этого функции Add(), RemoveEnd() и CopyTo() радикально меняют
	/// свое поведение, многие другие меняют слегка.
	/// </summary>
	/// <returns>Ссылка на себя.</returns>
	public override TCertain Reverse()
	{
		bReversed = !bReversed;
		return (TCertain)this;
	}

	protected override void ReverseInternal(MpzT index, MpzT length)
	{
		if (length == 0)
			return;
		if (low != null)
		{
			low.Reverse((int)(bReversed ? Length - length - index : index), (int)length);
#if VERIFY
			Verify();
#endif
			return;
		}
		Debug.Assert(high != null && highLength != null);
		var endIndex = index + length - 1;
		var bReversedOld = bReversed;
		if (bReversedOld)
		{
			Reverse();
			(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
		}
		try
		{
			var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
			var endIntIndex = highLength.IndexOfNotGreaterSum(endIndex, out var endBitsIndex);
			if (intIndex == endIntIndex)
			{
				high[intIndex].ReverseInternal(bitsIndex, length);
				return;
			}
			if (bitsIndex == 0 && endBitsIndex == highLength[endIntIndex] - 1)
			{
				for (var i = intIndex; i < endIntIndex + 1; i++)
					high[i].Reverse();
				high.Reverse(intIndex, endIntIndex + 1 - intIndex);
				highLength.Reverse(intIndex, endIntIndex + 1 - intIndex);
				return;
			}
			for (var i = intIndex + 1; i < endIntIndex - 1; i++)
				high[i].Reverse();
			high.Reverse(intIndex + 1, endIntIndex - 1 - intIndex);
			highLength.Reverse(intIndex + 1, endIntIndex - 1 - intIndex);
			TCertain tempRange;
			if (endBitsIndex + 1 == highLength[intIndex] - bitsIndex)
			{
				tempRange = high[intIndex].GetRange(bitsIndex);
				high[intIndex].CopyToInternal(bitsIndex, high[endIntIndex], 0, endBitsIndex + 1, true);
				high[endIntIndex].Reverse(0, endBitsIndex + 1);
				tempRange.Reverse();
				tempRange.CopyToInternal(0, high[intIndex], bitsIndex, tempRange.Length, true);
				return;
			}
			var greaterAtEnd = endBitsIndex + 1 > highLength[intIndex] - bitsIndex;
			var greaterIndex = greaterAtEnd ? endIntIndex : intIndex;
			var lessIndex = greaterAtEnd ? intIndex : endIntIndex;
			var greater = high[greaterIndex];
			var less = high[lessIndex];
			var commonLength = greaterAtEnd ? highLength[intIndex] - bitsIndex : endBitsIndex + 1;
			var endCommonIndex = greaterAtEnd ? endBitsIndex + 1 - commonLength : 0;
			var difference = (endBitsIndex + 1 - highLength[intIndex] + bitsIndex).Abs();
			if (commonLength == 0)
			{
				if (greaterAtEnd)
				{
					high.CopyRangeTo(intIndex + 1, high, intIndex + 2, endIntIndex - intIndex - 2);
					less.Reverse();
					high[intIndex + 1] = less;
					var movingLength = highLength[endIntIndex];
					highLength.CopyRangeTo(intIndex + 1, highLength, intIndex + 2, endIntIndex - intIndex - 2);
					highLength[intIndex + 1] = movingLength;
				}
				else
				{
					high.CopyRangeTo(intIndex + 1, high, intIndex, endIntIndex - intIndex - 2);
					less.Reverse();
					high[endIntIndex - 1] = less;
					var movingLength = highLength[intIndex];
					highLength.CopyRangeTo(intIndex + 1, highLength, intIndex, endIntIndex - intIndex - 2);
					highLength[endIntIndex - 1] = movingLength;
				}
			}
			tempRange = high[intIndex].GetRange(bitsIndex, commonLength);
			high[intIndex].CopyToInternal(bitsIndex, high[endIntIndex], endCommonIndex, commonLength, true);
			high[endIntIndex].Reverse(endCommonIndex, commonLength);
			tempRange.Reverse();
			tempRange.CopyToInternal(0, high[intIndex], bitsIndex, commonLength, true);
			var takeIndex = greaterAtEnd ? 0 : bitsIndex + commonLength;
			var insertIndex = greaterAtEnd ? less.Length : 0;
			if (less.Length + difference <= fragment)
			{
				if (less.Length + difference > less.Capacity)
					less.Capacity = fragment;
				less.InsertInternal(insertIndex, greater.GetRange(takeIndex, difference));
				greater.Remove(takeIndex, difference);
				less.Reverse(insertIndex, difference);
				highLength[greaterIndex] -= difference;
				highLength[lessIndex] += difference;
				return;
			}
			var compactified = false;
		tryAddBranch:
			if (highLength.Length < high.Length)
			{
				var offsetFromEnd = 0;
				if (high[^1].Capacity == fragment) { }
				else if (highLength.Length < high.Length - 1)
					offsetFromEnd = 1;
				else
					high[^1].Capacity = fragment;
				less = high[^(offsetFromEnd + 1)];
				var shiftIndex = greaterAtEnd ? endIntIndex : intIndex + 1;
				high.CopyRangeTo(shiftIndex, high, shiftIndex + 1, high.Length - shiftIndex - offsetFromEnd - 1);
				less.AddRange(greater.GetRange(takeIndex, difference));
				less.Reverse();
				greater.Remove(takeIndex, difference);
				high[shiftIndex] = less;
				highLength.CopyRangeTo(shiftIndex, highLength, shiftIndex + 1,
					RedStarLinqMath.Min(high.Length - shiftIndex - offsetFromEnd - 1, highLength.Length - shiftIndex));
				highLength[shiftIndex] = difference;
			}
			else if (high.Length < Subbranches)
			{
				IncreaseCapacityLinear(Min(high.Length << 1, Subbranches) * fragment);
				goto tryAddBranch;
			}
			else if (!compactified)
			{
				Compactify();
				compactified = true;
				goto tryAddBranch;
			}
			else
			{
				IncreaseCapacityExponential(Capacity << 1, fragment << SubbranchesBitLength);
				Debug.Assert(high != null && high.Length == 2 && highLength != null && highLength.Length == 1
					&& high[0].low == null && high[0].high != null && high[0].highLength != null
					&& high[0].high!.Length > 1 && high[0].highLength!.Length > 1);
				less = high[0].high![^1];
				high[1].AddRange(less);
				highLength[1].Add(less.Length);
				less.Clear();
				var shiftIndex = greaterAtEnd ? endIntIndex : intIndex + 1;
				high[0].high!.CopyRangeTo(shiftIndex, high[0].high!, shiftIndex + 1, high[0].high!.Length - shiftIndex - 1);
				less.AddRange(greater.GetRange(takeIndex, difference));
				less.Reverse();
				greater.Remove(takeIndex, difference);
				high[0].high![shiftIndex] = less;
				high[0].highLength!.CopyRangeTo(shiftIndex, high[0].highLength!, shiftIndex + 1,
					high[0].highLength!.Length - shiftIndex - 1);
				high[0].highLength![shiftIndex] = difference;
			}
		}
		finally
		{
#if VERIFY
			if (high != null && highLength != null)
			{
				Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
				Debug.Assert(highLength.All((x, index) => x == high[index].Length));
			}
			Verify();
#endif
			if (bReversedOld)
				Reverse();
		}
	}

	protected override void SetInternal(MpzT index, T value)
	{
		if (low != null)
			low[(int)(bReversed ? Length - 1 - index : index)] = value;
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var bitsIndex);
			if (intIndex != 0 && (intIndex == highLength.Length || bitsIndex == highLength[intIndex])
				&& high[intIndex - 1].Capacity != high[intIndex - 1].Length)
				bitsIndex = high[--intIndex].Length;
			high[intIndex].SetInternal(bitsIndex, value);
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public virtual TCertain SetOrAdd(MpzT index, T value)
	{
		if ((uint)index > (uint)Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		return SetOrAddInternal(index, value);
	}

	protected virtual TCertain SetOrAddInternal(MpzT index, T value)
	{
		if (index == Length)
			return Add(value);
		SetInternal(index, value);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
		return (TCertain)this;
	}
#if VERIFY

	protected override void Verify() => Verify(Root);

	protected virtual void Verify(TCertain item)
	{
		item.VerifySingle();
		if (item.high == null)
			return;
		for (var i = 0; i < item.high.Length; i++)
		{
			var x = item.high[i];
			Verify(x);
		}
	}

	protected override void VerifySingle()
	{
		Debug.Assert(low != null ^ (high != null || highLength != null));
		Debug.Assert(Length <= Capacity);
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
			Debug.Assert(high.Length == 0 || high[^1].Capacity <= fragment);
			Debug.Assert((high.Length - 1) * fragment + high[^1].Capacity == Capacity);
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}
#endif

	protected record struct CopyRangeContext(TCertain Source, MpzT SourceIndex,
		TCertain Destination, MpzT DestinationIndex, MpzT Length, bool IgnoreReversed)
	{
		public static implicit operator CopyRangeContext((TCertain Source, MpzT SourceIndex,
			TCertain Destination, MpzT DestinationIndex, MpzT Length, bool IgnoreReversed) x) =>
			new(x.Source, x.SourceIndex, x.Destination, x.DestinationIndex, x.Length, x.IgnoreReversed);
	}

	protected readonly struct CopyRangeIndex
	{
		public MpzT Index { get; }
		public MpzT RestIndex { get; }
		public int IntIndex { get; }

		public CopyRangeIndex(BigSumList highLength, MpzT index)
		{
			Index = index;
			IntIndex = highLength.IndexOfNotGreaterSum(index, out var restIndex);
			RestIndex = restIndex;
		}

		public CopyRangeIndex(MpzT index, int intIndex, MpzT restIndex)
		{
			Index = index;
			IntIndex = intIndex;
			RestIndex = restIndex;
		}

		public static implicit operator CopyRangeIndex((MpzT Index, int IntIndex, MpzT RestIndex) x) =>
			new(x.Index, x.IntIndex, x.RestIndex);
	}

	protected record struct CopyRangeSide(CopyRangeIndex Start, CopyRangeIndex End)
	{
		public static implicit operator CopyRangeSide((CopyRangeIndex Start, CopyRangeIndex End) x) => new(x.Start, x.End);
	}
}
