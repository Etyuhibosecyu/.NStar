
using System.Collections.Immutable;

namespace Corlib.NStar.Tests;

[TestClass]
public class ListTests
{
	private readonly ImmutableArray<string> list = ImmutableArray.Create("MMM", "BBB", "PPP", "DDD", "MMM", "EEE", "DDD");

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
			var c = new List<string>(list).AddRange(defaultCollection.AsSpan(2, 3));
			var d = new G.List<string>(list);
			d.AddRange(defaultCollection.Skip(2).Take(3));
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
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
			b = a.Contains(new List<string>("PPP", "DDD", "MMM"));
			Assert.IsTrue(b);
			b = a.Contains(new List<string>("PPP", "DDD", "NNN"));
			Assert.IsTrue(!b);
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
			b = a.IndexOf(new List<string>("PPP", "DDD", "MMM"));
			Assert.AreEqual(b, 2);
			b = a.IndexOf(new List<string>("PPP", "DDD", "NNN"));
			Assert.AreEqual(b, -1);
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
			b = a.IndexOfAny(new List<string>("XXX", "YYY", "ZZZ"));
			Assert.AreEqual(b, -1);
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
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}
