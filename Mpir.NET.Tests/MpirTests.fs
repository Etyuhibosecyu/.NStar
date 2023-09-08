module MpirTests

open System
open Mpir.NET
open NUnit.Framework

[<AutoOpen>]
module NumericLiteralZ =
    let FromZero () = new MpzT(0)
    let FromOne ()  = new MpzT(1)
    let FromInt32 (n : int32)   = new MpzT(n)
    let FromInt64 (n : int64)   = new MpzT(n)
    let FromString (s : string) = new MpzT(s)

let bigNumLiteral1 = 60239597246800183089356887648914080803568478687972971429218563117893654732155483254Z
let bigNumStr1 =    "60239597246800183089356887648914080803568478687972971429218563117893654732155483254"
let bigint1 = bigint.Parse bigNumStr1

// A more readable way to concatenate arrays.
let inline private (++) (a:^T[]) (b:^T[]) = Array.append a b


type ``MpzT - literals`` () =
    [<Test>]
    static member ``Large literal`` () =
        let z = bigNumLiteral1
        let zStr = Mpir.MpzGetString(10u, z)
        Assert.AreEqual(bigNumStr1, zStr)

type ``MpzT - import and export`` () =
    [<Test>]
    static member ``Importing BigInteger`` () =
        let z = new MpzT(bigint1.ToByteArray(), -1)
        let zStr = Mpir.MpzGetString(10u, z)
        Assert.AreEqual(bigNumStr1, zStr)

    [<Test>]
    static member ``Exporting BigInteger`` () =
        let z = new MpzT(bigNumStr1)
        let t = z.ToBigInteger()
        Assert.AreEqual(bigint1, t)

    [<Test>]
    static member ``Importing bigint bytes, big endian`` () =
        let a = bigint.Parse(bigNumStr1)
        let z = new MpzT(a.ToByteArray() |> Array.rev, 1)

        let zStr = Mpir.MpzGetString(10u, z)
        Assert.AreEqual(bigNumStr1, zStr)

    [<Test>]
    static member ``Importing bigint bytes, little endian`` () =
        let a = bigint.Parse(bigNumStr1)
        let z = new MpzT(a.ToByteArray(), -1)

        let zStr = Mpir.MpzGetString(10u, z)
        Assert.AreEqual(bigNumStr1, zStr)

    [<Test>]
    static member ``Exporting mpz, big endian`` () =
        // Make a bigint from the exported byte array and compare to that bigint's ToString()
        // rather than comparing to a bigint.Parse(bigNumStr1).ToByteArray(), as bigint.ToByteArray 
        // may or may not put a leading 0x00 first to indicate positive sign depending on MSB.
        let z = new MpzT()
        Mpir.MpzSetStr(z, bigNumStr1, 10u) |> ignore

        let bytes = z.ToByteArray(1)
        let exportStr = (bigint( [|0uy|] ++ bytes |> Array.rev )).ToString()

        Assert.AreEqual(bigNumStr1, exportStr)

    [<Test>]
    static member ``Exporting mpz, little endian`` () =
        let z = new MpzT()
        Mpir.MpzSetStr(z, bigNumStr1, 10u) |> ignore

        let bytes = z.ToByteArray(-1)
        let exportStr = (bigint(bytes)).ToString()

        Assert.AreEqual(bigNumStr1, exportStr)

    [<TestCase(18446744073709551615UL)>]
    [<TestCase( 9223372036854775807UL)>] 
    [<TestCase( 4887567363547568832UL)>] 
    [<TestCase(                   0UL)>] 
    static member ``Importing uint64, big endian`` (n : uint64) = 
        let bytes = BitConverter.GetBytes(n)
        let bigEndianBytes =
            if BitConverter.IsLittleEndian then Array.rev bytes
            else bytes
        let z = new MpzT(bigEndianBytes, 1)

        let zStr = Mpir.MpzGetString(10u, z)
        Assert.AreEqual(n.ToString(), zStr)

    [<TestCase(18446744073709551615UL)>]
    [<TestCase( 9223372036854775807UL)>] 
    [<TestCase( 4887567363547568832UL)>] 
    [<TestCase(                   0UL)>] 
    static member ``Importing uint64, little endian`` (n : uint64) = 
        let bytes = BitConverter.GetBytes(n)
        let littleEndianBytes =
            if BitConverter.IsLittleEndian then bytes
            else Array.rev bytes
        let z = new MpzT(littleEndianBytes, -1)

        let zStr = Mpir.MpzGetString(10u, z)
        Assert.AreEqual(n.ToString(), zStr)


type ``MpzT - casts`` () =
    [<Test>]
    static member ``MpzT to long`` () =
        let tstVal : int64 = 0x7F00ABCDEA007851L
        let a = new MpzT(tstVal)
        let b = (int64 a)
        Assert.AreEqual(tstVal, b)

    [<Test>]
    static member ``MpzT to ulong`` () =
        let tstVal : uint64 = 0xFF00ABCDEA007851UL
        let a = new MpzT(tstVal)
        let b = (uint64 a)
        Assert.AreEqual(tstVal, b)


type ``MpzT - operations`` () =
    [<TestCase("67907490790576908375907590346925623895", "67907490790576908375907590346925623895")>]
    [<TestCase("-99943967907490790576908375907590346925623895", "99943967907490790576908375907590346925623895")>]
    [<TestCase("-1", "1")>]
    static member ``Mpir.Abs``(a: string, b: string) =
        let za = new MpzT(a)
        let zb = new MpzT(b)
        let result = za.Abs()
        Assert.AreEqual(result, zb)

    [<TestCase("43967907490790576908375907590346925623895", "67907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-99943967907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-9994396790895")>]
    static member ``Mpir.Max``(a: string, b: string) =
        let za = new MpzT(a)
        let zb = new MpzT(b)
        let max = Mpir.Max(za, zb)
        Assert.AreEqual(za, max)

    [<TestCase("43967907490790576908375907590346925623895", "67907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-99943967907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-9994396790895")>]
    static member ``Mpir.Min``(a: string, b: string) =
        let za = new MpzT(a)
        let zb = new MpzT(b)
        let min = Mpir.Min(za, zb)
        Assert.AreEqual(zb, min)

    [<Test>]
    static member ``PowerMod with negative exponent``() =
        let za = 3Z
        let zb = 7Z
        let actual = za.PowerMod(-1, zb)
        let expected = 5Z
        Assert.AreEqual(expected, actual)

    [<Test>]
    static member ``Very big BitLength``() =
        let za = 3Z
        let actual = za.Power(2147483647).GetFullBitLength()
        let expected = 3403681052Z
        Assert.AreEqual(expected, actual)
[<EntryPoint>]
let main _ =
    0
