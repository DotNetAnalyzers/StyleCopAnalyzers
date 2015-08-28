namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The while footer at the bottom of a do-while statement is separated from the statement by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when the while keyword at the bottom of a do-while statement is separated
    /// from the main part of the statement by one or more blank lines. For example:</para>
    ///
    /// <code language="csharp">
    /// do
    /// {
    ///     Console.WriteLine("Loop forever");
    /// }
    ///
    /// while (true);
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1511WhileDoFooterMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1511WhileDoFooterMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1511";
        private const string Title = "While-do footer must not be preceded by blank line";
        private const string MessageFormat = "While-do footer must not be preceded by blank line";
        private const string Description = "The while footer at the bottom of a do-while statement is separated from the statement by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1511.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(this.HandleCompilationStart);
        }

        private void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDoStatement, SyntaxKind.DoStatement);
        }

        private void HandleDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;
            var whileKeyword = doStatement.WhileKeyword;

            if (!whileKeyword.HasLeadingBlankLines())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, whileKeyword.GetLocation()));
        }
    }
}
