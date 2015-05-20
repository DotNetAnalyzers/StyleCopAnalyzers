namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The opening and closing curly brackets for a C# statement have been omitted.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing curly brackets for a statement have been
    /// omitted. In C#, some types of statements may optionally include curly brackets. Examples include <c>if</c>,
    /// <c>while</c>, and <c>for</c> statements. For example, an if-statement may be written without curly
    /// brackets:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return this.value;
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop always requires the curly brackets to be present, to increase the
    /// readability and maintainability of the code.</para>
    ///
    /// <para>When the curly brackets are omitted, it is possible to introduce an error in the code by inserting an
    /// additional statement beneath the if-statement. For example:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     this.value = 2;
    ///     return this.value;
    /// </code>
    ///
    /// <para>Glancing at this code, it appears as if both the assignment statement and the return statement are
    /// children of the if-statement. In fact, this is not true. Only the assignment statement is a child of the
    /// if-statement, and the return statement will always execute regardless of the outcome of the if-statement.</para>
    ///
    /// <para>StyleCop always requires the opening and closing curly brackets to be present, to prevent these kinds of
    /// errors:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    /// {
    ///     this.value = 2;
    ///     return this.value;
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1503CurlyBracketsMustNotBeOmitted : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1503CurlyBracketsMustNotBeOmitted"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1503";
        private const string Title = "Curly brackets must not be omitted";
        private const string MessageFormat = "Curly brackets must not be omitted";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The opening and closing curly brackets for a C# statement have been omitted.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1503.html";

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
            if (!(childStatement is BlockSyntax))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, childStatement.GetLocation()));
            }
        }
    }
}
