namespace Corlib.NStar.Tests;

[TestClass]
public class ListTests
{
	[TestMethod]
	public void TestAdd()
	{
		try
		{
			var a = new List<string>(list).Add(defaultString);
			var b = new G.List<string>(list) { defaultString };
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestAddRange()
	{
		try
		{
			var a = new List<string>(list).AddRange(defaultCollection);
			var b = new G.List<string>(list);
			b.AddRange(defaultCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).AddRange(defaultCollection.AsSpan(2, 3));
			b = new G.List<string>(list);
			b.AddRange(defaultCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestAppend()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.Append(defaultString);
			var c = E.Append(new G.List<string>(list), defaultString);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestBinarySearch()
	{
		try
		{
			var a = new List<string>(list).Sort();
			var b = a.BinarySearch("MMM");
			var c = new G.List<string>(list);
			c.Sort();
			var d = c.BinarySearch("MMM");
			Assert.AreEqual(b, d);
			b = a.BinarySearch("NNN");
			d = c.BinarySearch("NNN");
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.BreakFilter(x => x.Length == 3, out var c);
			var d = new G.List<string>(list);
			d.InsertRange(3, new G.List<string>() { "$", "###" });
			var e = E.Where(d, x => x.Length == 3);
			var f = E.Where(d, x => x.Length != 3);
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, b));
			Assert.IsTrue(c.Equals(f));
			Assert.IsTrue(E.SequenceEqual(f, c));
			b = a.BreakFilter(x => x.All(y => y is >= 'A' and <= 'Z'), out c);
			e = E.Where(d, x => E.All(x, y => y is >= 'A' and <= 'Z'));
			f = E.Where(d, x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, b));
			Assert.IsTrue(c.Equals(f));
			Assert.IsTrue(E.SequenceEqual(f, c));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestBreakFilterInPlace()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.BreakFilterInPlace(x => x.Length == 3, out var c);
			var d = new G.List<string>(list);
			d.InsertRange(3, new G.List<string>() { "$", "###" });
			var e = E.ToList(E.Where(d, x => x.Length != 3));
			d = E.ToList(E.Where(d, x => x.Length == 3));
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(c.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, c));
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.BreakFilterInPlace(x => x.All(y => y is >= 'A' and <= 'Z'), out c);
			d = new G.List<string>(list);
			d.InsertRange(3, new G.List<string>() { "$", "###" });
			e = E.ToList(E.Where(d, x => !E.All(x, y => y is >= 'A' and <= 'Z')));
			d = E.ToList(E.Where(d, x => E.All(x, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(c.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, c));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestClear()
	{
		try
		{
			var a = new List<string>(list);
			a.Clear(2, 4);
			var b = new G.List<string>(list);
			for (int i = 0; i < 4; i++)
				b[2 + i] = default!;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.Concat(new(defaultCollection));
			var c = E.Concat(new G.List<string>(list), defaultCollection);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestContains()
	{
		try
		{
			var a = new List<string>(list);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestContainsAny()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.ContainsAny(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.ContainsAny(new List<string>("LLL", "MMM", "NNN"));
			Assert.IsTrue(b);
			b = a.ContainsAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.ContainsAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(a);
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestConvert()
	{
		try
		{
			var a = new List<string>(list);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		try
		{
			var a = new List<string>(list);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestEndsWith()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.EndsWith("DDD");
			Assert.IsTrue(b);
			b = a.EndsWith(new List<string>("MMM", "EEE", "DDD"));
			Assert.IsTrue(b);
			b = a.EndsWith(new List<string>("PPP", "EEE", "DDD"));
			Assert.IsTrue(!b);
			b = a.EndsWith(new List<string>("MMM", "EEE", "NNN"));
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestEquals()
	{
		try
		{
			var a = new List<string>(list);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFilter()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.Filter(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.FilterInPlace(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			c = E.ToList(E.Where(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.FilterInPlace((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index >= 1);
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			c = E.ToList(E.Where(c, (x, index) => E.All(x, y => y is >= 'A' and <= 'Z') && index >= 1));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFind()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.Find(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.Find(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.Find(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.Find(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindAll()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.FindAll(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindAll(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.FindAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindAll(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindIndex()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.FindIndex(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindIndex(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.FindIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindLast()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.FindLast(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindLast(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.FindLast(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindLast(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.FindLastIndex(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.FindLastIndex(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.FindLastIndex(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.FindLastIndex(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetAfter()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetAfter("DDD");
			var c = new G.List<string>() { "MMM", "EEE", "DDD" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfter(new());
			c = new();
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfter(new("DDD", "MMM"));
			c = new G.List<string>() { "EEE", "DDD" };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetAfterLast("MMM");
			var c = new G.List<string>() { "EEE", "DDD" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfterLast(new());
			c = new();
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfterLast(new("DDD", "MMM"));
			c = new G.List<string>() { "EEE", "DDD" };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBefore()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetBefore("DDD");
			var c = new G.List<string>() { "MMM", "BBB", "PPP" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBefore(new());
			c = new(list);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBefore(new("DDD", "MMM"));
			c = new G.List<string>() { "MMM", "BBB", "PPP" };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetBeforeLast("MMM");
			var c = new G.List<string>() { "MMM", "BBB", "PPP", "DDD" };
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBeforeLast(new());
			c = new(list);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBeforeLast(new("DDD", "MMM"));
			c = new G.List<string>() { "MMM", "BBB", "PPP" };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetBeforeSetAfter("DDD");
			var c = new G.List<string>() { "MMM", "BBB", "PPP" };
			var d = new G.List<string>() { "MMM", "EEE", "DDD" };
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetBeforeSetAfterLast("MMM");
			var c = new G.List<string>() { "MMM", "BBB", "PPP", "DDD" };
			var d = new G.List<string>() { "EEE", "DDD" };
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetCopyRange()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetCopyRange(..);
			var c = new G.List<string>(list);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(..^1);
			c = new G.List<string>(list).GetRange(0, list.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(1..);
			c = new G.List<string>(list).GetRange(1, list.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(1..^1);
			c = new G.List<string>(list).GetRange(1, list.Length - 2);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(1..5);
			c = new G.List<string>(list).GetRange(1, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(^5..^1);
			c = new G.List<string>(list).GetRange(list.Length - 5, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(^5..5);
			c = new G.List<string>(list).GetRange(list.Length - 5, 10 - list.Length);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetCopyRange(-1..5));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetCopyRange(^1..1));
			Assert.ThrowsException<ArgumentException>(() => b = a.GetCopyRange(1..1000));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetRange()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.GetRange(..);
			var c = new G.List<string>(list);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(..^1);
			c = new G.List<string>(list).GetRange(0, list.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(1..);
			c = new G.List<string>(list).GetRange(1, list.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(1..^1);
			c = new G.List<string>(list).GetRange(1, list.Length - 2);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(1..5);
			c = new G.List<string>(list).GetRange(1, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(^5..^1);
			c = new G.List<string>(list).GetRange(list.Length - 5, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(^5..5);
			c = new G.List<string>(list).GetRange(list.Length - 5, 10 - list.Length);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(-1..5));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(^1..1));
			Assert.ThrowsException<ArgumentException>(() => b = a.GetRange(1..1000));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestIndexOf()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.IndexOf("MMM");
			Assert.AreEqual(b, 0);
			b = a.IndexOf("BBB", 2);
			Assert.AreEqual(b, -1);
			b = a.IndexOf("BBB", 1, 2);
			Assert.AreEqual(b, 1);
			b = a.IndexOf(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 2);
			b = a.IndexOf(new List<string>("PPP", "DDD", "NNN"));
			Assert.AreEqual(b, -1);
			b = a.IndexOf(new[] { "MMM", "EEE" }, 4);
			Assert.AreEqual(b, 4);
			b = a.IndexOf(new[] { "MMM", "EEE" }, 0, 4);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOf((G.IEnumerable<string>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		try
		{
			var a = new List<string>(list);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.IndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 1);
			b = a.IndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, 0);
			b = a.IndexOfAnyExcluding(a);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestInsert()
	{
		try
		{
			var a = new List<string>(list).Insert(3, defaultString);
			var b = new G.List<string>(list);
			b.Insert(3, defaultString);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).Insert(4, defaultCollection);
			b = new G.List<string>(list);
			b.InsertRange(4, defaultCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).Insert(2, defaultCollection.AsSpan(2, 3));
			b = new G.List<string>(list);
			b.InsertRange(2, defaultCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a = new List<string>(list).Insert(1000, defaultString));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new List<string>(list).Insert(-1, defaultCollection));
			Assert.ThrowsException<ArgumentNullException>(() => new List<string>(list).Insert(1, null));
			Assert.ThrowsException<ArgumentNullException>(() => new List<string>(list).Insert(5, (G.IEnumerable<string>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.LastIndexOf("MMM");
			Assert.AreEqual(b, 4);
			b = a.LastIndexOf("BBB", 2);
			Assert.AreEqual(b, 1);
			b = a.LastIndexOf("BBB", 3, 2);
			Assert.AreEqual(b, -1);
			b = a.LastIndexOf(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 2);
			b = a.LastIndexOf(new List<string>("PPP", "DDD", "NNN"));
			Assert.AreEqual(b, -1);
			b = a.LastIndexOf(new[] { "MMM", "EEE" }, 3);
			Assert.AreEqual(b, -1);
			b = a.LastIndexOf(new[] { "MMM", "EEE" }, 5, 4);
			Assert.AreEqual(b, 4);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOf((G.IEnumerable<string>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		try
		{
			var a = new List<string>(list);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.LastIndexOfAnyExcluding(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 5);
			b = a.LastIndexOfAnyExcluding(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, 6);
			b = a.LastIndexOfAnyExcluding(a);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<string>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestNSort()
	{
		try
		{
			var c = new G.List<string>(new string[256].ToArray(x => new byte[random.Next(1, 17)].ToString(y => (char)random.Next(65536))));
			var a = new List<string>(c);
			var b = new List<string>(a).NSort(x => x[^1]);
			c = E.ToList(E.OrderBy(c, x => x[^1]));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemove()
	{
		try
		{
			var a = new List<string>(list);
			var b = new List<string>(a).Remove(6);
			var c = new G.List<string>(list);
			c.RemoveRange(6, 1);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
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
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.RemoveAll(x => x.Length != 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = c.RemoveAll(x => x.Length != 3);
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.RemoveAll(x => !x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = c.RemoveAll(x => !E.All(x, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		try
		{
			var a = new List<string>(list);
			for (int i = 0; i < 1000; i++)
			{
				int index = random.Next(a.Length);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemoveValue()
	{
		try
		{
			var a = new Chain(15, 10).ToList();
			for (int i = 0; i < 1000; i++)
			{
				int value = a[random.Next(a.Length)];
				var b = new List<int>(a);
				b.RemoveValue(value);
				var c = new G.List<int>(a);
				c.Remove(value);
				foreach (var x in a)
					Assert.AreEqual(b.Contains(x), x != value);
				Assert.IsTrue(b.Equals(c));
				Assert.IsTrue(E.SequenceEqual(c, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		try
		{
			var a = new List<string>(list).Replace(defaultCollection);
			var b = new G.List<string>(list);
			b.Clear();
			b.AddRange(defaultCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).AddRange(defaultCollection.AsSpan(2, 3));
			b = new G.List<string>(list);
			b.AddRange(defaultCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReplace2()
	{
		try
		{
			for (int i = 0; i < 1000; i++)
			{
				var arr = new char[1000];
				for (int j = 0; j < 1000; j++)
					arr[j] = (char)random.Next(33, 127);
				string s = new(arr);
				String a = s;
				char oldItem = (char)random.Next(33, 127);
				char newItem = (char)random.Next(33, 127);
				var b = a.Replace(oldItem, newItem);
				var c = s.Replace(oldItem, newItem);
				Assert.IsTrue(a.Equals(s));
				Assert.IsTrue(E.SequenceEqual(s, a));
				Assert.IsTrue(b.Equals(c));
				Assert.IsTrue(E.SequenceEqual(c, b));
			}
			for (int i = 0; i < 100; i++)
			{
				var arr = new char[1000];
				for (int j = 0; j < 1000; j++)
					arr[j] = (char)random.Next(33, 127);
				string s = new(arr);
				String a = s;
				var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
				var newArray = new char[random.Next(10)];
				for (int j = 0; j < newArray.Length; j++)
					newArray[j] = (char)random.Next(33, 127);
				string newCollection = new(newArray);
				var b = a.Replace(oldCollection, newArray);
				var c = s.Replace(oldCollection, newCollection);
				Assert.IsTrue(a.Equals(s));
				Assert.IsTrue(E.SequenceEqual(s, a));
				Assert.IsTrue(b.Equals(c));
				Assert.IsTrue(E.SequenceEqual(c, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReplaceInPlace()
	{
		try
		{
			for (int i = 0; i < 1000; i++)
			{
				var arr = new char[1000];
				for (int j = 0; j < 1000; j++)
					arr[j] = (char)random.Next(33, 127);
				string s = new(arr);
				String a = s;
				char oldItem = (char)random.Next(33, 127);
				char newItem = (char)random.Next(33, 127);
				var b = a.ReplaceInPlace(oldItem, newItem);
				var c = s.Replace(oldItem, newItem);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				Assert.IsTrue(b.Equals(c));
				Assert.IsTrue(E.SequenceEqual(c, b));
			}
			for (int i = 0; i < 100; i++)
			{
				var arr = new char[1000];
				for (int j = 0; j < 1000; j++)
					arr[j] = (char)random.Next(33, 127);
				string s = new(arr);
				String a = s;
				var oldCollection = a.GetRange(random.Next(991), random.Next(1, 10)).ToString();
				var newArray = new char[random.Next(10)];
				for (int j = 0; j < newArray.Length; j++)
					newArray[j] = (char)random.Next(33, 127);
				string newCollection = new(newArray);
				var b = a.ReplaceInPlace(oldCollection, newArray);
				var c = s.Replace(oldCollection, newCollection);
				Assert.IsTrue(a.Equals(b));
				Assert.IsTrue(E.SequenceEqual(b, a));
				Assert.IsTrue(b.Equals(c));
				Assert.IsTrue(E.SequenceEqual(c, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		try
		{
			var a = new List<string>(list).ReplaceRange(2, 3, defaultCollection);
			var b = new G.List<string>(list);
			b.RemoveRange(2, 3);
			b.InsertRange(2, defaultCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).Insert(2, defaultCollection.AsSpan(2, 3));
			b = new G.List<string>(list);
			b.InsertRange(2, defaultCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentException>(() => a = new List<string>(list).ReplaceRange(1, 1000, defaultString));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new List<string>(list).ReplaceRange(-1, 3, defaultCollection));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new List<string>(list).ReplaceRange(4, -2, defaultCollection));
			Assert.ThrowsException<ArgumentNullException>(() => new List<string>(list).ReplaceRange(4, 1, null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReverse()
	{
		try
		{
			var a = new List<string>(list).Reverse();
			var b = new G.List<string>(list);
			b.Reverse();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).AddRange(defaultCollection.AsSpan(2, 3)).Reverse();
			b = new G.List<string>(list);
			b.AddRange(defaultCollection.Skip(2).Take(3));
			b.Reverse();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).AddRange(defaultCollection.AsSpan(2, 3)).Reverse(2, 4);
			b = new G.List<string>(list);
			b.AddRange(defaultCollection.Skip(2).Take(3));
			b.Reverse(2, 4);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSetAll()
	{
		try
		{
			var a = new List<string>(list).SetAll(defaultString);
			var b = new G.List<string>(list);
			for (int i = 0; i < b.Count; i++)
				b[i] = defaultString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).SetAll(defaultString, 3);
			b = new G.List<string>(list);
			for (int i = 3; i < b.Count; i++)
				b[i] = defaultString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).SetAll(defaultString, 2, 4);
			b = new G.List<string>(list);
			for (int i = 2; i < 6; i++)
				b[i] = defaultString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).SetAll(defaultString, ^5);
			b = new G.List<string>(list);
			for (int i = b.Count - 5; i < b.Count; i++)
				b[i] = defaultString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new List<string>(list).SetAll(defaultString, ^6..4);
			b = new G.List<string>(list);
			for (int i = b.Count - 6; i < 4; i++)
				b[i] = defaultString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		try
		{
			var hs = defaultCollection.ToHashSet();
			var a = new List<string>(list).SetRange(2, hs);
			var b = new G.List<string>(list);
			for (int i = 0; i < hs.Length; i++)
				b[i + 2] = hs[i];
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentException>(() => a = new List<string>(list).SetRange(5, hs));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new List<string>(list).SetRange(-1, hs));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new List<string>(list).SetRange(1000, hs));
			Assert.ThrowsException<ArgumentNullException>(() => new List<string>(list).SetRange(4, null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSkip()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.Skip(2);
			var c = E.Skip(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.Skip(0);
			c = E.Skip(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.Skip(1000);
			c = E.Skip(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.Skip(-4);
			c = E.Skip(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSkipLast()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.SkipLast(2);
			var c = E.SkipLast(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.SkipLast(0);
			c = E.SkipLast(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.SkipLast(1000);
			c = E.SkipLast(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.SkipLast(-4);
			c = E.SkipLast(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.SkipWhile(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = E.ToList(E.SkipWhile(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.SkipWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') || index < 1);
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => E.All(x, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSort()
	{
		try
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestStartsWith()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.StartsWith("MMM");
			Assert.IsTrue(b);
			b = a.StartsWith(new List<string>("MMM", "BBB", "PPP"));
			Assert.IsTrue(b);
			b = a.StartsWith(new List<string>("MMM", "BBB", "XXX"));
			Assert.IsTrue(!b);
			Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<string>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTake()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.Take(2);
			var c = E.Take(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.Take(0);
			c = E.Take(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.Take(1000);
			c = E.Take(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.Take(-4);
			c = E.Take(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTakeLast()
	{
		try
		{
			var a = new List<string>(list);
			var b = a.TakeLast(2);
			var c = E.TakeLast(new G.List<string>(list), 2);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.TakeLast(0);
			c = E.TakeLast(new G.List<string>(list), 0);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.TakeLast(1000);
			c = E.TakeLast(new G.List<string>(list), 1000);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new List<string>(list);
			b = a.TakeLast(-4);
			c = E.TakeLast(new G.List<string>(list), -4);
			Assert.IsTrue(a.Equals(list));
			Assert.IsTrue(E.SequenceEqual(list, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.TakeWhile(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			var d = E.ToList(E.TakeWhile(c, x => x.Length == 3));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			b = a.TakeWhile((x, index) => x.All(y => y is >= 'A' and <= 'Z') && index < 10);
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			d = E.ToList(E.TakeWhile(E.Take(c, 10), x => E.All(x, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestToArray()
	{
		try
		{
			int length, capacity;
			List<string> a;
			G.List<string> b;
			string[] array;
			string[] array2;
			string elem;
			for (int i = 0; i < 1000; i++)
			{
				length = random.Next(51);
				capacity = length + random.Next(151);
				a = new(capacity);
				b = new(capacity);
				for (int j = 0; j < length; j++)
				{
					a.Add(elem = new((char)random.Next(33, 127), random.Next(10)));
					b.Add(elem);
				}
				array = a.ToArray();
				array2 = b.ToArray();
				Assert.IsTrue(RedStarLinq.Equals(a, b));
				Assert.IsTrue(E.SequenceEqual(a, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTrimExcess()
	{
		try
		{
			int length, capacity;
			List<string> a;
			G.List<string> b;
			string elem;
			for (int i = 0; i < 1000; i++)
			{
				length = random.Next(51);
				capacity = length + random.Next(9951);
				a = new(capacity);
				b = new(capacity);
				for (int j = 0; j < length; j++)
				{
					a.Add(elem = new((char)random.Next(33, 127), random.Next(10)));
					b.Add(elem);
				}
				a.TrimExcess();
				Assert.IsTrue(RedStarLinq.Equals(a, b));
				Assert.IsTrue(E.SequenceEqual(a, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTrueForAll()
	{
		try
		{
			var a = new List<string>(list).Insert(3, new List<string>("$", "###"));
			var b = a.TrueForAll(x => x.Length == 3);
			var c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTuples()
	{
		try
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}

[TestClass]
public class NListTests
{
	[TestMethod]
	public void TestAdd()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Add(defaultNString);
			var b = new G.List<(char, char, char)>(nList) { defaultNString };
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestAddRange()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).AddRange(defaultNCollection);
			var b = new G.List<(char, char, char)>(nList);
			b.AddRange(defaultNCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).AddRange(defaultNCollection.AsSpan(2, 3));
			b = new G.List<(char, char, char)>(nList);
			b.AddRange(defaultNCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestAppend()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.Append(defaultNString);
			var c = E.Append(new G.List<(char, char, char)>(nList), defaultNString);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.BreakFilter(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0', out var c);
			var d = new G.List<(char, char, char)>(nList);
			d.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var e = E.Where(d, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
			var f = E.Where(d, x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, b));
			Assert.IsTrue(c.Equals(f));
			Assert.IsTrue(E.SequenceEqual(f, c));
			b = a.BreakFilter(x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'), out c);
			e = E.Where(d, x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			f = E.Where(d, x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, b));
			Assert.IsTrue(c.Equals(f));
			Assert.IsTrue(E.SequenceEqual(f, c));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestBreakFilterInPlace()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.BreakFilterInPlace(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0', out var c);
			var d = new G.List<(char, char, char)>(nList);
			d.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var e = E.ToList(E.Where(d, x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0'));
			d = E.ToList(E.Where(d, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(c.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, c));
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.BreakFilterInPlace(x => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'), out c);
			d = new G.List<(char, char, char)>(nList);
			d.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			e = E.ToList(E.Where(d, x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
			d = E.ToList(E.Where(d, x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.IsTrue(c.Equals(e));
			Assert.IsTrue(E.SequenceEqual(e, c));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestClear()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			a.Clear(2, 4);
			var b = new G.List<(char, char, char)>(nList);
			for (int i = 0; i < 4; i++)
				b[2 + i] = default!;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.Concat(new(defaultNCollection));
			var c = E.Concat(new G.List<(char, char, char)>(nList), defaultNCollection);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestContains()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestContainsAny()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.ContainsAny(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
			Assert.IsTrue(b);
			b = a.ContainsAny(new NList<(char, char, char)>(('L', 'L', 'L'), ('M', 'M', 'M'), ('N', 'N', 'N')));
			Assert.IsTrue(b);
			b = a.ContainsAny(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.ContainsAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
			Assert.IsTrue(b);
			b = a.ContainsAnyExcluding(a);
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestConvert()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestCopy()
	{
		try
		{
			int length, capacity;
			NList<(char, char, char)> a;
			NList<(char, char, char)> b;
			int index;
			for (int i = 0; i < 1000; i++)
			{
				length = random.Next(1, 51);
				capacity = length + random.Next(9951);
				a = new(capacity);
				for (int j = 0; j < length; j++)
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestCopyTo()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestEndsWith()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.EndsWith(('D', 'D', 'D'));
			Assert.IsTrue(b);
			b = a.EndsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D')));
			Assert.IsTrue(b);
			b = a.EndsWith(new NList<(char, char, char)>(('P', 'P', 'P'), ('E', 'E', 'E'), ('D', 'D', 'D')));
			Assert.IsTrue(!b);
			b = a.EndsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('E', 'E', 'E'), ('N', 'N', 'N')));
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestEquals()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.Contains(('M', 'M', 'M'));
			Assert.IsTrue(b);
			b = a.Equals(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')), 2);
			Assert.IsTrue(b);
			b = a.Equals(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('N', 'N', 'N')), 2);
			Assert.IsTrue(!b);
			b = a.Equals(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')), 3);
			Assert.IsTrue(!b);
			b = a.Equals(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')), 2, true);
			Assert.IsTrue(!b);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFilter()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.Filter(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.FilterInPlace(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			c = E.ToList(E.Where(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.FilterInPlace((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index >= 1);
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			c = E.ToList(E.Where(c, (x, index) => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z') && index >= 1));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFind()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.Find(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = c.Find(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.Find(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = c.Find(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindAll()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.FindAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = c.FindAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.FindAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = c.FindAll(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindIndex()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.FindIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = c.FindIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.FindIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = c.FindIndex(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindLast()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.FindLast(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = c.FindLast(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.FindLast(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = c.FindLast(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.FindLastIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = c.FindLastIndex(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.FindLastIndex(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = c.FindLastIndex(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetAfter()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetAfter(('D', 'D', 'D'));
			var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D') };
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfter(new());
			c = new();
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfter(new(('D', 'D', 'D'), ('M', 'M', 'M')));
			c = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetAfterLast(('M', 'M', 'M'));
			var c = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfterLast(new());
			c = new();
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetAfterLast(new(('D', 'D', 'D'), ('M', 'M', 'M')));
			c = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBefore()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetBefore(('D', 'D', 'D'));
			var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBefore(new());
			c = new(nList);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBefore(new(('D', 'D', 'D'), ('M', 'M', 'M')));
			c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetBeforeLast(('M', 'M', 'M'));
			var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D') };
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBeforeLast(new());
			c = new(nList);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetBeforeLast(new(('D', 'D', 'D'), ('M', 'M', 'M')));
			c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetBeforeSetAfter(('D', 'D', 'D'));
			var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P') };
			var d = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D') };
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetBeforeSetAfterLast(('M', 'M', 'M'));
			var c = new G.List<(char, char, char)>() { ('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D') };
			var d = new G.List<(char, char, char)>() { ('E', 'E', 'E'), ('D', 'D', 'D') };
			Assert.IsTrue(a.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetCopyRange()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetCopyRange(..);
			var c = new G.List<(char, char, char)>(nList);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(..^1);
			c = new G.List<(char, char, char)>(nList).GetRange(0, nList.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(1..);
			c = new G.List<(char, char, char)>(nList).GetRange(1, nList.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(1..^1);
			c = new G.List<(char, char, char)>(nList).GetRange(1, nList.Length - 2);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(1..5);
			c = new G.List<(char, char, char)>(nList).GetRange(1, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(^5..^1);
			c = new G.List<(char, char, char)>(nList).GetRange(nList.Length - 5, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetCopyRange(^5..5);
			c = new G.List<(char, char, char)>(nList).GetRange(nList.Length - 5, 10 - nList.Length);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetCopyRange(-1..5));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetCopyRange(^1..1));
			Assert.ThrowsException<ArgumentException>(() => b = a.GetCopyRange(1..1000));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestGetRange()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.GetRange(..);
			var c = new G.List<(char, char, char)>(nList);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(..^1);
			c = new G.List<(char, char, char)>(nList).GetRange(0, nList.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(1..);
			c = new G.List<(char, char, char)>(nList).GetRange(1, nList.Length - 1);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(1..^1);
			c = new G.List<(char, char, char)>(nList).GetRange(1, nList.Length - 2);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(1..5);
			c = new G.List<(char, char, char)>(nList).GetRange(1, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(^5..^1);
			c = new G.List<(char, char, char)>(nList).GetRange(nList.Length - 5, 4);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			b = a.GetRange(^5..5);
			c = new G.List<(char, char, char)>(nList).GetRange(nList.Length - 5, 10 - nList.Length);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(-1..5));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(^1..1));
			Assert.ThrowsException<ArgumentException>(() => b = a.GetRange(1..1000));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestIndexOf()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.IndexOfAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
			Assert.AreEqual(b, 1);
			b = a.IndexOfAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
			Assert.AreEqual(b, 0);
			b = a.IndexOfAnyExcluding(a);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<(char, char, char)>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestInsert()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, defaultNString);
			var b = new G.List<(char, char, char)>(nList);
			b.Insert(3, defaultNString);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).Insert(4, defaultNCollection);
			b = new G.List<(char, char, char)>(nList);
			b.InsertRange(4, defaultNCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).Insert(2, defaultNCollection.AsSpan(2, 3));
			b = new G.List<(char, char, char)>(nList);
			b.InsertRange(2, defaultNCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => a = new NList<(char, char, char)>(nList).Insert(1000, defaultNString));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new NList<(char, char, char)>(nList).Insert(-1, defaultNCollection));
			Assert.ThrowsException<ArgumentNullException>(() => new NList<(char, char, char)>(nList).Insert(5, (G.IEnumerable<(char, char, char)>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.LastIndexOfAnyExcluding(new NList<(char, char, char)>(('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M')));
			Assert.AreEqual(b, 5);
			b = a.LastIndexOfAnyExcluding(new NList<(char, char, char)>(('X', 'X', 'X'), ('Y', 'Y', 'Y'), ('Z', 'Z', 'Z')));
			Assert.AreEqual(b, 6);
			b = a.LastIndexOfAnyExcluding(a);
			Assert.AreEqual(b, -1);
			Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<(char, char, char)>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemove()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
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
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.RemoveAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = c.RemoveAll(x => x.Item1 == '\0' || x.Item2 == '\0' || x.Item3 == '\0');
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.RemoveAll(x => !new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = c.RemoveAll(x => !E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.AreEqual(b, d);
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			for (int i = 0; i < 1000; i++)
			{
				int index = random.Next(a.Length);
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestRemoveValue()
	{
		try
		{
			var a = new Chain(15, 10).ToList();
			for (int i = 0; i < 1000; i++)
			{
				int value = a[random.Next(a.Length)];
				var b = new NList<int>(a);
				b.RemoveValue(value);
				var c = new G.List<int>(a);
				c.Remove(value);
				foreach (var x in a)
					Assert.AreEqual(b.Contains(x), x != value);
				Assert.IsTrue(b.Equals(c));
				Assert.IsTrue(E.SequenceEqual(c, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Replace(defaultNCollection);
			var b = new G.List<(char, char, char)>(nList);
			b.Clear();
			b.AddRange(defaultNCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).AddRange(defaultNCollection.AsSpan(2, 3));
			b = new G.List<(char, char, char)>(nList);
			b.AddRange(defaultNCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}


	[TestMethod]
	public void TestReplaceRange()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).ReplaceRange(2, 3, defaultNCollection);
			var b = new G.List<(char, char, char)>(nList);
			b.RemoveRange(2, 3);
			b.InsertRange(2, defaultNCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).Insert(2, defaultNCollection.AsSpan(2, 3));
			b = new G.List<(char, char, char)>(nList);
			b.InsertRange(2, defaultNCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentException>(() => a = new NList<(char, char, char)>(nList).ReplaceRange(1, 1000, defaultNString));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new NList<(char, char, char)>(nList).ReplaceRange(-1, 3, defaultNCollection));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new NList<(char, char, char)>(nList).ReplaceRange(4, -2, defaultNCollection));
			Assert.ThrowsException<ArgumentNullException>(() => new NList<(char, char, char)>(nList).ReplaceRange(4, 1, null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestReverse()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Reverse();
			var b = new G.List<(char, char, char)>(nList);
			b.Reverse();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).AddRange(defaultNCollection.AsSpan(2, 3)).Reverse();
			b = new G.List<(char, char, char)>(nList);
			b.AddRange(defaultNCollection.Skip(2).Take(3));
			b.Reverse();
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).AddRange(defaultNCollection.AsSpan(2, 3)).Reverse(2, 4);
			b = new G.List<(char, char, char)>(nList);
			b.AddRange(defaultNCollection.Skip(2).Take(3));
			b.Reverse(2, 4);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSetAll()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).SetAll(defaultNString);
			var b = new G.List<(char, char, char)>(nList);
			for (int i = 0; i < b.Count; i++)
				b[i] = defaultNString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).SetAll(defaultNString, 3);
			b = new G.List<(char, char, char)>(nList);
			for (int i = 3; i < b.Count; i++)
				b[i] = defaultNString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).SetAll(defaultNString, 2, 4);
			b = new G.List<(char, char, char)>(nList);
			for (int i = 2; i < 6; i++)
				b[i] = defaultNString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).SetAll(defaultNString, ^5);
			b = new G.List<(char, char, char)>(nList);
			for (int i = b.Count - 5; i < b.Count; i++)
				b[i] = defaultNString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			a = new NList<(char, char, char)>(nList).SetAll(defaultNString, ^6..4);
			b = new G.List<(char, char, char)>(nList);
			for (int i = b.Count - 6; i < 4; i++)
				b[i] = defaultNString;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSetRange()
	{
		try
		{
			var hs = defaultNCollection.ToHashSet();
			var a = new NList<(char, char, char)>(nList).SetRange(2, hs);
			var b = new G.List<(char, char, char)>(nList);
			for (int i = 0; i < hs.Length; i++)
				b[i + 2] = hs[i];
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			Assert.ThrowsException<ArgumentException>(() => a = new NList<(char, char, char)>(nList).SetRange(5, hs));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new NList<(char, char, char)>(nList).SetRange(-1, hs));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new NList<(char, char, char)>(nList).SetRange(1000, hs));
			Assert.ThrowsException<ArgumentNullException>(() => new NList<(char, char, char)>(nList).SetRange(4, null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSkip()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.Skip(2);
			var c = E.Skip(new G.List<(char, char, char)>(nList), 2);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.Skip(0);
			c = E.Skip(new G.List<(char, char, char)>(nList), 0);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.Skip(1000);
			c = E.Skip(new G.List<(char, char, char)>(nList), 1000);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.Skip(-4);
			c = E.Skip(new G.List<(char, char, char)>(nList), -4);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSkipLast()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.SkipLast(2);
			var c = E.SkipLast(new G.List<(char, char, char)>(nList), 2);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.SkipLast(0);
			c = E.SkipLast(new G.List<(char, char, char)>(nList), 0);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.SkipLast(1000);
			c = E.SkipLast(new G.List<(char, char, char)>(nList), 1000);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.SkipLast(-4);
			c = E.SkipLast(new G.List<(char, char, char)>(nList), -4);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.SkipWhile(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = E.ToList(E.SkipWhile(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.SkipWhile((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') || index < 1);
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = E.ToList(E.SkipWhile(E.Skip(c, 1), x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestSort()
	{
		try
		{
			var c = new G.List<(char, char, char)>(new byte[256].ToArray(x => ((char)random.Next(65536), (char)random.Next(65536), (char)random.Next(65536))));
			var a = new NList<(char, char, char)>(c);
			var b = new NList<(char, char, char)>(a).Sort(x => x.Item3);
			c = E.ToList(E.OrderBy(c, x => x.Item3));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestStartsWith()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.StartsWith(('M', 'M', 'M'));
			Assert.IsTrue(b);
			b = a.StartsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P')));
			Assert.IsTrue(b);
			b = a.StartsWith(new NList<(char, char, char)>(('M', 'M', 'M'), ('B', 'B', 'B'), ('X', 'X', 'X')));
			Assert.IsTrue(!b);
			Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<(char, char, char)>)null!));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTake()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.Take(2);
			var c = E.Take(new G.List<(char, char, char)>(nList), 2);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.Take(0);
			c = E.Take(new G.List<(char, char, char)>(nList), 0);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.Take(1000);
			c = E.Take(new G.List<(char, char, char)>(nList), 1000);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.Take(-4);
			c = E.Take(new G.List<(char, char, char)>(nList), -4);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTakeLast()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList);
			var b = a.TakeLast(2);
			var c = E.TakeLast(new G.List<(char, char, char)>(nList), 2);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.TakeLast(0);
			c = E.TakeLast(new G.List<(char, char, char)>(nList), 0);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.TakeLast(1000);
			c = E.TakeLast(new G.List<(char, char, char)>(nList), 1000);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
			a = new NList<(char, char, char)>(nList);
			b = a.TakeLast(-4);
			c = E.TakeLast(new G.List<(char, char, char)>(nList), -4);
			Assert.IsTrue(a.Equals(nList));
			Assert.IsTrue(E.SequenceEqual(nList, a));
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.TakeWhile(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			var d = E.ToList(E.TakeWhile(c, x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0'));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
			a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			b = a.TakeWhile((x, index) => new[] { x.Item1, x.Item2, x.Item3 }.All(y => y is >= 'A' and <= 'Z') && index < 10);
			c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
			d = E.ToList(E.TakeWhile(E.Take(c, 10), x => E.All(new[] { x.Item1, x.Item2, x.Item3 }, y => y is >= 'A' and <= 'Z')));
			Assert.IsTrue(a.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, a));
			Assert.IsTrue(b.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, b));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestToArray()
	{
		try
		{
			int length, capacity;
			NList<(char, char, char)> a;
			G.List<(char, char, char)> b;
			(char, char, char)[] array;
			(char, char, char)[] array2;
			(char, char, char) elem;
			for (int i = 0; i < 1000; i++)
			{
				length = random.Next(51);
				capacity = length + random.Next(151);
				a = new(capacity);
				b = new(capacity);
				for (int j = 0; j < length; j++)
				{
					a.Add(elem = ((char)random.Next(33, 127), (char)random.Next(33, 127), (char)random.Next(33, 127)));
					b.Add(elem);
				}
				array = a.ToArray();
				array2 = b.ToArray();
				Assert.IsTrue(RedStarLinq.Equals(a, b));
				Assert.IsTrue(E.SequenceEqual(a, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTrimExcess()
	{
		try
		{
			int length, capacity;
			NList<(char, char, char)> a;
			G.List<(char, char, char)> b;
			(char, char, char) elem;
			for (int i = 0; i < 1000; i++)
			{
				length = random.Next(51);
				capacity = length + random.Next(9951);
				a = new(capacity);
				b = new(capacity);
				for (int j = 0; j < length; j++)
				{
					a.Add(elem = ((char)random.Next(33, 127), (char)random.Next(33, 127), (char)random.Next(33, 127)));
					b.Add(elem);
				}
				a.TrimExcess();
				Assert.IsTrue(RedStarLinq.Equals(a, b));
				Assert.IsTrue(E.SequenceEqual(a, b));
			}
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTrueForAll()
	{
		try
		{
			var a = new NList<(char, char, char)>(nList).Insert(3, new NList<(char, char, char)>(('$', '\0', '\0'), ('#', '#', '#')));
			var b = a.TrueForAll(x => x.Item1 != '\0' && x.Item2 != '\0' && x.Item3 != '\0');
			var c = new G.List<(char, char, char)>(nList);
			c.InsertRange(3, new G.List<(char, char, char)>() { ('$', '\0', '\0'), ('#', '#', '#') });
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
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}

	[TestMethod]
	public void TestTuples()
	{
		//try
		//{
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
		//}
		//catch (Exception ex)
		//{
		//	Assert.Fail(ex.ToString());
		//}
	}
}

[TestClass]
public class SumListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			var arr = RedStarLinq.FillArray(16, _ => random.Next(1, 16));
			SumList sl = new(arr);
			G.List<int> gl = new(arr);
			var bytes = new byte[16];
			var updateActions = new[] { (int key) =>
			{
				int newValue = random.Next(16);
				sl.Update(key, newValue);
				if (newValue <= 0)
					gl.RemoveAt(key);
				else
					gl[key] = newValue;
			}, key =>
			{
				sl.Increase(key);
				gl[key]++;
			}, key =>
			{
				if (sl[key] == 1)
					gl.RemoveAt(key);
				else
					gl[key]--;
				sl.Decrease(key);
			} };
			var actions = new[] { () =>
			{
				int n = random.Next(1, 16);
				if (random.Next(2) == 0)
				{
					sl.Add(n);
					gl.Add(n);
				}
				else
				{
					int index = random.Next(sl.Length + 1);
					sl.Insert(index, n);
					gl.Insert(index, n);
				}
				Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			}, () =>
			{
				if (sl.Length == 0) return;
				int index = random.Next(sl.Length);
				gl.RemoveAt(index);
				sl.RemoveAt(index);
				Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			}, () =>
			{
				if (sl.Length == 0) return;
				int index = random.Next(sl.Length);
				updateActions.Random(random)(index);
				Assert.IsTrue(RedStarLinq.Equals(sl, gl));
				Assert.AreEqual(sl.GetLeftValuesSum(index, out int value), E.Sum(E.Take(gl, index)));
			}, () =>
			{
				random.NextBytes(bytes);
				int index = sl.IndexOfNotGreaterSum(CreateVar((long)(new mpz_t(bytes, 1) % (sl.ValuesSum + 1)), out var sum));
				Assert.IsTrue((index == gl.Count && sum == E.Sum(gl)) || (CreateVar(E.Sum(E.Take(gl, index)), out var sum2) <= sum && (gl[index] == 0 || sum2 + gl[index] > sum)));
			}, () =>
			{
				if (sl.Length == 0) return;
				int index = random.Next(sl.Length);
				Assert.AreEqual(sl[index], gl[index]);
			} };
			for (int i = 0; i < 1000; i++)
				actions.Random(random)();
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}

[TestClass]
public class BigSumListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		try
		{
			var bytes = new byte[20];
			var bytes2 = new byte[48];
			var arr = RedStarLinq.FillArray(16, _ =>
			{
				random.NextBytes(bytes);
				return new mpz_t(bytes, 1);
			});
			BigSumList sl = new(arr);
			G.List<mpz_t> gl = new(arr);
			var updateActions = new[] { (int key) =>
			{
				random.NextBytes(bytes);
				mpz_t newValue = new(bytes, 1);
				sl.Update(key, newValue);
				if (newValue <= 0)
					gl.RemoveAt(key);
				else
					gl[key] = newValue;
			}, key =>
			{
				sl.Increase(key);
				gl[key]++;
			}, key =>
			{
				if (sl[key] == 1)
					gl.RemoveAt(key);
				else
					gl[key]--;
				sl.Decrease(key);
			} };
			var actions = new[] { () =>
			{
				random.NextBytes(bytes);
				mpz_t n = new(bytes, 1);
				if (random.Next(2) == 0)
				{
					sl.Add(n);
					gl.Add(n);
				}
				else
				{
					int index = random.Next(sl.Length + 1);
					sl.Insert(index, n);
					gl.Insert(index, n);
				}
				Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			}, () =>
			{
				if (sl.Length == 0) return;
				int index = random.Next(sl.Length);
				gl.RemoveAt(index);
				sl.RemoveAt(index);
				Assert.IsTrue(RedStarLinq.Equals(sl, gl));
			}, () =>
			{
				if (sl.Length == 0) return;
				int index = random.Next(sl.Length);
				updateActions.Random(random)(index);
				Assert.IsTrue(RedStarLinq.Equals(sl, gl));
				Assert.AreEqual(sl.GetLeftValuesSum(index, out mpz_t value), index == 0 ? 0 : E.Aggregate(E.Take(gl, index), (x, y) => x + y));
			}, () =>
			{
				random.NextBytes(bytes2);
				int index = sl.IndexOfNotGreaterSum(CreateVar(new mpz_t(bytes2, 1) % (sl.ValuesSum + 1), out var sum));
				Assert.IsTrue((index == 0 && (gl.Count == 0 || sum < gl[0])) || (index == gl.Count && sum == E.Aggregate(gl, (x, y) => x + y)) || (CreateVar(E.Aggregate(E.Take(gl, index + 1), (x, y) => x + y), out var sum2) > sum && (gl[index] == 0 || sum2 + gl[index] > sum)));
			}, () =>
			{
				if (sl.Length == 0) return;
				int index = random.Next(sl.Length);
				Assert.AreEqual(sl[index], gl[index]);
			} };
			for (int i = 0; i < 1000; i++)
				actions.Random(random)();
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
