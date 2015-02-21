namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A <c>&lt;param&gt;</c> tag within a C# element's documentation header is missing a <c>name</c> attribute
    /// containing the name of the parameter.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the documentation for an element contains a <c>&lt;param&gt;</c> tag
    /// which is missing a <c>name</c> attribute, or which contains an empty <c>name</c> attribute.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1613ElementParameterDocumentationMustDeclareParameterName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1613";
        private const string Title = "Element parameter documentation must declare parameter name";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "A <param> tag within a C# element's documentation header is missing a name attribute containing the name of the parameter.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1613.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
