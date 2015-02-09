namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A using-alias directive is positioned before a regular using directive.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using-alias directive is placed before a normal using directive.
    /// Using-alias directives have special behavior which can alter the meaning of the rest of the code within the file
    /// or namespace. Placing the using-alias directives together below all other using-directives can make the code
    /// cleaner and easier to read, and can help make it easier to identify the types used throughout the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1209";
        private const string Title = "Using alias directives must be placed after other using directives";
        private const string MessageFormat = "Using alias directive for '{0}' must appear after directive for '{1}'";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "A using-alias directive is positioned before a regular using directive.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1209.html";

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
            context.RegisterSyntaxNodeAction(this.HandleUsingDirectiveSyntax, SyntaxKind.UsingDirective);
        }

        private void HandleUsingDirectiveSyntax(SyntaxNodeAnalysisContext context)
        {
            UsingDirectiveSyntax syntax = context.Node as UsingDirectiveSyntax;
            if (syntax.Alias == null)
                return;

            CompilationUnitSyntax compilationUnit = syntax.Parent as CompilationUnitSyntax;
            SyntaxList<UsingDirectiveSyntax>? usingDirectives = compilationUnit?.Usings;
            if (!usingDirectives.HasValue)
            {
                NamespaceDeclarationSyntax namespaceDeclaration = syntax.Parent as NamespaceDeclarationSyntax;
                usingDirectives = namespaceDeclaration?.Usings;
            }

            if (!usingDirectives.HasValue)
                return;

            bool foundCurrent = false;
            foreach (var usingDirective in usingDirectives)
            {
                // we are only interested in nodes after the current node
                if (usingDirective == syntax)
                {
                    foundCurrent = true;
                    continue;
                }
                else if (!foundCurrent)
                {
                    continue;
                }

                // ignore following using alias directives
                if (usingDirective.Alias != null)
                    continue;

                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(usingDirective.Name, context.CancellationToken);
                INamespaceSymbol followingNamespaceSymbol = symbolInfo.Symbol as INamespaceSymbol;
                if (followingNamespaceSymbol == null)
                    continue;

                string alias = syntax.Alias.Name.ToString();
                string precedingNamespace = followingNamespaceSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

                // Using alias directive for '{alias}' must appear after directive for '{precedingNamespace}'
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation(), alias, precedingNamespace));
                break;
            }
        }
    }
}
