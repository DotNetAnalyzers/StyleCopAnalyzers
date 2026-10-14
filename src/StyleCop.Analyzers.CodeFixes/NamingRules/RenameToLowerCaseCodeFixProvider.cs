// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for diagnostics which are fixed by renaming a symbol to start with a lower case letter.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the field or variable so that it begins with a
    /// lower-case letter, or place the item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameToLowerCaseCodeFixProvider))]
    [Shared]
    internal class RenameToLowerCaseCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1306FieldNamesMustBeginWithLowerCaseLetter.DiagnosticId,
                SA1312VariableNamesMustBeginWithLowerCaseLetter.DiagnosticId,
                SA1313ParameterNamesMustBeginWithLowerCaseLetter.DiagnosticId);

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
                if (string.IsNullOrEmpty(token.ValueText))
                {
                    continue;
                }

                var originalName = token.ValueText;

                var baseName = originalName.TrimStart('_');
                if (baseName.Length == 0)
                {
                    // only offer a code fix if the name does not consist of only underscores.
                    continue;
                }

                baseName = char.ToLower(baseName[0]) + baseName.Substring(1);
                int underscoreCount = originalName.Length - baseName.Length;

                var memberSyntax = RenameHelper.GetParentDeclaration(token);

                SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                var declaredSymbol = semanticModel.GetDeclaredSymbol(memberSyntax);
                if (declaredSymbol == null)
                {
                    continue;
                }

                // preserve the underscores, but only for fields.
                var prefix = declaredSymbol.Kind == SymbolKind.Field ? originalName.Substring(0, underscoreCount) : string.Empty;
                var newName = prefix + baseName;

                int index = 0;
                while (!await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, context.CancellationToken).ConfigureAwait(false))
                {
                    index++;
                    newName = prefix + baseName + index;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        string.Format(NamingResources.RenameToCodeFix, newName),
                        cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                        nameof(RenameToLowerCaseCodeFixProvider) + "_" + underscoreCount + "_" + index),
                    diagnostic);
            }
        }
    }
}
