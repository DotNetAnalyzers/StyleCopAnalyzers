using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;

namespace StyleCop.Analyzers.DocumentationRules
{
    public abstract class PartialElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        abstract protected void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, params Location[] diagnosticLocations);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static XmlNodeSyntax GetTopLevelElement(DocumentationCommentTriviaSyntax syntax, string tagName)
        {
            XmlElementSyntax elementSyntax = syntax.Content.OfType<XmlElementSyntax>().FirstOrDefault(element => string.Equals(element.StartTag.Name.ToString(), tagName));
            if (elementSyntax != null)
                return elementSyntax;

            XmlEmptyElementSyntax emptyElementSyntax = syntax.Content.OfType<XmlEmptyElementSyntax>().FirstOrDefault(element => string.Equals(element.Name.ToString(), tagName));
            return emptyElementSyntax;
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseTypeDeclarationSyntax;
            if (node == null || node.Identifier.IsMissing)
                return;

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
                return;

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // non-partial elements are handled by ElementDocumentationSummaryBase
                return;
            }

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

            if (GetTopLevelElement(documentation, XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            var xmlElement = GetTopLevelElement(documentation, XmlCommentHelper.SummaryXmlTag);
            if (xmlElement == null)
                xmlElement = GetTopLevelElement(documentation, XmlCommentHelper.ContentXmlTag);

            this.HandleXmlElement(context, xmlElement, locations);
        }
    }
}
