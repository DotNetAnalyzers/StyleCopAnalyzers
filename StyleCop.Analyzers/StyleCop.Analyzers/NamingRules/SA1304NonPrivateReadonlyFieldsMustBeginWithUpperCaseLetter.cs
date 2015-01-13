namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The name of a non-private readonly C# field must begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a readonly field which is not private does not begin with
    /// an upper-case letter. Non-private readonly fields must always start with an upper-case letter.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to begin with a lower-case letter, place the field or variable within a special <c>NativeMethods</c>
    /// class. A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is
    /// intended as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed
    /// within a <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1304";
        internal const string Title = "Non-private readonly fields must begin with upper-case letter";
        internal const string MessageFormat = "Non-private readonly fields must begin with upper-case letter.";
        internal const string Category = "StyleCop.CSharp.NamingRules";

        internal const string Description = "The name of a non-private readonly C# field must being with an upper-case letter.";

        internal const string HelpLink = "http://www.stylecop.com/docs/SA1304.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return _supportedDiagnostics; }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(HandleFieldDeclaration, SymbolKind.Field);
        }

        private void HandleFieldDeclaration(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol as IFieldSymbol;

            if (symbol == null || 
                !symbol.IsReadOnly || 
                symbol.DeclaredAccessibility == Accessibility.Private)
            {
                return;
            }

            if (!string.IsNullOrEmpty(symbol.Name) &&
                char.IsLower(symbol.Name[0]) &&
                symbol.Locations.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, symbol.Locations[0]));
            }
        }
    }
}
