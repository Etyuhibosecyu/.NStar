global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Running;
global using Mpir.NET;
global using NStar.Core;
global using NStar.Linq;
global using NStar.MathLib;
global using System;
global using System.Diagnostics;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
global using static System.Math;
global using String = NStar.Core.String;
using NStar.BigCollections;
using NStar.BigCollections.Tests;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);

var sw = Stopwatch.StartNew();
BigList<byte> bigList = new(GC.AllocateUninitializedArray<byte>(1000000000), 5, 10);
for (var i = 1; i < 32; i++)
{
	Console.WriteLine(i);
	bigList.AddRange(GC.AllocateUninitializedArray<byte>(1000000000));
}
sw.Stop();
Console.WriteLine(sw.Elapsed);
;
