namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helper class used for working with file headers
    /// </summary>
    internal static class FileHeaderHelpers
    {
        /// <summary>
        /// Parses a comment-only file header.
        /// </summary>
        /// <param name="root">The root of the syntax tree.</param>
        /// <returns>The copyright string, as parsed from the file header.</returns>
        internal static FileHeader ParseFileHeader(SyntaxNode root)
        {
            var firstToken = root.GetFirstToken(includeZeroWidth: true);

            if (!firstToken.HasLeadingTrivia)
            {
                return FileHeader.MissingFileHeader;
            }

            var sb = new StringBuilder();
            var endOfLineCount = 0;
            var done = false;
            var fileHeaderStart = int.MaxValue;
            var fileHeaderEnd = int.MinValue;

            int i;
            for (i = 0; !done && (i < firstToken.LeadingTrivia.Count); i++)
            {
                var trivia = firstToken.LeadingTrivia[i];

                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    endOfLineCount = 0;
                    break;
                case SyntaxKind.SingleLineCommentTrivia:
                    endOfLineCount = 0;

                    var commentString = trivia.ToFullString();

                    fileHeaderStart = Math.Min(trivia.FullSpan.Start, fileHeaderStart);
                    fileHeaderEnd = trivia.FullSpan.End;

                    sb.AppendLine(commentString.Substring(2).Trim());
                    break;
                case SyntaxKind.MultiLineCommentTrivia:
                    // only process a MultiLineCommentTrivia if no SingleLineCommentTrivia have been processed
                    if (sb.Length == 0)
                    {
                        var triviaString = trivia.ToFullString();

                        var startIndex = triviaString.IndexOf("/*", StringComparison.Ordinal) + 2;
                        var endIndex = triviaString.LastIndexOf("*/", StringComparison.Ordinal);
                        var commentContext = triviaString.Substring(startIndex, endIndex - startIndex).Trim();

                        var triviaStringParts = commentContext.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var part in triviaStringParts)
                        {
                            var trimmedPart = part.Trim();
                            if (trimmedPart.StartsWith("*"))
                            {
                                trimmedPart = trimmedPart.Substring(1).TrimStart();
                            }

                            sb.AppendLine(trimmedPart);
                        }

                        fileHeaderStart = trivia.FullSpan.Start;
                        fileHeaderEnd = trivia.FullSpan.End;
                    }

                    done = true;
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    endOfLineCount++;
                    done = endOfLineCount > 1;
                    break;
                default:
                    done = (fileHeaderStart < fileHeaderEnd) || !trivia.IsDirective;
                    break;
                }
            }

            if (fileHeaderStart > fileHeaderEnd)
            {
                return FileHeader.MissingFileHeader;
            }

            if (sb.Length > 0)
            {
                // remove the final newline
                var eolLength = Environment.NewLine.Length;
                sb.Remove(sb.Length - eolLength, eolLength);
            }

            return new FileHeader(sb.ToString(), fileHeaderStart, fileHeaderEnd);
        }

        /// <summary>
        /// Parses an XML-based file header.
        /// </summary>
        /// <param name="root">The root of the syntax tree.</param>
        /// <returns>The parsed file header.</returns>
        internal static XmlFileHeader ParseXmlFileHeader(SyntaxNode root)
        {
            var firstToken = root.GetFirstToken(includeZeroWidth: true);

            if (!firstToken.HasLeadingTrivia)
            {
                return XmlFileHeader.MissingFileHeader;
            }

            var sb = new StringBuilder();
            var endOfLineCount = 0;
            var done = false;
            var fileHeaderStart = int.MaxValue;
            var fileHeaderEnd = int.MinValue;

            // wrap the XML from the file header in a single root element to make XML parsing work.
            sb.AppendLine("<root>");

            int i;
            for (i = 0; !done && (i < firstToken.LeadingTrivia.Count); i++)
            {
                var trivia = firstToken.LeadingTrivia[i];

                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    endOfLineCount = 0;
                    break;
                case SyntaxKind.SingleLineCommentTrivia:
                    endOfLineCount = 0;

                    var commentString = trivia.ToFullString();

                    // ignore borders
                    if (commentString.StartsWith("//-", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    fileHeaderStart = Math.Min(trivia.FullSpan.Start, fileHeaderStart);
                    fileHeaderEnd = trivia.FullSpan.End;

                    sb.AppendLine(commentString.Substring(2));
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    endOfLineCount++;
                    done = endOfLineCount > 1;
                    break;
                default:
                    done = (fileHeaderStart < fileHeaderEnd) || !trivia.IsDirective;
                    break;
                }
            }

            if (fileHeaderStart > fileHeaderEnd)
            {
                return XmlFileHeader.MissingFileHeader;
            }

            sb.AppendLine("</root>");

            try
            {
                var parsedFileHeaderXml = XElement.Parse(sb.ToString());

                // a header without any XML tags is malformed.
                if (!parsedFileHeaderXml.Descendants().Any())
                {
                    return XmlFileHeader.MalformedFileHeader;
                }

                return new XmlFileHeader(parsedFileHeaderXml, fileHeaderStart, fileHeaderEnd);
            }
            catch (XmlException)
            {
                return XmlFileHeader.MalformedFileHeader;
            }
        }
    }
}
