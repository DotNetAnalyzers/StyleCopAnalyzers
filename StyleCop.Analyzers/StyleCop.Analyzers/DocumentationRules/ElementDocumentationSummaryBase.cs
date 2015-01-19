using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;

namespace StyleCop.Analyzers.DocumentationRules
{
    public abstract class ElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        abstract protected internal void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, Location diagnosticLocation);
        abstract protected internal void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, Location[] diagnosticLocations);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleMemberDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(HandleFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private void HandleMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseTypeDeclarationSyntax;

            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as DelegateDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;

            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ConstructorDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as DestructorDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as PropertyDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as IndexerDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.ThisKeyword.GetLocation());
            }
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseFieldDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null && node.Declaration != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                var locations = node.Declaration.Variables.Select(v => v.Identifier.GetLocation()).ToArray();
                HandleXmlElement(context, xmlElement, locations);
            }
        }

        private void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as EventDeclarationSyntax;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);
            if (documentation != null)
            {
                var xmlElement = documentation.Content.OfType<XmlElementSyntax>().FirstOrDefault(x => x.StartTag.Name.ToString() == XmlCommentHelper.SummaryXmlTag);

                HandleXmlElement(context, xmlElement, node.Identifier.GetLocation());
            }
        }
    }
}
