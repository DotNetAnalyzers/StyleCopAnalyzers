// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1629Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1629MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1629Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1629.md";

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
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            foreach (var xmlElement in syntaxList.OfType<XmlElementSyntax>())
            {
                if (xmlElement.StartTag?.Name?.LocalName.ValueText == XmlCommentHelper.SeeAlsoXmlTag)
                {
                    continue;
                }

                var elementDone = false;
                int? reportingLocation = null;
                for (var i = xmlElement.Content.Count - 1; !elementDone && (i >= 0); i--)
                {
                    if (xmlElement.Content[i] is XmlTextSyntax contentNode)
                    {
                        for (var j = contentNode.TextTokens.Count - 1; !elementDone && (j >= 0); j--)
                        {
                            var textToken = contentNode.TextTokens[j];
                            var textWithoutTrailingWhitespace = textToken.Text.TrimEnd(' ', '\r', '\n');

                            if (!string.IsNullOrEmpty(textWithoutTrailingWhitespace))
                            {
                                if (!textWithoutTrailingWhitespace.EndsWith(".", StringComparison.Ordinal))
                                {
                                    var location = Location.Create(xmlElement.SyntaxTree, new TextSpan(reportingLocation ?? textToken.SpanStart + textWithoutTrailingWhitespace.Length, 1));
                                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                                }

                                elementDone = true;
                            }
                        }
                    }
                    else if (xmlElement.Content[i].IsInlineElement())
                    {
                        // If a diagnostic gets reported for the element, place the diagnostic after the last inline
                        // element.
                        reportingLocation = reportingLocation ?? xmlElement.Content[i].Span.End;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            foreach (var node in completeDocumentation.Nodes().OfType<XElement>())
            {
                var textWithoutTrailingWhitespace = node.Value.TrimEnd(' ', '\r', '\n');
                if (!string.IsNullOrEmpty(textWithoutTrailingWhitespace))
                {
                    if (!textWithoutTrailingWhitespace.EndsWith(".", StringComparison.Ordinal))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocations[0], NoCodeFixProperties));

                        // only report a single instance of the diagnostic, as they will all be reported on the same location anyway.
                        break;
                    }
                }
            }
        }
    }
}
