namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A static element is positioned beneath an instance element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a static element is positioned beneath an instance element of the
    /// same type. All static elements must be placed above all instance elements of the same type to make it easier to
    /// see the interface exposed from the instance and static version of the class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1204StaticElementsMustAppearBeforeInstanceElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1204StaticElementsMustAppearBeforeInstanceElements"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1204";
        private const string Title = "Static elements must appear before instance elements";
        private const string MessageFormat = "Static {0} must appear before instance {0}.";
        private const string Description = "A static element is positioned beneath an instance element of the same type.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1204.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.DelegateDeclaration] = "delegates",
            [SyntaxKind.EnumDeclaration] = "enums",
            [SyntaxKind.InterfaceDeclaration] = "interfaces",
            [SyntaxKind.StructDeclaration] = "structs",
            [SyntaxKind.ClassDeclaration] = "classes",
            [SyntaxKind.FieldDeclaration] = "fields",
            [SyntaxKind.EventDeclaration] = "events",
            [SyntaxKind.PropertyDeclaration] = "properties",
            [SyntaxKind.IndexerDeclaration] = "indexers",
            [SyntaxKind.MethodDeclaration] = "methods",
            [SyntaxKind.ConversionOperatorDeclaration] = "conversions",
            [SyntaxKind.OperatorDeclaration] = "operators"
        };

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDelcaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, compilationUnit.Members);
        }

        private static void HandleTypeDelcaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            HandleMemberList(context, typeDeclaration.Members);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
        {
            var previousSyntaxKind = SyntaxKind.None;
            var previousMemberStatic = true;
            var previousMemberConst = true;
            foreach (var member in members)
            {
                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;
                var modifiers = member.GetModifiers();
                var currentMemberStatic = modifiers.Any(SyntaxKind.StaticKeyword);
                var currentMemberConst = modifiers.Any(SyntaxKind.ConstKeyword);

                if (currentSyntaxKind == previousSyntaxKind
                    && !previousMemberStatic
                    && currentMemberStatic
                    && !previousMemberConst
                    && !currentMemberConst)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(member), MemberNames[currentSyntaxKind]));
                }

                previousSyntaxKind = currentSyntaxKind;
                previousMemberStatic = currentMemberStatic;
                previousMemberConst = currentMemberConst;
            }
        }
    }
}
