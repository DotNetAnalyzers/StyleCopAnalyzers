// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Analyzes the correct usage of property summary documentation.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class PropertySummaryDocumentationAnalyzer : PropertyDocumentationBase
    {
        public const string ExpectedTextKey = "ExpectedText";
        public const string TextToRemoveKey = "TextToRemove";

        private const string SA1623DiagnosticId = "SA1623";
        private const string SA1624DiagnosticId = "SA1624";

        private static readonly LocalizableString SA1623Title = new LocalizableResourceString(nameof(DocumentationResources.SA1623Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1623MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1623MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1623Description = new LocalizableResourceString(nameof(DocumentationResources.SA1623Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1623HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1623.md";

        private static readonly LocalizableString SA1624Title = new LocalizableResourceString(nameof(DocumentationResources.SA1624Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1624MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1624MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1624Description = new LocalizableResourceString(nameof(DocumentationResources.SA1624Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1624HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1624.md";

        /// <summary>
        /// Gets the <see cref="DiagnosticDescriptor"/> for SA1623.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1623.</value>
        public static DiagnosticDescriptor SA1623Descriptor { get; } =
            new DiagnosticDescriptor(SA1623DiagnosticId, SA1623Title, SA1623MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1623Description, SA1623HelpLink);

        /// <summary>
        /// Gets the <see cref="DiagnosticDescriptor"/> for SA1624.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1624.</value>
        public static DiagnosticDescriptor SA1624Descriptor { get; } =
            new DiagnosticDescriptor(SA1624DiagnosticId, SA1624Title, SA1624MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1624Description, SA1624HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(SA1623Descriptor, SA1624Descriptor);

        /// <inheritdoc/>
        protected override string XmlTagToHandle => XmlCommentHelper.SummaryXmlTag;

        /// <inheritdoc/>
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, XmlNodeSyntax syntax, XElement completeDocumentation, Location diagnosticLocation)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            var propertyType = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type.StripRefFromType());
            var culture = settings.DocumentationRules.DocumentationCultureInfo;
            var resourceManager = DocumentationResources.ResourceManager;

            if (propertyType.Type.SpecialType == SpecialType.System_Boolean)
            {
                AnalyzeSummaryElement(
                    context,
                    syntax,
                    diagnosticLocation,
                    propertyDeclaration,
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextSetsWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrSetsWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextReturnsWhether), culture));
            }
            else
            {
                AnalyzeSummaryElement(
                    context,
                    syntax,
                    diagnosticLocation,
                    propertyDeclaration,
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGets), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextSets), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrSets), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextReturns), culture));
            }
        }

        private static void AnalyzeSummaryElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, Location diagnosticLocation, PropertyDeclarationSyntax propertyDeclaration, string startingTextGets, string startingTextSets, string startingTextGetsOrSets, string startingTextReturns)
        {
            var propertyData = PropertyAnalyzerHelper.AnalyzePropertyAccessors(propertyDeclaration, context.SemanticModel, context.CancellationToken);
            var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();
            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (!(syntax is XmlElementSyntax summaryElement))
            {
                // This is reported by SA1604 or SA1606.
                return;
            }

            // Add a no code fix tag when the summary element is empty.
            // This will only impact SA1623, because SA1624 cannot trigger with an empty summary.
            if (summaryElement.Content.Count == 0)
            {
                diagnosticProperties.Add(NoCodeFixKey, string.Empty);
            }

            var textElement = XmlCommentHelper.TryGetFirstTextElementWithContent(summaryElement);
            string text = textElement is null ? string.Empty : XmlCommentHelper.GetText(textElement, normalizeWhitespace: true).TrimStart();

            bool prefixIsGetsOrSets = text.StartsWith(startingTextGetsOrSets, StringComparison.OrdinalIgnoreCase);
            bool prefixIsGets = !prefixIsGetsOrSets && text.StartsWith(startingTextGets, StringComparison.OrdinalIgnoreCase);
            bool prefixIsSets = text.StartsWith(startingTextSets, StringComparison.OrdinalIgnoreCase);

            if (propertyData.GetterVisible)
            {
                if (propertyData.SetterVisible)
                {
                    // Both getter and setter are visible.
                    if (!prefixIsGetsOrSets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGetsOrSets, unexpectedStartingText1: startingTextGets, unexpectedStartingText2: startingTextSets, unexpectedStartingText3: startingTextReturns);
                    }
                }
                else if (propertyData.HasSetter)
                {
                    // Both getter and setter exist, but only getter is visible.
                    if (!prefixIsGets)
                    {
                        if (prefixIsGetsOrSets)
                        {
                            ReportSA1624(context, diagnosticLocation, diagnosticProperties, accessor: "get", expectedStartingText: startingTextGets, startingTextToRemove: startingTextGetsOrSets);
                        }
                        else
                        {
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGets, unexpectedStartingText1: startingTextSets, unexpectedStartingText2: startingTextReturns);
                        }
                    }
                }
                else
                {
                    // Getter exists and is visible. Setter does not exist.
                    if (!prefixIsGets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGets, unexpectedStartingText1: startingTextSets, unexpectedStartingText2: startingTextGetsOrSets, unexpectedStartingText3: startingTextReturns);
                    }
                }
            }
            else if (propertyData.SetterVisible)
            {
                if (propertyData.HasGetter)
                {
                    // Both getter and setter exist, but only setter is visible.
                    if (!prefixIsSets)
                    {
                        if (prefixIsGetsOrSets)
                        {
                            ReportSA1624(context, diagnosticLocation, diagnosticProperties, accessor: "set", expectedStartingText: startingTextSets, startingTextToRemove: startingTextGetsOrSets);
                        }
                        else
                        {
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextSets, unexpectedStartingText1: startingTextGets, unexpectedStartingText2: startingTextReturns);
                        }
                    }
                }
                else
                {
                    // Setter exists and is visible. Getter does not exist.
                    if (!prefixIsSets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextSets, unexpectedStartingText1: startingTextGetsOrSets, unexpectedStartingText2: startingTextGets, unexpectedStartingText3: startingTextReturns);
                    }
                }
            }
        }

        private static void ReportSA1623(SyntaxNodeAnalysisContext context, Location diagnosticLocation, ImmutableDictionary<string, string>.Builder diagnosticProperties, string text, string expectedStartingText, string unexpectedStartingText1, string unexpectedStartingText2 = null, string unexpectedStartingText3 = null)
        {
            diagnosticProperties.Add(ExpectedTextKey, expectedStartingText);

            if (text.StartsWith(unexpectedStartingText1, StringComparison.OrdinalIgnoreCase))
            {
                diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, unexpectedStartingText1.Length));
            }
            else if ((unexpectedStartingText2 != null) && text.StartsWith(unexpectedStartingText2, StringComparison.OrdinalIgnoreCase))
            {
                diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, unexpectedStartingText2.Length));
            }
            else if ((unexpectedStartingText3 != null) && text.StartsWith(unexpectedStartingText3, StringComparison.OrdinalIgnoreCase))
            {
                diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, unexpectedStartingText3.Length));
            }

            context.ReportDiagnostic(Diagnostic.Create(SA1623Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), expectedStartingText));
        }

        private static void ReportSA1624(SyntaxNodeAnalysisContext context, Location diagnosticLocation, ImmutableDictionary<string, string>.Builder diagnosticProperties, string accessor, string expectedStartingText, string startingTextToRemove)
        {
            diagnosticProperties.Add(ExpectedTextKey, expectedStartingText);
            diagnosticProperties.Add(TextToRemoveKey, startingTextToRemove);
            context.ReportDiagnostic(Diagnostic.Create(SA1624Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), accessor, expectedStartingText));
        }
    }
}
