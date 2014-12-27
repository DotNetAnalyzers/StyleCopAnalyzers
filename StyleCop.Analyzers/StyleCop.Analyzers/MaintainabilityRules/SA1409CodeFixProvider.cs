namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Implements a code fix for <see cref="SA1408ConditionalExpressionsMustDeclarePrecedence"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove unnecessary code.
    /// above.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1409CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1409CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1409RemoveUnnecessaryCode.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1409RemoveUnnecessaryCode.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create("Remove unnecessary code", t => GetTransformedDocumentAsync(context.Document, diagnostic, t)), diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode token = root.FindNode(diagnostic.Location.SourceSpan);
            if (token == null)
            {
                return document;
            }

            if (token.IsKind(SyntaxKind.UnsafeStatement)
                || token.IsKind(SyntaxKind.CheckedStatement)
                || token.IsKind(SyntaxKind.UncheckedStatement)
                || token.IsKind(SyntaxKind.ConstructorDeclaration)
                || token.IsKind(SyntaxKind.ElseClause))
            {
                var newSyntaxRoot = root.RemoveNode(token, SyntaxRemoveOptions.KeepExteriorTrivia);

                return document.WithSyntaxRoot(newSyntaxRoot);
            }
            else if (token.IsKind(SyntaxKind.TryStatement))
            {
                TryStatementSyntax tryStatement = token as TryStatementSyntax;

                // Empty try block
                if (tryStatement.Block.Statements.Count == 0)
                {
                    if (tryStatement.Finally == null || tryStatement.Finally.Block.Statements.Count == 0)
                    {
                        // Remove complete try statement
                        var newSyntaxRoot = root.RemoveNode(token, SyntaxRemoveOptions.KeepExteriorTrivia);
                        return document.WithSyntaxRoot(newSyntaxRoot);
                    }
                    else
                    {
                        // Replace try statement with content of finally block
                        var newSyntaxRoot = root.ReplaceNode(token, tryStatement.Finally.Block.Statements).WithAdditionalAnnotations(Formatter.Annotation);
                        return document.WithSyntaxRoot(newSyntaxRoot);
                    }
                }
            }

            return document;
        }
    }
}