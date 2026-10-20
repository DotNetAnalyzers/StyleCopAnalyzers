// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System;
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
using Microsoft.CodeAnalysis.Formatting;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// Implements a code fix for <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1612CodeFixProvider))]
[Shared]
internal class SA1612CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1612ElementParameterDocumentationMustMatchElementParameters.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
    {
        return CustomFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return;
        }

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            if (!TryGetDocumentation(root, diagnostic, out _, out _, out _))
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    DocumentationResources.SA1612CodeFix,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                    nameof(SA1612CodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null
            || !TryGetDocumentation(root, diagnostic, out DocumentationCommentTriviaSyntax? documentation, out ParameterSyntax[]? parameters, out XmlNodeSyntax[]? parameterElements)
            || documentation == null
            || parameters == null
            || parameterElements == null)
        {
            return document;
        }

        var elementsByName = new Dictionary<string, XmlNodeSyntax>(StringComparer.Ordinal);
        foreach (XmlNodeSyntax element in parameterElements)
        {
            XmlNameAttributeSyntax? nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);
            string? name = nameAttribute?.Identifier.Identifier.ValueText;
            if (name == null || string.IsNullOrWhiteSpace(name))
            {
                return document;
            }

            if (parameters.Any(parameter => parameter.Identifier.ValueText == name) && !elementsByName.ContainsKey(name))
            {
                elementsByName.Add(name, element);
            }
        }

        var orderedElements = new List<XmlNodeSyntax>();
        foreach (ParameterSyntax parameter in parameters)
        {
            string name = parameter.Identifier.ValueText;
            if (elementsByName.TryGetValue(name, out XmlNodeSyntax? element))
            {
                orderedElements.Add(element);
            }
            else
            {
                orderedElements.Add(XmlSyntaxFactory.ParamElement(name));
            }
        }

        SyntaxList<XmlNodeSyntax> content = documentation.Content;
        int[] parameterIndexes = parameterElements.Select(content.IndexOf).ToArray();
        int existingElementCount = parameterIndexes.Length;
        int replacementCount = Math.Min(orderedElements.Count, existingElementCount);
        for (int i = existingElementCount - 1; i >= orderedElements.Count; i--)
        {
            int index = parameterIndexes[i];
            content = content.RemoveAt(index);
            if (index > 0 && content[index - 1] is XmlTextSyntax previousText && XmlCommentHelper.IsConsideredEmpty(previousText))
            {
                content = content.RemoveAt(index - 1);
            }
        }

        for (int i = 0; i < replacementCount; i++)
        {
            content = content.Replace(content[parameterIndexes[i]], orderedElements[i]);
        }

        if (orderedElements.Count > replacementCount)
        {
            SyntaxTrivia exteriorTrivia = GetLastDocumentationCommentExteriorTrivia(documentation);
            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
            int insertionIndex = parameterIndexes[replacementCount - 1] + 1;
            for (int i = replacementCount; i < orderedElements.Count; i++)
            {
                XmlNodeSyntax newLine = XmlSyntaxFactory.NewLine(newLineText);
                XmlNodeSyntax newElement = orderedElements[i];
                if (!exteriorTrivia.Token.IsMissing)
                {
                    newLine = newLine.ReplaceExteriorTrivia(exteriorTrivia);
                    newElement = newElement.ReplaceExteriorTrivia(exteriorTrivia);
                }

                content = content.Insert(insertionIndex++, newLine);
                content = content.Insert(insertionIndex++, newElement);
            }
        }

        DocumentationCommentTriviaSyntax updatedDocumentation = documentation.WithContent(content);
        SyntaxNode updatedRoot = root.ReplaceNode(documentation, updatedDocumentation);
        return document.WithSyntaxRoot(updatedRoot);
    }

    private static bool TryGetDocumentation(
        SyntaxNode root,
        Diagnostic diagnostic,
        out DocumentationCommentTriviaSyntax? documentation,
        out ParameterSyntax[]? parameters,
        out XmlNodeSyntax[]? parameterElements)
    {
        SyntaxNode diagnosticNode = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
        documentation = diagnosticNode.FirstAncestorOrSelf<DocumentationCommentTriviaSyntax>();
        SyntaxNode? documentationOwner = documentation?.ParentTrivia.Token.Parent;
        SyntaxNode? declaration = GetDeclaration(documentationOwner ?? diagnosticNode);

        documentation ??= declaration?.GetDocumentationCommentTriviaSyntax();
        parameters = declaration == null ? null : GetParameters(declaration)?.ToArray();
        parameterElements = documentation?.Content.GetXmlElements(XmlCommentHelper.ParamXmlTag).ToArray();
        return documentation != null
            && declaration != null
            && parameters != null
            && parameters.Length > 0
            && parameterElements != null
            && parameterElements.Length > 0
            && documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag) == null
            && parameterElements.All(element =>
            {
                XmlNameAttributeSyntax? nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);
                return !element.ContainsDiagnostics
                    && !string.IsNullOrWhiteSpace(nameAttribute?.Identifier.Identifier.ValueText);
            });
    }

    private static SyntaxNode? GetDeclaration(SyntaxNode node)
    {
        return (SyntaxNode?)node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>()
            ?? (SyntaxNode?)node.FirstAncestorOrSelf<DelegateDeclarationSyntax>()
            ?? (SyntaxNode?)node.FirstAncestorOrSelf<IndexerDeclarationSyntax>()
            ?? (SyntaxNode?)node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
    }

    private static IEnumerable<ParameterSyntax>? GetParameters(SyntaxNode declaration)
    {
        return (declaration as BaseMethodDeclarationSyntax)?.ParameterList.Parameters
            ?? (declaration as IndexerDeclarationSyntax)?.ParameterList.Parameters
            ?? (declaration as DelegateDeclarationSyntax)?.ParameterList.Parameters
            ?? (declaration as TypeDeclarationSyntax)?.ParameterList()?.Parameters;
    }

    private static SyntaxTrivia GetLastDocumentationCommentExteriorTrivia(DocumentationCommentTriviaSyntax documentation)
    {
        return documentation.DescendantTrivia(descendIntoTrivia: true)
            .LastOrDefault(trivia => trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia));
    }
}
