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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
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

                // FindToken without this flag returns the token that owns the trivia, so a documentation cref
                // would replace that token (for example, the class keyword) instead of the referenced name.
                var seenStarts = new HashSet<int>();
                var tokens = new List<SyntaxToken>();
                foreach (var location in treeGroup)
                {
                    var token = root.FindToken(location.SourceSpan.Start, findInsideTrivia: true);
                    if (token.Span.Start == location.SourceSpan.Start && seenStarts.Add(token.Span.Start))
                    {
                        tokens.Add(token);
                    }
                }

                if (tokens.Count == 0)
                {
                    continue;
                }

                // Renaming the identifier in `new { value.nameA }` would also change the inferred property name.
                // Keep that name explicit before the remaining references are renamed.
                var renameAnnotation = new SyntaxAnnotation("SA1316Rename");
                root = root.ReplaceTokens(tokens, (original, rewritten) => original.WithAdditionalAnnotations(renameAnnotation));
                var declarators = root.GetAnnotatedTokens(renameAnnotation)
                    .Select(GetInferredAnonymousMember)
                    .OfType<AnonymousObjectMemberDeclaratorSyntax>()
                    .Distinct()
                    .ToList();
                if (declarators.Count > 0)
                {
                    root = root.ReplaceNodes(
                        declarators,
                        (original, rewritten) => PreserveInferredAnonymousName(rewritten, renameAnnotation, fixedTupleElementName));
                }

                var remaining = root.GetAnnotatedTokens(renameAnnotation).ToList();
                if (remaining.Count > 0)
                {
                    root = root.ReplaceTokens(
                        remaining,
                        (original, rewritten) => SyntaxFactory.Identifier(fixedTupleElementName).WithTriviaFrom(original));
                }

                solution = solution.WithDocumentSyntaxRoot(referenceDocument.Id, root);
            }

            return solution;
        }

        private static AnonymousObjectMemberDeclaratorSyntax? GetInferredAnonymousMember(SyntaxToken token)
        {
            if (token.Parent is not IdentifierNameSyntax identifierName
                || identifierName.Parent is not MemberAccessExpressionSyntax memberAccess
                || memberAccess.Name != identifierName
                || memberAccess.Parent is not AnonymousObjectMemberDeclaratorSyntax declarator
                || declarator.NameEquals != null
                || declarator.Expression != memberAccess)
            {
                return null;
            }

            return declarator;
        }

        private static SyntaxNode PreserveInferredAnonymousName(AnonymousObjectMemberDeclaratorSyntax declarator, SyntaxAnnotation annotation, string fixedTupleElementName)
        {
            SyntaxToken? nameToken = null;
            foreach (var token in declarator.GetAnnotatedTokens(annotation))
            {
                if (GetInferredAnonymousMember(token) != null)
                {
                    nameToken = token;
                    break;
                }
            }

            if (nameToken == null)
            {
                return declarator;
            }

            var renamedExpression = declarator.Expression.ReplaceToken(
                nameToken.Value,
                SyntaxFactory.Identifier(fixedTupleElementName).WithTriviaFrom(nameToken.Value));
            var equalsToken = SyntaxFactory.Token(
                SyntaxFactory.TriviaList(SyntaxFactory.Space),
                SyntaxKind.EqualsToken,
                SyntaxFactory.TriviaList(SyntaxFactory.Space));
            return declarator
                .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(nameToken.Value.ValueText)).WithEqualsToken(equalsToken))
                .WithExpression(renamedExpression);
        }
    }
}
