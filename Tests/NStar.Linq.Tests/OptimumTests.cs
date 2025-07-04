﻿using NStar.MathLib;
using System.Numerics;

namespace NStar.Linq.Tests;

[TestClass]
public class OptimumTests
{
	private static readonly BigInteger power = new([175, 16, 6]), mod = new([134, 116, 171, 29, 108, 91, 58, 139, 101, 78, 109, 137, 45, 245, 203, 228, 21, 249, 235, 144, 21]);

	private static void OptimumTest(int iterations, Action<G.IEnumerable<MpzT>, ImmutableArray<BigInteger>> MainAction)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[21];
		var bytes2 = new List<byte[]>();
		for (var i = 0; i < iterations; i++)
		{
			var (list2, list3) = RedStarLinq.FillArray(random.Next(17), _ =>
			{
				if (bytes2.Length != 0 && random.Next(2) == 1)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			G.IEnumerable<MpzT> a = RedStarLinq.ToList(list2);
			MainAction(a, list3);
			a = RedStarLinq.ToArray(list2);
			MainAction(a, list3);
			a = E.ToList(list2);
			MainAction(a, list3);
			a = list2.ToList().Insert(0, 12345678901234567890).GetSlice(1);
			MainAction(a, list3);
			a = E.Select(list2, x => x);
			MainAction(a, list3);
			a = E.SkipWhile(list2, _ => random.Next(10) == -1);
			MainAction(a, list3);
			bytes2.Clear(false);
		}
		{
			var (list2, list3) = RedStarLinq.FillArray(16, _ =>
			{
				if (bytes2.Length != 0)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			G.IEnumerable<MpzT> a = RedStarLinq.ToList(list2);
			MainAction(a, list3);
			a = RedStarLinq.ToArray(list2);
			MainAction(a, list3);
			a = E.ToList(list2);
			MainAction(a, list3);
			a = list2.ToList().Insert(0, 12345678901234567890).GetSlice(1);
			MainAction(a, list3);
			a = E.Select(list2, x => x);
			MainAction(a, list3);
			a = E.SkipWhile(list2, _ => random.Next(10) == -1);
			MainAction(a, list3);
		}
	}

	private static void OptimumTestSpan(int iterations, Action<ReadOnlySpan<MpzT>, ImmutableArray<BigInteger>> MainAction)
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[21];
		var bytes2 = new List<byte[]>();
		for (var i = 0; i < iterations; i++)
		{
			var (list2, list3) = RedStarLinq.FillArray(random.Next(17), _ =>
			{
				if (bytes2.Length != 0 && random.Next(2) == 1)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			MainAction(RedStarLinq.ToArray(list2), list3);
			MainAction(RedStarLinq.ToArray(list2).AsSpan(), list3);
			MainAction((ReadOnlySpan<MpzT>)RedStarLinq.ToArray(list2).AsSpan(), list3);
			bytes2.Clear(false);
		}
		{
			var (list2, list3) = RedStarLinq.FillArray(16, _ =>
			{
				if (bytes2.Length != 0)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			MainAction(RedStarLinq.ToArray(list2), list3);
			MainAction(RedStarLinq.ToArray(list2).AsSpan(), list3);
			MainAction((ReadOnlySpan<MpzT>)RedStarLinq.ToArray(list2).AsSpan(), list3);
		}
	}

	[TestMethod]
	public void TestFindAllMax() => OptimumTest(1000, (a, list3) =>
	{
		var c = RedStarLinq.ToArray(a.FindAllMax(x => x.PowerMod(new MpzT(power), new(mod))), x => x.ToBigInteger());
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod))), x => x.ToBigInteger());
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax(x => (decimal)((double)x / 1000000000000 / 1000000000000)), x => x.ToBigInteger());
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), x => x.ToBigInteger());
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax(x => (double)x / 1000000000000), x => x.ToBigInteger());
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax((x, index) => (double)x / 1000000000000 * (index % 5)), x => x.ToBigInteger());
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax(x => (int)x), x => x.ToBigInteger());
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax((x, index) => (int)(x * (index % 5))), x => x.ToBigInteger());
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax(x => (uint)x), x => x.ToBigInteger());
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax((x, index) => (uint)(x * (index % 5))), x => x.ToBigInteger());
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax(x => (long)x), x => x.ToBigInteger());
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMax((x, index) => (long)(x * (index % 5))), x => x.ToBigInteger());
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
	});

	[TestMethod]
	public void TestFindAllMin() => OptimumTest(1000, (a, list3) =>
	{
		var c = RedStarLinq.ToArray(a.FindAllMin(x => x.PowerMod(new MpzT(power), new(mod))), x => x.ToBigInteger());
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod))), x => x.ToBigInteger());
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin(x => (decimal)((double)x / 1000000000000 / 1000000000000)), x => x.ToBigInteger());
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), x => x.ToBigInteger());
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin(x => (double)x / 1000000000000), x => x.ToBigInteger());
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin((x, index) => (double)x / 1000000000000 * (index % 5)), x => x.ToBigInteger());
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin(x => (int)x), x => x.ToBigInteger());
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin((x, index) => (int)(x * (index % 5))), x => x.ToBigInteger());
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin(x => (uint)x), x => x.ToBigInteger());
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin((x, index) => (uint)(x * (index % 5))), x => x.ToBigInteger());
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin(x => (long)x), x => x.ToBigInteger());
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = RedStarLinq.ToArray(a.FindAllMin((x, index) => (long)(x * (index % 5))), x => x.ToBigInteger());
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
	});

	[TestMethod]
	public void TestFindMax() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.FindMax(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MaxBy(list3, x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(list3, (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * x.index, mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(list3, (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * (x.index % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(list3, x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax((x, index) => (double)x / 1000000000000 * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(list3, (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * (x.index % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(list3, x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax((x, index) => (int)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(list3, (elem, index) => (elem, index)), x => (int)(uint)(x.elem * (x.index % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(list3, x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax((x, index) => (uint)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(list3, (elem, index) => (elem, index)), x => (uint)(x.elem * (x.index % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMax((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(list3, (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * (x.index % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * (x.index % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestFindMin() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.FindMin(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * x.index, mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * (x.index % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (double)x / 1000000000000 * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * (x.index % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (int)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (int)(uint)(x.elem * (x.index % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (uint)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (uint)(x.elem * (x.index % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * (x.index % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * (x.index % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestIndexesOfMax() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.IndexesOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
	});

	[TestMethod]
	public void TestIndexesOfMean() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.IndexesOfMean();
		var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
		var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean(x => (int)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean((x, index) => (int)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean(x => (uint)x);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean((x, index) => (uint)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean(x => (long)x);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMean((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
	});

	[TestMethod]
	public void TestIndexesOfMin() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.IndexesOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
		c = a.IndexesOfMin((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(c, d));
		Assert.IsTrue(E.SequenceEqual(d, c));
	});

	[TestMethod]
	public void TestIndexOfMax() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.IndexOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestIndexOfMean() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.IndexOfMean();
		var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
		var d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean(x => (int)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean((x, index) => (int)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean(x => (uint)x);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean((x, index) => (uint)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean(x => (long)x);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMean((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestIndexOfMin() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.IndexOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.IndexOfMin((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		d = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestLastIndexOfMax() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.LastIndexOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestLastIndexOfMean() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.LastIndexOfMean();
		var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
		var d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean(x => (int)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean((x, index) => (int)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean(x => (uint)x);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean((x, index) => (uint)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean(x => (long)x);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMean((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestLastIndexOfMin() => OptimumTest(1000, (a, list3) =>
	{
		var c = a.LastIndexOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
		c = a.LastIndexOfMin((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		d = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(d, c);
	});

	[TestMethod]
	public void TestMax() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.Max();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Max(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Max((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Max(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Max((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Max(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Max((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Max(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Max((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Max(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Max((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Max(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Max((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		var @string = a.Max(x => x.ToString());
		var stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = a.Max((x, index) => (x * (index % 5)).ToString());
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
	});

	[TestMethod]
	public void TestMaxSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var mpzT = a.Max();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Max(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Max((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Max(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Max((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Max(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Max((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Max(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Max((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Max(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Max((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Max(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Max((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestMean() => OptimumTest(10000, (a, list3) =>
	{
		var @double = a.Mean();
		var doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => x.PowerMod(new MpzT(power), new(mod)));
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		var @decimal = a.Mean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Mean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.Mean(x => (double)x / 1000000000000);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => (int)x);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (int)(x * (index % 5)));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => (uint)x);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (uint)(x * (index % 5)));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => (long)x);
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (long)(x * (index % 5)));
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
	});

	[TestMethod]
	public void TestMeanSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var @double = a.Mean();
		var doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => x.PowerMod(new MpzT(power), new(mod)));
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		var @decimal = a.Mean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Mean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.Mean(x => (double)x / 1000000000000);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => (int)x);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (int)(x * (index % 5)));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => (uint)x);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (uint)(x * (index % 5)));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean(x => (long)x);
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Mean((x, index) => (long)(x * (index % 5)));
		doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		Assert.AreEqual(doubleOptimum, @double);
	});

	[TestMethod]
	public void TestMedian() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.Median();
		var optimum = list3.Length == 0 ? 0 : E.Order(list3).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Median(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => BigInteger.ModPow(x, power, mod))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Median((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Median(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Median((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Median(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (double)x / 1000000000000)).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Median((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Median(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (int)(uint)(x % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Median((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Median(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (uint)(x % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Median((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Median(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Median((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(longOptimum, @long);
		var @string = a.Median(x => x.ToString());
		var stringOptimum = list3.Length == 0 ? null : E.Order(E.Select(list3, x => x.ToString())).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : null);
		Assert.AreEqual(stringOptimum, @string);
		@string = a.Median((x, index) => (x * (index % 5)).ToString());
		stringOptimum = list3.Length == 0 ? null : E.Order(E.Select(list3, (x, index) => (x * (index % 5)).ToString())).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : null);
		Assert.AreEqual(stringOptimum, @string);
	});

	[TestMethod]
	public void TestMedianSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var mpzT = a.Median();
		var optimum = list3.Length == 0 ? 0 : E.Order(list3).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Median(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => BigInteger.ModPow(x, power, mod))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Median((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Median(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Median((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Median(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (double)x / 1000000000000)).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Median((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Median(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (int)(uint)(x % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Median((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Median(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (uint)(x % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Median((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Median(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Median((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Order(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296))).Wrap(x => E.Any(x) ? E.ElementAt(x, (E.Count(x) - 1) / 2) : 0);
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestMin() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.Min();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Min(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Min((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Min(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Min((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Min(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Min((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Min(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Min((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Min(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Min((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Min(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Min((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		var @string = a.Min(x => x.ToString());
		var stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = a.Min((x, index) => (x * (index % 5)).ToString());
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
	});

	[TestMethod]
	public void TestMinSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var mpzT = a.Min();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Min(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Min((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Min(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Min((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Min(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Min((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Min(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Min((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Min(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Min((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Min(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Min((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestPDirect()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var bytes = new byte[21];
		var bytes2 = new List<byte[]>();
		for (var i = 0; i < 10000; i++)
		{
			var (list2, list3) = RedStarLinq.FillArray(random.Next(0, 17), _ =>
			{
				if (bytes2.Length != 0 && random.Next(2) == 1)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			G.IReadOnlyList<MpzT> a = RedStarLinq.ToList(list2);
			ProcessA(a, list3);
			a = RedStarLinq.ToArray(list2);
			ProcessA(a, list3);
			a = E.ToList(list2);
			ProcessA(a, list3);
			a = list2.ToList().Insert(0, 12345678901234567890).GetSlice(1);
			ProcessA(a, list3);
			bytes2.Clear(false);
		}
		{
			var (list2, list3) = RedStarLinq.FillArray(16, _ =>
			{
				if (bytes2.Length != 0)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			G.IReadOnlyList<MpzT> a = RedStarLinq.ToList(list2);
			ProcessA(a, list3);
			a = RedStarLinq.ToArray(list2);
			ProcessA(a, list3);
			a = E.ToList(list2);
			ProcessA(a, list3);
			a = list2.ToList().Insert(0, 12345678901234567890).GetSlice(1);
			ProcessA(a, list3);
		}
		static void ProcessA(G.IReadOnlyList<MpzT> a, ImmutableArray<BigInteger> list3)
		{
			//var mpzT = a.PMax();
			//var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			BigInteger power = new([175, 16, 6]), mod = new([134, 116, 171, 29, 108, 91, 58, 139, 101, 78, 109, 137, 45, 245, 203, 228, 21, 249, 235, 144, 21]);
			var mpzT = a.PMax(x => x.PowerMod(new MpzT(power), new(mod)));
			var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.PMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			var @decimal = a.PMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
			Assert.AreEqual(decimalOptimum, @decimal);
			@decimal = a.PMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
			Assert.AreEqual(decimalOptimum, @decimal);
			var @double = a.PMax(x => (double)x / 1000000000000);
			var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
			Assert.AreEqual(doubleOptimum, @double);
			@double = a.PMax((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
			Assert.AreEqual(doubleOptimum, @double);
			var @int = a.PMax(x => (int)x);
			var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
			Assert.AreEqual(intOptimum, @int);
			@int = a.PMax((x, index) => (int)(x * (index % 5)));
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
			Assert.AreEqual(intOptimum, @int);
			var @uint = a.PMax(x => (uint)x);
			var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
			Assert.AreEqual(uintOptimum, @uint);
			@uint = a.PMax((x, index) => (uint)(x * (index % 5)));
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
			Assert.AreEqual(uintOptimum, @uint);
			var @long = a.PMax(x => (long)x);
			var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
			Assert.AreEqual(longOptimum, @long);
			@long = a.PMax((x, index) => (long)(x * (index % 5)));
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
			Assert.AreEqual(longOptimum, @long);
			//mpzT = a.PMin();
			//optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.PMin(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.PMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			@decimal = a.PMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
			Assert.AreEqual(decimalOptimum, @decimal);
			@decimal = a.PMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
			Assert.AreEqual(decimalOptimum, @decimal);
			@double = a.PMin(x => (double)x / 1000000000000);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
			Assert.AreEqual(doubleOptimum, @double);
			@double = a.PMin((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
			Assert.AreEqual(doubleOptimum, @double);
			@int = a.PMin(x => (int)x);
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
			Assert.AreEqual(intOptimum, @int);
			@int = a.PMin((x, index) => (int)(x * (index % 5)));
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
			Assert.AreEqual(intOptimum, @int);
			@uint = a.PMin(x => (uint)x);
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
			Assert.AreEqual(uintOptimum, @uint);
			@uint = a.PMin((x, index) => (uint)(x * (index % 5)));
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
			Assert.AreEqual(uintOptimum, @uint);
			@long = a.PMin(x => (long)x);
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
			Assert.AreEqual(longOptimum, @long);
			@long = a.PMin((x, index) => (long)(x * (index % 5)));
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
			Assert.AreEqual(longOptimum, @long);
			//mpzT = a.Product();
			//optimum = list3.Length == 0 ? 1 : E.Aggregate(list3, (BigInteger)1, (x, y) => x * y);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			//mpzT = a.Product(x => x.PowerMod(new MpzT(power), new(mod)));
			//optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)1, (x, y) => x * y);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			//mpzT = a.Product((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			//optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)1, (x, y) => x * y);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			//@decimal = a.Product(x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)));
			//decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))), 1m, (x, y) => x * y);
			//Assert.IsTrue(@decimal == decimalOptimum);
			//@decimal = a.Product((x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1));
			//decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)), 1m, (x, y) => x * y);
			//Assert.IsTrue(@decimal == decimalOptimum);
			//@double = a.Product(x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5));
			//doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)), 1d, (x, y) => x * y);
			//Assert.IsTrue(@double == doubleOptimum);
			//@double = a.Product((x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1));
			//doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)), 1d, (x, y) => x * y);
			//Assert.IsTrue(@double == doubleOptimum);
			//@int = a.Product(x => (int)x);
			//intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 1, (x, y) => x * y);
			//Assert.IsTrue(@int == intOptimum);
			//@int = a.Product((x, index) => (int)(x * (index % 5 + 1)));
			//intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5 + 1) % 4294967296)), 1, (x, y) => x * y);
			//Assert.IsTrue(@int == intOptimum);
			//@uint = a.Product(x => (uint)x);
			//uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 1u, (x, y) => x * y);
			//Assert.IsTrue(@uint == uintOptimum);
			//@uint = a.Product((x, index) => (uint)(x * (index % 5 + 1)));
			//uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5 + 1) % 4294967296)), 1u, (x, y) => x * y);
			//Assert.IsTrue(@uint == uintOptimum);
			//@long = a.Product(x => (long)x);
			//longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 1L, (x, y) => x * y);
			//Assert.IsTrue(@long == longOptimum);
			//@long = a.Product((x, index) => (long)(x * (index % 5 + 1)));
			//longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5 + 1) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5 + 1) % 4294967296)), 1L, (x, y) => x * y);
			//Assert.IsTrue(@long == longOptimum);
			//mpzT = a.Sum();
			//optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, (BigInteger)0, (x, y) => x + y);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			//mpzT = a.Sum(x => x.PowerMod(new MpzT(power), new(mod)));
			//optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			//mpzT = a.Sum((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			//optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
			//Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			//@decimal = a.Sum(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			//decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
			//Assert.IsTrue(@decimal == decimalOptimum);
			//@decimal = a.Sum((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			//decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
			//Assert.IsTrue(@decimal == decimalOptimum);
			//@double = a.Sum(x => (double)x / 1000000000000);
			//doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
			//Assert.IsTrue(@double == doubleOptimum);
			//@double = a.Sum((x, index) => (double)x / 1000000000000 * (index % 5));
			//doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
			//Assert.IsTrue(@double == doubleOptimum);
			//@int = a.Sum(x => (int)x);
			//intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
			//Assert.IsTrue(@int == intOptimum);
			//@int = a.Sum((x, index) => (int)(x * (index % 5)));
			//intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
			//Assert.IsTrue(@int == intOptimum);
			//@uint = a.Sum(x => (uint)x);
			//uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
			//Assert.IsTrue(@uint == uintOptimum);
			//@uint = a.Sum((x, index) => (uint)(x * (index % 5)));
			//uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
			//Assert.IsTrue(@uint == uintOptimum);
			//@long = a.Sum(x => (long)x);
			//longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
			//Assert.IsTrue(@long == longOptimum);
			//@long = a.Sum((x, index) => (long)(x * (index % 5)));
			//longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
			//Assert.IsTrue(@long == longOptimum);
		}
	}

	[TestMethod]
	public void TestProduct() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.Product();
		var optimum = list3.Length == 0 ? 1 : E.Aggregate(list3, (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Product(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Product((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Product(x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)));
		var decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Product((x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1));
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Product(x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5));
		var doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Product((x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1));
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Product(x => (int)x);
		var intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Product((x, index) => (int)(x * (index % 5 + 1)));
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5 + 1) % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Product(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Product((x, index) => (uint)(x * (index % 5 + 1)));
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5 + 1) % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Product(x => (long)x);
		var longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Product((x, index) => (long)(x * (index % 5 + 1)));
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5 + 1) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5 + 1) % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestSum() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.Sum();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Sum(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Sum((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Sum(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Sum((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Sum(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Sum((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Sum(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Sum((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Sum(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Sum((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Sum(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Sum((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
	});
}
