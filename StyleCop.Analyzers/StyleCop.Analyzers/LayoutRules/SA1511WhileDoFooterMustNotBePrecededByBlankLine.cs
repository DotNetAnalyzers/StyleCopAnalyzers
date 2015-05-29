namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1511WhileDoFooterMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1511";
        private const string Title = "While-do footer must not be preceded by blank line";
        private const string MessageFormat = "While-do footer must not be preceded by blank line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The while footer at the bottom of a do-while statement is separated from the statement by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1511.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDoStatement, SyntaxKind.DoStatement);
        }

        private void HandleDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;
            var whileKeyword = doStatement.WhileKeyword;

            if (!whileKeyword.HasLeadingTrivia)
            {
                return;
            }

            if (!HasLeadingBlankLines(whileKeyword.LeadingTrivia))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, whileKeyword.GetLocation()));
        }

        private static bool HasLeadingBlankLines(SyntaxTriviaList triviaList)
        {
            // skip any leading whitespace
            var index = triviaList.Count - 1;
            while ((index >= 0) && triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            if ((index < 0) || !triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return false;
            }

            var blankLineCount = -1;
            while (index >= 0)
            {
                switch (triviaList[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    // ignore;
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    blankLineCount++;
                    break;
                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    // directive trivia have an embedded end of line
                    blankLineCount++;
                    return blankLineCount > 0;
                default:
                    return blankLineCount > 0;
                }

                index--;
            }

            return true;
        }
    }
}
