global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Running;
global using NStar.Core;
global using NStar.Linq;
global using NStar.MathLib;
global using System;
global using E = System.Linq.Enumerable;
global using G = System.Collections.Generic;
global using static System.Math;
using Mono.Cecil;
using NStar.BigCollections;

// See https://aka.ms/new-console-template for more information
Random random = new(1234567890);
var _assembly = AssemblyDefinition.ReadAssembly(typeof(BigList<>).Assembly.Location);
var programType = E.FirstOrDefault(_assembly.MainModule.Types,
	x => x.Name == "BigList`3") ?? throw new InvalidOperationException();
var programType2 = E.FirstOrDefault(_assembly.MainModule.Types,
	x => x.Name == "BaseBigList`3") ?? throw new InvalidOperationException();
var methods = E.Concat(programType.Methods, programType2.Methods).ToHashSet();
var recursiveMethods = methods.Filter(x => GetMethodsCalled(x).Contains(x));
ListHashSet<(string, string)> hs = [];
foreach (var x in recursiveMethods)
	foreach (var y in methods)
		if (x != y && x.Name.CompareTo(y.Name) < 0 && GetMethodsCalledFull(y).Contains(y)
			&& GetMethodsCalledFull(y).Contains(x) && GetMethodsCalledFull(x).Contains(y))
			hs.Add((x.Name, y.Name));
;

ListHashSet<MethodDefinition> GetMethodsCalled(MethodDefinition caller)
{
	var instructions = caller?.Body?.Instructions?.Filter(x =>
		x.OpCode.Name is "call" or "calli" or "callvirt" or "newobj").ToHashSet();
	ListHashSet<MethodDefinition> hs = [];
	if (instructions == null || !instructions.Any())
		return hs;
	foreach (var x in instructions)
	{
		var operand = (MethodReference)x.Operand;
		var item = E.FirstOrDefault(methods, y => operand.Name == y.Name && operand.DeclaringType.Name is "BaseBigList`3" or "BigList`3");
		if (item != null)
			hs.Add(item);
	}
	return hs;
}

ListHashSet<MethodDefinition> GetMethodsCalledFull(MethodDefinition caller)
{
	var hs = GetMethodsCalled(caller);
	for (var i = 0; i < hs.Length; i++)
		hs.AddRange(GetMethodsCalled(hs[i]));
	return hs;
}
