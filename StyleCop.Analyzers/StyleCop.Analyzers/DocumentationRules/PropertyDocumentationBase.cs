// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;value&gt;</c> text of a documentation comment on a property declaration.
    /// </summary>
    internal abstract class PropertyDocumentationBase : DiagnosticAnalyzer
    {
        private readonly Action<CompilationStartAnalysisContext> compilationStartAction;
        private readonly Action<SyntaxNodeAnalysisContext> propertyDeclarationAction;

        protected PropertyDocumentationBase()
        {
            this.compilationStartAction = this.HandleCompilationStart;
            this.propertyDeclarationAction = this.HandlePropertyDeclaration;
        }

        /// <summary>
        /// Gets the XML tag within the property documentation that should be handled.
        /// </summary>
        /// <value>The XML tag to handle.</value>
        protected abstract string XmlTagToHandle { get; }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(this.compilationStartAction);
        }

        /// <summary>
        /// Checks if the given property declaration has a valid (allowed) &lt;inheritdoc/&gt; declaration.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="propertyDeclaration">The property declaration to check.</param>
        /// <param name="documentation">The documentation for the property.</param>
        /// <returns>True if a valid &lt;inheritdoc/&gt; declaration is present.</returns>
        protected static bool HasValidInheritDoc(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration, DocumentationCommentTriviaSyntax documentation)
        {
            return documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null
                && CanUseInheritDoc(context, propertyDeclaration);
        }

        /// <summary>
        /// Analyzes the top-level element (of type <see cref="XmlTagToHandle"/>) of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="propertyDeclaration">The property declaration to check.</param>
        /// <param name="documentation">The documentation for the property.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocation">The location where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration, DocumentationCommentTriviaSyntax documentation, XmlNodeSyntax syntax, Location diagnosticLocation);

        private static bool CanUseInheritDoc(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            var symbolInfo = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);
            return symbolInfo.IsOverride || NamedTypeHelpers.IsImplementingAnInterfaceMember(symbolInfo);
        }

        private void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.propertyDeclarationAction, SyntaxKind.PropertyDeclaration);
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (PropertyDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration, Location location)
        {
            var documentation = propertyDeclaration.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                // missing documentation is reported by SA1600, SA1601, and SA1602
                return;
            }

            var valueXmlElement = documentation.Content.GetFirstXmlElement(this.XmlTagToHandle);
            this.HandleXmlElement(context, propertyDeclaration, documentation, valueXmlElement, location);
        }
    }
}
