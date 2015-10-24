// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;summary&gt;</c> text of a documentation comment.
    /// </summary>
    internal abstract class ElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        private static readonly ImmutableArray<SyntaxKind> BaseTypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration);

        private static readonly ImmutableArray<SyntaxKind> BaseFieldDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration);

        private readonly Action<CompilationStartAnalysisContext> compilationStartAction;
        private readonly Action<SyntaxNodeAnalysisContext> typeDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> methodDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> constructorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> destructorDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> propertyDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> indexerDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> fieldDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> delegateDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext> eventDeclarationAction;

        protected ElementDocumentationSummaryBase()
        {
            this.compilationStartAction = this.HandleCompilationStart;
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
            context.RegisterCompilationStartAction(this.compilationStartAction);
        }

        /// <summary>
        /// Analyzes the top-level <c>&lt;summary&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, params Location[] diagnosticLocations);

        private void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.typeDeclarationAction, BaseTypeDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(this.methodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.constructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.destructorDeclarationAction, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.propertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.indexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.fieldDeclarationAction, BaseFieldDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(this.delegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.eventDeclarationAction, SyntaxKind.EventDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
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

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
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

        private void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (DestructorDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
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

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (IndexerDeclarationSyntax)context.Node;
            if (node.ThisKeyword.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.ThisKeyword.GetLocation());
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
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

            this.HandleDeclaration(context, node, locations);
        }

        private void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (EventDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, SyntaxNode node, params Location[] locations)
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

            var summaryXmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag);
            this.HandleXmlElement(context, summaryXmlElement, locations);
        }
    }
}
