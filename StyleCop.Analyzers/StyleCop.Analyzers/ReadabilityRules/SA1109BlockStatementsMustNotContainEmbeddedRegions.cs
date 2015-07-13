namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A C# statement contains a region tag between the declaration of the statement and the opening curly bracket of
    /// the statement.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a region tag in between the declaration and the
    /// opening curly bracket. For example:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// #region
    /// {
    /// }
    /// #endregion
    /// </code>
    /// <para>This will result in the body of the statement being hidden when the region is collapsed.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1109BlockStatementsMustNotContainEmbeddedRegions : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "SA1109";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1109Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1109MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1109Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1109.md";

        /// <summary>
        /// Gets the diagnostic descriptor for SA1109.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1109.</value>
        public static DiagnosticDescriptor SA1109Descriptor { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(SA1109Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            if (context.IsAnalyzerSuppressed(SA1123DoNotPlaceRegionsWithinElements.DiagnosticId))
            {
                context.RegisterSyntaxNodeActionHonorExclusions(HandleBlock, SyntaxKind.Block);
                context.RegisterSyntaxNodeActionHonorExclusions(HandleSwitchStatement, SyntaxKind.SwitchStatement);
            }
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            SyntaxToken followingToken;
            switch (block.Parent.Kind())
            {
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.ElseClause:
                case SyntaxKind.FinallyClause:
                    CheckTrivia(context, block.GetLeadingTrivia());
                    break;

                case SyntaxKind.IfStatement:
                    var ifStatement = (IfStatementSyntax)block.Parent;
                    CheckTrivia(context, block.GetLeadingTrivia());
                    if (ifStatement.Else != null)
                    {
                        followingToken = block.CloseBraceToken.GetNextToken();
                        CheckTrivia(context, GetTrailingTriviaList(block.CloseBraceToken, followingToken));
                    }

                    break;

                case SyntaxKind.DoStatement:
                    var doStatement = (DoStatementSyntax)block.Parent;
                    CheckTrivia(context, block.GetLeadingTrivia());
                    CheckTrivia(context, GetTrailingTriviaList(block.CloseBraceToken, doStatement.WhileKeyword));
                    break;

                case SyntaxKind.TryStatement:
                    CheckTrivia(context, block.GetLeadingTrivia());
                    followingToken = block.CloseBraceToken.GetNextToken();
                    CheckTrivia(context, GetTrailingTriviaList(block.CloseBraceToken, followingToken));
                    break;

                case SyntaxKind.CatchClause:
                    var catchClause = (CatchClauseSyntax)block.Parent;
                    var tryStatement = (TryStatementSyntax)catchClause.Parent;
                    CheckTrivia(context, block.GetLeadingTrivia());

                    if ((tryStatement.Catches.Last() != catchClause) || (tryStatement.Finally != null))
                    {
                        followingToken = block.CloseBraceToken.GetNextToken();
                        CheckTrivia(context, GetTrailingTriviaList(block.CloseBraceToken, followingToken));
                    }

                    break;
            }
        }

        private static void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;
            CheckTrivia(context, switchStatement.OpenBraceToken.LeadingTrivia);
        }

        private static void CheckTrivia(SyntaxNodeAnalysisContext context, IEnumerable<SyntaxTrivia> triviaList)
        {
            var violatingDirective = triviaList.FirstOrDefault(t => t.IsKind(SyntaxKind.RegionDirectiveTrivia) || t.IsKind(SyntaxKind.EndRegionDirectiveTrivia));

            Location location;
            switch (violatingDirective.Kind())
            {
                case SyntaxKind.RegionDirectiveTrivia:
                    location = Location.Create(context.Node.SyntaxTree, TextSpan.FromBounds(violatingDirective.SpanStart, violatingDirective.SpanStart + 7));
                    context.ReportDiagnostic(Diagnostic.Create(SA1109Descriptor, location));
                    break;
                case SyntaxKind.EndRegionDirectiveTrivia:
                    location = Location.Create(context.Node.SyntaxTree, TextSpan.FromBounds(violatingDirective.SpanStart, violatingDirective.SpanStart + 10));
                    context.ReportDiagnostic(Diagnostic.Create(SA1109Descriptor, location));
                    break;
            }
        }

        private static IEnumerable<SyntaxTrivia> GetTrailingTriviaList(SyntaxToken firstToken, SyntaxToken secondToken)
        {
            // Merging the SyntaxTriviaLists will clear the syntax tree information that is needed.
            // Therefore the trivia are combined into a generic collection.
            var trailingTriviaList = new List<SyntaxTrivia>();
            trailingTriviaList.AddRange(firstToken.TrailingTrivia);
            trailingTriviaList.AddRange(secondToken.LeadingTrivia);

            return trailingTriviaList;
        }
    }
}
