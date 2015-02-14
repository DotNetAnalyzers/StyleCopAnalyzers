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

    /// <summary>
    /// This class provides a code fix for <see cref="SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the <c>base.</c> prefix to <c>this.</c>.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1100CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1100CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return FixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
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
                    CodeAction.Create("Replace 'base.' with 'this.'", context.Document.WithSyntaxRoot(newSyntaxRoot)), diagnostic);
            }
        }
    }
}