// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// An readonly element is positioned beneath a non-readonly element.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1214";
        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1214Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1214MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1214Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKindEx.RecordDeclaration,
                SyntaxKindEx.RecordStructDeclaration,
                SyntaxKindEx.UnionDeclaration);

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
                context.RegisterSyntaxNodeAction(TypeDeclarationAction, TypeDeclarationKinds);
            });
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int readonlyIndex = elementOrder.IndexOf(OrderingTrait.Readonly);
            if (readonlyIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;
            if (settings.OrderingRules.MemberKindOrder is { } order
                && elementOrder.IndexOf(OrderingTrait.Kind) is var kindIndex
                && kindIndex >= 0 && kindIndex < readonlyIndex)
            {
                HandleConfiguredMemberList(context, typeDeclaration.Members, order);
                return;
            }

            MemberOrderTraits? previous = null;
            foreach (var member in typeDeclaration.Members)
            {
                if (!(member is FieldDeclarationSyntax field))
                {
                    previous = null;
                    continue;
                }

                var current = new MemberOrderTraits(field);
                if (previous is { } precedingField
                    && current.IsModifierOutOfOrder(precedingField, elementOrder, readonlyIndex, OrderingTrait.Readonly))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(member)));
                }

                previous = current;
            }
        }

        private static void HandleConfiguredMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members, MemberKindOrder order)
        {
            var precedingPriority = 0;
            var previousPriority = -1;
            foreach (var member in members)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var priority = order.GetPrecedingPriority(member);
                if (MemberKindOrder.StartsNewGroup(member, priority, precedingPriority))
                {
                    previousPriority = -1;
                }

                precedingPriority = priority;
                if (!(member is FieldDeclarationSyntax field))
                {
                    continue;
                }

                var modifierPriority = order.GetModifierPriority(field, OrderingTrait.Readonly, SyntaxKind.ReadOnlyKeyword);
                if (MemberKindOrder.IsModifierOutOfOrder(modifierPriority, previousPriority))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(member)));
                }

                previousPriority = modifierPriority;
            }
        }
    }
}
