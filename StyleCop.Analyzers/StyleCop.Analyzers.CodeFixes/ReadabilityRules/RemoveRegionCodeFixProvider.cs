// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1123DoNotPlaceRegionsWithinElements"/> and <see cref="SA1124DoNotUseRegions"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the region.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRegionCodeFixProvider))]
    [Shared]
    internal class RemoveRegionCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1123DoNotPlaceRegionsWithinElements.DiagnosticId, SA1124DoNotUseRegions.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            // The batch fixer does not do a very good job if regions are stacked in each other
            return new RemoveRegionFixAllProvider();
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.RemoveRegionCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic),
                        nameof(RemoveRegionCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
            var node = syntaxRoot?.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
            if (node != null && node.IsKind(SyntaxKind.RegionDirectiveTrivia))
            {
                var regionDirective = node as RegionDirectiveTriviaSyntax;

                var newSyntaxRoot = syntaxRoot.RemoveNodes(regionDirective.GetRelatedDirectives(), SyntaxRemoveOptions.AddElasticMarker);

                return document.WithSyntaxRoot(newSyntaxRoot);
            }

            return document.WithSyntaxRoot(syntaxRoot);
        }
    }
}
