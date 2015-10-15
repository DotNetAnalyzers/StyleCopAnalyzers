// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

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
                if (!string.IsNullOrEmpty(token.ValueText))
                {
                    var newName = token.ValueText.TrimStart('_');

                    // only offer a codefix if the name does not consist of only underscores.
                    if (!string.IsNullOrEmpty(newName))
                    {
                        newName = char.ToLower(newName[0]) + newName.Substring(1);
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                string.Format(NamingResources.RenameToCodeFix, newName),
                                cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                                nameof(RenameToLowerCaseCodeFixProvider)),
                            diagnostic);
                    }
                }
            }
        }
    }
}
