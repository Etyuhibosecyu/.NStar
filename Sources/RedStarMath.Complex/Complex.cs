global using NStar.Core;
global using NStar.Mpir;
global using System;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Numerics;
global using static NStar.Core.Extents;
global using static RedStarMath.Math;

namespace RedStarMath;
/// <summary>
/// Представляет базовое комплексное число в RedStarMath, как <see cref="System.Numerics.Complex"/>,
/// только с бо́льшим количеством методов, с действительной и мнимой частями типа <see langword="double"/>.
/// </summary>
/// <param name="real">Действительная часть комплексного числа.</param>
/// <param name="imaginary">Мнимая часть комплексного числа.</param>
#pragma warning disable CA2260 // Используйте правильный параметр типа
public readonly struct Complex(double real, double imaginary) : IComplexNumber<double, Complex>
#pragma warning restore CA2260 // Используйте правильный параметр типа
{
	private const string NoComparisons = "Ошибка, операции \"больше\" и \"меньше\" не определены для комплексных чисел.";

	public Complex(ReadOnlySpan<byte> bytes, int order) : this(bytes.Length < sizeof(double) << 1
		? throw new ArgumentException("Ошибка, слишком короткая последовательность байт для преобразования в этот тип.")
		: BitConverter.ToDouble(bytes), BitConverter.ToDouble(bytes[sizeof(double)..])) => _ = order;

	public static Complex AdditiveIdentity => Zero;
	static Func<double, double, Complex> IComplexNumber<double, Complex>.Creator => (real, imaginary) => new(real, imaginary);
	/// <inheritdoc cref="IFloatingPointConstants{double}.E"/>
	public static Complex E => new(double.E, 0d);
	public double Imaginary { get; } = imaginary;
	public double Magnitude => ((IComplexNumber<double, Complex>)this).MagnitudeInterface;
	public static Complex MultiplicativeIdentity => One;
	/// <inheritdoc cref="double.NaN"/>
	public static Complex NaN { get; } = new(double.NaN, 0d);
	/// <inheritdoc cref="double.NegativeInfinity"/>
	public static Complex NegativeInfinity { get; } = new(double.NegativeInfinity, 0d);
	/// <inheritdoc cref="ISignedNumber{double}.NegativeOne"/>
	public static Complex NegativeOne => new(-1d, 0d);
	public static Complex One => new(1d, 0d);
	public double Phase => ((IComplexNumber<double, Complex>)this).PhaseInterface;
	/// <inheritdoc cref="IFloatingPointConstants{double}.Pi"/>
	public static Complex Pi => new(double.Pi, 0d);
	/// <inheritdoc cref="double.PositiveInfinity"/>
	public static Complex PositiveInfinity { get; } = new(double.PositiveInfinity, 0d);
	public static int Radix => 2;
	public double Real { get; } = real;
	/// <inheritdoc cref="IFloatingPointConstants{double}.Tau"/>
	public static Complex Tau => new(double.Tau, 0d);
	public static Complex Zero => new(0d, 0d);

	/// <summary>
	/// Computes the absolute of this number.
	/// </summary>
	/// <returns>The absolute of this number.</returns>
	public double Abs() => IComplexNumber<double, Complex>.AbsInterface(this);
	/// <inheritdoc cref="INumberBase{Complex}.Abs"/>
	public static double Abs(Complex value) => IComplexNumber<double, Complex>.AbsInterface(value);
	static Complex INumberBase<Complex>.Abs(Complex value) => IComplexNumber<double, Complex>.AbsInterface(value);

	/// <summary>
	/// Вычисляет арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - Pi &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public Complex Acos() => IComplexNumber<double, Complex>.AcosInterface(this);

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
	public static Complex Acos(Complex value) => IComplexNumber<double, Complex>.AcosInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля, для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - гиперболический арккосинус данного числа.
	/// </returns>
	public Complex Acosh() => IComplexNumber<double, Complex>.AcoshInterface(this);

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
	public static Complex Acosh(Complex value) => IComplexNumber<double, Complex>.AcoshInterface(value);

	/// <summary>
	/// Вычисляет арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арксинус данного числа.
	/// </returns>
	public Complex Asin() => IComplexNumber<double, Complex>.AsinInterface(this);

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
	public static Complex Asin(Complex value) => IComplexNumber<double, Complex>.AsinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арксинус данного числа.
	/// </returns>
	public Complex Asinh() => IComplexNumber<double, Complex>.AsinhInterface(this);

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
	public static Complex Asinh(Complex value) => IComplexNumber<double, Complex>.AsinhInterface(value);

	/// <summary>
	/// Вычисляет арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арктангенс данного числа.
	/// </returns>
	public Complex Atan() => IComplexNumber<double, Complex>.AtanInterface(this);

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
	public static Complex Atan(Complex value) => IComplexNumber<double, Complex>.AtanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public Complex Atanh() => IComplexNumber<double, Complex>.AtanhInterface(this);

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
	public static Complex Atanh(Complex value) => IComplexNumber<double, Complex>.AtanhInterface(value);

	public int CompareTo(object? obj) => throw new NotSupportedException(NoComparisons);
	public int CompareTo(Complex other) => throw new NotSupportedException(NoComparisons);

	/// <summary>
	/// Вычисляет комплексное сопряжение данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - комплексное сопряжение данного числа.
	/// </returns>
	public Complex Conjugate() => IComplexNumber<double, Complex>.ConjugateInterface(this);

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
	public static Complex Conjugate(Complex value) => IComplexNumber<double, Complex>.ConjugateInterface(value);

	/// <summary>
	/// Вычисляет косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - косинус данного числа.
	/// </returns>
	public Complex Cos() => IComplexNumber<double, Complex>.CosInterface(this);

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
	public static Complex Cos(Complex value) => IComplexNumber<double, Complex>.CosInterface(value);

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
	public Complex Cosh() => IComplexNumber<double, Complex>.CoshInterface(this);

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
	public static Complex Cosh(Complex value) => IComplexNumber<double, Complex>.CosInterface(value);

	public bool Equals(Complex other) => Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		((IComplexNumber<double, Complex>)this).EqualsInterface(obj);

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
	public Complex Exp() => IComplexNumber<double, Complex>.ExpInterface(this);

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
	public static Complex Exp(Complex value) => IComplexNumber<double, Complex>.ExpInterface(value);

	public static Complex FromPolarCoordinates(double magnitude, double phase)
	{
		var (sin, cos) = phase.SinCos();
		return new Complex(magnitude * cos, magnitude * sin);
	}

	public override int GetHashCode() => ((IComplexNumber<double, Complex>)this).GetHashCodeInterface();
	public static bool IsCanonical(Complex value) => true;
	public static bool IsComplexNumber(Complex value) => true;
#pragma warning disable IDE0079 // Удалить ненужное подавление
#pragma warning disable S1244
	public static bool IsEvenInteger(Complex value) => double.IsEvenInteger(value.Real) && value.Imaginary == 0d;
	public static bool IsFinite(Complex value) => double.IsFinite(value.Real) && double.IsFinite(value.Imaginary);
	public static bool IsImaginaryNumber(Complex value) => value.Imaginary != 0d;
	public static bool IsInfinity(Complex value) => double.IsInfinity(value.Real) || double.IsInfinity(value.Imaginary);
	public static bool IsInteger(Complex value) => double.IsInteger(value.Real) && value.Imaginary == 0d;
	public static bool IsNaN(Complex value) => double.IsNaN(value.Real) || double.IsNaN(value.Imaginary);
	public static bool IsNegative(Complex value) => value.Real < 0d && value.Imaginary == 0d;
	public static bool IsNegativeInfinity(Complex value) =>
		double.IsNegativeInfinity(value.Real) && value.Imaginary == 0d;
	public static bool IsNormal(Complex value) => double.IsNormal(value.Real) && double.IsNormal(value.Imaginary);
	public static bool IsOddInteger(Complex value) => double.IsOddInteger(value.Real) && value.Imaginary == 0d;
	public static bool IsPositive(Complex value) => value.Real > 0d && value.Imaginary == 0d;
	public static bool IsPositiveInfinity(Complex value) =>
		double.IsPositiveInfinity(value.Real) && value.Imaginary == 0d;
	public static bool IsRealNumber(Complex value) => value.Imaginary == 0d;
	public static bool IsSubnormal(Complex value) => double.IsSubnormal(value.Real) || double.IsSubnormal(value.Imaginary);
	public static bool IsZero(Complex value) => value.Real == 0d && value.Imaginary == 0d;

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
	public Complex Ln() => IComplexNumber<double, Complex>.LogInterface(this);

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
	public static Complex Ln(Complex value) => IComplexNumber<double, Complex>.LogInterface(value);

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
	public Complex Log() => IComplexNumber<double, Complex>.LogInterface(this);

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
	public Complex Log(double @base) => IComplexNumber<double, Complex>.LogInterface(this, @base);

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
	public static Complex Log(Complex value, double @base) => IComplexNumber<double, Complex>.LogInterface(value, @base);

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
	public Complex Log2() => IComplexNumber<double, Complex>.LogInterface(this, 2d);

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
	public static Complex Log2(Complex value) => IComplexNumber<double, Complex>.LogInterface(value, 2d);

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
	public Complex Log10() => IComplexNumber<double, Complex>.LogInterface(this, 10d);

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
	public static Complex Log10(Complex value) => IComplexNumber<double, Complex>.LogInterface(value, 10d);

	public static Complex MaxMagnitude(Complex x, Complex y) => Abs(MaxMagnitudeNumber(x, y));
	public static Complex MaxMagnitudeNumber(Complex x, Complex y) => Abs(x) > Abs(y) ? x : y;
	public static Complex MinMagnitude(Complex x, Complex y) => Abs(MinMagnitudeNumber(x, y));
	public static Complex MinMagnitudeNumber(Complex x, Complex y) => Abs(x) < Abs(y) ? x : y;
	public static Complex Parse(string s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static Complex Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);
	public static Complex Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static Complex Parse(string s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);

	/// <summary>
	/// Возводит данное число в указанную степень.
	/// </summary>
	/// <param name="exponent">Показатель степени, в которую нужно возвести данное число.</param>
	/// <returns>Результат возведения в степень.</returns>
	public Complex Power(int exponent) => IComplexNumber<double, Complex>.PowInterface(this, exponent);

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
	public Complex Power(double exponent) => IComplexNumber<double, Complex>.PowInterface(this, exponent);

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
	public static Complex Power(Complex @base, double exponent) => @base.Power(exponent);

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
	public Complex Power(Complex exponent) => IComplexNumber<double, Complex>.PowInterface(this, exponent);

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
	public static Complex Power(Complex @base, Complex exponent) => @base.Power(exponent);

	/// <summary>
	/// Вычисляет число, обратное данному (1 / x).
	/// </summary>
	/// <returns>
	/// Для нуля - плюс бесконечность;<br />
	/// для плюс бесконечности или для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - число, обратное данному (1 / x).
	/// </returns>
	public Complex Reciproc() => IComplexNumber<double, Complex>.ReciprocInterface(this);

	/// <summary>
	/// Вычисляет синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - синус данного числа.
	/// </returns>
	public Complex Sin() => IComplexNumber<double, Complex>.SinInterface(this);

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
	public static Complex Sin(Complex value) => IComplexNumber<double, Complex>.SinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус данного числа.
	/// </returns>
	public Complex Sinh() => IComplexNumber<double, Complex>.SinhInterface(this);

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
	public static Complex Sinh(Complex value) => IComplexNumber<double, Complex>.SinhInterface(value);

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
	public Complex Sqrt() => IComplexNumber<double, Complex>.SqrtInterface(this);

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
	public static Complex Sqrt(Complex value) => IComplexNumber<double, Complex>.SqrtInterface(value);

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
	public Complex Square() => IComplexNumber<double, Complex>.SquareInterface(this);

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
	public static Complex Square(Complex value) => IComplexNumber<double, Complex>.SquareInterface(value);

	/// <summary>
	/// Вычисляет тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс данного числа.
	/// </returns>
	public Complex Tan() => IComplexNumber<double, Complex>.TanInterface(this);

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
	public static Complex Tan(Complex value) => IComplexNumber<double, Complex>.TanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	public Complex Tanh() => IComplexNumber<double, Complex>.TanhInterface(this);

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
	public static Complex Tanh(Complex value) => IComplexNumber<double, Complex>.TanhInterface(value);

	/// <summary>
	/// Преобразует данное число в массив байт.
	/// </summary>
	/// <param name="order">Порядок записи: &lt; 0 - Little Endian, &gt; 0 - Big Endian.</param>
	/// <returns>Массив байт, из которого можно восстановить данное число,
	/// с явным указанием длины мантиссы или без такового.</returns>
	/// <remarks>
	/// В данном типе порядок записи ни на что не влияет, оставлен только для унификации.
	/// </remarks>
	public byte[] ToByteArray(int order)
	{
		var bytes = GC.AllocateUninitializedArray<byte>(sizeof(double) << 1);
		if (order < 0 && TryWriteLittleEndian(bytes, out var bytesWritten) && bytesWritten == bytes.Length)
			return bytes;
		else if (order > 0 && TryWriteBigEndian(bytes, out bytesWritten) && bytesWritten == bytes.Length)
			return bytes;
		else
			throw new InvalidOperationException("Ошибка, не удалось преобразовать в массив байт.");
	}

	public override string ToString() => ((IComplexNumber<double, Complex>)this).ToStringInterface();
	public string ToString(IFormatProvider? formatProvider) =>
		((IComplexNumber<double, Complex>)this).ToStringInterface(formatProvider);
	public string ToString(string? format) => ((IComplexNumber<double, Complex>)this).ToStringInterface(format);
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		((IComplexNumber<double, Complex>)this).ToStringInterface(format, formatProvider);

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out Complex result)
		where TOther : INumberBase<TOther>
	{
		result = System.Numerics.Complex.CreateChecked(value);
		return true;
	}

	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out Complex result)
		where TOther : INumberBase<TOther>
	{
		result = System.Numerics.Complex.CreateSaturating(value);
		return true;
	}

	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out Complex result)
		where TOther : INumberBase<TOther>
	{
		result = System.Numerics.Complex.CreateTruncating(value);
		return true;
	}

	public static bool TryConvertToChecked<TOther>(Complex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToSaturating<TOther>(Complex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToTruncating<TOther>(Complex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
		((System.Numerics.Complex)this).TryFormat(destination, out charsWritten, format, provider);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Complex result) =>
		TryParse(s, NumberStyles.None, provider, out result);

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Complex result)
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

	/// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out Complex)"/>
	public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out Complex result) =>
		TryParse(s, NumberStyles.None, null, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Complex result) => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out Complex result) => TryParse(s.AsSpan(), style, provider, out result);
	/// <inheritdoc cref="TryParse(string, IFormatProvider?, out Complex)"/>
	public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Complex result) =>
		TryParse(s.AsSpan(), NumberStyles.None, null, out result);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian"/>
	/// <remarks>
	/// В данном типе этот метод и <see cref="TryWriteLittleEndian"/> эквивалентны, оба оставлены только для унификации.
	/// </remarks>
	public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten) =>
		TryWriteLittleEndian(destination, out bytesWritten);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian"/>
	/// <remarks>
	/// В данном типе этот метод и <see cref="TryWriteBigEndian"/> эквивалентны, оба оставлены только для унификации.
	/// </remarks>
	public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= sizeof(double) << 1 && BitConverter.TryWriteBytes(destination[..sizeof(double)], Real)
			&& BitConverter.TryWriteBytes(destination[sizeof(double)..], Imaginary))
		{
			bytesWritten = sizeof(double) << 1;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public static implicit operator Complex(double value) => new(value, 0d);
	public static implicit operator Complex(System.Numerics.Complex value) => new(value.Real, value.Imaginary);
	public static explicit operator System.Numerics.Complex(Complex value) => new(value.Real, value.Imaginary);

	public static Complex operator +(Complex value) => value;
	public static Complex operator -(Complex value) => -(IComplexNumber<double, Complex>)value;
	/// <inheritdoc cref="operator +(Complex, Complex)"/>
	public static Complex operator +(double left, Complex right) => right + left;
	/// <inheritdoc cref="operator +(Complex, Complex)"/>
	public static Complex operator +(Complex left, double right) =>
		new(left.Real + right, left.Imaginary);
	public static Complex operator +(Complex left, Complex right) => (IComplexNumber<double, Complex>)left + right;
	/// <inheritdoc cref="operator -(Complex, Complex)"/>
	public static Complex operator -(double left, Complex right) => -(right - left);
	/// <inheritdoc cref="operator -(Complex, Complex)"/>
	public static Complex operator -(Complex left, double right) =>
		new(left.Real - right, left.Imaginary);
	public static Complex operator -(Complex left, Complex right) => (IComplexNumber<double, Complex>)left - right;
	/// <inheritdoc cref="operator *(Complex, Complex)"/>
	public static Complex operator *(double left, Complex right) => right * left;
	/// <inheritdoc cref="operator *(Complex, Complex)"/>
	public static Complex operator *(Complex left, double right) =>
		new(double.IsNaN(left.Imaginary) ? double.NaN : left.Real * right,
		double.IsNaN(left.Real) ? double.NaN : left.Imaginary * right);
	public static Complex operator *(Complex left, Complex right) => (IComplexNumber<double, Complex>)left * right;
	/// <inheritdoc cref="operator /(Complex, Complex)"/>
	public static Complex operator /(Complex left, double right) =>
		new(double.IsNaN(left.Imaginary) ? double.NaN : left.Real / right,
		double.IsNaN(left.Real) ? double.NaN : left.Imaginary / right);
	public static Complex operator /(Complex left, Complex right) => (IComplexNumber<double, Complex>)left / right;
	static Complex IModulusOperators<Complex, Complex, Complex>.operator %(Complex left, Complex right) =>
		throw new NotSupportedException("Ошибка, остаток от деления не определен для комплексных чисел.");
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static Complex operator <<(Complex x, int shiftAmount) => x * Math.Shift(1d, shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static Complex operator >>(Complex x, int shiftAmount) => x * Math.Shift(1d, -shiftAmount);
	public static Complex operator ++(Complex value) => new(value.Real + 1d, value.Imaginary);
	public static Complex operator --(Complex value) => new(value.Real - 1d, value.Imaginary);
	/// <inheritdoc cref="operator ==(Complex, Complex)"/>
	public static bool operator ==(double left, Complex right) => right == left;
	/// <inheritdoc cref="operator ==(Complex, Complex)"/>
	public static bool operator ==(Complex left, double right) =>
		left.Real == right && left.Imaginary == 0d;
	/// <inheritdoc cref="IEqualityOperators{Complex, Complex, bool}.operator =="/>
	public static bool operator ==(Complex left, Complex right) => left.Real == right.Real && left.Imaginary == right.Imaginary;
	/// <inheritdoc cref="operator !=(Complex, Complex)"/>
	public static bool operator !=(double left, Complex right) => right != left;
	/// <inheritdoc cref="operator !=(Complex, Complex)"/>
	public static bool operator !=(Complex left, double right) =>
		left.Real != right || left.Imaginary != 0d;
	/// <inheritdoc cref="IEqualityOperators{Complex, Complex, bool}.operator !="/>
	public static bool operator !=(Complex left, Complex right) => left.Real != right.Real || left.Imaginary != right.Imaginary;
#pragma warning restore S1244
#pragma warning disable S3877
	static bool IComparisonOperators<Complex, Complex, bool>.operator >=(Complex left, Complex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<Complex, Complex, bool>.operator <=(Complex left, Complex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<Complex, Complex, bool>.operator >(Complex left, Complex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<Complex, Complex, bool>.operator <(Complex left, Complex right) =>
		throw new NotSupportedException(NoComparisons);
#pragma warning restore S3877
#pragma warning restore IDE0079 // Удалить ненужное подавление
}