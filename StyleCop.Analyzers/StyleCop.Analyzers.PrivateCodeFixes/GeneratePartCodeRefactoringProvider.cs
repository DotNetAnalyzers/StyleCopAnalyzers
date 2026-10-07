// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.PrivateCodeFixes;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Analyzer.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(GeneratePartCodeRefactoringProvider))]
internal sealed class GeneratePartCodeRefactoringProvider
    : CodeRefactoringProvider
{
    public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        var partialType = await context.TryGetRelevantNodeAsync<ClassDeclarationSyntax>(CSharpRefactoringHelpers.Instance).ConfigureAwait(false);
        if (partialType is not { Modifiers: var modifiers }
            || !modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        context.RegisterRefactoring(CodeAction.Create(
            "Generate additional part",
            async cancellationToken =>
            {
                var namespaceDeclaration = partialType.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
                if (namespaceDeclaration is null)
                {
                    return context.Document.Project.Solution;
                }

                var firstUsing = namespaceDeclaration.Usings.FirstOrDefault()?.Name.ToString();

                var namespaceName = namespaceDeclaration.Name.ToString();
                var subNamespace = namespaceName;
                var rootNamespace = context.Document.Project.DefaultNamespace;
                if (!string.IsNullOrEmpty(rootNamespace) && namespaceName.StartsWith(rootNamespace + "."))
                {
                    subNamespace = namespaceName[(rootNamespace.Length + 1)..];
                }

                var content = $@"// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace {namespaceName};

using {firstUsing};

public partial class {partialType.Identifier.ValueText} {partialType.BaseList}
{{
}}
";

                var fileName = partialType.Identifier.ValueText + ".cs";
                var directory = Path.GetDirectoryName(context.Document.Project.FilePath)!;
                var existingText = await context.Document.GetTextAsync(cancellationToken).ConfigureAwait(false);

                var addedDocument = context.Document.Project.AddDocument(
                    fileName,
                    SourceText.From(content, new UTF8Encoding(true), existingText.ChecksumAlgorithm),
                    folders: subNamespace.Split('.'),
                    filePath: Path.Combine(directory, Path.Combine(subNamespace.Split('.')), fileName));

                return addedDocument.Project.Solution;
            },
            nameof(GeneratePartCodeRefactoringProvider)));
    }
}
