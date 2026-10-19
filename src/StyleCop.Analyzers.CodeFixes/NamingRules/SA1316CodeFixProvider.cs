// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.FindSymbols;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1316TupleElementNamesShouldUseCorrectCasing"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1316CodeFixProvider))]
    [Shared]
    internal class SA1316CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1316TupleElementNamesShouldUseCorrectCasing.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider? GetFixAllProvider()
        {
            // Fix All is not yet supported
            return null;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Properties.TryGetValue(SA1316TupleElementNamesShouldUseCorrectCasing.ExpectedTupleElementNameKey, out string fixedTupleElementName))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            NamingResources.SA1316CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, fixedTupleElementName, cancellationToken),
                            nameof(SA1316CodeFixProvider)),
                        diagnostic);
                }
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Solution> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, string fixedTupleElementName, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var identifierToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var symbol = semanticModel.GetDeclaredSymbol(identifierToken.Parent, cancellationToken);

            // Tuple literal names, such as (name: 1), are not declared symbols. Renamer also fails on tuple element
            // symbols, so declared names and their references are replaced directly.
            if (symbol == null)
            {
                var replacedRoot = syntaxRoot.ReplaceToken(
                    identifierToken,
                    SyntaxFactory.Identifier(fixedTupleElementName).WithTriviaFrom(identifierToken));
                return document.WithSyntaxRoot(replacedRoot).Project.Solution;
            }

            var solution = document.Project.Solution;
            var referenceLocations = (await SymbolFinder.FindReferencesAsync(symbol, solution, cancellationToken).ConfigureAwait(false))
                .SelectMany(reference => reference.Locations)
                .Select(referenceLocation => referenceLocation.Location)
                .Where(location => location.IsInSource)
                .ToList();
            referenceLocations.Add(identifierToken.GetLocation());

            foreach (var treeGroup in referenceLocations.GroupBy(location => location.SourceTree))
            {
                var referenceDocument = solution.GetDocument(treeGroup.Key);
                if (referenceDocument == null)
                {
                    continue;
                }

                var root = await referenceDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var tokens = treeGroup
                    .Select(location => root.FindToken(location.SourceSpan.Start))
                    .GroupBy(token => token.Span.Start)
                    .Select(group => group.First())
                    .ToList();
                var newRoot = root.ReplaceTokens(
                    tokens,
                    (original, rewritten) => SyntaxFactory.Identifier(fixedTupleElementName).WithTriviaFrom(original));
                solution = solution.WithDocumentSyntaxRoot(referenceDocument.Id, newRoot);
            }

            return solution;
        }
    }
}
