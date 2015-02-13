namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

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
    public class SA1008OpeningParenthesisMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1008OpeningParenthesisMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1008";
        private const string Title = "Opening parenthesis must be spaced correctly";
        private const string MessageFormat = "Opening parenthesis must{0} be {1} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "An opening parenthesis within a C# statement is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1008.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
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
                switch (token.CSharpKind())
                {
                case SyntaxKind.OpenParenToken:
                    this.HandleOpenParenToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleOpenParenToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            bool precededBySpace;
            bool firstInLine;
            bool allowLeadingSpace;
            bool allowLeadingNoSpace;
            bool reportPrecedingError = true;

            bool followedBySpace;
            bool lastInLine;
            bool reportFollowingError = true;

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (firstInLine)
            {
                precededBySpace = true;
                allowLeadingSpace = true;
                allowLeadingNoSpace = true;
            }
            else
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                precededBySpace = precedingToken.HasTrailingTrivia;
                switch (precedingToken.CSharpKind())
                {
                case SyntaxKind.AwaitKeyword:
                case SyntaxKind.CaseKeyword:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.ForEachKeyword:
                //case SyntaxKind.FromKeyword: // ?
                case SyntaxKind.GroupKeyword: // ?
                case SyntaxKind.IfKeyword:
                //case SyntaxKind.IntoKeyword: // ?
                //case SyntaxKind.JoinKeyword: // ?
                //case SyntaxKind.LetKeyword: // ?
                case SyntaxKind.LockKeyword:
                case SyntaxKind.OrderByKeyword: // ?
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.SelectKeyword: // ?
                //case SyntaxKind.StackAllocKeyword: // ?
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.WhereKeyword: // ?
                case SyntaxKind.WhileKeyword:
                    allowLeadingNoSpace = false;
                    allowLeadingSpace = true;
                    // allow these to be reported as SA1000
                    reportPrecedingError = false;
                    break;

                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.NameOfKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.TypeOfKeyword:
                case SyntaxKind.UncheckedKeyword:
                    allowLeadingNoSpace = true;
                    allowLeadingSpace = false;
                    // allow these to be reported as SA1000
                    reportPrecedingError = false;
                    break;

                case SyntaxKind.EqualsGreaterThanToken: // lambda containing a cast
                case SyntaxKind.CommaToken:
                    allowLeadingNoSpace = false;
                    allowLeadingSpace = true;
                    break;

                case SyntaxKind.IdentifierToken:
                    if (precedingToken.Text == "nameof")
                    {
                        if (precedingToken.Parent is IdentifierNameSyntax && precedingToken.Parent.Parent is InvocationExpressionSyntax)
                        {
                            // allow this to be reported as SA1000
                            // TODO: this code should not ignore nameof(...) which is not actually a Name Of Expression
                            goto case SyntaxKind.NameOfKeyword;
                        }
                    }

                    goto default;

                default:
                    if (precedingToken.Parent is BinaryExpressionSyntax
                        || precedingToken.Parent is AssignmentExpressionSyntax
                        || precedingToken.Parent is ConditionalExpressionSyntax
                        || precedingToken.Parent is EqualsValueClauseSyntax)
                    {
                        allowLeadingNoSpace = false;
                        allowLeadingSpace = true;
                        break;
                    }
                    else
                    {
                        allowLeadingNoSpace = true;
                        allowLeadingSpace = false;
                        break;
                    }
                }
            }

            followedBySpace = token.HasTrailingTrivia;
            lastInLine = followedBySpace && token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia);

            if (reportPrecedingError && !firstInLine)
            {
                if (!allowLeadingSpace && precededBySpace)
                {
                    // Opening parenthesis must{ not} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "preceded"));
                }
                else if (!allowLeadingNoSpace && !precededBySpace)
                {
                    // Opening parenthesis must{} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "preceded"));
                }
            }

            if (reportFollowingError && !lastInLine && followedBySpace)
            {
                // Opening parenthesis must{ not} be {followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "followed"));
            }
        }
    }
}
