namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An opening parenthesis within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening parenthesis within a statement is not spaced correctly.
    /// An opening parenthesis should not be preceded by any whitespace, unless it is the first character on the line,
    /// or it is preceded by certain C# keywords such as if, while, or for. In addition, an opening parenthesis is
    /// allowed to be preceded by whitespace when it follows an operator symbol within an expression.</para>
    ///
    /// <para>An opening parenthesis should not be followed by whitespace, unless it is the last character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1008OpeningParenthesisMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1008OpeningParenthesisMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1008";
        private const string Title = "Opening parenthesis must be spaced correctly";
        private const string MessageFormat = "Opening parenthesis must{0} be {1} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "An opening parenthesis within a C# statement is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1008.html";

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
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTree);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens().Where(t => t.IsKind(SyntaxKind.OpenParenToken)))
            {
                HandleOpenParenToken(context, token);
            }
        }

        private static void HandleOpenParenToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (token.HasTrailingTrivia && token.TrailingTrivia[0].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                // ignore open parenthesis when last on line.
                return;
            }

            var prevToken = token.GetPreviousToken();
            var leadingTriviaList = prevToken.TrailingTrivia.AddRange(token.LeadingTrivia);

            var isFirstOnLine = false;
            if (prevToken.GetLocation().GetLineSpan().EndLinePosition.Line < token.GetLocation().GetLineSpan().StartLinePosition.Line)
            {
                var done = false;
                for (var i = leadingTriviaList.Count - 1; !done && (i >= 0); i--)
                {
                    switch (leadingTriviaList[i].Kind())
                    {
                        case SyntaxKind.WhitespaceTrivia:
                            break;

                        case SyntaxKind.EndOfLineTrivia:
                            isFirstOnLine = true;
                            done = true;
                            return;

                        default:
                            done = true;
                            break;
                    }
                }
            }

            bool haveLeadingSpace;
            bool haveTrailingSpace;
            bool partOfUnaryExpression;
            bool startOfIndexer;

            var prevTokenIsOpenParen = prevToken.IsKind(SyntaxKind.OpenParenToken);

            switch (token.Parent.Kind())
            {
                case SyntaxKind.IfStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.CatchDeclaration:
                case SyntaxKind.CatchFilterClause:
                    haveLeadingSpace = true;
                    haveTrailingSpace = false;
                    break;

                case SyntaxKind.ArgumentList:
                case SyntaxKind.AttributeArgumentList:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.ConstructorConstraint:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.TypeOfExpression:
                    haveLeadingSpace = false;
                    haveTrailingSpace = false;
                    break;

                case SyntaxKind.ParenthesizedExpression:
                    partOfUnaryExpression = prevToken.Parent is PrefixUnaryExpressionSyntax;
                    startOfIndexer = prevToken.IsKind(SyntaxKind.OpenBracketToken);
                    var partOfCastExpression = prevToken.IsKind(SyntaxKind.CloseParenToken) && prevToken.Parent.IsKind(SyntaxKind.CastExpression);

                    haveLeadingSpace = !partOfUnaryExpression && !startOfIndexer && !partOfCastExpression;
                    haveTrailingSpace = false;
                    break;

                case SyntaxKind.CastExpression:
                    partOfUnaryExpression = prevToken.Parent is PrefixUnaryExpressionSyntax;
                    startOfIndexer = prevToken.IsKind(SyntaxKind.OpenBracketToken);
                    var consecutiveCast = prevToken.IsKind(SyntaxKind.CloseParenToken) && prevToken.Parent.IsKind(SyntaxKind.CastExpression);
                    var partOfInterpolation = prevToken.IsKind(SyntaxKind.OpenBraceToken) && prevToken.Parent.IsKind(SyntaxKind.Interpolation);

                    haveLeadingSpace = !partOfUnaryExpression && !startOfIndexer && !consecutiveCast && !partOfInterpolation;
                    haveTrailingSpace = false;
                    break;

                case SyntaxKind.ParameterList:
                    var partOfLambdaExpression = token.Parent.Parent.IsKind(SyntaxKind.ParenthesizedLambdaExpression);
                    haveLeadingSpace = partOfLambdaExpression;
                    haveTrailingSpace = false;
                    break;

                default:
                    haveLeadingSpace = false;
                    haveTrailingSpace = false;
                    break;
            }

            // Ignore spacing before if another opening parenthesis is before this.
            // That way the first opening parenthesis will report any spacing errors.
            if (!prevTokenIsOpenParen)
            {
                var hasLeadingComment = (leadingTriviaList.Count > 0) && leadingTriviaList.Last().IsKind(SyntaxKind.MultiLineCommentTrivia);
                var hasLeadingSpace = (leadingTriviaList.Count > 0) && leadingTriviaList.Last().IsKind(SyntaxKind.WhitespaceTrivia);

                if (!isFirstOnLine && !hasLeadingComment && (haveLeadingSpace ^ hasLeadingSpace))
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "location", "preceding" },
                        { "action", haveLeadingSpace ? "insert" : "remove" }
                    };

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), haveLeadingSpace ? string.Empty : " not", "preceded"));
                }
            }

            if (haveTrailingSpace ^ HasTrailingSpace(token))
            {
                var properties = new Dictionary<string, string>
                {
                    { "location", "following" },
                    { "action", haveTrailingSpace ? "insert" : "remove" }
                };

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), haveTrailingSpace ? string.Empty : " not", "followed"));
            }
        }

        private static bool HasTrailingSpace(SyntaxToken token)
        {
            var nextToken = token.GetNextToken();
            var triviaList = token.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

            return (triviaList.Count > 0) && triviaList.First().IsKind(SyntaxKind.WhitespaceTrivia);
        }
    }
}
