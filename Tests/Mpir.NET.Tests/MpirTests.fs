module MpirTests

open System
open System.Linq
open Mpir.NET
open NUnit.Framework

[<AutoOpen>]
module NumericLiteralZ =
    let FromZero () = new MpzT(0)
    let FromOne () = new MpzT(1)
    let FromInt32 (n : int32)   = new MpzT(n)
    let FromInt64 (n : int64)   = new MpzT(n)
    let FromString (s : string) = new MpzT(s)

let bigNumLiteral1 = 60239597246800183089356887648914080803568478687972971429218563117893654732155483254Z
let bigNumStr1 = "60239597246800183089356887648914080803568478687972971429218563117893654732155483254"
let bigint1 = bigint.Parse bigNumStr1
let byte1 = byte (bigint1 % bigint 256)
let short1 = int16 (bigint1 % bigint 65536)
let ushort1 = uint16 (bigint1 % bigint 65536)
let int1 = int32 (bigint1 % bigint 4294967296L)
let uint1 = uint32 (bigint1 % bigint 4294967296L)
let ulong1 = uint64 (bigint1 % bigint.Parse "18446744073709551616")
let long1 = int64 ulong1
let byteArray : byte[] = Array.zeroCreate 8
let random = Random 1234567890

// A more readable way to concatenate arrays.
let inline private (++) (a:^T[]) (b:^T[]) = Array.append a b


type ``MpzT - literals`` () =
    [<Test>]
    static member ``Large literal`` () =
        let z = bigNumLiteral1
        let zStr = Mpir.MpzGetString(10u, z)
        Assert.That(bigNumStr1, Is.EqualTo(zStr))

type ``MpzT - import and export`` () =
    [<Test>]
    static member ``Importing BigInteger`` () =
        let z = new MpzT(bigint1.ToByteArray(), -1)
        let zStr = Mpir.MpzGetString(10u, z)
        Assert.That(bigNumStr1, Is.EqualTo(zStr))

    [<Test>]
    static member ``Exporting BigInteger`` () =
        let z = new MpzT(bigNumStr1)
        let t = z.ToBigInteger()
        Assert.That(bigint1, Is.EqualTo(t))

    [<Test>]
    static member ``Exporting byte`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (byte)z
        Assert.That(byte1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (byte)z
            Assert.That(byteArray[0], Is.EqualTo(t))

    [<Test>]
    static member ``Exporting short`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (int16)z
        Assert.That(short1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (int16)z
            Assert.That(((int16)byteArray[1] <<< 8) + (int16)byteArray[0], Is.EqualTo(t))

    [<Test>]
    static member ``Exporting ushort`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (uint16)z
        Assert.That(ushort1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (uint16)z
            Assert.That(((uint16)byteArray[1] <<< 8) + (uint16)byteArray[0], Is.EqualTo(t))

    [<Test>]
    static member ``Exporting int`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (int32)z
        Assert.That(int1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (int32)z
            Assert.That(((((int32)byteArray[3] <<< 8) + (int32)byteArray[2] <<< 8) + (int32)byteArray[1] <<< 8) + (int32)byteArray[0], Is.EqualTo(t))

    [<Test>]
    static member ``Exporting uint`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (uint32)z
        Assert.That(uint1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (uint32)z
            Assert.That(((((uint32)byteArray[3] <<< 8) + (uint32)byteArray[2] <<< 8) + (uint32)byteArray[1] <<< 8) + (uint32)byteArray[0], Is.EqualTo(t))

    [<Test>]
    static member ``Exporting long`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (int64)z
        Assert.That(long1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (int64)z
            Assert.That(BitConverter.ToInt64(byteArray), Is.EqualTo(t))

    [<Test>]
    static member ``Exporting ulong`` () =
        let mutable z = new MpzT(bigNumStr1)
        let mutable t = (uint64)z
        Assert.That(ulong1, Is.EqualTo(t))
        for i in 0..1000 do
            random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            t <- (uint64)z
            Assert.That(BitConverter.ToUInt64(byteArray), Is.EqualTo(t))

    [<Test>]
    static member ``Importing bigint bytes, big endian`` () =
        let a = bigint.Parse(bigNumStr1)
        let mutable byteArray = a.ToByteArray()
        let mutable z = new MpzT(byteArray |> Array.rev, 1)
        let mutable zStr = Mpir.MpzGetString(10u, z)
        let mutable arr = z.ToByteArray(1) |> Array.rev
        Assert.That(bigNumStr1, Is.EqualTo(zStr))
        Assert.That(Enumerable.SequenceEqual(byteArray, arr))
        for i in 0..10000 do
            while byteArray[byteArray.Length - 1] = (byte)0 || byteArray[byteArray.Length - 1] = (byte)255 do
                random.NextBytes byteArray
            z <- new MpzT(byteArray |> Array.rev, 1)
            zStr <- Mpir.MpzGetString(10u, z)
            arr <- z.ToByteArray(1) |> Array.rev
            Assert.That((new bigint(byteArray)).ToString(), Is.EqualTo(zStr))
            Assert.That(Enumerable.SequenceEqual(byteArray, arr))

    [<Test>]
    static member ``Importing bigint bytes, little endian`` () =
        let a = bigint.Parse(bigNumStr1)
        let mutable byteArray = a.ToByteArray()
        let mutable z = new MpzT(byteArray, -1)
        let mutable zStr = Mpir.MpzGetString(10u, z)
        let mutable arr = z.ToByteArray(-1)
        Assert.That(bigNumStr1, Is.EqualTo(zStr))
        Assert.That(Enumerable.SequenceEqual(byteArray, arr))
        for i in 0..10000 do
            while byteArray[0] = (byte)0 || byteArray[0] = (byte)255 do
                random.NextBytes byteArray
            z <- new MpzT(byteArray, -1)
            zStr <- Mpir.MpzGetString(10u, z)
            arr <- z.ToByteArray(-1)
            Assert.That((new bigint(byteArray)).ToString(), Is.EqualTo(zStr))
            Assert.That(Enumerable.SequenceEqual(byteArray, arr))

    [<Test>]
    static member ``Exporting mpz, big endian`` () =
        // Make a bigint from the exported byte array and compare to that bigint's ToString()
        // rather than comparing to a bigint.Parse(bigNumStr1).ToByteArray(), as bigint.ToByteArray 
        // may or may not put a leading 0x00 first to indicate positive sign depending on MSB.
        let z = new MpzT()
        Mpir.MpzSetStr(z, bigNumStr1, 10u) |> ignore

        let bytes = z.ToByteArray(1)
        let exportStr = (bigint( [|0uy|] ++ bytes |> Array.rev )).ToString()

        Assert.That(bigNumStr1, Is.EqualTo(exportStr))

    [<Test>]
    static member ``Exporting mpz, little endian`` () =
        let z = new MpzT()
        Mpir.MpzSetStr(z, bigNumStr1, 10u) |> ignore

        let bytes = z.ToByteArray(-1)
        let exportStr = (bigint(bytes)).ToString()

        Assert.That(bigNumStr1, Is.EqualTo(exportStr))

    [<TestCase(18446744073709551615UL)>]
    [<TestCase(9223372036854775807UL)>] 
    [<TestCase(4887567363547568832UL)>] 
    [<TestCase(0UL)>] 
    static member ``Importing uint64, big endian`` (n : uint64) = 
        let bytes = BitConverter.GetBytes(n)
        let bigEndianBytes =
            Array.append (Array.zeroCreate 1) (if BitConverter.IsLittleEndian then Array.rev bytes
            else bytes)
        let z = new MpzT(bigEndianBytes, 1)

        let zStr = Mpir.MpzGetString(10u, z)
        Assert.That(n.ToString(), Is.EqualTo(zStr))

    [<TestCase(18446744073709551615UL)>]
    [<TestCase(9223372036854775807UL)>] 
    [<TestCase(4887567363547568832UL)>] 
    [<TestCase(0UL)>] 
    static member ``Importing uint64, little endian`` (n : uint64) = 
        let bytes = BitConverter.GetBytes(n)
        let littleEndianBytes =
            Array.append (if BitConverter.IsLittleEndian then bytes
            else Array.rev bytes) (Array.zeroCreate 1)
        let z = new MpzT(littleEndianBytes, -1)

        let zStr = Mpir.MpzGetString(10u, z)
        Assert.That(n.ToString(), Is.EqualTo(zStr))


type ``MpzT - casts`` () =
    [<Test>]
    static member ``MpzT to long`` () =
        let tstVal : int64 = 0x7F00ABCDEA007851L
        let a = new MpzT(tstVal)
        let b = (int64 a)
        Assert.That(tstVal, Is.EqualTo(b))

    [<Test>]
    static member ``MpzT to ulong`` () =
        let tstVal : uint64 = 0xFF00ABCDEA007851UL
        let a = new MpzT(tstVal)
        let b = (uint64 a)
        Assert.That(tstVal, Is.EqualTo(b))


type ``MpzT - operations`` () =
    [<TestCase("67907490790576908375907590346925623895", "67907490790576908375907590346925623895")>]
    [<TestCase("-99943967907490790576908375907590346925623895", "99943967907490790576908375907590346925623895")>]
    [<TestCase("-1", "1")>]
    static member ``Mpir.Abs``(a: string, b: string) =
        let za = new MpzT(a)
        let zb = new MpzT(b)
        let result = za.Abs()
        Assert.That(result, Is.EqualTo(zb))

    [<TestCase("43967907490790576908375907590346925623895", "67907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-99943967907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-9994396790895")>]
    static member ``Mpir.Max``(a: string, b: string) =
        let za = new MpzT(a)
        let zb = new MpzT(b)
        let max = Mpir.Max(za, zb)
        Assert.That(za, Is.EqualTo(max))

    [<TestCase("43967907490790576908375907590346925623895", "67907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-99943967907490790576908375907590346925623895")>]
    [<TestCase("43967907490790576908375907590346925623895", "-9994396790895")>]
    static member ``Mpir.Min``(a: string, b: string) =
        let za = new MpzT(a)
        let zb = new MpzT(b)
        let min = Mpir.Min(za, zb)
        Assert.That(zb, Is.EqualTo(min))

    [<Test>]
    static member ``PowerMod with negative exponent``() =
        let za = 3Z
        let zb = 7Z
        let actual = za.PowerMod(-1, zb)
        let expected = 5Z
        Assert.That(expected, Is.EqualTo(actual))

    [<Test>]
    static member ``Very big BitLength``() =
        let za = 3Z
        let actual = za.Power(2147483647).GetFullBitLength()
        let expected = 3403681052Z
        Assert.That(expected, Is.EqualTo(actual))
[<EntryPoint>]
let main _ =
    0
