namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;


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
    public class SA1513ClosingCurlyBracketMustBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1513ClosingCurlyBracketMustBeFollowedByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1513";
        private const string Title = "Closing curly bracket must be followed by blank line";
        private const string MessageFormat = "Closing curly bracket must be followed by blank line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "A closing curly bracket within a C# element, statement, or expression is not followed by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1513.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTreeAction);
        }

        private async void HandleSyntaxTreeAction(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = await context.Tree.GetRootAsync(context.CancellationToken).ConfigureAwait(false);

            var visitor = new CurlyBracketsVisitor(context);
            visitor.Visit(syntaxRoot);
        }

        private class CurlyBracketsVisitor : CSharpSyntaxWalker
        {
            private SyntaxTreeAnalysisContext context;
            private Stack<SyntaxToken> curlyBracketsQueue = new Stack<SyntaxToken>();

            public CurlyBracketsVisitor(SyntaxTreeAnalysisContext context)
                : base(SyntaxWalkerDepth.Token)
            {
                this.context = context;
            }

            public override void VisitToken(SyntaxToken token)
            {
                if (token.IsKind(SyntaxKind.OpenBraceToken))
                {
                    this.curlyBracketsQueue.Push(token);
                }
                else if (token.IsKind(SyntaxKind.CloseBraceToken))
                {
                    this.AnalyzeCloseBrace(token);

                    this.curlyBracketsQueue.Pop();
                }

                base.VisitToken(token);
            }

            private void AnalyzeCloseBrace(SyntaxToken token)
            {
                var nextToken = token.GetNextToken(true, true);

                if (nextToken.HasLeadingTrivia && this.HasLeadingBlankLine(nextToken.LeadingTrivia))
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

                    if (this.IsPartOf<QueryExpressionSyntax>(token) && ((nextToken.Parent is QueryClauseSyntax) || (nextToken.Parent is SelectOrGroupClauseSyntax)))
                    {
                        // the close brace is part of a query expression
                        return;
                    }

                    if (this.IsPartOf<ArgumentListSyntax>(token))
                    {
                        // the close brace is part of an object initializer, anonymous function or lambda expression within an argument list.
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.SemicolonToken) &&
                        (this.IsPartOf<VariableDeclaratorSyntax>(token) ||
                         this.IsPartOf<AssignmentExpressionSyntax>(token) ||
                         this.IsPartOf<ReturnStatementSyntax>(token)))
                    {
                        // the close brace is part of a variable initialization statement or a return statement
                        return;
                    }

                    if (nextToken.IsKind(SyntaxKind.CommaToken) &&
                        (this.IsPartOf<InitializerExpressionSyntax>(token) ||
                         this.IsPartOf<AnonymousObjectCreationExpressionSyntax>(token)))
                    {
                        // the close brace is part of an initializer statement.
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

                    if (nextToken.IsKind(SyntaxKind.EndOfFileToken))
                    {
                        // this is the last close brace in the file
                        return;
                    }
                }

                if (this.StartsWithDirectiveTrivia(nextToken.LeadingTrivia))
                {
                    // the close brace is followed by directive trivia.
                    return;
                }

                var location = Location.Create(this.context.Tree, TextSpan.FromBounds(token.Span.End, nextToken.FullSpan.Start));
                this.context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }

            private bool HasLeadingBlankLine(SyntaxTriviaList triviaList)
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

            private bool IsOnSameLineAsOpeningBrace(SyntaxToken closeBrace)
            {
                var matchingOpenBrace = this.curlyBracketsQueue.Peek();
                return matchingOpenBrace.GetLocation().GetLineSpan().EndLinePosition.Line == closeBrace.GetLocation().GetLineSpan().StartLinePosition.Line;
            }

            private bool StartsWithDirectiveTrivia(SyntaxTriviaList triviaList)
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

            private bool IsQueryClause(SyntaxToken token)
            {
                return (token.Parent is FromClauseSyntax) ||
                       (token.Parent is GroupClauseSyntax);
            }

            private bool IsPartOf<T>(SyntaxToken token)
            {
                var result = false;

                for (var current = token.Parent; !result && (current != null); current = current.Parent)
                {
                    result = current is T;
                }

                return result;
            }
        }
    }
}
