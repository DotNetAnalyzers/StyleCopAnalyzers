// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System.Collections.Immutable;
using System.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SX1412StoreFilesAsUtf8WithoutByteOrderMark"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SX1412CodeFixProvider))]
[Shared]
internal class SX1412CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SX1412StoreFilesAsUtf8WithoutByteOrderMark.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() =>
        new SX1412FixAllProvider();

    /// <inheritdoc/>
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    MaintainabilityResources.SX1412CodeFix,
                    cancellationToken => GetTransformedSolutionAsync(context.Document, cancellationToken),
                    nameof(SX1412CodeFixProvider)),
                diagnostic);
        }

        return SpecializedTasks.CompletedTask;
    }

    internal static async Task<Solution> GetTransformedSolutionAsync(Document document, CancellationToken cancellationToken)
    {
        SourceText text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        SourceText textWithoutByteOrderMark = SourceText.From(text.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        // Changing the encoding as part of a normal text change does not work.
        Solution solutionWithoutDocument = document.Project.Solution.RemoveDocument(document.Id);
        return solutionWithoutDocument.AddDocument(DocumentId.CreateNewId(document.Project.Id), document.Name, textWithoutByteOrderMark, document.Folders, document.FilePath);
    }
}
