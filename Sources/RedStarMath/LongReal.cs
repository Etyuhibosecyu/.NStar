namespace RedStarMath;

/// <summary>
/// Представляет действительное число с плавающей точкой,
/// с положительной или отрицательной мантиссой настраиваемой длины и с потенциально бесконечной экспонентой в плюс и в минус.
/// Доступны операторы преобразования и конструкторы из знаковых и беззнаковых целых чисел,
/// double, <see cref="UnsignedLongReal"/> и строки, преобразование в массив байт и из него,
/// основные математические константы, арифметические, тригонометрические и другие основные операции.
/// В этом типе мантисса является двоичной, поэтому возможны ошибки при работе с десятичными числами.
/// Если для вас это критично, используйте LongDecimal.
/// </summary>
public readonly struct LongReal : IFloatingPoint<LongReal>, ICloneable, IConvertible
{
	private static readonly ConcurrentDictionary<int, MpzT> MantissaMasks = [], MantissaOverflows = [];
	private static readonly LongReal ten = new(1uL << 62, 3, MinMantissaLength);
	internal readonly MpzT m;
	internal readonly UnsignedLongReal e;
	internal readonly int MantissaLength = 0;
	public const int AutoMantissaLength = -1, DefaultMantissaLength = 2048, MinMantissaLength = 64;

	internal LongReal(MpzT m, UnsignedLongReal e, int mantissaLength = DefaultMantissaLength)
	{
		if (mantissaLength is < MinMantissaLength or > int.MaxValue)
			mantissaLength = DefaultMantissaLength;
		MantissaLength = mantissaLength;
		this.m = m;
		this.e = e;
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
			m = ShiftedMantissaOverflow + 1;
			e = new(MpuT.Zero, null, mantissaLength);
			return;
			case double.PositiveInfinity:
			m = ShiftedMantissaOverflow + 2;
			e = new(MpuT.Zero, null, mantissaLength);
			return;
			case double.NegativeInfinity:
			m = ShiftedMantissaOverflow + 3;
			e = new(MpuT.Zero, null, mantissaLength);
			return;
			case double.NaN:
			m = ShiftedMantissaOverflow + 4;
			e = new(MpuT.Zero, null, mantissaLength);
			return;
		}
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
			m = ShiftedMantissaOverflow + 1;
			e = new(MpuT.Zero, null, mantissaLength);
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
		{
			if (mantissaLength is < MinMantissaLength or > int.MaxValue)
				mantissaLength = DefaultMantissaLength;
			op = op.GetWithOtherML(mantissaLength, false);
		}
		MantissaLength = mantissaLength;
		if (op.e is null)
		{
			if (Mpir.MpuCmpSi(op.m, 0) == 0)
				m = ShiftedMantissaOverflow + 1;
			else
				m = (ShiftUniversal(Unsafe.As<MpzT>(op.m), MantissaLength - op.m.BitLength + 1) & MantissaMask) << 1;
			e = op.m.BitLength - 1;
		}
		else
		{
			m = Unsafe.As<MpzT>(op.m) << 1;
			e = op.e + (uint)(MantissaLength - 1);
		}
	}

	public LongReal(LongReal op) : this(op.m, op.e, op.MantissaLength) { }

	public LongReal(LongReal op, int mantissaLength)
		: this(op.GetWithOtherML(mantissaLength) is var x ? x.m : MpzT.Zero, x.e, mantissaLength) { }

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
			m = ShiftedMantissaOverflow + 1;
			e = new(MpuT.Zero, null, mantissaLength);
		}
		var mantissaByteLength = MantissaByteLength;
		if (bytes.Length <= mantissaByteLength)
		{
			m = new(bytes, order);
			e = new(MpuT.Zero, null, mantissaLength);
		}
		else
		{
			var mStart = Math.Max(order, 0) * (bytes.Length - mantissaByteLength);
			var eStart = Math.Max(-order, 0) * mantissaByteLength;
			m = new(bytes.Slice(mStart, mantissaByteLength), order);
			e = new UnsignedLongReal(bytes.Slice(eStart, bytes.Length - mantissaByteLength), order, mantissaLength);
		}
		var bitLength = m.BitLength;
		if (bitLength <= MantissaLength + 1 || Mpir.MpzCmp(m, ZeroMantissa) >= 0 && Mpir.MpzCmp(m, NaNMantissa) <= 0)
			return;
		var shiftAmount = bitLength - MantissaLength - 1;
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
	/// <summary>Gets the mathematical constant ln2.</summary>
	public static LongReal Ln2 { get; } = new(new MpzT("24967754427249423"
			+ "708525205747497001916726298924115674817853714105532799691947119558772265262657731881821679"
			+ "967175345660098658288646499683845961795537480371050165164918330420583275336170228392476911"
			+ "863201713467503604185996828422790561741495211377967712611750705722758763473224352283038658"
			+ "719646623920820701947449859866796858357314540111563746551555908371562368790829959955303474"
			+ "688178653879324378455075626438210264897657355700579597727386599835543043538782151160193950"
			+ "868207881451568910345457532214277663968636501301973715891225506562239342265260222480528658"
			+ "377131953208479986214776729405607414665548631974587246249519"), 0, DefaultMantissaLength);
	/// <summary>Gets the mathematical constant ln10.</summary>
	public static LongReal Ln10 { get; } = new(new MpzT("9778644287376780"
		+ "110269415607844281018128355904215069894977107489417638149475088934081129044237361753067437"
		+ "042600760324829712691829836159513949365867714554477208667457449291066005449636002063010622"
		+ "775296806656581131337705774889812169051978225238515112924104520035100593509272052567374533"
		+ "889503341322986068728442354509210388477204349091102699660012829882157117697859833156257204"
		+ "129275976388486747197476952749035406913923843228267462137550232670250495296551429315626322"
		+ "646989652823402720172396868237425618341432076845966216491401654786669011006535148805759371"
		+ "068055986562303903841554196631409082527647138479899794838076"), 1, DefaultMantissaLength);
	/// <summary>Gets the mathematical constant log₁₀2.</summary>
	public static LongReal Log10of2 { get; } = new(new MpzT("13193093437534837"
			+ "303673461252140622067065792820957531270659411632284461569252577010857745113059787258480204"
			+ "186472726335377931673446064383153736393503926492538757757561984120326268315316537998451554"
			+ "839523510621367800345588111718926389747284947354326946700039679660225431638470719198288276"
			+ "446710819745440722585243479695067370464255248517977286999371364226860463401868836916767885"
			+ "810612769020679044320362723051051466850550847762060664708019280295908792546704928761727222"
			+ "792247522640563384788844899176486907742221216650263284117521446698234236500962746490371493"
			+ "018765124548818722563691945476359472209241002518358953159865"), 1, DefaultMantissaLength);
	private int MantissaByteLength => (MantissaLength - 4) / 8 + 2;
	private MpzT MantissaMask =>
		this is var this2 ? MantissaMasks.GetOrAdd(MantissaLength, x => this2.MantissaOverflow - 1) : 0;
	private MpzT MantissaOverflow => MantissaOverflows.GetOrAdd(MantissaLength, x => Unsafe.As<MpzT>(MpuT.One << x));
	public static LongReal MultiplicativeIdentity => One;
	public static LongReal NaN { get; }
		= new((MpzT.One << MinMantissaLength + 1) + 4, UnsignedLongReal.Zero, MinMantissaLength);
	private MpzT NaNMantissa => ShiftedMantissaOverflow + 4;
	public static LongReal NegativeInfinity { get; }
		= new((MpzT.One << MinMantissaLength + 1) + 3, UnsignedLongReal.Zero, MinMantissaLength);
	private MpzT NegativeInfinityMantissa => ShiftedMantissaOverflow + 3;
	public static LongReal NegativeOne { get; } = new(-2, 0, MinMantissaLength);
	public static LongReal One { get; } = new(MpzT.Zero, UnsignedLongReal.Zero, MinMantissaLength);
	/// <summary>Получает (двоичный) порядок числа: количество бит в целой части для чисел &gt;= 1 и 0 для &lt; 1.</summary>
	public UnsignedLongReal Order => (m & 1u) != 0 ? new(MpuT.Zero, null, MantissaLength) : e + 1u;
	public static LongReal Pi { get; } = new(new MpzT("36892856717025391680"
			+ "740891802812412405176592852830664590007670367492169080340481831853321118904436015143933"
			+ "552972672082765245263376508596357945745324793896227010542086020897025607990336625291706026"
			+ "141155573427642072109590340260233176873111405413677348003751560516427907288614894025342811"
			+ "957248030664070398822430092170158931943496115579490752191456854647455877616932226540310715"
			+ "226431511337193553086781892270787770264832522584079134584415722107695941190721836268649424"
			+ "712276400413118826525211701140639299587367404718191375213418672365248997374612830495675957"
			+ "061903613212067735740210469522784694662354974839940735756692"), 1, DefaultMantissaLength);
	public static LongReal PositiveInfinity { get; }
		= new((MpzT.One << MinMantissaLength + 1) + 2, UnsignedLongReal.Zero, MinMantissaLength);
	private MpzT PositiveInfinityMantissa => ShiftedMantissaOverflow + 2;
	public static int Radix => 2;
	private MpzT ShiftedMantissaOverflow => MantissaOverflows.GetOrAdd(MantissaLength + 1, x => MpzT.One << x);

	/// <summary>Получает знак числа (в формате целого числа 1, 0 или -1).</summary>
	public int Sign
	{
		get
		{
			var shiftedMantissaOverflow = MpzT.One << MantissaLength + 1;
			if (Mpir.MpzCmp(m, shiftedMantissaOverflow) > 0 && Mpir.MpzCmp(m, shiftedMantissaOverflow + int.MaxValue) <= 0)
				return (int)(m - shiftedMantissaOverflow) switch
				{
					1 => 0,
					2 => 1,
					3 => -1,
					_ => throw new InvalidOperationException("Ошибка, невозможно вычислить знак у неопределенности!"),
				};
			return Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1;
		}
	}

	/// <summary>Получает порядок числа с учетом знака: положительный для положительных чисел и отрицательный иначе.</summary>
	public LongReal SignedOrder => (LongReal)Order * Sign;
	public static LongReal Tau { get; } = new(Pi.m, 2, DefaultMantissaLength);
	/// <summary>Gets the mathematical constant 10.</summary>
	public static LongReal Ten { get; } = new(new MpzT("16158503035655503"
			+ "650357438344334975980222051334857742016065172713762327569433945446598600705761456731844358"
			+ "980460949009747059779575245460547544076193224141560315438683650498045875098875194826053398"
			+ "028819192033784138396109321309878080919047169238085235290822926018152521443787945770532904"
			+ "303776199561965192760957166694834171210342487393282284747428088017663161029038902829665513"
			+ "096354230157075129296432088558362971801859230928678799175576150822952201848806616643615613"
			+ "562842355410104862578550863465661734839271290328348967522998634176499319107762583194718667"
			+ "771801067716614802322659239302476074096777926805529798115328"), 3, DefaultMantissaLength);
	/// <summary>Gets the mathematical constant 2.</summary>
	public static LongReal Two { get; } = new(MpzT.Zero, UnsignedLongReal.One, MinMantissaLength);
	public static LongReal Zero { get; }
		= new((MpzT.One << MinMantissaLength + 1) + 1, UnsignedLongReal.Zero, MinMantissaLength);
	private MpzT ZeroMantissa => ShiftedMantissaOverflow + 1;

	/// <summary>
	/// Computes the absolute of this number.
	/// </summary>
	/// <returns>The absolute of this number.</returns>
	public LongReal Abs() => Mpir.MpzCmpSi(m, 0) < 0 ? -this : this;
	public static LongReal Abs(LongReal value) => value.Abs();

	/// <summary>
	/// Вычисляет арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - <see cref="Pi"/> &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public LongReal Acos()
	{
		if (Abs() < One >> MantissaLength / 2)
			return Pi.GetWithOtherML(MantissaLength) >> 1;
		else if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if ((m & 1) == 0)
		{
			if (e != 0)
				return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
			else if (Mpir.MpzCmpSi(m, 0) == 0)
				return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
			else if (Mpir.MpzCmpSi(m, -2) == 0)
				return Pi.GetWithOtherML(MantissaLength);
			else
				return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		}
		var sign = Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1;
		var localValue = Abs().GetWithOtherML(MantissaLength + 100);
		var threshold = new LongReal(MpzT.Zero, UnsignedLongReal.One, MantissaLength + 100).SqrtInternal().ReciprocInternal();
		if (localValue == threshold)
			return (2 - sign) * Pi.GetWithOtherML(MantissaLength) >> 2;
		var reverse = localValue < threshold;
		if (reverse)
			localValue = AddInternal(One, -localValue.ReciprocInternal().SquareInternal().ReciprocInternal(),
				MantissaLength + 100).ReciprocInternal().SqrtInternal().ReciprocInternal();
		localValue = AddInternal(One, -AddInternal(One, -localValue, MantissaLength + 100) >> 1,
			MantissaLength + 100).ReciprocInternal().SqrtInternal().ReciprocInternal();
		localValue = AddInternal(One, -localValue.ReciprocInternal().SquareInternal().ReciprocInternal(),
			MantissaLength + 100).ReciprocInternal().SqrtInternal().ReciprocInternal();
		localValue = localValue.AsinInternal() << 1;
		if (reverse)
			localValue = (Pi.GetWithOtherML(MantissaLength + 100) >> 1) - localValue;
		if (sign < 0)
			localValue = Pi.GetWithOtherML(MantissaLength + 100) - localValue;
		return localValue.GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет арккосинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - <see cref="Pi"/> &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public static LongReal Acos(LongReal value) => value.Acos();

	/// <summary>
	/// Вычисляет гиперболический арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля, для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - гиперболический арккосинус данного числа.
	/// </returns>
	public LongReal Acosh() => Ln(this + Sqrt(Square() - One));

	/// <summary>
	/// Вычисляет гиперболический арккосинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля, для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический арккосинус данного числа.
	/// </returns>
	public static LongReal Acosh(LongReal value) => value.Acosh();

	private static LongReal AddInternal(LongReal x, LongReal y, int mantissaLength, int xmlDiff = 0, int ymlDiff = 0)
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
					newE = (switchE ? 0u : x.e - 1u).GetWithOtherML(mantissaLength, false);
					return new((xm + ym).ShiftRightRound(1) << 1 | (switchE ? 0 : 1), newE, mantissaLength);
				}
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					var switchE = x.e == 0;
					newE = (switchE ? 0u : x.e - 1u).GetWithOtherML(mantissaLength, false);
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
					return x.GetWithOtherML(mantissaLength);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1 | 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				newE = (x.e + 1u).GetWithOtherML(mantissaLength, false);
				return new((mDiff << 1 & mantissaMask) << 1 | 1, newE, mantissaLength);
			}
			else if (x.e == y.e)
			{
				var mDiff = xm - ym;
				if (mDiff == 0)
					return new((MpzT.One << mantissaLength + 1) + 1, UnsignedLongReal.Zero, mantissaLength);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				newE = (x.e + new UnsignedLongReal(shiftAmount)).GetWithOtherML(mantissaLength, false);
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
				newE = (x.e + (new UnsignedLongReal(shiftAmount) + 1u)).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
			}
		}
		else if ((y.m & 1) != 0)
		{
			if (Mpir.MpzCmpSi(ym, 0) >= 0)
			{
				var eDiff = x.e + y.e + 1;
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e + 1u).GetWithOtherML(mantissaLength, false);
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
					return x.GetWithOtherML(mantissaLength);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				var switchE = x.e == 0;
				newE = (switchE ? 0u : x.e - 1u).GetWithOtherML(mantissaLength, false);
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
					newE = (x.e + 1u).GetWithOtherML(mantissaLength, false);
					return new((xm + ym).ShiftRightRound(1) << 1, newE, mantissaLength);
				}
				if (eDiff > mantissaLength)
					return x.GetWithOtherML(mantissaLength);
				var mSum = xm + (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mSum, mantissaOverflow) >= 0)
				{
					newE = (x.e + 1u).GetWithOtherML(mantissaLength, false);
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
					return x.GetWithOtherML(mantissaLength);
				var mDiff = mantissaOverflow + xm - (mantissaOverflow + ym).ShiftRightRound((int)eDiff);
				if (Mpir.MpzCmp(mDiff, mantissaOverflow) >= 0)
					return new((mDiff & mantissaMask) << 1, x.e.GetWithOtherML(mantissaLength, true), mantissaLength);
				newE = (x.e - 1u).GetWithOtherML(mantissaLength, false);
				return new((mDiff << 1 & mantissaMask) << 1, newE, mantissaLength);
			}
			else if (x.e == y.e)
			{
				var mDiff = xm - ym;
				if (mDiff == 0)
					return new((MpzT.One << mantissaLength + 1) + 1, UnsignedLongReal.Zero, mantissaLength);
				var shiftAmount = mantissaLength - mDiff.BitLength + 1;
				if (x.e < shiftAmount)
				{
					newE = new(shiftAmount - (int)x.e - 1, mantissaLength);
					return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
				}
				newE = (x.e - new UnsignedLongReal(shiftAmount)).GetWithOtherML(mantissaLength, false);
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
				if (x.e <= shiftAmount)
				{
					newE = new(shiftAmount - (int)x.e, mantissaLength);
					return new((mDiff << shiftAmount & mantissaMask) << 1 | 1, newE, mantissaLength);
				}
				newE = (x.e - (new UnsignedLongReal(shiftAmount) + 1u)).GetWithOtherML(mantissaLength, false);
				return new((mDiff << shiftAmount & mantissaMask) << 1, newE, mantissaLength);
			}
		}
	}

	/// <summary>
	/// Вычисляет
	/// <see href="https://ru.wikipedia.org/wiki/Арифметико-геометрическое_среднее">арифметико-геометрическое среднее</see>
	/// указанных чисел.
	/// </summary>
	/// <param name="x">Первое число для вычисления арифметико-геометрического среднего.</param>
	/// <param name="y">Второе число для вычисления арифметико-геометрического среднего.</param>
	/// <returns>
	/// Если среди чисел есть неопределенность - неопределенность;<br />
	/// если одно число - ноль, а другое - бесконечность какого-либо знака - неопределенность;<br />
	/// есть ноль, но нет бесконечности - ноль;<br />
	/// бесконечность разных знаков - неопределенность;<br />
	/// бесконечность одного знака и число другого знака - неопределенность;<br />
	/// две бесконечности одного знака - бесконечность этого же знака;<br />
	/// бесконечность какого-либо знака и число этого же знака - бесконечность этого же знака;<br />
	/// числа разных знаков - неопределенность;<br />
	/// конечные числа одного знака - арифметико-геометрическое среднее этих чисел (подробнее см. по ссылке вsыше).
	/// </returns>
	public static LongReal AGM(LongReal x, LongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (Mpir.MpzCmp(x.m, x.NaNMantissa) == 0 || Mpir.MpzCmp(y.m, y.NaNMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0 || Mpir.MpzCmp(y.m, y.ZeroMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1)
				+ (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0
				|| Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0
				? 4 : 1), UnsignedLongReal.Zero, maxMantissaLength);
		else if ((Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			&& (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0))
			return new(Mpir.MpzCmp(x.m, y.m) == 0 ? x.m : (MpzT.One << maxMantissaLength + 1) + 4,
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(y.m, 0) < 0 ? 4 : 2),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(y.m, 0) < 0 ? 3 : 4),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(x.m, 0) < 0 ? 4 : 2),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(x.m, 0) < 0 ? 3 : 4),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0 ^ Mpir.MpzCmpSi(y.m, 0) < 0)
			return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmpSi(x.m, 0) == 0 && x.e == 0 && Mpir.MpzCmpSi(y.m, 0) == 0 && y.e == 0)
			return new(MpzT.Zero, UnsignedLongReal.Zero, maxMantissaLength);
		x = x.GetWithOtherML(maxMantissaLength);
		y = y.GetWithOtherML(maxMantissaLength);
		if (Mpir.MpzCmp(x.m, y.m) == 0 && x.e == y.e)
			return new(x.m, x.e, maxMantissaLength);
		if (y > x)
			(x, y) = (y, x);
		UnsignedLongReal shiftAmount = new(MpuT.Zero, null, maxMantissaLength);
		if ((x.m & 1) != 0)
			shiftAmount = x.e + 1u;
		if ((y.m & 1) != 0 && y.e >= shiftAmount)
			shiftAmount = y.e + 1u;
		if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -AGMInternal(-y << shiftAmount, -x << shiftAmount, maxMantissaLength) >> shiftAmount;
		else
			return AGMInternal(x << shiftAmount, y << shiftAmount, maxMantissaLength) >> shiftAmount;
	}

	private static LongReal AGMInternal(LongReal x, LongReal y, int maxMantissaLength)
	{
		LongReal a = x, b = y;
		do
			(a, b) = (AddInternal(a, b, maxMantissaLength) >> 1, GeometricMeanInternal(a, b, maxMantissaLength));
		while (a.e != b.e || Mpir.MpzCmpabsUi((a.m >> 1) - (b.m >> 1), 1) > 0);
		return a;
	}

	/// <summary>
	/// Вычисляет арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арксинус данного числа.
	/// </returns>
	public LongReal Asin()
	{
		if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if ((m & 1) == 0)
		{
			if (e != 0)
				return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
			else if (Mpir.MpzCmpSi(m, 0) == 0)
				return Pi.GetWithOtherML(MantissaLength) >> 1;
			else if (Mpir.MpzCmpSi(m, -2) == 0)
				return -Pi.GetWithOtherML(MantissaLength) >> 1;
			else
				return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		}
		var sign = Mpir.MpzCmpSi(m, 0) < 0 ? -1 : 1;
		var localValue = Abs().GetWithOtherML(MantissaLength + 100);
		var threshold = new LongReal(MpzT.Zero, UnsignedLongReal.One, MantissaLength + 100).SqrtInternal().ReciprocInternal();
		if (localValue == threshold)
			return (2 - sign) * Pi.GetWithOtherML(MantissaLength) >> 2;
		var reverse = localValue > threshold;
		if (!reverse)
			localValue = AddInternal(One, -localValue.ReciprocInternal().SquareInternal().ReciprocInternal(),
				MantissaLength + 100).ReciprocInternal().SqrtInternal().ReciprocInternal();
		localValue = AddInternal(One, -AddInternal(One, -localValue, MantissaLength + 100) >> 1,
			MantissaLength + 100).ReciprocInternal().SqrtInternal().ReciprocInternal();
		localValue = AddInternal(One, -localValue.ReciprocInternal().SquareInternal().ReciprocInternal(),
			MantissaLength + 100).ReciprocInternal().SqrtInternal().ReciprocInternal();
		localValue = localValue.AsinInternal() << 1;
		if (reverse)
			localValue = (Pi.GetWithOtherML(MantissaLength + 100) >> 1) - localValue;
		return (localValue * sign).GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет арксинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арксинус данного числа.
	/// </returns>
	public static LongReal Asin(LongReal value) => value.Asin();

	/// <summary>
	/// Вычисляет гиперболический арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арксинус данного числа.
	/// </returns>
	public LongReal Asinh() => Ln(this + Sqrt(Square() + One));

	/// <summary>
	/// Вычисляет гиперболический арксинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арксинус данного числа.
	/// </returns>
	public static LongReal Asinh(LongReal value) => value.Asinh();

	private LongReal AsinInternal()
	{
		var inverseSquare = ReciprocInternal().SquareInternal();
		LongReal frac = inverseSquare >> 2, fracPow = frac;
		LongReal factorial = new(MpzT.Zero, UnsignedLongReal.Zero, MantissaLength);
		LongReal doubleFactorial = new(MpzT.Zero, UnsignedLongReal.One, MantissaLength);
		LongReal rowSum = inverseSquare.ReciprocInternal() << 1, prev;
		uint i = 2u, i2 = 3;
		do
		{
			prev = rowSum;
#pragma warning disable IDE0079 // Удалить ненужное подавление
#pragma warning disable S1121
			rowSum += MultiplyInternal(fracPow = MultiplyInternal(fracPow, frac, MantissaLength),
				MultiplyUiInternal(DivideInternal(doubleFactorial
				= MultiplyUiInternal(doubleFactorial, i2++ * i2++, MantissaLength),
				MultiplyInternal(factorial = MultiplyUiInternal(factorial, i, MantissaLength), factorial, MantissaLength),
				MantissaLength), i * i, MantissaLength), MantissaLength).ReciprocInternal();
			i++;
#pragma warning restore S1121
#pragma warning restore IDE0079 // Удалить ненужное подавление
		} while (rowSum.e != prev.e || Mpir.MpzCmpabsUi((rowSum.m >> 1) - (prev.m >> 1), 1) > 0);
		return (rowSum >> 1).ReciprocInternal().SqrtInternal().ReciprocInternal();
	}

	/// <summary>
	/// Вычисляет арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арктангенс данного числа.
	/// </returns>
	public LongReal Atan()
	{
		var cos = Sqrt(1 / (1 + Square()));
		return Acos(cos) * Sign;
	}

	/// <summary>
	/// Вычисляет арктангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арктангенс данного числа.
	/// </returns>
	public static LongReal Atan(LongReal value) => value.Atan();

	public static LongReal Atan2(LongReal y, LongReal x)
	{
		if (Mpir.MpzCmp(x.m, x.NaNMantissa) == 0 || Mpir.MpzCmp(y.m, y.NaNMantissa) == 0
			|| Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0 && Mpir.MpzCmp(y.m, y.ZeroMantissa) == 0)
			return NaN;
		var pi = Pi.GetWithOtherML(Math.Max(x.MantissaLength, y.MantissaLength));
		if (Abs(x) < Abs(y))
		{
			if (x / y == Zero)
				return y <= Zero ? -pi / 2 : pi / 2;
			var atan = Atan(y / x);
			if (x >= Zero)
				return atan;
			else if (y > Zero)
				return pi + atan;
			else
				return atan - pi;
		}
		else if (y / x == Zero)
			return x > Zero ? Zero : pi;
		else
		{
			var atan = Atan(x / y);
			return y > Zero ? pi / 2 - atan : -atan - pi / 2;
		}
	}

	/// <summary>
	/// Вычисляет гиперболический арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public LongReal Atanh() => Ln((One + this) / (One - this)) >> 1;

	/// <summary>
	/// Вычисляет гиперболический арктангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public static LongReal Atanh(LongReal value) => value.Atanh();

	/// <summary>
	/// Возвращает наименьшее целое число, которое не меньше данного числа:
	/// само данное число для целых и ближайшее сверху целое для дробных.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public LongReal Ceiling()
	{
		var truncated = Truncate();
		if (this > 0 && truncated != this)
			truncated++;
		return truncated;
	}

	/// <summary>
	/// Возвращает наименьшее целое число, которое не меньше указанного числа:
	/// само число <paramref name="value"/> для целых и ближайшее сверху целое для дробных.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - <paramref name="value"/>;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public static LongReal Ceiling(LongReal value) => value.Ceiling();

	/// <summary>
	/// Вычисляет гиперболический косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус данного числа.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Cosh()">Cosh()</see>.</remarks>
	public LongReal Ch() => Cosh();

	/// <summary>
	/// Вычисляет гиперболический косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус <paramref name="value"/>.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Cosh(LongReal)">Cosh()</see>.</remarks>
	public static LongReal Ch(LongReal value) => value.Cosh();

	public object Clone() => new LongReal(m, e, MantissaLength);

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
		if (Mpir.MpzCmp(m, NaNMantissa) == 0)
			return int.MinValue;
		else if (Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0)
			return -1;
		else if (Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0)
			return 1;
		else if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return -Mpir.MpzCmpSi(other, 0);
		else if (Mpir.MpzCmpSi(m, 0) < 0 && Mpir.MpzCmpSi(other, 0) >= 0)
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
		if (Mpir.MpzCmp(m, NaNMantissa) == 0 || Mpir.MpzCmp(other.m, other.NaNMantissa) == 0)
			return int.MinValue;
		else if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0 && Mpir.MpzCmp(other.m, other.ShiftedMantissaOverflow) > 0
			&& Mpir.MpzCmp(m - ShiftedMantissaOverflow, other.m - other.ShiftedMantissaOverflow) == 0)
			return 0;
		else if (Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0)
			return -1;
		else if (Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0)
			return 1;
		else if (Mpir.MpzCmp(other.m, other.NegativeInfinityMantissa) == 0)
			return 1;
		else if (Mpir.MpzCmp(other.m, other.PositiveInfinityMantissa) == 0)
			return -1;
		else if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return Mpir.MpzCmpSi(other.m, 0) < 0 ? 1 : -1;
		else if (Mpir.MpzCmp(other.m, other.ZeroMantissa) == 0)
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
		if (Mpir.MpzCmpSi(m, 0) < 0)
			return ShiftUniversal(~(other.m >> 1), mantissaLength - other.MantissaLength)
				.CompareTo(ShiftUniversal(~(m >> 1), mantissaLength - MantissaLength));
		else
			return ShiftUniversal(m >> 1, mantissaLength - MantissaLength)
				.CompareTo(ShiftUniversal(other.m >> 1, mantissaLength - other.MantissaLength));
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
		float f => CompareTo(f),
		double d => CompareTo(d),
		LongReal lr => CompareTo(lr),
		BigInteger bi => CompareTo(new MpzT(bi)),
		IComparable ic => -ic.CompareTo(this),
		_ => 0,
	};

	/// <summary>
	/// Вычисляет косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - косинус данного числа.
	/// </returns>
	public LongReal Cos()
	{
		if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new((int)(m - ShiftedMantissaOverflow) switch
			{
				1 => 0,
				_ => ShiftedMantissaOverflow + 4,
			}, UnsignedLongReal.Zero, MantissaLength);
		var abs = Abs();
		if (abs >= Tau << MantissaLength)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		var divisor = Tau.GetWithOtherML(MantissaLength + 100);
		var localValue = abs.GetWithOtherML(MantissaLength + 100) % divisor;
		var oldDivisor = divisor;
		divisor >>= 1;
		if (localValue == divisor)
			return new(-2, 0, MantissaLength);
		if (localValue >= divisor)
			localValue = oldDivisor - localValue;
		oldDivisor = divisor;
		if (localValue == (divisor >>= 1))
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		(var sign, localValue) = localValue >= divisor ? (-1, oldDivisor - localValue) : (1, localValue);
		if (Mpir.MpzCmp(localValue.m, localValue.ZeroMantissa) == 0)
			return new(sign - 1, 0, MantissaLength);
		oldDivisor = divisor;
		divisor >>= 1;
		(var reverse, localValue) = (localValue >= divisor) ? (true, oldDivisor - localValue) : (false, localValue);
		var cos = localValue.CosInternal();
		if (!reverse)
			return (cos * sign).GetWithOtherML(MantissaLength);
		else if (cos.e == 0 && Mpir.MpzCmpSi(cos.m, 0) == 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		else
			return (AddInternal(One, -cos.ReciprocInternal().SquareInternal().ReciprocInternal(), MantissaLength + 100)
				.ReciprocInternal().SqrtInternal().ReciprocInternal() * sign).GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - косинус <paramref name="value"/>.
	/// </returns>
	public static LongReal Cos(LongReal value) => value.Cos();

	/// <summary>
	/// Вычисляет гиперболический косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус данного числа.
	/// </returns>
	public LongReal Cosh()
	{
		var exp = Exp();
		return exp + exp.Reciproc() >> 1;
	}

	/// <summary>
	/// Вычисляет гиперболический косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус <paramref name="value"/>.
	/// </returns>
	public static LongReal Cosh(LongReal value) => value.Cosh();

	private LongReal CosInternal()
	{
		var frac = Reciproc().SquareInternal();
		var rowSum = AddInternal(new(MpzT.Zero, UnsignedLongReal.Zero, MantissaLength), -frac.Reciproc() >> 1, MantissaLength);
		LongReal factorial = new(MpzT.Zero, UnsignedLongReal.One, MantissaLength), prev;
		var fracExponent = frac;
		var i = 3L;
		do
		{
			prev = rowSum;
#pragma warning disable IDE0079 // Удалить ненужное подавление
#pragma warning disable S1121
			rowSum = AddInternal(rowSum,
				AddInternal(MultiplyInternal(fracExponent = MultiplyInternal(fracExponent, frac, MantissaLength),
				factorial = MultiplyInternal(factorial, new(i++ * i++, MantissaLength), MantissaLength),
				MantissaLength).ReciprocInternal(),
				-MultiplyInternal(fracExponent = MultiplyInternal(fracExponent, frac, MantissaLength),
				factorial = MultiplyInternal(factorial, new(i++ * i++, MantissaLength), MantissaLength),
				MantissaLength).ReciprocInternal(), MantissaLength), MantissaLength);
#pragma warning restore S1121
#pragma warning restore IDE0079 // Удалить ненужное подавление
		} while (prev.e != rowSum.e || Mpir.MpzCmpabsUi((prev.m >> 1) - (rowSum.m >> 1), 1) > 0);
		return rowSum;
	}

	private static LongReal DivideInternal(LongReal x, LongReal y, int maxMantissaLength)
	{
		var MantissaOverflow = MpzT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var quotient = (MantissaOverflow + (x.m >> 1) << maxMantissaLength + 1) / (MantissaOverflow + (y.m >> 1));
		var shiftAmount = quotient.BitLength - maxMantissaLength - 1;
		quotient = quotient.ShiftRightRound(shiftAmount) & MantissaMask;
		if (x.e + new UnsignedLongReal(shiftAmount) >= y.e + 1u)
			return new(quotient << 1, x.e - y.e + new UnsignedLongReal(shiftAmount) - 1u, maxMantissaLength);
		else
			return new(quotient << 1 | 1, y.e - x.e - new UnsignedLongReal(shiftAmount), maxMantissaLength);
	}

	private static LongReal DivideUiInternal(LongReal x, uint y, int MantissaLength)
	{
		var MantissaOverflow = MpzT.One << MantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var quotient = (MantissaOverflow + (x.m >> 1) << MantissaLength + 1) / y;
		var shiftAmount = MantissaLength * 2 + 2 - quotient.BitLength;
		var mantissa = (quotient.ShiftRightRound(MantissaLength - shiftAmount + 1) & MantissaMask) << 1 | x.m & 1;
		if ((x.m & 1) != 0)
			return new(mantissa, x.e + new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(mantissa, x.e - new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else
			return new(mantissa | 1, new UnsignedLongReal(shiftAmount) - x.e - 1u, x.MantissaLength);
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
		byte y => CompareTo((int)y) == 0,
		short si => CompareTo(si) == 0,
		ushort usi => CompareTo((int)usi) == 0,
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
	/// Вычисляет e в степени данного числа (экспоненту).
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - e в степени данного числа (экспонента).
	/// </returns>
	public LongReal Exp() => (this / Ln2.GetWithOtherML(MantissaLength)).PowerOf2();

	/// <summary>
	/// Вычисляет e в степени указанного числа (экспоненту).
	/// </summary>
	/// <param name="value">Показатель для вычисления экспоненты.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - e в степени <paramref name="value"/> (экспонента).
	/// </returns>
	public static LongReal Exp(LongReal value) => (value / Ln2.GetWithOtherML(value.MantissaLength)).PowerOf2();

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше данного числа:
	/// само данное число для целых и ближайшее снизу целое для дробных.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public LongReal Floor()
	{
		var truncated = Truncate();
		if (this < 0 && truncated != this)
			truncated--;
		return truncated;
	}

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше указанного числа:
	/// само число <paramref name="value"/> для целых и ближайшее снизу целое для дробных.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - <paramref name="value"/>;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public static LongReal Floor(LongReal value) => value.Floor();

	/// <summary>
	/// Возвращает дробную часть данного числа, положительную для положительных чисел
	/// и отрицательную для отрицательных, отбрасывая целую часть.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для целых чисел - ноль;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public LongReal Frac() => this - Truncate();

	/// <summary>
	/// Возвращает дробную часть <paramref name="value"/>, положительную для положительных чисел
	/// и отрицательную для отрицательных, отбрасывая целую часть.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для целых чисел - ноль;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public static LongReal Frac(LongReal value) => value.Frac();

	/// <summary>
	/// Вычисляет геометрическое среднее указанных чисел.
	/// </summary>
	/// <param name="x">Первое число для вычисления геометрического среднего.</param>
	/// <param name="y">Второе число для вычисления геометрического среднего.</param>
	/// <returns>
	/// Если среди чисел есть неопределенность - неопределенность;<br />
	/// если одно число - ноль, а другое - бесконечность какого-либо знака - неопределенность;<br />
	/// есть ноль, но нет бесконечности - ноль;<br />
	/// бесконечность разных знаков - неопределенность;<br />
	/// бесконечность одного знака и число другого знака - неопределенность;<br />
	/// две бесконечности одного знака - бесконечность этого же знака;<br />
	/// бесконечность какого-либо знака и число этого же знака - бесконечность этого же знака;<br />
	/// числа разных знаков - неопределенность;<br />
	/// конечные числа одного знака - квадратный корень этого же знака из их произведения.
	/// </returns>
	public static LongReal GeometricMean(LongReal x, LongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (Mpir.MpzCmp(x.m, x.NaNMantissa) == 0 || Mpir.MpzCmp(y.m, y.NaNMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0 || Mpir.MpzCmp(y.m, y.ZeroMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1)
				+ (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0
				|| Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0
				? 4 : 1), UnsignedLongReal.Zero, maxMantissaLength);
		else if ((Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			&& (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0))
			return new(Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa)
				? x.m : x.NaNMantissa, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(y.m, 0) < 0 ? 4 : 2),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(y.m, 0) < 0 ? 3 : 4),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(x.m, 0) < 0 ? 4 : 2),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(x.m, 0) < 0 ? 3 : 4),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0 ^ Mpir.MpzCmpSi(y.m, 0) < 0)
			return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmpSi(x.m, 0) == 0 && x.e == 0 && Mpir.MpzCmpSi(y.m, 0) == 0 && y.e == 0)
			return new(MpzT.Zero, UnsignedLongReal.Zero, maxMantissaLength);
		x = x.GetWithOtherML(maxMantissaLength);
		y = y.GetWithOtherML(maxMantissaLength);
		if (Mpir.MpzCmp(x.m, y.m) == 0 && x.e == y.e)
			return new(x.m, x.e, maxMantissaLength);
		var xShiftAmount = (x.m & 1u) != 0u ? x.e + 1u : 0u;
		var yShiftAmount = (y.m & 1u) != 0u ? y.e + 1u : 0u;
		if ((xShiftAmount & 1) != (yShiftAmount & 1))
			xShiftAmount++;
		var resultShiftAmount = xShiftAmount + yShiftAmount >> 1;
		if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -GeometricMeanInternal(-x << xShiftAmount, -y << yShiftAmount, maxMantissaLength) >> resultShiftAmount;
		else
			return GeometricMeanInternal(x << xShiftAmount, y << yShiftAmount, maxMantissaLength) >> resultShiftAmount;
	}

	private static LongReal GeometricMeanInternal(LongReal x, LongReal y, int maxMantissaLength) =>
		MultiplyInternal(x, y, maxMantissaLength).SqrtInternal();

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

	internal LongReal GetWithOtherML(int mantissaLength)
	{
		if (mantissaLength == MantissaLength)
			return this;
		if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new((MpzT.One << mantissaLength + 1) + (m - ShiftedMantissaOverflow), e, mantissaLength);
		var mantissa = m >> 1;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			mantissa = ~mantissa;
		mantissa = ShiftUniversal(mantissa, mantissaLength - MantissaLength);
		if (Mpir.MpzCmpSi(m, 0) < 0)
			mantissa = ~mantissa;
		return new(mantissa << 1 | m & 1, e, mantissaLength);
	}

	public static bool IsCanonical(LongReal value) => true;
	public static bool IsComplexNumber(LongReal value) => true;
	/// <summary>Проверяет, является ли данное число четным (возвращает true или false).</summary>
	public bool IsEven() => Mpir.MpzCmp(m, ZeroMantissa) == 0 || (m & 1) == 0 && e >= 1
		&& (e > MantissaLength || Mpir.MpzCmpSi(m, 0) == 0 || TrailingZeroCount(m >> 1) >= MantissaLength - (int)e + 1);
	public static bool IsEvenInteger(LongReal value) => value.IsEven();
	public static bool IsFinite(LongReal value) => true;
	public static bool IsImaginaryNumber(LongReal value) => false;
	public static bool IsInfinity(LongReal value) =>
		Mpir.MpzCmp(value.m, value.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(value.m, value.NegativeInfinityMantissa) == 0;
	/// <summary>Проверяет, является ли данное число целым (возвращает true или false).</summary>
	public bool IsInteger() => Mpir.MpzCmp(m, ZeroMantissa) == 0 || (m & 1) == 0
		&& (e > MantissaLength || Mpir.MpzCmpSi(m, 0) == 0 || TrailingZeroCount(m >> 1) >= MantissaLength - (int)e);
	public static bool IsInteger(LongReal value) => value.IsInteger();
	public static bool IsNaN(LongReal value) => Mpir.MpzCmp(value.m, value.NaNMantissa) == 0;
	public static bool IsNegative(LongReal value) => Mpir.MpzCmpSi(value.m, 0) < 0;
	public static bool IsNegativeInfinity(LongReal value) => Mpir.MpzCmp(value.m, value.NegativeInfinityMantissa) == 0;
	public static bool IsNormal(LongReal value) => true;
	public static bool IsOddInteger(LongReal value) =>
		Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) <= 0 && (value.m & 1) == 0 && value.e <= value.MantissaLength
		&& TrailingZeroCount(value.m >> 1) == value.MantissaLength - (int)value.e;
	public static bool IsPositive(LongReal value) =>
		Mpir.MpzCmpSi(value.m, 0) >= 0 && Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) <= 0;
	public static bool IsPositiveInfinity(LongReal value) => Mpir.MpzCmp(value.m, value.PositiveInfinityMantissa) == 0;
	public static bool IsRealNumber(LongReal value) => true;
	public static bool IsSubnormal(LongReal value) => false;
	public static bool IsZero(LongReal value) => Mpir.MpzCmp(value.m, value.ZeroMantissa) == 0;

	/// <summary>
	/// Вычисляет натуральный логарифм данного числа (по основанию e).
	/// </summary>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - натуральный логарифм данного числа.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Log()">Log()</see>.</remarks>
	public LongReal Ln() => Log();

	/// <summary>
	/// Вычисляет натуральный логарифм указанного числа (по основанию e).
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - натуральный логарифм <paramref name="value"/>.
	/// </returns>
	/// <remarks>
	/// Данный метод называется только Ln(), без варианта Log(), так как уже существует метод
	/// <see cref="Log(LongReal)">Log()</see> с одним параметром типа <see cref="LongReal"/>,
	/// а два метода с одинаковыми количеством и типами параметров невозможны,
	/// даже если один статический, а другой экземплярный.
	/// </remarks>
	public static LongReal Ln(LongReal value) => value.Log();

	/// <summary>
	/// Вычисляет натуральный логарифм данного числа (по основанию e).
	/// </summary>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - натуральный логарифм данного числа.
	/// </returns>
	/// <remarks>Данный метод также имеет альтернативное название <see cref="Ln()">Ln()</see>.</remarks>
	public LongReal Log()
	{
		if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return new(ShiftedMantissaOverflow + 3, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0)
			return new(ShiftedMantissaOverflow + 2, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0 || Mpir.MpzCmp(m, NaNMantissa) == 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) < 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) == 0 && e == 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		else if (this == E)
			return new(MpzT.Zero, UnsignedLongReal.Zero, MantissaLength);
		else if (e > int.MaxValue)
		{
			var mLog = LogInternal(new LongReal(MantissaOverflow + (m >> 1), MantissaLength + 100) >> MantissaLength);
			var eLog = (m & 1) != 0 ? ~e : e;
			return (eLog * Ln2.GetWithOtherML(MantissaLength + 100) + mLog).GetWithOtherML(MantissaLength);
		}
		else if ((m & 1) != 0)
			return -LogInternal(One / GetWithOtherML(MantissaLength + 100)).GetWithOtherML(MantissaLength);
		else
			return LogInternal(GetWithOtherML(MantissaLength + 100)).GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет логарифм данного числа по основанию <paramref name="base"/>.
	/// </summary>
	/// <param name="base">Основание логарифма.</param>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм данного числа по основанию <paramref name="base"/>.
	/// </returns>
	public LongReal Log(LongReal @base)
	{
		var maxMantissaLength = Math.Max(MantissaLength, @base.MantissaLength);
		return GetWithOtherML(maxMantissaLength).Log() / @base.GetWithOtherML(maxMantissaLength).Log();
	}

	/// <summary>
	/// Вычисляет логарифм указанного числа по указанному основанию.
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <param name="base">Основание логарифма.</param>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм <paramref name="value"/> по основанию <paramref name="base"/>.
	/// </returns>
	public static LongReal Log(LongReal value, LongReal @base) => value.Log(@base);

	/// <summary>
	/// Вычисляет логарифм данного числа по основанию 2.
	/// </summary>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм данного числа по основанию 2.
	/// </returns>
	public LongReal Log2()
	{
		if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return new(ShiftedMantissaOverflow + 3, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0)
			return new(ShiftedMantissaOverflow + 2, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0 || Mpir.MpzCmp(m, NaNMantissa) == 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) < 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) == 0 && e == 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) == 0)
			return new(e, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 1) == 0)
			return -new LongReal(e + UnsignedLongReal.One, MantissaLength);
		else if (e > int.MaxValue)
		{
			var mLog = LogInternal(new LongReal(MantissaOverflow + (m >> 1), MantissaLength + 100) >> MantissaLength);
			var eLog = (m & 1) != 0 ? ~e : e;
			return (eLog + mLog / Ln2.GetWithOtherML(MantissaLength + 100)).GetWithOtherML(MantissaLength);
		}
		else if ((m & 1) != 0)
			return -(LogInternal(One / GetWithOtherML(MantissaLength + 100)) / Ln2.GetWithOtherML(MantissaLength + 100))
			.GetWithOtherML(MantissaLength);
		else
			return (LogInternal(GetWithOtherML(MantissaLength + 100)) / Ln2.GetWithOtherML(MantissaLength + 100))
			.GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет логарифм указанного числа по основанию 2.
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм <paramref name="value"/> по основанию 2.
	/// </returns>
	public static LongReal Log2(LongReal value) => value.Log2();

	/// <summary>
	/// Вычисляет логарифм данного числа по основанию 10.
	/// </summary>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм данного числа по основанию 10.
	/// </returns>
	public LongReal Log10() => Log() / Ln10.GetWithOtherML(MantissaLength);

	/// <summary>
	/// Вычисляет логарифм указанного числа по основанию 10.
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <returns>
	/// Для нуля - минус бесконечность;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм <paramref name="value"/> по основанию 10.
	/// </returns>
	public static LongReal Log10(LongReal value) => value.Log() / Ln10.GetWithOtherML(value.MantissaLength);

	private static LongReal LogInternal(LongReal value)
	{
		var m = GetArrayLength(value.MantissaLength, 2);
		var s = value * new LongReal(0, m, value.MantissaLength);
		return (Pi.GetWithOtherML(value.MantissaLength) / AGM(One, 4 / s) >> 1)
			- m * Ln2.GetWithOtherML(value.MantissaLength);
	}

	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	public static LongReal Max(LongReal x) => x;
	public static LongReal Max(LongReal x, LongReal y) => x.CompareTo(y) >= 0 ? x : y;
	/// <summary>Возвращает максимальное из трех значений.</summary>
	public static LongReal Max(LongReal x, LongReal y, LongReal z) => Max(Max(x, y), z);
	/// <summary>Возвращает максимальное из произвольного количества значений.</summary>
	public static LongReal Max(params LongReal[] values) => values.Length == 0 ? Zero : values.Progression(Max);
	public static LongReal MaxMagnitude(LongReal x, LongReal y) => x.CompareTo(y) >= 0 ? x : y;
	public static LongReal MaxMagnitudeNumber(LongReal x, LongReal y) => x.CompareTo(y) >= 0 ? x : y;
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	public static LongReal Mean(LongReal x) => x;
	/// <summary>Возвращает арифметическое среднее двух значений - (x + y) / 2.</summary>
	public static LongReal Mean(LongReal x, LongReal y) => (x + y) >> 1;
	/// <summary>Возвращает арифметическое среднее трех значений - (x + y + z) / 3.</summary>
	public static LongReal Mean(LongReal x, LongReal y, LongReal z) => (x + y + z) / 3;
	/// <summary>Возвращает арифметическое среднее произвольного количества значений - (x + y + z) / 3.</summary>
	public static LongReal Mean(params LongReal[] values) =>
		values.Length == 0 ? Zero : values.Progression((x, y) => x + y) / values.Length;
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	public static LongReal Min(LongReal x) => x;
	public static LongReal Min(LongReal x, LongReal y) => x.CompareTo(y) < 0 ? x : y;
	/// <summary>Возвращает минимальное из трех значений.</summary>
	public static LongReal Min(LongReal x, LongReal y, LongReal z) => Min(Min(x, y), z);
	/// <summary>Возвращает минимальное из произвольного количества значений.</summary>
	public static LongReal Min(params LongReal[] values) => values.Length == 0 ? Zero : values.Progression(Min);
	public static LongReal MinMagnitude(LongReal x, LongReal y) => x.CompareTo(y) < 0 ? x : y;
	public static LongReal MinMagnitudeNumber(LongReal x, LongReal y) => x.CompareTo(y) < 0 ? x : y;

	private static LongReal MultiplyInternal(LongReal x, LongReal y, int maxMantissaLength)
	{
		var MantissaOverflow = MpzT.One << maxMantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var product = (MantissaOverflow + (x.m >> 1)) * (MantissaOverflow + (y.m >> 1));
		var shiftAmount = product.BitLength - maxMantissaLength - 1;
		var shifted = product.ShiftRightRound(shiftAmount);
		if (Mpir.MpzCmp(shifted, MantissaOverflow << 1) == 0)
			shiftAmount++;
		return new((shifted & MantissaMask) << 1, x.e + y.e + (new UnsignedLongReal(shiftAmount) - (uint)maxMantissaLength),
			maxMantissaLength);
	}

	private static LongReal MultiplyUiInternal(LongReal x, uint y, int mantissaLength)
	{
		var MantissaOverflow = MpzT.One << mantissaLength;
		var MantissaMask = MantissaOverflow - 1;
		var product = (MantissaOverflow + (x.m >> 1)) * y;
		var shiftAmount = product.BitLength - mantissaLength - 1;
		var mantissa = (product.ShiftRightRound(shiftAmount) & MantissaMask) << 1 | x.m & 1;
		if ((x.m & 1) == 0)
			return new(mantissa, x.e + new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(mantissa, x.e - new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else
			return new(mantissa & new MpzT(-2), new UnsignedLongReal(shiftAmount) - x.e - 1u, x.MantissaLength);
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
		for (var i = BitsPerInt - (int)uint.LeadingZeroCount(exponent) - 2; i >= 0; i--)
		{
			result *= result;
			if ((exponent & 1u << i) != 0)
				result *= this;
		}
		return result;
	}

	/// <summary>
	/// Вычисляет данное число в степени указанного числа.
	/// </summary>
	/// <param name="exponent">Показатель для вычисления степени.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - данное число в степени <paramref name="exponent"/>.
	/// </returns>
	public LongReal Power(LongReal exponent)
	{
		var maxMantissaLength = Math.Max(MantissaLength, exponent.MantissaLength);
		if (exponent.IsInteger() && exponent < uint.MaxValue)
			return GetWithOtherML(maxMantissaLength).Power((uint)exponent);
		return (exponent.GetWithOtherML(maxMantissaLength) * GetWithOtherML(maxMantissaLength).Log2()).PowerOf2();
	}

	/// <summary>
	/// Вычисляет указанное основание в степени указанного показателя.
	/// </summary>
	/// <param name="base">Основание для вычисления степени.</param>
	/// <param name="exponent">Показатель для вычисления степени.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - <paramref name="base"/> в степени <paramref name="exponent"/>.
	/// </returns>
	public static LongReal Power(LongReal @base, LongReal exponent) => @base.Power(exponent);

	/// <summary>
	/// Вычисляет 2 в степени данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - 2 в степени данного числа.
	/// </returns>
	public LongReal PowerOf2()
	{
		if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new((int)(m - ShiftedMantissaOverflow) switch
			{
				1 => 0,
				2 => ShiftedMantissaOverflow + 2,
				3 => ShiftedMantissaOverflow + 1,
				4 => ShiftedMantissaOverflow + 4,
				_ => throw new InvalidOperationException("Невозможно возвести в степень. Возможные причины:\r\n"
					+ InternalError + $"Текущее состояние: число - {this},"
					+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}"),
			}, UnsignedLongReal.Zero, MantissaLength);
		var floor = Floor();
		LongReal floorExponent = floor < 0 ? new(MpzT.One, (UnsignedLongReal)~floor, MantissaLength)
			: new(MpzT.Zero, (UnsignedLongReal)floor, MantissaLength);
		var fracOriginal = (GetWithOtherML(MantissaLength * 2) - floor) * Ln2.GetWithOtherML(MantissaLength * 2);
		if (Mpir.MpzCmp(fracOriginal.m, fracOriginal.ZeroMantissa) == 0)
			return floorExponent;
		LongReal frac = fracOriginal.ReciprocInternal(), fracPow = frac;
		LongReal factorial = new(MpzT.Zero, UnsignedLongReal.Zero, MantissaLength * 2);
		LongReal rowSum = factorial + fracOriginal, prev;
		var i = 2u;
		do
		{
			prev = rowSum;
#pragma warning disable IDE0079 // Удалить ненужное подавление
#pragma warning disable S1121
			rowSum += MultiplyInternal(fracPow = MultiplyInternal(fracPow, frac, MantissaLength * 2),
				factorial = MultiplyUiInternal(factorial, i++, MantissaLength * 2), MantissaLength * 2).ReciprocInternal();
#pragma warning restore S1121
#pragma warning restore IDE0079 // Удалить ненужное подавление
		} while (rowSum.e != prev.e || Mpir.MpzCmpabsUi((rowSum.m >> 1) - (prev.m >> 1), 1) > 0);
		return floorExponent * rowSum.GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет 2 в степени указанного числа.
	/// </summary>
	/// <param name="value">Показатель для вычисления степени.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - 2 в степени <paramref name="value"/>.
	/// </returns>
	public static LongReal PowerOf2(LongReal value) => value.PowerOf2();

	/// <summary>
	/// Вычисляет 10 в степени данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - 10 в степени данного числа.
	/// </returns>
	public LongReal PowerOf10() => (this / Log10of2.GetWithOtherML(MantissaLength)).PowerOf2();

	/// <summary>
	/// Вычисляет 10 в степени указанного числа.
	/// </summary>
	/// <param name="value">Показатель для вычисления степени.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - 10 в степени <paramref name="value"/>.
	/// </returns>
	public static LongReal PowerOf10(LongReal value) => value.PowerOf10();

	/// <summary>
	/// Вычисляет число, обратное данному (1 / x).
	/// </summary>
	/// <returns>
	/// Для нуля - плюс бесконечность;<br />
	/// для плюс бесконечности или для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - число, обратное данному (1 / x).
	/// </returns>
	public LongReal Reciproc()
	{
		if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new(ShiftedMantissaOverflow + (int)(m - ShiftedMantissaOverflow) switch
			{
				1 => 2,
				2 or 3 => 1,
				4 => 4,
				_ => throw new InvalidOperationException("Невозможно вычислить обратное число. Возможные причины:\r\n"
					+ InternalError + $"Текущее состояние: число - {this},"
					+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}"),
			}, UnsignedLongReal.Zero, MantissaLength);
		if (e == 0 && Mpir.MpzCmpSi(m, 0) == 0)
			return this;
		else if (Mpir.MpzCmpSi(m, 0) < 0)
			return -(-this).ReciprocInternal();
		else
			return ReciprocInternal();
	}

	private LongReal ReciprocInternal()
	{
		var mantissaOverflow = MpzT.One << MantissaLength;
		var mantissaMask = mantissaOverflow - 1;
		var quotient = (mantissaOverflow << MantissaLength + 1) / (mantissaOverflow + (m >> 1));
		var shiftAmount = quotient.BitLength - MantissaLength - 1;
		quotient = quotient.ShiftRightRound(shiftAmount) & mantissaMask;
		return new(quotient << 1 | (m & 1) ^ 1, Mpir.MpzCmpSi(m, 1) <= 0 ? e - (1u - (m & 1u) * 2u) : e, MantissaLength);
	}

	/// <summary>
	/// Возвращает целое число, ближайшее к данному числу. Если два целых числа одинаково близки к данному
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое является четным.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
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

	/// <summary>
	/// Возвращает целое число, ближайшее к <paramref name="value"/>. Если два целых числа одинаково близки к нему
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое является четным.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public static LongReal Round(LongReal value) => value.Round();

	public static LongReal Round(LongReal x, int digits, MidpointRounding mode)
	{
		var multiplier = ten.Power(digits);
		return (x / multiplier).RoundFunction(mode)() * multiplier;
	}

	/// <summary>
	/// Возвращает целое число, ближайшее к данному числу. Если два целых числа одинаково близки к данному
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое дальше от нуля.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
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
		MidpointRounding.ToEven => Round,
		MidpointRounding.AwayFromZero => RoundAwayFromZero,
		MidpointRounding.ToZero => Truncate,
		MidpointRounding.ToNegativeInfinity => Floor,
		MidpointRounding.ToPositiveInfinity => Ceiling,
		_ => Truncate,
	};

	/// <summary>
	/// Вычисляет гиперболический синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус данного числа.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Sinh()">Sinh()</see>.</remarks>
	public LongReal Sh() => Sinh();

	/// <summary>
	/// Вычисляет гиперболический синус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус <paramref name="value"/>.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Sinh(LongReal)">Sinh()</see>.</remarks>
	public static LongReal Sh(LongReal value) => value.Sinh();

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
		shiftAmount < 0 ? this >> new UnsignedLongReal(-shiftAmount, MantissaLength)
		: this << new UnsignedLongReal(shiftAmount, MantissaLength);

	private static MpzT ShiftUniversal(MpzT x, int shiftAmount) => shiftAmount switch
	{
		> 0 => x << shiftAmount,
		< 0 => x.ShiftRightRound(-shiftAmount),
		_ => x,
	};

	/// <summary>
	/// Вычисляет синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - синус данного числа.
	/// </returns>
	public LongReal Sin()
	{
		if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return new(ShiftedMantissaOverflow + (int)(m - ShiftedMantissaOverflow) switch
			{
				1 => 1,
				_ => 4,
			}, UnsignedLongReal.Zero, MantissaLength);
		var abs = Abs();
		if (abs >= Tau << MantissaLength)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		var divisor = Tau.GetWithOtherML(MantissaLength * 2);
		var localValue = this - Floor(GetWithOtherML(MantissaLength * 2) / divisor) * divisor;
		if (Mpir.MpzCmp(localValue.m, localValue.ZeroMantissa) == 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		var oldDivisor = divisor;
		divisor >>= 1;
		(var sign, localValue) = localValue >= divisor ? (-1, oldDivisor - localValue) : (1, localValue);
		if (localValue == divisor)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		oldDivisor = divisor;
		if (localValue >= (divisor >>= 1))
			localValue = oldDivisor - localValue;
		if (Abs(localValue - divisor) <= new LongReal(MpzT.One, MantissaLength - 2, MantissaLength))
			return new(sign - 1, 0, MantissaLength);
		oldDivisor = divisor;
		divisor >>= 1;
		(var reverse, localValue) = (localValue >= divisor) ? (true, oldDivisor - localValue) : (false, localValue);
		var cos = localValue.CosInternal();
		return ((reverse ? cos : AddInternal(One, -cos.ReciprocInternal().SquareInternal().ReciprocInternal(),
			MantissaLength * 2).ReciprocInternal().SqrtInternal().ReciprocInternal()) * sign)
			.GetWithOtherML(MantissaLength);
	}

	/// <summary>
	/// Вычисляет синус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - синус <paramref name="value"/>.
	/// </returns>
	public static LongReal Sin(LongReal value) => value.Sin();

	/// <summary>
	/// Вычисляет гиперболический синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус данного числа.
	/// </returns>
	public LongReal Sinh()
	{
		var exp = Exp();
		return exp - exp.Reciproc() >> 1;
	}

	/// <summary>
	/// Вычисляет гиперболический синус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус <paramref name="value"/>.
	/// </returns>
	public static LongReal Sinh(LongReal value) => value.Cosh();

	/// <summary>
	/// Вычисляет квадратный корень данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - арифметический квадратный корень данного числа.
	/// </returns>
	public LongReal Sqrt()
	{
		if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0)
			return new(ShiftedMantissaOverflow + 2, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0 || Mpir.MpzCmp(m, NaNMantissa) == 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) < 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmpSi(m, 0) == 0 && e == 0)
			return new(MpzT.Zero, UnsignedLongReal.Zero, MantissaLength);
		else if ((m & 1) != 0)
			return ReciprocInternal().SqrtInternal().ReciprocInternal();
		else
			return SqrtInternal();
	}

	/// <summary>
	/// Вычисляет квадратный корень указанного числа.
	/// </summary>
	/// <param name="value">Число для извлечения квадратного корня.</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - арифметический квадратный корень <paramref name="value"/>.
	/// </returns>
	public static LongReal Sqrt(LongReal value) => value.Sqrt();

	private LongReal SqrtInternal()
	{
		LongReal current = new(MpzT.Zero, e + 1u >> 1, MantissaLength), prev;
		do
		{
			prev = current;
			current = AddInternal(current, DivideInternal(this, current, MantissaLength), MantissaLength) >> 1;
		} while (current.e != prev.e || Mpir.MpzCmpabsUi((current.m >> 1) - (prev.m >> 1), 1) > 0);
		return current;
	}

	/// <summary>
	/// Вычисляет квадрат данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - квадрат данного числа.
	/// </returns>
	public LongReal Square()
	{
		if (Mpir.MpzCmp(m, NaNMantissa) == 0)
			return new(ShiftedMantissaOverflow + 4, UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, ZeroMantissa) == 0)
			return new(ShiftedMantissaOverflow
				+ ((Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0)
				? 4 : 1), UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, NegativeInfinityMantissa) == 0)
			return new(ShiftedMantissaOverflow + (Mpir.MpzCmpSi(m, 0) < 0 ? 2 : 3),
				UnsignedLongReal.Zero, MantissaLength);
		else if (Mpir.MpzCmp(m, PositiveInfinityMantissa) == 0)
			return new(ShiftedMantissaOverflow + (Mpir.MpzCmpSi(m, 0) < 0 ? 3 : 2),
				UnsignedLongReal.Zero, MantissaLength);
		if (Mpir.MpzCmpSi(m, 0) == 0 && e == 0)
			return this;
		var shiftAmount = (m & 1u) != 0u ? e + 1u : new(MpuT.Zero, null, MantissaLength);
		var x = Abs() << shiftAmount;
		return x.SquareInternal() >> (shiftAmount << 1);
	}

	/// <summary>
	/// Вычисляет квадрат указанного числа.
	/// </summary>
	/// <param name="value">Число для извлечения квадратного корня.</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - квадрат <paramref name="value"/>.
	/// </returns>
	public static LongReal Square(LongReal value) => value.Square();

	private LongReal SquareInternal()
	{
		var mantissaOverflow = MpzT.One << MantissaLength;
		var mantissaMask = mantissaOverflow - 1;
		var product = (mantissaOverflow + (m >> 1)).Square();
		var shiftAmount = product.BitLength - MantissaLength - 1;
		var shifted = product.ShiftRightRound(shiftAmount);
		if (Mpir.MpzCmp(shifted, mantissaOverflow << 1) == 0)
			shiftAmount++;
		return new((shifted & mantissaMask) << 1, (e << 1) + (new UnsignedLongReal(shiftAmount) - (uint)MantissaLength),
			MantissaLength);
	}

	/// <summary>
	/// Вычисляет тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс данного числа.
	/// </returns>
	public LongReal Tan()
	{
		var cos = Cos();
		var tau = Tau.GetWithOtherML(MantissaLength);
		var result = Sqrt(1 - cos.Square()) / cos;
		if (this - Floor(this / tau) * tau >= tau >> 1)
			result = -result;
		return result;
	}

	/// <summary>
	/// Вычисляет тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс <paramref name="value"/>.
	/// </returns>
	public static LongReal Tan(LongReal value) => value.Tan();

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	public LongReal Tanh()
	{
		var exp = (this << 1).Exp();
		return (exp - 1) / (exp + 1);
	}

	/// <summary>
	/// Вычисляет гиперболический тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс <paramref name="value"/>.
	/// </returns>
	public static LongReal Tanh(LongReal value) => value.Tanh();

	/// <summary>
	/// Вычисляет тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс данного числа.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Tan()">Tan()</see>.</remarks>
	public LongReal Tg() => Tan();

	/// <summary>
	/// Вычисляет тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше <see cref="Pi"/> &lt;&lt; <see cref="MantissaLength"/> + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс <paramref name="value"/>.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Tan(LongReal)">Tan()</see>.</remarks>
	public static LongReal Tg(LongReal value) => value.Tan();

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Tanh()">Tanh()</see>.</remarks>
	public LongReal Tgh() => Tanh();

	/// <summary>
	/// Вычисляет гиперболический тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс <paramref name="value"/>.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Tanh(LongReal)">Tanh()</see>.</remarks>
	public static LongReal Tgh(LongReal value) => value.Tanh();

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Tanh()">Tanh()</see>.</remarks>
	public LongReal Th() => Tanh();

	/// <summary>
	/// Вычисляет гиперболический тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс <paramref name="value"/>.
	/// </returns>
	/// <remarks>Данный метод является альтернативным названием для <see cref="Tanh(LongReal)">Tanh()</see>.</remarks>
	public static LongReal Th(LongReal value) => value.Tanh();

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
		if (string.IsNullOrEmpty(format))
			format = "G16";
		return ((LongDecimal)this).ToString(format, formatProvider);
	}

	object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(conversionType);
		if (conversionType == typeof(LongReal))
			return this;
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
			return this;
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
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public LongReal Truncate()
	{
		if (Mpir.MpzCmp(m, ShiftedMantissaOverflow) > 0)
			return this;
		if ((m & 1) != 0)
			return new(ShiftedMantissaOverflow + 1, UnsignedLongReal.Zero, MantissaLength);
		if (e >= MantissaLength)
			return this;
		var newM = m >> 1;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			newM = ~newM;
		var shiftAmount = MantissaLength - (int)e;
		newM = newM >> shiftAmount << shiftAmount;
		if (Mpir.MpzCmpSi(m, 0) < 0)
			newM = ~newM;
		return new(newM << 1, e, MantissaLength);
	}

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше указанного числа, для положительных,
	/// и наименьшее целое число, которое не меньше указанного числа, для отрицательных (для нуля, если это непонятно, ноль).
	/// Другими словами, возвращает целую часть указанного числа (<paramref name="value"/>), отбрасывая дробную.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	public static LongReal Truncate(LongReal value) => value.Truncate();

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result)
		where TOther : INumberBase<TOther> => TryConvertFromTruncating(value, out result);
	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result)
		where TOther : INumberBase<TOther> => TryConvertFromTruncating(value, out result);

	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out LongReal result)
		where TOther : INumberBase<TOther>
	{
		result = value switch
		{
			byte n => new((int)n),
			short n => new(n),
			ushort n => new((int)n),
			int n => new(n),
			uint n => new(n),
			long n => new(n),
			ulong n => new(n),
			MpzT n => new(n),
			MpuT n => new(n),
			float r => new(r),
			double r => new(r),
			decimal r => (LongReal)(LongDecimal)r,
			UnsignedLongReal r => new(r),
			UnsignedLongDecimal r => (LongReal)(LongDecimal)r,
			LongReal r => r,
			LongDecimal r => (LongReal)r,
			string s => TryParse(s, out var c) ? c : default,
			_ => default,
		};
		return result != default;
	}

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
		TryParse(s, NumberStyles.Float, provider, out result);

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

	/// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out LongReal)"/>
	public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out LongReal result) =>
		TryParse(s, NumberStyles.None, null, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongReal result) => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongReal result) => TryParse(s.AsSpan(), style, provider, out result);
	/// <inheritdoc cref="TryParse(string, IFormatProvider?, out LongReal)"/>
	public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out LongReal result) =>
		TryParse(s.AsSpan(), NumberStyles.None, null, out result);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian"/>
	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteBigEndian(destination, out bytesWritten, true);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian"/>
	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength)
	{
		bytesWritten = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(destination, MantissaLength);
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
		if (!e.TryWriteBigEndian(destination[..^MantissaByteLength], out var bytesWritten2, false))
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

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian"/>
	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteLittleEndian(destination, out bytesWritten, true);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian"/>
	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten, bool saveMantissaLength)
	{
		bytesWritten = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(destination, MantissaLength);
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
		bytesWritten += bytesWritten2;
		return true;
	}

	public bool TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) =>
		m.TryWriteBigEndian(destination, out bytesWritten);
	public bool TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) =>
		m.TryWriteLittleEndian(destination, out bytesWritten);

	public static implicit operator LongReal(byte value) => new((uint)value);
	public static implicit operator LongReal(short value) => new(value, MinMantissaLength);
	public static implicit operator LongReal(ushort value) => new((uint)value, MinMantissaLength);
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
		if (Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) > 0 || (value.m & 1) != 0
			|| value.e >= (uint)value.MantissaLength + sizeof(uint) * 8u)
			return 0u;
		var eAfterCast = (int)value.e;
		if (eAfterCast <= value.MantissaLength)
			return (uint)(value.MantissaOverflow + (value.m >> 1)).ShiftRightRound(value.MantissaLength - eAfterCast);
		else
			return (uint)(value.m << eAfterCast - value.MantissaLength);
	}

	public static explicit operator long(LongReal value) => (long)(ulong)value;

	public static explicit operator ulong(LongReal value)
	{
		if (Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) > 0 || (value.m & 1) != 0
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
		if (Mpir.MpzCmp(value.m, value.ZeroMantissa) == 0)
			return 0d;
		else if (Mpir.MpzCmp(value.m, value.PositiveInfinityMantissa) == 0)
			return double.PositiveInfinity;
		else if (Mpir.MpzCmp(value.m, value.NegativeInfinityMantissa) == 0)
			return double.NegativeInfinity;
		else if (Mpir.MpzCmp(value.m, value.NaNMantissa) == 0)
			return double.NaN;
		var negative = Mpir.MpzCmpSi(value.m, 0) < 0;
		var negativeExponent = (value.m & 1) != 0;
		if (!negativeExponent && value.e >= 1024)
			return negative ? double.NegativeInfinity : double.PositiveInfinity;
		if (negativeExponent && value.e > 1074)
			return 0d;
		var eAfterCast = (int)value.e;
		if (negativeExponent)
			eAfterCast = ~eAfterCast;
		eAfterCast += 1023;
		var exponent = (ulong)Math.Max(eAfterCast, 0);
		var mantissa = value.m >> 1;
		if (negative)
			mantissa = ~mantissa;
		if (exponent == 0)
			mantissa = (value.MantissaOverflow + mantissa).ShiftRightRound(value.MantissaLength - eAfterCast - 51);
		else
			mantissa = mantissa.ShiftRightRound(value.MantissaLength - 52);
		return BitConverter.UInt64BitsToDouble((negative ? 0x8000000000000000 : 0) + (exponent << 52) + (ulong)mantissa);
	}

	public static explicit operator decimal(LongReal value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);

	public static explicit operator string?(LongReal value) => value.ToString();

	public static explicit operator MpzT(LongReal value)
	{
		if (Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) > 0 || (value.m & 1) != 0 || value.e > int.MaxValue)
			return 0;
		var eAfterCast = (int)value.e;
		if (eAfterCast <= value.MantissaLength)
		{
			var mantissa = value.m >> 1;
			if (Mpir.MpzCmpSi(value.m, 0) < 0)
				mantissa = ~mantissa;
			if (Mpir.MpzCmpSi(value.m, 0) >= 0 || Mpir.MpzCmpSi(mantissa, 0) != 0)
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
			return MpuT.Zero;
		var eAfterCast = (int)value.e;
		if (eAfterCast <= value.MantissaLength)
			return (MpuT)(value.MantissaOverflow + (value.m >> 1)).ShiftRightRound(value.MantissaLength - eAfterCast);
		else
			return (MpuT)(value.m << eAfterCast - value.MantissaLength);
	}

	public static explicit operator UnsignedLongReal(LongReal value)
	{
		if (Mpir.MpzCmp(value.m, value.ZeroMantissa) == 0)
			return UnsignedLongReal.Zero;
		else if (Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) > 0)
			throw new OverflowException(ULRConversionError);
		else if (Mpir.MpzCmpSi(value.m, 0) < 0)
			throw new OverflowException(ULRConversionError);
		else if ((value.m & 1) != 0)
			return UnsignedLongReal.Zero;
		else if (value.e < value.MantissaLength)
			return new(Unsafe.As<MpuT>(value.MantissaOverflow + (value.m >> 1))
				.ShiftRightRound(value.MantissaLength - (int)value.e), null, value.MantissaLength);
		else
			return new(Unsafe.As<MpuT>(value.m >> 1), value.e - ((uint)value.MantissaLength - 1u), value.MantissaLength);
	}

	public static LongReal operator +(LongReal value) => new(value);

	public static LongReal operator -(LongReal value)
	{
		if (Mpir.MpzCmp(value.m, value.ShiftedMantissaOverflow) > 0)
		{
			var specialExceed = (int)(value.m - value.ShiftedMantissaOverflow);
			return new(value.ShiftedMantissaOverflow + specialExceed switch
			{
				2 => 3,
				3 => 2,
				_ => specialExceed,
			}, UnsignedLongReal.Zero, value.MantissaLength);
		}
		return new(~(value.m >> 1) << 1 | value.m & 1, value.e, value.MantissaLength);
	}

	/// <inheritdoc cref="IBitwiseOperators{LongReal, LongReal, LongReal}.operator ~(LongReal)"/>
	public static LongReal operator ~(LongReal value) => -(value + One);

	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(byte x, LongReal y) => (uint)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, byte y) => x + (uint)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(short x, LongReal y) => (int)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, short y) => x + (int)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(ushort x, LongReal y) => (uint)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, ushort y) => x + (uint)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(int x, LongReal y) => (LongReal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, int y) => x + (LongReal)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(uint x, LongReal y) => (LongReal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, uint y) => x + (LongReal)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(long x, LongReal y) => (LongReal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, long y) => x + (LongReal)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(ulong x, LongReal y) => (LongReal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, ulong y) => x + (LongReal)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(MpzT x, LongReal y) => (LongReal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, MpzT y) => x + (LongReal)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(MpuT x, LongReal y) => (LongReal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongReal operator +(LongReal x, MpuT y) => x + (LongReal)y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongDecimal operator +(decimal x, LongReal y) => (LongDecimal)y + x;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongDecimal operator +(LongReal x, decimal y) => (LongDecimal)x + y;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongDecimal operator +(UnsignedLongDecimal x, LongReal y) => (LongDecimal)y + x;
	/// <inheritdoc cref="operator +(LongReal, LongReal)"/>
	public static LongDecimal operator +(LongReal x, UnsignedLongDecimal y) => (LongDecimal)x + y;

	public static LongReal operator +(LongReal x, LongReal y)
	{
		var mantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (Mpir.MpzCmp(x.m, x.NaNMantissa) == 0 || Mpir.MpzCmp(y.m, y.NaNMantissa) == 0)
			return new((MpzT.One << mantissaLength + 1) + 4, UnsignedLongReal.Zero, mantissaLength);
		else if (Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << mantissaLength + 1) + (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0 ? 4 : 3),
				UnsignedLongReal.Zero, mantissaLength);
		else if (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << mantissaLength + 1) + (Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0 ? 4 : 2),
				UnsignedLongReal.Zero, mantissaLength);
		else if (Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << mantissaLength + 1) + 3, UnsignedLongReal.Zero, mantissaLength);
		else if (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << mantissaLength + 1) + 2, UnsignedLongReal.Zero, mantissaLength);
		else if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0)
			return y.GetWithOtherML(mantissaLength);
		else if (Mpir.MpzCmp(y.m, y.ZeroMantissa) == 0)
			return x.GetWithOtherML(mantissaLength);
		else if (Mpir.MpzCmp(x.m, y.m) == 0 && x.e == y.e)
			return x << 1;
		else if (Mpir.MpzCmp(x.m >> 1, ~y.m >> 1) == 0 && (x.m & 1) == (y.m & 1) && x.e == y.e)
			return new((MpzT.One << mantissaLength + 1) + 1, UnsignedLongReal.Zero, mantissaLength);
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

	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(byte x, LongReal y) => (uint)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, byte y) => x - (uint)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(short x, LongReal y) => (int)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, short y) => x - (int)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(ushort x, LongReal y) => (uint)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, ushort y) => x - (uint)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(int x, LongReal y) => (LongReal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, int y) => x - (LongReal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(uint x, LongReal y) => (LongReal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, uint y) => x - (LongReal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(long x, LongReal y) => (LongReal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, long y) => x - (LongReal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(ulong x, LongReal y) => (LongReal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, ulong y) => x - (LongReal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(MpzT x, LongReal y) => (LongReal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, MpzT y) => x - (LongReal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(MpuT x, LongReal y) => (LongReal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongReal operator -(LongReal x, MpuT y) => x - (LongReal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongDecimal operator -(decimal x, LongReal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongDecimal operator -(LongReal x, decimal y) => (LongDecimal)x - y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongDecimal operator -(UnsignedLongDecimal x, LongReal y) => x - (LongDecimal)y;
	/// <inheritdoc cref="operator -(LongReal, LongReal)"/>
	public static LongDecimal operator -(LongReal x, UnsignedLongDecimal y) => (LongDecimal)x - y;
	public static LongReal operator -(LongReal x, LongReal y) => x + -y;

	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(byte x, LongReal y) => (uint)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, byte y) => x * (uint)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(short x, LongReal y) => (int)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, short y) => x * (int)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(ushort x, LongReal y) => (uint)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, ushort y) => x * (uint)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(int x, LongReal y) => y * x;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(uint x, LongReal y) => y * x;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(long x, LongReal y) => (LongReal)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, long y) => x * (LongReal)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(ulong x, LongReal y) => (LongReal)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, ulong y) => x * (LongReal)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(MpzT x, LongReal y) => (LongReal)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, MpzT y) => x * (LongReal)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(MpuT x, LongReal y) => (LongReal)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongReal operator *(LongReal x, MpuT y) => x * (LongReal)y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongDecimal operator *(decimal x, LongReal y) => (LongDecimal)y * x;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongDecimal operator *(LongReal x, decimal y) => (LongDecimal)x * y;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongDecimal operator *(UnsignedLongDecimal x, LongReal y) => (LongDecimal)y * x;
	/// <inheritdoc cref="operator *(LongReal, LongReal)"/>
	public static LongDecimal operator *(LongReal x, UnsignedLongDecimal y) => (LongDecimal)x * y;

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
			return new((MpzT.One << mantissaLength + 1)
				+ (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) <= 0 || Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0
				? 1 : 4), UnsignedLongReal.Zero, mantissaLength);
		else if (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) > 0 || y == 1)
			return x;
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
		if (Mpir.MpzCmp(x.m, x.NaNMantissa) == 0 || Mpir.MpzCmp(y.m, y.NaNMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0 || Mpir.MpzCmp(y.m, y.ZeroMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1)
				+ (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0
				|| Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0
				? 4 : 1), UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (y < 0 ? 2 : 3), UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (y < 0 ? 3 : 2), UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(x.m, 0) < 0 ? 2 : 3),
				UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (Mpir.MpzCmpSi(x.m, 0) < 0 ? 3 : 2),
				UnsignedLongReal.Zero, maxMantissaLength);
		x = x.GetWithOtherML(maxMantissaLength);
		y = y.GetWithOtherML(maxMantissaLength);
		if (Mpir.MpzCmpSi(x.m, 0) == 0 && x.e == 0)
			return y;
		else if (Mpir.MpzCmpSi(y.m, 0) == 0 && y.e == 0)
			return x;
		var xShiftAmount = (x.m & 1u) != 0u ? x.e + 1u : new(MpuT.Zero, null, maxMantissaLength);
		var yShiftAmount = (y.m & 1u) != 0u ? y.e + 1u : new(MpuT.Zero, null, maxMantissaLength);
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
	public static LongReal operator /(byte x, LongReal y) => (uint)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, byte y) => x / (uint)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(short x, LongReal y) => (int)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, short y) => x / (int)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(ushort x, LongReal y) => (uint)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, ushort y) => x / (uint)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(int x, LongReal y) => (LongReal)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(uint x, LongReal y) => (LongReal)x / y;

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
		{
			if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0 || Mpir.MpzCmp(x.m, x.NaNMantissa) == 0)
				return new((MpzT.One << mantissaLength + 1) + 4, UnsignedLongReal.Zero, mantissaLength);
			else
				return new((MpzT.One << mantissaLength + 1) + (x < 0 ? 3 : 2), UnsignedLongReal.Zero, mantissaLength);
		}
		else if (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) > 0 || y == 1)
			return x;
		else if ((y & y - 1) == 0)
			return x >> (int)uint.TrailingZeroCount(y);
		else if (Mpir.MpzCmpSi(x.m, 0) < 0)
			return -DivideUiInternal(-x, y, mantissaLength);
		else
			return DivideUiInternal(x, y, mantissaLength);
	}

	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(long x, LongReal y) => (LongReal)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, long y) => x / (LongReal)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(ulong x, LongReal y) => (LongReal)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, ulong y) => x / (LongReal)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(MpzT x, LongReal y) => (LongReal)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, MpzT y) => x / (LongReal)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(MpuT x, LongReal y) => (LongReal)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongReal operator /(LongReal x, MpuT y) => x / (LongReal)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongDecimal operator /(decimal x, LongReal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongDecimal operator /(LongReal x, decimal y) => (LongDecimal)x / y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongDecimal operator /(UnsignedLongDecimal x, LongReal y) => x / (LongDecimal)y;
	/// <inheritdoc cref="operator /(LongReal, LongReal)"/>
	public static LongDecimal operator /(LongReal x, UnsignedLongDecimal y) => (LongDecimal)x / y;

	public static LongReal operator /(LongReal x, LongReal y)
	{
		var maxMantissaLength = Math.Max(x.MantissaLength, y.MantissaLength);
		if (Mpir.MpzCmp(x.m, x.NaNMantissa) == 0 || Mpir.MpzCmp(y.m, y.NaNMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(y.m, y.ZeroMantissa) == 0)
		{
			if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0 || Mpir.MpzCmp(x.m, x.NaNMantissa) == 0)
				return new((MpzT.One << maxMantissaLength + 1) + 4, UnsignedLongReal.Zero, maxMantissaLength);
			else
				return new((MpzT.One << maxMantissaLength + 1) + (x < 0 ? 3 : 2), UnsignedLongReal.Zero, maxMantissaLength);
		}
		else if (Mpir.MpzCmp(y.m, y.ShiftedMantissaOverflow) > 0)
			return new((MpzT.One << maxMantissaLength + 1)
				+ ((Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0 || Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
				? 4 : 1), UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.ZeroMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + 1, UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.NegativeInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (y < 0 ? 2 : 3), UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmp(x.m, x.PositiveInfinityMantissa) == 0)
			return new((MpzT.One << maxMantissaLength + 1) + (y < 0 ? 3 : 2), UnsignedLongReal.Zero, maxMantissaLength);
		else if (Mpir.MpzCmpSi(y.m, 0) == 0 && y.e == 0)
			return x;
		else if (Mpir.MpzCmpSi(y.m, -2) == 0 && y.e == 0)
			return -x;
		x = x.GetWithOtherML(maxMantissaLength);
		y = y.GetWithOtherML(maxMantissaLength);
		UnsignedLongReal shiftAmount = new(MpuT.Zero, null, maxMantissaLength);
		if ((x.m & 1) != 0)
			shiftAmount = x.e + 1u;
		if ((y.m & 1) != 0 && y.e >= shiftAmount)
			shiftAmount = y.e + 1u;
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

	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(byte x, LongReal y) => (uint)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, byte y) => x % (uint)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(short x, LongReal y) => (int)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, short y) => x % (int)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(ushort x, LongReal y) => (uint)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, ushort y) => x % (uint)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(int x, LongReal y) => (LongReal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, int y) => x % (LongReal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(uint x, LongReal y) => (LongReal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, uint y) => x % (LongReal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(long x, LongReal y) => (LongReal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, long y) => x % (LongReal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(ulong x, LongReal y) => (LongReal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, ulong y) => x % (LongReal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(MpzT x, LongReal y) => (LongReal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, MpzT y) => x % (LongReal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(MpuT x, LongReal y) => (LongReal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongReal operator %(LongReal x, MpuT y) => x % (LongReal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongDecimal operator %(decimal x, LongReal y) => x % (LongDecimal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongDecimal operator %(LongReal x, decimal y) => (LongDecimal)x % y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongDecimal operator %(UnsignedLongDecimal x, LongReal y) => x % (LongDecimal)y;
	/// <inheritdoc cref="operator %(LongReal, LongReal)"/>
	public static LongDecimal operator %(LongReal x, UnsignedLongDecimal y) => (LongDecimal)x % y;

	public static LongReal operator %(LongReal x, LongReal y) => x - Truncate(x / y) * y;

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongReal operator <<(LongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) > 0 || shiftAmount == 0)
			return x;
		else if ((x.m & 1) == 0)
			return new(x.m, x.e + new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else
			return new(x.m & new MpzT(-2), new UnsignedLongReal(shiftAmount) - x.e - 1u, x.MantissaLength);
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongReal operator <<(LongReal x, UnsignedLongReal shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) > 0 || shiftAmount == 0)
			return x;
		else if ((x.m & 1) == 0)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new(x.m & new MpzT(-2), shiftAmount - x.e - 1u, x.MantissaLength);
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongReal operator >>(LongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) > 0 || shiftAmount == 0)
			return x;
		else if ((x.m & 1) != 0)
			return new(x.m, x.e + new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - new UnsignedLongReal(shiftAmount), x.MantissaLength);
		else
			return new(x.m | 1, new UnsignedLongReal(shiftAmount) - x.e - 1u, x.MantissaLength);
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongReal operator >>(LongReal x, UnsignedLongReal shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (Mpir.MpzCmp(x.m, x.ShiftedMantissaOverflow) > 0 || shiftAmount == 0)
			return x;
		else if ((x.m & 1) != 0)
			return new(x.m, x.e + shiftAmount, x.MantissaLength);
		else if (x.e >= shiftAmount)
			return new(x.m, x.e - shiftAmount, x.MantissaLength);
		else
			return new(x.m | 1, shiftAmount - x.e - 1u, x.MantissaLength);
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
