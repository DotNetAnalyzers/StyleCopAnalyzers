namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A closing curly bracket within a C# element, statement, or expression is preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a closing curly bracket is preceded by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         return this.enabled;
    ///
    ///     }
    ///
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where closing
    /// curly brackets are preceded by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1508ClosingCurlyBracketsMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1508ClosingCurlyBracketsMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1508";
        private const string Title = "Closing curly brackets must not be preceded by blank line";
        private const string MessageFormat = "A closing curly bracket must not be preceded by a blank line.";
        private const string Description = "A closing curly bracket within a C# element, statement, or expression is preceded by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1508.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBlock, SyntaxKind.Block);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleInitializers, SyntaxKind.ObjectInitializerExpression, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ArrayInitializerExpression, SyntaxKind.ComplexElementInitializerExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAnonymousObjectCreation, SyntaxKind.AnonymousObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSwitchStatement, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAccessorList, SyntaxKind.AccessorList);
        }

        private void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;
            this.AnalyzeCloseBrace(context, block.CloseBraceToken);
        }

        private void HandleInitializers(SyntaxNodeAnalysisContext context)
        {
            var expression = (InitializerExpressionSyntax)context.Node;
            this.AnalyzeCloseBrace(context, expression.CloseBraceToken);
        }

        private void HandleAnonymousObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var expression = (AnonymousObjectCreationExpressionSyntax)context.Node;
            this.AnalyzeCloseBrace(context, expression.CloseBraceToken);
        }

        private void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;
            this.AnalyzeCloseBrace(context, switchStatement.CloseBraceToken);
        }

        private void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            this.AnalyzeCloseBrace(context, namespaceDeclaration.CloseBraceToken);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (BaseTypeDeclarationSyntax)context.Node;
            this.AnalyzeCloseBrace(context, typeDeclaration.CloseBraceToken);
        }

        private void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;
            this.AnalyzeCloseBrace(context, accessorList.CloseBraceToken);
        }

        private void AnalyzeCloseBrace(SyntaxNodeAnalysisContext context, SyntaxToken closeBraceToken)
        {
            var previousToken = closeBraceToken.GetPreviousToken();
            if ((closeBraceToken.GetLocation().GetLineSpan().StartLinePosition.Line - previousToken.GetLocation().GetLineSpan().EndLinePosition.Line) < 2)
            {
                // there will be no blank lines when the closing brace and the preceding token are not at least two lines apart.
                return;
            }

            var separatingTrivia = previousToken.TrailingTrivia.AddRange(closeBraceToken.LeadingTrivia);

            // skip all leading whitespace for the close brace
            var index = separatingTrivia.Count - 1;
            while (separatingTrivia[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            var done = false;
            var eolCount = 0;
            while (!done && index >= 0)
            {
                switch (separatingTrivia[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                default:
                    done = true;
                    break;
                }

                index--;
            }

            if (eolCount > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeBraceToken.GetLocation()));
            }
        }
    }
}
