// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The code file has blank lines at the start.
    /// </summary>
    /// <remarks>
    /// <para>To improve the layout of the code, StyleCop requires no blank lines at the start of files.</para>
    ///
    /// <para>A violation of this rule occurs when one or more blank lines are at the start of the file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1517CodeMustNotContainBlankLinesAtStartOfFile : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1517CodeMustNotContainBlankLinesAtStartOfFile"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1517";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1517.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1517Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1517MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1517Description), LayoutResources.ResourceManager, typeof(LayoutResources));

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
            if (context.Tree.IsWhitespaceOnly(context.CancellationToken))
            {
                // Handling of empty documents is now the responsibility of the analyzers
                return;
            }

            var firstToken = context.Tree.GetRoot().GetFirstToken(includeZeroWidth: true);

            if (firstToken.HasLeadingTrivia)
            {
                var leadingTrivia = firstToken.LeadingTrivia;

                var firstNonBlankLineTriviaIndex = TriviaHelper.IndexOfFirstNonBlankLineTrivia(leadingTrivia);
                switch (firstNonBlankLineTriviaIndex)
                {
                case 0:
                    // no blank lines
                    break;

                case -1:
                    // only blank lines
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, leadingTrivia.Span)));
                    break;

                default:
                    var textSpan = TextSpan.FromBounds(leadingTrivia[0].Span.Start, leadingTrivia[firstNonBlankLineTriviaIndex].Span.Start);
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, textSpan)));
                    break;
                }
            }
        }
    }
}
