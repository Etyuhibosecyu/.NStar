global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using static NStar.Mpir.MpzT;

namespace NStar.Mpir;

[DebuggerDisplay("{ToShortString()}")]
public sealed class UnsignedLongReal : ICloneable, IConvertible, IComparable, IComparable<UnsignedLongReal>,
	IDisposable, IBinaryInteger<UnsignedLongReal>, IFloatingPoint<UnsignedLongReal>
{
	private enum ComputeOperation
	{
		Identity,
		BitLength,
		Compare,
		ChangeML,
		Add,
		Subtract,
	}

	private static readonly ConcurrentDictionary<int, MpuT> MantissaMasks = [], MantissaOverflows = [];
	private readonly int MantissaLength = 0;

	private readonly MpuT m;
	private readonly UnsignedLongReal? e;
	public const int AutoMantissaLength = -1, DefaultMantissaLength = 2048, MinMantissaLength = 64;

	private UnsignedLongReal(int mantissaLength = DefaultMantissaLength)
	{
		m = MpuT.Zero;
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
	}

	private UnsignedLongReal(MpuT m, UnsignedLongReal? e, int mantissaLength = DefaultMantissaLength)
		: this(mantissaLength)
	{
		this.m = m;
		this.e = e;
	}

	public UnsignedLongReal(decimal op, int mantissaLength = DefaultMantissaLength) : this(new MpuT(op), mantissaLength) { }

	public UnsignedLongReal(double op, int mantissaLength = DefaultMantissaLength) : this(new MpuT(op), mantissaLength) { }

	public UnsignedLongReal(int op, int mantissaLength = MinMantissaLength) : this(mantissaLength)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(uint op, int mantissaLength = MinMantissaLength) : this(mantissaLength)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(long op, int mantissaLength = MinMantissaLength) : this(mantissaLength)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(ulong op, int mantissaLength = MinMantissaLength) : this(mantissaLength)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(MpzT op, int mantissaLength = DefaultMantissaLength) : this(mantissaLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(op);
		var bitLength = op.BitLength;
		if (bitLength <= MantissaLength)
		{
			m = Unsafe.As<MpuT>(op);
			e = null;
		}
		else
		{
			var eDiff = bitLength - MantissaLength - 1;
			m = Unsafe.As<MpuT>(op).ShiftRightRound(eDiff) & MantissaMask;
			e = new(eDiff + 1, mantissaLength);
		}
	}

	public UnsignedLongReal(MpuT op, int mantissaLength = DefaultMantissaLength) : this(mantissaLength)
	{
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

	public UnsignedLongReal(UnsignedLongReal op) : this(op.m, op.e?.Copy(), op.MantissaLength) { }

	public UnsignedLongReal(UnsignedLongReal op, int mantissaLength = DefaultMantissaLength)
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
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.E => throw new NotSupportedException();
	private int MantissaByteLength => GetArrayLength(MantissaLength, 8);
	private MpuT MantissaOverflow => MantissaOverflows.GetOrAdd(MantissaLength, x => MpuT.One << x);
	private MpuT MantissaMask => MantissaMasks.GetOrAdd(MantissaLength, x => MantissaOverflow - 1);
	public static UnsignedLongReal MultiplicativeIdentity => One;
	static UnsignedLongReal ISignedNumber<UnsignedLongReal>.NegativeOne => throw new NotSupportedException();
	public static UnsignedLongReal One => new(1, DefaultMantissaLength);
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.Pi => throw new NotSupportedException();
	public static int Radix => 2;
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.Tau => throw new NotSupportedException();
	public static UnsignedLongReal Zero => new(0, DefaultMantissaLength);

	public UnsignedLongReal BitLength => Compute(this, null!, ComputeOperation.BitLength);

	public static UnsignedLongReal Abs(UnsignedLongReal op) => new(op.m, op.e?.Copy());

	public object Clone() => new UnsignedLongReal(m, e?.Copy());

	public int CompareTo(int other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	public int CompareTo(uint other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	public int CompareTo(long other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	public int CompareTo(ulong other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	public int CompareTo(MpzT other)
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

	public int CompareTo(UnsignedLongReal? other) => (int)Compute(this, other!, ComputeOperation.Compare).m - 1;

	private static UnsignedLongReal Compute(UnsignedLongReal x, UnsignedLongReal y, ComputeOperation operation)
	{
		switch (operation)
		{
			case ComputeOperation.BitLength:
			return x.e is null ? x.m.BitLength : Compute(x.e, x.MantissaLength, ComputeOperation.Add);
			case ComputeOperation.Compare:
			if (y is null)
				return new(2, null);
			if (x.e is null && y.e is null)
				return new(Math.Sign(x.m.CompareTo(y.m)) + 1, null);
			if (x.e is null)
				return new(0, null);
			if (y.e is null)
				return new(2, null);
			var xBitLength = Compute(x, null!, ComputeOperation.BitLength);
			var yBitLength = Compute(y, null!, ComputeOperation.BitLength);
			var compared = Compute(xBitLength, yBitLength, ComputeOperation.Compare).m;
			if (compared != 1)
				return new(compared, null);
			var mlDiff = x.MantissaLength - y.MantissaLength;
			if (mlDiff >= 0)
				return new(Math.Sign(x.m.CompareTo(y.m << mlDiff)) + 1, null);
			else
				return new(Math.Sign((x.m << -mlDiff).CompareTo(y.m)) + 1, null);
			case ComputeOperation.ChangeML:
			var mantissaLength = (int)y.m >>> 1;
			var copy = (y.m & 1) != 0;
			if (mantissaLength == x.MantissaLength)
				return copy ? x.Copy() : x;
			mlDiff = mantissaLength - x.MantissaLength;
			var xMantissaOverfow = x.MantissaOverflow;
			if (mlDiff > 0)
			{
				if (x.e is null)
					return new(x.m, mantissaLength);
				else if (Compute(x.e, mlDiff, ComputeOperation.Compare).m <= 1)
					return new(xMantissaOverfow + x.m << (x.e & -1) - 1, mantissaLength);
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
			if (Compute(y, x, ComputeOperation.Compare).m > 1)
				(x, y) = (y, x);
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			var mantissaOverflow = MpuT.One << mantissaLength;
			var mantissaMask = mantissaOverflow - 1;
			var xmlDiff = mantissaLength - x.MantissaLength;
			var ymlDiff = mantissaLength - y.MantissaLength;
			xBitLength = Compute(x, null!, ComputeOperation.BitLength);
			yBitLength = Compute(y, null!, ComputeOperation.BitLength);
			var yMantissaOverflow = y.MantissaOverflow;
			UnsignedLongReal newE;
			if (x.e is null || Compute(xBitLength, mantissaLength, ComputeOperation.Compare).m <= 1
				&& Compute(yBitLength, mantissaLength, ComputeOperation.Compare).m <= 1)
			{
				var mSum = (MpuT)x + (MpuT)y;
				if (Mpir.MpuCmp(mSum, mantissaOverflow) >= 0)
					return new(mSum & mantissaMask, 1, mantissaLength);
				return new(mSum, null, mantissaLength);
			}
			else if (y.e is null || Compute(yBitLength, mantissaLength, ComputeOperation.Compare).m <= 1)
			{
				if (xBitLength.e is not null)
					return Compute(x, mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var blDiff = (xBitLength & -1) - Math.Max(y.MantissaLength + 1, yBitLength & -1);
				if (blDiff > mantissaLength)
					return Compute(x, mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var ym = (y.e is null ? 0 : yMantissaOverflow) + y.m;
				var mSum = (x.m << xmlDiff) + (ym << ymlDiff).ShiftRightRound(blDiff);
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
			else if (Compute(x.e, y.e, ComputeOperation.Compare).m >= 1)
			{
				var blDiff = Compute(xBitLength, yBitLength, ComputeOperation.Subtract);
				if (blDiff.e is null && blDiff.m == 0)
				{
					newE = Compute(Compute(x.e, 1, ComputeOperation.Add), xmlDiff, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new(((x.m << xmlDiff) + y.m).ShiftRightRound(1), newE, mantissaLength);
				}
				if (Compute(blDiff, mantissaLength, ComputeOperation.Compare).m > 1)
					return Compute(x, mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var mSum = (x.m << xmlDiff) + (yMantissaOverflow + y.m << ymlDiff).ShiftRightRound(blDiff & -1);
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
				var mSum = x.m + (yMantissaOverflow + y.m << (eDiff & -1));
				if (Mpir.MpuCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = Compute(xBitLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mSum & mantissaMask).ShiftRightRound(1), newE, mantissaLength);
				}
				return new(mSum, Compute(x.e, mantissaLength << 1 | 1, ComputeOperation.ChangeML), mantissaLength);
			}
			case ComputeOperation.Subtract:
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			mantissaOverflow = MpuT.One << mantissaLength;
			mantissaMask = mantissaOverflow - 1;
			x = Compute(x, mantissaLength << 1, ComputeOperation.ChangeML);
			y = Compute(y, mantissaLength << 1, ComputeOperation.ChangeML);
			if (x.e is null && y.e is null)
				return new(x.m - y.m, null, mantissaLength);
			else if (y.e is null)
			{
				Debug.Assert(x.e is not null);
				if (Compute(x.e, mantissaLength + 1, ComputeOperation.Compare).m >= 1)
					return x.Copy();
				var mDiff = mantissaOverflow + x.m - y.m.ShiftRightRound((x.e & -1) - 1);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff & mantissaMask, x.e?.Copy(), mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new((mDiff << 1) & mantissaMask, (x.e & -1) - 1, mantissaLength);
			}
			else if (x.e is null || Compute(x.e, y.e, ComputeOperation.Compare).m < 1)
				throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
			else if (Compute(x.e, Compute(y.e, 1, ComputeOperation.Add), ComputeOperation.Compare).m > 1)
			{
				var eDiff = Compute(x.e, y.e, ComputeOperation.Subtract);
				if (Compute(eDiff, mantissaLength, ComputeOperation.Compare).m > 1)
					return x.Copy();
				var mDiff = mantissaOverflow + x.m - (mantissaOverflow + y.m).ShiftRightRound(eDiff & -1);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff & mantissaMask, x.e?.Copy(), mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new((mDiff << 1) & mantissaMask, Compute(x.e, 1, ComputeOperation.Subtract), mantissaLength);
			}
			else if (Compute(x.e, y.e, ComputeOperation.Compare).m == 1)
			{
				var mDiff = x.m - y.m;
				if (mDiff == 0)
					return new(0, null, mantissaLength);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (Compute(x.e, shiftAmount, ComputeOperation.Compare).m <= 1)
					return new(mDiff << (x.e & -1) - 1, null);
				return new((mDiff << shiftAmount) & mantissaMask,
					Compute(x.e, shiftAmount, ComputeOperation.Subtract), mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + x.m << 1) - (mantissaOverflow + y.m);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (shiftAmount == -1)
					return new(mDiff.ShiftRightRound(1) & mantissaMask, x.e?.Copy(), mantissaLength);
				return new((mDiff << shiftAmount) & mantissaMask,
					Compute(x.e, shiftAmount + 1, ComputeOperation.Subtract), mantissaLength);
			}
			default:
			return Zero;
		}
	}

	public UnsignedLongReal Copy() => new(m, e?.Copy(), MantissaLength);

	public void Dispose()
	{
		e?.Dispose();
		GC.SuppressFinalize(this);
	}

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
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(x, 1) == 0)
				return (this, MpuT.Zero);
			else if (e <= x.BitLength + 1)
				return (new((mantissaOverflow + m << (e & -1) - 1).Divide(x, out var remainder), null,
					MantissaLength), remainder);
			var quotient = (mantissaOverflow + m << MantissaLength + 1) / x;
			var shiftAmount = quotient.BitLength - MantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, e + shiftAmount - MantissaLength - 1,
				MantissaLength), MpuT.Zero);
		}
		else if (e is null || e < x.BitLength - MantissaLength - 1)
			return (new(0, MantissaLength), (MpuT)this);
		else if (e <= x.BitLength + 1)
			return (new((MantissaOverflow + m << (e & -1) - 1).Divide(x, out var remainder), null,
				MantissaLength), remainder);
		else
		{
			var quotient = (MantissaOverflow + m << (e & -1)) / (x << 1);
			var shiftAmount = quotient.BitLength - MantissaLength;
			return (new(quotient.ShiftRightRound(shiftAmount - 1) & MantissaMask, shiftAmount, MantissaLength), MpuT.Zero);
		}
	}

	public (UnsignedLongReal Quotient, UnsignedLongReal Remainder) DivRem(UnsignedLongReal x)
	{
		var maxMantissaLength = Math.Max(MantissaLength, x.MantissaLength);
		var MantissaOverflow = MantissaOverflows.GetOrAdd(maxMantissaLength, x => MpuT.One << x);
		var MantissaMask = MantissaOverflow - 1;
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
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return (this2, new(0, maxMantissaLength));
			else if (this2.e <= x.m.BitLength)
				return (new(((MantissaOverflow + this2.m) << (this2.e & -1) - 1).Divide(x.m, out var remainder),
					maxMantissaLength), new(remainder, maxMantissaLength));
			var quotient = (MantissaOverflow + this2.m << maxMantissaLength + 1) / x.m;
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, this2.e + shiftAmount - maxMantissaLength - 1,
				maxMantissaLength), new(0, maxMantissaLength));
		}
		else if (this2.e is null || this2.e < x.e)
			return (new(0, maxMantissaLength), this2);
		else if (this2.e <= x.e + maxMantissaLength)
		{
			var eDiff = (this2.e - x.e) & -1;
			var quotient = ((MantissaOverflow + this2.m) << eDiff).Divide(MantissaOverflow + x.m, out var remainder);
			return (new(quotient, maxMantissaLength), new(remainder << (x.e & -1) - 1, maxMantissaLength));
		}
		else
		{
			var quotient = (MantissaOverflow + this2.m << maxMantissaLength + 1) / (MantissaOverflow + x.m);
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, this2.e - x.e + shiftAmount - maxMantissaLength,
				maxMantissaLength), new(0, maxMantissaLength));
		}
	}

	public UnsignedLongReal DivRem(MpuT x, out MpuT remainder)
	{
		(var Quotient, remainder) = DivRem(x);
		return Quotient;
	}

	public UnsignedLongReal DivRem(UnsignedLongReal x, out UnsignedLongReal remainder)
	{
		(var Quotient, remainder) = DivRem(x);
		return Quotient;
	}

	public bool Equals(int other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	public bool Equals(uint other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	public bool Equals(long other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	public bool Equals(ulong other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

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

	public bool Equals(UnsignedLongReal? other) => CompareTo(other) == 0;

	public int GetByteCount() => GetByteCount(true);
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

	public int GetShortestBitLength() => e is null ? m.GetShortestBitLength() : (e + MantissaLength) & -1;
	public int GetSignificandBitLength() => m.GetShortestBitLength();
	public int GetSignificandByteCount() => m.GetByteCount();
	TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

	private UnsignedLongReal GetWithOtherML(int mantissaLength, bool copy) =>
		Compute(this, new((ulong)mantissaLength << 1 | (copy ? 1u : 0), null), ComputeOperation.ChangeML);

	public static bool IsCanonical(UnsignedLongReal value) => true;
	public static bool IsComplexNumber(UnsignedLongReal value) => true;
	public bool IsEven() => e is not null || (m & 1) == 0;
	public static bool IsEvenInteger(UnsignedLongReal value) => value.IsEven();
	public static bool IsFinite(UnsignedLongReal value) => true;
	public static bool IsImaginaryNumber(UnsignedLongReal value) => false;
	public static bool IsInfinity(UnsignedLongReal value) => false;
	public static bool IsInteger(UnsignedLongReal value) => true;
	public static bool IsNaN(UnsignedLongReal value) => false;
	public static bool IsNegative(UnsignedLongReal value) => false;
	public static bool IsNegativeInfinity(UnsignedLongReal value) => false;
	public static bool IsNormal(UnsignedLongReal value) => true;
	public static bool IsOddInteger(UnsignedLongReal value) => !IsEvenInteger(value);
	public static bool IsPositive(UnsignedLongReal value) => true;
	public static bool IsPositiveInfinity(UnsignedLongReal value) => false;
	public static bool IsPow2(UnsignedLongReal value) => value.PopCount() == 1;
	public static bool IsRealNumber(UnsignedLongReal value) => true;
	public static bool IsSubnormal(UnsignedLongReal value) => true;
	public static bool IsZero(UnsignedLongReal value) => Mpir.MpzCmpSi(value.m, 0) == 0 && value.e is null;

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
	public static UnsignedLongReal Parse(string? s) => new(s);
	public static UnsignedLongReal Parse(string s, IFormatProvider? provider) => new(s);
	public static UnsignedLongReal Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);

	public int PopCount() => m.PopCount() + (e is null ? 0 : 1);
	public static UnsignedLongReal PopCount(UnsignedLongReal value) => value.PopCount();

	static UnsignedLongReal IFloatingPoint<UnsignedLongReal>.Round(UnsignedLongReal x, int digits, MidpointRounding mode) => x;

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

	public byte[] ToByteArray(int order, bool saveMantissaLength = true)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(GetByteCount(saveMantissaLength));
		var indent = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(bytes, MantissaByteLength);
			indent = sizeof(int);
		}
		if (e is null)
		{
			if (order < 0)
				m.TryWriteLittleEndian(bytes.AsSpan(indent), out _);
			else
				m.TryWriteBigEndian(bytes.AsSpan(indent), out _);
			return bytes;
		}
		var mLength = m.GetByteCount();
		if (order < 0)
			m.TryWriteLittleEndian(bytes.AsSpan(indent), out _);
		else
			m.TryWriteBigEndian(bytes.AsSpan(^mLength), out _);
		Array.Fill<byte>(bytes, 0, order < 0 ? mLength : indent, MantissaByteLength - mLength);
		if (order < 0)
			e.TryWriteLittleEndian(bytes.AsSpan(indent + MantissaByteLength), out _, false);
		else
			e.TryWriteBigEndian(bytes.AsSpan(indent..^MantissaByteLength), out _, false);
		return bytes;
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
	public string? ToShortString() =>
		m.val == 0 ? "0" : BitLength >= 65536 ? "Too large for short string, use ToString() instead." : ((MpuT)this).ToString();
	public override string? ToString() => ((MpuT)this).ToString(DefaultStringBase);
	public string ToString(IFormatProvider? provider) => ToString(DefaultStringBase) ?? "";
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString(DefaultStringBase));
	public string? ToString(uint @base) => ((MpuT)this).ToString(@base);

	object IConvertible.ToType(Type targetType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(targetType);
		if (targetType == typeof(UnsignedLongReal))
			return Copy();
		IConvertible value = this;
		if (targetType == typeof(sbyte))
			return value.ToSByte(provider);
		else if (targetType == typeof(byte))
			return value.ToByte(provider);
		else if (targetType == typeof(short))
			return value.ToInt16(provider);
		else if (targetType == typeof(ushort))
			return value.ToUInt16(provider);
		else if (targetType == typeof(int))
			return value.ToInt32(provider);
		else if (targetType == typeof(uint))
			return value.ToUInt32(provider);
		else if (targetType == typeof(long))
			return value.ToInt64(provider);
		else if (targetType == typeof(ulong))
			return value.ToUInt64(provider);
		else if (targetType == typeof(float))
			return value.ToSingle(provider);
		else if (targetType == typeof(double))
			return value.ToDouble(provider);
		else if (targetType == typeof(decimal))
			return value.ToDecimal(provider);
		else if (targetType == typeof(MpzT))
			return new MpzT(value.ToString(provider));
		else if (targetType == typeof(MpuT))
			return new MpuT(value.ToString(provider));
		else if (targetType == typeof(string))
			return value.ToString(provider);
		else if (targetType == typeof(object))
			return Copy();
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(UnsignedLongReal)
			+ ", " + nameof(MpzT) + ", " + nameof(MpuT)
			+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

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
				MpzT z => z,
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
				MpzT z => z,
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

	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength)
	{
		bytesWritten = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(destination, MantissaByteLength);
			destination = destination[sizeof(int)..];
			bytesWritten += sizeof(int);
		}
		if (e is null)
			return m.TryWriteBigEndian(destination, out bytesWritten);
		var mLength = m.GetByteCount();
		if (!m.TryWriteBigEndian(destination[^mLength..], out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += MantissaByteLength;
		destination[..(MantissaByteLength - mLength)].Clear();
		if (!e.TryWriteBigEndian(destination[..^MantissaByteLength], out var bytesWritten2, saveMantissaLength))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += bytesWritten2;
		return true;
	}

	public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? 0 : e).TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? 0 : e).TryWriteLittleEndian(destination, out bytesWritten);

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteLittleEndian(destination, out bytesWritten, true);

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength)
	{
		bytesWritten = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(destination, MantissaByteLength);
			destination = destination[sizeof(int)..];
			bytesWritten += sizeof(int);
		}
		if (e is null)
			return m.TryWriteLittleEndian(destination, out bytesWritten);
		var mLength = m.GetByteCount();
		if (!m.TryWriteLittleEndian(destination, out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += MantissaByteLength;
		destination[mLength..MantissaByteLength].Clear();
		if (!e.TryWriteLittleEndian(destination[MantissaByteLength..], out var bytesWritten2, saveMantissaLength))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += bytesWritten2;
		return true;
	}

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
	public static implicit operator UnsignedLongReal(MpzT value) => new(value);
	public static implicit operator UnsignedLongReal(MpuT value) => new(value);
	public static explicit operator UnsignedLongReal(float value) => new((double)value);
	public static explicit operator UnsignedLongReal(double value) => new(value);
	public static explicit operator UnsignedLongReal(decimal value) => new(value);
	public static explicit operator UnsignedLongReal(string value) => new(value, DefaultStringBase);
	public static explicit operator byte(UnsignedLongReal value) => (byte)(uint)value;
	public static explicit operator int(UnsignedLongReal value) => value & -1;
	public static explicit operator uint(UnsignedLongReal value) => value & uint.MaxValue;
	public static explicit operator short(UnsignedLongReal value) => (short)(int)value;
	public static explicit operator ushort(UnsignedLongReal value) => (ushort)(uint)value;
	public static explicit operator long(UnsignedLongReal value) => (long)(value & ulong.MaxValue).m;
	public static explicit operator ulong(UnsignedLongReal value) => (ulong)(value & ulong.MaxValue).m;
	public static explicit operator float(UnsignedLongReal value) => (float)(double)value;

	public static explicit operator double(UnsignedLongReal value) =>
		value.BitLength > 1024 ? double.PositiveInfinity
		: (double)(value.e is null ? value.m : value.MantissaOverflow + value.m << (value.e & -1) - 1);

	public static explicit operator decimal(UnsignedLongReal value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);

	public static explicit operator string?(UnsignedLongReal value) => value.ToString();

	public static explicit operator MpzT(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return new MpzT(value.MantissaOverflow + value.m) << (value.e & -1) - 1;
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
			return 0;
	}

	public static UnsignedLongReal operator +(UnsignedLongReal value) => new(value);
	static UnsignedLongReal IUnaryNegationOperators<UnsignedLongReal, UnsignedLongReal>.operator -(UnsignedLongReal value) =>
		throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");
	static UnsignedLongReal IBitwiseOperators<UnsignedLongReal, UnsignedLongReal, UnsignedLongReal>.operator ~(UnsignedLongReal value) =>
		throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");

	public static UnsignedLongReal operator +(UnsignedLongReal x, UnsignedLongReal y) =>
		Compute(x, y, ComputeOperation.Add);

	public static UnsignedLongReal operator -(UnsignedLongReal x, UnsignedLongReal y) =>
		Compute(x, y, ComputeOperation.Subtract);

	public static UnsignedLongReal operator *(int x, UnsignedLongReal y) => y * x;
	public static UnsignedLongReal operator *(uint x, UnsignedLongReal y) => y * x;

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
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount, MantissaLength);
		}
	}

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
				return x.Copy();
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
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y.m;
			var shiftAmount = product.BitLength - maxMantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount, maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				return new(0, maxMantissaLength);
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return y.Copy();
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

	public static UnsignedLongReal operator /(UnsignedLongReal x, int y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e is null)
			return new(x.m / y, null, MantissaLength);
		else if (y == 0)
			throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
		else if (y == 1)
			return x.Copy();
		else if (x.e <= sizeof(int) * 8 - int.LeadingZeroCount(y))
			return new(((MantissaOverflow + x.m) << (x.e & -1) - 1) / y, MantissaLength);
		var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y;
		var shiftAmount = quotient.BitLength - MantissaLength - 1;
		return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1,
			MantissaLength);
	}

	public static UnsignedLongReal operator /(UnsignedLongReal x, uint y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e is null)
			return new(x.m / y, null, MantissaLength);
		else if (y == 0)
			throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
		else if (y == 1)
			return x.Copy();
		else if (x.e <= sizeof(uint) * 8 - uint.LeadingZeroCount(y))
			return new(((MantissaOverflow + x.m) << (x.e & -1) - 1) / y, MantissaLength);
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
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x.Copy();
			else if (x.e <= y.m.BitLength)
				return new(((MantissaOverflow + x.m) << (x.e & -1) - 1) / y.m, maxMantissaLength);
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

	public static int operator &(UnsignedLongReal x, int y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > sizeof(int) * 8)
			return 0;
		else
			return x.MantissaOverflow + x.m << (x.e & -1) - 1 & y;
	}

	public static uint operator &(UnsignedLongReal x, uint y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1u) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > sizeof(uint) * 8)
			return 0;
		else
			return x.MantissaOverflow + x.m << (x.e & -1) - 1 & y;
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
			return new(x.m << (x.e & -1) - 1 & y.m, maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e > x.m.BitLength)
				return new(0, maxMantissaLength);
			return new(x.m & y.m << (y.e & -1) - 1, maxMantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff >= maxMantissaLength)
				return new(0, maxMantissaLength);
			var newMantissa = MantissaOverflow + x.m & (MantissaOverflow + y.m << (eDiff & -1));
			if (Mpir.MpzCmpSi(newMantissa, 0) == 0)
				return new(0, maxMantissaLength);
			var shiftAmount = (MantissaOverflow + x.m).BitLength - newMantissa.BitLength;
			if (x.e <= shiftAmount)
				return new(newMantissa << (x.e & -1) - 1, null, maxMantissaLength);
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
				return x.Copy();
			return new(x.m | y.m.ShiftRightRound((x.e & -1) - 1), x.e?.Copy(), maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= maxMantissaLength + 1)
				return y.Copy();
			return new(x.m.ShiftRightRound((y.e & -1) - 1) | y.m, y.e?.Copy(), maxMantissaLength);
		}
		else if (x.e == y.e)
			return new(x.m | y.m, x.e?.Copy(), maxMantissaLength);
		else if (x.e > y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff >= maxMantissaLength)
				return x.Copy();
			return new(x.m | (MantissaOverflow + y.m).ShiftRightRound(eDiff & -1), x.e?.Copy(), maxMantissaLength);
		}
		else
		{
			var eDiff = y.e - x.e;
			if (eDiff >= maxMantissaLength)
				return y.Copy();
			return new((MantissaOverflow + x.m).ShiftRightRound(eDiff & -1) | y.m, y.e?.Copy(), maxMantissaLength);
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
				return x.Copy();
			return new(x.m ^ y.m.ShiftRightRound((x.e & -1) - 1), x.e?.Copy(), maxMantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= maxMantissaLength + 1)
				return y.Copy();
			return new(x.m.ShiftRightRound((y.e & -1) - 1) ^ y.m, y.e?.Copy(), maxMantissaLength);
		}
		else if (x.e > y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff >= maxMantissaLength)
				return x.Copy();
			return new(x.m ^ (MantissaOverflow + y.m).ShiftRightRound(eDiff & -1), x.e?.Copy(), maxMantissaLength);
		}
		else if (x.e < y.e)
		{
			var eDiff = y.e - x.e;
			if (eDiff >= maxMantissaLength)
				return y.Copy();
			return new((MantissaOverflow + x.m).ShiftRightRound(eDiff & -1) ^ y.m, y.e?.Copy(), maxMantissaLength);
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
			return x.Copy();
		else if (x.e is null)
			return new(x.m << shiftAmount, x.MantissaLength);
		else
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
	}

	public static UnsignedLongReal operator <<(UnsignedLongReal x, UnsignedLongReal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x.Copy();
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
			return x.Copy();
		else if (x.e is null)
			return new(x.m.ShiftRightRound(shiftAmount), null, x.MantissaLength);
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new((x.MantissaOverflow + x.m).ShiftRightRound(shiftAmount - (x.e & -1) + 1), null, x.MantissaLength);
	}

	public static UnsignedLongReal operator >>(UnsignedLongReal x, UnsignedLongReal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x.Copy();
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

	public static UnsignedLongReal operator >>>(UnsignedLongReal x, UnsignedLongReal shiftAmount) => x >> shiftAmount;

	public static UnsignedLongReal operator ++(UnsignedLongReal value)
	{
#pragma warning disable IDE0078 // Используйте сопоставление шаблонов
		if (value.e is not null && value.e > 2)
			return value.Copy();
		else if (Mpir.MpuCmp(value.m, value.MantissaMask) == 0)
			return new(0, value.e is not null ? 2 : 1, value.MantissaLength);
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
			return value.Copy();
	}

	public static bool operator ==(UnsignedLongReal x, int y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal x, int y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, int y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, int y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, int y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, int y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongReal x, uint y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal x, uint y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, uint y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, uint y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, uint y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, uint y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongReal x, long y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal x, long y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, long y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, long y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, long y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, long y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongReal x, ulong y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal x, ulong y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, ulong y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, ulong y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, ulong y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, ulong y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongReal x, MpzT y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal x, MpzT y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, MpzT y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, MpzT y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, MpzT y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, MpzT y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongReal x, MpuT y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal x, MpuT y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, MpuT y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, MpuT y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, MpuT y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, MpuT y) => x.CompareTo(y) < 0;
	public static bool operator ==(int x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(int x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(int x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(int x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(int x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(int x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(uint x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(uint x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(uint x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(uint x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(uint x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(uint x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(long x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(long x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(long x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(long x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(long x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(long x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(ulong x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(ulong x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(ulong x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(ulong x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(ulong x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(ulong x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(MpzT x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(MpzT x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(MpzT x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(MpzT x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(MpzT x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(MpzT x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(MpuT x, UnsignedLongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(MpuT x, UnsignedLongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(MpuT x, UnsignedLongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(MpuT x, UnsignedLongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(MpuT x, UnsignedLongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(MpuT x, UnsignedLongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(UnsignedLongReal? x, UnsignedLongReal? y) => x?.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongReal? x, UnsignedLongReal? y) => x?.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongReal x, UnsignedLongReal y) => x.CompareTo(y) < 0;
}
