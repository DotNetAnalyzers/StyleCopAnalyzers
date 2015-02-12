namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A colon within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a colon is not correct.</para>
    /// 
    /// <para>The spacing around a colon depends upon the type of colon and how it is used within the code. A colon
    /// appearing within an element declaration must always have a single space on either side, unless it is the first
    /// or last character on the line. For example all of the colons below follow this rule:</para>
    ///
    /// <code language="cs">
    /// public class Class2&lt;T&gt; : Class1 where T : MyType
    /// {
    ///     public Class2(int x) : base(x)
    ///     {
    ///     }
    /// }
    /// </code>
    ///
    /// <para>When the colon comes at the end of a label or case statement, it must always be followed by whitespace or
    /// be the last character on the line, but should never be preceded by whitespace. For example:</para>
    ///
    /// <code language="cs">
    /// _label:
    /// switch (x)
    /// {
    ///     case 2: 
    ///         return x;
    /// }
    /// </code>
    ///
    /// <para>Finally, when a colon is used within a conditional statement, it must always contain a single space on
    /// either side, unless the colon is the first or last character on the line. For example:</para>
    ///
    /// <code language="cs">
    /// int x = y ? 2 : 3;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1024ColonsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1024ColonsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1024";
        private const string Title = "Colons Must Be Spaced Correctly";
        private const string MessageFormat = "Colon must{0} be {1} by a space.";
        private const string Category = "StyleCop.CSharp.SpacingRules";
        private const string Description = "A colon within a C# element is not spaced correctly.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1024.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
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
            foreach (var token in root.DescendantTokens())
            {
                switch (token.CSharpKind())
                {
                case SyntaxKind.ColonToken:
                    this.HandleColonToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleColonToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            bool requireBefore;
            switch (token.Parent.CSharpKind())
            {
            case SyntaxKind.BaseList:
            case SyntaxKind.BaseConstructorInitializer:
            case SyntaxKind.ThisConstructorInitializer:
            case SyntaxKind.TypeParameterConstraintClause:
            case SyntaxKind.ConditionalExpression:
                requireBefore = true;
                break;

            case SyntaxKind.LabeledStatement:
            case SyntaxKind.CaseSwitchLabel:
            case SyntaxKind.DefaultSwitchLabel:
            // NameColon is not explicitly listed in the description of this warning, but the behavior is inferred
            case SyntaxKind.NameColon:
                requireBefore = false;
                break;

            default:
                return;
            }

            // check for a following space
            bool missingFollowingSpace = true;
            if (token.HasTrailingTrivia)
            {
                if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                    missingFollowingSpace = false;
                else if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                    missingFollowingSpace = false;
            }

            bool? hasPrecedingSpace = null;
            if (!token.HasLeadingTrivia)
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.HasTrailingTrivia)
                    hasPrecedingSpace = true;
            }

            if (missingFollowingSpace)
            {
                // colon must{} be {followed} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), string.Empty, "followed"));
            }

            if (hasPrecedingSpace.HasValue && hasPrecedingSpace != requireBefore)
            {
                // colon must{ not}? be {preceded} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), requireBefore ? string.Empty : " not", "preceded"));
            }
        }
    }
}
