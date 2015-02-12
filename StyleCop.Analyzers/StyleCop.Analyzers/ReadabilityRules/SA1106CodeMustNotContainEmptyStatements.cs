namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# code contains an extra semicolon.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contain an extra semicolon. Syntactically, this results in
    /// an extra, empty statement in the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1106CodeMustNotContainEmptyStatements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1106CodeMustNotContainEmptyStatements"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1106";
        private const string Title = "Code must not contain empty statements";
        private const string MessageFormat = "Code must not contain empty statements";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The C# code contains an extra semicolon.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1106.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

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
            context.RegisterSyntaxNodeAction(this.HandleEmptyStatementSyntax, SyntaxKind.EmptyStatement);
        }

        private void HandleEmptyStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            EmptyStatementSyntax syntax = context.Node as EmptyStatementSyntax;
            if (syntax == null)
                return;

            LabeledStatementSyntax labeledStatementSyntax = syntax.Parent as LabeledStatementSyntax;
            if (labeledStatementSyntax != null)
            {
                BlockSyntax blockSyntax = labeledStatementSyntax.Parent as BlockSyntax;
                if (blockSyntax != null)
                {
                    for (int i = blockSyntax.Statements.Count - 1; i >= 0; i--)
                    {
                        StatementSyntax statement = blockSyntax.Statements[i];

                        // allow an empty statement to be used for a label, but only if no non-empty statements exist
                        // before the end of the block
                        if (blockSyntax.Statements[i] == labeledStatementSyntax)
                            return;

                        if (!statement.IsKind(SyntaxKind.EmptyStatement))
                            break;
                    }
                }
            }

            // Code must not contain empty statements
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation()));
        }
    }
}
