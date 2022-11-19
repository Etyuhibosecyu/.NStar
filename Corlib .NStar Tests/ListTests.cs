
namespace Corlib.NStar.Tests;

[TestClass]
public class ListTests
{
	[TestMethod]
	public void TestList()
	{
		List<string> list = new("MMM", "BBB", "PPP", "DDD", "MMM", "EEE", "DDD");
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
		try
		{
			var a = new List<string>(list).AddRange(defaultCollection);
			var b = new G.List<string>(list);
			b.AddRange(defaultCollection);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
		try
		{
			var a = new List<string>(list).AddRange(defaultCollection.AsSpan(2, 3));
			var b = new G.List<string>(list);
			b.AddRange(defaultCollection.Skip(2).Take(3));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
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
}
