global using NStar.Core;
global using NStar.Dictionaries;
global using NStar.Linq;
global using System;
global using System.Buffers;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text;
global using System.Threading;
global using static NStar.Core.Extents;
global using static NStar.Mpir.MpzT;

namespace NStar.Mpir;

/// <summary>
/// Представляет число с плавающей точкой, беззнаковое как по мантиссе - только неотрицательное - так и по экспоненте -
/// только целое, но, в отличие от <see cref="MpuT"/>, которое также является целым неотрицательным числом,
/// в данном типе очень большие числа тратят несравнимо меньше памяти, легко умещаются числа намного больше гуголплекса.
/// </summary>
[DebuggerDisplay("{ToShortString()}")]
public sealed class UnsignedLongReal : IUnsignedLongReal<UnsignedLongReal>
{
	private enum ComputeOperation : byte
	{
		Identity,
		BitLength,
		Compare,
		ChangeML,
		Add,
		Subtract,
	}

	private static readonly ConcurrentDictionary<int, MpuT> MantissaMasks = [], MantissaOverflows = [];
	internal readonly MpuT m;
	internal readonly UnsignedLongReal? e;
	internal readonly int MantissaLength = 0;
	public const int AutoMantissaLength = -1, DefaultMantissaLength = 2048, MinMantissaLength = 64;

	internal UnsignedLongReal(MpuT m, UnsignedLongReal? e, int mantissaLength = DefaultMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		this.m = m;
		this.e = e;
	}

	public UnsignedLongReal(decimal op, int mantissaLength = DefaultMantissaLength) : this(new MpuT(op), mantissaLength) { }

	public UnsignedLongReal(double op, int mantissaLength = DefaultMantissaLength) : this(new MpuT(op), mantissaLength) { }

	public UnsignedLongReal(int op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongReal(uint op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongReal(long op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongReal(ulong op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongReal(MpzT op, int mantissaLength = DefaultMantissaLength) : this(op < 0
		? throw new ArgumentException(NoNegativeNumbers, nameof(op))
		: Unsafe.As<MpuT>(op), mantissaLength) { }

	public UnsignedLongReal(MpuT op, int mantissaLength = DefaultMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		var bitLength = op.BitLength;
		if (bitLength <= MantissaLength)
		{
			m = op;
			e = null;
		}
		else
		{
			var eDiff = bitLength - MantissaLength - 1;
			var shifted = op.ShiftRightRound(eDiff);
			if (shifted == MantissaOverflow << 1)
				eDiff++;
			m = shifted & MantissaMask;
			e = new(eDiff + 1, mantissaLength);
		}
	}

	public UnsignedLongReal(UnsignedLongReal op) : this(op.m, op.e, op.MantissaLength) { }

	public UnsignedLongReal(UnsignedLongReal op, int mantissaLength)
		: this(op.GetWithOtherML(mantissaLength, true) is var x ? x.m : MpuT.Zero, x.e, mantissaLength) { }

	public UnsignedLongReal(BigInteger op, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(op), mantissaLength) { }

	public UnsignedLongReal(string? s, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(s), mantissaLength) { }

	public UnsignedLongReal(string? s, uint @base, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(s, @base), mantissaLength) { }

	public UnsignedLongReal(ReadOnlySpan<byte> bytes, int order, int mantissaLength = AutoMantissaLength)
	{
		if (mantissaLength == AutoMantissaLength)
		{
			if (bytes.Length < sizeof(int))
			{
				mantissaLength = DefaultMantissaLength;
				bytes = default;
			}
			else
			{
				mantissaLength = BitConverter.ToInt32(bytes[..sizeof(int)]);
				bytes = bytes[sizeof(int)..];
			}
		}
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		var mantissaByteLength = MantissaByteLength;
		if (bytes.Length <= mantissaByteLength)
		{
			m = new(bytes, order);
			e = null;
		}
		else
		{
			var mStart = Math.Max(order, 0) * (bytes.Length - mantissaByteLength);
			var eStart = Math.Max(-order, 0) * mantissaByteLength;
			m = new(bytes.Slice(mStart, mantissaByteLength), order);
			e = new UnsignedLongReal(bytes.Slice(eStart, bytes.Length - mantissaByteLength), order, mantissaLength)
				is var num && num > 0 ? num : null;
		}
	}

	~UnsignedLongReal() => Dispose();

	public static UnsignedLongReal AdditiveIdentity => Zero;
	/// <summary>Gets the count of bits in the binary representation of the number.</summary>
	public UnsignedLongReal BitLength => Compute(this, null!, ComputeOperation.BitLength);
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.E => throw new NotSupportedException();
	UnsignedLongReal? IUnsignedLongReal<UnsignedLongReal>.Exponent => e;
	MpuT IUnsignedLongReal<UnsignedLongReal>.Mantissa => m;
	private int MantissaByteLength => GetArrayLength(MantissaLength, 8);
	int IUnsignedLongReal<UnsignedLongReal>.MantissaByteLength => MantissaByteLength;
	int IUnsignedLongReal<UnsignedLongReal>.MantissaLength => MantissaLength;
	private MpuT MantissaMask => MantissaMasks.GetOrAdd(MantissaLength, x => MantissaOverflow - 1);
	private MpuT MantissaOverflow => MantissaOverflows.GetOrAdd(MantissaLength, x => MpuT.One << x);
	MpuT IUnsignedLongReal<UnsignedLongReal>.MantissaOverflow => MantissaOverflow;
	public static UnsignedLongReal MultiplicativeIdentity => One;
	static UnsignedLongReal ISignedNumber<UnsignedLongReal>.NegativeOne => throw new NotSupportedException();
	public static UnsignedLongReal One { get; } = new(1, MinMantissaLength);
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.Pi => throw new NotSupportedException();
	public static int Radix => 2;
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.Tau => throw new NotSupportedException();
	public static UnsignedLongReal Zero { get; } = new(0, MinMantissaLength);

	public static UnsignedLongReal Abs(UnsignedLongReal op) => new(op.m, op.e);

	public object Clone() => new UnsignedLongReal(m, e, MantissaLength);

	/// <summary>
	/// Сравнивает данное число с <see langword="int"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(int other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see langword="uint"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(uint other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see langword="long"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(long other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see langword="ulong"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(ulong other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see cref="MpuT"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(MpuT other)
	{
		if (e is null)
			return m.CompareTo(other);
		var bitLength = other.BitLength;
		var eDiff = bitLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return eComparison;
		return (MantissaOverflow + m << eDiff - 1).CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see cref="MpzT"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(MpzT other)
	{
		if (Mpir.MpzCmpSi(other, 0) < 0)
			return 1;
		if (e is null)
			return m.CompareTo(other);
		var bitLength = other.BitLength;
		var eDiff = bitLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return eComparison;
		return (MantissaOverflow + m << eDiff - 1).CompareTo(other);
	}

	public int CompareTo(UnsignedLongReal? other) => (int)Compute(this, other!, ComputeOperation.Compare).m - 1;

	public int CompareTo(object? obj) => obj switch
	{
		null => 1,
		byte y => CompareTo(y),
		short si => CompareTo(si),
		ushort usi => CompareTo(usi),
		int i => CompareTo(i),
		uint ui => CompareTo(ui),
		long li => CompareTo(li),
		ulong uli => CompareTo(uli),
		MpzT z => CompareTo(z),
		MpuT uz => CompareTo(uz),
		UnsignedLongReal ulr => CompareTo(ulr),
		BigInteger bi => CompareTo(new MpzT(bi)),
		IComparable ic => -ic.CompareTo(this),
		_ => 0,
	};

	private static UnsignedLongReal Compute(UnsignedLongReal x, UnsignedLongReal y, ComputeOperation operation)
	{
		switch (operation)
		{
			case ComputeOperation.BitLength:
			return x.e is null ? x.m.BitLength : Compute(x.e, x.MantissaLength, ComputeOperation.Add);
			case ComputeOperation.Compare:
			if (y is null)
				return new(MpuT.Two, null);
			if (x.e is null && y.e is null)
				return new(Math.Sign(x.m.CompareTo(y.m)) + MpuT.One, null);
			if (x.e is null
				&& (x.MantissaLength <= y.MantissaLength || y.e!.e is not null || Mpir.MpuCmpSi(y.e.m, int.MaxValue) > 1))
				return new(MpuT.Zero, null);
			if (y.e is null
				&& (x.MantissaLength >= y.MantissaLength || x.e!.e is not null || Mpir.MpuCmpSi(x.e.m, int.MaxValue) > 1))
				return new(MpuT.Two, null);
			var xBitLength = Compute(x, null!, ComputeOperation.BitLength);
			var yBitLength = Compute(y, null!, ComputeOperation.BitLength);
			var compared = Compute(xBitLength, yBitLength, ComputeOperation.Compare).m;
			if (Mpir.MpuCmpSi(compared, 1) != 0)
				return new(compared, null);
			if (x.e is null)
				return new(Math.Sign(x.m.CompareTo(y.MantissaOverflow + y.m << (int)y.e! - 1)) + MpuT.One, null);
			else if (y.e is null)
				return new(Math.Sign((x.MantissaOverflow + x.m << (int)x.e! - 1).CompareTo(y.m)) + MpuT.One, null);
			var mlDiff = x.MantissaLength - y.MantissaLength;
			if (mlDiff >= 0)
				return new(Math.Sign(x.m.CompareTo(y.m << mlDiff)) + MpuT.One, null);
			else
				return new(Math.Sign((x.m << -mlDiff).CompareTo(y.m)) + MpuT.One, null);
			case ComputeOperation.ChangeML:
			var mantissaLength = (int)y.m >>> 1;
			if (mantissaLength == x.MantissaLength)
				return x;
			mlDiff = mantissaLength - x.MantissaLength;
			var xMantissaOverfow = x.MantissaOverflow;
			if (mlDiff > 0)
			{
				if (x.e is null)
					return new(x.m, mantissaLength);
				else if (Mpir.MpuCmpSi(Compute(x.e, mlDiff, ComputeOperation.Compare).m, 1) <= 0)
					return new(xMantissaOverfow + x.m << (int)x.e - 1, mantissaLength);
				else
					return new(x.m << mlDiff, Compute(x.e, mlDiff, ComputeOperation.Subtract), mantissaLength);
			}
			else
			{
				mlDiff = -mlDiff;
				if (x.e is null)
					return new(x.m, mantissaLength);
				else
					return new(x.m.ShiftRightRound(mlDiff), Compute(x, mlDiff, ComputeOperation.Add), mantissaLength);
			}
			case ComputeOperation.Add:
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			if (x.e is null && Mpir.MpuCmpSi(x.m, 0) == 0)
				return Compute(y, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			if (y.e is null && Mpir.MpuCmpSi(y.m, 0) == 0)
				return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			if (Mpir.MpuCmpSi(Compute(y, x, ComputeOperation.Compare).m, 1) > 0)
				(x, y) = (y, x);
			var mantissaOverflow = MpuT.One << mantissaLength;
			var mantissaMask = mantissaOverflow - 1;
			var xmlDiff = mantissaLength - x.MantissaLength;
			var ymlDiff = mantissaLength - y.MantissaLength;
			xBitLength = Compute(x, null!, ComputeOperation.BitLength);
			yBitLength = Compute(y, null!, ComputeOperation.BitLength);
			var yMantissaOverflow = y.MantissaOverflow;
			UnsignedLongReal newE;
			if (x.e is null || Mpir.MpuCmpSi(Compute(xBitLength, mantissaLength, ComputeOperation.Compare).m, 1) <= 0
				&& Mpir.MpuCmpSi(Compute(yBitLength, mantissaLength, ComputeOperation.Compare).m, 1) <= 0)
			{
				var mSum = (MpuT)x + (MpuT)y;
				if (Mpir.MpuCmp(mSum, mantissaOverflow) >= 0)
					return new(mSum & mantissaMask, 1, mantissaLength);
				return new(mSum, null, mantissaLength);
			}
			else if (y.e is null || Mpir.MpuCmpSi(Compute(yBitLength, mantissaLength, ComputeOperation.Compare).m, 1) <= 0)
			{
				if (xBitLength.e is not null)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var blDiff = Compute(xBitLength, Math.Max(y.MantissaLength + 1, (int)yBitLength), ComputeOperation.Subtract);
				if (Mpir.MpuCmpSi(Compute(blDiff, mantissaLength, ComputeOperation.Compare).m, 1) > 0)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var ym = (y.e is null ? MpuT.Zero : yMantissaOverflow) + y.m;
				var mSum = (x.m << xmlDiff) + (ym << ymlDiff).ShiftRightRound((int)blDiff);
				if (Mpir.MpuCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = Compute(xBitLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mSum & mantissaMask).ShiftRightRound(1), newE, mantissaLength);
				}
				newE = Compute(x.e, xmlDiff, ComputeOperation.Subtract);
				newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
				return new(mSum, newE, mantissaLength);
			}
			else if (Mpir.MpuCmpSi(Compute(x.e, y.e, ComputeOperation.Compare).m, 1) >= 0)
			{
				var blDiff = Compute(xBitLength, yBitLength, ComputeOperation.Subtract);
				if (blDiff.e is null && blDiff.m == 0)
				{
					newE = Compute(Compute(x.e, One, ComputeOperation.Add), xmlDiff, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new(((x.m << xmlDiff) + y.m).ShiftRightRound(1), newE, mantissaLength);
				}
				if (Mpir.MpuCmpSi(Compute(blDiff, mantissaLength, ComputeOperation.Compare).m, 1) > 0)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var mSum = (x.m << xmlDiff) + (yMantissaOverflow + y.m << ymlDiff).ShiftRightRound((int)blDiff);
				if (Mpir.MpuCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = Compute(xBitLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mSum & mantissaMask).ShiftRightRound(1), newE, mantissaLength);
				}
				newE = Compute(x.e, xmlDiff, ComputeOperation.Subtract);
				newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
				return new(mSum, newE, mantissaLength);
			}
			else
			{
				var blDiff = Compute(xBitLength, yBitLength, ComputeOperation.Subtract);
				if (blDiff.e is null && blDiff.m == 0)
				{
					newE = Compute(y.e, ymlDiff - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((x.m + (y.m << ymlDiff)).ShiftRightRound(1), newE, mantissaLength);
				}
				var eDiff = Compute(y.e, x.e, ComputeOperation.Subtract);
				var mSum = x.m + (yMantissaOverflow + y.m << ((int)eDiff));
				if (Mpir.MpuCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = Compute(xBitLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mSum & mantissaMask).ShiftRightRound(1), newE, mantissaLength);
				}
				return new(mSum, Compute(x.e, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML), mantissaLength);
			}
			case ComputeOperation.Subtract:
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			if (y.e is null && Mpir.MpuCmpSi(y.m, 0) == 0)
				return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			mantissaOverflow = MpuT.One << mantissaLength;
			mantissaMask = mantissaOverflow - 1;
			x = Compute(x, (long)mantissaLength << 1, ComputeOperation.ChangeML);
			y = Compute(y, (long)mantissaLength << 1, ComputeOperation.ChangeML);
			if (x.e is null && y.e is null)
				return new(x.m - y.m, null, mantissaLength);
			else if (y.e is null)
			{
				Debug.Assert(x.e is not null);
				if (Mpir.MpuCmpSi(Compute(x.e, mantissaLength + 1, ComputeOperation.Compare).m, 1) >= 0)
					return x;
				var mDiff = mantissaOverflow + x.m - y.m.ShiftRightRound((int)x.e - 1);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff & mantissaMask, x.e, mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new((mDiff << 1) & mantissaMask, (int)x.e - 1, mantissaLength);
			}
			else if (x.e is null || Mpir.MpuCmpSi(Compute(x.e, y.e, ComputeOperation.Compare).m, 1) < 0)
				throw new OverflowException(NoNegativeNumbers);
			else if (Mpir.MpuCmpSi(Compute(x.e, Compute(y.e, One, ComputeOperation.Add), ComputeOperation.Compare).m, 1) > 0)
			{
				var eDiff = Compute(x.e, y.e, ComputeOperation.Subtract);
				if (Mpir.MpuCmpSi(Compute(eDiff, mantissaLength, ComputeOperation.Compare).m, 1) > 0)
					return x;
				var mDiff = mantissaOverflow + x.m - (mantissaOverflow + y.m).ShiftRightRound((int)eDiff);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff & mantissaMask, x.e, mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new((mDiff << 1) & mantissaMask, Compute(x.e, One, ComputeOperation.Subtract), mantissaLength);
			}
			else if (Compute(x.e, y.e, ComputeOperation.Compare).m == 1)
			{
				var mDiff = x.m - y.m;
				if (mDiff == 0)
					return new(MpuT.Zero, null, mantissaLength);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (Mpir.MpuCmpSi(Compute(x.e, shiftAmount, ComputeOperation.Compare).m, 1) <= 0)
					return new(mDiff << (int)x.e - 1, null, mantissaLength);
				return new((mDiff << shiftAmount) & mantissaMask,
					Compute(x.e, shiftAmount, ComputeOperation.Subtract), mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + x.m << 1) - (mantissaOverflow + y.m);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (shiftAmount == -1)
					return new(mDiff.ShiftRightRound(1) & mantissaMask, x.e, mantissaLength);
				return new((mDiff << shiftAmount) & mantissaMask,
					Compute(x.e, shiftAmount + 1, ComputeOperation.Subtract), mantissaLength);
			}
			default:
			return Zero;
		}
	}

	public void Dispose()
	{
		e?.Dispose();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Возвращает частное и остаток от деления данного числа на указанное число типа <see cref="MpuT"/>.
	/// </summary>
	/// <param name="x">Делитель.</param>
	/// <returns>Кортеж, содержащий частное в первом элементе и остаток во втором.</returns>
	public (UnsignedLongReal Quotient, MpuT Remainder) DivRem(MpuT x)
	{
		if (e is null)
		{
			var result = m.Divide(x, out var remainder);
			return (new(result, null, MantissaLength), remainder);
		}
		else if (x.BitLength < MantissaLength)
		{
			Debug.Assert(e is not null);
			var mantissaOverflow = MantissaOverflow;
			if (Mpir.MpuCmpSi(x, 0) == 0)
				throw new DivideByZeroException(NoDivisionByZero);
			else if (Mpir.MpuCmpSi(x, 1) == 0)
				return (this, MpuT.Zero);
			else if (e <= x.BitLength + 1)
				return (new((mantissaOverflow + m << (int)e - 1).Divide(x, out var remainder), null,
					MantissaLength), remainder);
			var quotient = (mantissaOverflow + m << MantissaLength + 1) / x;
			var shiftAmount = quotient.BitLength - MantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, e + shiftAmount - MantissaLength - 1,
				MantissaLength), MpuT.Zero);
		}
		else if (e is null || e < x.BitLength - MantissaLength - 1)
			return (new(0, MantissaLength), (MpuT)this);
		else if (e <= x.BitLength + 1)
			return (new((MantissaOverflow + m << (int)e - 1).Divide(x, out var remainder), null,
				MantissaLength), remainder);
		else
		{
			var quotient = (MantissaOverflow + m << ((int)e)) / (x << 1);
			var shiftAmount = quotient.BitLength - MantissaLength;
			return (new(quotient.ShiftRightRound(shiftAmount - 1) & MantissaMask, shiftAmount, MantissaLength), MpuT.Zero);
		}
	}

	/// <summary>
	/// Возвращает частное и остаток от деления данного числа на указанное число типа <see cref="UnsignedLongReal"/>.
	/// </summary>
	/// <param name="x">Делитель.</param>
	/// <returns>Кортеж, содержащий частное в первом элементе и остаток во втором.</returns>
	public (UnsignedLongReal Quotient, UnsignedLongReal Remainder) DivRem(UnsignedLongReal x)
	{
		var maxMantissaLength = Math.Max(MantissaLength, x.MantissaLength);
		var mantissaOverflow = MantissaOverflows.GetOrAdd(maxMantissaLength, x => MpuT.One << x);
		var mantissaMask = mantissaOverflow - 1;
		var this2 = GetWithOtherML(maxMantissaLength, false);
		x = x.GetWithOtherML(maxMantissaLength, false);
		if (this2.e is null && x.e is null)
		{
			var result = this2.m.Divide(x.m, out var remainder);
			return (new(result, null, maxMantissaLength), new(remainder, maxMantissaLength));
		}
		else if (x.e is null)
		{
			Debug.Assert(this2.e is not null);
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				throw new DivideByZeroException(NoDivisionByZero);
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return (this2, new(0, maxMantissaLength));
			else if (this2.e <= x.m.BitLength)
				return (new(((mantissaOverflow + this2.m) << (int)this2.e - 1).Divide(x.m, out var remainder),
					maxMantissaLength), new(remainder, maxMantissaLength));
			var quotient = (mantissaOverflow + this2.m << maxMantissaLength + 1) / x.m;
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & mantissaMask, this2.e + shiftAmount - maxMantissaLength - 1,
				maxMantissaLength), new(0, maxMantissaLength));
		}
		else if (this2.e is null || this2.e < x.e)
			return (new(0, maxMantissaLength), this2);
		else if (this2.e <= x.e + maxMantissaLength)
		{
			var eDiff = (int)(this2.e - x.e);
			var quotient = ((mantissaOverflow + this2.m) << eDiff).Divide(mantissaOverflow + x.m, out var remainder);
			return (new(quotient, maxMantissaLength), new(remainder << (int)x.e - 1, maxMantissaLength));
		}
		else
		{
			var quotient = (mantissaOverflow + this2.m << maxMantissaLength + 1) / (mantissaOverflow + x.m);
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & mantissaMask, this2.e - x.e + shiftAmount - maxMantissaLength,
				maxMantissaLength), new(0, maxMantissaLength));
		}
	}

	/// <summary>
	/// Возвращает частное и остаток от деления данного числа на указанное число типа <see cref="MpuT"/>.
	/// </summary>
	/// <param name="x">Делитель.</param>
	/// <param name="remainder"><see langword="out"/>-параметр, в который помещается остаток от деления.</param>
	/// <returns>Эта перегрузка метода возвращает только частное,
	/// помещая остаток в <see langword="out"/>-параметр <paramref name="remainder"/>.</returns>
	public UnsignedLongReal DivRem(MpuT x, out MpuT remainder)
	{
		(var Quotient, remainder) = DivRem(x);
		return Quotient;
	}

	/// <summary>
	/// Возвращает частное и остаток от деления данного числа на указанное число типа <see cref="UnsignedLongReal"/>.
	/// </summary>
	/// <param name="x">Делитель.</param>
	/// <param name="remainder"><see langword="out"/>-параметр, в который помещается остаток от деления.</param>
	/// <returns>Эта перегрузка метода возвращает только частное,
	/// помещая остаток в <see langword="out"/>-параметр <paramref name="remainder"/>.</returns>
	public UnsignedLongReal DivRem(UnsignedLongReal x, out UnsignedLongReal remainder)
	{
		(var Quotient, remainder) = DivRem(x);
		return Quotient;
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="int"/>.
	/// См. описание <see cref="Equals(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(int other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="uint"/>.
	/// См. описание <see cref="Equals(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(uint other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="long"/>.
	/// См. описание <see cref="Equals(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(long other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="ulong"/>.
	/// См. описание <see cref="Equals(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(ulong other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see cref="MpuT"/>.
	/// См. описание <see cref="Equals(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(MpuT other)
	{
		if (e is null)
			return m.Equals(other);
		var bitLength = other.BitLength;
		var eDiff = bitLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return false;
		return (MantissaOverflow + m << eDiff - 1).Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see cref="MpzT"/>.
	/// См. описание <see cref="Equals(UnsignedLongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(MpzT other)
	{
		if (e is null)
			return m.Equals(other);
		var bitLength = other.BitLength;
		var eDiff = bitLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return false;
		return (MantissaOverflow + m << eDiff - 1).Equals(other);
	}

	public bool Equals(UnsignedLongReal? other) => CompareTo(other) == 0;

	public override bool Equals(object? obj) => obj switch
	{
		null => false,
		byte y => CompareTo(y) == 0,
		short si => CompareTo(si) == 0,
		ushort usi => CompareTo(usi) == 0,
		int i => CompareTo(i) == 0,
		uint ui => CompareTo(ui) == 0,
		long li => CompareTo(li) == 0,
		ulong uli => CompareTo(uli) == 0,
		MpzT z => CompareTo(z) == 0,
		MpuT uz => CompareTo(uz) == 0,
		UnsignedLongReal ulr => CompareTo(ulr) == 0,
		BigInteger bi => CompareTo(new MpzT(bi)) == 0,
		IConvertible ic => ic.Equals(this),
		_ => false,
	};

	public int GetByteCount() => GetByteCount(true);
	/// <summary>Возвращает количество байт, необходимое для сохранения числа вместе с длиной мантиссы или без нее.</summary>
	public int GetByteCount(bool saveMantissaLength) =>
		(e is null ? m.GetByteCount() : MantissaByteLength + e.GetByteCount(false)) + (saveMantissaLength ? sizeof(int) : 0);
	public int GetExponentByteCount() => e is null ? 0 : e.GetByteCount();
	public int GetExponentShortestBitLength() => e is null ? 0 : e.GetShortestBitLength();

	public override int GetHashCode()
	{
		var hash = 486187739;
		hash = (hash * 16777619) ^ m.GetHashCode();
		if (e is null)
			return hash;
		return (hash * 16777619) ^ e.GetHashCode();
	}

	public int GetShortestBitLength() => e is null ? m.GetShortestBitLength() : (int)(e + MantissaLength);
	public int GetSignificandBitLength() => m.GetShortestBitLength();
	public int GetSignificandByteCount() => m.GetByteCount();
	TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

	internal UnsignedLongReal GetWithOtherML(int mantissaLength, bool copy) =>
		Compute(this, new(new((ulong)mantissaLength << 1 | (copy ? 1u : 0)), null), ComputeOperation.ChangeML);

	public static bool IsCanonical(UnsignedLongReal value) => true;
	public static bool IsComplexNumber(UnsignedLongReal value) => true;
	/// <summary>Проверяет, является ли данное число четным (возвращает true или false).</summary>
	public bool IsEven() => e is not null || (m & 1) == 0;
	public static bool IsEvenInteger(UnsignedLongReal value) => value.IsEven();
	public static bool IsFinite(UnsignedLongReal value) => true;
	public static bool IsImaginaryNumber(UnsignedLongReal value) => false;
	public static bool IsInfinity(UnsignedLongReal value) => false;
	public static bool IsInteger(UnsignedLongReal value) => true;
	public static bool IsNaN(UnsignedLongReal value) => false;
	public static bool IsNegative(UnsignedLongReal value) => false;
	public static bool IsNegativeInfinity(UnsignedLongReal value) => false;
	public static bool IsNormal(UnsignedLongReal value) => value.e is not null;
	public static bool IsOddInteger(UnsignedLongReal value) => !IsEvenInteger(value);
	public static bool IsPositive(UnsignedLongReal value) => true;
	public static bool IsPositiveInfinity(UnsignedLongReal value) => false;
	public static bool IsPow2(UnsignedLongReal value) => value.PopCount() == 1;
	public static bool IsRealNumber(UnsignedLongReal value) => true;
	public static bool IsSubnormal(UnsignedLongReal value) => value.e is null;
	public static bool IsZero(UnsignedLongReal value) => Mpir.MpuCmpSi(value.m, 0) == 0 && value.e is null;

	public static UnsignedLongReal Log2(UnsignedLongReal value)
	{
		var bitLength = value.BitLength;
		var sqrt = (new UnsignedLongReal(1, value.MantissaByteLength) << bitLength << bitLength - 1).Sqrt();
		return value >= sqrt ? bitLength : bitLength - 1;
	}

	public static UnsignedLongReal Max(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongReal MaxMagnitude(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongReal MaxMagnitudeNumber(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongReal Min(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) < 0 ? x : y;
	public static UnsignedLongReal MinMagnitude(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) < 0 ? x : y;
	public static UnsignedLongReal MinMagnitudeNumber(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) < 0 ? x : y;

	public static UnsignedLongReal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => new(s.ToString());
	public static UnsignedLongReal Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		new(s.ToString());

	/// <summary>
	/// Читает число из указанной строки, выбрасывая <see cref="FormatException"/> в случае неудачи.
	/// </summary>
	/// <param name="s">Строка для чтения из нее числа.</param>
	/// <returns>Прочитанное число.</returns>
	public static UnsignedLongReal Parse(string? s) => new(s);

	public static UnsignedLongReal Parse(string s, IFormatProvider? provider) => new(s);
	public static UnsignedLongReal Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);
	/// <summary>Возвращает количество единиц в двоичном представлении данного числа.</summary>
	public int PopCount() => m.PopCount() + (e is null ? 0 : 1);
	public static UnsignedLongReal PopCount(UnsignedLongReal value) => value.PopCount();
	static UnsignedLongReal IFloatingPoint<UnsignedLongReal>.Round(UnsignedLongReal x, int digits, MidpointRounding mode) => x;

	/// <summary>Возвращает квадратный корень данного числа (округленный вниз).</summary>
	public UnsignedLongReal Sqrt()
	{
		if (e is null)
			return new(m.Sqrt());
		else if (e.IsEven())
			return new((MantissaOverflow + m).Sqrt() << MantissaLength / 2 & MantissaMask, e >> 1);
		else
			return new((MantissaOverflow + m << MantissaLength + 1).Sqrt() & MantissaMask, e >> 1);
	}

	bool IConvertible.ToBoolean(IFormatProvider? provider) => CompareTo(1) >= 0;
	byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

	/// <summary>
	/// Преобразует данное число в массив байт.
	/// </summary>
	/// <param name="order">Порядок записи: &lt; 0 - Little Endian, &gt; 0 - Big Endian.</param>
	/// <param name="saveMantissaLength">Нужно ли записывать длину мантиссы:
	/// если да, то увеличивает длину результата на <see langword="sizeof(int)"/>.</param>
	/// <returns>Массив байт, из которого можно восстановить данное число,
	/// с явным указанием длины мантиссы или без такового.</returns>
	public byte[] ToByteArray(int order, bool saveMantissaLength = true)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(GetByteCount(saveMantissaLength));
		if (order < 0 && TryWriteLittleEndian(bytes, out var bytesWritten, saveMantissaLength) && bytesWritten == bytes.Length)
			return bytes;
		else if (order > 0 && TryWriteBigEndian(bytes, out bytesWritten, saveMantissaLength) && bytesWritten == bytes.Length)
			return bytes;
		else
			throw new InvalidOperationException("Ошибка, не удалось преобразовать в массив байт.");
	}

	char IConvertible.ToChar(IFormatProvider? provider) => (char)(uint)this;
	DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();
	decimal IConvertible.ToDecimal(IFormatProvider? provider) => (decimal)this;
	double IConvertible.ToDouble(IFormatProvider? provider) => (double)this;
	short IConvertible.ToInt16(IFormatProvider? provider) => (short)this;
	int IConvertible.ToInt32(IFormatProvider? provider) => (int)this;
	long IConvertible.ToInt64(IFormatProvider? provider) => (long)this;
	sbyte IConvertible.ToSByte(IFormatProvider? provider) => (sbyte)(short)this;
	float IConvertible.ToSingle(IFormatProvider? provider) => (float)this;

	/// <summary>
	/// Преобразует данное число в строку, только если его длина меньше заданного порога
	/// (65536 бит для данного типа).
	/// </summary>
	/// <returns>Результат преобразования в строку или строка-заглушка, если число слишком большое.</returns>
	public string? ToShortString()
	{
		if (m is null || m.val == 0)
			return "0";
		else if (BitLength >= 65536)
			return (string?)"Too large for short string, use ToString() instead.";
		else
			return ((MpuT)this).ToString();
	}

	public override string? ToString() => ((MpuT)this).ToString(DefaultStringBase);
	public string ToString(IFormatProvider? provider) => ToString(DefaultStringBase) ?? "";
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString(DefaultStringBase));

	/// <summary>
	/// Возвращает строковую запись числа в указанной системе счисления.
	/// </summary>
	/// <param name="base">Система счисления, в которой нужно получить строковую запись числа (от 2 до 36).</param>
	/// <returns>См. общее описание.</returns>
	public string? ToString(uint @base) => ((MpuT)this).ToString(@base);

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	public static UnsignedLongReal TrailingZeroCount(UnsignedLongReal value) =>
		MpuT.TrailingZeroCount(value.m) + (value.e is null ? 0 : (value.e - 1));

	private static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = value switch
			{
				UnsignedLongReal ulr => ulr,
				MpzT z => (UnsignedLongReal)z,
				MpuT uz => uz,
				byte y => y,
				sbyte sy => sy,
				short si => si,
				ushort usi => usi,
				int i => i,
				uint ui => ui,
				long li => li,
				ulong uli => uli,
				float f => (UnsignedLongReal)f,
				double d => (UnsignedLongReal)d,
				decimal m => (UnsignedLongReal)(double)m,
				BigInteger ll => new(ll),
				string s => new(s),
				_ => throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(UnsignedLongReal)
				+ ", " + nameof(MpzT) + ", " + nameof(MpuT)
				+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, string."),
			};
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}

	static bool INumberBase<UnsignedLongReal>.TryConvertFromChecked<TOther>(TOther value,
		[MaybeNullWhen(false)] out UnsignedLongReal result) =>
		TryConvertFromChecked(value, out result);

	static bool INumberBase<UnsignedLongReal>.TryConvertFromSaturating<TOther>(TOther value,
		[MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = value switch
			{
				UnsignedLongReal ulr => ulr,
				MpzT z => (UnsignedLongReal)z,
				MpuT uz => uz,
				byte y => y,
				sbyte sy => sy,
				short si => si,
				ushort usi => usi,
				int i => i,
				uint ui => ui,
				long li => li,
				ulong uli => uli,
				float f => (MpuT)MathF.Ceiling(MathF.Abs(f)) * MathF.Sign(f),
				double d => (MpuT)Math.Ceiling(Math.Abs(d)) * Math.Sign(d),
				string s => new(s),
				_ => throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(MpzT)
				+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, string."),
			};
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}

	static bool INumberBase<UnsignedLongReal>.TryConvertFromTruncating<TOther>(TOther value,
		[MaybeNullWhen(false)] out UnsignedLongReal result) =>
		TryConvertFromChecked(value, out result);

	private static bool TryConvertToChecked<TOther>(UnsignedLongReal value, out TOther result)
	{
		try
		{
			result = (TOther)((IConvertible)value).ToType(typeof(TOther), new CultureInfo("en-US"));
			return true;
		}
		catch
		{
			result = default!;
			return false;
		}
	}

	static bool INumberBase<UnsignedLongReal>.TryConvertToChecked<TOther>(UnsignedLongReal value, out TOther result) =>
		TryConvertToChecked(value, out result);

	static bool INumberBase<UnsignedLongReal>.TryConvertToSaturating<TOther>(UnsignedLongReal value, out TOther result) =>
		TryConvertToChecked(value, out result);

	static bool INumberBase<UnsignedLongReal>.TryConvertToTruncating<TOther>(UnsignedLongReal value, out TOther result) =>
		TryConvertToChecked(value, out result);

	bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten,
		ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		try
		{
			var s = ToString("{0:N0}", provider);
			for (var i = 0; i < s.Length; i++)
				destination[i] = s[i];
			charsWritten = s.Length;
			return true;
		}
		catch
		{
			charsWritten = 0;
			return false;
		}
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = Parse(s, style, provider);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	/// <summary>
	/// Пробует прочитать число из указанной строки, возвращая <see langword="true"/> в случае успеха
	/// и <see langword="false"/> в случае неудачи.
	/// </summary>
	/// <param name="s">Строка для чтения из нее числа.</param>
	/// <param name="result">Прочитанное число, или пустое значение (ноль, null и т. д.) в случае неудачи.</param>
	/// <returns>Булево значение, отражающее, удалось ли прочитать число.</returns>
	public static bool TryParse(string? s, [MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = Parse(s);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	public static bool TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = Parse(s ?? "", provider);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out UnsignedLongReal result)
	{
		try
		{
			result = Parse(s ?? "", style, provider);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UnsignedLongReal value)
	{
		value = new(source, 1);
		return true;
	}

	public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UnsignedLongReal value)
	{
		value = new(source, -1);
		return true;
	}

	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteBigEndian(destination, out bytesWritten, true);

	/// <inheritdoc cref="TryWriteBigEndian(Span{byte}, out int)"/>
	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength) =>
		((IUnsignedLongReal<UnsignedLongReal>)this)
		.TryWriteBigEndianInterface(destination, out bytesWritten, saveMantissaLength);

	public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? 0 : e).TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? 0 : e).TryWriteLittleEndian(destination, out bytesWritten);

	/// <inheritdoc cref="TryWriteLittleEndian(Span{byte}, out int)"/>
	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteLittleEndian(destination, out bytesWritten, true);

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength) =>
		((IUnsignedLongReal<UnsignedLongReal>)this)
		.TryWriteLittleEndianInterface(destination, out bytesWritten, saveMantissaLength);

	public bool TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) =>
		m.TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) =>
		m.TryWriteLittleEndian(destination, out bytesWritten);

	public static implicit operator UnsignedLongReal(byte value) => new((uint)value);
	public static implicit operator UnsignedLongReal(short value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongReal(ushort value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongReal(int value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongReal(uint value) => new(value);
	public static implicit operator UnsignedLongReal(long value) => new(value);
	public static implicit operator UnsignedLongReal(ulong value) => new(value);
	public static explicit operator UnsignedLongReal(MpzT value) => new(value);
	public static implicit operator UnsignedLongReal(MpuT value) => new(value);
	public static explicit operator UnsignedLongReal(float value) => new((double)value);
	public static explicit operator UnsignedLongReal(double value) => new(value);
	public static explicit operator UnsignedLongReal(decimal value) => new(value);
	public static explicit operator UnsignedLongReal(string value) => new(value, DefaultStringBase);
	public static explicit operator byte(UnsignedLongReal value) => (byte)(uint)value;
	public static explicit operator short(UnsignedLongReal value) => (short)(int)value;
	public static explicit operator ushort(UnsignedLongReal value) => (ushort)(uint)value;
	public static explicit operator int(UnsignedLongReal value) => (int)(uint)value;
	public static explicit operator uint(UnsignedLongReal value) => value & uint.MaxValue;
	public static explicit operator long(UnsignedLongReal value) => (long)(value & ulong.MaxValue).m;
	public static explicit operator ulong(UnsignedLongReal value) => (ulong)(value & ulong.MaxValue).m;
	public static explicit operator float(UnsignedLongReal value) => (float)(double)value;

	public static explicit operator double(UnsignedLongReal value)
	{
		if (value.BitLength > 1024)
			return double.PositiveInfinity;
		else if (value.e is null)
			return (double)value.m;
		return (double)(value.MantissaOverflow + value.m << (int)value.e - 1);
	}

	public static explicit operator decimal(UnsignedLongReal value)
	{
		if ((double)value is var x && x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN))
			return (decimal)x;
		return 0m;
	}

	public static explicit operator string?(UnsignedLongReal value) => value.ToString();

	public static explicit operator MpzT(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return new MpzT(value.MantissaOverflow + value.m) << (int)value.e - 1;
		else
			return 0;
	}

	public static explicit operator MpuT(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return value.MantissaOverflow + value.m << (int)value.e - 1;
		else
			return MpuT.Zero;
	}

	public static UnsignedLongReal operator +(UnsignedLongReal value) => new(value);
	/// <inheritdoc cref="IUnaryNegationOperators{UnsignedLongReal, UnsignedLongReal}.operator -(UnsignedLongReal-)"/>
	public static LongReal operator -(UnsignedLongReal value) => -new LongReal(value, value.MantissaLength);
	static UnsignedLongReal IUnaryNegationOperators<UnsignedLongReal, UnsignedLongReal>.operator -(UnsignedLongReal value) =>
		throw new NotSupportedException(NoNegativeNumbers);
	/// <inheritdoc cref="IBitwiseOperators{UnsignedLongReal, UnsignedLongReal, UnsignedLongReal}.operator ~(UnsignedLongReal)"/>
	public static LongReal operator ~(UnsignedLongReal value) => ~new LongReal(value, value.MantissaLength);
	static UnsignedLongReal IBitwiseOperators<UnsignedLongReal, UnsignedLongReal, UnsignedLongReal>.operator ~(UnsignedLongReal value) =>
		throw new NotSupportedException(NoNegativeNumbers);

	/// <inheritdoc cref="operator +(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator +(int x, UnsignedLongReal y) => y + x;
	/// <inheritdoc cref="operator +(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator +(UnsignedLongReal x, int y) =>
		y < 0 ? x - new UnsignedLongReal(-y) : x + new UnsignedLongReal(y);
	public static UnsignedLongReal operator +(UnsignedLongReal x, UnsignedLongReal y) =>
		Compute(x, y, ComputeOperation.Add);
	/// <inheritdoc cref="operator -(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator -(UnsignedLongReal x, int y) =>
		y < 0 ? x + new UnsignedLongReal(-y) : x - new UnsignedLongReal(y);
	public static UnsignedLongReal operator -(UnsignedLongReal x, UnsignedLongReal y) =>
		Compute(x, y, ComputeOperation.Subtract);
	/// <inheritdoc cref="operator *(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator *(int x, UnsignedLongReal y) => y * x;
	/// <inheritdoc cref="operator *(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator *(uint x, UnsignedLongReal y) => y * x;

	/// <inheritdoc cref="operator *(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator *(UnsignedLongReal x, int y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e is null)
			return new(x.m * y, MantissaLength);
		else
		{
			if (y == 0)
				return new(0, MantissaLength);
			else if (y == 1)
				return x;
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount, MantissaLength);
		}
	}

	/// <inheritdoc cref="operator *(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator *(UnsignedLongReal x, uint y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e is null)
			return new(x.m * y, MantissaLength);
		else
		{
			if (y == 0)
				return new(0, MantissaLength);
			else if (y == 1)
				return x;
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount, MantissaLength);
		}
	}

	public static UnsignedLongReal operator *(UnsignedLongReal x, UnsignedLongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m * y.m, maxMantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				return new(0, maxMantissaLength);
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x;
			var product = (MantissaOverflow + x.m) * y.m;
			var shiftAmount = product.BitLength - maxMantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount, maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				return new(0, maxMantissaLength);
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return y;
			var product = x.m * (MantissaOverflow + y.m);
			var shiftAmount = product.BitLength - maxMantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, y.e + shiftAmount, maxMantissaLength);
		}
		else
		{
			var product = (MantissaOverflow + x.m) * (MantissaOverflow + y.m);
			var shiftAmount = product.BitLength - maxMantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + y.e + shiftAmount - 1, maxMantissaLength);
		}
	}

	/// <inheritdoc cref="operator /(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator /(UnsignedLongReal x, int y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e is null)
			return new(x.m / y, null, MantissaLength);
		else if (y == 0)
			throw new DivideByZeroException(NoDivisionByZero);
		else if (y == 1)
			return x;
		else if (x.e <= BitsPerInt - int.LeadingZeroCount(y))
			return new(((MantissaOverflow + x.m) << (int)x.e - 1) / y, MantissaLength);
		var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y;
		var shiftAmount = quotient.BitLength - MantissaLength - 1;
		return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1,
			MantissaLength);
	}

	/// <inheritdoc cref="operator /(UnsignedLongReal, UnsignedLongReal)"/>
	public static UnsignedLongReal operator /(UnsignedLongReal x, uint y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e is null)
			return new(x.m / y, null, MantissaLength);
		else if (y == 0)
			throw new DivideByZeroException(NoDivisionByZero);
		else if (y == 1)
			return x;
		else if (x.e <= BitsPerInt - uint.LeadingZeroCount(y))
			return new(((MantissaOverflow + x.m) << (int)x.e - 1) / y, MantissaLength);
		var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y;
		var shiftAmount = quotient.BitLength - MantissaLength - 1;
		return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1,
			MantissaLength);
	}

	public static UnsignedLongReal operator /(UnsignedLongReal x, UnsignedLongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m / y.m, null, maxMantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				throw new DivideByZeroException(NoDivisionByZero);
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x;
			else if (x.e <= y.m.BitLength)
				return new(((MantissaOverflow + x.m) << (int)x.e - 1) / y.m, maxMantissaLength);
			var quotient = (MantissaOverflow + x.m << maxMantissaLength + 1) / y.m;
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - maxMantissaLength - 1,
				maxMantissaLength);
		}
		else if (x.e is null || x.e < y.e)
			return new(0, maxMantissaLength);
		else
		{
			if (x.e <= y.e + maxMantissaLength + 1)
				return new(((MantissaOverflow + x.m) << (int)(x.e - y.e)) / (MantissaOverflow + y.m), maxMantissaLength);
			var quotient = (MantissaOverflow + x.m << maxMantissaLength + 2) / (MantissaOverflow + y.m);
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e - y.e + shiftAmount - maxMantissaLength - 1,
				maxMantissaLength);
		}
	}

	public static UnsignedLongReal operator %(UnsignedLongReal x, MpuT y) => new(x.DivRem(y).Remainder, x.MantissaLength);

	public static UnsignedLongReal operator %(UnsignedLongReal x, UnsignedLongReal y) => x.DivRem(y).Remainder;

	/// <inheritdoc cref="operator &(UnsignedLongReal, UnsignedLongReal)"/>
	public static int operator &(UnsignedLongReal x, int y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > BitsPerInt)
			return 0;
		else
			return x.MantissaOverflow + x.m << (int)(x.e & uint.MaxValue) - 1 & y;
	}

	/// <inheritdoc cref="operator &(UnsignedLongReal, UnsignedLongReal)"/>
	public static uint operator &(UnsignedLongReal x, uint y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1u) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > BitsPerInt)
			return 0;
		else
			return x.MantissaOverflow + x.m << (int)(x.e & uint.MaxValue) - 1 & y;
	}

	public static UnsignedLongReal operator &(UnsignedLongReal x, UnsignedLongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m & y.m, null, maxMantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e > y.m.BitLength)
				return new(0, maxMantissaLength);
			return new(x.m << (int)x.e - 1 & y.m, maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e > x.m.BitLength)
				return new(0, maxMantissaLength);
			return new(x.m & y.m << (int)y.e - 1, maxMantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff >= maxMantissaLength)
				return new(0, maxMantissaLength);
			var newMantissa = MantissaOverflow + x.m & (MantissaOverflow + y.m << ((int)eDiff));
			if (Mpir.MpuCmpSi(newMantissa, 0) == 0)
				return new(0, maxMantissaLength);
			var shiftAmount = (MantissaOverflow + x.m).BitLength - newMantissa.BitLength;
			if (x.e <= shiftAmount)
				return new(newMantissa << (int)x.e - 1, null, maxMantissaLength);
			return new(newMantissa << shiftAmount & MantissaMask, x.e - shiftAmount, maxMantissaLength);
		}
	}

	public static UnsignedLongReal operator |(UnsignedLongReal x, UnsignedLongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m | y.m, null, maxMantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= maxMantissaLength + 1)
				return x;
			return new(x.m | y.m.ShiftRightRound((int)x.e - 1), x.e, maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= maxMantissaLength + 1)
				return y;
			return new(x.m.ShiftRightRound((int)y.e - 1) | y.m, y.e, maxMantissaLength);
		}
		else if (x.e == y.e)
			return new(x.m | y.m, x.e, maxMantissaLength);
		else if (x.e > y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff >= maxMantissaLength)
				return x;
			return new(x.m | (MantissaOverflow + y.m).ShiftRightRound((int)eDiff), x.e, maxMantissaLength);
		}
		else
		{
			var eDiff = y.e - x.e;
			if (eDiff >= maxMantissaLength)
				return y;
			return new((MantissaOverflow + x.m).ShiftRightRound((int)eDiff) | y.m, y.e, maxMantissaLength);
		}
	}

	public static UnsignedLongReal operator ^(UnsignedLongReal x, UnsignedLongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m ^ y.m, null, maxMantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= maxMantissaLength + 1)
				return x;
			return new(x.m ^ y.m.ShiftRightRound((int)x.e - 1), x.e, maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= maxMantissaLength + 1)
				return y;
			return new(x.m.ShiftRightRound((int)y.e - 1) ^ y.m, y.e, maxMantissaLength);
		}
		else if (x.e > y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff >= maxMantissaLength)
				return x;
			return new(x.m ^ (MantissaOverflow + y.m).ShiftRightRound((int)eDiff), x.e, maxMantissaLength);
		}
		else if (x.e < y.e)
		{
			var eDiff = y.e - x.e;
			if (eDiff >= maxMantissaLength)
				return y;
			return new((MantissaOverflow + x.m).ShiftRightRound((int)eDiff) ^ y.m, y.e, maxMantissaLength);
		}
		else
		{
			var mXor = x.m ^ y.m;
			var shiftAmount = maxMantissaLength + 1 - mXor.BitLength;
			if (x.e <= shiftAmount)
				return new(mXor, null, maxMantissaLength);
			else
				return new((mXor << shiftAmount) & MantissaMask, x.e - shiftAmount, maxMantissaLength);
		}
	}

	public static UnsignedLongReal operator <<(UnsignedLongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x;
		else if (x.e is null)
			return new(x.m << shiftAmount, x.MantissaLength);
		else
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
	}

	/// <inheritdoc cref="operator {{(UnsignedLongReal, int)"/>
	public static UnsignedLongReal operator <<(UnsignedLongReal x, UnsignedLongReal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x;
		else if (x.e is not null)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (shiftAmount < x.MantissaLength)
			return new(x.m << (int)shiftAmount, x.MantissaLength);
		return new UnsignedLongReal(x.m << x.MantissaLength, x.MantissaLength)
			<< shiftAmount - x.MantissaLength;
	}

	public static UnsignedLongReal operator >>(UnsignedLongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x;
		else if (x.e is null)
			return new(x.m.ShiftRightRound(shiftAmount), null, x.MantissaLength);
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new((x.MantissaOverflow + x.m).ShiftRightRound(shiftAmount - (int)x.e + 1), null, x.MantissaLength);
	}

	/// <inheritdoc cref="operator }}(UnsignedLongReal, int)"/>
	public static UnsignedLongReal operator >>(UnsignedLongReal x, UnsignedLongReal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x;
		else if (x.e is null)
		{
			if (shiftAmount > x.MantissaLength)
				return new(0, x.MantissaLength);
			return new(x.m.ShiftRightRound((int)shiftAmount), null, x.MantissaLength);
		}
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		var restShiftAmount = shiftAmount - x.e;
		if (restShiftAmount > x.MantissaLength)
			return new(0, x.MantissaLength);
		else
			return new((x.MantissaOverflow + x.m).ShiftRightRound((int)restShiftAmount + 1), null, x.MantissaLength);
	}

	public static UnsignedLongReal operator >>>(UnsignedLongReal x, int shiftAmount) => x >> shiftAmount;
	/// <inheritdoc cref="operator }}}(UnsignedLongReal, int)"/>
	public static UnsignedLongReal operator >>>(UnsignedLongReal x, UnsignedLongReal shiftAmount) => x >> shiftAmount;

	public static UnsignedLongReal operator ++(UnsignedLongReal value)
	{
#pragma warning disable IDE0078 // Используйте сопоставление шаблонов
		if (value.e is not null && value.e > 2)
			return value;
		else if (Mpir.MpuCmp(value.m, value.MantissaMask) == 0)
			return new(MpuT.Zero, value.e is not null ? 2 : 1, value.MantissaLength);
		else
			return new(value.m + 1, value.e, value.MantissaLength);
#pragma warning restore IDE0078 // Используйте сопоставление шаблонов
	}

	public static UnsignedLongReal operator --(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m - 1, null, value.MantissaLength);
		var compTo2 = Mpir.MpuCmpSi(value.e.m, 2);
		if (Mpir.MpuCmpSi(value.m, 0) == 0 && value.e.e is null && compTo2 <= 0)
			return new(value.MantissaMask, compTo2 == 0 ? 1 : null, value.MantissaLength);
		else if (value.e.e is null && compTo2 <= 0)
			return new(value.m - 1, value.e, value.MantissaLength);
		else
			return value;
	}

	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(UnsignedLongReal x, int y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(UnsignedLongReal x, int y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(UnsignedLongReal x, int y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(UnsignedLongReal x, int y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(UnsignedLongReal x, int y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(UnsignedLongReal x, int y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(UnsignedLongReal x, uint y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(UnsignedLongReal x, uint y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(UnsignedLongReal x, uint y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(UnsignedLongReal x, uint y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(UnsignedLongReal x, uint y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(UnsignedLongReal x, uint y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(UnsignedLongReal x, long y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(UnsignedLongReal x, long y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(UnsignedLongReal x, long y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(UnsignedLongReal x, long y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(UnsignedLongReal x, long y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(UnsignedLongReal x, long y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(UnsignedLongReal x, ulong y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(UnsignedLongReal x, ulong y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(UnsignedLongReal x, ulong y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(UnsignedLongReal x, ulong y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(UnsignedLongReal x, ulong y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(UnsignedLongReal x, ulong y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(UnsignedLongReal x, MpzT y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(UnsignedLongReal x, MpzT y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(UnsignedLongReal x, MpzT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(UnsignedLongReal x, MpzT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(UnsignedLongReal x, MpzT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(UnsignedLongReal x, MpzT y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(UnsignedLongReal x, MpuT y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(UnsignedLongReal x, MpuT y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(UnsignedLongReal x, MpuT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(UnsignedLongReal x, MpuT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(UnsignedLongReal x, MpuT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(UnsignedLongReal x, MpuT y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(int x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(int x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(int x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(int x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(int x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(int x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(uint x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(uint x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(uint x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(uint x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(uint x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(uint x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(long x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(long x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(long x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(long x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(long x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(long x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(ulong x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(ulong x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(ulong x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(ulong x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(ulong x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(ulong x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(MpzT x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(MpzT x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(MpzT x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(MpzT x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(MpzT x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(MpzT x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator ==(MpuT x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongReal?, UnsignedLongReal?)"/>
	public static bool operator !=(MpuT x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >=(MpuT x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <=(MpuT x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator >(MpuT x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongReal, UnsignedLongReal)"/>
	public static bool operator <(MpuT x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(UnsignedLongReal? x, UnsignedLongReal? y) => x?.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal? x, UnsignedLongReal? y) => x?.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) < 0;
}
