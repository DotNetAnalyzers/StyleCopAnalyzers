namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An opening curly bracket within a C# element, statement, or expression is preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when an opening curly bracket is preceded by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    ///
    /// {
    ///     get
    ///
    ///     {
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where opening
    /// curly brackets are preceded by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1509OpeningCurlyBracketsMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1509";
        private const string Title = "Opening curly brackets must not be preceded by blank line";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "An opening curly bracket within a C# element, statement, or expression is preceded by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1509.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
