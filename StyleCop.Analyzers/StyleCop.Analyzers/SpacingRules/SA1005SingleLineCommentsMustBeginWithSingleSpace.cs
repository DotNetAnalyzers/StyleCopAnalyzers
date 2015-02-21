namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A single-line comment within a C# code file does not begin with a single space.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a single-line comment does not begin with a single space. For
    /// example:</para>
    ///
    /// <code language="cs">
    /// private void Method1()
    /// {
    ///     //A single-line comment.
    ///     //   A single-line comment.
    /// }
    /// </code>
    ///
    /// <para>The comments should begin with a single space after the leading forward slashes:</para>
    ///
    /// <code language="cs">
    /// private void Method1()
    /// {
    ///     // A single-line comment.
    ///     // A single-line comment.
    /// }
    /// </code>
    ///
    /// <para>An exception to this rule occurs when the comment is being used to comment out a line of code. In this
    /// case, the space can be omitted if the comment begins with four forward slashes to indicate out-commented code.
    /// For example:</para>
    ///
    /// <code language="cs">
    /// private void Method1()
    /// {
    ///     ////int x = 2;
    ///     ////return x;
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1005SingleLineCommentsMustBeginWithSingleSpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1005SingleLineCommentsMustBeginWithSingleSpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1005";
        private const string Title = "Single line comments must begin with single space";
        private const string MessageFormat = "Single line comment must begin with a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "A single-line comment within a C# code file does not begin with a single space.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1005.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var trivia in root.DescendantTrivia())
            {
                switch (trivia.CSharpKind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                    this.HandleSingleLineCommentTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleSingleLineCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            string text = trivia.ToFullString();
            if (text.Equals("//") || text.StartsWith("// "))
                return;

            // special case: commented code
            if (text.StartsWith("////"))
                return;

            // Single line comment must begin with a space.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
