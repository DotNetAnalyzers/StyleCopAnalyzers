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
        abstract protected void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, Location diagnosticLocation);
        abstract protected void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, Location[] diagnosticLocations);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void HandleMemberDeclaration(SyntaxNodeAnalysisContext context)
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
