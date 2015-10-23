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
    /// Implements a code fix for <see cref="SA1309FieldNamesMustNotBeginWithUnderscore"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the underscore from the beginning of the field name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1309CodeFixProvider))]
    [Shared]
    internal class SA1309CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
          ImmutableArray.Create(SA1309FieldNamesMustNotBeginWithUnderscore.DiagnosticId);

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
                    var newName = token.ValueText.TrimStart(new[] { '_' });

                    if (string.IsNullOrEmpty(newName))
                    {
                        // The variable consisted of only underscores. In this case we cannot
                        // generate a valid variable name and thus will not offer a code fix.
                        continue;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(SA1309CodeFixProvider)),
                        diagnostic);
                }
            }
        }
    }
}
