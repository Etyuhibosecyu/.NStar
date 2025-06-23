namespace LINQ.NStar;

public class ArrayEComparer<T> : IListEComparer<T>, G.IEqualityComparer<T[]>
{
	public ArrayEComparer() : base()
	{
	}

	public ArrayEComparer(Func<T, T, bool> equals) : base(equals)
	{
	}

	public ArrayEComparer(Func<T, T, bool> equals, Func<T, int> hashCode) : base(equals, hashCode)
	{
	}

	public bool Equals(T[]? x, T[]? y) => base.Equals(x, y);

	public int GetHashCode(T[] x) => base.GetHashCode(x);
}

public class ListEComparer<T> : IListEComparer<T>, G.IEqualityComparer<List<T>>
{
	public ListEComparer() : base()
	{
	}

	public ListEComparer(Func<T, T, bool> equals) : base(equals)
	{
	}

	public ListEComparer(Func<T, T, bool> equals, Func<T, int> hashCode) : base(equals, hashCode)
	{
	}

	public bool Equals(List<T>? x, List<T>? y) => base.Equals(x, y);

	public int GetHashCode(List<T> x) => base.GetHashCode(x);
}

public class NListEComparer<T> : G.IEqualityComparer<NList<T>> where T : unmanaged
{
	private protected readonly Func<T, T, bool> equals;
	private protected readonly Func<T, int> hashCode;
	private protected readonly bool defaultEquals;

	public NListEComparer()
	{
		equals = default!;
		hashCode = x => x.GetHashCode();
		defaultEquals = true;
	}

	public NListEComparer(Func<T, T, bool> equals)
	{
		this.equals = equals;
		hashCode = x => x.GetHashCode();
		defaultEquals = false;
	}

	public NListEComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
	{
		this.equals = equals;
		this.hashCode = hashCode;
		defaultEquals = false;
	}

	public NListEComparer(Func<T, int> hashCode)
	{
		equals = default!;
		this.hashCode = hashCode;
		defaultEquals = true;
	}

	public bool Equals(NList<T>? x, NList<T>? y)
	{
		if (x == null && y == null)
			return true;
		else if (x == null || y == null)
			return false;
		else if (defaultEquals)
			return x.Equals(y);
		if (x.Length != y.Length)
			return false;
		for (var i = 0; i < x.Length; i++)
			if (!equals(x[i], y[i]))
				return false;
		return true;
	}

	public int GetHashCode(NList<T> x)
	{
		var hash = 486187739;
		var en = x.GetEnumerator();
		if (en.MoveNext())
		{
			hash = (hash * 16777619) ^ hashCode(en.Current);
			if (en.MoveNext())
			{
				hash = (hash * 16777619) ^ hashCode(en.Current);
				hash = (hash * 16777619) ^ hashCode(x[^1]);
			}
		}
		return hash;
	}
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Group<T, TKey> : List<T>
{
	public virtual TKey Key { get; private protected set; }

	public Group(int capacity, TKey key) : base(capacity) => Key = key;

	public Group(G.IEnumerable<T> collection, TKey key) : base(collection) => Key = key;

	public Group(int capacity, T item, TKey key) : base(capacity, item) => Key = key;
}

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class NGroup<T, TKey> : NList<T> where T : unmanaged
{
	public virtual TKey Key { get; private protected set; }

	public NGroup(int capacity, TKey key) : base(capacity) => Key = key;

	public NGroup(G.IEnumerable<T> collection, TKey key) : base(collection) => Key = key;

	public NGroup(int capacity, T item, TKey key) : base(capacity, item) => Key = key;
}
