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
    /// The spacing around a semicolon is incorrect, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a semicolon is incorrect.</para>
    ///
    /// <para>A semicolon should always be followed by a single space, unless it is the last character on the line, and
    /// a semicolon should never be preceded by any whitespace, unless it is the first character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1002SemicolonsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1002SemicolonsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1002";
        private const string Title = "Semicolons should be spaced correctly";
        private const string MessageFormat = "Semicolons should{0} be {1} by a space.";
        private const string Description = "The spacing around a semicolon is incorrect, within a C# code file.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1002.md";

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
                case SyntaxKind.SemicolonToken:
                    HandleSemicolonToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleSemicolonToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
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
            else
            {
                SyntaxToken nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.CloseParenToken:
                    // Special handling for the following case:
                    // for (; ;)
                    missingFollowingSpace = false;
                    break;

                case SyntaxKind.SemicolonToken:
                    // Special handling for the following case:
                    // Statement();;
                    if (nextToken.Parent.IsKind(SyntaxKind.EmptyStatement))
                    {
                        missingFollowingSpace = false;
                    }

                    break;

                case SyntaxKind.None:
                    // The semi colon is the last character in the file.
                    return;

                default:
                    break;
                }
            }

            bool hasPrecedingSpace = false;
            bool ignorePrecedingSpace = false;
            if (!token.IsFirstInLine())
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                SyntaxTriviaList trailingTrivia = precedingToken.TrailingTrivia;
                if (trailingTrivia.Any() && trailingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    hasPrecedingSpace = true;
                }

                if (precedingToken.IsKind(SyntaxKind.SemicolonToken))
                {
                    // Special handling for the following case:
                    // for (; ;)
                    ignorePrecedingSpace = true;
                }
            }

            if (missingFollowingSpace)
            {
                // semicolon should{} be {followed} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.InsertFollowing, string.Empty, "followed"));
            }

            if (hasPrecedingSpace && !ignorePrecedingSpace)
            {
                // semicolon should{ not} be {preceded} by a space
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.RemoveImmediatePreceding, " not", "preceded"));
            }
        }
    }
}
