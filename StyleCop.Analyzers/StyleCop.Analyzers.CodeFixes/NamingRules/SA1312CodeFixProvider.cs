// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
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
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// Implements a code fix for <see cref="SA1312VariableNamesMustBeginWithLowerCaseLetter"/> which replaces a local
    /// variable named only with underscores (such as <c>_</c> or <c>__</c>) with a discard.
    /// </summary>
    /// <remarks>
    /// <para>The fix is only offered when the variable is never referenced and the discard has the same meaning as the
    /// original declaration. Names which contain other characters are handled by
    /// <see cref="RenameToLowerCaseCodeFixProvider"/> instead.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1312CodeFixProvider))]
    [Shared]
    internal class SA1312CodeFixProvider : CodeFixProvider
    {
        private const string Discard = "_";

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1312VariableNamesMustBeginWithLowerCaseLetter.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (!(root.SyntaxTree.Options is CSharpParseOptions parseOptions) || parseOptions.LanguageVersion < LanguageVersionEx.CSharp7)
            {
                // Discards were introduced in C# 7.
                return;
            }

            SemanticModel semanticModel = null;
            foreach (var diagnostic in context.Diagnostics)
            {
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!IsUnderscoreOnly(token.ValueText))
                {
                    continue;
                }

                semanticModel ??= await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
                if (!TryGetDiscardChange(semanticModel, root, token, context.CancellationToken, out TextChange change))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        NamingResources.SA1312CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(document, change, cancellationToken),
                        nameof(SA1312CodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, TextChange change, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return document.WithText(text.WithChanges(change));
        }

        private static bool TryGetDiscardChange(SemanticModel semanticModel, SyntaxNode root, SyntaxToken identifier, CancellationToken cancellationToken, out TextChange change)
        {
            change = default;
            var parent = identifier.Parent;
            if (parent is VariableDeclaratorSyntax declarator)
            {
                return TryGetLocalDeclarationChange(semanticModel, root, identifier, declarator, cancellationToken, out change);
            }

            if (!parent.IsKind(SyntaxKindEx.SingleVariableDesignation))
            {
                // For example, catch declarations and query range variables, which cannot be discards.
                return false;
            }

            if (!(semanticModel.GetDeclaredSymbol(parent, cancellationToken) is ILocalSymbol local)
                || IsReferenced(semanticModel, root, identifier, local, cancellationToken))
            {
                return false;
            }

            var declarationExpression = parent.Parent;
            if (declarationExpression.IsKind(SyntaxKindEx.DeclarationExpression)
                && declarationExpression.Parent is ArgumentSyntax argument
                && argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)
                && ((DeclarationExpressionSyntaxWrapper)declarationExpression).Type.IsVar
                && !HasConflictingDiscardName(semanticModel, declarationExpression.SpanStart, local))
            {
                // out var __ => out _
                change = new TextChange(declarationExpression.Span, Discard);
                return true;
            }

            // out int __ => out int _, var (__, x) => var (_, x), is int __ => is int _
            change = new TextChange(identifier.Span, Discard);
            return true;
        }

        private static bool TryGetLocalDeclarationChange(SemanticModel semanticModel, SyntaxNode root, SyntaxToken identifier, VariableDeclaratorSyntax declarator, CancellationToken cancellationToken, out TextChange change)
        {
            change = default;
            if (!(declarator.Parent is VariableDeclarationSyntax declaration)
                || !(declaration.Parent is LocalDeclarationStatementSyntax statement)
                || declaration.Variables.Count != 1
                || declarator.ArgumentList != null
                || declarator.Initializer?.Value == null
                || statement.Modifiers.Count != 0
                || !statement.UsingKeyword().IsKind(SyntaxKind.None)
                || !statement.AwaitKeyword().IsKind(SyntaxKind.None))
            {
                return false;
            }

            var type = declaration.Type;
            if (type.DescendantTokens().Any(token => token.IsKind(SyntaxKind.RefKeyword) || token.IsKind(SyntaxKindEx.ScopedKeyword))
                || type.GetTrailingTrivia().Any(trivia => !trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia)))
            {
                return false;
            }

            var value = declarator.Initializer.Value;
            if (value is AnonymousFunctionExpressionSyntax
                || value.IsKind(SyntaxKind.StackAllocArrayCreationExpression)
                || value.IsKind(SyntaxKindEx.ImplicitStackAllocArrayCreationExpression))
            {
                return false;
            }

            if (!(semanticModel.GetDeclaredSymbol(declarator, cancellationToken) is ILocalSymbol local)
                || IsReferenced(semanticModel, root, identifier, local, cancellationToken)
                || HasConflictingDiscardName(semanticModel, statement.SpanStart, local))
            {
                return false;
            }

            // The discard takes the natural type of the value, so only allow conversions which cannot have side
            // effects or change which value is produced.
            var valueType = semanticModel.GetTypeInfo(value, cancellationToken).Type;
            if (valueType == null || valueType.TypeKind == TypeKind.Error)
            {
                return false;
            }

            var conversion = semanticModel.ClassifyConversion(value, local.Type);
            if (!conversion.IsIdentity && !(conversion.IsImplicit && (conversion.IsReference || conversion.IsBoxing)))
            {
                return false;
            }

            // string _ = Foo(); => _ = Foo();
            change = new TextChange(TextSpan.FromBounds(type.SpanStart, identifier.Span.End), Discard);
            return true;
        }

        private static bool HasConflictingDiscardName(SemanticModel semanticModel, int position, ILocalSymbol local)
        {
            // After the change, '_' must bind to a discard rather than to another symbol which is in scope.
            foreach (var symbol in semanticModel.LookupSymbols(position, name: Discard))
            {
                if (!Equals(symbol, local))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsReferenced(SemanticModel semanticModel, SyntaxNode root, SyntaxToken identifier, ILocalSymbol local, CancellationToken cancellationToken)
        {
            SyntaxNode scope = identifier.Parent.Ancestors().FirstOrDefault(node => node is MemberDeclarationSyntax && !node.IsKind(SyntaxKind.GlobalStatement)) ?? root;
            string name = identifier.ValueText;
            foreach (var identifierName in scope.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                if (identifierName.Identifier.ValueText == name
                    && Equals(semanticModel.GetSymbolInfo(identifierName, cancellationToken).Symbol, local))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsUnderscoreOnly(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            foreach (char c in name)
            {
                if (c != '_')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
