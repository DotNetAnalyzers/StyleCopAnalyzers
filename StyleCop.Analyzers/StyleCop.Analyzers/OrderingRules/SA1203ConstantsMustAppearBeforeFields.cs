namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Helpers;

    /// <summary>
    /// A constant field is placed beneath a non-constant field.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a constant field is placed beneath a non-constant field. Constants
    /// must be placed above fields to indicate that the two are fundamentally different types of elements with
    /// different considerations for the compiler, different naming requirements, etc.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1203ConstantsMustAppearBeforeFields : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1203ConstantsMustAppearBeforeFields"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1203";
        private const string Title = "Constants must appear before fields";
        private const string MessageFormat = "Constants must appear before fields";
        private const string Description = "A constant field is placed beneath a non-constant field.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1203.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = context.Node as TypeDeclarationSyntax;
            var members = typeDeclaration.Members;
            bool nonConstFieldFound = false;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].IsKind(SyntaxKind.IncompleteMember))
                {
                    continue;
                }

                if (!members[i].IsKind(SyntaxKind.FieldDeclaration))
                {
                    continue;
                }

                var field = members[i] as FieldDeclarationSyntax;

                bool thisFieldIsConstant = false;
                foreach (var modifier in field.Modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.ConstKeyword))
                    {
                        thisFieldIsConstant = true;
                        break;
                    }
                }

                if (!thisFieldIsConstant)
                {
                    nonConstFieldFound = true;
                }

                if (thisFieldIsConstant && nonConstFieldFound)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(members[i])));
                }
            }
        }
    }
}
