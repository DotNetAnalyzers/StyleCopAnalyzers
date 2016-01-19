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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not
    /// match the actual parameters on the element.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the documentation for an element's parameters does not match the actual
    /// parameters on the element, or if the parameter documentation is not listed in the same order as the element's parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1612ElementParameterDocumentationMustMatchElementParameters : ElementDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1612";
        private const string Title = "Element parameter documentation must match element parameters";
        private const string Description = "The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not match the actual parameters on the element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1612.md";

        private const string MissingParamForDocumentationMessageFormat = "The parameter '{0}' does not exist.";
        private const string ParamWrongOrderMessageFormat = "The parameter documentation for '{0}' should be at position {1}.";

        private static readonly DiagnosticDescriptor MissingParameterDescriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MissingParamForDocumentationMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor OrderDescriptor =
                   new DiagnosticDescriptor(DiagnosticId, Title, ParamWrongOrderMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public SA1612ElementParameterDocumentationMustMatchElementParameters()
            : base(matchElementName: XmlCommentHelper.ParamXmlTag, inheritDocSuppressesWarnings: true)
        {
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(MissingParameterDescriptor);

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            var node = context.Node;
            var identifier = GetIdentifier(node);

            bool supportedIdentifier = identifier != null;
            if (!supportedIdentifier)
            {
                return;
            }

            var identifierLocation = identifier.Value.GetLocation();
            var parameterList = GetParameters(node)?.ToImmutableArray();

            bool hasNoParameters = !parameterList?.Any() ?? false;
            if (hasNoParameters)
            {
                return;
            }

            var xmlParameterNames = syntaxList
                .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                .Select(x => new Tuple<string, Location>(x?.Identifier?.Identifier.ValueText, x?.Identifier.GetLocation()))
                .ToImmutableArray();

            VerifyParameters(context, parameterList.Value, xmlParameterNames, identifierLocation);
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            var node = context.Node;
            var identifier = GetIdentifier(node);

            bool supportedIdentifier = identifier != null;
            if (!supportedIdentifier)
            {
                return;
            }

            var identifierLocation = identifier.Value.GetLocation();
            var parameterList = GetParameters(node)?.ToImmutableArray();

            bool hasNoParameters = !parameterList?.Any() ?? false;
            if (hasNoParameters)
            {
                return;
            }

            // We are working with an <include> element
            var xmlParameterNames = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(e => e.Name == XmlCommentHelper.ParamXmlTag)
                .SelectMany(p => p.Attributes().Where(a => a.Name == "name"))
                .Select(a => new Tuple<string, Location>(a.Value, null))
                .ToImmutableArray();

            VerifyParameters(context, parameterList.Value, xmlParameterNames, identifierLocation);
        }

        private static void VerifyParameters(SyntaxNodeAnalysisContext context, ImmutableArray<ParameterSyntax> parentParameters, ImmutableArray<Tuple<string, Location>> documentationParameters, Location identifierLocation)
        {
            var index = 0;

            foreach (var documentedParameter in documentationParameters)
            {
                // Make sure we ignore violations that should be reported by SA1613 instead.
                if (string.IsNullOrWhiteSpace(documentedParameter.Item1))
                {
                    return;
                }

                var parentParameter = parentParameters.FirstOrDefault(s => s.Identifier.Text == documentedParameter.Item1);

                if (parentParameter == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MissingParameterDescriptor, documentedParameter.Item2 ?? identifierLocation, documentedParameter.Item1));
                }
                else if (parentParameters.Length <= index || parentParameters[index] != parentParameter)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            OrderDescriptor,
                            documentedParameter.Item2 ?? identifierLocation,
                            documentedParameter.Item1,
                            parentParameters.IndexOf(parentParameter) + 1));
                }

                index++;
            }
        }

        private static IEnumerable<ParameterSyntax> GetParameters(SyntaxNode node)
        {
            return (node as BaseMethodDeclarationSyntax)?.ParameterList?.Parameters
                ?? (node as IndexerDeclarationSyntax)?.ParameterList?.Parameters
                ?? (node as DelegateDeclarationSyntax)?.ParameterList?.Parameters;
        }

        private static SyntaxToken? GetIdentifier(SyntaxNode node)
        {
            return (node as MethodDeclarationSyntax)?.Identifier
                ?? (node as IndexerDeclarationSyntax)?.ThisKeyword
                ?? (node as DelegateDeclarationSyntax)?.Identifier;
        }
    }
}
