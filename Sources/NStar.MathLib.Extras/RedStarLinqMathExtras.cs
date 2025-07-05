global using Mpir.NET;
global using NStar.Core;
global using NStar.Linq;
global using System;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static NStar.Core.Extents;
global using static System.Math;

namespace NStar.MathLib.Extras;

public static class RedStarLinqMathExtras
{
	internal static readonly Random random = new();

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static List<T> FindAllMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static List<T> FindAllMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMax(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static T? FindLastMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMin(new List<T>(source), function);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMaxIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static int FindLastMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
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
			return FindLastMinIndex([.. source], function, out indicator);
	}

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static T? FindMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static T? FindMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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

	public static int FindMaxIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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
			indicator = 0;
			decimal f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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
			indicator = 0;
			double f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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
			indicator = 0;
			int f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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
			indicator = 0;
			uint f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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
			indicator = 0;
			long f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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

	public static int FindMinIndex<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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
			indicator = 0;
			MpzT f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static int IndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static int IndexOfMax(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
		else if (source is decimal[] array)
		{
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static int IndexOfMax(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
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
		else if (source is double[] array)
		{
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static int IndexOfMax(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
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
		else if (source is int[] array)
		{
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static int IndexOfMax(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
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
		else if (source is uint[] array)
		{
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static int IndexOfMax(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
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
		else if (source is long[] array)
		{
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static int IndexOfMax(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
		else if (source is MpzT[] array)
		{
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.IndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Mean());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Mean());
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Mean());
		}
	}

	public static int IndexOfMean(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
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
			var list_ = NList<int>.ReturnOrConstruct(source);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
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
			var list_ = NList<uint>.ReturnOrConstruct(source);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexOf(value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
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
		if (source is NList<MpzT> list)
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var list_ = NList<int>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = NList<int>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = NList<int>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = NList<uint>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = NList<uint>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var list_ = NList<long>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = NList<long>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = NList<long>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = NList<long>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list);
			return list_.IndexOf(list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(array);
			return list_.IndexOf(list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list2);
			return list_.IndexOf(list_.Median());
		}
		else
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(source);
			return list_.IndexOf(list_.Median());
		}
	}

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static int IndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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

	public static int IndexOfMin(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
		else if (source is decimal[] array)
		{
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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

	public static int IndexOfMin(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
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
		else if (source is double[] array)
		{
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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

	public static int IndexOfMin(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
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
		else if (source is int[] array)
		{
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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

	public static int IndexOfMin(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
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
		else if (source is uint[] array)
		{
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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

	public static int IndexOfMin(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
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
		else if (source is long[] array)
		{
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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

	public static int IndexOfMin(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
		else if (source is MpzT[] array)
		{
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			return LastIndexOfMax(new List<T>(source), function);
	}

	public static int LastIndexOfMax(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
		else if (source is decimal[] array)
		{
			var length = array.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			return LastIndexOfMax(new NList<decimal>(source));
	}

	public static int LastIndexOfMax(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
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
		else if (source is double[] array)
		{
			var length = array.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			return LastIndexOfMax(new NList<double>(source));
	}

	public static int LastIndexOfMax(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
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
		else if (source is int[] array)
		{
			var length = array.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			return LastIndexOfMax(new NList<int>(source));
	}

	public static int LastIndexOfMax(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
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
		else if (source is uint[] array)
		{
			var length = array.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			return LastIndexOfMax(new NList<uint>(source));
	}

	public static int LastIndexOfMax(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
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
		else if (source is long[] array)
		{
			var length = array.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			return LastIndexOfMax(new NList<long>(source));
	}

	public static int LastIndexOfMax(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
		else if (source is MpzT[] array)
		{
			var length = array.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			return LastIndexOfMax(new NList<MpzT>(source));
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return list_.LastIndexOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
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
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new NList<decimal>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Mean());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Mean());
		}
		else
			return LastIndexOfMean(new NList<double>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
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
			return LastIndexOfMean(new NList<int>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
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
			return LastIndexOfMean(new NList<uint>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
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
			return LastIndexOfMean(new NList<long>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
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
			return LastIndexOfMean(new NList<MpzT>(source));
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<decimal>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<double>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var list_ = NList<int>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = NList<int>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = NList<int>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<int>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = NList<uint>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<uint>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var list_ = NList<long>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = NList<long>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = NList<long>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<long>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(array);
			return list_.LastIndexOf(list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list2);
			return list_.LastIndexOf(list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<MpzT>(source));
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			decimal indicator = 0;
			decimal f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			double indicator = 0;
			double f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			var indicator = 0;
			int f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			uint indicator = 0;
			uint f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			long indicator = 0;
			long f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			MpzT indicator = 0;
			MpzT f;
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
			return LastIndexOfMin(new List<T>(source), function);
	}

	public static int LastIndexOfMin(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
		else if (source is decimal[] array)
		{
			var length = array.Length;
			var result = -1;
			decimal indicator = 0;
			decimal f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			var result = -1;
			decimal indicator = 0;
			decimal f;
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
			return LastIndexOfMin(new NList<decimal>(source));
	}

	public static int LastIndexOfMin(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			var result = -1;
			double indicator = 0;
			double f;
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
		else if (source is double[] array)
		{
			var length = array.Length;
			var result = -1;
			double indicator = 0;
			double f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			var result = -1;
			double indicator = 0;
			double f;
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
			return LastIndexOfMin(new NList<double>(source));
	}

	public static int LastIndexOfMin(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			var result = -1;
			var indicator = 0;
			int f;
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
		else if (source is int[] array)
		{
			var length = array.Length;
			var result = -1;
			var indicator = 0;
			int f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			var result = -1;
			var indicator = 0;
			int f;
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
			return LastIndexOfMin(new NList<int>(source));
	}

	public static int LastIndexOfMin(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
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
		else if (source is uint[] array)
		{
			var length = array.Length;
			var result = -1;
			uint indicator = 0;
			uint f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			var result = -1;
			uint indicator = 0;
			uint f;
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
			return LastIndexOfMin(new NList<uint>(source));
	}

	public static int LastIndexOfMin(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			var result = -1;
			long indicator = 0;
			long f;
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
		else if (source is long[] array)
		{
			var length = array.Length;
			var result = -1;
			long indicator = 0;
			long f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			var result = -1;
			long indicator = 0;
			long f;
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
			return LastIndexOfMin(new NList<long>(source));
	}

	public static int LastIndexOfMin(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
		else if (source is MpzT[] array)
		{
			var length = array.Length;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
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
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			var result = -1;
			MpzT indicator = 0;
			MpzT f;
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
			return LastIndexOfMin(new NList<MpzT>(source));
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
			}
			else if (f == indicator!)
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static int FindIndex<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			if (function(source[i]))
				return i;
		return -1;
	}

	public static int FindIndex<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			if (function(source[i], i))
				return i;
		return -1;
	}

	public static int FindLastIndex<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = length - 1; i >= 0; i--)
			if (function(source[i]))
				return i;
		return -1;
	}

	public static int FindLastIndex<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = length - 1; i >= 0; i--)
			if (function(source[i], i))
				return i;
		return -1;
	}

	public static T? FindLast<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (function(item))
				return item;
		}
		return default;
	}

	public static T? FindLast<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = length - 1; i >= 0; i--)
		{
			var item = source[i];
			if (function(item, i))
				return item;
		}
		return default;
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
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.IndexOf(value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexOf(value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
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
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.IndexOf(list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
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
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? list_.LastIndexOf(value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.LastIndexOf(value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
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
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return list_.LastIndexOf(list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			decimal f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			decimal f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			double f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			double f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			int f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			int f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			uint f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			uint f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			long f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			long f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			MpzT f;
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

	public static NList<int> FindMaxIndexes<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			MpzT f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			decimal f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function, out decimal indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			decimal f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			double f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, double> function, out double indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			double f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			int f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, int> function, out int indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			int f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			uint f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, uint> function, out uint indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			uint f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			long f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, long> function, out long indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			long f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			MpzT f;
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

	public static NList<int> FindMinIndexes<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function, out MpzT indicator)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			indicator = 0;
			MpzT f;
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

	public static NList<int> IndexesOfMax(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
		else if (source is decimal[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			decimal indicator = 0;
			decimal f;
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

	public static NList<int> IndexesOfMax(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
		else if (source is double[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			double indicator = 0;
			double f;
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

	public static NList<int> IndexesOfMax(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
		else if (source is int[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = 0;
			int f;
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

	public static NList<int> IndexesOfMax(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
		else if (source is uint[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			uint indicator = 0;
			uint f;
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

	public static NList<int> IndexesOfMax(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
		else if (source is long[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			long indicator = 0;
			long f;
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

	public static NList<int> IndexesOfMax(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
		else if (source is MpzT[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			MpzT indicator = 0;
			MpzT f;
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

	public static NList<int> IndexesOfMean(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
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
			var list_ = NList<int>.ReturnOrConstruct(source);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
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
			var list_ = NList<int>.ReturnOrConstruct(source);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
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
			var list_ = NList<long>.ReturnOrConstruct(source);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
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
			var list_ = NList<MpzT>.ReturnOrConstruct(source);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMin(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
		else if (source is decimal[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			decimal indicator = 0;
			decimal f;
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

	public static NList<int> IndexesOfMin(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
		else if (source is double[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			double indicator = 0;
			double f;
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

	public static NList<int> IndexesOfMin(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
		else if (source is int[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = 0;
			int f;
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

	public static NList<int> IndexesOfMin(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
		else if (source is uint[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			uint indicator = 0;
			uint f;
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

	public static NList<int> IndexesOfMin(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
		else if (source is long[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			long indicator = 0;
			long f;
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

	public static NList<int> IndexesOfMin(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
		else if (source is MpzT[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			MpzT indicator = 0;
			MpzT f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			decimal indicator = 0;
			decimal f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			decimal indicator = 0;
			decimal f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			double indicator = 0;
			double f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			double indicator = 0;
			double f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = 0;
			int f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = 0;
			int f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			uint indicator = 0;
			uint f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			uint indicator = 0;
			uint f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			long indicator = 0;
			long f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			long indicator = 0;
			long f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			MpzT indicator = 0;
			MpzT f;
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

	public static NList<int> IndexesOfMax<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			MpzT indicator = 0;
			MpzT f;
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

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = list_.Mean();
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = list2.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = list3.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
		else
		{
			var list_ = source.ToNList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return list_.IndexesOf(value);
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = source.ToNList(function);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var list_ = NList<int>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = NList<int>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = NList<int>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = NList<uint>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = NList<uint>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var list_ = NList<long>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = NList<long>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = NList<long>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = NList<long>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(array);
			return list_.IndexesOf(list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list2);
			return list_.IndexesOf(list_.Median());
		}
		else
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(source);
			return list_.IndexesOf(list_.Median());
		}
	}

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			decimal indicator = 0;
			decimal f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			decimal indicator = 0;
			var j = 0;
			decimal f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			decimal indicator = 0;
			decimal f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			double indicator = 0;
			double f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			double indicator = 0;
			var j = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			double indicator = 0;
			var j = 0;
			double f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			double indicator = 0;
			double f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = 0;
			int f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var indicator = 0;
			var j = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			var indicator = 0;
			var j = 0;
			int f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var indicator = 0;
			int f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			uint indicator = 0;
			uint f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			uint indicator = 0;
			var j = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			uint indicator = 0;
			var j = 0;
			uint f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			uint indicator = 0;
			uint f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			long indicator = 0;
			long f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			long indicator = 0;
			var j = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			long indicator = 0;
			var j = 0;
			long f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			long indicator = 0;
			long f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			MpzT indicator = 0;
			MpzT f;
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

	public static NList<int> IndexesOfMin<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
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
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
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
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			var result = RedStarLinq.NEmptyList<int>(length);
			MpzT indicator = 0;
			var j = 0;
			MpzT f;
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
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			MpzT indicator = 0;
			MpzT f;
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMaxIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> FindMinIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMax(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = list_.Mean();
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return list_.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<decimal> source)
	{
		var value = source.Mean();
		return source.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<double> source)
	{
		var value = source.Mean();
		return source.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<int> source)
	{
		var value = (int)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<uint> source)
	{
		var value = (uint)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return source.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<long> source)
	{
		var value = (long)(source.Sum(x => (MpzT)x) / Max(source.Length, 1));
		return source.IndexesOf(value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<MpzT> source)
	{
		var value = source.Sum() / (MpzT)source.Length;
		return source.IndexesOf(value);
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToNList(function);
		return list_.IndexesOf(list_.Median());
	}

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<decimal> source) => source.IndexesOf(source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<double> source) => source.IndexesOf(source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<int> source) => source.IndexesOf(source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<uint> source) => source.IndexesOf(source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<long> source) => source.IndexesOf(source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<MpzT> source) => source.IndexesOf(source.Median());

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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

	public static NList<int> IndexesOfMin(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
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
