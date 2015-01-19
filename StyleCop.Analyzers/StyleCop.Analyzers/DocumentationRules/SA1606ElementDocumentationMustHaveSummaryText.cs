namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;


    /// <summary>
    /// The <c>&lt;summary&gt;</c> tag within the documentation header for a C# code element is empty.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the documentation header for an element contains an empty
    /// <c>&lt;summary&gt;</c> tag which does not contain a description of the element.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1606ElementDocumentationMustHaveSummaryText : ElementDocumentationSummaryBase
    {
        public const string DiagnosticId = "SA1606";
        internal const string Title = "Element documentation must have summary text";
        internal const string MessageFormat = "Element documentation must have summary text";
        internal const string Category = "StyleCop.CSharp.DocumentationRules";
        internal const string Description = "The <summary> tag within the documentation header for a C# code element is empty.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1606.html";

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

        protected internal override void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, Location diagnosticLocation)
        {
            if (syntax != null)
            {
                if (XmlCommentHelper.IsConsideredEmpty(syntax))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocation));
                }
            }
        }

        protected internal override void HandleXmlElement(SyntaxNodeAnalysisContext context, XmlElementSyntax syntax, Location[] diagnosticLocations)
        {
            if (syntax != null)
            {
                if (XmlCommentHelper.IsConsideredEmpty(syntax))
                {
                    foreach (var location in diagnosticLocations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    }
                }
            }
        }
    }
}
