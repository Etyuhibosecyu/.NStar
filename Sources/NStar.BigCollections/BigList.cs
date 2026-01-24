global using Mpir.NET;
global using NStar.BufferLib;
global using NStar.Dictionaries;
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
using static Mpir.NET.Mpir;

namespace NStar.BigCollections;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BigList<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow>
	where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected LimitedBuffer<TCertain>? high;
	private protected ListOfBigSums? highLength;
	private protected TCertain? parent;
	private protected MpzT _capacity = 0;
	private protected MpzT fragment = 1;
	private protected Stack<(TCertain Branch, MpzT Start)>? accessCache;
	private protected List<CopyRangeContext>? copyStack;
	private protected List<MoveRangeContext>? moveStack;
	private protected bool bReversed;

	protected const string InternalError = "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
		+ "2. Нарушение целостности структуры списка (ошибка в логике -"
		+ " список все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
		+ "3. Системная ошибка (память, диск и т. д.).\r\n";

	protected BigList() : this(-1) { }

	protected BigList(int subbranchesBitLength, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength is >= 2 and <= 30)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength is >= 2 and <= 30)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength is >= 2 and <= 30)
			LeafSizeBitLength = subbranchesBitLength;
		low = new();
#if VERIFY
		Verify();
#endif
	}

	protected BigList(MpzT capacity, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength is >= 2 and <= 30)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength is >= 2 and <= 30)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength is >= 2 and <= 30)
			LeafSizeBitLength = subbranchesBitLength;
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity == 0)
			low = new();
		ConstructFromCapacity(capacity);
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected BigList(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
	{
		if (subbranchesBitLength is >= 2 and <= 30)
			SubbranchesBitLength = subbranchesBitLength;
		if (leafSizeBitLength is >= 2 and <= 30)
			LeafSizeBitLength = leafSizeBitLength;
		else if (subbranchesBitLength is >= 2 and <= 30)
			LeafSizeBitLength = subbranchesBitLength;
		ArgumentNullException.ThrowIfNull(collection);
		low = new();
		ConstructFromEnumerable(collection);
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected BigList(MpzT capacity, G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(capacity, subbranchesBitLength, leafSizeBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected BigList(ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(collection.Length, subbranchesBitLength, leafSizeBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected BigList(MpzT capacity, ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(RedStarLinqMath.Max(capacity, collection.Length), subbranchesBitLength, leafSizeBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	public override MpzT Capacity
	{
		get => _capacity;
		set
		{
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
					high = null;
					highLength?.Dispose();
					highLength = null;
				}
				high = null;
				highLength = null;
			}
			else if (value <= LeafSize) // Список сокращается до одного листа
			{
				if (high != null)
					ReduceCapacityExponential(LeafSize);
				if (high != null) // В этом случае в списке очень много пустоты
					MoveRange((null!, -2, (TCertain)this, -1, -_capacity, MoveRangeMode.None));
				var first = this;
				var reverse = false;
				// Ищем первый лист и разрушаем "лишние" ветки
				while (first.high != null)
				{
					first.high.Skip(1).ForEach(x => x.Dispose());
					first.highLength?.Dispose();
					first.highLength = null;
					first = first.high[0];
					first.parent?.high?.Dispose();
					first.parent?.high = null;
					reverse ^= first.bReversed;
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
				IncreaseLowCapacity(value, fragment);
				return;
			}
			else if (high != null) // Как старая, так и новая емкость определяет много листьев
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
			if (high != null)
				Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
		var compactified = false;
		if (low != null)
		{
			low.Insert(bReversed ? 0 : ^0, item);
			Length += 1;
#if VERIFY
			if (high != null)
				Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
			Verify();
#endif
			return this2;
		}
		else if (high == null)
			throw new InvalidOperationException("Невозможно добавить элемент. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		var bReversedOld = bReversed;
		if (bReversedOld)
		{
			InsertInternal(Length, item);
			return this2;
		}
		while (true)
		{
			var intIndex = IndexOfNotGreaterSum(Length, out _);
			if (intIndex != 0 && high[intIndex - 1].Capacity != high[intIndex - 1].Length)
				intIndex--;
			if (high.Length == intIndex)
			{
				if (high[^1].Capacity != fragment)
				{
					high[^1].IncreaseCapacity(fragment, high[0].fragment);
					intIndex--;
				}
				else if (high.Length < Subbranches)
				{
					high.Capacity = Min(high.Capacity << 1, Subbranches);
					high.Add(CapacityCreator(fragment));
					high[^1].parent = this2;
					AddCapacity(fragment);
				}
				else
				{
					Debug.Assert(!compactified);
					MoveRange((null!, -2, (TCertain)this, -1, -Length - 1, MoveRangeMode.None));
					compactified = true;
					continue;
				}
			}
			high[intIndex].Add(item);
			if (!HasSufficientLength(intIndex))
				highLength?.Add(1);
			else
				highLength?[intIndex] += 1;
			break;
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
		return this2;
	}

	protected void AddCapacity(MpzT increment)
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
		else if (high != null)
		{
			foreach (var x in high)
				x?.ClearInternal(false);
			highLength?.Clear();
		}
		else
			throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
				+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.");
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
		else if (high != null)
		{
			if (bReversed)
				index = Length - length - index;
			var endIndex = index + length - 1;
			var ((_, intIndex, restIndex), (_, endIntIndex, _)) = new CopyRangeSide((TCertain)this, index, endIndex);
			if (intIndex == endIntIndex)
			{
				high[intIndex].ClearInternal(restIndex, length);
				return;
			}
			var previousPart = high[intIndex].Length - restIndex;
			high[intIndex].ClearInternal(restIndex, previousPart);
			for (var i = intIndex + 1; i < endIntIndex; i++)
			{
				previousPart += high[i].Length;
				high[i].ClearInternal(0, high[i].Length);
			}
			high[endIntIndex].ClearInternal(0, length - previousPart);
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected void ConstructFromCapacity(MpzT capacity)
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
			var intIndex = capacity.Divide(fragment, out var restIndex);
			var highCount = (int)GetArrayLength(capacity, fragment);
			high = new(highCount);
			highLength = SubbranchesBitLength >= 6 ? [] : null;
			for (MpzT i = 0; i < intIndex; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			if (restIndex != 0)
			{
				high.Add(CapacityCreator(restIndex));
				high[^1].parent = this2;
				AddCapacity(restIndex);
			}
		}
		Length = 0;
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected void ConstructFromEnumerable(G.IEnumerable<T> collection)
	{
		if (collection is G.IReadOnlyList<T> list)
			ConstructFromList(list);
		else if (collection is G.IList<T> list2)
			ConstructFromList(list2.GetSlice());
		else if (collection is BigList<T, TCertain, TLow> bigList)
		{
			if (bigList.low != null)
				ConstructFromList(bigList.low);
			else if (bigList.high != null)
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

	protected void ConstructFromList(G.IReadOnlyList<T> list)
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
				low.AddRange(list);
			Length = list.Count;
		}
		else
		{
			Debug.Assert(low == null && high != null && fragment != 1);
			var fragment2 = (int)fragment;
			var i = 0;
			var index = 0;
			for (; index <= list.Count - fragment2; index += fragment2)
			{
				high[i++].ConstructFromList(list.GetROLSlice(index, fragment2));
				highLength?.Add(fragment);
			}
			var rest = list.Count - index;
			Debug.Assert(rest < fragment);
			if (rest != 0)
			{
				high[i].ConstructFromList(list.GetROLSlice(index));
				highLength?.Add(rest);
			}
		}
		AddCapacity(Length - _capacity);
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
			high = new(GetArrayLength(list.Count, fragment2));
			highLength = SubbranchesBitLength >= 6 ? [] : null;
			var index = 0;
			for (; index <= list.Count - fragment2; index += fragment2)
			{
				high.Add(CapacityCreator(0));
				high[^1].parent = (TCertain)this;
				high[^1].ConstructFromListFromScratch(list.GetROLSlice(index, fragment2));
				highLength?.Add(fragment);
			}
			if (list.Count % fragment2 != 0)
			{
				var rest = list.Count - index;
				high.Add(CapacityCreator(0));
				high[^1].parent = (TCertain)this;
				high[^1].ConstructFromListFromScratch(list.GetROLSlice(index));
				highLength?.Add(rest);
			}
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected static void CheckParams(MoveRangeContext context)
	{
		var (source, sourceIndex, destination, destinationIndex, length, mode) = context;
		if ((mode & MoveRangeMode.SpecialMode) != 0)
			return;
		if (source == null && sourceIndex < 0 && destinationIndex < 0)
			return;
		if (source == null)
			throw new ArgumentNullException(nameof(context), "Исходный массив не может быть нулевым.");
		if (source.Capacity == 0 && !(sourceIndex == 0 && length == 0))
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(context));
		if (destination == null)
			throw new ArgumentNullException(nameof(context), "Целевой массив не может быть нулевым.");
		if (destination.Capacity == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(context));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Исходный индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Целевой индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Длина не может быть отрицательной.");
		if (sourceIndex + length > source.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destination.Capacity)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
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
				MoveRange((null!, -1, destination, -1, destination.Length - destinationIndex - length, MoveRangeMode.Copy));
			destinationIndex = destination.Length - length - destinationIndex;
		}
		MoveRange(((TCertain)this, sourceIndex, destination, destinationIndex, length, MoveRangeMode.Copy));
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
		else if (high != null)
		{
			var endIndex = index + length - 1;
			var ((_, intIndex, restIndex), (_, endIntIndex, endRestIndex)) = new CopyRangeSide((TCertain)this, index, endIndex);
			if (endIntIndex == intIndex)
			{
				high[intIndex].CopyToInternal(restIndex, list, listIndex, length);
				return;
			}
			high[intIndex].CopyToInternal(restIndex, list, listIndex, fragment - restIndex);
			var destIndex = listIndex + fragment - restIndex;
			for (var i = intIndex + 1; i < endIntIndex; i++, destIndex += fragment)
				high[i].CopyToInternal(0, list, destIndex, fragment);
			high[endIntIndex].CopyToInternal(0, list, destIndex, endRestIndex + 1);
		}
		else
			throw new InvalidOperationException("Невозможно скопировать диапазон. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	protected override void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex + i] = GetInternal(index + i);
	}

	public override void Dispose()
	{
		low?.Dispose();
		low = null;
		if (high != null)
		{
			foreach (var x in high)
				x.Dispose();
			high.Dispose();
		}
		high = null;
		highLength?.Dispose();
		highLength = null;
		parent = null;
		_capacity = 0;
		fragment = 1;
		Length = 0;
		Root.accessCache?.Clear();
		bReversed = false;
		GC.SuppressFinalize(this);
	}

	protected virtual int EffectiveHighLength()
	{
		if (highLength != null)
			return highLength.Length;
		Debug.Assert(high != null);
		if (MpzCmpSi(high[^1].Length, 0) != 0)
			return high.Length;
		var lo = 0;
		var hi = high.Length - 1;
		while (lo <= hi)
		{
			var mid = lo + (hi - lo) / 2;
			if (MpzCmpSi(high[mid].Length, 0) == 0)
				hi = mid - 1;
			else
				lo = mid + 1;
		}
		return lo;
	}

	protected override T GetInternal(MpzT index, bool invoke = true)
	{
		if (low != null)
			return low[(int)(bReversed ? Length - 1 - index : index)];
		else if (GetProcessAccessCache(Root, index, invoke, out var result))
			return result;
		else
			throw new InvalidOperationException("Невозможно получить элемент. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	protected virtual MpzT GetLeftValuesSum(int index, out MpzT actualValue)
	{
		if (highLength != null)
			return highLength.GetLeftValuesSum(index, out actualValue);
		Debug.Assert(high != null);
		MpzT sum = 0;
		var i = 0;
		for (; i < index && i < high.Length; i++)
			sum += high[i].Length;
		actualValue = i == high.Length ? 0 : high[i].Length;
		return sum;
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
			if (branch.high == null)
			{
				result = default;
				return false;
			}
			reducedIndex = branch.IsReversed ? start + branch.Length - 1 - index : index - start;
			var intIndex = branch.IndexOfNotGreaterSum(reducedIndex, out var restIndex);
			root.accessCache.Push((branch.high[intIndex], start + (branch.IsReversed
				? branch.Length - (reducedIndex - restIndex + branch.high[intIndex].Length) : reducedIndex - restIndex)));
			(branch, start) = root.accessCache.Peek();
		}
		reducedIndex = (branch.parent?.IsReversed ?? false) ? start + branch.Length - 1 - index : index - start;
		Debug.Assert(reducedIndex >= 0 && reducedIndex < branch.Length);
		result = branch.low[(int)(branch.bReversed ? branch.Length - 1 - reducedIndex : reducedIndex), invoke];
		return true;
	}

	protected override void GetRangeCopyTo(MpzT index, MpzT length, TCertain list) =>
		MoveRange(ProcessReverseContext(((TCertain)this, index, list, 0, length, MoveRangeMode.Copy),
			processDestination: false));

	protected virtual bool HasMaxHighLength()
	{
		if (highLength != null)
			return highLength.Length == Subbranches;
		Debug.Assert(high != null);
		return high.Length == Subbranches && high[^1].Length != 0;
	}

	protected virtual bool HasSufficientLength(int length)
	{
		if (highLength != null)
			return highLength.Length > length;
		Debug.Assert(high != null);
		return high.Length > length && high[length].Length != 0;
	}

	protected virtual void IncreaseCapacity(MpzT value, MpzT targetFragment)
	{
		if (value == _capacity)
			return;
		var this2 = (TCertain)this;
		Root.accessCache?.Clear();
		if (low != null)
		{
			IncreaseLowCapacity(value, targetFragment);
			return;
		}
		Debug.Assert(high != null);
		var newFragment = fragment << SubbranchesBitLength;
		var value2 = RedStarLinqMath.Min(value, newFragment);
		if (_capacity < newFragment)
		{
			var targetFragment2 = fragment == LeafSize ? 1 : fragment >> SubbranchesBitLength;
			if (GetArrayLength(value2, fragment) == GetArrayLength(_capacity, fragment))
				high[^1].IncreaseCapacity(value2 % fragment == 0 ? fragment : value2 % fragment, targetFragment2);
			else if (_capacity < fragment * high.Length)
			{
				high[^1].IncreaseCapacity(fragment, targetFragment2);
				if (_capacity != value2)
					AddBranches();
			}
			else
				AddBranches();
			void AddBranches()
			{
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
		}
		if (_capacity == value)
		{
#if VERIFY
			if (high != null)
				Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
			high = new(highCount) { CapacityCreator(0) };
			var first = high[0];
			first.fragment = fragment;
			fragment = newFragment;
			first.low = null;
			first.high = oldHigh;
			first.highLength = oldHighLength;
			first.Length = oldHighLength?.ValuesSum ?? oldHigh.Sum(x => x.Length);
			first._capacity = _capacity;
			Debug.Assert(first.high != null);
			first.parent = this2;
			highLength = SubbranchesBitLength >= 6 ? [Length] : null;
			foreach (var x in oldHigh)
				x.parent = first;
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
				if (high != null)
					Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
				Verify();
#endif
				return;
			}
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void IncreaseLowCapacity(MpzT value, MpzT targetFragment)
	{
		if (value == _capacity)
			return;
		var this2 = (TCertain)this;
		Root.accessCache?.Clear();
		Debug.Assert(low != null);
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
		{
			first.highLength = SubbranchesBitLength >= 6 ? [preservedLow.Length] : null;
			first.high[0]._capacity = RedStarLinqMath.Min(first.fragment, value);
		}
		ArgumentNullException.ThrowIfNull(first.low);
		// Вставляем первый лист
		first.low.AddRange(preservedLow);
		if (preservedLow.Length != 0)
			first.Length = preservedLow.Length;
		AddCapacity(value - _capacity);
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
		return;
	}

	protected override MpzT IndexOfInternal(T item, MpzT index, MpzT length, bool fromEnd)
	{
		if (length == 0)
			return -1;
		if (low != null)
			return IndexOfTrivial(item, index, length, fromEnd);
		else if (high == null)
			throw new InvalidOperationException("Невозможно найти элемент. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
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
			var ((_, intIndex, restIndex), (_, endIntIndex, endRestIndex)) = new CopyRangeSide((TCertain)this, index, endIndex);
			while (endRestIndex >= fragment)
			{
				endIntIndex++;
				endRestIndex -= fragment;
			}
			if (intIndex != 0 && !HasSufficientLength(intIndex) && restIndex == 0 && high[intIndex - 1].Length != fragment)
			{
				intIndex--;
				restIndex = LastEffectiveLength();
			}
			MpzT foundIndex;
			if (intIndex == endIntIndex)
			{
				foundIndex = high[intIndex].IndexOfInternal(item, restIndex, length, fromEnd);
				if (foundIndex >= 0)
				{
					foundIndex += index - restIndex;
					return bReversedOld ? Length - 1 - foundIndex : foundIndex;
				}
				return foundIndex;
			}
			MpzT offset;
			if (fromEnd)
			{
				offset = endIndex - endRestIndex;
				foundIndex = high[endIntIndex].IndexOfInternal(item, 0, endRestIndex + 1, true);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
				offset -= high[endIntIndex - 1].Length;
			}
			else
			{
				offset = index - restIndex;
				foundIndex = high[intIndex].IndexOfInternal(item, restIndex, high[intIndex].Length - restIndex, false);
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
				foundIndex = high[intIndex].IndexOfInternal(item, restIndex, high[intIndex].Length - restIndex, true);
				if (foundIndex >= 0)
					return bReversedOld ? Length - 1 - offset - foundIndex : offset + foundIndex;
			}
			else
			{
				foundIndex = high[endIntIndex].IndexOfInternal(item, 0, endRestIndex + 1, false);
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

	protected virtual int IndexOfNotGreaterSum(MpzT sum, out MpzT sumExceedsBy)
	{
		if (highLength != null)
			return highLength.IndexOfNotGreaterSum(sum, out sumExceedsBy);
		Debug.Assert(high != null);
		if (sum == 0)
		{
			sumExceedsBy = 0;
			return 0;
		}
		if (MpzCmp(sum << 1, Length) < 0)
		{
			sum = new(sum);
			var i = 0;
			for (; i < high.Length; i++)
			{
				var length = high[i].Length;
				if (MpzCmpSi(length, 0) == 0 || MpzCmpSi(sum, 0) == 0 || MpzCmp(sum, length) < 0)
					break;
				MpzSub(sum, sum, length);
			}
			sumExceedsBy = sum;
			return i;
		}
		else
		{
			var effectiveHighLength = EffectiveHighLength();
			if (MpzCmp(sum, Length) >= 0)
			{
				sumExceedsBy = sum - Length;
				return effectiveHighLength;
			}
			sum = Length - sum;
			var i = effectiveHighLength - 1;
			for (; i >= 0; i--)
			{
				var length = high[i].Length;
				if (MpzCmp(sum, length) <= 0)
					break;
				MpzSub(sum, sum, length);
			}
			sumExceedsBy = high[i].Length - sum;
			return i;
		}
	}

	protected virtual void IndexOfNotGreaterSum(MpzT leftSum, MpzT rightSum,
		out CopyRangeIndex leftIndexes, out CopyRangeIndex rightIndexes)
	{
		if (highLength != null)
		{
			var leftIntIndex = highLength.IndexOfNotGreaterSum(leftSum, out var leftRestIndex);
			var rightIntIndex = highLength.IndexOfNotGreaterSum(rightSum, out var rightRestIndex);
			leftIndexes = new(leftSum, leftIntIndex, leftRestIndex);
			rightIndexes = new(rightSum, rightIntIndex, rightRestIndex);
			return;
		}
		Debug.Assert(high != null);
		var oldLeftSum = leftSum;
		var oldRightSum = rightSum;
		rightSum = new(rightSum);
		var i = 0;
		if (MpzCmpSi(leftSum, 0) == 0)
		{
			leftIndexes = new(leftSum, 0, MpzT.Zero);
			if (MpzCmpSi(rightSum, 0) == 0)
			{
				rightIndexes = new(rightSum, 0, MpzT.Zero);
				return;
			}
			if (MpzCmp(rightSum << 1, Length - leftSum) < 0)
				RightForwardLoop();
			else
				RightReverseLoop(EffectiveHighLength());
			rightIndexes = new(oldRightSum, i, rightSum);
			return;
		}
		leftSum = new(leftSum);
		if (MpzCmp(rightSum, Length) >= 0)
		{
			MpzSub(rightSum, rightSum, Length);
			var effectiveHighLength = EffectiveHighLength();
			rightIndexes = new(rightSum, effectiveHighLength, rightSum);
			if (MpzCmp(leftSum, Length) >= 0)
			{
				leftIndexes = new(leftSum, effectiveHighLength, leftSum - Length);
				return;
			}
			if (MpzCmp(leftSum << 1, Length) < 0)
				LeftForwardLoop();
			else
			{
				i = effectiveHighLength;
				LeftReverseLoop();
			}
			leftIndexes = new(oldLeftSum, i, leftSum);
			return;
		}
		if (MpzCmp(leftSum << 1, Length) < 0)
		{
			LeftForwardLoop();
			leftIndexes = new(oldLeftSum, i, leftSum);
			if (MpzCmp((rightSum - oldLeftSum) << 1, Length - oldLeftSum) < 0)
			{
				MpzSub(rightSum, rightSum, oldLeftSum);
				MpzAdd(rightSum, rightSum, leftSum);
				RightForwardLoop();
			}
			else
				RightReverseLoop(EffectiveHighLength());
			rightIndexes = new(oldRightSum, i, rightSum);
			return;
		}
		else
		{
			RightReverseLoop(EffectiveHighLength());
			rightIndexes = new(oldRightSum, i, rightSum);
			LeftReverseLoop();
			leftIndexes = new(oldLeftSum, i, leftSum);
		}
		void LeftForwardLoop()
		{
			for (; i < high.Length; i++)
			{
				var length = high[i].Length;
				if (MpzCmpSi(length, 0) == 0 || MpzCmpSi(leftSum, 0) == 0 || MpzCmp(leftSum, length) < 0)
					break;
				MpzSub(leftSum, leftSum, length);
			}
		}
		void LeftReverseLoop()
		{
			var leftLength = oldRightSum - rightSum;
			for (i--; i >= 0; i--)
			{
				var length = high[i].Length;
				if (MpzCmp(leftLength, leftSum) <= 0)
					break;
				MpzSub(leftLength, leftLength, length);
			}
			MpzSub(leftSum, leftSum, leftLength);
			i++;
		}
		void RightForwardLoop()
		{
			for (; i < high.Length; i++)
			{
				var length = high[i].Length;
				if (MpzCmpSi(length, 0) == 0 || MpzCmpSi(rightSum, 0) == 0 || MpzCmp(rightSum, length) < 0)
					break;
				MpzSub(rightSum, rightSum, length);
			}
		}
		void RightReverseLoop(int effectiveHighLength)
		{
			var leftIntIndex = i;
			MpzT leftLength = new(Length);
			for (i = effectiveHighLength - 1; i >= leftIntIndex; i--)
			{
				var length = high[i].Length;
				if (MpzCmp(leftLength, rightSum) <= 0)
					break;
				MpzSub(leftLength, leftLength, length);
			}
			MpzSub(rightSum, rightSum, leftLength);
			i++;
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
		if (high == null)
			throw new InvalidOperationException("Невозможно вставить элемент. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		if (bReversed)
		{
			Reverse();
			index = Length - index;
		}
		while (true)
		{
			var intIndex = IndexOfNotGreaterSum(index, out var restIndex);
			if (intIndex != 0 && (!HasSufficientLength(intIndex) || restIndex == high[intIndex].Length)
				&& high[intIndex - 1].Capacity != high[intIndex - 1].Length)
				restIndex = high[--intIndex].Length;
			if (high.Length == intIndex)
			{
				if (high.Length >= Subbranches)
				{
					Debug.Assert(!compactified);
					MoveRange((null!, -2, (TCertain)this, -1, -Length - 1, MoveRangeMode.None));
					compactified = true;
					continue;
				}
				else if (high[^1].Capacity != fragment)
				{
					high[^1].IncreaseCapacity(fragment, high[0].fragment);
					continue;
				}
				else
				{
					high.Capacity = Min(high.Capacity << 1, Subbranches);
					high.Add(CapacityCreator(fragment));
					high[^1].parent = this2;
					AddCapacity(fragment);
				}
			}
			if (Length > fragment * Subbranches - 1)
			{
				Debug.Assert(parent == null);
				Capacity <<= 1;
				continue;
			}
			if (!(HasSufficientLength(intIndex) && (high[intIndex].low != null && high[intIndex].Length == LeafSize
				|| high[intIndex].high != null && high[intIndex].HasMaxHighLength())))
			{
				high[intIndex].InsertInternal(restIndex, item);
				if (!HasSufficientLength(intIndex))
					highLength?.Add(1);
				else
					highLength?[intIndex] += 1;
				break;
			}
			if (HasMaxHighLength())
			{
				if (parent == null && Length << GetArrayLength((Length - 1).BitLength - LeafSizeBitLength,
					SubbranchesBitLength) >= fragment << SubbranchesBitLength)
				{
					IncreaseCapacity(Capacity << 1, fragment << SubbranchesBitLength);
					continue;
				}
				MoveRange((this2, index, this2, index + 1, Length - index, MoveRangeMode.Copy));
				if (index == Length)
				{
					MoveRange((null!, -2, high[^1], -1, -high[^1].Length - 1, MoveRangeMode.None));
					high[^1].InsertInternal(high[^1].Length, item);
					highLength?[^1] += 1;
				}
				else
					SetInternal(index, item);
				break;
			}
			if (intIndex == high.Length - 1)
				high[intIndex].IncreaseCapacity(fragment, high[0].fragment);
			var temp = high[^1];
			if (temp.Length != 0)
			{
				high.Capacity = Max(high.Capacity, Subbranches);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
				temp = high[^1];
			}
			temp.IncreaseCapacity(fragment, high[0].fragment);
			high.CopyRangeTo(intIndex + 1, high, intIndex + 2, high.Length - intIndex - 2);
			if (restIndex == 0)
			{
				high[intIndex + 1] = high[intIndex];
				highLength?.Insert(intIndex + 1, high[intIndex].Length);
				high[intIndex] = temp;
				temp.ClearInternal(false);
			}
			else
			{
				high[intIndex + 1] = temp;
				MoveRange(ProcessReverseContext((high[intIndex], restIndex, temp, 0,
					high[intIndex].Length - restIndex, MoveRangeMode.None), processDestination: false));
				temp.parent = this2;
				highLength?.Insert(intIndex + 1, temp.Length);
			}
			high[intIndex].InsertInternal(restIndex, item);
			highLength?[intIndex] = restIndex + 1;
			break;
		}
		if (bReversedOld)
			Reverse();
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override void InsertInternal(MpzT index, TCertain bigList, bool saveOriginal)
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
		else if (high == null)
			throw new InvalidOperationException("Невозможно вставить диапазон. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
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
		while (true)
		{
			var ((_, intIndex, restIndex), (_, endIntIndex, endRestIndex)) = new CopyRangeSide(this2, index, endIndex);
			while (endRestIndex >= fragment)
			{
				endIntIndex++;
				endRestIndex -= fragment;
			}
			if (intIndex != 0 && !HasSufficientLength(intIndex) && restIndex == 0 && high[intIndex - 1].Length != fragment)
			{
				intIndex--;
				restIndex = LastEffectiveLength();
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
					MoveRange((null!, -2, (TCertain)this, -1, -Length - length, MoveRangeMode.None));
					compactified = true;
					continue;
				}
			}
			if (intIndex == endIntIndex && high[intIndex].Length + length <= fragment)
			{
				high[intIndex].InsertInternal(restIndex, bigList, saveOriginal);
				if (!HasSufficientLength(intIndex))
					highLength?.Add(length);
				else
					highLength?[intIndex] += length;
			}
			else if (index == Length)
			{
				if (saveOriginal)
					MoveRange((bigList, 0, this2, Length, length, MoveRangeMode.Copy));
				else
					MoveRange((bigList, 0, this2, Length, length, MoveRangeMode.None));
			}
			else
				InsertInternal(bigList, (index, intIndex, restIndex), saveOriginal);
			if (bReversedOld)
			{
				Reverse();
				bigList.Reverse();
				(index, endIndex) = (Length + length - 1 - endIndex, Length + length - 1 - index);
			}
			break;
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void InsertInternal(TCertain bigList, CopyRangeIndex copyRange, bool saveOriginal)
	{
		var this2 = (TCertain)this;
		var length = bigList.Length;
		var (index, intIndex, restIndex) = copyRange;
		Debug.Assert(high != null);
		var oldLength = Length;
		var targetLength = length;
		var shiftedIntIndex = intIndex;
		var intIndexSum = GetLeftValuesSum(intIndex, out _);
		high.Capacity = Min(Min(high.Capacity + (int)GetArrayLength(length, fragment), high.Capacity << 1), Subbranches);
		while (high.Length < Subbranches && targetLength >= fragment)
		{
			high.Insert(intIndex, CapacityCreator(fragment));
			high[intIndex].parent = this2;
			AddCapacity(fragment);
			MoveRange((null!, -1, high[intIndex], -1, high[intIndex].Length - fragment, MoveRangeMode.Copy));
			highLength?.Insert(intIndex, fragment);
			shiftedIntIndex++;
			targetLength -= fragment;
		}
		while (!HasMaxHighLength() && targetLength >= fragment)
		{
			var effectiveHighLength = EffectiveHighLength();
			var buffer = high[effectiveHighLength];
			buffer.IncreaseCapacity(fragment, high[0].fragment);
			high.CopyRangeTo(intIndex, high, intIndex + 1, effectiveHighLength - intIndex);
			high[intIndex] = buffer;
			MoveRange((null!, -1, high[intIndex], -1, -fragment, MoveRangeMode.Copy));
			high[intIndex].bReversed = false;
			highLength?.Insert(intIndex, fragment);
			shiftedIntIndex++;
			targetLength -= fragment;
		}
		if (targetLength != 0)
		{
			if (high.Length < Subbranches)
			{
				high.Insert(intIndex, CapacityCreator(fragment));
				high[intIndex].parent = this2;
				AddCapacity(fragment);
				MoveRange((null!, -1, high[intIndex], -1, high[intIndex].Length - targetLength, MoveRangeMode.Copy));
				highLength?.Insert(intIndex, targetLength);
				shiftedIntIndex++;
				targetLength = 0;
			}
			else if (!HasMaxHighLength())
			{
				var effectiveHighLength = EffectiveHighLength();
				var buffer = high[effectiveHighLength];
				buffer.IncreaseCapacity(fragment, high[0].fragment);
				high.CopyRangeTo(intIndex, high, intIndex + 1, effectiveHighLength - intIndex);
				high[intIndex] = buffer;
				MoveRange((null!, -1, high[intIndex], -1, high[intIndex].Length - targetLength, MoveRangeMode.Copy));
				high[intIndex].bReversed = false;
				highLength?.Insert(intIndex, targetLength);
				shiftedIntIndex++;
				targetLength = 0;
			}
		}
		var shiftedIntIndexSum = GetLeftValuesSum(shiftedIntIndex, out _);
		var bResized = false;
		if (index + length > Length)
		{
			MoveRange((null!, -1, this2, -1, Length - index - length, MoveRangeMode.Copy));
			bResized = true;
		}
		if (intIndex != shiftedIntIndex)
		{
			if (bResized)
				MoveRange((this2, shiftedIntIndexSum, this2, intIndexSum, restIndex, MoveRangeMode.Copy));
			else
			{
				var copyLength = RedStarLinqMath.Min(restIndex, high[intIndex].Length);
				MoveRange((high[shiftedIntIndex], ProcessReverse(high[shiftedIntIndex], 0, copyLength),
					high[intIndex], 0, copyLength, MoveRangeMode.Copy));
				MoveRange((high[shiftedIntIndex], ProcessReverse(high[shiftedIntIndex], copyLength, restIndex - copyLength),
					high[intIndex + 1], ProcessReverse(high[intIndex + 1], 0, restIndex - copyLength),
					restIndex - copyLength, MoveRangeMode.Copy));
			}
		}
		if (targetLength != 0)
			MoveRange((this2, index + length - targetLength, this2, index + length, oldLength - index, MoveRangeMode.Copy));
		if (saveOriginal)
			MoveRange((bigList, 0, this2, index, length, MoveRangeMode.Copy));
		else
			MoveRange((bigList, 0, this2, index, length, MoveRangeMode.None));
	}

	protected virtual MpzT LastEffectiveLength()
	{
		Debug.Assert(high != null);
		if (highLength != null)
			return highLength[^1];
		return high[EffectiveHighLength() - 1].Length;
	}

	protected static void MoveRange(MoveRangeContext context)
	{
		List<MoveRangeContext> stack;
		if (context.Destination.Root.moveStack == null)
			stack = context.Destination.Root.moveStack = new(32, context);
		else
		{
			stack = context.Destination.Root.moveStack;
			stack.Clear();
			stack.Add(context);
		}
		while (stack.Length > 0)
			MoveRangeIteration(stack);
	}

	protected static void MoveRangeIteration(List<MoveRangeContext> stack)
	{
		var context = stack.GetAndRemove(^1);
		CheckParams(context);
		var (source, sourceIndex, destination, destinationIndex, length, mode) = context;
		if ((mode & MoveRangeMode.SpecialMode) == 0 && length == 0)
			return;
		var stackIndex = stack.Length;
		if (source == null)
		{
			if (sourceIndex == -1 && destinationIndex <= -1 && destinationIndex >= -2 && length < 0)
				MoveRangeIterationResize(stack, context, stackIndex);
			else if (sourceIndex == -2 && destinationIndex < 0 && length < 0)
				MoveRangeIterationCompactify(stack, context, stackIndex);
			else
				throw new ArgumentException("Исходный массив не может быть нулевым.");
			return;
		}
		Debug.Assert(source != destination || (mode & MoveRangeMode.Copy) != 0 || (mode & MoveRangeMode.SpecialMode) != 0);
		var fragment = source.fragment;
		// Если копирование не нужно
		if ((mode & MoveRangeMode.SpecialMode) != 0)
		{
			if (sourceIndex == -1 && destinationIndex == -1 && length < 0)
				MoveRangeIterationSpecial2(stack, context, stackIndex);
			else
				MoveRangeIterationSpecial(stack, context, stackIndex);
		}
		// Тривиальный случай - оба списка являются листьями дерева
		else if (source.low != null && destination.low != null)
			MoveRangeTrivial(context);
		// Только целевой список является листом дерева
		else if (destination.low != null || destination.fragment < fragment)
			MoveRangeToLeaf(stack, context, stackIndex, fragment);
		// Исходный список является более мелкой веткой (возможно, даже листом)
		else if ((source.low != null || destination.fragment > fragment)
			&& destination.high != null)
			MoveRangeToLarger(stack, context, stackIndex);
		// Самый сложный случай: исходный список является соизмеримой или более крупной веткой,
		// а целевой также не является листом
		else
			MoveRangeToSimilar(stack, context, stackIndex);
	}

	protected static void MoveRangeIterationCompactify(List<MoveRangeContext> stack, MoveRangeContext context, int stackIndex)
	{
		var (_, _, source, sourceIndex, targetLength, mode) = context;
		var startIndex = -(int)sourceIndex;
		targetLength = -targetLength;
		source.Root.accessCache?.Clear();
		Debug.Assert(source.high != null);
		MpzT oldTargetLength = new(targetLength);
		LimitedBuffer<MpzT> estimatedLengths = new(source.high.Convert(x => x.Length).TakeWhile(x => x != 0));
		var deepCompactify = targetLength >= source.fragment * (source.high.Length - 1);
		var mergeThreshold = source.fragment - (source.high[0].high != null ? source.high[0].fragment : 0);
	start:
		if (source.Length + (estimatedLengths.Length > 0 ? source.fragment - estimatedLengths[^1] : 0)
			+ (source.high.Length - estimatedLengths.Length - 1) * source.fragment + source.high[^1].Capacity
			>= targetLength)
			return;
		targetLength -= source.high[0].Length;
		var offset = 0;
		var highOffset = 0;
		var effectiveHighLength = estimatedLengths.Length;
		for (var i = 1; i < effectiveHighLength + offset; i++)
		{
			var highLengthAtI = estimatedLengths[i - offset];
			var highLengthBeforeI = estimatedLengths[i - offset - 1];
			if (highLengthBeforeI == source.fragment)
			{
				targetLength -= highLengthAtI;
				highOffset = 0;
				continue;
			}
			if (source.Length + (estimatedLengths.Length > 0 ? source.fragment - estimatedLengths[^1] : 0)
				+ (source.high.Length - effectiveHighLength - 1) * source.fragment + source.high[^1].Capacity >= oldTargetLength)
				break;
			var amount = RedStarLinqMath.Min(source.fragment - highLengthBeforeI, targetLength);
			if (amount <= 0)
				break;
			var highAtI = source.high[i];
			var highBeforeI = source.high[i - highOffset - 1];
			if (i >= startIndex && highBeforeI.high != null && highBeforeI.Length
				+ highBeforeI.fragment - highBeforeI.LastEffectiveLength() + amount > highBeforeI.Capacity)
			{
				stack.Insert(stackIndex, (null!, -2, highBeforeI, -1, -source.fragment, mode));
				stack.Insert(stackIndex, (null!, -2, source, -i - 1, -oldTargetLength, mode));
				return;
			}
			if (highLengthAtI > amount)
			{
				if (!deepCompactify)
				{
					highOffset = 0;
					targetLength -= amount;
					continue;
				}
				if (highBeforeI.bReversed && highLengthBeforeI < highLengthBeforeI + amount)
					stack.Insert(stackIndex, (null!, -1, highBeforeI, -1, -amount, MoveRangeMode.Copy));
				stack.Insert(stackIndex, (highAtI, highAtI.bReversed ? highLengthAtI - amount : 0, highBeforeI,
					highBeforeI.bReversed ? 0 : highLengthBeforeI, amount, MoveRangeMode.None));
				estimatedLengths[i - offset - 1] += amount;
				estimatedLengths[i - offset] -= amount;
				source.highLength?[i - offset - 1] += amount;
				source.highLength?[i - offset] -= amount;
				targetLength -= amount;
				highOffset = 0;
			}
			else
			{
				if (!deepCompactify && highLengthBeforeI + highLengthAtI > mergeThreshold)
				{
					highOffset = 0;
					targetLength -= highLengthAtI;
					continue;
				}
				if (highBeforeI.bReversed)
					stack.Insert(stackIndex, (null!, -1, highBeforeI, -1, -highLengthAtI, MoveRangeMode.Copy));
				stack.Insert(stackIndex, (source, (i - offset) * (1L << 30) + (i - offset - 1), source, 0,
					highBeforeI.bReversed ? 0 : highLengthBeforeI, MoveRangeMode.SpecialMode));
				estimatedLengths[i - offset - 1] += highLengthAtI;
				source.highLength?[i - offset - 1] += highLengthAtI;
				targetLength -= highLengthAtI;
				estimatedLengths.RemoveAt(i - offset);
				source.highLength?.RemoveAt(i - offset);
				effectiveHighLength--;
				offset++;
				highOffset++;
			}
		}
		if (targetLength > 0 && !deepCompactify)
		{
			if (stackIndex == stack.Length)
			{
				targetLength = oldTargetLength;
				deepCompactify = true;
				goto start;
			}
			else
				stack.Insert(stackIndex, (null!, -2, source, -1, -oldTargetLength, mode));
		}
#if VERIFY
		if (source.high != null)
			Debug.Assert((source.highLength == null || source.Length == source.highLength?.ValuesSum)
				&& source.Length == source.high.Sum(x => x.Length));
		source.Verify();
#endif
	}

	protected static void MoveRangeIterationResize(List<MoveRangeContext> stack, MoveRangeContext context, int stackIndex)
	{
		var source = context.Destination;
		var increment = -context.Length;
		var inverted = context.DestinationIndex == -2;
		if (source.low != null)
		{
			if (!source.bReversed || source.low.Length == 0)
				source.low.Resize((int)(source.Length + increment));
			else
				source.low.ResizeLeft((int)(source.Length + increment));
			source.Length = source.low.Length;
			return;
		}
		else if (source.high == null)
			throw new InvalidOperationException("Невозможно переместить диапазон. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {source.Length}, подветок - {source.high?.Length ?? 0},"
				+ $" реверс - {source.bReversed}, емкость - {source.Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		if (source.bReversed ^ inverted && increment <= source.Length)
		{
			stack.Insert(stackIndex, (source, 0, source, increment, source.Length, MoveRangeMode.Copy));
			return;
		}
		var sourceEffectiveHighLength = source.EffectiveHighLength();
		if (source.high != null && source.HasSufficientLength(0)
			&& (sourceEffectiveHighLength - 1) * source.fragment + source.LastEffectiveLength() + increment
			> source.fragment << source.SubbranchesBitLength)
		{
			stack.Insert(stackIndex, (null!, -2, source, -1, -source.Length - increment, MoveRangeMode.Copy));
			stack.Insert(stackIndex, context);
			return;
		}
		Debug.Assert(source.high != null);
		sourceEffectiveHighLength = source.EffectiveHighLength();
		var targetCapacity = (source.HasSufficientLength(0) ? (sourceEffectiveHighLength - 1)
			* source.fragment + source.LastEffectiveLength() : 0) + increment;
		if (source.Capacity < targetCapacity)
			source.IncreaseCapacity(RedStarLinqMath.Min(RedStarLinqMath.Max(source.Length << 1, targetCapacity),
				source.fragment << source.SubbranchesBitLength), source.fragment);
		sourceEffectiveHighLength = source.EffectiveHighLength();
		var leftIncrement = increment;
		if (source.HasSufficientLength(0) && source.LastEffectiveLength() != source.fragment)
		{
			var leftToFragment = RedStarLinqMath.Min(source.fragment
				- source.high[sourceEffectiveHighLength - 1].Length, leftIncrement);
			leftIncrement -= leftToFragment;
			stack.Insert(stackIndex, (null!, -1, source.high[sourceEffectiveHighLength - 1], -1,
				-leftToFragment, MoveRangeMode.Copy));
			source.highLength?[^1] += leftToFragment;
		}
		var i = sourceEffectiveHighLength;
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
			stack.Insert(stackIndex, (null!, -1, source.high[i], -1, -source.fragment, MoveRangeMode.Copy));
			source.highLength?.Add(source.fragment);
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
			stack.Insert(stackIndex, (null!, -1, source.high[i], -1, -leftIncrement, MoveRangeMode.Copy));
			source.highLength?.Add(leftIncrement);
		}
		if (source.bReversed ^ inverted)
			stack.Insert(stackIndex, (source, 0, source, increment, source.Length, MoveRangeMode.Copy));
#if VERIFY
		source.Verify();
#endif
	}

	protected static void MoveRangeIterationSpecial(List<MoveRangeContext> stack, MoveRangeContext context, int stackIndex)
	{
		var (source, indexes, destination, step, destinationCurrentRestIndex, mode) = context;
		Debug.Assert(source.high != null && destination.high != null);
		Debug.Assert((mode & MoveRangeMode.Copy) == 0);
		var sourceIndex = indexes.Divide(1 << 30, out int iDestination);
		var iSource = (int)sourceIndex;
		var currentSource = source.high[iSource];
		var currentDestination = destination.high[iDestination];
		if (step != 0)
		{
			stack.Insert(stackIndex, (currentSource,
				currentSource.IsReversed ^ destination.IsReversed ? currentSource.Length - step : 0,
				currentDestination, destinationCurrentRestIndex, step, MoveRangeMode.None));
		}
		else if ((currentSource.Length != currentDestination.Length || destinationCurrentRestIndex > 0)
			&& currentDestination.Length != 0)
		{
			stack.Insert(stackIndex, (currentSource, 0, currentDestination,
				destinationCurrentRestIndex, currentSource.Length, MoveRangeMode.None));
			stack.Insert(stackIndex, (source, -1, destination, -1, ~iSource, MoveRangeMode.SpecialMode));
		}
		else if (destinationCurrentRestIndex < 0)
		{
			var destinationIntLength = -(int)destinationCurrentRestIndex;
			if (source.IsReversed != destination.IsReversed)
				for (var i = iSource; i < iSource + destinationIntLength; i++)
					source.high[i].Reverse();
			source.high[^1].IncreaseCapacity(source.fragment,
				source.fragment >> source.SubbranchesBitLength < source.LeafSize
				? 1 : source.fragment >> source.SubbranchesBitLength);
			destination.high[^1].IncreaseCapacity(destination.fragment,
				destination.fragment >> destination.SubbranchesBitLength < destination.LeafSize
				? 1 : destination.fragment >> destination.SubbranchesBitLength);
			for (var i = iSource; i < iSource + destinationIntLength; i++)
				destination.Length += source.high[i].Length;
			for (var i = iDestination; i < iDestination + destinationIntLength; i++)
				destination.high[i].Dispose();
			source.high.CopyRangeTo(iSource, destination.high, iDestination, destinationIntLength);
			for (var i = iDestination; i < iDestination + destinationIntLength; i++)
				destination.highLength?.SetOrAdd(i, destination.high[i].Length);
			for (var i = iSource; i < iSource + destinationIntLength; i++)
			{
				if (source.parent == null && source.high.Length - i + iSource != 1)
					source.AddCapacity(-source.high[iSource].Capacity);
				source.Length -= source.high[i].Length;
				source.high[i].parent = destination;
			}
			source.high.Remove(iSource, destinationIntLength);
			for (var i = 0; i < destinationIntLength; i++)
				if (source.parent != null || source.high.Length == 0)
				{
					source.high.Add(source.CapacityCreator(source.fragment));
					source.high[^1].parent = source;
				}
		}
		else
		{
			source.high[^1].IncreaseCapacity(source.fragment,
				source.fragment >> source.SubbranchesBitLength < source.LeafSize
				? 1 : source.fragment >> source.SubbranchesBitLength);
			destination.high[^1].IncreaseCapacity(destination.fragment,
				destination.fragment >> destination.SubbranchesBitLength < destination.LeafSize
				? 1 : destination.fragment >> destination.SubbranchesBitLength);
			if (currentSource.Length == 1)
				currentSource.bReversed = false;
			else
			{
				if (source.IsReversed != destination.IsReversed)
					currentSource.Reverse();
				if (currentSource.bReversed)
				{
					currentSource.low?.Reverse();
					var highEffectiveLength = currentSource.high != null ? currentSource.EffectiveHighLength() : 0;
					currentSource.high?.Reverse(0, highEffectiveLength);
					currentSource.high?.Take(highEffectiveLength).ForEach(x => x.Reverse());
					currentSource.highLength?.Reverse();
					currentSource.bReversed = false;
				}
			}
			if (currentDestination.Length == 0)
				destination.Length += currentSource.Length;
			currentDestination.Dispose();
			destination.high[iDestination] = currentSource;
			if (destination.highLength != null && destination.highLength[iDestination] < currentSource.Length)
				destination.highLength[iDestination] = currentSource.Length;
			if (source.parent == null && source.high.Length != 1)
				source.AddCapacity(-source.high[iSource].Capacity);
			source.high.RemoveAt(iSource);
			source.Length -= currentSource.Length;
			currentSource.parent = destination;
			if (source.parent != null || source.high.Length == 0)
			{
				source.high.Add(source.CapacityCreator(source.fragment));
				source.high[^1].parent = source;
			}
		}
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
	}

	protected static void MoveRangeIterationSpecial2(List<MoveRangeContext> stack, MoveRangeContext context, int stackIndex)
	{
		var (source, leftCheck, destination, rightCheck, sourceIndex, mode) = context;
		Debug.Assert(source.high != null && destination.high != null && leftCheck == -1 && rightCheck == -1);
		Debug.Assert((mode & MoveRangeMode.Copy) == 0);
		var iSource = ~(int)sourceIndex;
		var currentSource = source.high[iSource];
		var temp = source.high[iSource];
		source.high[^1].IncreaseCapacity(source.fragment,
			source.fragment >> source.SubbranchesBitLength < source.LeafSize
			? 1 : source.fragment >> source.SubbranchesBitLength);
		source.high.RemoveAt(iSource);
		source.Length -= currentSource.Length;
		temp.ClearInternal(false);
		source.high.Add(temp);
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
	}

	protected static void MoveRangeToLarger(List<MoveRangeContext> stack, MoveRangeContext context, int stackIndex)
	{
		var (source, sourceIndex, destination, destinationIndex, length, mode) = context;
		Debug.Assert((source.low != null || destination.fragment > source.fragment)
			&& destination.high != null);
		var fragment = destination.fragment;
		var endIndex = destinationIndex + length - 1;
		var (start, end) = new CopyRangeSide(destination, destinationIndex, endIndex);
		// Диапазон дописывается после конца списка
		if (start.IntIndex != 0 && !destination.HasSufficientLength(start.IntIndex)
			&& start.RestIndex == 0 && destination.LastEffectiveLength() != fragment)
			start = (start.Index, start.IntIndex - 1, destination.LastEffectiveLength());
		// Диапазон частично расположен за концом списка
		if (end.IntIndex != 0 && !destination.HasSufficientLength(end.IntIndex))
		{
			if (end.RestIndex >= fragment - destination.LastEffectiveLength())
				end = (end.Index, end.IntIndex, end.RestIndex - (fragment - destination.LastEffectiveLength()));
			else
				end = (end.Index, end.IntIndex - 1, end.RestIndex + destination.LastEffectiveLength());
		}
		while (end.RestIndex >= fragment)
			end = (end.Index, end.IntIndex + 1, end.RestIndex - fragment);
		if (end.IntIndex >= destination.high.Length || destination.HasSufficientLength(0)
			&& destination.Length + fragment - destination.LastEffectiveLength()
			+ (destination.high.Length - destination.EffectiveHighLength() - 1) * fragment
			+ destination.high[^1].Capacity < destinationIndex + length)
		{
			stack.Insert(stackIndex, (null!, -2, destination, -1, -destinationIndex - length, MoveRangeMode.Copy));
			stack.Insert(stackIndex, context);
			return;
		}
		// Весь диапазон помещается в одну ветку
		if (start.IntIndex == end.IntIndex)
		{
			var newSize = start.RestIndex + length;
			var oldSize = destination.high[start.IntIndex].Length;
			if (newSize < oldSize)
				newSize = oldSize;
			if (newSize != oldSize)
				stack.Insert(stackIndex, (null!, -1, destination.high[start.IntIndex], -1, oldSize - newSize,
					mode & ~MoveRangeMode.SpecialMode));
			stack.Insert(stackIndex, (source, sourceIndex,
				destination.high[start.IntIndex], ProcessReverse(destination.high[start.IntIndex],
				start.RestIndex, length, newSize), length, mode & ~MoveRangeMode.SpecialMode));
			if (destination.HasSufficientLength(start.IntIndex))
				destination.highLength?.UpdateIfGreater(start.IntIndex, newSize);
			else
				destination.highLength?.SetOrAdd(start.IntIndex, newSize);
		}
		else if (source.IsReversed != destination.IsReversed)
			MoveRangeToLargerDiagonal(stack, context, stackIndex, fragment, (start, end));
		else
			MoveRangeToLargerDirect(stack, context, stackIndex, fragment, (start, end));
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
	}

	protected static void MoveRangeToLargerDiagonal(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		var (source, sourceIndex, destination, _, length, mode) = context;
		Debug.Assert((source.low != null || destination.fragment > source.fragment)
			&& destination.high != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var previousPart = RedStarLinqMath.Min((destination.HasSufficientLength(indexes.Start.IntIndex + 1)
			? destination.high[indexes.Start.IntIndex].Length : fragment) - indexes.Start.RestIndex, length);
		var newSize = indexes.Start.RestIndex + previousPart;
		var oldSize = destination.high[indexes.Start.IntIndex].Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, destination.high[indexes.Start.IntIndex], -1,
				oldSize - newSize, mode & ~MoveRangeMode.SpecialMode));
		stack.Insert(stackIndex, (source, sourceIndex + length - previousPart, destination.high[indexes.Start.IntIndex],
			ProcessReverse(destination.high[indexes.Start.IntIndex], indexes.Start.RestIndex, previousPart, newSize),
			previousPart, mode & ~MoveRangeMode.SpecialMode));
		if (destination.HasSufficientLength(indexes.Start.IntIndex + 1))
			destination.highLength?.UpdateIfGreater(indexes.Start.IntIndex, indexes.Start.RestIndex + previousPart);
		else
			destination.highLength?.SetOrAdd(indexes.Start.IntIndex, indexes.Start.RestIndex + previousPart);
		for (var i = indexes.Start.IntIndex + 1; i < indexes.End.IntIndex; i++)
		{
			var branchLength = destination.HasSufficientLength(i + 1) ? destination.high[i].Length : fragment;
			previousPart += branchLength;
			destination.high[i].bReversed = false;
			stack.Insert(stackIndex, (source, sourceIndex + length - previousPart,
				destination.high[i], 0, branchLength, mode & ~MoveRangeMode.SpecialMode));
			destination.highLength?.SetOrAdd(i, branchLength);
		}
		stack.Insert(stackIndex, (source, sourceIndex,
			destination.high[indexes.End.IntIndex], ProcessReverse(destination.high[indexes.End.IntIndex], 0,
			length - previousPart, length - previousPart), length - previousPart, mode & ~MoveRangeMode.SpecialMode));
		if (destination.HasSufficientLength(indexes.End.IntIndex))
			destination.highLength?.UpdateIfGreater(indexes.End.IntIndex, length - previousPart);
		else
			destination.highLength?.SetOrAdd(indexes.End.IntIndex, length - previousPart);
	}

	protected static void MoveRangeToLargerDirect(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		var (source, sourceIndex, destination, _, length, mode) = context;
		Debug.Assert((source.low != null || destination.fragment > source.fragment)
			&& destination.high != null);
		// Диапазон копируется слева направо
		var previousPart = RedStarLinqMath.Min((destination.HasSufficientLength(indexes.Start.IntIndex + 1)
			? destination.high[indexes.Start.IntIndex].Length : fragment) - indexes.Start.RestIndex, length);
		var newSize = indexes.Start.RestIndex + previousPart;
		var oldSize = destination.high[indexes.Start.IntIndex].Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, destination.high[indexes.Start.IntIndex], -1,
				oldSize - newSize, mode & ~MoveRangeMode.SpecialMode));
		stack.Insert(stackIndex, (source, sourceIndex, destination.high[indexes.Start.IntIndex],
			ProcessReverse(destination.high[indexes.Start.IntIndex],
			indexes.Start.RestIndex, previousPart, newSize), previousPart, mode & ~MoveRangeMode.SpecialMode));
		destination.highLength?.SetOrAdd(indexes.Start.IntIndex,
			RedStarLinqMath.Max(destination.highLength.Length
			> indexes.Start.IntIndex ? destination.high[indexes.Start.IntIndex].Length : 0,
			indexes.Start.RestIndex + previousPart));
		for (var i = indexes.Start.IntIndex + 1; i < indexes.End.IntIndex; i++)
		{
			var branchLength = destination.HasSufficientLength(i + 1) ? destination.high[i].Length : fragment;
			stack.Insert(stackIndex, (source, sourceIndex + ((mode & MoveRangeMode.Copy) == 0 ? 0 : previousPart),
				destination.high[i], 0, branchLength, mode & ~MoveRangeMode.SpecialMode));
			destination.highLength?.SetOrAdd(i, branchLength);
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (source, sourceIndex + ((mode & MoveRangeMode.Copy) == 0 ? 0 : previousPart),
			destination.high[indexes.End.IntIndex], ProcessReverse(destination.high[indexes.End.IntIndex], 0,
			length - previousPart, length - previousPart), length - previousPart, mode & ~MoveRangeMode.SpecialMode));
		if (destination.HasSufficientLength(indexes.End.IntIndex))
			destination.highLength?.UpdateIfGreater(indexes.End.IntIndex, length - previousPart);
		else
			destination.highLength?.SetOrAdd(indexes.End.IntIndex, length - previousPart);
	}

	protected static void MoveRangeToLeaf(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment)
	{
		var (source, sourceIndex, destination, destinationIndex, length, mode) = context;
		Debug.Assert(source.high != null);
		var endIndex = sourceIndex + length - 1;
		var (start, end) = new CopyRangeSide(source, sourceIndex, endIndex);
		// Диапазон дописывается после конца списка
		if (start.IntIndex != 0 && !source.HasSufficientLength(start.IntIndex)
			&& start.RestIndex == 0 && source.LastEffectiveLength() != fragment)
			start = (start.Index, start.IntIndex - 1, source.LastEffectiveLength());
		// Диапазон частично расположен за концом списка
		if (start.IntIndex != 0 && !source.HasSufficientLength(start.IntIndex))
		{
			if (end.RestIndex >= fragment - source.LastEffectiveLength())
				end = (end.Index, end.IntIndex, end.RestIndex - (fragment - source.LastEffectiveLength()));
			else
				end = (end.Index, end.IntIndex - 1, end.RestIndex + source.LastEffectiveLength());
		}
		// Весь диапазон помещается в одну ветку
		if (start.IntIndex == end.IntIndex)
			stack.Insert(stackIndex, (source.high[start.IntIndex],
				ProcessReverse(source.high[start.IntIndex], start.RestIndex, length),
				destination, destinationIndex, length, mode & ~MoveRangeMode.SpecialMode));
		else if (source.IsReversed != destination.IsReversed)
			MoveRangeToLeafDiagonal(stack, context, stackIndex, fragment, (start, end));
		else
			MoveRangeToLeafDirect(stack, context, stackIndex, fragment, (start, end));
		if ((mode & MoveRangeMode.Copy) == 0)
		{
			if (start.IntIndex == end.IntIndex)
				source.highLength?[start.IntIndex] -= end.RestIndex + 1 - start.RestIndex;
			else
			{
				source.highLength?.Remove((start.IntIndex + 1)..end.IntIndex);
				source.highLength?[start.IntIndex + 1] -= end.RestIndex + 1;
				source.highLength?[start.IntIndex] = start.RestIndex;
			}
		}
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
	}

	protected static void MoveRangeToLeafDiagonal(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		var (source, _, destination, destinationIndex, length, mode) = context;
		Debug.Assert(source.high != null);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		var (start, end) = indexes;
		var newSize = destinationIndex + length;
		var oldSize = destination.Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, destination, -1, oldSize - newSize, mode & ~MoveRangeMode.SpecialMode));
		var previousPart = end.RestIndex + 1;
		stack.Insert(stackIndex, (source.high[end.IntIndex],
			ProcessReverse(source.high[end.IntIndex], 0, previousPart),
			destination, destinationIndex, previousPart, mode & ~MoveRangeMode.SpecialMode));
		for (var i = end.IntIndex - 1; i > start.IntIndex; i--)
		{
			var branchLength = source.HasSufficientLength(i + 1) ? source.high[i].Length : fragment;
			stack.Insert(stackIndex, (source.high[i], 0, destination,
				destinationIndex + previousPart, branchLength, mode & ~MoveRangeMode.SpecialMode));
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (source.high[start.IntIndex],
			ProcessReverse(source.high[start.IntIndex], start.RestIndex, length - previousPart),
			destination, destinationIndex + previousPart, length - previousPart, mode & ~MoveRangeMode.SpecialMode));
	}

	protected static void MoveRangeToLeafDirect(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide indexes)
	{
		var (source, _, destination, destinationIndex, length, mode) = context;
		Debug.Assert(source.high != null);
		// Диапазон копируется слева направо
		var (start, end) = indexes;
		var newSize = destinationIndex + length;
		var oldSize = destination.Length;
		if (newSize < oldSize)
			newSize = oldSize;
		if (newSize != oldSize)
			stack.Insert(stackIndex, (null!, -1, destination, -1, oldSize - newSize, mode & ~MoveRangeMode.SpecialMode));
		var previousPart = (source.HasSufficientLength(start.IntIndex + 1)
			? source.high[start.IntIndex].Length : fragment) - start.RestIndex;
		stack.Insert(stackIndex, (source.high[start.IntIndex],
			ProcessReverse(source.high[start.IntIndex], start.RestIndex, previousPart),
			destination, destinationIndex, previousPart, mode & ~MoveRangeMode.SpecialMode));
		for (var i = start.IntIndex + 1; i < end.IntIndex; i++)
		{
			var branchLength = source.HasSufficientLength(i + 1) ? source.high[i].Length : fragment;
			stack.Insert(stackIndex, (source.high[i], 0, destination,
				destinationIndex + previousPart, branchLength, mode & ~MoveRangeMode.SpecialMode));
			previousPart += branchLength;
		}
		stack.Insert(stackIndex, (source.high[end.IntIndex],
			ProcessReverse(source.high[end.IntIndex], 0, length - previousPart),
			destination, destinationIndex + previousPart, length - previousPart, mode & ~MoveRangeMode.SpecialMode));
	}

	protected static void MoveRangeToSimilar(List<MoveRangeContext> stack, MoveRangeContext context, int stackIndex)
	{
		var (source, sourceIndex, destination, destinationIndex, length, mode) = context;
		if (!(source.high != null && destination.high != null))
			throw new InvalidOperationException("Невозможно переместить диапазон. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина источника - {source.Length}, подветок источника - {source.high?.Length ?? 0},"
				+ $" реверс источника - {source.bReversed}, емкость источника - {source.Capacity},"
				+ $" длина назначения - {destination.Length}, подветок назначения - {destination.high?.Length ?? 0},"
				+ $" реверс назначения - {destination.bReversed}, емкость назначения - {destination.Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(context), "Индекс не может быть отрицательным.");
		if (destinationIndex > destination.Length)
			throw new ArgumentOutOfRangeException(nameof(context),
				"Индекс не может быть больше длины содержимого цели.");
		var fragment = destination.fragment;
		if (length == 0)
			return;
		var sourceEndIndex = sourceIndex + length - 1;
		var destinationEndIndex = destinationIndex + length - 1;
		var (sourceStart, sourceEnd) = new CopyRangeSide(source, sourceIndex, sourceEndIndex);
		var (destinationStart, destinationEnd) = new CopyRangeSide(destination, destinationIndex, destinationEndIndex);
		var (_, destinationIntIndex, destinationRestIndex) = destinationStart;
		// Сохраняем значение как изменяющееся после реверса
		while (sourceEnd.RestIndex >= source.fragment)
			sourceEnd = (sourceEnd.Index, sourceEnd.IntIndex + 1, sourceEnd.RestIndex - source.fragment);
		if (destination.HasSufficientLength(0) && !destination.HasSufficientLength(destinationEnd.IntIndex))
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex,
				destinationEnd.RestIndex - (fragment - destination.LastEffectiveLength()));
		while (destinationEnd.RestIndex >= fragment)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex + 1, destinationEnd.RestIndex - fragment);
		// Диапазон дописывается после конца списка
		if (destinationIntIndex != 0 && !destination.HasSufficientLength(destinationIntIndex)
			&& destinationRestIndex == 0 && destination.high[destinationIntIndex - 1].Length != fragment)
			destinationStart = (destinationIndex, destinationIntIndex - 1, destination.LastEffectiveLength());
		if (destinationEnd.IntIndex != 0 && destinationEnd.RestIndex < 0)
			destinationEnd = (destinationEnd.Index, destinationEnd.IntIndex - 1, destinationEnd.RestIndex + fragment);
		if (destinationEnd.IntIndex >= destination.high.Length
			|| destinationEnd.RestIndex < 0 || destination.HasSufficientLength(0)
			&& destination.Length + fragment - destination.LastEffectiveLength()
			+ (destination.high.Length - destination.EffectiveHighLength() - 1) * fragment
			+ destination.high[^1].Capacity < destinationIndex + length)
		{
			stack.Insert(stackIndex, (null!, -2, destination, -1, -destinationIndex - length, MoveRangeMode.Copy));
			stack.Insert(stackIndex, context);
			return;
		}
		var (_, sourceIntIndex, sourceRestIndex) = sourceStart;
		(_, destinationIntIndex, destinationRestIndex) = destinationStart;
		Debug.Assert(sourceIntIndex >= 0 && source.HasSufficientLength(sourceIntIndex)
			&& sourceEnd.IntIndex >= 0 && source.HasSufficientLength(sourceEnd.IntIndex)
			&& destinationIntIndex >= 0 && destinationIntIndex < destination.high.Length
			&& destinationEnd.IntIndex >= 0 && destinationEnd.IntIndex < destination.high.Length
			&& sourceRestIndex >= 0 && sourceEnd.RestIndex >= 0
			&& destinationRestIndex >= 0 && destinationEnd.RestIndex >= 0);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		if (source == destination && !source.bReversed && sourceIndex + 1 == destinationIndex
		&& sourceIndex + length == source.Length - 1 && sourceIntIndex == sourceEnd.IntIndex
		&& destinationIndex + length == source.Length)
			destination.RemoveEndInternal(sourceIndex + length);
		if ((mode & MoveRangeMode.Copy) != 0 && length == 1
			&& (destinationIndex == 0 || destinationIndex != destination.Length))
		{
			stack.Insert(stackIndex, (source.high[sourceIntIndex],
				source.high[sourceIntIndex].bReversed
				? source.high[sourceIntIndex].Length - sourceRestIndex - 1 : sourceRestIndex,
				destination.high[destinationIntIndex],
				destination.high[destinationIntIndex].Length == 0 ? 0
				: destination.high[destinationIntIndex].bReversed
				? destination.high[destinationIntIndex].Length - destinationRestIndex - 1
				: destinationRestIndex, 1, MoveRangeMode.Copy));
			if (destination.high[destinationIntIndex].Length == 0)
				destination.highLength?.SetOrAdd(destinationIntIndex, 1);
		}
		else if ((mode & MoveRangeMode.Copy) != 0
			&& source.fragment == source.LeafSize && destination.fragment == destination.LeafSize
			&& sourceIntIndex == sourceEnd.IntIndex && destinationIntIndex == destinationEnd.IntIndex
			&& destinationRestIndex + length <= destination.high[destinationIntIndex].Capacity)
		{
			if (destination.high[destinationIntIndex].bReversed
				&& destinationRestIndex + length > destination.high[destinationIntIndex].Length)
			{
				destination.high[destinationIntIndex].low!.ResizeLeft((int)(destinationRestIndex + length));
				destination.high[destinationIntIndex].Length = destination.high[destinationIntIndex].low!.Length;
			}
			MoveRangeTrivial((source.high[sourceIntIndex], ProcessReverse(source.high[sourceIntIndex], sourceRestIndex, length),
				destination.high[destinationIntIndex],
				ProcessReverse(destination.high[destinationIntIndex], destinationRestIndex, length),
				length, MoveRangeMode.Copy));
			if (destination.highLength != null && (destinationIntIndex >= destination.highLength.Length
				|| destinationRestIndex + length > destination.highLength[destinationIntIndex]))
				destination.highLength.SetOrAdd(destinationIntIndex, destinationRestIndex + length);
		}
		else if ((mode & MoveRangeMode.Copy) != 0
			&& source == destination && sourceIndex + 1 == destinationIndex && source.high[^1].Length != fragment
			&& (sourceIndex + length == source.Length - 1 && sourceIntIndex == sourceEnd.IntIndex
			|| sourceIndex + length == source.Length))
			MoveRangeToSimilarShiftOne(stack, context, stackIndex, sourceStart);
		// Диапазон "копируется в буфер" справа налево, а вставляется слева направо
		else if (source.IsReversed != destination.IsReversed)
			MoveRangeToSimilarDiagonal(stack, context, stackIndex, fragment,
				(sourceStart, sourceEnd), (destinationStart, destinationEnd));
		// Диапазон копируется слева направо
		else if (sourceIndex >= destinationIndex
			|| destinationIndex >= sourceIndex + length || source != destination)
			MoveRangeToSimilarLTR(stack, context, stackIndex, fragment,
				(sourceStart, sourceEnd), (destinationStart, destinationEnd));
		// Диапазон копируется справа налево
		else
			MoveRangeToSimilarRTL(stack, context, stackIndex, fragment,
				(sourceStart, sourceEnd), (destinationStart, destinationEnd));
	}

	protected static void MoveRangeToSimilarShiftOne(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, CopyRangeIndex sourceStart)
	{
		var (source, sourceIndex, destination, _, length, mode) = context;
		Debug.Assert((mode & MoveRangeMode.Copy) != 0);
		var offset = source.Length - (sourceIndex + length);
		Debug.Assert(source == destination && source.high != null);
		for (var i = sourceStart.IntIndex; i < source.high.Length; i++)
		{
			if ((source.high[i].Length == source.fragment || source.high[i].Length == source.high[i].Capacity)
				&& offset == 0)
				continue;
			if (source.high[i].bReversed && offset == 0)
				stack.Insert(stackIndex, (null!, -1, source.high[i], -1, -1, MoveRangeMode.Copy));
			var shiftLength = source.high[i].Length - (i == sourceStart.IntIndex ? sourceStart.RestIndex : 0) - offset;
			stack.Insert(stackIndex, (source.high[i],
				ProcessReverse(source.high[i], i == sourceStart.IntIndex ? sourceStart.RestIndex : 0,
				shiftLength, source.high[i].Length + 1 - offset), source.high[i],
				ProcessReverse(source.high[i], (i == sourceStart.IntIndex ? sourceStart.RestIndex : 0) + 1,
				shiftLength, source.high[i].Length + 1 - offset), shiftLength, MoveRangeMode.Copy));
			if ((i == sourceStart.IntIndex ? sourceStart.RestIndex : 0) + shiftLength == source.high[i].Length)
			{
				if (source.HasSufficientLength(i))
					source.highLength?[i] += 1;
				else
					source.highLength?.Add(1);
			}
			for (var j = i - 1; j >= sourceStart.IntIndex; j--)
			{
				stack.Insert(stackIndex, (source.high[j], source.high[j].bReversed ? 0 : source.high[j].Length - 1,
					source.high[j + 1],
					!source.high[j + 1].bReversed ? 0 : source.high[j + 1].Length - (j + 1 == i ? 0 : 1),
					1, MoveRangeMode.Copy));
				shiftLength = source.high[j].Length - (j == sourceStart.IntIndex ? sourceStart.RestIndex : 0) - 1;
				stack.Insert(stackIndex, (source.high[j],
					ProcessReverse(source.high[j], j == sourceStart.IntIndex ? sourceStart.RestIndex : 0,
					shiftLength, source.high[j].Length), source.high[j],
					ProcessReverse(source.high[j], (j == sourceStart.IntIndex ? sourceStart.RestIndex : 0) + 1,
					shiftLength, source.high[j].Length), shiftLength, MoveRangeMode.Copy));
			}
			return;
		}
		throw new InvalidOperationException("Невозможно скопировать диапазон. Возможные причины:\r\n" + InternalError
			+ $"Текущее состояние: длина источника - {source.Length}, подветок источника - {source.high?.Length ?? 0},"
			+ $" реверс источника - {source.bReversed}, емкость источника - {source.Capacity},"
			+ $" длина назначения - {destination.Length}, подветок назначения - {destination.high?.Length ?? 0},"
			+ $" реверс назначения - {destination.bReversed}, емкость назначения - {destination.Capacity},"
			+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	protected static void MoveRangeToSimilarDiagonal(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		var (source, _, destination, _, length, mode) = context;
		Debug.Assert(source.high != null && destination.high != null);
		var sourceCurrentRestIndex = sourceIndexes.End.RestIndex + 1;
		MpzT sourceCurrentRestIndexOffset = 0;
		var destinationCurrentRestIndex = destinationIndexes.Start.RestIndex;
		var iSource = sourceIndexes.End.IntIndex;
		var iDestination = destinationIndexes.Start.IntIndex;
		using ListHashSet<int> hs = [];
		MpzT step = 0;
		TCertain currentSource, currentDestination;
		var leftLength = length;
		while (leftLength > 0)
		{
			currentSource = source.high[iSource];
			currentDestination = destination.high[iDestination];
			var destinationMax = destination.HasSufficientLength(iDestination + 1)
				? currentDestination.Length : fragment;
			step = RedStarLinqMath.Min(sourceCurrentRestIndex, destinationMax - destinationCurrentRestIndex, leftLength);
			var sourceThresholdReached = (sourceCurrentRestIndex -= step) == 0;
			var newSize = RedStarLinqMath.Min(destinationMax, destinationCurrentRestIndex + leftLength);
			if (currentDestination.Length <= 1)
				currentDestination.bReversed = false;
			var oldSize = currentDestination.Length;
			if (newSize < oldSize)
				newSize = oldSize;
			if (newSize != oldSize && !hs.Contains(iDestination) && currentDestination.bReversed)
				stack.Insert(stackIndex, (null!, -1, currentDestination, -1, oldSize - newSize,
					mode & ~MoveRangeMode.SpecialMode));
			if (newSize != oldSize && !hs.Contains(iDestination))
			{
				destination.highLength?.SetOrAdd(iDestination, newSize);
				hs.Add(iDestination);
			}
			if ((mode & MoveRangeMode.Copy) != 0
				|| iSource == sourceIndexes.End.IntIndex && sourceIndexes.End.RestIndex + 1 != currentSource.Length)
				stack.Insert(stackIndex, (currentSource,
					ProcessReverse(currentSource, sourceCurrentRestIndex, step) - sourceCurrentRestIndexOffset,
					currentDestination, ProcessReverse(currentDestination, destinationCurrentRestIndex, step, newSize),
					step, mode & ~MoveRangeMode.SpecialMode));
			else if (sourceCurrentRestIndex != 0)
				stack.Insert(stackIndex, (source, iSource * (1L << 30) + iDestination, destination, step,
					ProcessReverse(currentDestination, destinationCurrentRestIndex, step, newSize),
					mode | MoveRangeMode.SpecialMode));
			else
				stack.Insert(stackIndex, (source, iSource * (1L << 30) + iDestination, destination, 0,
					ProcessReverse(currentDestination, destinationCurrentRestIndex, step, newSize),
					mode | MoveRangeMode.SpecialMode));
			if (currentSource.bReversed && (mode & MoveRangeMode.Copy) == 0)
				sourceCurrentRestIndexOffset += step;
			var destinationThresholdReached = (destinationCurrentRestIndex += step) == destinationMax;
			leftLength -= step;
			if (sourceThresholdReached)
			{
				iSource--;
				sourceCurrentRestIndex = iSource >= sourceIndexes.Start.IntIndex
					? source.high[iSource].Length : fragment;
				sourceCurrentRestIndexOffset = 0;
			}
			if (destinationThresholdReached)
			{
				iDestination++;
				destinationCurrentRestIndex = 0;
			}
		}
		destination.highLength?.SetOrAdd(destinationIndexes.End.IntIndex,
			RedStarLinqMath.Max(destination.HasSufficientLength(destinationIndexes.End.IntIndex)
			? destination.high[destinationIndexes.End.IntIndex].Length : 0, destinationIndexes.End.RestIndex + 1));
		if ((mode & MoveRangeMode.Copy) == 0)
		{
			if (sourceIndexes.Start.IntIndex == sourceIndexes.End.IntIndex)
				source.highLength?[sourceIndexes.Start.IntIndex]
					-= sourceIndexes.End.RestIndex + 1 - sourceIndexes.Start.RestIndex;
			else
			{
				source.highLength?.Remove((sourceIndexes.Start.IntIndex + 1)..sourceIndexes.End.IntIndex);
				source.highLength?[sourceIndexes.Start.IntIndex + 1] -= sourceIndexes.End.RestIndex + 1;
				source.highLength?[sourceIndexes.Start.IntIndex] = sourceIndexes.Start.RestIndex;
			}
		}
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
	}

	protected static void MoveRangeToSimilarLTR(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		var (source, _, destination, _, length, mode) = context;
		Debug.Assert(source.high != null && destination.high != null);
		Dictionary<int, MpzT> highLengthPool = [];
		var toEnd = sourceIndexes.End.Index + 1 == source.Length && destinationIndexes.End.Index + 1 >= destination.Length;
		var sourceCurrentRestIndex = sourceIndexes.Start.RestIndex;
		MpzT sourceCurrentRestIndexOffset = 0;
		var destinationCurrentRestIndex = destinationIndexes.Start.RestIndex;
		bool skipRest = false, skipRestNext = false;
		var iSource = sourceIndexes.Start.IntIndex;
		var iSourceOffset = 0;
		var iDestination = destinationIndexes.Start.IntIndex;
		using ListHashSet<int> hs = [];
		MpzT step = 0, sourceEffectiveLength;
		TCertain currentSource, currentDestination;
		var leftLength = length;
		while (leftLength > 0)
		{
			currentSource = source.high[iSource];
			currentDestination = destination.high[iDestination];
			var destinationMax = destination.HasSufficientLength(iDestination + 1) ? currentDestination.Length : fragment;
			if (source != destination)
				sourceEffectiveLength = source.high[iSource].Length;
			else
				sourceEffectiveLength = RedStarLinqMath.Max(highLengthPool.TryGetValue(iSource, out var poolLength)
					? poolLength : 0, source.high[iSource].Length);
			step = RedStarLinqMath.Min(sourceEffectiveLength - sourceCurrentRestIndex,
				destinationMax - destinationCurrentRestIndex, leftLength);
			if ((mode & MoveRangeMode.Copy) == 0)
				SetupSkipRest();
			if (currentDestination.Length <= 1)
				currentDestination.bReversed = false;
			var newSize = RedStarLinqMath.Min(destinationMax, destinationCurrentRestIndex + leftLength);
			var oldSize = currentDestination.Length;
			if (skipRestNext)
				newSize = destinationCurrentRestIndex + step;
			if (newSize < oldSize)
				newSize = oldSize;
			if (source == destination && iSource == iDestination && newSize != oldSize && !hs.Contains(iDestination))
				sourceEffectiveLength = newSize;
			if (newSize != oldSize && !hs.Contains(iDestination) && !skipRest && currentDestination.bReversed)
				stack.Insert(stackIndex, (null!, -1, currentDestination, -1, oldSize - newSize,
					mode & ~MoveRangeMode.SpecialMode));
			if (newSize != oldSize && !hs.Contains(iDestination) && !skipRest)
			{
				destination.highLength?.SetOrAdd(iDestination, newSize);
				highLengthPool[iDestination] = newSize;
				hs.Add(iDestination);
			}
			if ((mode & MoveRangeMode.Copy) != 0
				|| iSource == sourceIndexes.Start.IntIndex && sourceIndexes.Start.RestIndex != 0)
				stack.Insert(stackIndex, (currentSource,
					ProcessReverse(currentSource, sourceCurrentRestIndex - sourceCurrentRestIndexOffset, step,
					source != destination || currentSource.low == null && !hs.Contains(iSource) ? 0 : sourceEffectiveLength),
					currentDestination, ProcessReverse(currentDestination, destinationCurrentRestIndex, step, newSize),
					step, mode & ~MoveRangeMode.SpecialMode));
			else if (sourceCurrentRestIndex + step != sourceEffectiveLength && !skipRest)
				stack.Insert(stackIndex, (source, (iSource - iSourceOffset) * (1L << 30) + iDestination, destination, step,
					currentDestination.bReversed && currentDestination.Length != 0
					? newSize - destinationCurrentRestIndex - step : destinationCurrentRestIndex,
					mode | MoveRangeMode.SpecialMode));
			else if (!toEnd || !skipRest)
			{
				stack.Insert(stackIndex, (source, (iSource - iSourceOffset++) * (1L << 30) + iDestination,
					destination, 0, currentDestination.bReversed && currentDestination.Length != 0
					? newSize - destinationCurrentRestIndex - step : destinationCurrentRestIndex,
					mode | MoveRangeMode.SpecialMode));
				skipRest |= skipRestNext;
			}
			else if (sourceIndexes.End.RestIndex + 1 == source.high[sourceIndexes.End.IntIndex].Length)
			{
				stack.Insert(stackIndex, (source, (iSource - iSourceOffset) * (1L << 30) + iDestination,
					destination, 0, iSource - sourceIndexes.End.IntIndex - 1, mode | MoveRangeMode.SpecialMode));
				leftLength = 0;
				break;
			}
			else
			{
				stack.Insert(stackIndex, (source, (iSource - iSourceOffset++) * (1L << 30) + iDestination,
					destination, 0, iSource - sourceIndexes.End.IntIndex, mode | MoveRangeMode.SpecialMode));
				leftLength = sourceIndexes.End.RestIndex + 1;
				iDestination += sourceIndexes.End.IntIndex - iSource;
				destination.highLength?.Resize(iDestination);
				iSource = sourceIndexes.End.IntIndex;
				sourceCurrentRestIndex = sourceCurrentRestIndexOffset = destinationCurrentRestIndex = 0;
				continue;
			}
			var sourceThresholdReached = (sourceCurrentRestIndex += step) == sourceEffectiveLength;
			Debug.Assert(sourceCurrentRestIndex <= sourceEffectiveLength);
			if (!currentSource.bReversed && (mode & MoveRangeMode.Copy) == 0)
				sourceCurrentRestIndexOffset += step;
			var destinationThresholdReached = (destinationCurrentRestIndex += step) == destinationMax;
			leftLength -= step;
			if (skipRest)
				destination.highLength?.SetOrAdd(iDestination, destinationCurrentRestIndex);
			if (sourceThresholdReached)
			{
				iSource++;
				sourceCurrentRestIndex = sourceCurrentRestIndexOffset = 0;
			}
			if (destinationThresholdReached || skipRest)
			{
				iDestination++;
				destinationCurrentRestIndex = 0;
			}
		}
		Debug.Assert(leftLength == 0);
		if ((mode & MoveRangeMode.Copy) == 0)
		{
			if (sourceIndexes.Start.IntIndex == sourceIndexes.End.IntIndex)
				source.highLength?[sourceIndexes.Start.IntIndex]
					-= sourceIndexes.End.RestIndex + 1 - sourceIndexes.Start.RestIndex;
			else
			{
				source.highLength?.Remove((sourceIndexes.Start.IntIndex + 1)..sourceIndexes.End.IntIndex);
				source.highLength?[sourceIndexes.Start.IntIndex + 1] -= sourceIndexes.End.RestIndex + 1;
				source.highLength?[sourceIndexes.Start.IntIndex] = sourceIndexes.Start.RestIndex;
			}
		}
#if VERIFY
		source.Verify();
		destination.Verify();
#endif
		void SetupSkipRest()
		{
			skipRest |= toEnd && destinationCurrentRestIndex == 0 && !destination.HasSufficientLength(iDestination)
				&& destination.ValuesSum() + leftLength <= destination.Capacity
				&& sourceIndexes.End.IntIndex + Max(iDestination - iSource, 0)
				+ (sourceIndexes.End.RestIndex + 1 == source.high[sourceIndexes.End.IntIndex].Length ? 1 : 0)
				<= destination.high.Length;
			if (skipRest
				&& (iSource != sourceIndexes.End.IntIndex || sourceIndexes.End.RestIndex + 1 == sourceEffectiveLength))
				step = RedStarLinqMath.Min(sourceEffectiveLength - sourceCurrentRestIndex,
					fragment - destinationCurrentRestIndex, leftLength);
			skipRestNext |= skipRest || toEnd && destinationCurrentRestIndex + step >= currentDestination.Length
				&& !(currentDestination.bReversed && currentDestination.Length != 0)
				&& !destination.HasSufficientLength(iDestination + (destinationCurrentRestIndex == 0 ? 0 : 1))
				&& destination.ValuesSum() + leftLength <= destination.Capacity
				&& sourceIndexes.End.IntIndex + Max(iDestination + 1 - iSource, 0)
				+ (sourceIndexes.End.RestIndex + 1 == source.high[sourceIndexes.End.IntIndex].Length ? 1 : 0)
				<= destination.high.Length;
		}
	}

	protected static void MoveRangeToSimilarRTL(List<MoveRangeContext> stack, MoveRangeContext context,
		int stackIndex, MpzT fragment, CopyRangeSide sourceIndexes, CopyRangeSide destinationIndexes)
	{
		var (source, _, destination, _, length, mode) = context;
		Debug.Assert(source == destination && source.high != null);
		Debug.Assert((mode & MoveRangeMode.Copy) != 0);
		var sourceEffectiveHighLength = source.EffectiveHighLength();
		MpzT destinationHighLengthOld = new(sourceEffectiveHighLength);
		for (var i = sourceEffectiveHighLength; i < destinationIndexes.End.IntIndex + 1; i++)
			source.highLength?.Add(1);
		var sourceCurrentRestIndex = sourceIndexes.End.RestIndex + 1;
		var destinationCurrentRestIndex = destinationIndexes.End.RestIndex + 1;
		var iSource = sourceIndexes.End.IntIndex;
		var iDestination = destinationIndexes.End.IntIndex;
		MpzT step = 0, newSize, oldSize;
		using ListHashSet<int> hs = [];
		for (var i = destinationIndexes.Start.IntIndex; i < destinationIndexes.End.IntIndex; i++)
		{
			newSize = i >= destinationHighLengthOld - 1 ? fragment : 0;
			oldSize = source.high[i].Length;
			if (newSize > oldSize)
			{
				source.highLength?[i] = newSize;
				stack.Insert(stackIndex, (null!, -1, source.high[i], -1, oldSize - newSize, MoveRangeMode.Copy));
				hs.Add(i);
			}
		}
		newSize = destinationIndexes.End.RestIndex + 1;
		oldSize = source.high[destinationIndexes.End.IntIndex].Length;
		if (newSize > oldSize)
		{
			source.highLength?[destinationIndexes.End.IntIndex] = newSize;
			stack.Insert(stackIndex, (null!, -1, source.high[destinationIndexes.End.IntIndex],
				-1, oldSize - newSize, MoveRangeMode.Copy));
			hs.Add(destinationIndexes.End.IntIndex);
		}
		TCertain currentSource;
		var lengthSource = EffectiveLength(iSource);
		var leftLength = length;
		while (leftLength > 0)
		{
			step = RedStarLinqMath.Min(sourceCurrentRestIndex, destinationCurrentRestIndex, leftLength);
			var sourceThresholdReached = (sourceCurrentRestIndex -= step) == 0;
			var destinationThresholdReached = (destinationCurrentRestIndex -= step) == 0;
			currentSource = source.high[iSource];
			stack.Insert(stackIndex, (currentSource,
				ProcessReverse(currentSource, sourceCurrentRestIndex, step,
				currentSource.low == null && !hs.Contains(iSource) ? 0 : lengthSource),
				source.high[iDestination],
				ProcessReverse(source.high[iDestination], destinationCurrentRestIndex, step,
				EffectiveLength(iDestination)), step, MoveRangeMode.Copy));
			leftLength -= step;
			if (sourceThresholdReached)
			{
				iSource--;
				sourceCurrentRestIndex = iSource >= sourceIndexes.Start.IntIndex
					? lengthSource = source.high[iSource].Length : fragment;
			}
			if (destinationThresholdReached)
			{
				iDestination--;
				destinationCurrentRestIndex = iDestination < destinationHighLengthOld - 1
					? source.high[iDestination].Length : fragment;
			}
		}
#if VERIFY
		source.Verify();
#endif
		MpzT EffectiveLength(int index)
		{
			MpzT result;
			if (index == destinationIndexes.End.IntIndex)
				result = destinationIndexes.End.RestIndex + 1;
			else if (index < destinationHighLengthOld - 1)
				result = source.high[index].Length;
			else
				result = fragment;
			if (source.high[index].Length > result)
				result = source.high[index].Length;
			return result;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void MoveRangeTrivial(MoveRangeContext context)
	{
		var (source, sourceIndex, destination, destinationIndex, length, mode) = context;
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
				destination.Length = destinationIndex + length;
			}
		}
		// Если оба списка направлены одинаково, копируем напрямую
		if (source.IsReversed == destination.IsReversed || length == 1)
			source.low.CopyRangeTo((int)sourceIndex, destination.low, (int)destinationIndex, (int)length);
		else // Иначе используем срезы для предотвращения лишних проходов
		{
			var reversedSource = source.low.Skip((int)sourceIndex).Take((int)length).Reverse();
			if (destination.low is List<T> lowList && typeof(List<T>).GetField("_items",
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
				?.GetValue(lowList) is T[] destinationArray)
				reversedSource.CopyTo(destinationArray, (int)destinationIndex);
			else
				destination.low.SetRange((int)destinationIndex, reversedSource);
		}
		if ((mode & MoveRangeMode.Copy) == 0)
		{
			source.low.Remove((int)sourceIndex, (int)length);
			source.Length = source.low.Length;
		}
#if VERIFY
		Debug.Assert(destination.Length == RedStarLinqMath.Max(oldLength, destinationIndex + length));
		source.Verify();
		destination.Verify();
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
	protected static MoveRangeContext ProcessReverseContext(MoveRangeContext context,
		MpzT destinationBound = default, bool processSource = true,
		bool processDestination = true) =>
		(context.Source, processSource ? ProcessReverse(context.Source, context.SourceIndex,
		context.Length, context.Source.Length) : context.SourceIndex, context.Destination, processDestination
		? ProcessReverse(context.Destination, context.DestinationIndex, context.Length,
		destinationBound) : context.DestinationIndex, context.Length, context.Mode);

	protected virtual void ReduceCapacityExponential(MpzT newFragment)
	{
		var this2 = (TCertain)this;
		var reverse = false;
		Root.accessCache?.Clear();
		do
		{
			Debug.Assert(high != null);
			if (high.Length > 1 && high[1].Length != 0)
				MoveRange((null!, -2, (TCertain)this, -1, -fragment << SubbranchesBitLength, MoveRangeMode.None));
			fragment >>= SubbranchesBitLength;
			var oldHigh = high;
			var oldHighLength = highLength;
			for (var i = 1; i < oldHigh.Length; i++)
				oldHigh[i].Dispose();
			highLength?.Dispose();
			low = oldHigh[0].low;
			high = oldHigh[0].high;
			highLength = oldHigh[0].highLength;
			AddCapacity(oldHigh[0].Capacity - _capacity);
			reverse ^= oldHigh[0].bReversed;
			oldHigh.Dispose();
			oldHighLength?.Dispose();
			if (high == null)
			{
				Debug.Assert(low != null);
				break;
			}
			foreach (var x in high)
				x.parent = this2;
		} while (fragment > newFragment);
		if (reverse)
			Reverse();
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual void ReduceCapacityLinear(MpzT value)
	{
		Debug.Assert(high != null);
		var highCount = (int)GetArrayLength(value, fragment);
		if (highCount == high.Length || high[highCount].Length != 0)
			MoveRange((null!, -2, (TCertain)this, -1, -fragment << SubbranchesBitLength, MoveRangeMode.None));
		for (var i = high.Length - 1; !HasSufficientLength(i); i--)
		{
			AddCapacity(-high[i].Capacity);
			high[i].Dispose();
		}
		high.RemoveEnd(EffectiveHighLength());
		var leftCapacity = value % fragment;
		if (leftCapacity == 0)
			leftCapacity = fragment;
		if (high[^1].Length > leftCapacity)
			MoveRange((null!, -2, (TCertain)this, -1, -fragment << SubbranchesBitLength, MoveRangeMode.None));
		var targetLastCapacity = leftCapacity == 0 ? fragment : leftCapacity;
		if (high[^1].Capacity > targetLastCapacity)
		{
			if (high[^1].low != null)
			{
				high[^1].low!.Capacity = (int)targetLastCapacity;
				high[^1].AddCapacity(targetLastCapacity - high[^1].Capacity);
			}
			else
				high[^1].ReduceCapacityLinear(targetLastCapacity);
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected override void RemoveAtInternal(MpzT index)
	{
		Root.accessCache?.Clear();
		if (low != null)
		{
			low.RemoveAt((int)(bReversed ? Length - 1 - index : index));
			Length -= 1;
		}
		else if (high != null)
		{
			var intIndex = IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var restIndex);
			if (HasSufficientLength(intIndex + 1) && high[intIndex].Length == 1)
			{
				var temp = high[intIndex];
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(intIndex + 1, high, intIndex, high.Length - intIndex - offsetFromEnd - 1);
				var pasteIndex = ^(offsetFromEnd + 1);
				high[pasteIndex] = temp;
				var lastHigh = high[^1];
				lastHigh.IncreaseCapacity(fragment, high[0].fragment);
				(high[pasteIndex], high[^1]) = (lastHigh, temp);
				temp.ClearInternal(false);
				highLength?.RemoveAt(intIndex);
			}
			else
			{
				highLength?.Decrease(intIndex);
				high[intIndex].RemoveAtInternal(restIndex);
			}
		}
		else
			throw new InvalidOperationException("Невозможно удалить элемент. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
		else if (high != null)
		{
			var intIndex = IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var restIndex);
			for (var i = high.Length - 1; i > intIndex; i--)
				high[i].ClearInternal(false);
			high[intIndex].RemoveEndInternal(restIndex);
			if (restIndex == 0)
				highLength?.RemoveEnd(intIndex);
			else
			{
				highLength?.RemoveEnd(intIndex + 1);
				highLength?[intIndex] = restIndex;
			}
		}
		else
			throw new InvalidOperationException("Невозможно удалить диапазон. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
		else if (high == null)
			throw new InvalidOperationException("Невозможно удалить диапазон. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
		Root.accessCache?.Clear();
		var endIndex = index + length - 1;
		if (bReversed)
			(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
		var ((_, intIndex, restIndex), (_, endIntIndex, endRestIndex)) = new CopyRangeSide((TCertain)this, index, endIndex);
		var highAtI = high[intIndex];
		if (intIndex == endIntIndex)
		{
			if (HasSufficientLength(intIndex + 1) && restIndex == 0 && endRestIndex == high[intIndex].Length - 1)
			{
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(intIndex + 1, high, intIndex, high.Length - intIndex - offsetFromEnd - 1);
				var pasteIndex = ^(offsetFromEnd + 1);
				high[pasteIndex] = highAtI;
				var lastHigh = high[^1];
				if (lastHigh.Capacity != fragment)
					lastHigh.IncreaseCapacity(fragment, high[0].fragment);
				(high[pasteIndex], high[^1]) = (lastHigh, highAtI);
				highAtI.ClearInternal(false);
				if (offsetFromEnd != 0)
					highLength?[intIndex] = highLength[intIndex + 1];
				highLength?.RemoveAt(intIndex + offsetFromEnd);
			}
			else
			{
				high[intIndex].RemoveInternal(restIndex, length);
				highLength?[intIndex] -= length;
			}
		}
		else
		{
			var startOffset = restIndex == 0 ? 0 : 1;
			var endOffset = endRestIndex + 1 == high[endIntIndex].Length ? 1 : 0;
			if (startOffset == 1)
			{
				highAtI.RemoveInternal(restIndex, highAtI.Length - restIndex);
				highLength?[intIndex] = restIndex;
			}
			var pasteIndex = intIndex + startOffset;
			var bufferLength = endIntIndex + endOffset - (intIndex + startOffset);
			var bufferRange = bufferLength == 0 ? null : high.GetRange(pasteIndex, bufferLength);
			var tempOffset = Capacity == high.Length * fragment ? 0 : 1;
			var copyLength = high.Length - (endIntIndex + endOffset) - tempOffset;
			if (copyLength > 0)
			{
				high.CopyRangeTo(endIntIndex + endOffset, high, intIndex + startOffset, copyLength);
				bufferRange?.CopyRangeTo(0, high, pasteIndex = high.Length + intIndex + startOffset
					- (endIntIndex + endOffset) - tempOffset, Min(bufferLength, high.Length - pasteIndex));
			}
			if (bufferRange != null && bufferRange.Length != 0)
			{
				var lastHigh = high[^1];
				if (lastHigh.Capacity != fragment)
					lastHigh.IncreaseCapacity(fragment, high[0].fragment);
				(high[pasteIndex], high[^1]) = (lastHigh, bufferRange[0]);
				bufferRange[0].ClearInternal(false);
			}
			highLength?.Remove((intIndex + startOffset)..(endIntIndex + endOffset));
			for (var i = high.Length - (bufferRange?.Length ?? 0); i < high.Length; i++)
				high[i].ClearInternal(false);
			if (endOffset == 0)
			{
				high[intIndex + startOffset].RemoveInternal(0, endRestIndex + 1);
				if (HasSufficientLength(intIndex + startOffset))
					highLength?[intIndex + startOffset] -= endRestIndex + 1;
			}
		}
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
		MoveRange((null!, -1, (TCertain)this, -1, -newSize, MoveRangeMode.Copy));
	}

	protected override void ResizeLeftInternal(MpzT newSize)
	{
		if (newSize < Length)
		{
			RemoveInternal(0, Length - newSize);
			return;
		}
		EnsureCapacity(newSize);
		newSize -= Length;
		MoveRange((null!, -1, (TCertain)this, -2, -newSize, MoveRangeMode.Copy));
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
		Debug.Assert(high != null);
		var endIndex = index + length - 1;
		var bReversedOld = bReversed;
		if (bReversedOld)
		{
			Reverse();
			(index, endIndex) = (Length - 1 - endIndex, Length - 1 - index);
		}
		try
		{
			var ((_, intIndex, restIndex), (_, endIntIndex, endRestIndex)) = new CopyRangeSide((TCertain)this, index, endIndex);
			if (intIndex == endIntIndex)
			{
				high[intIndex].ReverseInternal(restIndex, length);
				return;
			}
			if (restIndex == 0 && endRestIndex == high[endIntIndex].Length - 1)
			{
				for (var i = intIndex; i < endIntIndex + 1; i++)
					high[i].Reverse().IncreaseCapacity(fragment, high[0].fragment);
				high.Reverse(intIndex, endIntIndex + 1 - intIndex);
				highLength?.Reverse(intIndex, endIntIndex + 1 - intIndex);
				return;
			}
			for (var i = intIndex + 1; i < endIntIndex; i++)
				high[i].Reverse();
			high.Reverse(intIndex + 1, endIntIndex - 1 - intIndex);
			highLength?.Reverse(intIndex + 1, endIntIndex - 1 - intIndex);
			TCertain tempRange;
			if (endRestIndex + 1 == high[intIndex].Length - restIndex)
			{
				tempRange = high[endIntIndex].GetRangeInternal(0, endRestIndex + 1,
					endRestIndex + 1 == high[endIntIndex].Length);
				MoveRange(ProcessReverseContext((high[intIndex], restIndex, high[endIntIndex], 0,
					endRestIndex + 1, MoveRangeMode.Copy)));
				high[endIntIndex].ReverseInternal(0, endRestIndex + 1);
				tempRange.Reverse();
				MoveRange(ProcessReverseContext((tempRange, 0, high[intIndex], restIndex,
					tempRange.Length, MoveRangeMode.None), processSource: false));
				return;
			}
			var greaterAtEnd = endRestIndex + 1 > high[intIndex].Length - restIndex;
			var greaterIndex = greaterAtEnd ? endIntIndex : intIndex;
			var lessIndex = greaterAtEnd ? intIndex : endIntIndex;
			var greater = high[greaterIndex];
			var less = high[lessIndex];
			var commonLength = greaterAtEnd ? high[intIndex].Length - restIndex : (endRestIndex + 1) % fragment;
			var endCommonIndex = greaterAtEnd ? endRestIndex + 1 - commonLength : 0;
			var difference = (endRestIndex + 1 - high[intIndex].Length + restIndex).Abs();
			tempRange = high[endIntIndex].GetRangeInternal(endCommonIndex, commonLength,
				endCommonIndex == 0 && commonLength == high[endIntIndex].Length
				|| restIndex > endCommonIndex - commonLength && restIndex < endCommonIndex + commonLength);
			MoveRange(ProcessReverseContext((high[intIndex], restIndex, high[endIntIndex], endCommonIndex,
				commonLength, MoveRangeMode.Copy)));
			high[endIntIndex].ReverseInternal(endCommonIndex, commonLength);
			tempRange.Reverse();
			MoveRange(ProcessReverseContext((tempRange, 0, high[intIndex], restIndex, commonLength, MoveRangeMode.None),
				processSource: false));
			var takeIndex = greaterAtEnd ? 0 : restIndex + commonLength;
			var insertIndex = greaterAtEnd ? less.Length : 0;
			if (less.Length + difference <= fragment)
			{
				if (less.Length + difference > less.Capacity)
					less.IncreaseCapacity(fragment, high[0].fragment);
				less.InsertInternal(insertIndex, greater.GetRangeInternal(takeIndex, difference).Reverse(), false);
				greater.RemoveInternal(takeIndex, difference);
				highLength?[greaterIndex] -= difference;
				highLength?[lessIndex] += difference;
				return;
			}
			if (difference <= 1 && endIntIndex - 1 - intIndex == 0)
				return;
			if (endIntIndex - 1 - intIndex == 0 && (takeIndex == 0 || takeIndex + difference == greater.Length))
			{
				greater.ReverseInternal(takeIndex, difference);
				return;
			}
			tempRange.IncreaseCapacity(fragment, high[0].fragment);
			MoveRange(ProcessReverseContext((greater, takeIndex, tempRange, 0, difference, MoveRangeMode.None),
				processDestination: false));
			tempRange.Reverse();
			highLength?[greaterIndex] -= difference;
#if VERIFY
			Verify();
#endif
			InsertInternal(GetLeftValuesSum(lessIndex, out _) + insertIndex, tempRange, false);
		}
		finally
		{
#if VERIFY
			if (high != null)
			{
				Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
				Debug.Assert(highLength == null || highLength.All((x, index) => x == high[index].Length));
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
		else if (high != null)
		{
			var intIndex = IndexOfNotGreaterSum(bReversed ? Length - 1 - index : index, out var restIndex);
			if (intIndex != 0 && (!HasSufficientLength(intIndex) || restIndex == high[intIndex].Length)
				&& high[intIndex - 1].Capacity != high[intIndex - 1].Length)
				restIndex = high[--intIndex].Length;
			high[intIndex].SetInternal(restIndex, value);
		}
		else
			throw new InvalidOperationException("Невозможно установить элемент. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length ?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
#if VERIFY
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
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
		if (high != null)
			Debug.Assert((highLength == null || Length == highLength?.ValuesSum) && Length == high.Sum(x => x.Length));
		Verify();
#endif
	}

	protected virtual MpzT ValuesSum()
	{
		if (highLength != null)
			return highLength.ValuesSum;
		Debug.Assert(high != null);
		MpzT sum = 0;
		for (var i = 0; i < high.Length; i++)
			if (high[i].Length == 0)
				return sum;
			else
				sum += high[i].Length;
		return sum;
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
		Debug.Assert(low != null ^ high != null);
		Debug.Assert(Length <= Capacity);
		if (low != null)
		{
			Debug.Assert(Length == low.Length);
			Debug.Assert(Length <= LeafSize);
			Debug.Assert(Capacity <= LeafSize);
		}
		else if (high != null)
		{
			Debug.Assert(Length == high.Sum(x => x.Length));
			Debug.Assert(Length <= fragment << SubbranchesBitLength);
			Debug.Assert(Capacity > 0 && Capacity <= fragment << SubbranchesBitLength);
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high.SkipLast(1).All(x => x.Capacity == fragment));
			Debug.Assert(high.Length == 0 || high[^1].Capacity <= fragment);
			Debug.Assert((high.Length - 1) * fragment + high[^1].Capacity == Capacity);
		}
		else
			throw new InvalidOperationException("Обнаружен недействительный список. Возможные причины:\r\n" + InternalError
				+ $"Текущее состояние: длина - {Length}, подветок - {high?.Length?? 0},"
				+ $" реверс - {bReversed}, емкость - {Capacity},"
				+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}
#endif

	[Flags]
	protected enum MoveRangeMode : byte
	{
		None = 0,
		Copy = 1,
		SpecialMode = 2,
	}

	protected readonly record struct CopyRangeContext(TCertain Source, MpzT SourceIndex,
		TCertain Destination, MpzT DestinationIndex, MpzT Length)
	{
		public static implicit operator CopyRangeContext((TCertain Source, MpzT SourceIndex,
			TCertain Destination, MpzT DestinationIndex, MpzT Length) x) =>
			new(x.Source, x.SourceIndex, x.Destination, x.DestinationIndex, x.Length);
	}

	protected readonly record struct MoveRangeContext(TCertain Source, MpzT SourceIndex,
		TCertain Destination, MpzT DestinationIndex, MpzT Length, MoveRangeMode Mode)
	{
		public static implicit operator MoveRangeContext((TCertain Source, MpzT SourceIndex,
			TCertain Destination, MpzT DestinationIndex, MpzT Length, MoveRangeMode Mode) x) =>
			new(x.Source, x.SourceIndex, x.Destination, x.DestinationIndex, x.Length, x.Mode);
	}

	protected readonly record struct CopyRangeIndex
	{
		public MpzT Index { get; }
		public MpzT RestIndex { get; }
		public int IntIndex { get; }

		public CopyRangeIndex(MpzT index, int intIndex, MpzT restIndex)
		{
			Index = index;
			IntIndex = intIndex;
			RestIndex = restIndex;
		}

		public void Deconstruct(out MpzT index, out int intIndex, out MpzT restIndex)
		{
			index = Index;
			intIndex = IntIndex;
			restIndex = RestIndex;
		}

		public static implicit operator CopyRangeIndex((MpzT Index, int IntIndex, MpzT RestIndex) x) =>
			new(x.Index, x.IntIndex, x.RestIndex);
	}

	protected readonly struct CopyRangeSide
	{
		public CopyRangeIndex Start { get; }
		public CopyRangeIndex End { get; }

		public CopyRangeSide(CopyRangeIndex start, CopyRangeIndex end)
		{
			Start = start;
			End = end;
		}

		public CopyRangeSide(TCertain source, MpzT startSum, MpzT endSum)
		{
			source.IndexOfNotGreaterSum(startSum, endSum, out var start, out var end);
			Start = start;
			End = end;
		}

		public readonly void Deconstruct(out CopyRangeIndex start, out CopyRangeIndex end)
		{
			start = Start;
			end = End;
		}

		public static implicit operator CopyRangeSide((CopyRangeIndex Start, CopyRangeIndex End) x) => new(x.Start, x.End);
	}
}
