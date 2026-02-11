global using NStar.Core;
global using System;
global using G = System.Collections.Generic;
global using static System.Math;
using System.Diagnostics.CodeAnalysis;

namespace NStar.RemoveDoubles;

public static class RedStarLinqRemoveDoubles
{
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
		return RemoveDoublesInternal(source, function, hs);
	}

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
		return RemoveDoublesInternal(source, function, hs);
	}

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source)
	{
		ListHashSet<T> hs = [];
		return RemoveDoublesInternal(source, hs);
	}

	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function,
		G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
		return RemoveDoublesInternal(source, function, hs);
	}

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function,
		G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
		return RemoveDoublesInternal(source, function, hs);
	}

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> hs = new(comparer);
		return RemoveDoublesInternal(source, hs);
	}

	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function,
		Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		return RemoveDoublesInternal(source, function, hs);
	}

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function,
		Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		return RemoveDoublesInternal(source, function, hs);
	}

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction));
		return RemoveDoublesInternal(source, hs);
	}

	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function,
		Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		return RemoveDoublesInternal(source, function, hs);
	}

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function,
		Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		return RemoveDoublesInternal(source, function, hs);
	}

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source,
		Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		return RemoveDoublesInternal(source, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2)
	{
		ListHashSet<T> hs = [];
		return RemoveDoublesInternal(source, source2, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> hs = new(comparer);
		return RemoveDoublesInternal(source, source2, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction));
		return RemoveDoublesInternal(source, source2, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		return RemoveDoublesInternal(source, source2, function, hs);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		return RemoveDoublesInternal(source, source2, hs);
	}

	private static List<T> RemoveDoublesInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		ListHashSet<TResult> hs)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (hs.TryAdd(function(item)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (hs.TryAdd(function(item)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (hs.TryAdd(function(item)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (hs.TryAdd(function(item)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (hs.TryAdd(function(item)))
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<T> RemoveDoublesInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		ListHashSet<TResult> hs)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (hs.TryAdd(function(item, i)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (hs.TryAdd(function(item, i)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (hs.TryAdd(function(item, i)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (hs.TryAdd(function(item, i)))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (hs.TryAdd(function(item, i)))
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<T> RemoveDoublesInternal<T>(G.IEnumerable<T> source, ListHashSet<T> hs)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (hs.TryAdd(item))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (hs.TryAdd(item))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (hs.TryAdd(item))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (hs.TryAdd(item))
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (hs.TryAdd(item))
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static (List<T>, List<T2>) RemoveDoublesInternal<T, T2, TResult>(G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, TResult> function, ListHashSet<TResult> hs)
	{
		if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (hs.TryAdd(function(item)))
				{
					result.Add(item);
					result2.Add(item2);
				}
			}
			result.TrimExcess();
			result2.TrimExcess();
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (hs.TryAdd(function(item)))
				{
					result.Add(item);
					result2.Add(item2);
				}
			}
			result.TrimExcess();
			result2.TrimExcess();
			return (result, result2);
		}
		else
			return RemoveDoublesInternal(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, hs);
	}

	private static (List<T>, List<T2>) RemoveDoublesInternal<T, T2, TResult>(G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		Func<T, int, TResult> function, ListHashSet<TResult> hs)
	{
		if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (hs.TryAdd(function(item, i)))
				{
					result.Add(item);
					result2.Add(item2);
				}
			}
			result.TrimExcess();
			result2.TrimExcess();
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (hs.TryAdd(function(item, i)))
				{
					result.Add(item);
					result2.Add(item2);
				}
			}
			result.TrimExcess();
			result2.TrimExcess();
			return (result, result2);
		}
		else
			return RemoveDoublesInternal(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, hs);
	}

	private static (List<T>, List<T2>) RemoveDoublesInternal<T, T2>(G.IEnumerable<T> source, G.IEnumerable<T2> source2,
		ListHashSet<T> hs)
	{
		if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (hs.TryAdd(item))
				{
					result.Add(item);
					result2.Add(item2);
				}
			}
			result.TrimExcess();
			result2.TrimExcess();
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (hs.TryAdd(item))
				{
					result.Add(item);
					result2.Add(item2);
				}
			}
			result.TrimExcess();
			result2.TrimExcess();
			return (result, result2);
		}
		else
			return RemoveDoublesInternal(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), hs);
	}

	public static List<T> RemoveDoubles<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(G.EqualityComparer<TResult>.Default);
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(function(item)))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(G.EqualityComparer<TResult>.Default);
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i)))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T>(this ReadOnlySpan<T> source)
	{
		ListHashSet<T> hs = new(G.EqualityComparer<T>.Default);
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(item))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(function(item)))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i)))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction));
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(item))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(function(item)))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i)))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> RemoveDoubles<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (hs.TryAdd(item))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}
}
