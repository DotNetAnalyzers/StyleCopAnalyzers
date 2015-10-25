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
    internal class SA1005SingleLineCommentsMustBeginWithSingleSpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1005SingleLineCommentsMustBeginWithSingleSpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1005";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1005Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1005MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1005Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1005.md";

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

            bool isFirstSingleLineTrivia = true;
            int newLineCount = 0;

            foreach (var trivia in root.DescendantTrivia())
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                    HandleSingleLineCommentTrivia(context, trivia, isFirstSingleLineTrivia);
                    isFirstSingleLineTrivia = false;
                    newLineCount = 0;
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    newLineCount++;
                    if (newLineCount == 2)
                    {
                        isFirstSingleLineTrivia = true;
                        newLineCount = 0;
                    }

                    break;

                case SyntaxKind.WhitespaceTrivia:
                    break;

                default:
                    isFirstSingleLineTrivia = true;
                    break;
                }
            }
        }

        private static void HandleSingleLineCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia, bool isFirstSingleLineTrivia)
        {
            string text = trivia.ToFullString();
            if (text.Equals(@"//"))
            {
                return;
            }

            // special case: commented code or documentation if the parsers documentation mode is DocumentationMode.None
            if (text.StartsWith(@"///", StringComparison.Ordinal))
            {
                return;
            }

            // Special case: multiple dashes at start of comment
            if (text.StartsWith(@"//--", StringComparison.Ordinal))
            {
                return;
            }

            // Special case: //\ negates spacing requirements
            if (text.StartsWith(@"//\", StringComparison.Ordinal))
            {
                return;
            }

            // No need to handle documentation comments ("///") because they're not
            // reported as SingleLineCommentTrivia.
            int spaceCount = 0;
            for (int i = 2; (i < text.Length) && (text[i] == ' '); i++)
            {
                spaceCount++;
            }

            if (spaceCount == 1)
            {
                return;
            }

            // Special case: follow-on comment lines may be indented with more than
            // one space.
            if (spaceCount > 1 && !isFirstSingleLineTrivia)
            {
                return;
            }

            // Single line comment must begin with a space.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
