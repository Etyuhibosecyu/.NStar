global using Mpir.NET;
global using NStar.Core;
global using NStar.Linq;
global using NStar.MathLib;
global using NStar.SumCollections;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static NStar.Core.Extents;
global using static System.Math;
global using E = System.Linq.Enumerable;
using NStar.ExtraHS;

namespace NStar.BigCollections;

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
	private protected Stack<(TCertain Branch, MpzT Start)>? accessCache;
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

	public BigList(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(subbranchesBitLength, leafSizeBitLength)
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
			subbranchesBitLength, leafSizeBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(MpzT capacity, G.IEnumerable<T> collection, int subbranchesBitLength = -1,
		int leafSizeBitLength = -1) : this(capacity, subbranchesBitLength, leafSizeBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(collection.Length, subbranchesBitLength, leafSizeBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public BigList(MpzT capacity, ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(RedStarLinqMath.Max(capacity, collection.Length), subbranchesBitLength, leafSizeBitLength)
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
					Compactify(Length);
				var first = this;
				var reverse = false;
				// Ищем первый лист и разрушаем "лишние" ветки
				for (; first.high != null; first = first.high[0], first.parent?.high?.Dispose(), reverse ^= first.bReversed)
				{
					Debug.Assert(first.highLength != null);
					first.high.Skip(1).ForEach(x => x.Dispose());
					first.highLength.Dispose();
				}
				Debug.Assert(first.low != null);
				low = first.low;
				if (reverse)
					Reverse();
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
				IncreaseCapacity(value, fragment);
				return;
			}
			else if (high != null && highLength != null) // Как старая, так и новая емкость определяет много листьев
			{
				// См. подробный разбор аналогичной строки в предыдущем разделе
				var newFragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - LeafSizeBitLength,
					SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
				while (newFragment << SubbranchesBitLength < value)
					newFragment <<= SubbranchesBitLength;
				if (value > _capacity)
				{
					IncreaseCapacity(value, newFragment);
					return;
				}
				else if (newFragment < fragment) // Старое дерево существенно больше
				{
					ReduceCapacityExponential(newFragment);
					AddCapacity((fragment << SubbranchesBitLength) - _capacity);
					ReduceCapacityLinear(value);
				}
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
					Compactify(Length);
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

	protected override void ClearInternal(bool verify)
	{
		Root.accessCache?.Clear();
		if (low != null)
		{
			low.Clear(false);
			Length = 0;
		}
		else
		{
			high?.ForEach(x => x?.ClearInternal(false));
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
			if (bReversed)
				index = Length - length - index;
			var endIndex = index + length - 1;
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

	protected virtual void Compactify(MpzT limit)
	{
		Root.accessCache?.Clear();
		Debug.Assert(high != null && highLength != null);
		var i = 1;
		for (; i < highLength.Length; i++)
		{
			var highLengthBeforeI = highLength[i - 1];
			if (highLengthBeforeI == fragment)
				continue;
			var amount = RedStarLinqMath.Min(fragment - highLengthBeforeI, limit);
			if (amount <= 0)
				break;
			var highAtI = high[i];
			var highLengthAtI = highLength[i];
			var highBeforeI = high[i - 1];
			if (highBeforeI.high != null && highBeforeI.highLength != null
				&& highBeforeI.highLength.ValuesSum + highBeforeI.fragment
				- highBeforeI.highLength[^1] + amount > highBeforeI.Capacity)
				highBeforeI.Compactify(limit);
			if (highLengthAtI > amount)
			{
				if (highBeforeI.Length < highLengthBeforeI + amount)
					CopyRange((null!, -1, highBeforeI, -1, highBeforeI.Length - highLengthBeforeI - amount));
				CopyRange(ProcessReverseContext((highAtI, 0, highBeforeI, highLengthBeforeI, amount)));
				CopyRange(ProcessReverseContext((highAtI, amount, highAtI, 0, highLengthAtI - amount)));
				highLength[i - 1] += amount;
				highLength[i] -= amount;
				limit -= amount;
			}
			else
			{
				if (highBeforeI.Length < highLengthBeforeI + highLengthAtI)
					CopyRange((null!, -1, highBeforeI, -1, highBeforeI.Length - highLengthBeforeI - highLengthAtI));
				CopyRange(ProcessReverseContext((highAtI, 0, highBeforeI, highLengthBeforeI, highLengthAtI)));
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				Index pasteIndex = i;
				if (i != highLength.Length - 1)
				{
					high.CopyRangeTo(i + 1, high, i, high.Length - i - offsetFromEnd - 1);
					high[pasteIndex = ^(offsetFromEnd + 1)] = highAtI;
				}
				var lastHigh = high[^1];
				lastHigh.Capacity = fragment;
				(high[pasteIndex], high[^1]) = (lastHigh, highAtI);
				highAtI.ClearInternal(false);
				highLength[i - 1] += highLengthAtI;
				limit -= highLengthAtI;
				highLength.RemoveAt(i);
				i--;
			}
		}
		if (i <= highLength.Length)
		{
			high[i - 1].RemoveEndInternal(highLength[i - 1]);
			if (i >= 2)
				high[i - 2].RemoveEndInternal(highLength[i - 2]);
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
			high = new(highCount, true);
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
		Debug.Assert((low == null || low.Capacity == 0) && high == null && highLength == null
			&& fragment == 1 && _capacity == 0);
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
			high = new(GetArrayLength(list.Count, fragment2), true);
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
		if (context.Source == null && context.SourceIndex < 0 && context.DestinationIndex < 0)
			return;
		if (context.Source == null)
			throw new ArgumentNullException(nameof(context.Source), "Исходный массив не может быть нулевым.");
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

	protected static void CopyRange(CopyRangeContext context)
	{
		List<CopyRangeContext> stack
			= [context];
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
		if (context.Source == null)
		{
			if (context.SourceIndex == -1 && context.DestinationIndex == -1 && context.Length < 0)
				CopyRangeIterationResize(stack, context.Destination, -context.Length, stackIndex);
			else
				throw new ArgumentNullException(nameof(context.Source), "Исходный массив не может быть нулевым.");
			return;
		}
		var fragment = context.Source.fragment;
		// Тривиальный случай - оба списка являются листьями дерева
		if (context.Source.low != null && context.Destination.low != null)
			CopyRangeTrivial(context);
		// Только целевой список является листом дерева
		else if (context.Destination.low != null && context.Source.high != null && context.Source.highLength != null)
			CopyRangeToLeaf(stack, context, stackIndex, fragment);
		// Исходный список является более мелкой веткой (возможно, даже листом)
		else if ((context.Source.low != null || context.Destination.fragment > fragment)
			&& context.Destination.high != null && context.Destination.highLength != null)
			CopyRangeToLarger(stack, context, stackIndex);
		// Самый сложный случай: исходный список является соизмеримой или более крупной веткой,
		// а целевой также не является листом
		else
			CopyRangeToSimilar(stack, context, stackIndex);
	}

	protected static void CopyRangeIterationResize(List<CopyRangeContext> stack, TCertain source, MpzT increment, int stackIndex)
	{
		if (source.low != null)
		{
			if (!source.bReversed || source.low.Length == 0)
				source.low.Resize((int)(source.Length + increment));
			else
				source.low.ResizeLeft((int)(source.Length + increment));
			source.Length = source.low.Length;
			return;
		}
		else if (source.high == null || source.highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		if (source.bReversed && increment <= source.Length)
		{
			stack.Insert(stackIndex, (source, 0, source, increment, source.Length));
			return;
		}
		if (source.high != null && source.highLength != null && source.highLength.Length != 0
			&& (source.highLength.Length - 1) * source.fragment + source.highLength[^1] + increment
			> source.fragment << source.SubbranchesBitLength)
			source.Compactify(source.Length);
		Debug.Assert(source.high != null && source.highLength != null);
		source.EnsureCapacity((source.highLength.Length != 0 ? (source.highLength.Length - 1)
			* source.fragment + source.highLength[^1] : 0) + increment);
		var leftIncrement = increment;
		if (source.highLength.Length != 0 && source.highLength[^1] != source.fragment)
		{
			var leftToFragment = RedStarLinqMath.Min(source.fragment
				- source.high[source.highLength.Length - 1].Length, leftIncrement);
			leftIncrement -= leftToFragment;
			stack.Insert(stackIndex, (null!, -1, source.high[source.highLength.Length - 1], -1, -leftToFragment));
			source.highLength[^1] += leftToFragment;
		}
		var i = source.highLength.Length;
		for (; leftIncrement >= source.fragment; leftIncrement -= source.fragment, i++)
		{
			if (i >= source.high.Capacity)
				source.high.Capacity = source.Subbranches;
			if (i >= source.high.Length)
			{
				source.high.Add(source.CapacityCreator(source.fragment));
				source.high[^1].parent = source;
				source.AddCapacity(source.fragment);
			}
			source.high[i].EnsureCapacity(source.fragment);
			stack.Insert(stackIndex, (null!, -1, source.high[i], -1, -source.fragment));
			source.highLength.Add(source.fragment);
		}
		if (leftIncrement != 0)
		{
			if (i >= source.high.Capacity)
				source.high.Capacity = source.Subbranches;
			if (i >= source.high.Length)
			{
				source.high.Add(source.CapacityCreator(leftIncrement));
				source.high[^1].parent = source;
				source.AddCapacity(leftIncrement);
			}
			source.high[i].EnsureCapacity(leftIncrement);
			stack.Insert(stackIndex, (null!, -1, source.high[i], -1, -leftIncrement));
			source.highLength.Add(leftIncrement);
		}
		if (source.bReversed)
			stack.Insert(stackIndex, (source, 0, source, increment, source.Length));
#if VERIFY
		source.Verify();
#endif
	}

	protected static void CopyRangeToLarger(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment)
			&& context.Destination.high != null && context.Destination.highLength != null);
		var fragment = context.Destination.fragment;
	destinationFragmentBigger:
		var endIndex = context.DestinationIndex + context.Length - 1;
		CopyRangeIndex start = new(context.Destination.highLength, context.DestinationIndex);
		CopyRangeIndex end = new(context.Destination.highLength, endIndex);
		// Диапазон дописывается после конца списка
		if (start.IntIndex != 0 && start.IntIndex == context.Destination.highLength.Length
			&& start.RestIndex == 0 && context.Destination.highLength[^1] != fragment)
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
			context.Destination.Compactify(context.DestinationIndex + context.Length);
			goto destinationFragmentBigger;
		}
		// Весь диапазон помещается в одну ветку
		if (start.IntIndex == end.IntIndex)
		{
			var newSize = start.RestIndex + context.Length;
			var oldSize = context.Destination.high[start.IntIndex].Length;
			if (newSize < oldSize)
				newSize = oldSize;
			if (newSize != oldSize)
				stack.Insert(stackIndex, (null!, -1, context.Destination.high[start.IntIndex], -1, oldSize - newSize));
			stack.Insert(stackIndex, (context.Source, context.SourceIndex,
				context.Destination.high[start.IntIndex], ProcessReverse(context.Destination.high[start.IntIndex],
				start.RestIndex, context.Length, newSize), context.Length));
			if (context.Destination.highLength.Length > start.IntIndex)
				context.Destination.highLength.UpdateIfGreater(start.IntIndex, newSize);
			else
				context.Destination.highLength.SetOrAdd(start.IntIndex, newSize);
		}
		else if (context.Source.IsReversed != context.Destination.IsReversed)
			CopyRangeToLargerDiagonal(stack, context, stackIndex, fragment, (start, end));
		else
			CopyRangeToLargerDirect(stack, context, stackIndex, fragment, (start, end));
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToLargerDiagonal(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment)
			&& context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var previousPart = RedStarLinqMath.Min((indexes.Start.IntIndex < context.Destination.highLength.Length - 1
			? context.Destination.highLength[indexes.Start.IntIndex] : fragment) - indexes.Start.RestIndex, context.Length);
		var newSize = indexes.Start.RestIndex + previousPart;
		var oldSize = context.Destination.high[indexes.Start.IntIndex].Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, context.Destination.high[indexes.Start.IntIndex], -1, oldSize - newSize));
		stack.Insert(stackIndex, (context.Source, context.SourceIndex + context.Length - previousPart,
			context.Destination.high[indexes.Start.IntIndex],
			ProcessReverse(context.Destination.high[indexes.Start.IntIndex],
			indexes.Start.RestIndex, previousPart, newSize), previousPart));
		if (indexes.Start.IntIndex < context.Destination.highLength.Length - 1)
			context.Destination.highLength.UpdateIfGreater(indexes.Start.IntIndex, indexes.Start.RestIndex + previousPart);
		else
			context.Destination.highLength.SetOrAdd(indexes.Start.IntIndex, indexes.Start.RestIndex + previousPart);
		for (var i = indexes.Start.IntIndex + 1; i < indexes.End.IntIndex; i++)
		{
			var branchLength = i < context.Destination.highLength.Length - 1 ? context.Destination.highLength[i] : fragment;
			previousPart += branchLength;
			stack.Insert(stackIndex, (context.Source, context.SourceIndex + context.Length - previousPart,
				context.Destination.high[i], 0, branchLength));
			context.Destination.highLength.SetOrAdd(i, branchLength);
		}
		stack.Insert(stackIndex, (context.Source, context.SourceIndex,
			context.Destination.high[indexes.End.IntIndex], ProcessReverse(context.Destination.high[indexes.End.IntIndex], 0,
			context.Length - previousPart, context.Length - previousPart), context.Length - previousPart));
		if (context.Destination.highLength.Length > indexes.End.IntIndex)
			context.Destination.highLength.UpdateIfGreater(indexes.End.IntIndex, context.Length - previousPart);
		else
			context.Destination.highLength.SetOrAdd(indexes.End.IntIndex, context.Length - previousPart);
	}

	protected static void CopyRangeToLargerDirect(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert((context.Source.low != null || context.Destination.fragment > context.Source.fragment)
			&& context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон копируется слева направо
		var previousPart = RedStarLinqMath.Min((indexes.Start.IntIndex < context.Destination.highLength.Length - 1
			? context.Destination.highLength[indexes.Start.IntIndex] : fragment) - indexes.Start.RestIndex, context.Length);
		var newSize = indexes.Start.RestIndex + previousPart;
		var oldSize = context.Destination.high[indexes.Start.IntIndex].Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, context.Destination.high[indexes.Start.IntIndex], -1, oldSize - newSize));
		stack.Insert(stackIndex, (context.Source, context.SourceIndex, context.Destination.high[indexes.Start.IntIndex],
			ProcessReverse(context.Destination.high[indexes.Start.IntIndex],
			indexes.Start.RestIndex, previousPart, newSize), previousPart));
		context.Destination.highLength.SetOrAdd(indexes.Start.IntIndex,
			RedStarLinqMath.Max(context.Destination.highLength.Length
			> indexes.Start.IntIndex ? context.Destination.highLength[indexes.Start.IntIndex] : 0,
			indexes.Start.RestIndex + previousPart));
		for (var i = indexes.Start.IntIndex + 1; i < indexes.End.IntIndex; i++)
		{
			var branchLength = i < context.Destination.highLength.Length - 1 ? context.Destination.highLength[i] : fragment;
			stack.Insert(stackIndex, (context.Source, context.SourceIndex + previousPart,
				context.Destination.high[i], 0, branchLength));
				context.Destination.highLength.SetOrAdd(i, branchLength);
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (context.Source, context.SourceIndex + previousPart,
			context.Destination.high[indexes.End.IntIndex], ProcessReverse(context.Destination.high[indexes.End.IntIndex], 0,
			context.Length - previousPart, context.Length - previousPart), context.Length - previousPart));
		if (context.Destination.highLength.Length > indexes.End.IntIndex)
			context.Destination.highLength.UpdateIfGreater(indexes.End.IntIndex, context.Length - previousPart);
		else
			context.Destination.highLength.SetOrAdd(indexes.End.IntIndex, context.Length - previousPart);
	}

	protected static void CopyRangeToLeaf(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		var endIndex = context.SourceIndex + context.Length - 1;
		CopyRangeIndex start = new(context.Source.highLength, context.SourceIndex);
		CopyRangeIndex end = new(context.Source.highLength, endIndex);
		// Диапазон дописывается после конца списка
		if (start.IntIndex != 0 && start.IntIndex == context.Source.highLength.Length
			&& start.RestIndex == 0 && context.Source.highLength[^1] != fragment)
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
			stack.Insert(stackIndex, (context.Source.high[start.IntIndex],
				ProcessReverse(context.Source.high[start.IntIndex], start.RestIndex, context.Length),
				context.Destination, context.DestinationIndex, context.Length));
		else if (context.Source.IsReversed != context.Destination.IsReversed)
			CopyRangeToLeafDiagonal(stack, context, stackIndex, fragment, (start, end));
		else
			CopyRangeToLeafDirect(stack, context, stackIndex, fragment, (start, end));
		context.Destination.highLength?.SetOrAdd(start.IntIndex, context.Destination.highLength.Length
			> start.IntIndex ? context.Destination.highLength[start.IntIndex] : context.DestinationIndex + context.Length);
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToLeafDiagonal(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var (start, end) = indexes;
		var newSize = context.DestinationIndex + context.Length;
		var oldSize = context.Destination.Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, context.Destination, -1, oldSize - newSize));
		var previousPart = end.RestIndex + 1;
		stack.Insert(stackIndex, (context.Source.high[end.IntIndex],
			ProcessReverse(context.Source.high[end.IntIndex], 0, previousPart),
			context.Destination, context.DestinationIndex, previousPart));
		for (var i = end.IntIndex - 1; i > start.IntIndex; i--)
		{
			var branchLength = i < context.Source.highLength.Length - 1 ? context.Source.highLength[i] : fragment;
			stack.Insert(stackIndex, (context.Source.high[i], 0, context.Destination,
				context.DestinationIndex + previousPart, branchLength));
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (context.Source.high[start.IntIndex],
			ProcessReverse(context.Source.high[start.IntIndex], start.RestIndex, context.Length - previousPart),
			context.Destination, context.DestinationIndex + previousPart, context.Length - previousPart));
	}

	protected static void CopyRangeToLeafDirect(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		Debug.Assert(context.Destination.low != null && context.Source.high != null && context.Source.highLength != null);
		// Диапазон копируется слева направо
		var (start, end) = indexes;
		var newSize = context.DestinationIndex + context.Length;
		var oldSize = context.Destination.Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, context.Destination, -1, oldSize - newSize));
		var previousPart = (start.IntIndex < context.Source.highLength.Length - 1
			? context.Source.highLength[start.IntIndex] : fragment) - start.RestIndex;
		stack.Insert(stackIndex, (context.Source.high[start.IntIndex],
			ProcessReverse(context.Source.high[start.IntIndex], start.RestIndex, previousPart),
			context.Destination, context.DestinationIndex, previousPart));
		for (var i = start.IntIndex + 1; i < end.IntIndex; i++)
		{
			var branchLength = i < context.Source.highLength.Length - 1 ? context.Source.highLength[i] : fragment;
			stack.Insert(stackIndex, (context.Source.high[i], 0, context.Destination,
				context.DestinationIndex + previousPart, branchLength));
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (context.Source.high[end.IntIndex],
			ProcessReverse(context.Source.high[end.IntIndex], 0, context.Length - previousPart),
			context.Destination, context.DestinationIndex + previousPart, context.Length - previousPart));
	}

	protected static void CopyRangeToSimilar(List<CopyRangeContext> stack, CopyRangeContext context, int stackIndex)
	{
		if (!(context.Source.high != null && context.Source.highLength != null
			&& context.Destination.high != null && context.Destination.highLength != null))
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		if (context.SourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Индекс не может быть отрицательным.");
		if (context.DestinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Индекс не может быть отрицательным.");
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
		CopyRangeIndex sourceStart = new(context.Source.highLength, context.SourceIndex);
		CopyRangeIndex sourceEnd = new(context.Source.highLength, sourceEndIndex);
		CopyRangeIndex destinationStart = new(context.Destination.highLength, context.DestinationIndex);
		CopyRangeIndex destinationEnd = new(context.Destination.highLength, destinationEndIndex);
		// Сохраняем значение как изменяющееся после реверса
		while (sourceEnd.RestIndex >= context.Source.fragment)
			sourceEnd = (sourceEnd.Index, sourceEnd.IntIndex + 1, sourceEnd.RestIndex - context.Source.fragment);
		if (context.Destination.highLength.Length != 0 && destinationEnd.IntIndex == context.Destination.highLength.Length)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex,
				destinationEnd.RestIndex - (fragment - context.Destination.highLength[^1]));
		while (destinationEnd.RestIndex >= fragment)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex + 1, destinationEnd.RestIndex - fragment);
		// Диапазон дописывается после конца списка
		if (destinationStart.IntIndex != 0 && destinationStart.IntIndex == context.Destination.highLength.Length
			&& destinationStart.RestIndex == 0 && context.Destination.highLength[destinationStart.IntIndex - 1] != fragment)
			destinationStart = (destinationStart.Index, destinationStart.IntIndex - 1, context.Destination.highLength[^1]);
		if (destinationEnd.IntIndex != 0 && destinationEnd.RestIndex < 0)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex - 1, destinationEnd.RestIndex + fragment);
		// Если список после серии удалений стал слишком "рыхлым", уплотняем
		if (destinationEnd.IntIndex >= context.Destination.high.Length
			|| destinationEnd.RestIndex < 0 || context.Destination.highLength.Length != 0
			&& context.Destination.highLength.ValuesSum + fragment - context.Destination.highLength[^1]
			+ (context.Destination.high.Length - context.Destination.highLength.Length - 1) * fragment
			+ context.Destination.high[^1].Capacity < context.DestinationIndex + context.Length)
		{
			Debug.Assert(!compactified);
			context.Destination.Compactify(context.DestinationIndex + context.Length);
			compactified = true;
			goto sameFragment;
		}
		else
		{
			Debug.Assert(sourceStart.IntIndex >= 0 && sourceStart.IntIndex < context.Source.highLength.Length
				&& sourceEnd.IntIndex >= 0 && sourceEnd.IntIndex < context.Source.highLength.Length
				&& destinationStart.IntIndex >= 0 && destinationStart.IntIndex < context.Destination.high.Length
				&& destinationEnd.IntIndex >= 0 && destinationEnd.IntIndex < context.Destination.high.Length
				&& sourceStart.RestIndex >= 0 && sourceEnd.RestIndex >= 0
				&& destinationStart.RestIndex >= 0 && destinationEnd.RestIndex >= 0);
			if (context.Source.IsReversed != context.Destination.IsReversed)
				CopyRangeToSimilarDiagonal(stack, context, stackIndex, fragment,
					(sourceStart, sourceEnd), (destinationStart, destinationEnd));
			else if (context.SourceIndex >= context.DestinationIndex
				|| context.DestinationIndex >= context.SourceIndex + context.Length || context.Source != context.Destination)
				CopyRangeToSimilarLTR(stack, context, stackIndex, fragment,
					(sourceStart, sourceEnd), (destinationStart, destinationEnd));
			else
				CopyRangeToSimilarRTL(stack, context, stackIndex, fragment,
					(sourceStart, sourceEnd), (destinationStart, destinationEnd));
		}
	}

	protected static void CopyRangeToSimilarDiagonal(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		Debug.Assert(context.Source.high != null && context.Source.highLength != null
			&& context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var sourceCurrentRestIndex = sourceIndexes.End.RestIndex + 1;
		var destinationCurrentRestIndex = destinationIndexes.Start.RestIndex;
		var iSource = sourceIndexes.End.IntIndex;
		var iDestination = destinationIndexes.Start.IntIndex;
		var startRestIndexDiff = (sourceIndexes.Start.RestIndex - destinationIndexes.Start.RestIndex).Abs();
		startRestIndexDiff = RedStarLinqMath.Max(startRestIndexDiff, fragment - startRestIndexDiff);
		NListHashSet<int> hs = [];
		MpzT step = 0;
		var leftLength = context.Length;
		while (leftLength > 0)
		{
			var destinationMax = context.Destination.highLength.Length > iDestination + 1
				? context.Destination.highLength[iDestination] : fragment;
			step = RedStarLinqMath.Min(context.Source.highLength[iSource], startRestIndexDiff,
				(iDestination == destinationIndexes.End.IntIndex
				? destinationIndexes.End.RestIndex + 1 : fragment) - step,
				sourceCurrentRestIndex, destinationMax - destinationCurrentRestIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			var sourceThresholdReached = (sourceCurrentRestIndex -= step) == 0;
			var newSize = RedStarLinqMath.Min(destinationMax, destinationCurrentRestIndex + leftLength);
			var oldSize = context.Destination.high[iDestination].Length;
			if (newSize < oldSize)
				newSize = oldSize;
			if (newSize != oldSize && !hs.Contains(iDestination))
			{
				context.Destination.highLength.SetOrAdd(iDestination, newSize);
				stack.Insert(stackIndex, (null!, -1, context.Destination.high[iDestination],
					-1, oldSize - newSize));
				hs.Add(iDestination);
			}
			stack.Insert(stackIndex, (context.Source.high[iSource],
				ProcessReverse(context.Source.high[iSource], sourceCurrentRestIndex, step),
				context.Destination.high[iDestination],
				ProcessReverse(context.Destination.high[iDestination],
				destinationCurrentRestIndex, step, context.Destination.highLength[iDestination]), step));
			var destinationThresholdReached = (destinationCurrentRestIndex += step) == destinationMax;
			leftLength -= step;
			if (sourceThresholdReached)
			{
				iSource--;
				sourceCurrentRestIndex = iSource >= sourceIndexes.Start.IntIndex
					? context.Source.highLength[iSource] : fragment;
			}
			if (destinationThresholdReached)
			{
				iDestination++;
				destinationCurrentRestIndex = 0;
			}
		}
		context.Destination.highLength.SetOrAdd(destinationIndexes.End.IntIndex,
			RedStarLinqMath.Max(context.Destination.highLength.Length > destinationIndexes.End.IntIndex
			? context.Destination.highLength[destinationIndexes.End.IntIndex] : 0, destinationIndexes.End.RestIndex + 1));
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToSimilarLTR(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		Debug.Assert(context.Source.high != null && context.Source.highLength != null
			&& context.Destination.high != null && context.Destination.highLength != null);
		// Диапазон копируется слева направо
		var sourceCurrentRestIndex = sourceIndexes.Start.RestIndex;
		var destinationCurrentRestIndex = destinationIndexes.Start.RestIndex;
		var iSource = sourceIndexes.Start.IntIndex;
		var iDestination = destinationIndexes.Start.IntIndex;
		var startRestIndexDiff = (sourceIndexes.Start.RestIndex - destinationIndexes.Start.RestIndex).Abs();
		startRestIndexDiff = RedStarLinqMath.Max(startRestIndexDiff, fragment - startRestIndexDiff);
		NListHashSet<int> hs = [];
		MpzT step = 0;
		TCertain currentSource, currentDestination;
		var leftLength = context.Length;
		while (leftLength > 0)
		{
			var destinationMax = iDestination + 1 < context.Destination.highLength.Length
				? context.Destination.highLength[iDestination] : fragment;
			step = RedStarLinqMath.Min(context.Source.highLength[iSource] - step, startRestIndexDiff,
				(iDestination == destinationIndexes.End.IntIndex
				? destinationIndexes.End.RestIndex + 1 : fragment) - step,
				context.Source.highLength[iSource] - sourceCurrentRestIndex,
				destinationMax - destinationCurrentRestIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			var newSize = RedStarLinqMath.Min(destinationMax, destinationCurrentRestIndex + leftLength);
			currentDestination = context.Destination.high[iDestination];
			var oldSize = currentDestination.Length;
			if (newSize < oldSize)
				newSize = oldSize;
			if (newSize != oldSize && !hs.Contains(iDestination))
			{
				context.Destination.highLength.SetOrAdd(iDestination, newSize);
				stack.Insert(stackIndex, (null!, -1, currentDestination,
					-1, oldSize - newSize));
				hs.Add(iDestination);
			}
			currentSource = context.Source.high[iSource];
			stack.Insert(stackIndex, (currentSource,
				ProcessReverse(currentSource, sourceCurrentRestIndex, step,
				context.Source != context.Destination || currentSource.low == null && !hs.Contains(iSource)
				? 0 : context.Source.highLength[iSource]), currentDestination,
				ProcessReverse(currentDestination,
				destinationCurrentRestIndex, step, newSize), step));
			var sourceThresholdReached = (sourceCurrentRestIndex += step) == context.Source.highLength[iSource];
			var destinationThresholdReached = (destinationCurrentRestIndex += step) == destinationMax;
			leftLength -= step;
			if (sourceThresholdReached)
			{
				iSource++;
				sourceCurrentRestIndex = 0;
			}
			if (destinationThresholdReached)
			{
				iDestination++;
				destinationCurrentRestIndex = 0;
			}
		}
		context.Destination.highLength.SetOrAdd(destinationIndexes.End.IntIndex,
			RedStarLinqMath.Max(context.Destination.highLength.Length > destinationIndexes.End.IntIndex
			? context.Destination.highLength[destinationIndexes.End.IntIndex] : 0, destinationIndexes.End.RestIndex + 1));
#if VERIFY
		context.Source.Verify();
		context.Destination.Verify();
#endif
	}

	protected static void CopyRangeToSimilarRTL(List<CopyRangeContext> stack, CopyRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		var (source, sourceIndex, destination, destinationIndex, length) = context;
		Debug.Assert(source == destination
			&& source.high != null && source.highLength != null);
		MpzT destinationHighLengthOld = new(source.highLength.Length);
		for (var i = source.highLength.Length; i < destinationIndexes.End.IntIndex + 1; i++)
			source.highLength.Add(1);
		// Диапазон копируется справа налево
		var sourceCurrentRestIndex = sourceIndexes.End.RestIndex + 1;
		var destinationCurrentRestIndex = destinationIndexes.End.RestIndex + 1;
		var iSource = sourceIndexes.End.IntIndex;
		var iDestination = destinationIndexes.End.IntIndex;
		var startRestIndexDiff = (sourceIndexes.Start.RestIndex - destinationIndexes.Start.RestIndex).Abs();
		startRestIndexDiff = RedStarLinqMath.Max(startRestIndexDiff, fragment - startRestIndexDiff);
		MpzT step = 0, newSize, oldSize;
		NListHashSet<int> hs = [];
		for (var i = destinationIndexes.Start.IntIndex; i < destinationIndexes.End.IntIndex; i++)
		{
			newSize = i >= destinationHighLengthOld - 1 ? fragment : 0;
			oldSize = source.high[i].Length;
			if (newSize > oldSize)
			{
				source.highLength[i] = newSize;
				stack.Insert(stackIndex, (null!, -1, source.high[i], -1, oldSize - newSize));
				hs.Add(i);
			}
		}
		newSize = destinationIndexes.End.RestIndex + 1;
		oldSize = source.high[destinationIndexes.End.IntIndex].Length;
		if (newSize > oldSize)
		{
			source.highLength[destinationIndexes.End.IntIndex] = newSize;
			stack.Insert(stackIndex, (null!, -1, source.high[destinationIndexes.End.IntIndex],
				-1, oldSize - newSize));
				hs.Add(destinationIndexes.End.IntIndex);
		}
		TCertain currentSource;
		var lengthSource = source.highLength[iSource];
		var leftLength = length;
		while (leftLength > 0)
		{
			step = RedStarLinqMath.Min(lengthSource - step, startRestIndexDiff,
				(iDestination == destinationIndexes.End.IntIndex
				? destinationIndexes.End.RestIndex + 1 : iDestination
				< destinationHighLengthOld ? source.highLength[iDestination] : fragment)
				- step, sourceCurrentRestIndex, destinationCurrentRestIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			var sourceThresholdReached = (sourceCurrentRestIndex -= step) == 0;
			var destinationThresholdReached = (destinationCurrentRestIndex -= step) == 0;
			currentSource = source.high[iSource];
			stack.Insert(stackIndex, (currentSource,
				ProcessReverse(currentSource, sourceCurrentRestIndex, step,
				currentSource.low == null && !hs.Contains(iSource) ? 0 : lengthSource),
				source.high[iDestination],
				ProcessReverse(source.high[iDestination], destinationCurrentRestIndex,
				step, source.highLength[iDestination]), step));
			leftLength -= step;
			if (sourceThresholdReached)
			{
				iSource--;
				sourceCurrentRestIndex = iSource >= sourceIndexes.Start.IntIndex
					? lengthSource = source.highLength[iSource] : fragment;
			}
			if (destinationThresholdReached)
			{
				iDestination--;
				destinationCurrentRestIndex = iDestination < destinationHighLengthOld - 1
					? source.highLength[iDestination] : fragment;
			}
		}
#if VERIFY
		source.Verify();
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void CopyRangeTrivial(CopyRangeContext context)
	{
		var (source, sourceIndex, destination, destinationIndex, length) = context;
		Debug.Assert(source.low != null && destination.low != null);
		MpzT oldLength = new(destination.Length);
		if (destinationIndex == 0 && length >= destination.Length)
			destination.bReversed = false;
		// Если копируемый диапазон не помещается в целевой список, расширяем его
		if (destination.low.Length < destinationIndex + length)
		{
			if (destination.bReversed)
			{
				destination.low.ResizeLeft((int)(destinationIndex + length));
				destination.Length = destination.low.Length;
				if (source == destination)
					sourceIndex += destination.low.Length - oldLength;
			}
			else
			{
				// Иначе справа
				destination.low.Resize((int)(destinationIndex + length));
				destination.Length = destination.low.Length;
			}
		}
		// Если оба списка направлены одинаково, копируем напрямую
		if (source.IsReversed == destination.IsReversed)
			source.low.CopyRangeTo((int)sourceIndex, destination.low, (int)destinationIndex, (int)length);
		else // Иначе используем срезы для предотвращения лишних проходов
			source.CollectionLowCreator(source.low.Skip((int)sourceIndex).Take((int)length).Reverse())
				.CopyRangeTo(0, destination.low, (int)destinationIndex, (int)length);
#if VERIFY
		Debug.Assert(destination.Length == RedStarLinqMath.Max(oldLength, destinationIndex + length));
		source.Verify();
		destination.Verify();
#endif
	}

	protected override void CopyToInternal(MpzT sourceIndex, TCertain destination,
		MpzT destinationIndex, MpzT length) =>
		CopyToInternal(sourceIndex, destination, destinationIndex, length, false);

	protected virtual void CopyToInternal(MpzT sourceIndex, TCertain destination,
		MpzT destinationIndex, MpzT length, bool ignoreReversed)
	{
		bool bReversedOld = bReversed, destinationReversedOld = destination.bReversed;
		if (bReversedOld && !ignoreReversed)
		{
			Reverse();
			sourceIndex = Length - length - sourceIndex;
		}
		if (destinationReversedOld && !ignoreReversed && this != destination)
		{
			destination.Reverse();
			if (destination.Length < destinationIndex + length)
				destination.ResizeInternal(destinationIndex + length);
			destinationIndex = destination.Length - length - destinationIndex;
		}
		CopyRange(((TCertain)this, sourceIndex, destination, destinationIndex, length));
		if (bReversedOld && !ignoreReversed)
			Reverse();
		if (destinationReversedOld && !ignoreReversed && this != destination)
			destination.Reverse();
	}

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
		Root.accessCache?.Clear();
		bReversed = false;
		GC.SuppressFinalize(this);
	}

	protected override T GetInternal(MpzT index, bool invoke = true)
	{
		if (low != null)
			return low[(int)(bReversed ? Length - 1 - index : index)];
		else if (GetProcessAccessCache(Root, index, invoke, out var result))
			return result;
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
	}

	protected static bool GetProcessAccessCache(TCertain root, MpzT index, bool invoke, [MaybeNullWhen(false)] out T result)
	{
		TCertain branch;
		MpzT start;
		if (root.accessCache == null || root.accessCache.Length == 0)
		{
			root.accessCache ??= new();
			(branch, start) = (root, 0);
		}
		else
			(branch, start) = root.accessCache.Peek();
		MpzT reducedIndex;
		while (index < start || index >= start + branch.Length)
		{
			root.accessCache.Pop();
			if (root.accessCache.Length == 0)
			{
				root.accessCache ??= new();
				(branch, start) = (root, 0);
				break;
			}
			(branch, start) = root.accessCache.Peek();
		}
		while (branch.low == null)
		{
			if (branch.high == null || branch.highLength == null)
			{
				result = default;
				return false;
			}
			reducedIndex = branch.IsReversed ? start + branch.Length - 1 - index : index - start;
			var intIndex = branch.highLength.IndexOfNotGreaterSum(reducedIndex, out var bitsIndex);
			root.accessCache.Push((branch.high[intIndex], start + (branch.IsReversed
				? branch.Length - (reducedIndex - bitsIndex + branch.high[intIndex].Length) : reducedIndex - bitsIndex)));
			(branch, start) = root.accessCache.Peek();
		}
		reducedIndex = (branch.parent?.IsReversed ?? false) ? start + branch.Length - 1 - index : index - start;
		Debug.Assert(reducedIndex >= 0 && reducedIndex < branch.Length);
		result = branch.low[(int)(branch.bReversed ? branch.Length - 1 - reducedIndex : reducedIndex), invoke];
		return true;
	}

	protected override void GetRangeCopyTo(MpzT index, MpzT length, TCertain list) =>
		CopyRange(ProcessReverseContext(((TCertain)this, index, list, 0, length), processDestination: false));

	protected virtual void IncreaseCapacity(MpzT value, MpzT targetFragment)
	{
		var this2 = (TCertain)this;
		Root.accessCache?.Clear();
		if (low != null)
		{
			if (value <= LeafSize)
			{
				low.Capacity = (int)value;
				AddCapacity(value - _capacity);
#if VERIFY
				Verify();
#endif
				return;
			}
			fragment = targetFragment;
			// Количество подветок корня
			var highCount = (int)GetArrayLength(value, fragment);
			high = new(highCount, true);
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
			{
				first.highLength = [preservedLow.Length];
				first.high[0]._capacity = RedStarLinqMath.Min(first.fragment, value);
			}
			ArgumentNullException.ThrowIfNull(first.low);
			// Вставляем первый лист
			first.low.AddRange(preservedLow);
			if (preservedLow.Length != 0)
				first.Length = preservedLow.Length;
			AddCapacity(value - _capacity);
#if VERIFY
			if (high != null && highLength != null)
				Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Verify();
#endif
			return;
		}
		Debug.Assert(high != null && highLength != null);
		var newFragment = fragment << SubbranchesBitLength;
		var value2 = RedStarLinqMath.Min(value, newFragment);
		if (_capacity < newFragment)
		{
			var targetFragment2 = fragment == LeafSize ? 1 : fragment >> SubbranchesBitLength;
			if (GetArrayLength(value2, fragment) == GetArrayLength(_capacity, fragment))
			{
				high[^1].IncreaseCapacity(value2 % fragment == 0 ? fragment : value2 % fragment, targetFragment2);
				goto mainCycle;
			}
			if (_capacity < fragment * high.Length)
			{
				high[^1].IncreaseCapacity(fragment, targetFragment2);
				if (_capacity == value2)
					goto mainCycle;
			}
			var highCount = (int)GetArrayLength(value2 - _capacity, fragment);
			high.Capacity = Max(high.Capacity, high.Length + highCount);
			for (var i = 0; i < highCount - 1; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			var leftCapacity = (value2 - _capacity) % fragment;
			if (leftCapacity == 0)
				leftCapacity = fragment;
			high.Add(CapacityCreator(leftCapacity));
			high[^1].parent = this2;
			AddCapacity(leftCapacity);
		}
	mainCycle:
		if (_capacity == value)
		{
#if VERIFY
			if (high != null && highLength != null)
				Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
			Verify();
#endif
			return;
		}
		while (fragment < targetFragment)
		{
			newFragment = fragment << SubbranchesBitLength;
			value2 = RedStarLinqMath.Min(value, newFragment << SubbranchesBitLength);
			var highCount = (int)RedStarLinqMath.Min(GetArrayLength(value2, newFragment), Subbranches);
			var oldHigh = high;
			var oldHighLength = highLength;
			var oldLength = Length;
			high = new(highCount, true) { CapacityCreator(0) };
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
			highLength = [Length];
			oldHigh.ForEach(x => x.parent = first);
			for (var i = 1; i < high.Capacity - 1; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			var leftCapacity = value2 % fragment;
			if (leftCapacity == 0)
				leftCapacity = fragment;
			high.Add(CapacityCreator(leftCapacity));
			high[^1].parent = this2;
			AddCapacity(leftCapacity);
			if (_capacity == value)
			{
#if VERIFY
				if (high != null && highLength != null)
					Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
				Verify();
#endif
				return;
			}
		}
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
		var compactified = false;
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
		Root.accessCache?.Clear();
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
				Debug.Assert(!compactified);
				Compactify(index);
				compactified = true;
				goto start;
			}
		}
		if (Length > fragment * Subbranches - 1)
		{
			Debug.Assert(parent == null);
			Capacity <<= 1;
			goto start;
		}
		if (intIndex < highLength.Length && (high[intIndex].low != null && high[intIndex].Length == LeafSize
			|| high[intIndex].high != null && high[intIndex].highLength != null
			&& high[intIndex].Length > high[intIndex].fragment * high[intIndex].Subbranches - 1))
		{
			if (highLength.Length == Subbranches)
			{
				CopyToInternal(index, this2, index + 1, Length - index, true);
				SetInternal(index, item);
				goto end;
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
			CopyRange(ProcessReverseContext((high[intIndex], bitsIndex, temp, 0,
				high[intIndex].Length - bitsIndex), processDestination: false));
			temp.parent = this2;
			highLength.Insert(intIndex + 1, high[intIndex].Length - bitsIndex);
			high[intIndex].SetInternal(bitsIndex, item);
			high[intIndex].RemoveEndInternal(bitsIndex + 1);
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
	end:
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
		Root.accessCache?.Clear();
		if (low != null)
		{
			Debug.Assert(index < int.MaxValue - 1);
			low.Insert((int)(bReversed ? Length - index : index), bigList.Wrap(x => bReversed ? x.Reverse() : x));
			Length += length;
#if VERIFY
			Verify();
#endif
			return;
		}
		else if (high == null || highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		if (Length <= 1)
			bReversed = false;
		var bReversedOld = bReversed;
		if (bReversedOld)
		{
			Reverse();
			bigList.Reverse();
			index = Length - index;
		}
		var endIndex = index + length - 1;
		var compactified = false;
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
				Debug.Assert(!compactified);
				Compactify(index);
				compactified = true;
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
		else
		{
			var oldLength = Length;
			if (index + length > Length)
				ResizeInternal(index + length);
			CopyToInternal(index, this2, index + length, oldLength - index, true);
			bigList.CopyToInternal(0, this2, index, length, true);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static MpzT ProcessReverse(TCertain source, MpzT sourceIndex, MpzT length, MpzT bound = default)
	{
		if (bound.val == default || bound < source.Length)
			bound = source.Length;
		return source.bReversed ? bound - length - sourceIndex : sourceIndex;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static CopyRangeContext ProcessReverseContext(CopyRangeContext context,
		MpzT destinationBound = default, bool processSource = true,
		bool processDestination = true) =>
		(context.Source, processSource ? ProcessReverse(context.Source, context.SourceIndex,
		context.Length, context.Source.Length) : context.SourceIndex, context.Destination, processDestination
		? ProcessReverse(context.Destination, context.DestinationIndex, context.Length,
		destinationBound) : context.DestinationIndex, context.Length);

	protected virtual void ReduceCapacityExponential(MpzT newFragment)
	{
		var this2 = (TCertain)this;
		var reverse = false;
		Root.accessCache?.Clear();
		do
		{
			Debug.Assert(high != null && highLength != null);
			if (high.Length > 1 && high[1].Length != 0)
				Compactify(Length);
			fragment >>= SubbranchesBitLength;
			var oldHigh = high;
			var oldHighLength = highLength;
			oldHigh.Skip(1).ForEach(x => x.Dispose());
			highLength.Dispose();
			low = oldHigh[0].low;
			high = oldHigh[0].high;
			highLength = oldHigh[0].highLength;
			AddCapacity(oldHigh[0].Capacity - _capacity);
			reverse ^= oldHigh[0].bReversed;
			oldHigh.Dispose();
			oldHighLength.Dispose();
			if (high == null || highLength == null)
			{
				Debug.Assert(low != null);
				break;
			}
			high.ForEach(x => x.parent = this2);
		} while (fragment > newFragment);
		if (reverse)
			Reverse();
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
			Compactify(Length);
		for (var i = high.Length - 1; i >= highCount; i--)
		{
			AddCapacity(-high[i].Capacity);
			high[i].Dispose();
		}
		high.RemoveEnd(highCount);
		var leftCapacity = value % fragment;
		if (leftCapacity == 0)
			leftCapacity = fragment;
		if (high[^1].Length > leftCapacity)
			Compactify(Length);
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
		Root.accessCache?.Clear();
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
				Index pasteIndex = intIndex;
				var temp = high[intIndex];
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(intIndex + 1, high, intIndex, high.Length - intIndex - offsetFromEnd - 1);
				high[pasteIndex = ^(offsetFromEnd + 1)] = temp;
				var lastHigh = high[^1];
				lastHigh.Capacity = fragment;
				(high[pasteIndex], high[^1]) = (lastHigh, temp);
				temp.ClearInternal(false);
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
		Root.accessCache?.Clear();
		if (low != null)
		{
			low.RemoveEnd((int)index);
			Length = index;
		}
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var bitsIndex);
			for (var i = high.Length - 1; i > intIndex; i--)
				high[i].ClearInternal(false);
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
		else if (index == 0 && length == Length)
		{
			ClearInternal(true);
			return;
		}
		else if (high == null || highLength == null)
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
		Root.accessCache?.Clear();
		var endIndex = index + length - 1;
		if (bReversed)
			(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
		var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
		var endIntIndex = highLength.IndexOfNotGreaterSum(endIndex, out var endBitsIndex);
		var highAtI = high[intIndex];
		if (intIndex == endIntIndex)
		{
			if (intIndex != highLength.Length - 1 && bitsIndex == 0 && endBitsIndex == highLength[intIndex] - 1)
			{
				Index pasteIndex = intIndex;
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(intIndex + 1, high, intIndex, high.Length - intIndex - offsetFromEnd - 1);
				high[pasteIndex = ^(offsetFromEnd + 1)] = highAtI;
				var lastHigh = high[^1];
				if (lastHigh.Capacity != fragment)
					lastHigh.IncreaseCapacity(fragment, high[0].fragment);
				(high[pasteIndex], high[^1]) = (lastHigh, highAtI);
				highAtI.ClearInternal(false);
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
				highAtI.RemoveInternal(bitsIndex, highAtI.Length - bitsIndex);
				highLength[intIndex] = bitsIndex;
			}
			var pasteIndex = intIndex + startOffset;
			var tempRange = high.GetRange(pasteIndex, endIntIndex + endOffset - (intIndex + startOffset));
			var tempOffset = Capacity == high.Length * fragment ? 0 : 1;
			var copyLength = high.Length - (endIntIndex + endOffset) - tempOffset;
			if (copyLength > 0)
			{
				high.CopyRangeTo(endIntIndex + endOffset, high, intIndex + startOffset, copyLength);
				tempRange.CopyRangeTo(0, high, pasteIndex = high.Length + intIndex + startOffset
					- (endIntIndex + endOffset) - tempOffset, tempRange.Length);
			}
			if (tempRange.Length != 0)
			{
				var lastHigh = high[^1];
				if (lastHigh.Capacity != fragment)
					lastHigh.IncreaseCapacity(fragment, high[0].fragment);
				(high[pasteIndex], high[^1]) = (lastHigh, tempRange[0]);
				tempRange[0].ClearInternal(false);
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

	protected override void ResizeInternal(MpzT newSize)
	{
		if (newSize < Length)
		{
			RemoveEndInternal(newSize);
			return;
		}
		EnsureCapacity(newSize);
		newSize -= Length;
		CopyRange((null!, -1, (TCertain)this, -1, -newSize));
	}

	/// <summary>
	/// Этот метод выполняется моментально, всего лишь меняя состояние внутреннего флага "список перевернут"
	/// на противоположное. После этого функции Add(), RemoveEnd() и CopyTo() радикально меняют
	/// свое поведение, многие другие меняют слегка.
	/// </summary>
	/// <returns>Ссылка на себя.</returns>
	public override TCertain Reverse()
	{
		Root.accessCache?.Clear();
		bReversed = !bReversed;
		return (TCertain)this;
	}

	protected override void ReverseInternal(MpzT index, MpzT length)
	{
		if (length <= 1)
			return;
		if (index == 0 && length == Length)
		{
			Reverse();
			return;
		}
		if (low != null)
		{
			low.Reverse((int)(bReversed ? Length - length - index : index), (int)length);
#if VERIFY
			Verify();
#endif
			return;
		}
		Root.accessCache?.Clear();
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
					high[i].Reverse().Capacity = fragment;
				high.Reverse(intIndex, endIntIndex + 1 - intIndex);
				highLength.Reverse(intIndex, endIntIndex + 1 - intIndex);
				return;
			}
			for (var i = intIndex + 1; i < endIntIndex; i++)
				high[i].Reverse();
			high.Reverse(intIndex + 1, endIntIndex - 1 - intIndex);
			highLength.Reverse(intIndex + 1, endIntIndex - 1 - intIndex);
			TCertain tempRange;
			if (endBitsIndex + 1 == highLength[intIndex] - bitsIndex)
			{
				tempRange = high[endIntIndex].GetRangeInternal(0, endBitsIndex + 1, endBitsIndex + 1 == high[endIntIndex].Length);
				CopyRange(ProcessReverseContext((high[intIndex], bitsIndex, high[endIntIndex], 0, endBitsIndex + 1)));
				high[endIntIndex].ReverseInternal(0, endBitsIndex + 1);
				tempRange.Reverse();
				CopyRange(ProcessReverseContext((tempRange, 0, high[intIndex], bitsIndex,
					tempRange.Length), processSource: false));
				return;
			}
			var greaterAtEnd = endBitsIndex + 1 > highLength[intIndex] - bitsIndex;
			var greaterIndex = greaterAtEnd ? endIntIndex : intIndex;
			var lessIndex = greaterAtEnd ? intIndex : endIntIndex;
			var greater = high[greaterIndex];
			var less = high[lessIndex];
			var commonLength = greaterAtEnd ? highLength[intIndex] - bitsIndex : (endBitsIndex + 1) % fragment;
			var endCommonIndex = greaterAtEnd ? endBitsIndex + 1 - commonLength : 0;
			var difference = (endBitsIndex + 1 - highLength[intIndex] + bitsIndex).Abs();
			tempRange = high[endIntIndex].GetRangeInternal(endCommonIndex, commonLength,
				endCommonIndex == 0 && commonLength == high[endIntIndex].Length);
			CopyRange(ProcessReverseContext((high[intIndex], bitsIndex, high[endIntIndex], endCommonIndex, commonLength)));
			high[endIntIndex].ReverseInternal(endCommonIndex, commonLength);
			tempRange.Reverse();
			CopyRange(ProcessReverseContext((tempRange, 0, high[intIndex], bitsIndex, commonLength), processSource: false));
			var takeIndex = greaterAtEnd ? 0 : bitsIndex + commonLength;
			var insertIndex = greaterAtEnd ? less.Length : 0;
			if (less.Length + difference <= fragment)
			{
				if (less.Length + difference > less.Capacity)
					less.Capacity = fragment;
				less.InsertInternal(insertIndex, greater.GetRangeInternal(takeIndex, difference).Reverse());
				greater.RemoveInternal(takeIndex, difference);
				highLength[greaterIndex] -= difference;
				highLength[lessIndex] += difference;
				return;
			}
			if (difference <= 1 && endIntIndex - 1 - intIndex == 0)
				return;
			var insertionIncrement = greaterAtEnd ? highLength[intIndex] - bitsIndex : 0;
			tempRange = greater.GetRangeInternal(takeIndex, difference);
			greater.RemoveInternal(takeIndex, difference);
			tempRange.Reverse();
			highLength[greaterIndex] -= difference;
#if VERIFY
			Verify();
#endif
			InsertInternal(highLength.GetLeftValuesSum(lessIndex, out _) + insertIndex, tempRange);
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

	protected override void SetOrAddInternal(MpzT index, T value)
	{
		if (index == Length)
		{
			Add(value);
			return;
		}
		SetInternal(index, value);
#if VERIFY
		if (high != null && highLength != null)
			Debug.Assert(Length == highLength.ValuesSum && Length == high.Sum(x => x.Length));
		Verify();
#endif
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
		{
			Debug.Assert(Length == low.Length);
			Debug.Assert(Length <= LeafSize);
		}
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
		TCertain Destination, MpzT DestinationIndex, MpzT Length)
	{
		public static implicit operator CopyRangeContext((TCertain Source, MpzT SourceIndex,
			TCertain Destination, MpzT DestinationIndex, MpzT Length) x) =>
			new(x.Source, x.SourceIndex, x.Destination, x.DestinationIndex, x.Length);
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
