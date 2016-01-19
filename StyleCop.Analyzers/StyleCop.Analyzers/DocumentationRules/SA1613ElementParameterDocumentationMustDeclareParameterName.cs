// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A <c>&lt;param&gt;</c> tag within a C# element's documentation header is missing a <c>name</c> attribute
    /// containing the name of the parameter.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
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
        private const string Title = "Element parameter documentation must declare parameter name";
        private const string MessageFormat = "Element parameter documentation must declare parameter name";
        private const string Description = "A <param> tag within a C# element's documentation header is missing a name attribute containing the name of the parameter.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1613.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Initializes a new instance of the <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/> class.
        /// </summary>
        /// <remarks>The presence of a &lt;inheritdoc/&gt; tag should NOT suppress warnings from this diagnostic. See DotNetAnalyzers/StyleCopAnalyzers#631</remarks>
        public SA1613ElementParameterDocumentationMustDeclareParameterName()
            : base(matchElementName: XmlCommentHelper.ParamXmlTag, inheritDocSuppressesWarnings: false)
        {
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            var xmlParameterNames = syntaxList
                .Where(x => string.Equals(x.GetName()?.ToString(), XmlCommentHelper.ParamXmlTag))
                .Select(x =>
                {
                    var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(x);
                    var location = x.GetLocation();

                    if (nameAttribute != null)
                    {
                        location = nameAttribute.GetLocation();
                    }

                    return new Tuple<string, Location>(nameAttribute?.Identifier?.Identifier.ValueText, location);
                })
                .ToImmutableArray();

            VerifyParameters(context, xmlParameterNames, diagnosticLocations.First());
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            var xmlParameterNames = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(e => e.Name == XmlCommentHelper.ParamXmlTag)
                .Select(x =>
                {
                    var name = x.Attributes().FirstOrDefault(a => a.Name == "name")?.Value;

                    return new Tuple<string, Location>(name, null);
                })
                .ToImmutableArray();

            VerifyParameters(context, xmlParameterNames, diagnosticLocations.First());
        }

        private static void VerifyParameters(SyntaxNodeAnalysisContext context, ImmutableArray<Tuple<string, Location>> documentationParameters, Location identifierLocation)
        {
            var index = 0;

            foreach (var documentedParameter in documentationParameters)
            {
                if (string.IsNullOrWhiteSpace(documentedParameter.Item1))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, documentedParameter.Item2 ?? identifierLocation));
                }

                index++;
            }
        }
    }
}
