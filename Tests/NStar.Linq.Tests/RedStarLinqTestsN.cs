using NStar.ExtraHS;

namespace NStar.Linq.Tests;

[TestClass]
public class RedStarLinqTestsN
{
	public static void Test(Action<G.IEnumerable<(char, char, char)>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			char @char;
			var array = i == 0 ? [.. nList] : RedStarLinq.FillArray(random.Next(1001), _ =>
				(@char = (char)random.Next('A', 'Z' + 1), @char, @char));
			G.IEnumerable<(char, char, char)> a = RedStarLinq.ToList(array);
			action(a);
			a = RedStarLinq.ToArray(array);
			action(a);
			a = E.ToList(array);
			action(a);
			a = array.ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
			action(a);
			a = array.Prepend(('X', 'X', 'X'));
			action(a);
			a = nEnumerable;
			action(a);
			a = nEnumerable2;
			action(a);
			a = E.SkipWhile(array, _ => random.Next(10) != -1);
			action(a);
		}
	}

	public static void Test2(Action<G.IReadOnlyList<(char, char, char)>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			char @char;
			var array = i == 0 ? [.. nList] : RedStarLinq.FillArray(random.Next(1001), _ =>
				(@char = (char)random.Next('A', 'Z' + 1), @char, @char));
			G.IReadOnlyList<(char, char, char)> a = RedStarLinq.ToList(array);
			action(a);
			a = RedStarLinq.ToArray(array);
			action(a);
			a = E.ToList(array);
			action(a);
			a = array.ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
			action(a);
			a = array.Prepend(('X', 'X', 'X'));
			action(a);
		}
	}

	public static void TestSpan(Action<ReadOnlySpan<(char, char, char)>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(1001), _ => ((char, char, char))new String((char)random.Next('A', 'Z' + 1), 3));
			action(array);
			action(array.AsSpan());
			action((ReadOnlySpan<(char, char, char)>)array.AsSpan());
		}
	}

	[TestMethod]
	public void TestNBreak() => Test(a =>
	{
		var c = a.NBreak(x => x.Item1, x => (x.Item2, x.Item3));
		var d = (E.Select(a, x => x.Item1), E.Select(a, x => (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.NBreak(x => (x.Item1, (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = E.Select(a, x => (x.Item1, (x.Item2, x.Item3))).NBreak();
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), char>)null!, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak(x => x.Item1, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), (char, (char, char, char))>)null!));
		c = a.NBreak((x, index) => (char)(x.Item1 + index), (x, index) => (x.Item2, (char)(x.Item3 + index)));
		d = (E.Select(a, (x, index) => (char)(x.Item1 + index)), E.Select(a, (x, index) => (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.NBreak((x, index) => ((char)(x.Item1 + index), (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((x, index) => (char)(x.Item1 + index), (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), int, (char, (char, char, char))>)null!));
		var c2 = a.NBreak(x => x.Item1, x => x.Item3, x => (x.Item2, x.Item3));
		var d2 = (E.Select(a, x => x.Item1), E.Select(a, x => x.Item3), E.Select(a, x => (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.NBreak(x => (x.Item1, x.Item3, (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = E.Select(a, x => (x.Item1, x.Item3, (x.Item2, x.Item3))).NBreak();
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), char>)null!, (Func<(char, char, char), char>)null!, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak(x => x.Item1, (Func<(char, char, char), char>)null!, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak(x => x.Item1, x => x.Item3, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), (char, char, (char, char, char))>)null!));
		c2 = a.NBreak((x, index) => (char)(x.Item1 + index), (x, index) => (char)(x.Item3 * index + 5), (x, index) => (x.Item2, (char)(x.Item3 + index)));
		d2 = (E.Select(a, (x, index) => (char)(x.Item1 + index)), E.Select(a, (x, index) => (char)(x.Item3 * index + 5)), E.Select(a, (x, index) => (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.NBreak((x, index) => ((char)(x.Item1 + index), (char)(x.Item3 * index + 5), (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((x, index) => (char)(x.Item1 + index), (Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((x, index) => (char)(x.Item1 + index), (x, index) => (char)(x.Item3 * index + 5), (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NBreak((Func<(char, char, char), int, (char, char, (char, char, char))>)null!));
	});

	[TestMethod]
	public void TestNBreakSpan() => TestSpan(a =>
	{
		var a2 = a.ToArray();
		var c = a.NBreak(x => x.Item1, x => (x.Item2, x.Item3));
		var d = (E.Select(a2, x => x.Item1), E.Select(a2, x => (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.NBreak(x => (x.Item1, (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = E.Select(a2, x => (x.Item1, (x.Item2, x.Item3))).NBreak();
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.NBreak((x, index) => (char)(x.Item1 + index), (x, index) => (x.Item2, (char)(x.Item3 + index)));
		d = (E.Select(a2, (x, index) => (char)(x.Item1 + index)), E.Select(a2, (x, index) => (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.NBreak((x, index) => ((char)(x.Item1 + index), (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		var c2 = a.NBreak(x => x.Item1, x => x.Item3, x => (x.Item2, x.Item3));
		var d2 = (E.Select(a2, x => x.Item1), E.Select(a2, x => x.Item3), E.Select(a2, x => (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.NBreak(x => (x.Item1, x.Item3, (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = E.Select(a2, x => (x.Item1, x.Item3, (x.Item2, x.Item3))).NBreak();
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.NBreak((x, index) => (char)(x.Item1 + index), (x, index) => (char)(x.Item3 * index + 5), (x, index) => (x.Item2, (char)(x.Item3 + index)));
		d2 = (E.Select(a2, (x, index) => (char)(x.Item1 + index)), E.Select(a2, (x, index) => (char)(x.Item3 * index + 5)), E.Select(a2, (x, index) => (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.NBreak((x, index) => ((char)(x.Item1 + index), (char)(x.Item3 * index + 5), (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
	});

	[TestMethod]
	public void TestNCombine() => Test(a =>
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		G.IEnumerable<(char, char, char)> b = RedStarLinq.ToList(E.Skip(nList2, 1));
		ProcessB(a, b);
		b = RedStarLinq.ToHashSet(E.Skip(nList2, 1));
		ProcessB(a, b);
		b = E.ToArray(E.Skip(nList2, 1));
		ProcessB(a, b);
		b = E.ToList(E.Skip(nList2, 1));
		ProcessB(a, b);
		b = E.Skip(nList2, 1).ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
		ProcessB(a, b);
		b = E.Skip(nEnumerable, 1);
		ProcessB(a, b);
		b = E.Skip(nEnumerable2, 1);
		ProcessB(a, b);
		b = E.SkipWhile(E.Skip(nList2, 1), _ => random.Next(10) != -1);
		ProcessB(a, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NCombine(b, (Func<(char, char, char), (char, char, char), (char, char, char)>)null!).ToList());
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NCombine(b, (Func<(char, char, char), (char, char, char), int, (char, char, char)>)null!).ToList());
		void ProcessB(G.IEnumerable<(char, char, char)> a, G.IEnumerable<(char, char, char)> b)
		{
			var c = a.NCombine(b, (x, y) => (x.Item1 + y.Item1, x.Item2 + y.Item2));
			var d = E.Zip(a, b, (x, y) => (x.Item1 + y.Item1, x.Item2 + y.Item2));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.NCombine(b, (x, y, index) => (x.Item1 + y.Item1, x.Item2 + y.Item2 + index));
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => (x.elem.Item1 + y.Item1, x.elem.Item2 + y.Item2 + x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.NCombine(b);
			var d2 = E.Zip(a, b);
			Assert.IsTrue(RedStarLinq.Equals(c2, d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			G.IEnumerable<(char, char, char)> b2 = RedStarLinq.ToList(E.Skip(nList2, 2));
			ProcessB2(a, b, b2);
			b2 = RedStarLinq.ToHashSet(E.Skip(nList2, 2));
			ProcessB2(a, b, b2);
			b2 = E.ToArray(E.Skip(nList2, 2));
			ProcessB2(a, b, b2);
			b2 = E.ToList(E.Skip(nList2, 2));
			ProcessB2(a, b, b2);
			b2 = E.Skip(nList2, 2).ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
			ProcessB2(a, b, b2);
			b2 = E.Skip(nEnumerable, 2);
			ProcessB2(a, b, b2);
			b2 = E.Skip(nEnumerable2, 2);
			ProcessB2(a, b, b2);
			b2 = E.SkipWhile(E.Skip(nList2, 2), _ => random.Next(10) != -1);
			ProcessB2(a, b, b2);
			Assert.ThrowsExactly<ArgumentNullException>(() => a.NCombine(b, b2, (Func<(char, char, char), (char, char, char), (char, char, char), (char, char, char)>)null!).ToList());
			Assert.ThrowsExactly<ArgumentNullException>(() => a.NCombine(b, b2, (Func<(char, char, char), (char, char, char), (char, char, char), int, (char, char, char)>)null!).ToList());
		}
		static void ProcessB2(G.IEnumerable<(char, char, char)> a, G.IEnumerable<(char, char, char)> b, G.IEnumerable<(char, char, char)> b2)
		{
			var c = a.NCombine(b, b2, (x, y, z) => (x.Item1 + y.Item1 + z.Item1, x.Item2 + y.Item2 + z.Item2));
			var d = E.Select(E.Zip(a, b, b2), x => (x.First.Item1 + x.Second.Item1 + x.Third.Item1, x.First.Item2 + x.Second.Item2 + x.Third.Item2));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.NCombine(b, b2, (x, y, z, index) => (x.Item1 + y.Item1 + z.Item1, x.Item2 + y.Item2 + z.Item2 + index));
			d = E.Zip(E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => (x.elem.Item1 + y.Item1, x.elem.Item2 + y.Item2, index: x.index)), b2, (x, y) => (x.Item1 + y.Item1, x.Item2 + y.Item2 + x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.NCombine(b, b2);
			var d2 = E.Zip(a, b, b2);
			Assert.IsTrue(RedStarLinq.Equals(c2, d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
		}
	});

	[TestMethod]
	public void TestNFill()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var @char = (char)random.Next('A', 'Z' + 1);
			var @string = (@char, @char, @char);
			var length = random.Next(1001);
			var a = RedStarLinq.NFill(@string, length);
			var b = E.ToList(E.Repeat(@string, length));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RedStarLinq.NFill(@string, -5));
			a = RedStarLinq.NFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), length);
			b = E.ToList(E.Select(E.Range(0, length), index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RedStarLinq.NFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), -5));
			Assert.ThrowsExactly<ArgumentNullException>(() => RedStarLinq.NFill((Func<int, (char, char, char)>)null!, length));
			a = RedStarLinq.NFill(length, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RedStarLinq.NFill(-5, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.ThrowsExactly<ArgumentNullException>(() => RedStarLinq.NFill(length, (Func<int, (char, char, char)>)null!));
		}
	}

	[TestMethod]
	public void TestNGroup()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		EComparer<int> comparer = new((x, y) => x / 3 == y / 3), comparer2 = new((x, y) => x / 3 == y / 3, x => 42), comparer3 = new((x, y) => x / 3 == y / 3, x => x / 4);
		for (var i = 0; i < 1000; i++)
		{
			var original = E.ToArray(E.Select(E.Range(0, random.Next(101)), _ => random.Next(-30, 30)));
			G.IEnumerable<int> a = RedStarLinq.ToList(original);
			ProcessA(a);
			a = RedStarLinq.ToArray(original);
			ProcessA(a);
			a = E.ToList(original);
			ProcessA(a);
			a = new List<int>(original).Insert(0, -42).GetSlice(1);
			ProcessA(a);
			a = E.Select(original, x => x);
			ProcessA(a);
			a = E.SkipWhile(original, _ => random.Next(10) == -1);
			ProcessA(a);
			a = E.SkipWhile(original, _ => random.Next(10) != -1);
			ProcessA(a);
		}
		void ProcessA(G.IEnumerable<int> a)
		{
			var c = a.NGroup(x => x);
			var d = E.GroupBy(a, x => x);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2);
			d = E.GroupBy(a, x => x / 2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(comparer);
			d = E.GroupBy(a, x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2, comparer);
			d = E.GroupBy(a, x => x / 2, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10, comparer);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(equalFunction: (x, y) => x / 3 == y / 3);
			d = E.GroupBy(a, x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x, equalFunction: (x, y) => x / 3 == y / 3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.GroupBy(a, x => x / 2, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(comparer2);
			d = E.GroupBy(a, x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2, comparer2);
			d = E.GroupBy(a, x => x / 2, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10, comparer2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.GroupBy(a, x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.GroupBy(a, x => x / 2, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(comparer3);
			d = E.GroupBy(a, x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2, comparer3);
			d = E.GroupBy(a, x => x / 2, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10, comparer3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.GroupBy(a, x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.GroupBy(a, x => x / 2, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.NGroup((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
		}
	}

	[TestMethod]
	public void TestNPairs()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		G.IEnumerable<(char, char, char)> a = new List<(char, char, char)>(nList);
		ProcessA(a);
		a = RedStarLinq.ToArray(nList);
		ProcessA(a);
		a = E.ToList(nList);
		ProcessA(a);
		a = new List<(char, char, char)>(nList).Insert(0, ('X', 'X', 'X')).GetSlice(1);
		ProcessA(a);
		a = nEnumerable;
		ProcessA(a);
		a = nEnumerable2;
		ProcessA(a);
		a = E.SkipWhile(nList, _ => random.Next(10) != -1);
		ProcessA(a);
		void ProcessA(G.IEnumerable<(char, char, char)> a)
		{
			var c = a.NPairs((x, y) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3);
			var d = E.Zip(a, E.Skip(a, 1), (x, y) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.NPairs((x, y, index) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3 + index);
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 1), (x, y) => x.elem.Item1 * y.Item1 + x.elem.Item2 * y.Item2 + x.elem.Item3 * y.Item3 + x.index);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.NPairs();
			var d2 = E.Zip(a, E.Skip(a, 1), (x, y) => (x, y));
			Assert.IsTrue(c2.Equals(d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			c = a.NPairs((x, y) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3, 3);
			d = E.Zip(a, E.Skip(a, 3), (x, y) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.NPairs((x, y, index) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3 + index, 3);
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 3), (x, y) => x.elem.Item1 * y.Item1 + x.elem.Item2 * y.Item2 + x.elem.Item3 * y.Item3 + x.index);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c2 = a.NPairs(3);
			d2 = E.Zip(a, E.Skip(a, 3), (x, y) => (x, y));
			Assert.IsTrue(c2.Equals(d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsExactly<ArgumentNullException>(() => a.NPairs((Func<(char, char, char), (char, char, char), int>)null!));
			Assert.ThrowsExactly<ArgumentNullException>(() => a.NPairs((Func<(char, char, char), (char, char, char), int, int>)null!));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.NPairs((x, y) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3, 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.NPairs((x, y, index) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3 + index, 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.NPairs(0));
		}
	}

	[TestMethod]
	public void TestNShuffle() => Test(a =>
	{
		var b = a.NShuffle(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var c = b.ToList().Sort();
		var d = E.Order(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NShuffle((Func<(char, char, char), (char, char, char)>)null!));
		b = a.NShuffle((x, index) => (x.Item2, x.Item3, (char)index));
		c = b.ToList().Sort();
		d = E.Order(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.NShuffle((Func<(char, char, char), int, (char, char, char)>)null!));
		var b2 = a.NShuffle();
		var c2 = b2.ToList().Sort();
		var d2 = E.Order(a);
		Assert.IsTrue(E.SequenceEqual(c2, d2));
	});

	[TestMethod]
	public void TestNSplitIntoEqual() => Test2(a =>
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var n = random.Next(1, 17);
		var b = a.NSplitIntoEqual(n);
		var c = E.SelectMany(b, x => x);
		Assert.IsTrue(E.All(E.SkipLast(b, 1), x => x.Length == n));
		Assert.IsTrue(E.All(E.TakeLast(b, 1), x => x.Length == (E.Count(a) + n - 1) % n + 1));
		Assert.IsTrue(RedStarLinq.Equals(a, c));
		Assert.IsTrue(E.SequenceEqual(c, a));
	});

	[TestMethod]
	public void TestToNList() => Test(a =>
	{
		var b = a.ToNList(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var c = E.ToList(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(b, c));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), (char, char, char)>)null!));
		b = a.ToNList((x, index) => (x.Item2, x.Item3, (char)index));
		c = E.ToList(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(b, c));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), int, (char, char, char)>)null!));
	});

	[TestMethod]
	public void TestPNBreak() => Test2(a =>
	{
		var c = a.PNBreak(x => x.Item1, x => (x.Item2, x.Item3));
		var d = (E.Select(a, x => x.Item1), E.Select(a, x => (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.PNBreak(x => (x.Item1, (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = RedStarLinq.Convert(a, x => (x.Item1, (x.Item2, x.Item3))).PNBreak();
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), char>)null!, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak(x => x.Item1, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), (char, (char, char, char))>)null!));
		c = a.PNBreak((x, index) => (char)(x.Item1 + index), (x, index) => (x.Item2, (char)(x.Item3 + index)));
		d = (E.Select(a, (x, index) => (char)(x.Item1 + index)), E.Select(a, (x, index) => (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.PNBreak((x, index) => ((char)(x.Item1 + index), (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((x, index) => (char)(x.Item1 + index), (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), int, (char, (char, char, char))>)null!));
		var c2 = a.PNBreak(x => x.Item1, x => x.Item3, x => (x.Item2, x.Item3));
		var d2 = (E.Select(a, x => x.Item1), E.Select(a, x => x.Item3), E.Select(a, x => (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.PNBreak(x => (x.Item1, x.Item3, (x.Item2, x.Item3)));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = RedStarLinq.Convert(a, x => (x.Item1, x.Item3, (x.Item2, x.Item3))).PNBreak();
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), char>)null!, (Func<(char, char, char), char>)null!, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak(x => x.Item1, (Func<(char, char, char), char>)null!, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak(x => x.Item1, x => x.Item3, (Func<(char, char, char), (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), (char, char, (char, char, char))>)null!));
		c2 = a.PNBreak((x, index) => (char)(x.Item1 + index), (x, index) => (char)(x.Item3 * index + 5), (x, index) => (x.Item2, (char)(x.Item3 + index)));
		d2 = (E.Select(a, (x, index) => (char)(x.Item1 + index)), E.Select(a, (x, index) => (char)(x.Item3 * index + 5)), E.Select(a, (x, index) => (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.PNBreak((x, index) => ((char)(x.Item1 + index), (char)(x.Item3 * index + 5), (x.Item2, (char)(x.Item3 + index))));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((x, index) => (char)(x.Item1 + index), (Func<(char, char, char), int, char>)null!, (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((x, index) => (char)(x.Item1 + index), (x, index) => (char)(x.Item3 * index + 5), (Func<(char, char, char), int, (char, char, char)>)null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNBreak((Func<(char, char, char), int, (char, char, (char, char, char))>)null!));
	});

	[TestMethod]
	public void TestPNConvert() => Test2(a =>
	{
		var b = a.ToNList(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var c = E.ToList(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(b, c));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNConvert((Func<(char, char, char), (char, char, char)>)null!));
		b = a.ToNList((x, index) => (x.Item2, x.Item3, (char)index));
		c = E.ToList(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(b, c));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.PNConvert((Func<(char, char, char), int, (char, char, char)>)null!));
	});

	[TestMethod]
	public void TestPNFill()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var @char = (char)random.Next('A', 'Z' + 1);
			var @string = (@char, @char, @char);
			var length = random.Next(1001);
			var a = RedStarLinqExtras.PNFill(@string, length);
			var b = E.ToList(E.Repeat(@string, length));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RedStarLinqExtras.PNFill(@string, -5));
			a = RedStarLinqExtras.PNFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), length);
			b = E.ToList(E.Select(E.Range(0, length), index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RedStarLinqExtras.PNFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), -5));
			Assert.ThrowsExactly<ArgumentNullException>(() => RedStarLinqExtras.PNFill((Func<int, (char, char, char)>)null!, length));
			a = RedStarLinqExtras.PNFill(length, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RedStarLinqExtras.PNFill(-5, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.ThrowsExactly<ArgumentNullException>(() => RedStarLinqExtras.PNFill(length, (Func<int, (char, char, char)>)null!));
		}
	}
}
