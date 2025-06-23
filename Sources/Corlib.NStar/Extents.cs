﻿global using Mpir.NET;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static System.Math;

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
	private protected readonly Func<T, T, int> comparer = comparer;

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
	private protected readonly Func<T, T, bool> equals;
	private protected readonly Func<T, int> hashCode;

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

public class IListEComparer<T> : IEqualityComparer<G.IList<T>>
{
	private protected readonly Func<T, T, bool> equals;
	private protected readonly Func<T, int> hashCode;

	public IListEComparer()
	{
		equals = EqualityComparer<T>.Default.Equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public IListEComparer(Func<T, T, bool> equals)
	{
		this.equals = equals;
		hashCode = x => x?.GetHashCode() ?? 0;
	}

	public IListEComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
	{
		this.equals = equals;
		this.hashCode = hashCode;
	}

	public bool Equals(G.IList<T>? x, G.IList<T>? y)
	{
		if (x == null && y == null)
			return true;
		else if (x == null || y == null)
			return false;
		if (x.Count != y.Count)
			return false;
		for (var i = 0; i < x.Count; i++)
			if (!equals(x[i], y[i]))
				return false;
		return true;
	}

	public int GetHashCode(G.IList<T> x)
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

[Serializable]
public class SlowOperationException : Exception
{
	public SlowOperationException() : this("Внимание! Эта операция будет выполняться очень долго. Это исключение не прерывает работу программы, а служит только для оповещения. Нажмите F5 для продолжения.") { }

	public SlowOperationException(string? message) : base(message) { }

	public SlowOperationException(string? message, Exception? innerException) : base(message, innerException) { }
}

public interface ICollection : System.Collections.ICollection
{
	int Length { get; }
	int System.Collections.ICollection.Count => Length;
}

public interface ICollection<T> : G.ICollection<T>, ICollection
{
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

public static unsafe class Extents
{
	// XPerY=n means that n Xs can be stored in 1 Y. 
	public const int BitsPerInt = sizeof(int) * BitsPerByte;
	public const int BytesPerInt = sizeof(int);
	public const int BitsPerByte = 8;
	public const int ValuesInByte = 1 << BitsPerByte;
	public const int ValuesIn2Bytes = ValuesInByte << BitsPerByte;
	public const int ValuesIn3Bytes = ValuesIn2Bytes << BitsPerByte;

	internal static readonly Random random = new();

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

	public static int CompareMemory<T>(T* left, T* right, int length) where T : unmanaged => new Span<T>(left, length).CommonPrefixLength(new Span<T>(right, length));

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

	public static void CopyMemory<T>(T* source, T* destination, int length) where T : unmanaged => new Span<T>(source, length).CopyTo(new Span<T>(destination, length));

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

	public static bool EqualMemory<T>(T* left, T* right, int length) where T : unmanaged => new Span<T>(left, length).CommonPrefixLength(new Span<T>(right, length)) == length;

	public static bool EqualMemory<T>(T* left, int leftIndex, T* right, int rightIndex, int length) where T : unmanaged => EqualMemory(left + leftIndex, right + rightIndex, length);

	public static bool EqualMemory<T>(T[] left, T[] right, int length) where T : unmanaged
	{
		fixed (T* left2 = left)
		fixed (T* right2 = right)
			return EqualMemory(left2, right2, length);
	}

	public static bool EqualMemory<T>(T[] left, int leftIndex, T[] right, int rightIndex, int length) where T : unmanaged
	{
		fixed (T* left2 = left)
		fixed (T* right2 = right)
			return EqualMemory(left2 + leftIndex, right2 + rightIndex, length);
	}

	public static void FillMemory<T>(T* source, int length, byte fill) where T : unmanaged => new Span<byte>((byte*)source, sizeof(T) * length).Fill(fill);

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

	public static MpzT GetOffset(this Index index, MpzT length) => index.IsFromEnd ? length - index.Value : index.Value;

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
			throw new ArgumentException("Сортируемый диапазон выходит за размер исходного массива.");
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
			throw new ArgumentException("Сортируемый диапазон выходит за размер исходного массива.");
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
		Parallel.For(0, length, i =>
		{
			converted[i] = function(array[index + i]);
			indexes[i] = i;
		});
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

	internal static void RadixSort<T>(T* @in, int n) where T : unmanaged
	{
		var @out = (T*)Marshal.AllocHGlobal(sizeof(T) * n);
		int* counters = (int*)Marshal.AllocHGlobal(ValuesInByte * sizeof(T) * sizeof(int)), count;
		CreateCounters(@in, counters, n);
		for (var i = 0; i < sizeof(T); i++)
		{
			count = counters + ValuesInByte * i;
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
		int* counters = (int*)Marshal.AllocHGlobal(ValuesInByte * sizeof(T) * sizeof(int)), count;
		CreateCounters(@in, counters, n);
		var countPasses = 0;
		for (var i = 0; i < sizeof(T); i++)
		{
			count = counters + ValuesInByte * i;
			if (count[0] != n)
			{
				RadixPass(i, n, @in, in2, @out, out2, count);
				countPasses++;
			}
			if (i == sizeof(T) - 1 && (i & 1) == 0)
			{
				if (count[0] != n)
				{
					CopyMemory(@out, @in, n);
					CopyMemory(out2, in2, n);
				}
			}
			else
			{
				if (count[0] == n)
				{
					CopyMemory(@in, @out, n);
					CopyMemory(in2, out2, n);
				}
				var temp = @in;
				@in = @out;
				@out = temp;
				var temp2 = in2;
				in2 = @out2;
				@out2 = temp2;
			}
		}
		Marshal.FreeHGlobal((nint)@out);
		Marshal.FreeHGlobal((nint)out2);
		Marshal.FreeHGlobal((nint)counters);
	}

	private static void RadixPass<T>(int offset, int n, T* @in, T* @out, int* count) where T : unmanaged
	{
		RadixPassMain(offset, @in, count, out var sp, out var i, out var bp, out var cp);
		for (i = n; i > 0; --i, bp += sizeof(T), ++sp)
		{
			cp = count + *bp;
			@out[*cp] = *sp;
			++*cp;
		}
	}

	private static void RadixPass<T, T2>(int offset, int n, T* @in, T2* in2, T* @out, T2* out2, int* count) where T : unmanaged where T2 : unmanaged
	{
		T2* sp2;
		RadixPassMain(offset, @in, count, out var sp, out var i, out var bp, out var cp);
		sp2 = in2;
		for (i = n; i > 0; --i, bp += sizeof(T), ++sp, ++sp2)
		{
			cp = count + *bp;
			@out[*cp] = *sp;
			out2[*cp] = *sp2;
			++*cp;
		}
	}

	private static void RadixPassMain<T>(int offset, T* @in, int* count, out T* sp, out int i, out byte* bp, out int* cp) where T : unmanaged
	{
		int s, c;
		s = 0;
		cp = count;
		for (i = ValuesInByte; i > 0; --i, ++cp)
		{
			c = *cp;
			*cp = s;
			s += c;
		}
		bp = (byte*)@in + offset;
		sp = @in;
	}

	private static void CreateCounters<T>(T* data, int* counters, int n) where T : unmanaged
	{
		FillMemory((byte*)counters, ValuesInByte * sizeof(T) * sizeof(int), 0);
		var bp = (byte*)data;
		var dataEnd = (byte*)(data + n);
		for (var i = 0; i < n; i++)
		{
			for (var j = 0; j < sizeof(T); j++)
			{
				var value = *(bp + i * sizeof(T) + j);
				counters[ValuesInByte * j + value]++;
			}
		}
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

	public static PrimitiveType GetPrimitiveType<T>() where T : unmanaged =>
		typeof(T).Equals(typeof(byte)) ? PrimitiveType.ByteType
		: typeof(T).Equals(typeof(short)) ? PrimitiveType.ShortType
		: typeof(T).Equals(typeof(ushort)) ? PrimitiveType.UShortType
		: typeof(T).Equals(typeof(int)) ? PrimitiveType.IntType
		: typeof(T).Equals(typeof(uint)) ? PrimitiveType.UIntType
		: typeof(T).Equals(typeof(long)) ? PrimitiveType.LongType
		: typeof(T).Equals(typeof(ulong)) ? PrimitiveType.ULongType
		: throw new InvalidOperationException("Поддерживаются только типы byte, short, ushort, int, uint, long, ulong.");

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
}
