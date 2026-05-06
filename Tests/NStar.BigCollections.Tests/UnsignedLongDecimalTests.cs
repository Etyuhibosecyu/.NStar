using System.Globalization;
using System.Reflection;

namespace NStar.BigCollections.Tests;

[TestClass]
public class UnsignedLongDecimalTests
{
	private static readonly int MantissaLength = 300, MantissaByteLength = (int)Ceiling(MantissaLength * Log(10, 256));
	private static readonly MpuT MantissaOverflow = MpuT.PowerOfTen(MantissaLength);

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
		UnsignedLongDecimal ulr = new(uz, MantissaLength);
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
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				if (shiftAmount > 0)
					ulr = ulr >> shiftAmount << shiftAmount;
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
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				var op = random.Next();
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength > ((MpuT)op).DecLength + MantissaLength)
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
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
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
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				if (shiftAmount > 0)
					ulr = ulr >> shiftAmount << shiftAmount;
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
				if (Mpir.Mpir.MpuCmp(op, uz) > 0)
					return;
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
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
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(bitLengthDiff), MantissaOverflow);
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(bitLengthDiff), (expected - actual).Abs());
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, MantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void ComplexTestMixedMantissaLength()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
	l1:
		bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		var mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
		var maxMantissaLength = mantissaLength;
		UnsignedLongDecimal ulr = new(uz, mantissaLength);
		Validate();
		var actions = new[]
		{
			() =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var shiftAmount = Max(uz.DecLength - mantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz += op;
				ulr += new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = uz.DecLength < minMantissaLength + 1 ? 0
					: Max(uz.DecLength - minMantissaLength - 1, 0);
				var shiftAmountLite = uz.DecLength < minMantissaLength + 1 ? 0
					: Max(uz.DecLength - mantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmountLite).ShiftLeftDec(shiftAmountLite);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				ulr = ulr >> shiftAmount << shiftAmount;
				if (random.Next(1000) == 0)
					op = uz;
				if (op > uz)
					return;
				if (uz.DecLength <= op.DecLength + maxMantissaLength)
					uz -= op;
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				ulr -= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				uz *= op;
				ulr *= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				uz /= op;
				ulr /= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				UnsignedLongDecimal op = new(new MpuT(bytes.AsSpan(), RandomOrder()), mantissaLength2);
				var oldBitLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(oldBitLength - minMantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op >> shiftAmount << shiftAmount;
				ulr = ulr >> shiftAmount << shiftAmount;
				if (op == 0)
					return;
				uz %= (MpuT)op;
				ulr %= op;
				shiftAmount = Max(oldBitLength - minMantissaLength, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				ulr = ulr >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				uz /= op;
				ulr = ulr.DivRem(new UnsignedLongDecimal(op, mantissaLength2), out _);
				var shiftAmount = Max(uz.DecLength - maxMantissaLength - 1, 0);
				if (shiftAmount > 0)
					ulr = ulr >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(Max(oldBitLength, op.DecLength) - minMantissaLength - 1, 0);
				var shiftAmountLite = Max(Max(oldBitLength, op.DecLength) - mantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmountLite).ShiftLeftDec(shiftAmountLite);
				ulr = ulr >> shiftAmountLite << shiftAmountLite;
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + minMantissaLength
					|| uz.DecLength > op.DecLength + minMantissaLength)
					uz = 0;
				else
					uz &= op;
				ulr &= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(Max(oldBitLength, op.DecLength) - minMantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + maxMantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + maxMantissaLength)
					uz |= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr |= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(Max(oldBitLength, op.DecLength) - minMantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + maxMantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + maxMantissaLength)
					uz ^= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr ^= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			ulr = new(uz, mantissaLength);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.DecLength - maxMantissaLength - 1, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(bitLengthDiff), maxMantissaLength);
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false);
			var maxMantissaByteLength = Min(bytesWritten, GetArrayLength(maxMantissaLength, 8));
			using var actual = new MpuT(writeBuffer.AsSpan(0, maxMantissaByteLength), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(maxMantissaLength), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > maxMantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, maxMantissaByteLength)..bytesWritten), -1));
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
		UnsignedLongDecimal ulr = new(uz, MantissaLength);
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
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (random.Next(1000) == 0)
					op = uz;
				if (op > uz)
					return;
				var old = uz;
				if (uz.DecLength <= op.DecLength + MantissaLength)
					uz -= op;
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				shiftAmount = Max(oldBitLength - MantissaLength, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
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
				var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
				if (bitLengthDiff > 0)
					ulr = ulr >> bitLengthDiff << bitLengthDiff;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				UnsignedLongDecimal op = new(new MpuT(bytes.AsSpan(), RandomOrder()), MantissaLength);
				if (op == 0)
					return;
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(oldBitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= (MpuT)op;
				shiftAmount = Max(oldBitLength - MantissaLength, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				ulr %= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(Max(oldBitLength, op.DecLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + MantissaLength
					|| uz.DecLength > op.DecLength + MantissaLength)
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
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(Max(oldBitLength, op.DecLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + MantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + MantissaLength)
					uz |= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr |= op;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldBitLength = uz.DecLength;
				var shiftAmount = Max(Max(oldBitLength, op.DecLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + MantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + MantissaLength)
					uz ^= op;
				ulr = ulr >> shiftAmount << shiftAmount;
				ulr ^= op;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (uint)(int)uz;
				ulr = (uint)(int)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (uint)uz;
				ulr = (uint)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (ulong)(long)uz;
				ulr = (ulong)(long)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (ulong)uz;
				ulr = (ulong)ulr;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (MpuT)(double)uz;
				ulr = new((double)ulr, MantissaLength);
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (MpuT)(decimal)uz;
				ulr = new((decimal)ulr, MantissaLength);
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
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(bitLengthDiff), MantissaOverflow);
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(bitLengthDiff), (expected - actual).Abs());
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, MantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void ConversionTest()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		var counter = 0;
	l1:
		for (var i = 0; i < 1000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongDecimal ulr = new(uz, MantissaLength);
			var bitLengthDiff = Max(uz.DecLength - MantissaLength - 1, 0);
			if (bitLengthDiff > 0)
				uz = uz.ShiftRightRoundDec(bitLengthDiff).ShiftLeftDec(bitLengthDiff);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(bitLengthDiff), MantissaOverflow);
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.AreEqual(expected, actual);
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, MantissaByteLength)..bytesWritten), -1));
		}
		if (counter++ < 2500)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestAdd()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
	l1:
		bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		var mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
		var maxMantissaLength = mantissaLength;
		UnsignedLongDecimal ulr = new(uz, mantissaLength);
		Validate();
		void Action()
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			MpuT op = new(bytes.AsSpan(), RandomOrder());
			maxMantissaLength = Max(mantissaLength, mantissaLength2);
			uz += op;
			ulr += new UnsignedLongDecimal(op, mantissaLength2);
			Validate();
		}
		for (var i = 0; i < 1000; i++)
		{
			mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			ulr = new(uz, mantissaLength);
			Action();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.DecLength - maxMantissaLength - 1, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(bitLengthDiff), maxMantissaLength);
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false);
			var maxMantissaByteLength = Min(bytesWritten, GetArrayLength(maxMantissaLength, 8));
			using var actual = new MpuT(writeBuffer.AsSpan(0, maxMantissaByteLength), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(maxMantissaLength), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > maxMantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, maxMantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void TestCompareTo()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			using UnsignedLongDecimal ulr = new(bytes.AsSpan(), RandomOrder(), MantissaLength);
			if (bytes.Length - MantissaByteLength is 3 or 4)
				continue;
			ProcessA(ulr);
		}
		void ProcessA(UnsignedLongDecimal ulr)
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
			Validate(ulr, num);
			num = (short)0;
			Validate(ulr, num);
			num = (ushort)0;
			Validate(ulr, num);
			num = 0;
			Validate(ulr, num);
			num = 0u;
			Validate(ulr, num);
			num = 0L;
			Validate(ulr, num);
			num = 0uL;
			Validate(ulr, num);
			num = MpuT.Zero;
			Validate2(ulr, num);
			num = MpzT.Zero;
			Validate2(ulr, num);
			num = UnsignedLongDecimal.Zero;
			Validate2(ulr, num);
		}
		void ProcessB(UnsignedLongDecimal ulr, dynamic num)
		{
			dynamic num2 = (byte)num;
			Validate(ulr, num2);
			num2 = (short)num is var si && si < 0 ? ~si : si;
			Validate(ulr, num2);
			num2 = (ushort)num;
			Validate(ulr, num2);
			num2 = (int)num is var i && i < 0 ? ~i : i;
			Validate(ulr, num2);
			num2 = (uint)num;
			Validate(ulr, num2);
			num2 = (long)num is var li && li < 0 ? ~li : li;
			Validate(ulr, num2);
			num2 = (ulong)num;
			Validate(ulr, num2);
			num2 = (MpuT)num;
			Validate2(ulr, num2);
			num2 = (MpzT)num;
			Validate2(ulr, num2);
			num2 = new UnsignedLongDecimal(num, MantissaLength);
			var comp = num2.ToByteArray(1) is not byte[] rightArr
				? 0 : ulr.ToByteArray(1) is var leftArr
				&& leftArr.Length.CompareTo(rightArr.Length) is var lenDiff && lenDiff != 0
				? Sign(lenDiff) : MemoryExtensions.CommonPrefixLength(leftArr, rightArr) is var len
				&& len == leftArr.Length && len == rightArr.Length
				? 0 : len == leftArr.Length ? -1 : len == rightArr.Length ? 1 : Sign(leftArr[len].CompareTo(rightArr[len]));
			Assert.AreEqual(comp, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(comp, Sign(ulr.CompareTo((object)num2)));
			Assert.AreEqual(comp, -Sign(num2.CompareTo(ulr)));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
		static void Validate(UnsignedLongDecimal ulr, dynamic num2)
		{
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
		}
		static void Validate2(UnsignedLongDecimal ulr, dynamic num)
		{
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, -Sign(num.CompareTo(ulr)));
		}
	}

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			using UnsignedLongDecimal ulr = new(bytes.AsSpan(), RandomOrder(), MantissaLength);
			if (bytes.Length - MantissaByteLength is 3 or 4)
				continue;
			ProcessA(ulr);
		}
		void ProcessA(UnsignedLongDecimal ulr)
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
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = (short)0;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = (ushort)0;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0u;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0L;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = 0uL;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			num = MpuT.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), num.Equals(ulr));
			num = MpzT.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), num.Equals(ulr));
			num = UnsignedLongDecimal.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), num.Equals(ulr));
		}
		void ProcessB(UnsignedLongDecimal ulr, dynamic num)
		{
			dynamic num2 = (byte)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (short)num is var si && si < 0 ? ~si : si;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (ushort)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (int)num is var i && i < 0 ? ~i : i;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (uint)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (long)num is var li && li < 0 ? ~li : li;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (ulong)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			num2 = (MpuT)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), num2.Equals(ulr));
			num2 = (MpzT)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals(num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), ulr.Equals((object)num2));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num2), num2.Equals(ulr));
			num2 = new UnsignedLongDecimal(num, MantissaLength);
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), ulr.Equals(num2));
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), ulr.Equals((object)num2));
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), num2.Equals(ulr));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestIncrementDecrement()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(259), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			UnsignedLongDecimal ulr = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			bytes.FillInPlace(random.Next(3), _ => (byte)random.Next(256));
			bytes.PadRightInPlace(4);
			Assert.AreEqual(++uz, ++ulr);
			Assert.AreEqual(uz++, ulr++);
			Assert.AreEqual(--uz, --ulr);
			Assert.AreEqual(uz--, ulr--);
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestShifts()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(259), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongDecimal ulr = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			bytes.FillInPlace(random.Next(3), _ => (byte)random.Next(256));
			bytes.PadRightInPlace(4);
			shiftAmount = BitConverter.ToInt32(bytes.AsSpan());
			Assert.AreEqual(uz.ShiftLeftDec(shiftAmount), ulr << shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), ulr >> shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), ulr >>> shiftAmount);
			Assert.AreEqual(uz.ShiftLeftDec(shiftAmount), ulr << (UnsignedLongDecimal)shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), ulr >> (UnsignedLongDecimal)shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), ulr >>> (UnsignedLongDecimal)shiftAmount);
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestSubtract()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
	l1:
		bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		var mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
		var maxMantissaLength = mantissaLength;
		UnsignedLongDecimal ulr = new(uz, mantissaLength);
		Validate();
		void Action()
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			MpuT op = new(bytes.AsSpan(), RandomOrder());
			maxMantissaLength = Max(mantissaLength, mantissaLength2);
			var minMantissaLength = Min(mantissaLength, mantissaLength2);
			var shiftAmount = uz.DecLength < minMantissaLength + 1 ? 0
				: Max(uz.DecLength - minMantissaLength - 1, 0);
			var shiftAmountLite = uz.DecLength < minMantissaLength + 1 ? 0
				: Max(uz.DecLength - mantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmountLite).ShiftLeftDec(shiftAmountLite);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			ulr = ulr >> shiftAmount << shiftAmount;
			if (random.Next(1000) == 0)
				op = uz;
			if (op > uz)
				return;
			if (uz.DecLength <= op.DecLength + maxMantissaLength)
				uz -= op;
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			ulr -= new UnsignedLongDecimal(op, mantissaLength2);
			Validate();
		}
		for (var i = 0; i < 1000; i++)
		{
			mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			ulr = new(uz, mantissaLength);
			Action();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var bitLengthDiff = Max(uz.DecLength - maxMantissaLength - 1, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(bitLengthDiff), maxMantissaLength);
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false);
			var maxMantissaByteLength = Min(bytesWritten, GetArrayLength(maxMantissaLength, 8));
			using var actual = new MpuT(writeBuffer.AsSpan(0, maxMantissaByteLength), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(maxMantissaLength), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > maxMantissaByteLength)
				Assert.AreEqual(bitLengthDiff + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, maxMantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void TestToByteArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			var order = RandomOrder();
			if (order < 0)
				bytes.Resize(Max(bytes.FindLastIndex(x => x != 0), 0) + 1);
			else
				bytes.ResizeLeft(Max(bytes.Length, 1) - Max(bytes.FindIndex(x => x != 0), 0));
			using UnsignedLongDecimal ulr = new(bytes.AsSpan(), order, random.Next(15, Max(bytes.Length, 15)));
			var bytes2 = ulr.ToByteArray(order, false);
			Assert.IsTrue(bytes.Equals(bytes2));
			Assert.IsTrue(E.SequenceEqual(bytes2, bytes));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToString()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 10000; i++)
		{
			bytes.FillInPlace(random.Next(260), _ => (byte)random.Next(256));
			var order = RandomOrder();
			using UnsignedLongDecimal ulr = new(bytes.AsSpan(), order, MantissaLength);
			var @base = (uint)random.Next(2, 37);
			Assert.IsTrue(ulr.Equals(new UnsignedLongDecimal(ulr.ToString())));
			Assert.IsTrue(ulr.Equals(new UnsignedLongDecimal(ulr.ToString(@base), @base)));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToType()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 10000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongDecimal ulr = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			var type = new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
				typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
				typeof(MpzT), typeof(MpuT), typeof(string), typeof(object) }.Random(random);
			Assert.AreEqual(((IConvertible)uz).ToType(type, CultureInfo.InvariantCulture),
				((IConvertible)ulr).ToType(type, CultureInfo.InvariantCulture));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)ulr).ToType(typeof(DateTime), CultureInfo.InvariantCulture));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)ulr).ToType(typeof(byte[]), CultureInfo.InvariantCulture));
			Assert.AreEqual(((IConvertible)uz).ToType(type, new CultureInfo("ru-RU")),
				((IConvertible)ulr).ToType(type, new CultureInfo("ru-RU")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)ulr).ToType(typeof(DateTime), new CultureInfo("ru-RU")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)ulr).ToType(typeof(byte[]), new CultureInfo("ru-RU")));
			Assert.AreEqual(((IConvertible)uz).ToType(type, new CultureInfo("zh-Hant-CN")),
				((IConvertible)ulr).ToType(type, new CultureInfo("zh-Hant-CN")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)ulr).ToType(typeof(DateTime), new CultureInfo("zh-Hant-CN")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)ulr).ToType(typeof(byte[]), new CultureInfo("zh-Hant-CN")));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestTryParse()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(260), _ => (byte)random.Next(256));
			var order = RandomOrder();
			using UnsignedLongDecimal ulr = new(bytes.AsSpan(), order, MantissaLength);
			var @base = (uint)random.Next(2, 37);
			Assert.IsTrue(UnsignedLongDecimal.TryParse(ulr.ToString(), out var @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(ulr.ToString(),
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(ulr.ToString(),
				new CultureInfo("ru-RU"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(ulr.ToString(),
				new CultureInfo("zh-Hant-CN"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(ulr.ToString(), NumberStyles.None,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(ulr.ToString(), NumberStyles.BinaryNumber,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((ulr.ToString() ?? "0").AsSpan(),
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((ulr.ToString() ?? "0").AsSpan(),
				new CultureInfo("ru-RU"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((ulr.ToString() ?? "0").AsSpan(),
				new CultureInfo("zh-Hant-CN"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((ulr.ToString() ?? "0").AsSpan(), NumberStyles.None,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((ulr.ToString() ?? "0").AsSpan(), NumberStyles.BinaryNumber,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestTryWrite()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024), bytes2 = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			var order = RandomOrder();
			using UnsignedLongDecimal ulr = new(bytes.AsSpan(), order, random.Next(15, Max(bytes.Length, 15)));
			bytes2.FillInPlace(0, bytes.Length);
			if (order < 0)
				ulr.TryWriteLittleEndian(bytes2.AsSpan(), out _, false);
			else
				ulr.TryWriteBigEndian(bytes2.AsSpan(), out _, false);
			Assert.IsTrue(bytes.Equals(bytes2));
			Assert.IsTrue(E.SequenceEqual(bytes2, bytes));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	private static MpuT SafeSubtract(MpuT x, MpuT y) => x >= y ? x - y : x;
}
