namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;


    /// <summary>
    /// The XML header documentation for a C# element is missing a <c>&lt;summary&gt;</c> tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the element documentation is missing a <c>&lt;summary&gt;</c>
    /// tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1604ElementDocumentationMustHaveSummary : ElementDocumentationSummaryBase
    {
        public const string DiagnosticId = "SA1604";
        internal const string Title = "Element documentation must have summary";
        internal const string MessageFormat = "Element documentation must have summary";
        internal const string Category = "StyleCop.CSharp.DocumentationRules";
        internal const string Description = "The XML header documentation for a C# element is missing a <summary> tag.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1604.html";
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

        protected internal override void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax syntax, Location[] diagnosticLocations)
        {
            if (syntax == null)
            {
                foreach (var location in diagnosticLocations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                }
            }
        }
    }
}