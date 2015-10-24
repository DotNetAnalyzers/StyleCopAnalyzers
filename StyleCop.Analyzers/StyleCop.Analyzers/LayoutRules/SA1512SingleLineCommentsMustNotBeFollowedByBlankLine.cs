// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A single-line comment within C# code is followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a single-line comment is followed by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         // Return the value of the 'enabled' field.
    ///
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate an instance of this violation, since the single-line comment is followed by
    /// a blank line.</para>
    ///
    /// <para>It is allowed to place a blank line in between two blocks of single-line comments. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         // This is a sample comment which doesn't really say anything.
    ///         // This is another part of the comment.
    ///
    ///         // There is a blank line above this comment but that is ok.
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>If the comment is being used to comment out a line of code, place four forward slashes at the beginning of
    /// the comment, rather than two. This will cause StyleCop to ignore this violation. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         ////return false;
    ///
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1512SingleLineCommentsMustNotBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1512SingleLineCommentsMustNotBeFollowedByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1512";
        private const string Title = "Single-line comments must not be followed by blank line";
        private const string MessageFormat = "Single-line comments must not be followed by blank line";
        private const string Description = "A single-line comment within C# code is followed by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1512.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            var diagnosticOptions = context.Compilation.Options.SpecificDiagnosticOptions;
            context.RegisterSyntaxTreeActionHonorExclusions(c => HandleSyntaxTreeAnalysis(c, diagnosticOptions));
        }

        private static void HandleSyntaxTreeAnalysis(SyntaxTreeAnalysisContext context, ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            foreach (var trivia in syntaxRoot.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)))
            {
                if (trivia.ToString().StartsWith("////", StringComparison.Ordinal))
                {
                    // ignore commented out code
                    continue;
                }

                int triviaIndex;

                // PERF: Explicitly cast to IReadOnlyList so we only box once.
                var triviaList = TriviaHelper.GetContainingTriviaList(trivia, out triviaIndex);

                if (!IsOnOwnLine(triviaList, triviaIndex))
                {
                    // ignore comments after other code elements.
                    continue;
                }

                if (IsPartOfFileHeader(triviaList, triviaIndex))
                {
                    // ignore comments that are part of the file header.
                    continue;
                }

                var trailingBlankLineCount = GetTrailingBlankLineCount(triviaList, ref triviaIndex);
                if (trailingBlankLineCount == 0)
                {
                    // ignore comments that are not followed by a blank line
                    continue;
                }
                else if (trailingBlankLineCount > 1)
                {
                    if (specificDiagnosticOptions.GetValueOrDefault(SA1507CodeMustNotContainMultipleBlankLinesInARow.DiagnosticId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress)
                    {
                        // ignore comments that are followed by multiple blank lines -> the multiple blank lines will be reported by SA1507
                        continue;
                    }
                }
                else
                {
                    if (triviaIndex < triviaList.Count)
                    {
                        switch (triviaList[triviaIndex].Kind())
                        {
                        case SyntaxKind.SingleLineCommentTrivia:
                        case SyntaxKind.SingleLineDocumentationCommentTrivia:
                        case SyntaxKind.MultiLineCommentTrivia:
                        case SyntaxKind.MultiLineDocumentationCommentTrivia:
                            // ignore a single blank line in between two comments.
                            continue;
                        }
                    }
                }

                var diagnosticSpan = TextSpan.FromBounds(trivia.SpanStart, trivia.SpanStart + 2);
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, diagnosticSpan)));
            }
        }

        private static bool IsOnOwnLine<T>(T triviaList, int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            while (triviaIndex >= 0)
            {
                if (triviaList[triviaIndex].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return true;
                }

                triviaIndex--;
            }

            return false;
        }

        private static bool IsPartOfFileHeader<T>(T triviaList, int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            if (triviaList[0].FullSpan.Start > 0)
            {
                return false;
            }

            var inSingleLineComment = false;

            for (var i = 0; i < triviaList.Count; i++)
            {
                switch (triviaList[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    if (!inSingleLineComment)
                    {
                        return triviaIndex < i;
                    }

                    inSingleLineComment = false;
                    break;
                case SyntaxKind.SingleLineCommentTrivia:
                    inSingleLineComment = true;
                    break;
                default:
                    return triviaIndex < i;
                }
            }

            return true;
        }

        private static int GetTrailingBlankLineCount<T>(T triviaList, ref int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            int eolCount = 0;

            for (var i = triviaIndex + 1; i < triviaList.Count; i++)
            {
                switch (triviaList[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    // ignore whitespace
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;

                default:
                    triviaIndex = i;
                    return Math.Max(0, eolCount - 1);
                }
            }

            triviaIndex = triviaList.Count;
            return Math.Max(0, eolCount - 1);
        }
    }
}
