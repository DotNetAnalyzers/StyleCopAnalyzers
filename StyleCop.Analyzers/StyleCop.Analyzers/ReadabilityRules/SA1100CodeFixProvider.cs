namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SpacingRules;

    [ExportCodeFixProvider(nameof(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists), LanguageNames.CSharp)]
    [Shared]
    public class SA1100CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId);

        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var node = root.FindNode(diagnostic.Location.SourceSpan) as BaseExpressionSyntax;
                if (node == null)
                {
                    return;
                }

                var thisExpressionSyntax = SyntaxFactory.ThisExpression()
                    .WithTriviaFrom(node)
                    .WithoutFormatting();

                var newSyntaxRoot = root.ReplaceNode(node, thisExpressionSyntax);

                context.RegisterFix(
                    CodeAction.Create("Replace with this", context.Document.WithSyntaxRoot(newSyntaxRoot)), diagnostic);
            }
        }
    }
}