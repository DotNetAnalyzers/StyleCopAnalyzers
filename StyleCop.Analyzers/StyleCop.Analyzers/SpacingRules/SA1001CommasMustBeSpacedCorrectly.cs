namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The spacing around a comma is incorrect, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a comma is incorrect.</para>
    ///
    /// <para>A comma should always be followed by a single space, unless it is the last character on the line, and a
    /// comma should never be preceded by any whitespace, unless it is the first character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1001CommasMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1001";
        private const string Title = "Commas must be spaced correctly";
        private const string MessageFormat = "Commas must{0} be {1} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "The spacing around a comma is incorrect, within a C# code file.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1001.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
                case SyntaxKind.CommaToken:
                    this.HandleCommaToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleCommaToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            // check for a following space
            bool missingFollowingSpace = true;
            if (token.HasTrailingTrivia)
            {
                if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                    missingFollowingSpace = false;
                else if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                    missingFollowingSpace = false;
            }
            else
            {
                SyntaxToken nextToken = token.GetNextToken();
                if (nextToken.IsKind(SyntaxKind.CommaToken) || nextToken.IsKind(SyntaxKind.GreaterThanToken))
                {
                    // make an exception for things like typeof(Func<,>) and typeof(Func<,,>)
                    missingFollowingSpace = false;
                }
            }

            bool hasPrecedingSpace = false;
            if (!token.IsFirstTokenOnLine(context.CancellationToken))
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.HasTrailingTrivia)
                    hasPrecedingSpace = true;
            }

            if (missingFollowingSpace)
            {
                // comma must{} be {followed} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "followed"));
            }

            if (hasPrecedingSpace)
            {
                // comma must{ not} be {preceded} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), " not", "preceded"));
            }
        }
    }
}
