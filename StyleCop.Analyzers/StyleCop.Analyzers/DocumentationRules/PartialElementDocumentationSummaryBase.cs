// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// This is the base class for analyzers which examine the <c>&lt;summary&gt;</c> or <c>&lt;content&gt;</c> text of
    /// the documentation comment associated with a <c>partial</c> element.
    /// </summary>
    internal abstract class PartialElementDocumentationSummaryBase : DiagnosticAnalyzer
    {
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> typeDeclarationAction;
        private readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> methodDeclarationAction;

        protected PartialElementDocumentationSummaryBase()
        {
            this.typeDeclarationAction = this.HandleTypeDeclaration;
            this.methodDeclarationAction = this.HandleMethodDeclaration;
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(this.typeDeclarationAction, SyntaxKinds.TypeDeclaration);
                context.RegisterSyntaxNodeAction(this.methodDeclarationAction, SyntaxKind.MethodDeclaration);
            });
        }

        /// <summary>
        /// Analyzes the top-level <c>&lt;summary&gt;</c> or <c>&lt;content&gt;</c> element of a documentation comment.
        /// </summary>
        /// <param name="context">The current analysis context.</param>
        /// <param name="needsComment"><see langword="true"/> if the current documentation settings indicate that the
        /// element should be documented; otherwise, <see langword="false"/>.</param>
        /// <param name="syntax">The <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/> of the node
        /// to examine.</param>
        /// <param name="completeDocumentation">The complete documentation for the declared symbol, with any
        /// <c>&lt;include&gt;</c> elements expanded. If the XML documentation comment included a <c>&lt;summary&gt;</c>
        /// element, this value will be <see langword="null"/>, even if the XML documentation comment also included an
        /// <c>&lt;include&gt;</c> element.</param>
        /// <param name="diagnosticLocations">The location(s) where diagnostics, if any, should be reported.</param>
        protected abstract void HandleXmlElement(SyntaxNodeAnalysisContext context, bool needsComment, XmlNodeSyntax syntax, XElement completeDocumentation, params Location[] diagnosticLocations);

        private static bool IsPartialMethodDefinition(SyntaxNode node)
        {
            if (!node.IsKind(SyntaxKind.MethodDeclaration))
            {
                return false;
            }

            var methodDeclaration = (MethodDeclarationSyntax)node;

            return methodDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
                && (methodDeclaration.Body == null);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            // We handle TypeDeclarationSyntax instead of BaseTypeDeclarationSyntax because enums are not allowed to be
            // partial.
            var node = (TypeDeclarationSyntax)context.Node;
            if (node.Identifier.IsMissing)
            {
                return;
            }

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // non-elements are handled by ElementDocumentationSummaryBase
                return;
            }

            Accessibility declaredAccessibility = node.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
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

            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // non-partial elements are handled by ElementDocumentationSummaryBase
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

            if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            XElement completeDocumentation = null;
            var relevantXmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag);
            if (relevantXmlElement == null)
            {
                relevantXmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.ContentXmlTag);
            }

            if (relevantXmlElement == null)
            {
                relevantXmlElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag);
                if (relevantXmlElement != null)
                {
                    string rawDocumentation;
                    if (IsPartialMethodDefinition(node))
                    {
                        // Workaround: Roslyn does not support expanding include directives for partial method definitions.
                        //             (see src/Compilers/CSharp/Portable/Compiler/DocumentationCommentCompiler.cs#L315)
                        rawDocumentation = this.ExpandDocumentation(context.Compilation, documentation, relevantXmlElement);
                    }
                    else
                    {
                        var declaration = context.SemanticModel.GetDeclaredSymbol(node, context.CancellationToken);
                        rawDocumentation = declaration?.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                    }

                    completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);
                    if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                    {
                        // Ignore nodes with an <inheritdoc/> tag in the included XML.
                        return;
                    }
                }
            }

            this.HandleXmlElement(context, needsComment, relevantXmlElement, completeDocumentation, locations);
        }

        private string ExpandDocumentation(Compilation compilation, DocumentationCommentTriviaSyntax documentCommentTrivia, XmlNodeSyntax includeTag)
        {
            var sb = new StringBuilder();

            sb.Append("<member>\n");

            foreach (XmlNodeSyntax xmlNode in documentCommentTrivia.Content)
            {
                if (xmlNode == includeTag)
                {
                    this.ExpandIncludeTag(compilation, sb, xmlNode);
                }
                else
                {
                    sb.Append(xmlNode.ToString()).Append('\n');
                }
            }

            sb.Append("</member>\n");

            return sb.ToString();
        }

        private void ExpandIncludeTag(Compilation compilation, StringBuilder sb, XmlNodeSyntax xmlNode)
        {
            try
            {
                var includeElement = XElement.Parse(xmlNode.ToString(), LoadOptions.None);

                var fileAttribute = includeElement.Attribute(XName.Get(XmlCommentHelper.FileAttributeName));
                var pathAttribute = includeElement.Attribute(XName.Get(XmlCommentHelper.PathAttributeName));

                if ((fileAttribute != null) && (pathAttribute != null))
                {
                    var resolver = compilation.Options.XmlReferenceResolver;
                    if (resolver != null)
                    {
                        string resolvedFilePath = resolver.ResolveReference(fileAttribute.Value, null);

                        using (var xmlStream = resolver.OpenRead(resolvedFilePath))
                        {
                            var document = XDocument.Load(xmlStream);
                            var expandedInclude = document.XPathSelectElements(pathAttribute.Value);

                            foreach (var x in expandedInclude)
                            {
                                sb.Append(x.ToString()).Append('\n');
                            }
                        }
                    }
                }
            }
            catch
            {
                // if the include tag is invalid, ignore it.
            }
        }
    }
}
