namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1411";
        private const string Title = "Attribute constructor must not use unnecessary parenthesis";
        private const string MessageFormat = "Attribute constructor must not use unnecessary parenthesis";
        private const string Description = "TODO.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1411.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttributeArgumentListSyntax, SyntaxKind.AttributeArgumentList);
        }

        private void HandleAttributeArgumentListSyntax(SyntaxNodeAnalysisContext context)
        {
            AttributeArgumentListSyntax syntax = context.Node as AttributeArgumentListSyntax;
            if (syntax.Arguments.Count != 0)
            {
                return;
            }

            // Attribute constructor must not use unnecessary parenthesis
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation(), ArrayEx.Empty<object>()));
        }
    }
}
