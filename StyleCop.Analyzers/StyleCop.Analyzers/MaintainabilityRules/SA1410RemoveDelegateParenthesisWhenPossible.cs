namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to a C# anonymous method does not contain any method parameters, yet the statement still includes
    /// parenthesis.
    /// </summary>
    /// <remarks>
    /// <para>When an anonymous method does not contain any method parameters, the parenthesis around the parameters are
    /// optional.</para>
    ///
    /// <para>A violation of this rule occurs when the parenthesis are present on an anonymous method call which takes
    /// no method parameters. For example:</para>
    ///
    /// <code language="csharp">
    /// this.Method(delegate() { return 2; });
    /// </code>
    ///
    /// <para>The parenthesis are unnecessary and should be removed:</para>
    ///
    /// <code language="csharp">
    /// this.Method(delegate { return 2; });
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1410RemoveDelegateParenthesisWhenPossible : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1410RemoveDelegateParenthesisWhenPossible"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1410";
        private const string Title = "Remove delegate parenthesis when possible";
        private const string MessageFormat = "Remove delegate parenthesis when possible";
        private const string Category = "StyleCop.CSharp.MaintainabilityRules";
        private const string Description = "A call to a C# anonymous method does not contain any method parameters, yet the statement still includes parenthesis.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1410.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

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
            context.RegisterSyntaxNodeAction(this.HandleAnonymousMethodExpressionSyntax, SyntaxKind.AnonymousMethodExpression);
        }

        private void HandleAnonymousMethodExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            AnonymousMethodExpressionSyntax syntax = context.Node as AnonymousMethodExpressionSyntax;
            if (syntax == null)
            {
                return;
            }

            // ignore if no parameter list exists
            if (syntax.ParameterList == null)
            {
                return;
            }

            // ignore if parameter list is not empty
            if (syntax.ParameterList.Parameters.Count > 0)
            {
                return;
            }

            // Remove delegate parenthesis when possible
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.ParameterList.GetLocation()));
        }
    }
}
