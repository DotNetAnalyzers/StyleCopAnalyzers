namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An instance readonly element is positioned beneath an instance non-readonly element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an instance readonly element is positioned beneath an instance
    /// non-readonly element of the same type.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1215";
        private const string Title = "Instance readonly elements must appear before instance non-readonly elements";
        private const string MessageFormat = "Instance readonly elements must appear before instance non-readonly elements.";
        private const string Description = "An instance readonly element is positioned beneath an instance non-readonly element of the same type.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1215.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            var previousFieldReadonly = true;
            foreach (var member in typeDeclaration.Members)
            {
                var field = member as FieldDeclarationSyntax;
                if (field == null || field.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    continue;
                }

                var currentFieldReadonly = field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                if (currentFieldReadonly && !previousFieldReadonly)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(field)));
                }

                previousFieldReadonly = currentFieldReadonly;
            }
        }
    }
}
