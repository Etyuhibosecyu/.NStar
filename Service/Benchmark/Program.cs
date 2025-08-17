global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Running;
global using NStar.Core;
global using NStar.Linq;
global using NStar.MathLib;
global using System;
global using System.Diagnostics;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
global using static System.Math;
using NStar.BigCollections;
using NStar.BigCollections.Tests;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);
new BigListTests().TestReverse();
