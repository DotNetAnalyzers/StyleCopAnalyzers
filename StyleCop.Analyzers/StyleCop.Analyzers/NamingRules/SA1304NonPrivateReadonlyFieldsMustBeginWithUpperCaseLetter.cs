namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        internal const string MessageFormat = "Non-private readonly fields must begin with upper-case letter";
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

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleFieldDeclarationSyntax, SyntaxKind.FieldDeclaration);
        }

        private void HandleFieldDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax syntax = (FieldDeclarationSyntax)context.Node;
            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
                return;

            if (!syntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
            {
                // this analyzer only applies to readonly fields
                return;
            }

            if (!syntax.Modifiers.Any(SyntaxKind.PublicKeyword)
                && !syntax.Modifiers.Any(SyntaxKind.ProtectedKeyword)
                && !syntax.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                // this analyzer only applies to non-private fields
                return;
            }

            var variables = syntax.Declaration?.Variables;
            if (variables == null)
                return;

            foreach (VariableDeclaratorSyntax variableDeclarator in variables.Value)
            {
                if (variableDeclarator == null)
                    continue;

                var identifier = variableDeclarator.Identifier;
                if (identifier.IsMissing)
                    continue;

                string name = identifier.ValueText;
                if (string.IsNullOrEmpty(name) || !char.IsLower(name[0]))
                    continue;

                // Non-private readonly fields must begin with upper-case letter.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation()));
            }
        }
    }
}
