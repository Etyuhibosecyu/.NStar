global using NStar.Core;
global using NStar.Linq;
global using NStar.Mpir;
global using System;
global using System.Buffers;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Threading;
global using static NStar.Core.Extents;
global using static NStar.Mpir.MpzT;
global using static RedStarMath.Internal;
using McNeight;

namespace RedStarMath;

public static class Math
{
	public const double E = System.Math.E, Pi = System.Math.PI, Tau = System.Math.Tau;
	public const double Ln2 = 0.6931471805599453d, Ln10 = 2.3025850929940457d, Log10of2 = 0.3010299956639812d;
	public const decimal DecimalE = MathM.E, DecimalPi = MathM.PI, DecimalTau = 2 * MathM.PI;
	public const decimal DecimalLn2 = 0.6931471805599453094172321215m, DecimalLn10 = 2.302585092994045684017991455m;
	public const decimal DecimalLog10of2 = 0.3010299956639811952137388947m;
	public const decimal DecimalMin = -79_228_162_514_264_337_593_543_950_335m;
	public const decimal DecimalMax = 79_228_162_514_264_337_593_543_950_335m;
	public const decimal DecimalEpsilon = 0.0000000000000000000000000001m;

	/// <inheritdoc cref="System.Math.Abs(decimal)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Abs(this decimal value) => System.Math.Abs(value);
	/// <inheritdoc cref="System.Math.Abs(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Abs(this double value) => System.Math.Abs(value);
	/// <inheritdoc cref="System.Math.Abs(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Abs(this int value) => System.Math.Abs(value);
	/// <inheritdoc cref="System.Math.Abs(long)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Abs(this long value) => System.Math.Abs(value);
	/// <inheritdoc cref="System.Math.Abs(short)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Abs(this short value) => System.Math.Abs(value);

	/// <summary>
	/// Вычисляет арккосинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - <see cref="Pi"/> &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Acos(this decimal value) => (decimal)LongDecimal.Acos(value);

	/// <summary>
	/// Вычисляет арккосинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - <see cref="Pi"/> &gt;&gt; 1;<br />
	/// для плюс бесконечности, минус бесконечности и неопределенности - неопределенность;<br />
	/// для чисел, модуль которых больше 1 - неопределенность;<br />
	/// в остальных случаях - арккосинус данного числа.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Acos(this double value) => System.Math.Acos(value);

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Acosh(this decimal value) => (decimal)LongDecimal.Acosh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Acosh(this double value) => System.Math.Acosh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Asin(this decimal value) => (decimal)LongDecimal.Asin(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Asin(this double value) => System.Math.Asin(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Asinh(this decimal value) => (decimal)LongDecimal.Asinh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Asinh(this double value) => System.Math.Asinh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Atan(this decimal value) => (decimal)LongDecimal.Atan(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Atan(this double value) => System.Math.Atan(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Atan2(decimal y, decimal x)
	{
		if (x == 0m && y == 0m)
			return 0m;
		if (x == 0m)
		{
			if (y > 0m)
				return 1.5707963267948966192313216916m;
			return -1.5707963267948966192313216916m;
		}
		if (y == 0m)
		{
			if (x <= 0m)
				return 3.1415926535897932384626433833m;
			return 0m;
		}
		if (MathM.Abs(x) < 1 && y > MathM.Abs(x) * decimal.MaxValue)
			return 1.5707963267948966192313216916m;
		if (MathM.Abs(x) < 1 && y < MathM.Abs(x) * decimal.MinValue)
			return -1.5707963267948966192313216916m;
		var num = MathM.Atan(y / x);
		if (x > 0m)
			return num;
		if (y <= 0m)
			return num - 3.1415926535897932384626433833m;
		return num + 3.1415926535897932384626433833m;
	}

	/// <inheritdoc cref="System.Math.Atan2(double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Atan2(double y, double x) => System.Math.Atan2(y, x);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Atanh(this decimal value) => (decimal)LongDecimal.Atanh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Atanh(this double value) => System.Math.Atanh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Cbrt(this decimal value) => MathM.Cbrt(value);

	/// <inheritdoc cref="System.Math.Cbrt(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Cbrt(this double value) => System.Math.Cbrt(value);

	/// <summary>
	/// Возвращает наименьшее целое число, которое не меньше указанного числа:
	/// само число <paramref name="value"/> для целых и ближайшее сверху целое для дробных.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для целых чисел - <paramref name="value"/>;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Ceiling(this decimal value) => System.Math.Ceiling(value);

	/// <summary>
	/// Возвращает наименьшее целое число, которое не меньше указанного числа:
	/// само число <paramref name="value"/> для целых и ближайшее сверху целое для дробных.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - <paramref name="value"/>;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Ceiling(this double value) => System.Math.Ceiling(value);

	/// <inheritdoc cref="System.Math.Clamp(byte, byte, byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Clamp(this byte value, byte min, byte max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(decimal, decimal, decimal)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Clamp(this decimal value, decimal min, decimal max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(double, double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Clamp(this double value, double min, double max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(int, int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Clamp(this int value, int min, int max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(long, long, long)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Clamp(this long value, long min, long max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(short, short, short)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Clamp(this short value, short min, short max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(uint, uint, uint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Clamp(this uint value, uint min, uint max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(ulong, ulong, ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Clamp(this ulong value, ulong min, ulong max) => System.Math.Clamp(value, min, max);

	/// <inheritdoc cref="System.Math.Clamp(ushort, ushort, ushort)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Clamp(this ushort value, ushort min, ushort max) => System.Math.Clamp(value, min, max);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Cos(this decimal value) => (decimal)LongDecimal.Cos(value);

	/// <inheritdoc cref="System.Math.Cos(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Cos(this double value) => System.Math.Cos(value);

	/// <summary>
	/// Вычисляет гиперболический косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// в остальных случаях - гиперболический косинус <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Cosh(this decimal value) => (decimal)LongDecimal.Cosh(value);

	/// <summary>
	/// Вычисляет гиперболический косинус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// для плюс бесконечности и минус бесконечности - плюс бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - гиперболический косинус <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Cosh(this double value) => System.Math.Cosh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (byte Quotient, byte Remainder) DivRem(this byte left, byte right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (int Quotient, int Remainder) DivRem(this int left, int right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (long Quotient, long Remainder) DivRem(this long left, long right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (short Quotient, short Remainder) DivRem(this short left, short right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (uint Quotient, uint Remainder) DivRem(this uint left, uint right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (ulong Quotient, ulong Remainder) DivRem(this ulong left, ulong right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (ushort Quotient, ushort Remainder) DivRem(this ushort left, ushort right) => System.Math.DivRem(left, right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int DivRem(this int left, int right, out int result) => System.Math.DivRem(left, right, out result);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long DivRem(this long left, long right, out long result) => System.Math.DivRem(left, right, out result);

	/// <summary>
	/// Вычисляет e в степени указанного числа (экспоненту).
	/// </summary>
	/// <param name="value">Показатель для вычисления экспоненты.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// в остальных случаях - e в степени <paramref name="value"/> (экспонента) (возможно <see cref="OverflowException"/>).
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Exp(this decimal value) => (decimal)LongDecimal.Exp(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Exp(this double value) => System.Math.Exp(value);

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше указанного числа:
	/// само число <paramref name="value"/> для целых и ближайшее снизу целое для дробных.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для целых чисел - <paramref name="value"/>;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Floor(this decimal value) => System.Math.Floor(value);

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше указанного числа:
	/// само число <paramref name="value"/> для целых и ближайшее снизу целое для дробных.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - <paramref name="value"/>;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Floor(this double value) => System.Math.Floor(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal IEEERemainder(this decimal x, decimal y) => MathM.IEEERemainder(x, y);

	/// <inheritdoc cref="System.Math.IEEERemainder(double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double IEEERemainder(this double x, double y) => System.Math.IEEERemainder(x, y);

	/// <summary>
	/// Вычисляет натуральный логарифм указанного числа (по основанию e).
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <returns>
	/// Для нуля - <see cref="OverflowException"/>;<br />
	/// для отрицательных чисел - <see cref="OverflowException"/>;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - натуральный логарифм <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Log(this decimal value) => (decimal)LongDecimal.Ln(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Log(this double value) => System.Math.Log(value);

	/// <summary>
	/// Вычисляет логарифм данного числа по основанию <paramref name="base"/>.
	/// </summary>
	/// <param name="base">Основание логарифма.</param>
	/// <returns>
	/// Для нуля - <see cref="OverflowException"/>;<br />
	/// для отрицательных чисел - <see cref="OverflowException"/>;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм данного числа по основанию <paramref name="base"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Log(this decimal value, decimal @base) => (decimal)LongDecimal.Log(value, @base);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Log(this double value, double @base) => System.Math.Log(value, @base);

	/// <summary>
	/// Вычисляет логарифм указанного числа по основанию 2.
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <returns>
	/// Для нуля - <see cref="OverflowException"/>;<br />
	/// для отрицательных чисел - <see cref="OverflowException"/>;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм <paramref name="value"/> по основанию 2.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Log2(this decimal value) => (decimal)LongDecimal.Log2(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Log2(this double value) => System.Math.Log2(value);

	/// <summary>
	/// Вычисляет логарифм указанного числа по основанию 10.
	/// </summary>
	/// <param name="value">Число для вычисления логарифма.</param>
	/// <returns>
	/// Для нуля - <see cref="OverflowException"/>;<br />
	/// для отрицательных чисел - <see cref="OverflowException"/>;<br />
	/// для единицы - ноль;<br />
	/// в остальных случаях - логарифм <paramref name="value"/> по основанию 10.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Log10(this decimal value) => (decimal)LongDecimal.Log10(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Log10(this double value) => System.Math.Log10(value);

	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Max(byte x) => x;
	/// <inheritdoc cref="System.Math.Max(byte, byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Max(byte x, byte y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Max(byte x, byte y, byte z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Max(decimal x) => x;
	/// <inheritdoc cref="System.Math.Max(decimal, decimal)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Max(decimal x, decimal y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Max(decimal x, decimal y, decimal z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Max(double x) => x;
	/// <inheritdoc cref="System.Math.Max(double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Max(double x, double y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Max(double x, double y, double z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Max(int x) => x;
	/// <inheritdoc cref="System.Math.Max(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Max(int x, int y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Max(int x, int y, int z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Max(long x) => x;
	/// <inheritdoc cref="System.Math.Max(long, long)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Max(long x, long y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Max(long x, long y, long z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Max(short x) => x;
	/// <inheritdoc cref="System.Math.Max(short, short)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Max(short x, short y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Max(short x, short y, short z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Max(uint x) => x;
	/// <inheritdoc cref="System.Math.Max(uint, uint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Max(uint x, uint y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Max(uint x, uint y, uint z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Max(ulong x) => x;
	/// <inheritdoc cref="System.Math.Max(ulong, ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Max(ulong x, ulong y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Max(ulong x, ulong y, ulong z) => Max(Max(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Max(ushort x) => x;
	/// <inheritdoc cref="System.Math.Max(ushort, ushort)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Max(ushort x, ushort y) => System.Math.Max(x, y);
	/// <summary>Возвращает максимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Max(ushort x, ushort y, ushort z) => Max(Max(x, y), z);

	/// <inheritdoc cref="System.Math.MaxMagnitude(double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double MaxMagnitude(double x, double y) => System.Math.MaxMagnitude(x, y);

	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Min(byte x) => x;
	/// <inheritdoc cref="System.Math.Min(byte, byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Min(byte x, byte y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Min(byte x, byte y, byte z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Min(decimal x) => x;
	/// <inheritdoc cref="System.Math.Min(decimal, decimal)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Min(decimal x, decimal y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Min(decimal x, decimal y, decimal z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Min(double x) => x;
	/// <inheritdoc cref="System.Math.Min(double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Min(double x, double y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Min(double x, double y, double z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Min(int x) => x;
	/// <inheritdoc cref="System.Math.Min(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Min(int x, int y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Min(int x, int y, int z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Min(long x) => x;
	/// <inheritdoc cref="System.Math.Min(long, long)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Min(long x, long y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Min(long x, long y, long z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Min(short x) => x;
	/// <inheritdoc cref="System.Math.Min(short, short)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Min(short x, short y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Min(short x, short y, short z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Min(uint x) => x;
	/// <inheritdoc cref="System.Math.Min(uint, uint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Min(uint x, uint y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Min(uint x, uint y, uint z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Min(ulong x) => x;
	/// <inheritdoc cref="System.Math.Min(ulong, ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Min(ulong x, ulong y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Min(ulong x, ulong y, ulong z) => Min(Min(x, y), z);
	/// <summary>Возвращает число x. Метод-заглушка, чтобы не удалять имя метода, если не осталось второго параметра.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Min(ushort x) => x;
	/// <inheritdoc cref="System.Math.Min(ushort, ushort)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Min(ushort x, ushort y) => System.Math.Min(x, y);
	/// <summary>Возвращает минимальное из трех значений.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Min(ushort x, ushort y, ushort z) => Min(Min(x, y), z);

	/// <inheritdoc cref="System.Math.MinMagnitude(double, double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double MinMagnitude(double x, double y) => System.Math.MinMagnitude(x, y);

	/// <summary>
	/// Вычисляет указанное основание в степени указанного показателя.
	/// </summary>
	/// <param name="base">Основание для вычисления степени.</param>
	/// <param name="exponent">Показатель для вычисления степени.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// в остальных случаях - <paramref name="base"/> в степени <paramref name="exponent"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Power(this decimal @base, decimal exponent) => (decimal)LongDecimal.Power(@base, exponent);

	/// <summary>
	/// Вычисляет указанное основание в степени указанного показателя.
	/// </summary>
	/// <param name="base">Основание для вычисления степени.</param>
	/// <param name="exponent">Показатель для вычисления степени.</param>
	/// <returns>
	/// Для нуля - единица;<br />
	/// в остальных случаях - <paramref name="base"/> в степени <paramref name="exponent"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Power(this decimal @base, int exponent)
	{
		switch (exponent)
		{
			case 0:
			return 1m;
			case 1:
			return @base;
			case 2:
			return @base * @base;
			case 3:
			return @base * @base * @base;
			case 4:
			@base *= @base;
			return @base * @base;
			case 5:
			var square = @base * @base;
			return square * square * @base;
			case 6:
			@base *= @base;
			return @base * @base * @base;
			case 7:
			var cube = @base * @base * @base;
			return cube * cube * @base;
			case 8:
			@base *= @base;
			@base *= @base;
			return @base * @base;
			case 9:
			@base = @base * @base * @base;
			return @base * @base * @base;
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
			for (var i = BitsPerInt - int.LeadingZeroCount(exponent); i >= 0; i--)
			{
				result *= result;
				if ((exponent & 1u << i) != 0)
					result *= @base;
			}
			return result;
		}
	}

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Power(this double @base, double exponent) => System.Math.Pow(@base, exponent);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Power(this double @base, int exponent)
	{
		switch (exponent)
		{
			case 0:
			return 1d;
			case 1:
			return @base;
			case 2:
			return @base * @base;
			case 3:
			return @base * @base * @base;
			case 4:
			@base *= @base;
			return @base * @base;
			case 5:
			var square = @base * @base;
			return square * square * @base;
			case 6:
			@base *= @base;
			return @base * @base * @base;
			case 7:
			var cube = @base * @base * @base;
			return cube * cube * @base;
			case 8:
			@base *= @base;
			@base *= @base;
			return @base * @base;
			case 9:
			@base = @base * @base * @base;
			return @base * @base * @base;
			default:
			if (@base.Equals(1d))
				return @base;
			var negative = false;
			if (exponent < 0)
			{
				negative = true;
				exponent = -exponent;
			}
			if (@base.Equals(0d))
				return negative ? throw new DivideByZeroException() : 0.0d;
			if (@base.Equals(1.0d))
				return @base;
			if (@base.Equals(-1.0d))
				return (exponent & 1) == 0 ? 1.0d : -1.0d;
			if (negative)
				@base = 1.0d / @base;
			var result = 1d;
			for (var i = BitsPerInt - int.LeadingZeroCount(exponent); i >= 0; i--)
			{
				result *= result;
				if ((exponent & 1u << i) != 0)
					result *= @base;
			}
			return result;
		}
	}

	/// <summary>
	/// Вычисляет число, обратное указанному (1 / <paramref name="value"/>).
	/// </summary>
	/// <param name="value">Число для вычисления обратного.</param>
	/// <returns>
	/// Для нуля - <see cref="OverflowException"/>;<br />
	/// в остальных случаях - число, обратное данному (1 / <paramref name="value"/>).
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Reciproc(this decimal value) => 1m / value;

	/// <summary>
	/// Вычисляет число, обратное указанному (1 / <paramref name="value"/>).
	/// </summary>
	/// <param name="value">Число для вычисления обратного.</param>
	/// <returns>
	/// Для нуля - плюс бесконечность;<br />
	/// для плюс бесконечности или для минус бесконечности - ноль;<br />
	/// для неопределенности - неопределенность;<br />
	/// в остальных случаях - число, обратное данному (1 / <paramref name="value"/>).
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Reciproc(this double value) => 1d / value;

	/// <summary>
	/// Возвращает целое число, ближайшее к <paramref name="value"/>. Если два целых числа одинаково близки к нему
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое является четным.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Round(this decimal value) => System.Math.Round(value);

	/// <summary>
	/// Возвращает целое число, ближайшее к <paramref name="value"/>. Если два целых числа одинаково близки к нему
	/// (дробная часть точно равна 0.5 или -0.5), возвращает то из них, которое является четным.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Round(this double value) => System.Math.Round(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Shift(this decimal x, int n) => n switch
	{
		0 => x,
		> 0 => x * Power(2m, n),
		< 0 => x / Power(2m, -n),
	};

	/// <inheritdoc cref="System.Math.ScaleB(double, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Shift(this double x, int n) => System.Math.ScaleB(x, n);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal ShiftDec(this decimal x, int n) => x * Power(10m, n);

	/// <inheritdoc cref="System.Math.Sign(decimal)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(this decimal value) => System.Math.Sign(value);
	/// <inheritdoc cref="System.Math.Sign(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(this double value) => System.Math.Sign(value);
	/// <inheritdoc cref="System.Math.Sign(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(this int value) => System.Math.Sign(value);
	/// <inheritdoc cref="System.Math.Sign(long)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(this long value) => System.Math.Sign(value);
	/// <inheritdoc cref="System.Math.Sign(short)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(this short value) => System.Math.Sign(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Sin(this decimal value) => (decimal)LongDecimal.Sin(value);

	/// <inheritdoc cref="System.Math.Sin(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Sin(this double value) => System.Math.Sin(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (decimal Sin, decimal Cos) SinCos(this decimal x)
	{
		var sin = Sin(x);
		return (sin, Sqrt(1 - sin * sin) * (Abs(x) is > DecimalPi / 2 and < DecimalTau - DecimalPi / 2 ? -1 : 1));
	}

	/// <inheritdoc cref="System.Math.SinCos(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (double Sin, double Cos) SinCos(this double x) => System.Math.SinCos(x);

	/// <summary>
	/// Вычисляет гиперболический синус указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// в остальных случаях - гиперболический синус <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Sinh(this decimal value) => (decimal)LongDecimal.Sinh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Sinh(this double value) => System.Math.Sinh(value);

	/// <summary>
	/// Вычисляет квадратный корень указанного числа.
	/// </summary>
	/// <param name="value">Число для извлечения квадратного корня.</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для отрицательных чисел - <see cref="OverflowException"/>;<br />
	/// в остальных случаях - арифметический квадратный корень <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Sqrt(this decimal value)
	{
		if (value < 0)
			throw new ArgumentOutOfRangeException(nameof(value), "Квадратный корень из отрицательного числа не определен.");
		if (value == 0m)
			return 0m;
		if (value == 1m)
			return 1m;
		var guess = MathM.Sqrt(value); // Начальное приближение
		while (true)
		{
			var previous = guess;
			guess = (previous + value / previous) / 2;
			if (System.Math.Abs(previous - guess) < 1e-28m) // Условие сходимости
				return guess;
		}
	}

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Sqrt(this double value) => System.Math.Sqrt(value);

	/// <summary>
	/// Вычисляет квадрат указанного числа.
	/// </summary>
	/// <param name="value">Число для извлечения квадратного корня.</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// в остальных случаях - квадрат <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Square(this decimal value) => value * value;

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Square(this double value) => value * value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Tan(this decimal value) => (decimal)LongDecimal.Tan(value);

	/// <inheritdoc cref="System.Math.Sin(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Tan(this double value) => System.Math.Tan(value);

	/// <summary>
	/// Вычисляет гиперболический тангенс указанного числа.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// в остальных случаях - гиперболический тангенс <paramref name="value"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Tanh(this decimal value) => (decimal)LongDecimal.Tanh(value);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Tanh(this double value) => System.Math.Tanh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this byte value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this int value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this long value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this MpzT value) => (char)(ushort)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this MpuT value) => (char)(ushort)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this short value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this uint value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this ulong value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char ToChar(this ushort value) => (char)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal ToDecimal(this double value) => decimal.Parse(value.ToString("F28").Take(29).AsSpan());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double ToReal(this decimal value) => double.Parse(value.ToString("F28"));

	/// <summary>
	/// Преобразует указанное число из знакового типа в ближайший беззнаковый:
	/// для short int - unsigned short int, для int - unsigned int, для long int - unsigned long int,
	/// для long long - unsigned long long.
	/// </summary>
	/// <param name="value">Число, которое нужно преобразовать в беззнаковый тип.</param>
	/// <returns>Для нуля - ноль;<br />
	/// для положительных чисел - беззнаковый эквивалент этого числа;<br />
	/// для отрицательных чисел - беззнаковый эквивалент модуля этого числа.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ToUnsigned(this int value) => (uint)Abs(value);

	/// <summary>
	/// Преобразует указанное число из знакового типа в ближайший беззнаковый:
	/// для short int - unsigned short int, для int - unsigned int, для long int - unsigned long int,
	/// для long long - unsigned long long.
	/// </summary>
	/// <param name="value">Число, которое нужно преобразовать в беззнаковый тип.</param>
	/// <returns>Для нуля - ноль;<br />
	/// для положительных чисел - беззнаковый эквивалент этого числа;<br />
	/// для отрицательных чисел - беззнаковый эквивалент модуля этого числа.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong ToUnsigned(this long value) => (ulong)Abs(value);

	/// <summary>
	/// Преобразует указанное число из знакового типа в ближайший беззнаковый:
	/// для short int - unsigned short int, для int - unsigned int, для long int - unsigned long int,
	/// для long long - unsigned long long.
	/// </summary>
	/// <param name="value">Число, которое нужно преобразовать в беззнаковый тип.</param>
	/// <returns>Для нуля - ноль;<br />
	/// для положительных чисел - беззнаковый эквивалент этого числа;<br />
	/// для отрицательных чисел - беззнаковый эквивалент модуля этого числа.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MpuT ToUnsigned(this MpzT value) => (MpuT)value.Abs();

	/// <summary>
	/// Преобразует указанное число из знакового типа в ближайший беззнаковый:
	/// для short int - unsigned short int, для int - unsigned int, для long int - unsigned long int,
	/// для long long - unsigned long long.
	/// </summary>
	/// <param name="value">Число, которое нужно преобразовать в беззнаковый тип.</param>
	/// <returns>Для нуля - ноль;<br />
	/// для положительных чисел - беззнаковый эквивалент этого числа;<br />
	/// для отрицательных чисел - беззнаковый эквивалент модуля этого числа.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort ToUnsigned(this short value) => (ushort)Abs(value);

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше указанного числа, для положительных,
	/// и наименьшее целое число, которое не меньше указанного числа, для отрицательных (для нуля, если это непонятно, ноль).
	/// Другими словами, возвращает целую часть указанного числа (<paramref name="value"/>), отбрасывая дробную.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Truncate(this decimal value) => System.Math.Truncate(value);

	/// <summary>
	/// Возвращает наибольшее целое число, которое не больше указанного числа, для положительных,
	/// и наименьшее целое число, которое не меньше указанного числа, для отрицательных (для нуля, если это непонятно, ноль).
	/// Другими словами, возвращает целую часть указанного числа (<paramref name="value"/>), отбрасывая дробную.
	/// </summary>
	/// <param name="value">Число, являющееся аргументом данной функции
	/// (эта функция статическая и зависит только от аргумента).</param>
	/// <returns>
	/// Для нуля - ноль;<br />
	/// для плюс бесконечности - плюс бесконечность;<br />
	/// для минус бесконечности - минус бесконечность;<br />
	/// для неопределенности - неопределенность;<br />
	/// для целых чисел - данное число;<br />
	/// в остальных случаях - см. общее описание.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Truncate(this double value) => System.Math.Truncate(value);
}
