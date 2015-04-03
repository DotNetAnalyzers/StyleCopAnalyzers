namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An implicitly typed new array allocation within a C# code file is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains an implicitly typed new array allocation which
    /// is not spaced correctly. Within an implicitly typed new array allocation, there should not be any space between
    /// the new keyword and the opening array bracket. For example:</para>
    ///
    /// <code language="cs">
    /// var a = new[] { 1, 10, 100, 1000 };
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1026";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1026Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1026MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string Category = "StyleCop.CSharp.SpacingRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1026Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1026.html";

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
                case SyntaxKind.NewKeyword:
                    this.HandleNewKeywordToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleNewKeywordToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (!token.Parent.IsKind(SyntaxKind.ImplicitArrayCreationExpression))
            {
                return;
            }

            this.HandleDisallowedSpaceToken(context, token);
        }

        private void HandleDisallowedSpaceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing || !token.HasTrailingTrivia)
            {
                return;
            }

            if (!token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }
    }
}
