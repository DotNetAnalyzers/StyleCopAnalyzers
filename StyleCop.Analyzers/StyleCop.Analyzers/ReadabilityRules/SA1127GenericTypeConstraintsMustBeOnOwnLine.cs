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
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1127.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (MethodDeclarationSyntax)context.Node;
            var declarationLineSpan = declaration.GetLineSpan();

            if (declaration.TypeParameterList?.Parameters.Count > 0)
            {
                Analyze(context, declarationLineSpan, declaration.ConstraintClauses);
            }
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (TypeDeclarationSyntax)context.Node;
            var declarationLineSpan = declaration.GetLineSpan();

            if (declaration.TypeParameterList?.Parameters.Count > 0)
            {
                Analyze(context, declarationLineSpan, declaration.ConstraintClauses);
            }
        }

        private static void Analyze(
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
