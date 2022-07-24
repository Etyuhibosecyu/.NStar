using Mpir.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BigCollections;

public class Comparer<T> : IComparer<T>
{
	private readonly Func<T, T, int> comparer;

	public Comparer(Func<T, T, int> comparer) => this.comparer = comparer;

	public int Compare(T? x, T? y)
	{
		if (x == null)
		{
			if (y == null)
				return 0;
			else
				return -1;
		}
		else
		{
			if (y == null)
				return 1;
			else
				return comparer(x, y);
		}
	}
}

public class EComparer<T> : IEqualityComparer<T>
{
	private readonly Func<T, T, bool> equals;
	private readonly Func<T, int> hashCode;

	public EComparer(Func<T, T, bool> equals)
	{
		this.equals = equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public EComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
	{
		this.equals = equals;
		this.hashCode = hashCode;
	}

	public bool Equals(T? x, T? y)
	{
		if (x == null && y == null)
			return true;
		else if (x != null && y != null)
			return equals(x, y);
		else
			return false;
	}

	public int GetHashCode(T obj) => hashCode(obj);
}

[Serializable]
public class SlowOperationException : Exception
{
	public SlowOperationException()
	{
	}
	public SlowOperationException(string message) : base(message)
	{
	}
	public SlowOperationException(string message, Exception inner) : base(message, inner)
	{
	}
	protected SlowOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}

public interface IBigCollection<T> : IEnumerable<T>
{
	mpz_t Count { get; }
	bool IsReadOnly { get; }

	void Add(T item);
	void Clear();
	bool Contains(T item);
	void CopyTo(T[] array, int arrayIndex);
	bool Remove(T item);
}

public interface IBigList<T> : IBigCollection<T>
{
	T this[mpz_t index] { get; set; }

	mpz_t IndexOf(T item);
	void Insert(mpz_t index, T item);
	void RemoveAt(mpz_t index);
}

public static class Extents
{
	internal static Span<TSource> AsSpan<TSource>(this TSource[] source) => MemoryExtensions.AsSpan(source);
	internal static Span<TSource> AsSpan<TSource>(this TSource[] source, int index) => MemoryExtensions.AsSpan(source, index);
	internal static Span<TSource> AsSpan<TSource>(this TSource[] source, int index, int count) => MemoryExtensions.AsSpan(source, index, count);

	public static T Create<T>(int capacity, Func<int, T> creator) => creator(capacity);

	public static T Create<T>(mpz_t capacity, Func<mpz_t, T> creator) => creator(capacity);

	public static T Create<T, TSource>(IEnumerable<TSource> collection, Func<IEnumerable<TSource>, T> creator) => creator(collection);

	public static (mpz_t Quotient, int Remainder) DivRem(mpz_t left, int right)
	{
		mpz_t quotient = left.Divide(right, out int remainder);
		return (quotient, remainder);
	}

	public static (mpz_t Quotient, mpz_t Remainder) DivRem(mpz_t left, mpz_t right)
	{
		mpz_t quotient = left.Divide(right, out mpz_t remainder);
		return (quotient, remainder);
	}

	/// <summary>
	/// Used for conversion between different representations of bit array. 
	/// Returns (n+(div-1))/div, rearranged to avoid arithmetic overflow. 
	/// For example, in the bit to int case, the straightforward calc would 
	/// be (n+31)/BitsPerInt32, but that would cause overflow. So instead it's 
	/// rearranged to ((n-1)/BitsPerInt32) + 1, with special casing for 0.
	/// 
	/// Usage:
	/// GetArrayLength(77, BitsPerInt32): returns how many ints must be 
	/// allocated to store 77 bits.
	/// </summary>
	/// <param name="n"></param>
	/// <param name="div">use a conversion constant, e.g. BytesPerInt32 to get
	/// how many ints are required to store n bytes</param>
	/// <returns></returns>
	public static int GetArrayLength(int n, int div) => n > 0 ? (((n - 1) / div) + 1) : 0;

}
