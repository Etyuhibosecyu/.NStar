global using BenchmarkDotNet.Attributes;
global using NStar.Core;
global using System;
global using System.Diagnostics;
global using G = System.Collections.Generic;
using NStar.BigCollections.LowMemory;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);

var sw = Stopwatch.StartNew();
BigList<byte> bigList = new(GC.AllocateUninitializedArray<byte>(1 << 30));
for (var i = 0; i < 20; i++)
{
	Console.WriteLine(i);
	for (var j = 0; j < 100000000; j++)
		bigList.Add(default!);
}
sw.Stop();
Console.WriteLine(sw.Elapsed);
while (true) ;

