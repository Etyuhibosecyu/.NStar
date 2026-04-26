global using BenchmarkDotNet.Attributes;
global using NStar.Core;
global using System;
global using System.Diagnostics;
global using G = System.Collections.Generic;
using NStar.BigCollections.LowMemory;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);

var arr = GC.AllocateUninitializedArray<(long, long)>(2147_483_591); // Максимальное число, с которым не вылетает
var sw = Stopwatch.StartNew();
G.List<(long, long)> gl = new(arr);
sw.Stop();
Console.WriteLine(sw.Elapsed);
sw.Restart();
List<(long, long)> l = new(arr);
sw.Stop();
Console.WriteLine(sw.Elapsed);
Console.WriteLine(System.Linq.Enumerable.SequenceEqual(l, gl));
while (true) ;
