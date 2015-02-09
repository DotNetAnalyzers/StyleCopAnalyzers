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
        private const string Title = "Interface names must begin with I";
        private const string MessageFormat = "Interface names must begin with I";
        private const string Category = "StyleCop.CSharp.NamingRules";
        private const string Description = "The name of a C# interface does not begin with the capital letter I.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1302.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

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
            context.RegisterSyntaxNodeAction(this.HandleInterfaceDeclarationSyntax, SyntaxKind.InterfaceDeclaration);
        }

        private void HandleInterfaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax) context.Node;
            if (interfaceDeclaration.Identifier.IsMissing)
                return;

            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(interfaceDeclaration))
            {
                return;
            }

            string name = interfaceDeclaration.Identifier.ValueText;
            if (name != null && !name.StartsWith("I", StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, interfaceDeclaration.Identifier.GetLocation()));
            }
        }
    }
}
