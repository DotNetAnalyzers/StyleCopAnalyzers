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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The XML header documentation for a C# element is missing a <c>&lt;summary&gt;</c> tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when the element documentation is missing a <c>&lt;summary&gt;</c>
    /// tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1604ElementDocumentationMustHaveSummary : ElementDocumentationSummaryBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1604ElementDocumentationMustHaveSummary"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1604";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1604.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1604Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1604MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1604Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, DocumentationCommentTriviaSyntax documentation, XmlNodeSyntax syntax, XElement completeDocumentation, Location[] diagnosticLocations)
        {
            if (!needsComment)
            {
                // A missing summary is allowed for this element.
                return;
            }

            if (completeDocumentation != null)
            {
                // This documentation rule is excluded via the <exclude /> tag
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ExcludeXmlTag))
                {
                    return;
                }

                // We are working with an <include> element
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.SummaryXmlTag))
                {
                    return;
                }

                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                {
                    // Ignore nodes with an <inheritdoc/> tag in the included XML.
                    return;
                }
            }
            else
            {
                if (syntax != null)
                {
                    return;
                }

                if (documentation?.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
                {
                    // Ignore nodes with an <inheritdoc/> tag.
                    return;
                }

                // This documentation rule is excluded via the <exclude /> tag
                if (documentation?.Content.GetFirstXmlElement(XmlCommentHelper.ExcludeXmlTag) != null)
                {
                    return;
                }
            }

            foreach (var location in diagnosticLocations)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }
        }
    }
}
