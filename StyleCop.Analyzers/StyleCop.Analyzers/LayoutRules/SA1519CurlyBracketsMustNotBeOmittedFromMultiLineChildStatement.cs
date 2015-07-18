namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The opening and closing curly brackets for a multi-line C# statement have been omitted.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing curly brackets for a multi-line statement
    /// have been omitted. In C#, some types of statements may optionally include curly brackets. Examples include
    /// <c>if</c>, <c>while</c>, and <c>for</c> statements. For example, an if-statement may be written without curly
    /// brackets:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return
    ///         this.value;
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop requires the curly brackets to be present when the statement spans
    /// multiple lines, to increase the readability and maintainability of the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1519CurlyBracketsMustNotBeOmittedFromMultiLineChildStatement : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1519CurlyBracketsMustNotBeOmittedFromMultiLineChildStatement"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1519";
        private const string Title = "Curly brackets must not be omitted from multi-line child statement";
        private const string MessageFormat = "Curly brackets must not be omitted from multi-line child statement";
        private const string Description = "The opening and closing curly brackets for a multi-line C# statement have been omitted.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1519.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleForStatement, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleForEachStatement, SyntaxKind.ForEachStatement);
        }

        private void HandleIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = context.Node as IfStatementSyntax;
            if (ifStatement != null)
            {
                this.CheckChildStatement(context, ifStatement.Statement);

                if (ifStatement.Else != null)
                {
                    // an 'else' directly followed by an 'if' should not trigger this diagnostic.
                    if (!ifStatement.Else.Statement.IsKind(SyntaxKind.IfStatement))
                    {
                        this.CheckChildStatement(context, ifStatement.Else.Statement);
                    }
                }
            }
        }

        private void HandleDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = context.Node as DoStatementSyntax;
            if (doStatement != null)
            {
                this.CheckChildStatement(context, doStatement.Statement);
            }
        }

        private void HandleWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = context.Node as WhileStatementSyntax;
            if (whileStatement != null)
            {
                this.CheckChildStatement(context, whileStatement.Statement);
            }
        }

        private void HandleForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = context.Node as ForStatementSyntax;
            if (forStatement != null)
            {
                this.CheckChildStatement(context, forStatement.Statement);
            }
        }

        private void HandleForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = context.Node as ForEachStatementSyntax;
            if (forEachStatement != null)
            {
                this.CheckChildStatement(context, forEachStatement.Statement);
            }
        }

        private void CheckChildStatement(SyntaxNodeAnalysisContext context, StatementSyntax childStatement)
        {
            if (childStatement is BlockSyntax)
            {
                return;
            }

            FileLinePositionSpan lineSpan = childStatement.GetLineSpan();
            if (lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, childStatement.GetLocation(), ArrayEx.Empty<object>()));
        }
    }
}
