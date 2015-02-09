namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The while footer at the bottom of a do-while statement is separated from the statement by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when the while keyword at the bottom of a do-while statement is separated
    /// from the main part of the statement by one or more blank lines. For example:</para>
    ///
    /// <code language="csharp">
    /// do
    /// {
    ///     Console.WriteLine("Loop forever");
    /// }
    ///
    /// while (true);
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1511WhileDoFooterMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1511";
        private const string Title = "while-do footer must not be preceded by blank line";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The while footer at the bottom of a do-while statement is separated from the statement by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1511.html";

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
