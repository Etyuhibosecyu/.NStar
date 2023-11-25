﻿namespace Corlib.NStar.Tests;

[TestClass]
public class RedStarLinqTests
{
	[TestMethod]
	public void TestAll()
	{
		G.IEnumerable<string> a = list.ToList();
		ProcessA(a);
		a = list.ToArray();
		ProcessA(a);
		a = E.ToList(list);
		ProcessA(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
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
		}
	}

	[TestMethod]
	public void TestAny()
	{
		G.IEnumerable<string> a = list.ToList();
		ProcessA(a);
		a = list.ToArray();
		ProcessA(a);
		a = E.ToList(list);
		ProcessA(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
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
			a = list.ToArray();
			c = a.Any();
			d = E.Any(a);
			Assert.AreEqual(c, d);
			a = E.ToList(list);
			c = a.Any();
			d = E.Any(a);
			Assert.AreEqual(c, d);
		}
	}

	[TestMethod]
	public void TestBreak()
	{
		G.IEnumerable<string> a = list2.ToList();
		ProcessA(a);
		a = list2.ToArray();
		ProcessA(a);
		a = E.ToList(list2);
		ProcessA(a);
		a = list2.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list2, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
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
		}
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		G.IEnumerable<string> a = list.ToList();
		ProcessA(a);
		a = list.ToArray();
		ProcessA(a);
		a = E.ToList(list);
		ProcessA(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
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
		}
	}

	[TestMethod]
	public void TestCombine()
	{
		G.IEnumerable<string> a = list2.ToList();
		ProcessA(a);
		a = list2.ToArray();
		ProcessA(a);
		a = E.ToList(list2);
		ProcessA(a);
		a = list2.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list2, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
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
		}
		static void ProcessB(G.IEnumerable<string> a, G.IEnumerable<string> b)
		{
			var c = a.Combine(b, (x, y) => x + y);
			var d = E.Zip(a, b, (x, y) => x + y);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Combine(b, (x, y, index) => x + y + index.ToString());
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => x.elem + y + x.index.ToString());
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Combine(b);
			var d2 = E.Zip(a, b);
			Assert.IsTrue(c2.Equals(d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, (Func<string, string, string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, (Func<string, string, int, string>)null!));
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
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Combine(b, b2, (x, y, z, index) => x + y + z + index.ToString());
			d = E.Zip(E.Zip(E.Select(a, (elem, index) => (elem, index)), b, (x, y) => (x.elem + y, x.index)), b2, (x, y) => x.Item1 + y + x.index.ToString());
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Combine(b, b2);
			var d2 = E.Zip(a, b, b2);
			Assert.IsTrue(c2.Equals(d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, b2, (Func<string, string, string, string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Combine(b, b2, (Func<string, string, string, int, string>)null!));
		}
	}

	[TestMethod]
	public void TestConvert()
	{
		G.IEnumerable<string> a = list2.ToList();
		ProcessA(a);
		a = list2.ToArray();
		ProcessA(a);
		a = E.ToList(list2);
		ProcessA(a);
		a = list2.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list2, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
		{
			var c = a.Convert(x => x[1..]);
			var d = E.Select(a, x => x[1..]);
			Assert.IsTrue(E.SequenceEqual(c, d));
			Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, string>)null!));
			c = a.Convert((x, index) => x[1..] + index.ToString("D2"));
			d = E.Select(a, (x, index) => x[1..] + index.ToString("D2"));
			Assert.IsTrue(E.SequenceEqual(c, d));
			Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, int, string>)null!));
		}
	}

	[TestMethod]
	public void TestConvertAndJoin()
	{
		G.IEnumerable<string> a = list2.ToList();
		ProcessA(a);
		a = list2.ToArray();
		ProcessA(a);
		a = E.ToList(list2);
		ProcessA(a);
		a = list2.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list2, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
		{
			var c = a.ConvertAndJoin(x => x[1..]);
			var d = E.SelectMany(a, x => x[1..]);
			Assert.IsTrue(E.SequenceEqual(c, d));
			Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, string>)null!));
			c = a.ConvertAndJoin((x, index) => x[1..] + index.ToString("D2"));
			d = E.SelectMany(a, (x, index) => x[1..] + index.ToString("D2"));
			Assert.IsTrue(E.SequenceEqual(c, d));
			Assert.ThrowsException<ArgumentNullException>(() => a.Convert((Func<string, int, string>)null!));
		}
	}

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
			b = new List<string>();
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = Array.Empty<string>();
			Assert.AreEqual(RedStarLinq.Equals(a, b), E.SequenceEqual(a, b));
			b = new G.List<string>();
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
	public void TestFilter()
	{
		G.IEnumerable<string> a = list.ToList();
		ProcessA(a);
		a = list.ToArray();
		ProcessA(a);
		a = E.ToList(list);
		ProcessA(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
		{
			var c = a.Filter(x => x.Length > 0);
			var d = E.Where(a, x => x.Length > 0);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Filter(x => x.StartsWith('#'));
			d = E.Where(a, x => x.StartsWith('#'));
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Filter(x => x.StartsWith('M'));
			d = E.Where(a, x => x.StartsWith('M'));
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			Assert.ThrowsException<ArgumentNullException>(() => a.Filter((Func<string, bool>)null!));
			c = a.Filter((x, index) => x.Length > 0 && index >= 0);
			d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.elem.Length > 0 && x.index >= 0), x => x.elem);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Filter((x, index) => index < 0);
			d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.index < 0), x => x.elem);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Filter((x, index) => x.StartsWith('M') && index > 0);
			d = E.Select(E.Where(E.Select(a, (elem, index) => (elem, index)), x => x.elem.StartsWith('M') && x.index > 0), x => x.elem);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			Assert.ThrowsException<ArgumentNullException>(() => a.Filter((Func<string, int, bool>)null!));
		}
	}

	[TestMethod]
	public void TestPairs()
	{
		G.IEnumerable<string> a = list.ToList();
		ProcessA(a);
		a = list.ToArray();
		ProcessA(a);
		a = E.ToList(list);
		ProcessA(a);
		a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = enumerable;
		ProcessA(a);
		a = enumerable2;
		ProcessA(a);
		a = E.SkipWhile(list, _ => random.Next(10) != -1);
		ProcessA(a);
		static void ProcessA(G.IEnumerable<string> a)
		{
			var c = a.Pairs((x, y) => x + y);
			var d = E.Zip(a, E.Skip(a, 1), (x, y) => x + y);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Pairs((x, y, index) => x + y + index.ToString());
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 1), (x, y) => x.elem + y + x.index.ToString());
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			var c2 = a.Pairs();
			var d2 = E.Zip(a, E.Skip(a, 1), (x, y) => (x, y));
			Assert.IsTrue(c2.Equals(d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			c = a.Pairs((x, y) => x + y, 3);
			d = E.Zip(a, E.Skip(a, 3), (x, y) => x + y);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.Pairs((x, y, index) => x + y + index.ToString(), 3);
			d = E.Zip(E.Select(a, (elem, index) => (elem, index)), E.Skip(a, 3), (x, y) => x.elem + y + x.index.ToString());
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c2 = a.Pairs(3);
			d2 = E.Zip(a, E.Skip(a, 3), (x, y) => (x, y));
			Assert.IsTrue(c2.Equals(d2));
			Assert.IsTrue(E.SequenceEqual(d2, c2));
			Assert.ThrowsException<ArgumentNullException>(() => a.Pairs((Func<string, string, string>)null!));
			Assert.ThrowsException<ArgumentNullException>(() => a.Pairs((Func<string, string, int, string>)null!));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs((x, y) => x + y, 0));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs((x, y, index) => x + y + index.ToString(), 0));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pairs(0));
		}
	}

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
			ProcessA(original6, new BitList(new[] { 1234567890, 1234567890 }));
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
			Assert.IsTrue(RedStarLinq.Equals(c, b));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(RedStarLinq.Equals(d, b));
		}
	}

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
			Assert.IsTrue(RedStarLinq.Equals(c, b));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(RedStarLinq.Equals(d, b));
		}
	}
}

[TestClass]
public class RedStarLinqTestsN
{
	[TestMethod]
	public void TestNPairs()
	{
		G.IEnumerable<(char, char, char)> a = new List<(char, char, char)>(nList);
		ProcessA(a);
		a = nList.ToArray();
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
}
