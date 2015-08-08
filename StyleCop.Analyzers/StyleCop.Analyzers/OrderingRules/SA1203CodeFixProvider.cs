namespace StyleCop.Analyzers.OrderingRules
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements code fixes for <see cref="SA1203ConstantsMustAppearBeforeFields"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1203CodeFixProvider))]
    [Shared]
    public class SA1203CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1203ConstantsMustAppearBeforeFields.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(OrderingResources.SA1203CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1203CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            while (true)
            {
                var typeDeclarationNode = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<TypeDeclarationSyntax>();

                var fieldReplaced = false;

                FieldDeclarationSyntax firstNonConst = null;
                foreach (var member in typeDeclarationNode.Members)
                {
                    if (!member.IsKind(SyntaxKind.FieldDeclaration))
                    {
                        continue;
                    }

                    var field = (FieldDeclarationSyntax)member;

                    var fieldIsConst = field.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword));
                    if (firstNonConst != null && fieldIsConst)
                    {
                        syntaxRoot = MoveField(syntaxRoot, field, firstNonConst);
                        fieldReplaced = true;
                        break;
                    }

                    if (firstNonConst == null && !fieldIsConst)
                    {
                        firstNonConst = field;
                    }
                }

                if (!fieldReplaced)
                {
                    break;
                }
            }

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode MoveField(SyntaxNode root, FieldDeclarationSyntax field, FieldDeclarationSyntax firstNonConst)
        {
            var trackedRoot = root.TrackNodes(field, firstNonConst);
            var fieldToMove = trackedRoot.GetCurrentNode(field);
            var firstNonConstTracked = trackedRoot.GetCurrentNode(firstNonConst);
            root = trackedRoot.InsertNodesBefore(firstNonConstTracked, new[] { fieldToMove });
            var fieldToMoveTracked = root.GetCurrentNodes(field).Last();
            return root.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}
