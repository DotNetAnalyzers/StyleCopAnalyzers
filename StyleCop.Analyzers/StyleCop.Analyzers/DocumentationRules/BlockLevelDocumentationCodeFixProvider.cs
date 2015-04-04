namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for diagnostics that need to insert &lt;para&gt; elements around inline documentation
    /// content to form block-level documentation content.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlockLevelDocumentationCodeFixProvider))]
    [Shared]
    public class BlockLevelDocumentationCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(
                SA1653PlaceTextInParagraphs.DiagnosticId,
                SA1654UseChildBlocksConsistently.DiagnosticId,
                SA1655UseChildBlocksConsistentlyAcrossElementsOfTheSameKind.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!this.FixableDiagnosticIds.Contains(diagnostic.Id))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(DocumentationResources.BlockLevelDocumentationCodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), nameof(BlockLevelDocumentationCodeFixProvider)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode enclosingNode = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true);
            XmlNodeSyntax xmlNodeSyntax = enclosingNode as XmlNodeSyntax;
            if (xmlNodeSyntax == null)
            {
                return document;
            }

            XmlElementSyntax xmlElementSyntax = xmlNodeSyntax.FirstAncestorOrSelf<XmlElementSyntax>();
            if (xmlElementSyntax == null)
            {
                return document;
            }

            SyntaxToken startToken = root.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
            XmlNodeSyntax startNode = startToken.Parent as XmlNodeSyntax;
            if (startNode == null || startNode == xmlElementSyntax)
            {
                return document;
            }

            while (startNode.Parent != xmlElementSyntax)
            {
                startNode = startNode.Parent as XmlNodeSyntax;
                if (startNode == null)
                {
                    return document;
                }
            }

            SyntaxToken stopToken = root.FindToken(diagnostic.Location.SourceSpan.End - 1, findInsideTrivia: true);
            XmlNodeSyntax stopNode = stopToken.Parent as XmlNodeSyntax;
            if (stopNode == null || stopNode == xmlElementSyntax)
            {
                return document;
            }

            while (stopNode.Parent != xmlElementSyntax)
            {
                stopNode = stopNode.Parent as XmlNodeSyntax;
                if (stopNode == null)
                {
                    return document;
                }
            }

            int startIndex = xmlElementSyntax.Content.IndexOf(startNode);
            int stopIndex = xmlElementSyntax.Content.IndexOf(stopNode);
            if (startIndex < 0 || stopIndex < 0)
            {
                return document;
            }

            XmlElementSyntax paragraph = XmlSyntaxFactory.ParaElement(xmlElementSyntax.Content.Skip(startIndex).Take(stopIndex - startIndex + 1).ToArray());
            SyntaxList<XmlNodeSyntax> leadingWhitespaceContent;
            SyntaxList<XmlNodeSyntax> trailingWhitespaceContent;
            paragraph = TrimWhitespaceContent(paragraph, out leadingWhitespaceContent, out trailingWhitespaceContent);

            SyntaxList<XmlNodeSyntax> newContent = XmlSyntaxFactory.List();
            newContent = newContent.AddRange(xmlElementSyntax.Content.Take(startIndex));
            newContent = newContent.AddRange(leadingWhitespaceContent);
            newContent = newContent.Add(paragraph);
            newContent = newContent.AddRange(trailingWhitespaceContent);
            newContent = newContent.AddRange(xmlElementSyntax.Content.Skip(stopIndex + 1));
            return document.WithSyntaxRoot(root.ReplaceNode(xmlElementSyntax, xmlElementSyntax.WithContent(newContent)));
        }

        private static XmlElementSyntax TrimWhitespaceContent(XmlElementSyntax paragraph, out SyntaxList<XmlNodeSyntax> leadingWhitespaceContent, out SyntaxList<XmlNodeSyntax> trailingWhitespaceContent)
        {
            SyntaxList<XmlNodeSyntax> completeContent = XmlSyntaxFactory.List(paragraph.Content.SelectMany(ExpandTextNodes).ToArray());

            leadingWhitespaceContent = XmlSyntaxFactory.List(completeContent.TakeWhile(XmlCommentHelper.IsConsideredEmpty).ToArray());
            trailingWhitespaceContent = XmlSyntaxFactory.List(completeContent.Skip(leadingWhitespaceContent.Count).Reverse().TakeWhile(XmlCommentHelper.IsConsideredEmpty).Reverse().ToArray());

            SyntaxList<XmlNodeSyntax> trimmedContent = XmlSyntaxFactory.List(completeContent.Skip(leadingWhitespaceContent.Count).Take(completeContent.Count - leadingWhitespaceContent.Count - trailingWhitespaceContent.Count).ToArray());
            SyntaxTriviaList leadingTrivia = SyntaxFactory.TriviaList();
            SyntaxTriviaList trailingTrivia = SyntaxFactory.TriviaList();
            if (trimmedContent.Any())
            {
                leadingTrivia = trimmedContent[0].GetLeadingTrivia();
                trailingTrivia = trimmedContent.Last().GetTrailingTrivia();
                trimmedContent = trimmedContent.Replace(trimmedContent[0], trimmedContent[0].WithoutLeadingTrivia());
                trimmedContent = trimmedContent.Replace(trimmedContent.Last(), trimmedContent.Last().WithoutTrailingTrivia());
            }
            else
            {
                leadingTrivia = SyntaxFactory.TriviaList();
                trailingTrivia = SyntaxFactory.TriviaList();
            }

            XmlElementSyntax result = paragraph;

            if (leadingWhitespaceContent.Any())
            {
                var first = leadingWhitespaceContent[0];
                var newFirst = first.WithLeadingTrivia(first.GetLeadingTrivia().InsertRange(0, paragraph.GetLeadingTrivia()));
                leadingWhitespaceContent = leadingWhitespaceContent.Replace(first, newFirst);
            }
            else
            {
                leadingTrivia = leadingTrivia.InsertRange(0, result.GetLeadingTrivia());
            }

            if (trailingWhitespaceContent.Any())
            {
                var last = trailingWhitespaceContent.Last();
                var newLast = last.WithLeadingTrivia(last.GetLeadingTrivia().AddRange(paragraph.GetTrailingTrivia()));
                trailingWhitespaceContent = trailingWhitespaceContent.Replace(last, newLast);
            }
            else
            {
                trailingTrivia = trailingTrivia.AddRange(result.GetTrailingTrivia());
            }

            XmlTextSyntax firstTextNode = trimmedContent.FirstOrDefault() as XmlTextSyntax;
            if (firstTextNode != null && firstTextNode.TextTokens.Any())
            {
                SyntaxToken firstTextToken = firstTextNode.TextTokens[0];
                string leadingWhitespace = new string(firstTextToken.Text.Cast<char>().TakeWhile(char.IsWhiteSpace).ToArray());
                if (leadingWhitespace.Length > 0)
                {
                    SyntaxToken newFirstTextToken = XmlSyntaxFactory.TextLiteral(firstTextToken.Text.Substring(leadingWhitespace.Length)).WithTriviaFrom(firstTextToken);
                    XmlTextSyntax newFirstTextNode = firstTextNode.WithTextTokens(firstTextNode.TextTokens.Replace(firstTextToken, newFirstTextToken));
                    trimmedContent = trimmedContent.Replace(firstTextNode, newFirstTextNode);
                    leadingTrivia = leadingTrivia.Add(SyntaxFactory.Whitespace(leadingWhitespace));
                }
            }

            XmlTextSyntax lastTextNode = trimmedContent.LastOrDefault() as XmlTextSyntax;
            if (lastTextNode != null && lastTextNode.TextTokens.Any())
            {
                SyntaxToken lastTextToken = lastTextNode.TextTokens.Last();
                string trailingWhitespace = new string(lastTextToken.Text.Cast<char>().Reverse().TakeWhile(char.IsWhiteSpace).Reverse().ToArray());
                if (trailingWhitespace.Length > 0)
                {
                    SyntaxToken newLastTextToken = XmlSyntaxFactory.TextLiteral(lastTextToken.Text.Substring(0, lastTextToken.Text.Length - trailingWhitespace.Length)).WithTriviaFrom(lastTextToken);
                    XmlTextSyntax newLastTextNode = lastTextNode.WithTextTokens(lastTextNode.TextTokens.Replace(lastTextToken, newLastTextToken));
                    trimmedContent = trimmedContent.Replace(lastTextNode, newLastTextNode);
                    trailingTrivia = trailingTrivia.Insert(0, SyntaxFactory.Whitespace(trailingWhitespace));
                }
            }

            return result.WithContent(trimmedContent)
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia);
        }

        private static IEnumerable<XmlNodeSyntax> ExpandTextNodes(XmlNodeSyntax node)
        {
            XmlTextSyntax xmlTextSyntax = node as XmlTextSyntax;
            if (xmlTextSyntax == null)
            {
                yield return node;
                yield break;
            }

            foreach (var textToken in xmlTextSyntax.TextTokens)
            {
                yield return XmlSyntaxFactory.Text(textToken);
            }
        }
    }
}
