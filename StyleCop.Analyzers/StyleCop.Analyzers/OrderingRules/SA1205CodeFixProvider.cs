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
    /// Implements code fixes for <see cref="SA1205PartialElementsMustDeclareAccess"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1205CodeFixProvider))]
    [Shared]
    public class SA1205CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1205PartialElementsMustDeclareAccess.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(OrderingResources.SA1205CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var typeDeclarationNode = syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as TypeDeclarationSyntax;
            if (typeDeclarationNode == null)
            {
                return document;
            }

            var symbol = semanticModel.GetDeclaredSymbol(typeDeclarationNode);
            var accessModifierKind = (symbol.DeclaredAccessibility == Accessibility.Public) ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword;

            var accessModifier = SyntaxFactory.Token(accessModifierKind);
            var replacementModifiers = typeDeclarationNode.Modifiers.Insert(0, accessModifier);
            var replacementNode = ReplaceModifiers(typeDeclarationNode, replacementModifiers);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(typeDeclarationNode, replacementNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        // This code was copied from the Roslyn codebase (and slightly modified). It can be removed if TypeDeclarationSyntaxExtensions.WithModifiers is made public (roslyn issue #2186)
        private static TypeDeclarationSyntax ReplaceModifiers(TypeDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            switch (node.Kind())
            {
            case SyntaxKind.ClassDeclaration:
                return ((ClassDeclarationSyntax)node).WithModifiers(modifiers);
            case SyntaxKind.InterfaceDeclaration:
                return ((InterfaceDeclarationSyntax)node).WithModifiers(modifiers);
            case SyntaxKind.StructDeclaration:
                return ((StructDeclarationSyntax)node).WithModifiers(modifiers);
            }

            return node;
        }
    }
}
