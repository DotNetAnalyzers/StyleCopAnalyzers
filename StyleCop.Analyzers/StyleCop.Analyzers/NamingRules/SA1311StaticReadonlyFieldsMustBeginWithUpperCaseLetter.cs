namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The name of a static readonly field does not begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a static readonly field begins with a lower-case
    /// letter.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1311";
        private const string Title = "Static readonly fields must begin with upper-case letter";
        private const string MessageFormat = "Static readonly fields must begin with upper-case letter";
        private const string Category = "StyleCop.CSharp.NamingRules";
        private const string Description = "The name of a static readonly field does not begin with an upper-case letter.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1311.html";

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
            context.RegisterSyntaxNodeAction(this.HandleFieldDeclarationm, SyntaxKind.FieldDeclaration);
        }

        private void HandleFieldDeclarationm(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = context.Node as FieldDeclarationSyntax;
            if (fieldDeclaration == null)
            {
                return;
            }

            if (!fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword) ||
               !fieldDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
            {
                return;
            }

            var variables = fieldDeclaration.Declaration?.Variables;
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

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation()));
            }
        }
    }
}
