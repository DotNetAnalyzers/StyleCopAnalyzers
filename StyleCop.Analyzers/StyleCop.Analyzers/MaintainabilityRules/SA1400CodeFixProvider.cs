namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.SpacingRules;

    [ExportCodeFixProvider(nameof(SA1119CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1400CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1400AccessModifierMustBeDeclared.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1400AccessModifierMustBeDeclared.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                if (node == null || node.IsMissing)
                    continue;

                SyntaxNode declarationNode = FindParentDeclarationNode(node);
                if (declarationNode == null)
                    continue;

                SyntaxNode updatedDeclarationNode;
                switch (declarationNode.CSharpKind())
                {
                case SyntaxKind.ClassDeclaration:
                    updatedDeclarationNode = HandleClassDeclaration((ClassDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.InterfaceDeclaration:
                    updatedDeclarationNode = HandleInterfaceDeclaration((InterfaceDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.EnumDeclaration:
                    updatedDeclarationNode = HandleEnumDeclaration((EnumDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.StructDeclaration:
                    updatedDeclarationNode = HandleStructDeclaration((StructDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.DelegateDeclaration:
                    updatedDeclarationNode = HandleDelegateDeclaration((DelegateDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.EventDeclaration:
                    updatedDeclarationNode = HandleEventDeclaration((EventDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.EventFieldDeclaration:
                    updatedDeclarationNode = HandleEventFieldDeclaration((EventFieldDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.MethodDeclaration:
                    updatedDeclarationNode = HandleMethodDeclaration((MethodDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.PropertyDeclaration:
                    updatedDeclarationNode = HandlePropertyDeclaration((PropertyDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.FieldDeclaration:
                    updatedDeclarationNode = HandleFieldDeclaration((FieldDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.OperatorDeclaration:
                    updatedDeclarationNode = HandleOperatorDeclaration((OperatorDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.ConversionOperatorDeclaration:
                    updatedDeclarationNode = HandleConversionOperatorDeclaration((ConversionOperatorDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.IndexerDeclaration:
                    updatedDeclarationNode = HandleIndexerDeclaration((IndexerDeclarationSyntax)declarationNode);
                    break;

                case SyntaxKind.ConstructorDeclaration:
                    updatedDeclarationNode = HandleConstructorDeclaration((ConstructorDeclarationSyntax)declarationNode);
                    break;

                default:
                    throw new InvalidOperationException("Unhandled declaration kind: " + declarationNode.CSharpKind());
                }

                if (updatedDeclarationNode != null)
                {
                    var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
                    var newSyntaxRoot = syntaxRoot.ReplaceNode(declarationNode, updatedDeclarationNode);
                    var newDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);
                    context.RegisterFix(CodeAction.Create("Declare accessibility", newDocument), diagnostic);
                }
            }
        }

        /// <summary>
        /// Adds a modifier token for <paramref name="modifierKeyword"/> to the beginning of
        /// <paramref name="modifiers"/>. The trivia for the new modifier and the trivia for the token that follows it
        /// are updated to ensure that the new modifier is placed immediately before the syntax token that follows it,
        /// separated by exactly one space.
        /// </summary>
        /// <typeparam name="T">The type of syntax node which follows the modifier list.</typeparam>
        /// <param name="modifiers">The existing modifiers. This may be empty if no modifiers are present.</param>
        /// <param name="leadingTriviaNode">The syntax node which follows the modifiers. The trivia for this node is
        /// updated if (and only if) the existing <paramref name="modifiers"/> list is empty.</param>
        /// <param name="modifierKeyword">The modifier keyword to add.</param>
        /// <returns>A <see cref="SyntaxTokenList"/> representing the original modifiers (if any) with the addition of a
        /// modifier of the specified <paramref name="modifierKeyword"/> at the beginning of the list.</returns>
        private static SyntaxTokenList AddModifier<T>(SyntaxTokenList modifiers, ref T leadingTriviaNode, SyntaxKind modifierKeyword)
            where T : SyntaxNode
        {
            SyntaxToken modifier = SyntaxFactory.Token(modifierKeyword);
            if (modifiers.Count > 0)
            {
                modifier = modifier.WithLeadingTrivia(modifiers[0].LeadingTrivia);
                modifiers = modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(SyntaxFactory.Whitespace(" ")));
                modifiers = modifiers.Insert(0, modifier);
            }
            else
            {
                modifiers = SyntaxTokenList.Create(modifier.WithLeadingTrivia(leadingTriviaNode.GetLeadingTrivia()));
                leadingTriviaNode = leadingTriviaNode.WithLeadingTrivia(SyntaxFactory.Whitespace(" "));
            }

            return modifiers;
        }

        /// <summary>
        /// Adds a modifier token for <paramref name="modifierKeyword"/> to the beginning of
        /// <paramref name="modifiers"/>. The trivia for the new modifier and the trivia for the token that follows it
        /// are updated to ensure that the new modifier is placed immediately before the syntax token that follows it,
        /// separated by exactly one space.
        /// </summary>
        /// <param name="modifiers">The existing modifiers. This may be empty if no modifiers are present.</param>
        /// <param name="leadingTriviaToken">The syntax token which follows the modifiers. The trivia for this token is
        /// updated if (and only if) the existing <paramref name="modifiers"/> list is empty.</param>
        /// <param name="modifierKeyword">The modifier keyword to add.</param>
        /// <returns>A <see cref="SyntaxTokenList"/> representing the original modifiers (if any) with the addition of a
        /// modifier of the specified <paramref name="modifierKeyword"/> at the beginning of the list.</returns>
        private static SyntaxTokenList AddModifier(SyntaxTokenList modifiers, ref SyntaxToken leadingTriviaToken, SyntaxKind modifierKeyword)
        {
            SyntaxToken modifier = SyntaxFactory.Token(modifierKeyword);
            if (modifiers.Count > 0)
            {
                modifier = modifier.WithLeadingTrivia(modifiers[0].LeadingTrivia);
                modifiers = modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(SyntaxFactory.Whitespace(" ")));
                modifiers = modifiers.Insert(0, modifier);
            }
            else
            {
                modifiers = SyntaxTokenList.Create(modifier.WithLeadingTrivia(leadingTriviaToken.LeadingTrivia));
                leadingTriviaToken = leadingTriviaToken.WithLeadingTrivia(SyntaxFactory.Whitespace(" "));
            }

            return modifiers;
        }

        private SyntaxNode HandleClassDeclaration(ClassDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Keyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Keyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleEnumDeclaration(EnumDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.EnumKeyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithEnumKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleStructDeclaration(StructDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Keyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.DelegateKeyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithDelegateKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleEventDeclaration(EventDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.EventKeyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PrivateKeyword);
            return node
                .WithEventKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.EventKeyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PrivateKeyword);
            return node
                .WithEventKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleMethodDeclaration(MethodDeclarationSyntax node)
        {
            TypeSyntax type = node.ReturnType;
            if (type == null || type.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref type, SyntaxKind.PrivateKeyword);
            return node
                .WithReturnType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandlePropertyDeclaration(PropertyDeclarationSyntax node)
        {
            TypeSyntax type = node.Type;
            if (type == null || type.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref type, SyntaxKind.PrivateKeyword);
            return node
                .WithType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleFieldDeclaration(FieldDeclarationSyntax node)
        {
            VariableDeclarationSyntax declaration = node.Declaration;
            if (declaration == null || declaration.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref declaration, SyntaxKind.PrivateKeyword);
            return node
                .WithDeclaration(declaration)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            TypeSyntax type = node.ReturnType;
            if (type == null || type.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref type, SyntaxKind.PublicKeyword);
            return node
                .WithReturnType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.ImplicitOrExplicitKeyword;
            if (triviaToken.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PublicKeyword);
            return node
                .WithImplicitOrExplicitKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            TypeSyntax type = node.Type;
            if (type == null || type.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref type, SyntaxKind.PrivateKeyword);
            return node
                .WithType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private SyntaxNode HandleConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Identifier;
            if (triviaToken.IsMissing)
                return null;

            SyntaxTokenList modifiers = AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PrivateKeyword);
            return node
                .WithIdentifier(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode FindParentDeclarationNode(SyntaxNode node)
        {
            while (node != null)
            {
                switch (node.CSharpKind())
                {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                    return node;

                default:
                    node = node.Parent;
                    break;
                }
            }

            return node;
        }

        private static bool IsNestedType(BaseTypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }

        private static bool IsNestedType(DelegateDeclarationSyntax delegateDeclaration)
        {
            return delegateDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }
    }
}
