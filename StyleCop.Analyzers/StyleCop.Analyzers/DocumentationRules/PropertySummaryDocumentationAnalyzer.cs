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
    using StyleCop.Analyzers.Lightup;
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
            if (!(syntax is XmlElementSyntax summaryElement))
            {
                // This is reported by SA1604 or SA1606.
                return;
            }

            if (SummaryStartsWithInheritdoc(summaryElement))
            {
                // Ignore nodes starting with an <inheritdoc/> tag.
                return;
            }

            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            var propertyType = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type.StripRefFromType());
            var culture = settings.DocumentationRules.DocumentationCultureInfo;
            var resourceManager = DocumentationResources.ResourceManager;

            if (propertyType.Type.SpecialType == SpecialType.System_Boolean)
            {
                AnalyzeSummaryElement(
                    context,
                    summaryElement,
                    diagnosticLocation,
                    propertyDeclaration,
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextSetsWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextInitializesWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrSetsWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrInitializesWhether), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextReturnsWhether), culture));
            }
            else
            {
                AnalyzeSummaryElement(
                    context,
                    summaryElement,
                    diagnosticLocation,
                    propertyDeclaration,
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGets), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextSets), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextInitializes), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrSets), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrInitializes), culture),
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextReturns), culture));
            }
        }

        private static void AnalyzeSummaryElement(
            SyntaxNodeAnalysisContext context,
            XmlElementSyntax summaryElement,
            Location diagnosticLocation,
            PropertyDeclarationSyntax propertyDeclaration,
            string startingTextGets,
            string startingTextSets,
            string startingTextInitializes,
            string startingTextGetsOrSets,
            string startingTextGetsOrInitializes,
            string startingTextReturns)
        {
            var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();
            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;
            AccessorDeclarationSyntax getter = null;
            AccessorDeclarationSyntax setter = null;
            bool setterIsInitOnly = false;

            if (propertyDeclaration.AccessorList != null)
            {
                foreach (var accessor in propertyDeclaration.AccessorList.Accessors)
                {
                    switch (accessor.Keyword.Kind())
                    {
                    case SyntaxKind.GetKeyword:
                        getter = accessor;
                        break;

                    case SyntaxKind.SetKeyword:
                    case SyntaxKindEx.InitKeyword:
                        setter = accessor;
                        setterIsInitOnly = accessor.Keyword.IsKind(SyntaxKindEx.InitKeyword);
                        break;
                    }
                }
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
            bool prefixIsGetsOrInitializes = text.StartsWith(startingTextGetsOrInitializes, StringComparison.OrdinalIgnoreCase);
            bool prefixIsGets = !prefixIsGetsOrSets && !prefixIsGetsOrInitializes && text.StartsWith(startingTextGets, StringComparison.OrdinalIgnoreCase);
            bool prefixIsSets = text.StartsWith(startingTextSets, StringComparison.OrdinalIgnoreCase);
            bool prefixIsInitializes = text.StartsWith(startingTextInitializes, StringComparison.OrdinalIgnoreCase);

            bool getterVisible, setterVisible;
            if (getter != null && setter != null)
            {
                if (!getter.Modifiers.Any() && !setter.Modifiers.Any())
                {
                    // The getter and setter have the same declared accessibility
                    getterVisible = true;
                    setterVisible = true;
                }
                else if (getter.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    getterVisible = false;
                    setterVisible = true;
                }
                else if (setter.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    getterVisible = true;
                    setterVisible = false;
                }
                else
                {
                    var propertyAccessibility = propertyDeclaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                    bool propertyOnlyInternal = propertyAccessibility == Accessibility.Internal
                                                || propertyAccessibility == Accessibility.ProtectedAndInternal
                                                || propertyAccessibility == Accessibility.Private;
                    if (propertyOnlyInternal)
                    {
                        // Property only internal and no accessor is explicitly private
                        getterVisible = true;
                        setterVisible = true;
                    }
                    else
                    {
                        var getterAccessibility = getter.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                        var setterAccessibility = setter.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);

                        switch (getterAccessibility)
                        {
                        case Accessibility.Public:
                        case Accessibility.ProtectedOrInternal:
                        case Accessibility.Protected:
                            getterVisible = true;
                            break;

                        case Accessibility.Internal:
                        case Accessibility.ProtectedAndInternal:
                        case Accessibility.Private:
                        default:
                            // The property is externally accessible, so the setter must be more accessible.
                            getterVisible = false;
                            break;
                        }

                        switch (setterAccessibility)
                        {
                        case Accessibility.Public:
                        case Accessibility.ProtectedOrInternal:
                        case Accessibility.Protected:
                            setterVisible = true;
                            break;

                        case Accessibility.Internal:
                        case Accessibility.ProtectedAndInternal:
                        case Accessibility.Private:
                        default:
                            // The property is externally accessible, so the getter must be more accessible.
                            setterVisible = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (getter != null || expressionBody != null)
                {
                    getterVisible = true;
                    setterVisible = false;
                }
                else
                {
                    getterVisible = false;
                    setterVisible = setter != null;
                }
            }

            if (getterVisible)
            {
                if (setterVisible)
                {
                    // Both getter and setter are visible.
                    if (setterIsInitOnly)
                    {
                        if (!prefixIsGetsOrInitializes && !prefixIsGets)
                        {
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGetsOrInitializes, unexpectedStartingText1: startingTextGetsOrSets, unexpectedStartingText2: startingTextSets, unexpectedStartingText3: startingTextInitializes, unexpectedStartingText4: startingTextReturns);
                        }
                    }
                    else if (!prefixIsGetsOrSets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGetsOrSets, unexpectedStartingText1: startingTextGetsOrInitializes, unexpectedStartingText2: startingTextGets, unexpectedStartingText3: startingTextSets, unexpectedStartingText4: startingTextInitializes, unexpectedStartingText5: startingTextReturns);
                    }
                }
                else if (setter != null)
                {
                    // Both getter and setter exist, but only getter is visible.
                    if (!prefixIsGets)
                    {
                        if (prefixIsGetsOrSets || prefixIsGetsOrInitializes)
                        {
                            ReportSA1624(context, diagnosticLocation, diagnosticProperties, accessor: "get", expectedStartingText: startingTextGets, startingTextToRemove: prefixIsGetsOrSets ? startingTextGetsOrSets : startingTextGetsOrInitializes);
                        }
                        else
                        {
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGets, unexpectedStartingText1: startingTextSets, unexpectedStartingText2: startingTextInitializes, unexpectedStartingText3: startingTextReturns);
                        }
                    }
                }
                else
                {
                    // Getter exists and is visible. Setter does not exist.
                    if (!prefixIsGets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGets, unexpectedStartingText1: startingTextSets, unexpectedStartingText2: startingTextInitializes, unexpectedStartingText3: startingTextGetsOrSets, unexpectedStartingText4: startingTextGetsOrInitializes, unexpectedStartingText5: startingTextReturns);
                    }
                }
            }
            else if (setterVisible)
            {
                if (getter != null)
                {
                    // Both getter and setter exist, but only setter is visible.
                    if (setterIsInitOnly)
                    {
                        if (!prefixIsInitializes)
                        {
                            if (prefixIsGetsOrSets || prefixIsGetsOrInitializes)
                            {
                                ReportSA1624(context, diagnosticLocation, diagnosticProperties, accessor: "init", expectedStartingText: startingTextInitializes, startingTextToRemove: prefixIsGetsOrSets ? startingTextGetsOrSets : startingTextGetsOrInitializes);
                            }
                            else
                            {
                                ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextInitializes, unexpectedStartingText1: startingTextGetsOrSets, unexpectedStartingText2: startingTextGets, unexpectedStartingText3: startingTextSets, unexpectedStartingText4: startingTextReturns);
                            }
                        }
                    }
                    else if (!prefixIsSets)
                    {
                        if (prefixIsGetsOrSets || prefixIsGetsOrInitializes)
                        {
                            ReportSA1624(context, diagnosticLocation, diagnosticProperties, accessor: "set", expectedStartingText: startingTextSets, startingTextToRemove: prefixIsGetsOrSets ? startingTextGetsOrSets : startingTextGetsOrInitializes);
                        }
                        else
                        {
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextSets, unexpectedStartingText1: startingTextGetsOrInitializes, unexpectedStartingText2: startingTextGets, unexpectedStartingText3: startingTextInitializes, unexpectedStartingText4: startingTextReturns);
                        }
                    }
                }
                else
                {
                    // Setter exists and is visible. Getter does not exist.
                    if (setterIsInitOnly)
                    {
                        if (!prefixIsInitializes)
                        {
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextInitializes, unexpectedStartingText1: startingTextGetsOrSets, unexpectedStartingText2: startingTextGetsOrInitializes, unexpectedStartingText3: startingTextGets, unexpectedStartingText4: startingTextSets, unexpectedStartingText5: startingTextReturns);
                        }
                    }
                    else if (!prefixIsSets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextSets, unexpectedStartingText1: startingTextGetsOrSets, unexpectedStartingText2: startingTextGetsOrInitializes, unexpectedStartingText3: startingTextGets, unexpectedStartingText4: startingTextInitializes, unexpectedStartingText5: startingTextReturns);
                    }
                }
            }
        }

        private static bool SummaryStartsWithInheritdoc(XmlElementSyntax summaryElement)
        {
            foreach (var child in summaryElement.Content)
            {
                var firstContent = GetFirstMeaningfulChild(child);
                if (firstContent is null)
                {
                    continue;
                }

                return string.Equals(firstContent.GetName()?.ToString(), XmlCommentHelper.InheritdocXmlTag, StringComparison.Ordinal);
            }

            return false;
        }

        private static XmlNodeSyntax GetFirstMeaningfulChild(XmlNodeSyntax node)
        {
            switch (node)
            {
            case XmlTextSyntax textSyntax:
                foreach (var token in textSyntax.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ValueText))
                    {
                        return textSyntax;
                    }
                }

                return null;

            case XmlEmptyElementSyntax emptyElement:
                return emptyElement;

            case XmlElementSyntax elementSyntax:
                if (string.Equals(elementSyntax.StartTag?.Name?.ToString(), XmlCommentHelper.InheritdocXmlTag, StringComparison.Ordinal))
                {
                    return elementSyntax;
                }

                foreach (var child in elementSyntax.Content)
                {
                    var nested = GetFirstMeaningfulChild(child);
                    if (nested != null)
                    {
                        return nested;
                    }
                }

                return null;

            case XmlCDataSectionSyntax cdataSyntax:
                foreach (var token in cdataSyntax.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ValueText))
                    {
                        return cdataSyntax;
                    }
                }

                return null;

            default:
                return null;
            }
        }

        private static void ReportSA1623(SyntaxNodeAnalysisContext context, Location diagnosticLocation, ImmutableDictionary<string, string>.Builder diagnosticProperties, string text, string expectedStartingText, string unexpectedStartingText1, string unexpectedStartingText2 = null, string unexpectedStartingText3 = null, string unexpectedStartingText4 = null, string unexpectedStartingText5 = null)
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
            else if ((unexpectedStartingText4 != null) && text.StartsWith(unexpectedStartingText4, StringComparison.OrdinalIgnoreCase))
            {
                diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, unexpectedStartingText4.Length));
            }
            else if ((unexpectedStartingText5 != null) && text.StartsWith(unexpectedStartingText5, StringComparison.OrdinalIgnoreCase))
            {
                diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, unexpectedStartingText5.Length));
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
