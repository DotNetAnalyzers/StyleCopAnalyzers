// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// An element within a C# code file is out of order within regard to access level, in relation to other elements in
    /// the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme based on access level.</para>
    ///
    /// <para>To comply with this rule, adjacent elements of the same type should be positioned in the following order by
    /// access level:</para>
    ///
    /// <list type="bullet">
    /// <item><description>public</description></item>
    /// <item><description>internal</description></item>
    /// <item><description>protected internal</description></item>
    /// <item><description>protected</description></item>
    /// <item><description>private</description></item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on access level can increase the readability and
    /// maintainability of the file and make it easier to identify the public interface that is being exposed from a
    /// class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1202ElementsMustBeOrderedByAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1202ElementsMustBeOrderedByAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1202";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1202.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1202Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1202MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1202Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly ImmutableHashSet<SyntaxKind> MemberKinds = ImmutableHashSet.Create(
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.OperatorDeclaration);

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
            int accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
            if (accessibilityIndex < 0)
            {
                return;
            }

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, elementOrder, accessibilityIndex, compilationUnit.Members);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
            if (accessibilityIndex < 0)
            {
                return;
            }

            var compilationUnit = (NamespaceDeclarationSyntax)context.Node;

            HandleMemberList(context, elementOrder, accessibilityIndex, compilationUnit.Members);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
            if (accessibilityIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            HandleMemberList(context, elementOrder, accessibilityIndex, typeDeclaration.Members);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, ImmutableArray<OrderingTrait> elementOrder, int accessibilityIndex, SyntaxList<MemberDeclarationSyntax> members)
        {
            var previousSyntaxKind = SyntaxKind.None;
            var previousAccessLevel = AccessLevel.NotSpecified;
            bool previousIsConst = false;
            bool previousIsReadonly = false;
            bool previousIsStatic = false;

            foreach (var member in members)
            {
                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;

                // if the SyntaxKind of this member (e.g. SyntaxKind.IncompleteMember) will not
                // be handled, skip early.
                if (!MemberKinds.Contains(currentSyntaxKind))
                {
                    continue;
                }

                var modifiers = member.GetModifiers();
                AccessLevel currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers);
                bool currentIsConst = modifiers.Any(SyntaxKind.ConstKeyword);
                bool currentIsReadonly = modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                bool currentIsStatic = modifiers.Any(SyntaxKind.StaticKeyword);

                if (previousAccessLevel != AccessLevel.NotSpecified)
                {
                    bool compareAccessLevel = true;
                    for (int j = 0; compareAccessLevel && j < accessibilityIndex; j++)
                    {
                        switch (elementOrder[j])
                        {
                        case OrderingTrait.Kind:
                            if (previousSyntaxKind != currentSyntaxKind)
                            {
                                compareAccessLevel = false;
                            }

                            continue;

                        case OrderingTrait.Constant:
                            if (previousIsConst != currentIsConst)
                            {
                                compareAccessLevel = false;
                            }

                            continue;

                        case OrderingTrait.Readonly:
                            if (previousIsReadonly != currentIsReadonly)
                            {
                                compareAccessLevel = false;
                            }

                            continue;

                        case OrderingTrait.Static:
                            if (previousIsStatic != currentIsStatic)
                            {
                                compareAccessLevel = false;
                            }

                            continue;

                        case OrderingTrait.Accessibility:
                        default:
                            continue;
                        }
                    }

                    if (compareAccessLevel && currentAccessLevel > previousAccessLevel)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptor,
                                NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                                AccessLevelHelper.GetName(currentAccessLevel),
                                AccessLevelHelper.GetName(previousAccessLevel)));
                    }
                }

                previousSyntaxKind = currentSyntaxKind;
                previousAccessLevel = currentAccessLevel;
                previousIsConst = currentIsConst;
                previousIsReadonly = currentIsReadonly;
                previousIsStatic = currentIsStatic;
            }
        }
    }
}
