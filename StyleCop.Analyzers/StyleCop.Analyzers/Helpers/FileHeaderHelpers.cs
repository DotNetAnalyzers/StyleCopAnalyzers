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

            // wrap the xml from the file header in a single root element to make XML parsing work.
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
                return FileHeader.MissingFileHeader;
            }

            sb.AppendLine("</root>");

            try
            {
                var parsedFileHeaderXml = XElement.Parse(sb.ToString());

                // a header without any XML tags is malformed.
                if (!parsedFileHeaderXml.Descendants().Any())
                {
                    return FileHeader.MalformedFileHeader;
                }

                return new FileHeader(parsedFileHeaderXml, fileHeaderStart, fileHeaderEnd);
            }
            catch (XmlException)
            {
                return FileHeader.MalformedFileHeader;
            }
        }
    }
}
