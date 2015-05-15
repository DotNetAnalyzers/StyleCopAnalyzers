namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Class containing the parsed file header information.
    /// </summary>
    internal class FileHeader
    {
        private XElement headerXml;
        private SyntaxTree syntaxTree;
        private int fileHeaderStart;
        private int fileHeaderEnd;

        /// <summary>
        /// Prevents a default instance of the <see cref="FileHeader"/> class from being created.
        /// </summary>
        private FileHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHeader"/> class.
        /// </summary>
        /// <param name="headerXml">The parsed XML tree from the header.</param>
        /// <param name="syntaxTree">The syntax tree that the header belongs to.</param>
        /// <param name="fileHeaderStart">The offset within the file at which the header started.</param>
        /// <param name="fileHeaderEnd">The offset within the file at which the header ended.</param>
        internal FileHeader(XElement headerXml, SyntaxTree syntaxTree, int fileHeaderStart, int fileHeaderEnd)
        {
            this.headerXml = headerXml;
            this.syntaxTree = syntaxTree;
            this.fileHeaderStart = fileHeaderStart;
            this.fileHeaderEnd = fileHeaderEnd;
        }

        /// <summary>
        /// Gets a <see cref="FileHeader"/> instance representing a missing file header.
        /// </summary>
        /// <value>
        /// A <see cref="FileHeader"/> instance representing a missing file header.
        /// </value>
        internal static FileHeader MissingFileHeader
        {
            get
            {
                return new FileHeader { IsMissing = true };
            }
        }

        /// <summary>
        /// Gets a <see cref="FileHeader"/> instance representing a missing file header.
        /// </summary>
        /// <value>
        /// A <see cref="FileHeader"/> instance representing a missing file header.
        /// </value>
        internal static FileHeader MalformedFileHeader
        {
            get
            {
                return new FileHeader { IsMalformed = true };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the file header is missing.
        /// </summary>
        /// <value>
        /// True if the file header is missing.
        /// </value>
        internal bool IsMissing { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file header contains a properly formatted XML structure.
        /// </summary>
        /// <value>
        /// True if the file header is not properly formatted xml.
        /// </value>
        internal bool IsMalformed { get; private set; }

        /// <summary>
        /// Gets a XML element from the file header with the given tag name.
        /// </summary>
        /// <param name="tagName">The tag name for the node.</param>
        /// <returns>The requested node, or null if the node could not be found.</returns>
        internal XElement GetElement(string tagName)
        {
            return this.headerXml.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals(tagName, StringComparison.Ordinal));
        }

        internal Location GetElementLocation(XElement element)
        {
            var headerSourceText = this.syntaxTree.GetText().GetSubText(TextSpan.FromBounds(this.fileHeaderStart, this.fileHeaderEnd)).ToString();

            var tagStart = "<" + element.Name.LocalName;
            var index = headerSourceText.IndexOf(tagStart);

            var textSpan = TextSpan.FromBounds(this.fileHeaderStart + index, this.fileHeaderStart + index + tagStart.Length);
            return Location.Create(this.syntaxTree, textSpan);
        }
    }
}
