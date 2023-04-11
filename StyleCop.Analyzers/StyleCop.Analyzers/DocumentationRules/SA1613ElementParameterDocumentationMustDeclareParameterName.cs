﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
    /// A <c>&lt;param&gt;</c> tag within a C# element's documentation header is missing a <c>name</c> attribute
    /// containing the name of the parameter.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if the documentation for an element contains a <c>&lt;param&gt;</c> tag
    /// which is missing a <c>name</c> attribute, or which contains an empty <c>name</c> attribute.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1613ElementParameterDocumentationMustDeclareParameterName : ElementDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1613";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1613.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1613Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1613MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1613Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Initializes a new instance of the <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/> class.
        /// </summary>
        /// <remarks><para>The presence of a &lt;inheritdoc/&gt; tag should NOT suppress warnings from this diagnostic.
        /// See DotNetAnalyzers/StyleCopAnalyzers#631.</para></remarks>
        public SA1613ElementParameterDocumentationMustDeclareParameterName()
            : base(matchElementName: XmlCommentHelper.ParamXmlTag, inheritDocSuppressesWarnings: false)
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
                var nameParameter = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(syntax);
                var parameterValue = nameParameter?.Identifier?.Identifier.ValueText;

                if (string.IsNullOrWhiteSpace(parameterValue))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, nameParameter?.GetLocation() ?? syntax.GetLocation()));
                }
            }
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            var xmlParamTags = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(e => e.Name == XmlCommentHelper.ParamXmlTag);

            foreach (var paramTag in xmlParamTags)
            {
                var name = paramTag.Attributes().FirstOrDefault(a => a.Name == "name")?.Value;

                if (string.IsNullOrWhiteSpace(name))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocations.First()));
                }
            }
        }
    }
}
