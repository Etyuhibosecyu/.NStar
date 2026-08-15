namespace NStar.Linq;

public class ArrayEComparer<T> : Core.ListEComparer<T>, G.IEqualityComparer<T[]>
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

[ComVisible(true), DebuggerDisplay("Length = {Length}"), Serializable]
public class Group<T, TKey> : List<T>
{
	public virtual TKey Key { get; private protected set; }

	public Group(int capacity, TKey key) : base(capacity) => Key = key;

	public Group(G.IEnumerable<T> collection, TKey key) : base(collection) => Key = key;

	public Group(int capacity, T item, TKey key) : base(capacity, item) => Key = key;
}
