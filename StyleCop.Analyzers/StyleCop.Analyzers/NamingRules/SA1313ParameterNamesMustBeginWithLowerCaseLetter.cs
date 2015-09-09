namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The name of a parameter in C# does not begin with a lower-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a parameter does not begin with a lower-case letter.</para>
    ///
    /// <para>If the parameter name is intended to match the name of an item associated with Win32 or COM, and thus
    /// needs to begin with an upper-case letter, place the parameter within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1313ParameterNamesMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1313ParameterNamesMustBeginWithLowerCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1313";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1313Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1313MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1313Description), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1313.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleParameterSyntax, SyntaxKind.Parameter);
        }

        private static void HandleParameterSyntax(SyntaxNodeAnalysisContext context)
        {
            ParameterSyntax syntax = (ParameterSyntax)context.Node;
            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
            {
                return;
            }

            var identifier = syntax.Identifier;
            if (identifier.IsMissing)
            {
                return;
            }

            string name = identifier.ValueText;
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            {
                return;
            }

            if (NameMatchesAbstraction(syntax, context.SemanticModel))
            {
                return;
            }

            // Parameter names must begin with lower-case letter
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
        }

        private static bool NameMatchesAbstraction(ParameterSyntax syntax, SemanticModel semanticModel)
        {
            var parameterList = (ParameterListSyntax)syntax.Parent;
            var index = parameterList.Parameters.IndexOf(syntax);
            var declaringMember = syntax.Parent.Parent;

            if (!declaringMember.IsKind(SyntaxKind.MethodDeclaration))
            {
                return false;
            }

            var methodDeclaration = (MethodDeclarationSyntax)declaringMember;
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

            if (methodSymbol.IsOverride)
            {
                if (methodSymbol.OverriddenMethod.Parameters[index].Name == syntax.Identifier.ValueText)
                {
                    return true;
                }
            }
            else
            {
                var implementedInterfaces = methodSymbol.ContainingType.Interfaces;
                if (implementedInterfaces.Length != 0)
                {
                    foreach (var @interface in implementedInterfaces)
                    {
                        foreach (var member in @interface.GetMembers(methodSymbol.Name).OfType<IMethodSymbol>())
                        {
                            if (methodSymbol.ContainingType.FindImplementationForInterfaceMember(member).Equals(methodSymbol))
                            {
                                return member.Parameters[index].Name == syntax.Identifier.ValueText;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
