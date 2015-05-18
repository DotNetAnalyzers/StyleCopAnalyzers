namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A closing attribute bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing attribute bracket is not correct.</para>
    ///
    /// <para>A closing attribute bracket should never be preceded by whitespace, unless the bracket is the first
    /// character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1017ClosingAttributeBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1017ClosingAttributeBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1017";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1017Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1017MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string Category = "StyleCop.CSharp.SpacingRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1017Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1017.html";

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

            if (!token.Parent.IsKind(SyntaxKind.AttributeList))
            {
                return;
            }

            bool hasPrecedingSpace = false;
            if (!token.HasLeadingTrivia)
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.HasTrailingTrivia)
                {
                    hasPrecedingSpace = true;
                }
            }

            if (hasPrecedingSpace)
            {
                // Closing attribute brackets must not be preceded by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
            }
        }
    }
}
