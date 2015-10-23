// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
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
                SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements.DiagnosticId,
                SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements.DiagnosticId);

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
            var orderingChecks = await GetEnabledRulesForDocumentAsync(document, cancellationToken).ConfigureAwait(false);
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var indentationOptions = IndentationOptions.FromDocument(document);

            var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return document;
            }

            syntaxRoot = UpdateSyntaxRoot(memberDeclaration, orderingChecks, syntaxRoot, indentationOptions);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static async Task<ElementOrderingChecks> GetEnabledRulesForDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            SemanticModel semanticModel;
            if (!document.TryGetSemanticModel(out semanticModel))
            {
                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            }

            return ElementOrderingChecks.GetElementOrderingChecksForSemanticModel(semanticModel);
        }

        private static SyntaxNode UpdateSyntaxRoot(MemberDeclarationSyntax memberDeclaration, ElementOrderingChecks checks, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            var parentDeclaration = memberDeclaration.Parent;
            var memberToMove = new MemberOrderHelper(memberDeclaration, checks);

            if (parentDeclaration is TypeDeclarationSyntax)
            {
                return HandleTypeDeclaration(memberToMove, (TypeDeclarationSyntax)parentDeclaration, checks, syntaxRoot, indentationOptions);
            }

            if (parentDeclaration is NamespaceDeclarationSyntax)
            {
                return HandleNamespaceDeclaration(memberToMove, (NamespaceDeclarationSyntax)parentDeclaration, checks, syntaxRoot, indentationOptions);
            }

            if (parentDeclaration is CompilationUnitSyntax)
            {
                return HandleCompilationUnitDeclaration(memberToMove, (CompilationUnitSyntax)parentDeclaration, checks, syntaxRoot, indentationOptions);
            }

            return syntaxRoot;
        }

        private static SyntaxNode HandleTypeDeclaration(MemberOrderHelper memberOrder, TypeDeclarationSyntax typeDeclarationNode, ElementOrderingChecks checks, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            return OrderMember(memberOrder, typeDeclarationNode.Members, checks, syntaxRoot, indentationOptions);
        }

        private static SyntaxNode HandleCompilationUnitDeclaration(MemberOrderHelper memberOrder, CompilationUnitSyntax compilationUnitDeclaration, ElementOrderingChecks checks, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            return OrderMember(memberOrder, compilationUnitDeclaration.Members, checks, syntaxRoot, indentationOptions);
        }

        private static SyntaxNode HandleNamespaceDeclaration(MemberOrderHelper memberOrder, NamespaceDeclarationSyntax namespaceDeclaration, ElementOrderingChecks checks, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            return OrderMember(memberOrder, namespaceDeclaration.Members, checks, syntaxRoot, indentationOptions);
        }

        private static SyntaxNode OrderMember(MemberOrderHelper memberOrder, SyntaxList<MemberDeclarationSyntax> members, ElementOrderingChecks checks, SyntaxNode syntaxRoot, IndentationOptions indentationOptions)
        {
            var memberIndex = members.IndexOf(memberOrder.Member);
            MemberOrderHelper target = default(MemberOrderHelper);

            for (var i = memberIndex - 1; i >= 0; --i)
            {
                var orderHelper = new MemberOrderHelper(members[i], checks);
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
                var orderingChecks = await GetEnabledRulesForDocumentAsync(document, fixAllContext.CancellationToken).ConfigureAwait(false);
                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                var trackedDiagnosticMembers = new List<MemberDeclarationSyntax>();
                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    trackedDiagnosticMembers.Add(memberDeclaration);
                }

                syntaxRoot = syntaxRoot.TrackNodes(trackedDiagnosticMembers);

                foreach (var member in trackedDiagnosticMembers)
                {
                    var memberDeclaration = syntaxRoot.GetCurrentNode(member);
                    syntaxRoot = UpdateSyntaxRoot(memberDeclaration, orderingChecks, syntaxRoot, indentationOptions);
                }

                return syntaxRoot;
            }
        }
    }
}
