// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    /// The C# code contains a tab character.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a tab character.</para>
    ///
    /// <para>Tabs should not be used within C# code, because the length of the tab character can vary depending upon
    /// the editor being used to view the code. This can cause the spacing and indexing of the code to vary from the
    /// developer's original intention, and can in some cases make the code difficult to read.</para>
    ///
    /// <para>For these reasons, tabs should not be used, and each level of indentation should consist of four spaces.
    /// This will ensure that the code looks the same no matter which editor is being used to view the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1027TabsMustNotBeUsed : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1027TabsMustNotBeUsed"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1027";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1027Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1027MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1027Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1027.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;

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

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            SyntaxTree syntaxTree = context.Tree;
            SourceText sourceText = syntaxTree.GetText(context.CancellationToken);

            ImmutableArray<TextSpan> convertToTabsSpans;
            ImmutableArray<TextSpan> convertToSpacesSpans;
            if (!LocateIncorrectTabUsage(sourceText, settings.Indentation, out convertToTabsSpans, out convertToSpacesSpans))
            {
                return;
            }

            SyntaxNode root = syntaxTree.GetCompilationUnitRoot(context.CancellationToken);
            ImmutableArray<TextSpan> excludedSpans;
            if (!LocateExcludedSpans(root, out excludedSpans))
            {
                return;
            }

            int toTabsIndex = 0;
            int toSpacesIndex = 0;

            for (int excludedIndex = 0; excludedIndex < excludedSpans.Length; excludedIndex++)
            {
                TextSpan excluded = excludedSpans[excludedIndex];

                while (toTabsIndex < convertToTabsSpans.Length)
                {
                    TextSpan included = convertToTabsSpans[toTabsIndex];
                    if (included.Start >= excluded.End)
                    {
                        // Doesn't overlap the current excluded span, but might overlap the next
                        break;
                    }

                    if (included.Start < excluded.Start)
                    {
                        int violationStart = included.Start;
                        int violationEnd = Math.Min(included.End, excluded.Start);

                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptor,
                                Location.Create(syntaxTree, TextSpan.FromBounds(violationStart, violationEnd))));
                    }

                    if (included.End > excluded.End)
                    {
                        // It overlapped the current excluded span, and might also overlap the next
                        break;
                    }

                    toTabsIndex++;
                }

                while (toSpacesIndex < convertToSpacesSpans.Length)
                {
                    TextSpan included = convertToSpacesSpans[toSpacesIndex];
                    if (included.Start >= excluded.End)
                    {
                        // Doesn't overlap the current excluded span, but might overlap the next
                        break;
                    }

                    if (included.Start < excluded.Start)
                    {
                        int violationStart = included.Start;
                        int violationEnd = Math.Min(included.End, excluded.Start);

                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptor,
                                Location.Create(syntaxTree, TextSpan.FromBounds(violationStart, violationEnd))));
                    }

                    if (included.End > excluded.End)
                    {
                        // It overlapped the current excluded span, and might also overlap the next
                        break;
                    }

                    toSpacesIndex++;
                }
            }

            for (; toTabsIndex < convertToTabsSpans.Length; toTabsIndex++)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptor,
                        Location.Create(syntaxTree, convertToTabsSpans[toTabsIndex])));
            }

            for (; toSpacesIndex < convertToSpacesSpans.Length; toSpacesIndex++)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptor,
                        Location.Create(syntaxTree, convertToSpacesSpans[toSpacesIndex])));
            }
        }

        private static bool LocateIncorrectTabUsage(SourceText sourceText, IndentationSettings indentationSettings, out ImmutableArray<TextSpan> convertToTabsSpans, out ImmutableArray<TextSpan> convertToSpacesSpans)
        {
            ImmutableArray<TextSpan>.Builder toTabsBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            ImmutableArray<TextSpan>.Builder toSpacesBuilder = ImmutableArray.CreateBuilder<TextSpan>();

            string completeText = sourceText.ToString();
            int length = completeText.Length;

            bool useTabs = indentationSettings.UseTabs;
            int tabSize = indentationSettings.TabSize;

            int lastLineStart = 0;
            for (int startIndex = 0; startIndex < length; startIndex++)
            {
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
                        toTabsBuilder.Add(TextSpan.FromBounds(startIndex, endIndex));
                    }
                }
                else
                {
                    if (tabCount > 0)
                    {
                        toSpacesBuilder.Add(TextSpan.FromBounds(startIndex, endIndex));
                    }
                }

                // Make sure to not analyze overlapping spans
                startIndex = endIndex - 1;
            }

            convertToTabsSpans = toTabsBuilder.ToImmutable();
            convertToSpacesSpans = toSpacesBuilder.ToImmutable();
            return true;
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

                    // Only increment nextIndex this iteration if necessary to ensure nextIndex > currentIndex on the
                    // next iteration. At this point we already incremented currentIndex, but haven't incremented
                    // nextIndex.
                    if (currentIndex > nextIndex)
                    {
                        nextIndex--;
                    }

                    continue;
                }

                // Since sortedTextSpans is sorted, we already know current and next overlap
                sortedTextSpans[currentIndex] = TextSpan.FromBounds(current.Start, next.End);
            }

            sortedTextSpans.Count = currentIndex + 1;
        }
    }
}
