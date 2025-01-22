
namespace Corlib.NStar.Tests;

[TestClass]
public class PrependTests
{
	[TestMethod]
	public void TestContains()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestContains();
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.ContainsAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.ContainsAny(new List<string>("LLL", "MMM", "NNN"));
			Assert.IsTrue(b);
			b = a.ContainsAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.IsFalse(b);
		}
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.ContainsAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(a);
			Assert.IsFalse(b);
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestEndsWith();
	}

	[TestMethod]
	public void TestEquals()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestEquals();
	}

	[TestMethod]
	public void TestFind()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.Find(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			var d = c.Find(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
			b = a.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			d = c.Find(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestFindAll();
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindIndex(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			var d = c.FindIndex(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
			b = a.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			d = c.FindIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindLast(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			var d = c.FindLast(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
			b = a.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			d = c.FindLast(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.FindLastIndex(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			var d = c.FindLastIndex(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
			b = a.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			d = c.FindLastIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.IndexOfAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(0, b);
			b = a.IndexOfAny(new List<string>("LLL", "NNN", "PPP"));
			Assert.AreEqual(2, b);
			b = a.IndexOfAny(new[] { "LLL", "NNN", "PPP" }, 4);
			Assert.AreEqual(-1, b);
			b = a.IndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(-1, b);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(1, b);
			b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(0, b);
			b = a.IndexOfAnyExcluding(a);
			Assert.AreEqual(-1, b);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.LastIndexOfAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(6, b);
			b = a.LastIndexOfAny(new List<string>("LLL", "NNN", "PPP"));
			Assert.AreEqual(2, b);
			b = a.LastIndexOfAny(new[] { "LLL", "NNN", "EEE" }, 4);
			Assert.AreEqual(-1, b);
			b = a.LastIndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(-1, b);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.LastIndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(5, b);
			b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(6, b);
			b = a.LastIndexOfAnyExcluding(a);
			Assert.AreEqual(-1, b);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.SkipWhile(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			var d = E.ToList(E.SkipWhile(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			b = a.SkipWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') || index < 1);
			c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.StartsWith("MMM");
			Assert.IsTrue(b);
			b = a.StartsWith(new List<string>("MMM", "BBB", "PPP"));
			Assert.IsTrue(b);
			b = a.StartsWith(new List<string>("MMM", "BBB", "XXX"));
			Assert.IsFalse(b);
			Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestTake()
	{
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Prepend("XXX").GetRange(1).GetSlice();
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
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX")), 1);
		ProcessA(a);
		a = list.ToList().Insert(3, new List<string>("$", "###")).Prepend("XXX").GetRange(1).GetSlice();
		ProcessA(a);
		static void ProcessA(Slice<string> a)
		{
			var b = a.TakeWhile(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
			var d = E.ToList(E.TakeWhile(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			b = a.TakeWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index < 10);
			c = new G.List<string>(list);
			c.InsertRange(3, ["$", "###"]);
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
			array = [.. a.Prepend("XXX").GetSlice(1)];
			array2 = [.. b];
			Assert.IsTrue(RedStarLinq.Equals(array, array2));
			Assert.IsTrue(E.SequenceEqual(array, array2));
		}
	}
}
