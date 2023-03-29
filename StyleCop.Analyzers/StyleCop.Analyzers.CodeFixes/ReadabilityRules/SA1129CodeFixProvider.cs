// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

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
            var newSyntaxRoot = syntaxRoot.ReplaceNode(newExpression, GetReplacementNode(document.Project, newExpression, semanticModel, cancellationToken));

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GetReplacementNode(Project project, SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var newExpression = (BaseObjectCreationExpressionSyntaxWrapper)node;

            var symbolInfo = semanticModel.GetSymbolInfo(newExpression, cancellationToken);
            var namedTypeSymbol = (symbolInfo.Symbol as IMethodSymbol)?.ContainingType;

            var type = GetOrCreateTypeSyntax(project, newExpression, namedTypeSymbol);

            SyntaxNode replacement;

            if (IsType<CancellationToken>(namedTypeSymbol)
                || namedTypeSymbol?.SpecialType == SpecialType.System_IntPtr
                || namedTypeSymbol?.SpecialType == SpecialType.System_UIntPtr
                || IsType<Guid>(namedTypeSymbol))
            {
                if (IsDefaultParameterValue(newExpression))
                {
                    replacement = SyntaxFactory.DefaultExpression(type);
                }
                else
                {
                    string fieldName;
                    if (IsType<CancellationToken>(namedTypeSymbol))
                    {
                        fieldName = nameof(CancellationToken.None);
                    }
                    else if (namedTypeSymbol.SpecialType == SpecialType.System_IntPtr)
                    {
                        fieldName = nameof(IntPtr.Zero);
                    }
                    else if (namedTypeSymbol.SpecialType == SpecialType.System_UIntPtr)
                    {
                        fieldName = nameof(IntPtr.Zero);
                    }
                    else
                    {
                        Debug.Assert(IsType<Guid>(namedTypeSymbol), "Assertion failed: IsType<Guid>(namedTypeSymbol)");
                        fieldName = nameof(Guid.Empty);
                    }

                    replacement = ConstructMemberAccessSyntax(type, fieldName);
                }
            }
            else if (IsEnumWithDefaultMember(namedTypeSymbol, out string memberName))
            {
                replacement = ConstructMemberAccessSyntax(type, memberName);
            }
            else
            {
                replacement = SyntaxFactory.DefaultExpression(type);
            }

            return replacement
                .WithLeadingTrivia(newExpression.SyntaxNode.GetLeadingTrivia())
                .WithTrailingTrivia(newExpression.SyntaxNode.GetTrailingTrivia());
        }

        private static TypeSyntax GetOrCreateTypeSyntax(Project project, BaseObjectCreationExpressionSyntaxWrapper baseObjectCreationExpression, INamedTypeSymbol constructedType)
        {
            if (baseObjectCreationExpression.SyntaxNode is ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
            {
                return objectCreationExpressionSyntax.Type;
            }
            else
            {
                SyntaxGenerator generator = SyntaxGenerator.GetGenerator(project);
                return (TypeSyntax)generator.TypeExpression(constructedType);
            }
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

        private static bool IsDefaultParameterValue(BaseObjectCreationExpressionSyntaxWrapper expression)
        {
            if (expression.SyntaxNode.Parent.Parent is ParameterSyntax parameterSyntax)
            {
                return parameterSyntax.Parent.Parent is BaseMethodDeclarationSyntax;
            }

            return false;
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

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => GetReplacementNode(document.Project, rewrittenNode, semanticModel, fixAllContext.CancellationToken));
            }
        }
    }
}
