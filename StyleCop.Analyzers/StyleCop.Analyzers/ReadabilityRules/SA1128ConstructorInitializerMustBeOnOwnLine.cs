namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Todo
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1128ConstructorInitializerMustBeOnOwnLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1128ConstructorInitializerMustBeOnOwnLine"/>
        /// </summary>
        public const string DiagnosticId = "SA1128";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1128Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1128MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1128Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1128.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConstructor, SyntaxKind.ConstructorDeclaration);
        }

        private void HandleConstructor(SyntaxNodeAnalysisContext context)
        {
            var constructor = (ConstructorDeclarationSyntax)context.Node;
            if (constructor.Initializer != null)
            {
                this.Analyze(context, constructor);
            }
        }

        private void Analyze(
            SyntaxNodeAnalysisContext context,
            ConstructorDeclarationSyntax constructor)
        {
            var initializer = constructor.Initializer;
            var colon = initializer.ColonToken;

            if (!colon.IsFirstInLine())
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.GetLocation()));
                return;
            }

            if (colon.TrailingTrivia.Any(t => t.IsKind(SyntaxKind.EndOfLineTrivia)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.GetLocation()));
            }
        }
    }
}
