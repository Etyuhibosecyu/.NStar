namespace NStar.Mpir.Tests;

[TestClass]
public class ComplexTests
{
	[TestMethod]
	public void ComplexTest()
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
		bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
		if (random.Next(2) == 0)
			bytes.Resize(8);
		else
			bytes.ResizeLeft(8);
		var r2 = BitConverter.ToDouble(bytes.AsSpan());
		System.Numerics.Complex nc = new(r, r2);
		Complex c = new(r, r2);
		Validate();
		var actions = new[]
		{
			() =>
			{
				var op = (byte)random.Next(256);
				nc += op;
				c += op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				nc -= op;
				c -= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				nc *= op;
				c *= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				nc /= op;
				c /= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				nc += op;
				c += op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				nc -= op;
				c -= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				nc *= op;
				c *= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				nc /= op;
				c /= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				nc += op;
				c += op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				nc -= op;
				c -= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				nc *= op;
				c *= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				nc /= op;
				c /= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				nc += op;
				c += op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				nc -= op;
				c -= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				nc *= op;
				c *= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				nc /= op;
				c /= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				nc += op;
				c += op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				nc -= op;
				c -= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				nc *= op;
				c *= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				nc /= op;
				c /= op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				nc += op;
				c += op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				nc -= op;
				c -= op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				nc *= op;
				c *= op;
				Validate();
			}, () =>
			{
				var op = BitConverter.UInt64BitsToDouble((ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63));
				if (op.Equals(0))
					return;
				nc /= op;
				c /= op;
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			c = new(r, r2);
			nc = new(r, r2);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		void Validate() => Assert.IsTrue(nc.Equals((System.Numerics.Complex)c)
			|| nc.Real is double.NaN && c.Real is double.NaN || nc.Imaginary is double.NaN && c.Imaginary is double.NaN);
	}

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			ProcessA(c, nc);
		}
		void ProcessA(Complex c, System.Numerics.Complex nc)
		{
			dynamic num = c;
			ProcessB(c, nc, num);
			num = c + 1;
			ProcessB(c, nc, num);
			num = c - 1;
			ProcessB(c, nc, num);
			num = c * 2;
			ProcessB(c, nc, num);
			num = c / 2;
			ProcessB(c, nc, num);
			num = c * 3;
			ProcessB(c, nc, num);
			num = c / 3;
			ProcessB(c, nc, num);
			num = (byte)0;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = (short)0;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = (ushort)0;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = 0;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = 0u;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = 0L;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = 0uL;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = MpuT.Zero;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), num.Equals(c));
			num = MpzT.Zero;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), num.Equals(c));
			num = 0f;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = 0d;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num), c.Equals((object)num));
			num = Complex.Zero;
			Assert.AreEqual(c.Equals(nc) && num.Equals(nc), c.Equals(num));
			Assert.AreEqual(c.Equals(nc) && num.Equals(nc), c.Equals((object)num));
			Assert.AreEqual(c.Equals(nc) && num.Equals(nc), num.Equals(c));
		}
		void ProcessB(Complex c, System.Numerics.Complex nc, dynamic num)
		{
			dynamic num2 = (byte)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (short)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (ushort)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (int)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (uint)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (long)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (ulong)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (MpzT)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), num2.Equals(c));
			num2 = (float)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (double)num.Real;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (System.Numerics.Complex)num;
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && nc.Equals(num2), c.Equals((object)num2));
			num2 = (Complex)num;
			Assert.AreEqual(c.Equals(nc) && num2.Equals(nc), c.Equals(num2));
			Assert.AreEqual(c.Equals(nc) && num2.Equals(nc), c.Equals((object)num2));
		}
	}

	[TestMethod]
	public void TestInverseTrigonometry()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		for (var i = 0; i < 5000; i++)
		{
			var r = random.NextDouble() * (random.Next(2) == 0 ? -1 : 1);
			var r2 = random.NextDouble() * (random.Next(2) == 0 ? -1 : 1);
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			var nc2 = System.Numerics.Complex.Asin(nc);
			var c2 = c.Asin();
			var nc3 = System.Numerics.Complex.Acos(nc);
			var c3 = c.Acos();
			var nc4 = System.Numerics.Complex.Atan(nc);
			var c4 = c.Atan();
			Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(nc3), 1d) / Pow(2, 50),
				System.Numerics.Complex.Abs(nc2 - (System.Numerics.Complex)c2));
			Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(nc3), 1d) / Pow(2, 50),
				System.Numerics.Complex.Abs(nc3 - (System.Numerics.Complex)c3));
			Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(nc3), 1d) / Pow(2, 50),
				System.Numerics.Complex.Abs(nc4 - (System.Numerics.Complex)c4));
		}
	}

	[TestMethod]
	public void TestLog()
	{
		Assert.AreEqual(Complex.NegativeInfinity, Complex.Zero.Log());
		Assert.AreEqual(Complex.PositiveInfinity, Complex.PositiveInfinity.Log());
		Assert.AreEqual(new(double.PositiveInfinity, double.Pi), Complex.NegativeInfinity.Log());
		Assert.IsTrue(Complex.IsNaN(Complex.NaN.Log()));
		Assert.AreEqual(Complex.Zero, Complex.One.Log());
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
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			if (Complex.IsNaN(c))
				Assert.IsTrue(Complex.IsNaN(c.Log()));
			else
			{
				var log = System.Numerics.Complex.Log(nc);
				Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(log), 1d) / Pow(2, 50),
					System.Numerics.Complex.Abs(log - (System.Numerics.Complex)c.Log()));
			}
		}
	}

	[TestMethod]
	public void TestPower()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var complexThree = new Complex(3, 0);
		Assert.AreEqual(Complex.One, complexThree.Power(Complex.Zero));
		Assert.AreEqual(Complex.PositiveInfinity, complexThree.Power(Complex.PositiveInfinity));
		Assert.AreEqual(Complex.Zero, complexThree.Power(Complex.NegativeInfinity));
		Assert.IsTrue(Complex.IsNaN(complexThree.Power(Complex.NaN)));
		Assert.AreEqual(complexThree, complexThree.Power(Complex.One));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r = BitConverter.ToDouble(bytes.AsSpan());
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			if (Complex.IsNaN(c))
				Assert.IsTrue(Complex.IsNaN(complexThree.Power(c)));
			else
			{
				var log = System.Numerics.Complex.Pow(new(3, 0), nc);
				Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(log), 1d) / Pow(2, 50),
					System.Numerics.Complex.Abs(log - (System.Numerics.Complex)complexThree.Power(c)));
			}
		}
	}

	[TestMethod]
	public void TestReciproc()
	{
		Assert.AreEqual(Complex.PositiveInfinity, Complex.Zero.Reciproc());
		Assert.AreEqual(Complex.Zero, Complex.PositiveInfinity.Reciproc());
		Assert.AreEqual(Complex.Zero, Complex.NegativeInfinity.Reciproc());
		Assert.IsTrue(Complex.IsNaN(Complex.NaN.Reciproc()));
		Assert.AreEqual(Complex.One, Complex.One.Reciproc());
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
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			if (c == 0)
				Assert.IsTrue(c.Reciproc() == Complex.PositiveInfinity);
			else
				Assert.IsLessThanOrEqualTo(System.Numerics.Complex.Abs(System.Numerics.Complex.Reciprocal(nc)) / Pow(2, 52),
					(System.Numerics.Complex.Reciprocal(nc) - c.Reciproc()).Abs());
		}
	}

	[TestMethod]
	public void TestShifts()
	{
		Assert.AreEqual(Complex.Zero, Complex.Zero << 3);
		Assert.AreEqual(Complex.PositiveInfinity, Complex.PositiveInfinity << 3);
		Assert.AreEqual(Complex.NegativeInfinity, Complex.NegativeInfinity << 3);
		Assert.IsTrue(Complex.IsNaN(Complex.NaN << 3));
		Assert.AreEqual(Complex.Zero, Complex.Zero >> 3);
		Assert.AreEqual(Complex.PositiveInfinity, Complex.PositiveInfinity >> 3);
		Assert.AreEqual(Complex.NegativeInfinity, Complex.NegativeInfinity >> 3);
		Assert.IsTrue(Complex.IsNaN(Complex.NaN >> 3));
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
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			var shiftAmount = random.Next(257);
			Assert.AreEqual(nc * Pow(2, shiftAmount), (System.Numerics.Complex)(c << shiftAmount));
			Assert.AreEqual(nc / Pow(2, shiftAmount), (System.Numerics.Complex)(c >> shiftAmount));
		}
	}

	[TestMethod]
	public void TestSqrt()
	{
		Assert.AreEqual(Complex.Zero, Complex.Zero.Sqrt());
		Assert.AreEqual(Complex.PositiveInfinity, Complex.PositiveInfinity.Sqrt());
		Assert.IsTrue(Complex.IsNaN(Complex.NegativeInfinity.Sqrt()));
		Assert.IsTrue(Complex.IsNaN(Complex.NaN.Sqrt()));
		Assert.AreEqual(Complex.One, Complex.One.Sqrt());
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
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			if (Complex.IsNaN(c))
				Assert.IsTrue(Complex.IsNaN(Complex.Sqrt(c)));
			else
			{
				var sqrt = System.Numerics.Complex.Sqrt(nc);
				Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(sqrt), 1d) / Pow(2, 50),
					System.Numerics.Complex.Abs(sqrt - (System.Numerics.Complex)c.Sqrt()));
			}
		}
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
				bytes.Resize(sizeof(double) << 1);
			else
				bytes.ResizeLeft(sizeof(double) << 1);
			Complex c = random.Next(1000) switch
			{
				0 => new(0d, 0d),
				1 => new(double.PositiveInfinity, 0d),
				2 => new(double.NegativeInfinity, 0d),
				3 => new(double.NaN, 0d),
				4 => new(double.NegativeZero, 0d),
				_ => new(bytes.AsSpan(), order),
			};
			Complex c2 = new(c.ToByteArray(order), order);
			Assert.IsTrue(Complex.IsNaN(c) && Complex.IsNaN(c2) || c.Equals(c2));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToDefaultComplex()
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
			bytes.FillInPlace(random.Next(9), _ => (byte)random.Next(256));
			if (random.Next(2) == 0)
				bytes.Resize(8);
			else
				bytes.ResizeLeft(8);
			var r2 = BitConverter.ToDouble(bytes.AsSpan());
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			Assert.AreEqual(nc, (System.Numerics.Complex)c);
		}
	}

	[TestMethod]
	public void TestToString()
	{
		//CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		//var longReal = new Complex(1).Shift(0);
		//var result = longReal.ToString("E6");
		//Assert.AreEqual("1E+0", result);
		//longReal = new Complex(1).Shift(1);
		//result = longReal.ToString("E6");
		//Assert.AreEqual("2E+0", result);
		//longReal = new Complex(1).Shift(2);
		//result = longReal.ToString("E6");
		//Assert.AreEqual("4E+0", result);
		//longReal = new Complex(3).Shift(3);
		//result = longReal.ToString("E6");
		//Assert.AreEqual("2.4E+1", result);
		//longReal = new Complex(5).Shift(-2);
		//result = longReal.ToString("E6");
		//Assert.AreEqual("1.25E+0", result);
		//longReal = new Complex(123).Shift(50);
		//result = longReal.ToString("E4");
		//Assert.AreEqual("1.3849E+17", result);
		//longReal = new Complex(1000).Shift(-10);
		//result = longReal.ToString("F6", CultureInfo.GetCultureInfo("en-US"));
		//Assert.AreEqual("0.976563", result);
		//var largeDigits = "123456789";
		//var mpz = MpzT.Parse(largeDigits);
		//longReal = new Complex(mpz).Shift(20);
		//result = longReal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		//Assert.Contains("129 453 825 982 464", result);
		//longReal = new Complex(1).Shift(100);
		//result = longReal.ToString("E2");
		//Assert.AreEqual("1.27E+30", result);
		//foreach (var (number, format, en, ru, de) in CultureTestData())
		//{
		//	longReal = number;
		//	var enResult = longReal.ToString(format, CultureInfo.GetCultureInfo("en-US"));
		//	Assert.AreEqual(en, enResult);
		//	var ruResult = longReal.ToString(format, CultureInfo.GetCultureInfo("ru-RU"));
		//	Assert.AreEqual(ru, ruResult);
		//	var deResult = longReal.ToString(format, CultureInfo.GetCultureInfo("de-DE"));
		//	Assert.AreEqual(de, deResult);
		//}
		//mpz = new MpzT(77).Power(77);
		//longReal = new Complex((MpzT)1).Shift(mpz);
		//result = longReal.ToString("E6");
		//Assert.AreEqual("1.358443E+5475144815987627762430594775150486533643549212522238631644821558595137232066160304681082998798877694978398467245688991276872900744519537448240061", result);
	}

	[TestMethod]
	public void TestTrigonometry()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - Complex.Zero.Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (1 - (Complex.Pi >> 1).Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - Complex.Pi.Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (-1 - (3 * Complex.Pi >> 1).Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (Complex.Pi << 1).Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (-1 - (-Complex.Pi >> 1).Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (-Complex.Pi).Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (1 - (-3 * Complex.Pi >> 1).Sin()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (-Complex.Pi << 1).Sin()).Abs());
		Assert.IsTrue(Complex.IsNaN(Complex.PositiveInfinity.Sin()));
		Assert.IsTrue(Complex.IsNaN(Complex.NegativeInfinity.Sin()));
		Assert.IsTrue(Complex.IsNaN(Complex.NaN.Sin()));
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (1 - Complex.Zero.Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (Complex.Pi >> 1).Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (-1 - Complex.Pi.Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (3 * Complex.Pi >> 1).Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (1 - (Complex.Pi << 1).Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (-Complex.Pi >> 1).Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (-1 - (-Complex.Pi).Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (0 - (-3 * Complex.Pi >> 1).Cos()).Abs());
		Assert.IsLessThanOrEqualTo(Pow(2, -50), (1 - (-Complex.Pi << 1).Cos()).Abs());
		Assert.IsTrue(Complex.IsNaN(Complex.PositiveInfinity.Cos()));
		Assert.IsTrue(Complex.IsNaN(Complex.NegativeInfinity.Cos()));
		Assert.IsTrue(Complex.IsNaN(Complex.NaN.Cos()));
		for (var i = 0; i < 10000; i++)
		{
			var r = Pow(2, random.NextDouble() * 128 - 64);
			var r2 = Pow(2, random.NextDouble() * 128 - 64);
			System.Numerics.Complex nc = new(r, r2);
			Complex c = new(r, r2);
			if (Complex.IsNaN(c))
			{
				Assert.IsTrue(Complex.IsNaN(c.Sin()));
				Assert.IsTrue(Complex.IsNaN(c.Cos()));
				Assert.IsTrue(Complex.IsNaN(c.Tan()));
			}
			else
			{
				var sin = System.Numerics.Complex.Sin(nc);
				Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(sin), 1d) / Pow(2, 50),
					System.Numerics.Complex.Abs(sin - (System.Numerics.Complex)c.Sin()));
				var cos = System.Numerics.Complex.Cos(nc);
				Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(cos), 1d) / Pow(2, 50),
					System.Numerics.Complex.Abs(cos - (System.Numerics.Complex)c.Cos()));
				Assert.IsLessThanOrEqualTo(Max(System.Numerics.Complex.Abs(sin / cos), 1d) / Pow(2, 50),
					System.Numerics.Complex.Abs(sin / cos - (System.Numerics.Complex)c.Tan()));
			}
		}
	}

	//private static G.IEnumerable<(Complex number, string format, string en, string ru, string de)> CultureTestData()
	//{
	//	yield return (new Complex(15L).Shift(12), "F2", "61,440.00", "61 440,00", "61.440,00");
	//	yield return (new Complex(-987L).Shift(-8), "E3", "-3.855E+0", "-3,855E+0", "-3,855E+0");
	//	yield return (new(123456.789), "N5", "123,456.78900", "123 456,78900", "123.456,78900");
	//}
}
