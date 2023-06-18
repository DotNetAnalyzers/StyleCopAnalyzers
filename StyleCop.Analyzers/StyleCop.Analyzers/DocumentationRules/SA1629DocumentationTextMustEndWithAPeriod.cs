// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A section of the XML header documentation for a C# element does not end with a period (also known as a full
    /// stop).
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when part of the documentation does not end with a period. For example,
    /// the summary text in the documentation below does not end with a period:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joins a first name and a last name together into a single string
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;The first name.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;The last name.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1629DocumentationTextMustEndWithAPeriod : ElementDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1629DocumentationTextMustEndWithAPeriod"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1629";

        /// <summary>
        /// The key used for signalling that no codefix should be offered.
        /// </summary>
        internal const string NoCodeFixKey = "NoCodeFix";

        internal const string ReplaceCharKey = "CharToReplace";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1629.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1629Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1629MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1629Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableDictionary<string, string> NoCodeFixProperties = ImmutableDictionary.Create<string, string>().Add(NoCodeFixKey, string.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="SA1629DocumentationTextMustEndWithAPeriod"/> class.
        /// </summary>
        public SA1629DocumentationTextMustEndWithAPeriod()
            : base(inheritDocSuppressesWarnings: false)
        {
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            foreach (var xmlElement in syntaxList.OfType<XmlElementSyntax>())
            {
                HandleSectionOrBlockXmlElement(context, settings, xmlElement, startingWithFinalParagraph: true);
            }
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            foreach (var node in completeDocumentation.Nodes().OfType<XElement>())
            {
                var textWithoutTrailingWhitespace = node.Value.TrimEnd(' ', '\r', '\n');
                if (!string.IsNullOrEmpty(textWithoutTrailingWhitespace))
                {
                    if (!textWithoutTrailingWhitespace.EndsWith(".", StringComparison.Ordinal)
                        && !textWithoutTrailingWhitespace.EndsWith(".)", StringComparison.Ordinal))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocations[0], NoCodeFixProperties));

                        // only report a single instance of the diagnostic, as they will all be reported on the same location anyway.
                        break;
                    }
                }
            }
        }

        private static void HandleSectionOrBlockXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, XmlElementSyntax xmlElement, bool startingWithFinalParagraph)
        {
            var startTag = xmlElement.StartTag?.Name?.LocalName.ValueText;
            if (settings.DocumentationRules.ExcludeFromPunctuationCheck.Contains(startTag))
            {
                return;
            }

            var currentParagraphDone = false;
            for (var i = xmlElement.Content.Count - 1; i >= 0; i--)
            {
                if (xmlElement.Content[i] is XmlTextSyntax contentNode)
                {
                    for (var j = contentNode.TextTokens.Count - 1; !currentParagraphDone && (j >= 0); j--)
                    {
                        var textToken = contentNode.TextTokens[j];
                        var textWithoutTrailingWhitespace = textToken.Text.TrimEnd(' ', '\r', '\n');

                        if (!string.IsNullOrEmpty(textWithoutTrailingWhitespace))
                        {
                            if (!textWithoutTrailingWhitespace.EndsWith(".", StringComparison.Ordinal)
                                && !textWithoutTrailingWhitespace.EndsWith(".)", StringComparison.Ordinal)
                                && (startingWithFinalParagraph || !textWithoutTrailingWhitespace.EndsWith(":", StringComparison.Ordinal))
                                && !textWithoutTrailingWhitespace.EndsWith("-or-", StringComparison.Ordinal))
                            {
                                int spanStart = textToken.SpanStart + textWithoutTrailingWhitespace.Length;
                                ImmutableDictionary<string, string> properties = null;
                                if (textWithoutTrailingWhitespace.EndsWith(",", StringComparison.Ordinal)
                                    || textWithoutTrailingWhitespace.EndsWith(";", StringComparison.Ordinal))
                                {
                                    spanStart -= 1;
                                    SetReplaceChar();
                                }
                                else if (textWithoutTrailingWhitespace.EndsWith(",)", StringComparison.Ordinal)
                                    || textWithoutTrailingWhitespace.EndsWith(";)", StringComparison.Ordinal))
                                {
                                    spanStart -= 2;
                                    SetReplaceChar();
                                }

                                var location = Location.Create(xmlElement.SyntaxTree, new TextSpan(spanStart, 1));
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, properties));

                                void SetReplaceChar()
                                {
                                    var propertiesBuilder = ImmutableDictionary.CreateBuilder<string, string>();
                                    propertiesBuilder.Add(ReplaceCharKey, string.Empty);
                                    properties = propertiesBuilder.ToImmutable();
                                }
                            }

                            currentParagraphDone = true;
                        }
                    }
                }
                else if (xmlElement.Content[i].IsInlineElement() && !currentParagraphDone)
                {
                    // Treat empty XML elements as a "word not ending with a period"
                    var location = Location.Create(xmlElement.SyntaxTree, new TextSpan(xmlElement.Content[i].Span.End, 1));
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    currentParagraphDone = true;
                }
                else if (xmlElement.Content[i] is XmlElementSyntax childXmlElement)
                {
                    switch (childXmlElement.StartTag?.Name?.LocalName.ValueText)
                    {
                    case XmlCommentHelper.NoteXmlTag:
                    case XmlCommentHelper.ParaXmlTag:
                        // Recursively handle <note> and <para> elements
                        HandleSectionOrBlockXmlElement(context, settings, childXmlElement, startingWithFinalParagraph);
                        break;

                    default:
                        break;
                    }

                    if (childXmlElement.IsBlockElement())
                    {
                        currentParagraphDone = false;
                        startingWithFinalParagraph = false;
                    }
                }
                else if (xmlElement.Content[i] is XmlEmptyElementSyntax emptyElement)
                {
                    // Treat the empty element <para/> as a paragraph separator
                    if (emptyElement.Name?.LocalName.ValueText == XmlCommentHelper.ParaXmlTag)
                    {
                        currentParagraphDone = false;
                        startingWithFinalParagraph = false;
                    }
                }
            }
        }
    }
}
