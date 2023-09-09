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
