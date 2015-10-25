// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
    internal class SA1003SymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1003SymbolsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1003";

        internal const string CodeFixAction = "Action";
        internal const string InsertBeforeTag = "InsertBefore";
        internal const string RemoveBeforeTag = "RemoveBefore";
        internal const string InsertAfterTag = "InsertAfter";
        internal const string RemoveAfterTag = "RemoveAfter";
        internal const string RemoveEndOfLineTag = "RemoveEndOfLine";
        internal const string RemoveEndOfLineWithTrailingSpaceTag = "RemoveEndOfLineWithTrailingSpace";

        private const string Title = "Symbols must be spaced correctly";
        private const string MessageFormatNotFollowedByComment = "Operator '{0}' must not be followed by a comment.";
        private const string MessageFormatPrecededByWhitespace = "Operator '{0}' must be preceded by whitespace.";
        private const string MessageFormatNotPrecededByWhitespace = "Operator '{0}' must not be preceded by whitespace.";
        private const string MessageFormatFollowedByWhitespace = "Operator '{0}' must be followed by whitespace.";
        private const string MessageFormatNotFollowedByWhitespace = "Operator '{0}' must not be followed by whitespace.";
        private const string MessageFormatNotAtEndOfLine = "Operator '{0}' must not appear at the end of a line.";
        private const string Description = "The spacing around an operator symbol is incorrect, within a C# code file.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1003.md";

        private static readonly ImmutableArray<SyntaxKind> BinaryExpressionKinds =
            ImmutableArray.Create(
                SyntaxKind.CoalesceExpression,
                SyntaxKind.IsExpression,
                SyntaxKind.AsExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.ExclusiveOrExpression,
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.LessThanExpression,
                SyntaxKind.LessThanOrEqualExpression,
                SyntaxKind.GreaterThanExpression,
                SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxKind.LeftShiftExpression,
                SyntaxKind.RightShiftExpression,
                SyntaxKind.AddExpression,
                SyntaxKind.SubtractExpression,
                SyntaxKind.MultiplyExpression,
                SyntaxKind.DivideExpression,
                SyntaxKind.ModuloExpression,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrExpression);

        private static readonly ImmutableArray<SyntaxKind> PrefixUnaryExpressionKinds =
            ImmutableArray.Create(
                SyntaxKind.UnaryPlusExpression,
                SyntaxKind.UnaryMinusExpression,
                SyntaxKind.BitwiseNotExpression,
                SyntaxKind.LogicalNotExpression,
                SyntaxKind.PreIncrementExpression,
                SyntaxKind.PreDecrementExpression,
                SyntaxKind.AddressOfExpression,
                SyntaxKind.PointerIndirectionExpression);

        private static readonly ImmutableArray<SyntaxKind> PostfixUnaryExpressionKinds =
            ImmutableArray.Create(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression);

        private static readonly ImmutableArray<SyntaxKind> AssignmentExpressionKinds =
            ImmutableArray.Create(
                SyntaxKind.OrAssignmentExpression,
                SyntaxKind.AndAssignmentExpression,
                SyntaxKind.ExclusiveOrAssignmentExpression,
                SyntaxKind.LeftShiftAssignmentExpression,
                SyntaxKind.RightShiftAssignmentExpression,
                SyntaxKind.AddAssignmentExpression,
                SyntaxKind.SubtractAssignmentExpression,
                SyntaxKind.MultiplyAssignmentExpression,
                SyntaxKind.DivideAssignmentExpression,
                SyntaxKind.ModuloAssignmentExpression,
                SyntaxKind.SimpleAssignmentExpression);

        private static readonly ImmutableArray<SyntaxKind> LambdaExpressionKinds =
            ImmutableArray.Create(SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorDeclarationAction = HandleConstructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConditionalExpressionAction = HandleConditionalExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeParameterConstraintClauseAction = HandleTypeParameterConstraintClause;
        private static readonly Action<SyntaxNodeAnalysisContext> BinaryExpressionAction = HandleBinaryExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> PrefixUnaryExpressionAction = HandlePrefixUnaryExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> PostfixUnaryExpressionAction = HandlePostfixUnaryExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AssignmentExpressionAction = HandleAssignmentExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> CastExpressionAction = HandleCastExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> EqualsValueClauseAction = HandleEqualsValueClause;
        private static readonly Action<SyntaxNodeAnalysisContext> LambdaExpressionAction = HandleLambdaExpression;

        /// <summary>
        /// Gets the descriptor for prefix unary expression that may not be followed by a comment.
        /// </summary>
        /// <value>
        /// A diagnostic descriptor.
        /// </value>
        public static DiagnosticDescriptor DescriptorNotFollowedByComment { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatNotFollowedByComment, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the descriptor indicating that an operator must be preceded by whitespace.
        /// </summary>
        /// <value>
        /// A diagnostic descriptor.
        /// </value>
        public static DiagnosticDescriptor DescriptorPrecededByWhitespace { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatPrecededByWhitespace, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the descriptor indicating that an operator must be preceded by whitespace.
        /// </summary>
        /// <value>
        /// A diagnostic descriptor.
        /// </value>
        public static DiagnosticDescriptor DescriptorNotPrecededByWhitespace { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatNotPrecededByWhitespace, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the descriptor indicating that an operator must be followed by whitespace.
        /// </summary>
        /// <value>
        /// A diagnostic descriptor.
        /// </value>
        public static DiagnosticDescriptor DescriptorFollowedByWhitespace { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatFollowedByWhitespace, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the descriptor indicating that an operator must be preceded by whitespace.
        /// </summary>
        /// <value>
        /// A diagnostic descriptor.
        /// </value>
        public static DiagnosticDescriptor DescriptorNotFollowedByWhitespace { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatNotFollowedByWhitespace, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the descriptor indicating that an operator must be appear at the end of a text line.
        /// </summary>
        /// <value>
        /// A diagnostic descriptor.
        /// </value>
        public static DiagnosticDescriptor DescriptorNotAtEndOfLine { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatNotAtEndOfLine, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DescriptorPrecededByWhitespace);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ConditionalExpressionAction, SyntaxKind.ConditionalExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(TypeParameterConstraintClauseAction, SyntaxKind.TypeParameterConstraintClause);
            context.RegisterSyntaxNodeActionHonorExclusions(BinaryExpressionAction, BinaryExpressionKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(PrefixUnaryExpressionAction, PrefixUnaryExpressionKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(PostfixUnaryExpressionAction, PostfixUnaryExpressionKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(AssignmentExpressionAction, AssignmentExpressionKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(CastExpressionAction, SyntaxKind.CastExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(EqualsValueClauseAction, SyntaxKind.EqualsValueClause);
            context.RegisterSyntaxNodeActionHonorExclusions(LambdaExpressionAction, LambdaExpressionKinds);
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;
            if (constructorDeclaration.Initializer == null)
            {
                return;
            }

            CheckToken(context, constructorDeclaration.Initializer.ColonToken, true, false, true);
        }

        private static void HandleConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            CheckToken(context, conditionalExpression.QuestionToken, true, true, true);
            CheckToken(context, conditionalExpression.ColonToken, true, true, true);
        }

        private static void HandleTypeParameterConstraintClause(SyntaxNodeAnalysisContext context)
        {
            var typeParameterConstraint = (TypeParameterConstraintClauseSyntax)context.Node;

            CheckToken(context, typeParameterConstraint.ColonToken, true, true, true);
        }

        private static void HandleBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            CheckToken(context, binaryExpression.OperatorToken, true, true, true);
        }

        private static void HandlePrefixUnaryExpression(SyntaxNodeAnalysisContext context)
        {
            var unaryExpression = (PrefixUnaryExpressionSyntax)context.Node;
            var precedingToken = unaryExpression.OperatorToken.GetPreviousToken();
            var followingToken = unaryExpression.OperatorToken.GetNextToken();
            var followingTrivia = TriviaHelper.MergeTriviaLists(unaryExpression.OperatorToken.TrailingTrivia, followingToken.LeadingTrivia);

            /* let the outer operator handle things like the following, so no error is reported for '++':
             *   c ^= *++buf4;
             *
             * if the unary expression is inside parenthesis or an indexer, there should be no leading space
             */
            var mustHaveLeadingWhitespace = !(unaryExpression.Parent is PrefixUnaryExpressionSyntax)
                && !(unaryExpression.Parent is CastExpressionSyntax)
                && !precedingToken.IsKind(SyntaxKind.OpenParenToken)
                && !precedingToken.IsKind(SyntaxKind.OpenBracketToken)
                && !(precedingToken.IsKind(SyntaxKind.OpenBraceToken) && (precedingToken.Parent is InterpolationSyntax));

            bool analyze;
            switch (unaryExpression.OperatorToken.Kind())
            {
            case SyntaxKind.PlusToken:
                analyze = context.IsAnalyzerSuppressed(SA1022PositiveSignsMustBeSpacedCorrectly.DiagnosticId);
                break;
            case SyntaxKind.MinusToken:
                analyze = context.IsAnalyzerSuppressed(SA1021NegativeSignsMustBeSpacedCorrectly.DiagnosticId);
                break;
            case SyntaxKind.PlusPlusToken:
            case SyntaxKind.MinusMinusToken:
                analyze = context.IsAnalyzerSuppressed(SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly.DiagnosticId);
                break;
            default:
                analyze = true;
                break;
            }

            if (analyze)
            {
                if (followingTrivia.Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)) || followingTrivia.Any(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowedByComment, unaryExpression.OperatorToken.GetLocation(), unaryExpression.OperatorToken.Text));
                }
                else
                {
                    CheckToken(context, unaryExpression.OperatorToken, mustHaveLeadingWhitespace, false, false);
                }
            }
        }

        private static void HandlePostfixUnaryExpression(SyntaxNodeAnalysisContext context)
        {
            var unaryExpression = (PostfixUnaryExpressionSyntax)context.Node;
            var followingToken = unaryExpression.OperatorToken.GetNextToken();

            var mustHaveTrailingWhitespace = !followingToken.IsKind(SyntaxKind.CloseParenToken)
                && !followingToken.IsKind(SyntaxKind.CloseBracketToken)
                && !followingToken.IsKind(SyntaxKind.SemicolonToken)
                && !followingToken.IsKind(SyntaxKind.CommaToken)
                && !(followingToken.IsKind(SyntaxKind.CloseBraceToken) && (followingToken.Parent is InterpolationSyntax));

            // If the next token is a close brace token we are in an anonymous object creation or an initialization.
            // Then we allow a new line
            bool allowEndOfLine = followingToken.IsKind(SyntaxKind.CloseBraceToken);

            CheckToken(context, unaryExpression.OperatorToken, false, allowEndOfLine, mustHaveTrailingWhitespace);
        }

        private static void HandleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var assignmentExpression = (AssignmentExpressionSyntax)context.Node;

            CheckToken(context, assignmentExpression.OperatorToken, true, true, true);
        }

        private static void HandleCastExpression(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax)context.Node;
            var precedingToken = castExpression.OpenParenToken.GetPreviousToken();

            var mustHaveLeadingWhitespace = !(castExpression.Parent is PrefixUnaryExpressionSyntax)
                && !(castExpression.Parent is CastExpressionSyntax)
                && !precedingToken.IsKind(SyntaxKind.OpenParenToken)
                && !precedingToken.IsKind(SyntaxKind.OpenBracketToken)
                && !(precedingToken.IsKind(SyntaxKind.OpenBraceToken) && (precedingToken.Parent is InterpolationSyntax));

            var tokenString = castExpression.OpenParenToken.ToString() + castExpression.Type.ToString() + castExpression.CloseParenToken.ToString();
            CheckToken(context, castExpression.OpenParenToken, mustHaveLeadingWhitespace, false, false, tokenString);
            CheckToken(context, castExpression.CloseParenToken, false, false, false, tokenString);
        }

        private static void HandleEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            var equalsValueClause = (EqualsValueClauseSyntax)context.Node;

            CheckToken(context, equalsValueClause.EqualsToken, true, true, true);
        }

        private static void HandleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (LambdaExpressionSyntax)context.Node;

            CheckToken(context, lambdaExpression.ArrowToken, true, true, true);
        }

        private static void CheckToken(SyntaxNodeAnalysisContext context, SyntaxToken token, bool withLeadingWhitespace, bool allowAtEndOfLine, bool withTrailingWhitespace, string tokenText = null)
        {
            tokenText = tokenText ?? token.Text;

            var precedingToken = token.GetPreviousToken();
            var precedingTriviaList = TriviaHelper.MergeTriviaLists(precedingToken.TrailingTrivia, token.LeadingTrivia);

            var followingToken = token.GetNextToken();
            var followingTriviaList = TriviaHelper.MergeTriviaLists(token.TrailingTrivia, followingToken.LeadingTrivia);

            if (withLeadingWhitespace)
            {
                // Don't report missing leading whitespace when the token is the first token on a text line.
                if (!token.IsFirstInLine()
                    && ((precedingTriviaList.Count == 0) || !precedingTriviaList.Last().IsKind(SyntaxKind.WhitespaceTrivia)))
                {
                    var properties = ImmutableDictionary.Create<string, string>()
                        .Add(CodeFixAction, InsertBeforeTag);
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorPrecededByWhitespace, token.GetLocation(), properties, tokenText));
                }
            }
            else
            {
                // don't report leading whitespace when the token is the first token on a text line
                if (!token.IsOnlyPrecededByWhitespaceInLine()
                    && ((precedingTriviaList.Count > 0) && precedingTriviaList.Last().IsKind(SyntaxKind.WhitespaceTrivia)))
                {
                    var properties = ImmutableDictionary.Create<string, string>()
                        .Add(CodeFixAction, RemoveBeforeTag);
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPrecededByWhitespace, token.GetLocation(), properties, tokenText));
                }
            }

            if (!allowAtEndOfLine && token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
            {
                var properties = ImmutableDictionary.Create<string, string>();

                // Do not register a code fix action if there are non whitespace or end of line tokens present.
                if (followingTriviaList.All(t => t.IsKind(SyntaxKind.WhitespaceTrivia) || t.IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    properties = properties.Add(CodeFixAction, withTrailingWhitespace ? RemoveEndOfLineWithTrailingSpaceTag : RemoveEndOfLineTag);
                }

                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotAtEndOfLine, token.GetLocation(), properties, tokenText));
                return;
            }

            if (withTrailingWhitespace)
            {
                if ((followingTriviaList.Count == 0) || !(followingTriviaList.First().IsKind(SyntaxKind.WhitespaceTrivia) || followingTriviaList.First().IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    var properties = ImmutableDictionary.Create<string, string>()
                        .Add(CodeFixAction, InsertAfterTag);
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorFollowedByWhitespace, token.GetLocation(), properties, tokenText));
                }
            }
            else
            {
                if ((followingTriviaList.Count > 0) && followingTriviaList.First().IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    var properties = ImmutableDictionary.Create<string, string>()
                        .Add(CodeFixAction, RemoveAfterTag);
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowedByWhitespace, token.GetLocation(), properties, tokenText));
                }
            }
        }
    }
}
