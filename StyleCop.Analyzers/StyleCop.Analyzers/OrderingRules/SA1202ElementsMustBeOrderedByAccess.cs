// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
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
    /// <para>To comply with this rule, adjacent elements of the same type must be positioned in the following order by
    /// access level:</para>
    ///
    /// <list type="bullet">
    /// <item>public</item>
    /// <item>internal</item>
    /// <item>protected internal</item>
    /// <item>protected</item>
    /// <item>private</item>
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
        private const string Title = "Elements must be ordered by access";
        private const string MessageFormat = "All {0} {1} must come before {2} {1}.";
        private const string Description = "An element within a C# code file is out of order within regard to access level, in relation to other elements in the code.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1202.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.DelegateDeclaration] = "delegates",
            [SyntaxKind.EnumDeclaration] = "enums",
            [SyntaxKind.InterfaceDeclaration] = "interfaces",
            [SyntaxKind.StructDeclaration] = "structs",
            [SyntaxKind.ClassDeclaration] = "classes",
            [SyntaxKind.FieldDeclaration] = "fields",
            [SyntaxKind.ConstructorDeclaration] = "constructors",
            [SyntaxKind.EventDeclaration] = "events",
            [SyntaxKind.PropertyDeclaration] = "properties",
            [SyntaxKind.IndexerDeclaration] = "indexers",
            [SyntaxKind.MethodDeclaration] = "methods",
            [SyntaxKind.ConversionOperatorDeclaration] = "conversions",
            [SyntaxKind.OperatorDeclaration] = "operators"
        };

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(TypeDeclarationAction, TypeDeclarationKinds);
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

            HandleMemberList(context, elementOrder, accessibilityIndex, compilationUnit.Members, AccessLevel.Internal);
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

            HandleMemberList(context, elementOrder, accessibilityIndex, compilationUnit.Members, AccessLevel.Internal);
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

            HandleMemberList(context, elementOrder, accessibilityIndex, typeDeclaration.Members, AccessLevel.Private);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, ImmutableArray<OrderingTrait> elementOrder, int accessibilityIndex, SyntaxList<MemberDeclarationSyntax> members, AccessLevel defaultAccessLevel)
        {
            MemberDeclarationSyntax previousMember = null;
            var previousSyntaxKind = SyntaxKind.None;
            var previousAccessLevel = AccessLevel.NotSpecified;

            foreach (var member in members)
            {
                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;

                // if the SyntaxKind of this member (e.g. SyntaxKind.IncompleteMember) will not
                // be handled, skip early.
                if (!MemberNames.ContainsKey(currentSyntaxKind))
                {
                    continue;
                }

                var modifiers = member.GetModifiers();
                AccessLevel currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers);

                if (previousMember != null && previousAccessLevel != AccessLevel.NotSpecified)
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
                            // Only fields may be marked const
                            bool previousIsConst = previousMember.IsKind(SyntaxKind.FieldDeclaration) && previousMember.GetModifiers().Any(SyntaxKind.ConstKeyword);
                            bool currentIsConst = member.IsKind(SyntaxKind.FieldDeclaration) && modifiers.Any(SyntaxKind.ConstKeyword);
                            if (previousIsConst != currentIsConst)
                            {
                                compareAccessLevel = false;
                            }

                            continue;

                        case OrderingTrait.Readonly:
                            // Only fields may be marked readonly
                            bool previousIsReadonly = previousMember.IsKind(SyntaxKind.FieldDeclaration) && previousMember.GetModifiers().Any(SyntaxKind.ReadOnlyKeyword);
                            bool currentIsReadonly = member.IsKind(SyntaxKind.FieldDeclaration) && modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                            if (previousIsReadonly != currentIsReadonly)
                            {
                                compareAccessLevel = false;
                            }

                            continue;

                        case OrderingTrait.Static:
                            bool previousIsStatic = previousMember.GetModifiers().Any(SyntaxKind.StaticKeyword);
                            bool currentIsStatic = modifiers.Any(SyntaxKind.StaticKeyword);
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
                                MemberNames[currentSyntaxKind],
                                AccessLevelHelper.GetName(previousAccessLevel)));
                    }
                }

                previousMember = member;
                previousSyntaxKind = currentSyntaxKind;
                previousAccessLevel = currentAccessLevel;
            }
        }
    }
}
