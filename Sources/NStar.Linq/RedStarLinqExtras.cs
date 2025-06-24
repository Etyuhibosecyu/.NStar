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
using NStar.MathLib;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace NStar.Linq;

public static class RedStarLinqExtras
{
	internal static readonly Random random = new();

	public static bool All<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!function(item, i))
					return false;
			}
			return true;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!function(item, i))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!function(item, i))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!function(item, i))
					return false;
			}
			return true;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (!function(item, i))
					return false;
				i++;
			}
			return true;
		}
	}

	public static bool AllEqual<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			var prev = function(list[0]);
			for (var i = 1; i < length; i++)
			{
				var item = function(list[i]);
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			var prev = function(array[0]);
			for (var i = 1; i < array.Length; i++)
			{
				var item = function(array[i]);
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			var prev = function(list2[0]);
			for (var i = 1; i < length; i++)
			{
				var item = function(list2[i]);
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			var prev = function(list3[0]);
			for (var i = 1; i < length; i++)
			{
				var item = function(list3[i]);
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
				prev = item;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			var prev = function(en.Current);
			while (en.MoveNext())
			{
				var item = function(en.Current);
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
				prev = item;
			}
			return true;
		}
	}

	public static bool AllEqual<T>(this G.IEnumerable<T> source, Func<T, T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			var prev = list[0];
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				if (!function(item, prev))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			var prev = array[0];
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				if (!function(item, prev))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			var prev = list2[0];
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				if (!function(item, prev))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			var prev = list3[0];
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				if (!function(item, prev))
					return false;
				prev = item;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			var prev = en.Current;
			while (en.MoveNext())
			{
				var item = en.Current;
				if (!function(item, prev))
					return false;
				prev = item;
			}
			return true;
		}
	}

	public static bool AllEqual<T>(this G.IEnumerable<T> source, Func<T, T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			var prev = list[0];
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				if (!function(item, prev, i - 1))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			var prev = array[0];
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				if (!function(item, prev, i - 1))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			var prev = list2[0];
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				if (!function(item, prev, i - 1))
					return false;
				prev = item;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			var prev = list3[0];
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				if (!function(item, prev, i - 1))
					return false;
				prev = item;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			var i = 1;
			var prev = en.Current;
			while (en.MoveNext())
			{
				var item = en.Current;
				if (!function(item, prev, i - 1))
					return false;
				prev = item;
				i++;
			}
			return true;
		}
	}

	public static bool AllEqual<T>(this G.IEnumerable<T> source)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			var prev = list[0];
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			var prev = array[0];
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			var prev = list2[0];
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			var prev = list3[0];
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			var prev = en.Current;
			while (en.MoveNext())
			{
				var item = en.Current;
				if (!(item?.Equals(prev) ?? prev == null))
					return false;
				prev = item;
			}
			return true;
		}
	}

	public static bool AllUnique<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			ListHashSet<TResult> hs = new(length, function(list[0]));
			for (var i = 1; i < length; i++)
			{
				var item = function(list[i]);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			ListHashSet<TResult> hs = new(array.Length, function(array[0]));
			for (var i = 1; i < array.Length; i++)
			{
				var item = function(array[i]);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			ListHashSet<TResult> hs = new(length, function(list2[0]));
			for (var i = 1; i < length; i++)
			{
				var item = function(list2[i]);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			ListHashSet<TResult> hs = new(length, function(list3[0]));
			for (var i = 1; i < length; i++)
			{
				var item = function(list3[i]);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			ListHashSet<TResult> hs = new(source.TryGetLengthEasily(out var length) ? length : 1024, function(en.Current));
			while (en.MoveNext())
			{
				var item = function(en.Current);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
	}

	public static bool AllUnique<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			ListHashSet<TResult> hs = new(length, function(list[0], 0));
			for (var i = 1; i < length; i++)
			{
				var item = function(list[i], i);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			ListHashSet<TResult> hs = new(array.Length, function(array[0], 0));
			for (var i = 1; i < array.Length; i++)
			{
				var item = function(array[i], i);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			ListHashSet<TResult> hs = new(length, function(list2[0], 0));
			for (var i = 1; i < length; i++)
			{
				var item = function(list2[i], i);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			ListHashSet<TResult> hs = new(length, function(list3[0], 0));
			for (var i = 1; i < length; i++)
			{
				var item = function(list3[i], i);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			ListHashSet<TResult> hs = new(source.TryGetLengthEasily(out var length) ? length : 1024, function(en.Current, 0));
			var i = 1;
			while (en.MoveNext())
			{
				var item = function(en.Current, i++);
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
	}

	public static bool AllUnique<T>(this G.IEnumerable<T> source)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			if (length <= 1)
				return true;
			ListHashSet<T> hs = new(length, list[0]);
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is T[] array)
		{
			if (array.Length <= 1)
				return true;
			ListHashSet<T> hs = new(array.Length, array[0]);
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			if (length <= 1)
				return true;
			ListHashSet<T> hs = new(length, list2[0]);
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			if (length <= 1)
				return true;
			ListHashSet<T> hs = new(length, list3[0]);
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			if (!en.MoveNext())
				return true;
			ListHashSet<T> hs = new(source.TryGetLengthEasily(out var length) ? length : 1024, en.Current);
			while (en.MoveNext())
			{
				var item = en.Current;
				if (!hs.TryAdd(item))
					return false;
			}
			return true;
		}
	}

	public static bool Any<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item, i))
					return true;
			}
			return false;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item, i))
					return true;
			}
			return false;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item, i))
					return true;
			}
			return false;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item, i))
					return true;
			}
			return false;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (function(item, i))
					return true;
				i++;
			}
			return false;
		}
	}

	private class AppendList<T> : BaseIndexable<T, AppendList<T>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly T element;

		public AppendList()
		{
			source = [];
			element = default!;
		}

		public AppendList(G.IReadOnlyList<T> source, T element)
		{
			this.source = source;
			this.element = element;
			_size = source.Count + 1;
		}

		public override Span<T> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override T GetInternal(int index, bool invoke = true) => index == source.Count ? element : source[index];

		protected override AppendList<T> GetRangeInternal(int index, int length) => index == _size - length ? new(source.GetROLSlice(index, length - 1), element) : new(source.GetROLSlice(index, length - 1), source[index + length - 1]);

		protected override Slice<T> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(T item, int index, int length)
		{
			for (var i = index; i < Min(index + length, source.Count); i++)
				if (source[i - 1]?.Equals(item) ?? item == null)
					return i;
			if (index == _size - length && (element?.Equals(item) ?? item == null))
				return source.Count;
			return -1;
		}

		protected override int LastIndexOfInternal(T item, int index, int length)
		{
			if (index == source.Count && (element?.Equals(item) ?? item == null))
				return 0;
			var endIndex = index - length + 1;
			for (var i = Min(index, source.Count - 1); i >= endIndex; i++)
				if (source[i - 1]?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	public static Slice<T> Append<T>(this G.IReadOnlyList<T> source, T element) => new AppendList<T>(source, element).GetROLSlice();

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
			}
			return (result, result2);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			return (result, result2);
		}
	}

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, (TResult, TResult2)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				(result[i], result2[i]) = function(item);
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				(result[i], result2[i]) = function(item);
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				(result[i], result2[i]) = function(item);
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				(result[i], result2[i]) = function(item);
			}
			return (result, result2);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			return (result, result2);
		}
	}

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, int, (TResult, TResult2)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				(result[i], result2[i]) = function(item, i);
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				(result[i], result2[i]) = function(item, i);
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				(result[i], result2[i]) = function(item, i);
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				(result[i], result2[i]) = function(item, i);
			}
			return (result, result2);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item, i);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			return (result, result2);
		}
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item);
				result2[i] = function2(item);
				result3[i] = function3(item);
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result[i] = function(item);
				result2[i] = function2(item);
				result3[i] = function3(item);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = function(item);
				result2[i] = function2(item);
				result3[i] = function3(item);
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item);
				result2[i] = function2(item);
				result3[i] = function3(item);
			}
			return (result, result2, result3);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			List<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item));
				result2.Add(function2(item));
				result3.Add(function3(item));
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			result3.Resize(i);
			return (result, result2, result3);
		}
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
				result3[i] = function3(item, i);
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
				result3[i] = function3(item, i);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
				result3[i] = function3(item, i);
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item, i);
				result2[i] = function2(item, i);
				result3[i] = function3(item, i);
			}
			return (result, result2, result3);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			List<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				result3.Add(function3(item, i));
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			result3.Resize(i);
			return (result, result2, result3);
		}
	}

	public static (List<T>, List<T2>, List<T3>) Break<T, T2, T3>(this G.IEnumerable<(T, T2, T3)> source)
	{
		if (source is List<(T, T2, T3)> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<T>(length);
			var result2 = RedStarLinq.EmptyList<T2>(length);
			var result3 = RedStarLinq.EmptyList<T3>(length);
			for (var i = 0; i < length; i++)
				(result[i], result2[i], result3[i]) = list[i];
			return (result, result2, result3);
		}
		else if (source is (T, T2, T3)[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<T>(length);
			var result2 = RedStarLinq.EmptyList<T2>(length);
			var result3 = RedStarLinq.EmptyList<T3>(length);
			for (var i = 0; i < array.Length; i++)
				(result[i], result2[i], result3[i]) = array[i];
			return (result, result2, result3);
		}
		else if (source is G.IList<(T, T2, T3)> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<T>(length);
			var result2 = RedStarLinq.EmptyList<T2>(length);
			var result3 = RedStarLinq.EmptyList<T3>(length);
			for (var i = 0; i < length; i++)
				(result[i], result2[i], result3[i]) = list2[i];
			return (result, result2, result3);
		}
		else
		{
			List<T> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<T2> result2 = new(length);
			List<T3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			result3.Resize(i);
			return (result, result2, result3);
		}
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, (TResult, TResult2, TResult3)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				(result[i], result2[i], result3[i]) = function(item);
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				(result[i], result2[i], result3[i]) = function(item);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				(result[i], result2[i], result3[i]) = function(item);
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				(result[i], result2[i], result3[i]) = function(item);
			}
			return (result, result2, result3);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			List<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				result3.Add(x.Item3);
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			result3.Resize(i);
			return (result, result2, result3);
		}
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, int, (TResult, TResult2, TResult3)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				(result[i], result2[i], result3[i]) = function(item, i);
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				(result[i], result2[i], result3[i]) = function(item, i);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				(result[i], result2[i], result3[i]) = function(item, i);
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			var result2 = RedStarLinq.EmptyList<TResult2>(length);
			var result3 = RedStarLinq.EmptyList<TResult3>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				(result[i], result2[i], result3[i]) = function(item, i);
			}
			return (result, result2, result3);
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<TResult2> result2 = new(length);
			List<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item, i);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				result3.Add(x.Item3);
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			result3.Resize(i);
			return (result, result2, result3);
		}
	}

	public static List<T> BreakFilter<T>(this G.IEnumerable<T> source, Func<T, bool> function, out List<T> result2)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(length / 2);
			result2 = new(length / 2);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<T> result = new(array.Length / 2);
			result2 = new(array.Length / 2);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(length / 2);
			result2 = new(length / 2);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(length / 2);
			result2 = new(length / 2);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(source.TryGetLengthEasily(out var length) ? length / 2 : 0);
			result2 = new(length / 2);
			var i = 0;
			foreach (var item in source)
			{
				if (function(item))
					result.Add(item);
				else
					result2.Add(item);
				i++;
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
	}

	public static List<T> BreakFilter<T>(this G.IEnumerable<T> source, Func<T, int, bool> function, out List<T> result2)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(length / 2);
			result2 = new(length / 2);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item, i))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<T> result = new(array.Length / 2);
			result2 = new(array.Length / 2);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item, i))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<T> result = new(length / 2);
			result2 = new(length / 2);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item, i))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<T> result = new(length / 2);
			result2 = new(length / 2);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item, i))
					result.Add(item);
				else
					result2.Add(item);
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
		else
		{
			List<T> result = new(source.TryGetLengthEasily(out var length) ? length / 2 : 0);
			result2 = new(length / 2);
			var i = 0;
			foreach (var item in source)
			{
				if (function(item, i))
					result.Add(item);
				else
					result2.Add(item);
				i++;
			}
			result.TrimExcess();
			result2.TrimExcess();
			return result;
		}
	}

	private class CombineList<T, T2, TResult> : BaseIndexable<TResult, CombineList<T, T2, TResult>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly G.IReadOnlyList<T2> source2;
		private readonly Func<T, T2, TResult> function;

		public CombineList()
		{
			source = [];
			source2 = [];
			function = (x, y) => default!;
		}

		public CombineList(G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T2, TResult> function)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(source2);
			ArgumentNullException.ThrowIfNull(function);
			this.source = source;
			this.source2 = source2;
			this.function = function;
			_size = Min(source.Count, source2.Count);
		}

		public override Span<TResult> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, TResult[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override TResult GetInternal(int index, bool invoke = true) => function(source[index], source2[index]);

		protected override CombineList<T, T2, TResult> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), source2.GetROLSlice(index, length), function);

		protected override Slice<TResult> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(TResult item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (function(source[i], source2[i])?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(TResult item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (function(source[i], source2[i])?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	private class CombineListInt<T, T2, TResult> : BaseIndexable<TResult, CombineListInt<T, T2, TResult>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly G.IReadOnlyList<T2> source2;
		private readonly Func<T, T2, int, TResult> function;

		public CombineListInt()
		{
			source = [];
			source2 = [];
			function = (x, y, index) => default!;
		}

		public CombineListInt(G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T2, int, TResult> function)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(source2);
			ArgumentNullException.ThrowIfNull(function);
			this.source = source;
			this.source2 = source2;
			this.function = function;
			_size = Min(source.Count, source2.Count);
		}

		public override Span<TResult> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, TResult[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override TResult GetInternal(int index, bool invoke = true) => function(source[index], source2[index], index);

		protected override CombineListInt<T, T2, TResult> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), source2.GetROLSlice(index, length), function);

		protected override Slice<TResult> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(TResult item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (function(source[i], source2[i], i)?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(TResult item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (function(source[i], source2[i], i)?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	private class CombineListPure<T, T2> : BaseIndexable<(T, T2), CombineListPure<T, T2>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly G.IReadOnlyList<T2> source2;

		public CombineListPure()
		{
			source = [];
			source2 = [];
		}

		public CombineListPure(G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(source2);
			this.source = source;
			this.source2 = source2;
			_size = Min(source.Count, source2.Count);
		}

		public override Span<(T, T2)> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, (T, T2)[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override (T, T2) GetInternal(int index, bool invoke = true) => (source[index], source2[index]);

		protected override CombineListPure<T, T2> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), source2.GetROLSlice(index, length));

		protected override Slice<(T, T2)> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal((T, T2) item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if ((source[i]?.Equals(item.Item1) ?? item.Item1 == null) && (source2[i]?.Equals(item.Item2) ?? item.Item2 == null))
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal((T, T2) item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if ((source[i]?.Equals(item.Item1) ?? item.Item1 == null) && (source2[i]?.Equals(item.Item2) ?? item.Item2 == null))
					return i;
			return -1;
		}
	}

	private class CombineList<T, T2, T3, TResult> : BaseIndexable<TResult, CombineList<T, T2, T3, TResult>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly G.IReadOnlyList<T2> source2;
		private readonly G.IReadOnlyList<T3> source3;
		private readonly Func<T, T2, T3, TResult> function;

		public CombineList()
		{
			source = [];
			source2 = [];
			source3 = [];
			function = (x, y, z) => default!;
		}

		public CombineList(G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3, Func<T, T2, T3, TResult> function)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(source2);
			ArgumentNullException.ThrowIfNull(source3);
			ArgumentNullException.ThrowIfNull(function);
			this.source = source;
			this.source2 = source2;
			this.source3 = source3;
			this.function = function;
			_size = RedStarLinqMath.Min([source.Count, source2.Count, source3.Count]);
		}

		public override Span<TResult> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, TResult[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override TResult GetInternal(int index, bool invoke = true) => function(source[index], source2[index], source3[index]);

		protected override CombineList<T, T2, T3, TResult> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), source2.GetROLSlice(index, length), source3.GetROLSlice(index, length), function);

		protected override Slice<TResult> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(TResult item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (function(source[i], source2[i], source3[i])?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(TResult item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (function(source[i], source2[i], source3[i])?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	private class CombineListInt<T, T2, T3, TResult> : BaseIndexable<TResult, CombineListInt<T, T2, T3, TResult>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly G.IReadOnlyList<T2> source2;
		private readonly G.IReadOnlyList<T3> source3;
		private readonly Func<T, T2, T3, int, TResult> function;

		public CombineListInt()
		{
			source = [];
			source2 = [];
			source3 = [];
			function = (x, y, z, index) => default!;
		}

		public CombineListInt(G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3, Func<T, T2, T3, int, TResult> function)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(source2);
			ArgumentNullException.ThrowIfNull(source3);
			ArgumentNullException.ThrowIfNull(function);
			this.source = source;
			this.source2 = source2;
			this.source3 = source3;
			this.function = function;
			_size = RedStarLinqMath.Min([source.Count, source2.Count, source3.Count]);
		}

		public override Span<TResult> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, TResult[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override TResult GetInternal(int index, bool invoke = true) => function(source[index], source2[index], source3[index], index);

		protected override CombineListInt<T, T2, T3, TResult> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), source2.GetROLSlice(index, length), source3.GetROLSlice(index, length), function);

		protected override Slice<TResult> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(TResult item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (function(source[i], source2[i], source3[i], i)?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(TResult item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (function(source[i], source2[i], source3[i], i)?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	private class CombineListPure<T, T2, T3> : BaseIndexable<(T, T2, T3), CombineListPure<T, T2, T3>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly G.IReadOnlyList<T2> source2;
		private readonly G.IReadOnlyList<T3> source3;

		public CombineListPure()
		{
			source = [];
			source2 = [];
			source3 = [];
		}

		public CombineListPure(G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(source2);
			ArgumentNullException.ThrowIfNull(source3);
			this.source = source;
			this.source2 = source2;
			this.source3 = source3;
			_size = RedStarLinqMath.Min([source.Count, source2.Count, source3.Count]);
		}

		public override Span<(T, T2, T3)> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, (T, T2, T3)[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override (T, T2, T3) GetInternal(int index, bool invoke = true) => (source[index], source2[index], source3[index]);

		protected override CombineListPure<T, T2, T3> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), source2.GetROLSlice(index, length), source3.GetROLSlice(index, length));

		protected override Slice<(T, T2, T3)> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal((T, T2, T3) item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if ((source[i]?.Equals(item.Item1) ?? item.Item1 == null) && (source2[i]?.Equals(item.Item2) ?? item.Item2 == null) && (source3[i]?.Equals(item.Item3) ?? item.Item3 == null))
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal((T, T2, T3) item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if ((source[i]?.Equals(item.Item1) ?? item.Item1 == null) && (source2[i]?.Equals(item.Item2) ?? item.Item2 == null) && (source3[i]?.Equals(item.Item3) ?? item.Item3 == null))
					return i;
			return -1;
		}
	}

	public static Slice<TResult> Combine<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T2, TResult> function) => new CombineList<T, T2, TResult>(source, source2, function).GetSlice();

	public static Slice<TResult> Combine<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T2, int, TResult> function) => new CombineListInt<T, T2, TResult>(source, source2, function).GetSlice();

	public static Slice<(T, T2)> Combine<T, T2>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2) => new CombineListPure<T, T2>(source, source2).GetSlice();

	public static Slice<TResult> Combine<T, T2, T3, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3, Func<T, T2, T3, TResult> function) => new CombineList<T, T2, T3, TResult>(source, source2, source3, function).GetSlice();

	public static Slice<TResult> Combine<T, T2, T3, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3, Func<T, T2, T3, int, TResult> function) => new CombineListInt<T, T2, T3, TResult>(source, source2, source3, function).GetSlice();

	public static Slice<(T, T2, T3)> Combine<T, T2, T3>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3) => new CombineListPure<T, T2, T3>(source, source2, source3).GetSlice();

	public static G.IEnumerable<TResult> Combine<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				yield return function(item, item2, i);
			}
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				yield return function(item, item2, i);
			}
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				yield return function(item, item2, i);
			}
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				yield return function(item, item2, i);
			}
		}
		else
		{
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			var i = 0;
			while (en.MoveNext() && en2.MoveNext())
			{
				var item = en.Current;
				var item2 = en2.Current;
				yield return function(item, item2, i);
				i++;
			}
		}
	}

	public static List<TResult> Combine<T, T2, T3, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEnumerable<T3> source3, Func<T, T2, T3, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2 && source3 is List<T3> list3)
		{
			var length = RedStarLinqMath.Min([list.Length, list2.Length, list3.Length]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				var item3 = list3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2 && source3 is T3[] array3)
		{
			var length = RedStarLinqMath.Min([array.Length, array2.Length, array3.Length]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				var item3 = array3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2 && source3 is G.IList<T3> list2_3)
		{
			var length = RedStarLinqMath.Min([list2_.Count, list2_2.Count, list2_3.Count]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				var item3 = list2_3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2 && source3 is G.IReadOnlyList<T3> list3_3)
		{
			var length = RedStarLinqMath.Min([list3_.Count, list3_2.Count, list3_3.Count]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				var item3 = list3_3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : source2.TryGetLengthEasily(out length) ? length : source3.TryGetLengthEasily(out length) ? length : 1024);
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			using var en3 = source3.GetEnumerator();
			var i = 0;
			while (en.MoveNext() && en2.MoveNext() && en3.MoveNext())
			{
				var item = en.Current;
				var item2 = en2.Current;
				var item3 = en3.Current;
				result.Add(function(item, item2, item3));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static List<TResult> Combine<T, T2, T3, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEnumerable<T3> source3, Func<T, T2, T3, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2 && source3 is List<T3> list3)
		{
			var length = RedStarLinqMath.Min([list.Length, list2.Length, list3.Length]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				var item3 = list3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2 && source3 is T3[] array3)
		{
			var length = RedStarLinqMath.Min([array.Length, array2.Length, array3.Length]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				var item3 = array3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2 && source3 is G.IList<T3> list2_3)
		{
			var length = RedStarLinqMath.Min([list2_.Count, list2_2.Count, list2_3.Count]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				var item3 = list2_3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2 && source3 is G.IReadOnlyList<T3> list3_3)
		{
			var length = RedStarLinqMath.Min([list3_.Count, list3_2.Count, list3_3.Count]);
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				var item3 = list3_3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : source2.TryGetLengthEasily(out length) ? length : source3.TryGetLengthEasily(out length) ? length : 1024);
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			using var en3 = source3.GetEnumerator();
			var i = 0;
			while (en.MoveNext() && en2.MoveNext() && en3.MoveNext())
			{
				var item = en.Current;
				var item2 = en2.Current;
				var item3 = en3.Current;
				result.Add(function(item, item2, item3, i));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static List<T> Concat<T>(this G.IEnumerable<T> source, params G.IEnumerable<T>[] collections)
	{
		ArgumentNullException.ThrowIfNull(collections);
		List<T> result = [.. source];
		for (var i = 0; i < collections.Length; i++)
			result.AddRange(collections[i]);
		return result;
	}

	public static bool Contains<T>(this G.IEnumerable<T> source, T target)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (item?.Equals(target) ?? false)
					return true;
			}
			return false;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (item?.Equals(target) ?? false)
					return true;
			}
			return false;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (item?.Equals(target) ?? false)
					return true;
			}
			return false;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (item?.Equals(target) ?? false)
					return true;
			}
			return false;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (item?.Equals(target) ?? false)
					return true;
				i++;
			}
			return false;
		}
	}

	public static bool Contains<T>(this G.IEnumerable<T> source, T target, G.IEqualityComparer<T> comparer)
	{
		ArgumentNullException.ThrowIfNull(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (comparer.Equals(item, target))
					return true;
				i++;
			}
			return false;
		}
	}

	public static bool Contains<T>(this G.IEnumerable<T> source, T target, Func<T, T, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(equalFunction);
		var comparer = new EComparer<T>(equalFunction);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (comparer.Equals(item, target))
					return true;
				i++;
			}
			return false;
		}
	}

	public static bool Contains<T>(this G.IEnumerable<T> source, T target, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(equalFunction);
		ArgumentNullException.ThrowIfNull(hashCodeFunction);
		var comparer = new EComparer<T>(equalFunction, hashCodeFunction);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return true;
			}
			return false;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (comparer.Equals(item, target))
					return true;
				i++;
			}
			return false;
		}
	}

	public static int Count<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var n = 0;
			for (var i = 0; i < length; i++)
				if (function(list[i]))
					n++;
			return n;
		}
		else if (source is T[] array)
		{
			var n = 0;
			for (var i = 0; i < array.Length; i++)
				if (function(array[i]))
					n++;
			return n;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var n = 0;
			for (var i = 0; i < length; i++)
				if (function(list2[i]))
					n++;
			return n;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var n = 0;
			for (var i = 0; i < length; i++)
				if (function(list3[i]))
					n++;
			return n;
		}
		else
		{
			var n = 0;
			foreach (var item in source)
				if (function(item))
					n++;
			return n;
		}
	}

	public static int Count<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var n = 0;
			for (var i = 0; i < length; i++)
				if (function(list[i], i))
					n++;
			return n;
		}
		else if (source is T[] array)
		{
			var n = 0;
			for (var i = 0; i < array.Length; i++)
				if (function(array[i], i))
					n++;
			return n;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var n = 0;
			for (var i = 0; i < length; i++)
				if (function(list2[i], i))
					n++;
			return n;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var n = 0;
			for (var i = 0; i < length; i++)
				if (function(list3[i], i))
					n++;
			return n;
		}
		else
		{
			var n = 0;
			var i = 0;
			foreach (var item in source)
				if (function(item, i++))
					n++;
			return n;
		}
	}

	public static int Count<T>(this G.IEnumerable<T> source, T target)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			var n = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (item?.Equals(target) ?? false)
					n++;
			}
			return n;
		}
		else if (source is T[] array)
		{
			var n = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (item?.Equals(target) ?? false)
					n++;
			}
			return n;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var n = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (item?.Equals(target) ?? false)
					n++;
			}
			return n;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var n = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (item?.Equals(target) ?? false)
					n++;
			}
			return n;
		}
		else
		{
			var n = 0;
			var i = 0;
			foreach (var item in source)
			{
				if (item?.Equals(target) ?? false)
					n++;
				i++;
			}
			return n;
		}
	}

	public static bool Equals<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			if (list.Length != list2.Length)
				return false;
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			if (array.Length != array2.Length)
				return false;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			if (list2_.Count != list2_2.Count)
				return false;
			var length = list2_.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			if (list3_.Count != list3_2.Count)
				return false;
			var length = list3_.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else
		{
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			var i = 0;
			while (en.MoveNext())
			{
				if (!en2.MoveNext())
					return false;
				var item = en.Current;
				var item2 = en2.Current;
				if (!function(item, item2, i))
					return false;
				i++;
			}
			return !en2.MoveNext();
		}
	}

	public static T? Find<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (function(item))
					return item;
				i++;
			}
			return default;
		}
	}

	public static T? Find<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (function(item, i))
					return item;
				i++;
			}
			return default;
		}
	}

	public static List<T> FindAll<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item))
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
				if (function(item))
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
				if (function(item))
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
				if (function(item))
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
				if (function(item))
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<T> FindAll<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item, i))
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
				if (function(item, i))
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
				if (function(item, i))
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
				if (function(item, i))
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
				if (function(item, i))
					result.Add(item);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static int FindIndex<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
				if (function(list[i]))
					return i;
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
				if (function(array[i]))
					return i;
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
				if (function(list2[i]))
					return i;
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
				if (function(list3[i]))
					return i;
			return -1;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (function(item))
					return i;
				i++;
			}
			return -1;
		}
	}

	public static int FindIndex<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (function(item, i))
					return i;
				i++;
			}
			return -1;
		}
	}

	public static T? FindLast<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (function(item))
					return item;
			}
			return default;
		}
		else
			return FindLast(new List<T>(source), function);
	}

	public static T? FindLast<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (function(item, i))
					return item;
			}
			return default;
		}
		else
			return FindLast(new List<T>(source), function);
	}

	public static int FindLastIndex<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
				if (function(list[i]))
					return i;
			return -1;
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			for (var i = length - 1; i >= 0; i--)
				if (function(array[i]))
					return i;
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
				if (function(list2[i]))
					return i;
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
				if (function(list3[i]))
					return i;
			return -1;
		}
		else
			return FindLastIndex(new List<T>(source), function);
	}

	public static int FindLastIndex<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (function(item, i))
					return i;
			}
			return -1;
		}
		else
			return FindLastIndex(new List<T>(source), function);
	}

	public static void ForEach<T>(this G.IEnumerable<T> source, Action<T> action)
	{
		ArgumentNullException.ThrowIfNull(action);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
				action(list[i]);
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
				action(array[i]);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
				action(list2[i]);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
				action(list3[i]);
		}
		else
		{
			foreach (var item in source)
				action(item);
		}
	}

	public static void ForEach<T>(this G.IEnumerable<T> source, Action<T, int> action)
	{
		ArgumentNullException.ThrowIfNull(action);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
				action(list[i], i);
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
				action(array[i], i);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
				action(list2[i], i);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
				action(list3[i], i);
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				action(item, i);
				i++;
			}
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this G.IEnumerable<T> source)
	{
		ListHashSet<T> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(T Key, int Count)> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(T Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(T Key, int Count)> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(T Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(T Key, int Count)> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(T Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(TResult Key, int Count)> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(TResult Key, int Count)> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(TResult Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<(T Key, int Count)> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<(T Key, int Count)> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<(T Key, int Count)> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index] = (f, result[index].Count + 1);
				else
					result.Add((f, 1));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, T>> Group<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, T>> Group<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<T, T>> Group<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<T, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<T, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, T>> GroupIndexes<T>(this G.IEnumerable<T> source)
	{
		ListHashSet<T> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, T>> GroupIndexes<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, T>> GroupIndexes<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<Group<int, T>> GroupIndexes<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<Group<int, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<Group<int, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<Group<int, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(i);
				else
					result.Add(new(32, i, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static int IndexOf<T>(this G.IEnumerable<T> source, T target)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (item?.Equals(target) ?? false)
					return i;
				i++;
			}
			return -1;
		}
	}

	public static int IndexOf<T>(this G.IEnumerable<T> source, T target, G.IEqualityComparer<T> comparer)
	{
		ArgumentNullException.ThrowIfNull(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (comparer.Equals(item, target))
					return i;
				i++;
			}
			return -1;
		}
	}

	public static int IndexOf<T>(this G.IEnumerable<T> source, T target, Func<T, T, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(equalFunction);
		var comparer = new EComparer<T>(equalFunction);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (comparer.Equals(item, target))
					return i;
				i++;
			}
			return -1;
		}
	}

	public static int IndexOf<T>(this G.IEnumerable<T> source, T target, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(equalFunction);
		ArgumentNullException.ThrowIfNull(hashCodeFunction);
		var comparer = new EComparer<T>(equalFunction, hashCodeFunction);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else
		{
			var i = 0;
			foreach (var item in source)
			{
				if (comparer.Equals(item, target))
					return i;
				i++;
			}
			return -1;
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
			return IndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Mean());
		}
	}

	public static int IndexOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Mean());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Mean());
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
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
			return IndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
		else
		{
			var list_ = source.ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Mean());
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Mean());
		}
	}

	public static int IndexOfMean(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Mean());
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Mean());
		}
	}

	public static int IndexOfMean(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var value = (int)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return IndexOf(list, value);
		}
		else if (source is int[] array)
		{
			var value = (int)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return IndexOf(array, value);
		}
		else if (source is G.IList<int> list2)
		{
			var value = (int)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return IndexOf(list2, value);
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var value = (uint)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return IndexOf(list, value);
		}
		else if (source is uint[] array)
		{
			var value = (uint)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return IndexOf(array, value);
		}
		else if (source is G.IList<uint> list2)
		{
			var value = (uint)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return IndexOf(list2, value);
		}
		else
		{
			var list_ = NList<uint>.ReturnOrConstruct(source);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var value = (long)(list.Sum(x => (MpzT)x) / Max(list.Length, 1));
			return IndexOf(list, value);
		}
		else if (source is long[] array)
		{
			var value = (long)(array.Sum(x => (MpzT)x) / Max(array.Length, 1));
			return IndexOf(array, value);
		}
		else if (source is G.IList<long> list2)
		{
			var value = (long)(list2.Sum(x => (MpzT)x) / Max(list2.Count, 1));
			return IndexOf(list2, value);
		}
		else
		{
			var list_ = List<long>.ReturnOrConstruct(source);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexOf(list_, value);
		}
	}

	public static int IndexOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var value = list.Sum() / Max(list.Length, 1);
			return IndexOf(list, value);
		}
		else if (source is MpzT[] array)
		{
			var value = array.Sum() / Max(array.Length, 1);
			return IndexOf(array, value);
		}
		else if (source is G.IList<MpzT> list2)
		{
			var value = list2.Sum() / Max(list2.Count, 1);
			return IndexOf(list2, value);
		}
		else
		{
			var list_ = List<MpzT>.ReturnOrConstruct(source);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return IndexOf(list_, value);
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = list.ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = source.ToList(function);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var list_ = NList<int>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = NList<int>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = NList<int>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = NList<uint>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<uint>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var list_ = NList<long>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = NList<long>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = NList<long>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<long>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Median());
		}
	}

	public static int IndexOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list);
			return IndexOf(list_, list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(array);
			return IndexOf(list_, list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list2);
			return IndexOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(source);
			return IndexOf(list_, list_.Median());
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

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<List<T>> source)
	{
		if (source is List<List<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is List<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<List<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<TResult> JoinIntoSingle<TSource, TResult>(this G.IEnumerable<TSource> source) where TSource : G.IEnumerable<TResult>
	{
		if (source is List<TSource> list)
		{
			var length = list.Length;
			List<TResult> result = new(1024);
			for (var i = 0; i < length; i++)
				result.AddRange(list[i]);
			return result;
		}
		else if (source is TSource[] array)
		{
			List<TResult> result = new(1024);
			for (var i = 0; i < array.Length; i++)
				result.AddRange(array[i]);
			return result;
		}
		else if (source is G.IList<TSource> list2)
		{
			var length = list2.Count;
			List<TResult> result = new(1024);
			for (var i = 0; i < length; i++)
				result.AddRange(list2[i]);
			return result;
		}
		else
		{
			List<TResult> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<T[]> source)
	{
		if (source is List<T[]> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is T[][] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<T[]> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<G.IList<T>> source)
	{
		if (source is List<G.IList<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<G.IList<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<G.IEnumerable<T>> source)
	{
		if (source is List<G.IEnumerable<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IEnumerable<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<G.IEnumerable<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<List<T>> source, T separator)
	{
		if (source is List<List<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is List<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<List<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.Add(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<TResult> JoinIntoSingle<TSource, TResult>(this G.IEnumerable<TSource> source, TResult separator) where TSource : G.IEnumerable<TResult>
	{
		if (source is List<TSource> list)
		{
			var length = list.Length;
			List<TResult> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is TSource[] array)
		{
			List<TResult> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<TSource> list2)
		{
			var length = list2.Count;
			List<TResult> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<TResult> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.Add(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<T[]> source, T separator)
	{
		if (source is List<T[]> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is T[][] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<T[]> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.Add(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<G.IList<T>> source, T separator)
	{
		if (source is List<G.IList<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<G.IList<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.Add(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<G.IEnumerable<T>> source, T separator)
	{
		if (source is List<G.IEnumerable<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IEnumerable<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<G.IEnumerable<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.Add(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.Add(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<List<T>> source, G.IEnumerable<T> separator)
	{
		if (source is List<List<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is List<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<List<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.AddRange(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<TResult> JoinIntoSingle<TSource, TResult>(this G.IEnumerable<TSource> source, G.IEnumerable<TResult> separator) where TSource : G.IEnumerable<TResult>
	{
		if (source is List<TSource> list)
		{
			var length = list.Length;
			List<TResult> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is TSource[] array)
		{
			List<TResult> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<TSource> list2)
		{
			var length = list2.Count;
			List<TResult> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<TResult> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.AddRange(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<T[]> source, G.IEnumerable<T> separator)
	{
		if (source is List<T[]> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is T[][] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<T[]> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.AddRange(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<G.IList<T>> source, G.IEnumerable<T> separator)
	{
		if (source is List<G.IList<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<G.IList<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.AddRange(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static List<T> JoinIntoSingle<T>(this G.IEnumerable<G.IEnumerable<T>> source, G.IEnumerable<T> separator)
	{
		if (source is List<G.IEnumerable<T>> list)
		{
			var length = list.Length;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IEnumerable<T>[] array)
		{
			List<T> result = new(1024);
			for (var i = 0; i < array.Length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = array[i];
				result.AddRange(item);
			}
			return result;
		}
		else if (source is G.IList<G.IEnumerable<T>> list2)
		{
			var length = list2.Count;
			List<T> result = new(1024);
			for (var i = 0; i < length; i++)
			{
				if (i != 0)
					result.AddRange(separator);
				var item = list2[i];
				result.AddRange(item);
			}
			return result;
		}
		else
		{
			List<T> result = new(1024);
			var i = 0;
			foreach (var item in source)
			{
				if (i != 0)
					result.AddRange(separator);
				result.AddRange(item);
				i++;
			}
			return result;
		}
	}

	public static int LastIndexOf<T>(this G.IEnumerable<T> source, T target)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (item?.Equals(target) ?? false)
					return i;
			}
			return -1;
		}
		else
			return LastIndexOf(new List<T>(source), target);
	}

	public static int LastIndexOf<T>(this G.IEnumerable<T> source, T target, G.IEqualityComparer<T> comparer)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = array.Length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else
			return LastIndexOf([.. source], target, comparer);
	}

	public static int LastIndexOf<T>(this G.IEnumerable<T> source, T target, Func<T, T, bool> equalFunction)
	{
		var comparer = new EComparer<T>(equalFunction);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = array.Length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else
			return LastIndexOf([.. source], target, equalFunction);
	}

	public static int LastIndexOf<T>(this G.IEnumerable<T> source, T target, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		var comparer = new EComparer<T>(equalFunction, hashCodeFunction);
		if (source is List<T> list)
		{
			var length = list.Length;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is T[] array)
		{
			for (var i = array.Length - 1; i >= 0; i--)
			{
				var item = array[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list2[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			for (var i = length - 1; i >= 0; i--)
			{
				var item = list3[i];
				if (comparer.Equals(item, target))
					return i;
			}
			return -1;
		}
		else
			return LastIndexOf([.. source], target, equalFunction, hashCodeFunction);
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
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Mean());
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
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Mean());
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
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Mean());
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
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Mean());
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return LastIndexOf(list_, value);
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
			return LastIndexOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return LastIndexOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return LastIndexOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return -1;
			return LastIndexOf(list_, value);
		}
		else
			return LastIndexOfMean(new List<T>(source), function);
	}

	public static int LastIndexOfMean(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Mean());
		}
		else
			return LastIndexOfMean(new NList<decimal>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Mean());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Mean());
		}
		else
			return LastIndexOfMean(new NList<double>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var value = (int)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return LastIndexOf(list, value);
		}
		else if (source is int[] array)
		{
			var value = (int)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return LastIndexOf(array, value);
		}
		else if (source is G.IList<int> list2)
		{
			var value = (int)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return LastIndexOf(list2, value);
		}
		else
			return LastIndexOfMean(new NList<int>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var value = (uint)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return LastIndexOf(list, value);
		}
		else if (source is uint[] array)
		{
			var value = (uint)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return LastIndexOf(array, value);
		}
		else if (source is G.IList<uint> list2)
		{
			var value = (uint)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return LastIndexOf(list2, value);
		}
		else
			return LastIndexOfMean(new NList<uint>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var value = (long)(list.Sum(x => (MpzT)x) / Max(list.Length, 1));
			return LastIndexOf(list, value);
		}
		else if (source is long[] array)
		{
			var value = (long)(array.Sum(x => (MpzT)x) / Max(array.Length, 1));
			return LastIndexOf(array, value);
		}
		else if (source is G.IList<long> list2)
		{
			var value = (long)(list2.Sum(x => (MpzT)x) / Max(list2.Count, 1));
			return LastIndexOf(list2, value);
		}
		else
			return LastIndexOfMean(new NList<long>(source));
	}

	public static int LastIndexOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var value = list.Sum() / Max(list.Length, 1);
			return LastIndexOf(list, value);
		}
		else if (source is MpzT[] array)
		{
			var value = array.Sum() / Max(array.Length, 1);
			return LastIndexOf(array, value);
		}
		else if (source is G.IList<MpzT> list2)
		{
			var value = list2.Sum() / Max(list2.Count, 1);
			return LastIndexOf(list2, value);
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
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
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = array.AsSpan().ToList(function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return LastIndexOf(list_, list_.Median());
		}
		else
			return LastIndexOfMedian(new List<T>(source), function);
	}

	public static int LastIndexOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<decimal>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<double>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var list_ = NList<int>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = NList<int>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = NList<int>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<int>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = NList<uint>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<uint>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var list_ = NList<long>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = NList<long>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = NList<long>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Median());
		}
		else
			return LastIndexOfMedian(new NList<long>(source));
	}

	public static int LastIndexOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(array);
			return LastIndexOf(list_, list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list2);
			return LastIndexOf(list_, list_.Median());
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

	public static Slice<TResult> Pairs<T, TResult>(this G.IReadOnlyList<T> source, Func<T, T, TResult> function, int offset = 1)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source.Count < offset)
			return [];
		return new CombineList<T, T, TResult>(source, source.GetROLSlice(offset), function).GetSlice();
	}

	public static Slice<TResult> Pairs<T, TResult>(this G.IReadOnlyList<T> source, Func<T, T, int, TResult> function, int offset = 1)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source.Count < offset)
			return [];
		return new CombineListInt<T, T, TResult>(source, source.GetROLSlice(offset), function).GetSlice();
	}

	public static Slice<(T, T)> Pairs<T>(this G.IReadOnlyList<T> source, int offset = 1)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source.Count < offset)
			return [];
		return new CombineListPure<T, T>(source, source.GetROLSlice(offset)).GetSlice();
	}

	public static G.IEnumerable<TResult> Pairs<T, TResult>(this G.IEnumerable<T> source, Func<T, T, TResult> function, int offset = 1)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source is List<T> list)
		{
			var length = list.Length - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list[i + offset];
				yield return function(item, item2);
			}
		}
		else if (source is T[] array)
		{
			var length = array.Length - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array[i + offset];
				yield return function(item, item2);
			}
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				var item2 = list2[i + offset];
				yield return function(item, item2);
			}
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				var item2 = list3[i + offset];
				yield return function(item, item2);
			}
		}
		else
		{
			using var en = source.GetEnumerator();
			using LimitedQueue<T> queue = new(offset);
			while (!queue.IsFull && en.MoveNext())
				queue.Enqueue(en.Current);
			var i = 0;
			while (en.MoveNext())
			{
				var item = queue.Dequeue();
				var item2 = en.Current;
				yield return function(item, item2);
				queue.Enqueue(item2);
				i++;
			}
		}
	}

	public static G.IEnumerable<TResult> Pairs<T, TResult>(this G.IEnumerable<T> source, Func<T, T, int, TResult> function, int offset = 1)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source is List<T> list)
		{
			var length = list.Length - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list[i + offset];
				yield return function(item, item2, i);
			}
		}
		else if (source is T[] array)
		{
			var length = array.Length - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array[i + offset];
				yield return function(item, item2, i);
			}
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				var item2 = list2[i + offset];
				yield return function(item, item2, i);
			}
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				var item2 = list3[i + offset];
				yield return function(item, item2, i);
			}
		}
		else
		{
			using var en = source.GetEnumerator();
			using LimitedQueue<T> queue = new(offset);
			while (!queue.IsFull && en.MoveNext())
				queue.Enqueue(en.Current);
			var i = 0;
			while (en.MoveNext())
			{
				var item = queue.Dequeue();
				var item2 = en.Current;
				yield return function(item, item2, i);
				queue.Enqueue(item2);
				i++;
			}
		}
	}

	public static G.IEnumerable<(T, T)> Pairs<T>(this G.IEnumerable<T> source, int offset = 1)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source is List<T> list)
		{
			var length = list.Length - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list[i + offset];
				yield return (item, item2);
			}
		}
		else if (source is T[] array)
		{
			var length = array.Length - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array[i + offset];
				yield return (item, item2);
			}
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				var item2 = list2[i + offset];
				yield return (item, item2);
			}
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count - offset;
			if (length <= 0)
				yield break;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				var item2 = list3[i + offset];
				yield return (item, item2);
			}
		}
		else
		{
			using var en = source.GetEnumerator();
			using LimitedQueue<T> queue = new(offset);
			while (!queue.IsFull && en.MoveNext())
				queue.Enqueue(en.Current);
			var i = 0;
			while (en.MoveNext())
			{
				var item = queue.Dequeue();
				var item2 = en.Current;
				yield return (item, item2);
				queue.Enqueue(item2);
				i++;
			}
		}
	}

	private class PrependList<T> : BaseIndexable<T, PrependList<T>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly T element;

		public PrependList()
		{
			source = [];
			element = default!;
		}

		public PrependList(G.IReadOnlyList<T> source, T element)
		{
			this.source = source;
			this.element = element;
			_size = source.Count + 1;
		}

		public override Span<T> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override T GetInternal(int index, bool invoke = true) => index == 0 ? element : source[index - 1];

		protected override PrependList<T> GetRangeInternal(int index, int length) => index == 0 ? new(source.GetROLSlice(0, length - 1), element) : new(source.GetROLSlice(index, length - 1), source[index - 1]);

		protected override Slice<T> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(T item, int index, int length)
		{
			if (index == 0 && (element?.Equals(item) ?? item == null))
				return 0;
			for (var i = Max(index, 1); i < index + length; i++)
				if (source[i - 1]?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(T item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= Max(endIndex, 1); i++)
				if (source[i - 1]?.Equals(item) ?? item == null)
					return i;
			if (endIndex == 0 && (element?.Equals(item) ?? item == null))
				return 0;
			return -1;
		}
	}

	public static Slice<T> Prepend<T>(this G.IReadOnlyList<T> source, T element) => new PrependList<T>(source, element).GetROLSlice();

	public static T? Progression<T>(this G.IEnumerable<T> source, Func<T, T, T> function)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			T? result = default;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result = result == null || i == 0 ? item : function(result, item);
			}
			return result;
		}
		else if (source is T[] array)
		{
			T? result = default;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result = result == null || i == 0 ? item : function(result, item);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			T? result = default;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result = result == null || i == 0 ? item : function(result, item);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			T? result = default;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result = result == null || i == 0 ? item : function(result, item);
			}
			return result;
		}
		else
		{
			T? result = default;
			var i = 0;
			foreach (var item in source)
			{
				result = result == null || i == 0 ? item : function(result, item);
				i++;
			}
			return result;
		}
	}

	public static TResult? Progression<T, TResult>(this G.IEnumerable<T> source, TResult seed, Func<TResult, T, TResult> function)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = seed;
			for (var i = 0; i < length; i++)
				result = function(result, list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			var result = seed;
			for (var i = 0; i < array.Length; i++)
				result = function(result, array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = seed;
			for (var i = 0; i < length; i++)
				result = function(result, list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = seed;
			for (var i = 0; i < length; i++)
				result = function(result, list3[i]);
			return result;
		}
		else
		{
			var result = seed;
			var i = 0;
			foreach (var item in source)
			{
				result = function(result, item);
				i++;
			}
			return result;
		}
	}

	public static NList<int> RepresentIntoNumbers<T>(this G.IEnumerable<T> source)
	{
		ListHashSet<T> dic = [];
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var j = 0;
			var i = 0;
			foreach (var item in source)
			{
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
				i++;
			}
			return result;
		}
	}

	public static NList<int> RepresentIntoNumbers<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var j = 0;
			var i = 0;
			foreach (var item in source)
			{
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
				i++;
			}
			return result;
		}
	}

	public static NList<int> RepresentIntoNumbers<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var j = 0;
			var i = 0;
			foreach (var item in source)
			{
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
				i++;
			}
			return result;
		}
	}

	public static NList<int> RepresentIntoNumbers<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<int> result = new(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
			}
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var j = 0;
			var i = 0;
			foreach (var item in source)
			{
				result.Add(!dic.TryAdd(item, out var index) ? index : j++);
				i++;
			}
			return result;
		}
	}

	private class ReverseList<T> : BaseIndexable<T, ReverseList<T>>
	{
		private readonly G.IReadOnlyList<T> source;

		public ReverseList() => source = [];

		public ReverseList(G.IReadOnlyList<T> source)
		{
			ArgumentNullException.ThrowIfNull(source);
			this.source = source;
			_size = source.Count;
		}

		public override Span<T> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, T[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override T GetInternal(int index, bool invoke = true) => source[_size - 1 - index];

		protected override ReverseList<T> GetRangeInternal(int index, int length) => new(source.GetROLSlice(_size - index - length, length));

		protected override Slice<T> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(T item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (source[_size - 1 - index]?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(T item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (source[_size - 1 - index]?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	public static Slice<T> Reverse<T>(this G.IReadOnlyList<T> source) => new ReverseList<T>(source).GetSlice();

	public static G.IList<T> SetAll<T>(this G.IList<T> source, T value)
	{
		if (source is List<T> list)
			return list.SetAll(value);
		else
		{
			for (var i = 0; i < source.Count; i++)
				source[i] = value;
			return source;
		}
	}

	public static G.IList<T> SetAll<T>(this G.IList<T> source, T value, Index index)
	{
		if (source is List<T> list)
			return list.SetAll(value, index);
		else
		{
			for (var i = index.GetOffset(source.Count); i < source.Count; i++)
				source[i] = value;
			return source;
		}
	}

	public static G.IList<T> SetAll<T>(this G.IList<T> source, T value, int index)
	{
		if (source is List<T> list)
			return list.SetAll(value, index);
		else
		{
			for (var i = index; i < source.Count; i++)
				source[i] = value;
			return source;
		}
	}

	public static G.IList<T> SetAll<T>(this G.IList<T> source, T value, int index, int length)
	{
		if (source is List<T> list)
			return list.SetAll(value, index, length);
		else
		{
			var endIndex = index + length;
			for (var i = index; i < endIndex; i++)
				source[i] = value;
			return source;
		}
	}

	public static G.IList<T> SetAll<T>(this G.IList<T> source, T value, Range range)
	{
		if (source is List<T> list)
			return list.SetAll(value, range);
		else
		{
			var (startIndex, length) = range.GetOffsetAndLength(source.Count);
			for (var i = startIndex; i < startIndex + length; i++)
				source[i] = value;
			return source;
		}
	}

	public static List<TResult> SetInnerType<T, TResult>(this IEnumerable source)
	{
		List<TResult> result = [];
		foreach (var item in source)
			result.Add((TResult)item);
		return result;
	}

	public static List<TResult> SetInnerType<T, TResult>(this IEnumerable source, Func<object?, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is ArrayList list)
		{
			var length = list.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list[i]);
			return result;
		}
		else if (source is BitArray bitArray)
		{
			List<TResult> result = new(bitArray.Length);
			for (var i = 0; i < bitArray.Length; i++)
				result[i] = function(bitArray[i]);
			return result;
		}
		else if (source is System.Collections.IList list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list2[i]);
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.EmptyList<TResult>(length);
			var i = 0;
			foreach (var item in source)
			{
				result[i] = function(item);
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			List<TResult> result = [];
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static List<TResult> SetInnerType<T, TResult>(this IEnumerable source, Func<object?, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is ArrayList list)
		{
			var length = list.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list[i], i);
			return result;
		}
		else if (source is BitArray bitArray)
		{
			var result = RedStarLinq.EmptyList<TResult>(bitArray.Length);
			for (var i = 0; i < bitArray.Length; i++)
				result[i] = function(bitArray[i], i);
			return result;
		}
		else if (source is System.Collections.IList list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list2[i], i);
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.EmptyList<TResult>(length);
			var i = 0;
			foreach (var item in source)
			{
				result[i] = function(item, i);
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			List<TResult> result = [];
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static List<TResult> Shuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Random random)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list[0]);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.EmptyList<TResult>(array.Length);
			if (array.Length == 0)
				return result;
			result[0] = function(array[0]);
			int blend;
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list2[0]);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list3[0]);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			List<TResult> result = new(1024);
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result.Add(function(item));
				else
				{
					result.Add(result[blend]);
					result[blend] = function(item);
				}
				i++;
			}
			return result;
		}
	}

	public static List<TResult> Shuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Random random)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list[0], 0);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.EmptyList<TResult>(array.Length);
			if (array.Length == 0)
				return result;
			result[0] = function(array[0], 0);
			int blend;
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list2[0], 0);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list3[0], 0);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.EmptyList<TResult>(length);
			if (length == 0)
				return result;
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			List<TResult> result = new(1024);
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result.Add(function(item, i));
				else
				{
					result.Add(result[blend]);
					result[blend] = function(item, i);
				}
				i++;
			}
			return result;
		}
	}

	public static List<T> Shuffle<T>(this G.IEnumerable<T> source, Random random)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.EmptyList<T>(length);
			if (length == 0)
				return result;
			result[0] = list[0];
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.EmptyList<T>(array.Length);
			if (array.Length == 0)
				return result;
			result[0] = array[0];
			int blend;
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.EmptyList<T>(length);
			if (length == 0)
				return result;
			result[0] = list2[0];
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.EmptyList<T>(length);
			if (length == 0)
				return result;
			result[0] = list3[0];
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.EmptyList<T>(length);
			if (length == 0)
				return result;
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			List<T> result = new(1024);
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result.Add(item);
				else
				{
					result.Add(result[blend]);
					result[blend] = item;
				}
				i++;
			}
			return result;
		}
	}

	public static Slice<T> Skip<T>(this G.IEnumerable<T> source, int length)
	{
		if (source is G.IList<T> list)
			return list.GetSlice(Clamp(length, 0, list.Count));
		else if (length <= 0)
			return new((G.IReadOnlyList<T>)source.ToList());
		else
		{
			using var en = source.GetEnumerator();
			for (var i = 0; i < length; i++)
				if (!en.MoveNext())
					return new();
			List<T> result = new(source.TryGetLengthEasily(out var length2) ? Max(length2 - length, 0) : 1024);
			while (en.MoveNext())
				result.Add(en.Current);
			return result.GetSlice();
		}
	}

	public static Slice<T> SkipLast<T>(this G.IEnumerable<T> source, int length)
	{
		if (source is G.IList<T> list)
			return list.GetSlice(0, Clamp(list.Count - length, 0, list.Count));
		else if (length <= 0)
			return new((G.IReadOnlyList<T>)source.ToList());
		else if (source.TryGetLengthEasily(out var length2))
		{
			var end = Max(length2 - length, 0);
			var result = RedStarLinq.EmptyList<T>(end);
			using var en = source.GetEnumerator();
			var i = 0;
			for (; i < end && en.MoveNext(); i++)
				result[i] = en.Current;
			result.Resize(i);
			return result.GetSlice();
		}
		else
		{
			List<T> result = [];
			using LimitedQueue<T> queue = new(length);
			using var en = source.GetEnumerator();
			while (en.MoveNext())
				queue.Enqueue(en.Current, result);
			return result.GetSlice();
		}
	}

	public static Slice<T> SkipWhile<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		if (source is List<T> list)
			return list.SkipWhile(function);
		else
		{
			List<T> result = [];
			using var en = source.GetEnumerator();
			var i = 0;
			for (; en.MoveNext() && function(en.Current); i++) ;
			for (; en.MoveNext(); i++) result.Add(en.Current);
			return result.GetSlice();
		}
	}

	public static Slice<T> SkipWhile<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		if (source is List<T> list)
			return list.SkipWhile(function);
		else
		{
			List<T> result = [];
			using var en = source.GetEnumerator();
			var i = 0;
			for (; en.MoveNext() && function(en.Current, i); i++) ;
			for (; en.MoveNext(); i++) result.Add(en.Current);
			return result.GetSlice();
		}
	}

	public static List<List<T>> SplitIntoEqual<T>(this G.IEnumerable<T> source, int fragmentLength)
	{
		if (fragmentLength <= 0)
			throw new ArgumentException("Длина фрагмента должна быть положительной.", nameof(fragmentLength));
		if (source is List<T> list)
		{
			var length = GetArrayLength(list.Length, fragmentLength);
			var result = RedStarLinq.EmptyList<List<T>>(length);
			var length2 = list.Length / fragmentLength;
			var index = 0;
			for (var i = 0; i < length2; i++)
			{
				result[i] = RedStarLinq.EmptyList<T>(fragmentLength);
				for (var j = 0; j < fragmentLength; j++)
					result[i][j] = list[index++];
			}
			var rest = list.Length % fragmentLength;
			if (rest != 0)
			{
				result[length2] = RedStarLinq.EmptyList<T>(rest);
				for (var j = 0; j < rest; j++)
					result[length2][j] = list[index++];
			}
			return result;
		}
		else if (source is T[] array)
		{
			var length = GetArrayLength(array.Length, fragmentLength);
			var result = RedStarLinq.EmptyList<List<T>>(length);
			var length2 = array.Length / fragmentLength;
			var index = 0;
			for (var i = 0; i < length2; i++)
			{
				result[i] = RedStarLinq.EmptyList<T>(fragmentLength);
				for (var j = 0; j < fragmentLength; j++)
					result[i][j] = array[index++];
			}
			var rest = array.Length % fragmentLength;
			if (rest != 0)
			{
				result[length2] = RedStarLinq.EmptyList<T>(rest);
				for (var j = 0; j < rest; j++)
					result[length2][j] = array[index++];
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = GetArrayLength(list2.Count, fragmentLength);
			var result = RedStarLinq.EmptyList<List<T>>(length);
			var length2 = list2.Count / fragmentLength;
			var index = 0;
			for (var i = 0; i < length2; i++)
			{
				result[i] = RedStarLinq.EmptyList<T>(fragmentLength);
				for (var j = 0; j < fragmentLength; j++)
					result[i][j] = list2[index++];
			}
			var rest = list2.Count % fragmentLength;
			if (rest != 0)
			{
				result[length2] = RedStarLinq.EmptyList<T>(rest);
				for (var j = 0; j < rest; j++)
					result[length2][j] = list2[index++];
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = GetArrayLength(list3.Count, fragmentLength);
			var result = RedStarLinq.EmptyList<List<T>>(length);
			var length2 = list3.Count / fragmentLength;
			var index = 0;
			for (var i = 0; i < length2; i++)
			{
				result[i] = RedStarLinq.EmptyList<T>(fragmentLength);
				for (var j = 0; j < fragmentLength; j++)
					result[i][j] = list3[index++];
			}
			var rest = list3.Count % fragmentLength;
			if (rest != 0)
			{
				result[length2] = RedStarLinq.EmptyList<T>(rest);
				for (var j = 0; j < rest; j++)
					result[length2][j] = list3[index++];
			}
			return result;
		}
		else
		{
			if (!source.Any())
				return [];
			List<List<T>> result = new(64);
			result.Add(new(fragmentLength));
			foreach (var item in source)
			{
				if (result[^1].Length >= fragmentLength)
					result.Add(new(fragmentLength));
				result[^1].Add(item);
			}
			return result;
		}
	}

	public static bool StartsWith<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			if (list.Length < list2.Length)
				return false;
			var length = list2.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				if (!function(item, item2))
					return false;
			}
			return true;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			if (array.Length < array2.Length)
				return false;
			for (var i = 0; i < array2.Length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				if (!function(item, item2))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			if (list2_.Count < list2_2.Count)
				return false;
			var length = list2_2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (!function(item, item2))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			if (list3_.Count < list3_2.Count)
				return false;
			var length = list3_2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (!function(item, item2))
					return false;
			}
			return true;
		}
		else
		{
			bool b2;
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			while (en.MoveNext() & (b2 = en2.MoveNext()))
			{
				var item = en.Current;
				var item2 = en2.Current;
				if (!function(item, item2))
					return false;
			}
			return !b2;
		}
	}

	public static bool StartsWith<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			if (list.Length < list2.Length)
				return false;
			var length = list2.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			if (array.Length < array2.Length)
				return false;
			for (var i = 0; i < array2.Length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			if (list2_.Count < list2_2.Count)
				return false;
			var length = list2_2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			if (list3_.Count < list3_2.Count)
				return false;
			var length = list3_2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (!function(item, item2, i))
					return false;
			}
			return true;
		}
		else
		{
			bool b2;
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			var i = 0;
			while (en.MoveNext() & (b2 = en2.MoveNext()))
			{
				var item = en.Current;
				var item2 = en2.Current;
				if (!function(item, item2, i))
					return false;
				i++;
			}
			return !b2;
		}
	}

	public static bool StartsWith<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2)
	{
		if (source is List<T> list && source2 is List<T2> list2)
		{
			if (list.Length < list2.Length)
				return false;
			var length = list2.Length;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				if (!(item?.Equals(item2) ?? item2 == null))
					return false;
			}
			return true;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			if (array.Length < array2.Length)
				return false;
			for (var i = 0; i < array2.Length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				if (!(item?.Equals(item2) ?? item2 == null))
					return false;
			}
			return true;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			if (list2_.Count < list2_2.Count)
				return false;
			var length = list2_2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				if (!(item?.Equals(item2) ?? item2 == null))
					return false;
			}
			return true;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			if (list3_.Count < list3_2.Count)
				return false;
			var length = list3_2.Count;
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				if (!(item?.Equals(item2) ?? item2 == null))
					return false;
			}
			return true;
		}
		else
		{
			bool b2;
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			while (en.MoveNext() & (b2 = en2.MoveNext()))
			{
				var item = en.Current;
				var item2 = en2.Current;
				if (!(item?.Equals(item2) ?? item2 == null))
					return false;
			}
			return !b2;
		}
	}

	public static Slice<T> Take<T>(this G.IEnumerable<T> source, int length)
	{
		if (length <= 0)
			return new();
		else if (source is G.IList<T> list)
			return list.GetSlice(0, Clamp(length, 0, list.Count));
		else
		{
			var result = RedStarLinq.EmptyList<T>(length);
			var i = 0;
			foreach (var item in source)
			{
				result[i++] = item;
				if (i >= length)
					break;
			}
			result.Resize(i);
			return result.GetSlice();
		}
	}

	public static Slice<T> Take<T>(this G.IEnumerable<T> source, Range range)
	{
		if (source is G.IList<T> list)
		{
			var start = Clamp(range.Start.GetOffset(list.Count), 0, list.Count);
			var end = Clamp(range.End.GetOffset(list.Count), 0, list.Count);
			return start >= end ? new() : list.GetSlice(start, end - start);
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var start = Clamp(range.Start.GetOffset(length), 0, length);
			var end = Clamp(range.End.GetOffset(length), 0, length);
			if (start >= end)
				return new();
			List<T> result = new(end - start);
			var i = 0;
			foreach (var item in source)
			{
				if (i >= start)
					result.Add(item);
				i++;
				if (i >= end)
					break;
			}
			return result.GetSlice();
		}
		int index = range.Start.Value, index2 = range.End.Value;
		using var en = source.GetEnumerator();
		if (!range.Start.IsFromEnd && !range.End.IsFromEnd)
		{
			if (index >= index2)
				return new();
			var i = 0;
			for (; i < index; i++)
				if (!en.MoveNext())
					return new();
			List<T> result = new(index2 - index);
			for (; i < index2 && en.MoveNext(); i++)
				result.Add(en.Current);
			result.Resize(i - index);
			return result.GetSlice();
		}
		else if (!range.Start.IsFromEnd && range.End.IsFromEnd)
		{
			var i = 0;
			for (; i < index; i++)
				if (!en.MoveNext())
					return new();
			List<T> result = [];
			using LimitedQueue<T> queue = new(index2);
			while (en.MoveNext())
				queue.Enqueue(en.Current, result);
			return result.GetSlice();
		}
		else if (range.Start.IsFromEnd && !range.End.IsFromEnd)
		{
			using LimitedQueue<T> queue = new(index);
			var i = 0;
			while (en.MoveNext())
			{
				queue.Enqueue(en.Current);
				i++;
				if (i >= index + index2)
					return new();
			}
			var result = RedStarLinq.EmptyList<T>(Min(index + index2 - i, i));
			for (i = 0; i < result.Length; i++)
				result[i] = queue.Dequeue();
			return result.GetSlice();
		}
		else if (range.Start.IsFromEnd && range.End.IsFromEnd)
		{
			using LimitedQueue<T> queue = new(index);
			while (en.MoveNext())
				queue.Enqueue(en.Current);
			if (queue.Length <= index2)
				return new();
			var result = RedStarLinq.EmptyList<T>(queue.Length - index2);
			for (var i = 0; i < result.Length; i++)
				result[i] = queue.Dequeue();
			return result.GetSlice();
		}
		else
			return new();
	}

	public static Slice<T> TakeLast<T>(this G.IEnumerable<T> source, int length)
	{
		if (length <= 0)
			return new();
		else if (source is G.IList<T> list)
			return list.GetSlice(Clamp(list.Count - length, 0, list.Count));
		else if (source.TryGetLengthEasily(out var length2))
		{
			var start = Max(length2 - length, 0);
			using var en = source.GetEnumerator();
			var i = 0;
			for (; i < start; i++)
				if (!en.MoveNext())
					return new();
			var result = RedStarLinq.EmptyList<T>(Min(length, length2));
			for (i = 0; i < result.Length && en.MoveNext(); i++)
				result[i] = en.Current;
			return result.GetSlice();
		}
		else
		{
			using LimitedQueue<T> queue = new(length);
			using var en = source.GetEnumerator();
			while (en.MoveNext())
				queue.Enqueue(en.Current);
			return queue.ToList().GetSlice();
		}
	}

	public static Slice<T> TakeWhile<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		if (source is List<T> list)
			return list.TakeWhile(function);
		else
		{
			List<T> result = [];
			using var en = source.GetEnumerator();
			var i = 0;
			T item;
			for (; en.MoveNext() && function(item = en.Current); i++) result.Add(item);
			return result.GetSlice();
		}
	}

	public static Slice<T> TakeWhile<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		if (source is List<T> list)
			return list.TakeWhile(function);
		else
		{
			List<T> result = [];
			using var en = source.GetEnumerator();
			var i = 0;
			T item;
			for (; en.MoveNext() && function(item = en.Current, i); i++) result.Add(item);
			return result.GetSlice();
		}
	}

	public static bool All<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			if (!function(source[i]))
				return false;
		return true;
	}

	public static bool All<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			if (!function(source[i], i))
				return false;
		return true;
	}

	public static bool Any<T>(this ReadOnlySpan<T> source)
	{
		var length = source.Length;
		return length != 0;
	}

	public static bool Any<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			if (function(source[i]))
				return true;
		return false;
	}

	public static bool Any<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			if (function(source[i], i))
				return true;
		return false;
	}

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<T, TResult2> function2)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = function(item);
			result2[i] = function2(item);
		}
		return (result, result2);
	}

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = function(item, i);
			result2[i] = function2(item, i);
		}
		return (result, result2);
	}

	public static (List<T>, List<T2>) Break<T, T2>(this ReadOnlySpan<(T, T2)> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<T>(length);
		List<T2> result2 = new(length);
		for (var i = 0; i < length; i++)
			(result[i], result2[i]) = source[i];
		return (result, result2);
	}

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, (TResult, TResult2)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			(result[i], result2[i]) = function(item);
		}
		return (result, result2);
	}

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, int, (TResult, TResult2)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			(result[i], result2[i]) = function(item, i);
		}
		return (result, result2);
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = function(item);
			result2[i] = function2(item);
			result3[i] = function3(item);
		}
		return (result, result2, result3);
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = function(item, i);
			result2[i] = function2(item, i);
			result3[i] = function3(item, i);
		}
		return (result, result2, result3);
	}

	public static (List<T>, List<T2>, List<T3>) Break<T, T2, T3>(this ReadOnlySpan<(T, T2, T3)> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		var result3 = RedStarLinq.EmptyList<T3>(length);
		for (var i = 0; i < length; i++)
			(result[i], result2[i], result3[i]) = source[i];
		return (result, result2, result3);
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, (TResult, TResult2, TResult3)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			(result[i], result2[i], result3[i]) = function(item);
		}
		return (result, result2, result3);
	}

	public static (List<TResult>, List<TResult2>, List<TResult3>) Break<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, int, (TResult, TResult2, TResult3)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			(result[i], result2[i], result3[i]) = function(item, i);
		}
		return (result, result2, result3);
	}

	public static List<T> BreakFilter<T>(this ReadOnlySpan<T> source, Func<T, bool> function, out List<T> result2)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(length / 2);
		result2 = new(length / 2);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item))
				result.Add(item);
			else
				result2.Add(item);
		}
		result.TrimExcess();
		result2.TrimExcess();
		return result;
	}

	public static List<T> BreakFilter<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function, out List<T> result2)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(length / 2);
		result2 = new(length / 2);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item, i))
				result.Add(item);
			else
				result2.Add(item);
		}
		result.TrimExcess();
		result2.TrimExcess();
		return result;
	}

	[Obsolete("Этот метод не рекомендуется, так как создает новый список, который потребляет очень много памяти (сравнимо с исходными Span<T>). Для устранения проблемы замените Span<T> на Slice<T> (создается методами GetSlice() и GetROLSlice()).")]
	public static List<TResult> Combine<T, T2, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Length, source2.Length);
		var result = RedStarLinq.EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i]);
		return result;
	}

	[Obsolete("Этот метод не рекомендуется, так как создает новый список, который потребляет очень много памяти (сравнимо с исходными Span<T>). Для устранения проблемы замените Span<T> на Slice<T> (создается методами GetSlice() и GetROLSlice()).")]
	public static List<TResult> Combine<T, T2, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Length, source2.Length);
		var result = RedStarLinq.EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], i);
		return result;
	}

	[Obsolete("Этот метод не рекомендуется, так как создает новый список, который потребляет очень много памяти (сравнимо с исходными Span<T>). Для устранения проблемы замените Span<T> на Slice<T> (создается методами GetSlice() и GetROLSlice()).")]
	public static List<(T, T2)> Combine<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2)
	{
		var length = Min(source.Length, source2.Length);
		List<(T, T2)> result = new(length);
		for (var i = 0; i < length; i++)
			result[i] = (source[i], source2[i]);
		return result;
	}

	[Obsolete("Этот метод не рекомендуется, так как создает новый список, который потребляет очень много памяти (сравнимо с исходными Span<T>). Для устранения проблемы замените Span<T> на Slice<T> (создается методами GetSlice() и GetROLSlice()).")]
	public static List<TResult> Combine<T, T2, T3, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, ReadOnlySpan<T3> source3, Func<T, T2, T3, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min(new[] { source.Length, source2.Length, source3.Length }.AsSpan());
		var result = RedStarLinq.EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], source3[i]);
		return result;
	}

	[Obsolete("Этот метод не рекомендуется, так как создает новый список, который потребляет очень много памяти (сравнимо с исходными Span<T>). Для устранения проблемы замените Span<T> на Slice<T> (создается методами GetSlice() и GetROLSlice()).")]
	public static List<TResult> Combine<T, T2, T3, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, ReadOnlySpan<T3> source3, Func<T, T2, T3, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min(new[] { source.Length, source2.Length, source3.Length }.AsSpan());
		var result = RedStarLinq.EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], source3[i], i);
		return result;
	}

	[Obsolete("Этот метод не рекомендуется, так как создает новый список, который потребляет очень много памяти (сравнимо с исходными Span<T>). Для устранения проблемы замените Span<T> на Slice<T> (создается методами GetSlice() и GetROLSlice()).")]
	public static List<(T, T2, T3)> Combine<T, T2, T3>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, ReadOnlySpan<T3> source3)
	{
		var length = RedStarLinqMath.Min(new[] { source.Length, source2.Length, source3.Length }.AsSpan());
		List<(T, T2, T3)> result = new(length);
		for (var i = 0; i < length; i++)
			result[i] = (source[i], source2[i], source3[i]);
		return result;
	}

	public static List<TResult> ConvertAndJoin<T, TResult>(this ReadOnlySpan<T> source, Func<T, G.IEnumerable<TResult>> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<TResult> result = new(1024);
		for (var i = 0; i < length; i++)
			result.AddRange(function(source[i]));
		return result;
	}

	public static List<TResult> ConvertAndJoin<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, G.IEnumerable<TResult>> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<TResult> result = new(1024);
		for (var i = 0; i < length; i++)
			result.AddRange(function(source[i], i));
		return result;
	}

	public static int Count<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = 0;
		for (var i = 0; i < length; i++)
			if (function(source[i]))
				result++;
		return result;
	}

	public static int Count<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = 0;
		for (var i = 0; i < length; i++)
			if (function(source[i], i))
				result++;
		return result;
	}

	public static int Count<T>(this ReadOnlySpan<T> source, T target)
	{
		var length = source.Length;
		var result = 0;
		for (var i = 0; i < length; i++)
			if (source[i]?.Equals(target) ?? false)
				result++;
		return result;
	}

	public static bool Equals<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source.Length != source2.Length)
			return false;
		var length = Min(source.Length, source2.Length);
		for (var i = 0; i < length; i++)
			if (!function(source[i], source2[i]))
				return false;
		return true;
	}

	public static bool Equals<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source.Length != source2.Length)
			return false;
		var length = Min(source.Length, source2.Length);
		for (var i = 0; i < length; i++)
			if (!function(source[i], source2[i], i))
				return false;
		return true;
	}

	public static bool Equals<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2)
	{
		if (source.Length != source2.Length)
			return false;
		var length = Min(source.Length, source2.Length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (item == null || !item.Equals(source2[i]))
				return false;
		}
		return true;
	}

	public static List<T> Filter<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(length / 2);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> Filter<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(length / 2);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item, i))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static T? Find<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item))
				return item;
		}
		return default;
	}

	public static T? Find<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item, i))
				return item;
		}
		return default;
	}

	public static List<T> FindAll<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item))
				result.Add(item);
		}
		result.TrimExcess();
		return result;
	}

	public static List<T> FindAll<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item, i))
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

	public static void ForEach<T>(this ReadOnlySpan<T> source, Action<T> action)
	{
		ArgumentNullException.ThrowIfNull(action);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			action(source[i]);
	}

	public static void ForEach<T>(this ReadOnlySpan<T> source, Action<T, int> action)
	{
		ArgumentNullException.ThrowIfNull(action);
		var length = source.Length;
		for (var i = 0; i < length; i++)
			action(source[i], i);
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i]), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i], i), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this ReadOnlySpan<T> source)
	{
		ListHashSet<T> dic = [];
		var length = source.Length;
		List<(T Key, int Count)> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = source[i], out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i]), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i], i), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		var length = source.Length;
		List<(T Key, int Count)> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = source[i], out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i]), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i], i), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this ReadOnlySpan<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		var length = source.Length;
		List<(T Key, int Count)> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = source[i], out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i]), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(TResult Key, int Count)> FrequencyTable<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<(TResult Key, int Count)> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = function(source[i], i), out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<(T Key, int Count)> FrequencyTable<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<(T Key, int Count)> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			if (!dic.TryAdd(f = source[i], out var index))
				result[index] = (result[index].Key, result[index].Count + 1);
			else
				result.Add((f, 1));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, T>> Group<T>(this ReadOnlySpan<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		var length = source.Length;
		List<Group<T, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, T>> Group<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		var length = source.Length;
		List<Group<T, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, TResult>> Group<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<Group<T, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<T, T>> Group<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<Group<T, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(item);
			else
				result.Add(new(32, item, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, T>> GroupIndexes<T>(this ReadOnlySpan<T> source)
	{
		ListHashSet<T> dic = [];
		var length = source.Length;
		List<Group<int, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, T>> GroupIndexes<T>(this ReadOnlySpan<T> source, G.IEqualityComparer<T> comparer)
	{
		ListHashSet<T> dic = new(comparer);
		var length = source.Length;
		List<Group<int, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, T>> GroupIndexes<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		var length = source.Length;
		List<Group<int, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, TResult>> GroupIndexes<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<Group<int, TResult>> result = new(length);
		TResult f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = function(item, i), out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
		return result;
	}

	public static List<Group<int, T>> GroupIndexes<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Length;
		List<Group<int, T>> result = new(length);
		T f;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (!dic.TryAdd(f = item, out var index))
				result[index].Add(i);
			else
				result.Add(new(32, i, f));
		}
		result.TrimExcess();
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
		return value == value2 ? IndexOf(list_, value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? IndexOf(list_, value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? IndexOf(list_, value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? IndexOf(list_, value2) : -1;
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return IndexOf(list_, value);
	}

	public static int IndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return IndexOf(list_, value);
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
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
	}

	public static int IndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return IndexOf(list_, list_.Median());
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

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<List<T>> source)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<TResult> JoinIntoSingle<TSource, TResult>(this ReadOnlySpan<TSource> source) where TSource : G.IEnumerable<TResult>
	{
		var length = source.Length;
		List<TResult> result = new(1024);
		for (var i = 0; i < length; i++)
			result.AddRange(source[i]);
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<T[]> source)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<G.IList<T>> source)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<G.IEnumerable<T>> source)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<List<T>> source, T separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.Add(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<TResult> JoinIntoSingle<TSource, TResult>(this ReadOnlySpan<TSource> source, TResult separator) where TSource : G.IEnumerable<TResult>
	{
		var length = source.Length;
		List<TResult> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.Add(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<T[]> source, T separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.Add(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<G.IList<T>> source, T separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.Add(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<G.IEnumerable<T>> source, T separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.Add(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<List<T>> source, G.IEnumerable<T> separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.AddRange(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<TResult> JoinIntoSingle<TSource, TResult>(this ReadOnlySpan<TSource> source, G.IEnumerable<TResult> separator) where TSource : G.IEnumerable<TResult>
	{
		var length = source.Length;
		List<TResult> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.AddRange(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<T[]> source, G.IEnumerable<T> separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.AddRange(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<G.IList<T>> source, G.IEnumerable<T> separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.AddRange(separator);
			var item = source[i];
			result.AddRange(item);
		}
		return result;
	}

	public static List<T> JoinIntoSingle<T>(this ReadOnlySpan<G.IEnumerable<T>> source, G.IEnumerable<T> separator)
	{
		var length = source.Length;
		List<T> result = new(1024);
		for (var i = 0; i < length; i++)
		{
			if (i != 0)
				result.AddRange(separator);
			var item = source[i];
			result.AddRange(item);
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
		return value == value2 ? LastIndexOf(list_, value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? LastIndexOf(list_, value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? LastIndexOf(list_, value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Mean();
		var value2 = value;
		return value == value2 ? LastIndexOf(list_, value2) : -1;
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return LastIndexOf(list_, value);
	}

	public static int LastIndexOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return LastIndexOf(list_, value);
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
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
	}

	public static int LastIndexOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = source.ToList(function);
		return LastIndexOf(list_, list_.Median());
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

	public static List<TResult> Pairs<T, TResult>(this ReadOnlySpan<T> source, Func<T, T, TResult> function, int offset = 1)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		var length = source.Length - offset;
		if (length <= 0)
			return [];
		var result = RedStarLinq.EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			var item2 = source[i + offset];
			result[i] = function(item, item2);
		}
		return result;
	}

	public static List<TResult> Pairs<T, TResult>(this ReadOnlySpan<T> source, Func<T, T, int, TResult> function, int offset = 1)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		var length = source.Length - offset;
		if (length <= 0)
			return [];
		var result = RedStarLinq.EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			var item2 = source[i + offset];
			result[i] = function(item, item2, i);
		}
		return result;
	}

	public static List<(T, T)> Pairs<T>(this ReadOnlySpan<T> source, int offset = 1)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		var length = source.Length - offset;
		if (length <= 0)
			return [];
		List<(T, T)> result = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			var item2 = source[i + offset];
			result.Add((item, item2));
		}
		return result;
	}

	public static T? Progression<T>(this ReadOnlySpan<T> source, Func<T, T, T> function)
	{
		var length = source.Length;
		T? result = default;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result = result == null ? item : function(result, item);
		}
		return result;
	}

	public static TResult? Progression<T, TResult>(this ReadOnlySpan<T> source, TResult seed, Func<TResult, T, TResult> function)
	{
		var length = source.Length;
		TResult? result = default;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result = result == null ? seed : function(result, item);
		}
		return result;
	}

	public static List<T> Reverse<T>(this ReadOnlySpan<T> source)
	{
		var length = source.Length;
		var result = RedStarLinq.EmptyList<T>(length);
		for (var i = 0; i < length; i++)
			result[^(i + 1)] = source[i];
		return result;
	}

	public static Span<T> SetAll<T>(this Span<T> source, T value)
	{
		for (var i = 0; i < source.Length; i++)
			source[i] = value;
		return source;
	}

	public static bool StartsWith<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source.Length < source2.Length)
			return false;
		var length = source2.Length;
		for (var i = 0; i < length; i++)
			if (!function(source[i], source2[i]))
				return false;
		return true;
	}

	public static bool StartsWith<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source.Length < source2.Length)
			return false;
		var length = source2.Length;
		for (var i = 0; i < length; i++)
			if (!function(source[i], source2[i], i))
				return false;
		return true;
	}

	public static bool StartsWith<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2)
	{
		if (source.Length < source2.Length)
			return false;
		var length = source2.Length;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (item == null || !item.Equals(source2[i]))
				return false;
		}
		return true;
	}

	public static TResult[] ToArray<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = new TResult[length];
		for (var i = 0; i < length; i++)
			result[i] = function(source[i]);
		return result;
	}

	public static TResult[] ToArray<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = new TResult[length];
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], i);
		return result;
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>) PBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, TResult> function, Func<T, TResult2> function2)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item);
			result2[i] = function2(item);
		});
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>) PBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item, i);
			result2[i] = function2(item, i);
		});
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) PBreak<T, T2>(this G.IReadOnlyList<(T, T2)> source)
	{
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i => (result[i], result2[i]) = source[i]);
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>) PBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, (TResult, TResult2)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i]) = function(item);
		});
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>) PBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, int, (TResult, TResult2)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i]) = function(item, i);
		});
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>, List<TResult3>) PBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item);
			result2[i] = function2(item);
			result3[i] = function3(item);
		});
		return (result, result2, result3);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>, List<TResult3>) PBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item, i);
			result2[i] = function2(item, i);
			result3[i] = function3(item, i);
		});
		return (result, result2, result3);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>, List<T3>) PBreak<T, T2, T3>(this G.IReadOnlyList<(T, T2, T3)> source)
	{
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		var result3 = RedStarLinq.EmptyList<T3>(length);
		Parallel.For(0, length, i => (result[i], result2[i], result3[i]) = source[i]);
		return (result, result2, result3);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>, List<TResult3>) PBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, (TResult, TResult2, TResult3)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i], result3[i]) = function(item);
		});
		return (result, result2, result3);
	}

	[Experimental("CS9216")]
	public static (List<TResult>, List<TResult2>, List<TResult3>) PBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, int, (TResult, TResult2, TResult3)> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		var result2 = RedStarLinq.EmptyList<TResult2>(length);
		var result3 = RedStarLinq.EmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i], result3[i]) = function(item, i);
		});
		return (result, result2, result3);
	}

	[Experimental("CS9216")]
	public static List<TResult> PCombine<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T2, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			result[i] = function(item, item2);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static List<TResult> PCombine<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T2, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			result[i] = function(item, item2, i);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static List<(T, T2)> PCombine<T, T2>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2)
	{
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<(T, T2)>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			result[i] = (item, item2);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static List<TResult> PCombine<T, T2, T3, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3, Func<T, T2, T3, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min(new[] { source.Count, source2.Count, source3.Count }.AsSpan());
		var result = RedStarLinq.EmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			var item3 = source3[i];
			result[i] = function(item, item2, item3);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static List<TResult> PCombine<T, T2, T3, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3, Func<T, T2, T3, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min(new[] { source.Count, source2.Count, source3.Count }.AsSpan());
		var result = RedStarLinq.EmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			var item3 = source3[i];
			result[i] = function(item, item2, item3, i);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static List<(T, T2, T3)> PCombine<T, T2, T3>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IReadOnlyList<T3> source3)
	{
		var length = RedStarLinqMath.Min(new[] { source.Count, source2.Count, source3.Count }.AsSpan());
		List<(T, T2, T3)> result = new(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			var item3 = source3[i];
			result[i] = (item, item2, item3);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static bool PContains<T>(this G.IReadOnlyList<T> source, T target)
	{
		var length = source.Count;
		var result = false;
		Parallel.For(0, length, (i, pls) =>
		{
			var item = source[i];
			if (item?.Equals(target) ?? false)
			{
				result = true;
				pls.Stop();
			}
		});
		return result;
	}

	[Experimental("CS9216")]
	public static bool PContains<T>(this G.IReadOnlyList<T> source, T target, G.IEqualityComparer<T> comparer)
	{
		var length = source.Count;
		var result = false;
		Parallel.For(0, length, (i, pls) =>
		{
			var item = source[i];
			if (comparer.Equals(item, target))
			{
				result = true;
				pls.Stop();
			}
		});
		return result;
	}

	[Experimental("CS9216")]
	public static bool PContains<T>(this G.IReadOnlyList<T> source, T target, Func<T, T, bool> equalFunction)
	{
		var comparer = new EComparer<T>(equalFunction);
		var length = source.Count;
		var result = false;
		Parallel.For(0, length, (i, pls) =>
		{
			var item = source[i];
			if (comparer.Equals(item, target))
			{
				result = true;
				pls.Stop();
			}
		});
		return result;
	}

	public static bool PContains<T>(this G.IReadOnlyList<T> source, T target, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		var comparer = new EComparer<T>(equalFunction, hashCodeFunction);
		var length = source.Count;
		var result = false;
		Parallel.For(0, length, (i, pls) =>
		{
			var item = source[i];
			if (comparer.Equals(item, target))
			{
				result = true;
				pls.Stop();
			}
		});
		return result;
	}

	public static List<TResult> PConvert<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		Parallel.For(0, length, i => result[i] = function(source[i]));
		return result;
	}

	public static List<TResult> PConvert<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<TResult>(length);
		Parallel.For(0, length, i => result[i] = function(source[i], i));
		return result;
	}

	public static List<T> PFill<T>(T elem, int length)
	{
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i => result[i] = elem);
		return result;
	}

	public static List<T> PFill<T>(this Func<int, T> function, int length)
	{
		ArgumentNullException.ThrowIfNull(function);
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i => result[i] = function(i));
		return result;
	}

	public static T[] PFillArray<T>(T elem, int length)
	{
		var result = new T[length];
		Parallel.For(0, length, i => result[i] = elem);
		return result;
	}

	public static T[] PFillArray<T>(this Func<int, T> function, int length)
	{
		ArgumentNullException.ThrowIfNull(function);
		var result = new T[length];
		Parallel.For(0, length, i => result[i] = function(i));
		return result;
	}

	[Experimental("CS9216")]
	public static List<T> PFilter<T>(this G.IReadOnlyList<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		List<bool> values = new(length);
		Parallel.For(0, length, i => values[i] = function(source[i]));
		List<T> result = new(length / 2);
		for (var i = 0; i < length; i++)
			if (values[i])
				result.Add(source[i]);
		result.TrimExcess();
		return result;
	}

	[Experimental("CS9216")]
	public static List<T> PFilter<T>(this G.IReadOnlyList<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		List<bool> values = new(length);
		Parallel.For(0, length, i => values[i] = function(source[i], i));
		List<T> result = new(length / 2);
		for (var i = 0; i < length; i++)
			if (values[i])
				result.Add(source[i]);
		result.TrimExcess();
		return result;
	}

	public static TResult[] PToArray<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = new TResult[length];
		Parallel.For(0, length, i => result[i] = function(source[i]));
		return result;
	}

	public static TResult[] PToArray<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = new TResult[length];
		Parallel.For(0, length, i => result[i] = function(source[i], i));
		return result;
	}

	public static T[] PToArray<T>(this G.IReadOnlyList<T> source)
	{
		var length = source.Count;
		var result = new T[length];
		Parallel.For(0, length, i => result[i] = source[i]);
		return result;
	}

	public static bool TryWrap<T>(this T source, Action<T> action)
	{
		try
		{
			action(source);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool TryWrap<T, TResult>(this T source, Func<T, TResult> function, out TResult? result)
	{
		try
		{
			result = function(source);
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<T, TResult2> function2) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(function(item));
				result2.Add(function2(item));
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(function(item));
				result2.Add(function2(item));
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(function(item));
				result2.Add(function2(item));
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(function(item));
				result2.Add(function2(item));
			}
			return (result, result2);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item));
				result2.Add(function2(item));
				i++;
			}
			return (result, result2);
		}
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
			}
			return (result, result2);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				i++;
			}
			return (result, result2);
		}
	}

	public static (NList<T>, NList<T2>) NBreak<T, T2>(this G.IEnumerable<(T, T2)> source) where T : unmanaged where T2 : unmanaged
	{
		if (source is List<(T, T2)> list)
		{
			var length = list.Length;
			NList<T> result = new(length);
			NList<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is (T, T2)[] array)
		{
			var length = array.Length;
			NList<T> result = new(length);
			NList<T2> result2 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is G.IList<(T, T2)> list2)
		{
			var length = list2.Count;
			NList<T> result = new(length);
			NList<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else
		{
			NList<T> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<T2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(item.Item1);
				result2.Add(item.Item2);
				i++;
			}
			return (result, result2);
		}
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, (TResult, TResult2)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = function(array[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list2[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list3[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				i++;
			}
			return (result, result2);
		}
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this G.IEnumerable<T> source, Func<T, int, (TResult, TResult2)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = function(array[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list2[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list3[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
			}
			return (result, result2);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item, i);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				i++;
			}
			return (result, result2);
		}
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(function(item));
				result2.Add(function2(item));
				result3.Add(function3(item));
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(function(item));
				result2.Add(function2(item));
				result3.Add(function3(item));
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(function(item));
				result2.Add(function2(item));
				result3.Add(function3(item));
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(function(item));
				result2.Add(function2(item));
				result3.Add(function3(item));
			}
			return (result, result2, result3);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item));
				result2.Add(function2(item));
				result3.Add(function3(item));
				i++;
			}
			return (result, result2, result3);
		}
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				result3.Add(function3(item, i));
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				result3.Add(function3(item, i));
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				result3.Add(function3(item, i));
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				result3.Add(function3(item, i));
			}
			return (result, result2, result3);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				result2.Add(function2(item, i));
				result3.Add(function3(item, i));
				i++;
			}
			return (result, result2, result3);
		}
	}

	public static (NList<T>, NList<T2>, NList<T3>) NBreak<T, T2, T3>(this G.IEnumerable<(T, T2, T3)> source) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		if (source is List<(T, T2, T3)> list)
		{
			var length = list.Length;
			NList<T> result = new(length);
			NList<T2> result2 = new(length);
			NList<T3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is (T, T2, T3)[] array)
		{
			var length = array.Length;
			NList<T> result = new(length);
			NList<T2> result2 = new(length);
			NList<T3> result3 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<(T, T2, T3)> list2)
		{
			var length = list2.Count;
			NList<T> result = new(length);
			NList<T2> result2 = new(length);
			NList<T3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else
		{
			NList<T> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<T2> result2 = new(length);
			NList<T3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
				i++;
			}
			return (result, result2, result3);
		}
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, (TResult, TResult2, TResult3)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = function(array[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list2[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list3[i]);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				result3.Add(x.Item3);
				i++;
			}
			return (result, result2, result3);
		}
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this G.IEnumerable<T> source, Func<T, int, (TResult, TResult2, TResult3)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = function(array[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list2[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			NList<TResult> result = new(length);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = function(list3[i], i);
				result.Add(item.Item1);
				result2.Add(item.Item2);
				result3.Add(item.Item3);
			}
			return (result, result2, result3);
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			NList<TResult2> result2 = new(length);
			NList<TResult3> result3 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				var x = function(item, i);
				result.Add(x.Item1);
				result2.Add(x.Item2);
				result3.Add(x.Item3);
				i++;
			}
			return (result, result2, result3);
		}
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<T, TResult2> function2) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item));
			result2.Add(function2(item));
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item, i));
			result2.Add(function2(item, i));
		}
		return (result, result2);
	}

	public static (NList<T>, NList<T2>) NBreak<T, T2>(this ReadOnlySpan<(T, T2)> source) where T : unmanaged where T2 : unmanaged
	{
		var length = source.Length;
		NList<T> result = new(length);
		NList<T2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(item.Item1);
			result2.Add(item.Item2);
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, (TResult, TResult2)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i]);
			result.Add(item.Item1);
			result2.Add(item.Item2);
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this ReadOnlySpan<T> source, Func<T, int, (TResult, TResult2)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i], i);
			result.Add(item.Item1);
			result2.Add(item.Item2);
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item));
			result2.Add(function2(item));
			result3.Add(function3(item));
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item, i));
			result2.Add(function2(item, i));
			result3.Add(function3(item, i));
		}
		return (result, result2, result3);
	}

	public static (NList<T>, NList<T2>, NList<T3>) NBreak<T, T2, T3>(this ReadOnlySpan<(T, T2, T3)> source) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		var length = source.Length;
		NList<T> result = new(length);
		NList<T2> result2 = new(length);
		NList<T3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(item.Item1);
			result2.Add(item.Item2);
			result3.Add(item.Item3);
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, (TResult, TResult2, TResult3)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i]);
			result.Add(item.Item1);
			result2.Add(item.Item2);
			result3.Add(item.Item3);
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this ReadOnlySpan<T> source, Func<T, int, (TResult, TResult2, TResult3)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i], i);
			result.Add(item.Item1);
			result2.Add(item.Item2);
			result3.Add(item.Item3);
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this NList<T> source, Func<T, TResult> function, Func<T, TResult2> function2) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item));
			result2.Add(function2(item));
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this NList<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item, i));
			result2.Add(function2(item, i));
		}
		return (result, result2);
	}

	public static (NList<T>, NList<T2>) NBreak<T, T2>(this NList<(T, T2)> source) where T : unmanaged where T2 : unmanaged
	{
		var length = source.Length;
		NList<T> result = new(length);
		NList<T2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(item.Item1);
			result2.Add(item.Item2);
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this NList<T> source, Func<T, (TResult, TResult2)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i]);
			result.Add(item.Item1);
			result2.Add(item.Item2);
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) NBreak<T, TResult, TResult2>(this NList<T> source, Func<T, int, (TResult, TResult2)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i], i);
			result.Add(item.Item1);
			result2.Add(item.Item2);
		}
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this NList<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item));
			result2.Add(function2(item));
			result3.Add(function3(item));
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this NList<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(function(item, i));
			result2.Add(function2(item, i));
			result3.Add(function3(item, i));
		}
		return (result, result2, result3);
	}

	public static (NList<T>, NList<T2>, NList<T3>) NBreak<T, T2, T3>(this NList<(T, T2, T3)> source) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		var length = source.Length;
		NList<T> result = new(length);
		NList<T2> result2 = new(length);
		NList<T3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result.Add(item.Item1);
			result2.Add(item.Item2);
			result3.Add(item.Item3);
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this NList<T> source, Func<T, (TResult, TResult2, TResult3)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i]);
			result.Add(item.Item1);
			result2.Add(item.Item2);
			result3.Add(item.Item3);
		}
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) NBreak<T, TResult, TResult2, TResult3>(this NList<T> source, Func<T, int, (TResult, TResult2, TResult3)> function) where T : unmanaged where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<TResult> result = new(length);
		NList<TResult2> result2 = new(length);
		NList<TResult3> result3 = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = function(source[i], i);
			result.Add(item.Item1);
			result2.Add(item.Item2);
			result3.Add(item.Item3);
		}
		return (result, result2, result3);
	}

	public static NList<TResult> NCombine<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				result[i] = function(item, item2);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				result[i] = function(item, item2);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				result[i] = function(item, item2);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				result[i] = function(item, item2);
			}
			return result;
		}
		else
			return NCombine(List<T>.ReturnOrConstruct(source), List<T2>.ReturnOrConstruct(source2), function);
	}

	public static NList<TResult> NCombine<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, int, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else
			return NCombine(List<T>.ReturnOrConstruct(source), List<T2>.ReturnOrConstruct(source2), function);
	}

	public static NList<(T, T2)> NCombine<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2) where T : unmanaged where T2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			NList<(T, T2)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				result.Add((item, item2));
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			var length = Min(array.Length, array2.Length);
			NList<(T, T2)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				result.Add((item, item2));
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2)
		{
			var length = Min(list2_.Count, list2_2.Count);
			NList<(T, T2)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				result.Add((item, item2));
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2)
		{
			var length = Min(list3_.Count, list3_2.Count);
			NList<(T, T2)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				result.Add((item, item2));
			}
			return result;
		}
		else
			return NCombine(List<T>.ReturnOrConstruct(source), List<T2>.ReturnOrConstruct(source2));
	}

	public static NList<TResult> NCombine<T, T2, T3, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEnumerable<T3> source3, Func<T, T2, T3, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2 && source3 is List<T3> list3)
		{
			var length = RedStarLinqMath.Min([list.Length, list2.Length, list3.Length]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				var item3 = list3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2 && source3 is T3[] array3)
		{
			var length = RedStarLinqMath.Min([array.Length, array2.Length, array3.Length]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				var item3 = array3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2 && source3 is G.IList<T3> list2_3)
		{
			var length = RedStarLinqMath.Min([list2_.Count, list2_2.Count, list2_3.Count]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				var item3 = list2_3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2 && source3 is G.IReadOnlyList<T3> list3_3)
		{
			var length = RedStarLinqMath.Min([list3_.Count, list3_2.Count, list3_3.Count]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				var item3 = list3_3[i];
				result[i] = function(item, item2, item3);
			}
			return result;
		}
		else
			return NCombine(List<T>.ReturnOrConstruct(source), List<T2>.ReturnOrConstruct(source2), List<T3>.ReturnOrConstruct(source3), function);
	}

	public static NList<TResult> NCombine<T, T2, T3, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEnumerable<T3> source3, Func<T, T2, T3, int, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2 && source3 is List<T3> list3)
		{
			var length = RedStarLinqMath.Min([list.Length, list2.Length, list3.Length]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				var item3 = list3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2 && source3 is T3[] array3)
		{
			var length = RedStarLinqMath.Min([array.Length, array2.Length, array3.Length]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				var item3 = array3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2 && source3 is G.IList<T3> list2_3)
		{
			var length = RedStarLinqMath.Min([list2_.Count, list2_2.Count, list2_3.Count]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				var item3 = list2_3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2 && source3 is G.IReadOnlyList<T3> list3_3)
		{
			var length = RedStarLinqMath.Min([list3_.Count, list3_2.Count, list3_3.Count]);
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				var item3 = list3_3[i];
				result[i] = function(item, item2, item3, i);
			}
			return result;
		}
		else
			return NCombine(List<T>.ReturnOrConstruct(source), List<T2>.ReturnOrConstruct(source2), List<T3>.ReturnOrConstruct(source3), function);
	}

	public static NList<(T, T2, T3)> NCombine<T, T2, T3>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEnumerable<T3> source3) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		if (source is List<T> list && source2 is List<T2> list2 && source3 is List<T3> list3)
		{
			var length = RedStarLinqMath.Min([list.Length, list2.Length, list3.Length]);
			NList<(T, T2, T3)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				var item3 = list3[i];
				result[i] = (item, item2, item3);
			}
			return result;
		}
		else if (source is T[] array && source2 is T2[] array2 && source3 is T3[] array3)
		{
			var length = RedStarLinqMath.Min([array.Length, array2.Length, array3.Length]);
			NList<(T, T2, T3)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array2[i];
				var item3 = array3[i];
				result[i] = (item, item2, item3);
			}
			return result;
		}
		else if (source is G.IList<T> list2_ && source2 is G.IList<T2> list2_2 && source3 is G.IList<T3> list2_3)
		{
			var length = RedStarLinqMath.Min([list2_.Count, list2_2.Count, list2_3.Count]);
			NList<(T, T2, T3)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2_[i];
				var item2 = list2_2[i];
				var item3 = list2_3[i];
				result[i] = (item, item2, item3);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3_ && source2 is G.IReadOnlyList<T2> list3_2 && source3 is G.IReadOnlyList<T3> list3_3)
		{
			var length = RedStarLinqMath.Min([list3_.Count, list3_2.Count, list3_3.Count]);
			NList<(T, T2, T3)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3_[i];
				var item2 = list3_2[i];
				var item3 = list3_3[i];
				result[i] = (item, item2, item3);
			}
			return result;
		}
		else
			return NCombine(List<T>.ReturnOrConstruct(source), List<T2>.ReturnOrConstruct(source2), List<T3>.ReturnOrConstruct(source3));
	}

	public static NList<TResult> NCombine<T, T2, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, TResult> function) where T : unmanaged where T2 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Length, source2.Length);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, Func<T, T2, int, TResult> function) where T : unmanaged where T2 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Length, source2.Length);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], i);
		return result;
	}

	public static NList<(T, T2)> NCombine<T, T2>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2) where T : unmanaged where T2 : unmanaged
	{
		var length = Min(source.Length, source2.Length);
		NList<(T, T2)> result = new(length);
		for (var i = 0; i < length; i++)
			result[i] = (source[i], source2[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, T3, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, ReadOnlySpan<T3> source3, Func<T, T2, T3, TResult> function) where T : unmanaged where T2 : unmanaged where T3 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min((ReadOnlySpan<int>)[source.Length, source2.Length, source3.Length]);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], source3[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, T3, TResult>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, ReadOnlySpan<T3> source3, Func<T, T2, T3, int, TResult> function) where T : unmanaged where T2 : unmanaged where T3 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min((ReadOnlySpan<int>)[source.Length, source2.Length, source3.Length]);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], source3[i], i);
		return result;
	}

	public static NList<(T, T2, T3)> NCombine<T, T2, T3>(this ReadOnlySpan<T> source, ReadOnlySpan<T2> source2, ReadOnlySpan<T3> source3) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		var length = RedStarLinqMath.Min((ReadOnlySpan<int>)[source.Length, source2.Length, source3.Length]);
		NList<(T, T2, T3)> result = new(length);
		for (var i = 0; i < length; i++)
			result[i] = (source[i], source2[i], source3[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, TResult>(this NList<T> source, NList<T2> source2, Func<T, T2, TResult> function) where T : unmanaged where T2 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Length, source2.Length);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, TResult>(this NList<T> source, NList<T2> source2, Func<T, T2, int, TResult> function) where T : unmanaged where T2 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Length, source2.Length);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], i);
		return result;
	}

	public static NList<(T, T2)> NCombine<T, T2>(this NList<T> source, NList<T2> source2) where T : unmanaged where T2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		var length = Min(source.Length, source2.Length);
		NList<(T, T2)> result = new(length);
		for (var i = 0; i < length; i++)
			result[i] = (source[i], source2[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, T3, TResult>(this NList<T> source, NList<T2> source2, NList<T3> source3, Func<T, T2, T3, TResult> function) where T : unmanaged where T2 : unmanaged where T3 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min((ReadOnlySpan<int>)[source.Length, source2.Length, source3.Length]);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], source3[i]);
		return result;
	}

	public static NList<TResult> NCombine<T, T2, T3, TResult>(this NList<T> source, NList<T2> source2, NList<T3> source3, Func<T, T2, T3, int, TResult> function) where T : unmanaged where T2 : unmanaged where T3 : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min((ReadOnlySpan<int>)[source.Length, source2.Length, source3.Length]);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], source2[i], source3[i], i);
		return result;
	}

	public static NList<(T, T2, T3)> NCombine<T, T2, T3>(this NList<T> source, NList<T2> source2, NList<T3> source3) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source2);
		ArgumentNullException.ThrowIfNull(source3);
		var length = RedStarLinqMath.Min((ReadOnlySpan<int>)[source.Length, source2.Length, source3.Length]);
		NList<(T, T2, T3)> result = new(length);
		for (var i = 0; i < length; i++)
			result[i] = (source[i], source2[i], source3[i]);
		return result;
	}

	public static NList<TResult> ConvertAndJoin<T, TResult>(this NList<T> source, Func<T, G.IEnumerable<TResult>> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result.AddRange(function(source[i]));
		return result;
	}

	public static NList<TResult> ConvertAndJoin<T, TResult>(this NList<T> source, Func<T, int, G.IEnumerable<TResult>> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result.AddRange(function(source[i], i));
		return result;
	}

	public static NList<TResult> NFill<T, TResult>(this Func<int, TResult> function, int length) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(i);
		return result;
	}

	public static NList<TResult> NFill<T, TResult>(this TResult elem, int length) where T : unmanaged where TResult : unmanaged
	{
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = elem;
		return result;
	}

	public static NList<T> Filter<T>(this NList<T> source, Func<T, bool> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<T> result = new(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item))
				result.Add(item);
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static NList<T> Filter<T>(this NList<T> source, Func<T, int, bool> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<T> result = new(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item, i))
				result.Add(item);
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static NList<T> FindAll<T>(this NList<T> source, Func<T, bool> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<T> result = new(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item))
				result.Add(item);
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static NList<T> FindAll<T>(this NList<T> source, Func<T, int, bool> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		NList<T> result = new(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			if (function(item, i))
				result.Add(item);
		}
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static NList<int> FindIndexes<T>(this G.IEnumerable<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var j = 0;
			for (var i = 0; i < length; i++)
				if (function(list[i]))
					result[j++] = i;
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
				if (function(array[i]))
					result[j++] = i;
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var j = 0;
			for (var i = 0; i < length; i++)
				if (function(list2[i]))
					result[j++] = i;
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.NEmptyList<int>(length);
			var j = 0;
			for (var i = 0; i < length; i++)
				if (function(list3[i]))
					result[j++] = i;
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var i = 0;
			foreach (var item in source)
			{
				if (function(item))
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static NList<int> FindIndexes<T>(this G.IEnumerable<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (function(item, i))
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			NList<int> result = new(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (function(item, i))
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
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (function(item, i))
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
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (function(item, i))
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var i = 0;
			foreach (var item in source)
			{
				if (function(item, i))
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = [];
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(comparer);
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, T>> NGroup<T>(this G.IEnumerable<T> source, G.IEqualityComparer<T> comparer) where T : unmanaged
	{
		ListHashSet<T> dic = new(comparer);
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, T>> NGroup<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction) where T : unmanaged
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where T : unmanaged where TResult : notnull
	{
		ArgumentNullException.ThrowIfNull(function);
		ListHashSet<TResult> dic = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(array.Length);
			TResult f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, TResult>> result = new(length);
			TResult f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, T>> NGroup<T>(this G.IEnumerable<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction) where T : unmanaged
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, T>> result = new(array.Length);
			T f;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			List<NGroup<T, T>> result = new(length);
			T f;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, T>> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!dic.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static NList<int> IndexesOf<T>(this G.IEnumerable<T> source, T target)
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<int>(length);
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				if (item?.Equals(target) ?? false)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<int>(array.Length);
			var j = 0;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				if (item?.Equals(target) ?? false)
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
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				if (item?.Equals(target) ?? false)
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
			var j = 0;
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				if (item?.Equals(target) ?? false)
					result[j++] = i;
			}
			result.Resize(j);
			result.TrimExcess();
			return result;
		}
		else
		{
			NList<int> result = new(source.TryGetLengthEasily(out var length) ? length : 0);
			var i = 0;
			foreach (var item in source)
			{
				if (item?.Equals(target) ?? false)
					result.Add(i);
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static NList<int> IndexesOf<T>(this NList<T> source, T target) where T : unmanaged
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
			if (source[i].Equals(target))
				result[j++] = i;
		result.Resize(j);
		result.TrimExcess();
		return result;
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
			return IndexesOf(list_, value);
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var value = (int)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return IndexesOf(list, value);
		}
		else if (source is int[] array)
		{
			var value = (int)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return IndexesOf(array.AsSpan(), value);
		}
		else if (source is G.IList<int> list2)
		{
			var value = (int)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return IndexesOf(list2, value);
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var value = (uint)(list.Sum(x => (long)x) / Max(list.Length, 1));
			return IndexesOf(list, value);
		}
		else if (source is uint[] array)
		{
			var value = (uint)(array.Sum(x => (long)x) / Max(array.Length, 1));
			return IndexesOf(array.AsSpan(), value);
		}
		else if (source is G.IList<uint> list2)
		{
			var value = (uint)(list2.Sum(x => (long)x) / Max(list2.Count, 1));
			return IndexesOf(list2, value);
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var value = (long)(list.Sum(x => (MpzT)x) / Max(list.Length, 1));
			return IndexesOf(list, value);
		}
		else if (source is long[] array)
		{
			var value = (long)(array.Sum(x => (MpzT)x) / Max(array.Length, 1));
			return IndexesOf(array.AsSpan(), value);
		}
		else if (source is G.IList<long> list2)
		{
			var value = (long)(list2.Sum(x => (MpzT)x) / Max(list2.Count, 1));
			return IndexesOf(list2, value);
		}
		else
		{
			var list_ = NList<long>.ReturnOrConstruct(source);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var value = list.Sum() / Max(list.Length, 1);
			return IndexesOf(list, value);
		}
		else if (source is MpzT[] array)
		{
			var value = array.Sum() / Max(array.Length, 1);
			return IndexesOf(array.AsSpan(), value);
		}
		else if (source is G.IList<MpzT> list2)
		{
			var value = list2.Sum() / Max(list2.Count, 1);
			return IndexesOf(list2, value);
		}
		else
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(source);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
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
			var list_ = ToNList(list, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = list_.Mean();
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMean<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = ToNList(list, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = ToNList(array.AsSpan(), function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = ToNList(list2, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = ToNList(list3, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
		else
		{
			var list_ = ToNList(source, function);
			var sum = list_.Sum();
			var value = sum / Max(list_.Length, 1);
			if (value * list_.Length != sum)
				return [];
			return IndexesOf(list_, value);
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian<T>(this G.IEnumerable<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var list_ = RedStarLinq.Convert(list, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var list_ = RedStarLinq.Convert(array, function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var list_ = RedStarLinq.Convert(list2.GetSlice(), function);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var list_ = RedStarLinq.Convert(list3, function);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = ToNList(source, function);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<decimal> source)
	{
		if (source is NList<decimal> list)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is decimal[] array)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(array);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<decimal> list2)
		{
			var list_ = NList<decimal>.ReturnOrConstruct(list2);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<decimal>.ReturnOrConstruct(source);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<double> source)
	{
		if (source is NList<double> list)
		{
			var list_ = NList<double>.ReturnOrConstruct(list);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is double[] array)
		{
			var list_ = NList<double>.ReturnOrConstruct(array);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<double> list2)
		{
			var list_ = NList<double>.ReturnOrConstruct(list2);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<double>.ReturnOrConstruct(source);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<int> source)
	{
		if (source is NList<int> list)
		{
			var list_ = NList<int>.ReturnOrConstruct(list);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is int[] array)
		{
			var list_ = NList<int>.ReturnOrConstruct(array);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<int> list2)
		{
			var list_ = NList<int>.ReturnOrConstruct(list2);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<int>.ReturnOrConstruct(source);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<uint> source)
	{
		if (source is NList<uint> list)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is uint[] array)
		{
			var list_ = NList<uint>.ReturnOrConstruct(array);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<uint> list2)
		{
			var list_ = NList<uint>.ReturnOrConstruct(list2);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<uint>.ReturnOrConstruct(source);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<long> source)
	{
		if (source is NList<long> list)
		{
			var list_ = NList<long>.ReturnOrConstruct(list);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is long[] array)
		{
			var list_ = NList<long>.ReturnOrConstruct(array);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<long> list2)
		{
			var list_ = NList<long>.ReturnOrConstruct(list2);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<long>.ReturnOrConstruct(source);
			return IndexesOf(list_, list_.Median());
		}
	}

	public static NList<int> IndexesOfMedian(this G.IEnumerable<MpzT> source)
	{
		if (source is NList<MpzT> list)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is MpzT[] array)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(array);
			return IndexesOf(list_, list_.Median());
		}
		else if (source is G.IList<MpzT> list2)
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(list2);
			return IndexesOf(list_, list_.Median());
		}
		else
		{
			var list_ = NList<MpzT>.ReturnOrConstruct(source);
			return IndexesOf(list_, list_.Median());
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

	public static NList<TResult> NPairs<T, TResult>(this G.IEnumerable<T> source, Func<T, T, TResult> function, int offset = 1) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source is List<T> list)
		{
			var length = list.Length - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list[i + offset];
				result[i] = function(item, item2);
			}
			return result;
		}
		else if (source is T[] array)
		{
			var length = array.Length - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array[i + offset];
				result[i] = function(item, item2);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				var item2 = list2[i + offset];
				result[i] = function(item, item2);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				var item2 = list3[i + offset];
				result[i] = function(item, item2);
			}
			return result;
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? Max(length - offset, 0) : 1024);
			if (result.Capacity == 0)
				return result;
			using var en = source.GetEnumerator();
			using LimitedQueue<T> queue = new(offset);
			while (!queue.IsFull && en.MoveNext())
				queue.Enqueue(en.Current);
			var i = 0;
			while (en.MoveNext())
			{
				var item = queue.Dequeue();
				var item2 = en.Current;
				result.Add(function(item, item2));
				queue.Enqueue(item2);
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static NList<TResult> NPairs<T, TResult>(this G.IEnumerable<T> source, Func<T, T, int, TResult> function, int offset = 1) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source is List<T> list)
		{
			var length = list.Length - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list[i + offset];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			var length = array.Length - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array[i + offset];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				var item2 = list2[i + offset];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count - offset;
			if (length <= 0)
				return [];
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				var item2 = list3[i + offset];
				result[i] = function(item, item2, i);
			}
			return result;
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? Max(length - offset, 0) : 1024);
			if (result.Capacity == 0)
				return result;
			using var en = source.GetEnumerator();
			using LimitedQueue<T> queue = new(offset);
			while (!queue.IsFull && en.MoveNext())
				queue.Enqueue(en.Current);
			var i = 0;
			while (en.MoveNext())
			{
				var item = queue.Dequeue();
				var item2 = en.Current;
				result.Add(function(item, item2, i));
				queue.Enqueue(item2);
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static NList<(T, T)> NPairs<T>(this G.IEnumerable<T> source, int offset = 1) where T : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		if (source is List<T> list)
		{
			var length = list.Length - offset;
			if (length <= 0)
				return [];
			NList<(T, T)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list[i + offset];
				result.Add((item, item2));
			}
			return result;
		}
		else if (source is T[] array)
		{
			var length = array.Length - offset;
			if (length <= 0)
				return [];
			NList<(T, T)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = array[i];
				var item2 = array[i + offset];
				result.Add((item, item2));
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count - offset;
			if (length <= 0)
				return [];
			NList<(T, T)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				var item2 = list2[i + offset];
				result.Add((item, item2));
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count - offset;
			if (length <= 0)
				return [];
			NList<(T, T)> result = new(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				var item2 = list3[i + offset];
				result.Add((item, item2));
			}
			return result;
		}
		else
		{
			NList<(T, T)> result = new(source.TryGetLengthEasily(out var length) ? Max(length - offset, 0) : 1024);
			if (result.Capacity == 0)
				return result;
			using var en = source.GetEnumerator();
			using LimitedQueue<T> queue = new(offset);
			while (!queue.IsFull && en.MoveNext())
				queue.Enqueue(en.Current);
			var i = 0;
			while (en.MoveNext())
			{
				var item = queue.Dequeue();
				var item2 = en.Current;
				result.Add((item, item2));
				queue.Enqueue(item2);
				i++;
			}
			result.Resize(i);
			result.Resize(i);
			return result;
		}
	}

	public static NList<TResult> NPairs<T, TResult>(this ReadOnlySpan<T> source, Func<T, T, TResult> function, int offset = 1) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		var length = source.Length - offset;
		if (length <= 0)
			return [];
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			var item2 = source[i + offset];
			result[i] = function(item, item2);
		}
		return result;
	}

	public static NList<TResult> NPairs<T, TResult>(this ReadOnlySpan<T> source, Func<T, T, int, TResult> function, int offset = 1) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		var length = source.Length - offset;
		if (length <= 0)
			return [];
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			var item2 = source[i + offset];
			result[i] = function(item, item2, i);
		}
		return result;
	}

	public static NList<(T, T)> NPairs<T>(this ReadOnlySpan<T> source, int offset = 1) where T : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(offset, 1);
		var length = source.Length - offset;
		if (length <= 0)
			return [];
		NList<(T, T)> result = new(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			var item2 = source[i + offset];
			result.Add((item, item2));
		}
		return result;
	}

	public static NList<int> RepresentIntoNumbers<T>(this NList<T> source) where T : unmanaged
	{
		ListHashSet<T> dic = [];
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = !dic.TryAdd(item, out var index) ? index : j++;
		}
		return result;
	}

	public static NList<int> RepresentIntoNumbers<T>(this NList<T> source, Func<T, T, bool> equalFunction) where T : unmanaged
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = !dic.TryAdd(item, out var index) ? index : j++;
		}
		return result;
	}

	public static NList<int> RepresentIntoNumbers<T>(this NList<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction) where T : unmanaged
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = !dic.TryAdd(item, out var index) ? index : j++;
		}
		return result;
	}

	public static NList<T> Reverse<T>(this NList<T> source) where T : unmanaged
	{
		var length = source.Length;
		NList<T> result = new(length);
		for (var i = 0; i < length; i++)
			result[source.Length - 1 - i] = source[i];
		return result;
	}

	public static NList<TResult> NShuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Random random) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list[0]);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<TResult>(array.Length);
			if (array.Length == 0)
				return result;
			result[0] = function(array[0]);
			int blend;
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list2[0]);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list3[0]);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item);
				}
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			NList<TResult> result = new(1024);
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result.Add(function(item));
				else
				{
					result.Add(result[blend]);
					result[blend] = function(item);
				}
				i++;
			}
			return result;
		}
	}

	public static NList<TResult> NShuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Random random) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list[0], 0);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<TResult>(array.Length);
			if (array.Length == 0)
				return result;
			result[0] = function(array[0], 0);
			int blend;
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list2[0], 0);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			result[0] = function(list3[0], 0);
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.NEmptyList<TResult>(length);
			if (length == 0)
				return result;
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = function(item, i);
				else
				{
					result[i] = result[blend];
					result[blend] = function(item, i);
				}
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			NList<TResult> result = new(1024);
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result.Add(function(item, i));
				else
				{
					result.Add(result[blend]);
					result[blend] = function(item, i);
				}
				i++;
			}
			return result;
		}
	}

	public static NList<T> NShuffle<T>(this G.IEnumerable<T> source, Random random) where T : unmanaged
	{
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<T>(length);
			if (length == 0)
				return result;
			result[0] = list[0];
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<T>(array.Length);
			if (array.Length == 0)
				return result;
			result[0] = array[0];
			int blend;
			for (var i = 1; i < array.Length; i++)
			{
				var item = array[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<T>(length);
			if (length == 0)
				return result;
			result[0] = list2[0];
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list2[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.NEmptyList<T>(length);
			if (length == 0)
				return result;
			result[0] = list3[0];
			int blend;
			for (var i = 1; i < length; i++)
			{
				var item = list3[i];
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = RedStarLinq.NEmptyList<T>(length);
			if (length == 0)
				return result;
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result[i] = item;
				else
				{
					result[i] = result[blend];
					result[blend] = item;
				}
				i++;
			}
			result.Resize(i);
			return result;
		}
		else
		{
			NList<T> result = new(1024);
			int blend;
			var i = 0;
			foreach (var item in source)
			{
				blend = random.Next(i + 1);
				if (blend == i)
					result.Add(item);
				else
				{
					result.Add(result[blend]);
					result[blend] = item;
				}
				i++;
			}
			return result;
		}
	}

	public static List<NList<T>> NSplitIntoEqual<T>(this G.IReadOnlyList<T> source, int fragmentLength) where T : unmanaged
	{
		if (fragmentLength <= 0)
			throw new ArgumentException("Длина фрагмента должна быть положительной.", nameof(fragmentLength));
		var length = GetArrayLength(source.Count, fragmentLength);
		List<NList<T>> result = new(length);
		var length2 = source.Count / fragmentLength;
		var index = 0;
		for (var i = 0; i < length2; i++)
		{
			result.Add(RedStarLinq.NEmptyList<T>(fragmentLength));
			for (var j = 0; j < fragmentLength; j++)
				result[i][j] = source[index++];
		}
		var rest = source.Count % fragmentLength;
		if (rest != 0)
		{
			result.Add(RedStarLinq.NEmptyList<T>(rest));
			for (var j = 0; j < rest; j++)
				result[length2][j] = source[index++];
		}
		return result;
	}

	public static NList<TResult> ToNList<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<TResult>(array.Length);
			for (var i = 0; i < array.Length; i++)
				result[i] = function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list3[i]);
			return result;
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : 1024);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static NList<TResult> ToNList<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = RedStarLinq.NEmptyList<TResult>(array.Length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = RedStarLinq.NEmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else
		{
			NList<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : 1024);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static NList<TResult> ToNList<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i]);
		return result;
	}

	public static NList<TResult> ToNList<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], i);
		return result;
	}

	public static NList<TResult> ToNList<T, TResult>(this NList<T> source, Func<T, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i]);
		return result;
	}

	public static NList<TResult> ToNList<T, TResult>(this NList<T> source, Func<T, int, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = function(item, i);
		}
		return result;
	}

	public static String ToNString<T>(this G.IEnumerable<T> source, Func<T, char> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			String result = new('0', length);
			for (var i = 0; i < length; i++)
				result[i] = function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			String result = new('0', array.Length);
			for (var i = 0; i < array.Length; i++)
				result[i] = function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			String result = new('0', length);
			for (var i = 0; i < length; i++)
				result[i] = function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			String result = new('0', length);
			for (var i = 0; i < length; i++)
				result[i] = function(list3[i]);
			return result;
		}
		else
		{
			String result = new(source.TryGetLengthEasily(out var length) ? length : 1024);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static String ToNString<T>(this G.IEnumerable<T> source, Func<T, int, char> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			String result = new('0', length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			String result = new('0', array.Length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			String result = new('0', length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			String result = new('0', length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else
		{
			String result = new(source.TryGetLengthEasily(out var length) ? length : 1024);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(function(item, i));
				i++;
			}
			result.Resize(i);
			return result;
		}
	}

	public static String ToNString<T>(this ReadOnlySpan<T> source, Func<T, char> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		String result = new('0', length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i]);
		return result;
	}

	public static String ToNString<T>(this ReadOnlySpan<T> source, Func<T, int, char> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		String result = new('0', length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], i);
		return result;
	}

	public static NList<int> FindIndexes<T>(this ReadOnlySpan<T> source, Func<T, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
			if (function(source[i]))
				result[j++] = i;
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static NList<int> FindIndexes<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
			if (function(source[i], i))
				result[j++] = i;
		result.Resize(j);
		result.TrimExcess();
		return result;
	}

	public static NList<int> IndexesOf<T>(this ReadOnlySpan<T> source, T target)
	{
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
			if (source[i]?.Equals(target) ?? false)
				result[j++] = i;
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
		var list_ = ToNList(source, function);
		var value = list_.Mean();
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = list_.Mean();
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = list_.Mean();
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = list_.Mean();
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = (int)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = (uint)(list_.Sum(x => (long)x) / Max(list_.Length, 1));
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = (long)(list_.Sum(x => (MpzT)x) / Max(list_.Length, 1));
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		var value = list_.Sum() / (MpzT)list_.Length;
		return IndexesOf(list_, value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<decimal> source)
	{
		var value = source.Mean();
		return IndexesOf(source, value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<double> source)
	{
		var value = source.Mean();
		return IndexesOf(source, value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<int> source)
	{
		var value = (int)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return IndexesOf(source, value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<uint> source)
	{
		var value = (uint)(source.Sum(x => (long)x) / Max(source.Length, 1));
		return IndexesOf(source, value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<long> source)
	{
		var value = (long)(source.Sum(x => (MpzT)x) / Max(source.Length, 1));
		return IndexesOf(source, value);
	}

	public static NList<int> IndexesOfMean(this ReadOnlySpan<MpzT> source)
	{
		var value = source.Sum() / (MpzT)source.Length;
		return IndexesOf(source, value);
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, decimal> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, double> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, int> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, uint> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, long> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian<T>(this ReadOnlySpan<T> source, Func<T, int, MpzT> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var list_ = ToNList(source, function);
		return IndexesOf(list_, list_.Median());
	}

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<decimal> source) => IndexesOf(source, source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<double> source) => IndexesOf(source, source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<int> source) => IndexesOf(source, source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<uint> source) => IndexesOf(source, source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<long> source) => IndexesOf(source, source.Median());

	public static NList<int> IndexesOfMedian(this ReadOnlySpan<MpzT> source) => IndexesOf(source, source.Median());

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

	public static NList<int> RepresentIntoNumbers<T>(this ReadOnlySpan<T> source)
	{
		ListHashSet<T> dic = [];
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = !dic.TryAdd(item, out var index) ? index : j++;
		}
		return result;
	}

	public static NList<int> RepresentIntoNumbers<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction));
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = !dic.TryAdd(item, out var index) ? index : j++;
		}
		return result;
	}

	public static NList<int> RepresentIntoNumbers<T>(this ReadOnlySpan<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ListHashSet<T> dic = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Length;
		var result = RedStarLinq.NEmptyList<int>(length);
		var j = 0;
		for (var i = 0; i < length; i++)
		{
			var item = source[i];
			result[i] = !dic.TryAdd(item, out var index) ? index : j++;
		}
		return result;
	}

	public static (NList<TResult>, NList<TResult2>) PNBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, TResult> function, Func<T, TResult2> function2) where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item);
			result2[i] = function2(item);
		});
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) PNBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2) where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item, i);
			result2[i] = function2(item, i);
		});
		return (result, result2);
	}

	public static (NList<T>, NList<T2>) PNBreak<T, T2>(this G.IReadOnlyList<(T, T2)> source) where T : unmanaged where T2 : unmanaged
	{
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<T>(length);
		var result2 = RedStarLinq.NEmptyList<T2>(length);
		Parallel.For(0, length, i => (result[i], result2[i]) = source[i]);
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) PNBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, (TResult, TResult2)> function) where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i]) = function(item);
		});
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>) PNBreak<T, TResult, TResult2>(this G.IReadOnlyList<T> source, Func<T, int, (TResult, TResult2)> function) where TResult : unmanaged where TResult2 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i]) = function(item, i);
		});
		return (result, result2);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) PNBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, TResult> function, Func<T, TResult2> function2, Func<T, TResult3> function3) where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		var result3 = RedStarLinq.NEmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item);
			result2[i] = function2(item);
			result3[i] = function3(item);
		});
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) PNBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, Func<T, int, TResult2> function2, Func<T, int, TResult3> function3) where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		ArgumentNullException.ThrowIfNull(function3);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		var result3 = RedStarLinq.NEmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item, i);
			result2[i] = function2(item, i);
			result3[i] = function3(item, i);
		});
		return (result, result2, result3);
	}

	public static (NList<T>, NList<T2>, NList<T3>) PNBreak<T, T2, T3>(this G.IReadOnlyList<(T, T2, T3)> source) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<T>(length);
		var result2 = RedStarLinq.NEmptyList<T2>(length);
		var result3 = RedStarLinq.NEmptyList<T3>(length);
		Parallel.For(0, length, i => (result[i], result2[i], result3[i]) = source[i]);
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) PNBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, (TResult, TResult2, TResult3)> function) where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		var result3 = RedStarLinq.NEmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i], result3[i]) = function(item);
		});
		return (result, result2, result3);
	}

	public static (NList<TResult>, NList<TResult2>, NList<TResult3>) PNBreak<T, TResult, TResult2, TResult3>(this G.IReadOnlyList<T> source, Func<T, int, (TResult, TResult2, TResult3)> function) where TResult : unmanaged where TResult2 : unmanaged where TResult3 : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		var result2 = RedStarLinq.NEmptyList<TResult2>(length);
		var result3 = RedStarLinq.NEmptyList<TResult3>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			(result[i], result2[i], result3[i]) = function(item, i);
		});
		return (result, result2, result3);
	}

	[Experimental("CS9216")]
	public static NList<TResult> PCombine<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IList<T2> source2, Func<T, T2, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			result[i] = function(item, item2);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static NList<TResult> PCombine<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IList<T2> source2, Func<T, T2, int, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.NEmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			result[i] = function(item, item2, i);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static NList<(T, T2)> PCombine<T, T2>(this G.IReadOnlyList<T> source, G.IList<T2> source2) where T : unmanaged where T2 : unmanaged
	{
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.NEmptyList<(T, T2)>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			result[i] = (item, item2);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static NList<TResult> PCombine<T, T2, T3, TResult>(this G.IReadOnlyList<T> source, G.IList<T2> source2, G.IList<T3> source3, Func<T, T2, T3, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min(new[] { source.Count, source2.Count, source3.Count }.AsSpan());
		var result = RedStarLinq.NEmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			var item3 = source3[i];
			result[i] = function(item, item2, item3);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static NList<TResult> PCombine<T, T2, T3, TResult>(this G.IReadOnlyList<T> source, G.IList<T2> source2, G.IList<T3> source3, Func<T, T2, T3, int, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = RedStarLinqMath.Min(new[] { source.Count, source2.Count, source3.Count }.AsSpan());
		var result = RedStarLinq.NEmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			var item3 = source3[i];
			result[i] = function(item, item2, item3, i);
		});
		return result;
	}

	[Experimental("CS9216")]
	public static NList<(T, T2, T3)> PCombine<T, T2, T3>(this G.IReadOnlyList<T> source, G.IList<T2> source2, G.IList<T3> source3) where T : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		var length = RedStarLinqMath.Min(new[] { source.Count, source2.Count, source3.Count }.AsSpan());
		NList<(T, T2, T3)> result = new(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			var item3 = source3[i];
			result[i] = (item, item2, item3);
		});
		return result;
	}

	public static NList<TResult> PNConvert<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		Parallel.For(0, length, i => result[i] = function(source[i]));
		return result;
	}

	public static NList<TResult> PNConvert<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function) where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		var result = RedStarLinq.NEmptyList<TResult>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			result[i] = function(item, i);
		});
		return result;
	}

	public static NList<T> PNFill<T>(T elem, int length) where T : unmanaged
	{
		var result = RedStarLinq.NEmptyList<T>(length);
		Parallel.For(0, length, i => result[i] = elem);
		return result;
	}

	public static NList<T> PNFill<T>(Func<int, T> function, int length) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var result = RedStarLinq.NEmptyList<T>(length);
		Parallel.For(0, length, i => result[i] = function(i));
		return result;
	}

	[Experimental("CS9216")]
	public static NList<T> PNFilter<T>(this G.IReadOnlyList<T> source, Func<T, bool> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		NList<bool> values = new(length);
		Parallel.For(0, length, i => values[i] = function(source[i]));
		NList<T> result = new(length / 2);
		for (var i = 0; i < length; i++)
			if (values[i])
				result.Add(source[i]);
		result.TrimExcess();
		return result;
	}

	[Experimental("CS9216")]
	public static NList<T> PNFilter<T>(this G.IReadOnlyList<T> source, Func<T, int, bool> function) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Count;
		NList<bool> values = new(length);
		Parallel.For(0, length, i => values[i] = function(source[i], i));
		NList<T> result = new(length / 2);
		for (var i = 0; i < length; i++)
			if (values[i])
				result.Add(source[i]);
		result.TrimExcess();
		return result;
	}

	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++));
	}

	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, G.IEqualityComparer<TKey> comparer)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), comparer);
	}

	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, Func<TKey, TKey, bool> equalFunction)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), new EComparer<TKey>(equalFunction));
	}

	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), new EComparer<TKey>(equalFunction, hashCodeFunction));
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<TKey, G.IEnumerable<T>, int, TResult> resultSelector)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), (x, y) => resultSelector(x, y, j++));
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<TKey, G.IEnumerable<T>, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), (x, y) => resultSelector(x, y, j++), comparer);
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<TKey, G.IEnumerable<T>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), (x, y) => resultSelector(x, y, j++), new EComparer<TKey>(equalFunction));
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<TKey, G.IEnumerable<T>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	{
		int i = 0, j = 0;
		return E.GroupBy(source, x => keySelector(x, i++), (x, y) => resultSelector(x, y, j++), new EComparer<TKey>(equalFunction, hashCodeFunction));
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, int, TResult> resultSelector)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), (x, y) => resultSelector(x, y, k++));
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), (x, y) => resultSelector(x, y, k++), comparer);
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), (x, y) => resultSelector(x, y, k++), new EComparer<TKey>(equalFunction));
	}

	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupBy(source, x => keySelector(x, i++), x => elementSelector(x, j++), (x, y) => resultSelector(x, y, k++), new EComparer<TKey>(equalFunction, hashCodeFunction));
	}

	public static G.IEnumerable<TResult> GroupJoin<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupJoin(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++));
	}

	public static G.IEnumerable<TResult> GroupJoin<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupJoin(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++), comparer);
	}

	public static G.IEnumerable<TResult> GroupJoin<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupJoin(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++), new EComparer<TKey>(equalFunction));
	}

	public static G.IEnumerable<TResult> GroupJoin<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	{
		int i = 0, j = 0, k = 0;
		return E.GroupJoin(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++), new EComparer<TKey>(equalFunction, hashCodeFunction));
	}

	public static G.IEnumerable<TResult> Join<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector)
	{
		int i = 0, j = 0, k = 0;
		return E.Join(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++));
	}

	public static G.IEnumerable<TResult> Join<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer)
	{
		int i = 0, j = 0, k = 0;
		return E.Join(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++), comparer);
	}

	public static G.IEnumerable<TResult> Join<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction)
	{
		int i = 0, j = 0, k = 0;
		return E.Join(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++), new EComparer<TKey>(equalFunction));
	}

	public static G.IEnumerable<TResult> Join<T, TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction)
	{
		int i = 0, j = 0, k = 0;
		return E.Join(outer, inner, x => outerKeySelector(x, i++), x => innerKeySelector(x, j++), (x, y) => resultSelector(x, y, k++), new EComparer<TKey>(equalFunction, hashCodeFunction));
	}

	public static G.IEnumerable<T> Sort<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		var i = 0;
		return E.OrderBy(source, x => function(x, i++));
	}

	public static G.IEnumerable<T> Sort<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IComparer<TResult> comparer)
	{
		var i = 0;
		return E.OrderBy(source, x => function(x, i++), comparer);
	}

	public static G.IEnumerable<T> Sort<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, int> compareFunction)
	{
		var i = 0;
		return E.OrderBy(source, x => function(x, i++), new Comparer<TResult>(compareFunction));
	}

	public static G.IEnumerable<T> SortDesc<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function)
	{
		var i = 0;
		return E.OrderByDescending(source, x => function(x, i++));
	}

	public static G.IEnumerable<T> SortDesc<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IComparer<TResult> comparer)
	{
		var i = 0;
		return E.OrderByDescending(source, x => function(x, i++), comparer);
	}

	public static G.IEnumerable<T> SortDesc<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, int> compareFunction)
	{
		var i = 0;
		return E.OrderByDescending(source, x => function(x, i++), new Comparer<TResult>(compareFunction));
	}

	public static G.IEnumerable<T> Append<T>(this G.IEnumerable<T> source, T element) => E.Append(source, element);
	public static (List<T>, List<T>) BreakFilter<T>(this G.IEnumerable<T> source, Func<T, bool> function) => (BreakFilter(source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this G.IEnumerable<T> source, Func<T, int, bool> function) => (BreakFilter(source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this ReadOnlySpan<T> source, Func<T, bool> function) => (BreakFilter(source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this ReadOnlySpan<T> source, Func<T, int, bool> function) => (BreakFilter(source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this Span<T> source, Func<T, bool> function) => (BreakFilter((ReadOnlySpan<T>)source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this Span<T> source, Func<T, int, bool> function) => (BreakFilter((ReadOnlySpan<T>)source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this T[] source, Func<T, bool> function) => (BreakFilter((G.IList<T>)source, function, out var result2), result2);
	public static (List<T>, List<T>) BreakFilter<T>(this T[] source, Func<T, int, bool> function) => (BreakFilter((G.IList<T>)source, function, out var result2), result2);
	public static G.IEnumerable<TResult> Combine<T, T2, TResult>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, Func<T, T2, TResult> function) => E.Zip(source, source2, function);
	public static G.IEnumerable<(T, T2)> Combine<T, T2>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2) => E.Zip(source, source2);
	public static G.IEnumerable<(T, T2, T3)> Combine<T, T2, T3>(this G.IEnumerable<T> source, G.IEnumerable<T2> source2, G.IEnumerable<T3> source3) => E.Zip(source, source2, source3);
	public static Slice<TResult> Combine<T, T2, TResult>(this T[] source, T2[] source2, Func<T, T2, TResult> function) => Combine((G.IReadOnlyList<T>)source, source2, function);
	public static Slice<TResult> Combine<T, T2, TResult>(this T[] source, T2[] source2, Func<T, T2, int, TResult> function) => Combine((G.IReadOnlyList<T>)source, source2, function);
	public static Slice<TResult> Combine<T, T2, TResult>(this (G.IReadOnlyList<T>, G.IReadOnlyList<T2>) source, Func<T, T2, TResult> function) => Combine(source.Item1, source.Item2, function);
	public static G.IEnumerable<TResult> Combine<T, T2, TResult>(this (G.IEnumerable<T>, G.IEnumerable<T2>) source, Func<T, T2, TResult> function) => E.Zip(source.Item1, source.Item2, function);
	public static Slice<TResult> Combine<T, T2, TResult>(this (G.IReadOnlyList<T>, G.IReadOnlyList<T2>) source, Func<T, T2, int, TResult> function) => Combine(source.Item1, source.Item2, function);
	public static G.IEnumerable<TResult> Combine<T, T2, TResult>(this (G.IEnumerable<T>, G.IEnumerable<T2>) source, Func<T, T2, int, TResult> function) => Combine(source.Item1, source.Item2, function);
	public static Slice<(T, T2)> Combine<T, T2>(this (G.IReadOnlyList<T>, G.IReadOnlyList<T2>) source) => Combine(source.Item1, source.Item2);
	public static G.IEnumerable<(T, T2)> Combine<T, T2>(this (G.IEnumerable<T>, G.IEnumerable<T2>) source) => E.Zip(source.Item1, source.Item2);
	public static Slice<TResult> Combine<T, T2, T3, TResult>(this (G.IReadOnlyList<T>, G.IReadOnlyList<T2>, G.IReadOnlyList<T3>) source, Func<T, T2, T3, TResult> function) => Combine(source.Item1, source.Item2, source.Item3, function);
	public static G.IEnumerable<TResult> Combine<T, T2, T3, TResult>(this (G.IEnumerable<T>, G.IEnumerable<T2>, G.IEnumerable<T3>) source, Func<T, T2, T3, TResult> function) => Combine(source.Item1, source.Item2, source.Item3, function);
	public static Slice<TResult> Combine<T, T2, T3, TResult>(this (G.IReadOnlyList<T>, G.IReadOnlyList<T2>, G.IReadOnlyList<T3>) source, Func<T, T2, T3, int, TResult> function) => Combine(source.Item1, source.Item2, source.Item3, function);
	public static G.IEnumerable<TResult> Combine<T, T2, T3, TResult>(this (G.IEnumerable<T>, G.IEnumerable<T2>, G.IEnumerable<T3>) source, Func<T, T2, T3, int, TResult> function) => Combine(source.Item1, source.Item2, source.Item3, function);
	public static Slice<(T, T2, T3)> Combine<T, T2, T3>(this (G.IReadOnlyList<T>, G.IReadOnlyList<T2>, G.IReadOnlyList<T3>) source) => Combine(source.Item1, source.Item2, source.Item3);
	public static G.IEnumerable<(T, T2, T3)> Combine<T, T2, T3>(this (G.IEnumerable<T>, G.IEnumerable<T2>, G.IEnumerable<T3>) source) => E.Zip(source.Item1, source.Item2, source.Item3);
	public static G.IEnumerable<TResult> ConvertAndJoin<T, TResult>(this G.IEnumerable<T> source, Func<T, G.IEnumerable<TResult>> function) => E.SelectMany(source, function);
	public static G.IEnumerable<TResult> ConvertAndJoin<T, TResult>(this G.IEnumerable<T> source, Func<T, int, G.IEnumerable<TResult>> function) => E.SelectMany(source, function);
	public static G.IEnumerable<TResult> ConvertAndJoin<T, TCollection, TResult>(this G.IEnumerable<T> source, Func<T, G.IEnumerable<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) => E.SelectMany(source, collectionSelector, resultSelector);
	public static G.IEnumerable<TResult> ConvertAndJoin<T, TResult>(this T[] source, Func<T, G.IEnumerable<TResult>> function) => E.SelectMany(source, function);
	public static G.IEnumerable<TResult> ConvertAndJoin<T, TResult>(this T[] source, Func<T, int, G.IEnumerable<TResult>> function) => E.SelectMany(source, function);
	public static List<List<T>> CopyDoubleList<T>(this List<List<T>> source) => source.ToList(x => x.Copy());
	public static List<List<List<T>>> CopyTripleList<T>(this List<List<List<T>>> source) => source.ToList(x => x.CopyDoubleList());
	public static G.IEnumerable<T?> DefaultIfEmpty<T>(this G.IEnumerable<T> source) => E.DefaultIfEmpty(source);
	public static G.IEnumerable<T> DefaultIfEmpty<T>(this G.IEnumerable<T> source, T defaultValue) => E.DefaultIfEmpty(source, defaultValue);
	public static T ElementAt<T>(this G.IEnumerable<T> source, int index) => E.ElementAt(source, index);
	public static T? ElementAtOrDefault<T>(this G.IEnumerable<T> source, int index) => E.ElementAtOrDefault(source, index);
	public static G.IEnumerable<T?> Empty<T>() => [];
	public static G.IEnumerable<T> Except<T>(this G.IEnumerable<T> source, G.IEnumerable<T> source2) => E.Except(source, source2);
	public static G.IEnumerable<T> Except<T>(this G.IEnumerable<T> source, G.IEnumerable<T> source2, G.IEqualityComparer<T> comparer) => E.Except(source, source2, comparer);
	public static G.IEnumerable<T> Filter<T>(this T[] source, Func<T, bool> function) => E.Where(source, function);
	public static G.IEnumerable<T> Filter<T>(this T[] source, Func<T, int, bool> function) => E.Where(source, function);
	public static T First<T>(this G.IEnumerable<T> source) => E.First(source);
	public static T? FirstOrDefault<T>(this G.IEnumerable<T> source) => E.FirstOrDefault(source);
	[Obsolete("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в последовательности, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.", true)]
	public static List<Group<T, T>> Group<T>(this G.IEnumerable<T> source) where T : notnull => throw new NotSupportedException("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в последовательности, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.");
	[Obsolete("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в ReadOnlySpan, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.", true)]
	public static List<Group<T, T>> Group<T>(this ReadOnlySpan<T> source) where T : notnull => throw new NotSupportedException("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в ReadOnlySpan, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.");
	[Obsolete("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в Span, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.", true)]
	public static List<Group<T, T>> Group<T>(this Span<T> source) where T : notnull => throw new NotSupportedException("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в Span, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.");
	[Obsolete("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в массиве, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.", true)]
	public static List<Group<T, T>> Group<T>(this T[] source) where T : notnull => throw new NotSupportedException("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в массиве, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.");
	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) => E.GroupBy(source, keySelector, elementSelector);
	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, G.IEqualityComparer<TKey> comparer) => E.GroupBy(source, keySelector, elementSelector, comparer);
	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, TKey, bool> equalFunction) => E.GroupBy(source, keySelector, elementSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<System.Linq.IGrouping<TKey, TElement>> Group<T, TKey, TElement>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.GroupBy(source, keySelector, elementSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, G.IEnumerable<T>, TResult> resultSelector) => E.GroupBy(source, keySelector, resultSelector);
	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, G.IEnumerable<T>, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => E.GroupBy(source, keySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, G.IEnumerable<T>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => E.GroupBy(source, keySelector, resultSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<TResult> Group<T, TKey, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, G.IEnumerable<T>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.GroupBy(source, keySelector, resultSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, TResult> resultSelector) => E.GroupBy(source, keySelector, elementSelector, resultSelector);
	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => E.GroupBy(source, keySelector, elementSelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => E.GroupBy(source, keySelector, elementSelector, resultSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<TResult> Group<T, TKey, TElement, TResult>(this G.IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, G.IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.GroupBy(source, keySelector, elementSelector, resultSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector) => E.GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector) => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => E.GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => E.GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, equalFunction);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, equalFunction, hashCodeFunction);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector) => E.GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector) => GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => E.GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => E.GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, equalFunction);
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, G.IEnumerable<TInner>, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => GroupJoin(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, equalFunction, hashCodeFunction);
	public static G.IEnumerable<T> Intersect<T>(this G.IEnumerable<T> source, G.IEnumerable<T> source2) => E.Intersect(source, source2);
	public static G.IEnumerable<T> Intersect<T>(this G.IEnumerable<T> source, G.IEnumerable<T> source2, G.IEqualityComparer<T> comparer) => E.Intersect(source, source2, comparer);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) => E.Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector) => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => E.Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => E.Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, equalFunction);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this G.IEnumerable<TOuter> outer, G.IEnumerable<TInner> inner, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, equalFunction, hashCodeFunction);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) => E.Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector) => Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => E.Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, G.IEqualityComparer<TKey> comparer) => Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, comparer);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => E.Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction));
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction) => Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, equalFunction);
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => E.Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, new EComparer<TKey>(equalFunction, hashCodeFunction));
	public static G.IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this (G.IEnumerable<TOuter>, G.IEnumerable<TInner>) source, Func<TOuter, int, TKey> outerKeySelector, Func<TInner, int, TKey> innerKeySelector, Func<TOuter, TInner, int, TResult> resultSelector, Func<TKey, TKey, bool> equalFunction, Func<TKey, int> hashCodeFunction) => Join(source.Item1, source.Item2, outerKeySelector, innerKeySelector, resultSelector, equalFunction, hashCodeFunction);
	public static T Last<T>(this G.IEnumerable<T> source) => E.Last(source);
	public static T? LastOrDefault<T>(this G.IEnumerable<T> source) => E.LastOrDefault(source);
	public static G.IEnumerable<TResult> OfType<TResult>(this IEnumerable source) => E.OfType<TResult>(source);
	public static G.IEnumerable<T> Prepend<T>(this G.IEnumerable<T> source, T element) => E.Prepend(source, element);
	public static T Random<T>(this ReadOnlySpan<T> source) => source[random.Next(source.Length)];
	public static T Random<T>(this Span<T> source) => source[random.Next(source.Length)];
	public static T Random<T>(this T[] source) => source[random.Next(source.Length)];
	public static T Random<T>(this ReadOnlySpan<T> source, Random randomObj) => source[randomObj.Next(source.Length)];
	public static T Random<T>(this Span<T> source, Random randomObj) => source[randomObj.Next(source.Length)];
	public static T Random<T>(this T[] source, Random randomObj) => source[randomObj.Next(source.Length)];
	public static G.IEnumerable<T> Reverse<T>(this G.IEnumerable<T> source) => E.Reverse(source);
	public static T[] SetAll<T>(this T[] source, T value) => SetAll((G.IList<T>)source, value) as T[] ?? throw new InvalidCastException("Произошла внутренняя программная или аппаратная ошибка. Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
	public static T[] SetAll<T>(this T[] source, T value, Index index) => SetAll((G.IList<T>)source, value, index) as T[] ?? throw new InvalidCastException("Произошла внутренняя программная или аппаратная ошибка. Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
	public static T[] SetAll<T>(this T[] source, T value, int index) => SetAll((G.IList<T>)source, value, index) as T[] ?? throw new InvalidCastException("Произошла внутренняя программная или аппаратная ошибка. Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
	public static T[] SetAll<T>(this T[] source, T value, int index, int length) => SetAll((G.IList<T>)source, value, index, length) as T[] ?? throw new InvalidCastException("Произошла внутренняя программная или аппаратная ошибка. Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
	public static T[] SetAll<T>(this T[] source, T value, Range range) => SetAll((G.IList<T>)source, value, range) as T[] ?? throw new InvalidCastException("Произошла внутренняя программная или аппаратная ошибка. Повторите попытку позже. Если проблема остается, обратитесь к разработчикам .NStar.");
	public static List<TResult> Shuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) => Shuffle(source, function, random);
	public static List<TResult> Shuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) => Shuffle(source, function, random);
	public static List<T> Shuffle<T>(this G.IReadOnlyList<T> source) => Shuffle(source, random);
	public static G.IEnumerable<T> Sort<T>(this G.IEnumerable<T> source) => E.Order(source);
	public static G.IEnumerable<T> Sort<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) => E.OrderBy(source, function);
	public static G.IEnumerable<T> Sort<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IComparer<TResult> comparer) => E.OrderBy(source, function, comparer);
	public static G.IEnumerable<T> Sort<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, int> compareFunction) => E.OrderBy(source, function, new Comparer<TResult>(compareFunction));
	public static G.IEnumerable<T> SortDesc<T>(this G.IEnumerable<T> source) => E.OrderDescending(source);
	public static G.IEnumerable<T> SortDesc<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) => E.OrderByDescending(source, function);
	public static G.IEnumerable<T> SortDesc<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IComparer<TResult> comparer) => E.OrderByDescending(source, function, comparer);
	public static G.IEnumerable<T> SortDesc<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, int> compareFunction) => E.OrderByDescending(source, function, new Comparer<TResult>(compareFunction));
	public static BitList ToBitList(this ReadOnlySpan<bool> source) => new(source);
	public static BitList ToBitList(this ReadOnlySpan<byte> source) => new(source.ToArray());
	public static BitList ToBitList(this ReadOnlySpan<int> source) => new(source);
	public static BitList ToBitList(this ReadOnlySpan<uint> source) => new(source);
	public static BitList ToBitList(this Span<bool> source) => [.. source];
	public static BitList ToBitList(this Span<byte> source) => new(source.ToArray());
	public static BitList ToBitList(this Span<int> source) => new(source);
	public static BitList ToBitList(this Span<uint> source) => new(source);
	public static BitList ToBitList(this bool[] source) => [.. source];
	public static BitList ToBitList(this byte[] source) => new(source);
	public static BitList ToBitList(this int[] source) => new(source);
	public static BitList ToBitList(this uint[] source) => new(source);
	public static ListHashSet<T> ToHashSet<T>(this ReadOnlySpan<T> source) => new(source);
	public static ListHashSet<T> ToHashSet<T>(this Span<T> source) => new((ReadOnlySpan<T>)source);
	public static ListHashSet<T> ToHashSet<T>(this T[] source) => [.. (G.IList<T>)source];
	public static NList<TResult> ToNList<T, TResult>(this Span<T> source, Func<T, TResult> function) where TResult : unmanaged => ToNList((ReadOnlySpan<T>)source, function);
	public static NList<TResult> ToNList<T, TResult>(this Span<T> source, Func<T, int, TResult> function) where TResult : unmanaged => ToNList((ReadOnlySpan<T>)source, function);
	public static NList<TResult> ToNList<T, TResult>(this T[] source, Func<T, TResult> function) where TResult : unmanaged => ToNList((G.IList<T>)source, function);
	public static NList<TResult> ToNList<T, TResult>(this T[] source, Func<T, int, TResult> function) where TResult : unmanaged => ToNList((G.IList<T>)source, function);
	public static NList<T> ToNList<T>(this ReadOnlySpan<T> source) where T : unmanaged => new(source);
	public static NList<T> ToNList<T>(this Span<T> source) where T : unmanaged => new((ReadOnlySpan<T>)source);
	public static NList<T> ToNList<T>(this T[] source) where T : unmanaged => [.. (G.IList<T>)source];
	public static String ToNString<T>(this Span<T> source, Func<T, char> function) => ToNString((ReadOnlySpan<T>)source, function);
	public static String ToNString<T>(this Span<T> source, Func<T, int, char> function) => ToNString((ReadOnlySpan<T>)source, function);
	public static String ToNString<T>(this T[] source, Func<T, char> function) => ToNString((G.IList<T>)source, function);
	public static String ToNString<T>(this T[] source, Func<T, int, char> function) => ToNString((G.IList<T>)source, function);
	public static String ToNString(this ReadOnlySpan<char> source) => new(source);
	public static String ToNString(this Span<char> source) => new((ReadOnlySpan<char>)source);
	public static String ToNString(this char[] source) => new((G.IList<char>)source);
	public static string ToString<T>(this ReadOnlySpan<T> source, Func<T, char> function) => new(ToArray(source, function));
	public static string ToString<T>(this Span<T> source, Func<T, char> function) => new(ToArray((ReadOnlySpan<T>)source, function));
	public static string ToString<T>(this T[] source, Func<T, char> function) => new(RedStarLinq.ToArray((G.IList<T>)source, function));
	public static string ToString<T>(this ReadOnlySpan<T> source, Func<T, int, char> function) => new(ToArray(source, function));
	public static string ToString<T>(this Span<T> source, Func<T, int, char> function) => new(ToArray((ReadOnlySpan<T>)source, function));
	public static string ToString<T>(this T[] source, Func<T, int, char> function) => new(RedStarLinq.ToArray((G.IList<T>)source, function));
	public static string ToString(this ReadOnlySpan<char> source) => new(source.ToArray());
	public static string ToString(this Span<char> source) => new((ReadOnlySpan<char>)source.ToArray());
	public static string ToString(this char[] source) => new(source);
	public static List<List<T>> Transpose<T>(this List<List<T>> source, bool widen = false) => List<T>.Transpose(source, widen);
	public static List<NList<T>> Transpose<T>(this List<NList<T>> source, bool widen = false) where T : unmanaged => NList<T>.Transpose(source, widen);
	public static G.IEnumerable<T> Union<T>(this G.IEnumerable<T> source, G.IEnumerable<T> source2) => E.Union(source, source2);
	public static G.IEnumerable<T> Union<T>(this G.IEnumerable<T> source, G.IEnumerable<T> source2, G.IEqualityComparer<T> comparer) => E.Union(source, source2, comparer);
	[Obsolete("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в последовательности, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.", true)]
	public static List<NGroup<T, T>> NGroup<T>(this G.IEnumerable<T> source) where T : unmanaged => throw new NotSupportedException("Этот метод не имеет смысла, так как без использования функций сравнения или вычисления ключа все элементы в каждой группе будут в точности одинаковыми, и такая группировка будет впустую расходовать память. Если вы хотите узнать количество вхождений каждого элемента в последовательности, используйте экстент FrequencyTable(). Если вы по ошибке не добавили функцию сравнения или функцию ключа, добавьте их.");
	public static NList<TResult> NShuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where TResult : unmanaged => NShuffle(source, function, random);
	public static NList<TResult> NShuffle<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where TResult : unmanaged => NShuffle(source, function, random);
	public static NList<T> NShuffle<T>(this G.IEnumerable<T> source) where T : unmanaged => NShuffle(source, random);
	public static List<T> PFill<T>(int length, Func<int, T> function) => PFill(function, length);
	public static T[] PFillArray<T>(int length, Func<int, T> function) => PFillArray(function, length);
	public static NList<T> PNFill<T>(int length, Func<int, T> function) where T : unmanaged => PNFill(function, length);
}
