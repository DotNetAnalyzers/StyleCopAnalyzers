namespace StyleCop.Analyzers.LayoutRules
{
    using System.Linq;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A C# element containing opening and closing curly brackets is written completely on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an element that is wrapped in opening and closing curly brackets is
    /// written on a single line. For example:</para>
    /// <code language="csharp">
    /// public object Method() { return null; }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the entire method is written
    /// on one line. The method should be written across multiple lines, with the opening and closing curly brackets
    /// each on their own line, as follows:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     return null; 
    /// }
    /// </code>
    ///
    /// <para>As an exception to this rule, accessors within properties, events, or indexers are allowed to be written
    /// all on a single line, as long as the accessor is short.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1502ElementMustNotBeOnASingleLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1502ElementMustNotBeOnASingleLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1502";
        private const string Title = "Element must not be on a single line";
        private const string MessageFormat = "Element must not be on a single line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "A C# element containing opening and closing curly brackets is written completely on a single line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1502.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(this.HandleTypeDeclarations, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(this.HandlePropertyLikeDeclarations, SyntaxKind.PropertyDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleMethodLikeDeclarations, SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleEnumDeclarations, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleNamespaceDeclarations, SyntaxKind.NamespaceDeclaration);
        }

        private void HandleTypeDeclarations(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;
            CheckViolation(context, typeDeclaration.OpenBraceToken, typeDeclaration.CloseBraceToken);
        }

        private void HandlePropertyLikeDeclarations(SyntaxNodeAnalysisContext context)
        {
            var basePropertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;

            // The AccessorList will be null when an expression body is present.
            if (basePropertyDeclaration.AccessorList != null)
            {
                bool isAutoProperty = basePropertyDeclaration.AccessorList.Accessors.All(accessor => accessor.Body == null);
                if (!isAutoProperty)
                {
                    CheckViolation(context, basePropertyDeclaration.AccessorList.OpenBraceToken, basePropertyDeclaration.AccessorList.CloseBraceToken);
                }
            }
        }

        private void HandleMethodLikeDeclarations(SyntaxNodeAnalysisContext context)
        {
            var baseMethodDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            // Method declarations in interfaces will have an empty body.
            if (baseMethodDeclaration.Body != null)
            {
                CheckViolation(context, baseMethodDeclaration.Body.OpenBraceToken, baseMethodDeclaration.Body.CloseBraceToken);
            }
        }

        private void HandleEnumDeclarations(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;
            CheckViolation(context, enumDeclaration.OpenBraceToken, enumDeclaration.CloseBraceToken);
        }

        private void HandleNamespaceDeclarations(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            CheckViolation(context, namespaceDeclaration.OpenBraceToken, namespaceDeclaration.CloseBraceToken);
        }

        private static void CheckViolation(SyntaxNodeAnalysisContext context, SyntaxToken openBraceToken, SyntaxToken closeBraceToken)
        {
            var openingBraceLineSpan = openBraceToken.GetLocation().GetLineSpan();
            var closingBraceLineSpan = closeBraceToken.GetLocation().GetLineSpan();

            if (openingBraceLineSpan.EndLinePosition.Line == closingBraceLineSpan.StartLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openBraceToken.GetLocation()));
            }
        }
    }
}
