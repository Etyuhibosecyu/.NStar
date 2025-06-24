using System.Diagnostics.CodeAnalysis;

namespace NStar.ParallelHS;

[Serializable]
public class FakeIndexesException : Exception
{
	public FakeIndexesException() : this("Внимание! Вы пытаетесь получить или установить элемент по индексу," +
		" но он является фейковым. Вы можете получить недействительный элемент, либо же элемент, действительный \"номер\"" +
		" которого в коллекции существенно отличается от указанного вами индекса." +
		" Это исключение не прерывает работу программы, а служит только для оповещения. Нажмите F5 для продолжения.") { }

	public FakeIndexesException(string? message) : base(message) { }

	public FakeIndexesException(string? message, Exception? innerException) : base(message, innerException) { }
}

public static class RedStarLinqParallel
{
	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = [];
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	[Experimental("CS9216")]
	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = [];
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T>(this G.IReadOnlyList<T> source)
	{
		ParallelHashSet<T> hs = [];
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(item, out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(comparer);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	[Experimental("CS9216")]
	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(comparer);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T>(this G.IReadOnlyList<T> source, G.IEqualityComparer<T> comparer)
	{
		ParallelHashSet<T> hs = new(comparer);
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(item, out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	[Experimental("CS9216")]
	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T>(this G.IReadOnlyList<T> source, Func<T, T, bool> equalFunction)
	{
		ParallelHashSet<T> hs = new(new EComparer<T>(equalFunction));
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(item, out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	[Experimental("CS9216")]
	public static List<T> PRemoveDoubles<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(function(item, i), out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static List<T> PRemoveDoubles<T>(this G.IReadOnlyList<T> source, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ParallelHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = source.Count;
		var result = RedStarLinq.EmptyList<T>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			if (hs.TryAdd(item, out var index))
				result[index] = item;
		});
		result.Resize(hs.Length);
		result.TrimExcess();
		return result;
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = [];
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = [];
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item, i), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2)
	{
		ParallelHashSet<T> hs = [];
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(item, out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(comparer);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, int, TResult> function, G.IEqualityComparer<TResult> comparer)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(comparer);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item, i), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, G.IEqualityComparer<T> comparer)
	{
		ParallelHashSet<T> hs = new(comparer);
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(item, out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction));
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item, i), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T, bool> equalFunction)
	{
		ParallelHashSet<T> hs = new(new EComparer<T>(equalFunction));
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(item, out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	[Experimental("CS9216")]
	public static (List<T>, List<T2>) PRemoveDoubles<T, T2, TResult>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, int, TResult> function, Func<TResult, TResult, bool> equalFunction, Func<TResult, int> hashCodeFunction)
	{
		ArgumentNullException.ThrowIfNull(function);
		ParallelHashSet<TResult> hs = new(new EComparer<TResult>(equalFunction, hashCodeFunction));
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(function(item, i), out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static (List<T>, List<T2>) PRemoveDoubles<T, T2>(this G.IReadOnlyList<T> source, G.IReadOnlyList<T2> source2, Func<T, T, bool> equalFunction, Func<T, int> hashCodeFunction)
	{
		ParallelHashSet<T> hs = new(new EComparer<T>(equalFunction, hashCodeFunction));
		var length = Min(source.Count, source2.Count);
		var result = RedStarLinq.EmptyList<T>(length);
		var result2 = RedStarLinq.EmptyList<T2>(length);
		Parallel.For(0, length, i =>
		{
			var item = source[i];
			var item2 = source2[i];
			if (hs.TryAdd(item, out var index))
			{
				result[index] = item;
				result2[index] = item2;
			}
		});
		result.Resize(hs.Length);
		result2.Resize(hs.Length);
		result.TrimExcess();
		result2.TrimExcess();
		return (result, result2);
	}

	public static ParallelHashSet<T> ToParallelHashSet<T>(this G.IEnumerable<T> source) => [.. source];
	public static ParallelHashSet<T> ToParallelHashSet<T>(this ReadOnlySpan<T> source) => new(source);
	public static ParallelHashSet<T> ToParallelHashSet<T>(this Span<T> source) => new((ReadOnlySpan<T>)source);
	public static ParallelHashSet<T> ToParallelHashSet<T>(this T[] source) => [.. (G.IList<T>)source];
}
