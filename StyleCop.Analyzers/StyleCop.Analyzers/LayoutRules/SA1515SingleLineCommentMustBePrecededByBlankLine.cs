namespace StyleCop.Analyzers.LayoutRules
{
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
    public class SA1515SingleLineCommentMustBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1515SingleLineCommentMustBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1515";
        private const string Title = "Single-line comment must be preceded by blank line";
        private const string MessageFormat = "Single-line comment must be preceded by blank line";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "A single-line comment within C# code is not preceded by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1515.html";

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
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTreeAnalysis);
        }

        private void HandleSyntaxTreeAnalysis(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            foreach (var trivia in syntaxRoot.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)))
            {
                if (trivia.FullSpan.Start == 0)
                {
                    // skip the trivia if it is at the start of the file
                    continue;
                }

                if (trivia.ToString().StartsWith("////"))
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

                if (IsPrecededBySingleLineComment(triviaList, triviaIndex))
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

        private static bool IsOnOwnLine(SyntaxTriviaList triviaList, int triviaIndex)
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

        private static bool IsPrecededBySingleLineComment(SyntaxTriviaList triviaList, int triviaIndex)
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
                    return true;

                default:
                    return false;
                }
            }

            return false;
        }

        private static bool IsPrecededByBlankLine(SyntaxTriviaList triviaList, int triviaIndex)
        {
            var eolCount = 0;
            var index = triviaIndex - 1;

            while ((eolCount < 2) && (index >= 0))
            {
                switch (triviaList[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    index--;
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    index--;
                    break;

                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    // directive trivia are special, as they have a 'built-in' end-of-line.
                    return eolCount > 0;

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
            return prevToken.IsKind(SyntaxKind.OpenBraceToken);
        }

        private static bool IsPrecededByDirectiveTrivia(SyntaxTriviaList triviaList, int triviaIndex)
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
