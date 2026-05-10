namespace NStar.Mpir;

/// <summary>
/// Представляет число с плавающей точкой - как <see cref="UnsignedLongReal"/>,
/// только с десятичной экспонентой вместо двоичной, что гарантирует точность при десятичных операциях
/// (при достаточной длине мантиссы, разумеется!).
/// </summary>
[DebuggerDisplay("{ToShortString()}")]
public sealed class UnsignedLongDecimal : ICloneable, IConvertible, IComparable, IComparable<UnsignedLongDecimal>,
	IDisposable, IBinaryInteger<UnsignedLongDecimal>, IFloatingPoint<UnsignedLongDecimal>
{
	private enum ComputeOperation : byte
	{
		Identity,
		DecLength,
		Compare,
		ChangeML,
		Add,
		Subtract,
	}

	private static readonly ConcurrentDictionary<int, MpuT> MantissaMasks = [], MantissaOverflows = [];
	private readonly MpuT m;
	private readonly UnsignedLongDecimal? e;
	private readonly int MantissaLength = 0;
	public const int AutoMantissaLength = -1, DefaultMantissaLength = 3000, MinMantissaLength = 30;

	private UnsignedLongDecimal(MpuT m, UnsignedLongDecimal? e, int mantissaLength = DefaultMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		this.m = m;
		this.e = e;
	}

	public UnsignedLongDecimal(decimal op, int mantissaLength = DefaultMantissaLength) : this(new MpuT(op), mantissaLength) { }

	public UnsignedLongDecimal(double op, int mantissaLength = DefaultMantissaLength) : this(new MpuT(op), mantissaLength) { }

	public UnsignedLongDecimal(int op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongDecimal(uint op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongDecimal(long op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongDecimal(ulong op, int mantissaLength = MinMantissaLength) : this(new MpuT(op), null, mantissaLength) { }

	public UnsignedLongDecimal(MpzT op, int mantissaLength = DefaultMantissaLength) : this(op < 0
		? throw new ArgumentException("Этот тип не поддерживает отрицательные числа.", nameof(op))
		: Unsafe.As<MpuT>(op), mantissaLength) { }

	public UnsignedLongDecimal(MpuT op, int mantissaLength = DefaultMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		if ((op / 9).DecLength < MantissaLength || op < MantissaOverflow)
		{
			m = op;
			e = null;
		}
		else
		{
			var eDiff = op.DecLength - MantissaLength - 1;
			var shifted = op.ShiftRightRoundDec(eDiff);
			if (shifted == MantissaOverflow * 10)
			{
				m = MpuT.Zero;
				e = new(eDiff + 2, mantissaLength);
			}
			else
			{
				m = shifted - MantissaOverflow;
				e = new(eDiff + 1, mantissaLength);
			}
		}
	}

	public UnsignedLongDecimal(UnsignedLongDecimal op) : this(op.m, op.e?.Copy(), op.MantissaLength) { }

	public UnsignedLongDecimal(UnsignedLongDecimal op, int mantissaLength = DefaultMantissaLength)
		: this(op.GetWithOtherML(mantissaLength, true) is var x ? x.m : MpuT.Zero, x.e, mantissaLength) { }

	public UnsignedLongDecimal(BigInteger op, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(op), mantissaLength) { }

	public UnsignedLongDecimal(string? s, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(s), mantissaLength) { }

	public UnsignedLongDecimal(string? s, uint @base, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(s, @base), mantissaLength) { }

	public UnsignedLongDecimal(ReadOnlySpan<byte> bytes, int order, int mantissaLength = AutoMantissaLength)
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
			if ((m / 9).DecLength < MantissaLength || m < MantissaOverflow)
				return;
			var shiftAmount = m.DecLength - MantissaLength - 1;
			m = m.ShiftRightRoundDec(shiftAmount) - MantissaOverflow;
			e = new(shiftAmount == 0 ? 1 : shiftAmount + 1, null, mantissaLength);
		}
		else
		{
			var mStart = Math.Max(order, 0) * (bytes.Length - mantissaByteLength);
			var eStart = Math.Max(-order, 0) * mantissaByteLength;
			m = new(bytes.Slice(mStart, mantissaByteLength), order);
			e = new UnsignedLongDecimal(bytes.Slice(eStart, bytes.Length - mantissaByteLength), order, mantissaLength)
				is var num && num > 0 ? num : null;
			var decLength = (m / 9).DecLength;
			if (decLength <= MantissaLength)
				return;
			var shiftAmount = decLength - MantissaLength;
			m = m.ShiftRightRoundDec(shiftAmount);
			if (e is not null)
				e += new UnsignedLongDecimal(shiftAmount, null, mantissaLength);
			else if (shiftAmount != 0)
				e = new(shiftAmount, null, mantissaLength);
		}
	}

	~UnsignedLongDecimal() => Dispose();

	public static UnsignedLongDecimal AdditiveIdentity => Zero;
	static UnsignedLongDecimal IFloatingPointConstants<UnsignedLongDecimal>.E => throw new NotSupportedException();
	private int MantissaByteLength => (int)Math.Ceiling((MantissaLength + Math.Log10(9)) * Math.Log(10, 256));
	private MpuT MantissaMask => MantissaMasks.GetOrAdd(MantissaLength, x => MpuT.PowerOfTen(MantissaLength) * 9 - 1);
	private MpuT MantissaOverflow => MpuT.PowerOfTen(MantissaLength);
	public static UnsignedLongDecimal MultiplicativeIdentity => One;
	static UnsignedLongDecimal ISignedNumber<UnsignedLongDecimal>.NegativeOne => throw new NotSupportedException();
	public static UnsignedLongDecimal One { get; } = new(1, MinMantissaLength);
	static UnsignedLongDecimal IFloatingPointConstants<UnsignedLongDecimal>.Pi => throw new NotSupportedException();
	public static int Radix => 10;
	static UnsignedLongDecimal IFloatingPointConstants<UnsignedLongDecimal>.Tau => throw new NotSupportedException();
	public static UnsignedLongDecimal Zero { get; } = new(0, MinMantissaLength);

	public UnsignedLongDecimal DecLength => Compute(this, null!, ComputeOperation.DecLength);

	public static UnsignedLongDecimal Abs(UnsignedLongDecimal op) => new(op.m, op.e?.Copy());

	public object Clone() => new UnsignedLongDecimal(m, e?.Copy());

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

	public int CompareTo(MpuT other)
	{
		if (e is null)
			return m.CompareTo(other);
		var decLength = other.DecLength;
		var eDiff = decLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return eComparison;
		return (MantissaOverflow + m).ShiftLeftDec(eDiff - 1).CompareTo(other);
	}

	public int CompareTo(MpzT other)
	{
		if (Mpir.MpzCmpSi(other, 0) < 0)
			return 1;
		if (e is null)
			return m.CompareTo(other);
		var decLength = other.DecLength;
		var eDiff = decLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return eComparison;
		return (MantissaOverflow + m).ShiftLeftDec(eDiff - 1).CompareTo(other);
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
		UnsignedLongDecimal uld => CompareTo(uld),
		BigInteger bi => CompareTo(new MpzT(bi)),
		IComparable ic => -ic.CompareTo(this),
		_ => 0,
	};

	public int CompareTo(UnsignedLongDecimal? other) => (int)Compute(this, other!, ComputeOperation.Compare).m - 1;

	private static UnsignedLongDecimal Compute(UnsignedLongDecimal x, UnsignedLongDecimal y, ComputeOperation operation)
	{
		switch (operation)
		{
			case ComputeOperation.DecLength:
			if (x.e is null)
				return new(x.m.DecLength, null, x.MantissaLength);
			else
				return Compute(x.e, new(x.MantissaLength, null, x.MantissaLength), ComputeOperation.Add);
			case ComputeOperation.Compare:
			if (y is null)
				return new(2, null);
			if (x.e is null && y.e is null)
				return new(Math.Sign(x.m.CompareTo(y.m)) + 1, null);
			if (x.e is null && y.e is not null && (y.e.e is not null || y.e.m + y.MantissaLength > x.m.DecLength))
				return new(0, null);
			if (y.e is null && x.e is not null && (x.e.e is not null || x.e.m + x.MantissaLength > y.m.DecLength))
				return new(2, null);
			if (x.e is null && y.e is not null && y.e.e is null)
				return new(Math.Sign(x.m.ShiftRightDec((y.e.m & -1) - 1).CompareTo(y.MantissaOverflow + y.m)) + 1, null);
			if (y.e is null && x.e is not null && x.e.e is null)
				return new(Math.Sign((x.MantissaOverflow + x.m).CompareTo(y.m.ShiftRightDec((x.e.m & -1) - 1))) + 1, null);
			var xDecLength = Compute(x, null!, ComputeOperation.DecLength);
			var yDecLength = Compute(y, null!, ComputeOperation.DecLength);
			var compared = Compute(xDecLength, yDecLength, ComputeOperation.Compare).m;
			if (compared != 1)
				return new(compared, null);
			var mlDiff = x.MantissaLength - y.MantissaLength;
			if (mlDiff >= 0)
				return new(Math.Sign(x.m.CompareTo(y.m.ShiftLeftDec(mlDiff))) + 1, null);
			else
				return new(Math.Sign(x.m.ShiftLeftDec(-mlDiff).CompareTo(y.m)) + 1, null);
			case ComputeOperation.ChangeML:
			var mantissaLength = (int)y.m >>> 1;
			var copy = (y.m & 1) != 0;
			if (mantissaLength == x.MantissaLength)
				return copy ? x.Copy() : x;
			mlDiff = mantissaLength - x.MantissaLength;
			var xMantissaOverfow = x.MantissaOverflow;
			UnsignedLongDecimal newE;
			if (mlDiff > 0)
			{
				if (x.e is null)
					return new(x.m, mantissaLength);
				else if (Compute(x.e, mlDiff, ComputeOperation.Compare).m <= 1)
					return new((xMantissaOverfow + x.m).ShiftLeftDec((x.e & -1) - 1), mantissaLength);
				newE = Compute(x.e, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				newE = Compute(newE, new(mlDiff, null, mantissaLength), ComputeOperation.Subtract);
				return new(x.m.ShiftLeftDec(mlDiff), newE, mantissaLength);
			}
			else
			{
				mlDiff = -mlDiff;
				if (x.e is null)
					return new(x.m, mantissaLength);
				newE = Compute(x.e, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				newE = Compute(newE, new(mlDiff, null, mantissaLength), ComputeOperation.Add);
				return new(x.m.ShiftRightRoundDec(mlDiff), newE, mantissaLength);
			}
			case ComputeOperation.Add:
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			if (x.e is null && Mpir.MpuCmpSi(x.m, 0) == 0)
				return Compute(y, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			if (y.e is null && Mpir.MpuCmpSi(y.m, 0) == 0)
				return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			if (Compute(y, x, ComputeOperation.Compare).m > 1)
				(x, y) = (y, x);
			var mantissaOverflow = MpuT.PowerOfTen(mantissaLength);
			var xmlDiff = mantissaLength - x.MantissaLength;
			var ymlDiff = mantissaLength - y.MantissaLength;
			xDecLength = Compute(x, null!, ComputeOperation.DecLength);
			yDecLength = Compute(y, null!, ComputeOperation.DecLength);
			var yMantissaOverflow = y.MantissaOverflow;
			var mantissaMask = mantissaOverflow * 9 - 1;
			if (x.e is null || Compute(xDecLength, mantissaLength, ComputeOperation.Compare).m <= 1
				&& Compute(yDecLength, mantissaLength, ComputeOperation.Compare).m <= 1)
			{
				var mSum = (MpuT)x + (MpuT)y;
				if (Mpir.MpuCmp(mSum, mantissaMask) > 0)
					return new(mSum - mantissaOverflow, 1, mantissaLength);
				return new(mSum, null, mantissaLength);
			}
			else if (y.e is null || Compute(yDecLength, mantissaLength, ComputeOperation.Compare).m <= 1)
			{
				if (xDecLength.e is not null || xDecLength.m.BitLength >= sizeof(int) * 8)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var blDiff = (xDecLength & -1) - Math.Max(y.MantissaLength + 1, yDecLength & -1);
				if (blDiff > mantissaLength)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var ym = (y.e is null ? 0 : yMantissaOverflow) + y.m;
				var mSum = x.m.ShiftLeftDec(xmlDiff) + ym.ShiftLeftDec(ymlDiff).ShiftRightRoundDec(blDiff & -1);
				if (Mpir.MpuCmp(mSum, mantissaMask) > 0)
				{
					newE = Compute(xDecLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow, newE, mantissaLength);
				}
				newE = Compute(x.e, xmlDiff, ComputeOperation.Subtract);
				newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
				return new(mSum, newE, mantissaLength);
			}
			else if (Compute(x.e, y.e, ComputeOperation.Compare).m >= 1)
			{
				var blDiff = Compute(xDecLength, yDecLength, ComputeOperation.Subtract);
				MpuT mSum;
				if (blDiff.e is null && blDiff.m == 0)
				{
					newE = Compute(x.e, xmlDiff, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					mSum = x.m.ShiftLeftDec(xmlDiff) + mantissaOverflow + y.m;
					var b = Mpir.MpuCmp(mSum, mantissaMask) > 0;
					var newM = b ? (mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow : mSum;
					return new(newM, b ? Compute(newE, 1, ComputeOperation.Add) : newE, mantissaLength);
				}
				if (Compute(blDiff, mantissaLength + 1, ComputeOperation.Compare).m > 1)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				mSum = x.m.ShiftLeftDec(xmlDiff)
					+ (yMantissaOverflow + y.m).ShiftLeftDec(ymlDiff).ShiftRightRoundDec(blDiff & -1);
				if (Mpir.MpuCmp(mSum, mantissaMask) > 0)
				{
					newE = Compute(xDecLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow, newE, mantissaLength);
				}
				newE = Compute(x.e, xmlDiff, ComputeOperation.Subtract);
				newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
				return new(mSum, newE, mantissaLength);
			}
			else
			{
				var blDiff = Compute(xDecLength, yDecLength, ComputeOperation.Subtract);
				MpuT mSum;
				if (blDiff.e is null && blDiff.m == 0)
				{
					newE = Compute(y.e, ymlDiff, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					mSum = mantissaOverflow + x.m + y.m.ShiftLeftDec(ymlDiff);
					var b = Mpir.MpuCmp(mSum, mantissaMask) > 0;
					var newM = b ? (mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow : mSum;
					return new(newM, b ? Compute(newE, 1, ComputeOperation.Add) : newE, mantissaLength);
				}
				var eDiff = Compute(y.e, x.e, ComputeOperation.Subtract);
				mSum = x.m + (yMantissaOverflow + y.m).ShiftLeftDec(eDiff & -1);
				if (Mpir.MpuCmp(mSum, mantissaMask) > 0)
				{
					newE = Compute(xDecLength, mantissaLength - 1, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					return new((mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow, newE, mantissaLength);
				}
				return new(mSum, Compute(x.e, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML), mantissaLength);
			}
			case ComputeOperation.Subtract:
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			if (y.e is null && Mpir.MpuCmpSi(y.m, 0) == 0)
				return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			mantissaOverflow = MpuT.PowerOfTen(mantissaLength);
			x = Compute(x, (long)mantissaLength << 1, ComputeOperation.ChangeML);
			y = Compute(y, (long)mantissaLength << 1, ComputeOperation.ChangeML);
			if (x.e is null && y.e is null)
				return new(x.m - y.m, null, mantissaLength);
			else if (y.e is null)
			{
				Debug.Assert(x.e is not null);
				if (Compute(x.e, mantissaLength + 1, ComputeOperation.Compare).m >= 1)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var mDiff = mantissaOverflow + x.m - y.m.ShiftRightRoundDec((x.e & -1) - 1);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff - mantissaOverflow, x.e?.Copy(), mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new(mDiff * 10 - mantissaOverflow, (x.e & -1) - 1, mantissaLength);
			}
			else if (x.e is null || Compute(x.e, y.e, ComputeOperation.Compare).m < 1)
				throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
			else if (Compute(x.e, Compute(y.e, 1, ComputeOperation.Add), ComputeOperation.Compare).m > 1)
			{
				var eDiff = Compute(x.e, y.e, ComputeOperation.Subtract);
				if (Compute(eDiff, mantissaLength, ComputeOperation.Compare).m > 1)
					return x.Copy();
				var mDiff = mantissaOverflow + x.m - (mantissaOverflow + y.m).ShiftRightRoundDec(eDiff & -1);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff - mantissaOverflow, x.e?.Copy(), mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new(mDiff * 10 - mantissaOverflow, Compute(x.e, 1, ComputeOperation.Subtract), mantissaLength);
			}
			else if (Compute(x.e, y.e, ComputeOperation.Compare).m == 1)
			{
				var mDiff = x.m - y.m;
				if (mDiff == 0)
					return new(0, null, mantissaLength);
				var shiftAmount = mantissaLength - mDiff.DecLength + 1;
				if (Compute(x.e, shiftAmount, ComputeOperation.Compare).m <= 1)
					return new(mDiff.ShiftLeftDec((x.e & -1) - 1), null);
				return new(mDiff.ShiftLeftDec(shiftAmount) - mantissaOverflow,
					Compute(x.e, shiftAmount, ComputeOperation.Subtract), mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + x.m) * 10 - (mantissaOverflow + y.m);
				var shiftAmount = mantissaLength - mDiff.DecLength + 1;
				if (shiftAmount == -1)
					return new(mDiff.ShiftRightRoundDec(1) - mantissaOverflow, x.e?.Copy(), mantissaLength);
				return new(mDiff.ShiftLeftDec(shiftAmount) - mantissaOverflow,
					Compute(x.e, shiftAmount + 1, ComputeOperation.Subtract), mantissaLength);
			}
			default:
			return Zero;
		}
	}

	public UnsignedLongDecimal Copy() => new(m, e?.Copy(), MantissaLength);

	public void Dispose()
	{
		e?.Dispose();
		GC.SuppressFinalize(this);
	}

	public (UnsignedLongDecimal Quotient, MpuT Remainder) DivRem(MpuT x)
	{
		if (e is null)
		{
			var result = m.Divide(x, out var remainder);
			return (new(result, null, MantissaLength), remainder);
		}
		else if (x.DecLength < MantissaLength)
		{
			Debug.Assert(e is not null);
			var mantissaOverflow = MantissaOverflow;
			if (Mpir.MpuCmpSi(x, 0) == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(x, 1) == 0)
				return (this, MpuT.Zero);
			else if (e <= x.DecLength + 1)
				return (new((mantissaOverflow + m).ShiftLeftDec((e & -1) - 1).Divide(x, out var remainder),
					MantissaLength), remainder);
			var quotient = (mantissaOverflow + m).ShiftLeftDec(MantissaLength + 1) / x;
			var shiftAmount = quotient.DecLength - MantissaLength - 1;
			return (new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				e + (shiftAmount - MantissaLength - 1), MantissaLength), MpuT.Zero);
		}
		else if (e is null || e < x.DecLength - MantissaLength - 1)
			return (new(0, null, MantissaLength), (MpuT)this);
		else if (e <= x.DecLength + 1)
			return (new((MantissaOverflow + m).ShiftLeftDec((e & -1) - 1).Divide(x, out var remainder), null,
				MantissaLength), remainder);
		else
		{
			var quotient = (MantissaOverflow + m).ShiftLeftDec(e & -1) / (x * 10);
			var shiftAmount = quotient.DecLength - MantissaLength;
			return (new(quotient.ShiftRightRoundDec(shiftAmount - 1) - MantissaOverflow, shiftAmount, MantissaLength), MpuT.Zero);
		}
	}

	public (UnsignedLongDecimal Quotient, UnsignedLongDecimal Remainder) DivRem(UnsignedLongDecimal x)
	{
		var mantissaLength = Math.Max(MantissaLength, x.MantissaLength);
		var MantissaOverflow = MantissaOverflows.GetOrAdd(mantissaLength, MpuT.PowerOfTen);
		var MantissaMask = MantissaOverflow - 1;
		var this2 = GetWithOtherML(mantissaLength, false);
		x = x.GetWithOtherML(mantissaLength, false);
		if (this2.e is null && x.e is null)
		{
			var result = this2.m.Divide(x.m, out var remainder);
			return (new(result, null, mantissaLength), new(remainder, mantissaLength));
		}
		else if (x.e is null)
		{
			Debug.Assert(this2.e is not null);
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return (this2, new(0, mantissaLength));
			else if (this2.e <= x.m.DecLength)
				return (new((MantissaOverflow + this2.m).ShiftLeftDec((this2.e & -1) - 1).Divide(x.m, out var remainder),
					mantissaLength), new(remainder, mantissaLength));
			var quotient = (MantissaOverflow + this2.m).ShiftLeftDec(mantissaLength + 1) / x.m;
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return (new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				this2.e + (shiftAmount - mantissaLength - 1), mantissaLength), new(0, mantissaLength));
		}
		else if (this2.e is null || this2.e < x.e)
			return (new(0, mantissaLength), this2);
		else if (this2.e <= x.e + mantissaLength)
		{
			var eDiff = (this2.e - x.e) & -1;
			var quotient = (MantissaOverflow + this2.m).ShiftLeftDec(eDiff).Divide(MantissaOverflow + x.m, out var remainder);
			return (new(quotient, mantissaLength), new(remainder.ShiftLeftDec((x.e & -1) - 1), mantissaLength));
		}
		else
		{
			var quotient = (MantissaOverflow + this2.m).ShiftLeftDec(mantissaLength + 1) / (MantissaOverflow + x.m);
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return (new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				this2.e - x.e + (shiftAmount - mantissaLength), mantissaLength), new(0, mantissaLength));
		}
	}

	public UnsignedLongDecimal DivRem(MpuT x, out MpuT remainder)
	{
		(var Quotient, remainder) = DivRem(x);
		return Quotient;
	}

	public UnsignedLongDecimal DivRem(UnsignedLongDecimal x, out UnsignedLongDecimal remainder)
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

	public bool Equals(MpuT other)
	{
		if (e is null)
			return m.Equals(other);
		var decLength = other.DecLength;
		var eDiff = decLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return false;
		return (MantissaOverflow + m).ShiftLeftDec(eDiff - 1).Equals(other);
	}

	public bool Equals(MpzT other)
	{
		if (e is null)
			return m.Equals(other);
		var decLength = other.DecLength;
		var eDiff = decLength - MantissaLength;
		var eComparison = e.CompareTo(eDiff);
		if (eComparison != 0)
			return false;
		return (MantissaOverflow + m).ShiftLeftDec(eDiff - 1).Equals(other);
	}

	public bool Equals(UnsignedLongDecimal? other) => CompareTo(other) == 0;

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
		UnsignedLongDecimal uld => CompareTo(uld) == 0,
		BigInteger bi => CompareTo(new MpzT(bi)) == 0,
		IConvertible ic => ic.Equals(this),
		_ => false,
	};

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

	private UnsignedLongDecimal GetWithOtherML(int mantissaLength, bool copy) =>
		Compute(this, new((ulong)mantissaLength << 1 | (copy ? 1u : 0), null), ComputeOperation.ChangeML);

	public static bool IsCanonical(UnsignedLongDecimal value) => true;
	public static bool IsComplexNumber(UnsignedLongDecimal value) => true;
	public bool IsEven() => e is not null || (m & 1) == 0;
	public static bool IsEvenInteger(UnsignedLongDecimal value) => value.IsEven();
	public static bool IsFinite(UnsignedLongDecimal value) => true;
	public static bool IsImaginaryNumber(UnsignedLongDecimal value) => false;
	public static bool IsInfinity(UnsignedLongDecimal value) => false;
	public static bool IsInteger(UnsignedLongDecimal value) => true;
	public static bool IsNaN(UnsignedLongDecimal value) => false;
	public static bool IsNegative(UnsignedLongDecimal value) => false;
	public static bool IsNegativeInfinity(UnsignedLongDecimal value) => false;
	public static bool IsNormal(UnsignedLongDecimal value) => value.e is not null;
	public static bool IsOddInteger(UnsignedLongDecimal value) => !IsEvenInteger(value);
	public static bool IsPositive(UnsignedLongDecimal value) => true;
	public static bool IsPositiveInfinity(UnsignedLongDecimal value) => false;
	public static bool IsPow2(UnsignedLongDecimal value) => value.PopCount() == 1;
	public static bool IsRealNumber(UnsignedLongDecimal value) => true;
	public static bool IsSubnormal(UnsignedLongDecimal value) => value.e is null;
	public static bool IsZero(UnsignedLongDecimal value) => Mpir.MpzCmpSi(value.m, 0) == 0 && value.e is null;

	public static UnsignedLongDecimal Log2(UnsignedLongDecimal value)
	{
		var decLength = value.DecLength;
		var sqrt = (new UnsignedLongDecimal(1, value.MantissaByteLength) << decLength << decLength - 1).Sqrt();
		return value >= sqrt ? decLength : decLength - 1;
	}

	public static UnsignedLongDecimal Max(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongDecimal MaxMagnitude(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongDecimal MaxMagnitudeNumber(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongDecimal Min(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0 ? x : y;
	public static UnsignedLongDecimal MinMagnitude(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0 ? x : y;
	public static UnsignedLongDecimal MinMagnitudeNumber(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0 ? x : y;

	public static UnsignedLongDecimal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => new(s.ToString());
	public static UnsignedLongDecimal Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		new(s.ToString());
	public static UnsignedLongDecimal Parse(string? s) => new(s);
	public static UnsignedLongDecimal Parse(string s, IFormatProvider? provider) => new(s);
	public static UnsignedLongDecimal Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);
	public int PopCount() => m.PopCount() + (e is null ? 0 : 1);
	public static UnsignedLongDecimal PopCount(UnsignedLongDecimal value) => value.PopCount();
	static UnsignedLongDecimal IFloatingPoint<UnsignedLongDecimal>.Round(UnsignedLongDecimal x, int digits, MidpointRounding mode) => x;

	public UnsignedLongDecimal Sqrt()
	{
		if (e is null)
			return new(m.Sqrt());
		else if (e.IsEven())
			return new((MantissaOverflow + m).Sqrt().ShiftLeftDec(MantissaLength / 2) - MantissaOverflow, e >> 1);
		else
			return new((MantissaOverflow + m).ShiftLeftDec(MantissaLength + 1).Sqrt() - MantissaOverflow, e >> 1);
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
		m.val == 0 ? "0" : DecLength >= 65536 ? "Too large for short string, use ToString() instead." : ((MpuT)this).ToString();
	public override string? ToString() => ((MpuT)this).ToString(DefaultStringBase);
	public string ToString(IFormatProvider? provider) => ToString(DefaultStringBase) ?? "";
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString(DefaultStringBase));
	public string? ToString(uint @base) => ((MpuT)this).ToString(@base);

	object IConvertible.ToType(Type targetType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(targetType);
		if (targetType == typeof(UnsignedLongDecimal))
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
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(UnsignedLongDecimal)
			+ ", " + nameof(MpzT) + ", " + nameof(MpuT)
			+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	public static UnsignedLongDecimal TrailingZeroCount(UnsignedLongDecimal value) =>
		MpuT.TrailingZeroCount(value.m) + (value.e is null ? 0 : (value.e - 1));

	private static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out UnsignedLongDecimal result)
	{
		try
		{
			result = value switch
			{
				UnsignedLongDecimal uld => uld,
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
				float f => (UnsignedLongDecimal)f,
				double d => (UnsignedLongDecimal)d,
				decimal m => (UnsignedLongDecimal)(double)m,
				BigInteger ll => new(ll),
				string s => new(s),
				_ => throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(UnsignedLongDecimal)
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

	static bool INumberBase<UnsignedLongDecimal>.TryConvertFromChecked<TOther>(TOther value,
		[MaybeNullWhen(false)] out UnsignedLongDecimal result) =>
		TryConvertFromChecked(value, out result);

	static bool INumberBase<UnsignedLongDecimal>.TryConvertFromSaturating<TOther>(TOther value,
		[MaybeNullWhen(false)] out UnsignedLongDecimal result)
	{
		try
		{
			result = value switch
			{
				UnsignedLongDecimal uld => uld,
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

	static bool INumberBase<UnsignedLongDecimal>.TryConvertFromTruncating<TOther>(TOther value,
		[MaybeNullWhen(false)] out UnsignedLongDecimal result) =>
		TryConvertFromChecked(value, out result);

	private static bool TryConvertToChecked<TOther>(UnsignedLongDecimal value, out TOther result)
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

	static bool INumberBase<UnsignedLongDecimal>.TryConvertToChecked<TOther>(UnsignedLongDecimal value, out TOther result) =>
		TryConvertToChecked(value, out result);

	static bool INumberBase<UnsignedLongDecimal>.TryConvertToSaturating<TOther>(UnsignedLongDecimal value, out TOther result) =>
		TryConvertToChecked(value, out result);

	static bool INumberBase<UnsignedLongDecimal>.TryConvertToTruncating<TOther>(UnsignedLongDecimal value, out TOther result) =>
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
		[MaybeNullWhen(false)] out UnsignedLongDecimal result)
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
		[MaybeNullWhen(false)] out UnsignedLongDecimal result)
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

	public static bool TryParse(string? s, [MaybeNullWhen(false)] out UnsignedLongDecimal result)
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

	public static bool TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out UnsignedLongDecimal result)
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
		[MaybeNullWhen(false)] out UnsignedLongDecimal result)
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

	public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UnsignedLongDecimal value)
	{
		value = new(source, 1);
		return true;
	}

	public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UnsignedLongDecimal value)
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

	public static implicit operator UnsignedLongDecimal(byte value) => new((uint)value);
	public static implicit operator UnsignedLongDecimal(short value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongDecimal(ushort value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongDecimal(int value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongDecimal(uint value) => new(value);
	public static implicit operator UnsignedLongDecimal(long value) => new(value);
	public static implicit operator UnsignedLongDecimal(ulong value) => new(value);
	public static implicit operator UnsignedLongDecimal(MpzT value) => new(value);
	public static implicit operator UnsignedLongDecimal(MpuT value) => new(value);
	public static explicit operator UnsignedLongDecimal(float value) => new((double)value);
	public static explicit operator UnsignedLongDecimal(double value) => new(value);
	public static explicit operator UnsignedLongDecimal(decimal value) => new(value);
	public static explicit operator UnsignedLongDecimal(string value) => new(value, DefaultStringBase);
	public static explicit operator byte(UnsignedLongDecimal value) => (byte)(uint)value;
	public static explicit operator short(UnsignedLongDecimal value) => (short)(int)value;
	public static explicit operator ushort(UnsignedLongDecimal value) => (ushort)(uint)value;
	public static explicit operator int(UnsignedLongDecimal value) => value & -1;
	public static explicit operator uint(UnsignedLongDecimal value) => value & uint.MaxValue;
	public static explicit operator long(UnsignedLongDecimal value) => (long)(value & ulong.MaxValue).m;
	public static explicit operator ulong(UnsignedLongDecimal value) => (ulong)(value & ulong.MaxValue).m;
	public static explicit operator float(UnsignedLongDecimal value) => (float)(double)value;

	public static explicit operator double(UnsignedLongDecimal value)
	{
		if (value.DecLength > 309)
			return double.PositiveInfinity;
		else if (value.e is null)
			return (double)value.m;
		return (double)(value.MantissaOverflow + value.m).ShiftLeftDec((value.e & -1) - 1);
	}

	public static explicit operator decimal(UnsignedLongDecimal value)
	{
		if ((double)value is var x && x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN))
			return (decimal)x;
		else
			return 0m;
	}

	public static explicit operator string?(UnsignedLongDecimal value) => value.ToString();

	public static explicit operator MpzT(UnsignedLongDecimal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return new MpzT(value.MantissaOverflow + value.m) << (value.e & -1) - 1;
		else
			return 0;
	}

	public static explicit operator MpuT(UnsignedLongDecimal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return (value.MantissaOverflow + value.m).ShiftLeftDec((value.e & -1) - 1);
		else
			return 0;
	}

	public static UnsignedLongDecimal operator +(UnsignedLongDecimal value) => new(value);
	static UnsignedLongDecimal IUnaryNegationOperators<UnsignedLongDecimal, UnsignedLongDecimal>.operator -(UnsignedLongDecimal value) =>
		throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");
	static UnsignedLongDecimal IBitwiseOperators<UnsignedLongDecimal, UnsignedLongDecimal, UnsignedLongDecimal>.operator ~(UnsignedLongDecimal value) =>
		throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");

	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, int y) =>
		y >= 0 ? Compute(x, y, ComputeOperation.Add) : Compute(x, -y, ComputeOperation.Subtract);
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, UnsignedLongDecimal y) =>
		Compute(x, y, ComputeOperation.Add);
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, int y) =>
		y >= 0 ? Compute(x, y, ComputeOperation.Subtract) : Compute(x, -y, ComputeOperation.Add);
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, UnsignedLongDecimal y) =>
		Compute(x, y, ComputeOperation.Subtract);

	public static UnsignedLongDecimal operator *(int x, UnsignedLongDecimal y) => y * x;
	public static UnsignedLongDecimal operator *(uint x, UnsignedLongDecimal y) => y * x;

	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, int y)
	{
		var mantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		if (x.e is null)
			return new(x.m * y, mantissaLength);
		if (y == 0)
			return new(0, mantissaLength);
		else if (y == 1)
			return x.Copy();
		var product = (MantissaOverflow + x.m) * y;
		var shiftAmount = product.DecLength - mantissaLength - 1;
		var newE = Compute(x.e, shiftAmount, ComputeOperation.Add);
		newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
		return new(product.ShiftRightRoundDec(shiftAmount) - MantissaOverflow, newE, mantissaLength);
	}

	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, uint y)
	{
		var mantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		if (x.e is null)
			return new(x.m * y, mantissaLength);
		else
		{
			if (y == 0)
				return new(0, mantissaLength);
			else if (y == 1)
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - MantissaOverflow, x.e + shiftAmount, mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m * y.m, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				return new(0, mantissaLength);
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y.m;
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - MantissaOverflow, x.e + shiftAmount, mantissaLength);
		}
		else if (x.e is null)
		{
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				return new(0, mantissaLength);
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return y.Copy();
			var product = x.m * (MantissaOverflow + y.m);
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - MantissaOverflow, y.e + shiftAmount, mantissaLength);
		}
		else
		{
			var product = (MantissaOverflow + x.m) * (MantissaOverflow + y.m);
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				x.e + y.e + (shiftAmount - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, int y)
	{
		var mantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		if (x.e is null)
			return new(x.m / y, null, mantissaLength);
		else if (y == 0)
			throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
		else if (y == 1)
			return x.Copy();
		else if (x.e <= sizeof(int) * 8 - int.LeadingZeroCount(y))
			return new((MantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) / y, mantissaLength);
		var quotient = (MantissaOverflow + x.m).ShiftLeftDec(mantissaLength + 1) / y;
		var shiftAmount = quotient.DecLength - mantissaLength - 1;
		return new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
			new(x.e + (shiftAmount - mantissaLength - 1), mantissaLength), mantissaLength);
	}

	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, uint y)
	{
		var mantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		if (x.e is null)
			return new(x.m / y, null, mantissaLength);
		else if (y == 0)
			throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
		else if (y == 1)
			return x.Copy();
		else if (x.e <= sizeof(uint) * 8 - uint.LeadingZeroCount(y))
			return new((MantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) / y, mantissaLength);
		var quotient = (MantissaOverflow + x.m).ShiftLeftDec(mantissaLength + 1) / y;
		var shiftAmount = quotient.DecLength - mantissaLength - 1;
		return new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
			new(x.e + (shiftAmount - mantissaLength - 1), mantissaLength), mantissaLength);
	}

	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m / y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x.Copy();
			else if (x.e <= y.m.DecLength)
				return new((MantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) / y.m, mantissaLength);
			var quotient = (MantissaOverflow + x.m).ShiftLeftDec(mantissaLength + 1) / y.m;
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				x.e + (shiftAmount - mantissaLength - 1), mantissaLength);
		}
		else if (x.e is null || x.e < y.e)
			return new(0, mantissaLength);
		else
		{
			if (x.e <= y.e + (mantissaLength + 1))
				return new((MantissaOverflow + x.m).ShiftLeftDec((int)(x.e - y.e)) / (MantissaOverflow + y.m), mantissaLength);
			var quotient = (MantissaOverflow + x.m).ShiftLeftDec(mantissaLength + 2) / (MantissaOverflow + y.m);
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				x.e - y.e + (shiftAmount - mantissaLength - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator %(UnsignedLongDecimal x, MpuT y) => new(x.DivRem(y).Remainder, x.MantissaLength);

	public static UnsignedLongDecimal operator %(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.DivRem(y).Remainder;

	public static int operator &(UnsignedLongDecimal x, int y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > x.MantissaLength)
			return 0;
		else
			return (x.MantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) & y;
	}

	public static uint operator &(UnsignedLongDecimal x, uint y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1u) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > x.MantissaLength)
			return 0;
		else
			return (x.MantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) & y;
	}

	public static UnsignedLongDecimal operator &(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m & y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e > mantissaLength)
				return new(0, mantissaLength);
			return new((mantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) & y.m, mantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e > x.m.DecLength)
				return new(0, mantissaLength);
			return new(x.m & (mantissaOverflow + y.m).ShiftLeftDec((y.e & -1) - 1), mantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff > mantissaLength)
				return new(0, mantissaLength);
			if (x.DecLength > 300_000_000 || y.DecLength > 300_000_000)
				throw new OverflowException("Ошибка, эти числа слишком большие для этой операции!");
			return new((mantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1)
				& (mantissaOverflow + y.m).ShiftLeftDec((y.e & -1) - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator |(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m | y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= mantissaLength + 1)
				return x.Copy();
			return new((mantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) | y.m, mantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= mantissaLength + 1)
				return y.Copy();
			return new(x.m | (mantissaOverflow + y.m).ShiftLeftDec((y.e & -1) - 1), mantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff > mantissaLength)
				return y.Copy();
			if (x.DecLength > 300_000_000 || y.DecLength > 300_000_000)
				throw new OverflowException("Ошибка, эти числа слишком большие для этой операции!");
			return new((mantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1)
				| (mantissaOverflow + y.m).ShiftLeftDec((y.e & -1) - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator ^(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOfTen(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m ^ y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= mantissaLength + 1)
				return x.Copy();
			return new((mantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1) ^ y.m, mantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= mantissaLength + 1)
				return y.Copy();
			return new(x.m ^ (mantissaOverflow + y.m).ShiftLeftDec((y.e & -1) - 1), mantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff > mantissaLength)
				return y.Copy();
			if (x.DecLength > 300_000_000 || y.DecLength > 300_000_000)
				throw new OverflowException("Ошибка, эти числа слишком большие для этой операции!");
			return new((mantissaOverflow + x.m).ShiftLeftDec((x.e & -1) - 1)
				^ (mantissaOverflow + y.m).ShiftLeftDec((y.e & -1) - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator <<(UnsignedLongDecimal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x.Copy();
		else if (x.e is null)
			return new(x.m.ShiftLeftDec(shiftAmount), x.MantissaLength);
		else
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
	}

	public static UnsignedLongDecimal operator <<(UnsignedLongDecimal x, UnsignedLongDecimal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x.Copy();
		else if (x.e is not null)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (shiftAmount < x.MantissaLength)
			return new(x.m.ShiftLeftDec((int)shiftAmount), x.MantissaLength);
		return new UnsignedLongDecimal(x.m.ShiftLeftDec(x.MantissaLength), x.MantissaLength)
			<< shiftAmount - x.MantissaLength;
	}

	public static UnsignedLongDecimal operator >>(UnsignedLongDecimal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x.Copy();
		else if (x.e is null)
			return new(x.m.ShiftRightRoundDec(shiftAmount), null, x.MantissaLength);
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new((x.MantissaOverflow + x.m).ShiftRightRoundDec(shiftAmount - (x.e & -1) + 1), null, x.MantissaLength);
	}

	public static UnsignedLongDecimal operator >>(UnsignedLongDecimal x, UnsignedLongDecimal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x.Copy();
		else if (x.e is null)
		{
			if (shiftAmount > x.MantissaLength)
				return new(0, x.MantissaLength);
			return new(x.m.ShiftRightRoundDec((int)shiftAmount), null, x.MantissaLength);
		}
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		var restShiftAmount = shiftAmount - x.e;
		if (restShiftAmount > x.MantissaLength)
			return new(0, x.MantissaLength);
		else
			return new((x.MantissaOverflow + x.m).ShiftRightRoundDec((int)restShiftAmount + 1), null, x.MantissaLength);
	}

	public static UnsignedLongDecimal operator >>>(UnsignedLongDecimal x, int shiftAmount) => x >> shiftAmount;

	public static UnsignedLongDecimal operator >>>(UnsignedLongDecimal x, UnsignedLongDecimal shiftAmount) => x >> shiftAmount;

	public static UnsignedLongDecimal operator ++(UnsignedLongDecimal value)
	{
#pragma warning disable IDE0078 // Используйте сопоставление шаблонов
		if (value.e is not null && value.e >= 2)
			return value.Copy();
		else if (Mpir.MpuCmp(value.m, value.MantissaMask) == 0)
			return new(0, value.e is not null ? 2 : 1, value.MantissaLength);
		else
			return new(value.m + 1, value.e, value.MantissaLength);
#pragma warning restore IDE0078 // Используйте сопоставление шаблонов
	}

	public static UnsignedLongDecimal operator --(UnsignedLongDecimal value)
	{
		if (value.e is null)
			return new(value.m - 1, null, value.MantissaLength);
		var compTo2 = Mpir.MpuCmpSi(value.e.m, 2);
		if (Mpir.MpuCmpSi(value.m, 0) == 0 && value.e.e is null && compTo2 <= 0)
			return new(value.MantissaMask, compTo2 == 0 ? 1 : null, value.MantissaLength);
		else if (value.e.e is null && compTo2 < 0)
			return new(value.m - 1, value.e, value.MantissaLength);
		else
			return value.Copy();
	}

	public static bool operator ==(UnsignedLongDecimal x, int y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal x, int y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, int y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, int y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, int y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, int y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongDecimal x, uint y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal x, uint y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, uint y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, uint y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, uint y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, uint y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongDecimal x, long y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal x, long y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, long y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, long y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, long y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, long y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) < 0;
	public static bool operator ==(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) < 0;
	public static bool operator ==(int x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	public static bool operator !=(int x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	public static bool operator >=(int x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(int x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	public static bool operator >(int x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	public static bool operator <(int x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	public static bool operator ==(uint x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	public static bool operator !=(uint x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	public static bool operator >=(uint x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(uint x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	public static bool operator >(uint x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	public static bool operator <(uint x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	public static bool operator ==(long x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	public static bool operator !=(long x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	public static bool operator >=(long x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(long x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	public static bool operator >(long x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	public static bool operator <(long x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	public static bool operator ==(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	public static bool operator !=(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	public static bool operator >=(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	public static bool operator >(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	public static bool operator <(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	public static bool operator ==(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	public static bool operator !=(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	public static bool operator >=(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	public static bool operator >(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	public static bool operator <(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	public static bool operator ==(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	public static bool operator !=(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	public static bool operator >=(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	public static bool operator >(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	public static bool operator <(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	public static bool operator ==(UnsignedLongDecimal? x, UnsignedLongDecimal? y) => x?.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal? x, UnsignedLongDecimal? y) => x?.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0;
}
