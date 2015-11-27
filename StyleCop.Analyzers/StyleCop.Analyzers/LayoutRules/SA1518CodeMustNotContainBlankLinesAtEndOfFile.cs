// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The code file has blank lines at the end.
    /// </summary>
    /// <remarks>
    /// <para>To improve the layout of the code, StyleCop requires no blank lines at the end of files.</para>
    ///
    /// <para>A violation of this rule occurs when one or more blank lines are at the end of the file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1518CodeMustNotContainBlankLinesAtEndOfFile : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1518CodeMustNotContainBlankLinesAtEndOfFile"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1518";
        private const string TitleForAllowSetting = "Code must not contain blank lines at end of file";
        private const string MessageFormatForAllowSetting = "Code must not contain blank lines at end of file";
        private const string TitleForRequireSetting = "Code is required to end with a single newline character";
        private const string MessageFormatForRequireSetting = "Code is required to end with a single newline character";
        private const string TitleForOmitSetting = "Code may not end with a newline character";
        private const string MessageFormatForOmitSetting = "Code may not end with a newline character";
        private const string Description = "Fix blank lines at the end of the file.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1518.md";

        private static readonly DiagnosticDescriptor DescriptorForAllowSetting =
            new DiagnosticDescriptor(DiagnosticId, TitleForAllowSetting, MessageFormatForAllowSetting, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor DescriptorForRequireSetting =
            new DiagnosticDescriptor(DiagnosticId, TitleForRequireSetting, MessageFormatForRequireSetting, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor DescriptorForOmitSetting =
            new DiagnosticDescriptor(DiagnosticId, TitleForOmitSetting, MessageFormatForOmitSetting, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DescriptorForAllowSetting);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            var endOfFileToken = context.Tree.GetRoot().GetLastToken(includeZeroWidth: true);
            TextSpan reportedSpan = new TextSpan(endOfFileToken.SpanStart, 0);

            SyntaxTrivia precedingTrivia = default(SyntaxTrivia);
            bool checkPrecedingToken;
            if (endOfFileToken.HasLeadingTrivia)
            {
                var leadingTrivia = endOfFileToken.LeadingTrivia;
                var trailingWhitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(leadingTrivia);
                if (trailingWhitespaceIndex > 0)
                {
                    checkPrecedingToken = false;
                    reportedSpan = TextSpan.FromBounds(leadingTrivia[trailingWhitespaceIndex].SpanStart, reportedSpan.End);
                    precedingTrivia = leadingTrivia[trailingWhitespaceIndex - 1];
                }
                else if (trailingWhitespaceIndex == 0)
                {
                    checkPrecedingToken = true;
                    reportedSpan = TextSpan.FromBounds(leadingTrivia[trailingWhitespaceIndex].SpanStart, reportedSpan.End);
                }
                else
                {
                    checkPrecedingToken = false;
                    precedingTrivia = leadingTrivia.Last();
                }
            }
            else
            {
                checkPrecedingToken = true;
            }

            if (checkPrecedingToken)
            {
                var previousToken = endOfFileToken.GetPreviousToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true);
                var trailingWhitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(previousToken.TrailingTrivia);
                if (trailingWhitespaceIndex > 0)
                {
                    reportedSpan = TextSpan.FromBounds(previousToken.TrailingTrivia[trailingWhitespaceIndex].SpanStart, reportedSpan.End);
                    precedingTrivia = previousToken.TrailingTrivia[trailingWhitespaceIndex - 1];
                }
                else if (trailingWhitespaceIndex == 0)
                {
                    reportedSpan = TextSpan.FromBounds(previousToken.TrailingTrivia[trailingWhitespaceIndex].SpanStart, reportedSpan.End);
                    precedingTrivia = default(SyntaxTrivia);
                }
                else
                {
                    if (previousToken.TrailingTrivia.Count > 0)
                    {
                        precedingTrivia = previousToken.TrailingTrivia.Last();
                    }
                }
            }

            if (precedingTrivia.IsDirective)
            {
                DirectiveTriviaSyntax directiveTriviaSyntax = precedingTrivia.GetStructure() as DirectiveTriviaSyntax;
                if (directiveTriviaSyntax != null && directiveTriviaSyntax.EndOfDirectiveToken.HasTrailingTrivia)
                {
                    var trailingWhitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(directiveTriviaSyntax.EndOfDirectiveToken.TrailingTrivia);
                    if (trailingWhitespaceIndex >= 0)
                    {
                        reportedSpan = TextSpan.FromBounds(directiveTriviaSyntax.EndOfDirectiveToken.TrailingTrivia[trailingWhitespaceIndex].SpanStart, reportedSpan.End);
                    }
                }
            }
            else if (precedingTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                reportedSpan = TextSpan.FromBounds(precedingTrivia.SpanStart, reportedSpan.End);
            }

            SourceText sourceText = context.Tree.GetText(context.CancellationToken);
            string trailingWhitespaceText = sourceText.ToString(reportedSpan);
            int firstNewline = trailingWhitespaceText.IndexOf('\n');
            int secondNewline = firstNewline >= 0 ? trailingWhitespaceText.IndexOf('\n', firstNewline + 1) : -1;

            DiagnosticDescriptor descriptorToReport;
            switch (settings.LayoutRules.NewlineAtEndOfFile)
            {
            case EndOfFileHandling.Omit:
                if (firstNewline < 0)
                {
                    return;
                }

                descriptorToReport = DescriptorForOmitSetting;
                break;

            case EndOfFileHandling.Require:
                if (firstNewline >= 0 && firstNewline == trailingWhitespaceText.Length - 1)
                {
                    return;
                }

                descriptorToReport = DescriptorForRequireSetting;
                break;

            case EndOfFileHandling.Allow:
            default:
                if (secondNewline < 0)
                {
                    // 1. A newline is allowed but not required
                    // 2. If a newline is included, it cannot be followed by whitespace
                    if (firstNewline < 0 || firstNewline == trailingWhitespaceText.Length - 1)
                    {
                        return;
                    }
                }

                descriptorToReport = DescriptorForAllowSetting;
                break;
            }

            context.ReportDiagnostic(Diagnostic.Create(descriptorToReport, Location.Create(context.Tree, reportedSpan)));
        }
    }
}
