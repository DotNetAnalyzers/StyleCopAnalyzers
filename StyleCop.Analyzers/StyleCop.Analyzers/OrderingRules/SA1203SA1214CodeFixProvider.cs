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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1203SA1214CodeFixProvider))]
    [Shared]
    public class SA1203SA1214CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1203ConstantsMustAppearBeforeFields.DiagnosticId, SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(OrderingResources.SA1203CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1203SA1214CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var fieldDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var typeDeclarationNode = fieldDeclaration.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            if (typeDeclarationNode == null)
            {
                return document;
            }

            syntaxRoot = MoveField(fieldDeclaration, typeDeclarationNode, syntaxRoot);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode MoveField(FieldDeclarationSyntax fieldDeclaration, TypeDeclarationSyntax typeDeclarationNode, SyntaxNode syntaxRoot)
        {
            var fieldToMove = new MemberOrderHelper(fieldDeclaration);
            for (var i = 0; i < typeDeclarationNode.Members.Count; i++)
            {
                var member = typeDeclarationNode.Members[i];
                if (member.Kind() != SyntaxKind.FieldDeclaration)
                {
                    continue;
                }

                var orderHelper = new MemberOrderHelper(member);
                if (orderHelper.ModifierPriority < fieldToMove.ModifierPriority)
                {
                    syntaxRoot = MoveField(syntaxRoot, fieldToMove.Member, member);
                    break;
                }
            }
            return syntaxRoot;
        }

        private static SyntaxNode MoveField(SyntaxNode root, MemberDeclarationSyntax field, MemberDeclarationSyntax firstNonConst)
        {
            var trackedRoot = root.TrackNodes(field, firstNonConst);
            var fieldToMove = trackedRoot.GetCurrentNode(field);
            var firstNonConstTracked = trackedRoot.GetCurrentNode(firstNonConst);
            if (!fieldToMove.HasLeadingTrivia)
            {
                fieldToMove = fieldToMove.WithLeadingTrivia(firstNonConstTracked.GetLeadingTrivia());
            }

            root = trackedRoot.InsertNodesBefore(firstNonConstTracked, new[] { fieldToMove });
            var fieldToMoveTracked = root.GetCurrentNodes(field).Last();
            return root.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}
