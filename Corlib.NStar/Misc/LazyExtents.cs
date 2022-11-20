using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Corlib.NStar;

public record struct LazyElement<T>(bool Bool, T Value)
{
	public static implicit operator LazyElement<T>(T x) => new(true, x);
	public static implicit operator LazyElement<T>((bool Bool, T Value) x) => new(x.Bool, x.Value);
}

public class LazyEnumerable<TSource, TInternal, TResult> : IEnumerable<TResult>
{
	private protected readonly IEnumerator<TSource> en;
	private protected readonly TInternal @internal;
	private protected readonly Func<TSource, TInternal, LazyElement<TResult>> selector;

	public LazyEnumerable(IEnumerator<TSource> en, TInternal @internal, Func<TSource, TInternal, LazyElement<TResult>> selector)
	{
		this.en = en;
		this.@internal = @internal;
		this.selector = selector;
	}

	public LazyEnumerable(IEnumerable<TSource> en, TInternal @internal, Func<TSource, TInternal, LazyElement<TResult>> selector)
	{
		this.en = en.GetEnumerator();
		this.@internal = @internal;
		this.selector = selector;
	}

	public IEnumerator<TResult> GetEnumerator() => new Enumerator(this);

	IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

	public struct Enumerator : IEnumerator<TResult>
	{
		private readonly LazyEnumerable<TSource, TInternal, TResult> collection;

		public Enumerator(LazyEnumerable<TSource, TInternal, TResult> collection) => this.collection = collection;

		public TResult Current { get; private set; } = default!;

		object IEnumerator.Current => Current!;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			while (true)
			{
				bool result = collection.en.MoveNext();
				if (!result)
				{
					Current = default!;
					return false;
				}
				var (Bool, Value) = collection.selector(collection.en.Current, collection.@internal);
				if (Bool)
				{
					Current = Value;
					return true;
				}
			}
		}

		public void Reset()
		{
			collection.en.Reset();
			Current = default!;
		}
	}
}

public class LazyCollection<TSource, TInternal, TResult> : LazyEnumerable<TSource, TInternal, TResult>, IReadOnlyCollection<TResult>
{
	public LazyCollection(IEnumerable<TSource> source, TInternal @internal, Func<TSource, TInternal, LazyElement<TResult>> selector) : this(source, @internal, selector, source.TryGetCountEasily(out int count) ? count : -1)
	{
	}

	public LazyCollection(IEnumerable<TSource> source, TInternal @internal, Func<TSource, TInternal, LazyElement<TResult>> selector, int length) : base(source, @internal, selector) => Length = length;

	public LazyCollection(IEnumerable<TSource> source, TInternal @internal, Func<TSource, TInternal, LazyElement<TResult>> selector, Func<int, int> lengthSelector) : this(source, @internal, selector, source.TryGetCountEasily(out int count) ? lengthSelector(count) : -1)
	{
	}

	public virtual int Length { get; private set; }
}

public static class LazyExtents
{
	public static (LazyCollection<TSource, int, TResult>, LazyCollection<TSource, int, TResult2>) BreakLazy<TSource, TResult, TResult2>(this IEnumerable<TSource> source, Func<TSource, TResult> function, Func<TSource, TResult2> function2) => (new(source, 0, (item, t) => function(item)), new(source, 0, (item, t) => function2(item)));
	public static (LazyCollection<TSource, int, TResult>, LazyCollection<TSource, int, TResult2>) BreakLazy<TSource, TResult, TResult2>(this IEnumerable<TSource> source, Func<TSource, int, TResult> function, Func<TSource, int, TResult2> function2) => (new(source, CreateVar(0, out int i), (item, t) => function(item, i)), new(source, 0, (item, t) => function2(item, i++)));
	public static (LazyCollection<(TSource, TSource2), int, TSource>, LazyCollection<(TSource, TSource2), int, TSource2>) BreakLazy<TSource, TSource2>(this IEnumerable<(TSource, TSource2)> source) => (new(source, 0, (item, t) => item.Item1), new(source, 0, (item, t) => item.Item2));
	public static LazyCollection<TSource, int, (TResult, TResult2)> BreakLazy<TSource, TResult, TResult2>(this IEnumerable<TSource> source, Func<TSource, (TResult, TResult2)> function) => new(source, 0, (item, t) => function(item));
	public static LazyCollection<TSource, int, (TResult, TResult2)> BreakLazy<TSource, TResult, TResult2>(this IEnumerable<TSource> source, Func<TSource, int, (TResult, TResult2)> function) => new(source, CreateVar(0, out int i), (item, t) => function(item, i++));
	public static (LazyCollection<TSource, int, TResult>, LazyCollection<TSource, int, TResult2>, LazyCollection<TSource, int, TResult3>) BreakLazy<TSource, TResult, TResult2, TResult3>(this IEnumerable<TSource> source, Func<TSource, TResult> function, Func<TSource, TResult2> function2, Func<TSource, TResult3> function3) => (new(source, 0, (item, t) => function(item)), new(source, 0, (item, t) => function2(item)), new(source, 0, (item, t) => function3(item)));
	public static (LazyCollection<TSource, int, TResult>, LazyCollection<TSource, int, TResult2>, LazyCollection<TSource, int, TResult3>) BreakLazy<TSource, TResult, TResult2, TResult3>(this IEnumerable<TSource> source, Func<TSource, int, TResult> function, Func<TSource, int, TResult2> function2, Func<TSource, int, TResult3> function3) => (new(source, CreateVar(0, out int i), (item, t) => function(item, i)), new(source, 0, (item, t) => function2(item, i)), new(source, 0, (item, t) => function3(item, i++)));
	public static (LazyCollection<(TSource, TSource2, TSource3), int, TSource>, LazyCollection<(TSource, TSource2, TSource3), int, TSource2>, LazyCollection<(TSource, TSource2, TSource3), int, TSource3>) BreakLazy<TSource, TSource2, TSource3>(this IEnumerable<(TSource, TSource2, TSource3)> source) => (new(source, 0, (item, t) => item.Item1), new(source, 0, (item, t) => item.Item2), new(source, 0, (item, t) => item.Item3));
	public static LazyCollection<TSource, int, (TResult, TResult2, TResult3)> BreakLazy<TSource, TResult, TResult2, TResult3>(this IEnumerable<TSource> source, Func<TSource, (TResult, TResult2, TResult3)> function) => new(source, 0, (item, t) => function(item));
	public static LazyCollection<TSource, int, (TResult, TResult2, TResult3)> BreakLazy<TSource, TResult, TResult2, TResult3>(this IEnumerable<TSource> source, Func<TSource, int, (TResult, TResult2, TResult3)> function) => new(source, CreateVar(0, out int i), (item, t) => function(item, i++));
	public static LazyCollection<TSource, (IEnumerator<TSource2>, int), TResult> CombineLazy<TSource, TSource2, TResult>(this IEnumerable<TSource> source, IEnumerable<TSource2> source2, Func<TSource, TSource2, TResult> function) => new(source, (source2.GetEnumerator(), 0), (item, t) => { t.Item1.MoveNext(); return function(item, t.Item1.Current); });
	public static LazyCollection<TSource, (IEnumerator<TSource2>, int), TResult> CombineLazy<TSource, TSource2, TResult>(this IEnumerable<TSource> source, IEnumerable<TSource2> source2, Func<TSource, TSource2, int, TResult> function) => new(source, (source2.GetEnumerator(), CreateVar(0, out int i)), (item, t) => { t.Item1.MoveNext(); return function(item, t.Item1.Current, i++); });
	public static LazyCollection<TSource, (IEnumerator<TSource2>, int), (TSource, TSource2)> CombineLazy<TSource, TSource2>(this IEnumerable<TSource> source, IEnumerable<TSource2> source2) => new(source, (source2.GetEnumerator(), 0), (item, t) => { t.Item1.MoveNext(); return (item, t.Item1.Current); });
	public static LazyCollection<TSource, (IEnumerator<TSource2>, IEnumerator<TSource3>, int), TResult> CombineLazy<TSource, TSource2, TSource3, TResult>(this IEnumerable<TSource> source, IEnumerable<TSource2> source2, IEnumerable<TSource3> source3, Func<TSource, TSource2, TSource3, TResult> function) => new(source, (source2.GetEnumerator(), source3.GetEnumerator(), 0), (item, t) => { t.Item1.MoveNext(); return function(item, t.Item1.Current, t.Item2.Current); });
	public static LazyCollection<TSource, (IEnumerator<TSource2>, IEnumerator<TSource3>, int), TResult> CombineLazy<TSource, TSource2, TSource3, TResult>(this IEnumerable<TSource> source, IEnumerable<TSource2> source2, IEnumerable<TSource3> source3, Func<TSource, TSource2, TSource3, int, TResult> function) => new(source, (source2.GetEnumerator(), source3.GetEnumerator(), CreateVar(0, out int i)), (item, t) => { t.Item1.MoveNext(); return function(item, t.Item1.Current, t.Item2.Current, i++); });
	public static LazyCollection<TSource, (IEnumerator<TSource2>, IEnumerator<TSource3>, int), (TSource, TSource2, TSource3)> CombineLazy<TSource, TSource2, TSource3>(this IEnumerable<TSource> source, IEnumerable<TSource2> source2, IEnumerable<TSource3> source3) => new(source, (source2.GetEnumerator(), source3.GetEnumerator(), 0), (item, t) => { t.Item1.MoveNext(); return (item, t.Item1.Current, t.Item2.Current); });
	public static LazyCollection<TSource, (IEnumerator<TSource2>, int), TResult> CombineLazy<TSource, TSource2, TResult>(this (IEnumerable<TSource>, IEnumerable<TSource2>) source, Func<TSource, TSource2, TResult> function) => CombineLazy(source.Item1, source.Item2, function);
	public static LazyCollection<TSource, (IEnumerator<TSource2>, int), TResult> CombineLazy<TSource, TSource2, TResult>(this (IEnumerable<TSource>, IEnumerable<TSource2>) source, Func<TSource, TSource2, int, TResult> function) => CombineLazy(source.Item1, source.Item2, function);
	public static LazyCollection<TSource, (IEnumerator<TSource2>, int), (TSource, TSource2)> CombineLazy<TSource, TSource2>(this (IEnumerable<TSource>, IEnumerable<TSource2>) source) => CombineLazy(source.Item1, source.Item2);
	public static LazyCollection<TSource, (IEnumerator<TSource2>, IEnumerator<TSource3>, int), TResult> CombineLazy<TSource, TSource2, TSource3, TResult>(this (IEnumerable<TSource>, IEnumerable<TSource2>, IEnumerable<TSource3>) source, Func<TSource, TSource2, TSource3, TResult> function) => CombineLazy(source.Item1, source.Item2, source.Item3, function);
	public static LazyCollection<TSource, (IEnumerator<TSource2>, IEnumerator<TSource3>, int), TResult> CombineLazy<TSource, TSource2, TSource3, TResult>(this (IEnumerable<TSource>, IEnumerable<TSource2>, IEnumerable<TSource3>) source, Func<TSource, TSource2, TSource3, int, TResult> function) => CombineLazy(source.Item1, source.Item2, source.Item3, function);
	public static LazyCollection<TSource, (IEnumerator<TSource2>, IEnumerator<TSource3>, int), (TSource, TSource2, TSource3)> CombineLazy<TSource, TSource2, TSource3>(this (IEnumerable<TSource>, IEnumerable<TSource2>, IEnumerable<TSource3>) source) => CombineLazy(source.Item1, source.Item2, source.Item3);
	public static LazyCollection<TSource, int, TResult> ConvertLazy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> function) => new(source, 0, (item, t) => function(item));
	public static LazyCollection<TSource, int, TResult> ConvertLazy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> function) => new(source, CreateVar(0, out int i), (item, t) => function(item, i++));
	public static LazyCollection<int, int, TResult> FillLazy<TResult>(TResult elem, int count) => new(new Chain(0, count), 0, (_, _) => elem);
	public static LazyCollection<int, int, TResult> FillLazy<TResult>(Func<int, TResult> function, int count) => new(new Chain(0, count), 0, (i, _) => function(i));
	public static LazyCollection<int, int, TResult> FillLazy<TResult>(int count, Func<int, TResult> function) => FillLazy(function, count);
	public static LazyCollection<TSource, int, TSource> FilterLazy<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> function) => new(source, 0, (elem, t) => (function(elem), elem));
	public static LazyCollection<TSource, int, TSource> FilterLazy<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> function) => new(source, CreateVar(0, out int i), (elem, t) => (function(elem, i++), elem));
	public static LazyCollection<TSource, int, TSource> SkipLazy<TSource>(this IEnumerable<TSource> source, int count) => new(source, CreateVar(0, out int i), (elem, t) => (i++ >= count, elem), x => Max(x - count, 0));
	public static LazyCollection<TSource, int, TSource> TakeLazy<TSource>(this IEnumerable<TSource> source, int count) => new(source, CreateVar(0, out int i), (elem, t) => (i++ < count, elem), x => Min(x, count));
}
