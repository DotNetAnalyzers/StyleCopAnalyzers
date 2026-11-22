// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// The opening and closing tags of an XML documentation element are either both on lines by themselves, or
/// neither is.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SA1655DocumentationElementTagsMustBeConsistentlyPlaced : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by this analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1655";

    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1655Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1655MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1655Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

    private static readonly DiagnosticDescriptor Descriptor =
        new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(HandleXmlElement, SyntaxKind.XmlElement);
    }

    private static void HandleXmlElement(SyntaxNodeAnalysisContext context)
    {
        var element = (XmlElementSyntax)context.Node;
        var startTag = element.StartTag;
        var endTag = element.EndTag;
        if (startTag.IsMissing
            || endTag.IsMissing
            || startTag.GreaterThanToken.IsMissing
            || endTag.GreaterThanToken.IsMissing
            || endTag.LessThanSlashToken.IsMissing)
        {
            return;
        }

        var text = element.SyntaxTree.GetText(context.CancellationToken);
        var startLine = text.Lines.GetLineFromPosition(startTag.GreaterThanToken.SpanStart);
        var endLine = text.Lines.GetLineFromPosition(endTag.LessThanSlashToken.SpanStart);
        if (startLine.LineNumber == endLine.LineNumber)
        {
            // Both tags share a line, so neither is on a line by itself.
            return;
        }

        bool startAlone = IsAloneOnLine(text, startTag.LessThanToken.SpanStart, startTag.GreaterThanToken.Span.End);
        bool endAlone = IsAloneOnLine(text, endTag.LessThanSlashToken.SpanStart, endTag.GreaterThanToken.Span.End);
        if (startAlone == endAlone)
        {
            return;
        }

        var location = startAlone ? endTag.GetLocation() : startTag.GetLocation();
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, startTag.Name.ToString()));
    }

    private static bool IsAloneOnLine(SourceText text, int tagStart, int tagEnd)
    {
        var firstLine = text.Lines.GetLineFromPosition(tagStart);
        var lastLine = text.Lines.GetLineFromPosition(tagEnd);

        for (int i = firstLine.Start; i < tagStart; i++)
        {
            // Only the documentation comment exterior ("///", "/**" or "*") may precede the tag.
            char c = text[i];
            if (!char.IsWhiteSpace(c) && c != '/' && c != '*')
            {
                return false;
            }
        }

        for (int i = tagEnd; i < lastLine.End; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                return false;
            }
        }

        return true;
    }
}
