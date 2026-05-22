using System.Globalization;

namespace NStar.BigCollections.Tests;

[TestClass]
public class LongDecimalTests
{
	private static readonly int MantissaLength = 1000;
	private static readonly int MantissaByteLength = GetArrayLength(MantissaLength, 8);

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
		LongDecimal lr = new(r, MantissaLength);
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
				lr += op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				r -= op;
				lr -= op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong) random.NextInt64() +(random.Next(2) == 0 ? 0 : 1uL << 63));
				r *= op;
				lr *= op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong) random.NextInt64() +(random.Next(2) == 0 ? 0 : 1uL << 63));
				if (op.Equals(0))
					return;
				r /= op;
				lr /= op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				var order = lr.Abs() < 1 ? -(int)(1 / lr).Order : (int)lr.Order;
				r %= op;
				lr %= op;
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
		void Validate() => Assert.IsTrue(r.Equals((double)lr) || r is double.NaN && (double)lr is double.NaN);
		void ValidateRemainder(int validOrder) => Assert.IsTrue(Abs(r - (double)lr) < ((LongDecimal)1).Shift(validOrder));
	}

	[TestMethod]
	public void TestCompareTo()
	{
		var x = new LongDecimal(123).Shift(456); // мантисса = 123, экспонента = 456
		var y = new LongDecimal(123).Shift(456);
		var result = x.CompareTo(y);
		Assert.AreEqual(0, result);
		x = new LongDecimal(100).Shift(50);
		y = new LongDecimal(200).Shift(50);
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		x = new LongDecimal(100).Shift(1000);
		y = new LongDecimal(100).Shift(2000);
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		// Очень большие экспоненты
		x = new LongDecimal(1).Shift(int.MaxValue);      // экспонента = 2 147 483 647
		y = new LongDecimal(1).Shift(int.MaxValue + 1L); // экспонента = 2 147 483 648
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		// Очень маленькие (отрицательные) экспоненты
		x = new LongDecimal(1).Shift(int.MinValue);      // экспонента = -2 147 483 648
		y = new LongDecimal(1).Shift(int.MinValue - 1L); // экспонента = -2 147 483 649
		Assert.AreEqual(1, x.CompareTo(y));  // x > y, т.к. -2 147 483 648 > -2 147 483 649
		Assert.AreEqual(-1, y.CompareTo(x));
		x = new LongDecimal(-1).Shift(int.MinValue);      // экспонента = -2 147 483 648
		y = new LongDecimal(-1).Shift(int.MinValue - 1L); // экспонента = -2 147 483 649
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		x = new LongDecimal(500).Shift(int.MaxValue);    // очень большое число
		y = new LongDecimal(500).Shift(int.MinValue);    // очень маленькое число
		Assert.AreEqual(1, x.CompareTo(y));
		Assert.AreEqual(-1, y.CompareTo(x));
		x = new LongDecimal(0).Shift(0);
		y = new LongDecimal(1).Shift(0);
		Assert.AreEqual(-1, x.CompareTo(y));
		Assert.AreEqual(1, y.CompareTo(x));
		x = new LongDecimal(-100).Shift(50);
		y = new LongDecimal(-200).Shift(50);
		var z = new LongDecimal(100).Shift(50);
		Assert.AreEqual(1, x.CompareTo(y));   // -100 > -200
		Assert.AreEqual(-1, y.CompareTo(x)); // -200 < -100
		Assert.AreEqual(-1, x.CompareTo(z));   // -100 < 100
		x = new LongDecimal(100).Shift(50);
		y = new LongDecimal(-100).Shift(50);
		Assert.AreEqual(1, x.CompareTo(y));
		Assert.AreEqual(-1, y.CompareTo(x));
		// Числа с нулевой мантиссой
		x = new LongDecimal(0).Shift(int.MaxValue);
		y = new LongDecimal(0).Shift(int.MinValue);
		Assert.AreEqual(0, x.CompareTo(y));
		// Крайние случаи: максимально возможная разница в экспонентах
		x = new LongDecimal(1).Shift(long.MaxValue);
		y = new LongDecimal(1).Shift(long.MinValue);
		Assert.AreEqual(1, x.CompareTo(y));
		Assert.AreEqual(-1, y.CompareTo(x));
		x = new LongDecimal(1).Shift(1);
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
			LongDecimal lr = new(r, MantissaLength);
			if (random.Next(1000) != 0)
			{
				bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
				if (random.Next(2) == 0)
					bytes.Resize(8);
				else
					bytes.ResizeLeft(8);
			}
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			LongDecimal lr2 = new(r2, (int)Round(MantissaLength * (random.NextDouble() * 2 - 1)));
			if (LongDecimal.IsNaN(lr) || LongDecimal.IsNaN(lr2))
				Assert.AreEqual(int.MinValue, lr.CompareTo(lr2));
			else
				Assert.AreEqual(Sign(r.CompareTo(r2)), Sign(lr.CompareTo(lr2)));
		}
		Assert.Throws<ArgumentNullException>(() => x.CompareTo((MpzT)null!));
		Assert.Throws<ArgumentNullException>(() => x.CompareTo((MpuT)null!));
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
			LongDecimal lr = new(bytes.AsSpan(), order, MantissaLength);
			if (bytes.Length - MantissaByteLength == 4)
				continue;
			ProcessA(lr);
		}
		void ProcessA(LongDecimal lr)
		{
			dynamic num = lr;
			ProcessB(lr, num);
			num = lr + 1;
			ProcessB(lr, num);
			if (lr.CompareTo(0) != 0)
			{
				num = lr - 1;
				ProcessB(lr, num);
			}
			num = lr * 2;
			ProcessB(lr, num);
			num = lr / 2;
			ProcessB(lr, num);
			num = lr * 3;
			ProcessB(lr, num);
			num = lr / 3;
			ProcessB(lr, num);
			num = (byte)0;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = (short)0;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = (ushort)0;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = 0;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = 0u;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = 0L;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = 0uL;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = MpuT.Zero;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), num.Equals(lr));
			num = MpzT.Zero;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), num.Equals(lr));
			num = 0f;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = 0d;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			num = LongDecimal.Zero;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals(num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), lr.Equals((object)num));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num), num.Equals(lr));
		}
		void ProcessB(LongDecimal lr, dynamic num)
		{
			dynamic num2 = (byte)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (short)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (ushort)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (int)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (uint)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (long)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (ulong)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (MpuT)(num < 0 ? -num : num);
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), num2.Equals(lr));
			num2 = (MpzT)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), num2.Equals(lr));
			num2 = (float)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (double)num;
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals(num2));
			Assert.AreEqual(lr.Equals((MpzT)lr) && ((MpzT)lr).Equals(num2), lr.Equals((object)num2));
			num2 = (LongDecimal)num;
			Assert.AreEqual(E.SequenceEqual(lr.ToByteArray(-1), num2.ToByteArray(-1)), lr.Equals(num2));
			Assert.AreEqual(E.SequenceEqual(lr.ToByteArray(-1), num2.ToByteArray(-1)), lr.Equals((object)num2));
			Assert.AreEqual(E.SequenceEqual(lr.ToByteArray(-1), num2.ToByteArray(-1)), num2.Equals(lr));
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
			LongDecimal lr = new(r, MantissaLength);
			if (random.Next(1000) != 0)
			{
				bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
				if (random.Next(2) == 0)
					bytes.Resize(8);
				else
					bytes.ResizeLeft(8);
			}
			var uz2 = BitConverter.ToDouble(bytes.AsSpan());
			LongDecimal lr2 = new(uz2, MantissaLength);
			if (LongDecimal.IsNaN(lr) || LongDecimal.IsNaN(lr2))
				Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.GeometricMean(lr, lr2)));
			else if (lr == 0 || lr2 == 0)
				Assert.IsTrue(LongDecimal.IsZero(LongDecimal.GeometricMean(lr, lr2)));
			else if (lr < 0 ^ lr2 < 0)
				Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.GeometricMean(lr, lr2)));
			else if (lr < 0)
				Assert.IsLessThanOrEqualTo(Max(Sqrt(-r) * Sqrt(-uz2) / (1L << 51), double.Epsilon),
					Abs(Sqrt(-r) * Sqrt(-uz2) + (double)LongDecimal.GeometricMean(lr, lr2)));
			else
				Assert.IsLessThanOrEqualTo(Max(Sqrt(r) * Sqrt(uz2) / (1L << 51), double.Epsilon),
					Abs(Sqrt(r) * Sqrt(uz2) - (double)LongDecimal.GeometricMean(lr, lr2)));
		}
	}

	[TestMethod]
	public void TestInverseTrigonometry()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 5000; i++)
		{
			var r = random.NextDouble() * (random.Next(2) == 0 ? -1 : 1);
			var lr = new LongDecimal(r, MantissaLength);
			var lr2 = lr.Asin().Sin();
			var lr3 = lr.Acos().Cos();
			var lr4 = lr.Atan().Tan();
			Assert.IsLessThanOrEqualTo(LongDecimal.One >> MantissaLength, (lr - lr2).Abs());
			Assert.IsLessThanOrEqualTo(LongDecimal.One >> MantissaLength, (lr - lr3).Abs());
			Assert.IsLessThanOrEqualTo(LongDecimal.One >> MantissaLength - 16, (lr - lr4).Abs());
		}
	}

	[TestMethod]
	public void TestLog()
	{
		Assert.AreEqual(LongDecimal.NegativeInfinity, LongDecimal.Zero.Log());
		Assert.AreEqual(LongDecimal.PositiveInfinity, LongDecimal.PositiveInfinity.Log());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NegativeInfinity.Log()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Log()));
		Assert.AreEqual(LongDecimal.Zero, LongDecimal.One.Log());
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
			LongDecimal lr = new(r, MantissaLength);
			if (LongDecimal.IsNaN(lr))
				Assert.IsTrue(LongDecimal.IsNaN(lr.Log()));
			else
				Assert.AreEqual(Log(r), (double)lr.Log());
		}
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var baseA = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shiftA = new MpzT(bytes.AsSpan(), RandomOrder());
			var a = new LongDecimal(baseA, MantissaLength).Shift(shiftA);
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var baseB = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shiftB = new MpzT(bytes.AsSpan(), RandomOrder());
			var b = new LongDecimal(baseB, MantissaLength).Shift(shiftB);
			var logA = a.Log();
			var logB = b.Log();
			var logProd = (a * b).Log();
			var logQuot = (a / b).Log();
			var logAAbs = logA.Abs();
			var logBAbs = logB.Abs();
			Assert.IsLessThanOrEqualTo(logAAbs + logBAbs >> MantissaLength - 3, (logA + logB - logProd).Abs());
			Assert.IsLessThanOrEqualTo(logAAbs + logBAbs >> MantissaLength - 3, (logA - logB - logQuot).Abs());
			var log2A = a.Log2();
			var log2B = b.Log2();
			var log2Prod = (a * b).Log2();
			var log2Quot = (a / b).Log2();
			var log2AAbs = log2A.Abs();
			var log2BAbs = log2B.Abs();
			Assert.IsLessThanOrEqualTo(log2AAbs + log2BAbs >> MantissaLength - 3, (log2A + log2B - log2Prod).Abs());
			Assert.IsLessThanOrEqualTo(log2AAbs + log2BAbs >> MantissaLength - 3, (log2A - log2B - log2Quot).Abs());
			Assert.AreEqual(a.CompareTo(b), a.Log().CompareTo(b.Log()));
			Assert.AreEqual(a.CompareTo(b), logA.CompareTo(logB));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestPower()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var longRealThree = new LongDecimal(3);
		Assert.AreEqual(LongDecimal.One, longRealThree.Power(LongDecimal.Zero));
		Assert.AreEqual(LongDecimal.PositiveInfinity, longRealThree.Power(LongDecimal.PositiveInfinity));
		Assert.AreEqual(LongDecimal.Zero, longRealThree.Power(LongDecimal.NegativeInfinity));
		Assert.IsTrue(LongDecimal.IsNaN(longRealThree.Power(LongDecimal.NaN)));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var uz = new MpuT(bytes.AsSpan(), RandomOrder());
			var lr = new LongDecimal(uz, MantissaLength);
			var lr2 = longRealThree.Power(lr).Log(longRealThree);
			Assert.IsLessThanOrEqualTo(lr >> MantissaLength - 8, (lr - lr2).Abs());
		}
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var @base = new MpuT(bytes.AsSpan(), RandomOrder());
			var shift = random.Next();
			var lr = new LongDecimal(@base, MantissaLength).Shift(shift);
			var lr2 = longRealThree.Power(lr.Log(longRealThree));
			Assert.IsLessThanOrEqualTo(lr >> MantissaLength - 50, (lr - lr2).Abs());
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestReciproc()
	{
		Assert.AreEqual(LongDecimal.PositiveInfinity, LongDecimal.Zero.Reciproc());
		Assert.AreEqual(LongDecimal.Zero, LongDecimal.PositiveInfinity.Reciproc());
		Assert.AreEqual(LongDecimal.Zero, LongDecimal.NegativeInfinity.Reciproc());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Reciproc()));
		Assert.AreEqual(LongDecimal.One, LongDecimal.One.Reciproc());
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var @base = new MpzT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shift = new MpzT(bytes.AsSpan(), RandomOrder());
			var lr = new LongDecimal(@base, MantissaLength).Shift(shift);
			var lr2 = 1 / lr.Reciproc();
			Assert.IsLessThanOrEqualTo(lr.Abs() >> MantissaLength - 1, (lr - lr2).Abs());
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestShifts()
	{
		Assert.AreEqual(LongDecimal.Zero, LongDecimal.Zero << 3);
		Assert.AreEqual(LongDecimal.PositiveInfinity, LongDecimal.PositiveInfinity << 3);
		Assert.AreEqual(LongDecimal.NegativeInfinity, LongDecimal.NegativeInfinity << 3);
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN << 3));
		Assert.AreEqual(LongDecimal.Zero, LongDecimal.Zero >> 3);
		Assert.AreEqual(LongDecimal.PositiveInfinity, LongDecimal.PositiveInfinity >> 3);
		Assert.AreEqual(LongDecimal.NegativeInfinity, LongDecimal.NegativeInfinity >> 3);
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN >> 3));
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
			LongDecimal lr = new(r, MantissaLength);
			var shiftAmount = random.Next(257);
			Assert.AreEqual(r * Pow(2, shiftAmount), (double)(lr << shiftAmount));
			Assert.AreEqual(r * Pow(2, shiftAmount), (double)(lr << (UnsignedLongDecimal)shiftAmount));
			Assert.AreEqual(r / Pow(2, shiftAmount), (double)(lr >> shiftAmount));
			Assert.AreEqual(r / Pow(2, shiftAmount), (double)(lr >> (UnsignedLongDecimal)shiftAmount));
		}
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(259), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			LongDecimal lr = new(uz, MantissaLength);
			var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
			bytes.FillInPlace(random.Next(3), _ => (byte)random.Next(256));
			bytes.PadRightInPlace(4);
			shiftAmount = BitConverter.ToInt32(bytes.AsSpan());
			Assert.IsLessThanOrEqualTo(uz << shiftAmount >> MantissaLength, (uz << shiftAmount) - (lr << shiftAmount));
			Assert.IsLessThanOrEqualTo(MpuT.Max(uz >> MantissaLength, 1),
				uz.ShiftRightRound(shiftAmount) - (lr >> shiftAmount));
			Assert.IsLessThanOrEqualTo(uz << shiftAmount >> MantissaLength,
				(uz << shiftAmount) - (lr << (UnsignedLongDecimal)shiftAmount));
			Assert.IsLessThanOrEqualTo(MpuT.Max(uz >> MantissaLength, 1),
				uz.ShiftRightRound(shiftAmount) - (lr >> (UnsignedLongDecimal)shiftAmount));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestSqrt()
	{
		Assert.AreEqual(LongDecimal.Zero, LongDecimal.Zero.Sqrt());
		Assert.AreEqual(LongDecimal.PositiveInfinity, LongDecimal.PositiveInfinity.Sqrt());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NegativeInfinity.Sqrt()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Sqrt()));
		Assert.AreEqual(LongDecimal.One, LongDecimal.One.Sqrt());
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
			LongDecimal lr = new(r, MantissaLength);
			if (LongDecimal.IsNaN(lr))
				Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.Sqrt(lr)));
			else
				Assert.AreEqual(Sqrt(r), (double)LongDecimal.Sqrt(lr));
		}
		for (var i = 0; i < 100000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var @base = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shift = new MpzT(bytes.AsSpan(), RandomOrder());
			var lr = new LongDecimal(@base, MantissaLength).Shift(shift);
			var sqrt = LongDecimal.Sqrt(lr);
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
			LongDecimal lr = random.Next(1000) switch
			{
				0 => new(0d, mantissaLength),
				1 => new(double.PositiveInfinity, mantissaLength),
				2 => new(double.NegativeInfinity, mantissaLength),
				3 => new(double.NaN, mantissaLength),
				4 => new(double.NegativeZero, mantissaLength),
				_ => new(bytes.AsSpan(), order, mantissaLength),
			};
			LongDecimal lr2 = new(lr.ToByteArray(order, false), order, mantissaLength);
			Assert.IsTrue(LongDecimal.IsNaN(lr) && LongDecimal.IsNaN(lr2) || lr.Equals(lr2));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToDouble()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongDecimal lr = new(r, MantissaLength);
			Assert.AreEqual(r, (double)lr);
		}
		for (var i = 0; i < 100; i++)
		{
			var lr = new LongDecimal(random.Next()).Shift(-random.Next());
			Assert.AreEqual(UnsignedLongDecimal.Zero, (UnsignedLongDecimal)lr);
		}
	}

	[TestMethod]
	public void TestToString()
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		var longReal = new LongDecimal(1).Shift(0);
		var result = longReal.ToString("E6");
		Assert.AreEqual("1E+0", result);
		longReal = new LongDecimal(1).Shift(1);
		result = longReal.ToString("E6");
		Assert.AreEqual("2E+0", result);
		longReal = new LongDecimal(1).Shift(2);
		result = longReal.ToString("E6");
		Assert.AreEqual("4E+0", result);
		longReal = new LongDecimal(3).Shift(3);
		result = longReal.ToString("E6");
		Assert.AreEqual("2.4E+1", result);
		longReal = new LongDecimal(5).Shift(-2);
		result = longReal.ToString("E6");
		Assert.AreEqual("1.25E+0", result);
		longReal = new LongDecimal(123).Shift(50);
		result = longReal.ToString("E4");
		Assert.AreEqual("1.3849E+17", result);
		longReal = new LongDecimal(1000).Shift(-10);
		result = longReal.ToString("F6", CultureInfo.GetCultureInfo("en-US"));
		Assert.AreEqual("0.976563", result);
		var largeDigits = "123456789";
		var mpz = MpzT.Parse(largeDigits);
		longReal = new LongDecimal(mpz).Shift(20);
		result = longReal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		Assert.Contains("129 453 825 982 464", result);
		longReal = new LongDecimal(1).Shift(100);
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
		//mpz = new MpzT(77).Power(77);
		//longReal = new LongReal(1).Shift(mpz);
		//result = longReal.ToString("E6");
		//Assert.AreEqual("1.358443E+5475144815987627762430594775150486533643549212522238631644821558595137232066160304681082998798877694978398467245688991276872900744519537448240061", result);
	}

	[TestMethod]
	public void TestTrigonometry()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		Assert.AreEqual(0, LongDecimal.Zero.Sin());
		Assert.AreEqual(1, (LongDecimal.Pi >> 1).Sin());
		Assert.AreEqual(0, LongDecimal.Pi.Sin());
		Assert.AreEqual(-1, (3 * LongDecimal.Pi >> 1).Sin());
		Assert.AreEqual(0, (LongDecimal.Pi << 1).Sin());
		Assert.AreEqual(-1, (-LongDecimal.Pi >> 1).Sin());
		Assert.AreEqual(0, (-LongDecimal.Pi).Sin());
		Assert.AreEqual(1, (-3 * LongDecimal.Pi >> 1).Sin());
		Assert.AreEqual(0, (-LongDecimal.Pi << 1).Sin());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.PositiveInfinity.Sin()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NegativeInfinity.Sin()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Sin()));
		Assert.AreEqual(1, LongDecimal.Zero.Cos());
		Assert.AreEqual(0, (LongDecimal.Pi >> 1).Cos());
		Assert.AreEqual(-1, LongDecimal.Pi.Cos());
		Assert.AreEqual(0, (3 * LongDecimal.Pi >> 1).Cos());
		Assert.AreEqual(1, (LongDecimal.Pi << 1).Cos());
		Assert.AreEqual(0, (-LongDecimal.Pi >> 1).Cos());
		Assert.AreEqual(-1, (-LongDecimal.Pi).Cos());
		Assert.AreEqual(0, (-3 * LongDecimal.Pi >> 1).Cos());
		Assert.AreEqual(1, (-LongDecimal.Pi << 1).Cos());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.PositiveInfinity.Cos()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NegativeInfinity.Cos()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Cos()));
		for (var i = 0; i < 10000; i++)
		{
			var r = Pow(2, random.NextDouble() * 128 - 64);
			LongDecimal lr = new(r, MantissaLength);
			if (LongDecimal.IsNaN(lr))
			{
				Assert.IsTrue(LongDecimal.IsNaN(lr.Sin()));
				Assert.IsTrue(LongDecimal.IsNaN(lr.Cos()));
				Assert.IsTrue(LongDecimal.IsNaN(lr.Tan()));
			}
			else
			{
				Assert.IsLessThanOrEqualTo(Pow(2, -52), Abs(Sin(r) - (double)lr.Sin()));
				Assert.IsLessThanOrEqualTo(Pow(2, -52), Abs(Cos(r) - (double)lr.Cos()));
				Assert.IsLessThanOrEqualTo(Max(Abs(Tan(r)), 1) / Pow(2, 52), Abs(Tan(r) - (double)lr.Tan()));
			}
		}
		for (var i = 0; i < 10000; i++)
		{
			var r = (1 - random.NextDouble()) * PI / 2;
			LongDecimal lr = new(r, MantissaLength);
			Assert.IsLessThan(lr, lr.Sin());
			Assert.IsGreaterThan(lr, lr.Tan());
		}
	}

	private static G.IEnumerable<(LongDecimal number, string format, string en, string ru, string de)> CultureTestData()
	{
		yield return (new LongDecimal(15L).Shift(12), "F2", "61,440.00", "61 440,00", "61.440,00");
		yield return (new LongDecimal(-987L).Shift(-8), "E3", "-3.855E+0", "-3,855E+0", "-3,855E+0");
		yield return (new(123456.789), "N5", "123,456.78900", "123 456,78900", "123.456,78900");
	}

	[TestMethod]
	public void TestToUnsignedLongReal()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(1, 501), _ => (byte)random.Next(256));
			var order = RandomOrder();
			UnsignedLongDecimal ulr = new(bytes.AsSpan(), order, MantissaLength);
			if (bytes.Length - MantissaByteLength == 4)
				continue;
			LongDecimal lr = ulr;
			Assert.AreEqual(ulr, (UnsignedLongDecimal)lr);
		}
		for (var i = 0; i < 100; i++)
		{
			var lr = new LongDecimal(random.Next()).Shift(-random.Next());
			Assert.AreEqual(UnsignedLongDecimal.Zero, (UnsignedLongDecimal)lr);
		}
		Assert.ThrowsExactly<OverflowException>(() => (UnsignedLongDecimal)LongDecimal.PositiveInfinity);
		Assert.ThrowsExactly<OverflowException>(() => (UnsignedLongDecimal)LongDecimal.NegativeInfinity);
		Assert.ThrowsExactly<OverflowException>(() => (UnsignedLongDecimal)LongDecimal.NaN);
		int RandomOrder() => random.Next(2) * 2 - 1;
	}
}
