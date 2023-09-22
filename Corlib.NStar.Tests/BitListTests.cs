
namespace Corlib.NStar.Tests;

[TestClass]
public class BitListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[40];
		random.NextBytes(bytes);
		var length = 10;
		var sourceIndex = 36;
		var destinationIndex = 61;
		BitList bitList;
		BitList bitList2;
		PerformIteration();
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(97);
			sourceIndex = random.Next(bytes.Length * 8 - length + 1);
			destinationIndex = random.Next(bytes.Length * 8 - length + 1);
			PerformIteration();
		}
		void PerformIteration()
		{
			bitList = new(bytes);
			bitList2 = new(bitList);
			bitList.SetRange(destinationIndex, bitList.GetRange(sourceIndex, length));
			Assert.IsTrue(bitList[..destinationIndex].Equals(bitList2[..destinationIndex]));
			Assert.IsTrue(E.SequenceEqual(bitList[..destinationIndex], E.Take(bitList2, destinationIndex)));
			Assert.IsTrue(bitList[destinationIndex..(destinationIndex + length)].Equals(bitList2[sourceIndex..(sourceIndex + length)]));
			Assert.IsTrue(E.SequenceEqual(bitList[destinationIndex..(destinationIndex + length)], E.Take(E.Skip(bitList2, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(bitList[(destinationIndex - 1)..(destinationIndex + length)].Equals(bitList2[(sourceIndex - 1)..(sourceIndex + length)]), E.SequenceEqual(bitList[(destinationIndex - 1)..(destinationIndex + length)], E.Take(E.Skip(bitList2, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length * 8 - 1 && destinationIndex + length < bytes.Length * 8 - 1)
				Assert.AreEqual(bitList[destinationIndex..(destinationIndex + length + 1)].Equals(bitList2[sourceIndex..(sourceIndex + length + 1)]), E.SequenceEqual(bitList[destinationIndex..(destinationIndex + length + 1)], E.Take(E.Skip(bitList2, sourceIndex), length + 1)));
			Assert.IsTrue(bitList[(destinationIndex + length)..].Equals(bitList2[(destinationIndex + length)..]));
			Assert.IsTrue(E.SequenceEqual(bitList[(destinationIndex + length)..], E.Skip(bitList2, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestAdd()
	{
		var a = new BitList(bitList).Add(defaultBit);
		var b = new G.List<bool>(bitList) { defaultBit };
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAddRange()
	{
		var a = new BitList(bitList).AddRange(defaultBitCollection);
		var b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultBitCollection.Take(2..5));
		b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestAppend()
	{
		var a = new BitList(bitList);
		var b = a.Append(defaultBit);
		var c = E.Append(new G.List<bool>(bitList), defaultBit);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestBreakFilter()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.BreakFilter(x => x ^ bitList[25], out var c);
		var d = new G.List<bool>(bitList);
		d.InsertRange(3, new G.List<bool>() { false, true });
		var e = E.Where(d, x => x ^ bitList[25]);
		var f = E.Where(d, x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, b));
		Assert.IsTrue(c.Equals(f));
		Assert.IsTrue(E.SequenceEqual(f, c));
		b = a.BreakFilter((x, index) => index >= 11, out c);
		e = E.Where(d, (x, index) => index >= 11);
		f = E.Where(d, (x, index) => index < 11);
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
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.BreakFilterInPlace(x => x ^ bitList[25], out var c);
		var d = new G.List<bool>(bitList);
		d.InsertRange(3, new G.List<bool>() { false, true });
		var e = E.ToList(E.Where(d, x => !x ^ bitList[25]));
		d = E.ToList(E.Where(d, x => x ^ bitList[25]));
		BaseListTests<bool, BitList>.BreakFilterInPlaceAsserts(a, b, c, d, e);
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.BreakFilterInPlace((x, index) => index >= 11, out c);
		d = new G.List<bool>(bitList);
		d.InsertRange(3, new G.List<bool>() { false, true });
		e = E.ToList(E.Where(d, (x, index) => index < 11));
		d = E.ToList(E.Where(d, (x, index) => index >= 11));
		BaseListTests<bool, BitList>.BreakFilterInPlaceAsserts(a, b, c, d, e);
	}

	[TestMethod]
	public void TestClear()
	{
		var a = new BitList(bitList);
		a.Clear(2, 4);
		var b = new G.List<bool>(bitList);
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
			var a = new BitList(E.Select(E.Range(0, random.Next(3, 3000)), _ => random.Next(2) == 1));
			var b = new BitList(a);
			var n = random.Next(0, a.Length);
			do
				b[n] = random.Next(2) == 1;
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b) == n);
			a = new(E.Select(E.Range(0, random.Next(5, 3000)), _ => random.Next(2) == 1));
			b = new(a);
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(2) == 1;
			while (b[n] == a[n]);
			Assert.IsTrue(a.Compare(b, n - 1) == n - 1);
			a = new(E.Select(E.Range(0, random.Next(5, 3000)), _ => random.Next(2) == 1));
			b = new(a);
			var length = a.Length;
			n = random.Next(2, a.Length);
			do
				b[n] = random.Next(2) == 1;
			while (b[n] == a[n]);
			int index = random.Next(2, 50), otherIndex = random.Next(2, 50);
			a.Insert(0, E.Select(E.Range(0, index), _ => random.Next(2) == 1));
			b.Insert(0, E.Select(E.Range(0, otherIndex), _ => random.Next(2) == 1));
			Assert.IsTrue(a.Compare(index, b, otherIndex) == n);
			Assert.IsTrue(a.Compare(index, b, otherIndex, length) == n);
		}
	}

	[TestMethod]
	public void TestConcat()
	{
		var a = new BitList(bitList);
		var b = a.Concat(new(defaultBitCollection));
		var c = E.Concat(new G.List<bool>(bitList), defaultBitCollection);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = new BitList(bitList);
		var b = a.Contains(true);
		Assert.IsTrue(b);
		b = a.Contains(false, 65);
		Assert.IsTrue(!b);
		b = a.Contains(new G.List<bool>() { false, true, true });
		Assert.IsTrue(b);
		b = a.Contains(new G.List<bool>() { false, true, false }, 3, 3);
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = new BitList(bitList);
		var b = a.ContainsAny(new G.List<bool>() { false, true, true });
		Assert.IsTrue(b);
		b = a.ContainsAny(new G.List<bool>() { true, true, false });
		Assert.IsTrue(b);
		b = a.ContainsAny(new G.List<bool>() { false, false, false }, 65);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = new BitList(bitList);
		var b = a.ContainsAnyExcluding(new G.List<bool>() { false, true, true });
		Assert.IsTrue(!b);
		b = a.ContainsAnyExcluding(new G.List<bool>() { false, false, false }, 65);
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestConvert()
	{
		var a = new BitList(bitList);
		var b = a.Convert((x, index) => (x, index));
		var c = E.Select(new G.List<bool>(bitList), (x, index) => (x, index));
		var d = a.Convert(x => x + "A");
		var e = E.Select(new G.List<bool>(bitList), x => x + "A");
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(d.Equals(e));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = new BitList(bitList);
		var b = RedStarLinq.FillArray(128, x => random.Next(2) == 1);
		var c = (bool[])b.Clone();
		var d = (bool[])b.Clone();
		var e = (bool[])b.Clone();
		a.CopyTo(b);
		new G.List<bool>(bitList).CopyTo(c);
		a.CopyTo(d, 3);
		new G.List<bool>(bitList).CopyTo(e, 3);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestEndsWith()
	{
		var a = new BitList(bitList);
		var b = a.EndsWith(new G.List<bool>() { true });
		Assert.IsTrue(b);
		b = a.EndsWith(new G.List<bool>() { true, true, true });
		Assert.IsTrue(b);
		b = a.EndsWith(new G.List<bool>() { true, true, false });
		Assert.IsTrue(!b);
		b = a.EndsWith(new G.List<bool>() { false, false, false });
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestEquals()
	{
		var a = new BitList(bitList);
		var b = a.Contains(true);
		Assert.IsTrue(b);
		b = a.Equals(new G.List<bool>() { false, false, false }, 2);
		Assert.IsTrue(b);
		b = a.Equals(new G.List<bool>() { false, true, false }, 2);
		Assert.IsTrue(!b);
		b = a.Equals(new G.List<bool>() { false, true, true }, 3);
		Assert.IsTrue(!b);
		b = a.Equals(new G.List<bool>() { false, true, true }, 2, true);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestFilter()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.Filter(x => x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = E.Where(c, x => x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		b = a.Filter((x, index) => index >= 11);
		d = E.Where(c, (x, index) => index >= 11);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFilterInPlace()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.FilterInPlace(x => x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		c = E.ToList(E.Where(c, x => x ^ bitList[25]));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.FilterInPlace((x, index) => index >= 11);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		c = E.ToList(E.Where(c, (x, index) => index >= 11));
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
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.Find(x => !x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.Find(x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.Find(x => x ^ x);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = c.Find(x => x ^ x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindAll()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.FindAll(x => !x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.FindAll(x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.FindAll(x => !x);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = c.FindAll(x => !x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestFindIndex()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.FindIndex(x => !x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.FindIndex(x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.FindIndex(x => x ^ x);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = c.FindIndex(x => x ^ x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindLast()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.FindLast(x => !x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.FindLast(x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.FindLast(x => x ^ x);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = c.FindLast(x => x ^ x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestFindLastIndex()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.FindLastIndex(x => !x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.FindLastIndex(x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.FindLastIndex(x => x ^ x);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = c.FindLastIndex(x => x ^ x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestGetAfter()
	{
		var a = new BitList(bitList);
		var b = a.GetAfter(new G.List<bool>() { false });
		var c = new G.List<bool>(a[1..]);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new());
		c = new();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfter(new G.List<bool>() { true, true }, 65);
		c = new G.List<bool>() { true, true, true, true };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetAfterLast()
	{
		var a = new BitList(bitList);
		var b = a.GetAfterLast(new G.List<bool>() { true }, 65);
		var c = new G.List<bool>(a[66..]);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast(new());
		c = new();
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetAfterLast(new G.List<bool>() { true, true }, 65);
		c = new G.List<bool>() { true, true, true, true, true };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBefore()
	{
		var a = new BitList(bitList);
		var b = a.GetBefore(new G.List<bool>() { true }, 65);
		var c = new G.List<bool>(a[..65]);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new());
		c = new(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBefore(new G.List<bool>() { false, false }, 3, 2);
		c = new G.List<bool>() { false, false, false };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeLast()
	{
		var a = new BitList(bitList);
		var b = a.GetBeforeLast(new G.List<bool>() { true }, 65);
		var c = new G.List<bool>(a[..65]);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast(new());
		c = new(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetBeforeLast(new G.List<bool>() { false, false }, 3, 2);
		c = new G.List<bool>() { false, false };
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfter()
	{
		var a = new BitList(bitList);
		var b = a.GetBeforeSetAfter(new G.List<bool>() { true }, 65);
		var c = new G.List<bool>(bitList[..65]);
		var d = new G.List<bool>(new G.List<bool>() { true, true, true, true, true });
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetBeforeSetAfterLast()
	{
		var a = new BitList(bitList);
		var b = a.GetBeforeSetAfterLast(new G.List<bool>() { true }, 65);
		var c = new G.List<bool>(bitList[..65]);
		var d = new G.List<bool>(bitList[66..]);
		Assert.IsTrue(a.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestGetRange()
	{
		var a = new BitList(bitList);
		var b = a.GetRange(..);
		var c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(.., true);
		b.Add(defaultBit);
		c = new G.List<bool>(bitList).GetRange(0, bitList.Length);
		c.Add(defaultBit);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(..^1);
		c = new G.List<bool>(bitList).GetRange(0, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(1..);
		c = new G.List<bool>(bitList).GetRange(1, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(1..^1);
		c = new G.List<bool>(bitList).GetRange(1, bitList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(1..5);
		c = new G.List<bool>(bitList).GetRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(^5..);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 5, 5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(^5..^1);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetRange(^50..50);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 50, 100 - bitList.Length);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetRange(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = a.GetRange(1..1000));
	}

	[TestMethod]
	public void TestGetSlice()
	{
		var a = new BitList(bitList);
		var b = a.GetSlice(..);
		var c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1);
		c = new G.List<bool>(bitList).GetRange(1, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1, 4);
		c = new G.List<bool>(bitList).GetRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^5);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 5, 5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(..^1);
		c = new G.List<bool>(bitList).GetRange(0, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1..);
		c = new G.List<bool>(bitList).GetRange(1, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1..^1);
		c = new G.List<bool>(bitList).GetRange(1, bitList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(1..5);
		c = new G.List<bool>(bitList).GetRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^5..);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 5, 5);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^5..^1);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.GetSlice(^50..50);
		c = new G.List<bool>(bitList).GetRange(bitList.Length - 50, 100 - bitList.Length);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetSlice(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = a.GetSlice(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = a.GetSlice(1..1000));
	}

	[TestMethod]
	public void TestGetSmallRange()
	{
		byte[] bytes;
		BitList bitList;
		G.List<bool> bitList2;
		int length;
		int sourceIndex;
		uint range;
		for (var i = 0; i < 1000; i++)
		{
			bytes = new byte[40];
			random.NextBytes(bytes);
			length = random.Next(33);
			sourceIndex = random.Next(bytes.Length * 8 - length);
			bitList = new(bytes);
			bitList2 = new(bitList);
			range = bitList.GetSmallRange(sourceIndex, length);
			Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList[sourceIndex..(sourceIndex + length)]));
			Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitList2.GetRange(sourceIndex, length), new BitList(new[] { range })[..length]));
		}
		length = 32;
		for (var i = 0; i < 100; i++)
		{
			bytes = new byte[40];
			random.NextBytes(bytes);
			sourceIndex = random.Next(bytes.Length * 8 - length);
			bitList = new(bytes);
			bitList2 = new(bitList);
			range = bitList.GetSmallRange(sourceIndex, length);
			Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList[sourceIndex..(sourceIndex + length)]));
			Assert.IsTrue(new BitList(new[] { range })[..length].Equals(bitList2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitList2.GetRange(sourceIndex, length), new BitList(new[] { range })[..length]));
		}
	}

	[TestMethod]
	public void TestIndexOf()
	{
		var a = new BitList(bitList);
		var b = a.IndexOf(false);
		Assert.AreEqual(b, 0);
		b = a.IndexOf(false, 65);
		Assert.AreEqual(b, -1);
		b = a.IndexOf(a[1], 1, 2);
		Assert.AreEqual(b, 1);
		b = a.IndexOf(new G.List<bool>() { false, false, false });
		Assert.AreEqual(b, 0);
		b = a.IndexOf(new G.List<bool>() { false, true, false }, 65);
		Assert.AreEqual(b, -1);
		b = a.IndexOf(new[] { a[4], a[5], a[6], a[7], a[8] }, 4);
		Assert.AreEqual(b, 4);
		b = a.IndexOf(new[] { a[4], a[5], a[6], a[7], a[8] }, 0, 4);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOf((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestIndexOfAny()
	{
		var a = new BitList(bitList);
		var b = a.IndexOfAny(new G.List<bool>() { false, true, true });
		Assert.AreEqual(b, 0);
		b = a.IndexOfAny(new G.List<bool>() { a[9], a[2], a[45] }, 2);
		Assert.AreEqual(b, 2);
		b = a.IndexOfAny(new[] { true, true, true }, 0, 4);
		Assert.AreEqual(b, -1);
		b = a.IndexOfAny(new G.List<bool>() { false, false, false }, 65);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAny((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestIndexOfAnyExcluding()
	{
		var a = new BitList(bitList);
		var b = a.IndexOfAnyExcluding(new G.List<bool>() { false, true, true });
		Assert.AreEqual(b, -1);
		b = a.IndexOfAnyExcluding(new G.List<bool>() { false, false, false }, 65);
		Assert.AreEqual(b, 65);
		b = a.IndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.IndexOfAnyExcluding((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestInsert()
	{
		var a = new BitList(bitList).Insert(3, defaultBit);
		var b = new G.List<bool>(bitList);
		b.Insert(3, defaultBit);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(4, defaultBitCollection);
		b = new G.List<bool>(bitList);
		b.InsertRange(4, defaultBitCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(2, defaultBitCollection.Take(2..5));
		b = new G.List<bool>(bitList);
		b.InsertRange(2, defaultBitCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a = new BitList(bitList).Insert(1000, defaultBit));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BitList(bitList).Insert(-1, defaultBitCollection));
		Assert.ThrowsException<ArgumentNullException>(() => new BitList(bitList).Insert(1, null!));
	}

	[TestMethod]
	public void TestLastIndexOf()
	{
		var a = new BitList(bitList);
		var b = a.LastIndexOf(true);
		Assert.AreEqual(b, 70);
		b = a.LastIndexOf(new[] { a[1], a[2] }, 2);
		Assert.AreEqual(b, 1);
		b = a.LastIndexOf(new BitList(5, true), 3, 2);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOf(new G.List<bool>() { false, true, true });
		Assert.AreEqual(b, 63);
		b = a.LastIndexOf(new G.List<bool>() { false, true, false });
		Assert.AreEqual(b, 54);
		b = a.LastIndexOf(new[] { a[4], a[5], a[6], a[7], a[8] }, 3);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOf(new[] { a[4], a[5], a[6], a[7], a[8] });
		Assert.AreEqual(b, 56);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOf((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAny()
	{
		var a = new BitList(bitList);
		var b = a.LastIndexOfAny(new G.List<bool>() { false, true, true });
		Assert.AreEqual(b, 70);
		b = a.LastIndexOfAny(new G.List<bool>() { a[9], a[2], a[45] }, 9);
		Assert.AreEqual(b, 9);
		b = a.LastIndexOfAny(new BitList(3, true), 4);
		Assert.AreEqual(b, -1);
		b = a.LastIndexOfAny(new G.List<bool>() { true, true, true }, 3);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAny((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestLastIndexOfAnyExcluding()
	{
		var a = new BitList(bitList);
		var b = a.LastIndexOfAnyExcluding(new G.List<bool>() { false, true, true });
		Assert.AreEqual(b, -1);
		b = a.LastIndexOfAnyExcluding(new G.List<bool>() { true, true, true }, 3);
		Assert.AreEqual(b, 3);
		b = a.LastIndexOfAnyExcluding(a);
		Assert.AreEqual(b, -1);
		Assert.ThrowsException<ArgumentNullException>(() => a.LastIndexOfAnyExcluding((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestPad()
	{
		var a = new BitList(bitList);
		var b = a.Pad(100);
		var c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(14, false));
		c.AddRange(new BitList(15, false));
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(100, true);
		c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(14, true));
		c.AddRange(new BitList(15, true));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(36);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.Pad(36, true);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pad(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.Pad(-1, true));
	}

	[TestMethod]
	public void TestPadInPlace()
	{
		var a = new BitList(bitList);
		var b = a.PadInPlace(100);
		var c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(14, false));
		c.AddRange(new BitList(15, false));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadInPlace(100, true);
		c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(14, true));
		c.AddRange(new BitList(15, true));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadInPlace(36);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadInPlace(36, true);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadInPlace(-1, true));
	}

	[TestMethod]
	public void TestPadLeft()
	{
		var a = new BitList(bitList);
		var b = a.PadLeft(100);
		var c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(29, false));
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(100, true);
		c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(29, true));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(36);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadLeft(36, true);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeft(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeft(-1, true));
	}

	[TestMethod]
	public void TestPadLeftInPlace()
	{
		var a = new BitList(bitList);
		var b = a.PadLeftInPlace(100);
		var c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(29, false));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadLeftInPlace(100, true);
		c = new G.List<bool>(bitList);
		c.InsertRange(0, new BitList(29, true));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadLeftInPlace(36);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadLeftInPlace(36, true);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadLeftInPlace(-1, true));
	}

	[TestMethod]
	public void TestPadRight()
	{
		var a = new BitList(bitList);
		var b = a.PadRight(100);
		var c = new G.List<bool>(bitList);
		c.AddRange(new BitList(29, false));
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(100, true);
		c = new G.List<bool>(bitList);
		c.AddRange(new BitList(29, true));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(36);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.PadRight(36, true);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRight(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRight(-1, true));
	}

	[TestMethod]
	public void TestPadRightInPlace()
	{
		var a = new BitList(bitList);
		var b = a.PadRightInPlace(100);
		var c = new G.List<bool>(bitList);
		c.AddRange(new BitList(29, false));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadRightInPlace(100, true);
		c = new G.List<bool>(bitList);
		c.AddRange(new BitList(29, true));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadRightInPlace(36);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new(bitList);
		b = a.PadRightInPlace(36, true);
		c = new G.List<bool>(bitList);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => a.PadRightInPlace(-1, true));
	}

	[TestMethod]
	public void TestRemove()
	{
		var a = new BitList(bitList);
		var b = new BitList(a).Remove(70);
		var c = new G.List<bool>(bitList);
		c.RemoveRange(70, 1);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(0, 1);
		c = new G.List<bool>(bitList);
		c.RemoveRange(0, 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(1, bitList.Length - 2);
		c = new G.List<bool>(bitList);
		c.RemoveRange(1, bitList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(1, 4);
		c = new G.List<bool>(bitList);
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(bitList.Length - 5, 4);
		c = new G.List<bool>(bitList);
		c.RemoveRange(bitList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(bitList.Length - 50, 100 - bitList.Length);
		c = new G.List<bool>(bitList);
		c.RemoveRange(bitList.Length - 50, 100 - bitList.Length);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new BitList(a).Remove(-1, 6));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new BitList(a).Remove(bitList.Length - 1, 2 - bitList.Length));
		Assert.ThrowsException<ArgumentException>(() => b = new BitList(a).Remove(1, 1000));
		b = new BitList(a).Remove(..);
		c = new G.List<bool>();
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(..^1);
		c = new G.List<bool>(bitList);
		c.RemoveRange(0, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(1..);
		c = new G.List<bool>(bitList);
		c.RemoveRange(1, bitList.Length - 1);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(1..^1);
		c = new G.List<bool>(bitList);
		c.RemoveRange(1, bitList.Length - 2);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(1..5);
		c = new G.List<bool>(bitList);
		c.RemoveRange(1, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(^5..^1);
		c = new G.List<bool>(bitList);
		c.RemoveRange(bitList.Length - 5, 4);
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = new BitList(a).Remove(^50..50);
		c = new G.List<bool>(bitList);
		c.RemoveRange(bitList.Length - 50, 100 - bitList.Length);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new BitList(a).Remove(-1..5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => b = new BitList(a).Remove(^1..1));
		Assert.ThrowsException<ArgumentException>(() => b = new BitList(a).Remove(1..1000));
	}

	[TestMethod]
	public void TestRemoveAll()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.RemoveAll(x => !x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.RemoveAll(x => !x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.RemoveAll(x => x);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = c.RemoveAll(x => x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}

	[TestMethod]
	public void TestRemoveAt()
	{
		var a = new BitList(bitList);
		for (var i = 0; i < 1000; i++)
		{
			var index = random.Next(a.Length);
			var b = new BitList(a).RemoveAt(index);
			var c = new G.List<bool>(a);
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
		for (var i = 0; i < 1000; i++)
		{
			var a = new Chain(15, 10).ToBitList();
			var value = a.Random(random);
			var b = new BitList(a);
			b.RemoveValue(value);
			var c = new G.List<bool>(a);
			c.Remove(value);
			Assert.IsTrue(b.Equals(c));
			Assert.IsTrue(E.SequenceEqual(c, b));
		}
	}

	[TestMethod]
	public void TestRepeat()
	{
		for (var i = 0; i < 1000; i++)
		{
			var arr = RedStarLinq.FillArray(random.Next(1, 101), _ => random.Next());
			var a = arr.ToBitList();
			var b = E.ToList(E.SelectMany(arr, x => E.Select(E.Range(0, 32), y => (x & 1 << y) != 0)));
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(E.SequenceEqual(b, a));
			var n = random.Next(11);
			var c = a.Repeat(n);
			var d = new G.List<bool>();
			for (var j = 0; j < n; j++)
				d.AddRange(b);
			Assert.IsTrue(c.Equals(d));
			Assert.IsTrue(E.SequenceEqual(d, c));
		}
	}

	[TestMethod]
	public void TestReplace()
	{
		var a = new BitList(bitList).Replace(defaultBitCollection);
		var b = new G.List<bool>(bitList);
		b.Clear();
		b.AddRange(defaultBitCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultBitCollection.Take(2..5));
		b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestReplaceRange()
	{
		var a = new BitList(bitList).ReplaceRange(2, 3, defaultBitCollection);
		var b = new G.List<bool>(bitList);
		b.RemoveRange(2, 3);
		b.InsertRange(2, defaultBitCollection);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).Insert(2, defaultBitCollection.Take(2..5));
		b = new G.List<bool>(bitList);
		b.InsertRange(2, defaultBitCollection.Skip(2).Take(3));
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentException>(() => a = new BitList(bitList).ReplaceRange(1, 1000, new[] { defaultBit }));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BitList(bitList).ReplaceRange(-1, 3, defaultBitCollection));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BitList(bitList).ReplaceRange(4, -2, defaultBitCollection));
		Assert.ThrowsException<ArgumentNullException>(() => new BitList(bitList).ReplaceRange(4, 1, null!));
	}

	[TestMethod]
	public void TestReverse()
	{
		var a = new BitList(bitList).Reverse();
		var b = new G.List<bool>(bitList);
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultBitCollection.Take(2..5)).Reverse();
		b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection.Skip(2).Take(3));
		b.Reverse();
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		a = new BitList(bitList).AddRange(defaultBitCollection.Take(2..5)).Reverse(2, 4);
		b = new G.List<bool>(bitList);
		b.AddRange(defaultBitCollection.Skip(2).Take(3));
		b.Reverse(2, 4);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
	}

	[TestMethod]
	public void TestSetAll()
	{
		var bytes = new byte[40];
		BitList bitList;
		G.List<bool> boolList;
		int count;
		int index;
		bool value;
		int endIndex;
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			count = random.Next(97);
			index = random.Next(bytes.Length * 8 - count + 1);
			value = random.Next(2) == 1;
			bitList = new(bytes);
			boolList = new(bitList);
			bitList.SetAll(value, index, count);
			endIndex = index + count;
			for (var j = index; j < endIndex; j++)
				boolList[j] = value;
			Assert.IsTrue(bitList.Equals(boolList));
			Assert.IsTrue(E.SequenceEqual(boolList, bitList));
		}
	}

	[TestMethod]
	public void TestSetOrAdd()
	{
		var a = new BitList(bitList);
		var b = new G.List<bool>(bitList);
		for (var i = 0; i < 1000; i++)
		{
			var n = (int)Floor(Cbrt(random.NextDouble()) * (a.Length + 1));
			var n2 = random.Next(2) == 1;
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
		var newList = E.ToList(defaultBitCollection);
		var a = new BitList(bitList).SetRange(2, newList);
		var b = new G.List<bool>(bitList);
		for (var i = 0; i < newList.Count; i++)
			b[i + 2] = newList[i];
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(E.SequenceEqual(b, a));
		Assert.ThrowsException<ArgumentException>(() => a = new BitList(bitList).SetRange(60, newList));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BitList(bitList).SetRange(-1, newList));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BitList(bitList).SetRange(1000, newList));
		Assert.ThrowsException<ArgumentNullException>(() => new BitList(bitList).SetRange(4, null!));
	}

	[TestMethod]
	public void TestSkip()
	{
		var a = new BitList(bitList);
		var b = a.Skip(2);
		var c = E.Skip(new G.List<bool>(bitList), 2);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.Skip(0);
		c = E.Skip(new G.List<bool>(bitList), 0);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.Skip(1000);
		c = E.Skip(new G.List<bool>(bitList), 1000);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.Skip(-4);
		c = E.Skip(new G.List<bool>(bitList), -4);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipLast()
	{
		var a = new BitList(bitList);
		var b = a.SkipLast(2);
		var c = E.SkipLast(new G.List<bool>(bitList), 2);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.SkipLast(0);
		c = E.SkipLast(new G.List<bool>(bitList), 0);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.SkipLast(1000);
		c = E.SkipLast(new G.List<bool>(bitList), 1000);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.SkipLast(-4);
		c = E.SkipLast(new G.List<bool>(bitList), -4);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestSkipWhile()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.SkipWhile(x => x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = E.ToList(E.SkipWhile(c, x => x ^ bitList[25]));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.SkipWhile((x, index) => !x || index < 1);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = E.ToList(E.SkipWhile(E.Skip(c, 1), (x, index) => !x || index < 1));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestStartsWith()
	{
		var a = new BitList(bitList);
		var b = a.StartsWith(new G.List<bool>() { false });
		Assert.IsTrue(b);
		b = a.StartsWith(new G.List<bool>() { false, false, false, false, false });
		Assert.IsTrue(b);
		b = a.StartsWith(new G.List<bool>() { false, false, false, false, true });
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.StartsWith((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestTake()
	{
		var a = new BitList(bitList);
		var b = a.Take(2);
		var c = E.Take(new G.List<bool>(bitList), 2);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.Take(0);
		c = E.Take(new G.List<bool>(bitList), 0);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.Take(1000);
		c = E.Take(new G.List<bool>(bitList), 1000);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.Take(-4);
		c = E.Take(new G.List<bool>(bitList), -4);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeLast()
	{
		var a = new BitList(bitList);
		var b = a.TakeLast(2);
		var c = E.TakeLast(new G.List<bool>(bitList), 2);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.TakeLast(0);
		c = E.TakeLast(new G.List<bool>(bitList), 0);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.TakeLast(1000);
		c = E.TakeLast(new G.List<bool>(bitList), 1000);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		a = new BitList(bitList);
		b = a.TakeLast(-4);
		c = E.TakeLast(new G.List<bool>(bitList), -4);
		Assert.IsTrue(a.Equals(bitList));
		Assert.IsTrue(E.SequenceEqual(bitList, a));
		Assert.IsTrue(b.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	[TestMethod]
	public void TestTakeWhile()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.TakeWhile(x => x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = E.ToList(E.TakeWhile(c, x => x ^ bitList[25]));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
		a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		b = a.TakeWhile((x, index) => index < 10);
		c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		d = E.ToList(E.TakeWhile(E.Take(c, 10), (x, index) => index < 10));
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.IsTrue(b.Equals(d));
		Assert.IsTrue(E.SequenceEqual(d, b));
	}

	[TestMethod]
	public void TestToArray() => BaseListTests<bool, BitList>.TestToArray(() => random.Next(2) == 1);

	[TestMethod]
	public void TestTrimExcess() => BaseListTests<bool, BitList>.TestTrimExcess(() => random.Next(2) == 1);

	[TestMethod]
	public void TestTrueForAll()
	{
		var a = new BitList(bitList).Insert(3, new G.List<bool>() { false, true });
		var b = a.TrueForAll(x => x ^ bitList[25]);
		var c = new G.List<bool>(bitList);
		c.InsertRange(3, new G.List<bool>() { false, true });
		var d = c.TrueForAll(x => x ^ bitList[25]);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x ^ !x);
		d = c.TrueForAll(x => x ^ !x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
		b = a.TrueForAll(x => x ^ x);
		d = c.TrueForAll(x => x ^ x);
		Assert.IsTrue(a.Equals(c));
		Assert.IsTrue(E.SequenceEqual(c, a));
		Assert.AreEqual(b, d);
	}
}
