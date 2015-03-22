namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// The C# code contains multiple blank lines in a row.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when the code contains more than one blank line in a row. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get 
    ///     { 
    ///         Console.WriteLine("Getting the enabled flag.");
    /// 
    /// 
    ///         return this.enabled; 
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate an instance of this violation, since it contains blank multiple lines in a
    /// row.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1507CodeMustNotContainMultipleBlankLinesInARow : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1507CodeMustNotContainMultipleBlankLinesInARow"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1507";
        private const string Title = "Code must not contain multiple blank lines in a row";
        private const string MessageFormat = "Code must not contain multiple blank lines in a row";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The C# code contains multiple blank lines in a row.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1507.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTreeAnalysis);
        }

        private void HandleSyntaxTreeAnalysis(SyntaxTreeAnalysisContext context)
        {
            var text = context.Tree.GetText();
            var syntaxRoot = context.Tree.GetRoot();
            var blankLinesStart = 0;
            var blankLinesCount = 0;

            for (var i = 0; i < text.Lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(text.Lines[i].ToString()))
                {
                    blankLinesCount++;
                }
                else
                {
                    // If the file starts with blank lines, ignore them, they will be handled by SA1517.
                    if ((blankLinesStart > 0) && (blankLinesCount > 1))
                    {
                        var blankLinesSpan = TextSpan.FromBounds(text.Lines[blankLinesStart].Span.Start, text.Lines[i - 1].Span.End);

                        var node = syntaxRoot.FindNode(text.Lines[blankLinesStart].Span);
                        if (!node.IsKind(SyntaxKind.StringLiteralExpression))
                        {
                            var trivia = syntaxRoot.FindTrivia(text.Lines[blankLinesStart].Span.Start);
                            switch (trivia.Kind())
                            {
                                case SyntaxKind.DisabledTextTrivia:
                                case SyntaxKind.MultiLineCommentTrivia:
                                    // ignore multiple blanks lines
                                    break;

                                default:
                                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, blankLinesSpan)));
                                    break;
                            }
                        }
                    }

                    blankLinesStart = i + 1;
                    blankLinesCount = 0;
                }
            }

            // If the file ends with blanks lines, ignore them, they will be handled by SA1518.
        }
    }
}
