namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An <c>include</c> tag within an XML documentation header does not contain valid file and path attribute.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>As an alternative to authoring documentation directly within the code file, it is possible to place
    /// documentation for multiple elements within a separate XML file, and then reference a section of that file within
    /// an element's documentation header. This causes the compiler to import the documentation for that element from
    /// the included document. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;include file="IncludedDocumentation.xml" path="root/EnabledMethodDocs" /&gt;
    /// public bool Enabled(bool true)
    /// {
    /// }
    /// </code>
    ///
    /// <para>A violation of this rule occurs when the include tag is missing a file or path attribute, or contains an
    /// improperly formatted file or path attribute.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1647IncludeNodeDoesNotContainValidFileAndPath : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1647";
        internal const string Title = "Include node does not contain valid file and path";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.DocumentationRules";
        internal const string Description = "An include tag within an XML documentation header does not contain valid file and path attribute.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1647.html";

        public static readonly DiagnosticDescriptor Descriptor =
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
