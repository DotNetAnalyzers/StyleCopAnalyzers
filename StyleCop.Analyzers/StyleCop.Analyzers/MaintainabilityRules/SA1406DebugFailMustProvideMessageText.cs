namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// A call to <see cref="O:System.Diagnostics.Debug.Fail"/> in C# code does not include a descriptive message.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a call to
    /// <see cref="O:System.Diagnostics.Debug.Fail"/> which does not provide a description for the end-user. For
    /// example, the following call includes a description message:</para>
    ///
    /// <code language="csharp">
    /// Debug.Fail("The code should never reach this point.");
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1406DebugFailMustProvideMessageText : SystemDiagnosticsDebugDiagnosticBase
    {
        public const string DiagnosticId = "SA1406";
        internal const string Title = "Debug.Fail must provide message text";
        internal const string MessageFormat = "Debug.Fail must provide message text";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A call to Debug.Fail in C# code does not include a descriptive message.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1406.html";

        public static readonly DiagnosticDescriptor Descriptor =
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
            context.RegisterSyntaxNodeAction(HandleMethodCall, SyntaxKind.InvocationExpression);
        }

        private void HandleMethodCall(SyntaxNodeAnalysisContext context)
        {
            // Debug.Fail is not availible in a portable library. So no nameof(Debug.Fail) here
            HandleMethodCall(context, "Fail", 0, Descriptor);
        }
    }
}
