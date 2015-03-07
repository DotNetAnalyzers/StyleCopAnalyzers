namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A static readonly element is positioned beneath a static non-readonly element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a static readonly element is positioned beneath a static non-readonly
    /// element of the same type.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1214";
        private const string Title = "Static readonly elements must appear before static non-readonly elements";
        private const string MessageFormat = "Static readonly elements must appear before static non-readonly elements.";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "A static readonly element is positioned beneath a static non-readonly element of the same type.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1214.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(this.AnalyzeClass, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(this.AnalyzeStruct, SyntaxKind.StructDeclaration);
        }

        private void AnalyzeStruct(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            AnalyzeType(context, structDeclaration);
        }

        private void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax) context.Node;

            AnalyzeType(context, classDeclaration);
        }

        private static void AnalyzeType(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
        {
            var staticFields = typeDeclaration.Members
                .OfType<BaseFieldDeclarationSyntax>()
                .Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
                .ToList();

            var endLocationsOfStaticNonReadonlyFields =
                staticFields.Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)) == false)
                    .Select(f => f.GetLocation().GetLineSpan())
                    .Where(l => l.IsValid)
                    .Select(l => l.EndLinePosition)
                    .ToList();

            var readonlyFieldLocations =
                staticFields.Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)))
                    .Where(f => f.GetLocation().GetLineSpan().IsValid)
                    .Where(f => f.Declaration != null && f.Declaration.Variables.Any())
                    .Select(f => new {
                        FieldLocations = f.Declaration.Variables
                                                      .Where(v => !v.IsMissing && !v.Identifier.IsMissing)
                                                      .Select(v => v.Identifier.GetLocation())
                                                      .ToList(),
                        FieldEndLinePosition = f.GetLocation().GetLineSpan().EndLinePosition
                    })
                    .ToList();

            foreach (var location in readonlyFieldLocations)
            {
                if (endLocationsOfStaticNonReadonlyFields.Any(e => e < location.FieldEndLinePosition))
                {
                    foreach (var fieldLocation in location.FieldLocations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, fieldLocation));
                    }
                }
            }
        }
    }
}
