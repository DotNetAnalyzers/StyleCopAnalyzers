// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    /// The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not
    /// match the actual parameters on the element.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1612.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1612Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1612Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly LocalizableString MissingParamForDocumentationMessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1612MissingParamForDocumentationMessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString ParamWrongOrderMessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1612ParamWrongOrderMessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));

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
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            var node = context.Node;
            var identifier = GetIdentifier(node);

            bool supportedIdentifier = identifier != null;
            if (!supportedIdentifier)
            {
                return;
            }

            var parameterList = GetParameters(node)?.ToImmutableArray();

            bool hasNoParameters = !parameterList?.Any() ?? false;
            if (hasNoParameters)
            {
                return;
            }

            var parentParameters = parameterList.Value;

            var index = 0;
            foreach (var syntax in syntaxList)
            {
                var nameAttributeSyntax = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(syntax);
                var nameAttributeText = nameAttributeSyntax?.Identifier?.Identifier.ValueText;

                // Make sure we ignore violations that should be reported by SA1613 instead.
                if (string.IsNullOrWhiteSpace(nameAttributeText))
                {
                    return;
                }

                var location = nameAttributeSyntax.Identifier.Identifier.GetLocation();

                var parentParameter = parentParameters.FirstOrDefault(s => s.Identifier.ValueText == nameAttributeText);
                if (parentParameter == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MissingParameterDescriptor, location, nameAttributeText));
                }
                else
                {
                    if (!needsComment)
                    {
                        // Parameter documentation is allowed to be omitted, so skip parameters for which there is no
                        // documentation.
                        while (index < parentParameters.Length && parentParameters[index] != parentParameter)
                        {
                            index++;
                        }
                    }

                    if (parentParameters.Length <= index || parentParameters[index] != parentParameter)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                OrderDescriptor,
                                location,
                                nameAttributeText,
                                parentParameters.IndexOf(parentParameter) + 1));
                    }
                }

                index++;
            }
        }

        /// <inheritdoc/>
        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations)
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
            var xmlParamTags = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(e => e.Name == XmlCommentHelper.ParamXmlTag);

            var parentParameters = parameterList.Value;

            var index = 0;
            foreach (var paramTag in xmlParamTags)
            {
                var nameAttributeText = paramTag.Attributes().FirstOrDefault(a => a.Name == "name")?.Value;

                // Make sure we ignore violations that should be reported by SA1613 instead.
                if (string.IsNullOrWhiteSpace(nameAttributeText))
                {
                    return;
                }

                var parentParameter = parentParameters.FirstOrDefault(s => s.Identifier.ValueText == nameAttributeText);
                if (parentParameter == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MissingParameterDescriptor, identifierLocation, nameAttributeText));
                }
                else
                {
                    if (!needsComment)
                    {
                        // Parameter documentation is allowed to be omitted, so skip parameters for which there is no
                        // documentation.
                        while (index < parentParameters.Length && parentParameters[index] != parentParameter)
                        {
                            index++;
                        }
                    }

                    if (parentParameters.Length <= index || parentParameters[index] != parentParameter)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                OrderDescriptor,
                                identifierLocation,
                                nameAttributeText,
                                parentParameters.IndexOf(parentParameter) + 1));
                    }
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
