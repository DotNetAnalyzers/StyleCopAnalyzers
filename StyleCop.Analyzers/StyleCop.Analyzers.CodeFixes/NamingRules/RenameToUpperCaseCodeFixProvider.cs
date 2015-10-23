// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for all analyzers that require a symbol to be upper case.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the symbol so that it begins with an upper-case letter,
    /// or place the item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameToUpperCaseCodeFixProvider))]
    [Shared]
    internal class RenameToUpperCaseCodeFixProvider : CodeFixProvider
    {
        private const string Suffix = "Value";

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1300ElementMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1303ConstFieldNamesMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId);

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

            foreach (var diagnostic in context.Diagnostics)
            {
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                var newName = char.ToUpper(token.ValueText[0]) + token.ValueText.Substring(1);
                var memberSyntax = this.GetParentTypeDeclaration(token);

                if (memberSyntax is NamespaceDeclarationSyntax)
                {
                    // namespaces are not symbols. So we are just renaming the namespace
                    Func<CancellationToken, Task<Document>> renameNamespace = cancellationToken =>
                    {
                        IdentifierNameSyntax identifierSyntax = (IdentifierNameSyntax)token.Parent;

                        var newIdentifierSyntac = identifierSyntax.WithIdentifier(SyntaxFactory.Identifier(newName));

                        var newRoot = root.ReplaceNode(identifierSyntax, newIdentifierSyntac);
                        return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                    };

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            renameNamespace,
                            nameof(RenameToUpperCaseCodeFixProvider) + "_" + diagnostic.Id),
                        diagnostic);
                }
                else if (memberSyntax != null)
                {
                    SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                    var typeDeclarationSymbol = semanticModel.GetDeclaredSymbol(memberSyntax);
                    if (typeDeclarationSymbol == null)
                    {
                        continue;
                    }

                    if (!this.IsValidNewMemberName(typeDeclarationSymbol, newName))
                    {
                        newName = newName + Suffix;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(RenameToUpperCaseCodeFixProvider) + "_" + diagnostic.Id),
                        diagnostic);
                }
            }
        }

        private bool IsValidNewMemberName(ISymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }
            else if (typeSymbol.Name == name)
            {
                return false;
            }

            var members = (typeSymbol as INamedTypeSymbol)?.GetMembers(name);
            if (members.HasValue && !members.Value.IsDefaultOrEmpty)
            {
                return false;
            }

            var containingType = typeSymbol.ContainingSymbol as INamedTypeSymbol;
            if (containingType != null)
            {
                // The name can't be the same as the name of the containing type
                if (containingType.Name == name)
                {
                    return false;
                }

                // The name can't be the same as the name of an other member of the same type
                members = containingType.GetMembers(name);
                if (!members.Value.IsDefaultOrEmpty)
                {
                    return false;
                }
            }

            return true;
        }

        private SyntaxNode GetParentTypeDeclaration(SyntaxToken token)
        {
            SyntaxNode parent = token.Parent;

            while (parent != null)
            {
                var declarationParent = parent as MemberDeclarationSyntax
                    ?? (SyntaxNode)(parent as VariableDeclaratorSyntax);
                if (declarationParent != null)
                {
                    return declarationParent;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }
}
