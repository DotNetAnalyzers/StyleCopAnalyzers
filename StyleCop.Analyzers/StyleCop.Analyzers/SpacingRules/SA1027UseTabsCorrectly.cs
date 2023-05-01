// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The code contains a tab or space character which is not consistent with the current project settings.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1027UseTabsCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1027UseTabsCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1027";

        internal static readonly string BehaviorKey = "Behavior";
        internal static readonly string ConvertToTabsBehavior = "ConvertToTabs";
        internal static readonly string ConvertToSpacesBehavior = "ConvertToSpaces";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1027.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1027Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1027MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1027Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;

        private static readonly ImmutableDictionary<string, string> ConvertToTabsProperties =
            ImmutableDictionary.Create<string, string>().SetItem(BehaviorKey, ConvertToTabsBehavior);

        private static readonly ImmutableDictionary<string, string> ConvertToSpacesProperties =
            ImmutableDictionary.Create<string, string>().SetItem(BehaviorKey, ConvertToSpacesBehavior);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartActionWithSettings(context =>
            {
                context.RegisterSyntaxTreeAction(SyntaxTreeAction);
            });
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            ImmutableArray<TextSpan> excludedSpans;
            if (!LocateExcludedSpans(root, out excludedSpans))
            {
                return;
            }

            ReportIncorrectTabUsage(context, settings.Indentation, excludedSpans);
        }

        private static void ReportIncorrectTabUsage(SyntaxTreeAnalysisContext context, IndentationSettings indentationSettings, ImmutableArray<TextSpan> excludedSpans)
        {
            SyntaxTree syntaxTree = context.Tree;
            SourceText sourceText = syntaxTree.GetText(context.CancellationToken);

            string completeText = sourceText.ToString();
            int length = completeText.Length;

            bool useTabs = indentationSettings.UseTabs;
            int tabSize = indentationSettings.TabSize;

            int excludedSpanIndex = 0;
            var lastExcludedSpan = new TextSpan(completeText.Length, 0);
            TextSpan nextExcludedSpan = !excludedSpans.IsEmpty ? excludedSpans[0] : lastExcludedSpan;

            int lastLineStart = 0;
            for (int startIndex = 0; startIndex < length; startIndex++)
            {
                if (startIndex == nextExcludedSpan.Start)
                {
                    startIndex = nextExcludedSpan.End - 1;
                    excludedSpanIndex++;
                    nextExcludedSpan = excludedSpanIndex < excludedSpans.Length ? excludedSpans[excludedSpanIndex] : lastExcludedSpan;
                    continue;
                }

                int tabCount = 0;
                bool containsSpaces = false;
                bool tabAfterSpace = false;
                switch (completeText[startIndex])
                {
                case ' ':
                    containsSpaces = true;
                    break;

                case '\t':
                    tabCount++;
                    break;

                case '\r':
                case '\n':
                    // Handle newlines. We can ignore CR/LF/CRLF issues because we are only tracking column position
                    // in a line, and not the line numbers themselves.
                    lastLineStart = startIndex + 1;
                    continue;

                default:
                    continue;
                }

                int endIndex;
                for (endIndex = startIndex + 1; endIndex < length; endIndex++)
                {
                    if (endIndex == nextExcludedSpan.Start)
                    {
                        break;
                    }

                    if (completeText[endIndex] == ' ')
                    {
                        containsSpaces = true;
                    }
                    else if (completeText[endIndex] == '\t')
                    {
                        tabCount++;
                        tabAfterSpace = containsSpaces;
                    }
                    else
                    {
                        break;
                    }
                }

                if (useTabs && startIndex == lastLineStart)
                {
                    // For the case we care about in the following condition (tabAfterSpace is false), spaceCount is
                    // the number of consecutive trailing spaces.
                    int spaceCount = (endIndex - startIndex) - tabCount;
                    if (tabAfterSpace || spaceCount >= tabSize)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptor,
                                Location.Create(syntaxTree, TextSpan.FromBounds(startIndex, endIndex)),
                                ConvertToTabsProperties));
                    }
                }
                else
                {
                    if (tabCount > 0)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptor,
                                Location.Create(syntaxTree, TextSpan.FromBounds(startIndex, endIndex)),
                                ConvertToSpacesProperties));
                    }
                }

                // Make sure to not analyze overlapping spans
                startIndex = endIndex - 1;
            }
        }

        private static bool LocateExcludedSpans(SyntaxNode root, out ImmutableArray<TextSpan> excludedSpans)
        {
            ImmutableArray<TextSpan>.Builder builder = ImmutableArray.CreateBuilder<TextSpan>();

            // Locate disabled text
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsKind(SyntaxKind.DisabledTextTrivia))
                {
                    builder.Add(trivia.Span);
                }
                else if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    if (trivia.ToString().StartsWith("////"))
                    {
                        // Exclude comments starting with //// because they could contain commented code which contains
                        // string or character literals, and we don't want to change the contents of those strings.
                        builder.Add(trivia.Span);
                    }
                }
            }

            // Locate string literals
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true))
            {
                switch (token.Kind())
                {
                case SyntaxKind.XmlTextLiteralToken:
                    if (token.Parent.IsKind(SyntaxKind.XmlCDataSection))
                    {
                        builder.Add(token.Span);
                    }

                    break;

                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.InterpolatedStringTextToken:
                    builder.Add(token.Span);
                    break;

                default:
                    break;
                }
            }

            // Sort the results
            builder.Sort();

            // Combine adjacent and overlapping spans
            ReduceTextSpans(builder);

            excludedSpans = builder.ToImmutable();
            return true;
        }

        private static void ReduceTextSpans(ImmutableArray<TextSpan>.Builder sortedTextSpans)
        {
            if (sortedTextSpans.Count == 0)
            {
                return;
            }

            int currentIndex = 0;
            for (int nextIndex = 1; nextIndex < sortedTextSpans.Count; nextIndex++)
            {
                TextSpan current = sortedTextSpans[currentIndex];
                TextSpan next = sortedTextSpans[nextIndex];
                if (current.End < next.Start)
                {
                    // Increment currentIndex this iteration
                    currentIndex++;
                    sortedTextSpans[currentIndex] = next;
                    continue;
                }

                // Since sortedTextSpans is sorted, we already know current and next overlap
                sortedTextSpans[currentIndex] = TextSpan.FromBounds(current.Start, next.End);
            }

            sortedTextSpans.Count = currentIndex + 1;
        }
    }
}
