// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1302InterfaceNamesMustBeginWithI"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add the capital letter I to the front of the interface name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1302CodeFixProvider))]
    [Shared]
    public class SA1302CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1302InterfaceNamesMustBeginWithI.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1302InterfaceNamesMustBeginWithI.DiagnosticId))
                {
                    continue;
                }

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                var newName = "I" + token.ValueText;
                context.RegisterCodeFix(CodeAction.Create(string.Format(NamingResources.RenameToCodeFix, newName), cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
            }
        }
    }
}
