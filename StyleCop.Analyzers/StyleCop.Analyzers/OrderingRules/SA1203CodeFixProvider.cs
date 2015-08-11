namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
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
                if (typeDeclarationNode == null)
                {
                    break;
                }

                var fieldReplaced = false;

                var allFields = GetOrderHelperForFields(typeDeclarationNode);
                var fieldToMove = GetFieldToMove(allFields);
                if (fieldToMove != null)
                {
                    for (var i = 0; i < allFields.Count; i++)
                    {
                        if (allFields[i].Priority < fieldToMove.Priority)
                        {
                            syntaxRoot = MoveField(syntaxRoot, fieldToMove.Member, allFields[i].Member);
                            fieldReplaced = true;
                            break;
                        }
                    }
                }

                if (!fieldReplaced)
                {
                    break;
                }
            }

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static List<MemberOrderHelper> GetOrderHelperForFields(TypeDeclarationSyntax typeDeclarationNode)
        {
            var allFields = new List<MemberOrderHelper>();
            foreach (var member in typeDeclarationNode.Members)
            {
                if (!member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    continue;
                }

                allFields.Add(new MemberOrderHelper(member));
            }

            return allFields;
        }

        private static MemberOrderHelper GetFieldToMove(List<MemberOrderHelper> allFields)
        {
            for (var i = 1; i < allFields.Count; i++)
            {
                if (allFields[i].ShouldBeBefore(allFields[i - 1]))
                {
                    return allFields[i];
                }
            }

            return null;
        }

        private static SyntaxNode MoveField(SyntaxNode root, MemberDeclarationSyntax field, MemberDeclarationSyntax firstNonConst)
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
