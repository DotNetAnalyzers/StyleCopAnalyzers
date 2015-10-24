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
    /// A single-line comment within C# code is not preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a single-line comment is not preceded by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         Console.WriteLine("Getting the enabled flag.");
    ///         // Return the value of the 'enabled' field.
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate an instance of this violation, since the single-line comment is not preceded
    /// by a blank line.</para>
    ///
    /// <para>An exception to this rule occurs when the single-line comment is the first item within its scope. In this
    /// case, the comment should not be preceded by a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         // Return the value of the 'enabled' field.
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>In the code above, the comment is the first item within its scope, and thus it should not be preceded by a
    /// blank line.</para>
    ///
    /// <para>If the comment is being used to comment out a line of code, begin the comment with four forward slashes
    /// rather than two. This will cause StyleCop to ignore this violation. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         Console.WriteLine("Getting the enabled flag.");
    ///         ////return false;
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1515SingleLineCommentMustBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1515SingleLineCommentMustBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1515";
        private const string Title = "Single-line comment must be preceded by blank line";
        private const string MessageFormat = "Single-line comment must be preceded by blank line";
        private const string Description = "A single-line comment within C# code is not preceded by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1515.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

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
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            foreach (var trivia in syntaxRoot.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)))
            {
                if (trivia.FullSpan.Start == 0)
                {
                    // skip the trivia if it is at the start of the file
                    continue;
                }

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

                if (IsPrecededByBlankLine(triviaList, triviaIndex))
                {
                    // allow properly formatted blank line comments.
                    continue;
                }

                if (IsPrecededBySingleLineCommentOrDocumentation(triviaList, triviaIndex))
                {
                    // allow consecutive single line comments.
                    continue;
                }

                if (IsAtStartOfScope(trivia))
                {
                    // allow single line comment at scope start.
                    continue;
                }

                if (IsPrecededByDirectiveTrivia(triviaList, triviaIndex))
                {
                    // allow single line comment that is preceded by some directive trivia (if, elif, else)
                    continue;
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

        private static bool IsPrecededBySingleLineCommentOrDocumentation<T>(T triviaList, int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            var eolCount = 0;

            triviaIndex--;
            while ((eolCount < 2) && (triviaIndex >= 0))
            {
                switch (triviaList[triviaIndex].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    triviaIndex--;
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    triviaIndex--;
                    break;

                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return true;

                default:
                    return false;
                }
            }

            return false;
        }

        private static bool IsPrecededByBlankLine<T>(T triviaList, int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            var eolCount = 0;
            var index = triviaIndex - 1;

            while ((eolCount < 2) && (index >= 0))
            {
                if (triviaList[index].IsDirective)
                {
                    // directive trivia are special, as they have a 'built-in' end-of-line.
                    return eolCount > 0;
                }

                switch (triviaList[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    index--;
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    index--;
                    break;

                default:
                    return false;
                }
            }

            return eolCount >= 2;
        }

        private static bool IsAtStartOfScope(SyntaxTrivia trivia)
        {
            var token = trivia.Token;

            if (token.TrailingTrivia.Contains(trivia) && token.IsKind(SyntaxKind.OpenBraceToken))
            {
                return true;
            }

            var prevToken = token.GetPreviousToken();
            return prevToken.IsKind(SyntaxKind.OpenBraceToken)
                   || prevToken.Parent.IsKind(SyntaxKind.CaseSwitchLabel)
                   || prevToken.Parent.IsKind(SyntaxKind.DefaultSwitchLabel);
        }

        private static bool IsPrecededByDirectiveTrivia<T>(T triviaList, int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            triviaIndex--;
            while (triviaIndex >= 0)
            {
                switch (triviaList[triviaIndex].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    triviaIndex--;
                    break;

                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                    return true;

                default:
                    return false;
                }
            }

            return false;
        }
    }
}
