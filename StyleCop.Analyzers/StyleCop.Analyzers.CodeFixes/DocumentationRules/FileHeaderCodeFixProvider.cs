// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Helpers.ObjectPools;
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
    internal class FileHeaderCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(
                FileHeaderAnalyzers.SA1633DescriptorMissing.Id,
                FileHeaderAnalyzers.SA1634Descriptor.Id,
                FileHeaderAnalyzers.SA1635Descriptor.Id,
                FileHeaderAnalyzers.SA1636Descriptor.Id,
                FileHeaderAnalyzers.SA1637Descriptor.Id,
                FileHeaderAnalyzers.SA1638Descriptor.Id,
                FileHeaderAnalyzers.SA1640Descriptor.Id,
                FileHeaderAnalyzers.SA1641Descriptor.Id);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1633CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, cancellationToken),
                        nameof(FileHeaderCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = document.Project.AnalyzerOptions.GetStyleCopSettings();

            var fileHeader = FileHeaderHelpers.ParseFileHeader(root);
            SyntaxNode newSyntaxRoot;
            if (fileHeader.IsMissing)
            {
                newSyntaxRoot = AddHeader(document, root, document.Name, settings);
            }
            else
            {
                var trivia = root.GetLeadingTrivia();
                var commentIndex = TriviaHelper.IndexOfFirstNonWhitespaceTrivia(trivia, false);

                // Safe to do this as fileHeader.IsMissing is false.
                var isMultiLineComment = trivia[commentIndex].IsKind(SyntaxKind.MultiLineCommentTrivia);

                var xmlFileHeader = FileHeaderHelpers.ParseXmlFileHeader(root);
                if (isMultiLineComment && !xmlFileHeader.IsMalformed)
                {
                    newSyntaxRoot = ReplaceWellFormedMultiLineCommentHeader(document, root, settings, commentIndex, xmlFileHeader);
                }
                else
                {
                    newSyntaxRoot = ReplaceHeader(document, root, settings, xmlFileHeader.IsMalformed);
                }
            }

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode ReplaceWellFormedMultiLineCommentHeader(Document document, SyntaxNode root, StyleCopSettings settings, int commentIndex, XmlFileHeader header)
        {
            SyntaxTriviaList trivia = root.GetLeadingTrivia();
            var commentTrivia = trivia[commentIndex];

            // Is the comment pushed in by a prefix?
            var commentIndentation = string.Empty;
            if (commentIndex > 0)
            {
                var prefixTrivia = trivia[commentIndex - 1];
                if (prefixTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    commentIndentation = prefixTrivia.ToFullString();
                }
            }

            var triviaString = commentTrivia.ToFullString();
            var startIndex = triviaString.IndexOf("/*", StringComparison.Ordinal) + 2;
            var endIndex = triviaString.LastIndexOf("*/", StringComparison.Ordinal);
            var commentContext = triviaString.Substring(startIndex, endIndex - startIndex).Trim(' ', '\t').TrimEnd();
            var triviaStringParts = commentContext.Replace("\r\n", "\n").Split('\n');

            // Assume we have comments that have a leading *
            string interlinePadding = " *";

            int minExpectedLength = (commentIndentation + interlinePadding).Length;
            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

            // Examine second line to see if we should have stars or not if it's blank
            // set the interline padding to be blank also.
            if ((triviaStringParts.Length > 2) &&
                (triviaStringParts[1].Length > minExpectedLength) &&
                string.IsNullOrWhiteSpace(triviaStringParts[1].Substring(0, minExpectedLength)))
            {
                interlinePadding = "  ";
            }

            // Pad line that used to be next to a /*
            triviaStringParts[0] = commentIndentation + interlinePadding + " " + triviaStringParts[0];
            StringBuilder sb = StringBuilderPool.Allocate();
            var copyrightText = commentIndentation + interlinePadding + " " +
                GetCopyrightText(commentIndentation + interlinePadding, settings.DocumentationRules.CopyrightText, newLineText);
            var newHeader = WrapInXmlComment(commentIndentation + interlinePadding, copyrightText, document.Name, settings, newLineText);

            sb.Append(commentIndentation);
            sb.Append("/*");
            if (header.GetElement("copyright") == null)
            {
                // No copyright element at the moment so add us.
                sb.Append(newHeader.Substring(minExpectedLength));
                sb.Append(newLineText);

                // Append the original stuff
                foreach (var oldLine in triviaStringParts)
                {
                    sb.Append(oldLine.TrimEnd());
                    sb.Append(newLineText);
                }
            }
            else
            {
                bool firstLine = true;
                bool inCopyright = false;
                foreach (var oldLine in triviaStringParts)
                {
                    var openingTag = oldLine.Contains("<copyright ");
                    var closingTag = oldLine.Contains("</copyright>") ||
                        (openingTag && oldLine.Trim().EndsWith("/>"));
                    if (openingTag)
                    {
                        inCopyright = !closingTag;
                        sb.Append(newHeader.Substring(firstLine ? minExpectedLength : 0));
                        sb.Append(newLineText);
                    }

                    if (inCopyright)
                    {
                        inCopyright = !closingTag;
                    }
                    else
                    {
                        sb.Append(oldLine.Substring(firstLine ? minExpectedLength : 0));
                        sb.Append(newLineText);
                    }

                    firstLine = false;
                }
            }

            sb.Append(commentIndentation);
            sb.Append(" */");

            // Get rid of any trailing spaces.
            var lines = sb.ToString().Split(new string[] { newLineText }, StringSplitOptions.None);
            sb.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                sb.Append((i == 0 ? string.Empty : newLineText) + lines[i].TrimEnd());
            }

            var newTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, StringBuilderPool.ReturnAndFree(sb));
            return root.WithLeadingTrivia(trivia.Replace(commentTrivia, newTrivia));
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
            var leadingSpaces = string.Empty;
            string possibleLeadingSpaces = string.Empty;

            // Need to do this with index so we get the line endings correct.
            for (int i = 0; i < trivia.Count; i++)
            {
                var triviaLine = trivia[i];
                bool done = false;
                switch (triviaLine.Kind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                    if (possibleLeadingSpaces != string.Empty)
                    {
                        leadingSpaces = possibleLeadingSpaces;
                    }

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
                        else if (inCopyright)
                        {
                            removalList.Add(i);
                            inCopyright = !closingTag;
                        }
                    }
                    else
                    {
                        removalList.Add(i);
                    }

                    onBlankLine = false;
                    break;

                case SyntaxKind.WhitespaceTrivia:
                    if (leadingSpaces == string.Empty)
                    {
                        possibleLeadingSpaces = triviaLine.ToFullString();
                    }

                    if (inCopyright)
                    {
                        removalList.Add(i);
                    }

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

            // Remove copyright lines in reverse order.
            for (int i = removalList.Count - 1; i >= 0; i--)
            {
                trivia = trivia.RemoveAt(removalList[i]);
            }

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
            var newLineTrivia = SyntaxFactory.EndOfLine(newLineText);

            var newHeaderTrivia = CreateNewHeader(leadingSpaces + "//", document.Name, settings, newLineText);
            if (!isMalformedHeader && copyrightTriviaIndex.HasValue)
            {
                // Does the copyright element have leading whitespace? If so remove it.
                if ((copyrightTriviaIndex.Value > 0) && trivia[copyrightTriviaIndex.Value - 1].IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    copyrightTriviaIndex = copyrightTriviaIndex - 1;
                    trivia = trivia.RemoveAt(copyrightTriviaIndex.Value);
                }

                // Replace copyright element in place.
                return root.WithLeadingTrivia(trivia.ReplaceRange(trivia[copyrightTriviaIndex.Value], newHeaderTrivia));
            }
            else
            {
                // Add blank line if we don't already have comments at top of file.
                if (!FirstLineIsComment(trivia))
                {
                    newHeaderTrivia = newHeaderTrivia.Add(newLineTrivia);
                }

                // Insert header at top of the file.
                return root.WithLeadingTrivia(newHeaderTrivia.Add(newLineTrivia).AddRange(trivia));
            }
        }

        private static bool FirstLineIsComment(SyntaxTriviaList trivia)
        {
            if ((trivia.Count > 0) && trivia[0].IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                return true;
            }

            if ((trivia.Count > 1) && trivia[0].IsKind(SyntaxKind.WhitespaceTrivia) && trivia[1].IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                return true;
            }

            return false;
        }

        private static SyntaxNode AddHeader(Document document, SyntaxNode root, string name, StyleCopSettings settings)
        {
            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
            var newLineTrivia = SyntaxFactory.EndOfLine(newLineText);
            var newTrivia = CreateNewHeader("//", name, settings, newLineText).Add(newLineTrivia).Add(newLineTrivia);
            newTrivia = newTrivia.AddRange(root.GetLeadingTrivia());

            return root.WithLeadingTrivia(newTrivia);
        }

        private static SyntaxTriviaList CreateNewHeader(string prefixWithLeadingSpaces, string filename, StyleCopSettings settings, string newLineText)
        {
            var copyrightText = prefixWithLeadingSpaces + " " + GetCopyrightText(prefixWithLeadingSpaces, settings.DocumentationRules.CopyrightText, newLineText);
            var newHeader = settings.DocumentationRules.XmlHeader
                ? WrapInXmlComment(prefixWithLeadingSpaces, copyrightText, filename, settings, newLineText)
                : copyrightText;
            return SyntaxFactory.ParseLeadingTrivia(newHeader);
        }

        private static string WrapInXmlComment(string prefixWithLeadingSpaces, string copyrightText, string filename, StyleCopSettings settings, string newLineText)
        {
            string encodedFilename = new XAttribute("t", filename).ToString().Substring(2).Trim('"');
            string encodedCompanyName = new XAttribute("t", settings.DocumentationRules.CompanyName).ToString().Substring(2).Trim('"');
            string encodedCopyrightText = new XText(copyrightText).ToString();

            return
                $"{prefixWithLeadingSpaces} <copyright file=\"{encodedFilename}\" company=\"{encodedCompanyName}\">" + newLineText
                + encodedCopyrightText + newLineText
                + prefixWithLeadingSpaces + " </copyright>";
        }

        private static string GetCopyrightText(string prefixWithLeadingSpaces, string copyrightText, string newLineText)
        {
            copyrightText = copyrightText.Replace("\r\n", "\n");
            return string.Join(newLineText + prefixWithLeadingSpaces + " ", copyrightText.Split('\n')).Replace(prefixWithLeadingSpaces + " " + newLineText, prefixWithLeadingSpaces + newLineText);
        }
    }
}
