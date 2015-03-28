﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A closing square bracket within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing square bracket is not correct.</para>
    ///
    /// <para>A closing square bracket must never be preceded by whitespace, unless it is the first character on the
    /// line.</para>
    /// 
    /// <para>A closing square bracket must be followed by whitespace, unless it is the last character on the line, it
    /// is followed by a closing bracket or an opening parenthesis, it is followed by a comma or semicolon, or it is
    /// followed by certain types of operator symbols.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1011ClosingSquareBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1011ClosingSquareBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1011";
        private const string Title = "Closing square brackets must be spaced correctly";
        private const string MessageFormat = "Closing square bracket must{0} be {1} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "A closing square bracket within a C# statement is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1011.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.Kind())
                {
                case SyntaxKind.CloseBracketToken:
                    this.HandleCloseBracketToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleCloseBracketToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            // attribute brackets are handled separately
            if (token.Parent.IsKind(SyntaxKind.AttributeList))
            {
                return;
            }

            bool precededBySpace;
            bool firstInLine;

            bool followedBySpace;
            bool lastInLine;
            bool precedesSpecialCharacter;

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (firstInLine)
            {
                precededBySpace = true;
            }
            else
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                precededBySpace = precedingToken.HasTrailingTrivia;
            }

            followedBySpace = token.HasTrailingTrivia;
            lastInLine = followedBySpace && token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia);
            if (!followedBySpace && !lastInLine)
            {
                SyntaxToken nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.SemicolonToken:
                // TODO: "certain types of operator symbols"
                case SyntaxKind.DotToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.CloseParenToken:
                    precedesSpecialCharacter = true;
                    break;

                case SyntaxKind.GreaterThanToken:
                    precedesSpecialCharacter = nextToken.Parent.IsKind(SyntaxKind.TypeArgumentList);
                    break;

                case SyntaxKind.QuestionToken:
                    precedesSpecialCharacter = nextToken.Parent.IsKind(SyntaxKind.ConditionalAccessExpression);
                    break;

                default:
                    precedesSpecialCharacter = false;
                    break;
                }
            }
            else
            {
                precedesSpecialCharacter = false;
            }

            if (!firstInLine && precededBySpace)
            {
                // Closing square bracket must{ not} be {preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "preceded"));
            }

            if (!lastInLine)
            {
                if (!precedesSpecialCharacter && !followedBySpace)
                {
                    // Closing square bracket must{} be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "followed"));
                }
                else if (precedesSpecialCharacter && followedBySpace)
                {
                    // Closing square bracket must{ not} be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "followed"));
                }
            }
        }
    }
}
