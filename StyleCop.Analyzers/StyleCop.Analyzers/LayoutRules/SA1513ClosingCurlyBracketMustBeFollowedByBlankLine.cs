// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A closing curly bracket within a C# element, statement, or expression is not followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a closing curly bracket is not followed by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         return this.enabled;
    ///     }}
    /// </code>
    ///
    /// <para>The code above would generate one instance of this violation, since there is one place where a closing
    /// curly bracket is not followed by a blank line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1513ClosingCurlyBracketMustBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1513ClosingCurlyBracketMustBeFollowedByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1513";
        private const string Title = "Closing curly bracket must be followed by blank line";
        private const string MessageFormat = "Closing curly bracket must be followed by blank line";
        private const string Description = "A closing curly bracket within a C# element, statement, or expression is not followed by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1513.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            var visitor = new CurlyBracketsVisitor(context);
            visitor.Visit(syntaxRoot);
        }

        private class CurlyBracketsVisitor : CSharpSyntaxWalker
        {
            private readonly SyntaxTreeAnalysisContext context;
            private readonly Stack<SyntaxToken> curlyBracketsStack = new Stack<SyntaxToken>();

            public CurlyBracketsVisitor(SyntaxTreeAnalysisContext context)
                : base(SyntaxWalkerDepth.Token)
            {
                this.context = context;
            }

            public override void VisitToken(SyntaxToken token)
            {
                if (token.IsKind(SyntaxKind.OpenBraceToken))
                {
                    this.curlyBracketsStack.Push(token);
                }
                else if (token.IsKind(SyntaxKind.CloseBraceToken))
                {
                    this.AnalyzeCloseBrace(token);

                    this.curlyBracketsStack.Pop();
                }

                base.VisitToken(token);
            }

            private static bool HasLeadingBlankLine(SyntaxTriviaList triviaList)
            {
                foreach (var trivia in triviaList)
                {
                    switch (trivia.Kind())
                    {
                    case SyntaxKind.WhitespaceTrivia:
                        // ignore
                        break;

                    case SyntaxKind.EndOfLineTrivia:
                        return true;

                    default:
                        return false;
                    }
                }

                return false;
            }

            private static bool StartsWithDirectiveTrivia(SyntaxTriviaList triviaList)
            {
                foreach (var trivia in triviaList)
                {
                    switch (trivia.Kind())
                    {
                    case SyntaxKind.WhitespaceTrivia:
                        // ignore
                        break;

                    default:
                        return trivia.IsDirective;
                    }
                }

                return false;
            }

            private static bool IsQueryClause(SyntaxToken token)
            {
                return (token.Parent is FromClauseSyntax) ||
                       (token.Parent is GroupClauseSyntax);
            }

            private static bool IsPartOf<T>(SyntaxToken token)
            {
                var result = false;

                for (var current = token.Parent; !result && (current != null); current = current.Parent)
                {
                    result = current is T;
                }

                return result;
            }

            private void AnalyzeCloseBrace(SyntaxToken token)
            {
                var nextToken = token.GetNextToken(true, true);

                if (nextToken.HasLeadingTrivia && HasLeadingBlankLine(nextToken.LeadingTrivia))
                {
                    // the close brace has a trailing blank line
                    return;
                }

                if (this.IsOnSameLineAsOpeningBrace(token))
                {
                    // the close brace is on the same line as the corresponding opening token
                    return;
                }

                if ((token.Parent is BlockSyntax) && (token.Parent.Parent is DoStatementSyntax))
                {
                    // the close brace is part of do ... while statement
                    return;
                }

                // check if the next token is not preceded by significant trivia.
                if (nextToken.LeadingTrivia.All(trivia => trivia.IsKind(SyntaxKind.WhitespaceTrivia)))
                {
                    if (nextToken.IsKind(SyntaxKind.DotToken))
                    {
                        // the close brace is followed by a member accessor on the next line
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.CloseBraceToken))
                    {
                        // the close brace is followed by another close brace on the next line
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.CatchKeyword) || nextToken.IsKind(SyntaxKind.FinallyKeyword))
                    {
                        // the close brace is followed by catch or finally statement
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.ElseKeyword))
                    {
                        // the close brace is followed by else (no need to check for if -> the compiler will handle that)
                        return;
                    }

                    if (IsPartOf<QueryExpressionSyntax>(token) && ((nextToken.Parent is QueryClauseSyntax) || (nextToken.Parent is SelectOrGroupClauseSyntax)))
                    {
                        // the close brace is part of a query expression
                        return;
                    }

                    if (IsPartOf<ArgumentListSyntax>(token))
                    {
                        // the close brace is part of an object initializer, anonymous function or lambda expression within an argument list.
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.SemicolonToken) &&
                        (IsPartOf<VariableDeclaratorSyntax>(token) ||
                         IsPartOf<YieldStatementSyntax>(token) ||
                         IsPartOf<ArrowExpressionClauseSyntax>(token) ||
                         IsPartOf<EqualsValueClauseSyntax>(token) ||
                         IsPartOf<AssignmentExpressionSyntax>(token) ||
                         IsPartOf<ReturnStatementSyntax>(token) ||
                         IsPartOf<ObjectCreationExpressionSyntax>(token)))
                    {
                        // the close brace is part of a variable initialization statement or a return statement
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.CommaToken) &&
                        (IsPartOf<InitializerExpressionSyntax>(token) ||
                         IsPartOf<AnonymousObjectCreationExpressionSyntax>(token)))
                    {
                        // the close brace is part of an initializer statement.
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.ColonToken))
                    {
                        // the close brace is in the first part of a conditional expression.
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.AddKeyword)
                        || nextToken.IsKind(SyntaxKind.RemoveKeyword)
                        || nextToken.IsKind(SyntaxKind.GetKeyword)
                        || nextToken.IsKind(SyntaxKind.SetKeyword))
                    {
                        // the close brace is followed by an accessor (SA1516 will handle that)
                        return;
                    }

                    var parenthesizedExpressionSyntax = nextToken.Parent as ParenthesizedExpressionSyntax;
                    if (parenthesizedExpressionSyntax?.CloseParenToken == nextToken)
                    {
                        // the close brace is followed by the closing paren of a parenthesized expression.
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.EndOfFileToken))
                    {
                        // this is the last close brace in the file
                        return;
                    }
                }

                if (StartsWithDirectiveTrivia(nextToken.LeadingTrivia))
                {
                    // the close brace is followed by directive trivia.
                    return;
                }

                var location = Location.Create(this.context.Tree, TextSpan.FromBounds(token.Span.End, nextToken.FullSpan.Start));
                this.context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }

            private bool IsOnSameLineAsOpeningBrace(SyntaxToken closeBrace)
            {
                var matchingOpenBrace = this.curlyBracketsStack.Peek();
                return matchingOpenBrace.GetLineSpan().EndLinePosition.Line == closeBrace.GetLineSpan().StartLinePosition.Line;
            }
        }
    }
}
