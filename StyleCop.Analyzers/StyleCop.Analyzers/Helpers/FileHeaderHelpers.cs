namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
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
            var fileHeaderStarted = false;

            for (var i = 0; !done && (i < firstToken.LeadingTrivia.Count); i++)
            {
                var trivia = firstToken.LeadingTrivia[i];

                switch (trivia.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                        endOfLineCount = 0;
                        break;
                    case SyntaxKind.SingleLineCommentTrivia:
                        endOfLineCount = 0;
                        fileHeaderStarted = true;

                        var commentString = trivia.ToFullString();

                        // ignore borders
                        if (!commentString.StartsWith("//-", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine(commentString.Substring(2));
                        }

                        break;
                    case SyntaxKind.EndOfLineTrivia:
                        endOfLineCount++;
                        done = endOfLineCount > 1;
                        break;
                    default:
                        done = fileHeaderStarted || !trivia.IsDirective;
                        break;
                }
            }

            try
            {
                var parsedFileHeaderXml = XElement.Parse(sb.ToString());
                return new FileHeader(parsedFileHeaderXml);
            }
            catch (XmlException)
            {
                return FileHeader.MalformedFileHeader;
            }
        }
    }
}
