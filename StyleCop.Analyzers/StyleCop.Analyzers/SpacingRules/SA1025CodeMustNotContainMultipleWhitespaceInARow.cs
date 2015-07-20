namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The code contains multiple whitespace characters in a row.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains multiple whitespace characters in a row, unless
    /// the characters come at the beginning or end of a line of code, following a comma or semicolon or preceeding a
    /// symbol.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1025CodeMustNotContainMultipleWhitespaceInARow : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1025CodeMustNotContainMultipleWhitespaceInARow"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1025";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1025Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1025MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1025Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1025.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var trivia in root.DescendantTrivia())
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    this.HandleWhitespaceTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleWhitespaceTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            if (trivia.Span.Length <= 1)
            {
                return;
            }

            if (trivia.SyntaxTree.GetMappedLineSpan(trivia.Span).StartLinePosition.Character == 0)
            {
                return;
            }

            SyntaxToken token = trivia.Token;
            SyntaxToken precedingToken;
            SyntaxToken followingToken;
            if (token.LeadingTrivia.Contains(trivia))
            {
                precedingToken = token.GetPreviousToken();
                followingToken = token;
            }
            else if (token.TrailingTrivia.Contains(trivia))
            {
                precedingToken = token;
                followingToken = precedingToken.GetNextToken();
            }
            else
            {
                // shouldn't be reachable, but either way can't proceed
                return;
            }

            if (precedingToken.IsKind(SyntaxKind.CommaToken) || precedingToken.IsKind(SyntaxKind.SemicolonToken))
            {
                return;
            }

            // Code must not contain multiple whitespace characters in a row.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
