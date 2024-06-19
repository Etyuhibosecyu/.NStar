using System.Numerics;

namespace Corlib.NStar.Tests;

[TestClass]
public class OptimumTests
{
	[TestMethod]
	public void TestDirect()
	{
		var bytes = new byte[21];
		var bytes2 = new List<byte[]>();
		for (var i = 0; i < 1000; i++)
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
			G.IEnumerable<MpzT> a = RedStarLinq.ToList(list2);
			ProcessA(a, list3);
			a = RedStarLinq.ToArray(list2);
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
			G.IEnumerable<MpzT> a = RedStarLinq.ToList(list2);
			ProcessA(a, list3);
			a = RedStarLinq.ToArray(list2);
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
			var mpzT = a.Max();
			var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			BigInteger power = new(new byte[] { 175, 16, 6 }), mod = new(new byte[] { 134, 116, 171, 29, 108, 91, 58, 139, 101, 78, 109, 137, 45, 245, 203, 228, 21, 249, 235, 144, 21 });
			mpzT = a.Max(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Max);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.Max((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Max);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			var @decimal = a.Max(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			var decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Max);
			Assert.IsTrue(@decimal == decimalOptimum);
			@decimal = a.Max((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Max);
			Assert.IsTrue(@decimal == decimalOptimum);
			var @double = a.Max(x => (double)x / 1000000000000);
			var doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Max);
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Max((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Max);
			Assert.IsTrue(@double == doubleOptimum);
			var @int = a.Max(x => (int)x);
			var intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Max);
			Assert.IsTrue(@int == intOptimum);
			@int = a.Max((x, index) => (int)(x * (index % 5)));
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Max);
			Assert.IsTrue(@int == intOptimum);
			var @uint = a.Max(x => (uint)x);
			var uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Max);
			Assert.IsTrue(@uint == uintOptimum);
			@uint = a.Max((x, index) => (uint)(x * (index % 5)));
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Max);
			Assert.IsTrue(@uint == uintOptimum);
			var @long = a.Max(x => (long)x);
			var longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Max);
			Assert.IsTrue(@long == longOptimum);
			@long = a.Max((x, index) => (long)(x * (index % 5)));
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Max);
			Assert.IsTrue(@long == longOptimum);
			@double = a.Mean();
			doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (BigInteger)0, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean(x => x.PowerMod(new MpzT(power), new(mod)));
			doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@decimal = a.Mean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@decimal == decimalOptimum);
			@decimal = a.Mean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@decimal == decimalOptimum);
			@double = a.Mean(x => (double)x / 1000000000000);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean(x => (int)x);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean((x, index) => (int)(x * (index % 5)));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean(x => (uint)x);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean((x, index) => (uint)(x * (index % 5)));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0d, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean(x => (long)x);
			doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Mean((x, index) => (long)(x * (index % 5)));
			doubleOptimum = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			Assert.IsTrue(@double == doubleOptimum);
			mpzT = a.Min();
			optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.Min(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), BigInteger.Min);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.Min((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), BigInteger.Min);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			@decimal = a.Min(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
			Assert.IsTrue(@decimal == decimalOptimum);
			@decimal = a.Min((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
			Assert.IsTrue(@decimal == decimalOptimum);
			@double = a.Min(x => (double)x / 1000000000000);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Min((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
			Assert.IsTrue(@double == doubleOptimum);
			@int = a.Min(x => (int)x);
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
			Assert.IsTrue(@int == intOptimum);
			@int = a.Min((x, index) => (int)(x * (index % 5)));
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
			Assert.IsTrue(@int == intOptimum);
			@uint = a.Min(x => (uint)x);
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
			Assert.IsTrue(@uint == uintOptimum);
			@uint = a.Min((x, index) => (uint)(x * (index % 5)));
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
			Assert.IsTrue(@uint == uintOptimum);
			@long = a.Min(x => (long)x);
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
			Assert.IsTrue(@long == longOptimum);
			@long = a.Min((x, index) => (long)(x * (index % 5)));
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
			Assert.IsTrue(@long == longOptimum);
			mpzT = a.Sum();
			optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, (BigInteger)0, (x, y) => x + y);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.Sum(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (BigInteger)0, (x, y) => x + y);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			mpzT = a.Sum((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (BigInteger)0, (x, y) => x + y);
			Assert.IsTrue(optimum == new BigInteger([.. mpzT.ToByteArray(-1), 0]));
			@decimal = a.Sum(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), 0m, (x, y) => x + y);
			Assert.IsTrue(@decimal == decimalOptimum);
			@decimal = a.Sum((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), 0m, (x, y) => x + y);
			Assert.IsTrue(@decimal == decimalOptimum);
			@double = a.Sum(x => (double)x / 1000000000000);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), 0d, (x, y) => x + y);
			Assert.IsTrue(@double == doubleOptimum);
			@double = a.Sum((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), 0d, (x, y) => x + y);
			Assert.IsTrue(@double == doubleOptimum);
			@int = a.Sum(x => (int)x);
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0, (x, y) => x + y);
			Assert.IsTrue(@int == intOptimum);
			@int = a.Sum((x, index) => (int)(x * (index % 5)));
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), 0, (x, y) => x + y);
			Assert.IsTrue(@int == intOptimum);
			@uint = a.Sum(x => (uint)x);
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), 0u, (x, y) => x + y);
			Assert.IsTrue(@uint == uintOptimum);
			@uint = a.Sum((x, index) => (uint)(x * (index % 5)));
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), 0u, (x, y) => x + y);
			Assert.IsTrue(@uint == uintOptimum);
			@long = a.Sum(x => (long)x);
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), 0L, (x, y) => x + y);
			Assert.IsTrue(@long == longOptimum);
			@long = a.Sum((x, index) => (long)(x * (index % 5)));
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), 0L, (x, y) => x + y);
			Assert.IsTrue(@long == longOptimum);
		}
	}

	[TestMethod]
	public void TestIndexesOf()
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
			G.IEnumerable<MpzT> a = RedStarLinq.ToList(list2);
			ProcessA(a, list3);
			a = RedStarLinq.ToArray(list2);
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
			G.IEnumerable<MpzT> a = RedStarLinq.ToList(list2);
			ProcessA(a, list3);
			a = RedStarLinq.ToArray(list2);
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
			var optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Max);
			var d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			BigInteger power = new(new byte[] { 175, 16, 6 }), mod = new(new byte[] { 134, 116, 171, 29, 108, 91, 58, 139, 101, 78, 109, 137, 45, 245, 203, 228, 21, 249, 235, 144, 21 });
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
			c = a.IndexesOfMean();
			var optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(list3, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => (double)x.elem == optimum2), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => x.PowerMod(new MpzT(power), new(mod)));
			optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, x => BigInteger.ModPow(x, power, mod)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power, mod), index)), x => (double)x.elem == optimum2), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => x.PowerMod(new MpzT(power * index), new(mod)));
			optimum2 = list3.Length == 0 ? 0 : (double)E.Aggregate(E.Select(list3, (x, index) => BigInteger.ModPow(x, power * index, mod)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: BigInteger.ModPow(elem, power * index, mod), index)), x => (double)x.elem == optimum2), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (decimal)((double)x / 1000000000000 / 1000000000000));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (double)x / 1000000000000);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean(x => (int)x);
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), 0L, (x, y) => x + y) / list3.Length;
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
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMean((x, index) => (long)(x * (index % 5)));
			optimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), (BigInteger)0, (x, y) => x + y) / list3.Length;
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == optimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin();
			optimum = list3.Length == 0 ? 0 : E.Aggregate(list3, BigInteger.Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem, index)), x => x.elem == optimum), x => x.index));
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
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (decimal)((double)x / 1000000000000 / 1000000000000)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5));
			decimalOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (decimal)((double)x / 1000000000000 / 1000000000000) * (index % 5)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (decimal)((double)elem / 1000000000000 / 1000000000000) * (index % 5), index)), x => x.elem == decimalOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (double)x / 1000000000000);
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (double)x / 1000000000000), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000, index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (double)x / 1000000000000 * (index % 5));
			doubleOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (double)x / 1000000000000 * (index % 5)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (double)elem / 1000000000000 * (index % 5), index)), x => x.elem == doubleOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (int)x);
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (int)(uint)(x % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (int)(x * (index % 5)));
			intOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (int)(uint)(x * (index % 5) % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (int)(uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == intOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (uint)x);
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (uint)(x % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (uint)(x * (index % 5)));
			uintOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (uint)(x * (index % 5) % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (uint)(elem * (index % 5) % 4294967296), index)), x => x.elem == uintOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin(x => (long)x);
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, x => (long)(ulong)(x / 4294967296 % 4294967296 * 4294967296 + x % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem / 4294967296 % 4294967296 * 4294967296 + elem % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
			c = a.IndexesOfMin((x, index) => (long)(x * (index % 5)));
			longOptimum = list3.Length == 0 ? 0 : E.Aggregate(E.Select(list3, (x, index) => (long)(ulong)(x * (index % 5) / 4294967296 % 4294967296 * 4294967296 + x * (index % 5) % 4294967296)), Min);
			d = E.ToArray(E.Select(E.Where(E.Select(list3, (elem, index) => (elem: (long)(ulong)(elem * (index % 5) / 4294967296 % 4294967296 * 4294967296 + elem * (index % 5) % 4294967296), index)), x => x.elem == longOptimum), x => x.index));
			Assert.IsTrue(RedStarLinq.Equals(c, d));
			Assert.IsTrue(E.SequenceEqual(d, c));
		}
	}
}
