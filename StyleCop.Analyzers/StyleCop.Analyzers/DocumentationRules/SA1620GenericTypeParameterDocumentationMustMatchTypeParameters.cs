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
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The <c>&lt;typeparam&gt;</c> tags within the XML header documentation for a generic C# element do not match the
    /// generic type parameters on the element.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the <c>&lt;typeparam&gt;</c> tags within the element's header
    /// documentation do not match the generic type parameters on the element, or do not appear in the same order as the
    /// element's type parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1620GenericTypeParameterDocumentationMustMatchTypeParameters : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1620GenericTypeParameterDocumentationMustMatchTypeParameters"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1620";
        private const string Title = "Generic type parameter documentation must match type parameters";
        private const string Description = "The <typeparam> tags within the Xml header documentation for a generic C# element do not match the generic type parameters on the element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1620.md";

        private const string MissingTypeParamForDocumentationMessageFormat = "The type parameter '{0}' does not exist.";
        private const string TypeParamWrongOrderMessageFormat = "The type parameter documentation for '{0}' should be at position {1}.";

        private static readonly DiagnosticDescriptor MissingTypeParameterDescriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MissingTypeParamForDocumentationMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor OrderDescriptor =
                   new DiagnosticDescriptor(DiagnosticId, Title, TypeParamWrongOrderMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(MissingTypeParameterDescriptor);

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
                rawDocumentation = declaration?.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
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
                    .Where(x => x != null)
                    .ToImmutableArray();

                // Check based on the documented type parameters, as we must detect scenarios where there are too many type parameters documented.
                // It is not necessary to detect missing type parameter documentation, this belongs to SA1618.
                for (var i = 0; i < typeParameterAttributes.Length; i++)
                {
                    var documentedParameterName = typeParameterAttributes[i].Value;
                    HandleTypeParamElement(context, documentedParameterName, i, typeParameterList, includeElement.GetLocation());
                }
            }
            else
            {
                var xmlParameterNames = documentation.Content.GetXmlElements(XmlCommentHelper.TypeParamXmlTag)
                    .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                    .Where(x => x != null)
                    .ToImmutableArray();

                // Check based on the documented type parameters, as we must detect scenarios where there are too many type parameters documented.
                // It is not necessary to detect missing type parameter documentation, this belongs to SA1618.
                for (var i = 0; i < xmlParameterNames.Length; i++)
                {
                    var nameSyntax = xmlParameterNames[i].Identifier;
                    var documentedParameterName = nameSyntax?.Identifier.ValueText;

                    HandleTypeParamElement(context, documentedParameterName, i, typeParameterList, nameSyntax.Identifier.GetLocation());
                }
            }
        }

        private static void HandleTypeParamElement(SyntaxNodeAnalysisContext context, string documentedParameterName, int index, TypeParameterListSyntax typeParameterList, Location locationToReport)
        {
            if (string.IsNullOrWhiteSpace(documentedParameterName))
            {
                // Make sure we ignore violations that should be reported by SA1621 instead.
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
                        OrderDescriptor,
                        locationToReport,
                        documentedParameterName,
                        typeParameterList.Parameters.IndexOf(matchingTypeParameter) + 1));
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        MissingTypeParameterDescriptor,
                        locationToReport,
                        documentedParameterName));
            }
        }
    }
}
