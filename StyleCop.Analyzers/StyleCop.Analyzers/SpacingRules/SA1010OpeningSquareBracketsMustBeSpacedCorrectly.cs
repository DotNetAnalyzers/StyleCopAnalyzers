﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An opening square bracket within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an opening square bracket within a statement is preceded or followed
    /// by whitespace.</para>
    ///
    /// <para>An opening square bracket must never be preceded by whitespace, unless it is the first character on the
    /// line, and an opening square must never be followed by whitespace, unless it is the last character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1010OpeningSquareBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1010OpeningSquareBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1010";
        private const string Title = "Opening square brackets must be spaced correctly";
        private const string MessageFormat = "Opening square brackets must {0} by a space.";
        private const string Description = "An opening square bracket within a C# statement is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1010.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTree);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                if (token.IsKind(SyntaxKind.OpenBracketToken) && !token.IsMissing)
                {
                    // attribute brackets are handled separately
                    if (!token.Parent.IsKind(SyntaxKind.AttributeList))
                    {
                        HandleOpenBracketToken(context, token);
                    }
                }
            }
        }

        private static void HandleOpenBracketToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = true;
            bool ignorePrecedingSpaceProblem = false;

            if (!firstInLine)
            {
                precededBySpace = token.IsPrecededByWhitespace();

                // ignore if handled by SA1026
                ignorePrecedingSpaceProblem = precededBySpace && token.GetPreviousToken().IsKind(SyntaxKind.NewKeyword);
            }

            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();

            if (!firstInLine && precededBySpace && !ignorePrecedingSpaceProblem)
            {
                // Opening square bracket must {not be preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), OpenCloseSpacingCodeFixProvider.RemovePreceding, "not be preceded"));
            }

            if (!lastInLine && followedBySpace)
            {
                // Opening square bracket must {not be followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), OpenCloseSpacingCodeFixProvider.RemoveFollowing, "not be followed"));
            }
        }
    }
}
