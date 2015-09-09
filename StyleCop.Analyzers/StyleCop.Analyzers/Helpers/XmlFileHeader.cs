// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Class containing the parsed file header information.
    /// </summary>
    internal class XmlFileHeader
    {
        private readonly XElement headerXml;
        private readonly int fileHeaderStart;
        private readonly int fileHeaderEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileHeader"/> class.
        /// </summary>
        /// <param name="headerXml">The parsed XML tree from the header.</param>
        /// <param name="fileHeaderStart">The offset within the file at which the header started.</param>
        /// <param name="fileHeaderEnd">The offset within the file at which the header ended.</param>
        internal XmlFileHeader(XElement headerXml, int fileHeaderStart, int fileHeaderEnd)
        {
            this.headerXml = headerXml;
            this.fileHeaderStart = fileHeaderStart;
            this.fileHeaderEnd = fileHeaderEnd;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="XmlFileHeader"/> class from being created.
        /// </summary>
        private XmlFileHeader()
        {
        }

        /// <summary>
        /// Gets a <see cref="XmlFileHeader"/> instance representing a missing file header.
        /// </summary>
        /// <value>
        /// A <see cref="XmlFileHeader"/> instance representing a missing file header.
        /// </value>
        internal static XmlFileHeader MissingFileHeader
        {
            get
            {
                return new XmlFileHeader { IsMissing = true };
            }
        }

        /// <summary>
        /// Gets a <see cref="XmlFileHeader"/> instance representing a missing file header.
        /// </summary>
        /// <value>
        /// A <see cref="XmlFileHeader"/> instance representing a missing file header.
        /// </value>
        internal static XmlFileHeader MalformedFileHeader
        {
            get
            {
                return new XmlFileHeader { IsMalformed = true };
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
        /// True if the file header is not properly formatted XML.
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

        /// <summary>
        /// Gets the location representing the start of the file header.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to use for generating the location.</param>
        /// <returns>The location representing the start of the file header.</returns>
        internal Location GetLocation(SyntaxTree syntaxTree)
        {
            if (this.IsMissing || this.IsMalformed)
            {
                return Location.Create(syntaxTree, new TextSpan(0, 0));
            }

            return Location.Create(syntaxTree, TextSpan.FromBounds(this.fileHeaderStart, this.fileHeaderStart + 2));
        }

        /// <summary>
        /// Gets the location representing the position of the given element in the source file.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to use for generating the location.</param>
        /// <param name="element">The XML element to get the location of.</param>
        /// <returns>The location representing the position of the given element in the source file.</returns>
        internal Location GetElementLocation(SyntaxTree syntaxTree, XElement element)
        {
            var headerSourceText = syntaxTree.GetText().GetSubText(TextSpan.FromBounds(this.fileHeaderStart, this.fileHeaderEnd)).ToString();

            var tagStart = "<" + element.Name.LocalName;
            var index = headerSourceText.IndexOf(tagStart);

            var textSpan = TextSpan.FromBounds(this.fileHeaderStart + index, this.fileHeaderStart + index + tagStart.Length);
            return Location.Create(syntaxTree, textSpan);
        }
    }
}
