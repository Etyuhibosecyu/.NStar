
namespace Corlib.NStar.Tests;

[TestClass]
public class SliceTests
{
	[TestMethod]
	public void TestCompare()
	{
		for (var i = 0; i < 1000; i++)
		{
			var a = new List<string>(E.Select(E.Range(0, random.Next(4, 101)), _ => random.Next(1000).ToString("D3")));
			var b = new List<string>(a.GetSlice(1));
			var n = random.Next(0, a.Length - 1);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n + 1]);
			Assert.IsTrue(a.GetSlice(1).Compare(b.GetSlice()) == n);
			a = new(E.Select(E.Range(0, random.Next(7, 102)), _ => random.Next(1000).ToString("D3")));
			b = new(a.GetSlice(2));
			n = random.Next(2, a.Length - 2);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n + 2]);
			Assert.IsTrue(a.GetSlice(2).Compare(b.GetSlice(), n - 1) == n - 1);
			a = new(E.Select(E.Range(0, random.Next(7, 102)), _ => random.Next(1000).ToString("D3")));
			b = new(a.GetSlice(2));
			var length = a.Length - 2;
			n = random.Next(2, a.Length - 2);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n + 2]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(2, E.Select(E.Range(0, index), _ => random.Next(1000).ToString("D3")));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => random.Next(1000).ToString("D3")));
			Assert.IsTrue(a.GetSlice(2).Compare(index, b.GetSlice(), otherIndex) == n);
			Assert.IsTrue(a.GetSlice(2).Compare(index, b.GetSlice(), otherIndex, length) == n);
		}
	}

	[TestMethod]
	public void TestContains()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.Contains("MMM");
			Assert.IsTrue(b);
			b = a.Contains("BBB", 2);
			Assert.IsTrue(!b);
			b = a.Contains(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.Contains(new List<string>("PPP", "DDD", "NNN"));
			Assert.IsTrue(!b);
			Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.ContainsAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.ContainsAny(new List<string>("LLL", "MMM", "NNN"));
			Assert.IsTrue(b);
			b = a.ContainsAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.IsTrue(!b);
		}
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.ContainsAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(a);
			Assert.IsTrue(!b);
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = RedStarLinq.FillArray(16, x => new string(RedStarLinq.FillArray(3, x => (char)random.Next(65536))));
			var c = (string[])b.Clone();
			var d = (string[])b.Clone();
			var e = (string[])b.Clone();
			a.CopyTo(b);
			new G.List<string>(list).CopyTo(c);
			a.CopyTo(d, 3);
			new G.List<string>(list).CopyTo(e, 3);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(E.SequenceEqual(e, d));
		}
	}

	[TestMethod]
	public void TestEndsWith()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.EndsWith("DDD");
			Assert.IsTrue(b);
			b = a.EndsWith(new List<string>("MMM", "EEE", "DDD"));
			Assert.IsTrue(b);
			b = a.EndsWith(new List<string>("PPP", "EEE", "DDD"));
			Assert.IsTrue(!b);
			b = a.EndsWith(new List<string>("MMM", "EEE", "NNN"));
			Assert.IsTrue(!b);
		}
	}

	[TestMethod]
	public void TestEquals()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.Contains("MMM");
			Assert.IsTrue(b);
			b = a.Equals(new List<string>("PPP", "DDD", "MMM"), 2);
			Assert.IsTrue(b);
			b = a.Equals(new List<string>("PPP", "DDD", "NNN"), 2);
			Assert.IsTrue(!b);
			b = a.Equals(new List<string>("PPP", "DDD", "MMM"), 3);
			Assert.IsTrue(!b);
			b = a.Equals(new List<string>("PPP", "DDD", "MMM"), 2, true);
			Assert.IsTrue(!b);
		}
	}

	[TestMethod]
	public void TestFind()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.Find(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.Find(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			b = a.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.Find(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindAll(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindAll(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			b = a.FindAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindAll(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindIndex(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindIndex(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			b = a.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindLast(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindLast(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			b = a.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindLast(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindLastIndex(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindLastIndex(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			b = a.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindLastIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.GetAfter(new List<string>("DDD"));
			var c = new G.List<string>() { "MMM", "EEE", "DDD" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfter(new());
			c = [];
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfter(new List<string>("DDD", "MMM"));
			c = ["EEE", "DDD"];
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.GetAfterLast(new List<string>("MMM"));
			var c = new G.List<string>() { "EEE", "DDD" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfterLast(new());
			c = [];
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfterLast(new List<string>("DDD", "MMM"));
			c = ["EEE", "DDD"];
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.GetBefore(new List<string>("DDD"));
			var c = new G.List<string>() { "MMM", "BBB", "PPP" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBefore(new());
			c = new(list);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBefore(new List<string>("DDD", "MMM"));
			c = ["MMM", "BBB", "PPP"];
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.GetBeforeLast(new List<string>("MMM"));
			var c = new G.List<string>() { "MMM", "BBB", "PPP", "DDD" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBeforeLast(new());
			c = new(list);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBeforeLast(new List<string>("DDD", "MMM"));
			c = ["MMM", "BBB", "PPP"];
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestGetRange()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.IndexOfAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 0);
			b = a.IndexOfAny(new List<string>("LLL", "NNN", "PPP"));
			Assert.AreEqual(b, 2);
			b = a.IndexOfAny(new[] { "LLL", "NNN", "PPP" }, 4);
			Assert.AreEqual(b, -1);
			b = a.IndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 1);
			b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, 0);
			b = a.IndexOfAnyExcluding(a);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.LastIndexOfAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 6);
			b = a.LastIndexOfAny(new List<string>("LLL", "NNN", "PPP"));
			Assert.AreEqual(b, 2);
			b = a.LastIndexOfAny(new[] { "LLL", "NNN", "EEE" }, 4);
			Assert.AreEqual(b, -1);
			b = a.LastIndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.LastIndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 5);
			b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, 6);
			b = a.LastIndexOfAnyExcluding(a);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.Skip(2);
			var c = E.Skip(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.Skip(0);
			c = E.Skip(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.Skip(1000);
			c = E.Skip(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.Skip(-4);
			c = E.Skip(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.SkipLast(2);
			var c = E.SkipLast(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.SkipLast(0);
			c = E.SkipLast(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.SkipLast(1000);
			c = E.SkipLast(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.SkipLast(-4);
			c = E.SkipLast(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.SkipWhile(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = E.ToList(E.SkipWhile(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			b = a.SkipWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') || index < 1);
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => E.All(x, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.StartsWith("MMM");
			Assert.IsTrue(b);
			b = a.StartsWith(new List<string>("MMM", "BBB", "PPP"));
			Assert.IsTrue(b);
			b = a.StartsWith(new List<string>("MMM", "BBB", "XXX"));
			Assert.IsTrue(!b);
			Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestTake()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.Take(2);
			var c = E.Take(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.Take(0);
			c = E.Take(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.Take(1000);
			c = E.Take(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.Take(-4);
			c = E.Take(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.TakeLast(2);
			var c = E.TakeLast(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.TakeLast(0);
			c = E.TakeLast(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.TakeLast(1000);
			c = E.TakeLast(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.TakeLast(-4);
			c = E.TakeLast(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").ToArray(), 1);
		ProcessA(a);
		a = new(E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.TakeWhile(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = E.ToList(E.TakeWhile(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			b = a.TakeWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index < 10);
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = E.ToList(E.TakeWhile(E.Take(c, 10), x => E.All(x, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestToArray()
	{
		int length, capacity;
		List<string> a;
		G.List<string> b;
		string[] array;
		string[] array2;
		string elem;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(51);
			capacity = length + random.Next(151);
			a = new(capacity);
			b = new(capacity);
			for (var j = 0; j < length; j++)
			{
				a.Add(elem = new((char)random.Next(33, 127), random.Next(10)));
				b.Add(elem);
			}
			array = [.. a.Insert(0, "XXX").GetSlice(1)];
			array2 = [.. b];
			Assert.IsTrue(RedStarLinq.Equals(array, array2));
			Assert.IsTrue(E.SequenceEqual(array, array2));
		}
	}
}

[TestClass]
public class BigArrayTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigArray<byte> a = new(bytes, 2, 3);
		BigArray<byte> b = new(a, 2, 3);
		Assert.IsTrue(E.SequenceEqual(a, b));
		var length = 9;
		var sourceIndex = 10;
		var destinationIndex = 15;
		a.SetRange(destinationIndex, a.GetRange(sourceIndex, length));
		Assert.IsTrue(E.SequenceEqual(a.GetRange(0, destinationIndex), E.Take(b, destinationIndex)));
		Assert.IsTrue(E.SequenceEqual(a.GetRange(destinationIndex, length), E.Take(E.Skip(b, sourceIndex), length)));
		Assert.IsTrue(RedStarLinq.Equals(a.GetRange(destinationIndex + length), E.Skip(b, destinationIndex + length)));
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(33);
			sourceIndex = random.Next(bytes.Length - length + 1);
			destinationIndex = random.Next(bytes.Length - length + 1);
			a = new(bytes, 2, 3);
			b = new(a, 2, 3);
			a.SetRange(destinationIndex, a.GetRange(sourceIndex, length));
			Assert.IsTrue(E.SequenceEqual(a.GetRange(0, destinationIndex), E.Take(b, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(a.GetRange(destinationIndex, length), E.Take(E.Skip(b, sourceIndex), length)));
			Assert.IsTrue(E.SequenceEqual(a.GetRange(destinationIndex + length), E.Skip(b, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestClear()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigArray<byte> a = new(bytes, 2, 3);
		var b = E.ToArray(bytes);
		a.Clear(24, 41);
		Array.Clear(b, 24, 41);
		Assert.IsTrue(E.SequenceEqual(a, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = a.Contains(199);
		Assert.IsTrue(b);
		b = a.Contains(171, 137);
		Assert.IsTrue(!b);
		b = a.Contains(new byte[] { 1, 66, 221 }.ToList());
		Assert.IsTrue(b);
		b = a.Contains(new byte[] { 1, 66, 220 }.ToList());
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<byte>)null!));
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = a.ContainsAny(new byte[] { 82, 245, 123 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAny(new byte[] { 8, 6, 5 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAny(new byte[] { 8, 6, 2 }.ToList());
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = a.ContainsAnyExcluding(new byte[] { 82, 245, 123 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new byte[] { 8, 6, 2 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = RedStarLinq.FillArray(256, _ => (byte)random.Next(256));
		var c = (byte[])b.Clone();
		var d = (byte[])b.Clone();
		var e = (byte[])b.Clone();
		a.CopyTo(b);
		new G.List<byte>(testBytes).CopyTo(c);
		a.CopyTo(d, 11);
		new G.List<byte>(testBytes).CopyTo(e, 11);
		Assert.IsTrue(E.SequenceEqual(testBytes, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}
}

[TestClass]
public class BigBitArrayTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigBitArray bitArray = new(bytes, 2, 6);
		BigBitArray bitArray2 = new(bitArray, 2, 6);
		Assert.IsTrue(E.SequenceEqual(bitArray, bitArray2));
		var length = 20;
		var sourceIndex = 72;
		var destinationIndex = 123;
		bitArray.SetRange(destinationIndex, bitArray.GetRange(sourceIndex, length));
		Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(0, destinationIndex), E.Take(bitArray2, destinationIndex)));
		Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex, length), E.Take(E.Skip(bitArray2, sourceIndex), length)));
		Assert.IsTrue(RedStarLinq.Equals(bitArray.GetRange(destinationIndex + length), E.Skip(bitArray2, destinationIndex + length)));
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(257);
			sourceIndex = random.Next(bytes.Length * 8 - length + 1);
			destinationIndex = random.Next(bytes.Length * 8 - length + 1);
			bitArray = new(bytes, 2, 6);
			bitArray2 = new(bitArray, 2, 6);
			bitArray.SetRange(destinationIndex, bitArray.GetRange(sourceIndex, length));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(0, destinationIndex), E.Take(bitArray2, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex, length), E.Take(E.Skip(bitArray2, sourceIndex), length)));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex + length), E.Skip(bitArray2, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestClear()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigBitArray a = new(bytes, 2, 6);
		var b = E.ToArray(E.SelectMany(bytes, x => E.Select(E.Range(0, 8), y => (x & 1 << y) != 0)));
		Assert.IsTrue(E.SequenceEqual(a, b));
		a.Clear(248, 431);
		Array.Clear(b, 248, 431);
		Assert.IsTrue(E.SequenceEqual(a, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = new BigBitArray(testBytes, 2, 6);
		var b = a.Contains(false);
		Assert.IsTrue(b);
		b = a.Contains(true, 1193);
		Assert.IsTrue(!b);
		b = a.Contains(new BitList(new byte[] { 1, 66, 221 }));
		Assert.IsTrue(b);
		b = a.Contains(new BitList(new byte[] { 1, 66, 220 }));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = new BigBitArray(testBytes, 2, 6);
		var bytes = new byte[256];
		var b = E.ToArray(E.SelectMany(bytes, x => E.Select(E.Range(0, 8), y => (x & 1 << y) != 0)));
		var c = (bool[])b.Clone();
		var d = (bool[])b.Clone();
		var e = (bool[])b.Clone();
		a.CopyTo(b);
		new G.List<bool>(testBools).CopyTo(c);
		a.CopyTo(d, 185);
		new G.List<bool>(testBools).CopyTo(e, 185);
		Assert.IsTrue(E.SequenceEqual(testBools, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestGetSmallRange()
	{
		byte[] bytes;
		BigBitArray bitArray;
		G.List<bool> bitArray2;
		int length;
		int sourceIndex;
		uint range;
		for (var i = 0; i < 1000; i++)
		{
			bytes = new byte[40];
			random.NextBytes(bytes);
			length = random.Next(33);
			sourceIndex = random.Next(bytes.Length * 8 - length);
			bitArray = new(bytes, 2, 6);
			bitArray2 = new(bitArray);
			range = bitArray.GetSmallRange(sourceIndex, length);
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray.GetRange(sourceIndex, length)));
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitArray2.GetRange(sourceIndex, length), new[] { range }.ToBitList()[..length]));
		}
		length = 32;
		for (var i = 0; i < 100; i++)
		{
			bytes = new byte[40];
			random.NextBytes(bytes);
			sourceIndex = random.Next(bytes.Length * 8 - length);
			bitArray = new(bytes, 2, 6);
			bitArray2 = new(bitArray);
			range = bitArray.GetSmallRange(sourceIndex, length);
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray.GetRange(sourceIndex, length)));
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitArray2.GetRange(sourceIndex, length), new[] { range }.ToBitList()[..length]));
		}
	}
}
