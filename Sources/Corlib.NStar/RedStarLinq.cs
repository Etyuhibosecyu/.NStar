using System.Reflection;

namespace Corlib.NStar;

public static class RedStarLinq
{
	public static bool All<T>(this IEnumerable<T> source, Func<T, bool> function) => Enumerable.All(source, function);
	public static bool Any<T>(this IEnumerable<T> source) => Enumerable.Any(source);
	public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> function) => Enumerable.Any(source, function);
	public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<T> source) => source;
	public static Span<T> AsSpan<T>(this IEnumerable<T> source) => source is BaseIndexable<T> collection ? collection.AsSpan() : source is T[] array ? MemoryExtensions.AsSpan(array) : List<T>.ReturnOrConstruct(source).AsSpan();
	public static Span<T> AsSpan<T>(this IEnumerable<T> source, Index index) => source is BaseIndexable<T> collection ? collection.AsSpan(index) : source is T[] array ? MemoryExtensions.AsSpan(array, index) : List<T>.ReturnOrConstruct(source).AsSpan(index);
	public static Span<T> AsSpan<T>(this IEnumerable<T> source, int index) => source is BaseIndexable<T> collection ? collection.AsSpan(index) : source is T[] array ? MemoryExtensions.AsSpan(array, index) : List<T>.ReturnOrConstruct(source).AsSpan(index);
	public static Span<T> AsSpan<T>(this IEnumerable<T> source, int index, int length) => source is BaseIndexable<T> collection ? collection.AsSpan(index, length) : source is T[] array ? MemoryExtensions.AsSpan(array, index, length) : List<T>.ReturnOrConstruct(source).AsSpan(index, length);
	public static Span<T> AsSpan<T>(this IEnumerable<T> source, Range range) => AsSpan(source)[range];

	public static (List<TResult>, List<TResult2>) Break<T, TResult, TResult2>(this IEnumerable<T> source, Func<T, TResult> function, Func<T, TResult2> function2)
	{
		ArgumentNullException.ThrowIfNull(function);
		ArgumentNullException.ThrowIfNull(function2);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = EmptyList<TResult>(length);
			var result2 = EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item);
				result2[i] = function2(item);
			}
			return (result, result2);
		}
		else if (source is T[] array)
		{
			var length = array.Length;
			var result = EmptyList<TResult>(length);
			var result2 = EmptyList<TResult2>(length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array[i];
				result[i] = function(item);
				result2[i] = function2(item);
			}
			return (result, result2);
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = EmptyList<TResult>(length);
			var result2 = EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = function(item);
				result2[i] = function2(item);
			}
			return (result, result2);
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = EmptyList<TResult>(length);
			var result2 = EmptyList<TResult2>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item);
				result2[i] = function2(item);
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
				result.Add(function(item));
				result2.Add(function2(item));
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			return (result, result2);
		}
	}

	public static (List<T>, List<T2>) Break<T, T2>(this IEnumerable<(T, T2)> source)
	{
		if (source is List<(T, T2)> list)
		{
			var length = list.Length;
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
				(result[i], result2[i]) = list[i];
			return (result, result2);
		}
		else if (source is (T, T2)[] array)
		{
			var length = array.Length;
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < array.Length; i++)
				(result[i], result2[i]) = array[i];
			return (result, result2);
		}
		else if (source is G.IList<(T, T2)> list2)
		{
			var length = list2.Count;
			List<T> result = new(length);
			List<T2> result2 = new(length);
			for (var i = 0; i < length; i++)
				(result[i], result2[i]) = list2[i];
			return (result, result2);
		}
		else
		{
			List<T> result = new(source.TryGetLengthEasily(out var length) ? length : length = 1024);
			List<T2> result2 = new(length);
			var i = 0;
			foreach (var item in source)
			{
				result.Add(item.Item1);
				result2.Add(item.Item2);
				i++;
			}
			result.Resize(i);
			result2.Resize(i);
			return (result, result2);
		}
	}

	private class ConvertList<T, TResult> : BaseIndexable<TResult, ConvertList<T, TResult>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly Func<T, TResult> function;

		public ConvertList()
		{
			source = [];
			function = x => default!;
		}

		public ConvertList(G.IReadOnlyList<T> source, Func<T, TResult> function)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(function);
			this.source = source;
			this.function = function;
			_size = source.Count;
		}

		public override Span<TResult> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, TResult[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override TResult GetInternal(int index, bool invoke = true) => function(source[index]);

		protected override ConvertList<T, TResult> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), function);

		protected override Slice<TResult> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(TResult item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (function(source[i])?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(TResult item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (function(source[i])?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	private class ConvertListInt<T, TResult> : BaseIndexable<TResult, ConvertListInt<T, TResult>>
	{
		private readonly G.IReadOnlyList<T> source;
		private readonly Func<T, int, TResult> function;

		public ConvertListInt()
		{
			source = [];
			function = (x, index) => default!;
		}

		public ConvertListInt(G.IReadOnlyList<T> source, Func<T, int, TResult> function)
		{
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(function);
			this.source = source;
			this.function = function;
			_size = source.Count;
		}

		public override Span<TResult> AsSpan(int index, int length) => GetSlice(index, length).ToArray().AsSpan();

		protected override void CopyToInternal(int index, TResult[] array, int arrayIndex, int length)
		{
			for (var i = 0; i < length; i++)
				array[arrayIndex++] = GetInternal(index++);
		}

		public override void Dispose() => GC.SuppressFinalize(this);

		protected override TResult GetInternal(int index, bool invoke = true) => function(source[index], index);

		protected override ConvertListInt<T, TResult> GetRangeInternal(int index, int length) => new(source.GetROLSlice(index, length), function);

		protected override Slice<TResult> GetSliceInternal(int index, int length) => new(this, index, length);

		protected override int IndexOfInternal(TResult item, int index, int length)
		{
			for (var i = index; i < index + length; i++)
				if (function(source[i], i)?.Equals(item) ?? item == null)
					return i;
			return -1;
		}

		protected override int LastIndexOfInternal(TResult item, int index, int length)
		{
			var endIndex = index - length + 1;
			for (var i = index; i >= endIndex; i++)
				if (function(source[i], i)?.Equals(item) ?? item == null)
					return i;
			return -1;
		}
	}

	public static Slice<TResult> Convert<T, TResult>(this G.IReadOnlyList<T> source, Func<T, TResult> function) => new ConvertList<T, TResult>(source, function).GetSlice();

	public static Slice<TResult> Convert<T, TResult>(this G.IReadOnlyList<T> source, Func<T, int, TResult> function) => new ConvertListInt<T, TResult>(source, function).GetSlice();

	public static IEnumerable<TResult> Convert<T, TResult>(this IEnumerable<T> source, Func<T, TResult> function) => Enumerable.Select(source, function);

	public static IEnumerable<TResult> Convert<T, TResult>(this IEnumerable<T> source, Func<T, int, TResult> function) => Enumerable.Select(source, function);

	public static List<T> EmptyList<T>(int length) => List<T>.EmptyList(length);

	public static bool Equals<T, T2>(this IEnumerable<T> source, IEnumerable<T2> source2, Func<T, T2, bool> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list && source2 is List<T2> list2)
		{
			var length = Min(list.Length, list2.Length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				var item2 = list2[i];
				if (!function(item, item2))
					return false;
			}
			return list.Length == list2.Length;
		}
		else if (source is T[] array && source2 is T2[] array2)
		{
			if (array.Length != array2.Length)
				return false;
			for (var i = 0; i < array.Length; i++)
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
			if (list2_.Count != list2_2.Count)
				return false;
			var length = list2_.Count;
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
			if (list3_.Count != list3_2.Count)
				return false;
			var length = list3_.Count;
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
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			while (en.MoveNext())
			{
				if (!en2.MoveNext())
					return false;
				var item = en.Current;
				var item2 = en2.Current;
				if (!function(item, item2))
					return false;
			}
			return !en2.MoveNext();
		}
	}

	public static bool Equals<T, T2>(this IEnumerable<T> source, IEnumerable<T2> source2)
	{
		if (source is List<T> list && source2 is List<T2> list2)
		{
			if (list.Length != list2.Length)
				return false;
			var length = list.Length;
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
			if (array.Length != array2.Length)
				return false;
			for (var i = 0; i < array.Length; i++)
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
			if (list2_.Count != list2_2.Count)
				return false;
			var length = list2_.Count;
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
			if (list3_.Count != list3_2.Count)
				return false;
			var length = list3_.Count;
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
			using var en = source.GetEnumerator();
			using var en2 = source2.GetEnumerator();
			while (en.MoveNext())
			{
				if (!en2.MoveNext())
					return false;
				var item = en.Current;
				var item2 = en2.Current;
				if (!(item?.Equals(item2) ?? item2 == null))
					return false;
			}
			return !en2.MoveNext();
		}
	}

	public static List<T> Fill<T>(T elem, int length)
	{
		var result = EmptyList<T>(length);
		for (var i = 0; i < length; i++)
			result[i] = elem;
		return result;
	}

	public static List<T> Fill<T>(Func<int, T> function, int length)
	{
		ArgumentNullException.ThrowIfNull(function);
		var result = EmptyList<T>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(i);
		return result;
	}

	public static List<T> Fill<T>(int length, Func<int, T> function) => Fill(function, length);

	public static T[] FillArray<T>(T elem, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		var result = new T[length];
		for (var i = 0; i < length; i++)
			result[i] = elem;
		return result;
	}

	public static T[] FillArray<T>(Func<int, T> function, int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		ArgumentNullException.ThrowIfNull(function);
		var result = new T[length];
		for (var i = 0; i < length; i++)
			result[i] = function(i);
		return result;
	}

	public static T[] FillArray<T>(int length, Func<int, T> function) => FillArray(function, length);

	public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> function) => Enumerable.Where(source, function);
	public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, int, bool> function) => Enumerable.Where(source, function);

	public static Slice<T> GetSlice<T>(this G.IList<T> source) => new(source);
	public static Slice<T> GetSlice<T>(this G.IList<T> source, Index index) => new(source, index);
	public static Slice<T> GetSlice<T>(this G.IList<T> source, int index) => new(source, index);
	public static Slice<T> GetSlice<T>(this G.IList<T> source, int index, int length) => new(source, index, length);
	public static Slice<T> GetSlice<T>(this G.IList<T> source, Range range) => new(source, range);
	public static Slice<T> GetROLSlice<T>(this G.IReadOnlyList<T> source) => new(source);
	public static Slice<T> GetROLSlice<T>(this G.IReadOnlyList<T> source, Index index) => new(source, index);
	public static Slice<T> GetROLSlice<T>(this G.IReadOnlyList<T> source, int index) => new(source, index);
	public static Slice<T> GetROLSlice<T>(this G.IReadOnlyList<T> source, int index, int length) => new(source, index, length);
	public static Slice<T> GetROLSlice<T>(this G.IReadOnlyList<T> source, Range range) => new(source, range);

	public static int Length<T>(this IEnumerable<T> source)
	{
		if (TryGetLengthEasily(source, out var length))
			return length;
		else
		{
			var n = 0;
			using var en = source.GetEnumerator();
			while (en.MoveNext()) n++;
			return n;
		}
	}

	public static NList<T> NEmptyList<T>(int length) where T : unmanaged => NList<T>.EmptyList(length);

	public static NList<T> NFill<T>(T elem, int length) where T : unmanaged
	{
		var result = NEmptyList<T>(length);
		for (var i = 0; i < length; i++)
			result[i] = elem;
		return result;
	}

	public static NList<T> NFill<T>(Func<int, T> function, int length) where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(function);
		var result = NEmptyList<T>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(i);
		return result;
	}

	public static NList<T> NFill<T>(int length, Func<int, T> function) where T : unmanaged => NFill(function, length);

	public static T Random<T>(this G.IReadOnlyList<T> source) => source[random.Next(source.Count)];

	public static T Random<T>(this G.IReadOnlyList<T> source, Random randomObj) => source[randomObj.Next(source.Count)];

	public static TResult[] ToArray<T, TResult>(this IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = new TResult[length];
			for (var i = 0; i < length; i++)
				result[i] = function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			var result = new TResult[array.Length];
			for (var i = 0; i < array.Length; i++)
				result[i] = function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = new TResult[length];
			for (var i = 0; i < length; i++)
				result[i] = function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = new TResult[length];
			for (var i = 0; i < length; i++)
				result[i] = function(list3[i]);
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = new TResult[length];
			var i = 0;
			foreach (var item in source)
			{
				result[i] = function(item);
				i++;
			}
			return result;
		}
		else
			return ToArray(new List<T>(source), function);
	}

	public static TResult[] ToArray<T, TResult>(this IEnumerable<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = new TResult[length];
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = new TResult[array.Length];
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
			var result = new TResult[length];
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
			var result = new TResult[length];
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = new TResult[length];
			var i = 0;
			foreach (var item in source)
			{
				result[i] = function(item, i);
				i++;
			}
			return result;
		}
		else
			return ToArray(new List<T>(source), function);
	}

	public static T[] ToArray<T>(this IEnumerable<T> source)
	{
		if (source is List<T> list)
			return [.. list];
		else if (source is T[] array)
		{
			var result = new T[array.Length];
			Array.Copy(array, result, array.Length);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = new T[length];
			for (var i = 0; i < length; i++)
			{
				var item = list2[i];
				result[i] = item;
			}
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = new T[length];
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = item;
			}
			return result;
		}
		else if (source.TryGetLengthEasily(out var length))
		{
			var result = new T[length];
			var i = 0;
			foreach (var item in source)
			{
				result[i] = item;
				i++;
			}
			return result;
		}
		else
			return [.. source];
	}

	public static BitList ToBitList(this IEnumerable<bool> source) => new(source);
	public static BitList ToBitList(this IEnumerable<byte> source) => new(source);
	public static BitList ToBitList(this IEnumerable<int> source) => new(source);
	public static BitList ToBitList(this IEnumerable<uint> source) => new(source);
	public static ListHashSet<T> ToHashSet<T>(this IEnumerable<T> source) => [.. source];

	public static List<T> ToList<T>(this IEnumerable<T> source) => List<T>.ReturnOrConstruct(source);

	public static List<TResult> ToList<T, TResult>(this IEnumerable<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list[i]);
			return result;
		}
		else if (source is T[] array)
		{
			var result = EmptyList<TResult>(array.Length);
			for (var i = 0; i < array.Length; i++)
				result[i] = function(array[i]);
			return result;
		}
		else if (source is G.IList<T> list2)
		{
			var length = list2.Count;
			var result = EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list2[i]);
			return result;
		}
		else if (source is G.IReadOnlyList<T> list3)
		{
			var length = list3.Count;
			var result = EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
				result[i] = function(list3[i]);
			return result;
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : 32);
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

	public static List<TResult> ToList<T, TResult>(this IEnumerable<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		if (source is List<T> list)
		{
			var length = list.Length;
			var result = EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else if (source is T[] array)
		{
			var result = EmptyList<TResult>(array.Length);
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
			var result = EmptyList<TResult>(length);
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
			var result = EmptyList<TResult>(length);
			for (var i = 0; i < length; i++)
			{
				var item = list3[i];
				result[i] = function(item, i);
			}
			return result;
		}
		else
		{
			List<TResult> result = new(source.TryGetLengthEasily(out var length) ? length : 32);
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

	public static List<TResult> ToList<T, TResult>(this ReadOnlySpan<T> source, Func<T, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i]);
		return result;
	}

	public static List<TResult> ToList<T, TResult>(this ReadOnlySpan<T> source, Func<T, int, TResult> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		var length = source.Length;
		var result = EmptyList<TResult>(length);
		for (var i = 0; i < length; i++)
			result[i] = function(source[i], i);
		return result;
	}

	public static List<TResult> ToList<T, TResult>(this Span<T> source, Func<T, TResult> function) => ToList((ReadOnlySpan<T>)source, function);
	public static List<TResult> ToList<T, TResult>(this Span<T> source, Func<T, int, TResult> function) => ToList((ReadOnlySpan<T>)source, function);
	public static List<TResult> ToList<T, TResult>(this T[] source, Func<T, TResult> function) => ToList((ReadOnlySpan<T>)source.AsSpan(), function);
	public static List<TResult> ToList<T, TResult>(this T[] source, Func<T, int, TResult> function) => ToList((ReadOnlySpan<T>)source.AsSpan(), function);
	public static List<T> ToList<T>(this ReadOnlySpan<T> source) => new(source);
	public static List<T> ToList<T>(this Span<T> source) => new((ReadOnlySpan<T>)source);
	public static List<T> ToList<T>(this T[] source) => [.. (G.IList<T>)source];

	public static NList<T> ToNList<T>(this IEnumerable<T> source) where T : unmanaged => NList<T>.ReturnOrConstruct(source);
	public static String ToNString(this IEnumerable<char> source) => new(source);
	public static string ToString<T>(this IEnumerable<T> source, Func<T, char> function) => new(ToArray(source, function));
	public static string ToString<T>(this IEnumerable<T> source, Func<T, int, char> function) => new(ToArray(source, function));
	public static string ToString(this IEnumerable<char> source) => new(source.ToArray());

	public static bool TryGetLengthEasily<T>(this IEnumerable<T> source, out int length)
	{
		try
		{
			if (source is G.ICollection<T> col)
			{
				length = col.Count;
				return length >= 0;
			}
			else if (source is G.IReadOnlyCollection<T> col2)
			{
				length = col2.Count;
				return length >= 0;
			}
			else if (source is System.Collections.ICollection col3)
			{
				length = col3.Count;
				return length >= 0;
			}
			else if (source is string s)
			{
				length = s.Length;
				return length >= 0;
			}
			else if (CreateVar(Assembly.Load("System.Linq").GetType("System.Linq.Enumerable+Iterator`1")
				?.MakeGenericType(typeof(T))
				?? throw new TypeLoadException("Не удалось загрузить тип-образец для сравнения с типом коллекции." +
				" Обратитесь к разработчикам .NStar."), out var targetType).IsInstanceOfType(source)
				&& targetType.GetMethod("GetCount")?.Invoke(source, [true]) is int n)
			{
				length = n;
				return length >= 0;
			}
		}
		catch
		{
		}
		length = -1;
		return false;
	}

	public static bool TryGetLengthEasily(this IEnumerable source, out int length)
	{
		try
		{
			if (source is System.Collections.ICollection col)
			{
				length = col.Count;
				return length >= 0;
			}
			else if (source is string s)
			{
				length = s.Length;
				return length >= 0;
			}
		}
		catch
		{
		}
		length = -1;
		return false;
	}

	public static TResult Wrap<T, TResult>(this T source, Func<T, TResult> function) => function(source);
}
