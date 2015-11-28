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
    /// Implements code fixes for element ordering rules.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ElementOrderCodeFixProvider))]
    [Shared]
    internal class ElementOrderCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId,
                SA1202ElementsMustBeOrderedByAccess.DiagnosticId,
                SA1203ConstantsMustAppearBeforeFields.DiagnosticId,
                SA1204StaticElementsMustAppearBeforeInstanceElements.DiagnosticId,
                SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.ElementOrderCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(ElementOrderCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            var elementOrder = settings.OrderingRules.ElementOrder;
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var indentationOptions = IndentationOptions.FromDocument(document);

            var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return document;
            }

            syntaxRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, indentationOptions);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode UpdateSyntaxRoot(MemberDeclarationSyntax memberDeclaration, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            var parentDeclaration = memberDeclaration.Parent;
            var memberToMove = new MemberOrderHelper(memberDeclaration, elementOrder);

            return OrderMember(memberToMove, GetMembers(parentDeclaration), elementOrder, syntaxRoot, indentationOptions);
        }

        private static SyntaxList<MemberDeclarationSyntax> GetMembers(SyntaxNode node)
        {
            if (node is TypeDeclarationSyntax)
            {
                return ((TypeDeclarationSyntax)node).Members;
            }

            if (node is NamespaceDeclarationSyntax)
            {
                return ((NamespaceDeclarationSyntax)node).Members;
            }

            if (node is CompilationUnitSyntax)
            {
                return ((CompilationUnitSyntax)node).Members;
            }

            throw new ArgumentException($"{nameof(node)} does not have a member list", nameof(node));
        }

        private static SyntaxNode WithMembers(SyntaxNode node, SyntaxList<MemberDeclarationSyntax> members)
        {
            if (node is ClassDeclarationSyntax)
            {
                return ((ClassDeclarationSyntax)node).WithMembers(members);
            }

            if (node is CompilationUnitSyntax)
            {
                return ((CompilationUnitSyntax)node).WithMembers(members);
            }

            if (node is InterfaceDeclarationSyntax)
            {
                return ((InterfaceDeclarationSyntax)node).WithMembers(members);
            }

            if (node is NamespaceDeclarationSyntax)
            {
                return ((NamespaceDeclarationSyntax)node).WithMembers(members);
            }

            if (node is StructDeclarationSyntax)
            {
                return ((StructDeclarationSyntax)node).WithMembers(members);
            }

            throw new ArgumentException($"{nameof(node)} does not have a member list", nameof(node));
        }

        private static SyntaxNode OrderMember(MemberOrderHelper memberOrder, SyntaxList<MemberDeclarationSyntax> members, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            var memberIndex = members.IndexOf(memberOrder.Member);
            MemberOrderHelper target = default(MemberOrderHelper);

            for (var i = memberIndex - 1; i >= 0; --i)
            {
                var orderHelper = new MemberOrderHelper(members[i], elementOrder);
                if (orderHelper.Priority < memberOrder.Priority)
                {
                    target = orderHelper;
                }
                else
                {
                    break;
                }
            }

            return target.Member != null ? MoveMember(syntaxRoot, memberOrder.Member, target.Member, indentationOptions) : syntaxRoot;
        }

        private static SyntaxNode MoveMember(SyntaxNode syntaxRoot, MemberDeclarationSyntax member, MemberDeclarationSyntax targetMember, IndentationOptions indentationOptions)
        {
            var firstToken = syntaxRoot.GetFirstToken();
            var fileHeader = GetFileHeader(firstToken.LeadingTrivia);
            syntaxRoot = syntaxRoot.TrackNodes(member, targetMember, firstToken.Parent);
            var memberToMove = syntaxRoot.GetCurrentNode(member);
            var targetMemberTracked = syntaxRoot.GetCurrentNode(targetMember);
            if (!memberToMove.HasLeadingTrivia)
            {
                var targetIndentationLevel = IndentationHelper.GetIndentationSteps(indentationOptions, targetMember);
                var indentationString = IndentationHelper.GenerateIndentationString(indentationOptions, targetIndentationLevel);
                memberToMove = memberToMove.WithLeadingTrivia(SyntaxFactory.Whitespace(indentationString));
            }

            if (!HasLeadingBlankLines(targetMember)
                && HasLeadingBlankLines(member))
            {
                memberToMove = memberToMove.WithTrailingTrivia(memberToMove.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                memberToMove = memberToMove.WithLeadingTrivia(GetLeadingTriviaWithoutLeadingBlankLines(memberToMove));
            }

            syntaxRoot = syntaxRoot.InsertNodesBefore(targetMemberTracked, new[] { memberToMove });
            var fieldToMoveTracked = syntaxRoot.GetCurrentNodes(member).Last();
            syntaxRoot = syntaxRoot.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
            if (fileHeader.Any())
            {
                var oldFirstToken = syntaxRoot.GetCurrentNode(firstToken.Parent).ChildTokens().First();
                syntaxRoot = syntaxRoot.ReplaceToken(oldFirstToken, oldFirstToken.WithLeadingTrivia(StripFileHeader(oldFirstToken.LeadingTrivia)));
                var newFirstToken = syntaxRoot.GetFirstToken();
                syntaxRoot = syntaxRoot.ReplaceToken(newFirstToken, newFirstToken.WithLeadingTrivia(fileHeader.AddRange(newFirstToken.LeadingTrivia)));
            }

            return syntaxRoot;
        }

        private static SyntaxTriviaList StripFileHeader(SyntaxTriviaList newLeadingTrivia)
        {
            var fileHeader = GetFileHeader(newLeadingTrivia);
            return SyntaxTriviaList.Empty.AddRange(newLeadingTrivia.Skip(fileHeader.Count));
        }

        private static SyntaxTriviaList GetFileHeader(SyntaxTriviaList newLeadingTrivia)
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

            return hasHeader ? SyntaxTriviaList.Empty.AddRange(fileHeader) : SyntaxTriviaList.Empty;
        }

        private static bool HasLeadingBlankLines(SyntaxNode node)
        {
            var firstTriviaIgnoringWhitespace = node.GetLeadingTrivia().FirstOrDefault(x => !x.IsKind(SyntaxKind.WhitespaceTrivia));
            return firstTriviaIgnoringWhitespace.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        private static SyntaxTriviaList GetLeadingTriviaWithoutLeadingBlankLines(SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();

            var skipIndex = 0;
            for (var i = 0; i < leadingTrivia.Count; i++)
            {
                var currentTrivia = leadingTrivia[i];
                if (currentTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    skipIndex = i + 1;
                }
                else if (!currentTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    // Preceded by whitespace
                    skipIndex = i > 0 && leadingTrivia[i - 1].IsKind(SyntaxKind.WhitespaceTrivia) ? i - 1 : i;
                    break;
                }
            }

            return SyntaxFactory.TriviaList(leadingTrivia.Skip(skipIndex));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => OrderingResources.ElementOrderCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var indentationOptions = IndentationOptions.FromDocument(document);
                var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, fixAllContext.CancellationToken);
                var elementOrder = settings.OrderingRules.ElementOrder;
                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                // trackedParents are the elements that have elements that are not in order
                var trackedParents = new HashSet<SyntaxNode>();
                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    trackedParents.Add(memberDeclaration.Parent);
                }

                syntaxRoot = syntaxRoot.TrackNodes(trackedParents);

                foreach (var member in trackedParents)
                {
                    var parent = syntaxRoot.GetCurrentNode(member);
                    syntaxRoot = SortChildren(parent, elementOrder, syntaxRoot, indentationOptions);
                }

                return syntaxRoot;
            }

            private static SyntaxNode SortChildren(SyntaxNode parent, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
            {
                SyntaxList<MemberDeclarationSyntax> memberList = GetMembers(parent);

                List<MemberOrderHelper> orderHelpers = new List<MemberOrderHelper>(memberList.Count);

                foreach (var member in memberList)
                {
                    orderHelpers.Add(new MemberOrderHelper(member, elementOrder));
                }

                syntaxRoot = syntaxRoot.TrackNodes(memberList);

                var orderHelperArray = orderHelpers.ToArray();
                Action<MemberOrderHelper, MemberOrderHelper> moveMember = (i, j) =>
                    {
                        syntaxRoot = MoveMember(syntaxRoot, syntaxRoot.GetCurrentNode(i.Member), syntaxRoot.GetCurrentNode(j.Member), indentationOptions);
                    };
                SortingHelper.InsertionSort(orderHelperArray, (a, b) => b.Priority - a.Priority, moveMember);

                return syntaxRoot;
            }
        }
    }
}
