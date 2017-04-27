﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using static ModifierOrderHelper;

    /// <summary>
    /// The keywords within the declaration of an element do not follow a standard ordering scheme.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the keywords within an element’s declaration do not follow a standard
    /// ordering scheme.</para>
    ///
    /// <para>Within an element declaration, keywords must appear in the following order:</para>
    ///
    /// <list type="bullet">
    /// <item>Access modifiers</item>
    /// <item><see langword="static"/></item>
    /// <item>All other keywords</item>
    /// </list>
    ///
    /// <para>Using a standard ordering scheme for element declaration keywords can make the code more readable by
    /// highlighting the access level of each element. This can help prevent elements from being given a higher access
    /// level than needed.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1206DeclarationKeywordsMustFollowOrder : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1206DeclarationKeywordsMustFollowOrder"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1206";
        private const string Title = "Declaration keywords must follow order";
        private const string MessageFormat = "The '{0}' modifier must appear before '{1}'";
        private const string Description = "The keywords within the declaration of an element do not follow a standard ordering scheme.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1206.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledSyntaxKinds =
            ImmutableArray.Create(
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration,
                SyntaxKind.ConstructorDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext> DeclarationAction = HandleDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(DeclarationAction, HandledSyntaxKinds);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            var modifiers = DeclarationModifiersHelper.GetModifiers(context.Node as MemberDeclarationSyntax);
            CheckModifiersOrderAndReportDiagnostics(context, modifiers);
        }

        private static void CheckModifiersOrderAndReportDiagnostics(SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers)
        {
            var previousModifierType = ModifierType.None;
            var otherModifiersAppearEarlier = false;
            SyntaxToken previousModifier = default(SyntaxToken);
            SyntaxToken previousOtherModifier = default(SyntaxToken);

            foreach (var modifier in modifiers)
            {
                var currentModifierType = GetModifierType(modifier);

                if (CompareModifiersType(currentModifierType, previousModifierType) < 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, modifier.GetLocation(), modifier.ValueText, previousModifier.ValueText));
                }

                if (AccessOrStaticModifierNotFollowingOtherModifier(currentModifierType, previousModifierType) && otherModifiersAppearEarlier)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, modifier.GetLocation(), modifier.ValueText, previousOtherModifier.ValueText));
                }

                if (!otherModifiersAppearEarlier && currentModifierType == ModifierType.Other)
                {
                    otherModifiersAppearEarlier = true;
                    previousOtherModifier = modifier;
                }

                previousModifierType = currentModifierType;
                previousModifier = modifier;
            }
        }

        private static int CompareModifiersType(ModifierType first, ModifierType second)
        {
            const int lessThan = -1;
            const int greaterThan = 1;

            var result = 0;

            if (first == second)
            {
                result = 0;
            }
            else if (first == ModifierType.None)
            {
                result = lessThan;
            }
            else if (second == ModifierType.None)
            {
                result = greaterThan;
            }
            else if (first == ModifierType.Access && (second == ModifierType.Static || second == ModifierType.Other))
            {
                result = lessThan;
            }
            else if (first == ModifierType.Static && second == ModifierType.Other)
            {
                result = lessThan;
            }
            else if (first == ModifierType.Static && second == ModifierType.Access)
            {
                result = greaterThan;
            }
            else if (first == ModifierType.Other && (second == ModifierType.Static || second == ModifierType.Access))
            {
                result = greaterThan;
            }

            return result;
        }

        private static string GetModifierTypeText(ModifierType modifierType)
        {
            var result = string.Empty;

            switch (modifierType)
            {
            case ModifierType.Access:
                result = "access modifier";
                break;

            case ModifierType.Static:
                result = "static";
                break;

            case ModifierType.Other:
                result = "other";
                break;
            }

            return result;
        }

        private static bool AccessOrStaticModifierNotFollowingOtherModifier(ModifierType current, ModifierType previous) => (current == ModifierType.Access || current == ModifierType.Static) && previous != ModifierType.Other;
    }
}
