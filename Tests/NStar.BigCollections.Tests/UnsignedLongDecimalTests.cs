using System.Globalization;
using System.Reflection;

namespace NStar.BigCollections.Tests;

[TestClass]
public class UnsignedLongDecimalTests
{
	private static readonly int MantissaLength = 300;
	private static readonly int MantissaByteLength = (int)Ceiling((MantissaLength + Log10(9)) * Log(10, 256));
	private static readonly MpuT MantissaOverflow = MpuT.PowerOfTen(MantissaLength);

	[TestMethod]
	public void ComplexTestMixed()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		var counter = 0;
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
	l1:
		bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		UnsignedLongDecimal uld = new(uz, MantissaLength);
		Validate();
		var actions = new[]
		{
			() =>
			{
				var op = (byte)random.Next(256);
				uz += op;
				uld += op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op > uz)
					return;
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
					uz -= op;
				uld -= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				uz *= op;
				uld *= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld /= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldDecLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldDecLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				}
				uld %= op;
				Validate();
			}, () =>
			{
				var op = (byte)random.Next(256);
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld = uld.DivRem(op, out _);
				Validate();
			}, () =>
			{
				var op = random.Next();
				uz += op;
				uld += op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op > uz)
					return;
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
					uz -= op;
				uld -= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				uz *= op;
				uld *= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld /= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldDecLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldDecLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				}
				uld %= op;
				Validate();
			}, () =>
			{
				var op = random.Next();
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld = uld.DivRem(op, out _);
				Validate();
			}, () =>
			{
				var op = random.Next();
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz &= op;
				uld &= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				uz += op;
				uld += op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op > uz)
					return;
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
					uz -= op;
				uld -= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				uz *= op;
				uld *= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld /= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldDecLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldDecLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				}
				uld %= op;
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld = uld.DivRem(op, out _);
				Validate();
			}, () =>
			{
				var op = (uint)random.Next() + (random.Next(2) == 0 ? 0 : 1u << 31);
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz &= op;
				uld &= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				uz += op;
				uld += op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op > uz)
					return;
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
					uz -= op;
				uld -= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				uz *= op;
				uld *= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld /= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldDecLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldDecLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				}
				uld %= op;
				Validate();
			}, () =>
			{
				var op = random.NextInt64();
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld = uld.DivRem(op, out _);
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				uz += op;
				uld += op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (Mpir.Mpir.MpuCmp(op, uz) > 0)
					return;
				if (uz.DecLength <= MantissaLength + ((MpuT)op).DecLength)
					uz -= op;
				uld -= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				uz *= op;
				uld *= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld /= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				if (oldDecLength > MantissaLength + ((MpuT)op).DecLength + 1)
				{
					shiftAmount = oldDecLength - MantissaLength;
					uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				}
				uld %= op;
				Validate();
			}, () =>
			{
				var op = (ulong)random.NextInt64() + (random.Next(2) == 0 ? 0 : 1uL << 63);
				if (op == 0)
					return;
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz /= op;
				uld = uld.DivRem(op, out _);
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			if (random.Next(100) == 0)
				uz = new(bytes.AsSpan(), RandomOrder());
			uld = new(uz, MantissaLength);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var decLengthDiff = uz.DecLength - MantissaLength - 1;
			var decLengthDiffClamped = Max(decLengthDiff, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(decLengthDiffClamped), MantissaOverflow,
				decLengthDiff > 0 || uz.GetByteCount() != uld.GetByteCount(false));
			Assert.IsTrue(uld.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false));
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(decLengthDiffClamped), (expected - actual).Abs());
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(decLengthDiffClamped + 1,
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
		bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		var mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
		var maxMantissaLength = mantissaLength;
		UnsignedLongDecimal uld = new(uz, mantissaLength);
		Validate();
		var actions = new[]
		{
			() =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var shiftAmount = Max(uz.DecLength - mantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz += op;
				uld += new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
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
				uld = uld >> shiftAmount << shiftAmount;
				if (random.Next(1000) == 0)
					op = uz;
				if (op > uz)
					return;
				if (uz.DecLength <= op.DecLength + maxMantissaLength)
					uz -= op;
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld -= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				uz *= op;
				uld *= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var shiftAmount = Max(uz.DecLength - mantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				shiftAmount = Max(op.DecLength - mantissaLength2 - 1, 0);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (op == 0)
					return;
				uz /= op;
				uld /= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				UnsignedLongDecimal op = new(new MpuT(bytes.AsSpan(), RandomOrder()), mantissaLength2);
				var oldDecLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(oldDecLength - minMantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op >> shiftAmount << shiftAmount;
				uld = uld >> shiftAmount << shiftAmount;
				if (op == 0)
					return;
				uz %= (MpuT)op;
				uld %= op;
				shiftAmount = Max(oldDecLength - minMantissaLength, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld = uld >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				uz /= op;
				uld = uld.DivRem(new UnsignedLongDecimal(op, mantissaLength2), out _);
				var shiftAmount = Max(uz.DecLength - maxMantissaLength - 1, 0);
				if (shiftAmount > 0)
					uld = uld >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldDecLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(Max(oldDecLength, op.DecLength) - minMantissaLength - 1, 0);
				var shiftAmountLite = Max(Max(oldDecLength, op.DecLength) - mantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmountLite).ShiftLeftDec(shiftAmountLite);
				uld = uld >> shiftAmountLite << shiftAmountLite;
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (op.DecLength > uz.DecLength + maxMantissaLength || uz.DecLength > op.DecLength + maxMantissaLength)
					uz = 0;
				else
					uz &= op;
				uld &= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldDecLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(Max(oldDecLength, op.DecLength) - minMantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + maxMantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + maxMantissaLength)
					uz |= op;
				uld = uld >> shiftAmount << shiftAmount;
				uld |= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldDecLength = uz.DecLength;
				maxMantissaLength = Max(mantissaLength, mantissaLength2);
				var minMantissaLength = Min(mantissaLength, mantissaLength2);
				var shiftAmount = Max(Max(oldDecLength, op.DecLength) - minMantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + maxMantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + maxMantissaLength)
					uz ^= op;
				uld = uld >> shiftAmount << shiftAmount;
				uld ^= new UnsignedLongDecimal(op, mantissaLength2);
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			uld = new(uz, mantissaLength);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var decLengthDiff = uz.DecLength - maxMantissaLength - 1;
			var decLengthDiffClamped = Max(decLengthDiff, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(decLengthDiffClamped),
				MpuT.PowerOfTen(maxMantissaLength), decLengthDiff > 0 || uz.GetByteCount() != uld.GetByteCount(false));
			Assert.IsTrue(uld.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false));
			var maxMantissaByteLength = Min(bytesWritten, (int)Ceiling((maxMantissaLength + Log10(9)) * Log(10, 256)));
			using var actual = new MpuT(writeBuffer.AsSpan(0, maxMantissaByteLength), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(maxMantissaLength), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > maxMantissaByteLength)
				Assert.AreEqual(decLengthDiffClamped + 1,
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
		bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		UnsignedLongDecimal uld = new(uz, MantissaLength);
		Validate();
		var actions = new[]
		{
			() =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				uz += op;
				uld += new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				shiftAmount = Max(op.DecLength - MantissaLength - 1, 0);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (random.Next(1000) == 0)
					op = uz;
				if (op > uz)
					return;
				if (uz.DecLength <= op.DecLength + MantissaLength)
					uz -= op;
				uld -= new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				uz *= op;
				uld *= new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				shiftAmount = Max(op.DecLength - MantissaLength - 1, 0);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (op == 0)
					return;
				uz /= op;
				uld /= new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				shiftAmount = Max(op.DecLength - MantissaLength - 1, 0);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= op;
				uld %= new UnsignedLongDecimal(op, MantissaLength);
				shiftAmount = Max(oldDecLength - MantissaLength, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld = uld >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				shiftAmount = Max(op.DecLength - MantissaLength - 1, 0);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				if (op == 0)
					return;
				uz /= op;
				uld = uld.DivRem(new UnsignedLongDecimal(op, MantissaLength), out _);
				var decLengthDiffClamped = Max(uz.DecLength - MantissaLength - 1, 0);
				if (decLengthDiffClamped > 0)
					uld = uld >> decLengthDiffClamped << decLengthDiffClamped;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				UnsignedLongDecimal op = new(new MpuT(bytes.AsSpan(), RandomOrder()), MantissaLength);
				if (op == 0)
					return;
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(oldDecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz %= (MpuT)op;
				shiftAmount = Max(oldDecLength - MantissaLength, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld %= op;
				uld = uld >> shiftAmount << shiftAmount;
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(Max(oldDecLength, op.DecLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld = uld >> shiftAmount << shiftAmount;
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + MantissaLength
					|| uz.DecLength > op.DecLength + MantissaLength)
					uz = 0;
				else
					uz &= op;
				uld &= new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(Max(oldDecLength, op.DecLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld = uld >> shiftAmount << shiftAmount;
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + MantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + MantissaLength)
					uz |= op;
				uld |= new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
				MpuT op = new(bytes.AsSpan(), RandomOrder());
				var oldDecLength = uz.DecLength;
				var shiftAmount = Max(Max(oldDecLength, op.DecLength) - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				op = op.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uld = uld >> shiftAmount << shiftAmount;
				if (uz.DecLength < op.DecLength && op.DecLength > uz.DecLength + MantissaLength)
					uz = op;
				else if (uz.DecLength <= op.DecLength + MantissaLength)
					uz ^= op;
				uld ^= new UnsignedLongDecimal(op, MantissaLength);
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (uint)(int)uz;
				uld = (uint)(int)uld;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (uint)uz;
				uld = (uint)uld;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (ulong)(long)uz;
				uld = (ulong)(long)uld;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (ulong)uz;
				uld = (ulong)uld;
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (MpuT)(double)uz;
				uld = new((double)uld, MantissaLength);
				Validate();
			}, () =>
			{
				var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
				uz = (MpuT)(decimal)uz;
				uld = new((decimal)uld, MantissaLength);
				Validate();
			},
		};
		for (var i = 0; i < 1000; i++)
		{
			uld = new(uz, MantissaLength);
			actions.Random(random)();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var decLengthDiff = uz.DecLength - MantissaLength - 1;
			var decLengthDiffClamped = Max(decLengthDiff, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(decLengthDiffClamped), MantissaOverflow,
				decLengthDiff > 0 || uz.GetByteCount() != uld.GetByteCount(false));
			Assert.IsTrue(uld.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false));
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(decLengthDiffClamped), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(decLengthDiffClamped + 1,
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
			bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongDecimal uld = new(uz, MantissaLength);
			var decLengthDiff = uz.DecLength - MantissaLength - 1;
			var decLengthDiffClamped = Max(decLengthDiff, 0);
			if (decLengthDiffClamped > 0)
				uz = uz.ShiftRightRoundDec(decLengthDiffClamped).ShiftLeftDec(decLengthDiffClamped);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(decLengthDiffClamped), MantissaOverflow,
				decLengthDiff > 0 || uz.GetByteCount() != uld.GetByteCount(false));
			Assert.IsTrue(uld.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false));
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.AreEqual(expected, actual);
			if (bytesWritten > MantissaByteLength)
				Assert.AreEqual(decLengthDiffClamped + 1,
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
		bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		var mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
		var maxMantissaLength = mantissaLength;
		UnsignedLongDecimal uld = new(uz, mantissaLength);
		Validate();
		void Action()
		{
			bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
			var mantissaLength2 = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			MpuT op = new(bytes.AsSpan(), RandomOrder());
			maxMantissaLength = Max(mantissaLength, mantissaLength2);
			uz += op;
			uld += new UnsignedLongDecimal(op, mantissaLength2);
			Validate();
		}
		for (var i = 0; i < 1000; i++)
		{
			mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			uld = new(uz, mantissaLength);
			Action();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var decLengthDiff = uz.DecLength - maxMantissaLength - 1;
			var decLengthDiffClamped = Max(decLengthDiff, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(decLengthDiffClamped),
				MpuT.PowerOfTen(maxMantissaLength), decLengthDiff > 0 || uz.GetByteCount() != uld.GetByteCount(false));
			Assert.IsTrue(uld.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false));
			var maxMantissaByteLength = Min(bytesWritten, (int)Ceiling((maxMantissaLength + Log10(9)) * Log(10, 256)));
			using var actual = new MpuT(writeBuffer.AsSpan(0, maxMantissaByteLength), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(maxMantissaLength), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > maxMantissaByteLength)
				Assert.AreEqual(decLengthDiffClamped + 1,
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
			using UnsignedLongDecimal uld = new(bytes.AsSpan(), RandomOrder(), MantissaLength);
			if (bytes.Length - MantissaByteLength is 3 or 4)
				continue;
			ProcessA(uld);
		}
		void ProcessA(UnsignedLongDecimal uld)
		{
			dynamic num = uld;
			ProcessB(uld, num);
			num = uld + 1;
			ProcessB(uld, num);
			if (uld.CompareTo(0) != 0)
			{
				num = uld - 1;
				ProcessB(uld, num);
			}
			num = uld * 2;
			ProcessB(uld, num);
			num = uld / 2;
			ProcessB(uld, num);
			num = uld * 3;
			ProcessB(uld, num);
			num = uld / 3;
			ProcessB(uld, num);
			num = (byte)0;
			Validate(uld, num);
			num = (short)0;
			Validate(uld, num);
			num = (ushort)0;
			Validate(uld, num);
			num = 0;
			Validate(uld, num);
			num = 0u;
			Validate(uld, num);
			num = 0L;
			Validate(uld, num);
			num = 0uL;
			Validate(uld, num);
			num = MpuT.Zero;
			Validate2(uld, num);
			num = MpzT.Zero;
			Validate2(uld, num);
			num = UnsignedLongDecimal.Zero;
			Validate2(uld, num);
		}
		void ProcessB(UnsignedLongDecimal uld, dynamic num)
		{
			dynamic num2 = (byte)num;
			Validate(uld, num2);
			num2 = (short)num is var si && si < 0 ? ~si : si;
			Validate(uld, num2);
			num2 = (ushort)num;
			Validate(uld, num2);
			num2 = (int)num is var i && i < 0 ? ~i : i;
			Validate(uld, num2);
			num2 = (uint)num;
			Validate(uld, num2);
			num2 = (long)num is var li && li < 0 ? ~li : li;
			Validate(uld, num2);
			num2 = (ulong)num;
			Validate(uld, num2);
			num2 = (MpuT)num;
			Validate2(uld, num2);
			num2 = (MpzT)num;
			Validate2(uld, num2);
			num2 = new UnsignedLongDecimal(num, MantissaLength);
			var comp = num2.ToByteArray(1) is not byte[] rightArr
				? 0 : uld.ToByteArray(1) is var leftArr
				&& leftArr.Length.CompareTo(rightArr.Length) is var lenDiff && lenDiff != 0
				? Sign(lenDiff) : MemoryExtensions.CommonPrefixLength(leftArr, rightArr) is var len
				&& len == leftArr.Length && len == rightArr.Length
				? 0 : len == leftArr.Length ? -1 : len == rightArr.Length ? 1 : Sign(leftArr[len].CompareTo(rightArr[len]));
			Assert.AreEqual(comp, Sign(uld.CompareTo(num2)));
			Assert.AreEqual(comp, Sign(uld.CompareTo((object)num2)));
			Assert.AreEqual(comp, -Sign(num2.CompareTo(uld)));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
		static void Validate(UnsignedLongDecimal uld, dynamic num2)
		{
			Assert.AreEqual(uld.Equals((MpuT)uld) ? Sign(((MpuT)uld).CompareTo(num2)) : 1, Sign(uld.CompareTo(num2)));
			Assert.AreEqual(uld.Equals((MpuT)uld) ? Sign(((MpuT)uld).CompareTo(num2)) : 1, Sign(uld.CompareTo((object)num2)));
		}
		static void Validate2(UnsignedLongDecimal uld, dynamic num)
		{
			Assert.AreEqual(uld.Equals((MpuT)uld) ? Sign(((MpuT)uld).CompareTo(num)) : 1, Sign(uld.CompareTo(num)));
			Assert.AreEqual(uld.Equals((MpuT)uld) ? Sign(((MpuT)uld).CompareTo(num)) : 1, Sign(uld.CompareTo((object)num)));
			Assert.AreEqual(uld.Equals((MpuT)uld) ? Sign(((MpuT)uld).CompareTo(num)) : 1, -Sign(num.CompareTo(uld)));
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
			using UnsignedLongDecimal uld = new(bytes.AsSpan(), RandomOrder(), MantissaLength);
			if (bytes.Length - MantissaByteLength is 3 or 4)
				continue;
			ProcessA(uld);
		}
		void ProcessA(UnsignedLongDecimal uld)
		{
			dynamic num = uld;
			ProcessB(uld, num);
			num = uld + 1;
			ProcessB(uld, num);
			if (uld.CompareTo(0) != 0)
			{
				num = uld - 1;
				ProcessB(uld, num);
			}
			num = uld * 2;
			ProcessB(uld, num);
			num = uld / 2;
			ProcessB(uld, num);
			num = uld * 3;
			ProcessB(uld, num);
			num = uld / 3;
			ProcessB(uld, num);
			num = (byte)0;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = (short)0;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = (ushort)0;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = 0;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = 0u;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = 0L;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = 0uL;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			num = MpuT.Zero;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), num.Equals(uld));
			num = MpzT.Zero;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), num.Equals(uld));
			num = UnsignedLongDecimal.Zero;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals(num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), uld.Equals((object)num));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num), num.Equals(uld));
		}
		void ProcessB(UnsignedLongDecimal uld, dynamic num)
		{
			dynamic num2 = (byte)num;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (short)num is var si && si < 0 ? ~si : si;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (ushort)num;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (int)num is var i && i < 0 ? ~i : i;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (uint)num;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (long)num is var li && li < 0 ? ~li : li;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (ulong)num;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			num2 = (MpuT)num;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), num2.Equals(uld));
			num2 = (MpzT)num;
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals(num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), uld.Equals((object)num2));
			Assert.AreEqual(uld.Equals((MpuT)uld) && ((MpuT)uld).Equals(num2), num2.Equals(uld));
			num2 = new UnsignedLongDecimal(num, MantissaLength);
			Assert.AreEqual(E.SequenceEqual(uld.ToByteArray(-1), num2.ToByteArray(-1)), uld.Equals(num2));
			Assert.AreEqual(E.SequenceEqual(uld.ToByteArray(-1), num2.ToByteArray(-1)), uld.Equals((object)num2));
			Assert.AreEqual(E.SequenceEqual(uld.ToByteArray(-1), num2.ToByteArray(-1)), num2.Equals(uld));
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
			UnsignedLongDecimal uld = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			Assert.AreEqual(++uz, ++uld);
			Assert.AreEqual(uz++, uld++);
			Assert.AreEqual(--uz, --uld);
			Assert.AreEqual(uz--, uld--);
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestShifts()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 100000; i++)
		{
			bytes.FillInPlace(random.Next(259), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongDecimal uld = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			bytes.FillInPlace(random.Next(3), _ => (byte)random.Next(256));
			bytes.PadRightInPlace(4);
			shiftAmount = BitConverter.ToInt32(bytes.AsSpan());
			Assert.AreEqual(uz.ShiftLeftDec(shiftAmount), uld << shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), uld >> shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), uld >>> shiftAmount);
			Assert.AreEqual(uz.ShiftLeftDec(shiftAmount), uld << (UnsignedLongDecimal)shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), uld >> (UnsignedLongDecimal)shiftAmount);
			Assert.AreEqual(uz.ShiftRightRoundDec(shiftAmount), uld >>> (UnsignedLongDecimal)shiftAmount);
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
		bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
		MpuT uz = new(bytes.AsSpan(), RandomOrder());
		var mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
		var maxMantissaLength = mantissaLength;
		UnsignedLongDecimal uld = new(uz, mantissaLength);
		Validate();
		void Action()
		{
			bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
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
			uld = uld >> shiftAmount << shiftAmount;
			if (random.Next(1000) == 0)
				op = uz;
			if (op > uz)
				return;
			if (uz.DecLength <= op.DecLength + maxMantissaLength)
				uz -= op;
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			uld -= new UnsignedLongDecimal(op, mantissaLength2);
			Validate();
		}
		for (var i = 0; i < 1000; i++)
		{
			mantissaLength = (int)Round(Pow(2, random.NextDouble() * 2) * 150);
			uld = new(uz, mantissaLength);
			Action();
		}
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) * 2 - 1;
		void Validate()
		{
			var decLengthDiff = uz.DecLength - maxMantissaLength - 1;
			var decLengthDiffClamped = Max(decLengthDiff, 0);
			using var expected = (MpzT)SafeSubtract(uz.ShiftRightRoundDec(decLengthDiffClamped),
				MpuT.PowerOfTen(maxMantissaLength), decLengthDiff > 0 || uz.GetByteCount() != uld.GetByteCount(false));
			Assert.IsTrue(uld.TryWriteLittleEndian(writeBuffer, out var bytesWritten, false));
			var maxMantissaByteLength = Min(bytesWritten, (int)Ceiling((maxMantissaLength + Log10(9)) * Log(10, 256)));
			using var actual = new MpuT(writeBuffer.AsSpan(0, maxMantissaByteLength), -1);
			Assert.IsLessThanOrEqualTo(MpuT.PowerOfTen(maxMantissaLength), (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(expected >> 2, (expected - actual).Abs());
			Assert.IsLessThanOrEqualTo(actual >> 2, (expected - actual).Abs());
			if (bytesWritten > maxMantissaByteLength)
				Assert.AreEqual(decLengthDiffClamped + 1,
					new MpuT(writeBuffer.AsSpan(Min(bytesWritten, maxMantissaByteLength)..bytesWritten), -1));
		}
	}

	[TestMethod]
	public void TestToByteArray()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 1000000; i++)
		{
			bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
			var order = RandomOrder();
			if (order < 0)
				bytes.Resize(Max(bytes.FindLastIndex(x => x != 0), 0) + 1);
			else
				bytes.ResizeLeft(Max(bytes.Length, 1) - Max(bytes.FindIndex(x => x != 0), 0));
			var mantissaLength = random.Next(15, Max((int)Ceiling(bytes.Length * Log10(256)), 15));
			using UnsignedLongDecimal uld = new(bytes.AsSpan(), order, mantissaLength);
			using UnsignedLongDecimal uld2 = new(uld.ToByteArray(order, false), order, mantissaLength);
			Assert.IsTrue(uld.Equals(uld2));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestToString()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 2500; i++)
		{
			bytes.FillInPlace(random.Next(MantissaByteLength + 4), _ => (byte)random.Next(256));
			var order = RandomOrder();
			using UnsignedLongDecimal uld = new(bytes.AsSpan(), order, MantissaLength);
			var @base = (uint)random.Next(2, 37);
			Assert.IsTrue(uld.Equals(new UnsignedLongDecimal(uld.ToString())));
			Assert.IsTrue(uld.Equals(new UnsignedLongDecimal(uld.ToString(@base), @base)));
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
			bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongDecimal uld = new(uz, MantissaLength);
			var shiftAmount = Max(uz.DecLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRoundDec(shiftAmount).ShiftLeftDec(shiftAmount);
			var type = new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
				typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
				typeof(MpzT), typeof(MpuT), typeof(string), typeof(object) }.Random(random);
			Assert.AreEqual(((IConvertible)uz).ToType(type, CultureInfo.InvariantCulture),
				((IConvertible)uld).ToType(type, CultureInfo.InvariantCulture));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)uld).ToType(typeof(DateTime), CultureInfo.InvariantCulture));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)uld).ToType(typeof(byte[]), CultureInfo.InvariantCulture));
			Assert.AreEqual(((IConvertible)uz).ToType(type, new CultureInfo("ru-RU")),
				((IConvertible)uld).ToType(type, new CultureInfo("ru-RU")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)uld).ToType(typeof(DateTime), new CultureInfo("ru-RU")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)uld).ToType(typeof(byte[]), new CultureInfo("ru-RU")));
			Assert.AreEqual(((IConvertible)uz).ToType(type, new CultureInfo("zh-Hant-CN")),
				((IConvertible)uld).ToType(type, new CultureInfo("zh-Hant-CN")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)uld).ToType(typeof(DateTime), new CultureInfo("zh-Hant-CN")));
			Assert.ThrowsExactly<InvalidCastException>(() =>
				((IConvertible)uld).ToType(typeof(byte[]), new CultureInfo("zh-Hant-CN")));
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
			bytes.FillInPlace(random.Next(MantissaByteLength + 3), _ => (byte)random.Next(256));
			var order = RandomOrder();
			using UnsignedLongDecimal uld = new(bytes.AsSpan(), order, MantissaLength);
			var @base = (uint)random.Next(2, 37);
			Assert.IsTrue(UnsignedLongDecimal.TryParse(uld.ToString(), out var @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(uld.ToString(),
				CultureInfo.InvariantCulture, out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(uld.ToString(),
				new CultureInfo("ru-RU"), out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(uld.ToString(),
				new CultureInfo("zh-Hant-CN"), out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(uld.ToString(), NumberStyles.None,
				CultureInfo.InvariantCulture, out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse(uld.ToString(), NumberStyles.BinaryNumber,
				CultureInfo.InvariantCulture, out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((uld.ToString() ?? "0").AsSpan(),
				CultureInfo.InvariantCulture, out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((uld.ToString() ?? "0").AsSpan(),
				new CultureInfo("ru-RU"), out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((uld.ToString() ?? "0").AsSpan(),
				new CultureInfo("zh-Hant-CN"), out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((uld.ToString() ?? "0").AsSpan(), NumberStyles.None,
				CultureInfo.InvariantCulture, out @string) && uld.Equals(@string));
			Assert.IsTrue(UnsignedLongDecimal.TryParse((uld.ToString() ?? "0").AsSpan(), NumberStyles.BinaryNumber,
				CultureInfo.InvariantCulture, out @string) && uld.Equals(@string));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	[TestMethod]
	public void TestTryWrite()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024), bytes2 = new(1024);
		for (var i = 0; i < 250000; i++)
		{
			bytes.FillInPlace(random.Next(500), _ => (byte)random.Next(256));
			var order = RandomOrder();
			var mantissaLength = random.Next(15, Max((int)Ceiling(bytes.Length * Log10(256)), 15));
			using UnsignedLongDecimal uld = new(bytes.AsSpan(), order, mantissaLength);
			bytes2.FillInPlace(0, bytes.Length + 2);
			if (order < 0)
				Assert.IsTrue(uld.TryWriteLittleEndian(bytes2.AsSpan(), out _, false));
			else
				Assert.IsTrue(uld.TryWriteBigEndian(bytes2.AsSpan(), out _, false));
			Assert.IsTrue(uld.Equals(new UnsignedLongDecimal(bytes2.AsSpan(), order, mantissaLength)));
		}
		int RandomOrder() => random.Next(2) * 2 - 1;
	}

	private static MpuT SafeSubtract(MpuT x, MpuT mantissaOverflow, bool doSubtract) =>
		x >= mantissaOverflow && doSubtract ? x - mantissaOverflow : x;
}
