namespace NStar.Mpir;

internal interface IComplexNumber<T, TSelf> : INumber<TSelf>
	where T : struct, IFloatingPoint<T> where TSelf : struct, IComplexNumber<T, TSelf>
{
	protected static abstract Func<T, T, TSelf> Creator { get; }
	public T Imaginary { get; }
	public static TSelf ImaginaryOne { get; } = TSelf.Creator(T.Zero, T.One);
	public T Magnitude => AbsInterface((TSelf)this);
	public static TSelf OneInterface { get; } = TSelf.Creator(T.One, T.Zero);
	public T Phase => Atan2(Imaginary, Real);

	private static T PositiveInfinity => T.Zero switch
	{
		double => T.CreateTruncating(double.PositiveInfinity),
		decimal => throw new OverflowException("Тип decimal не поддерживает бесконечность."),
		LongReal => T.CreateTruncating(LongReal.PositiveInfinity),
		LongDecimal => T.CreateTruncating(LongDecimal.PositiveInfinity),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public T Real { get; }

	private static ReadOnlySpan<decimal> SinTable
	{
		get
		{
			SinTableObj ??= new[]
			{
				0m, 0.0392598157590686090208033638m, 0.0784590957278449450329602460m, 0.1175373974578376441055682668m,
				0.1564344650402308690101053195m, 0.1950903220161282678482848685m, 0.2334453638559054117677444302m,
				0.2714404498650742533437874013m, 0.3090169943749474241022934172m, 0.3461170570774929764682149949m,
				0.3826834323650897717284599840m, 0.4186597375374280866755652051m, 0.4539904997395467915604083664m,
				0.4886212414969549474201908878m, 0.5224985647159488649878978802m, 0.5555702330196022247428308139m,
				0.5877852522924731291687059546m, 0.6190939493098339869415608562m, 0.6494480483301836557263207709m,
				0.6788007455329417413938555542m, 0.7071067811865475244008443621m, 0.7343225094356855356361262222m,
				0.7604059656000309381745943648m, 0.7853169308807449274703402789m, 0.8090169943749474241022934172m,
				0.8314696123025452370787883776m, 0.8526401643540922215193834581m, 0.8724960070727971145251610992m,
				0.8910065241883678623597095714m, 0.9081431738250812992580858366m, 0.9238795325112867561281831894m,
				0.9381913359224841344523397267m, 0.9510565162951535721164393334m, 0.9624552364536472876302664052m,
				0.9723699203976766018336458341m, 0.9807852804032304491261822361m, 0.9876883405951377261900402477m,
				0.9930684569549262956374372478m, 0.9969173337331279761977734087m, 0.9992290362407229347371262603m, 1m
			};
			return new ReadOnlySpan<decimal>((decimal[]?)SinTableObj);
		}
	}

	private static object? SinTableObj { get; set; }
	public static TSelf ZeroInterface { get; } = TSelf.Creator(T.Zero, T.Zero);

	private static T Abs(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Abs(r)),
		decimal d => T.CreateTruncating(Math.Abs(d)),
		LongReal lr => T.CreateTruncating(LongReal.Abs(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Abs(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static T AbsInterface(TSelf value)
	{
		if (T.IsInfinity(value.Real) || T.IsInfinity(value.Imaginary))
			return PositiveInfinity;
		// |value| == sqrt(a^2 + b^2)
		// sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2)
		// Using the above we can factor out the square of the larger component to dodge overflow.
		var c = T.Abs(value.Real);
		var d = T.Abs(value.Imaginary);
		if (c > d)
		{
			var r = d / c;
			return c * Sqrt(T.One + r * r);
		}
		else if (d == T.Zero)
			return c;
		else
		{
			var r = c / d;
			return d * Sqrt(T.One + r * r);
		}
	}

	public static TSelf AcosInterface(TSelf value) =>
		-ImaginaryOne * LogInterface(value + ImaginaryOne * SqrtInterface(OneInterface - value * value));
	public static TSelf AcoshInterface(TSelf value) =>
		LogInterface(value + SqrtInterface(SquareInterface(value) - TSelf.One));
	public static TSelf AsinInterface(TSelf value) =>
		-ImaginaryOne * LogInterface(ImaginaryOne * value + SqrtInterface(OneInterface - value * value));
	public static TSelf AsinhInterface(TSelf value) => LogInterface(value + SqrtInterface(SquareInterface(value) + TSelf.One));

	private static decimal Atan(decimal value)
	{
		if (Math.Abs(value) > 1000000m)
		{
			var atanOther = Atan(1m / value);
			if (value > 0m)
				return 1.5707963267948966192313216916m - atanOther;
			return -(1.5707963267948966192313216916m + atanOther);
		}
		var doubleAtan = (decimal)Math.Atan((double)value);
		decimal rowItem;
		do
		{
			var tuple = SinCos(doubleAtan);
			var item = tuple.Item1;
			var item2 = tuple.Item2;
			rowItem = item2 * (item2 * value - item);
			doubleAtan += rowItem;
		} while (Math.Abs(rowItem) > 1E-15m);
		return doubleAtan;
	}

	public static TSelf AtanInterface(TSelf value)
	{
		var Two = TSelf.Creator(T.One + T.One, T.Zero);
		return ImaginaryOne / Two * (LogInterface(OneInterface - ImaginaryOne * value)
			- LogInterface(OneInterface + ImaginaryOne * value));
	}

	public static TSelf AtanhInterface(TSelf value) =>
		LogInterface((TSelf.One + value) / (TSelf.One - value)) / (TSelf.One + TSelf.One);

	private static decimal Atan2(decimal y, decimal x)
	{
		if (x == 0m && y == 0m)
			throw new ArgumentException("Ошибка! Это направление не определено,"
				+ " а тип decimal не поддерживает неопределенность.");
		if (Math.Abs(x) < Math.Abs(y))
		{
			if (x / y == 0m)
				return y <= 0m ? -1.5707963267948966192313216916m : 1.5707963267948966192313216916m;
			var atan = Atan(y / x);
			if (x >= 0m)
				return atan;
			else if (y > 0m)
				return 3.1415926535897932384626433833m + atan;
			else
				return atan - 3.1415926535897932384626433833m;
		}
		else if (y / x == 0m)
			return x > 0m ? 0m : 3.1415926535897932384626433833m;
		else
		{
			var atan = Atan(x / y);
			return y > 0m ? 1.5707963267948966192313216916m - atan : -atan - 1.5707963267948966192313216916m;
		}
	}

	private static T Atan2(T yValue, T xValue) => (yValue, xValue) switch
	{
		(double yValue2, double xValue2) => T.CreateTruncating(Math.Atan2(yValue2, xValue2)),
		(decimal yValue2, decimal xValue2) => T.CreateTruncating(Atan2(yValue2, xValue2)),
		(LongReal yValue2, LongReal xValue2) => T.CreateTruncating(LongReal.Atan2(yValue2, xValue2)),
		(LongDecimal yValue2, LongDecimal xValue2) => T.CreateTruncating(LongDecimal.Atan2(yValue2, xValue2)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf ConjugateInterface(TSelf value) => TSelf.Creator(value.Real, -value.Imaginary);

	private static decimal Cos(decimal angle) => SinCos(angle).Item2;

	private static T Cos(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Cos(r)),
		decimal d => T.CreateTruncating(Cos(d)),
		LongReal lr => T.CreateTruncating(LongReal.Cos(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Cos(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf CosInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Cos(a) * Cosh(b), -(Sin(a) * Sinh(b)));
	}

	private static decimal Cosh(decimal value)
	{
		var exp = Exp(Math.Abs(value));
		if (exp == 0m)
			throw new OverflowException("Тип decimal не поддерживает бесконечность.");
		return 0.5m * (exp + 1m / exp);
	}

	private static T Cosh(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Cosh(r)),
		decimal d => T.CreateTruncating(Cosh(d)),
		LongReal lr => T.CreateTruncating(LongReal.Cosh(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Cosh(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf CoshInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Cosh(a) * Cos(b), Sinh(a) * Sin(b));
	}

	public bool EqualsInterface(object? obj) => obj switch
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

	public bool EqualsInterface(TSelf value) => Real.Equals(value.Real) && Imaginary.Equals(value.Imaginary);

	private static decimal Exp(decimal value)
	{
		if (value > 66.542129333754749704054283660m)
			throw new OverflowException("Экспонента этого числа не умещается в decimal!");
		if (value < -66.774967696827324836521752186m)
			return 0m;
		if (value == 0m)
			return 1m;
		if (value == 1m)
			return 2.71828182845904523536028747m;
		var negative = value < 0m;
		if (negative)
			value = -value;
		var floor = (int)decimal.Floor(value / 2.3025850929940456840179914547m + 0.5m);
		value -= floor * 2.3025850929940456840179914547m;
		var floor2 = (int)decimal.Floor(value / 0.0953101798043248600439521233m);
		value -= floor2 * 0.0953101798043248600439521233m;
		var exp = 1m;
		var rowItem = value;
		for (var i = 1; rowItem > 0m; rowItem *= value / i)
		{
			exp += rowItem;
			i++;
		}
		exp = exp * Pow(10m, floor) * Pow(1.1m, floor2);
		if (negative)
			return 1.0m / exp;
		return exp;
	}

	private static T Exp(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Exp(r)),
		decimal d => T.CreateTruncating(Exp(d)),
		LongReal lr => T.CreateTruncating(LongReal.Exp(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Exp(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf ExpInterface(TSelf value)
	{
		var temp_factor = Exp(value.Real);
		var result_re = temp_factor * Cos(value.Imaginary);
		var result_im = temp_factor * Sin(value.Imaginary);
		return TSelf.Creator(result_re, result_im);
	}

	public static TSelf FromPolarCoordinatesInterface(T magnitude, T phase) =>
		TSelf.Creator(magnitude * Cos(phase), magnitude * Sin(phase));

	public int GetHashCodeInterface()
	{
		var n1 = 99999997;
		var hash_real = Real.GetHashCode() % n1;
		var hash_imaginary = Imaginary.GetHashCode();
		var final_hashcode = hash_real ^ hash_imaginary;
		return final_hashcode;
	}

	private static (decimal, int) GetSinCosRange(decimal value)
	{
		var negative = value < 0m;
		value = Math.Abs(value);
		var truncated = decimal.Truncate(value / 6.2831853071795864769252867666m);
		if (Math.Abs(truncated) > 2m)
		{
			var truncated2 = 0m;
			var truncated3 = 0m;
			if (Math.Abs(truncated) > 1E+9m)
			{
				truncated2 = decimal.Truncate(truncated * 1E-9m);
				truncated -= truncated2 * 1E+9m;
				if (Math.Abs(truncated2) > 1E+9m)
				{
					truncated3 = decimal.Truncate(truncated2 * 1E-9m);
					truncated2 -= truncated3 * 1E+9m;
					value -= truncated3 * 6.283185307179586476m * 1E+18m;
				}
				value -= truncated2 * 6.283185307179586476m * 1E+9m;
				if (truncated3 != 0m)
					value -= truncated3 * 0.925286766559005768m;
				truncated2 *= 1E-9m;
				truncated3 *= 1E-18m;
			}
			value -= truncated * 6.283185307179586476m;
			truncated *= 1E-18m;
			if (truncated2 != 0m)
			{
				value -= truncated2 * 0.925286766559005768m;
				truncated2 *= 1E-18m;
			}
			value -= truncated * 0.925286766559005768m;
			truncated *= 1E-18m;
			if (truncated3 != 0m)
			{
				value -= truncated3 * 0.394338798750211641m;
				truncated3 *= 1E-18m;
			}
			if (truncated2 != 0m)
				value -= truncated2 * 0.394338798750211641m;
			if (truncated3 != 0m)
				value -= truncated3 * 0.9498m;
			value -= truncated * 0.394338798750211641m;
		}
		value = value * 25.464790894703253723021402140m % 160m;
		var index = (int)(value + 0.5m);
		value = (value - index) * 0.0392699081698724154807830423m;
		if (negative)
		{
			index = 160 - index;
			value = -value;
		}
		index %= 160;
		return (value, index);
	}

	private static decimal Log(decimal value)
	{
		if (value == 1m)
			return 0m;
		else if (value == 0m)
			throw new OverflowException("Логарифм нуля - минус бесконечность, а decimal ее не поддерживает.");
		else if (value < 0m)
			throw new ArgumentOutOfRangeException(nameof(value), "Логарифм отрицательного числа не определен.");
		var log = (decimal)Math.Log((double)value);
		var exp = Exp(log);
		return log + (value - exp) / exp;
	}

	private static T Log(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Log(r)),
		decimal d => T.CreateTruncating(Log(d)),
		LongReal lr => T.CreateTruncating(LongReal.Ln(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Ln(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf LogInterface(TSelf value) => TSelf.Creator(Log(AbsInterface(value)), Atan2(value.Imaginary, value.Real));
	public static TSelf LogInterface(TSelf value, T baseValue) => LogInterface(value) / Log(baseValue);

	private static decimal Pow(decimal value, decimal exponent)
	{
		var @int = (int)exponent;
		if (exponent == @int)
			return Pow(value, @int);
		if (value == 0m)
			return exponent < 0m ? throw new DivideByZeroException() : 0m;
		if (value == 1m)
			return 1m;
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0m);
		var truncated = decimal.Truncate(exponent);
		if (truncated == @int)
			return Pow(value, @int) * Exp((exponent - truncated) * Log(value));
		return Exp(exponent * Log(value));
	}

	protected static decimal Pow(decimal value, int exponent) => exponent switch
	{
		2 => value * value,
		3 => value * value * value,
		_ => PowInternal(value, exponent),
	};

	private static T Pow(T value, T power) => (value, power) switch
	{
		(double value2, double power2) => T.CreateTruncating(Math.Pow(value2, power2)),
		(decimal value2, decimal power2) => T.CreateTruncating(Pow(value2, power2)),
		(LongReal value2, LongReal power2) => T.CreateTruncating(LongReal.Power(value2, power2)),
		(LongDecimal value2, LongDecimal power2) => T.CreateTruncating(LongDecimal.Power(value2, power2)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf PowInterface(TSelf value, int power) => PowInterface(value, TSelf.Creator(T.CreateTruncating(power), T.Zero));
	public static TSelf PowInterface(TSelf value, T power) => PowInterface(value, TSelf.Creator(power, T.Zero));

	public static TSelf PowInterface(TSelf value, TSelf power)
	{
		if (power == ZeroInterface)
			return OneInterface;
		if (value == ZeroInterface)
			return ZeroInterface;
		if (value.Imaginary == T.Zero && power.Imaginary == T.Zero)
			return TSelf.Creator(Pow(value.Real, power.Real), T.Zero);
		var a = value.Real;
		var b = value.Imaginary;
		var c = power.Real;
		var d = power.Imaginary;
		var rho = AbsInterface(value);
		var theta = Atan2(b, a);
		var newRho = c * theta + d * Log(rho);
		var t = Pow(rho, c) * Pow(T.E, -d * theta);
		return TSelf.Creator(t * Cos(newRho), t * Sin(newRho));
	}

	private static decimal PowInternal(decimal @base, int exponent)
	{
		switch (exponent)
		{
			case 0:
			return 1m;
			case 1:
			return @base;
			case 4:
			@base *= @base;
			return @base * @base;
			case 5:
			var square = @base * @base;
			return square * square * @base;
			default:
			if (@base == 1m)
				return @base;
			var negative = false;
			if (exponent < 0)
			{
				negative = true;
				exponent = -exponent;
			}
			if (@base == 0m)
				return negative ? throw new DivideByZeroException() : 0.0m;
			if (@base == 1.0m)
				return @base;
			if (@base == -1.0m)
				return (exponent & 1) == 0 ? 1.0m : -1.0m;
			if (negative)
				@base = 1.0m / @base;
			var result = 1m;
			for (var i = BitsPerInt - int.LeadingZeroCount(exponent) - 2; i >= 0; i--)
			{
				result *= result;
				if ((exponent & 1u << i) != 0)
					result *= @base;
			}
			return result;
		}
	}

	public static TSelf ReciprocInterface(TSelf value) =>
		value.Imaginary == T.Zero ? TSelf.Creator(T.One / value.Real, T.Zero) : OneInterface / value;

	private static decimal Sin(decimal angle) => SinCos(angle).Item1;

	private static T Sin(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Sin(r)),
		decimal d => T.CreateTruncating(Sin(d)),
		LongReal lr => T.CreateTruncating(LongReal.Sin(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Sin(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf SinInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Sin(a) * Cosh(b), Cos(a) * Sinh(b));
	}

	private static (decimal, decimal) SinCos(decimal angle)
	{
		if (angle == 0m)
			return (0m, 1m);
		(angle, var range) = GetSinCosRange(angle);
		SinCosSmall(angle, out var sin, out var cos);
		var quotient = (range + 20) / 40;
		var value = range - 40 * quotient;
		quotient &= 3;
		var (abs, sign) = value < 0 ? (-value, -1) : (value, 1);
		decimal tableSin;
		decimal signed;
		switch (quotient)
		{
			case 0:
			tableSin = SinTable[40 - abs];
			signed = sign * SinTable[abs];
			break;
			case 1:
			tableSin = -sign * SinTable[abs];
			signed = SinTable[40 - abs];
			break;
			case 2:
			tableSin = -SinTable[40 - abs];
			signed = -sign * SinTable[abs];
			break;
			case 3:
			tableSin = sign * SinTable[abs];
			signed = -SinTable[40 - abs];
			break;
			default:
			throw new InvalidOperationException();
		}

		return (signed * cos + tableSin * sin, tableSin * cos - signed * sin);
	}

	private static void SinCosSmall(decimal value, out decimal sin, out decimal cos)
	{
		var sinRowItem = value;
		value *= value;
		var cosRowItem = -0.5m * value;
		value = -value;
		sin = sinRowItem;
		cos = 1m + cosRowItem;
		var i = 2;
		do
		{
			sinRowItem *= value / (i++ * i++);
			sin += sinRowItem;
			cosRowItem *= value / (i * (i - 1));
			cos += cosRowItem;
		} while (Math.Abs(cosRowItem) > 0m);
	}

	private static decimal Sinh(decimal value)
	{
		if (value == 0m)
			return 0m;
		else if (value > 0m)
		{
			var exp = Exp(value);
			return 0.5m * (exp - 1m / exp);
		}
		else
		{
			var exp = Exp(-value);
			return 0.5m * (-exp + 1m / exp);
		}
	}

	private static T Sinh(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Sinh(r)),
		decimal d => T.CreateTruncating(Sinh(d)),
		LongReal lr => T.CreateTruncating(LongReal.Sinh(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Sinh(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf SinhInterface(TSelf value)
	{
		var a = value.Real;
		var b = value.Imaginary;
		return TSelf.Creator(Sinh(a) * Cos(b), Cosh(a) * Sin(b));
	}

	private static decimal Sqrt(decimal value)
	{
		if (value < 0) throw new ArgumentOutOfRangeException(nameof(value),
			"Квадратный корень из отрицательного числа не определен.");
		var guess = (decimal)Math.Sqrt((double)value); // Начальное приближение
		while (true)
		{
			var previous = guess;
			guess = (previous + value / previous) / 2;
			if (Math.Abs(previous - guess) < 1e-28m) // Условие сходимости
				return guess;
		}
	}

	private static T Sqrt(T value) => value switch
	{
		double r => T.CreateTruncating(Math.Sqrt(r)),
		decimal d => T.CreateTruncating(Sqrt(d)),
		LongReal lr => T.CreateTruncating(LongReal.Sqrt(lr)),
		LongDecimal ld => T.CreateTruncating(LongDecimal.Sqrt(ld)),
		_ => throw new InvalidCastException("Поддерживаются типы double, decimal, "
			+ nameof(LongReal) + " и " + nameof(LongDecimal) + '.'),
	};

	public static TSelf SqrtInterface(TSelf value) =>
		value.Imaginary == T.Zero ? TSelf.Creator(Sqrt(value.Real), T.Zero)
		: FromPolarCoordinatesInterface(Sqrt(value.Magnitude), value.Phase / (T.One + T.One));
	public static TSelf SquareInterface(TSelf value) =>
		FromPolarCoordinatesInterface(Sqrt(value.Magnitude), value.Phase * (T.One + T.One));
	public static TSelf TanInterface(TSelf value) => SinInterface(value) / CosInterface(value);
	public static TSelf TanhInterface(TSelf value) => SinhInterface(value) / CoshInterface(value);
	public string ToStringInterface(string? format) =>
		'(' + Real.ToString(format, CultureInfo.CurrentCulture) + ", "
		+ Imaginary.ToString(format, CultureInfo.CurrentCulture) + ')';
	public string ToStringInterface(IFormatProvider? provider) =>
		'(' + Real.ToString(null, provider) + ", " + Imaginary.ToString(null, provider) + ')';
	public string ToStringInterface() => string.Format(CultureInfo.CurrentCulture, "({0}, {1})", Real, Imaginary);
	public string ToStringInterface(string? format, IFormatProvider? provider) =>
		'(' + Real.ToString(format, provider) + ", " + Imaginary.ToString(format, provider) + ')';

	public static TSelf operator -(IComplexNumber<T, TSelf> value) => TSelf.Creator(-value.Real, -value.Imaginary);
	public static TSelf operator +(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right) =>
		TSelf.Creator(left.Real + right.Real, left.Imaginary + right.Imaginary);
	public static TSelf operator -(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right) =>
		TSelf.Creator(left.Real - right.Real, left.Imaginary - right.Imaginary);

	public static TSelf operator *(IComplexNumber<T, TSelf> left, IComplexNumber<T, TSelf> right)
	{
		// Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
		var result_Realpart = left.Real * right.Real - left.Imaginary * right.Imaginary;
		var result_Imaginarypart = left.Imaginary * right.Real + left.Real * right.Imaginary;
		return TSelf.Creator(result_Realpart, result_Imaginarypart);
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
