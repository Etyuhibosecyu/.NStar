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
#pragma warning disable CS9216 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.
BigList<byte> bigList = new(RedStarLinq.FillArray(1000, _ => (byte)random.Next(256)), 5, 5);
#pragma warning restore CS9216 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.
for (var i = 0; i < 1000000; i++)
{
	bigList.Insert(random.Next((int)bigList.Length + 1), (byte)random.Next(256));
}
;
