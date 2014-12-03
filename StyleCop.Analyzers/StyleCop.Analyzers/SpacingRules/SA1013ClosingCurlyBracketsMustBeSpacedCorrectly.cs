namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A closing curly bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing curly bracket is not correct.</para>
    ///
    /// <para>A closing curly bracket should always be followed by a single space, unless it is the last character on
    /// the line, or unless it is followed by a closing parenthesis, a comma, or a semicolon.</para>
    ///
    /// <para>A closing curly bracket must always be preceded by a single space, unless it is the first character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1013ClosingCurlyBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1013";
        internal const string Title = "Closing Curly Brackets Must Be Spaced Correctly";
        internal const string MessageFormat = "Closing curly bracket must{0} be {1} by a space.";
        internal const string Category = "StyleCop.CSharp.SpacingRules";
        internal const string Description = "A closing curly bracket within a C# element is not spaced correctly.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1013.html";

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
            foreach (var token in root.DescendantTokens())
            {
                switch (token.CSharpKind())
                {
                case SyntaxKind.CloseBraceToken:
                    HandleCloseBraceToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleCloseBraceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            bool precededBySpace;
            bool firstInLine;

            bool followedBySpace;
            bool lastInLine;
            bool precedesSpecialCharacter;

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (firstInLine)
            {
                precededBySpace = true;
            }
            else
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                precededBySpace = precedingToken.HasTrailingTrivia;
            }

            followedBySpace = token.HasTrailingTrivia;
            lastInLine = followedBySpace && token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia);
            if (!followedBySpace && !lastInLine)
            {
                SyntaxToken nextToken = token.GetNextToken();
                precedesSpecialCharacter =
                    nextToken.IsKind(SyntaxKind.CloseParenToken)
                    || nextToken.IsKind(SyntaxKind.CommaToken)
                    || nextToken.IsKind(SyntaxKind.SemicolonToken);
            }
            else
            {
                precedesSpecialCharacter = false;
            }

            if (!firstInLine && !precededBySpace)
            {
                // Closing curly bracket must{} be {preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "preceded"));
            }

            if (!lastInLine)
            {
                if (!precedesSpecialCharacter && !followedBySpace)
                {
                    // Closing curly bracket must{} be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "followed"));
                }
                else if (precedesSpecialCharacter && followedBySpace)
                {
                    // Closing curly bracket must{ not} be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "followed"));
                }
            }
        }
    }
}
