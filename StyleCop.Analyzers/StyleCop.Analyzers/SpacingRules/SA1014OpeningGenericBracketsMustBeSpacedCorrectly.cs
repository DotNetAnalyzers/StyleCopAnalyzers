namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An opening generic bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around an opening generic bracket is not correct.</para>
    ///
    /// <para>An opening generic bracket should never be preceded or followed by whitespace, unless the bracket is the
    /// first or last character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1014OpeningGenericBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1014OpeningGenericBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1014";
        private const string Title = "Opening generic brackets must be spaced correctly";
        private const string MessageFormat = "Opening generic brackets must not be {0} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "An opening generic bracket within a C# element is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1014.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.CSharpKind())
                {
                case SyntaxKind.LessThanToken:
                    this.HandleLessThanToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleLessThanToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            switch (token.Parent.CSharpKind())
            {
            case SyntaxKind.TypeArgumentList:
            case SyntaxKind.TypeParameterList:
                break;

            default:
                // not a generic bracket
                return;
            }

            bool precededBySpace;
            bool firstInLine;

            bool followedBySpace;

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

            if (!firstInLine && precededBySpace)
            {
                // Opening generic brackets must not be {preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "preceded"));
            }

            if (followedBySpace)
            {
                // Opening generic brackets must not be {followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "followed"));
            }
        }
    }
}
