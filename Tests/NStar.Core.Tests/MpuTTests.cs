using Mpir.NET;
using System.Numerics;

namespace NStar.Core.Tests;

[TestClass]
public class MpuTTests
{
	[TestMethod]
	public void Constructor_FromUInt32()
	{
		MpuT value = new(42U);
		Assert.AreEqual(42, value);
	}

	[TestMethod]
	public void Constructor_FromUInt64()
	{
		MpuT value = new(10000000000UL);
		Assert.AreEqual((BigInteger)10000000000, value.ToBigInteger());
	}

	[TestMethod]
	public void Constructor_FromBigInteger()
	{
		var big = BigInteger.Parse("12345678901234567890");
		MpuT value = new(big);
		Assert.AreEqual(big, value.ToBigInteger());
	}

	[TestMethod]
	public void ToString_ReturnsCorrectValue()
	{
		MpuT value = new(987654321U);
		Assert.AreEqual("987654321", value.ToString());
	}

	[TestMethod]
	public void Constructor_FromNegativeInt() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT(-1));

	[TestMethod]
	public void Constructor_FromNegativeLong() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT(-100L));

	[TestMethod]
	public void Constructor_FromNegativeBigInteger()
	{
		BigInteger negative = BigInteger.Parse("-12345");
		Assert.ThrowsExactly<ArgumentException>(() => new MpuT(negative));
	}

	[TestMethod]
	public void Constructor_FromNegativeDouble() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT(-3.14));

	[TestMethod]
	public void Constructor_FromZero()
	{
		var value = new MpuT(0);
		Assert.AreEqual(0, value.ToBigInteger());
	}

	[TestMethod]
	public void Constructor_FromPositiveDouble()
	{
		var value = new MpuT(42.99);
		Assert.AreEqual(42, value.ToBigInteger());
	}

	[TestMethod]
	public void Parse_NegativeDecimalString() => Assert.ThrowsExactly<ArgumentException>(() => MpuT.Parse("-42"));

	[TestMethod]
	public void Parse_NegativeWithWhitespace() => Assert.ThrowsExactly<ArgumentException>(() => MpuT.Parse(" -100 "));

	[TestMethod]
	public void Parse_ZeroString()
	{
		var value = MpuT.Parse("0");
		Assert.AreEqual(0, value.ToBigInteger());
	}

	[TestMethod]
	public void Parse_PositiveString()
	{
		var value = MpuT.Parse("  123  ");
		Assert.AreEqual(123, value.ToBigInteger());
	}

	[TestMethod]
	public void Parse_HexNegative_WithMinus() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT("-1A", 16));

	[TestMethod]
	public void Parse_HexNegative_TwoComplementSyntax() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT("  -  FF  ", 16));

	[TestMethod]
	public void Parse_HexPositive()
	{
		var value = new MpuT("1A", 16);  // 26
		Assert.AreEqual(26, value.ToBigInteger());
	}

	[TestMethod]
	public void Parse_HexUpperCase()
	{
		var value = new MpuT("FF", 16);  // 255
		Assert.AreEqual(255, value.ToBigInteger());
	}

	[TestMethod]
	public void Parse_OctalNegative_WithMinus_ThrowsArgumentException() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT("        - 77", 8));

	[TestMethod]
	public void Parse_OctalPositive_DoesNotThrow()
	{
		var value = new MpuT("77", 8);  // 63
		Assert.AreEqual(63, value.ToBigInteger());
	}

	[TestMethod]
	public void Parse_BinaryNegative_WithMinus_ThrowsArgumentException() => Assert.ThrowsExactly<ArgumentException>(() => new MpuT("    -  1010 ", 2));

	[TestMethod]
	public void Parse_BinaryPositive_DoesNotThrow()
	{
		var value = new MpuT("       1010", 2);  // 10
		Assert.AreEqual(10, value.ToBigInteger());
	}

	[TestMethod]
	public void UnaryMinus_ReturnsMpzT_NegativeValue()
	{
		MpuT a = new(5U);
		var result = -a;
		Assert.AreEqual(-5, result.ToBigInteger());
	}

	[TestMethod]
	public void BitwiseNot_ReturnsMpzT_Complement()
	{
		MpuT a = new(3U);
		var result = ~a;
								
		Assert.AreEqual(-4, result.ToBigInteger());
	}

	[TestMethod]
	public void Addition_Simple()
	{
		MpuT a = new(10U);
		MpuT b = new(20U);
		var result = a + b;
		Assert.AreEqual(30, result);
	}

	[TestMethod]
	public void Subtraction_PositiveResult()
	{
		MpuT a = new(50U);
		MpuT b = new(20U);
		var result = a - b;
		Assert.AreEqual(30, result);
	}

	[TestMethod]
	public void Subtraction_NegativeResult()
	{
		MpuT a = new(10U);
		MpuT b = new(20U);
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a - b);
	}
	[TestMethod]
	public void Add_MpuT_Plus_PositiveInt_ReturnsCorrectResult()
	{
		MpuT a = new(100U);
		var b = 50;
		var result = a + b;
		Assert.AreEqual(150, result);
	}

	[TestMethod]
	public void Add_MpuT_Plus_NegativeInt_PositiveResult_ReturnsCorrect()
	{
		MpuT a = new(100U);
		var b = -30;
		var result = a + b;
		Assert.AreEqual(70, result);
	}

	[TestMethod]
	public void Add_MpuT_Plus_NegativeInt_NegativeResult_Throws()
	{
		MpuT a = new(10U);
		var b = -20;
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a + b);
	}

	[TestMethod]
	public void Add_MpuT_MaxUInt32_Plus_Zero_ReturnsSame()
	{
		MpuT a = new(uint.MaxValue);
		var b = 0;
		var result = a + b;
		Assert.AreEqual((BigInteger)uint.MaxValue, result.ToBigInteger());
	}

	[TestMethod]
	public void Add_MpuT_Zero_Plus_NegativeInt_Throws()
	{
		MpuT a = new(0U);
		var b = -1;
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a + b);
	}

	[TestMethod]
	public void Add_MpuT_Plus_Uint_ReturnsCorrectResult()
	{
		MpuT a = new(50U);
		var b = 25U;
		var result = a + b;
		Assert.AreEqual(75, result);
	}

	[TestMethod]
	public void Add_MpuT_Zero_Plus_Uint_ReturnsUint()
	{
		MpuT a = new(0U);
		var b = 100U;
		var result = a + b;
		Assert.AreEqual(100, result);
	}

	[TestMethod]
	public void Add_MpuT_MaxValue_Plus_Zero_ReturnsMaxValue()
	{
		MpuT a = new((BigInteger)ulong.MaxValue);
		var b = 0U;
		var result = a + b;
		Assert.AreEqual((BigInteger)ulong.MaxValue, result.ToBigInteger());
	}

	[TestMethod]
	public void Add_LargeMpuT_Plus_LargeUint_NoOverflow()
	{
		MpuT a = new(BigInteger.Parse("123456789012345"));
		var b = uint.MaxValue;
		var result = a + b;
		Assert.AreEqual(BigInteger.Parse("123456789012345") + uint.MaxValue, result.ToBigInteger());
	}

	[TestMethod]
	public void Subtract_MpuT_Minus_PositiveInt_PositiveResult_ReturnsCorrect()
	{
		MpuT a = new(100U);
		var b = 30;
		var result = a - b;
		Assert.AreEqual(70, result);
	}

	[TestMethod]
	public void Subtract_MpuT_Minus_PositiveInt_NegativeResult_Throws()
	{
		MpuT a = new(10U);
		var b = 20;
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a - b);
	}

	[TestMethod]
	public void Subtract_MpuT_Minus_NegativeInt_EquivalentToAddition()
	{
		MpuT a = new(50U);
		var b = -15;
		var result = a - b;
		Assert.AreEqual(65, result);
	}

	[TestMethod]
	public void Subtract_MpuT_Zero_Minus_PositiveInt_Throws()
	{
		MpuT a = new(0U);
		var b = 1;
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a - b);
	}

	[TestMethod]
	public void Subtract_MpuT_SameValues_ReturnsZero()
	{
		MpuT a = new(42U);
		var b = 42;
		var result = a - b;
		Assert.AreEqual(0, result);
	}

	[TestMethod]
	public void Subtract_MpuT_GreaterThanUint_ReturnsPositive()
	{
		MpuT a = new(100U);
		var b = 25U;
		var result = a - b;
		Assert.AreEqual(75, result);
	}

	[TestMethod]
	public void Subtract_MpuT_EqualToUint_ReturnsZero()
	{
		MpuT a = new(50U);
		var b = 50U;
		var result = a - b;
		Assert.AreEqual(0, result);
	}

	[TestMethod]
	public void Subtract_MpuT_LessThanUint_Throws()
	{
		MpuT a = new(10U);
		var b = 20U;
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a - b);
	}

	[TestMethod]
	public void Subtract_MpuT_Zero_Minus_PositiveUint_Throws()
	{
		MpuT a = new(0U);
		var b = 1U;
		MpuT result;
		Assert.ThrowsExactly<OverflowException>(() => result = a - b);
	}

	[TestMethod]
	public void Subtract_LargeMpuT_Minus_SmallUint_ReturnsCorrect()
	{
		MpuT a = new(BigInteger.Parse("9876543210987654321"));
		var b = 1000U;
		var result = a - b;
		Assert.AreEqual(BigInteger.Parse("9876543210987653321"), result.ToBigInteger());
	}

	[TestMethod]
	public void Multiplication_Simple()
	{
		MpuT a = new(7U);
		MpuT b = new(8U);
		var result = a * b;
		Assert.AreEqual(56, result);
	}

	[TestMethod]
	public void Division_Simple()
	{
		MpuT a = new(100U);
		MpuT b = new(4U);
		var result = a / b;
		Assert.AreEqual(25, result);
	}

	[TestMethod]
	public void Modulo_Simple()
	{
		MpuT a = new(23U);
		MpuT b = new(5U);
		var result = a % b;
		Assert.AreEqual(3, result);
	}

	[TestMethod]
	public void BitwiseAnd_Simple()
	{
		MpuT a = new(0b1101);
		MpuT b = new(0b1011);
		var result = a & b;
		Assert.AreEqual(0b1001, result);
	}

	[TestMethod]
	public void BitwiseOr_Simple()
	{
		MpuT a = new(0b1101);
		MpuT b = new(0b1011);
		var result = a | b;
		Assert.AreEqual(0b1111, result);
	}

	[TestMethod]
	public void BitwiseXor_Simple()
	{
		MpuT a = new(0b1101);
		MpuT b = new(0b1011);
		var result = a ^ b;
		Assert.AreEqual(0b0110, result);
	}

	[TestMethod]
	public void LeftShift_Simple()
	{
		MpuT a = new(1U);
		var result = a << 3;
		Assert.AreEqual(8, result);
	}

	[TestMethod]
	public void RightShift_Simple()
	{
		MpuT a = new(16U);
		var result = a >> 2;
		Assert.AreEqual(4, result);
	}

	[TestMethod]
	public void Equal_SameValues()
	{
		MpuT a = new(42U);
		MpuT b = new(42U);
		Assert.IsTrue(a == b);
	}

	[TestMethod]
	public void NotEqual_DifferentValues()
	{
		MpuT a = new(10U);
		MpuT b = new(20U);
		Assert.IsTrue(a != b);
	}

	[TestMethod]
	public void GreaterThan_Simple()
	{
		MpuT a = new(50U);
		MpuT b = new(30U);
		Assert.IsTrue(a > b);
		Assert.IsTrue(a >= b);
		b = new(50U);
		Assert.IsFalse(a > b);
		Assert.IsTrue(a >= b);
		b = new(70U);
		Assert.IsFalse(a > b);
		Assert.IsFalse(a >= b);
	}

	[TestMethod]
	public void LessThan_Simple()
	{
		MpuT a = new(15U);
		MpuT b = new(25U);
		Assert.IsTrue(a < b);
		Assert.IsTrue(a <= b);
		b = new(15U);
		Assert.IsFalse(a < b);
		Assert.IsTrue(a <= b);
		b = new(5U);
		Assert.IsFalse(a < b);
		Assert.IsFalse(a <= b);
	}

	[TestMethod]
	public void VeryBigBitLength()
	{
		MpuT z = 3;
		Assert.AreEqual(3403681052, z.Power(2147483647).GetFullBitLength());
	}
}
