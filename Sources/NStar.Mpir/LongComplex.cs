namespace NStar.Mpir;
#pragma warning disable CA2260 // Используйте правильный параметр типа
public readonly struct LongComplex(LongReal real, LongReal imaginary) : IComplexNumber<LongReal, LongComplex>
#pragma warning restore CA2260 // Используйте правильный параметр типа
{
	private const string NoComparisons = "Ошибка, операции \"больше\" и \"меньше\" не определены для комплексных чисел.";

	public static LongComplex AdditiveIdentity => Zero;
	static Func<LongReal, LongReal, LongComplex> IComplexNumber<LongReal, LongComplex>.Creator =>
		(real, imaginary) => new(real, imaginary);
	/// <inheritdoc cref="IFloatingPointConstants{double}.E"/>
	public static LongComplex E => new(LongReal.E, LongReal.Zero);
	public LongReal Imaginary { get; } = imaginary;
	public static LongComplex MultiplicativeIdentity => One;
	/// <inheritdoc cref="LongReal.NaN"/>
	public static LongComplex NaN { get; } = new(LongReal.NaN, LongReal.Zero);
	/// <inheritdoc cref="LongReal.NegativeInfinity"/>
	public static LongComplex NegativeInfinity { get; } = new(LongReal.NegativeInfinity, LongReal.Zero);
	/// <inheritdoc cref="ISignedNumber{double}.NegativeOne"/>
	public static LongComplex NegativeOne => new(LongReal.NegativeOne, LongReal.Zero);
	public static LongComplex One => new(LongReal.One, LongReal.Zero);
	/// <inheritdoc cref="IFloatingPointConstants{double}.Pi"/>
	public static LongComplex Pi => new(LongReal.Pi, LongReal.Zero);
	/// <inheritdoc cref="LongReal.PositiveInfinity"/>
	public static LongComplex PositiveInfinity { get; } = new(LongReal.PositiveInfinity, LongReal.Zero);
	public static int Radix => 2;
	public LongReal Real { get; } = real;
	/// <inheritdoc cref="IFloatingPointConstants{double}.Tau"/>
	public static LongComplex Tau => new(LongReal.Tau, LongReal.Zero);
	public static LongComplex Zero => new(LongReal.Zero, LongReal.Zero);

	/// <summary>
	/// Computes the absolute of this number.
	/// </summary>
	/// <returns>The absolute of this number.</returns>
	public LongReal Abs() => IComplexNumber<LongReal, LongComplex>.AbsInterface(this);
	/// <inheritdoc cref="INumberBase{Complex}.Abs"/>
	public static LongReal Abs(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AbsInterface(value);
	static LongComplex INumberBase<LongComplex>.Abs(LongComplex value) =>
		IComplexNumber<LongReal, LongComplex>.AbsInterface(value);

	/// <summary>
	/// Вычисляет арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - Pi &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public LongComplex Acos() => IComplexNumber<LongReal, LongComplex>.AcosInterface(this);

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
	public static LongComplex Acos(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AcosInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля, для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - гиперболический арккосинус данного числа.
	/// </returns>
	public LongComplex Acosh() => IComplexNumber<LongReal, LongComplex>.AcoshInterface(this);

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
	public static LongComplex Acosh(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AcoshInterface(value);

	/// <summary>
	/// Вычисляет арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арксинус данного числа.
	/// </returns>
	public LongComplex Asin() => IComplexNumber<LongReal, LongComplex>.AsinInterface(this);

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
	public static LongComplex Asin(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AsinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арксинус данного числа.
	/// </returns>
	public LongComplex Asinh() => IComplexNumber<LongReal, LongComplex>.AsinhInterface(this);

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
	public static LongComplex Asinh(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AsinhInterface(value);

	/// <summary>
	/// Вычисляет арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арктангенс данного числа.
	/// </returns>
	public LongComplex Atan() => IComplexNumber<LongReal, LongComplex>.AtanInterface(this);

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
	public static LongComplex Atan(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AtanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public LongComplex Atanh() => IComplexNumber<LongReal, LongComplex>.AtanhInterface(this);

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
	public static LongComplex Atanh(LongComplex value) => IComplexNumber<LongReal, LongComplex>.AtanhInterface(value);

	public int CompareTo(object? obj) => throw new NotSupportedException(NoComparisons);
	public int CompareTo(LongComplex other) => throw new NotSupportedException(NoComparisons);

	/// <summary>
	/// Вычисляет комплексное сопряжение данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - комплексное сопряжение данного числа.
	/// </returns>
	public LongComplex Conjugate() => IComplexNumber<LongReal, LongComplex>.ConjugateInterface(this);

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
	public static LongComplex Conjugate(LongComplex value) => IComplexNumber<LongReal, LongComplex>.ConjugateInterface(value);

	/// <summary>
	/// Вычисляет косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - косинус данного числа.
	/// </returns>
	public LongComplex Cos() => IComplexNumber<LongReal, LongComplex>.CosInterface(this);

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
	public static LongComplex Cos(LongComplex value) => IComplexNumber<LongReal, LongComplex>.CosInterface(value);

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
	public LongComplex Cosh() => IComplexNumber<LongReal, LongComplex>.CoshInterface(this);

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
	public static LongComplex Cosh(LongComplex value) => IComplexNumber<LongReal, LongComplex>.CosInterface(value);

	public bool Equals(LongComplex other) => Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		((IComplexNumber<LongReal, LongComplex>)this).EqualsInterface(obj);

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
	public LongComplex Exp() => IComplexNumber<LongReal, LongComplex>.ExpInterface(this);

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
	public static LongComplex Exp(LongComplex value) => IComplexNumber<LongReal, LongComplex>.ExpInterface(value);

	public override int GetHashCode() => ((IComplexNumber<LongReal, LongComplex>)this).GetHashCodeInterface();
	public static bool IsCanonical(LongComplex value) => true;
	public static bool IsComplexNumber(LongComplex value) => true;
#pragma warning disable IDE0079 // Удалить ненужное подавление
	public static bool IsEvenInteger(LongComplex value) =>
		LongReal.IsEvenInteger(value.Real) && value.Imaginary == LongReal.Zero;
	public static bool IsFinite(LongComplex value) => LongReal.IsFinite(value.Real) && LongReal.IsFinite(value.Imaginary);
	public static bool IsImaginaryNumber(LongComplex value) => value.Imaginary != LongReal.Zero;
	public static bool IsInfinity(LongComplex value) => LongReal.IsInfinity(value.Real) || LongReal.IsInfinity(value.Imaginary);
	public static bool IsInteger(LongComplex value) => LongReal.IsInteger(value.Real) && value.Imaginary == LongReal.Zero;
	public static bool IsNaN(LongComplex value) => LongReal.IsNaN(value.Real) || LongReal.IsNaN(value.Imaginary);
	public static bool IsNegative(LongComplex value) => value.Real < LongReal.Zero && value.Imaginary == LongReal.Zero;
	public static bool IsNegativeInfinity(LongComplex value) =>
		LongReal.IsNegativeInfinity(value.Real) && LongReal.IsNegativeInfinity(value.Imaginary);
	public static bool IsNormal(LongComplex value) => LongReal.IsNormal(value.Real) && LongReal.IsNormal(value.Imaginary);
	public static bool IsOddInteger(LongComplex value) => LongReal.IsOddInteger(value.Real) && value.Imaginary == LongReal.Zero;
	public static bool IsPositive(LongComplex value) => value.Real > LongReal.Zero && value.Imaginary == LongReal.Zero;
	public static bool IsPositiveInfinity(LongComplex value) =>
		LongReal.IsPositiveInfinity(value.Real) && LongReal.IsPositiveInfinity(value.Imaginary);
	public static bool IsRealNumber(LongComplex value) => value.Imaginary == LongReal.Zero;
	public static bool IsSubnormal(LongComplex value) =>
		LongReal.IsSubnormal(value.Real) || LongReal.IsSubnormal(value.Imaginary);
	public static bool IsZero(LongComplex value) => value.Real == LongReal.Zero && value.Imaginary == LongReal.Zero;

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
	public LongComplex Ln() => IComplexNumber<LongReal, LongComplex>.LogInterface(this);

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
	public static LongComplex Ln(LongComplex value) => IComplexNumber<LongReal, LongComplex>.LogInterface(value);

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
	public LongComplex Log() => IComplexNumber<LongReal, LongComplex>.LogInterface(this);

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
	public LongComplex Log(LongReal @base) => IComplexNumber<LongReal, LongComplex>.LogInterface(this, @base);

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
	public static LongComplex Log(LongComplex value, LongReal @base) =>
		IComplexNumber<LongReal, LongComplex>.LogInterface(value, @base);

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
	public LongComplex Log2() =>
		IComplexNumber<LongReal, LongComplex>.LogInterface(this, new(MpzT.Zero, UnsignedLongReal.One));

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
	public static LongComplex Log2(LongComplex value) =>
		IComplexNumber<LongReal, LongComplex>.LogInterface(value, new(MpzT.Zero, UnsignedLongReal.One));

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
	public LongComplex Log10() => IComplexNumber<LongReal, LongComplex>.LogInterface(this, 10d);

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
	public static LongComplex Log10(LongComplex value) => IComplexNumber<LongReal, LongComplex>.LogInterface(value, 10d);

	public static LongComplex MaxMagnitude(LongComplex x, LongComplex y) => Abs(MaxMagnitudeNumber(x, y));
	public static LongComplex MaxMagnitudeNumber(LongComplex x, LongComplex y) => Abs(x) > Abs(y) ? x : y;
	public static LongComplex MinMagnitude(LongComplex x, LongComplex y) => Abs(MinMagnitudeNumber(x, y));
	public static LongComplex MinMagnitudeNumber(LongComplex x, LongComplex y) => Abs(x) < Abs(y) ? x : y;
	public static LongComplex Parse(string s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static LongComplex Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);
	public static LongComplex Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static LongComplex Parse(string s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);

	/// <summary>
	/// Возводит данное число в указанную степень.
	/// </summary>
	/// <param name="exponent">Показатель степени, в которую нужно возвести данное число.</param>
	/// <returns>Результат возведения в степень.</returns>
	public LongComplex Power(int exponent) => IComplexNumber<LongReal, LongComplex>.PowInterface(this, exponent);

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
	public LongComplex Power(LongReal exponent) => IComplexNumber<LongReal, LongComplex>.PowInterface(this, exponent);

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
	public static LongComplex Power(LongComplex @base, LongReal exponent) => @base.Power(exponent);

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
	public LongComplex Power(LongComplex exponent) => IComplexNumber<LongReal, LongComplex>.PowInterface(this, exponent);

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
	public static LongComplex Power(LongComplex @base, LongComplex exponent) => @base.Power(exponent);

	/// <summary>
	/// Вычисляет число, обратное данному (1 / x).
	/// </summary>
	/// <returns>
	/// Для нуля - плюс бесконечность;<br />
	/// для плюс бесконечности или для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - число, обратное данному (1 / x).
	/// </returns>
	public LongComplex Reciproc() => IComplexNumber<LongReal, LongComplex>.ReciprocInterface(this);

	/// <summary>
	/// Вычисляет синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - синус данного числа.
	/// </returns>
	public LongComplex Sin() => IComplexNumber<LongReal, LongComplex>.SinInterface(this);

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
	public static LongComplex Sin(LongComplex value) => IComplexNumber<LongReal, LongComplex>.SinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус данного числа.
	/// </returns>
	public LongComplex Sinh() => IComplexNumber<LongReal, LongComplex>.SinhInterface(this);

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
	public static LongComplex Sinh(LongComplex value) => IComplexNumber<LongReal, LongComplex>.SinhInterface(value);

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
	public LongComplex Sqrt() => IComplexNumber<LongReal, LongComplex>.SqrtInterface(this);

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
	public static LongComplex Sqrt(LongComplex value) => IComplexNumber<LongReal, LongComplex>.SqrtInterface(value);

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
	public LongComplex Square() => IComplexNumber<LongReal, LongComplex>.SquareInterface(this);

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
	public static LongComplex Square(LongComplex value) => IComplexNumber<LongReal, LongComplex>.SquareInterface(value);

	/// <summary>
	/// Вычисляет тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс данного числа.
	/// </returns>
	public LongComplex Tan() => IComplexNumber<LongReal, LongComplex>.TanInterface(this);

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
	public static LongComplex Tan(LongComplex value) => IComplexNumber<LongReal, LongComplex>.TanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	public LongComplex Tanh() => IComplexNumber<LongReal, LongComplex>.TanhInterface(this);

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
	public static LongComplex Tanh(LongComplex value) => IComplexNumber<LongReal, LongComplex>.TanhInterface(value);

	public override string ToString() => ((IComplexNumber<LongReal, LongComplex>)this).ToStringInterface();
	public string ToString(IFormatProvider? formatProvider) =>
		((IComplexNumber<LongReal, LongComplex>)this).ToStringInterface(formatProvider);
	public string ToString(string? format) => ((IComplexNumber<LongReal, LongComplex>)this).ToStringInterface(format);
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		((IComplexNumber<LongReal, LongComplex>)this).ToStringInterface(format, formatProvider);

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out LongComplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out LongComplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out LongComplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToChecked<TOther>(LongComplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToSaturating<TOther>(LongComplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToTruncating<TOther>(LongComplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongComplex result)
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
		[MaybeNullWhen(false)] out LongComplex result) => TryParse(s.AsSpan(), style, provider, out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongComplex result) =>
		TryParse(s, NumberStyles.None, provider, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongComplex result) => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
		((System.Numerics.Complex)this).TryFormat(destination, out charsWritten, format, provider);

	public static implicit operator LongComplex(LongReal value) => new(value, LongReal.Zero);
	public static implicit operator LongComplex(Complex value) => new(value.Real, value.Imaginary);
	public static implicit operator LongComplex(System.Numerics.Complex value) => new(value.Real, value.Imaginary);
	public static explicit operator System.Numerics.Complex(LongComplex value) =>
		new((double)value.Real, (double)value.Imaginary);

	public static LongComplex operator +(LongComplex value) => value;
	public static LongComplex operator -(LongComplex value) => -(IComplexNumber<LongReal, LongComplex>)value;
	public static LongComplex operator +(LongReal left, LongComplex right) => right + left;
	public static LongComplex operator +(LongComplex left, LongReal right) =>
		new(left.Real + right, left.Imaginary);
	public static LongComplex operator +(LongComplex left, LongComplex right) =>
		(IComplexNumber<LongReal, LongComplex>)left + right;
	public static LongComplex operator -(LongReal left, LongComplex right) => -(right - left);
	public static LongComplex operator -(LongComplex left, LongReal right) =>
		new(left.Real - right, left.Imaginary);
	public static LongComplex operator -(LongComplex left, LongComplex right) =>
		(IComplexNumber<LongReal, LongComplex>)left - right;
	public static LongComplex operator *(LongReal left, LongComplex right) => right * left;
	public static LongComplex operator *(LongComplex left, LongReal right) =>
		new(left.Real * right, left.Imaginary * right);
	public static LongComplex operator *(LongComplex left, LongComplex right) =>
		(IComplexNumber<LongReal, LongComplex>)left * right;
	public static LongComplex operator /(LongComplex left, LongReal right) =>
		new(left.Real / right, left.Imaginary / right);
	public static LongComplex operator /(LongComplex left, LongComplex right) =>
		(IComplexNumber<LongReal, LongComplex>)left / right;
	static LongComplex IModulusOperators<LongComplex, LongComplex, LongComplex>.operator %(LongComplex left,
		LongComplex right) => throw new NotSupportedException("Ошибка, остаток от деления не определен для комплексных чисел.");
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongComplex operator <<(LongComplex x, int shiftAmount) =>
		new(x.Real << shiftAmount, x.Imaginary << shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongComplex operator <<(LongComplex x, UnsignedLongReal shiftAmount) =>
		new(x.Real << shiftAmount, x.Imaginary << shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongComplex operator >>(LongComplex x, int shiftAmount) =>
		new(x.Real >> shiftAmount, x.Imaginary >> shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongComplex operator >>(LongComplex x, UnsignedLongReal shiftAmount) =>
		new(x.Real >> shiftAmount, x.Imaginary >> shiftAmount);
	public static LongComplex operator ++(LongComplex value) => new(value.Real + 1d, value.Imaginary);
	public static LongComplex operator --(LongComplex value) => new(value.Real - 1d, value.Imaginary);
	public static bool operator ==(LongReal left, LongComplex right) => right == left;
	public static bool operator ==(LongComplex left, LongReal right) =>
		left.Real == right && left.Imaginary == LongReal.Zero;
	public static bool operator ==(LongComplex left, LongComplex right) =>
		left.Real == right.Real && left.Imaginary == right.Imaginary;
	public static bool operator !=(LongReal left, LongComplex right) => right != left;
	public static bool operator !=(LongComplex left, LongReal right) =>
		left.Real != right || left.Imaginary != LongReal.Zero;
	public static bool operator !=(LongComplex left, LongComplex right) =>
		left.Real != right.Real || left.Imaginary != right.Imaginary;
#pragma warning disable S3877
	static bool IComparisonOperators<LongComplex, LongComplex, bool>.operator >=(LongComplex left, LongComplex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<LongComplex, LongComplex, bool>.operator <=(LongComplex left, LongComplex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<LongComplex, LongComplex, bool>.operator >(LongComplex left, LongComplex right) =>
		throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<LongComplex, LongComplex, bool>.operator <(LongComplex left, LongComplex right) =>
		throw new NotSupportedException(NoComparisons);
#pragma warning restore S3877
#pragma warning restore IDE0079 // Удалить ненужное подавление
}