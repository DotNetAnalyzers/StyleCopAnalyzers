namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A field within a C# class has an access modifier other than private.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever a field in a class is given non-private access. For
    /// maintainability reasons, properties should always be used as the mechanism for exposing fields outside of a
    /// class, and fields should always be declared with private access. This allows the internal implementation of the
    /// property to change over time without changing the interface of the class.</para>
    ///
    /// <para>Fields located within C# structs are allowed to have any access level.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1401FieldsMustBePrivate : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1401";
        internal const string Title = "Fields must be private";
        internal const string MessageFormat = "Field must be private";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A field within a C# class has an access modifier other than private.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1401.html";

        public static readonly DiagnosticDescriptor Descriptor =
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
            context.RegisterSymbolAction(AnalyzeField,SymbolKind.Field);
        }

        private void AnalyzeField(SymbolAnalysisContext symbolAnalysisContext)
        {
            var fieldDeclarationSyntax = (IFieldSymbol)symbolAnalysisContext.Symbol;
            if (!IsFieldPrivate(fieldDeclarationSyntax) &&
                IsParentAClass(fieldDeclarationSyntax) &&
                !fieldDeclarationSyntax.IsConst)
            {
                symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(Descriptor, fieldDeclarationSyntax.Locations[0]));
            }
        }

        private bool IsFieldPrivate(IFieldSymbol fieldDeclarationSyntax)
        {
            return fieldDeclarationSyntax.DeclaredAccessibility == Accessibility.Private;
        }

        private bool IsParentAClass(IFieldSymbol fieldDeclarationSyntax)
        {
            if (fieldDeclarationSyntax.ContainingSymbol != null &&
                fieldDeclarationSyntax.ContainingSymbol.Kind == SymbolKind.NamedType)
            {
                return ((ITypeSymbol) fieldDeclarationSyntax.ContainingSymbol).TypeKind == TypeKind.Class;
            }

            return false;
        }
    }
}
