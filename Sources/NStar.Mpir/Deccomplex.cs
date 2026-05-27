namespace NStar.Mpir;
#pragma warning disable CA2260 // Используйте правильный параметр типа
public readonly struct Deccomplex(decimal real, decimal imaginary) : IComplexNumber<decimal, Deccomplex>
#pragma warning restore CA2260 // Используйте правильный параметр типа
{
	private const string NoComparisons = "Ошибка, операции \"больше\" и \"меньше\" не определены для комплексных чисел.";

	public static Deccomplex AdditiveIdentity => Zero;
	static Func<decimal, decimal, Deccomplex> IComplexNumber<decimal, Deccomplex>.Creator =>
		(real, imaginary) => new(real, imaginary);
	/// <inheritdoc cref="IFloatingPointConstants{double}.E"/>
	public static Deccomplex E => new(2.7182818284590452353602874714m, 0m);
	public decimal Imaginary { get; } = imaginary;
	public static Deccomplex MultiplicativeIdentity => One;
	/// <inheritdoc cref="ISignedNumber{double}.NegativeOne"/>
	public static Deccomplex NegativeOne => new(-1m, 0m);
	public static Deccomplex One => new(1m, 0m);
	/// <inheritdoc cref="IFloatingPointConstants{double}.Pi"/>
	public static Deccomplex Pi => new(3.1415926535897932384626433833m, 0m);
	public static int Radix => 10;
	public decimal Real { get; } = real;
	/// <inheritdoc cref="IFloatingPointConstants{double}.Tau"/>
	public static Deccomplex Tau => new(6.2831853071795864769252867666m, 0m);
	public static Deccomplex Zero => new(0m, 0m);

	/// <summary>
	/// Computes the absolute of this number.
	/// </summary>
	/// <returns>The absolute of this number.</returns>
	public decimal Abs() => IComplexNumber<decimal, Deccomplex>.AbsInterface(this);
	/// <inheritdoc cref="INumberBase{Complex}.Abs"/>
	public static decimal Abs(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AbsInterface(value);
	static Deccomplex INumberBase<Deccomplex>.Abs(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AbsInterface(value);

	/// <summary>
	/// Вычисляет арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - Pi &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public Deccomplex Acos() => IComplexNumber<decimal, Deccomplex>.AcosInterface(this);

	/// <summary>
	/// Вычисляет арккосинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - Pi &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public static Deccomplex Acos(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AcosInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля, для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - гиперболический арккосинус данного числа.
	/// </returns>
	public Deccomplex Acosh() => IComplexNumber<decimal, Deccomplex>.AcoshInterface(this);

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
	public static Deccomplex Acosh(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AcoshInterface(value);

	/// <summary>
	/// Вычисляет арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арксинус данного числа.
	/// </returns>
	public Deccomplex Asin() => IComplexNumber<decimal, Deccomplex>.AsinInterface(this);

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
	public static Deccomplex Asin(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AsinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арксинус данного числа.
	/// </returns>
	public Deccomplex Asinh() => IComplexNumber<decimal, Deccomplex>.AsinhInterface(this);

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
	public static Deccomplex Asinh(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AsinhInterface(value);

	/// <summary>
	/// Вычисляет арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арктангенс данного числа.
	/// </returns>
	public Deccomplex Atan() => IComplexNumber<decimal, Deccomplex>.AtanInterface(this);

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
	public static Deccomplex Atan(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AtanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public Deccomplex Atanh() => IComplexNumber<decimal, Deccomplex>.AtanhInterface(this);

	/// <summary>
	/// Вычисляет гиперболический арктангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public static Deccomplex Atanh(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.AtanhInterface(value);

	public int CompareTo(object? obj) => throw new NotSupportedException(NoComparisons);
	public int CompareTo(Deccomplex other) => throw new NotSupportedException(NoComparisons);

	/// <summary>
	/// Вычисляет комплексное сопряжение данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - комплексное сопряжение данного числа.
	/// </returns>
	public Deccomplex Conjugate() => IComplexNumber<decimal, Deccomplex>.ConjugateInterface(this);

	/// <summary>
	/// Вычисляет комплексное сопряжение указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - комплексное сопряжение данного числа.
	/// </returns>
	public static Deccomplex Conjugate(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.ConjugateInterface(value);

	/// <summary>
	/// Вычисляет косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - косинус данного числа.
	/// </returns>
	public Deccomplex Cos() => IComplexNumber<decimal, Deccomplex>.CosInterface(this);

	/// <summary>
	/// Вычисляет косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - косинус <paramref name="value"/>.
	/// </returns>
	public static Deccomplex Cos(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.CosInterface(value);

	/// <summary>
	/// Вычисляет гиперболический косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус данного числа.
	/// </returns>
	public Deccomplex Cosh() => IComplexNumber<decimal, Deccomplex>.CoshInterface(this);

	/// <summary>
	/// Вычисляет гиперболический косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус <paramref name="value"/>.
	/// </returns>
	public static Deccomplex Cosh(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.CosInterface(value);

	public bool Equals(Deccomplex other) => Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		((IComplexNumber<decimal, Deccomplex>)this).EqualsInterface(obj);

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
	public Deccomplex Exp() => IComplexNumber<decimal, Deccomplex>.ExpInterface(this);

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
	public static Deccomplex Exp(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.ExpInterface(value);

	public override int GetHashCode() => ((IComplexNumber<decimal, Deccomplex>)this).GetHashCodeInterface();
	public static bool IsCanonical(Deccomplex value) => true;
	public static bool IsComplexNumber(Deccomplex value) => true;
#pragma warning disable IDE0079 // Удалить ненужное подавление
	public static bool IsEvenInteger(Deccomplex value) => decimal.IsEvenInteger(value.Real) && value.Imaginary == 0m;
	public static bool IsFinite(Deccomplex value) => true;
	public static bool IsImaginaryNumber(Deccomplex value) => value.Imaginary != 0m;
	public static bool IsInfinity(Deccomplex value) => false;
	public static bool IsInteger(Deccomplex value) => decimal.IsInteger(value.Real) && value.Imaginary == 0m;
	public static bool IsNaN(Deccomplex value) => false;
	public static bool IsNegative(Deccomplex value) => value.Real < 0m && value.Imaginary == 0m;
	public static bool IsNegativeInfinity(Deccomplex value) => false;
	public static bool IsNormal(Deccomplex value) => true;
	public static bool IsOddInteger(Deccomplex value) => decimal.IsOddInteger(value.Real) && value.Imaginary == 0m;
	public static bool IsPositive(Deccomplex value) => value.Real > 0m && value.Imaginary == 0m;
	public static bool IsPositiveInfinity(Deccomplex value) => false;
	public static bool IsRealNumber(Deccomplex value) => value.Imaginary == 0m;
	public static bool IsSubnormal(Deccomplex value) => false;
	public static bool IsZero(Deccomplex value) => value.Real == 0m && value.Imaginary == 0m;

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
	public Deccomplex Ln() => IComplexNumber<decimal, Deccomplex>.LogInterface(this);

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
	public static Deccomplex Ln(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.LogInterface(value);

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
	public Deccomplex Log() => IComplexNumber<decimal, Deccomplex>.LogInterface(this);

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
	public Deccomplex Log(decimal @base) => IComplexNumber<decimal, Deccomplex>.LogInterface(this, @base);

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
	public static Deccomplex Log(Deccomplex value, decimal @base) =>
		IComplexNumber<decimal, Deccomplex>.LogInterface(value, @base);

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
	public Deccomplex Log2() => IComplexNumber<decimal, Deccomplex>.LogInterface(this, 2m);

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
	public static Deccomplex Log2(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.LogInterface(value, 2m);

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
	public Deccomplex Log10() => IComplexNumber<decimal, Deccomplex>.LogInterface(this, 10m);

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
	public static Deccomplex Log10(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.LogInterface(value, 10m);

	public static Deccomplex MaxMagnitude(Deccomplex x, Deccomplex y) => Abs(MaxMagnitudeNumber(x, y));
	public static Deccomplex MaxMagnitudeNumber(Deccomplex x, Deccomplex y) => Abs(x) > Abs(y) ? x : y;
	public static Deccomplex MinMagnitude(Deccomplex x, Deccomplex y) => Abs(MinMagnitudeNumber(x, y));
	public static Deccomplex MinMagnitudeNumber(Deccomplex x, Deccomplex y) => Abs(x) < Abs(y) ? x : y;
	public static Deccomplex Parse(string s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static Deccomplex Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);
	public static Deccomplex Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static Deccomplex Parse(string s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);

	/// <summary>
	/// Возводит данное число в указанную степень.
	/// </summary>
	/// <param name="exponent">Показатель степени, в которую нужно возвести данное число.</param>
	/// <returns>Результат возведения в степень.</returns>
	public Deccomplex Power(int exponent) => IComplexNumber<decimal, Deccomplex>.PowInterface(this, exponent);

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
	public Deccomplex Power(decimal exponent) => IComplexNumber<decimal, Deccomplex>.PowInterface(this, exponent);

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
	public static Deccomplex Power(Deccomplex @base, decimal exponent) => @base.Power(exponent);

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
	public Deccomplex Power(Deccomplex exponent) => IComplexNumber<decimal, Deccomplex>.PowInterface(this, exponent);

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
	public static Deccomplex Power(Deccomplex @base, Deccomplex exponent) => @base.Power(exponent);

	/// <summary>
	/// Вычисляет число, обратное данному (1 / x).
	/// </summary>
	/// <returns>
	/// Для нуля - плюс бесконечность;<br />
	/// для плюс бесконечности или для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - число, обратное данному (1 / x).
	/// </returns>
	public Deccomplex Reciproc() => IComplexNumber<decimal, Deccomplex>.ReciprocInterface(this);

	/// <summary>
	/// Вычисляет синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - синус данного числа.
	/// </returns>
	public Deccomplex Sin() => IComplexNumber<decimal, Deccomplex>.SinInterface(this);

	/// <summary>
	/// Вычисляет синус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - синус <paramref name="value"/>.
	/// </returns>
	public static Deccomplex Sin(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.SinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус данного числа.
	/// </returns>
	public Deccomplex Sinh() => IComplexNumber<decimal, Deccomplex>.SinhInterface(this);

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
	public static Deccomplex Sinh(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.SinhInterface(value);

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
	public Deccomplex Sqrt() => IComplexNumber<decimal, Deccomplex>.SqrtInterface(this);

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
	public static Deccomplex Sqrt(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.SqrtInterface(value);

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
	public Deccomplex Square() => IComplexNumber<decimal, Deccomplex>.SquareInterface(this);

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
	public static Deccomplex Square(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.SquareInterface(value);

	/// <summary>
	/// Вычисляет тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс данного числа.
	/// </returns>
	public Deccomplex Tan() => IComplexNumber<decimal, Deccomplex>.TanInterface(this);

	/// <summary>
	/// Вычисляет тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс <paramref name="value"/>.
	/// </returns>
	public static Deccomplex Tan(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.TanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	public Deccomplex Tanh() => IComplexNumber<decimal, Deccomplex>.TanhInterface(this);

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
	public static Deccomplex Tanh(Deccomplex value) => IComplexNumber<decimal, Deccomplex>.TanhInterface(value);

	public override string ToString() => ((IComplexNumber<decimal, Deccomplex>)this).ToStringInterface();
	public string ToString(IFormatProvider? formatProvider) =>
		((IComplexNumber<decimal, Deccomplex>)this).ToStringInterface(formatProvider);
	public string ToString(string? format) => ((IComplexNumber<decimal, Deccomplex>)this).ToStringInterface(format);
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		((IComplexNumber<decimal, Deccomplex>)this).ToStringInterface(format, formatProvider);

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out Deccomplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out Deccomplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out Deccomplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToChecked<TOther>(Deccomplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToSaturating<TOther>(Deccomplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToTruncating<TOther>(Deccomplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Deccomplex result)
	{
		if (System.Numerics.Complex.TryParse(s, style, provider, out var other))
		{
			result = other;
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Deccomplex result) => TryParse(s.AsSpan(), style, provider, out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Deccomplex result) =>
		TryParse(s, NumberStyles.None, provider, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Deccomplex result) => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
		((System.Numerics.Complex)this).TryFormat(destination, out charsWritten, format, provider);

	public static implicit operator Deccomplex(decimal value) => new(value, 0m);
	public static implicit operator Deccomplex(System.Numerics.Complex value) =>
		new((decimal)value.Real, (decimal)value.Imaginary);
	public static explicit operator System.Numerics.Complex(Deccomplex value) =>
		new((double)value.Real, (double)value.Imaginary);

	public static Deccomplex operator +(Deccomplex value) => value;
	public static Deccomplex operator -(Deccomplex value) => -(IComplexNumber<decimal, Deccomplex>)value;
	public static Deccomplex operator +(decimal left, Deccomplex right) => right + left;
	public static Deccomplex operator +(Deccomplex left, decimal right) =>
		new(left.Real + right, left.Imaginary);
	public static Deccomplex operator +(Deccomplex left, Deccomplex right) =>
		(IComplexNumber<decimal, Deccomplex>)left + right;
	public static Deccomplex operator -(decimal left, Deccomplex right) => -(right - left);
	public static Deccomplex operator -(Deccomplex left, decimal right) =>
		new(left.Real - right, left.Imaginary);
	public static Deccomplex operator -(Deccomplex left, Deccomplex right) =>
		(IComplexNumber<decimal, Deccomplex>)left - right;
	public static Deccomplex operator *(decimal left, Deccomplex right) => right * left;
	public static Deccomplex operator *(Deccomplex left, decimal right) =>
		new(left.Real * right, left.Imaginary * right);
	public static Deccomplex operator *(Deccomplex left, Deccomplex right) =>
		(IComplexNumber<decimal, Deccomplex>)left * right;
	public static Deccomplex operator /(Deccomplex left, decimal right) =>
		new(left.Real / right, left.Imaginary / right);
	public static Deccomplex operator /(Deccomplex left, Deccomplex right) =>
		(IComplexNumber<decimal, Deccomplex>)left / right;
	static Deccomplex IModulusOperators<Deccomplex, Deccomplex, Deccomplex>.operator %(Deccomplex left, Deccomplex right) =>
		throw new NotSupportedException("Ошибка, остаток от деления не определен для комплексных чисел.");
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static Deccomplex operator <<(Deccomplex x, int shiftAmount) =>
		x * IComplexNumber<decimal, Deccomplex>.Pow(2, shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static Deccomplex operator >>(Deccomplex x, int shiftAmount) =>
		x / IComplexNumber<decimal, Deccomplex>.Pow(2, shiftAmount);
	public static Deccomplex operator ++(Deccomplex value) => new(value.Real + 1m, value.Imaginary);
	public static Deccomplex operator --(Deccomplex value) => new(value.Real - 1m, value.Imaginary);
	public static bool operator ==(decimal left, Deccomplex right) => right == left;
	public static bool operator ==(Deccomplex left, decimal right) =>
		left.Real == right && left.Imaginary == 0m;
	public static bool operator ==(Deccomplex left, Deccomplex right) =>
		left.Real == right.Real && left.Imaginary == right.Imaginary;
	public static bool operator !=(decimal left, Deccomplex right) => right != left;
	public static bool operator !=(Deccomplex left, decimal right) =>
		left.Real != right || left.Imaginary != 0m;
	public static bool operator !=(Deccomplex left, Deccomplex right) =>
		left.Real != right.Real || left.Imaginary != right.Imaginary;
#pragma warning disable S3877
	static bool IComparisonOperators<Deccomplex, Deccomplex, bool>.operator >=(Deccomplex left, Deccomplex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<Deccomplex, Deccomplex, bool>.operator <=(Deccomplex left, Deccomplex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<Deccomplex, Deccomplex, bool>.operator >(Deccomplex left, Deccomplex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<Deccomplex, Deccomplex, bool>.operator <(Deccomplex left, Deccomplex right) =>
		throw new NotSupportedException(NoComparisons);
#pragma warning restore S3877
#pragma warning restore IDE0079 // Удалить ненужное подавление
}