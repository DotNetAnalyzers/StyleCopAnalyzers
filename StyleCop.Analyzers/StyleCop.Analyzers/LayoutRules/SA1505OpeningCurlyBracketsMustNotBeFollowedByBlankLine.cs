namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An opening curly bracket within a C# element, statement, or expression is followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when an opening curly bracket is followed by a blank line. For
    /// example:</para>
    ///
    /// <code>
    /// public bool Enabled
    /// {
    ///
    ///     get
    ///     {
    ///
    ///         return this.enabled; 
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where opening
    /// curly brackets are followed by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1505";
        private const string Title = "Opening curly brackets must not be followed by blank line";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "An opening curly bracket within a C# element, statement, or expression is followed by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1505.html";

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
