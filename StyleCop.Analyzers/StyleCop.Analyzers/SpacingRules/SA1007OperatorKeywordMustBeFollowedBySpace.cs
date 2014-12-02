namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The operator keyword within a C# operator overload method is not followed by any whitespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the operator keyword within an operator overload method is not
    /// followed by any whitespace. The operator keyword should always be followed by a single space. For
    /// example:</para>
    ///
    /// <code language="cs">
    /// public MyClass operator +(MyClass a, MyClass b)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1007OperatorKeywordMustBeFollowedBySpace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1007";
        internal const string Title = "Operator Keyword Must Be Followed By Space";
        internal const string MessageFormat = "Operator keyword must be followed by a space.";
        internal const string Category = "StyleCop.CSharp.SpacingRules";
        internal const string Description = "The operator keyword within a C# operator overload method is not followed by any whitespace.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1007.html";

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
                case SyntaxKind.OperatorKeyword:
                    HandleRequiredSpaceToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleRequiredSpaceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            if (token.HasTrailingTrivia)
            {
                if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                    return;

                if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                    return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }
    }
}
