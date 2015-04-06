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
    /// A using directive which declares a member of the <see cref="System"/> namespace appears after a using directive
    /// which declares a member of a different namespace, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using directive for the <see cref="System"/> namespace is placed
    /// after a non-<see cref="System"/> using directive. Placing all <see cref="System"/> using directives at the top
    /// of the using directives can make the code cleaner and easier to read, and can help make it easier to identify
    /// the namespaces that are being used by the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1208";
        private const string Title = "System using directives must be placed before other using directives";
        private const string MessageFormat = "Using directive for '{0}' must appear before directive for '{1}'";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "A using directive which declares a member of the 'System' namespace appears after a using directive which declares a member of a different namespace, within a C# code file.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1208.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(this.HandleUsingDirectiveSyntax, SyntaxKind.UsingDirective);
        }

        private void HandleUsingDirectiveSyntax(SyntaxNodeAnalysisContext context)
        {
            UsingDirectiveSyntax syntax = context.Node as UsingDirectiveSyntax;
            if (syntax.Alias != null)
            {
                return;
            }

            SemanticModel semanticModel = context.SemanticModel;
            INamespaceSymbol namespaceSymbol;
            string topLevelNamespace = GetTopLevelNamespace(semanticModel, syntax, out namespaceSymbol, context.CancellationToken);
            if (!"System".Equals(topLevelNamespace, StringComparison.Ordinal))
            {
                return;
            }

            CompilationUnitSyntax compilationUnit = syntax.Parent as CompilationUnitSyntax;
            SyntaxList<UsingDirectiveSyntax>? usingDirectives = compilationUnit?.Usings;
            if (!usingDirectives.HasValue)
            {
                NamespaceDeclarationSyntax namespaceDeclaration = syntax.Parent as NamespaceDeclarationSyntax;
                usingDirectives = namespaceDeclaration?.Usings;
            }

            if (!usingDirectives.HasValue)
            {
                return;
            }

            foreach (var usingDirective in usingDirectives)
            {
                // we are only interested in nodes before the current node
                if (usingDirective == syntax)
                {
                    break;
                }

                // ignore using alias directives, since they are handled by SA1209
                if (usingDirective.Alias != null)
                {
                    continue;
                }

                INamespaceSymbol precedingNamespaceSymbol;
                string precedingTopLevelNamespace = GetTopLevelNamespace(semanticModel, usingDirective, out precedingNamespaceSymbol, context.CancellationToken);
                if (precedingTopLevelNamespace == null || "System".Equals(precedingTopLevelNamespace, StringComparison.Ordinal))
                {
                    continue;
                }

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
            {
                return null;
            }

            string fullyQualifiedName = namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string name = fullyQualifiedName;

            int doubleColon = name.IndexOf("::");
            if (doubleColon >= 0)
            {
                name = name.Substring(doubleColon + 2);
            }

            int dot = name.IndexOf('.');
            if (dot >= 0)
            {
                name = name.Substring(0, dot);
            }

            return name;
        }
    }
}
