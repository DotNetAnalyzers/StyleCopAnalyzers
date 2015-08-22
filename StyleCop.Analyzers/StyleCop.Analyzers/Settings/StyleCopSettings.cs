namespace StyleCop.Analyzers
{
    /// <summary>
    /// Class containing the StyleCop settings.
    /// </summary>
    internal class StyleCopSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopSettings"/> class.
        /// </summary>
        /// <param name="companyName">The company name.</param>
        /// <param name="copyrightText">The copyright text.</param>
        internal StyleCopSettings(string companyName, string copyrightText)
        {
            this.CompanyName = companyName;
            this.CopyrightText = copyrightText;
        }

        /// <summary>
        /// Gets the company name, as defined in the settings.
        /// </summary>
        /// <value>A string representing the company name that must be present in all file headers.</value>
        public string CompanyName { get; }

        /// <summary>
        /// Gets the copyright text, as defined in the settings.
        /// </summary>
        /// <value>A string representing the copyright text that must be present in all file headers.</value>
        public string CopyrightText { get; }
    }
}
