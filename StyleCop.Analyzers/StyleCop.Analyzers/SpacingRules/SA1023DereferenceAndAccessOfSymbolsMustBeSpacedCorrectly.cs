namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A dereference symbol or an access-of symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a dereference or access-of symbol is not
    /// correct.</para>
    ///
    /// <para>The spacing around the symbol depends upon whether the symbol is used within a type declaration. If so,
    /// the symbol must always be followed by a single space, unless it is the last character on the line, or is
    /// followed by an opening square bracket or a parenthesis. In addition, the symbol should not be preceded by
    /// whitespace, and should not be the first character on the line. An example of a properly spaced dereference
    /// symbol used within a type declaration is:</para>
    ///
    /// <code language="cs">
    /// object* x = null;
    /// </code>
    ///
    /// <para>When a dereference or access-of symbol is used outside of a type declaration, the opposite rule applies.
    /// In this case, the symbol must always be preceded by a single space, unless it is the first character on the
    /// line, or is preceded by an opening square bracket, a parenthesis or a symbol of the same type i.e. an equals.
    /// The symbol should not be followed by whitespace, and should not be the last character on the line. For
    /// example:</para>
    ///
    /// <code language="cs">
    /// y = *x;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1023";
        private const string Title = "Dereference and access of symbols must be spaced correctly";
        private const string MessageFormat = "Dereference symbol '*' must {0}.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "A dereference symbol or an access-of symbol within a C# element is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1023.html";

        private static readonly DiagnosticDescriptor Descriptor =
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
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.CSharpKind())
                {
                case SyntaxKind.AsteriskToken:
                    this.HandleAsteriskToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleAsteriskToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            bool allowAtLineStart;
            bool allowAtLineEnd;
            bool allowPrecedingSpace;
            bool allowPrecedingNoSpace;
            bool allowTrailingSpace;
            bool allowTrailingNoSpace;
            switch (token.Parent.CSharpKind())
            {
            case SyntaxKind.PointerType:
                allowAtLineStart = false;
                allowAtLineEnd = true;
                allowPrecedingNoSpace = true;
                allowPrecedingSpace = false;
                allowTrailingNoSpace = true;
                allowTrailingSpace = true;
                break;

            case SyntaxKind.PointerIndirectionExpression:
                allowAtLineStart = true;
                allowAtLineEnd = false;
                allowPrecedingNoSpace = true;
                allowPrecedingSpace = true;
                allowTrailingNoSpace = true;
                allowTrailingSpace = false;
                break;

            default:
                return;
            }

            bool precededBySpace;
            bool firstInLine;

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

            bool followedBySpace = token.HasTrailingTrivia;
            bool lastInLine = followedBySpace && token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia);

            if (!allowAtLineStart && firstInLine)
            {
                // Dereference symbol '*' must {not appear at the beginning of a line}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "not appear at the beginning of a line"));
            }

            if (!allowAtLineEnd && lastInLine)
            {
                // Dereference symbol '*' must {not appear at the end of a line}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "not appear at the end of a line"));
            }

            if (!allowPrecedingSpace && precededBySpace)
            {
                // Dereference symbol '*' must {not be preceded by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "not be preceded by a space"));
            }
            else if (!allowPrecedingNoSpace && !precededBySpace)
            {
                // Dereference symbol '*' must {be preceded by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "be preceded by a space"));
            }

            if (!allowTrailingSpace && followedBySpace)
            {
                // Dereference symbol '*' must {not be followed by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "not be followed by a space"));
            }
            else if (!allowTrailingNoSpace && !followedBySpace)
            {
                // Dereference symbol '*' must {be followed by a space}.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), "be followed by a space"));
            }
        }
    }
}
