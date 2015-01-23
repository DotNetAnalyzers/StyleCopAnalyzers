namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A section of the XML header documentation for a C# element does not contain enough alphabetic characters.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when part of the documentation does contain enough characters. This rule
    /// is calculated by counting the number of alphabetic characters and numbers within the documentation text, and
    /// comparing it against the number of symbols and other non-alphabetic characters. If the percentage of
    /// non-alphabetic characters is too high, this generally indicates poorly formatted documentation which will be
    /// difficult to read. For example, consider the follow summary documentation:</para>
    ///
    /// <code>
    /// /// &lt;summary&gt;
    /// /// @)$(*A name--------
    /// /// &lt;/summary&gt;
    /// public class Name    
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1631DocumentationMustMeetCharacterPercentage : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1631";
        internal const string Title = "Documentation must meet character percentage";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.DocumentationRules";
        internal const string Description = "A section of the Xml header documentation for a C# element does not contain enough alphabetic characters.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1631.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
