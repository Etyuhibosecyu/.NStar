global using Mpir.NET;
global using NStar.Core;
global using System;
global using System.Collections;
global using System.Diagnostics;
global using System.Runtime.InteropServices;
global using G = System.Collections.Generic;
global using static NStar.Core.Extents;
global using static System.Math;
global using E = System.Linq.Enumerable;
global using String = NStar.Core.String;
using System.Numerics;
using System.Threading.Tasks;

namespace NStar.MathLib;

public static class RedStarLinqMath
{
	public static decimal Max<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Max(function(list[0]), function(list[1])),
					3 => Math.Max(Math.Max(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Max(function(array[0]), function(array[1])),
					3 => Math.Max(Math.Max(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Max(function(list2[0]), function(list2[1])),
					3 => Math.Max(Math.Max(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Max(function(list3[0]), function(list3[1])),
					3 => Math.Max(Math.Max(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			decimal indicator = 0;
			decimal f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static decimal Max<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Max(function(list[0], 0), function(list[1], 1)),
					3 => Math.Max(Math.Max(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Max(function(array[0], 0), function(array[1], 1)),
					3 => Math.Max(Math.Max(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Max(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Max(Math.Max(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Max(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Max(Math.Max(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			decimal indicator = 0;
			decimal f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static double Max<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Max(function(list[0]), function(list[1])),
					3 => Math.Max(Math.Max(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Max(function(array[0]), function(array[1])),
					3 => Math.Max(Math.Max(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Max(function(list2[0]), function(list2[1])),
					3 => Math.Max(Math.Max(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Max(function(list3[0]), function(list3[1])),
					3 => Math.Max(Math.Max(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			double indicator = 0;
			double f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static double Max<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Max(function(list[0], 0), function(list[1], 1)),
					3 => Math.Max(Math.Max(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Max(function(array[0], 0), function(array[1], 1)),
					3 => Math.Max(Math.Max(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Max(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Max(Math.Max(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Max(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Max(Math.Max(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			double indicator = 0;
			double f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static int Max<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Max(function(list[0]), function(list[1])),
					3 => Math.Max(Math.Max(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Max(function(array[0]), function(array[1])),
					3 => Math.Max(Math.Max(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Max(function(list2[0]), function(list2[1])),
					3 => Math.Max(Math.Max(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Max(function(list3[0]), function(list3[1])),
					3 => Math.Max(Math.Max(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			var indicator = 0;
			int f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static int Max<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Max(function(list[0], 0), function(list[1], 1)),
					3 => Math.Max(Math.Max(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			var indicator = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Max(function(array[0], 0), function(array[1], 1)),
					3 => Math.Max(Math.Max(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Max(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Max(Math.Max(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Max(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Max(Math.Max(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			var indicator = 0;
			int f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static uint Max<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Max(function(list[0]), function(list[1])),
					3 => Math.Max(Math.Max(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Max(function(array[0]), function(array[1])),
					3 => Math.Max(Math.Max(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Max(function(list2[0]), function(list2[1])),
					3 => Math.Max(Math.Max(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Max(function(list3[0]), function(list3[1])),
					3 => Math.Max(Math.Max(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			uint indicator = 0;
			uint f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static uint Max<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Max(function(list[0], 0), function(list[1], 1)),
					3 => Math.Max(Math.Max(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Max(function(array[0], 0), function(array[1], 1)),
					3 => Math.Max(Math.Max(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Max(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Max(Math.Max(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Max(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Max(Math.Max(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			uint indicator = 0;
			uint f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static long Max<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Max(function(list[0]), function(list[1])),
					3 => Math.Max(Math.Max(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Max(function(array[0]), function(array[1])),
					3 => Math.Max(Math.Max(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Max(function(list2[0]), function(list2[1])),
					3 => Math.Max(Math.Max(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Max(function(list3[0]), function(list3[1])),
					3 => Math.Max(Math.Max(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			long indicator = 0;
			long f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static long Max<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Max(function(list[0], 0), function(list[1], 1)),
					3 => Math.Max(Math.Max(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Max(function(array[0], 0), function(array[1], 1)),
					3 => Math.Max(Math.Max(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Max(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Max(Math.Max(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Max(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Max(Math.Max(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			long indicator = 0;
			long f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static MpzT Max<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => MpzT.Max(function(list[0]), function(list[1])),
					3 => MpzT.Max(MpzT.Max(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => MpzT.Max(function(array[0]), function(array[1])),
					3 => MpzT.Max(MpzT.Max(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => MpzT.Max(function(list2[0]), function(list2[1])),
					3 => MpzT.Max(MpzT.Max(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => MpzT.Max(function(list3[0]), function(list3[1])),
					3 => MpzT.Max(MpzT.Max(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			MpzT indicator = 0;
			MpzT f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static MpzT Max<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => MpzT.Max(function(list[0], 0), function(list[1], 1)),
					3 => MpzT.Max(MpzT.Max(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => MpzT.Max(function(array[0], 0), function(array[1], 1)),
					3 => MpzT.Max(MpzT.Max(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => MpzT.Max(function(list2[0], 0), function(list2[1], 1)),
					3 => MpzT.Max(MpzT.Max(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => MpzT.Max(function(list3[0], 0), function(list3[1], 1)),
					3 => MpzT.Max(MpzT.Max(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			MpzT indicator = 0;
			MpzT f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static decimal Max(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Max(list[0], list[1]),
					3 => Math.Max(Math.Max(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is decimal[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Max(array[0], array[1]),
					3 => Math.Max(Math.Max(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Max(list2[0], list2[1]),
					3 => Math.Max(Math.Max(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			decimal indicator = 0;
			decimal f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static double Max(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Max(list[0], list[1]),
					3 => Math.Max(Math.Max(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is double[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Max(array[0], array[1]),
					3 => Math.Max(Math.Max(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Max(list2[0], list2[1]),
					3 => Math.Max(Math.Max(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			double indicator = 0;
			double f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static int Max(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Max(list[0], list[1]),
					3 => Math.Max(Math.Max(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is int[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Max(array[0], array[1]),
					3 => Math.Max(Math.Max(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Max(list2[0], list2[1]),
					3 => Math.Max(Math.Max(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			var indicator = 0;
			int f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static uint Max(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Max(list[0], list[1]),
					3 => Math.Max(Math.Max(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is uint[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Max(array[0], array[1]),
					3 => Math.Max(Math.Max(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Max(list2[0], list2[1]),
					3 => Math.Max(Math.Max(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			uint indicator = 0;
			uint f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static long Max(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Max(list[0], list[1]),
					3 => Math.Max(Math.Max(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is long[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Max(array[0], array[1]),
					3 => Math.Max(Math.Max(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Max(list2[0], list2[1]),
					3 => Math.Max(Math.Max(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			long indicator = 0;
			long f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static MpzT Max(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => MpzT.Max(list[0], list[1]),
					3 => MpzT.Max(MpzT.Max(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is MpzT[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => MpzT.Max(array[0], array[1]),
					3 => MpzT.Max(MpzT.Max(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => MpzT.Max(list2[0], list2[1]),
					3 => MpzT.Max(MpzT.Max(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			MpzT indicator = 0;
			MpzT f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) > indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static decimal Mean<T>(this G.IEnumerable<T> source, Func<T, decimal> function) => MeanInternal<T, decimal, decimal, decimal>(source, function);

	public static decimal Mean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) => MeanInternal<T, decimal, decimal, decimal>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, double> function) => MeanInternal<T, double, double, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, int, double> function) => MeanInternal<T, double, double, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, int> function) => MeanInternal<T, int, long, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, int, int> function) => MeanInternal<T, int, long, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, uint> function) => MeanInternal<T, uint, ulong, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) => MeanInternal<T, uint, ulong, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, long> function) => MeanInternal<T, long, MpzT, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, int, long> function) => MeanInternal<T, long, MpzT, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) => MeanInternal<T, MpzT, MpzT, double>(source, function);

	public static double Mean<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) => MeanInternal<T, MpzT, MpzT, double>(source, function);

	public static decimal Mean(this G.IEnumerable<decimal> source) => MeanInternal<decimal, decimal, decimal>(source);

	public static double Mean(this G.IEnumerable<double> source) => MeanInternal<double, double, double>(source);

	public static double Mean(this G.IEnumerable<int> source) => MeanInternal<int, long, double>(source);

	public static double Mean(this G.IEnumerable<uint> source) => MeanInternal<uint, ulong, double>(source);

	public static double Mean(this G.IEnumerable<long> source) => MeanInternal<long, MpzT, double>(source);

	public static double Mean(this G.IEnumerable<MpzT> source) => MeanInternal<MpzT, MpzT, double>(source);

	private static TResult MeanInternal<TSource, TFunction, TAccumulator, TResult>(G.IEnumerable<TSource> source, Func<TSource, TFunction> function) where TFunction : struct, INumber<TFunction> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(function);
		using var e = source.GetEnumerator();
		if (!e.MoveNext())
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(function(e.Current));
		var count = 1;
		while (e.MoveNext())
		{
			checked
			{
				sum += TAccumulator.CreateChecked(function(e.Current));
				count++;
			}
		}
		return TResult.CreateChecked(sum) / TResult.CreateChecked(count);
	}

	private static TResult MeanInternal<TSource, TFunction, TAccumulator, TResult>(G.IEnumerable<TSource> source, Func<TSource, int, TFunction> function) where TFunction : struct, INumber<TFunction> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(function);
		using var e = source.GetEnumerator();
		if (!e.MoveNext())
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(function(e.Current, 0));
		var count = 1;
		while (e.MoveNext())
		{
			checked
			{
				sum += TAccumulator.CreateChecked(function(e.Current, count));
				count++;
			}
		}
		return TResult.CreateChecked(sum) / TResult.CreateChecked(count);
	}

	private static TResult MeanInternal<TSource, TAccumulator, TResult>(G.IEnumerable<TSource> source) where TSource : struct, INumber<TSource> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(source);
		using var e = source.GetEnumerator();
		if (!e.MoveNext())
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(e.Current);
		var count = 1;
		while (e.MoveNext())
		{
			checked
			{
				sum += TAccumulator.CreateChecked(e.Current);
				count++;
			}
		}
		return TResult.CreateChecked(sum) / TResult.CreateChecked(count);
	}

	public static decimal Median<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static decimal Median<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static double Median<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static double Median<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static int Median<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static int Median<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static uint Median<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static uint Median<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static long Median<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static long Median<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static MpzT Median<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static MpzT Median<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? 0 : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? 0 : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? 0 : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? 0 : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else
			return CreateVar(source.ToList(function), out var col).Any() ? col.Sort()[(col.Length - 1) / 2] : 0;
	}

	public static decimal Median(this G.IEnumerable<decimal> source)
	{
		if (source is List<decimal> list)
			return list.Length == 0 ? 0 : new List<decimal>(list).Sort()[(list.Length - 1) / 2];
		else if (source is decimal[] array)
			return array.Length == 0 ? 0 : new List<decimal>(array).Sort()[(array.Length - 1) / 2];
		else if (source is G.IList<decimal> list2)
			return list2.Count == 0 ? 0 : new List<decimal>(list2).Sort()[(list2.Count - 1) / 2];
		else
			return CreateVar(new List<decimal>(source), out var col).Length == 0 ? 0 : col.Sort()[(col.Length - 1) / 2];
	}

	public static double Median(this G.IEnumerable<double> source)
	{
		if (source is List<double> list)
			return list.Length == 0 ? 0 : new List<double>(list).Sort()[(list.Length - 1) / 2];
		else if (source is double[] array)
			return (double)(array.Length == 0 ? 0 : new List<double>(array).Sort()[(array.Length - 1) / 2]);
		else if (source is G.IList<double> list2)
			return (double)(double)(list2.Count == 0 ? 0 : new List<double>(list2).Sort()[(list2.Count - 1) / 2]);
		else
			return (double)(double)(CreateVar(new List<double>(source), out var col).Length == 0 ? 0 : col.Sort()[(col.Length - 1) / 2]);
	}

	public static int Median(this G.IEnumerable<int> source)
	{
		if (source is List<int> list)
			return list.Length == 0 ? 0 : new List<int>(list).Sort()[(list.Length - 1) / 2];
		else if (source is int[] array)
			return array.Length == 0 ? 0 : new List<int>(array).Sort()[(array.Length - 1) / 2];
		else if (source is G.IList<int> list2)
			return list2.Count == 0 ? 0 : new List<int>(list2).Sort()[(list2.Count - 1) / 2];
		else
			return CreateVar(new List<int>(source), out var col).Length == 0 ? 0 : col.Sort()[(col.Length - 1) / 2];
	}

	public static uint Median(this G.IEnumerable<uint> source)
	{
		if (source is List<uint> list)
			return list.Length == 0 ? 0 : new List<uint>(list).Sort()[(list.Length - 1) / 2];
		else if (source is uint[] array)
			return array.Length == 0 ? 0 : new List<uint>(array).Sort()[(array.Length - 1) / 2];
		else if (source is G.IList<uint> list2)
			return list2.Count == 0 ? 0 : new List<uint>(list2).Sort()[(list2.Count - 1) / 2];
		else
			return CreateVar(new List<uint>(source), out var col).Length == 0 ? 0 : col.Sort()[(col.Length - 1) / 2];
	}

	public static long Median(this G.IEnumerable<long> source)
	{
		if (source is List<long> list)
			return list.Length == 0 ? 0 : new List<long>(list).Sort()[(list.Length - 1) / 2];
		else if (source is long[] array)
			return array.Length == 0 ? 0 : new List<long>(array).Sort()[(array.Length - 1) / 2];
		else if (source is G.IList<long> list2)
			return list2.Count == 0 ? 0 : new List<long>(list2).Sort()[(list2.Count - 1) / 2];
		else
			return CreateVar(new List<long>(source), out var col).Length == 0 ? 0 : col.Sort()[(col.Length - 1) / 2];
	}

	public static MpzT Median(this G.IEnumerable<MpzT> source)
	{
		if (source is List<MpzT> list)
			return list.Length == 0 ? 0 : new List<MpzT>(list).Sort()[(list.Length - 1) / 2];
		else if (source is MpzT[] array)
			return array.Length == 0 ? 0 : new List<MpzT>(array).Sort()[(array.Length - 1) / 2];
		else if (source is G.IList<MpzT> list2)
			return list2.Count == 0 ? 0 : new List<MpzT>(list2).Sort()[(list2.Count - 1) / 2];
		else
			return CreateVar(new List<MpzT>(source), out var col).Length == 0 ? 0 : col.Sort()[(col.Length - 1) / 2];
	}

	public static TResult? Median<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? default : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? default : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? default : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? default : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else if (source.TryGetLengthEasily(out var length))
			return length == 0 ? default : source.ToList(function).Sort()[(length - 1) / 2];
		else
			return Median([.. source], function);
	}

	public static TResult? Median<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			return list.Length == 0 ? default : list.ToList(function).Sort()[(list.Length - 1) / 2];
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			return array.Length == 0 ? default : array.AsSpan().ToList(function).Sort()[(array.Length - 1) / 2];
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			return list2.Count == 0 ? default : list2.ToList(function).Sort()[(list2.Count - 1) / 2];
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			return list3.Count == 0 ? default : list3.ToList(function).Sort()[(list3.Count - 1) / 2];
		}
		else if (source.TryGetLengthEasily(out var length))
			return length == 0 ? default : source.ToList(function).Sort()[(length - 1) / 2];
		else
			return Median([.. source], function);
	}

	public static T? Median<T>(this G.IEnumerable<T> source)
	{
		if (source is List<T> list)
			return list.Length == 0 ? default : new List<T>(list).Sort()[(list.Length - 1) / 2];
		else if (source is T[] array)
			return array.Length == 0 ? default : new List<T>(array).Sort()[(array.Length - 1) / 2];
		else if (source is G.IList<T> list2)
			return list2.Count == 0 ? default : new List<T>(list2).Sort()[(list2.Count - 1) / 2];
		else if (source.TryGetLengthEasily(out var length))
			return length == 0 ? default : new List<T>(source).Sort()[(length - 1) / 2];
		else
			return Median(new List<T>(source));
	}

	public static decimal Min<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Min(function(list[0]), function(list[1])),
					3 => Math.Min(Math.Min(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Min(function(array[0]), function(array[1])),
					3 => Math.Min(Math.Min(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Min(function(list2[0]), function(list2[1])),
					3 => Math.Min(Math.Min(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Min(function(list3[0]), function(list3[1])),
					3 => Math.Min(Math.Min(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			decimal indicator = 0;
			decimal f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static decimal Min<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Min(function(list[0], 0), function(list[1], 1)),
					3 => Math.Min(Math.Min(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Min(function(array[0], 0), function(array[1], 1)),
					3 => Math.Min(Math.Min(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Min(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Min(Math.Min(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Min(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Min(Math.Min(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			decimal indicator = 0;
			decimal f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static double Min<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Min(function(list[0]), function(list[1])),
					3 => Math.Min(Math.Min(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Min(function(array[0]), function(array[1])),
					3 => Math.Min(Math.Min(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Min(function(list2[0]), function(list2[1])),
					3 => Math.Min(Math.Min(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Min(function(list3[0]), function(list3[1])),
					3 => Math.Min(Math.Min(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			double indicator = 0;
			double f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static double Min<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Min(function(list[0], 0), function(list[1], 1)),
					3 => Math.Min(Math.Min(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Min(function(array[0], 0), function(array[1], 1)),
					3 => Math.Min(Math.Min(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Min(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Min(Math.Min(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Min(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Min(Math.Min(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			double indicator = 0;
			double f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static int Min<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Min(function(list[0]), function(list[1])),
					3 => Math.Min(Math.Min(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Min(function(array[0]), function(array[1])),
					3 => Math.Min(Math.Min(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Min(function(list2[0]), function(list2[1])),
					3 => Math.Min(Math.Min(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Min(function(list3[0]), function(list3[1])),
					3 => Math.Min(Math.Min(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			var indicator = 0;
			int f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static int Min<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Min(function(list[0], 0), function(list[1], 1)),
					3 => Math.Min(Math.Min(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Min(function(array[0], 0), function(array[1], 1)),
					3 => Math.Min(Math.Min(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Min(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Min(Math.Min(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Min(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Min(Math.Min(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			var indicator = 0;
			int f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static uint Min<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Min(function(list[0]), function(list[1])),
					3 => Math.Min(Math.Min(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Min(function(array[0]), function(array[1])),
					3 => Math.Min(Math.Min(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Min(function(list2[0]), function(list2[1])),
					3 => Math.Min(Math.Min(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Min(function(list3[0]), function(list3[1])),
					3 => Math.Min(Math.Min(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			uint indicator = 0;
			uint f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static uint Min<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Min(function(list[0], 0), function(list[1], 1)),
					3 => Math.Min(Math.Min(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Min(function(array[0], 0), function(array[1], 1)),
					3 => Math.Min(Math.Min(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Min(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Min(Math.Min(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Min(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Min(Math.Min(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			uint indicator = 0;
			uint f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static long Min<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => Math.Min(function(list[0]), function(list[1])),
					3 => Math.Min(Math.Min(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => Math.Min(function(array[0]), function(array[1])),
					3 => Math.Min(Math.Min(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => Math.Min(function(list2[0]), function(list2[1])),
					3 => Math.Min(Math.Min(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => Math.Min(function(list3[0]), function(list3[1])),
					3 => Math.Min(Math.Min(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			long indicator = 0;
			long f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static long Min<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => Math.Min(function(list[0], 0), function(list[1], 1)),
					3 => Math.Min(Math.Min(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => Math.Min(function(array[0], 0), function(array[1], 1)),
					3 => Math.Min(Math.Min(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => Math.Min(function(list2[0], 0), function(list2[1], 1)),
					3 => Math.Min(Math.Min(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => Math.Min(function(list3[0], 0), function(list3[1], 1)),
					3 => Math.Min(Math.Min(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			long indicator = 0;
			long f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static MpzT Min<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0]),
					2 => MpzT.Min(function(list[0]), function(list[1])),
					3 => MpzT.Min(MpzT.Min(function(list[0]), function(list[1])), function(list[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0]),
					2 => MpzT.Min(function(array[0]), function(array[1])),
					3 => MpzT.Min(MpzT.Min(function(array[0]), function(array[1])), function(array[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0]),
					2 => MpzT.Min(function(list2[0]), function(list2[1])),
					3 => MpzT.Min(MpzT.Min(function(list2[0]), function(list2[1])), function(list2[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0]),
					2 => MpzT.Min(function(list3[0]), function(list3[1])),
					3 => MpzT.Min(MpzT.Min(function(list3[0]), function(list3[1])), function(list3[2])),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			MpzT indicator = 0;
			MpzT f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item);
				else if ((f = function(item)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static MpzT Min<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list[0], 0),
					2 => MpzT.Min(function(list[0], 0), function(list[1], 1)),
					3 => MpzT.Min(MpzT.Min(function(list[0], 0), function(list[1], 1)), function(list[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => function(array[0], 0),
					2 => MpzT.Min(function(array[0], 0), function(array[1], 1)),
					3 => MpzT.Min(MpzT.Min(function(array[0], 0), function(array[1], 1)), function(array[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list2[0], 0),
					2 => MpzT.Min(function(list2[0], 0), function(list2[1], 1)),
					3 => MpzT.Min(MpzT.Min(function(list2[0], 0), function(list2[1], 1)), function(list2[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => function(list3[0], 0),
					2 => MpzT.Min(function(list3[0], 0), function(list3[1], 1)),
					3 => MpzT.Min(MpzT.Min(function(list3[0], 0), function(list3[1], 1)), function(list3[2], 2)),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			MpzT indicator = 0;
			MpzT f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = function(item, i);
				else if ((f = function(item, i)) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static decimal Min(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Min(list[0], list[1]),
					3 => Math.Min(Math.Min(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is decimal[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Min(array[0], array[1]),
					3 => Math.Min(Math.Min(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Min(list2[0], list2[1]),
					3 => Math.Min(Math.Min(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			decimal indicator = 0;
			decimal f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			decimal indicator = 0;
			decimal f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static double Min(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Min(list[0], list[1]),
					3 => Math.Min(Math.Min(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is double[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Min(array[0], array[1]),
					3 => Math.Min(Math.Min(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Min(list2[0], list2[1]),
					3 => Math.Min(Math.Min(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			double indicator = 0;
			double f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			double indicator = 0;
			double f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static int Min(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Min(list[0], list[1]),
					3 => Math.Min(Math.Min(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is int[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Min(array[0], array[1]),
					3 => Math.Min(Math.Min(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Min(list2[0], list2[1]),
					3 => Math.Min(Math.Min(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			var indicator = 0;
			int f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			var indicator = 0;
			int f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static uint Min(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Min(list[0], list[1]),
					3 => Math.Min(Math.Min(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is uint[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Min(array[0], array[1]),
					3 => Math.Min(Math.Min(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Min(list2[0], list2[1]),
					3 => Math.Min(Math.Min(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			uint indicator = 0;
			uint f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			uint indicator = 0;
			uint f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static long Min(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => Math.Min(list[0], list[1]),
					3 => Math.Min(Math.Min(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is long[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => Math.Min(array[0], array[1]),
					3 => Math.Min(Math.Min(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => Math.Min(list2[0], list2[1]),
					3 => Math.Min(Math.Min(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			long indicator = 0;
			long f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			long indicator = 0;
			long f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static MpzT Min(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => MpzT.Min(list[0], list[1]),
					3 => MpzT.Min(MpzT.Min(list[0], list[1]), list[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is MpzT[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => MpzT.Min(array[0], array[1]),
					3 => MpzT.Min(MpzT.Min(array[0], array[1]), array[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => MpzT.Min(list2[0], list2[1]),
					3 => MpzT.Min(MpzT.Min(list2[0], list2[1]), list2[2]),
					_ => throw new ApplicationException("Произошла серьезная ошибка при попытке выполнить действие. К сожалению, причина ошибки неизвестна.")
				};
			}
			MpzT indicator = 0;
			MpzT f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
			}
			return indicator;
		}
		else
		{
			MpzT indicator = 0;
			MpzT f;
			var i = 0;
			foreach (var item in source)
			{
				if (i == 0)
					indicator = item;
				else if ((f = item) < indicator!)
					indicator = f;
				i++;
			}
			return indicator;
		}
	}

	public static decimal Product<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0]),
					2 => function(list[0]) * function(list[1]),
					3 => function(list[0]) * function(list[1]) * function(list[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0]),
					2 => function(array[0]) * function(array[1]),
					3 => function(array[0]) * function(array[1]) * function(array[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < array.Length; i++)
				result *= function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0]),
					2 => function(list2[0]) * function(list2[1]),
					3 => function(list2[0]) * function(list2[1]) * function(list2[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0]),
					2 => function(list3[0]) * function(list3[1]),
					3 => function(list3[0]) * function(list3[1]) * function(list3[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list3[i]);
			return result;
		}
		else
		{
			decimal result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item);
				i++;
			}
			return result;
		}
	}

	public static decimal Product<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0], 0),
					2 => function(list[0], 0) * function(list[1], 1),
					3 => function(list[0], 0) * function(list[1], 1) * function(list[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0], 0),
					2 => function(array[0], 0) * function(array[1], 1),
					3 => function(array[0], 0) * function(array[1], 1) * function(array[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0], 0),
					2 => function(list2[0], 0) * function(list2[1], 1),
					3 => function(list2[0], 0) * function(list2[1], 1) * function(list2[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result *= function(item, i);
			}
			return result;
		}
		else
		{
			decimal result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item, i);
				i++;
			}
			return result;
		}
	}

	public static double Product<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0]),
					2 => function(list[0]) * function(list[1]),
					3 => function(list[0]) * function(list[1]) * function(list[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0]),
					2 => function(array[0]) * function(array[1]),
					3 => function(array[0]) * function(array[1]) * function(array[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < array.Length; i++)
				result *= function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0]),
					2 => function(list2[0]) * function(list2[1]),
					3 => function(list2[0]) * function(list2[1]) * function(list2[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0]),
					2 => function(list3[0]) * function(list3[1]),
					3 => function(list3[0]) * function(list3[1]) * function(list3[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list3[i]);
			return result;
		}
		else
		{
			double result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item);
				i++;
			}
			return result;
		}
	}

	public static double Product<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0], 0),
					2 => function(list[0], 0) * function(list[1], 1),
					3 => function(list[0], 0) * function(list[1], 1) * function(list[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0], 0),
					2 => function(array[0], 0) * function(array[1], 1),
					3 => function(array[0], 0) * function(array[1], 1) * function(array[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0], 0),
					2 => function(list2[0], 0) * function(list2[1], 1),
					3 => function(list2[0], 0) * function(list2[1], 1) * function(list2[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result *= function(item, i);
			}
			return result;
		}
		else
		{
			double result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item, i);
				i++;
			}
			return result;
		}
	}

	public static int Product<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0]),
					2 => function(list[0]) * function(list[1]),
					3 => function(list[0]) * function(list[1]) * function(list[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0]),
					2 => function(array[0]) * function(array[1]),
					3 => function(array[0]) * function(array[1]) * function(array[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < array.Length; i++)
				result *= function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0]),
					2 => function(list2[0]) * function(list2[1]),
					3 => function(list2[0]) * function(list2[1]) * function(list2[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0]),
					2 => function(list3[0]) * function(list3[1]),
					3 => function(list3[0]) * function(list3[1]) * function(list3[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list3[i]);
			return result;
		}
		else
		{
			var result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item);
				i++;
			}
			return result;
		}
	}

	public static int Product<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0], 0),
					2 => function(list[0], 0) * function(list[1], 1),
					3 => function(list[0], 0) * function(list[1], 1) * function(list[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0], 0),
					2 => function(array[0], 0) * function(array[1], 1),
					3 => function(array[0], 0) * function(array[1], 1) * function(array[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0], 0),
					2 => function(list2[0], 0) * function(list2[1], 1),
					3 => function(list2[0], 0) * function(list2[1], 1) * function(list2[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result *= function(item, i);
			}
			return result;
		}
		else
		{
			var result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item, i);
				i++;
			}
			return result;
		}
	}

	public static uint Product<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0]),
					2 => function(list[0]) * function(list[1]),
					3 => function(list[0]) * function(list[1]) * function(list[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0]),
					2 => function(array[0]) * function(array[1]),
					3 => function(array[0]) * function(array[1]) * function(array[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < array.Length; i++)
				result *= function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0]),
					2 => function(list2[0]) * function(list2[1]),
					3 => function(list2[0]) * function(list2[1]) * function(list2[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0]),
					2 => function(list3[0]) * function(list3[1]),
					3 => function(list3[0]) * function(list3[1]) * function(list3[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list3[i]);
			return result;
		}
		else
		{
			uint result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item);
				i++;
			}
			return result;
		}
	}

	public static uint Product<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0], 0),
					2 => function(list[0], 0) * function(list[1], 1),
					3 => function(list[0], 0) * function(list[1], 1) * function(list[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0], 0),
					2 => function(array[0], 0) * function(array[1], 1),
					3 => function(array[0], 0) * function(array[1], 1) * function(array[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0], 0),
					2 => function(list2[0], 0) * function(list2[1], 1),
					3 => function(list2[0], 0) * function(list2[1], 1) * function(list2[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result *= function(item, i);
			}
			return result;
		}
		else
		{
			uint result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item, i);
				i++;
			}
			return result;
		}
	}

	public static long Product<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0]),
					2 => function(list[0]) * function(list[1]),
					3 => function(list[0]) * function(list[1]) * function(list[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0]),
					2 => function(array[0]) * function(array[1]),
					3 => function(array[0]) * function(array[1]) * function(array[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < array.Length; i++)
				result *= function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0]),
					2 => function(list2[0]) * function(list2[1]),
					3 => function(list2[0]) * function(list2[1]) * function(list2[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0]),
					2 => function(list3[0]) * function(list3[1]),
					3 => function(list3[0]) * function(list3[1]) * function(list3[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list3[i]);
			return result;
		}
		else
		{
			long result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item);
				i++;
			}
			return result;
		}
	}

	public static long Product<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0], 0),
					2 => function(list[0], 0) * function(list[1], 1),
					3 => function(list[0], 0) * function(list[1], 1) * function(list[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0], 0),
					2 => function(array[0], 0) * function(array[1], 1),
					3 => function(array[0], 0) * function(array[1], 1) * function(array[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0], 0),
					2 => function(list2[0], 0) * function(list2[1], 1),
					3 => function(list2[0], 0) * function(list2[1], 1) * function(list2[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result *= function(item, i);
			}
			return result;
		}
		else
		{
			long result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item, i);
				i++;
			}
			return result;
		}
	}

	public static MpzT Product<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0]),
					2 => function(list[0]) * function(list[1]),
					3 => function(list[0]) * function(list[1]) * function(list[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0]),
					2 => function(array[0]) * function(array[1]),
					3 => function(array[0]) * function(array[1]) * function(array[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < array.Length; i++)
				result *= function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0]),
					2 => function(list2[0]) * function(list2[1]),
					3 => function(list2[0]) * function(list2[1]) * function(list2[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0]),
					2 => function(list3[0]) * function(list3[1]),
					3 => function(list3[0]) * function(list3[1]) * function(list3[2]),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
				result *= function(list3[i]);
			return result;
		}
		else
		{
			MpzT result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item);
				i++;
			}
			return result;
		}
	}

	public static MpzT Product<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list[0], 0),
					2 => function(list[0], 0) * function(list[1], 1),
					3 => function(list[0], 0) * function(list[1], 1) * function(list[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => function(array[0], 0),
					2 => function(array[0], 0) * function(array[1], 1),
					3 => function(array[0], 0) * function(array[1], 1) * function(array[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list2[0], 0),
					2 => function(list2[0], 0) * function(list2[1], 1),
					3 => function(list2[0], 0) * function(list2[1], 1) * function(list2[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => function(list3[0], 0),
					2 => function(list3[0], 0) * function(list3[1], 1),
					3 => function(list3[0], 0) * function(list3[1], 1) * function(list3[2], 2),
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result *= function(item, i);
			}
			return result;
		}
		else
		{
			MpzT result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= function(item, i);
				i++;
			}
			return result;
		}
	}

	public static decimal Sum<T>(this G.IEnumerable<T> source, Func<T, decimal> function) => SumInternal<T, decimal, decimal, decimal>(source, function);

	public static decimal Sum<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) => SumInternal<T, decimal, decimal, decimal>(source, function);

	public static double Sum<T>(this G.IEnumerable<T> source, Func<T, double> function) => SumInternal<T, double, double, double>(source, function);

	public static double Sum<T>(this G.IEnumerable<T> source, Func<T, int, double> function) => SumInternal<T, double, double, double>(source, function);

	public static int Sum<T>(this G.IEnumerable<T> source, Func<T, int> function) => SumInternal<T, int, long, int>(source, function);

	public static int Sum<T>(this G.IEnumerable<T> source, Func<T, int, int> function) => SumInternal<T, int, long, int>(source, function);

	public static uint Sum<T>(this G.IEnumerable<T> source, Func<T, uint> function) => SumInternal<T, uint, ulong, uint>(source, function);

	public static uint Sum<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) => SumInternal<T, uint, ulong, uint>(source, function);

	public static long Sum<T>(this G.IEnumerable<T> source, Func<T, long> function) => SumInternal<T, long, MpzT, long>(source, function);

	public static long Sum<T>(this G.IEnumerable<T> source, Func<T, int, long> function) => SumInternal<T, long, MpzT, long>(source, function);

	public static MpzT Sum<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) => SumInternal<T, MpzT, MpzT, MpzT>(source, function);

	public static MpzT Sum<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) => SumInternal<T, MpzT, MpzT, MpzT>(source, function);

	private static TResult SumInternal<TSource, TFunction, TAccumulator, TResult>(G.IEnumerable<TSource> source, Func<TSource, TFunction> function) where TFunction : struct, INumber<TFunction> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(function);
		using var e = source.GetEnumerator();
		if (!e.MoveNext())
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(function(e.Current));
		var count = 1;
		while (e.MoveNext())
		{
			checked
			{
				sum += TAccumulator.CreateChecked(function(e.Current));
				count++;
			}
		}
		return TResult.CreateTruncating(sum);
	}

	private static TResult SumInternal<TSource, TFunction, TAccumulator, TResult>(G.IEnumerable<TSource> source, Func<TSource, int, TFunction> function) where TFunction : struct, INumber<TFunction> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(function);
		using var e = source.GetEnumerator();
		if (!e.MoveNext())
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(function(e.Current, 0));
		var count = 1;
		while (e.MoveNext())
		{
			checked
			{
				sum += TAccumulator.CreateChecked(function(e.Current, count));
				count++;
			}
		}
		return TResult.CreateTruncating(sum);
	}

	public static decimal Max<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static decimal Max<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static double Max<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static double Max<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static int Max<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static int Max<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static uint Max<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static uint Max<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static long Max<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static long Max<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static MpzT Max<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static MpzT Max<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static decimal Max(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Max(source[0], source[1]),
				3 => Math.Max(Math.Max(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static double Max(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Max(source[0], source[1]),
				3 => Math.Max(Math.Max(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static int Max(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Max(source[0], source[1]),
				3 => Math.Max(Math.Max(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static uint Max(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Max(source[0], source[1]),
				3 => Math.Max(Math.Max(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static long Max(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Max(source[0], source[1]),
				3 => Math.Max(Math.Max(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static MpzT Max(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => MpzT.Max(source[0], source[1]),
				3 => MpzT.Max(MpzT.Max(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) > indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static decimal Mean<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) => MeanInternal<T, decimal, decimal, decimal>(source, function);

	public static decimal Mean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) => MeanInternal<T, decimal, decimal, decimal>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, double> function) => MeanInternal<T, double, double, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) => MeanInternal<T, double, double, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, int> function) => MeanInternal<T, int, long, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) => MeanInternal<T, int, long, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, uint> function) => MeanInternal<T, uint, ulong, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) => MeanInternal<T, uint, ulong, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, long> function) => MeanInternal<T, long, MpzT, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) => MeanInternal<T, long, MpzT, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) => MeanInternal<T, MpzT, MpzT, double>(source, function);

	public static double Mean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) => MeanInternal<T, MpzT, MpzT, double>(source, function);

	private static TResult MeanInternal<TSource, TFunction, TAccumulator, TResult>(ReadOnlySpan<TSource> source, Func<TSource, TFunction> function) where TFunction : struct, INumber<TFunction> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source.Length == 0)
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(function(source[0]));
		for (var i = 1; i < source.Length; i++)
		{
			checked
			{
				sum += TAccumulator.CreateChecked(function(source[i]));
			}
		}
		return TResult.CreateChecked(sum) / TResult.CreateChecked(source.Length);
	}

	private static TResult MeanInternal<TSource, TFunction, TAccumulator, TResult>(ReadOnlySpan<TSource> source, Func<TSource, int, TFunction> function) where TFunction : struct, INumber<TFunction> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source.Length == 0)
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(function(source[0], 0));
		for (var i = 1; i < source.Length; i++)
		{
			checked
			{
				sum += TAccumulator.CreateChecked(function(source[i], i));
			}
		}
		return TResult.CreateChecked(sum) / TResult.CreateChecked(source.Length);
	}

	public static decimal Mean(this ReadOnlySpan<decimal> source) => MeanInternal<decimal, decimal, decimal>(source);

	public static double Mean(this ReadOnlySpan<double> source) => MeanInternal<double, double, double>(source);

	public static double Mean(this ReadOnlySpan<int> source) => MeanInternal<int, long, double>(source);

	public static double Mean(this ReadOnlySpan<uint> source) => MeanInternal<uint, ulong, double>(source);

	public static double Mean(this ReadOnlySpan<long> source) => MeanInternal<long, MpzT, double>(source);

	public static double Mean(this ReadOnlySpan<MpzT> source) => MeanInternal<MpzT, MpzT, double>(source);

	private static TResult MeanInternal<TSource, TAccumulator, TResult>(ReadOnlySpan<TSource> source) where TSource : struct, INumber<TSource> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		if (source.Length == 0)
			return TResult.Zero;
		var sum = TAccumulator.CreateChecked(source[0]);
		for (var i = 1; i < source.Length; i++)
		{
			checked
			{
				sum += TAccumulator.CreateChecked(source[i]);
			}
		}
		return TResult.CreateChecked(sum) / TResult.CreateChecked(source.Length);
	}

	public static decimal Median<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static decimal Median<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static double Median<T>(this ReadOnlySpan<T> source, Func<T, double> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static double Median<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static int Median<T>(this ReadOnlySpan<T> source, Func<T, int> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static int Median<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static uint Median<T>(this ReadOnlySpan<T> source, Func<T, uint> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static uint Median<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static long Median<T>(this ReadOnlySpan<T> source, Func<T, long> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static long Median<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? default : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static MpzT Median<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? 0 : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static MpzT Median<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) => function == null
			? throw new ArgumentNullException(nameof(function))
			: source.Length == 0 ? 0 : source.ToList(function).Sort()[(source.Length - 1) / 2];

	public static decimal Median(this ReadOnlySpan<decimal> source) => source.Length == 0 ? default : new NList<decimal>(source).Sort()[(source.Length - 1) / 2];

	public static double Median(this ReadOnlySpan<double> source) => source.Length == 0 ? default : new NList<double>(source).Sort()[(source.Length - 1) / 2];

	public static int Median(this ReadOnlySpan<int> source) => source.Length == 0 ? default : new NList<int>(source).Sort()[(source.Length - 1) / 2];

	public static uint Median(this ReadOnlySpan<uint> source) => source.Length == 0 ? default : new NList<uint>(source).Sort()[(source.Length - 1) / 2];

	public static long Median(this ReadOnlySpan<long> source) => source.Length == 0 ? default : new NList<long>(source).Sort()[(source.Length - 1) / 2];

	public static MpzT Median(this ReadOnlySpan<MpzT> source) => source.Length == 0 ? 0 : new List<MpzT>(source).Sort()[(source.Length - 1) / 2];

	public static T? Median<T>(this ReadOnlySpan<T> source) => source.Length == 0 ? default : new List<T>(source).Sort()[(source.Length - 1) / 2];

	public static decimal Min<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static decimal Min<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static double Min<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static double Min<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static int Min<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static int Min<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static uint Min<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static uint Min<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static long Min<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static long Min<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static MpzT Min<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item);
			else if ((f = function(item)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static MpzT Min<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = function(item, i);
			else if ((f = function(item, i)) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static decimal Min(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Min(source[0], source[1]),
				3 => Math.Min(Math.Min(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		decimal indicator = 0;
		decimal f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static double Min(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Min(source[0], source[1]),
				3 => Math.Min(Math.Min(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		double indicator = 0;
		double f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static int Min(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Min(source[0], source[1]),
				3 => Math.Min(Math.Min(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = 0;
		int f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static uint Min(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Min(source[0], source[1]),
				3 => Math.Min(Math.Min(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		uint indicator = 0;
		uint f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static long Min(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => Math.Min(source[0], source[1]),
				3 => Math.Min(Math.Min(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		long indicator = 0;
		long f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static MpzT Min(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => source[0],
				2 => MpzT.Min(source[0], source[1]),
				3 => MpzT.Min(MpzT.Min(source[0], source[1]), source[2]),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		MpzT indicator = 0;
		MpzT f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (i == 0)
				indicator = item;
			else if ((f = item) < indicator!)
				indicator = f;
		}
		return indicator;
	}

	public static decimal Product(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list[0],
					2 => list[0] * list[1],
					3 => list[0] * list[1] * list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= item;
			}
			return result;
		}
		else if (source is decimal[] array)
		{
			decimal result = 1;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => array[0],
					2 => array[0] * array[1],
					3 => array[0] * array[1] * array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= item;
			}
			return result;
		}
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list2[0],
					2 => list2[0] * list2[1],
					3 => list2[0] * list2[1] * list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= item;
			}
			return result;
		}
		else
		{
			decimal result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= item;
				i++;
			}
			return result;
		}
	}

	public static double Product(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list[0],
					2 => list[0] * list[1],
					3 => list[0] * list[1] * list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= item;
			}
			return result;
		}
		else if (source is double[] array)
		{
			double result = 1;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => array[0],
					2 => array[0] * array[1],
					3 => array[0] * array[1] * array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= item;
			}
			return result;
		}
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list2[0],
					2 => list2[0] * list2[1],
					3 => list2[0] * list2[1] * list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= item;
			}
			return result;
		}
		else
		{
			double result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= item;
				i++;
			}
			return result;
		}
	}

	public static int Product(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list[0],
					2 => list[0] * list[1],
					3 => list[0] * list[1] * list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= item;
			}
			return result;
		}
		else if (source is int[] array)
		{
			var result = 1;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => array[0],
					2 => array[0] * array[1],
					3 => array[0] * array[1] * array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= item;
			}
			return result;
		}
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list2[0],
					2 => list2[0] * list2[1],
					3 => list2[0] * list2[1] * list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= item;
			}
			return result;
		}
		else
		{
			var result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= item;
				i++;
			}
			return result;
		}
	}

	public static uint Product(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list[0],
					2 => list[0] * list[1],
					3 => list[0] * list[1] * list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= item;
			}
			return result;
		}
		else if (source is uint[] array)
		{
			uint result = 1;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => array[0],
					2 => array[0] * array[1],
					3 => array[0] * array[1] * array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= item;
			}
			return result;
		}
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list2[0],
					2 => list2[0] * list2[1],
					3 => list2[0] * list2[1] * list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= item;
			}
			return result;
		}
		else
		{
			uint result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= item;
				i++;
			}
			return result;
		}
	}

	public static long Product(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list[0],
					2 => list[0] * list[1],
					3 => list[0] * list[1] * list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= item;
			}
			return result;
		}
		else if (source is long[] array)
		{
			long result = 1;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => array[0],
					2 => array[0] * array[1],
					3 => array[0] * array[1] * array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= item;
			}
			return result;
		}
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list2[0],
					2 => list2[0] * list2[1],
					3 => list2[0] * list2[1] * list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= item;
			}
			return result;
		}
		else
		{
			long result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= item;
				i++;
			}
			return result;
		}
	}

	public static MpzT Product(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			MpzT result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result *= item;
			}
			return result;
		}
		else if (source is MpzT[] array)
		{
			MpzT result = 1;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 1,
					1 => array[0],
					2 => array[0] * array[1],
					3 => array[0] * array[1] * array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result *= item;
			}
			return result;
		}
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 1,
					1 => list2[0],
					2 => list2[0] * list2[1],
					3 => list2[0] * list2[1] * list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 1;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result *= item;
			}
			return result;
		}
		else
		{
			MpzT result = 1;
			var i = 0;
			foreach (var item in source)
			{
				result *= item;
				i++;
			}
			return result;
		}
	}

	public static decimal Sum(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => list[0] + list[1],
					3 => list[0] + list[1] + list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result += item;
			}
			return result;
		}
		else if (source is decimal[] array)
		{
			decimal result = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => array[0] + array[1],
					3 => array[0] + array[1] + array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result += item;
			}
			return result;
		}
		else if (source is G.IList<decimal> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => list2[0] + list2[1],
					3 => list2[0] + list2[1] + list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			decimal result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result += item;
			}
			return result;
		}
		else
		{
			decimal result = 0;
			var i = 0;
			foreach (var item in source)
			{
				result += item;
				i++;
			}
			return result;
		}
	}

	public static double Sum(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => list[0] + list[1],
					3 => list[0] + list[1] + list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result += item;
			}
			return result;
		}
		else if (source is double[] array)
		{
			double result = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => array[0] + array[1],
					3 => array[0] + array[1] + array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result += item;
			}
			return result;
		}
		else if (source is G.IList<double> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => list2[0] + list2[1],
					3 => list2[0] + list2[1] + list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			double result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result += item;
			}
			return result;
		}
		else
		{
			double result = 0;
			var i = 0;
			foreach (var item in source)
			{
				result += item;
				i++;
			}
			return result;
		}
	}

	public static int Sum(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => list[0] + list[1],
					3 => list[0] + list[1] + list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result += item;
			}
			return result;
		}
		else if (source is int[] array)
		{
			var result = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => array[0] + array[1],
					3 => array[0] + array[1] + array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result += item;
			}
			return result;
		}
		else if (source is G.IList<int> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => list2[0] + list2[1],
					3 => list2[0] + list2[1] + list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			var result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result += item;
			}
			return result;
		}
		else
		{
			var result = 0;
			var i = 0;
			foreach (var item in source)
			{
				result += item;
				i++;
			}
			return result;
		}
	}

	public static uint Sum(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => list[0] + list[1],
					3 => list[0] + list[1] + list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result += item;
			}
			return result;
		}
		else if (source is uint[] array)
		{
			uint result = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => array[0] + array[1],
					3 => array[0] + array[1] + array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result += item;
			}
			return result;
		}
		else if (source is G.IList<uint> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => list2[0] + list2[1],
					3 => list2[0] + list2[1] + list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			uint result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result += item;
			}
			return result;
		}
		else
		{
			uint result = 0;
			var i = 0;
			foreach (var item in source)
			{
				result += item;
				i++;
			}
			return result;
		}
	}

	public static long Sum(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var length = list.Length;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list[0],
					2 => list[0] + list[1],
					3 => list[0] + list[1] + list[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result += item;
			}
			return result;
		}
		else if (source is long[] array)
		{
			long result = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => array[0] + array[1],
					3 => array[0] + array[1] + array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result += item;
			}
			return result;
		}
		else if (source is G.IList<long> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => list2[0] + list2[1],
					3 => list2[0] + list2[1] + list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			long result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result += item;
			}
			return result;
		}
		else
		{
			long result = 0;
			var i = 0;
			foreach (var item in source)
			{
				result += item;
				i++;
			}
			return result;
		}
	}

	public static MpzT Sum(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var length = list.Length;
			MpzT result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result += item;
			}
			return result;
		}
		else if (source is MpzT[] array)
		{
			MpzT result = 0;
			if (array.Length <= 3)
			{
				return array.Length switch
				{
					0 => 0,
					1 => array[0],
					2 => array[0] + array[1],
					3 => array[0] + array[1] + array[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result += item;
			}
			return result;
		}
		else if (source is G.IList<MpzT> list2)
		{
			var length = list2.Count;
			if (length <= 3)
			{
				return length switch
				{
					0 => 0,
					1 => list2[0],
					2 => list2[0] + list2[1],
					3 => list2[0] + list2[1] + list2[2],
					_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
				};
			}
			MpzT result = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result += item;
			}
			return result;
		}
		else
		{
			MpzT result = 0;
			var i = 0;
			foreach (var item in source)
			{
				result += item;
				i++;
			}
			return result;
		}
	}

	public static decimal Sum<T>(this ReadOnlySpan<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		decimal result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i]);
		return result;
	}

	public static decimal Sum<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		decimal result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i], i);
		return result;
	}

	public static double Sum<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		double result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i]);
		return result;
	}

	public static double Sum<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		double result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i], i);
		return result;
	}

	public static int Sum<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i]);
		return result;
	}

	public static int Sum<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i], i);
		return result;
	}

	public static uint Sum<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		uint result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i]);
		return result;
	}

	public static uint Sum<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		uint result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i], i);
		return result;
	}

	public static long Sum<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		long result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i]);
		return result;
	}

	public static long Sum<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		long result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i], i);
		return result;
	}

	public static MpzT Sum<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		MpzT result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i]);
		return result;
	}

	public static MpzT Sum<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		MpzT result = 0;
		for (var i = 0; i < length; i++)
			result += function(source[i], i);
		return result;
	}

	public static decimal Sum(this ReadOnlySpan<decimal> source)
	{
		var length = source.Length;
		decimal result = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result += item;
		}
		return result;
	}

	public static double Sum(this ReadOnlySpan<double> source)
	{
		var length = source.Length;
		double result = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result += item;
		}
		return result;
	}

	public static int Sum(this ReadOnlySpan<int> source)
	{
		var length = source.Length;
		var result = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result += item;
		}
		return result;
	}

	public static uint Sum(this ReadOnlySpan<uint> source)
	{
		var length = source.Length;
		uint result = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result += item;
		}
		return result;
	}

	public static long Sum(this ReadOnlySpan<long> source)
	{
		var length = source.Length;
		long result = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result += item;
		}
		return result;
	}

	public static MpzT Sum(this ReadOnlySpan<MpzT> source)
	{
		var length = source.Length;
		MpzT result = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result += item;
		}
		return result;
	}

	public static decimal PMax<T>(this G.IReadOnlyList<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static decimal PMax<T>(this G.IReadOnlyList<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static double PMax<T>(this G.IReadOnlyList<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static double PMax<T>(this G.IReadOnlyList<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static int PMax<T>(this G.IReadOnlyList<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static int PMax<T>(this G.IReadOnlyList<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static uint PMax<T>(this G.IReadOnlyList<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static uint PMax<T>(this G.IReadOnlyList<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static long PMax<T>(this G.IReadOnlyList<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Max(function(source[0]), function(source[1])),
				3 => Math.Max(Math.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static long PMax<T>(this G.IReadOnlyList<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Max(function(source[0], 0), function(source[1], 1)),
				3 => Math.Max(Math.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static MpzT PMax<T>(this G.IReadOnlyList<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => MpzT.Max(function(source[0]), function(source[1])),
				3 => MpzT.Max(MpzT.Max(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static MpzT PMax<T>(this G.IReadOnlyList<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => MpzT.Max(function(source[0], 0), function(source[1], 1)),
				3 => MpzT.Max(MpzT.Max(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f < indicator)
				return;
			lock (lockObj)
				if (f > indicator)
					indicator = f;
		});
		return indicator;
	}

	public static decimal PMin<T>(this G.IReadOnlyList<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static decimal PMin<T>(this G.IReadOnlyList<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static double PMin<T>(this G.IReadOnlyList<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static double PMin<T>(this G.IReadOnlyList<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static int PMin<T>(this G.IReadOnlyList<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static int PMin<T>(this G.IReadOnlyList<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static uint PMin<T>(this G.IReadOnlyList<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static uint PMin<T>(this G.IReadOnlyList<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static long PMin<T>(this G.IReadOnlyList<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => Math.Min(function(source[0]), function(source[1])),
				3 => Math.Min(Math.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static long PMin<T>(this G.IReadOnlyList<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => Math.Min(function(source[0], 0), function(source[1], 1)),
				3 => Math.Min(Math.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static MpzT PMin<T>(this G.IReadOnlyList<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0]),
				2 => MpzT.Min(function(source[0]), function(source[1])),
				3 => MpzT.Min(MpzT.Min(function(source[0]), function(source[1])), function(source[2])),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0]);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static MpzT PMin<T>(this G.IReadOnlyList<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		if (length <= 3)
		{
			return length switch
			{
				0 => 0,
				1 => function(source[0], 0),
				2 => MpzT.Min(function(source[0], 0), function(source[1], 1)),
				3 => MpzT.Min(MpzT.Min(function(source[0], 0), function(source[1], 1)), function(source[2], 2)),
				_ => throw new InvalidOperationException("Произошла внутренняя ошибка. Возможно, вы пытаетесь писать в один список"
					+ " в несколько потоков? Если нет, повторите попытку позже, возможно, какая-то аппаратная ошибка.")
			};
		}
		var indicator = function(source[0], 0);
		object lockObj = new();
		Parallel.For(1, source.Count, i =>
		{
			var item = source[i];
			var f = function(item, i);
			if (f > indicator)
				return;
			lock (lockObj)
				if (f < indicator)
					indicator = f;
		});
		return indicator;
	}

	public static TResult? Max<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		var i = 0;
		return E.Max(source, x => function(x, i++));
	}

	public static TResult? Min<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		var i = 0;
		return E.Min(source, x => function(x, i++));
	}

	public static TResult? Max<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> selector) => E.Max(source, selector);
	public static T? Max<T>(this G.IEnumerable<T> source) => E.Max(source);
	public static decimal Max(params decimal[] source) => Max(source.AsSpan());
	public static double Max(params double[] source) => Max(source.AsSpan());
	public static int Max(params int[] source) => Max(source.AsSpan());
	public static uint Max(params uint[] source) => Max(source.AsSpan());
	public static long Max(params long[] source) => Max(source.AsSpan());
	public static MpzT Max(params MpzT[] source) => Max(source.AsSpan());
	public static T? Max<T>(params T?[] source) => E.Max(source);
	public static TResult? Min<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> selector) => E.Min(source, selector);
	public static T? Min<T>(this G.IEnumerable<T> source) => E.Min(source);
	public static decimal Min(params decimal[] source) => Min(source.AsSpan());
	public static double Min(params double[] source) => Min(source.AsSpan());
	public static int Min(params int[] source) => Min(source.AsSpan());
	public static uint Min(params uint[] source) => Min(source.AsSpan());
	public static long Min(params long[] source) => Min(source.AsSpan());
	public static MpzT Min(params MpzT[] source) => Min(source.AsSpan());
	public static T? Min<T>(params T?[] source) => E.Min(source);
	public static decimal Sum<T>(this Span<T> source, Func<T, decimal> function) => Sum((ReadOnlySpan<T>)source, function);
	public static decimal Sum<T>(this Span<T> source, Func<T, int, decimal> function) => Sum((ReadOnlySpan<T>)source, function);
	public static decimal Sum<T>(this T[] source, Func<T, decimal> function) => Sum((G.IList<T>)source, function);
	public static decimal Sum<T>(this T[] source, Func<T, int, decimal> function) => Sum((G.IList<T>)source, function);
	public static double Sum<T>(this Span<T> source, Func<T, double> function) => Sum((ReadOnlySpan<T>)source, function);
	public static double Sum<T>(this Span<T> source, Func<T, int, double> function) => Sum((ReadOnlySpan<T>)source, function);
	public static double Sum<T>(this T[] source, Func<T, double> function) => Sum((G.IList<T>)source, function);
	public static double Sum<T>(this T[] source, Func<T, int, double> function) => Sum((G.IList<T>)source, function);
	public static int Sum<T>(this Span<T> source, Func<T, int> function) => Sum((ReadOnlySpan<T>)source, function);
	public static int Sum<T>(this Span<T> source, Func<T, int, int> function) => Sum((ReadOnlySpan<T>)source, function);
	public static int Sum<T>(this T[] source, Func<T, int> function) => Sum((G.IList<T>)source, function);
	public static int Sum<T>(this T[] source, Func<T, int, int> function) => Sum((G.IList<T>)source, function);
	public static uint Sum<T>(this Span<T> source, Func<T, uint> function) => Sum((ReadOnlySpan<T>)source, function);
	public static uint Sum<T>(this Span<T> source, Func<T, int, uint> function) => Sum((ReadOnlySpan<T>)source, function);
	public static uint Sum<T>(this T[] source, Func<T, uint> function) => Sum((G.IList<T>)source, function);
	public static uint Sum<T>(this T[] source, Func<T, int, uint> function) => Sum((G.IList<T>)source, function);
	public static long Sum<T>(this Span<T> source, Func<T, long> function) => Sum((ReadOnlySpan<T>)source, function);
	public static long Sum<T>(this Span<T> source, Func<T, int, long> function) => Sum((ReadOnlySpan<T>)source, function);
	public static long Sum<T>(this T[] source, Func<T, long> function) => Sum((G.IList<T>)source, function);
	public static long Sum<T>(this T[] source, Func<T, int, long> function) => Sum((G.IList<T>)source, function);
	public static MpzT Sum<T>(this Span<T> source, Func<T, MpzT> function) => Sum((ReadOnlySpan<T>)source, function);
	public static MpzT Sum<T>(this Span<T> source, Func<T, int, MpzT> function) => Sum((ReadOnlySpan<T>)source, function);
	public static MpzT Sum<T>(this T[] source, Func<T, MpzT> function) => Sum((G.IList<T>)source, function);
	public static MpzT Sum<T>(this T[] source, Func<T, int, MpzT> function) => Sum((G.IList<T>)source, function);
	public static decimal Sum(params decimal[] source) => Sum(source.AsSpan());
	public static double Sum(params double[] source) => Sum(source.AsSpan());
	public static int Sum(params int[] source) => Sum(source.AsSpan());
	public static uint Sum(params uint[] source) => Sum(source.AsSpan());
	public static long Sum(params long[] source) => Sum(source.AsSpan());
	public static MpzT Sum(params MpzT[] source) => Sum(source.AsSpan());
}
