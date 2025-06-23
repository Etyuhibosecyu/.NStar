global using Corlib.NStar;
global using LINQ.NStar;
global using Mpir.NET;
global using System;
global using System.Collections.Immutable;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
global using static Corlib.NStar.Extents;
global using static LINQ.NStar.Tests.Global;
global using static System.Math;
global using String = Corlib.NStar.String;

namespace LINQ.NStar.Tests;

public static class Global
{
	public static readonly Random random = new(1234567890);
	public static readonly object lockObj = new();
	internal static readonly string defaultString = "XXX";
	internal static readonly ImmutableArray<string> list = ["MMM", "BBB", "PPP", "DDD", "MMM", "EEE", "DDD"];
	internal static readonly ImmutableArray<string> list2 = ["MMM", "BBB", "#", "PPP", "DDD", "MMM", "EEE", "DDD"];
	internal static readonly ImmutableArray<(char, char, char)> nList = [('M', 'M', 'M'), ('B', 'B', 'B'), ('P', 'P', 'P'), ('D', 'D', 'D'), ('M', 'M', 'M'), ('E', 'E', 'E'), ('D', 'D', 'D')];
	internal static readonly ImmutableArray<char> nString = ['M', 'B', 'P', 'D', 'M', 'E', 'D'];
	internal static readonly G.IEnumerable<string> enumerable = E.Select(list, x => x);
	internal static readonly G.IEnumerable<string> enumerable2 = E.SkipWhile(list, _ => Lock(lockObj, () => random.Next(10) == -1));
	internal static readonly G.IEnumerable<(char, char, char)> nEnumerable = E.Select(nList, x => x);
	internal static readonly G.IEnumerable<(char, char, char)> nEnumerable2 = E.SkipWhile(nList, _ => Lock(lockObj, () => random.Next(10) == -1));
	internal static readonly G.IEnumerable<char> nSEnumerable = E.Select(nString, x => x);
	internal static readonly G.IEnumerable<char> nSEnumerable2 = E.SkipWhile(nString, _ => Lock(lockObj, () => random.Next(10) == -1));
}
