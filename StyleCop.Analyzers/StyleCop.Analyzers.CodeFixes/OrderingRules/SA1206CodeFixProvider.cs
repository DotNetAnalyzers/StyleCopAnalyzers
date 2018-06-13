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
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using static StyleCop.Analyzers.OrderingRules.ModifierOrderHelper;

    /// <summary>
    /// Implements code fixes for element ordering rules.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1206CodeFixProvider))]
    [Shared]
    internal sealed class SA1206CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1206DeclarationKeywordsMustFollowOrder.DiagnosticId);

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
                        OrderingResources.ModifierOrderCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1206CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return document;
            }

            var modifierTokenToFix = memberDeclaration.FindToken(diagnostic.Location.SourceSpan.Start);
            if (GetModifierType(modifierTokenToFix) == ModifierType.None)
            {
                return document;
            }

            var newModifierList = PartiallySortModifiers(memberDeclaration.GetModifiers(), modifierTokenToFix);
            syntaxRoot = UpdateSyntaxRoot(memberDeclaration, newModifierList, syntaxRoot);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode UpdateSyntaxRoot(MemberDeclarationSyntax memberDeclaration, SyntaxTokenList newModifiers, SyntaxNode syntaxRoot)
        {
            var newDeclaration = memberDeclaration.WithModifiers(newModifiers);

            return syntaxRoot.ReplaceNode(memberDeclaration, newDeclaration);
        }

        /// <summary>
        /// Sorts the complete modifier list to fix all issues.
        /// The trivia will be maintained positionally.
        /// The relative order within the different kinds <seealso cref="ModifierType"/> will not be
        /// changed.
        /// </summary>
        /// <param name="modifiers">All modifiers from the declaration.</param>
        /// <returns>A fully sorted modifier list.</returns>
        private static SyntaxTokenList FullySortModifiers(SyntaxTokenList modifiers)
        {
            var accessModifiers = modifiers.Where(modifier => GetModifierType(modifier) == ModifierType.Access);
            var staticModifiers = modifiers.Where(modifier => GetModifierType(modifier) == ModifierType.Static);
            var otherModifiers = modifiers.Where(modifier => GetModifierType(modifier) == ModifierType.Other);

            return AdjustTrivia(
                accessModifiers
                    .Concat(staticModifiers)
                    .Concat(otherModifiers), modifiers);
        }

        /// <summary>
        /// Sorts the modifier list to fix all issues before <paramref name="modifierToFix"/>
        /// and keep the remaining modifiers untouched.
        /// The trivia will be maintained positionally.
        /// The relative order within the different kinds <seealso cref="ModifierType"/> will not be
        /// changed.
        /// </summary>
        /// <param name="modifiers">All modifiers from the declaration.</param>
        /// <param name="modifierToFix">The modifier with diagnostics.</param>
        /// <returns>A partially sorted modifier list (sorted up to <paramref name="modifierToFix"/>).</returns>
        private static SyntaxTokenList PartiallySortModifiers(SyntaxTokenList modifiers, SyntaxToken modifierToFix)
        {
            var accessModifiers = modifiers.Where(modifier => GetModifierType(modifier) == ModifierType.Access);
            var staticModifiers = modifiers.Where(modifier => GetModifierType(modifier) == ModifierType.Static);
            var otherModifiers = modifiers.Where(modifier => GetModifierType(modifier) == ModifierType.Other);

            IEnumerable<SyntaxToken> beforeIncluding;

            // the modifier to fix is of type other, so we need to sort the whole list of
            // modifier list
            if (GetModifierType(modifierToFix) == ModifierType.Other)
            {
                beforeIncluding = accessModifiers
                    .Concat(staticModifiers)
                    .Concat(otherModifiers);
            }
            else if (GetModifierType(modifierToFix) == ModifierType.Static)
            {
                beforeIncluding = accessModifiers
                    .Concat(staticModifiers.TakeWhile(modifier => modifier != modifierToFix))
                    .Concat(new[] { modifierToFix });
            }
            else
            {
                beforeIncluding = accessModifiers
                    .TakeWhile(modifier => modifier != modifierToFix)
                    .Concat(new[] { modifierToFix });
            }

            var after = modifiers.Where(modifier => !beforeIncluding.Contains(modifier));

            return AdjustTrivia(beforeIncluding.Concat(after), modifiers);
        }

        /// <summary>
        /// Positionally apply the trivia from the old modifier list to the new one.
        /// </summary>
        /// <param name="newModifiers">The new modifiers.</param>
        /// <param name="oldModifiers">The old modifiers.</param>
        /// <returns>New modifier list with trivia from the old one.</returns>
        private static SyntaxTokenList AdjustTrivia(IEnumerable<SyntaxToken> newModifiers, SyntaxTokenList oldModifiers)
        {
            var newTokenList = default(SyntaxTokenList);
            return newTokenList.AddRange(
                newModifiers.Zip(oldModifiers, (m1, m2) => m1.WithTriviaFrom(m2)));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => OrderingResources.ModifierOrderCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                // because all modifiers can be fixed in one run, we
                // only need to store each declaration once
                var trackedDiagnosticMembers = new HashSet<MemberDeclarationSyntax>();
                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    var modifierToken = memberDeclaration.FindToken(diagnostic.Location.SourceSpan.Start);
                    if (GetModifierType(modifierToken) == ModifierType.None)
                    {
                        continue;
                    }

                    trackedDiagnosticMembers.Add(memberDeclaration);
                }

                syntaxRoot = syntaxRoot.TrackNodes(trackedDiagnosticMembers);

                foreach (var member in trackedDiagnosticMembers)
                {
                    var memberDeclaration = syntaxRoot.GetCurrentNode(member);
                    var newModifierList = FullySortModifiers(memberDeclaration.GetModifiers());
                    syntaxRoot = UpdateSyntaxRoot(memberDeclaration, newModifierList, syntaxRoot);
                }

                return syntaxRoot;
            }
        }
    }
}
