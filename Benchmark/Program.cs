global using System;
global using System.IO;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
using Corlib.NStar;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);
var list = OptimizedLinq.Fill(x => random.Next(0, 65536), 100000000);

//Stopwatch sw = Stopwatch.StartNew();
//var a = E.ToDictionary(E.Where(E.GroupBy(E.Zip(E.Skip(list, 1), E.Skip(list, 2), (x, y) => ((ulong)(uint)x << 32) + (uint)y), x => x), x => E.Count(x) >= 2), x => x.Key, col => E.ToList(E.OrderBy(col, x => (uint)x)));
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//sw.Restart();
//var b = list.AsSpan(1).Combine(list.AsSpan(2), (x, y) => ((ulong)(uint)x << 32) + (uint)y).Group(new EComparer<ulong>((x, y) => x == y, x => (int)(x >> 32) ^ (int)x)).FilterInPlace(x => x.Length >= 2).ToDictionary(x => x.Key, col => col.Sort());
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//Console.WriteLine(OptimizedLinq.Equals(a, b, (x, y) => x.Key == y.Key && OptimizedLinq.Equals(x.Value, y.Value, (x, y) => x == y)));

//Stopwatch sw = Stopwatch.StartNew();
//var a = new G.HashSet<int>(list);
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//sw.Restart();
//var b = new ParallelHashSet<int>(list);
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//Console.WriteLine(b.SetEquals(a));

Stopwatch sw = Stopwatch.StartNew();
var a = E.Average(E.Where(E.Zip(E.Skip(list, 1), E.Skip(list, 2), (x, y) => (Item1: x, Item2: y)), x => Math.Abs(x.Item1 - x.Item2) < 4096), x => Math.Abs(x.Item1 - x.Item2));
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
sw.Restart();
var b = list.AsSpan(1).Combine(list.AsSpan(2), (x, y) => (Item1: x, Item2: y)).FilterInPlace(x => Math.Abs(x.Item1 - x.Item2) < 4096).Mean(x => Math.Abs(x.Item1 - x.Item2));
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
Console.WriteLine(a == b);

//var s = new string('a', 1000000000);
//Stopwatch sw = Stopwatch.StartNew();
//var a = E.Count(s);
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//sw.Restart();
//var b = s.Count();
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);

//Stopwatch sw = Stopwatch.StartNew();
//var a = list.Sort(x => (double)1 / x);
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//sw.Restart();
//var b = list.Sort(x => (double)1 / x, false);
//sw.Stop();
//Console.WriteLine(sw.ElapsedMilliseconds);
//Console.WriteLine(a.Equals(b));
