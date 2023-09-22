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
	public void TestToArray(Func<T> randomizer)
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

	public void TestTrimExcess(Func<T> randomizer)
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
