// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;param&gt;</c> text of a documentation comment on an element declaration.
    /// </summary>
    internal abstract class ElementDocumentationBase : DiagnosticAnalyzer
    {
        private readonly string matchElementName;
        private readonly bool inheritDocSuppressesWarnings;

        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> methodDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> constructorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> delegateDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> indexerDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> operatorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> conversionOperatorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> baseTypeDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> fieldDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> propertyDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> enumMemberDeclarationAction;

        protected ElementDocumentationBase(bool inheritDocSuppressesWarnings, string matchElementName = null)
        {
            this.matchElementName = matchElementName;
            this.inheritDocSuppressesWarnings = inheritDocSuppressesWarnings;

            this.methodDeclarationAction = this.HandleMethodDeclaration;
            this.constructorDeclarationAction = this.HandleConstructorDeclaration;
            this.delegateDeclarationAction = this.HandleDelegateDeclaration;
            this.indexerDeclarationAction = this.HandleIndexerDeclaration;
            this.operatorDeclarationAction = this.HandleOperatorDeclaration;
            this.conversionOperatorDeclarationAction = this.HandleConversionOperatorDeclaration;
            this.baseTypeDeclarationAction = this.HandleBaseTypeDeclaration;
            this.fieldDeclarationAction = this.HandleFieldDeclaration;
            this.propertyDeclarationAction = this.HandlePropertyDeclaration;
            this.enumMemberDeclarationAction = this.HandleEnumMemberDeclaration;
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(this.methodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(this.constructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(this.delegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(this.indexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(this.operatorDeclarationAction, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(this.conversionOperatorDeclarationAction, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(this.baseTypeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
            context.RegisterSyntaxNodeAction(this.fieldDeclarationAction, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(this.propertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(this.enumMemberDeclarationAction, SyntaxKind.EnumMemberDeclaration);
        }

        /// <summary>
        /// Analyzes the top-level elements of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="settings">The StyleCop settings to use.</param>
        /// <param name="needsComment"><see langword="true"/> if the current documentation settings indicate that the
        /// element should be documented; otherwise, <see langword="false"/>.</param>
        /// <param name="syntaxList">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations);

        /// <summary>
        /// Analyzes the XML elements of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="needsComment"><see langword="true"/> if the current documentation settings indicate that the
        /// element should be documented; otherwise, <see langword="false"/>.</param>
        /// <param name="completeDocumentation">The complete documentation for the declared symbol, with any
        /// <c>&lt;include&gt;</c> elements expanded. If the XML documentation comment included a <c>&lt;param&gt;</c>
        /// element, this value will be <see langword="null"/>, even if the XML documentation comment also included an
        /// <c>&lt;include&gt;</c> element.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, bool needsComment, XElement completeDocumentation, params Location[] diagnosticLocations);

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (MethodDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (ConstructorDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (DelegateDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (IndexerDeclarationSyntax)context.Node;
            if (node.ThisKeyword.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.ThisKeyword.GetLocation());
        }

        private void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (OperatorDeclarationSyntax)context.Node;
            if (node.OperatorToken.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.OperatorToken.GetLocation());
        }

        private void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (ConversionOperatorDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.GetLocation());
        }

        private void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (BaseTypeDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (FieldDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Declaration.GetLocation());
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (PropertyDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleEnumMemberDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (EnumMemberDeclarationSyntax)context.Node;

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility();
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, settings, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings, bool needsComment, SyntaxNode node, params Location[] locations)
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

            var hasIncludedDocumentation =
                documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag) is object;

            if (hasIncludedDocumentation)
            {
                var declaration = node switch
                {
                    BaseFieldDeclarationSyntax baseFieldDeclaration => baseFieldDeclaration.Declaration.Variables.FirstOrDefault() ?? node,
                    _ => node,
                };

                var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);
                if (declaredSymbol is not null)
                {
                    var rawDocumentation = declaredSymbol?.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                    var completeDocumentation = XElement.Parse(rawDocumentation ?? "<doc></doc>", LoadOptions.None);

                    if (this.inheritDocSuppressesWarnings &&
                        completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                    {
                        // Ignore nodes with an <inheritdoc/> tag in the included XML.
                        return;
                    }

                    this.HandleCompleteDocumentation(context, needsComment, completeDocumentation, locations);
                    return;
                }
            }

            IEnumerable<XmlNodeSyntax> matchingXmlElements = string.IsNullOrEmpty(this.matchElementName)
                ? documentation.Content
                    .Where(x => x is XmlElementSyntax || x is XmlEmptyElementSyntax)
                    .Where(x => !string.Equals(x.GetName()?.ToString(), XmlCommentHelper.IncludeXmlTag, StringComparison.Ordinal))
                : documentation.Content.GetXmlElements(this.matchElementName);

            this.HandleXmlElement(context, settings, needsComment, matchingXmlElements, locations);
        }
    }
}
