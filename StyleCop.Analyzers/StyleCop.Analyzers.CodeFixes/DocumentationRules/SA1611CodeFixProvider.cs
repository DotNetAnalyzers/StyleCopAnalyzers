// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1611CodeFixProvider))]
    [Shared]
    internal class SA1611CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1611ElementParametersMustBeDocumented.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxToken identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (identifierToken.IsMissingOrDefault())
                {
                    continue;
                }

                var parameterSyntax = (ParameterSyntax)identifierToken.Parent;

                // Declaration --> ParameterList --> Parameter
                var parentDeclaration = parameterSyntax.Parent.Parent;
                switch (parentDeclaration.Kind())
                {
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.ParameterDocumentationCodeFix,
                            _ => GetParameterDocumentationTransformedDocumentAsync(context.Document, root, parentDeclaration, parameterSyntax),
                            nameof(SA1611CodeFixProvider)),
                        diagnostic);
                    break;
                }
            }
        }

        private static Task<Document> GetParameterDocumentationTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxNode parent, ParameterSyntax parameterSyntax)
        {
            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
            var documentation = parent.GetDocumentationCommentTriviaSyntax();

            var parameters = GetParentDeclarationParameters(parameterSyntax).ToList();
            var prevNode = ParameterDocumentationHelper.GetParameterDocumentationPrevNode(parent, parameterSyntax, parameters, s => s.Identifier, XmlCommentHelper.ParamXmlTag);

            if (prevNode == null)
            {
                prevNode = documentation.Content.GetXmlElements(XmlCommentHelper.TypeParamXmlTag).LastOrDefault();
            }

            // last fallback Summery or first in existing XML doc
            if (prevNode == null)
            {
                prevNode = documentation.Content.GetXmlElements(XmlCommentHelper.SummaryXmlTag).FirstOrDefault() ?? documentation.Content.First();
            }

            XmlNodeSyntax leadingNewLine = XmlSyntaxFactory.NewLine(newLineText);

            // HACK: The formatter isn't working when contents are added to an existing documentation comment, so we
            // manually apply the indentation from the last line of the existing comment to each new line of the
            // generated content.
            SyntaxTrivia exteriorTrivia = CommonDocumentationHelper.GetLastDocumentationCommentExteriorTrivia(documentation);
            if (!exteriorTrivia.Token.IsMissing)
            {
                leadingNewLine = leadingNewLine.ReplaceExteriorTrivia(exteriorTrivia);
            }

            var parameterDocumentation = MethodDocumentationHelper.CreateParametersDocumentationWithLeadingLine(leadingNewLine, parameterSyntax);
            var newDocumentation = documentation.InsertNodesAfter(prevNode, parameterDocumentation);

            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(documentation, newDocumentation)));
        }

        private static IEnumerable<ParameterSyntax> GetParentDeclarationParameters(ParameterSyntax parameterSyntax)
        {
            return (parameterSyntax.Parent as ParameterListSyntax)?.Parameters
                ?? (parameterSyntax.Parent as BracketedParameterListSyntax)?.Parameters;
        }
    }
}
