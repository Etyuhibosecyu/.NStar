global using NStar.Core;
global using System;
global using G = System.Collections.Generic;
global using static System.Math;
global using String = NStar.Core.String;
using System.Diagnostics.CodeAnalysis;

namespace NStar.RemoveDoubles;

public static class RedStarLinqRemoveDoubles
{
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
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

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
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

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source)
	{
		ListHashSet<T> hs = [];
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

	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
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

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
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

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> hs = new(comparer);
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

	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
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

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
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

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction));
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

	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
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

	[Experimental("CS9216")]
	public static List<T> RemoveDoubles<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
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

	public static List<T> RemoveDoubles<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
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

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = [];
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2)
	{
		ListHashSet<T> hs = [];
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2));
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, comparer);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(comparer);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, comparer);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> hs = new(comparer);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), comparer);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, equalFunction);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, equalFunction);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction));
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), equalFunction);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, equalFunction, hashCodeFunction);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) RemoveDoubles<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), function, equalFunction, hashCodeFunction);
	}

	public static (List<T>, List<T2>) RemoveDoubles<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
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
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
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
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
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
			return RemoveDoubles(List<T>.ReturnOrConstruct(source), List<T>.ReturnOrConstruct(source2), equalFunction, hashCodeFunction);
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
