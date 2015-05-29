namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement containing opening and closing curly brackets is written completely on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a statement that is wrapped in opening and closing curly brackets is
    /// written on a single line. For example:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this) { return this.value; }
    /// }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the entire lock statement is
    /// written on one line. The statement should be written across multiple lines, with the opening and closing curly
    /// brackets each on their own line, as follows:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this)
    ///     {
    ///         return this.value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1501StatementMustNotBeOnASingleLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1501StatementMustNotBeOnASingleLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1501";
        private const string Title = "Statement must not be on a single line";
        private const string MessageFormat = "Statement must not be on a single line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "A C# statement containing opening and closing curly brackets is written completely on a single line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1501.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(this.HandleBlock, SyntaxKind.Block);
        }

        private void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = context.Node as BlockSyntax;
            if ((block != null) &&
                !block.OpenBraceToken.IsMissing &&
                !block.CloseBraceToken.IsMissing &&
                IsPartOfStatement(block))
            {
                var openBraceLineNumber = block.SyntaxTree.GetLineSpan(block.OpenBraceToken.Span).StartLinePosition.Line;
                var closeBraceLineNumber = block.SyntaxTree.GetLineSpan(block.CloseBraceToken.Span).StartLinePosition.Line;

                if (openBraceLineNumber == closeBraceLineNumber)
                {
                    switch (block.Parent.Kind())
                    {
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        var containingExpression = GetContainingExpression(block.Parent);
                        if (IsSingleLineExpression(containingExpression))
                        {
                            // Single line lambda expressions and anonymous method declarations are allowed for single line expressions.
                            return;
                        }

                        break;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, block.OpenBraceToken.GetLocation()));
                }
            }
        }

        private static bool IsSingleLineExpression(ExpressionSyntax containingExpression)
        {
            if (containingExpression == null)
            {
                return false;
            }

            var lineSpan = containingExpression.SyntaxTree.GetLineSpan(containingExpression.Span);
            return lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line;
        }

        private static bool IsPartOfStatement(BlockSyntax block)
        {
            var parent = block.Parent;

            while ((parent != null) && !(parent is StatementSyntax))
            {
                parent = parent.Parent;
            }

            return parent != null;
        }

        private static ExpressionSyntax GetContainingExpression(SyntaxNode node)
        {
            while (node != null)
            {
                var expressionNode = node as ExpressionSyntax;
                if (expressionNode != null)
                {
                    return expressionNode;
                }

                node = node.Parent;
            }

            return null;
        }
    }
}
