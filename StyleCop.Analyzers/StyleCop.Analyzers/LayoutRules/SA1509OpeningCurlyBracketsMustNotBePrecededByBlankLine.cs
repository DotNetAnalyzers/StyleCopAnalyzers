namespace StyleCop.Analyzers.LayoutRules
{

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Collections.Immutable;
    using System.Linq;

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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1509OpeningCurlyBracketsMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1509";
        private const string Title = "Opening curly brackets must not be preceded by blank line";
        private const string MessageFormat = "Opening curly brackets must not be preceded by blank line.";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "An opening curly bracket within a C# element, statement, or expression is preceded by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1509.html";

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
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private async void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = await context.Tree.GetRootAsync().ConfigureAwait(false);
            var openCurlyBraces = root.DescendantTokens()
                                      .Where(t => t.IsKind(SyntaxKind.OpenBraceToken))
                                      .ToList();

            foreach (var brace in openCurlyBraces)
            {
                var isPreviousLineEmpty = IsPreviousLineEmpty(brace);
                if (isPreviousLineEmpty.HasValue && 
                    isPreviousLineEmpty.Value) 
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, brace.GetLocation()));
                }
            }
        }

        private bool? IsPreviousLineEmpty(SyntaxToken token)
        {
            var fileLinePositionSpan = token.GetLocation().GetLineSpan();
            if (!fileLinePositionSpan.IsValid)
            {
                return null;
            }

            var startLine = fileLinePositionSpan.StartLinePosition.Line;
            var previousToken = token.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return false;
            }

            var endLineOfPreviousToken = previousToken.GetLocation().GetLineSpan().EndLinePosition.Line;
            var blankLinesBetweenTokens = startLine - endLineOfPreviousToken;
            if (blankLinesBetweenTokens <= 1)
            {
                return false;
            }

            return CheckIfAllBlanksLinesHaveComments(blankLinesBetweenTokens, endLineOfPreviousToken, token);
        }

        private static bool CheckIfAllBlanksLinesHaveComments(int blankLinesBetweenTokens, int endLineOfPreviousToken, SyntaxToken token)
        {
            var leadingCommentsTriviaLineSpans = token.LeadingTrivia
                .Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia) || t.IsKind(SyntaxKind.SingleLineCommentTrivia))
                .Select(t => t.GetLocation().GetLineSpan())
                .Where(t => t.IsValid)
                .ToList();
            if (leadingCommentsTriviaLineSpans.Count == 0)
            {
                return true;
            }

            for (int i = 1; i < blankLinesBetweenTokens; i++)
            {
                var lineToCheck = endLineOfPreviousToken + i;
                if (
                    !leadingCommentsTriviaLineSpans.Any(
                        c => c.StartLinePosition.Line == lineToCheck || c.EndLinePosition.Line == lineToCheck))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
