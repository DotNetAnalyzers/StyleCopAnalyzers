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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1214Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1214MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1214Description), OrderingResources.ResourceManager, typeof(OrderingResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1214.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TypeDeclarationAction, TypeDeclarationKinds);
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

            // This variable is null when the previous member is not a field.
            FieldDeclarationSyntax previousField = null;
            var previousFieldConst = true;
            var previousFieldStatic = false;
            var previousFieldReadonly = false;
            var previousAccessLevel = AccessLevel.NotSpecified;
            foreach (var member in typeDeclaration.Members)
            {
                if (!(member is FieldDeclarationSyntax field))
                {
                    previousField = null;
                    continue;
                }

                var modifiers = member.GetModifiers();
                var currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers);
                bool currentFieldConst = modifiers.Any(SyntaxKind.ConstKeyword);
                bool currentFieldStatic = currentFieldConst || modifiers.Any(SyntaxKind.StaticKeyword);
                bool currentFieldReadonly = currentFieldConst || modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                if (previousField == null)
                {
                    previousField = field;
                    previousFieldConst = currentFieldConst;
                    previousFieldStatic = currentFieldStatic;
                    previousFieldReadonly = currentFieldReadonly;
                    previousAccessLevel = currentAccessLevel;
                    continue;
                }

                bool compareReadonly = true;
                for (int j = 0; compareReadonly && j < readonlyIndex; j++)
                {
                    switch (elementOrder[j])
                    {
                    case OrderingTrait.Kind:
                        // This analyzer only ever looks at sequences of fields.
                        continue;

                    case OrderingTrait.Accessibility:
                        if (previousAccessLevel != currentAccessLevel)
                        {
                            compareReadonly = false;
                        }

                        continue;

                    case OrderingTrait.Constant:
                        if (previousFieldConst != currentFieldConst)
                        {
                            compareReadonly = false;
                        }

                        continue;

                    case OrderingTrait.Static:
                        if (previousFieldStatic != currentFieldStatic)
                        {
                            compareReadonly = false;
                        }

                        continue;

                    case OrderingTrait.Readonly:
                    default:
                        continue;
                    }
                }

                if (compareReadonly)
                {
                    if (currentFieldReadonly && !previousFieldReadonly)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(member)));
                    }
                }

                previousField = field;
                previousFieldConst = currentFieldConst;
                previousFieldStatic = currentFieldStatic;
                previousFieldReadonly = currentFieldReadonly;
                previousAccessLevel = currentAccessLevel;
            }
        }
    }
}
