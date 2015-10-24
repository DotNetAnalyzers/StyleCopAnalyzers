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
    /// The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not
    /// match the actual parameters on the element.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the documentation for an element's parameters does not match the actual
    /// parameters on the element, or if the parameter documentation is not listed in the same order as the element's parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1612ElementParameterDocumentationMustMatchElementParameters : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1612";
        private const string Title = "Element parameter documentation must match element parameters";
        private const string Description = "The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not match the actual parameters on the element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1612.md";

        private const string MissingParamForDocumentationMessageFormat = "The parameter '{0}' does not exist.";
        private const string ParamWrongOrderMessageFormat = "The parameter documentation for '{0}' should be at position {1}.";

        private static readonly DiagnosticDescriptor MissingParameterDescriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MissingParamForDocumentationMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor OrderDescriptor =
                   new DiagnosticDescriptor(DiagnosticId, Title, ParamWrongOrderMessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DocumentationTriviaAction = HandleDocumentationTrivia;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(MissingParameterDescriptor);

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
            IEnumerable<string> parentParametersEnumerable = GetParentParameters(syntax);

            if (parentParametersEnumerable == null)
            {
                return;
            }

            ImmutableArray<string> parentParameters = parentParametersEnumerable.ToImmutableArray();

            ImmutableArray<XmlNodeSyntax> nodes = syntax.Content
                .Where(node => string.Equals(GetName(node)?.ToString(), XmlCommentHelper.ParamXmlTag))
                .ToImmutableArray();

            for (int i = 0; i < nodes.Length; i++)
            {
                HandleElement(context, nodes[i], parentParameters, i, GetName(nodes[i])?.GetLocation());
            }
        }

        private static XmlNameSyntax GetName(XmlNodeSyntax element)
        {
            return (element as XmlElementSyntax)?.StartTag?.Name
                ?? (element as XmlEmptyElementSyntax)?.Name;
        }

        private static void HandleElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax element, ImmutableArray<string> parentParameters, int index, Location alternativeDiagnosticLocation)
        {
            var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);

            // Make sure we ignore violations that should be reported by SA1613 instead.
            if (string.IsNullOrWhiteSpace(nameAttribute?.Identifier?.Identifier.ValueText))
            {
                return;
            }

            if (!parentParameters.Contains(nameAttribute.Identifier.Identifier.ValueText))
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingParameterDescriptor, nameAttribute?.Identifier?.GetLocation() ?? alternativeDiagnosticLocation, nameAttribute.Identifier.Identifier.ValueText));
            }
            else if (parentParameters.Length <= index || parentParameters[index] != nameAttribute.Identifier.Identifier.ValueText)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        OrderDescriptor,
                        nameAttribute?.Identifier?.GetLocation() ?? alternativeDiagnosticLocation,
                        nameAttribute.Identifier.Identifier.ValueText,
                        parentParameters.IndexOf(nameAttribute.Identifier.Identifier.ValueText) + 1));
            }
        }

        /// <summary>
        /// Checks if the given <see cref="SyntaxNode"/> has a <see cref="BaseMethodDeclarationSyntax"/>, <see cref="IndexerDeclarationSyntax"/> or a <see cref="DelegateDeclarationSyntax"/>
        /// as one of its parent. If it finds one of those three with a valid type parameter list it returns a <see cref="IEnumerable{T}"/> containing the names of all parameters.
        /// </summary>
        /// <param name="node">The node the analysis should start at.</param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> containing all parameters or null, of no valid parent could be found.
        /// </returns>
        private static IEnumerable<string> GetParentParameters(SyntaxNode node)
        {
            var methodParent = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodParent != null)
            {
                return methodParent.ParameterList?.Parameters.Select(x => x.Identifier.ValueText) ?? Enumerable.Empty<string>();
            }

            var delegateParent = node.FirstAncestorOrSelf<DelegateDeclarationSyntax>();
            if (delegateParent != null)
            {
                return delegateParent.ParameterList?.Parameters.Select(x => x.Identifier.ValueText) ?? Enumerable.Empty<string>();
            }

            var indexerParent = node.FirstAncestorOrSelf<IndexerDeclarationSyntax>();
            if (indexerParent != null)
            {
                return indexerParent.ParameterList?.Parameters.Select(x => x.Identifier.ValueText) ?? Enumerable.Empty<string>();
            }

            return null;
        }
    }
}
