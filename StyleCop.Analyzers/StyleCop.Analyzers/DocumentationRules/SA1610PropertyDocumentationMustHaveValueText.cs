// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The XML header documentation for a C# property contains an empty <c>&lt;value&gt;</c> tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>The documentation for properties may include a <c>&lt;value&gt;</c> tag, which describes the value held by
    /// the property.</para>
    ///
    /// <para>A violation of this rule occurs when the <c>&lt;value&gt;</c> tag for a property is empty.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1610PropertyDocumentationMustHaveValueText : PropertyDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1610PropertyDocumentationMustHaveValueText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1610";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1610.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1610Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1610MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1610Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override string XmlTagToHandle => XmlCommentHelper.ValueXmlTag;

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, XmlNodeSyntax syntax, XElement completeDocumentation, Location diagnosticLocation)
        {
            var properties = ImmutableDictionary.Create<string, string>();

            if (completeDocumentation != null)
            {
                var valueTag = completeDocumentation.Nodes().OfType<XElement>().FirstOrDefault(element => element.Name == XmlCommentHelper.ValueXmlTag);
                if (valueTag == null)
                {
                    // handled by SA1609
                    return;
                }

                if (!XmlCommentHelper.IsConsideredEmpty(valueTag))
                {
                    return;
                }

                properties = properties.Add(NoCodeFixKey, string.Empty);
            }
            else
            {
                if (syntax == null)
                {
                    // Handled by SA1609
                    return;
                }

                if (!XmlCommentHelper.IsConsideredEmpty(syntax))
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocation, properties));
        }
    }
}
