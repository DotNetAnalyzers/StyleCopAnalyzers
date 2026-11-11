// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System;
using System.Collections.Immutable;
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

    private static readonly Action<SyntaxNodeAnalysisContext> MemberDeclarationAction = HandleMemberDeclaration;

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(MemberDeclarationAction, MemberDeclarationKinds);
    }

    private static void HandleMemberDeclaration(SyntaxNodeAnalysisContext context)
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

        if (IsRequiredToBePublic(member, context.SemanticModel, context.CancellationToken))
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

    private static bool IsRequiredToBePublic(MemberDeclarationSyntax member, SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken)
    {
        switch (member)
        {
        case EventFieldDeclarationSyntax eventField:
            foreach (var variable in eventField.Declaration.Variables)
            {
                if (IsInterfaceImplementationOrOverride(semanticModel.GetDeclaredSymbol(variable, cancellationToken)))
                {
                    return true;
                }
            }

            return false;

        case FieldDeclarationSyntax:
        case DelegateDeclarationSyntax:
        case BaseTypeDeclarationSyntax:
            return false;

        default:
            return IsInterfaceImplementationOrOverride(semanticModel.GetDeclaredSymbol(member, cancellationToken));
        }
    }

    private static bool IsInterfaceImplementationOrOverride(ISymbol? symbol)
    {
        if (symbol is null || symbol.IsOverride)
        {
            return symbol?.IsOverride == true;
        }

        if (HasExplicitInterfaceImplementations(symbol))
        {
            return true;
        }

        var containingType = symbol.ContainingType;
        if (containingType is null || containingType.AllInterfaces.IsDefaultOrEmpty)
        {
            return false;
        }

        foreach (var iface in containingType.AllInterfaces)
        {
            foreach (var member in iface.GetMembers(symbol.Name))
            {
                var implementation = containingType.FindImplementationForInterfaceMember(member);
                if (implementation is not null && implementation.Equals(symbol))
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
}
