// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1204Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1204MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1204Description), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1204.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(TypeDeclarationAction, TypeDeclarationKinds);
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

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int staticIndex = elementOrder.IndexOf(OrderingTrait.Static);
            if (staticIndex < 0)
            {
                return;
            }

            var compilationUnit = (NamespaceDeclarationSyntax)context.Node;

            HandleMemberList(context, elementOrder, staticIndex, compilationUnit.Members);
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

            HandleMemberList(context, elementOrder, staticIndex, typeDeclaration.Members);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, ImmutableArray<OrderingTrait> elementOrder, int staticIndex, SyntaxList<MemberDeclarationSyntax> members)
        {
            var previousSyntaxKind = SyntaxKind.None;
            var previousAccessLevel = AccessLevel.NotSpecified;
            var previousMemberStatic = true;
            var previousMemberConstant = false;
            var previousMemberReadonly = false;

            foreach (var member in members)
            {
                var modifiers = member.GetModifiers();

                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;
                var currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers);
                bool currentMemberConstant = modifiers.Any(SyntaxKind.ConstKeyword);
                bool currentMemberReadonly = currentMemberConstant || modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                bool currentMemberStatic = currentMemberConstant || modifiers.Any(SyntaxKind.StaticKeyword);
                bool compareStatic = true;
                for (int j = 0; compareStatic && j < staticIndex; j++)
                {
                    switch (elementOrder[j])
                    {
                    case OrderingTrait.Accessibility:
                        if (currentAccessLevel != previousAccessLevel)
                        {
                            compareStatic = false;
                        }

                        continue;

                    case OrderingTrait.Readonly:
                        if (currentMemberReadonly != previousMemberReadonly)
                        {
                            compareStatic = false;
                        }

                        continue;

                    case OrderingTrait.Constant:
                        if (currentMemberConstant != previousMemberConstant)
                        {
                            compareStatic = false;
                        }

                        continue;

                    case OrderingTrait.Kind:
                        if (previousSyntaxKind != currentSyntaxKind)
                        {
                            compareStatic = false;
                        }

                        continue;

                    case OrderingTrait.Static:
                    default:
                        continue;
                    }
                }

                if (compareStatic)
                {
                    if (currentMemberStatic && !previousMemberStatic)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptor,
                                NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                                AccessLevelHelper.GetName(currentAccessLevel)));
                    }
                }

                previousSyntaxKind = currentSyntaxKind;
                previousAccessLevel = currentAccessLevel;
                previousMemberStatic = currentMemberStatic;
                previousMemberConstant = currentMemberConstant;
                previousMemberReadonly = currentMemberReadonly;
            }
        }
    }
}
