using System.Numerics;

namespace Corlib.NStar.Tests;

[TestClass]
public class IndexesOfOptimumTests
{
	[TestMethod]
	public void MpzTTest()
	{
		var bytes = new byte[21];
		var bytes2 = new List<byte[]>();
		for (var i = 0; i < 1000; i++)
		{
			var (list2, list3) = RedStarLinq.FillArray(16, _ =>
			{
				if (bytes2.Length != 0 && random.Next(2) == 1)
					Array.Copy(bytes2.Random(), bytes, 21);
				else
					random.NextBytes(bytes.AsSpan(..^1));
				bytes2.Add(bytes);
				return (new MpzT(bytes, -1), new BigInteger(bytes));
			}).Wrap(x => (ImmutableArray.Create(x.ToArray(y => y.Item1)), ImmutableArray.Create(x.ToArray(y => y.Item2))));
			G.IEnumerable<MpzT> a = list2.ToList();
			ProcessA(a, list3);
			a = list2.ToArray();
			ProcessA(a, list3);
			a = E.ToList(list2);
			ProcessA(a, list3);
			a = list2.ToList().Insert(0, 12345678901234567890).GetSlice(1);
			ProcessA(a, list3);
			a = E.Select(list2, x => x);
			ProcessA(a, list3);
			a = E.SkipWhile(list2, _ => random.Next(10) == -1);
			ProcessA(a, list3);
			bytes2.Clear();
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
			G.IEnumerable<MpzT> a = list2.ToList();
			ProcessA(a, list3);
			a = list2.ToArray();
			ProcessA(a, list3);
			a = E.ToList(list2);
			ProcessA(a, list3);
			a = list2.ToList().Insert(0, 12345678901234567890).GetSlice(1);
			ProcessA(a, list3);
			a = E.Select(list2, x => x);
			ProcessA(a, list3);
			a = E.SkipWhile(list2, _ => random.Next(10) == -1);
			ProcessA(a, list3);
		}
		static void ProcessA(G.IEnumerable<MpzT> a, ImmutableArray<BigInteger> list3)
		{
			var c = a.IndexesOfMax();
			var optimum = E.Aggregate(list3, BigInteger.Max);
			var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			BigInteger power = new(new byte[] { 175, 16, 6 }), mod = new(new byte[] { 134, 116, 171, 29, 108, 91, 58, 139, 101, 78, 109, 137, 45, 245, 203, 228, 21, 249, 235, 144, 21 });
			c = a.IndexesOfMax(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum = E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			var decimalOptimum = E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax(x => (double)x / 1000000000000);
			var doubleOptimum = E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax(x => (int)x);
			var intOptimum = E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax((x, index) => (int)(x * (index % 5)));
			intOptimum = E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax(x => (uint)x);
			var uintOptimum = E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax((x, index) => (uint)(x * (index % 5)));
			uintOptimum = E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax(x => (long)x);
			var longOptimum = E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMax((x, index) => (long)(x * (index % 5)));
			longOptimum = E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean();
			var optimum2 = (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => (double)x.elem == optimum2), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum2 = (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => (double)x.elem == optimum2), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum2 = (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => (double)x.elem == optimum2), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (double)x / 1000000000000);
			doubleOptimum = E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (int)x);
			longOptimum = E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (int)(x * (index % 5)));
			longOptimum = E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (uint)x);
			longOptimum = E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (uint)(x * (index % 5)));
			longOptimum = E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (long)x);
			optimum = E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (long)(x * (index % 5)));
			optimum = E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin();
			optimum = E.Aggregate(list3, BigInteger.Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum = E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (double)x / 1000000000000);
			doubleOptimum = E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (int)x);
			intOptimum = E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (int)(x * (index % 5)));
			intOptimum = E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (uint)x);
			uintOptimum = E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (uint)(x * (index % 5)));
			uintOptimum = E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (long)x);
			longOptimum = E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (long)(x * (index % 5)));
			longOptimum = E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
		}
	}
}
