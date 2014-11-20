namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An increment or decrement symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around an increment or decrement symbol is not
    /// correct.</para>
    ///
    /// <para>There should be no whitespace between the increment or decrement symbol and the item that is being
    /// incremented or decremented.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1020";
        internal const string Title = "Increment Decrement Symbols Must Be Spaced Correctly";
        internal const string MessageFormat = "{0} symbol '{1}' must not be {2} by a space.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "An increment or decrement symbol within a C# element is not spaced correctly.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1020.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.CSharpKind())
                {
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                    HandleIncrementDecrementToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleIncrementDecrementToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            switch (token.Parent.CSharpKind())
            {
            case SyntaxKind.PreIncrementExpression:
            case SyntaxKind.PreDecrementExpression:
                if (token.HasTrailingTrivia)
                {
                    string symbolName;
                    if (token.IsKind(SyntaxKind.MinusMinusToken))
                        symbolName = "Decrement";
                    else
                        symbolName = "Increment";

                    // {Increment|Decrement} symbol '{++|--}' must not be {followed} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), symbolName, token.Text, "followed"));
                }

                break;

            case SyntaxKind.PostIncrementExpression:
            case SyntaxKind.PostDecrementExpression:
                SyntaxToken previousToken = token.GetPreviousToken();
                if (!previousToken.IsMissing && previousToken.HasTrailingTrivia)
                {
                    string symbolName;
                    if (token.IsKind(SyntaxKind.MinusMinusToken))
                        symbolName = "Decrement";
                    else
                        symbolName = "Increment";

                    // {Increment|Decrement} symbol '{++|--}' must not be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), symbolName, token.Text, "preceded"));
                }
                break;

            default:
                return;
            }
        }
    }
}
