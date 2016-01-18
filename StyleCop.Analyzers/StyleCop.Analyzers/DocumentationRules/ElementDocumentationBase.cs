// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;param&gt;</c> text of a documentation comment on an element declaration.
    /// </summary>
    internal abstract class ElementDocumentationBase : DiagnosticAnalyzer
    {
        private readonly string matchElementName;
        private readonly bool inheritDocSuppressesWarnings;

        private readonly Action<CompilationStartAnalysisContext> compilationStartAction;
        private readonly Action<SyntaxNodeAnalysisContext> methodDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> constructorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> delegateDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> indexerDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> operatorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> conversionOperatorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> classDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> structDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> enumDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> fieldDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> propertyDeclarationAction;

        protected ElementDocumentationBase(bool inheritDocSuppressesWarnings, string matchElementName = null)
        {
            this.matchElementName = matchElementName;
            this.inheritDocSuppressesWarnings = inheritDocSuppressesWarnings;

            this.compilationStartAction = this.HandleCompilationStart;
            this.methodDeclarationAction = this.HandleMethodDeclaration;
            this.constructorDeclarationAction = this.HandleConstructorDeclaration;
            this.delegateDeclarationAction = this.HandleDelegateDeclaration;
            this.indexerDeclarationAction = this.HandleIndexerDeclaration;
            this.operatorDeclarationAction = this.HandleOperatorDeclaration;
            this.conversionOperatorDeclarationAction = this.HandleConversionOperatorDeclaration;
            this.classDeclarationAction = this.HandleClassDeclaration;
            this.structDeclarationAction = this.HandleStructDeclaration;
            this.enumDeclarationAction = this.HandleEnumDeclaration;
            this.fieldDeclarationAction = this.HandleFieldDeclaration;
            this.propertyDeclarationAction = this.HandlePropertyDeclaration;
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(this.compilationStartAction);
        }

        /// <summary>
        /// Analyzes the top-level <c>&lt;param&gt;</c> elements of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="syntaxList">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations);

        /// <summary>
        /// Analyzes the top-level <c>&lt;param&gt;</c> elements of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="completeDocumentation">The complete documentation for the declared symbol, with any
        /// <c>&lt;include&gt;</c> elements expanded. If the XML documentation comment included a <c>&lt;param&gt;</c>
        /// element, this value will be <see langword="null"/>, even if the XML documentation comment also included an
        /// <c>&lt;include&gt;</c> element.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, XElement completeDocumentation, params Location[] diagnosticLocations);

        private void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.methodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.constructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.delegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.indexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.operatorDeclarationAction, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.conversionOperatorDeclarationAction, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.classDeclarationAction, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.structDeclarationAction, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.enumDeclarationAction, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.fieldDeclarationAction, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.propertyDeclarationAction, SyntaxKind.PropertyDeclaration);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (MethodDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (ConstructorDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (DelegateDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (IndexerDeclarationSyntax)context.Node;
            if (node.ThisKeyword.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.ThisKeyword.GetLocation());
        }

        private void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (OperatorDeclarationSyntax)context.Node;
            if (node.OperatorToken.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.OperatorToken.GetLocation());
        }

        private void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (ConversionOperatorDeclarationSyntax)context.Node;

            this.HandleDeclaration(context, node, node.GetLocation());
        }

        private void HandleClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (ClassDeclarationSyntax)context.Node;

            this.HandleDeclaration(context, node, node.GetLocation());
        }

        private void HandleStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (StructDeclarationSyntax)context.Node;

            this.HandleDeclaration(context, node, node.GetLocation());
        }

        private void HandleEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (EnumDeclarationSyntax)context.Node;

            this.HandleDeclaration(context, node, node.GetLocation());
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (FieldDeclarationSyntax)context.Node;

            this.HandleDeclaration(context, node, node.GetLocation());
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (PropertyDeclarationSyntax)context.Node;

            this.HandleDeclaration(context, node, node.GetLocation());
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, SyntaxNode node, params Location[] locations)
        {
            var documentation = node.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                // missing documentation is reported by SA1600, SA1601, and SA1602
                return;
            }

            if (this.inheritDocSuppressesWarnings
                && documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            IEnumerable<XmlNodeSyntax> matchingXmlElements = string.IsNullOrEmpty(this.matchElementName)
                ? documentation.Content
                    .Where(x => x is XmlElementSyntax || x is XmlEmptyElementSyntax)
                    .Where(x => !string.Equals(GetName(x)?.ToString(), XmlCommentHelper.IncludeXmlTag))
                : documentation.Content.GetXmlElements(this.matchElementName);

            if (!matchingXmlElements.Any())
            {
                var includedDocumentation = documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag);
                if (includedDocumentation != null)
                {
                    var declaration = context.SemanticModel.GetDeclaredSymbol(node, context.CancellationToken);
                    var rawDocumentation = declaration?.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                    var completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);

                    if (this.inheritDocSuppressesWarnings &&
                        completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                    {
                        // Ignore nodes with an <inheritdoc/> tag in the included XML.
                        return;
                    }

                    this.HandleCompleteDocumentation(context, completeDocumentation, locations);
                    return; // done
                }
            }

            this.HandleXmlElement(context, matchingXmlElements, locations);
        }

        private static XmlNameSyntax GetName(XmlNodeSyntax element)
        {
            return (element as XmlElementSyntax)?.StartTag?.Name
                ?? (element as XmlEmptyElementSyntax)?.Name;
        }
    }
}
