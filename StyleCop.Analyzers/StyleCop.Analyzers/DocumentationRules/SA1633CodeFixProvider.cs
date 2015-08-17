namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;


    /// <summary>
    /// Implements a code fix for <see cref="SA1633FileMustHaveHeader"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the <c>&lt;returns&gt;</c> tag from the element.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1633CodeFixProvider))]
    [Shared]
    public class SA1633CodeFixProvider : CodeFixProvider
    {
        private const string CompanyName = "FooCorp"; // Should come from settings.

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1633FileMustHaveHeader.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(DocumentationResources.SA1617CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1633CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var fileHeader = FileHeaderHelpers.ParseFileHeader(root);
            SyntaxNode newSyntaxRoot;
            if (fileHeader.IsMissing)
            {
                newSyntaxRoot = AddHeader(root, document.Name);
            }
            else
            {
                newSyntaxRoot = await ReplaceHeaderAsync(document, fileHeader, root, cancellationToken).ConfigureAwait(false);
            }

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static async Task<SyntaxNode> ReplaceHeaderAsync(
            Document document,
            FileHeader fileHeader,
            SyntaxNode root,
            CancellationToken cancellationToken)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var headerSpan = fileHeader.GetLocation(tree).SourceSpan;
            var overlappingTrivia = root.GetLeadingTrivia().Where(x => x.GetLocation().SourceSpan.OverlapsWith(headerSpan));
            var newSyntaxRoot = root.ReplaceTrivia(
                overlappingTrivia,
                (trivia, syntaxTrivia) => SyntaxFactory.ParseLeadingTrivia(string.Empty).Single());
            return root.WithLeadingTrivia(CreateNewHeader(document.Name).Add(SyntaxFactory.CarriageReturnLineFeed));
        }

        private static SyntaxNode AddHeader(SyntaxNode root, string name)
        {
            var newTrivia = CreateNewHeader(name).AddRange(root.GetLeadingTrivia());
            if (!newTrivia.Last().IsKind(SyntaxKind.EndOfLineTrivia))
            {
                newTrivia = newTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
            }
            return root.WithLeadingTrivia(newTrivia);
        }

        private static SyntaxTriviaList CreateNewHeader(string name)
        {
            return SyntaxFactory.ParseLeadingTrivia($@"// <copyright file=""{name}"" company=""{CompanyName}"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
");
        }
    }
}