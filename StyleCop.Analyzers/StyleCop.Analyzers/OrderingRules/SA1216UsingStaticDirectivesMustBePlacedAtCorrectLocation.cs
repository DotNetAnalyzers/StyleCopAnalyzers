namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A static using directive is positioned at the wrong location.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A violation of this rule occurs when a using static directive is placed before a normal or an alias using directive. 
    /// Placing the using static directives together below normal and alias using-directives can make the code cleaner and easier to read,
    /// and can help make it easier to identify the static members used throughout the code.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1216UsingStaticDirectivesMustBePlacedAtCorrectLocation : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1216UsingStaticDirectivesMustBePlacedAtCorrectLocation"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1216";
        private const string Title = "Using static directives must be placed at correct location";
        private const string MessageFormat = "The using static directive must appear after regular or alias using directives";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "A using static directive is positioned before a regular or alias using directive.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/wiki/SA1216";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleUsingDirectiveSyntax, SyntaxKind.UsingDirective);
        }

        private void HandleUsingDirectiveSyntax(SyntaxNodeAnalysisContext context)
        {
            UsingDirectiveSyntax usingDirective = context.Node as UsingDirectiveSyntax;
            if (usingDirective.StaticKeyword.IsKind(SyntaxKind.None))
            {
                return;
            }

            SyntaxList<UsingDirectiveSyntax> usingDirectives;
            CompilationUnitSyntax compilationUnit = usingDirective.Parent as CompilationUnitSyntax;
            if (compilationUnit != null)
            {
                usingDirectives = compilationUnit.Usings;
            }
            else
            {
                var namespaceDeclaration = (NamespaceDeclarationSyntax)usingDirective.Parent;
                usingDirectives = namespaceDeclaration.Usings;
            }

            for (var i = usingDirectives.IndexOf(usingDirective) + 1; i < usingDirectives.Count; i++)
            {
                var currentUsingDirective = usingDirectives[i];
                if (currentUsingDirective.StaticKeyword.IsKind(SyntaxKind.None))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, usingDirective.GetLocation()));
                    break;
                }
            }
        }
    }
}
