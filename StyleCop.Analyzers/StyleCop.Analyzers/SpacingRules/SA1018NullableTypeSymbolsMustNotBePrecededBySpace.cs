namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A nullable type symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a nullable type symbol is not correct.</para>
    ///
    /// <para>A nullable type symbol should never be preceded by whitespace, unless the symbol is the first character on
    /// the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1018NullableTypeSymbolsMustNotBePrecededBySpace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1018";
        internal const string Title = "Nullable type symbols must be spaced correctly";
        internal const string MessageFormat = "Nullable type symbol must not be preceded by a space.";
        internal const string Category = "StyleCop.CSharp.SpacingRules";
        internal const string Description = "A nullable type symbol within a C# element is not spaced correctly.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1018.html";

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
                case SyntaxKind.QuestionToken:
                    HandleQuestionToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleQuestionToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            if (token.Parent.CSharpKind() != SyntaxKind.NullableType)
                return;

            bool hasPrecedingSpace = false;
            if (!token.HasLeadingTrivia)
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.HasTrailingTrivia)
                    hasPrecedingSpace = true;
            }

            if (hasPrecedingSpace)
            {
                // nullable type symbol must not be preceded by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
            }
        }
    }
}
