// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1114ParameterListMustFollowDeclaration"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the parameter list is on the same line as
    /// the opening bracket, or on the next line.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1114SA1115CodeFixProvider))]
    [Shared]
    internal class SA1114SA1115CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
              SA1114ParameterListMustFollowDeclaration.DiagnosticId,
              SA1115ParameterMustFollowComma.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1114SA1115CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1114SA1115CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

            TextChange textChange = new TextChange(token.LeadingTrivia.Span, string.Empty);
            return document.WithText(text.WithChanges(textChange));
        }
    }
}
