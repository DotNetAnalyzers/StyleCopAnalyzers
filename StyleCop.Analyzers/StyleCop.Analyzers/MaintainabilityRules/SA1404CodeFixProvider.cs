namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    /// <summary>
    /// Implements a code fix for <see cref="SA1404CodeAnalysisSuppressionMustHaveJustification"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add a justification to your suppression.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1404CodeFixProvider))]
    [Shared]
    public class SA1404CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The placeholder to insert as part of the code fix.
        /// </summary>
        public const string JustificationPlaceholder = "Code reviewed, suppressed.";

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1404CodeAnalysisSuppressionMustHaveJustification.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = null;
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!this.FixableDiagnosticIds.Contains(diagnostic.Id))
                {
                    continue;
                }

                if (root == null)
                {
                    root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                }

                var node = root.FindNode(diagnostic.Location.SourceSpan);

                var attribute = node as AttributeSyntax;
                if (attribute != null)
                {
                    // In this case there is no justification at all
                    var fixTitle = MaintainabilityResources.SA1404CodeFixAdd;
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            fixTitle,
                            token => AddJustificationToAttributeAsync(context.Document, root, attribute),
                            fixTitle), diagnostic);
                    return;
                }

                var argument = node as AttributeArgumentSyntax;
                if (argument != null)
                {
                    var fixTitle = MaintainabilityResources.SA1404CodeFixUpdate;
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            fixTitle,
                            token => UpdateValueOfArgumentAsync(context.Document, root, argument),
                            fixTitle), diagnostic);
                    return;
                }
            }
        }

        private static Task<Document> UpdateValueOfArgumentAsync(Document document, SyntaxNode root, AttributeArgumentSyntax argument)
        {
            var newArgument = argument.WithExpression(GetNewAttributeValue());
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(argument, newArgument)));
        }

        private static Task<Document> AddJustificationToAttributeAsync(Document document, SyntaxNode syntaxRoot, AttributeSyntax attribute)
        {
            var attributeName = IdentifierName(nameof(SuppressMessageAttribute.Justification));
            var newArgument = AttributeArgument(NameEquals(attributeName), null, GetNewAttributeValue());

            var newArgumentList = attribute.ArgumentList.AddArguments(newArgument);
            return Task.FromResult(document.WithSyntaxRoot(syntaxRoot.ReplaceNode(attribute.ArgumentList, newArgumentList)));
        }

        private static LiteralExpressionSyntax GetNewAttributeValue()
        {
            return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(JustificationPlaceholder));
        }
    }
}
