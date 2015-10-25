// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The code contains multiple whitespace characters in a row.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains multiple whitespace characters in a row, unless
    /// the characters come at the beginning or end of a line of code, following a comma or semicolon or preceding a
    /// symbol.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1025CodeMustNotContainMultipleWhitespaceInARow : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1025CodeMustNotContainMultipleWhitespaceInARow"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1025";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1025Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1025MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1025Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1025.md";

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
            foreach (var trivia in root.DescendantTrivia())
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    HandleWhitespaceTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleWhitespaceTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            if (trivia.Span.Length <= 1)
            {
                return;
            }

            if (trivia.SyntaxTree.GetMappedLineSpan(trivia.Span).StartLinePosition.Character == 0)
            {
                return;
            }

            SyntaxToken token = trivia.Token;
            SyntaxToken precedingToken;
            SyntaxToken followingToken;

            int index;
            SyntaxTriviaList list;

            if ((index = token.LeadingTrivia.IndexOf(trivia)) >= 0)
            {
                precedingToken = token.GetPreviousToken();
                followingToken = token;
                list = token.LeadingTrivia;
            }
            else if ((index = token.TrailingTrivia.IndexOf(trivia)) >= 0)
            {
                precedingToken = token;
                followingToken = precedingToken.GetNextToken();
                list = token.TrailingTrivia;
            }
            else
            {
                // shouldn't be reachable, but either way can't proceed
                return;
            }

            var followingTrivia = index + 1 < list.Count ? list[index + 1] : default(SyntaxTrivia);

            if (precedingToken.IsKind(SyntaxKind.CommaToken)
                || precedingToken.IsKind(SyntaxKind.SemicolonToken)
                || followingTrivia.IsKind(SyntaxKind.EndOfLineTrivia)
                || followingToken.IsKind(SyntaxKind.EndOfFileToken))
            {
                return;
            }

            // Code must not contain multiple whitespace characters in a row.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
