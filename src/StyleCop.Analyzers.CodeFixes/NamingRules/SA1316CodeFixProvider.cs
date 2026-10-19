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
    using StyleCop.Analyzers.Lightup;

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
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Properties.TryGetValue(SA1316TupleElementNamesShouldUseCorrectCasing.ExpectedTupleElementNameKey, out string fixedTupleElementName))
                {
                    continue;
                }

                // The fix is omitted when a reference cannot be rewritten without changing meaning or language.
                var fixedSolution = await GetTransformedDocumentAsync(context.Document, diagnostic, fixedTupleElementName, context.CancellationToken).ConfigureAwait(false);
                if (fixedSolution == null)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        NamingResources.SA1316CodeFix,
                        cancellationToken => Task.FromResult(fixedSolution),
                        nameof(SA1316CodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Solution?> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, string fixedTupleElementName, CancellationToken cancellationToken)
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

            // A C# identifier cannot be inserted into another language. Skipping that document would leave the
            // reference broken, so the fix is not offered unless every reference can be rewritten.
            if (!CanRewriteAllReferences(solution, referenceLocations))
            {
                return null;
            }

            var consumerLocations = await GetInferredTupleConsumerLocationsAsync(solution, referenceLocations, fixedTupleElementName, cancellationToken).ConfigureAwait(false);
            if (consumerLocations == null || !CanRewriteAllReferences(solution, consumerLocations))
            {
                return null;
            }

            referenceLocations.AddRange(consumerLocations);

            foreach (var treeGroup in referenceLocations.GroupBy(location => location.SourceTree))
            {
                var referenceDocument = solution.GetDocument(treeGroup.Key);
                if (referenceDocument == null)
                {
                    return null;
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

                if (tokens.Exists(IsUnsafeInferredTupleRename))
                {
                    return null;
                }

                // Renaming the identifier in `new { value.nameA }` or `(value.nameA)` would also change the inferred
                // name. Keep that name explicit before the remaining references are renamed.
                var renameAnnotation = new SyntaxAnnotation("SA1316Rename");
                root = root.ReplaceTokens(tokens, (original, rewritten) => original.WithAdditionalAnnotations(renameAnnotation));
                var tupleArguments = root.GetAnnotatedTokens(renameAnnotation)
                    .Select(GetInferredTupleArgument)
                    .OfType<ArgumentSyntax>()
                    .Distinct()
                    .ToList();
                if (tupleArguments.Count > 0)
                {
                    root = root.ReplaceNodes(
                        tupleArguments,
                        (original, rewritten) => PreserveInferredTupleName(rewritten, renameAnnotation, fixedTupleElementName));
                }

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

            // ValueText drops the '@' from an escaped keyword, so `value.@class` would become `class = ...`.
            var preservedName = nameToken.Value.WithoutTrivia().WithoutAnnotations(annotation);
            return declarator
                .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(preservedName)).WithEqualsToken(equalsToken))
                .WithExpression(renamedExpression);
        }

        private static ArgumentSyntax? GetInferredTupleArgument(SyntaxToken token)
        {
            var argument = token.Parent?.FirstAncestorOrSelf<ArgumentSyntax>();
            if (!IsInferredTupleName(argument, token))
            {
                return null;
            }

            return argument;
        }

        private static bool IsUnsafeInferredTupleRename(SyntaxToken token)
        {
            var argument = token.Parent?.FirstAncestorOrSelf<ArgumentSyntax>();
            if (argument?.NameColon != null
                || argument?.Expression == null
                || !TupleExpressionSyntaxWrapper.IsInstance(argument.Parent)
                || argument.Expression.TryGetInferredMemberName() != token.ValueText)
            {
                return false;
            }

            // Another token supplies the inferred name, so renaming this one does not change it.
            var inferredToken = GetInferredNameToken(argument.Expression);
            return inferredToken == default;
        }

        private static bool IsInferredTupleName(ArgumentSyntax? argument, SyntaxToken token)
        {
            if (argument?.NameColon != null
                || argument?.Expression == null
                || !TupleExpressionSyntaxWrapper.IsInstance(argument.Parent)
                || argument.Expression.TryGetInferredMemberName() != token.ValueText)
            {
                return false;
            }

            return GetInferredNameToken(argument.Expression).Span == token.Span;
        }

        private static SyntaxToken GetInferredNameToken(ExpressionSyntax expression)
        {
            while (expression != null)
            {
                switch (expression.Kind())
                {
                case SyntaxKind.IdentifierName:
                    return ((IdentifierNameSyntax)expression).Identifier;

                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.PointerMemberAccessExpression:
                    return ((MemberAccessExpressionSyntax)expression).Name.Identifier;

                case SyntaxKind.ConditionalAccessExpression:
                    expression = ((ConditionalAccessExpressionSyntax)expression).WhenNotNull;
                    continue;

                case SyntaxKind.MemberBindingExpression:
                    return ((MemberBindingExpressionSyntax)expression).Name.Identifier;

                case SyntaxKind.ParenthesizedExpression:
                    expression = ((ParenthesizedExpressionSyntax)expression).Expression;
                    continue;

                case SyntaxKind.CastExpression:
                    expression = ((CastExpressionSyntax)expression).Expression;
                    continue;

                default:
                    return default;
                }
            }

            return default;
        }

        private static SyntaxNode PreserveInferredTupleName(ArgumentSyntax argument, SyntaxAnnotation annotation, string fixedTupleElementName)
        {
            SyntaxToken? nameToken = null;
            foreach (var token in argument.GetAnnotatedTokens(annotation))
            {
                if (IsInferredTupleName(argument, token))
                {
                    nameToken = token;
                    break;
                }
            }

            if (nameToken == null)
            {
                return argument;
            }

            var renamedExpression = argument.Expression.ReplaceToken(
                nameToken.Value,
                SyntaxFactory.Identifier(fixedTupleElementName).WithTriviaFrom(nameToken.Value));
            var colonToken = SyntaxFactory.Token(SyntaxKind.ColonToken).WithTrailingTrivia(SyntaxFactory.Space);

            // Use the corrected name. Keeping the old inferred name would be diagnosed again and then collide.
            return argument
                .WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(fixedTupleElementName), colonToken))
                .WithExpression(renamedExpression);
        }

        private static bool CanRewriteAllReferences(Solution solution, IEnumerable<Location> locations)
        {
            foreach (var location in locations)
            {
                if (solution.GetDocument(location.SourceTree)?.Project.Language != LanguageNames.CSharp)
                {
                    return false;
                }
            }

            return true;
        }

        private static async Task<List<Location>?> GetInferredTupleConsumerLocationsAsync(Solution solution, IReadOnlyList<Location> referenceLocations, string fixedTupleElementName, CancellationToken cancellationToken)
        {
            var consumers = new List<Location>();
            foreach (var treeGroup in referenceLocations.GroupBy(location => location.SourceTree))
            {
                var referenceDocument = solution.GetDocument(treeGroup.Key);
                if (referenceDocument == null)
                {
                    return null;
                }

                var root = await referenceDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var semanticModel = await referenceDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                foreach (var location in treeGroup)
                {
                    var token = root.FindToken(location.SourceSpan.Start, findInsideTrivia: true);
                    if (token.Span.Start != location.SourceSpan.Start)
                    {
                        continue;
                    }

                    var argument = GetInferredTupleArgument(token);
                    if (argument == null)
                    {
                        continue;
                    }

                    if (!TryGetInferredTupleElement(semanticModel, argument, fixedTupleElementName, cancellationToken, out var elementSymbol))
                    {
                        return null;
                    }

                    var elementReferences = await SymbolFinder.FindReferencesAsync(elementSymbol, solution, cancellationToken).ConfigureAwait(false);
                    consumers.AddRange(elementReferences
                        .SelectMany(reference => reference.Locations)
                        .Select(referenceLocation => referenceLocation.Location)
                        .Where(referenceLocation => referenceLocation.IsInSource));
                }
            }

            return consumers;
        }

        private static bool TryGetInferredTupleElement(SemanticModel semanticModel, ArgumentSyntax argument, string fixedTupleElementName, CancellationToken cancellationToken, out IFieldSymbol? elementSymbol)
        {
            elementSymbol = null;
            var tuple = (TupleExpressionSyntaxWrapper)argument.Parent;
            if (tuple.SyntaxNode == null)
            {
                return false;
            }

            var arguments = tuple.Arguments;
            var index = -1;
            for (var i = 0; i < arguments.Count; i++)
            {
                if (arguments[i] == argument)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0 || semanticModel.GetTypeInfo(tuple.SyntaxNode, cancellationToken).Type is not INamedTypeSymbol tupleType)
            {
                return false;
            }

            var elements = tupleType.TupleElements();
            if (elements.IsDefault || index >= elements.Length)
            {
                return false;
            }

            for (var i = 0; i < elements.Length; i++)
            {
                if (i != index && elements[i].Name == fixedTupleElementName)
                {
                    return false;
                }
            }

            elementSymbol = elements[index];
            return true;
        }
    }
}
