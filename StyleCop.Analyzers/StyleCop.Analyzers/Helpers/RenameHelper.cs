// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.FindSymbols;
    using Microsoft.CodeAnalysis.Rename;
    using Microsoft.CodeAnalysis.Text;

    internal static class RenameHelper
    {
        public static async Task<Solution> RenameSymbolAsync(Document document, SyntaxNode root, SyntaxToken declarationToken, string newName, CancellationToken cancellationToken)
        {
            var annotatedRoot = root.ReplaceToken(declarationToken, declarationToken.WithAdditionalAnnotations(RenameAnnotation.Create()));
            var annotatedSolution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, annotatedRoot);
            var annotatedDocument = annotatedSolution.GetDocument(document.Id);

            annotatedRoot = await annotatedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var annotatedToken = annotatedRoot.FindToken(declarationToken.SpanStart);

            var semanticModel = await annotatedDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var symbol = semanticModel.GetDeclaredSymbol(annotatedToken.Parent, cancellationToken)
                ?? semanticModel.GetSymbolInfo(annotatedToken.Parent, cancellationToken).Symbol;

            var newSolution = await Renamer.RenameSymbolAsync(annotatedSolution, symbol, newName, null, cancellationToken).ConfigureAwait(false);

            // TODO: return annotatedSolution instead of newSolution if newSolution contains any new errors (for any project)
            return newSolution;
        }

        /// <summary>
        /// Tries to rename multiple symbols in a solution.
        /// </summary>
        /// <remarks>
        /// Algorithm:
        /// 1. Calculate a map that maps a symbol to the set of documents where it was referenced in.
        /// 2. Calculate a map that maps a new name to all the symbols which should be renamed to it.
        /// 3. With this information get a set of symbols that can be safely renamed.
        /// 4. Get all text changes that correspond to the safe renames.
        /// 5. Apply all renames.
        /// </remarks>
        /// <param name="symbols">The set of symbols that should be renamed.</param>
        /// <param name="getNewName">A callback that should return the new name of a given symbol.</param>
        /// <param name="solution">The solution where the rename should be made in.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous process.</param>
        /// <returns>A <see cref="Solution"/> where as much occurences of symbols in <paramref name="symbols"/> are renamed.</returns>
        internal static async Task<Solution> RenameAllSymbolsAsync(
            ImmutableHashSet<ISymbol> symbols,
            Func<ISymbol, string> getNewName,
            Solution solution,
            CancellationToken cancellationToken)
        {
            Dictionary<ISymbol, ImmutableHashSet<Document>> symbolDocuments = await GetSymbolReferencesAsync(symbols, solution, cancellationToken).ConfigureAwait(false);

            ImmutableDictionary<string, ImmutableHashSet<ISymbol>> newNamesToSymbols = GetNewNameReferences(symbols, getNewName);

            ImmutableHashSet<ISymbol> symbolsThatCanBeRenamed = GetSymbolsThatCanBeSafelyRenamed(symbolDocuments, newNamesToSymbols);

            Dictionary<Document, SortedSet<TextChange>> textChanges = await GetTextChangesAsync(getNewName, solution, symbolDocuments, symbolsThatCanBeRenamed).ConfigureAwait(false);

            return await GetChangedSolutionAsync(solution, textChanges).ConfigureAwait(false);
        }

        private static async Task<Solution> GetChangedSolutionAsync(Solution solution, Dictionary<Document, SortedSet<TextChange>> textChanges)
        {
            // Apply text changes
            foreach (var change in textChanges)
            {
                // TODO: parallelize
                var text = await change.Key.GetTextAsync().ConfigureAwait(false);
                solution = solution.WithDocumentText(change.Key.Id, text.WithChanges(change.Value));
            }

            return solution;
        }

        private static async Task<Dictionary<Document, SortedSet<TextChange>>> GetTextChangesAsync(Func<ISymbol, string> getNewName, Solution solution, Dictionary<ISymbol, ImmutableHashSet<Document>> symbolDocuments, ImmutableHashSet<ISymbol> symbolsThatCanBeRenamed)
        {
            Dictionary<Document, SortedSet<TextChange>> textChanges = new Dictionary<Document, SortedSet<TextChange>>();

            foreach (var item in symbolDocuments.SelectMany(x => x.Value))
            {
                if (!textChanges.ContainsKey(item))
                {
                    textChanges.Add(item, new SortedSet<TextChange>(Comparer<TextChange>.Create((a, b) => a.Span.Start.CompareTo(b.Span.Start))));
                }
            }

            foreach (var symbol in symbolsThatCanBeRenamed)
            {
                Solution newSolution = await Renamer.RenameSymbolAsync(solution, symbol, getNewName(symbol), null).ConfigureAwait(false);

                SolutionChanges changes = newSolution.GetChanges(solution);
                var changesDocumentIds = changes.GetProjectChanges().SelectMany(x => x.GetChangedDocuments()).ToArray();

                foreach (var id in changesDocumentIds)
                {
                    var oldDocument = solution.GetDocument(id);
                    var newDocument = newSolution.GetDocument(id);

                    // TODO: parallelize
                    var newTextChanges = await newDocument.GetTextChangesAsync(oldDocument).ConfigureAwait(false);

                    foreach (var textChange in newTextChanges)
                    {
                        textChanges[oldDocument].Add(textChange);
                    }
                }
            }

            return textChanges;
        }

        private static ImmutableHashSet<ISymbol> GetSymbolsThatCanBeSafelyRenamed(Dictionary<ISymbol, ImmutableHashSet<Document>> symbolDocuments, ImmutableDictionary<string, ImmutableHashSet<ISymbol>> newNamesToSymbols)
        {
            var symbolsThatCanBeRenamed = ImmutableHashSet.CreateBuilder<ISymbol>();

            foreach (var item in newNamesToSymbols)
            {
                foreach (var symbol in GetSafeRenames(item.Value, symbolDocuments))
                {
                    symbolsThatCanBeRenamed.Add(symbol);
                }
            }

            return symbolsThatCanBeRenamed.ToImmutable();
        }

        private static ImmutableDictionary<string, ImmutableHashSet<ISymbol>> GetNewNameReferences(ImmutableHashSet<ISymbol> symbols, Func<ISymbol, string> getNewName)
        {
            return symbols
                .GroupBy(x => getNewName(x))
                .ToImmutableDictionary(x => x.Key, x => x.ToImmutableHashSet());
        }

        private static async Task<Dictionary<ISymbol, ImmutableHashSet<Document>>> GetSymbolReferencesAsync(ImmutableHashSet<ISymbol> symbols, Solution solution, CancellationToken cancellationToken)
        {
            Dictionary<ISymbol, ImmutableHashSet<Document>> symbolDocuments = new Dictionary<ISymbol, ImmutableHashSet<Document>>();

            foreach (var symbol in symbols)
            {
                // TODO: This should be parallelized
                symbolDocuments.Add(symbol, await GetDocumentsWithSymbolInItAsync(symbol, solution, cancellationToken).ConfigureAwait(false));
            }

            return symbolDocuments;
        }

        private static IEnumerable<ISymbol> GetSafeRenames(ImmutableHashSet<ISymbol> symbols, Dictionary<ISymbol, ImmutableHashSet<Document>> symbolDocuments)
        {
            // We prefer renames that effect a small number of documents to maximize the number of renames we do.
            // We sort the symbols by the number of effected documents and greedily choose symbols in that order.
            // Note: This is not optimal. If we have documents { a, b, c, d, e, f} and renames that effect these documents:
            //       {a, b, c}, {d, e, f}, {c, d}
            //       we choose poorly.
            // This is essentially the set packing problem which can't be approximated very well.
            HashSet<Document> documents = new HashSet<Document>();

            KeyValuePair<ISymbol, ImmutableHashSet<Document>>[] items = symbolDocuments.Where(x => symbols.Contains(x.Key)).ToArray();

            Array.Sort(items, (a, b) => a.Value.Count.CompareTo(b.Value.Count));

            foreach (var documentsLinkedToSymbol in items)
            {
                if (!documentsLinkedToSymbol.Value.Any(documents.Contains))
                {
                    foreach (var item in documentsLinkedToSymbol.Value)
                    {
                        documents.Add(item);
                        yield return documentsLinkedToSymbol.Key;
                    }
                }
            }
        }

        private static async Task<ImmutableHashSet<Document>> GetDocumentsWithSymbolInItAsync(ISymbol symbol, Solution solution, CancellationToken cancellationToken)
        {
            var references = await SymbolFinder.FindReferencesAsync(symbol, solution, cancellationToken).ConfigureAwait(false);
            var referencedInDocuments = references.SelectMany(x => x.Locations)
                .Select(x => x.Document);

            var resultBuilder = ImmutableHashSet.CreateBuilder<Document>();

            var declarations = symbol.Locations
                .Where(y => y.IsInSource)
                .Select(y => solution.GetDocument(y.SourceTree));

            foreach (var declarationDocuments in declarations)
            {
                resultBuilder.Add(declarationDocuments);
            }

            foreach (var document in referencedInDocuments)
            {
                resultBuilder.Add(document);
            }

            return resultBuilder.ToImmutable();
        }
    }
}
