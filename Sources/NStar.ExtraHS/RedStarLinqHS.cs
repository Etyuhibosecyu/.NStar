namespace NStar.ExtraHS;

public static class RedStarLinqHS
{
	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = [];
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = [];
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = new(comparer);
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = new(comparer);
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item, i), out var index))
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
		using NListHashSet<T> hs = new(comparer);
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = item, out var index))
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
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = item, out var index))
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
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, T>> result = new(32);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item, i), out var index))
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
		using NListHashSet<T> hs = new(new EComparer<T>(equalFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = item, out var index))
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
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = item, out var index))
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
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, T>> result = new(32);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static List<NGroup<T, TResult>> NGroup<T, TResult>(this G.IEnumerable<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction) where T : unmanaged where TResult : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		using NListHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
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
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			T item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = function(item, i), out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, TResult>> result = new(32);
			TResult f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = function(item, i), out var index))
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
		using NListHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		if (source is NList<T> list)
		{
			var length = list.Length;
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list[i];
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else if (source is T[] array)
		{
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < array.Length; i++)
			{
				item = array[i];
				if (!hs.TryAdd(f = item, out var index))
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
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list2[i];
				if (!hs.TryAdd(f = item, out var index))
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
			List<NGroup<T, T>> result = new(32);
			T f, item;
			for (var i = 0; i < length; i++)
			{
				item = list3[i];
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
			}
			result.TrimExcess();
			return result;
		}
		else
		{
			List<NGroup<T, T>> result = new(32);
			T f;
			var i = 0;
			foreach (var item in source)
			{
				if (!hs.TryAdd(f = item, out var index))
					result[index].Add(item);
				else
					result.Add(new(32, item, f));
				i++;
			}
			result.TrimExcess();
			return result;
		}
	}

	public static NListHashSet<T> ToNHashSet<T>(this G.IEnumerable<T> source) where T : unmanaged => [.. source];
}
