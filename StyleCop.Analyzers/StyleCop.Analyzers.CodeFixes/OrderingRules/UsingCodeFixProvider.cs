// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
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

    /// <summary>
    /// Implements a code fix for all misaligned using statements.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsingCodeFixProvider))]
    [Shared]
    internal sealed class UsingCodeFixProvider : CodeFixProvider
    {
        private static readonly List<UsingDirectiveSyntax> EmptyUsingsList = new List<UsingDirectiveSyntax>();

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId,
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
                if ((diagnostic.Id == SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId)
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

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode syntaxRoot, CancellationToken cancellationToken)
        {
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            var indentationOptions = IndentationOptions.FromDocument(document);

            var usingsHelper = new UsingsHelper(document, compilationUnit);
            var namespaceCount = CountNamespaces(compilationUnit.Members);

            // Only move using declarations inside the namespace when
            // - SA1200 is enabled
            // - There are no global attributes
            // - There is only a single namespace declared at the top level
            var moveInsideNamespace =
                !document.Project.CompilationOptions.IsAnalyzerSuppressed(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId)
                && !compilationUnit.AttributeLists.Any()
                && compilationUnit.Members.Count == 1
                && namespaceCount == 1;

            string usingsIndentation;

            if (moveInsideNamespace)
            {
                var rootNamespace = compilationUnit.Members.OfType<NamespaceDeclarationSyntax>().First();
                var indentationLevel = IndentationHelper.GetIndentationSteps(indentationOptions, rootNamespace);
                usingsIndentation = IndentationHelper.GenerateIndentationString(indentationOptions, indentationLevel + 1);
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
            if (namespaceCount > 1)
            {
                BuildReplaceMapForNamespaces(usingsHelper, replaceMap, indentationOptions);
                stripList = new List<UsingDirectiveSyntax>();
            }
            else
            {
                stripList = usingsHelper.GetContainedUsings(usingsHelper.RootSpan);
            }

            BuildReplaceMapForConditionalDirectives(usingsHelper, replaceMap, indentationOptions, usingsHelper.RootSpan);

            var usingSyntaxRewriter = new UsingSyntaxRewriter(stripList, replaceMap);
            var newSyntaxRoot = usingSyntaxRewriter.Visit(syntaxRoot);

            if (moveInsideNamespace)
            {
                newSyntaxRoot = AddUsingsToNamespace(newSyntaxRoot, usingsHelper, usingsIndentation, replaceMap.Any());
            }
            else if (namespaceCount <= 1)
            {
                newSyntaxRoot = AddUsingsToCompilationRoot(newSyntaxRoot, usingsHelper, usingsIndentation, replaceMap.Any());
            }

            newSyntaxRoot = ReAddFileHeader(syntaxRoot, newSyntaxRoot);

            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return Task.FromResult(newDocument);
        }

        private static SyntaxNode ReAddFileHeader(SyntaxNode syntaxRoot, SyntaxNode newSyntaxRoot)
        {
            var oldFirstToken = syntaxRoot.GetFirstToken();
            if (!oldFirstToken.HasLeadingTrivia)
            {
                return newSyntaxRoot;
            }

            var fileHeader = UsingsHelper.GetFileHeader(oldFirstToken.LeadingTrivia.ToList());
            if (!fileHeader.Any())
            {
                return newSyntaxRoot;
            }

            var newFirstToken = newSyntaxRoot.GetFirstToken();
            return newSyntaxRoot.ReplaceToken(newFirstToken, newFirstToken.WithLeadingTrivia(fileHeader));
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

        private static void BuildReplaceMapForNamespaces(UsingsHelper usingsHelper, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, IndentationOptions indentationOptions)
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

                    var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, usingList[0].Parent);
                    if (usingList[0].Parent is NamespaceDeclarationSyntax)
                    {
                        indentationSteps++;
                    }

                    var indentation = IndentationHelper.GenerateIndentationString(indentationOptions, indentationSteps);

                    var modifiedUsings = usingsHelper.GenerateGroupedUsings(usingList, indentation, false);

                    for (var i = 0; i < usingList.Count; i++)
                    {
                        replaceMap.Add(usingList[i], modifiedUsings[i]);
                    }
                }
            }
        }

        private static void BuildReplaceMapForConditionalDirectives(UsingsHelper usingsHelper, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap, IndentationOptions indentationOptions, DirectiveSpan rootSpan)
        {
            foreach (var childSpan in rootSpan.Children)
            {
                var originalUsings = usingsHelper.GetContainedUsings(childSpan);
                if (originalUsings.Count > 0)
                {
                    // sort the original using declarations on Span.Start, in order to have the correct replace mapping.
                    originalUsings.Sort(CompareSpanStart);

                    var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, originalUsings[0].Parent);
                    if (originalUsings[0].Parent is NamespaceDeclarationSyntax)
                    {
                        indentationSteps++;
                    }

                    var indentation = IndentationHelper.GenerateIndentationString(indentationOptions, indentationSteps);

                    var modifiedUsings = usingsHelper.GenerateGroupedUsings(childSpan, indentation, false);

                    for (var i = 0; i < originalUsings.Count; i++)
                    {
                        replaceMap.Add(originalUsings[i], modifiedUsings[i]);
                    }
                }

                BuildReplaceMapForConditionalDirectives(usingsHelper, replaceMap, indentationOptions, childSpan);
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

            var groupedUsings = usingsHelper.GenerateGroupedUsings(usingsHelper.RootSpan, usingsIndentation, withTrailingBlankLine);
            groupedUsings = groupedUsings.AddRange(rootNamespace.Usings);
            var newRootNamespace = rootNamespace.WithUsings(groupedUsings);

            newSyntaxRoot = newSyntaxRoot.ReplaceNode(rootNamespace, newRootNamespace);
            return newSyntaxRoot;
        }

        private static SyntaxNode AddUsingsToCompilationRoot(SyntaxNode newSyntaxRoot, UsingsHelper usingsHelper, string usingsIndentation, bool hasConditionalDirectives)
        {
            var newCompilationUnit = (CompilationUnitSyntax)newSyntaxRoot;
            var withTrailingBlankLine = hasConditionalDirectives || newCompilationUnit.AttributeLists.Any() || newCompilationUnit.Members.Any() || newCompilationUnit.Externs.Any();

            var groupedUsings = usingsHelper.GenerateGroupedUsings(usingsHelper.RootSpan, usingsIndentation, withTrailingBlankLine);
            groupedUsings = groupedUsings.AddRange(newCompilationUnit.Usings);
            newSyntaxRoot = newCompilationUnit.WithUsings(groupedUsings);
            return newSyntaxRoot;
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
            private readonly DirectiveSpan conditionalDirectiveTree;
            private readonly bool separateSystemDirectives;

            public UsingsHelper(Document document, CompilationUnitSyntax compilationUnit)
            {
                this.conditionalDirectiveTree = DirectiveSpan.BuildConditionalDirectiveTree(compilationUnit);
                this.separateSystemDirectives = !document.Project.CompilationOptions.IsAnalyzerSuppressed(SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives.DiagnosticId);

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

            public SyntaxList<UsingDirectiveSyntax> GenerateGroupedUsings(DirectiveSpan directiveSpan, string indentation, bool withTrailingBlankLine)
            {
                var usingList = new List<UsingDirectiveSyntax>();
                List<SyntaxTrivia> triviaToMove = new List<SyntaxTrivia>();

                usingList.AddRange(this.GenerateUsings(this.NamespaceUsings, directiveSpan, indentation, triviaToMove));
                usingList.AddRange(this.GenerateUsings(this.StaticImports, directiveSpan, indentation, triviaToMove));
                usingList.AddRange(this.GenerateUsings(this.Aliases, directiveSpan, indentation, triviaToMove));

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

            public SyntaxList<UsingDirectiveSyntax> GenerateGroupedUsings(List<UsingDirectiveSyntax> usingsList, string indentation, bool withTrailingBlankLine)
            {
                var usingList = new List<UsingDirectiveSyntax>();
                List<SyntaxTrivia> triviaToMove = new List<SyntaxTrivia>();

                usingList.AddRange(this.GenerateUsings(this.NamespaceUsings, usingsList, indentation, triviaToMove));
                usingList.AddRange(this.GenerateUsings(this.StaticImports, usingsList, indentation, triviaToMove));
                usingList.AddRange(this.GenerateUsings(this.Aliases, usingsList, indentation, triviaToMove));

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

            internal static List<SyntaxTrivia> GetFileHeader(List<SyntaxTrivia> newLeadingTrivia)
            {
                var onBlankLine = false;
                var hasHeader = false;
                var fileHeader = new List<SyntaxTrivia>();
                for (var i = 0; i < newLeadingTrivia.Count; i++)
                {
                    bool done = false;
                    switch (newLeadingTrivia[i].Kind())
                    {
                    case SyntaxKind.SingleLineCommentTrivia:
                    case SyntaxKind.MultiLineCommentTrivia:
                        fileHeader.Add(newLeadingTrivia[i]);
                        onBlankLine = false;
                        hasHeader = true;
                        break;

                    case SyntaxKind.WhitespaceTrivia:
                        fileHeader.Add(newLeadingTrivia[i]);
                        break;

                    case SyntaxKind.EndOfLineTrivia:
                        fileHeader.Add(newLeadingTrivia[i]);

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

                return hasHeader ? fileHeader : new List<SyntaxTrivia>();
            }

            private static List<SyntaxTrivia> StripFileHeader(List<SyntaxTrivia> newLeadingTrivia)
            {
                var fileHeader = GetFileHeader(newLeadingTrivia);
                return newLeadingTrivia.Skip(fileHeader.Count).ToList();
            }

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, DirectiveSpan directiveSpan, string indentation, List<SyntaxTrivia> triviaToMove)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();
                List<UsingDirectiveSyntax> usingsList;

                if (!usingsGroup.TryGetValue(directiveSpan, out usingsList))
                {
                    return result;
                }

                return this.GenerateUsings(usingsList, indentation, triviaToMove);
            }

            private List<UsingDirectiveSyntax> GenerateUsings(List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();

                if (!usingsList.Any())
                {
                    return result;
                }

                for (var i = 0; i < usingsList.Count; i++)
                {
                    var currentUsing = usingsList[i];

                    triviaToMove.AddRange(currentUsing.GetLeadingTrivia().Where(tr => tr.IsDirective || tr.IsKind(SyntaxKind.DisabledTextTrivia)));

                    // preserve leading trivia (excluding directive trivia), indenting each line as appropriate
                    var newLeadingTrivia = currentUsing
                        .GetLeadingTrivia()
                        .Where(tr => !tr.IsDirective && !tr.IsKind(SyntaxKind.DisabledTextTrivia))
                        .ToList();

                    if (i == 0)
                    {
                        newLeadingTrivia = StripFileHeader(newLeadingTrivia);
                    }

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

                    var processedUsing = currentUsing.WithLeadingTrivia(newLeadingTrivia).WithTrailingTrivia(newTrailingTrivia);

                    // filter duplicate using declarations, preferring to keep the one with an alias
                    var existingUsing = result.Find(u => string.Equals(u.Name.ToUnaliasedString(), processedUsing.Name.ToUnaliasedString(), StringComparison.Ordinal));
                    if (existingUsing != null)
                    {
                        if (!existingUsing.HasNamespaceAliasQualifier() && processedUsing.HasNamespaceAliasQualifier())
                        {
                            result.Remove(existingUsing);
                            result.Add(processedUsing);
                        }
                    }
                    else
                    {
                        result.Add(processedUsing);
                    }
                }

                result.Sort(this.CompareUsings);

                return result;
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

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove)
            {
                var filteredUsingsList = this.FilterRelevantUsings(usingsGroup, usingsList);

                return this.GenerateUsings(filteredUsingsList, indentation, triviaToMove);
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
            private List<UsingDirectiveSyntax> stripList;
            private Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap;
            private LinkedList<SyntaxToken> tokensToStrip = new LinkedList<SyntaxToken>();

            public UsingSyntaxRewriter(List<UsingDirectiveSyntax> stripList, Dictionary<UsingDirectiveSyntax, UsingDirectiveSyntax> replaceMap)
            {
                this.stripList = stripList;
                this.replaceMap = replaceMap;
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
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            /// <inheritdoc/>
            protected override string CodeActionTitle
                => OrderingResources.UsingCodeFix;

            /// <inheritdoc/>
            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
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
