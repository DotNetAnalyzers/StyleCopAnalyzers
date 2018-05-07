// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for all misaligned using statements.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsingCodeFixProvider))]
    [Shared]
    internal sealed partial class UsingCodeFixProvider : CodeFixProvider
    {
        private const string SystemUsingDirectiveIdentifier = nameof(System);

        private static readonly List<UsingDirectiveSyntax> EmptyUsingsList = new List<UsingDirectiveSyntax>();
        private static readonly SyntaxAnnotation UsingCodeFixAnnotation = new SyntaxAnnotation(nameof(UsingCodeFixAnnotation));
        private static readonly SymbolDisplayFormat FullNamespaceDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId,
                SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives.DiagnosticId,
                SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives.DiagnosticId,
                SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace.DiagnosticId,
                SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName.DiagnosticId,
                SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation.DiagnosticId,
                SA1217UsingStaticDirectivesMustBeOrderedAlphabetically.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                var isSA1200 = diagnostic.Id == SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId;

                // do not offer a code fix for SA1200 when there are multiple namespaces in the source file
                if (isSA1200 && (CountNamespaces(compilationUnit.Members) > 1))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.UsingCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, syntaxRoot, !isSA1200, cancellationToken),
                        nameof(UsingCodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode syntaxRoot, bool forcePreservePlacement, CancellationToken cancellationToken)
        {
            var fileHeader = GetFileHeader(syntaxRoot);
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var usingDirectivesPlacement = forcePreservePlacement ? UsingDirectivesPlacement.Preserve : DeterminePlacement(compilationUnit, settings);

            var usingsHelper = new UsingsSorter(settings, semanticModel, compilationUnit, fileHeader);
            var usingsIndentation = DetermineIndentation(compilationUnit, settings.Indentation, usingDirectivesPlacement);

            // - The strategy is to strip all using directive that are not inside a conditional directive and replace them later with a sorted list at the correct spot
            // - The using directives that are inside a conditional directive are replaced (in sorted order) on the spot.
            // - Conditional directives are not moved, as correctly parsing them is too tricky
            // - No using directives will be stripped when there are multiple namespaces. In that case everything is replaced on the spot.
            List<UsingDirectiveSyntax> stripList;
            var replaceMap = new Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax>();

            // When there are multiple namespaces, do not move using statements outside of them, only sort.
            if (usingDirectivesPlacement == UsingDirectivesPlacement.Preserve)
            {
                BuildReplaceMapForNamespaces(usingsHelper, replaceMap, settings.Indentation, false);
                stripList = new List<UsingDirectiveSyntax>();
            }
            else
            {
                stripList = BuildStripList(usingsHelper);
            }

            BuildReplaceMapForConditionalDirectives(usingsHelper, replaceMap, settings.Indentation, usingsHelper.ConditionalRoot);

            var usingSyntaxRewriter = new UsingSyntaxRewriter(stripList, replaceMap, fileHeader);
            var newSyntaxRoot = usingSyntaxRewriter.Visit(syntaxRoot);

            if (usingDirectivesPlacement == UsingDirectivesPlacement.InsideNamespace)
            {
                newSyntaxRoot = AddUsingsToNamespace(newSyntaxRoot, usingsHelper, usingsIndentation, replaceMap.Any());
            }
            else if (usingDirectivesPlacement == UsingDirectivesPlacement.OutsideNamespace)
            {
                newSyntaxRoot = AddUsingsToCompilationRoot(newSyntaxRoot, usingsHelper, usingsIndentation, replaceMap.Any());
            }

            // Final cleanup
            newSyntaxRoot = StripMultipleBlankLines(newSyntaxRoot);
            newSyntaxRoot = ReAddFileHeader(newSyntaxRoot, fileHeader);

            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }

        private static string DetermineIndentation(CompilationUnitSyntax compilationUnit, IndentationSettings indentationSettings, UsingDirectivesPlacement usingDirectivesPlacement)
        {
            string usingsIndentation;

            if (usingDirectivesPlacement == UsingDirectivesPlacement.InsideNamespace)
            {
                var rootNamespace = compilationUnit.Members.OfType<NamespaceDeclarationSyntax>().First();
                var indentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, rootNamespace);
                usingsIndentation = IndentationHelper.GenerateIndentationString(indentationSettings, indentationLevel + 1);
            }
            else
            {
                usingsIndentation = string.Empty;
            }

            return usingsIndentation;
        }

        private static UsingDirectivesPlacement DeterminePlacement(CompilationUnitSyntax compilationUnit, StyleCopSettings settings)
        {
            switch (settings.OrderingRules.UsingDirectivesPlacement)
            {
            case UsingDirectivesPlacement.InsideNamespace:
                var namespaceCount = CountNamespaces(compilationUnit.Members);

                // Only move using declarations inside the namespace when
                // - There are no global attributes
                // - There is only a single namespace declared at the top level
                // - OrderingSettings.UsingDirectivesPlacement is set to InsideNamespace
                if (compilationUnit.AttributeLists.Any()
                    || compilationUnit.Members.Count > 1
                    || namespaceCount > 1)
                {
                    // Override the user's setting with a more conservative one
                    return UsingDirectivesPlacement.Preserve;
                }

                if (namespaceCount == 0)
                {
                    return UsingDirectivesPlacement.OutsideNamespace;
                }

                return UsingDirectivesPlacement.InsideNamespace;

            case UsingDirectivesPlacement.OutsideNamespace:
                return UsingDirectivesPlacement.OutsideNamespace;

            case UsingDirectivesPlacement.Preserve:
            default:
                return UsingDirectivesPlacement.Preserve;
            }
        }

        private static int CountNamespaces(SyntaxList<MemberDeclarationSyntax> members)
        {
            var result = 0;

            foreach (var namespaceDeclaration in members.OfType<NamespaceDeclarationSyntax>())
            {
                result += 1 + CountNamespaces(namespaceDeclaration.Members);
            }

            return result;
        }

        private static List<UsingDirectiveSyntax> BuildStripList(UsingsSorter usingsHelper)
        {
            return usingsHelper.GetContainedUsings(TreeTextSpan.Empty).ToList();
        }

        private static void BuildReplaceMapForNamespaces(UsingsSorter usingsHelper, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, IndentationSettings indentationSettings, bool qualifyNames)
        {
            var usingsPerNamespace = usingsHelper
                .GetContainedUsings(TreeTextSpan.Empty)
                .GroupBy(ud => ud.Parent)
                .Select(gr => gr.ToList());

            foreach (var usingList in usingsPerNamespace)
            {
                if (usingList.Count > 0)
                {
                    // sort the original using declarations on Span.Start, in order to have the correct replace mapping.
                    usingList.Sort(CompareSpanStart);

                    var indentationSteps = IndentationHelper.GetIndentationSteps(indentationSettings, usingList[0].Parent);
                    if (usingList[0].Parent is NamespaceDeclarationSyntax)
                    {
                        indentationSteps++;
                    }

                    var indentation = IndentationHelper.GenerateIndentationString(indentationSettings, indentationSteps);

                    var modifiedUsings = usingsHelper.GenerateGroupedUsings(usingList, indentation, false, qualifyNames);

                    for (var i = 0; i < usingList.Count; i++)
                    {
                        replaceMap.Add(usingList[i], modifiedUsings[i]);
                    }
                }
            }
        }

        private static void BuildReplaceMapForConditionalDirectives(UsingsSorter usingsHelper, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, IndentationSettings indentationSettings, TreeTextSpan rootSpan)
        {
            foreach (var childSpan in rootSpan.Children)
            {
                var originalUsings = usingsHelper.GetContainedUsings(childSpan);
                if (originalUsings.Count > 0)
                {
                    // sort the original using declarations on Span.Start, in order to have the correct replace mapping.
                    originalUsings.Sort(CompareSpanStart);

                    var indentationSteps = IndentationHelper.GetIndentationSteps(indentationSettings, originalUsings[0].Parent);
                    if (originalUsings[0].Parent is NamespaceDeclarationSyntax)
                    {
                        indentationSteps++;
                    }

                    var indentation = IndentationHelper.GenerateIndentationString(indentationSettings, indentationSteps);

                    var modifiedUsings = usingsHelper.GenerateGroupedUsings(childSpan, indentation, false, qualifyNames: false);

                    for (var i = 0; i < originalUsings.Count; i++)
                    {
                        replaceMap.Add(originalUsings[i], modifiedUsings[i]);
                    }
                }

                BuildReplaceMapForConditionalDirectives(usingsHelper, replaceMap, indentationSettings, childSpan);
            }
        }

        private static int CompareSpanStart(UsingDirectiveSyntax left, UsingDirectiveSyntax right)
        {
            return left.SpanStart - right.SpanStart;
        }

        private static SyntaxNode AddUsingsToNamespace(SyntaxNode newSyntaxRoot, UsingsSorter usingsHelper, string usingsIndentation, bool hasConditionalDirectives)
        {
            var rootNamespace = ((CompilationUnitSyntax)newSyntaxRoot).Members.OfType<NamespaceDeclarationSyntax>().First();
            var withTrailingBlankLine = hasConditionalDirectives || rootNamespace.Members.Any() || rootNamespace.Externs.Any();

            var groupedUsings = usingsHelper.GenerateGroupedUsings(TreeTextSpan.Empty, usingsIndentation, withTrailingBlankLine, qualifyNames: false);
            groupedUsings = groupedUsings.AddRange(rootNamespace.Usings);

            var newRootNamespace = rootNamespace.WithUsings(groupedUsings);
            newSyntaxRoot = newSyntaxRoot.ReplaceNode(rootNamespace, newRootNamespace);

            return newSyntaxRoot;
        }

        private static SyntaxNode AddUsingsToCompilationRoot(SyntaxNode newSyntaxRoot, UsingsSorter usingsHelper, string usingsIndentation, bool hasConditionalDirectives)
        {
            var newCompilationUnit = (CompilationUnitSyntax)newSyntaxRoot;
            var withTrailingBlankLine = hasConditionalDirectives || newCompilationUnit.AttributeLists.Any() || newCompilationUnit.Members.Any() || newCompilationUnit.Externs.Any();

            var groupedUsings = usingsHelper.GenerateGroupedUsings(TreeTextSpan.Empty, usingsIndentation, withTrailingBlankLine, qualifyNames: true);
            groupedUsings = groupedUsings.AddRange(newCompilationUnit.Usings);
            newSyntaxRoot = newCompilationUnit.WithUsings(groupedUsings);

            return newSyntaxRoot;
        }

        private static SyntaxNode StripMultipleBlankLines(SyntaxNode syntaxRoot)
        {
            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();

            var usingDirectives = syntaxRoot.GetAnnotatedNodes(UsingCodeFixAnnotation).Cast<UsingDirectiveSyntax>();

            foreach (var usingDirective in usingDirectives)
            {
                var nextToken = usingDirective.SemicolonToken.GetNextToken(true);

                // start at -1 to compensate for the always present end-of-line.
                var trailingCount = -1;

                // count the blanks lines at the end of the using statement.
                foreach (var trivia in usingDirective.SemicolonToken.TrailingTrivia.Reverse())
                {
                    if (!trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        break;
                    }

                    trailingCount++;
                }

                // count the blank lines at the start of the next token
                var leadingCount = 0;

                foreach (var trivia in nextToken.LeadingTrivia)
                {
                    if (!trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        break;
                    }

                    leadingCount++;
                }

                if ((trailingCount + leadingCount) > 1)
                {
                    var totalStripCount = trailingCount + leadingCount - 1;

                    if (trailingCount > 0)
                    {
                        var trailingStripCount = Math.Min(totalStripCount, trailingCount);

                        var trailingTrivia = usingDirective.SemicolonToken.TrailingTrivia;
                        replaceMap[usingDirective.SemicolonToken] = usingDirective.SemicolonToken.WithTrailingTrivia(trailingTrivia.Take(trailingTrivia.Count - trailingStripCount));
                        totalStripCount -= trailingStripCount;
                    }

                    if (totalStripCount > 0)
                    {
                        replaceMap[nextToken] = nextToken.WithLeadingTrivia(nextToken.LeadingTrivia.Skip(totalStripCount));
                    }
                }
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (original, rewritten) => replaceMap[original]);
            return newSyntaxRoot;
        }

        private static ImmutableArray<SyntaxTrivia> GetFileHeader(SyntaxNode syntaxRoot)
        {
            var onBlankLine = true;
            var hasHeader = false;
            var fileHeaderBuilder = ImmutableArray.CreateBuilder<SyntaxTrivia>();

            var firstToken = syntaxRoot.GetFirstToken(includeZeroWidth: true);
            var firstTokenLeadingTrivia = firstToken.LeadingTrivia;

            int i;
            for (i = 0; i < firstTokenLeadingTrivia.Count; i++)
            {
                bool done = false;
                switch (firstTokenLeadingTrivia[i].Kind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    fileHeaderBuilder.Add(firstTokenLeadingTrivia[i]);
                    onBlankLine = false;
                    break;

                case SyntaxKind.WhitespaceTrivia:
                    fileHeaderBuilder.Add(firstTokenLeadingTrivia[i]);
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    hasHeader = true;
                    fileHeaderBuilder.Add(firstTokenLeadingTrivia[i]);

                    if (onBlankLine)
                    {
                        done = true;
                    }
                    else
                    {
                        onBlankLine = true;
                    }

                    break;

                default:
                    done = true;
                    break;
                }

                if (done)
                {
                    break;
                }
            }

            return hasHeader ? fileHeaderBuilder.ToImmutableArray() : ImmutableArray.Create<SyntaxTrivia>();
        }

        private static SyntaxNode ReAddFileHeader(SyntaxNode syntaxRoot, ImmutableArray<SyntaxTrivia> fileHeader)
        {
            if (fileHeader.IsEmpty)
            {
                // Only re-add the file header if it was stripped.
                return syntaxRoot;
            }

            var firstToken = syntaxRoot.GetFirstToken(includeZeroWidth: true);
            var newLeadingTrivia = firstToken.LeadingTrivia.InsertRange(0, fileHeader);
            return syntaxRoot.ReplaceToken(firstToken, firstToken.WithLeadingTrivia(newLeadingTrivia));
        }

        private class UsingSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly List<UsingDirectiveSyntax> stripList;
            private readonly Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap;
            private readonly ImmutableArray<SyntaxTrivia> fileHeader;
            private LinkedList<SyntaxToken> tokensToStrip = new LinkedList<SyntaxToken>();

            public UsingSyntaxRewriter(List<UsingDirectiveSyntax> stripList, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, ImmutableArray<SyntaxTrivia> fileHeader)
            {
                this.stripList = stripList;
                this.replaceMap = replaceMap;
                this.fileHeader = fileHeader;
            }

            public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
            {
                // The strip list is used to remove using directives that will be moved.
                if (this.stripList.Contains(node))
                {
                    var nextToken = node.SemicolonToken.GetNextToken();

                    if (!nextToken.IsKind(SyntaxKind.None))
                    {
                        var index = TriviaHelper.IndexOfFirstNonBlankLineTrivia(nextToken.LeadingTrivia);
                        if (index != 0)
                        {
                            this.tokensToStrip.AddLast(nextToken);
                        }
                    }

                    return null;
                }

                // The replacement map is used to replace using declarations in place in sorted order (inside directive trivia)
                UsingDirectiveSyntax replacementNode;
                if (this.replaceMap.TryGetValue(node, out replacementNode))
                {
                    return replacementNode;
                }

                return base.VisitUsingDirective(node);
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (this.tokensToStrip.Contains(token))
                {
                    this.tokensToStrip.Remove(token);

                    var index = TriviaHelper.IndexOfFirstNonBlankLineTrivia(token.LeadingTrivia);
                    var newLeadingTrivia = (index == -1) ? SyntaxFactory.TriviaList() : SyntaxFactory.TriviaList(token.LeadingTrivia.Skip(index));
                    return token.WithLeadingTrivia(newLeadingTrivia);
                }

                return base.VisitToken(token);
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                if (this.fileHeader.Contains(trivia))
                {
                    return default(SyntaxTrivia);
                }

                return base.VisitTrivia(trivia);
            }
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            /// <inheritdoc/>
            protected override string CodeActionTitle
                => OrderingResources.UsingCodeFix;

            /// <inheritdoc/>
            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var forcePreserve = diagnostics.All(d => d.Id != SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId);

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                Document newDocument = await GetTransformedDocumentAsync(document, syntaxRoot, forcePreserve, fixAllContext.CancellationToken).ConfigureAwait(false);
                return await newDocument.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
