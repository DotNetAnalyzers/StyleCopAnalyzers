namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The opening and closing curly braces of a chained <c>if</c>/<c>else if</c>/<c>else</c> construct were included
    /// for some clauses, but omitted for others.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing curly braces for a chained statement have
    /// been included for some clauses but omitted for others. In C#, some types of statements may optionally include
    /// curly braces. For example, an <c>if</c>-statement may be written with inconsistent curly braces:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return this.value;
    /// else
    /// {
    ///     return that.value.
    /// }
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop requires the curly braces to be present for all clauses of a
    /// chained <c>if</c>/<c>else if</c>/<c>else</c> construct when braces are included for any clause, to increase
    /// the readability and maintainability of the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1520UseCurlyBracesConsistently : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1520UseCurlyBracesConsistently"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1520";
        private const string Title = "Use curly braces consistently";
        private const string MessageFormat = "Use curly braces consistently";
        private const string Description = "The opening and closing curly braces of a chained if/else if/else construct were included for some clauses, but omitted for others.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1520.md";

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
        }

        private void HandleIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            if (ifStatement.Parent.IsKind(SyntaxKind.ElseClause))
            {
                // this will be analyzed as a clause of the outer if statement
                return;
            }

            List<StatementSyntax> clauses = new List<StatementSyntax>();
            for (IfStatementSyntax current = ifStatement; current != null; current = current.Else?.Statement as IfStatementSyntax)
            {
                clauses.Add(current.Statement);
                if (current.Else != null && !(current.Else.Statement is IfStatementSyntax))
                {
                    clauses.Add(current.Else.Statement);
                }
            }

            if (clauses.All(i => i is BlockSyntax))
            {
                // consistent inclusion of braces
                return;
            }

            if (!clauses.OfType<BlockSyntax>().Any())
            {
                // consistent lack of braces
                return;
            }

            foreach (StatementSyntax clause in clauses)
            {
                this.CheckChildStatement(context, clause);
            }
        }

        private void CheckChildStatement(SyntaxNodeAnalysisContext context, StatementSyntax childStatement)
        {
            if (childStatement is BlockSyntax)
            {
                return;
            }

            if (context.SemanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(SA1519CurlyBracesMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress)
            {
                // diagnostics for multi-line statements is handled by SA1519, as long as it's not suppressed
                FileLinePositionSpan lineSpan = childStatement.GetLineSpan();
                if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, childStatement.GetLocation()));
        }
    }
}
