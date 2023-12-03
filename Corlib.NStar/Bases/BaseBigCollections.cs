
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseBigList<T, TCertain, TLow> : IBigList<T> where TCertain : BaseBigList<T, TCertain, TLow>, new() where TLow : G.IList<T>, new()
{
	public virtual T this[MpzT index]
	{
		get
		{
			if (index >= Size)
				throw new IndexOutOfRangeException();
			return GetInternal(index);
		}
		set
		{
			if (index >= Size)
				throw new IndexOutOfRangeException();
			SetInternal(index, value);
		}
	}

	public abstract MpzT Capacity { get; set; }

	public virtual MpzT Length => Size;

	private protected abstract Func<MpzT, TCertain> CapacityCreator { get; }

	private protected abstract Func<int, TLow> CapacityLowCreator { get; }

	private protected abstract Func<IEnumerable<T>, TCertain> CollectionCreator { get; }

	private protected abstract Func<IEnumerable<T>, TLow> CollectionLowCreator { get; }

	private protected virtual int DefaultCapacity => 32;

	bool IBigCollection<T>.IsReadOnly => false;

	private protected virtual MpzT Size { get; set; } = 0;

	public delegate void ListChangedHandler(TCertain newList);

	public event ListChangedHandler? ListChanged;

	public abstract TCertain Add(T item);

	void IBigCollection<T>.Add(T item) => Add(item);

	public virtual TCertain AddRange(IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		var length = bigList.Length;
		if (length == 0)
			return this as TCertain ?? throw new InvalidOperationException();
		return SetRangeAndSizeInternal(Size, Size + length, bigList);
	}

	private protected void Changed() => ListChanged?.Invoke(this as TCertain ?? throw new InvalidOperationException());

	public virtual void Clear()
	{
		if (Size > 0)
		{
			ClearInternal();
			Size = 0;
		}
	}

	public virtual void Clear(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
		ClearInternal(index, length);
	}

	private protected abstract void ClearInternal();

	private protected abstract void ClearInternal(MpzT index, MpzT length);

	public virtual bool Contains(T item) => Contains(item, 0, Size);

	public virtual bool Contains(T item, MpzT index) => Contains(item, index, Size - index);

	public virtual bool Contains(T item, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Size);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
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

	public virtual bool Contains(IEnumerable<T> collection) => Contains(collection, 0, Size);

	public virtual bool Contains(IEnumerable<T> collection, MpzT index) => Contains(collection, index, Size - index);

	public virtual bool Contains(IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
		ArgumentNullException.ThrowIfNull(collection);
		return ContainsInternal(collection, index, length);
	}

	public virtual bool Contains(TCertain list) => Contains((IEnumerable<T>)list, 0, Size);

	public virtual bool Contains(TCertain list, MpzT index) => Contains((IEnumerable<T>)list, index, Size - index);

	public virtual bool Contains(TCertain list, MpzT index, MpzT length) => Contains((IEnumerable<T>)list, index, length);

	public virtual bool ContainsAny(IEnumerable<T> collection) => ContainsAny(collection, 0, Size);

	public virtual bool ContainsAny(IEnumerable<T> collection, MpzT index) => ContainsAny(collection, index, Size - index);

	public virtual bool ContainsAny(IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
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

	public virtual bool ContainsAny(TCertain list) => ContainsAny((IEnumerable<T>)list, 0, Size);

	public virtual bool ContainsAny(TCertain list, MpzT index) => ContainsAny((IEnumerable<T>)list, index, Size - index);

	public virtual bool ContainsAny(TCertain list, MpzT index, MpzT length) => ContainsAny((IEnumerable<T>)list, index, length);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection) => ContainsAnyExcluding(collection, 0, Size);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, MpzT index) => ContainsAnyExcluding(collection, index, Size - index);

	public virtual bool ContainsAnyExcluding(IEnumerable<T> collection, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
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

	public virtual bool ContainsAnyExcluding(TCertain list) => ContainsAnyExcluding((IEnumerable<T>)list, 0, Size);

	public virtual bool ContainsAnyExcluding(TCertain list, MpzT index) => ContainsAnyExcluding((IEnumerable<T>)list, index, Size - index);

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

	private protected abstract void Copy(TCertain sourceBits, MpzT sourceIndex, TCertain destinationBits, MpzT destinationIndex, MpzT length);

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		if (array != null && array.Rank != 1)
			throw new ArgumentException(null);
		ArgumentNullException.ThrowIfNull(array);
		try
		{
			CopyToInternal(array, arrayIndex);
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException(null);
		}
	}

	public virtual void CopyTo(IBigList<T> list) => CopyTo(list, 0);

	public virtual void CopyTo(IBigList<T> list, MpzT listIndex) => CopyTo(0, list, listIndex, Length);

	public virtual void CopyTo(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
		CopyToInternal(index, list, listIndex, length);
	}

	public virtual void CopyTo(MpzT index, T[] array, int arrayIndex, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
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
			throw new ArgumentException(null, nameof(array));
		CopyToInternal(0, array2, arrayIndex, (int)Length);
	}

	private protected abstract void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length);

	private protected abstract void CopyToInternal(MpzT index, IBigList<T> list, MpzT listIndex, MpzT length);

	private protected virtual void EnsureCapacity(MpzT min)
	{
		if (Capacity < min)
		{
			var newCapacity = Size == 0 ? DefaultCapacity : Size * 2;
			if (newCapacity < min) newCapacity = min;
			Capacity = newCapacity;
		}
	}

	public virtual Enumerator GetEnumerator() => new(this);

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private protected abstract T GetInternal(MpzT index, bool invoke = true);

	public virtual TCertain GetRange(MpzT index, bool alwaysCopy = false) => GetRange(index, Size - index, alwaysCopy);

	public virtual TCertain GetRange(MpzT index, MpzT length, bool alwaysCopy = false)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
		if (length == 0)
			return CapacityCreator(0);
		else if (!alwaysCopy && index == 0 && length == Size && this is TCertain thisList)
			return thisList;
		var list = CapacityCreator(length);
		Copy(this as TCertain ?? throw new InvalidOperationException(), index, list, 0, length);
		list.Size = length;
		return list;
	}

	public virtual MpzT IndexOf(T item) => IndexOf(item, 0, Size);

	public virtual MpzT IndexOf(T item, MpzT index) => IndexOf(item, index, Size - index);

	public virtual MpzT IndexOf(T item, MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Size);
		if (length < 0 || index > Size - length)
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

	public virtual TCertain Remove(MpzT index) => Remove(index, Size - index);

	public virtual TCertain Remove(MpzT index, MpzT length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > Size)
			throw new ArgumentException(null);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length > 0)
		{
			Size -= length;
			if (index < Size)
				Copy(this2, index + length, this2, index, Size - index);
			ClearInternal(Size, length);
		}
		return this2;
	}

	public virtual TCertain RemoveAt(MpzT index)
	{
		if ((uint)index >= (uint)Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		Size--;
		if (index < Size)
			Copy(this2, index + 1, this2, index, Size - index);
		SetInternal(Size, default!);
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
		return false;
	}

	public virtual TCertain Replace(IEnumerable<T> collection)
	{
		if (collection is IBigCollection<T> col)
		{
			col.CopyTo(this, 0);
			Size = col.Length;
		}
		else
		{
			MpzT i = 0;
			foreach (var item in collection)
			{
				SetInternal(i, item);
				i++;
			}
			Size = i;
		}
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected abstract void SetInternal(MpzT index, T value);

	public virtual TCertain SetRange(MpzT index, IEnumerable<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (index < 0 || index > Size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (collection is not TCertain bigList)
			bigList = CollectionCreator(collection);
		if (index + bigList.Length > Size)
			throw new ArgumentException(null);
		return SetRangeInternal(index, bigList);
	}

	internal virtual TCertain SetRangeAndSizeInternal(MpzT index, MpzT length, TCertain list)
	{
		SetRangeInternal(index, list);
		Size = RedStarLinq.Max(Size, length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	internal virtual TCertain SetRangeAndSizeInternal(MpzT index, TCertain list)
	{
		SetRangeInternal(index, list);
		Size = RedStarLinq.Max(Size, index + list.Length);
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected virtual TCertain SetRangeInternal(MpzT index, TCertain bigList)
	{
		var length = bigList.Length;
		if (length == 0)
			return this as TCertain ?? throw new InvalidOperationException();
		EnsureCapacity(index + length);
		var this2 = this as TCertain ?? throw new InvalidOperationException();
		if (length > 0)
			Copy(bigList, 0, this2, index, length);
		return this2;
	}

	public virtual T[] ToArray()
	{
		if (Length > int.MaxValue)
			throw new InvalidOperationException("Слишком большой список для преобразования в массив!");
		var length = (int)Length;
		var array = new T[length];
		CopyToInternal(0, array, 0, length);
		return array;
	}

	public virtual TCertain TrimExcess()
	{
		var threshold = (int)(Capacity * 0.9);
		if (Size < threshold)
			Capacity = Size;
		return this as TCertain ?? throw new InvalidOperationException();
	}

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
			if (index >= list.Size)
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
			index = list.Size + 1;
			current = default!;
			return false;
		}

		public readonly T Current => current;

		readonly object IEnumerator.Current
		{
			get
			{
				if (index == 0 || index == list.Size + 1)
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
	private protected BigSumList? highCapacity;
	private protected MpzT _capacity = 0;
	private protected MpzT fragment = 1;
	private protected bool isHigh;

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
		highCapacity = null;
		fragment = 1;
		isHigh = false;
		Size = 0;
		_capacity = 0;
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
		if (capacity <= CapacityFirstStep)
		{
			low = CapacityLowCreator((int)capacity);
			high = null;
			highCapacity = null;
			fragment = 1;
			isHigh = false;
		}
		else
		{
			low = null;
			fragment = (MpzT)1 << (GetArrayLength((capacity - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
			while (ProperFragment * (CapacityStep * 3 >> 2) < capacity)
				fragment <<= CapacityStepBitLength;
			var quotient = capacity.Divide(ProperFragment, out var remainder);
			var highCount = (int)GetArrayLength(capacity, ProperFragment);
			high = new(highCount);
			highCapacity = [];
			for (MpzT i = 0; i < quotient; i++)
			{
				high.Add(CapacityCreator(ProperFragment));
				highCapacity.Add(ProperFragment);
			}
			if (remainder != 0)
			{
				high.Add(CapacityCreator(remainder));
				highCapacity.Add(remainder);
			}
			isHigh = true;
		}
		Size = 0;
		_capacity = capacity;
	}

	public BigList(IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(collection == null ? throw new ArgumentNullException(nameof(collection)) : List<T>.TryGetLengthEasilyEnumerable(collection, out var length) ? length : 0, capacityFirstStepBitLength, capacityStepBitLength)
	{
		using var en = collection.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	public BigList(MpzT capacity, IEnumerable<T> collection, int capacityStepBitLength = -1, int capacityFirstStepBitLength = -1) : this(capacity, capacityFirstStepBitLength, capacityStepBitLength)
	{
		using var en = collection.GetEnumerator();
		while (en.MoveNext())
			Add(en.Current);
	}

	public override MpzT Capacity
	{
		get => _capacity;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, Size);
			if (value == _capacity)
				return;
			if (value <= 0)
			{
				low = new();
				high = null;
				highCapacity = null;
				isHigh = false;
			}
			else if (value <= CapacityFirstStep)
			{
				//try
				//{
				//	throw new ExperimentalException();
				//}
				//catch
				//{
				//}
				low = GetFirstLists();
				var value2 = (int)value;
				low.Capacity = value2;
				high = null;
				highCapacity = null;
				isHigh = false;
			}
			else if (!isHigh && low != null)
			{
				fragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				while (ProperFragment * (CapacityStep * 3 >> 2) < value)
					fragment <<= CapacityStepBitLength;
				var highCount = (int)GetArrayLength(value, ProperFragment);
				high = new(highCount);
				highCapacity = [];
				for (MpzT i = 0; i < value / ProperFragment; i++)
				{
					high.Add(CapacityCreator(ProperFragment));
					highCapacity.Add(ProperFragment);
				}
				if (value % ProperFragment != 0)
				{
					high.Add(CapacityCreator(value % ProperFragment));
					highCapacity.Add(value % ProperFragment);
				}
				high[0].AddRange(low);
				low = null;
				isHigh = true;
			}
			else if (high != null && highCapacity != null)
			{
				var oldFragment = fragment;
				fragment = (MpzT)1 << (GetArrayLength((value - 1).BitLength - CapacityFirstStepBitLength, CapacityStepBitLength) - 1) * CapacityStepBitLength + CapacityFirstStepBitLength;
				while (ProperFragment * (CapacityStep * 3 >> 2) < value)
					fragment <<= CapacityStepBitLength;
				if (fragment > oldFragment)
					goto l0;
				if (fragment < oldFragment)
					goto l1;
				high.Capacity = (int)GetArrayLength(value, ProperFragment);
				high[^1].Capacity = (high.Length < high.Capacity || value % ProperFragment == 0) ? ProperFragment : value % ProperFragment;
				for (var i = high.Length; i < high.Capacity - 1; i++)
				{
					high.Add(CapacityCreator(ProperFragment));
					highCapacity.Add(ProperFragment);
				}
				if (high.Length < high.Capacity)
				{
					var remainder = value % ProperFragment == 0 ? ProperFragment : value % ProperFragment;
					high.Add(CapacityCreator(remainder));
					highCapacity.Add(remainder);
				}
				return;
			l0:
				do
				{
					oldFragment <<= CapacityStepBitLength;
					var highCount = (int)RedStarLinq.Min(GetArrayLength(value, oldFragment), CapacityStep);
					high = new(highCount) { this as TCertain ?? throw new InvalidOperationException() };
					new Chain(1, high.Capacity - 2).ForEach(_ => high.Add(CapacityCreator(oldFragment)));
					highCapacity = new(RedStarLinq.FillArray(high.Capacity - 1, _ => new MpzT(oldFragment)));
					var remainder = (oldFragment < fragment || value % oldFragment == 0) ? oldFragment : value % oldFragment;
					high.Add(CapacityCreator(remainder));
					highCapacity.Add(remainder);
				} while (oldFragment < fragment);
				return;
			l1:
				do
				{
					oldFragment >>= CapacityStepBitLength;
					high = high[0].high!;
				} while (oldFragment > fragment);
				high[0].Capacity = value;
			}
			_capacity = value;
		}
	}

	private protected virtual int CapacityFirstStepBitLength { get; init; } = 16;

	private protected virtual int CapacityFirstStep => 1 << CapacityFirstStepBitLength;

	private protected virtual int CapacityStepBitLength { get; init; } = 16;

	private protected virtual int CapacityStep => 1 << CapacityStepBitLength;

	private protected virtual MpzT ProperFragment
	{
		get
		{
			if (fragment == 1)
				return 1;
			var steps = ((fragment - 1).BitLength - CapacityFirstStepBitLength) / CapacityStepBitLength;
			return ((MpzT)1 << steps * (CapacityStepBitLength - 2) + (CapacityFirstStepBitLength - 2)) * MpzT.Power(3, steps + 1);
		}
	}

	public override TCertain Add(T item)
	{
		EnsureCapacity(Size + 1);
		if (!isHigh && low != null)
			low.Add(item);
		else if (high != null)
			high[(int)(Size / fragment)].Add(item);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Size++;
		return this as TCertain ?? throw new InvalidOperationException();
	}

	private protected override void ClearInternal()
	{
		if (!isHigh && low != null)
			low.Clear();
		else
		{
			high?.Clear();
			highCapacity?.Clear();
		}
	}

	private protected override void ClearInternal(MpzT index, MpzT length)
	{
		if (!isHigh && low != null)
			low.Clear((int)index, (int)length);
		else if (high != null && highCapacity != null)
		{
			var quotient = highCapacity.IndexOfNotGreaterSum(index, out var remainder);
			var quotient2 = highCapacity.IndexOfNotGreaterSum(index + length - 1);
			if (quotient == quotient2)
			{
				high[quotient].ClearInternal(remainder, length);
				return;
			}
			var previousPart = highCapacity[quotient] - remainder;
			high[quotient].ClearInternal(remainder, previousPart);
			for (var i = quotient + 1; i < quotient2; i++)
			{
				high[i].ClearInternal(0, highCapacity[i]);
				previousPart += highCapacity[i];
			}
			high[quotient2].ClearInternal(0, length - previousPart);
		}
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
			destinationBits.low.SetRangeAndSizeInternal((int)destinationIndex, (int)length, sourceBits.low.GetRange((int)sourceIndex, (int)length));
			destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
			return;
		}
		if (sourceBits.fragment > destinationBits.fragment && sourceBits.isHigh && sourceBits.high != null && sourceBits.highCapacity != null)
		{
			int index = sourceBits.highCapacity.IndexOfNotGreaterSum(sourceIndex, out var remainder), index2 = sourceBits.highCapacity.IndexOfNotGreaterSum(sourceIndex + length - 1);
			if (index == index2)
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, length);
			else
			{
				var previousPart = sourceBits.highCapacity[index] - remainder;
				Copy(sourceBits.high[index], remainder, destinationBits, destinationIndex, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits.high[i], 0, destinationBits, destinationIndex + previousPart, sourceBits.highCapacity[i]);
					previousPart += sourceBits.highCapacity[i];
				}
				Copy(sourceBits.high[index2], 0, destinationBits, destinationIndex + previousPart, length - previousPart);
			}
			destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
			return;
		}
		if (destinationBits.fragment > sourceBits.fragment && destinationBits.isHigh && destinationBits.high != null && destinationBits.highCapacity != null)
		{
			int index = destinationBits.highCapacity.IndexOfNotGreaterSum(destinationIndex, out var remainder), index2 = destinationBits.highCapacity.IndexOfNotGreaterSum(destinationIndex + length - 1);
			if (index == index2)
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, length);
			else
			{
				var previousPart = destinationBits.highCapacity[index] - remainder;
				Copy(sourceBits, sourceIndex, destinationBits.high[index], remainder, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits, sourceIndex + previousPart, destinationBits.high[i], 0, destinationBits.highCapacity[i]);
					previousPart += destinationBits.highCapacity[i];
				}
				Copy(sourceBits, sourceIndex + previousPart, destinationBits.high[index2], 0, length - previousPart);
			}
			destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
			return;
		}
		if (!(sourceBits.isHigh && sourceBits.high != null && sourceBits.highCapacity != null && destinationBits.isHigh && destinationBits.high != null && destinationBits.highCapacity != null && sourceBits.fragment == destinationBits.fragment))
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
		Copy(sourceBits.high, sourceBits.highCapacity, sourceIndex, destinationBits.high, destinationBits.highCapacity, destinationIndex, length, sourceBits.fragment);
		destinationBits.Size = RedStarLinq.Max(destinationBits.Size, sourceIndex + length);
	}

	private protected virtual void Copy(List<TCertain> sourceBits, BigSumList sourceCount, MpzT sourceIndex, List<TCertain> destinationBits, BigSumList destinationCount, MpzT destinationIndex, MpzT length, MpzT fragment)
	{
		var sourceIntIndex = sourceCount.IndexOfNotGreaterSum(sourceIndex, out var sourceBitsIndex);               // Целый индекс в исходном массиве.
		var destinationIntIndex = destinationCount.IndexOfNotGreaterSum(destinationIndex, out var destinationBitsIndex);     // Целый индекс в целевом массиве.
		var sourceEndIndex = sourceIndex + length - 1;        // Индекс последнего бита в исходном массиве.
		var sourceEndIntIndex = sourceCount.IndexOfNotGreaterSum(sourceEndIndex, out var sourceEndBitsIndex);  // Индекс инта последнего бита.
		var destinationEndIndex = destinationIndex + length - 1;        // Индекс последнего бита в целевом массиве.
		var destinationEndIntIndex = destinationCount.IndexOfNotGreaterSum(destinationEndIndex, out var destinationEndBitsIndex);  // Индекс инта последнего бита.
		if (sourceEndIntIndex == sourceIntIndex)
		{
			int index = destinationCount.IndexOfNotGreaterSum(destinationIndex, out var remainder), index2 = destinationCount.IndexOfNotGreaterSum(destinationIndex + length - 1);
			if (index == index2)
				Copy(sourceBits[sourceIntIndex], sourceIndex, destinationBits[index], remainder, length);
			else
			{
				var previousPart = destinationCount[index] - remainder;
				Copy(sourceBits[sourceIntIndex], sourceIndex, destinationBits[index], remainder, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits[sourceIntIndex], sourceIndex + previousPart, destinationBits[i], 0, destinationCount[i]);
					previousPart += destinationCount[i];
				}
				Copy(sourceBits[sourceIntIndex], sourceIndex + previousPart, destinationBits[index2], 0, length - previousPart);
			}
		}
		else if (destinationEndIntIndex == destinationIntIndex)
		{
			int index = sourceCount.IndexOfNotGreaterSum(sourceIndex, out var remainder), index2 = sourceCount.IndexOfNotGreaterSum(sourceIndex + length - 1);
			if (index == index2)
				Copy(sourceBits[index], remainder, destinationBits[destinationIntIndex], destinationIndex, length);
			else
			{
				var previousPart = sourceCount[index] - remainder;
				Copy(sourceBits[index], remainder, destinationBits[destinationIntIndex], destinationIndex, previousPart);
				for (var i = index + 1; i < index2; i++)
				{
					Copy(sourceBits[i], 0, destinationBits[destinationIntIndex], destinationIndex + previousPart, sourceCount[i]);
					previousPart += sourceCount[i];
				}
				Copy(sourceBits[index2], 0, destinationBits[destinationIntIndex], destinationIndex + previousPart, length - previousPart);
			}
		}
		else if (sourceIndex >= destinationIndex)
		{
			var buff = destinationBits[destinationIntIndex].GetRange(0, destinationBitsIndex);
			buff.AddRange(sourceBits[sourceIntIndex].GetRange(sourceBitsIndex));
			for (int sourceCurrentIntIndex = sourceIntIndex + 1, destinationCurrentIntIndex = destinationIntIndex; sourceCurrentIntIndex < sourceEndIntIndex + 1 || destinationCurrentIntIndex < destinationEndIntIndex;)
			{
				var currentLength = destinationCount[destinationCurrentIntIndex];
				if (buff.Length < currentLength)
				{
					var sourceElem = sourceBits[sourceCurrentIntIndex++];
					buff.AddRange(sourceCurrentIntIndex == sourceEndIntIndex ? sourceElem.GetRange(0, destinationEndBitsIndex + 1) : sourceElem);
				}
				if (buff.Length >= currentLength && destinationCurrentIntIndex < destinationEndIntIndex)
				{
					destinationBits[destinationCurrentIntIndex++] = buff.GetRange(0, currentLength, true);
					buff.Remove(0, currentLength);
				}
			}
			destinationBits[destinationEndIntIndex].SetRangeAndSizeInternal(0, buff.GetRange(0, destinationEndBitsIndex + 1));
		}
		else
		{
			var buff = sourceBits[sourceEndIntIndex].GetRange(0, sourceEndBitsIndex + 1);
			buff.AddRange(destinationBits[destinationEndIntIndex].GetRange(destinationEndBitsIndex + 1));
			buff.Capacity = fragment << 1;
			for (int sourceCurrentIntIndex = sourceEndIntIndex - 1, destinationCurrentIntIndex = destinationEndIntIndex; sourceCurrentIntIndex > sourceIntIndex - 1 || destinationCurrentIntIndex > destinationIntIndex;)
			{
				var currentLength = destinationCount[destinationCurrentIntIndex];
				if (buff.Length < currentLength)
				{
					var sourceElem = sourceBits[sourceCurrentIntIndex--];
					buff.high?.Insert(0, sourceElem);
				}
				if (buff.Length >= currentLength && destinationCurrentIntIndex > destinationIntIndex)
				{
					destinationBits[destinationCurrentIntIndex--] = buff.GetRange(buff.Size - fragment, true);
					buff.Remove(buff.Size - fragment);
				}
			}
			destinationBits[destinationIntIndex].SetRangeAndSizeInternal(destinationBitsIndex, buff.GetRange(buff.Size - destinationBitsIndex));
		}
	}

	private static void CheckParams(TCertain sourceBits, MpzT sourceIndex, TCertain destinationBits, MpzT destinationIndex, MpzT length)
	{
		if (sourceBits == null)
			throw new ArgumentNullException(nameof(sourceBits), "Исходный массив не может быть нулевым.");
		if (sourceBits.Capacity == 0)
			throw new ArgumentException("Исходный массив не может быть пустым.", nameof(sourceBits));
		if (destinationBits == null)
			throw new ArgumentNullException(nameof(destinationBits), "Целевой массив не может быть нулевым.");
		if (destinationBits.Capacity == 0)
			throw new ArgumentException("Целевой массив не может быть пустым.", nameof(destinationBits));
		if (sourceIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Индекс не может быть отрицательным.");
		if (destinationIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Индекс не может быть отрицательным.");
		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), "Длина не может быть отрицательной.");
		if (sourceIndex + length > sourceBits.Capacity)
			throw new ArgumentException("Копируемая последовательность выходит за размер исходного массива.");
		if (destinationIndex + length > destinationBits.Capacity)
			throw new ArgumentException("Копируемая последовательность не помещается в размер целевого массива.");
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

	private protected override void CopyToInternal(MpzT index, T[] array, int arrayIndex, int length)
	{
		for (var i = 0; i < length; i++)
			array[arrayIndex + i] = GetInternal(index + i);
	}

	private protected virtual TLow GetFirstLists()
	{
		if (!isHigh && low != null)
			return low;
		else if (high != null)
			return high[0].GetFirstLists();
		else
			return new();
	}

	private protected override T GetInternal(MpzT index, bool invoke = true)
	{
		if (!isHigh && low != null)
			return low.GetInternal((int)index);
		else if (high != null)
			return high.GetInternal((int)(index / fragment)).GetInternal(index % fragment);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}

	private protected override void SetInternal(MpzT index, T value)
	{
		if (!isHigh && low != null)
			low.SetInternal((int)index, value);
		else if (high != null)
			high.GetInternal((int)(index / fragment)).SetInternal(index % fragment, value);
		else
			throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.");
	}
}
