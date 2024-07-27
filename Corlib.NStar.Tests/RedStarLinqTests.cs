namespace Corlib.NStar.Tests;

[TestClass]
public class RedStarLinqTests
{
	public static void Test(Action<G.IEnumerable<string>> action)
	{
		G.IEnumerable<string> a = RedStarLinq.ToList(list);
		action(a);
		a = RedStarLinq.ToArray(list);
		action(a);
		a = E.ToList(list);
		action(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		action(a);
		a = list.Prepend("XXX");
		action(a);
		a = enumerable;
		action(a);
		a = enumerable2;
		action(a);
		a = E.SkipWhile(list, _ => random.Next(10) != -1);
		action(a);
	}

	public static void Test2(Action<G.IReadOnlyList<string>> action)
	{
		G.IReadOnlyList<string> a = RedStarLinq.ToList(list);
		action(a);
		a = RedStarLinq.ToArray(list);
		action(a);
		a = E.ToList(list);
		action(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		action(a);
		a = list.Prepend("XXX");
		action(a);
	}

	public static void TestN(Action<G.IEnumerable<(char, char, char)>> action)
	{
		G.IEnumerable<(char, char, char)> a = RedStarLinq.ToList(nList);
		action(a);
		a = RedStarLinq.ToArray(nList);
		action(a);
		a = E.ToList(nList);
		action(a);
		a = nList.ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
		action(a);
		a = nList.Prepend(('X', 'X', 'X'));
		action(a);
		a = nEnumerable;
		action(a);
		a = nEnumerable2;
		action(a);
		a = E.SkipWhile(nList, _ => random.Next(10) != -1);
		action(a);
	}

	public static void Test2N(Action<G.IReadOnlyList<(char, char, char)>> action)
	{
		G.IReadOnlyList<(char, char, char)> a = RedStarLinq.ToList(nList);
		action(a);
		a = RedStarLinq.ToArray(nList);
		action(a);
		a = E.ToList(nList);
		action(a);
		a = nList.ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
		action(a);
		a = nList.Prepend(('X', 'X', 'X'));
		action(a);
	}

	public static void TestS(Action<G.IEnumerable<char>> action)
	{
		G.IEnumerable<char> a = RedStarLinq.ToList(nString);
		action(a);
		a = RedStarLinq.ToArray(nString);
		action(a);
		a = E.ToList(nString);
		action(a);
		a = nString.ToList().Insert(0, 'X').GetSlice(1);
		action(a);
		a = nString.Prepend('X');
		action(a);
		a = nSEnumerable;
		action(a);
		a = nSEnumerable2;
		action(a);
		a = E.SkipWhile(nString, _ => random.Next(10) != -1);
		action(a);
	}

	public static void Test2S(Action<G.IReadOnlyList<char>> action)
	{
		G.IReadOnlyList<char> a = RedStarLinq.ToList(nString);
		action(a);
		a = RedStarLinq.ToArray(nString);
		action(a);
		a = E.ToList(nString);
		action(a);
		a = nString.ToList().Insert(0, 'X').GetSlice(1);
		action(a);
		a = nString.Prepend('X');
		action(a);
	}

	[TestMethod]
	public void TestAll() => Test(a =>
	{
		var c = a.All(x => x.Length > 0);
		var d = E.All(a, x => x.Length > 0);
		Assert.AreEqual(c, d);
		c = a.All(x => x.StartsWith('#'));
		d = E.All(a, x => x.StartsWith('#'));
		Assert.AreEqual(c, d);
		c = a.All(x => x.StartsWith('M'));
		d = E.All(a, x => x.StartsWith('M'));
		Assert.AreEqual(c, d);
		Assert.ThrowsException<ArgumentNullException>(() => a.All((Func<string, bool>)null!));
		c = a.All((x, index) => x.Length > 0 && index >= 0);
		d = E.All(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0);
		Assert.AreEqual(c, d);
		c = a.All((x, index) => index < 0);
		d = E.All(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0);
		Assert.AreEqual(c, d);
		c = a.All((x, index) => x.StartsWith('M') && index > 0);
		d = E.All(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0);
		Assert.AreEqual(c, d);
		Assert.ThrowsException<ArgumentNullException>(() => a.All((Func<string, int, bool>)null!));
	});

	[TestMethod]
	public void TestAllEqual()
	{
		Test(a =>
	{
		var c = a.AllEqual(x => x.Length);
		Assert.AreEqual(c, true);
		c = a.AllEqual() == a.Any();
		Assert.AreEqual(c, false);
		Assert.ThrowsException<ArgumentNullException>(() => a.AllEqual((Func<string, string>)null!));
		c = a.AllEqual((x, y) => x.Length == y.Length);
		Assert.AreEqual(c, true);
		c = a.AllEqual((x, y) => x == y) == a.Any();
		Assert.AreEqual(c, false);
		Assert.ThrowsException<ArgumentNullException>(() => a.AllEqual((Func<string, string, bool>)null!));
		c = a.AllEqual((x, y, index) => x.Length == y.Length && index < 10);
		Assert.AreEqual(c, true);
		c = a.AllEqual((x, y, index) => x.Length == y.Length && index < 0) == a.Any();
		Assert.AreEqual(c, false);
		c = a.AllEqual((x, y, index) => x.Length == y.Length && index > 0) == a.Any();
		Assert.AreEqual(c, false);
		c = a.AllEqual((x, y, index) => x.Length == y.Length && index < 5) == a.Any();
		Assert.AreEqual(c, false);
		Assert.ThrowsException<ArgumentNullException>(() => a.AllEqual((Func<string, string, int, bool>)null!));
	});
		var c = new[] { 3, 3, 3, 3, 3 }.AllEqual();
		Assert.AreEqual(c, true);
		c = new[] { 3, 4, 5, 6, 7 }.AllEqual();
		Assert.AreEqual(c, false);
		c = new[] { 3, 3, 3, 3, -42 }.AllEqual();
	}

	[TestMethod]
	public void TestAny() => Test(a =>
	{
		var c = a.Any(x => x.Length > 0);
		var d = E.Any(a, x => x.Length > 0);
		Assert.AreEqual(c, d);
		c = a.Any(x => x.StartsWith('#'));
		d = E.Any(a, x => x.StartsWith('#'));
		Assert.AreEqual(c, d);
		c = a.Any(x => x.StartsWith('M'));
		d = E.Any(a, x => x.StartsWith('M'));
		Assert.AreEqual(c, d);
		Assert.ThrowsException<ArgumentNullException>(() => a.Any((Func<string, bool>)null!));
		c = a.Any((x, index) => x.Length > 0 && index >= 0);
		d = E.Any(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0);
		Assert.AreEqual(c, d);
		c = a.Any((x, index) => index < 0);
		d = E.Any(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0);
		Assert.AreEqual(c, d);
		c = a.Any((x, index) => x.StartsWith('M') && index > 0);
		d = E.Any(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0);
		Assert.AreEqual(c, d);
		Assert.ThrowsException<ArgumentNullException>(() => a.Any((Func<string, int, bool>)null!));
		c = a.Any();
		d = E.Any(a);
		Assert.AreEqual(c, d);
		a = RedStarLinq.ToArray(list);
		c = a.Any();
		d = E.Any(a);
		Assert.AreEqual(c, d);
		a = E.ToList(list);
		c = a.Any();
		d = E.Any(a);
		Assert.AreEqual(c, d);
	});

	[TestMethod]
	public void TestBreak() => Test(a =>
	{
		var c = a.Break(x => x[0], x => x[1..]);
		var d = (E.Select(a, x => x[0]), E.Select(a, x => x[1..]));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.Break(x => (x[0], x[1..]));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = E.Select(a, x => (x[0], x[1..])).Break();
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, char>)null!, (Func<string, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break(x => x[0], (Func<string, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, (char, string)>)null!));
		c = a.Break((x, index) => (char)(x[0] + index), (x, index) => x[1..] + index.ToString("D2"));
		d = (E.Select(a, (x, index) => (char)(x[0] + index)), E.Select(a, (x, index) => x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		c = a.Break((x, index) => ((char)(x[0] + index), x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c.Item1, d.Item1) && E.SequenceEqual(c.Item2, d.Item2));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, int, char>)null!, (Func<string, int, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((x, index) => (char)(x[0] + index), (Func<string, int, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, int, (char, string)>)null!));
		var c2 = a.Break(x => x[0], x => x[^1], x => x[1..]);
		var d2 = (E.Select(a, x => x[0]), E.Select(a, x => x[^1]), E.Select(a, x => x[1..]));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.Break(x => (x[0], x[^1], x[1..]));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = E.Select(a, x => (x[0], x[^1], x[1..])).Break();
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, char>)null!, (Func<string, char>)null!, (Func<string, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break(x => x[0], (Func<string, char>)null!, (Func<string, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break(x => x[0], x => x[^1], (Func<string, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, (char, char, string)>)null!));
		c2 = a.Break((x, index) => (char)(x[0] + index), (x, index) => (char)(x[^1] * index + 5), (x, index) => x[1..] + index.ToString("D2"));
		d2 = (E.Select(a, (x, index) => (char)(x[0] + index)), E.Select(a, (x, index) => (char)(x[^1] * index + 5)), E.Select(a, (x, index) => x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		c2 = a.Break((x, index) => ((char)(x[0] + index), (char)(x[^1] * index + 5), x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c2.Item1, d2.Item1) && E.SequenceEqual(c2.Item2, d2.Item2) && E.SequenceEqual(c2.Item3, d2.Item3));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, int, char>)null!, (Func<string, int, char>)null!, (Func<string, int, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((x, index) => (char)(x[0] + index), (Func<string, int, char>)null!, (Func<string, int, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((x, index) => (char)(x[0] + index), (x, index) => (char)(x[^1] * index + 5), (Func<string, int, string>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => a.Break((Func<string, int, (char, char, string)>)null!));
	});

	[TestMethod]
	public void TestBreakFilter() => Test(a =>
	{
		var c = a.BreakFilter(x => x.Length > 0, out var c2);
		var d = E.Where(a, x => x.Length > 0);
		var d2 = E.Where(a, x => x.Length <= 0);
		Assert.IsTrue(c.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.IsTrue(c2.Equals(d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		c = a.BreakFilter(x => x.StartsWith('#'), out c2);
		d = E.Where(a, x => x.StartsWith('#'));
		d2 = E.Where(a, x => !x.StartsWith('#'));
		Assert.IsTrue(c.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.IsTrue(c2.Equals(d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		c = a.BreakFilter(x => x.StartsWith('M'), out c2);
		d = E.Where(a, x => x.StartsWith('M'));
		d2 = E.Where(a, x => !x.StartsWith('M'));
		Assert.IsTrue(c.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.IsTrue(c2.Equals(d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		Assert.ThrowsException<ArgumentNullException>(() => a.BreakFilter((Func<string, bool>)null!, out c2));
		c = a.BreakFilter((x, index) => x.Length > 0 && index >= 0, out c2);
		d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0), x => x.elem);
		d2 = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => !(x.elem.Length > 0 && x.index >= 0)), x => x.elem);
		Assert.IsTrue(c.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.IsTrue(c2.Equals(d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		c = a.BreakFilter((x, index) => index < 0, out c2);
		d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0), x => x.elem);
		d2 = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.index >= 0), x => x.elem);
		Assert.IsTrue(c.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.IsTrue(c2.Equals(d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		c = a.BreakFilter((x, index) => x.StartsWith('M') && index > 0, out c2);
		d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0), x => x.elem);
		d2 = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => !(x.elem.StartsWith('M') && x.index > 0)), x => x.elem);
		Assert.IsTrue(c.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.IsTrue(c2.Equals(d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		Assert.ThrowsException<ArgumentNullException>(() => a.BreakFilter((Func<string, int, bool>)null!, out c2));
	});

	[TestMethod]
	public void TestCombine() => Test(a =>
	{
		G.IEnumerable<string> b = E.Skip(list2, 1).ToList();
		ProcessB(a, b);
		b = E.Skip(list2, 1).ToArray();
		ProcessB(a, b);
		b = E.ToList(E.Skip(list2, 1));
		ProcessB(a, b);
		b = E.Skip(list2, 1).ToList().Insert(0, "XXX").GetSlice(1);
		ProcessB(a, b);
		b = E.Skip(enumerable, 1);
		ProcessB(a, b);
		b = E.Skip(enumerable2, 1);
		ProcessB(a, b);
		b = E.SkipWhile(E.Skip(list2, 1), _ => random.Next(10) != -1);
		ProcessB(a, b);
		static void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
		{
			var c = a.Combine(b, (x, y) => x + y);
			var d = E.Zip(a, b, (x, y) => x + y);
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Combine(b, (x, y, index) => x + y + index.ToString());
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => x.elem + y + x.index.ToString());
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Combine(b);
			var d2 = E.Zip(a, b);
			Assert.IsTrue(RedStarLinq.Equals(c2, d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, (Func<string, string, string>)null!).ToList());
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, (Func<string, string, int, string>)null!).ToList());
			G.IEnumerable<string> b2 = E.Skip(list2, 2).ToList();
			ProcessB2(a, b, b2);
			b2 = E.Skip(list2, 2).ToArray();
			ProcessB2(a, b, b2);
			b2 = E.ToList(E.Skip(list2, 2));
			ProcessB2(a, b, b2);
			b2 = E.Skip(list2, 2).ToList().Insert(0, "XXX").GetSlice(1);
			ProcessB2(a, b, b2);
			b2 = E.Skip(enumerable, 2);
			ProcessB2(a, b, b2);
			b2 = E.Skip(enumerable2, 2);
			ProcessB2(a, b, b2);
			b2 = E.SkipWhile(E.Skip(list2, 2), _ => random.Next(10) != -1);
			ProcessB2(a, b, b2);
		}
		static void ProcessB2(G.IEnumerable<string> a, G.IEnumerable<string> b, G.IEnumerable<string> b2)
		{
			var c = a.Combine(b, b2, (x, y, z) => x + y + z);
			var d = E.Select(E.Zip(a, b, b2), x => x.First + x.Second + x.Third);
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Combine(b, b2, (x, y, z, index) => x + y + z + index.ToString());
			d = E.Zip(E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => (x.elem + y, x.index)), b2, (x, y) => x.Item1 + y + x.index.ToString());
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Combine(b, b2);
			var d2 = E.Zip(a, b, b2);
			Assert.IsTrue(RedStarLinq.Equals(c2, d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, b2, (Func<string, string, string, string>)null!).ToList());
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, b2, (Func<string, string, string, int, string>)null!).ToList());
		}
	});

	[TestMethod]
	public void TestCombine2() => Test2(a =>
	{
		G.IReadOnlyList<string> b = E.Skip(list2, 1).ToList();
		ProcessB(a, b);
		b = E.Skip(list2, 1).ToArray();
		ProcessB(a, b);
		b = E.ToList(E.Skip(list2, 1));
		ProcessB(a, b);
		b = E.Skip(list2, 1).ToList().Insert(0, "XXX").GetSlice(1);
		ProcessB(a, b);
		static void ProcessB(G.IReadOnlyList<string> a, G.IReadOnlyList<string> b)
		{
			var c = a.Combine(b, (x, y) => x + y);
			var d = E.Zip(a, b, (x, y) => x + y);
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Combine(b, (x, y, index) => x + y + index.ToString());
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => x.elem + y + x.index.ToString());
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Combine(b);
			var d2 = E.Zip(a, b);
			Assert.IsTrue(RedStarLinq.Equals(c2, d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, (Func<string, string, string>)null!).ToList());
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, (Func<string, string, int, string>)null!).ToList());
			G.IReadOnlyList<string> b2 = E.Skip(list2, 2).ToList();
			ProcessB2(a, b, b2);
			b2 = E.Skip(list2, 2).ToArray();
			ProcessB2(a, b, b2);
			b2 = E.ToList(E.Skip(list2, 2));
			ProcessB2(a, b, b2);
			b2 = E.Skip(list2, 2).ToList().Insert(0, "XXX").GetSlice(1);
			ProcessB2(a, b, b2);
		}
		static void ProcessB2(G.IReadOnlyList<string> a, G.IReadOnlyList<string> b, G.IReadOnlyList<string> b2)
		{
			var c = a.Combine(b, b2, (x, y, z) => x + y + z);
			var d = E.Select(E.Zip(a, b, b2), x => x.First + x.Second + x.Third);
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Combine(b, b2, (x, y, z, index) => x + y + z + index.ToString());
			d = E.Zip(E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => (x.elem + y, x.index)), b2, (x, y) => x.Item1 + y + x.index.ToString());
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Combine(b, b2);
			var d2 = E.Zip(a, b, b2);
			Assert.IsTrue(RedStarLinq.Equals(c2, d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, b2, (Func<string, string, string, string>)null!).ToList());
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, b2, (Func<string, string, string, int, string>)null!).ToList());
		}
	});

	[TestMethod]
	public void TestConcat() => Test(a =>
	{
		G.IEnumerable<string> b = E.Skip(list2, 1).ToList();
		ProcessB(a, b);
		b = E.Skip(list2, 1).ToArray();
		ProcessB(a, b);
		b = E.ToList(E.Skip(list2, 1));
		ProcessB(a, b);
		b = E.Skip(list2, 1).ToList().Insert(0, "XXX").GetSlice(1);
		ProcessB(a, b);
		b = E.Skip(enumerable, 1);
		ProcessB(a, b);
		b = E.Skip(enumerable2, 1);
		ProcessB(a, b);
		b = E.SkipWhile(E.Skip(list2, 1), _ => random.Next(10) != -1);
		ProcessB(a, b);
		static void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
		{
			var c = a.Concat(b);
			var d = E.Concat(a, b);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var array = new[] { a, b };
			c = a.Concat(array);
			d = E.Concat(a, E.Concat(a, b));
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			Assert.ThrowsException<ArgumentNullException>(() => a.Concat((G.IEnumerable<string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Concat((G.IEnumerable<string>[])null!));
			G.IEnumerable<string> b2 = E.Skip(list2, 2).ToList();
			ProcessB2(a, b, b2);
			b2 = E.Skip(list2, 2).ToArray();
			ProcessB2(a, b, b2);
			b2 = E.ToList(E.Skip(list2, 2));
			ProcessB2(a, b, b2);
			b2 = E.Skip(list2, 2).ToList().Insert(0, "XXX").GetSlice(1);
			ProcessB2(a, b, b2);
			b2 = E.Skip(enumerable, 2);
			ProcessB2(a, b, b2);
			b2 = E.Skip(enumerable2, 2);
			ProcessB2(a, b, b2);
			b2 = E.SkipWhile(E.Skip(list2, 2), _ => random.Next(10) != -1);
			ProcessB2(a, b, b2);
		}
		static void ProcessB2(G.IEnumerable<string> a, G.IEnumerable<string> b, G.IEnumerable<string> b2)
		{
			var c = a.Concat(b, b2);
			var d = E.Concat(E.Concat(a, b), b2);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var array = new[] { a, b, b2 };
			c = a.Concat(array);
			d = E.Concat(a, E.Concat(E.Concat(a, b), b2));
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			Assert.ThrowsException<ArgumentNullException>(() => a.Concat(b, (G.IEnumerable<string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Concat(b, b2, (G.IEnumerable<string>)null!));
		}
	});

	[TestMethod]
	public void TestContains() => Test(a =>
	{
		ProcessString(a, "MMM");
		ProcessString(a, "#");
		ProcessString(a, null!);
		static void ProcessString(G.IEnumerable<string> a, string s)
		{
			var c = a.Contains(s);
			var d = E.Contains(a, s);
			Assert.AreEqual(c, d);
			c = a.Contains(s, new EComparer<string>((x, y) => x == y));
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y));
			Assert.AreEqual(c, d);
			c = a.Contains(s, (x, y) => x == y);
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y));
			Assert.AreEqual(c, d);
			c = a.Contains(s, new EComparer<string>((x, y) => x == y, x => 42));
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y, x => 42));
			Assert.AreEqual(c, d);
			c = a.Contains(s, (x, y) => x == y, x => 42);
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y, x => 42));
			Assert.AreEqual(c, d);
			c = a.Contains(s, new EComparer<string>((x, y) => false));
			d = E.Contains(a, s, new EComparer<string>((x, y) => false));
			Assert.AreEqual(c, d);
			c = a.Contains(s, (x, y) => false);
			d = E.Contains(a, s, new EComparer<string>((x, y) => false));
			Assert.AreEqual(c, d);
			c = a.Contains(s, new EComparer<string>((x, y) => false, x => 42));
			d = E.Contains(a, s, new EComparer<string>((x, y) => false, x => 42));
			Assert.AreEqual(c, d);
			c = a.Contains(s, (x, y) => false, x => 42);
			d = E.Contains(a, s, new EComparer<string>((x, y) => false, x => 42));
			Assert.AreEqual(c, d);
			Assert.ThrowsException<ArgumentNullException>(() => a.Contains(s, (G.IEqualityComparer<string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Contains(s, (Func<string, string, bool>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Contains(s, (x, y) => x == y, null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Contains(s, (Func<string, string, bool>)null!, null!));
		}
	});

	[TestMethod]
	public void TestConvert() => Test(a =>
	{
		var c = a.Convert(x => x[1..]);
		var d = E.Select(a, x => x[1..]);
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, string>)null!));
		c = a.Convert((x, index) => x[1..] + index.ToString("D2"));
		d = E.Select(a, (x, index) => x[1..] + index.ToString("D2"));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, int, string>)null!));
	});

	[TestMethod]
	public void TestConvert2() => Test2(a =>
	{
		var c = a.Convert(x => x[1..]);
		var d = E.Select(a, x => x[1..]);
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, string>)null!));
		c = a.Convert((x, index) => x[1..] + index.ToString("D2"));
		d = E.Select(a, (x, index) => x[1..] + index.ToString("D2"));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, int, string>)null!));
	});

	[TestMethod]
	public void TestConvertAndJoin() => Test(a =>
	{
		var c = a.ConvertAndJoin(x => x[1..]);
		var d = E.SelectMany(a, x => x[1..]);
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, string>)null!));
		c = a.ConvertAndJoin((x, index) => x[1..] + index.ToString("D2"));
		d = E.SelectMany(a, (x, index) => x[1..] + index.ToString("D2"));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, int, string>)null!));
	});

	[TestMethod]
	public void TestEquals()
	{
		for (var i = 0; i < 1000; i++)
		{
			G.IEnumerable<string> a = new List<string>(E.Select(E.Range(0, random.Next(2, 100)), _ => random.Next(1000).ToString("D3")));
			ProcessA(a);
			a = E.ToArray(E.Select(E.Range(0, random.Next(2, 100)), _ => random.Next(1000).ToString("D3")));
			ProcessA(a);
			a = new G.List<string>(E.Select(E.Range(0, random.Next(2, 100)), _ => random.Next(1000).ToString("D3")));
			ProcessA(a);
			a = new List<string>(E.Select(E.Range(0, random.Next(2, 100)), _ => random.Next(1000).ToString("D3"))).Insert(0, "XXX").GetSlice(1);
			ProcessA(a);
			a = E.Select(E.ToArray(E.Select(E.Range(0, random.Next(2, 100)), _ => random.Next(1000).ToString("D3"))), x => x);
			ProcessA(a);
			a = E.SkipWhile(E.ToArray(E.Select(E.Range(0, random.Next(2, 100)), _ => random.Next(1000).ToString("D3"))), _ => random.Next(10) != -1);
			ProcessA(a);
		}
		static void ProcessA(G.IEnumerable<string> a)
		{
			var b = a;
			ProcessB(a, b);
			b = E.Append(b, random.Next(1000).ToString("D3"));
			ProcessB(a, b);
			b = E.Skip(b, 1);
			ProcessB(a, b);
			b = E.Prepend(b, random.Next(1000).ToString("D3"));
			ProcessB(a, b);
			b = E.SkipLast(b, 1);
			ProcessB(a, b);
			b = E.Append(E.SkipLast(b, 1), random.Next(1000).ToString("D3"));
			ProcessB(a, b);
			b = E.Prepend(E.Skip(b, 1), random.Next(1000).ToString("D3"));
			ProcessB(a, b);
#pragma warning disable IDE0028 // Упростите инициализацию коллекции
#pragma warning disable IDE0301 // Упростите инициализацию коллекции
			b = new List<string>();
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = Array.Empty<string>();
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = new G.List<string>();
#pragma warning restore IDE0301 // Упростите инициализацию коллекции
#pragma warning restore IDE0028 // Упростите инициализацию коллекции
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = new List<string>().Insert(0, "XXX").GetSlice(1);
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = E.Select(E.Take(a, 0), x => x);
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = E.TakeWhile(a, _ => random.Next(10) == -1);
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
		}
		static void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
		{
			G.IEnumerable<string> c = new List<string>(b);
			Assert.AreEqual(RedStarLinq.Equals(a, c), E.SequenceEqual(a, c));
			c = E.ToArray(b);
			Assert.AreEqual(RedStarLinq.Equals(a, c), E.SequenceEqual(a, c));
			c = E.ToList(b);
			Assert.AreEqual(RedStarLinq.Equals(a, c), E.SequenceEqual(a, c));
			c = new List<string>(b).Insert(0, "XXX").GetSlice(1);
			Assert.AreEqual(RedStarLinq.Equals(a, c), E.SequenceEqual(a, c));
			c = E.Select(b, x => x);
			Assert.AreEqual(RedStarLinq.Equals(a, c), E.SequenceEqual(a, c));
			c = E.SkipWhile(b, _ => random.Next(10) != -1);
			Assert.AreEqual(RedStarLinq.Equals(a, c), E.SequenceEqual(a, c));
		}
	}

	[TestMethod]
	public void TestFilter() => Test(a =>
	{
		var c = a.Filter(x => x.Length > 0);
		var d = E.Where(a, x => x.Length > 0);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Filter(x => x.StartsWith('#'));
		d = E.Where(a, x => x.StartsWith('#'));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Filter(x => x.StartsWith('M'));
		d = E.Where(a, x => x.StartsWith('M'));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.ThrowsException<ArgumentNullException>(() => a.Filter((Func<string, bool>)null!));
		c = a.Filter((x, index) => x.Length > 0 && index >= 0);
		d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0), x => x.elem);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Filter((x, index) => index < 0);
		d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0), x => x.elem);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Filter((x, index) => x.StartsWith('M') && index > 0);
		d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0), x => x.elem);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		Assert.ThrowsException<ArgumentNullException>(() => a.Filter((Func<string, int, bool>)null!));
	});

	[TestMethod]
	public void TestPairs() => Test(a =>
	{
		var c = a.Pairs((x, y) => x + y);
		var d = E.Zip(a, E.Skip(a, 1), (x, y) => x + y);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Pairs((x, y, index) => x + y + index.ToString());
		d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 1), (x, y) => x.elem + y + x.index.ToString());
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		var c2 = a.Pairs();
		var d2 = E.Zip(a, E.Skip(a, 1), (x, y) => (x, y));
		Assert.IsTrue(RedStarLinq.Equals(c2, d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		c = a.Pairs((x, y) => x + y, 3);
		d = E.Zip(a, E.Skip(a, 3), (x, y) => x + y);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Pairs((x, y, index) => x + y + index.ToString(), 3);
		d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 3), (x, y) => x.elem + y + x.index.ToString());
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c2 = a.Pairs(3);
		d2 = E.Zip(a, E.Skip(a, 3), (x, y) => (x, y));
		Assert.IsTrue(RedStarLinq.Equals(c2, d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		Assert.ThrowsException<ArgumentNullException>(() => a.Pairs((Func<string, string, string>)null!).ToList());
		Assert.ThrowsException<ArgumentNullException>(() => a.Pairs((Func<string, string, int, string>)null!).ToList());
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs((x, y) => x + y, 0).ToList());
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs((x, y, index) => x + y + index.ToString(), 0).ToList());
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs(0).ToList());
	});

	[TestMethod]
	public void TestPairs2() => Test2(a =>
	{
		var c = a.Pairs((x, y) => x + y);
		var d = E.Zip(a, E.Skip(a, 1), (x, y) => x + y);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Pairs((x, y, index) => x + y + index.ToString());
		d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 1), (x, y) => x.elem + y + x.index.ToString());
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		var c2 = a.Pairs();
		var d2 = E.Zip(a, E.Skip(a, 1), (x, y) => (x, y));
		Assert.IsTrue(RedStarLinq.Equals(c2, d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		c = a.Pairs((x, y) => x + y, 3);
		d = E.Zip(a, E.Skip(a, 3), (x, y) => x + y);
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.Pairs((x, y, index) => x + y + index.ToString(), 3);
		d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 3), (x, y) => x.elem + y + x.index.ToString());
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c2 = a.Pairs(3);
		d2 = E.Zip(a, E.Skip(a, 3), (x, y) => (x, y));
		Assert.IsTrue(RedStarLinq.Equals(c2, d2));
		Assert.IsTrue(E.SequenceEqual(d2, c2));
		Assert.ThrowsException<ArgumentNullException>(() => a.Pairs((Func<string, string, string>)null!).ToList());
		Assert.ThrowsException<ArgumentNullException>(() => a.Pairs((Func<string, string, int, string>)null!).ToList());
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs((x, y) => x + y, 0).ToList());
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs((x, y, index) => x + y + index.ToString(), 0).ToList());
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs(0).ToList());
	});

	[TestMethod]
	public void TestSetAll()
	{
		var a = E.ToArray(list).SetAll(defaultString);
		ProcessA(a, 0, list.Length);
		a = E.ToArray(list).SetAll(defaultString, 3);
		ProcessA(a, 3, list.Length);
		a = E.ToArray(list).SetAll(defaultString, 2, 4);
		ProcessA(a, 2, 6);
		a = E.ToArray(list).SetAll(defaultString, ^5);
		ProcessA(a, list.Length - 5, list.Length);
		a = E.ToArray(list).SetAll(defaultString, ^6..4);
		ProcessA(a, list.Length - 6, 4);
		static void ProcessA(string[] a, int start, int end)
		{
			var b = new G.List<string>(list);
			for (var i = start; i < end; i++)
				b[i] = defaultString;
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
	}

	[TestMethod]
	public void TestShuffle() => Test(a =>
	{
		var c = a.Shuffle(x => x[1..]);
		var d = c.NSort();
		var e = E.Order(E.Select(a, x => x[1..]));
		Assert.IsTrue(E.SequenceEqual(c, e));
		Assert.ThrowsException<ArgumentNullException>(() => a.Shuffle((Func<string, string>)null!));
		c = a.Shuffle((x, index) => x[1..] + index.ToString("D2"));
		d = c.NSort();
		e = E.Order(E.Select(a, (x, index) => x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c, e));
		Assert.ThrowsException<ArgumentNullException>(() => a.Shuffle((Func<string, int, string>)null!));
		var c2 = a.Shuffle();
		var d2 = c2.NSort();
		var e2 = E.Order(a);
		Assert.IsTrue(E.SequenceEqual(c2, e2));
	});

	[TestMethod]
	public void TestSkip()
	{
		var a = list.Skip(3);
		var b = E.Skip(list, 3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Skip(1000);
		b = E.Skip(list, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Skip(-2);
		b = E.Skip(list, -2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Skip(3);
		b = E.Skip(enumerable, 3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Skip(1000);
		b = E.Skip(enumerable, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Skip(-2);
		b = E.Skip(enumerable, -2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Skip(3);
		b = E.Skip(enumerable2, 3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Skip(1000);
		b = E.Skip(enumerable2, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Skip(-2);
		b = E.Skip(enumerable2, -2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = list.SkipLast(4);
		var b = E.SkipLast(list, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.SkipLast(1000);
		b = E.SkipLast(list, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.SkipLast(-5);
		b = E.SkipLast(list, -5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.SkipLast(4);
		b = E.SkipLast(enumerable, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.SkipLast(1000);
		b = E.SkipLast(enumerable, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.SkipLast(-5);
		b = E.SkipLast(enumerable, -5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.SkipLast(4);
		b = E.SkipLast(enumerable2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.SkipLast(1000);
		b = E.SkipLast(enumerable2, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.SkipLast(-5);
		b = E.SkipLast(enumerable2, -5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestTake()
	{
		var a = list.Take(3);
		var b = E.Take(list, 3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(1000);
		b = E.Take(list, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(-2);
		b = E.Take(list, -2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(2..5);
		b = E.Take(list, 2..5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(5..2);
		b = E.Take(list, 5..2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(2..^3);
		b = E.Take(list, 2..^3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(5..^4);
		b = E.Take(list, 5..^4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(^5..6);
		b = E.Take(list, ^5..6);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(^10..9);
		b = E.Take(list, ^10..9);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.Take(^5..^2);
		b = E.Take(list, ^5..^2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(3);
		b = E.Take(enumerable, 3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(1000);
		b = E.Take(enumerable, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(-2);
		b = E.Take(enumerable, -2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(2..5);
		b = E.Take(enumerable, 2..5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(5..2);
		b = E.Take(enumerable, 5..2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(2..^3);
		b = E.Take(enumerable, 2..^3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(5..^4);
		b = E.Take(enumerable, 5..^4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(^5..6);
		b = E.Take(enumerable, ^5..6);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(^10..9);
		b = E.Take(enumerable, ^10..9);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.Take(^5..^2);
		b = E.Take(enumerable, ^5..^2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(3);
		b = E.Take(enumerable2, 3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(1000);
		b = E.Take(enumerable2, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(-2);
		b = E.Take(enumerable2, -2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(2..5);
		b = E.Take(enumerable2, 2..5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(5..2);
		b = E.Take(enumerable2, 5..2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(2..^3);
		b = E.Take(enumerable2, 2..^3);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(5..^4);
		b = E.Take(enumerable2, 5..^4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(^5..6);
		b = E.Take(enumerable2, ^5..6);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(^10..9);
		b = E.Take(enumerable2, ^10..9);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.Take(^5..^2);
		b = E.Take(enumerable2, ^5..^2);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = list.TakeLast(4);
		var b = E.TakeLast(list, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.TakeLast(1000);
		b = E.TakeLast(list, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.TakeLast(-5);
		b = E.TakeLast(list, -5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.TakeLast(4);
		b = E.TakeLast(enumerable, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.TakeLast(1000);
		b = E.TakeLast(enumerable, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable.TakeLast(-5);
		b = E.TakeLast(enumerable, -5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.TakeLast(4);
		b = E.TakeLast(enumerable2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.TakeLast(1000);
		b = E.TakeLast(enumerable2, 1000);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = enumerable2.TakeLast(-5);
		b = E.TakeLast(enumerable2, -5);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestToArray()
	{
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => random.Next()));
			ProcessA(original, 1234567890);
			var original2 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (long)random.Next() << 31 | (uint)random.Next()));
			ProcessA(original2, 1234567890123456789);
			var original3 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => random.Next().ToString("D3")));
			ProcessA(original3, "XXX");
			var original4 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (((MpzT)random.Next() << 31 | random.Next()) << 31 | random.Next()) << 31 | random.Next()));
			ProcessA(original4, 12345678901234567890);
			var original5 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (random.Next(), random.Next())));
			ProcessA(original5, (1234567890, 1234567890));
			var original6 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => new BitList(new[] { random.Next(), random.Next() })));
			ProcessA(original6, new BitList([1234567890, 1234567890]));
		}
		static void ProcessA<T>(ImmutableArray<T> original, T @default)
		{
			G.IEnumerable<T> a = new List<T>(original);
			ProcessA2(a);
			a = E.ToArray(original);
			ProcessA2(a);
			a = E.ToList(original);
			ProcessA2(a);
			a = new List<T>(original).Insert(0, @default).GetSlice(1);
			ProcessA2(a);
			a = E.Select(original, x => x);
			ProcessA2(a);
			a = E.SkipWhile(original, _ => random.Next(10) == -1);
			ProcessA2(a);
		}
		static void ProcessA2<T>(G.IEnumerable<T> a)
		{
			var b = new List<T>(a);
			var c = a.ToArray();
			var d = E.ToArray(a);
			Assert.IsTrue(RedStarLinq.Equals(b, c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(RedStarLinq.Equals(b, d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestToArray2() => Test2(a =>
	{
		var c = a.ToArray(x => x[1..]);
		var d = E.ToArray(E.Select(a, x => x[1..]));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToArray((Func<string, string>)null!));
		c = a.ToArray((x, index) => x[1..] + index.ToString("D2"));
		d = E.ToArray(E.Select(a, (x, index) => x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToArray((Func<string, int, string>)null!));
	});

	[TestMethod]
	public void TestToList()
	{
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => random.Next()));
			ProcessA(original, 1234567890);
			var original2 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (long)random.Next() << 31 | (uint)random.Next()));
			ProcessA(original2, 1234567890123456789);
			var original3 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => random.Next().ToString("D3")));
			ProcessA(original3, "XXX");
			var original4 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (((MpzT)random.Next() << 31 | random.Next()) << 31 | random.Next()) << 31 | random.Next()));
			ProcessA(original4, 12345678901234567890);
			var original5 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (random.Next(), random.Next())));
			ProcessA(original5, (1234567890, 1234567890));
			var original6 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => new BitList(new[] { random.Next(), random.Next() })));
			ProcessA(original6, new BitList([1234567890, 1234567890]));
		}
		static void ProcessA<T>(ImmutableArray<T> original, T @default)
		{
			G.IEnumerable<T> a = new List<T>(original);
			ProcessA2(a);
			a = E.ToArray(original);
			ProcessA2(a);
			a = E.ToList(original);
			ProcessA2(a);
			a = new List<T>(original).Insert(0, @default).GetSlice(1);
			ProcessA2(a);
			a = E.Select(original, x => x);
			ProcessA2(a);
			a = E.SkipWhile(original, _ => random.Next(10) == -1);
			ProcessA2(a);
		}
		static void ProcessA2<T>(G.IEnumerable<T> a)
		{
			var b = new List<T>(a);
			var c = a.ToList();
			var d = E.ToList(a);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestToList2() => Test2(a =>
	{
		var c = a.ToList(x => x[1..]);
		var d = E.ToList(E.Select(a, x => x[1..]));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToList((Func<string, string>)null!));
		c = a.ToList((x, index) => x[1..] + index.ToString("D2"));
		d = E.ToList(E.Select(a, (x, index) => x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToList((Func<string, int, string>)null!));
	});

	[TestMethod]
	public void TestToNList()
	{
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => random.Next()));
			ProcessA(original, 1234567890);
			var original2 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (long)random.Next() << 31 | (uint)random.Next()));
			ProcessA(original2, 1234567890123456789);
			var original3 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (random.Next(), random.Next())));
			ProcessA(original3, (1234567890, 1234567890));
		}
		static void ProcessA<T>(ImmutableArray<T> original, T @default) where T : unmanaged
		{
			G.IEnumerable<T> a = new List<T>(original);
			ProcessA2(a);
			a = E.ToArray(original);
			ProcessA2(a);
			a = E.ToList(original);
			ProcessA2(a);
			a = new List<T>(original).Insert(0, @default).GetSlice(1);
			ProcessA2(a);
			a = E.Select(original, x => x);
			ProcessA2(a);
			a = E.SkipWhile(original, _ => random.Next(10) == -1);
			ProcessA2(a);
		}
		static void ProcessA2<T>(G.IEnumerable<T> a) where T : unmanaged
		{
			var b = new NList<T>(a);
			var c = a.ToNList();
			var d = E.ToList(a);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestToNList2() => Test2N(a =>
	{
		var c = a.ToNList(x => (x.Item2, x.Item3, '\0'));
		var d = E.ToList(E.Select(a, x => (x.Item2, x.Item3, '\0')));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), (char, char, char)>)null!));
		c = a.ToNList((x, index) => (x.Item2, x.Item3, (char)index));
		d = E.ToList(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), int, (char, char, char)>)null!));
	});

	[TestMethod]
	public void TestToNString()
	{
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (char)random.Next(65536)));
			ProcessA(original, (char)12345);
		}
		static void ProcessA(ImmutableArray<char> original, char @default)
		{
			G.IEnumerable<char> a = new String(original);
			ProcessA2(a);
			a = RedStarLinq.ToList(original);
			ProcessA2(a);
			a = E.ToArray(original);
			ProcessA2(a);
			a = E.ToList(original);
			ProcessA2(a);
			a = new List<char>(original).Insert(0, @default).GetSlice(1);
			ProcessA2(a);
			a = E.Select(original, x => x);
			ProcessA2(a);
			a = E.SkipWhile(original, _ => random.Next(10) == -1);
			ProcessA2(a);
		}
		static void ProcessA2(G.IEnumerable<char> a)
		{
			var b = new String(a);
			var c = a.ToNString();
			var d = new string(E.ToArray(a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestToNString2() => Test2S(a =>
	{
		var c = a.ToNString(x => (char)(x + 1));
		var d = E.ToList(E.Select(a, x => (char)(x + 1)));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNString((Func<char, char>)null!));
		c = a.ToNString((x, index) => (char)(x + index));
		d = E.ToList(E.Select(a, (x, index) => (char)(x + index)));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNString((Func<char, int, char>)null!));
	});

	[TestMethod]
	public void TestPRemoveDoubles()
	{
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(1000, _ => random.Next(100)));
			var original2 = ImmutableArray.Create(RedStarLinq.FillArray(1000, _ => (long)random.Next(100)));
			ProcessA(original, original2, 1234567890);
		}
		static void ProcessA(ImmutableArray<int> original, ImmutableArray<long> original2, int @default)
		{
			G.IReadOnlyList<int> a = new List<int>(original);
			G.IReadOnlyList<long> a2 = new List<long>(original2);
			ProcessA2(a, a2);
			a = new NList<int>(original);
			a2 = new NList<long>(original2);
			ProcessA2(a, a2);
			a = E.ToArray(original);
			a2 = E.ToArray(original2);
			ProcessA2(a, a2);
			a = E.ToList(original);
			a2 = E.ToList(original2);
			ProcessA2(a, a2);
		}
		static void ProcessA2(G.IReadOnlyList<int> a, G.IReadOnlyList<long> a2)
		{
			var b = a.PRemoveDoubles();
			b.Sort();
			var c = E.ToList(E.Order(E.Distinct(a)));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.PRemoveDoubles(new EComparer<int>((x, y) => x / 2 == y / 2));
			b.Sort();
			c = E.ToList(E.Order(E.Distinct(a)));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.PRemoveDoubles(equalFunction: (x, y) => x / 2 == y / 2);
			b.Sort();
			c = E.ToList(E.Order(E.Distinct(a)));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.PRemoveDoubles((x, y) => x / 2 == y / 2, x => x / 2);
			b.Sort();
			c = E.ToList(E.Order(E.DistinctBy(a, x => x / 2)));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 2 == y / 2));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)));
			(b, var b2) = a.PRemoveDoubles(a2);
			b.Sort();
			b2.Sort();
			(c, var c2) = (E.ToList(E.Order(E.Distinct(a))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First))));
			Assert.IsTrue(RedStarLinq.Equals(b, c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x == y.First));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First), b));
			(b, b2) = a.PRemoveDoubles(a2, new EComparer<int>((x, y) => x / 2 == y / 2));
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.Distinct(a))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First))));
			Assert.IsTrue(RedStarLinq.Equals(b, c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x == y.First));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First), b));
			(b, b2) = a.PRemoveDoubles(a2, equalFunction: (x, y) => x / 2 == y / 2);
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.Distinct(a))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First))));
			Assert.IsTrue(RedStarLinq.Equals(b, c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x == y.First));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First), b));
			(b, b2) = a.PRemoveDoubles(a2, (x, y) => x / 2 == y / 2, x => x / 2);
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.DistinctBy(a, x => x / 2))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First / 2))));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 2 == y / 2));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x / 2 == y.First / 2));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First / 2), E.Select(b, x => x / 2)));
			b = a.PRemoveDoubles(x => x / 3);
			b.Sort();
			c = E.ToList(E.Order(E.DistinctBy(a, x => x / 3)));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 3 == y / 3));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 3 == y / 3, x => x / 3)));
			b = a.PRemoveDoubles(x => x / 3, new EComparer<int>((x, y) => x / 2 == y / 2));
			b.Sort();
			c = E.ToList(E.Order(E.DistinctBy(a, x => x / 3)));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 3 == y / 3));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 3 == y / 3, x => x / 3)));
			b = a.PRemoveDoubles(x => x / 3, equalFunction: (x, y) => x / 2 == y / 2);
			b.Sort();
			c = E.ToList(E.Order(E.DistinctBy(a, x => x / 3)));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 3 == y / 3));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 3 == y / 3, x => x / 3)));
			b = a.PRemoveDoubles(x => x / 3, (x, y) => x / 2 == y / 2, x => x / 2);
			b.Sort();
			c = E.ToList(E.Order(E.DistinctBy(a, x => x / 6)));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 6 == y / 6));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 6 == y / 6, x => x / 6)));
			(b, b2) = a.PRemoveDoubles(a2, x => x / 3);
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.DistinctBy(a, x => x / 3))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First / 3))));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 3 == y / 3));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 3 == y / 3, x => x / 3)));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x / 3 == y.First / 3));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First / 3), E.Select(b, x => x / 3)));
			(b, b2) = a.PRemoveDoubles(a2, x => x / 3, new EComparer<int>((x, y) => x / 2 == y / 2));
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.DistinctBy(a, x => x / 3))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First / 3))));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 3 == y / 3));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 3 == y / 3, x => x / 3)));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x / 3 == y.First / 3));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First / 3), E.Select(b, x => x / 3)));
			(b, b2) = a.PRemoveDoubles(a2, x => x / 3, equalFunction: (x, y) => x / 2 == y / 2);
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.DistinctBy(a, x => x / 3))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First / 3))));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 3 == y / 3));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 3 == y / 3, x => x / 3)));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x / 3 == y.First / 3));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First / 3), E.Select(b, x => x / 3)));
			(b, b2) = a.PRemoveDoubles(a2, x => x / 3, (x, y) => x / 2 == y / 2, x => x / 2);
			b.Sort();
			b2.Sort();
			(c, c2) = (E.ToList(E.Order(E.DistinctBy(a, x => x / 6))), E.ToList(E.Order(E.DistinctBy(E.Zip(a, a2), x => x.First / 6))));
			Assert.IsTrue(RedStarLinq.Equals(b, c, (x, y) => x / 6 == y / 6));
			Assert.IsTrue(E.SequenceEqual(c, b, new EComparer<int>((x, y) => x / 6 == y / 6, x => x / 6)));
			Assert.IsTrue(RedStarLinq.Equals(b, c2, (x, y) => x / 6 == y.First / 6));
			Assert.IsTrue(E.SequenceEqual(E.Select(c2, x => x.First / 6), E.Select(b, x => x / 6)));
			//b = a.PRemoveDoubles((x, index) => x + index % 10);
			//b.Sort();
			//var c3 = E.ToList(E.OrderBy(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index % 10), x => x.elem));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b));
			//b = a.PRemoveDoubles((x, index) => x + index % 10, new EComparer<int>((x, y) => x / 2 == y / 2));
			//b.Sort();
			//c3 = E.ToList(E.OrderBy(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index % 10), x => x.elem));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b));
			//b = a.PRemoveDoubles((x, index) => x + index % 10, equalFunction: (x, y) => x / 2 == y / 2);
			//b.Sort();
			//c3 = E.ToList(E.OrderBy(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index % 10), x => x.elem));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b));
			//b = a.PRemoveDoubles((x, index) => x + index % 10, (x, y) => x / 2 == y / 2, x => x / 2);
			//b.Sort();
			//c3 = E.ToList(E.OrderBy(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => (x.elem + x.index % 10) / 2), x => x.elem));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b, new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)));
			//(b, b2) = a.PRemoveDoubles(a2, (x, index) => x + index % 10);
			//b.Sort();
			//b2.Sort();
			//(c3, var c4) = (E.ToList(E.Order(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index % 10))), E.ToList(E.Order(E.Select(E.DistinctBy(E.Select(E.Zip(a, a2), (elem, index) => (elem, index)), x => x.elem.First + x.index % 10), x => (x.elem.First, x.elem.Second, x.index)))));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem + y.index % 10));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b));
			//Assert.IsTrue(RedStarLinq.Equals(b, c4, (x, y) => x == y.First + y.index % 10));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c4, x => x.First + x.index % 10), b));
			//(b, b2) = a.PRemoveDoubles(a2, (x, index) => x + index % 10, new EComparer<int>((x, y) => x / 2 == y / 2));
			//b.Sort();
			//b2.Sort();
			//(c3, c4) = (E.ToList(E.Order(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index % 10))), E.ToList(E.Order(E.Select(E.DistinctBy(E.Select(E.Zip(a, a2), (elem, index) => (elem, index)), x => x.elem.First + x.index % 10), x => (x.elem.First, x.elem.Second, x.index)))));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem + y.index % 10));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b));
			//Assert.IsTrue(RedStarLinq.Equals(b, c4, (x, y) => x == y.First + y.index % 10));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c4, x => x.First + x.index % 10), b));
			//(b, b2) = a.PRemoveDoubles(a2, (x, index) => x + index % 10, equalFunction: (x, y) => x / 2 == y / 2);
			//b.Sort();
			//b2.Sort();
			//(c3, c4) = (E.ToList(E.Order(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index % 10))), E.ToList(E.Order(E.Select(E.DistinctBy(E.Select(E.Zip(a, a2), (elem, index) => (elem, index)), x => x.elem.First + x.index % 10), x => (x.elem.First, x.elem.Second, x.index)))));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == y.elem + y.index % 10));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b));
			//Assert.IsTrue(RedStarLinq.Equals(b, c4, (x, y) => x == y.First + y.index % 10));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c4, x => x.First + x.index % 10), b));
			//(b, b2) = a.PRemoveDoubles(a2, (x, index) => x + index % 10, (x, y) => x / 2 == y / 2, x => x / 2);
			//b.Sort();
			//b2.Sort();
			//(c3, c4) = (E.ToList(E.Order(E.DistinctBy(E.Select(a, (elem, index) => (elem, index)), x => (x.elem + x.index % 10) / 2))), E.ToList(E.Order(E.Select(E.DistinctBy(E.Select(E.Zip(a, a2), (elem, index) => (elem, index)), x => (x.elem.First + x.index % 10) / 2), x => (x.elem.First, x.elem.Second, x.index)))));
			//Assert.IsTrue(RedStarLinq.Equals(b, c3, (x, y) => x == (y.elem + y.index % 10) / 2));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c3, x => x.elem + x.index % 10), b, new EComparer<int>((x, y) => x / 2 == y / 2, x => x / 2)));
			//Assert.IsTrue(RedStarLinq.Equals(b, c4, (x, y) => x / 2 == (y.First + y.index % 10) / 2));
			//Assert.IsTrue(E.SequenceEqual(E.Select(c4, x => (x.First + x.index % 10) / 2), b));
		}
	}
}

[TestClass]
public class RedStarLinqTestsN
{
	public static void Test(Action<G.IEnumerable<(char, char, char)>> action)
	{
		G.IEnumerable<(char, char, char)> a = RedStarLinq.ToList(nList);
		action(a);
		a = RedStarLinq.ToArray(nList);
		action(a);
		a = E.ToList(nList);
		action(a);
		a = nList.ToList().Insert(0, ('X', 'X', 'X')).GetSlice(1);
		action(a);
		a = nEnumerable;
		action(a);
		a = nEnumerable2;
		action(a);
		a = E.SkipWhile(nList, _ => random.Next(10) != -1);
		action(a);
	}

	[TestMethod]
	public void TestNPairs()
	{
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
		static void ProcessA(G.IEnumerable<(char, char, char)> a)
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
			Assert.ThrowsException<ArgumentNullException>(() => a.NPairs((Func<(char, char, char), (char, char, char), int>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.NPairs((Func<(char, char, char), (char, char, char), int, int>)null!));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.NPairs((x, y) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3, 0));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.NPairs((x, y, index) => x.Item1 * y.Item1 + x.Item2 * y.Item2 + x.Item3 * y.Item3 + index, 0));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.NPairs(0));
		}
	}

	[TestMethod]
	public void TestNShuffle() => Test(a =>
	{
		var c = a.NShuffle(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var d = c.ToList().Sort();
		var e = E.Order(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(d, e));
		Assert.ThrowsException<ArgumentNullException>(() => a.NShuffle((Func<(char, char, char), (char, char, char)>)null!));
		c = a.NShuffle((x, index) => (x.Item2, x.Item3, (char)index));
		d = c.ToList().Sort();
		e = E.Order(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(d, e));
		Assert.ThrowsException<ArgumentNullException>(() => a.NShuffle((Func<(char, char, char), int, (char, char, char)>)null!));
		var c2 = a.NShuffle();
		var d2 = c2.ToList().Sort();
		var e2 = E.Order(a);
		Assert.IsTrue(E.SequenceEqual(d2, e2));
	});

	[TestMethod]
	public void TestToNList() => Test(a =>
	{
		var c = a.ToNList(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var d = E.ToList(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), (char, char, char)>)null!));
		c = a.ToNList((x, index) => (x.Item2, x.Item3, (char)index));
		d = E.ToList(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), int, (char, char, char)>)null!));
	});
}
