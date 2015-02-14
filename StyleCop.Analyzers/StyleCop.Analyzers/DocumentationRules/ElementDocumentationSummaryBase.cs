using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;

namespace StyleCop.Analyzers.DocumentationRules
{
    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;summary&gt;</c> text of a documentation comment.
    /// </summary>
    public abstract class ElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        /// <summary>
        /// Analyzes the top-level <c>&lt;summary&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        abstract protected void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, params Location[] diagnosticLocations);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseTypeDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // partial elements are handled by PartialElementDocumentationSummaryBase
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as DelegateDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // partial elements are handled by PartialElementDocumentationSummaryBase
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ConstructorDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as DestructorDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as PropertyDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as IndexerDeclarationSyntax;
            if (node == null || node.ThisKeyword.IsMissing)
                return;

            this.HandleDeclaration(context, node, node.ThisKeyword.GetLocation());
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseFieldDeclarationSyntax;
            if (node == null || node.Declaration == null)
                return;

            var locations =
                from variable in node.Declaration.Variables
                let identifier = variable.Identifier
                where !identifier.IsMissing
                select identifier.GetLocation();

            this.HandleDeclaration(context, node, locations.ToArray());
        }

        private void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as EventDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, SyntaxNode node, params Location[] locations)
        {
            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation == null)
            {
                // missing documentation is reported by SA1600, SA1601, and SA1602
                return;
            }

            if (XmlCommentHelper.GetTopLevelElement(documentation, XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            var summaryXmlElement = XmlCommentHelper.GetTopLevelElement(documentation, XmlCommentHelper.SummaryXmlTag);
            this.HandleXmlElement(context, summaryXmlElement, locations);
        }
    }
}
