namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;summary&gt;</c> text of a documentation comment.
    /// </summary>
    public abstract class ElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        /// <summary>
        /// Analyzes the top-level <c>&lt;summary&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, params Location[] diagnosticLocations);

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

            var locations =
                from variable in node.Declaration.Variables
                let identifier = variable.Identifier
                where !identifier.IsMissing
                select identifier.GetLocation();

            this.HandleDeclaration(context, node, locations.ToArray());
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
