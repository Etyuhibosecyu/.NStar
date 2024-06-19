using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Analyzer.NStar;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AnalyzerNStarAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "AnalyzerNStar";

	// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
	// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
	private const string Category = "Usage";
	private static readonly LocalizableString BitListTitle = new LocalizableResourceString(nameof(Resources.BitListTitle), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString BitListMessageFormat = new LocalizableResourceString(nameof(Resources.BitListMessageFormat), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString BitListDescription = new LocalizableResourceString(nameof(Resources.BitListDescription), Resources.ResourceManager, typeof(Resources));
	private static readonly DiagnosticDescriptor BitListRule = new(DiagnosticId, BitListTitle, BitListMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: BitListDescription);
	private static readonly LocalizableString NListTitle = new LocalizableResourceString(nameof(Resources.NListTitle), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString NListMessageFormat = new LocalizableResourceString(nameof(Resources.NListMessageFormat), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString NListDescription = new LocalizableResourceString(nameof(Resources.NListDescription), Resources.ResourceManager, typeof(Resources));
	private static readonly DiagnosticDescriptor NListRule = new(DiagnosticId, NListTitle, NListMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: NListDescription);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(BitListRule, NListRule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
		// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
		context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement);
	}

	private void AnalyzeNode(SyntaxNodeAnalysisContext context)
	{
		var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
		var tree = localDeclaration.SyntaxTree;
		var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
		var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: [tree], references: [mscorlib]);
		var solution = GetPrivatePropertyValue<Workspace>(context.Options, "Workspace").CurrentSolution;
		var model = compilation.GetSemanticModel(tree);
		var type = localDeclaration.Declaration.Type;
		if (type.GetFirstToken().ValueText == "List" && type.ChildNodes().FirstOrDefault() is TypeArgumentListSyntax typeArgs && typeArgs.ChildNodes().FirstOrDefault() is TypeSyntax innerType && model.GetSymbolInfo(innerType).Symbol is INamedTypeSymbol innerTypeSymbol)
		{
			if (innerTypeSymbol.SpecialType == SpecialType.System_Boolean)
				context.ReportDiagnostic(Diagnostic.Create(BitListRule, type.GetLocation(), innerTypeSymbol.ToString()));
			else if (innerTypeSymbol.IsUnmanagedType && CreateVar(localDeclaration.FindToken(type.Span.End + 1), out var identifier).IsKind(SyntaxKind.IdentifierToken)/* && SymbolFinder.FindReferencesAsync(model.GetSymbolInfo(identifier.Parent).Symbol, solution).Result.SelectMany(x => x.Locations).Any(x => x is ReferenceLocation)*/)
				context.ReportDiagnostic(Diagnostic.Create(NListRule, type.GetLocation(), innerTypeSymbol.ToString()));
		}
		else if (type.GetFirstToken().ValueText == "NList" && type.ChildNodes().FirstOrDefault() is TypeArgumentListSyntax typeArgs2 && typeArgs2.ChildNodes().FirstOrDefault() is TypeSyntax innerType2 && model.GetSymbolInfo(innerType2).Symbol is INamedTypeSymbol innerTypeSymbol2 && innerTypeSymbol2.SpecialType == SpecialType.System_Boolean)
			context.ReportDiagnostic(Diagnostic.Create(BitListRule, type.GetLocation(), innerTypeSymbol2.ToString()));
	}

	private static T GetPrivatePropertyValue<T>(object obj, string propName)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));
		var pi = obj.GetType().GetRuntimeProperty(propName) ?? throw new ArgumentOutOfRangeException(nameof(propName), $"Property {propName} was not found in Type {obj.GetType().FullName}");
		return (T)pi.GetValue(obj, null);
	}

	public static T CreateVar<T>(T value, out T @out) => @out = value;
}
