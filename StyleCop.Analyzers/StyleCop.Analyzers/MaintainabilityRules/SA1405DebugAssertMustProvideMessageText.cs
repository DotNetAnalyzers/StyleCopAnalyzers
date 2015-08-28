namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to <see cref="O:System.Diagnostics.Debug.Assert"/> in C# code does not include a descriptive message.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a call to
    /// <see cref="O:System.Diagnostics.Debug.Assert"/> which does not provide a description for the end-user. For
    /// example, the following assert includes a description message:</para>
    ///
    /// <code language="csharp">
    /// Debug.Assert(value != true, "The value must always be true.");
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1405DebugAssertMustProvideMessageText : SystemDiagnosticsDebugDiagnosticBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1405DebugAssertMustProvideMessageText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1405";
        private const string Title = "Debug.Assert must provide message text";
        private const string MessageFormat = "Debug.Assert must provide message text";
        private const string Description = "A call to Debug.Assert in C# code does not include a descriptive message.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1405.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterCompilationStartAction(this.HandleCompilationStart);
        }

        private void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodCall, SyntaxKind.InvocationExpression);
        }

        private void HandleMethodCall(SyntaxNodeAnalysisContext context)
        {
            this.HandleMethodCall(context, nameof(Debug.Assert), 1, Descriptor);
        }
    }
}