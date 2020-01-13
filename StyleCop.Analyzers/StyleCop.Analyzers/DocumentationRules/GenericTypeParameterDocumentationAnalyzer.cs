// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Analyzer that covers generic typeparam documentation checks. This currently includes SA1620, SA1621, and SA1622.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class GenericTypeParameterDocumentationAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string SA1620DiagnosticId = "SA1620";
        private static readonly LocalizableString SA1620Title = new LocalizableResourceString(nameof(DocumentationResources.SA1620Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1620MissingMessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1620MissingMessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1620WrongOrderMessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1620WrongOrderMessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1620Description = new LocalizableResourceString(nameof(DocumentationResources.SA1620Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1620HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1620.md";

        private static readonly string SA1621DiagnosticId = "SA1621";
        private static readonly LocalizableString SA1621Title = new LocalizableResourceString(nameof(DocumentationResources.SA1621Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1621MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1621MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1621Description = new LocalizableResourceString(nameof(DocumentationResources.SA1621Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1621HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1621.md";

        private static readonly string SA1622DiagnosticId = "SA1622";
        private static readonly LocalizableString SA1622Title = new LocalizableResourceString(nameof(DocumentationResources.SA1622Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1622MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1622MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1622Description = new LocalizableResourceString(nameof(DocumentationResources.SA1622Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1622HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1622.md";

        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;

        /// <summary>
        /// Gets the descriptor for SA1620, where the typeparam tag is missing.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1620.</value>
        public static DiagnosticDescriptor SA1620MissingTypeParameterDescriptor { get; } =
            new DiagnosticDescriptor(SA1620DiagnosticId, SA1620Title, SA1620MissingMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1620Description, SA1620HelpLink);

        /// <summary>
        /// Gets the descriptor for SA1620, where the typeparam tags is not ordered correctly.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1620.</value>
        public static DiagnosticDescriptor SA1620WrongOrderDescriptor { get; } =
                   new DiagnosticDescriptor(SA1620DiagnosticId, SA1620Title, SA1620WrongOrderMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1620Description, SA1620HelpLink);

        /// <summary>
        /// Gets the descriptor for SA1621.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1621.</value>
        public static DiagnosticDescriptor SA1621Descriptor { get; } =
            new DiagnosticDescriptor(SA1621DiagnosticId, SA1621Title, SA1621MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1621Description, SA1621HelpLink);

        /// <summary>
        /// Gets the descriptor for SA1622.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1621.</value>
        public static DiagnosticDescriptor SA1622Descriptor { get; } =
            new DiagnosticDescriptor(SA1622DiagnosticId, SA1622Title, SA1622MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1622Description, SA1622HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(SA1620MissingTypeParameterDescriptor, SA1621Descriptor, SA1622Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;
            HandleDeclaration(context, typeDeclaration, typeDeclaration.TypeParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleDeclaration(context, delegateDeclaration, delegateDeclaration.TypeParameterList);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleDeclaration(context, methodDeclaration, methodDeclaration.TypeParameterList);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context, SyntaxNode node, TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList == null)
            {
                // The node does not have a type parameter list
                return;
            }

            var documentation = node.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                // Don't report if the documentation is missing
                return;
            }

            if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            var includeElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag);
            if (includeElement != null)
            {
                string rawDocumentation;
                var declaration = context.SemanticModel.GetDeclaredSymbol(context.Node, context.CancellationToken);
                if (declaration == null)
                {
                    return;
                }

                rawDocumentation = declaration.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                var completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ExcludeXmlTag))
                {
                    // Ignore nodes with an <exclude /> tag in the included XML
                    return;
                }

                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                {
                    // Ignore nodes with an <inheritdoc/> tag in the included XML.
                    return;
                }

                var typeParameterAttributes = completeDocumentation.Nodes()
                    .OfType<XElement>()
                    .Where(element => element.Name == XmlCommentHelper.TypeParamXmlTag)
                    .ToImmutableArray();

                // Check based on the documented type parameters, as we must detect scenarios where there are too many type parameters documented.
                // It is not necessary to detect missing type parameter documentation, this belongs to SA1618.
                for (var i = 0; i < typeParameterAttributes.Length; i++)
                {
                    var documentedParameterName = typeParameterAttributes[i].Attribute(XmlCommentHelper.NameArgumentName)?.Value;
                    HandleTypeParamElement(context, documentedParameterName, i, typeParameterList, includeElement.GetLocation());

                    if (XmlCommentHelper.IsConsideredEmpty(typeParameterAttributes[i]))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                SA1622Descriptor,
                                includeElement.GetLocation()));
                    }
                }
            }
            else
            {
                var typeParameterTags = documentation.Content.GetXmlElements(XmlCommentHelper.TypeParamXmlTag)
                    .ToImmutableArray();

                // Check based on the documented type parameters, as we must detect scenarios where there are too many type parameters documented.
                // It is not necessary to detect missing type parameter documentation, this belongs to SA1618.
                for (var i = 0; i < typeParameterTags.Length; i++)
                {
                    var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(typeParameterTags[i]);
                    var documentedParameterName = nameAttribute?.Identifier?.Identifier.ValueText;

                    var location = nameAttribute?.Identifier?.GetLocation() ?? typeParameterTags[i].GetLocation();
                    HandleTypeParamElement(context, documentedParameterName, i, typeParameterList, location);

                    if (XmlCommentHelper.IsConsideredEmpty(typeParameterTags[i], true))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                SA1622Descriptor,
                                typeParameterTags[i].GetLocation()));
                    }
                }
            }
        }

        private static void HandleTypeParamElement(SyntaxNodeAnalysisContext context, string documentedParameterName, int index, TypeParameterListSyntax typeParameterList, Location locationToReport)
        {
            if (string.IsNullOrWhiteSpace(documentedParameterName))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SA1621Descriptor,
                        locationToReport));

                return;
            }

            var typeParameterName = (index < typeParameterList.Parameters.Count) ? typeParameterList.Parameters[index].Identifier.ValueText : null;
            if (string.Equals(typeParameterName, documentedParameterName, StringComparison.Ordinal))
            {
                return;
            }

            var matchingTypeParameter = typeParameterList.Parameters.FirstOrDefault(tp => string.Equals(tp.Identifier.ValueText, documentedParameterName, StringComparison.Ordinal));
            if (matchingTypeParameter != null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SA1620WrongOrderDescriptor,
                        locationToReport,
                        documentedParameterName,
                        typeParameterList.Parameters.IndexOf(matchingTypeParameter) + 1));
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SA1620MissingTypeParameterDescriptor,
                        locationToReport,
                        documentedParameterName));
            }
        }
    }
}
