global using System.Globalization;
global using System.Numerics;
global using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NStar.Mpir;

public sealed class UnsignedLongReal : ICloneable, IConvertible, IComparable, IComparable<UnsignedLongReal>,
	IDisposable, IBinaryInteger<UnsignedLongReal>, IFloatingPoint<UnsignedLongReal>
{
	private const int MantissaByteLength = 256, MantissaLength = MantissaByteLength * 8;
	private const uint sDefaultStringBase = 10u;
	private static readonly MpuT MantissaOverflow = new MpuT(1) << MantissaLength;
	private static readonly MpuT MantissaMask = MantissaOverflow - 1;

	private readonly MpuT m;
	private readonly UnsignedLongReal? e;

	private UnsignedLongReal(MpuT m, UnsignedLongReal? e)
	{
		this.m = m;
		this.e = e;
	}

	public UnsignedLongReal(decimal op)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(double op)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(int op)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(uint op)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(long op)
	{
		m = new(op);
		e = null;
	}

	public UnsignedLongReal(ulong op)
	{
		m = new(op);
		e = null;
	}

	public unsafe UnsignedLongReal(MpzT op)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(op);
		var bitLength = op.BitLength;
		if (bitLength <= MantissaLength)
		{
			m = new(op);
			e = null;
		}
		else
		{
			var eDiff = bitLength - MantissaLength - 1;
			m = new((*(MpuT*)&op).ShiftRightRound(eDiff) & MantissaMask);
			e = new(eDiff + 1);
		}
	}

	public UnsignedLongReal(MpuT op)
	{
		var bitLength = op.BitLength;
		if (bitLength <= MantissaLength)
		{
			m = new(op);
			e = null;
		}
		else
		{
			var eDiff = bitLength - MantissaLength - 1;
			m = new(op.ShiftRightRound(eDiff) & MantissaMask);
			e = new(eDiff + 1);
		}
	}

	public UnsignedLongReal(UnsignedLongReal op) : this(op.m, op.e?.Copy()) { }

	public UnsignedLongReal(BigInteger op) : this(new MpuT(op)) { }

	public UnsignedLongReal(string? s) : this(new MpuT(s)) { }

	public UnsignedLongReal(string? s, uint @base) : this(new MpuT(s, @base)) { }

	public UnsignedLongReal(ReadOnlySpan<byte> bytes, int order)
	{
		if (bytes.Length < MantissaByteLength)
		{
			m = new(bytes, order);
			e = null;
		}
		else
		{
			m = new(bytes[..MantissaByteLength], order);
			e = new(bytes[MantissaByteLength..], order);
		}
	}

	~UnsignedLongReal() => Dispose();

	public static UnsignedLongReal AdditiveIdentity => new(0);
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.E => throw new NotSupportedException();
	public static UnsignedLongReal MultiplicativeIdentity => new(1);
	static UnsignedLongReal ISignedNumber<UnsignedLongReal>.NegativeOne => throw new NotSupportedException();
	public static UnsignedLongReal One => new(1);
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.Pi => throw new NotSupportedException();
	public static int Radix => 2;
	static UnsignedLongReal IFloatingPointConstants<UnsignedLongReal>.Tau => throw new NotSupportedException();
	public static UnsignedLongReal Zero => new(0);

	public UnsignedLongReal BitLength => e is null ? m.BitLength : e + MantissaLength;

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
		return m.CompareTo((other >> eDiff - 1) & MantissaMask);
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
		return m.CompareTo(other.ShiftRightRound(eDiff - 1) & MantissaMask);
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
		_ => 0,
	};

	public int CompareTo(UnsignedLongReal? other)
	{
		if (other is null)
			return 1;
		if (e is null && other.e is null)
			return m.CompareTo(other.m);
		if (e is null)
			return -1;
		if (other.e is null)
			return 1;
		var compared = e.CompareTo(other.e);
		if (compared != 0)
			return compared;
		else
			return m.CompareTo(other.m);
	}

	public UnsignedLongReal Copy() => new(m, e?.Copy());

	public void Dispose()
	{
		m.Dispose();
		e?.Dispose();
		GC.SuppressFinalize(this);
	}

	public (UnsignedLongReal Quotient, MpuT Remainder) DivRem(MpuT x)
	{
		if (e is null)
		{
			var result = m.Divide(x, out var remainder);
			return (new(result, null), new(remainder));
		}
		else if (x.BitLength < MantissaLength)
		{
			Debug.Assert(e is not null);
			if (Mpir.MpuCmpSi(x, 0) == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(x, 1) == 0)
				return (this, MpuT.Zero);
			else if (e <= x.BitLength + 1)
				return (new((MantissaOverflow + m << (int)e - 1).Divide(x, out var remainder), null), remainder);
			else
			{
				var quotient = (MantissaOverflow + m << MantissaLength + 1) / x;
				var shiftAmount = quotient.BitLength - MantissaLength - 1;
				return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, e + shiftAmount - MantissaLength - 1), MpuT.Zero);
			}
		}
		else if (e is null || e < x.BitLength - MantissaLength - 1)
			return (Zero, (MpuT)this);
		else
		{
			if (e <= x.BitLength + 1)
				return (new((MantissaOverflow + m << (int)e - 1).Divide(x, out var remainder), null), remainder);
			var quotient = (MantissaOverflow + m << (int)e) / (x << 1);
			var shiftAmount = quotient.BitLength - MantissaLength;
			return (new(quotient.ShiftRightRound(shiftAmount - 1) & MantissaMask, shiftAmount), MpuT.Zero);
		}
	}

	public (UnsignedLongReal Quotient, UnsignedLongReal Remainder) DivRem(UnsignedLongReal x)
	{
		if (e is null && x.e is null)
		{
			var result = m.Divide(x.m, out var remainder);
			return (new(result, null), new(remainder));
		}
		else if (x.e is null)
		{
			Debug.Assert(e is not null);
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return (this, Zero);
			else if (e <= x.m.BitLength)
				return (new(((MantissaOverflow + m) << (int)e - 1).Divide(x.m, out var remainder), null), remainder);
			else
			{
				var quotient = (MantissaOverflow + m << MantissaLength + 1) / x.m;
				var shiftAmount = quotient.BitLength - MantissaLength - 1;
				return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, e + shiftAmount - MantissaLength - 1), Zero);
			}
		}
		else if (e is null || e < x.e)
			return (Zero, Copy());
		else
		{
			if (e < x.e + MantissaLength)
			{
				var eDiff = (int)(e - x.e);
				var quotient = ((MantissaOverflow + m) << eDiff).Divide(MantissaOverflow + x.m, out var remainder);
				return (new(quotient, null), remainder << (int)x.e - 1);
			}
			else
			{
				var quotient = (MantissaOverflow + m << MantissaLength + 1) / (MantissaOverflow + x.m);
				var shiftAmount = quotient.BitLength - MantissaLength - 1;
				return (new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, e - x.e + shiftAmount - MantissaLength), Zero);
			}
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
		_ => false,
	};

	public bool Equals(UnsignedLongReal? other) => CompareTo(other) == 0;

	public int GetByteCount() => e is null ? m.GetByteCount() : MantissaByteLength + e.GetByteCount();
	public int GetExponentByteCount() => e is null ? 0 : e.GetByteCount();
	public int GetExponentShortestBitLength() => e is null ? 0 : e.GetShortestBitLength();
	public int GetSignificandBitLength() => m.GetShortestBitLength();
	public int GetSignificandByteCount() => m.GetByteCount();

	public override int GetHashCode()
	{
		var hash = 486187739;
		hash = (hash * 16777619) ^ m.GetHashCode();
		if (e is null)
			return hash;
		return (hash * 16777619) ^ e.GetHashCode();
	}

	public int GetShortestBitLength() => e is null ? m.GetShortestBitLength() : (int)(e + MantissaLength);
	TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

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
		var sqrt = (One << bitLength << bitLength - 1).Sqrt();
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

	bool IConvertible.ToBoolean(IFormatProvider? provider) => ((IConvertible)this).ToBoolean(provider);

	byte IConvertible.ToByte(IFormatProvider? provider) => (byte)this;

	public byte[] ToByteArray(int order)
	{
		if (e is null)
			return m.ToByteArray(order);
		var bytes = GC.AllocateUninitializedArray<byte>(GetByteCount());
		var mLength = m.GetByteCount();
		if (order < 0)
			m.TryWriteLittleEndian(bytes, out _);
		else
			m.TryWriteBigEndian(bytes.AsSpan(^mLength), out _);
		Array.Fill<byte>(bytes, 0, order < 0 ? mLength : 0, MantissaByteLength - mLength);
		if (order < 0)
			e.TryWriteLittleEndian(bytes.AsSpan(MantissaByteLength), out _);
		else
			e.TryWriteBigEndian(bytes.AsSpan(MantissaByteLength), out _);
		return bytes;
	}

	char IConvertible.ToChar(IFormatProvider? provider) => ((IConvertible)this).ToChar(provider);
	DateTime IConvertible.ToDateTime(IFormatProvider? provider) => ((IConvertible)this).ToDateTime(provider);
	decimal IConvertible.ToDecimal(IFormatProvider? provider) => ((IConvertible)this).ToDecimal(provider);
	double IConvertible.ToDouble(IFormatProvider? provider) => (double)this;
	short IConvertible.ToInt16(IFormatProvider? provider) => (short)this;
	int IConvertible.ToInt32(IFormatProvider? provider) => (int)this;
	long IConvertible.ToInt64(IFormatProvider? provider) => (long)this;
	sbyte IConvertible.ToSByte(IFormatProvider? provider) => (sbyte)(short)this;
	float IConvertible.ToSingle(IFormatProvider? provider) => (float)this;
	public override string? ToString() => ((MpuT)this).ToString();
	public string ToString(IFormatProvider? provider) => ToString() ?? "";
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		string.Format(formatProvider, format ?? "{0:N0}", ToString());

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

	static bool INumberBase<UnsignedLongReal>.TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out UnsignedLongReal result) =>
		TryConvertFromChecked(value, out result);

	static bool INumberBase<UnsignedLongReal>.TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out UnsignedLongReal result)
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

	static bool INumberBase<UnsignedLongReal>.TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out UnsignedLongReal result) =>
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

	bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
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

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out UnsignedLongReal result)
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

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out UnsignedLongReal result)
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

	public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out UnsignedLongReal result)
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

	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
	{
		if (e is null)
			return m.TryWriteBigEndian(destination, out bytesWritten);
		var mLength = m.GetByteCount();
		if (!m.TryWriteBigEndian(destination[^mLength..], out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten = MantissaByteLength;
		destination[..(MantissaByteLength - mLength)].Clear();
		if (!e.TryWriteBigEndian(destination[MantissaByteLength..], out var bytesWritten2))
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

	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		if (e is null)
			return m.TryWriteLittleEndian(destination, out bytesWritten);
		var mLength = m.GetByteCount();
		if (!m.TryWriteLittleEndian(destination, out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten = MantissaByteLength;
		destination[mLength..MantissaByteLength].Clear();
		if (!e.TryWriteLittleEndian(destination[MantissaByteLength..], out var bytesWritten2))
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
	public static implicit operator UnsignedLongReal(int value) => new(value);
	public static implicit operator UnsignedLongReal(uint value) => new(value);
	public static implicit operator UnsignedLongReal(short value) => new(value);
	public static implicit operator UnsignedLongReal(ushort value) => new(value);
	public static implicit operator UnsignedLongReal(long value) => new(value);
	public static implicit operator UnsignedLongReal(ulong value) => new(value);
	public static implicit operator UnsignedLongReal(MpzT value) => new(value);
	public static implicit operator UnsignedLongReal(MpuT value) => new(value);
	public static explicit operator UnsignedLongReal(float value) => new((double)value);
	public static explicit operator UnsignedLongReal(double value) => new(value);
	public static explicit operator UnsignedLongReal(decimal value) => new(value);
	public static explicit operator UnsignedLongReal(string value) => new(value, sDefaultStringBase);
	public static explicit operator byte(UnsignedLongReal value) => (byte)(uint)value;
	public static explicit operator int(UnsignedLongReal value) => (int)(value & uint.MaxValue);
	public static explicit operator uint(UnsignedLongReal value) => value & uint.MaxValue;
	public static explicit operator short(UnsignedLongReal value) => (short)(int)value;
	public static explicit operator ushort(UnsignedLongReal value) => (ushort)(uint)value;
	public static explicit operator long(UnsignedLongReal value) => (long)(value & ulong.MaxValue).m;
	public static explicit operator ulong(UnsignedLongReal value) => (ulong)(value & ulong.MaxValue).m;
	public static explicit operator float(UnsignedLongReal value) => (float)(double)value;

	public static explicit operator double(UnsignedLongReal value) =>
		value.BitLength > 1024 ? double.PositiveInfinity : (double)value.m;

	public static explicit operator decimal(UnsignedLongReal value) => (decimal)((double)value is var x
		&& x is not (< (double)decimal.MinValue or > (double)decimal.MaxValue or double.NaN) ? x : 0);

	public static explicit operator string?(UnsignedLongReal value) => value.ToString();

	public static explicit operator MpzT(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return new MpzT(MantissaOverflow + value.m) << (int)value.e - 1;
		else
			throw new OverflowException("Ошибка, слишком большое число для преобразования в целое.");
	}

	public static explicit operator MpuT(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m);
		else if (value.e <= int.MaxValue)
			return new MpuT(MantissaOverflow + value.m) << (int)value.e - 1;
		else
			throw new OverflowException("Ошибка, слишком большое число для преобразования в целое.");
	}

	public static UnsignedLongReal operator +(UnsignedLongReal value) => new(value);
	static UnsignedLongReal IUnaryNegationOperators<UnsignedLongReal, UnsignedLongReal>.operator -(UnsignedLongReal value) =>
		throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");
	static UnsignedLongReal IBitwiseOperators<UnsignedLongReal, UnsignedLongReal, UnsignedLongReal>.operator ~(UnsignedLongReal value) =>
		throw new NotSupportedException("Этот тип не поддерживает отрицательные числа.");

	public static UnsignedLongReal operator +(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
		{
			var mSum = x.m + y.m;
			if (mSum >= MantissaOverflow)
				return new(mSum & MantissaMask, 1);
			return new(mSum, null);
		}
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= MantissaLength + 1)
				return x.Copy();
			var mSum = x.m + y.m.ShiftRightRound((int)x.e - 1);
			if (mSum >= MantissaOverflow)
				return new((mSum & MantissaMask).ShiftRightRound(1), x.e + 1);
			return new(mSum, x.e?.Copy());
		}
		else if (x.e is null)
		{
			if (y.e >= MantissaLength + 1)
				return y.Copy();
			var mSum = x.m.ShiftRightRound((int)y.e - 1) + y.m;
			if (mSum >= MantissaOverflow)
				return new((mSum & MantissaMask).ShiftRightRound(1), y.e + 1);
			return new(mSum, y.e?.Copy());
		}
		else if (x.e >= y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff == 0)
				return new((x.m + y.m).ShiftRightRound(1), x.e + 1);
			if (eDiff > MantissaLength)
				return x.Copy();
			var mSum = x.m + (MantissaOverflow + y.m).ShiftRightRound((int)eDiff);
			if (mSum >= MantissaOverflow)
				return new((mSum & MantissaMask).ShiftRightRound(1), x.e + 1);
			return new(mSum, x.e?.Copy());
		}
		else
		{
			var eDiff = y.e - x.e;
			if (eDiff > MantissaLength)
				return y.Copy();
			var mSum = (MantissaOverflow + x.m).ShiftRightRound((int)eDiff) + y.m;
			if (mSum >= MantissaOverflow)
				return new((mSum & MantissaMask).ShiftRightRound(1), y.e + 1);
			return new(mSum, y.e?.Copy());
		}
	}

	public static UnsignedLongReal operator -(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
			return new(x.m - y.m, null);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= MantissaLength + 1)
				return x.Copy();
			var mDiff = MantissaOverflow + x.m - y.m.ShiftRightRound((int)x.e - 1);
			if (mDiff >= MantissaOverflow)
				return new(mDiff & MantissaMask, x.e?.Copy());
			else if (x.e == 1)
				return new(mDiff, null);
			else
				return new((mDiff << 1) & MantissaMask, x.e - 1);
		}
		else if (x.e is null || x.e < y.e)
			throw new OverflowException("Этот тип не поддерживает отрицательные числа.");
		else if (x.e > y.e + 1)
		{
			var eDiff = x.e - y.e;
			if (eDiff > MantissaLength)
				return x.Copy();
			var mDiff = MantissaOverflow + x.m - (MantissaOverflow + y.m).ShiftRightRound((int)eDiff);
			if (mDiff >= MantissaOverflow)
				return new(mDiff & MantissaMask, x.e?.Copy());
			else if (x.e == 1)
				return new(mDiff, null);
			else
				return new((mDiff << 1) & MantissaMask, x.e - 1);
		}
		else if (x.e == y.e)
		{
			var mDiff = x.m - y.m;
			if (mDiff == 0)
				return new(0, null);
			var shiftAmount = MantissaLength - mDiff.BitLength + 1;
			if (x.e < shiftAmount)
				return new(mDiff << (int)x.e, null);
			return new((mDiff << shiftAmount) & MantissaMask, x.e - shiftAmount);
		}
		else
		{
			var mDiff = (MantissaOverflow + x.m << 1) - (MantissaOverflow + y.m);
			var shiftAmount = MantissaLength - mDiff.BitLength + 1;
			if (shiftAmount == -1)
				return new(mDiff.ShiftRightRound(1) & MantissaMask, x.e?.Copy());
			return new((mDiff << shiftAmount) & MantissaMask, x.e - shiftAmount - 1);
		}
	}

	public static UnsignedLongReal operator *(UnsignedLongReal x, int y)
	{
		if (x.e is null)
			return new(x.m * y);
		else
		{
			if (y == 0)
				return Zero;
			else if (y == 1)
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount);
		}
	}

	public static UnsignedLongReal operator *(UnsignedLongReal x, uint y)
	{
		if (x.e is null)
			return new(x.m * y);
		else
		{
			if (y == 0)
				return Zero;
			else if (y == 1)
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y;
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount);
		}
	}

	public static UnsignedLongReal operator *(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
			return new(x.m * y.m);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				return Zero;
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x.Copy();
			var product = (MantissaOverflow + x.m) * y.m;
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount);
		}
		else if (x.e is null)
		{
			if (Mpir.MpuCmpSi(x.m, 0) == 0)
				return Zero;
			else if (Mpir.MpuCmpSi(x.m, 1) == 0)
				return y;
			var product = x.m * (MantissaOverflow + y.m);
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, y.e + shiftAmount);
		}
		else
		{
			var product = (MantissaOverflow + x.m) * (MantissaOverflow + y.m);
			var shiftAmount = product.BitLength - MantissaLength - 1;
			return new(product.ShiftRightRound(shiftAmount) & MantissaMask, x.e + y.e + shiftAmount - 1);
		}
	}

	public static UnsignedLongReal operator /(UnsignedLongReal x, int y)
	{
		if (x.e is null)
			return new(x.m / y, null);
		else
		{
			if (y == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (y == 1)
				return x.Copy();
			else if (x.e <= sizeof(int) * 8 - int.LeadingZeroCount(y))
				return new(((MantissaOverflow + x.m) << (int)x.e - 1) / y);
			var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y;
			var shiftAmount = quotient.BitLength - MantissaLength - 1;
			return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1);
		}
	}

	public static UnsignedLongReal operator /(UnsignedLongReal x, uint y)
	{
		if (x.e is null)
			return new(x.m / y, null);
		else
		{
			if (y == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (y == 1)
				return x.Copy();
			else if (x.e <= sizeof(uint) * 8 - uint.LeadingZeroCount(y))
				return new(((MantissaOverflow + x.m) << (int)x.e - 1) / y);
			var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y;
			var shiftAmount = quotient.BitLength - MantissaLength - 1;
			return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1);
		}
	}

	public static UnsignedLongReal operator /(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
			return new(x.m / y.m, null);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (Mpir.MpuCmpSi(y.m, 0) == 0)
				throw new DivideByZeroException("Этот тип не поддерживает деление на ноль.");
			else if (Mpir.MpuCmpSi(y.m, 1) == 0)
				return x.Copy();
			else if (x.e <= y.m.BitLength)
				return new(((MantissaOverflow + x.m) << (int)x.e - 1) / y.m);
			var quotient = (MantissaOverflow + x.m << MantissaLength + 1) / y.m;
			var shiftAmount = quotient.BitLength - MantissaLength - 1;
			return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e + shiftAmount - MantissaLength - 1);
		}
		else if (x.e is null || x.e < y.e)
			return Zero;
		else
		{
			if (x.e <= y.e + MantissaLength + 1)
				return new(((MantissaOverflow + x.m) << (int)(x.e - y.e)) / (MantissaOverflow + y.m));
			var quotient = (MantissaOverflow + x.m << MantissaLength + 2) / (MantissaOverflow + y.m);
			var shiftAmount = quotient.BitLength - MantissaLength - 1;
			return new(quotient.ShiftRightRound(shiftAmount) & MantissaMask, x.e - y.e + shiftAmount - MantissaLength - 1);
		}
	}

	public static UnsignedLongReal operator %(UnsignedLongReal x, MpuT y) => x.DivRem(y).Remainder;

	public static UnsignedLongReal operator %(UnsignedLongReal x, UnsignedLongReal y) => x.DivRem(y).Remainder;

	public static int operator &(UnsignedLongReal x, int y)
	{
		if (y == 1)
			return x.e is null ? (x.m & 1) : 0;
		else if (x.e is null)
			return x.m & y;
		else if (x.e >= sizeof(int) * 8)
			return 0;
		else
			return MantissaOverflow + x.m << (int)x.e - 1 & y;
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
			return MantissaOverflow + x.m << (int)x.e - 1 & y;
	}

	public static UnsignedLongReal operator &(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
			return new(x.m & y.m, null);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e > y.m.BitLength)
				return Zero;
			return new(x.m << (int)x.e - 1 & y.m);
		}
		else if (x.e is null)
		{
			if (y.e > x.m.BitLength)
				return Zero;
			return new(x.m & y.m << (int)y.e - 1);
		}
		else
		{
			if (x.e > y.e)
				(x, y) = (y, x);
			var eDiff = y.e - x.e;
			if (eDiff >= MantissaLength)
				return Zero;
			var newMantissa = MantissaOverflow + x.m & (MantissaOverflow + y.m << (int)eDiff);
			if (Mpir.MpzCmpSi(newMantissa, 0) == 0)
				return Zero;
			var shiftAmount = (MantissaOverflow + x.m).BitLength - newMantissa.BitLength;
			if (x.e <= shiftAmount)
				return new(newMantissa << (int)x.e - 1, null);
			return new(newMantissa << shiftAmount & MantissaMask, x.e - shiftAmount);
		}
	}

	public static UnsignedLongReal operator |(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
			return new(x.m | y.m, null);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= MantissaLength + 1)
				return x.Copy();
			return new(x.m | y.m.ShiftRightRound((int)x.e - 1), x.e?.Copy());
		}
		else if (x.e is null)
		{
			if (y.e >= MantissaLength + 1)
				return y.Copy();
			return new(x.m.ShiftRightRound((int)y.e - 1) | y.m, y.e?.Copy());
		}
		else if (x.e == y.e)
			return new(x.m | y.m, x.e?.Copy());
		else if (x.e > y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff >= MantissaLength)
				return x.Copy();
			return new(x.m | (MantissaOverflow + y.m).ShiftRightRound((int)eDiff), x.e?.Copy());
		}
		else
		{
			var eDiff = y.e - x.e;
			if (eDiff >= MantissaLength)
				return y.Copy();
			return new((MantissaOverflow + x.m).ShiftRightRound((int)eDiff) | y.m, y.e?.Copy());
		}
	}

	public static UnsignedLongReal operator ^(UnsignedLongReal x, UnsignedLongReal y)
	{
		if (x.e is null && y.e is null)
			return new(x.m ^ y.m, null);
		else if (y.e is null)
		{
			Debug.Assert(x.e is not null);
			if (x.e >= MantissaLength + 1)
				return x.Copy();
			return new(x.m ^ y.m.ShiftRightRound((int)x.e - 1), x.e?.Copy());
		}
		else if (x.e is null)
		{
			if (y.e >= MantissaLength + 1)
				return y.Copy();
			return new(x.m.ShiftRightRound((int)y.e - 1) ^ y.m, y.e?.Copy());
		}
		else if (x.e > y.e)
		{
			var eDiff = x.e - y.e;
			if (eDiff >= MantissaLength)
				return x.Copy();
			return new(x.m ^ (MantissaOverflow + y.m).ShiftRightRound((int)eDiff), x.e?.Copy());
		}
		else if (x.e < y.e)
		{
			var eDiff = y.e - x.e;
			if (eDiff >= MantissaLength)
				return y.Copy();
			return new((MantissaOverflow + x.m).ShiftRightRound((int)eDiff) ^ y.m, y.e?.Copy());
		}
		else
		{
			var mXor = x.m ^ y.m;
			var shiftAmount = MantissaLength + 1 - mXor.BitLength;
			return new((mXor << shiftAmount) & MantissaMask, x.e - shiftAmount);
		}
	}

	public static UnsignedLongReal operator <<(UnsignedLongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x.Copy();
		else if (x.e is null)
			return new(x.m << shiftAmount);
		else
			return new(x.m, x.e + shiftAmount);
	}

	public static UnsignedLongReal operator <<(UnsignedLongReal x, UnsignedLongReal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x.Copy();
		else if (x.e is not null)
			return new(x.m, x.e + shiftAmount);
		else if (shiftAmount < MantissaLength)
			return new(x.m << (int)shiftAmount);
		else
			return new UnsignedLongReal(x.m << MantissaLength) << shiftAmount - MantissaLength;
	}

	public static UnsignedLongReal operator >>(UnsignedLongReal x, int shiftAmount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(shiftAmount);
		if (shiftAmount == 0)
			return x.Copy();
		else if (x.e is null)
			return new(x.m.ShiftRightRound(shiftAmount), null);
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount);
		else
			return new((MantissaOverflow + x.m).ShiftRightRound(shiftAmount - (int)x.e + 1), null);
	}

	public static UnsignedLongReal operator >>(UnsignedLongReal x, UnsignedLongReal shiftAmount)
	{
		if (shiftAmount.CompareTo(0) == 0)
			return x.Copy();
		else if (x.e is null)
		{
			if (shiftAmount > MantissaLength)
				return Zero;
			return new(x.m.ShiftRightRound((int)shiftAmount), null);
		}
		else if (x.e > shiftAmount)
			return new(x.m, x.e - shiftAmount);
		var restShiftAmount = shiftAmount - x.e;
		if (restShiftAmount > MantissaLength)
			return Zero;
		else
			return new((MantissaOverflow + x.m).ShiftRightRound((int)restShiftAmount + 1), null);
	}

	public static UnsignedLongReal operator >>>(UnsignedLongReal x, int shiftAmount) => x >> shiftAmount;

	public static UnsignedLongReal operator >>>(UnsignedLongReal x, UnsignedLongReal shiftAmount) => x >> shiftAmount;

	public static UnsignedLongReal operator ++(UnsignedLongReal value)
	{
		if (value.e is not null)
			return value.Copy();
		else if (Mpir.MpuCmp(value.m, MantissaMask) == 0)
			return new(0, null);
		else
			return new(value.m + 1, null);
	}

	public static UnsignedLongReal operator --(UnsignedLongReal value)
	{
		if (value.e is null)
			return new(value.m - 1, null);
		else if (Mpir.MpuCmpSi(value.m, 0) == 0 && value.e.e is null && Mpir.MpuCmpSi(value.e.m, 1) == 0)
			return new(MantissaMask, null);
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
