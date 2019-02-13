// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An opening brace within a C# element, statement, or expression is preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when an opening brace is preceded by a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    ///
    /// {
    ///     get
    ///
    ///     {
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where opening
    /// braces are preceded by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1509OpeningBracesMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1509OpeningBracesMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1509";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1509.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1509Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1509MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1509Description), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            SyntaxToken previousToken = default;
            foreach (var token in syntaxRoot.DescendantTokens())
            {
                if (token.IsKind(SyntaxKind.OpenBraceToken) && !previousToken.IsKind(SyntaxKind.CloseBraceToken))
                {
                    AnalyzeOpenBrace(context, token, previousToken);
                }

                previousToken = token;
            }
        }

        private static void AnalyzeOpenBrace(SyntaxTreeAnalysisContext context, SyntaxToken openBrace, SyntaxToken previousToken)
        {
            var triviaList = TriviaHelper.MergeTriviaLists(previousToken.TrailingTrivia, openBrace.LeadingTrivia);

            var done = false;
            var eolCount = 0;
            for (var i = triviaList.Count - 1; !done && (i >= 0); i--)
            {
                switch (triviaList[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                default:
                    if (triviaList[i].IsDirective)
                    {
                        // These have a built-in end of line
                        eolCount++;
                    }

                    done = true;
                    break;
                }
            }

            if (eolCount < 2)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, openBrace.GetLocation()));
        }
    }
}
