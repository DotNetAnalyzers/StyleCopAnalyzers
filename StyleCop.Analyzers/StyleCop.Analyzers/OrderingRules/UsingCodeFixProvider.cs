﻿namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for all misaligned using statements.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsingCodeFixProvider))]
    [Shared]
    public sealed class UsingCodeFixProvider : CodeFixProvider
    {
        private static readonly List<UsingDirectiveSyntax> EmptyUsingsList = new List<UsingDirectiveSyntax>();

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(
                SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId,
                SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives.DiagnosticId,
                SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives.DiagnosticId,
                SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace.DiagnosticId,
                SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName.DiagnosticId,
                SA1216UsingStaticDirectivesMustBePlacedAfterOtherUsingDirectives.DiagnosticId,
                SA1217UsingStaticDirectivesMustBeOrderedAlphabetically.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                // do not offer a code fix for SA1200 when there are multiple namespaces in the source file
                if ((diagnostic.Id == SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId)
                    && (CountNamespaces(compilationUnit.Members) > 1))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(OrderingResources.UsingCodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(UsingCodeFixProvider)), diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            var indentationOptions = IndentationOptions.FromDocument(document);

            var usingsHelper = new UsingsHelper(compilationUnit);
            var namespaceCount = CountNamespaces(compilationUnit.Members);

            // Only move using declarations inside the namespace when
            // - SA1200 is enabled
            // - There are no global attributes
            // - There is only a single namespace declared at the top level
            var moveInsideNamespace =
                !document.IsAnalyzerSuppressed(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId)
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
            private DirectiveSpan conditionalDirectiveTree;

            public UsingsHelper(CompilationUnitSyntax compilationUnit)
            {
                this.conditionalDirectiveTree = DirectiveSpan.BuildConditionalDirectiveTree(compilationUnit);

                this.ProcessUsingDirectives(compilationUnit.Usings);
                this.ProcessMembers(compilationUnit.Members);
            }

            public Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> SystemUsings { get; } = new Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>>();

            public Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> OtherUsings { get; } = new Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>>();

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

                if (this.SystemUsings.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                if (this.OtherUsings.TryGetValue(directiveSpan, out usingsList))
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

                usingList.AddRange(GenerateUsings(this.SystemUsings, directiveSpan, indentation, triviaToMove, false));
                usingList.AddRange(GenerateUsings(this.OtherUsings, directiveSpan, indentation, triviaToMove, usingList.Any()));
                usingList.AddRange(GenerateUsings(this.Aliases, directiveSpan, indentation, triviaToMove, usingList.Any()));
                usingList.AddRange(GenerateUsings(this.StaticImports, directiveSpan, indentation, triviaToMove, usingList.Any()));

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

                usingList.AddRange(this.GenerateUsings(this.SystemUsings, usingsList, indentation, triviaToMove, false));
                usingList.AddRange(this.GenerateUsings(this.OtherUsings, usingsList, indentation, triviaToMove, usingList.Any()));
                usingList.AddRange(this.GenerateUsings(this.Aliases, usingsList, indentation, triviaToMove, usingList.Any()));
                usingList.AddRange(this.GenerateUsings(this.StaticImports, usingsList, indentation, triviaToMove, usingList.Any()));

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

            private static List<UsingDirectiveSyntax> GenerateUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, DirectiveSpan directiveSpan, string indentation, List<SyntaxTrivia> triviaToMove, bool leadingBlankLine)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();
                List<UsingDirectiveSyntax> usingsList;

                if (!usingsGroup.TryGetValue(directiveSpan, out usingsList))
                {
                    return result;
                }

                return GenerateUsings(usingsList, indentation, triviaToMove, leadingBlankLine);
            }

            private static List<UsingDirectiveSyntax> GenerateUsings(List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove, bool leadingBlankLine)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();

                if (!usingsList.Any())
                {
                    return result;
                }

                for (var i = 0; i < usingsList.Count; i++)
                {
                    var currentUsing = usingsList[i];

                    triviaToMove.AddRange(currentUsing.GetLeadingTrivia().Where(tr => tr.IsDirective));

                    // preserve leading trivia (excluding directive trivia), indenting each line as appropriate
                    var newLeadingTrivia = currentUsing
                        .GetLeadingTrivia()
                        .WithoutLeadingWhitespace()
                        .Where(tr => !tr.IsDirective)
                        .ToList();

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

                result.Sort(CompareUsings);

                if (leadingBlankLine)
                {
                    result[0] = result[0].WithLeadingTrivia(result[0].GetLeadingTrivia().Insert(0, SyntaxFactory.CarriageReturnLineFeed));
                }

                return result;
            }

            private static int CompareUsings(UsingDirectiveSyntax left, UsingDirectiveSyntax right)
            {
                if ((left.Alias != null) && (right.Alias != null))
                {
                    return string.CompareOrdinal(left.Alias.Name.Identifier.ValueText, right.Alias.Name.Identifier.ValueText);
                }

                return string.CompareOrdinal(left.Name.ToUnaliasedString(), right.Name.ToUnaliasedString());
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
                    else if (usingDirective.IsSystemUsingDirective())
                    {
                        this.AddUsingDirective(this.SystemUsings, usingDirective, owningSpan);
                    }
                    else
                    {
                        this.AddUsingDirective(this.OtherUsings, usingDirective, owningSpan);
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

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<DirectiveSpan, List<UsingDirectiveSyntax>> usingsGroup, List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove, bool leadingBlankLine)
            {
                var filteredUsingsList = this.FilterRelevantUsings(usingsGroup, usingsList);

                return GenerateUsings(filteredUsingsList, indentation, triviaToMove, leadingBlankLine);
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
    }
}
