// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1316TupleElementNamesMustUseCorrectCasing"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1316CodeFixProvider))]
    [Shared]
    internal class SA1316CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1316TupleElementNamesMustUseCorrectCasing.DiagnosticId);

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Properties.TryGetValue(SA1316TupleElementNamesMustUseCorrectCasing.ExpectedTupleElementNameKey, out string fixedTupleElementName))
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

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, string fixedTupleElementName, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var identifierToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            var newSyntaxRoot = syntaxRoot.ReplaceToken(identifierToken, SyntaxFactory.Identifier(fixedTupleElementName).WithTriviaFrom(identifierToken));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
