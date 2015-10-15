// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1412StoreFilesAsUtf8"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the encoding to UTF-8 with preamble.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1412CodeFixProvider))]
    [Shared]
    internal class SA1412CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1412StoreFilesAsUtf8.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return new SA1412FixAllProvider();
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                string usedEncoding = diagnostic.Properties[SA1412StoreFilesAsUtf8.EncodingProperty];

                context.RegisterCodeFix(
                    CodeAction.Create(
                        string.Format(MaintainabilityResources.SA1412CodeFix, usedEncoding),
                        cancellationToken => GetTransformedSolutionAsync(context.Document, cancellationToken),
                        nameof(SA1412CodeFixProvider) + "." + usedEncoding),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        internal static async Task<Solution> GetTransformedSolutionAsync(Document document, CancellationToken cancellationToken)
        {
            SourceText text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            string actualSourceText = text.ToString();

            text = SourceText.From(actualSourceText, Encoding.UTF8);

            // Changing the encoding as part of a "normal" text change does not work.
            // Roslyn will not see an encoding change as a text change and assumes that
            // there is nothing to do.
            Solution solutionWithoutDocument = document.Project.Solution.RemoveDocument(document.Id);
            return solutionWithoutDocument.AddDocument(DocumentId.CreateNewId(document.Project.Id), document.Name, text, document.Folders, document.FilePath);
        }
    }
}
