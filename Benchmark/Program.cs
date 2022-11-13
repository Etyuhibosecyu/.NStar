global using System;
global using System.IO;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using G = System.Collections.Generic;
using Corlib.NStar;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);
Console.WriteLine("Hello World!");
var list = OptimizedLinq.Fill(x => random.Next(0, 65536), 10000000);
Stopwatch sw = Stopwatch.StartNew();
var a = new string('a', 65536).SplitIntoEqual(1000);
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
sw.Restart();
var b = list.OfType<char>();
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
