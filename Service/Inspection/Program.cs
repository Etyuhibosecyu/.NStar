global using NStar.Core;
global using NStar.Linq;
global using System;
global using E = System.Linq.Enumerable;
using Mono.Cecil;
using NStar.BigCollections;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);
var arr = GC.AllocateUninitializedArray<int>(1000000000);
var sw = Stopwatch.StartNew();
ListHashSet<int> hs = new(arr);
sw.Stop();
Console.WriteLine(sw.Elapsed);
sw.Restart();
for (var i = 0; i < 1000000000; i++)
	_ = hs.IndexOf(random.Next());
sw.Stop();
Console.WriteLine(sw.Elapsed);
sw.Restart();
_ = hs.ToList();
sw.Stop();
Console.WriteLine(sw.Elapsed);
