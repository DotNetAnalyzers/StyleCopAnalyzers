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
            context.RegisterSyntaxNodeActionHonorExclusions(this.AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            AnalyzeType(context, typeDeclaration);
        }

        private static void AnalyzeType(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
        {
            var staticFields = typeDeclaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(f => f.Modifiers.Any(SyntaxKind.StaticKeyword))
                .ToList();

            var firstNonReadonlyField =
                staticFields.FirstOrDefault(f => !f.Modifiers.Any(SyntaxKind.ReadOnlyKeyword));

            if (firstNonReadonlyField == null)
            {
                return;
            }

            var firstNonReadonlyFieldLocation = firstNonReadonlyField.GetLocation().GetLineSpan();
            if (!firstNonReadonlyFieldLocation.IsValid)
            {
                return;
            }

            var readonlyFieldLocations =
                staticFields.Where(f => f.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                    .Where(f => f.GetLocation().GetLineSpan().IsValid)
                    .Where(f => f.Declaration != null && f.Declaration.Variables.Any())
                    .Select(f => new
                    {
                        FieldLocation = f.GetLocation(),
                        FieldEndLinePosition = f.GetLocation().GetLineSpan().EndLinePosition
                    })
                    .ToList();

            foreach (var location in readonlyFieldLocations)
            {
                if (firstNonReadonlyFieldLocation.EndLinePosition < location.FieldEndLinePosition)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location.FieldLocation));
                }
            }
        }
    }
}
