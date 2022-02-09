// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

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
    /// <para>If the closing parenthesis is followed by whitespace, the next non-whitespace character should not be an
    /// opening or closing parenthesis or square bracket, or a semicolon or comma.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1009ClosingParenthesisMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1009ClosingParenthesisMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1009";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1009.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1009Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1009Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly LocalizableString MessageNotPreceded = new LocalizableResourceString(nameof(SpacingResources.SA1009MessageNotPreceded), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1009MessageNotFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1009MessageFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly DiagnosticDescriptor DescriptorNotPreceded =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotPreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorNotFollowed =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorFollowed =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);
#pragma warning restore SA1202 // Elements should be ordered by access

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DescriptorNotPreceded);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true))
            {
                if (token.IsKind(SyntaxKind.CloseParenToken))
                {
                    HandleCloseParenToken(context, token);
                }
            }
        }

        private static void HandleCloseParenToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool precededBySpace = token.IsFirstInLine() || token.IsPrecededByWhitespace(context.CancellationToken);
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();
            bool precedesStickyCharacter;
            bool allowEndOfLine = false;
            bool preserveLayout = false;

            bool suppressFollowingSpaceError = false;

            SyntaxToken nextToken = token.GetNextToken();
            switch (nextToken.Kind())
            {
            case SyntaxKind.OpenParenToken:
                // Allow a space between an open and a close paren when:
                // - they are part of an if statement
                // - they are on the same line
                // - the open paren is part of a parenthesized expression or a tuple expression.
                precedesStickyCharacter =
                        !(token.Parent.IsKind(SyntaxKind.IfStatement)
                        && (token.GetLine() == nextToken.GetLine())
                        && (nextToken.Parent.IsKind(SyntaxKind.ParenthesizedExpression) || nextToken.Parent.IsKind(SyntaxKindEx.TupleExpression)));
                break;

            case SyntaxKind.CloseParenToken:
            case SyntaxKind.OpenBracketToken:
            case SyntaxKind.CloseBracketToken:
            case SyntaxKind.SemicolonToken:
            case SyntaxKind.CommaToken:
            case SyntaxKind.DoubleQuoteToken:
            case SyntaxKindEx.DotDotToken:
                precedesStickyCharacter = true;
                break;

            case SyntaxKind.GreaterThanToken:
                precedesStickyCharacter = nextToken.Parent.IsKind(SyntaxKind.TypeArgumentList);
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
                    // A space follows unless this is a nullable tuple type
                    precedesStickyCharacter = nextToken.Parent.IsKind(SyntaxKind.NullableType);
                }

                break;

            case SyntaxKind.PlusToken:
                precedesStickyCharacter = nextToken.Parent.IsKind(SyntaxKind.UnaryPlusExpression);

                // this will be reported as SA1022
                suppressFollowingSpaceError = precedesStickyCharacter;
                break;

            case SyntaxKind.MinusToken:
                precedesStickyCharacter = nextToken.Parent.IsKind(SyntaxKind.UnaryMinusExpression);

                // this will be reported as SA1021
                suppressFollowingSpaceError = precedesStickyCharacter;
                break;

            case SyntaxKind.DotToken:
                // allow a space for this case, but only if the ')' character is the last on the line
                allowEndOfLine = true;
                precedesStickyCharacter = true;

                preserveLayout = nextToken.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression);
                break;

            case SyntaxKind.MinusGreaterThanToken:
                // allow a space for this case, but only if the ')' character is the last on the line
                allowEndOfLine = true;
                precedesStickyCharacter = true;
                break;

            case SyntaxKind.ColonToken:
                bool requireSpace =
                    nextToken.Parent.IsKind(SyntaxKind.ConditionalExpression)
                    || nextToken.Parent.IsKind(SyntaxKind.BaseConstructorInitializer)
                    || nextToken.Parent.IsKind(SyntaxKind.ThisConstructorInitializer)
                    || nextToken.Parent.IsKind(SyntaxKind.BaseList);
                precedesStickyCharacter = !requireSpace;
                break;

            case SyntaxKind.PlusPlusToken:
            case SyntaxKind.MinusMinusToken:
                precedesStickyCharacter = true;
                suppressFollowingSpaceError = false;
                break;

            case SyntaxKind.CloseBraceToken:
                precedesStickyCharacter = nextToken.Parent is InterpolationSyntax;
                break;

            case SyntaxKind.ExclamationToken when nextToken.Parent.IsKind(SyntaxKindEx.SuppressNullableWarningExpression):
                precedesStickyCharacter = true;
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

            foreach (var trivia in token.TrailingTrivia)
            {
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    break;
                }
                else if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)
                    || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                {
                    lastInLine = false;
                    precedesStickyCharacter = false;
                    break;
                }
            }

            if (precededBySpace)
            {
                // Closing parenthesis should{ not} be {preceded} by a space.
                ImmutableDictionary<string, string> properties;

                if (preserveLayout)
                {
                    properties = TokenSpacingProperties.RemovePrecedingPreserveLayout;
                }
                else
                {
                    properties = token.IsFirstInLine()
                        ? TokenSpacingProperties.RemovePreceding
                        : TokenSpacingProperties.RemoveImmediatePreceding;
                }

                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPreceded, token.GetLocation(), properties));
            }

            if (!suppressFollowingSpaceError)
            {
                if (!precedesStickyCharacter && !followedBySpace && !lastInLine)
                {
                    // Closing parenthesis should{} be {followed} by a space.
                    var properties = TokenSpacingProperties.InsertFollowing;
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorFollowed, token.GetLocation(), properties));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
                }
                else if (precedesStickyCharacter && followedBySpace && (!lastInLine || !allowEndOfLine))
                {
                    // Closing parenthesis should{ not} be {followed} by a space.
                    var properties = TokenSpacingProperties.RemoveFollowing;
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), properties));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
                }
            }
        }
    }
}
