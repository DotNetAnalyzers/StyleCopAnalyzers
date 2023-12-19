// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

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
        /// <summary>
        /// During conflict resolution for fields, this suffix is tried before falling back to 1, 2, 3, etc...
        /// </summary>
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
                var tokenText = token.ValueText.TrimStart('_');
                if (tokenText == string.Empty)
                {
                    // Skip this one, since we can't create a new identifier from this
                    continue;
                }

                var baseName = char.ToUpper(tokenText[0]) + tokenText.Substring(1);
                var newName = baseName;
                var memberSyntax = RenameHelper.GetParentDeclaration(token);

                if (BaseNamespaceDeclarationSyntaxWrapper.IsInstance(memberSyntax))
                {
                    // namespaces are not symbols. So we are just renaming the namespace
                    Task<Document> RenameNamespace(CancellationToken cancellationToken)
                    {
                        IdentifierNameSyntax identifierSyntax = (IdentifierNameSyntax)token.Parent;

                        var newIdentifierSyntax = identifierSyntax.WithIdentifier(SyntaxFactory.Identifier(newName));

                        var newRoot = root.ReplaceNode(identifierSyntax, newIdentifierSyntax);
                        return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            (Func<CancellationToken, Task<Document>>)RenameNamespace,
                            nameof(RenameToUpperCaseCodeFixProvider) + "_" + diagnostic.Id),
                        diagnostic);
                }
                else if (memberSyntax != null)
                {
                    SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                    var declaredSymbol = semanticModel.GetDeclaredSymbol(memberSyntax);
                    if (declaredSymbol == null)
                    {
                        continue;
                    }

                    bool usedSuffix = false;
                    if (declaredSymbol.Kind == SymbolKind.Field
                        && declaredSymbol.ContainingType?.TypeKind != TypeKind.Enum
                        && !await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, context.CancellationToken).ConfigureAwait(false))
                    {
                        usedSuffix = true;
                        newName += Suffix;
                    }

                    int index = 0;
                    while (!await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, context.CancellationToken).ConfigureAwait(false))
                    {
                        usedSuffix = false;
                        index++;
                        newName = baseName + index;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(RenameToUpperCaseCodeFixProvider) + "_" + diagnostic.Id + "_" + usedSuffix + "_" + index),
                        diagnostic);
                }
            }
        }
    }
}
