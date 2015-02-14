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
    /// This class provides a code fix for <see cref="SA1101PrefixLocalCallsWithThis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert the <c>this.</c> prefix before the call to the class
    /// member.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1101CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1101CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1101PrefixLocalCallsWithThis.DiagnosticId);

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
