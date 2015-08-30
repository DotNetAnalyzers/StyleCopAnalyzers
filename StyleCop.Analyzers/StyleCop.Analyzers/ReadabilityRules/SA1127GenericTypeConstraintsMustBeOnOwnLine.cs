namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Todo
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1127GenericTypeConstraintsMustBeOnOwnLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1127GenericTypeConstraintsMustBeOnOwnLine"/>
        /// </summary>
        public const string DiagnosticId = "SA1127";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1127Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1127MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1127Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1127.md";

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (MethodDeclarationSyntax)context.Node;
            var declarationLineSpan = declaration.GetLineSpan();

            if (declaration.TypeParameterList?.Parameters.Count > 0)
            {
                this.Analyze(context, declarationLineSpan, declaration.ConstraintClauses);
            }
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (TypeDeclarationSyntax)context.Node;
            var declarationLineSpan = declaration.GetLineSpan();

            if (declaration.TypeParameterList?.Parameters.Count > 0)
            {
                this.Analyze(context, declarationLineSpan, declaration.ConstraintClauses);
            }
        }

        private void Analyze(
            SyntaxNodeAnalysisContext context,
            FileLinePositionSpan declarationLineSpan,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            int currentLine = declarationLineSpan.StartLinePosition.Line;
            foreach (var constraint in constraintClauses)
            {
                int constraintLine = constraint.GetLineSpan().StartLinePosition.Line;
                if (currentLine == constraintLine)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, constraint.GetLocation()));
                }

                currentLine = constraintLine;
            }
        }
    }
}
