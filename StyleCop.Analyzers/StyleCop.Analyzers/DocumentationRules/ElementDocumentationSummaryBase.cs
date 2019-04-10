// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;summary&gt;</c> text of a documentation comment.
    /// </summary>
    internal abstract class ElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> typeDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> methodDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> constructorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> destructorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> propertyDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> indexerDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> fieldDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> delegateDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> eventDeclarationAction;

        protected ElementDocumentationSummaryBase()
        {
            this.typeDeclarationAction = this.HandleTypeDeclaration;
            this.methodDeclarationAction = this.HandleMethodDeclaration;
            this.constructorDeclarationAction = this.HandleConstructorDeclaration;
            this.destructorDeclarationAction = this.HandleDestructorDeclaration;
            this.propertyDeclarationAction = this.HandlePropertyDeclaration;
            this.indexerDeclarationAction = this.HandleIndexerDeclaration;
            this.fieldDeclarationAction = this.HandleFieldDeclaration;
            this.delegateDeclarationAction = this.HandleDelegateDeclaration;
            this.eventDeclarationAction = this.HandleEventDeclaration;
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(this.typeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
            context.RegisterSyntaxNodeAction(this.methodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(this.constructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(this.destructorDeclarationAction, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(this.propertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(this.indexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(this.fieldDeclarationAction, SyntaxKinds.BaseFieldDeclaration);
            context.RegisterSyntaxNodeAction(this.delegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(this.eventDeclarationAction, SyntaxKind.EventDeclaration);
        }

        /// <summary>
        /// Analyzes the top-level <c>&lt;summary&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="needsComment"><see langword="true"/> if the current documentation settings indicate that the
        /// element should be documented; otherwise, <see langword="false"/>.</param>
        /// <param name="documentation">The documentation syntax associated with the element.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="completeDocumentation">The complete documentation for the declared symbol, with any
        /// <c>&lt;include&gt;</c> elements expanded. If the XML documentation comment included a <c>&lt;summary&gt;</c>
        /// element, this value will be <see langword="null"/>, even if the XML documentation comment also included an
        /// <c>&lt;include&gt;</c> element.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, DocumentationCommentTriviaSyntax documentation, XmlNodeSyntax syntax, XElement completeDocumentation, params Location[] diagnosticLocations);

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (BaseTypeDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // partial elements are handled by PartialElementDocumentationSummaryBase
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (DelegateDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (MethodDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // partial elements are handled by PartialElementDocumentationSummaryBase
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
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
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (DestructorDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (PropertyDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
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
            this.HandleDeclaration(context, needsComment, node, node.ThisKeyword.GetLocation());
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (BaseFieldDeclarationSyntax)context.Node;
            if (node.Declaration == null)
            {
                return;
            }

            Location[] locations = new Location[node.Declaration.Variables.Count];

            int insertionIndex = 0;

            foreach (var variable in node.Declaration.Variables)
            {
                var identifier = variable.Identifier;
                if (!identifier.IsMissing)
                {
                    locations[insertionIndex++] = identifier.GetLocation();
                }
            }

            // PERF: Most of the time locations will have the correct size.
            // The only case where it might be smaller is in invalid syntax like
            // int i,;
            Array.Resize(ref locations, insertionIndex);

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, locations);
        }

        private void HandleEventDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var node = (EventDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
            Accessibility effectiveAccessibility = node.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
            bool needsComment = SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, node.Kind(), node.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
            this.HandleDeclaration(context, needsComment, node, node.Identifier.GetLocation());
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, bool needsComment, SyntaxNode node, params Location[] locations)
        {
            var documentation = node.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                // missing documentation is reported by SA1600, SA1601, and SA1602
                return;
            }

            XElement completeDocumentation = null;
            var relevantXmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag);
            if (relevantXmlElement == null)
            {
                relevantXmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag);
                if (relevantXmlElement != null)
                {
                    var declaration = context.SemanticModel.GetDeclaredSymbol(node, context.CancellationToken);
                    if (declaration == null)
                    {
                        return;
                    }

                    var rawDocumentation = declaration.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                    completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);
                }
            }

            this.HandleXmlElement(context, needsComment, documentation, relevantXmlElement, completeDocumentation, locations);
        }
    }
}
