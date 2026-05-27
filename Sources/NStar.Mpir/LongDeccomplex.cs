namespace NStar.Mpir;
#pragma warning disable CA2260 // Используйте правильный параметр типа
public readonly struct LongDeccomplex(LongDecimal real, LongDecimal imaginary) : IComplexNumber<LongDecimal, LongDeccomplex>
#pragma warning restore CA2260 // Используйте правильный параметр типа
{
	private const string NoComparisons = "Ошибка, операции \"больше\" и \"меньше\" не определены для комплексных чисел.";

	public static LongDeccomplex AdditiveIdentity => Zero;
	static Func<LongDecimal, LongDecimal, LongDeccomplex> IComplexNumber<LongDecimal, LongDeccomplex>.Creator =>
		(real, imaginary) => new(real, imaginary);
	/// <inheritdoc cref="IFloatingPointConstants{double}.E"/>
	public static LongDeccomplex E => new(LongDecimal.E, LongDecimal.Zero);
	public LongDecimal Imaginary { get; } = imaginary;
	public static LongDeccomplex MultiplicativeIdentity => One;
	/// <inheritdoc cref="LongDecimal.NaN"/>
	public static LongDeccomplex NaN { get; } = new(LongDecimal.NaN, LongDecimal.Zero);
	/// <inheritdoc cref="LongDecimal.NegativeInfinity"/>
	public static LongDeccomplex NegativeInfinity { get; } = new(LongDecimal.NegativeInfinity, LongDecimal.Zero);
	/// <inheritdoc cref="ISignedNumber{double}.NegativeOne"/>
	public static LongDeccomplex NegativeOne => new(LongDecimal.NegativeOne, LongDecimal.Zero);
	public static LongDeccomplex One => new(LongDecimal.One, LongDecimal.Zero);
	/// <inheritdoc cref="IFloatingPointConstants{double}.Pi"/>
	public static LongDeccomplex Pi => new(LongDecimal.Pi, LongDecimal.Zero);
	/// <inheritdoc cref="LongDecimal.PositiveInfinity"/>
	public static LongDeccomplex PositiveInfinity { get; } = new(LongDecimal.PositiveInfinity, LongDecimal.Zero);
	public static int Radix => 10;
	public LongDecimal Real { get; } = real;
	/// <inheritdoc cref="IFloatingPointConstants{double}.Tau"/>
	public static LongDeccomplex Tau => new(LongDecimal.Tau, LongDecimal.Zero);
	public static LongDeccomplex Zero => new(LongDecimal.Zero, LongDecimal.Zero);

	/// <summary>
	/// Computes the absolute of this number.
	/// </summary>
	/// <returns>The absolute of this number.</returns>
	public LongDecimal Abs() => IComplexNumber<LongDecimal, LongDeccomplex>.AbsInterface(this);
	/// <inheritdoc cref="INumberBase{Complex}.Abs"/>
	public static LongDecimal Abs(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.AbsInterface(value);
	static LongDeccomplex INumberBase<LongDeccomplex>.Abs(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.AbsInterface(value);

	/// <summary>
	/// Вычисляет арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - Pi &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	public LongDeccomplex Acos() => IComplexNumber<LongDecimal, LongDeccomplex>.AcosInterface(this);

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
	public static LongDeccomplex Acos(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.AcosInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арккосинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля, для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для отрицательных чисел - неопределенность;<br />
	/// в остальных случаях - гиперболический арккосинус данного числа.
	/// </returns>
	public LongDeccomplex Acosh() => IComplexNumber<LongDecimal, LongDeccomplex>.AcoshInterface(this);

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
	public static LongDeccomplex Acosh(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.AcoshInterface(value);

	/// <summary>
	/// Вычисляет арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арксинус данного числа.
	/// </returns>
	public LongDeccomplex Asin() => IComplexNumber<LongDecimal, LongDeccomplex>.AsinInterface(this);

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
	public static LongDeccomplex Asin(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.AsinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арксинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический арксинус данного числа.
	/// </returns>
	public LongDeccomplex Asinh() => IComplexNumber<LongDecimal, LongDeccomplex>.AsinhInterface(this);

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
	public static LongDeccomplex Asinh(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.AsinhInterface(value);

	/// <summary>
	/// Вычисляет арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арктангенс данного числа.
	/// </returns>
	public LongDeccomplex Atan() => IComplexNumber<LongDecimal, LongDeccomplex>.AtanInterface(this);

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
	public static LongDeccomplex Atan(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.AtanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический арктангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - гиперболический арктангенс данного числа.
	/// </returns>
	public LongDeccomplex Atanh() => IComplexNumber<LongDecimal, LongDeccomplex>.AtanhInterface(this);

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
	public static LongDeccomplex Atanh(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.AtanhInterface(value);

	public int CompareTo(object? obj) => throw new NotSupportedException(NoComparisons);
	public int CompareTo(LongDeccomplex other) => throw new NotSupportedException(NoComparisons);

	/// <summary>
	/// Вычисляет комплексное сопряжение данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - комплексное сопряжение данного числа.
	/// </returns>
	public LongDeccomplex Conjugate() => IComplexNumber<LongDecimal, LongDeccomplex>.ConjugateInterface(this);

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
	public static LongDeccomplex Conjugate(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.ConjugateInterface(value);

	/// <summary>
	/// Вычисляет косинус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - косинус данного числа.
	/// </returns>
	public LongDeccomplex Cos() => IComplexNumber<LongDecimal, LongDeccomplex>.CosInterface(this);

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
	public static LongDeccomplex Cos(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.CosInterface(value);

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
	public LongDeccomplex Cosh() => IComplexNumber<LongDecimal, LongDeccomplex>.CoshInterface(this);

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
	public static LongDeccomplex Cosh(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.CosInterface(value);

	public bool Equals(LongDeccomplex other) => Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		((IComplexNumber<LongDecimal, LongDeccomplex>)this).EqualsInterface(obj);

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
	public LongDeccomplex Exp() => IComplexNumber<LongDecimal, LongDeccomplex>.ExpInterface(this);

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
	public static LongDeccomplex Exp(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.ExpInterface(value);

	public override int GetHashCode() => ((IComplexNumber<LongDecimal, LongDeccomplex>)this).GetHashCodeInterface();
	public static bool IsCanonical(LongDeccomplex value) => true;
	public static bool IsComplexNumber(LongDeccomplex value) => true;
#pragma warning disable IDE0079 // Удалить ненужное подавление
	public static bool IsEvenInteger(LongDeccomplex value) =>
		LongDecimal.IsEvenInteger(value.Real) && value.Imaginary == LongDecimal.Zero;
	public static bool IsFinite(LongDeccomplex value) =>
		LongDecimal.IsFinite(value.Real) && LongDecimal.IsFinite(value.Imaginary);
	public static bool IsImaginaryNumber(LongDeccomplex value) => value.Imaginary != LongDecimal.Zero;
	public static bool IsInfinity(LongDeccomplex value) =>
		LongDecimal.IsInfinity(value.Real) || LongDecimal.IsInfinity(value.Imaginary);
	public static bool IsInteger(LongDeccomplex value) =>
		LongDecimal.IsInteger(value.Real) && value.Imaginary == LongDecimal.Zero;
	public static bool IsNaN(LongDeccomplex value) => LongDecimal.IsNaN(value.Real) || LongDecimal.IsNaN(value.Imaginary);
	public static bool IsNegative(LongDeccomplex value) => value.Real < LongDecimal.Zero && value.Imaginary == LongDecimal.Zero;
	public static bool IsNegativeInfinity(LongDeccomplex value) =>
		LongDecimal.IsNegativeInfinity(value.Real) && LongDecimal.IsNegativeInfinity(value.Imaginary);
	public static bool IsNormal(LongDeccomplex value) =>
		LongDecimal.IsNormal(value.Real) && LongDecimal.IsNormal(value.Imaginary);
	public static bool IsOddInteger(LongDeccomplex value) =>
		LongDecimal.IsOddInteger(value.Real) && value.Imaginary == LongDecimal.Zero;
	public static bool IsPositive(LongDeccomplex value) => value.Real > LongDecimal.Zero && value.Imaginary == LongDecimal.Zero;
	public static bool IsPositiveInfinity(LongDeccomplex value) =>
		LongDecimal.IsPositiveInfinity(value.Real) && LongDecimal.IsPositiveInfinity(value.Imaginary);
	public static bool IsRealNumber(LongDeccomplex value) => value.Imaginary == LongDecimal.Zero;
	public static bool IsSubnormal(LongDeccomplex value) =>
		LongDecimal.IsSubnormal(value.Real) || LongDecimal.IsSubnormal(value.Imaginary);
	public static bool IsZero(LongDeccomplex value) => value.Real == LongDecimal.Zero && value.Imaginary == LongDecimal.Zero;

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
	public LongDeccomplex Ln() => IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(this);

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
	/// <see cref="Log(LongDecimal)">Log()</see> с одним параметром типа <see cref="LongDecimal"/>,
	/// а два метода с одинаковыми количеством и типами параметров невозможны,
	/// даже если один статический, а другой экземплярный.
	/// </remarks>
	public static LongDeccomplex Ln(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(value);

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
	public LongDeccomplex Log() => IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(this);

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
	public LongDeccomplex Log(LongDecimal @base) => IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(this, @base);

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
	public static LongDeccomplex Log(LongDeccomplex value, LongDecimal @base) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(value, @base);

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
	public LongDeccomplex Log2() =>
		IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(this, LongDecimal.Two);

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
	public static LongDeccomplex Log2(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(value, LongDecimal.Two);

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
	public LongDeccomplex Log10() => IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(this, 10d);

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
	public static LongDeccomplex Log10(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.LogInterface(value, 10d);

	public static LongDeccomplex MaxMagnitude(LongDeccomplex x, LongDeccomplex y) => Abs(MaxMagnitudeNumber(x, y));
	public static LongDeccomplex MaxMagnitudeNumber(LongDeccomplex x, LongDeccomplex y) => Abs(x) > Abs(y) ? x : y;
	public static LongDeccomplex MinMagnitude(LongDeccomplex x, LongDeccomplex y) => Abs(MinMagnitudeNumber(x, y));
	public static LongDeccomplex MinMagnitudeNumber(LongDeccomplex x, LongDeccomplex y) => Abs(x) < Abs(y) ? x : y;
	public static LongDeccomplex Parse(string s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static LongDeccomplex Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);
	public static LongDeccomplex Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, provider);
	public static LongDeccomplex Parse(string s, NumberStyles style, IFormatProvider? provider) =>
		System.Numerics.Complex.Parse(s, style, provider);

	/// <summary>
	/// Возводит данное число в указанную степень.
	/// </summary>
	/// <param name="exponent">Показатель степени, в которую нужно возвести данное число.</param>
	/// <returns>Результат возведения в степень.</returns>
	public LongDeccomplex Power(int exponent) => IComplexNumber<LongDecimal, LongDeccomplex>.PowInterface(this, exponent);

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
	public LongDeccomplex Power(LongDecimal exponent) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.PowInterface(this, exponent);

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
	public static LongDeccomplex Power(LongDeccomplex @base, LongDecimal exponent) => @base.Power(exponent);

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
	public LongDeccomplex Power(LongDeccomplex exponent) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.PowInterface(this, exponent);

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
	public static LongDeccomplex Power(LongDeccomplex @base, LongDeccomplex exponent) => @base.Power(exponent);

	/// <summary>
	/// Вычисляет число, обратное данному (1 / x).
	/// </summary>
	/// <returns>
	/// Для нуля - плюс бесконечность;<br />
	/// для плюс бесконечности или для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - число, обратное данному (1 / x).
	/// </returns>
	public LongDeccomplex Reciproc() => IComplexNumber<LongDecimal, LongDeccomplex>.ReciprocInterface(this);

	/// <summary>
	/// Вычисляет синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - синус данного числа.
	/// </returns>
	public LongDeccomplex Sin() => IComplexNumber<LongDecimal, LongDeccomplex>.SinInterface(this);

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
	public static LongDeccomplex Sin(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.SinInterface(value);

	/// <summary>
	/// Вычисляет гиперболический синус данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический синус данного числа.
	/// </returns>
	public LongDeccomplex Sinh() => IComplexNumber<LongDecimal, LongDeccomplex>.SinhInterface(this);

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
	public static LongDeccomplex Sinh(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.SinhInterface(value);

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
	public LongDeccomplex Sqrt() => IComplexNumber<LongDecimal, LongDeccomplex>.SqrtInterface(this);

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
	public static LongDeccomplex Sqrt(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.SqrtInterface(value);

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
	public LongDeccomplex Square() => IComplexNumber<LongDecimal, LongDeccomplex>.SquareInterface(this);

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
	public static LongDeccomplex Square(LongDeccomplex value) =>
		IComplexNumber<LongDecimal, LongDeccomplex>.SquareInterface(value);

	/// <summary>
	/// Вычисляет тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше Pi &lt;&lt; MantissaLength + 1 - неопределенность;<br />
	/// в остальных случаях - тангенс данного числа.
	/// </returns>
	public LongDeccomplex Tan() => IComplexNumber<LongDecimal, LongDeccomplex>.TanInterface(this);

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
	public static LongDeccomplex Tan(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.TanInterface(value);

	/// <summary>
	/// Вычисляет гиперболический тангенс данного числа.
	/// </summary>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический тангенс данного числа.
	/// </returns>
	public LongDeccomplex Tanh() => IComplexNumber<LongDecimal, LongDeccomplex>.TanhInterface(this);

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
	public static LongDeccomplex Tanh(LongDeccomplex value) => IComplexNumber<LongDecimal, LongDeccomplex>.TanhInterface(value);

	public override string ToString() => ((IComplexNumber<LongDecimal, LongDeccomplex>)this).ToStringInterface();
	public string ToString(IFormatProvider? formatProvider) =>
		((IComplexNumber<LongDecimal, LongDeccomplex>)this).ToStringInterface(formatProvider);
	public string ToString(string? format) => ((IComplexNumber<LongDecimal, LongDeccomplex>)this).ToStringInterface(format);
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		((IComplexNumber<LongDecimal, LongDeccomplex>)this).ToStringInterface(format, formatProvider);

	public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out LongDeccomplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out LongDeccomplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out LongDeccomplex result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToChecked<TOther>(LongDeccomplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToSaturating<TOther>(LongDeccomplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryConvertToTruncating<TOther>(LongDeccomplex value, [MaybeNullWhen(false)] out TOther result)
		where TOther : INumberBase<TOther> => throw new NotImplementedException();
	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongDeccomplex result)
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
		[MaybeNullWhen(false)] out LongDeccomplex result) => TryParse(s.AsSpan(), style, provider, out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongDeccomplex result) =>
		TryParse(s, NumberStyles.None, provider, out result);
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		[MaybeNullWhen(false)] out LongDeccomplex result) => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
		((System.Numerics.Complex)this).TryFormat(destination, out charsWritten, format, provider);

	public static implicit operator LongDeccomplex(LongDecimal value) => new(value, LongDecimal.Zero);
	public static implicit operator LongDeccomplex(Complex value) => new(value.Real, value.Imaginary);
	public static implicit operator LongDeccomplex(Deccomplex value) => new(value.Real, value.Imaginary);
	public static implicit operator LongDeccomplex(LongComplex value) => new(value.Real, value.Imaginary);
	public static implicit operator LongDeccomplex(System.Numerics.Complex value) => new(value.Real, value.Imaginary);
	public static explicit operator System.Numerics.Complex(LongDeccomplex value) =>
		new((double)value.Real, (double)value.Imaginary);

	public static LongDeccomplex operator +(LongDeccomplex value) => value;
	public static LongDeccomplex operator -(LongDeccomplex value) => -(IComplexNumber<LongDecimal, LongDeccomplex>)value;
	public static LongDeccomplex operator +(LongDecimal left, LongDeccomplex right) => right + left;
	public static LongDeccomplex operator +(LongDeccomplex left, LongDecimal right) =>
		new(left.Real + right, left.Imaginary);
	public static LongDeccomplex operator +(LongDeccomplex left, LongDeccomplex right) =>
		(IComplexNumber<LongDecimal, LongDeccomplex>)left + right;
	public static LongDeccomplex operator -(LongDecimal left, LongDeccomplex right) => -(right - left);
	public static LongDeccomplex operator -(LongDeccomplex left, LongDecimal right) =>
		new(left.Real - right, left.Imaginary);
	public static LongDeccomplex operator -(LongDeccomplex left, LongDeccomplex right) =>
		(IComplexNumber<LongDecimal, LongDeccomplex>)left - right;
	public static LongDeccomplex operator *(LongDecimal left, LongDeccomplex right) => right * left;
	public static LongDeccomplex operator *(LongDeccomplex left, LongDecimal right) =>
		new(left.Real * right, left.Imaginary * right);
	public static LongDeccomplex operator *(LongDeccomplex left, LongDeccomplex right) =>
		(IComplexNumber<LongDecimal, LongDeccomplex>)left * right;
	public static LongDeccomplex operator /(LongDeccomplex left, LongDecimal right) =>
		new(left.Real / right, left.Imaginary / right);
	public static LongDeccomplex operator /(LongDeccomplex left, LongDeccomplex right) =>
		(IComplexNumber<LongDecimal, LongDeccomplex>)left / right;
	static LongDeccomplex IModulusOperators<LongDeccomplex, LongDeccomplex, LongDeccomplex>.operator %(LongDeccomplex left,
		LongDeccomplex right) =>
		throw new NotSupportedException("Ошибка, остаток от деления не определен для комплексных чисел.");
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongDeccomplex operator <<(LongDeccomplex x, int shiftAmount) =>
		new(x.Real << shiftAmount, x.Imaginary << shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator {{"/>
	public static LongDeccomplex operator <<(LongDeccomplex x, UnsignedLongDecimal shiftAmount) =>
		new(x.Real << shiftAmount, x.Imaginary << shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongDeccomplex operator >>(LongDeccomplex x, int shiftAmount) =>
		new(x.Real >> shiftAmount, x.Imaginary >> shiftAmount);
	/// <inheritdoc cref="IShiftOperators{TSelf, int, TSelf}.operator }}"/>
	public static LongDeccomplex operator >>(LongDeccomplex x, UnsignedLongDecimal shiftAmount) =>
		new(x.Real >> shiftAmount, x.Imaginary >> shiftAmount);
	public static LongDeccomplex operator ++(LongDeccomplex value) => new(value.Real + 1d, value.Imaginary);
	public static LongDeccomplex operator --(LongDeccomplex value) => new(value.Real - 1d, value.Imaginary);
	public static bool operator ==(LongDecimal left, LongDeccomplex right) => right == left;
	public static bool operator ==(LongDeccomplex left, LongDecimal right) =>
		left.Real == right && left.Imaginary == LongDecimal.Zero;
	public static bool operator ==(LongDeccomplex left, LongDeccomplex right) =>
		left.Real == right.Real && left.Imaginary == right.Imaginary;
	public static bool operator !=(LongDecimal left, LongDeccomplex right) => right != left;
	public static bool operator !=(LongDeccomplex left, LongDecimal right) =>
		left.Real != right || left.Imaginary != LongDecimal.Zero;
	public static bool operator !=(LongDeccomplex left, LongDeccomplex right) =>
		left.Real != right.Real || left.Imaginary != right.Imaginary;
#pragma warning disable S3877
	static bool IComparisonOperators<LongDeccomplex, LongDeccomplex, bool>.operator >=(LongDeccomplex left,
		LongDeccomplex right) => throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<LongDeccomplex, LongDeccomplex, bool>.operator <=(LongDeccomplex left,
		LongDeccomplex right) => throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<LongDeccomplex, LongDeccomplex, bool>.operator >(LongDeccomplex left,
		LongDeccomplex right) => throw new NotSupportedException(NoComparisons);
	static bool IComparisonOperators<LongDeccomplex, LongDeccomplex, bool>.operator <(LongDeccomplex left,
		LongDeccomplex right) => throw new NotSupportedException(NoComparisons);
#pragma warning restore S3877
#pragma warning restore IDE0079 // Удалить ненужное подавление
}