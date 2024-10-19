﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using static ModifierOrderHelper;

    /// <summary>
    /// The keywords within the declaration of an element do not follow a standard ordering scheme.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the keywords within an element’s declaration do not follow a standard
    /// ordering scheme.</para>
    ///
    /// <para>Within an element declaration, keywords should appear in the following order:</para>
    ///
    /// <list type="bullet">
    /// <item><description>Access modifiers</description></item>
    /// <item><description><see langword="static"/></description></item>
    /// <item><description>All other keywords</description></item>
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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1206.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1206Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1206MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1206Description), OrderingResources.ResourceManager, typeof(OrderingResources));

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
            SyntaxToken previousModifier = default;
            SyntaxToken previousOtherModifier = default;

            foreach (var modifier in modifiers)
            {
                var currentModifierType = GetModifierType(modifier);

                bool reportPreviousModifier = false;
                bool reportPreviousOtherModifier = false;
                if (CompareModifiersType(currentModifierType, previousModifierType) < 0)
                {
                    reportPreviousModifier = true;
                }

                if (AccessOrStaticModifierNotFollowingOtherModifier(currentModifierType, previousModifierType) && otherModifiersAppearEarlier)
                {
                    reportPreviousOtherModifier = true;
                }

                if (reportPreviousModifier || reportPreviousOtherModifier)
                {
                    // Note: Only report one diagnostic per modifier. If both diagnostics apply, report the diagnostic
                    // relative to the earlier modifier.
                    var reportedModifier = reportPreviousModifier && (!reportPreviousOtherModifier || previousModifier.SpanStart < previousOtherModifier.SpanStart)
                        ? previousModifier.ValueText
                        : previousOtherModifier.ValueText;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, modifier.GetLocation(), modifier.ValueText, reportedModifier));
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

        private static bool AccessOrStaticModifierNotFollowingOtherModifier(ModifierType current, ModifierType previous) => (current == ModifierType.Access || current == ModifierType.Static) && previous != ModifierType.Other;
    }
}
