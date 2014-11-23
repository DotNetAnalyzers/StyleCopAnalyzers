namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A closing parenthesis within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the closing parenthesis within a statement is not spaced
    /// correctly.</para>
    ///
    /// <para>A closing parenthesis should never be preceded by whitespace. In most cases, a closing parenthesis should
    /// be followed by a single space, unless the closing parenthesis comes at the end of a cast, or the closing
    /// parenthesis is followed by certain types of operator symbols, such as positive signs, negative signs, and
    /// colons.</para>
    ///
    /// <para>If the closing parenthesis is followed by whitespace, the next non-whitespace character must not be an
    /// opening or closing parenthesis or square bracket, or a semicolon or comma.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1009ClosingParenthesisMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1009";
        internal const string Title = "Closing Parenthesis Must Be Spaced Correctly";
        internal const string MessageFormat = "Closing parenthesis must{0} be {1} by a space.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "A closing parenthesis within a C# statement is not spaced correctly.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1009.html";

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
                case SyntaxKind.CloseParenToken:
                    HandleCloseParenToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleCloseParenToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            bool precededBySpace;

            bool followedBySpace;
            bool lastInLine;
            bool precedesStickyCharacter;
            bool allowEndOfLine = false;

            bool firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
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
            bool suppressFollowingSpaceError = false;

            SyntaxToken nextToken = token.GetNextToken();
            switch (nextToken.CSharpKind())
            {
            case SyntaxKind.OpenParenToken:
            case SyntaxKind.CloseParenToken:
            case SyntaxKind.OpenBracketToken:
            case SyntaxKind.CloseBracketToken:
            case SyntaxKind.SemicolonToken:
            case SyntaxKind.CommaToken:
                precedesStickyCharacter = true;
                break;

            case SyntaxKind.PlusToken:
                precedesStickyCharacter = nextToken.Parent.IsKind(SyntaxKind.UnaryPlusExpression);
                // this will be reported as SA1022
                suppressFollowingSpaceError = true;
                break;

            case SyntaxKind.MinusToken:
                precedesStickyCharacter = nextToken.Parent.IsKind(SyntaxKind.UnaryMinusExpression);
                // this will be reported as SA1021
                suppressFollowingSpaceError = true;
                break;

            case SyntaxKind.DotToken:
                // allow a space for this case, but only if the ')' character is the last on the line
                allowEndOfLine = true;
                precedesStickyCharacter = true;
                break;

            case SyntaxKind.ColonToken:
                bool requireSpace =
                    nextToken.Parent.IsKind(SyntaxKind.ConditionalExpression)
                    || nextToken.Parent.IsKind(SyntaxKind.BaseConstructorInitializer)
                    || nextToken.Parent.IsKind(SyntaxKind.ThisConstructorInitializer);
                precedesStickyCharacter = !requireSpace;
                break;

            default:
                precedesStickyCharacter = false;
                break;
            }

            switch (token.Parent.CSharpKind())
            {
            case SyntaxKind.CastExpression:
                precedesStickyCharacter = true;
                break;

            default:
                break;
            }

            if (precededBySpace)
            {
                // Closing parenthesis must{ not} be {preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "preceded"));
            }

            if (!suppressFollowingSpaceError)
            {
                if (!precedesStickyCharacter && !followedBySpace)
                {
                    // Closing parenthesis must{} be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "followed"));
                }
                else if (precedesStickyCharacter && followedBySpace && (!lastInLine || !allowEndOfLine))
                {
                    // Closing parenthesis must{ not} be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "followed"));
                }
            }
        }
    }
}
