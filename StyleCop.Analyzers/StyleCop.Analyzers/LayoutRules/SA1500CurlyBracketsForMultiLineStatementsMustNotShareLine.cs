namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The opening or closing curly bracket within a C# statement, element, or expression is not placed on its own
    /// line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening or closing curly bracket within a statement, element, or
    /// expression is not placed on its own line. For example:</para>
    ///
    /// <code language="cs">
    /// public object Method()
    /// {
    ///   lock (this) {
    ///     return this.value;
    ///   }
    /// }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the opening curly bracket of
    /// the lock statement is placed on the same line as the lock keyword, rather than being placed on its own line, as
    /// follows:</para>
    ///
    /// <code language="cs">
    /// public object Method()
    /// {
    ///   lock (this)
    ///   {
    ///     return this.value;
    ///   }
    /// }
    /// </code>
    ///
    /// <para>A violation will also occur if the closing curly bracket shares a line with other code. For
    /// example:</para>
    ///
    /// <code language="cs">
    /// public object Method()
    /// {
    ///   lock (this)
    ///   {
    ///     return this.value; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1500";
        private const string Title = "Curly brackets for multi-line statements must not share line";
        private const string MessageFormat = "Curly brackets for multi-line statements must not share line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The opening or closing curly bracket within a C# statement, element, or expression is not placed on its own line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1500.html";

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
            context.RegisterSyntaxNodeAction(HandleNamespaceDeclarationSyntax, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleClassDeclarationSyntax, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleEnumDeclarationSyntax, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(HandleInterfaceDeclarationSyntax, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleStructDeclarationSyntax, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(HandleAccessorListSyntax, SyntaxKind.AccessorList);
            context.RegisterSyntaxNodeAction(HandleBlockSyntax, SyntaxKind.Block);
        }

        private void HandleNamespaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((NamespaceDeclarationSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((NamespaceDeclarationSyntax)context.Node).CloseBraceToken);
        }

        private void HandleClassDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((ClassDeclarationSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((ClassDeclarationSyntax)context.Node).CloseBraceToken);
        }

        private void HandleEnumDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((EnumDeclarationSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((EnumDeclarationSyntax)context.Node).CloseBraceToken);
        }

        private void HandleInterfaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((InterfaceDeclarationSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((InterfaceDeclarationSyntax)context.Node).CloseBraceToken);
        }

        private void HandleStructDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((StructDeclarationSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((StructDeclarationSyntax)context.Node).CloseBraceToken);
        }

        private void HandleAccessorListSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((AccessorListSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((AccessorListSyntax)context.Node).CloseBraceToken);
        }

        private void HandleBlockSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckCurlyBracketToken(context, ((BlockSyntax)context.Node).OpenBraceToken);
            CheckCurlyBracketToken(context, ((BlockSyntax)context.Node).CloseBraceToken);
        }

        private void CheckCurlyBracketToken(SyntaxNodeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            Location location = token.GetLocation();
            int line = location.GetLineSpan().StartLinePosition.Line;

            SyntaxToken previousToken = token.GetPreviousToken();
            if (!previousToken.IsMissing)
            {
                if (previousToken.GetLocation().GetLineSpan().StartLinePosition.Line == line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    // no need to report more than one instance for this token
                    return;
                }
            }

            SyntaxToken nextToken = token.GetNextToken();
            if (!nextToken.IsMissing)
            {
                if (nextToken.GetLocation().GetLineSpan().StartLinePosition.Line == line)
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }
        }
    }
}
