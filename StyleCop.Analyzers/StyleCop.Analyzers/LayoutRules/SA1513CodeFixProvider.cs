// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1513ClosingCurlyBracketMustBeFollowedByBlankLine"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure a blank line follows closing curly brackets.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1513CodeFixProvider))]
    [Shared]
    public class SA1513CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1513ClosingCurlyBracketMustBeFollowedByBlankLine.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1513CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1513CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.End);

            var newTrivia = token.LeadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
            var newToken = token.WithLeadingTrivia(newTrivia);
            var newSyntaxRoot = syntaxRoot.ReplaceToken(token, newToken);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return newDocument;
        }
    }
}
