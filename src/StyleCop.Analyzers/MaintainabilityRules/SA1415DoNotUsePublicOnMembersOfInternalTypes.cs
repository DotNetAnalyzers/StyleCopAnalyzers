// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// Members of effectively internal or private types should not be declared public.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class SA1415DoNotUsePublicOnMembersOfInternalTypes : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by this analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1415";

    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1415Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1415MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1415Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

    private static readonly DiagnosticDescriptor Descriptor =
        new(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

    private static readonly ImmutableArray<SyntaxKind> MemberDeclarationKinds =
        SyntaxKinds.BaseTypeDeclaration
            .AddRange(SyntaxKinds.BaseFieldDeclaration)
            .AddRange(SyntaxKinds.BaseMethodDeclaration)
            .AddRange(SyntaxKinds.BasePropertyDeclaration)
            .Add(SyntaxKind.DelegateDeclaration)
            .Remove(SyntaxKind.OperatorDeclaration)
            .Remove(SyntaxKind.ConversionOperatorDeclaration)
            .Remove(SyntaxKind.DestructorDeclaration);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(CompilationStartAction);
    }

    private static void CompilationStartAction(CompilationStartAnalysisContext context)
    {
        var lookup = new InheritedImplementationLookup(context.Compilation);
        context.RegisterSyntaxNodeAction(
            nodeContext => HandleMemberDeclaration(nodeContext, lookup),
            MemberDeclarationKinds);
    }

    private static void HandleMemberDeclaration(SyntaxNodeAnalysisContext context, InheritedImplementationLookup lookup)
    {
        var member = (MemberDeclarationSyntax)context.Node;

        if (AccessLevelHelper.GetAccessLevel(member.Modifiers()) != AccessLevel.Public)
        {
            return;
        }

        if (member.Parent is not BaseTypeDeclarationSyntax containingType)
        {
            return;
        }

        if (containingType.IsKind(SyntaxKind.InterfaceDeclaration) || containingType.IsKind(SyntaxKind.EnumDeclaration))
        {
            return;
        }

        if (!IsEffectivelyNotExternallyVisible(containingType.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken)))
        {
            return;
        }

        if (IsRequiredToBePublic(member, context.SemanticModel, lookup, context.CancellationToken))
        {
            return;
        }

        var publicToken = GetPublicKeyword(member.Modifiers());
        if (publicToken.IsKind(SyntaxKind.PublicKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, publicToken.GetLocation()));
        }
    }

    private static bool IsEffectivelyNotExternallyVisible(Accessibility accessibility)
    {
        return accessibility is Accessibility.Internal
            or Accessibility.Private
            or Accessibility.ProtectedAndInternal;
    }

    private static SyntaxToken GetPublicKeyword(SyntaxTokenList modifiers)
    {
        foreach (var modifier in modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PublicKeyword))
            {
                return modifier;
            }
        }

        return default;
    }

    private static bool IsRequiredToBePublic(
        MemberDeclarationSyntax member,
        SemanticModel semanticModel,
        InheritedImplementationLookup lookup,
        CancellationToken cancellationToken)
    {
        switch (member)
        {
        case ConstructorDeclarationSyntax constructor:
            if (constructor.ParameterList.Parameters.Count == 0)
            {
                return true;
            }

            if (semanticModel.GetDeclaredSymbol(constructor, cancellationToken) is IMethodSymbol constructorSymbol
                && DerivesFromAttribute(constructorSymbol.ContainingType))
            {
                return true;
            }

            return IsNonSealedRecordCopyConstructor(constructor, semanticModel, cancellationToken);

        case EventFieldDeclarationSyntax eventField:
            foreach (var variable in eventField.Declaration.Variables)
            {
                var eventSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);
                if (IsInterfaceImplementationOrOverride(eventSymbol)
                    || (eventSymbol is not null && lookup.RequiresPublic(DefinitionPart(eventSymbol), cancellationToken)))
                {
                    return true;
                }
            }

            return false;

        case FieldDeclarationSyntax field:
            if (IsAttributeNamedArgumentField(field, semanticModel, cancellationToken))
            {
                return true;
            }

            foreach (var variable in field.Declaration.Variables)
            {
                var fieldSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);
                if (fieldSymbol is not null && IsEnumerationPatternMember(fieldSymbol, lookup, cancellationToken))
                {
                    return true;
                }
            }

            return false;

        case DelegateDeclarationSyntax:
        case BaseTypeDeclarationSyntax:
            return false;

        default:
            var symbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);
            if (symbol is null)
            {
                return false;
            }

            if (member is PropertyDeclarationSyntax property && IsAttributeSettableProperty(property, symbol))
            {
                return true;
            }

            if (IsEnumerationPatternMember(symbol, lookup, cancellationToken)
                || IsInterfaceImplementationOrOverride(symbol))
            {
                return true;
            }

            if (symbol is IMethodSymbol or IPropertySymbol or IEventSymbol)
            {
                return lookup.RequiresPublic(DefinitionPart(symbol), cancellationToken);
            }

            return false;
        }
    }

    private static bool IsNonSealedRecordCopyConstructor(
        ConstructorDeclarationSyntax constructor,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (constructor.Parent is not BaseTypeDeclarationSyntax typeDeclaration
            || !typeDeclaration.IsKind(SyntaxKindEx.RecordDeclaration)
            || constructor.ParameterList.Parameters.Count != 1)
        {
            return false;
        }

        if (semanticModel.GetDeclaredSymbol(constructor, cancellationToken) is not IMethodSymbol symbol
            || symbol.ContainingType is not { IsSealed: false } containingType)
        {
            return false;
        }

        return symbol.Parameters[0].Type?.Equals(containingType) == true;
    }

    private static bool IsAttributeNamedArgumentField(
        FieldDeclarationSyntax field,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (field.Parent is not BaseTypeDeclarationSyntax typeDeclaration)
        {
            return false;
        }

        if (!DerivesFromAttribute(semanticModel.GetDeclaredSymbol(typeDeclaration, cancellationToken)))
        {
            return false;
        }

        foreach (var modifier in field.Modifiers())
        {
            if (modifier.IsKind(SyntaxKind.StaticKeyword)
                || modifier.IsKind(SyntaxKind.ConstKeyword)
                || modifier.IsKind(SyntaxKind.ReadOnlyKeyword))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAttributeSettableProperty(PropertyDeclarationSyntax property, ISymbol symbol)
    {
        if (symbol.IsStatic || property.AccessorList is null || !DerivesFromAttribute(symbol.ContainingType))
        {
            return false;
        }

        foreach (var accessor in property.AccessorList.Accessors)
        {
            if (accessor.Kind() is not (SyntaxKind.SetAccessorDeclaration or SyntaxKindEx.InitAccessorDeclaration))
            {
                continue;
            }

            var level = AccessLevelHelper.GetAccessLevel(accessor.Modifiers);
            if (level is AccessLevel.NotSpecified or AccessLevel.Public)
            {
                return true;
            }
        }

        return false;
    }

    private static bool DerivesFromAttribute(ITypeSymbol? type)
    {
        var seen = new HashSet<ITypeSymbol>();
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (!seen.Add(current))
            {
                return false;
            }

            if (IsSystemAttribute(current))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSystemAttribute(ITypeSymbol type)
    {
        return type.Name == nameof(Attribute)
            && type.ContainingNamespace?.Name == nameof(System)
            && type.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true;
    }

    private static bool IsEnumerationPatternMember(ISymbol symbol, InheritedImplementationLookup lookup, CancellationToken cancellationToken)
    {
        if (symbol.IsStatic || symbol.DeclaredAccessibility != Accessibility.Public)
        {
            return false;
        }

        var containingType = symbol.ContainingType;
        if (containingType is null)
        {
            return false;
        }

        switch (symbol)
        {
        case IMethodSymbol method when IsInstanceGetEnumerator(method, asyncPattern: false):
            return IsEnumeratorType(method.ReturnType, asyncPattern: false);

        case IMethodSymbol method when IsInstanceGetEnumerator(method, asyncPattern: true):
            return IsEnumeratorType(method.ReturnType, asyncPattern: true);

        case IMethodSymbol method when IsMoveNext(method, asyncPattern: false):
            return FindCurrent(containingType) is not null || lookup.IsBoundPatternMember(symbol, cancellationToken);

        case IMethodSymbol method when IsMoveNext(method, asyncPattern: true):
            return FindCurrent(containingType) is not null || lookup.IsBoundPatternMember(symbol, cancellationToken);

        case IPropertySymbol { Name: "Current", IsIndexer: false }:
        case IFieldSymbol { Name: "Current" }:
            return FindMoveNext(containingType, asyncPattern: false) is not null
                || FindMoveNext(containingType, asyncPattern: true) is not null
                || lookup.IsBoundPatternMember(symbol, cancellationToken);

        default:
            return false;
        }
    }

    private static bool IsInstanceGetEnumerator(IMethodSymbol method, bool asyncPattern)
    {
        if (method.IsStatic || method.IsExtensionMethod || !method.TypeParameters.IsEmpty)
        {
            return false;
        }

        if (asyncPattern)
        {
            return method.Name == "GetAsyncEnumerator" && ParametersAreOptional(method);
        }

        return method.Name == "GetEnumerator" && method.Parameters.IsEmpty;
    }

    private static bool IsMoveNext(IMethodSymbol method, bool asyncPattern)
    {
        if (method.IsStatic
            || method.DeclaredAccessibility != Accessibility.Public
            || !method.TypeParameters.IsEmpty
            || !method.Parameters.IsEmpty)
        {
            return false;
        }

        if (asyncPattern)
        {
            return method.Name == "MoveNextAsync" && IsAwaitableBool(method.ReturnType);
        }

        return method.Name == "MoveNext" && method.ReturnType.SpecialType == SpecialType.System_Boolean;
    }

    private static bool ParametersAreOptional(IMethodSymbol method)
    {
        foreach (var parameter in method.Parameters)
        {
            if (!parameter.IsOptional)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsEnumeratorType(ITypeSymbol? type, bool asyncPattern)
    {
        return type is INamedTypeSymbol namedType
            && FindCurrent(namedType) is not null
            && FindMoveNext(namedType, asyncPattern) is not null;
    }

    private static ISymbol? FindCurrent(INamedTypeSymbol type)
    {
        return FindPatternMember(type, current =>
        {
            foreach (var member in current.GetMembers("Current"))
            {
                if (member.IsStatic || member.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }

                if (member is IPropertySymbol { IsIndexer: false } or IFieldSymbol)
                {
                    return member;
                }
            }

            return null;
        });
    }

    private static ISymbol? FindMoveNext(INamedTypeSymbol type, bool asyncPattern)
    {
        var name = asyncPattern ? "MoveNextAsync" : "MoveNext";
        return FindPatternMember(type, current =>
        {
            foreach (var member in current.GetMembers(name))
            {
                if (member is IMethodSymbol method && IsMoveNext(method, asyncPattern))
                {
                    return method;
                }
            }

            return null;
        });
    }

    private static ISymbol? FindPatternMember(INamedTypeSymbol type, Func<INamedTypeSymbol, ISymbol?> match)
    {
        var seen = new HashSet<INamedTypeSymbol>();
        for (var current = type; current is not null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
        {
            if (!seen.Add(current))
            {
                break;
            }

            var found = match(current);
            if (found is not null)
            {
                return found;
            }
        }

        foreach (var iface in type.AllInterfaces)
        {
            if (!seen.Add(iface))
            {
                continue;
            }

            var found = match(iface);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private static bool IsAwaitableBool(ITypeSymbol? type)
    {
        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        IMethodSymbol? getAwaiter = null;
        var seen = new HashSet<INamedTypeSymbol>();
        for (var current = namedType; current is not null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
        {
            if (!seen.Add(current))
            {
                break;
            }

            foreach (var member in current.GetMembers("GetAwaiter"))
            {
                if (member is IMethodSymbol { IsStatic: false, TypeParameters.IsEmpty: true } method && method.Parameters.IsEmpty)
                {
                    getAwaiter = method;
                    break;
                }
            }

            if (getAwaiter is not null)
            {
                break;
            }
        }

        if (getAwaiter?.ReturnType is not INamedTypeSymbol awaiterType)
        {
            return false;
        }

        var hasResult = false;
        var hasCompleted = false;
        for (var current = awaiterType; current is not null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
        {
            foreach (var member in current.GetMembers("GetResult"))
            {
                if (member is IMethodSymbol { IsStatic: false, Parameters.IsEmpty: true } method
                    && method.ReturnType.SpecialType == SpecialType.System_Boolean)
                {
                    hasResult = true;
                }
            }

            foreach (var member in current.GetMembers("IsCompleted"))
            {
                if (member is IPropertySymbol { IsStatic: false, IsIndexer: false } property
                    && property.Type.SpecialType == SpecialType.System_Boolean)
                {
                    hasCompleted = true;
                }
            }

            if (hasResult && hasCompleted)
            {
                return true;
            }
        }

        return false;
    }

    private static ISymbol DefinitionPart(ISymbol symbol)
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

    private static bool IsInterfaceImplementationOrOverride(ISymbol? symbol)
    {
        if (symbol is null || symbol.IsOverride)
        {
            return symbol?.IsOverride == true;
        }

        var definition = DefinitionPart(symbol);
        if (HasExplicitInterfaceImplementations(symbol)
            || (!definition.Equals(symbol) && HasExplicitInterfaceImplementations(definition)))
        {
            return true;
        }

        var containingType = definition.ContainingType;
        if (containingType is null || containingType.AllInterfaces.IsDefaultOrEmpty)
        {
            return false;
        }

        foreach (var iface in containingType.AllInterfaces)
        {
            foreach (var member in iface.GetMembers(definition.Name))
            {
                var implementation = containingType.FindImplementationForInterfaceMember(member);
                if (implementation is not null && DefinitionPart(implementation).Equals(definition))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasExplicitInterfaceImplementations(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol method => !method.ExplicitInterfaceImplementations.IsDefaultOrEmpty,
            IPropertySymbol property => !property.ExplicitInterfaceImplementations.IsDefaultOrEmpty,
            IEventSymbol ev => !ev.ExplicitInterfaceImplementations.IsDefaultOrEmpty,
            _ => false,
        };
    }

    /// <summary>
    /// Records source types that directly implement an interface, keyed by each base type a derived
    /// type can use as an implicit implementation. Built once per compilation, and only when a public
    /// member of an internal type survives the cheaper exemptions.
    /// </summary>
    private sealed class InheritedImplementationLookup
    {
        private readonly Compilation compilation;
        private readonly object gate = new();
        private readonly Dictionary<ISymbol, bool> results = new();
        private readonly Dictionary<ISymbol, bool> patternResults = new();
        private Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>? implementersByBase;
        private HashSet<ISymbol>? boundPatternMembers;

        public InheritedImplementationLookup(Compilation compilation)
        {
            this.compilation = compilation;
        }

        public bool RequiresPublic(ISymbol symbol, CancellationToken cancellationToken)
        {
            lock (this.gate)
            {
                if (this.results.TryGetValue(symbol, out var cached))
                {
                    return cached;
                }
            }

            var required = this.Compute(symbol, cancellationToken);

            lock (this.gate)
            {
                this.results[symbol] = required;
            }

            return required;
        }

        public bool IsBoundPatternMember(ISymbol symbol, CancellationToken cancellationToken)
        {
            var key = DefinitionPart(symbol).OriginalDefinition;
            lock (this.gate)
            {
                if (this.patternResults.TryGetValue(key, out var cached))
                {
                    return cached;
                }
            }

            var required = this.GetBoundPatternMembers(cancellationToken).Contains(key);

            lock (this.gate)
            {
                this.patternResults[key] = required;
            }

            return required;
        }

        private static void CollectType(
            INamedTypeSymbol type,
            Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> map,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!type.Interfaces.IsDefaultOrEmpty)
            {
                var seen = new HashSet<INamedTypeSymbol>();
                for (var baseType = type.BaseType; baseType is not null && baseType.SpecialType != SpecialType.System_Object; baseType = baseType.BaseType)
                {
                    if (!seen.Add(baseType))
                    {
                        break;
                    }

                    var key = baseType.OriginalDefinition;
                    if (!map.TryGetValue(key, out var derivedTypes))
                    {
                        derivedTypes = new List<INamedTypeSymbol>();
                        map.Add(key, derivedTypes);
                    }

                    derivedTypes.Add(type);
                }
            }

            foreach (var nested in type.GetTypeMembers())
            {
                CollectType(nested, map, cancellationToken);
            }
        }

        private static bool ChainContains(ISymbol implementation, ISymbol symbol)
        {
            var target = DefinitionPart(symbol).OriginalDefinition;
            var seen = new HashSet<ISymbol>();
            for (var current = implementation; current is not null; current = GetOverridden(current))
            {
                if (!seen.Add(current))
                {
                    return false;
                }

                var currentDefinition = DefinitionPart(current);
                if (currentDefinition.Equals(DefinitionPart(symbol)) || currentDefinition.OriginalDefinition.Equals(target))
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

        private static void CollectPatternType(INamedTypeSymbol type, HashSet<ISymbol> members, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CollectPatternMethods(type, "GetEnumerator", asyncPattern: false, members);
            CollectPatternMethods(type, "GetAsyncEnumerator", asyncPattern: true, members);

            foreach (var nested in type.GetTypeMembers())
            {
                CollectPatternType(nested, members, cancellationToken);
            }
        }

        private static void CollectPatternMethods(INamedTypeSymbol type, string name, bool asyncPattern, HashSet<ISymbol> members)
        {
            foreach (var member in type.GetMembers(name))
            {
                if (member is not IMethodSymbol method || !IsPatternDiscoveryMethod(method, asyncPattern))
                {
                    continue;
                }

                if (method.ReturnType is not INamedTypeSymbol returnType)
                {
                    continue;
                }

                var current = FindCurrent(returnType);
                var moveNext = FindMoveNext(returnType, asyncPattern);
                if (current is null || moveNext is null)
                {
                    continue;
                }

                members.Add(DefinitionPart(current).OriginalDefinition);
                members.Add(DefinitionPart(moveNext).OriginalDefinition);
            }
        }

        private static bool IsPatternDiscoveryMethod(IMethodSymbol method, bool asyncPattern)
        {
            if (!method.TypeParameters.IsEmpty || (method.IsStatic && !method.IsExtensionMethod))
            {
                return false;
            }

            if (asyncPattern)
            {
                return method.Name == "GetAsyncEnumerator" && ParametersAreOptional(method);
            }

            return method.Name == "GetEnumerator" && method.Parameters.IsEmpty;
        }

        private bool Compute(ISymbol symbol, CancellationToken cancellationToken)
        {
            var containingType = symbol.ContainingType?.OriginalDefinition;
            if (containingType is null)
            {
                return false;
            }

            var implementers = this.GetImplementers(cancellationToken);
            if (!implementers.TryGetValue(containingType, out var derivedTypes))
            {
                return false;
            }

            foreach (var derived in derivedTypes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var iface in derived.AllInterfaces)
                {
                    foreach (var member in iface.GetMembers(symbol.Name))
                    {
                        var implementation = derived.FindImplementationForInterfaceMember(member);
                        if (implementation is not null && ChainContains(implementation, symbol))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> GetImplementers(CancellationToken cancellationToken)
        {
            var existing = this.implementersByBase;
            if (existing is not null)
            {
                return existing;
            }

            lock (this.gate)
            {
                if (this.implementersByBase is null)
                {
                    var map = new Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>();
                    this.CollectImplementers(this.compilation.Assembly.GlobalNamespace, map, cancellationToken);
                    this.implementersByBase = map;
                }

                return this.implementersByBase;
            }
        }

        private HashSet<ISymbol> GetBoundPatternMembers(CancellationToken cancellationToken)
        {
            var existing = this.boundPatternMembers;
            if (existing is not null)
            {
                return existing;
            }

            lock (this.gate)
            {
                if (this.boundPatternMembers is null)
                {
                    var members = new HashSet<ISymbol>();
                    this.CollectPatternMembers(this.compilation.Assembly.GlobalNamespace, members, cancellationToken);
                    this.boundPatternMembers = members;
                }

                return this.boundPatternMembers;
            }
        }

        private void CollectPatternMembers(INamespaceSymbol namespaceSymbol, HashSet<ISymbol> members, CancellationToken cancellationToken)
        {
            foreach (var member in namespaceSymbol.GetMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (member)
                {
                case INamespaceSymbol child:
                    this.CollectPatternMembers(child, members, cancellationToken);
                    break;

                case INamedTypeSymbol type:
                    CollectPatternType(type, members, cancellationToken);
                    break;
                }
            }
        }

        private void CollectImplementers(
            INamespaceSymbol namespaceSymbol,
            Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> map,
            CancellationToken cancellationToken)
        {
            foreach (var member in namespaceSymbol.GetMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (member)
                {
                case INamespaceSymbol child:
                    this.CollectImplementers(child, map, cancellationToken);
                    break;

                case INamedTypeSymbol type:
                    CollectType(type, map, cancellationToken);
                    break;
                }
            }
        }
    }
}
