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
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1000KeywordsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add or remove a space after the keyword, according to the description
    /// above.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1122UseStringEmptyForEmptyStrings), LanguageNames.CSharp)]
    [Shared]
    public class SA1122CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1122UseStringEmptyForEmptyStrings.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
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
                if (!diagnostic.Id.Equals(SA1122UseStringEmptyForEmptyStrings.DiagnosticId))
                    continue;
                var syntaxRoot = await context.Document.GetSyntaxRootAsync().ConfigureAwait(false);
                var node = syntaxRoot?.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
                if (node != null && node.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    var identifierNameSyntax = SyntaxFactory.IdentifierName(nameof(String.Empty));
                    var stringKeyword = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
                    var newNode = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, stringKeyword, identifierNameSyntax)
                        .WithoutElasticTrivia();

                    var newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNode);

                    context.RegisterFix(CodeAction.Create($"Replace with {newNode}", context.Document.WithSyntaxRoot(newSyntaxRoot)), diagnostic);
                }
            }
        }
    }
}
