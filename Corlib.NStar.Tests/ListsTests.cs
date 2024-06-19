using System.Numerics;
using System.Threading;

namespace Corlib.NStar.Tests;

[TestClass]
public class BufferTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var arr = RedStarLinq.FillArray(100, _ => random.Next(16));
		var toInsert = Array.Empty<int>();
		Buffer<int> buf = new(16, arr);
		G.List<int> gl = new(arr[^16..]);
		var collectionActions = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			buf.AddRange(toInsert);
			gl.AddRange(toInsert);
			gl.RemoveRange(0, Max(gl.Count - 16, 0));
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			var n = random.Next(buf.Length);
			toInsert = RedStarLinq.FillArray(random.Next(6), _ => random.Next(16));
			buf.Insert(n, toInsert);
			gl.InsertRange(n, toInsert);
			gl.RemoveRange(0, Max(gl.Count - 16, 0));
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			var length = Min(random.Next(9), buf.Length);
			if (buf.Length < length)
				return;
			var start = random.Next(buf.Length - length + 1);
			buf.Remove(start, length);
			gl.RemoveRange(start, length);
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		} };
		var actions = new[] { () =>
		{
			var n = random.Next(16);
			if (random.Next(2) == 0)
			{
				var index = random.Next(buf.Length);
				buf.Insert(index, n);
				gl.Insert(index, n);
			}
			else
			{
				buf.Add(n);
				gl.Add(n);
			}
			if (gl.Count > 16)
				gl.RemoveAt(0);
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			if (buf.Length == 0) return;
			if (random.Next(2) == 0)
			{
				var n = random.Next(buf.Length);
				buf.RemoveAt(n);
				gl.RemoveAt(n);
			}
			else
			{
				var n = random.Next(16);
				buf.RemoveValue(n);
				gl.Remove(n);
			}
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			collectionActions.Random(random)();
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			if (buf.Length == 0)
				return;
			var n = random.Next(buf.Length);
			var n2 = random.Next(16);
			buf[n] = n2;
			gl[n] = n2;
			Assert.IsTrue(buf.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, buf));
		}, () =>
		{
			if (buf.Length == 0) return;
			var n = random.Next(buf.Length);
			Assert.AreEqual(buf[buf.IndexOf(buf[n])], buf[n]);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		arr = RedStarLinq.FillArray(14, _ => random.Next(16));
		var n = 1;
		toInsert = RedStarLinq.FillArray(5, _ => random.Next(16));
		buf = new(16, arr);
		gl = new(arr);
		buf.Insert(n, toInsert);
		gl.InsertRange(n, toInsert);
		gl.RemoveRange(0, Max(gl.Count - 16, 0));
		Assert.IsTrue(buf.Equals(gl));
		Assert.IsTrue(E.SequenceEqual(gl, buf));
	}
}

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
	}

	[TestMethod]
	public void TestAddRange()
	{
		var a = list.ToList().AddRange(defaultCollection);
		var b = new G.List<string>(list);
		b.AddRange(defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.AsSpan(2, 3));
		b = new G.List<string>(list);
		b.AddRange(defaultCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = list.ToList();
		var b = a.Append(defaultString);
		var c = E.Append(new G.List<string>(list), defaultString);
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
		Assert.AreEqual(b, d);
		b = a.BinarySearch("NNN");
		d = c.BinarySearch("NNN");
		Assert.AreEqual(b, d);
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
		e = E.ToList(E.Where(d, x => E.All(x, y => y is >= 'A' and <= 'Z')));
		f = E.ToList(E.Where(d, x => !E.All(x, y => y is >= 'A' and <= 'Z')));
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
		d = new G.List<string>(list);
		d.InsertRange(3, ["$", "###"]);
		e = E.ToList(E.Where(d, x => !E.All(x, y => y is >= 'A' and <= 'Z')));
		d = E.ToList(E.Where(d, x => E.All(x, y => y is >= 'A' and <= 'Z')));
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
		for (var i = 0; i < 1000; i++)
		{
			var a = new List<string>(E.Select(E.Range(0, random.Next(3, 100)), _ => random.Next(1000).ToString("D3")));
			var b = new List<string>(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b) == n);
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => random.Next(1000).ToString("D3")));
			b = new(a);
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b, n - 1) == n - 1);
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => random.Next(1000).ToString("D3")));
			b = new(a);
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(1000).ToString("D3");
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => random.Next(1000).ToString("D3")));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => random.Next(1000).ToString("D3")));
			Assert.IsTrue(a.Compare(index, b, otherIndex) == n);
			Assert.IsTrue(a.Compare(index, b, otherIndex, length) == n);
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = list.ToList();
		var b = a.Concat(new(defaultCollection));
		var c = E.Concat(new G.List<string>(list), defaultCollection);
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
		Assert.IsTrue(!b);
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
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = list.ToList();
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(new G.List<string>(list), (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(new G.List<string>(list), x => x + "A");
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
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		c = E.ToList(E.Where(c, (x, index) => E.All(x, y => y is >= 'A' and <= 'Z') && index >= 1));
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
		Assert.AreEqual(b, d);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = c.Find(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
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
		Assert.AreEqual(b, d);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = c.FindIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
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
		Assert.AreEqual(b, d);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = c.FindLast(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
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
		Assert.AreEqual(b, d);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = c.FindLastIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
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
		c = [];
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
		c = new(list);
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
		c = new(list);
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
		Assert.AreEqual(b, 0);
		b = a.IndexOfAny(new List<string>("LLL", "NNN", "PPP"));
		Assert.AreEqual(b, 2);
		b = a.IndexOfAny(["LLL", "NNN", "PPP"], 4);
		Assert.AreEqual(b, -1);
		b = a.IndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = list.ToList();
		var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 1);
		b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, 0);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestInsert()
	{
		var a = list.ToList().Insert(3, defaultString);
		var b = new G.List<string>(list);
		b.Insert(3, defaultString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(4, defaultCollection);
		b = new G.List<string>(list);
		b.InsertRange(4, defaultCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().Insert(2, defaultCollection.AsSpan(2, 3));
		b = new G.List<string>(list);
		b.InsertRange(2, defaultCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a = list.ToList().Insert(1000, defaultString));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.ToList().Insert(-1, defaultCollection));
		Assert.ThrowsException<ArgumentNullException>(() => list.ToList().Insert(1, null));
		Assert.ThrowsException<ArgumentNullException>(() => list.ToList().Insert(5, (G.IEnumerable<string>)null!));
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
		Assert.AreEqual(b, 6);
		b = a.LastIndexOfAny(new List<string>("LLL", "NNN", "PPP"));
		Assert.AreEqual(b, 2);
		b = a.LastIndexOfAny(["LLL", "NNN", "EEE"], 4);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = list.ToList();
		var b = a.LastIndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 5);
		b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
		Assert.AreEqual(b, 6);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestNSort()
	{
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
		c = new G.List<string>(list);
		c.Insert(0, "XXX");
		c.Add("XXX");
		c.Add("XXX");
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5);
		c = new G.List<string>(list);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5, "XXX");
		c = new G.List<string>(list);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pad(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pad(-1, "XXX"));
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
		a = new(list);
		b = a.PadInPlace(10, "XXX");
		c = new G.List<string>(list);
		c.Insert(0, "XXX");
		c.Add("XXX");
		c.Add("XXX");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(list);
		b = a.PadInPlace(5);
		c = new G.List<string>(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(list);
		b = a.PadInPlace(5, "XXX");
		c = new G.List<string>(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadInPlace(-1, "XXX"));
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
		c = new G.List<string>(list);
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5);
		c = new G.List<string>(list);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5, "XXX");
		c = new G.List<string>(list);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeft(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeft(-1, "XXX"));
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
		a = new(list);
		b = a.PadLeftInPlace(10, "XXX");
		c = new G.List<string>(list);
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		c.Insert(0, "XXX");
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(list);
		b = a.PadLeftInPlace(5);
		c = new G.List<string>(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(list);
		b = a.PadLeftInPlace(5, "XXX");
		c = new G.List<string>(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1, "XXX"));
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
		c = new G.List<string>(list) { "XXX", "XXX", "XXX" };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5);
		c = new G.List<string>(list);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5, "XXX");
		c = new G.List<string>(list);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRight(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRight(-1, "XXX"));
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
		a = new(list);
		b = a.PadRightInPlace(10, "XXX");
		c = new G.List<string>(list) { "XXX", "XXX", "XXX" };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(list);
		b = a.PadRightInPlace(5);
		c = new G.List<string>(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(list);
		b = a.PadRightInPlace(5, "XXX");
		c = new G.List<string>(list);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1, "XXX"));
	}

	[TestMethod]
	public void TestRemove()
	{
		var a = list.ToList();
		var b = new List<string>(a).Remove(6);
		var c = new G.List<string>(list);
		c.RemoveRange(6, 1);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(0, 1);
		c = new G.List<string>(list);
		c.RemoveRange(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1, list.Length - 2);
		c = new G.List<string>(list);
		c.RemoveRange(1, list.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1, 4);
		c = new G.List<string>(list);
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(list.Length - 5, 4);
		c = new G.List<string>(list);
		c.RemoveRange(list.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(list.Length - 5, 10 - list.Length);
		c = new G.List<string>(list);
		c.RemoveRange(list.Length - 5, 10 - list.Length);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(-1, 6));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(list.Length - 1, 2 - list.Length));
		Assert.ThrowsException<ArgumentException>(() => b = new List<string>(a).Remove(1, 1000));
		b = new List<string>(a).Remove(..);
		c = [];
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(..^1);
		c = new G.List<string>(list);
		c.RemoveRange(0, list.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..);
		c = new G.List<string>(list);
		c.RemoveRange(1, list.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..^1);
		c = new G.List<string>(list);
		c.RemoveRange(1, list.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(1..5);
		c = new G.List<string>(list);
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(^5..^1);
		c = new G.List<string>(list);
		c.RemoveRange(list.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Remove(^5..5);
		c = new G.List<string>(list);
		c.RemoveRange(list.Length - 5, 10 - list.Length);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new List<string>(a).Remove(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = new List<string>(a).Remove(1..1000));
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
		Assert.AreEqual(b, d);
		a = list.ToList().Insert(3, new List<string>("$", "###"));
		b = a.RemoveAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = c.RemoveAll(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
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
		b = new G.List<string>(list);
		b.AddRange(defaultCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentNullException>(() => a.Replace((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestReplace2()
	{
		for (var i = 0; i < 1000; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldItem = (char)random.Next(33, 127);
			var newItem = (char)random.Next(33, 127);
			var b = a.Replace(oldItem, newItem);
			var c = s.Replace(oldItem, newItem);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
			var newArray = new char[random.Next(10)];
			for (var j = 0; j < newArray.Length; j++)
				newArray[j] = (char)random.Next(33, 127);
			string newCollection = new(newArray);
			var b = a.Replace(oldCollection, newArray);
			var c = s.Replace(oldCollection, newCollection);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace(null!, null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace("925", null!));
		var backConverter = new Chain(48, 10).ToArray(x => ((char)(x - 48 + 'A'), (char)x));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (char)random.Next(48, 58));
			var b = a.Replace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, (char)(x.Value - 48 + 'A')));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace(replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, char>)null!));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (G.IEnumerable<char>)new Chain(random.Next(0, 5)).ToString(y => (char)random.Next(48, 58)));
			var b = a.Replace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, x.Value.ToString(y => (char)(y - 48 + 'A'))));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace("" + replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, G.IEnumerable<char>>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, String>)null!));
		ProcessPairs("3692581470", "3692581470", []);
		ProcessPairs("3692581470", "3696925814070", new() { { ('6', '9'), "6969" }, { ('1', '4'), "140" } });
		ProcessPairs("3692581470", "3122007370", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("", "", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("6", "6", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('3', '2'), "256" }, { ('2', '3'), "128" }, { ('3', '1'), "888" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('2', '3'), "128" }, { ('3', '2'), "256" }, { ('3', '1'), "888" } });
		ProcessPairs("77777", "777777777", new() { { ('7', '7'), "7777" }, { ('8', '8'), "8888" } });
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), String>)null!));
		ProcessTriples("3692581470", "3692581470", []);
		ProcessTriples("3692581470", "36969581400", new() { { ('6', '9', '2'), "6969" }, { ('1', '4', '7'), "140" } });
		ProcessTriples("3692581470", "391114641470", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("", "", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("6", "6", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("256256256256256!", "40964096409640964096!", new() { { ('2', '5', '6'), "4096" }, { ('5', '6', '2'), "2048" }, { ('6', '2', '5'), "888" } });
		ProcessTriples("3232323232!", "2048409620482!", new() { { ('2', '3', '2'), "4096" }, { ('3', '2', '3'), "2048" }, { ('3', '1', '2'), "888" } });
		ProcessTriples("77777777", "777777777777", new() { { ('7', '7', '7'), "77777" }, { ('8', '8', '8'), "88888" } });
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), String>)null!));
		static void ProcessPairs(string s, string c, Dictionary<(char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.Replace(dic);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		static void ProcessTriples(string s, string c, Dictionary<(char, char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.Replace(dic);
			Assert.IsTrue(a.Equals(s));
			Assert.IsTrue(E.SequenceEqual(s, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestReplaceInPlace()
	{
		for (var i = 0; i < 1000; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldItem = (char)random.Next(33, 127);
			var newItem = (char)random.Next(33, 127);
			var b = a.ReplaceInPlace(oldItem, newItem);
			var c = s.Replace(oldItem, newItem);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(33, 127);
			string s = new(arr);
			String a = s;
			var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
			var newArray = new char[random.Next(10)];
			for (var j = 0; j < newArray.Length; j++)
				newArray[j] = (char)random.Next(33, 127);
			string newCollection = new(newArray);
			var b = a.ReplaceInPlace(oldCollection, newArray);
			var c = s.Replace(oldCollection, newCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace(null!, null!));
		var backConverter = new Chain(48, 10).ToArray(x => ((char)(x - 48 + 'A'), (char)x));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (char)random.Next(48, 58));
			var b = a.ReplaceInPlace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, (char)(x.Value - 48 + 'A')));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace(replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, char>)null!));
		for (var i = 0; i < 100; i++)
		{
			var arr = new char[1000];
			for (var j = 0; j < arr.Length; j++)
				arr[j] = (char)random.Next(48, 58);
			string s = new(arr);
			String a = s;
			var dic = new Chain(48, 10).ToList().Shuffle().Take(random.Next(2, 11)).ToDictionary(x => (char)x, x => (G.IEnumerable<char>)new Chain(random.Next(0, 5)).ToString(y => (char)random.Next(48, 58)));
			var b = a.ReplaceInPlace(dic);
			string c = new(s);
			var replace = dic.ToArray(x => (x.Key, x.Value.ToString(y => (char)(y - 48 + 'A'))));
			for (var j = 0; j < replace.Length; j++)
				c = c.Replace("" + replace[j].Key, replace[j].Item2);
			for (var j = 0; j < backConverter.Length; j++)
				c = c.Replace(backConverter[j].Item1, backConverter[j].Item2);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, G.IEnumerable<char>>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<char, String>)null!));
		ProcessPairs("3692581470", "3692581470", []);
		ProcessPairs("3692581470", "3696925814070", new() { { ('6', '9'), "6969" }, { ('1', '4'), "140" } });
		ProcessPairs("3692581470", "3122007370", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("", "", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("6", "6", new() { { ('6', '9'), "12" }, { ('1', '4'), "3" }, { ('5', '8'), "007" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('3', '2'), "256" }, { ('2', '3'), "128" }, { ('3', '1'), "888" } });
		ProcessPairs("3232323232!", "256256256256256!", new() { { ('2', '3'), "128" }, { ('3', '2'), "256" }, { ('3', '1'), "888" } });
		ProcessPairs("77777", "777777777", new() { { ('7', '7'), "7777" }, { ('8', '8'), "8888" } });
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char), String>)null!));
		ProcessTriples("3692581470", "3692581470", []);
		ProcessTriples("3692581470", "36969581400", new() { { ('6', '9', '2'), "6969" }, { ('1', '4', '7'), "140" } });
		ProcessTriples("3692581470", "391114641470", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("", "", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("6", "6", new() { { ('6', '9', '2'), "911" }, { ('1', '4', '7'), "3" }, { ('5', '8', '1'), "14641" } });
		ProcessTriples("256256256256256!", "40964096409640964096!", new() { { ('2', '5', '6'), "4096" }, { ('5', '6', '2'), "2048" }, { ('6', '2', '5'), "888" } });
		ProcessTriples("3232323232!", "2048409620482!", new() { { ('2', '3', '2'), "4096" }, { ('3', '2', '3'), "2048" }, { ('3', '1', '2'), "888" } });
		ProcessTriples("77777777", "777777777777", new() { { ('7', '7', '7'), "77777" }, { ('8', '8', '8'), "88888" } });
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), G.IEnumerable<char>>)null!));
		Assert.ThrowsException<ArgumentNullException>(() => ((String)"3692581470").Replace((Dictionary<(char, char, char), String>)null!));
		static void ProcessPairs(string s, string c, Dictionary<(char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.ReplaceInPlace(dic);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		static void ProcessTriples(string s, string c, Dictionary<(char, char, char), G.IEnumerable<char>> dic)
		{
			String a = s;
			var b = a.ReplaceInPlace(dic);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
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
		b = new G.List<string>(list);
		b.InsertRange(2, defaultCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentException>(() => a = list.ToList().ReplaceRange(1, 1000, defaultString));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.ToList().ReplaceRange(-1, 3, defaultCollection));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.ToList().ReplaceRange(4, -2, defaultCollection));
		Assert.ThrowsException<ArgumentNullException>(() => list.ToList().ReplaceRange(4, 1, null!));
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
		b = new G.List<string>(list);
		b.AddRange(defaultCollection.Skip(2).Take(3));
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().AddRange(defaultCollection.AsSpan(2, 3)).Reverse(2, 4);
		b = new G.List<string>(list);
		b.AddRange(defaultCollection.Skip(2).Take(3));
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
		b = new G.List<string>(list);
		for (var i = 3; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, 2, 4);
		b = new G.List<string>(list);
		for (var i = 2; i < 6; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, ^5);
		b = new G.List<string>(list);
		for (var i = b.Count - 5; i < b.Count; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = list.ToList().SetAll(defaultString, ^6..4);
		b = new G.List<string>(list);
		for (var i = b.Count - 6; i < 4; i++)
			b[i] = defaultString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var a = list.ToList();
		var b = new G.List<string>(list);
		for (var i = 0; i < 1000; i++)
		{
			var n = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var n2 = random.Next(1000).ToString("D3");
			a.SetOrAdd(n, n2);
			if (n < b.Count)
				b[n] = n2;
			else
				b.Add(n2);
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
		Assert.ThrowsException<ArgumentException>(() => a = list.ToList().SetRange(5, hs));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.ToList().SetRange(-1, hs));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.ToList().SetRange(1000, hs));
		Assert.ThrowsException<ArgumentNullException>(() => list.ToList().SetRange(4, null!));
	}

	[TestMethod]
	public void TestShuffle()
	{
		var toShuffle = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = new List<string>(toShuffle);
		var b = new List<string>(a).Shuffle();
		b.Sort();
		var c = new G.List<string>(a);
		c = new(c.Shuffle());
		c.Sort();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = list.ToList();
		var b = a.Skip(2);
		var c = E.Skip(new G.List<string>(list), 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Skip(0);
		c = E.Skip(new G.List<string>(list), 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Skip(1000);
		c = E.Skip(new G.List<string>(list), 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Skip(-4);
		c = E.Skip(new G.List<string>(list), -4);
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
		var c = E.SkipLast(new G.List<string>(list), 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.SkipLast(0);
		c = E.SkipLast(new G.List<string>(list), 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.SkipLast(1000);
		c = E.SkipLast(new G.List<string>(list), 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.SkipLast(-4);
		c = E.SkipLast(new G.List<string>(list), -4);
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
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => E.All(x, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestSort()
	{
		var toSort = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
		var a = new List<string>(toSort);
		var b = new List<string>(a).Sort();
		var c = new G.List<string>(a);
		c.Sort();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Sort(new Comparer<string>((x, y) => y.CompareTo(x)));
		c = new G.List<string>(a);
		c.Sort(new Comparer<string>((x, y) => y.CompareTo(x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new List<string>(a).Sort(2, 4, new Comparer<string>((x, y) => y.CompareTo(x)));
		c = new G.List<string>(a);
		c.Sort(2, 4, new Comparer<string>((x, y) => y.CompareTo(x)));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = list.ToList();
		var b = a.StartsWith("MMM");
		Assert.IsTrue(b);
		b = a.StartsWith(new List<string>("MMM", "BBB", "PPP"));
		Assert.IsTrue(b);
		b = a.StartsWith(new List<string>("MMM", "BBB", "XXX"));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<string>)null!));
	}

	[TestMethod]
	public void TestTake()
	{
		var a = list.ToList();
		var b = a.Take(2);
		var c = E.Take(new G.List<string>(list), 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Take(0);
		c = E.Take(new G.List<string>(list), 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Take(1000);
		c = E.Take(new G.List<string>(list), 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.Take(-4);
		c = E.Take(new G.List<string>(list), -4);
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
		var c = E.TakeLast(new G.List<string>(list), 2);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.TakeLast(0);
		c = E.TakeLast(new G.List<string>(list), 0);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.TakeLast(1000);
		c = E.TakeLast(new G.List<string>(list), 1000);
		Assert.IsTrue(a.Equals(list));
		Assert.IsTrue(E.SequenceEqual(list, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = [.. list];
		b = a.TakeLast(-4);
		c = E.TakeLast(new G.List<string>(list), -4);
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
		c = new G.List<string>(list);
		c.InsertRange(3, ["$", "###"]);
		d = E.ToList(E.TakeWhile(E.Take(c, 10), x => E.All(x, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray() => BaseListTests<string, List<string>>.TestToArray(() => new((char)random.Next(33, 127), random.Next(10)));

	[TestMethod]
	public void TestTrimExcess() => BaseListTests<string, List<string>>.TestTrimExcess(() => new((char)random.Next(33, 127), random.Next(10)));

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
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x.Length <= 3);
		d = c.TrueForAll(x => x.Length <= 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x.Length > 3);
		d = c.TrueForAll(x => x.Length > 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
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
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string))fullList[..1]);
		Assert.AreEqual(((string, string))fullList[..2], ("AAA", "BBB"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string))fullList[..3]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string))fullList[..2]);
		Assert.AreEqual(((string, string, string))fullList[..3], ("AAA", "BBB", "CCC"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string))fullList[..4]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string))fullList[..3]);
		Assert.AreEqual(((string, string, string, string))fullList[..4], ("AAA", "BBB", "CCC", "DDD"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string))fullList[..5]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string))fullList[..4]);
		Assert.AreEqual(((string, string, string, string, string))fullList[..5], ("AAA", "BBB", "CCC", "DDD", "EEE"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string))fullList[..6]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string))fullList[..5]);
		Assert.AreEqual(((string, string, string, string, string, string))fullList[..6], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string))fullList[..7]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string))fullList[..6]);
		Assert.AreEqual(((string, string, string, string, string, string, string))fullList[..7], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string))fullList[..8]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string))fullList[..7]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string))fullList[..8], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string))fullList[..9]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string))fullList[..8]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string))fullList[..9], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string))fullList[..10]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string))fullList[..9]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string))fullList[..10], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string))fullList[..11]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string))fullList[..10]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string))fullList[..11], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string))fullList[..12]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string))fullList[..11]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string))fullList[..12], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string))fullList[..13]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..12]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..13], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..14]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..13]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..14], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..15]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..14]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..15], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO"));
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..16]);
		Assert.ThrowsException<InvalidOperationException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..15]);
		Assert.AreEqual(((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..16], ("AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "KKK", "LLL", "MMM", "NNN", "OOO", "PPP"));
		Assert.ThrowsException<ArgumentException>(() => ((string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string))fullList[..17]);
	}
}

[TestClass]
public class NListTests
{
	[TestMethod]
	public void ConstructionTest()
	{
		var array = new NList<int>[1000];
		for (var i = 0; i < array.Length; i++)
		{
			array[i] = new[] { () => new NList<int>(), () => new(500), () => new(RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(RedStarLinq.FillArray(random.Next(500), _ => random.Next()).AsSpan()), () => new((G.IEnumerable<int>)RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(RedStarLinq.Fill(random.Next(500), _ => random.Next())), () => new(RedStarLinq.NFill(random.Next(500), _ => random.Next())), () => new(E.Select(RedStarLinq.Fill(random.Next(500), _ => random.Next()), x => x)), () => new(E.SkipWhile(RedStarLinq.Fill(random.Next(500), _ => random.Next()), _ => random.Next(10) == -1)), () => new(500, RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(500, RedStarLinq.FillArray(random.Next(500), _ => random.Next()).AsSpan()), () => new(500, (G.IEnumerable<int>)RedStarLinq.FillArray(random.Next(500), _ => random.Next())), () => new(500, RedStarLinq.Fill(random.Next(500), _ => random.Next())), () => new(500, RedStarLinq.NFill(random.Next(500), _ => random.Next())), () => new(500, E.Select(RedStarLinq.Fill(random.Next(500), _ => random.Next()), x => x)), () => new(500, E.SkipWhile(RedStarLinq.Fill(random.Next(500), _ => random.Next()), _ => random.Next(10) == -1)) }.Random(random)();
			for (var j = 0; j < 1000; j++)
			{
				array[i].Add(random.Next());
				Assert.IsTrue(array[i].Capacity >= array[i].Length);
			}
		}
		Thread.Sleep(50);
		for (var i = 0; i < array.Length; i++)
			array[i].Add(random.Next());
	}

	[TestMethod]
	public void TestAdd()
	{
		var a = nList.ToNList().Add(defaultNString);
		var b = new G.List<(char, char, char)>(nList) { defaultNString };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new NList<(char, char, char)>(10, nList).Add(defaultNString);
		b = new G.List<(char, char, char)>(nList) { defaultNString };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAddRange()
	{
		var a = nList.ToNList().AddRange(defaultNCollection);
		var b = new G.List<(char, char, char)>(nList);
		b.AddRange(defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3));
		b = new G.List<(char, char, char)>(nList);
		b.AddRange(defaultNCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = nList.ToNList();
		var b = a.Append(defaultNString);
		var c = E.Append(new G.List<(char, char, char)>(nList), defaultNString);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.BreakFilter(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0', out var c);
		var d = new G.List<(char, char, char)>(nList);
		d.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var e = E.ToList(E.Where(d, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		var f = E.ToList(E.Where(d, x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0'));
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
		b = a.BreakFilter(x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'), out c);
		e = E.ToList(E.Where(d, x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
		f = E.ToList(E.Where(d, x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
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
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.BreakFilterInPlace(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0', out var c);
		var d = new G.List<(char, char, char)>(nList);
		d.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var e = E.ToList(E.Where(d, x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0'));
		d = E.ToList(E.Where(d, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		BaseListTests<(char, char, char), NList<(char, char, char)>>.BreakFilterInPlaceAsserts(a, b, c, d, e);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.BreakFilterInPlace(x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'), out c);
		d = new G.List<(char, char, char)>(nList);
		d.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		e = E.ToList(E.Where(d, x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
		d = E.ToList(E.Where(d, x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
		BaseListTests<(char, char, char), NList<(char, char, char)>>.BreakFilterInPlaceAsserts(a, b, c, d, e);
	}

	[TestMethod]
	public void TestClear()
	{
		var a = nList.ToNList();
		a.Clear(2, 4);
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < 4; i++)
			b[2 + i] = default!;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestCompare()
	{
		for (var i = 0; i < 1000; i++)
		{
			var a = new NList<(char, char, char)>(E.Select(E.Range(0, random.Next(3, 100)), _ => Next()));
			var b = new NList<(char, char, char)>(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b) == n);
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => Next()));
			b = new(a);
			n = random.Next(2, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b, n - 1) == n - 1);
			a = new(E.Select(E.Range(0, random.Next(5, 100)), _ => Next()));
			b = new(a);
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = Next();
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => Next()));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => Next()));
			Assert.IsTrue(a.Compare(index, b, otherIndex) == n);
			Assert.IsTrue(a.Compare(index, b, otherIndex, length) == n);
		}
		static (char, char, char) Next() => ((char, char, char))RedStarLinq.ToList(random.Next(1000).ToString("D3"));
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = nList.ToNList();
		var b = a.Concat(new(defaultNCollection));
		var c = E.Concat(new G.List<(char, char, char)>(nList), defaultNCollection);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = nList.ToNList();
		var b = a.Contains(('M', 'M', 'M'));
		Assert.IsTrue(b);
		b = a.Contains(('B', 'B', 'B'), 2);
		Assert.IsTrue(!b);
		b = a.Contains(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.IsTrue(b);
		b = a.Contains(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = nList.ToNList();
		var b = a.ContainsAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.IsTrue(b);
		b = a.ContainsAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N')));
		Assert.IsTrue(b);
		b = a.ContainsAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = nList.ToNList();
		var b = a.ContainsAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = nList.ToNList();
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(new G.List<(char, char, char)>(nList), (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(new G.List<(char, char, char)>(nList), x => x + "A");
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(d.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestCopy()
	{
		int length, capacity;
		NList<(char, char, char)> a;
		NList<(char, char, char)> b;
		int index;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(1, 51);
			capacity = length + random.Next(9951);
			a = new(capacity);
			for (var j = 0; j < length; j++)
				a.Add(((char)random.Next(33, 126), (char)random.Next(33, 126), (char)random.Next(33, 126)));
			b = a.Copy();
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(a, b));
			index = random.Next(a.Length);
			a[index] = ((char)(a[index].Item1 + 1), (char)(a[index].Item2 + 1), (char)(a[index].Item3 + 1));
			Assert.IsTrue(!RedStarLinq.Equals(a, b));
			Assert.IsTrue(!E.SequenceEqual(a, b));
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = nList.ToNList();
		var b = RedStarLinq.FillArray(16, x => ((char)random.Next(65536), (char)random.Next(65536), (char)random.Next(65536)));
		var c = ((char, char, char)[])b.Clone();
		var d = ((char, char, char)[])b.Clone();
		var e = ((char, char, char)[])b.Clone();
		a.CopyTo(b);
		new G.List<(char, char, char)>(nList).CopyTo(c);
		a.CopyTo(d, 3);
		new G.List<(char, char, char)>(nList).CopyTo(e, 3);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestEndsWith()
	{
		var a = nList.ToNList();
		var b = a.EndsWith(('D', 'D', 'D'));
		Assert.IsTrue(b);
		b = a.EndsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D')));
		Assert.IsTrue(b);
		b = a.EndsWith(new NList<(char, char, char)>(('P', 'P', 'P'), ('E', 'E', 'E'), ('D', 'D', 'D')));
		Assert.IsTrue(!b);
		b = a.EndsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('E', 'E', 'E'), ('N', 'N', 'N')));
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestEquals()
	{
		for (var i = 0; i < 1000; i++)
		{
			var a = new NList<(char, char, char)>(E.Select(E.Range(0, random.Next(2, 100)), _ => Next()));
			G.IEnumerable<(char, char, char)> b = new NList<(char, char, char)>(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new NList<(char, char, char)>(E.Append(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new NList<(char, char, char)>(E.Skip(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new NList<(char, char, char)>(E.Prepend(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new NList<(char, char, char)>(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new NList<(char, char, char)>(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new NList<(char, char, char)>(E.Prepend(E.Skip(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(E.Append(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(E.Skip(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(E.Prepend(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new List<(char, char, char)>(E.Prepend(E.Skip(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Append(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Skip(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Prepend(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.ToArray(E.Prepend(E.Skip(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(a);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(E.Append(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(E.Skip(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(E.Prepend(b, Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(E.SkipLast(b, 1));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(E.Append(E.SkipLast(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = new G.List<(char, char, char)>(E.Prepend(E.Skip(b, 1), Next()));
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = [];
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(a, x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Append(b, Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Skip(b, 1), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Prepend(b, Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.SkipLast(b, 1), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Append(E.SkipLast(b, 1), Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Prepend(E.Skip(b, 1), Next()), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.Select(E.Take(a, 0), x => x);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(a, _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Append(a, Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Skip(a, 1), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Prepend(a, Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.SkipLast(a, 1), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Append(E.SkipLast(a, 1), Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.SkipWhile(E.Prepend(E.Skip(a, 1), Next()), _ => random.Next(10) != -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
			b = E.TakeWhile(a, _ => random.Next(10) == -1);
			Assert.AreEqual(a.Equals(b), E.SequenceEqual(a, b));
		}
		static (char, char, char) Next() => ((char, char, char))RedStarLinq.ToList(random.Next(1000).ToString("D3"));
	}

	[TestMethod]
	public void TestFilter()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.Filter(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = E.Where(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		b = a.Filter((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		d = E.Where(c, (x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FilterInPlace(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		c = E.ToList(E.Where(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FilterInPlace((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		c = E.ToList(E.Where(c, (x, index) => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z') && index >= 1));
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
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.Find(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.Find(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.Find(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.Find(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindAll(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindIndex(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindLast(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindLast(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindLast(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindLast(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.FindLastIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.FindLastIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.FindLastIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.FindLastIndex(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = nList.ToNList();
		var b = a.GetAfter(('D', 'D', 'D'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter([]);
		c = [];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('E', 'E', 'E'), ('D', 'D', 'D')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		var a = nList.ToNList();
		var b = a.GetAfterLast(('M', 'M', 'M'));
		var c = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast([]);
		c = [];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('E', 'E', 'E'), ('D', 'D', 'D')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = nList.ToNList();
		var b = a.GetBefore(('D', 'D', 'D'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore([]);
		c = new(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		var a = nList.ToNList();
		var b = a.GetBeforeLast(('M', 'M', 'M'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast([]);
		c = new(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast(new(('D', 'D', 'D'), ('M', 'M', 'M')));
		c = [('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')];
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		var a = nList.ToNList();
		var b = a.GetBeforeSetAfter(('D', 'D', 'D'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
		var d = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		var a = nList.ToNList();
		var b = a.GetBeforeSetAfterLast(('M', 'M', 'M'));
		var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D') };
		var d = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetRange()
	{
		var a = nList.ToNList();
		var b = a.GetRange(.., true);
		b.Add(defaultNString);
		var c = new G.List<(char, char, char)>(nList).GetRange(0, nList.Length);
		c.Add(defaultNString);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		new BaseIndexableTests<(char, char, char), NList<(char, char, char)>>(a, nList, defaultNString, defaultNCollection).TestGetRange();
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = nList.ToNList();
		new BaseIndexableTests<(char, char, char), NList<(char, char, char)>>(a, nList, defaultNString, defaultNCollection).TestGetSlice();
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = nList.ToNList();
		var b = a.IndexOf(('M', 'M', 'M'));
		Assert.AreEqual(b, 0);
		b = a.IndexOf(('B', 'B', 'B'), 2);
		Assert.AreEqual(b, -1);
		b = a.IndexOf(('B', 'B', 'B'), 1, 2);
		Assert.AreEqual(b, 1);
		b = a.IndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(b, 2);
		b = a.IndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')));
		Assert.AreEqual(b, -1);
		b = a.IndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 4);
		Assert.AreEqual(b, 4);
		b = a.IndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 0, 4);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOf((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = nList.ToNList();
		var b = a.IndexOfAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(b, 0);
		b = a.IndexOfAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('N', 'N', 'N'), ('P', 'P', 'P')));
		Assert.AreEqual(b, 2);
		b = a.IndexOfAny(new[] { ('L', 'L', 'L'), ('N', 'N', 'N'), ('P', 'P', 'P') }, 4);
		Assert.AreEqual(b, -1);
		b = a.IndexOfAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = nList.ToNList();
		var b = a.IndexOfAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(b, 1);
		b = a.IndexOfAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(b, 0);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestInsert()
	{
		var a = nList.ToNList().Insert(3, defaultNString);
		var b = new G.List<(char, char, char)>(nList);
		b.Insert(3, defaultNString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().Insert(4, defaultNCollection);
		b = new G.List<(char, char, char)>(nList);
		b.InsertRange(4, defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().Insert(2, defaultNCollection.AsSpan(2, 3));
		b = new G.List<(char, char, char)>(nList);
		b.InsertRange(2, defaultNCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new NList<(char, char, char)>(10, nList).Insert(3, defaultNString);
		b = new G.List<(char, char, char)>(nList);
		b.Insert(3, defaultNString);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new NList<(char, char, char)>(10, nList).Insert(4, defaultNCollection);
		b = new G.List<(char, char, char)>(nList);
		b.InsertRange(4, defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new NList<(char, char, char)>(10, nList).Insert(2, defaultNCollection.AsSpan(2, 3));
		b = new G.List<(char, char, char)>(nList);
		b.InsertRange(2, defaultNCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a = nList.ToNList().Insert(1000, defaultNString));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => nList.ToNList().Insert(-1, defaultNCollection));
		Assert.ThrowsException<ArgumentNullException>(() => nList.ToNList().Insert(5, (G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = nList.ToNList();
		var b = a.LastIndexOf(('M', 'M', 'M'));
		Assert.AreEqual(b, 4);
		b = a.LastIndexOf(('B', 'B', 'B'), 2);
		Assert.AreEqual(b, 1);
		b = a.LastIndexOf(('B', 'B', 'B'), 3, 2);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(b, 2);
		b = a.LastIndexOf(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')));
		Assert.AreEqual(b, -1);
		b = a.LastIndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 3);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOf(new[] { ('M', 'M', 'M'), ('E', 'E', 'E') }, 5, 4);
		Assert.AreEqual(b, 4);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOf((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = nList.ToNList();
		var b = a.LastIndexOfAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(b, 6);
		b = a.LastIndexOfAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('N', 'N', 'N'), ('P', 'P', 'P')));
		Assert.AreEqual(b, 2);
		b = a.LastIndexOfAny(new[] { ('L', 'L', 'L'), ('N', 'N', 'N'), ('E', 'E', 'E') }, 4);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOfAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = nList.ToNList();
		var b = a.LastIndexOfAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
		Assert.AreEqual(b, 5);
		b = a.LastIndexOfAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
		Assert.AreEqual(b, 6);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestPad()
	{
		var a = nList.ToNList();
		var b = a.Pad(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Add(default!);
		c.Add(default!);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		c.Insert(0, ('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5);
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(5, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pad(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pad(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadInPlace()
	{
		var a = nList.ToNList();
		var b = a.PadInPlace(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Add(default!);
		c.Add(default!);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadInPlace(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		c.Insert(0, ('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		c.Add(('X', 'X', 'X'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadInPlace(5);
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadInPlace(5, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadInPlace(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadLeft()
	{
		var a = nList.ToNList();
		var b = a.PadLeft(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Insert(0, default!);
		c.Insert(0, default!);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5);
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(5, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeft(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeft(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadLeftInPlace()
	{
		var a = nList.ToNList();
		var b = a.PadLeftInPlace(10);
		var c = new G.List<(char, char, char)>(nList);
		c.Insert(0, default!);
		c.Insert(0, default!);
		c.Insert(0, default!);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadLeftInPlace(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		c.Insert(0, ('X', 'X', 'X'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadLeftInPlace(5);
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadLeftInPlace(5, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadRight()
	{
		var a = nList.ToNList();
		var b = a.PadRight(10);
		var c = new G.List<(char, char, char)>(nList) { default!, default!, default! };
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList) { ('X', 'X', 'X'), ('X', 'X', 'X'), ('X', 'X', 'X') };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5);
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(5, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRight(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRight(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestPadRightInPlace()
	{
		var a = nList.ToNList();
		var b = a.PadRightInPlace(10);
		var c = new G.List<(char, char, char)>(nList) { default!, default!, default! };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadRightInPlace(10, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList) { ('X', 'X', 'X'), ('X', 'X', 'X'), ('X', 'X', 'X') };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadRightInPlace(5);
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(nList);
		b = a.PadRightInPlace(5, ('X', 'X', 'X'));
		c = new G.List<(char, char, char)>(nList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1, ('X', 'X', 'X')));
	}

	[TestMethod]
	public void TestRemove()
	{
		var a = nList.ToNList();
		var b = new NList<(char, char, char)>(a).Remove(6);
		var c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(6, 1);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(0, 1);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1, nList.Length - 2);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(1, nList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1, 4);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(nList.Length - 5, 4);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(nList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(nList.Length - 5, 10 - nList.Length);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(nList.Length - 5, 10 - nList.Length);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(-1, 6));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(nList.Length - 1, 2 - nList.Length));
		Assert.ThrowsException<ArgumentException>(() => b = new NList<(char, char, char)>(a).Remove(1, 1000));
		b = new NList<(char, char, char)>(a).Remove(..);
		c = [];
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(..^1);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(0, nList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1..);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(1, nList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1..^1);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(1, nList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(1..5);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(^5..^1);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(nList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new NList<(char, char, char)>(a).Remove(^5..5);
		c = new G.List<(char, char, char)>(nList);
		c.RemoveRange(nList.Length - 5, 10 - nList.Length);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new NList<(char, char, char)>(a).Remove(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = new NList<(char, char, char)>(a).Remove(1..1000));
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.RemoveAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.RemoveAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.RemoveAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = c.RemoveAll(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		var a = nList.ToNList();
		for (var i = 0; i < 1000; i++)
		{
			var index = random.Next(a.Length);
			var b = new NList<(char, char, char)>(a).RemoveAt(index);
			var c = new G.List<(char, char, char)>(a);
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
		var a = new Chain(15, 10).ToNList(x => ((char, char, char))(String)x.ToString("D3"));
		for (var i = 0; i < 1000; i++)
		{
			var value = a.Random(random);
			var b = new NList<(char, char, char)>(a);
			b.RemoveValue(value);
			var c = new G.List<(char, char, char)>(a);
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
		for (var i = 0; i < 1000; i++)
		{
			var arr = RedStarLinq.FillArray(random.Next(1, 1001), _ => random.Next());
			var a = arr.ToNList();
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
		var a = nList.ToNList().Replace(defaultNCollection);
		var b = new G.List<(char, char, char)>(nList);
		b.Clear();
		b.AddRange(defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3));
		b = new G.List<(char, char, char)>(nList);
		b.AddRange(defaultNCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		var a = nList.ToNList().ReplaceRange(2, 3, defaultNCollection);
		var b = new G.List<(char, char, char)>(nList);
		b.RemoveRange(2, 3);
		b.InsertRange(2, defaultNCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().Insert(2, defaultNCollection.AsSpan(2, 3));
		b = new G.List<(char, char, char)>(nList);
		b.InsertRange(2, defaultNCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentException>(() => a = nList.ToNList().ReplaceRange(1, 1000, defaultNString));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => nList.ToNList().ReplaceRange(-1, 3, defaultNCollection));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => nList.ToNList().ReplaceRange(4, -2, defaultNCollection));
		Assert.ThrowsException<ArgumentNullException>(() => nList.ToNList().ReplaceRange(4, 1, null!));
	}

	[TestMethod]
	public void TestReverse()
	{
		var a = nList.ToNList().Reverse();
		var b = new G.List<(char, char, char)>(nList);
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3)).Reverse();
		b = new G.List<(char, char, char)>(nList);
		b.AddRange(defaultNCollection.Skip(2).Take(3));
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().AddRange(defaultNCollection.AsSpan(2, 3)).Reverse(2, 4);
		b = new G.List<(char, char, char)>(nList);
		b.AddRange(defaultNCollection.Skip(2).Take(3));
		b.Reverse(2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetAll()
	{
		var a = nList.ToNList().SetAll(defaultNString);
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < b.Count; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, 3);
		b = new G.List<(char, char, char)>(nList);
		for (var i = 3; i < b.Count; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, 2, 4);
		b = new G.List<(char, char, char)>(nList);
		for (var i = 2; i < 6; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, ^5);
		b = new G.List<(char, char, char)>(nList);
		for (var i = b.Count - 5; i < b.Count; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = nList.ToNList().SetAll(defaultNString, ^6..4);
		b = new G.List<(char, char, char)>(nList);
		for (var i = b.Count - 6; i < 4; i++)
			b[i] = defaultNString;
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var a = nList.ToNList();
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < 1000; i++)
		{
			var n = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var n2 = random.Next(1000).ToString("D3");
			var n3 = (n2[0], n2[1], n2[2]);
			a.SetOrAdd(n, n3);
			if (n < b.Count)
				b[n] = n3;
			else
				b.Add(n3);
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		var hs = defaultNCollection.ToHashSet();
		var a = nList.ToNList().SetRange(2, hs);
		var b = new G.List<(char, char, char)>(nList);
		for (var i = 0; i < hs.Length; i++)
			b[i + 2] = hs[i];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentException>(() => a = nList.ToNList().SetRange(5, hs));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => nList.ToNList().SetRange(-1, hs));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => nList.ToNList().SetRange(1000, hs));
		Assert.ThrowsException<ArgumentNullException>(() => nList.ToNList().SetRange(4, null!));
	}

	[TestMethod]
	public void TestShuffle()
	{
		var toShuffle = new G.List<(char, char, char)>(new byte[256].ToArray(x => ((char)random.Next(65536), (char)random.Next(65536), (char)random.Next(65536))));
		var a = new NList<(char, char, char)>(toShuffle);
		var b = new NList<(char, char, char)>(a).Shuffle();
		b.Replace(b.ToList().Sort());
		var c = new G.List<(char, char, char)>(a);
		c = new(c.Shuffle());
		c.Sort();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = nList.ToNList();
		var b = a.Skip(2);
		var c = E.Skip(new G.List<(char, char, char)>(nList), 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Skip(0);
		c = E.Skip(new G.List<(char, char, char)>(nList), 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Skip(1000);
		c = E.Skip(new G.List<(char, char, char)>(nList), 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Skip(-4);
		c = E.Skip(new G.List<(char, char, char)>(nList), -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = nList.ToNList();
		var b = a.SkipLast(2);
		var c = E.SkipLast(new G.List<(char, char, char)>(nList), 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.SkipLast(0);
		c = E.SkipLast(new G.List<(char, char, char)>(nList), 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.SkipLast(1000);
		c = E.SkipLast(new G.List<(char, char, char)>(nList), 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.SkipLast(-4);
		c = E.SkipLast(new G.List<(char, char, char)>(nList), -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.SkipWhile(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = E.ToList(E.SkipWhile(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.SkipWhile((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') || index < 1);
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestSort()
	{
		var c = new G.List<(char, char, char)>(new byte[256].ToArray(x => ((char)random.Next(65536), (char)random.Next(65536), (char)random.Next(65536))));
		var a = new NList<(char, char, char)>(c);
		var b = new NList<(char, char, char)>(a).Sort(x => x.Item3);
		c = E.ToList(E.OrderBy(c, x => x.Item3));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = nList.ToNList();
		var b = a.StartsWith(('M', 'M', 'M'));
		Assert.IsTrue(b);
		b = a.StartsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')));
		Assert.IsTrue(b);
		b = a.StartsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('B', 'B', 'B'), ('X', 'X', 'X')));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<(char, char, char)>)null!));
	}

	[TestMethod]
	public void TestTake()
	{
		var a = nList.ToNList();
		var b = a.Take(2);
		var c = E.Take(new G.List<(char, char, char)>(nList), 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Take(0);
		c = E.Take(new G.List<(char, char, char)>(nList), 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Take(1000);
		c = E.Take(new G.List<(char, char, char)>(nList), 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.Take(-4);
		c = E.Take(new G.List<(char, char, char)>(nList), -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = nList.ToNList();
		var b = a.TakeLast(2);
		var c = E.TakeLast(new G.List<(char, char, char)>(nList), 2);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.TakeLast(0);
		c = E.TakeLast(new G.List<(char, char, char)>(nList), 0);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.TakeLast(1000);
		c = E.TakeLast(new G.List<(char, char, char)>(nList), 1000);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = nList.ToNList();
		b = a.TakeLast(-4);
		c = E.TakeLast(new G.List<(char, char, char)>(nList), -4);
		Assert.IsTrue(a.Equals(nList));
		Assert.IsTrue(E.SequenceEqual(nList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.TakeWhile(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = E.ToList(E.TakeWhile(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		b = a.TakeWhile((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index < 10);
		c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		d = E.ToList(E.TakeWhile(E.Take(c, 10), x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray() => BaseListTests<(char, char, char), NList<(char, char, char)>>.TestToArray(() => ((char)random.Next(33, 127), (char)random.Next(33, 127), (char)random.Next(33, 127)));

	[TestMethod]
	public void TestTrimExcess() => BaseListTests<(char, char, char), NList<(char, char, char)>>.TestTrimExcess(() => ((char)random.Next(33, 127), (char)random.Next(33, 127), (char)random.Next(33, 127)));

	[TestMethod]
	public void TestTrueForAll()
	{
		var a = nList.ToNList().Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
		var b = a.TrueForAll(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		var c = new G.List<(char, char, char)>(nList);
		c.InsertRange(3, [('$', '\0', '\0'), ('#', '#', '#')]);
		var d = c.TrueForAll(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x.Item1 != '\0');
		d = c.TrueForAll(x => x.Item1 != '\0');
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => new[] { x.Item1, x.Item2, x.Item3 }.Length > 3);
		d = c.TrueForAll(x => new[] { x.Item1, x.Item2, x.Item3 }.Length > 3);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestTuples()
	{
		var a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'));
		var b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = (NList<(char, char, char)>)(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'), ('P', 'P', 'P'));
		b = new NList<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'), ('P', 'P', 'P'));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		var fullNList = b.Copy();
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char)))fullNList[..1]);
		Assert.AreEqual((((char, char, char), (char, char, char)))fullNList[..2], (('A', 'A', 'A'), ('B', 'B', 'B')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char)))fullNList[..3]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char)))fullNList[..2]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char)))fullNList[..3], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char)))fullNList[..4]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..3]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..4], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..5]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..4]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..5], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..6]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..5]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..6], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..7]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..6]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..7], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..8]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..7]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..8], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..9]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..8]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..9], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..10]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..9]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..10], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..11]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..10]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..11], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..12]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..11]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..12], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..13]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..12]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..13], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..14]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..13]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..14], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..15]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..14]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..15], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O')));
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..16]);
		Assert.ThrowsException<InvalidOperationException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..15]);
		Assert.AreEqual((((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..16], (('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('E', 'E', 'E'), ('F', 'F', 'F'), ('G', 'G', 'G'), ('H', 'H', 'H'), ('I', 'I', 'I'), ('J', 'J', 'J'), ('K', 'K', 'K'), ('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N'), ('O', 'O', 'O'), ('P', 'P', 'P')));
		Assert.ThrowsException<ArgumentException>(() => (((char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char), (char, char, char)))fullNList[..17]);
	}
}

[TestClass]
public class BaseSumListTests<T, TCertain> where T : INumber<T> where TCertain : BaseSumList<T, TCertain>, new()
{
	public static void ComplexTest(Func<(BaseSumList<T, TCertain>, G.List<T>, byte[])> create, Func<T> newValueFunc, Action<int> check, Action<byte[]> check2)
	{
		var counter = 0;
	l1:
		var (sl, gl, bytes) = create();
		var updateActions = new[] { (int key) =>
		{
			var newValue = newValueFunc();
			sl.Update(key, newValue);
			if (newValue <= T.Zero)
				gl.RemoveAt(key);
			else
				gl[key] = newValue;
		}, key =>
		{
			sl.Increase(key);
			gl[key]++;
		}, key =>
		{
			if (sl[key] == T.One)
				gl.RemoveAt(key);
			else
				gl[key]--;
			sl.Decrease(key);
		} };
		var actions = new[] { () =>
		{
			var n = newValueFunc();
			while (n < T.One)
				n = newValueFunc();
			if (random.Next(2) == 0)
			{
				sl.Add(n);
				gl.Add(n);
			}
			else
			{
				var index = random.Next(sl.Length + 1);
				sl.Insert(index, n);
				gl.Insert(index, n);
			}
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
		}, () =>
		{
			if (sl.Length == 0) return;
			var index = random.Next(sl.Length);
			gl.RemoveAt(index);
			sl.RemoveAt(index);
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
		}, () =>
		{
			if (sl.Length == 0) return;
			var index = random.Next(sl.Length);
			updateActions.Random(random)(index);
			Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			check(index);
		}, () =>
		{
			random.NextBytes(bytes);
			check2(bytes);
		}, () =>
		{
			if (sl.Length == 0) return;
			var index = random.Next(sl.Length);
			Assert.AreEqual(sl[index], gl[index]);
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 1000)
			goto l1;
	}
}

[TestClass]
public class SumListTests
{
	private SumList sl = default!;
	private G.List<int> gl = default!;

	[TestMethod]
	public void ComplexTest() => BaseSumListTests<int, SumList>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(16, _ => random.Next(1, 16));
		sl = new(arr);
		gl = new(arr);
		var bytes = new byte[16];
		return (sl, gl, bytes);
	}, () => random.Next(16), index => Assert.AreEqual(sl.GetLeftValuesSum(index, out var value), E.Sum(E.Take(gl, index))), bytes =>
	{
		var index = sl.IndexOfNotGreaterSum(CreateVar((long)(new MpzT(bytes, 1) % (sl.ValuesSum + 1)), out var sum));
		Assert.IsTrue(index == gl.Count && sum == E.Sum(gl) || CreateVar(E.Sum(E.Take(gl, index)), out var sum2) <= sum && (gl[index] == 0 || sum2 + gl[index] > sum));
	});
}

[TestClass]
public class BigSumListTests
{
	private BigSumList sl = default!;
	private G.List<MpzT> gl = default!;
	private readonly byte[] bytes = new byte[20], bytes2 = new byte[48];

	[TestMethod]
	public void ComplexTest() => BaseSumListTests<MpzT, BigSumList>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(16, _ =>
		{
			random.NextBytes(bytes);
			return new MpzT(bytes, 1);
		});
		sl = new(arr);
		gl = new(arr);
		return (sl, gl, bytes2);
	}, () =>
	{
		random.NextBytes(bytes);
		return new(bytes, 1);
	}, index => Assert.AreEqual(sl.GetLeftValuesSum(index, out var value), index == 0 ? 0 : E.Aggregate(E.Take(gl, index), (x, y) => x + y)), bytes =>
	{
		var index = sl.IndexOfNotGreaterSum(CreateVar(new MpzT(bytes, 1) % (sl.ValuesSum + 1), out var sum));
		Assert.IsTrue(index == 0 && (gl.Count == 0 || sum < gl[0]) || index == gl.Count && sum == E.Aggregate(gl, (x, y) => x + y) || CreateVar(E.Aggregate(E.Take(gl, index + 1), (x, y) => x + y), out var sum2) > sum && (gl[index] == 0 || sum2 + gl[index] > sum));
	});
}
