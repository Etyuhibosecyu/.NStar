using System.Numerics;

namespace Corlib.NStar.Tests;

[TestClass]
public class ListTests
{
	[TestMethod]
	public void TestAdd()
	{
		var a = list.ToList().Add(defaultString);
		var b = new G.List<string>(list) { defaultString };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new List<string>(10, list).Add(defaultString);
		b = [.. list, defaultString];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAddSeries()
	{
		var a = list.ToList();
		a.AddSeries("XXX", 0);
		G.List<string> b = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries("XXX", 3);
		b.AddRange(["XXX", "XXX", "XXX"]);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries("XXX", 101);
		b.AddRange(E.Repeat("XXX", 101));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries("XXX", -1));
		a.Replace(list);
		a.AddSeries(index => (index ^ index >> 1).ToString("D3"), 0);
		b.Clear();
		b.AddRange(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(index => (index ^ index >> 1).ToString("D3"), 3);
		b.AddRange(["000", "001", "003"]);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(index => (index ^ index >> 1).ToString("D3"), 101);
		b.AddRange(E.Select(E.Range(0, 101), index => (index ^ index >> 1).ToString("D3")));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(index => (index ^ index >> 1).ToString("D3"), -1));
		a.Replace(list);
		a.AddSeries(0, index => (index ^ index >> 1).ToString("D3"));
		b.Clear();
		b.AddRange(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(3, index => (index ^ index >> 1).ToString("D3"));
		b.AddRange(["000", "001", "003"]);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.AddSeries(101, index => (index ^ index >> 1).ToString("D3"));
		b.AddRange(E.Select(E.Range(0, 101), index => (index ^ index >> 1).ToString("D3")));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.AddSeries(-1, index => (index ^ index >> 1).ToString("D3")));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = list.ToList();
		var b = a.Append(defaultString);
		var c = E.Append(list, defaultString);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestBinarySearch()
	{
		var a = list.ToList().Sort();
		var b = a.BinarySearch("MMM");
		var c = new G.List<string>(list);
		c.Sort();
		var d = c.BinarySearch("MMM");
		Assert.AreEqual(d, b);
		b = a.BinarySearch("NNN");
		d = c.BinarySearch("NNN");
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.BreakFilter(x => x.Length == 3, out var c);
		var d = new G.List<string>(list);
		d.InsertRange(3, ["$", "###"]);
		var e = E.ToList(E.Where(d, x => x.Length == 3));
		var f = E.ToList(E.Where(d, x => x.Length != 3));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
		b = a.BreakFilter(x => x.All(y => y is >= 'A' and <= 'Z'), out c);
		e = E.ToList(E.Where(d, x => x.All(y => y is >= 'A' and <= 'Z')));
		f = E.ToList(E.Where(d, x => !x.All(y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
	}

	[TestMethod]
	public void TestBreakFilterInPlace()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.BreakFilterInPlace(x => x.Length == 3, out var c);
		var d = new G.List<string>(list);
		d.InsertRange(3, ["$", "###"]);
		var e = E.ToList(E.Where(d, x => x.Length != 3));
		d = E.ToList(E.Where(d, x => x.Length == 3));
		BaseListTests<string, List<string>>.BreakFilterInPlaceAsserts(a, b, c, d, e);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.BreakFilterInPlace(x => x.All(y => y is >= 'A' and <= 'Z'), out c);
		d = [.. list];
		d.InsertRange(3, ["$", "###"]);
		e = E.ToList(E.Where(d, x => !x.All(y => y is >= 'A' and <= 'Z')));
		d = E.ToList(E.Where(d, x => x.All(y => y is >= 'A' and <= 'Z')));
		BaseListTests<string, List<string>>.BreakFilterInPlaceAsserts(a, b, c, d, e);
	}

	[TestMethod]
	public void TestClear()
	{
		var a = list.ToList();
		a.Clear(2, 4);
		var b = new G.List<string>(list);
		for (var i = 0; i < 4; i++)
			b[2 + i] = default!;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestCompare()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var a = new List<string>(E.Select(E.Range(0, random.Next(3, 100)), _ => random.Next(1000).ToString("D3")));
			var b = new List<string>(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			Assert.AreEqual(n, a.Compare(b));
			a = [.. E.Select(E.Range(0, random.Next(5, 100)), _ => random.Next(1000).ToString("D3"))];
			b = [.. a];
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			Assert.AreEqual(n - 1, a.Compare(b, n - 1));
			a = [.. E.Select(E.Range(0, random.Next(5, 100)), _ => random.Next(1000).ToString("D3"))];
			b = [.. a];
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => random.Next(1000).ToString("D3")));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => random.Next(1000).ToString("D3")));
			Assert.AreEqual(n, a.Compare(index, b, otherIndex));
			Assert.AreEqual(n, a.Compare(index, b, otherIndex, length));
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = list.ToList();
		var b = a.Concat([.. defaultCollection]);
		var c = E.Concat(list, defaultCollection);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = list.ToList();
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestContains();
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = list.ToList();
		var b = a.ContainsAny(new List<string>("PPP", "DDD", "MMM"));
		Assert.IsTrue(b);
		b = a.ContainsAny(new List<string>("LLL", "MMM", "NNN"));
		Assert.IsTrue(b);
		b = a.ContainsAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = list.ToList();
		var b = a.ContainsAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsFalse(b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = list.ToList();
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(list, (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(list, x => x + "A");
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(d.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = list.ToList();
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

	[TestMethod]
	public void TestEndsWith()
	{
		var a = list.ToList();
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestEndsWith();
	}

	[TestMethod]
	public void TestEquals()
	{
		var a = list.ToList();
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestEquals();
	}

	[TestMethod]
	public void TestFillInPlace()
	{
		var a = list.ToList();
		a.FillInPlace("XXX", 0);
		G.List<string> b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace("XXX", 3);
		b = ["XXX", "XXX", "XXX"];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace("XXX", 101);
		b = [.. E.Repeat("XXX", 101)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace("XXX", -1));
		a.FillInPlace(index => (index ^ index >> 1).ToString("D3"), 0);
		b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(index => (index ^ index >> 1).ToString("D3"), 3);
		b = ["000", "001", "003"];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(index => (index ^ index >> 1).ToString("D3"), 101);
		b = [.. E.Select(E.Range(0, 101), index => (index ^ index >> 1).ToString("D3"))];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(index => (index ^ index >> 1).ToString("D3"), -1));
		a.FillInPlace(0, index => (index ^ index >> 1).ToString("D3"));
		b = [];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(3, index => (index ^ index >> 1).ToString("D3"));
		b = ["000", "001", "003"];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a.FillInPlace(101, index => (index ^ index >> 1).ToString("D3"));
		b = [.. E.Select(E.Range(0, 101), index => (index ^ index >> 1).ToString("D3"))];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.FillInPlace(-1, index => (index ^ index >> 1).ToString("D3")));
	}

	[TestMethod]
	public void TestFilter()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.Filter(x => x.Length == 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = E.Where(c, x => x.Length == 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		b = a.Filter((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		d = E.Where(c, (x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.FilterInPlace(x => x.Length == 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		c = E.ToList(E.Where(c, x => x.Length == 3));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FilterInPlace((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		c = E.ToList(E.Where(c, (x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestFind()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.Find(x => x.Length != 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = c.Find(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = c.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestFindAll();
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.FindIndex(x => x.Length != 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = c.FindIndex(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = c.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.FindLast(x => x.Length != 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = c.FindLast(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = c.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.FindLastIndex(x => x.Length != 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = c.FindLastIndex(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = c.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = list.ToList();
		var b = a.GetAfter("DDD");
		var c = new G.List<string>() { "MMM", "EEE", "DDD" };
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter([]);
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new("DDD", "MMM"));
		c = ["EEE", "DDD"];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		var a = list.ToList();
		var b = a.GetAfterLast("MMM");
		var c = new G.List<string>() { "EEE", "DDD" };
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast([]);
		c = [];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast(new("DDD", "MMM"));
		c = ["EEE", "DDD"];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = list.ToList();
		var b = a.GetBefore("DDD");
		var c = new G.List<string>() { "MMM", "BBB", "PPP" };
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore([]);
		c = [];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new("DDD", "MMM"));
		c = ["MMM", "BBB", "PPP"];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		var a = list.ToList();
		var b = a.GetBeforeLast("MMM");
		var c = new G.List<string>() { "MMM", "BBB", "PPP", "DDD" };
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast([]);
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast(new("DDD", "MMM"));
		c = ["MMM", "BBB", "PPP"];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		var a = list.ToList();
		var b = a.GetBeforeSetAfter("DDD");
		var c = new G.List<string>() { "MMM", "BBB", "PPP" };
		var d = new G.List<string>() { "MMM", "EEE", "DDD" };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		var a = list.ToList();
		var b = a.GetBeforeSetAfterLast("MMM");
		var c = new G.List<string>() { "MMM", "BBB", "PPP", "DDD" };
		var d = new G.List<string>() { "EEE", "DDD" };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetRange()
	{
		var a = list.ToList();
		var b = a.GetRange(.., true);
		b.Add(defaultString);
		var c = new G.List<string>(list).GetRange(0, list.Length);
		c.Add(defaultString);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		new BaseIndexableTests<string, List<string>>(a, list, defaultString, defaultCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = list.ToList();
		new BaseIndexableTests<string, List<string>>(a, list, defaultString, defaultCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = list.ToList();
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestIndexOf();
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = list.ToList();
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

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = list.ToList();
		var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(1, b);
		b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(0, b);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = list.ToList();
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestLastIndexOf();
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = list.ToList();
		var b = a.LastIndexOfAny(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(6, b);
		b = a.LastIndexOfAny(new List<string>("LLL", "NNN", "PPP"));
		Assert.AreEqual(2, b);
		b = a.LastIndexOfAny(["LLL", "NNN", "EEE"], 4);
		Assert.AreEqual(-1, b);
		b = a.LastIndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = list.ToList();
		var b = a.LastIndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(5, b);
		b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(6, b);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(-1, b);
		Assert.ThrowsExactly<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestNSort()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var c = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = new List<string>(c);
		var b = new List<string>(a).NSort(x => x[^1]);
		c = E.ToList(E.OrderBy(c, x => x[^1]));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestPad()
	{
		var a = list.ToList();
		var b = a.Pad(10);
		var c = new G.List<string>(list);
		c.Insert(0, default!);
		c.Add(default!);
		c.Add(default!);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(10, "XXX");
		c = [.. list];
		c.Insert(0, "XXX");
		c.Add("XXX");
		c.Add("XXX");
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5);
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5, "XXX");
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.Pad(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.Pad(-1, "XXX"));
	}

	[TestMethod]
	public void TestPadInPlace()
	{
		var a = list.ToList();
		var b = a.PadInPlace(10);
		var c = new G.List<string>(list);
		c.Insert(0, default!);
		c.Add(default!);
		c.Add(default!);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadInPlace(10, "XXX");
		c = [.. list];
		c.Insert(0, "XXX");
		c.Add("XXX");
		c.Add("XXX");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadInPlace(5);
		c = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadInPlace(5, "XXX");
		c = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadInPlace(-1, "XXX"));
	}

	[TestMethod]
	public void TestPadLeft()
	{
		var a = list.ToList();
		var b = a.PadLeft(10);
		var c = new G.List<string>(list);
		c.Insert(0, default!);
		c.Insert(0, default!);
		c.Insert(0, default!);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(10, "XXX");
		c = [.. list];
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5);
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5, "XXX");
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeft(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeft(-1, "XXX"));
	}

	[TestMethod]
	public void TestPadLeftInPlace()
	{
		var a = list.ToList();
		var b = a.PadLeftInPlace(10);
		var c = new G.List<string>(list);
		c.Insert(0, default!);
		c.Insert(0, default!);
		c.Insert(0, default!);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadLeftInPlace(10, "XXX");
		c = [.. list];
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadLeftInPlace(5);
		c = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadLeftInPlace(5, "XXX");
		c = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1, "XXX"));
	}

	[TestMethod]
	public void TestPadRight()
	{
		var a = list.ToList();
		var b = a.PadRight(10);
		var c = new G.List<string>(list) { default!, default!, default! };
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(10, "XXX");
		c = [.. list, "XXX", "XXX", "XXX"];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5);
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5, "XXX");
		c = [.. list];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRight(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRight(-1, "XXX"));
	}

	[TestMethod]
	public void TestPadRightInPlace()
	{
		var a = list.ToList();
		var b = a.PadRightInPlace(10);
		var c = new G.List<string>(list) { default!, default!, default! };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadRightInPlace(10, "XXX");
		c = [.. list, "XXX", "XXX", "XXX"];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadRightInPlace(5);
		c = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.PadRightInPlace(5, "XXX");
		c = [.. list];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1, "XXX"));
	}

	[TestMethod]
	public void TestRemove()
	{
		var a = list.ToList();
		var b = new List<string>(a).RemoveEnd(6);
		var c = new G.List<string>(list);
		c.RemoveRange(6, 1);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(0, 1);
		c = [.. list];
		c.RemoveRange(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1, list.Length - 2);
		c = [.. list];
		c.RemoveRange(1, list.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1, 4);
		c = [.. list];
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(list.Length - 5, 4);
		c = [.. list];
		c.RemoveRange(list.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(list.Length - 5, 10 - list.Length);
		c = [.. list];
		c.RemoveRange(list.Length - 5, 10 - list.Length);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(-1, 6));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(list.Length - 1, 2 - list.Length));
		Assert.ThrowsExactly<ArgumentException>(() => b = new List<string>(a).Remove(1, 1000));
		b = new List<string>(a).Remove(..);
		c = [];
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(..^1);
		c = [.. list];
		c.RemoveRange(0, list.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..);
		c = [.. list];
		c.RemoveRange(1, list.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..^1);
		c = [.. list];
		c.RemoveRange(1, list.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..5);
		c = [.. list];
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(^5..^1);
		c = [.. list];
		c.RemoveRange(list.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(^5..5);
		c = [.. list];
		c.RemoveRange(list.Length - 5, 10 - list.Length);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(-1..5));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(^1..1));
		Assert.ThrowsExactly<ArgumentException>(() => b = new List<string>(a).Remove(1..1000));
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.RemoveAll(x => x.Length != 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = c.RemoveAll(x => x.Length != 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.RemoveAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = c.RemoveAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = list.ToList();
		for (var i = 0; i < 1000; i++)
		{
			var index = random.Next(a.Length);
			var b = new List<string>(a).RemoveAt(index);
			var c = new G.List<string>(a);
			c.RemoveAt(index);
			Assert.IsTrue(a[..index].Equals(b[..index]));
			Assert.IsTrue(E.SequenceEqual(b[..index], a[..index]));
			Assert.IsTrue(a[(index + 1)..].Equals(b[index..]));
			Assert.IsTrue(E.SequenceEqual(b[index..], a[(index + 1)..]));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestRemoveValue()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = new Chain(15, 10).ToList(x => x.ToString());
		for (var i = 0; i < 1000; i++)
		{
			var value = a.Random(random);
			var b = new List<string>(a);
			b.RemoveValue(value);
			var c = new G.List<string>(a);
			c.Remove(value);
			foreach (var x in a)
				Assert.AreEqual(b.Contains(x), x != value);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestRepeat()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 1000; i++)
		{
			var arr = RedStarLinq.FillArray(random.Next(1, 1001), _ => random.Next());
			var a = arr.ToList();
			var b = E.ToList(arr);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var n = random.Next(11);
			var c = a.Repeat(n);
			var d = new G.List<int>();
			for (var j = 0; j < n; j++)
				d.AddRange(b);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		var a = list.ToList().Replace(defaultCollection);
		var b = new G.List<string>(list);
		b.Clear();
		b.AddRange(defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.AsSpan(2, 3));
		b = [.. list, .. E.Take(E.Skip(defaultCollection, 2), 3)];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentNullException>(() => a.Replace((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		var a = list.ToList().ReplaceRange(2, 3, defaultCollection);
		var b = new G.List<string>(list);
		b.RemoveRange(2, 3);
		b.InsertRange(2, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(2, defaultCollection.AsSpan(2, 3));
		b = [.. list];
		b.InsertRange(2, E.Take(E.Skip(defaultCollection, 2), 3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentException>(() => a = list.ToList().ReplaceRange(1, 1000, defaultString));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToList().ReplaceRange(-1, 3, defaultCollection));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToList().ReplaceRange(4, -2, defaultCollection));
		Assert.ThrowsExactly<ArgumentNullException>(() => list.ToList().ReplaceRange(4, 1, null!));
	}

	[TestMethod]
	public void TestReverse()
	{
		var a = list.ToList().Reverse();
		var b = new G.List<string>(list);
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.AsSpan(2, 3)).Reverse();
		b = [.. list, .. E.Take(E.Skip(defaultCollection, 2), 3)];
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.AsSpan(2, 3)).Reverse(2, 4);
		b = [.. list, .. E.Take(E.Skip(defaultCollection, 2), 3)];
		b.Reverse(2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetAll()
	{
		var a = list.ToList().SetAll(defaultString);
		var b = new G.List<string>(list);
		for (var i = 0; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, 3);
		b = [.. list];
		for (var i = 3; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, 2, 4);
		b = [.. list];
		for (var i = 2; i < 6; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, ^5);
		b = [.. list];
		for (var i = b.Count - 5; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, ^6..4);
		b = [.. list];
		for (var i = b.Count - 6; i < 4; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var a = list.ToList();
		var b = new G.List<string>(list);
		for (var i = 0; i < 1000; i++)
		{
			var index = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var n = random.Next(1000).ToString("D3");
			a.SetOrAdd(index, n);
			if (index < b.Count)
				b[index] = n;
			else
				b.Add(n);
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		var hs = defaultCollection.ToHashSet();
		var a = list.ToList().SetRange(2, hs);
		var b = new G.List<string>(list);
		for (var i = 0; i < hs.Length; i++)
			b[i + 2] = hs[i];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsExactly<ArgumentException>(() => a = list.ToList().SetRange(5, hs));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToList().SetRange(-1, hs));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToList().SetRange(1000, hs));
		Assert.ThrowsExactly<ArgumentNullException>(() => list.ToList().SetRange(4, null!));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = list.ToList();
		var b = a.Skip(2);
		var c = E.Skip(list, 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Skip(0);
		c = E.Skip(list, 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Skip(1000);
		c = E.Skip(list, 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Skip(-4);
		c = E.Skip(list, -4);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = list.ToList();
		var b = a.SkipLast(2);
		var c = E.SkipLast(list, 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.SkipLast(0);
		c = E.SkipLast(list, 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.SkipLast(1000);
		c = E.SkipLast(list, 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.SkipLast(-4);
		c = E.SkipLast(list, -4);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.SkipWhile(x => x.Length == 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = E.ToList(E.SkipWhile(c, x => x.Length == 3));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.SkipWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') || index < 1);
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => x.All(y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestSort()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var toSort = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = new List<string>(toSort);
		var b = new List<string>(a).Sort();
		var c = new G.List<string>(a);
		c.Sort();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Sort(new Comparer<string>((x, y) => y.CompareTo(x)));
		c = [.. a];
		c.Sort(new Comparer<string>((x, y) => y.CompareTo(x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Sort(2, 4, new Comparer<string>((x, y) => y.CompareTo(x)));
		c = [.. a];
		c.Sort(2, 4, new Comparer<string>((x, y) => y.CompareTo(x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = list.ToList();
		new BaseStringIndexableTests<List<string>>(a, list, defaultString, defaultCollection).TestStartsWith();
	}

	[TestMethod]
	public void TestTake()
	{
		var a = list.ToList();
		var b = a.Take(2);
		var c = E.Take(list, 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Take(0);
		c = E.Take(list, 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Take(1000);
		c = E.Take(list, 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Take(-4);
		c = E.Take(list, -4);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = list.ToList();
		var b = a.TakeLast(2);
		var c = E.TakeLast(list, 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.TakeLast(0);
		c = E.TakeLast(list, 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.TakeLast(1000);
		c = E.TakeLast(list, 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.TakeLast(-4);
		c = E.TakeLast(list, -4);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.TakeWhile(x => x.Length == 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = E.ToList(E.TakeWhile(c, x => x.Length == 3));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.TakeWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index < 10);
		c = [.. list];
		c.InsertRange(3, ["$", "###"]);
		d = E.ToList(E.TakeWhile(E.Take(c, 10), x => x.All(y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseListTests<string, List<string>>.TestToArray(() => new((char)random.Next(33, 127), random.Next(10)));
	}

	[TestMethod]
	public void TestTrimExcess()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseListTests<string, List<string>>.TestTrimExcess(() => new((char)random.Next(33, 127), random.Next(10)));
	}

	[TestMethod]
	public void TestTrueForAll()
	{
		var a = list.ToList().Insert(3, new List<string>("$", "###"));
		var b = a.TrueForAll(x => x.Length == 3);
		var c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		var d = c.TrueForAll(x => x.Length == 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		b = a.TrueForAll(x => x.Length <= 3);
		d = c.TrueForAll(x => x.Length <= 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
		b = a.TrueForAll(x => x.Length > 3);
		d = c.TrueForAll(x => x.Length > 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(d, b);
	}

	[TestMethod]
	public void TestTuples()
	{
		var a = (List<string>)("AAA", "BBB");
		var b = new List<string>("AAA", "BBB");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC");
		b = new List<string>("AAA", "BBB", "CCC");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD");
		b = new List<string>("AAA", "BBB", "CCC", "DDD");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (List<string>)("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO", "PPP");
		b = new List<string>("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO", "PPP");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		var fullList = b.Copy();
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string))fullList[..1]);
		Assert.AreEqual(((string, string))fullList[..2], ("AAA", "BBB"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string))fullList[..3]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string))fullList[..2]);
		Assert.AreEqual(((string, string, string))fullList[..3], ("AAA", "BBB", "CCC"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string))fullList[..4]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string))fullList[..3]);
		Assert.AreEqual(((string, string, string, string))fullList[..4], ("AAA", "BBB", "CCC", "DDD"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string))fullList[..5]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string))fullList[..4]);
		Assert.AreEqual(((string, string, string, string, string))fullList[..5], ("AAA", "BBB", "CCC", "DDD", "EEE"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string))fullList[..6]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string))fullList[..5]);
		Assert.AreEqual(((string, string, string, string, string, string))fullList[..6], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string))fullList[..7]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string))fullList[..6]);
		Assert.AreEqual(((string, string, string, string, string, string, string))fullList[..7], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string))fullList[..8]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string))fullList[..7]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string))fullList[..8], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string))fullList[..9]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string))fullList[..8]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string))fullList[..9], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string))fullList[..10]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string))fullList[..9]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string))fullList[..10], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string))fullList[..11]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string))fullList[..10]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string))fullList[..11], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string))fullList[..12]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string))fullList[..11]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string))fullList[..12], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string))fullList[..13]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..12]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..13], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..14]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..13]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..14], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..15]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..14]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..15], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO"));
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..16]);
		Assert.ThrowsExactly<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..15]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..16], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO", "PPP"));
		Assert.ThrowsExactly<ArgumentException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..17]);
	}
}
