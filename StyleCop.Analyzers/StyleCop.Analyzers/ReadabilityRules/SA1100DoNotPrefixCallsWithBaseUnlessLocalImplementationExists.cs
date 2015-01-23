namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// A call to a member from an inherited class begins with <c>base.</c>, and the local class does not contain an
    /// override or implementation of the member.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a call to a member from the base class prefixed
    /// with <c>base.</c>, and there is no local implementation of the member. For example:</para>
    ///
    /// <code language="cs">
    /// string name = base.JoinName("John", "Doe");
    /// </code>
    ///
    /// <para>This rule is in place to prevent a potential source of bugs.Consider a base class which contains the
    /// following virtual method:</para>
    ///
    /// <code language="cs">
    /// public virtual string JoinName(string first, string last)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Another class inherits from this base class but does not provide a local override of this method.
    /// Somewhere within this class, the base class method is called using <c>base.JoinName(...)</c>. This works as
    /// expected. At a later date, someone adds a local override of this method to the class:</para>
    ///
    /// <code language="cs">
    /// public override string JoinName(string first, string last)
    /// {
    ///   return “Bob”;
    /// }
    /// </code>
    ///
    /// <para>At this point, the local call to <c>base.JoinName(...)</c> most likely introduces a bug into the code.
    /// This call will always call the base class method and will cause the local override to be ignored.</para>
    ///
    /// <para>For this reason, calls to members from a base class should not begin with <c>base.</c>, unless a local
    /// override is implemented, and the developer wants to specifically call the base class member. When there is no
    /// local override of the base class member, the call should be prefixed with <c>this.</c> rather than
    /// <c>base.</c>.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1100";
        internal const string Title = "Do not prefix calls with base unless local implementation exists";
        internal const string MessageFormat = "Do not prefix calls with base unless local implementation exists";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "A call to a member from an inherited class begins with 'base.', and the local class does not contain an override or implementation of the member.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1100.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(AnalyzeBaseExpression, SyntaxKind.BaseExpression);
        }

        private void AnalyzeBaseExpression(SyntaxNodeAnalysisContext context)
        {
            var baseExpressionSyntax = (BaseExpressionSyntax)context.Node;
            var targetSymbol = context.SemanticModel.GetSymbolInfo(baseExpressionSyntax.Parent, context.CancellationToken);
            if (targetSymbol.Symbol == null)
                return;

            // Replace the 'base.' with 'this.' and analyze the symbol again
            var tree = context.Node.SyntaxTree;
            var root = tree.GetRoot(context.CancellationToken);

            var testExpression = SyntaxFactory.ThisExpression().WithTriviaFrom(baseExpressionSyntax).WithoutFormatting();
            var testTree = tree.WithRootAndOptions(root.ReplaceNode(baseExpressionSyntax, testExpression), tree.Options);
            var testCompilation = context.SemanticModel.Compilation.ReplaceSyntaxTree(tree, testTree);

            var testSemanticModel = testCompilation.GetSemanticModel(testTree);
            var testRoot = testSemanticModel.SyntaxTree.GetRoot(context.CancellationToken);
            var testToken = testRoot.FindToken(context.Node.GetLocation().SourceSpan.Start);
            var testNode = testToken.Parent;
            var testSymbol = testSemanticModel.GetSymbolInfo(testNode.Parent, context.CancellationToken);
            if (testSymbol.Symbol == null)
                return;

            // If 'this.' caused the expression to resolve to a different symbol, then 'base.' is required
            string baseContainerName = targetSymbol.Symbol.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string thisContainerName = testSymbol.Symbol.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (baseContainerName != thisContainerName)
                return;

            // Do not prefix calls with base unless local implementation exists
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, baseExpressionSyntax.GetLocation()));
        }
    }
}
