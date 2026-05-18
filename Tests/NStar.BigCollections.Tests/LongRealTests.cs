using System.Globalization;

namespace NStar.BigCollections.Tests;

[TestClass]
public class LongRealTests
{
	private static readonly int MantissaLength = 1000;
	private static readonly int MantissaByteLength = GetArrayLength(MantissaLength, 8);
	private static readonly MpuT MantissaOverflow = MpuT.One << MantissaLength;
	private static readonly MpuT MantissaMask = MantissaOverflow - 1;

	[TestMethod]
	public void ComplexTestMixed()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
	l1:
		bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
		if (random.Next(2) == 0)
			bytes.Resize(8);
		else
			bytes.ResizeLeft(8);
		var r = BitConverter.ToDouble(bytes.AsSpan());
		LongReal lr = new(r, MantissaLength);
		Validate();
		var actions = new[]
		{
			() =>
			{
				var op = (byte)random.Next(256);
				r += op;
				lr += op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				r -= op;
				lr -= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				r *= op;
				lr *= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				r /= op;
				lr /= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = random.Next();
				r += op;
				lr += op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				r -= op;
				lr -= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				r *= op;
				lr *= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				r /= op;
				lr /= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				r += op;
				lr += op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				r -= op;
				lr -= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				r *= op;
				lr *= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				r /= op;
				lr /= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = random.NextInt64();
				r += op;
				lr += (double)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				r -= op;
				lr -= (double)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				r *= op;
				lr *= (double)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				r /= op;
				lr /= (double)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= (double)op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				r += op;
				lr += (double)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				r -= op;
				lr -= (double)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				r *= op;
				lr *= (double)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				r /= op;
				lr /= (double)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= (double)op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				r += op;
				lr += (double)op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				r -= op;
				lr -= (double)op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong) random.NextInt64() +(random.Next(2) == 0 ? 0 : 1uL << 63));
				r *= op;
				lr *= (double)op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong) random.NextInt64() +(random.Next(2) == 0 ? 0 : 1uL << 63));
				if (op == 0)
					return;
				r /= op;
				lr /= (double)op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= (double)op;
				ValidateRemainder(order - 52);
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			if (random.Next(100) == 0)
				r = BitConverter.ToDouble(bytes.AsSpan());
			lr = new(r, MantissaLength);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		void Validate() => Assert.IsTrue(r == (double)lr || r is double.NaN && (double)lr is double.NaN);
		void ValidateRemainder(int validOrder) => Assert.IsTrue(Abs(r - (double)lr) < ((LongReal)1).Shift(validOrder));
	}

	[TestMethod]
	public void TestCompareTo()
	{
		var x = new LongReal(123).Shift(456); // мантисса = 123, экспонента = 456
		var y = new LongReal(123).Shift(456);
		var result = x.CompareTo(y);
		Assert.AreEqual(0, result);
		x = new LongReal(100).Shift(50);
		y = new LongReal(200).Shift(50);
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		x = new LongReal(100).Shift(1000);
		y = new LongReal(100).Shift(2000);
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		// Очень большие экспоненты
		x = new LongReal(1).Shift(int.MaxValue);      // экспонента = 2 147 483 647
		y = new LongReal(1).Shift(int.MaxValue + 1L); // экспонента = 2 147 483 648
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		// Очень маленькие (отрицательные) экспоненты
		x = new LongReal(1).Shift(int.MinValue);      // экспонента = -2 147 483 648
		y = new LongReal(1).Shift(int.MinValue - 1L); // экспонента = -2 147 483 649
		Assert.AreEqual(1, x.CompareTo(y));  // x > y, т.к. -2 147 483 648 > -2 147 483 649
		Assert.AreEqual(-1, y.CompareTo(x));
		x = new LongReal(-1).Shift(int.MinValue);      // экспонента = -2 147 483 648
		y = new LongReal(-1).Shift(int.MinValue - 1L); // экспонента = -2 147 483 649
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		x = new LongReal(500).Shift(int.MaxValue);    // очень большое число
		y = new LongReal(500).Shift(int.MinValue);    // очень маленькое число
		Assert.AreEqual(1, x.CompareTo(y));
		Assert.AreEqual(-1, y.CompareTo(x));
		x = new LongReal(0).Shift(0);
		y = new LongReal(1).Shift(0);
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		x = new LongReal(-100).Shift(50);
		y = new LongReal(-200).Shift(50);
		var z = new LongReal(100).Shift(50);
		Assert.AreEqual(1, x.CompareTo(y));   // -100 > -200
		Assert.AreEqual(-1, y.CompareTo(x)); // -200 < -100
		Assert.AreEqual(-1, x.CompareTo(z));   // -100 < 100
		x = new LongReal(100).Shift(50);
		y = new LongReal(-100).Shift(50);
		Assert.AreEqual(1, x.CompareTo(y));
		Assert.AreEqual(-1, y.CompareTo(x));
		// Числа с нулевой мантиссой
		x = new LongReal(0).Shift(int.MaxValue);
		y = new LongReal(0).Shift(int.MinValue);
		Assert.AreEqual(0, x.CompareTo(y));
		// Крайние случаи: максимально возможная разница в экспонентах
		x = new LongReal(1).Shift(long.MaxValue);
		y = new LongReal(1).Shift(long.MinValue);
		Assert.AreEqual(1, x.CompareTo(y));
		Assert.AreEqual(-1, y.CompareTo(x));
		x = new LongReal(1).Shift(1);
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 10000000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr = new(r, MantissaLength);
			if (random.Next(1000) != 0)
			{
				bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
				if (random.Next(2) == 0)
					bytes.Resize(8);
				else
					bytes.ResizeLeft(8);
			}
			var uz2 = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr2 = new(uz2, MantissaLength);
			if (LongReal.IsNaN(lr) || LongReal.IsNaN(lr2))
				Assert.AreEqual(int.MinValue, lr.CompareTo(lr2));
			else
				Assert.AreEqual(Sign(r.CompareTo(uz2)), Sign(lr.CompareTo(lr2)));
		}
		Assert.Throws<ArgumentNullException>(() => x.CompareTo(null!));
	}

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(1, 501), _ => (byte)random.Next(256));
			var order = RandomOrder();
			bytes[order < 0 ? ^1 : 0] = 0;
			LongReal ulr = new(bytes.AsSpan(), order, MantissaLength);
			if (bytes.Length - MantissaByteLength == 4)
				continue;
			ProcessA(ulr);
		}
		void ProcessA(LongReal ulr)
		{
			dynamic num = ulr;
			ProcessB(ulr, num);
			num = ulr + 1;
			ProcessB(ulr, num);
			if (ulr.CompareTo(0) != 0)
			{
				num = ulr - 1;
				ProcessB(ulr, num);
			}
			num = ulr * 2;
			ProcessB(ulr, num);
			num = ulr / 2;
			ProcessB(ulr, num);
			num = ulr * 3;
			ProcessB(ulr, num);
			num = ulr / 3;
			ProcessB(ulr, num);
			num = (byte)0;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = (short)0;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = (ushort)0;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0u;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0L;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0uL;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = MpuT.Zero;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), num.Equals(ulr));
			num = MpzT.Zero;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), num.Equals(ulr));
			num = 0f;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0d;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			num = LongReal.Zero;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num), num.Equals(ulr));
		}
		void ProcessB(LongReal ulr, dynamic num)
		{
			dynamic num2 = (byte)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (short)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (ushort)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (int)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (uint)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (long)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (ulong)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (MpuT)(num < 0 ? -num : num);
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), num2.Equals(ulr));
			num2 = (MpzT)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), num2.Equals(ulr));
			num2 = (float)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (double)num;
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpzT)ulr) && ((MpzT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (LongReal)num;
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), ulr.Equals(num2));
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), ulr.Equals((object)num2));
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), num2.Equals(ulr));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestGeometricMean()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 100000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr = new(r, MantissaLength);
			if (random.Next(1000) != 0)
			{
				bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
				if (random.Next(2) == 0)
					bytes.Resize(8);
				else
					bytes.ResizeLeft(8);
			}
			var uz2 = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr2 = new(uz2, MantissaLength);
			if (LongReal.IsNaN(lr) || LongReal.IsNaN(lr2))
				Assert.IsTrue(LongReal.IsNaN(LongReal.GeometricMean(lr, lr2)));
			else if (lr == 0 || lr2 == 0)
				Assert.IsTrue(LongReal.IsZero(LongReal.GeometricMean(lr, lr2)));
			else if (lr < 0 ^ lr2 < 0)
				Assert.IsTrue(LongReal.IsNaN(LongReal.GeometricMean(lr, lr2)));
			else if (lr < 0)
				Assert.IsLessThanOrEqualTo(Max(Sqrt(-r) * Sqrt(-uz2) / (1L << 51), double.Epsilon),
					Abs(Sqrt(-r) * Sqrt(-uz2) + (double)LongReal.GeometricMean(lr, lr2)));
			else
				Assert.IsLessThanOrEqualTo(Max(Sqrt(r) * Sqrt(uz2) / (1L << 51), double.Epsilon),
					Abs(Sqrt(r) * Sqrt(uz2) - (double)LongReal.GeometricMean(lr, lr2)));
		}
	}

	[TestMethod]
	public void TestLog()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 10000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr = new(r, MantissaLength);
			if (LongReal.IsNaN(lr))
				Assert.IsTrue(LongReal.IsNaN(LongReal.Log(lr)));
			else
				Assert.AreEqual(Log(r), (double)LongReal.Log(lr));
		}
	}

	[TestMethod]
	public void TestLogProperties()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 10000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var baseA = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shiftA = new MpzT(bytes.AsSpan(), RandomOrder());
			var a = new LongReal(baseA, MantissaLength).Shift(shiftA);
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var baseB = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shiftB = new MpzT(bytes.AsSpan(), RandomOrder());
			var b = new LongReal(baseB, MantissaLength).Shift(shiftB);
			var logA = LongReal.Log(a);
			var logB = LongReal.Log(b);
			var logProd = LongReal.Log(a * b);
			var logQuot = LongReal.Log(a / b);
			var logAAbs = logA.Abs();
			var logBAbs = logB.Abs();
			Assert.IsLessThanOrEqualTo(logAAbs + logBAbs >> MantissaLength - 2, (logA + logB - logProd).Abs());
			Assert.IsLessThanOrEqualTo(logAAbs + logBAbs >> MantissaLength - 2, (logA - logB - logQuot).Abs());
			Assert.AreEqual(a.CompareTo(b), LongReal.Log(a).CompareTo(LongReal.Log(b)));
			Assert.AreEqual(a.CompareTo(b), logA.CompareTo(logB));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestShiftsDouble()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr = new(r, MantissaLength);
			var shiftAmount = random.Next(257);
			Assert.AreEqual(r * Pow(2, shiftAmount), (double)(lr << shiftAmount));
			Assert.AreEqual(r * Pow(2, shiftAmount), (double)(lr << (UnsignedLongReal)shiftAmount));
			Assert.AreEqual(r / Pow(2, shiftAmount), (double)(lr >> shiftAmount));
			Assert.AreEqual(r / Pow(2, shiftAmount), (double)(lr >> (UnsignedLongReal)shiftAmount));
		}
	}

	[TestMethod]
	public void TestShiftsMpuT()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(259), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			LongReal lr = new(uz, MantissaLength);
			var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
			bytes.FillInPlace(random.Next(3), _ => (byte)random.Next(256));
			bytes.PadRightInPlace(4);
			shiftAmount = BitConverter.ToInt32(bytes.AsSpan());
			Assert.IsLessThanOrEqualTo(uz << shiftAmount >> MantissaLength, (uz << shiftAmount) - (lr << shiftAmount));
			Assert.IsLessThanOrEqualTo(MpuT.Max(uz >> MantissaLength, 1),
				uz.ShiftRightRound(shiftAmount) - (lr >> shiftAmount));
			Assert.IsLessThanOrEqualTo(uz << shiftAmount >> MantissaLength,
				(uz << shiftAmount) - (lr << (UnsignedLongReal)shiftAmount));
			Assert.IsLessThanOrEqualTo(MpuT.Max(uz >> MantissaLength, 1),
				uz.ShiftRightRound(shiftAmount) - (lr >> (UnsignedLongReal)shiftAmount));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestSqrt()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 100000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongReal lr = new(r, MantissaLength);
			if (LongReal.IsNaN(lr))
				Assert.IsTrue(LongReal.IsNaN(LongReal.Sqrt(lr)));
			else
				Assert.AreEqual(Sqrt(r), (double)LongReal.Sqrt(lr));
		}
	}

	[TestMethod]
	public void TestSqrt2()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 100000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var @base = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shift = new MpzT(bytes.AsSpan(), RandomOrder());
			var lr = new LongReal(@base, MantissaLength).Shift(shift);
			var sqrt = LongReal.Sqrt(lr);
			Assert.IsLessThanOrEqualTo(sqrt * sqrt >> MantissaLength - 2, (sqrt * sqrt - lr).Abs());
			Assert.IsLessThanOrEqualTo(lr >> MantissaLength - 2, (sqrt * sqrt - lr).Abs());
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToByteArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 250000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			var order = RandomOrder();
			if (order < 0)
				bytes.Resize(Max(bytes.FindLastIndex(x => x != 0), 0) + 1);
			else
				bytes.ResizeLeft(Max(bytes.Length, 1) - Max(bytes.FindIndex(x => x != 0), 0));
			var mantissaLength = random.Next(32, Max(bytes.Length * 8, 32));
			LongReal lr = new(bytes.AsSpan(), order, mantissaLength);
			LongReal lr2 = new(lr.ToByteArray(order, false), order, mantissaLength);
			Assert.IsTrue(LongReal.IsNaN(lr) && LongReal.IsNaN(lr2) || lr.Equals(lr2));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	[DataRow(1, 0, "1E+0")]     // 1 * 2^0 = 1
	[DataRow(1, 1, "2E+0")]     // 1 * 2^1 = 2
	[DataRow(1, 2, "4E+0")]     // 1 * 2^2 = 4
	[DataRow(3, 3, "2.4E+1")]  // 3 * 2^3 = 24
	[DataRow(5, -2, "1.25E+0")] // 5 * 2^-2 = 1.25
	public void TestToString_BasicNumbers(long a, int n, string expected)
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		var longReal = new LongReal(a).Shift(n);
		var result = longReal.ToString("E6");
		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public void TestToString_Complex()
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		var longReal = new LongReal(123).Shift(50);
		var result = longReal.ToString("E4");
		Assert.AreEqual("1.3849E+17", result);
		longReal = new LongReal(1000).Shift(-10);
		result = longReal.ToString("F6", CultureInfo.GetCultureInfo("en-US"));
		Assert.AreEqual("0.976563", result);
		var largeDigits = "123456789";
		var mpz = MpzT.Parse(largeDigits);
		longReal = new LongReal(mpz).Shift(20);
		result = longReal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		Assert.Contains("129 453 825 982 464", result);
		longReal = new LongReal(1).Shift(100);
		result = longReal.ToString("E2");
		Assert.AreEqual("1.27E+30", result);
		foreach (var (number, format, en, ru, de) in CultureTestData())
		{
			longReal = number;
			var enResult = longReal.ToString(format, CultureInfo.GetCultureInfo("en-US"));
			Assert.AreEqual(en, enResult);
			var ruResult = longReal.ToString(format, CultureInfo.GetCultureInfo("ru-RU"));
			Assert.AreEqual(ru, ruResult);
			var deResult = longReal.ToString(format, CultureInfo.GetCultureInfo("de-DE"));
			Assert.AreEqual(de, deResult);
		}
		mpz = new MpzT(77).Power(77);
		longReal = new LongReal(1).Shift(mpz);
		result = longReal.ToString("E6");
		Assert.AreEqual("1.358443E+5475144815987627762430594775150486533643549212522238631644821558595137232066160304681082998798877694978398467245688991276872900744519537448240061", result);
	}

	public static G.IEnumerable<(LongReal number, string format, string en, string ru, string de)> CultureTestData()
	{
		yield return (new LongReal(15L).Shift(12), "F2", "61,440.00", "61 440,00", "61.440,00");
		yield return (new LongReal(-987L).Shift(-8), "E3", "-3.855E+0", "-3,855E+0", "-3,855E+0");
		yield return (new(123456.789), "N5", "123,456.78900", "123 456,78900", "123.456,78900");
	}
}
