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
    /// Implements a code fix for <see cref="SX1309FieldNamesMustBeginWithUnderscore"/> and
    /// <see cref="SX1309SStaticFieldNamesMustBeginWithUnderscore"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add an underscore to the beginning of the field name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SX1309CodeFixProvider))]
    [Shared]
    internal class SX1309CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SX1309FieldNamesMustBeginWithUnderscore.DiagnosticId,
                SX1309SStaticFieldNamesMustBeginWithUnderscore.DiagnosticId);

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
                    string newName = '_' + token.ValueText;
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(SX1309CodeFixProvider)),
                        diagnostic);
                }
            }
        }
    }
}
