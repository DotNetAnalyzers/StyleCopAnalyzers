﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
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
    /// Implements a code fix for <see cref="SA1129DoNotUseDefaultValueTypeConstructor"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1129CodeFixProvider))]
    [Shared]
    internal class SA1129CodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1129DoNotUseDefaultValueTypeConstructor.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1129CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1129CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var newExpression = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(newExpression, GetReplacementNode(newExpression, semanticModel, cancellationToken));

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GetReplacementNode(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var newExpression = (ObjectCreationExpressionSyntax)node;

            var symbolInfo = semanticModel.GetSymbolInfo(newExpression.Type, cancellationToken);
            var namedTypeSymbol = symbolInfo.Symbol as INamedTypeSymbol;

            SyntaxNode replacement = null;
            string memberName = null;

            if (IsType<CancellationToken>(namedTypeSymbol))
            {
                replacement = ConstructMemberAccessSyntax(newExpression.Type, nameof(CancellationToken.None));
            }
            else if (IsEnumWithDefaultMember(namedTypeSymbol, out memberName))
            {
                replacement = ConstructMemberAccessSyntax(newExpression.Type, memberName);
            }
            else
            {
                replacement = SyntaxFactory.DefaultExpression(newExpression.Type);
            }

            return replacement
                .WithLeadingTrivia(newExpression.GetLeadingTrivia())
                .WithTrailingTrivia(newExpression.GetTrailingTrivia());
        }

        /// <summary>
        /// Determines whether a symbol is an instance of a given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <param name="namedTypeSymbol">The symbol.</param>
        /// <returns><see langword="true"/> if the syntax matches the type; <see langword="false"/> otherwise.</returns>
        private static bool IsType<T>(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
            {
                return false;
            }

            var expectedType = typeof(T);

            if (!string.Equals(expectedType.Name, namedTypeSymbol.Name, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(
                expectedType.Namespace,
                namedTypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a given enumeration symbol contains a member with value <c>0</c>.
        /// </summary>
        /// <param name="namedTypeSymbol">The symbol.</param>
        /// <param name="foundMemberName">Will be set to the string name of the member, if one is found.</param>
        /// <returns><see langword="true"/> if the syntax is an enumeration with a value of <c>0</c>; <see langword="false"/> otherwise.</returns>
        private static bool IsEnumWithDefaultMember(INamedTypeSymbol namedTypeSymbol, out string foundMemberName)
        {
            foundMemberName = null;

            if (namedTypeSymbol == null || namedTypeSymbol.TypeKind != TypeKind.Enum)
            {
                return false;
            }

            var foundMembers = namedTypeSymbol
                .GetMembers()
                .Where(m => m.Kind == SymbolKind.Field)
                .OfType<IFieldSymbol>()
                .Where(fs => fs.ConstantValue.Equals(0))
                .ToList();

            if (foundMembers.Count != 1)
            {
                return false;
            }

            foundMemberName = foundMembers[0].Name;
            return true;
        }

        /// <summary>
        /// Gets a qualified member access expression for the given <paramref name="typeSyntax"/>.
        /// </summary>
        /// <param name="typeSyntax">The type syntax from the original constructor.</param>
        /// <param name="memberName">The member name.</param>
        /// <returns>A new member access expression.</returns>
        private static SyntaxNode ConstructMemberAccessSyntax(TypeSyntax typeSyntax, string memberName)
        {
            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                typeSyntax,
                SyntaxFactory.IdentifierName(memberName));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1129CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var nodes = diagnostics.Select(diagnostic => syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true));

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => GetReplacementNode(rewrittenNode, semanticModel, fixAllContext.CancellationToken));
            }
        }
    }
}
