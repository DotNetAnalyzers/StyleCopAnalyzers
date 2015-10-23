// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Implements a code fix for <see cref="SA1308VariableNamesMustNotBePrefixed"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the prefix from the beginning of the field name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1308CodeFixProvider))]
    [Shared]
    internal class SA1308CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1308VariableNamesMustNotBePrefixed.DiagnosticId);

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

                // The variable name is the full suffix. In this case we cannot generate a valid variable name and thus will not offer a code fix.
                if (token.ValueText.Length <= 2)
                {
                    continue;
                }

                var numberOfCharsToRemove = 2;

                // If a variable contains multiple prefixes that would result in this diagnostic,
                // we detect that and remove all of the bad prefixes such that after
                // the fix is applied there are no more violations of this rule.
                for (int i = 2; i < token.ValueText.Length; i += 2)
                {
                    if (string.Compare("m_", 0, token.ValueText, i, 2, StringComparison.Ordinal) == 0
                        || string.Compare("s_", 0, token.ValueText, i, 2, StringComparison.Ordinal) == 0
                        || string.Compare("t_", 0, token.ValueText, i, 2, StringComparison.Ordinal) == 0)
                    {
                        numberOfCharsToRemove += 2;
                        continue;
                    }

                    break;
                }

                if (!string.IsNullOrEmpty(token.ValueText))
                {
                    var newName = token.ValueText.Substring(numberOfCharsToRemove);
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(SA1308CodeFixProvider)),
                        diagnostic);
                }
            }
        }
    }
}
