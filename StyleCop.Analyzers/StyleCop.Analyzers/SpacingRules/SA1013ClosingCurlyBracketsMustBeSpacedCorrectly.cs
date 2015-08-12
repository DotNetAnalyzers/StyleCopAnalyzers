namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A closing curly bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing curly bracket is not correct.</para>
    ///
    /// <para>A closing curly bracket should always be followed by a single space, unless it is the last character on
    /// the line, or unless it is followed by a closing parenthesis, a comma, a semicolon, or a member access
    /// operator.</para>
    ///
    /// <para>A closing curly bracket must always be preceded by a single space, unless it is the first character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1013ClosingCurlyBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1013ClosingCurlyBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1013";
        private const string Title = "Closing curly brackets must be spaced correctly";
        private const string MessageFormat = "Closing curly bracket must{0} be {1} by a space.";
        private const string Description = "A closing curly bracket within a C# element is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1013.html";

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
                if (token.IsKind(SyntaxKind.CloseBraceToken))
                {
                    HandleCloseBraceToken(context, token);
                }
            }
        }

        private static void HandleCloseBraceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool precededBySpace = token.IsFirstInLine() || token.IsPrecededByWhitespace();

            if (token.Parent is InterpolationSyntax)
            {
                if (precededBySpace)
                {
                    // Closing curly bracket must{ not} be {preceded} by a space.
                    var properties = new Dictionary<string, string>
                    {
                        [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationPreceding,
                        [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                    };
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), " not", "preceded"));
                }

                return;
            }

            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();
            bool precedesSpecialCharacter;

            if (!followedBySpace && !lastInLine)
            {
                SyntaxToken nextToken = token.GetNextToken();
                precedesSpecialCharacter =
                    nextToken.IsKind(SyntaxKind.CloseParenToken)
                    || nextToken.IsKind(SyntaxKind.CloseBracketToken)
                    || nextToken.IsKind(SyntaxKind.CommaToken)
                    || nextToken.IsKind(SyntaxKind.SemicolonToken)
                    || nextToken.IsKind(SyntaxKind.DotToken)
                    || (nextToken.IsKind(SyntaxKind.QuestionToken) && nextToken.GetNextToken(includeZeroWidth: true).IsKind(SyntaxKind.DotToken));
            }
            else
            {
                precedesSpecialCharacter = false;
            }

            if (!precededBySpace)
            {
                // Closing curly bracket must{} be {preceded} by a space.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationPreceding,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionInsert
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), string.Empty, "preceded"));
            }

            if (!lastInLine && !precedesSpecialCharacter && !followedBySpace)
            {
                // Closing curly bracket must{} be {followed} by a space.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationFollowing,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionInsert
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), string.Empty, "followed"));
            }
        }
    }
}
