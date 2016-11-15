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
        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, XElement completeDocumentation, Location diagnosticLocation)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            var propertyType = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type);
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

            XmlElementSyntax summaryElement = (XmlElementSyntax)syntax;
            if (summaryElement == null)
            {
                // This is reported by SA1604.
                return;
            }

            // Add a no code fix tag when the summary element is empty.
            // This will only impact SA1623, because SA1624 cannot trigger with an empty summary.
            if (summaryElement.Content.Count == 0)
            {
                diagnosticProperties.Add(NoCodeFixKey, string.Empty);
            }

            var textElement = summaryElement.Content.FirstOrDefault() as XmlTextSyntax;
            var text = textElement == null ? string.Empty : XmlCommentHelper.GetText(textElement, true).TrimStart();

            bool startsWithGetOrSet = text.StartsWith(startingTextGetsOrSets, StringComparison.OrdinalIgnoreCase);
            bool getterVisible, setterVisible;
            if (getter != null && setter != null)
            {
                if (!getter.Modifiers.Any() && !setter.Modifiers.Any())
                {
                    // Case 1: The getter and setter have the same declared accessibility
                    getterVisible = true;
                    setterVisible = true;
                }
                else if (getter.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    // Case 3
                    getterVisible = false;
                    setterVisible = true;
                }
                else if (setter.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    // Case 3
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
                        // Case 2: Property only internal and no accessor is explicitly private
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
                            // Case 4
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
                            // Case 4
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
                    if (!startsWithGetOrSet)
                    {
                        diagnosticProperties.Add(ExpectedTextKey, startingTextGetsOrSets);

                        if (text.StartsWith(startingTextGets, StringComparison.OrdinalIgnoreCase))
                        {
                            diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, startingTextGets.Length));
                        }
                        else if (text.StartsWith(startingTextSets, StringComparison.OrdinalIgnoreCase))
                        {
                            diagnosticProperties.Add(TextToRemoveKey, text.Substring(0, startingTextSets.Length));
                        }

                        context.ReportDiagnostic(Diagnostic.Create(SA1623Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), startingTextGetsOrSets));
                    }
                }
                else
                {
                    if (startsWithGetOrSet)
                    {
                        diagnosticProperties.Add(ExpectedTextKey, startingTextGets);
                        diagnosticProperties.Add(TextToRemoveKey, startingTextGetsOrSets);
                        context.ReportDiagnostic(Diagnostic.Create(SA1624Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), "get", startingTextGets));
                    }
                    else if (!text.StartsWith(startingTextGets, StringComparison.OrdinalIgnoreCase))
                    {
                        diagnosticProperties.Add(ExpectedTextKey, startingTextGets);
                        context.ReportDiagnostic(Diagnostic.Create(SA1623Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), startingTextGets));
                    }
                }
            }
            else if (setterVisible)
            {
                if (startsWithGetOrSet)
                {
                    diagnosticProperties.Add(ExpectedTextKey, startingTextSets);
                    diagnosticProperties.Add(TextToRemoveKey, startingTextGetsOrSets);
                    context.ReportDiagnostic(Diagnostic.Create(SA1624Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), "set", startingTextSets));
                }
                else if (!text.StartsWith(startingTextSets, StringComparison.OrdinalIgnoreCase))
                {
                    diagnosticProperties.Add(ExpectedTextKey, startingTextSets);
                    context.ReportDiagnostic(Diagnostic.Create(SA1623Descriptor, diagnosticLocation, diagnosticProperties.ToImmutable(), startingTextSets));
                }
            }
        }
    }
}
