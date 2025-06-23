global using Corlib.NStar;
global using Corlib.NStar.Tests;
global using LINQ.NStar;
global using Mpir.NET;
global using System;
global using System.Collections;
global using System.Threading;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static Corlib.NStar.Tests.Global;
global using static System.Math;

namespace BigCollections.NStar.Tests;

public static class BaseBigListTests<T, TCertain, TLow> where TCertain : BigList<T, TCertain, TLow>, new() where TLow : BaseList<T, TLow>, new()
{
	public static void ComplexTest(Func<(BigList<T, TCertain, TLow>, G.List<T>, byte[])> create, Func<T> newValueFunc, int multiplier, int repeats)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		var toInsert = Array.Empty<T>();
	l1:
		var (bl, gl, bytes) = create();
		var secondaryActions = new[] { () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.AddRange(toInsert);
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.AddRange(RedStarLinq.ToList(toInsert));
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var toInsert2 = (TCertain)bl.GetType().GetConstructor([typeof(G.IEnumerable<T>), typeof(int), typeof(int)])?.Invoke([toInsert, -1, -1])!;
			var bl2 = bl.AddRange(toInsert2);
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var toInsert2 = ((TCertain)bl.GetType().GetConstructor([typeof(G.IEnumerable<T>), typeof(int), typeof(int)])?.Invoke([toInsert, -1, -1])!).Reverse();
			var bl2 = bl.AddRange(toInsert2);
			gl.AddRange(toInsert.Reverse());
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.AddRange(E.Select(toInsert, x => x));
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.AddRange(E.SkipWhile(toInsert, _ => random.Next(10) == -1));
			gl.AddRange(toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.Insert(index, toInsert);
			gl.InsertRange(index, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.Insert(index, RedStarLinq.ToList(toInsert));
			gl.InsertRange(index, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var toInsert2 = (TCertain)bl.GetType().GetConstructor([typeof(G.IEnumerable<T>), typeof(int), typeof(int)])?.Invoke([toInsert, -1, -1])!;
			var bl2 = bl.Insert(index, toInsert2);
			gl.InsertRange(index, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var toInsert2 = ((TCertain)bl.GetType().GetConstructor([typeof(G.IEnumerable<T>), typeof(int), typeof(int)])?.Invoke([toInsert, -1, -1])!).Reverse();
			var bl2 = bl.Insert(index, toInsert2);
			gl.InsertRange(index, toInsert.Reverse());
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.Insert(index, E.Select(toInsert, x => x));
			gl.InsertRange(index, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length);
			toInsert = RedStarLinq.FillArray(random.Next(multiplier * 5 + 1), _ => newValueFunc());
			var bl2 = bl.Insert(index, E.SkipWhile(toInsert, _ => random.Next(10) == -1));
			gl.InsertRange(index, toInsert);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var length = Min(random.Next(multiplier * 8 + 1), (int)bl.Length);
			if (bl.Length < length)
				return;
			var start = random.Next((int)bl.Length - length + 1);
			bl.Clear(start, length);
			gl.SetAll(default!, start, length);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var n = newValueFunc();
			Assert.AreEqual(gl.Contains(n), bl.Contains(n));
			Assert.AreEqual(gl.IndexOf(n), bl.IndexOf(n));
			Assert.AreEqual(gl.LastIndexOf(n), bl.LastIndexOf(n));
		}, () =>
		{
			if (bl.Length == 0)
				return;
			var index = random.Next((int)bl.Length);
			var n = newValueFunc();
			Assert.AreEqual(gl.IndexOf(n, index) >= 0, bl.Contains(n, index));
			Assert.AreEqual(gl.IndexOf(n, index), bl.IndexOf(n, index));
			Assert.AreEqual(gl.LastIndexOf(n, index), bl.LastIndexOf(n, index));
		}, () =>
		{
			var n = newValueFunc();
			var length = Min(random.Next(multiplier * 8 + 1), (int)bl.Length);
			if (length == 0)
				return;
			var start = random.Next((int)bl.Length - length + 1);
			Assert.AreEqual(gl.IndexOf(n, start, length) >= 0, bl.Contains(n, start, length));
			Assert.AreEqual(gl.IndexOf(n, start, length), bl.IndexOf(n, start, length));
			Assert.AreEqual(gl.LastIndexOf(n, gl.Count - 1 - start, length), bl.LastIndexOf(n, bl.Length - 1 - start, length));
		} };
		var actions = new[] { () =>
		{
			var n = newValueFunc();
			var bl2 = bl.Add(n);
			gl.Add(n);
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var index = random.Next((int)bl.Length + 1);
			var n = newValueFunc();
			var bl2 = bl.Insert(index, n);
			gl.Insert(index, n);
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			if (bl.Length == 0) return;
			var index = random.Next((int)bl.Length);
			var bl2 = bl.RemoveAt(index);
			gl.RemoveAt(index);
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			if (bl.Length == 0) return;
			var n = newValueFunc();
			var removed = bl.RemoveValue(n);
			Assert.AreEqual(removed, gl.Remove(n));
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			secondaryActions.Random(random)();
			Assert.IsTrue(RedStarLinq.Equals(bl, gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
		}, () =>
		{
			var length = Min(random.Next(multiplier * 8 + 1), (int)bl.Length);
			if (bl.Length < length)
				return;
			var start = random.Next((int)bl.Length - length + 1);
			var bl2 = bl.Remove(start, length);
			gl.RemoveRange(start, length);
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
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
			Assert.AreEqual(bl2, bl);
		}, () =>
		{
			var bl2 = bl.Reverse();
			gl.Reverse();
			Assert.IsTrue(bl.Equals(gl));
			Assert.IsTrue(E.SequenceEqual(gl, bl));
			Assert.AreEqual(bl2, bl);
		//}, () =>
		//{
		//	var length = Min(random.Next(multiplier * 8 + 1), (int)bl.Length);
		//	if (bl.Length < length)
		//		return;
		//	var start = random.Next((int)bl.Length - length + 1);
		//	var bl2 = bl.Reverse(start, length);
		//	gl.Reverse(start, length);
		//	Assert.IsTrue(bl.Equals(gl));
		//	Assert.IsTrue(E.SequenceEqual(gl, bl));
		//	Assert.AreEqual(bl2, bl);
		}, () =>
		{
			if (bl.Length == 0) return;
			var index = random.Next((int)bl.Length);
			var n = newValueFunc();
			bl[index] = n;
			gl[index] = n;
			Assert.IsTrue(bl.Equals(gl));
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
		if (counter++ < repeats)
			goto l1;
	}
}

[TestClass]
public class BigBitListTests
{
	private BigBitList bl = default!;
	private G.List<bool> gl = default!;

	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseBigListTests<bool, BigBitList, BitList>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(512, _ => random.Next(2) == 1);
		bl = new(arr, 2, 6);
		gl = [.. arr];
		var bytes = new byte[16];
		return (bl, gl, bytes);
	}, () => random.Next(2) == 1, BitsPerInt, 250);
	}

	[TestMethod]
	public void ConstructionTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
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
			G.List<bool> gl = [.. array[i]];
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
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[40];
		random.NextBytes(bytes);
		var length = 10;
		var sourceIndex = 36;
		var destinationIndex = 61;
		BigBitList modified, original;
		BitList bitList;
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
			bitList = new(bytes);
			modified = new(bitList, 2, 6);
			original = new(modified, 2, 6);
			modified.CopyTo(sourceIndex, modified, destinationIndex, length);
			Assert.IsTrue(modified.GetRange(0, destinationIndex).Equals(original.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(0, destinationIndex), E.Take(original, destinationIndex)));
			Assert.IsTrue(modified.GetRange(destinationIndex, length).Equals(original.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex, length), E.Take(E.Skip(original, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(modified.GetRange(destinationIndex - 1, length + 1).Equals(original.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(original, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length * BitsPerByte - 1 && destinationIndex + length < bytes.Length * BitsPerByte - 1)
				Assert.AreEqual(modified.GetRange(destinationIndex, length + 1).Equals(original.GetRange(sourceIndex, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex, length + 1), E.Take(E.Skip(original, sourceIndex), length + 1)));
			Assert.IsTrue(modified.GetRange(destinationIndex + length).Equals(original.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex + length), E.Skip(original, destinationIndex + length)));
			Assert.IsTrue(modified.GetRange(0, destinationIndex).Equals(bitList.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(0, destinationIndex), E.Take(bitList, destinationIndex)));
			Assert.IsTrue(modified.GetRange(destinationIndex, length).Equals(bitList.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex, length), E.Take(E.Skip(bitList, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(modified.GetRange(destinationIndex - 1, length + 1).Equals(bitList.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length * BitsPerByte - 1 && destinationIndex + length < bytes.Length * BitsPerByte - 1)
				Assert.AreEqual(modified.GetRange(destinationIndex, length + 1).Equals(bitList.GetRange(sourceIndex, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList, sourceIndex), length + 1)));
			Assert.IsTrue(modified.GetRange(destinationIndex + length).Equals(bitList.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex + length), E.Skip(bitList, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestCopyPro()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[1250];
		random.NextBytes(bytes);
		var length = 435;
		var sourceIndex = 123;
		var destinationIndex = 272;
		BigBitList destination, source;
		BitList bitList;
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
			bitList = new(bytes);
			destination = new(bitList, random.Next(2, 5), random.Next(5, 11));
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
	private BigList<int> bl = default!;
	private G.List<int> gl = default!;

	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseBigListTests<int, BigList<int>, List<int>>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(32, _ => random.Next(16));
		bl = new(arr, 2, 2);
		gl = [.. arr];
		var bytes = new byte[16];
		return (bl, gl, bytes);
	}, () => random.Next(16), 2, 2500);
	}

	[TestMethod]
	public void ComplexTestPro()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		BaseBigListTests<int, BigList<int>, List<int>>.ComplexTest(() =>
	{
		var arr = RedStarLinq.FillArray(32, _ => random.Next(16));
		bl = new(arr, 2, 2);
		gl = [.. arr];
		var bytes = new byte[16];
		return (bl, gl, bytes);
	}, () => random.Next(16), 32, 100);
	}

	[TestMethod]
	public void TestCopy()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[40];
		random.NextBytes(bytes);
		var length = 2;
		var sourceIndex = 5;
		var destinationIndex = 9;
		BigList<byte> modified, original;
		NList<byte> bitList;
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
			bitList = new(bytes);
			modified = new(bitList, 2, 2);
			original = new(modified, 2, 2);
			modified.CopyTo(sourceIndex, modified, destinationIndex, length);
			Assert.IsTrue(modified.GetRange(0, destinationIndex).Equals(original.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(0, destinationIndex), E.Take(original, destinationIndex)));
			Assert.IsTrue(modified.GetRange(destinationIndex, length).Equals(original.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex, length), E.Take(E.Skip(original, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(modified.GetRange(destinationIndex - 1, length + 1).Equals(original.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(original, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(modified.GetRange(destinationIndex, length + 1).Equals(original.GetRange(sourceIndex, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex, length + 1), E.Take(E.Skip(original, sourceIndex), length + 1)));
			Assert.IsTrue(modified.GetRange(destinationIndex + length).Equals(original.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex + length), E.Skip(original, destinationIndex + length)));
			Assert.IsTrue(modified.GetRange(0, destinationIndex).Equals(bitList.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(0, destinationIndex), E.Take(bitList, destinationIndex)));
			Assert.IsTrue(modified.GetRange(destinationIndex, length).Equals(bitList.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex, length), E.Take(E.Skip(bitList, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(modified.GetRange(destinationIndex - 1, length + 1).Equals(bitList.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(bitList, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < bytes.Length - 1 && destinationIndex + length < bytes.Length - 1)
				Assert.AreEqual(modified.GetRange(destinationIndex, length + 1).Equals(bitList.GetRange(sourceIndex, length + 1)), E.SequenceEqual(modified.GetRange(destinationIndex, length + 1), E.Take(E.Skip(bitList, sourceIndex), length + 1)));
			Assert.IsTrue(modified.GetRange(destinationIndex + length).Equals(bitList.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(modified.GetRange(destinationIndex + length), E.Skip(bitList, destinationIndex + length)));
		}
	}

	[TestMethod]
	public void TestCopyPro()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[2500];
		random.NextBytes(bytes);
		NList<byte> regularList = new(bytes);
		regularList.Resize(1250);
		var length = 435;
		var sourceIndex = 123;
		var destinationIndex = 272;
		BigList<byte> destination, source;
		bool reverseSource = false, reverseDestination = false;
		PerformIteration();
		for (var i = 0; i < 1000; i++)
		{
			random.NextBytes(bytes);
			regularList = new(bytes);
			regularList.Resize(i % 2 == 1 ? (int)Round(Exp(random.NextDouble() * Log(regularList.Length + 1))) : random.Next(regularList.Length + 1));
			length = random.Next(Min(regularList.Length, 800) + 1);
			sourceIndex = random.Next(regularList.Length - length + 1);
			destinationIndex = random.Next(regularList.Length - length + 1);
			reverseSource = random.Next(2) == 1;
			reverseDestination = random.Next(2) == 1;
			PerformIteration();
		}
		void PerformIteration()
		{
			destination = new(regularList, random.Next(2, 5), random.Next(2, 7));
			source = new(destination, random.Next(2, 5), random.Next(2, 7));
			Assert.IsTrue(destination.Equals(source));
			if (reverseSource)
			{
				source.Reverse();
				sourceIndex = (int)source.Length - length - sourceIndex;
			}
			if (reverseDestination)
			{
				destination.Reverse();
				destinationIndex = (int)destination.Length - length - destinationIndex;
			}
			source.CopyTo(sourceIndex, destination, destinationIndex, length);
			if (reverseSource)
			{
				source.Reverse();
				sourceIndex = (int)source.Length - length - sourceIndex;
			}
			if (reverseDestination)
			{
				destination.Reverse();
				destinationIndex = (int)destination.Length - length - destinationIndex;
			}
			Assert.IsTrue(destination.GetRange(0, destinationIndex).Equals(source.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(0, destinationIndex), E.Take(source, destinationIndex)));
			Assert.IsTrue(Process(destination.GetRange(destinationIndex, length)).Equals(source.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(Process(destination.GetRange(destinationIndex, length)),
				E.Take(E.Skip(source, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(Process(destination.GetRange(destinationIndex - 1, length + 1))
					.Equals(source.GetRange(sourceIndex - 1, length + 1)),
					E.SequenceEqual(Process(destination.GetRange(destinationIndex - 1, length + 1)),
					E.Take(E.Skip(source, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < regularList.Length - 1 && destinationIndex + length < regularList.Length - 1)
				Assert.AreEqual(Process(destination.GetRange(destinationIndex, length + 1))
					.Equals(source.GetRange(sourceIndex, length + 1)),
					E.SequenceEqual(Process(destination.GetRange(destinationIndex, length + 1)),
					E.Take(E.Skip(source, sourceIndex), length + 1)));
			Assert.IsTrue(destination.GetRange(destinationIndex + length).Equals(source.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(destinationIndex + length),
				E.Skip(source, destinationIndex + length)));
			Assert.IsTrue(destination.GetRange(0, destinationIndex).Equals(regularList.GetRange(0, destinationIndex)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(0, destinationIndex), E.Take(regularList, destinationIndex)));
			Assert.IsTrue(Process(destination.GetRange(destinationIndex, length))
				.Equals(regularList.GetRange(sourceIndex, length)));
			Assert.IsTrue(E.SequenceEqual(Process(destination.GetRange(destinationIndex, length)),
				E.Take(E.Skip(regularList, sourceIndex), length)));
			if (sourceIndex >= 1 && destinationIndex >= 1)
				Assert.AreEqual(destination.GetRange(destinationIndex - 1, length + 1).Equals(regularList.GetRange(sourceIndex - 1, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex - 1, length + 1), E.Take(E.Skip(regularList, sourceIndex - 1), length + 1)));
			if (sourceIndex + length < regularList.Length - 1 && destinationIndex + length < regularList.Length - 1)
				Assert.AreEqual(destination.GetRange(destinationIndex, length + 1).Equals(regularList.GetRange(sourceIndex, length + 1)), E.SequenceEqual(destination.GetRange(destinationIndex, length + 1), E.Take(E.Skip(regularList, sourceIndex), length + 1)));
			Assert.IsTrue(destination.GetRange(destinationIndex + length).Equals(regularList.GetRange(destinationIndex + length)));
			Assert.IsTrue(E.SequenceEqual(destination.GetRange(destinationIndex + length), E.Skip(regularList, destinationIndex + length)));
		}
		BigList<byte> Process(BigList<byte> input) => reverseSource != reverseDestination ? new BigList<byte>(input).Reverse() : input;
	}
}
