// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Class containing the parsed file header information.
    /// </summary>
    internal class FileHeader
    {
        private readonly int fileHeaderStart;
        private readonly int fileHeaderEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHeader"/> class.
        /// </summary>
        /// <param name="copyrightText">The copyright string, as parsed from the header.</param>
        /// <param name="fileHeaderStart">The offset within the file at which the header started.</param>
        /// <param name="fileHeaderEnd">The offset within the file at which the header ended.</param>
        internal FileHeader(string copyrightText, int fileHeaderStart, int fileHeaderEnd)
        {
            this.CopyrightText = copyrightText;
            this.fileHeaderStart = fileHeaderStart;
            this.fileHeaderEnd = fileHeaderEnd;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="FileHeader"/> class from being created.
        /// </summary>
        private FileHeader()
        {
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
        /// Gets or sets a value indicating whether the file header is missing.
        /// </summary>
        /// <value>
        /// True if the file header is missing.
        /// </value>
        internal bool IsMissing { get; private set; }

        /// <summary>
        /// Gets the copyright text, as parsed from the header.
        /// </summary>
        /// <value>
        /// The copyright text, as parsed from the header.
        /// </value>
        internal string CopyrightText { get; }

        /// <summary>
        /// Gets the location representing the start of the file header.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to use for generating the location.</param>
        /// <returns>The location representing the start of the file header.</returns>
        internal Location GetLocation(SyntaxTree syntaxTree)
        {
            if (this.IsMissing)
            {
                return Location.Create(syntaxTree, new TextSpan(0, 0));
            }

            return Location.Create(syntaxTree, TextSpan.FromBounds(this.fileHeaderStart, this.fileHeaderStart + 2));
        }
    }
}
