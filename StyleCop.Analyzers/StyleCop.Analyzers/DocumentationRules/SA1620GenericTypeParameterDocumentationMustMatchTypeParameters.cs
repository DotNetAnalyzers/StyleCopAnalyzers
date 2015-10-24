// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
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

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DocumentationTriviaAction = HandleDocumentationTrivia;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(MissingTypeParameterDescriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(DocumentationTriviaAction, SyntaxKind.SingleLineDocumentationCommentTrivia);
        }

        private static void HandleDocumentationTrivia(SyntaxNodeAnalysisContext context)
        {
            DocumentationCommentTriviaSyntax syntax = context.Node as DocumentationCommentTriviaSyntax;

            // Find the type parameters of the parent node
            IEnumerable<string> parentTypeParametersEnumerable = GetParentTypeParameters(syntax);

            if (parentTypeParametersEnumerable == null)
            {
                return;
            }

            ImmutableArray<string> parentTypeParameters = parentTypeParametersEnumerable.ToImmutableArray();

            ImmutableArray<XmlNodeSyntax> nodes = syntax.Content
                .Where(node => string.Equals(GetName(node)?.ToString(), XmlCommentHelper.TypeParamXmlTag))
                .ToImmutableArray();

            for (int i = 0; i < nodes.Length; i++)
            {
                HandleElement(context, nodes[i], parentTypeParameters, i, GetName(nodes[i])?.GetLocation());
            }
        }

        private static XmlNameSyntax GetName(XmlNodeSyntax element)
        {
            return (element as XmlElementSyntax)?.StartTag?.Name
                ?? (element as XmlEmptyElementSyntax)?.Name;
        }

        private static void HandleElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax element, ImmutableArray<string> parentTypeParameters, int index, Location alternativeDiagnosticLocation)
        {
            var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);

            // Make sure we ignore violations that should be reported by SA1613 instead.
            if (string.IsNullOrWhiteSpace(nameAttribute?.Identifier?.Identifier.ValueText))
            {
                return;
            }

            if (!parentTypeParameters.Contains(nameAttribute.Identifier.Identifier.ValueText))
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingTypeParameterDescriptor, nameAttribute?.Identifier?.GetLocation() ?? alternativeDiagnosticLocation, nameAttribute.Identifier.Identifier.ValueText));
            }
            else if (parentTypeParameters.Length <= index || parentTypeParameters[index] != nameAttribute.Identifier.Identifier.ValueText)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        OrderDescriptor,
                        nameAttribute?.Identifier?.GetLocation() ?? alternativeDiagnosticLocation,
                        nameAttribute.Identifier.Identifier.ValueText,
                        parentTypeParameters.IndexOf(nameAttribute.Identifier.Identifier.ValueText) + 1));
            }
        }

        /// <summary>
        /// Checks if the given <see cref="SyntaxNode"/> has a <see cref="MethodDeclarationSyntax"/>, a <see cref="DelegateDeclarationSyntax"/> or a <see cref="TypeDeclarationSyntax"/>
        /// as one of its parent. If it finds one of those three with a valid type parameter list it returns a <see cref="IEnumerable{T}"/> containing the names of all type parameters.
        /// </summary>
        /// <param name="node">The node the analysis should start at.</param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> containing all type parameters or null, of no valid parent could be found.
        /// </returns>
        private static IEnumerable<string> GetParentTypeParameters(SyntaxNode node)
        {
            var methodParent = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodParent != null)
            {
                return methodParent.TypeParameterList?.Parameters.Select(x => x.Identifier.ValueText) ?? Enumerable.Empty<string>();
            }

            var delegateParent = node.FirstAncestorOrSelf<DelegateDeclarationSyntax>();
            if (delegateParent != null)
            {
                return delegateParent.TypeParameterList?.Parameters.Select(x => x.Identifier.ValueText) ?? Enumerable.Empty<string>();
            }

            var typeParent = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            if (typeParent != null)
            {
                return typeParent.TypeParameterList?.Parameters.Select(x => x.Identifier.ValueText) ?? Enumerable.Empty<string>();
            }

            return null;
        }
    }
}
