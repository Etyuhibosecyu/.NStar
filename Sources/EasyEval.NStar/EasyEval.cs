using Corlib.NStar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using G = System.Collections.Generic;
using String = Corlib.NStar.String;

namespace EasyEvalLib.NStar;

public static class EasyEval
{
	public static void CompileAndExecute(String sourceCode, G.IEnumerable<String> extraAssemblies, string[] args, TextWriter? errors = null) => Execute(Compile(sourceCode, extraAssemblies, errors), args);

	public static Assembly? CompileAndGetAssembly(String sourceCode, G.IEnumerable<String> extraAssemblies, TextWriter? errors = null) => GetAssembly(Compile(sourceCode, extraAssemblies, errors));

	public static dynamic? Eval(String sourceCode, G.IEnumerable<String>? extraAssemblies = null, G.IEnumerable<String>? extraUsings = null, params dynamic?[] args)
	{
		var sb = new StringBuilder();
		var errors = new StringWriter(sb);
		var assembly = CompileAndGetAssembly(((String)@"using BufferLib.NStar;
using Corlib.NStar;
using Dictionaries.NStar;
using ExtraReplacing.NStar;
using LINQ.NStar;
using MathLib.NStar;
using Mpir.NET;
using RemoveDoubles.NStar;
using SumCollections.NStar;
using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using G = System.Collections.Generic;
using static Corlib.NStar.Extents;
using static System.Math;
using String = Corlib.NStar.String;
").AddRange(String.Join(Environment.NewLine, extraUsings?.ToArray(x => ((String)"using ").AddRange(x).Add(';')) ?? [])).AddRange(@"
namespace MyProject;
public static class Program
{
public static dynamic? F(params dynamic?[] args)
{
").AddRange(sourceCode).AddRange(@"
return null;
}

public static void Main()
{
}
}
"), new ListHashSet<String>().UnionWith(extraAssemblies ?? []), errors);
		if (sb.ToString() != "Compilation done without any error.")
			throw new EvaluationFailedException();
		return assembly?.GetType("MyProject.Program")?.GetMethod("F")?.Invoke(null, [args]) ?? null;
	}

	public static byte[]? Compile(String sourceCode, G.IEnumerable<String> extraAssemblies, TextWriter? errors = null)
	{
		using var peStream = new MemoryStream();
		var result = GenerateCode(sourceCode, extraAssemblies).Emit(peStream);
		if (!result.Success)
		{
			errors?.WriteLine("Compilation done with error.");
			var failures = result.Diagnostics.Filter(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
			foreach (var diagnostic in failures)
			{
				errors?.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
			}
			return null;
		}
		errors?.WriteLine("Compilation done without any error.");
		peStream.Seek(0, SeekOrigin.Begin);
		return peStream.ToArray();
	}

	private static CSharpCompilation GenerateCode(String sourceCode, G.IEnumerable<String> extraAssemblies)
	{
		var codeString = SourceText.From(sourceCode.ToString());
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);
		var references = new ListHashSet<String>("BufferLib.NStar", "Corlib.NStar", "Dictionaries.NStar",
			"ExtraReplacing.NStar", "LINQ.NStar", "MathLib.NStar",
			"Microsoft.CSharp", "mscorlib", "Mpir.NET", "netstandard", "ParallelHS.NStar", "RemoveDoubles.NStar",
			"SumCollections.NStar", "System", "System.Console", "System.Core", "System.Linq.Expressions",
			"System.Private.CoreLib", "System.Runtime")
			.UnionWith(extraAssemblies).ToList(x =>
			MetadataReference.CreateFromFile(Assembly.Load(x.Replace(".dll", []).ToString()).Location));
		return CSharpCompilation.Create("MyProject.dll",
			[parsedSyntaxTree],
			references: references,
			options: new(OutputKind.ConsoleApplication,
				optimizationLevel: OptimizationLevel.Release,
				assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
	}

	public static void Execute(byte[]? compiledAssembly, string[] args)
	{
		if (compiledAssembly == null)
			return;
		LoadAndExecute(compiledAssembly, args);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void LoadAndExecute(byte[]? compiledAssembly, string[] args)
	{
		var assembly = GetAssembly(compiledAssembly);
		var entry = assembly?.EntryPoint;
		_ = entry != null && entry.GetParameters().Length > 0 ? entry.Invoke(null, [args]) : entry?.Invoke(null, null);
	}

	public static Assembly? GetAssembly(byte[]? compiledAssembly)
	{
		if (compiledAssembly == null)
			return null;
		using var asm = new MemoryStream(compiledAssembly);
		var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
		return assemblyLoadContext.LoadFromStream(asm);
	}
}

public class EvaluationFailedException(string? message, Exception? innerException) : Exception(message, innerException)
{
	public EvaluationFailedException() : this("Unable to evaluate this expression due to compile errors.") { }
	public EvaluationFailedException(string? message) : this(message, null) { }
}

internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
{
	public SimpleUnloadableAssemblyLoadContext() : base() { }

	protected override Assembly? Load(AssemblyName assemblyName) => null;
}
