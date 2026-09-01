namespace RedStarMath;

internal static class Internal
{
    internal const string NoDivisionByZero = "Этот тип не поддерживает деление на ноль.";
    internal const string NoNegativeNumbers = "Этот тип не поддерживает отрицательные числа.";
    internal const string SpecialValuesConversionError = "Это преобразование не поддерживает бесконечность и неопределенность.";
    internal const string ULRConversionError = "Это преобразование не поддерживает бесконечность,"
        + " неопределенность и отрицательные числа.";
    internal static readonly MpuT MpuTTwo = new(2);
    internal static MpzT MpuTPowerOf5(int exponent) => Unsafe.As<MpzT>(MpuT.PowerOf5(exponent));
    internal static MpzT MpuTPowerOf10(int exponent) => Unsafe.As<MpzT>(MpuT.PowerOf10(exponent));
}
