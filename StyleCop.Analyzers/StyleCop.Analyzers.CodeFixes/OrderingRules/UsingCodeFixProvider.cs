// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Globalization;
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
    internal sealed class UsingCodeFixProvider : CodeFixProvider
    {
        private static readonly List<UsingDirectiveSyntax> EmptyUsingsList = new List<UsingDirectiveSyntax>();
        private static readonly SyntaxAnnotation UsingCodeFixAnnotation = new SyntaxAnnotation(nameof(UsingCodeFixAnnotation));

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
                // do not offer a code fix for SA1200 when there are multiple namespaces in the source file
                if ((diagnostic.Id == SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId)
                    && (CountNamespaces(compilationUnit.Members) > 1))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.UsingCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, syntaxRoot, cancellationToken),
                        nameof(UsingCodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode syntaxRoot, CancellationToken cancellationToken)
        {
            var fileHeader = GetFileHeader(syntaxRoot);
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var usingsHelper = new UsingsHelper(settings, semanticModel, compilationUnit, fileHeader);
            var namespaceCount = CountNamespaces(compilationUnit.Members);

            // Only move using declarations inside the namespace when
            // - There are no global attributes
            // - There is only a single namespace declared at the top level
            // - OrderingSettings.UsingDirectivesPlacement is set to InsideNamespace
            UsingDirectivesPlacement usingDirectivesPlacement;

            switch (settings.OrderingRules.UsingDirectivesPlacement)
            {
            case UsingDirectivesPlacement.InsideNamespace:
                if (compilationUnit.AttributeLists.Any()
                    || compilationUnit.Members.Count > 1
                    || namespaceCount > 1)
                {
                    // Override the user's setting with a more conservative one
                    usingDirectivesPlacement = UsingDirectivesPlacement.Preserve;
                }
                else if (namespaceCount == 0)
                {
                    usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
                }
                else
                {
                    usingDirectivesPlacement = UsingDirectivesPlacement.InsideNamespace;
                }

                break;

            case UsingDirectivesPlacement.OutsideNamespace:
                usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
                break;

            case UsingDirectivesPlacement.Preserve:
            default:
                usingDirectivesPlacement = UsingDirectivesPlacement.Preserve;
                break;
            }

            string usingsIndentation;

            if (usingDirectivesPlacement == UsingDirectivesPlacement.InsideNamespace)
            {
                var rootNamespace = compilationUnit.Members.OfType<NamespaceDeclarationSyntax>().First();
                var indentationLevel = IndentationHelper.GetIndentationSteps(settings.Indentation, rootNamespace);
                usingsIndentation = IndentationHelper.GenerateIndentationString(settings.Indentation, indentationLevel + 1);
            }
            else
            {
                usingsIndentation = string.Empty;
            }

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
                stripList = usingsHelper.GetContainedUsings(usingsHelper.RootSpan);
            }

            BuildReplaceMapForConditionalDirectives(usingsHelper, replaceMap, settings.Indentation, usingsHelper.RootSpan);

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

        private static int CountNamespaces(SyntaxList<MemberDeclarationSyntax> members)
        {
            var result = 0;

            foreach (var namespaceDeclaration in members.OfType<NamespaceDeclarationSyntax>())
            {
                result += 1 + CountNamespaces(namespaceDeclaration.Members);
            }

            return result;
        }

        private static void BuildReplaceMapForNamespaces(UsingsHelper usingsHelper, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, IndentationSettings indentationSettings, bool qualifyNames)
        {
            var usingsPerNamespace = usingsHelper
                .GetContainedUsings(usingsHelper.RootSpan)
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

        private static void BuildReplaceMapForConditionalDirectives(UsingsHelper usingsHelper, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, IndentationSettings indentationSettings, DirectiveSpan rootSpan)
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

        private static SyntaxNode AddUsingsToNamespace(SyntaxNode newSyntaxRoot, UsingsHelper usingsHelper, string usingsIndentation, bool hasConditionalDirectives)
        {
            var rootNamespace = ((CompilationUnitSyntax)newSyntaxRoot).Members.OfType<NamespaceDeclarationSyntax>().First();
            var withTrailingBlankLine = hasConditionalDirectives || rootNamespace.Members.Any() || rootNamespace.Externs.Any();

            var groupedUsings = usingsHelper.GenerateGroupedUsings(usingsHelper.RootSpan, usingsIndentation, withTrailingBlankLine, qualifyNames: false);
            groupedUsings = groupedUsings.AddRange(rootNamespace.Usings);

            var newRootNamespace = rootNamespace.WithUsings(groupedUsings);
            newSyntaxRoot = newSyntaxRoot.ReplaceNode(rootNamespace, newRootNamespace);

            return newSyntaxRoot;
        }

        private static SyntaxNode AddUsingsToCompilationRoot(SyntaxNode newSyntaxRoot, UsingsHelper usingsHelper, string usingsIndentation, bool hasConditionalDirectives)
        {
            var newCompilationUnit = (CompilationUnitSyntax)newSyntaxRoot;
            var withTrailingBlankLine = hasConditionalDirectives || newCompilationUnit.AttributeLists.Any() || newCompilationUnit.Members.Any() || newCompilationUnit.Externs.Any();

            var groupedUsings = usingsHelper.GenerateGroupedUsings(usingsHelper.RootSpan, usingsIndentation, withTrailingBlankLine, qualifyNames: true);
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
                var count = -1;

                // count the blanks lines at the end of the using statement.
                foreach (var trivia in usingDirective.SemicolonToken.TrailingTrivia.Reverse())
                {
                    if (!trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        break;
                    }

                    count++;
                }

                // count the blank lines at the start of the next token
                foreach (var trivia in nextToken.LeadingTrivia)
                {
                    if (!trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        break;
                    }

                    count++;
                }

                if (count > 1)
                {
                    replaceMap[nextToken] = nextToken.WithLeadingTrivia(nextToken.LeadingTrivia.Skip(count - 1));
                }
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (original, rewritten) => replaceMap[original]);
            return newSyntaxRoot;
        }

        private static ImmutableArray<SyntaxTrivia> GetFileHeader(SyntaxNode syntaxRoot)
        {
            var onBlankLine = false;
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
                    hasHeader = true;
                    break;

                case SyntaxKind.WhitespaceTrivia:
                    fileHeaderBuilder.Add(firstTokenLeadingTrivia[i]);
                    break;

                case SyntaxKind.EndOfLineTrivia:
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

        private class DirectiveSpan
        {
            private List<DirectiveSpan> children = new List<DirectiveSpan>();

            public DirectiveSpan(int start)
            {
                this.Start = start;
            }

            public int Start { get; }

            public int End { get; set; }

            public List<DirectiveSpan> Children
            {
                get { return this.children; }
            }

            public static DirectiveSpan BuildConditionalDirectiveTree(CompilationUnitSyntax compilationUnit)
            {
                var root = new DirectiveSpan(0) { End = compilationUnit.Span.End };
                var directiveStack = new Stack<DirectiveSpan>();
                directiveStack.Push(root);

                for (var directiveTrivia = compilationUnit.GetFirstDirective(); directiveTrivia != null; directiveTrivia = directiveTrivia.GetNextDirective())
                {
                    DirectiveSpan previousDirectiveSpan;
                    DirectiveSpan newDirectiveSpan;

                    switch (directiveTrivia.Kind())
                    {
                    case SyntaxKind.IfDirectiveTrivia:
                        newDirectiveSpan = directiveStack.Peek().AddChild(directiveTrivia.SpanStart);
                        directiveStack.Push(newDirectiveSpan);
                        break;

                    case SyntaxKind.ElifDirectiveTrivia:
                    case SyntaxKind.ElseDirectiveTrivia:
                        previousDirectiveSpan = directiveStack.Pop();
                        previousDirectiveSpan.End = directiveTrivia.SpanStart;

                        newDirectiveSpan = directiveStack.Peek().AddChild(directiveTrivia.SpanStart);
                        directiveStack.Push(newDirectiveSpan);
                        break;

                    case SyntaxKind.EndIfDirectiveTrivia:
                        previousDirectiveSpan = directiveStack.Pop();
                        previousDirectiveSpan.End = directiveTrivia.SpanStart;
                        break;

                    default:
                        // ignore all other directive trivia
                        break;
                    }
                }

                // cleanup the stack if for some reason the tree of directives has not reached closure.
                while (directiveStack.Count > 1)
                {
                    var directiveSpan = directiveStack.Pop();
                    directiveSpan.End = compilationUnit.Span.End;
                }

                return root;
            }

            public DirectiveSpan AddChild(int start)
            {
                var newChild = new DirectiveSpan(start);
                this.children.Add(newChild);
                return newChild;
            }

            public DirectiveSpan FindContainingSpan(SyntaxNode node)
            {
                foreach (var child in this.children)
                {
                    var containingSpan = child.FindContainingSpan(node);
                    if (containingSpan != null)
                    {
                        return containingSpan;
                    }
                }

                if ((node.Span.Start >= this.Start) && (node.Span.End <= this.End))
                {
                    return this;
                }

                return null;
            }
        }

        private class UsingsHelper
        {
            private readonly StyleCopSettings settings;
            private readonly SemanticModel semanticModel;
            private readonly ImmutableArray<SyntaxTrivia> fileHeader;
            private readonly DirectiveSpan conditionalDirectiveTree;
            private readonly bool separateSystemDirectives;

            public UsingsHelper(StyleCopSettings settings, SemanticModel semanticModel, CompilationUnitSyntax compilationUnit, ImmutableArray<SyntaxTrivia> fileHeader)
            {
                this.settings = settings;
                this.semanticModel = semanticModel;
                this.fileHeader = fileHeader;

                this.conditionalDirectiveTree = DirectiveSpan.BuildConditionalDirectiveTree(compilationUnit);
                this.separateSystemDirectives = settings.OrderingRules.SystemUsingDirectivesFirst;

                this.ProcessUsingDirectives(compilationUnit.Usings);
                this.ProcessMembers(compilationUnit.Members);
            }

            public Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> NamespaceUsings { get; } = new Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>>();

            public Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> Aliases { get; } = new Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>>();

            public Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> StaticImports { get; } = new Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>>();

            public DirectiveSpan RootSpan
            {
                get { return this.conditionalDirectiveTree; }
            }

            public List<UsingDirectiveSyntax> GetContainedUsings(DirectiveSpan directiveSpan)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();
                List<UsingDirectiveSyntax> usingsList;

                if (this.NamespaceUsings.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                if (this.Aliases.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                if (this.StaticImports.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                return result;
            }

            public SyntaxList<UsingDirectiveSyntax> GenerateGroupedUsings(DirectiveSpan directiveSpan, string indentation, bool withTrailingBlankLine, bool qualifyNames)
            {
                var usingList = new List<UsingDirectiveSyntax>();
                List<SyntaxTrivia> triviaToMove = new List<SyntaxTrivia>();

                usingList.AddRange(this.GenerateUsings(this.NamespaceUsings, directiveSpan, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.StaticImports, directiveSpan, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.Aliases, directiveSpan, indentation, triviaToMove, qualifyNames));

                if (triviaToMove.Count > 0)
                {
                    var newLeadingTrivia = SyntaxFactory.TriviaList(triviaToMove).AddRange(usingList[0].GetLeadingTrivia());
                    usingList[0] = usingList[0].WithLeadingTrivia(newLeadingTrivia);
                }

                if (withTrailingBlankLine && (usingList.Count > 0))
                {
                    var lastUsing = usingList[usingList.Count - 1];
                    usingList[usingList.Count - 1] = lastUsing.WithTrailingTrivia(lastUsing.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                }

                return SyntaxFactory.List(usingList);
            }

            public SyntaxList<UsingDirectiveSyntax> GenerateGroupedUsings(List<UsingDirectiveSyntax> usingsList, string indentation, bool withTrailingBlankLine, bool qualifyNames)
            {
                var usingList = new List<UsingDirectiveSyntax>();
                List<SyntaxTrivia> triviaToMove = new List<SyntaxTrivia>();

                usingList.AddRange(this.GenerateUsings(this.NamespaceUsings, usingsList, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.StaticImports, usingsList, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.Aliases, usingsList, indentation, triviaToMove, qualifyNames));

                if (triviaToMove.Count > 0)
                {
                    var newLeadingTrivia = SyntaxFactory.TriviaList(triviaToMove).AddRange(usingList[0].GetLeadingTrivia());
                    usingList[0] = usingList[0].WithLeadingTrivia(newLeadingTrivia);
                }

                if (withTrailingBlankLine && (usingList.Count > 0))
                {
                    var lastUsing = usingList[usingList.Count - 1];
                    usingList[usingList.Count - 1] = lastUsing.WithTrailingTrivia(lastUsing.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                }

                return SyntaxFactory.List(usingList);
            }

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, DirectiveSpan directiveSpan, string indentation, List<SyntaxTrivia> triviaToMove, bool qualifyNames)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();
                List<UsingDirectiveSyntax> usingsList;

                if (!usingsGroup.TryGetValue(directiveSpan, out usingsList))
                {
                    return result;
                }

                return this.GenerateUsings(usingsList, indentation, triviaToMove, qualifyNames);
            }

            private List<UsingDirectiveSyntax> GenerateUsings(List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove, bool qualifyNames)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();

                if (!usingsList.Any())
                {
                    return result;
                }

                for (var i = 0; i < usingsList.Count; i++)
                {
                    var currentUsing = usingsList[i];

                    // strip the file header, if the using is the first node in the source file.
                    List<SyntaxTrivia> leadingTrivia;
                    if ((i == 0) && currentUsing.GetFirstToken().GetPreviousToken().IsMissingOrDefault())
                    {
                        leadingTrivia = currentUsing.GetLeadingTrivia().Except(this.fileHeader).ToList();
                    }
                    else
                    {
                        leadingTrivia = currentUsing.GetLeadingTrivia().ToList();
                    }

                    // when there is a directive trivia, add it (and any trivia before it) to the triviaToMove collection.
                    for (var m = leadingTrivia.Count - 1; m >= 0; m--)
                    {
                        if (leadingTrivia[m].IsDirective)
                        {
                            triviaToMove.AddRange(leadingTrivia.Take(m + 1));
                            break;
                        }
                    }

                    // preserve leading trivia (excluding directive trivia), indenting each line as appropriate
                    var newLeadingTrivia = leadingTrivia.Except(triviaToMove).ToList();

                    // strip any leading whitespace on each line (and also all blank lines)
                    var k = 0;
                    var startOfLine = true;
                    while (k < newLeadingTrivia.Count)
                    {
                        switch (newLeadingTrivia[k].Kind())
                        {
                        case SyntaxKind.WhitespaceTrivia:
                            newLeadingTrivia.RemoveAt(k);
                            break;

                        case SyntaxKind.EndOfLineTrivia:
                            if (startOfLine)
                            {
                                newLeadingTrivia.RemoveAt(k);
                            }
                            else
                            {
                                startOfLine = true;
                                k++;
                            }

                            break;

                        default:
                            startOfLine = newLeadingTrivia[k].IsDirective;
                            k++;
                            break;
                        }
                    }

                    for (var j = newLeadingTrivia.Count - 1; j >= 0; j--)
                    {
                        if (newLeadingTrivia[j].IsKind(SyntaxKind.EndOfLineTrivia))
                        {
                            newLeadingTrivia.Insert(j + 1, SyntaxFactory.Whitespace(indentation));
                        }
                    }

                    newLeadingTrivia.Insert(0, SyntaxFactory.Whitespace(indentation));

                    // preserve trailing trivia, adding an end of line if necessary.
                    var currentTrailingTrivia = currentUsing.GetTrailingTrivia();
                    var newTrailingTrivia = currentTrailingTrivia;
                    if (!currentTrailingTrivia.Any() || !currentTrailingTrivia.Last().IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        newTrailingTrivia = newTrailingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
                    }

                    var processedUsing = (qualifyNames ? this.QualifyUsingDirective(currentUsing) : currentUsing)
                        .WithLeadingTrivia(newLeadingTrivia)
                        .WithTrailingTrivia(newTrailingTrivia)
                        .WithAdditionalAnnotations(UsingCodeFixAnnotation);

                    result.Add(processedUsing);
                }

                result.Sort(this.CompareUsings);

                return result;
            }

            private UsingDirectiveSyntax QualifyUsingDirective(UsingDirectiveSyntax usingDirective)
            {
                NameSyntax originalName = usingDirective.Name;
                NameSyntax rewrittenName;
                switch (originalName.Kind())
                {
                case SyntaxKind.QualifiedName:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.GenericName:
                    if (originalName.Parent.IsKind(SyntaxKind.UsingDirective)
                        || originalName.Parent.IsKind(SyntaxKind.TypeArgumentList))
                    {
                        var symbol = this.semanticModel.GetSymbolInfo(originalName, cancellationToken: CancellationToken.None).Symbol;
                        if (symbol == null)
                        {
                            rewrittenName = originalName;
                            break;
                        }

                        if (symbol is INamespaceSymbol)
                        {
                            // TODO: Preserve inner trivia
                            string fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            NameSyntax replacement = SyntaxFactory.ParseName(fullName);
                            if (!originalName.DescendantNodesAndSelf().OfType<AliasQualifiedNameSyntax>().Any())
                            {
                                replacement = replacement.ReplaceNodes(
                                    replacement.DescendantNodesAndSelf().OfType<AliasQualifiedNameSyntax>(),
                                    (originalNode2, rewrittenNode2) => rewrittenNode2.Name);
                            }

                            rewrittenName = replacement.WithTriviaFrom(originalName);
                            break;
                        }
                        else if (symbol is INamedTypeSymbol)
                        {
                            // TODO: Preserve inner trivia
                            // TODO: simplify after qualification
                            string fullName;
                            if (SpecialTypeHelper.IsPredefinedType(((INamedTypeSymbol)symbol).OriginalDefinition.SpecialType))
                            {
                                fullName = "global::System." + symbol.Name;
                            }
                            else
                            {
                                fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            }

                            NameSyntax replacement = SyntaxFactory.ParseName(fullName);
                            if (!originalName.DescendantNodesAndSelf().OfType<AliasQualifiedNameSyntax>().Any())
                            {
                                replacement = replacement.ReplaceNodes(
                                    replacement.DescendantNodesAndSelf().OfType<AliasQualifiedNameSyntax>(),
                                    (originalNode2, rewrittenNode2) => rewrittenNode2.Name);
                            }

                            rewrittenName = replacement.WithTriviaFrom(originalName);
                            break;
                        }
                        else
                        {
                            rewrittenName = originalName;
                            break;
                        }
                    }
                    else
                    {
                        rewrittenName = originalName;
                        break;
                    }

                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.PredefinedType:
                default:
                    rewrittenName = originalName;
                    break;
                }

                if (rewrittenName == originalName)
                {
                    return usingDirective;
                }

                return usingDirective.ReplaceNode(originalName, rewrittenName);
            }

            private int CompareUsings(UsingDirectiveSyntax left, UsingDirectiveSyntax right)
            {
                if ((left.Alias != null) && (right.Alias != null))
                {
                    return CultureInfo.InvariantCulture.CompareInfo.Compare(left.Alias.Name.Identifier.ValueText, right.Alias.Name.Identifier.ValueText, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth);
                }

                bool leftIsSystem = this.IsSeparatedSystemUsing(left);
                bool rightIsSystem = this.IsSeparatedSystemUsing(right);
                if (leftIsSystem != rightIsSystem)
                {
                    return leftIsSystem ? -1 : 1;
                }

                return CultureInfo.InvariantCulture.CompareInfo.Compare(left.Name.ToNormalizedString(), right.Name.ToNormalizedString(), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth);
            }

            private bool IsSeparatedSystemUsing(UsingDirectiveSyntax syntax)
            {
                if (!this.separateSystemDirectives)
                {
                    return false;
                }

                return syntax.Alias == null
                    && syntax.IsSystemUsingDirective()
                    && !syntax.HasNamespaceAliasQualifier()
                    && syntax.StaticKeyword.IsKind(SyntaxKind.None);
            }

            private void ProcessMembers(SyntaxList<MemberDeclarationSyntax> members)
            {
                foreach (var namespaceDeclaration in members.OfType<NamespaceDeclarationSyntax>())
                {
                    this.ProcessUsingDirectives(namespaceDeclaration.Usings);
                    this.ProcessMembers(namespaceDeclaration.Members);
                }
            }

            private void ProcessUsingDirectives(SyntaxList<UsingDirectiveSyntax> usingDirectives)
            {
                foreach (var usingDirective in usingDirectives)
                {
                    var owningSpan = this.conditionalDirectiveTree.FindContainingSpan(usingDirective);

                    if (usingDirective.Alias != null)
                    {
                        this.AddUsingDirective(this.Aliases, usingDirective, owningSpan);
                    }
                    else if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.None))
                    {
                        this.AddUsingDirective(this.StaticImports, usingDirective, owningSpan);
                    }
                    else
                    {
                        this.AddUsingDirective(this.NamespaceUsings, usingDirective, owningSpan);
                    }
                }
            }

            private void AddUsingDirective(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> container, UsingDirectiveSyntax usingDirective, DirectiveSpan owningSpan)
            {
                List<UsingDirectiveSyntax> usingList;

                if (!container.TryGetValue(owningSpan, out usingList))
                {
                    usingList = new List<UsingDirectiveSyntax>();
                    container.Add(owningSpan, usingList);
                }

                usingList.Add(usingDirective);
            }

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove, bool qualifyNames)
            {
                var filteredUsingsList = this.FilterRelevantUsings(usingsGroup, usingsList);

                return this.GenerateUsings(filteredUsingsList, indentation, triviaToMove, qualifyNames);
            }

            private List<UsingDirectiveSyntax> FilterRelevantUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, List<UsingDirectiveSyntax> usingsList)
            {
                List<UsingDirectiveSyntax> groupList;

                if (!usingsGroup.TryGetValue(this.RootSpan, out groupList))
                {
                    return EmptyUsingsList;
                }

                return groupList.Where(u => usingsList.Contains(u)).ToList();
            }
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

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                Document newDocument = await GetTransformedDocumentAsync(document, syntaxRoot, fixAllContext.CancellationToken).ConfigureAwait(false);
                return await newDocument.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
