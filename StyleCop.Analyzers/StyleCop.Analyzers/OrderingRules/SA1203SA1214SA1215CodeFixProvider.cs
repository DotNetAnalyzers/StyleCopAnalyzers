// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
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

    /// <summary>
    /// Implements code fixes for <see cref="SA1203ConstantsMustAppearBeforeFields"/>, <see cref="SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements" />
    /// and <see cref="SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements" />.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1203SA1214SA1215CodeFixProvider))]
    [Shared]
    public class SA1203SA1214SA1215CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1203ConstantsMustAppearBeforeFields.DiagnosticId,
                SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements.DiagnosticId,
                SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.SA1203CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        equivalenceKey: nameof(SA1203SA1214SA1215CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var fieldDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var typeDeclarationNode = fieldDeclaration.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            if (typeDeclarationNode == null)
            {
                return document;
            }

            syntaxRoot = MoveField(fieldDeclaration, typeDeclarationNode, syntaxRoot);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode MoveField(FieldDeclarationSyntax fieldDeclaration, TypeDeclarationSyntax typeDeclarationNode, SyntaxNode syntaxRoot)
        {
            var fieldToMove = new MemberOrderHelper(fieldDeclaration);
            foreach (var member in typeDeclarationNode.Members)
            {
                if (!member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    continue;
                }

                var orderHelper = new MemberOrderHelper(member);
                if (orderHelper.ModifierPriority < fieldToMove.ModifierPriority)
                {
                    syntaxRoot = MoveField(syntaxRoot, fieldToMove.Member, member);
                    break;
                }
            }

            return syntaxRoot;
        }

        private static SyntaxNode MoveField(SyntaxNode root, MemberDeclarationSyntax field, MemberDeclarationSyntax firstNonConst)
        {
            var trackedRoot = root.TrackNodes(field, firstNonConst);
            var fieldToMove = trackedRoot.GetCurrentNode(field);
            var firstNonConstTracked = trackedRoot.GetCurrentNode(firstNonConst);
            if (!fieldToMove.HasLeadingTrivia)
            {
                fieldToMove = fieldToMove.WithLeadingTrivia(firstNonConstTracked.GetLeadingTrivia().Where(x => x.IsKind(SyntaxKind.WhitespaceTrivia)).LastOrDefault());
            }

            root = trackedRoot.InsertNodesBefore(firstNonConstTracked, new[] { fieldToMove });
            var fieldToMoveTracked = root.GetCurrentNodes(field).Last();
            return root.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}
