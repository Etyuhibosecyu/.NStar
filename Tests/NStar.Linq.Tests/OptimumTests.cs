using NStar.MathLib;
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
				if (bytes2.Length != 0 && random.Next(2) == 0)
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
				if (bytes2.Length != 0 && random.Next(2) == 0)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			MainAction(RedStarLinq.ToArray(list2), list3);
			MainAction(RedStarLinq.ToArray(list2).AsSpan(), list3);
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
		}
	}

	private static void ProcessIndexesOf<T>(G.IEnumerable<MpzT> a, ImmutableArray<BigInteger> list,
		Func<MpzT, int, T> selector, Func<BigInteger, int, T> selector2,
		Func<BigInteger, int, (T elem, int index)> selector3, Func<T, T, T> aggregator,
		Func<G.IEnumerable<T>, List<int>> resultSelector) where T : INumber<T>
	{
		var b = resultSelector(a.ToArray(selector));
		var optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		var c = E.ToArray(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = resultSelector(a.ToList(selector));
		optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		c = E.ToArray(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = resultSelector(E.ToList(E.Select(a, selector)));
		optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		c = E.ToArray(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = resultSelector(E.Select(a, selector));
		optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		c = E.ToArray(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	}

	private static void ProcessIndexOf<T>(G.IEnumerable<MpzT> a, ImmutableArray<BigInteger> list,
		Func<MpzT, int, T> selector, Func<BigInteger, int, T> selector2,
		Func<BigInteger, int, (T elem, int index)> selector3, Func<T, T, T> aggregator,
		Func<G.IEnumerable<T>, int> resultSelector, Func<G.IEnumerable<int>, int, int> resultSelector2) where T : INumber<T>
	{
		var b = resultSelector(a.ToArray(selector));
		var optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		var c = resultSelector2(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = resultSelector(a.ToList(selector));
		optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		c = resultSelector2(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = resultSelector(E.ToList(E.Select(a, selector)));
		optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		c = resultSelector2(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = resultSelector(E.Select(a, selector));
		optimum = list.Length == 0 ? T.Zero : E.Aggregate(E.Select(list, selector2), aggregator);
		c = resultSelector2(E.Select(E.Where(E.Select(list, selector3), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
	}

	[TestMethod]
	public void TestFindAllMax() => OptimumTest(1000, (a, list3) =>
	{
		var b = RedStarLinq.ToArray(a.FindAllMax(x => x.PowerMod(new MpzT(power), new(mod))), x => x.ToBigInteger());
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod))), x => x.ToBigInteger());
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (decimal)((double)x / 1000000000000 / 1000000000000)), x => x.ToBigInteger());
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), x => x.ToBigInteger());
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (double)x / 1000000000000), x => x.ToBigInteger());
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (double)x / 1000000000000 * (index % 5)), x => x.ToBigInteger());
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (int)x), x => x.ToBigInteger());
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (int)(x * (index % 5))), x => x.ToBigInteger());
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (uint)x), x => x.ToBigInteger());
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (uint)(x * (index % 5))), x => x.ToBigInteger());
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (long)x), x => x.ToBigInteger());
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (long)(x * (index % 5))), x => x.ToBigInteger());
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestFindAllMaxSpan() => OptimumTestSpan(1000, (a, list3) =>
	{
		var b = RedStarLinq.ToArray(a.FindAllMax(x => x.PowerMod(new MpzT(power), new(mod))), x => x.ToBigInteger());
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod))), x => x.ToBigInteger());
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (decimal)((double)x / 1000000000000 / 1000000000000)), x => x.ToBigInteger());
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), x => x.ToBigInteger());
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (double)x / 1000000000000), x => x.ToBigInteger());
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (double)x / 1000000000000 * (index % 5)), x => x.ToBigInteger());
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (int)x), x => x.ToBigInteger());
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (int)(x * (index % 5))), x => x.ToBigInteger());
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (uint)x), x => x.ToBigInteger());
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (uint)(x * (index % 5))), x => x.ToBigInteger());
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax(x => (long)x), x => x.ToBigInteger());
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMax((x, index) => (long)(x * (index % 5))), x => x.ToBigInteger());
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestFindAllMin() => OptimumTest(1000, (a, list3) =>
	{
		var b = RedStarLinq.ToArray(a.FindAllMin(x => x.PowerMod(new MpzT(power), new(mod))), x => x.ToBigInteger());
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod))), x => x.ToBigInteger());
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (decimal)((double)x / 1000000000000 / 1000000000000)), x => x.ToBigInteger());
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), x => x.ToBigInteger());
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (double)x / 1000000000000), x => x.ToBigInteger());
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), x => x.ToBigInteger());
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (int)x), x => x.ToBigInteger());
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (int)(x * ((index + 1) % 5))), x => x.ToBigInteger());
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (uint)x), x => x.ToBigInteger());
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (uint)(x * ((index + 1) % 5))), x => x.ToBigInteger());
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (long)x), x => x.ToBigInteger());
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (long)(x * ((index + 1) % 5))), x => x.ToBigInteger());
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestFindAllMinSpan() => OptimumTestSpan(1000, (a, list3) =>
	{
		var b = RedStarLinq.ToArray(a.FindAllMin(x => x.PowerMod(new MpzT(power), new(mod))), x => x.ToBigInteger());
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod))), x => x.ToBigInteger());
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (decimal)((double)x / 1000000000000 / 1000000000000)), x => x.ToBigInteger());
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), x => x.ToBigInteger());
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (double)x / 1000000000000), x => x.ToBigInteger());
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), x => x.ToBigInteger());
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (int)x), x => x.ToBigInteger());
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (int)(x * ((index + 1) % 5))), x => x.ToBigInteger());
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (uint)x), x => x.ToBigInteger());
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (uint)(x * ((index + 1) % 5))), x => x.ToBigInteger());
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin(x => (long)x), x => x.ToBigInteger());
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = RedStarLinq.ToArray(a.FindAllMin((x, index) => (long)(x * ((index + 1) % 5))), x => x.ToBigInteger());
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => list3[x.index]));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestFindLastMax() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.FindLastMax(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * (list3.Length - 1 - x.index), mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * ((list3.Length - 1 - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (double)x / 1000000000000 * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * ((list3.Length - 1 - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (int)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (int)(uint)(x.elem * ((list3.Length - 1 - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (uint)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (uint)(x.elem * ((list3.Length - 1 - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * ((list3.Length - 1 - x.index) % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * ((list3.Length - 1 - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestFindLastMaxIndex() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.FindLastMaxIndex(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindLastMaxIndex((x, index) => x.PowerMod(new MpzT(power * index), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindLastMaxIndex(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindLastMaxIndex((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMaxIndex(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindLastMaxIndex((x, index) => (double)x / 1000000000000 * (index % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMaxIndex(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(intOptimum, @int);
		b = a.FindLastMaxIndex((x, index) => (int)(x * (index % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMaxIndex(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindLastMaxIndex((x, index) => (uint)(x * (index % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMaxIndex(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(longOptimum, @long);
		b = a.FindLastMaxIndex((x, index) => (long)(x * (index % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
	});

	[TestMethod]
	public void TestFindLastMaxSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var mpzT = a.FindLastMax(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * (list3.Length - 1 - x.index), mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * ((list3.Length - 1 - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (double)x / 1000000000000 * (index % 5));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * ((list3.Length - 1 - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (int)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (int)(uint)(x.elem * ((list3.Length - 1 - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (uint)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (uint)(x.elem * ((list3.Length - 1 - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Reverse(list3), x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMax((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.MaxBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * ((list3.Length - 1 - x.index) % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * ((list3.Length - 1 - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestFindLastMin() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.FindLastMin(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * (list3.Length - x.index), mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * ((list3.Length - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * ((list3.Length - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (int)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (int)(uint)(x.elem * ((list3.Length - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (uint)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (uint)(x.elem * ((list3.Length - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (long)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * ((list3.Length - x.index) % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * ((list3.Length - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestFindLastMinIndex() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.FindLastMinIndex(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindLastMinIndex((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindLastMinIndex(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindLastMinIndex((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMinIndex(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindLastMinIndex((x, index) => (double)x / 1000000000000 * ((index + 1) % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMinIndex(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(intOptimum, @int);
		b = a.FindLastMinIndex((x, index) => (int)(x * ((index + 1) % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMinIndex(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindLastMinIndex((x, index) => (uint)(x * ((index + 1) % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindLastMinIndex(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(longOptimum, @long);
		b = a.FindLastMinIndex((x, index) => (long)(x * ((index + 1) % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
	});

	[TestMethod]
	public void TestFindLastMinSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var mpzT = a.FindLastMin(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * (list3.Length - x.index), mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * ((list3.Length - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * ((list3.Length - x.index) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (int)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (int)(uint)(x.elem * ((list3.Length - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (uint)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (uint)(x.elem * ((list3.Length - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Reverse(list3), x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindLastMin((x, index) => (long)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(E.Reverse(list3), (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * ((list3.Length - x.index) % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * ((list3.Length - x.index) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
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
	public void TestFindMaxSpan() => OptimumTestSpan(10000, (a, list3) =>
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
	public void TestFindMaxIndex() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.FindMaxIndex(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMaxIndex((x, index) => x.PowerMod(new MpzT(power * index), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMaxIndex(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMaxIndex((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMaxIndex(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMaxIndex((x, index) => (double)x / 1000000000000 * (index % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMaxIndex(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMaxIndex((x, index) => (int)(x * (index % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMaxIndex(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMaxIndex((x, index) => (uint)(x * (index % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMaxIndex(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(longOptimum, @long);
		b = a.FindMaxIndex((x, index) => (long)(x * (index % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestFindMaxIndexes() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.FindMaxIndexes(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMaxIndexes((x, index) => x.PowerMod(new MpzT(power * index), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMaxIndexes(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMaxIndexes((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMaxIndexes(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMaxIndexes((x, index) => (double)x / 1000000000000 * (index % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMaxIndexes(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMaxIndexes((x, index) => (int)(x * (index % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMaxIndexes(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMaxIndexes((x, index) => (uint)(x * (index % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMaxIndexes(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
		b = a.FindMaxIndexes((x, index) => (long)(x * (index % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestFindMaxIndexesSpan() => OptimumTestSpan(1000, (a, list3) =>
	{
		var b = a.FindMaxIndexes(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMaxIndexes((x, index) => x.PowerMod(new MpzT(power * index), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMaxIndexes(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMaxIndexes((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMaxIndexes(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMaxIndexes((x, index) => (double)x / 1000000000000 * (index % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMaxIndexes(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMaxIndexes((x, index) => (int)(x * (index % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMaxIndexes(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMaxIndexes((x, index) => (uint)(x * (index % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMaxIndexes(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
		b = a.FindMaxIndexes((x, index) => (long)(x * (index % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestFindMin() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.FindMin(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * (x.index + 1), mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * ((x.index + 1) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * ((x.index + 1) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (int)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (int)(uint)(x.elem * ((x.index + 1) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (uint)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (uint)(x.elem * ((x.index + 1) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (long)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * ((x.index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * ((x.index + 1) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestFindMinIndexes() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.FindMinIndexes(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMinIndexes((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMinIndexes(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMinIndexes((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMinIndexes(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMinIndexes((x, index) => (double)x / 1000000000000 * ((index + 1) % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMinIndexes(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMinIndexes((x, index) => (int)(x * ((index + 1) % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMinIndexes(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMinIndexes((x, index) => (uint)(x * ((index + 1) % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMinIndexes(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
		b = a.FindMinIndexes((x, index) => (long)(x * ((index + 1) % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestFindMinIndexesSpan() => OptimumTestSpan(1000, (a, list3) =>
	{
		var b = a.FindMinIndexes(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMinIndexes((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMinIndexes(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMinIndexes((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMinIndexes(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMinIndexes((x, index) => (double)x / 1000000000000 * ((index + 1) % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMinIndexes(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMinIndexes((x, index) => (int)(x * ((index + 1) % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMinIndexes(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMinIndexes((x, index) => (uint)(x * ((index + 1) % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMinIndexes(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
		b = a.FindMinIndexes((x, index) => (long)(x * ((index + 1) % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestFindMinSpan() => OptimumTestSpan(10000, (a, list3) =>
	{
		var mpzT = a.FindMin(x => x.PowerMod(new MpzT(power), new(mod)));
		var optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => BigInteger.ModPow(x, power, mod));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => BigInteger.ModPow(x.elem, power * (x.index + 1), mod)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (decimal)((double)x.elem / 1000000000000 / 1000000000000) * ((x.index + 1) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (double)x / 1000000000000);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (double)x / 1000000000000);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (double)x.elem / 1000000000000 * ((x.index + 1) % 5)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (int)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (int)(uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (int)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (int)(uint)(x.elem * ((x.index + 1) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (uint)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (uint)(x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (uint)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (uint)(x.elem * ((x.index + 1) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin(x => (long)x);
		optimum = list3.Length == 0 ? 0 : E.MinBy(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296));
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.FindMin((x, index) => (long)(x * ((index + 1) % 5)));
		optimum = list3.Length == 0 ? 0 : E.MinBy(E.Select(list3, (elem, index) => (elem, index)), x => (long)(ulong)(x.elem * ((x.index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x.elem * ((x.index + 1) % 5) % 4294967296)).elem;
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
	});

	[TestMethod]
	public void TestFindMinIndex() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.FindMinIndex(x => x.PowerMod(new MpzT(power), new(mod)), out var mpzT);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		var c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMinIndex((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)), out mpzT);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		b = a.FindMinIndex(x => (decimal)((double)x / 1000000000000 / 1000000000000), out var @decimal);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(decimalOptimum, @decimal);
		b = a.FindMinIndex((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5), out @decimal);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindMinIndex(x => (double)x / 1000000000000, out var @double);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(doubleOptimum, @double);
		b = a.FindMinIndex((x, index) => (double)x / 1000000000000 * ((index + 1) % 5), out @double);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindMinIndex(x => (int)x, out var @int);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(intOptimum, @int);
		b = a.FindMinIndex((x, index) => (int)(x * ((index + 1) % 5)), out @int);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindMinIndex(x => (uint)x, out var @uint);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(uintOptimum, @uint);
		b = a.FindMinIndex((x, index) => (uint)(x * ((index + 1) % 5)), out @uint);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.FindMinIndex(x => (long)x, out var @long);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		Assert.AreEqual(longOptimum, @long);
		b = a.FindMinIndex((x, index) => (long)(x * ((index + 1) % 5)), out @long);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
	});

	[TestMethod]
	public void TestIndexesOfMax() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.IndexesOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		ProcessIndexesOf(a, list3, (x, index) => x.PowerMod(new MpzT(power), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power, mod)), index),
			RedStarLinqMath.Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power * index, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power * index, mod)), index),
			RedStarLinqMath.Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000), index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index),
			Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (double)x / 1000000000000, (x, index) => (double)x / 1000000000000,
			(elem, index) => ((double)elem / 1000000000000, index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (double)x / 1000000000000 * (index % 5),
			(x, index) => (double)x / 1000000000000 * (index % 5),
			(elem, index) => ((double)elem / 1000000000000 * (index % 5), index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (int)x, (x, index) => (int)(uint)(x % 4294967296),
			(elem, index) => ((int)(uint)(elem % 4294967296), index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (int)(x * (index % 5)),
			(x, index) => (int)(uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((int)(uint)(elem * (index % 5) % 4294967296), index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (uint)x, (x, index) => (uint)(x % 4294967296),
			(elem, index) => ((uint)(elem % 4294967296), index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (uint)(x * (index % 5)), (x, index) => (uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((uint)(elem * (index % 5) % 4294967296), index), Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (long)x,
			(x, index) => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296),
			(elem, index) => ((long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index),
			Max, x => x.IndexesOfMax());
		ProcessIndexesOf(a, list3, (x, index) => (long)(x * (index % 5)),
			(x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296),
			(elem, index) => ((long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296
			+ elem * (index % 5) % 4294967296), index), Max, x => x.IndexesOfMax());
	});

	[TestMethod]
	public void TestIndexesOfMaxSpan() => OptimumTestSpan(1000, (a, list3) =>
	{
		var b = a.IndexesOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => x.PowerMod(new MpzT(power), new(mod))).AsSpan().IndexesOfMax();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).AsSpan().IndexesOfMax();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsSpan().IndexesOfMax();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsSpan().IndexesOfMax();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (double)x / 1000000000000).AsSpan().IndexesOfMax();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsSpan().IndexesOfMax();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (int)x).AsSpan().IndexesOfMax();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (int)(x * (index % 5))).AsSpan().IndexesOfMax();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (uint)x).AsSpan().IndexesOfMax();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (uint)(x * (index % 5))).AsSpan().IndexesOfMax();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (long)x).AsSpan().IndexesOfMax();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (long)(x * (index % 5))).AsSpan().IndexesOfMax();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestIndexesOfMean() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.IndexesOfMean();
		var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean(x => (int)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean((x, index) => (int)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean(x => (uint)x);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean((x, index) => (uint)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean(x => (long)x);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMean((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => x.PowerMod(new MpzT(power), new(mod))).AsEnumerable().IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).AsEnumerable().IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsEnumerable().IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsEnumerable().IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (double)x / 1000000000000).AsEnumerable().IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsEnumerable().IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (int)x).AsEnumerable().IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (int)(x * (index % 5))).AsEnumerable().IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (uint)x).AsEnumerable().IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (uint)(x * (index % 5))).AsEnumerable().IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (long)x).AsEnumerable().IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (long)(x * (index % 5))).AsEnumerable().IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList(x => x.PowerMod(new MpzT(power), new(mod))).IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList(x => (decimal)((double)x / 1000000000000 / 1000000000000)).IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList(x => (double)x / 1000000000000).IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList((x, index) => (double)x / 1000000000000 * (index % 5)).IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList(x => (int)x).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList((x, index) => (int)(x * (index % 5))).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList(x => (uint)x).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList((x, index) => (uint)(x * (index % 5))).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList(x => (long)x).IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToList((x, index) => (long)(x * (index % 5))).IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, x => x.PowerMod(new MpzT(power), new(mod)))).IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)))).IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000))).IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5))).IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, x => (double)x / 1000000000000)).IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5))).IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, x => (int)x)).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, (x, index) => (int)(x * (index % 5)))).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, x => (uint)x)).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, (x, index) => (uint)(x * (index % 5)))).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, x => (long)x)).IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.ToList(E.Select(a, (x, index) => (long)(x * (index % 5)))).IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, x => x.PowerMod(new MpzT(power), new(mod))).IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, (x, index) => x.PowerMod(new MpzT(power * index), new(mod))).IndexesOfMean();
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000)).IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).IndexesOfMean();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, x => (double)x / 1000000000000).IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5)).IndexesOfMean();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, x => (int)x).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, (x, index) => (int)(x * (index % 5))).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, x => (uint)x).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, (x, index) => (uint)(x * (index % 5))).IndexesOfMean();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, x => (long)x).IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = E.Select(a, (x, index) => (long)(x * (index % 5))).IndexesOfMean();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestIndexesOfMin() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.IndexesOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (int)(x * ((index + 1) % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (uint)(x * ((index + 1) % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (long)(x * ((index + 1) % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		ProcessIndexesOf(a, list3, (x, index) => x.PowerMod(new MpzT(power), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power, mod)), index),
			RedStarLinqMath.Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power * index, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power * index, mod)), index),
			RedStarLinqMath.Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000), index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index),
			Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (double)x / 1000000000000, (x, index) => (double)x / 1000000000000,
			(elem, index) => ((double)elem / 1000000000000, index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (double)x / 1000000000000 * (index % 5),
			(x, index) => (double)x / 1000000000000 * (index % 5),
			(elem, index) => ((double)elem / 1000000000000 * (index % 5), index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (int)x, (x, index) => (int)(uint)(x % 4294967296),
			(elem, index) => ((int)(uint)(elem % 4294967296), index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (int)(x * (index % 5)),
			(x, index) => (int)(uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((int)(uint)(elem * (index % 5) % 4294967296), index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (uint)x, (x, index) => (uint)(x % 4294967296),
			(elem, index) => ((uint)(elem % 4294967296), index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (uint)(x * (index % 5)), (x, index) => (uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((uint)(elem * (index % 5) % 4294967296), index), Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (long)x,
			(x, index) => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296),
			(elem, index) => ((long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index),
			Min, x => x.IndexesOfMin());
		ProcessIndexesOf(a, list3, (x, index) => (long)(x * (index % 5)),
			(x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296),
			(elem, index) => ((long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296
			+ elem * (index % 5) % 4294967296), index), Min, x => x.IndexesOfMin());
	});

	[TestMethod]
	public void TestIndexesOfMinSpan() => OptimumTestSpan(1000, (a, list3) =>
	{
		var b = a.IndexesOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (double)x / 1000000000000 * ((index + 1) % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (int)(x * ((index + 1) % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (uint)(x * ((index + 1) % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.IndexesOfMin((x, index) => (long)(x * ((index + 1) % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => x.PowerMod(new MpzT(power), new(mod))).AsSpan().IndexesOfMin();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => x.PowerMod(new MpzT(power * (index + 1)), new(mod))).AsSpan().IndexesOfMin();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * (index + 1), mod)), BigInteger.Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * (index + 1), mod), index)), x => x.elem == optimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsSpan().IndexesOfMin();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)).AsSpan().IndexesOfMin();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * ((index + 1) % 5), index)), x => x.elem == decimalOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (double)x / 1000000000000).AsSpan().IndexesOfMin();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (double)x / 1000000000000 * ((index + 1) % 5)).AsSpan().IndexesOfMin();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * ((index + 1) % 5)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * ((index + 1) % 5), index)), x => x.elem == doubleOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (int)x).AsSpan().IndexesOfMin();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (int)(x * ((index + 1) % 5))).AsSpan().IndexesOfMin();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (uint)x).AsSpan().IndexesOfMin();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (uint)(x * ((index + 1) % 5))).AsSpan().IndexesOfMin();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray(x => (long)x).AsSpan().IndexesOfMin();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
		b = a.ToArray((x, index) => (long)(x * ((index + 1) % 5))).AsSpan().IndexesOfMin();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + x * ((index + 1) % 5) % 4294967296)), Min);
		c = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * ((index + 1) % 5) / 4294967296 % 4294967296 * 4294967296 + elem * ((index + 1) % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
		Assert.IsTrue(RedStarLinq.Equals(b, c));
		Assert.IsTrue(E.SequenceEqual(c, b));
	});

	[TestMethod]
	public void TestIndexOfMax() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.IndexOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power, mod)), index),
			RedStarLinqMath.Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power * index, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power * index, mod)), index),
			RedStarLinqMath.Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000), index),
			Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index),
			Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000, (x, index) => (double)x / 1000000000000,
			(elem, index) => ((double)elem / 1000000000000, index), Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000 * (index % 5),
			(x, index) => (double)x / 1000000000000 * (index % 5),
			(elem, index) => ((double)elem / 1000000000000 * (index % 5), index), Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)x, (x, index) => (int)(uint)(x % 4294967296),
			(elem, index) => ((int)(uint)(elem % 4294967296), index), Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)(x * (index % 5)),
			(x, index) => (int)(uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((int)(uint)(elem * (index % 5) % 4294967296), index), Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)x,
			(x, index) => (uint)(x % 4294967296), (elem, index) => ((uint)(elem % 4294967296), index),
			Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)(x * (index % 5)),
			(x, index) => (uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((uint)(elem * (index % 5) % 4294967296), index), Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)x,
			(x, index) => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296),
			(elem, index) => ((long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index),
			Max, x => x.IndexOfMax(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)(x * (index % 5)),
			(x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296),
			(elem, index) => ((long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296
			+ elem * (index % 5) % 4294967296), index), Max, x => x.IndexOfMax(), E.FirstOrDefault);
	});

	[TestMethod]
	public void TestIndexOfMean() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.IndexOfMean();
		var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
		var c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean(x => (int)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean((x, index) => (int)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean(x => (uint)x);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean((x, index) => (uint)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean(x => (long)x);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMean((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
	});

	[TestMethod]
	public void TestIndexOfMin() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.IndexOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.IndexOfMin((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		c = E.FirstOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power, mod)), index),
			RedStarLinqMath.Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power * index, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power * index, mod)), index),
			RedStarLinqMath.Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000), index),
			Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index),
			Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000, (x, index) => (double)x / 1000000000000,
			(elem, index) => ((double)elem / 1000000000000, index), Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000 * (index % 5),
			(x, index) => (double)x / 1000000000000 * (index % 5),
			(elem, index) => ((double)elem / 1000000000000 * (index % 5), index), Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)x, (x, index) => (int)(uint)(x % 4294967296),
			(elem, index) => ((int)(uint)(elem % 4294967296), index), Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)(x * (index % 5)),
			(x, index) => (int)(uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((int)(uint)(elem * (index % 5) % 4294967296), index), Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)x,
			(x, index) => (uint)(x % 4294967296), (elem, index) => ((uint)(elem % 4294967296), index),
			Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)(x * (index % 5)),
			(x, index) => (uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((uint)(elem * (index % 5) % 4294967296), index), Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)x,
			(x, index) => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296),
			(elem, index) => ((long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index),
			Min, x => x.IndexOfMin(), E.FirstOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)(x * (index % 5)),
			(x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296),
			(elem, index) => ((long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296
			+ elem * (index % 5) % 4294967296), index), Min, x => x.IndexOfMin(), E.FirstOrDefault);
	});

	[TestMethod]
	public void TestLastIndexOfMax() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.LastIndexOfMax();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
		var c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMax((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power, mod)), index),
			RedStarLinqMath.Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power * index, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power * index, mod)), index),
			RedStarLinqMath.Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000), index),
			Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index),
			Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000, (x, index) => (double)x / 1000000000000,
			(elem, index) => ((double)elem / 1000000000000, index), Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000 * (index % 5),
			(x, index) => (double)x / 1000000000000 * (index % 5),
			(elem, index) => ((double)elem / 1000000000000 * (index % 5), index), Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)x, (x, index) => (int)(uint)(x % 4294967296),
			(elem, index) => ((int)(uint)(elem % 4294967296), index), Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)(x * (index % 5)),
			(x, index) => (int)(uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((int)(uint)(elem * (index % 5) % 4294967296), index), Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)x,
			(x, index) => (uint)(x % 4294967296), (elem, index) => ((uint)(elem % 4294967296), index),
			Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)(x * (index % 5)),
			(x, index) => (uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((uint)(elem * (index % 5) % 4294967296), index), Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)x,
			(x, index) => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296),
			(elem, index) => ((long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index),
			Max, x => x.LastIndexOfMax(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)(x * (index % 5)),
			(x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296),
			(elem, index) => ((long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296
			+ elem * (index % 5) % 4294967296), index), Max, x => x.LastIndexOfMax(), E.LastOrDefault);
	});

	[TestMethod]
	public void TestLastIndexOfMean() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.LastIndexOfMean();
		var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
		var c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => Abs((double)x.elem - optimum2) <= Max((double)x.elem, optimum2) / (1L << 32)), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean(x => (int)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean((x, index) => (int)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean(x => (uint)x);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean((x, index) => (uint)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean(x => (long)x);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMean((x, index) => (long)(x * (index % 5)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
	});

	[TestMethod]
	public void TestLastIndexOfMin() => OptimumTest(1000, (a, list3) =>
	{
		var b = a.LastIndexOfMin();
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
		var c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin(x => (double)x / 1000000000000);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin(x => (int)x);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin((x, index) => (int)(x * (index % 5)));
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin(x => (uint)x);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin((x, index) => (uint)(x * (index % 5)));
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin(x => (long)x);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		b = a.LastIndexOfMin((x, index) => (long)(x * (index % 5)));
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		c = E.LastOrDefault(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index), -1);
		Assert.AreEqual(c, b);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power, mod)), index),
			RedStarLinqMath.Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)),
			(x, index) => new MpzT(BigInteger.ModPow(x, power * index, mod)),
			(elem, index) => (new MpzT(BigInteger.ModPow(elem, power * index, mod)), index),
			RedStarLinqMath.Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000), index),
			Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5),
			(elem, index) => ((decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index),
			Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000, (x, index) => (double)x / 1000000000000,
			(elem, index) => ((double)elem / 1000000000000, index), Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (double)x / 1000000000000 * (index % 5),
			(x, index) => (double)x / 1000000000000 * (index % 5),
			(elem, index) => ((double)elem / 1000000000000 * (index % 5), index), Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)x, (x, index) => (int)(uint)(x % 4294967296),
			(elem, index) => ((int)(uint)(elem % 4294967296), index), Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (int)(x * (index % 5)),
			(x, index) => (int)(uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((int)(uint)(elem * (index % 5) % 4294967296), index), Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)x,
			(x, index) => (uint)(x % 4294967296), (elem, index) => ((uint)(elem % 4294967296), index),
			Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (uint)(x * (index % 5)),
			(x, index) => (uint)(x * (index % 5) % 4294967296),
			(elem, index) => ((uint)(elem * (index % 5) % 4294967296), index), Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)x,
			(x, index) => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296),
			(elem, index) => ((long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index),
			Min, x => x.LastIndexOfMin(), E.LastOrDefault);
		ProcessIndexOf(a, list3, (x, index) => (long)(x * (index % 5)),
			(x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296),
			(elem, index) => ((long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296
			+ elem * (index % 5) % 4294967296), index), Min, x => x.LastIndexOfMin(), E.LastOrDefault);
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
		@decimal = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsEnumerable().Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsEnumerable().Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToArray(x => (double)x / 1000000000000).AsEnumerable().Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsEnumerable().Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToArray(x => (int)x).AsEnumerable().Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToArray((x, index) => (int)(x * (index % 5))).AsEnumerable().Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToArray(x => (uint)x).AsEnumerable().Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToArray((x, index) => (uint)(x * (index % 5))).AsEnumerable().Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToArray(x => (long)x).AsEnumerable().Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToArray((x, index) => (long)(x * (index % 5))).AsEnumerable().Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@string = a.ToArray(x => x.ToString()).AsEnumerable().Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = a.ToArray((x, index) => (x * (index % 5)).ToString()).AsEnumerable().Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@decimal = a.ToList(x => (decimal)((double)x / 1000000000000 / 1000000000000)).Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToList((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToList(x => (double)x / 1000000000000).Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToList((x, index) => (double)x / 1000000000000 * (index % 5)).Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToList(x => (int)x).Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToList((x, index) => (int)(x * (index % 5))).Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToList(x => (uint)x).Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToList((x, index) => (uint)(x * (index % 5))).Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToList(x => (long)x).Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToList((x, index) => (long)(x * (index % 5))).Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@string = a.ToList(x => x.ToString()).Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = a.ToList((x, index) => (x * (index % 5)).ToString()).Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@decimal = E.ToList(E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000))).Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.ToList(E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5))).Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.ToList(E.Select(a, x => (double)x / 1000000000000)).Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.ToList(E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5))).Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.ToList(E.Select(a, x => (int)x)).Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = E.ToList(E.Select(a, (x, index) => (int)(x * (index % 5)))).Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.ToList(E.Select(a, x => (uint)x)).Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.ToList(E.Select(a, (x, index) => (uint)(x * (index % 5)))).Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.ToList(E.Select(a, x => (long)x)).Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = E.ToList(E.Select(a, (x, index) => (long)(x * (index % 5)))).Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@string = E.ToList(E.Select(a, x => x.ToString())).Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = E.ToList(E.Select(a, (x, index) => (x * (index % 5)).ToString())).Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@decimal = E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000)).Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.Select(a, x => (double)x / 1000000000000).Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5)).Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.Select(a, x => (int)x).Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = E.Select(a, (x, index) => (int)(x * (index % 5))).Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.Select(a, x => (uint)x).Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.Select(a, (x, index) => (uint)(x * (index % 5))).Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.Select(a, x => (long)x).Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = E.Select(a, (x, index) => (long)(x * (index % 5))).Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@string = E.Select(a, x => x.ToString()).Max();
		stringOptimum = list3.Length == 0 ? null : E.Max(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = E.Select(a, (x, index) => (x * (index % 5)).ToString()).Max();
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
		@decimal = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsSpan().Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsSpan().Max();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToArray(x => (double)x / 1000000000000).AsSpan().Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsSpan().Max();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToArray(x => (int)x).AsSpan().Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToArray((x, index) => (int)(x * (index % 5))).AsSpan().Max();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToArray(x => (uint)x).AsSpan().Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToArray((x, index) => (uint)(x * (index % 5))).AsSpan().Max();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToArray(x => (long)x).AsSpan().Max();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToArray((x, index) => (long)(x * (index % 5))).AsSpan().Max();
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
		@decimal = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsEnumerable().Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsEnumerable().Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToArray(x => (double)x / 1000000000000).AsEnumerable().Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsEnumerable().Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToArray(x => (int)x).AsEnumerable().Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToArray((x, index) => (int)(x * (index % 5))).AsEnumerable().Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToArray(x => (uint)x).AsEnumerable().Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToArray((x, index) => (uint)(x * (index % 5))).AsEnumerable().Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToArray(x => (long)x).AsEnumerable().Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToArray((x, index) => (long)(x * (index % 5))).AsEnumerable().Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@string = a.ToArray(x => x.ToString()).AsEnumerable().Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = a.ToArray((x, index) => (x * (index % 5)).ToString()).AsEnumerable().Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@decimal = a.ToList(x => (decimal)((double)x / 1000000000000 / 1000000000000)).Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToList((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToList(x => (double)x / 1000000000000).Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToList((x, index) => (double)x / 1000000000000 * (index % 5)).Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToList(x => (int)x).Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToList((x, index) => (int)(x * (index % 5))).Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToList(x => (uint)x).Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToList((x, index) => (uint)(x * (index % 5))).Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToList(x => (long)x).Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToList((x, index) => (long)(x * (index % 5))).Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@string = a.ToList(x => x.ToString()).Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = a.ToList((x, index) => (x * (index % 5)).ToString()).Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@decimal = E.ToList(E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000))).Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.ToList(E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5))).Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.ToList(E.Select(a, x => (double)x / 1000000000000)).Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.ToList(E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5))).Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.ToList(E.Select(a, x => (int)x)).Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = E.ToList(E.Select(a, (x, index) => (int)(x * (index % 5)))).Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.ToList(E.Select(a, x => (uint)x)).Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.ToList(E.Select(a, (x, index) => (uint)(x * (index % 5)))).Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.ToList(E.Select(a, x => (long)x)).Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = E.ToList(E.Select(a, (x, index) => (long)(x * (index % 5)))).Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@string = E.ToList(E.Select(a, x => x.ToString())).Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = E.ToList(E.Select(a, (x, index) => (x * (index % 5)).ToString())).Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, (x, index) => (x * (index % 5)).ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@decimal = E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000)).Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.Select(a, x => (double)x / 1000000000000).Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5)).Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.Select(a, x => (int)x).Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = E.Select(a, (x, index) => (int)(x * (index % 5))).Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.Select(a, x => (uint)x).Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.Select(a, (x, index) => (uint)(x * (index % 5))).Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.Select(a, x => (long)x).Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = E.Select(a, (x, index) => (long)(x * (index % 5))).Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@string = E.Select(a, x => x.ToString()).Min();
		stringOptimum = list3.Length == 0 ? null : E.Min(E.Select(list3, x => x.ToString()));
		Assert.AreEqual(stringOptimum, @string);
		@string = E.Select(a, (x, index) => (x * (index % 5)).ToString()).Min();
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
		@decimal = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsSpan().Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsSpan().Min();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToArray(x => (double)x / 1000000000000).AsSpan().Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsSpan().Min();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToArray(x => (int)x).AsSpan().Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToArray((x, index) => (int)(x * (index % 5))).AsSpan().Min();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToArray(x => (uint)x).AsSpan().Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToArray((x, index) => (uint)(x * (index % 5))).AsSpan().Min();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToArray(x => (long)x).AsSpan().Min();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToArray((x, index) => (long)(x * (index % 5))).AsSpan().Min();
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
				if (bytes2.Length != 0 && random.Next(2) == 0)
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
		mpzT = a.ToArray(x => x.PowerMod(new MpzT(power), new(mod))).AsEnumerable().Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.ToArray((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).AsEnumerable().Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = a.ToArray(x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))).AsEnumerable().Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToArray((x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)).AsEnumerable().Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToArray(x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)).AsEnumerable().Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToArray((x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)).AsEnumerable().Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToArray(x => (int)x).AsEnumerable().Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToArray((x, index) => (int)(x * (index % 5 + 1))).AsEnumerable().Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5 + 1) % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToArray(x => (uint)x).AsEnumerable().Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToArray((x, index) => (uint)(x * (index % 5 + 1))).AsEnumerable().Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5 + 1) % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToArray(x => (long)x).AsEnumerable().Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToArray((x, index) => (long)(x * (index % 5 + 1))).AsEnumerable().Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5 + 1) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5 + 1) % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = a.ToList(x => x.PowerMod(new MpzT(power), new(mod))).Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.ToList((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = a.ToList(x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))).Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToList((x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)).Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToList(x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)).Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToList((x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)).Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToList(x => (int)x).Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToList((x, index) => (int)(x * (index % 5 + 1))).Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5 + 1) % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToList(x => (uint)x).Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToList((x, index) => (uint)(x * (index % 5 + 1))).Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5 + 1) % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToList(x => (long)x).Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToList((x, index) => (long)(x * (index % 5 + 1))).Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5 + 1) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5 + 1) % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = E.ToList(E.Select(a, x => x.PowerMod(new MpzT(power), new(mod)))).Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = E.ToList(E.Select(a, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)))).Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = E.ToList(E.Select(a, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)))).Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.ToList(E.Select(a, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1))).Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.ToList(E.Select(a, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5))).Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.ToList(E.Select(a, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1))).Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.ToList(E.Select(a, x => (int)x)).Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@int = E.ToList(E.Select(a, (x, index) => (int)(x * (index % 5 + 1)))).Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5 + 1) % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.ToList(E.Select(a, x => (uint)x)).Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.ToList(E.Select(a, (x, index) => (uint)(x * (index % 5 + 1)))).Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5 + 1) % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.ToList(E.Select(a, x => (long)x)).Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		@long = E.ToList(E.Select(a, (x, index) => (long)(x * (index % 5 + 1)))).Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5 + 1) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5 + 1) % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = E.Select(a, x => x.PowerMod(new MpzT(power), new(mod))).Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = E.Select(a, (x, index) => x.PowerMod(new MpzT(power * index), new(mod))).Product();
		optimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)1, (x, y) => x * y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = E.Select(a, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))).Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2))), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.Select(a, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)).Product();
		decimalOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / Pow(2, Floor(Log((double)x, 2)) - 2)) * (index % 5 + 1)), 1m, (x, y) => x * y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.Select(a, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)).Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.Select(a, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)).Product();
		doubleOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (double)x / Pow(2, Floor(Log((double)x, 2)) - 5) * (index % 5 + 1)), 1d, (x, y) => x * y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.Select(a, x => (int)x).Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@int = E.Select(a, (x, index) => (int)(x * (index % 5 + 1))).Product();
		intOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5 + 1) % 4294967296)), 1, (x, y) => x * y);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.Select(a, x => (uint)x).Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.Select(a, (x, index) => (uint)(x * (index % 5 + 1))).Product();
		uintOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5 + 1) % 4294967296)), 1u, (x, y) => x * y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.Select(a, x => (long)x).Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
		@long = E.Select(a, (x, index) => (long)(x * (index % 5 + 1))).Product();
		longOptimum = list3.Length == 0 ? 1 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5 + 1) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5 + 1) % 4294967296)), 1L, (x, y) => x * y);
		Assert.AreEqual(longOptimum, @long);
	});

	[TestMethod]
	public void TestProgression() => OptimumTest(10000, (a, list3) =>
	{
		var mpzT = a.Progression((x, y) => x + y);
		var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Convert(x => x.PowerMod(new MpzT(power), new(mod))).Progression((x, y) => x + y);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Convert((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).Progression((x, y) => x + y);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		var @decimal = a.Convert(x => (decimal)((double)x / 1000000000000 / 1000000000000)).Progression((x, y) => x + y);
		var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Convert((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Progression((x, y) => x + y);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		var @double = a.Convert(x => (double)x / 1000000000000).Progression((x, y) => x + y);
		var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Convert((x, index) => (double)x / 1000000000000 * (index % 5)).Progression((x, y) => x + y);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		var @int = a.Convert(x => (int)x).Progression((x, y) => x + y);
		var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Convert((x, index) => (int)(x * (index % 5))).Progression((x, y) => x + y);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		var @uint = a.Convert(x => (uint)x).Progression((x, y) => x + y);
		var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Convert((x, index) => (uint)(x * (index % 5))).Progression((x, y) => x + y);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		var @long = a.Convert(x => (long)x).Progression((x, y) => x + y);
		var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Convert((x, index) => (long)(x * (index % 5))).Progression((x, y) => x + y);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = a.Progression((MpzT)0, (x, y) => x + y);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Convert(x => x.PowerMod(new MpzT(power), new(mod))).Progression((MpzT)0, (x, y) => x + y);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.Convert((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).Progression((MpzT)0, (x, y) => x + y);
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = a.Convert(x => (decimal)((double)x / 1000000000000 / 1000000000000)).Progression(0m, (x, y) => x + y);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.Convert((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Progression(0m, (x, y) => x + y);
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.Convert(x => (double)x / 1000000000000).Progression(0d, (x, y) => x + y);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.Convert((x, index) => (double)x / 1000000000000 * (index % 5)).Progression(0d, (x, y) => x + y);
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.Convert(x => (int)x).Progression(0, (x, y) => x + y);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.Convert((x, index) => (int)(x * (index % 5))).Progression(0, (x, y) => x + y);
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.Convert(x => (uint)x).Progression(0u, (x, y) => x + y);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.Convert((x, index) => (uint)(x * (index % 5))).Progression(0u, (x, y) => x + y);
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.Convert(x => (long)x).Progression(0L, (x, y) => x + y);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.Convert((x, index) => (long)(x * (index % 5))).Progression(0L, (x, y) => x + y);
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
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
		mpzT = a.ToArray(x => x.PowerMod(new MpzT(power), new(mod))).AsEnumerable().Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.ToArray((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).AsEnumerable().Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = a.ToArray(x => (decimal)((double)x / 1000000000000 / 1000000000000)).AsEnumerable().Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToArray((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).AsEnumerable().Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToArray(x => (double)x / 1000000000000).AsEnumerable().Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToArray((x, index) => (double)x / 1000000000000 * (index % 5)).AsEnumerable().Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToArray(x => (int)x).AsEnumerable().Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToArray((x, index) => (int)(x * (index % 5))).AsEnumerable().Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToArray(x => (uint)x).AsEnumerable().Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToArray((x, index) => (uint)(x * (index % 5))).AsEnumerable().Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToArray(x => (long)x).AsEnumerable().Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToArray((x, index) => (long)(x * (index % 5))).AsEnumerable().Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = a.ToList(x => x.PowerMod(new MpzT(power), new(mod))).Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = a.ToList((x, index) => x.PowerMod(new MpzT(power * index), new(mod))).Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = a.ToList(x => (decimal)((double)x / 1000000000000 / 1000000000000)).Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = a.ToList((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = a.ToList(x => (double)x / 1000000000000).Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = a.ToList((x, index) => (double)x / 1000000000000 * (index % 5)).Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = a.ToList(x => (int)x).Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = a.ToList((x, index) => (int)(x * (index % 5))).Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@uint = a.ToList(x => (uint)x).Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = a.ToList((x, index) => (uint)(x * (index % 5))).Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = a.ToList(x => (long)x).Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = a.ToList((x, index) => (long)(x * (index % 5))).Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = E.ToList(E.Select(a, x => x.PowerMod(new MpzT(power), new(mod)))).Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = E.ToList(E.Select(a, (x, index) => x.PowerMod(new MpzT(power * index), new(mod)))).Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = E.ToList(E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000))).Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.ToList(E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5))).Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.ToList(E.Select(a, x => (double)x / 1000000000000)).Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.ToList(E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5))).Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.ToList(E.Select(a, x => (int)x)).Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = E.ToList(E.Select(a, (x, index) => (int)(x * (index % 5)))).Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.ToList(E.Select(a, x => (uint)x)).Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.ToList(E.Select(a, (x, index) => (uint)(x * (index % 5)))).Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.ToList(E.Select(a, x => (long)x)).Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = E.ToList(E.Select(a, (x, index) => (long)(x * (index % 5)))).Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		mpzT = E.Select(a, x => x.PowerMod(new MpzT(power), new(mod))).Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		mpzT = E.Select(a, (x, index) => x.PowerMod(new MpzT(power * index), new(mod))).Sum();
		optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
		Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
		@decimal = E.Select(a, x => (decimal)((double)x / 1000000000000 / 1000000000000)).Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@decimal = E.Select(a, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)).Sum();
		decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
		Assert.AreEqual(decimalOptimum, @decimal);
		@double = E.Select(a, x => (double)x / 1000000000000).Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@double = E.Select(a, (x, index) => (double)x / 1000000000000 * (index % 5)).Sum();
		doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
		Assert.AreEqual(doubleOptimum, @double);
		@int = E.Select(a, x => (int)x).Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@int = E.Select(a, (x, index) => (int)(x * (index % 5))).Sum();
		intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
		Assert.AreEqual(intOptimum, @int);
		@uint = E.Select(a, x => (uint)x).Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@uint = E.Select(a, (x, index) => (uint)(x * (index % 5))).Sum();
		uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
		Assert.AreEqual(uintOptimum, @uint);
		@long = E.Select(a, x => (long)x).Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
		@long = E.Select(a, (x, index) => (long)(x * (index % 5))).Sum();
		longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
		Assert.AreEqual(longOptimum, @long);
	});
}
