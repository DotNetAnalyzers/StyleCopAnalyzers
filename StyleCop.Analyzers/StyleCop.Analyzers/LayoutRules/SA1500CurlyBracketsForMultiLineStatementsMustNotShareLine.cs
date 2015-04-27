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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleNamespaceDeclarationSyntax, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBaseTypeDeclarationSyntax, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBaseTypeDeclarationSyntax, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBaseTypeDeclarationSyntax, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBaseTypeDeclarationSyntax, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAccessorListSyntax, SyntaxKind.AccessorList);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBlockSyntax, SyntaxKind.Block);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSwitchStatementSyntax, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleInitializerExpressionSyntax, SyntaxKind.ObjectInitializerExpression, SyntaxKind.ArrayInitializerExpression, SyntaxKind.CollectionInitializerExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAnonymousObjectCreationExpressionSyntax, SyntaxKind.AnonymousObjectCreationExpression);
        }

        private void HandleNamespaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (NamespaceDeclarationSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void HandleBaseTypeDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (BaseTypeDeclarationSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void HandleAccessorListSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (AccessorListSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void HandleBlockSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (BlockSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void HandleSwitchStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (SwitchStatementSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void HandleInitializerExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (InitializerExpressionSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void HandleAnonymousObjectCreationExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (AnonymousObjectCreationExpressionSyntax)context.Node;
            this.CheckCurlyBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private void CheckCurlyBraces(SyntaxNodeAnalysisContext context, SyntaxToken openBraceToken, SyntaxToken closeBraceToken)
        {
            bool checkCloseBrace = true;

            if (GetStartLine(openBraceToken) == GetStartLine(closeBraceToken))
            {
                switch (context.Node.Parent.Kind())
                {
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    if (GetStartLine(((AccessorDeclarationSyntax)context.Node.Parent).Keyword) == GetStartLine(openBraceToken))
                    {
                        // reported as SA1504, if at all
                        return;
                    }

                    checkCloseBrace = false;
                    break;

                default:
                    // reported by SA1501 or SA1502
                    return;
                }
            }

            this.CheckCurlyBracketToken(context, openBraceToken);
            if (checkCloseBrace)
            {
                this.CheckCurlyBracketToken(context, closeBraceToken);
            }
        }

        private static int? GetStartLine(SyntaxToken token)
        {
            return token.GetLocation()?.GetLineSpan().StartLinePosition.Line;
        }

        private void CheckCurlyBracketToken(SyntaxNodeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

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
                switch (nextToken.Kind())
                {
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.SemicolonToken:
                    // these are allowed to appear on the same line
                    return;

                default:
                    break;
                }

                if (nextToken.GetLocation().GetLineSpan().StartLinePosition.Line == line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                }
            }
        }
    }
}
