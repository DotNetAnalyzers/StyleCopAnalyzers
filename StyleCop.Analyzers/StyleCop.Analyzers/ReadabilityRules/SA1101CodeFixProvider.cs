namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SpacingRules;
    using StyleCop.Analyzers.Helpers;

    [ExportCodeFixProvider(nameof(SA1101PrefixLocalCallsWithThis), LanguageNames.CSharp)]
    [Shared]
    public class SA1101CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1101PrefixLocalCallsWithThis.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return new RobustFixAllProvider("Prefix reference with 'this.'", Prefix);
        }

        private SyntaxNode Prefix(SyntaxNode originalNode, SyntaxNode rewrittenNode)
        {
            var qualifiedExpression =
                      SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), rewrittenNode.WithoutTrivia().WithoutFormatting() as SimpleNameSyntax)
                      .WithTriviaFrom(rewrittenNode)
                      .WithoutFormatting();

            return qualifiedExpression;
        }


        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1101PrefixLocalCallsWithThis.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as SimpleNameSyntax;
                if (node == null)
                    return;

                var qualifiedExpression =
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), node.WithoutTrivia().WithoutFormatting())
                    .WithTriviaFrom(node)
                    .WithoutFormatting();

                var newSyntaxRoot = root.ReplaceNode(node, qualifiedExpression);

                context.RegisterFix(
                    CodeAction.Create("Prefix reference with 'this.'", context.Document.WithSyntaxRoot(newSyntaxRoot)), diagnostic);
            }
        }
    }
}
