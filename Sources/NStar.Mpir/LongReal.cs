namespace NStar.Mpir;

/// <summary>
/// Представляет действительное число с плавающей точкой,
/// с положительной или отрицательной мантиссой настраиваемой длины и с потенциально бесконечной экспонентой в плюс и в минус.
/// Доступны операторы преобразования и конструкторы из знаковых и беззнаковых целых чисел,
/// <see cref="UnsignedLongReal"/> и строки, преобразование в массив байт и из него,
/// основные математические константы, арифметические, тригонометрические и другие основные операции.
/// В этом типе мантисса является двоичной, поэтому возможны ошибки при работе с десятичными числами.
/// Если для вас это критично, используйте LongDecimal.
/// </summary>
public readonly struct LongReal : IFloatingPoint<LongReal>, ICloneable, IConvertible
{
	private enum SpecialValue : byte
	{
		None,
		Zero,
		PositiveInfinity,
		NegativeInfinity,
		NaN,
	}

	private static readonly ConcurrentDictionary<int, MpzT> MantissaMasks = [], MantissaOverflows = [];
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
				m = mantissa << MantissaLength - mantissa.BitLength + 1 & MantissaMask;
				if (negative)
					m = ~m;
				m = m << 1 | 1;
				e = 1074 - mantissa.BitLength;
				return;
			}
			m = mantissa << MantissaLength - 52;
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
			m = ShiftUniversal(op.Abs(), MantissaLength - op.BitLength + 1) & MantissaMask;
			if (Mpir.MpzCmpSi(op, 0) < 0)
				m = ~m;
			m <<= 1;
			e = op.BitLength - 1;
		}
	}

	public LongReal(MpuT op, int mantissaLength = DefaultMantissaLength)
		: this(Unsafe.As<MpzT>(op), mantissaLength) { }

	public LongReal(UnsignedLongReal op, int mantissaLength = AutoMantissaLength)
	{
		if (mantissaLength == AutoMantissaLength)
			mantissaLength = op.MantissaLength;
		else
			op = op.GetWithOtherML(mantissaLength, false);
		MantissaLength = mantissaLength;
		if (op.e is null)
		{
			m = (ShiftUniversal(Unsafe.As<MpzT>(op.m), MantissaLength - op.m.BitLength + 1) & MantissaMask) << 1;
			e = op.m.BitLength - 1;
		}
		else
		{
			m = op.m << 1;
			e = op.e + (MantissaLength - 1);
		}
	}

	public LongReal(LongReal op) : this(op.m, op.e.Copy(), op.MantissaLength) { }

	public LongReal(LongReal op, int mantissaLength)
		: this(op.GetWithOtherML(mantissaLength, true) is var x ? x.m : MpuT.Zero, x.e, mantissaLength) { }

	public LongReal(BigInteger op, int mantissaLength = DefaultMantissaLength)
		: this(new MpzT(op), mantissaLength) { }

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
		if (bytes.Length == 0)
		{
			m = MpzT.Zero;
			e = UnsignedLongReal.Zero;
			this.specialValue = SpecialValue.Zero;
		}
		if ((SpecialValue)bytes[order < 0 ? ^1 : 0] is var specialValue
			&& specialValue is >= SpecialValue.None and <= SpecialValue.NaN)
			this.specialValue = specialValue;
		var mantissaByteLength = MantissaByteLength;
		if (bytes.Length <= mantissaByteLength)
		{
			m = new(bytes[1..], order);
			e = UnsignedLongReal.Zero;
		}
		else
		{
			var mStart = Math.Max(order, 0) * (bytes.Length - mantissaByteLength);
			var eStart = Math.Max(-order, 0) * mantissaByteLength + Math.Max(order, 0);
			m = new(bytes.Slice(mStart, mantissaByteLength), order);
			e = new UnsignedLongReal(bytes.Slice(eStart, bytes.Length - mantissaByteLength - 1), order, mantissaLength);
		}
		var bitLength = m.BitLength;
		if (bitLength <= MantissaLength)
			return;
		var shiftAmount = bitLength - MantissaLength;
		m = (m >> 1).ShiftRightRound(shiftAmount) | m & 1;
	}

	public static LongReal AdditiveIdentity => Zero;
	public static LongReal E { get; } = new(new MpzT("23212718211223336"
			+ "338623627838297100776348251929620990379728283835586523792270058342736889343686329215866504"
			+ "815269621432837232866115176558791245177843865619800054694216410724161112387921814581439932"
			+ "684940829078821020334889504819701090331694359957187190718536416722072406964020706889652604"
			+ "053429180342877396752981788683550014616919088840960588789961265593202889897289440512729446"
			+ "717086636778861641962452234234749520554076827998086713716064567509795174004362093332781629"
			+ "567107659669538251983058846087973406038448310636920957272362896111174395380333124525480621"
			+ "632817319411246384029082243736043887693002745763858253613282"), 1, DefaultMantissaLength);
	/// <summary>Gets the mathematical constant log₁₀2.</summary>
	public static LongReal Log10of2 { get; } = new(new MpzT("13193093437534837"
			+ "303673461252140622067065792820957531270659411632284461569252577010857745113059787258480204"
			+ "186472726335377931673446064383153736393503926492538757757561984120326268315316537998451554"
			+ "839523510621367800345588111718926389747284947354326946700039679660225431638470719198288276"
			+ "446710819745440722585243479695067370464255248517977286999371364226860463401868836916767885"
			+ "810612769020679044320362723051051466850550847762060664708019280295908792546704928761727222"
			+ "792247522640563384788844899176486907742221216650263284117521446698234236500962746490371493"
			+ "018765124548818722563691945476359472209241002518358953159865"), 1, DefaultMantissaLength);
	private int MantissaByteLength => GetArrayLength(MantissaLength + 2, 8);
	private MpzT MantissaMask =>
		this is var this2 ? MantissaMasks.GetOrAdd(MantissaLength, x => this2.MantissaOverflow - 1) : 0;
	private MpzT MantissaOverflow => MantissaOverflows.GetOrAdd(MantissaLength, x => MpuT.One << x);
	public static LongReal MultiplicativeIdentity => One;
	public static LongReal NaN { get; } = new(0, 0, MinMantissaLength, SpecialValue.NaN);
	public static LongReal NegativeInfinity { get; } = new(0, 0, MinMantissaLength, SpecialValue.NegativeInfinity);
	public static LongReal NegativeOne { get; } = new(-2, 0, MinMantissaLength);
	public static LongReal One { get; } = new(0, 0, MinMantissaLength);
	/// <summary>Получает (двоичный) порядок числа: количество бит в целой части для чисел &gt;= 1 и 0 для &lt; 1.</summary>
	public UnsignedLongReal Order => (m & 1) != 0 ? UnsignedLongReal.Zero : e + 1;
	public static LongReal Pi { get; } = new(new MpzT("36892856717025391680"
			+ "740891802812412405176592852830664590007670367492169080340481831853321118904436015143933"
			+ "552972672082765245263376508596357945745324793896227010542086020897025607990336625291706026"
			+ "141155573427642072109590340260233176873111405413677348003751560516427907288614894025342811"
			+ "957248030664070398822430092170158931943496115579490752191456854647455877616932226540310715"
			+ "226431511337193553086781892270787770264832522584079134584415722107695941190721836268649424"
			+ "712276400413118826525211701140639299587367404718191375213418672365248997374612830495675957"
			+ "061903613212067735740210469522784694662354974839940735756692"), 1, DefaultMantissaLength);
	public static LongReal PositiveInfinity { get; } = new(0, 0, MinMantissaLength, SpecialValue.PositiveInfinity);
	public static int Radix => 2;

	/// <summary>Получает знак числа (в формате целого числа 1, 0 или -1).</summary>
	public int Sign => specialValue switch
	{
		SpecialValue.Zero => 0,
		SpecialValue.PositiveInfinity => 1,
		SpecialValue.NegativeInfinity => -1,
		SpecialValue.NaN => throw new InvalidOperationException("Ошибка, невозможно вычислить знак у неопределенности!"),
		_ => Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1,
	};

	/// <summary>Получает порядок числа с учетом знака: положительный для положительных чисел и отрицательный иначе.</summary>
	public LongReal SignedOrder => (LongReal)Order * Sign;
	public static LongReal Tau { get; } = new(new MpzT("18446428358512695"
			+ "840370445901406206202588296426415332295003835183746084540170240915926660559452218007571966"
			+ "776486336041382622631688254298178972872662396948113505271043010448512803995168312645853013"
			+ "070577786713821036054795170130116588436555702706838674001875780258213953644307447012671405"
			+ "978624015332035199411215046085079465971748057789745376095728427323727938808466113270155357"
			+ "613215755668596776543390946135393885132416261292039567292207861053847970595360918134324712"
			+ "356138200206559413262605850570319649793683702359095687606709336182624498687306415247837978"
			+ "530951806606033867870105234761392347331177487419970367878346"), 2, DefaultMantissaLength);
	/// <summary>Gets the mathematical constant 10.</summary>
	public static LongReal Ten { get; } = new(new MpzT("16158503035655503"
			+ "650357438344334975980222051334857742016065172713762327569433945446598600705761456731844358"
			+ "980460949009747059779575245460547544076193224141560315438683650498045875098875194826053398"
			+ "028819192033784138396109321309878080919047169238085235290822926018152521443787945770532904"
			+ "303776199561965192760957166694834171210342487393282284747428088017663161029038902829665513"
			+ "096354230157075129296432088558362971801859230928678799175576150822952201848806616643615613"
			+ "562842355410104862578550863465661734839271290328348967522998634176499319107762583194718667"
			+ "771801067716614802322659239302476074096777926805529798115328"), 3, DefaultMantissaLength);
	public static LongReal Zero { get; } = new(0, 0, MinMantissaLength, SpecialValue.Zero);

	/// <summary>
	/// Computes the absolute of this number.
	/// </summary>
	/// <returns>The absolute of this number.</returns>
	public LongReal Abs() => Mpir.MpzCmpSi(m, 0) < 0 ? -this : this;
	public static LongReal Abs(LongReal value) => value.Abs();

	private static LongReal AddInternal(LongReal x, LongReal y, int mantissaLength, int xmlDiff, int ymlDiff)
	{
		var mantissaOverflow = MpzT.One << mantissaLength;
		var mantissaMask = mantissaOverflow - 1;
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
					var switchE = x.e == 0;
					newE = (switchE ? 0 : x.e - 1).GetWithOtherML(mantissaLength, false);
					return new((xm + ym).ShiftRightRound(1) << 1 | (switchE ? 0 : 1), newE, mantissaLength);
				}
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					var switchE = x.e == 0;
					newE = (switchE ? 0 : x.e - 1).GetWithOtherML(mantissaLength, false);
					return new((mSum & mantissaMask).ShiftRightRound(1) << 1 | (switchE ? 0 : 1), newE, mantissaLength);
				}
				newE = x.e.GetWithOtherML(mantissaLength, true);
				return new(mSum << 1 | 1, newE, mantissaLength);
			}
			ym = ~(y.m >> 1) << ymlDiff;
			if (x.e + 1 < y.e)
			{
				var eDiff = y.e - x.e;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
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
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e + 1).GetWithOtherML(mantissaLength, false);
					return new((mSum & mantissaMask).ShiftRightRound(1) << 1, newE, mantissaLength);
				}
				newE = x.e.GetWithOtherML(mantissaLength, true);
				return new(mSum << 1, newE, mantissaLength);
			}
			ym = ~(y.m >> 1) << ymlDiff;
			if (x.e != 0 || y.e != 0)
			{
				var eDiff = x.e + y.e + 1;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				var switchE = x.e == 0;
				newE = (switchE ? 0 : x.e - 1).GetWithOtherML(mantissaLength, false);
				return new((mDiff << 1 & mantissaMask) << 1 | (switchE ? 1 : 0), newE, mantissaLength);
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
				newE = new(shiftAmount, mantissaLength);
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
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e + 1).GetWithOtherML(mantissaLength, false);
					return new((mSum & mantissaMask).ShiftRightRound(1) << 1, newE, mantissaLength);
				}
				newE = x.e.GetWithOtherML(mantissaLength, true);
				return new(mSum << 1, newE, mantissaLength);
			}
			ym = ~(y.m >> 1) << ymlDiff;
			if (x.e > y.e + 1)
			{
				var eDiff = x.e - y.e;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength, true);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
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
					newE = new(shiftAmount - (int)x.e - 1, mantissaLength);
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
					newE = new(shiftAmount - (int)x.e, mantissaLength);
					return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
				}
				newE = (x.e - (shiftAmount + 1)).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1, newE, mantissaLength);
			}
		}
	}

	/// <summary>
	/// Возвращает наименьшее целое число, которое не меньше данного числа:
	/// само данное число для целых и ближайшее сверху целое для дробных.
	/// </summary>
	/// <returns>См. общее описание.</returns>
	public LongReal Ceiling()
	{
		var truncated = Truncate();
		if (this > 0 && truncated != this)
			truncated++;
		return truncated;
	}

	public object Clone() => Copy();

	/// <summary>
	/// Сравнивает данное число с <see langword="int"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(int other) => CompareTo(new MpzT(other));

	/// <summary>
	/// Сравнивает данное число с <see langword="uint"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(uint other) => CompareTo(new MpzT(other));

	/// <summary>
	/// Сравнивает данное число с <see langword="long"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(long other) => CompareTo(new MpzT(other));

	/// <summary>
	/// Сравнивает данное число с <see langword="ulong"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(ulong other) => CompareTo(new MpzT(other));

	/// <summary>
	/// Сравнивает данное число с <see cref="MpzT"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(MpzT other)
	{
		ArgumentNullException.ThrowIfNull(other);
		switch (specialValue)
		{
			case SpecialValue.NaN:
			return int.MinValue;
			case SpecialValue.NegativeInfinity:
			return -1;
			case SpecialValue.PositiveInfinity:
			return 1;
			case SpecialValue.Zero:
			return -Mpir.MpzCmpSi(other, 0);
			default:
			if (Mpir.MpzCmpSi(m, 0) < 0 && Mpir.MpzCmpSi(other, 0) >= 0)
				return -1;
			else if (Mpir.MpzCmpSi(m, 0) >= 0 && Mpir.MpzCmpSi(other, 0) <= 0)
				return 1;
			else if ((m & 1) != 0)
				return Mpir.MpzCmpSi(m, 0) < 0 ? 1 : -1;
			var compared = e.CompareTo(other.BitLength - 1);
			if (compared != 0)
				return compared;
			return (MantissaOverflow + (m >> 1)).CompareTo(ShiftUniversal(other, MantissaLength - other.BitLength + 1));
		}
	}

	/// <summary>
	/// Сравнивает данное число с <see cref="MpuT"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(MpuT other) => CompareTo(Unsafe.As<MpzT>(other));

	/// <summary>
	/// Сравнивает данное число с <see cref="UnsignedLongReal"/>.
	/// См. описание <see cref="CompareTo(LongReal)"/> для более подробных сведений.
	/// </summary>
	public int CompareTo(UnsignedLongReal other) => CompareTo(new LongReal(other));

	public int CompareTo(LongReal other)
	{
		if (specialValue == SpecialValue.NaN && other.specialValue == SpecialValue.NaN)
			return 0;
		else if (specialValue == SpecialValue.NaN || other.specialValue == SpecialValue.NaN)
			return int.MinValue;
		else if (specialValue != SpecialValue.None && specialValue == other.specialValue)
			return 0;
		else if (specialValue == SpecialValue.NegativeInfinity)
			return -1;
		else if (specialValue == SpecialValue.PositiveInfinity)
			return 1;
		else if (other.specialValue == SpecialValue.NegativeInfinity)
			return 1;
		else if (other.specialValue == SpecialValue.PositiveInfinity)
			return -1;
		else if (specialValue == SpecialValue.Zero)
			return Mpir.MpzCmpSi(other.m, 0) < 0 ? 1 : -1;
		else if (other.specialValue == SpecialValue.Zero)
			return Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1;
		else if (Mpir.MpzCmpSi(m, 0) < 0 && Mpir.MpzCmpSi(other.m, 0) >= 0)
			return -1;
		else if (Mpir.MpzCmpSi(m, 0) >= 0 && Mpir.MpzCmpSi(other.m, 0) < 0)
			return 1;
		else if ((m & 1) != 0)
		{
			if ((other.m & 1) == 0)
				return Mpir.MpzCmpSi(m, 0) < 0 ? 1 : -1;
			var compared = other.e.CompareTo(e);
			if (compared != 0)
				return Mpir.MpzCmpSi(m, 0) < 0 ? -compared : compared;
		}
		else
		{
			if ((other.m & 1) != 0)
				return Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1;
			var compared = e.CompareTo(other.e);
			if (compared != 0)
				return Mpir.MpzCmpSi(m, 0) < 0 ? -compared : compared;
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
		float f => CompareTo(f),
		double d => CompareTo(d),
		LongReal lr => CompareTo(lr),
		BigInteger bi => CompareTo(new MpzT(bi)),
		IComparable ic => -ic.CompareTo(this),
		_ => 0,
	};

	/// <inheritdoc cref="Clone"/>
	public LongReal Copy() => new(m, e.Copy(), MantissaLength, specialValue);

	private static LongReal DivideInternal(LongReal x, LongReal y, int maxMantissaLength)
	{
		var MantissaOverflow = MpuT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var quotient = (MantissaOverflow + (x.m >> 1) << maxMantissaLength + 1) / (MantissaOverflow + (y.m >> 1));
		var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
		quotient = quotient.ShiftRightRound(shiftAmount) & MantissaMask;
		if (x.e + shiftAmount >= y.e + 1)
			return new(quotient << 1, x.e - y.e + shiftAmount - 1, maxMantissaLength);
		else
			return new(quotient << 1 | 1, y.e - x.e - shiftAmount, maxMantissaLength);
	}

	private static LongReal DivideUiInternal(LongReal x, uint y, int MantissaLength)
	{
		var MantissaOverflow = MpuT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var quotient = (MantissaOverflow + (x.m >> 1) << MantissaLength + 1) / y;
		var shiftAmount = MantissaLength * 2 + 2 - quotient.BitLength;
		var mantissa = (quotient.ShiftRightRound(MantissaLength - shiftAmount + 1) & MantissaMask) << 1 | x.m & 1;
		if ((x.m & 1) != 0)
			return new(mantissa, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(mantissa, x.e - shiftAmount, x.MantissaLength);
		else
			return new(mantissa | 1, shiftAmount - (int)x.e - 1, x.MantissaLength);
	}

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="int"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(int other) => CompareTo(other) == 0;

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="uint"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(uint other) => CompareTo(other) == 0;

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="long"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(long other) => CompareTo(other) == 0;

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see langword="ulong"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(ulong other) => CompareTo(other) == 0;

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see cref="MpzT"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(MpzT other) => CompareTo(other) == 0;

	/// <summary>
	/// Проверяет, равно ли данное число указанному числу типа <see cref="MpuT"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(MpuT other) => CompareTo(other) == 0;

	/// <summary>
	/// Сравнивает данное число с <see cref="UnsignedLongReal"/>.
	/// См. описание <see cref="Equals(LongReal)"/> для более подробных сведений.
	/// </summary>
	public bool Equals(UnsignedLongReal other) => Equals(new LongReal(other));

	public bool Equals(LongReal other) => CompareTo(other) == 0;

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
		float f => CompareTo(f) == 0,
		double d => CompareTo(d) == 0,
		LongReal lr => CompareTo(lr) == 0,
		BigInteger bi => CompareTo(new MpzT(bi)) == 0,
		IConvertible ic => ic.Equals(this),
		_ => false,
	};

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше данного числа:
	/// само данное число для целых и ближайшее снизу целое для дробных.
	/// </summary>
	/// <returns>См. общее описание.</returns>
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
			mantissaDigits = MpuT.TryParse(mantissaDigits, out var uz)
				&& uz.ShiftRightRoundDec(Math.Max(mantissaDigits.Length - precision - 1, 0)).ToString() is var s
				? s![0] + nfi.NumberDecimalSeparator + s[1..] : "1";
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
		var decimalPosition = (int)exponent; // Позиция десятичной точки
		if (decimalPosition <= 0)
		{
			// Очень маленькое число — добавляем ведущие нули
			result.Append('0').Append(nfi.NumberDecimalSeparator);
			result.Append('0', -decimalPosition);
			result.Append(MpuT.TryParse(mantissaDigits, out var uz)
				&& uz.ShiftRightRoundDec(Math.Max(mantissaDigits.Length - precision, 0)).ToString() is var s ? s! : "1");
		}
		else if (decimalPosition >= mantissaDigits.Length)
		{
			// Очень большое число — добавляем trailing нули
			ReadOnlySpan<char> chars = [.. mantissaDigits, .. Enumerable.Repeat('0', decimalPosition - mantissaDigits.Length)];
			result.Append(FormatInsertGroupSeparators(chars, nfi));
			if (precision != 0)
			{
				result.Append(nfi.NumberDecimalSeparator);
				result.Append('0', precision);
			}
		}
		else
		{
			// Число в нормальном диапазоне
			result.Append(FormatInsertGroupSeparators(mantissaDigits.AsSpan(0, decimalPosition), nfi));
			result.Append(nfi.NumberDecimalSeparator);
			result.Append(mantissaDigits.AsSpan(decimalPosition,
				Math.Min(precision, mantissaDigits.Length - decimalPosition)));
			result.Append('0', Math.Max(precision - mantissaDigits.Length + decimalPosition, 0));
		}
		return result.ToString();
	}

	private static string FormatFixedPointAutoPrecision(string mantissaDigits, MpzT exponent, int maxPrecision,
		NumberFormatInfo nfi, StringBuilder result)
	{
		exponent++;
		if (exponent.BitLength > 31)
			throw new FormatException("Слишком большое или слишком маленькое число"
				+ " для форматирования с фиксированной точккой!");
		var decimalPosition = (int)exponent; // Позиция десятичной точки
		if (decimalPosition <= 0)
		{
			// Очень маленькое число — добавляем ведущие нули
			result.Append('0').Append(nfi.NumberDecimalSeparator);
			result.Append('0', -decimalPosition);
			result.Append(MpuT.TryParse(mantissaDigits, out var uz)
				&& uz.ShiftRightRoundDec(Math.Max(mantissaDigits.Length - maxPrecision, 0)).ToString() is var s ? s : "1");
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
			result.Append(mantissaDigits[decimalPosition..]);
		}
		return result.ToString();
	}

	private static string FormatFlexible(string mantissaDigits, MpzT exponent, int precision,
		NumberFormatInfo nfi, StringBuilder result)
	{
		var exponent2 = exponent + 1;
		if (exponent2.BitLength > 31)
			return FormatExponential(mantissaDigits, exponent2, precision, nfi, result);
		var decimalPosition = (int)exponent2; // Позиция десятичной точки
		var exponentialLength = Math.Min(mantissaDigits.Length, precision + 1) + nfi.NumberDecimalSeparator.Length
			+ Mpir.MpzSizeinbase(exponent2, 10) + (exponent2 >= 0 ? nfi.PositiveSign.Length : nfi.NegativeSign.Length) + 1;
		int fixedLength;
		if (decimalPosition <= 0)
			fixedLength = -decimalPosition + nfi.NumberDecimalSeparator.Length
				+ Math.Min(precision, mantissaDigits.Length) + 1;
		else if (decimalPosition >= mantissaDigits.Length)
			fixedLength = decimalPosition;
		else
			fixedLength = mantissaDigits.Length + nfi.NumberDecimalSeparator.Length;
		var sum = 0;
		_ = nfi.NumberGroupSizes.FirstOrDefault(x =>
		{
			var value = (sum += x) >= decimalPosition;
			if (!value)
				fixedLength++;
			return value;
		});
		fixedLength += nfi.NumberGroupSizes.Length == 0 || nfi.NumberGroupSizes[^1] == 0
			? 0 : (decimalPosition - sum) / nfi.NumberGroupSizes[^1];
		if (exponentialLength < fixedLength)
			return FormatExponential(mantissaDigits, exponent, precision, nfi, result);
		else
			return FormatFixedPointAutoPrecision(mantissaDigits, exponent, precision, nfi, result);
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
			if (i == 0 || input.Length - i - offset != nfi.NumberGroupSizes[numberGroupIndex])
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

	/// <summary>
	/// Возвращает дробную часть данного числа, положительную для положительных чисел и отрицательную для отрицательных,
	/// отбрасывая целую часть.
	/// </summary>
	/// <returns>См. общее описание.</returns>
	public LongReal Frac() => this - Truncate();

	/// <inheritdoc cref="IBinaryInteger{TSelf}.GetByteCount"/>
	public int GetByteCount() => GetByteCount(true);

	/// <summary>
	/// Считает количество байт, которое необходимо для записи числа в <see cref="Span{byte}"/>,
	/// в зависимости от того, нужно ли также записывать длину мантиссы.
	/// </summary>
	/// <param name="saveMantissaLength">
	/// Нужно ли записывать длину мантиссы (если да, увеличивает результат на <see langword="sizeof(int)"/>).</param>
	/// <returns>Посчитанное количество байт.</returns>
	public int GetByteCount(bool saveMantissaLength) =>
		MantissaByteLength + e.GetByteCount(false) + 1 + (saveMantissaLength ? sizeof(int) : 0);

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

	internal LongReal GetWithOtherML(int mantissaLength, bool copy)
	{
		if (mantissaLength == MantissaLength)
			return copy ? Copy() : this;
		var mantissa = m >> 1;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			mantissa = ~mantissa;
		mantissa = ShiftUniversal(mantissa, mantissaLength - MantissaLength);
		if (Mpir.MpzCmpSi(m, 0) < 0)
			mantissa = ~mantissa;
		return new(mantissa << 1 | m & 1, e, mantissaLength, specialValue);
	}

	public static bool IsCanonical(LongReal value) => true;
	public static bool IsComplexNumber(LongReal value) => true;
	/// <summary>Проверяет, является ли данное число четным (возвращает true или false).</summary>
	public bool IsEven() => specialValue == SpecialValue.Zero || (m & 1) == 0 && e >= 1
		&& (e > MantissaLength || TrailingZeroCount(m >> 1) >= MantissaLength - (int)e + 1);
	public static bool IsEvenInteger(LongReal value) => value.IsEven();
	public static bool IsFinite(LongReal value) => true;
	public static bool IsImaginaryNumber(LongReal value) => false;
	public static bool IsInfinity(LongReal value) =>
		value.specialValue is SpecialValue.PositiveInfinity or SpecialValue.NegativeInfinity;
	/// <summary>Проверяет, является ли данное число целым (возвращает true или false).</summary>
	public bool IsInteger() => specialValue == SpecialValue.Zero || (m & 1) == 0
		&& (e > MantissaLength || TrailingZeroCount(m >> 1) >= MantissaLength - (int)e);
	public static bool IsInteger(LongReal value) => value.IsInteger();
	public static bool IsNaN(LongReal value) => value.specialValue == SpecialValue.NaN;
	public static bool IsNegative(LongReal value) => Mpir.MpzCmpSi(value.m, 0) < 0;
	public static bool IsNegativeInfinity(LongReal value) => value.specialValue == SpecialValue.NegativeInfinity;
	public static bool IsNormal(LongReal value) => true;
	public static bool IsOddInteger(LongReal value) =>
		value.specialValue == SpecialValue.None && (value.m & 1) == 0 && value.e <= value.MantissaLength
		&& TrailingZeroCount(value.m >> 1) == value.MantissaLength - (int)value.e;
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
		var product = (MantissaOverflow + (x.m >> 1)) * (MantissaOverflow + (y.m >> 1));
		var shiftAmount = product.BitLength - maxMantissaLength - 1;
		var shifted = product.ShiftRightRound(shiftAmount);
		if (Mpir.MpzCmp(shifted, MantissaOverflow << 1) == 0)
			shiftAmount++;
		return new((shifted & MantissaMask) << 1,
			x.e + y.e + (shiftAmount - maxMantissaLength), maxMantissaLength);
	}

	private static LongReal MultiplyUiInternal(LongReal x, uint y, int mantissaLength)
	{
		var MantissaOverflow = MpuT.One << mantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var product = (MantissaOverflow + (x.m >> 1)) * y;
		var shiftAmount = product.BitLength - mantissaLength - 1;
		var mantissa = (product.ShiftRightRound(shiftAmount) & MantissaMask) << 1 | x.m & 1;
		if ((x.m & 1) == 0)
			return new(mantissa, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(mantissa, x.e - shiftAmount, x.MantissaLength);
		else
			return new(mantissa & new MpzT(-2), shiftAmount - (int)x.e - 1, x.MantissaLength);
	}

	public static LongReal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => double.Parse(s, provider);
	public static LongReal Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		double.Parse(s, style, provider);
	public static LongReal Parse(string s, IFormatProvider? provider) => double.Parse(s, provider);
	public static LongReal Parse(string s, NumberStyles style, IFormatProvider? provider) => double.Parse(s, style, provider);

	/// <summary>
	/// Возводит данное число в указанную степень.
	/// </summary>
	/// <param name="exponent">Показатель степени, в которую нужно возвести данное число.</param>
	/// <returns>Результат возведения в степень.</returns>
	public LongReal Power(int exponent)
	{
		if (exponent < 0)
			return One / Power((uint)-exponent);
		else
			return Power((uint)exponent);
	}

	/// <summary>
	/// Возводит данное число в указанную степень.
	/// </summary>
	/// <param name="exponent">Показатель степени, в которую нужно возвести данное число.</param>
	/// <returns>Результат возведения в степень.</returns>
	public LongReal Power(uint exponent)
	{
		if (exponent == 0)
			return One;
		else if (exponent == 1)
			return this;
		var result = this;
		for (var i = sizeof(uint) * 8 - (int)uint.LeadingZeroCount(exponent) - 2; i >= 0; i--)
		{
			result *= result;
			if ((exponent & 1u << i) != 0)
				result *= this;
		}
		return result;
	}

	private LongReal Power(MpzT exponent)
	{
		if (exponent < 0)
			return One / Power(Unsafe.As<MpuT>(-exponent));
		else
			return Power(Unsafe.As<MpuT>(exponent));
	}

	private LongReal Power(MpuT exponent)
	{
		if (Mpir.MpuCmpSi(exponent, 0) == 0)
			return One;
		else if (Mpir.MpuCmpSi(exponent, 1) == 0)
			return this;
		var result = this;
		for (var i = exponent.BitLength - 2; i >= 0; i--)
		{
			result *= result;
			if ((exponent & 1u << i) != 0)
				result *= this;
		}
		return result;
	}

	/// <summary>
	/// Возвращает целое число, ближайшее к данному числу. Если два целых числа одинаково близки к данному
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое является четным.
	/// </summary>
	/// <returns>См. общее описание.</returns>
	public LongReal Round()
	{
		var truncated = Truncate();
		var frac = this - truncated;
		if (Mpir.MpzCmpSi(m, 0) < 0)
		{
			var compared = frac.CompareTo(-0.5);
			return compared switch
			{
				< 0 => truncated - 1,
				> 0 => truncated,
				_ => truncated.IsEven() ? truncated : truncated - 1,
			};
		}
		else
		{
			var compared = frac.CompareTo(0.5);
			return compared switch
			{
				< 0 => truncated,
				> 0 => truncated + 1,
				_ => truncated.IsEven() ? truncated : truncated + 1,
			};
		}
	}

	public static LongReal Round(LongReal x, int digits, MidpointRounding mode)
	{
		var multiplier = ten.Power(digits);
		return (x / multiplier).RoundFunction(mode)() * multiplier;
	}

	/// <summary>
	/// Возвращает целое число, ближайшее к данному числу. Если два целых числа одинаково близки к данному
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое дальше от нуля.
	/// </summary>
	/// <returns>См. общее описание.</returns>
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
			return truncated + Sign;
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
			return truncated + Sign;
	}

	/// <summary>
	/// Производит универсальный сдвиг данного числа, как влево, так и вправо, в зависимости от знака параметра.
	/// </summary>
	/// <param name="shiftAmount">Величина сдвига данного числа, положительная или отрицательная (или нулевая).</param>
	/// <returns>Данное число, умноженное на 2 в степени <paramref name="shiftAmount"/>.</returns>
	public LongReal Shift(int shiftAmount)
	{
		if (shiftAmount == int.MinValue)
			return this >> unchecked((uint)int.MinValue);
		else if (shiftAmount < 0)
			return this >> -shiftAmount;
		else
			return this << shiftAmount;
	}

	/// <summary>
	/// Производит универсальный сдвиг данного числа, как влево, так и вправо, в зависимости от знака параметра.
	/// </summary>
	/// <param name="shiftAmount">Величина сдвига данного числа, положительная или отрицательная (или нулевая).</param>
	/// <returns>Данное число, умноженное на 2 в степени <paramref name="shiftAmount"/>.</returns>
	public LongReal Shift(MpzT shiftAmount) =>
		shiftAmount < 0 ? this >> (UnsignedLongReal)(-shiftAmount) : this << (UnsignedLongReal)shiftAmount;

	private static MpzT ShiftUniversal(MpzT x, int shiftAmount) => shiftAmount switch
	{
		> 0 => x << shiftAmount,
		< 0 => x.ShiftRightRound(-shiftAmount),
		_ => x,
	};

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
	public override string? ToString() => ToString(null, null);
	public string ToString(IFormatProvider? provider) => ToString(null, provider) ?? "";
	public string ToString(string? format) => ToString(format, null);

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var nfi = NumberFormatInfo.GetInstance(formatProvider);
		if (string.IsNullOrEmpty(format))
			format = "G16";
		var formatSpecifier = char.ToUpper(format[0]);
		if (formatSpecifier is not ('F' or 'N' or 'E' or 'G' or 'P') || !uint.TryParse(format[1..], out var precision))
			throw new FormatException("Поддержка формата " + format + " в разработке."
				+ " В настоящее время поддерживаются только форматы, состоящие из буквы F, N, G, E или P,"
				+ " за которой следует целое неотрицательное число (состоящее только из цифр 0-9,"
				+ " без точки и других знаков), а также пустая строка или null.");
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
			if (m is null || m.val == 0)
				return "0";
			if (e > (MpuT)double.MaxValue)
				return "Too large or too small number";
			var mantissa = m >> 1;
			var negative = Mpir.MpzCmpSi(m, 0) < 0;
			if (negative)
				mantissa = ~mantissa;
			var exponent = (MpzT)e;
			if ((m & 1) != 0)
				exponent = ~exponent;
			mantissa += MantissaOverflow;
			exponent = (MpzT)((exponent + (LongReal)(BigInteger.Log((BigInteger)mantissa, 2) - MantissaLength + 0.000001))
				* Log10of2).Floor();
			var mantissaDigits = (double)Abs(this / Ten.Power(exponent));
			return Format(mantissaDigits.ToString("F" + precision, CultureInfo.InvariantCulture).Replace(".", "")?.TrimEnd('0') ?? "1",
				exponent, negative, format, nfi);
		}
	}

	object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(conversionType);
		if (conversionType == typeof(LongReal))
			return Copy();
		IConvertible value = this;
		if (conversionType == typeof(sbyte))
			return value.ToSByte(provider);
		else if (conversionType == typeof(byte))
			return value.ToByte(provider);
		else if (conversionType == typeof(short))
			return value.ToInt16(provider);
		else if (conversionType == typeof(ushort))
			return value.ToUInt16(provider);
		else if (conversionType == typeof(int))
			return value.ToInt32(provider);
		else if (conversionType == typeof(uint))
			return value.ToUInt32(provider);
		else if (conversionType == typeof(long))
			return value.ToInt64(provider);
		else if (conversionType == typeof(ulong))
			return value.ToUInt64(provider);
		else if (conversionType == typeof(float))
			return value.ToSingle(provider);
		else if (conversionType == typeof(double))
			return value.ToDouble(provider);
		else if (conversionType == typeof(decimal))
			return value.ToDecimal(provider);
		else if (conversionType == typeof(MpzT))
			return new MpzT(value.ToString(provider));
		else if (conversionType == typeof(MpuT))
			return new MpuT(value.ToString(provider));
		else if (conversionType == typeof(string))
			return value.ToString(provider);
		else if (conversionType == typeof(object))
			return Copy();
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(LongReal)
			+ ", " + nameof(MpzT) + ", " + nameof(MpuT)
			+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше данного числа, для положительных,
	/// и наименьшее целое число, которое не меньше данного числа, для отрицательных (для нуля, если это непонятно, ноль).
	/// Другими словами, возвращает целую часть данного числа, отбрасывая дробную.
	/// </summary>
	/// <returns>См. общее описание.</returns>
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
		var shiftAmount = MantissaLength - (int)e;
		newM = newM >> shiftAmount << shiftAmount;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			newM = ~newM;
		return new(newM << 1, e, MantissaLength);
	}

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToChecked<TOther>(LongReal value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToSaturating<TOther>(LongReal value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToTruncating<TOther>(LongReal value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		var @string = ToString(format.ToString(), provider);
		if (@string.TryCopyTo(destination))
		{
			charsWritten = @string.Length;
			return true;
		}
		else
		{
			charsWritten = 0;
			return false;
		}
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out LongReal result) =>
		TryParse(s, NumberStyles.None, provider, out result);

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongReal result)
	{
		if (double.TryParse(s, style, provider, out var doubleResult))
		{
			result = doubleResult;
			return true;
		}
		else
		{
			result = Zero;
			return false;
		}
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongReal result) => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongReal result) => TryParse(s.AsSpan(), style, provider, out result);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian"/>
	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteBigEndian(destination, out bytesWritten, true);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian"/>
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
		if (!m.TryWriteBigEndian(destination[^MantissaByteLength..], out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += MantissaByteLength;
		destination[^MantissaByteLength..^mLength].Fill((byte)(m < 0 ? 255 : 0));
		if (!e.TryWriteBigEndian(destination[1..^MantissaByteLength], out var bytesWritten2, false))
		{
			bytesWritten = 0;
			return false;
		}
		destination[0] = (byte)specialValue;
		bytesWritten += bytesWritten2 + 1;
		return true;
	}

	public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? 0 : e).TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) =>
		(e is null ? 0 : e).TryWriteLittleEndian(destination, out bytesWritten);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian"/>
	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteLittleEndian(destination, out bytesWritten, true);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian"/>
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
		destination[mLength..MantissaByteLength].Fill((byte)(m < 0 ? 255 : 0));
		if (!e.TryWriteLittleEndian(destination[MantissaByteLength..], out var bytesWritten2, false))
		{
			bytesWritten = 0;
			return false;
		}
		destination[^1] = (byte)specialValue;
		bytesWritten += bytesWritten2 + 1;
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
	public static implicit operator LongReal(UnsignedLongReal value) => new(value);
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
		var eAfterCast = (int)(uint)value.e;
		if (eAfterCast <= value.MantissaLength)
			return (uint)(value.MantissaOverflow + (value.m >> 1)).ShiftRightRound(value.MantissaLength - eAfterCast);
		else
			return (uint)(value.m << eAfterCast - value.MantissaLength);
	}

	public static explicit operator long(LongReal value) => (long)(ulong)value;

	public static explicit operator ulong(LongReal value)
	{
		if (value.specialValue != SpecialValue.None || (value.m & 1) != 0
			|| value.e >= (uint)value.MantissaLength + sizeof(ulong) * 8u)
			return 0uL;
		var eAfterCast = (int)value.e;
		if (eAfterCast <= value.MantissaLength)
			return (value.MantissaOverflow + (value.m >> 1)).ShiftRightRound(value.MantissaLength - eAfterCast) & uint.MaxValue;
		else
			return value.m << eAfterCast - value.MantissaLength & uint.MaxValue;
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
			if (!negativeExponent && value.e >= 1024)
				return negative ? double.NegativeInfinity : double.PositiveInfinity;
			if (negativeExponent && value.e > 1074)
				return 0d;
			var eAfterCast = (int)value.e;
			if (negativeExponent)
				eAfterCast = ~eAfterCast;
			var exponent = (ulong)Math.Max(eAfterCast += 1023, 0);
			var mantissa = value.m >> 1;
			if (negative)
				mantissa = ~mantissa;
			if (exponent == 0)
				mantissa = (value.MantissaOverflow + mantissa).ShiftRightRound(value.MantissaLength - eAfterCast - 51);
			else
				mantissa = mantissa.ShiftRightRound(value.MantissaLength - 52);
			return BitConverter.UInt64BitsToDouble((negative ? 0x8000000000000000 : 0) + (exponent << 52) + (ulong)mantissa);
		}
	}

	public static explicit operator decimal(LongReal value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);

	public static explicit operator string?(LongReal value) => value.ToString();

	public static explicit operator MpzT(LongReal value)
	{
		if (value.specialValue != SpecialValue.None || (value.m & 1) != 0 || value.e > int.MaxValue)
			return 0;
		var eAfterCast = (int)value.e;
		if (eAfterCast <= value.MantissaLength)
		{
			var mantissa = value.m >> 1;
			if (Mpir.MpzCmpSi(value.m, 0) < 0)
				mantissa = ~mantissa;
			mantissa = (value.MantissaOverflow + mantissa).ShiftRightRound(value.MantissaLength - eAfterCast);
			if (Mpir.MpzCmpSi(value.m, 0) < 0)
				mantissa = ~mantissa;
			return mantissa;
		}
		else
			return value.m << eAfterCast - value.MantissaLength;
	}

	public static explicit operator MpuT(LongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e > int.MaxValue)
			return 0;
		var eAfterCast = (int)value.e;
		if (eAfterCast <= value.MantissaLength)
			return (MpuT)(value.MantissaOverflow + (value.m >> 1)).ShiftRightRound(value.MantissaLength - eAfterCast);
		else
			return (MpuT)(value.m << eAfterCast - value.MantissaLength);
	}

	public static LongReal operator +(LongReal value) => new(value);
	public static LongReal operator -(LongReal value) =>
		new(~(value.m >> 1) << 1 | value.m & 1, value.e, value.MantissaLength,
			value.specialValue == SpecialValue.PositiveInfinity ? SpecialValue.NegativeInfinity
			: value.specialValue == SpecialValue.NegativeInfinity ? SpecialValue.PositiveInfinity : value.specialValue);

	public static LongReal operator +(LongReal x, LongReal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (x.specialValue == SpecialValue.NaN || y.specialValue == SpecialValue.NaN)
			return new(0, 0, mantissaLength, SpecialValue.NaN);
		else if (x.specialValue == SpecialValue.NegativeInfinity)
			return new(0, 0, mantissaLength,
				y.specialValue == SpecialValue.PositiveInfinity ? SpecialValue.NaN : SpecialValue.NegativeInfinity);
		else if (x.specialValue == SpecialValue.PositiveInfinity)
			return new(0, 0, mantissaLength,
				y.specialValue == SpecialValue.NegativeInfinity ? SpecialValue.NaN : SpecialValue.PositiveInfinity);
		else if (y.specialValue == SpecialValue.NegativeInfinity)
			return new(0, 0, mantissaLength, SpecialValue.NegativeInfinity);
		else if (y.specialValue == SpecialValue.PositiveInfinity)
			return new(0, 0, mantissaLength, SpecialValue.PositiveInfinity);
		else if (x.specialValue == SpecialValue.Zero)
			return y.GetWithOtherML(mantissaLength, true);
		else if (y.specialValue == SpecialValue.Zero)
			return x.GetWithOtherML(mantissaLength, true);
		else if (Mpir.MpzCmp(x.m, y.m) == 0 && x.e == y.e)
			return x << 1;
		else if (Mpir.MpzCmp(x.m >> 1, ~y.m >> 1) == 0 && (x.m & 1) == (y.m & 1) && x.e == y.e)
			return new(0, 0, mantissaLength, SpecialValue.Zero);
		if (y > x)
			(x, y) = (y, x);
		var xmlDiff = mantissaLength - x.MantissaLength;
		var ymlDiff = mantissaLength - y.MantissaLength;
		if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return x < y ? -AddInternal(-x, -y, mantissaLength, ymlDiff, xmlDiff)
				: -AddInternal(-y, -x, mantissaLength, ymlDiff, xmlDiff);
		if ((x.m & 1) != 0 && ((y.m & 1) == 0 || x.e > y.e
			|| (y.m & 1) != 0 && (x.e > y.e || x.e == y.e && Mpir.MpzCmpSi(y.m, 0) < 0
			&& x.m >> 1 << xmlDiff < ~(y.m >> 1) << ymlDiff))
			|| (y.m & 1) == 0 && (x.e < y.e || x.e == y.e && Mpir.MpzCmpSi(y.m, 0) < 0
			&& x.m >> 1 << xmlDiff < ~(y.m >> 1) << ymlDiff))
			return -AddInternal(-y, -x, mantissaLength, ymlDiff, xmlDiff);
		return AddInternal(x, y, mantissaLength, xmlDiff, ymlDiff);
	}

	public static LongReal operator -(LongReal x, LongReal y) => x + -y;

	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(int x, LongReal y) => y * x;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(uint x, LongReal y) => y * x;

	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, int y)
	{
		if (y < 0)
			return -x * (uint)-y;
		else
			return x * (uint)y;
	}

	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, uint y)
	{
		var mantissaLength = x.MantissaLength;
		if (y == 0)
			return new(0, 0, mantissaLength,
				x.specialValue is SpecialValue.None or SpecialValue.Zero ? SpecialValue.Zero : SpecialValue.NaN);
		else if (x.specialValue != SpecialValue.None || y == 1)
			return x.Copy();
		else if ((y & y - 1) == 0)
			return x << (int)uint.TrailingZeroCount(y);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -MultiplyUiInternal(-x, y, mantissaLength);
		else
			return MultiplyUiInternal(x, y, mantissaLength);
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
		if (Mpir.MpzCmpSi(x.m, 0) == 0 && x.e == 0)
			return y;
		else if (Mpir.MpzCmpSi(y.m, 0) == 0 && y.e == 0)
			return x;
		var xShiftAmount = UnsignedLongReal.Zero;
		if ((x.m & 1) != 0)
			xShiftAmount = x.e + 1;
		var yShiftAmount = UnsignedLongReal.Zero;
		if ((y.m & 1) != 0)
			yShiftAmount = y.e + 1;
		x <<= xShiftAmount;
		y <<= yShiftAmount;
		if (Mpir.MpzCmpSi(x.m, 0) < 0 && Mpir.MpzCmpSi(y.m, 0) < 0)
			return MultiplyInternal(-x, -y, maxMantissaLength) >> xShiftAmount + yShiftAmount;
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -MultiplyInternal(-x, y, maxMantissaLength) >> xShiftAmount + yShiftAmount;
		else if (Mpir.MpzCmpSi(y.m, 0) < 0)
			return -MultiplyInternal(x, -y, maxMantissaLength) >> xShiftAmount + yShiftAmount;
		else
			return MultiplyInternal(x, y, maxMantissaLength) >> xShiftAmount + yShiftAmount;
	}

	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, int y)
	{
		if (y < 0)
			return -x / (uint)-y;
		else
			return x / (uint)y;
	}

	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, uint y)
	{
		var mantissaLength = x.MantissaLength;
		if (y == 0)
			return new(0, 0, mantissaLength,
				x.specialValue is SpecialValue.Zero or SpecialValue.NaN ? SpecialValue.NaN
				: x < 0 ? SpecialValue.NegativeInfinity : SpecialValue.PositiveInfinity);
		else if (x.specialValue != SpecialValue.None || y == 1)
			return x.Copy();
		else if ((y & y - 1) == 0)
			return x >> (int)uint.TrailingZeroCount(y);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -DivideUiInternal(-x, y, mantissaLength);
		else
			return DivideUiInternal(x, y, mantissaLength);
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
		var shiftAmount = UnsignedLongReal.Zero;
		if ((x.m & 1) != 0)
			shiftAmount = x.e + 1;
		if ((y.m & 1) != 0 && y.e >= shiftAmount)
			shiftAmount = y.e + 1;
		x <<= shiftAmount;
		y <<= shiftAmount;
		if (Mpir.MpzCmpSi(x.m, 0) < 0 && Mpir.MpzCmpSi(y.m, 0) < 0)
			return DivideInternal(-x, -y, maxMantissaLength);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -DivideInternal(-x, y, maxMantissaLength);
		else if (Mpir.MpzCmpSi(y.m, 0) < 0)
			return -DivideInternal(x, -y, maxMantissaLength);
		else
			return DivideInternal(x, y, maxMantissaLength);
	}

	public static LongReal operator %(LongReal x, LongReal y) => x - (x / y).Truncate() * y;

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongReal operator <<(LongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (x.specialValue != SpecialValue.None || shiftAmount == 0)
			return x.Copy();
		else if ((x.m & 1) == 0)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new(x.m & new MpzT(-2), shiftAmount - (int)x.e - 1, x.MantissaLength);
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongReal operator <<(LongReal x, UnsignedLongReal shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (x.specialValue != SpecialValue.None || shiftAmount == 0)
			return x.Copy();
		else if ((x.m & 1) == 0)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new(x.m & new MpzT(-2), shiftAmount - x.e - 1, x.MantissaLength);
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongReal operator >>(LongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (x.specialValue != SpecialValue.None || shiftAmount == 0)
			return x.Copy();
		else if ((x.m & 1) != 0)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new(x.m | 1, shiftAmount - x.e - 1, x.MantissaLength);
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongReal operator >>(LongReal x, UnsignedLongReal shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (x.specialValue != SpecialValue.None || shiftAmount == 0)
			return x.Copy();
		else if ((x.m & 1) != 0)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new(x.m | 1, shiftAmount - (int)x.e - 1, x.MantissaLength);
	}

	public static LongReal operator ++(LongReal x) => x + One;
	public static LongReal operator --(LongReal x) => x - One;

	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, int y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, int y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, int y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, int y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, int y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, int y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, uint y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, uint y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, uint y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, uint y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, uint y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, uint y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, long y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, long y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, long y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, long y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, long y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, long y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, ulong y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, ulong y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, ulong y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, ulong y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, ulong y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, ulong y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, MpzT y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, MpzT y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, MpzT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, MpzT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, MpzT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, MpzT y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, MpuT y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, MpuT y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, MpuT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, MpuT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, MpuT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, MpuT y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(LongReal x, UnsignedLongReal y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(LongReal x, UnsignedLongReal y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(LongReal x, UnsignedLongReal y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(LongReal x, UnsignedLongReal y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(LongReal x, UnsignedLongReal y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(LongReal x, UnsignedLongReal y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(int x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(int x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(int x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(int x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(int x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(int x, LongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(uint x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(uint x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(uint x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(uint x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(uint x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(uint x, LongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(long x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(long x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(long x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(long x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(long x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(long x, LongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(ulong x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(ulong x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(ulong x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(ulong x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(ulong x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(ulong x, LongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(MpzT x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(MpzT x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(MpzT x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(MpzT x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(MpzT x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(MpzT x, LongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(MpuT x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(MpuT x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(MpuT x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(MpuT x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(MpuT x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(MpuT x, LongReal y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator ==(LongReal, LongReal)"/>
	public static bool operator ==(UnsignedLongReal x, LongReal y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator !=(LongReal, LongReal)"/>
	public static bool operator !=(UnsignedLongReal x, LongReal y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator }=(LongReal, LongReal)"/>
	public static bool operator >=(UnsignedLongReal x, LongReal y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator {=(LongReal, LongReal)"/>
	public static bool operator <=(UnsignedLongReal x, LongReal y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator }(LongReal, LongReal)"/>
	public static bool operator >(UnsignedLongReal x, LongReal y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator {(LongReal, LongReal)"/>
	public static bool operator <(UnsignedLongReal x, LongReal y) => y.CompareTo(x) > 0;
	public static bool operator ==(LongReal x, LongReal y) => x.CompareTo(y) == 0;
	public static bool operator !=(LongReal x, LongReal y) => x.CompareTo(y) != 0;
	public static bool operator >=(LongReal x, LongReal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(LongReal x, LongReal y) => x.CompareTo(y) <= 0;
	public static bool operator >(LongReal x, LongReal y) => x.CompareTo(y) > 0;
	public static bool operator <(LongReal x, LongReal y) => x.CompareTo(y) < 0;
}
