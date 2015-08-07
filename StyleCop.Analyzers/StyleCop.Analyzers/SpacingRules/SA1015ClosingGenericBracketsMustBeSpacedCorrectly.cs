namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A closing generic bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing generic bracket is not correct.</para>
    ///
    /// <para>A closing generic bracket should never be preceded by whitespace, unless the bracket is the first
    /// character on the line. A closing generic bracket should be followed by an open parenthesis, a close parenthesis,
    /// a closing generic bracket, a nullable symbol, an end of line or a single whitespace (but not whitespace and an
    /// open parenthesis).</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1015ClosingGenericBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1015ClosingGenericBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1015";
        private const string Title = "Closing generic brackets must be spaced correctly";
        private const string MessageFormat = "Closing generic bracket must{0} be {1} by a space.";
        private const string Description = "A closing generic bracket within a C# element is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1015.html";

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
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTree);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                if (token.IsKind(SyntaxKind.GreaterThanToken))
                {
                    HandleGreaterThanToken(context, token);
                }
            }
        }

        private static void HandleGreaterThanToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            switch (token.Parent.Kind())
            {
            case SyntaxKind.TypeArgumentList:
            case SyntaxKind.TypeParameterList:
                break;

            default:
                // not a generic bracket
                return;
            }

            bool firstInLine = token.IsFirstInLine();
            bool lastInLine = token.IsLastInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace();
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool allowTrailingNoSpace;
            bool allowTrailingSpace;

            if (!lastInLine)
            {
                SyntaxToken nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.OpenParenToken:
                // DotToken isn't listed above, but it's required for reasonable member access formatting
                case SyntaxKind.DotToken:
                // CommaToken isn't listed above, but it's required for reasonable nested generic type arguments formatting
                case SyntaxKind.CommaToken:
                // OpenBracketToken isn't listed above, but it's required for reasonable array type formatting
                case SyntaxKind.OpenBracketToken:
                // SemicolonToken isn't listed above, but it's required for reasonable using alias declaration formatting
                case SyntaxKind.SemicolonToken:
                    allowTrailingNoSpace = true;
                    allowTrailingSpace = false;
                    break;

                case SyntaxKind.CloseParenToken:
                case SyntaxKind.GreaterThanToken:
                    allowTrailingNoSpace = true;
                    allowTrailingSpace = true;
                    break;

                case SyntaxKind.QuestionToken:
                    allowTrailingNoSpace = nextToken.Parent.IsKind(SyntaxKind.NullableType);
                    allowTrailingSpace = true;
                    break;

                default:
                    allowTrailingNoSpace = false;
                    allowTrailingSpace = true;
                    break;
                }
            }
            else
            {
                allowTrailingNoSpace = true;
                allowTrailingSpace = true;
            }

            if (!firstInLine && precededBySpace)
            {
                // Closing generic bracket must{ not} be {preceded} by a space.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationPreceding,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), " not", "preceded"));
            }

            if (!lastInLine)
            {
                if (!allowTrailingNoSpace && !followedBySpace)
                {
                    // Closing generic bracket must{} be {followed} by a space.
                    var properties = new Dictionary<string, string>
                    {
                        [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationFollowing,
                        [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionInsert
                    };
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), string.Empty, "followed"));
                }
                else if (!allowTrailingSpace && followedBySpace)
                {
                    // Closing generic bracket must{ not} be {followed} by a space.
                    var properties = new Dictionary<string, string>
                    {
                        [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationFollowing,
                        [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                    };
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), " not", "followed"));
                }
            }
        }
    }
}
