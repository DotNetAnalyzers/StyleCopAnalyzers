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
        /// Analyzes the top-level <c>&lt;summary&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocation">The location where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, Location diagnosticLocation);

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

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, SyntaxNode node, Location location)
        {
            var documentation = node.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                // missing documentation is reported by SA1600, SA1601, and SA1602
                return;
            }

            if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            var valueXmlElement = documentation.Content.GetFirstXmlElement(this.XmlTagToHandle);
            this.HandleXmlElement(context, valueXmlElement, location);
        }
    }
}
