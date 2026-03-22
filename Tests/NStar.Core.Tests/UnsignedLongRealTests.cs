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
	public void ComplexTest()
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
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength)
				{
					var shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
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
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength)
				{
					var shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
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
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength)
				{
					var shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
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
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength)
				{
					var shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
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
				uz %= op;
				if (oldBitLength > MantissaLength + ((MpuT)op).BitLength)
				{
					var shiftAmount = oldBitLength - MantissaLength;
					uz = uz.ShiftRightRound(shiftAmount) << shiftAmount;
				}
				ulr %= op;
				Validate();
			}, () =>
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
				if (op != 0)
					return;
				var oldBitLength = uz.BitLength;
				uz %= op;
				if (uz.BitLength >= MantissaLength)
					uz &= (new MpuT(1) << MantissaLength + 1) - 1 << uz.BitLength - MantissaLength - 1;
				ulr %= op;
				Validate();
			}, () =>
			{
				uz = (uint)(int)uz;
				ulr = (uint)(int)ulr;
				Validate();
			}, () =>
			{
				uz = (uint)uz;
				ulr = (uint)ulr;
				Validate();
			}, () =>
			{
				uz = (ulong)(long)uz;
				ulr = (ulong)(long)ulr;
				Validate();
			}, () =>
			{
				uz = (ulong)uz;
				ulr = (ulong)ulr;
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
			if (bitLengthDiff > 0)
				uz = uz.ShiftRightRound(bitLengthDiff) << bitLengthDiff;
			using var expected = (MpzT)uz.ShiftRightRound(bitLengthDiff) & MantissaMask;
			ulr.TryWriteLittleEndian(writeBuffer, out var bytesWritten);
			using var actual = new MpuT(writeBuffer.AsSpan(0, Min(bytesWritten, MantissaByteLength)), -1);
			Assert.IsLessThanOrEqualTo(1, (expected - actual).Abs());
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
		if (counter++ < 10000)
			goto l1;
		int RandomOrder() => random.Next(2) == 0 ? 1 : -1;
	}
}
