// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A negative sign within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a negative sign is not correct.</para>
    ///
    /// <para>A negative sign should always be preceded by a single space, unless it comes after an opening square
    /// bracket, a parenthesis, or is the first character on the line.</para>
    ///
    /// <para>A negative sign should never be followed by whitespace, and should never be the last character on a
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1021NegativeSignsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1021NegativeSignsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1021";
        private const string Title = "Negative signs must be spaced correctly";
        private const string MessageFormat = "Negative sign must{0} be {1} by a space.";
        private const string Description = "A negative sign within a C# element is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1021.md";

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
                switch (token.Kind())
                {
                case SyntaxKind.MinusToken:
                    HandleMinusToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleMinusToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (!token.Parent.IsKind(SyntaxKind.UnaryMinusExpression))
            {
                return;
            }

            bool precededBySpace = true;
            bool firstInLine = token.IsFirstInLine();
            bool followsSpecialCharacter = false;

            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();

            if (!firstInLine)
            {
                precededBySpace = token.IsPrecededByWhitespace();
                SyntaxToken precedingToken = token.GetPreviousToken();

                followsSpecialCharacter =
                    precedingToken.IsKind(SyntaxKind.OpenBracketToken)
                    || precedingToken.IsKind(SyntaxKind.OpenParenToken)
                    || precedingToken.IsKind(SyntaxKind.CloseParenToken);
            }

            if (!firstInLine)
            {
                if (!followsSpecialCharacter && !precededBySpace)
                {
                    // Negative sign must{} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.InsertPreceding, string.Empty, "preceded"));
                }
                else if (followsSpecialCharacter && precededBySpace)
                {
                    // Negative sign must{ not} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.RemovePreceding, " not", "preceded"));
                }
            }

            if (lastInLine || followedBySpace)
            {
                // Negative sign must{ not} be {followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.RemoveFollowing, " not", "followed"));
            }
        }
    }
}
