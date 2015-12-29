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
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for diagnostics which are fixed by renaming a symbol to a set of given new names.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the symbol so that it is one of the set of new names.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameToAnyCodeFixProvider))]
    [Shared]
    internal class RenameToAnyCodeFixProvider : CodeFixProvider
    {
        private static char[] comma = { ',' };

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1315ParametersShouldMatchInheritedNames.DiagnosticId);

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

                var memberSyntax = RenameHelper.GetParentDeclaration(token);

                SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                var declaredSymbol = semanticModel.GetDeclaredSymbol(memberSyntax);
                if (declaredSymbol == null)
                {
                    continue;
                }

                string[] newNames = diagnostic.Properties[SA1315ParametersShouldMatchInheritedNames.PropertyName].Split(comma);
                foreach (var newName in newNames)
                {
                    if (!await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, context.CancellationToken).ConfigureAwait(false))
                    {
                        continue;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken),
                            nameof(RenameToAnyCodeFixProvider) + newName),
                        diagnostic);
                }
            }
        }
    }
}
