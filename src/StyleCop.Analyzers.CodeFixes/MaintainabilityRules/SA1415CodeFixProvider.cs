// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// Implements a code fix for <see cref="SA1415DoNotUsePublicOnMembersOfInternalTypes"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1415CodeFixProvider))]
[Shared]
internal sealed class SA1415CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => CustomFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            if (!token.IsKind(SyntaxKind.PublicKeyword) || token.Parent is not MemberDeclarationSyntax member)
            {
                continue;
            }

            var symbol = GetDeclaredSymbol(semanticModel, member, context.CancellationToken);
            if (!await CanOfferFixAsync(context.Document.Project.Solution, member, symbol, context.CancellationToken).ConfigureAwait(false))
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    MaintainabilityResources.SA1415CodeFix,
                    cancellationToken => ApplyFixAsync(context.Document, diagnostic, cancellationToken),
                    nameof(SA1415CodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Solution> ApplyFixAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var solution = document.Project.Solution;
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return solution;
        }

        var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
        if (!token.IsKind(SyntaxKind.PublicKeyword) || token.Parent is not MemberDeclarationSyntax member)
        {
            return solution;
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var symbol = GetDeclaredSymbol(semanticModel, member, cancellationToken);
        var targets = await TryCollectTargetsAsync(solution, member, symbol, cancellationToken).ConfigureAwait(false);
        if (targets is null)
        {
            return solution;
        }

        return await RewriteTargetsAsync(solution, targets, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<bool> CanOfferFixAsync(
        Solution solution,
        MemberDeclarationSyntax member,
        ISymbol? symbol,
        CancellationToken cancellationToken)
    {
        var targets = await TryCollectTargetsAsync(solution, member, symbol, cancellationToken).ConfigureAwait(false);
        return targets is not null;
    }

    private static async Task<List<MemberDeclarationSyntax>?> TryCollectTargetsAsync(
        Solution solution,
        MemberDeclarationSyntax member,
        ISymbol? symbol,
        CancellationToken cancellationToken)
    {
        var targets = new List<MemberDeclarationSyntax>();
        var seen = new HashSet<NodeKey>();
        if (!TryAddDeclarations(solution, member, symbol, targets, seen, cancellationToken))
        {
            return null;
        }

        if (symbol is not null && IsOverridable(symbol))
        {
            if (!await TryAddOverridesAsync(solution, symbol, targets, seen, cancellationToken).ConfigureAwait(false))
            {
                return null;
            }
        }

        return targets.Count > 0 ? targets : null;
    }

    private static bool TryAddDeclarations(
        Solution solution,
        MemberDeclarationSyntax? fallback,
        ISymbol? symbol,
        List<MemberDeclarationSyntax> targets,
        HashSet<NodeKey> seen,
        CancellationToken cancellationToken)
    {
        var declarations = new List<MemberDeclarationSyntax>();
        if (!TryGetDeclarations(solution, fallback, symbol, declarations, cancellationToken))
        {
            return false;
        }

        foreach (var declaration in declarations)
        {
            if (!AccessorChangeIsSafe(declaration))
            {
                return false;
            }

            AddTarget(targets, seen, declaration);
        }

        return true;
    }

    private static bool TryGetDeclarations(
        Solution solution,
        MemberDeclarationSyntax? fallback,
        ISymbol? symbol,
        List<MemberDeclarationSyntax> declarations,
        CancellationToken cancellationToken)
    {
        if (symbol is null)
        {
            if (fallback is null || HasPartialOrVirtualModifier(fallback))
            {
                return false;
            }

            declarations.Add(fallback);
            return true;
        }

        var symbols = new List<ISymbol> { symbol };
        AddOtherPartial(symbol, symbols);

        var sawPartial = false;
        foreach (var part in symbols)
        {
            foreach (var reference in part.DeclaringSyntaxReferences)
            {
                var declaration = GetMemberDeclaration(reference.GetSyntax(cancellationToken));
                if (declaration is null)
                {
                    continue;
                }

                if (IsGenerated(solution, declaration, cancellationToken))
                {
                    return false;
                }

                if (HasPartialModifier(declaration))
                {
                    sawPartial = true;
                }

                declarations.Add(declaration);
            }
        }

        if (declarations.Count == 0)
        {
            if (fallback is null || HasPartialOrVirtualModifier(fallback))
            {
                return false;
            }

            declarations.Add(fallback);
        }

        if (sawPartial && UniqueDeclarationCount(declarations) < 2)
        {
            return false;
        }

        return true;
    }

    private static async Task<bool> TryAddOverridesAsync(
        Solution solution,
        ISymbol symbol,
        List<MemberDeclarationSyntax> targets,
        HashSet<NodeKey> seen,
        CancellationToken cancellationToken)
    {
        var containingType = symbol.ContainingType;
        var assembly = symbol.ContainingAssembly;
        if (containingType is null || assembly is null)
        {
            return false;
        }

        foreach (var project in solution.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!project.SupportsCompilation)
            {
                continue;
            }

            var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            if (compilation is null)
            {
                return false;
            }

            if (!CompilationMayContainOverrides(compilation, assembly))
            {
                continue;
            }

            var overrides = new List<ISymbol>();
            CollectOverrides(compilation.Assembly.GlobalNamespace, containingType, symbol, overrides, cancellationToken);
            foreach (var overrideSymbol in overrides)
            {
                if (!TryAddDeclarations(solution, fallback: null, overrideSymbol, targets, seen, cancellationToken))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool CompilationMayContainOverrides(Compilation compilation, IAssemblySymbol assembly)
    {
        if (compilation.Assembly.Equals(assembly))
        {
            return true;
        }

        var name = assembly.Identity.Name;
        foreach (var reference in compilation.ReferencedAssemblyNames)
        {
            if (string.Equals(reference.Name, name, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static void CollectOverrides(
        INamespaceSymbol namespaceSymbol,
        INamedTypeSymbol baseType,
        ISymbol symbol,
        List<ISymbol> overrides,
        CancellationToken cancellationToken)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            switch (member)
            {
            case INamespaceSymbol child:
                CollectOverrides(child, baseType, symbol, overrides, cancellationToken);
                break;

            case INamedTypeSymbol type:
                CollectOverrideTypes(type, baseType, symbol, overrides, cancellationToken);
                break;
            }
        }
    }

    private static void CollectOverrideTypes(
        INamedTypeSymbol type,
        INamedTypeSymbol baseType,
        ISymbol symbol,
        List<ISymbol> overrides,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (DerivesFrom(type, baseType))
        {
            foreach (var member in type.GetMembers(symbol.Name))
            {
                if (member.IsOverride && ChainContains(member, symbol))
                {
                    overrides.Add(member);
                }
            }
        }

        foreach (var nested in type.GetTypeMembers())
        {
            CollectOverrideTypes(nested, baseType, symbol, overrides, cancellationToken);
        }
    }

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        var target = baseType.OriginalDefinition;
        var seen = new HashSet<INamedTypeSymbol>();
        for (var current = type.BaseType; current is not null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
        {
            if (!seen.Add(current))
            {
                return false;
            }

            if (current.Equals(baseType) || current.OriginalDefinition.Equals(target))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ChainContains(ISymbol implementation, ISymbol symbol)
    {
        var target = symbol.OriginalDefinition;
        var seen = new HashSet<ISymbol>();
        for (var current = implementation; current is not null; current = GetOverridden(current))
        {
            if (!seen.Add(current))
            {
                return false;
            }

            if (current.Equals(symbol) || current.OriginalDefinition.Equals(target))
            {
                return true;
            }
        }

        return false;
    }

    private static ISymbol? GetOverridden(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol method => method.OverriddenMethod,
            IPropertySymbol property => property.OverriddenProperty,
            IEventSymbol ev => ev.OverriddenEvent,
            _ => null,
        };
    }

    private static async Task<Solution> RewriteTargetsAsync(
        Solution solution,
        List<MemberDeclarationSyntax> targets,
        CancellationToken cancellationToken)
    {
        var byTree = new Dictionary<SyntaxTree, List<MemberDeclarationSyntax>>();
        foreach (var target in targets)
        {
            if (!byTree.TryGetValue(target.SyntaxTree, out var list))
            {
                list = new List<MemberDeclarationSyntax>();
                byTree.Add(target.SyntaxTree, list);
            }

            list.Add(target);
        }

        var updated = solution;
        foreach (var pair in byTree)
        {
            var targetDocument = updated.GetDocument(pair.Key);
            if (targetDocument is null)
            {
                return solution;
            }

            var targetRoot = await targetDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (targetRoot is null)
            {
                return solution;
            }

            var newRoot = targetRoot.ReplaceNodes(pair.Value, (original, rewritten) => RewriteMember(rewritten));
            updated = targetDocument.WithSyntaxRoot(newRoot).Project.Solution;
        }

        return updated;
    }

    private static MemberDeclarationSyntax RewriteMember(MemberDeclarationSyntax member)
    {
        var updated = ReplacePublicToken(member);
        return updated switch
        {
            PropertyDeclarationSyntax property when property.AccessorList is not null
                => property.WithAccessorList(RemoveRedundantAccessorModifiers(property.AccessorList)),
            IndexerDeclarationSyntax indexer when indexer.AccessorList is not null
                => indexer.WithAccessorList(RemoveRedundantAccessorModifiers(indexer.AccessorList)),
            EventDeclarationSyntax ev when ev.AccessorList is not null
                => ev.WithAccessorList(RemoveRedundantAccessorModifiers(ev.AccessorList)),
            _ => updated,
        };
    }

    private static MemberDeclarationSyntax ReplacePublicToken(MemberDeclarationSyntax member)
    {
        foreach (var modifier in member.Modifiers())
        {
            if (modifier.IsKind(SyntaxKind.PublicKeyword))
            {
                var replacement = SyntaxFactory.Token(modifier.LeadingTrivia, SyntaxKind.InternalKeyword, modifier.TrailingTrivia);
                return (MemberDeclarationSyntax)member.ReplaceToken(modifier, replacement);
            }
        }

        return member;
    }

    private static AccessorListSyntax RemoveRedundantAccessorModifiers(AccessorListSyntax accessorList)
    {
        var accessors = new List<AccessorDeclarationSyntax>(accessorList.Accessors.Count);
        var changed = false;
        foreach (var accessor in accessorList.Accessors)
        {
            var updated = RemoveRedundantAccessorModifier(accessor);
            changed |= !ReferenceEquals(updated, accessor);
            accessors.Add(updated);
        }

        return changed ? accessorList.WithAccessors(SyntaxFactory.List(accessors)) : accessorList;
    }

    private static AccessorDeclarationSyntax RemoveRedundantAccessorModifier(AccessorDeclarationSyntax accessor)
    {
        var level = AccessLevelHelper.GetAccessLevel(accessor.Modifiers);
        if (level is not (AccessLevel.Internal or AccessLevel.Public))
        {
            return accessor;
        }

        SyntaxToken? removed = null;
        var remaining = new List<SyntaxToken>(accessor.Modifiers.Count);
        foreach (var modifier in accessor.Modifiers)
        {
            if (removed is null
                && ((level == AccessLevel.Internal && modifier.IsKind(SyntaxKind.InternalKeyword))
                    || (level == AccessLevel.Public && modifier.IsKind(SyntaxKind.PublicKeyword))))
            {
                removed = modifier;
            }
            else
            {
                remaining.Add(modifier);
            }
        }

        if (removed is null)
        {
            return accessor;
        }

        var keyword = accessor.Keyword;
        var leading = removed.Value.LeadingTrivia.AddRange(keyword.LeadingTrivia);
        var trailing = keyword.TrailingTrivia;
        if (TriviaHasCommentOrDirective(removed.Value.TrailingTrivia))
        {
            trailing = removed.Value.TrailingTrivia.AddRange(trailing);
        }

        keyword = keyword.WithLeadingTrivia(leading).WithTrailingTrivia(trailing);
        return accessor.WithModifiers(SyntaxFactory.TokenList(remaining)).WithKeyword(keyword);
    }

    private static bool TriviaHasCommentOrDirective(SyntaxTriviaList trivia)
    {
        foreach (var trivium in trivia)
        {
            if (trivium.IsDirective
                || trivium.IsKind(SyntaxKind.SingleLineCommentTrivia)
                || trivium.IsKind(SyntaxKind.MultiLineCommentTrivia))
            {
                return true;
            }
        }

        return false;
    }

    private static bool AccessorChangeIsSafe(MemberDeclarationSyntax member)
    {
        AccessorListSyntax? accessorList = member switch
        {
            BasePropertyDeclarationSyntax property => property.AccessorList,
            _ => null,
        };

        if (accessorList is null)
        {
            return true;
        }

        foreach (var accessor in accessorList.Accessors)
        {
            var level = AccessLevelHelper.GetAccessLevel(accessor.Modifiers);
            if (level is AccessLevel.Protected or AccessLevel.ProtectedInternal)
            {
                return false;
            }
        }

        return true;
    }

    private static ISymbol? GetDeclaredSymbol(SemanticModel? semanticModel, MemberDeclarationSyntax member, CancellationToken cancellationToken)
    {
        if (semanticModel is null)
        {
            return null;
        }

        var symbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);
        if (symbol is null
            && member is BaseFieldDeclarationSyntax field
            && field.Declaration.Variables.Count > 0)
        {
            symbol = semanticModel.GetDeclaredSymbol(field.Declaration.Variables[0], cancellationToken);
        }

        return symbol;
    }

    private static void AddOtherPartial(ISymbol symbol, List<ISymbol> symbols)
    {
        switch (symbol)
        {
        case IMethodSymbol method:
            AddIfDistinct(symbols, method.PartialDefinitionPart);
            AddIfDistinct(symbols, method.PartialImplementationPart);
            break;

        case IPropertySymbol property:
            AddIfDistinct(symbols, property.PartialDefinitionPart());
            AddIfDistinct(symbols, property.PartialImplementationPart());
            break;

        case IEventSymbol ev:
            AddIfDistinct(symbols, ev.PartialDefinitionPart());
            AddIfDistinct(symbols, ev.PartialImplementationPart());
            break;
        }
    }

    private static void AddIfDistinct(List<ISymbol> symbols, ISymbol? other)
    {
        if (other is null)
        {
            return;
        }

        foreach (var existing in symbols)
        {
            if (existing.Equals(other))
            {
                return;
            }
        }

        symbols.Add(other);
    }

    private static MemberDeclarationSyntax? GetMemberDeclaration(SyntaxNode node)
    {
        return node switch
        {
            MemberDeclarationSyntax member => member,
            VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Parent: MemberDeclarationSyntax member } } => member,
            _ => null,
        };
    }

    private static bool IsGenerated(Solution solution, SyntaxNode node, CancellationToken cancellationToken)
    {
        var document = solution.GetDocument(node.SyntaxTree);
        if (document is null)
        {
            return true;
        }

        return node.SyntaxTree.IsGeneratedDocument(document.Project.AnalyzerOptions, cancellationToken);
    }

    private static bool IsOverridable(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol method => (method.IsVirtual || method.IsAbstract) && !method.IsOverride,
            IPropertySymbol property => (property.IsVirtual || property.IsAbstract) && !property.IsOverride,
            IEventSymbol ev => (ev.IsVirtual || ev.IsAbstract) && !ev.IsOverride,
            _ => false,
        };
    }

    private static bool HasPartialModifier(MemberDeclarationSyntax member)
    {
        foreach (var modifier in member.Modifiers())
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasPartialOrVirtualModifier(MemberDeclarationSyntax member)
    {
        foreach (var modifier in member.Modifiers())
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword)
                || modifier.IsKind(SyntaxKind.VirtualKeyword)
                || modifier.IsKind(SyntaxKind.AbstractKeyword))
            {
                return true;
            }
        }

        return false;
    }

    private static int UniqueDeclarationCount(List<MemberDeclarationSyntax> declarations)
    {
        var seen = new HashSet<NodeKey>();
        foreach (var declaration in declarations)
        {
            seen.Add(new NodeKey(declaration));
        }

        return seen.Count;
    }

    private static void AddTarget(List<MemberDeclarationSyntax> targets, HashSet<NodeKey> seen, MemberDeclarationSyntax declaration)
    {
        if (seen.Add(new NodeKey(declaration)))
        {
            targets.Add(declaration);
        }
    }

    private readonly struct NodeKey : IEquatable<NodeKey>
    {
        private readonly SyntaxTree tree;
        private readonly int start;
        private readonly int length;

        public NodeKey(SyntaxNode node)
        {
            this.tree = node.SyntaxTree;
            this.start = node.Span.Start;
            this.length = node.Span.Length;
        }

        public bool Equals(NodeKey other)
        {
            return this.tree == other.tree && this.start == other.start && this.length == other.length;
        }

        public override bool Equals(object? obj)
        {
            return obj is NodeKey other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = this.tree is null ? 0 : this.tree.GetHashCode();
                hash = (hash * 397) ^ this.start;
                hash = (hash * 397) ^ this.length;
                return hash;
            }
        }
    }
}
