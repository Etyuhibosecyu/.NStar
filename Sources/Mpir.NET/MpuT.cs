// WARNING!!! This file is NOT a part of the original Mpir.NET library, it has been created by Red-Star-Soft!

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

// Disable warning about missing XML comments.

namespace Mpir.NET;

public struct MpuT : ICloneable, IConvertible, IComparable, IBinaryInteger<MpuT>
{
	#region Data
	private const uint sDefaultStringBase = 10u;

	public nint val;
	#endregion

	#region Creation and destruction

	/// Initializes a new MpuT to 0.
	public MpuT() => val = Mpir.MpuInit();
	/// Initializes a new MpuT to the same value as op.
	public MpuT(MpuT op) => val = Mpir.MpuInitSet(op);
	/// Initializes a new MpuT to the unsigned int op.
	public MpuT(uint op) => val = Mpir.MpuInitSetUi(op);
	/// Initializes a new MpuT to the int op.
	public MpuT(int op)
	{
		if (op < 0)
			throw new ArgumentException("Этот тип не поддерживает отрицательные числа.", nameof(op));
		val = Mpir.MpuInitSetSi(op);
	}

	/// Initializes a new MpuT to the double op.
	public MpuT(double op)
	{
		if (op < 0)
			throw new ArgumentException("Этот тип не поддерживает отрицательные числа.", nameof(op));
		val = Mpir.MpuInitSetD(op);
	}

	/// Initializes a new MpuT to string s, parsed as an integer in the specified base.
	public MpuT(string? s, uint Base)
	{
		var s2 = s ?? "0";
		if (s2.Contains('-'))
			throw new ArgumentException("Этот тип не поддерживает отрицательные числа.", nameof(s));
		val = Mpir.MpuInitSetStr(s2, Base);
	}

	/// Initializes a new MpuT to string s, parsed as an integer in base 10.
	public MpuT(string? s) : this(s, sDefaultStringBase) { }
	/// Initializes a new MpuT to the BigInteger op.
	public MpuT(BigInteger op) : this(op.ToByteArray(), -1)
	{
		if (op < 0)
			throw new ArgumentException("Этот тип не поддерживает отрицательные числа.", nameof(op));
		FromByteArray(op.ToByteArray(), -1);
	}

	/// Initializes a new MpuT to using MPIR MpuInit2. Only use if you need to
	/// avoid reallocations.
	//
	// Initialization with MpuInit2 should not be confused with MpuT construction
	// from a ulong. Thus, so we use a static construction function instead, and add
	// the dummy type init2Type to enable us to write a ctor with a unique signature.
	public static MpuT Init2(ulong n) => new(Init2Type.init2, n);
	private enum Init2Type { init2 }
	private MpuT(Init2Type _, ulong n) => val = Mpir.MpuInit2(n);

	/// Initializes a new MpuT to the long op.
	public MpuT(long op) : this()
	{
		if (op < 0)
			throw new ArgumentException("Этот тип не поддерживает отрицательные числа.", nameof(op));
		var bytes = BitConverter.GetBytes(op);
		FromByteArray(bytes, BitConverter.IsLittleEndian ? -1 : 1);
	}

	/// Initializes a new MpuT to the unsigned long op.
	public MpuT(ulong op) : this()
	{
		var bytes = BitConverter.GetBytes(op);
		FromByteArray(bytes, BitConverter.IsLittleEndian ? -1 : 1);
	}

	/// Initializes a new MpuT to the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	public MpuT(ReadOnlySpan<byte> bytes, int order) : this() => FromByteArray(bytes, order);

	#endregion

	#region Import and export byte array

	/// Import the integer in the byte array bytes.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	public readonly void FromByteArray(ReadOnlySpan<byte> source, int order) => Mpir.MpirMpuImport(this, (uint)source.Length, order, sizeof(byte), 0, 0u, source);

	/// Import the integer in the byte array bytes, starting at startOffset
	/// and ending at endOffset.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	public readonly void ImportByOffset(ReadOnlySpan<byte> source, int startOffset, int endOffset, int order) => Mpir.MpirMpuImportByOffset(this, startOffset, endOffset, order, sizeof(byte), 0, 0u, source);

	/// Export to the value to a byte array.
	/// Endianess is specified by order, which is 1 for big endian or -1
	/// for little endian.
	public readonly byte[] ToByteArray(int order) => val == 0 ? [] : Mpir.MpirMpuExport(order, sizeof(byte), 0, 0u, this);
	#endregion

	// Almost everything below is copied from Emil Stefanov's BigInt
	// http://www.emilstefanov.net/Projects/GnuMpDotNet/
	// with a few minor adjustments, e.g. datatype used
	// and ++ and -- operators which now do in-place inrements/decrements).
	// All code handling Decimal is commented out, dute to some
	// unexpected behaviour.

	// TODO: Dispose temp MpuT objects that are created by casts in operators.

	#region Predefined Values

	public static MpuT Zero => new(0);
	public static MpuT One => new(1);
	public static MpuT Two => new(2);
	public static MpuT Three => new(3);
	public static MpuT Ten => new(10);
	public static MpuT AdditiveIdentity => Zero;
	public static MpuT MultiplicativeIdentity => One;
	public static int Radix => 2;

	#endregion

	#region Operators

	public static MpuT operator +(MpuT value) => new(value);

	public static MpzT operator -(MpuT x)
	{
		var z = new MpzT();
		Mpir.MpzNeg(z, x);
		return z;
	}

	static MpuT IUnaryNegationOperators<MpuT, MpuT>.operator -(MpuT value) => throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");

	public static MpzT operator ~(MpuT x)
	{
		var z = new MpzT();
		Mpir.MpzCom(z, x);
		return z;
	}

	static MpuT IBitwiseOperators<MpuT, MpuT, MpuT>.operator ~(MpuT value) => throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");

	public static MpuT operator +(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuAdd(z, x, y);
		return z;
	}

	public static MpuT operator +(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(x, -y) < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		if (y >= 0)
			Mpir.MpuAddUi(z, x, (uint)y);
		else
			Mpir.MpuSubUi(z, x, (uint)-y);
		return z;
	}

	public static MpuT operator +(int x, MpuT y)
	{
		if (Mpir.MpuCmpSi(y, -x) > 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		if (x >= 0)
			Mpir.MpuAddUi(z, y, (uint)x);
		else
			Mpir.MpuSubUi(z, y, (uint)-x);
		return z;
	}

	public static MpuT operator +(MpuT x, uint y)
	{
		var z = new MpuT();
		Mpir.MpuAddUi(z, x, y);
		return z;
	}

	public static MpuT operator +(uint x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuAddUi(z, y, x);
		return z;
	}

	public static MpuT operator -(MpuT x, MpuT y)
	{
		if (Mpir.MpuCmp(x, y) < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuSub(z, x, y);
		return z;
	}

	public static MpuT operator -(int x, MpuT y)
	{
		if (Mpir.MpuCmpSi(y, x) > 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuUiSub(z, (uint)x, y);
		return z;
	}

	public static MpuT operator -(MpuT x, int y)
	{
		if (Mpir.MpuCmpSi(x, y) < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		if (y >= 0)
			Mpir.MpuSubUi(z, x, (uint)y);
		else
			Mpir.MpuAddUi(z, x, (uint)-y);

		return z;
	}

	public static MpuT operator -(uint x, MpuT y)
	{
		if (Mpir.MpuCmpUi(y, x) > 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuUiSub(z, x, y);
		return z;
	}

	public static MpuT operator -(MpuT x, uint y)
	{
		if (Mpir.MpuCmpUi(x, y) < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuSubUi(z, x, y);
		return z;
	}

	public static MpuT operator ++(MpuT x)
	{
		var z = new MpuT();
		Mpir.MpuAddUi(z, x, 1);
		Mpir.MpuSet(x, z);
		return x;
	}

	public static MpuT operator --(MpuT x)
	{
		if (Mpir.MpuCmpUi(x, 0) == 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuSubUi(z, x, 1);
		Mpir.MpuSet(x, z);
		return x;
	}

	public static MpuT operator *(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuMul(z, x, y);
		return z;
	}

	public static MpuT operator *(int x, MpuT y)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuMulSi(z, y, x);
		return z;
	}

	public static MpuT operator *(MpuT x, int y)
	{
		if (y < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuMulSi(z, x, y);
		return z;
	}

	public static MpuT operator *(uint x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuMulUi(z, y, x);
		return z;
	}

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

	public static MpuT operator /(MpuT x, int y)
	{
		if (y < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var quotient = new MpuT();
		Mpir.MpuTdivQUi(quotient, x, (uint)y);
		return quotient;
	}

	public static MpuT operator /(MpuT x, uint y)
	{
		var quotient = new MpuT();
		Mpir.MpuTdivQUi(quotient, x, y);
		return quotient;
	}

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

	public static MpuT operator %(MpuT x, MpuT mod)
	{
		var z = new MpuT();
		Mpir.MpuMod(z, x, mod);
		return z;
	}

	public static MpuT operator %(MpuT x, int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		var z = new MpuT();
		Mpir.MpuFdivRUi(z, x, (uint)mod);
		return z;
	}

	public static MpuT operator %(MpuT x, uint mod)
	{
		var z = new MpuT();
		Mpir.MpuFdivRUi(z, x, mod);
		return z;
	}

	public static bool operator <(MpuT x, MpuT y) => x.CompareTo(y) < 0;

	public static bool operator <(int x, MpuT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpuT x, int y) => x.CompareTo(y) < 0;

	public static bool operator <(uint x, MpuT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpuT x, uint y) => x.CompareTo(y) < 0;

	public static bool operator <(long x, MpuT y) => (MpuT)x < y;

	public static bool operator <(MpuT x, long y) => x < (MpuT)y;

	public static bool operator <(ulong x, MpuT y) => (MpuT)x < y;

	public static bool operator <(MpuT x, ulong y) => x < (MpuT)y;

	public static bool operator <(float x, MpuT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpuT x, float y) => x.CompareTo(y) < 0;

	public static bool operator <(double x, MpuT y) => y.CompareTo(x) > 0;

	public static bool operator <(MpuT x, double y) => x.CompareTo(y) < 0;

	//public static bool operator <(decimal x, MpuT y)
	//{
	//	return y.CompareTo(x) > 0;
	//}

	//public static bool operator <(MpuT x, decimal y)
	//{
	//	return x.CompareTo(y) < 0;
	//}

	public static bool operator <=(MpuT x, MpuT y) => x.CompareTo(y) <= 0;

	public static bool operator <=(int x, MpuT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpuT x, int y) => x.CompareTo(y) <= 0;

	public static bool operator <=(uint x, MpuT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpuT x, uint y) => x.CompareTo(y) <= 0;

	// TODO: Implement by accessing the data directly
	public static bool operator <=(long x, MpuT y) => (MpuT)x <= y;

	public static bool operator <=(MpuT x, long y) => x <= (MpuT)y;

	public static bool operator <=(ulong x, MpuT y) => (MpuT)x <= y;

	public static bool operator <=(MpuT x, ulong y) => x <= (MpuT)y;

	public static bool operator <=(float x, MpuT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpuT x, float y) => x.CompareTo(y) <= 0;

	public static bool operator <=(double x, MpuT y) => y.CompareTo(x) >= 0;

	public static bool operator <=(MpuT x, double y) => x.CompareTo(y) <= 0;

	//public static bool operator <=(decimal x, MpuT y)
	//{
	//	return y.CompareTo(x) >= 0;
	//}

	//public static bool operator <=(MpuT x, decimal y)
	//{
	//	return x.CompareTo(y) <= 0;
	//}

	public static bool operator >(MpuT x, MpuT y) => x.CompareTo(y) > 0;

	public static bool operator >(int x, MpuT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpuT x, int y) => x.CompareTo(y) > 0;

	public static bool operator >(uint x, MpuT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpuT x, uint y) => x.CompareTo(y) > 0;

	// TODO: Implement by accessing the data directly
	public static bool operator >(long x, MpuT y) => (MpuT)x > y;

	// TODO: Implement by accessing the data directly
	public static bool operator >(MpuT x, long y) => x > (MpuT)y;

	// TODO: Implement by accessing the data directly
	public static bool operator >(ulong x, MpuT y) => (MpuT)x > y;

	// TODO: Implement by accessing the data directly
	public static bool operator >(MpuT x, ulong y) => x > (MpuT)y;

	public static bool operator >(float x, MpuT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpuT x, float y) => x.CompareTo(y) > 0;

	public static bool operator >(double x, MpuT y) => y.CompareTo(x) < 0;

	public static bool operator >(MpuT x, double y) => x.CompareTo(y) > 0;

	//public static bool operator >(decimal x, MpuT y)
	//{
	//	return y.CompareTo(x) < 0;
	//}

	//public static bool operator >(MpuT x, decimal y)
	//{
	//	return x.CompareTo(y) > 0;
	//}

	public static bool operator >=(MpuT x, MpuT y) => x.CompareTo(y) >= 0;

	public static bool operator >=(int x, MpuT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpuT x, int y) => x.CompareTo(y) >= 0;

	public static bool operator >=(uint x, MpuT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpuT x, uint y) => x.CompareTo(y) >= 0;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(long x, MpuT y) => (MpuT)x >= y;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(MpuT x, long y) => x >= (MpuT)y;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(ulong x, MpuT y) => (MpuT)x >= y;

	// TODO: Implement by accessing the data directly
	public static bool operator >=(MpuT x, ulong y) => x >= (MpuT)y;

	public static bool operator >=(float x, MpuT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpuT x, float y) => x.CompareTo(y) >= 0;

	public static bool operator >=(double x, MpuT y) => y.CompareTo(x) <= 0;

	public static bool operator >=(MpuT x, double y) => x.CompareTo(y) >= 0;

	//public static bool operator >=(decimal x, MpuT y)
	//{
	//	return y.CompareTo(x) <= 0;
	//}

	//public static bool operator >=(MpuT x, decimal y)
	//{
	//	return x.CompareTo(y) >= 0;
	//}

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
		if (x >= 0)
			return x >> shiftAmount;
		throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
	}

	public readonly int this[int bitIndex] => Mpir.MpuTstbit(this, (uint)bitIndex);

	public readonly MpuT ChangeBit(int bitIndex, int value)
	{
		var z = new MpuT(this);
		if (value == 0)
			Mpir.MpuClrbit(z, (uint)bitIndex);
		else
			Mpir.MpuSetbit(z, (uint)bitIndex);
		return z;
	}

	#endregion

	#region Basic Arithmetic

	/// Returns a new MpuT which is the absolute value of this value.
	public readonly MpuT Abs()
	{
		var result = new MpuT();
		Mpir.MpuAbs(result, this);
		return result;
	}

	public readonly MpzT Negate() => -this;

	public readonly MpzT Complement() => ~this;

	public readonly MpuT Add(MpuT x) => this + x;

	public readonly MpuT Add(int x) => this + x;

	public readonly MpuT Add(uint x) => this + x;

	public readonly MpuT Subtract(MpuT x) => this - x;

	public readonly MpuT Subtract(int x) => this - x;

	public readonly MpuT Subtract(uint x) => this - x;

	public readonly MpuT Multiply(MpuT x) => this * x;

	public readonly MpuT Multiply(int x) => this * x;

	public readonly MpuT Multiply(uint x) => this * x;

	public readonly MpuT Square() => this * this;

	public readonly MpuT Divide(MpuT x) => this / x;

	public readonly MpuT Divide(int x) => this / x;

	public readonly MpuT Divide(uint x) => this / x;

	public readonly MpuT Divide(MpuT x, out MpuT remainder)
	{
		var quotient = new MpuT();
		remainder = new MpuT();
		Mpir.MpuTdivQr(quotient, remainder, this, x);
		return quotient;
	}

	public readonly MpuT Divide(int x, out MpuT remainder)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var quotient = new MpuT();
		remainder = new MpuT();
		Mpir.MpuTdivQrUi(quotient, remainder, this, (uint)x);
		return quotient;
	}

	public readonly MpuT Divide(int x, out int remainder)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var quotient = new MpuT();
		remainder = (int)Mpir.MpuTdivQUi(quotient, this, (uint)x);
		return quotient;
	}

	public readonly MpuT Divide(uint x, out MpuT remainder)
	{
		var quotient = new MpuT();
		remainder = new MpuT();
		Mpir.MpuTdivQrUi(quotient, remainder, this, x);
		return quotient;
	}

	public readonly MpuT Divide(uint x, out uint remainder)
	{
		// Unsure about the below exception for negative numbers. It's in Stefanov's
		// original code, but that limitation isn't mentioned in
		// http://Gmplib.org/manual/Integer-Division.html#Integer-Division.
		//if(this.ChunkCount < 0)
		//	throw new InvalidOperationException("This method may not be called when the instance represents a negative number.");
		var quotient = new MpuT();
		remainder = Mpir.MpuTdivQUi(quotient, this, x);
		return quotient;
	}

	public readonly MpuT Divide(uint x, out int remainder)
	{
		var quotient = new MpuT();
		var uintRemainder = Mpir.MpuTdivQUi(quotient, this, x);
		if (uintRemainder > int.MaxValue)
			throw new OverflowException();
		remainder = (int)uintRemainder;
		return quotient;
	}

	public readonly MpuT Remainder(MpuT x)
	{
		var z = new MpuT();
		Mpir.MpuTdivR(z, this, x);
		return z;
	}

	public readonly bool IsDivisibleBy(MpuT x) => Mpir.MpuDivisibleP(this, x) != 0;

	public readonly bool IsDivisibleBy(int x)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		return Mpir.MpuDivisibleUiP(this, (uint)x) != 0;
	}

	public readonly bool IsDivisibleBy(uint x) => Mpir.MpuDivisibleUiP(this, x) != 0;

	/// <summary>
	/// Divides exactly. Only works when the division is gauranteed to be exact (there is no remainder).
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public readonly MpuT DivideExactly(MpuT x)
	{
		var z = new MpuT();
		Mpir.MpuDivexact(z, this, x);
		return z;
	}

	public readonly MpuT DivideExactly(int x)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuDivexactUi(z, this, (uint)x);
		return z;
	}

	public readonly MpuT DivideExactly(uint x)
	{
		var z = new MpuT();
		Mpir.MpuDivexactUi(z, this, x);
		return z;
	}

	public readonly MpuT DivideMod(MpuT x, MpuT mod) => this * x.InvertMod(mod) % mod;

	public readonly MpuT And(MpuT x) => this & x;

	public readonly MpuT Or(MpuT x) => this | x;

	public readonly MpuT Xor(MpuT x) => this ^ x;

	public readonly MpuT Mod(MpuT mod) => this % mod;

	public readonly MpuT Mod(int mod) => this % mod;

	public readonly MpuT Mod(uint mod) => this % mod;

	public readonly int ModAsInt32(int mod)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(mod);
		return (int)Mpir.MpuFdivUi(this, (uint)mod);
	}

	public readonly uint ModAsUInt32(uint mod) => Mpir.MpuFdivUi(this, mod);

	public readonly MpuT ShiftLeft(int shiftAmount) => this << shiftAmount;

	public readonly MpuT ShiftRight(int shiftAmount) => this >> shiftAmount;

	public readonly MpuT PowerMod(MpuT exponent, MpuT mod)
	{
		var z = new MpuT();
		Mpir.MpuPowm(z, this, exponent, mod);
		return z;
	}

	public readonly MpuT PowerMod(int exponent, MpuT mod)
	{
		var z = new MpuT();
		Mpir.MpuPowm(z, this, exponent, mod);
		return z;
	}

	public readonly MpuT PowerMod(uint exponent, MpuT mod)
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

	public readonly MpuT Power(int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpuT();
		Mpir.MpuPowUi(z, this, (uint)exponent);
		return z;
	}

	public readonly MpuT Power(uint exponent)
	{
		var z = new MpuT();
		Mpir.MpuPowUi(z, this, exponent);
		return z;
	}

	public static MpuT Power(int x, int exponent)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(exponent);
		var z = new MpuT();
		Mpir.MpuUiPowUi(z, (uint)x, (uint)exponent);
		return z;
	}

	public static MpuT Power(uint x, uint exponent)
	{
		var z = new MpuT();
		Mpir.MpuUiPowUi(z, x, exponent);
		return z;
	}

	public readonly MpuT InvertMod(MpuT mod)
	{
		var z = new MpuT();
		var status = Mpir.MpuInvert(z, this, mod);
		if (status == 0)
			throw new ArithmeticException("This modular inverse does not exists.");
		return z;
	}

	public readonly bool TryInvertMod(MpuT mod, out MpuT result)
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

	public readonly bool InverseModExists(MpuT mod)
	{
		TryInvertMod(mod, out _);
		return true;
	}

	public readonly int GetByteCount() => (BitLength + 7) / 8;

	public readonly int GetShortestBitLength() => BitLength;

	public readonly int BitLength => (int)Mpir.MpuSizeinbase(this, 2);

	public readonly int Sign =>
		IsPositive(this) ? 1 : IsZero(this) ? 0 : IsNegative(this) ? -1
		: throw new ArithmeticException("Произошла ошибка при  вычислении знака.");

	public readonly MpuT GetFullBitLength() => Mpir.MpuSizeinbase(this, 2);

	#endregion

	#region Roots

	public readonly MpuT Sqrt()
	{
		var z = new MpuT();
		Mpir.MpuSqrt(z, this);
		return z;
	}

	public readonly MpuT Sqrt(out MpuT remainder)
	{
		var z = new MpuT();
		remainder = new MpuT();
		Mpir.MpuSqrtrem(z, remainder, this);
		return z;
	}

	public readonly MpuT Sqrt(out bool isExact)
	{
		var z = new MpuT();
		var result = Mpir.MpuRoot(z, this, 2);
		isExact = result != 0;
		return z;
	}

	public readonly MpuT Root(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		Mpir.MpuRoot(z, this, (uint)n);
		return z;
	}

	public readonly MpuT Root(uint n)
	{
		var z = new MpuT();
		Mpir.MpuRoot(z, this, n);
		return z;
	}

	public readonly MpuT Root(int n, out bool isExact)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		var result = Mpir.MpuRoot(z, this, (uint)n);
		isExact = result != 0;
		return z;
	}

	public readonly MpuT Root(uint n, out bool isExact)
	{
		var z = new MpuT();
		var result = Mpir.MpuRoot(z, this, n);
		isExact = result != 0;
		return z;
	}

	public readonly MpuT Root(int n, out MpuT remainder)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		remainder = new MpuT();
		Mpir.MpuRootrem(z, remainder, this, (uint)n);
		return z;
	}

	public readonly MpuT Root(uint n, out MpuT remainder)
	{
		var z = new MpuT();
		remainder = new MpuT();
		Mpir.MpuRootrem(z, remainder, this, n);
		return z;
	}

	public readonly bool IsPerfectSquare() => Mpir.MpuPerfectSquareP(this) != 0;

	public readonly bool IsPerfectPower() =>
		// There is a known issue with this function for negative inputs in GMP 4.2.4.
		// Haven't heard of any issues in MPIR 5.x though.
		Mpir.MpuPerfectPowerP(this) != 0;
	#endregion

	#region Number Theoretic Functions

	public readonly bool IsProbablyPrimeRabinMiller(uint repetitions)
	{
		var result = Mpir.MpuProbabPrimeP(this, repetitions);
		return result != 0;
	}

	// TODO: Create a version of this method which takes in a parameter to represent how well tested the prime should be.
	public readonly MpuT NextPrimeGMP()
	{
		var z = new MpuT();
		Mpir.MpuNextprime(z, this);
		return z;
	}

	public static MpuT Gcd(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuGcd(z, x, y);
		return z;
	}

	public static MpuT Gcd(MpuT x, int y)
	{
		if (y < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuGcdUi(z, x, (uint)y);
		return z;
	}

	public static MpuT Gcd(int x, MpuT y)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuGcdUi(z, y, (uint)x);
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

	public static MpuT Gcd(MpuT x, MpuT y, out MpuT a, out MpuT b)
	{
		var z = new MpuT();
		a = new MpuT();
		b = new MpuT();
		Mpir.MpuGcdext(z, a, b, x, y);
		return z;
	}

	public static MpuT Gcd(MpuT x, MpuT y, out MpuT a)
	{
		var z = new MpuT();
		a = new MpuT();
		Mpir.MpuGcdext(z, a, default!, x, y);
		return z;
	}

	public static MpuT Lcm(MpuT x, MpuT y)
	{
		var z = new MpuT();
		Mpir.MpuLcm(z, x, y);
		return z;
	}

	public static MpuT Lcm(MpuT x, int y)
	{
		if (y < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuLcmUi(z, x, (uint)y);
		return z;
	}

	public static MpuT Lcm(int x, MpuT y)
	{
		if (x < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		var z = new MpuT();
		Mpir.MpuLcmUi(z, y, (uint)x);
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
		System.Diagnostics.Debug.Assert(primeY != 2); // Not defined for 2

		return Mpir.MpuJacobi(x, primeY);
	}

	public static int JacobiSymbol(MpuT x, MpuT y)
	{
		if(IsEvenInteger(y) || y < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpuJacobi(x, y);
	}

	public static int JacobiSymbol(MpuT x, int y)
	{
		if ((y & 1) == 0 || y < 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpuKroneckerSi(x, y);
	}

	public static int JacobiSymbol(int x, MpuT y)
	{
		if (IsEvenInteger(y) || y < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpuSiKronecker(x, y);
	}

	public static int JacobiSymbol(MpuT x, uint y)
	{
		if ((y & 1) == 0)
			throw new ArgumentException(null, nameof(y));
		return Mpir.MpuKroneckerUi(x, y);
	}

	public static int JacobiSymbol(uint x, MpuT y) {
		if (IsEvenInteger(y) || y < 0)
			throw new ArgumentException(nameof(y) + " must be odd and positive");
		return Mpir.MpuUiKronecker(x, y);
	}
	public static int KroneckerSymbol(MpuT x, MpuT y) => Mpir.MpuKronecker(x, y);

	public static int KroneckerSymbol(MpuT x, int y) => Mpir.MpuKroneckerSi(x, y);

	public static int KroneckerSymbol(int x, MpuT y) => Mpir.MpuSiKronecker(x, y);

	public static int KroneckerSymbol(MpuT x, uint y) => Mpir.MpuKroneckerUi(x, y);

	public static int KroneckerSymbol(uint x, MpuT y) => Mpir.MpuUiKronecker(x, y);

	public readonly MpuT RemoveFactor(MpuT factor)
	{
		var z = new MpuT();
		Mpir.MpuRemove(z, this, factor);
		return z;
	}

	public readonly MpuT RemoveFactor(MpuT factor, out int count)
	{
		var z = new MpuT();
		count = (int)Mpir.MpuRemove(z, this, factor);
		return z;
	}

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

	public static MpuT Binomial(MpuT n, uint k)
	{
		var z = new MpuT();
		Mpir.MpuBinUi(z, n, k);
		return z;
	}

	public static MpuT Binomial(MpuT n, int k)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpuT();
		Mpir.MpuBinUi(z, n, (uint)k);
		return z;
	}

	public static MpuT Binomial(uint n, uint k)
	{
		var z = new MpuT();
		Mpir.MpuBinUiui(z, n, k);
		return z;
	}

	public static MpuT Binomial(int n, int k)
	{
		if (n < 0)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		ArgumentOutOfRangeException.ThrowIfNegative(k);
		var z = new MpuT();
		Mpir.MpuBinUiui(z, (uint)n, (uint)k);
		return z;
	}

	public static MpuT Fibonacci(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		Mpir.MpuFibUi(z, (uint)n);
		return z;
	}

	public static MpuT Fibonacci(uint n)
	{
		var z = new MpuT();
		Mpir.MpuFibUi(z, n);
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

	public static MpuT Fibonacci(uint n, out MpuT previous)
	{
		var z = new MpuT();
		previous = new MpuT();
		Mpir.MpuFib2Ui(z, previous, n);
		return z;
	}

	public static MpuT Lucas(int n)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(n);
		var z = new MpuT();
		Mpir.MpuLucnumUi(z, (uint)n);
		return z;
	}

	public static MpuT Lucas(uint n)
	{
		var z = new MpuT();
		Mpir.MpuLucnumUi(z, n);
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

	public static MpuT Lucas(uint n, out MpuT previous)
	{
		var z = new MpuT();
		previous = new MpuT();
		Mpir.MpuLucnum2Ui(z, previous, n);
		return z;
	}

	#endregion

	#region Bitwise Functions

	public readonly int CountOnes() => (int)Mpir.MpuPopcount(this);

	public static int HammingDistance(MpuT x, MpuT y) => (int)Mpir.MpuHamdist(x, y);

	public readonly int IndexOfZero(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpuScan0(this, (uint)startingIndex);
		}
	}

	public readonly int IndexOfOne(int startingIndex)
	{
		unchecked
		{
			ArgumentOutOfRangeException.ThrowIfNegative(startingIndex);
			// Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
			return (int)Mpir.MpuScan1(this, (uint)startingIndex);
		}
	}

	public static MpuT PopCount(MpuT value) => value.CountOnes();

	public static MpuT TrailingZeroCount(MpuT value)
	{
		if (value == Zero)
			return Zero;
		var result = 0;
		const int ulongBits = sizeof(ulong) * 8;
		for (MpuT i = ulong.MaxValue; i < value; i <<= ulongBits)
			if ((value & i) == 0)
				result += ulongBits;
		for (var i = One << value.BitLength / ulongBits * ulongBits; i < value; i <<= 1)
			if ((value & i) == 0)
				result++;
		return result;
	}

	public static bool IsPow2(MpuT value) => value.CountOnes() == 1;

	public static MpuT Log2(MpuT value)
	{
		var bitLength = value.BitLength;
		var sqrt = (One << bitLength << bitLength - 1).Sqrt();
		return value >= sqrt ? bitLength : bitLength - 1;
	}

	#endregion

	#region Static Methods

	public static MpuT Abs(MpuT value) => value.Abs();
	public static bool IsCanonical(MpuT value) => true;
	public static bool IsComplexNumber(MpuT value) => true;
	public static bool IsEvenInteger(MpuT value) => (value & 1) == 0;
	public static bool IsFinite(MpuT value) => true;
	public static bool IsImaginaryNumber(MpuT value) => false;
	public static bool IsInfinity(MpuT value) => false;
	public static bool IsInteger(MpuT value) => true;
	public static bool IsNaN(MpuT value) => false;
	public static bool IsNegative(MpuT value) => Mpir.MpuCmpSi(value, 0) < 0;
	public static bool IsNegativeInfinity(MpuT value) => false;
	public static bool IsNormal(MpuT value) => true;
	public static bool IsOddInteger(MpuT value) => !IsEvenInteger(value);
	public static bool IsPositive(MpuT value) => Mpir.MpuCmpSi(value, 0) > 0;
	public static bool IsPositiveInfinity(MpuT value) => false;
	public static bool IsRealNumber(MpuT value) => true;
	public static bool IsSubnormal(MpuT value) => true;
	public static bool IsZero(MpuT value) => Mpir.MpuCmpSi(value, 0) == 0;
	public static MpuT Max(MpuT x, MpuT y) => x > y ? x : y;
	public static MpuT MaxMagnitude(MpuT x, MpuT y) => Max(x, y);
	public static MpuT MaxMagnitudeNumber(MpuT x, MpuT y) => Max(x, y);
	public static MpuT Min(MpuT x, MpuT y) => x < y ? x : y;
	public static MpuT MinMagnitude(MpuT x, MpuT y) => Min(x, y);
	public static MpuT MinMagnitudeNumber(MpuT x, MpuT y) => Min(x, y);
	public static MpuT Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);
	public static MpuT Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => Parse(s.ToString(), style, provider);
	public static MpuT Parse(string s) => new(s);
	public static MpuT Parse(string s, IFormatProvider? provider) => new(s);
	public static MpuT Parse(string s, NumberStyles style, IFormatProvider? provider) => new(s);

	private static bool TryConvertFromChecked<TOther>(TOther value, out MpuT result)
	{
		try
		{
			result = value switch
			{
				MpuT z => z,
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

	#endregion

	#region Comparing

	public override readonly int GetHashCode()
	{
		uint hash = 0;
		var bytes = ToByteArray(-1);
		var len = bytes.Length; // Make sure it's only evaluated once.
		var shift = 0;
		for (var i = 0; i < len; i++)
		{
			hash ^= (uint)bytes[i] << shift;
			shift = (shift + 8) & 0x1F;
		}

		return (int)hash;
	}

	public readonly bool Equals(MpuT other) => Compare(this, other) == 0;

	public override readonly bool Equals(object? obj) => obj switch
	{
		null => false,
		MpuT objAsBigInt => CompareTo(objAsBigInt) == 0,
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
	//	return this.CompareTo(other) == 0;
	//}

	public readonly bool EqualsMod(MpuT x, MpuT mod) => Mpir.MpuCongruentP(this, x, mod) != 0;

	public readonly bool EqualsMod(int x, int mod)
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

	public readonly bool EqualsMod(uint x, uint mod) => Mpir.MpuCongruentUiP(this, x, mod) != 0;

	public static bool operator ==(MpuT x, MpuT y) => x.CompareTo(y) == 0;

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

	//public static bool operator ==(decimal x, MpuT y)
	//{
	//	if(y is null)
	//		return false;
	//	return y.CompareTo(x) == 0;
	//}

	//public static bool operator ==(MpuT x, decimal y)
	//{
	//	if(x is null)
	//		return false;
	//	return x.CompareTo(y) == 0;
	//}

	public static bool operator !=(MpuT x, MpuT y) => x.CompareTo(y) != 0;

	public static bool operator !=(int x, MpuT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpuT x, int y) => x.CompareTo(y) != 0;

	public static bool operator !=(uint x, MpuT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpuT x, uint y) => x.CompareTo(y) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(long x, MpuT y) => y.CompareTo((MpuT)x) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(MpuT x, long y) => x.CompareTo((MpuT)y) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(ulong x, MpuT y) => y.CompareTo((MpuT)x) != 0;

	// TODO: Optimize this by accessing memory directly
	public static bool operator !=(MpuT x, ulong y) => x.CompareTo((MpuT)y) != 0;

	public static bool operator !=(float x, MpuT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpuT x, float y) => x.CompareTo(y) != 0;

	public static bool operator !=(double x, MpuT y) => y.CompareTo(x) != 0;

	public static bool operator !=(MpuT x, double y) => x.CompareTo(y) != 0;

	//public static bool operator !=(decimal x, MpuT y)
	//{
	//	if(y is null)
	//		return true;
	//	return y.CompareTo(x) != 0;
	//}

	//public static bool operator !=(MpuT x, decimal y)
	//{
	//	if(x is null)
	//		return true;
	//	return x.CompareTo(y) != 0;
	//}

	public readonly int CompareTo(object? obj) => obj switch
	{
		MpuT objAsBigInt => CompareTo(objAsBigInt),
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
		_ => throw new ArgumentException("Cannot compare to " + (obj?.GetType()?.ToString() ?? "null"))
	};

	public readonly int CompareTo(MpuT other) => Mpir.MpuCmp(this, other);

	public readonly int CompareTo(int other) => Mpir.MpuCmpSi(this, other);

	public readonly int CompareTo(uint other) => Mpir.MpuCmpUi(this, other);

	// TODO: Optimize by accessing the memory directly
	public readonly int CompareTo(long other)
	{
		var otherMpu = new MpuT(other);
		var ret = CompareTo(otherMpu);
		return ret;
	}

	// TODO: Optimize by accessing the memory directly
	public readonly int CompareTo(ulong other)
	{
		var otherMpu = new MpuT(other);
		var ret = CompareTo(otherMpu);
		return ret;
	}

	public readonly int CompareTo(float other) => Mpir.MpuCmpD(this, (double)other);

	public readonly int CompareTo(double other) => Mpir.MpuCmpD(this, other);

	//public int CompareTo(decimal other)
	//{
	//	return mpir.MpuCmpD(this, (double)other);
	//}

	public readonly int CompareAbsTo(object obj)
	{
		if (obj is not MpuT objAsBigInt)
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
			//	return this.CompareAbsTo((decimal)obj);
			else if (obj is string s)
				return CompareAbsTo(new MpuT(s));
			else
				throw new ArgumentException("Cannot compare to " + obj.GetType());
		}

		return CompareAbsTo(objAsBigInt);
	}

	public readonly int CompareAbsTo(MpuT other) => Mpir.MpuCmpabs(this, other);

	public readonly int CompareAbsTo(int other) => Mpir.MpuCmpabsUi(this, (uint)other);

	public readonly int CompareAbsTo(uint other) => Mpir.MpuCmpabsUi(this, other);

	public readonly int CompareAbsTo(long other) => CompareAbsTo((MpuT)other);

	public readonly int CompareAbsTo(ulong other) => CompareAbsTo((MpuT)other);

	public readonly int CompareAbsTo(double other) => Mpir.MpuCmpabsD(this, other);

	//public int CompareAbsTo(decimal other)
	//{
	//	return mpir.MpuCmpabsD(this, (double)other);
	//}

	public static int Compare(MpuT x, object? y) => x.CompareTo(y);

	public static int Compare(object x, MpuT y) => -y.CompareTo(x);

	public static int Compare(MpuT x, MpuT y) => x.CompareTo(y);

	public static int Compare(MpuT x, int y) => x.CompareTo(y);

	public static int Compare(int x, MpuT y) => -y.CompareTo(x);

	public static int Compare(MpuT x, uint y) => x.CompareTo(y);

	public static int Compare(uint x, MpuT y) => -y.CompareTo(x);

	public static int Compare(MpuT x, long y) => x.CompareTo(y);

	public static int Compare(long x, MpuT y) => -y.CompareTo(x);

	public static int Compare(MpuT x, ulong y) => x.CompareTo(y);

	public static int Compare(ulong x, MpuT y) => -y.CompareTo(x);

	public static int Compare(MpuT x, double y) => x.CompareTo(y);

	public static int Compare(double x, MpuT y) => -y.CompareTo(x);

	//public static int Compare(MpuT x, decimal y)
	//{
	//	return x.CompareTo(y);
	//}

	//public static int Compare(decimal x, MpuT y)
	//{
	//	return -y.CompareTo(x);
	//}

	public static int CompareAbs(MpuT x, object y) => x.CompareAbsTo(y);

	public static int CompareAbs(object x, MpuT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpuT x, MpuT y) => x.CompareAbsTo(y);

	public static int CompareAbs(MpuT x, int y) => x.CompareAbsTo(y);

	public static int CompareAbs(int x, MpuT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpuT x, uint y) => x.CompareAbsTo(y);

	public static int CompareAbs(uint x, MpuT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpuT x, long y) => x.CompareAbsTo(y);

	public static int CompareAbs(long x, MpuT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpuT x, ulong y) => x.CompareAbsTo(y);

	public static int CompareAbs(ulong x, MpuT y) => -y.CompareAbsTo(x);

	public static int CompareAbs(MpuT x, double y) => x.CompareAbsTo(y);

	public static int CompareAbs(double x, MpuT y) => -y.CompareAbsTo(x);

	//public static int CompareAbs(MpuT x, decimal y)
	//{
	//	return x.CompareAbsTo(y);
	//}

	//public static int CompareAbs(decimal x, MpuT y)
	//{
	//	return -y.CompareAbsTo(x);
	//}

	readonly int IComparable.CompareTo(object? obj) => Compare(this, obj);

	#endregion

	#region Casting

	public static implicit operator MpuT(byte value) => new((uint)value);

	public static implicit operator MpuT(int value) => new(value);

	public static implicit operator MpuT(uint value) => new(value);

	public static implicit operator MpuT(short value) => new(value);

	public static implicit operator MpuT(ushort value) => new(value);

	public static implicit operator MpuT(long value) => new(value);

	public static implicit operator MpuT(ulong value) => new(value);

	public static explicit operator MpuT(float value) => new((double)value);

	public static explicit operator MpuT(double value) => new(value);

	//public static implicit operator MpuT(decimal value)
	//{
	//	return new MpuT(value);
	//}

	public static implicit operator MpzT(MpuT value)
	{
		MpzT z = default;
		z.val = Mpir.MpzInitSet(value);
		return z;
	}

	public static explicit operator MpuT(string value) => new(value, sDefaultStringBase);

	public static explicit operator byte(MpuT value) => (byte)(uint)value;

	public static explicit operator int(MpuT value) => Mpir.MpuGetSi(value);

	public static explicit operator uint(MpuT value)
	{
		var result = Mpir.MpuGetUi(value);
		if (value < 0)
			result = ~result + 1;
		return result;
	}

	public static explicit operator short(MpuT value) => (short)(int)value;

	public static explicit operator ushort(MpuT value) => (ushort)(uint)value;

	public static explicit operator long(MpuT value)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(8);
		Array.Fill(bytes, (byte)0);
		var exportedBytes = value.ToByteArray(BitConverter.IsLittleEndian ? -1 : 1);
		var length = Math.Min(exportedBytes.Length, bytes.Length);
		var destOffset = BitConverter.IsLittleEndian ? 0 : 8 - length;
		Buffer.BlockCopy(exportedBytes, 0, bytes, destOffset, length);
		return BitConverter.ToInt64(bytes, 0);
	}

	public static explicit operator ulong(MpuT value)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(8);
		Array.Fill(bytes, (byte)0);
		var exportedBytes = value.ToByteArray(BitConverter.IsLittleEndian ? -1 : 1);
		var length = Math.Min(exportedBytes.Length, bytes.Length);
		var destOffset = BitConverter.IsLittleEndian ? 0 : 8 - length;
		Buffer.BlockCopy(exportedBytes, 0, bytes, destOffset, length);
		return BitConverter.ToUInt64(bytes, 0);
	}

	public static explicit operator float(MpuT value) => (float)(double)value;

	public static explicit operator double(MpuT value) => Mpir.MpuGetD(value);

	//public static explicit operator decimal(MpuT value)
	//{
	//	return (decimal)(double)value;
	//}

	public static explicit operator string?(MpuT value) => value.ToString();

	#endregion

	#region Cloning

	readonly object ICloneable.Clone() => Clone();

	public readonly MpuT Clone() => new(this);

	#endregion

	#region Conversions

	public readonly BigInteger ToBigInteger() => new([.. ToByteArray(-1), 0]);

	public override readonly string? ToString() => ToString((int)sDefaultStringBase);

	public readonly string? ToString(uint @base) => val == 0 ? "0" : Mpir.MpuGetString(@base, this);

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

	readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => ((IConvertible)this).ToBoolean(provider);

	readonly byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

	readonly char IConvertible.ToChar(IFormatProvider? provider) => ((IConvertible)this).ToChar(provider);

	readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => ((IConvertible)this).ToDateTime(provider);

	readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => ((IConvertible)this).ToDecimal(provider);

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
		if (targetType == typeof(MpuT))
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
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(MpuT)
				+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)this;

	readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)this;

	readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)this;

	#endregion

	static bool INumberBase<MpuT>.TryConvertFromChecked<TOther>(TOther value, out MpuT result) => TryConvertFromChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertFromSaturating<TOther>(TOther value, out MpuT result)
	{
		try
		{
			result = value switch
			{
				MpuT z => z,
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

	static bool INumberBase<MpuT>.TryConvertFromTruncating<TOther>(TOther value, out MpuT result) => TryConvertFromChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertToChecked<TOther>(MpuT value, out TOther result) => TryConvertToChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertToSaturating<TOther>(MpuT value, out TOther result) => TryConvertToChecked(value, out result);

	static bool INumberBase<MpuT>.TryConvertToTruncating<TOther>(MpuT value, out TOther result) => TryConvertToChecked(value, out result);

	public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out MpuT value)
	{
		value = new(source, 1);
		if (!isUnsigned && value.BitLength == source.Length * 8)
			value -= One << value.BitLength;
		return true;
	}

	public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out MpuT value)
	{
		value = new(source, -1);
		if (!isUnsigned && value.BitLength == source.Length * 8)
			value -= One << value.BitLength;
		return true;
	}

	public readonly bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
	{
		var bytes = ToByteArray(1);
		if (MemoryExtensions.AsSpan(bytes).TryCopyTo(destination))
		{
			bytesWritten = bytes.Length;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public readonly bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		var bytes = ToByteArray(-1);
		if (MemoryExtensions.AsSpan(bytes).TryCopyTo(destination))
		{
			bytesWritten = bytes.Length;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	#endregion Conversions
}
