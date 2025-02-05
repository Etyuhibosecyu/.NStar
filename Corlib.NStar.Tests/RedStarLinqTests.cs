
namespace Corlib.NStar.Tests;

[TestClass]
public class RedStarLinqTests
{
	public static void Test(Action<G.IEnumerable<string>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(101), _ => new string((char)random.Next('A', 'Z' + 1), random.Next(1, 6)));
			G.IEnumerable<string> a = RedStarLinq.ToList(array);
			action(a);
			a = RedStarLinq.ToArray(array);
			action(a);
			a = E.ToList(array);
			action(a);
			a = array.ToList().Insert(0, "XXX").GetSlice(1);
			action(a);
			a = array.Prepend("XXX");
			action(a);
			a = enumerable;
			action(a);
			a = enumerable2;
			action(a);
			a = E.SkipWhile(array, _ => random.Next(10) != -1);
			action(a);
		}
	}

	public static void Test2(Action<G.IReadOnlyList<string>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(1001), _ => new string((char)random.Next('A', 'Z' + 1), 3));
			G.IReadOnlyList<string> a = RedStarLinq.ToList(array);
			action(a);
			a = RedStarLinq.ToArray(array);
			action(a);
			a = E.ToList(array);
			action(a);
			a = array.ToList().Insert(0, "XXX").GetSlice(1);
			action(a);
			a = array.Prepend("XXX");
			action(a);
		}
	}

	public static void TestN(Action<G.IEnumerable<(char, char, char)>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			char @char;
			var array = RedStarLinq.FillArray(random.Next(1001), _ => (@char = (char)random.Next('A', 'Z' + 1), @char, @char));
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

	public static void Test2N(Action<G.IReadOnlyList<(char, char, char)>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			char @char;
			var array = RedStarLinq.FillArray(random.Next(1001), _ => (@char = (char)random.Next('A', 'Z' + 1), @char, @char));
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

	public static void TestS(Action<G.IEnumerable<char>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
			G.IEnumerable<char> a = RedStarLinq.ToList(array);
			action(a);
			a = RedStarLinq.ToArray(array);
			action(a);
			a = E.ToList(array);
			action(a);
			a = array.ToList().Insert(0, 'X').GetSlice(1);
			action(a);
			a = array.Prepend('X');
			action(a);
			a = nSEnumerable;
			action(a);
			a = nSEnumerable2;
			action(a);
			a = E.SkipWhile(array, _ => random.Next(10) != -1);
			action(a);
		}
	}

	public static void Test2S(Action<G.IReadOnlyList<char>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var array = RedStarLinq.FillArray(random.Next(1001), _ => (char)random.Next('A', 'Z' + 1));
			G.IReadOnlyList<char> a = RedStarLinq.ToList(array);
			action(a);
			a = RedStarLinq.ToArray(array);
			action(a);
			a = E.ToList(array);
			action(a);
			a = array.ToList().Insert(0, 'X').GetSlice(1);
			action(a);
			a = array.Prepend('X');
			action(a);
		}
	}

	[TestMethod]
	public void TestAll() => Test(a =>
	{
		var c = a.All(x => x.Length > 0);
		var d = E.All(a, x => x.Length > 0);
		Assert.AreEqual(d, c);
		c = a.All(x => x.StartsWith('#'));
		d = E.All(a, x => x.StartsWith('#'));
		Assert.AreEqual(d, c);
		c = a.All(x => x.StartsWith('M'));
		d = E.All(a, x => x.StartsWith('M'));
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.All((Func<string, bool>)null!));
		c = a.All((x, index) => x.Length > 0 && index >= 0);
		d = E.All(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0);
		Assert.AreEqual(d, c);
		c = a.All((x, index) => index < 0);
		d = E.All(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0);
		Assert.AreEqual(d, c);
		c = a.All((x, index) => x.StartsWith('M') && index > 0);
		d = E.All(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0);
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.All((Func<string, int, bool>)null!));
	});

	[TestMethod]
	public void TestAllEqual()
	{
		Test(a =>
	{
		var b = a.AllEqual();
		var c = E.All(E.Zip(a, E.Skip(a, 1)), x => x.First == x.Second);
		Assert.AreEqual(c, b);
		Assert.ThrowsException<ArgumentNullException>(() => a.AllEqual((Func<string, string>)null!));
		b = a.AllEqual((x, y) => x == y);
		Assert.AreEqual(c, b);
		Assert.ThrowsException<ArgumentNullException>(() => a.AllEqual((Func<string, string, bool>)null!));
		b = a.AllEqual(x => x.Length);
		c = E.All(E.Zip(a, E.Skip(a, 1)), x => x.First.Length == x.Second.Length);
		Assert.AreEqual(c, b);
		b = a.AllEqual((x, y) => x.Length == y.Length);
		Assert.AreEqual(c, b);
		b = a.AllEqual((x, y, index) => x.Length == y.Length && index < 10);
		c = E.All(E.Zip(a, E.Skip(a, 1)), x => x.First.Length == x.Second.Length) && E.Count(a) <= 11;
		Assert.AreEqual(c, b);
		b = a.AllEqual((x, y, index) => x.Length == y.Length && index < 0);
		c = E.Count(a) <= 1;
		Assert.AreEqual(c, b);
		b = a.AllEqual((x, y, index) => x.Length == y.Length && index > 0);
		Assert.AreEqual(c, b);
		Assert.ThrowsException<ArgumentNullException>(() => a.AllEqual((Func<string, string, int, bool>)null!));
	});
		int[] arr = [3, 3, 3, 3, 3];
		var c = arr.AllEqual();
		Assert.IsTrue(c);
		arr = [3, 4, 5, 6, 7];
		c = arr.AllEqual();
		Assert.IsFalse(c);
		arr = [3, 3, 3, 3, -42];
		c = arr.AllEqual();
		Assert.IsFalse(c);
		arr = [3, 4, 5, 6, 4];
		c = arr.AllUnique();
		Assert.IsFalse(c);
	}

	[TestMethod]
	public void TestAllUnique()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		Test(a =>
		{
			var b = a.AllUnique(x => x.Length);
			var c = E.Count(E.Distinct(E.Select(a, x => x.Length))) == E.Count(E.Select(a, x => x.Length));
			Assert.AreEqual(c, b);
			b = a.AllUnique((x, index) => index);
			Assert.IsTrue(b);
			b = a.AllUnique();
			c = E.Count(E.Distinct(a)) == E.Count(a);
			Assert.AreEqual(c, b);
			b = E.Distinct(a).AllUnique(x => x);
			Assert.IsTrue(b);
			b = E.Distinct(a).AllUnique();
			Assert.IsTrue(b);
			Assert.ThrowsException<ArgumentNullException>(() => a.AllUnique((Func<string, string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.AllUnique((Func<string, int, string>)null!));
		});
		ProcessA(["3", "3", "3", "3", "3"]);
		ProcessA(["3", "4", "5", "6", "7"]);
		ProcessA(["3", "3", "3", "3", "-42"]);
		ProcessA(["3", "4", "5", "6", "4"]);
		void ProcessA(string[] array)
		{
			G.IEnumerable<string> a = RedStarLinq.ToList(array);
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = RedStarLinq.ToArray(array);
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = E.ToList(array);
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = array.ToList().Insert(0, "XXX").GetSlice(1);
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = array.Prepend("XXX");
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = enumerable;
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = enumerable2;
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
			a = E.SkipWhile(array, _ => random.Next(10) != -1);
			Assert.AreEqual(a.AllUnique(), E.Count(E.Distinct(a)) == E.Count(a));
		}
	}

	[TestMethod]
	public void TestAny() => Test(a =>
	{
		var c = a.Any(x => x.Length > 0);
		var d = E.Any(a, x => x.Length > 0);
		Assert.AreEqual(d, c);
		c = a.Any(x => x.StartsWith('#'));
		d = E.Any(a, x => x.StartsWith('#'));
		Assert.AreEqual(d, c);
		c = a.Any(x => x.StartsWith('M'));
		d = E.Any(a, x => x.StartsWith('M'));
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.Any((Func<string, bool>)null!));
		c = a.Any((x, index) => x.Length > 0 && index >= 0);
		d = E.Any(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0);
		Assert.AreEqual(d, c);
		c = a.Any((x, index) => index < 0);
		d = E.Any(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0);
		Assert.AreEqual(d, c);
		c = a.Any((x, index) => x.StartsWith('M') && index > 0);
		d = E.Any(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0);
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.Any((Func<string, int, bool>)null!));
		c = a.Any();
		d = E.Any(a);
		Assert.AreEqual(d, c);
		a = RedStarLinq.ToArray(list);
		c = a.Any();
		d = E.Any(a);
		Assert.AreEqual(d, c);
		a = E.ToList(list);
		c = a.Any();
		d = E.Any(a);
		Assert.AreEqual(d, c);
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
		void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
		void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
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
			Assert.AreEqual(d, c);
			c = a.Contains(s, new EComparer<string>((x, y) => x == y));
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y));
			Assert.AreEqual(d, c);
			c = a.Contains(s, (x, y) => x == y);
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y));
			Assert.AreEqual(d, c);
			c = a.Contains(s, new EComparer<string>((x, y) => x == y, x => 42));
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y, x => 42));
			Assert.AreEqual(d, c);
			c = a.Contains(s, (x, y) => x == y, x => 42);
			d = E.Contains(a, s, new EComparer<string>((x, y) => x == y, x => 42));
			Assert.AreEqual(d, c);
			c = a.Contains(s, new EComparer<string>((x, y) => false));
			d = E.Contains(a, s, new EComparer<string>((x, y) => false));
			Assert.AreEqual(d, c);
			c = a.Contains(s, (x, y) => false);
			d = E.Contains(a, s, new EComparer<string>((x, y) => false));
			Assert.AreEqual(d, c);
			c = a.Contains(s, new EComparer<string>((x, y) => false, x => 42));
			d = E.Contains(a, s, new EComparer<string>((x, y) => false, x => 42));
			Assert.AreEqual(d, c);
			c = a.Contains(s, (x, y) => false, x => 42);
			d = E.Contains(a, s, new EComparer<string>((x, y) => false, x => 42));
			Assert.AreEqual(d, c);
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
	public void TestCountOrLength() => Test(a =>
	{
		var c = a.Count("MMM");
		var d = E.Count(a, x => x == "MMM");
		Assert.AreEqual(d, c);
		c = a.Count(x => x is "MMM" or "PPP");
		d = E.Count(a, x => x is "MMM" or "PPP");
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.Count((Func<string, bool>)null!));
		c = a.Count((x, index) => (x[0] + index) % 2 == 1);
		d = E.Count(E.Where(a, (x, index) => (x[0] + index) % 2 == 1));
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.Count((Func<string, int, bool>)null!));
		c = a.Length();
		d = E.Count(a);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestCountOrLength2() => Test2(a =>
	{
		var c = a.Count("MMM");
		var d = E.Count(a, x => x == "MMM");
		Assert.AreEqual(d, c);
		c = a.Count(x => x is "MMM" or "PPP");
		d = E.Count(a, x => x is "MMM" or "PPP");
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.Count((Func<string, bool>)null!));
		c = a.Count((x, index) => (x[0] + index) % 2 == 1);
		d = E.Count(E.Where(a, (x, index) => (x[0] + index) % 2 == 1));
		Assert.AreEqual(d, c);
		Assert.ThrowsException<ArgumentNullException>(() => a.Count((Func<string, int, bool>)null!));
		c = a.Length();
#pragma warning disable
		d = E.Count(a);
#pragma warning restore
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
		void ProcessA(G.IEnumerable<string> a)
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
		void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
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
	public void TestFill()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var @char = (char)random.Next('A', 'Z' + 1);
			string @string = new(@char, 3);
			var length = random.Next(1001);
			var a = RedStarLinq.Fill(@string, length);
			var b = E.ToList(E.Repeat(@string, length));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.Fill(@string, -5));
			a = RedStarLinq.Fill(index => @string.ToString(x => (char)(x + index)), length);
			b = E.ToList(E.Select(E.Range(0, length), index => @string.ToString(x => (char)(x + index))));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.Fill(index => @string.ToString(x => (char)(x + index)), -5));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.Fill((Func<int, string>)null!, length));
			a = RedStarLinq.Fill(length, index => @string.ToString(x => (char)(x + index)));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.Fill(-5, index => @string.ToString(x => (char)(x + index))));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.Fill(length, (Func<int, string>)null!));
			var b2 = RedStarLinq.FillArray(@string, length);
			var c2 = E.ToArray(E.Repeat(@string, length));
			Assert.IsTrue(RedStarLinq.Equals(b2, c2));
			Assert.IsTrue(E.SequenceEqual(c2, b2));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.FillArray(@string, -5));
			b2 = RedStarLinq.FillArray(index => @string.ToString(x => (char)(x + index)), length);
			c2 = E.ToArray(E.Select(E.Range(0, length), index => @string.ToString(x => (char)(x + index))));
			Assert.IsTrue(RedStarLinq.Equals(b2, c2));
			Assert.IsTrue(E.SequenceEqual(c2, b2));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.FillArray(index => @string.ToString(x => (char)(x + index)), -5));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.FillArray((Func<int, string>)null!, length));
			b2 = RedStarLinq.FillArray(length, index => @string.ToString(x => (char)(x + index)));
			Assert.IsTrue(RedStarLinq.Equals(b2, c2));
			Assert.IsTrue(E.SequenceEqual(c2, b2));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.FillArray(-5, index => @string.ToString(x => (char)(x + index))));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.FillArray(length, (Func<int, string>)null!));
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
	public void TestFrequencyTable()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		EComparer<int> comparer = new((x, y) => x / 3 == y / 3), comparer2 = new((x, y) => x / 3 == y / 3, x => 42), comparer3 = new((x, y) => x / 3 == y / 3, x => x / 4);
		for (var i = 0; i < 1000; i++)
		{
			var original = E.ToArray(E.Select(E.Range(0, random.Next(101)), _ => random.Next(-30, 30)));
			G.IEnumerable<int> a = new List<int>(original);
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
			var c = a.FrequencyTable();
			var d = E.GroupBy(a, x => x);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2);
			d = E.GroupBy(a, x => x / 2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(comparer);
			d = E.GroupBy(a, x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2, comparer);
			d = E.GroupBy(a, x => x / 2, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10, comparer);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(equalFunction: (x, y) => x / 3 == y / 3);
			d = E.GroupBy(a, x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x, equalFunction: (x, y) => x / 3 == y / 3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.GroupBy(a, x => x / 2, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(comparer2);
			d = E.GroupBy(a, x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2, comparer2);
			d = E.GroupBy(a, x => x / 2, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10, comparer2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.GroupBy(a, x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.GroupBy(a, x => x / 2, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(comparer3);
			d = E.GroupBy(a, x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2, comparer3);
			d = E.GroupBy(a, x => x / 2, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10, comparer3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.GroupBy(a, x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.GroupBy(a, x => x / 2, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
			c = a.FrequencyTable((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Count == E.Count(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.Count(x) == y.Count), x => x));
		}
	}

	[TestMethod]
	public void TestGroup()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		EComparer<int> comparer = new((x, y) => x / 3 == y / 3), comparer2 = new((x, y) => x / 3 == y / 3, x => 42), comparer3 = new((x, y) => x / 3 == y / 3, x => x / 4);
		for (var i = 0; i < 1000; i++)
		{
			var original = E.ToArray(E.Select(E.Range(0, random.Next(101)), _ => random.Next(-30, 30)));
			G.IEnumerable<int> a = new List<int>(original);
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
			var c = a.Group(x => x);
			var d = E.GroupBy(a, x => x);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2);
			d = E.GroupBy(a, x => x / 2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(comparer);
			d = E.GroupBy(a, x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2, comparer);
			d = E.GroupBy(a, x => x / 2, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10, comparer);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(equalFunction: (x, y) => x / 3 == y / 3);
			d = E.GroupBy(a, x => x, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x, equalFunction: (x, y) => x / 3 == y / 3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.GroupBy(a, x => x / 2, comparer);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(comparer2);
			d = E.GroupBy(a, x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2, comparer2);
			d = E.GroupBy(a, x => x / 2, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10, comparer2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.GroupBy(a, x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.GroupBy(a, x => x / 2, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(comparer3);
			d = E.GroupBy(a, x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2, comparer3);
			d = E.GroupBy(a, x => x / 2, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10, comparer3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.GroupBy(a, x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.GroupBy(a, x => x / 2, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.Group((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.elem), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
		}
	}

	[TestMethod]
	public void TestGroupIndexes()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		EComparer<int> comparer = new((x, y) => x / 3 == y / 3), comparer2 = new((x, y) => x / 3 == y / 3, x => 42), comparer3 = new((x, y) => x / 3 == y / 3, x => x / 4);
		for (var i = 0; i < 1000; i++)
		{
			var original = E.ToArray(E.Select(E.Range(0, random.Next(101)), _ => random.Next(-30, 30)));
			G.IEnumerable<int> a = new List<int>(original);
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
			var c = a.GroupIndexes();
			var d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(comparer);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2, comparer);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10, comparer);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(equalFunction: (x, y) => x / 3 == y / 3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x, equalFunction: (x, y) => x / 3 == y / 3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(comparer2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x, comparer2);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2, comparer2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10, comparer2);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => 42);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer2), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(comparer3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x, comparer3);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2, comparer3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10, comparer3);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes(x => x / 2, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem / 2, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
			c = a.GroupIndexes((x, index) => x + index / 10, equalFunction: (x, y) => x / 3 == y / 3, x => x / 4);
			d = E.Select(E.GroupBy(E.Select(a, (elem, index) => (elem, index)), x => x.elem + x.index / 10, comparer3), x => E.First(E.GroupBy(E.Select(x, y => y.index), y => x.Key)));
			Assert.IsTrue(c.Equals(d, (x, y) => x.Equals(y)));
			Assert.IsTrue(E.All(E.Zip(d, c, (x, y) => E.SequenceEqual(x, y)), x => x));
		}
	}

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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var b = a.Shuffle(x => x[1..], random);
		var c = b.NSort();
		var d = E.Order(E.Select(a, x => x[1..]));
		Assert.IsTrue(E.SequenceEqual(b, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Shuffle((Func<string, string>)null!));
		b = a.Shuffle((x, index) => x[1..] + index.ToString("D2"), random);
		c = b.NSort();
		d = E.Order(E.Select(a, (x, index) => x[1..] + index.ToString("D2")));
		Assert.IsTrue(E.SequenceEqual(b, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.Shuffle((Func<string, int, string>)null!));
		var b2 = a.Shuffle(random);
		var c2 = b2.NSort();
		var d2 = E.Order(a);
		Assert.IsTrue(E.SequenceEqual(b2, d2));
	});

	[TestMethod]
	public void TestSkip() => Test(a =>
	{
		var b = a.Skip(3);
		var c = E.Skip(a, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Skip(1000);
		c = E.Skip(a, 1000);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Skip(-2);
		c = E.Skip(a, -2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestSkipLast() => Test(a =>
	{
		var b = a.SkipLast(4);
		var c = E.SkipLast(a, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.SkipLast(1000);
		c = E.SkipLast(a, 1000);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.SkipLast(-5);
		c = E.SkipLast(a, -5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestTake() => Test(a =>
	{
		var b = a.Take(3);
		var c = E.Take(a, 3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(1000);
		c = E.Take(a, 1000);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(-2);
		c = E.Take(a, -2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(2..5);
		c = E.Take(a, 2..5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(5..2);
		c = E.Take(a, 5..2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(2..^3);
		c = E.Take(a, 2..^3);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(5..^4);
		c = E.Take(a, 5..^4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(^5..6);
		c = E.Take(a, ^5..6);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(^10..9);
		c = E.Take(a, ^10..9);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Take(^5..^2);
		c = E.Take(a, ^5..^2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestTakeLast() => Test(a =>
	{
		var b = a.TakeLast(4);
		var c = E.TakeLast(a, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.TakeLast(1000);
		c = E.TakeLast(a, 1000);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.TakeLast(-5);
		c = E.TakeLast(a, -5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestToArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
			var original6 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => new BitList([random.Next(), random.Next()])));
			ProcessA(original6, new BitList((int[])[1234567890, 1234567890]));
		}
		void ProcessA<T>(ImmutableArray<T> original, T @default)
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
			var original6 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => new BitList([random.Next(), random.Next()])));
			ProcessA(original6, new BitList((int[])[1234567890, 1234567890]));
		}
		void ProcessA<T>(ImmutableArray<T> original, T @default)
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => random.Next()));
			ProcessA(original, 1234567890);
			var original2 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (long)random.Next() << 31 | (uint)random.Next()));
			ProcessA(original2, 1234567890123456789);
			var original3 = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (random.Next(), random.Next())));
			ProcessA(original3, (1234567890, 1234567890));
		}
		void ProcessA<T>(ImmutableArray<T> original, T @default) where T : unmanaged
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var original = ImmutableArray.Create(RedStarLinq.FillArray(random.Next(17), _ => (char)random.Next(65536)));
			ProcessA(original, (char)12345);
		}
		void ProcessA(ImmutableArray<char> original, char @default)
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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

	[TestMethod]
	public void TestPFill()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var @char = (char)random.Next('A', 'Z' + 1);
			var @string = (@char, @char, @char);
			var length = random.Next(1001);
			var a = RedStarLinq.PFill(@string, length);
			var b = E.ToList(E.Repeat(@string, length));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.PFill(@string, -5));
			a = RedStarLinq.PFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), length);
			b = E.ToList(E.Select(E.Range(0, length), index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.PFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), -5));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.PFill((Func<int, (char, char, char)>)null!, length));
			a = RedStarLinq.PFill(length, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.PFill(-5, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.PFill(length, (Func<int, (char, char, char)>)null!));
		}
	}
}

[TestClass]
public class RedStarLinqTestsN
{
	public static void Test(Action<G.IEnumerable<(char, char, char)>> action)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.NFill(@string, -5));
			a = RedStarLinq.NFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), length);
			b = E.ToList(E.Select(E.Range(0, length), index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.NFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), -5));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.NFill((Func<int, (char, char, char)>)null!, length));
			a = RedStarLinq.NFill(length, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.NFill(-5, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.NFill(length, (Func<int, (char, char, char)>)null!));
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
			G.IEnumerable<int> a = new List<int>(original);
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
		var b = a.NShuffle(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var c = b.ToList().Sort();
		var d = E.Order(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.NShuffle((Func<(char, char, char), (char, char, char)>)null!));
		b = a.NShuffle((x, index) => (x.Item2, x.Item3, (char)index));
		c = b.ToList().Sort();
		d = E.Order(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(c, d));
		Assert.ThrowsException<ArgumentNullException>(() => a.NShuffle((Func<(char, char, char), int, (char, char, char)>)null!));
		var b2 = a.NShuffle();
		var c2 = b2.ToList().Sort();
		var d2 = E.Order(a);
		Assert.IsTrue(E.SequenceEqual(c2, d2));
	});

	[TestMethod]
	public void TestToNList() => Test(a =>
	{
		var b = a.ToNList(x => (x.Item2, x.Item3, (char)(x.Item3 + 123)));
		var c = E.ToList(E.Select(a, x => (x.Item2, x.Item3, (char)(x.Item3 + 123))));
		Assert.IsTrue(E.SequenceEqual(b, c));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), (char, char, char)>)null!));
		b = a.ToNList((x, index) => (x.Item2, x.Item3, (char)index));
		c = E.ToList(E.Select(a, (x, index) => (x.Item2, x.Item3, (char)index)));
		Assert.IsTrue(E.SequenceEqual(b, c));
		Assert.ThrowsException<ArgumentNullException>(() => a.ToNList((Func<(char, char, char), int, (char, char, char)>)null!));
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
			var a = RedStarLinq.PNFill(@string, length);
			var b = E.ToList(E.Repeat(@string, length));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.PNFill(@string, -5));
			a = RedStarLinq.PNFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), length);
			b = E.ToList(E.Select(E.Range(0, length), index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.PNFill(index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)), -5));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.PNFill((Func<int, (char, char, char)>)null!, length));
			a = RedStarLinq.PNFill(length, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index)));
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => RedStarLinq.PNFill(-5, index => ((char)(@char + index), (char)(@char + index), (char)(@char + index))));
			Assert.ThrowsException<ArgumentNullException>(() => RedStarLinq.PNFill(length, (Func<int, (char, char, char)>)null!));
		}
	}
}
