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
        /// Retrieves the indentation options from a document.
        /// </summary>
        /// <param name="document">The document for which the indentation options should be retrieved.</param>
        /// <returns>The active indentation options for the given document.</returns>
        public static IndentationOptions FromDocument(Document document)
        {
            return new IndentationOptions(document.Project.Solution.Workspace);
        }

        /// <summary>
        /// Gets the indentation size.
        /// </summary>
        public int IndentationSize { get; private set; }

        /// <summary>
        /// Gets the tab size.
        /// </summary>
        public int TabSize { get; private set; }

        /// <summary>
        /// Gets a value indicating whether tabs should be used instead of spaces.
        /// </summary>
        public bool UseTabs { get; private set; }
    }
}