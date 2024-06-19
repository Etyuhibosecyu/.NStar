using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer.NStar;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnalyzerNStarCodeFixProvider)), Shared]
public class AnalyzerNStarCodeFixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AnalyzerNStarAnalyzer.DiagnosticId);

	public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
		var diagnostic = context.Diagnostics.First();
		var diagnosticSpan = diagnostic.Location.SourceSpan;
		var type = (GenericNameSyntax)root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeSyntax>().First();
		context.RegisterCodeFix(CodeAction.Create(CodeFixResources.CodeFixTitle, c => FixTypeAsync(context.Document, type, c), nameof(CodeFixResources.CodeFixTitle)), diagnostic);
	}

	private async Task<Document> FixTypeAsync(Document document, GenericNameSyntax type, CancellationToken cancellationToken)
	{
		var innerType = (TypeSyntax)type.ChildNodes().First().ChildNodes().First();
		var model = await document.GetSemanticModelAsync(cancellationToken);
		var typeSymbol = (INamedTypeSymbol)model.GetSymbolInfo(innerType).Symbol;
		var newName = typeSymbol.ToString();
		var originalSolution = document.Project.Solution;
		var optionSet = originalSolution.Workspace.Options;
		var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
		if (typeSymbol.SpecialType == SpecialType.System_Boolean)
		{
			var newRoot = oldRoot.ReplaceNode(type, SyntaxFactory.IdentifierName("BitList"));
			return document.WithSyntaxRoot(newRoot);
		}
		else
		{
			var newRoot = oldRoot.ReplaceToken(type.Identifier, SyntaxFactory.Identifier("NList"));
			return document.WithSyntaxRoot(newRoot);
		}
	}
}
