// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Class used for obtaining information about indentation options.
    /// </summary>
    internal class IndentationOptions
    {
        private IndentationOptions(Workspace workspace)
        {
            var options = workspace.Options;
            this.IndentationSize = options.GetOption(FormattingOptions.IndentationSize, LanguageNames.CSharp);
            this.TabSize = options.GetOption(FormattingOptions.TabSize, LanguageNames.CSharp);
            this.UseTabs = options.GetOption(FormattingOptions.UseTabs, LanguageNames.CSharp);
        }

        /// <summary>
        /// Gets the indentation size.
        /// </summary>
        /// <value>
        /// The indentation size.
        /// </value>
        public int IndentationSize { get; }

        /// <summary>
        /// Gets the tab size.
        /// </summary>
        /// <value>
        /// The tab size.
        /// </value>
        public int TabSize { get; }

        /// <summary>
        /// Gets a value indicating whether tabs should be used instead of spaces.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if tabs should be used instead of spaces; otherwise, <see langword="false"/>.
        /// </value>
        public bool UseTabs { get; }

        /// <summary>
        /// Retrieves the indentation options from a document.
        /// </summary>
        /// <param name="document">The document for which the indentation options should be retrieved.</param>
        /// <returns>The active indentation options for the given document.</returns>
        public static IndentationOptions FromDocument(Document document)
        {
            return new IndentationOptions(document.Project.Solution.Workspace);
        }
    }
}
