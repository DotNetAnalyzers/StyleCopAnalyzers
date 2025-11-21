// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
    using StyleCop.Analyzers.Lightup;

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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1515.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1515Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1515MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1515Description), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
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

                if (IsPrecededBySingleLineCommentOnOwnLineOrDocumentation(triviaList, triviaIndex))
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
            if (triviaList[triviaIndex].Span.Start == 0)
            {
                return true;
            }

            while (triviaIndex >= 0)
            {
                if (triviaList[triviaIndex].IsDirective)
                {
                    // directive trivia are special, as they have a 'built-in' end-of-line.
                    return true;
                }

                if (triviaList[triviaIndex].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return true;
                }

                triviaIndex--;
            }

            return false;
        }

        private static bool IsPrecededBySingleLineCommentOnOwnLineOrDocumentation<T>(T triviaList, int triviaIndex)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            var eolCount = 0;

            triviaIndex--;
            while ((eolCount < 2) && (triviaIndex >= 0))
            {
                var currentTrivia = triviaList[triviaIndex];
                switch (currentTrivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    triviaIndex--;
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    triviaIndex--;
                    break;

                case SyntaxKind.SingleLineCommentTrivia:
                    return IsOnOwnLine(triviaList, triviaIndex);

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
                || (prevToken.IsKind(SyntaxKind.OpenBracketToken) && prevToken.Parent.IsKind(SyntaxKindEx.CollectionExpression))
                || prevToken.Parent.IsKind(SyntaxKind.CaseSwitchLabel)
                || prevToken.Parent.IsKind(SyntaxKindEx.CasePatternSwitchLabel)
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
                case SyntaxKind.PragmaWarningDirectiveTrivia:
                    return true;

                default:
                    return false;
                }
            }

            return false;
        }
    }
}
