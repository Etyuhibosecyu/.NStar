namespace NStar.Mpir.Tests;

[TestClass]
public class LongDecimalTests
{
	private static readonly int MantissaLength = 300;
	private static readonly int MantissaByteLength = (int)Ceiling((MantissaLength + Log10(36)) * Log(10, 256));

	[TestMethod]
	public void ComplexTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
	l1:
		var d = ConstructDecimal(bytes);
		LongDecimal ld = new(d, MantissaLength);
		Validate();
		var actions = new[]
		{
			() =>
			{
				var op = (byte)random.Next(256);
				if ((double)d + op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d += op;
				ld += op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if ((double)d - op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d -= op;
				ld -= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if ((double)d * op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d *= op;
				ld *= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				if ((double)d / op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d /= op;
				ld /= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				var order = ld.Abs() < 1 ? -(int)(1 / ld).Order : (int)ld.Order;
				if (op.Equals(0))
					return;
				d %= op;
				ld %= op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = random.Next();
				if ((double)d + op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d += op;
				ld += op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if ((double)d - op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d -= op;
				ld -= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if ((double)d * op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d *= op;
				ld *= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				if ((double)d / op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d /= op;
				ld /= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				var order = ld.Abs() < 1 ? -(int)(1 / ld).Order : (int)ld.Order;
				if (op.Equals(0))
					return;
				d %= op;
				ld %= op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if ((double)d + op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d += op;
				ld += op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if ((double)d - op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d -= op;
				ld -= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if ((double)d * op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d *= op;
				ld *= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				if ((double)d / op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d /= op;
				ld /= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				var order = ld.Abs() < 1 ? -(int)(1 / ld).Order : (int)ld.Order;
				if (op.Equals(0))
					return;
				d %= op;
				ld %= op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = random.NextInt64();
				if ((double)d + op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d += op;
				ld += (decimal)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if ((double)d - op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d -= op;
				ld -= (decimal)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if ((double)d * op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d *= op;
				ld *= (decimal)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				if ((double)d / op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d /= op;
				ld /= (decimal)op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				var order = ld.Abs() < 1 ? -(int)(1 / ld).Order : (int)ld.Order;
				if (op.Equals(0))
					return;
				d %= op;
				ld %= (decimal)op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if ((double)d + op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d += op;
				ld += (decimal)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if ((double)d - op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d -= op;
				ld -= (decimal)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if ((double)d * op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d *= op;
				ld *= (decimal)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				if ((double)d / op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d /= op;
				ld /= (decimal)op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				var order = ld.Abs() < 1 ? -(int)(1 / ld).Order : (int)ld.Order;
				if (op.Equals(0))
					return;
				d %= op;
				ld %= (decimal)op;
				ValidateRemainder(order - 52);
			}, () =>
			{
				var op = ConstructDecimal(bytes);
				if ((double)d + (double)op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d += op;
				ld += op;
				Validate();
			}, () =>
			{
				var op = ConstructDecimal(bytes);
				if ((double)d - (double)op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d -= op;
				ld -= op;
				Validate();
			}, () =>
			{
				var op = ConstructDecimal(bytes);
				if ((double)d * (double)op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d *= op;
				ld *= op;
				Validate();
			}, () =>
			{
				var op = ConstructDecimal(bytes);
				if (op.Equals(0))
					return;
				if ((double)d / (double)op is < (double)decimal.MinValue or > (double)decimal.MaxValue)
					return;
				d /= op;
				ld /= op;
				Validate();
			}, () =>
			{
				var op = ConstructDecimal(bytes);
				var order = ld.Abs() < 1 ? -(int)(1 / ld).Order : (int)ld.Order;
				if (op.Equals(0))
					return;
				d %= op;
				ld %= op;
				ValidateRemainder(order - 52);
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			if (random.Next(100) == 0)
				d = ConstructDecimal(bytes);
			ld = new(d, MantissaLength);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		void Validate() => Assert.AreEqual(d, (decimal)ld);
		void ValidateRemainder(int validOrder) => Assert.IsTrue(Abs(d - (decimal)ld) < ((LongDecimal)1).Shift(validOrder));
	}

	private static decimal ConstructDecimal(List<byte> bytes)
	{
		bytes.FillInPlace(random.Next(17), _ => (byte)random.Next(256));
		if (random.Next(2) == 0)
			bytes.Resize(16);
		else
			bytes.ResizeLeft(16);
		var i1 = BitConverter.ToInt32(bytes.AsSpan());
		var i2 = BitConverter.ToInt32(bytes.AsSpan(4));
		var i3 = BitConverter.ToInt32(bytes.AsSpan(8));
		var i4 = BitConverter.ToInt32(bytes.AsSpan(12));
		i4 = (i4 & int.MaxValue) % 29 << 16 | i4 & int.MinValue;
		return new([i1, i2, i3, i4]);
	}

	private static G.IEnumerable<(LongDecimal number, string format, string en, string ru, string de)> CultureTestData()
	{
		yield return (new LongReal(15L).Shift(12), "F2", "61,440.00", "61 440,00", "61.440,00");
		yield return (new LongReal(-987L).Shift(-8), "E3", "-3.855E+0", "-3,855E+0", "-3,855E+0");
		yield return (new(123456.789), "N5", "123,456.78900", "123 456,78900", "123.456,78900");
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
		Assert.AreEqual(-1, Sign(x.CompareTo(y)));
		Assert.AreEqual(1, Sign(y.CompareTo(x)));
		x = new LongDecimal(100).Shift(50);
		y = new LongDecimal(150).Shift(50);
		Assert.AreEqual(-1, Sign(x.CompareTo(y)));
		Assert.AreEqual(1, Sign(y.CompareTo(x)));
		x = new LongDecimal(100).Shift(1000);
		y = new LongDecimal(100).Shift(2000);
		Assert.AreEqual(-1, Sign(x.CompareTo(y)));
		Assert.AreEqual(1, Sign(y.CompareTo(x)));
		// Очень большие экспоненты
		x = new LongDecimal(1).Shift(int.MaxValue);      // экспонента = 2 147 483 647
		y = new LongDecimal(1).Shift(int.MaxValue + 1L); // экспонента = 2 147 483 648
		Assert.AreEqual(-1, Sign(x.CompareTo(y)));
		Assert.AreEqual(1, Sign(y.CompareTo(x)));
		// Очень маленькие (отрицательные) экспоненты
		x = new LongDecimal(1).Shift(int.MinValue);      // экспонента = -2 147 483 648
		y = new LongDecimal(1).Shift(int.MinValue - 1L); // экспонента = -2 147 483 649
		Assert.AreEqual(1, Sign(x.CompareTo(y)));  // x > y, т.к. -2 147 483 648 > -2 147 483 649
		Assert.AreEqual(-1, Sign(y.CompareTo(x)));
		x = new LongDecimal(-1).Shift(int.MinValue);      // экспонента = -2 147 483 648
		y = new LongDecimal(-1).Shift(int.MinValue - 1L); // экспонента = -2 147 483 649
		Assert.AreEqual(-1, Sign(x.CompareTo(y)));
		Assert.AreEqual(1, Sign(y.CompareTo(x)));
		x = new LongDecimal(500).Shift(int.MaxValue);    // очень большое число
		y = new LongDecimal(500).Shift(int.MinValue);    // очень маленькое число
		Assert.AreEqual(1, Sign(x.CompareTo(y)));
		Assert.AreEqual(-1, Sign(y.CompareTo(x)));
		x = new LongDecimal(0).Shift(0);
		y = new LongDecimal(1).Shift(0);
		Assert.AreEqual(-1, Sign(x.CompareTo(y)));
		Assert.AreEqual(1, Sign(y.CompareTo(x)));
		x = new LongDecimal(-100).Shift(50);
		y = new LongDecimal(-200).Shift(50);
		var z = new LongDecimal(100).Shift(50);
		Assert.AreEqual(1, Sign(x.CompareTo(y)));   // -100 > -200
		Assert.AreEqual(-1, Sign(y.CompareTo(x))); // -200 < -100
		Assert.AreEqual(-1, x.CompareTo(z));   // -100 < 100
		x = new LongDecimal(-100).Shift(50);
		y = new LongDecimal(-150).Shift(50);
		Assert.AreEqual(1, Sign(x.CompareTo(y)));   // -100 > -200
		Assert.AreEqual(-1, Sign(y.CompareTo(x))); // -200 < -100
		x = new LongDecimal(100).Shift(50);
		y = new LongDecimal(-100).Shift(50);
		Assert.AreEqual(1, Sign(x.CompareTo(y)));
		Assert.AreEqual(-1, Sign(y.CompareTo(x)));
		// Числа с нулевой мантиссой
		x = new LongDecimal(0).Shift(int.MaxValue);
		y = new LongDecimal(0).Shift(int.MinValue);
		Assert.AreEqual(0, Sign(x.CompareTo(y)));
		// Крайние случаи: максимально возможная разница в экспонентах
		x = new LongDecimal(1).Shift(long.MaxValue);
		y = new LongDecimal(1).Shift(long.MinValue);
		Assert.AreEqual(1, Sign(x.CompareTo(y)));
		Assert.AreEqual(-1, Sign(y.CompareTo(x)));
		x = new LongDecimal(1).Shift(1);
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 10000000; i++)
		{
			var d = ConstructDecimal(bytes);
			LongDecimal ld = new(d, MantissaLength);
			var d2 = random.Next(1000) == 0 ? d : ConstructDecimal(bytes);
			LongDecimal ld2 = new(d2, (int)Round(MantissaLength * (random.NextDouble() * 2 - 1)));
			if (LongDecimal.IsNaN(ld) || LongDecimal.IsNaN(ld2))
				Assert.AreEqual(int.MinValue, ld.CompareTo(ld2));
			else
				Assert.AreEqual(Sign(d.CompareTo(d2)), Sign(ld.CompareTo(ld2)));
		}
		Assert.Throws<ArgumentNullException>(() => x.CompareTo((MpzT)null!));
		Assert.Throws<ArgumentNullException>(() => x.CompareTo((MpuT)null!));
	}

	[TestMethod]
	public void TestCompareToDouble()
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
			LongDecimal ld = new(r, MantissaLength);
			if (random.Next(1000) != 0)
			{
				bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
				if (random.Next(2) == 0)
					bytes.Resize(8);
				else
					bytes.ResizeLeft(8);
			}
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			LongDecimal ld2 = new(r2, MantissaLength);
			if (LongDecimal.IsNaN(ld) || LongDecimal.IsNaN(ld2))
				Assert.AreEqual(int.MinValue, ld.CompareTo(ld2));
			else
				Assert.AreEqual(Sign(r.CompareTo(r2)), Sign(ld.CompareTo(ld2)));
		}
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
			LongDecimal ld = new(bytes.AsSpan(), order, MantissaLength);
			if (bytes.Length - MantissaByteLength is 3 or 4 or 5)
				continue;
			ProcessA(ld);
		}
		void ProcessA(LongDecimal ld)
		{
			dynamic num = ld;
			ProcessB(ld, num);
			num = ld + 1;
			ProcessB(ld, num);
			if (ld.CompareTo(0) != 0)
			{
				num = ld - 1;
				ProcessB(ld, num);
			}
			num = ld * 2;
			ProcessB(ld, num);
			num = ld / 2;
			ProcessB(ld, num);
			num = ld * 3;
			ProcessB(ld, num);
			num = ld / 3;
			ProcessB(ld, num);
			num = (byte)0;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = (short)0;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = (ushort)0;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = 0;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = 0u;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = 0L;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = 0uL;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = MpuT.Zero;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), num.Equals(ld));
			num = MpzT.Zero;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), num.Equals(ld));
			num = 0f;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = 0d;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			num = LongDecimal.Zero;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals(num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), ld.Equals((object)num));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num), num.Equals(ld));
		}
		void ProcessB(LongDecimal ld, dynamic num)
		{
			dynamic num2 = (byte)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (short)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (ushort)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (int)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (uint)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (long)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (ulong)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (MpuT)(num < 0 ? -num : num);
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), num2.Equals(ld));
			num2 = (MpzT)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), num2.Equals(ld));
			num2 = (float)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (double)num;
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals(num2));
			Assert.AreEqual(ld.Equals((MpzT)ld) && ((MpzT)ld).Equals(num2), ld.Equals((object)num2));
			num2 = (LongDecimal)num;
			Assert.AreEqual(E.SequenceEqual(ld.ToByteArray(-1), num2.ToByteArray(-1)), ld.Equals(num2));
			Assert.AreEqual(E.SequenceEqual(ld.ToByteArray(-1), num2.ToByteArray(-1)), ld.Equals((object)num2));
			Assert.AreEqual(E.SequenceEqual(ld.ToByteArray(-1), num2.ToByteArray(-1)), num2.Equals(ld));
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
			LongDecimal ld = new(r, MantissaLength);
			if (random.Next(1000) != 0)
			{
				bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
				if (random.Next(2) == 0)
					bytes.Resize(8);
				else
					bytes.ResizeLeft(8);
			}
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			LongDecimal ld2 = new(r2, MantissaLength);
			if (LongDecimal.IsNaN(ld) || LongDecimal.IsNaN(ld2))
				Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.GeometricMean(ld, ld2)));
			else if (ld == 0 || ld2 == 0)
				Assert.IsTrue(LongDecimal.IsZero(LongDecimal.GeometricMean(ld, ld2)));
			else if (ld < 0 ^ ld2 < 0)
				Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.GeometricMean(ld, ld2)));
			else if (ld < 0)
				Assert.IsLessThanOrEqualTo(Max(Sqrt(-r) * Sqrt(-r2) / (1L << 51), double.Epsilon),
					Abs(Sqrt(-r) * Sqrt(-r2) + (double)LongDecimal.GeometricMean(ld, ld2)));
			else
				Assert.IsLessThanOrEqualTo(Max(Sqrt(r) * Sqrt(r2) / (1L << 51), double.Epsilon),
					Abs(Sqrt(r) * Sqrt(r2) - (double)LongDecimal.GeometricMean(ld, ld2)));
		}
	}

	[TestMethod]
	public void TestInverseTrigonometry()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 5000; i++)
		{
			var r = random.NextDouble() * (random.Next(2) == 0 ? -1 : 1);
			var ld = new LongDecimal(r, MantissaLength);
			var ld2 = ld.Asin().Sin();
			var ld3 = ld.Acos().Cos();
			var ld4 = ld.Atan().Tan();
			Assert.IsLessThanOrEqualTo(LongDecimal.One >> MantissaLength, (ld - ld2).Abs());
			Assert.IsLessThanOrEqualTo(LongDecimal.One >> MantissaLength, (ld - ld3).Abs());
			Assert.IsLessThanOrEqualTo(LongDecimal.One >> MantissaLength - 16, (ld - ld4).Abs());
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
			var d = ConstructDecimal(bytes);
			LongDecimal ld = new(d, MantissaLength);
			if (LongDecimal.IsNaN(ld))
			{
				Assert.IsTrue(LongDecimal.IsNaN(ld.Log()));
				continue;
			}
			var log = ld.Log();
			var dLog = Log((double)d);
			if (LongDecimal.IsNaN(log))
				Assert.IsTrue(double.IsNaN(dLog));
			else if (LongDecimal.IsNegativeInfinity(log))
				Assert.IsTrue(double.IsNegativeInfinity(dLog));
			else if (LongDecimal.IsPositiveInfinity(log))
				Assert.IsTrue(double.IsPositiveInfinity(dLog));
			else
				Assert.IsLessThanOrEqualTo(Max(Abs(dLog), 1) / Pow(2, 51), Abs(dLog - (double)(decimal)log));
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
			var log10A = a.Log10();
			var log10B = b.Log10();
			var log10Prod = (a * b).Log10();
			var log10Quot = (a / b).Log10();
			var log10AAbs = log10A.Abs();
			var log10BAbs = log10B.Abs();
			Assert.IsLessThanOrEqualTo(log10AAbs + log10BAbs >> MantissaLength - 3, (log10A + log10B - log10Prod).Abs());
			Assert.IsLessThanOrEqualTo(log10AAbs + log10BAbs >> MantissaLength - 3, (log10A - log10B - log10Quot).Abs());
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
		Assert.AreEqual(longRealThree, longRealThree.Power(LongDecimal.One));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var uz = new MpuT(bytes.AsSpan(), RandomOrder());
			var ld = new LongDecimal(uz, MantissaLength);
			var ld2 = longRealThree.Power(ld).Log(longRealThree);
			Assert.IsLessThanOrEqualTo(ld >> MantissaLength - 8, (ld - ld2).Abs());
		}
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var @base = new MpuT(bytes.AsSpan(), RandomOrder());
			var shift = random.Next();
			var ld = new LongDecimal(@base, MantissaLength).Shift(shift);
			var ld2 = longRealThree.Power(ld.Log(longRealThree));
			Assert.IsLessThanOrEqualTo(ld >> MantissaLength - 50, (ld - ld2).Abs());
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
			var ld = new LongDecimal(@base, MantissaLength).Shift(shift);
			var ld2 = 1 / ld.Reciproc();
			Assert.IsLessThanOrEqualTo(ld.Abs() >> MantissaLength - 1, (ld - ld2).Abs());
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
		for (var i = 0; i < 500000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			LongDecimal ld = new(r, MantissaLength);
			var shiftAmount = random.Next(257);
			if (double.IsNaN(r))
			{
				Assert.IsTrue(double.IsNaN((double)(ld << shiftAmount)));
				Assert.IsTrue(double.IsNaN((double)(ld << (UnsignedLongDecimal)shiftAmount)));
				Assert.IsTrue(double.IsNaN((double)(ld >> shiftAmount)));
				Assert.IsTrue(double.IsNaN((double)(ld >> (UnsignedLongDecimal)shiftAmount)));
			}
			else
			{
				Assert.AreEqual(r * Pow(10, shiftAmount), (double)(ld << shiftAmount),
					Max(Abs(r) * Pow(10, shiftAmount) / Pow(2, 52), double.Epsilon));
				Assert.AreEqual(r * Pow(10, shiftAmount), (double)(ld << (UnsignedLongDecimal)shiftAmount),
					Max(Abs(r) * Pow(10, shiftAmount) / Pow(2, 52), double.Epsilon));
				Assert.AreEqual(r / Pow(10, shiftAmount), (double)(ld >> shiftAmount),
					Max(Abs(r) * Pow(10, shiftAmount) / Pow(2, 52), double.Epsilon));
				Assert.AreEqual(r / Pow(10, shiftAmount), (double)(ld >> (UnsignedLongDecimal)shiftAmount),
					Max(Abs(r) * Pow(10, shiftAmount) / Pow(2, 52), double.Epsilon));
			}
		}
		for (var i = 0; i < 500000; i++)
		{
			bytes.FillInPlace(random.Next(259), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			LongDecimal ld = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			bytes.FillInPlace(random.Next(3), _ => (byte)random.Next(256));
			bytes.PadRightInPlace(4);
			shiftAmount = BitConverter.ToInt32(bytes.AsSpan());
			Assert.IsLessThanOrEqualTo(uz.ShiftLeftDec(shiftAmount).ShiftRightRoundDec(MantissaLength),
				new LongDecimal(uz.ShiftLeftDec(shiftAmount), MantissaLength) - (ld << shiftAmount));
			Assert.IsLessThanOrEqualTo(MpuT.Max(uz >> MantissaLength, MpuT.One),
				new LongDecimal(uz.ShiftRightRoundDec(shiftAmount), MantissaLength) - (ld >> shiftAmount));
			Assert.IsLessThanOrEqualTo(uz.ShiftLeftDec(shiftAmount).ShiftRightRoundDec(MantissaLength),
				new LongDecimal(uz.ShiftLeftDec(shiftAmount), MantissaLength) - (ld << (UnsignedLongDecimal)shiftAmount));
			Assert.IsLessThanOrEqualTo(MpuT.Max(uz >> MantissaLength, MpuT.One),
				new LongDecimal(uz.ShiftRightRoundDec(shiftAmount), MantissaLength) - (ld >> (UnsignedLongDecimal)shiftAmount));
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
			LongDecimal ld = new(r, MantissaLength);
			if (LongDecimal.IsNaN(ld))
				Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.Sqrt(ld)));
			else
				Assert.AreEqual(Sqrt(r), (double)LongDecimal.Sqrt(ld));
		}
		for (var i = 0; i < 100000; i++)
		{
			bytes.FillInPlace(random.Next(251), _ => (byte)random.Next(256));
			var @base = new MpuT(bytes.AsSpan(), RandomOrder());
			bytes.FillInPlace(random.Next(65), _ => (byte)random.Next(256));
			var shift = new MpzT(bytes.AsSpan(), RandomOrder());
			var ld = new LongDecimal(@base, MantissaLength).Shift(shift);
			var sqrt = LongDecimal.Sqrt(ld);
			Assert.IsLessThanOrEqualTo(sqrt * sqrt >> MantissaLength - 2, (sqrt * sqrt - ld).Abs());
			Assert.IsLessThanOrEqualTo(ld >> MantissaLength - 2, (sqrt * sqrt - ld).Abs());
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
			var mantissaLength = random.Next(30, Max((int)Ceiling(bytes.Length * Log10(256)), 30));
			var switcher = random.Next(1000);
			LongDecimal ld = switcher switch
			{
				0 => new(0d, mantissaLength),
				1 => new(double.PositiveInfinity, mantissaLength),
				2 => new(double.NegativeInfinity, mantissaLength),
				3 => new(double.NaN, mantissaLength),
				4 => new(double.NegativeZero, mantissaLength),
				_ => new(bytes.AsSpan(), order, mantissaLength),
			};
			LongDecimal ld2 = new(ld.ToByteArray(order, false), order, mantissaLength);
			Assert.IsTrue(LongDecimal.IsNaN(ld) && LongDecimal.IsNaN(ld2) || ld.Equals(ld2));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToDecimal()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000000; i++)
		{
			bytes.FillInPlace(random.Next(17), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(16);
			else
				bytes.ResizeLeft(16);
			var i1 = BitConverter.ToInt32(bytes.AsSpan());
			var i2 = BitConverter.ToInt32(bytes.AsSpan(4));
			var i3 = BitConverter.ToInt32(bytes.AsSpan(8));
			var i4 = BitConverter.ToInt32(bytes.AsSpan(12));
			i4 = (i4 & int.MaxValue) % 29 << 16 | i4 & int.MinValue;
			decimal d = new([i1, i2, i3, i4]);
			LongDecimal ld = new(d, MantissaLength);
			Assert.AreEqual(d, (decimal)ld);
		}
		for (var i = 0; i < 100; i++)
		{
			var ld = new LongDecimal(random.Next()).Shift(-random.Next());
			Assert.AreEqual(UnsignedLongDecimal.Zero, (UnsignedLongDecimal)ld);
		}
	}

	[TestMethod]
	public void TestToDouble()
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
			LongDecimal ld = new(r, MantissaLength);
			Assert.AreEqual(r, (double)ld);
		}
		for (var i = 0; i < 100; i++)
		{
			var ld = new LongDecimal(random.Next()).Shift(-random.Next());
			Assert.AreEqual(UnsignedLongDecimal.Zero, (UnsignedLongDecimal)ld);
		}
	}

	[TestMethod]
	public void TestToString()
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		LongDecimal longDecimal = new LongReal(1).Shift(0);
		var result = longDecimal.ToString("E6");
		Assert.AreEqual("1E+0", result);
		longDecimal = new LongReal(1).Shift(1);
		result = longDecimal.ToString("E6");
		Assert.AreEqual("2E+0", result);
		longDecimal = new LongReal(1).Shift(2);
		result = longDecimal.ToString("E6");
		Assert.AreEqual("4E+0", result);
		longDecimal = new LongReal(3).Shift(3);
		result = longDecimal.ToString("E6");
		Assert.AreEqual("2.4E+1", result);
		longDecimal = new LongReal(5).Shift(-2);
		result = longDecimal.ToString("E6");
		Assert.AreEqual("1.25E+0", result);
		longDecimal = new LongReal(123).Shift(50);
		result = longDecimal.ToString("E4");
		Assert.AreEqual("1.3849E+17", result);
		longDecimal = new LongReal(1000).Shift(-10);
		result = longDecimal.ToString("F6", CultureInfo.GetCultureInfo("en-US"));
		Assert.AreEqual("0.976563", result);
		var largeDigits = "123456789";
		var mpz = MpzT.Parse(largeDigits);
		longDecimal = new LongReal(mpz).Shift(20);
		result = longDecimal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		Assert.Contains("129 453 825 982 464", result);
		longDecimal = new LongReal(1).Shift(100);
		result = longDecimal.ToString("E2");
		Assert.AreEqual("1.27E+30", result);
		foreach (var (number, format, en, ru, de) in CultureTestData())
		{
			longDecimal = number;
			var enResult = longDecimal.ToString(format, CultureInfo.GetCultureInfo("en-US"));
			Assert.AreEqual(en, enResult);
			var ruResult = longDecimal.ToString(format, CultureInfo.GetCultureInfo("ru-RU"));
			Assert.AreEqual(ru, ruResult);
			var deResult = longDecimal.ToString(format, CultureInfo.GetCultureInfo("de-DE"));
			Assert.AreEqual(de, deResult);
		}
		mpz = new MpzT(77).Power(77);
		longDecimal = new LongReal((MpzT)1).Shift(mpz);
		result = longDecimal.ToString("E6");
		Assert.AreEqual("1.358443E+5475144815987627762430594775150486533643549212522238631644821558595137232066160304681082998798877694978398467245688991276872900744519537448240061", result);
	}

	[TestMethod]
	public void TestTrigonometry()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		Assert.AreEqual(0, LongDecimal.Zero.Sin());
		Assert.AreEqual(1, (LongDecimal.Pi / 2).Sin());
		Assert.AreEqual(0, LongDecimal.Pi.Sin());
		Assert.AreEqual(-1, (3 * LongDecimal.Pi / 2).Sin());
		Assert.AreEqual(0, (LongDecimal.Pi * 2).Sin());
		Assert.AreEqual(-1, (-LongDecimal.Pi / 2).Sin());
		Assert.AreEqual(0, (-LongDecimal.Pi).Sin());
		Assert.AreEqual(1, (-3 * LongDecimal.Pi / 2).Sin());
		Assert.AreEqual(0, (-LongDecimal.Pi * 2).Sin());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.PositiveInfinity.Sin()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NegativeInfinity.Sin()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Sin()));
		Assert.AreEqual(1, LongDecimal.Zero.Cos());
		Assert.AreEqual(0, (LongDecimal.Pi / 2).Cos());
		Assert.AreEqual(-1, LongDecimal.Pi.Cos());
		Assert.AreEqual(0, (3 * LongDecimal.Pi / 2).Cos());
		Assert.AreEqual(1, (LongDecimal.Pi * 2).Cos());
		Assert.AreEqual(0, (-LongDecimal.Pi / 2).Cos());
		Assert.AreEqual(-1, (-LongDecimal.Pi).Cos());
		Assert.AreEqual(0, (-3 * LongDecimal.Pi / 2).Cos());
		Assert.AreEqual(1, (-LongDecimal.Pi * 2).Cos());
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.PositiveInfinity.Cos()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NegativeInfinity.Cos()));
		Assert.IsTrue(LongDecimal.IsNaN(LongDecimal.NaN.Cos()));
		for (var i = 0; i < 10000; i++)
		{
			var r = Pow(2, random.NextDouble() * 128 - 64);
			LongDecimal ld = new(r, MantissaLength);
			if (LongDecimal.IsNaN(ld))
			{
				Assert.IsTrue(LongDecimal.IsNaN(ld.Sin()));
				Assert.IsTrue(LongDecimal.IsNaN(ld.Cos()));
				Assert.IsTrue(LongDecimal.IsNaN(ld.Tan()));
			}
			else
			{
				Assert.IsLessThanOrEqualTo(Pow(2, -52), Abs(Sin(r) - (double)ld.Sin()));
				Assert.IsLessThanOrEqualTo(Pow(2, -52), Abs(Cos(r) - (double)ld.Cos()));
				Assert.IsLessThanOrEqualTo(Max(Abs(Tan(r)), 1) / Pow(2, 52), Abs(Tan(r) - (double)ld.Tan()));
			}
		}
		for (var i = 0; i < 10000; i++)
		{
			var r = (1 - random.NextDouble()) * PI / 2;
			LongDecimal ld = new(r, MantissaLength);
			Assert.IsLessThan(ld, ld.Sin());
			Assert.IsGreaterThan(ld, ld.Tan());
		}
	}

	[TestMethod]
	public void TestToUnsignedLongDecimal()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(1, 501), _ => (byte)random.Next(256));
			var order = RandomOrder();
			UnsignedLongDecimal uld = new(bytes.AsSpan(), order, MantissaLength);
			if (bytes.Length - MantissaByteLength == 4)
				continue;
			LongDecimal ld = uld;
			Assert.AreEqual(uld, (UnsignedLongDecimal)ld);
		}
		for (var i = 0; i < 100; i++)
		{
			var ld = new LongDecimal(random.Next()).Shift(-random.Next());
			Assert.AreEqual(UnsignedLongDecimal.Zero, (UnsignedLongDecimal)ld);
		}
		Assert.ThrowsExactly<OverflowException>(() => (UnsignedLongDecimal)LongDecimal.PositiveInfinity);
		Assert.ThrowsExactly<OverflowException>(() => (UnsignedLongDecimal)LongDecimal.NegativeInfinity);
		Assert.ThrowsExactly<OverflowException>(() => (UnsignedLongDecimal)LongDecimal.NaN);
		int RandomOrder() => random.Next(2) * 2 - 1;
	}
}
