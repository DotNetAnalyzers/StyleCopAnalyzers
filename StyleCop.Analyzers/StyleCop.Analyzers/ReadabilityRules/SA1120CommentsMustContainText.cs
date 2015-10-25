// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The C# comment does not contain any comment text.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a C# comment which does not contain any
    /// text.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1120CommentsMustContainText : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1120CommentsMustContainText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1120";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1120Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1120MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1120Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1120.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            foreach (var node in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (node.Kind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                    HandleSingleLineComment(context, node);
                    break;

                case SyntaxKind.MultiLineCommentTrivia:
                    HandleMultiLineComment(context, node);
                    break;
                }
            }
        }

        private static void HandleMultiLineComment(SyntaxTreeAnalysisContext context, SyntaxTrivia multiLineComment)
        {
            var nodeText = multiLineComment.ToString();

            // We remove the /* and the */ and determine if the comment has any content.
            var commentText = nodeText.Substring(2, nodeText.Length - 4);

            if (string.IsNullOrWhiteSpace(commentText))
            {
                var diagnostic = Diagnostic.Create(Descriptor, multiLineComment.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void HandleSingleLineComment(SyntaxTreeAnalysisContext context, SyntaxTrivia singleLineComment)
        {
            int index = 0;

            // PERF: Explicitly cast to IReadOnlyList so we only box once.
            IReadOnlyList<SyntaxTrivia> list = TriviaHelper.GetContainingTriviaList(singleLineComment, out index);
            var firstNonWhiteSpace = TriviaHelper.IndexOfFirstNonWhitespaceTrivia(list);

            // When we encounter a block of single line comments, we only want to raise this diagnostic
            // on the first or last line.  This ensures that whitespace in code commented out using
            // the Comment Selection option in Visual Studio will not raise the diagnostic for every
            // blank line in the code which is commented out.
            bool isFirst = index == firstNonWhiteSpace;
            if (!isFirst)
            {
                // This is -2 because we need to go back past the end of line trivia as well.
                var lastNonWhiteSpace = TriviaHelper.IndexOfTrailingWhitespace(list) - 2;
                if (index != lastNonWhiteSpace)
                {
                    return;
                }
            }

            if (IsNullOrWhiteSpace(singleLineComment.ToString(), 2))
            {
                var diagnostic = Diagnostic.Create(Descriptor, singleLineComment.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsNullOrWhiteSpace(string value, int startIndex)
        {
            if (value == null)
            {
                return true;
            }

            for (int i = startIndex; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
