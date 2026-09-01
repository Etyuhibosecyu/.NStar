namespace RedStarMath;

internal interface IComplexNumber<T, TSelf> : INumber<TSelf>
	where T : struct, IFloatingPoint<T> where TSelf : struct, IComplexNumber<T, TSelf>
{
	protected static abstract Func<T, T, TSelf> Creator { get; }
	internal T Imaginary { get; }
	internal static TSelf ImaginaryOne { get; } = TSelf.Creator(T.Zero, T.One);
	internal T MagnitudeInterface => AbsInterface((TSelf)this);
	internal static TSelf OneInterface { get; } = TSelf.Creator(T.One, T.Zero);
	internal T PhaseInterface => Atan2(Imaginary, Real);

	private static T PositiveInfinity => T.Zero switch
	{
		double => T.CreateTruncating(double.PositiveInfinity),
		decimal => throw new OverflowException("Тип decimal не поддерживает бесконечность и неопределенность."),
		LongReal => T.CreateTruncating(LongReal.PositiveInfinity),
		LongDecimal => T.CreateTruncating(LongDecimal.PositiveInfinity),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal T Real { get; }
	internal static TSelf ZeroInterface { get; } = TSelf.Creator(T.Zero, T.Zero);

	private static T Abs(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Abs(r)),
		decimal d => T.CreateTruncating(Math.Abs(d)),
		LongReal lr => T.CreateTruncating(LongReal.Abs(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Abs(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static T AbsInterface(TSelf value)
	{
		if (T.IsInfinity(value.Real) || T.IsInfinity(value.Imaginary))
			return PositiveInfinity;
		// |value| == sqrt(a^2 + b^2)
		// sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2)
		// Using the above we can factor out the square of the larger component to dodge overflow.
		var re = T.Abs(value.Real);
		var im = T.Abs(value.Imaginary);
		if (re > im)
		{
			var r = im / re;
			return re * Sqrt(T.One + r * r);
		}
		else if (im == T.Zero)
			return re;
		else
		{
			var r = re / im;
			return im * Sqrt(T.One + r * r);
		}
	}

	internal static TSelf AcosInterface(TSelf value) =>
		-ImaginaryOne * LogInterface(value + ImaginaryOne * SqrtInterface(OneInterface - value * value));
	internal static TSelf AcoshInterface(TSelf value) =>
		LogInterface(value + SqrtInterface(SquareInterface(value) - TSelf.One));

	internal static TSelf AsinInterface(TSelf value) =>
		-ImaginaryOne * LogInterface(ImaginaryOne * value + SqrtInterface(OneInterface - value * value));
	internal static TSelf AsinhInterface(TSelf value) => LogInterface(value + SqrtInterface(SquareInterface(value) + TSelf.One));

	internal static TSelf AtanInterface(TSelf value)
	{
		var Two = TSelf.Creator(T.One + T.One, T.Zero);
		return ImaginaryOne / Two * (LogInterface(OneInterface - ImaginaryOne * value)
			- LogInterface(OneInterface + ImaginaryOne * value));
	}

	private static T Atan2(T yValue, T xValue) => (yValue, xValue) switch
	{
		(double yValue2, double xValue2) => T.CreateTruncating(Math.Atan2(yValue2, xValue2)),
		(decimal yValue2, decimal xValue2) => T.CreateTruncating(Math.Atan2(yValue2, xValue2)),
		(LongReal yValue2, LongReal xValue2) => T.CreateTruncating(LongReal.Atan2(yValue2, xValue2)),
		(LongDecimal yValue2, LongDecimal xValue2) => T.CreateTruncating(LongDecimal.Atan2(yValue2, xValue2)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf AtanhInterface(TSelf value) =>
		LogInterface((TSelf.One + value) / (TSelf.One - value)) / (TSelf.One + TSelf.One);

	internal static TSelf ConjugateInterface(TSelf value) => TSelf.Creator(value.Real, -value.Imaginary);

	private static T Cos(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Cos(r)),
		decimal d => T.CreateTruncating(Math.Cos(d)),
		LongReal lr => T.CreateTruncating(LongReal.Cos(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Cos(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf CosInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Cos(a) * Cosh(b), -(Sin(a) * Sinh(b)));
	}

	private static T Cosh(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Cosh(r)),
		decimal d => T.CreateTruncating(Math.Cosh(d)),
		LongReal lr => T.CreateTruncating(LongReal.Cosh(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Cosh(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf CoshInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Cosh(a) * Cos(b), Sinh(a) * Sin(b));
	}

	internal bool EqualsInterface(object? obj) => obj switch
	{
		null => false,
		TSelf ts => Real.Equals(ts.Real) && Imaginary.Equals(ts.Imaginary),
		T t => Real == t && Imaginary == T.Zero,
		int i => (TSelf)this == TSelf.CreateTruncating(i),
		uint ui => (TSelf)this == TSelf.CreateTruncating(ui),
		long li => (TSelf)this == TSelf.CreateTruncating(li),
		ulong uli => (TSelf)this == TSelf.CreateTruncating(uli),
		double d => (TSelf)this == TSelf.CreateTruncating(d),
		float f => (TSelf)this == TSelf.CreateTruncating(f),
		short si => (TSelf)this == TSelf.CreateTruncating(si),
		ushort usi => (TSelf)this == TSelf.CreateTruncating(usi),
		byte y => (TSelf)this == TSelf.CreateTruncating(y),
		sbyte sy => (TSelf)this == TSelf.CreateTruncating(sy),
		System.Numerics.Complex c => Real.Equals(c.Real) && Imaginary.Equals(c.Imaginary),
		IConvertible ic => ic.Equals(this),
		_ => false
	};

	internal bool EqualsInterface(TSelf value) => Real.Equals(value.Real) && Imaginary.Equals(value.Imaginary);

	private static T Exp(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Exp(r)),
		decimal d => T.CreateTruncating(Math.Exp(d)),
		LongReal lr => T.CreateTruncating(LongReal.Exp(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Exp(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf ExpInterface(TSelf value)
	{
		var temp_factor = Exp(value.Real);
		var result_re = temp_factor * Cos(value.Imaginary);
		var result_im = temp_factor * Sin(value.Imaginary);
		return TSelf.Creator(result_re, result_im);
	}

	internal static TSelf FromPolarCoordinatesInterface(T magnitude, T phase) =>
		TSelf.Creator(magnitude * Cos(phase), magnitude * Sin(phase));

	internal int GetHashCodeInterface()
	{
		var n1 = 99999997;
		var hash_real = Real.GetHashCode() % n1;
		var hash_imaginary = Imaginary.GetHashCode();
		var final_hashcode = hash_real ^ hash_imaginary;
		return final_hashcode;
	}

	private static T Log(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Log(r)),
		decimal d => T.CreateTruncating(Math.Log(d)),
		LongReal lr => T.CreateTruncating(LongReal.Ln(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Ln(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf LogInterface(TSelf value) =>
		TSelf.Creator(Log(AbsInterface(value)), Atan2(value.Imaginary, value.Real));
	internal static TSelf LogInterface(TSelf value, T baseValue) => LogInterface(value) / Log(baseValue);

	private static T Power(T value, T power) => (value, power) switch
	{
		(double value2, double power2) => T.CreateTruncating(Math.Power(value2, power2)),
		(decimal value2, decimal power2) => T.CreateTruncating(Math.Power(value2, power2)),
		(LongReal value2, LongReal power2) => T.CreateTruncating(LongReal.Power(value2, power2)),
		(LongDecimal value2, LongDecimal power2) => T.CreateTruncating(LongDecimal.Power(value2, power2)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf PowInterface(TSelf value, int power) =>
		PowInterface(value, TSelf.Creator(T.CreateTruncating(power), T.Zero));
	internal static TSelf PowInterface(TSelf value, T power) => PowInterface(value, TSelf.Creator(power, T.Zero));

	internal static TSelf PowInterface(TSelf value, TSelf power)
	{
		if (power == ZeroInterface)
			return OneInterface;
		if (value == ZeroInterface)
			return ZeroInterface;
		if (value.Imaginary == T.Zero && power.Imaginary == T.Zero)
			return TSelf.Creator(Power(value.Real, power.Real), T.Zero);
		var a = value.Real;
		var b = value.Imaginary;
		var c = power.Real;
		var d = power.Imaginary;
		var rho = AbsInterface(value);
		var theta = Atan2(b, a);
		var newRho = c * theta + d * Log(rho);
		var t = Power(rho, c) * Power(T.E, -d * theta);
		return TSelf.Creator(t * Cos(newRho), t * Sin(newRho));
	}

	internal static TSelf ReciprocInterface(TSelf value) =>
		value.Imaginary == T.Zero ? TSelf.Creator(T.One / value.Real, T.Zero) : OneInterface / value;

	private static T Sin(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Sin(r)),
		decimal d => T.CreateTruncating(Math.Sin(d)),
		LongReal lr => T.CreateTruncating(LongReal.Sin(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Sin(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf SinInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Sin(a) * Cosh(b), Cos(a) * Sinh(b));
	}

	private static T Sinh(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Sinh(r)),
		decimal d => T.CreateTruncating(Math.Sinh(d)),
		LongReal lr => T.CreateTruncating(LongReal.Sinh(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Sinh(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf SinhInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Sinh(a) * Cos(b), Cosh(a) * Sin(b));
	}

	private static T Sqrt(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Sqrt(r)),
		decimal d => T.CreateTruncating(Math.Sqrt(d)),
		LongReal lr => T.CreateTruncating(LongReal.Sqrt(lr)),
		LongDecimal lm => T.CreateTruncating(LongDecimal.Sqrt(lm)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	internal static TSelf SqrtInterface(TSelf value)
	{
		if (TSelf.IsZero(value))
			return TSelf.Zero;
		else if (TSelf.IsPositiveInfinity(value))
			return TSelf.Creator(value.Real switch
			{
				double r => (T)(object)double.PositiveInfinity,
				decimal d => throw new OverflowException("Тип decimal не поддерживает бесконечность и неопределенность."),
				LongReal lr => (T)(object)LongReal.PositiveInfinity,
				LongDecimal lm => (T)(object)LongReal.PositiveInfinity,
				_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
					+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
			}, T.Zero);
		else if (TSelf.IsNegativeInfinity(value))
			return TSelf.Creator(value.Real switch
			{
				double r => (T)(object)double.NaN,
				decimal d => throw new OverflowException("Тип decimal не поддерживает бесконечность и неопределенность."),
				LongReal lr => (T)(object)LongReal.NaN,
				LongDecimal lm => (T)(object)LongReal.NaN,
				_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
					+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
			}, T.Zero);
		return FromPolarCoordinatesInterface(Sqrt(value.MagnitudeInterface), value.PhaseInterface / (T.One + T.One));
	}

	internal static TSelf SquareInterface(TSelf value) =>
		FromPolarCoordinatesInterface(value.MagnitudeInterface * value.MagnitudeInterface, value.PhaseInterface * (T.One + T.One));
	internal static TSelf TanInterface(TSelf value) => SinInterface(value) / CosInterface(value);
	internal static TSelf TanhInterface(TSelf value) => SinhInterface(value) / CoshInterface(value);
	internal string ToStringInterface(string? format) =>
		'(' + Real.ToString(format, CultureInfo.CurrentCulture) + ", "
		+ Imaginary.ToString(format, CultureInfo.CurrentCulture) + ')';
	internal string ToStringInterface(IFormatProvider? provider) =>
		'(' + Real.ToString(null, provider) + ", " + Imaginary.ToString(null, provider) + ')';
	internal string ToStringInterface() => string.Format(CultureInfo.CurrentCulture, "({0}, {1})", Real, Imaginary);
	internal string ToStringInterface(string? format, IFormatProvider? provider) =>
		'(' + Real.ToString(format, provider) + ", " + Imaginary.ToString(format, provider) + ')';

	public static TSelf operator -(IComplexNumber<T, TSelf> value) => TSelf.Creator(-value.Real, -value.Imaginary);
	public static TSelf operator +(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right) =>
		TSelf.Creator(left.Real + right.Real, left.Imaginary + right.Imaginary);
	public static TSelf operator -(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right) =>
		TSelf.Creator(left.Real - right.Real, left.Imaginary - right.Imaginary);

	public static TSelf operator *(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right)
	{
		// Multiplication:  (a + bi)(c + di) = (ac - bd) + (bc + ad)i
		var resultReal = left.Real * right.Real - left.Imaginary * right.Imaginary;
		var resultImaginary = left.Imaginary * right.Real + left.Real * right.Imaginary;
		return TSelf.Creator(resultReal, resultImaginary);
	}

	public static TSelf operator /(IComplexNumber<T, TSelf> left, T right) =>
		TSelf.Creator(left.Real / right, left.Imaginary / right);

	public static TSelf operator /(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right)
	{
		// Division : Smith's formula.
		var a = left.Real;
		var b = left.Imaginary;
		var c = right.Real;
		var d = right.Imaginary;
		if (Abs(d) < Abs(c))
		{
			var doc = d / c;
			return TSelf.Creator((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
		}
		else
		{
			var cod = c / d;
			return TSelf.Creator((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
		}
	}

	public static virtual bool operator ==(TSelf left, TSelf right) =>
		left.Real == right.Real && left.Imaginary == right.Imaginary;
	public static virtual bool operator !=(TSelf left, TSelf right) =>
		left.Real != right.Real || left.Imaginary != right.Imaginary;
}
