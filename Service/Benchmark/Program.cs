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
using NStar.BigCollections.LowMemory;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);

var sw = Stopwatch.StartNew();
BigList<byte> bigList = new();
for (var i = 0; i < 20; i++)
{
	Console.WriteLine(i);
	for (var j = 0; j < 100000000; j++)
		bigList.Add(default!);
}
sw.Stop();
Console.WriteLine(sw.Elapsed);
while (true) ;

