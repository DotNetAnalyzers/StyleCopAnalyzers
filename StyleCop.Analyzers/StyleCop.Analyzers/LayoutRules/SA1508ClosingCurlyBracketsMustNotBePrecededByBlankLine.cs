namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A closing curly bracket within a C# element, statement, or expression is preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a closing curly bracket is preceded by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get 
    ///     { 
    ///         return this.enabled; 
    ///
    ///     }
    ///
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where closing
    /// curly brackets are preceded by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1508ClosingCurlyBracketsMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1508ClosingCurlyBracketsMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1508";
        private const string Title = "Closing curly brackets must not be preceded by blank line";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "A closing curly bracket within a C# element, statement, or expression is preceded by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1508.html";

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
