namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A line within a documentation header above a C# element does not begin with a single space.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a line within a documentation header does not begin with a single
    /// space. For example:</para>
    ///
    /// <code language="cs">
    /// ///&lt;summary&gt;
    /// ///The summary text.
    /// ///&lt;/summary&gt;
    /// ///   &lt;param name="x"&gt;The document root.&lt;/param&gt;
    /// ///    &lt;param name="y"&gt;The Xml header token.&lt;/param&gt;
    /// private void Method1(int x, int y)
    /// {
    /// }
    /// </code>
    ///
    /// <para>The header lines should begin with a single space after the three leading forward slashes:</para>
    ///
    /// <code language="cs">
    /// /// &lt;summary&gt;
    /// /// The summary text.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="x"&gt;The document root.&lt;/param&gt;
    /// /// &lt;param name="y"&gt;The Xml header token.&lt;/param&gt;
    /// private void Method1(int x, int y)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1004DocumentationLinesMustBeginWithSingleSpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1004DocumentationLinesMustBeginWithSingleSpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1004";
        private const string Title = "Documentation lines must begin with single space";
        private const string MessageFormat = "Documentation line must begin with a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "A line within a documentation header above a C# element does not begin with a single space.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1004.html";

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
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (trivia.CSharpKind())
                {
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    this.HandleDocumentationCommentExteriorTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleDocumentationCommentExteriorTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            SyntaxToken token = trivia.Token;
            if (token.IsMissing)
                return;

            switch (token.CSharpKind())
            {
            case SyntaxKind.EqualsToken:
            case SyntaxKind.DoubleQuoteToken:
            case SyntaxKind.SingleQuoteToken:
            case SyntaxKind.IdentifierToken:
            case SyntaxKind.GreaterThanToken:
            case SyntaxKind.SlashGreaterThanToken:
            case SyntaxKind.LessThanToken:
            case SyntaxKind.LessThanSlashToken:
            case SyntaxKind.XmlCommentStartToken:
            case SyntaxKind.XmlCommentEndToken:
            case SyntaxKind.XmlCDataStartToken:
            case SyntaxKind.XmlCDataEndToken:
                if (!token.HasLeadingTrivia)
                    break;

                SyntaxTrivia lastLeadingTrivia = token.LeadingTrivia.Last();
                switch (lastLeadingTrivia.CSharpKind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    if (lastLeadingTrivia.ToFullString().StartsWith(" "))
                        return;

                    break;

                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    if (lastLeadingTrivia.ToFullString().EndsWith(" "))
                        return;

                    break;

                default:
                    break;
                }

                break;

            case SyntaxKind.EndOfDocumentationCommentToken:
            case SyntaxKind.XmlTextLiteralNewLineToken:
                return;

            case SyntaxKind.XmlTextLiteralToken:
                if (token.Text.StartsWith(" "))
                {
                    return;
                }
                else if (trivia.ToFullString().EndsWith(" "))
                {
                    // javadoc-style documentation comments without a leading * on one of the lines.
                    return;
                }

                break;

            default:
                break;
            }

            // Documentation line must begin with a space.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }
    }
}
