namespace NStar.Mpir;

/// <summary>Represents an arbitrarily large signed integer.</summary>
public sealed class MpzT : ICloneable, IConvertible, IComparable, IComparable<MpzT>, IDisposable, IBinaryInteger<MpzT>
{
	internal const uint DefaultStringBase = 10u;
	private static readonly byte[] convertToLongBytes = GC.AllocateUninitializedArray<byte>(8);
	private static byte[] exportBytes = GC.AllocateUninitializedArray<byte>(1024);
	private static readonly Lock lockObj = new();
	private const string InternalError = "1. Конкурентный доступ из нескольких потоков (используйте синхронизацию).\r\n"
		+ "2. Нарушение целостности структуры списка (ошибка в логике -"
		+ " список все еще не в релизной версии, разные ошибки в структуре в некоторых случаях возможны).\r\n"
		+ "3. Системная ошибка (память, диск и т. д.).\r\n";

	internal nint val;

	/// <summary>Initializes a new MpzT to 0.</summary>
	public MpzT() => val = Mpir.MpzInit();
	/// <summary>Initializes a new MpzT to the same value as op.</summary>
	public MpzT(MpzT op) => val = Mpir.MpzInitSet(op);
	/// <summary>Initializes a new MpzT to the unsigned int op.</summary>
	public MpzT(uint op) => val = Mpir.MpzInitSetUi(op);
	/// <summary>Initializes a new MpzT to the int op.</summary>
	public MpzT(int op) => val = Mpir.MpzInitSetSi(op);
	/// <summary>Initializes a new MpzT to the double op.</summary>
	public MpzT(double op) => val = Mpir.MpzInitSetD(op);
	/// <summary>Initializes a new MpzT to string s, parsed as an integer in the specified base.</summary>
	public MpzT(string? s, uint @base) => val = Mpir.MpzInitSetStr(s ?? "0", @base);
	/// <summary>Initializes a new MpzT to string s, parsed as an integer in base 10.</summary>
	public MpzT(string? s) : this(s, DefaultStringBase) { }
	/// <summary>Initializes a new MpzT to the BigInteger op.</summary>
	public MpzT(BigInteger op) : this(op.ToByteArray(), -1) { }

	/// <summary>Initializes a new MpzT to using MPIR MpzInit2. Only use if you need to avoid reallocations.</summary>
	//
	// Initialization with MpzInit2 should not be confused with MpzT construction
	// from a ulong. Thus, so we use a static construction function instead, and add
	// the dummy type init2Type to enable us to write a ctor with a unique signature.
	public static MpzT Init2(ulong n) => new(Init2Type.init2, n);
	private enum Init2Type { init2 }
	private MpzT(Init2Type _, ulong n) => val = Mpir.MpzInit2(n);

	/// <summary>Initializes a new MpzT to the long op.</summary>
	public MpzT(long op) : this()
	{
		val = Mpir.MpzInitSetSi(unchecked((int)(op >> sizeof(int) * 8)));
		Mpir.MpzMul2exp(this, this, sizeof(int) * 8);
		Mpir.MpzAddUi(this, this, unchecked((uint)op));
	}

	/// <summary>Initializes a new MpzT to the unsigned long op.</summary>
	public MpzT(ulong op) : this()
	{
		val = Mpir.MpzInitSetUi(unchecked((uint)(op >> sizeof(uint) * 8)));
		Mpir.MpzMul2exp(this, this, sizeof(uint) * 8);
		Mpir.MpzAddUi(this, this, unchecked((uint)op));
	}

	public MpzT(decimal op) : this(new BigInteger(op)) { }

	public MpzT(MpuT op) => val = Mpir.MpzInitSet(op);

	/// <summary>
	/// Initializes a new MpzT to the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	/// </summary>
	public MpzT(ReadOnlySpan<byte> bytes, int order) : this() => FromByteArray(bytes, order);

	~MpzT() => Dispose(false);

	/// <summary>Gets the bit of the number at the specified index.</summary>
	public int this[int bitIndex] => Mpir.MpzTstbit(this, (uint)bitIndex);
	public static MpzT AdditiveIdentity => Zero;
	/// <summary>Gets the count of bits in the binary representation of the number.</summary>
	public int BitLength => (int)Mpir.MpzSizeinbase(this, 2);
	/// <summary>Gets the count of digits in the decimal representation of the number.</summary>
	public int DecLength => ToString() is var s && s is not null ? s.Length - (s.StartsWith('-') ? 1 : 0) : 1;
	public static MpzT MultiplicativeIdentity => One;
	public static MpzT One { get; } = new(1);
	public static int Radix => 2;
	/// <summary>Gets the sign of the number (in the format of the integer number 1, 0 or -1).</summary>
	public int Sign => IsPositive(this) ? 1 : IsZero(this) ? 0 : IsNegative(this) ? -1
		: throw new ArithmeticException("Произошла ошибка при вычислении знака.");
	public static MpzT Zero { get; } = new(0);

	/// <summary>
	/// Returns a new MpzT which is the absolute value of this value.
	/// </summary>
	public MpzT Abs()
	{
		var result = new MpzT();
		Mpir.MpzAbs(result, this);
		return result;
	}

	public static MpzT Abs(MpzT value) => value.Abs();

	public static MpzT Binomial(int n, int k)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpzT();
		if (n >= 0)
		{
			Mpir.MpzBinUiui(z, (uint)n, (uint)k);
			return z;
		}
		else
		{
			// Use the identity bin(n,k) = (-1)^k * bin(-n+k-1,k)
			Mpir.MpzBinUiui(z, (uint)(-n + k - 1), (uint)k);
			if ((k & 1) != 0)
			{
				var res = -z;
				return res;
			}
			else
				return z;
		}
	}

	public static MpzT Binomial(MpzT n, int k)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpzT();
		Mpir.MpzBinUi(z, n, (uint)k);
		return z;
	}

	public static MpzT Binomial(MpzT n, uint k)
	{
		var z = new MpzT();
		Mpir.MpzBinUi(z, n, k);
		return z;
	}

	public static MpzT Binomial(uint n, uint k)
	{
		var z = new MpzT();
		Mpir.MpzBinUiui(z, n, k);
		return z;
	}

	public MpzT ChangeBit(int bitIndex, int value)
	{
		var z = new MpzT(this);
		if (value == 0)
			Mpir.MpzClrbit(z, (uint)bitIndex);
		else
			Mpir.MpzSetbit(z, (uint)bitIndex);
		return z;
	}

	object ICloneable.Clone() => Clone();
	public MpzT Clone() => new(this);

	public static int Compare(int x, MpzT y) => -y.CompareTo(x);
	public static int Compare(MpzT x, int y) => x.CompareTo(y);
	public static int Compare(uint x, MpzT y) => -y.CompareTo(x);
	public static int Compare(MpzT x, uint y) => x.CompareTo(y);
	public static int Compare(long x, MpzT y) => -y.CompareTo(x);
	public static int Compare(MpzT x, long y) => x.CompareTo(y);
	public static int Compare(ulong x, MpzT y) => -y.CompareTo(x);
	public static int Compare(MpzT x, ulong y) => x.CompareTo(y);
	public static int Compare(double x, MpzT y) => -y.CompareTo(x);
	public static int Compare(MpzT x, double y) => x.CompareTo(y);
	public static int Compare(decimal x, MpzT y) => -y.CompareTo(x);
	public static int Compare(MpzT x, decimal y) => x.CompareTo(y);
	public static int Compare(MpzT x, MpzT? y) => x.CompareTo(y);
	public static int Compare(MpzT x, object? y) => x.CompareTo(y);
	public static int Compare(object x, MpzT y) => -y.CompareTo(x);
	public static int CompareAbs(int x, MpzT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpzT x, int y) => x.CompareAbsTo(y);
	public static int CompareAbs(uint x, MpzT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpzT x, uint y) => x.CompareAbsTo(y);
	public static int CompareAbs(long x, MpzT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpzT x, long y) => x.CompareAbsTo(y);
	public static int CompareAbs(ulong x, MpzT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpzT x, ulong y) => x.CompareAbsTo(y);
	public static int CompareAbs(double x, MpzT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpzT x, double y) => x.CompareAbsTo(y);
	public static int CompareAbs(decimal x, MpzT y) => -y.CompareAbsTo(x);
	public static int CompareAbs(MpzT x, decimal y) => x.CompareAbsTo(y);
	public static int CompareAbs(MpzT x, MpzT y) => x.CompareAbsTo(y);
	public static int CompareAbs(MpzT x, object y) => x.CompareAbsTo(y);
	public static int CompareAbs(object x, MpzT y) => -y.CompareAbsTo(x);

	public int CompareAbsTo(int other) => Mpir.MpzCmpabsUi(this, (uint)other);
	public int CompareAbsTo(uint other) => Mpir.MpzCmpabsUi(this, other);
	public int CompareAbsTo(long other) => CompareAbsTo((MpzT)other);
	public int CompareAbsTo(ulong other) => CompareAbsTo((MpzT)other);
	public int CompareAbsTo(double other) => Mpir.MpzCmpabsD(this, other);
	public int CompareAbsTo(decimal other) => Mpir.MpzCmpabsD(this, (double)other);
	public int CompareAbsTo(MpzT other) => Mpir.MpzCmpabs(this, other);

	public int CompareAbsTo(object obj) => obj switch
	{
		MpzT z => CompareAbsTo(z),
		MpuT uz => CompareAbsTo(uz),
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
		string s => CompareAbsTo(new MpzT(s)),
		_ => throw new ArgumentException("Cannot compare to " + obj.GetType())
	};

	public int CompareTo(int other) => Mpir.MpzCmpSi(this, other);
	public int CompareTo(uint other) => Mpir.MpzCmpUi(this, other);
	// TODO: Optimize by accessing the memory directly
	public int CompareTo(long other) => CompareTo(new MpzT(other));
	// TODO: Optimize by accessing the memory directly
	public int CompareTo(ulong other) => CompareTo(new MpzT(other));
	public int CompareTo(MpzT? other) => Mpir.MpzCmp(this, other);
	public int CompareTo(float other) => Mpir.MpzCmpD(this, (double)other);
	public int CompareTo(double other) => Mpir.MpzCmpD(this, other);
	public int CompareTo(decimal other) => Mpir.MpzCmpD(this, (double)other);

	public int CompareTo(object? obj) => obj switch
	{
		MpzT z => CompareTo(z),
		MpuT uz => CompareTo(uz),
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
		string s => CompareTo(new MpzT(s)),
		IComparable ic => -ic.CompareTo(this),
		_ => throw new ArgumentException("Cannot compare to " + (obj?.GetType()?.ToString() ?? "null"))
	};

	public MpzT Complement() => ~this;

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public void Dispose(bool disposing)
	{
		if (val == 0 || ReferenceEquals(this, Zero) || ReferenceEquals(this, One))
			return;
		try
		{
			Mpir.MpzClear(this);
		}
		catch (Exception) when (!disposing)
		{
		}
		val = 0;
	}

	public MpzT Divide(int x, out int remainder)
	{
		var quotient = new MpzT();
		if (x >= 0)
		{
			remainder = (int)Mpir.MpzTdivQUi(quotient, this, (uint)x);
			return quotient;
		}
		else
		{
			remainder = -(int)Mpir.MpzTdivQUi(quotient, this, (uint)-x);
			var res = -quotient;
			return res;
		}
	}

	public MpzT Divide(int x, out MpzT remainder)
	{
		var quotient = new MpzT();
		remainder = new MpzT();
		if (x >= 0)
		{
			Mpir.MpzTdivQrUi(quotient, remainder, this, (uint)x);
			return quotient;
		}
		else
		{
			Mpir.MpzTdivQrUi(quotient, remainder, this, (uint)-x);
			var res = -quotient;
			return res;
		}
	}

	public MpzT Divide(MpzT x, out MpzT remainder)
	{
		var quotient = new MpzT();
		remainder = new MpzT();
		Mpir.MpzTdivQr(quotient, remainder, this, x);
		return quotient;
	}

	public MpzT Divide(uint x, out MpzT remainder)
	{
		var quotient = new MpzT();
		remainder = new MpzT();
		Mpir.MpzTdivQrUi(quotient, remainder, this, x);
		return quotient;
	}

	public MpzT Divide(uint x, out uint remainder)
	{
		// Unsure about the below exception for negative numbers. It's in Stefanov's
		// original code, but that limitation isn't mentioned in
		// http://Gmplib.org/manual/Integer-Division.html#Integer-Division.
		//if(this.ChunkCount < 0)
		//	throw new InvalidOperationException("This method may not be called when the instance represents a negative number.");
		var quotient = new MpzT();
		remainder = Mpir.MpzTdivQUi(quotient, this, x);
		return quotient;
	}

	public MpzT Divide(uint x, out int remainder)
	{
		var quotient = new MpzT();
		var uintRemainder = Mpir.MpzTdivQUi(quotient, this, x);
		if (uintRemainder > int.MaxValue)
			throw new OverflowException();
		if (Mpir.MpzCmpSi(this, 0) >= 0)
			remainder = (int)uintRemainder;
		else
			remainder = -(int)uintRemainder;
		return quotient;
	}

	/// <summary>
	/// Divides exactly. Only works when the division is gauranteed to be exact (there is no remainder).
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public MpzT DivideExactly(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzDivexact(z, this, x);
		return z;
	}

	public MpzT DivideExactly(int x)
	{
		var z = new MpzT();
		Mpir.MpzDivexactUi(z, this, (uint)x);
		if (x < 0)
		{
			var res = -z;
			return res;
		}
		else
			return z;
	}

	public MpzT DivideExactly(uint x)
	{
		var z = new MpzT();
		Mpir.MpzDivexactUi(z, this, x);
		return z;
	}

	public MpzT DivideMod(MpzT x, MpzT mod) => this * x.InvertMod(mod) % mod;

	public bool Equals(int other) => CompareTo(other) == 0;
	public bool Equals(uint other) => CompareTo(other) == 0;
	public bool Equals(long other) => CompareTo(other) == 0;
	public bool Equals(ulong other) => CompareTo(other) == 0;
	public bool Equals(MpzT? other) => Compare(this, other) == 0;
	public bool Equals(double other) => CompareTo(other) == 0;
	public bool Equals(decimal other) => CompareTo(other) == 0;

	public override bool Equals(object? obj) => obj switch
	{
		null => false,
		MpzT z => CompareTo(z) == 0,
		MpuT uz => CompareTo(uz) == 0,
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
			return Mpir.MpzCongruentUiP(this, (uint)x, (uint)mod) != 0;
		else
		{
			var xAsUint = (uint)(x % mod + mod);
			return Mpir.MpzCongruentUiP(this, xAsUint, (uint)mod) != 0;
		}
	}

	public bool EqualsMod(MpzT x, MpzT mod) => Mpir.MpzCongruentP(this, x, mod) != 0;
	public bool EqualsMod(uint x, uint mod) => Mpir.MpzCongruentUiP(this, x, mod) != 0;

	public static MpzT Factorial(int x)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(x);
		var z = new MpzT();
		Mpir.MpzFacUi(z, (uint)x);
		return z;
	}

	public static MpzT Factorial(uint x)
	{
		var z = new MpzT();
		Mpir.MpzFacUi(z, x);
		return z;
	}

	public static MpzT Fibonacci(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		Mpir.MpzFibUi(z, (uint)n);
		return z;
	}

	public static MpzT Fibonacci(int n, out MpzT previous)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		previous = new MpzT();
		Mpir.MpzFib2Ui(z, previous, (uint)n);
		return z;
	}

	public static MpzT Fibonacci(uint n)
	{
		var z = new MpzT();
		Mpir.MpzFibUi(z, n);
		return z;
	}

	public static MpzT Fibonacci(uint n, out MpzT previous)
	{
		var z = new MpzT();
		previous = new MpzT();
		Mpir.MpzFib2Ui(z, previous, n);
		return z;
	}

	/// <summary>
	/// Import the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	/// </summary>
	public void FromByteArray(ReadOnlySpan<byte> source, int order)
	{
		Mpir.MpirMpzImport(this, (uint)source.Length, order, sizeof(byte), 0, 0u, source);
		if (source[order == 1 ? 0 : ^1] >= 128)
			Mpir.MpzSub(this, this, One << source.Length * 8);
	}

	public static MpzT Gcd(MpzT x, int y)
	{
		var z = new MpzT();
		if (y >= 0)
			Mpir.MpzGcdUi(z, x, (uint)y);
		else
			Mpir.MpzGcdUi(z, x, (uint)-y);
		return z;
	}

	public static MpzT Gcd(int x, MpzT y)
	{
		var z = new MpzT();
		if (x >= 0)
			Mpir.MpzGcdUi(z, y, (uint)x);
		else
			Mpir.MpzGcdUi(z, y, (uint)-x);
		return z;
	}

	public static MpzT Gcd(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzGcd(z, x, y);
		return z;
	}

	public static MpzT Gcd(MpzT x, MpzT y, out MpzT a)
	{
		var z = new MpzT();
		a = new MpzT();
		Mpir.MpzGcdext(z, a, default!, x, y);
		return z;
	}

	public static MpzT Gcd(MpzT x, MpzT y, out MpzT a, out MpzT b)
	{
		var z = new MpzT();
		a = new MpzT();
		b = new MpzT();
		Mpir.MpzGcdext(z, a, b, x, y);
		return z;
	}

	public static MpzT Gcd(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzGcdUi(z, x, y);
		return z;
	}

	public static MpzT Gcd(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzGcdUi(z, y, x);
		return z;
	}

	public int GetByteCount() => (BitLength + 7) / 8;
	public MpzT GetFullBitLength() => Mpir.MpzSizeinbase(this, 2);

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
	public static int HammingDistance(MpzT x, MpzT y) => (int)Mpir.MpzHamdist(x, y);

	/// <summary>
	/// Import the integer in the byte array bytes, starting at startOffset
	/// and ending at endOffset.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	/// </summary>
	public void ImportByOffset(ReadOnlySpan<byte> source, int startOffset, int endOffset, int order) =>
		Mpir.MpirMpzImportByOffset(this, startOffset, endOffset, order, sizeof(byte), 0, 0u, source);

	public int IndexOfOne(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpzScan1(this, (uint)startingIndex);
		}
	}

	public int IndexOfZero(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpzScan0(this, (uint)startingIndex);
		}
	}

	public bool InverseModExists(MpzT mod)
	{
		TryInvertMod(mod, out _);
		return true;
	}

	public MpzT InvertMod(MpzT mod)
	{
		var z = new MpzT();
		var status = Mpir.MpzInvert(z, this, mod);
		if (status == 0)
			throw new ArithmeticException("This modular inverse does not exists.");
		return z;
	}

	internal static int GetArrayLength(int n, int div) => n > 0 ? ((n - 1) / div + 1) : 0;
	public static bool IsCanonical(MpzT value) => true;
	public static bool IsComplexNumber(MpzT value) => true;

	public bool IsDivisibleBy(int x)
	{
		if (x >= 0)
			return Mpir.MpzDivisibleUiP(this, (uint)x) != 0;
		else
			return Mpir.MpzDivisibleUiP(this, (uint)-x) != 0;
	}

	public bool IsDivisibleBy(MpzT x) => Mpir.MpzDivisibleP(this, x) != 0;
	public bool IsDivisibleBy(uint x) => Mpir.MpzDivisibleUiP(this, x) != 0;
	public static bool IsEvenInteger(MpzT value) => (value & 1) == 0;
	public static bool IsFinite(MpzT value) => true;
	public static bool IsImaginaryNumber(MpzT value) => false;
	public static bool IsInfinity(MpzT value) => false;
	public static bool IsInteger(MpzT value) => true;
	public static bool IsNaN(MpzT value) => false;
	public static bool IsNegative(MpzT value) => Mpir.MpzCmpSi(value, 0) < 0;
	public static bool IsNegativeInfinity(MpzT value) => false;
	public static bool IsNormal(MpzT value) => true;
	public static bool IsOddInteger(MpzT value) => !IsEvenInteger(value);

	public bool IsPerfectPower() =>
		// There is a known issue with this function for negative inputs in GMP 4.2.4.
		// Haven't heard of any issues in MPIR 5.x though.
		Mpir.MpzPerfectPowerP(this) != 0;

	public bool IsPerfectSquare() => Mpir.MpzPerfectSquareP(this) != 0;
	public static bool IsPositive(MpzT value) => Mpir.MpzCmpSi(value, 0) > 0;
	public static bool IsPositiveInfinity(MpzT value) => false;
	public static bool IsPow2(MpzT value) => value.PopCount() == 1;

	public bool IsProbablyPrimeRabinMiller(uint repetitions)
	{
		var result = Mpir.MpzProbabPrimeP(this, repetitions);
		return result != 0;
	}

	public static bool IsRealNumber(MpzT value) => true;
	public static bool IsSubnormal(MpzT value) => false;
	public static bool IsZero(MpzT value) => Mpir.MpzCmpSi(value, 0) == 0;

	public static int JacobiSymbol(int x, MpzT y)
	{
		if (IsEvenInteger(y) || Mpir.MpzCmpSi(y, 0) < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpzSiKronecker(x, y);
	}

	public static int JacobiSymbol(MpzT x, int y)
	{
		if ((y & 1) == 0 || Mpir.MpzCmpSi(y, 0) < 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpzKroneckerSi(x, y);
	}

	public static int JacobiSymbol(MpzT x, MpzT y)
	{
		if(IsEvenInteger(y) || Mpir.MpzCmpSi(y, 0) < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpzJacobi(x, y);
	}

	public static int JacobiSymbol(MpzT x, uint y)
	{
		if ((y & 1) == 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpzKroneckerUi(x, y);
	}

	public static int JacobiSymbol(uint x, MpzT y) {
		if (IsEvenInteger(y) || Mpir.MpzCmpSi(y, 0) < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpzUiKronecker(x, y);
	}

	public static int KroneckerSymbol(int x, MpzT y) => Mpir.MpzSiKronecker(x, y);
	public static int KroneckerSymbol(MpzT x, int y) => Mpir.MpzKroneckerSi(x, y);
	public static int KroneckerSymbol(MpzT x, MpzT y) => Mpir.MpzKronecker(x, y);
	public static int KroneckerSymbol(MpzT x, uint y) => Mpir.MpzKroneckerUi(x, y);
	public static int KroneckerSymbol(uint x, MpzT y) => Mpir.MpzUiKronecker(x, y);

	public static MpzT Lcm(int x, MpzT y)
	{
		var z = new MpzT();
		if (x >= 0)
			Mpir.MpzLcmUi(z, y, (uint)x);
		else
			Mpir.MpzLcmUi(z, y, (uint)-x);
		return z;
	}

	public static MpzT Lcm(MpzT x, int y)
	{
		var z = new MpzT();
		if (y >= 0)
			Mpir.MpzLcmUi(z, x, (uint)y);
		else
			Mpir.MpzLcmUi(z, x, (uint)-y);
		return z;
	}

	public static MpzT Lcm(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzLcm(z, x, y);
		return z;
	}

	public static MpzT Lcm(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzLcmUi(z, x, y);
		return z;
	}

	public static MpzT Lcm(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzLcmUi(z, y, x);
		return z;
	}

	public static int LegendreSymbol(MpzT x, MpzT primeY)
	{
		Debug.Assert(primeY != 2); // Not defined for 2
		return Mpir.MpzJacobi(x, primeY);
	}

	public static MpzT Log2(MpzT value)
	{
		var bitLength = value.BitLength;
		var sqrt = (One << bitLength << bitLength - 1).Sqrt();
		return Mpir.MpzCmp(value, sqrt) >= 0 ? bitLength : bitLength - 1;
	}

	public static MpzT Lucas(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		Mpir.MpzLucnumUi(z, (uint)n);
		return z;
	}

	public static MpzT Lucas(int n, out MpzT previous)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		previous = new MpzT();
		Mpir.MpzLucnum2Ui(z, previous, (uint)n);
		return z;
	}

	public static MpzT Lucas(uint n)
	{
		var z = new MpzT();
		Mpir.MpzLucnumUi(z, n);
		return z;
	}

	public static MpzT Lucas(uint n, out MpzT previous)
	{
		var z = new MpzT();
		previous = new MpzT();
		Mpir.MpzLucnum2Ui(z, previous, n);
		return z;
	}

	public static MpzT Max(MpzT x, MpzT y) => x.CompareTo(y) > 0 ? x : y;
	public static MpzT MaxMagnitude(MpzT x, MpzT y) => Max(x, y);
	public static MpzT MaxMagnitudeNumber(MpzT x, MpzT y) => Max(x, y);
	public static MpzT Min(MpzT x, MpzT y) => x.CompareTo(y) < 0 ? x : y;
	public static MpzT MinMagnitude(MpzT x, MpzT y) => Min(x, y);
	public static MpzT MinMagnitudeNumber(MpzT x, MpzT y) => Min(x, y);

	public int ModAsInt32(int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		return (int)Mpir.MpzFdivUi(this, (uint)mod);
	}

	public uint ModAsUInt32(uint mod) => Mpir.MpzFdivUi(this, mod);
	public MpzT Negate() => -this;

	// TODO: Create a version of this method which takes in a parameter to represent how well tested the prime should be.
	public MpzT NextPrimeGMP()
	{
		var z = new MpzT();
		Mpir.MpzNextprime(z, this);
		return z;
	}

	public static MpzT Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);
	public static MpzT Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		Parse(s.ToString(), style, provider);
	public static MpzT Parse(string s) => new(s);
	public static MpzT Parse(string s, IFormatProvider? provider) => new(s);
	public static MpzT Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);
	public int PopCount() => (int)Mpir.MpzPopcount(this);
	public static MpzT PopCount(MpzT value) => value.PopCount();

	public MpzT Power(int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpzT();
		Mpir.MpzPowUi(z, this, (uint)exponent);
		return z;
	}

	public MpzT Power(uint exponent)
	{
		var z = new MpzT();
		Mpir.MpzPowUi(z, this, exponent);
		return z;
	}

	public static MpzT Power(int x, int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpzT();
		Mpir.MpzUiPowUi(z, (uint)x, (uint)exponent);
		return z;
	}

	public static MpzT Power(uint x, uint exponent)
	{
		var z = new MpzT();
		Mpir.MpzUiPowUi(z, x, exponent);
		return z;
	}

	public MpzT PowerMod(int exponent, MpzT mod)
	{
		var z = new MpzT();
		Mpir.MpzPowm(z, this, exponent, mod);
		return z;
	}

	public MpzT PowerMod(MpzT exponent, MpzT mod)
	{
		var z = new MpzT();
		Mpir.MpzPowm(z, this, exponent, mod);
		return z;
	}

	public MpzT PowerMod(uint exponent, MpzT mod)
	{
		var z = new MpzT();
		if (exponent >= 0)
			Mpir.MpzPowmUi(z, this, exponent, mod);
		else
		{
			MpzT bigExponent = exponent;
			var inverse = bigExponent.InvertMod(mod);
			Mpir.MpzPowmUi(z, inverse, exponent, mod);
		}
		return z;
	}

	private static void ProcessLongConversion(MpzT value)
	{
		if (Mpir.MpzCmpSi(value, 0) < 0)
			value += One << Math.Max(sizeof(long), (value.BitLength + 7) / 8) * 8;
		var exportBytesSpan = ProcessToByteArray(value, BitConverter.IsLittleEndian);
		var length = Math.Min(exportBytesSpan.Length, convertToLongBytes.Length);
		var destOffset = BitConverter.IsLittleEndian ? 0 : 8 - length;
		convertToLongBytes.AsSpan(BitConverter.IsLittleEndian ? length.. : ..destOffset).Clear();
		exportBytesSpan[BitConverter.IsLittleEndian ? 0..length : ^length..].CopyTo(convertToLongBytes.AsSpan(destOffset));
	}

	private static Span<byte> ProcessToByteArray(MpzT value, bool bLittleEndian)
	{
		var exportLength = (int)Math.Min(Mpir.MpzSizeinbase(value, 256), 2147483647);
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

	public MpzT Remainder(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzTdivR(z, this, x);
		return z;
	}

	public MpzT RemoveFactor(MpzT factor)
	{
		var z = new MpzT();
		Mpir.MpzRemove(z, this, factor);
		return z;
	}

	public MpzT RemoveFactor(MpzT factor, out int count)
	{
		var z = new MpzT();
		count = (int)Mpir.MpzRemove(z, this, factor);
		return z;
	}

	public MpzT Root(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		Mpir.MpzRoot(z, this, (uint)n);
		return z;
	}

	public MpzT Root(int n, out bool isExact)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		var result = Mpir.MpzRoot(z, this, (uint)n);
		isExact = result != 0;
		return z;
	}

	public MpzT Root(int n, out MpzT remainder)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		remainder = new MpzT();
		Mpir.MpzRootrem(z, remainder, this, (uint)n);
		return z;
	}

	public MpzT Root(uint n)
	{
		var z = new MpzT();
		Mpir.MpzRoot(z, this, n);
		return z;
	}

	public MpzT Root(uint n, out bool isExact)
	{
		var z = new MpzT();
		var result = Mpir.MpzRoot(z, this, n);
		isExact = result != 0;
		return z;
	}

	public MpzT Root(uint n, out MpzT remainder)
	{
		var z = new MpzT();
		remainder = new MpzT();
		Mpir.MpzRootrem(z, remainder, this, n);
		return z;
	}

	public MpzT ShiftRightRound(int shiftAmount)
	{
		if (shiftAmount <= 0)
			return new(this);
		if (Mpir.MpzCmpSi(this, 0) < 0)
			return -(-this).ShiftRightRound(shiftAmount);
		var result = this >> shiftAmount;
		if (shiftAmount <= 32)
		{
			if ((this & uint.MaxValue >>> sizeof(uint) * 8 - shiftAmount) >= 1u << shiftAmount - 1)
				result++.Dispose();
		}
		else
		{
			using var left = One << shiftAmount;
			Mpir.MpzSubUi(left, left, 1);
			Mpir.MpzAnd(left, left, this);
			using var right = One << shiftAmount - 1;
			if (Mpir.MpzCmp(left, right) >= 0)
				result++.Dispose();
		}
		return result;
	}

	public MpzT Sqrt()
	{
		var z = new MpzT();
		Mpir.MpzSqrt(z, this);
		return z;
	}

	public MpzT Sqrt(out bool isExact)
	{
		var z = new MpzT();
		var result = Mpir.MpzRoot(z, this, 2);
		isExact = result != 0;
		return z;
	}

	public MpzT Sqrt(out MpzT remainder)
	{
		var z = new MpzT();
		remainder = new MpzT();
		Mpir.MpzSqrtrem(z, remainder, this);
		return z;
	}

	public MpzT Square() => this * this;
	public BigInteger ToBigInteger() => new(ToByteArray(-1));
	bool IConvertible.ToBoolean(IFormatProvider? provider) => Mpir.MpzCmpSi(this, 1) >= 0;
	byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

	/// <summary>
	/// Export to the value to a byte array.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	/// </summary>
	public byte[] ToByteArray(int order) => val == 0 ? [] : Mpir.MpirMpzExport(order, sizeof(byte), 0, 0u, this);

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
	public string? ToString(uint @base) => val == 0 ? "0" : Mpir.MpzGetString(@base, this);
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString());
	string IConvertible.ToString(IFormatProvider? provider) => ToString() ?? "";

	object IConvertible.ToType(Type targetType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(targetType);
		if (targetType == typeof(MpzT))
			return this;
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
			return value;
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(MpzT) + ", " + nameof(MpuT)
				+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;
	uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	public static MpzT TrailingZeroCount(MpzT value)
	{
		if (value == Zero)
			return Zero;
		if (Mpir.MpzCmpSi(value, 0) < 0)
			value += One << value.BitLength;
		var result = 0;
		const int ulongBits = sizeof(ulong) * 8;
		var value2 = value << ulongBits;
		MpzT mask = ulong.MaxValue;
		for (; Mpir.MpzCmp(mask, value2) < 0; mask <<= ulongBits)
		{
			var maskedValue = value & mask;
			if (Mpir.MpzCmpSi(maskedValue, 0) == 0)
				result += ulongBits;
			else
				return result + (int)ulong.TrailingZeroCount((ulong)(maskedValue >> result));
		}
		throw new InvalidOperationException("Невозможно добавить элемент. Возможные причины:\r\n" + InternalError
			+ $"Текущее состояние: длина - {value.BitLength}, значение - {value}"
			+ $" ThreadId={Environment.CurrentManagedThreadId}, Timestamp={DateTime.UtcNow}");
	}

	private static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out MpzT result)
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
				float f => (MpzT)f,
				double d => (MpzT)d,
				decimal m => (MpzT)(double)m,
				BigInteger ll => new(ll),
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

	static bool INumberBase<MpzT>.TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out MpzT result) =>
		TryConvertFromChecked(value, out result);
	static bool INumberBase<MpzT>.TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out MpzT result)
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
				float f => (MpzT)MathF.Ceiling(MathF.Abs(f)) * MathF.Sign(f),
				double d => (MpzT)Math.Ceiling(Math.Abs(d)) * Math.Sign(d),
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

	static bool INumberBase<MpzT>.TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out MpzT result) =>
		TryConvertFromChecked(value, out result);

	private static bool TryConvertToChecked<TOther>(MpzT value, out TOther result)
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

	static bool INumberBase<MpzT>.TryConvertToChecked<TOther>(MpzT value, out TOther result) =>
		TryConvertToChecked(value, out result);
	static bool INumberBase<MpzT>.TryConvertToSaturating<TOther>(MpzT value, out TOther result) =>
		TryConvertToChecked(value, out result);
	static bool INumberBase<MpzT>.TryConvertToTruncating<TOther>(MpzT value, out TOther result) =>
		TryConvertToChecked(value, out result);

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

	public bool TryInvertMod(MpzT mod, [MaybeNullWhen(false)] out MpzT result)
	{
		var z = new MpzT();
		var status = Mpir.MpzInvert(z, this, mod);
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

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out MpzT result) => TryParse(s.ToString(), out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out MpzT result) =>
		TryParse(s.ToString(), out result);
	public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out MpzT result)
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
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out MpzT result) => TryParse(s, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out MpzT result) => TryParse(s, out result);

	public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out MpzT value)
	{
		value = new(source, 1);
		if (!isUnsigned && value.BitLength == source.Length * 8)
			value -= One << value.BitLength;
		return true;
	}

	public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out MpzT value)
	{
		value = new(source, -1);
		if (!isUnsigned && value.BitLength == source.Length * 8)
			value -= One << value.BitLength;
		return true;
	}

	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
	{
		var bufSize = (int)Math.Min(Mpir.MpzSizeinbase(this, 256), 2147483647);
		if (destination.Length >= bufSize)
		{
			Mpir.MpirMpzExport(destination[^bufSize..], 1, sizeof(byte), 0, 0u, this);
			bytesWritten = bufSize;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		var bufSize = (int)Math.Min(Mpir.MpzSizeinbase(this, 256), 2147483647);
		if (destination.Length >= bufSize)
		{
			Mpir.MpirMpzExport(destination, -1, sizeof(byte), 0, 0u, this);
			bytesWritten = bufSize;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public static implicit operator MpzT(byte value) => new((uint)value);
	public static implicit operator MpzT(int value) => new(value);
	public static implicit operator MpzT(uint value) => new(value);
	public static implicit operator MpzT(short value) => new(value);
	public static implicit operator MpzT(ushort value) => new(value);
	public static implicit operator MpzT(long value) => new(value);
	public static implicit operator MpzT(ulong value) => new(value);
	public static implicit operator MpzT(MpuT value) => new(value);
	public static explicit operator MpzT(float value) => new((double)value);
	public static explicit operator MpzT(double value) => new(value);
	public static explicit operator MpzT(decimal value) => new(value);
	public static explicit operator MpzT(string value) => new(value, DefaultStringBase);
	public static explicit operator byte(MpzT value) => (byte)(uint)value;
	public static explicit operator int(MpzT value) => Mpir.MpzGetSi(value);

	public static explicit operator uint(MpzT value)
	{
		var result = Mpir.MpzGetUi(value);
		if (Mpir.MpzCmpSi(value, 0) < 0)
			result = ~result + 1;
		return result;
	}

	public static explicit operator short(MpzT value) => (short)(int)value;
	public static explicit operator ushort(MpzT value) => (ushort)(uint)value;

	public static explicit operator long(MpzT value)
	{
		lock (lockObj)
		{
			ProcessLongConversion(value);
			return BitConverter.ToInt64(convertToLongBytes, 0);
		}
	}

	public static explicit operator ulong(MpzT value)
	{
		lock (lockObj)
		{
			ProcessLongConversion(value);
			return BitConverter.ToUInt64(convertToLongBytes, 0);
		}
	}

	public static explicit operator BigInteger(MpzT value) => new(value.ToByteArray(-1));
	public static explicit operator float(MpzT value) => (float)(double)value;
	public static explicit operator double(MpzT value) => Mpir.MpzGetD(value);
	public static explicit operator decimal(MpzT value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);
	public static explicit operator string?(MpzT value) => value.ToString();

	public static MpzT operator +(MpzT value) => new(value);

	public static MpzT operator -(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzNeg(z, x);
		return z;
	}

	public static MpzT operator ~(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzCom(z, x);
		return z;
	}

	public static MpzT operator +(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzAdd(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator +(MpzT, MpzT)"/>
	public static MpzT operator +(MpzT x, int y)
	{
		var z = new MpzT();
		if (y >= 0)
			Mpir.MpzAddUi(z, x, (uint)y);
		else
			Mpir.MpzSubUi(z, x, (uint)-y);

		return z;
	}

	/// <inheritdoc cref="operator +(MpzT, MpzT)"/>
	public static MpzT operator +(int x, MpzT y)
	{
		var z = new MpzT();
		if (x >= 0)
			Mpir.MpzAddUi(z, y, (uint)x);
		else
			Mpir.MpzSubUi(z, y, (uint)-x);

		return z;
	}

	/// <inheritdoc cref="operator +(MpzT, MpzT)"/>
	public static MpzT operator +(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzAddUi(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator +(MpzT, MpzT)"/>
	public static MpzT operator +(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzAddUi(z, y, x);
		return z;
	}

	public static MpzT operator -(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzSub(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator -(MpzT, MpzT)"/>
	public static MpzT operator -(int x, MpzT y)
	{
		if (x >= 0)
		{
			var z = new MpzT();
			Mpir.MpzUiSub(z, (uint)x, y);
			return z;
		}
		else
		{
			var z = new MpzT();
			Mpir.MpzAddUi(z, y, (uint)-x);
			var z1 = -z;
			return z1;
		}
	}

	/// <inheritdoc cref="operator -(MpzT, MpzT)"/>
	public static MpzT operator -(MpzT x, int y)
	{
		var z = new MpzT();
		if (y >= 0)
			Mpir.MpzSubUi(z, x, (uint)y);
		else
			Mpir.MpzAddUi(z, x, (uint)-y);
		return z;
	}

	/// <inheritdoc cref="operator -(MpzT, MpzT)"/>
	public static MpzT operator -(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzUiSub(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator -(MpzT, MpzT)"/>
	public static MpzT operator -(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzSubUi(z, x, y);
		return z;
	}

	public static MpzT operator *(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzMul(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator *(MpzT, MpzT)"/>
	public static MpzT operator *(int x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzMulSi(z, y, x);
		return z;
	}

	/// <inheritdoc cref="operator *(MpzT, MpzT)"/>
	public static MpzT operator *(MpzT x, int y)
	{
		var z = new MpzT();
		Mpir.MpzMulSi(z, x, y);
		return z;
	}

	/// <inheritdoc cref="operator *(MpzT, MpzT)"/>
	public static MpzT operator *(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzMulUi(z, y, x);
		return z;
	}

	/// <inheritdoc cref="operator *(MpzT, MpzT)"/>
	public static MpzT operator *(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzMulUi(z, x, y);
		return z;
	}

	public static MpzT operator /(MpzT x, MpzT y)
	{
		var quotient = new MpzT();
		Mpir.MpzTdivQ(quotient, x, y);
		return quotient;
	}

	/// <inheritdoc cref="operator /(MpzT, MpzT)"/>
	public static MpzT operator /(MpzT x, int y)
	{
		if (y >= 0)
		{
			var quotient = new MpzT();
			Mpir.MpzTdivQUi(quotient, x, (uint)y);
			return quotient;
		}
		else
		{
			var quotient = new MpzT();
			Mpir.MpzTdivQUi(quotient, x, (uint)-y);
			var negQ = -quotient;
			return negQ;
		}
	}

	/// <inheritdoc cref="operator /(MpzT, MpzT)"/>
	public static MpzT operator /(MpzT x, uint y)
	{
		var quotient = new MpzT();
		Mpir.MpzTdivQUi(quotient, x, y);
		return quotient;
	}

	public static MpzT operator %(MpzT x, MpzT mod)
	{
		var z = new MpzT();
		Mpir.MpzMod(z, x, mod);
		return z;
	}

	/// <inheritdoc cref="operator %(MpzT, MpzT)"/>
	public static MpzT operator %(MpzT x, int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		var z = new MpzT();
		Mpir.MpzFdivRUi(z, x, (uint)mod);
		return z;
	}

	/// <inheritdoc cref="operator %(MpzT, MpzT)"/>
	public static MpzT operator %(MpzT x, uint mod)
	{
		var z = new MpzT();
		Mpir.MpzFdivRUi(z, x, mod);
		return z;
	}

	/// <inheritdoc cref="operator &(MpzT, MpzT)"/>
	public static int operator &(MpzT x, int y) => Mpir.MpzGetSi(x) & y;
	/// <inheritdoc cref="operator &(MpzT, MpzT)"/>
	public static uint operator &(MpzT x, uint y) => Mpir.MpzGetUi(x) & y;

	public static MpzT operator &(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzAnd(z, x, y);
		return z;
	}

	public static MpzT operator |(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzIor(z, x, y);
		return z;
	}

	public static MpzT operator ^(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzXor(z, x, y);
		return z;
	}

	public static MpzT operator <<(MpzT x, int shiftAmount)
	{
		var z = new MpzT();
		Mpir.MpzMul2exp(z, x, (uint)shiftAmount);
		return z;
	}

	public static MpzT operator >>(MpzT x, int shiftAmount)
	{
		var z = new MpzT();
		Mpir.MpzTdivQ2exp(z, x, (uint)shiftAmount);
		return z;
	}

	public static MpzT operator >>>(MpzT x, int shiftAmount)
	{
		if (Mpir.MpzCmpSi(x, 0) >= 0)
			return x >> shiftAmount;
		return ~((~x) >> shiftAmount);
	}

	public static MpzT operator ++(MpzT x)
	{
		if (ReferenceEquals(x, Zero) || ReferenceEquals(x, One))
		{
			var z = new MpzT();
			Mpir.MpzAddUi(z, x, 1);
			return z;
		}
		else
		{
			Mpir.MpzAddUi(x, x, 1);
			return x;
		}
	}

	public static MpzT operator --(MpzT x)
	{
		if (ReferenceEquals(x, Zero) || ReferenceEquals(x, One))
		{
			var z = new MpzT();
			Mpir.MpzSubUi(z, x, 1);
			return z;
		}
		else
		{
			Mpir.MpzSubUi(x, x, 1);
			return x;
		}
	}

	public static bool operator ==(MpzT? x, MpzT? y) => (x ?? Zero).CompareTo(y) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(int x, MpzT y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, int y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(uint x, MpzT y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, uint y) => x.CompareTo(y) == 0;
	// TODO: Optimize this by accessing memory directly.
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(long x, MpzT y) => y.CompareTo(x) == 0;
	// TODO: Optimize this by accessing memory directly.
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, long y) => x.CompareTo(y) == 0;
	// TODO: Optimize this by accessing memory directly.
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(ulong x, MpzT y) => y.CompareTo(x) == 0;
	// TODO: Optimize this by accessing memory directly.
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, ulong y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(float x, MpzT y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, float y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(double x, MpzT y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, double y) => x.CompareTo(y) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(decimal x, MpzT y) => y.CompareTo(x) == 0;
	/// <inheritdoc cref="operator ==(MpzT, MpzT)"/>
	public static bool operator ==(MpzT x, decimal y) => x.CompareTo(y) == 0;
	public static bool operator !=(MpzT? x, MpzT? y) => (x ?? Zero).CompareTo(y) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(int x, MpzT y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, int y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(uint x, MpzT y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, uint y) => x.CompareTo(y) != 0;
	// TODO: Optimize this by accessing memory directly
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(long x, MpzT y) => y.CompareTo((MpzT)x) != 0;
	// TODO: Optimize this by accessing memory directly
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, long y) => x.CompareTo((MpzT)y) != 0;
	// TODO: Optimize this by accessing memory directly
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(ulong x, MpzT y) => y.CompareTo((MpzT)x) != 0;
	// TODO: Optimize this by accessing memory directly
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, ulong y) => x.CompareTo((MpzT)y) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(float x, MpzT y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, float y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(double x, MpzT y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, double y) => x.CompareTo(y) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(decimal x, MpzT y) => y.CompareTo(x) != 0;
	/// <inheritdoc cref="operator !=(MpzT, MpzT)"/>
	public static bool operator !=(MpzT x, decimal y) => x.CompareTo(y) != 0;
	public static bool operator <(MpzT x, MpzT y) => x.CompareTo(y) < 0;
	public static bool operator <(int x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, int y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(uint x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, uint y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(long x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, long y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(ulong x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, ulong y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(float x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, float y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(double x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, double y) => x.CompareTo(y) < 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(decimal x, MpzT y) => y.CompareTo(x) > 0;
	/// <inheritdoc cref="operator {(MpzT, MpzT)"/>
	public static bool operator <(MpzT x, decimal y) => x.CompareTo(y) < 0;
	public static bool operator <=(MpzT x, MpzT y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(int x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, int y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(uint x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, uint y) => x.CompareTo(y) <= 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(long x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, long y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(ulong x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, ulong y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(float x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, float y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(double x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, double y) => x.CompareTo(y) <= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(decimal x, MpzT y) => y.CompareTo(x) >= 0;
	/// <inheritdoc cref="operator {=(MpzT, MpzT)"/>
	public static bool operator <=(MpzT x, decimal y) => x.CompareTo(y) <= 0;
	public static bool operator >(MpzT x, MpzT y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(int x, MpzT y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, int y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(uint x, MpzT y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, uint y) => x.CompareTo(y) > 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(long x, MpzT y) => y.CompareTo(x) < 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, long y) => x.CompareTo(y) > 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(ulong x, MpzT y) => y.CompareTo(x) < 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, ulong y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(float x, MpzT y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, float y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(double x, MpzT y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, double y) => x.CompareTo(y) > 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(decimal x, MpzT y) => y.CompareTo(x) < 0;
	/// <inheritdoc cref="operator }(MpzT, MpzT)"/>
	public static bool operator >(MpzT x, decimal y) => x.CompareTo(y) > 0;
	public static bool operator >=(MpzT x, MpzT y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(int x, MpzT y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, int y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(uint x, MpzT y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, uint y) => x.CompareTo(y) >= 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(long x, MpzT y) => y.CompareTo(x) <= 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, long y) => x.CompareTo(y) >= 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(ulong x, MpzT y) => y.CompareTo(x) <= 0;
	// TODO: Implement by accessing the data directly
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, ulong y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(float x, MpzT y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, float y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(double x, MpzT y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, double y) => x.CompareTo(y) >= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(decimal x, MpzT y) => y.CompareTo(x) <= 0;
	/// <inheritdoc cref="operator }=(MpzT, MpzT)"/>
	public static bool operator >=(MpzT x, decimal y) => x.CompareTo(y) >= 0;
}
