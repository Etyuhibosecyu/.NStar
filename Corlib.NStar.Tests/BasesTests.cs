using System.Collections.Immutable;

namespace Corlib.NStar.Tests;
public record class BaseIndexableTests<T, TCertain>(TCertain TestCollection, ImmutableArray<T> OriginalCollection, T DefaultString, G.IEnumerable<T> DefaultCollection) where TCertain : BaseIndexable<T, TCertain>, new()
{
	public void TestGetRange()
	{
		var b = TestCollection.GetRange(..);
		var c = new G.List<T>(OriginalCollection);
		Assert.IsTrue(TestCollection.Equals(OriginalCollection));
		Assert.IsTrue(E.SequenceEqual(OriginalCollection, TestCollection));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(..^1);
		c = new G.List<T>(OriginalCollection).GetRange(0, OriginalCollection.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(1..);
		c = new G.List<T>(OriginalCollection).GetRange(1, OriginalCollection.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(1..^1);
		c = new G.List<T>(OriginalCollection).GetRange(1, OriginalCollection.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(1..5);
		c = new G.List<T>(OriginalCollection).GetRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(^5..);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(^5..^1);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetRange(^5..5);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 10 - OriginalCollection.Length);
		Assert.IsTrue(TestCollection.Equals(OriginalCollection));
		Assert.IsTrue(E.SequenceEqual(OriginalCollection, TestCollection));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.GetRange(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.GetRange(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = TestCollection.GetRange(1..1000));
	}

	public void TestGetSlice()
	{
		var b = TestCollection.GetSlice(..);
		var c = new G.List<T>(OriginalCollection);
		Assert.IsTrue(TestCollection.Equals(OriginalCollection));
		Assert.IsTrue(E.SequenceEqual(OriginalCollection, TestCollection));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(1);
		c = new G.List<T>(OriginalCollection).GetRange(1, OriginalCollection.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(1, 4);
		c = new G.List<T>(OriginalCollection).GetRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(^5);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(..^1);
		c = new G.List<T>(OriginalCollection).GetRange(0, OriginalCollection.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(1..);
		c = new G.List<T>(OriginalCollection).GetRange(1, OriginalCollection.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(1..^1);
		c = new G.List<T>(OriginalCollection).GetRange(1, OriginalCollection.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(1..5);
		c = new G.List<T>(OriginalCollection).GetRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(^5..);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(^5..^1);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = TestCollection.GetSlice(^5..5);
		c = new G.List<T>(OriginalCollection).GetRange(OriginalCollection.Length - 5, 10 - OriginalCollection.Length);
		Assert.IsTrue(TestCollection.Equals(OriginalCollection));
		Assert.IsTrue(E.SequenceEqual(OriginalCollection, TestCollection));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.GetSlice(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.GetSlice(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = TestCollection.GetSlice(1..1000));
	}
}

public record class BaseListTests<T, TCertain>(TCertain TestCollection, ImmutableArray<T> OriginalCollection, T DefaultString, G.IEnumerable<T> DefaultCollection) where TCertain : BaseList<T, TCertain>, new()
{
	public static void BreakFilterInPlaceAsserts(TCertain a, TCertain b, TCertain c, G.List<T> d, G.List<T> e)
	{
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(c.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, c));
	}

	public static void TestToArray(Func<T> randomizer)
	{
		int length, capacity;
		G.List<T> b;
		T[] array;
		T[] array2;
		T elem;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(51);
			capacity = length + random.Next(151);
			var method = typeof(TCertain).GetConstructor(new[] { typeof(int) });
			var a = method?.Invoke(new object[] { capacity }) as TCertain ?? throw new InvalidOperationException();
			b = new(capacity);
			for (var j = 0; j < length; j++)
			{
				a.Add(elem = randomizer());
				b.Add(elem);
			}
			array = a.ToArray();
			array2 = b.ToArray();
			Assert.IsTrue(RedStarLinq.Equals(array, array2));
			Assert.IsTrue(E.SequenceEqual(array, array2));
		}
	}

	public static void TestTrimExcess(Func<T> randomizer)
	{
		int length, capacity;
		G.List<T> b;
		T elem;
		for (var i = 0; i < 1000; i++)
		{
			length = random.Next(51);
			capacity = length + random.Next(9951);
			var method = typeof(TCertain).GetConstructor(new[] { typeof(int) });
			var a = method?.Invoke(new object[] { capacity }) as TCertain ?? throw new InvalidOperationException();
			b = new(capacity);
			for (var j = 0; j < length; j++)
			{
				a.Add(elem = randomizer());
				b.Add(elem);
			}
			a.TrimExcess();
			Assert.IsTrue(RedStarLinq.Equals(a, b));
			Assert.IsTrue(E.SequenceEqual(a, b));
		}
	}
}

public record class BaseStringIndexableTests<TCertain>(TCertain TestCollection, ImmutableArray<string> OriginalCollection, string DefaultString, G.IEnumerable<string> DefaultCollection) where TCertain : BaseIndexable<string, TCertain>, new()
{
	public void TestIndexOf()
	{
		var b = TestCollection.IndexOf("MMM");
		Assert.AreEqual(b, 0);
		b = TestCollection.IndexOf("BBB", 2);
		Assert.AreEqual(b, -1);
		b = TestCollection.IndexOf("BBB", 1, 2);
		Assert.AreEqual(b, 1);
		b = TestCollection.IndexOf(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 2);
		b = TestCollection.IndexOf(new List<string>("PPP", "DDD", "NNN"));
		Assert.AreEqual(b, -1);
		b = TestCollection.IndexOf(new[] { "MMM", "EEE" }, 4);
		Assert.AreEqual(b, 4);
		b = TestCollection.IndexOf(new[] { "MMM", "EEE" }, 0, 4);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.IndexOf("BBB", -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.IndexOf("BBB", -1, 5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.IndexOf("BBB", 3, -5));
		Assert.ThrowsException<ArgumentException>(() => b = TestCollection.IndexOf("BBB", 1, 1000));
		Assert.ThrowsException<ArgumentNullException>(() => TestCollection.IndexOf((G.IEnumerable<string>)null!));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.IndexOf(new[] { "MMM", "EEE" }, -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.IndexOf(new[] { "MMM", "EEE" }, -1, 5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.IndexOf(new[] { "MMM", "EEE" }, 3, -5));
		Assert.ThrowsException<ArgumentException>(() => b = TestCollection.IndexOf(new[] { "MMM", "EEE" }, 1, 1000));
	}

	public void TestLastIndexOf()
	{
		var b = TestCollection.LastIndexOf("MMM");
		Assert.AreEqual(b, 4);
		b = TestCollection.LastIndexOf("BBB", 2);
		Assert.AreEqual(b, 1);
		b = TestCollection.LastIndexOf("BBB", 3, 2);
		Assert.AreEqual(b, -1);
		b = TestCollection.LastIndexOf(new List<string>("PPP", "DDD", "MMM"));
		Assert.AreEqual(b, 2);
		b = TestCollection.LastIndexOf(new List<string>("PPP", "DDD", "NNN"));
		Assert.AreEqual(b, -1);
		b = TestCollection.LastIndexOf(new[] { "MMM", "EEE" }, 3);
		Assert.AreEqual(b, -1);
		b = TestCollection.LastIndexOf(new[] { "MMM", "EEE" }, 5, 4);
		Assert.AreEqual(b, 4);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.LastIndexOf("BBB", -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.LastIndexOf("BBB", -1, 5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.LastIndexOf("BBB", 3, -5));
		Assert.ThrowsException<ArgumentException>(() => b = TestCollection.LastIndexOf("BBB", 1, 1000));
		Assert.ThrowsException<ArgumentNullException>(() => TestCollection.LastIndexOf((G.IEnumerable<string>)null!));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.LastIndexOf(new[] { "MMM", "EEE" }, -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.LastIndexOf(new[] { "MMM", "EEE" }, -1, 5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = TestCollection.LastIndexOf(new[] { "MMM", "EEE" }, 3, -5));
		Assert.ThrowsException<ArgumentException>(() => b = TestCollection.LastIndexOf(new[] { "MMM", "EEE" }, 1, 1000));
	}
}
