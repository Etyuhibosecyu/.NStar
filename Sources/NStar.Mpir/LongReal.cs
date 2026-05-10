using System.Text;

namespace NStar.Mpir;

public readonly struct LongReal : ICloneable, IConvertible, IComparable, IComparable<LongReal>, IFloatingPoint<LongReal>
{
	private enum SpecialValue : byte
	{
		None,
		Zero,
		PositiveInfinity,
		NegativeInfinity,
		NaN,
	}

	private static readonly ConcurrentDictionary<int, MpuT> MantissaMasks = [], MantissaOverflows = [];
	private static readonly LongReal ten = new(1uL << 62, 3, MinMantissaLength, SpecialValue.None);
	private readonly MpzT m;
	private readonly UnsignedLongReal e;
	private readonly int MantissaLength = 0;
	private readonly SpecialValue specialValue = SpecialValue.None;
	public const int AutoMantissaLength = -1, DefaultMantissaLength = 2048, MinMantissaLength = 64;

	private LongReal(MpzT m, UnsignedLongReal e, int mantissaLength = DefaultMantissaLength,
		SpecialValue specialValue = SpecialValue.None)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		this.m = m;
		this.e = e;
		this.specialValue = specialValue;
	}

	public LongReal(decimal op, int mantissaLength = DefaultMantissaLength) : this((double)op, mantissaLength) { }

	public LongReal(double op, int mantissaLength = MinMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		switch (op)
		{
			case 0d or double.NegativeZero:
			m = MpzT.Zero;
			e = UnsignedLongReal.Zero;
			specialValue = SpecialValue.Zero;
			return;
			case double.PositiveInfinity:
			m = MpzT.Zero;
			e = UnsignedLongReal.Zero;
			specialValue = SpecialValue.PositiveInfinity;
			return;
			case double.NegativeInfinity:
			m = MpzT.Zero;
			e = UnsignedLongReal.Zero;
			specialValue = SpecialValue.NegativeInfinity;
			return;
			case double.NaN:
			m = MpzT.Zero;
			e = UnsignedLongReal.Zero;
			specialValue = SpecialValue.NaN;
			return;
			default:
			var bits = BitConverter.DoubleToUInt64Bits(op);
			var negative = (bits & 0x8000000000000000) != 0;
			var exponent = (int)(bits >> 52 & 0x7FF) - 1023;
			MpzT mantissa = bits & 0xFFFFFFFFFFFFF;
			if (exponent == -1023)
			{
				m = (ShiftUniversal(mantissa, MantissaLength - mantissa.BitLength) & MantissaMask) << 1 | 1;
				e = 1074 - mantissa.BitLength;
				return;
			}
			m = ShiftUniversal(mantissa, MantissaLength - 52);
			if (negative)
				m = ~m;
			m = m << 1 | (exponent >= 0 ? MpzT.Zero : MpzT.One);
			e = exponent >= 0 ? exponent : ~exponent;
			return;
		}
	}

	public LongReal(int op, int mantissaLength = MinMantissaLength) : this(new MpzT(op), mantissaLength) { }

	public LongReal(uint op, int mantissaLength = MinMantissaLength) : this(new MpzT(op), mantissaLength) { }

	public LongReal(long op, int mantissaLength = MinMantissaLength) : this(new MpzT(op), mantissaLength) { }

	public LongReal(ulong op, int mantissaLength = MinMantissaLength) : this(new MpzT(op), mantissaLength) { }

	public LongReal(MpzT op, int mantissaLength = DefaultMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		if (op == 0)
		{
			m = MpzT.Zero;
			e = UnsignedLongReal.Zero;
			specialValue = SpecialValue.Zero;
		}
		else
		{
			m = (ShiftUniversal(op, MantissaLength - op.BitLength) & MantissaMask) << 1;
			e = op.BitLength - 1;
		}
	}

	public LongReal(MpuT op, int mantissaLength = DefaultMantissaLength)
		: this(Unsafe.As<MpzT>(op), mantissaLength) { }

	public LongReal(LongReal op) : this(op.m, op.e.Copy(), op.MantissaLength) { }

	public LongReal(LongReal op, int mantissaLength = DefaultMantissaLength)
		: this(op.GetWithOtherML(mantissaLength, true) is var x ? x.m : MpuT.Zero, x.e, mantissaLength) { }

	public LongReal(BigInteger op, int mantissaLength = DefaultMantissaLength)
		: this(new MpuT(op), mantissaLength) { }

	public LongReal(ReadOnlySpan<byte> bytes, int order, int mantissaLength = AutoMantissaLength)
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
			e = UnsignedLongReal.Zero;
		}
		else
		{
			var mStart = Math.Max(order, 0) * (bytes.Length - mantissaByteLength);
			var eStart = Math.Max(-order, 0) * mantissaByteLength;
			m = new(bytes.Slice(mStart, mantissaByteLength), order);
			e = new UnsignedLongReal(bytes.Slice(eStart, bytes.Length - mantissaByteLength), order, mantissaLength);
		}
	}

	public static LongReal AdditiveIdentity => Zero;
	public static LongReal E { get; } = new(new MpzT("11606359105611668"
			+ "169311813919148550388174125964810495189864141917793261896135029171368444671843164607933252"
			+ "407634810716418616433057588279395622588921932809900027347108205362080556193960907290719966"
			+ "342470414539410510167444752409850545165847179978593595359268208361036203482010353444826302"
			+ "026714590171438698376490894341775007308459544420480294394980632796601444948644720256364723"
			+ "358543318389430820981226117117374760277038413999043356858032283754897587002181046666390814"
			+ "783553829834769125991529423043986703019224155318460478636181448055587197690166562262740310"
			+ "816408659705623192014541121868021943846501372881929126806641"), 1, DefaultMantissaLength);
	private int MantissaByteLength => GetArrayLength(MantissaLength, 8);
	private MpuT MantissaMask =>
		this is var this2 ? MantissaMasks.GetOrAdd(MantissaLength, x => this2.MantissaOverflow - 1) : 0;
	private MpuT MantissaOverflow => MantissaOverflows.GetOrAdd(MantissaLength, x => MpuT.One << x);
	public static LongReal MultiplicativeIdentity => One;
	public static LongReal NaN { get; } = new(0, 0, MinMantissaLength, SpecialValue.NaN);
	public static LongReal NegativeInfinity { get; } = new(0, 0, MinMantissaLength, SpecialValue.NegativeInfinity);
	public static LongReal NegativeOne { get; } = new(-1, MinMantissaLength);
	public static LongReal One { get; } = new(1, MinMantissaLength);
	public static LongReal Pi { get; } = new(new MpzT("18446428358512695"
			+ "840370445901406206202588296426415332295003835183746084540170240915926660559452218007571966"
			+ "776486336041382622631688254298178972872662396948113505271043010448512803995168312645853013"
			+ "070577786713821036054795170130116588436555702706838674001875780258213953644307447012671405"
			+ "978624015332035199411215046085079465971748057789745376095728427323727938808466113270155357"
			+ "613215755668596776543390946135393885132416261292039567292207861053847970595360918134324712"
			+ "356138200206559413262605850570319649793683702359095687606709336182624498687306415247837978"
			+ "530951806606033867870105234761392347331177487419970367878346"), 1, DefaultMantissaLength);
	public static LongReal PositiveInfinity { get; } = new(0, 0, MinMantissaLength, SpecialValue.PositiveInfinity);
	public static int Radix => 2;
	public static LongReal Tau { get; } = new(new MpzT("18446428358512695"
			+ "840370445901406206202588296426415332295003835183746084540170240915926660559452218007571966"
			+ "776486336041382622631688254298178972872662396948113505271043010448512803995168312645853013"
			+ "070577786713821036054795170130116588436555702706838674001875780258213953644307447012671405"
			+ "978624015332035199411215046085079465971748057789745376095728427323727938808466113270155357"
			+ "613215755668596776543390946135393885132416261292039567292207861053847970595360918134324712"
			+ "356138200206559413262605850570319649793683702359095687606709336182624498687306415247837978"
			+ "530951806606033867870105234761392347331177487419970367878346"), 2, DefaultMantissaLength);
	public static LongReal Zero { get; } = new(0, 0, MinMantissaLength, SpecialValue.Zero);

	public static LongReal Abs(LongReal value) => value < 0 ? -value : value;

	private static LongReal AddInternal(LongReal x, LongReal y, int mantissaLength)
	{
		var mantissaOverflow = MpzT.One << mantissaLength;
		var mantissaMask = mantissaOverflow - 1;
		var xmlDiff = mantissaLength - x.MantissaLength;
		var ymlDiff = mantissaLength - y.MantissaLength;
		var xm = x.m >> 1 << xmlDiff;
		var ym = y.m >> 1 << ymlDiff;
		UnsignedLongReal newE;
		if ((x.m & 1) != 0)
		{
			if (Mpir.MpzCmpSi(ym, 0) >= 0)
			{
				var eDiff = y.e - x.e;
				if (eDiff == 0)
				{
					newE = (x.e - 1).GetWithOtherML(mantissaLength, false);
					return new((xm + ym).ShiftRightRound(1) << 1 | 1, newE, mantissaLength);
				}
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound(eDiff & -1);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e - 1).GetWithOtherML(mantissaLength, false);
					return new((mSum & mantissaMask).ShiftRightRound(1) << 1 | 1, newE, mantissaLength);
				}
				newE = x.e.GetWithOtherML(mantissaLength, true);
				return new(mSum << 1 | 1, newE, mantissaLength);
			}
			ym = ~ym;
			if (x.e < y.e - 1)
			{
				var eDiff = y.e - x.e;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound(eDiff & -1);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1 | 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				newE = (x.e + 1).GetWithOtherML(mantissaLength, false);
				return new((mDiff << 1 & mantissaMask) << 1 | 1, newE, mantissaLength);
			}
			else if (x.e == y.e)
			{
				var mDiff = xm - ym;
				if (mDiff == 0)
					return new(0, 0, mantissaLength, SpecialValue.Zero);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				newE = (x.e + shiftAmount).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + xm << 1) - (mantissaOverflow + ym);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (shiftAmount == -1)
				{
					newE = x.e.GetWithOtherML(mantissaLength, true);
					return new((mDiff.ShiftRightRound(1) & mantissaMask) << 1 | 1, newE, mantissaLength);
				}
				newE = (x.e + (shiftAmount + 1)).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
			}
		}
		else if ((y.m & 1) != 0)
		{
			if (Mpir.MpzCmpSi(ym, 0) >= 0)
			{
				var eDiff = x.e + y.e + 1;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound(eDiff & -1);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e + 1).GetWithOtherML(mantissaLength, false);
					return new((mSum & mantissaMask).ShiftRightRound(1) << 1, newE, mantissaLength);
				}
				newE = x.e.GetWithOtherML(mantissaLength, true);
				return new(mSum << 1, newE, mantissaLength);
			}
			ym = ~ym;
			if (x.e != 0 && y.e != 0)
			{
				var eDiff = x.e + y.e + 1;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound(eDiff & -1);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				newE = (x.e - 1).GetWithOtherML(mantissaLength, false);
				return new((mDiff << 1 & mantissaMask) << 1, newE, mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + xm << 1) - (mantissaOverflow + ym);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (shiftAmount == -1)
				{
					newE = x.e.GetWithOtherML(mantissaLength, true);
					return new((mDiff.ShiftRightRound(1) & mantissaMask) << 1, newE, mantissaLength);
				}
				newE = new(~(shiftAmount + 1), mantissaLength);
				return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
			}
		}
		else
		{
			if (Mpir.MpzCmpSi(ym, 0) >= 0)
			{
				var eDiff = x.e - y.e;
				if (eDiff == 0)
				{
					newE = (x.e + 1).GetWithOtherML(mantissaLength, false);
					return new((xm + ym).ShiftRightRound(1) << 1, newE, mantissaLength);
				}
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound(eDiff & -1);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e + 1).GetWithOtherML(mantissaLength, false);
					return new((mSum & mantissaMask).ShiftRightRound(1) << 1, newE, mantissaLength);
				}
				newE = x.e.GetWithOtherML(mantissaLength, true);
				return new(mSum << 1, newE, mantissaLength);
			}
			ym = ~ym;
			if (x.e > y.e + 1)
			{
				var eDiff = x.e - y.e;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound(eDiff & -1);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				newE = (x.e - 1).GetWithOtherML(mantissaLength, false);
				return new((mDiff << 1 & mantissaMask) << 1, newE, mantissaLength);
			}
			else if (x.e == y.e)
			{
				var mDiff = xm - ym;
				if (mDiff == 0)
					return new(0, 0, mantissaLength, SpecialValue.Zero);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (x.e < shiftAmount)
				{
					newE = new(~(shiftAmount - (x.e & -1)), mantissaLength);
					return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
				}
				newE = (x.e - shiftAmount).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1, newE, mantissaLength);
			}
			else
			{
				var mDiff = (mantissaOverflow + xm << 1) - (mantissaOverflow + ym);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (shiftAmount == -1)
				{
					newE = x.e.GetWithOtherML(mantissaLength, true);
					return new((mDiff.ShiftRightRound(1) & mantissaMask) << 1, newE, mantissaLength);
				}
				if (x.e < shiftAmount + 1)
				{
					newE = new(~(shiftAmount + 1 - (x.e & -1)), mantissaLength);
					return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
				}
				newE = (x.e - (shiftAmount + 1)).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1, newE, mantissaLength);
			}
		}
	}

	public LongReal Ceiling()
	{
		var truncated = Truncate();
		if (this > 0 && truncated != this)
			truncated++;
		return truncated;
	}

	public readonly object Clone() => Copy();
	public int CompareTo(int other) => CompareTo(new MpzT(other));
	public int CompareTo(uint other) => CompareTo(new MpzT(other));
	public int CompareTo(long other) => CompareTo(new MpzT(other));
	public int CompareTo(ulong other) => CompareTo(new MpzT(other));

	public int CompareTo(MpzT other)
	{
		var compared = e.CompareTo(other.BitLength - 1);
		if (compared != 0)
			return compared;
		return (MantissaOverflow + (m >> 1)).CompareTo(ShiftUniversal(other, MantissaLength - other.BitLength));
	}

	public int CompareTo(MpuT other) => CompareTo(Unsafe.As<MpzT>(other));

	public readonly int CompareTo(LongReal other)
	{
		if ((m & 1) != 0)
		{
			if ((other.m & 1) == 0)
				return -1;
			var compared = other.e.CompareTo(e);
			if (compared != 0)
				return compared;
		}
		else
		{
			if ((other.m & 1) != 0)
				return 1;
			var compared = e.CompareTo(other.e);
			if (compared != 0)
				return compared;
		}
		var mantissaLength = Math.Max(MantissaLength, other.MantissaLength);
		return ShiftUniversal(m >> 1, mantissaLength - MantissaLength)
			.CompareTo(ShiftUniversal(other.m >> 1, mantissaLength - other.MantissaLength));
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
		LongReal lr => CompareTo(lr),
		BigInteger bi => CompareTo(new MpzT(bi)),
		IComparable ic => -ic.CompareTo(this),
		_ => 0,
	};

	public readonly LongReal Copy() => new(m, e.Copy(), MantissaLength, specialValue);

	private static (LongReal Quotient, LongReal Remainder) DivRemInternal(LongReal x, LongReal y, int maxMantissaLength)
	{
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (x.e <= y.e + maxMantissaLength)
		{
			var eDiff = (x.e - y.e) & -1;
			var quotient = ((MantissaOverflow + x.m) << eDiff).Divide(MantissaOverflow + y.m, out var remainder);
			return (new(quotient, maxMantissaLength), new(remainder << (y.e & -1) - 1, maxMantissaLength));
		}
		else
		{
			var quotient = (MantissaOverflow + x.m << maxMantissaLength + 1) / (MantissaOverflow + y.m);
			var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
			return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e - y.e + shiftAmount - maxMantissaLength,
				maxMantissaLength), new(0, maxMantissaLength));
		}
	}

	public bool Equals(int other) => CompareTo(other) == 0;
	public bool Equals(long other) => CompareTo(other) == 0;
	public bool Equals(LongReal other) => CompareTo(other) == 0;
	public bool Equals(MpuT other) => CompareTo(other) == 0;
	public bool Equals(MpzT other) => CompareTo(other) == 0;

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
		LongReal lr => CompareTo(lr) == 0,
		BigInteger bi => CompareTo(new MpzT(bi)) == 0,
		IConvertible ic => ic.Equals(this),
		_ => false,
	};

	public bool Equals(uint other) => CompareTo(other) == 0;
	public bool Equals(ulong other) => CompareTo(other) == 0;

	public LongReal Floor()
	{
		var truncated = Truncate();
		if (this < 0 && truncated != this)
			truncated--;
		return truncated;
	}

	private static string Format(string mantissaDigits, MpzT exponent, bool isNegative, string format, NumberFormatInfo nfi)
	{
		mantissaDigits = mantissaDigits.TrimStart('0');
		if (string.IsNullOrEmpty(mantissaDigits))
			return "0";
		StringBuilder result = new();
		if (isNegative)
			result.Append(nfi.NegativeSign);
		if (!string.IsNullOrEmpty(format))
			return FormatWithCustomFormat(mantissaDigits, exponent, format, nfi, result);
		else
			return FormatStandardScientific(mantissaDigits, exponent, nfi, result);
	}

	private static string FormatExponential(string mantissaDigits, MpzT exponent, int precision,
		NumberFormatInfo nfi, StringBuilder result)
	{
		// Нормализуем мантиссу к диапазону [1, 10)
		if (mantissaDigits.Length > 1)
		{
			exponent += mantissaDigits.Length - 1;
			mantissaDigits = mantissaDigits[0] + nfi.NumberDecimalSeparator
				+ mantissaDigits[1..Math.Min(mantissaDigits.Length, precision + 1)];
		}
		result.Append(mantissaDigits);
		result.Append('E');
		if (exponent >= 0)
			result.Append(nfi.PositiveSign);
		result.Append(exponent);
		return result.ToString();
	}

	private static string FormatFixedPoint(string mantissaDigits, MpzT exponent, int precision,
		NumberFormatInfo nfi, StringBuilder result)
	{
		exponent++;
		if (exponent.BitLength > 31)
			throw new FormatException("Слишком большое или слишком маленькое число"
				+ " для форматирования с фиксированной точккой!");
		var decimalPosition = exponent & -1; // Позиция десятичной точки
		if (decimalPosition <= 0)
		{
			// Очень маленькое число — добавляем ведущие нули
			result.Append('0').Append(nfi.NumberDecimalSeparator);
			result.Append('0', -decimalPosition);
			result.Append(mantissaDigits.AsSpan(0, Math.Min(precision, mantissaDigits.Length)));
		}
		else if (decimalPosition >= mantissaDigits.Length)
		{
			// Очень большое число — добавляем trailing нули
			ReadOnlySpan<char> chars = [.. mantissaDigits, .. Enumerable.Repeat('0', decimalPosition - mantissaDigits.Length)];
			result.Append(FormatInsertGroupSeparators(chars, nfi));
		}
		else
		{
			// Число в нормальном диапазоне
			result.Append(FormatInsertGroupSeparators(mantissaDigits.AsSpan(0, decimalPosition), nfi));
			result.Append(nfi.NumberDecimalSeparator);
			result.Append(mantissaDigits.AsSpan(decimalPosition,
				Math.Min(precision, mantissaDigits.Length - decimalPosition)));
		}
		return result.ToString();
	}

	private static string FormatFlexible(string mantissaDigits, MpzT exponent, int precision,
		NumberFormatInfo nfi, StringBuilder result)
	{
		exponent++;
		if (exponent.BitLength > 31)
			return FormatExponential(mantissaDigits, exponent, precision, nfi, result);
		var decimalPosition = exponent & -1; // Позиция десятичной точки
		var exponentialLength = Math.Min(mantissaDigits.Length, precision + 1) + nfi.NumberDecimalSeparator.Length
			+ Mpir.MpzSizeinbase(exponent, 10) + (exponent >= 0 ? nfi.PositiveSign.Length : nfi.NegativeSign.Length) + 1;
		int fixedLength;
		if (decimalPosition <= 0)
			fixedLength = -decimalPosition + nfi.NumberDecimalSeparator.Length
				+ Math.Min(precision, mantissaDigits.Length) + 1;
		else if (decimalPosition >= mantissaDigits.Length)
			fixedLength = decimalPosition;
		else
			fixedLength = Math.Min(decimalPosition + precision, mantissaDigits.Length) + nfi.NumberDecimalSeparator.Length;
		var sum = 0;
		_ = nfi.NumberGroupSizes.FirstOrDefault(x =>
		{
			var value = (sum += x + 1) >= decimalPosition;
			if (!value)
				fixedLength++;
			return value;
		});
		if (exponentialLength > fixedLength)
			return FormatExponential(mantissaDigits, exponent, precision, nfi, result);
		else
			return FormatFixedPoint(mantissaDigits, exponent, precision, nfi, result);
	}

	private static ReadOnlySpan<char> FormatInsertGroupSeparators(ReadOnlySpan<char> input, NumberFormatInfo nfi)
	{
		if (input.Length < 3)
			return input;
		StringBuilder result = new();
		var numberGroupIndex = 0;
		var offset = 0;
		for (var i = input.Length - 1; i >= 0; i--)
		{
			result.Append(input[i]);
			if (input.Length - i - offset != nfi.NumberGroupSizes[numberGroupIndex])
				continue;
			result.Append([.. nfi.NumberGroupSeparator.Reverse()]);
			offset += nfi.NumberGroupSizes[numberGroupIndex];
			if (numberGroupIndex < nfi.NumberGroupSizes.Length - 1)
				numberGroupIndex++;
		}
		return result.ToString().Reverse().ToArray();
	}

	private static string FormatStandardScientific(string mantissaDigits, MpzT exponent,
		NumberFormatInfo nfi, StringBuilder result)
	{
		if (mantissaDigits.Length == 1)
		{
			result.Append(mantissaDigits[0]);
			result.Append('E');
			if (exponent >= 0)
				result.Append('+');
			result.Append(exponent);
		}
		else
		{
			result.Append(mantissaDigits[0]);
			result.Append(nfi.NumberDecimalSeparator);
			result.Append(mantissaDigits.AsSpan(1));
			result.Append('E');
			if (exponent >= 0)
				result.Append('+');
			result.Append(exponent);
		}
		return result.ToString();
	}

	private static string FormatWithCustomFormat(string mantissaDigits, MpzT exponent, string format,
		NumberFormatInfo nfi, StringBuilder result)
	{
		var formatSpecifier = format[0];
		var precision = format.Length > 1 ? int.Parse(format[1..]) : 6;
		return char.ToUpper(formatSpecifier) switch
		{
			'F' or 'N' => FormatFixedPoint(mantissaDigits, exponent, precision, nfi, result),
			'E' => FormatExponential(mantissaDigits, exponent, precision, nfi, result),
			'G' => FormatFlexible(mantissaDigits, exponent, precision, nfi, result),
			'P' => FormatFixedPoint(mantissaDigits, exponent - 2, precision, nfi, result) + '%',
			_ => FormatStandardScientific(mantissaDigits, exponent, nfi, result),
		};
	}

	public LongReal Frac() => this - Truncate();
	public int GetByteCount() => GetByteCount(true);
	public int GetByteCount(bool saveMantissaLength) =>
		MantissaByteLength + e.GetByteCount(false) + (saveMantissaLength ? sizeof(int) : 0);
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

	public int GetSignificandBitLength() => m.GetShortestBitLength();
	public int GetSignificandByteCount() => m.GetByteCount();
	TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

	public readonly LongReal GetWithOtherML(int mantissaLength, bool copy)
	{
		if (mantissaLength == MantissaLength)
			return copy ? Copy() : this;
		return new(ShiftUniversal(m >> 1, mantissaLength - MantissaLength) << 1 | m & 1, e, mantissaLength, specialValue);
	}

	public static bool IsCanonical(LongReal value) => true;
	public static bool IsComplexNumber(LongReal value) => true;
	public bool IsEven() => specialValue == SpecialValue.Zero || (m & 1) == 0 && e >= 1
		&& (e > MantissaLength || TrailingZeroCount(m >> 1) >= MantissaLength - (e & -1) + 1);
	public static bool IsEvenInteger(LongReal value) => value.IsEven();
	public static bool IsFinite(LongReal value) => true;
	public static bool IsImaginaryNumber(LongReal value) => false;
	public static bool IsInfinity(LongReal value) =>
		value.specialValue is SpecialValue.PositiveInfinity or SpecialValue.NegativeInfinity;
	public bool IsInteger() => specialValue == SpecialValue.Zero || (m & 1) == 0
		&& (e > MantissaLength || TrailingZeroCount(m >> 1) >= MantissaLength - (e & -1));
	public static bool IsInteger(LongReal value) => value.IsInteger();
	public static bool IsNaN(LongReal value) => value.specialValue == SpecialValue.NaN;
	public static bool IsNegative(LongReal value) => Mpir.MpzCmpSi(value.m, 0) < 0;
	public static bool IsNegativeInfinity(LongReal value) => value.specialValue == SpecialValue.NegativeInfinity;
	public static bool IsNormal(LongReal value) => true;
	public static bool IsOddInteger(LongReal value) =>
		value.specialValue == SpecialValue.None && (value.m & 1) == 0 && value.e <= value.MantissaLength
		&& TrailingZeroCount(value.m >> 1) == value.MantissaLength - (value.e & -1);
	public static bool IsPositive(LongReal value) => value.m > 0;
	public static bool IsPositiveInfinity(LongReal value) => value.specialValue == SpecialValue.PositiveInfinity;
	public static bool IsRealNumber(LongReal value) => true;
	public static bool IsSubnormal(LongReal value) => false;
	public static bool IsZero(LongReal value) => value.specialValue == SpecialValue.Zero;
	public static LongReal Max(LongReal x, LongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static LongReal MaxMagnitude(LongReal x, LongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static LongReal MaxMagnitudeNumber(LongReal x, LongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static LongReal Min(LongReal x, LongReal y) => x.CompareTo(y) < 0 ? x : y;
	public static LongReal MinMagnitude(LongReal x, LongReal y) => x.CompareTo(y) < 0 ? x : y;
	public static LongReal MinMagnitudeNumber(LongReal x, LongReal y) => x.CompareTo(y) < 0 ? x : y;

	private static LongReal MultiplyInternal(LongReal x, LongReal y, int maxMantissaLength)
	{
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var product = (MantissaOverflow + x.m) * (MantissaOverflow + y.m);
		var shiftAmount = product.BitLength - maxMantissaLength - 1;
		return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + y.e + shiftAmount - 1, maxMantissaLength);
	}

	public static LongReal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();
	public static LongReal Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();
	public static LongReal Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();
	public static LongReal Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

	public LongReal Power(int exponent)
	{
		if (exponent < 0)
			return One / Power((uint)-exponent);
		else
			return Power((uint)exponent);
	}

	public LongReal Power(uint exponent)
	{
		if (exponent == 0)
			return One;
		else if (exponent == 1)
			return this;
		var result = this;
		for (var i = sizeof(uint) * 8 - (int)uint.LeadingZeroCount(exponent) - 2; i >= 0; i++)
		{
			result *= result;
			if ((exponent & 1u << i) != 0)
				result *= this;
		}
		return result;
	}

	public LongReal Round()
	{
		var frac = Frac();
		if (frac < 0)
			frac++;
		return frac >= 0.5 ? Ceiling() : Floor();
	}

	public static LongReal Round(LongReal x, int digits, MidpointRounding mode)
	{
		var multiplier = ten.Power(digits);
		return (x / multiplier).RoundFunction(mode)() * multiplier;
	}

	public LongReal RoundAwayFromZero()
	{
		var truncated = Truncate();
		var frac = this - truncated;
		if (frac < 0)
			frac++;
		if (frac > 0.5)
			return Ceiling();
		else if (frac < 0.5)
			return Floor();
		else
			return truncated + Sign();
	}

	private Func<LongReal> RoundFunction(MidpointRounding mode) => mode switch
	{
		MidpointRounding.ToEven => RoundToEven,
		MidpointRounding.AwayFromZero => RoundAwayFromZero,
		MidpointRounding.ToZero => Truncate,
		MidpointRounding.ToNegativeInfinity => Floor,
		MidpointRounding.ToPositiveInfinity => Ceiling,
		_ => Truncate,
	};

	public LongReal RoundToEven()
	{
		var truncated = Truncate();
		var frac = this - truncated;
		if (frac < 0)
			frac++;
		if (frac > 0.5)
			return Ceiling();
		else if (frac < 0.5)
			return Floor();
		else if (truncated.IsEven())
			return truncated;
		else
			return truncated + Sign();
	}

	private static MpzT ShiftUniversal(MpzT x, int shiftAmount) => shiftAmount switch
	{
		> 0 => x << shiftAmount,
		< 0 => x.ShiftRightRound(-shiftAmount),
		_ => x,
	};

	public int Sign() => specialValue switch
	{
		SpecialValue.Zero => 0,
		SpecialValue.PositiveInfinity => 1,
		SpecialValue.NegativeInfinity => -1,
		SpecialValue.NaN => throw new InvalidOperationException("Ошибка, невозможно вычислить знак!"),
		_ => Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1,
	};

	public static int Sign(LongReal value) => value.Sign();
	bool IConvertible.ToBoolean(IFormatProvider? provider) => CompareTo(1) >= 0;
	byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

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
		m.val == 0 ? "0" : e >= 65536 ? "Too large for short string, use ToString() instead." : ((MpuT)this).ToString();
	public override string? ToString() => ToString(null, null);
	public string ToString(IFormatProvider? provider) => ToString(null, provider) ?? "";
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var nfi = NumberFormatInfo.GetInstance(formatProvider);
		if (string.IsNullOrEmpty(format))
			format = "G6";
		var formatSpecifier = char.ToUpper(format[0]);
		if (formatSpecifier is not ('F' or 'N' or 'E' or 'G' or 'P'))
			throw new FormatException("Поддержка формата " + format + " в разработке."
				+ " В настоящее время поддерживаются только форматы, состоящие из буквы F, N, G, E или P,"
				+ " за которой следует целое неотрицательное число (состоящее только из цифр 0-9,"
				+ " без точки и других знаков), а также пустая строка или null.");
		var precision = format.Length > 1 ? int.Parse(format[1..]) : 6;
		switch (specialValue)
		{
			case SpecialValue.Zero:
			return "0";
			case SpecialValue.PositiveInfinity:
			return nfi.PositiveInfinitySymbol;
			case SpecialValue.NegativeInfinity:
			return nfi.NegativeInfinitySymbol;
			case SpecialValue.NaN:
			return nfi.NaNSymbol;
			default:
			var mantissa = m >> 1;
			var negative = Mpir.MpzCmpSi(m, 0) < 0;
			if (negative)
				mantissa = ~mantissa;
			var exponent = (MpzT)e;
			if ((m & 1) != 0)
				exponent = ~exponent;
			exponent = (MpzT)Math.Ceiling(((double)exponent + BigInteger.Log((BigInteger)(mantissa += MantissaOverflow), 2)
				- MantissaLength) * Math.Log10(2) - 1);
			mantissa = ShiftUniversal(mantissa * new MpzT(5).Power(precision), precision - MantissaLength);
			return Format(mantissa.ToString() ?? "1", exponent, negative, format, nfi);
		}
	}

	object IConvertible.ToType(Type targetType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(targetType);
		if (targetType == typeof(LongReal))
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
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(LongReal)
			+ ", " + nameof(MpzT) + ", " + nameof(MpuT)
			+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	public LongReal Truncate()
	{
		if (specialValue != SpecialValue.None)
			return Copy();
		if ((m & 1) != 0)
			return new(0, 0, MantissaLength, SpecialValue.Zero);
		if (e >= MantissaLength)
			return Copy();
		var newM = m >> 1;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			newM = ~newM;
		var shiftAmount = MantissaLength - (e & -1);
		newM = newM >> shiftAmount << shiftAmount;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			newM = ~newM;
		return new(newM << 1, e, MantissaLength);
	}

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToChecked<TOther>(LongReal value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToSaturating<TOther>(LongReal value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToTruncating<TOther>(LongReal value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();
	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out LongReal result) => throw new NotImplementedException();
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out LongReal result) => throw new NotImplementedException();
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out LongReal result) => throw new NotImplementedException();
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out LongReal result) => throw new NotImplementedException();

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

	public static implicit operator LongReal(byte value) => new((uint)value);
	public static implicit operator LongReal(short value) => new(value, MinMantissaLength);
	public static implicit operator LongReal(ushort value) => new(value, MinMantissaLength);
	public static implicit operator LongReal(int value) => new(value, MinMantissaLength);
	public static implicit operator LongReal(uint value) => new(value);
	public static implicit operator LongReal(long value) => new(value);
	public static implicit operator LongReal(ulong value) => new(value);
	public static implicit operator LongReal(MpzT value) => new(value);
	public static implicit operator LongReal(MpuT value) => new(value);
	public static implicit operator LongReal(float value) => new((double)value);
	public static implicit operator LongReal(double value) => new(value);
	public static explicit operator LongReal(decimal value) => new(value);
	public static explicit operator LongReal(string value) => double.Parse(value);
	public static explicit operator byte(LongReal value) => (byte)(uint)value;
	public static explicit operator short(LongReal value) => (short)(int)value;
	public static explicit operator ushort(LongReal value) => (ushort)(uint)value;
	public static explicit operator int(LongReal value) => (int)(uint)value;

	public static explicit operator uint(LongReal value)
	{
		if (value.specialValue != SpecialValue.None || (value.m & 1) != 0
			|| value.e >= (uint)value.MantissaLength + sizeof(uint) * 8u)
			return 0u;
		var eAfterCast = value.e & -1;
		if (eAfterCast <= value.MantissaLength)
			return value.m << value.MantissaLength - eAfterCast & uint.MaxValue;
		else
			return value.m >> eAfterCast - value.MantissaLength & uint.MaxValue;
	}

	public static explicit operator long(LongReal value) => (long)(ulong)value;

	public static explicit operator ulong(LongReal value)
	{
		if (value.specialValue != SpecialValue.None || (value.m & 1) != 0
			|| value.e >= (uint)value.MantissaLength + sizeof(ulong) * 8u)
			return 0uL;
		var eAfterCast = value.e & -1;
		if (eAfterCast <= value.MantissaLength)
			return value.m << value.MantissaLength - eAfterCast & uint.MaxValue;
		else
			return value.m >> eAfterCast - value.MantissaLength & uint.MaxValue;
	}

	public static explicit operator float(LongReal value) => (float)(double)value;

	public static explicit operator double(LongReal value)
	{
		switch (value.specialValue)
		{
			case SpecialValue.Zero:
			return 0d;
			case SpecialValue.PositiveInfinity:
			return double.PositiveInfinity;
			case SpecialValue.NegativeInfinity:
			return double.NegativeInfinity;
			case SpecialValue.NaN:
			return double.NaN;
			default:
			var negative = Mpir.MpzCmpSi(value.m, 0) < 0;
			var negativeExponent = (value.m & 1) != 0;
			if (!negativeExponent && value.e >= (uint)value.MantissaLength + 1024u)
				return negative ? double.PositiveInfinity : double.NegativeInfinity;
			if (negativeExponent && value.e >= 1074)
				return 0d;
			var eAfterCast = value.e & -1;
			if (negativeExponent)
				eAfterCast = ~eAfterCast;
			var exponent = (ulong)Math.Max(eAfterCast += 1023, 0);
			var mantissa = value.m >> 1;
			if (negative)
				mantissa = ~mantissa;
			if (exponent == 0)
				mantissa = value.MantissaOverflow + mantissa >> value.MantissaLength - eAfterCast - 51;
			else
				mantissa >>= value.MantissaLength - 52;
			return BitConverter.UInt64BitsToDouble((negative ? 0x8000000000000000 : 0) + (exponent << 52) + (ulong)mantissa);
		}
	}

	public static explicit operator decimal(LongReal value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);

	public static explicit operator string?(LongReal value) => value.ToString();

	public static explicit operator MpzT(LongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return value.MantissaOverflow + value.m << (value.e & -1) - 1;
		else
			return 0;
	}

	public static explicit operator MpuT(LongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return new MpuT(value.MantissaOverflow + value.m) << (int)value.e - 1;
		else
			return 0;
	}

	public static LongReal operator +(LongReal value) => new(value);
	public static LongReal operator -(LongReal value) =>
		new(-(value.m >> 1) << 1 | value.m & 1, value.e, value.MantissaLength,
			value.specialValue == SpecialValue.PositiveInfinity ? SpecialValue.NegativeInfinity
			: value.specialValue == SpecialValue.NegativeInfinity ? SpecialValue.PositiveInfinity : value.specialValue);

	public static LongReal operator +(LongReal x, LongReal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (x.specialValue == SpecialValue.Zero)
			return y.GetWithOtherML(mantissaLength, true);
		if (y.specialValue == SpecialValue.Zero)
			return x.GetWithOtherML(mantissaLength, true);
		if (x.specialValue == SpecialValue.NaN || y.specialValue == SpecialValue.NaN)
			return new(0, 0, mantissaLength, SpecialValue.NaN);
		if (y > x)
			(x, y) = (y, x);
		if (Mpir.MpzCmpSi(x.m, 0) < 0 || (x.m & 1) != 0 && (y.m & 1) == 0)
			return -AddInternal(-y, -x, mantissaLength);
		return AddInternal(x, y, mantissaLength);
	}

	public static LongReal operator -(LongReal x, LongReal y) => x + -y;

	public static LongReal operator *(int x, LongReal y) => y * x;
	public static LongReal operator *(uint x, LongReal y) => y * x;

	public static LongReal operator *(LongReal x, int y)
	{
		if (y < 0)
			return -x * (uint)-y;
		else
			return x * (uint)y;
	}

	public static LongReal operator *(LongReal x, uint y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (y == 0)
			return new(0, 0, MantissaLength,
				x.specialValue is SpecialValue.None or SpecialValue.Zero ? SpecialValue.Zero : SpecialValue.NaN);
		else if (y == 1)
			return x.Copy();
		var product = (MantissaOverflow + x.m) * y;
		var shiftAmount = product.BitLength - MantissaLength - 1;
		return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount, MantissaLength);
	}

	public static LongReal operator *(LongReal x, LongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (x.specialValue == SpecialValue.NaN || y.specialValue == SpecialValue.NaN)
			return new(0, 0, maxMantissaLength, SpecialValue.NaN);
		else if (x.specialValue == SpecialValue.Zero || y.specialValue == SpecialValue.Zero)
			return new(0, 0, maxMantissaLength,
				x.specialValue is SpecialValue.PositiveInfinity or SpecialValue.NegativeInfinity
				|| y.specialValue is SpecialValue.PositiveInfinity or SpecialValue.NegativeInfinity
				? SpecialValue.NaN : SpecialValue.Zero);
		else if (x.specialValue == SpecialValue.NegativeInfinity)
			return new(0, 0, maxMantissaLength, y < 0 ? SpecialValue.PositiveInfinity : SpecialValue.NegativeInfinity);
		else if (x.specialValue == SpecialValue.PositiveInfinity)
			return new(0, 0, maxMantissaLength, y < 0 ? SpecialValue.NegativeInfinity : SpecialValue.PositiveInfinity);
		else if (y.specialValue == SpecialValue.NegativeInfinity)
			return new(0, 0, maxMantissaLength,
				Mpir.MpzCmpSi(x.m, 0) < 0 ? SpecialValue.PositiveInfinity : SpecialValue.NegativeInfinity);
		else if (y.specialValue == SpecialValue.PositiveInfinity)
			return new(0, 0, maxMantissaLength,
				Mpir.MpzCmpSi(x.m, 0) < 0 ? SpecialValue.NegativeInfinity : SpecialValue.PositiveInfinity);
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (Mpir.MpzCmpSi(x.m, 0) < 0 && Mpir.MpzCmpSi(y.m, 0) < 0)
			return MultiplyInternal(-x, -y, maxMantissaLength);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -MultiplyInternal(-x, y, maxMantissaLength);
		else if (Mpir.MpzCmpSi(y.m, 0) < 0)
			return -MultiplyInternal(x, -y, maxMantissaLength);
		else
			return MultiplyInternal(x, y, maxMantissaLength);
	}

	public static LongReal operator /(LongReal x, int y)
	{
		if (y < 0)
			return -x / (uint)-y;
		else
			return x / (uint)y;
	}

	public static LongReal operator /(LongReal x, uint y)
	{
		var MantissaLength = x.MantissaLength;
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		if (y == 0)
			return new(0, 0, MantissaLength,
				x.specialValue is SpecialValue.Zero or SpecialValue.NaN ? SpecialValue.NaN
				: x < 0 ? SpecialValue.NegativeInfinity : SpecialValue.PositiveInfinity);
		else if (x.specialValue != SpecialValue.None || y == 1)
			return x.Copy();
		else if (x.e <= sizeof(uint) * 8 - uint.LeadingZeroCount(y))
			return new(((MantissaOverflow + x.m) << (x.e & -1) - 1) / y, MantissaLength);
		var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y;
		var shiftAmount = quotient.BitLength - MantissaLength - 1;
		return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1,
			MantissaLength);
	}

	public static LongReal operator /(LongReal x, LongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (x.specialValue == SpecialValue.NaN || y.specialValue == SpecialValue.NaN)
			return new(0, 0, maxMantissaLength, SpecialValue.NaN);
		else if (y.specialValue == SpecialValue.Zero)
			return new(0, 0, maxMantissaLength,
				x.specialValue is SpecialValue.Zero or SpecialValue.NaN ? SpecialValue.NaN
				: x < 0 ? SpecialValue.NegativeInfinity : SpecialValue.PositiveInfinity);
		else if (y.specialValue != SpecialValue.None)
			return new(0, 0, maxMantissaLength,
				x.specialValue is SpecialValue.PositiveInfinity or SpecialValue.NegativeInfinity
				? SpecialValue.NaN : SpecialValue.Zero);
		else if (x.specialValue == SpecialValue.Zero)
			return new(0, 0, maxMantissaLength, SpecialValue.Zero);
		else if (x.specialValue == SpecialValue.NegativeInfinity)
			return new(0, 0, maxMantissaLength, y < 0 ? SpecialValue.PositiveInfinity : SpecialValue.NegativeInfinity);
		else if (x.specialValue == SpecialValue.PositiveInfinity)
			return new(0, 0, maxMantissaLength, y < 0 ? SpecialValue.NegativeInfinity : SpecialValue.PositiveInfinity);
		else if (Mpir.MpzCmpSi(y.m, 0) == 0 && y.e == 0)
			return x.Copy();
		else if (Mpir.MpzCmpSi(y.m, -2) == 0 && y.e == 0)
			return -x;
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (Mpir.MpzCmpSi(x.m, 0) < 0 && Mpir.MpzCmpSi(y.m, 0) < 0)
			return DivRemInternal(-x, -y, maxMantissaLength).Quotient;
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -DivRemInternal(-x, y, maxMantissaLength).Quotient;
		else if (Mpir.MpzCmpSi(y.m, 0) < 0)
			return -DivRemInternal(x, -y, maxMantissaLength).Quotient;
		else
			return DivRemInternal(x, y, maxMantissaLength).Quotient;
	}

	public static LongReal operator %(LongReal x, LongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (x.specialValue == SpecialValue.NaN || y.specialValue == SpecialValue.NaN || y.specialValue == SpecialValue.Zero
			|| x.specialValue is SpecialValue.NegativeInfinity or SpecialValue.PositiveInfinity)
			return new(0, 0, maxMantissaLength, SpecialValue.NaN);
		else if (x.specialValue == SpecialValue.Zero)
			return new(0, 0, maxMantissaLength, SpecialValue.Zero);
		x = x.GetWithOtherML(maxMantissaLength, false);
		y = y.GetWithOtherML(maxMantissaLength, false);
		if (y.specialValue == SpecialValue.NegativeInfinity)
			return -x;
		else if (y.specialValue == SpecialValue.PositiveInfinity)
			return x;
		else if (Mpir.MpzCmpSi(x.m, 0) < 0 && Mpir.MpzCmpSi(y.m, 0) < 0)
			return DivRemInternal(-x, -y, maxMantissaLength).Remainder;
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -DivRemInternal(-x, y, maxMantissaLength).Remainder;
		else if (Mpir.MpzCmpSi(y.m, 0) < 0)
			return -DivRemInternal(x, -y, maxMantissaLength).Remainder;
		else
			return DivRemInternal(x, y, maxMantissaLength).Remainder;
	}
	public static LongReal operator ++(LongReal x) => x + One;
	public static LongReal operator --(LongReal x) => x - One;

	public static bool operator ==(LongReal x, int y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, int y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, int y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, int y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, int y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, int y) => x.CompareTo(y) < 0;
	public static bool operator ==(LongReal x, uint y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, uint y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, uint y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, uint y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, uint y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, uint y) => x.CompareTo(y) < 0;
	public static bool operator ==(LongReal x, long y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, long y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, long y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, long y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, long y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, long y) => x.CompareTo(y) < 0;
	public static bool operator ==(LongReal x, ulong y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, ulong y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, ulong y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, ulong y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, ulong y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, ulong y) => x.CompareTo(y) < 0;
	public static bool operator ==(LongReal x, MpzT y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, MpzT y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, MpzT y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, MpzT y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, MpzT y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, MpzT y) => x.CompareTo(y) < 0;
	public static bool operator ==(LongReal x, MpuT y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, MpuT y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, MpuT y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, MpuT y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, MpuT y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, MpuT y) => x.CompareTo(y) < 0;
	public static bool operator ==(int x, LongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(int x, LongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(int x, LongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(int x, LongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(int x, LongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(int x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(uint x, LongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(uint x, LongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(uint x, LongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(uint x, LongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(uint x, LongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(uint x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(long x, LongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(long x, LongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(long x, LongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(long x, LongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(long x, LongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(long x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(ulong x, LongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(ulong x, LongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(ulong x, LongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(ulong x, LongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(ulong x, LongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(ulong x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(MpzT x, LongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(MpzT x, LongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(MpzT x, LongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(MpzT x, LongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(MpzT x, LongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(MpzT x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(MpuT x, LongReal y) => y.CompareTo(x) == 0;
	public static bool operator !=(MpuT x, LongReal y) => y.CompareTo(x) != 0;
	public static bool operator >=(MpuT x, LongReal y) => y.CompareTo(x) <= 0;
	public static bool operator <=(MpuT x, LongReal y) => y.CompareTo(x) >= 0;
	public static bool operator >(MpuT x, LongReal y) => y.CompareTo(x) < 0;
	public static bool operator <(MpuT x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(LongReal x, LongReal y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, LongReal y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, LongReal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, LongReal y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, LongReal y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, LongReal y) => x.CompareTo(y) < 0;
}
