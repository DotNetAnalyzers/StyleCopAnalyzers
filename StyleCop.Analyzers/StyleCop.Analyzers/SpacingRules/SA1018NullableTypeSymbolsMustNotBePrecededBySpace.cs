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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1018";
        private const string Title = "Nullable type symbols must be spaced correctly";
        private const string MessageFormat = "Nullable type symbol must not be preceded by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "A nullable type symbol within a C# element is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1018.html";

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
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.Kind())
                {
                case SyntaxKind.QuestionToken:
                    this.HandleQuestionToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleQuestionToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (token.Parent.Kind() != SyntaxKind.NullableType)
            {
                return;
            }

            if (token.IsFirstTokenOnLine(context.CancellationToken))
            {
                return;
            }

            SyntaxToken precedingToken = token.GetPreviousToken();
            if (precedingToken.HasTrailingTrivia)
            {
                // nullable type symbol must not be preceded by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
            }
        }
    }
}
