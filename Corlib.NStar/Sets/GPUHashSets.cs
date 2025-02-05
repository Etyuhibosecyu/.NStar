
namespace Corlib.NStar;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseGPUHashSet<T, TCertain> : BaseSet<T, TCertain> where T : unmanaged where TCertain : BaseGPUHashSet<T, TCertain>, new()
{
	private protected MemoryBuffer1D<Index1D, Stride1D.Dense> buckets = default!;
	private protected MemoryBuffer1D<int, Stride1D.Dense> hashCodes = default!;
	private protected MemoryBuffer1D<Index1D, Stride1D.Dense> nexts = default!;
	private protected MemoryBuffer1D<T, Stride1D.Dense> items = default!;
	
	private protected static Context context = Context.CreateDefault();
	private protected static Accelerator accel = context.GetPreferredDevice(false).CreateAccelerator(context);

	public override int Capacity
	{
		get => buckets.IntExtent;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, _size);
			Resize(value, false);
			Changed();
		}
	}
	public virtual IEqualityComparer<T> Comparer { get; private protected set; } = EqualityComparer<T>.Default;

	private protected override void ClearInternal()
	{
		if (_size > 0)
		{
			buckets.Clear(accel);
			hashCodes.Clear(accel);
			nexts.Clear(accel);
			items.Clear(accel);
			_size = 0;
		}
	}

	private protected override void ClearInternal(int index, int length)
	{
		for (var i = 0; i < length; i++)
			SetNull(index + i);
		Changed();
	}

	private protected override void CopyToInternal(int sourceIndex, TCertain destination, int destinationIndex, int length)
	{
		if (this != destination || sourceIndex >= destinationIndex)
			for (var i = 0; i < length; i++)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
		else
		{
			for (var i = length - destinationIndex + sourceIndex; i < length; i++)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
			for (var i = length - 1; i >= 0; i--)
				CopyOne(sourceIndex + i, destination, destinationIndex + i);
		}
	}

	private protected virtual void CopyOne(int sourceIndex, TCertain destination, int destinationIndex)
	{
		var hashCode = hashCodes.View[sourceIndex];
		if (hashCode < 0)
		{
			destination.SetInternal(destinationIndex, items.View[sourceIndex]);
			if (destination is ListHashSet<T> && destinationIndex == destination._size)
				destination._size++;
		}
		else if (destination.hashCodes.View[destinationIndex] < 0)
			destination.SetNull(destinationIndex);
		else if (destinationIndex == destination._size)
		{
			destination.SetInternal(destinationIndex, default!);
			destination.SetNull(destinationIndex);
		}
	}

	private protected virtual void CopyToCommon(int index, T[] array, int arrayIndex, int length)
	{
		var skipped = 0;
		for (var i = 0; i < index; i++)
			if (hashCodes.View[i] >= 0)
				skipped++;
		for (var i = 0; i < length; i++)
			if (hashCodes.View[i] < 0)
				array[arrayIndex++] = items.View[index + i + skipped];
			else
				length++;
	}

	public override void Dispose()
	{
		buckets.Dispose();
		hashCodes.Dispose();
		nexts.Dispose();
		items.Dispose();
		_size = 0;
		GC.SuppressFinalize(this);
	}

	private protected static unsafe bool Equals(T x, T y)
	{
		ulong* xptr = (ulong*)&x, yptr = (ulong*)&y;
		if (sizeof(T) >= 8)
		{
			var length = sizeof(T) / 8;
			for (var i = 0; i < length; i++)
				if (xptr[i] != yptr[i])
					return false;
			xptr += length;
			yptr += length;
		}
		return (sizeof(T) % 8) switch
		{
			0 => true,
			1 => *(byte*)xptr == *(byte*)yptr,
			2 => *(ushort*)xptr == *(ushort*)yptr,
			3 => *(ushort*)xptr == *(ushort*)yptr && *((byte*)xptr + 2) == *((byte*)yptr + 2),
			4 => *(uint*)xptr == *(uint*)yptr,
			5 => *(uint*)xptr == *(uint*)yptr && *((byte*)xptr + 4) == *((byte*)yptr + 4),
			6 => *(uint*)xptr == *(uint*)yptr && *((ushort*)xptr + 2) == *((ushort*)yptr + 2),
			7 => *(uint*)xptr == *(uint*)yptr && *((ushort*)xptr + 2) == *((ushort*)yptr + 2) && *((byte*)xptr + 6) == *((byte*)yptr + 6),
			_ => false,
		};
	}

	internal override T GetInternal(int index, bool invoke = true)
	{
		var item = items.View[index];
		if (invoke)
			Changed();
		return item;
	}

	private protected static int IndexOfInternal(ArrayView<Index1D> buckets, ArrayView<int> hashCodes, ArrayView<Index1D> nexts,
		ArrayView<T> items, T item, int index, int length) =>
		IndexOfInternal(buckets, hashCodes, nexts, items, item, index, length, item.GetHashCode() & 0x7FFFFFFF);

	private protected static int IndexOfInternal(ArrayView<Index1D> buckets, ArrayView<int> hashCodes, ArrayView<Index1D> nexts,
		ArrayView<T> items, T item, int index, int length, int hashCode)
	{
		if (buckets.IsValid)
		{
			uint collisionCount = 0;
			Debug.Assert(hashCode >= 0);
			for (var i = ~buckets[hashCode % buckets.IntExtent]; i >= 0; i = ~nexts[i])
			{
				if (~nexts[i] == i)
					return -2147483648;
				if (hashCodes[i] == ~hashCode && Equals(items[i], item) && i >= index && i < index + length)
					return i;
				collisionCount++;
				if (collisionCount > buckets.IntExtent)
					return -2147483648;
			}
		}
		return -1;
	}

	private protected override int IndexOfInternal(T item, int index, int length) => IndexOfInternal(item, index, length, Comparer.GetHashCode(item) & 0x7FFFFFFF);

	private protected virtual int IndexOfInternal(T item, int index, int length, int hashCode)
	{
		if (buckets != null)
		{
			uint collisionCount = 0;
			Debug.Assert(hashCode >= 0);
			for (var i = ~buckets.View[hashCode % buckets.IntExtent]; i >= 0; i = ~nexts.View[i])
			{
				if (~nexts.View[i] == i)
					throw new InvalidOperationException();
				if (hashCodes.View[i] == ~hashCode && Comparer.Equals(items.View[i], item) && i >= index && i < index + length)
					return i;
				collisionCount++;
				if (collisionCount > buckets.IntExtent)
					throw new InvalidOperationException();
			}
		}
		return -1;
	}

	private protected virtual void Initialize(int capacity, out MemoryBuffer1D<Index1D, Stride1D.Dense> buckets, out MemoryBuffer1D<int, Stride1D.Dense> hashCodes, out MemoryBuffer1D<Index1D, Stride1D.Dense> nexts, out MemoryBuffer1D<T, Stride1D.Dense> items)
	{
		var size = HashHelpers.GetPrime(capacity);
		buckets = accel.Allocate1D<Index1D>(size);
		hashCodes = accel.Allocate1D<int>(size);
		nexts = accel.Allocate1D<Index1D>(size);
		items = accel.Allocate1D<T>(size);
	}

	private protected static void Insert(ArrayView<Index1D> buckets, ArrayView<int> hashCodes,
		ArrayView<Index1D> nexts, ArrayView<T> items, VariableView<int> freeList, VariableView<int> freeCount,
		VariableView<int> length, T item, VariableView<int> index, int hashCode) =>
		GPUHashSet<T>.Insert(buckets, hashCodes, nexts, items, freeList, freeCount, length, item, index, hashCode);

	private protected abstract TCertain Insert(T item, out int index, int hashCode);

	private protected override TCertain InsertInternal(int index, IEnumerable<T> collection)
	{
		var this2 = (TCertain)this;
		var set = CollectionCreator(collection).ExceptWith(this);
		if (set is FastDelHashSet<T> fhs)
			fhs.FixUpFakeIndexes();
		else if (set is ParallelHashSet<T> phs)
			phs.FixUpFakeIndexes();
		var length = set.Length;
		if (length > 0)
		{
			if (this == collection)
				return this2;
			EnsureCapacity(_size + length);
			if (index < _size)
				CopyToInternal(index, this2, index + length, _size - index);
			set.CopyToInternal(0, this2, index, length);
		}
		return this2;
	}

	private protected virtual void RemoveAtCommon(int index, ref int tHashCode, ref T tItem)
	{
		uint collisionCount = 0;
		var bucket = ~tHashCode % buckets.IntExtent;
		var last = -1;
		for (var i = ~buckets.View[bucket]; i >= 0; last = i, i = ~nexts.View[i])
		{
			if (~nexts.View[i] == i || ~nexts.View[i] == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					buckets.View[bucket] = nexts.View[i];
				else
					nexts.View[last] = nexts.View[i];
				break;
			}
			collisionCount++;
			if (collisionCount > buckets.IntExtent)
				throw new InvalidOperationException();
		}
		tHashCode = 0;
		tItem = default!;
	}

	private protected virtual bool RemoveValueCommon(T item, int hashCode, RemoveValueAction action)
	{
		uint collisionCount = 0;
		var bucket = hashCode % buckets.IntExtent;
		var last = -1;
		for (var i = ~buckets.View[bucket]; i >= 0; last = i, i = ~nexts.View[i])
		{
			if (~nexts.View[i] == i || ~nexts.View[i] == last && last != -1)
				throw new InvalidOperationException();
			if (hashCodes.View[i] == ~hashCode && Comparer.Equals(items.View[i], item))
			{
				if (last < 0)
					buckets.View[bucket] = nexts.View[i];
				else
					nexts.View[last] = nexts.View[i];
				ref var tHashCode = ref hashCodes.View[i];
				ref var tNext = ref nexts.View[i];
				ref var tItem = ref items.View[i];
				tHashCode = 0;
				tItem = default!;
				action(ref tHashCode, ref tNext, ref tItem, i);
				return true;
			}
			collisionCount++;
			if (collisionCount > buckets.IntExtent)
				throw new InvalidOperationException();
		}
		return false;
	}

	private protected delegate void RemoveValueAction(ref int hashCode, ref Index1D next, ref T item, int i);

	private protected virtual void Resize() => Resize(HashHelpers.ExpandPrime(_size), false);

	private protected virtual void Resize(int newSize, bool forceNewHashCodes)
	{
		var newBuckets = accel.Allocate1D<Index1D>(newSize);
		var newHashCodes = accel.Allocate1D<int>(newSize);
		var newNexts = accel.Allocate1D<Index1D>(newSize);
		var newItems = accel.Allocate1D<T>(newSize);
		CopyGPUMemory(hashCodes, newHashCodes, Min(buckets.IntExtent, newSize), accel);
		CopyGPUMemory(nexts, newNexts, Min(buckets.IntExtent, newSize), accel);
		CopyGPUMemory(items, newItems, Min(buckets.IntExtent, newSize), accel);
		if (forceNewHashCodes)
			for (var i = 0; i < _size; i++)
			{
				ref var t = ref newHashCodes.View[i];
				if (t != 0)
					t = ~newItems.View[i].GetHashCode() & 0x7FFFFFFF;
			}
		for (var i = 0; i < _size; i++)
			if (newHashCodes.View[i] < 0)
			{
				var bucket = ~newHashCodes.View[i] % newSize;
				newNexts.View[i] = newBuckets.View[bucket];
				newBuckets.View[bucket] = ~i;
			}
		buckets.Dispose();
		hashCodes.Dispose();
		nexts.Dispose();
		items.Dispose();
		buckets = newBuckets;
		hashCodes = newHashCodes;
		nexts = newNexts;
		items = newItems;
	}

	private protected virtual void SetNull(int index)
	{
		if (this is not ListHashSet<T>)
		{
			RemoveAt(index);
			return;
		}
		ref var tHashCode = ref hashCodes.View[index];
		if (tHashCode >= 0)
			return;
		ref var tItem = ref items.View[index];
		RemoveAtCommon(index, ref tHashCode, ref tItem);
		nexts.View[index] = 0;
		Debug.Assert(hashCodes.View[index] >= 0);
	}

	public static bool TryAdd(ArrayView<Index1D> buckets, ArrayView<int> hashCodes, ArrayView<Index1D> nexts,
		ArrayView<T> items, VariableView<int> freeList, VariableView<int> freeCount, VariableView<int> length,
		T item, VariableView<int> index)
	{
		var hashCode = item.GetHashCode() & 0x7FFFFFFF;
		if (TryGetIndexOf(buckets, hashCodes, nexts, items, length.Value, item, index, hashCode))
			return false;
		Insert(buckets, hashCodes, nexts, items, freeList, freeCount, length, item, index, hashCode);
		return true;
	}

	public override bool TryAdd(T item, out int index)
	{
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		if (TryGetIndexOf(item, out index, hashCode))
			return false;
		Insert(item, out index, hashCode);
		return true;
	}

	public static bool TryGetIndexOf(ArrayView<Index1D> buckets, ArrayView<int> hashCodes, ArrayView<Index1D> nexts,
		ArrayView<T> items, int length, T item, VariableView<int> index) =>
		TryGetIndexOf(buckets, hashCodes, nexts, items, length, item, index, item.GetHashCode() & 0x7FFFFFFF);

	private protected static bool TryGetIndexOf(ArrayView<Index1D> buckets, ArrayView<int> hashCodes, ArrayView<Index1D> nexts,
		ArrayView<T> items, int length, T item, VariableView<int> index, int hashCode) =>
		(index.Value = IndexOfInternal(buckets, hashCodes, nexts, items, item, 0, length, hashCode)) >= 0;

	public override bool TryGetIndexOf(T item, out int index) => TryGetIndexOf(item, out index, Comparer.GetHashCode(item) & 0x7FFFFFFF);

	private protected virtual bool TryGetIndexOf(T item, out int index, int hashCode) => (index = IndexOfInternal(item, 0, _size, hashCode)) >= 0;
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class GPUHashSet<T> : BaseGPUHashSet<T, GPUHashSet<T>> where T : unmanaged
{
	private protected int freeCount;
	private protected int freeList;
	private protected readonly object lockObj = new();

	public GPUHashSet() : this(0, (IEqualityComparer<T>?)null) { }

	public GPUHashSet(int capacity) : this(capacity, (IEqualityComparer<T>?)null) { }

	public GPUHashSet(IEqualityComparer<T>? comparer) : this(0, comparer) { }

	public GPUHashSet(int capacity, IEqualityComparer<T>? comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		if (capacity > 0)
			Initialize(capacity, out buckets, out hashCodes, out nexts, out items);
		else
		{
			buckets = default!;
			hashCodes = default!;
			nexts = default!;
			items = default!;
		}
		Comparer = comparer ?? EqualityComparer<T>.Default;
	}

	public GPUHashSet(IEnumerable<T> collection) : this(collection, null) { }

	public GPUHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : this(collection is ISet<T> set ? set.Count
		: typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection is G.IList<T> list_ ? list_.Count
		: collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
		{
			using var memoryBuffer = accel.Allocate1D([freeList, freeCount, _size, -1]);
			accel.LoadAutoGroupedStreamKernel(static (Index1D i, ArrayView<T> view, ArrayView<Index1D> buckets,
				ArrayView<int> hashCodes, ArrayView<Index1D> nexts, ArrayView<T> items, VariableView<int> freeList,
				VariableView<int> freeCount, VariableView<int> length, VariableView<int> index) =>
				TryAdd(buckets, hashCodes, nexts, items, freeList, freeCount,
				length, view[i], index))(list.Count, accel.Allocate1D([.. list]).View,
				buckets.View, hashCodes.View, nexts.View, items.View, memoryBuffer.View.VariableView(0),
				memoryBuffer.View.VariableView(1), memoryBuffer.View.VariableView(2), memoryBuffer.View.VariableView(3));
			var @out = new int[4];
			memoryBuffer.CopyToCPU(@out);
			freeList = @out[0];
			freeCount = @out[1];
			_size = @out[2];
		}
		else
			foreach (var item in collection)
				TryAdd(item);
	}

	public GPUHashSet(int capacity, IEnumerable<T> collection) : this(capacity, collection, null) { }

	public GPUHashSet(int capacity, IEnumerable<T> collection, IEqualityComparer<T>? comparer)
		: this(collection is G.IList<T> list_ ? Max(capacity, list_.Count) : capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		if (collection is G.IList<T> list)
			accel.LoadAutoGroupedStreamKernel<Index1D, ArrayView<T>>((i, view) => TryAdd(view[i]))(list.Count,
				accel.Allocate1D([.. list]).View);
		else
			foreach (var item in collection)
				TryAdd(item);
	}

	public GPUHashSet(int capacity, params T[] array) : this(capacity, (IEnumerable<T>)array) { }

	public GPUHashSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (IEnumerable<T>)span.ToArray()) { }

	public GPUHashSet(params T[] array) : this((IEnumerable<T>)array) { }

	public GPUHashSet(ReadOnlySpan<T> span) : this((IEnumerable<T>)span.ToArray()) { }

	private protected override Func<int, GPUHashSet<T>> CapacityCreator => x => new(x);

	private protected override Func<IEnumerable<T>, GPUHashSet<T>> CollectionCreator => x => new(x);

	private protected override void ClearInternal(int index, int length)
	{
		Parallel.For(0, length, i => RemoveValue(GetInternal(index + i)));
		Changed();
	}

	public override int Length => _size - freeCount;

	public virtual int Size => _size;

	public override bool Contains(T item, int index, int length) => UnsafeContains(item, index, length) || Lock(lockObj, UnsafeContains, item, index, length);

	public override bool Contains(IEnumerable<T> collection, int index, int length) => Lock(lockObj, base.Contains, collection, index, length);

	private protected override void CopyToInternal(int sourceIndex, GPUHashSet<T> destination, int destinationIndex, int length) => Lock(lockObj, base.CopyToInternal, sourceIndex, destination, destinationIndex, length);

	private protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => CopyToCommon(index, array, arrayIndex, length);

	private protected override bool EqualsInternal(IEnumerable<T>? collection, int index, bool toEnd = false) => Lock(lockObj, base.EqualsInternal, collection, index, toEnd);

	public override GPUHashSet<T> ExceptWith(IEnumerable<T> other)
	{
		if (other is FastDelHashSet<T> fhs)
			Parallel.For(0, fhs.Size, i => _ = fhs.IsValidIndex(i) && RemoveValue(fhs.GetInternal(i)));
		else if (other is ParallelHashSet<T> phs)
			Parallel.For(0, phs.Size, i => _ = phs.IsValidIndex(i) && RemoveValue(phs.GetInternal(i)));
		else if (other is G.IList<T> list)
			Parallel.For(0, list.Count, i => RemoveValue(list[i]));
		else
			foreach (var item in other)
				RemoveValue(item);
		return this;
	}

	public override int IndexOf(IEnumerable<T> collection, int index, int length, out int collectionLength)
	{
		lock (lockObj)
			return base.IndexOf(collection, index, length, out collectionLength);
	}

	private protected override int IndexOfInternal(T item, int index, int length, int hashCode)
	{
		var foundIndex = UnsafeIndexOf(item, index, length);
		return foundIndex < 0 ? foundIndex : Lock(lockObj, UnsafeIndexOf, item, index, length, hashCode);
	}

	private protected override void Initialize(int capacity, out MemoryBuffer1D<Index1D, Stride1D.Dense> buckets,
		out MemoryBuffer1D<int, Stride1D.Dense> hashCodes, out MemoryBuffer1D<Index1D, Stride1D.Dense> nexts,
		out MemoryBuffer1D<T, Stride1D.Dense> items)
	{
		lock (lockObj)
		{
			if (this.buckets == null)
				base.Initialize(capacity, out buckets, out hashCodes, out nexts, out items);
			else
			{
				buckets = this.buckets;
				hashCodes = this.hashCodes;
				nexts = this.nexts;
				items = this.items;
			}
		}
	}

	public override GPUHashSet<T> Insert(int index, T item)
	{
		if (buckets == null)
			Initialize(0, out buckets, out hashCodes, out nexts, out items);
		if ((uint)index > (uint)_size)
			throw new ArgumentOutOfRangeException(nameof(index));
		if (Contains(item))
			return this;
		lock (lockObj)
		{
			base.Insert(index, item);
			Debug.Assert(Contains(item));
		}
		return this;
	}

	public override GPUHashSet<T> Insert(int index, IEnumerable<T> collection) => Lock(lockObj, base.Insert, index, collection);

	internal new static void Insert(ArrayView<Index1D> buckets, ArrayView<int> hashCodes,
		ArrayView<Index1D> nexts, ArrayView<T> items, VariableView<int> freeList, VariableView<int> freeCount,
		VariableView<int> length, T item, VariableView<int> index, int hashCode)
	{
		if (!buckets.IsValid)
		{
			index.Value = -2147483648;
			return;
		}
		uint collisionCount = 0;
		var targetBucket = hashCode % buckets.IntExtent;
		for (var i = ~buckets[targetBucket]; i >= 0; i = ~nexts[i])
		{
			if (~nexts[i] == i)
			{
				index.Value = -2147483648;
				return;
			}
			if (hashCodes[i] == ~hashCode && Equals(items[i], item))
			{
				index.Value = i;
				return;
			}
			collisionCount++;
			if (collisionCount > buckets.IntExtent)
			{
				index.Value = -2147483648;
				return;
			}
		}
		lock ("A")
			UnsafeInsert(buckets, hashCodes, nexts, items, freeList, freeCount, length, item, index, hashCode);
	}

	private protected override GPUHashSet<T> Insert(T item, out int index, int hashCode)
	{
		if (buckets == null)
			Initialize(0, out buckets, out hashCodes, out nexts, out items);
		if (buckets == null)
			throw new InvalidOperationException();
		uint collisionCount = 0;
		var targetBucket = hashCode % buckets.IntExtent;
		for (var i = ~buckets.View[targetBucket]; i >= 0; i = ~nexts.View[i])
		{
			if (~nexts.View[i] == i)
				throw new InvalidOperationException();
			if (hashCodes.View[i] == ~hashCode && Comparer.Equals(items.View[i], item))
			{
				index = i;
				return this;
			}
			collisionCount++;
			if (collisionCount > buckets.IntExtent)
				throw new InvalidOperationException();
		}
		lock (lockObj)
			return UnsafeInsert(item, out index, hashCode);
	}

	public override GPUHashSet<T> IntersectWith(IEnumerable<T> other)
	{
		if (other is ISet<T> set)
			return Do(set);
		return Do(new GPUHashSet<T>(other));
		GPUHashSet<T> Do(ISet<T> set)
		{
			Parallel.For(0, _size, i =>
			{
				var item = GetInternal(i);
				if (!set.Contains(item))
					RemoveValue(item);
			});
			return this;
		}
	}

	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		var result = true;
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, (i, pls) =>
			{
				if (!Contains(list[i]))
				{
					result = false;
					pls.Stop();
				}
			});
		else
			foreach (var item in other)
				if (!Contains(item))
					return false;
		return result;
	}

	public virtual bool IsValidIndex(int index) => hashCodes.View[index] < 0;

	public override bool Overlaps(IEnumerable<T> other)
	{
		var result = false;
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, (i, pls) =>
			{
				if (Contains(list[i]))
				{
					result = true;
					pls.Stop();
				}
			});
		else
			foreach (var item in other)
				if (Contains(item))
					return true;
		return result;
	}

	public override GPUHashSet<T> RemoveAt(int index)
	{
		if (hashCodes.View[index] >= 0)
			return this;
		lock (lockObj)
		{
			UnsafeRemoveAt(index);
			Debug.Assert(hashCodes.View[index] >= 0);
		}
		return this;
	}

	private protected override GPUHashSet<T> RemoveInternal(int index, int length)
	{
		Parallel.For(index, index + length, i => RemoveAt(i));
		return this;
	}

	public override bool RemoveValue(T item)
	{
		if (buckets == null)
			return false;
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var index = UnsafeIndexOf(item, 0, _size, hashCode);
		if (index < 0)
			return false;
		lock (lockObj)
		{
			RemoveValueCommon(item, hashCode, (ref int _, ref Index1D tNext, ref T _, int i) =>
			{
				tNext = freeList;
				freeList = ~i;
				freeCount++;
			});
			Debug.Assert(!Contains(item));
		}
		return true;
	}

	private protected override void Resize(int newSize, bool forceNewHashCodes) => Lock(lockObj, base.Resize, newSize, forceNewHashCodes);

	public override bool SetEquals(IEnumerable<T> other)
	{
		var result = true;
		if (other is G.IList<T> list)
		{
			if (Length != list.Count)
				return false;
			Parallel.For(0, list.Count, (i, pls) =>
			{
				if (!Contains(list[i]))
				{
					result = false;
					pls.Stop();
				}
			});
			return result;
		}
		else if (other.TryGetLengthEasily(out var length))
		{
			if (Length != length)
				return false;
			foreach (var item in other)
				if (!Contains(item))
					return false;
			return true;
		}
		else
		{
			GPUHashSet<T> set = new(other);
			if (Length != set.Length)
				return false;
			Parallel.For(0, set.Length, (i, pls) =>
			{
				if (!Contains(set.GetInternal(i)))
				{
					result = false;
					pls.Stop();
				}
			});
			return result;
		}
	}

	internal override void SetInternal(int index, T item)
	{
		if (hashCodes.View[index] < 0)
			RemoveAt(index);
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		var targetBucket = hashCode % buckets.IntExtent;
		uint collisionCount = 0;
		var last = -1;
		for (var i = ~freeList; i >= 0; last = i, i = ~nexts.View[i])
		{
			if (~nexts.View[i] == i || ~nexts.View[i] == last && last != -1)
				throw new InvalidOperationException();
			if (i == index)
			{
				if (last < 0)
					freeList = nexts.View[i];
				else
					nexts.View[last] = nexts.View[i];
				freeCount--;
				goto l1;
			}
			collisionCount++;
			if (collisionCount > buckets.IntExtent)
				throw new InvalidOperationException();
		}
		if (_size == buckets.IntExtent)
		{
			Resize();
			targetBucket = hashCode % buckets.IntExtent;
		}
		_size++;
	l1:
		hashCodes.View[index] = ~hashCode;
		nexts.View[index] = buckets.View[targetBucket];
		items.View[index] = item;
		buckets.View[targetBucket] = ~index;
		Changed();
		Debug.Assert(IsValidIndex(index) && items.View[index].Equals(item));
	}

	private protected override GPUHashSet<T> SymmetricExceptInternal(IEnumerable<T> other) => Lock(lockObj, base.SymmetricExceptInternal, other);

	public override bool TryAdd(T item, out int index)
	{
		var hashCode = Comparer.GetHashCode(item) & 0x7FFFFFFF;
		index = UnsafeIndexOf(item, 0, _size, hashCode);
		if (index >= 0)
			return false;
		lock (lockObj)
		{
			UnsafeTryAdd(item, out index, hashCode);
			Debug.Assert(Contains(item));
		}
		return true;
	}

	public override GPUHashSet<T> UnionWith(IEnumerable<T> other)
	{
		if (other is G.IList<T> list)
			Parallel.For(0, list.Count, i => TryAdd(list[i]));
		else
			foreach (var item in other)
				TryAdd(item);
		return this;
	}

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual bool UnsafeContains(T item) => UnsafeContains(item, 0, _size);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual bool UnsafeContains(T item, int index) => UnsafeContains(item, index, _size - index);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual bool UnsafeContains(T item, int index, int length) => UnsafeIndexOf(item, index, length) >= 0;

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual int UnsafeIndexOf(T item) => UnsafeIndexOf(item, 0, _size);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual int UnsafeIndexOf(T item, int index) => UnsafeIndexOf(item, index, _size - index);

	/// <summary>
	/// Внимание! Этот метод не является потокобезопасным! При чтении такими методами одновременно с записью
	/// вы можете получить некорректный результат чтения (только попробуйте "достучаться" до небезопасных
	/// методов записи!). Перед использованием рекомендуется убедиться, что нет потоков, пытающихся писать
	/// в хэш-множество.
	/// </summary>
	public virtual int UnsafeIndexOf(T item, int index, int length) => UnsafeIndexOf(item, index, length, Comparer.GetHashCode(item) & 0x7FFFFFFF);

	private protected virtual int UnsafeIndexOf(T item, int index, int length, int hashCode)
	{
		var buckets = this.buckets;
		if (buckets.IsValid)
		{
			uint collisionCount = 0;
			for (var i = ~buckets.View[hashCode % buckets.IntExtent]; i >= 0; i = ~nexts.View[i])
			{
				if (hashCodes.View[i] == ~hashCode && Comparer.Equals(items.View[i], item) && i >= index && i < index + length)
					return i;
				if (~nexts.View[i] == i)
					throw new InvalidOperationException();
				collisionCount++;
				if (collisionCount > buckets.IntExtent)
					throw new InvalidOperationException();
			}
		}
		return -1;
	}

	private protected static void UnsafeInsert(ArrayView<Index1D> buckets, ArrayView<int> hashCodes,
		ArrayView<Index1D> nexts, ArrayView<T> items, VariableView<int> freeList, VariableView<int> freeCount,
		VariableView<int> length, T item, VariableView<int> index, int hashCode)
	{
		if (!buckets.IsValid)
		{
			index.Value = -2147483648;
			return;
		}
		var targetBucket = hashCode % buckets.IntExtent;
		if (freeCount.Value > 0)
		{
			index.Value = ~freeList.Value;
			freeList.Value = nexts[index.Value];
			freeCount.Value--;
		}
		else
		{
			if (length.Value == buckets.IntExtent)
			{
				index.Value = -2147483648;
				return;
			}
			index.Value = length.Value;
			length.Value++;
		}
		hashCodes[index.Value] = ~hashCode;
		nexts[index.Value] = buckets[targetBucket];
		items[index.Value] = item;
		buckets[targetBucket] = ~index.Value;
	}

	private protected virtual GPUHashSet<T> UnsafeInsert(T item, out int index, int hashCode)
	{
		if (buckets == null)
			Initialize(0, out buckets, out hashCodes, out nexts, out items);
		if (buckets == null)
			throw new InvalidOperationException();
		var targetBucket = hashCode % buckets.IntExtent;
		if (freeCount > 0)
		{
			index = ~freeList;
			freeList = nexts.View[index];
			freeCount--;
		}
		else
		{
			if (_size == buckets.IntExtent)
			{
				UnsafeResize();
				targetBucket = hashCode % buckets.IntExtent;
			}
			index = _size;
			_size++;
		}
		hashCodes.View[index] = ~hashCode;
		nexts.View[index] = buckets.View[targetBucket];
		items.View[index] = item;
		buckets.View[targetBucket] = ~index;
		return this;
	}

	private protected virtual GPUHashSet<T> UnsafeRemoveAt(int index)
	{
		if (!(buckets.IsValid && hashCodes.IsValid && nexts.IsValid && items.IsValid))
			return this;
		if (hashCodes.View[index] >= 0)
			return this;
		RemoveAtCommon(index, ref hashCodes.View[index], ref items.View[index]);
		nexts.View[index] = freeList;
		freeList = ~index;
		freeCount++;
		return this;
	}

	private protected virtual void UnsafeResize() => base.Resize(HashHelpers.ExpandPrime(_size), false);

	private protected virtual bool UnsafeTryAdd(T item, out int index, int hashCode)
	{
		index = UnsafeIndexOf(item);
		if (index >= 0)
			return false;
		UnsafeInsert(item, out index, hashCode);
		return true;
	}
}
