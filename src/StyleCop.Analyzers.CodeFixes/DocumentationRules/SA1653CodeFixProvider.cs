// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1653CodeFixProvider))]
[Shared]
internal class SA1653CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
    {
        return FixAll.Instance;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        SemanticModel? semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (semanticModel == null)
        {
            return;
        }

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
            TypeDeclarationSyntax? declaration = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            INamedTypeSymbol? symbol = declaration == null
                ? null
                : semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);
            if (declaration == null || symbol == null)
            {
                continue;
            }

            List<SummaryInfo> summaries = await GetSummaryInfoAsync(symbol, declaration, context.CancellationToken).ConfigureAwait(false);
            if (summaries.Any(summary => summary.IsGenerated && !summary.IsSelectedDeclaration))
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    DocumentationResources.SA1653CodeFix,
                    cancellationToken => GetTransformedSolutionAsync(context.Document, declaration, cancellationToken),
                    nameof(SA1653CodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Solution> GetTransformedSolutionAsync(Document document, TypeDeclarationSyntax selectedDeclaration, CancellationToken cancellationToken)
    {
        SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel == null)
        {
            throw new System.InvalidOperationException("The document does not have a semantic model.");
        }

        INamedTypeSymbol? symbol = semanticModel.GetDeclaredSymbol(selectedDeclaration, cancellationToken);
        if (symbol == null)
        {
            throw new System.InvalidOperationException("The partial declaration does not have a declared type symbol.");
        }

        List<SummaryInfo> summaries = await GetSummaryInfoAsync(symbol, selectedDeclaration, cancellationToken).ConfigureAwait(false);
        return await ApplySummaryChangesAsync(document.Project.Solution, summaries, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<List<SummaryInfo>> GetSummaryInfoAsync(INamedTypeSymbol symbol, TypeDeclarationSyntax selectedDeclaration, CancellationToken cancellationToken)
    {
        var declarations = new List<SummaryCandidate>();
        foreach (SyntaxReference reference in symbol.DeclaringSyntaxReferences)
        {
            if (await reference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false) is not TypeDeclarationSyntax declaration)
            {
                continue;
            }

            DocumentationCommentTriviaSyntax? documentation = declaration.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                continue;
            }

            List<XmlNodeSyntax> declarationSummaries = documentation.Content.GetXmlElements(XmlCommentHelper.SummaryXmlTag).ToList();
            if (declarationSummaries.Count == 0)
            {
                continue;
            }

            bool isGenerated = await IsGeneratedCodeAsync(declaration, cancellationToken).ConfigureAwait(false);
            foreach (XmlNodeSyntax summary in declarationSummaries)
            {
                declarations.Add(new SummaryCandidate(declaration, summary, isGenerated));
            }
        }

        Dictionary<string, int> counts = declarations
            .GroupBy(summary => GetSummaryText(summary.Summary), System.StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), System.StringComparer.Ordinal);

        return declarations
            .Select(summary => new SummaryInfo(
                summary.Declaration,
                summary.Summary,
                GetSummaryText(summary.Summary),
                summary.Declaration.SyntaxTree == selectedDeclaration.SyntaxTree && summary.Declaration.Span == selectedDeclaration.Span,
                summary.IsGenerated,
                counts))
            .ToList();
    }

    private static async Task<Solution> ApplySummaryChangesAsync(Solution solution, List<SummaryInfo> summaries, CancellationToken cancellationToken)
    {
        foreach (IGrouping<SyntaxTree, SummaryInfo> treeSummaries in summaries
            .Where(summary => !summary.IsSelectedDeclaration && !summary.IsGenerated)
            .GroupBy(summary => summary.Declaration.SyntaxTree))
        {
            Document? targetDocument = solution.GetDocument(treeSummaries.Key);
            if (targetDocument == null)
            {
                continue;
            }

            SyntaxNode targetRoot = await targetDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var summariesByDeclaration = treeSummaries
                .GroupBy(summary => summary.Declaration)
                .ToDictionary(group => group.Key, group => group.ToList());

            SyntaxNode updatedRoot = targetRoot.ReplaceNodes(
                summariesByDeclaration.Keys,
                (original, rewritten) => RewriteDeclaration((TypeDeclarationSyntax)original, summariesByDeclaration[original]));
            solution = solution.WithDocumentSyntaxRoot(targetDocument.Id, updatedRoot);
        }

        return solution;
    }

    private static async Task<bool> IsGeneratedCodeAsync(TypeDeclarationSyntax declaration, CancellationToken cancellationToken)
    {
        string filePath = declaration.SyntaxTree.FilePath;
        if (filePath.EndsWith(".g.cs", System.StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".generated.cs", System.StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".designer.cs", System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        SyntaxNode root = await declaration.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
        return root.GetLeadingTrivia().Any(trivia =>
            trivia.ToString().IndexOf("<auto-generated", System.StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static string GetSummaryText(XmlNodeSyntax summary)
    {
        return summary is XmlElementSyntax element
            ? element.Content.ToString().Trim()
            : string.Empty;
    }

    private static TypeDeclarationSyntax RewriteDeclaration(TypeDeclarationSyntax declaration, List<SummaryInfo> summaries)
    {
        DocumentationCommentTriviaSyntax? documentation = declaration.GetDocumentationCommentTriviaSyntax();
        if (documentation == null)
        {
            throw new System.InvalidOperationException("The partial declaration no longer has its documentation comment.");
        }

        SyntaxList<XmlNodeSyntax> content = documentation.Content;
        foreach (SummaryInfo summary in summaries)
        {
            if (summary.Counts[summary.Text] > 1)
            {
                content = content.Remove(summary.Node);
            }
            else
            {
                content = content.Replace(summary.Node, ChangeElementName(summary.Node, XmlCommentHelper.ContentXmlTag));
            }
        }

        DocumentationCommentTriviaSyntax updatedDocumentation = documentation.WithContent(content);
        SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();
        if (content.All(node => node is XmlTextSyntax && string.IsNullOrWhiteSpace(node.ToString())))
        {
            leadingTrivia = leadingTrivia.Remove(documentation.ParentTrivia);
        }
        else
        {
            leadingTrivia = leadingTrivia.Replace(documentation.ParentTrivia, SyntaxFactory.Trivia(updatedDocumentation));
        }

        return declaration.WithLeadingTrivia(leadingTrivia);
    }

    private static XmlNodeSyntax ChangeElementName(XmlNodeSyntax summary, string newName)
    {
        if (summary is XmlElementSyntax element)
        {
            XmlNameSyntax startTagName = SyntaxFactory.XmlName(newName)
                .WithLeadingTrivia(element.StartTag.Name.GetLeadingTrivia())
                .WithTrailingTrivia(element.StartTag.Name.GetTrailingTrivia());
            XmlNameSyntax endTagName = SyntaxFactory.XmlName(newName)
                .WithLeadingTrivia(element.EndTag.Name.GetLeadingTrivia())
                .WithTrailingTrivia(element.EndTag.Name.GetTrailingTrivia());
            return element
                .WithStartTag(element.StartTag.WithName(startTagName))
                .WithEndTag(element.EndTag.WithName(endTagName));
        }

        var emptyElement = (XmlEmptyElementSyntax)summary;
        XmlNameSyntax name = SyntaxFactory.XmlName(newName)
            .WithLeadingTrivia(emptyElement.Name.GetLeadingTrivia())
            .WithTrailingTrivia(emptyElement.Name.GetTrailingTrivia());
        return emptyElement.WithName(name);
    }

    private sealed class SummaryCandidate
    {
        public SummaryCandidate(TypeDeclarationSyntax declaration, XmlNodeSyntax summary, bool isGenerated)
        {
            this.Declaration = declaration;
            this.Summary = summary;
            this.IsGenerated = isGenerated;
        }

        public TypeDeclarationSyntax Declaration { get; }

        public XmlNodeSyntax Summary { get; }

        public bool IsGenerated { get; }
    }

    private sealed class SummaryInfo
    {
        public SummaryInfo(
            TypeDeclarationSyntax declaration,
            XmlNodeSyntax node,
            string text,
            bool isSelectedDeclaration,
            bool isGenerated,
            Dictionary<string, int> counts)
        {
            this.Declaration = declaration;
            this.Node = node;
            this.Text = text;
            this.IsSelectedDeclaration = isSelectedDeclaration;
            this.IsGenerated = isGenerated;
            this.Counts = counts;
        }

        public TypeDeclarationSyntax Declaration { get; }

        public XmlNodeSyntax Node { get; }

        public string Text { get; }

        public bool IsSelectedDeclaration { get; }

        public bool IsGenerated { get; }

        public Dictionary<string, int> Counts { get; }
    }

    private sealed class FixAll : FixAllProvider
    {
        public static FixAllProvider Instance { get; } = new FixAll();

        public override Task<CodeAction?> GetFixAsync(FixAllContext fixAllContext)
        {
            CodeAction action = CodeAction.Create(
                DocumentationResources.SA1653CodeFix,
                cancellationToken => GetFixedSolutionAsync(fixAllContext, cancellationToken),
                nameof(SA1653CodeFixProvider));
            return Task.FromResult<CodeAction?>(action);
        }

        private static async Task<Solution> GetFixedSolutionAsync(FixAllContext fixAllContext, CancellationToken cancellationToken)
        {
            ImmutableDictionary<Document, ImmutableArray<Diagnostic>> diagnosticsByDocument =
                await FixAllContextHelper.GetDocumentDiagnosticsToFixAsync(fixAllContext).ConfigureAwait(false);
            var selectedDeclarations = new Dictionary<INamedTypeSymbol, TypeDeclarationSyntax>();

            foreach (KeyValuePair<Document, ImmutableArray<Diagnostic>> documentDiagnostics in diagnosticsByDocument
                .OrderBy(item => item.Key.FilePath, System.StringComparer.Ordinal))
            {
                SyntaxNode root = await documentDiagnostics.Key.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                SemanticModel? semanticModel = await documentDiagnostics.Key.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                if (semanticModel == null)
                {
                    continue;
                }

                foreach (Diagnostic diagnostic in documentDiagnostics.Value.OrderBy(item => item.Location.SourceSpan.Start))
                {
                    SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                    TypeDeclarationSyntax? declaration = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
                    if (declaration == null)
                    {
                        continue;
                    }

                    INamedTypeSymbol? symbol = semanticModel.GetDeclaredSymbol(declaration, cancellationToken);
                    if (symbol != null && !selectedDeclarations.ContainsKey(symbol))
                    {
                        selectedDeclarations.Add(symbol, declaration);
                    }
                }
            }

            var summaries = new List<SummaryInfo>();
            foreach (KeyValuePair<INamedTypeSymbol, TypeDeclarationSyntax> selected in selectedDeclarations)
            {
                List<SummaryInfo> selectedSummaries = await GetSummaryInfoAsync(selected.Key, selected.Value, cancellationToken).ConfigureAwait(false);
                if (!selectedSummaries.Any(summary => summary.IsGenerated && !summary.IsSelectedDeclaration))
                {
                    summaries.AddRange(selectedSummaries);
                }
            }

            return await ApplySummaryChangesAsync(fixAllContext.Solution, summaries, cancellationToken).ConfigureAwait(false);
        }
    }
}
