﻿namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The code file has blank lines at the start.
    /// </summary>
    /// <remarks>
    /// <para>To improve the layout of the code, StyleCop requires no blank lines at the start of files.</para>
    ///
    /// <para>A violation of this rule occurs when one or more blank lines are at the start of the file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1517CodeMustNotContainBlankLinesAtStartOfFile : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1517CodeMustNotContainBlankLinesAtStartOfFile"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1517";
        private const string Title = "Code must not contain blank lines at start of file";
        private const string MessageFormat = "Code must not contain blank lines at start of file";
        private const string Description = "The code file has blank lines at the start.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1517.md";

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTreeAnalysis);
        }

        private static void HandleSyntaxTreeAnalysis(SyntaxTreeAnalysisContext context)
        {
            var firstToken = context.Tree.GetRoot().GetFirstToken(includeZeroWidth: true);

            if (firstToken.HasLeadingTrivia)
            {
                var leadingTrivia = firstToken.LeadingTrivia;

                var firstNonBlankLineTriviaIndex = TriviaHelper.IndexOfFirstNonBlankLineTrivia(leadingTrivia);
                switch (firstNonBlankLineTriviaIndex)
                {
                case 0:
                    // no blank lines
                    break;

                case -1:
                    // only blank lines
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, leadingTrivia.Span)));
                    break;

                default:
                    var textSpan = TextSpan.FromBounds(leadingTrivia[0].Span.Start, leadingTrivia[firstNonBlankLineTriviaIndex].Span.Start);
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, textSpan)));
                    break;
                }
            }
        }
    }
}
