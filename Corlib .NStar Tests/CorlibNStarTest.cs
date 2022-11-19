global using Mpir.NET;
global using System;
global using System.Collections;
global using System.Runtime.InteropServices;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static Corlib.NStar.Tests.Global;
global using static System.Math;

namespace Corlib.NStar.Tests;

internal static class Global
{
	internal static readonly Random random = new(1234567890);
	internal static readonly G.IEnumerable<string> defaultCollection = new List<string>("AAA", "BBB", "AAA", "BBB", "CCC", "BBB", "CCC", "DDD", "CCC");
	internal static readonly string defaultString = "XXX";
}

[TestClass]
public class CorlibNStarTest
{
	//[TestMethod]
	//public void ClearTest()
	//{
	//	BigBitList bitList = new(OptimizedLinq.Fill(x => (byte)random.Next(), 2500000));
	//	BigBitList bitList2 = new(bitList);
	//	int x = random.Next(20000000), y = random.Next(20000000);
	//	(int index, int endIndex) = (x < y) ? (x, y) : (y, x);
	//	int count = endIndex - index;
	//	bitList.Clear(index, count);
	//	Assert.IsTrue(E.SequenceEqual(bitList.GetRange(0, index).ToUIntBigList(), bitList2.GetRange(0, index).ToUIntBigList()));
	//	Assert.IsTrue(E.SequenceEqual(bitList.GetRange(index, count).ToUIntBigList(), new BigBitList(count, false).ToUIntBigList()));
	//	Assert.IsTrue(E.SequenceEqual(bitList.GetRange(endIndex, 20000000 - endIndex).ToUIntBigList(), bitList2.GetRange(endIndex, 20000000 - endIndex).ToUIntBigList()));
	//}
}