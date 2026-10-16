// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules;

    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1315ParametersShouldMatchInheritedNames"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1315CodeFixProvider))]
    [Shared]
    internal class SA1315CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1315ParametersShouldMatchInheritedNames.DiagnosticId);

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
            var semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null || semanticModel == null)
            {
                return;
            }

            foreach (var diagnostic in context.Diagnostics)
            {
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                var parameterSymbol = semanticModel.GetDeclaredSymbol(token.Parent, context.CancellationToken);
                if (parameterSymbol == null
                    || !diagnostic.Properties.TryGetValue(SA1315ParametersShouldMatchInheritedNames.InheritedParameterNamesProperty, out string? serializedNames))
                {
                    continue;
                }

                foreach (string newName in serializedNames.Split(','))
                {
                    if (newName.Length == 0
                        || !await RenameHelper.IsValidNewMemberNameAsync(semanticModel, parameterSymbol, newName, context.CancellationToken).ConfigureAwait(false))
                    {
                        continue;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(SA1315CodeFixProvider) + newName),
                        diagnostic);
                }
            }
            }
}
