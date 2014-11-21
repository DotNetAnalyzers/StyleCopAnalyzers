namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The code contains multiple whitespace characters in a row.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains multiple whitespace characters in a row, unless
    /// the characters come at the beginning or end of a line of code, following a comma or semicolon or preceeding a
    /// symbol.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1025CodeMustNotContainMultipleWhitespaceInARow : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1025";
        internal const string Title = "Code Must Not Contain Multiple Whitespace In A Row";
        internal const string MessageFormat = "Code must not contain multiple whitespace characters in a row.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "The code contains multiple whitespace characters in a row.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1025.html";

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
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (trivia.CSharpKind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    HandleWhitespaceTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleWhitespaceTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            if (trivia.Span.Length <= 1)
                return;

            if (trivia.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0)
                return;

            // Code must not contain multiple whitespace characters in a row.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
