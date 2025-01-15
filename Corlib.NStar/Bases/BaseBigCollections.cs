
namespace Corlib.NStar;

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

	private protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected abstract Func<IEnumerable<T>, TLow> CollectionLowCreator { get; }

	private protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public abstract TCertain Add(T item);

	void IBigCollection<T>.Add(T item) => Add(item);

	public virtual TCertain AddRange(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		var this2 = (TCertain)this;
		var length = bigList.Length;
		if (length == 0)
			return this2;
		bigList.CopyToInternal(0, this2, Length, length);
		if (collection is not TCertain)
			bigList.Dispose();
#if VERIFY
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

	private protected abstract void ClearInternal();

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

	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, Length);

	public virtual bool Contains(IEnumerable<T> collection, MpzT index) => Contains(collection, index, Length - index);

	public virtual bool Contains(IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Проверяемый диапазон выходит за текущий размер коллекции.");
		ArgumentNullException.ThrowIfNull(collection);
		return ContainsInternal(collection, index, length);
	}

	public virtual bool Contains(TCertain list) => Contains((IEnumerable<T>)list, 0, Length);

	public virtual bool Contains(TCertain list, MpzT index) => Contains((IEnumerable<T>)list, index, Length - index);

	public virtual bool Contains(TCertain list, MpzT index, MpzT length) => Contains((IEnumerable<T>)list, index, length);

	public virtual bool ContainsAny(IEnumerable<T> collection) => ContainsAny(collection, 0, Length);

	public virtual bool ContainsAny(IEnumerable<T> collection, MpzT index) => ContainsAny(collection, index, Length - index);

	public virtual bool ContainsAny(IEnumerable<T> collection, MpzT index, MpzT length)
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

	public virtual bool ContainsAny(TCertain list) => ContainsAny((IEnumerable<T>)list, 0, Length);

	public virtual bool ContainsAny(TCertain list, MpzT index) => ContainsAny((IEnumerable<T>)list, index, Length - index);

	public virtual bool ContainsAny(TCertain list, MpzT index, MpzT length) => ContainsAny((IEnumerable<T>)list, index, length);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, Length);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, MpzT index) => ContainsAnyExcluding(collection, index, Length - index);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, MpzT index, MpzT length)
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

	public virtual bool ContainsAnyExcluding(TCertain list) => ContainsAnyExcluding((IEnumerable<T>)list, 0, Length);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index) => ContainsAnyExcluding((IEnumerable<T>)list, index, Length - index);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index, MpzT length) => ContainsAnyExcluding((IEnumerable<T>)list, index, length);

	private protected virtual bool ContainsInternal(IEnumerable<T> collection, MpzT index, MpzT length)
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
			if (this[index + i]?.Equals(list[j]) ?? list[j] == null)
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

	private protected abstract void CopyToInternal(MpzT sourceIndex, TCertain destination, MpzT destinationIndex, MpzT length);

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
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(MpzT index, TCertain list, MpzT listIndex, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Копируемая последовательность выходит за размер целевого массива.");
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(MpzT index, T[] array, int arrayIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
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
			if (newCapacity < min) newCapacity = GetProperCapacity(min);
			Capacity = newCapacity;
		}
	}

	public virtual bool Equals(IEnumerable<T>? collection) => EqualsInternal(collection, 0, true);

	public virtual bool Equals(IEnumerable<T>? collection, MpzT index, bool toEnd = false) => EqualsInternal(collection, index, toEnd);

	public override bool Equals(object? obj) => obj switch
	{
		null => false,
		IEnumerable<T> enumerable => Equals(enumerable),
		IEquatable<IEnumerable<T>> iqen => iqen.Equals(this),
		_ => false,
	};

	private protected virtual bool EqualsInternal(IEnumerable<T>? collection, MpzT index, bool toEnd = false)
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

	private protected virtual bool EqualsToNonList(IEnumerable<T> collection, MpzT index, bool toEnd = false)
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

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

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
			for (MpzT i = new(index); i < index + length; i++)
				if (GetInternal(i)?.Equals(item) ?? false)
					return i;
			return -1;
		}
	}

	void IBigList<T>.Insert(MpzT index, T item) => Add(item);

	public virtual TCertain Remove(MpzT index) => Remove(index, Length - index);

	public virtual TCertain Remove(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Length)
			throw new ArgumentException("Удаляемый диапазон выходит за текущий размер коллекции.");
		var this2 = (TCertain)this;
		if (length > 0)
		{
			if (index < Length - length)
				CopyToInternal(index + length, this2, index, Length - index - length);
			RemoveFromEnd(Length - length);
		}
#if VERIFY
		Verify();
#endif
		return this2;
	}

	private protected abstract void RemoveFromEnd(MpzT index);

	public virtual TCertain RemoveAt(MpzT index)
	{
		if ((uint)index >= (uint)Length)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = (TCertain)this;
		Length -= 1;
		if (index < Length)
			CopyToInternal(index + 1, this2, index, Length - index);
		SetInternal(Length, default!);
#if VERIFY
		Verify();
#endif
		return this2;
	}

	void IBigList<T>.RemoveAt(MpzT index) => RemoveAt(index);

	public virtual bool RemoveValue(T item)
	{
		var index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
#if VERIFY
		Verify();
#endif
		return false;
	}

	public virtual TCertain Replace(IEnumerable<T> collection)
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

	public virtual TCertain SetRange(MpzT index, IEnumerable<T> collection)
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
		var threshold = (int)(Capacity * 0.9);
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
	public struct Enumerator : IEnumerator<T>, IEnumerator
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
public abstract class BigList<T, TCertain, TLow> : BaseBigList<T, TCertain, TLow> where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	private protected TLow? low;
	private protected List<TCertain>? high;
	private protected BigSumList? highLength;
	private protected TCertain? parent;
	private protected MpzT _capacity = 0;
	private protected MpzT fragment = 1;
	private protected bool isReversed;

	public BigList() : this(-1) { }

	public BigList(int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
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

	public BigList(MpzT capacity, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1)
	{
		if (capacityStepBitLength >= 2)
			CapacityStepBitLength = capacityStepBitLength;
		if (capacityFirstStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityFirstStepBitLength;
		else if (capacityStepBitLength >= 2)
			CapacityFirstStepBitLength = capacityStepBitLength;
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		ConstructFromCapacity(capacity);
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigList(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(collection == null ? throw new ArgumentNullException(nameof(collection)) : List<T>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : 0, capacityFirstStepBitLength, capacityStepBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigList(MpzT capacity, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(capacity, capacityFirstStepBitLength, capacityStepBitLength)
	{
		ConstructFromEnumerable(collection);
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigList(ReadOnlySpan<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(collection.Length, capacityFirstStepBitLength, capacityStepBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	public BigList(MpzT capacity, ReadOnlySpan<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(RedStarLinq.Max(capacity, collection.Length), capacityFirstStepBitLength, capacityStepBitLength)
	{
		ConstructFromList(collection.ToArray());
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
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
			else if (value <= CapacityFirstStep)
			{
				low = GetFirstLists();
				var value2 = (int)value;
				low.Capacity = value2;
				high = null;
				highLength = null;
			}
			else if (low != null)
			{
				fragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				while (fragment << CapacityStepBitLength < value)
					fragment <<= CapacityStepBitLength;
				var highCount = (int)GetArrayLength(value, fragment);
				high = new(highCount);
				highLength = [];
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
				for (; first.high != null; first = first.high[0]) ;
				ArgumentNullException.ThrowIfNull(first.low);
				first.low.AddRange(preservedLow);
				if (preservedLow.Length != 0)
				{
					first.Length = preservedLow.Length;
					highLength.Add(preservedLow.Length);
				}
			}
			else if (high != null && highLength != null)
			{
				var oldFragment = fragment;
				fragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				while (fragment << CapacityStepBitLength < value)
					fragment <<= CapacityStepBitLength;
				if (fragment > oldFragment)
					goto l0;
				if (fragment < oldFragment)
					goto l1;
				var oldHigh = high;
				var oldHighLength = highLength;
				var oldLength = Length;
				Length = 0;
				high = new((int)GetArrayLength(value, fragment));
				for (var i = 0; i < high.Capacity; i++)
				{
					high.Add(CapacityCreator(fragment));
					high[^1].parent = this2;
				}
				highLength = [];
				oldHigh.ForEach(x => x.parent = null);
				Copy(oldHigh, oldHighLength, 0, high, highLength, 0, oldLength, fragment);
				goto end;
			l0:
				do
				{
					var highCount = (int)RedStarLinq.Min(GetArrayLength(value, oldFragment << CapacityStepBitLength), CapacityStep);
					oldHigh = high;
					oldHighLength = highLength;
					oldLength = Length;
					high = new(highCount) { CapacityCreator(oldFragment << CapacityStepBitLength) };
					high[0].fragment = oldFragment;
					oldFragment <<= CapacityStepBitLength;
					high[0].low = null;
					oldHigh.ForEach(x => x.parent = null);
					Debug.Assert(high[0].high != null && high[0].highLength != null);
					Copy(oldHigh, oldHighLength, 0, high[0].high!, high[0].highLength!, 0, Length, high[0].fragment);
					high[0].parent = this2;
					new Chain(1, high.Capacity - 2).ForEach(_ => high.Add(CapacityCreator(oldFragment))[^1].parent = this2);
					high.Add(CapacityCreator(value % oldFragment == 0 ? oldFragment : value % oldFragment));
					high[^1].parent = this2;
					highLength = [Length];
				} while (oldFragment < fragment);
				goto end;
			l1:
				do
				{
					oldFragment >>= CapacityStepBitLength;
					high = high[0].high!;
					high.ForEach(x => x.parent = this2);
				} while (oldFragment > fragment);
				high[0].Capacity = value;
			}
		end:
			_capacity = value;
#if VERIFY
			if (low != null)
				Debug.Assert(Length == low.Length);
			else if (high != null && highLength != null)
			{
				Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
				Debug.Assert(high.All(x => x.parent == this));
			}
			else
				throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
			Verify();
#endif
		}
	}

	private protected virtual int CapacityFirstStepBitLength { get; init; } = 16;

	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	private protected virtual int CapacityStepBitLength { get; init; } = 16;

	private protected virtual int CapacityStep => 1 << CapacityStepBitLength;

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
		if (low != null)
		{
			low.Add(item);
			Length += 1;
		}
		else if (high != null && highLength != null)
		{
			var index = highLength.IndexOfNotGreaterSum(Length, out var sumExceedsBy);
			if (index != 0 && high[index - 1].Capacity != high[index - 1].Length)
				index--;
			if (high.Length == index)
			{
				Debug.Assert(high.Length * fragment < Capacity);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
			}
			high[index].Add(item);
			if (highLength.Length == index)
				highLength.Add(1);
			else
				highLength[index]++;
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
#if VERIFY
		if (low != null)
			Debug.Assert(Length == low.Length);
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
		return this2;
	}

	private protected override void ClearInternal()
	{
		if (low != null)
		{
			Length = 0;
			low.Clear();
		}
		else
		{
			high?.ForEach(x => x?.Clear());
			highLength?.Clear();
		}
#if VERIFY
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
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Verify();
#endif
	}

	private protected virtual void ConstructFromCapacity(MpzT capacity)
	{
		var this2 = (TCertain)this;
		if (capacity <= CapacityFirstStep)
		{
			low = CapacityLowCreator((int)capacity);
			high = null;
			highLength = null;
			fragment = 1;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			while (fragment << CapacityStepBitLength < capacity)
				fragment <<= CapacityStepBitLength;
			var quotient = capacity.Divide(fragment, out var remainder);
			var highCount = (int)GetArrayLength(capacity, fragment);
			high = new(highCount);
			highLength = [];
			for (MpzT i = 0; i < quotient; i++)
			{
				high.Add(CapacityCreator(fragment));
				high[^1].parent = this2;
			}
			if (remainder != 0)
			{
				high.Add(CapacityCreator(remainder));
				high[^1].parent = this2;
			}
		}
		Length = 0;
		_capacity = capacity;
#if VERIFY
		Verify();
#endif
	}

	private void ConstructFromEnumerable(IEnumerable<T> collection)
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
				ConstructFromCapacity(bigList.fragment * bigList.highLength.Length);
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
		if (list.Count <= CapacityFirstStep)
		{
			low = CollectionLowCreator(list);
			high = null;
			highLength = null;
			fragment = 1;
			Length = list.Count;
		}
		else
		{
			low = null;
			fragment = 1 << ((((MpzT)list.Count - 1).BitLength + CapacityStepBitLength - 1 - CapacityFirstStepBitLength) / CapacityStepBitLength - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			var fragment2 = (int)fragment;
			high = new(GetArrayLength(list.Count, fragment2));
			highLength = [];
			var index = 0;
			for (; index <= list.Count - fragment2; index += fragment2)
			{
				highLength.Add(fragment2);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = (TCertain)this;
				high[^1].AddRange(list.GetROLSlice(index, fragment2));
			}
			if (list.Count % fragment2 != 0)
			{
				highLength.Add(list.Count - index);
				high.Add(CapacityCreator(fragment));
				high[^1].parent = (TCertain)this;
				high[^1].AddRange(list.GetROLSlice(index));
			}
		}
		_capacity = Length;
	}

	private protected virtual void Copy(List<TCertain> source, BigSumList sourceLength, MpzT sourceIndex, List<TCertain> destination, BigSumList destinationLength, MpzT destinationIndex, MpzT length, MpzT fragment)
	{
		CheckParams(source, sourceIndex, destination, destinationIndex, length);
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (source == destination && sourceIndex == destinationIndex)
			return;
		var sourceIntIndex = sourceLength.IndexOfNotGreaterSum(sourceIndex, out var sourceBitsIndex);               // Целый индекс в исходном массиве.
		var destinationIntIndex = destinationLength.IndexOfNotGreaterSum(destinationIndex, out var destinationBitsIndex);     // Целый индекс в целевом массиве.
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		var sourceEndIntIndex = sourceLength.IndexOfNotGreaterSum(sourceEndIndex, out var sourceEndBitsIndex);  // Индекс инта последнего бита.
		while (sourceEndBitsIndex >= fragment)
		{
			sourceEndIntIndex++;
			sourceEndBitsIndex -= fragment;
		}
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		var destinationEndIntIndexA = destinationLength.IndexOfNotGreaterSum(destinationEndIndex, out var destinationEndBitsIndexA);  // Индекс инта последнего бита.
		var destinationEndIntIndexB = (int)destinationEndIndex.Divide(fragment, out var destinationEndBitsIndexB);
		var (destinationEndIntIndex, destinationEndBitsIndex) = destinationEndIntIndexA > destinationEndIntIndexB ? (destinationEndIntIndexA, destinationEndBitsIndexA) : (destinationEndIntIndexB, destinationEndBitsIndexB);
		while (destinationEndBitsIndex >= fragment)
		{
			destinationEndIntIndex++;
			destinationEndBitsIndex -= fragment;
		}
		if (destinationIntIndex != 0 && destinationBitsIndex == 0 && destinationLength[destinationIntIndex - 1] != fragment)
		{
			destinationIntIndex--;
			destinationBitsIndex = destinationLength[^1];
		}
		if (sourceEndIntIndex == sourceIntIndex)
		{
			int index = (int)destinationIndex.Divide(fragment, out var remainder), index2 = (int)(destinationIndex + length - 1).Divide(fragment);
			if (index == index2)
			{
				source[sourceIntIndex].CopyToInternal(sourceBitsIndex, destination[index], remainder, length);
				destinationLength.SetOrAdd(index, RedStarLinq.Max(destinationLength.Length > index ? destinationLength[index] : 0, remainder + length));
			}
			else if (sourceIndex >= destinationIndex)
			{
				var previousPart = RedStarLinq.Max(destinationLength.Length > index ? destinationLength[index] : 0, fragment) - remainder;
				source[sourceIntIndex].CopyToInternal(sourceBitsIndex, destination[index], remainder, previousPart);
				destinationLength.SetOrAdd(index, RedStarLinq.Max(destinationLength.Length > index ? destinationLength[index] : 0, remainder + previousPart));
				for (var i = index + 1; i < index2; i++)
				{
					source[sourceIntIndex].CopyToInternal(sourceBitsIndex + previousPart, destination[i], 0, destinationLength.Length > i ? destinationLength[i] : 0);
					destinationLength.SetOrAdd(i, RedStarLinq.Max(destinationLength.Length > i ? destinationLength[i] : 0, fragment));
					previousPart += RedStarLinq.Max(destinationLength.Length > i ? destinationLength[i] : 0, fragment);
				}
				source[sourceIntIndex].CopyToInternal(sourceBitsIndex + previousPart, destination[index2], 0, length - previousPart);
				destinationLength.SetOrAdd(index2, RedStarLinq.Max(destinationLength.Length > index2 ? destinationLength[index2] : 0, length - previousPart));
			}
			else
			{
				for (var i = destinationLength.Length; i < index2 + 1; i++)
					destinationLength.SetOrAdd(i, 1);
				var previousPart = RedStarLinq.Max(destinationLength.Length > index ? destinationLength[index] : 0, fragment) - remainder + (index2 - index - 1) * fragment;
				source[sourceIntIndex].CopyToInternal(sourceBitsIndex + previousPart, destination[index2], 0, length - previousPart);
				destinationLength.SetOrAdd(index2, RedStarLinq.Max(destinationLength.Length > index2 ? destinationLength[index2] : 0, length - previousPart));
				for (var i = index2 - 1; i > index; i--)
				{
					source[sourceIntIndex].CopyToInternal(sourceBitsIndex + previousPart, destination[i], 0, destinationLength.Length > i ? destinationLength[i] : 0);
					destinationLength.SetOrAdd(i, RedStarLinq.Max(destinationLength.Length > i ? destinationLength[i] : 0, fragment));
					previousPart -= RedStarLinq.Max(destinationLength.Length > i ? destinationLength[i] : 0, fragment);
				}
				source[sourceIntIndex].CopyToInternal(sourceBitsIndex, destination[index], remainder, previousPart);
				destinationLength.SetOrAdd(index, RedStarLinq.Max(destinationLength.Length > index ? destinationLength[index] : 0, remainder + previousPart));
			}
		}
		else if (destinationEndIntIndex == destinationIntIndex)
		{
			int index = sourceLength.IndexOfNotGreaterSum(sourceIndex, out var remainder), index2 = sourceLength.IndexOfNotGreaterSum(sourceIndex + length - 1);
			if (index == index2)
				source[index].CopyToInternal(remainder, destination[destinationIntIndex], destinationIndex, length);
			else if (sourceIndex >= destinationIndex)
			{
				var previousPart = (sourceLength.Length > index ? sourceLength[index] : 0) - remainder;
				source[index].CopyToInternal(remainder, destination[destinationIntIndex], destinationBitsIndex, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					source[i].CopyToInternal(0, destination[destinationIntIndex], destinationBitsIndex + previousPart, sourceLength.Length > i ? sourceLength[i] : 0);
					previousPart += sourceLength.Length > i ? sourceLength[i] : 0;
				}
				source[index2].CopyToInternal(0, destination[destinationIntIndex], destinationBitsIndex + previousPart, length - previousPart);
				destinationLength.SetOrAdd(destinationIntIndex, RedStarLinq.Max(destinationLength.Length > destinationIntIndex ? destinationLength[destinationIntIndex] : 0, destinationBitsIndex + length));
			}
			else
			{
				var previousPart = (sourceLength.Length > index ? sourceLength[index] : 0) - remainder;
				for (var i = index + 1; i < index2; i++)
					previousPart += sourceLength.Length > i ? sourceLength[i] : 0;
				source[index2].CopyToInternal(0, destination[destinationIntIndex], destinationBitsIndex + previousPart, length - previousPart);
				destinationLength.SetOrAdd(destinationIntIndex, RedStarLinq.Max(destinationLength.Length > destinationIntIndex ? destinationLength[destinationIntIndex] : 0, destinationBitsIndex + length));
				for (var i = index2 - 1; i > index; i--)
				{
					source[i].CopyToInternal(0, destination[destinationIntIndex], destinationBitsIndex + previousPart, sourceLength.Length > i ? sourceLength[i] : 0);
					previousPart -= sourceLength.Length > i ? sourceLength[i] : 0;
				}
				source[index].CopyToInternal(remainder, destination[destinationIntIndex], destinationBitsIndex, previousPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
			goto l1;
		else
		{
			using var buff = source[sourceEndIntIndex].GetRangeInternal(0, sourceEndBitsIndex + 1, true);
			buff.Capacity = fragment << 1;
			for (var i = destinationLength.Length; i < destinationEndIntIndex + 1; i++)
				destinationLength.SetOrAdd(i, 1);
			for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; sourceCurrentIntIndex > sourceIntIndex - 1 || destinationCurrentIntIndex > destinationIntIndex;)
			{
				var currentLength = RedStarLinq.Max(destinationLength.Length > destinationCurrentIntIndex ? destinationLength[destinationCurrentIntIndex] : 0, fragment);
				if (buff.Length < currentLength)
				{
					var sourceElem = source[sourceCurrentIntIndex--].Copy();
					buff.Length += sourceElem.Length;
					Debug.Assert(buff.high != null && buff.highLength != null);
					buff.highLength.Insert(0, sourceElem.Length);
					buff.high.Insert(0, sourceElem);
					buff.high[0].parent = buff;
				}
				if (buff.Length >= currentLength && destinationCurrentIntIndex > destinationIntIndex)
				{
					var currentLength2 = destinationCurrentIntIndex == destinationEndIntIndex ? destinationEndBitsIndex + 1 : currentLength;
					destinationLength[destinationCurrentIntIndex] = RedStarLinq.Max(destinationLength[destinationCurrentIntIndex], currentLength2);
					buff.CopyToInternal(buff.Length - currentLength2, destination[destinationCurrentIntIndex--], 0, currentLength2);
					buff.Remove(buff.Length - currentLength2);
				}
			}
			destinationLength[destinationIntIndex] = RedStarLinq.Max(destinationLength[destinationIntIndex], fragment);
			buff.CopyToInternal(buff.Length - (destinationLength[destinationIntIndex] - destinationBitsIndex), destination[destinationIntIndex], destinationBitsIndex, destinationLength[destinationIntIndex] - destinationBitsIndex);
			buff.high = null;
		}
		return;
	l1:
		if (sourceBitsIndex >= destinationBitsIndex)
		{
			source[sourceIntIndex].CopyToInternal(sourceBitsIndex, destination[destinationIntIndex], destinationBitsIndex, fragment - sourceBitsIndex);
			for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; sourceCurrentIntIndex < sourceEndIntIndex + 1 || destinationCurrentIntIndex < destinationEndIntIndex;)
			{
				source[sourceCurrentIntIndex].CopyToInternal(0, destination[destinationCurrentIntIndex],
					fragment - (sourceBitsIndex - destinationBitsIndex), destinationCurrentIntIndex < destinationEndIntIndex
					? sourceBitsIndex - destinationBitsIndex : sourceEndBitsIndex + 1);
				destinationLength.SetOrAdd(destinationCurrentIntIndex++,
					RedStarLinq.Max(destinationLength.Length > destinationCurrentIntIndex
					? destinationLength[destinationCurrentIntIndex] : 0, fragment));
				if (destinationCurrentIntIndex < destinationEndIntIndex + (destinationEndBitsIndex > sourceEndBitsIndex ? 1 : 0))
					source[sourceCurrentIntIndex].CopyToInternal(sourceBitsIndex - destinationBitsIndex, destination[destinationCurrentIntIndex], 0, fragment - (sourceBitsIndex - destinationBitsIndex));
				sourceCurrentIntIndex++;
			}
			if (sourceBitsIndex >= destinationBitsIndex && sourceEndIntIndex + destinationIntIndex - sourceIntIndex == destinationEndIntIndex)
				source[sourceEndIntIndex].CopyToInternal(sourceBitsIndex - destinationBitsIndex, destination[destinationEndIntIndex], 0, destinationEndBitsIndex + 1);
		}
		else
		{
			source[sourceIntIndex].CopyToInternal(sourceBitsIndex, destination[destinationIntIndex], destinationBitsIndex, fragment - destinationBitsIndex);
			for (int sourceCurrentIntIndex = sourceIntIndex, destinationCurrentIntIndex = destinationIntIndex + 1; sourceCurrentIntIndex < sourceEndIntIndex || destinationCurrentIntIndex < destinationEndIntIndex + 1;)
			{
				source[sourceCurrentIntIndex].CopyToInternal(fragment - (destinationBitsIndex - sourceBitsIndex),
					destination[destinationCurrentIntIndex], 0, sourceCurrentIntIndex < sourceEndIntIndex
					? destinationBitsIndex - sourceBitsIndex : destinationEndBitsIndex + 1);
				sourceCurrentIntIndex++;
				if (sourceCurrentIntIndex < sourceEndIntIndex + 1)
					source[sourceCurrentIntIndex].CopyToInternal(0, destination[destinationCurrentIntIndex], destinationBitsIndex - sourceBitsIndex, destinationCurrentIntIndex < destinationEndIntIndex ? fragment - (destinationBitsIndex - sourceBitsIndex) : sourceEndBitsIndex + 1);
				destinationLength.SetOrAdd(destinationCurrentIntIndex++, RedStarLinq.Max(destinationLength.Length > destinationCurrentIntIndex ? destinationLength[destinationCurrentIntIndex] : 0, fragment));
			}
		}
		destinationLength.SetOrAdd(destinationEndIntIndex, RedStarLinq.Max(destinationLength.Length > destinationEndIntIndex ? destinationLength[destinationEndIntIndex] : 0, destinationEndBitsIndex + 1));
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
		if (length == 0) // Если длина копируеммой последовательность ноль, то ничего делать не надо.
			return;
		if (this == destination && sourceIndex == destinationIndex)
			return;
		if (low != null && destination.low != null)
		{
			destination.low.SetRangeAndSizeInternal((int)destinationIndex, (int)length, low.GetRange((int)sourceIndex, (int)length));
			destination.Length = RedStarLinq.Max(destination.Length, destinationIndex + length);
#if VERIFY
			Verify();
			destination.Verify();
#endif
			return;
		}
		if ((destination.low != null || fragment > destination.fragment) && high != null && highLength != null)
		{
			int quotient = highLength.IndexOfNotGreaterSum(sourceIndex, out var remainder), quotient2 = highLength.IndexOfNotGreaterSum(sourceIndex + length - 1, out _);
			int index = isReversed ? highLength.Length - 1 - quotient : quotient, index2 = isReversed ? highLength.Length - 1 - quotient2 : quotient2;
			if (index == index2)
				high[index].CopyToInternal(remainder, destination, destinationIndex, length);
			else
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
#if VERIFY
			Verify();
			destination.Verify();
#endif
			return;
		}
		if ((low != null || destination.fragment > fragment) && destination.high != null && destination.highLength != null)
		{
			int index = destination.highLength.IndexOfNotGreaterSum(destinationIndex, out var remainder), index2A = destination.highLength.IndexOfNotGreaterSum(destinationIndex + length - 1, out var remainder2A);
			var index2B = (int)(destinationIndex + length - 1).Divide(destination.fragment, out var remainder2B);
			var (index2, remainder2) = index2A > index2B ? (index2A, remainder2A) : (index2B, remainder2B);
			if (index != 0 && index == destination.highLength.Length && remainder == 0 && destination.highLength[^1] != destination.fragment)
			{
				index--;
				remainder = destination.highLength[^1];
			}
			if (index2 != 0 && index2 == destination.highLength.Length && remainder2 == 0 && index2A > index2B && destination.highLength[^1] != destination.fragment)
				index2--;
			if (index == index2)
			{
				CopyToInternal(sourceIndex, destination.high[index], remainder, length);
				destination.highLength.SetOrAdd(index, RedStarLinq.Max(destination.highLength.Length > index ? destination.highLength[index] : 0, remainder + length));
			}
			else
			{
				var previousPart = RedStarLinq.Min(RedStarLinq.Max(destination.highLength.Length > index ? destination.highLength[index] : 0, destination.fragment) - remainder, length);
				CopyToInternal(sourceIndex, destination.high[index], remainder, previousPart);
				destination.highLength.SetOrAdd(index, RedStarLinq.Max(destination.highLength.Length > index ? destination.highLength[index] : 0, destination.fragment));
				for (var i = index + 1; i < index2; i++)
				{
					CopyToInternal(sourceIndex + previousPart, destination.high[i], 0, destination.highLength.Length > i ? destination.highLength[i] : 0);
					destination.highLength.SetOrAdd(i, RedStarLinq.Max(destination.highLength.Length > i ? destination.highLength[i] : 0, destination.fragment));
					previousPart += destination.highLength.Length > i ? destination.highLength[i] : 0;
				}
				CopyToInternal(sourceIndex + previousPart, destination.high[index2], 0, length - previousPart);
				if (length != previousPart)
					destination.highLength.SetOrAdd(index2, RedStarLinq.Max(destination.highLength.Length > index2 ? destination.highLength[index2] : 0, length - previousPart));
			}
#if VERIFY
			Verify();
			destination.Verify();
#endif
			return;
		}
		if (!(high != null && highLength != null && destination.high != null && destination.highLength != null && fragment == destination.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Copy(high, highLength, sourceIndex, destination.high, destination.highLength, destinationIndex, length, fragment);
#if VERIFY
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
			return low.GetInternal((int)(isReversed ? Length - 1 - index : index));
		else if (high != null && highLength != null)
		{
			var quotient = highLength.IndexOfNotGreaterSum(index, out var remainder);
			return high[isReversed ? high.Length - 1 - quotient : quotient].GetInternal(remainder, invoke);
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void RemoveFromEnd(MpzT index)
	{
		if (low != null)
			low.Remove((int)index);
		else if (high != null && highLength != null)
		{
			var quotient = (int)index.Divide(fragment, out var remainder);
			for (var i = high.Length - 1; i > quotient; i--)
				high[i].Clear();
			high[quotient].Remove(remainder);
			if (remainder == 0)
				highLength.Remove(quotient);
			else
			{
				highLength.Remove(quotient + 1);
				highLength[quotient] = remainder;
			}
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		if (low != null)
		{
			Length = index;
			Debug.Assert(Length == low.Length);
		}
		else if (high != null && highLength != null)
		{
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
		}
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
#if VERIFY
		Verify();
#endif
	}

	private protected override void SetInternal(MpzT index, T value)
	{
		if (low != null)
			low.SetInternal((int)(isReversed ? Length - 1 - index : index), value);
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
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
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
			Debug.Assert(Length == highLength.Sum() && Length == high.Sum(x => x.Length));
			Debug.Assert(high.All(x => x.parent == this));
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
			Debug.Assert(Length == high.Sum(x => x.Length));
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
#endif
}
