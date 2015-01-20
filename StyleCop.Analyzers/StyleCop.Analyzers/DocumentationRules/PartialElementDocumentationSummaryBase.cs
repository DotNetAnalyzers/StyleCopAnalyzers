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
        abstract protected void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, params Location[] diagnosticLocations);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseTypeDeclarationSyntax;

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag || x.StartTag.Name.ToString() == XmlCommentHelper.ContentXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag || x.StartTag.Name.ToString() == XmlCommentHelper.ContentXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }
    }
}
