using System.Threading;

namespace Corlib.NStar.Tests;

[TestClass]
public class BaseBigListTests<T, TCertain, TLow> where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	public static void ComplexTest(Func<(BigList<T, TCertain, TLow>, G.List<T>, byte[])> create, Func<T> newValueFunc, int multiplier)
	{
		var counter = 0;
		var toInsert = Array.Empty<T>();
	l1:
		var (bl, gl, bytes) = create();
		var collectionActions = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 6), _ => newValueFunc());
			var bl2 = bl.AddRange(toInsert);
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.IsTrue(bl == bl2);
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 6), _ => newValueFunc());
			var bl2 = bl.AddRange(toInsert);
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.IsTrue(bl == bl2);
			//var n = random.Next(sl.Length);
			//toInsert = RedStarLinq.FillArray(random.Next(6), _ => newValueFunc());
			//sl.Insert(n, toInsert);
			//gl.InsertRange(n, toInsert);
			//Assert.IsTrue(sl.Equals(gl));
			//Assert.IsTrue(E.SequenceEqual(gl, sl));
		}, () =>
		{
			var length = Min(random.Next(multiplier * 9), (int)bl.Length);
			if (bl.Length < length)
				return;
			var start = random.Next((int)bl.Length - length + 1);
			var bl2 = bl.Remove(start, length);
			gl.RemoveRange(start, length);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.IsTrue(bl == bl2);
		}, () =>
		{
			var length = (int)bl.Length;
			if (length == 0)
				return;
			var start = random.Next(1, length + 1);
			var bl2 = bl.RemoveEnd(start);
			gl.RemoveRange(start, length - start);
			if (random.Next(2) == 1)
				bl.TrimExcess();
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.IsTrue(bl == bl2);
		} };
		var actions = new[] { () =>
		{
			var n = newValueFunc();
			//if (random.Next(2) == 0)
			//{
				var bl2 = bl.Add(n);
				gl.Add(n);
			//}
			//else
			//{
			//	var index = random.Next((int)sl.Length + 1);
			//	sl.Insert(index, n);
			//	gl.Insert(index, n);
			//}
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.IsTrue(bl == bl2);
		}, () =>
		{
			if (bl.Length == 0) return;
			var index = random.Next((int)bl.Length);
			var bl2 = bl.RemoveAt(index);
			gl.RemoveAt(index);
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.IsTrue(bl == bl2);
		}, () =>
		{
			collectionActions.Random(random)();
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			if (bl.Length == 0) return;
			var index = random.Next((int)bl.Length);
			Assert.AreEqual(bl[index], gl[index]);
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		} };
		for (var i = 0; i < 1000; i++)
			actions.Random(random)();
		if (counter++ < 250)
			goto l1;
	}
}

[TestClass]
public class BigBitListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[40];
		random.NextBytes(bytes);
		var length = 10;
		var sourceIndex = 36;
		var destinationIndex = 61;
		BigBitList bitList, bitList2;
		BitList bitList3;
		PerformIteration();
		for (var i = 0; i < 5000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(97);
			sourceIndex = random.Next(bytes.Length * BitsPerByte - length + 1);
			destinationIndex = random.Next(bytes.Length * BitsPerByte - length + 1);
			PerformIteration();
		}
		void PerformIteration()
		{
			bitList3 = new(bytes);
			bitList = new(bitList3, 2, 6);
			bitList2 = new(bitList, 2, 6);
			bitList.CopyTo(sourceIndex, bitList, destinationIndex, length);
			Assert.IsTrue(bitList.GetRange(0, destinationIndex).Equals(bitList2.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(0, destinationIndex), E.Take(bitList2, destinationIndex)));
			Assert.IsTrue(bitList.GetRange(destinationIndex, length).Equals(bitList2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex, length), E.Take(E.Skip(bitList2, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex - 1, length + 1).Equals(bitList2.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList2, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length * BitsPerByte - 1 && destinationIndex + length < bytes.Length * BitsPerByte - 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex, length + 1).Equals(bitList2.GetRange(sourceIndex, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList2, sourceIndex), length + 1)));
			Assert.IsTrue(bitList.GetRange(destinationIndex + length).Equals(bitList2.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex + length), E.Skip(bitList2, destinationIndex + length)));
			Assert.IsTrue(bitList.GetRange(0, destinationIndex).Equals(bitList3.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(0, destinationIndex), E.Take(bitList3, destinationIndex)));
			Assert.IsTrue(bitList.GetRange(destinationIndex, length).Equals(bitList3.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex, length), E.Take(E.Skip(bitList3, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex - 1, length + 1).Equals(bitList3.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList3, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length * BitsPerByte - 1 && destinationIndex + length < bytes.Length * BitsPerByte - 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex, length + 1).Equals(bitList3.GetRange(sourceIndex, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList3, sourceIndex), length + 1)));
			Assert.IsTrue(bitList.GetRange(destinationIndex + length).Equals(bitList3.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex + length), E.Skip(bitList3, destinationIndex + length)));
		}
	}

	private BigBitList bl = default!;
	private G.List<bool> gl = default!;

	[TestMethod]
	public void ComplexTest2() => BaseBigListTests<bool, BigBitList, BitList>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(512, _ => random.Next(2) == 1);
		bl = new(arr, 2, 6);
		gl = new(arr);
		var bytes = new byte[16];
		return (bl, gl, bytes);
	}, () => random.Next(2) == 1, BitsPerInt);

	[TestMethod]
	public void ConstructionTest()
	{
		var array = new BigBitList[1000];
		var functions = new[]
		{
			() => new BigBitList(2, 6), () => new(1600, 2, 6), () => new(1600, false, 2, 6),
			() => new(RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()), 2, 6),
			() => new(1600, RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()), 2, 6),
			() => new(100, RedStarLinq.FillArray(random.Next(5, 50), _ => (uint)random.Next()), 2, 6),
			() => new(RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()).AsSpan(), 2, 6),
			() => new(100, RedStarLinq.FillArray(random.Next(5, 50), _ => (uint)random.Next()).AsSpan(), 2, 6),
			() => new(1600, RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()).AsSpan(), 2, 6),
			() => new(RedStarLinq.FillArray(random.Next(50), _ => random.Next()), 2, 6),
			() => new((G.IEnumerable<uint>)RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()), 2, 6),
			() => new(RedStarLinq.FillArray(random.Next(200), _ => (byte)random.Next(256)), 2, 6),
			() => new(RedStarLinq.FillArray(random.Next(1600), _ => random.Next(2) == 1), 2, 6),
			() => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => random.Next()), x => x), 2, 6),
			() => new(E.Select(RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()), x => x), 2, 6),
			() => new(E.Select(RedStarLinq.FillArray(random.Next(200), _ => (byte)random.Next(256)), x => x), 2, 6),
			() => new(E.Select(RedStarLinq.FillArray(random.Next(1600), _ => random.Next(2) == 1), x => x), 2, 6),
			() => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50),
				_ => random.Next()), _ => random.Next(10) == -1), 2, 6),
			() => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(50),
				_ => (uint)random.Next()), _ => random.Next(10) == -1), 2, 6),
			() => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(200),
				_ => (byte)random.Next(256)), _ => random.Next(10) == -1), 2, 6),
			() => new(E.SkipWhile(RedStarLinq.FillArray(random.Next(1600),
				_ => random.Next(2) == 1), _ => random.Next(10) == -1), 2, 6),
			() => new(new BitArray(1600), 2, 6), () => new(new BitArray(1600, false), 2, 6),
			() => new(new BitArray(RedStarLinq.FillArray(random.Next(50), _ => random.Next())), 2, 6),
			() => new(new BitArray(RedStarLinq.FillArray(random.Next(200), _ => (byte)random.Next(256))), 2, 6),
			() => new(new BitArray(RedStarLinq.FillArray(random.Next(1600), _ => random.Next(2) == 1)), 2, 6),
			() => new(new BigBitList(), 2, 6), () => new(new BigBitList((MpzT)1600), 2, 6),
			() => new(new BigBitList(1600, false), 2, 6),
			() => new(new BigBitList(RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next())), 2, 6),
			() => new(new BigBitList(1600, RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next())), 2, 6),
			() => new(new BigBitList(100, RedStarLinq.FillArray(random.Next(5, 50), _ => (uint)random.Next())), 2, 6),
			() => new(new BigBitList(RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next()).AsSpan()), 2, 6),
			() => new(new BigBitList(1600, RedStarLinq.FillArray(random.Next(50),
				_ => (uint)random.Next()).AsSpan()), 2, 6),
			() => new(new BigBitList(100, RedStarLinq.FillArray(random.Next(5, 50),
				_ => (uint)random.Next()).AsSpan()), 2, 6),
			() => new(new BigBitList(RedStarLinq.FillArray(random.Next(50), _ => random.Next())), 2, 6),
			() => new(new BigBitList((G.IEnumerable<uint>)RedStarLinq.FillArray(random.Next(50), _ => (uint)random.Next())), 2, 6),
			() => new(new BigBitList(RedStarLinq.FillArray(random.Next(200), _ => (byte)random.Next(256))), 2, 6),
			() => new(new BigBitList(RedStarLinq.FillArray(random.Next(1600), _ => random.Next(2) == 1)), 2, 6),
			() => new(new BigBitList(E.Select(RedStarLinq.FillArray(random.Next(50), _ => random.Next()), x => x)), 2, 6),
			() => new(new BigBitList(E.Select(RedStarLinq.FillArray(random.Next(50),
				_ => (uint)random.Next()), x => x)), 2, 6),
			() => new(new BigBitList(E.Select(RedStarLinq.FillArray(random.Next(200),
				_ => (byte)random.Next(256)), x => x)), 2, 6),
			() => new(new BigBitList(E.Select(RedStarLinq.FillArray(random.Next(1600),
				_ => random.Next(2) == 1), x => x)), 2, 6),
			() => new(new BigBitList(E.SkipWhile(RedStarLinq.FillArray(random.Next(50),
				_ => random.Next()), _ => random.Next(10) == -1)), 2, 6),
			() => new(new BigBitList(E.SkipWhile(RedStarLinq.FillArray(random.Next(50),
				_ => (uint)random.Next()), _ => random.Next(10) == -1)), 2, 6),
			() => new(new BigBitList(E.SkipWhile(RedStarLinq.FillArray(random.Next(200),
				_ => (byte)random.Next(256)), _ => random.Next(10) == -1)), 2, 6),
			() => new(new BigBitList(E.SkipWhile(RedStarLinq.FillArray(random.Next(1600),
				_ => random.Next(2) == 1), _ => random.Next(10) == -1)), 2, 6),
			() => new(new BigBitList(new BitArray(1600)), 2, 6), () => new(new BigBitList(new BitArray(1600, false)), 2, 6),
			() => new(new BigBitList(new BitArray(RedStarLinq.FillArray(random.Next(50), _ => random.Next()))), 2, 6),
			() => new(new BigBitList(new BitArray(RedStarLinq.FillArray(random.Next(200),
				_ => (byte)random.Next(256)))), 2, 6),
			() => new(new BigBitList(new BitArray(RedStarLinq.FillArray(random.Next(1600),
				_ => random.Next(2) == 1))), 2, 6)
		};
		for (var i = 0; i < array.Length; i++)
		{
			array[i] = functions.Random(random)();
			G.List<bool> gl = new(array[i]);
			MpzT oldLength = new(array[i].Length);
			for (var j = 0; j < 2400; j++)
			{
				var fullCheck = array[i].Capacity == array[i].Length;
				var item = random.Next(2) == 1;
				array[i].Add(item);
				gl.Add(item);
				if (fullCheck)
				{
					Assert.IsTrue(RedStarLinq.Equals(array[i], gl));
					Assert.IsTrue(E.SequenceEqual(gl, array[i]));
				}
				else
				{
					Assert.IsTrue(array[i].Capacity >= array[i].Length);
					Assert.AreEqual(array[i].Length, oldLength + j + 1);
				}
			}
		}
		Thread.Sleep(50);
		for (var i = 0; i < array.Length; i++)
			array[i].Add(random.Next(2) == 1);
	}

	[TestMethod]
	public void ConstructionTest2()
	{
		BigBitList bitList = new(RedStarLinq.FillArray(2345678901, 5 << 25)) { true };
		Assert.AreNotEqual(bitList.Length, (int)bitList.Length);
		Assert.AreNotEqual(bitList.Length, (uint)bitList.Length);
		Assert.AreEqual(bitList.Length, (long)bitList.Length);
	}

	[TestMethod]
	public void TestCopy()
	{
		var bytes = new byte[1250];
		random.NextBytes(bytes);
		var length = 435;
		var sourceIndex = 123;
		var destinationIndex = 272;
		BigBitList destination, source;
		BitList bitList3;
		PerformIteration();
		for (var i = 0; i < 100; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(801);
			sourceIndex = random.Next(bytes.Length - length + 1);
			destinationIndex = random.Next(bytes.Length - length + 1);
			PerformIteration();
		}
		void PerformIteration()
		{
			bitList3 = new(bytes);
			destination = new(bitList3, random.Next(2, 5), random.Next(5, 11));
			source = new(destination, random.Next(2, 5), random.Next(5, 11));
			Assert.IsTrue(destination.Equals(source));
			source.CopyTo(sourceIndex, destination, destinationIndex, length);
			var sourceStart = source.GetRange(0, destinationIndex);
			var destinationStart = destination.GetRange(0, destinationIndex);
			Assert.IsTrue(destinationStart.Equals(sourceStart));
			Assert.IsTrue(E.SequenceEqual(destinationStart, E.Take(source, destinationIndex)));
			var sourceMain = source.GetRange(sourceIndex, length);
			var destinationMain = destination.GetRange(destinationIndex, length);
			Assert.IsTrue(destinationMain.Equals(sourceMain));
			Assert.IsTrue(E.SequenceEqual(destinationMain, E.Take(E.Skip(source, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(destination.GetRange(destinationIndex - 1, length + 1).Equals(source.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(source, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(destination.GetRange(destinationIndex, length + 1).Equals(source.GetRange(sourceIndex, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex, length + 1), E.Take(E.Skip(source, sourceIndex), length + 1)));
			var sourceEnd = source.GetRange(destinationIndex + length);
			var destinationEnd = destination.GetRange(destinationIndex + length);
			Assert.IsTrue(destinationEnd.Equals(sourceEnd));
			Assert.IsTrue(E.SequenceEqual(destinationEnd, E.Skip(source, destinationIndex + length)));
		}
	}
}

[TestClass]
public class BigListTests
{
	[TestMethod]
	public void ComplexTest()
	{
		var bytes = new byte[40];
		random.NextBytes(bytes);
		var length = 2;
		var sourceIndex = 5;
		var destinationIndex = 9;
		BigList<byte> bitList, bitList2;
		NList<byte> bitList3;
		PerformIteration();
		for (var i = 0; i < 5000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(13);
			sourceIndex = random.Next(bytes.Length - length + 1);
			destinationIndex = random.Next(bytes.Length - length + 1);
			PerformIteration();
		}
		void PerformIteration()
		{
			bitList3 = new(bytes);
			bitList = new(bitList3, 2, 2);
			bitList2 = new(bitList, 2, 2);
			bitList.CopyTo(sourceIndex, bitList, destinationIndex, length);
			Assert.IsTrue(bitList.GetRange(0, destinationIndex).Equals(bitList2.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(0, destinationIndex), E.Take(bitList2, destinationIndex)));
			Assert.IsTrue(bitList.GetRange(destinationIndex, length).Equals(bitList2.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex, length), E.Take(E.Skip(bitList2, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex - 1, length + 1).Equals(bitList2.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList2, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex, length + 1).Equals(bitList2.GetRange(sourceIndex, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList2, sourceIndex), length + 1)));
			Assert.IsTrue(bitList.GetRange(destinationIndex + length).Equals(bitList2.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex + length), E.Skip(bitList2, destinationIndex + length)));
			Assert.IsTrue(bitList.GetRange(0, destinationIndex).Equals(bitList3.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(0, destinationIndex), E.Take(bitList3, destinationIndex)));
			Assert.IsTrue(bitList.GetRange(destinationIndex, length).Equals(bitList3.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex, length), E.Take(E.Skip(bitList3, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex - 1, length + 1).Equals(bitList3.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList3, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(bitList.GetRange(destinationIndex, length + 1).Equals(bitList3.GetRange(sourceIndex, length + 1)), E.SequenceEqual(bitList.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList3, sourceIndex), length + 1)));
			Assert.IsTrue(bitList.GetRange(destinationIndex + length).Equals(bitList3.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(bitList.GetRange(destinationIndex + length), E.Skip(bitList3, destinationIndex + length)));
		}
	}

	private BigList<int> bl = default!;
	private G.List<int> gl = default!;

	[TestMethod]
	public void ComplexTest2() => BaseBigListTests<int, BigList<int>, List<int>>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(32, _ => random.Next(16));
		bl = new(arr, 2, 2);
		gl = new(arr);
		var bytes = new byte[16];
		return (bl, gl, bytes);
	}, () => random.Next(16), 2);

	[TestMethod]
	public void TestCopy()
	{
		var bytes = new byte[1250];
		random.NextBytes(bytes);
		var length = 435;
		var sourceIndex = 123;
		var destinationIndex = 272;
		BigList<byte> destination, source;
		NList<byte> bitList3;
		PerformIteration();
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			length = random.Next(801);
			sourceIndex = random.Next(bytes.Length - length + 1);
			destinationIndex = random.Next(bytes.Length - length + 1);
			PerformIteration();
		}
		void PerformIteration()
		{
			bitList3 = new(bytes);
			destination = new(bitList3, random.Next(2, 5), random.Next(2, 7));
			source = new(destination, random.Next(2, 5), random.Next(2, 7));
			Assert.IsTrue(destination.Equals(source));
			source.CopyTo(sourceIndex, destination, destinationIndex, length);
			Assert.IsTrue(destination.GetRange(0, destinationIndex).Equals(source.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(0, destinationIndex), E.Take(source, destinationIndex)));
			Assert.IsTrue(destination.GetRange(destinationIndex, length).Equals(source.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(destinationIndex, length), E.Take(E.Skip(source, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(destination.GetRange(destinationIndex - 1, length + 1).Equals(source.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(source, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(destination.GetRange(destinationIndex, length + 1).Equals(source.GetRange(sourceIndex, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex, length + 1), E.Take(E.Skip(source, sourceIndex), length + 1)));
			Assert.IsTrue(destination.GetRange(destinationIndex + length).Equals(source.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(destinationIndex + length), E.Skip(source, destinationIndex + length)));
			Assert.IsTrue(destination.GetRange(0, destinationIndex).Equals(bitList3.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(0, destinationIndex), E.Take(bitList3, destinationIndex)));
			Assert.IsTrue(destination.GetRange(destinationIndex, length).Equals(bitList3.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(destinationIndex, length), E.Take(E.Skip(bitList3, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(destination.GetRange(destinationIndex - 1, length + 1).Equals(bitList3.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList3, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(destination.GetRange(destinationIndex, length + 1).Equals(bitList3.GetRange(sourceIndex, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList3, sourceIndex), length + 1)));
			Assert.IsTrue(destination.GetRange(destinationIndex + length).Equals(bitList3.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(destinationIndex + length), E.Skip(bitList3, destinationIndex + length)));
		}
	}
}
