namespace NStar.Mpir;

/// <summary>Represents an arbitrarily large unsigned integer.</summary>
public sealed class MpuT : IBinaryInteger<MpuT>, ICloneable, IConvertible, IDisposable
{
	private static readonly byte[] convertToLongBytes = GC.AllocateUninitializedArray<byte>(8);
	private static byte[] exportBytes = GC.AllocateUninitializedArray<byte>(1024);
	private static readonly Lock lockObj = new();
	private static readonly MpuT five = new(5);
	private static readonly MpuT ten = new(10);
	private static readonly Dictionary<int, MpuT> PowersOfFive = [];
	private static readonly Dictionary<int, MpuT> PowersOfTen = [];
	private const string InternalError = "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
		+ "2. Нарушение целостности структуры списка (ошибка в логике -"
		+ " список все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
		+ "3. Системная ошибка (память, диск и т. д.).\r\n";

	internal nint val;

	/// <summary>Initializes a new MpuT to 0.</summary>
	public MpuT() => val = Mpir.MpuInit();
	/// <summary>Initializes a new MpuT to the same value as op.</summary>
	public MpuT(MpuT op) => val = Mpir.MpuInitSet(op.val == 0 ? 0 : op);
	/// <summary>Initializes a new MpuT to the unsigned int op.</summary>
	public MpuT(uint op) => val = Mpir.MpuInitSetUi(op);
	/// <summary>Initializes a new MpuT to the int op.</summary>
	public MpuT(int op)
	{
		if (op < 0)
			throw new ArgumentException(NoNegativeNumbers, nameof(op));
		val = Mpir.MpuInitSetSi(op);
	}

	/// <summary>Initializes a new MpuT to the double op.</summary>
	public MpuT(double op)
	{
		if (op < 0)
			throw new ArgumentException(NoNegativeNumbers, nameof(op));
		if (op is double.PositiveInfinity or double.NaN)
			op = 0;
		val = Mpir.MpuInitSetD(op);
	}

	/// <summary>Initializes a new MpuT to string s, parsed as an integer in the specified base.</summary>
	public MpuT(string? s, uint @base)
	{
		var s2 = s ?? "0";
		if (s2.Contains('-'))
			throw new ArgumentException(NoNegativeNumbers, nameof(s));
		val = Mpir.MpuInitSetStr(s2, @base);
	}

	/// <summary>Initializes a new MpuT to string s, parsed as an integer in base 10.</summary>
	public MpuT(string? s) : this(s, DefaultStringBase) { }
	/// <summary>Initializes a new MpuT to the BigInteger op.</summary>
	public MpuT(BigInteger op) : this(op < 0
		? throw new ArgumentException(NoNegativeNumbers, nameof(op)) : op.ToByteArray(), -1) { }
	public MpuT(MpzT op) : this(Mpir.MpzCmpSi(op, 0) < 0
		? throw new ArgumentException(NoNegativeNumbers, nameof(op)) : op.ToByteArray(-1), -1) { }

	/// <summary>Initializes a new MpuT to using MPIR MpuInit2. Only use if you need to avoid reallocations.</summary>
	//
	// Initialization with MpuInit2 should not be confused with MpuT construction
	// from a ulong. Thus, so we use a static construction function instead, and add
	// the dummy type init2Type to enable us to write a ctor with a unique signature.
	public static MpuT Init2(ulong n) => new(Init2Type.init2, n);
	private enum Init2Type { init2 }
	private MpuT(Init2Type _, ulong n) => val = Mpir.MpuInit2(n);

	/// <summary>Initializes a new MpuT to the long op.</summary>
	public MpuT(long op)
	{
		if (op < 0)
			throw new ArgumentException(NoNegativeNumbers, nameof(op));
		val = Mpir.MpuInitSetSi(unchecked((int)(op >> BitsPerInt)));
		Mpir.MpuMul2exp(this, this, BitsPerInt);
		Mpir.MpuAddUi(this, this, unchecked((uint)op));
	}

	/// <summary>Initializes a new MpuT to the unsigned long op.</summary>
	public MpuT(ulong op)
	{
		val = Mpir.MpuInitSetUi(unchecked((uint)(op >> BitsPerInt)));
		Mpir.MpuMul2exp(this, this, BitsPerInt);
		Mpir.MpuAddUi(this, this, unchecked((uint)op));
	}

	public MpuT(decimal op) : this(op < 0
		? throw new ArgumentException(NoNegativeNumbers, nameof(op)) : new BigInteger(op)) { }

	/// <summary>
	/// Initializes a new MpuT to the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1 for little endian.
	/// </summary>
	public MpuT(ReadOnlySpan<byte> bytes, int order) : this() => FromByteArray(bytes, order);

	~MpuT() => Dispose(false);

	public int this[int bitIndex] => Mpir.MpuTstbit(this, (uint)bitIndex);
	public static MpuT AdditiveIdentity => Zero;
	public int BitLength => val == 0 ? 0 : (int)Mpir.MpuSizeinbase(this, 2);
	public int DecLength => ToString()?.Length ?? 1;
	public static MpuT One { get; } = new(1);
	public static MpuT MultiplicativeIdentity => One;
	public static int Radix => 2;

	public int Sign
	{
		get
		{
			if (IsPositive(this))
				return 1;
			else if (IsZero(this))
				return 0;
			return IsNegative(this) ? -1 : throw new ArithmeticException("Произошла ошибка при  вычислении знака.");
		}
	}

	public static MpuT Zero { get; } = new(0);

	/// <summary>Returns a new MpuT which is the absolute value of this value.</summary>
	public MpuT Abs()
	{
		var result = new MpuT();
		Mpir.MpuAbs(result, this);
		return result;
	}

	public static MpuT Abs(MpuT value) => value.Abs();
	public MpuT Add(int x) => this + x;
	public MpuT Add(MpuT x) => this + x;
	public MpuT Add(uint x) => this + x;
	public MpuT And(MpuT x) => this & x;

	public static MpuT Binomial(int n, int k)
	{
		if (n < 0)
			throw new OverflowException(NoNegativeNumbers);
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpuT();
		Mpir.MpuBinUiui(z, (uint)n, (uint)k);
		return z;
	}

	public static MpuT Binomial(MpuT n, int k)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpuT();
		Mpir.MpuBinUi(z, n, (uint)k);
		return z;
	}

	public static MpuT Binomial(MpuT n, uint k)
	{
		var z = new MpuT();
		Mpir.MpuBinUi(z, n, k);
		return z;
	}

	public static MpuT Binomial(uint n, uint k)
	{
		var z = new MpuT();
		Mpir.MpuBinUiui(z, n, k);
		return z;
	}

	public MpuT Clone() => new(this);
	object ICloneable.Clone() => Clone();

	public MpuT ChangeBit(int bitIndex, int value)
	{
		var z = new MpuT(this);
		if (value == 0)
			Mpir.MpuClrbit(z, (uint)bitIndex);
		else
			Mpir.MpuSetbit(z, (uint)bitIndex);
		return z;
	}

	public static int Compare(int x, MpuT y) => -y.CompareTo(x);
	public static int Compare(MpuT x, int y) => x.CompareTo(y);
	public static int Compare(uint x, MpuT y) => -y.CompareTo(x);
	public static int Compare(MpuT x, uint y) => x.CompareTo(y);
	public static int Compare(long x, MpuT y) => -y.CompareTo(x);
	public static int Compare(MpuT x, long y) => x.CompareTo(y);
	public static int Compare(ulong x, MpuT y) => -y.CompareTo(x);
	public static int Compare(MpuT x, ulong y) => x.CompareTo(y);
	public static int Compare(double x, MpuT y) => -y.CompareTo(x);
	public static int Compare(MpuT x, double y) => x.CompareTo(y);
	public static int Compare(decimal x, MpuT y) => -y.CompareTo(x);
	public static int Compare(MpuT x, decimal y) => x.CompareTo(y);
	public static int Compare(MpuT x, MpuT? y) => x.CompareTo(y);
	public static int Compare(MpuT x, MpzT? y) => x.CompareTo(y);
	public static int Compare(MpuT x, object? y) => x.CompareTo(y);
	public static int Compare(object x, MpuT y) => -y.CompareTo(x);
	public static int CompareAbs(MpuT x, MpuT y) => x.CompareAbsTo(y);
	public static int CompareAbs(MpuT x, object y) => x.CompareAbsTo(y);
	public static int CompareAbs(object x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(int x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpuT x, int y) => x.CompareAbsTo(y);
	public static int CompareAbs(uint x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpuT x, uint y) => x.CompareAbsTo(y);
	public static int CompareAbs(long x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpuT x, long y) => x.CompareAbsTo(y);
	public static int CompareAbs(ulong x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpuT x, ulong y) => x.CompareAbsTo(y);
	public static int CompareAbs(double x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpuT x, double y) => x.CompareAbsTo(y);
	public static int CompareAbs(decimal x, MpuT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpuT x, decimal y) => x.CompareAbsTo(y);
	public int CompareAbsTo(int other) => Mpir.MpuCmpabsUi(this, (uint)other);
	public int CompareAbsTo(uint other) => Mpir.MpuCmpabsUi(this, other);
	public int CompareAbsTo(long other) => CompareAbsTo((MpuT)other);
	public int CompareAbsTo(ulong other) => CompareAbsTo((MpuT)other);
	public int CompareAbsTo(double other) => Mpir.MpuCmpabsD(this, other);
	public int CompareAbsTo(decimal other) => Mpir.MpuCmpabsD(this, (double)other);
	public int CompareAbsTo(MpuT other) => Mpir.MpuCmpabs(this, other);

	public int CompareAbsTo(object obj) => obj switch
	{
		MpuT uz => CompareAbsTo(uz),
		MpzT z => CompareAbsTo(z),
		int i => CompareAbsTo(i),
		uint ui => CompareAbsTo(ui),
		long li => CompareAbsTo(li),
		ulong uli => CompareAbsTo(uli),
		double d => CompareAbsTo(d),
		float f => CompareAbsTo(f),
		short si => CompareAbsTo(si),
		ushort usi => CompareAbsTo(usi),
		byte y => CompareAbsTo(y),
		sbyte sy => CompareAbsTo(sy),
		decimal m => CompareAbsTo(m),
		string s => CompareAbsTo(new MpuT(s)),
		_ => throw new ArgumentException("Cannot compare to " + obj.GetType()),
	};

	public int CompareTo(int other) => Mpir.MpuCmpSi(this, other);
	public int CompareTo(uint other) => Mpir.MpuCmpUi(this, other);

	// TODO: Optimize by accessing the memory directly
	public int CompareTo(long other)
	{
		var otherMpu = new MpuT(other);
		var ret = CompareTo(otherMpu);
		return ret;
	}

	// TODO: Optimize by accessing the memory directly
	public int CompareTo(ulong other)
	{
		var otherMpu = new MpuT(other);
		var ret = CompareTo(otherMpu);
		return ret;
	}

	public int CompareTo(float other) => Mpir.MpuCmpD(this, (double)other);
	public int CompareTo(double other) => Mpir.MpuCmpD(this, other);
	public int CompareTo(decimal other) => Mpir.MpuCmpD(this, (double)other);
	public int CompareTo(MpzT? other) => Mpir.MpzCmp(Unsafe.As<MpzT>(this), other);
	public int CompareTo(MpuT? other) => Mpir.MpuCmp(this, other);

	public int CompareTo(object? obj) => obj switch
	{
		MpuT uz => CompareTo(uz),
		MpzT z => -z.CompareTo(this),
		int i => CompareTo(i),
		uint ui => CompareTo(ui),
		long li => CompareTo(li),
		ulong uli => CompareTo(uli),
		double d => CompareTo(d),
		float f => CompareTo(f),
		short si => CompareTo(si),
		ushort usi => CompareTo(usi),
		byte y => CompareTo(y),
		sbyte sy => CompareTo(sy),
		string s => CompareTo(new MpuT(s)),
		IComparable ic => -ic.CompareTo(this),
		_ => throw new ArgumentException("Cannot compare to " + (obj?.GetType()?.ToString() ?? "null"))
	};

	public MpzT Complement() => ~this;
	public int CountOnes() => (int)Mpir.MpuPopcount(this);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public void Dispose(bool disposing)
	{
		if (val == 0 || ReferenceEquals(this, Zero) || ReferenceEquals(this, One)
			|| IsInPowersOfFiveDictionary() || IsInPowersOfTenDictionary())
			return;
		try
		{
			Mpir.MpuClear(this);
		}
		catch (Exception) when (!disposing)
		{
		}
		val = 0;
	}
	public MpuT Divide(int x) => this / x;

	public MpuT Divide(int x, out int remainder)
	{
		if (x < 0)
			throw new OverflowException(NoNegativeNumbers);
		var quotient = new MpuT();
		remainder = (int)Mpir.MpuTdivQUi(quotient, this, (uint)x);
		return quotient;
	}

	public MpuT Divide(MpuT x) => this / x;

	public MpuT Divide(MpuT x, out MpuT remainder)
	{
		var quotient = new MpuT();
		remainder = new MpuT();
		Mpir.MpuTdivQr(quotient, remainder, this, x);
		return quotient;
	}

	public MpuT Divide(uint x) => this / x;

	public MpuT Divide(uint x, out uint remainder)
	{
		var quotient = new MpuT();
		remainder = Mpir.MpuTdivQUi(quotient, this, x);
		return quotient;
	}

	public MpuT DivideExactly(int x)
	{
		if (x < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuDivexactUi(z, this, (uint)x);
		return z;
	}

	/// <summary>
	/// Divides exactly. Only works when the division is gauranteed to be exact (there is no remainder).
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public MpuT DivideExactly(MpuT x)
	{
		var z = new MpuT();
		Mpir.MpuDivexact(z, this, x);
		return z;
	}

	public MpuT DivideExactly(uint x)
	{
		var z = new MpuT();
		Mpir.MpuDivexactUi(z, this, x);
		return z;
	}

	public MpuT DivideMod(MpuT x, MpuT mod) => this * x.InvertMod(mod) % mod;

	public bool Equals(int other) => CompareTo(other) == 0;

	public bool Equals(uint other) => CompareTo(other) == 0;

	public bool Equals(long other) => CompareTo(other) == 0;

	public bool Equals(ulong other) => CompareTo(other) == 0;

	public bool Equals(double other) => CompareTo(other) == 0;

	public bool Equals(decimal other) => CompareTo(other) == 0;

	public bool Equals(MpzT? other) => Compare(this, other) == 0;

	public bool Equals(MpuT? other) => Compare(this, other) == 0;

	public override bool Equals(object? obj) => obj switch
	{
		null => false,
		MpuT uz => CompareTo(uz) == 0,
		MpzT z => CompareTo(z) == 0,
		int i => this == i,
		uint ui => this == ui,
		long li => this == li,
		ulong uli => this == uli,
		double d => this == d,
		float f => this == f,
		short si => this == si,
		ushort usi => this == usi,
		byte y => this == y,
		sbyte sy => this == sy,
		IConvertible ic => ic.Equals(this),
		_ => false
	};

	public bool EqualsMod(int x, int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		if (x >= 0)
			return Mpir.MpuCongruentUiP(this, (uint)x, (uint)mod) != 0;
		else
		{
			var xAsUint = (uint)(x % mod + mod);
			return Mpir.MpuCongruentUiP(this, xAsUint, (uint)mod) != 0;
		}
	}

	public bool EqualsMod(MpuT x, MpuT mod) => Mpir.MpuCongruentP(this, x, mod) != 0;

	public bool EqualsMod(uint x, uint mod) => Mpir.MpuCongruentUiP(this, x, mod) != 0;

	public static MpuT Factorial(int x)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(x);
		var z = new MpuT();
		Mpir.MpuFacUi(z, (uint)x);
		return z;
	}

	public static MpuT Factorial(uint x)
	{
		var z = new MpuT();
		Mpir.MpuFacUi(z, x);
		return z;
	}

	public static MpuT Fibonacci(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		Mpir.MpuFibUi(z, (uint)n);
		return z;
	}

	public static MpuT Fibonacci(int n, out MpuT previous)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		previous = new MpuT();
		Mpir.MpuFib2Ui(z, previous, (uint)n);
		return z;
	}

	public static MpuT Fibonacci(uint n)
	{
		var z = new MpuT();
		Mpir.MpuFibUi(z, n);
		return z;
	}

	public static MpuT Fibonacci(uint n, out MpuT previous)
	{
		var z = new MpuT();
		previous = new MpuT();
		Mpir.MpuFib2Ui(z, previous, n);
		return z;
	}

	/// <summary>
	/// Import the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1 for little endian.
	/// </summary>
	public void FromByteArray(ReadOnlySpan<byte> source, int order) =>
		Mpir.MpirMpuImport(this, (uint)source.Length, order, sizeof(byte), 0, 0u, source);

	public static MpuT Gcd(int x, MpuT y)
	{
		if (x < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuGcdUi(z, y, (uint)x);
		return z;
	}

	public static MpuT Gcd(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(y, 0) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuGcdUi(z, x, (uint)y);
		return z;
	}

	public static MpuT Gcd(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuGcd(z, x, y);
		return z;
	}

	public static MpuT Gcd(MpuT x, MpuT y, out MpuT a)
	{
		var z = new MpuT();
		a = new MpuT();
		Mpir.MpuGcdext(z, a, default!, x, y);
		return z;
	}

	public static MpuT Gcd(MpuT x, MpuT y, out MpuT a, out MpuT b)
	{
		var z = new MpuT();
		a = new MpuT();
		b = new MpuT();
		Mpir.MpuGcdext(z, a, b, x, y);
		return z;
	}

	public static MpuT Gcd(MpuT x, uint y)
	{
		var z = new MpuT();
		Mpir.MpuGcdUi(z, x, y);
		return z;
	}

	public static MpuT Gcd(uint x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuGcdUi(z, y, x);
		return z;
	}

	public int GetByteCount() => (BitLength + 7) / 8;
	public MpuT GetFullBitLength() => Mpir.MpuSizeinbase(this, 2);

	public override int GetHashCode()
	{
		uint hash = 0;
		Span<byte> bytes;
		lock (lockObj)
			bytes = ProcessToByteArray(this, false);
		var len = bytes.Length; // Make sure it's only evaluated once.
		var shift = 0;
		for (var i = 0; i < len; i++)
		{
			hash ^= (uint)bytes[i] << shift;
			shift = (shift + 8) & 0x1F;
		}
		return (int)hash;
	}

	public int GetShortestBitLength() => BitLength;
	TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
	public static int HammingDistance(MpuT x, MpuT y) => (int)Mpir.MpuHamdist(x, y);

	/// <summary>
	/// Import the integer in the byte array bytes, starting at startOffset and ending at endOffset.
	/// Endianess is specified by order, which is 1 for big endian or -1 for little endian.
	/// </summary>
	public void ImportByOffset(ReadOnlySpan<byte> source, int startOffset, int endOffset, int order) =>
		Mpir.MpirMpuImportByOffset(this, startOffset, endOffset, order, sizeof(byte), 0, 0u, source);

	public int IndexOfOne(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpuScan1(this, (uint)startingIndex);
		}
	}

	public int IndexOfZero(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpuScan0(this, (uint)startingIndex);
		}
	}

	public bool InverseModExists(MpuT mod)
	{
		TryInvertMod(mod, out _);
		return true;
	}

	public MpuT InvertMod(MpuT mod)
	{
		var z = new MpuT();
		var status = Mpir.MpuInvert(z, this, mod);
		if (status == 0)
			throw new ArithmeticException("This modular inverse does not exists.");
		return z;
	}

	public static bool IsCanonical(MpuT value) => true;
	public static bool IsComplexNumber(MpuT value) => true;

	public bool IsDivisibleBy(int x)
	{
		if (x < 0)
			throw new OverflowException(NoNegativeNumbers);
		return Mpir.MpuDivisibleUiP(this, (uint)x) != 0;
	}

	public bool IsDivisibleBy(MpuT x) => Mpir.MpuDivisibleP(this, x) != 0;
	public bool IsDivisibleBy(uint x) => Mpir.MpuDivisibleUiP(this, x) != 0;
	public static bool IsEvenInteger(MpuT value) => (value & 1) == 0;
	public static bool IsFinite(MpuT value) => true;
	public static bool IsImaginaryNumber(MpuT value) => false;
	public static bool IsInfinity(MpuT value) => false;

	private bool IsInPowersOfFiveDictionary()
	{
		if (BitLength > 69658 || PowersOfTen.Count == 0)
			return false;
		MpuT? powerOfFive;
		lock (lockObj)
			if (!PowersOfFive.TryGetValue((int)Mpir.MpuSizeinbase(this, 10), out powerOfFive)
				&& !PowersOfFive.TryGetValue((int)Mpir.MpuSizeinbase(this, 10) - 1, out powerOfFive))
				return false;
		if (!ReferenceEquals(powerOfFive, this))
			return false;
		return true;
	}

	private bool IsInPowersOfTenDictionary()
	{
		if (BitLength > 99658 || PowersOfTen.Count == 0)
			return false;
		MpuT? powerOfTen;
		lock (lockObj)
			if (!PowersOfTen.TryGetValue((int)Mpir.MpuSizeinbase(this, 10), out powerOfTen)
				&& !PowersOfTen.TryGetValue((int)Mpir.MpuSizeinbase(this, 10) - 1, out powerOfTen))
				return false;
		if (!ReferenceEquals(powerOfTen, this))
			return false;
		return true;
	}

	public static bool IsInteger(MpuT value) => true;
	public static bool IsNaN(MpuT value) => false;
	public static bool IsNegative(MpuT value) => Mpir.MpuCmpSi(value, 0) < 0;
	public static bool IsNegativeInfinity(MpuT value) => false;
	public static bool IsNormal(MpuT value) => true;
	public static bool IsOddInteger(MpuT value) => !IsEvenInteger(value);

	public bool IsPerfectPower() =>
		// There is a known issue with this function for negative inputs in GMP 4.2.4.
		// Haven't heard of any issues in MPIR 5.x though.
		Mpir.MpuPerfectPowerP(this) != 0;

	public bool IsPerfectSquare() => Mpir.MpuPerfectSquareP(this) != 0;
	public static bool IsPow2(MpuT value) => value.PopCount() == 1;
	public static bool IsPositive(MpuT value) => Mpir.MpuCmpSi(value, 0) > 0;
	public static bool IsPositiveInfinity(MpuT value) => false;

	public bool IsProbablyPrimeRabinMiller(uint repetitions)
	{
		var result = Mpir.MpuProbabPrimeP(this, repetitions);
		return result != 0;
	}

	public static bool IsRealNumber(MpuT value) => true;
	public static bool IsSubnormal(MpuT value) => true;
	public static bool IsZero(MpuT value) => Mpir.MpuCmpSi(value, 0) == 0;

	public static int JacobiSymbol(int x, MpuT y)
	{
		if (IsEvenInteger(y) || Mpir.MpuCmpSi(y, 0) < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpuSiKronecker(x, y);
	}

	public static int JacobiSymbol(MpuT x, int y)
	{
		if ((y & 1) == 0 || Mpir.MpuCmpSi(y, 0) < 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpuKroneckerSi(x, y);
	}

	public static int JacobiSymbol(MpuT x, MpuT y)
	{
		if (IsEvenInteger(y) || Mpir.MpuCmpSi(y, 0) < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpuJacobi(x, y);
	}

	public static int JacobiSymbol(MpuT x, uint y)
	{
		if ((y & 1) == 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpuKroneckerUi(x, y);
	}

	public static int JacobiSymbol(uint x, MpuT y)
	{
		if (IsEvenInteger(y) || Mpir.MpuCmpSi(y, 0) < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpuUiKronecker(x, y);
	}

	public static int KroneckerSymbol(int x, MpuT y) => Mpir.MpuSiKronecker(x, y);
	public static int KroneckerSymbol(MpuT x, int y) => Mpir.MpuKroneckerSi(x, y);
	public static int KroneckerSymbol(MpuT x, MpuT y) => Mpir.MpuKronecker(x, y);
	public static int KroneckerSymbol(MpuT x, uint y) => Mpir.MpuKroneckerUi(x, y);
	public static int KroneckerSymbol(uint x, MpuT y) => Mpir.MpuUiKronecker(x, y);

	public static MpuT Lcm(int x, MpuT y)
	{
		if (x < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuLcmUi(z, y, (uint)x);
		return z;
	}

	public static MpuT Lcm(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(y, 0) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuLcmUi(z, x, (uint)y);
		return z;
	}

	public static MpuT Lcm(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuLcm(z, x, y);
		return z;
	}

	public static MpuT Lcm(MpuT x, uint y)
	{
		var z = new MpuT();
		Mpir.MpuLcmUi(z, x, y);
		return z;
	}

	public static MpuT Lcm(uint x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuLcmUi(z, y, x);
		return z;
	}

	public static int LegendreSymbol(MpuT x, MpuT primeY)
	{
		Debug.Assert(primeY != 2); // Not defined for 2

		return Mpir.MpuJacobi(x, primeY);
	}

	public static MpuT Log2(MpuT value)
	{
		var bitLength = value.BitLength;
		var sqrt = (One << bitLength << bitLength - 1).Sqrt();
		return Mpir.MpuCmp(value, sqrt) >= 0 ? bitLength : bitLength - 1;
	}

	public static MpuT Lucas(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		Mpir.MpuLucnumUi(z, (uint)n);
		return z;
	}

	public static MpuT Lucas(int n, out MpuT previous)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		previous = new MpuT();
		Mpir.MpuLucnum2Ui(z, previous, (uint)n);
		return z;
	}

	public static MpuT Lucas(uint n)
	{
		var z = new MpuT();
		Mpir.MpuLucnumUi(z, n);
		return z;
	}

	public static MpuT Lucas(uint n, out MpuT previous)
	{
		var z = new MpuT();
		previous = new MpuT();
		Mpir.MpuLucnum2Ui(z, previous, n);
		return z;
	}

	public static MpuT Max(MpuT x, MpuT y) => Mpir.MpuCmp(x, y) > 0 ? x : y;
	public static MpuT MaxMagnitude(MpuT x, MpuT y) => Max(x, y);
	public static MpuT MaxMagnitudeNumber(MpuT x, MpuT y) => Max(x, y);
	public static MpuT Min(MpuT x, MpuT y) => Mpir.MpuCmp(x, y) < 0 ? x : y;
	public static MpuT MinMagnitude(MpuT x, MpuT y) => Min(x, y);
	public static MpuT MinMagnitudeNumber(MpuT x, MpuT y) => Min(x, y);
	public MpuT Mod(MpuT mod) => this % mod;
	public MpuT Mod(int mod) => this % mod;
	public MpuT Mod(uint mod) => this % mod;

	public int ModAsInt32(int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		return (int)Mpir.MpuFdivUi(this, (uint)mod);
	}

	public uint ModAsUInt32(uint mod) => Mpir.MpuFdivUi(this, mod);

	public MpuT Multiply(int x) => this * x;
	public MpuT Multiply(MpuT x) => this * x;
	public MpuT Multiply(uint x) => this * x;
	public MpzT Negate() => -this;

	// TODO: Create a version of this method which takes in a parameter to represent how well tested the prime should be.
	public MpuT NextPrimeGMP()
	{
		var z = new MpuT();
		Mpir.MpuNextprime(z, this);
		return z;
	}

	public MpuT Or(MpuT x) => this | x;
	public static MpuT Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);
	public static MpuT Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		Parse(s.ToString(), style, provider);
	public static MpuT Parse(string s) => new(s);
	public static MpuT Parse(string s, IFormatProvider? provider) => new(s);
	public static MpuT Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);
	public int PopCount() => (int)Mpir.MpuPopcount(this);
	public static MpuT PopCount(MpuT value) => value.PopCount();

	public MpuT Power(int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpuT();
		Mpir.MpuPowUi(z, this, (uint)exponent);
		return z;
	}

	public static MpuT Power(int x, int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpuT();
		Mpir.MpuUiPowUi(z, (uint)x, (uint)exponent);
		return z;
	}

	public MpuT Power(uint exponent)
	{
		var z = new MpuT();
		Mpir.MpuPowUi(z, this, exponent);
		return z;
	}

	public static MpuT Power(uint x, uint exponent)
	{
		var z = new MpuT();
		Mpir.MpuUiPowUi(z, x, exponent);
		return z;
	}

	public MpuT PowerMod(MpuT exponent, MpuT mod)
	{
		var z = new MpuT();
		Mpir.MpuPowm(z, this, exponent, mod);
		return z;
	}

	public MpuT PowerMod(int exponent, MpuT mod)
	{
		var z = new MpuT();
		Mpir.MpuPowm(z, this, exponent, mod);
		return z;
	}

	public MpuT PowerMod(uint exponent, MpuT mod)
	{
		var z = new MpuT();
		if (exponent >= 0)
			Mpir.MpuPowmUi(z, this, exponent, mod);
		else
		{
			MpuT bigExponent = exponent;
			var inverse = bigExponent.InvertMod(mod);
			Mpir.MpuPowmUi(z, inverse, exponent, mod);
		}
		return z;
	}

	public static MpuT PowerOf5(int exponent)
	{
		if ((int)Math.Ceiling(exponent * Math.Log2(10)) > 99658)
			return five.Power(exponent);
		if (PowersOfFive.TryGetValue(exponent, out var power))
			return power;
		lock (lockObj)
		{
			if (PowersOfFive.TryGetValue(exponent, out power))
				return power;
			return PowersOfFive[exponent] = ten.Power(exponent);
		}
	}

	public static MpuT PowerOf10(int exponent)
	{
		if ((int)Math.Ceiling(exponent * Math.Log2(10)) > 99658)
			return ten.Power(exponent);
		if (PowersOfTen.TryGetValue(exponent, out var power))
			return power;
		lock (lockObj)
		{
			if (PowersOfTen.TryGetValue(exponent, out power))
				return power;
			return PowersOfTen[exponent] = ten.Power(exponent);
		}
	}

	private static void ProcessLongConversion(MpuT value)
	{
		var exportBytesSpan = ProcessToByteArray(value, BitConverter.IsLittleEndian);
		var length = Math.Min(exportBytesSpan.Length, convertToLongBytes.Length);
		var destOffset = BitConverter.IsLittleEndian ? 0 : 8 - length;
		convertToLongBytes.AsSpan(BitConverter.IsLittleEndian ? length.. : ..destOffset).Clear();
		exportBytesSpan[BitConverter.IsLittleEndian ? 0..length : ^length..].CopyTo(convertToLongBytes.AsSpan(destOffset));
	}

	private static Span<byte> ProcessToByteArray(MpuT value, bool bLittleEndian)
	{
		var exportLength = (int)Math.Min(Mpir.MpuSizeinbase(value, 256), 2147483647);
		if (exportLength > exportBytes.Length)
		{
			var newCapacity = Math.Max(1024, exportBytes.Length * 2);
			if ((uint)newCapacity > int.MaxValue)
				newCapacity = int.MaxValue;
			if (newCapacity < exportLength)
				newCapacity = exportLength;
			exportBytes = GC.AllocateUninitializedArray<byte>(newCapacity);
		}
		var exportBytesSpan = exportBytes.AsSpan(..exportLength);
		if (bLittleEndian)
			value.TryWriteLittleEndian(exportBytesSpan, out _);
		else
			value.TryWriteBigEndian(exportBytesSpan, out _);
		return exportBytesSpan;
	}

	public MpuT Remainder(MpuT x)
	{
		var z = new MpuT();
		Mpir.MpuTdivR(z, this, x);
		return z;
	}

	public MpuT RemoveFactor(MpuT factor)
	{
		var z = new MpuT();
		Mpir.MpuRemove(z, this, factor);
		return z;
	}

	public MpuT RemoveFactor(MpuT factor, out int count)
	{
		var z = new MpuT();
		count = (int)Mpir.MpuRemove(z, this, factor);
		return z;
	}

	public MpuT Root(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		Mpir.MpuRoot(z, this, (uint)n);
		return z;
	}

	public MpuT Root(int n, out bool isExact)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		var result = Mpir.MpuRoot(z, this, (uint)n);
		isExact = result != 0;
		return z;
	}

	public MpuT Root(int n, out MpuT remainder)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		remainder = new MpuT();
		Mpir.MpuRootrem(z, remainder, this, (uint)n);
		return z;
	}

	public MpuT Root(uint n)
	{
		var z = new MpuT();
		Mpir.MpuRoot(z, this, n);
		return z;
	}

	public MpuT Root(uint n, out bool isExact)
	{
		var z = new MpuT();
		var result = Mpir.MpuRoot(z, this, n);
		isExact = result != 0;
		return z;
	}

	public MpuT Root(uint n, out MpuT remainder)
	{
		var z = new MpuT();
		remainder = new MpuT();
		Mpir.MpuRootrem(z, remainder, this, n);
		return z;
	}

	public MpuT ShiftLeft(int shiftAmount) => this << shiftAmount;
	public MpuT ShiftLeftDec(int shiftAmount) => this * PowerOf10(shiftAmount);
	public MpuT ShiftRight(int shiftAmount) => this >> shiftAmount;
	public MpuT ShiftRightDec(int shiftAmount) => this / PowerOf10(shiftAmount);

	public MpuT ShiftRightRound(int shiftAmount)
	{
		if (shiftAmount <= 0)
			return new(this);
		var result = this >> shiftAmount;
		if (shiftAmount <= 32)
		{
			if ((this & uint.MaxValue >>> BitsPerInt - shiftAmount) >= 1u << shiftAmount - 1)
				result++;
		}
		else
		{
			using var left = One << shiftAmount;
			Mpir.MpuSubUi(left, left, 1);
			Mpir.MpuAnd(left, left, this);
			using var right = One << shiftAmount - 1;
			if (Mpir.MpuCmp(left, right) >= 0)
				result++;
		}
		return result;
	}

	public MpuT ShiftRightRoundDec(int shiftAmount)
	{
		if (shiftAmount <= 0)
			return new(this);
		var result = ShiftRightDec(shiftAmount);
		if (shiftAmount <= 9)
		{
			if (this % smallPowersOfTen[shiftAmount] >= 5 * smallPowersOfTen[shiftAmount - 1])
				result++;
		}
		else
		{
			using var left = this % PowerOf10(shiftAmount);
			using var right = 5 * PowerOf10(shiftAmount - 1);
			if (Mpir.MpuCmp(left, right) >= 0)
				result++;
		}
		return result;
	}

	public MpuT Sqrt()
	{
		var z = new MpuT();
		Mpir.MpuSqrt(z, this);
		return z;
	}

	public MpuT Sqrt(out bool isExact)
	{
		var z = new MpuT();
		var result = Mpir.MpuRoot(z, this, 2);
		isExact = result != 0;
		return z;
	}

	public MpuT Sqrt(out MpuT remainder)
	{
		var z = new MpuT();
		remainder = new MpuT();
		Mpir.MpuSqrtrem(z, remainder, this);
		return z;
	}

	public MpuT Subtract(int x) => this - x;
	public MpuT Subtract(MpuT x) => this - x;
	public MpuT Subtract(uint x) => this - x;
	public MpuT Square() => this * this;

	/// <summary>
	/// Export to the value to a byte array.
	/// Endianess is specified by order, which is 1 for big endian or -1 for little endian.
	/// </summary>
	public byte[] ToByteArray(int order) => val == 0 ? [] : Mpir.MpirMpuExport(order, sizeof(byte), 0, 0u, this);

	public static MpuT TrailingZeroCount(MpuT value)
	{
		if (value == Zero)
			return Zero;
		var result = 0;
		const int ulongBits = BitsPerLong;
		var value2 = value << ulongBits;
		MpuT mask = ulong.MaxValue;
		for (; Mpir.MpuCmp(mask, value2) < 0; mask <<= ulongBits)
		{
			var maskedValue = value & mask;
			if (Mpir.MpuCmpSi(maskedValue, 0) == 0)
				result += ulongBits;
			else
				return result + (int)ulong.TrailingZeroCount((ulong)(maskedValue >> result));
		}
		throw new InvalidOperationException("Невозможно добавить элемент. Возможные причины:\r\n" + InternalError
			+ $"Текущее состояние: длина - {value.BitLength}, значение - {value}"
			+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	public bool TryInvertMod(MpuT mod, [MaybeNullWhen(false)] out MpuT result)
	{
		var z = new MpuT();
		var status = Mpir.MpuInvert(z, this, mod);
		if (status == 0)
		{
			result = default;
			return false;
		}
		else
		{
			result = z;
			return true;
		}
	}

	public BigInteger ToBigInteger() => new([.. ToByteArray(-1), 0]);
	bool IConvertible.ToBoolean(IFormatProvider? provider) => Mpir.MpuCmpSi(this, 1) >= 0;
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

	public override string? ToString() => ToString((int)DefaultStringBase);

	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString());

	public string? ToString(uint @base) => val == 0 ? "0" : Mpir.MpuGetString(@base, this);
	string IConvertible.ToString(IFormatProvider? provider) => ToString() ?? "";

	object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(conversionType);
		if (conversionType == typeof(MpuT))
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
			return value;
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(MpzT) + ", " + nameof(MpuT)
				+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	private static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out MpuT result)
	{
		try
		{
			result = value switch
			{
				MpuT uz => uz,
				MpzT z => (MpuT)z,
				byte y => y,
				sbyte sy => sy,
				short si => si,
				ushort usi => usi,
				int i => i,
				uint ui => ui,
				long li => li,
				ulong uli => uli,
				float f => (MpuT)f,
				double d => (MpuT)d,
				decimal m => (MpuT)(double)m,
				BigInteger ll => new(ll),
				string s => new(s),
				_ => throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(MpuT)
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

	static bool INumberBase<MpuT>.TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out MpuT result) => TryConvertFromChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out MpuT result)
	{
		try
		{
			result = value switch
			{
				MpuT uz => uz,
				MpzT z => (MpuT)z,
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
				_ => throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(MpuT)
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

	static bool INumberBase<MpuT>.TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out MpuT result) => TryConvertFromChecked(value, out result);

	private static bool TryConvertToChecked<TOther>(MpuT value, out TOther result)
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

	static bool INumberBase<MpuT>.TryConvertToChecked<TOther>(MpuT value, out TOther result) => TryConvertToChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertToSaturating<TOther>(MpuT value, out TOther result) => TryConvertToChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertToTruncating<TOther>(MpuT value, out TOther result) => TryConvertToChecked(value, out result);

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
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

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out MpuT result) => TryParse(s.ToString(), out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out MpuT result) => TryParse(s.ToString(), out result);
	public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out MpuT result)
	{
		try
		{
			result = Parse(s ?? "");
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out MpuT result) => TryParse(s, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MpuT result) => TryParse(s, out result);

	public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out MpuT value)
	{
		value = new(source, 1);
		return true;
	}

	public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out MpuT value)
	{
		value = new(source, -1);
		return true;
	}

	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
	{
		var bufSize = (int)Math.Min(Mpir.MpuSizeinbase(this, 256), 2147483647);
		if (destination.Length >= bufSize)
		{
			Mpir.MpirMpuExport(destination[^bufSize..], 1, sizeof(byte), 0, 0u, this);
			bytesWritten = bufSize;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		var bufSize = (int)Math.Min(Mpir.MpuSizeinbase(this, 256), 2147483647);
		if (destination.Length >= bufSize)
		{
			destination[0] = 0;
			Mpir.MpirMpuExport(destination, -1, sizeof(byte), 0, 0u, this);
			bytesWritten = bufSize;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public MpuT Xor(MpuT x) => this ^ x;

	public static implicit operator MpuT(byte value) => new((uint)value);
	public static implicit operator MpuT(short value) => new(value);
	public static implicit operator MpuT(ushort value) => new(value);
	public static implicit operator MpuT(int value) => new(value);
	public static implicit operator MpuT(uint value) => new(value);
	public static implicit operator MpuT(long value) => new(value);
	public static implicit operator MpuT(ulong value) => new(value);
	public static explicit operator MpuT(float value) => new((double)value);
	public static explicit operator MpuT(double value) => new(value);
	public static explicit operator MpuT(decimal value) => new(value);
	public static explicit operator MpuT(MpzT value) => new(value);
	public static explicit operator MpuT(string value) => new(value, DefaultStringBase);
	public static explicit operator byte(MpuT value) => (byte)(uint)value;
	public static explicit operator short(MpuT value) => (short)(int)value;
	public static explicit operator ushort(MpuT value) => (ushort)(uint)value;
	public static explicit operator int(MpuT value) => Mpir.MpuGetSi(value);

	public static explicit operator uint(MpuT value)
	{
		var result = Mpir.MpuGetUi(value);
		if (Mpir.MpuCmpSi(value, 0) < 0)
			result = ~result + 1;
		return result;
	}

	public static explicit operator long(MpuT value)
	{
		lock (lockObj)
		{
			ProcessLongConversion(value);
			return BitConverter.ToInt64(convertToLongBytes, 0);
		}
	}

	public static explicit operator ulong(MpuT value)
	{
		lock (lockObj)
		{
			ProcessLongConversion(value);
			return BitConverter.ToUInt64(convertToLongBytes, 0);
		}
	}

	public static explicit operator float(MpuT value) => (float)(double)value;
	public static explicit operator double(MpuT value) => Mpir.MpuGetD(value);
	public static explicit operator decimal(MpuT value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);
	public static explicit operator string?(MpuT value) => value.ToString();

	public static MpuT operator +(MpuT value) => new(value);

	/// <summary>
	/// Computes the unary negation of a value as a number of the type <see cref="MpzT"/>.
	/// </summary>
	/// <param name="x">The value for which to compute its unary negation.</param>
	/// <returns>The unary negation of <paramref name="x"/>.</returns>
	public static MpzT operator -(MpuT x)
	{
		var z = new MpzT();
		Mpir.MpzNeg(z, Unsafe.As<MpzT>(x));
		return z;
	}

	static MpuT IUnaryNegationOperators<MpuT, MpuT>.operator -(MpuT value) =>
		throw new NotSupportedException(NoNegativeNumbers);

	/// <summary>
	/// Computes the ones-complement representation of a given value as a number of the type <see cref="MpzT"/>.
	/// </summary>
	/// <param name="x">The value for which to compute the ones-complement.</param>
	/// <returns>The ones-complement of <paramref name="x"/>.</returns>
	public static MpzT operator ~(MpuT x)
	{
		var z = new MpzT();
		Mpir.MpzCom(z, Unsafe.As<MpzT>(x));
		return z;
	}

	static MpuT IBitwiseOperators<MpuT, MpuT, MpuT>.operator ~(MpuT value) =>
		throw new NotSupportedException(NoNegativeNumbers);

	public static MpuT operator +(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuAdd(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator +(MpuT, MpuT)"/>
	public static MpuT operator +(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(x, -y) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		if (y >= 0)
			Mpir.MpuAddUi(z, x, (uint)y);
		else
			Mpir.MpuSubUi(z, x, (uint)-y);
		return z;
	}

	/// <inheritdoc cref="operator +(MpuT, MpuT)"/>
	public static MpuT operator +(int x, MpuT y)
	{
		if (Mpir.MpuCmpSi(y, -x) > 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		if (x >= 0)
			Mpir.MpuAddUi(z, y, (uint)x);
		else
			Mpir.MpuSubUi(z, y, (uint)-x);
		return z;
	}

	/// <inheritdoc cref="operator +(MpuT, MpuT)"/>
	public static MpuT operator +(MpuT x, uint y)
	{
		var z = new MpuT();
		Mpir.MpuAddUi(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator +(MpuT, MpuT)"/>
	public static MpuT operator +(uint x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuAddUi(z, y, x);
		return z;
	}

	public static MpuT operator -(MpuT x, MpuT y)
	{
		if (Mpir.MpuCmp(x, y) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuSub(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator -(MpuT, MpuT)"/>
	public static MpuT operator -(int x, MpuT y)
	{
		if (Mpir.MpuCmpSi(y, x) > 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuUiSub(z, (uint)x, y);
		return z;
	}

	/// <inheritdoc cref="operator -(MpuT, MpuT)"/>
	public static MpuT operator -(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(x, y) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		if (y >= 0)
			Mpir.MpuSubUi(z, x, (uint)y);
		else
			Mpir.MpuAddUi(z, x, (uint)-y);

		return z;
	}

	/// <inheritdoc cref="operator -(MpuT, MpuT)"/>
	public static MpuT operator -(uint x, MpuT y)
	{
		if (Mpir.MpuCmpUi(y, x) > 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuUiSub(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator -(MpuT, MpuT)"/>
	public static MpuT operator -(MpuT x, uint y)
	{
		if (Mpir.MpuCmpUi(x, y) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuSubUi(z, x, y);
		return z;
	}

	public static MpuT operator ++(MpuT x)
	{
		var z = new MpuT();
		Mpir.MpuAddUi(z, x, 1);
		return z;
	}

	public static MpuT operator --(MpuT x)
	{
		if (Mpir.MpuCmpUi(x, 0) == 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuSubUi(z, x, 1);
		return z;
	}

	public static MpuT operator *(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuMul(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator *(MpuT, MpuT)"/>
	public static MpuT operator *(int x, MpuT y)
	{
		if (x < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuMulSi(z, y, x);
		return z;
	}

	/// <inheritdoc cref="operator *(MpuT, MpuT)"/>
	public static MpuT operator *(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(y, 0) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var z = new MpuT();
		Mpir.MpuMulSi(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator *(MpuT, MpuT)"/>
	public static MpuT operator *(uint x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuMulUi(z, y, x);
		return z;
	}

	/// <inheritdoc cref="operator *(MpuT, MpuT)"/>
	public static MpuT operator *(MpuT x, uint y)
	{
		var z = new MpuT();
		Mpir.MpuMulUi(z, x, y);
		return z;
	}

	public static MpuT operator /(MpuT x, MpuT y)
	{
		var quotient = new MpuT();
		Mpir.MpuTdivQ(quotient, x, y);
		return quotient;
	}

	/// <inheritdoc cref="operator /(MpuT, MpuT)"/>
	public static MpuT operator /(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(y, 0) < 0)
			throw new OverflowException(NoNegativeNumbers);
		var quotient = new MpuT();
		Mpir.MpuTdivQUi(quotient, x, (uint)y);
		return quotient;
	}

	/// <inheritdoc cref="operator /(MpuT, MpuT)"/>
	public static MpuT operator /(MpuT x, uint y)
	{
		var quotient = new MpuT();
		Mpir.MpuTdivQUi(quotient, x, y);
		return quotient;
	}

	public static MpuT operator %(MpuT x, MpuT mod)
	{
		var z = new MpuT();
		Mpir.MpuMod(z, x, mod);
		return z;
	}

	/// <inheritdoc cref="operator %(MpuT, MpuT)"/>
	public static MpuT operator %(MpuT x, int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		var z = new MpuT();
		Mpir.MpuFdivRUi(z, x, (uint)mod);
		return z;
	}

	/// <inheritdoc cref="operator %(MpuT, MpuT)"/>
	public static MpuT operator %(MpuT x, uint mod)
	{
		var z = new MpuT();
		Mpir.MpuFdivRUi(z, x, mod);
		return z;
	}

	/// <inheritdoc cref="operator &(MpuT, MpuT)"/>
	public static int operator &(MpuT x, int y) => Mpir.MpuGetSi(x) & y;
	/// <inheritdoc cref="operator &(MpuT, MpuT)"/>
	public static uint operator &(MpuT x, uint y) => Mpir.MpuGetUi(x) & y;

	public static MpuT operator &(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuAnd(z, x, y);
		return z;
	}

	public static MpuT operator |(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuIor(z, x, y);
		return z;
	}

	public static MpuT operator ^(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuXor(z, x, y);
		return z;
	}

	public static MpuT operator <<(MpuT x, int shiftAmount)
	{
		var z = new MpuT();
		Mpir.MpuMul2exp(z, x, (uint)shiftAmount);
		return z;
	}

	public static MpuT operator >>(MpuT x, int shiftAmount)
	{
		var z = new MpuT();
		Mpir.MpuTdivQ2exp(z, x, (uint)shiftAmount);
		return z;
	}

	public static MpuT operator >>>(MpuT x, int shiftAmount)
	{
		if (Mpir.MpuCmpSi(x, 0) >= 0)
			return x >> shiftAmount;
		throw new OverflowException(NoNegativeNumbers);
	}

	public static bool operator ==(MpuT? x, MpuT? y) => (x ?? Zero).CompareTo(y) == 0;
	public static bool operator ==(int x, MpuT y) => y.CompareTo(x) == 0;
	public static bool operator ==(MpuT x, int y) => x.CompareTo(y) == 0;
	public static bool operator ==(uint x, MpuT y) => y.CompareTo(x) == 0;
	public static bool operator ==(MpuT x, uint y) => x.CompareTo(y) == 0;
	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(long x, MpuT y) => y.CompareTo(x) == 0;
	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(MpuT x, long y) => x.CompareTo(y) == 0;
	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(ulong x, MpuT y) => y.CompareTo(x) == 0;
	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(MpuT x, ulong y) => x.CompareTo(y) == 0;
	public static bool operator ==(float x, MpuT y) => y.CompareTo(x) == 0;
	public static bool operator ==(MpuT x, float y) => x.CompareTo(y) == 0;
	public static bool operator ==(double x, MpuT y) => y.CompareTo(x) == 0;
	public static bool operator ==(MpuT x, double y) => x.CompareTo(y) == 0;
	public static bool operator ==(decimal x, MpuT y) => y.CompareTo(x) == 0;
	public static bool operator ==(MpuT x, decimal y) => x.CompareTo(y) == 0;
	public static bool operator !=(MpuT? x, MpuT? y) => (x ?? Zero).CompareTo(y) != 0;
	public static bool operator !=(int x, MpuT y) => y.CompareTo(x) != 0;
	public static bool operator !=(MpuT x, int y) => x.CompareTo(y) != 0;
	public static bool operator !=(uint x, MpuT y) => y.CompareTo(x) != 0;
	public static bool operator !=(MpuT x, uint y) => x.CompareTo(y) != 0;
	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(long x, MpuT y) => y.CompareTo(x) != 0;
	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(MpuT x, long y) => x.CompareTo(y) != 0;
	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(ulong x, MpuT y) => y.CompareTo(x) != 0;
	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(MpuT x, ulong y) => x.CompareTo(y) != 0;
	public static bool operator !=(float x, MpuT y) => y.CompareTo(x) != 0;
	public static bool operator !=(MpuT x, float y) => x.CompareTo(y) != 0;
	public static bool operator !=(double x, MpuT y) => y.CompareTo(x) != 0;
	public static bool operator !=(MpuT x, double y) => x.CompareTo(y) != 0;
	public static bool operator !=(decimal x, MpuT y) => y.CompareTo(x) != 0;
	public static bool operator !=(MpuT x, decimal y) => x.CompareTo(y) != 0;
	public static bool operator >=(MpuT x, MpuT y) => x.CompareTo(y) >= 0;
	public static bool operator >=(int x, MpuT y) => y.CompareTo(x) <= 0;
	public static bool operator >=(MpuT x, int y) => x.CompareTo(y) >= 0;
	public static bool operator >=(uint x, MpuT y) => y.CompareTo(x) <= 0;
	public static bool operator >=(MpuT x, uint y) => x.CompareTo(y) >= 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >=(long x, MpuT y) => x.CompareTo(y) >= 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >=(MpuT x, long y) => y.CompareTo(x) <= 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >=(ulong x, MpuT y) => x.CompareTo(y) >= 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >=(MpuT x, ulong y) => y.CompareTo(x) <= 0;
	public static bool operator >=(float x, MpuT y) => y.CompareTo(x) <= 0;
	public static bool operator >=(MpuT x, float y) => x.CompareTo(y) >= 0;
	public static bool operator >=(double x, MpuT y) => y.CompareTo(x) <= 0;
	public static bool operator >=(MpuT x, double y) => x.CompareTo(y) >= 0;
	public static bool operator >=(decimal x, MpuT y) => y.CompareTo(x) <= 0;
	public static bool operator >=(MpuT x, decimal y) => x.CompareTo(y) >= 0;
	public static bool operator <=(MpuT x, MpuT y) => x.CompareTo(y) <= 0;
	public static bool operator <=(int x, MpuT y) => y.CompareTo(x) >= 0;
	public static bool operator <=(MpuT x, int y) => x.CompareTo(y) <= 0;
	public static bool operator <=(uint x, MpuT y) => y.CompareTo(x) >= 0;
	public static bool operator <=(MpuT x, uint y) => x.CompareTo(y) <= 0;
	// TODO: Implement by accessing the data directly
	public static bool operator <=(long x, MpuT y) => x.CompareTo(y) <= 0;
	public static bool operator <=(MpuT x, long y) => y.CompareTo(x) >= 0;
	public static bool operator <=(ulong x, MpuT y) => x.CompareTo(y) <= 0;
	public static bool operator <=(MpuT x, ulong y) => y.CompareTo(x) >= 0;
	public static bool operator <=(float x, MpuT y) => y.CompareTo(x) >= 0;
	public static bool operator <=(MpuT x, float y) => x.CompareTo(y) <= 0;
	public static bool operator <=(double x, MpuT y) => y.CompareTo(x) >= 0;
	public static bool operator <=(MpuT x, double y) => x.CompareTo(y) <= 0;
	public static bool operator <=(decimal x, MpuT y) => y.CompareTo(x) >= 0;
	public static bool operator <=(MpuT x, decimal y) => x.CompareTo(y) <= 0;
	public static bool operator >(MpuT x, MpuT y) => x.CompareTo(y) > 0;
	public static bool operator >(int x, MpuT y) => y.CompareTo(x) < 0;
	public static bool operator >(MpuT x, int y) => x.CompareTo(y) > 0;
	public static bool operator >(uint x, MpuT y) => y.CompareTo(x) < 0;
	public static bool operator >(MpuT x, uint y) => x.CompareTo(y) > 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >(long x, MpuT y) => y.CompareTo(x) < 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >(MpuT x, long y) => x.CompareTo(y) > 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >(ulong x, MpuT y) => y.CompareTo(x) < 0;
	// TODO: Implement by accessing the data directly
	public static bool operator >(MpuT x, ulong y) => x.CompareTo(y) > 0;
	public static bool operator >(float x, MpuT y) => y.CompareTo(x) < 0;
	public static bool operator >(MpuT x, float y) => x.CompareTo(y) > 0;
	public static bool operator >(double x, MpuT y) => y.CompareTo(x) < 0;
	public static bool operator >(MpuT x, double y) => x.CompareTo(y) > 0;
	public static bool operator >(decimal x, MpuT y) => y.CompareTo(x) < 0;
	public static bool operator >(MpuT x, decimal y) => x.CompareTo(y) > 0;
	public static bool operator <(MpuT x, MpuT y) => x.CompareTo(y) < 0;
	public static bool operator <(int x, MpuT y) => y.CompareTo(x) > 0;
	public static bool operator <(MpuT x, int y) => x.CompareTo(y) < 0;
	public static bool operator <(uint x, MpuT y) => y.CompareTo(x) > 0;
	public static bool operator <(MpuT x, uint y) => x.CompareTo(y) < 0;
	public static bool operator <(long x, MpuT y) => x.CompareTo(y) < 0;
	public static bool operator <(MpuT x, long y) => y.CompareTo(x) > 0;
	public static bool operator <(ulong x, MpuT y) => x.CompareTo(y) < 0;
	public static bool operator <(MpuT x, ulong y) => y.CompareTo(x) > 0;
	public static bool operator <(float x, MpuT y) => y.CompareTo(x) > 0;
	public static bool operator <(MpuT x, float y) => x.CompareTo(y) < 0;
	public static bool operator <(double x, MpuT y) => y.CompareTo(x) > 0;
	public static bool operator <(MpuT x, double y) => x.CompareTo(y) < 0;
	public static bool operator <(decimal x, MpuT y) => y.CompareTo(x) > 0;
	public static bool operator <(MpuT x, decimal y) => x.CompareTo(y) < 0;
}
