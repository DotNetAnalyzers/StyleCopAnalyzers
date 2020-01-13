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
    /// The XML header documentation for a C# property does not contain a <c>&lt;value&gt;</c> tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>The documentation for properties may include a <c>&lt;value&gt;</c> tag, which describes the value held by
    /// the property.</para>
    ///
    /// <para>A violation of this rule occurs when the <c>&lt;value&gt;</c> tag for a property is missing.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1609PropertyDocumentationMustHaveValue : PropertyDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1609PropertyDocumentationMustHaveValue"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1609";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1609.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1609Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1609MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1609Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override string XmlTagToHandle => XmlCommentHelper.ValueXmlTag;

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, XmlNodeSyntax syntax, XElement completeDocumentation, Location diagnosticLocation)
        {
            if (!needsComment)
            {
                // A missing 'value' documentation is allowed for this element.
                return;
            }

            var properties = ImmutableDictionary.Create<string, string>();

            if (completeDocumentation != null)
            {
                // This documentation rule is excluded via the <exclude /> tag
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ExcludeXmlTag))
                {
                    return;
                }

                var hasValueTag = completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ValueXmlTag);
                if (hasValueTag)
                {
                    return;
                }

                properties = properties.Add(NoCodeFixKey, string.Empty);
            }
            else
            {
                if (syntax != null)
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocation, properties));
        }
    }
}
