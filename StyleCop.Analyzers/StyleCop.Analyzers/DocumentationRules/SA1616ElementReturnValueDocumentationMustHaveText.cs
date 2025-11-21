// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The <c>&lt;returns&gt;</c> tag within a C# element's documentation header is empty.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if an element contains an empty <c>&lt;returns&gt;</c> tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1616ElementReturnValueDocumentationMustHaveText : ElementDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1616ElementReturnValueDocumentationMustHaveText"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1616";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1616.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1616Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1616MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1616Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public SA1616ElementReturnValueDocumentationMustHaveText()
            : base(matchElementName: XmlCommentHelper.ReturnsXmlTag, inheritDocSuppressesWarnings: true)
        {
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            foreach (var syntax in syntaxList)
            {
                bool isEmpty = syntax is XmlEmptyElementSyntax || XmlCommentHelper.IsConsideredEmpty(syntax);
                if (isEmpty)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation()));
                }
            }
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            var returnsNodes = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(n => n.Name == XmlCommentHelper.ReturnsXmlTag);

            foreach (var node in returnsNodes)
            {
                if (XmlCommentHelper.IsConsideredEmpty(node))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocations.First()));
                }
            }
        }
    }
}
