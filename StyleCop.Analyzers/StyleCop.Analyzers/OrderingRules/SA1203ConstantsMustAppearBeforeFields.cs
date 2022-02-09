// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A constant field is placed beneath a non-constant field.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a constant field is placed beneath a non-constant field. Constants
    /// should be placed above fields to indicate that the two are fundamentally different types of elements with
    /// different considerations for the compiler, different naming requirements, etc.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1203ConstantsMustAppearBeforeFields : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1203ConstantsMustAppearBeforeFields"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1203";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1203.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1203Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1203MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1203Description), OrderingResources.ResourceManager, typeof(OrderingResources));

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
            int constantIndex = elementOrder.IndexOf(OrderingTrait.Constant);
            if (constantIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            var members = typeDeclaration.Members;
            var previousFieldConstant = true;
            var previousFieldStatic = false;
            var previousFieldReadonly = false;
            var previousAccessLevel = AccessLevel.NotSpecified;

            foreach (var member in members)
            {
                if (!(member is FieldDeclarationSyntax field))
                {
                    continue;
                }

                AccessLevel currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(field, field.Modifiers);
                bool currentFieldConstant = field.Modifiers.Any(SyntaxKind.ConstKeyword);
                bool currentFieldReadonly = currentFieldConstant || field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                bool currentFieldStatic = currentFieldConstant || field.Modifiers.Any(SyntaxKind.StaticKeyword);
                bool compareConst = true;
                for (int j = 0; compareConst && j < constantIndex; j++)
                {
                    switch (elementOrder[j])
                    {
                    case OrderingTrait.Accessibility:
                        if (currentAccessLevel != previousAccessLevel)
                        {
                            compareConst = false;
                        }

                        continue;

                    case OrderingTrait.Readonly:
                        if (currentFieldReadonly != previousFieldReadonly)
                        {
                            compareConst = false;
                        }

                        continue;

                    case OrderingTrait.Static:
                        if (currentFieldStatic != previousFieldStatic)
                        {
                            compareConst = false;
                        }

                        continue;

                    case OrderingTrait.Kind:
                        // Only fields may be marked const, and all fields have the same kind.
                        continue;

                    case OrderingTrait.Constant:
                    default:
                        continue;
                    }
                }

                if (compareConst)
                {
                    if (!previousFieldConstant && currentFieldConstant)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(member)));
                    }
                }

                previousFieldConstant = currentFieldConstant;
                previousFieldReadonly = currentFieldReadonly;
                previousFieldStatic = currentFieldStatic;
                previousAccessLevel = currentAccessLevel;
            }
        }
    }
}
