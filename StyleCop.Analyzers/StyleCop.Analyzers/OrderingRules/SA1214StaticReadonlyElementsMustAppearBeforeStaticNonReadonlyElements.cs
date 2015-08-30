namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        private const string MessageFormat = "All {0} static readonly fields must appear before {0} static non-readonly fields.";
        private const string Description = "A static readonly element is positioned beneath a static non-readonly element of the same type.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1214.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            AnalyzeType(context, typeDeclaration);
        }

        private static void AnalyzeType(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
        {
            var previousFieldReadonly = true;
            var previousAccessLevel = AccessLevel.NotSpecified;
            var previousMemberStatic = true;
            foreach (var member in typeDeclaration.Members)
            {
                var field = member as FieldDeclarationSyntax;
                if (field == null)
                {
                    continue;
                }

                var currentFieldReadonly = field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                var currentAccessLevel = AccessLevelHelper.GetAccessLevel(field.Modifiers);
                currentAccessLevel = currentAccessLevel == AccessLevel.NotSpecified ? AccessLevel.Private : currentAccessLevel;
                var currentMemberStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword);
                if (currentAccessLevel == previousAccessLevel
                    && currentMemberStatic
                    && previousMemberStatic
                    && currentFieldReadonly
                    && !previousFieldReadonly)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(field), AccessLevelHelper.GetName(currentAccessLevel)));
                }

                previousFieldReadonly = currentFieldReadonly;
                previousAccessLevel = currentAccessLevel;
                previousMemberStatic = currentMemberStatic;
            }
        }
    }
}
