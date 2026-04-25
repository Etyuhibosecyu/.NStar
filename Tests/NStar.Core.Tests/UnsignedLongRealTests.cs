using System.Globalization;

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
				var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				if (random.Next(1000) == 0)
					op = uz;
				if (op > uz)
					return;
				var old = uz;
				if (uz.BitLength <= op.BitLength + MantissaLength)
					uz -= op;
				uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
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
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		var counter = 0;
	l1:
		for (var i = 0; i < 1000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			MpuT uz = new(bytes.AsSpan(), RandomOrder());
			using UnsignedLongReal ulr = uz;
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

	[TestMethod]
	public void TestCompareTo()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			using UnsignedLongReal ulr = new(bytes.AsSpan(), RandomOrder());
			ProcessA(ulr);
		}
		void ProcessA(UnsignedLongReal ulr)
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
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = (short)0;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = (ushort)0;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = 0;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = 0u;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = 0L;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = 0uL;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			num = MpuT.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, -Sign(num.CompareTo(ulr)));
			num = MpzT.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, -Sign(num.CompareTo(ulr)));
			num = UnsignedLongReal.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo(num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, Sign(ulr.CompareTo((object)num)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num)) : 1, -Sign(num.CompareTo(ulr)));
		}
		void ProcessB(UnsignedLongReal ulr, dynamic num)
		{
			dynamic num2 = (byte)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (short)num is var si && si < 0 ? ~si : si;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (ushort)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (int)num is var i && i < 0 ? ~i : i;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (uint)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (long)num is var li && li < 0 ? ~li : li;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (ulong)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			num2 = (MpuT)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, -Sign(num2.CompareTo(ulr)));
			num2 = (MpzT)num;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo(num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, Sign(ulr.CompareTo((object)num2)));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) ? Sign(((MpuT)ulr).CompareTo(num2)) : 1, -Sign(num2.CompareTo(ulr)));
			num2 = (UnsignedLongReal)num;
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
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
	}

	[TestMethod]
	public void TestEquals()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		for (var i = 0; i < 5000; i++)
		{
			bytes.FillInPlace(random.Next(1000), _ => (byte)random.Next(256));
			using UnsignedLongReal ulr = new(bytes.AsSpan(), RandomOrder());
			ProcessA(ulr);
		}
		void ProcessA(UnsignedLongReal ulr)
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
			num = UnsignedLongReal.Zero;
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals(num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), ulr.Equals((object)num));
			Assert.AreEqual(ulr.Equals((MpuT)ulr) && ((MpuT)ulr).Equals(num), num.Equals(ulr));
		}
		void ProcessB(UnsignedLongReal ulr, dynamic num)
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
			num2 = (UnsignedLongReal)num;
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), ulr.Equals(num2));
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), ulr.Equals((object)num2));
			Assert.AreEqual(E.SequenceEqual(ulr.ToByteArray(-1), num2.ToByteArray(-1)), num2.Equals(ulr));
		}
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
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
			using UnsignedLongReal ulr = new(bytes.AsSpan(), order);
			var bytes2 = ulr.ToByteArray(order);
			Assert.IsTrue(bytes.Equals(bytes2));
			Assert.IsTrue(E.SequenceEqual(bytes2, bytes));
		}
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
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
			using UnsignedLongReal ulr = new(bytes.AsSpan(), order);
			var @base = (uint)random.Next(2, 37);
			Assert.IsTrue(ulr.Equals(new UnsignedLongReal(ulr.ToString())));
			Assert.IsTrue(ulr.Equals(new UnsignedLongReal(ulr.ToString(@base), @base)));
		}
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
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
			using UnsignedLongReal ulr = uz;
			var shiftAmount = Max(uz.BitLength - MantissaLength - 1, 0);
			uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
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
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
	}

	[TestMethod]
	public void TestTryParse()
	{
		var random = Lock(lockObj, () => new Random(Global.random.Next()));
		List<byte> bytes = new(1024);
		var writeBuffer = GC.AllocateUninitializedArray<byte>(MantissaByteLength * 3);
		for (var i = 0; i < 1000; i++)
		{
			bytes.FillInPlace(random.Next(260), _ => (byte)random.Next(256));
			var order = RandomOrder();
			using UnsignedLongReal ulr = new(bytes.AsSpan(), order);
			var @base = (uint)random.Next(2, 37);
			Assert.IsTrue(UnsignedLongReal.TryParse(ulr.ToString(), out var @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse(ulr.ToString(),
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse(ulr.ToString(),
				new CultureInfo("ru-RU"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse(ulr.ToString(),
				new CultureInfo("zh-Hant-CN"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse(ulr.ToString(), NumberStyles.None,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse(ulr.ToString(), NumberStyles.BinaryNumber,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse((ulr.ToString() ?? "0").AsSpan(),
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse((ulr.ToString() ?? "0").AsSpan(),
				new CultureInfo("ru-RU"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse((ulr.ToString() ?? "0").AsSpan(),
				new CultureInfo("zh-Hant-CN"), out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse((ulr.ToString() ?? "0").AsSpan(), NumberStyles.None,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
			Assert.IsTrue(UnsignedLongReal.TryParse((ulr.ToString() ?? "0").AsSpan(), NumberStyles.BinaryNumber,
				CultureInfo.InvariantCulture, out @string) && ulr.Equals(@string));
		}
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
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
			using UnsignedLongReal ulr = new(bytes.AsSpan(), order);
			bytes2.FillInPlace(0, bytes.Length);
			if (order < 0)
				ulr.TryWriteLittleEndian(bytes2.AsSpan(), out _);
			else
				ulr.TryWriteBigEndian(bytes2.AsSpan(), out _);
			Assert.IsTrue(bytes.Equals(bytes2));
			Assert.IsTrue(E.SequenceEqual(bytes2, bytes));
		}
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
	}
}
