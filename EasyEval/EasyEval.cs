using Corlib.NStar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using G = System.Collections.Generic;

namespace EasyEvalLib;

public static class EasyEval
{
	public static void CompileAndExecute(string sourceCode, G.IEnumerable<string> extraAssemblies, string[] args, TextWriter? errors = null) => Execute(Compile(sourceCode, extraAssemblies, errors), args);

	public static Assembly? CompileAndGetAssembly(string sourceCode, G.IEnumerable<string> extraAssemblies, TextWriter? errors = null) => GetAssembly(Compile(sourceCode, extraAssemblies, errors));

	public static dynamic? Eval(string sourceCode, G.IEnumerable<string>? extraAssemblies = null, G.IEnumerable<string>? extraNamespaces = null, params dynamic?[] args)
	{
		var assembly = CompileAndGetAssembly(@"using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
" + string.Join(Environment.NewLine, extraNamespaces?.Convert(x => "using " + x + ";") ?? []) + @"
namespace MyProject;
public static class Program
{
public static dynamic? F(params dynamic?[] args)
{
" + sourceCode + @"
return null;
}

public static void Main()
{
}
}
", new ListHashSet<string>("System.Linq.Expressions", "Microsoft.CSharp", "netstandard", "System.Runtime").UnionWith(extraAssemblies ?? []));
		return assembly?.GetType("MyProject.Program")?.GetMethod("F")?.Invoke(null, [args]) ?? null;
	}

	public static byte[]? Compile(string sourceCode, G.IEnumerable<string> extraAssemblies, TextWriter? errors = null)
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

	private static CSharpCompilation GenerateCode(string sourceCode, G.IEnumerable<string> extraAssemblies)
	{
		var codeString = SourceText.From(sourceCode);
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);
		var references = new ListHashSet<string>("System.Private.CoreLib", "mscorlib", "System", "System.Core").UnionWith(extraAssemblies).Convert(x => MetadataReference.CreateFromFile(Assembly.Load(x.Replace(".dll", "")).Location));
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
		{
			return;
		}
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
		{
			return null;
		}
		using var asm = new MemoryStream(compiledAssembly);
		var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
		return assemblyLoadContext.LoadFromStream(asm);
	}
}

internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
{
	public SimpleUnloadableAssemblyLoadContext() : base() { }

	protected override Assembly? Load(AssemblyName assemblyName) => null;
}
