namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// TODO.
    /// </summary>
    /// <remarks>
    /// <para>TODO</para>
    ///
    /// <para>A violation of this rule occurs when unnecessary parenthesis have been used in an attribute constructor.
    /// For example:</para>
    ///
    /// <code language="csharp">
    /// [Serializable()]
    /// </code>
    /// <para>The parenthesis are unnecessary and should be removed:</para>
    /// <code language="csharp">
    /// [Serializable]
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1411";
        internal const string Title = "Attribute constructor must not use unnecessary parenthesis";
        internal const string MessageFormat = "Attribute constructor must not use unnecessary parenthesis";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "TODO.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1411.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

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
            context.RegisterSyntaxNodeAction(HandleAttributeArgumentListSyntax, SyntaxKind.AttributeArgumentList);
        }

        private void HandleAttributeArgumentListSyntax(SyntaxNodeAnalysisContext context)
        {
            AttributeArgumentListSyntax syntax = context.Node as AttributeArgumentListSyntax;
            if (syntax.Arguments.Count != 0)
                return;

            // Attribute constructor must not use unnecessary parenthesis
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation()));
        }
    }
}
