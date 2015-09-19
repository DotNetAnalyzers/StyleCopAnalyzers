// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for file header diagnostics.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add a standard file header at the top of the file.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileHeaderCodeFixProvider))]
    [Shared]
    public class FileHeaderCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(
                FileHeaderAnalyzers.SA1633DescriptorMissing.Id,
                FileHeaderAnalyzers.SA1634Descriptor.Id,
                FileHeaderAnalyzers.SA1635Descriptor.Id,
                FileHeaderAnalyzers.SA1636Descriptor.Id);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => this.FixableDiagnosticIds.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(DocumentationResources.SA1633CodeFix, token => GetTransformedDocumentAsync(context.Document, token), equivalenceKey: nameof(FileHeaderCodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = document.Project.AnalyzerOptions.GetStyleCopSettings();

            var fileHeader = FileHeaderHelpers.ParseFileHeader(root);
            var xmlFileHeader = FileHeaderHelpers.ParseXmlFileHeader(root);
            var newSyntaxRoot = fileHeader.IsMissing ? AddHeader(document, root, document.Name, settings) : ReplaceHeader(document, root, settings, xmlFileHeader.IsMalformed);

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode ReplaceHeader(Document document, SyntaxNode root, StyleCopSettings settings, bool isMalformedHeader)
        {
            // If the header is well formed Xml then we parse out the copyright otherwise
            // Skip single line comments, whitespace, and end of line trivia until a blank line is encountered.
            SyntaxTriviaList trivia = root.GetLeadingTrivia();
            bool onBlankLine = false;
            bool inCopyright = isMalformedHeader;
            int? copyrightTriviaIndex = null;
            var removalList = new System.Collections.Generic.List<int>();

            // Need to do this with index so we get the line endings correct.
            for (int i = 0; i < trivia.Count; i++)
            {
                var triviaLine = trivia[i];
                bool done = false;
                switch (triviaLine.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        if (!isMalformedHeader)
                        {
                            var openingTag = triviaLine.ToFullString().Contains("<copyright ");
                            var closingTag = triviaLine.ToFullString().Contains("</copyright>") ||
                                (openingTag && triviaLine.ToFullString().Trim().EndsWith("/>"));
                            if (openingTag)
                            {
                                inCopyright = !closingTag;
                                copyrightTriviaIndex = i;
                            }
                            else
                            {
                                if (inCopyright)
                                {
                                    removalList.Add(i);
                                    inCopyright = !closingTag;
                                }
                            }
                        }
                        else
                        {
                            removalList.Add(i);
                        }

                        onBlankLine = false;
                        break;

                    case SyntaxKind.WhitespaceTrivia:
                        removalList.Add(i);
                        break;

                    case SyntaxKind.EndOfLineTrivia:
                        if (inCopyright)
                        {
                            removalList.Add(i);
                        }

                        if (onBlankLine)
                        {
                            done = true;
                        }
                        else
                        {
                            onBlankLine = true;
                        }

                        break;

                    default:
                        done = true;
                        break;
                }

                if (done)
                {
                    break;
                }
            }

            if (isMalformedHeader)
            {
                copyrightTriviaIndex = null;
            }

            // Remove copyright lines in reverse order
            removalList.Reverse();
            foreach (var triviaLine in removalList)
            {
                trivia = trivia.RemoveAt(triviaLine);
            }

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

            var newHeaderTrivia = CreateNewHeader(document.Name, settings, newLineText);
            if (copyrightTriviaIndex.HasValue)
            {
                if (inCopyright)
                {
                    newHeaderTrivia = newHeaderTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
                }

                trivia = trivia.ReplaceRange(trivia[copyrightTriviaIndex.Value], newHeaderTrivia);
                return root.WithLeadingTrivia(trivia);
            }
            else
            {
                if ((trivia.Count == 0) || (trivia[0].Kind() != SyntaxKind.SingleLineCommentTrivia))
                {
                    newHeaderTrivia = newHeaderTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
                }

                return root.WithLeadingTrivia(newHeaderTrivia.Add(SyntaxFactory.CarriageReturnLineFeed).AddRange(trivia));
            }
        }

        private static SyntaxNode AddHeader(Document document, SyntaxNode root, string name, StyleCopSettings settings)
        {
            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
            var newTrivia = CreateNewHeader(name, settings, newLineText).Add(SyntaxFactory.CarriageReturnLineFeed).Add(SyntaxFactory.CarriageReturnLineFeed);
            newTrivia = newTrivia.AddRange(root.GetLeadingTrivia());

            return root.WithLeadingTrivia(newTrivia);
        }

        private static SyntaxTriviaList CreateNewHeader(string filename, StyleCopSettings settings, string newLineText)
        {
            var copyrightText = "// " + GetCopyrightText(settings.DocumentationRules.CopyrightText, newLineText);
            var newHeader = settings.DocumentationRules.XmlHeader
                ? WrapInXmlComment(copyrightText, filename, settings, newLineText)
                : copyrightText;
            return SyntaxFactory.ParseLeadingTrivia(newHeader);
        }

        private static string WrapInXmlComment(string copyrightText, string filename, StyleCopSettings settings, string newLineText)
        {
            return
                $"// <copyright file=\"{filename}\" company=\"{settings.DocumentationRules.CompanyName}\">" + newLineText
                + copyrightText + newLineText
                + "// </copyright>";
        }

        private static string GetCopyrightText(string copyrightText, string newLineText)
        {
            copyrightText = copyrightText.Replace("\r\n", "\n");
            return string.Join(newLineText + "// ", copyrightText.Split('\n')).Replace("// " + newLineText, "//" + newLineText);
        }
    }
}