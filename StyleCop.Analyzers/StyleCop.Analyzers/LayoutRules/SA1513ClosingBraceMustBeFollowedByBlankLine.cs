// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A closing brace within a C# element, statement, or expression is not followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a closing brace is not followed by a blank line. For example:</para>
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
    /// brace is not followed by a blank line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1513ClosingBraceMustBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1513ClosingBraceMustBeFollowedByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1513";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1513.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1513Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1513MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1513Description), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            var visitor = new BracesVisitor(context);
            visitor.Visit(syntaxRoot);
        }

        private class BracesVisitor : CSharpSyntaxWalker
        {
            private readonly SyntaxTreeAnalysisContext context;
            private readonly Stack<SyntaxToken> bracesStack = new Stack<SyntaxToken>();

            public BracesVisitor(SyntaxTreeAnalysisContext context)
                : base(SyntaxWalkerDepth.Token)
            {
                this.context = context;
            }

            public override void VisitToken(SyntaxToken token)
            {
                if (token.IsKind(SyntaxKind.OpenBraceToken))
                {
                    this.bracesStack.Push(token);
                }
                else if (token.IsKind(SyntaxKind.CloseBraceToken))
                {
                    this.AnalyzeCloseBrace(token);

                    this.bracesStack.Pop();
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

            private static bool StartsWithSpecialComment(SyntaxTriviaList triviaList)
            {
                foreach (var trivia in triviaList)
                {
                    switch (trivia.Kind())
                    {
                    case SyntaxKind.WhitespaceTrivia:
                        // ignore
                        break;

                    case SyntaxKind.SingleLineCommentTrivia:
                        return trivia.ToFullString().StartsWith("////", StringComparison.Ordinal);

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
                if (token.Parent.IsKind(SyntaxKind.Interpolation))
                {
                    // The text after an interpolation is part of a string literal, and therefore does not require a
                    // blank line in source.
                    return;
                }

                var nextToken = token.GetNextToken(true, true);

                if (nextToken.HasLeadingTrivia
                    && (HasLeadingBlankLine(nextToken.LeadingTrivia) || StartsWithSpecialComment(nextToken.LeadingTrivia)))
                {
                    // the close brace has a trailing blank line or is followed by a single line comment that starts with 4 slashes.
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

                    if (IsPartOf<QueryExpressionSyntax>(token))
                    {
                        if (nextToken.Parent is QueryClauseSyntax
                            || nextToken.Parent is SelectOrGroupClauseSyntax
                            || nextToken.Parent is QueryContinuationSyntax)
                        {
                            // the close brace is part of a query expression
                            return;
                        }
                    }

                    if (nextToken.IsKind(SyntaxKind.SemicolonToken) &&
                        (IsPartOf<VariableDeclaratorSyntax>(token) ||
                         IsPartOf<YieldStatementSyntax>(token) ||
                         IsPartOf<ArrowExpressionClauseSyntax>(token) ||
                         IsPartOf<EqualsValueClauseSyntax>(token) ||
                         IsPartOf<AssignmentExpressionSyntax>(token) ||
                         IsPartOf<ReturnStatementSyntax>(token) ||
                         IsPartOf<ThrowStatementSyntax>(token) ||
                         IsPartOf<ObjectCreationExpressionSyntax>(token)))
                    {
                        // the close brace is part of a variable initialization statement or a return/throw statement
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.CommaToken) || nextToken.IsKind(SyntaxKind.CloseParenToken))
                    {
                        // The close brace is the end of an object initializer, anonymous function, lambda expression, etc.
                        // Comma and close parenthesis never requires a preceeding blank line.
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
                        || nextToken.IsKind(SyntaxKind.SetKeyword)
                        || nextToken.IsKind(SyntaxKindEx.InitKeyword))
                    {
                        // the close brace is followed by an accessor (SA1516 will handle that)
                        return;
                    }

                    if ((nextToken.IsKind(SyntaxKind.PrivateKeyword)
                        || nextToken.IsKind(SyntaxKind.ProtectedKeyword)
                        || nextToken.IsKind(SyntaxKind.InternalKeyword))
                        && (nextToken.Parent is AccessorDeclarationSyntax))
                    {
                        // the close brace is followed by an accessor with an accessibility restriction.
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
                var matchingOpenBrace = this.bracesStack.Peek();
                return matchingOpenBrace.GetLineSpan().EndLinePosition.Line == closeBrace.GetLineSpan().StartLinePosition.Line;
            }
        }
    }
}
