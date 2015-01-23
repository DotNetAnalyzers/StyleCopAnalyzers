namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The spacing around a semicolon is incorrect, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a semicolon is incorrect.</para>
    ///
    /// <para>A semicolon should always be followed by a single space, unless it is the last character on the line, and
    /// a semicolon should never be preceded by any whitespace, unless it is the first character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1002SemicolonsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1002";
        private const string Title = "Semicolons must be spaced correctly";
        private const string MessageFormat = "Semicolons must{0} be {1} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "The spacing around a semicolon is incorrect, within a C# code file.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1002.html";

        private static readonly DiagnosticDescriptor Descriptor =
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
                case SyntaxKind.SemicolonToken:
                    HandleSemicolonToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleSemicolonToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            // check for a following space
            bool missingFollowingSpace = true;
            if (token.HasTrailingTrivia)
            {
                if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                    missingFollowingSpace = false;
                else if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                    missingFollowingSpace = false;
            }
            else
            {
                SyntaxToken nextToken = token.GetNextToken();
                if (nextToken.IsKind(SyntaxKind.CloseParenToken))
                {
                    // for (; ;)
                    missingFollowingSpace = false;
                }
            }

            bool hasPrecedingSpace = false;
            bool ignorePrecedingSpace = false;
            if (!token.HasLeadingTrivia)
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.HasTrailingTrivia)
                    hasPrecedingSpace = true;

                if (precedingToken.IsKind(SyntaxKind.SemicolonToken))
                {
                    // for (; ;)
                    ignorePrecedingSpace = true;
                }
            }

            if (missingFollowingSpace)
            {
                // semicolon must{} be {followed} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "followed"));
            }

            if (hasPrecedingSpace && !ignorePrecedingSpace)
            {
                // semicolon must{ not} be {preceded} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "preceded"));
            }
        }
    }
}
