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
    /// bracket, a parenthesis, is the first character on the line, or is part of a string interpolation alignment
    /// component.</para>
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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1021Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1021MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1021Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1021.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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

            var isInInterpolationAlignmentClause = token.Parent.Parent.IsKind(SyntaxKind.InterpolationAlignmentClause);
            if (isInInterpolationAlignmentClause && !token.IsFollowedByWhitespace())
            {
                // SA1001 is already handling the case like: line.Append($"{testResult.DisplayName, -75}");
                // Where the extra space before the minus sign is undesirable.
                return;
            }

            bool precededBySpace = true;
            bool firstInLine = token.IsFirstInLine();
            bool followsSpecialCharacter = false;

            bool followedBySpace = token.IsFollowedByWhitespace();
            bool interpolatedUnaryExpression = token.IsInterpolatedUnaryExpression();
            bool lastInLine = token.IsLastInLine();

            if (!firstInLine)
            {
                precededBySpace = token.IsPrecededByWhitespace(context.CancellationToken);
                SyntaxToken precedingToken = token.GetPreviousToken();

                followsSpecialCharacter =
                    precedingToken.IsKind(SyntaxKind.OpenBracketToken)
                    || precedingToken.IsKind(SyntaxKind.OpenParenToken)
                    || precedingToken.IsKind(SyntaxKind.CloseParenToken)
                    || (precedingToken.IsKind(SyntaxKind.OpenBraceToken) && interpolatedUnaryExpression);
            }

            if (!firstInLine && !isInInterpolationAlignmentClause)
            {
                if (!followsSpecialCharacter && !precededBySpace)
                {
                    // Negative sign should{} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.InsertPreceding, string.Empty, "preceded"));
                }
                else if (followsSpecialCharacter && precededBySpace)
                {
                    // Negative sign should{ not} be {preceded} by a space.
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.RemovePreceding, " not", "preceded"));
                }
            }

            if (lastInLine || followedBySpace)
            {
                // Negative sign should{ not} be {followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.RemoveFollowing, " not", "followed"));
            }
        }
    }
}
