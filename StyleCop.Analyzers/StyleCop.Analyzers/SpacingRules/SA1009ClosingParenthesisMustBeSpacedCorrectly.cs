﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1009ClosingParenthesisMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1009";
        private const string Title = "Closing parenthesis must be spaced correctly";
        private const string MessageFormat = "Closing parenthesis must{0} be {1} by a space.";
        private const string Description = "A closing parenthesis within a C# statement is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1009.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            foreach (var token in root.DescendantTokens())
            {
                switch (token.Kind())
                {
                case SyntaxKind.CloseParenToken:
                    this.HandleCloseParenToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleCloseParenToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool precededBySpace = token.IsFirstInLine() || token.IsPrecededByWhitespace();
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();
            bool precedesStickyCharacter;
            bool allowEndOfLine = false;

            bool suppressFollowingSpaceError = false;

            SyntaxToken nextToken = token.GetNextToken();
            switch (nextToken.Kind())
            {
            case SyntaxKind.OpenParenToken:
            case SyntaxKind.CloseParenToken:
            case SyntaxKind.OpenBracketToken:
            case SyntaxKind.CloseBracketToken:
            case SyntaxKind.SemicolonToken:
            case SyntaxKind.CommaToken:
                precedesStickyCharacter = true;
                break;

            case SyntaxKind.QuestionToken:
                if (nextToken.Parent.IsKind(SyntaxKind.ConditionalAccessExpression))
                {
                    // allow a space for this case, but only if the ')' character is the last on the line
                    allowEndOfLine = true;
                    precedesStickyCharacter = true;
                }
                else
                {
                    precedesStickyCharacter = false;
                }

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

            switch (token.Parent.Kind())
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
