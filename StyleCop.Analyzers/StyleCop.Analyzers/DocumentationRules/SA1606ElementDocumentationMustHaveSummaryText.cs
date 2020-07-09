// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    /// The <c>&lt;summary&gt;</c> tag within the documentation header for a C# code element is empty.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when the documentation header for an element contains an empty
    /// <c>&lt;summary&gt;</c> tag which does not contain a description of the element.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1606ElementDocumentationMustHaveSummaryText : ElementDocumentationSummaryBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1606ElementDocumentationMustHaveSummaryText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1606";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1606.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1606Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1606MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1606Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, DocumentationCommentTriviaSyntax documentation, XmlNodeSyntax syntax, XElement completeDocumentation, Location[] diagnosticLocations)
        {
            if (syntax == null)
            {
                return;
            }

            if (completeDocumentation != null)
            {
                // This documentation rule is excluded via the <exclude /> tag
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ExcludeXmlTag))
                {
                    return;
                }

                XElement summaryNode = completeDocumentation.Nodes().OfType<XElement>().FirstOrDefault(element => element.Name == XmlCommentHelper.SummaryXmlTag);
                if (summaryNode == null)
                {
                    // Handled by SA1604
                    return;
                }

                if (!XmlCommentHelper.IsConsideredEmpty(summaryNode))
                {
                    return;
                }
            }
            else
            {
                if (!XmlCommentHelper.IsConsideredEmpty(syntax))
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
