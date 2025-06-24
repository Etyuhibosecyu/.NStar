namespace NStar.Core.Tests;

[TestClass]
public class SliceTests
{
	[TestMethod]
	public void TestCompare()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new List<string>(E.Select(E.Range(0, random.Next(4, 101)), _ => random.Next(1000).ToString("D3")));
			var b = new List<string>(a.GetSlice(1));
			var n = random.Next(0, a.Length - 1);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n + 1]);
			Assert.AreEqual(n, a.GetSlice(1).Compare(b.GetSlice()));
			a = new(E.Select(E.Range(0, random.Next(7, 102)), _ => random.Next(1000).ToString("D3")));
			b = new(a.GetSlice(2));
			n = random.Next(2, a.Length - 2);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n + 2]);
			Assert.AreEqual(n - 1, a.GetSlice(2).Compare(b.GetSlice(), n - 1));
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
			Assert.AreEqual(n, a.GetSlice(2).Compare(index, b.GetSlice(), otherIndex));
			Assert.AreEqual(n, a.GetSlice(2).Compare(index, b.GetSlice(), otherIndex, length));
		}
	}

	[TestMethod]
	public void TestContains()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);

		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestContains();
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		void ProcessA(Slice<string> a)
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestEndsWith();
	}

	[TestMethod]
	public void TestEquals()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);

		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestEquals();
	}

	[TestMethod]
	public void TestFind()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
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
			d = c.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);

		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestFindAll();
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
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
			d = c.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
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
			d = c.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
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
			d = c.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(d, b);
		}
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
			c = new(list);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
			c = [];
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseIndexableTests<string, Slice<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		void ProcessA(Slice<string> a)
		{
			var b = a.IndexOfAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(0, b);
			b = a.IndexOfAny(new List<string>("LLL", "NNN", "PPP"));
			Assert.AreEqual(2, b);
			b = a.IndexOfAny(["LLL", "NNN", "PPP"], 4);
			Assert.AreEqual(-1, b);
			b = a.IndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(-1, b);
			Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		void ProcessA(Slice<string> a)
		{
			var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(1, b);
			b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(0, b);
			b = a.IndexOfAnyExcluding(a);
			Assert.AreEqual(-1, b);
			Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestLastIndexOfAny();
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		void ProcessA(Slice<string> a)
		{
			var b = a.LastIndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(5, b);
			b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(6, b);
			b = a.LastIndexOfAnyExcluding(a);
			Assert.AreEqual(-1, b);
			Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
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
			d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => x.All(y => y is >= 'A' and <= 'Z')));
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		static void ProcessA(Slice<string> a) => new BaseStringIndexableTests<Slice<string>>(a, list, defaultString, defaultCollection).TestStartsWith();
	}

	[TestMethod]
	public void TestTake()
	{
		var a = list.ToList().Insert(0, "XXX").GetSlice(1);
		ProcessA(a);
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(0, "XXX")), 1);
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
		a = new(RedStarLinq.ToArray(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
		ProcessA(a);
		a = new((G.IList<string>)E.ToList(list.ToList().Insert(3, new List<string>("$", "###")).Insert(0, "XXX")), 1);
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
			d = E.ToList(E.TakeWhile(E.Take(c, 10), x => x.All(y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
	}

	[TestMethod]
	public void TestToArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
public class QueueTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(random.Next(17), _ => random.Next(16));
		Queue<int> q = new(arr);
		G.Queue<int> gq = new(arr);
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			q.Enqueue(n);
			gq.Enqueue(n);
			Assert.IsTrue(RedStarLinq.Equals(q, gq));
			Assert.IsTrue(E.SequenceEqual(gq, q));
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (q.Length == 0)
					Assert.ThrowsExactly<InvalidOperationException>(() => q.Dequeue());
				else
					Assert.AreEqual(q.Dequeue(), gq.Dequeue());
				Assert.IsTrue(RedStarLinq.Equals(q, gq));
				Assert.IsTrue(E.SequenceEqual(gq, q));
			}
			else
			{
				if (q.TryDequeue(out var value))
					Assert.AreEqual(value, gq.Dequeue());
				else
				{
					Assert.AreEqual(0, q.Length);
					Assert.AreEqual(0, gq.Count);
				}
				Assert.IsTrue(RedStarLinq.Equals(q, gq));
				Assert.IsTrue(E.SequenceEqual(gq, q));
			}
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (q.Length == 0)
					Assert.ThrowsExactly<InvalidOperationException>(() => q.Peek());
				else
					Assert.AreEqual(q.Peek(), gq.Peek());
				Assert.IsTrue(RedStarLinq.Equals(q, gq));
				Assert.IsTrue(E.SequenceEqual(gq, q));
			}
			else
			{
				if (q.TryPeek(out var value))
					Assert.AreEqual(value, gq.Peek());
				else
				{
					Assert.AreEqual(0, q.Length);
					Assert.AreEqual(0, gq.Count);
				}
				Assert.IsTrue(RedStarLinq.Equals(q, gq));
				Assert.IsTrue(E.SequenceEqual(gq, q));
			}
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 10)
			goto l1;
	}
}

[TestClass]
public class StackTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
	l1:
		var arr = RedStarLinq.FillArray(random.Next(17), _ => random.Next(16));
		Stack<int> st = new(arr);
		G.Stack<int> gst = new(arr);
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			st.Push(n);
			gst.Push(n);
			Assert.IsTrue(RedStarLinq.Equals(st, E.Reverse(gst)));
			Assert.IsTrue(E.SequenceEqual(E.Reverse(gst), st));
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (st.Length == 0)
					Assert.ThrowsExactly<InvalidOperationException>(() => st.Pop());
				else
					Assert.AreEqual(st.Pop(), gst.Pop());
				Assert.IsTrue(RedStarLinq.Equals(st, E.Reverse(gst)));
				Assert.IsTrue(E.SequenceEqual(E.Reverse(gst), st));
			}
			else
			{
				if (st.TryPop(out var value))
					Assert.AreEqual(value, gst.Pop());
				else
				{
					Assert.AreEqual(0, st.Length);
					Assert.AreEqual(0, gst.Count);
				}
				Assert.IsTrue(RedStarLinq.Equals(st, E.Reverse(gst)));
				Assert.IsTrue(E.SequenceEqual(E.Reverse(gst), st));
			}
		}, () =>
		{
			if (random.Next(2) == 0)
			{
				if (st.Length == 0)
					Assert.ThrowsExactly<InvalidOperationException>(() => st.Peek());
				else
					Assert.AreEqual(st.Peek(), gst.Peek());
				Assert.IsTrue(RedStarLinq.Equals(st, E.Reverse(gst)));
				Assert.IsTrue(E.SequenceEqual(E.Reverse(gst), st));
			}
			else
			{
				if (st.TryPeek(out var value))
					Assert.AreEqual(value, gst.Peek());
				else
				{
					Assert.AreEqual(0, st.Length);
					Assert.AreEqual(0, gst.Count);
				}
				Assert.IsTrue(RedStarLinq.Equals(st, E.Reverse(gst)));
				Assert.IsTrue(E.SequenceEqual(E.Reverse(gst), st));
			}
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 10)
			goto l1;
	}
}
