// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
    internal class SA1024ColonsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1024ColonsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1024";
        private const string Title = "Colons Must Be Spaced Correctly";
        private const string MessageFormat = "Colon must{0} be {1}{2} by a space.";
        private const string Description = "A colon within a C# element is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1024.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                if (token.Kind() == SyntaxKind.ColonToken)
                {
                    HandleColonToken(context, token);
                }
            }
        }

        private static void HandleColonToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool requireBefore;
            switch (token.Parent.Kind())
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
                {
                    missingFollowingSpace = false;
                }
                else if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    missingFollowingSpace = false;
                }
            }

            bool hasPrecedingSpace = token.HasLeadingTrivia;
            if (!hasPrecedingSpace)
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                var combinedTrivia = TriviaHelper.MergeTriviaLists(precedingToken.TrailingTrivia, token.LeadingTrivia);
                if (combinedTrivia.Count > 0 && !combinedTrivia.Last().IsKind(SyntaxKind.MultiLineCommentTrivia))
                {
                    hasPrecedingSpace = true;
                }
            }

            if (hasPrecedingSpace != requireBefore)
            {
                // colon must{ not}? be {preceded}{} by a space
                var properties = requireBefore ? TokenSpacingProperties.InsertPreceding : TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, requireBefore ? string.Empty : " not", "preceded", string.Empty));
            }

            if (missingFollowingSpace)
            {
                // colon must{} be {followed}{} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.InsertFollowing, string.Empty, "followed", string.Empty));
            }
        }
    }
}
