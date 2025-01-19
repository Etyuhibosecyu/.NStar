using System.Threading;

namespace Corlib.NStar.Tests;

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
}
