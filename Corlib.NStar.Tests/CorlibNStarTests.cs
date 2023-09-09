global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Mpir.NET;
global using System;
global using System.Collections;
global using System.Collections.Immutable;
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
	internal static readonly ImmutableArray<string> list = ImmutableArray.Create("MMM", "BBB", "PPP", "DDD", "MMM", "EEE", "DDD");
	internal static readonly G.IEnumerable<(char, char, char)> defaultNCollection = new List<(char, char, char)>(('A', 'A', 'A'), ('B', 'B', 'B'), ('A', 'A', 'A'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('B', 'B', 'B'), ('C', 'C', 'C'), ('D', 'D', 'D'), ('C', 'C', 'C'));
	internal static readonly (char, char, char) defaultNString = ('X', 'X', 'X');
	internal static readonly ImmutableArray<(char, char, char)> nList = ImmutableArray.Create(('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D'));
	internal static readonly G.IEnumerable<bool> defaultBitCollection = new BitList() { true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true };
	internal static readonly bool defaultBit = true;
	internal static readonly ImmutableArray<bool> bitList = ImmutableArray.Create(new BitList(71, new uint[] { 4186021589, 210265814, 981043 }).SetAll(false, ..5).SetAll(true, ^7..).ToArray());
	internal static readonly byte[] testBytes = { 142, 86, 21, 34, 120, 162, 78, 221, 197, 63, 148, 46, 54, 161, 159, 197, 184, 249, 114, 247, 243, 37, 117, 251, 245, 156, 238, 94, 219, 131, 98, 40, 199, 255, 134, 69, 168, 144, 236, 108, 82, 139, 198, 224, 191, 140, 26, 56, 7, 123, 5, 139, 35, 173, 32, 104, 225, 26, 44, 219, 180, 240, 1, 66, 221, 108, 103, 55, 27, 89, 29, 40, 13, 5, 165, 104, 95, 148, 59, 104, 130, 181, 86, 95, 125, 215, 4, 25, 222, 29, 100, 141, 99, 16, 183, 97, 137, 132, 3, 82, 37, 227, 28, 173, 94, 221, 125, 29, 7, 184, 9, 77, 222, 195, 89, 254, 154, 162, 196, 5, 103, 78, 88, 253, 245, 143, 196, 253, 77, 68, 222, 218, 144, 233, 67, 159, 153, 168, 0, 159, 89, 231, 18, 38, 20, 22, 175, 160, 183, 0 };
	internal static readonly bool[] testBools = E.ToArray(E.SelectMany(testBytes, x => E.Select(E.Range(0, 8), y => (x & 1 << y) != 0)));
	internal static readonly G.IEnumerable<string> enumerable = E.Select(list, x => x);
	internal static readonly G.IEnumerable<string> enumerable2 = E.SkipWhile(list, _ => random.Next(10) == -1);
	internal static readonly G.IEnumerable<(char, char, char)> nEnumerable = E.Select(nList, x => x);
	internal static readonly G.IEnumerable<(char, char, char)> nEnumerable2 = E.SkipWhile(nList, _ => random.Next(10) == -1);
}
