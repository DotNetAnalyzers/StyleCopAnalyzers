﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for all misaligned using statements.
    /// </summary>
    internal sealed partial class UsingCodeFixProvider
    {
        /// <summary>
        /// Helper class that will sort the using statements and generate new using groups based on the given settings.
        /// </summary>
        private class UsingsSorter
        {
            private readonly SemanticModel semanticModel;
            private readonly ImmutableArray<SyntaxTrivia> fileHeader;
            private readonly bool separateSystemDirectives;
            private readonly bool insertBlankLinesBetweenGroups;

            private SourceMap sourceMap;

            private Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> systemUsings = new Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>>();
            private Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> namespaceUsings = new Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>>();
            private Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> aliases = new Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>>();
            private Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> staticImports = new Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>>();

            public UsingsSorter(StyleCopSettings settings, SemanticModel semanticModel, CompilationUnitSyntax compilationUnit, ImmutableArray<SyntaxTrivia> fileHeader)
            {
                this.separateSystemDirectives = settings.OrderingRules.SystemUsingDirectivesFirst;
                this.insertBlankLinesBetweenGroups = settings.OrderingRules.BlankLinesBetweenUsingGroups == OptionSetting.Require;

                this.semanticModel = semanticModel;
                this.fileHeader = fileHeader;

                this.sourceMap = SourceMap.FromCompilationUnit(compilationUnit);

                this.ProcessUsingDirectives(compilationUnit.Usings);
                this.ProcessMembers(compilationUnit.Members);
            }

            public TreeTextSpan ConditionalRoot
            {
                get { return this.sourceMap.ConditionalRoot; }
            }

            public List<UsingDirectiveSyntax> GetContainedUsings(TreeTextSpan directiveSpan)
            {
                List<UsingDirectiveSyntax> result = new List<UsingDirectiveSyntax>();
                List<UsingDirectiveSyntax> usingsList;

                if (this.systemUsings.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                if (this.namespaceUsings.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                if (this.aliases.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                if (this.staticImports.TryGetValue(directiveSpan, out usingsList))
                {
                    result.AddRange(usingsList);
                }

                return result;
            }

            public SyntaxList<UsingDirectiveSyntax> GenerateGroupedUsings(TreeTextSpan directiveSpan, string indentation, bool withTrailingBlankLine, bool qualifyNames)
            {
                var usingList = new List<UsingDirectiveSyntax>();
                List<SyntaxTrivia> triviaToMove = new List<SyntaxTrivia>();

                usingList.AddRange(this.GenerateUsings(this.systemUsings, directiveSpan, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.namespaceUsings, directiveSpan, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.staticImports, directiveSpan, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.aliases, directiveSpan, indentation, triviaToMove, qualifyNames));

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

                usingList.AddRange(this.GenerateUsings(this.systemUsings, usingsList, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.namespaceUsings, usingsList, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.staticImports, usingsList, indentation, triviaToMove, qualifyNames));
                usingList.AddRange(this.GenerateUsings(this.aliases, usingsList, indentation, triviaToMove, qualifyNames));

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

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> usingsGroup, TreeTextSpan directiveSpan, string indentation, List<SyntaxTrivia> triviaToMove, bool qualifyNames)
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
                    // when there are leading blank lines for the first entry, add them to the triviaToMove collection.
                    var previousIsEndOfLine = true;
                    for (var m = leadingTrivia.Count - 1; m >= 0; m--)
                    {
                        if (leadingTrivia[m].IsDirective)
                        {
                            triviaToMove.InsertRange(0, leadingTrivia.Take(m + 1));
                            break;
                        }

                        if ((i == 0) && leadingTrivia[m].IsKind(SyntaxKind.EndOfLineTrivia))
                        {
                            if (previousIsEndOfLine)
                            {
                                triviaToMove.Insert(0, leadingTrivia[m]);
                            }

                            previousIsEndOfLine = true;
                        }
                        else
                        {
                            previousIsEndOfLine = false;
                        }
                    }

                    // preserve leading trivia (excluding directive trivia), indenting each line as appropriate
                    var newLeadingTrivia = leadingTrivia.Except(triviaToMove).ToList();

                    // strip any leading whitespace on each line (and also blank lines)
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

                if (this.insertBlankLinesBetweenGroups)
                {
                    var last = result[result.Count - 1];

                    result[result.Count - 1] = last.WithTrailingTrivia(last.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                }

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

                return CultureInfo.InvariantCulture.CompareInfo.Compare(left.Name.ToNormalizedString(), right.Name.ToNormalizedString(), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth);
            }

            private bool IsSeparatedSystemUsing(UsingDirectiveSyntax syntax)
            {
                if (!this.separateSystemDirectives
                    || (syntax.Alias != null)
                    || syntax.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    || syntax.HasNamespaceAliasQualifier())
                {
                    return false;
                }

                var namespaceSymbol = this.semanticModel.GetSymbolInfo(syntax.Name).Symbol as INamespaceSymbol;
                if (namespaceSymbol == null)
                {
                    return false;
                }

                var namespaceTypeName = namespaceSymbol.ToDisplayString(FullNamespaceDisplayFormat);
                var firstPart = namespaceTypeName.Split('.')[0];

                return string.Equals(SystemUsingDirectiveIdentifier, firstPart, StringComparison.Ordinal);
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
                    TreeTextSpan containingSpan = this.sourceMap.GetContainingSpan(usingDirective);

                    if (usingDirective.Alias != null)
                    {
                        this.AddUsingDirective(this.aliases, usingDirective, containingSpan);
                    }
                    else if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.None))
                    {
                        this.AddUsingDirective(this.staticImports, usingDirective, containingSpan);
                    }
                    else if (this.IsSeparatedSystemUsing(usingDirective))
                    {
                        this.AddUsingDirective(this.systemUsings, usingDirective, containingSpan);
                    }
                    else
                    {
                        this.AddUsingDirective(this.namespaceUsings, usingDirective, containingSpan);
                    }
                }
            }

            private void AddUsingDirective(Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> container, UsingDirectiveSyntax usingDirective, TreeTextSpan containingSpan)
            {
                List<UsingDirectiveSyntax> usingList;

                if (!container.TryGetValue(containingSpan, out usingList))
                {
                    usingList = new List<UsingDirectiveSyntax>();
                    container.Add(containingSpan, usingList);
                }

                usingList.Add(usingDirective);
            }

            private List<UsingDirectiveSyntax> GenerateUsings(Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> usingsGroup, List<UsingDirectiveSyntax> usingsList, string indentation, List<SyntaxTrivia> triviaToMove, bool qualifyNames)
            {
                var filteredUsingsList = this.FilterRelevantUsings(usingsGroup, usingsList);

                return this.GenerateUsings(filteredUsingsList, indentation, triviaToMove, qualifyNames);
            }

            private List<UsingDirectiveSyntax> FilterRelevantUsings(Dictionary<TreeTextSpan, List<UsingDirectiveSyntax>> usingsGroup, List<UsingDirectiveSyntax> usingsList)
            {
                List<UsingDirectiveSyntax> groupList;

                if (!usingsGroup.TryGetValue(TreeTextSpan.Empty, out groupList))
                {
                    return EmptyUsingsList;
                }

                return groupList.Where(u => usingsList.Contains(u)).ToList();
            }
        }
    }
}
