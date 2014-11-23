namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A positive sign within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a positive sign is not correct.</para>
    ///
    /// <para>A positive sign should always be preceded by a single space, unless it comes after an opening square
    /// bracket, a parenthesis, or is the first character on the line.</para>
    ///
    /// <para>A positive sign should never be followed by whitespace, and should never be the last character on a
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1022PositiveSignsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1022";
        internal const string Title = "Positive Signs Must Be Spaced Correctly";
        internal const string MessageFormat = "Positive sign must{0} be {1} by a space.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "A positive sign within a C# element is not spaced correctly.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1022.html";

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
                case SyntaxKind.PlusToken:
                    HandlePlusToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandlePlusToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            if (!token.Parent.IsKind(SyntaxKind.UnaryPlusExpression))
                return;

            bool precededBySpace;
            bool firstInLine;
            bool followsSpecialCharacter;

            bool followedBySpace;
            bool lastInLine;

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (firstInLine)
            {
                precededBySpace = true;
                followsSpecialCharacter = false;
            }
            else
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                precededBySpace = precedingToken.HasTrailingTrivia;

                followsSpecialCharacter =
                    precedingToken.IsKind(SyntaxKind.OpenBracketToken)
                    || precedingToken.IsKind(SyntaxKind.OpenParenToken)
                    || precedingToken.IsKind(SyntaxKind.CloseParenToken);
            }

            followedBySpace = token.HasTrailingTrivia;
            lastInLine = token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia);

            if (!firstInLine)
            {
                if (!followsSpecialCharacter && !precededBySpace)
                {
                    // Positive sign must{} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "preceded"));
                }
                else if (followsSpecialCharacter && precededBySpace)
                {
                    // Positive sign must{ not} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "preceded"));
                }
            }

            if (lastInLine || followedBySpace)
            {
                // Positive sign must{ not} be {followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "followed"));
            }
        }
    }
}
