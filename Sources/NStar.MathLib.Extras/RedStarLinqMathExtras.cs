global using Mpir.NET;
global using NStar.Core;
global using NStar.Linq;
global using System;
global using System.Numerics;
global using static NStar.Core.Extents;
global using static System.Math;
global using G = System.Collections.Generic;

namespace NStar.MathLib.Extras;

public static class RedStarLinqMathExtras
{
	internal static readonly Random random = new();

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMaxInternal(source, function);
	}

	private static List<T> FindAllMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result.Add(item);
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result.Add(item);
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result.Add(item);
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<T> FindAllMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(item);
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(item);
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(item);
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindAllMinInternal(source, function);
	}

	private static List<T> FindAllMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result.Add(item);
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result.Add(item);
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result.Add(item);
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<T> FindAllMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(item);
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(item);
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(item);
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result.Clear();
					result.Add(item);
				}
				else if (f == indicator!)
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxInternal(source, function);
	}

	private static T? FindLastMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
			return FindLastMaxInternal(new List<T>(source), function);
	}

	private static T? FindLastMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
			return FindLastMaxInternal(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinInternal(source, function);
	}

	private static T? FindLastMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
			return FindLastMinInternal(new List<T>(source), function);
	}

	private static T? FindLastMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
			return FindLastMinInternal(new List<T>(source), function);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMaxIndexInternal(source, function, out indicator);
	}

	private static int FindLastMaxIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return FindLastMaxIndexInternal([.. source], function, out indicator);
	}

	private static int FindLastMaxIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return FindLastMaxIndexInternal([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindLastMinIndexInternal(source, function, out indicator);
	}

	private static int FindLastMinIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return FindLastMinIndexInternal([.. source], function, out indicator);
	}

	private static int FindLastMinIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return FindLastMinIndexInternal([.. source], function, out indicator);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxInternal(source, function);
	}

	private static T? FindMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
		{
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = item;
				}
				i++;
			}
			return result;
		}
	}

	private static T? FindMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
		{
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = item;
				}
				i++;
			}
			return result;
		}
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinInternal(source, function);
	}

	private static T? FindMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
		{
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result = item;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = item;
				}
				i++;
			}
			return result;
		}
	}

	private static T? FindMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = item;
				}
			}
			return result;
		}
		else
		{
			T? result = default;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result = item;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = item;
				}
				i++;
			}
			return result;
		}
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexInternal(source, function, out indicator);
	}

	private static int FindMaxIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	private static int FindMaxIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexInternal(source, function, out indicator);
	}

	private static int FindMinIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	private static int FindMinIndexInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMaxIndexesInternal(source, function, out indicator);
	}

	private static List<int> FindMaxIndexesInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result.Add(i);
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<int> FindMaxIndexesInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(i);
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	public static List<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		return FindMinIndexesInternal(source, function, out indicator);
	}

	private static List<int> FindMinIndexesInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result.Add(i);
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<int> FindMinIndexesInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function,
		out TResult indicator) where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(i);
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMaxInternal(source, function);
	}

	public static List<int> IndexesOfMax(this G.IEnumerable<decimal> source) => IndexesOfMaxInternal(source);

	public static List<int> IndexesOfMax(this G.IEnumerable<double> source) => IndexesOfMaxInternal(source);

	public static List<int> IndexesOfMax(this G.IEnumerable<int> source) => IndexesOfMaxInternal(source);

	public static List<int> IndexesOfMax(this G.IEnumerable<uint> source) => IndexesOfMaxInternal(source);

	public static List<int> IndexesOfMax(this G.IEnumerable<long> source) => IndexesOfMaxInternal(source);

	public static List<int> IndexesOfMax(this G.IEnumerable<MpzT> source) => IndexesOfMaxInternal(source);

	private static List<int> IndexesOfMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result.Add(i);
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<int> IndexesOfMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(i);
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<int> IndexesOfMaxInternal<T>(G.IEnumerable<T> source) where T : INumber<T>
	{
		if (source is G.IList<T> list)
		{
			var length = list.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = T.Zero;
			var j = 0;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
				{
					indicator = item;
					result[j++] = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = T.Zero;
			var j = 0;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = item;
					result[j++] = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = T.Zero;
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = item;
					result.Add(i);
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = list2.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = list3.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is decimal[] array)
		{
			var list_ = List<decimal>.ReturnOrConstruct(array);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list2);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = List<decimal>.ReturnOrConstruct(source);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
		{
			var list_ = List<double>.ReturnOrConstruct(list);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is double[] array)
		{
			var list_ = List<double>.ReturnOrConstruct(array);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = List<double>.ReturnOrConstruct(list2);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = List<double>.ReturnOrConstruct(source);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
		{
			var value = (int)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return list.IndexesOf(value);
		}
		else if (source is int[] array)
		{
			var value = (int)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return array.IndexesOf(value);
		}
		else if (source is G.IList<int> list2)
		{
			var value = (int)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return list2.IndexesOf(value);
		}
		else
		{
			var list_ = List<int>.ReturnOrConstruct(source);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
		{
			var value = (uint)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return list.IndexesOf(value);
		}
		else if (source is uint[] array)
		{
			var value = (uint)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return array.IndexesOf(value);
		}
		else if (source is G.IList<uint> list2)
		{
			var value = (uint)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return list2.IndexesOf(value);
		}
		else
		{
			var list_ = List<int>.ReturnOrConstruct(source);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
		{
			var value = (long)(list.Sum(x => (MpzT)x) / Max(list.Length, 1));
			return list.IndexesOf(value);
		}
		else if (source is long[] array)
		{
			var value = (long)(array.Sum(x => (MpzT)x) / Max(array.Length, 1));
			return array.IndexesOf(value);
		}
		else if (source is G.IList<long> list2)
		{
			var value = (long)(list2.Sum(x => (MpzT)x) / Max(list2.Count, 1));
			return list2.IndexesOf(value);
		}
		else
		{
			var list_ = List<long>.ReturnOrConstruct(source);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
		{
			var value = list.Sum() / Max(list.Length, 1);
			return list.IndexesOf(value);
		}
		else if (source is MpzT[] array)
		{
			var value = array.Sum() / Max(array.Length, 1);
			return array.IndexesOf(value);
		}
		else if (source is G.IList<MpzT> list2)
		{
			var value = list2.Sum() / Max(list2.Count, 1);
			return list2.IndexesOf(value);
		}
		else
		{
			var list_ = List<MpzT>.ReturnOrConstruct(source);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = List<decimal>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = List<decimal>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
		{
			var list_ = List<double>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = List<double>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = List<double>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = List<double>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
		{
			var list_ = List<int>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = List<int>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = List<int>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = List<int>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
		{
			var list_ = List<uint>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = List<uint>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = List<uint>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = List<uint>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
		{
			var list_ = List<long>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = List<long>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = List<long>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = List<long>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = List<MpzT>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexesOfMinInternal(source, function);
	}

	public static List<int> IndexesOfMin(this G.IEnumerable<decimal> source) => IndexesOfMinInternal(source);

	public static List<int> IndexesOfMin(this G.IEnumerable<double> source) => IndexesOfMinInternal(source);

	public static List<int> IndexesOfMin(this G.IEnumerable<int> source) => IndexesOfMinInternal(source);

	public static List<int> IndexesOfMin(this G.IEnumerable<uint> source) => IndexesOfMinInternal(source);

	public static List<int> IndexesOfMin(this G.IEnumerable<long> source) => IndexesOfMinInternal(source);

	public static List<int> IndexesOfMin(this G.IEnumerable<MpzT> source) => IndexesOfMinInternal(source);

	private static List<int> IndexesOfMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result[j++] = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result.Add(i);
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<int> IndexesOfMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = TResult.Zero;
			var j = 0;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result[j++] = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result.Add(i);
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	private static List<int> IndexesOfMinInternal<T>(G.IEnumerable<T> source) where T : INumber<T>
	{
		if (source is G.IList<T> list)
		{
			var length = list.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = T.Zero;
			var j = 0;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
				{
					indicator = item;
					result[j++] = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<int>(length);
			var indicator = T.Zero;
			var j = 0;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = item;
					result[j++] = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result[j = 0] = i;
					j++;
				}
				else if (f == indicator!)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			List<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = T.Zero;
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = item;
					result.Add(i);
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result.Clear(false);
					result.Add(i);
				}
				else if (f == indicator!)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMaxInternal(source, function);
	}

	public static int IndexOfMax(this G.IEnumerable<decimal> source) => IndexOfMaxInternal(source);

	public static int IndexOfMax(this G.IEnumerable<double> source) => IndexOfMaxInternal(source);

	public static int IndexOfMax(this G.IEnumerable<int> source) => IndexOfMaxInternal(source);

	public static int IndexOfMax(this G.IEnumerable<uint> source) => IndexOfMaxInternal(source);

	public static int IndexOfMax(this G.IEnumerable<long> source) => IndexOfMaxInternal(source);

	public static int IndexOfMax(this G.IEnumerable<MpzT> source) => IndexOfMaxInternal(source);

	private static int IndexOfMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	private static int IndexOfMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	private static int IndexOfMaxInternal<T>(this G.IEnumerable<T> source) where T : INumber<T>
	{
		if (source is G.IList<T> list)
		{
			var length = list.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			var indicator = T.Zero;
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else
		{
			var list_ = source.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is decimal[] array)
		{
			var list_ = List<decimal>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = List<decimal>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
		{
			var list_ = List<double>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is double[] array)
		{
			var list_ = List<double>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = List<double>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = List<double>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
		{
			var value = (int)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return list.IndexOf(value);
		}
		else if (source is int[] array)
		{
			var value = (int)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return array.AsSpan().IndexOf(value);
		}
		else if (source is G.IList<int> list2)
		{
			var value = (int)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return list2.IndexOf(value);
		}
		else
		{
			var list_ = List<int>.ReturnOrConstruct(source);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
		{
			var value = (uint)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return list.IndexOf(value);
		}
		else if (source is uint[] array)
		{
			var value = (uint)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return array.AsSpan().IndexOf(value);
		}
		else if (source is G.IList<uint> list2)
		{
			var value = (uint)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return list2.IndexOf(value);
		}
		else
		{
			var list_ = List<uint>.ReturnOrConstruct(source);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
		{
			var value = (long)(list.Sum(x => (MpzT)x) / Max(list.Length, 1));
			return list.IndexOf(value);
		}
		else if (source is long[] array)
		{
			var value = (long)(array.Sum(x => (MpzT)x) / Max(array.Length, 1));
			return array.AsSpan().IndexOf(value);
		}
		else if (source is G.IList<long> list2)
		{
			var value = (long)(list2.Sum(x => (MpzT)x) / Max(list2.Count, 1));
			return list2.IndexOf(value);
		}
		else
		{
			var list_ = List<long>.ReturnOrConstruct(source);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
		{
			var value = list.Sum() / Max(list.Length, 1);
			return list.IndexOf(value);
		}
		else if (source is MpzT[] array)
		{
			var value = array.Sum() / Max(array.Length, 1);
			return array.AsSpan().IndexOf(value);
		}
		else if (source is G.IList<MpzT> list2)
		{
			var value = list2.Sum() / Max(list2.Count, 1);
			return list2.IndexOf(value);
		}
		else
		{
			var list_ = List<MpzT>.ReturnOrConstruct(source);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = List<decimal>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = List<decimal>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
		{
			var list_ = List<double>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = List<double>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = List<double>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = List<double>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
		{
			var list_ = List<int>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = List<int>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = List<int>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = List<int>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
		{
			var list_ = List<uint>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = List<uint>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = List<uint>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = List<uint>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
		{
			var list_ = List<long>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = List<long>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = List<long>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = List<long>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = List<MpzT>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return IndexOfMinInternal(source, function);
	}

	public static int IndexOfMin(this G.IEnumerable<decimal> source) => IndexOfMinInternal(source);

	public static int IndexOfMin(this G.IEnumerable<double> source) => IndexOfMinInternal(source);

	public static int IndexOfMin(this G.IEnumerable<int> source) => IndexOfMinInternal(source);

	public static int IndexOfMin(this G.IEnumerable<uint> source) => IndexOfMinInternal(source);

	public static int IndexOfMin(this G.IEnumerable<long> source) => IndexOfMinInternal(source);

	public static int IndexOfMin(this G.IEnumerable<MpzT> source) => IndexOfMinInternal(source);

	private static int IndexOfMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	private static int IndexOfMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	private static int IndexOfMinInternal<T>(this G.IEnumerable<T> source) where T : INumber<T>
	{
		if (source is G.IList<T> list)
		{
			var length = list.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
		{
			var result = -1;
			var indicator = T.Zero;
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result = i;
				}
				i++;
			}
			return result;
		}
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMaxInternal(source, function);
	}

	public static int LastIndexOfMax(this G.IEnumerable<decimal> source) => LastIndexOfMaxInternal(source);

	public static int LastIndexOfMax(this G.IEnumerable<double> source) => LastIndexOfMaxInternal(source);

	public static int LastIndexOfMax(this G.IEnumerable<int> source) => LastIndexOfMaxInternal(source);

	public static int LastIndexOfMax(this G.IEnumerable<uint> source) => LastIndexOfMaxInternal(source);

	public static int LastIndexOfMax(this G.IEnumerable<long> source) => LastIndexOfMaxInternal(source);

	public static int LastIndexOfMax(this G.IEnumerable<MpzT> source) => LastIndexOfMaxInternal(source);

	private static int LastIndexOfMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return LastIndexOfMaxInternal(new List<T>(source), function);
	}

	private static int LastIndexOfMaxInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return LastIndexOfMaxInternal(new List<T>(source), function);
	}

	private static int LastIndexOfMaxInternal<T>(this G.IEnumerable<T> source) where T : INumber<T>
	{
		if (source is G.IList<T> list)
		{
			var length = list.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (i == length - 1)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) > indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return LastIndexOfMaxInternal(new List<T>(source));
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is decimal[] array)
		{
			var list_ = List<decimal>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<decimal>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
		{
			var list_ = List<double>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is double[] array)
		{
			var list_ = List<double>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = List<double>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<double>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
		{
			var value = (int)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return list.LastIndexOf(value);
		}
		else if (source is int[] array)
		{
			var value = (int)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return array.AsSpan().LastIndexOf(value);
		}
		else if (source is G.IList<int> list2)
		{
			var value = (int)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return list2.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<int>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
		{
			var value = (uint)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return list.LastIndexOf(value);
		}
		else if (source is uint[] array)
		{
			var value = (uint)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return array.AsSpan().LastIndexOf(value);
		}
		else if (source is G.IList<uint> list2)
		{
			var value = (uint)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return list2.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<uint>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
		{
			var value = (long)(list.Sum(x => (MpzT)x) / Max(list.Length, 1));
			return list.LastIndexOf(value);
		}
		else if (source is long[] array)
		{
			var value = (long)(array.Sum(x => (MpzT)x) / Max(array.Length, 1));
			return array.AsSpan().LastIndexOf(value);
		}
		else if (source is G.IList<long> list2)
		{
			var value = (long)(list2.Sum(x => (MpzT)x) / Max(list2.Count, 1));
			return list2.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<long>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
		{
			var value = list.Sum() / Max(list.Length, 1);
			return list.LastIndexOf(value);
		}
		else if (source is MpzT[] array)
		{
			var value = array.Sum() / Max(array.Length, 1);
			return array.AsSpan().LastIndexOf(value);
		}
		else if (source is G.IList<MpzT> list2)
		{
			var value = list2.Sum() / Max(list2.Count, 1);
			return list2.LastIndexOf(value);
		}
		else
			return LastIndexOfMean(new List<MpzT>(source));
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is G.IList<T> list2)
		{
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = List<decimal>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = List<decimal>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<decimal>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
		{
			var list_ = List<double>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = List<double>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = List<double>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<double>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
		{
			var list_ = List<int>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = List<int>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = List<int>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<int>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
		{
			var list_ = List<uint>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = List<uint>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = List<uint>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<uint>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
		{
			var list_ = List<long>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = List<long>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = List<long>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<long>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = List<MpzT>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<MpzT>(source));
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return LastIndexOfMinInternal(source, function);
	}

	public static int LastIndexOfMin(this G.IEnumerable<decimal> source) => LastIndexOfMinInternal(source);

	public static int LastIndexOfMin(this G.IEnumerable<double> source) => LastIndexOfMinInternal(source);

	public static int LastIndexOfMin(this G.IEnumerable<int> source) => LastIndexOfMinInternal(source);

	public static int LastIndexOfMin(this G.IEnumerable<uint> source) => LastIndexOfMinInternal(source);

	public static int LastIndexOfMin(this G.IEnumerable<long> source) => LastIndexOfMinInternal(source);

	public static int LastIndexOfMin(this G.IEnumerable<MpzT> source) => LastIndexOfMinInternal(source);

	private static int LastIndexOfMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item);
					result = i;
				}
				else if ((f = function(item)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return LastIndexOfMinInternal(new List<T>(source), function);
	}

	private static int LastIndexOfMinInternal<T, TResult>(G.IEnumerable<T> source, Func<T, int, TResult> function)
		where TResult : INumber<TResult>
	{
		if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = -1;
			var indicator = TResult.Zero;
			TResult f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (i == length - 1)
				{
					indicator = function(item, i);
					result = i;
				}
				else if ((f = function(item, i)) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return LastIndexOfMinInternal(new List<T>(source), function);
	}

	private static int LastIndexOfMinInternal<T>(this G.IEnumerable<T> source) where T : INumber<T>
	{
		if (source is G.IList<T> list)
		{
			var length = list.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (i == length - 1)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = T.Zero;
			T f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (i == length - 1)
				{
					indicator = item;
					result = i;
				}
				else if ((f = item) < indicator!)
				{
					indicator = f;
					result = i;
				}
			}
			return result;
		}
		else
			return LastIndexOfMinInternal(new List<T>(source));
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result.Add(item);
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAllMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result.Add(item);
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result.Clear();
				result.Add(item);
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindLastMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindLastMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = item;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static T? FindMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		T? result = default;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = item;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = item;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMaxIndex<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int FindMinIndex<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMax(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexOf(value);
	}

	public static int IndexOfMean(this ReadOnlySpan<decimal> source)
	{
		var value = source.Mean();
		var value2 = value;
		return value == value2 ? source.IndexOf(value2) : -1;
	}

	public static int IndexOfMean(this ReadOnlySpan<double> source)
	{
		var value = source.Mean();
		var value2 = value;
		return value == value2 ? source.IndexOf(value2) : -1;
	}

	public static int IndexOfMean(this ReadOnlySpan<int> source)
	{
		var value = (int)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.IndexOf(value);
	}

	public static int IndexOfMean(this ReadOnlySpan<uint> source)
	{
		var value = (uint)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.IndexOf(value);
	}

	public static int IndexOfMean(this ReadOnlySpan<long> source)
	{
		var value = (long)(source.Sum(x => (MpzT)x) / Max(source.Length, 1));
		return source.IndexOf(value);
	}

	public static int IndexOfMean(this ReadOnlySpan<MpzT> source)
	{
		var value = source.Sum() / (MpzT)source.Length;
		return source.IndexOf(value);
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian(this ReadOnlySpan<decimal> source) => source.IndexOf(source.Median());

	public static int IndexOfMedian(this ReadOnlySpan<double> source) => source.IndexOf(source.Median());

	public static int IndexOfMedian(this ReadOnlySpan<int> source) => source.IndexOf(source.Median());

	public static int IndexOfMedian(this ReadOnlySpan<uint> source) => source.IndexOf(source.Median());

	public static int IndexOfMedian(this ReadOnlySpan<long> source) => source.IndexOf(source.Median());

	public static int IndexOfMedian(this ReadOnlySpan<MpzT> source) => source.IndexOf(source.Median());

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int IndexOfMin(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMax(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean(this ReadOnlySpan<decimal> source)
	{
		var value = source.Mean();
		var value2 = value;
		return value == value2 ? source.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean(this ReadOnlySpan<double> source)
	{
		var value = source.Mean();
		var value2 = value;
		return value == value2 ? source.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean(this ReadOnlySpan<int> source)
	{
		var value = (int)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.LastIndexOf(value);
	}

	public static int LastIndexOfMean(this ReadOnlySpan<uint> source)
	{
		var value = (uint)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.LastIndexOf(value);
	}

	public static int LastIndexOfMean(this ReadOnlySpan<long> source)
	{
		var value = (long)(source.Sum(x => (MpzT)x) / Max(source.Length, 1));
		return source.LastIndexOf(value);
	}

	public static int LastIndexOfMean(this ReadOnlySpan<MpzT> source)
	{
		var value = source.Sum() / (MpzT)source.Length;
		return source.LastIndexOf(value);
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian(this ReadOnlySpan<decimal> source) => source.LastIndexOf(source.Median());

	public static int LastIndexOfMedian(this ReadOnlySpan<double> source) => source.LastIndexOf(source.Median());

	public static int LastIndexOfMedian(this ReadOnlySpan<int> source) => source.LastIndexOf(source.Median());

	public static int LastIndexOfMedian(this ReadOnlySpan<uint> source) => source.LastIndexOf(source.Median());

	public static int LastIndexOfMedian(this ReadOnlySpan<long> source) => source.LastIndexOf(source.Median());

	public static int LastIndexOfMedian(this ReadOnlySpan<MpzT> source) => source.LastIndexOf(source.Median());

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item);
				result = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = function(item, i);
				result = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = -1;
		decimal indicator = 0;
		decimal f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = -1;
		double indicator = 0;
		double f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = -1;
		var indicator = 0;
		int f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = -1;
		uint indicator = 0;
		uint f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = -1;
		long indicator = 0;
		long f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static int LastIndexOfMin(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = -1;
		MpzT indicator = 0;
		MpzT f;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (i == length - 1)
			{
				indicator = item;
				result = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result = i;
			}
		}
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		decimal indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		decimal indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		double indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		double indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		var indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		var indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		uint indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		uint indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		long indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		long indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		MpzT indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		MpzT indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		decimal indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		double indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		var indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		uint indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		long indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMax(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		MpzT indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) > indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexesOf(value);
	}

	public static List<int> IndexesOfMean(this ReadOnlySpan<decimal> source)
	{
		var value = source.Mean();
		return source.IndexesOf(value);
	}

	public static List<int> IndexesOfMean(this ReadOnlySpan<double> source)
	{
		var value = source.Mean();
		return source.IndexesOf(value);
	}

	public static List<int> IndexesOfMean(this ReadOnlySpan<int> source)
	{
		var value = (int)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.IndexesOf(value);
	}

	public static List<int> IndexesOfMean(this ReadOnlySpan<uint> source)
	{
		var value = (uint)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.IndexesOf(value);
	}

	public static List<int> IndexesOfMean(this ReadOnlySpan<long> source)
	{
		var value = (long)(source.Sum(x => (MpzT)x) / Max(source.Length, 1));
		return source.IndexesOf(value);
	}

	public static List<int> IndexesOfMean(this ReadOnlySpan<MpzT> source)
	{
		var value = source.Sum() / (MpzT)source.Length;
		return source.IndexesOf(value);
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var list_ = source.ToList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static List<int> IndexesOfMedian(this ReadOnlySpan<decimal> source) => source.IndexesOf(source.Median());

	public static List<int> IndexesOfMedian(this ReadOnlySpan<double> source) => source.IndexesOf(source.Median());

	public static List<int> IndexesOfMedian(this ReadOnlySpan<int> source) => source.IndexesOf(source.Median());

	public static List<int> IndexesOfMedian(this ReadOnlySpan<uint> source) => source.IndexesOf(source.Median());

	public static List<int> IndexesOfMedian(this ReadOnlySpan<long> source) => source.IndexesOf(source.Median());

	public static List<int> IndexesOfMedian(this ReadOnlySpan<MpzT> source) => source.IndexesOf(source.Median());

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		decimal indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		decimal indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		double indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		double indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		var indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		var indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		uint indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		uint indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		long indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		long indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		MpzT indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item);
				result[j++] = i;
			}
			else if ((f = function(item)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		MpzT indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = function(item, i);
				result[j++] = i;
			}
			else if ((f = function(item, i)) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		decimal indicator = 0;
		var j = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		double indicator = 0;
		var j = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		var indicator = 0;
		var j = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		uint indicator = 0;
		var j = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		long indicator = 0;
		var j = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static List<int> IndexesOfMin(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<int>(length);
		MpzT indicator = 0;
		var j = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
			{
				indicator = item;
				result[j++] = i;
			}
			else if ((f = item) < indicator!)
			{
				indicator = f;
				result[j = 0] = i;
				j++;
			}
			else if (f == indicator!)
				result[j++] = i;
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}
}
