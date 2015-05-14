namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Class containing the parsed file header information.
    /// </summary>
    internal class FileHeader
    {
        private XElement headerXml;

        /// <summary>
        /// Prevents a default instance of the <see cref="FileHeader"/> class from being created.
        /// </summary>
        private FileHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHeader"/> class.
        /// </summary>
        /// <param name="headerXml">The XML representing the contents of the file header.</param>
        internal FileHeader(XElement headerXml)
        {
            this.headerXml = headerXml;
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
    }
}
