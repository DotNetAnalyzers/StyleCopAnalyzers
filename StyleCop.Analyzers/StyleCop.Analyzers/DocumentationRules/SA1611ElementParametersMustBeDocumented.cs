// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    /// A C# method, constructor, delegate or indexer element is missing documentation for one or more of its
    /// parameters.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing parameters is missing documentation for one or
    /// more of its parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1611ElementParametersMustBeDocumented : ElementDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1611ElementParametersMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1611";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1611Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1611MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1611Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1611.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public SA1611ElementParametersMustBeDocumented()
            : base(matchElementName: XmlCommentHelper.ParamXmlTag, inheritDocSuppressesWarnings: true)
        {
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            if (!needsComment)
            {
                // Omitting documentation for a parameter is allowed for this element.
                return;
            }

            var node = context.Node;
            var parameterList = GetParameters(node);
            if (parameterList == null)
            {
                return;
            }

            var xmlParameterNames = syntaxList
                .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                .Where(x => x != null)
                .Select(x => x.Identifier.Identifier.ValueText);

            ReportMissingParameters(context, parameterList, xmlParameterNames);
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            if (!needsComment)
            {
                // Omitting documentation for a parameter is allowed for this element.
                return;
            }

            var node = context.Node;
            var parameterList = GetParameters(node);
            if (parameterList == null)
            {
                return;
            }

            // We are working with an <include> element
            var paramElements = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(e => e.Name == XmlCommentHelper.ParamXmlTag);

            var xmlParameterNames = paramElements
                .SelectMany(p => p.Attributes().Where(a => a.Name == "name"))
                .Select(a => a.Value);

            ReportMissingParameters(context, parameterList, xmlParameterNames);
        }

        private static IEnumerable<ParameterSyntax> GetParameters(SyntaxNode node)
        {
            return (node as BaseMethodDeclarationSyntax)?.ParameterList?.Parameters
                ?? (node as IndexerDeclarationSyntax)?.ParameterList?.Parameters
                ?? (node as DelegateDeclarationSyntax)?.ParameterList?.Parameters;
        }

        private static void ReportMissingParameters(SyntaxNodeAnalysisContext context, IEnumerable<ParameterSyntax> parameterList, IEnumerable<string> documentationParameterNames)
        {
            foreach (var parameter in parameterList)
            {
                if (!documentationParameterNames.Any(x => x == parameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation(), parameter.Identifier.ValueText));
                }
            }
        }
    }
}
