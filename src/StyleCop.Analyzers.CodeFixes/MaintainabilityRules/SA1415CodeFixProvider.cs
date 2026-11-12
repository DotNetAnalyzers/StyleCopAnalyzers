// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Text;
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

            var symbols = GetDeclaredSymbols(semanticModel, member, context.CancellationToken);
            if (!await CanOfferFixAsync(context.Document.Project.Solution, member, symbols, context.CancellationToken).ConfigureAwait(false))
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
        var symbols = GetDeclaredSymbols(semanticModel, member, cancellationToken);
        var plan = await TryCollectTargetsAsync(solution, member, symbols, cancellationToken).ConfigureAwait(false);
        if (plan is null)
        {
            return solution;
        }

        return await RewriteTargetsAsync(solution, plan, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<bool> CanOfferFixAsync(
        Solution solution,
        MemberDeclarationSyntax member,
        IReadOnlyList<ISymbol> symbols,
        CancellationToken cancellationToken)
    {
        var plan = await TryCollectTargetsAsync(solution, member, symbols, cancellationToken).ConfigureAwait(false);
        return plan is not null;
    }

    private static async Task<FixPlan?> TryCollectTargetsAsync(
        Solution solution,
        MemberDeclarationSyntax member,
        IReadOnlyList<ISymbol> symbols,
        CancellationToken cancellationToken)
    {
        var targets = new List<MemberDeclarationSyntax>();
        var seenNodes = new HashSet<NodeKey>();
        var seenSymbols = new HashSet<ISymbol>();
        var pending = new Queue<ISymbol>();
        foreach (var symbol in symbols)
        {
            pending.Enqueue(symbol);
        }

        if (pending.Count == 0)
        {
            if (!TryAddDeclarations(solution, member, symbol: null, targets, seenNodes, cancellationToken))
            {
                return null;
            }

            return targets.Count > 0
                ? await TryCreatePlanAsync(solution, targets, new SA1415DoNotUsePublicOnMembersOfInternalTypes.RequirementContext(), cancellationToken).ConfigureAwait(false)
                : null;
        }

        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var symbol = pending.Dequeue();
            if (!seenSymbols.Add(NormalizePartial(symbol)))
            {
                continue;
            }

            var declarations = new List<MemberDeclarationSyntax>();
            var fallback = IsSeedSymbol(symbols, symbol) ? member : null;
            if (!TryGetDeclarations(solution, fallback, symbol, declarations, cancellationToken))
            {
                return null;
            }

            foreach (var declaration in declarations)
            {
                if (!AccessorChangeIsSafe(declaration))
                {
                    return null;
                }

                AddTarget(targets, seenNodes, declaration);
                if (!await TryEnqueueDeclaredSymbolsAsync(solution, declaration, pending, cancellationToken).ConfigureAwait(false))
                {
                    return null;
                }
            }

            if (IsOverridable(symbol))
            {
                if (!await TryEnqueueOverridesAsync(solution, symbol, pending, cancellationToken).ConfigureAwait(false))
                {
                    return null;
                }
            }

            var overridden = GetOverridden(symbol);
            if (overridden is not null)
            {
                var source = await ResolveSourceSymbolAsync(solution, overridden, cancellationToken).ConfigureAwait(false);
                if (source is null)
                {
                    if (symbol.IsOverride)
                    {
                        return null;
                    }
                }
                else
                {
                    pending.Enqueue(source);
                }
            }
        }

        return targets.Count > 0
            ? await TryCreatePlanAsync(solution, targets, new SA1415DoNotUsePublicOnMembersOfInternalTypes.RequirementContext(), cancellationToken).ConfigureAwait(false)
            : null;
    }

    private static async Task<FixPlan?> TryCreatePlanAsync(
        Solution solution,
        List<MemberDeclarationSyntax> targets,
        SA1415DoNotUsePublicOnMembersOfInternalTypes.RequirementContext requirements,
        CancellationToken cancellationToken)
    {
        var plan = new FixPlan();
        foreach (var declaration in targets)
        {
            var symbols = await GetDeclarationSymbolsAsync(solution, declaration, cancellationToken).ConfigureAwait(false);
            var keep = new List<string>();
            var change = 0;
            if (symbols.Count == 0)
            {
                plan.Declarations.Add(declaration);
                continue;
            }

            foreach (var symbol in symbols)
            {
                var compilation = await GetCompilationAsync(solution, symbol, cancellationToken).ConfigureAwait(false);
                if (compilation is not null
                    && requirements.MustRemainPublic(compilation, symbol, cancellationToken))
                {
                    keep.Add(symbol.Name);
                }
                else
                {
                    change++;
                }
            }

            if (change == 0)
            {
                continue;
            }

            if (keep.Count > 0 && (declaration is not BaseFieldDeclarationSyntax field || !CanSplitField(declaration) || FieldSplitHasDirective(field)))
            {
                return null;
            }

            plan.Declarations.Add(declaration);
            if (keep.Count > 0)
            {
                plan.KeepPublic.Add(declaration, keep);
            }
        }

        return plan.Declarations.Count > 0 ? plan : null;
    }

    private static async Task<List<ISymbol>> GetDeclarationSymbolsAsync(
        Solution solution,
        MemberDeclarationSyntax declaration,
        CancellationToken cancellationToken)
    {
        var document = solution.GetDocument(declaration.SyntaxTree);
        if (document is null)
        {
            return new List<ISymbol>();
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        return GetDeclaredSymbols(semanticModel, declaration, cancellationToken);
    }

    private static async Task<Compilation?> GetCompilationAsync(Solution solution, ISymbol symbol, CancellationToken cancellationToken)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var document = solution.GetDocument(reference.SyntaxTree);
            if (document is null)
            {
                continue;
            }

            return await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
        }

        return null;
    }

    private static bool IsSeedSymbol(IReadOnlyList<ISymbol> symbols, ISymbol symbol)
    {
        var normalized = NormalizePartial(symbol);
        foreach (var candidate in symbols)
        {
            if (candidate.Equals(symbol) || NormalizePartial(candidate).Equals(normalized))
            {
                return true;
            }
        }

        return false;
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

        if (sawPartial && RequiresPartialPair(symbol) && UniqueDeclarationCount(declarations) < 2)
        {
            return false;
        }

        return true;
    }

    private static bool RequiresPartialPair(ISymbol? symbol)
    {
        return symbol is IMethodSymbol or IPropertySymbol or IEventSymbol;
    }

    private static async Task<bool> TryEnqueueDeclaredSymbolsAsync(
        Solution solution,
        MemberDeclarationSyntax declaration,
        Queue<ISymbol> pending,
        CancellationToken cancellationToken)
    {
        if (declaration is not BaseFieldDeclarationSyntax field || field.Declaration.Variables.Count < 2)
        {
            return true;
        }

        var document = solution.GetDocument(declaration.SyntaxTree);
        if (document is null)
        {
            return false;
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel is null)
        {
            return false;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            var symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);
            if (symbol is not null)
            {
                pending.Enqueue(symbol);
            }
        }

        return true;
    }

    private static async Task<bool> TryEnqueueOverridesAsync(
        Solution solution,
        ISymbol symbol,
        Queue<ISymbol> pending,
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
                pending.Enqueue(overrideSymbol);
            }
        }

        return true;
    }

    private static async Task<ISymbol?> ResolveSourceSymbolAsync(Solution solution, ISymbol symbol, CancellationToken cancellationToken)
    {
        if (symbol.DeclaringSyntaxReferences.Length > 0)
        {
            return symbol;
        }

        var metadataName = MetadataName(symbol.ContainingType);
        if (metadataName is null)
        {
            return null;
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
                return null;
            }

            var type = compilation.GetTypeByMetadataName(metadataName);
            if (type is null)
            {
                continue;
            }

            foreach (var member in type.GetMembers(symbol.Name))
            {
                if (member.Kind == symbol.Kind && member.DeclaringSyntaxReferences.Length > 0)
                {
                    return member;
                }
            }
        }

        return null;
    }

    private static string? MetadataName(INamedTypeSymbol? type)
    {
        if (type is null)
        {
            return null;
        }

        var builder = new StringBuilder();
        AppendNamespace(builder, type.ContainingNamespace);
        if (builder.Length > 0)
        {
            builder.Append('.');
        }

        AppendContainingTypes(builder, type);
        return builder.ToString();
    }

    private static void AppendNamespace(StringBuilder builder, INamespaceSymbol? namespaceSymbol)
    {
        if (namespaceSymbol is null || namespaceSymbol.IsGlobalNamespace)
        {
            return;
        }

        AppendNamespace(builder, namespaceSymbol.ContainingNamespace);
        if (builder.Length > 0)
        {
            builder.Append('.');
        }

        builder.Append(namespaceSymbol.Name);
    }

    private static void AppendContainingTypes(StringBuilder builder, INamedTypeSymbol type)
    {
        if (type.ContainingType is not null)
        {
            AppendContainingTypes(builder, type.ContainingType);
            builder.Append('+');
        }

        builder.Append(type.MetadataName);
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
        FixPlan plan,
        CancellationToken cancellationToken)
    {
        var byTree = new Dictionary<SyntaxTree, List<MemberDeclarationSyntax>>();
        foreach (var target in plan.Declarations)
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

            var keepPublic = new Dictionary<MemberDeclarationSyntax, List<string>>();
            foreach (var declaration in pair.Value)
            {
                if (plan.KeepPublic.TryGetValue(declaration, out var names))
                {
                    keepPublic.Add(declaration, names);
                }
            }

            var newRoot = new DeclarationRewriter(new HashSet<MemberDeclarationSyntax>(pair.Value), keepPublic).Visit(targetRoot);
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

    private static List<ISymbol> GetDeclaredSymbols(SemanticModel? semanticModel, MemberDeclarationSyntax member, CancellationToken cancellationToken)
    {
        var symbols = new List<ISymbol>();
        if (semanticModel is null)
        {
            return symbols;
        }

        if (member is BaseFieldDeclarationSyntax field)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                var variableSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);
                if (variableSymbol is not null)
                {
                    symbols.Add(variableSymbol);
                }
            }
        }

        if (symbols.Count == 0)
        {
            var symbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);
            if (symbol is not null)
            {
                symbols.Add(symbol);
            }
        }

        return symbols;
    }

    private static ISymbol NormalizePartial(ISymbol symbol)
    {
        switch (symbol)
        {
        case IMethodSymbol method when method.PartialDefinitionPart is not null:
            return method.PartialDefinitionPart;

        case IPropertySymbol property when property.PartialDefinitionPart() is IPropertySymbol definition:
            return definition;

        case IEventSymbol ev when ev.PartialDefinitionPart() is IEventSymbol definition:
            return definition;

        default:
            return symbol;
        }
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

    private static bool CanSplitField(MemberDeclarationSyntax declaration)
    {
        return declaration is BaseFieldDeclarationSyntax field
            && field.Parent is TypeDeclarationSyntax type
            && CanReplaceMembers(type);
    }

    private static bool CanReplaceMembers(TypeDeclarationSyntax type)
    {
        return type is ClassDeclarationSyntax or StructDeclarationSyntax or InterfaceDeclarationSyntax
            || RecordDeclarationSyntaxWrapper.IsInstance(type)
            || ExtensionBlockDeclarationSyntaxWrapper.IsInstance(type)
            || UnionDeclarationSyntaxWrapper.IsInstance(type);
    }

    private static TypeDeclarationSyntax ReplaceMembers(TypeDeclarationSyntax type, SyntaxList<MemberDeclarationSyntax> members)
    {
        return type switch
        {
            ClassDeclarationSyntax classDeclaration => classDeclaration.WithMembers(members),
            StructDeclarationSyntax structDeclaration => structDeclaration.WithMembers(members),
            InterfaceDeclarationSyntax interfaceDeclaration => interfaceDeclaration.WithMembers(members),
            _ when RecordDeclarationSyntaxWrapper.IsInstance(type) => ((RecordDeclarationSyntaxWrapper)type).WithMembers(members).SyntaxNode!,
            _ when ExtensionBlockDeclarationSyntaxWrapper.IsInstance(type) => ((ExtensionBlockDeclarationSyntaxWrapper)type).WithMembers(members).SyntaxNode!,
            _ when UnionDeclarationSyntaxWrapper.IsInstance(type) => ((UnionDeclarationSyntaxWrapper)type).WithMembers(members).SyntaxNode!,
            _ => type,
        };
    }

    private static bool FieldSplitHasDirective(BaseFieldDeclarationSyntax field)
    {
        if (TriviaHasDirective(field.SemicolonToken.LeadingTrivia) || TriviaHasDirective(field.SemicolonToken.TrailingTrivia))
        {
            return true;
        }

        var variables = field.Declaration.Variables;
        foreach (var variable in variables)
        {
            foreach (var trivia in variable.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsDirective)
                {
                    return true;
                }
            }
        }

        for (var i = 0; i < variables.SeparatorCount; i++)
        {
            var separator = variables.GetSeparator(i);
            if (TriviaHasDirective(separator.LeadingTrivia) || TriviaHasDirective(separator.TrailingTrivia))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TriviaHasDirective(SyntaxTriviaList trivia)
    {
        foreach (var trivium in trivia)
        {
            if (trivium.IsDirective)
            {
                return true;
            }
        }

        return false;
    }

    private static List<MemberDeclarationSyntax> SplitField(BaseFieldDeclarationSyntax field, List<string> keepNames)
    {
        var keep = new HashSet<string>(keepNames);
        var groups = new List<FieldGroup>();
        var variables = field.Declaration.Variables;
        for (var i = 0; i < variables.Count; i++)
        {
            var kept = keep.Contains(variables[i].Identifier.ValueText);
            if (groups.Count == 0 || groups[groups.Count - 1].Keep != kept)
            {
                if (groups.Count > 0)
                {
                    AttachBoundaryTrivia(groups[groups.Count - 1], variables.GetSeparator(i - 1));
                }

                groups.Add(new FieldGroup(kept));
            }

            var group = groups[groups.Count - 1];
            if (group.Variables.Count > 0)
            {
                group.Separators.Add(variables.GetSeparator(i - 1));
            }

            group.Variables.Add(variables[i]);
        }

        if (groups.Count <= 1)
        {
            var only = groups.Count == 1 && groups[0].Keep ? field : (BaseFieldDeclarationSyntax)RewriteMember(field);
            return new List<MemberDeclarationSyntax> { only };
        }

        var leading = field.GetLeadingTrivia();
        var trailing = field.GetTrailingTrivia();
        var parts = new List<MemberDeclarationSyntax>(groups.Count);
        for (var i = 0; i < groups.Count; i++)
        {
            var part = WithVariables(field, groups[i].Variables, groups[i].Separators);
            if (!groups[i].Keep)
            {
                part = (BaseFieldDeclarationSyntax)RewriteMember(part);
            }

            part = i == 0
                ? part.WithLeadingTrivia(leading)
                : part.WithLeadingTrivia(Indentation(leading));
            part = i == groups.Count - 1
                ? part.WithTrailingTrivia(trailing)
                : WithoutSemicolonLeadingTrivia(part).WithTrailingTrivia(EndOfLine(field));
            parts.Add(part);
        }

        return parts;
    }

    private static void AttachBoundaryTrivia(FieldGroup group, SyntaxToken separator)
    {
        var extra = NonWhitespaceTrivia(separator);
        if (extra.Count == 0 || group.Variables.Count == 0)
        {
            return;
        }

        var index = group.Variables.Count - 1;
        var variable = group.Variables[index];
        group.Variables[index] = variable.WithTrailingTrivia(variable.GetTrailingTrivia().AddRange(extra));
    }

    private static SyntaxTriviaList NonWhitespaceTrivia(SyntaxToken token)
    {
        var trivia = new List<SyntaxTrivia>();
        AddNonWhitespace(token.LeadingTrivia, trivia);
        AddNonWhitespace(token.TrailingTrivia, trivia);
        return SyntaxFactory.TriviaList(trivia);
    }

    private static void AddNonWhitespace(SyntaxTriviaList source, List<SyntaxTrivia> trivia)
    {
        foreach (var trivium in source)
        {
            if (trivium.IsKind(SyntaxKind.WhitespaceTrivia) || trivium.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                continue;
            }

            trivia.Add(trivium);
        }
    }

    private static BaseFieldDeclarationSyntax WithoutSemicolonLeadingTrivia(BaseFieldDeclarationSyntax field)
    {
        if (field.SemicolonToken.LeadingTrivia.Count == 0)
        {
            return field;
        }

        var semicolon = field.SemicolonToken.WithLeadingTrivia(SyntaxFactory.TriviaList());
        return field switch
        {
            EventFieldDeclarationSyntax eventField => eventField.WithSemicolonToken(semicolon),
            FieldDeclarationSyntax variableField => variableField.WithSemicolonToken(semicolon),
            _ => field,
        };
    }

    private static BaseFieldDeclarationSyntax WithVariables(
        BaseFieldDeclarationSyntax field,
        List<VariableDeclaratorSyntax> variables,
        List<SyntaxToken> separators)
    {
        var separated = SyntaxFactory.SeparatedList(variables, separators);
        var declaration = field.Declaration.WithVariables(separated);
        return field switch
        {
            EventFieldDeclarationSyntax eventField => eventField.WithDeclaration(declaration),
            FieldDeclarationSyntax variableField => variableField.WithDeclaration(declaration),
            _ => field,
        };
    }

    private static SyntaxTriviaList Indentation(SyntaxTriviaList leading)
    {
        var text = leading.ToFullString();
        var index = text.LastIndexOf('\n');
        var indent = index < 0 ? text : text.Substring(index + 1);
        if (indent.EndsWith("\r", StringComparison.Ordinal))
        {
            indent = indent.Substring(0, indent.Length - 1);
        }

        return indent.Length == 0
            ? SyntaxFactory.TriviaList()
            : SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(indent));
    }

    private static SyntaxTriviaList EndOfLine(SyntaxNode node)
    {
        var text = node.SyntaxTree.GetText().ToString();
        var eol = text.IndexOf("\r\n", StringComparison.Ordinal) >= 0 ? "\r\n" : "\n";
        return SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine(eol));
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

    private sealed class FieldGroup
    {
        public FieldGroup(bool keep)
        {
            this.Keep = keep;
        }

        public bool Keep { get; }

        public List<VariableDeclaratorSyntax> Variables { get; } = new();

        public List<SyntaxToken> Separators { get; } = new();
    }

    private sealed class FixPlan
    {
        public List<MemberDeclarationSyntax> Declarations { get; } = new();

        public Dictionary<MemberDeclarationSyntax, List<string>> KeepPublic { get; } = new();
    }

    private sealed class DeclarationRewriter : CSharpSyntaxRewriter
    {
        private readonly HashSet<MemberDeclarationSyntax> rewrites;
        private readonly Dictionary<MemberDeclarationSyntax, List<string>> keepPublic;

        public DeclarationRewriter(HashSet<MemberDeclarationSyntax> rewrites, Dictionary<MemberDeclarationSyntax, List<string>> keepPublic)
        {
            this.rewrites = rewrites;
            this.keepPublic = keepPublic;
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node is TypeDeclarationSyntax type && this.Touches(type))
            {
                return this.RewriteType(type);
            }

            return base.Visit(node)!;
        }

        private bool Touches(TypeDeclarationSyntax type)
        {
            foreach (var member in type.Members)
            {
                if (this.rewrites.Contains(member))
                {
                    return true;
                }
            }

            return false;
        }

        private TypeDeclarationSyntax RewriteType(TypeDeclarationSyntax type)
        {
            var members = new List<MemberDeclarationSyntax>(type.Members.Count);
            foreach (var member in type.Members)
            {
                if (!this.rewrites.Contains(member))
                {
                    members.Add((MemberDeclarationSyntax)this.Visit(member)!);
                    continue;
                }

                if (this.keepPublic.TryGetValue(member, out var keep) && member is BaseFieldDeclarationSyntax field)
                {
                    foreach (var part in SplitField(field, keep))
                    {
                        members.Add((MemberDeclarationSyntax)this.Visit(part)!);
                    }
                }
                else
                {
                    members.Add((MemberDeclarationSyntax)this.Visit(RewriteMember(member))!);
                }
            }

            return ReplaceMembers(type, SyntaxFactory.List(members));
        }
    }
}
