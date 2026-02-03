global using NStar.Core;
global using System;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static NStar.Core.Extents;
global using static System.Math;

namespace NStar.SortedSets;

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class BaseSortedSet<T, TCertain> : BaseSet<T, TCertain> where TCertain : BaseSortedSet<T, TCertain>, new()
{
	public override T this[Index index, bool invoke = false]
	{
		get => base[index, invoke];
		set => throw new NotSupportedException("Это действие не поддерживается в этой коллекции."
			+ " Если оно нужно вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");
	}

	public abstract G.IComparer<T> Comparer { get; }

	public override TCertain Add(T item)
	{
		TryAdd(item);
		return (TCertain)this;
	}

	protected override int IndexOfInternal(T item, int index, int length)
	{
		if (item is null)
			throw new ArgumentNullException(nameof(item));
		var ret = Search(item);
		return ret >= index && ret < index + length ? ret : -1;
	}

	public virtual int IndexOfNotLess(T item)
	{
		if (item is null)
			throw new ArgumentNullException(nameof(item));
		var ret = Search(item);
		return ret >= 0 ? ret : ~ret;
	}

	protected override void InsertInternal(int index, G.IEnumerable<T> collection) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public override TCertain Insert(int index, T item) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	protected abstract void InsertInternal(int index, T item);

	protected override void ReplaceRangeInternal(int index, int length, G.IEnumerable<T> collection) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public abstract int Search(T item);

	protected override void SetRangeInternal(int index, int length, TCertain list) =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public override TCertain Shuffle() =>
		throw new NotSupportedException("Этот метод не поддерживается в этой коллекции."
			+ " Если он нужен вам, используйте один из видов списков или хэш-множеств, а не отсортированных множеств.");

	public override bool TryAdd(T item, out int index)
	{
		if (item is null)
			throw new ArgumentNullException(nameof(item));
		index = Search(item);
		if (index >= 0)
			return false;
		InsertInternal(index = ~index, item);
		Changed();
		return true;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public abstract class SortedSet<T, TCertain> : BaseSortedSet<T, TCertain> where TCertain : SortedSet<T, TCertain>, new()
{
	private protected readonly List<T> items;

	public SortedSet() : this(G.Comparer<T>.Default) { }

	public SortedSet(int capacity) : this(capacity, G.Comparer<T>.Default) { }

	public SortedSet(G.IComparer<T>? comparer)
	{
		items = [];
		items.ListChanged += list =>
		{
			if (_size != list.Length)
			{
				_size = list.Length;
				Changed();
			}
		};
		Comparer = comparer ?? G.Comparer<T>.Default;
	}

	public SortedSet(Func<T, T, int> compareFunction) : this(new Comparer<T>(compareFunction)) { }

	public SortedSet(int capacity, G.IComparer<T>? comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity);
		items = new(capacity);
		items.ListChanged += list =>
		{
			if (_size != list.Length)
			{
				_size = list.Length;
				Changed();
			}
		};
		Comparer = comparer ?? G.Comparer<T>.Default;
	}

	public SortedSet(int capacity, Func<T, T, int> compareFunction) : this(capacity, new Comparer<T>(compareFunction)) { }

	public SortedSet(G.IEnumerable<T> collection) : this(collection, null) { }

	public SortedSet(G.IEnumerable<T> collection, G.IComparer<T>? comparer) : this(collection is G.ISet<T> set ? set.Count : typeof(T).Equals(typeof(byte)) ? ValuesInByte : collection.TryGetLengthEasily(out var length) ? (int)(Sqrt(length) * 10) : 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		items.AddRange(new ListHashSet<T>(collection, new EComparer<T>((x, y) => Comparer.Compare(x, y) == 0))).Sort(Comparer);
	}

	public SortedSet(int capacity, G.IEnumerable<T> collection) : this(capacity, collection, null) { }

	public SortedSet(int capacity, G.IEnumerable<T> collection, G.IComparer<T>? comparer) : this(capacity, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection);
		items.AddRange(new ListHashSet<T>(collection, new EComparer<T>((x, y) => Comparer.Compare(x, y) == 0))).Sort(Comparer);
	}

	public SortedSet(params T[] array) : this((G.IEnumerable<T>)array) { }

	public SortedSet(int capacity, params T[] array) : this(capacity, (G.IEnumerable<T>)array) { }

	public SortedSet(ReadOnlySpan<T> span) : this((G.IEnumerable<T>)span.ToArray()) { }

	public SortedSet(int capacity, ReadOnlySpan<T> span) : this(capacity, (G.IEnumerable<T>)span.ToArray()) { }

	public override int Capacity { get => items.Capacity; set => items.Capacity = value; }

	public override G.IComparer<T> Comparer { get; }

	public override int Length => items.Length;

	protected override void Changed()
	{
		base.Changed();
		if (_size != items.Length)
			items.Resize(_size);
	}

	protected override void ClearInternal(int index, int length) => items.Clear(index, length);

	protected override void CopyToInternal(Array array, int arrayIndex) => items.CopyTo(array, arrayIndex);

	protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length) => items.CopyTo(index, array, arrayIndex, length);

	public override void Dispose()
	{
		items.Dispose();
		Changed();
		GC.SuppressFinalize(this);
	}

	protected override T GetInternal(int index) =>
		(T?)items.GetType().GetMethod("GetInternal", System.Reflection.BindingFlags.Instance
		| System.Reflection.BindingFlags.NonPublic)?.Invoke(items, [index])
		?? throw new InvalidOperationException("Произошла внутренняя программная или аппаратная ошибка." +
		" Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");

	protected override void InsertInternal(int index, T item) => items.Insert(index, item);

	public override int Search(T item) => items.BinarySearch(item, Comparer);

	protected override void SetInternal(int index, T value)
	{
		items.GetType().GetMethod("SetInternal", System.Reflection.BindingFlags.Instance
			| System.Reflection.BindingFlags.NonPublic)?.Invoke(items, [index, value]);
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class SortedSet<T> : SortedSet<T, SortedSet<T>>
{
	public SortedSet() { }

	public SortedSet(int capacity) : base(capacity) { }

	public SortedSet(G.IComparer<T>? comparer) : base(comparer) { }

	public SortedSet(Func<T, T, int> compareFunction) : base(compareFunction) { }

	public SortedSet(G.IEnumerable<T> collection) : base(collection) { }

	public SortedSet(params T[] array) : base(array) { }

	public SortedSet(ReadOnlySpan<T> span) : base(span) { }

	public SortedSet(int capacity, G.IComparer<T>? comparer) : base(capacity, comparer) { }

	public SortedSet(int capacity, Func<T, T, int> compareFunction) : base(capacity, compareFunction) { }

	public SortedSet(G.IEnumerable<T> collection, G.IComparer<T>? comparer) : base(collection, comparer) { }

	public SortedSet(int capacity, G.IEnumerable<T> collection) : base(capacity, collection) { }

	public SortedSet(int capacity, params T[] array) : base(capacity, array) { }

	public SortedSet(int capacity, ReadOnlySpan<T> span) : base(capacity, span) { }

	public SortedSet(int capacity, G.IEnumerable<T> collection, G.IComparer<T>? comparer) : base(capacity, collection, comparer) { }

	protected override Func<int, SortedSet<T>> CapacityCreator { get; } = x => new(x);

	protected override Func<G.IEnumerable<T>, SortedSet<T>> CollectionCreator { get; } = x => new(x);

	protected override Func<ReadOnlySpan<T>, SortedSet<T>> SpanCreator { get; } = x => new(x);
}
