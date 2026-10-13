// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
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
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.FindSymbols;
    using Microsoft.CodeAnalysis.Simplification;
    using StyleCop.Analyzers.Helpers;

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

                    if (!SyntaxFacts.IsValidIdentifier(newName))
                    {
                        // The proposed name was not legal, so no code fix will be offered.
                        continue;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            string.Format(NamingResources.RenameToCodeFix, newName),
                            cancellationToken => RenameFieldAsync(document, root, token, newName, cancellationToken),
                            nameof(SA1309CodeFixProvider)),
                        diagnostic);
                }
            }
        }

        private static async Task<Solution> RenameFieldAsync(Document document, SyntaxNode root, SyntaxToken token, string newName, CancellationToken cancellationToken)
        {
            var declarationAnnotation = new SyntaxAnnotation();
            root = root.ReplaceToken(token, token.WithAdditionalAnnotations(declarationAnnotation));
            var solution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, root);
            document = solution.GetDocument(document.Id);
            root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var field = (IFieldSymbol)semanticModel.GetDeclaredSymbol(root.GetAnnotatedTokens(declarationAnnotation).Single().Parent, cancellationToken);
            var references = await SymbolFinder.FindReferencesAsync(field, solution, cancellationToken).ConfigureAwait(false);

            foreach (var referenceDocument in references.SelectMany(reference => reference.Locations).GroupBy(location => location.Document.Id))
            {
                var reference = solution.GetDocument(referenceDocument.Key);
                var referenceRoot = await reference.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var referenceModel = await reference.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                var identifiers = referenceDocument
                    .Select(location => referenceRoot.FindNode(location.Location.SourceSpan, getInnermostNodeForTie: true))
                    .OfType<IdentifierNameSyntax>()
                    .Where(identifier => !(identifier.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == identifier)
                        && !(identifier.Parent is MemberBindingExpressionSyntax)
                        && !(identifier.Parent is AssignmentExpressionSyntax assignment
                            && assignment.Left == identifier
                            && assignment.Parent is InitializerExpressionSyntax)
                        && referenceModel.LookupSymbols(identifier.SpanStart, name: newName)
                            .Any(symbol => symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter))
                    .ToImmutableArray();

                // Qualify before renaming: older rename engines miss conflicts with inline out variables.
                var generator = SyntaxGenerator.GetGenerator(reference);
                referenceRoot = referenceRoot.ReplaceNodes(
                    identifiers,
                    (original, rewritten) =>
                    {
                        var referencedField = (IFieldSymbol)referenceModel.GetSymbolInfo(original, cancellationToken).Symbol;
                        var receiver = referencedField.IsStatic
                            ? ((ExpressionSyntax)generator.TypeExpression(referencedField.ContainingType)).WithAdditionalAnnotations(Simplifier.Annotation)
                            : SyntaxFactory.ThisExpression();
                        return SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            receiver,
                            rewritten.WithoutTrivia()).WithTriviaFrom(original);
                    });
                solution = solution.WithDocumentSyntaxRoot(reference.Id, referenceRoot);
            }

            document = solution.GetDocument(document.Id);
            root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            token = root.GetAnnotatedTokens(declarationAnnotation).Single();
            return await RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken).ConfigureAwait(false);
        }
    }
}
