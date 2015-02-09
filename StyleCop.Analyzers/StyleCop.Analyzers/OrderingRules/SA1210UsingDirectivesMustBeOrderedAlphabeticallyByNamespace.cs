namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The using directives within a C# code file are not sorted alphabetically by namespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the using directives are not sorted alphabetically by namespace.
    /// Sorting the using directives alphabetically makes the code cleaner and easier to read, and can help make it
    /// easier to identify the namespaces that are being used by the code. The <see cref="System"/> namespaces are an
    /// exception to this rule and will always precede all other namespaces. See
    /// <see cref="SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives"/> for more details.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1210";
        private const string Title = "Using directives must be ordered alphabetically by namespace";
        private const string MessageFormat = "Using directive for '{0}' must appear before directive for '{1}'";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "The using directives within a C# code file are not sorted alphabetically by namespace.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1210.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
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
            if (syntax.Alias != null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            INamespaceSymbol namespaceSymbol;
            string topLevelNamespace = GetTopLevelNamespace(semanticModel, syntax, out namespaceSymbol, context.CancellationToken);
            if (namespaceSymbol == null)
                return;

            bool systemNamespace = "System".Equals(topLevelNamespace, StringComparison.Ordinal);
            string fullyQualifiedNamespace = namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            CompilationUnitSyntax compilationUnit = syntax.Parent as CompilationUnitSyntax;
            SyntaxList<UsingDirectiveSyntax>? usingDirectives = compilationUnit?.Usings;
            if (!usingDirectives.HasValue)
            {
                NamespaceDeclarationSyntax namespaceDeclaration = syntax.Parent as NamespaceDeclarationSyntax;
                usingDirectives = namespaceDeclaration?.Usings;
            }

            if (!usingDirectives.HasValue)
                return;

            foreach (var usingDirective in usingDirectives)
            {
                // we are only interested in nodes before the current node
                if (usingDirective == syntax)
                    break;

                // ignore using alias directives, since they are handled by SA1209
                if (usingDirective.Alias != null)
                    continue;

                INamespaceSymbol precedingNamespaceSymbol;
                string precedingTopLevelNamespace = GetTopLevelNamespace(semanticModel, usingDirective, out precedingNamespaceSymbol, context.CancellationToken);
                if (precedingTopLevelNamespace == null || precedingNamespaceSymbol == null)
                    continue;

                // compare System namespaces to each other, and non-System namespaces to each other
                if ("System".Equals(precedingTopLevelNamespace, StringComparison.Ordinal) != systemNamespace)
                    continue;

                string precedingFullyQualifiedNamespace = precedingNamespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (string.Compare(fullyQualifiedNamespace, precedingFullyQualifiedNamespace, StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                string @namespace = namespaceSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                string precedingNamespace = precedingNamespaceSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

                // Using directive for '{namespace}' must appear before directive for '{precedingNamespace}'
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation(), @namespace, precedingNamespace));
                break;
            }
        }

        private static string GetTopLevelNamespace(SemanticModel semanticModel, UsingDirectiveSyntax syntax, out INamespaceSymbol namespaceSymbol, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(syntax.Name, cancellationToken);
            namespaceSymbol = symbolInfo.Symbol as INamespaceSymbol;
            if (namespaceSymbol == null)
                return null;

            string fullyQualifiedName = namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string name = fullyQualifiedName;

            int doubleColon = name.IndexOf("::");
            if (doubleColon >= 0)
                name = name.Substring(doubleColon + 2);

            int dot = name.IndexOf('.');
            if (dot >= 0)
                name = name.Substring(0, dot);

            return name;
        }
    }
}
