namespace RedStarMath;

/// <summary>
/// Представляет число с плавающей точкой - как <see cref="UnsignedLongReal"/>,
/// только с десятичной экспонентой вместо двоичной, что гарантирует точность при десятичных операциях
/// (при достаточной длине мантиссы, разумеется!).
/// </summary>
[DebuggerDisplay("{ToShortString()}")]
public sealed class UnsignedLongDecimal : IUnsignedLongReal<UnsignedLongDecimal>
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
	internal readonly MpuT m;
	internal readonly UnsignedLongDecimal? e;
	internal readonly int MantissaLength = 0;
	public const int AutoMantissaLength = -1, DefaultMantissaLength = 3000, MinMantissaLength = 30;

	internal UnsignedLongDecimal(MpuT m, UnsignedLongDecimal? e, int mantissaLength = DefaultMantissaLength)
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
		? throw new ArgumentException(NoNegativeNumbers, nameof(op))
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
			return;
		}
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

	public UnsignedLongDecimal(UnsignedLongDecimal op) : this(op.m, op.e, op.MantissaLength) { }

	public UnsignedLongDecimal(UnsignedLongDecimal op, int mantissaLength)
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
			e = new(shiftAmount == 0 ? MpuT.One : new MpuT(shiftAmount + 1), null, mantissaLength);
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
				e += new UnsignedLongDecimal(new(shiftAmount), null, mantissaLength);
			else if (shiftAmount != 0)
				e = new(new(shiftAmount), null, mantissaLength);
		}
	}

	~UnsignedLongDecimal() => Dispose();

	public static UnsignedLongDecimal AdditiveIdentity => Zero;
	static UnsignedLongDecimal IFloatingPointConstants<UnsignedLongDecimal>.E => throw new NotSupportedException();
	UnsignedLongDecimal? IUnsignedLongReal<UnsignedLongDecimal>.Exponent => e;
	MpuT IUnsignedLongReal<UnsignedLongDecimal>.Mantissa => m;
	private int MantissaByteLength => (int)Math.Ceiling((MantissaLength + Math.Log10(9d)) * Math.Log(10d, 256d));
	int IUnsignedLongReal<UnsignedLongDecimal>.MantissaByteLength => MantissaByteLength;
	int IUnsignedLongReal<UnsignedLongDecimal>.MantissaLength => MantissaLength;
	private MpuT MantissaMask => MantissaMasks.GetOrAdd(MantissaLength, x => MpuT.PowerOf10(x) * 9u - 1u);
	private MpuT MantissaOverflow => MpuT.PowerOf10(MantissaLength);
	MpuT IUnsignedLongReal<UnsignedLongDecimal>.MantissaOverflow => MantissaOverflow;
	public static UnsignedLongDecimal MultiplicativeIdentity => One;
	static UnsignedLongDecimal ISignedNumber<UnsignedLongDecimal>.NegativeOne => throw new NotSupportedException();
	public static UnsignedLongDecimal One { get; } = new(1, MinMantissaLength);
	static UnsignedLongDecimal IFloatingPointConstants<UnsignedLongDecimal>.Pi => throw new NotSupportedException();
	public static int Radix => 10;
	static UnsignedLongDecimal IFloatingPointConstants<UnsignedLongDecimal>.Tau => throw new NotSupportedException();
	public static UnsignedLongDecimal Zero { get; } = new(0, MinMantissaLength);

	public UnsignedLongDecimal DecLength => Compute(this, null!, ComputeOperation.DecLength);

	public static UnsignedLongDecimal Abs(UnsignedLongDecimal op) => new(op.m, op.e);

	public object Clone() => new UnsignedLongDecimal(m, e, MantissaLength);

	/// <summary>
	/// Сравнивает данное число с <see langword="int"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(int other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see langword="uint"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(uint other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see langword="long"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(long other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see langword="ulong"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(ulong other)
	{
		if (e is not null)
			return 1;
		return m.CompareTo(other);
	}

	/// <summary>
	/// Сравнивает данное число с <see cref="MpuT"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
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

	/// <summary>
	/// Сравнивает данное число с <see cref="MpzT"/>.
	/// См. описание <see cref="CompareTo(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
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
		byte y => CompareTo((int)y),
		short si => CompareTo(si),
		ushort usi => CompareTo((int)usi),
		int i => CompareTo(i),
		uint ui => CompareTo(ui),
		long li => CompareTo(li),
		ulong uli => CompareTo(uli),
		MpzT z => CompareTo(z),
		MpuT uz => CompareTo(uz),
		UnsignedLongDecimal ulm => CompareTo(ulm),
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
				return new(new(x.m.DecLength), null, x.MantissaLength);
			else
				return Compute(x.e, new(new(x.MantissaLength), null, x.MantissaLength), ComputeOperation.Add);
			case ComputeOperation.Compare:
			if (y is null)
				return new(MpuTTwo, null);
			if (x.e is null && y.e is null)
				return new((MpuT)(Math.Sign(x.m.CompareTo(y.m)) + MpuT.One), null);
			if (x.e is null && y.e is not null
				&& (y.e.e is not null || Mpir.MpuCmpSi(y.e.m + (uint)y.MantissaLength, x.m.DecLength) > 0))
				return new(MpuT.Zero, null);
			if (y.e is null && x.e is not null
				&& (x.e.e is not null || Mpir.MpuCmpSi(x.e.m + (uint)x.MantissaLength, y.m.DecLength) > 0))
				return new(MpuTTwo, null);
			if (x.e is null && y.e is not null && y.e.e is null)
			{
				var shiftedSign = Math.Sign(x.m.ShiftRightDec((int)y.e.m - 1).CompareTo(y.MantissaOverflow + y.m));
				return new((MpuT)(shiftedSign + MpuT.One), null);
			}
			if (y.e is null && x.e is not null && x.e.e is null)
			{
				var shiftedSign = Math.Sign((x.MantissaOverflow + x.m).CompareTo(y.m.ShiftRightDec((int)x.e.m - 1)));
				return new((MpuT)(shiftedSign + MpuT.One), null);
			}
			var xDecLength = Compute(x, null!, ComputeOperation.DecLength);
			var yDecLength = Compute(y, null!, ComputeOperation.DecLength);
			var compared = Compute(xDecLength, yDecLength, ComputeOperation.Compare).m;
			if (Mpir.MpuCmpSi(compared, 1) != 0)
				return new(compared, null);
			var mlDiff = x.MantissaLength - y.MantissaLength;
			if (mlDiff >= 0)
				return new((MpuT)(Math.Sign(x.m.CompareTo(y.m.ShiftLeftDec(mlDiff))) + MpuT.One), null);
			else
				return new((MpuT)(Math.Sign(x.m.ShiftLeftDec(-mlDiff).CompareTo(y.m)) + MpuT.One), null);
			case ComputeOperation.ChangeML:
			var mantissaLength = (int)y.m >>> 1;
			if (mantissaLength == x.MantissaLength)
				return x;
			mlDiff = mantissaLength - x.MantissaLength;
			var xMantissaOverfow = x.MantissaOverflow;
			UnsignedLongDecimal newE;
			if (mlDiff > 0)
			{
				if (x.e is null)
					return new(x.m, mantissaLength);
				else if (Compute(x.e, mlDiff, ComputeOperation.Compare).m <= 1)
					return new((xMantissaOverfow + x.m).ShiftLeftDec((int)x.e - 1), mantissaLength);
				newE = Compute(x.e, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				newE = Compute(newE, new(new(mlDiff), null, mantissaLength), ComputeOperation.Subtract);
				return new(x.m.ShiftLeftDec(mlDiff), newE, mantissaLength);
			}
			else
			{
				mlDiff = -mlDiff;
				if (x.e is null)
					return new(x.m, mantissaLength);
				newE = Compute(x.e, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				newE = Compute(newE, new(new(mlDiff), null, mantissaLength), ComputeOperation.Add);
				return new(x.m.ShiftRightRoundDec(mlDiff), newE, mantissaLength);
			}
			case ComputeOperation.Add:
			mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
			if (x.e is null && Mpir.MpuCmpSi(x.m, 0) == 0)
				return Compute(y, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			if (y.e is null && Mpir.MpuCmpSi(y.m, 0) == 0)
				return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
			if (Mpir.MpuCmpSi(Compute(y, x, ComputeOperation.Compare).m, 1) > 0)
				(x, y) = (y, x);
			var mantissaOverflow = MpuT.PowerOf10(mantissaLength);
			var xmlDiff = mantissaLength - x.MantissaLength;
			var ymlDiff = mantissaLength - y.MantissaLength;
			xDecLength = Compute(x, null!, ComputeOperation.DecLength);
			yDecLength = Compute(y, null!, ComputeOperation.DecLength);
			var yMantissaOverflow = y.MantissaOverflow;
			var mantissaMask = mantissaOverflow * 9u - 1u;
			if (x.e is null || Mpir.MpuCmpSi(Compute(xDecLength, mantissaLength, ComputeOperation.Compare).m, 1) <= 0
				&& Mpir.MpuCmpSi(Compute(yDecLength, mantissaLength, ComputeOperation.Compare).m, 1) <= 0)
			{
				var mSum = (MpuT)x + (MpuT)y;
				if (Mpir.MpuCmp(mSum, mantissaMask) > 0)
					return new(mSum - mantissaOverflow, One, mantissaLength);
				return new(mSum, null, mantissaLength);
			}
			else if (y.e is null || Mpir.MpuCmpSi(Compute(yDecLength, mantissaLength, ComputeOperation.Compare).m, 1) <= 0)
			{
				if (xDecLength.e is not null || xDecLength.m.BitLength >= BitsPerInt)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var blDiff = (int)xDecLength - Math.Max(y.MantissaLength + 1, (int)yDecLength);
				if (blDiff > mantissaLength)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var ym = (y.e is null ? MpuT.Zero : yMantissaOverflow) + y.m;
				var mSum = x.m.ShiftLeftDec(xmlDiff) + ym.ShiftLeftDec(ymlDiff).ShiftRightRoundDec(blDiff);
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
			else if (Mpir.MpuCmpSi(Compute(x.e, y.e, ComputeOperation.Compare).m, 1) >= 0)
			{
				var blDiff = Compute(xDecLength, yDecLength, ComputeOperation.Subtract);
				MpuT mSum;
				if (blDiff.e is null && blDiff.m == 0)
				{
					newE = Compute(x.e, xmlDiff, ComputeOperation.Subtract);
					newE = Compute(newE, (long)mantissaLength << 1, ComputeOperation.ChangeML);
					mSum = x.m.ShiftLeftDec(xmlDiff) + mantissaOverflow + y.m;
					var changeE = Mpir.MpuCmp(mSum, mantissaMask) > 0;
					var newM = changeE ? (mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow : mSum;
					return new(newM, changeE ? Compute(newE, One, ComputeOperation.Add) : newE, mantissaLength);
				}
				if (Mpir.MpuCmpSi(Compute(blDiff, new(mantissaLength + 1), ComputeOperation.Compare).m, 1) > 0)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				mSum = x.m.ShiftLeftDec(xmlDiff)
					+ (yMantissaOverflow + y.m).ShiftLeftDec(ymlDiff).ShiftRightRoundDec((int)blDiff);
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
					var changeE = Mpir.MpuCmp(mSum, mantissaMask) > 0;
					var newM = changeE ? (mantissaOverflow + mSum).ShiftRightRoundDec(1) - mantissaOverflow : mSum;
					return new(newM, changeE ? Compute(newE, One, ComputeOperation.Add) : newE, mantissaLength);
				}
				var eDiff = Compute(y.e, x.e, ComputeOperation.Subtract);
				mSum = x.m + (yMantissaOverflow + y.m).ShiftLeftDec((int)eDiff);
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
			mantissaOverflow = MpuT.PowerOf10(mantissaLength);
			x = Compute(x, (long)mantissaLength << 1, ComputeOperation.ChangeML);
			y = Compute(y, (long)mantissaLength << 1, ComputeOperation.ChangeML);
			if (x.e is null && y.e is null)
				return new(x.m - y.m, null, mantissaLength);
			else if (y.e is null)
			{
				Debug.Assert(x.e is not null);
				if (Mpir.MpuCmpSi(Compute(x.e, (uint)mantissaLength + 1u, ComputeOperation.Compare).m, 1) >= 0)
					return Compute(x, (long)mantissaLength << 1 | 1, ComputeOperation.ChangeML);
				var mDiff = mantissaOverflow + x.m - y.m.ShiftRightRoundDec((int)x.e - 1);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff - mantissaOverflow, x.e, mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new(mDiff * 10u - mantissaOverflow, (int)x.e - 1, mantissaLength);
			}
			else if (x.e is null || Mpir.MpuCmpSi(Compute(x.e, y.e, ComputeOperation.Compare).m, 1) < 0)
				throw new OverflowException(NoNegativeNumbers);
			else if (Mpir.MpuCmpSi(Compute(x.e, Compute(y.e, One, ComputeOperation.Add), ComputeOperation.Compare).m, 1) > 0)
			{
				var eDiff = Compute(x.e, y.e, ComputeOperation.Subtract);
				if (Mpir.MpuCmpSi(Compute(eDiff, mantissaLength, ComputeOperation.Compare).m, 1) > 0)
					return x;
				var mDiff = mantissaOverflow + x.m - (mantissaOverflow + y.m).ShiftRightRoundDec((int)eDiff);
				if (Mpir.MpuCmp(mDiff, mantissaOverflow) >= 0)
					return new(mDiff - mantissaOverflow, x.e, mantissaLength);
				else if (x.e.e is null && x.e.m == 1)
					return new(mDiff, null, mantissaLength);
				else
					return new(mDiff * 10u - mantissaOverflow, Compute(x.e, One, ComputeOperation.Subtract), mantissaLength);
			}
			else if (Compute(x.e, y.e, ComputeOperation.Compare).m == 1)
			{
				var mDiff = x.m - y.m;
				if (mDiff == 0)
					return new(MpuT.Zero, null, mantissaLength);
				var shiftAmount = mantissaLength - mDiff.DecLength + 1;
				if (Mpir.MpuCmpSi(Compute(x.e, shiftAmount, ComputeOperation.Compare).m, 1) <= 0)
					return new(mDiff.ShiftLeftDec((int)x.e - 1), null);
				return new(mDiff.ShiftLeftDec(shiftAmount) - mantissaOverflow,
					Compute(x.e, shiftAmount, ComputeOperation.Subtract), mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + x.m) * 10u - (mantissaOverflow + y.m);
				var shiftAmount = mantissaLength - mDiff.DecLength + 1;
				if (shiftAmount == -1)
					return new(mDiff.ShiftRightRoundDec(1) - mantissaOverflow, x.e, mantissaLength);
				return new(mDiff.ShiftLeftDec(shiftAmount) - mantissaOverflow,
					Compute(x.e, new UnsignedLongDecimal(shiftAmount + 1), ComputeOperation.Subtract), mantissaLength);
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
				throw new DivideByZeroException(NoDivisionByZero);
			else if (Mpir.MpuCmpSi(x, 1) == 0)
				return (this, MpuT.Zero);
			else if (e <= x.DecLength + 1)
				return (new((mantissaOverflow + m).ShiftLeftDec((int)e - 1).Divide(x, out var remainder),
					MantissaLength), remainder);
			var quotient = (mantissaOverflow + m).ShiftLeftDec(MantissaLength + 1) / x;
			var shiftAmount = quotient.DecLength - MantissaLength - 1;
			return (new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				e + new UnsignedLongDecimal(shiftAmount) - new UnsignedLongDecimal(MantissaLength + 1),
				MantissaLength), MpuT.Zero);
		}
		else if (e is null || e < x.DecLength - MantissaLength - 1)
			return (new(MpuT.Zero, null, MantissaLength), (MpuT)this);
		else if (e <= x.DecLength + 1)
			return (new((MantissaOverflow + m).ShiftLeftDec((int)e - 1).Divide(x, out var remainder), null,
				MantissaLength), remainder);
		else
		{
			var quotient = (MantissaOverflow + m).ShiftLeftDec((int)e) / (x * 10u);
			var shiftAmount = quotient.DecLength - MantissaLength;
			return (new(quotient.ShiftRightRoundDec(shiftAmount - 1) - MantissaOverflow, shiftAmount, MantissaLength), MpuT.Zero);
		}
	}

	/// <summary>
	/// Возвращает частное и остаток от деления данного числа на указанное число типа <see cref="UnsignedLongDecimal"/>.
	/// </summary>
	/// <param name="x">Делитель.</param>
	/// <returns>Кортеж, содержащий частное в первом элементе и остаток во втором.</returns>
	public (UnsignedLongDecimal Quotient, UnsignedLongDecimal Remainder) DivRem(UnsignedLongDecimal x)
	{
		var mantissaLength = Math.Max(MantissaLength, x.MantissaLength);
		var mantissaOverflow = MantissaOverflows.GetOrAdd(mantissaLength, MpuT.PowerOf10);
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
				throw new DivideByZeroException(NoDivisionByZero);
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return (this2, new(0, mantissaLength));
			else if (this2.e <= x.m.DecLength)
				return (new((mantissaOverflow + this2.m).ShiftLeftDec((int)this2.e - 1).Divide(x.m, out var remainder),
					mantissaLength), new(remainder, mantissaLength));
			var quotient = (mantissaOverflow + this2.m).ShiftLeftDec(mantissaLength + 1) / x.m;
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return (new(quotient.ShiftRightRoundDec(shiftAmount) - mantissaOverflow,
				this2.e + new UnsignedLongDecimal(shiftAmount) - new UnsignedLongDecimal(mantissaLength + 1), mantissaLength),
				new(0, mantissaLength));
		}
		else if (this2.e is null || this2.e < x.e)
			return (new(0, mantissaLength), this2);
		else if (this2.e <= x.e + mantissaLength)
		{
			var eDiff = (int)(this2.e - x.e);
			var quotient = (mantissaOverflow + this2.m).ShiftLeftDec(eDiff).Divide(mantissaOverflow + x.m, out var remainder);
			return (new(quotient, mantissaLength), new(remainder.ShiftLeftDec((int)x.e - 1), mantissaLength));
		}
		else
		{
			var quotient = (mantissaOverflow + this2.m).ShiftLeftDec(mantissaLength + 1) / (mantissaOverflow + x.m);
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return (new(quotient.ShiftRightRoundDec(shiftAmount) - mantissaOverflow,
				this2.e - x.e + new UnsignedLongDecimal(shiftAmount) - new UnsignedLongDecimal(mantissaLength),
				mantissaLength), new(0, mantissaLength));
		}
	}

	/// <summary>
	/// Возвращает частное и остаток от деления данного числа на указанное число типа <see cref="MpuT"/>.
	/// </summary>
	/// <param name="x">Делитель.</param>
	/// <param name="remainder"><see langword="out"/>-параметр, в который помещается остаток от деления.</param>
	/// <returns>Эта перегрузка метода возвращает только частное,
	/// помещая остаток в <see langword="out"/>-параметр <paramref name="remainder"/>.</returns>
	public UnsignedLongDecimal DivRem(MpuT x, out MpuT remainder)
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
	public UnsignedLongDecimal DivRem(UnsignedLongDecimal x, out UnsignedLongDecimal remainder)
	{
		(var Quotient, remainder) = DivRem(x);
		return Quotient;
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="int"/>.
	/// См. описание <see cref="Equals(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(int other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="uint"/>.
	/// См. описание <see cref="Equals(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(uint other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="long"/>.
	/// См. описание <see cref="Equals(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(long other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="ulong"/>.
	/// См. описание <see cref="Equals(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(ulong other)
	{
		if (e is not null)
			return false;
		return m.Equals(other);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see cref="MpuT"/>.
	/// См. описание <see cref="Equals(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
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

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see cref="MpzT"/>.
	/// См. описание <see cref="Equals(UnsignedLongDecimal)"/> для более подробных сведений.
	/// </summary>
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
		byte y => CompareTo((int)y) == 0,
		short si => CompareTo(si) == 0,
		ushort usi => CompareTo((int)usi) == 0,
		int i => CompareTo(i) == 0,
		uint ui => CompareTo(ui) == 0,
		long li => CompareTo(li) == 0,
		ulong uli => CompareTo(uli) == 0,
		MpzT z => CompareTo(z) == 0,
		MpuT uz => CompareTo(uz) == 0,
		UnsignedLongDecimal ulm => CompareTo(ulm) == 0,
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

	internal UnsignedLongDecimal GetWithOtherML(int mantissaLength, bool copy) =>
		Compute(this, new(new((ulong)mantissaLength << 1 | (copy ? 1u : 0)), null), ComputeOperation.ChangeML);

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
	public static bool IsZero(UnsignedLongDecimal value) => Mpir.MpuCmpSi(value.m, 0) == 0 && value.e is null;

	public static UnsignedLongDecimal Log2(UnsignedLongDecimal value)
	{
		var decLength = value.DecLength;
		var sqrt = (new UnsignedLongDecimal(One, value.MantissaByteLength) << decLength << decLength - 1u).Sqrt();
		return value >= sqrt ? decLength : decLength - 1u;
	}

	public static UnsignedLongDecimal Max(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongDecimal MaxMagnitude(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongDecimal MaxMagnitudeNumber(UnsignedLongDecimal x, UnsignedLongDecimal y) =>
		x.CompareTo(y) >= 0 ? x : y;
	public static UnsignedLongDecimal Min(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0 ? x : y;
	public static UnsignedLongDecimal MinMagnitude(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0 ? x : y;
	public static UnsignedLongDecimal MinMagnitudeNumber(UnsignedLongDecimal x, UnsignedLongDecimal y) =>
		x.CompareTo(y) < 0 ? x : y;

	public static UnsignedLongDecimal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => new(s.ToString());
	public static UnsignedLongDecimal Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		new(s.ToString());
	public static UnsignedLongDecimal Parse(string? s) => new(s);
	public static UnsignedLongDecimal Parse(string s, IFormatProvider? provider) => new(s);
	public static UnsignedLongDecimal Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);
	public int PopCount() => m.PopCount() + (e is null ? 0 : 1);
	public static UnsignedLongDecimal PopCount(UnsignedLongDecimal value) => value.PopCount();
	static UnsignedLongDecimal IFloatingPoint<UnsignedLongDecimal>.Round(UnsignedLongDecimal x, int digits, MidpointRounding mode) => x;

	/// <summary>Возвращает квадратный корень данного числа (округленный вниз).</summary>
	public UnsignedLongDecimal Sqrt()
	{
		if (e is null)
			return new(m.Sqrt());
		else if (e.IsEven())
			return new((MantissaOverflow + m).Sqrt().ShiftLeftDec(MantissaLength / 2) - MantissaOverflow, e >> 1);
		else
			return new((MantissaOverflow + m).ShiftLeftDec(MantissaLength + 1).Sqrt() - MantissaOverflow, e >> 1);
	}

	bool IConvertible.ToBoolean(IFormatProvider? provider) => CompareTo(One) >= 0;
	byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

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
	/// (30000 десятичных цифр для данного типа).
	/// </summary>
	/// <returns>Результат преобразования в строку или строка-заглушка, если число слишком большое.</returns>
	public string? ToShortString()
	{
		if (m is null)
			return (string?)"0";
		else if (DecLength >= 30000)
			return (string?)"Too large for short string, use ToString() instead.";
		else
			return ((MpuT)this).ToString();
	}

	public override string? ToString() => ((MpuT)this).ToString(DefaultStringBase);
	public string ToString(IFormatProvider? provider) => ToString(DefaultStringBase) ?? "";
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString(DefaultStringBase));
	public string? ToString(uint @base) => ((MpuT)this).ToString(@base);

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	public static UnsignedLongDecimal TrailingZeroCount(UnsignedLongDecimal value) =>
		MpuT.TrailingZeroCount(value.m) + (value.e is null ? Zero : (value.e - One));

	private static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out UnsignedLongDecimal result)
	{
		try
		{
			result = value switch
			{
				UnsignedLongDecimal ulm => ulm,
				MpzT z => (UnsignedLongDecimal)z,
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
				UnsignedLongDecimal ulm => ulm,
				MpzT z => (UnsignedLongDecimal)z,
				MpuT uz => uz,
				byte y => y,
				sbyte sy => sy,
				short si => si,
				ushort usi => usi,
				int i => i,
				uint ui => ui,
				long li => li,
				ulong uli => uli,
				float f => (MpuT)(MathF.Ceiling(MathF.Abs(f)) * MathF.Sign(f)),
				double d => (MpuT)(Math.Ceiling(Math.Abs(d)) * Math.Sign(d)),
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

	/// <inheritdoc cref="TryWriteBigEndian(Span{byte}, out int)"/>
	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength) =>
		((IUnsignedLongReal<UnsignedLongDecimal>)this)
		.TryWriteBigEndianInterface(destination, out bytesWritten, saveMantissaLength);

	public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? Zero : e).TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? Zero : e).TryWriteLittleEndian(destination, out bytesWritten);

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteLittleEndian(destination, out bytesWritten, true);

	/// <inheritdoc cref="TryWriteLittleEndian(Span{byte}, out int)"/>
	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength) =>
		((IUnsignedLongReal<UnsignedLongDecimal>)this)
		.TryWriteLittleEndianInterface(destination, out bytesWritten, saveMantissaLength);

	public bool TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) =>
		m.TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) =>
		m.TryWriteLittleEndian(destination, out bytesWritten);

	public static implicit operator UnsignedLongDecimal(byte value) => new((uint)value);
	public static implicit operator UnsignedLongDecimal(short value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongDecimal(ushort value) => new((uint)value, MinMantissaLength);
	public static implicit operator UnsignedLongDecimal(int value) => new(value, MinMantissaLength);
	public static implicit operator UnsignedLongDecimal(uint value) => new(value);
	public static implicit operator UnsignedLongDecimal(long value) => new(value);
	public static implicit operator UnsignedLongDecimal(ulong value) => new(value);
	public static explicit operator UnsignedLongDecimal(MpzT value) => new(value);
	public static implicit operator UnsignedLongDecimal(MpuT value) => new(value);
	public static explicit operator UnsignedLongDecimal(float value) => new((double)value);
	public static explicit operator UnsignedLongDecimal(double value) => new(value);
	public static explicit operator UnsignedLongDecimal(decimal value) => new(value);
	public static explicit operator UnsignedLongDecimal(string value) => new(value, DefaultStringBase);
	public static explicit operator byte(UnsignedLongDecimal value) => (byte)(uint)value;
	public static explicit operator short(UnsignedLongDecimal value) => (short)(int)value;
	public static explicit operator ushort(UnsignedLongDecimal value) => (ushort)(uint)value;
	public static explicit operator int(UnsignedLongDecimal value) => (int)(uint)value;
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
		return (double)(value.MantissaOverflow + value.m).ShiftLeftDec((int)value.e - 1);
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
			return new MpzT(value.MantissaOverflow + value.m).ShiftLeftDec((int)value.e - 1);
		else
			return 0;
	}

	public static explicit operator MpuT(UnsignedLongDecimal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return (value.MantissaOverflow + value.m).ShiftLeftDec((int)value.e - 1);
		else
			return MpuT.Zero;
	}

	public static UnsignedLongDecimal operator +(UnsignedLongDecimal value) => new(value);
	public static LongDecimal operator -(UnsignedLongDecimal value) => -new LongDecimal(value, value.MantissaLength);
	static UnsignedLongDecimal IUnaryNegationOperators<UnsignedLongDecimal, UnsignedLongDecimal>.operator -(UnsignedLongDecimal value) =>
		throw new NotSupportedException(NoNegativeNumbers);
	public static LongDecimal operator ~(UnsignedLongDecimal value) => ~new LongDecimal(value, value.MantissaLength);
	static UnsignedLongDecimal IBitwiseOperators<UnsignedLongDecimal, UnsignedLongDecimal, UnsignedLongDecimal>.operator ~(UnsignedLongDecimal value) =>
		throw new NotSupportedException(NoNegativeNumbers);

	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(byte x, UnsignedLongDecimal y) => (uint)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, byte y) => x + (uint)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(short x, UnsignedLongDecimal y) => (int)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, short y) => x + (int)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(ushort x, UnsignedLongDecimal y) => (uint)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, ushort y) => x + (uint)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(int x, UnsignedLongDecimal y) => x + (LongDecimal)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, int y) => (LongDecimal)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(uint x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, uint y) => x + new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(long x, UnsignedLongDecimal y) => x + (LongDecimal)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, long y) => (LongDecimal)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(ulong x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, ulong y) => x + new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(MpzT x, UnsignedLongDecimal y) => x + (LongDecimal)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, MpzT y) => (LongDecimal)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(MpuT x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, MpuT y) => x + new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(double x, UnsignedLongDecimal y) => x + (LongDecimal)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, double y) => (LongDecimal)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(decimal x, UnsignedLongDecimal y) => x + (LongDecimal)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, decimal y) => (LongDecimal)x + y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongReal x, UnsignedLongDecimal y) => x + (LongDecimal)y;
	/// <inheritdoc cref="operator +(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, UnsignedLongReal y) => (LongDecimal)x + y;
	public static UnsignedLongDecimal operator +(UnsignedLongDecimal x, UnsignedLongDecimal y) =>
		Compute(x, y, ComputeOperation.Add);
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(byte x, UnsignedLongDecimal y) => (uint)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, byte y) => x - (uint)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(short x, UnsignedLongDecimal y) => (int)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, short y) => x - (int)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(ushort x, UnsignedLongDecimal y) => (uint)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, ushort y) => x - (uint)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(int x, UnsignedLongDecimal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, int y) => (LongDecimal)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(uint x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, uint y) => x - new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(long x, UnsignedLongDecimal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, long y) => (LongDecimal)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(ulong x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, ulong y) => x - new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(MpzT x, UnsignedLongDecimal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, MpzT y) => (LongDecimal)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(MpuT x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, MpuT y) => x - new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(double x, UnsignedLongDecimal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, double y) => (LongDecimal)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(decimal x, UnsignedLongDecimal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, decimal y) => (LongDecimal)x - y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongReal x, UnsignedLongDecimal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, UnsignedLongReal y) => (LongDecimal)x - y;
	public static UnsignedLongDecimal operator -(UnsignedLongDecimal x, UnsignedLongDecimal y) =>
		Compute(x, y, ComputeOperation.Subtract);

	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(byte x, UnsignedLongDecimal y) => y * x;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(short x, UnsignedLongDecimal y) => y * x;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(ushort x, UnsignedLongDecimal y) => y * x;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(int x, UnsignedLongDecimal y) => y * x;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(uint x, UnsignedLongDecimal y) => y * x;

	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, byte y) => x * (uint)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, short y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, ushort y) => x * (uint)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, int y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(long x, UnsignedLongDecimal y) => x * (LongDecimal)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, long y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(ulong x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, ulong y) => x * new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(MpzT x, UnsignedLongDecimal y) => x * (LongDecimal)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, MpzT y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(MpuT x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, MpuT y) => x * new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(double x, UnsignedLongDecimal y) => x * (LongDecimal)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, double y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(decimal x, UnsignedLongDecimal y) => x * (LongDecimal)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, decimal y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongReal x, UnsignedLongDecimal y) => x * (LongDecimal)y;
	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, UnsignedLongReal y) => (LongDecimal)x * y;

	/// <inheritdoc cref="operator *(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, uint y)
	{
		var mantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.PowerOf10(mantissaLength);
		if (x.e is null)
			return new(x.m * y, mantissaLength);
		else
		{
			if (y == 0)
				return new(0, mantissaLength);
			else if (y == 1)
				return x;
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				x.e + new UnsignedLongDecimal(shiftAmount), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator *(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOf10(mantissaLength);
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
				return x;
			var product = (mantissaOverflow + x.m) * y.m;
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - mantissaOverflow,
				x.e + new UnsignedLongDecimal(shiftAmount), mantissaLength);
		}
		else if (x.e is null)
		{
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				return new(0, mantissaLength);
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return y;
			var product = x.m * (mantissaOverflow + y.m);
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - mantissaOverflow,
				y.e + new UnsignedLongDecimal(shiftAmount), mantissaLength);
		}
		else
		{
			var product = (mantissaOverflow + x.m) * (mantissaOverflow + y.m);
			var shiftAmount = product.DecLength - mantissaLength - 1;
			return new(product.ShiftRightRoundDec(shiftAmount) - mantissaOverflow,
				x.e + y.e + new UnsignedLongDecimal(shiftAmount - 1), mantissaLength);
		}
	}

	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static byte operator /(byte x, UnsignedLongDecimal y) => (byte)(x / (y & uint.MaxValue));
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, byte y) => x / (uint)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static short operator /(short x, UnsignedLongDecimal y) => (short)(x / (y & uint.MaxValue));
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, short y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static ushort operator /(ushort x, UnsignedLongDecimal y) => (ushort)(x / (y & uint.MaxValue));
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, ushort y) => x / (uint)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(int x, UnsignedLongDecimal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, int y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(uint x, UnsignedLongDecimal y) => x / (y & uint.MaxValue);
	public static LongDecimal operator /(long x, UnsignedLongDecimal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, long y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(ulong x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, ulong y) => x / new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(MpzT x, UnsignedLongDecimal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, MpzT y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(MpuT x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, MpuT y) => x / new UnsignedLongDecimal(y);
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(double x, UnsignedLongDecimal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, double y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(decimal x, UnsignedLongDecimal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, decimal y) => (LongDecimal)x / y;

	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, uint y)
	{
		var mantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.PowerOf10(mantissaLength);
		if (x.e is null)
			return new(x.m / y, null, mantissaLength);
		else if (y == 0)
			throw new DivideByZeroException(NoDivisionByZero);
		else if (y == 1)
			return x;
		else if (x.e <= BitsPerInt - uint.LeadingZeroCount(y))
			return new((MantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) / y, mantissaLength);
		var quotient = (MantissaOverflow + x.m).ShiftLeftDec(mantissaLength + 1) / y;
		var shiftAmount = quotient.DecLength - mantissaLength - 1;
		return new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
			new(x.e + new UnsignedLongDecimal(shiftAmount) - new UnsignedLongDecimal(mantissaLength + 1),
			mantissaLength), mantissaLength);
	}

	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongReal x, UnsignedLongDecimal y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, UnsignedLongReal y) => (LongDecimal)x / y;

	public static UnsignedLongDecimal operator /(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var MantissaOverflow = MpuT.PowerOf10(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m / y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				throw new DivideByZeroException(NoDivisionByZero);
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x;
			else if (x.e <= y.m.DecLength)
				return new((MantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) / y.m, mantissaLength);
			var quotient = (MantissaOverflow + x.m).ShiftLeftDec(mantissaLength + 1) / y.m;
			var shiftAmount = quotient.DecLength - mantissaLength - 1;
			return new(quotient.ShiftRightRoundDec(shiftAmount) - MantissaOverflow,
				x.e + new UnsignedLongDecimal(shiftAmount) - new UnsignedLongDecimal(mantissaLength + 1), mantissaLength);
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
				x.e - y.e + new UnsignedLongDecimal(shiftAmount) - new UnsignedLongDecimal(mantissaLength + 1), mantissaLength);
		}
	}

	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static byte operator %(byte x, UnsignedLongDecimal y) => (byte)(y > x ? 0 : x % (byte)y);
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static byte operator %(UnsignedLongDecimal x, byte y) => (byte)(x % new MpuT((uint)y));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static short operator %(short x, UnsignedLongDecimal y) => (short)(y > x.ToUnsigned() ? 0 : x % (short)y);
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static short operator %(UnsignedLongDecimal x, short y) => (short)(y >= 0 ? x % new MpuT(y) : -(int)(x % new MpuT(-y)));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static ushort operator %(ushort x, UnsignedLongDecimal y) => (ushort)(y > x ? 0 : x % (ushort)y);
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static ushort operator %(UnsignedLongDecimal x, ushort y) => (ushort)(x % new MpuT((uint)y));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static int operator %(int x, UnsignedLongDecimal y) => y > x.ToUnsigned() ? 0 : x % (int)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static int operator %(UnsignedLongDecimal x, int y) => y >= 0 ? (int)(x % new MpuT(y)) : -(int)(x % new MpuT(-y));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static uint operator %(uint x, UnsignedLongDecimal y) => y > x ? 0 : x % (uint)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static uint operator %(UnsignedLongDecimal x, uint y) => (uint)(x % new MpuT(y));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static long operator %(long x, UnsignedLongDecimal y) => y > x.ToUnsigned() ? 0 : x % (long)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static long operator %(UnsignedLongDecimal x, long y) => y >= 0 ? (long)(x % new MpuT(y)) : -(long)(x % new MpuT(-y));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static ulong operator %(ulong x, UnsignedLongDecimal y) => (ulong)(new UnsignedLongDecimal(x) % y);
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static ulong operator %(UnsignedLongDecimal x, ulong y) => (ulong)(x % new MpuT(y));
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(MpzT x, UnsignedLongDecimal y) => x % (LongDecimal)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(UnsignedLongDecimal x, MpzT y) => (LongDecimal)x % y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator %(MpuT x, UnsignedLongDecimal y) => new UnsignedLongDecimal(x) % y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(double x, UnsignedLongDecimal y) => x % (LongDecimal)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(UnsignedLongDecimal x, double y) => (LongDecimal)x % y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(decimal x, UnsignedLongDecimal y) => x % (LongDecimal)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(UnsignedLongDecimal x, decimal y) => (LongDecimal)x % y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static UnsignedLongDecimal operator %(UnsignedLongDecimal x, MpuT y) => new(x.DivRem(y).Remainder, x.MantissaLength);
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(UnsignedLongReal x, UnsignedLongDecimal y) => x % (LongDecimal)y;
	/// <inheritdoc cref="operator %(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static LongDecimal operator %(UnsignedLongDecimal x, UnsignedLongReal y) => (LongDecimal)x % y;

	public static UnsignedLongDecimal operator %(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.DivRem(y).Remainder;

	/// <inheritdoc cref="operator &(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static int operator &(UnsignedLongDecimal x, int y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > x.MantissaLength)
			return 0;
		else
			return (x.MantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) & y;
	}

	/// <inheritdoc cref="operator &(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static uint operator &(UnsignedLongDecimal x, uint y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1u) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e > x.MantissaLength)
			return 0;
		else
			return (x.MantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) & y;
	}

	public static UnsignedLongDecimal operator &(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOf10(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m & y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e > mantissaLength)
				return new(0, mantissaLength);
			return new((mantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) & y.m, mantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e > x.m.DecLength)
				return new(0, mantissaLength);
			return new(x.m & (mantissaOverflow + y.m).ShiftLeftDec((int)y.e - 1), mantissaLength);
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
			return new((mantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1)
				& (mantissaOverflow + y.m).ShiftLeftDec((int)y.e - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator |(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOf10(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m | y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= mantissaLength + 1)
				return x;
			return new((mantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) | y.m, mantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= mantissaLength + 1)
				return y;
			return new(x.m | (mantissaOverflow + y.m).ShiftLeftDec((int)y.e - 1), mantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff > mantissaLength)
				return y;
			if (x.DecLength > 300_000_000 || y.DecLength > 300_000_000)
				throw new OverflowException("Ошибка, эти числа слишком большие для этой операции!");
			return new((mantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1)
				| (mantissaOverflow + y.m).ShiftLeftDec((int)y.e - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator ^(UnsignedLongDecimal x, UnsignedLongDecimal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		var mantissaOverflow = MpuT.PowerOf10(mantissaLength);
		x = x.GetWithOtherML(mantissaLength, false);
		y = y.GetWithOtherML(mantissaLength, false);
		if (x.e is null && y.e is null)
			return new(x.m ^ y.m, null, mantissaLength);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= mantissaLength + 1)
				return x;
			return new((mantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1) ^ y.m, mantissaLength);
		}
		else if (x.e is null)
		{
			if (y.e >= mantissaLength + 1)
				return y;
			return new(x.m ^ (mantissaOverflow + y.m).ShiftLeftDec((int)y.e - 1), mantissaLength);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff > mantissaLength)
				return y;
			if (x.DecLength > 300_000_000 || y.DecLength > 300_000_000)
				throw new OverflowException("Ошибка, эти числа слишком большие для этой операции!");
			return new((mantissaOverflow + x.m).ShiftLeftDec((int)x.e - 1)
				^ (mantissaOverflow + y.m).ShiftLeftDec((int)y.e - 1), mantissaLength);
		}
	}

	public static UnsignedLongDecimal operator <<(UnsignedLongDecimal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x;
		else if (x.e is null)
			return new(x.m.ShiftLeftDec(shiftAmount), x.MantissaLength);
		else
			return new(x.m, x.e + new UnsignedLongDecimal(shiftAmount), x.MantissaLength);
	}

	/// <inheritdoc cref="operator {{(UnsignedLongDecimal, int)"/>
	public static UnsignedLongDecimal operator <<(UnsignedLongDecimal x, UnsignedLongDecimal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x;
		else if (x.e is not null)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (shiftAmount < x.MantissaLength)
			return new(x.m.ShiftLeftDec((int)shiftAmount), x.MantissaLength);
		return new UnsignedLongDecimal(x.m.ShiftLeftDec(x.MantissaLength), x.MantissaLength)
			<< shiftAmount - (uint)x.MantissaLength;
	}

	public static UnsignedLongDecimal operator >>(UnsignedLongDecimal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x;
		else if (x.e is null)
			return new(x.m.ShiftRightRoundDec(shiftAmount), null, x.MantissaLength);
		else if (x.e > shiftAmount)
			return new(x.m, x.e - new UnsignedLongDecimal(shiftAmount), x.MantissaLength);
		else
			return new((x.MantissaOverflow + x.m).ShiftRightRoundDec(shiftAmount - (int)x.e + 1), null, x.MantissaLength);
	}

	/// <inheritdoc cref="operator }}(UnsignedLongDecimal, int)"/>
	public static UnsignedLongDecimal operator >>(UnsignedLongDecimal x, UnsignedLongDecimal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x;
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

	/// <inheritdoc cref="operator }}}(UnsignedLongDecimal, int)"/>
	public static UnsignedLongDecimal operator >>>(UnsignedLongDecimal x, UnsignedLongDecimal shiftAmount) => x >> shiftAmount;

	public static UnsignedLongDecimal operator ++(UnsignedLongDecimal value)
	{
#pragma warning disable IDE0078 // Используйте сопоставление шаблонов
		if (value.e is not null && value.e >= 2)
			return value;
		else if (Mpir.MpuCmp(value.m, value.MantissaMask) == 0)
			return new(MpuT.Zero, value.e is not null ? 2 : One, value.MantissaLength);
		else
			return new(value.m + 1u, value.e, value.MantissaLength);
#pragma warning restore IDE0078 // Используйте сопоставление шаблонов
	}

	public static UnsignedLongDecimal operator --(UnsignedLongDecimal value)
	{
		if (value.e is null)
			return new(value.m - 1u, null, value.MantissaLength);
		var compTo2 = Mpir.MpuCmpSi(value.e.m, 2);
		if (Mpir.MpuCmpSi(value.m, 0) == 0 && value.e.e is null && compTo2 <= 0)
			return new(value.MantissaMask, compTo2 == 0 ? One : null, value.MantissaLength);
		else if (value.e.e is null && compTo2 < 0)
			return new(value.m - 1u, value.e, value.MantissaLength);
		else
			return value;
	}

	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, int y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, int y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, int y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, int y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, int y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, int y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, uint y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, uint y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, uint y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, uint y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, uint y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, uint y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, long y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, long y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, long y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, long y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, long y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, long y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, ulong y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, MpzT y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, MpuT y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongDecimal x, UnsignedLongReal y) => ((LongDecimal)x).CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongDecimal x, UnsignedLongReal y) => ((LongDecimal)x).CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongDecimal x, UnsignedLongReal y) => ((LongDecimal)x).CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongDecimal x, UnsignedLongReal y) => ((LongDecimal)x).CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongDecimal x, UnsignedLongReal y) => ((LongDecimal)x).CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongDecimal x, UnsignedLongReal y) => ((LongDecimal)x).CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(int x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(int x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(int x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(int x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(int x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(int x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(uint x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(uint x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(uint x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(uint x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(uint x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(uint x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(long x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(long x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(long x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(long x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(long x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(long x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(ulong x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(MpzT x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(MpuT x, UnsignedLongDecimal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator ==(UnsignedLongReal x, UnsignedLongDecimal y) => ((LongDecimal)y).CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(UnsignedLongDecimal?, UnsignedLongDecimal?)"/>
	public static bool operator !=(UnsignedLongReal x, UnsignedLongDecimal y) => ((LongDecimal)y).CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >=(UnsignedLongReal x, UnsignedLongDecimal y) => ((LongDecimal)y).CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <=(UnsignedLongReal x, UnsignedLongDecimal y) => ((LongDecimal)y).CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator >(UnsignedLongReal x, UnsignedLongDecimal y) => ((LongDecimal)y).CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(UnsignedLongDecimal, UnsignedLongDecimal)"/>
	public static bool operator <(UnsignedLongReal x, UnsignedLongDecimal y) => ((LongDecimal)y).CompareTo(x) > 0;
	public static bool operator ==(UnsignedLongDecimal? x, UnsignedLongDecimal? y) => x?.CompareTo(y) == 0;
	public static bool operator !=(UnsignedLongDecimal? x, UnsignedLongDecimal? y) => x?.CompareTo(y) != 0;
	public static bool operator >=(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) <= 0;
	public static bool operator >(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) > 0;
	public static bool operator <(UnsignedLongDecimal x, UnsignedLongDecimal y) => x.CompareTo(y) < 0;
}
