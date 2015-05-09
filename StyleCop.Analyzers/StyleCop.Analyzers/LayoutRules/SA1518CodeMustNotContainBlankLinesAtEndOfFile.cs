namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The code file has blank lines at the end.
    /// </summary>
    /// <remarks>
    /// <para>To improve the layout of the code, StyleCop requires no blank lines at the end of files.</para>
    ///
    /// <para>A violation of this rule occurs when one or more blank lines are at the end of the file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1518CodeMustNotContainBlankLinesAtEndOfFile : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1518CodeMustNotContainBlankLinesAtEndOfFile"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1518";
        private const string Title = "Code must not contain blank lines at end of file";
        private const string MessageFormat = "Code must not contain blank lines at end of file";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The code file has blank lines at the end.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1518.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTreeAction);
        }

        private void HandleSyntaxTreeAction(SyntaxTreeAnalysisContext context)
        {
            var lastToken = context.Tree.GetRoot().GetLastToken(includeZeroWidth: true);

            if (lastToken.HasLeadingTrivia)
            {
                var leadingTrivia = lastToken.LeadingTrivia;

                var trailingWhitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(leadingTrivia);
                if (trailingWhitespaceIndex != -1)
                {
                    var textSpan = TextSpan.FromBounds(leadingTrivia[trailingWhitespaceIndex].SpanStart, leadingTrivia[leadingTrivia.Count - 1].Span.End);
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, textSpan)));
                }
            }
        }
    }
}
