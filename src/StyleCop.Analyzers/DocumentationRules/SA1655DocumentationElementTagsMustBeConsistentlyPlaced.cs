// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System.Collections.Immutable;
using System.Linq;
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

        bool isBlockComment = element.Ancestors().Any(n => n.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
        bool startAlone = IsAloneOnLine(text, startTag.LessThanToken, startTag.GreaterThanToken, isBlockComment);
        bool endAlone = IsAloneOnLine(text, endTag.LessThanSlashToken, endTag.GreaterThanToken, isBlockComment);
        if (startAlone == endAlone)
        {
            return;
        }

        var location = startAlone ? endTag.GetLocation() : startTag.GetLocation();
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, startTag.Name.ToString()));
    }

    private static bool IsAloneOnLine(SourceText text, SyntaxToken firstToken, SyntaxToken lastToken, bool isBlockComment)
    {
        int tagStart = firstToken.SpanStart;
        int tagEnd = lastToken.Span.End;
        var firstLine = text.Lines.GetLineFromPosition(tagStart);
        var lastLine = text.Lines.GetLineFromPosition(tagEnd);

        // Only the documentation comment exterior trivia may precede the tag on its line.
        int prefixStart = firstLine.Start;
        for (var token = firstToken; !token.IsKind(SyntaxKind.None) && token.FullSpan.End > firstLine.Start; token = token.GetPreviousToken(includeZeroWidth: true))
        {
            var exterior = token.LeadingTrivia.LastOrDefault(t => t.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia) && t.SpanStart >= firstLine.Start);
            if (exterior.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia))
            {
                prefixStart = exterior.Span.End;
                break;
            }
        }

        for (int i = prefixStart; i < tagStart; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                return false;
            }
        }

        int suffixEnd = lastLine.End;
        if (isBlockComment)
        {
            // The block comment terminator is not XML content.
            int terminator = text.ToString(TextSpan.FromBounds(tagEnd, suffixEnd)).IndexOf("*/", System.StringComparison.Ordinal);
            if (terminator >= 0)
            {
                suffixEnd = tagEnd + terminator;
            }
        }

        for (int i = tagEnd; i < suffixEnd; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                return false;
            }
        }

        return true;
    }
}
