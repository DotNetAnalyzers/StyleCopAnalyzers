namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A Code Analysis SuppressMessage attribute does not include a justification.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a Code Analysis
    /// <see cref="SuppressMessageAttribute"/> attribute, but a justification for the suppression has not been provided
    /// within the attribute. Whenever a Code Analysis rule is suppressed, a justification should be provided. This can
    /// increase the long-term maintainability of the code.</para>
    ///
    /// <code language="csharp">
    /// [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Used during unit testing")]
    /// public bool Enable()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1404CodeAnalysisSuppressionMustHaveJustification : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1404CodeAnalysisSuppressionMustHaveJustification"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1404";
        private const string Title = "Code analysis suppression must have justification";
        private const string MessageFormat = "Code analysis suppression must have justification";
        private const string Category = "StyleCop.CSharp.MaintainabilityRules";
        private const string Description = "A Code Analysis SuppressMessage attribute does not include a justification.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1404.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttributeNode, SyntaxKind.Attribute);
        }

        private void HandleAttributeNode(SyntaxNodeAnalysisContext context)
        {
            var attribute = context.Node as AttributeSyntax;
            if (attribute != null)
            {
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);
                ISymbol symbol = symbolInfo.Symbol;
                if (symbol != null)
                {
                    var suppressMessageType = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(SuppressMessageAttribute).FullName);
                    if (symbol.ContainingType == suppressMessageType)
                    {

                        foreach (var argument in attribute.ArgumentList.ChildNodes())
                        {
                            var attributeArgument = argument as AttributeArgumentSyntax;
                            if (attributeArgument?.NameEquals?.Name?.Identifier.ValueText == nameof(SuppressMessageAttribute.Justification))
                            {
                                // Check if the justification is not empty
                                var value = context.SemanticModel.GetConstantValue(attributeArgument.Expression);

                                // If value does not have a value the expression is not constant -> Compilation error
                                if (!value.HasValue || !string.IsNullOrWhiteSpace(value.Value as string))
                                {
                                    return;
                                }

                                // Empty, Whitespace or null justification provided
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, attributeArgument.GetLocation()));
                                return;
                            }
                        }

                        // No justification set
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, attribute.GetLocation()));
                    }
                }
            }
        }
    }
}
