namespace NStar.Core.Tests;

[TestClass]
public class UnsignedLongRealTests
{
	private static readonly int MantissaLength = typeof(UnsignedLongReal)
		.GetField(nameof(MantissaLength), BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null)
		is int n ? n : throw new MissingFieldException(), MantissaByteLength = MantissaLength / 8;
	private static readonly MpuT MantissaOverflow = new MpuT(1) << MantissaLength;
	private static readonly MpuT MantissaMask = MantissaOverflow - 1;

	[TestMethod]
	public void ComplexTestMixed()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
	l1:
		bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		UnsignedLongReal ulr = uz;
		Validate();
		var actions = new[]
		{
			() =>
			{
				var op = (byte)random.Next(256);
				uz += op;
				ulr += op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op > uz)
					return;
				if (uz.BitLength <= MantissaLength + ((MpuT)op).BitLength)
					uz -= op;
				ulr -= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				uz *= op;
				ulr *= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				uz /= op;
				ulr /= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(op, out _);
				var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				var op = random.Next();
				uz += op;
				ulr += op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op > uz)
					return;
				if (uz.BitLength <= MantissaLength + ((MpuT)op).BitLength)
					uz -= op;
				ulr -= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				uz *= op;
				ulr *= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				uz /= op;
				ulr /= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(op, out _);
				var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				var op = random.Next();
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				if (uz.BitLength > ((MpuT)op).BitLength + MantissaLength)
					uz = 0;
				else
					uz &= op;
				ulr &= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				uz += op;
				ulr += op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op > uz)
					return;
				if (uz.BitLength <= MantissaLength + ((MpuT)op).BitLength)
					uz -= op;
				ulr -= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				uz *= op;
				ulr *= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				uz /= op;
				ulr /= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(op, out _);
				var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				if (uz.BitLength <= MantissaLength + ((MpuT)op).BitLength)
					uz &= op;
				else
					uz = 0;
				ulr &= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				uz += op;
				ulr += op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op > uz)
					return;
				if (uz.BitLength <= MantissaLength + ((MpuT)op).BitLength)
					uz -= op;
				ulr -= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				uz *= op;
				ulr *= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				uz /= op;
				ulr /= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(op, out _);
				var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				uz += op;
				ulr += op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op > uz)
					return;
				if (uz.BitLength <= MantissaLength + ((MpuT)op).BitLength)
					uz -= op;
				ulr -= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				uz *= op;
				ulr *= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				uz /= op;
				ulr /= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(op, out _);
				var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			ulr = uz;
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
			using var expected = (MpzT)uz.ShiftRightRound(bitLengthDiff) & MantissaMask;
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo((MpuT)1 << bitLengthDiff, (expected - actual).Abs());
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, MantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void ComplexTestSame()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
	l1:
		bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		UnsignedLongReal ulr = uz;
		Validate();
		var actions = new[]
		{
			() =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				uz += op;
				ulr += op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (random.Next(1000) == 0)
					op = uz;
				if (op > uz)
					return;
				var old = uz;
				if (uz.BitLength <= op.BitLength + MantissaLength)
					uz -= op;
				if (old.BitLength >= MantissaLength && (old.BitLength == op.BitLength
					|| old.BitLength == op.BitLength + 1 && (int)(old * 8 / op) is 8 or 9 or 10))
				{
					var shiftAmount = op.BitLength - MantissaLength - 1;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr -= op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				uz *= op;
				ulr *= op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				uz /= op;
				ulr /= op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= op;
				shiftAmount = Max(oldBitLength - MantissaLength, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				ulr %= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(op, out _);
				var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				UnsignedLongReal op = new MpuT(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz %= (MpuT)op;
				shiftAmount = Max(oldBitLength - MantissaLength, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				ulr %= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(Max(oldBitLength, op.BitLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				op = op.ShiftRightRound(shiftAmount) << shiftAmount;
				if (uz.BitLength < op.BitLength && op.BitLength > uz.BitLength + MantissaLength
					|| uz.BitLength > op.BitLength + MantissaLength)
					uz = 0;
				else
					uz &= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr &= op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(Max(oldBitLength, op.BitLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				op = op.ShiftRightRound(shiftAmount) << shiftAmount;
				if (uz.BitLength < op.BitLength && op.BitLength > uz.BitLength + MantissaLength)
					uz = op;
				else if (uz.BitLength <= op.BitLength + MantissaLength)
					uz |= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr |= op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.BitLength;
				var shiftAmount = Max(Max(oldBitLength, op.BitLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				op = op.ShiftRightRound(shiftAmount) << shiftAmount;
				if (uz.BitLength < op.BitLength && op.BitLength > uz.BitLength + MantissaLength)
					uz = op;
				else if (uz.BitLength <= op.BitLength + MantissaLength)
					uz ^= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr ^= op;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz = (uint)(int)uz;
				ulr = (uint)(int)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz = (uint)uz;
				ulr = (uint)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz = (ulong)(long)uz;
				ulr = (ulong)(long)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz = (ulong)uz;
				ulr = (ulong)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz = (MpuT)(double)uz;
				ulr = (UnsignedLongReal)(double)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				uz = (MpuT)(decimal)uz;
				ulr = (UnsignedLongReal)(decimal)ulr;
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			ulr = uz;
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
			using var expected = (MpzT)uz.ShiftRightRound(bitLengthDiff) & MantissaMask;
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo((MpuT)1 << bitLengthDiff, (expected - actual).Abs());
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, MantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void ConversionTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		var counter = 0;
	l1:
		for (var i = 0; i < 1000; i++)
		{
			var bytes = RedStarLinq.FillArray(random.Next(1000), _ => (byte)random.Next(256));
			MpuT uz = new(bytes, RandomOrder());
			using var ulr = new UnsignedLongReal(uz);
			var bitLengthDiff = Max(uz.BitLength - MantissaLength - 1, 0);
			if (bitLengthDiff > 0)
				uz = uz.ShiftRightRound(bitLengthDiff) << bitLengthDiff;
			using var expected = (MpzT)uz.ShiftRightRound(bitLengthDiff) & MantissaMask;
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.AreEqual(expected, actual);
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, MantissaByteLength)..bytesWritten), -1));
		}
		if (counter++ < 2500)
			goto l1;
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
	}
}
