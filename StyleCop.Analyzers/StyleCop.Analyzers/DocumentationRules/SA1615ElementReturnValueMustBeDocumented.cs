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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A C# element is missing documentation for its return value.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing a return value is missing a
    /// <c>&lt;returns&gt;</c> tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1615ElementReturnValueMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1615ElementReturnValueMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1615";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1615.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1615Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1615MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1615Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> DelegateDeclarationAction = HandleDelegateDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (MethodDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            HandleDeclaration(context, needsComment, node.ReturnType);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (DelegateDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            HandleDeclaration(context, needsComment, node.ReturnType);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context, bool needsComment, TypeSyntax returnType)
        {
            if (!needsComment)
            {
                // Documentation is optional for this element.
                return;
            }

            if (returnType is PredefinedTypeSyntax predefinedType
                && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                // There is no return value
                return;
            }

            var documentationStructure = context.Node.GetDocumentationCommentTriviaSyntax();

            if (documentationStructure == null)
            {
                return;
            }

            if (documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Don't report if the documentation is inherited.
                return;
            }

            var relevantXmlElement = documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.ReturnsXmlTag);
            if (relevantXmlElement != null)
            {
                // A <returns> element was located.
                return;
            }

            relevantXmlElement = documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag);
            if (relevantXmlElement != null)
            {
                var declaration = context.SemanticModel.GetDeclaredSymbol(context.Node, context.CancellationToken);
                var rawDocumentation = declaration?.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                XElement completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                {
                    // Ignore nodes with an <inheritdoc/> tag in the included XML.
                    return;
                }

                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ReturnsXmlTag))
                {
                    // A <returns> element was located.
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, returnType.GetLocation()));
        }
    }
}
