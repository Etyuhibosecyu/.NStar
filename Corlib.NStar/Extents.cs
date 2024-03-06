global using Mpir.NET;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static System.Math;
using System.Runtime.Serialization;

namespace Corlib.NStar;

public enum PrimitiveType : byte
{
	ByteType,
	ShortType,
	UShortType,
	IntType,
	UIntType,
	LongType,
	ULongType,
}

public class Comparer<T>(Func<T, T, int> comparer) : IComparer<T>
{
	private readonly Func<T, T, int> comparer = comparer;

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

public class ArrayEComparer<T> : IEqualityComparer<T[]>
{
	private readonly Func<T, T, bool> equals;
	private readonly Func<T, int> hashCode;

	public ArrayEComparer()
	{
		equals = EqualityComparer<T>.Default.Equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public ArrayEComparer(Func<T, T, bool> equals)
	{
		this.equals = equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public ArrayEComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
	{
		this.equals = equals;
		this.hashCode = hashCode;
	}

	public bool Equals(T[]? x, T[]? y)
	{
		if (x == null && y == null)
			return true;
		else if (x == null || y == null)
			return false;
		if (x.Length != y.Length)
			return false;
		for (var i = 0; i < x.Length; i++)
			if (!equals(x[i], y[i]))
				return false;
		return true;
	}

	public int GetHashCode(T[] x) => x.Length switch
	{
		0 => 1234567890,
		1 => hashCode(x[0]),
		2 => hashCode(x[0]) << 7 ^ hashCode(x[1]),
		_ => (hashCode(x[0]) << 7 ^ hashCode(x[1])) << 7 ^ hashCode(x[^1]),
	};
}

public class ListEComparer<T> : IEqualityComparer<List<T>>
{
	private readonly Func<T, T, bool> equals;
	private readonly Func<T, int> hashCode;

	public ListEComparer()
	{
		equals = EqualityComparer<T>.Default.Equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public ListEComparer(Func<T, T, bool> equals)
	{
		this.equals = equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public ListEComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
	{
		this.equals = equals;
		this.hashCode = hashCode;
	}

	public bool Equals(List<T>? x, List<T>? y)
	{
		if (x == null && y == null)
			return true;
		else if (x == null || y == null)
			return false;
		if (x.Length != y.Length)
			return false;
		for (var i = 0; i < x.Length; i++)
			if (!equals(x[i], y[i]))
				return false;
		return true;
	}

	public int GetHashCode(List<T> x) => x.Length switch
	{
		0 => 1234567890,
		1 => hashCode(x[0]),
		2 => hashCode(x[0]) << 7 ^ hashCode(x[1]),
		_ => (hashCode(x[0]) << 7 ^ hashCode(x[1])) << 7 ^ hashCode(x[^1]),
	};
}

public class NListEComparer<T> : IEqualityComparer<NList<T>> where T : unmanaged
{
	private readonly Func<T, T, bool> equals;
	private readonly Func<T, int> hashCode;
	private readonly bool defaultEquals;

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

	public int GetHashCode(NList<T> x) => x.Length switch
	{
		0 => 1234567890,
		1 => hashCode(x[0]),
		2 => hashCode(x[0]) << 7 ^ hashCode(x[1]),
		_ => (hashCode(x[0]) << 7 ^ hashCode(x[1])) << 7 ^ hashCode(x[^1]),
	};
}

[Serializable]
public class ExperimentalException : Exception
{
	public ExperimentalException() : this("Внимание! Эта операция является экспериментальной. Используйте на свой страх и риск. Это исключение не прерывает работу программы, а служит только для оповещения. Нажмите F5 для продолжения.") { }

	public ExperimentalException(string? message) : base(message) { }

	public ExperimentalException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class FakeIndexesException : Exception
{
	public FakeIndexesException() : this("Внимание! Вы пытаетесь получить или установить элемент по индексу, но он является фейковым. Вы можете получить недействительный элемент, либо же элемент, действительный \"номер\" которого в коллекции существенно отличается от указанного вами индекса. Это исключение не прерывает работу программы, а служит только для оповещения. Нажмите F5 для продолжения.") { }

	public FakeIndexesException(string? message) : base(message) { }

	public FakeIndexesException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class SlowOperationException : Exception
{
	public SlowOperationException() : this("Внимание! Эта операция будет выполняться очень долго. Это исключение не прерывает работу программы, а служит только для оповещения. Нажмите F5 для продолжения.") { }

	public SlowOperationException(string? message) : base(message) { }

	public SlowOperationException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class ValueNotFoundException : SystemException
{
	public ValueNotFoundException() : this("The given value was not present in the dictionary.") { }

	public ValueNotFoundException(string? message) : base(message) { }

	public ValueNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

public interface IBigCollection<T> : IEnumerable<T>
{
	bool IsReadOnly { get; }
	MpzT Length { get; }

	void Add(T item);
	void Clear();
	bool Contains(T item);
	void CopyTo(T[] array, int arrayIndex);
	void CopyTo(IBigList<T> list, MpzT listIndex);
	bool RemoveValue(T item);
}

public interface IBigList<T> : IBigCollection<T>
{
	T this[MpzT index] { get; set; }

	MpzT IndexOf(T item);
	void Insert(MpzT index, T item);
	void RemoveAt(MpzT index);
}

public interface ICollection : System.Collections.ICollection
{
	int Length { get; }
	int System.Collections.ICollection.Count => Length;
}

public interface ICollection<T> : G.ICollection<T>
{
	int Length { get; }
	int G.ICollection<T>.Count => Length;
	bool G.ICollection<T>.Remove(T item) => RemoveValue(item);
	bool RemoveValue(T item);
}

public interface IDictionary : System.Collections.IDictionary, ICollection
{
}

public interface IDictionary<TKey, TValue> : G.IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
{
}

public interface IList : System.Collections.IList, ICollection
{
}

public interface IList<T> : G.IList<T>, ICollection<T>
{
}

public interface IReadOnlyCollection<T> : G.IReadOnlyCollection<T>
{
	int Length { get; }
	int G.IReadOnlyCollection<T>.Count => Length;
}

public interface IReadOnlyDictionary<TKey, TValue> : G.IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
}

public interface IReadOnlyList<T> : G.IReadOnlyList<T>, IReadOnlyCollection<T>
{
}

public static unsafe partial class Extents
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	public const int BitsPerInt = sizeof(int) * BitsPerByte;
	public const int BytesPerInt = sizeof(int);
	public const int BitsPerByte = 8;

	internal static readonly Random random = new();

	[LibraryImport("kernel32.dll", EntryPoint = "RtlCompareMemory", SetLastError = false)]
	private static partial uint CompareMemory(nint left, nint right, uint length);

	[LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
	private static partial void CopyMemory(nint destination, nint source, uint length);

	[LibraryImport("kernel32.dll", EntryPoint = "RtlFillMemory", SetLastError = false)]
	private static partial void FillMemory(nint destination, uint length, byte fill);

	public static Comparer<T[]> ArraySequentialComparer<T>() where T : unmanaged, IComparable<T> => new((x, y) =>
	{
		var minLength = Min(x.Length, y.Length);
		for (var i = 0; i < minLength; i++)
		{
			if (x[i].CompareTo(y[i]) < 0)
				return -1;
			if (x[i].CompareTo(y[i]) > 0)
				return 1;
		}
		if (x.Length < y.Length)
			return -1;
		if (x.Length > y.Length)
			return 1;
		return 0;
	});

	internal static Span<TSource> AsSpan<TSource>(this TSource[] source) => MemoryExtensions.AsSpan(source);
	internal static Span<TSource> AsSpan<TSource>(this TSource[] source, int index) => MemoryExtensions.AsSpan(source, index);
	internal static Span<TSource> AsSpan<TSource>(this TSource[] source, int index, int length) => MemoryExtensions.AsSpan(source, index, length);

	/// <summary>Считает количество бит в числе. Логарифм для этой цели использовать невыгодно, так как это достаточно медленная операция.</summary>
	/// <param name="x">Исходное число.</param>
	/// <returns>Количество бит в числе.</returns>
	public static int BitLength(this int x) => BitLength((uint)x);

	/// <summary>Считает количество бит в числе. Логарифм для этой цели использовать невыгодно, так как это достаточно медленная операция.</summary>
	/// <param name="x">Исходное число.</param>
	/// <returns>Количество бит в числе.</returns>
	public static int BitLength(this uint x) => ((MpzT)x).BitLength;

	public static int CompareMemory<T>(T* left, T* right, int length) where T : unmanaged => (int)(CompareMemory((nint)left, (nint)right, (uint)(sizeof(T) * length)) / sizeof(T));

	public static int CompareMemory<T>(T* left, int leftIndex, T* right, int rightIndex, int length) where T : unmanaged => CompareMemory(left + leftIndex, right + rightIndex, length);

	public static int CompareMemory<T>(T[] left, T[] right, int length) where T : unmanaged
	{
		fixed (T* left2 = left)
		fixed (T* right2 = right)
			return CompareMemory(left2, right2, length);
	}

	public static int CompareMemory<T>(T[] left, int leftIndex, T[] right, int rightIndex, int length) where T : unmanaged
	{
		fixed (T* left2 = left)
		fixed (T* right2 = right)
			return CompareMemory(left2 + leftIndex, right2 + rightIndex, length);
	}

	public static void CopyMemory<T>(T* source, T* destination, int length) where T : unmanaged => CopyMemory((nint)destination, (nint)source, (uint)(sizeof(T) * length));

	public static void CopyMemory<T>(T* source, int sourceIndex, T* destination, int destinationIndex, int length) where T : unmanaged => CopyMemory(source + sourceIndex, destination + destinationIndex, length);

	public static void CopyMemory<T>(T[] source, T[] destination, int length) where T : unmanaged
	{
		fixed (T* source2 = source)
		fixed (T* destination2 = destination)
			CopyMemory(source2, destination2, length);
	}

	public static void CopyMemory<T>(T[] source, int sourceIndex, T[] destination, int destinationIndex, int length) where T : unmanaged
	{
		fixed (T* source2 = source)
		fixed (T* destination2 = destination)
			CopyMemory(source2 + sourceIndex, destination2 + destinationIndex, length);
	}

	public static T CreateVar<T>(T value, out T @out) => @out = value;

	public static (MpzT Quotient, int Remainder) DivRem(MpzT left, int right)
	{
		var quotient = left.Divide(right, out int remainder);
		return (quotient, remainder);
	}

	public static (MpzT Quotient, MpzT Remainder) DivRem(MpzT left, MpzT right)
	{
		var quotient = left.Divide(right, out var remainder);
		return (quotient, remainder);
	}

	public static bool EqualMemory<T>(T* left, T* right, int length) where T : unmanaged => CompareMemory((nint)left, (nint)right, (uint)(sizeof(T) * length)) == sizeof(T) * length;

	public static void FillMemory<T>(T* source, int length, byte fill) where T : unmanaged => FillMemory((nint)source, (uint)(sizeof(T) * length), fill);

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
	public static int GetArrayLength(int n, int div) => n > 0 ? ((n - 1) / div + 1) : 0;

	public static MpzT GetArrayLength(MpzT n, MpzT div) => n > 0 ? ((n - 1) / div + 1) : 0;

	public static void Lock(object lockObj, Action function)
	{
		lock (lockObj)
			function();
	}

	public static void Lock<T>(object lockObj, Action<T> function, T arg)
	{
		lock (lockObj)
			function(arg);
	}

	public static void Lock<T1, T2>(object lockObj, Action<T1, T2> function, T1 arg1, T2 arg2)
	{
		lock (lockObj)
			function(arg1, arg2);
	}

	public static void Lock<T1, T2, T3>(object lockObj, Action<T1, T2, T3> function, T1 arg1, T2 arg2, T3 arg3)
	{
		lock (lockObj)
			function(arg1, arg2, arg3);
	}

	public static void Lock<T1, T2, T3, T4>(object lockObj, Action<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4);
	}

	public static void Lock<T1, T2, T3, T4, T5>(object lockObj, Action<T1, T2, T3, T4, T5> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6>(object lockObj, Action<T1, T2, T3, T4, T5, T6> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
	}

	public static void Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(object lockObj, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
	{
		lock (lockObj)
			function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
	}

	public static TResult Lock<TResult>(object lockObj, Func<TResult> function)
	{
		lock (lockObj)
			return function();
	}

	public static TResult Lock<T, TResult>(object lockObj, Func<T, TResult> function, T arg)
	{
		lock (lockObj)
			return function(arg);
	}

	public static TResult Lock<T1, T2, TResult>(object lockObj, Func<T1, T2, TResult> function, T1 arg1, T2 arg2)
	{
		lock (lockObj)
			return function(arg1, arg2);
	}

	public static TResult Lock<T1, T2, T3, TResult>(object lockObj, Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3);
	}

	public static TResult Lock<T1, T2, T3, T4, TResult>(object lockObj, Func<T1, T2, T3, T4, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
	}

	public static TResult Lock<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(object lockObj, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
	{
		lock (lockObj)
			return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
	}

	public static T[] NSort<T>(this T[] array) where T : unmanaged => NSort(array, 0, array.Length);

	public static T[] NSort<T>(this T[] array, int index, int length) where T : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > array.Length)
			throw new ArgumentException(null);
		fixed (T* items = array)
		{
			var shiftedItems = items + index;
			RadixSort(shiftedItems, length);
		}
		return array;
	}

	public static T[] NSort<T>(this T[] array, Func<T, byte> function) => NSort(array, function, 0, array.Length);

	public static T[] NSort<T>(this T[] array, Func<T, byte> function, int index, int length) => NSort<T, byte>(array, function, index, length);

	public static T* NSort<T>(T* array, Func<T, byte> function, int index, int length) where T : unmanaged => NSort<T, byte>(array, function, index, length);

	public static T[] NSort<T>(this T[] array, Func<T, uint> function) => NSort(array, function, 0, array.Length);

	public static T[] NSort<T>(this T[] array, Func<T, uint> function, int index, int length) => NSort<T, uint>(array, function, index, length);

	public static T* NSort<T>(T* array, Func<T, uint> function, int index, int length) where T : unmanaged => NSort<T, uint>(array, function, index, length);

	public static T[] NSort<T>(this T[] array, Func<T, ushort> function) => NSort(array, function, 0, array.Length);

	public static T[] NSort<T>(this T[] array, Func<T, ushort> function, int index, int length) => NSort<T, ushort>(array, function, index, length);

	public static T* NSort<T>(T* array, Func<T, ushort> function, int index, int length) where T : unmanaged => NSort<T, ushort>(array, function, index, length);

	public static T[] NSort<T, T2>(this T[] array, Func<T, T2> function, int index, int length) where T2 : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if (index + length > array.Length)
			throw new ArgumentException(null);
		var converted = (T2*)Marshal.AllocHGlobal(sizeof(T2) * length);
		var indexes = (int*)Marshal.AllocHGlobal(sizeof(int) * length);
		for (var i = 0; i < length; i++)
		{
			converted[i] = function(array[index + i]);
			indexes[i] = i;
		}
		RadixSort(converted, indexes, length);
		Marshal.FreeHGlobal((nint)converted);
		var oldItems = array[index..(index + length)];
		for (var i = 0; i < length; i++)
			array[index + i] = oldItems[indexes[i]];
		Marshal.FreeHGlobal((nint)indexes);
		return array;
	}

	private static T* NSort<T, T2>(T* array, Func<T, T2> function, int index, int length) where T : unmanaged where T2 : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var converted = (T2*)Marshal.AllocHGlobal(sizeof(T2) * length);
		var indexes = (int*)Marshal.AllocHGlobal(sizeof(int) * length);
		for (var i = 0; i < length; i++)
		{
			converted[i] = function(array[index + i]);
			indexes[i] = i;
		}
		RadixSort(converted, indexes, length);
		Marshal.FreeHGlobal((nint)converted);
		var oldItems = (T*)Marshal.AllocHGlobal(sizeof(T) * length);
		CopyMemory(array + index, oldItems, length);
		for (var i = 0; i < length; i++)
			array[index + i] = oldItems[indexes[i]];
		Marshal.FreeHGlobal((nint)oldItems);
		Marshal.FreeHGlobal((nint)indexes);
		return array;
	}

	public static int NthAbsent<TCertain>(this BaseSortedSet<int, TCertain> set, int n) where TCertain : BaseSortedSet<int, TCertain>, new()
	{
		ArgumentNullException.ThrowIfNull(set);
		if (set.Comparer != G.Comparer<int>.Default)
			throw new ArgumentException("Множество должно иметь стандартный для int компаратор.", nameof(set));
		if (set.Length == 0)
			return n;
		if (set[0] < 0)
			throw new ArgumentException("Не допускается множество, содержащее отрицательные значения.", nameof(set));
		var lo = 0;
		var hi = set.Length - 1;
		var comparer = G.Comparer<int>.Default;
		while (lo <= hi)
		{
			// i might overflow if lo and hi are both large positive numbers. 
			var i = lo + ((hi - lo) >> 1);
			int c;
			try
			{
				c = comparer.Compare(set[i] - i, n);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(null, ex);
			}
			if (c == 0)
			{
				var result = n + i + 1;
				while (++i < set.Length && set[i] == result)
					result++;
				return result;
			}
			if (c < 0)
				lo = i + 1;
			else
				hi = i - 1;
		}
		return n + lo;
	}

	internal static void RadixSort<T>(T* @in, int n) where T : unmanaged
	{
		var @out = (T*)Marshal.AllocHGlobal(sizeof(T) * n);
		int* counters = (int*)Marshal.AllocHGlobal(256 * sizeof(T) * sizeof(int)), count;
		CreateCounters(@in, counters, n);
		for (var i = 0; i < sizeof(T); i++)
		{
			count = counters + 256 * i;
			if (count[0] != n)
				RadixPass(i, n, @in, @out, count);
			if (i == sizeof(T) - 1 && (i & 1) == 0)
			{
				if (count[0] != n)
					CopyMemory(@out, @in, n);
			}
			else
			{
				if (count[0] == n)
					CopyMemory(@in, @out, n);
				var temp = @in;
				@in = @out;
				@out = temp;
			}
		}
		Marshal.FreeHGlobal((nint)@out);
		Marshal.FreeHGlobal((nint)counters);
	}

	internal static void RadixSort<T, T2>(T* @in, T2* in2, int n) where T : unmanaged where T2 : unmanaged
	{
		var @out = (T*)Marshal.AllocHGlobal(sizeof(T) * n);
		var @out2 = (T2*)Marshal.AllocHGlobal(sizeof(T2) * n);
		int* counters = (int*)Marshal.AllocHGlobal(256 * sizeof(T) * sizeof(int)), count;
		CreateCounters(@in, counters, n);
		var countPasses = 0;
		for (var i = 0; i < sizeof(T); i++)
		{
			count = counters + 256 * i;
			if (count[0] != n)
			{
				RadixPass(i, n, @in, in2, @out, out2, count);
				countPasses++;
			}
			if (i != sizeof(T) - 1 || (i & 1) != 0)
			{
				var temp = @in;
				@in = @out;
				@out = temp;
				var temp2 = in2;
				in2 = @out2;
				@out2 = temp2;
			}
		}
		if ((countPasses & 1) != 0)
		{
			CopyMemory(@out, @in, n);
			CopyMemory(out2, in2, n);
		}
		Marshal.FreeHGlobal((nint)@out);
		Marshal.FreeHGlobal((nint)out2);
		Marshal.FreeHGlobal((nint)counters);
	}

	private static void RadixPass<T>(int offset, int n, T* @in, T* @out, int* count) where T : unmanaged
	{
		T* sp;
		int s, c, i;
		byte* bp;
		s = 0;
		var cp = count;
		for (i = 256; i > 0; --i, ++cp)
		{
			c = *cp;
			*cp = s;
			s += c;
		}
		bp = (byte*)@in + offset;
		sp = @in;
		for (i = n; i > 0; --i, bp += sizeof(T), ++sp)
		{
			cp = count + *bp;
			@out[*cp] = *sp;
			++*cp;
		}
	}

	private static void RadixPass<T, T2>(int offset, int n, T* @in, T2* in2, T* @out, T2* out2, int* count) where T : unmanaged where T2 : unmanaged
	{
		T* sp;
		T2* sp2;
		int s, c, i;
		byte* bp;
		s = 0;
		var cp = count;
		for (i = 256; i > 0; --i, ++cp)
		{
			c = *cp;
			*cp = s;
			s += c;
		}
		bp = (byte*)@in + offset;
		sp = @in;
		sp2 = in2;
		for (i = n; i > 0; --i, bp += sizeof(T), ++sp, ++sp2)
		{
			cp = count + *bp;
			@out[*cp] = *sp;
			out2[*cp] = *sp2;
			++*cp;
		}
	}

	private static void CreateCounters<T>(T* data, int* counters, int n) where T : unmanaged
	{
		FillMemory((nint)counters, (uint)(256 * sizeof(T) * sizeof(int)), 0);
		var bp = (byte*)data;
		var dataEnd = (byte*)(data + n);
		int i;
		while (bp != dataEnd)
			for (i = 0; i < sizeof(T); i++)
				counters[256 * i + *bp++]++;
	}

	public static uint ReverseBits(uint n)
	{
		n = (n >> 1) & 0x55555555 | (n << 1) & 0xaaaaaaaa;
		n = (n >> 2) & 0x33333333 | (n << 2) & 0xcccccccc;
		n = (n >> 4) & 0x0f0f0f0f | (n << 4) & 0xf0f0f0f0;
		n = (n >> 8) & 0x00ff00ff | (n << 8) & 0xff00ff00;
		n = (n >> 16) & 0x0000ffff | (n << 16) & 0xffff0000;
		return n;
	}

	public static T[] Sort<T>(this T[] source)
	{
		Array.Sort(source);
		return source;
	}

	public static T[] Sort<T>(this T[] source, IComparer<T> comparer)
	{
		Array.Sort(source, comparer);
		return source;
	}

	public static T[] Sort<T>(this T[] source, int index, int length)
	{
		Array.Sort(source, index, length);
		return source;
	}

	public static T[] Sort<T>(this T[] source, int index, int length, IComparer<T> comparer)
	{
		Array.Sort(source, index, length, comparer);
		return source;
	}

	public static PrimitiveType GetPrimitiveType<T>() where T : unmanaged => typeof(T).Equals(typeof(byte)) ? PrimitiveType.ByteType : typeof(T).Equals(typeof(short)) ? PrimitiveType.ShortType : typeof(T).Equals(typeof(ushort)) ? PrimitiveType.UShortType : typeof(T).Equals(typeof(int)) ? PrimitiveType.IntType : typeof(T).Equals(typeof(uint)) ? PrimitiveType.UIntType : typeof(T).Equals(typeof(long)) ? PrimitiveType.LongType : typeof(T).Equals(typeof(ulong)) ? PrimitiveType.ULongType : throw new InvalidOperationException();

	public static int ToInt<T>(T item, PrimitiveType type) where T : unmanaged => type switch
	{
		PrimitiveType.ByteType => (byte)(object)item,
		PrimitiveType.ShortType => (short)(object)item,
		PrimitiveType.UShortType => (ushort)(object)item,
		PrimitiveType.IntType => (int)(object)item,
		PrimitiveType.UIntType => (int)(uint)(object)item,
		PrimitiveType.LongType => (int)(long)(object)item,
		PrimitiveType.ULongType => (int)(ulong)(object)item,
		_ => default,
	};

	public static uint ToUInt<T>(T item, PrimitiveType type) where T : unmanaged => type switch
	{
		PrimitiveType.ByteType => (byte)(object)item,
		PrimitiveType.ShortType => (uint)(short)(object)item,
		PrimitiveType.UShortType => (ushort)(object)item,
		PrimitiveType.IntType => (uint)(int)(object)item,
		PrimitiveType.UIntType => (uint)(object)item,
		PrimitiveType.LongType => (uint)(long)(object)item,
		PrimitiveType.ULongType => (uint)(ulong)(object)item,
		_ => default,
	};

	public static T[] TryNSort<T>(this T[] array, IComparer<T>? comparer = null) => TryNSort(array, 0, array.Length, comparer);

	public static T[] TryNSort<T>(this T[] array, int index, int length, IComparer<T>? comparer = null)
	{
		if (typeof(Extents).GetMethods().Find(x => x.Name == "NSort" && x.GetParameters().Wrap(y => y.Length == 3 && y[0].ParameterType.IsArray && y[1].ParameterType == typeof(int) && y[2].ParameterType == typeof(int)))?.MakeGenericMethod(typeof(T))?.Invoke(null, [array, index, length]) == null)
			Array.Sort(array, index, length, comparer);
		return array;
	}
}
