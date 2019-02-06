// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

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
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, XmlNodeSyntax syntax, XElement completeDocumentation, Location diagnosticLocation)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            var propertyType = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type.StripRefFromType());
            var settings = context.Options.GetStyleCopSettings(context.CancellationToken);
            var culture = new CultureInfo(settings.DocumentationRules.DocumentationCulture);
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
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrSetsWhether), culture));
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
                    resourceManager.GetString(nameof(DocumentationResources.StartingTextGetsOrSets), culture));
            }
        }

        private static void AnalyzeSummaryElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, Location diagnosticLocation, PropertyDeclarationSyntax propertyDeclaration, string startingTextGets, string startingTextSets, string startingTextGetsOrSets)
        {
            var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();
            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;
            AccessorDeclarationSyntax getter = null;
            AccessorDeclarationSyntax setter = null;

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
                        setter = accessor;
                        break;
                    }
                }
            }

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
                    if (!prefixIsGetsOrSets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGetsOrSets, unexpectedStartingText1: startingTextGets, unexpectedStartingText2: startingTextSets);
                    }
                }
                else if (setter != null)
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
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGets, unexpectedStartingText1: startingTextSets);
                        }
                    }
                }
                else
                {
                    // Getter exists and is visible. Setter does not exist.
                    if (!prefixIsGets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextGets, unexpectedStartingText1: startingTextSets, unexpectedStartingText2: startingTextGetsOrSets);
                    }
                }
            }
            else if (setterVisible)
            {
                if (getter != null)
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
                            ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextSets, unexpectedStartingText1: startingTextGets);
                        }
                    }
                }
                else
                {
                    // Setter exists and is visible. Getter does not exist.
                    if (!prefixIsSets)
                    {
                        ReportSA1623(context, diagnosticLocation, diagnosticProperties, text, expectedStartingText: startingTextSets, unexpectedStartingText1: startingTextGetsOrSets, unexpectedStartingText2: startingTextGets);
                    }
                }
            }
        }

        private static void ReportSA1623(SyntaxNodeAnalysisContext context, Location diagnosticLocation, ImmutableDictionary<string, string>.Builder diagnosticProperties, string text, string expectedStartingText, string unexpectedStartingText1, string unexpectedStartingText2 = null)
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
