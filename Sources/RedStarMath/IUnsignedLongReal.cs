namespace RedStarMath;

internal interface IUnsignedLongReal<TSelf> : ICloneable, IConvertible, IDisposable,
	IBinaryInteger<TSelf>, IFloatingPoint<TSelf> where TSelf : IUnsignedLongReal<TSelf>
{
	internal TSelf? Exponent { get; }
	internal MpuT Mantissa { get; }
	private protected int MantissaByteLength { get; }
	internal int MantissaLength { get; }
	private protected MpuT MantissaOverflow { get; }

	object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(conversionType);
		if (conversionType == typeof(UnsignedLongReal))
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
		throw new InvalidCastException("Поддерживаются следующие типы: " + nameof(UnsignedLongReal)
			+ ", " + nameof(MpzT) + ", " + nameof(MpuT)
			+ ", byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, string, object.");
	}

	public bool TryWriteBigEndianInterface(Span<byte> destination, out int bytesWritten, bool saveMantissaLength)
	{
		bytesWritten = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(destination, MantissaByteLength);
			destination = destination[sizeof(int)..];
			bytesWritten += sizeof(int);
		}
		if (Exponent is null)
		{
			var result = Mantissa.TryWriteBigEndian(destination, out var mantissaBytesWritten);
			bytesWritten += mantissaBytesWritten;
			return result;
		}
		var mLength = Mantissa.GetByteCount();
		if (!Mantissa.TryWriteBigEndian(destination[^mLength..], out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += MantissaByteLength;
		destination[..(MantissaByteLength - mLength)].Clear();
		if (!Exponent.TryWriteBigEndianInterface(destination[..^MantissaByteLength], out var bytesWritten2, false))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += bytesWritten2;
		return true;
	}

	public virtual bool TryWriteLittleEndianInterface(Span<byte> destination, out int bytesWritten, bool saveMantissaLength)
	{
		bytesWritten = 0;
		if (saveMantissaLength)
		{
			BitConverter.TryWriteBytes(destination, MantissaByteLength);
			destination = destination[sizeof(int)..];
			bytesWritten += sizeof(int);
		}
		if (Exponent is null)
		{
			var result = Mantissa.TryWriteLittleEndian(destination, out var mantissaBytesWritten);
			bytesWritten += mantissaBytesWritten;
			return result;
		}
		var mLength = Mantissa.GetByteCount();
		if (!Mantissa.TryWriteLittleEndian(destination, out _))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += MantissaByteLength;
		destination[mLength..MantissaByteLength].Clear();
		if (!Exponent.TryWriteLittleEndianInterface(destination[MantissaByteLength..], out var bytesWritten2, false))
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten += bytesWritten2;
		return true;
	}
}
