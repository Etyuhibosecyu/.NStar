
namespace Corlib.NStar.Tests;

[TestClass]
public class BigArrayTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigArray<byte> a = new(bytes, 2, 3);
		BigArray<byte> b = new(a, 2, 3);
		Assert.IsTrue(E.SequenceEqual(a, b));
		var length = 9;
		var sourceIndex = 10;
		var destinationIndex = 15;
		a.SetRange(destinationIndex, a.GetRange(sourceIndex, length));
		Assert.IsTrue(E.SequenceEqual(a.GetRange(0, destinationIndex), E.Take(b, destinationIndex)));
		Assert.IsTrue(E.SequenceEqual(a.GetRange(destinationIndex, length), E.Take(E.Skip(b, sourceIndex), length)));
		Assert.IsTrue(RedStarLinq.Equals(a.GetRange(destinationIndex + length), E.Skip(b, destinationIndex + length)));
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(33);
			sourceIndex = random.Next(bytes.Length - length + 1);
			destinationIndex = random.Next(bytes.Length - length + 1);
			a = new(bytes, 2, 3);
			b = new(a, 2, 3);
			a.SetRange(destinationIndex, a.GetRange(sourceIndex, length));
			Assert.IsTrue(E.SequenceEqual(a.GetRange(0, destinationIndex), E.Take(b, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(a.GetRange(destinationIndex, length), E.Take(E.Skip(b, sourceIndex), length)));
			Assert.IsTrue(E.SequenceEqual(a.GetRange(destinationIndex + length), E.Skip(b, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestClear()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigArray<byte> a = new(bytes, 2, 3);
		var b = E.ToArray(bytes);
		a.Clear(24, 41);
		Array.Clear(b, 24, 41);
		Assert.IsTrue(E.SequenceEqual(a, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = a.Contains(199);
		Assert.IsTrue(b);
		b = a.Contains(171, 137);
		Assert.IsTrue(!b);
		b = a.Contains(new byte[] { 1, 66, 221 }.ToList());
		Assert.IsTrue(b);
		b = a.Contains(new byte[] { 1, 66, 220 }.ToList());
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<byte>)null!));
	}

	[TestMethod]
	public void TestContainsAny()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = a.ContainsAny(new byte[] { 82, 245, 123 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAny(new byte[] { 8, 6, 5 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAny(new byte[] { 8, 6, 2 }.ToList());
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestContainsAnyExcluding()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = a.ContainsAnyExcluding(new byte[] { 82, 245, 123 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(new byte[] { 8, 6, 2 }.ToList());
		Assert.IsTrue(b);
		b = a.ContainsAnyExcluding(a);
		Assert.IsTrue(!b);
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = new BigArray<byte>(testBytes, 2, 3);
		var b = RedStarLinq.FillArray(256, _ => (byte)random.Next(256));
		var c = (byte[])b.Clone();
		var d = (byte[])b.Clone();
		var e = (byte[])b.Clone();
		a.CopyTo(b);
		new G.List<byte>(testBytes).CopyTo(c);
		a.CopyTo(d, 11);
		new G.List<byte>(testBytes).CopyTo(e, 11);
		Assert.IsTrue(E.SequenceEqual(testBytes, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}
}

[TestClass]
public class BigBitArrayTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigBitArray bitArray = new(bytes, 2, 6);
		BigBitArray bitArray2 = new(bitArray, 2, 6);
		Assert.IsTrue(E.SequenceEqual(bitArray, bitArray2));
		var length = 20;
		var sourceIndex = 72;
		var destinationIndex = 123;
		bitArray.SetRange(destinationIndex, bitArray.GetRange(sourceIndex, length));
		Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(0, destinationIndex), E.Take(bitArray2, destinationIndex)));
		Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex, length), E.Take(E.Skip(bitArray2, sourceIndex), length)));
		Assert.IsTrue(RedStarLinq.Equals(bitArray.GetRange(destinationIndex + length), E.Skip(bitArray2, destinationIndex + length)));
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(257);
			sourceIndex = random.Next(bytes.Length * 8 - length + 1);
			destinationIndex = random.Next(bytes.Length * 8 - length + 1);
			bitArray = new(bytes, 2, 6);
			bitArray2 = new(bitArray, 2, 6);
			bitArray.SetRange(destinationIndex, bitArray.GetRange(sourceIndex, length));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(0, destinationIndex), E.Take(bitArray2, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex, length), E.Take(E.Skip(bitArray2, sourceIndex), length)));
			Assert.IsTrue(E.SequenceEqual(bitArray.GetRange(destinationIndex + length), E.Skip(bitArray2, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestClear()
	{
		var bytes = new byte[150];
		random.NextBytes(bytes);
		BigBitArray a = new(bytes, 2, 6);
		var b = E.ToArray(E.SelectMany(bytes, x => E.Select(E.Range(0, 8), y => (x & 1 << y) != 0)));
		Assert.IsTrue(E.SequenceEqual(a, b));
		a.Clear(248, 431);
		Array.Clear(b, 248, 431);
		Assert.IsTrue(E.SequenceEqual(a, b));
	}

	[TestMethod]
	public void TestContains()
	{
		var a = new BigBitArray(testBytes, 2, 6);
		var b = a.Contains(false);
		Assert.IsTrue(b);
		b = a.Contains(true, 1193);
		Assert.IsTrue(!b);
		b = a.Contains(new BitList(new byte[] { 1, 66, 221 }));
		Assert.IsTrue(b);
		b = a.Contains(new BitList(new byte[] { 1, 66, 220 }));
		Assert.IsTrue(!b);
		Assert.ThrowsException<ArgumentNullException>(() => a.Contains((G.IEnumerable<bool>)null!));
	}

	[TestMethod]
	public void TestCopyTo()
	{
		var a = new BigBitArray(testBytes, 2, 6);
		var bytes = new byte[256];
		var b = E.ToArray(E.SelectMany(bytes, x => E.Select(E.Range(0, 8), y => (x & 1 << y) != 0)));
		var c = (bool[])b.Clone();
		var d = (bool[])b.Clone();
		var e = (bool[])b.Clone();
		a.CopyTo(b);
		new G.List<bool>(testBools).CopyTo(c);
		a.CopyTo(d, 185);
		new G.List<bool>(testBools).CopyTo(e, 185);
		Assert.IsTrue(E.SequenceEqual(testBools, a));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(E.SequenceEqual(e, d));
	}

	[TestMethod]
	public void TestGetSmallRange()
	{
		byte[] bytes;
		BigBitArray bitArray;
		G.List<bool> bitArray2;
		int length;
		int sourceIndex;
		uint range;
		for (var i = 0; i < 1000; i++)
		{
			bytes = new byte[40];
			random.NextBytes(bytes);
			length = random.Next(33);
			sourceIndex = random.Next(bytes.Length * 8 - length);
			bitArray = new(bytes, 2, 6);
			bitArray2 = new(bitArray);
			range = bitArray.GetSmallRange(sourceIndex, length);
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray.GetRange(sourceIndex, length)));
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitArray2.GetRange(sourceIndex, length), new[] { range }.ToBitList()[..length]));
		}
		length = 32;
		for (var i = 0; i < 100; i++)
		{
			bytes = new byte[40];
			random.NextBytes(bytes);
			sourceIndex = random.Next(bytes.Length * 8 - length);
			bitArray = new(bytes, 2, 6);
			bitArray2 = new(bitArray);
			range = bitArray.GetSmallRange(sourceIndex, length);
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray.GetRange(sourceIndex, length)));
			Assert.IsTrue(new[] { range }.ToBitList()[..length].Equals(bitArray2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitArray2.GetRange(sourceIndex, length), new[] { range }.ToBitList()[..length]));
		}
	}
}
