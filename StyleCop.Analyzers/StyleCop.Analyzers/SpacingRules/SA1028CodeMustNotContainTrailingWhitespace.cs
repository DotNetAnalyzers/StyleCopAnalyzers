// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Discovers any C# lines of code with trailing whitespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains whitespace at the end of the line.</para>
    ///
    /// <para>Trailing whitespace causes unnecessary diffs in source control,
    /// looks tacky in editors that show invisible whitespace as visible characters,
    /// and is highlighted as an error in some configurations of git.</para>
    ///
    /// <para>For these reasons, trailing whitespace should be avoided.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1028CodeMustNotContainTrailingWhitespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1028CodeMustNotContainTrailingWhitespace"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1028";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1028Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1028MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1028Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1028.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        /// <summary>
        /// Scans an entire document for lines with trailing whitespace.
        /// </summary>
        /// <param name="context">The context that provides the document to scan.</param>
        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var text = context.Tree.GetText(context.CancellationToken);

            SyntaxTrivia previousTrivia = default(SyntaxTrivia);
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    if (previousTrivia.Span.End < trivia.SpanStart)
                    {
                        // Some token appeared between the previous trivia and the end of the line.
                        break;
                    }

                    if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        // Report warning for whitespace token followed by the end of a line
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, previousTrivia.GetLocation()));
                    }
                    else if (previousTrivia.IsKind(SyntaxKind.PreprocessingMessageTrivia))
                    {
                        TextSpan trailinMessageWhitespace = FindTrailingWhitespace(text, previousTrivia.Span);
                        if (!trailinMessageWhitespace.IsEmpty)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, trailinMessageWhitespace)));
                        }
                    }

                    break;

                case SyntaxKind.SingleLineCommentTrivia:
                    TextSpan trailingWhitespace = FindTrailingWhitespace(text, trivia.Span);
                    if (!trailingWhitespace.IsEmpty)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, trailingWhitespace)));
                    }

                    break;

                case SyntaxKind.MultiLineCommentTrivia:
                    var line = text.Lines.GetLineFromPosition(trivia.Span.Start);
                    while (line.End <= trivia.Span.End)
                    {
                        trailingWhitespace = FindTrailingWhitespace(text, line.Span);
                        if (!trailingWhitespace.IsEmpty)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, trailingWhitespace)));
                        }

                        if (line.EndIncludingLineBreak == text.Length)
                        {
                            // We've reached the end of the document.
                            break;
                        }

                        line = text.Lines.GetLineFromPosition(line.EndIncludingLineBreak + 1);
                    }

                    break;

                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    SyntaxToken previousToken = default(SyntaxToken);
                    foreach (var token in trivia.GetStructure().DescendantTokens(descendIntoTrivia: true))
                    {
                        if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken)
                            && previousToken.IsKind(SyntaxKind.XmlTextLiteralToken)
                            && previousToken.Span.End == token.SpanStart)
                        {
                            trailingWhitespace = FindTrailingWhitespace(text, previousToken.Span);
                            if (!trailingWhitespace.IsEmpty)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, trailingWhitespace)));
                            }
                        }

                        previousToken = token;
                    }

                    break;

                default:
                    break;
                }

                previousTrivia = trivia;
            }

            if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia) && previousTrivia.Span.End == previousTrivia.SyntaxTree.Length)
            {
                // Report whitespace at the end of the last line in the document
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, previousTrivia.GetLocation()));
            }
        }

        private static TextSpan FindTrailingWhitespace(SourceText text, TextSpan within)
        {
            for (int i = within.End - 1; i >= within.Start; i--)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return TextSpan.FromBounds(i + 1, within.End);
                }
            }

            return within;
        }
    }
}
