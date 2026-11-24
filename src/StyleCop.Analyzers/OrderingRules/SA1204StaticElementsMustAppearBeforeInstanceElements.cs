// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A static element is positioned beneath an instance element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a static element is positioned beneath an instance element of the
    /// same type. All static elements should be placed above all instance elements of the same type to make it easier to
    /// see the interface exposed from the instance and static version of the class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1204StaticElementsMustAppearBeforeInstanceElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1204StaticElementsMustAppearBeforeInstanceElements"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1204";
        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1204Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1204MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1204Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKindEx.RecordDeclaration,
                SyntaxKindEx.RecordStructDeclaration,
                SyntaxKindEx.UnionDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseNamespaceDeclarationAction = HandleBaseNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
                context.RegisterSyntaxNodeAction(BaseNamespaceDeclarationAction, SyntaxKinds.BaseNamespaceDeclaration);
                context.RegisterSyntaxNodeAction(TypeDeclarationAction, TypeDeclarationKinds);
                context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKindEx.ExtensionBlockDeclaration);
            });
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int staticIndex = elementOrder.IndexOf(OrderingTrait.Static);
            if (staticIndex < 0)
            {
                return;
            }

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, elementOrder, staticIndex, compilationUnit.Members);
        }

        private static void HandleBaseNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int staticIndex = elementOrder.IndexOf(OrderingTrait.Static);
            if (staticIndex < 0)
            {
                return;
            }

            var baseNamespaceDeclaration = (BaseNamespaceDeclarationSyntaxWrapper)context.Node;

            HandleMemberList(context, elementOrder, staticIndex, baseNamespaceDeclaration.Members);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int staticIndex = elementOrder.IndexOf(OrderingTrait.Static);
            if (staticIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (settings.OrderingRules.MemberKindOrder is { } order
                && elementOrder.IndexOf(OrderingTrait.Kind) is var kindIndex
                && kindIndex >= 0 && kindIndex < staticIndex)
            {
                HandleConfiguredMemberList(context, typeDeclaration.Members, order);
                return;
            }

            HandleMemberList(context, elementOrder, staticIndex, typeDeclaration.Members);
        }

        private static void HandleConfiguredMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members, MemberKindOrder order)
        {
            var previousByKind = new Dictionary<SyntaxKind, int>();
            var precedingPriority = 0;
            foreach (var member in members)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var priority = order.GetPrecedingPriority(member);
                if (MemberKindOrder.StartsNewGroup(member, priority, precedingPriority))
                {
                    previousByKind.Clear();
                }

                precedingPriority = priority;
                var kind = MemberKindOrder.Normalize(member.Kind());
                var modifierPriority = order.GetModifierPriority(member, OrderingTrait.Static, SyntaxKind.StaticKeyword);
                if (previousByKind.TryGetValue(kind, out var previous)
                    && MemberKindOrder.IsModifierOutOfOrder(modifierPriority, previous))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptor,
                        NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                        AccessLevelHelper.GetName(MemberOrderHelper.GetAccessLevelForOrdering(member, member.GetModifiers()))));
                }

                previousByKind[kind] = modifierPriority;
            }
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, ImmutableArray<OrderingTrait> elementOrder, int staticIndex, SyntaxList<MemberDeclarationSyntax> members)
        {
            MemberOrderTraits? previous = null;
            foreach (var member in members)
            {
                var current = new MemberOrderTraits(member);
                if (previous is { } precedingMember
                    && current.IsModifierOutOfOrder(precedingMember, elementOrder, staticIndex, OrderingTrait.Static))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptor,
                            NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                            AccessLevelHelper.GetName(current.AccessLevel)));
                }

                previous = current;
            }
        }
    }
}
