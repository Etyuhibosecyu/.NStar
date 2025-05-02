using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

// Disable warning about missing XML comments.

namespace Mpir.NET;

public struct MpzT : ICloneable, IConvertible, IComparable, INumber<MpzT>
{
	#region Data
	private const uint sDefaultStringBase = 10u;

	public nint val;
	#endregion

	#region Creation and destruction

	/// Initializes a new MpzT to 0.
	public MpzT() => val = Mpir.MpzInit();
	/// Initializes a new MpzT to the same value as op.
	public MpzT(MpzT op) => val = Mpir.MpzInitSet(op);
	/// Initializes a new MpzT to the unsigned int op.
	public MpzT(uint op) => val = Mpir.MpzInitSetUi(op);
	/// Initializes a new MpzT to the int op.
	public MpzT(int op) => val = Mpir.MpzInitSetSi(op);
	/// Initializes a new MpzT to the double op.
	public MpzT(double op) => val = Mpir.MpzInitSetD(op);
	/// Initializes a new MpzT to string s, parsed as an integer in the specified base.
	public MpzT(string? s, uint Base) => val = Mpir.MpzInitSetStr(s ?? "0", Base);
	/// Initializes a new MpzT to string s, parsed as an integer in base 10.
	public MpzT(string? s) : this(s, sDefaultStringBase) { }
	/// Initializes a new MpzT to the BigInteger op.
	public MpzT(BigInteger op) : this(op.ToByteArray(), -1) { }

	/// Initializes a new MpzT to using MPIR MpzInit2. Only use if you need to
	/// avoid reallocations.
	// 
	// Initialization with MpzInit2 should not be confused with MpzT construction
	// from a ulong. Thus, so we use a static construction function instead, and add
	// the dummy type init2Type to enable us to write a ctor with a unique signature.
	public static MpzT Init2(ulong n) => new(Init2Type.init2, n);
	private enum Init2Type { init2 }
	private MpzT(Init2Type _, ulong n) => val = Mpir.MpzInit2(n);

	/// Initializes a new MpzT to the long op.
	public MpzT(long op)
			: this()
	{
		var bytes = BitConverter.GetBytes(op);
		FromByteArray(bytes, BitConverter.IsLittleEndian ? -1 : 1);
	}

	/// Initializes a new MpzT to the unsigned long op.
	public MpzT(ulong op)
		: this()
	{
		var bytes = BitConverter.GetBytes(op);
		FromByteArray(bytes, BitConverter.IsLittleEndian ? -1 : 1);
	}

	/// Initializes a new MpzT to the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1 
	/// for little endian.
	public MpzT(byte[] bytes, int order) : this() => FromByteArray(bytes, order);

	#endregion

	#region Import and export byte array

	/// Import the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1 
	/// for little endian.
	public readonly void FromByteArray(byte[] source, int order)
	{
		Mpir.MpirMpzImport(this, (uint)source.Length, order, sizeof(byte), 0, 0u, source);
		if (source[order == 1 ? 0 : ^1] >= 128)
			Mpir.MpzSet(this, Subtract((MpzT)1 << source.Length * 8));
	}

	/// Import the integer in the byte array bytes, starting at startOffset
	/// and ending at endOffset.
	/// Endianess is specified by order, which is 1 for big endian or -1 
	/// for little endian.
	public readonly void ImportByOffset(byte[] source, int startOffset, int endOffset, int order) => Mpir.MpirMpzImportByOffset(this, startOffset, endOffset, order, sizeof(byte), 0, 0u, source);

	/// Export to the value to a byte array.
	/// Endianess is specified by order, which is 1 for big endian or -1 
	/// for little endian.
	public readonly byte[] ToByteArray(int order) => val == 0 ? [] : Mpir.MpirMpzExport(order, sizeof(byte), 0, 0u, this);
	#endregion

	// Almost everything below is copied from Emil Stefanov's BigInt 
	// http://www.emilstefanov.net/Projects/GnuMpDotNet/
	// with a few minor adjustments, e.g. datatype used 
	// and ++ and -- operators which now do in-place inrements/decrements).
	// All code handling Decimal is commented out, dute to some
	// unexpected behaviour.

	// TODO: Dispose temp MpzT objects that are created by casts in operators.

	#region Predefined Values

	public static MpzT NegativeTen => new(-10);
	public static MpzT NegativeThree => new(-3);
	public static MpzT NegativeTwo => new(-2);
	public static MpzT NegativeOne => new(-1);
	public static MpzT Zero => new(0);
	public static MpzT One => new(1);
	public static MpzT Two => new(2);
	public static MpzT Three => new(3);
	public static MpzT Ten => new(10);
	public static MpzT AdditiveIdentity => Zero;
	public static MpzT MultiplicativeIdentity => One;
	public static int Radix => 2;

	#endregion

	#region Operators

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

	public static MpzT operator +(MpzT x, int y)
	{
		var z = new MpzT();
		if (y >= 0)
			Mpir.MpzAddUi(z, x, (uint)y);
		else
			Mpir.MpzSubUi(z, x, (uint)-y);

		return z;
	}

	public static MpzT operator +(int x, MpzT y)
	{
		var z = new MpzT();
		if (x >= 0)
			Mpir.MpzAddUi(z, y, (uint)x);
		else
			Mpir.MpzSubUi(z, y, (uint)-x);

		return z;
	}

	public static MpzT operator +(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzAddUi(z, x, y);
		return z;
	}

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

	public static MpzT operator -(MpzT x, int y)
	{
		var z = new MpzT();
		if (y >= 0)
			Mpir.MpzSubUi(z, x, (uint)y);
		else
			Mpir.MpzAddUi(z, x, (uint)-y);

		return z;
	}

	public static MpzT operator -(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzUiSub(z, x, y);
		return z;
	}

	public static MpzT operator -(MpzT x, uint y)
	{
		var z = new MpzT();
		Mpir.MpzSubUi(z, x, y);
		return z;
	}

	public static MpzT operator ++(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzAddUi(z, x, 1);
		Mpir.MpzSet(x, z);
		return x;
	}

	public static MpzT operator --(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzSubUi(z, x, 1);
		Mpir.MpzSet(x, z);
		return x;
	}

	public static MpzT operator *(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzMul(z, x, y);
		return z;
	}

	public static MpzT operator *(int x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzMulSi(z, y, x);
		return z;
	}

	public static MpzT operator *(MpzT x, int y)
	{
		var z = new MpzT();
		Mpir.MpzMulSi(z, x, y);
		return z;
	}

	public static MpzT operator *(uint x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzMulUi(z, y, x);
		return z;
	}

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

	public static MpzT operator /(MpzT x, uint y)
	{
		var quotient = new MpzT();
		Mpir.MpzTdivQUi(quotient, x, y);
		return quotient;
	}

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

	public static MpzT operator %(MpzT x, MpzT mod)
	{
		var z = new MpzT();
		Mpir.MpzMod(z, x, mod);
		return z;
	}

	public static MpzT operator %(MpzT x, int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		var z = new MpzT();
		Mpir.MpzFdivRUi(z, x, (uint)mod);
		return z;
	}

	public static MpzT operator %(MpzT x, uint mod)
	{
		var z = new MpzT();
		Mpir.MpzFdivRUi(z, x, mod);
		return z;
	}

	public static bool operator <(MpzT x, MpzT y) => x.CompareTo(y) < 0;

	public static bool operator <(int x, MpzT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpzT x, int y) => x.CompareTo(y) < 0;

	public static bool operator <(uint x, MpzT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpzT x, uint y) => x.CompareTo(y) < 0;

	public static bool operator <(long x, MpzT y) => (MpzT)x < y;

	public static bool operator <(MpzT x, long y) => x < (MpzT)y;

	public static bool operator <(ulong x, MpzT y) => (MpzT)x < y;

	public static bool operator <(MpzT x, ulong y) => x < (MpzT)y;

	public static bool operator <(float x, MpzT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpzT x, float y) => x.CompareTo(y) < 0;

	public static bool operator <(double x, MpzT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpzT x, double y) => x.CompareTo(y) < 0;

	//public static bool operator <(decimal x, MpzT y)
	//{
	//    return y.CompareTo(x) > 0;
	//}

	//public static bool operator <(MpzT x, decimal y)
	//{
	//    return x.CompareTo(y) < 0;
	//}

	public static bool operator <=(MpzT x, MpzT y) => x.CompareTo(y) <= 0;

	public static bool operator <=(int x, MpzT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpzT x, int y) => x.CompareTo(y) <= 0;

	public static bool operator <=(uint x, MpzT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpzT x, uint y) => x.CompareTo(y) <= 0;

	// TODO: Implement by accessing the data directly
	public static bool operator <=(long x, MpzT y) => (MpzT)x <= y;

	public static bool operator <=(MpzT x, long y) => x <= (MpzT)y;

	public static bool operator <=(ulong x, MpzT y) => (MpzT)x <= y;

	public static bool operator <=(MpzT x, ulong y) => x <= (MpzT)y;

	public static bool operator <=(float x, MpzT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpzT x, float y) => x.CompareTo(y) <= 0;

	public static bool operator <=(double x, MpzT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpzT x, double y) => x.CompareTo(y) <= 0;

	//public static bool operator <=(decimal x, MpzT y)
	//{
	//    return y.CompareTo(x) >= 0;
	//}

	//public static bool operator <=(MpzT x, decimal y)
	//{
	//    return x.CompareTo(y) <= 0;
	//}

	public static bool operator >(MpzT x, MpzT y) => x.CompareTo(y) > 0;

	public static bool operator >(int x, MpzT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpzT x, int y) => x.CompareTo(y) > 0;

	public static bool operator >(uint x, MpzT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpzT x, uint y) => x.CompareTo(y) > 0;

	// TODO: Implement by accessing the data directly
	public static bool operator >(long x, MpzT y) => (MpzT)x > y;

	// TODO: Implement by accessing the data directly
	public static bool operator >(MpzT x, long y) => x > (MpzT)y;

	// TODO: Implement by accessing the data directly
	public static bool operator >(ulong x, MpzT y) => (MpzT)x > y;

	// TODO: Implement by accessing the data directly
	public static bool operator >(MpzT x, ulong y) => x > (MpzT)y;

	public static bool operator >(float x, MpzT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpzT x, float y) => x.CompareTo(y) > 0;

	public static bool operator >(double x, MpzT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpzT x, double y) => x.CompareTo(y) > 0;

	//public static bool operator >(decimal x, MpzT y)
	//{
	//    return y.CompareTo(x) < 0;
	//}

	//public static bool operator >(MpzT x, decimal y)
	//{
	//    return x.CompareTo(y) > 0;
	//}

	public static bool operator >=(MpzT x, MpzT y) => x.CompareTo(y) >= 0;

	public static bool operator >=(int x, MpzT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpzT x, int y) => x.CompareTo(y) >= 0;

	public static bool operator >=(uint x, MpzT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpzT x, uint y) => x.CompareTo(y) >= 0;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(long x, MpzT y) => (MpzT)x >= y;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(MpzT x, long y) => x >= (MpzT)y;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(ulong x, MpzT y) => (MpzT)x >= y;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(MpzT x, ulong y) => x >= (MpzT)y;

	public static bool operator >=(float x, MpzT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpzT x, float y) => x.CompareTo(y) >= 0;

	public static bool operator >=(double x, MpzT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpzT x, double y) => x.CompareTo(y) >= 0;

	//public static bool operator >=(decimal x, MpzT y)
	//{
	//    return y.CompareTo(x) <= 0;
	//}

	//public static bool operator >=(MpzT x, decimal y)
	//{
	//    return x.CompareTo(y) >= 0;
	//}

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

	public readonly int this[int bitIndex] => Mpir.MpzTstbit(this, (uint)bitIndex);

	public readonly MpzT ChangeBit(int bitIndex, int value)
	{
		var z = new MpzT(this);
		if (value == 0)
			Mpir.MpzClrbit(z, (uint)bitIndex);
		else
			Mpir.MpzSetbit(z, (uint)bitIndex);
		return z;
	}

	#endregion

	#region Basic Arithmetic

	/// Returns a new MpzT which is the absolute value of this value.
	public readonly MpzT Abs()
	{
		var result = new MpzT();
		Mpir.MpzAbs(result, this);
		return result;
	}

	public readonly MpzT Negate() => -this;

	public readonly MpzT Complement() => ~this;

	public readonly MpzT Add(MpzT x) => this + x;

	public readonly MpzT Add(int x) => this + x;

	public readonly MpzT Add(uint x) => this + x;

	public readonly MpzT Subtract(MpzT x) => this - x;

	public readonly MpzT Subtract(int x) => this - x;

	public readonly MpzT Subtract(uint x) => this - x;

	public readonly MpzT Multiply(MpzT x) => this * x;

	public readonly MpzT Multiply(int x) => this * x;

	public readonly MpzT Multiply(uint x) => this * x;

	public readonly MpzT Square() => this * this;

	public readonly MpzT Divide(MpzT x) => this / x;

	public readonly MpzT Divide(int x) => this / x;

	public readonly MpzT Divide(uint x) => this / x;

	public readonly MpzT Divide(MpzT x, out MpzT remainder)
	{
		var quotient = new MpzT();
		remainder = new MpzT();
		Mpir.MpzTdivQr(quotient, remainder, this, x);
		return quotient;
	}

	public readonly MpzT Divide(int x, out MpzT remainder)
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

	public readonly MpzT Divide(int x, out int remainder)
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

	public readonly MpzT Divide(uint x, out MpzT remainder)
	{
		var quotient = new MpzT();
		remainder = new MpzT();
		Mpir.MpzTdivQrUi(quotient, remainder, this, x);
		return quotient;
	}

	public readonly MpzT Divide(uint x, out uint remainder)
	{
		// Unsure about the below exception for negative numbers. It's in Stefanov's 
		// original code, but that limitation isn't mentioned in 
		// http://Gmplib.org/manual/Integer-Division.html#Integer-Division.
		//if(this.ChunkCount < 0)
		//    throw new InvalidOperationException("This method may not be called when the instance represents a negative number.");
		var quotient = new MpzT();
		remainder = Mpir.MpzTdivQUi(quotient, this, x);
		return quotient;
	}

	public readonly MpzT Divide(uint x, out int remainder)
	{
		var quotient = new MpzT();
		var uintRemainder = Mpir.MpzTdivQUi(quotient, this, x);
		if (uintRemainder > int.MaxValue)
			throw new OverflowException();
		if (this >= 0)
			remainder = (int)uintRemainder;
		else
			remainder = -(int)uintRemainder;
		return quotient;
	}

	public readonly MpzT Remainder(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzTdivR(z, this, x);
		return z;
	}

	public readonly bool IsDivisibleBy(MpzT x) => Mpir.MpzDivisibleP(this, x) != 0;

	public readonly bool IsDivisibleBy(int x)
	{
		if (x >= 0)
			return Mpir.MpzDivisibleUiP(this, (uint)x) != 0;
		else
			return Mpir.MpzDivisibleUiP(this, (uint)-x) != 0;
	}

	public readonly bool IsDivisibleBy(uint x) => Mpir.MpzDivisibleUiP(this, x) != 0;

	/// <summary>
	/// Divides exactly. Only works when the division is gauranteed to be exact (there is no remainder).
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public readonly MpzT DivideExactly(MpzT x)
	{
		var z = new MpzT();
		Mpir.MpzDivexact(z, this, x);
		return z;
	}

	public readonly MpzT DivideExactly(int x)
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

	public readonly MpzT DivideExactly(uint x)
	{
		var z = new MpzT();
		Mpir.MpzDivexactUi(z, this, x);
		return z;
	}

	public readonly MpzT DivideMod(MpzT x, MpzT mod) => this * x.InvertMod(mod) % mod;

	public readonly MpzT And(MpzT x) => this & x;

	public readonly MpzT Or(MpzT x) => this | x;

	public readonly MpzT Xor(MpzT x) => this ^ x;

	public readonly MpzT Mod(MpzT mod) => this % mod;

	public readonly MpzT Mod(int mod) => this % mod;

	public readonly MpzT Mod(uint mod) => this % mod;

	public readonly int ModAsInt32(int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		return (int)Mpir.MpzFdivUi(this, (uint)mod);
	}

	public readonly uint ModAsUInt32(uint mod) => Mpir.MpzFdivUi(this, mod);

	public readonly MpzT ShiftLeft(int shiftAmount) => this << shiftAmount;

	public readonly MpzT ShiftRight(int shiftAmount) => this >> shiftAmount;

	public readonly MpzT PowerMod(MpzT exponent, MpzT mod)
	{
		var z = new MpzT();
		Mpir.MpzPowm(z, this, exponent, mod);
		return z;
	}

	public readonly MpzT PowerMod(int exponent, MpzT mod)
	{
		var z = new MpzT();
		Mpir.MpzPowm(z, this, exponent, mod);
		return z;
	}

	public readonly MpzT PowerMod(uint exponent, MpzT mod)
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

	public readonly MpzT Power(int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpzT();
		Mpir.MpzPowUi(z, this, (uint)exponent);
		return z;
	}

	public readonly MpzT Power(uint exponent)
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

	public readonly MpzT InvertMod(MpzT mod)
	{
		var z = new MpzT();
		var status = Mpir.MpzInvert(z, this, mod);
		if (status == 0)
			throw new ArithmeticException("This modular inverse does not exists.");
		return z;
	}

	public readonly bool TryInvertMod(MpzT mod, out MpzT result)
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

	public readonly bool InverseModExists(MpzT mod)
	{
		TryInvertMod(mod, out _);
		return true;
	}

	public readonly int BitLength => (int)Mpir.MpzSizeinbase(this, 2);

	public readonly MpzT GetFullBitLength() => Mpir.MpzSizeinbase(this, 2);

	#endregion

	#region Roots

	public readonly MpzT Sqrt()
	{
		var z = new MpzT();
		Mpir.MpzSqrt(z, this);
		return z;
	}

	public readonly MpzT Sqrt(out MpzT remainder)
	{
		var z = new MpzT();
		remainder = new MpzT();
		Mpir.MpzSqrtrem(z, remainder, this);
		return z;
	}

	public readonly MpzT Sqrt(out bool isExact)
	{
		var z = new MpzT();
		var result = Mpir.MpzRoot(z, this, 2);
		isExact = result != 0;
		return z;
	}

	public readonly MpzT Root(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		Mpir.MpzRoot(z, this, (uint)n);
		return z;
	}

	public readonly MpzT Root(uint n)
	{
		var z = new MpzT();
		Mpir.MpzRoot(z, this, n);
		return z;
	}

	public readonly MpzT Root(int n, out bool isExact)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		var result = Mpir.MpzRoot(z, this, (uint)n);
		isExact = result != 0;
		return z;
	}

	public readonly MpzT Root(uint n, out bool isExact)
	{
		var z = new MpzT();
		var result = Mpir.MpzRoot(z, this, n);
		isExact = result != 0;
		return z;
	}

	public readonly MpzT Root(int n, out MpzT remainder)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		remainder = new MpzT();
		Mpir.MpzRootrem(z, remainder, this, (uint)n);
		return z;
	}

	public readonly MpzT Root(uint n, out MpzT remainder)
	{
		var z = new MpzT();
		remainder = new MpzT();
		Mpir.MpzRootrem(z, remainder, this, n);
		return z;
	}

	public readonly bool IsPerfectSquare() => Mpir.MpzPerfectSquareP(this) != 0;

	public readonly bool IsPerfectPower() =>
		// There is a known issue with this function for negative inputs in GMP 4.2.4.
		// Haven't heard of any issues in MPIR 5.x though.
		Mpir.MpzPerfectPowerP(this) != 0;
	#endregion

	#region Number Theoretic Functions

	public readonly bool IsProbablyPrimeRabinMiller(uint repetitions)
	{
		var result = Mpir.MpzProbabPrimeP(this, repetitions);
		return result != 0;
	}

	// TODO: Create a version of this method which takes in a parameter to represent how well tested the prime should be.
	public readonly MpzT NextPrimeGMP()
	{
		var z = new MpzT();
		Mpir.MpzNextprime(z, this);
		return z;
	}

	public static MpzT Gcd(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzGcd(z, x, y);
		return z;
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

	public static MpzT Gcd(MpzT x, MpzT y, out MpzT a, out MpzT b)
	{
		var z = new MpzT();
		a = new MpzT();
		b = new MpzT();
		Mpir.MpzGcdext(z, a, b, x, y);
		return z;
	}

	public static MpzT Gcd(MpzT x, MpzT y, out MpzT a)
	{
		var z = new MpzT();
		a = new MpzT();
		Mpir.MpzGcdext(z, a, default!, x, y);
		return z;
	}

	public static MpzT Lcm(MpzT x, MpzT y)
	{
		var z = new MpzT();
		Mpir.MpzLcm(z, x, y);
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

	public static MpzT Lcm(int x, MpzT y)
	{
		var z = new MpzT();
		if (x >= 0)
			Mpir.MpzLcmUi(z, y, (uint)x);
		else
			Mpir.MpzLcmUi(z, y, (uint)-x);
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
		System.Diagnostics.Debug.Assert(primeY != 2); // Not defined for 2

		return Mpir.MpzJacobi(x, primeY);
	}

	public static int JacobiSymbol(MpzT x, MpzT y)
	{
		if(IsEvenInteger(y) || y < 0)
		    throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpzJacobi(x, y);
	}

	public static int JacobiSymbol(MpzT x, int y)
	{
		if ((y & 1) == 0 || y < 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpzKroneckerSi(x, y);
	}

	public static int JacobiSymbol(int x, MpzT y)
	{
		if (IsEvenInteger(y) || y < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpzSiKronecker(x, y);
	}

	public static int JacobiSymbol(MpzT x, uint y)
	{
		if ((y & 1) == 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpzKroneckerUi(x, y);
	}

	public static int JacobiSymbol(uint x, MpzT y) {
		if (IsEvenInteger(y) || y < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpzUiKronecker(x, y);
	}
	public static int KroneckerSymbol(MpzT x, MpzT y) => Mpir.MpzKronecker(x, y);

	public static int KroneckerSymbol(MpzT x, int y) => Mpir.MpzKroneckerSi(x, y);

	public static int KroneckerSymbol(int x, MpzT y) => Mpir.MpzSiKronecker(x, y);

	public static int KroneckerSymbol(MpzT x, uint y) => Mpir.MpzKroneckerUi(x, y);

	public static int KroneckerSymbol(uint x, MpzT y) => Mpir.MpzUiKronecker(x, y);

	public readonly MpzT RemoveFactor(MpzT factor)
	{
		var z = new MpzT();
		Mpir.MpzRemove(z, this, factor);
		return z;
	}

	public readonly MpzT RemoveFactor(MpzT factor, out int count)
	{
		var z = new MpzT();
		count = (int)Mpir.MpzRemove(z, this, factor);
		return z;
	}

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

	public static MpzT Binomial(MpzT n, uint k)
	{
		var z = new MpzT();
		Mpir.MpzBinUi(z, n, k);
		return z;
	}

	public static MpzT Binomial(MpzT n, int k)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpzT();
		Mpir.MpzBinUi(z, n, (uint)k);
		return z;
	}

	public static MpzT Binomial(uint n, uint k)
	{
		var z = new MpzT();
		Mpir.MpzBinUiui(z, n, k);
		return z;
	}

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

	public static MpzT Fibonacci(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		Mpir.MpzFibUi(z, (uint)n);
		return z;
	}

	public static MpzT Fibonacci(uint n)
	{
		var z = new MpzT();
		Mpir.MpzFibUi(z, n);
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

	public static MpzT Fibonacci(uint n, out MpzT previous)
	{
		var z = new MpzT();
		previous = new MpzT();
		Mpir.MpzFib2Ui(z, previous, n);
		return z;
	}

	public static MpzT Lucas(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpzT();
		Mpir.MpzLucnumUi(z, (uint)n);
		return z;
	}

	public static MpzT Lucas(uint n)
	{
		var z = new MpzT();
		Mpir.MpzLucnumUi(z, n);
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

	public static MpzT Lucas(uint n, out MpzT previous)
	{
		var z = new MpzT();
		previous = new MpzT();
		Mpir.MpzLucnum2Ui(z, previous, n);
		return z;
	}

	#endregion

	#region Bitwise Functions

	public readonly int CountOnes() => (int)Mpir.MpzPopcount(this);

	public static int HammingDistance(MpzT x, MpzT y) => (int)Mpir.MpzHamdist(x, y);

	public readonly int IndexOfZero(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpzScan0(this, (uint)startingIndex);
		}
	}

	public readonly int IndexOfOne(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpzScan1(this, (uint)startingIndex);
		}
	}

	#endregion

	#region Static Methods

	public static MpzT Abs(MpzT value) => value.Abs();
	public static bool IsCanonical(MpzT value) => true;
	public static bool IsComplexNumber(MpzT value) => true;
	public static bool IsEvenInteger(MpzT value) => (value & 1) == 0;
	public static bool IsFinite(MpzT value) => true;
	public static bool IsImaginaryNumber(MpzT value) => false;
	public static bool IsInfinity(MpzT value) => false;
	public static bool IsInteger(MpzT value) => true;
	public static bool IsNaN(MpzT value) => false;
	public static bool IsNegative(MpzT value) => value < 0;
	public static bool IsNegativeInfinity(MpzT value) => false;
	public static bool IsNormal(MpzT value) => true;
	public static bool IsOddInteger(MpzT value) => !IsEvenInteger(value);
	public static bool IsPositive(MpzT value) => value > 0;
	public static bool IsPositiveInfinity(MpzT value) => false;
	public static bool IsRealNumber(MpzT value) => true;
	public static bool IsSubnormal(MpzT value) => true;
	public static bool IsZero(MpzT value) => value == 0;
	public static MpzT Max(MpzT x, MpzT y) => x > y ? x : y;
	public static MpzT MaxMagnitude(MpzT x, MpzT y) => Max(x, y);
	public static MpzT MaxMagnitudeNumber(MpzT x, MpzT y) => Max(x, y);
	public static MpzT Min(MpzT x, MpzT y) => x < y ? x : y;
	public static MpzT MinMagnitude(MpzT x, MpzT y) => Min(x, y);
	public static MpzT MinMagnitudeNumber(MpzT x, MpzT y) => Min(x, y);
	public static MpzT Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);
	public static MpzT Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => Parse(s.ToString(), style, provider);
	public static MpzT Parse(string s) => new(s);
	public static MpzT Parse(string s, IFormatProvider? provider) => new(s);
	public static MpzT Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);

	private static bool TryConvertFromChecked<TOther>(TOther value, out MpzT result)
	{
		try
		{
			result = value switch
			{
				MpzT z => z,
				byte y => y,
				sbyte sy => sy,
				short si => si,
				ushort usi => usi,
				int i => i,
				uint ui => ui,
				long li => li,
				ulong uli => uli,
				float f => f,
				double d => d,
				string s => new(s),
				_ => throw new InvalidCastException(),
			};
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}

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

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out MpzT result) => TryParse(s.ToString(), out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out MpzT result) => TryParse(s.ToString(), out result);
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
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out MpzT result) => TryParse(s, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MpzT result) => TryParse(s, out result);

	#endregion

	#region Comparing

	public override readonly int GetHashCode()
	{
		uint hash = 0;
		var bytes = ToByteArray(-1);
		var len = bytes.Length;  // Make sure it's only evaluated once.
		var shift = 0;
		for (var i = 0; i < len; i++)
		{
			hash ^= (uint)bytes[i] << shift;
			shift = (shift + 8) & 0x1F;
		}

		return (int)hash;
	}

	public readonly bool Equals(MpzT other) => Compare(this, other) == 0;

	public override readonly bool Equals(object? obj) => obj switch
	{
		null => false,
		MpzT objAsBigInt => CompareTo(objAsBigInt) == 0,
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
		_ => false
	};

	public readonly bool Equals(int other) => CompareTo(other) == 0;

	public readonly bool Equals(uint other) => CompareTo(other) == 0;

	public readonly bool Equals(long other) => CompareTo(other) == 0;

	public readonly bool Equals(ulong other) => CompareTo(other) == 0;

	public readonly bool Equals(double other) => CompareTo(other) == 0;

	//public bool Equals(decimal other)
	//{
	//    return this.CompareTo(other) == 0;
	//}

	public readonly bool EqualsMod(MpzT x, MpzT mod) => Mpir.MpzCongruentP(this, x, mod) != 0;

	public readonly bool EqualsMod(int x, int mod)
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

	public readonly bool EqualsMod(uint x, uint mod) => Mpir.MpzCongruentUiP(this, x, mod) != 0;

	public static bool operator ==(MpzT x, MpzT y) => x.CompareTo(y) == 0;

	public static bool operator ==(int x, MpzT y) => y.CompareTo(x) == 0;

	public static bool operator ==(MpzT x, int y) => x.CompareTo(y) == 0;

	public static bool operator ==(uint x, MpzT y) => y.CompareTo(x) == 0;

	public static bool operator ==(MpzT x, uint y) => x.CompareTo(y) == 0;

	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(long x, MpzT y) => y.CompareTo(x) == 0;

	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(MpzT x, long y) => x.CompareTo(y) == 0;

	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(ulong x, MpzT y) => y.CompareTo(x) == 0;

	// TODO: Optimize this by accessing memory directly.
	public static bool operator ==(MpzT x, ulong y) => x.CompareTo(y) == 0;

	public static bool operator ==(float x, MpzT y) => y.CompareTo(x) == 0;

	public static bool operator ==(MpzT x, float y) => x.CompareTo(y) == 0;

	public static bool operator ==(double x, MpzT y) => y.CompareTo(x) == 0;

	public static bool operator ==(MpzT x, double y) => x.CompareTo(y) == 0;

	//public static bool operator ==(decimal x, MpzT y)
	//{
	//    if(y == null)
	//        return false;
	//    return y.CompareTo(x) == 0;
	//}

	//public static bool operator ==(MpzT x, decimal y)
	//{
	//    if(x == null)
	//        return false;
	//    return x.CompareTo(y) == 0;
	//}

	public static bool operator !=(MpzT x, MpzT y) => x.CompareTo(y) != 0;

	public static bool operator !=(int x, MpzT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpzT x, int y) => x.CompareTo(y) != 0;

	public static bool operator !=(uint x, MpzT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpzT x, uint y) => x.CompareTo(y) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(long x, MpzT y) => y.CompareTo((MpzT)x) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(MpzT x, long y) => x.CompareTo((MpzT)y) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(ulong x, MpzT y) => y.CompareTo((MpzT)x) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(MpzT x, ulong y) => x.CompareTo((MpzT)y) != 0;

	public static bool operator !=(float x, MpzT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpzT x, float y) => x.CompareTo(y) != 0;

	public static bool operator !=(double x, MpzT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpzT x, double y) => x.CompareTo(y) != 0;

	//public static bool operator !=(decimal x, MpzT y)
	//{
	//    if(y == null)
	//        return true;
	//    return y.CompareTo(x) != 0;
	//}

	//public static bool operator !=(MpzT x, decimal y)
	//{
	//    if(x == null)
	//        return true;
	//    return x.CompareTo(y) != 0;
	//}

	public readonly int CompareTo(object? obj) => obj switch
	{
		MpzT objAsBigInt => CompareTo(objAsBigInt),
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
		_ => throw new ArgumentException("Cannot compare to " + (obj?.GetType()?.ToString() ?? "null"))
	};

	public readonly int CompareTo(MpzT other) => Mpir.MpzCmp(this, other);

	public readonly int CompareTo(int other) => Mpir.MpzCmpSi(this, other);

	public readonly int CompareTo(uint other) => Mpir.MpzCmpUi(this, other);

	// TODO: Optimize by accessing the memory directly
	public readonly int CompareTo(long other)
	{
		var otherMpz = new MpzT(other);
		var ret = CompareTo(otherMpz);
		return ret;
	}

	// TODO: Optimize by accessing the memory directly
	public readonly int CompareTo(ulong other)
	{
		var otherMpz = new MpzT(other);
		var ret = CompareTo(otherMpz);
		return ret;
	}

	public readonly int CompareTo(float other) => Mpir.MpzCmpD(this, (double)other);

	public readonly int CompareTo(double other) => Mpir.MpzCmpD(this, other);

	//public int CompareTo(decimal other)
	//{
	//    return mpir.MpzCmpD(this, (double)other);
	//}

	public readonly int CompareAbsTo(object obj)
	{
		if (obj is not MpzT objAsBigInt)
		{
			if (obj is int i)
				return CompareAbsTo(i);
			else if (obj is uint ui)
				return CompareAbsTo(ui);
			else if (obj is long li)
				return CompareAbsTo(li);
			else if (obj is ulong uli)
				return CompareAbsTo(uli);
			else if (obj is double d)
				return CompareAbsTo(d);
			else if (obj is float f)
				return CompareAbsTo(f);
			else if (obj is short si)
				return CompareAbsTo(si);
			else if (obj is ushort usi)
				return CompareAbsTo(usi);
			else if (obj is byte y)
				return CompareAbsTo(y);
			else if (obj is sbyte sy)
				return CompareAbsTo(sy);
			//else if(obj is decimal)
			//    return this.CompareAbsTo((decimal)obj);
			else if (obj is string s)
				return CompareAbsTo(new MpzT(s));
			else
				throw new ArgumentException("Cannot compare to " + obj.GetType());
		}

		return CompareAbsTo(objAsBigInt);
	}

	public readonly int CompareAbsTo(MpzT other) => Mpir.MpzCmpabs(this, other);

	public readonly int CompareAbsTo(int other) => Mpir.MpzCmpabsUi(this, (uint)other);

	public readonly int CompareAbsTo(uint other) => Mpir.MpzCmpabsUi(this, other);

	public readonly int CompareAbsTo(long other) => CompareAbsTo((MpzT)other);

	public readonly int CompareAbsTo(ulong other) => CompareAbsTo((MpzT)other);

	public readonly int CompareAbsTo(double other) => Mpir.MpzCmpabsD(this, other);

	//public int CompareAbsTo(decimal other)
	//{
	//    return mpir.MpzCmpabsD(this, (double)other);
	//}

	public static int Compare(MpzT x, object? y) => x.CompareTo(y);

	public static int Compare(object x, MpzT y) => -y.CompareTo(x);

	public static int Compare(MpzT x, MpzT y) => x.CompareTo(y);

	public static int Compare(MpzT x, int y) => x.CompareTo(y);

	public static int Compare(int x, MpzT y) => -y.CompareTo(x);

	public static int Compare(MpzT x, uint y) => x.CompareTo(y);

	public static int Compare(uint x, MpzT y) => -y.CompareTo(x);

	public static int Compare(MpzT x, long y) => x.CompareTo(y);

	public static int Compare(long x, MpzT y) => -y.CompareTo(x);

	public static int Compare(MpzT x, ulong y) => x.CompareTo(y);

	public static int Compare(ulong x, MpzT y) => -y.CompareTo(x);

	public static int Compare(MpzT x, double y) => x.CompareTo(y);

	public static int Compare(double x, MpzT y) => -y.CompareTo(x);

	//public static int Compare(MpzT x, decimal y)
	//{
	//    return x.CompareTo(y);
	//}

	//public static int Compare(decimal x, MpzT y)
	//{
	//    return -y.CompareTo(x);
	//}

	public static int CompareAbs(MpzT x, object y) => x.CompareAbsTo(y);

	public static int CompareAbs(object x, MpzT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpzT x, MpzT y) => x.CompareAbsTo(y);

	public static int CompareAbs(MpzT x, int y) => x.CompareAbsTo(y);

	public static int CompareAbs(int x, MpzT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpzT x, uint y) => x.CompareAbsTo(y);

	public static int CompareAbs(uint x, MpzT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpzT x, long y) => x.CompareAbsTo(y);

	public static int CompareAbs(long x, MpzT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpzT x, ulong y) => x.CompareAbsTo(y);

	public static int CompareAbs(ulong x, MpzT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpzT x, double y) => x.CompareAbsTo(y);

	public static int CompareAbs(double x, MpzT y) => -y.CompareAbsTo(x);

	//public static int CompareAbs(MpzT x, decimal y)
	//{
	//    return x.CompareAbsTo(y);
	//}

	//public static int CompareAbs(decimal x, MpzT y)
	//{
	//    return -y.CompareAbsTo(x);
	//}

	readonly int IComparable.CompareTo(object? obj) => Compare(this, obj);

	#endregion

	#region Casting

	public static implicit operator MpzT(byte value) => new((uint)value);

	public static implicit operator MpzT(int value) => new(value);

	public static implicit operator MpzT(uint value) => new(value);

	public static implicit operator MpzT(short value) => new(value);

	public static implicit operator MpzT(ushort value) => new(value);

	public static implicit operator MpzT(long value) => new(value);

	public static implicit operator MpzT(ulong value) => new(value);

	public static implicit operator MpzT(float value) => new((double)value);

	public static implicit operator MpzT(double value) => new(value);

	//public static implicit operator MpzT(decimal value)
	//{
	//    return new MpzT(value);
	//}

	public static explicit operator MpzT(string value) => new(value, sDefaultStringBase);

	public static explicit operator byte(MpzT value) => (byte)(uint)value;

	public static explicit operator int(MpzT value) => Mpir.MpzGetSi(value);

	public static explicit operator uint(MpzT value)
	{
		var result = Mpir.MpzGetUi(value);
		if (value < 0)
			result = ~result + 1;
		return result;
	}

	public static explicit operator short(MpzT value) => (short)(int)value;

	public static explicit operator ushort(MpzT value) => (ushort)(uint)value;

	public static explicit operator long(MpzT value)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(8);
		Array.Fill(bytes, (byte)(value >= 0 ? 0 : 255));
		var exportedBytes = value.ToByteArray(BitConverter.IsLittleEndian ? -1 : 1);
		var length = Math.Min(exportedBytes.Length, bytes.Length);
		var destOffset = BitConverter.IsLittleEndian ? 0 : 8 - length;
		Buffer.BlockCopy(exportedBytes, 0, bytes, destOffset, length);
		return BitConverter.ToInt64(bytes, 0);
	}

	public static explicit operator ulong(MpzT value)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(8);
		Array.Fill(bytes, (byte)(value >= 0 ? 0 : 255));
		var exportedBytes = value.ToByteArray(BitConverter.IsLittleEndian ? -1 : 1);
		var length = Math.Min(exportedBytes.Length, bytes.Length);
		var destOffset = BitConverter.IsLittleEndian ? 0 : 8 - length;
		Buffer.BlockCopy(exportedBytes, 0, bytes, destOffset, length);
		return BitConverter.ToUInt64(bytes, 0);
	}

	public static explicit operator float(MpzT value) => (float)(double)value;

	public static explicit operator double(MpzT value) => Mpir.MpzGetD(value);

	//public static explicit operator decimal(MpzT value)
	//{
	//    return (decimal)(double)value;
	//}

	public static explicit operator string?(MpzT value) => value.ToString();

	#endregion

	#region Cloning

	readonly object ICloneable.Clone() => Clone();

	public readonly MpzT Clone() => new(this);

	#endregion

	#region Conversions

	public readonly BigInteger ToBigInteger() => new(ToByteArray(-1));

	public override readonly string? ToString() => ToString((int)sDefaultStringBase);

	public readonly string? ToString(uint @base) => val == 0 ? "0" : Mpir.MpzGetString(@base, this);

	public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
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

	public readonly string ToString(string? format, IFormatProvider? formatProvider) => string.Format(formatProvider, format ?? "{0:N0}", ToString());

	#region IConvertible Members

	readonly TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

	readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new InvalidCastException();

	readonly byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

	readonly char IConvertible.ToChar(IFormatProvider? provider) => throw new InvalidCastException();

	readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();

	readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new InvalidCastException();

	readonly double IConvertible.ToDouble(IFormatProvider? provider) => (double)this;

	readonly short IConvertible.ToInt16(IFormatProvider? provider) => (short)this;

	readonly int IConvertible.ToInt32(IFormatProvider? provider) => (int)this;

	readonly long IConvertible.ToInt64(IFormatProvider? provider) => (long)this;

	readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => (sbyte)this;

	readonly float IConvertible.ToSingle(IFormatProvider? provider) => (float)this;

	readonly string IConvertible.ToString(IFormatProvider? provider) => ToString() ?? "";

	readonly object IConvertible.ToType(Type targetType, IFormatProvider? provider)
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
		else if (targetType == typeof(string))
			return value.ToString(provider);
		else if (targetType == typeof(object))
			return value;
		throw new InvalidCastException();
	}

	readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;

	readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;

	readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	#endregion

	static bool INumberBase<MpzT>.TryConvertFromChecked<TOther>(TOther value, out MpzT result) => TryConvertFromChecked(value, out result);

	static bool INumberBase<MpzT>.TryConvertFromSaturating<TOther>(TOther value, out MpzT result)
	{
		try
		{
			result = value switch
			{
				MpzT z => z,
				byte y => y,
				sbyte sy => sy,
				short si => si,
				ushort usi => usi,
				int i => i,
				uint ui => ui,
				long li => li,
				ulong uli => uli,
				float f => MathF.Ceiling(MathF.Abs(f)) * MathF.Sign(f),
				double d => Math.Ceiling(Math.Abs(d)) * Math.Sign(d),
				string s => new(s),
				_ => throw new InvalidCastException(),
			};
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}

	static bool INumberBase<MpzT>.TryConvertFromTruncating<TOther>(TOther value, out MpzT result) => TryConvertFromChecked(value, out result);

	static bool INumberBase<MpzT>.TryConvertToChecked<TOther>(MpzT value, out TOther result) => TryConvertToChecked(value, out result);

	static bool INumberBase<MpzT>.TryConvertToSaturating<TOther>(MpzT value, out TOther result) => TryConvertToChecked(value, out result);

	static bool INumberBase<MpzT>.TryConvertToTruncating<TOther>(MpzT value, out TOther result) => TryConvertToChecked(value, out result);

	#endregion Conversions
}
