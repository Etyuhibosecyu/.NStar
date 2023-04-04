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
			b = a.Contains(new List<string>("PPP", "DDD", "NNN"), 2);
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
			b = a.Filter(x => x.All(y => y is >= 'A' and <= 'Z'));
			d = E.Where(c, x => E.All(x, y => y is >= 'A' and <= 'Z'));
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
			b = a.FilterInPlace(x => x.All(y => y is >= 'A' and <= 'Z'));
			c = new G.List<string>(list);
			c.InsertRange(3, new G.List<string>() { "$", "###" });
			c = E.ToList(E.Where(c, x => E.All(x, y => y is >= 'A' and <= 'Z')));
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
