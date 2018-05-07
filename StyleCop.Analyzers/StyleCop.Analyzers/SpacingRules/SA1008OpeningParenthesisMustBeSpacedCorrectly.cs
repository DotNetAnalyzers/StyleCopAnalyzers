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
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// An opening parenthesis within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening parenthesis within a statement is not spaced correctly.
    /// An opening parenthesis should not be preceded by any whitespace, unless it is the first character on the line,
    /// or it is preceded by certain C# keywords such as if, while, or for. In addition, an opening parenthesis is
    /// allowed to be preceded by whitespace when it follows an operator symbol within an expression.</para>
    ///
    /// <para>An opening parenthesis should not be followed by whitespace, unless it is the last character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1008OpeningParenthesisMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1008OpeningParenthesisMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1008";
        private const string Title = "Opening parenthesis should be spaced correctly";
        private const string Description = "An opening parenthesis within a C# statement is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1008.md";

        private const string MessageNotPreceded = "Opening parenthesis should not be preceded by a space.";
        private const string MessagePreceded = "Opening parenthesis should be preceded by a space.";
        private const string MessageNotFollowed = "Opening parenthesis should not be followed by a space.";

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <summary>
        /// Gets the diagnostic descriptor for an opening parenthesis that should not be preceded by whitespace.
        /// </summary>
        /// <value>The diagnostic descriptor for an opening parenthesis that should not be preceded by whitespace.</value>
        public static DiagnosticDescriptor DescriptorNotPreceded { get; }
            = new DiagnosticDescriptor(DiagnosticId, Title, MessageNotPreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for an opening parenthesis that should be preceded by whitespace.
        /// </summary>
        /// <value>The diagnostic descriptor for an opening parenthesis that should be preceded by whitespace.</value>
        public static DiagnosticDescriptor DescriptorPreceded { get; }
            = new DiagnosticDescriptor(DiagnosticId, Title, MessagePreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for an opening parenthesis that should not be followed by whitespace.
        /// </summary>
        /// <value>The diagnostic descriptor for an opening parenthesis that should not be followed by whitespace.</value>
        public static DiagnosticDescriptor DescriptorNotFollowed { get; }
            = new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true).Where(t => t.IsKind(SyntaxKind.OpenParenToken)))
            {
                HandleOpenParenToken(context, token);
            }
        }

        private static void HandleOpenParenToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (token.IsLastInLine())
            {
                // ignore open parenthesis when last on line.
                return;
            }

            var prevToken = token.GetPreviousToken();

            // Don't check leading spaces when preceded by a keyword that is already handled by SA1000
            bool precededByKeyword;
            switch (prevToken.Kind())
            {
            case SyntaxKind.AwaitKeyword:
            case SyntaxKind.CaseKeyword:
            case SyntaxKind.CatchKeyword:
            case SyntaxKind.CheckedKeyword:
            case SyntaxKind.DefaultKeyword:
            case SyntaxKind.FixedKeyword:
            case SyntaxKind.ForKeyword:
            case SyntaxKind.ForEachKeyword:
            case SyntaxKind.FromKeyword:
            case SyntaxKind.GroupKeyword:
            case SyntaxKind.IfKeyword:
            case SyntaxKind.InKeyword:
            case SyntaxKind.IntoKeyword:
            case SyntaxKind.JoinKeyword:
            case SyntaxKind.LetKeyword:
            case SyntaxKind.LockKeyword:
            case SyntaxKind.NameOfKeyword:
            case SyntaxKind.NewKeyword:
            case SyntaxKind.OrderByKeyword:
            case SyntaxKind.ReturnKeyword:
            case SyntaxKind.SelectKeyword:
            case SyntaxKind.SizeOfKeyword:
            case SyntaxKind.StackAllocKeyword:
            case SyntaxKind.SwitchKeyword:
            case SyntaxKind.ThrowKeyword:
            case SyntaxKind.TypeOfKeyword:
            case SyntaxKind.UncheckedKeyword:
            case SyntaxKind.UsingKeyword:
            case SyntaxKind.WhereKeyword:
            case SyntaxKind.WhileKeyword:
            case SyntaxKind.YieldKeyword:
                precededByKeyword = true;
                break;

            default:
                precededByKeyword = false;
                break;
            }

            var leadingTriviaList = TriviaHelper.MergeTriviaLists(prevToken.TrailingTrivia, token.LeadingTrivia);

            var isFirstOnLine = false;
            if (prevToken.GetLineSpan().EndLinePosition.Line < token.GetLineSpan().StartLinePosition.Line)
            {
                var done = false;
                for (var i = leadingTriviaList.Count - 1; !done && (i >= 0); i--)
                {
                    switch (leadingTriviaList[i].Kind())
                    {
                    case SyntaxKind.WhitespaceTrivia:
                        break;

                    case SyntaxKind.EndOfLineTrivia:
                        isFirstOnLine = true;
                        done = true;
                        break;

                    default:
                        done = true;
                        break;
                    }
                }
            }

            bool haveLeadingSpace;
            bool partOfUnaryExpression;
            bool startOfIndexer;

            var prevTokenIsOpenParen = prevToken.IsKind(SyntaxKind.OpenParenToken);

            switch (token.Parent.Kind())
            {
            case SyntaxKind.IfStatement:
            case SyntaxKind.DoStatement:
            case SyntaxKind.WhileStatement:
            case SyntaxKind.ForStatement:
            case SyntaxKind.ForEachStatement:
            case SyntaxKind.SwitchStatement:
            case SyntaxKind.FixedStatement:
            case SyntaxKind.LockStatement:
            case SyntaxKind.UsingStatement:
            case SyntaxKind.CatchDeclaration:
            case SyntaxKind.CatchFilterClause:
                haveLeadingSpace = true;
                break;

            case SyntaxKind.ArgumentList:
            case SyntaxKind.AttributeArgumentList:
            case SyntaxKind.CheckedExpression:
            case SyntaxKind.UncheckedExpression:
            case SyntaxKind.ConstructorConstraint:
            case SyntaxKind.DefaultExpression:
            case SyntaxKind.SizeOfExpression:
            case SyntaxKind.TypeOfExpression:
            default:
                haveLeadingSpace = false;
                break;

            case SyntaxKindEx.ParenthesizedVariableDesignation:
                haveLeadingSpace = true;
                break;

            case SyntaxKind.ParenthesizedExpression:
            case SyntaxKindEx.TupleExpression:
                if (prevToken.Parent.IsKind(SyntaxKind.Interpolation))
                {
                    haveLeadingSpace = false;
                    break;
                }

                partOfUnaryExpression = prevToken.Parent is PrefixUnaryExpressionSyntax;
                startOfIndexer = prevToken.IsKind(SyntaxKind.OpenBracketToken);
                var partOfCastExpression = prevToken.IsKind(SyntaxKind.CloseParenToken) && prevToken.Parent.IsKind(SyntaxKind.CastExpression);

                haveLeadingSpace = !partOfUnaryExpression && !startOfIndexer && !partOfCastExpression;
                break;

            case SyntaxKind.CastExpression:
                partOfUnaryExpression = prevToken.Parent is PrefixUnaryExpressionSyntax;
                startOfIndexer = prevToken.IsKind(SyntaxKind.OpenBracketToken);
                var consecutiveCast = prevToken.IsKind(SyntaxKind.CloseParenToken) && prevToken.Parent.IsKind(SyntaxKind.CastExpression);
                var partOfInterpolation = prevToken.IsKind(SyntaxKind.OpenBraceToken) && prevToken.Parent.IsKind(SyntaxKind.Interpolation);

                haveLeadingSpace = !partOfUnaryExpression && !startOfIndexer && !consecutiveCast && !partOfInterpolation;
                break;

            case SyntaxKind.ParameterList:
                var partOfLambdaExpression = token.Parent.Parent.IsKind(SyntaxKind.ParenthesizedLambdaExpression);
                haveLeadingSpace = partOfLambdaExpression;
                break;

            case SyntaxKindEx.TupleType:
                // Comma covers tuple types in parameters and nested within other tuple types.
                // 'out', 'ref', 'in', 'params' parameters are covered by IsKeywordKind.
                // Return types are handled by a helper.
                haveLeadingSpace = prevToken.IsKind(SyntaxKind.CommaToken)
                    || SyntaxFacts.IsKeywordKind(prevToken.Kind())
                    || ((TypeSyntax)token.Parent).GetContainingNotEnclosingType().IsReturnType();
                break;
            }

            // Ignore spacing before if another opening parenthesis is before this.
            // That way the first opening parenthesis will report any spacing errors.
            if (!prevTokenIsOpenParen && !precededByKeyword)
            {
                var hasLeadingComment = (leadingTriviaList.Count > 0) && leadingTriviaList.Last().IsKind(SyntaxKind.MultiLineCommentTrivia);
                var hasLeadingSpace = (leadingTriviaList.Count > 0) && leadingTriviaList.Last().IsKind(SyntaxKind.WhitespaceTrivia);

                if (!isFirstOnLine && !hasLeadingComment && (haveLeadingSpace != hasLeadingSpace))
                {
                    if (haveLeadingSpace)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DescriptorPreceded, token.GetLocation(), TokenSpacingProperties.InsertPreceding));
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPreceded, token.GetLocation(), TokenSpacingProperties.RemovePreceding));
                    }
                }
            }

            if (token.IsFollowedByWhitespace())
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), TokenSpacingProperties.RemoveFollowing));
            }
        }
    }
}
