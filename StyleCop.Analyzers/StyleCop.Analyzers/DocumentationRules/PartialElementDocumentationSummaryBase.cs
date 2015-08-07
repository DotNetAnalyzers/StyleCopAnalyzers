namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;summary&gt;</c> or <c>&lt;content&gt;</c> text of
    /// the documentation comment associated with a <c>partial</c> element.
    /// </summary>
    public abstract class PartialElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        /// <summary>
        /// Analyzes the top-level <c>&lt;summary&gt;</c> or <c>&lt;content&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, params Location[] diagnosticLocations);

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseTypeDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
            {
                return;
            }

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // non-elements are handled by ElementDocumentationSummaryBase
                return;
            }

            this.HandleDeclaration(context, node, node.Identifier.GetLocation());
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
            {
                return;
            }

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // non-partial elements are handled by ElementDocumentationSummaryBase
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

            var xmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag);
            if (xmlElement == null)
            {
                xmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.ContentXmlTag);
            }

            this.HandleXmlElement(context, xmlElement, locations);
        }
    }
}
