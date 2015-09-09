// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1004DocumentationLinesMustBeginWithSingleSpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the header line begins with a single space.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1004CodeFixProvider))]
    [Shared]
    public class SA1004CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1004DocumentationLinesMustBeginWithSingleSpace.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                if (!diagnostic.Id.Equals(SA1004DocumentationLinesMustBeginWithSingleSpace.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1004CodeFix, token => GetTransformedDocumentAsync(context.Document, root, diagnostic), equivalenceKey: nameof(SA1004CodeFixProvider)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, Diagnostic diagnostic)
        {
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
            SyntaxToken updatedToken;
            switch (token.Kind())
            {
            case SyntaxKind.XmlTextLiteralToken:
                updatedToken = XmlSyntaxFactory.TextLiteral(" " + token.Text.TrimStart(' ')).WithTriviaFrom(token);
                break;

            default:
                updatedToken = token.WithLeadingTrivia(token.LeadingTrivia.Add(SyntaxFactory.Space));
                break;
            }

            Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, updatedToken));
            return Task.FromResult(updatedDocument);
        }
    }
}
