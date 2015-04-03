namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement contains a comment between the declaration of the statement and the opening curly bracket of the
    /// statement.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a comment in between the declaration and the
    /// opening curly bracket. For example:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// // Make sure x does not equal y
    /// {
    /// }
    /// </code>
    /// <para>The comment can legally be placed above the statement, or within the body of the statement:</para>
    /// <code language="csharp">
    /// // Make sure x does not equal y
    /// if (x != y)
    /// {
    /// }
    ///
    /// if (x != y)
    /// {
    ///     // Make sure x does not equal y
    /// }
    /// </code>
    /// <para>If the comment is being used to comment out a line of code, begin the comment with four forward slashes
    /// rather than two:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// ////if (x == y)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1108BlockStatementsMustNotContainEmbeddedComments : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1108BlockStatementsMustNotContainEmbeddedComments"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1108";
        private const string Title = "Block statements must not contain embedded comments";
        private const string MessageFormat = "Block statements must not contain embedded comments.";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "A C# statement contains a comment between the declaration of the statement and the opening curly bracket of the statement.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1108.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        private readonly SyntaxKind[] supportedKinds = new[]
        {
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.DoStatement,
            SyntaxKind.IfStatement,
            SyntaxKind.ElseClause,
            SyntaxKind.LockStatement,
            SyntaxKind.TryStatement,
            SyntaxKind.CatchClause,
            SyntaxKind.FinallyClause,
            SyntaxKind.CheckedStatement,
            SyntaxKind.UncheckedStatement,
            SyntaxKind.FixedStatement
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.AnalyzeBlock, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(this.AnalyzeSwitch, SyntaxKind.SwitchStatement);
        }

        private void AnalyzeSwitch(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax) context.Node;
            var openBraceToken = switchStatement.OpenBraceToken;
            if (openBraceToken.IsMissing)
            {
                return;
            }

            var previousToken = openBraceToken.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return;
            }

            this.FindAllComments(context, previousToken, openBraceToken);
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax) context.Node;
            if (block.Parent == null || !this.supportedKinds.Any(block.Parent.IsKind))
            {
                return;
            }

            var openBraceToken = block.OpenBraceToken;
            if (openBraceToken.IsMissing)
            {
                return;
            }

            var previousToken = openBraceToken.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return;
            }

            this.FindAllComments(context, previousToken, openBraceToken);
        }

        private void FindAllComments(SyntaxNodeAnalysisContext context, SyntaxToken previousToken, SyntaxToken openBraceToken)
        {
            var comments = previousToken.TrailingTrivia.Where(this.IsComment)
                .Union(openBraceToken.LeadingTrivia.Where(this.IsComment))
                .ToList();
            foreach (var comment in comments)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, comment.GetLocation()));
            }
        }


        private bool IsComment(SyntaxTrivia syntaxTrivia)
        {
            return (syntaxTrivia.IsKind(SyntaxKind.SingleLineCommentTrivia) && !syntaxTrivia.ToFullString().StartsWith(@"////")) ||
                   syntaxTrivia.IsKind(SyntaxKind.MultiLineCommentTrivia);
        }
    }
}
