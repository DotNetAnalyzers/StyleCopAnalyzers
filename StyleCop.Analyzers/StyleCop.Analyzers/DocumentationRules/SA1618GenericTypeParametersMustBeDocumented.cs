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
    /// A generic C# element is missing documentation for one or more of its generic type parameters.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing generic type parameters is missing documentation
    /// for one or more of its generic type parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1618GenericTypeParametersMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1618GenericTypeParametersMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1618";
        private const string Title = "Generic type parameters should be documented";
        private const string MessageFormat = "The documentation for type parameter '{0}' is missing";
        private const string Description = "A generic C# element is missing documentation for one or more of its generic type parameters.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1618.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;
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

            context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKinds.TypeDeclaration);
            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            TypeDeclarationSyntax typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // This is handled by SA1619
                return;
            }

            Accessibility declaredAccessibility = typeDeclaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = typeDeclaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, typeDeclaration.Kind(), typeDeclaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            HandleMemberDeclaration(context, needsComment, typeDeclaration, typeDeclaration.TypeParameterList);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = methodDeclaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = methodDeclaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, methodDeclaration.Kind(), methodDeclaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            HandleMemberDeclaration(context, needsComment, methodDeclaration, methodDeclaration.TypeParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            DelegateDeclarationSyntax delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = delegateDeclaration.GetDeclaredAccessibility(context.SemanticModel);
            Accessibility effectiveAccessibility = delegateDeclaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, delegateDeclaration.Kind(), delegateDeclaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            HandleMemberDeclaration(context, needsComment, delegateDeclaration, delegateDeclaration.TypeParameterList);
        }

        private static void HandleMemberDeclaration(SyntaxNodeAnalysisContext context, bool needsComment, SyntaxNode node, TypeParameterListSyntax typeParameterList)
        {
            if (!needsComment)
            {
                // Documentation is not required for this element.
                return;
            }

            if (typeParameterList == null)
            {
                // The member does not have a type parameter list
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

            // Check if the return value is documented
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
                if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                {
                    // Ignore nodes with an <inheritdoc/> tag in the included XML.
                    return;
                }

                var typeParameterAttributes = completeDocumentation.Nodes()
                    .OfType<XElement>()
                    .Where(element => element.Name == XmlCommentHelper.TypeParamXmlTag)
                    .Select(element => element.Attribute(XmlCommentHelper.NameArgumentName))
                    .Where(x => x != null);

                foreach (var parameter in typeParameterList.Parameters)
                {
                    if (!typeParameterAttributes.Any(x => x.Value == parameter.Identifier.ValueText))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation(), parameter.Identifier.ValueText));
                    }
                }
            }
            else
            {
                var xmlParameterNames = documentation.Content.GetXmlElements(XmlCommentHelper.TypeParamXmlTag)
                    .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                    .Where(x => x != null)
                    .ToImmutableArray();

                foreach (var parameter in typeParameterList.Parameters)
                {
                    if (!xmlParameterNames.Any(x => x.Identifier.Identifier.ValueText == parameter.Identifier.ValueText))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation(), parameter.Identifier.ValueText));
                    }
                }
            }
        }
    }
}
