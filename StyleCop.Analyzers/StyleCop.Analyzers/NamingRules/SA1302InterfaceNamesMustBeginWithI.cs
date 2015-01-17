using StyleCop.Analyzers.Helpers;

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The name of a C# interface does not begin with the capital letter I.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of an interface does not begin with the capital letter I.
    /// Interface names should always begin with I. For example, <c>ICustomer</c>.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus cannot begin with the letter I, place the field or variable within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1302InterfaceNamesMustBeginWithI : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1302";
        internal const string Title = "Interface names must begin with I";
        internal const string MessageFormat = "Interface names must begin with I.";
        internal const string Category = "StyleCop.CSharp.NamingRules";
        internal const string Description = "The name of a C# interface does not begin with the capital letter I.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1302.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        private NamedTypeHelpers namedTypeHelpers = new NamedTypeHelpers();

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleInterfaceDeclarationSyntax, SyntaxKind.InterfaceDeclaration);
        }

        private void HandleInterfaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax) context.Node;

            var namedSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;

            if (namedSymbol == null || namedSymbol.TypeKind != TypeKind.Interface)
            {
                return;
            }

            if (namedTypeHelpers.IsContainedInNativeMethodsClass(namedSymbol.ContainingType))
            {
                return;
            }

            if (namedSymbol.Name != null && !namedSymbol.Name.StartsWith("I", StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, interfaceDeclaration.Identifier.GetLocation()));
            }
        }
    }
}
