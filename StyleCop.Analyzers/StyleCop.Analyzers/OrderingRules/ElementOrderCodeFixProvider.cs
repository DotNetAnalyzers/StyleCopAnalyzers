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
    /// Implements code fixes for element ordering rules.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ElementOrderCodeFixProvider))]
    [Shared]
    public class ElementOrderCodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(
                SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId,
                SA1202ElementsMustBeOrderedByAccess.DiagnosticId,
                SA1203ConstantsMustAppearBeforeFields.DiagnosticId,
                SA1204StaticElementsMustAppearBeforeInstanceElements.DiagnosticId,
                SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements.DiagnosticId,
                SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(OrderingResources.ElementOrderCodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(ElementOrderCodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return document;
            }

            syntaxRoot = UpdateSyntaxRoot(memberDeclaration, syntaxRoot);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode UpdateSyntaxRoot(MemberDeclarationSyntax memberDeclaration, SyntaxNode syntaxRoot)
        {
            var parentDeclaration = memberDeclaration.Parent;

            if (parentDeclaration is TypeDeclarationSyntax)
            {
                return HandleTypeDeclaration(memberDeclaration, (TypeDeclarationSyntax)parentDeclaration, syntaxRoot);
            }

            if (parentDeclaration is NamespaceDeclarationSyntax)
            {
                return HandleNamespaceDeclaration(memberDeclaration, (NamespaceDeclarationSyntax)parentDeclaration, syntaxRoot);
            }

            if (parentDeclaration is CompilationUnitSyntax)
            {
                return HandleCompilationUnitDeclaration(memberDeclaration, (CompilationUnitSyntax)parentDeclaration, syntaxRoot);
            }

            return syntaxRoot;
        }

        private static SyntaxNode HandleTypeDeclaration(MemberDeclarationSyntax memberDeclaration, TypeDeclarationSyntax typeDeclarationNode, SyntaxNode syntaxRoot)
        {
            var memberToMove = new MemberOrderHelper(memberDeclaration);
            return MoveMember(memberToMove, typeDeclarationNode.Members, syntaxRoot);
        }

        private static SyntaxNode HandleCompilationUnitDeclaration(MemberDeclarationSyntax memberDeclaration, CompilationUnitSyntax compilationUnitDeclaration, SyntaxNode syntaxRoot)
        {
            var memberToMove = new MemberOrderHelper(memberDeclaration);
            return MoveMember(memberToMove, compilationUnitDeclaration.Members, syntaxRoot);
        }

        private static SyntaxNode HandleNamespaceDeclaration(MemberDeclarationSyntax memberDeclaration, NamespaceDeclarationSyntax namespaceDeclaration, SyntaxNode syntaxRoot)
        {
            var memberToMove = new MemberOrderHelper(memberDeclaration);
            return MoveMember(memberToMove, namespaceDeclaration.Members, syntaxRoot);
        }

        private static SyntaxNode MoveMember(MemberOrderHelper memberOrder, SyntaxList<MemberDeclarationSyntax> members, SyntaxNode syntaxRoot)
        {
            foreach (var member in members)
            {
                var orderHelper = new MemberOrderHelper(member);
                if (orderHelper.Priority < memberOrder.Priority)
                {
                    syntaxRoot = MoveMember(syntaxRoot, memberOrder.Member, member);
                    break;
                }
            }

            return syntaxRoot;
        }

        private static SyntaxNode MoveMember(SyntaxNode root, MemberDeclarationSyntax field, MemberDeclarationSyntax firstNonConst)
        {
            var trackedRoot = root.TrackNodes(field, firstNonConst);
            var fieldToMove = trackedRoot.GetCurrentNode(field);
            var firstNonConstTracked = trackedRoot.GetCurrentNode(firstNonConst);
            if (!fieldToMove.HasLeadingTrivia)
            {
                fieldToMove = fieldToMove.WithLeadingTrivia(firstNonConstTracked.GetLeadingTrivia().Where(x => x.IsKind(SyntaxKind.WhitespaceTrivia)).LastOrDefault());
            }

            root = trackedRoot.InsertNodesBefore(firstNonConstTracked, new[] { fieldToMove });
            var fieldToMoveTracked = root.GetCurrentNodes(field).Last();
            return root.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => OrderingResources.ElementOrderCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    syntaxRoot = UpdateSyntaxRoot(memberDeclaration, syntaxRoot);
                }

                return syntaxRoot;
            }
        }
    }
}
