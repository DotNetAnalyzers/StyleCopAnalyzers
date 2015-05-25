namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1615ElementReturnValueMustBeDocumented"/> and
    /// <see cref="SA1616ElementReturnValueDocumentationMustHaveText"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add and fill-in documentation text within a &lt;returns&gt; tag
    /// describing the value returned from the element.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1615SA1616CodeFixProvider))]
    [Shared]
    public class SA1615SA1616CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1615ElementReturnValueMustBeDocumented.DiagnosticId, SA1616ElementReturnValueDocumentationMustHaveText.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!FixableDiagnostics.Contains(diagnostic.Id, StringComparer.Ordinal))
                {
                    continue;
                }

                string description = "Document return value";
                context.RegisterCodeFix(CodeAction.Create(description, cancellationToken => this.GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var documentRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode syntax = documentRoot.FindNode(diagnostic.Location.SourceSpan);
            if (syntax == null)
            {
                return document;
            }

            MethodDeclarationSyntax methodDeclarationSyntax = syntax.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            DelegateDeclarationSyntax delegateDeclarationSyntax = syntax.FirstAncestorOrSelf<DelegateDeclarationSyntax>();
            if (methodDeclarationSyntax == null && delegateDeclarationSyntax == null)
            {
                return document;
            }

            DocumentationCommentTriviaSyntax documentationComment =
                methodDeclarationSyntax?.GetDocumentationCommentTriviaSyntax()
                ?? delegateDeclarationSyntax?.GetDocumentationCommentTriviaSyntax();
            if (documentationComment == null)
            {
                return document;
            }

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            bool isTask;
            bool isAsynchronousTestMethod;
            if (methodDeclarationSyntax != null)
            {
                isTask = IsTaskReturningMethod(semanticModel, methodDeclarationSyntax, cancellationToken);
                isAsynchronousTestMethod = isTask && this.IsAsynchronousTestMethod(semanticModel, methodDeclarationSyntax, cancellationToken);
            }
            else
            {
                isTask = IsTaskReturningMethod(semanticModel, delegateDeclarationSyntax, cancellationToken);
                isAsynchronousTestMethod = false;
            }

            XmlNodeSyntax returnsElement = documentationComment.Content.GetFirstXmlElement(XmlCommentHelper.ReturnsXmlTag) as XmlNodeSyntax;
            if (returnsElement != null && !isTask)
            {
                // This code fix doesn't know how to do anything more than document Task-returning methods.
                return document;
            }

            SyntaxList<XmlNodeSyntax> content = XmlSyntaxFactory.List();
            if (isTask)
            {
                content = content.Add(XmlSyntaxFactory.Text("A "));
                content = content.Add(XmlSyntaxFactory.SeeElement(SyntaxFactory.TypeCref(SyntaxFactory.ParseTypeName("global::System.Threading.Tasks.Task"))).WithAdditionalAnnotations(Simplifier.Annotation));
                string operationKind = isAsynchronousTestMethod ? "unit test" : "operation";
                content = content.Add(XmlSyntaxFactory.Text($" representing the asynchronous {operationKind}."));

                // wrap the generated content in a <placeholder> element for review.
                content = XmlSyntaxFactory.List(XmlSyntaxFactory.PlaceholderElement(content));
            }

            // Try to replace an existing <returns> element if the comment contains one. Otherwise, add it as a new element.
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot;

            if (returnsElement != null)
            {
                XmlEmptyElementSyntax emptyElement = returnsElement as XmlEmptyElementSyntax;
                if (emptyElement != null)
                {
                    XmlElementSyntax updatedReturns = XmlSyntaxFactory.Element(XmlCommentHelper.ReturnsXmlTag, content)
                        .WithLeadingTrivia(returnsElement.GetLeadingTrivia())
                        .WithTrailingTrivia(returnsElement.GetTrailingTrivia());
                    newRoot = root.ReplaceNode(returnsElement, updatedReturns);
                }
                else
                {
                    XmlElementSyntax updatedReturns = ((XmlElementSyntax)returnsElement).WithContent(content);
                    newRoot = root.ReplaceNode(returnsElement, updatedReturns);
                }
            }
            else
            {
                string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

                returnsElement = XmlSyntaxFactory.Element(XmlCommentHelper.ReturnsXmlTag, content);

                XmlNodeSyntax leadingNewLine = XmlSyntaxFactory.NewLine(newLineText);

                // HACK: The formatter isn't working when contents are added to an existing documentation comment, so we
                // manually apply the indentation from the last line of the existing comment to each new line of the
                // generated content.
                SyntaxTrivia exteriorTrivia = GetLastDocumentationCommentExteriorTrivia(documentationComment);
                if (!exteriorTrivia.Token.IsMissing)
                {
                    leadingNewLine = leadingNewLine.ReplaceExteriorTrivia(exteriorTrivia);
                    returnsElement = returnsElement.ReplaceExteriorTrivia(exteriorTrivia);
                }

                DocumentationCommentTriviaSyntax newDocumentationComment = documentationComment.WithContent(
                    documentationComment.Content.InsertRange(documentationComment.Content.Count - 1,
                    XmlSyntaxFactory.List(
                        leadingNewLine,
                        returnsElement)));

                newRoot = root.ReplaceNode(documentationComment, newDocumentationComment);
            }

            return document.WithSyntaxRoot(newRoot);
        }

        private static bool IsTaskReturningMethod(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
        {
            return IsTaskType(semanticModel, methodDeclarationSyntax.ReturnType, cancellationToken);
        }

        private static bool IsTaskReturningMethod(SemanticModel semanticModel, DelegateDeclarationSyntax delegateDeclarationSyntax, CancellationToken cancellationToken)
        {
            return IsTaskType(semanticModel, delegateDeclarationSyntax.ReturnType, cancellationToken);
        }

        private static bool IsTaskType(SemanticModel semanticModel, TypeSyntax typeSyntax, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(typeSyntax, cancellationToken);
            INamedTypeSymbol namedTypeSymbol = symbolInfo.Symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
            {
                return false;
            }

            if (!string.Equals(nameof(Task), namedTypeSymbol.Name, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(typeof(Task).Namespace, namedTypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)), StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        private bool IsAsynchronousTestMethod(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
        {
            foreach (AttributeListSyntax attributeList in methodDeclarationSyntax.AttributeLists)
            {
                if (attributeList.Target != null)
                {
                    continue;
                }

                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    IMethodSymbol methodSymbol = semanticModel.GetSymbolInfo(attribute.Name).Symbol as IMethodSymbol;
                    if (methodSymbol == null)
                    {
                        continue;
                    }

                    if (string.Equals(methodSymbol.ContainingType.Name, "TestMethodAttribute", StringComparison.Ordinal)
                        || string.Equals(methodSymbol.ContainingType.Name, "FactAttribute", StringComparison.Ordinal)
                        || string.Equals(methodSymbol.ContainingType.Name, "TheoryAttribute", StringComparison.Ordinal)
                        || string.Equals(methodSymbol.ContainingType.Name, "TestAttribute", StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static SyntaxTrivia GetLastDocumentationCommentExteriorTrivia(SyntaxNode node)
        {
            return node
                .DescendantTrivia(descendIntoTrivia: true)
                .Where(trivia => trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia))
                .LastOrDefault();
        }
    }
}
