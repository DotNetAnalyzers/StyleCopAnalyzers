namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The spacing around an operator symbol is incorrect, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around an operator symbol is incorrect.</para>
    ///
    /// <para>The following types of operator symbols must be surrounded by a single space on either side: colons,
    /// arithmetic operators, assignment operators, conditional operators, logical operators, relational operators,
    /// shift operators, and lambda operators. For example:</para>
    ///
    /// <code language="cs">
    /// int x = 4 + y;
    /// </code>
    ///
    /// <para>In contrast, unary operators must be preceded by a single space, but must never be followed by any space.
    /// For example:</para>
    ///
    /// <code language="cs">
    /// bool x = !value;
    /// </code>
    ///
    /// <para>An exception occurs whenever the symbol is preceded or followed by a parenthesis or bracket, in which case
    /// there should be no space between the symbol and the bracket. For example:</para>
    ///
    /// <code language="cs">
    /// if (!value)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1003SymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1003";
        internal const string Title = "Symbols Must Be Spaced Correctly";
        internal const string MessageFormat = "Operator '{0}' must {1}.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "The spacing around an operator symbol is incorrect, within a C# code file.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1003.html";

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
                if (SyntaxFacts.IsBinaryExpressionOperatorToken(token.CSharpKind()) && token.Parent is BinaryExpressionSyntax)
                {
                    HandleBinaryExpressionOperatorToken(context, token);
                }
                else if (SyntaxFacts.IsPrefixUnaryExpressionOperatorToken(token.CSharpKind()) && token.Parent is PrefixUnaryExpressionSyntax)
                {
                    HandlePrefixUnaryExpressionOperatorToken(context, token);
                }
            }
        }

        private void HandleBinaryExpressionOperatorToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            HandleOperatorToken(context, token, true);
        }

        private void HandlePrefixUnaryExpressionOperatorToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            // let the outer operator handle things like the following, so no error is reported for '++':
            //   c ^= *++buf4;
            if (token.Parent?.Parent is PrefixUnaryExpressionSyntax)
                return;

            HandleOperatorToken(context, token, false);
        }

        private void HandleOperatorToken(SyntaxTreeAnalysisContext context, SyntaxToken token, bool isBinaryExpressionOperator)
        {
            if (token.IsMissing)
                return;

            bool allowAtLineEnd = isBinaryExpressionOperator;
            bool allowTrailingSpace = isBinaryExpressionOperator;
            bool allowTrailingNoSpace = !isBinaryExpressionOperator;

            bool precededBySpace;
            bool firstInLine;

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (firstInLine)
            {
                precededBySpace = true;
            }
            else
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                switch (precedingToken.CSharpKind())
                {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.CloseBracketToken:
                    // force this to true to suppress the warning for things like (!value)
                    precededBySpace = true;
                    break;

                default:
                    precededBySpace = precedingToken.HasTrailingTrivia;
                    break;
                }
            }

            bool followedBySpace = token.HasTrailingTrivia;
            bool lastInLine = followedBySpace && token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia);

            if (!allowAtLineEnd && lastInLine)
            {
                // Operator '{operator}' must {not appear at the end of a line}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), token.Text, "not appear at the end of a line"));
            }

            if (!precededBySpace)
            {
                // Operator '{operator}' must {be preceded by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), token.Text, "be preceded by a space"));
            }

            if (!allowTrailingSpace && followedBySpace)
            {
                // Operator '{operator}' must {not be followed by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), token.Text, "not be followed by a space"));
            }
            else if (!allowTrailingNoSpace && !followedBySpace)
            {
                // Operator '{operator}' must {be followed by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), token.Text, "be followed by a space"));
            }
        }
    }
}
