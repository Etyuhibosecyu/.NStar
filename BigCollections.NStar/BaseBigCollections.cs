global using Corlib.NStar;
global using ILGPU;
global using ILGPU.Runtime;
global using Mpir.NET;
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
public abstract class BaseBigList<T, TCertain, TLow> : IBigList<T>, IDisposable where TCertain : BaseBigList<T, TCertain, TLow>, new() where TLow : G.IList<T>, new()
{
	public virtual T this[MpzT index]
	{
		get
		{
			if (index >= Length)
				throw new IndexOutOfRangeException();
			return GetInternal(index);
		}
		set
		{
			if (index >= Length)
				throw new IndexOutOfRangeException();
			SetInternal(index, value);
		}
	}

	public abstract MpzT Capacity { get; set; }

	public virtual MpzT Length { get; private protected set; }

	private protected abstract Func<MpzT, TCertain> CapacityCreator { get; }

	private protected abstract Func<int, TLow> CapacityLowCreator { get; }

	private protected abstract Func<G.IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected abstract Func<G.IEnumerable<T>, TLow> CollectionLowCreator { get; }

	private protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public abstract TCertain Add(T item);

	void IBigCollection<T>.Add(T item) => Add(item);

	public virtual TCertain AddRange(G.IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		var this2 = (TCertain)this;
		var length = bigList.Length;
		if (length == 0)
			return this2;
		EnsureCapacity(Length + length);
#if VERIFY
		MpzT oldLength = new(Length);
#endif
		bigList.CopyToInternal(0, this2, Length, length);
		if (collection is not TCertain)
			bigList.Dispose();
#if VERIFY
		Debug.Assert(Length == oldLength + length);
		Verify();
#endif
		return this2;
	}

	private protected void Changed() => ListChanged?.Invoke((TCertain)this);

	public virtual void Clear()
	{
		if (Length > 0)
			ClearInternal();
	}

	public virtual void Clear(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Очищаемый диапазон выходит за текущий размер коллекции.");
		ClearInternal(index, length);
	}

	private protected abstract void ClearInternal(bool verify = true);

	private protected abstract void ClearInternal(MpzT index, MpzT length);

	public virtual bool Contains(T item) => Contains(item, 0, Length);

	public virtual bool Contains(T item, MpzT index) => Contains(item, index, Length - index);

	public virtual bool Contains(T item, MpzT index, MpzT length)
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
		for (MpzT i = new(index); i < index + length; i++)
			if (GetInternal(i)?.Equals(item) ?? item == null)
				return true;
		return false;
	}

	public virtual bool Contains(G.IEnumerable<T> collection) => Contains(collection, 0, Length);

	public virtual bool Contains(G.IEnumerable<T> collection, MpzT index) => Contains(collection, index, Length - index);

	public virtual bool Contains(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		return ContainsInternal(collection, index, length);
	}

	public virtual bool Contains(TCertain list) => Contains((G.IEnumerable<T>)list, 0, Length);

	public virtual bool Contains(TCertain list, MpzT index) => Contains((G.IEnumerable<T>)list, index, Length - index);

	public virtual bool Contains(TCertain list, MpzT index, MpzT length) => Contains((G.IEnumerable<T>)list, index, length);

	public virtual bool ContainsAny(G.IEnumerable<T> collection) => ContainsAny(collection, 0, Length);

	public virtual bool ContainsAny(G.IEnumerable<T> collection, MpzT index) => ContainsAny(collection, index, Length - index);

	public virtual bool ContainsAny(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		var hs = collection.ToHashSet();
		for (MpzT i = new(index); i < index + length; i++)
			if (hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAny(TCertain list) => ContainsAny((G.IEnumerable<T>)list, 0, Length);

	public virtual bool ContainsAny(TCertain list, MpzT index) => ContainsAny((G.IEnumerable<T>)list, index, Length - index);

	public virtual bool ContainsAny(TCertain list, MpzT index, MpzT length) =>
		ContainsAny((G.IEnumerable<T>)list, index, length);

	public virtual bool ContainsAnyExcluding(G.IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, Length);

	public virtual bool ContainsAnyExcluding(G.IEnumerable<T> collection, MpzT index) =>
		ContainsAnyExcluding(collection, index, Length - index);

	public virtual bool ContainsAnyExcluding(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		var hs = collection.ToHashSet();
		for (MpzT i = new(index); i < index + length; i++)
			if (!hs.Contains(GetInternal(i)))
				return true;
		return false;
	}

	public virtual bool ContainsAnyExcluding(TCertain list) => ContainsAnyExcluding((G.IEnumerable<T>)list, 0, Length);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index) =>
		ContainsAnyExcluding((G.IEnumerable<T>)list, index, Length - index);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index, MpzT length) =>
		ContainsAnyExcluding((G.IEnumerable<T>)list, index, length);

	private protected virtual bool ContainsInternal(G.IEnumerable<T> collection, MpzT index, MpzT length)
	{
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		if (length == 0 || !collection.Any())
			return false;
		if (collection is not IBigList<T> list)
			list = CollectionCreator(collection);
		MpzT j = 0;
		for (MpzT i = 0; i - j <= length - list.Length; i++)
		{
			if (GetInternal(index + i)?.Equals(list[j]) ?? list[j] == null)
			{
				j++;
				if (j >= list.Length)
					return true;
			}
			else if (j != 0)
			{
				i -= j;
				j = 0;
			}
		}
		return false;
	}

	public virtual TCertain Copy() => CollectionCreator(this);

	private protected abstract void CopyToInternal(MpzT sourceIndex,
		TCertain destination, MpzT destinationIndex, MpzT length);

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (array.Rank != 1)
			throw new RankException();
		try
		{
			CopyToInternal(array, arrayIndex);
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		}
	}

	public virtual void CopyTo(IBigList<T> list) => CopyTo(list, 0);

	public virtual void CopyTo(IBigList<T> list, MpzT listIndex) => CopyTo(0, list, listIndex, Length);

	public virtual void CopyTo(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		if (listIndex + length > list.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевой коллекции.");
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(MpzT index, TCertain list, MpzT listIndex, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		if (listIndex + length > list.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевой коллекции.");
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(MpzT index, T[] array, int arrayIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за текущий размер коллекции.");
		if (arrayIndex + length > array.Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		CopyToInternal(index, array, arrayIndex, length);
	}

	public virtual void CopyTo(T[] array) => CopyTo(array, 0);

	public virtual void CopyTo(T[] array, int arrayIndex)
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		CopyTo(0, array, arrayIndex, (int)Length);
	}

	private protected virtual void CopyToInternal(Array array, int arrayIndex)
	{
		if (array is not T[] array2)
			throw new ArgumentException("Ошибка, такой тип массива не подходит для копирования этой коллекции.", nameof(array));
		CopyToInternal(0, array2, arrayIndex, (int)Length);
	}

	private protected abstract void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length);

	private protected abstract void CopyToInternal(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length);

	public abstract void Dispose();

	private protected virtual void EnsureCapacity(MpzT min)
	{
		if (Capacity < min)
		{
			var newCapacity = Length == 0 ? DefaultCapacity : Length * 2;
			if (newCapacity < min)
				newCapacity = min;
			Capacity = newCapacity;
		}
	}

	public virtual bool Equals(G.IEnumerable<T>? collection) => EqualsInternal(collection, 0, true);

	public virtual bool Equals(G.IEnumerable<T>? collection, MpzT index, bool toEnd = false) =>
		EqualsInternal(collection, index, toEnd);

	public override bool Equals(object? obj) => obj switch
	{
		null => false,
		G.IEnumerable<T> enumerable => Equals(enumerable),
		IEquatable<G.IEnumerable<T>> iqen => iqen.Equals(this),
		_ => false,
	};

	private protected virtual bool EqualsInternal(G.IEnumerable<T>? collection, MpzT index, bool toEnd = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list && list is not (FastDelHashSet<T> or ParallelHashSet<T>))
			return EqualsToList(list, index, toEnd);
		else
			return EqualsToNonList(collection, index, toEnd);
	}

	private protected virtual bool EqualsToList(G.IList<T> list, MpzT index, bool toEnd = false)
	{
		if (index > Length - list.Count)
			return false;
		if (toEnd && index < Length - list.Count)
			return false;
		for (var i = 0; i < list.Count; i++, index++)
			if (!(GetInternal(index)?.Equals(list[i]) ?? list[i] == null))
				return false;
		return true;
	}

	private protected virtual bool EqualsToNonList(G.IEnumerable<T> collection, MpzT index, bool toEnd = false)
	{
		if (collection.TryGetLengthEasily(out var length))
		{
			if (index > Length - length)
				return false;
			if (toEnd && index < Length - length)
				return false;
		}
		foreach (var item in collection)
		{
			if (index >= Length || !(GetInternal(index)?.Equals(item) ?? item == null))
				return false;
			index++;
		}
		return !toEnd || index == Length;
	}

	public virtual Enumerator GetEnumerator() => new(this);

	G.IEnumerator<T> G.IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public override int GetHashCode()
	{
		var hash = 486187739;
		foreach (var item in this)
			hash = (hash * 16777619) ^ (item?.GetHashCode() ?? 0);
		return hash;
	}

	private protected abstract T GetInternal(MpzT index, bool invoke = true);

	private protected virtual MpzT GetProperCapacity(MpzT min) => min;

	public virtual TCertain GetRange(MpzT index, bool alwaysCopy = false) => GetRange(index, Length - index, alwaysCopy);

	public virtual TCertain GetRange(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Получаемый диапазон выходит за текущий размер коллекции.");
		return GetRangeInternal(index, length, alwaysCopy);
	}

	private protected virtual TCertain GetRangeInternal(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		if (length == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && length == Length && this is TCertain thisList)
			return thisList;
		var list = CapacityCreator(length);
		CopyToInternal(index, list, 0, length);
		list.Length = length;
#if VERIFY
		Verify();
		list.Verify();
#endif
		return list;
	}

	public virtual MpzT IndexOf(T item) => IndexOf(item, 0, Length);

	public virtual MpzT IndexOf(T item, MpzT index) => IndexOf(item, index, Length - index);

	public virtual MpzT IndexOf(T item, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Length);
		if (length < 0 || index > Length - length)
			throw new ArgumentOutOfRangeException(nameof(length));
		try
		{
			throw new SlowOperationException();
		}
		catch
		{
		}
		for (MpzT i = new(index); i < index + length; i++)
			if (GetInternal(i)?.Equals(item) ?? false)
				return i;
		return -1;
	}

	void IBigList<T>.Insert(MpzT index, T item) => Add(item);

	[Obsolete("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо использовать Remove()"
		+ " с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().", true)]
	public virtual TCertain Remove(MpzT index) =>
		throw new NotSupportedException("Этот метод был удален как имеющий неоднозначное название. Взамен необходимо"
			+ " использовать Remove() с указанием диапазона, RemoveAt(), RemoveEnd() или RemoveValue().");

	public virtual TCertain Remove(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveInternal(index, length);
		return (TCertain)this;
	}

	private protected abstract void RemoveInternal(MpzT index, MpzT length);

	public virtual TCertain RemoveAt(MpzT index)
	{
		if ((uint)index >= (uint)Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		RemoveAtInternal(index);
		return (TCertain)this;
	}

	void IBigList<T>.RemoveAt(MpzT index) => RemoveAt(index);

	private protected virtual void RemoveAtInternal(MpzT index)
	{
		var this2 = (TCertain)this;
		Length -= 1;
		if (index < Length)
			CopyToInternal(index + 1, this2, index, Length - index);
		SetInternal(Length, default!);
#if VERIFY
		Verify();
#endif
	}

	public virtual TCertain RemoveEnd(MpzT index)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (index > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveEndInternal(index);
		return (TCertain)this;
	}

	private protected abstract void RemoveEndInternal(MpzT index);

	public virtual bool RemoveValue(T item)
	{
		var index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAtInternal(index);
			return true;
		}
#if VERIFY
		Verify();
#endif
		return false;
	}

	public virtual TCertain Replace(G.IEnumerable<T> collection)
	{
		if (collection is IBigCollection<T> col)
		{
			col.CopyTo(this, 0);
			Length = col.Length;
		}
		else
		{
			MpzT i = 0;
			foreach (var item in collection)
			{
				SetInternal(i, item);
				i++;
			}
			Length = i;
		}
#if VERIFY
		Verify();
#endif
		return (TCertain)this;
	}

	private protected abstract void SetInternal(MpzT index, T value);

	public virtual TCertain SetRange(MpzT index, G.IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (index < 0 || index > Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		if (index + bigList.Length > Length)
			throw new ArgumentException("Устанавливаемая последовательность выходит за текущий размер коллекции.");
		return SetRangeInternal(index, bigList);
	}

	private protected virtual TCertain SetRangeInternal(MpzT index, TCertain bigList)
	{
		var length = bigList.Length;
		if (length == 0)
			return (TCertain)this;
		EnsureCapacity(index + length);
		var this2 = (TCertain)this;
		if (length > 0)
			bigList.CopyToInternal(0, this2, index, length);
#if VERIFY
		Verify();
#endif
		return this2;
	}

	public virtual T[] ToArray()
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		var length = (int)Length;
		var array = GC.AllocateUninitializedArray<T>(length);
		CopyToInternal(0, array, 0, length);
		return array;
	}

	public virtual TCertain TrimExcess()
	{
		var threshold = Capacity * 9 / 10;
		if (Length < threshold)
			Capacity = Length;
#if VERIFY
		Verify();
#endif
		return (TCertain)this;
	}
#if VERIFY

	private protected abstract void Verify();

	private protected abstract void VerifySingle();
#endif

	[Serializable]
	public struct Enumerator : G.IEnumerator<T>, IEnumerator
	{
		private readonly BaseBigList<T, TCertain, TLow> list;
		private MpzT index;
		private T current;

		internal Enumerator(BaseBigList<T, TCertain, TLow> list)
		{
			this.list = list;
			index = 0;
			current = default!;
		}

		public readonly void Dispose() => GC.SuppressFinalize(this);

		public bool MoveNext()
		{
			if (index >= list.Length)
				return MoveNextRare();
			try
			{
				current = list.GetInternal(index);
				index++;
				return true;
			}
			catch
			{
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			index = list.Length + 1;
			current = default!;
			return false;
		}

		public readonly T Current => current;

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list.Length + 1)
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
public abstract class BigList<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow>
	where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected List<TCertain>? high;
	private protected BigSumList? highLength;
	private protected TCertain? parent;
	private protected MpzT _capacity = 0;
	private protected MpzT fragment = 1;
	private protected bool isReversed;

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

	public BigList(G.IEnumerable<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1)
		: this(collection == null ? throw new ArgumentNullException(nameof(collection))
			  : collection.TryGetLengthEasily(out var length) ? length : 0,
			  leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromEnumerable(collection);
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

	public BigList(MpzT capacity, G.IEnumerable<T> collection, int subbranchesBitLength = -1,
		int leafSizeBitLength = -1) : this(capacity, leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromEnumerable(collection);
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

	public BigList(ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(collection.Length, leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromList(collection.ToArray());
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

	public BigList(MpzT capacity, ReadOnlySpan<T> collection, int subbranchesBitLength = -1, int leafSizeBitLength = -1) : this(RedStarLinq.Max(capacity, collection.Length), leafSizeBitLength, subbranchesBitLength)
	{
		ConstructFromList(collection.ToArray());
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

	public override MpzT Capacity
	{
		get => _capacity;
		set
		{
			var this2 = (TCertain)this;
			ArgumentOutOfRangeException.ThrowIfLessThan(value, Length);
			if (value == _capacity)
				return;
			if (value == 0)
			{
				low = new();
				high = null;
				highLength = null;
			}
			else if (value <= LeafSize)
			{
				if (high != null && highLength != null)
					ReduceCapacityExponential(LeafSize);
				if (high != null && highLength != null)
					Compactify();
				var first = this;
				for (; first.high != null; first = first.high[0], first.parent?.high?.Dispose())
				{
					Debug.Assert(first.highLength != null);
					first.high[1..].ForEach(x => x.Dispose());
					first.highLength.Dispose();
				}
				Debug.Assert(first.low != null);
				low = first.low;
				var value2 = (int)value;
				low.Capacity = value2;
				high = null;
				highLength = null;
			}
			else if (low != null)
			{
				fragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - LeafSizeBitLength,
					SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
				while (fragment << SubbranchesBitLength < value)
					fragment <<= SubbranchesBitLength;
				var highCount = (int)GetArrayLength(value, fragment);
				high = new(highCount);
				for (MpzT i = 0; i < value / fragment; i++)
				{
					high.Add(CapacityCreator(fragment));
					high[^1].parent = this2;
				}
				if (value % fragment != 0)
				{
					high.Add(CapacityCreator(value % fragment));
					high[^1].parent = this2;
				}
				var preservedLow = low;
				low = null;
				Length = 0;
				var first = this;
				for (; first.high != null; first = first.high[0])
					first.highLength = [preservedLow.Length];
				ArgumentNullException.ThrowIfNull(first.low);
				first.low.AddRange(preservedLow);
				if (preservedLow.Length != 0)
					first.Length = preservedLow.Length;
			}
			else if (high != null && highLength != null)
			{
				var newFragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - LeafSizeBitLength,
					SubbranchesBitLength) - 1) * SubbranchesBitLength + LeafSizeBitLength;
				while (newFragment << SubbranchesBitLength < value)
					newFragment <<= SubbranchesBitLength;
				if (newFragment > fragment)
					IncreaseCapacityExponential(value, newFragment);
				else if (newFragment < fragment)
				{
					ReduceCapacityExponential(newFragment);
					AddCapacity((fragment << SubbranchesBitLength) - _capacity);
					ReduceCapacityLinear(value);
				}
				else if (value > _capacity)
					IncreaseCapacityLinear(value);
				else
					ReduceCapacityLinear(value);
			}
			AddCapacity(value - _capacity);
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
	}

	private protected virtual int LeafSizeBitLength { get; init; } = 16;

	private protected virtual int LeafSize => 1 << LeafSizeBitLength;

	private protected virtual int SubbranchesBitLength { get; init; } = 16;

	private protected virtual int Subbranches => 1 << SubbranchesBitLength;

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

	private protected virtual TCertain Root => parent == null ? (TCertain)this : parent.Root;

	public override TCertain Add(T item)
	{
		var this2 = (TCertain)this;
		EnsureCapacity(Length + 1);
	start:
		if (low != null)
		{
			low.Add(item);
			Length += 1;
		}
		else if (high != null && highLength != null)
		{
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
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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
		return this2;
	}

	private protected virtual void AddCapacity(MpzT increment)
	{
		if (increment == 0)
			return;
		for (var list = this; list != null; list = list.parent)
			list._capacity += increment;
	}

	private protected override void ClearInternal(bool verify = true)
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

	private protected override void ClearInternal(MpzT index, MpzT length)
	{
		if (low != null)
			low.Clear((int)index, (int)length);
		else if (high != null && highLength != null)
		{
			var quotient = highLength.IndexOfNotGreaterSum(index, out var remainder);
			var quotient2 = highLength.IndexOfNotGreaterSum(index + length - 1);
			if (quotient == quotient2)
			{
				high[quotient].ClearInternal(remainder, length);
				return;
			}
			var previousPart = highLength[quotient] - remainder;
			high[quotient].ClearInternal(remainder, previousPart);
			for (var i = quotient + 1; i < quotient2; i++)
			{
				high[i].ClearInternal(0, highLength[i]);
				previousPart += highLength[i];
			}
			high[quotient2].ClearInternal(0, length - previousPart);
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

	private protected virtual void Compactify()
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
				temp.RemoveEndInternal(RedStarLinq.Min(high[^1].Length, temp.Length));
				high[^1].CopyToInternal(0, temp, 0, high[^1].Length);
				high[^1].ClearInternal(false);
				highLength[i - 1] += highLengthElementI;
				highLength.RemoveAt(i);
				i--;
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

	private protected virtual void ConstructFromCapacity(MpzT capacity)
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
			var quotient = capacity.Divide(fragment, out var remainder);
			var highCount = (int)GetArrayLength(capacity, fragment);
			high = new(highCount);
			highLength = [];
			for (MpzT i = 0; i < quotient; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
				AddCapacity(fragment);
			}
			if (remainder != 0)
			{
				high.Add(CapacityCreator(remainder));
				high[^1].parent = this2;
				AddCapacity(remainder);
			}
		}
		Length = 0;
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

	private void ConstructFromEnumerable(G.IEnumerable<T> collection)
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

	private protected virtual void ConstructFromList(G.IReadOnlyList<T> list)
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

	private protected virtual void ConstructFromListFromScratch(G.IReadOnlyList<T> list)
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

	private protected static void CheckParams(List<TCertain> source, MpzT sourceIndex, List<TCertain> destination, MpzT destinationIndex, MpzT length)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source), "Исходный массив не может быть нулевым.");
		if (source.Capacity == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(source));
		if (destination == null)
			throw new ArgumentNullException(nameof(destination), "Целевой массив не может быть нулевым.");
		if (destination.Capacity == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destination));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > source.Sum(x => x.Capacity))
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destination.Sum(x => x.Capacity))
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
	}

	private protected override void CopyToInternal(MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length)
	{
		CheckParams(sourceIndex, destination, destinationIndex, length);
#if VERIFY
		MpzT oldLength = new(destination.Length);
#endif
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (this == destination && sourceIndex == destinationIndex)
			return;
		var fragment = this.fragment;
		if (low != null && destination.low != null)
		{
			if (destination.low.Length < destinationIndex + length)
				destination.low.Resize((int)(destinationIndex + length));
			low.CopyRangeTo((int)sourceIndex, destination.low, (int)destinationIndex, (int)length);
			destination.Length = RedStarLinq.Max(destination.Length, destinationIndex + length);
#if VERIFY
			Verify();
			destination.Verify();
#endif
			return;
		}
		else if (destination.low != null && high != null && highLength != null)
		{
			var quotient = highLength.IndexOfNotGreaterSum(sourceIndex, out var remainder);
			var quotient2 = highLength.IndexOfNotGreaterSum(sourceIndex + length - 1, out _);
			var index = isReversed ? highLength.Length - 1 - quotient : quotient;
			var index2 = isReversed ? highLength.Length - 1 - quotient2 : quotient2;
			if (index == index2)
				high[index].CopyToInternal(remainder, destination, destinationIndex, length);
			else if (sourceIndex >= destinationIndex)
			{
				var previousPart = (highLength.Length > index ? highLength[index] : 0) - remainder;
				high[index].CopyToInternal(remainder, destination, destinationIndex, previousPart);
				for (var i = index + (isReversed ? -1 : 1); isReversed ? i > index2 : i < index2; i += isReversed ? -1 : 1)
				{
					high[i].CopyToInternal(0, destination, destinationIndex + previousPart, highLength.Length > i ? highLength[i] : 0);
					previousPart += highLength.Length > i ? highLength[i] : 0;
				}
				high[index2].CopyToInternal(0, destination, destinationIndex + previousPart, length - previousPart);
			}
			else
			{
				var previousPart = (highLength.Length > index ? highLength[index] : 0) - remainder;
				for (var i = index + (isReversed ? -1 : 1); isReversed ? i > index2 : i < index2; i += isReversed ? -1 : 1)
					previousPart += highLength[i];
				high[index2].CopyToInternal(0, destination, destinationIndex + previousPart, length - previousPart);
				for (var i = index2 - (isReversed ? -1 : 1); isReversed ? i < index : i > index; i -= isReversed ? -1 : 1)
				{
					previousPart -= highLength.Length > i ? highLength[i] : 0;
					high[i].CopyToInternal(0, destination, destinationIndex + previousPart, highLength.Length > i ? highLength[i] : 0);
				}
				high[index].CopyToInternal(remainder, destination, destinationIndex, previousPart);
			}
			destination.highLength?.SetOrAdd(index, destination.highLength.Length
				> index ? destination.highLength[index] : destinationIndex + length);
#if VERIFY
			Debug.Assert(destination.Length == RedStarLinq.Max(oldLength, destinationIndex + length));
			Verify();
			destination.Verify();
#endif
			return;
		}
		else if ((low != null || destination.fragment > fragment) && destination.high != null && destination.highLength != null)
		{
			fragment = destination.fragment;
		destinationFragmentBigger:
			var index = destination.highLength.IndexOfNotGreaterSum(destinationIndex, out var remainder);
			var index2 = destination.highLength.IndexOfNotGreaterSum(destinationIndex + length - 1, out var remainder2);
			if (index != 0 && index == destination.highLength.Length && remainder == 0 && destination.highLength[^1] != fragment)
			{
				index--;
				remainder = destination.highLength[^1];
			}
			if (index2 != 0 && index2 == destination.highLength.Length)
			{
				if (remainder2 >= fragment - destination.highLength[^1])
					remainder2 -= fragment - destination.highLength[^1];
				else
				{
					index2--;
					remainder2 += destination.highLength[^1];
				}
			}
			while (remainder2 >= fragment)
			{
				index2++;
				remainder2 -= fragment;
			}
			if (index2 >= destination.high.Length || destination.highLength.Length != 0 && destination.highLength.ValuesSum + fragment - destination.highLength[^1] + (destination.high.Length - destination.highLength.Length - 1) * fragment + destination.high[^1].Capacity < destinationIndex + length)
			{
				destination.Compactify();
				goto destinationFragmentBigger;
			}
			if (index == index2)
			{
				CopyToInternal(sourceIndex, destination.high[index], remainder, length);
				destination.highLength.SetOrAdd(index, RedStarLinq.Max(destination.highLength.Length
					> index ? destination.highLength[index] : 0, remainder + length));
			}
			else if (sourceIndex >= destinationIndex)
			{
				var previousPart = RedStarLinq.Min((destination.highLength.Length > index
					? destination.highLength[index] : fragment) - remainder, length);
				CopyToInternal(sourceIndex, destination.high[index], remainder, previousPart);
				destination.highLength.SetOrAdd(index, RedStarLinq.Max(destination.highLength.Length
					> index ? destination.highLength[index] : 0, remainder + previousPart));
				for (var i = index + 1; i < index2; i++)
				{
					CopyToInternal(sourceIndex + previousPart, destination.high[i], 0, destination.highLength.Length > i ? destination.highLength[i] : fragment);
					destination.highLength.SetOrAdd(i, destination.highLength.Length > i ? destination.highLength[i] : fragment);
					previousPart += destination.highLength.Length > i ? destination.highLength[i] : 0;
				}
				CopyToInternal(sourceIndex + previousPart, destination.high[index2], 0, length - previousPart);
				if (length != previousPart)
					destination.highLength.SetOrAdd(index2, RedStarLinq.Max(destination.highLength.Length > index2 ? destination.highLength[index2] : 0, length - previousPart));
			}
			else
			{
				for (var i = destination.highLength.Length; i < index2; i++)
					destination.highLength.Add(1);
				var previousPart = RedStarLinq.Min(fragment - remainder, length) + (index2 - index - 1) * fragment;
				CopyToInternal(sourceIndex + previousPart, destination.high[index2], 0, length - previousPart);
				if (length != previousPart)
					destination.highLength.SetOrAdd(index2, RedStarLinq.Max(destination.highLength.Length > index2 ? destination.highLength[index2] : 0, length - previousPart));
				for (var i = index2 - 1; i > index; i--)
				{
					previousPart -= fragment;
					CopyToInternal(sourceIndex + previousPart, destination.high[i], 0, fragment);
					destination.highLength[i] = fragment;
				}
				CopyToInternal(sourceIndex, destination.high[index], remainder, previousPart);
				destination.highLength[index] = RedStarLinq.Max(destination.highLength.Length > index ? destination.highLength[index] : 0, remainder + previousPart);
			}
#if VERIFY
			Debug.Assert(destination.Length == RedStarLinq.Max(oldLength, destinationIndex + length));
			Debug.Assert(this.Skip((int)sourceIndex).Take((int)length).Equals(destination.Skip((int)destinationIndex).Take((int)length)));
			Verify();
			destination.Verify();
#endif
			return;
		}
		if (!(high != null && highLength != null && destination.high != null && destination.highLength != null))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		fragment = destination.fragment;
		var compactified = false;
	sameFragment:
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		var sourceIntIndex = highLength.IndexOfNotGreaterSum(sourceIndex, out var sourceBitsIndex);               // Целый индекс в исходном массиве.
		var destinationIntIndex = destination.highLength.IndexOfNotGreaterSum(destinationIndex, out var destinationBitsIndex);     // Целый индекс в целевом массиве.
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		var sourceEndIntIndex = highLength.IndexOfNotGreaterSum(sourceEndIndex, out var sourceEndBitsIndex);  // Индекс инта последнего бита.
		while (sourceEndBitsIndex >= this.fragment)
		{
			sourceEndIntIndex++;
			sourceEndBitsIndex -= this.fragment;
		}
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		var destinationEndIntIndex = destination.highLength.IndexOfNotGreaterSum(destinationEndIndex, out var destinationEndBitsIndex);  // Индекс инта последнего бита.
		if (destination.highLength.Length != 0 && destinationEndIntIndex == destination.highLength.Length)
			destinationEndBitsIndex -= fragment - destination.highLength[^1];
		while (destinationEndBitsIndex >= fragment)
		{
			destinationEndIntIndex++;
			destinationEndBitsIndex -= fragment;
		}
		if (destinationIntIndex != 0 && destinationIntIndex == destination.highLength.Length && destinationBitsIndex == 0 && destination.highLength[destinationIntIndex - 1] != fragment)
		{
			destinationIntIndex--;
			destinationBitsIndex = destination.highLength[^1];
		}
		if (destinationEndIntIndex != 0 && destinationEndBitsIndex < 0)
		{
			destinationEndIntIndex--;
			destinationEndBitsIndex += fragment;
		}
		if (destinationEndIntIndex >= destination.high.Length || destinationEndBitsIndex < 0
			|| destination.highLength.Length != 0 && destination.highLength.ValuesSum + fragment - destination.highLength[^1]
			+ (destination.high.Length - destination.highLength.Length - 1) * fragment
			+ destination.high[^1].Capacity < destinationIndex + length)
		{
			Debug.Assert(!compactified);
			destination.Compactify();
			compactified = true;
			goto sameFragment;
		}
		else
		{
			Debug.Assert(sourceIntIndex >= 0 && sourceIntIndex < highLength.Length
				&& sourceEndIntIndex >= 0 && sourceEndIntIndex < highLength.Length
				&& destinationIntIndex >= 0 && destinationIntIndex < destination.high.Length
				&& destinationEndIntIndex >= 0 && destinationEndIntIndex < destination.high.Length
				&& sourceBitsIndex >= 0 && sourceEndBitsIndex >= 0 && destinationBitsIndex >= 0 && destinationEndBitsIndex >= 0);
			if (sourceIndex >= destinationIndex || destinationIndex == destination.Length)
				goto l1;
			else
			{
				for (var i = destination.highLength.Length; i < destinationEndIntIndex + 1; i++)
					destination.highLength.Add(1);
				goto l2;
			}
		}
	l1:
		var sourceCurrentBitsIndex = sourceBitsIndex;
		var destinationCurrentBitsIndex = destinationBitsIndex;
		var sourceCurrentIntIndex = sourceIntIndex;
		var destinationCurrentIntIndex = destinationIntIndex;
		var bitsIndexDiff = (sourceBitsIndex - destinationBitsIndex).Abs();
		bitsIndexDiff = RedStarLinq.Max(bitsIndexDiff, fragment - bitsIndexDiff);
		MpzT step = 0;
		var leftLength = length;
		while (leftLength > 0)
		{
			var destinationMax = destination.highLength.Length > destinationCurrentIntIndex + 1
				? destination.highLength[destinationCurrentIntIndex] : fragment;
			step = RedStarLinq.Min(highLength[sourceCurrentIntIndex] - step, bitsIndexDiff,
				(destinationCurrentIntIndex == destinationEndIntIndex ? destinationEndBitsIndex + 1 : fragment) - step,
				highLength[sourceCurrentIntIndex] - sourceCurrentBitsIndex, destinationMax - destinationCurrentBitsIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			high[sourceCurrentIntIndex].CopyToInternal(sourceCurrentBitsIndex,
				destination.high[destinationCurrentIntIndex], destinationCurrentBitsIndex, step);
			var sourceThresholdReached = (sourceCurrentBitsIndex += step) == highLength[sourceCurrentIntIndex];
			var destinationThresholdReached = (destinationCurrentBitsIndex += step) == destinationMax;
			leftLength -= step;
			if (sourceThresholdReached)
			{
				sourceCurrentIntIndex++;
				sourceCurrentBitsIndex = 0;
			}
			if (destinationThresholdReached)
			{
				destination.highLength.SetOrAdd(destinationCurrentIntIndex, destinationCurrentBitsIndex);
				destinationCurrentIntIndex++;
				destinationCurrentBitsIndex = 0;
			}
		}
		destination.highLength.SetOrAdd(destinationEndIntIndex, RedStarLinq.Max(destination.highLength.Length
			> destinationEndIntIndex ? destination.highLength[destinationEndIntIndex] : 0, destinationEndBitsIndex + 1));
#if VERIFY
		Debug.Assert(destination.Length == RedStarLinq.Max(oldLength, destinationIndex + length));
		Verify();
		destination.Verify();
#endif
		return;
	l2:
		sourceCurrentBitsIndex = sourceEndBitsIndex + 1;
		destinationCurrentBitsIndex = destinationEndBitsIndex + 1;
		sourceCurrentIntIndex = sourceEndIntIndex;
		destinationCurrentIntIndex = destinationEndIntIndex;
		bitsIndexDiff = (sourceBitsIndex - destinationBitsIndex).Abs();
		bitsIndexDiff = RedStarLinq.Max(bitsIndexDiff, fragment - bitsIndexDiff);
		step = 0;
		destination.highLength[destinationEndIntIndex] = RedStarLinq.Max(destination.highLength.Length > destinationEndIntIndex
			? destination.highLength[destinationEndIntIndex] : 0, destinationEndBitsIndex + 1);
		leftLength = length;
		while (leftLength > 0)
		{
			step = RedStarLinq.Min(highLength[sourceCurrentIntIndex] - step, bitsIndexDiff,
				(destinationCurrentIntIndex == destinationEndIntIndex ? destinationEndBitsIndex + 1 : fragment)
				- step, sourceCurrentBitsIndex, destinationCurrentBitsIndex, leftLength);
			if (step <= 0)
			{
				step = 0;
				continue;
			}
			var sourceThresholdReached = (sourceCurrentBitsIndex -= step) == 0;
			var destinationThresholdReached = (destinationCurrentBitsIndex -= step) == 0;
			high[sourceCurrentIntIndex].CopyToInternal(sourceCurrentBitsIndex,
				destination.high[destinationCurrentIntIndex], destinationCurrentBitsIndex, step);
			leftLength -= step;
			if (sourceThresholdReached)
			{
				sourceCurrentIntIndex--;
				sourceCurrentBitsIndex = sourceCurrentIntIndex >= sourceIntIndex ? highLength[sourceCurrentIntIndex] : fragment;
			}
			if (destinationThresholdReached)
			{
				destinationCurrentIntIndex--;
				destination.highLength[destinationCurrentIntIndex] = destinationCurrentBitsIndex = fragment;
			}
		}
#if VERIFY
		Debug.Assert(destination.Length <= RedStarLinq.Max(oldLength, destinationIndex + length));
		Verify();
		destination.Verify();
#endif
	}

	private protected void CheckParams(MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length)
	{
		if (Capacity == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.");
		if (destination == null)
			throw new ArgumentNullException(nameof(destination), "Целевой массив не может быть нулевым.");
		if (destination.Capacity == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destination));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > Capacity)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destination.Capacity)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
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
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
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
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length)
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
		isReversed = false;
		GC.SuppressFinalize(this);
	}

	private protected virtual TLow GetFirstLists()
	{
		if (low != null)
			return low;
		else if (high != null)
			return high[0].GetFirstLists();
		else
			return new();
	}

	private protected override T GetInternal(MpzT index, bool invoke = true)
	{
		if (low != null)
			return low[(int)(isReversed ? Length - 1 - index : index)];
		else if (high != null && highLength != null)
		{
			var quotient = highLength.IndexOfNotGreaterSum(index, out var remainder);
			return high[isReversed ? high.Length - 1 - quotient : quotient].GetInternal(remainder, invoke);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected virtual void IncreaseCapacityExponential(MpzT value, MpzT targetFragment)
	{
		var this2 = (TCertain)this;
		Debug.Assert(high != null && highLength != null);
		do
		{
			var newFragment = fragment << SubbranchesBitLength;
			if (_capacity < newFragment)
				IncreaseCapacityLinear(newFragment);
			var highCount = (int)RedStarLinq.Min(GetArrayLength(value, newFragment), Subbranches);
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

	private protected virtual void IncreaseCapacityLinear(MpzT value)
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

	private protected virtual void ReduceCapacityExponential(MpzT newFragment)
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
			oldHigh[1..].ForEach(x => x.Dispose());
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

	private protected virtual void ReduceCapacityLinear(MpzT value)
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
		high.RemoveEnd(highCount);
		var leftCapacity = value % fragment;
		if (leftCapacity == 0)
			leftCapacity = fragment;
		if (high[^1].Length > leftCapacity)
			Compactify();
		high[^1].Capacity = leftCapacity == 0 ? fragment : leftCapacity;
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

	public override TCertain Remove(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		RemoveInternal(index, length);
		return (TCertain)this;
	}

	private protected override void RemoveAtInternal(MpzT index)
	{
		if (low != null)
		{
			low.RemoveAt((int)index);
			Length -= 1;
		}
		else if (high != null && highLength != null)
		{
			var quotient = highLength.IndexOfNotGreaterSum(index, out var remainder);
			if (quotient != highLength.Length - 1 && highLength[quotient] == 1)
			{
				var temp = high[quotient];
				var offsetFromEnd = Capacity == high.Length * fragment ? 0 : 1;
				high.CopyRangeTo(quotient + 1, high, quotient, high.Length - quotient - offsetFromEnd - 1);
				high[^(offsetFromEnd + 1)] = temp;
				temp.RemoveEndInternal(RedStarLinq.Min(high[^1].Length, temp.Length));
				high[^1].CopyToInternal(0, temp, 0, high[^1].Length);
				high[^1].ClearInternal(false);
				highLength.RemoveAt(quotient);
			}
			else
			{
				highLength.Decrease(quotient);
				high[isReversed ? high.Length - 1 - quotient : quotient].RemoveAtInternal(remainder);
			}
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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

	private protected override void RemoveEndInternal(MpzT index)
	{
		if (index == Length)
			return;
		if (low != null)
		{
			low.RemoveEnd((int)index);
			Length = index;
		}
		else if (high != null && highLength != null)
		{
			var intIndex = highLength.IndexOfNotGreaterSum(index, out var bitsIndex);
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
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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

	private protected override void RemoveInternal(MpzT index, MpzT length)
	{
		var this2 = (TCertain)this;
		if (length == 0)
			return;
		if (low != null)
		{
			low.Remove((int)index, (int)length);
			Length -= length;
#if VERIFY
			Verify();
#endif
			return;
		}
		Debug.Assert(high != null && highLength != null);
		var endIndex = index + length - 1;
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
				temp.RemoveEndInternal(RedStarLinq.Min(high[^1].Length, temp.Length));
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
				tempRange[0].RemoveEndInternal(RedStarLinq.Min(high[^1].Length, tempRange[0].Length));
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

	private protected override void SetInternal(MpzT index, T value)
	{
		if (low != null)
			low[(int)(isReversed ? Length - 1 - index : index)] = value;
		else if (high != null && highLength != null)
		{
			var quotient = highLength.IndexOfNotGreaterSum(index, out var remainder);
			high[isReversed ? high.Length - 1 - quotient : quotient].SetInternal(remainder, value);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
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

	public virtual TCertain SetOrAdd(MpzT index, T value)
	{
		if ((uint)index > (uint)Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		return SetOrAddInternal(index, value);
	}

	private protected virtual TCertain SetOrAddInternal(MpzT index, T value)
	{
		if (index == Length)
			return Add(value);
		SetInternal(index, value);
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
		return (TCertain)this;
	}
#if VERIFY

	private protected override void Verify() => Verify(Root);

	private protected virtual void Verify(TCertain item)
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

	private protected override void VerifySingle()
	{
		Debug.Assert(low != null ^ (high != null || highLength != null));
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
			Debug.Assert(high.Capacity <= Subbranches);
			Debug.Assert(high.Length < 2 || high[..^1].All(x => x.Capacity == fragment));
			Debug.Assert((high.Length - 1) * fragment + high[^1].Capacity == Capacity);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
#endif
}
