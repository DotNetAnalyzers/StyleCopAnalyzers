namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Diagnostics;




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
        public const string DiagnosticId = "SA1405";
        internal const string Title = "Debug.Assert must provide message text";
        internal const string MessageFormat = "Debug.Assert must provide message text";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A call to Debug.Assert in C# code does not include a descriptive message.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1405.html";

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
            context.RegisterSyntaxNodeAction(HandleMethodCall, SyntaxKind.InvocationExpression);
        }

        private void HandleMethodCall(SyntaxNodeAnalysisContext context)
        {
            HandleMethodCall(context, nameof(Debug.Assert), 1, Descriptor);
        }
    }
}