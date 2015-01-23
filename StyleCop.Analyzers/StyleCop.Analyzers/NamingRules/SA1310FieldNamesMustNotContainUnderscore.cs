namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A field name in C# contains an underscore.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a field name contains an underscore. Fields and variables should be
    /// named using descriptive, readable wording which describes the function of the field or variable. Typically,
    /// these names will be written using camel case, and should not use underscores. For example, use
    /// <c>customerPostCode</c> rather than <c>customer_post_code</c>.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to include underscores, place the field or variable within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1310FieldNamesMustNotContainUnderscore : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1310";
        internal const string Title = "Field names must not contain underscore";
        internal const string MessageFormat = "Field '{0}' must not contain an underscore";
        internal const string Category = "StyleCop.CSharp.NamingRules";
        internal const string Description = "A field name in C# contains an underscore.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1310.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(HandleFieldDeclarationSyntax, SyntaxKind.FieldDeclaration);
        }

        private void HandleFieldDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax syntax = (FieldDeclarationSyntax)context.Node;
            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
                return;

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

                switch (identifier.ValueText.IndexOf('_'))
                {
                case -1:
                    // no _ character
                    continue;

                case 0:
                    // leading underscore -> report as SA1309
                    continue;

                case 1:
                    switch (identifier.ValueText[0])
                    {
                    case 'm':
                    case 's':
                        // m_ or s_ prefixes are reported as SA1308
                        continue;

                    default:
                        break;
                    }

                    break;

                default:
                    break;
                }

                // Field '{name}' must not contain an underscore
                string name = identifier.ValueText;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
            }
        }
    }
}
