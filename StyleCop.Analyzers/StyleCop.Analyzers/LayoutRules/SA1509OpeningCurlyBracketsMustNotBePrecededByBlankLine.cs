namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        private const string Description = "An opening curly bracket within a C# element, statement, or expression is preceded by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1509.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            var openBraces = syntaxRoot.DescendantTokens()
                .Where(t => t.IsKind(SyntaxKind.OpenBraceToken));

            foreach (var openBrace in openBraces)
            {
                AnalyzeOpenBrace(context, openBrace);
            }
        }

        private static void AnalyzeOpenBrace(SyntaxTreeAnalysisContext context, SyntaxToken openBrace)
        {
            var prevToken = openBrace.GetPreviousToken();
            var triviaList = prevToken.IsKind(SyntaxKind.None) ? openBrace.LeadingTrivia : TriviaHelper.MergeTriviaLists(prevToken.TrailingTrivia, openBrace.LeadingTrivia);

            var done = false;
            var eolCount = 0;
            for (var i = triviaList.Count - 1; !done && (i >= 0); i--)
            {
                switch (triviaList[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                default:
                    if (triviaList[i].IsDirective)
                    {
                        // These have a built-in end of line
                        eolCount++;
                    }

                    done = true;
                    break;
                }
            }

            if (eolCount < 2)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, openBrace.GetLocation()));
        }
    }
}
